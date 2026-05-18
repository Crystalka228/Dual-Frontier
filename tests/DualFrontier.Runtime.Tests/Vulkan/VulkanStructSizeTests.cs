using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Vulkan;

/// <summary>
/// Marshal.SizeOf verification tests per Lesson #7 strengthening (P/Invoke ABI alignment audit recipe).
/// Each new Vulkan struct gets size verification против Vulkan 1.3 spec sizeof on x64 MSVC ABI.
/// Caught alignment regressions early — V0.A executor surfaced VkPhysicalDeviceProperties
/// 816 → 824 bytes alignment fix; pattern inherited V0.B + future V briefs.
/// </summary>
public sealed class VulkanStructSizeTests
{
    // ============================================================
    // V0.A baseline structs (regression coverage)
    // ============================================================

    [Fact]
    public void VkApplicationInfo_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + pApplicationName (8) + appVersion (4) + pad (4)
        // + pEngineName (8) + engineVersion (4) + apiVersion (4) = 48 bytes на x64
        Marshal.SizeOf<VkApplicationInfo>().Should().Be(48);
    }

    [Fact]
    public void VkPhysicalDeviceProperties_Size_Matches_Spec()
    {
        // Per Vulkan 1.3 spec on x64: 824 bytes (V0.A alignment fix landed here).
        // Composition: header 20 + deviceName 256 + pipelineCacheUUID 16 + _padBeforeLimits 4
        //   + limits 504 + sparseProperties 20 + _padTrailing 4 = 824.
        Marshal.SizeOf<VkPhysicalDeviceProperties>().Should().Be(824);
    }

    // ============================================================
    // V0.B Commit 3 — Win32 surface foundation
    // ============================================================

    [Fact]
    public void VkWin32SurfaceCreateInfoKHR_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + flags (4) + pad (4) + hinstance (8) + hwnd (8) = 40
        Marshal.SizeOf<VkWin32SurfaceCreateInfoKHR>().Should().Be(40);
    }

    // ============================================================
    // V0.B Commit 6 — Memory + buffer + image
    // ============================================================

    [Fact]
    public void VkExtent2D_Size_Matches_Spec() => Marshal.SizeOf<VkExtent2D>().Should().Be(8);

    [Fact]
    public void VkExtent3D_Size_Matches_Spec() => Marshal.SizeOf<VkExtent3D>().Should().Be(12);

    [Fact]
    public void VkRect2D_Size_Matches_Spec() => Marshal.SizeOf<VkRect2D>().Should().Be(16);

    [Fact]
    public void VkComponentMapping_Size_Matches_Spec() => Marshal.SizeOf<VkComponentMapping>().Should().Be(16);

    [Fact]
    public void VkImageSubresourceRange_Size_Matches_Spec() => Marshal.SizeOf<VkImageSubresourceRange>().Should().Be(20);

    [Fact]
    public void VkMemoryType_Size_Matches_Spec() => Marshal.SizeOf<VkMemoryType>().Should().Be(8);

    [Fact]
    public void VkMemoryHeap_Size_Matches_Spec() => Marshal.SizeOf<VkMemoryHeap>().Should().Be(16);

    [Fact]
    public void VkMemoryRequirements_Size_Matches_Spec() => Marshal.SizeOf<VkMemoryRequirements>().Should().Be(24);

    [Fact]
    public void VkPhysicalDeviceMemoryProperties_Size_Matches_Spec()
    {
        // memoryTypeCount (4) + memoryTypes[32 × 8] (256) + memoryHeapCount (4) + memoryHeaps[16 × 16] (256) = 520
        Marshal.SizeOf<VkPhysicalDeviceMemoryProperties>().Should().Be(520);
    }

    [Fact]
    public void VkMemoryAllocateInfo_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + allocationSize (8) + memoryTypeIndex (4) + trailing pad (4) = 32
        Marshal.SizeOf<VkMemoryAllocateInfo>().Should().Be(32);
    }

    [Fact]
    public void VkBufferCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkBufferCreateInfo>().Should().Be(56);

    [Fact]
    public void VkImageCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkImageCreateInfo>().Should().Be(88);

    [Fact]
    public void VkImageViewCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkImageViewCreateInfo>().Should().Be(80);

    // ============================================================
    // V0.B additions appended per-commit (see brief §3 commits 7-12)
    // ============================================================
}
