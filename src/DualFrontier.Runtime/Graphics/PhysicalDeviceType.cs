namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Public mirror of <c>VkPhysicalDeviceType</c> for clean public API. Translation occurs
/// in <see cref="VulkanDevice"/> physical device enumeration (Native.Vulkan internal types
/// do not leak past Runtime project boundary per VULKAN_SUBSTRATE §2.4 Rule 5).
/// </summary>
public enum PhysicalDeviceType
{
    Other = 0,
    IntegratedGpu = 1,
    DiscreteGpu = 2,
    VirtualGpu = 3,
    Cpu = 4,
}
