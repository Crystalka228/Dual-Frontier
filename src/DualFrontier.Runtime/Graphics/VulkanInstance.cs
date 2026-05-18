using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkInstance lifecycle. Owns Vulkan 1.3 API version verification (К-L19 hardware tier surface),
/// validation layer + extension activation (when enableValidation: true). Disposed via vkDestroyInstance.
/// </summary>
public sealed class VulkanInstance : IDisposable
{
    private IntPtr _instance;
    private bool _disposed;

    public IntPtr Handle => _instance;
    public uint ApiVersion { get; private set; }
    public bool ValidationLayerEnabled { get; }

    public VulkanInstance(bool enableValidation)
    {
        ValidationLayerEnabled = enableValidation;
        VerifyVulkanApiVersion();
        CreateInstance();
    }

    private static void VerifyVulkanApiVersion()
    {
        var result = VkApi.vkEnumerateInstanceVersion(out uint apiVersion);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException(
                $"vkEnumerateInstanceVersion failed: {result}");
        }
        if (apiVersion < VkConstants.VK_API_VERSION_1_3)
        {
            throw new InvalidOperationException(
                $"Vulkan 1.3 required (К-L19 hardware tier). Detected API version: 0x{apiVersion:X}. " +
                "Upgrade GPU driver or install Vulkan 1.3 capable hardware. " +
                "See docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md К-L19 для architectural rationale.");
        }
    }

    private unsafe void CreateInstance()
    {
        IntPtr appNamePtr = Marshal.StringToCoTaskMemUTF8("Dual Frontier");
        IntPtr engineNamePtr = Marshal.StringToCoTaskMemUTF8("Dual Frontier V Substrate");

        var extensionNames = new List<string>
        {
            VkConstants.VK_KHR_SURFACE_EXTENSION_NAME,
            VkConstants.VK_KHR_WIN32_SURFACE_EXTENSION_NAME,
        };
        var layerNames = new List<string>();

        if (ValidationLayerEnabled)
        {
            extensionNames.Add(VkConstants.VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
            layerNames.Add(VkConstants.VK_LAYER_KHRONOS_VALIDATION);
        }

        var extPtrs = new IntPtr[extensionNames.Count];
        var layerPtrs = new IntPtr[layerNames.Count];

        try
        {
            for (int i = 0; i < extensionNames.Count; i++)
            {
                extPtrs[i] = Marshal.StringToCoTaskMemUTF8(extensionNames[i]);
            }
            for (int i = 0; i < layerNames.Count; i++)
            {
                layerPtrs[i] = Marshal.StringToCoTaskMemUTF8(layerNames[i]);
            }

            var appInfo = new VkApplicationInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
                pNext = IntPtr.Zero,
                pApplicationName = (byte*)appNamePtr,
                applicationVersion = 1,
                pEngineName = (byte*)engineNamePtr,
                engineVersion = 1,
                apiVersion = VkConstants.VK_API_VERSION_1_3,
            };

            fixed (IntPtr* extPtrsPinned = extPtrs)
            fixed (IntPtr* layerPtrsPinned = layerPtrs)
            {
                var createInfo = new VkInstanceCreateInfo
                {
                    sType = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
                    pNext = IntPtr.Zero,
                    flags = 0,
                    pApplicationInfo = &appInfo,
                    enabledLayerCount = (uint)layerNames.Count,
                    ppEnabledLayerNames = (byte**)layerPtrsPinned,
                    enabledExtensionCount = (uint)extensionNames.Count,
                    ppEnabledExtensionNames = (byte**)extPtrsPinned,
                };

                VkResult result = VkApi.vkCreateInstance(in createInfo, IntPtr.Zero, out _instance);
                if (result != VkResult.VK_SUCCESS)
                {
                    string hint = ValidationLayerEnabled
                        ? " Verify VK_LAYER_KHRONOS_validation manifest available (LunarG Vulkan SDK installed)."
                        : string.Empty;
                    throw new InvalidOperationException(
                        $"vkCreateInstance failed: {result}.{hint}");
                }

                ApiVersion = VkConstants.VK_API_VERSION_1_3;
            }
        }
        finally
        {
            for (int i = 0; i < extPtrs.Length; i++)
            {
                if (extPtrs[i] != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(extPtrs[i]);
                }
            }
            for (int i = 0; i < layerPtrs.Length; i++)
            {
                if (layerPtrs[i] != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(layerPtrs[i]);
                }
            }
            Marshal.FreeCoTaskMem(appNamePtr);
            Marshal.FreeCoTaskMem(engineNamePtr);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_instance != IntPtr.Zero)
        {
            VkApi.vkDestroyInstance(_instance, IntPtr.Zero);
            _instance = IntPtr.Zero;
        }
        _disposed = true;
    }
}
