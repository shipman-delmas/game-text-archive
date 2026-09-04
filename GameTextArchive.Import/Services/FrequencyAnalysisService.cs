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
        Lexeme lexeme = new();

        // check if vector is null (many will be).
        // iterate each npgsql vector lexeme in search vector and check if it exists in lexeme table.
        // if not, create and store it. increment frequency for said lexeme. 
        if (record.SearchVector != null)
            foreach (NpgsqlTypes.NpgsqlTsVector.Lexeme word in record.SearchVector)
            {
                // HOW TO STORE ACTUAL LEXEME IN VAR SO WE CAN ++LEXEME.FREQUENCY
                // WITHOUT QUERYING DB TWICE?
                bool lexemeExists = await context.Lexemes
                    .AnyAsync(l => l.Equals(word));

                // IF WE CAN REFERENCE LEXEME IN ONE QUERY, USE !LEXEME IS NULL.
                if (!lexemeExists)
                {
                    lexeme.Value = word.ToString();
                    context.Lexemes.Add(lexeme);
                }
                
                // INCREMENT LEXEME FREQUENCY.
                // HERE.
            }

        var recordLexeme = await Join(record, lexeme);
        context.TextRecordLexemes.Add(recordLexeme);
    }
    
    // method for creating join model and populating all data for specific record-lexeme combinations. 
    private async Task<TextRecordLexeme> Join(TextRecord record, Lexeme lexeme)
    {
        TextRecordLexeme recordLexeme = new();
        
        // ASSIGN VALUES TO RECORD-LEXEME. 

        return recordLexeme;
    }
}