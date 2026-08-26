using System.Reflection;
using System.Text.Json;

namespace GameTextArchive.Import;

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
}