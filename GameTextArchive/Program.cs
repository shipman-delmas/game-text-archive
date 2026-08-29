using GameTextArchive;
using Microsoft.EntityFrameworkCore;
using GameTextArchive.Data;
using GameTextArchive.Search;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<SearchService>();
        
        var app = builder.Build();
        
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