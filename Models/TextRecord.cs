using System.Text.Json;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace GameTextArchive;

public class TextRecord
{
    // basic data fields.
    public RecordType type { get; set; }
    public long id { get; set; }
    public string? speaker_id { get; set; }
    public string? text { get; set; }
    
    // derived data fields.
    public string SourceFile { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Dictionary<string, JsonElement>? Metadata { get; set; }
    
    [JsonIgnore]
    public NpgsqlTsVector? SearchVector { get; set; }
    [JsonIgnore]
    public NpgsqlTsVector FrequencyVector { get; set; }
}