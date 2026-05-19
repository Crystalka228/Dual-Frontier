using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Native.Win32;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkSurfaceKHR (Win32) lifecycle. Wraps vkCreateWin32SurfaceKHR + vkDestroySurfaceKHR.
/// Disposed releases the surface back к Vulkan instance.
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

        IntPtr hinstance = Win32Api.GetModuleHandle(null);
        if (hinstance == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"GetModuleHandle (Win32 surface foundation) failed: error {Win32Api.GetLastError()}");
        }

        var createInfo = new VkWin32SurfaceCreateInfoKHR
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            pNext = IntPtr.Zero,
            flags = 0,
            hinstance = hinstance,
            hwnd = window.Handle,
        };

        VkResult result = VkApi.vkCreateWin32SurfaceKHR(_instance, in createInfo, IntPtr.Zero, out _surface);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException(
                $"vkCreateWin32SurfaceKHR failed: {result}. Verify VK_KHR_win32_surface instance " +
                "extension was activated by VulkanInstance (V0.A baseline includes it).");
        }
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
