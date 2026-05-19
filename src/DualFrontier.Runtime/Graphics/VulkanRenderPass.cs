using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Minimal V0.B render pass: one color attachment, no depth/stencil, single subpass.
/// loadOp = CLEAR (per V0.B exit criterion = clear color rendered), storeOp = STORE,
/// initialLayout UNDEFINED → finalLayout PRESENT_SRC_KHR (swapchain-compatible).
/// Subpass dependency: external → 0 at COLOR_ATTACHMENT_OUTPUT stage.
/// </summary>
public sealed class VulkanRenderPass : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _renderPass;
    private bool _disposed;

    public IntPtr Handle => _renderPass;
    internal VkFormat Format { get; }

    public VulkanRenderPass(VulkanDevice device, VkFormatPublic colorAttachmentFormat)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;
        Format = (VkFormat)(int)colorAttachmentFormat;

        unsafe
        {
            var colorAttachment = new VkAttachmentDescription
            {
                flags = 0,
                format = Format,
                samples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
                loadOp = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
                storeOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE,
                stencilLoadOp = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
                stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
                initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
                finalLayout = VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR,
            };

            var colorRef = new VkAttachmentReference
            {
                attachment = 0,
                layout = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
            };

            var subpass = new VkSubpassDescription
            {
                flags = 0,
                pipelineBindPoint = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
                inputAttachmentCount = 0,
                pInputAttachments = null,
                colorAttachmentCount = 1,
                pColorAttachments = &colorRef,
                pResolveAttachments = null,
                pDepthStencilAttachment = null,
                preserveAttachmentCount = 0,
                pPreserveAttachments = null,
            };

            const uint VK_SUBPASS_EXTERNAL = unchecked((uint)~0);
            var dependency = new VkSubpassDependency
            {
                srcSubpass = VK_SUBPASS_EXTERNAL,
                dstSubpass = 0,
                srcStageMask = VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
                dstStageMask = VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
                srcAccessMask = 0,
                dstAccessMask = VkAccessFlags.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT,
                dependencyFlags = 0,
            };

            var createInfo = new VkRenderPassCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                attachmentCount = 1,
                pAttachments = &colorAttachment,
                subpassCount = 1,
                pSubpasses = &subpass,
                dependencyCount = 1,
                pDependencies = &dependency,
            };

            VkResult result = VkApi.vkCreateRenderPass(_device, in createInfo, IntPtr.Zero, out _renderPass);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateRenderPass failed: {result}");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_renderPass != IntPtr.Zero)
        {
            VkApi.vkDestroyRenderPass(_device, _renderPass, IntPtr.Zero);
            _renderPass = IntPtr.Zero;
        }
        _disposed = true;
    }
}
