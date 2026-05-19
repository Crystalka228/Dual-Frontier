using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// VulkanPipelineLayout extension tests. Verifies backward-compat empty layout (V0.B regression)
/// + new push constant range support (V0.C.1 S-LOCK-8).
/// </summary>
public sealed class VulkanPipelineLayoutTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;

    public VulkanPipelineLayoutTests()
    {
        var opts = new WindowOptions { Title = "PipelineLayout", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
    }

    public void Dispose()
    {
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void Empty_layout_creates_successfully()
    {
        // V0.B regression: empty layout (no descriptor sets, no push constants) must still work.
        using var layout = new VulkanPipelineLayout(_device);
        layout.Handle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Push_constant_range_creates_successfully()
    {
        // S-LOCK-8: push constant range for Camera MVP (vertex stage, 64 bytes = mat4).
        var range = new VkPushConstantRangePublic(
            StageFlags: VkShaderStageFlagsPublic.Vertex,
            Offset: 0,
            Size: 64);
        using var layout = new VulkanPipelineLayout(
            _device,
            descriptorSetLayouts: null,
            pushConstantRanges: new[] { range });
        layout.Handle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Multiple_push_constant_ranges_create_successfully()
    {
        var ranges = new[]
        {
            new VkPushConstantRangePublic(VkShaderStageFlagsPublic.Vertex, 0, 64),
            new VkPushConstantRangePublic(VkShaderStageFlagsPublic.Fragment, 64, 16),
        };
        using var layout = new VulkanPipelineLayout(_device, pushConstantRanges: ranges);
        layout.Handle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Dispose_idempotent()
    {
        var layout = new VulkanPipelineLayout(_device);
        layout.Dispose();
        layout.Dispose();
    }
}
