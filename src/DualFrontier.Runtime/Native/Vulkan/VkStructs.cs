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

// ===========================================================================
// V0.B Commit 6 — Memory allocator + buffer/image primitives
// ===========================================================================

// VkExtent2D — 2D extent (8 bytes, two uint32, 4-byte alignment).
[StructLayout(LayoutKind.Sequential)]
internal struct VkExtent2D
{
    internal uint width;
    internal uint height;
}

// VkExtent3D — 3D extent (12 bytes, three uint32, 4-byte alignment).
[StructLayout(LayoutKind.Sequential)]
internal struct VkExtent3D
{
    internal uint width;
    internal uint height;
    internal uint depth;
}

// VkRect2D (16 bytes — offset + extent).
[StructLayout(LayoutKind.Sequential)]
internal struct VkRect2D
{
    internal int offsetX;
    internal int offsetY;
    internal uint width;
    internal uint height;
}

// VkComponentMapping (16 bytes — 4 VkComponentSwizzle enums each 4 bytes).
[StructLayout(LayoutKind.Sequential)]
internal struct VkComponentMapping
{
    internal VkComponentSwizzle r;
    internal VkComponentSwizzle g;
    internal VkComponentSwizzle b;
    internal VkComponentSwizzle a;
}

// VkImageSubresourceRange (20 bytes — aspectMask + 4 uint32).
[StructLayout(LayoutKind.Sequential)]
internal struct VkImageSubresourceRange
{
    internal VkImageAspectFlags aspectMask;
    internal uint baseMipLevel;
    internal uint levelCount;
    internal uint baseArrayLayer;
    internal uint layerCount;
}

// VkMemoryType (8 bytes — two uint32, 4-byte alignment).
[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryType
{
    internal VkMemoryPropertyFlags propertyFlags;
    internal uint heapIndex;
}

// VkMemoryHeap (16 bytes — VkDeviceSize size + uint32 flags + 4-byte trailing pad).
[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryHeap
{
    internal ulong size;            // VkDeviceSize (uint64)
    internal VkMemoryHeapFlags flags;
    // 4-byte trailing pad implicit (struct contains 8-byte field → 8-byte alignment).
    internal uint _padTrailing;
}

// VkMemoryRequirements (24 bytes — two VkDeviceSize + memoryTypeBits + trailing pad).
// Per Vulkan 1.3 spec on x64 MSVC ABI:
//   size (8) + alignment (8) + memoryTypeBits (4) + trailing pad (4) = 24
[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryRequirements
{
    internal ulong size;          // VkDeviceSize
    internal ulong alignment;     // VkDeviceSize
    internal uint memoryTypeBits;
    internal uint _padTrailing;
}

// VkPhysicalDeviceMemoryProperties (520 bytes on x64).
// Inline arrays via fixed byte buffers — accessed through helper methods. C struct layout:
//   memoryTypeCount uint32 (4) + memoryTypes[32 × 8 = 256] (256) + memoryHeapCount uint32 (4)
//   + memoryHeaps[16 × 16 = 256] (256) = 520. No pad before memoryHeaps because
//   memoryHeapCount ends at offset 264 (already 8-aligned).
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPhysicalDeviceMemoryProperties
{
    internal uint memoryTypeCount;
    internal fixed byte _memoryTypesData[VkConstants.VK_MAX_MEMORY_TYPES * 8];
    internal uint memoryHeapCount;
    internal fixed byte _memoryHeapsData[VkConstants.VK_MAX_MEMORY_HEAPS * 16];

    internal VkMemoryType GetMemoryType(int index)
    {
        fixed (byte* p = _memoryTypesData)
        {
            return ((VkMemoryType*)p)[index];
        }
    }

    internal VkMemoryHeap GetMemoryHeap(int index)
    {
        fixed (byte* p = _memoryHeapsData)
        {
            return ((VkMemoryHeap*)p)[index];
        }
    }
}

// VkMemoryAllocateInfo (32 bytes — sType+pad+pNext+allocationSize+memoryTypeIndex+trailing pad).
// Brief originally stated 24 bytes — corrected к 32 per Vulkan 1.3 spec on x64.
[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryAllocateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal ulong allocationSize;  // VkDeviceSize
    internal uint memoryTypeIndex;
    internal uint _padTrailing;
}

// VkBufferCreateInfo (56 bytes per Vulkan 1.3 spec on x64).
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkBufferCreateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal uint _padBeforeSize;     // VkDeviceSize requires 8-byte alignment
    internal ulong size;
    internal VkBufferUsageFlags usage;
    internal VkSharingMode sharingMode;
    internal uint queueFamilyIndexCount;
    internal uint _padBeforePtr;
    internal uint* pQueueFamilyIndices;
}

// VkImageCreateInfo (88 bytes per Vulkan 1.3 spec on x64).
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkImageCreateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal VkImageType imageType;
    internal VkFormat format;
    internal VkExtent3D extent;        // 12 bytes inline
    internal uint mipLevels;
    internal uint arrayLayers;
    internal VkSampleCountFlagBits samples;
    internal VkImageTiling tiling;
    internal VkImageUsageFlags usage;
    internal VkSharingMode sharingMode;
    internal uint queueFamilyIndexCount;
    internal uint _padBeforePtr;
    internal uint* pQueueFamilyIndices;
    internal VkImageLayout initialLayout;
    internal uint _padTrailing;
}

// VkImageViewCreateInfo (80 bytes per Vulkan 1.3 spec on x64).
[StructLayout(LayoutKind.Sequential)]
internal struct VkImageViewCreateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal uint _padBeforeImage;
    internal IntPtr image;
    internal VkImageViewType viewType;
    internal VkFormat format;
    internal VkComponentMapping components;     // 16 bytes
    internal VkImageSubresourceRange subresourceRange;  // 20 bytes
    internal uint _padTrailing;
}
