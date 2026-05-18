using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkApplicationInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal byte* pApplicationName;
    internal uint applicationVersion;
    internal byte* pEngineName;
    internal uint engineVersion;
    internal uint apiVersion;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkInstanceCreateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal VkApplicationInfo* pApplicationInfo;
    internal uint enabledLayerCount;
    internal byte** ppEnabledLayerNames;
    internal uint enabledExtensionCount;
    internal byte** ppEnabledExtensionNames;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPhysicalDeviceProperties
{
    internal uint apiVersion;                                                       // offset 0
    internal uint driverVersion;                                                    // offset 4
    internal uint vendorID;                                                         // offset 8
    internal uint deviceID;                                                         // offset 12
    internal VkPhysicalDeviceType deviceType;                                       // offset 16
    internal fixed byte deviceName[VkConstants.VK_MAX_PHYSICAL_DEVICE_NAME_SIZE];   // offset 20
    internal fixed byte pipelineCacheUUID[VkConstants.VK_UUID_SIZE];                // offset 276
    // VkPhysicalDeviceLimits requires 8-byte alignment (contains VkDeviceSize/uint64 fields).
    // C compiler pads from offset 292 → 296 to align limits. Explicit pad here matches MSVC x64 ABI.
    internal fixed byte _padBeforeLimits[4];                                        // offset 292
    // Opaque: VkPhysicalDeviceLimits (504 bytes on x64 per Vulkan 1.3 spec).
    internal fixed byte limits[VkConstants.VK_PHYSICAL_DEVICE_LIMITS_SIZE];         // offset 296
    // Opaque: VkPhysicalDeviceSparseProperties (20 bytes, 5×VkBool32).
    internal fixed byte sparseProperties[VkConstants.VK_PHYSICAL_DEVICE_SPARSE_PROPERTIES_SIZE]; // offset 800
    // C struct trailing pad к 8-byte alignment: 820 → 824.
    internal fixed byte _padTrailing[4];                                            // offset 820
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkQueueFamilyProperties
{
    internal VkQueueFlags queueFlags;
    internal uint queueCount;
    internal uint timestampValidBits;
    // VkExtent3D minImageTransferGranularity (3×uint32 inline)
    internal uint minImageTransferGranularityWidth;
    internal uint minImageTransferGranularityHeight;
    internal uint minImageTransferGranularityDepth;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDeviceQueueCreateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal uint queueFamilyIndex;
    internal uint queueCount;
    internal float* pQueuePriorities;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDeviceCreateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal uint queueCreateInfoCount;
    internal VkDeviceQueueCreateInfo* pQueueCreateInfos;
    // enabledLayerCount + ppEnabledLayerNames deprecated в Vulkan 1.3 (instance-level layers
    // handle device-side, see VK_EXT_layer_settings); preserved для ABI compatibility.
    internal uint enabledLayerCount;
    internal byte** ppEnabledLayerNames;
    internal uint enabledExtensionCount;
    internal byte** ppEnabledExtensionNames;
    // VkPhysicalDeviceFeatures* — V0.A passes IntPtr.Zero (no required features).
    internal IntPtr pEnabledFeatures;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDebugUtilsMessengerCreateInfoEXT
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal VkDebugUtilsMessageSeverityFlagsEXT messageSeverity;
    internal VkDebugUtilsMessageTypeFlagsEXT messageType;
    internal IntPtr pfnUserCallback;
    internal IntPtr pUserData;
}

// VkDebugUtilsMessengerCallbackDataEXT — passed to debug messenger callback at runtime.
// V0.A consumes pMessage field for log capture (per ValidationLayer Commit 7); other fields
// (pMessageIdName, messageIdNumber, queueLabel/cmdBufLabel/object arrays) ignored for V0.A.
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkDebugUtilsMessengerCallbackDataEXT
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal byte* pMessageIdName;
    internal int messageIdNumber;
    internal byte* pMessage;
    internal uint queueLabelCount;
    internal IntPtr pQueueLabels;
    internal uint cmdBufLabelCount;
    internal IntPtr pCmdBufLabels;
    internal uint objectCount;
    internal IntPtr pObjects;
}

// ===========================================================================
// V0.B Commit 3 — Win32 surface foundation
// ===========================================================================

// VkWin32SurfaceCreateInfoKHR (VK_KHR_win32_surface extension)
// Per Vulkan 1.3 spec on x64 MSVC ABI: 40 bytes total.
// Layout (C#/CLR Sequential auto-pads IntPtr to 8-byte alignment):
//   sType (4) + pad (4) + pNext (8) + flags (4) + pad (4) + hinstance (8) + hwnd (8) = 40
[StructLayout(LayoutKind.Sequential)]
internal struct VkWin32SurfaceCreateInfoKHR
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal IntPtr hinstance;
    internal IntPtr hwnd;
}
