namespace GameTextArchive;

// join model and table.
public class TextRecordLexeme
{
    private Guid recordId { get; set; } 
    private int lexemeId { get; set; }
    
    private TextRecord record { get; set;}
    private Lexeme lexeme { get; set; }
    
    // word frequency in this specific record.
    private int frequency { get; set; }
}