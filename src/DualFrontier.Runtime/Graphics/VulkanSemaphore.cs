using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>VkSemaphore wrapper (GPU-GPU sync primitive). Initially unsignaled.</summary>
public sealed class VulkanSemaphore : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _semaphore;
    private bool _disposed;

    public IntPtr Handle => _semaphore;

    public VulkanSemaphore(VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;

        var createInfo = new VkSemaphoreCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
        };
        VkResult result = VkApi.vkCreateSemaphore(_device, in createInfo, IntPtr.Zero, out _semaphore);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateSemaphore failed: {result}");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_semaphore != IntPtr.Zero)
        {
            VkApi.vkDestroySemaphore(_device, _semaphore, IntPtr.Zero);
            _semaphore = IntPtr.Zero;
        }
        _disposed = true;
    }
}
