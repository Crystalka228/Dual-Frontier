using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// К-L19 Item 44: startup fail-fast hardware tier check. Verifies the host satisfies К-L19
/// hardware tier commitment (Vulkan 1.3 + async compute queue family). Failure throws
/// <see cref="HardwareCapabilityException"/> с user-facing diagnostic message.
/// Invoked by <see cref="Runtime.Create"/> immediately after <see cref="VulkanDevice"/>
/// construction — no further substrate composition occurs if hardware tier rejected.
/// </summary>
public static class HardwareCapabilityCheck
{
    public static void Verify(VulkanInstance vulkan, VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(vulkan);
        ArgumentNullException.ThrowIfNull(device);
        VerifyVulkanApiVersion(vulkan.ApiVersion);
        VerifyAsyncComputeQueueFamily(device.AsyncComputeQueueFamilyIndex, device.SelectedDevice.DeviceName);
    }

    /// <summary>К-L19 sub-check: Vulkan 1.3 API version mandate.</summary>
    internal static void VerifyVulkanApiVersion(uint apiVersion)
    {
        if (apiVersion < VkConstants.VK_API_VERSION_1_3)
        {
            throw new HardwareCapabilityException(
                "Dual Frontier requires Vulkan 1.3 API version. " +
                $"Detected version: 0x{apiVersion:X}. " +
                "Upgrade GPU driver or install Vulkan 1.3 capable hardware. " +
                "See README.md hardware requirements section.");
        }
    }

    /// <summary>К-L19 sub-check: async compute queue family mandate.</summary>
    internal static void VerifyAsyncComputeQueueFamily(uint? asyncComputeQueueFamilyIndex, string deviceName)
    {
        if (!asyncComputeQueueFamilyIndex.HasValue)
        {
            throw new HardwareCapabilityException(
                "Dual Frontier requires Vulkan 1.3 hardware с async compute queue family support. " +
                $"Detected GPU '{deviceName}' does not expose compute-capable queue family. " +
                "Required hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx и newer), " +
                "AMD RDNA 1+ (RX 5500 и newer), Intel Arc Alchemist+ (A380 и newer). " +
                "See README.md hardware requirements section.");
        }
    }
}
