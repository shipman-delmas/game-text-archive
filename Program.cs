using GameTextArchive;
using Microsoft.EntityFrameworkCore;
using GameTextArchive.Config;
using GameTextArchive.Data;
using GameTextArchive.Search;

public class Program
{
    public static void Main(string[] args)
    {
        // builder.
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        // registers database context. passes config options to ef core base class.
        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        // registers classes with ASP.NET core and instantiates objects automatically for every HTTP request.
        builder.Services.AddScoped<JsonReader>();
        builder.Services.AddScoped<RecordMapper>();
        builder.Services.AddScoped<RecordImporter>();

        // no controller classes in use yet. only minimal endpoints. 
        // endpoints are http routes and operations. controller group related endpoints. 
        // builder.Services.AddControllers();
        
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