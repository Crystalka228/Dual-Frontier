using DualFrontier.Runtime.Native.Vulkan;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

public sealed class VulkanDeviceMarshallingTests
{
    [Fact]
    public unsafe void VkPhysicalDeviceProperties_size_matches_x64_spec()
    {
        // Vulkan 1.3 spec on x64 (MSVC ABI с explicit padding для VkPhysicalDeviceLimits alignment):
        // - 5×uint32 header (apiVersion, driverVersion, vendorID, deviceID, deviceType) = 20
        // - deviceName[256] = 256
        // - pipelineCacheUUID[16] = 16
        // - pad к 8-byte alignment для limits = 4
        // - VkPhysicalDeviceLimits opaque = 504
        // - VkPhysicalDeviceSparseProperties opaque = 20
        // - trailing pad для struct 8-byte alignment = 4
        // Total = 824 bytes
        sizeof(VkPhysicalDeviceProperties).Should().Be(824);
    }

    [Fact]
    public unsafe void VkQueueFamilyProperties_size_matches_x64_spec()
    {
        // x64: queueFlags(4) + queueCount(4) + timestampValidBits(4)
        //    + VkExtent3D minImageTransferGranularity (3×uint32 = 12) = 24 bytes
        sizeof(VkQueueFamilyProperties).Should().Be(24);
    }

    [Fact]
    public unsafe void VkDeviceCreateInfo_size_matches_x64_layout()
    {
        // x64 layout: sType(4) + pad(4) + pNext(8) + flags(4) + queueCreateInfoCount(4)
        // + pQueueCreateInfos(8) + enabledLayerCount(4) + pad(4) + ppEnabledLayerNames(8)
        // + enabledExtensionCount(4) + pad(4) + ppEnabledExtensionNames(8) + pEnabledFeatures(8)
        // = 72 bytes
        sizeof(VkDeviceCreateInfo).Should().Be(72);
    }
}
