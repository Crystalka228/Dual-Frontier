using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// SpriteIndexBuffer tests per V0.C.2 S-LOCK-3 (Q2b indexed quad rendering ratification).
/// </summary>
public sealed class SpriteIndexBufferTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly MemoryAllocator _allocator;

    public SpriteIndexBufferTests()
    {
        var opts = new WindowOptions { Title = "SpriteIndexBuffer", Width = 400, Height = 300 };
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
    public void Constants_Are_Expected_Values()
    {
        SpriteIndexBuffer.IndicesPerQuad.Should().Be(6);
        SpriteIndexBuffer.MaxUint16Quads.Should().Be(10_000);
    }

    [Fact]
    public void Constructor_With_Valid_Capacity_Succeeds()
    {
        using var idx = new SpriteIndexBuffer(_device, _allocator, quadCapacity: 100);
        idx.QuadCapacity.Should().Be(100);
        idx.IndexCount.Should().Be(600);
        idx.Handle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Constructor_With_Zero_Capacity_Throws()
    {
        Action act = () => new SpriteIndexBuffer(_device, _allocator, quadCapacity: 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_With_Negative_Capacity_Throws()
    {
        Action act = () => new SpriteIndexBuffer(_device, _allocator, quadCapacity: -1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_Beyond_MaxUint16Quads_Throws()
    {
        Action act = () => new SpriteIndexBuffer(_device, _allocator,
            quadCapacity: SpriteIndexBuffer.MaxUint16Quads + 1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_At_MaxUint16Quads_Succeeds()
    {
        using var idx = new SpriteIndexBuffer(_device, _allocator,
            quadCapacity: SpriteIndexBuffer.MaxUint16Quads);
        idx.QuadCapacity.Should().Be(10_000);
        idx.IndexCount.Should().Be(60_000);
    }

    [Fact]
    public void IndexCount_Equals_QuadCapacity_Times_Six()
    {
        using var idx = new SpriteIndexBuffer(_device, _allocator, quadCapacity: 1234);
        idx.IndexCount.Should().Be(1234 * 6);
    }
}
