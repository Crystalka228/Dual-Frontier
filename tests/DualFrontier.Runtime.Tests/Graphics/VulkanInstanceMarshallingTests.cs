using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

public sealed class VulkanInstanceMarshallingTests
{
    [Fact]
    public unsafe void VkApplicationInfo_size_matches_x64_layout()
    {
        // x64 layout: sType(4) + pad(4) + pNext(8) + pApplicationName(8) + applicationVersion(4) + pad(4)
        //           + pEngineName(8) + engineVersion(4) + apiVersion(4) = 48 bytes
        sizeof(VkApplicationInfo).Should().Be(48);
    }

    [Fact]
    public unsafe void VkInstanceCreateInfo_size_matches_x64_layout()
    {
        // x64 layout: sType(4) + pad(4) + pNext(8) + flags(4) + pad(4) + pApplicationInfo(8)
        //           + enabledLayerCount(4) + pad(4) + ppEnabledLayerNames(8)
        //           + enabledExtensionCount(4) + pad(4) + ppEnabledExtensionNames(8) = 64 bytes
        sizeof(VkInstanceCreateInfo).Should().Be(64);
    }

    [Fact]
    public void Utf8_string_roundtrip_correct_for_layer_name()
    {
        // Vulkan layer names are UTF-8; Marshal.StringToCoTaskMemUTF8 is the canonical conversion.
        IntPtr ptr = Marshal.StringToCoTaskMemUTF8(VkConstants.VK_LAYER_KHRONOS_VALIDATION);
        try
        {
            string? roundtrip = Marshal.PtrToStringUTF8(ptr);
            roundtrip.Should().Be(VkConstants.VK_LAYER_KHRONOS_VALIDATION);
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    [Fact]
    public void VK_API_VERSION_1_3_matches_canonical_encoding()
    {
        // VK_MAKE_API_VERSION(0, 1, 3, 0) = (1 << 22) | (3 << 12) = 0x400000 | 0x3000 = 0x403000
        VkConstants.VK_API_VERSION_1_3.Should().Be(0x403000u);
    }
}
