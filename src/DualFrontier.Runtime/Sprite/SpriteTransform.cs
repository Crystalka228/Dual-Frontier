using System.Numerics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Sprite transform parameters: position + scale + rotation + tint color. Composed in NDC
/// or world space depending on the Camera2D MVP applied in the sprite vertex shader.
///
/// V0.C.1 single-sprite test renders при NDC origin (0,0) с scale (0.3, 0.3) — half-extent
/// scale = 0.15 NDC units, sprite fills ~30% of viewport width.
/// </summary>
public readonly record struct SpriteTransform(
    Vector2 Position,
    Vector2 Scale,
    float Rotation,
    uint TintRgba)
{
    /// <summary>Default transform: origin, unit scale, no rotation, white tint.</summary>
    public static SpriteTransform Default => new(
        Position: Vector2.Zero,
        Scale: Vector2.One,
        Rotation: 0f,
        TintRgba: SpriteVertex.WhiteTint);
}
