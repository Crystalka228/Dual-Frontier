namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// UV rectangle within an atlas texture (normalized 0..1 coordinates per RFC convention —
/// (0, 0) = top-left, (1, 1) = bottom-right). Used by <see cref="Sprite"/> к select a
/// sub-region of an atlas texture.
/// </summary>
public readonly record struct AtlasRegion(float U0, float V0, float U1, float V1)
{
    /// <summary>Full atlas (entire texture).</summary>
    public static AtlasRegion Full => new(0f, 0f, 1f, 1f);

    /// <summary>Construct atlas region from pixel coordinates given total texture dimensions.</summary>
    public static AtlasRegion FromPixels(int x, int y, int width, int height, int textureWidth, int textureHeight)
        => new(
            U0: (float)x / textureWidth,
            V0: (float)y / textureHeight,
            U1: (float)(x + width) / textureWidth,
            V1: (float)(y + height) / textureHeight);
}
