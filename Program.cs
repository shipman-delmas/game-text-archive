using Microsoft.EntityFrameworkCore;
using GameTextArchive.Config;
using GameTextArchive.Data;
using GameTextArchive.Search;

public class Program
{
    public static void Main(string[] args)
    {
        // BUILDER.
        // builder sets up web app before start.
        var builder = WebApplication.CreateBuilder(args);
        
        // REGISTRATION.
        // calls builder services to register db context. provides options for db context and tells
        // ef core to use postgresql with the provided connection string.
        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        var app = builder.Build();
        
        app.MapGet("/api/text-records", async (GameTextDbContext db) =>
        {
            var records = await db.TextRecords.ToListAsync();
            return Results.Ok(records);
        });
        
        app.Run();
    }
}