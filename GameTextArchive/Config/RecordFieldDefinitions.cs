using System.Text.Json;

namespace GameTextArchive.Config;

public class RecordFieldDefinitions
{
    private readonly HashSet<string> _basicFields = new()
    {
        "type",
        "id",
        "speaker_id",
        "text",
    };
    
    public bool IsBasicField(string fieldName) 
    {
        return _basicFields.Contains(fieldName);
    }
}