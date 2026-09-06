namespace GameTextArchive;

// join model and table.
public class TextRecordLexeme
{
    // FIX; IMPLEMENT COMPOSITE KEY
    // CURRENTLY KIND USELESS
    public Guid recordId { get; private set; } 
    public int lexemeId { get; private set; }
    
    public TextRecord record { get; set;}
    public Lexeme lexeme { get; set; }
    
    // word frequency in this specific record.
    public int frequency { get; set; }
}