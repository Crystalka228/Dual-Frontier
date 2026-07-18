using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkSwapchainKHR lifecycle. Owns: format/present mode/extent selection, image acquisition,
/// per-image VkImageView, present invocation. Recreation supported via <see cref="Recreate"/>
/// (caller invokes after <c>WindowResizeEvent</c>).
/// </summary>
public sealed class VulkanSwapchain : IDisposable
{
    private readonly IntPtr _device;
    private readonly IntPtr _physicalDevice;
    private readonly IntPtr _surface;

    private IntPtr _swapchain;
    private SwapchainImage[] _images = Array.Empty<SwapchainImage>();
    private bool _disposed;

    public IntPtr Handle => _swapchain;
    public IReadOnlyList<SwapchainImage> Images => _images;
    public int ImageCount => _images.Length;
    public VkFormatPublic Format { get; private set; }
    public uint Width { get; private set; }
    public uint Height { get; private set; }
    public VkPresentModePublic PresentMode { get; private set; }

    public VulkanSwapchain(VulkanDevice device, VulkanSurface surface, uint width, uint height)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(surface);
        _device = device.Handle;
        _physicalDevice = device.PhysicalDevice;
        _surface = surface.Handle;
        // First creation: prepare then commit; nothing to reclaim (no prior swapchain).
        Commit(PrepareSwapchain(width, height, oldSwapchain: IntPtr.Zero));
    }

    /// <summary>
    /// Recreate swapchain с new extent as a prepare-before-reclaim transaction
    /// (ELT §2.5 swapchain recreation): PREPARE the new swapchain + image views alongside the
    /// old, COMMIT them in one infallible assignment, then RECLAIM the old set. A mid-prepare
    /// failure leaves the old swapchain + views intact with no field mutated (ELT §1.1
    /// corollary 1: reclaim MUST NOT begin before commit succeeds). Preserves the underlying
    /// VkSurfaceKHR. Caller must ensure GPU idle (vkDeviceWaitIdle) before invoking.
    /// </summary>
    public void Recreate(uint width, uint height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        IntPtr oldSwapchain = _swapchain;
        SwapchainImage[] oldImages = _images;

        // PREPARE -- the old swapchain is still alive here, so it feeds
        // VkSwapchainCreateInfoKHR.oldSwapchain (:oldSwapchain hint). A failure throws BEFORE
        // commit, so the old resources below are never reached and stay intact.
        SwapchainResources prepared = PrepareSwapchain(width, height, oldSwapchain);

        // COMMIT -- straight-line, no vk calls: publish the new resources atomically.
        Commit(prepared);

        // RECLAIM -- only after a successful commit (ELT §2.5, best-effort teardown of old set).
        DestroyImageViews(oldImages);
        if (oldSwapchain != IntPtr.Zero)
        {
            VkApi.vkDestroySwapchainKHR(_device, oldSwapchain, IntPtr.Zero);
        }
    }

    /// <summary>Publishes prepared swapchain resources onto the instance (the ELT §2.5 commit step; no vk calls, cannot fail).</summary>
    private void Commit(SwapchainResources prepared)
    {
        _swapchain = prepared.Swapchain;
        _images = prepared.Images;
        Format = prepared.Format;
        Width = prepared.Width;
        Height = prepared.Height;
        PresentMode = prepared.PresentMode;
    }

    /// <summary>
    /// Acquire next swapchain image. Returns image index. Caller submits commands к graphics queue
    /// using this image, then calls <see cref="Present"/>.
    /// </summary>
    public uint AcquireNextImage(IntPtr signalSemaphore, IntPtr signalFence, out bool outOfDate)
    {
        VkResult result = VkApi.vkAcquireNextImageKHR(
            _device, _swapchain, ulong.MaxValue, signalSemaphore, signalFence, out uint imageIndex);
        outOfDate = result == VkResult.VK_ERROR_OUT_OF_DATE_KHR;
        if (result != VkResult.VK_SUCCESS && result != VkResult.VK_SUBOPTIMAL_KHR && !outOfDate)
        {
            throw new InvalidOperationException($"vkAcquireNextImageKHR failed: {result}");
        }
        return imageIndex;
    }

    /// <summary>
    /// Submit present command к graphics queue. Caller passes waiting semaphore(s) signaled by
    /// command buffer submission. Returns <see langword="true"/> if swapchain became out-of-date
    /// и caller should recreate.
    /// </summary>
    public unsafe bool Present(IntPtr queue, IntPtr waitSemaphore, uint imageIndex)
    {
        IntPtr swapchain = _swapchain;
        uint imgIdx = imageIndex;
        var presentInfo = new VkPresentInfoKHR
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PRESENT_INFO_KHR,
            pNext = IntPtr.Zero,
            waitSemaphoreCount = waitSemaphore == IntPtr.Zero ? 0u : 1u,
            pWaitSemaphores = waitSemaphore == IntPtr.Zero ? null : &waitSemaphore,
            swapchainCount = 1,
            pSwapchains = &swapchain,
            pImageIndices = &imgIdx,
            pResults = null,
        };

        VkResult result = VkApi.vkQueuePresentKHR(queue, in presentInfo);
        if (result == VkResult.VK_ERROR_OUT_OF_DATE_KHR || result == VkResult.VK_SUBOPTIMAL_KHR)
        {
            return true;
        }
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkQueuePresentKHR failed: {result}");
        }
        return false;
    }

    /// <summary>
    /// The ELT §2.5 PREPARE step: build a new VkSwapchainKHR (with <paramref name="oldSwapchain"/>
    /// as the recreation hint) and its per-image views into fresh locals, WITHOUT touching any
    /// instance field. On any failure the partial new resources are reclaimed and the exception
    /// propagates -- so the caller's old state is never disturbed.
    /// </summary>
    private unsafe SwapchainResources PrepareSwapchain(uint width, uint height, IntPtr oldSwapchain)
    {
        // Query surface capabilities.
        VkResult capResult = VkApi.vkGetPhysicalDeviceSurfaceCapabilitiesKHR(
            _physicalDevice, _surface, out VkSurfaceCapabilitiesKHR caps);
        if (capResult != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkGetPhysicalDeviceSurfaceCapabilitiesKHR failed: {capResult}");
        }

        // Choose extent (currentExtent == 0xFFFFFFFF means surface size determined by swapchain;
        // clamp к min/max capabilities).
        uint actualWidth = caps.currentExtent.width != uint.MaxValue ? caps.currentExtent.width
            : Math.Clamp(width, caps.minImageExtent.width, caps.maxImageExtent.width);
        uint actualHeight = caps.currentExtent.height != uint.MaxValue ? caps.currentExtent.height
            : Math.Clamp(height, caps.minImageExtent.height, caps.maxImageExtent.height);

        // Choose image count: min + 1 if not capped by max.
        uint imageCount = caps.minImageCount + 1;
        if (caps.maxImageCount > 0 && imageCount > caps.maxImageCount)
        {
            imageCount = caps.maxImageCount;
        }

        // Choose format: prefer B8G8R8A8_SRGB + SRGB_NONLINEAR.
        uint fmtCount = 0;
        VkApi.vkGetPhysicalDeviceSurfaceFormatsKHR(_physicalDevice, _surface, ref fmtCount, null);
        var formats = new VkSurfaceFormatKHR[fmtCount];
        fixed (VkSurfaceFormatKHR* fmtPtr = formats)
        {
            VkApi.vkGetPhysicalDeviceSurfaceFormatsKHR(_physicalDevice, _surface, ref fmtCount, fmtPtr);
        }
        VkSurfaceFormatKHR chosenFormat = formats[0];
        foreach (VkSurfaceFormatKHR f in formats)
        {
            if (f.format == VkFormat.VK_FORMAT_B8G8R8A8_SRGB
                && f.colorSpace == VkColorSpaceKHR.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR)
            {
                chosenFormat = f;
                break;
            }
        }

        // Choose present mode: prefer MAILBOX, fallback FIFO (guaranteed available).
        uint pmCount = 0;
        VkApi.vkGetPhysicalDeviceSurfacePresentModesKHR(_physicalDevice, _surface, ref pmCount, null);
        var presentModes = new VkPresentModeKHR[pmCount];
        fixed (VkPresentModeKHR* pmPtr = presentModes)
        {
            VkApi.vkGetPhysicalDeviceSurfacePresentModesKHR(_physicalDevice, _surface, ref pmCount, pmPtr);
        }
        VkPresentModeKHR chosenPresent = VkPresentModeKHR.VK_PRESENT_MODE_FIFO_KHR;
        foreach (VkPresentModeKHR m in presentModes)
        {
            if (m == VkPresentModeKHR.VK_PRESENT_MODE_MAILBOX_KHR)
            {
                chosenPresent = m;
                break;
            }
        }

        var createInfo = new VkSwapchainCreateInfoKHR
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR,
            pNext = IntPtr.Zero,
            flags = 0,
            surface = _surface,
            minImageCount = imageCount,
            imageFormat = chosenFormat.format,
            imageColorSpace = chosenFormat.colorSpace,
            imageExtent = new VkExtent2D { width = actualWidth, height = actualHeight },
            imageArrayLayers = 1,
            imageUsage = VkImageUsageFlags.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
            imageSharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE,
            queueFamilyIndexCount = 0,
            pQueueFamilyIndices = null,
            preTransform = caps.currentTransform,
            compositeAlpha = VkCompositeAlphaFlagsKHR.VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR,
            presentMode = chosenPresent,
            clipped = 1,    // VK_TRUE
            oldSwapchain = oldSwapchain,
        };

        VkResult result = VkApi.vkCreateSwapchainKHR(_device, in createInfo, IntPtr.Zero, out IntPtr newSwapchain);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateSwapchainKHR failed: {result}");
        }

        try
        {
            // Enumerate images of the NEW swapchain.
            uint count = 0;
            VkApi.vkGetSwapchainImagesKHR(_device, newSwapchain, ref count, null);
            var rawImages = new IntPtr[count];
            fixed (IntPtr* imgsPtr = rawImages)
            {
                VkApi.vkGetSwapchainImagesKHR(_device, newSwapchain, ref count, imgsPtr);
            }

            // Build the per-image views all-or-nothing (ELT §2.5 prepare): a mid-loop
            // vkCreateImageView failure rolls back the views already built in this call.
            SwapchainImage[] newImages = PrepareBeforeReclaim.Build(
                rawImages,
                build: img => new SwapchainImage(img, CreateImageView(img, chosenFormat.format)),
                reclaim: si =>
                {
                    if (si.ImageViewHandle != IntPtr.Zero)
                    {
                        VkApi.vkDestroyImageView(_device, si.ImageViewHandle, IntPtr.Zero);
                    }
                });

            return new SwapchainResources(
                newSwapchain,
                newImages,
                (VkFormatPublic)(int)chosenFormat.format,
                actualWidth,
                actualHeight,
                (VkPresentModePublic)(int)chosenPresent);
        }
        catch
        {
            // Any new views are already reclaimed by Build; drop the new swapchain too so a
            // failed prepare leaks nothing and leaves the caller's old state fully intact.
            VkApi.vkDestroySwapchainKHR(_device, newSwapchain, IntPtr.Zero);
            throw;
        }
    }

    private unsafe IntPtr CreateImageView(IntPtr image, VkFormat format)
    {
        var viewInfo = new VkImageViewCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            image = image,
            viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
            format = format,
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
        VkResult viewResult = VkApi.vkCreateImageView(_device, in viewInfo, IntPtr.Zero, out IntPtr view);
        if (viewResult != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateImageView failed: {viewResult}");
        }
        return view;
    }

    private void DestroyImageViews(SwapchainImage[] images)
    {
        foreach (SwapchainImage img in images)
        {
            if (img.ImageViewHandle != IntPtr.Zero)
            {
                VkApi.vkDestroyImageView(_device, img.ImageViewHandle, IntPtr.Zero);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        DestroyImageViews(_images);
        _images = Array.Empty<SwapchainImage>();
        if (_swapchain != IntPtr.Zero)
        {
            VkApi.vkDestroySwapchainKHR(_device, _swapchain, IntPtr.Zero);
            _swapchain = IntPtr.Zero;
        }
        _disposed = true;
    }

    /// <summary>Prepared-but-not-yet-committed swapchain resources (the ELT §2.5 prepare output).</summary>
    private readonly record struct SwapchainResources(
        IntPtr Swapchain,
        SwapchainImage[] Images,
        VkFormatPublic Format,
        uint Width,
        uint Height,
        VkPresentModePublic PresentMode);
}

/// <summary>Public-facing mirror of VkPresentModeKHR.</summary>
public enum VkPresentModePublic : int
{
    Immediate = 0,
    Mailbox = 1,
    Fifo = 2,
    FifoRelaxed = 3,
}
