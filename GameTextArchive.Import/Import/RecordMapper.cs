using System.Text.Json;

namespace GameTextArchive.Import;

public class RecordMapper
{
    // switch cases to map each key-value in existing json element to matching key in new record.
    public TextRecord Map(JsonElement json)
    {
        TextRecord record = new();
        Dictionary<string, JsonElement> metadata = new();
        
        // foreach statement calls json element extension method to get an enumerator for property iteration.
        foreach (JsonProperty property in json.EnumerateObject())
        {
            // cases based on key name in json element.
            switch (property.Name)
            {
                // sets extension property of new record to value of matching key in existing json element.
                case "id":
                    record.id = property.Value.GetString();
                    break;
                
                case "type":
                    record.type = property.Value.GetString();
                    break;
                
                case "speaker_id":
                    record.speaker_id = property.Value.GetString();
                    break;
                
                case "text":
                    record.text = property.Value.GetString();
                    break;
                
                // key of same name found or created and assigned value from json element property.
                default:
                    metadata[property.Name] = property.Value.Clone();
                    break;
            }
        }

        record.Metadata = metadata;
        return record;
    }
}