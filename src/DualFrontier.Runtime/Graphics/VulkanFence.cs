using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>VkFence wrapper (CPU-GPU sync primitive). Initially unsignaled by default.</summary>
public sealed class VulkanFence : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _fence;
    private bool _disposed;

    public IntPtr Handle => _fence;

    public VulkanFence(VulkanDevice device, bool startSignaled = false)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;

        var createInfo = new VkFenceCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_FENCE_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = startSignaled ? VkFenceCreateFlags.VK_FENCE_CREATE_SIGNALED_BIT : 0,
        };
        VkResult result = VkApi.vkCreateFence(_device, in createInfo, IntPtr.Zero, out _fence);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateFence failed: {result}");
        }
    }

    public unsafe void Wait(ulong timeoutNs = ulong.MaxValue)
    {
        IntPtr fence = _fence;
        VkResult result = VkApi.vkWaitForFences(_device, 1, &fence, waitAll: 1, timeoutNs);
        DeviceLost.ThrowIfLost(result, new DeviceLostContext(VulkanCall.WaitForFences));
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkWaitForFences failed: {result}");
        }
    }

    public unsafe void Reset()
    {
        IntPtr fence = _fence;
        VkResult result = VkApi.vkResetFences(_device, 1, &fence);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkResetFences failed: {result}");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_fence != IntPtr.Zero)
        {
            VkApi.vkDestroyFence(_device, _fence, IntPtr.Zero);
            _fence = IntPtr.Zero;
        }
        _disposed = true;
    }
}
