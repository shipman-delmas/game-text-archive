using System.Text.Json;
using GameTextArchive.Import;

namespace GameTextArchive.Tests;

public class RecordMapperTests
{
    [Fact]
    public void Map_MapsKnownFields()
    {
        const string json = """
                            {
                                "id": "Guard001",
                                "type": "NPC",
                                "name": "Imperial Guard",
                                "speaker_id": "Guard001",
                                "text": "Halt!"
                            }
                            """;

        using JsonDocument document = JsonDocument.Parse(json);

        RecordMapper mapper = new();

        var result = mapper.Map(document.RootElement);

        Assert.Equal("Guard001", result.EditorId);
        Assert.Equal("NPC", result.Type);
        Assert.Equal("Imperial Guard", result.Name);
        Assert.Equal("Guard001", result.SpeakerId);
        Assert.Equal("Halt!", result.Text);
    }

    [Fact]
    public void Map_StoresUnknownFieldsAsMetadata()
    {
        const string json = """
                            {
                                "id": "Guard001",
                                "type": "NPC",
                                "custom_field": "custom value",
                                "quest_id": 42
                            }
                            """;

        using JsonDocument document = JsonDocument.Parse(json);

        RecordMapper mapper = new();

        var result = mapper.Map(document.RootElement);

        Assert.NotNull(result.Metadata);
        Assert.Equal(2, result.Metadata!.Count);

        Assert.Equal(
            "custom value",
            result.Metadata["custom_field"].GetString());

        Assert.Equal(
            42,
            result.Metadata["quest_id"].GetInt32());
    }
}