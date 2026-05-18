using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

internal static unsafe partial class VkApi
{
    private const string VulkanLib = "vulkan-1.dll";

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateInstance")]
    internal static partial VkResult vkCreateInstance(
        in VkInstanceCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pInstance);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyInstance")]
    internal static partial void vkDestroyInstance(IntPtr instance, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkEnumerateInstanceVersion")]
    internal static partial VkResult vkEnumerateInstanceVersion(out uint pApiVersion);

    [LibraryImport(VulkanLib, EntryPoint = "vkEnumeratePhysicalDevices")]
    internal static partial VkResult vkEnumeratePhysicalDevices(
        IntPtr instance,
        ref uint pPhysicalDeviceCount,
        IntPtr* pPhysicalDevices);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceProperties")]
    internal static partial void vkGetPhysicalDeviceProperties(
        IntPtr physicalDevice,
        out VkPhysicalDeviceProperties pProperties);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceQueueFamilyProperties")]
    internal static partial void vkGetPhysicalDeviceQueueFamilyProperties(
        IntPtr physicalDevice,
        ref uint pQueueFamilyPropertyCount,
        VkQueueFamilyProperties* pQueueFamilyProperties);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateDevice")]
    internal static partial VkResult vkCreateDevice(
        IntPtr physicalDevice,
        in VkDeviceCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pDevice);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyDevice")]
    internal static partial void vkDestroyDevice(IntPtr device, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetDeviceQueue")]
    internal static partial void vkGetDeviceQueue(
        IntPtr device,
        uint queueFamilyIndex,
        uint queueIndex,
        out IntPtr pQueue);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetInstanceProcAddr",
        StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr vkGetInstanceProcAddr(IntPtr instance, string pName);

    // Note: vkCreateDebugUtilsMessengerEXT / vkDestroyDebugUtilsMessengerEXT are extension
    // functions — not direct exports of vulkan-1.dll. Loaded at runtime via
    // vkGetInstanceProcAddr by ValidationLayer (Commit 7).

    // =======================================================================
    // V0.B Commit 6 — Memory + buffer + image
    // =======================================================================

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceMemoryProperties")]
    internal static partial void vkGetPhysicalDeviceMemoryProperties(
        IntPtr physicalDevice,
        out VkPhysicalDeviceMemoryProperties pMemoryProperties);

    [LibraryImport(VulkanLib, EntryPoint = "vkAllocateMemory")]
    internal static partial VkResult vkAllocateMemory(
        IntPtr device,
        in VkMemoryAllocateInfo pAllocateInfo,
        IntPtr pAllocator,
        out IntPtr pMemory);

    [LibraryImport(VulkanLib, EntryPoint = "vkFreeMemory")]
    internal static partial void vkFreeMemory(IntPtr device, IntPtr memory, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkMapMemory")]
    internal static partial VkResult vkMapMemory(
        IntPtr device,
        IntPtr memory,
        ulong offset,
        ulong size,
        uint flags,
        out IntPtr ppData);

    [LibraryImport(VulkanLib, EntryPoint = "vkUnmapMemory")]
    internal static partial void vkUnmapMemory(IntPtr device, IntPtr memory);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateBuffer")]
    internal static partial VkResult vkCreateBuffer(
        IntPtr device,
        in VkBufferCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pBuffer);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyBuffer")]
    internal static partial void vkDestroyBuffer(IntPtr device, IntPtr buffer, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetBufferMemoryRequirements")]
    internal static partial void vkGetBufferMemoryRequirements(
        IntPtr device,
        IntPtr buffer,
        out VkMemoryRequirements pMemoryRequirements);

    [LibraryImport(VulkanLib, EntryPoint = "vkBindBufferMemory")]
    internal static partial VkResult vkBindBufferMemory(
        IntPtr device,
        IntPtr buffer,
        IntPtr memory,
        ulong memoryOffset);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateImage")]
    internal static partial VkResult vkCreateImage(
        IntPtr device,
        in VkImageCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pImage);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyImage")]
    internal static partial void vkDestroyImage(IntPtr device, IntPtr image, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetImageMemoryRequirements")]
    internal static partial void vkGetImageMemoryRequirements(
        IntPtr device,
        IntPtr image,
        out VkMemoryRequirements pMemoryRequirements);

    [LibraryImport(VulkanLib, EntryPoint = "vkBindImageMemory")]
    internal static partial VkResult vkBindImageMemory(
        IntPtr device,
        IntPtr image,
        IntPtr memory,
        ulong memoryOffset);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateImageView")]
    internal static partial VkResult vkCreateImageView(
        IntPtr device,
        in VkImageViewCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pView);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyImageView")]
    internal static partial void vkDestroyImageView(IntPtr device, IntPtr imageView, IntPtr pAllocator);

    // =======================================================================
    // V0.B Commit 7 — Surface + swapchain
    //
    // KHR functions exported by vulkan-1.dll loader trampoline on Windows; use [LibraryImport]
    // directly. Function pointers acquired при first call would also work via vkGetInstanceProcAddr
    // / vkGetDeviceProcAddr per Vulkan loader contract, but direct binding matches V0.A core
    // function approach (simpler + statically verifiable).
    // =======================================================================

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateWin32SurfaceKHR")]
    internal static partial VkResult vkCreateWin32SurfaceKHR(
        IntPtr instance,
        in VkWin32SurfaceCreateInfoKHR pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pSurface);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroySurfaceKHR")]
    internal static partial void vkDestroySurfaceKHR(IntPtr instance, IntPtr surface, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceSurfaceSupportKHR")]
    internal static partial VkResult vkGetPhysicalDeviceSurfaceSupportKHR(
        IntPtr physicalDevice,
        uint queueFamilyIndex,
        IntPtr surface,
        out uint pSupported);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceSurfaceCapabilitiesKHR")]
    internal static partial VkResult vkGetPhysicalDeviceSurfaceCapabilitiesKHR(
        IntPtr physicalDevice,
        IntPtr surface,
        out VkSurfaceCapabilitiesKHR pSurfaceCapabilities);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceSurfaceFormatsKHR")]
    internal static unsafe partial VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(
        IntPtr physicalDevice,
        IntPtr surface,
        ref uint pSurfaceFormatCount,
        VkSurfaceFormatKHR* pSurfaceFormats);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetPhysicalDeviceSurfacePresentModesKHR")]
    internal static unsafe partial VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(
        IntPtr physicalDevice,
        IntPtr surface,
        ref uint pPresentModeCount,
        VkPresentModeKHR* pPresentModes);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateSwapchainKHR")]
    internal static partial VkResult vkCreateSwapchainKHR(
        IntPtr device,
        in VkSwapchainCreateInfoKHR pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pSwapchain);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroySwapchainKHR")]
    internal static partial void vkDestroySwapchainKHR(IntPtr device, IntPtr swapchain, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkGetSwapchainImagesKHR")]
    internal static unsafe partial VkResult vkGetSwapchainImagesKHR(
        IntPtr device,
        IntPtr swapchain,
        ref uint pSwapchainImageCount,
        IntPtr* pSwapchainImages);

    [LibraryImport(VulkanLib, EntryPoint = "vkAcquireNextImageKHR")]
    internal static partial VkResult vkAcquireNextImageKHR(
        IntPtr device,
        IntPtr swapchain,
        ulong timeout,
        IntPtr semaphore,
        IntPtr fence,
        out uint pImageIndex);

    [LibraryImport(VulkanLib, EntryPoint = "vkQueuePresentKHR")]
    internal static partial VkResult vkQueuePresentKHR(IntPtr queue, in VkPresentInfoKHR pPresentInfo);

    [LibraryImport(VulkanLib, EntryPoint = "vkDeviceWaitIdle")]
    internal static partial VkResult vkDeviceWaitIdle(IntPtr device);

    [LibraryImport(VulkanLib, EntryPoint = "vkQueueWaitIdle")]
    internal static partial VkResult vkQueueWaitIdle(IntPtr queue);

    // =======================================================================
    // V0.B Commit 8 — Render pass + framebuffer
    // =======================================================================

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateRenderPass")]
    internal static partial VkResult vkCreateRenderPass(
        IntPtr device,
        in VkRenderPassCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pRenderPass);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyRenderPass")]
    internal static partial void vkDestroyRenderPass(IntPtr device, IntPtr renderPass, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateFramebuffer")]
    internal static partial VkResult vkCreateFramebuffer(
        IntPtr device,
        in VkFramebufferCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pFramebuffer);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyFramebuffer")]
    internal static partial void vkDestroyFramebuffer(IntPtr device, IntPtr framebuffer, IntPtr pAllocator);

    // =======================================================================
    // V0.B Commit 9 — Command pool + buffer + fence + semaphore
    // =======================================================================

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateCommandPool")]
    internal static partial VkResult vkCreateCommandPool(
        IntPtr device,
        in VkCommandPoolCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pCommandPool);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyCommandPool")]
    internal static partial void vkDestroyCommandPool(IntPtr device, IntPtr commandPool, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkAllocateCommandBuffers")]
    internal static unsafe partial VkResult vkAllocateCommandBuffers(
        IntPtr device,
        in VkCommandBufferAllocateInfo pAllocateInfo,
        IntPtr* pCommandBuffers);

    [LibraryImport(VulkanLib, EntryPoint = "vkFreeCommandBuffers")]
    internal static unsafe partial void vkFreeCommandBuffers(
        IntPtr device,
        IntPtr commandPool,
        uint commandBufferCount,
        IntPtr* pCommandBuffers);

    [LibraryImport(VulkanLib, EntryPoint = "vkBeginCommandBuffer")]
    internal static partial VkResult vkBeginCommandBuffer(
        IntPtr commandBuffer,
        in VkCommandBufferBeginInfo pBeginInfo);

    [LibraryImport(VulkanLib, EntryPoint = "vkEndCommandBuffer")]
    internal static partial VkResult vkEndCommandBuffer(IntPtr commandBuffer);

    [LibraryImport(VulkanLib, EntryPoint = "vkResetCommandBuffer")]
    internal static partial VkResult vkResetCommandBuffer(IntPtr commandBuffer, uint flags);

    [LibraryImport(VulkanLib, EntryPoint = "vkCmdBeginRenderPass")]
    internal static partial void vkCmdBeginRenderPass(
        IntPtr commandBuffer,
        in VkRenderPassBeginInfo pRenderPassBegin,
        VkSubpassContents contents);

    [LibraryImport(VulkanLib, EntryPoint = "vkCmdEndRenderPass")]
    internal static partial void vkCmdEndRenderPass(IntPtr commandBuffer);

    [LibraryImport(VulkanLib, EntryPoint = "vkCmdBindPipeline")]
    internal static partial void vkCmdBindPipeline(
        IntPtr commandBuffer,
        VkPipelineBindPoint pipelineBindPoint,
        IntPtr pipeline);

    [LibraryImport(VulkanLib, EntryPoint = "vkCmdDraw")]
    internal static partial void vkCmdDraw(
        IntPtr commandBuffer,
        uint vertexCount,
        uint instanceCount,
        uint firstVertex,
        uint firstInstance);

    [LibraryImport(VulkanLib, EntryPoint = "vkCmdDispatch")]
    internal static partial void vkCmdDispatch(
        IntPtr commandBuffer,
        uint groupCountX,
        uint groupCountY,
        uint groupCountZ);

    [LibraryImport(VulkanLib, EntryPoint = "vkCmdSetViewport")]
    internal static unsafe partial void vkCmdSetViewport(
        IntPtr commandBuffer,
        uint firstViewport,
        uint viewportCount,
        VkViewport* pViewports);

    [LibraryImport(VulkanLib, EntryPoint = "vkCmdSetScissor")]
    internal static unsafe partial void vkCmdSetScissor(
        IntPtr commandBuffer,
        uint firstScissor,
        uint scissorCount,
        VkRect2D* pScissors);

    [LibraryImport(VulkanLib, EntryPoint = "vkQueueSubmit")]
    internal static partial VkResult vkQueueSubmit(
        IntPtr queue,
        uint submitCount,
        in VkSubmitInfo pSubmits,
        IntPtr fence);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateFence")]
    internal static partial VkResult vkCreateFence(
        IntPtr device,
        in VkFenceCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pFence);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroyFence")]
    internal static partial void vkDestroyFence(IntPtr device, IntPtr fence, IntPtr pAllocator);

    [LibraryImport(VulkanLib, EntryPoint = "vkWaitForFences")]
    internal static unsafe partial VkResult vkWaitForFences(
        IntPtr device,
        uint fenceCount,
        IntPtr* pFences,
        uint waitAll,
        ulong timeout);

    [LibraryImport(VulkanLib, EntryPoint = "vkResetFences")]
    internal static unsafe partial VkResult vkResetFences(IntPtr device, uint fenceCount, IntPtr* pFences);

    [LibraryImport(VulkanLib, EntryPoint = "vkCreateSemaphore")]
    internal static partial VkResult vkCreateSemaphore(
        IntPtr device,
        in VkSemaphoreCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pSemaphore);

    [LibraryImport(VulkanLib, EntryPoint = "vkDestroySemaphore")]
    internal static partial void vkDestroySemaphore(IntPtr device, IntPtr semaphore, IntPtr pAllocator);
}
