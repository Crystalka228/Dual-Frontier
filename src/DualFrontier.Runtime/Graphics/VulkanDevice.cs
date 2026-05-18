using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Vulkan physical + logical device lifecycle. V0.A scope: enumerates physical devices,
/// selects one с graphics queue family (prefer discrete GPU), creates logical device с graphics
/// queue. Async compute queue family selection deferred к V0.B (per S-LOCK-3 + К10.3 brief
/// Item 43).
/// </summary>
public sealed class VulkanDevice : IDisposable
{
    private readonly IntPtr _instance;
    private IntPtr _physicalDevice;
    private IntPtr _device;
    private uint _graphicsQueueFamilyIndex;
    private IntPtr _graphicsQueue;
    private bool _disposed;

    public IntPtr Handle => _device;
    public IntPtr PhysicalDevice => _physicalDevice;
    public IntPtr GraphicsQueue => _graphicsQueue;
    public uint GraphicsQueueFamilyIndex => _graphicsQueueFamilyIndex;
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
        var queueCreateInfo = new VkDeviceQueueCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            queueFamilyIndex = _graphicsQueueFamilyIndex,
            queueCount = 1,
            pQueuePriorities = &queuePriority,
        };

        var createInfo = new VkDeviceCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            queueCreateInfoCount = 1,
            pQueueCreateInfos = &queueCreateInfo,
            enabledLayerCount = 0,
            ppEnabledLayerNames = null,
            enabledExtensionCount = 0,
            ppEnabledExtensionNames = null,
            pEnabledFeatures = IntPtr.Zero,
        };

        VkResult result = VkApi.vkCreateDevice(_physicalDevice, in createInfo, IntPtr.Zero, out _device);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateDevice failed: {result}");
        }

        VkApi.vkGetDeviceQueue(_device, _graphicsQueueFamilyIndex, 0, out _graphicsQueue);
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
