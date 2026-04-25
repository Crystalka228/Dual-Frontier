using DualFrontier.Components.World;
using DualFrontier.Persistence.Compression;
using DualFrontier.Persistence.Snapshots;
using FluentAssertions;
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
}
