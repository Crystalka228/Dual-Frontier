namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.1 sprite handle: composite of <see cref="SpriteTexture"/> reference, atlas UV region,
/// and transform parameters. Minimal handle — no caching, no batching membership tracking;
/// V0.C.2 will extend с full sprite handle for batched rendering.
/// </summary>
public readonly record struct Sprite(
    SpriteTexture Texture,
    AtlasRegion Region,
    SpriteTransform Transform)
{
    /// <summary>Construct sprite with default transform + full atlas region.</summary>
    public static Sprite Create(SpriteTexture texture)
        => new(texture, AtlasRegion.Full, SpriteTransform.Default);
}
