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
    // V0.B Commit 7 — Surface + swapchain
    // ============================================================

    [Fact]
    public void VkSurfaceCapabilitiesKHR_Size_Matches_Spec()
    {
        // 2×uint32 (8) + 3×VkExtent2D (24) + 5×uint32 (20) = 52
        Marshal.SizeOf<VkSurfaceCapabilitiesKHR>().Should().Be(52);
    }

    [Fact]
    public void VkSurfaceFormatKHR_Size_Matches_Spec() => Marshal.SizeOf<VkSurfaceFormatKHR>().Should().Be(8);

    [Fact]
    public void VkSwapchainCreateInfoKHR_Size_Matches_Spec() => Marshal.SizeOf<VkSwapchainCreateInfoKHR>().Should().Be(104);

    [Fact]
    public void VkPresentInfoKHR_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + waitSemaphoreCount (4) + pad (4) + pWaitSemaphores (8)
        //   + swapchainCount (4) + pad (4) + pSwapchains (8) + pImageIndices (8) + pResults (8) = 64
        Marshal.SizeOf<VkPresentInfoKHR>().Should().Be(64);
    }

    // ============================================================
    // V0.B Commit 8 — Render pass + framebuffer
    // ============================================================

    [Fact]
    public void VkAttachmentDescription_Size_Matches_Spec() => Marshal.SizeOf<VkAttachmentDescription>().Should().Be(36);

    [Fact]
    public void VkAttachmentReference_Size_Matches_Spec() => Marshal.SizeOf<VkAttachmentReference>().Should().Be(8);

    [Fact]
    public void VkSubpassDescription_Size_Matches_Spec() => Marshal.SizeOf<VkSubpassDescription>().Should().Be(72);

    [Fact]
    public void VkSubpassDependency_Size_Matches_Spec() => Marshal.SizeOf<VkSubpassDependency>().Should().Be(28);

    [Fact]
    public void VkRenderPassCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkRenderPassCreateInfo>().Should().Be(64);

    [Fact]
    public void VkFramebufferCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkFramebufferCreateInfo>().Should().Be(64);

    // ============================================================
    // V0.B Commit 9 — Command pool + buffer + fence + semaphore
    // ============================================================

    [Fact]
    public void VkCommandPoolCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkCommandPoolCreateInfo>().Should().Be(24);

    [Fact]
    public void VkCommandBufferAllocateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkCommandBufferAllocateInfo>().Should().Be(32);

    [Fact]
    public void VkCommandBufferBeginInfo_Size_Matches_Spec() => Marshal.SizeOf<VkCommandBufferBeginInfo>().Should().Be(32);

    [Fact]
    public void VkClearColorValue_Size_Matches_Spec() => Marshal.SizeOf<VkClearColorValue>().Should().Be(16);

    [Fact]
    public void VkClearValue_Size_Matches_Spec() => Marshal.SizeOf<VkClearValue>().Should().Be(16);

    [Fact]
    public void VkRenderPassBeginInfo_Size_Matches_Spec() => Marshal.SizeOf<VkRenderPassBeginInfo>().Should().Be(64);

    [Fact]
    public void VkSubmitInfo_Size_Matches_Spec() => Marshal.SizeOf<VkSubmitInfo>().Should().Be(72);

    [Fact]
    public void VkFenceCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkFenceCreateInfo>().Should().Be(24);

    [Fact]
    public void VkSemaphoreCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkSemaphoreCreateInfo>().Should().Be(24);

    [Fact]
    public void VkViewport_Size_Matches_Spec() => Marshal.SizeOf<VkViewport>().Should().Be(24);

    // ============================================================
    // V0.B Commit 10 — Shader module + pipeline shader stage
    // ============================================================

    [Fact]
    public void VkShaderModuleCreateInfo_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + flags (4) + pad (4) + codeSize (8 nuint) + pCode (8) = 40
        Marshal.SizeOf<VkShaderModuleCreateInfo>().Should().Be(40);
    }

    [Fact]
    public void VkPipelineShaderStageCreateInfo_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + flags (4) + stage (4) + module (8) + pName (8) + pSpecializationInfo (8) = 48
        Marshal.SizeOf<VkPipelineShaderStageCreateInfo>().Should().Be(48);
    }

    // ============================================================
    // V0.B Commit 11 — Graphics pipeline
    // ============================================================

    [Fact]
    public void VkPipelineVertexInputStateCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineVertexInputStateCreateInfo>().Should().Be(48);

    [Fact]
    public void VkPipelineInputAssemblyStateCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineInputAssemblyStateCreateInfo>().Should().Be(32);

    [Fact]
    public void VkPipelineViewportStateCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineViewportStateCreateInfo>().Should().Be(48);

    [Fact]
    public void VkPipelineRasterizationStateCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineRasterizationStateCreateInfo>().Should().Be(64);

    [Fact]
    public void VkPipelineMultisampleStateCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineMultisampleStateCreateInfo>().Should().Be(48);

    [Fact]
    public void VkPipelineColorBlendAttachmentState_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineColorBlendAttachmentState>().Should().Be(32);

    [Fact]
    public void VkPipelineColorBlendStateCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineColorBlendStateCreateInfo>().Should().Be(56);

    [Fact]
    public void VkPipelineDynamicStateCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineDynamicStateCreateInfo>().Should().Be(32);

    [Fact]
    public void VkPipelineLayoutCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkPipelineLayoutCreateInfo>().Should().Be(48);

    [Fact]
    public void VkGraphicsPipelineCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkGraphicsPipelineCreateInfo>().Should().Be(144);

    // ============================================================
    // V0.B Commit 12 — Compute pipeline + descriptors
    // ============================================================

    [Fact]
    public void VkComputePipelineCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkComputePipelineCreateInfo>().Should().Be(96);

    [Fact]
    public void VkDescriptorSetLayoutBinding_Size_Matches_Spec() => Marshal.SizeOf<VkDescriptorSetLayoutBinding>().Should().Be(24);

    [Fact]
    public void VkDescriptorSetLayoutCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkDescriptorSetLayoutCreateInfo>().Should().Be(32);

    [Fact]
    public void VkDescriptorPoolSize_Size_Matches_Spec() => Marshal.SizeOf<VkDescriptorPoolSize>().Should().Be(8);

    [Fact]
    public void VkDescriptorPoolCreateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkDescriptorPoolCreateInfo>().Should().Be(40);

    [Fact]
    public void VkDescriptorSetAllocateInfo_Size_Matches_Spec() => Marshal.SizeOf<VkDescriptorSetAllocateInfo>().Should().Be(40);

    [Fact]
    public void VkDescriptorBufferInfo_Size_Matches_Spec() => Marshal.SizeOf<VkDescriptorBufferInfo>().Should().Be(24);

    [Fact]
    public void VkDescriptorImageInfo_Size_Matches_Spec() => Marshal.SizeOf<VkDescriptorImageInfo>().Should().Be(24);

    [Fact]
    public void VkWriteDescriptorSet_Size_Matches_Spec() => Marshal.SizeOf<VkWriteDescriptorSet>().Should().Be(64);

    // ============================================================
    // V0.C.1 — Sampler + texture upload + vertex input + push constants
    // ============================================================

    [Fact]
    public void VkSamplerCreateInfo_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + flags (4) + magFilter (4) + minFilter (4)
        //   + mipmapMode (4) + addressU (4) + addressV (4) + addressW (4)
        //   + mipLodBias (4) + anisotropyEnable (4) + maxAnisotropy (4) + compareEnable (4)
        //   + compareOp (4) + minLod (4) + maxLod (4) + borderColor (4) + unnormalizedCoords (4) = 80
        Marshal.SizeOf<VkSamplerCreateInfo>().Should().Be(80);
    }

    [Fact]
    public void VkPushConstantRange_Size_Matches_Spec() => Marshal.SizeOf<VkPushConstantRange>().Should().Be(12);

    [Fact]
    public void VkVertexInputBindingDescription_Size_Matches_Spec() => Marshal.SizeOf<VkVertexInputBindingDescription>().Should().Be(12);

    [Fact]
    public void VkVertexInputAttributeDescription_Size_Matches_Spec() => Marshal.SizeOf<VkVertexInputAttributeDescription>().Should().Be(16);

    [Fact]
    public void VkOffset3D_Size_Matches_Spec() => Marshal.SizeOf<VkOffset3D>().Should().Be(12);

    [Fact]
    public void VkImageSubresourceLayers_Size_Matches_Spec() => Marshal.SizeOf<VkImageSubresourceLayers>().Should().Be(16);

    [Fact]
    public void VkBufferImageCopy_Size_Matches_Spec()
    {
        // bufferOffset (8) + bufferRowLength (4) + bufferImageHeight (4)
        //   + imageSubresource (16) + imageOffset (12) + imageExtent (12) = 56
        Marshal.SizeOf<VkBufferImageCopy>().Should().Be(56);
    }

    [Fact]
    public void VkImageMemoryBarrier_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + srcAccess (4) + dstAccess (4) + oldLayout (4) + newLayout (4)
        //   + srcQueueFamilyIndex (4) + dstQueueFamilyIndex (4) + image (8) + subresourceRange (20)
        //   + trailing pad (4) = 72
        Marshal.SizeOf<VkImageMemoryBarrier>().Should().Be(72);
    }

    // ============================================================
    // V0.C.2 — Indexed draw enum
    // ============================================================

    [Fact]
    public unsafe void VkIndexType_Size_Matches_Spec()
    {
        // enum underlying type uint = 4 bytes. Marshal.SizeOf<T>() does not support enums
        // in .NET 8 ("cannot be marshaled as an unmanaged structure"); use sizeof() instead.
        sizeof(VkIndexType).Should().Be(4);
        Enum.GetUnderlyingType(typeof(VkIndexType)).Should().Be(typeof(uint));
    }
}
