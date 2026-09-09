using GameTextArchive;
using Microsoft.EntityFrameworkCore;
using GameTextArchive.Data;
using GameTextArchive.Search;
using Npgsql;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        // dynamic json for metadata jsonb handling in data source.
        dataSourceBuilder.EnableDynamicJson();

        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        // pass data source from npgsql instead of straight connection string.
        // better data handling.
        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(dataSource));

        builder.Services.AddScoped<SearchService>();
        
        // cross origin resource sharing for asp.net to react and vice versa.
        // http requests from react and response from asp.net.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ReactFrontend", policy =>
            { policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod(); });
        });
        
        var app = builder.Build();
        
        // invoke application after built by builder. 
        app.UseCors("ReactFrontend");
        
        // asp.net api endpoints. define url from which react can get data.
        // text records from database context.
        app.MapGet("/api/text-records", async (GameTextDbContext db) =>
        {
            List<TextRecord> records = await db.TextRecords.ToListAsync();
            return Results.Ok(records);
        });

        // search function from search service to search database. 
        app.MapGet("/api/search", async (string query, int page, int pageSize, SearchService searchService) =>
        {
            var results = await searchService.SearchAsync(query, page, pageSize);
            return Results.Ok(results);
        });
        
        // directly get record from database via global identifier. 
        app.MapGet("/api/records/{id}", async (Guid id, GameTextDbContext db) =>
        {
            var record = await db.TextRecords.FirstOrDefaultAsync(r => r.recordId == id);

            if (record == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(record);
        });
        
        app.Run();
    }
}