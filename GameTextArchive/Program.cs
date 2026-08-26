using GameTextArchive;
using Microsoft.EntityFrameworkCore;
using GameTextArchive.Data;

public class Program
{
    public static void Main(string[] args)
    {
        // builder.
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        // build argument.
        var app = builder.Build();
        
        app.MapGet("/api/text-records", async (GameTextDbContext db) =>
        {
            List<TextRecord> records = await db.TextRecords.ToListAsync();
            return Results.Ok(records);
        });
        
        app.Run();
    }
}