namespace DualFrontier.Runtime.Graphics;

/// <summary>The Vulkan call at which a device loss surfaced (the M9 diagnostic site enum).</summary>
public enum VulkanCall
{
    QueueSubmit,
    QueuePresent,
    WaitForFences,
    AcquireNextImage,
    DeviceWaitIdle,
}

/// <summary>
/// The throw-site snapshot a wrapper holds when it detects VK_ERROR_DEVICE_LOST. Carries only what
/// the wrapper knows locally -- the failing <see cref="VulkanCall"/> and, for the swapchain sites,
/// the current swapchain extent + image count. The render-loop frame index is NOT known here; it is
/// added at the boundary (<see cref="DeviceLossBoundary"/>). VkResult is internal, so the public
/// surface carries this site enum, never the raw result.
/// </summary>
public readonly record struct DeviceLostContext(
    VulkanCall Call,
    uint SwapchainWidth,
    uint SwapchainHeight,
    int SwapchainImageCount)
{
    /// <summary>Site context with no swapchain state (submit / fence / device-wait-idle sites).</summary>
    public DeviceLostContext(VulkanCall call) : this(call, 0, 0, 0)
    {
    }

    /// <summary>Site context enriched with the swapchain's current state (acquire / present sites).</summary>
    public static DeviceLostContext ForSwapchain(VulkanCall call, VulkanSwapchain swapchain)
    {
        ArgumentNullException.ThrowIfNull(swapchain);
        return new DeviceLostContext(call, swapchain.Width, swapchain.Height, swapchain.ImageCount);
    }
}

/// <summary>
/// Typed carrier for a detected Vulkan device loss (VK_ERROR_DEVICE_LOST). Thrown at the detection
/// site by <see cref="DeviceLost.ThrowIfLost"/> and carried out to the render-loop boundary. Mirrors
/// the render-stack exception idiom (<see cref="HardwareCapabilityException"/>): the public surface
/// carries the site <see cref="DeviceLostContext"/>, never the internal VkResult.
/// </summary>
public sealed class DeviceLostException : Exception
{
    public DeviceLostException(DeviceLostContext context)
        : base($"Vulkan device lost (VK_ERROR_DEVICE_LOST) at {context.Call}.")
    {
        Context = context;
    }

    /// <summary>The throw-site snapshot (failing call + swapchain state where known).</summary>
    public DeviceLostContext Context { get; }
}

/// <summary>
/// The structured device-lost diagnostic, composed at the render-loop boundary from the throw-site
/// <see cref="DeviceLostContext"/> plus the loop-owned frame index. <see cref="Describe"/> renders the
/// message the fail-fast carries (D1: fail-fast + diagnostic, NO recovery in v1). Mirrors the
/// EngineSession ShutdownAbortReport.Describe() precedent.
/// </summary>
public sealed record DeviceLostDiagnostic(DeviceLostContext Context, long FrameIndex)
{
    public string Describe()
    {
        string swapchain = Context.SwapchainImageCount > 0
            ? $"Swapchain: {Context.SwapchainWidth}x{Context.SwapchainHeight}, {Context.SwapchainImageCount} image(s). "
            : "";
        return
            $"Vulkan DEVICE LOST (VK_ERROR_DEVICE_LOST) at {Context.Call} on render frame {FrameIndex}. "
            + swapchain
            + "Device loss is unrecoverable in v1 (D1 / ELT OQ-3): failing fast WITHOUT device "
            + "re-creation (ELT §4 device-lost class; VULKAN_SUBSTRATE §6.3; IAC §7). Device "
            + "re-creation is future work with its own design.";
    }
}
