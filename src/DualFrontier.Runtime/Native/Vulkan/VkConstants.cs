namespace DualFrontier.Runtime.Native.Vulkan;

internal static class VkConstants
{
    // VK_MAKE_API_VERSION(variant, major, minor, patch) = (variant << 29) | (major << 22) | (minor << 12) | patch
    // V0.A only needs major+minor; patch+variant set to zero.
    internal const uint VK_API_VERSION_1_0 = (1u << 22) | (0u << 12);
    internal const uint VK_API_VERSION_1_3 = (1u << 22) | (3u << 12);

    // Layer + extension names (UTF-8 strings — see Vulkan 1.3 spec)
    internal const string VK_LAYER_KHRONOS_VALIDATION = "VK_LAYER_KHRONOS_validation";
    internal const string VK_EXT_DEBUG_UTILS_EXTENSION_NAME = "VK_EXT_debug_utils";
    internal const string VK_KHR_SURFACE_EXTENSION_NAME = "VK_KHR_surface";
    internal const string VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface";
    internal const string VK_KHR_SWAPCHAIN_EXTENSION_NAME = "VK_KHR_swapchain";

    // Sizes
    internal const int VK_MAX_PHYSICAL_DEVICE_NAME_SIZE = 256;
    internal const int VK_UUID_SIZE = 16;

    // VkPhysicalDeviceLimits opaque size (Vulkan 1.3 spec on x64)
    internal const int VK_PHYSICAL_DEVICE_LIMITS_SIZE = 504;
    internal const int VK_PHYSICAL_DEVICE_SPARSE_PROPERTIES_SIZE = 20;

    // VkPhysicalDeviceMemoryProperties max array sizes
    internal const int VK_MAX_MEMORY_TYPES = 32;
    internal const int VK_MAX_MEMORY_HEAPS = 16;
}
