using GameTextArchive.Data;
using GameTextArchive.Import.Conversion;
using GameTextArchive.Import.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace GameTextArchive.Import;


// FIX: CREATE OUTPUT JSON W/O USER INPUT.
//      IMPLEMENT FILE PICKER.
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
        
        // path class combine method to create string file path for new directory.
        string outputDirectory = Path.Combine(builder.Environment.ContentRootPath, "Converted");
        // create new directory with new file path.
        Directory.CreateDirectory(outputDirectory);
        // combine method creates file path for output json.
        string outputFile = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(inputFile)}.json");
        
        // execute ingestion pipeline.
        if ((inputFile is not null)) converter.Execute(inputFile, outputFile);
        
        // importer receives output file.
        await importer.ImportAsync(outputFile);
        
        Console.WriteLine($"{outputFile} successfully imported to database.");
        
        Console.WriteLine("--------------------------------------------------");
    }
}