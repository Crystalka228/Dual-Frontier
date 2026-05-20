using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// Push constant payload for the V1 diffusion compute shader. Layout matches
/// the <c>diffusion.comp</c> <c>push_constant</c> block per S-LOCK-7 alignment
/// audit gate.
/// </summary>
/// <remarks>
/// <para>Layout (must mirror GLSL declaration order):</para>
/// <code>
///   float decayCoefficient (offset 0,  size 4)
///   float deltaTime        (offset 4,  size 4)
///   uint  width            (offset 8,  size 4)
///   uint  height           (offset 12, size 4)
/// </code>
/// <para>Total: 16 bytes — natural 4-byte alignment, no padding required.
/// Vulkan push constants do не follow std140 strictly, but for blocks of only
/// 4-byte scalars (no vectors/matrices/arrays) the C# struct layout с
/// <c>Pack = 4</c> matches the GLSL layout 1:1.</para>
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct DiffusionPushConstants
{
    public float DecayCoefficient;
    public float DeltaTime;
    public uint Width;
    public uint Height;
}
