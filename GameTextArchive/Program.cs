using GameTextArchive;
using Microsoft.EntityFrameworkCore;
using GameTextArchive.Data;
using GameTextArchive.Models;
using GameTextArchive.Search;
using Npgsql;
using Microsoft.AspNetCore.OpenApi;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

public class Program
{
    public static void Main(string[] args)
    {
        // serilog redirects where ILogger<T> events go.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override(
                "Microsoft.AspNetCore",
                LogEventLevel.Warning)
            .MinimumLevel.Override(
                "Microsoft.EntityFrameworkCore.Database.Command",
                LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                "logs/gametextarchive-.log",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        // register serilog.
        builder.Logging.ClearProviders();
        builder.Services.AddSerilog();
        Log.Information("===== SERILOG TEST =====");
        
        // machine-readable api description.
        builder.Services.AddOpenApi();
        
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        // dynamic json for metadata jsonb handling in data source.
        dataSourceBuilder.EnableDynamicJson();

        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        // pass data source from npgsql instead of straight connection string.
        // better data handling.
        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(dataSource));

        // cache paginated search results.
        builder.Services.AddMemoryCache();
        
        builder.Services.AddScoped<SearchService>();
        
        const string ApiRateLimitPolicy = "Api";

        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy<string>(
                ApiRateLimitPolicy,
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey:
                        httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            AutoReplenishment = true,
                            QueueLimit = 0
                        }));

            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (
                context,
                cancellationToken) =>
            {
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        error = "Too many requests. Please try again later."
                    },
                    cancellationToken);
            };
        });
        
        // cross origin resource sharing for asp.net to react and vice versa.
        // http requests from react and response from asp.net.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ReactFrontend", policy =>
            { policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod(); });
        });
        
        var app = builder.Build();
        
        // serilog middleware condenses asp.net request log collection to single event.
        app.UseSerilogRequestLogging();
        
        // restrict swagger open api to development only.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            
            // web interface to read open api document.
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/openapi/v1.json",
                    "GameTextArchive API v1");
    
                options.DocumentTitle = "GameTextArchive API";
            });
        }
        
        // routing should be before rate limiting.
        app.UseRouting();
        
        // invoke application after built by builder. 
        app.UseCors("ReactFrontend");
        
        app.UseRateLimiter();
        
        // rate limits to 60 requests per minute.
        var api = app.MapGroup("/api")
            .RequireRateLimiting(ApiRateLimitPolicy);
        
        // asp.net api endpoints. define url from which react can get data.
        // text records from database context.
        api.MapGet("/text-records", async (GameTextDbContext db) =>
        {
            List<TextRecord> records = await db.TextRecords.ToListAsync();
            return Results.Ok(records);
        });

        // search function from search service to search database. 
        api.MapGet("/search",
                async (
                    string query,
                    int page,
                    int pageSize,
                    SearchService searchService, 
                    ILogger<Program> logger) =>
                {
                    logger.LogInformation(
                        "Search requested for {Query}, page {Page}, page size {PageSize}",
                        query,
                        page,
                        pageSize);
                    
                    var results =
                        await searchService.SearchAsync(
                            query,
                            page,
                            pageSize);
                    
                    logger.LogInformation(
                        "Search completed for {Query} with {ResultCount} results",
                        query,
                        results.Items.Count);

                    return Results.Ok(results);
                })
            .WithName("SearchTextRecords")
            .WithTags("Search")
            .WithSummary("Search text records")
            .WithDescription(
                "Searches imported text using full-text search.")
            .Produces<PagedResult<SearchResult>>(StatusCodes.Status200OK);
        
        // directly get record from database via global identifier. 
        api.MapGet("/records/{id}",
                async (
                    Guid id,
                    GameTextDbContext db) =>
                {
                    var record =
                        await db.TextRecords
                            .FirstOrDefaultAsync(
                                r => r.recordId == id);

                    if (record == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(record);
                })
            .WithName("GetTextRecord")
            .WithTags("Records")
            .WithSummary("Get text record by ID")
            .WithDescription(
                "Retrieves text record by database GUID.")
            .Produces<TextRecord>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        app.Run();
    }
}