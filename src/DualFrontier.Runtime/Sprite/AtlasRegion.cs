namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// UV rectangle within an atlas texture (normalized 0..1 coordinates per RFC convention —
/// (0, 0) = top-left, (1, 1) = bottom-right). Used by <see cref="Sprite"/> к select a
/// sub-region of an atlas texture.
///
/// V0.C.2: FromPixels hardened с validation guards per S-LOCK-6 (Q5a code-defined atlas regions).
/// </summary>
public readonly record struct AtlasRegion(float U0, float V0, float U1, float V1)
{
    /// <summary>Full atlas (entire texture).</summary>
    public static AtlasRegion Full => new(0f, 0f, 1f, 1f);

    /// <summary>
    /// Construct atlas region from pixel coordinates within a texture of known size.
    /// V0.C.2: validation guards added per S-LOCK-6 (out-of-range coordinates rejected).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when coordinates are negative, dimensions are non-positive, or region exceeds texture bounds.
    /// </exception>
    public static AtlasRegion FromPixels(int x, int y, int width, int height, int textureWidth, int textureHeight)
    {
        if (x < 0 || y < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(x),
                $"Pixel coordinates must be non-negative (x={x}, y={y}).");
        }
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width),
                $"Region dimensions must be positive (width={width}, height={height}).");
        }
        if (textureWidth <= 0 || textureHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(textureWidth),
                $"Texture dimensions must be positive (textureWidth={textureWidth}, textureHeight={textureHeight}).");
        }
        if (x + width > textureWidth || y + height > textureHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(width),
                $"Region ({x},{y},{width}x{height}) exceeds texture bounds ({textureWidth}x{textureHeight}).");
        }

        return new AtlasRegion(
            U0: (float)x / textureWidth,
            V0: (float)y / textureHeight,
            U1: (float)(x + width) / textureWidth,
            V1: (float)(y + height) / textureHeight);
    }
}
