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

    // =======================================================================
    // V0.B Commit 6 — Memory + buffer + image
    // =======================================================================

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceMemoryProperties")]
    internal static partial void vkGetPhysicalDeviceMemoryProperties(
        IntPtr physicalDevice,
        out VkPhysicalDeviceMemoryProperties pMemoryProperties);

    [LibraryImport(VulkanLib, EntryPoint = "vkAllocateMemory")]
    internal static partial VkResult vkAllocateMemory(
        IntPtr device,
        in VkMemoryAllocateInfo pAllocateInfo,
        IntPtr pAllocator,
        out IntPtr pMemory);

    [LibraryImport(VulkanLib, EntryPoint = "vkFreeMemory")]
    internal static partial void vkFreeMemory(IntPtr device, IntPtr memory, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkMapMemory")]
    internal static partial VkResult vkMapMemory(
        IntPtr device,
        IntPtr memory,
        ulong offset,
        ulong size,
        uint flags,
        out IntPtr ppData);

    [LibraryImport(VulkanLib, EntryPoint = "vkUnmapMemory")]
    internal static partial void vkUnmapMemory(IntPtr device, IntPtr memory);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateBuffer")]
    internal static partial VkResult vkCreateBuffer(
        IntPtr device,
        in VkBufferCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pBuffer);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyBuffer")]
    internal static partial void vkDestroyBuffer(IntPtr device, IntPtr buffer, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetBufferMemoryRequirements")]
    internal static partial void vkGetBufferMemoryRequirements(
        IntPtr device,
        IntPtr buffer,
        out VkMemoryRequirements pMemoryRequirements);

    [LibraryImport(VulkanLib, EntryPoint = "vkBindBufferMemory")]
    internal static partial VkResult vkBindBufferMemory(
        IntPtr device,
        IntPtr buffer,
        IntPtr memory,
        ulong memoryOffset);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateImage")]
    internal static partial VkResult vkCreateImage(
        IntPtr device,
        in VkImageCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pImage);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyImage")]
    internal static partial void vkDestroyImage(IntPtr device, IntPtr image, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetImageMemoryRequirements")]
    internal static partial void vkGetImageMemoryRequirements(
        IntPtr device,
        IntPtr image,
        out VkMemoryRequirements pMemoryRequirements);

    [LibraryImport(VulkanLib, EntryPoint = "vkBindImageMemory")]
    internal static partial VkResult vkBindImageMemory(
        IntPtr device,
        IntPtr image,
        IntPtr memory,
        ulong memoryOffset);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateImageView")]
    internal static partial VkResult vkCreateImageView(
        IntPtr device,
        in VkImageViewCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pView);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyImageView")]
    internal static partial void vkDestroyImageView(IntPtr device, IntPtr imageView, IntPtr pAllocator);
}
