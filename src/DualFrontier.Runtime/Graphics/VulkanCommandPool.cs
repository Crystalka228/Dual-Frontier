using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkCommandPool wrapper. V0.B configures RESET_COMMAND_BUFFER flag so each VulkanCommandBuffer
/// can be reset individually (per-frame command buffer pattern). Pool bound к specific queue
/// family at construction.
/// </summary>
public sealed class VulkanCommandPool : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _commandPool;
    private bool _disposed;

    public IntPtr Handle => _commandPool;
    public uint QueueFamilyIndex { get; }

    public VulkanCommandPool(VulkanDevice device, uint queueFamilyIndex)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;
        QueueFamilyIndex = queueFamilyIndex;

        var createInfo = new VkCommandPoolCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = VkCommandPoolCreateFlags.VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT,
            queueFamilyIndex = queueFamilyIndex,
        };

        VkResult result = VkApi.vkCreateCommandPool(_device, in createInfo, IntPtr.Zero, out _commandPool);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateCommandPool failed: {result}");
        }
    }

    public VulkanCommandBuffer AllocateBuffer()
    {
        return new VulkanCommandBuffer(this, _device);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_commandPool != IntPtr.Zero)
        {
            VkApi.vkDestroyCommandPool(_device, _commandPool, IntPtr.Zero);
            _commandPool = IntPtr.Zero;
        }
        _disposed = true;
    }
}
