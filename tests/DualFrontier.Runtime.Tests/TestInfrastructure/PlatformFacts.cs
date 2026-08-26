using System.Runtime.InteropServices;
using DualFrontier.Runtime.Graphics;
using Xunit;

namespace DualFrontier.Runtime.Tests;

// ===========================================================================
// Capability gates. These ask what the HOST can do rather than which OS it is,
// which is the distinction that matters: a Vulkan-only test needs the loader and
// a GPU, while a windowing test additionally needs a display server. Both skip at
// DISCOVERY time (constructor-set Skip), so xunit reports a clean skip and never
// constructs the class -- the same F09 rationale the OS gate carried.
// ===========================================================================

/// <summary>Probes for the capabilities the capability gates below are named after.</summary>
internal static class HostCapability
{
    private const string WindowsVulkanLoader = "vulkan-1.dll";
    private const string LinuxVulkanLoader = "libvulkan.so.1";
    private const string LinuxXcb = "libxcb.so.1";

    /// <summary>True when the platform's Vulkan loader can be loaded in this process.</summary>
    internal static bool HasVulkanLoader()
    {
        string loader = OperatingSystem.IsWindows() ? WindowsVulkanLoader : LinuxVulkanLoader;
        if (!NativeLibrary.TryLoad(loader, out IntPtr handle))
        {
            return false;
        }
        // Deliberately NOT freed: the loader stays resident for the tests that follow, and
        // unloading it out from under a live VkInstance would be worse than leaking a handle
        // for the lifetime of a test run.
        _ = handle;
        return true;
    }

    /// <summary>
    /// True when a window can actually be opened here: the Vulkan loader plus a reachable
    /// display server. On Windows the desktop is always present. On Linux it takes a session
    /// (DISPLAY for X11/XWayland, or WAYLAND_DISPLAY) and the xcb client library.
    /// </summary>
    internal static bool HasDisplay()
    {
        if (!HasVulkanLoader())
        {
            return false;
        }
        if (OperatingSystem.IsWindows())
        {
            return true;
        }
        bool hasSession = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY"))
            || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY"));
        return hasSession && NativeLibrary.TryLoad(LinuxXcb, out _);
    }

    internal const string NoVulkanReason =
        "Requires a working platform Vulkan loader (vulkan-1.dll / libvulkan.so.1); skipped on this host.";

    /// <summary>
    /// Whether a VkInstance can be created WITH the Khronos validation layer. Probed once per
    /// test process by attempting exactly that, because it is the operational question and no
    /// cheaper proxy is truthful: the layer is a separately-installed manifest, so its presence
    /// is independent of both the loader and the display, and a filesystem scan would miss
    /// VK_LAYER_PATH installations.
    /// </summary>
    private static readonly Lazy<bool> ValidationLayerProbe = new(() =>
    {
        if (!HasVulkanLoader())
        {
            return false;
        }
        try
        {
            using var instance = new VulkanInstance(enableValidation: true);
            return true;
        }
        catch (InvalidOperationException)
        {
            // VK_ERROR_LAYER_NOT_PRESENT — the layer is not installed on this host.
            return false;
        }
    });

    internal static bool HasVulkanValidationLayer() => ValidationLayerProbe.Value;

    internal const string NoValidationLayerReason =
        "Requires the VK_LAYER_KHRONOS_validation layer (Vulkan SDK / vulkan-validationlayers); " +
        "skipped on this host.";

    internal const string NoDisplayReason =
        "Requires a Vulkan loader AND a display server (Windows desktop, or DISPLAY/WAYLAND_DISPLAY " +
        "with libxcb.so.1); skipped on this host.";
}

/// <summary>
/// A <see cref="FactAttribute"/> skipped unless the platform Vulkan loader is present. For tests
/// that build a VkInstance/VkDevice and never need a window or a surface.
/// </summary>
public sealed class RequiresVulkanFactAttribute : FactAttribute
{
    public RequiresVulkanFactAttribute()
    {
        if (!HostCapability.HasVulkanLoader())
        {
            Skip = HostCapability.NoVulkanReason;
        }
    }
}

/// <summary>
/// A <see cref="TheoryAttribute"/> skipped unless the platform Vulkan loader is present. See
/// <see cref="RequiresVulkanFactAttribute"/>.
/// </summary>
public sealed class RequiresVulkanTheoryAttribute : TheoryAttribute
{
    public RequiresVulkanTheoryAttribute()
    {
        if (!HostCapability.HasVulkanLoader())
        {
            Skip = HostCapability.NoVulkanReason;
        }
    }
}

/// <summary>
/// A <see cref="FactAttribute"/> skipped unless a window can be opened on this host. For tests
/// that construct a real window, a VkSurfaceKHR, or a full <c>Runtime</c>.
/// </summary>
public sealed class RequiresDisplayFactAttribute : FactAttribute
{
    public RequiresDisplayFactAttribute()
    {
        if (!HostCapability.HasDisplay())
        {
            Skip = HostCapability.NoDisplayReason;
        }
    }
}

/// <summary>
/// A <see cref="FactAttribute"/> skipped unless a window can be opened AND the Khronos
/// validation layer is installed.
///
/// <para>A third capability, not a variant of the other two. The validation layer is an
/// optional, separately-packaged component: a host can have a perfectly good Vulkan loader, a
/// GPU and a display and still not have it — which is the case on the reference Linux box,
/// where only the Mesa and Intel layers ship. Tagging a validation test with the display gate
/// would make it fail on such a host for a reason that has nothing to do with what it tests.</para>
/// </summary>
public sealed class RequiresVulkanValidationFactAttribute : FactAttribute
{
    public RequiresVulkanValidationFactAttribute()
    {
        if (!HostCapability.HasDisplay())
        {
            Skip = HostCapability.NoDisplayReason;
        }
        else if (!HostCapability.HasVulkanValidationLayer())
        {
            Skip = HostCapability.NoValidationLayerReason;
        }
    }
}
