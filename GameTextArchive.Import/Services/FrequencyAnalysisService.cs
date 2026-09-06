using Microsoft.EntityFrameworkCore;

namespace GameTextArchive.Import.Services;
using GameTextArchive.Data;

public class FrequencyAnalysisService (GameTextDbContext context)
{
    // need db context for referencing lexeme entities and lexeme table in db.
    private GameTextDbContext Context = context;
    
    // method for checking for lexeme, creating if not yet, and incrementing frequency.
    public async Task FrequencyAnalysis(TextRecord record)
    {
        // check if vector is null (many will be).
        // iterate each npgsql vector lexeme in search vector and check if it exists in lexeme table.
        // if not, create and store it. increment frequency for said lexeme. 
        if (record.SearchVector != null)
            foreach (NpgsqlTypes.NpgsqlTsVector.Lexeme word in record.SearchVector)
            {
                Lexeme lexeme;
                
                // nullable query for text archive lexeme matching npgsql lexeme. 
                // iterates list for text archive lexeme with value matching npgsql vector lexeme text.
                Lexeme? existingLexeme = await Context.Lexemes
                        .SingleOrDefaultAsync(l => l.Value == word.Text);

                // if query returns null, create text archive lexeme.
                if (existingLexeme is null)
                {
                    lexeme = new()
                    {
                        Value = word.Text,
                        Frequency = word.Count
                    };
                    
                    Context.Lexemes.Add(lexeme);
                }
                else
                {
                    lexeme = existingLexeme;
                    lexeme.Frequency += word.Count;
                }
                
                TextRecordLexeme? textRecordLexeme = await Context.TextRecordLexemes
                    .SingleOrDefaultAsync(t => t.lexeme == lexeme
                                               && t.record == record);

                if (textRecordLexeme is null)
                {
                    Context.TextRecordLexemes.Add(Join(record, lexeme, word.Count));
                }
                else
                {
                    textRecordLexeme.frequency += word.Count;
                }
            }
        
        await Context.SaveChangesAsync();
    }
    
    // method for creating join model and populating all data for specific record-lexeme combinations. 
    private TextRecordLexeme Join(TextRecord record, Lexeme lexeme, int frequency)
    {
        return new TextRecordLexeme
        {
            record = record,
            lexeme = lexeme,
            frequency = frequency
        };
    }
}