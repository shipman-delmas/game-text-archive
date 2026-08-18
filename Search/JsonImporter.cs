using System.Reflection;
using System.Text.Json;
using GameTextArchive.Config;

namespace GameTextArchive.Search;

public class JsonImporter (RecordFieldDefinitions recordFieldDefinitions)
{
    // method for importing json to text record. separates metadata from core fields.
    public TextRecord? ImportJson(string filePath)
    {
        // read json file to string. parse string to json document. 
        string json = File.ReadAllText(filePath);
        JsonDocument document = JsonDocument.Parse(json);
        
        // create new text record and new dictionary.
        TextRecord record = new();
        Dictionary<string, JsonElement> metadata = new();
        
        // iterate json properties (key-value pairs) and check if key is a core field.
        foreach (JsonProperty p in document.RootElement.EnumerateObject())
        {
            if (recordFieldDefinitions.IsBasicField(p.Name))
            {
                // get record property equal to json element using reflection. 
                var property = typeof(TextRecord).GetProperty(p.Name);
                
                // gets data type of specific property.
                var type = property.PropertyType;

                // deserializes json element to proper data type.
                var value = p.Value.Deserialize(type);

                // sets record property to deserialized value. 
                property.SetValue(record, value); 
            }
            else { metadata.Add(p.Name, p.Value); }
        }
        
        // derived data.
        record.SourceFile = Path.GetFileNameWithoutExtension(filePath);
        record.Timestamp = DateTime.Now;
        
        record.Metadata = metadata;
        
        return record;
    }
}