namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Diagnostic info about a Vulkan queue family. Materialized at <see cref="VulkanDevice"/>
/// physical device enumeration; exposed via <see cref="PhysicalDeviceInfo.QueueFamilies"/>.
/// Vulkan-specific <c>VkQueueFlags</c> decomposed into per-capability bool fields для clean
/// public API.
/// </summary>
public sealed record QueueFamilyInfo(
    uint Index,
    uint QueueCount,
    bool SupportsGraphics,
    bool SupportsCompute,
    bool SupportsTransfer,
    bool SupportsSparseBinding);
