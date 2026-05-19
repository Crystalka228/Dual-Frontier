using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Compute;

/// <summary>
/// V0.B compute pipeline round-trip integration tests на real К-L19 hardware. Verifies
/// noop.comp registration + synchronous dispatch + fence sync.
/// </summary>
public sealed class ComputePipelineRegistrationTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly VulkanCommandPool _computePool;

    public ComputePipelineRegistrationTests()
    {
        var opts = new WindowOptions { Title = "Compute", Width = 320, Height = 240 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        // Pool bound к async compute queue family (V0.B Commit 4 selected this).
        _computePool = new VulkanCommandPool(_device, _device.AsyncComputeQueueFamilyIndex!.Value);
    }

    public void Dispose()
    {
        _computePool.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
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
            throw new InvalidOperationException("Could not locate DualFrontier.sln from test base dir.");
        }
        return Path.Combine(dir.FullName, "assets", "shaders", name);
    }

    [Fact]
    public void Noop_pipeline_register_and_dispatch_round_trip()
    {
        using var registry = new ComputePipelineRegistry(_device);
        using var module = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("noop.comp.spv"));

        VulkanComputePipeline pipeline = registry.Register("noop", module, Array.Empty<ComputeDescriptorBinding>());
        pipeline.Handle.Should().NotBe(IntPtr.Zero);
        registry.Count.Should().Be(1);
        registry.Get("noop").Should().BeSameAs(pipeline);

        using var dispatch = new ComputeDispatch(_device, _computePool);
        dispatch.ExecuteSync(pipeline, x: 1, y: 1, z: 1);
        // No exception = success (fence signaled + queue idle for synchronous dispatch).
    }

    [Fact]
    public void Duplicate_registration_throws()
    {
        using var registry = new ComputePipelineRegistry(_device);
        using var module = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("noop.comp.spv"));
        registry.Register("noop", module, Array.Empty<ComputeDescriptorBinding>());

        Action act = () => registry.Register("noop", module, Array.Empty<ComputeDescriptorBinding>());
        act.Should().Throw<InvalidOperationException>().WithMessage("*already registered*");
    }

    [Fact]
    public void Unknown_pipeline_lookup_returns_null()
    {
        using var registry = new ComputePipelineRegistry(_device);
        registry.Get("does_not_exist").Should().BeNull();
    }
}
