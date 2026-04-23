using System.IO;
using System.Text.Json;
using DualFrontier.Application.Scene;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scene;

public sealed class SceneDefSerializationTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    [Fact]
    public void SampleDfscene_Roundtrips_Through_SceneDef()
    {
        string path = Path.Combine("..", "..", "..", "..", "..",
            "assets", "scenes", "sample.dfscene");
        string json = File.ReadAllText(path);

        SceneDef? scene = JsonSerializer.Deserialize<SceneDef>(json, Options);

        scene.Should().NotBeNull();
        scene!.Version.Should().Be(SceneDef.CurrentVersion);
        scene.Name.Should().Be("sample_colony");
        scene.Tilemap.Width.Should().Be(10);
        scene.Tilemap.Height.Should().Be(10);
        scene.Entities.Should().HaveCount(1);
        scene.Entities[0].Prefab.Should().Be("core:pawn");
        scene.Markers.Should().HaveCount(1);
        scene.Markers[0].Id.Should().Be("player_spawn");
    }

    [Fact]
    public void SceneDef_Serialize_Then_Deserialize_Is_Idempotent()
    {
        SceneDef original = new(
            Version: SceneDef.CurrentVersion,
            Name: "roundtrip",
            Tilemap: TilemapDef.Empty,
            Entities: System.Array.Empty<EntitySpawnDef>(),
            Markers: System.Array.Empty<MarkerDef>(),
            Metadata: new SceneMetadata("temperate", 0.5f, "test",
                System.DateTimeOffset.UnixEpoch)
        );

        string json = JsonSerializer.Serialize(original, Options);
        SceneDef? roundtripped = JsonSerializer.Deserialize<SceneDef>(json, Options);

        roundtripped.Should().NotBeNull();
        roundtripped!.Name.Should().Be("roundtrip");
        roundtripped.Metadata.Biome.Should().Be("temperate");
    }
}
