namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Per-swapchain-image data: driver-allocated VkImage handle + per-image VkImageView created
/// by <see cref="VulkanSwapchain"/>. Image not owned by us — destroyed via vkDestroySwapchainKHR.
/// View is owned by VulkanSwapchain и disposed alongside.
/// </summary>
public readonly record struct SwapchainImage(IntPtr ImageHandle, IntPtr ImageViewHandle);
