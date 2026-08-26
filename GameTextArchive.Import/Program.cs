using GameTextArchive.Data;
using GameTextArchive.Import;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        // generic host created by host app builder.
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        
        // registers database context. passes config options to ef core base class.
        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));
        
        // registers importer services with dependency injection container (DI).
        builder.Services.AddScoped<JsonReader>();
        builder.Services.AddScoped<RecordMapper>();
        builder.Services.AddScoped<RecordImporter>();

        // host build statement (generic host).
        using IHost host = builder.Build();

        // create lifetime boundary for scoped services (DI). 
        using IServiceScope scope = host.Services.CreateScope();

        // request DI container for service object. 
        RecordImporter importer = scope.ServiceProvider.GetRequiredService<RecordImporter>();

        // await importer.ImportAsync(filePath);
    }
}