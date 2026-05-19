using System.Numerics;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// VertexBufferRing tests per V0.C.2 S-LOCK-2 (Q1c ring buffer ratification).
/// GPU-backed: instantiates live VulkanDevice + MemoryAllocator per V0.C.1 SpriteRendererTests
/// precedent — managed-only mock would not exercise vkMapMemory/vkUnmapMemory contracts.
/// </summary>
public sealed class VertexBufferRingTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly MemoryAllocator _allocator;

    public VertexBufferRingTests()
    {
        var opts = new WindowOptions { Title = "VertexBufferRing", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _allocator = new MemoryAllocator(_device);
    }

    public void Dispose()
    {
        _device.WaitIdle();
        _allocator.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void Constructor_With_Valid_Args_Computes_ChunkSize_Correctly()
    {
        using var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 100);
        // chunkSize = 100 × 4 vertices × 20 bytes = 8000
        ring.ChunkSize.Should().Be(8000UL);
        ring.FrameCount.Should().Be(3);
        ring.MaxSpritesPerFrame.Should().Be(100);
        ring.Handle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Constructor_With_Zero_FrameCount_Throws()
    {
        Action act = () => new VertexBufferRing(_device, _allocator, frameCount: 0, maxSpritesPerFrame: 100);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_With_Zero_MaxSpritesPerFrame_Throws()
    {
        Action act = () => new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WriteSprite_Without_BeginFrame_Throws()
    {
        using var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 100);
        var v = new SpriteVertex(Vector2.Zero, Vector2.Zero, 0);
        Action act = () => ring.WriteSprite(v, v, v, v);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EndFrame_Without_BeginFrame_Throws()
    {
        using var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 100);
        Action act = () => ring.EndFrame();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void BeginFrame_Twice_Without_EndFrame_Throws()
    {
        using var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 100);
        ring.BeginFrame(0);
        try
        {
            Action act = () => ring.BeginFrame(1);
            act.Should().Throw<InvalidOperationException>();
        }
        finally
        {
            ring.EndFrame();
        }
    }

    [Fact]
    public void WriteSprite_Within_Capacity_Succeeds()
    {
        using var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 10);
        ring.BeginFrame(0);
        var v = new SpriteVertex(Vector2.Zero, Vector2.Zero, 0);
        for (int i = 0; i < 10; i++)
        {
            ring.WriteSprite(v, v, v, v);
        }
        ring.SpritesSubmittedThisFrame.Should().Be(10);
        ring.EndFrame();
    }

    [Fact]
    public void WriteSprite_Beyond_Capacity_Throws()
    {
        using var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 5);
        ring.BeginFrame(0);
        try
        {
            var v = new SpriteVertex(Vector2.Zero, Vector2.Zero, 0);
            for (int i = 0; i < 5; i++)
            {
                ring.WriteSprite(v, v, v, v);
            }
            Action act = () => ring.WriteSprite(v, v, v, v);
            act.Should().Throw<InvalidOperationException>();
        }
        finally
        {
            ring.EndFrame();
        }
    }

    [Fact]
    public void EndFrame_Returns_Correct_Chunk_Offset_For_Each_Frame()
    {
        using var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 10);

        ring.BeginFrame(0);
        ring.EndFrame().Should().Be(0UL);

        ring.BeginFrame(1);
        ring.EndFrame().Should().Be(ring.ChunkSize);

        ring.BeginFrame(2);
        ring.EndFrame().Should().Be(2 * ring.ChunkSize);

        // Frame 3 wraps к chunk 0 (modulo frameCount=3)
        ring.BeginFrame(3);
        ring.EndFrame().Should().Be(0UL);
    }

    [Fact]
    public void Use_After_Dispose_Throws()
    {
        var ring = new VertexBufferRing(_device, _allocator, frameCount: 3, maxSpritesPerFrame: 10);
        ring.Dispose();
        Action act = () => ring.BeginFrame(0);
        act.Should().Throw<ObjectDisposedException>();
    }
}
