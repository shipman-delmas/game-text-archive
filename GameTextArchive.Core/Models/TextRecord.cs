using System.Text.Json;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace GameTextArchive;

// database table.
public class TextRecord
{
    // postgres id.
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // basic data fields.
    public string? EditorId { get; set; }
    public string? Type { get; set; }
    public string? SpeakerId { get; set; }
    public string? Text { get; set; }
    
    // derived data fields.
    public string SourceFile { get; set; }
    public DateTime ImportedAt { get; set; }
    
    // metadata.
    public Dictionary<string, JsonElement>? Metadata { get; set; }
    
    // search vector. 
    [JsonIgnore]
    public NpgsqlTsVector? SearchVector { get; set; }
}