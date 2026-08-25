using System.Text.Json;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace GameTextArchive;

// database table.
public class TextRecord
{
    // basic data fields.
    public long id { get; set; }
    public RecordType type { get; set; }
    public string? speaker_id { get; set; }
    public string? text { get; set; }
    
    // derived data fields.
    public string SourceFile { get; set; }
    public DateTime ImportedAt { get; set; }
    
    // metadata.
    public Dictionary<string, JsonElement>? Metadata { get; set; }
    
    // search vector. 
    [JsonIgnore]
    public NpgsqlTsVector? SearchVector { get; set; }
}