using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Vulkan physical + logical device lifecycle. V0.A scope: enumerated physical devices, selected
/// one с graphics queue family (prefer discrete GPU), created logical device с graphics queue.
/// V0.B adds: async compute queue family selection (К-L19 Item 43 — prefer dedicated compute-only
/// queue family, fallback к compute-capable graphics queue family); VK_KHR_swapchain device
/// extension activation; logical device requests both graphics + async compute queues when
/// distinct families exist.
/// </summary>
public sealed class VulkanDevice : IDisposable
{
    private readonly IntPtr _instance;
    private IntPtr _physicalDevice;
    private IntPtr _device;
    private uint _graphicsQueueFamilyIndex;
    private uint? _asyncComputeQueueFamilyIndex;
    private IntPtr _graphicsQueue;
    private IntPtr _asyncComputeQueue;
    private bool _disposed;

    public IntPtr Handle => _device;
    public IntPtr PhysicalDevice => _physicalDevice;
    public IntPtr GraphicsQueue => _graphicsQueue;
    public uint GraphicsQueueFamilyIndex => _graphicsQueueFamilyIndex;

    /// <summary>
    /// Async compute queue handle. Populated when the selected physical device exposes a
    /// compute-capable queue family (К-L19 mandate enforced by HardwareCapabilityCheck).
    /// May equal <see cref="GraphicsQueue"/> if no dedicated compute family exists и the graphics
    /// family supports compute — in that case the device only has one queue created (sharing).
    /// </summary>
    public IntPtr AsyncComputeQueue => _asyncComputeQueue;

    /// <summary>
    /// Async compute queue family index. <see langword="null"/> if no compute-capable queue family
    /// found on selected device (HardwareCapabilityCheck throws in that case per К-L19).
    /// </summary>
    public uint? AsyncComputeQueueFamilyIndex => _asyncComputeQueueFamilyIndex;

    public PhysicalDeviceInfo SelectedDevice { get; private set; } = null!;
    public IReadOnlyList<PhysicalDeviceInfo> AvailableDevices { get; private set; } = Array.Empty<PhysicalDeviceInfo>();

    public VulkanDevice(VulkanInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _instance = instance.Handle;
        EnumeratePhysicalDevices();
        SelectPhysicalDevice();
        CreateLogicalDevice();
    }

    private unsafe void EnumeratePhysicalDevices()
    {
        uint count = 0;
        VkResult enumResult = VkApi.vkEnumeratePhysicalDevices(_instance, ref count, null);
        if (enumResult != VkResult.VK_SUCCESS && enumResult != VkResult.VK_INCOMPLETE)
        {
            throw new InvalidOperationException(
                $"vkEnumeratePhysicalDevices (count query) failed: {enumResult}");
        }
        if (count == 0)
        {
            throw new InvalidOperationException(
                "No Vulkan physical devices found. Verify GPU driver installation.");
        }

        var handles = new IntPtr[count];
        fixed (IntPtr* handlesPtr = handles)
        {
            enumResult = VkApi.vkEnumeratePhysicalDevices(_instance, ref count, handlesPtr);
        }
        if (enumResult != VkResult.VK_SUCCESS && enumResult != VkResult.VK_INCOMPLETE)
        {
            throw new InvalidOperationException(
                $"vkEnumeratePhysicalDevices (fill query) failed: {enumResult}");
        }

        var devices = new List<PhysicalDeviceInfo>((int)count);
        foreach (IntPtr handle in handles)
        {
            VkApi.vkGetPhysicalDeviceProperties(handle, out VkPhysicalDeviceProperties props);

            // Fixed-size buffer `deviceName` in props is intrinsically pinned via the
            // containing struct on stack; implicit pointer conversion via props.deviceName.
            string deviceName = Marshal.PtrToStringUTF8((IntPtr)props.deviceName) ?? "<unknown>";

            uint qfCount = 0;
            VkApi.vkGetPhysicalDeviceQueueFamilyProperties(handle, ref qfCount, null);
            var qfProps = new VkQueueFamilyProperties[qfCount];
            fixed (VkQueueFamilyProperties* qfPtr = qfProps)
            {
                VkApi.vkGetPhysicalDeviceQueueFamilyProperties(handle, ref qfCount, qfPtr);
            }

            var queueFamilies = new List<QueueFamilyInfo>((int)qfCount);
            for (uint i = 0; i < qfCount; i++)
            {
                VkQueueFamilyProperties qf = qfProps[i];
                queueFamilies.Add(new QueueFamilyInfo(
                    Index: i,
                    QueueCount: qf.queueCount,
                    SupportsGraphics: (qf.queueFlags & VkQueueFlags.VK_QUEUE_GRAPHICS_BIT) != 0,
                    SupportsCompute: (qf.queueFlags & VkQueueFlags.VK_QUEUE_COMPUTE_BIT) != 0,
                    SupportsTransfer: (qf.queueFlags & VkQueueFlags.VK_QUEUE_TRANSFER_BIT) != 0,
                    SupportsSparseBinding: (qf.queueFlags & VkQueueFlags.VK_QUEUE_SPARSE_BINDING_BIT) != 0));
            }

            devices.Add(new PhysicalDeviceInfo(
                Handle: handle,
                DeviceName: deviceName,
                DeviceType: (PhysicalDeviceType)(int)props.deviceType,
                VendorId: props.vendorID,
                DeviceId: props.deviceID,
                ApiVersion: props.apiVersion,
                DriverVersion: props.driverVersion,
                QueueFamilies: queueFamilies));
        }

        AvailableDevices = devices;
    }

    private void SelectPhysicalDevice()
    {
        PhysicalDeviceInfo? selected = null;

        // Prefer discrete GPU с graphics queue family
        foreach (PhysicalDeviceInfo device in AvailableDevices)
        {
            if (device.DeviceType == PhysicalDeviceType.DiscreteGpu
                && HasGraphicsQueueFamily(device))
            {
                selected = device;
                break;
            }
        }

        // Fallback: any device с graphics queue family
        if (selected is null)
        {
            foreach (PhysicalDeviceInfo device in AvailableDevices)
            {
                if (HasGraphicsQueueFamily(device))
                {
                    selected = device;
                    break;
                }
            }
        }

        if (selected is null)
        {
            throw new InvalidOperationException(
                "No suitable Vulkan physical device found. Graphics queue family required.");
        }

        SelectedDevice = selected;
        _physicalDevice = selected.Handle;

        foreach (QueueFamilyInfo qf in selected.QueueFamilies)
        {
            if (qf.SupportsGraphics)
            {
                _graphicsQueueFamilyIndex = qf.Index;
                break;
            }
        }

        // V0.B: async compute queue family selection (К-L19 Item 43). May resolve null if
        // device exposes graphics queue family that lacks compute bit (unusual on К-L19
        // hardware tier; HardwareCapabilityCheck throws fail-fast in that case).
        _asyncComputeQueueFamilyIndex = FindAsyncComputeQueueFamilyIndex(selected.QueueFamilies);
    }

    /// <summary>
    /// К-L19 Item 43 selection algorithm. Prefer dedicated compute-only queue family
    /// (parallel compute alongside graphics — К-L16 pipeline depth dispatches); fallback к
    /// any compute-capable queue family (incl. graphics-bit queue).
    /// </summary>
    internal static uint? FindAsyncComputeQueueFamilyIndex(IReadOnlyList<QueueFamilyInfo> queueFamilies)
    {
        // Prefer dedicated compute-only queue family.
        foreach (QueueFamilyInfo qf in queueFamilies)
        {
            if (qf.SupportsCompute && !qf.SupportsGraphics)
            {
                return qf.Index;
            }
        }
        // Fallback: compute-capable graphics queue family.
        foreach (QueueFamilyInfo qf in queueFamilies)
        {
            if (qf.SupportsCompute)
            {
                return qf.Index;
            }
        }
        return null;
    }

    private static bool HasGraphicsQueueFamily(PhysicalDeviceInfo device)
    {
        foreach (QueueFamilyInfo qf in device.QueueFamilies)
        {
            if (qf.SupportsGraphics)
            {
                return true;
            }
        }
        return false;
    }

    private unsafe void CreateLogicalDevice()
    {
        float queuePriority = 1.0f;

        // Request graphics queue always. If async compute family is distinct, request additional queue.
        // Single-family compute (graphics QF supports compute too) gets queue handle from the same
        // graphics queue create info — `_asyncComputeQueue` aliases `_graphicsQueue` in that case.
        bool distinctAsyncCompute =
            _asyncComputeQueueFamilyIndex.HasValue
            && _asyncComputeQueueFamilyIndex.Value != _graphicsQueueFamilyIndex;

        Span<VkDeviceQueueCreateInfo> queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[distinctAsyncCompute ? 2 : 1];
        queueCreateInfos[0] = new VkDeviceQueueCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            queueFamilyIndex = _graphicsQueueFamilyIndex,
            queueCount = 1,
            pQueuePriorities = &queuePriority,
        };
        if (distinctAsyncCompute)
        {
            queueCreateInfos[1] = new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                queueFamilyIndex = _asyncComputeQueueFamilyIndex!.Value,
                queueCount = 1,
                pQueuePriorities = &queuePriority,
            };
        }

        // Device extensions: VK_KHR_swapchain required для V0.B swapchain (Commit 7).
        IntPtr swapchainNamePtr = Marshal.StringToCoTaskMemUTF8(VkConstants.VK_KHR_SWAPCHAIN_EXTENSION_NAME);
        var extPtrs = new[] { swapchainNamePtr };

        try
        {
            fixed (VkDeviceQueueCreateInfo* queuesPtr = queueCreateInfos)
            fixed (IntPtr* extPtrsPinned = extPtrs)
            {
                var createInfo = new VkDeviceCreateInfo
                {
                    sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
                    pNext = IntPtr.Zero,
                    flags = 0,
                    queueCreateInfoCount = (uint)queueCreateInfos.Length,
                    pQueueCreateInfos = queuesPtr,
                    enabledLayerCount = 0,
                    ppEnabledLayerNames = null,
                    enabledExtensionCount = 1,
                    ppEnabledExtensionNames = (byte**)extPtrsPinned,
                    pEnabledFeatures = IntPtr.Zero,
                };

                VkResult result = VkApi.vkCreateDevice(_physicalDevice, in createInfo, IntPtr.Zero, out _device);
                if (result != VkResult.VK_SUCCESS)
                {
                    throw new InvalidOperationException($"vkCreateDevice failed: {result}");
                }
            }

            VkApi.vkGetDeviceQueue(_device, _graphicsQueueFamilyIndex, 0, out _graphicsQueue);

            if (_asyncComputeQueueFamilyIndex.HasValue)
            {
                // For distinct family, this fetches the second queue; for shared family, it
                // aliases the graphics queue handle (Vulkan returns the same VkQueue).
                VkApi.vkGetDeviceQueue(_device, _asyncComputeQueueFamilyIndex.Value, 0, out _asyncComputeQueue);
            }
        }
        finally
        {
            Marshal.FreeCoTaskMem(swapchainNamePtr);
        }
    }

    /// <summary>Blocks until the device is idle. Required before swapchain recreation / Dispose.</summary>
    public void WaitIdle()
    {
        if (_device != IntPtr.Zero)
        {
            // M9 device-lost v1: classify device loss here (the VkResult was previously discarded
            // entirely). Other non-success results stay unthrown -- adding a generic throw to this
            // teardown-path call (Runtime.Dispose / LauncherRenderer.Shutdown) is out of M9 scope and
            // hazardous during Dispose unwinding; the residual is seeded as a ROADMAP F-finding.
            VkResult result = VkApi.vkDeviceWaitIdle(_device);
            DeviceLost.ThrowIfLost(result, new DeviceLostContext(VulkanCall.DeviceWaitIdle));
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_device != IntPtr.Zero)
        {
            VkApi.vkDestroyDevice(_device, IntPtr.Zero);
            _device = IntPtr.Zero;
        }
        _disposed = true;
    }
}
