using System.Reflection;
using System.Text.Json;
using GameTextArchive.Config;

namespace GameTextArchive.Search;

public class JsonReader ()
{
    // asynchronously produce json element enumerable sequentially consumable by caller of method.
    // FIX: find more seamless way to provide filepath?
    public async IAsyncEnumerable<JsonElement> ReadAsync(string filePath,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // opens file as a filestream to read contents sequentially instead of loading all contents into memory at once.
        await using FileStream stream = File.OpenRead(filePath);

        // iterate each json element and return them sequentially.
        await foreach (JsonElement element in JsonSerializer.DeserializeAsyncEnumerable<JsonElement>(
                           stream, cancellationToken: cancellationToken))
        {
            yield return element;
        }
    }
    
    // holding onto this method for reference on data/metadata separation during import.
    /*
    public TextRecord? ImportJson(string filePath)
    {
        string json = File.ReadAllText(filePath);
        JsonDocument document = JsonDocument.Parse(json);
        
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
        record.ImportedAt = DateTime.Now;
        
        record.Metadata = metadata;
        
        return record;
    }
    */
}