using GameTextArchive.Data;
using GameTextArchive.Import.Conversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace GameTextArchive.Import;

public class Program
{
    public static async Task Main(string[] args)
    {
        // generic host created by host app builder.
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        
        // check for and configure connection string
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"Connection string loaded: {!string.IsNullOrWhiteSpace(connectionString)}");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DefaultConnection was not loaded."
            );
        }
        
        // configure Npgsql and create data builder. passes connection string.
        NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        // allow serialization of common runtime objects (specifically for metadata dictionary to jsonb).
        dataSourceBuilder.EnableDynamicJson();
        // build data source configured by npgsql.
        NpgsqlDataSource dataSource = dataSourceBuilder.Build();
        
        // registers database context. passes npgsql data source and connection string..
        builder.Services.AddDbContext<GameTextDbContext>(options => options.UseNpgsql(dataSource));
        
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
        
        Console.WriteLine("--------------------------------------------------");
        
        Console.WriteLine("What is the existing input file name? ");
        string? inputFile = Console.ReadLine();
        
        Console.WriteLine("--------------------------------------------------");
        
        Console.WriteLine("What is the desired output file name? ");
        string? outputFile = Console.ReadLine();
        
        Console.WriteLine("--------------------------------------------------");
        
        Console.WriteLine("Attempting file conversion...");
        
        // converter creates output file.
        if ((inputFile is not null) && (outputFile is not null)) 
            converter.Execute(inputFile, outputFile);

        // importer receives output file.
        if (outputFile is not null) 
            await importer.ImportAsync(outputFile);
        
        Console.WriteLine($"{outputFile} successfully imported to database.");
        
        Console.WriteLine("--------------------------------------------------");
    }
}