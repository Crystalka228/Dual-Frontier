using DualFrontier.Components.World;
using DualFrontier.Persistence.Compression;
using DualFrontier.Persistence.Snapshots;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Persistence.Tests;

public sealed class TileEncoderTests
{
    [Fact]
    public void TileEncoder_RLE_roundtrip()
    {
        const int width  = 100;
        const int height = 100;
        const int total  = width * height;
        const int rockSlice = total / 20; // 5%

        var tiles = new TerrainKind[total];
        for (int i = 0; i < total; i++)
            tiles[i] = i < total - rockSlice ? TerrainKind.Grass : TerrainKind.Rock;

        var snapshot = new TileMapSnapshot
        {
            Width  = width,
            Height = height,
            Tiles  = tiles
        };

        byte[] encoded = TileEncoder.Encode(snapshot);
        TileMapSnapshot decoded = TileEncoder.Decode(encoded, width, height);

        decoded.Width.Should().Be(width);
        decoded.Height.Should().Be(height);
        decoded.Tiles.Should().Equal(tiles);
        encoded.Length.Should().BeLessThan(100,
            "two homogeneous biomes must collapse to a handful of (count, kind) pairs");
    }

    [Fact]
    public void TerrainKind_Unknown_IsZero_SoZeroInitialisedTilesAreDetectable()
    {
        // F21: default(TerrainKind) / zero-initialised storage must read as Unknown, not Grass.
        ((byte)TerrainKind.Unknown).Should().Be(0);
        default(TerrainKind).Should().Be(TerrainKind.Unknown,
            "a zero-initialised tile must read as Unknown, not a valid biome (F21)");
        ((byte)TerrainKind.Grass).Should().Be(1, "explicit values pin the persisted wire format");
    }

    [Fact]
    public void TileEncoder_PersistsExplicitTerrainKindByteValues()
    {
        // 1×2 map: one Unknown then one Grass → two RLE pairs of (count: ushort LE)(kind: byte).
        var tiles = new[] { TerrainKind.Unknown, TerrainKind.Grass };
        var snapshot = new TileMapSnapshot { Width = 2, Height = 1, Tiles = tiles };

        byte[] encoded = TileEncoder.Encode(snapshot);

        // Unknown=0, Grass=1 (save format v2) → [count=1, kind=0][count=1, kind=1].
        encoded.Should().Equal(new byte[] { 1, 0, 0, 1, 0, 1 },
            "the persisted kind byte is the explicit TerrainKind value (F21, save format v2)");
        TileEncoder.Decode(encoded, 2, 1).Tiles.Should().Equal(tiles);
    }
}
