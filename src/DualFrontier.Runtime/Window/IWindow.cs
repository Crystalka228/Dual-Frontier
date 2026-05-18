namespace DualFrontier.Runtime.Window;

/// <summary>
/// High-level window abstraction. V0.A: lifecycle + message pump. V0.C extends с input event
/// publication. V0.B extends с swapchain surface creation hooks (WM_SIZE → swapchain recreate).
/// </summary>
public interface IWindow : IDisposable
{
    /// <summary>Win32 HWND for surface creation (vkCreateWin32SurfaceKHR — V0.B).</summary>
    IntPtr Handle { get; }

    int Width { get; }
    int Height { get; }

    /// <summary>True after construction; transitions к false on WM_CLOSE / WM_DESTROY / WM_QUIT.</summary>
    bool IsOpen { get; }

    void Show();
    void Hide();

    /// <summary>Drains Win32 message queue (PeekMessage / TranslateMessage / DispatchMessage loop).
    /// Call once per frame from message pump thread.</summary>
    void PumpMessages();
}
