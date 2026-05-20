using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.CpuKernels;
using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Compute;

/// <summary>
/// V1-7/V1-9 — CPU/GPU equivalence gate per S-LOCK-3 + VULKAN_SUBSTRATE.md
/// section 11. Every V1 compute shader has а tolerance-bounded CPU reference;
/// these tests verify the GPU shader matches that reference within tolerance.
/// </summary>
/// <remarks>
/// Mathematical equivalence rationale: when conductivity is uniform (all cells
/// hold D_self = D_neighbour = D), the asymmetric flow rule
/// flow = min(D_self, D_neighbour) * delta collapses к the standard isotropic
/// stencil D * laplacian. So IsotropicDiffusionKernel serves as the oracle для
/// uniform-D inputs к V1DiffusionPipeline. AnisotropicDiffusionKernel serves
/// as the oracle для per-cell varying D.
/// </remarks>
public sealed class V1DiffusionEquivalenceTests : IDisposable
{
    private const float IsotropicTolerance = 0.001f;
    private const float AnisotropicTolerance = 0.001f;

    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly NativeWorld _cpuWorld;
    private readonly NativeWorld _gpuWorld;

    public V1DiffusionEquivalenceTests()
    {
        var opts = new WindowOptions { Title = "V1Equiv", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _cpuWorld = new NativeWorld();
        _gpuWorld = new NativeWorld();
    }

    public void Dispose()
    {
        _gpuWorld.Dispose();
        _cpuWorld.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void Isotropic_uniform_D_matches_CPU_kernel_within_tolerance()
    {
        const int W = 32, H = 32;
        const float D = 0.1f, Decay = 0.0f, Dt = 1.0f;
        const int Iterations = 5;

        // CPU side: IsotropicDiffusionKernel
        var cpuField = _cpuWorld.Fields.Register<float>("equiv.iso.cpu", W, H);
        cpuField.WriteCell(W / 2, H / 2, 100f);
        var cpuParams = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = D,
            DecayCoefficient = Decay,
            DeltaTime = Dt,
        };
        IsotropicDiffusionKernel.Run(cpuField, cpuParams, Iterations);

        // GPU side: V1DiffusionPipeline с uniform conductivity D
        var binding = new FieldStorageBinding(_gpuWorld);
        binding.Attach(_instance, _device).Should().BeTrue();
        var gpuField = _gpuWorld.Fields.Register<float>("equiv.iso.gpu", W, H);
        gpuField.WriteCell(W / 2, H / 2, 100f);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                gpuField.SetConductivity(x, y, D);

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, "equiv.iso.pipeline", spirv);
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay, DeltaTime = Dt, Width = W, Height = H,
        };
        for (int i = 0; i < Iterations; i++)
        {
            pipeline.ExecuteIteration("equiv.iso.gpu", pc, dispatchX: 4, dispatchY: 4)
                    .Should().BeTrue($"iteration {i} dispatches successfully");
        }

        // Compare cell-by-cell within tolerance.
        int mismatchCount = 0;
        float maxAbsDiff = 0;
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                float cpu = cpuField.ReadCell(x, y);
                float gpu = gpuField.ReadCell(x, y);
                float diff = MathF.Abs(cpu - gpu);
                if (diff > maxAbsDiff) maxAbsDiff = diff;
                if (diff > IsotropicTolerance) mismatchCount++;
            }
        }
        mismatchCount.Should().Be(0,
            $"all cells must agree within {IsotropicTolerance:F4} tolerance. " +
            $"Max abs diff observed: {maxAbsDiff:F6}");
    }

    [Fact]
    public void Isotropic_corner_source_reflective_boundary_matches_CPU_within_tolerance()
    {
        // V1-8 edge case: source at corner (0,0) exercises the reflective-boundary
        // code path on the CPU (edge cell uses self as out-of-bounds neighbour) and
        // the "skip out-of-bounds neighbour" code path в the GLSL shader. These
        // collapse к the same delta because min(D, D) * (P - P) = 0.
        const int W = 32, H = 32;
        const float D = 0.1f, Decay = 0.0f, Dt = 1.0f;
        const int Iterations = 5;

        var cpuField = _cpuWorld.Fields.Register<float>("equiv.iso.corner.cpu", W, H);
        cpuField.WriteCell(0, 0, 100f);
        var cpuParams = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = D, DecayCoefficient = Decay, DeltaTime = Dt,
        };
        IsotropicDiffusionKernel.Run(cpuField, cpuParams, Iterations);

        var binding = new FieldStorageBinding(_gpuWorld);
        binding.Attach(_instance, _device).Should().BeTrue();
        var gpuField = _gpuWorld.Fields.Register<float>("equiv.iso.corner.gpu", W, H);
        gpuField.WriteCell(0, 0, 100f);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                gpuField.SetConductivity(x, y, D);

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, "equiv.iso.corner.pipeline", spirv);
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay, DeltaTime = Dt, Width = W, Height = H,
        };
        for (int i = 0; i < Iterations; i++)
        {
            pipeline.ExecuteIteration("equiv.iso.corner.gpu", pc, dispatchX: 4, dispatchY: 4)
                    .Should().BeTrue();
        }

        AssertCellWiseEquivalent(cpuField, gpuField, W, H, IsotropicTolerance, "iso corner reflective");
    }

    [Fact]
    public void Isotropic_decay_only_no_diffusion_matches_CPU_within_tolerance()
    {
        // V1-8 edge case: D=0 ⇒ no flow, every cell decays independently
        // P(t+1) = P(t) - K · P(t) · dt. CPU + GPU should evolve identically per-cell.
        const int W = 16, H = 16;
        const float D = 0.0f, Decay = 0.1f, Dt = 1.0f;
        const int Iterations = 5;

        var cpuField = _cpuWorld.Fields.Register<float>("equiv.iso.decay.cpu", W, H);
        // Non-uniform initial state — verifies decay applies к every cell, не just spike.
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                cpuField.WriteCell(x, y, (x + y) * 1.5f);
        var cpuParams = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = D, DecayCoefficient = Decay, DeltaTime = Dt,
        };
        IsotropicDiffusionKernel.Run(cpuField, cpuParams, Iterations);

        var binding = new FieldStorageBinding(_gpuWorld);
        binding.Attach(_instance, _device).Should().BeTrue();
        var gpuField = _gpuWorld.Fields.Register<float>("equiv.iso.decay.gpu", W, H);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                gpuField.WriteCell(x, y, (x + y) * 1.5f);
                gpuField.SetConductivity(x, y, D);
            }

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, "equiv.iso.decay.pipeline", spirv);
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay, DeltaTime = Dt, Width = W, Height = H,
        };
        for (int i = 0; i < Iterations; i++)
        {
            pipeline.ExecuteIteration("equiv.iso.decay.gpu", pc, dispatchX: 2, dispatchY: 2)
                    .Should().BeTrue();
        }

        AssertCellWiseEquivalent(cpuField, gpuField, W, H, IsotropicTolerance, "iso decay-only");
    }

    [Fact]
    public void Isotropic_combined_diffusion_and_decay_matches_CPU_within_tolerance()
    {
        // V1-8 edge case: both terms active simultaneously. Verifies the combined
        // delta = D · laplacian - K · center evolves identically on CPU + GPU.
        const int W = 24, H = 24;
        const float D = 0.15f, Decay = 0.05f, Dt = 1.0f;
        const int Iterations = 5;

        var cpuField = _cpuWorld.Fields.Register<float>("equiv.iso.mix.cpu", W, H);
        cpuField.WriteCell(W / 2, H / 2, 200f);
        var cpuParams = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = D, DecayCoefficient = Decay, DeltaTime = Dt,
        };
        IsotropicDiffusionKernel.Run(cpuField, cpuParams, Iterations);

        var binding = new FieldStorageBinding(_gpuWorld);
        binding.Attach(_instance, _device).Should().BeTrue();
        var gpuField = _gpuWorld.Fields.Register<float>("equiv.iso.mix.gpu", W, H);
        gpuField.WriteCell(W / 2, H / 2, 200f);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                gpuField.SetConductivity(x, y, D);

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, "equiv.iso.mix.pipeline", spirv);
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay, DeltaTime = Dt, Width = W, Height = H,
        };
        for (int i = 0; i < Iterations; i++)
        {
            pipeline.ExecuteIteration("equiv.iso.mix.gpu", pc, dispatchX: 3, dispatchY: 3)
                    .Should().BeTrue();
        }

        AssertCellWiseEquivalent(cpuField, gpuField, W, H, IsotropicTolerance, "iso combined D+K");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void Isotropic_iteration_count_matches_CPU_within_tolerance(int iterations)
    {
        // V1-8 edge case: multi-iteration evolution across {1, 5, 10, 20} iterations.
        // Larger iter counts accumulate floating-point error, but well within tolerance
        // because the stencil is contracting (laplacian-based, не amplifying).
        const int W = 32, H = 32;
        const float D = 0.1f, Decay = 0.0f, Dt = 1.0f;

        var cpuField = _cpuWorld.Fields.Register<float>($"equiv.iso.iter{iterations}.cpu", W, H);
        cpuField.WriteCell(W / 2, H / 2, 100f);
        var cpuParams = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = D, DecayCoefficient = Decay, DeltaTime = Dt,
        };
        IsotropicDiffusionKernel.Run(cpuField, cpuParams, iterations);

        var binding = new FieldStorageBinding(_gpuWorld);
        binding.Attach(_instance, _device).Should().BeTrue();
        var gpuField = _gpuWorld.Fields.Register<float>($"equiv.iso.iter{iterations}.gpu", W, H);
        gpuField.WriteCell(W / 2, H / 2, 100f);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                gpuField.SetConductivity(x, y, D);

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, $"equiv.iso.iter{iterations}.pipeline", spirv);
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay, DeltaTime = Dt, Width = W, Height = H,
        };
        for (int i = 0; i < iterations; i++)
        {
            pipeline.ExecuteIteration($"equiv.iso.iter{iterations}.gpu", pc, dispatchX: 4, dispatchY: 4)
                    .Should().BeTrue();
        }

        AssertCellWiseEquivalent(cpuField, gpuField, W, H, IsotropicTolerance, $"iso iter={iterations}");
    }

    [Fact]
    public void Anisotropic_wire_path_matches_CPU_kernel_within_tolerance()
    {
        const int W = 32, H = 32;
        const float WireD = 1.0f, OffWireD = 0.01f, Decay = 0.0f, Dt = 0.1f;
        const int Iterations = 3;

        // CPU side: AnisotropicDiffusionKernel
        var cpuField = _cpuWorld.Fields.Register<float>("equiv.aniso.cpu", W, H);
        cpuField.WriteCell(0, H / 2, 100f);
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                cpuField.SetConductivity(x, y, y == H / 2 ? WireD : OffWireD);
            }
        }
        var cpuParams = new AnisotropicDiffusionKernel.Parameters
        {
            DecayCoefficient = Decay, DeltaTime = Dt,
        };
        AnisotropicDiffusionKernel.Run(cpuField, cpuParams, Iterations);

        // GPU side: V1DiffusionPipeline с same conductivity map
        var binding = new FieldStorageBinding(_gpuWorld);
        binding.Attach(_instance, _device).Should().BeTrue();
        var gpuField = _gpuWorld.Fields.Register<float>("equiv.aniso.gpu", W, H);
        gpuField.WriteCell(0, H / 2, 100f);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                gpuField.SetConductivity(x, y, y == H / 2 ? WireD : OffWireD);

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, "equiv.aniso.pipeline", spirv);
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay, DeltaTime = Dt, Width = W, Height = H,
        };
        for (int i = 0; i < Iterations; i++)
        {
            pipeline.ExecuteIteration("equiv.aniso.gpu", pc, dispatchX: 4, dispatchY: 4)
                    .Should().BeTrue();
        }

        // Compare cell-by-cell.
        int mismatchCount = 0;
        float maxAbsDiff = 0;
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                float cpu = cpuField.ReadCell(x, y);
                float gpu = gpuField.ReadCell(x, y);
                float diff = MathF.Abs(cpu - gpu);
                if (diff > maxAbsDiff) maxAbsDiff = diff;
                if (diff > AnisotropicTolerance) mismatchCount++;
            }
        }
        mismatchCount.Should().Be(0,
            $"all cells must agree within {AnisotropicTolerance:F4} tolerance. " +
            $"Max abs diff observed: {maxAbsDiff:F6}");
    }

    private static void AssertCellWiseEquivalent(
        FieldHandle<float> cpu, FieldHandle<float> gpu,
        int width, int height, float tolerance, string scenario)
    {
        int mismatchCount = 0;
        float maxAbsDiff = 0;
        (int x, int y, float cpuVal, float gpuVal) firstMismatch = default;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float c = cpu.ReadCell(x, y);
                float g = gpu.ReadCell(x, y);
                float diff = MathF.Abs(c - g);
                if (diff > maxAbsDiff) maxAbsDiff = diff;
                if (diff > tolerance)
                {
                    if (mismatchCount == 0) firstMismatch = (x, y, c, g);
                    mismatchCount++;
                }
            }
        }
        mismatchCount.Should().Be(0,
            $"[{scenario}] all cells must agree within {tolerance:F4} tolerance. " +
            $"Max abs diff: {maxAbsDiff:F6}. First mismatch at ({firstMismatch.x},{firstMismatch.y}): " +
            $"cpu={firstMismatch.cpuVal:F6}, gpu={firstMismatch.gpuVal:F6}");
    }

    private static string FindShaderPath(string name)
    {
        string baseDir = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(baseDir);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
        {
            dir = dir.Parent;
        }
        if (dir == null)
        {
            throw new InvalidOperationException("Could не locate DualFrontier.sln");
        }
        return Path.Combine(dir.FullName, "assets", "shaders", name);
    }
}
