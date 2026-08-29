using GameTextArchive.Data;
using GameTextArchive.Import.Conversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameTextArchive.Import;

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
        builder.Services.AddScoped<Tes3ConvRunner>();
        
        // host build statement (generic host).
        using IHost host = builder.Build();

        // create lifetime boundary for scoped services (DI). 
        using IServiceScope scope = host.Services.CreateScope();
        
        Tes3ConvRunner converter = scope.ServiceProvider.GetRequiredService<Tes3ConvRunner>();

        // request DI container for service object. 
        RecordImporter importer = scope.ServiceProvider.GetRequiredService<RecordImporter>();
        
        Console.WriteLine("What is the existing input file name? ");
        string? inputFile = Console.ReadLine();
        
        Console.WriteLine("What is the desired output file name? ");
        string? outputFile = Console.ReadLine();
        
        // converter creates output file.
        if ((inputFile is not null) && (outputFile is not null)) 
            converter.Execute(inputFile, outputFile);

        // importer receives output file.
        if (outputFile is not null) 
            await importer.ImportAsync(outputFile);
    }
}