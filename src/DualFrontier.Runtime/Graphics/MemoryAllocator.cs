using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>Public-facing mirror of VkMemoryPropertyFlags. Bit-for-bit cast.</summary>
[Flags]
public enum VkMemoryPropertyFlagsPublic : uint
{
    DeviceLocal = 0x00000001,
    HostVisible = 0x00000002,
    HostCoherent = 0x00000004,
    HostCached = 0x00000008,
    LazilyAllocated = 0x00000010,
}

/// <summary>
/// V0.B bumper linear allocator (per S-LOCK-4 of V0.B brief). One VkDeviceMemory block per
/// memory type; allocations bump pointer forward — no free list. Allocator disposed releases
/// all blocks; <see cref="Reset"/> rewinds offsets к zero for reuse cycle (e.g., swapchain
/// recreation). Free-list / pool allocator deferred к V0.C or later.
/// </summary>
public sealed class MemoryAllocator : IDisposable
{
    /// <summary>Default per-memory-type block size (64 MB). Sufficient для V0.B exit criteria.</summary>
    private const ulong DefaultBlockSize = 64UL * 1024UL * 1024UL;

    private readonly IntPtr _device;
    private readonly IntPtr _physicalDevice;
    private readonly Dictionary<uint, MemoryBlock> _blocks = new();
    private bool _disposed;

    public MemoryAllocator(VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;
        _physicalDevice = device.PhysicalDevice;
    }

    /// <summary>
    /// Allocate a region of <paramref name="size"/> bytes с specified <paramref name="alignment"/>
    /// + memory property requirements. Returns the allocation descriptor that the caller binds
    /// to a buffer/image via <c>vkBindBufferMemory</c> / <c>vkBindImageMemory</c>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Block exhausted (V0.B bumper allocator does
    /// not have a free list — caller must Reset or wait для V0.C/free-list upgrade).</exception>
    public MemoryAllocation Allocate(
        ulong size,
        ulong alignment,
        VkMemoryPropertyFlagsPublic requiredProperties,
        uint memoryTypeBits)
    {
        return Allocate(size, alignment, (VkMemoryPropertyFlags)(uint)requiredProperties, memoryTypeBits);
    }

    internal MemoryAllocation Allocate(
        ulong size,
        ulong alignment,
        VkMemoryPropertyFlags requiredProperties,
        uint memoryTypeBits)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        uint memoryTypeIndex = FindMemoryType(memoryTypeBits, requiredProperties);

        if (!_blocks.TryGetValue(memoryTypeIndex, out MemoryBlock block))
        {
            var allocInfo = new VkMemoryAllocateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
                pNext = IntPtr.Zero,
                allocationSize = DefaultBlockSize,
                memoryTypeIndex = memoryTypeIndex,
            };
            VkResult result = VkApi.vkAllocateMemory(_device, in allocInfo, IntPtr.Zero, out IntPtr memory);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException(
                    $"vkAllocateMemory failed: {result} (size={DefaultBlockSize}, memoryTypeIndex={memoryTypeIndex})");
            }
            block = new MemoryBlock(memory, DefaultBlockSize, 0);
        }

        ulong alignedOffset = AlignUp(block.UsedBytes, alignment);
        ulong newUsedBytes = alignedOffset + size;
        if (newUsedBytes > block.Capacity)
        {
            throw new InvalidOperationException(
                $"MemoryAllocator bumper exhausted (memoryTypeIndex={memoryTypeIndex}, " +
                $"requested {size} bytes aligned {alignment}, available {block.Capacity - alignedOffset}). " +
                "V0.B bumper does not support free list; Reset() or wait для V0.C/free-list upgrade.");
        }

        _blocks[memoryTypeIndex] = block with { UsedBytes = newUsedBytes };
        return new MemoryAllocation(block.DeviceMemory, alignedOffset, size, memoryTypeIndex);
    }

    /// <summary>
    /// Rewind all bumper offsets to zero. Caller MUST ensure no buffer/image still references
    /// this memory — typically called после <c>vkDeviceWaitIdle</c> before swapchain recreation.
    /// </summary>
    public void Reset()
    {
        foreach ((uint key, MemoryBlock block) in _blocks.ToList())
        {
            _blocks[key] = block with { UsedBytes = 0 };
        }
    }

    private uint FindMemoryType(uint typeBits, VkMemoryPropertyFlags requiredProperties)
    {
        VkApi.vkGetPhysicalDeviceMemoryProperties(_physicalDevice, out VkPhysicalDeviceMemoryProperties memProps);
        for (uint i = 0; i < memProps.memoryTypeCount; i++)
        {
            bool typeOk = (typeBits & (1u << (int)i)) != 0;
            VkMemoryType memType = memProps.GetMemoryType((int)i);
            bool propsOk = (memType.propertyFlags & requiredProperties) == requiredProperties;
            if (typeOk && propsOk)
            {
                return i;
            }
        }
        throw new InvalidOperationException(
            $"No suitable memory type found (typeBits=0x{typeBits:X}, requiredProperties={requiredProperties})");
    }

    private static ulong AlignUp(ulong value, ulong alignment)
    {
        if (alignment == 0)
        {
            return value;
        }
        return (value + alignment - 1) & ~(alignment - 1);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        foreach ((_, MemoryBlock block) in _blocks)
        {
            VkApi.vkFreeMemory(_device, block.DeviceMemory, IntPtr.Zero);
        }
        _blocks.Clear();
        _disposed = true;
    }
}

/// <summary>
/// Allocation descriptor returned by <c>MemoryAllocator.Allocate</c>. Carries the
/// underlying <c>VkDeviceMemory</c> block handle + offset within the block — caller binds
/// via <c>vkBindBufferMemory</c> / <c>vkBindImageMemory</c>.
/// </summary>
public readonly record struct MemoryAllocation(IntPtr DeviceMemory, ulong Offset, ulong Size, uint MemoryTypeIndex);

internal record struct MemoryBlock(IntPtr DeviceMemory, ulong Capacity, ulong UsedBytes);
