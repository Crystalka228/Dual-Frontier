using System.Runtime.InteropServices;
using Xunit;

namespace DualFrontier.Runtime.Tests;

/// <summary>
/// A <see cref="FactAttribute"/> that is skipped on any non-Windows host. The Runtime layer is
/// Win32 + Vulkan, and its tests construct live OS/GPU objects (windows, Vulkan instances/devices,
/// the native world). On Linux/macOS CI those constructors throw <c>DllNotFoundException</c> or
/// platform-invoke failures rather than report a meaningful result, so an ungated <c>[Fact]</c>
/// shows up as an <em>error</em> instead of a skip (F09). Because the <c>Skip</c> reason is set at
/// discovery time, xunit reports these as cleanly skipped and never constructs the test class.
/// </summary>
public sealed class WindowsOnlyFactAttribute : FactAttribute
{
    public WindowsOnlyFactAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = "Requires Windows (Win32 + Vulkan runtime); skipped on non-Windows host.";
        }
    }
}

/// <summary>
/// A <see cref="TheoryAttribute"/> that is skipped on any non-Windows host. See
/// <see cref="WindowsOnlyFactAttribute"/> for the rationale (F09).
/// </summary>
public sealed class WindowsOnlyTheoryAttribute : TheoryAttribute
{
    public WindowsOnlyTheoryAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = "Requires Windows (Win32 + Vulkan runtime); skipped on non-Windows host.";
        }
    }
}


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
/// A <see cref="TheoryAttribute"/> skipped unless a window can be opened on this host. See
/// <see cref="RequiresDisplayFactAttribute"/>.
/// </summary>
public sealed class RequiresDisplayTheoryAttribute : TheoryAttribute
{
    public RequiresDisplayTheoryAttribute()
    {
        if (!HostCapability.HasDisplay())
        {
            Skip = HostCapability.NoDisplayReason;
        }
    }
}
