using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkSurfaceKHR lifetime, platform-neutral. Creation is delegated to the window — only the
/// window knows which window-system extension and which native handles its surface needs —
/// and this type owns nothing but the resulting handle and its destruction. Disposed releases
/// the surface back к the Vulkan instance.
/// </summary>
public sealed class VulkanSurface : IDisposable
{
    private readonly IntPtr _instance;
    private IntPtr _surface;
    private bool _disposed;

    public IntPtr Handle => _surface;

    public VulkanSurface(VulkanInstance instance, IWindow window)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(window);
        _instance = instance.Handle;
        _surface = window.CreateVulkanSurface(_instance);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_surface != IntPtr.Zero)
        {
            VkApi.vkDestroySurfaceKHR(_instance, _surface, IntPtr.Zero);
            _surface = IntPtr.Zero;
        }
        _disposed = true;
    }
}
