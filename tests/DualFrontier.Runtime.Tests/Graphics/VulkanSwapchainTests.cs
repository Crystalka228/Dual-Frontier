using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// V0.B swapchain lifecycle tests on real К-L19 hardware. Verifies VulkanSurface
/// (vkCreateWin32SurfaceKHR) + VulkanSwapchain (vkCreateSwapchainKHR + image enumeration +
/// per-image VkImageView). Recreation verified by issuing Recreate с new extent.
/// </summary>
public sealed class VulkanSwapchainTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly VulkanSurface _surface;

    public VulkanSwapchainTests()
    {
        var opts = new WindowOptions { Title = "Swapchain", Width = 640, Height = 480 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _surface = new VulkanSurface(_instance, _window);
    }

    public void Dispose()
    {
        _surface.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [WindowsOnlyFact]
    public void Surface_construct_returns_non_zero_handle()
    {
        _surface.Handle.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void Swapchain_construct_enumerates_images_with_views()
    {
        using var swapchain = new VulkanSwapchain(_device, _surface, 640, 480);

        swapchain.Handle.Should().NotBe(IntPtr.Zero);
        swapchain.ImageCount.Should().BeGreaterThan(0);
        foreach (SwapchainImage img in swapchain.Images)
        {
            img.ImageHandle.Should().NotBe(IntPtr.Zero);
            img.ImageViewHandle.Should().NotBe(IntPtr.Zero);
        }
        swapchain.Width.Should().BeGreaterThan(0u);
        swapchain.Height.Should().BeGreaterThan(0u);
    }

    [WindowsOnlyFact]
    public void Swapchain_recreate_succeeds_with_new_extent()
    {
        using var swapchain = new VulkanSwapchain(_device, _surface, 640, 480);
        IntPtr originalHandle = swapchain.Handle;

        // Wait device idle before recreation per Vulkan validation rules.
        DualFrontier.Runtime.Native.Vulkan.VkApi.vkDeviceWaitIdle(_device.Handle);
        swapchain.Recreate(800, 600);

        swapchain.Handle.Should().NotBe(IntPtr.Zero);
        swapchain.Handle.Should().NotBe(originalHandle, "vkCreateSwapchainKHR returns new handle on recreate");
        swapchain.ImageCount.Should().BeGreaterThan(0);
    }

    [WindowsOnlyFact]
    public void Swapchain_dispose_idempotent()
    {
        var swapchain = new VulkanSwapchain(_device, _surface, 640, 480);
        swapchain.Dispose();
        var act = () => swapchain.Dispose();
        act.Should().NotThrow();
    }
}
