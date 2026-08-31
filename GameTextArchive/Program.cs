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

        dataSourceBuilder.EnableDynamicJson();

        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(dataSource));

        builder.Services.AddScoped<SearchService>();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ReactFrontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        var app = builder.Build();
        
        app.UseCors("ReactFrontend");
        
        app.MapGet("/api/text-records", async (GameTextDbContext db) =>
        {
            List<TextRecord> records = await db.TextRecords.ToListAsync();
            return Results.Ok(records);
        });

        app.MapGet("api/search", async (string query, SearchService searchService) =>
        {
            var results = await searchService.SearchAsync(query);
            return Results.Ok(results);
        });
        
        app.Run();
    }
}