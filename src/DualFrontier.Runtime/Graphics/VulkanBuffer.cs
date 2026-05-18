using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkBuffer + bound device memory wrapper. Allocates VkBuffer, queries memory requirements,
/// allocates region via <see cref="MemoryAllocator"/>, binds memory. Disposed releases the
/// VkBuffer (memory remains owned by the allocator; allocator.Reset/Dispose handles those).
/// </summary>
public sealed class VulkanBuffer : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _buffer;
    private MemoryAllocation _allocation;
    private bool _disposed;

    public IntPtr Handle => _buffer;
    public ulong Size { get; }
    public VkMemoryPropertyFlagsPublic MemoryProperties { get; }
    public MemoryAllocation Allocation => _allocation;

    internal VkBufferUsageFlags Usage { get; }

    public VulkanBuffer(
        VulkanDevice device,
        MemoryAllocator allocator,
        ulong size,
        VkMemoryPropertyFlagsPublic memoryProperties,
        VkBufferUsageFlagsPublic usage)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        if (size == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Buffer size must be > 0.");
        }

        _device = device.Handle;
        Size = size;
        MemoryProperties = memoryProperties;
        Usage = (VkBufferUsageFlags)(uint)usage;

        unsafe
        {
            var createInfo = new VkBufferCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                size = size,
                usage = Usage,
                sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE,
                queueFamilyIndexCount = 0,
                pQueueFamilyIndices = null,
            };

            VkResult result = VkApi.vkCreateBuffer(_device, in createInfo, IntPtr.Zero, out _buffer);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateBuffer failed: {result}");
            }
        }

        VkApi.vkGetBufferMemoryRequirements(_device, _buffer, out VkMemoryRequirements memReqs);
        _allocation = allocator.Allocate(memReqs.size, memReqs.alignment, (VkMemoryPropertyFlags)(uint)memoryProperties, memReqs.memoryTypeBits);
        VkResult bindResult = VkApi.vkBindBufferMemory(_device, _buffer, _allocation.DeviceMemory, _allocation.Offset);
        if (bindResult != VkResult.VK_SUCCESS)
        {
            // Best-effort cleanup of buffer; memory remains in allocator (no free list anyway).
            VkApi.vkDestroyBuffer(_device, _buffer, IntPtr.Zero);
            _buffer = IntPtr.Zero;
            throw new InvalidOperationException($"vkBindBufferMemory failed: {bindResult}");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_buffer != IntPtr.Zero)
        {
            VkApi.vkDestroyBuffer(_device, _buffer, IntPtr.Zero);
            _buffer = IntPtr.Zero;
        }
        _disposed = true;
    }
}

/// <summary>
/// Public-facing mirror of <c>VkBufferUsageFlags</c>. Mirrors values bit-for-bit so callers
/// don't have к pull in internal Vulkan enums. Cast at construction boundary inside
/// <see cref="VulkanBuffer"/>.
/// </summary>
[Flags]
public enum VkBufferUsageFlagsPublic : uint
{
    TransferSrc = 0x00000001,
    TransferDst = 0x00000002,
    UniformTexelBuffer = 0x00000004,
    StorageTexelBuffer = 0x00000008,
    UniformBuffer = 0x00000010,
    StorageBuffer = 0x00000020,
    IndexBuffer = 0x00000040,
    VertexBuffer = 0x00000080,
    IndirectBuffer = 0x00000100,
}
