using System;

namespace DualFrontier.Core.Interop.CpuKernels;

/// <summary>
/// CPU reference implementation of the anisotropic 4-neighbour diffusion
/// stencil with per-cell conductivity. Equivalence oracle для V1 diffusion
/// compute shader (anisotropic variant) per VULKAN_SUBSTRATE.md §1.2 + §5.1.
/// </summary>
/// <remarks>
/// <para>Math (per <c>docs/architecture/VULKAN_SUBSTRATE.md</c> §5.1 anisotropic variant):</para>
/// <code>
///   flow(self → neighbour) = min(D_self, D_neighbour) · (P_neighbour - P_self)
///   ∂P/∂t ≈ Σ flow(self → neighbour) - K · P
/// </code>
/// <para>The asymmetric <c>min(D_self, D_neighbour)</c> rule guarantees that
/// flow is blocked whenever either side has zero conductivity (insulator),
/// which is the property that makes wire-path propagation emerge automatically
/// from a uniform stencil — high-D cells along the wire conduct freely с each
/// other, low-D off-wire cells offer near-zero flow к their high-D neighbours.</para>
/// <para>Boundary: missing neighbour contributes zero flow (equivalent к
/// reflective boundary with self-conductivity, since <c>min(D, D) · (P - P) = 0</c>).
/// Matches GLSL shader convention which skips out-of-bounds reads via <c>if</c>
/// guards on each dimension.</para>
/// <para>Conductivity map is snapshot once at <see cref="Run"/> entry; subsequent
/// per-cell <c>GetConductivity</c> calls would require P/Invoke per cell per
/// iteration. The CPU reference exists as the GPU equivalence oracle, не as a
/// performance target — V1 compute shader replaces it on the GPU at gameplay
/// runtime, where conductivity lives в a storage buffer alongside the field.</para>
/// </remarks>
public static class AnisotropicDiffusionKernel
{
    public readonly struct Parameters
    {
        public float DecayCoefficient { get; init; }
        public float DeltaTime { get; init; }

        public static Parameters Default => new()
        {
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

        var conductivity = new float[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                conductivity[y * width + x] = field.GetConductivity(x, y);
            }
        }

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
                        float dSelf = conductivity[i];

                        float flowSum = 0.0f;

                        if (y > 0)
                        {
                            int nIdx = i - width;
                            flowSum += MathF.Min(dSelf, conductivity[nIdx]) * (readBuf[nIdx] - center);
                        }
                        if (y < height - 1)
                        {
                            int sIdx = i + width;
                            flowSum += MathF.Min(dSelf, conductivity[sIdx]) * (readBuf[sIdx] - center);
                        }
                        if (x < width - 1)
                        {
                            int eIdx = i + 1;
                            flowSum += MathF.Min(dSelf, conductivity[eIdx]) * (readBuf[eIdx] - center);
                        }
                        if (x > 0)
                        {
                            int wIdx = i - 1;
                            flowSum += MathF.Min(dSelf, conductivity[wIdx]) * (readBuf[wIdx] - center);
                        }

                        float delta = (flowSum - p.DecayCoefficient * center) * p.DeltaTime;
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
