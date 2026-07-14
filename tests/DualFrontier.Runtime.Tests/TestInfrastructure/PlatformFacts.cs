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
