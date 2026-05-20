using DualFrontier.Core.Interop;
using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Compute;

/// <summary>
/// V1-6 integration test exercising V1DiffusionPipeline end-к-end через V1-5c
/// real Vulkan dispatch. Verifies that compute actually modifies the field —
/// distinguishes V1+ real dispatch from V0.B no-op stub.
/// </summary>
public sealed class V1DiffusionIntegrationTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly NativeWorld _world;

    public V1DiffusionIntegrationTests()
    {
        var opts = new WindowOptions { Title = "V1Diffusion", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _world = new NativeWorld();
    }

    public void Dispose()
    {
        _world.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void Single_iteration_spreads_source_spike_к_neighbours()
    {
        var binding = new FieldStorageBinding(_world);
        binding.Attach(_instance, _device).Should().BeTrue();

        // 8×8 field. Source spike at center, uniform conductivity D=0.1.
        var field = _world.Fields.Register<float>("v1.diffusion.test", 8, 8);
        field.WriteCell(4, 4, 100f);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                field.SetConductivity(x, y, 0.1f);
            }
        }

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, "v1.diffusion.test.pipeline", spirv);
        pipeline.PipelineId.Should().BeGreaterThan(0u);

        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = 0.0f,
            DeltaTime = 1.0f,
            Width = 8,
            Height = 8,
        };
        bool dispatched = pipeline.ExecuteIteration("v1.diffusion.test", pc, dispatchX: 1, dispatchY: 1);
        dispatched.Should().BeTrue("V1-5c real dispatch + fence wait completes successfully");

        // After one iteration с D=0.1, the center cell should have lost some value
        // to its 4 neighbours по the 4-neighbour stencil + asymmetric flow rule.
        // Center delta = sum of min(0.1, 0.1) * (P_n - P_center) = 4 * 0.1 * (0 - 100) = -40
        // So center = 100 + (-40 * 1.0) = 60.
        // Each cardinal neighbour delta = min(0.1, 0.1) * (100 - 0) = 10
        // (only one contributing neighbour для each — the center)
        field.ReadCell(4, 4).Should().BeLessThan(100f, "center loses mass к neighbours");
        field.ReadCell(4, 4).Should().BeGreaterThan(0f);
        field.ReadCell(3, 4).Should().BeGreaterThan(0f, "west neighbour received flow");
        field.ReadCell(5, 4).Should().BeGreaterThan(0f, "east neighbour received flow");
        field.ReadCell(4, 3).Should().BeGreaterThan(0f, "north neighbour received flow");
        field.ReadCell(4, 5).Should().BeGreaterThan(0f, "south neighbour received flow");
        field.ReadCell(0, 0).Should().Be(0f, "corner does не receive flow в single iteration");
        field.ReadCell(7, 7).Should().Be(0f);

        // Approximate mass conservation (no decay): sum of all cells should equal
        // 100 within numerical tolerance.
        float total = 0f;
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                total += field.ReadCell(x, y);
        total.Should().BeApproximately(100f, 0.001f, "no decay = mass conserved");
    }

    [Fact]
    public void Dispatch_on_unregistered_field_returns_false()
    {
        var binding = new FieldStorageBinding(_world);
        binding.Attach(_instance, _device).Should().BeTrue();

        byte[] spirv = File.ReadAllBytes(FindShaderPath("diffusion.comp.spv"));
        var pipeline = new V1DiffusionPipeline(binding, "v1.diffusion.unreg.pipeline", spirv);

        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = 0.0f, DeltaTime = 1.0f, Width = 4, Height = 4,
        };
        bool dispatched = pipeline.ExecuteIteration("not.registered.field", pc, 1, 1);
        dispatched.Should().BeFalse("dispatch against unknown field returns failure");
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
