using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 N-frame ring buffer per S-LOCK-2 (Q1c ratification).
/// Per-frame chunk size = maxSpritesPerFrame × 4 vertices × sizeof(SpriteVertex 20 bytes).
/// Total ring size = chunkSize × frameCount; typical frameCount = swapchain image count (2-3).
/// Each frame writes own chunk avoiding cross-frame synchronization hazards.
///
/// Host-visible + host-coherent memory per V0.B MemoryAllocator bumper pattern. Caller must
/// fence-synchronize per V0.B imageAvailable + V0.C.1 per-image renderFinished — frame N+frameCount
/// write must NOT collide с frame N's GPU consumption.
/// </summary>
public sealed class VertexBufferRing : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly VulkanBuffer _buffer;
    private readonly int _frameCount;
    private readonly int _maxSpritesPerFrame;
    private readonly ulong _chunkSize;
    private uint _activeFrame;
    private uint _lastBegunFrameIndex = uint.MaxValue;
    private IntPtr _mappedPtr;
    private ulong _writeOffset;
    private int _spritesSubmittedThisFrame;
    private bool _disposed;

    /// <summary>Per-frame chunk size в bytes (= maxSpritesPerFrame × 4 × 20).</summary>
    public ulong ChunkSize => _chunkSize;

    /// <summary>Number of ring slots (typically swapchain image count).</summary>
    public int FrameCount => _frameCount;

    /// <summary>Maximum sprites that can be written в a single frame chunk.</summary>
    public int MaxSpritesPerFrame => _maxSpritesPerFrame;

    /// <summary>Underlying VkBuffer handle. Caller uses с vkCmdBindVertexBuffers.</summary>
    public IntPtr Handle => _buffer.Handle;

    /// <summary>Number of sprites written в current frame (resets at BeginFrame).</summary>
    public int SpritesSubmittedThisFrame => _spritesSubmittedThisFrame;

    public VertexBufferRing(
        VulkanDevice device,
        MemoryAllocator allocator,
        int frameCount,
        int maxSpritesPerFrame)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        if (frameCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(frameCount), "Frame count must be positive.");
        }
        if (maxSpritesPerFrame <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSpritesPerFrame), "Max sprites per frame must be positive.");
        }

        _device = device;
        _frameCount = frameCount;
        _maxSpritesPerFrame = maxSpritesPerFrame;
        // 4 vertices per sprite × 20 bytes per SpriteVertex.
        _chunkSize = (ulong)maxSpritesPerFrame * 4UL * 20UL;
        ulong totalSize = _chunkSize * (ulong)frameCount;

        _buffer = new VulkanBuffer(
            device, allocator, totalSize,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent,
            VkBufferUsageFlagsPublic.VertexBuffer);
    }

    /// <summary>
    /// Begin writing к chunk N (frameIndex modulo frameCount). Maps memory at chunk offset.
    /// </summary>
    public void BeginFrame(uint frameIndex)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_mappedPtr != IntPtr.Zero)
        {
            throw new InvalidOperationException("VertexBufferRing.BeginFrame called twice without EndFrame.");
        }

        // F02 guard: each ring chunk is selected purely by frameIndex % frameCount and is
        // sized for exactly one batch (maxSpritesPerFrame). Beginning two consecutive frames
        // on the same frameIndex — the ">10K sprites → multiple BeginFrame/EndFrame cycles"
        // path — remaps the same chunk and rewrites from offset 0, silently overwriting the
        // prior batch's vertices before its draw is submitted. Fail fast instead of corrupting:
        // the caller must raise maxSpritesPerFrame to fit the scene in one batch, or submit +
        // present (advancing the swapchain image) between batches.
        if (frameIndex == _lastBegunFrameIndex)
        {
            throw new InvalidOperationException(
                $"VertexBufferRing.BeginFrame reused ring slot {frameIndex % (uint)_frameCount} for two " +
                "consecutive frames without the swapchain image advancing; a second sprite batch would " +
                "overwrite the first batch's vertices before submission (F02). Raise maxSpritesPerFrame to " +
                "fit the scene in one batch, or submit + present between batches.");
        }

        _activeFrame = frameIndex % (uint)_frameCount;
        ulong chunkOffset = (ulong)_activeFrame * _chunkSize;

        VkResult result = VkApi.vkMapMemory(
            _device.Handle,
            _buffer.Allocation.DeviceMemory,
            _buffer.Allocation.Offset + chunkOffset,
            _chunkSize,
            0,
            out _mappedPtr);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkMapMemory (vertex ring) failed: {result}");
        }

        _writeOffset = 0;
        _spritesSubmittedThisFrame = 0;
        _lastBegunFrameIndex = frameIndex;
    }

    /// <summary>
    /// Write 4 vertices к current frame chunk. Vertex order must match index pattern (0,1,2,2,3,0):
    /// TL → TR → BR → BL.
    /// </summary>
    public unsafe void WriteSprite(in SpriteVertex tl, in SpriteVertex tr, in SpriteVertex br, in SpriteVertex bl)
    {
        if (_mappedPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("VertexBufferRing.WriteSprite without BeginFrame.");
        }
        if (_spritesSubmittedThisFrame >= _maxSpritesPerFrame)
        {
            throw new InvalidOperationException(
                $"VertexBufferRing capacity exceeded ({_maxSpritesPerFrame} sprites per frame).");
        }

        byte* dst = (byte*)_mappedPtr + _writeOffset;
        *(SpriteVertex*)(dst + 0) = tl;
        *(SpriteVertex*)(dst + 20) = tr;
        *(SpriteVertex*)(dst + 40) = br;
        *(SpriteVertex*)(dst + 60) = bl;

        _writeOffset += 80;
        _spritesSubmittedThisFrame++;
    }

    /// <summary>
    /// Unmap memory + return current frame's chunk offset for vkCmdBindVertexBuffers.
    /// </summary>
    public ulong EndFrame()
    {
        if (_mappedPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("VertexBufferRing.EndFrame without BeginFrame.");
        }

        VkApi.vkUnmapMemory(_device.Handle, _buffer.Allocation.DeviceMemory);
        _mappedPtr = IntPtr.Zero;

        return (ulong)_activeFrame * _chunkSize;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_mappedPtr != IntPtr.Zero)
        {
            VkApi.vkUnmapMemory(_device.Handle, _buffer.Allocation.DeviceMemory);
            _mappedPtr = IntPtr.Zero;
        }
        _buffer.Dispose();
        _disposed = true;
    }
}
