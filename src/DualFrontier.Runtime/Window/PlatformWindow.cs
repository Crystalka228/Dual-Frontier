using DualFrontier.Runtime.Input;

namespace DualFrontier.Runtime.Window;

/// <summary>
/// Selects the windowing backend for the running host. The composition root calls this instead
/// of naming a concrete window type, so adding a backend is a change here and nowhere else.
/// </summary>
public static class PlatformWindow
{
    /// <summary>
    /// Creates the window implementation for this platform.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">No backend exists for the running host.</exception>
    public static IWindow Create(WindowOptions options, InputEventQueue inputQueue)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(inputQueue);

        if (OperatingSystem.IsWindows())
        {
            return new Win32Window(options, inputQueue);
        }
        if (OperatingSystem.IsLinux())
        {
            // XCB, which on a Wayland session runs through XWayland. A Wayland-native backend
            // is future work; XCB reaches both session types with one connection object.
            return new XcbWindow(options, inputQueue);
        }

        throw new PlatformNotSupportedException(
            $"No windowing backend for this platform ({Environment.OSVersion.Platform}). " +
            "Supported: Windows (Win32), Linux (XCB).");
    }
}
