using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

// Debug messenger callback signature per VK_EXT_debug_utils specification.
// Used as [UnmanagedCallersOnly] static method (function pointer) in ValidationLayer; this
// delegate type also kept as fallback marshalling path for extension function loading via
// Marshal.GetDelegateForFunctionPointer (vkCreateDebugUtilsMessengerEXT entry).
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate uint DebugUtilsMessengerCallback(
    VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
    VkDebugUtilsMessageTypeFlagsEXT messageType,
    IntPtr pCallbackData,
    IntPtr pUserData);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate VkResult CreateDebugUtilsMessengerDelegate(
    IntPtr instance,
    in VkDebugUtilsMessengerCreateInfoEXT pCreateInfo,
    IntPtr pAllocator,
    out IntPtr pMessenger);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void DestroyDebugUtilsMessengerDelegate(
    IntPtr instance,
    IntPtr messenger,
    IntPtr pAllocator);
