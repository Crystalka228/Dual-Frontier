using System.Runtime.InteropServices;
using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Synchronous texture upload pipeline: host-visible staging buffer → device-local VulkanImage
/// transfer via <c>vkCmdCopyBufferToImage</c> + image layout transitions
/// (UNDEFINED → TRANSFER_DST_OPTIMAL → SHADER_READ_ONLY_OPTIMAL).
///
/// Synchronous fence wait per К-L7 atomic-from-observer (V0.B precedent). Future V0.C.2/V1
/// may add async upload pattern for large asset streams where staging buffer pool amortizes cost.
///
/// Per S-LOCK-5: PngDecoder outputs straight-alpha RGBA8; uploader does NOT convert к
/// premultiplied alpha. Sprite shader handles this via discard threshold + blend mode.
/// </summary>
public sealed class TextureUploader : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly MemoryAllocator _allocator;
    private readonly VulkanCommandPool _commandPool;
    private bool _disposed;

    public TextureUploader(VulkanDevice device, MemoryAllocator allocator, VulkanCommandPool commandPool)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        ArgumentNullException.ThrowIfNull(commandPool);
        _device = device;
        _allocator = allocator;
        _commandPool = commandPool;
    }

    /// <summary>
    /// Upload RGBA8 pixel data к VulkanImage. Creates staging buffer + records transfer
    /// commands + submits + waits fence. Image transitions UNDEFINED → TRANSFER_DST_OPTIMAL
    /// → SHADER_READ_ONLY_OPTIMAL.
    /// </summary>
    public unsafe void Upload(VulkanImage destImage, byte[] rgba8Pixels)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(destImage);
        ArgumentNullException.ThrowIfNull(rgba8Pixels);

        ulong size = (ulong)rgba8Pixels.Length;
        if (size == 0)
        {
            throw new ArgumentException("Pixel buffer is empty.", nameof(rgba8Pixels));
        }

        // 1. Create host-visible staging buffer.
        using var staging = new VulkanBuffer(
            _device, _allocator, size,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent,
            VkBufferUsageFlagsPublic.TransferSrc);

        // 2. Map staging buffer + copy pixels.
        VkResult mapResult = VkApi.vkMapMemory(
            _device.Handle, staging.Allocation.DeviceMemory, staging.Allocation.Offset, size, 0, out IntPtr mappedPtr);
        if (mapResult != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkMapMemory failed: {mapResult}");
        }
        try
        {
            Marshal.Copy(rgba8Pixels, 0, mappedPtr, rgba8Pixels.Length);
        }
        finally
        {
            VkApi.vkUnmapMemory(_device.Handle, staging.Allocation.DeviceMemory);
        }

        // 3. Allocate one-shot command buffer + record commands.
        using var commandBuffer = _commandPool.AllocateBuffer();
        commandBuffer.Begin();

        // 3a. Pipeline barrier: UNDEFINED → TRANSFER_DST_OPTIMAL.
        RecordImageLayoutTransition(commandBuffer.Handle, destImage.Handle,
            VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            srcAccess: 0,
            dstAccess: VkAccessFlags.VK_ACCESS_TRANSFER_WRITE_BIT,
            srcStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            dstStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_TRANSFER_BIT);

        // 3b. vkCmdCopyBufferToImage.
        var region = new VkBufferImageCopy
        {
            bufferOffset = 0,
            bufferRowLength = 0,        // tightly packed
            bufferImageHeight = 0,
            imageSubresource = new VkImageSubresourceLayers
            {
                aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
                mipLevel = 0,
                baseArrayLayer = 0,
                layerCount = 1,
            },
            imageOffset = new VkOffset3D { x = 0, y = 0, z = 0 },
            imageExtent = new VkExtent3D { width = destImage.Width, height = destImage.Height, depth = 1 },
        };
        VkApi.vkCmdCopyBufferToImage(
            commandBuffer.Handle, staging.Handle, destImage.Handle,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            1, in region);

        // 3c. Pipeline barrier: TRANSFER_DST_OPTIMAL → SHADER_READ_ONLY_OPTIMAL.
        RecordImageLayoutTransition(commandBuffer.Handle, destImage.Handle,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
            srcAccess: VkAccessFlags.VK_ACCESS_TRANSFER_WRITE_BIT,
            dstAccess: VkAccessFlags.VK_ACCESS_SHADER_READ_BIT,
            srcStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_TRANSFER_BIT,
            dstStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT);

        commandBuffer.End();

        // 4. Submit + wait fence (К-L7 atomic-from-observer).
        using var fence = new VulkanFence(_device);
        commandBuffer.SubmitTo(_device.GraphicsQueue, fence: fence);
        fence.Wait();
    }

    private static unsafe void RecordImageLayoutTransition(
        IntPtr commandBuffer, IntPtr image,
        VkImageLayout oldLayout, VkImageLayout newLayout,
        VkAccessFlags srcAccess, VkAccessFlags dstAccess,
        VkPipelineStageFlags srcStage, VkPipelineStageFlags dstStage)
    {
        var barrier = new VkImageMemoryBarrier
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER,
            pNext = IntPtr.Zero,
            srcAccessMask = srcAccess,
            dstAccessMask = dstAccess,
            oldLayout = oldLayout,
            newLayout = newLayout,
            srcQueueFamilyIndex = uint.MaxValue,    // VK_QUEUE_FAMILY_IGNORED
            dstQueueFamilyIndex = uint.MaxValue,
            image = image,
            subresourceRange = new VkImageSubresourceRange
            {
                aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
                baseMipLevel = 0,
                levelCount = 1,
                baseArrayLayer = 0,
                layerCount = 1,
            },
        };

        VkApi.vkCmdPipelineBarrier(
            commandBuffer,
            srcStageMask: srcStage,
            dstStageMask: dstStage,
            dependencyFlags: 0,
            memoryBarrierCount: 0, pMemoryBarriers: IntPtr.Zero,
            bufferMemoryBarrierCount: 0, pBufferMemoryBarriers: IntPtr.Zero,
            imageMemoryBarrierCount: 1, pImageMemoryBarriers: &barrier);
    }

    public void Dispose()
    {
        // Uploader does not own persistent resources (one-shot upload pattern; staging buffers + fences disposed inline).
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(TextureUploader));
        }
    }
}
