using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

internal static unsafe partial class VkApi
{
    private const string VulkanLib = "vulkan-1.dll";

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateInstance")]
    internal static partial VkResult vkCreateInstance(
        in VkInstanceCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pInstance);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyInstance")]
    internal static partial void vkDestroyInstance(IntPtr instance, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkEnumerateInstanceVersion")]
    internal static partial VkResult vkEnumerateInstanceVersion(out uint pApiVersion);

    [LibraryImport(VulkanLib, EntryPoint = "vkEnumeratePhysicalDevices")]
    internal static partial VkResult vkEnumeratePhysicalDevices(
        IntPtr instance,
        ref uint pPhysicalDeviceCount,
        IntPtr* pPhysicalDevices);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceProperties")]
    internal static partial void vkGetPhysicalDeviceProperties(
        IntPtr physicalDevice,
        out VkPhysicalDeviceProperties pProperties);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceQueueFamilyProperties")]
    internal static partial void vkGetPhysicalDeviceQueueFamilyProperties(
        IntPtr physicalDevice,
        ref uint pQueueFamilyPropertyCount,
        VkQueueFamilyProperties* pQueueFamilyProperties);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateDevice")]
    internal static partial VkResult vkCreateDevice(
        IntPtr physicalDevice,
        in VkDeviceCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pDevice);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyDevice")]
    internal static partial void vkDestroyDevice(IntPtr device, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetDeviceQueue")]
    internal static partial void vkGetDeviceQueue(
        IntPtr device,
        uint queueFamilyIndex,
        uint queueIndex,
        out IntPtr pQueue);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetInstanceProcAddr",
        StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr vkGetInstanceProcAddr(IntPtr instance, string pName);

    // Note: vkCreateDebugUtilsMessengerEXT / vkDestroyDebugUtilsMessengerEXT are extension
    // functions — not direct exports of vulkan-1.dll. Loaded at runtime via
    // vkGetInstanceProcAddr by ValidationLayer (Commit 7).
}
