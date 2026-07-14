using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// К-L19 Item 44: startup fail-fast tests. Verifies both sub-checks throw appropriate
/// exceptions on hardware-tier rejection. Integration test verifies positive path
/// (real К-L19 hardware passes).
/// </summary>
public sealed class HardwareCapabilityCheckTests
{
    [WindowsOnlyFact]
    public void VerifyVulkanApiVersion_below_1_3_throws()
    {
        Action act = () => HardwareCapabilityCheck.VerifyVulkanApiVersion(VkConstants.VK_API_VERSION_1_0);
        act.Should().Throw<HardwareCapabilityException>()
            .WithMessage("*Vulkan 1.3*");
    }

    [WindowsOnlyFact]
    public void VerifyVulkanApiVersion_at_1_3_does_not_throw()
    {
        Action act = () => HardwareCapabilityCheck.VerifyVulkanApiVersion(VkConstants.VK_API_VERSION_1_3);
        act.Should().NotThrow();
    }

    [WindowsOnlyFact]
    public void VerifyAsyncComputeQueueFamily_null_throws_with_hardware_tier_message()
    {
        Action act = () => HardwareCapabilityCheck.VerifyAsyncComputeQueueFamily(null, "Test GPU");
        act.Should().Throw<HardwareCapabilityException>()
            .Where(ex => ex.Message.Contains("Test GPU") && ex.Message.Contains("async compute"));
    }

    [WindowsOnlyFact]
    public void VerifyAsyncComputeQueueFamily_with_value_does_not_throw()
    {
        Action act = () => HardwareCapabilityCheck.VerifyAsyncComputeQueueFamily(1u, "Test GPU");
        act.Should().NotThrow();
    }

    [WindowsOnlyFact]
    public void Verify_real_hardware_does_not_throw_on_test_machine()
    {
        // К-L19 baseline integration: Crystalka «Skarlet» AMD RX 7600S satisfies hardware tier.
        var winOpts = new WindowOptions { Title = "HW Check", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using var window = new global::DualFrontier.Runtime.Window.Window(winOpts, queue);
        using var instance = new VulkanInstance(enableValidation: false);
        using var device = new VulkanDevice(instance);

        Action act = () => HardwareCapabilityCheck.Verify(instance, device);
        act.Should().NotThrow();
    }
}
