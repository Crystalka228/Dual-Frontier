using System.Numerics;
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Sprite vertex format per S-LOCK-3. 20 bytes — verified via Marshal.SizeOf test gate.
/// Vertex attribute layout for VulkanSpritePipeline:
///   binding 0, stride 20, input rate VERTEX
///     attribute 0: Position (R32G32_SFLOAT, offset 0)
///     attribute 1: Uv (R32G32_SFLOAT, offset 8)
///     attribute 2: TintRgba (R8G8B8A8_UNORM, offset 16) — expands к vec4 in shader via UNORM
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SpriteVertex
{
    public Vector2 Position;      // 8 bytes — world или screen space, depending on shader MVP
    public Vector2 Uv;            // 8 bytes — texture UV (normalized 0..1)
    public uint TintRgba;         // 4 bytes — packed RGBA: R = (TintRgba >> 0) & 0xFF, ...

    public SpriteVertex(Vector2 position, Vector2 uv, uint tintRgba)
    {
        Position = position;
        Uv = uv;
        TintRgba = tintRgba;
    }

    /// <summary>Pack RGBA bytes into a single uint for SpriteVertex tint.</summary>
    public static uint PackTintRgba(byte r, byte g, byte b, byte a) =>
        (uint)r | ((uint)g << 8) | ((uint)b << 16) | ((uint)a << 24);

    /// <summary>Fully-opaque white tint (no color modulation; sample × white = sample).</summary>
    public static readonly uint WhiteTint = PackTintRgba(255, 255, 255, 255);
}
