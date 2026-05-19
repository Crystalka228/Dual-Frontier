using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkImage + VkImageView + bound device memory wrapper. V0.B foundation primitive consumed by
/// V0.C texture rendering. V0.B itself creates VkImageView для swapchain images (which are
/// driver-allocated, не VulkanImage-allocated) — see VulkanSwapchain (Commit 7). This wrapper
/// covers the full allocate + view path для future V0.C use.
/// </summary>
public sealed class VulkanImage : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _image;
    private IntPtr _imageView;
    private MemoryAllocation _allocation;
    private bool _disposed;

    public IntPtr Handle => _image;
    public IntPtr ViewHandle => _imageView;
    public uint Width { get; }
    public uint Height { get; }
    internal VkFormat Format { get; }

    public VulkanImage(
        VulkanDevice device,
        MemoryAllocator allocator,
        uint width,
        uint height,
        VkFormatPublic format,
        VkImageUsageFlagsPublic usage,
        VkMemoryPropertyFlagsPublic memoryProperties)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        if (width == 0 || height == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Image dimensions must be > 0.");
        }

        _device = device.Handle;
        Width = width;
        Height = height;
        Format = (VkFormat)(int)format;

        unsafe
        {
            var imageInfo = new VkImageCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                imageType = VkImageType.VK_IMAGE_TYPE_2D,
                format = Format,
                extent = new VkExtent3D { width = width, height = height, depth = 1 },
                mipLevels = 1,
                arrayLayers = 1,
                samples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
                tiling = VkImageTiling.VK_IMAGE_TILING_OPTIMAL,
                usage = (VkImageUsageFlags)(uint)usage,
                sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE,
                queueFamilyIndexCount = 0,
                pQueueFamilyIndices = null,
                initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            };

            VkResult result = VkApi.vkCreateImage(_device, in imageInfo, IntPtr.Zero, out _image);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateImage failed: {result}");
            }
        }

        VkApi.vkGetImageMemoryRequirements(_device, _image, out VkMemoryRequirements memReqs);
        _allocation = allocator.Allocate(memReqs.size, memReqs.alignment, (VkMemoryPropertyFlags)(uint)memoryProperties, memReqs.memoryTypeBits);
        VkResult bindResult = VkApi.vkBindImageMemory(_device, _image, _allocation.DeviceMemory, _allocation.Offset);
        if (bindResult != VkResult.VK_SUCCESS)
        {
            VkApi.vkDestroyImage(_device, _image, IntPtr.Zero);
            _image = IntPtr.Zero;
            throw new InvalidOperationException($"vkBindImageMemory failed: {bindResult}");
        }

        var viewInfo = new VkImageViewCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            image = _image,
            viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
            format = Format,
            components = new VkComponentMapping
            {
                r = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
                g = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
                b = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
                a = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY,
            },
            subresourceRange = new VkImageSubresourceRange
            {
                aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
                baseMipLevel = 0,
                levelCount = 1,
                baseArrayLayer = 0,
                layerCount = 1,
            },
        };

        VkResult viewResult = VkApi.vkCreateImageView(_device, in viewInfo, IntPtr.Zero, out _imageView);
        if (viewResult != VkResult.VK_SUCCESS)
        {
            VkApi.vkDestroyImage(_device, _image, IntPtr.Zero);
            _image = IntPtr.Zero;
            throw new InvalidOperationException($"vkCreateImageView failed: {viewResult}");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_imageView != IntPtr.Zero)
        {
            VkApi.vkDestroyImageView(_device, _imageView, IntPtr.Zero);
            _imageView = IntPtr.Zero;
        }
        if (_image != IntPtr.Zero)
        {
            VkApi.vkDestroyImage(_device, _image, IntPtr.Zero);
            _image = IntPtr.Zero;
        }
        _disposed = true;
    }

    /// <summary>
    /// V0.C.1 convenience: create a device-local 2D image (RGBA8) from a decoded PngImage,
    /// uploading pixel data via the supplied <see cref="TextureUploader"/>. Image is left in
    /// VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL after upload, ready for descriptor binding.
    /// </summary>
    public static VulkanImage CreateFromPngImage(
        VulkanDevice device,
        MemoryAllocator allocator,
        TextureUploader uploader,
        PngImage png)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        ArgumentNullException.ThrowIfNull(uploader);
        ArgumentNullException.ThrowIfNull(png);

        var image = new VulkanImage(
            device, allocator,
            (uint)png.Width, (uint)png.Height,
            VkFormatPublic.R8G8B8A8_UNorm,
            VkImageUsageFlagsPublic.Sampled | VkImageUsageFlagsPublic.TransferDst,
            VkMemoryPropertyFlagsPublic.DeviceLocal);

        try
        {
            uploader.Upload(image, png.PixelsRgba8);
            return image;
        }
        catch
        {
            image.Dispose();
            throw;
        }
    }
}

/// <summary>Public-facing mirror of VkFormat. V0.B covers swapchain + texture formats only.</summary>
public enum VkFormatPublic : int
{
    Undefined = 0,
    R8G8B8A8_UNorm = 37,
    R8G8B8A8_SRgb = 43,
    B8G8R8A8_UNorm = 44,
    B8G8R8A8_SRgb = 50,
    R32_SFloat = 100,
    R32G32_SFloat = 103,
    R32G32B32A32_SFloat = 109,
}

/// <summary>Public-facing mirror of VkImageUsageFlags. Bit-for-bit cast.</summary>
[Flags]
public enum VkImageUsageFlagsPublic : uint
{
    TransferSrc = 0x00000001,
    TransferDst = 0x00000002,
    Sampled = 0x00000004,
    Storage = 0x00000008,
    ColorAttachment = 0x00000010,
    DepthStencilAttachment = 0x00000020,
    TransientAttachment = 0x00000040,
    InputAttachment = 0x00000080,
}
