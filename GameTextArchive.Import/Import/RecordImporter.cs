using System.Text.Json;
using GameTextArchive.Data;

namespace GameTextArchive.Import;

public class RecordImporter (GameTextDbContext context, JsonReader reader, RecordMapper mapper)
{
    private readonly GameTextDbContext Context = context;
    private readonly JsonReader Reader = reader;
    private readonly RecordMapper Mapper = mapper;

    private const int BatchSize = 1000; 
    
    // connects reader and mapper with database via database context.
    public async Task ImportAsync(string outputFile, CancellationToken cancellationToken = default)
    {
        int count = 0;
        
        // iterate json elements in async file stream and map each to record.
        await foreach (JsonElement json in reader.ReadAsync(outputFile, cancellationToken))
        {
            TextRecord record = mapper.Map(json);
            
            // after mapping and before sending entity to db, call method for frequency analysis.
            // frequencyAnalyst.FrequencyAnalysis(record);

            record.SourceFile = outputFile;
            record.ImportedAt = DateTime.UtcNow;
            
            // add mapped records to database set in database context.
            context.TextRecords.Add(record);

            ++count;

            if (count % BatchSize == 0)
            {
                // instance method sends changes to postgresql via sql commands. 
                await context.SaveChangesAsync(cancellationToken);
                // instance method clears ef core entity tracker. 
                context.ChangeTracker.Clear();
            }
        }
        
        await context.SaveChangesAsync(cancellationToken);
        context.ChangeTracker.Clear();
    }
}