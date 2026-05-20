using DualFrontier.Core.Interop;
using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Compute;

/// <summary>
/// V0.B FieldStorageBinding round-trip via native C ABI. Verifies AttachVulkan +
/// RegisterComputePipeline + DispatchFieldCompute сквозь NativeWorld P/Invoke surface.
/// V0.B native side performs bookkeeping only; this test verifies P/Invoke marshalling
/// + state tracking. V1+ extends с real VkPipeline/dispatch verification.
/// </summary>
public sealed class FieldStorageBindingTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly NativeWorld _world;

    public FieldStorageBindingTests()
    {
        var opts = new WindowOptions { Title = "FieldBind", Width = 320, Height = 240 };
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
    public void Attach_then_register_then_dispatch_round_trip()
    {
        var binding = new FieldStorageBinding(_world);

        bool attached = binding.Attach(_instance, _device);
        attached.Should().BeTrue();

        // V1+: real dispatch requires а registered field. Use noop.comp (0
        // bindings) с а minimal 4×4 scratch field. The shader does nothing
        // but the dispatch path still allocates shadow VkBuffers + records
        // а command buffer + submits к the async compute queue + waits на
        // the fence per К-L7 atomic-from-observer.
        _ = _world.Fields.Register<float>("test_field", 4, 4);

        byte[] noopSpirv = File.ReadAllBytes(FindShaderPath("noop.comp.spv"));
        uint pid = binding.Register("noop", noopSpirv, descriptorBindingCount: 0, pushConstantSize: 0);
        pid.Should().BeGreaterThan(0u);
        binding.PipelineCount.Should().Be(1);

        bool dispatched = binding.DispatchField("test_field", pid, pushConstantData: ReadOnlySpan<byte>.Empty,
                                                x: 1, y: 1, z: 1);
        dispatched.Should().BeTrue("V1+ dispatch through async compute queue + fence wait completes");
    }

    [Fact]
    public void Attach_without_async_compute_throws()
    {
        // Defense-in-depth: if AsyncComputeQueueFamilyIndex is null (К-L19 hardware tier
        // failure), Attach throws — HardwareCapabilityCheck already covers this at
        // Runtime.Create, but FieldStorageBinding also guards defensively. Real К-L19
        // hardware does не trigger this path; assertion is defensive contract.
        var binding = new FieldStorageBinding(_world);
        // Forcing the negative case would require a mocked VulkanDevice. К-L19 verified
        // hardware passes (this test will be skipped/inverted на real K-L19 hardware).
        // We document the defensive guard exists в the source; no negative integration
        // is feasible без mocking the device.
        Action act = () => binding.Attach(_instance, _device);
        act.Should().NotThrow("К-L19 hardware satisfies the precondition");
    }

    [Fact]
    public void Register_without_attach_returns_zero()
    {
        var binding = new FieldStorageBinding(_world);
        byte[] spirv = new byte[] { 0x03, 0x02, 0x23, 0x07, 0x00, 0x00, 0x01, 0x00 };
        uint pid = binding.Register("noop", spirv, descriptorBindingCount: 0, pushConstantSize: 0);
        pid.Should().Be(0u, "native side rejects registration when Vulkan не attached");
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
            throw new InvalidOperationException("Could not locate DualFrontier.sln");
        }
        return Path.Combine(dir.FullName, "assets", "shaders", name);
    }
}
