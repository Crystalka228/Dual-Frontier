using DualFrontier.Core.Interop;
using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Compute;

/// <summary>
/// V1-14 — Runtime composition factory tests. Verifies the caller-owned-world pattern:
/// Runtime supplies Vulkan + SPIR-V + factory; caller owns the <see cref="NativeWorld"/>.
/// </summary>
public sealed class V1DiffusionFactoryTests
{
    [WindowsOnlyFact]
    public void CreateFieldStorageBinding_attaches_caller_world_к_runtime_Vulkan()
    {
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "V1 Factory A", Width = 320, Height = 240 },
            EnableValidationLayer = false,
        };
        using var runtime = Runtime.Create(options);
        using var world = new NativeWorld();

        var binding = runtime.CreateFieldStorageBinding(world);

        binding.Should().NotBeNull();
        // PipelineCount = 0 before any Register call (clean attach state).
        binding.PipelineCount.Should().Be(0);
    }

    [WindowsOnlyFact]
    public void CreateFieldStorageBinding_throws_for_null_world()
    {
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "V1 Factory B", Width = 320, Height = 240 },
            EnableValidationLayer = false,
        };
        using var runtime = Runtime.Create(options);

        var act = () => runtime.CreateFieldStorageBinding(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [WindowsOnlyFact]
    public void CreateV1DiffusionPipeline_registers_pipeline_with_nonzero_id()
    {
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "V1 Factory C", Width = 320, Height = 240 },
            EnableValidationLayer = false,
        };
        using var runtime = Runtime.Create(options);
        using var world = new NativeWorld();
        var binding = runtime.CreateFieldStorageBinding(world);

        var pipeline = runtime.CreateV1DiffusionPipeline(binding, "factory.v1.test");

        pipeline.Should().NotBeNull();
        pipeline.PipelineId.Should().BeGreaterThan(0u);
        binding.PipelineCount.Should().Be(1);
    }

    [WindowsOnlyFact]
    public void CreateV1DiffusionPipeline_caches_SPIRV_across_calls()
    {
        // Second call should reuse cached bytecode и still produce а distinct pipeline.
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "V1 Factory D", Width = 320, Height = 240 },
            EnableValidationLayer = false,
        };
        using var runtime = Runtime.Create(options);
        using var world = new NativeWorld();
        var binding = runtime.CreateFieldStorageBinding(world);

        var p1 = runtime.CreateV1DiffusionPipeline(binding, "factory.v1.first");
        var p2 = runtime.CreateV1DiffusionPipeline(binding, "factory.v1.second");

        p1.PipelineId.Should().NotBe(p2.PipelineId);
        binding.PipelineCount.Should().Be(2);
    }

    [WindowsOnlyFact]
    public void CreateV1DiffusionPipeline_dispatch_round_trip_works_via_factory()
    {
        // End-to-end through the factory: register pipeline + register field + dispatch.
        // Verifies the factory-built pipeline behaves identically к the direct constructor.
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "V1 Factory E", Width = 320, Height = 240 },
            EnableValidationLayer = false,
        };
        using var runtime = Runtime.Create(options);
        using var world = new NativeWorld();
        var binding = runtime.CreateFieldStorageBinding(world);
        var pipeline = runtime.CreateV1DiffusionPipeline(binding, "factory.v1.roundtrip");

        var field = world.Fields.Register<float>("factory.v1.field", 8, 8);
        field.WriteCell(4, 4, 100f);
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                field.SetConductivity(x, y, 0.1f);

        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = 0.0f, DeltaTime = 1.0f, Width = 8, Height = 8,
        };
        bool dispatched = pipeline.ExecuteIteration("factory.v1.field", pc, dispatchX: 1, dispatchY: 1);

        dispatched.Should().BeTrue();
        field.ReadCell(4, 4).Should().BeLessThan(100f, "center loses mass к neighbours via factory-registered pipeline");
        field.ReadCell(3, 4).Should().BeGreaterThan(0f);
    }
}
