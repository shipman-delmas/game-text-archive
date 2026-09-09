using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GameTextArchive.Import.Services;
using GameTextArchive.Data;

public class FrequencyAnalysisService (GameTextDbContext context)
{
    // need db context for referencing lexeme entities and lexeme table in db.
    private GameTextDbContext Context = context;
    
    // method for checking for lexeme, creating if not yet, and incrementing frequency.
    public async Task FrequencyAnalysis(List<TextRecord> batch)
    {
        HashSet<String> vectorValues = [];
        
        // add text value of all npgsql lexeme in record batch to hashset for future query. 
        foreach (var record in batch)
        {
            if (record.SearchVector is null) continue;
            
            foreach (var word in record.SearchVector)
            {
                vectorValues.Add(word.Text);
            }
        }

        // single query to database to check for existing archive lexemes.
        // (i.e. a lexeme like Balmora would probably already exist from previous record.)
        var existingArchiveLexemes = await Context.Lexemes
            .Where(l => l.Value != null && vectorValues.Contains(l.Value))
            .ToListAsync();
            
        // DICTIONARY POPULATION GOES HERE.
        Dictionary<string, Lexeme> lexemeCache = existingArchiveLexemes.ToDictionary(l => l.Value);
        
        foreach (var record in batch)
        {
            if (record.SearchVector is null) continue;
            
            foreach (var word in record.SearchVector)
            {
                if (!lexemeCache.TryGetValue(word.Text, out Lexeme? archiveLexeme))
                {
                    archiveLexeme = new Lexeme()
                    {
                        Value = word.Text,
                        Frequency = word.Count
                    };
                    Context.Lexemes.Add(archiveLexeme);
                    lexemeCache.Add(word.Text, archiveLexeme);
                }
                // if not null, if archive lexeme is found and returned, increment the global frequency for that
                // lexeme by record frequency. 
                else { archiveLexeme.Frequency += word.Count; }
            
                // CREATE NEW JOIN ENTITIES HERE. 
                Context.TextRecordLexemes.Add(new TextRecordLexeme
                {
                    record = record,
                    lexeme = archiveLexeme,
                    frequency = word.Count
                });
            }
        }
    }
}