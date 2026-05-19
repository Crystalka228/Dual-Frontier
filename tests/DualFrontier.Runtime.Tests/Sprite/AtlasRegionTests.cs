using DualFrontier.Runtime.Sprite;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// V0.C.2 AtlasRegion tests per S-LOCK-6 (Q5a code-defined atlas regions).
/// FromPixels existed в V0.C.1 без validation guards; V0.C.2 hardens с bounds checks.
/// </summary>
public sealed class AtlasRegionTests
{
    [Fact]
    public void Full_Is_Unit_Square()
    {
        AtlasRegion full = AtlasRegion.Full;
        full.U0.Should().Be(0f);
        full.V0.Should().Be(0f);
        full.U1.Should().Be(1f);
        full.V1.Should().Be(1f);
    }

    [Theory]
    [InlineData(0, 0, 16, 16, 256, 256, 0.0f, 0.0f, 0.0625f, 0.0625f)]      // top-left tile
    [InlineData(16, 0, 16, 16, 256, 256, 0.0625f, 0.0f, 0.125f, 0.0625f)]   // second tile in row
    [InlineData(0, 240, 16, 16, 256, 256, 0.0f, 0.9375f, 0.0625f, 1.0f)]    // bottom-left tile
    [InlineData(0, 0, 256, 256, 256, 256, 0.0f, 0.0f, 1.0f, 1.0f)]          // full atlas
    [InlineData(32, 48, 16, 16, 128, 128, 0.25f, 0.375f, 0.375f, 0.5f)]     // arbitrary region
    public void FromPixels_With_Valid_Args_Returns_Expected_Normalized_Region(
        int x, int y, int w, int h, int texW, int texH,
        float expectedU0, float expectedV0, float expectedU1, float expectedV1)
    {
        AtlasRegion region = AtlasRegion.FromPixels(x, y, w, h, texW, texH);
        region.U0.Should().BeApproximately(expectedU0, 0.0001f);
        region.V0.Should().BeApproximately(expectedV0, 0.0001f);
        region.U1.Should().BeApproximately(expectedU1, 0.0001f);
        region.V1.Should().BeApproximately(expectedV1, 0.0001f);
    }

    [Theory]
    [InlineData(-1, 0, 16, 16, 256, 256)]      // negative x
    [InlineData(0, -1, 16, 16, 256, 256)]      // negative y
    public void FromPixels_With_Negative_Coordinates_Throws(
        int x, int y, int w, int h, int texW, int texH)
    {
        Action act = () => AtlasRegion.FromPixels(x, y, w, h, texW, texH);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0, 0, 0, 16, 256, 256)]        // zero width
    [InlineData(0, 0, 16, 0, 256, 256)]        // zero height
    [InlineData(0, 0, -1, 16, 256, 256)]       // negative width
    [InlineData(0, 0, 16, -1, 256, 256)]       // negative height
    public void FromPixels_With_NonPositive_Dimensions_Throws(
        int x, int y, int w, int h, int texW, int texH)
    {
        Action act = () => AtlasRegion.FromPixels(x, y, w, h, texW, texH);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0, 0, 16, 16, 0, 256)]         // zero texture width
    [InlineData(0, 0, 16, 16, 256, 0)]         // zero texture height
    [InlineData(0, 0, 16, 16, -1, 256)]        // negative texture width
    public void FromPixels_With_NonPositive_Texture_Dimensions_Throws(
        int x, int y, int w, int h, int texW, int texH)
    {
        Action act = () => AtlasRegion.FromPixels(x, y, w, h, texW, texH);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(250, 0, 16, 16, 256, 256)]     // exceeds width (250 + 16 = 266 > 256)
    [InlineData(0, 250, 16, 16, 256, 256)]     // exceeds height
    [InlineData(256, 0, 1, 1, 256, 256)]       // x at boundary + width = past end
    public void FromPixels_When_Region_Exceeds_Texture_Bounds_Throws(
        int x, int y, int w, int h, int texW, int texH)
    {
        Action act = () => AtlasRegion.FromPixels(x, y, w, h, texW, texH);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
