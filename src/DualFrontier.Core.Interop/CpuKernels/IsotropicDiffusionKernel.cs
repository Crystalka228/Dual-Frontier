using System;

namespace DualFrontier.Core.Interop.CpuKernels;

/// <summary>
/// CPU reference implementation of the 4-neighbour isotropic diffusion
/// stencil used by <c>Vanilla.Magic</c> (G1) and as the equivalence oracle
/// for the future Vulkan compute shader (G-series).
/// </summary>
/// <remarks>
/// <para>Math (per <c>docs/architecture/GPU_COMPUTE.md</c> Mathematical models):</para>
/// <code>
///   ∂φ/∂t = D · ∇²φ - K · φ
///   ∇²φ ≈ φ(N) + φ(S) + φ(E) + φ(W) - 4·φ(center)
/// </code>
/// <para>Boundary: reflective (edge cell uses self as neighbour).</para>
/// <para>The kernel uses a managed scratch array as the next-state buffer.
/// Read pass is zero-copy via <see cref="FieldHandle{T}.AcquireSpan"/>;
/// write pass loops <see cref="FieldHandle{T}.WriteCell"/> per cell, which
/// is slow on CPU (40 000 P/Invokes per iteration on a 200×200 field) but
/// acceptable here because this kernel exists as the GPU equivalence oracle,
/// not as a performance target. G1 replaces it with a Vulkan compute
/// dispatch.</para>
/// </remarks>
public static class IsotropicDiffusionKernel
{
    public readonly struct Parameters
    {
        public float DiffusionCoefficient { get; init; }
        public float DecayCoefficient { get; init; }
        public float DeltaTime { get; init; }

        public static Parameters Default => new()
        {
            DiffusionCoefficient = 0.1f,
            DecayCoefficient = 0.01f,
            DeltaTime = 1.0f
        };
    }

    public static void Run(FieldHandle<float> field, Parameters p, int iterations)
    {
        if (field is null) throw new ArgumentNullException(nameof(field));
        if (iterations < 1) return;

        int width = field.Width;
        int height = field.Height;
        var scratch = new float[width * height];

        for (int iter = 0; iter < iterations; iter++)
        {
            using (var lease = field.AcquireSpan())
            {
                ReadOnlySpan<float> readBuf = lease.Span;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int i = y * width + x;
                        float center = readBuf[i];

                        float north = (y > 0)          ? readBuf[i - width] : center;
                        float south = (y < height - 1) ? readBuf[i + width] : center;
                        float east  = (x < width - 1)  ? readBuf[i + 1]     : center;
                        float west  = (x > 0)          ? readBuf[i - 1]     : center;

                        float laplacian = north + south + east + west - 4.0f * center;
                        float delta = (p.DiffusionCoefficient * laplacian - p.DecayCoefficient * center) * p.DeltaTime;
                        scratch[i] = center + delta;
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    field.WriteCell(x, y, scratch[y * width + x]);
                }
            }
        }
    }
}
