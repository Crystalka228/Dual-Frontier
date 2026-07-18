using AwesomeAssertions;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// Device-free unit tests for the M9 device-lost route (D1: fail-fast + structured diagnostic, no
/// recovery in v1). They exercise the classifier (<see cref="DeviceLost.ThrowIfLost"/>) and the
/// render-loop boundary (<see cref="DeviceLossBoundary.RunGuarded"/>) with an injected OnDeviceLost
/// recorder -- observing the diagnostic payload and the fail-fast route WITHOUT a real lost GPU and
/// WITHOUT calling <see cref="Environment.FailFast(string)"/> (mirrors the EngineSession OnAbort
/// seam). The production null-hook path (real fail-fast) is exercised by the C7 real-GPU run.
/// </summary>
public sealed class DeviceLostTests
{
    // --- Classifier: DeviceLost.ThrowIfLost ---------------------------------

    // VkResult is internal (visible here via InternalsVisibleTo), but a public xUnit test method
    // cannot expose it in its signature (CS0051) -- so pass the raw int and cast inside.
    [Theory]
    [InlineData((int)VkResult.VK_ERROR_DEVICE_LOST, true)]
    [InlineData((int)VkResult.VK_SUCCESS, false)]
    [InlineData((int)VkResult.VK_ERROR_OUT_OF_DATE_KHR, false)]
    [InlineData((int)VkResult.VK_SUBOPTIMAL_KHR, false)]
    [InlineData((int)VkResult.VK_ERROR_OUT_OF_DEVICE_MEMORY, false)]
    public void ThrowIfLost_ThrowsOnlyForDeviceLost(int rawResult, bool shouldThrow)
    {
        var result = (VkResult)rawResult;
        var act = () => DeviceLost.ThrowIfLost(result, new DeviceLostContext(VulkanCall.QueueSubmit));

        if (shouldThrow)
        {
            act.Should().Throw<DeviceLostException>()
                .Which.Context.Call.Should().Be(VulkanCall.QueueSubmit);
        }
        else
        {
            act.Should().NotThrow("only VK_ERROR_DEVICE_LOST is classified; every other result keeps its site's generic handling");
        }
    }

    [Fact]
    public void ThrowIfLost_CarriesTheThrowSiteContext()
    {
        var context = new DeviceLostContext(VulkanCall.QueuePresent, 1280, 720, 3);

        var act = () => DeviceLost.ThrowIfLost(VkResult.VK_ERROR_DEVICE_LOST, context);

        act.Should().Throw<DeviceLostException>().Which.Context.Should().Be(context);
    }

    // --- Boundary: DeviceLossBoundary.RunGuarded ----------------------------

    [Fact]
    public void RunGuarded_OnDeviceLost_InvokesRecorderWithDiagnostic_WithoutFailFast()
    {
        DeviceLostDiagnostic? recorded = null;
        var boundary = new DeviceLossBoundary(onDeviceLost: d => recorded = d);

        boundary.RunGuarded(
            frameIndex: 42,
            frame: () => throw new DeviceLostException(
                new DeviceLostContext(VulkanCall.QueuePresent, 800, 600, 2)));

        // The process is still alive (the recorder replaced Environment.FailFast), and the
        // diagnostic carries the throw-site context plus the loop-owned frame index.
        recorded.Should().NotBeNull();
        recorded!.Context.Call.Should().Be(VulkanCall.QueuePresent);
        recorded.FrameIndex.Should().Be(42);
        recorded.Describe().Should()
            .Contain("DEVICE LOST").And
            .Contain("frame 42").And
            .Contain("QueuePresent").And
            .Contain("800x600").And
            .Contain("WITHOUT device");
    }

    [Fact]
    public void RunGuarded_HappyPath_RunsFrame_AndNeverRecords()
    {
        var ran = false;
        DeviceLostDiagnostic? recorded = null;
        var boundary = new DeviceLossBoundary(onDeviceLost: d => recorded = d);

        boundary.RunGuarded(frameIndex: 0, frame: () => ran = true);

        ran.Should().BeTrue();
        recorded.Should().BeNull("a clean frame never routes to the device-loss handler");
    }

    [Fact]
    public void RunGuarded_NonDeviceLostException_Propagates_AndDoesNotRecord()
    {
        DeviceLostDiagnostic? recorded = null;
        var boundary = new DeviceLossBoundary(onDeviceLost: d => recorded = d);

        var act = () => boundary.RunGuarded(
            frameIndex: 1,
            frame: () => throw new InvalidOperationException("vkQueueSubmit failed: VK_ERROR_UNKNOWN"));

        act.Should().Throw<InvalidOperationException>("only device-lost is caught at this boundary");
        recorded.Should().BeNull();
    }

    [Fact]
    public void RunGuarded_NullFrame_Throws()
    {
        var boundary = new DeviceLossBoundary();

        var act = () => boundary.RunGuarded(0, null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
