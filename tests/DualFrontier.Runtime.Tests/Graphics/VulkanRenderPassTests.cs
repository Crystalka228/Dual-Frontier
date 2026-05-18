using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// V0.B render pass + framebuffer lifecycle on real hardware. Verifies vkCreateRenderPass
/// success with minimal config (1 color attachment, no depth, single subpass) + framebuffer
/// creation per swapchain image.
/// </summary>
public sealed class VulkanRenderPassTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly VulkanSurface _surface;
    private readonly VulkanSwapchain _swapchain;

    public VulkanRenderPassTests()
    {
        var opts = new WindowOptions { Title = "RenderPass", Width = 640, Height = 480 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _surface = new VulkanSurface(_instance, _window);
        _swapchain = new VulkanSwapchain(_device, _surface, 640, 480);
    }

    public void Dispose()
    {
        _swapchain.Dispose();
        _surface.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void RenderPass_construct_returns_non_zero_handle()
    {
        using var renderPass = new VulkanRenderPass(_device, _swapchain.Format);
        renderPass.Handle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Framebuffer_per_swapchain_image_constructs_successfully()
    {
        using var renderPass = new VulkanRenderPass(_device, _swapchain.Format);
        var framebuffers = new List<VulkanFramebuffer>();
        try
        {
            foreach (SwapchainImage img in _swapchain.Images)
            {
                framebuffers.Add(new VulkanFramebuffer(_device, renderPass, img.ImageViewHandle, _swapchain.Width, _swapchain.Height));
            }

            framebuffers.Should().HaveCount(_swapchain.ImageCount);
            foreach (VulkanFramebuffer fb in framebuffers)
            {
                fb.Handle.Should().NotBe(IntPtr.Zero);
                fb.Width.Should().Be(_swapchain.Width);
                fb.Height.Should().Be(_swapchain.Height);
            }
        }
        finally
        {
            foreach (VulkanFramebuffer fb in framebuffers)
            {
                fb.Dispose();
            }
        }
    }
}
