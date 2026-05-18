namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Diagnostic info about a Vulkan physical device. Materialized at <see cref="VulkanDevice"/>
/// enumeration; exposed via <see cref="VulkanDevice.AvailableDevices"/> (all) +
/// <see cref="VulkanDevice.SelectedDevice"/> (chosen one). Vulkan-specific
/// <c>VkPhysicalDeviceType</c> mapped к public <see cref="PhysicalDeviceType"/>.
/// </summary>
public sealed record PhysicalDeviceInfo(
    IntPtr Handle,
    string DeviceName,
    PhysicalDeviceType DeviceType,
    uint VendorId,
    uint DeviceId,
    uint ApiVersion,
    uint DriverVersion,
    IReadOnlyList<QueueFamilyInfo> QueueFamilies);
