namespace GameTextArchive;

public class Lexeme
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Value { get; set; }
    public int? Frequency { get; set; }
}