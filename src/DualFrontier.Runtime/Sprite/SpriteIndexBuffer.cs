using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 pre-populated index buffer per S-LOCK-3 (Q2b indexed quad rendering ratification).
/// 6 indices per quad pattern (0,1,2,2,3,0) repeated quadCapacity times. Index type uint16.
///
/// Memory layout: 10,000 quads × 6 indices × 2 bytes = 120 KB. Buffer host-visible +
/// host-coherent for simplicity (constant after pre-population; никаких per-frame writes).
///
/// S-LOCK-3a hard cap: maxUint16Quads = 10,000 (60,000 indices &lt; 65,536 uint16 max).
/// Grids exceeding cap require multiple SpriteRenderer BeginFrame/EndFrame cycles per S-LOCK-5a.
/// </summary>
public sealed class SpriteIndexBuffer : IDisposable
{
    public const int IndicesPerQuad = 6;
    public const int MaxUint16Quads = 10_000;

    private readonly VulkanBuffer _buffer;
    private readonly int _quadCapacity;
    private bool _disposed;

    public IntPtr Handle => _buffer.Handle;
    public int QuadCapacity => _quadCapacity;
    public int IndexCount => _quadCapacity * IndicesPerQuad;

    public unsafe SpriteIndexBuffer(
        VulkanDevice device,
        MemoryAllocator allocator,
        int quadCapacity)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        if (quadCapacity <= 0 || quadCapacity > MaxUint16Quads)
        {
            throw new ArgumentOutOfRangeException(nameof(quadCapacity),
                $"Quad capacity must be 1..{MaxUint16Quads} (uint16 index range constraint).");
        }

        _quadCapacity = quadCapacity;
        ulong totalBytes = (ulong)quadCapacity * (ulong)IndicesPerQuad * sizeof(ushort);

        _buffer = new VulkanBuffer(
            device, allocator, totalBytes,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent,
            VkBufferUsageFlagsPublic.IndexBuffer);

        // Pre-populate index pattern (0,1,2,2,3,0) per quad.
        VkResult mapResult = VkApi.vkMapMemory(
            device.Handle,
            _buffer.Allocation.DeviceMemory,
            _buffer.Allocation.Offset,
            totalBytes,
            0,
            out IntPtr mappedPtr);
        if (mapResult != VkResult.VK_SUCCESS)
        {
            _buffer.Dispose();
            throw new InvalidOperationException($"vkMapMemory (index buffer pre-pop) failed: {mapResult}");
        }

        try
        {
            ushort* indices = (ushort*)mappedPtr;
            for (int q = 0; q < quadCapacity; q++)
            {
                int baseVertex = q * 4;
                int baseIndex = q * IndicesPerQuad;
                indices[baseIndex + 0] = (ushort)(baseVertex + 0);  // TL
                indices[baseIndex + 1] = (ushort)(baseVertex + 1);  // TR
                indices[baseIndex + 2] = (ushort)(baseVertex + 2);  // BR
                indices[baseIndex + 3] = (ushort)(baseVertex + 2);  // BR
                indices[baseIndex + 4] = (ushort)(baseVertex + 3);  // BL
                indices[baseIndex + 5] = (ushort)(baseVertex + 0);  // TL
            }
        }
        finally
        {
            VkApi.vkUnmapMemory(device.Handle, _buffer.Allocation.DeviceMemory);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _buffer.Dispose();
        _disposed = true;
    }
}
