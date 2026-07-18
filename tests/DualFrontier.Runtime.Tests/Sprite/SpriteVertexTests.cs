using System.Numerics;
using System.Runtime.InteropServices;
using DualFrontier.Runtime.Sprite;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// Sprite vertex + primitive type tests. Critical: Marshal.SizeOf SpriteVertex == 20 bytes
/// per S-LOCK-3 (vertex input description в VulkanSpritePipeline depends on this exact size).
/// </summary>
public sealed class SpriteVertexTests
{
    [Fact]
    public void SpriteVertex_Size_Is_20_Bytes()
    {
        // S-LOCK-3: 8 (Position) + 8 (Uv) + 4 (TintRgba) = 20 bytes.
        Marshal.SizeOf<SpriteVertex>().Should().Be(20);
    }

    [Fact]
    public void PackTintRgba_RoundTrips()
    {
        uint tint = SpriteVertex.PackTintRgba(100, 150, 200, 255);
        ((byte)(tint & 0xFF)).Should().Be(100);
        ((byte)((tint >> 8) & 0xFF)).Should().Be(150);
        ((byte)((tint >> 16) & 0xFF)).Should().Be(200);
        ((byte)((tint >> 24) & 0xFF)).Should().Be(255);
    }

    [Fact]
    public void WhiteTint_Is_All_255()
    {
        SpriteVertex.WhiteTint.Should().Be(0xFFFFFFFFu);
    }

    [Fact]
    public void Constructor_Stores_Fields()
    {
        var v = new SpriteVertex(new Vector2(1, 2), new Vector2(0.3f, 0.4f), 0xDEADBEEF);
        v.Position.Should().Be(new Vector2(1, 2));
        v.Uv.Should().Be(new Vector2(0.3f, 0.4f));
        v.TintRgba.Should().Be(0xDEADBEEFu);
    }

    [Fact]
    public void AtlasRegion_Full_Is_Unit_Square()
    {
        AtlasRegion full = AtlasRegion.Full;
        full.U0.Should().Be(0f);
        full.V0.Should().Be(0f);
        full.U1.Should().Be(1f);
        full.V1.Should().Be(1f);
    }

    [Fact]
    public void AtlasRegion_FromPixels_Computes_Correctly()
    {
        // Sub-region: 16×16 pixel sprite at (32, 48) within a 128×128 atlas.
        var region = AtlasRegion.FromPixels(x: 32, y: 48, width: 16, height: 16,
                                              textureWidth: 128, textureHeight: 128);
        region.U0.Should().BeApproximately(0.25f, 0.001f);
        region.V0.Should().BeApproximately(0.375f, 0.001f);
        region.U1.Should().BeApproximately(0.375f, 0.001f);
        region.V1.Should().BeApproximately(0.5f, 0.001f);
    }

    [Fact]
    public void SpriteTransform_Default_Is_Identity_With_White_Tint()
    {
        SpriteTransform t = SpriteTransform.Default;
        t.Position.Should().Be(Vector2.Zero);
        t.Scale.Should().Be(Vector2.One);
        t.Rotation.Should().Be(0f);
        t.TintRgba.Should().Be(SpriteVertex.WhiteTint);
    }
}
