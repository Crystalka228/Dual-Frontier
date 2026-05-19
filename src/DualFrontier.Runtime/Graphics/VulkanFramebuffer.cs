using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkFramebuffer wrapping one VkImageView (color attachment). One framebuffer per swapchain
/// image, recreated alongside swapchain on resize.
/// </summary>
public sealed class VulkanFramebuffer : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _framebuffer;
    private bool _disposed;

    public IntPtr Handle => _framebuffer;
    public uint Width { get; }
    public uint Height { get; }

    public VulkanFramebuffer(VulkanDevice device, VulkanRenderPass renderPass, IntPtr imageView, uint width, uint height)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(renderPass);
        if (imageView == IntPtr.Zero)
        {
            throw new ArgumentException("imageView handle must be non-zero", nameof(imageView));
        }
        _device = device.Handle;
        Width = width;
        Height = height;

        unsafe
        {
            IntPtr view = imageView;
            var createInfo = new VkFramebufferCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                renderPass = renderPass.Handle,
                attachmentCount = 1,
                pAttachments = &view,
                width = width,
                height = height,
                layers = 1,
            };
            VkResult result = VkApi.vkCreateFramebuffer(_device, in createInfo, IntPtr.Zero, out _framebuffer);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateFramebuffer failed: {result}");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_framebuffer != IntPtr.Zero)
        {
            VkApi.vkDestroyFramebuffer(_device, _framebuffer, IntPtr.Zero);
            _framebuffer = IntPtr.Zero;
        }
        _disposed = true;
    }
}
