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
    // V0.B additions appended per-commit (see brief §3 commits 3-12)
    // ============================================================
}
