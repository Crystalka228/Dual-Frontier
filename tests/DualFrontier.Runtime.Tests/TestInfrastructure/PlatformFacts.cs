using System.Runtime.InteropServices;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Xcb;
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

    /// <summary>
    /// True when this host can actually build the Vulkan objects the gated tests build.
    ///
    /// <para>Loading the loader is NOT sufficient evidence. A headless CI image can ship
    /// <c>libvulkan.so.1</c> with no working ICD, no GPU and no render-node access, and
    /// <c>TryLoad</c> would happily succeed — enabling every gated test, which would then fail
    /// inside fixture construction instead of skipping. The gate is documented as "loader plus
    /// GPU", so it probes for exactly that: a real instance and a real device, once per test
    /// process.</para>
    /// </summary>
    internal static bool HasVulkanLoader() => VulkanDeviceProbe.Value;

    private static readonly Lazy<bool> VulkanDeviceProbe = new(() =>
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

        try
        {
            using var instance = new VulkanInstance(enableValidation: false);
            using var device = new VulkanDevice(instance);
            return device.Handle != IntPtr.Zero;
        }
        catch (InvalidOperationException)
        {
            // No usable ICD, no К-L19-tier device, or no render node.
            return false;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
    });

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
        return OperatingSystem.IsWindows() || HasXcbDisplay();
    }

    /// <summary>
    /// True when an xcb connection can actually be opened here.
    ///
    /// <para>Probed by opening one, not inferred from environment variables. <c>WAYLAND_DISPLAY</c>
    /// being set proves a Wayland session and nothing about X: on a pure Wayland box WITHOUT
    /// XWayland there is no <c>DISPLAY</c>, and <c>xcb_connect(null, ...)</c> — which is what
    /// <c>XcbWindow</c> calls — reads <c>DISPLAY</c> and cannot use the Wayland socket. Treating
    /// a Wayland session as a display would enable every windowing test on such a host and fail
    /// them all in window construction instead of skipping them.</para>
    /// </summary>
    internal static bool HasXcbDisplay() => XcbConnectionProbe.Value;

    private static readonly Lazy<bool> XcbConnectionProbe = new(() =>
    {
        if (!OperatingSystem.IsLinux() || !NativeLibrary.TryLoad(LinuxXcb, out _))
        {
            return false;
        }

        IntPtr connection = XcbApi.xcb_connect(null, out _);
        if (connection == IntPtr.Zero)
        {
            return false;
        }
        try
        {
            return XcbApi.xcb_connection_has_error(connection) == 0;
        }
        finally
        {
            // xcb_connect returns a connection object even on failure; it must be released
            // either way.
            XcbApi.xcb_disconnect(connection);
        }
    });

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
        "Requires a usable Vulkan device AND a display server (Windows desktop, or a reachable " +
        "X/XWayland display via libxcb.so.1); skipped on this host.";

    internal const string NoXcbReason =
        "Requires Linux with a reachable X/XWayland display (probed by opening an xcb " +
        "connection); skipped on this host.";
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

/// <summary>
/// A <see cref="FactAttribute"/> skipped unless this host is Linux WITH a reachable X display.
///
/// <para>Distinct from <see cref="RequiresDisplayFactAttribute"/>, which asks the
/// platform-neutral question "can a window be opened here" and is therefore true on Windows.
/// A test that names <c>XcbWindow</c> is not platform-neutral: on a Windows runner it would
/// fail loading <c>libxcb.so.1</c>, and a factory assertion would receive <c>Win32Window</c>.
/// This gate is for suites that exercise the XCB backend specifically.</para>
/// </summary>
public sealed class RequiresXcbFactAttribute : FactAttribute
{
    public RequiresXcbFactAttribute()
    {
        if (!HostCapability.HasVulkanLoader())
        {
            Skip = HostCapability.NoVulkanReason;
        }
        else if (!HostCapability.HasXcbDisplay())
        {
            Skip = HostCapability.NoXcbReason;
        }
    }
}
