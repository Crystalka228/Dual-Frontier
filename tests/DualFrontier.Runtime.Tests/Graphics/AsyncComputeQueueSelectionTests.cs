using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// К-L19 Item 43: async compute queue family selection algorithm tests. Algorithm prefers
/// dedicated compute-only queue family + falls back to compute-capable graphics queue family.
/// Algorithm tested via direct invocation of static helper; integration tested via real
/// VulkanDevice composition (smoke-style: на Crystalka «Skarlet» AMD RX 7600S QF 1 visible).
/// </summary>
public sealed class AsyncComputeQueueSelectionTests
{
    [WindowsOnlyFact]
    public void Dedicated_compute_only_queue_family_preferred_over_graphics_with_compute_bit()
    {
        var qfs = new List<QueueFamilyInfo>
        {
            new(Index: 0, QueueCount: 1, SupportsGraphics: true, SupportsCompute: true, SupportsTransfer: true, SupportsSparseBinding: false),
            new(Index: 1, QueueCount: 1, SupportsGraphics: false, SupportsCompute: true, SupportsTransfer: true, SupportsSparseBinding: false),
        };

        VulkanDevice.FindAsyncComputeQueueFamilyIndex(qfs).Should().Be(1u);
    }

    [WindowsOnlyFact]
    public void Graphics_with_compute_bit_used_when_no_dedicated_compute_exists()
    {
        var qfs = new List<QueueFamilyInfo>
        {
            new(Index: 0, QueueCount: 1, SupportsGraphics: true, SupportsCompute: true, SupportsTransfer: true, SupportsSparseBinding: false),
            new(Index: 1, QueueCount: 1, SupportsGraphics: false, SupportsCompute: false, SupportsTransfer: true, SupportsSparseBinding: false),
        };

        VulkanDevice.FindAsyncComputeQueueFamilyIndex(qfs).Should().Be(0u);
    }

    [WindowsOnlyFact]
    public void Null_returned_when_no_compute_queue_family_exists()
    {
        // К-L19 fail-fast condition: HardwareCapabilityCheck throws if this returns null.
        var qfs = new List<QueueFamilyInfo>
        {
            new(Index: 0, QueueCount: 1, SupportsGraphics: true, SupportsCompute: false, SupportsTransfer: true, SupportsSparseBinding: false),
            new(Index: 1, QueueCount: 1, SupportsGraphics: false, SupportsCompute: false, SupportsTransfer: true, SupportsSparseBinding: false),
        };

        VulkanDevice.FindAsyncComputeQueueFamilyIndex(qfs).Should().BeNull();
    }

    [WindowsOnlyFact]
    public void Empty_queue_family_list_returns_null()
    {
        VulkanDevice.FindAsyncComputeQueueFamilyIndex(Array.Empty<QueueFamilyInfo>()).Should().BeNull();
    }

    [WindowsOnlyFact]
    public void Real_VulkanDevice_exposes_async_compute_queue_on_test_hardware()
    {
        // Integration: К-L19 hardware tier check on Crystalka «Skarlet» (AMD RX 7600S) —
        // expects QF 1 (dedicated compute) selected. Test also passes на other К-L19 hardware
        // (NVIDIA Turing+, Intel Arc Alchemist+) regardless of specific QF index.
        var winOpts = new WindowOptions { Title = "AsyncCompute Integration", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using var window = new global::DualFrontier.Runtime.Window.Window(winOpts, queue);
        using var instance = new VulkanInstance(enableValidation: false);
        using var device = new VulkanDevice(instance);

        device.AsyncComputeQueueFamilyIndex.Should().NotBeNull(
            "К-L19 hardware tier mandates async compute queue family availability.");
        device.AsyncComputeQueue.Should().NotBe(IntPtr.Zero);
    }
}
