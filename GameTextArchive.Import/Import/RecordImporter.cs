using System.Text.Json;
using GameTextArchive.Data;
using GameTextArchive.Import.Services;

namespace GameTextArchive.Import;

public class RecordImporter (GameTextDbContext context, JsonReader reader, RecordMapper mapper)
{
    private readonly GameTextDbContext Context = context;
    private readonly JsonReader Reader = reader;
    private readonly RecordMapper Mapper = mapper;
    
    FrequencyAnalysisService Analyst = new(context);

    private const int BatchSize = 1000; 
    
    // connects reader and mapper with database via database context.
    public async Task ImportAsync(string outputFile, CancellationToken cancellationToken = default)
    {
        List<TextRecord> batch = new();
        
        // iterate json elements in async file stream, map each to record, and process in two phase batching.
        await foreach (JsonElement json in reader.ReadAsync(outputFile, cancellationToken))
        {
            TextRecord record = mapper.Map(json);

            record.SourceFile = outputFile;
            record.ImportedAt = DateTime.UtcNow;
            
            // add mapped records to tracked entities and batch list. 
            context.TextRecords.Add(record);
            batch.Add(record);
            
            if (batch.Count >= BatchSize)
            {
                // save batch to database and generate search vector column.
                await context.SaveChangesAsync(cancellationToken);

                // use batch list to reference records that were just saved to database.
                // call frequency analysis on each record now that they have search vectors. 
                await Analyst.FrequencyAnalysis(batch);

                // save lexeme entities and join entities to database. 
                await Context.SaveChangesAsync(cancellationToken);
                
                // clear tracker and batch. 
                context.ChangeTracker.Clear();
                batch.Clear();
            }
        }

        // perform batch processing on final batch which will likely not reach max size. 
        if (batch.Count > 0)
        {
            await Context.SaveChangesAsync(cancellationToken);

            await Analyst.FrequencyAnalysis(batch);
            
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();
        }
    }
}