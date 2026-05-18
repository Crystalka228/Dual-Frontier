using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// V0.B bumper allocator semantics — verified against real Vulkan device на test hardware.
/// Mocked-only unit tests not viable because allocator depends on
/// <c>vkGetPhysicalDeviceMemoryProperties</c> + <c>vkAllocateMemory</c> driver calls.
/// </summary>
public sealed class MemoryAllocatorTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;

    public MemoryAllocatorTests()
    {
        var opts = new WindowOptions { Title = "MemAlloc", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
    }

    public void Dispose()
    {
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void Allocate_returns_allocation_with_aligned_offset_zero_on_first_call()
    {
        using var allocator = new MemoryAllocator(_device);

        MemoryAllocation alloc = allocator.Allocate(
            size: 1024,
            alignment: 256,
            requiredProperties: VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent,
            memoryTypeBits: uint.MaxValue);

        alloc.Size.Should().Be(1024UL);
        alloc.Offset.Should().Be(0UL);
        alloc.DeviceMemory.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Allocate_bumps_offset_aligned_to_requested_alignment()
    {
        using var allocator = new MemoryAllocator(_device);
        var props = VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent;

        MemoryAllocation a = allocator.Allocate(100, 256, props, uint.MaxValue);
        MemoryAllocation b = allocator.Allocate(100, 256, props, uint.MaxValue);

        a.Offset.Should().Be(0UL);
        b.Offset.Should().Be(256UL, "bumper aligns next allocation к requested alignment from previous end (100)");
    }

    [Fact]
    public void Reset_rewinds_offset_to_zero()
    {
        using var allocator = new MemoryAllocator(_device);
        var props = VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent;

        allocator.Allocate(1024, 1, props, uint.MaxValue);
        allocator.Reset();
        MemoryAllocation after = allocator.Allocate(512, 1, props, uint.MaxValue);

        after.Offset.Should().Be(0UL);
    }

    [Fact]
    public void Dispose_does_not_throw()
    {
        var allocator = new MemoryAllocator(_device);
        allocator.Dispose();
        var act = () => allocator.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Use_after_dispose_throws_object_disposed()
    {
        var allocator = new MemoryAllocator(_device);
        allocator.Dispose();
        var act = () => allocator.Allocate(100, 1, VkMemoryPropertyFlagsPublic.HostVisible, uint.MaxValue);
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void VulkanBuffer_with_storage_buffer_usage_constructs_and_binds_memory()
    {
        using var allocator = new MemoryAllocator(_device);
        using var buffer = new VulkanBuffer(
            _device,
            allocator,
            size: 4096,
            memoryProperties: VkMemoryPropertyFlagsPublic.DeviceLocal,
            usage: VkBufferUsageFlagsPublic.StorageBuffer | VkBufferUsageFlagsPublic.TransferDst);

        buffer.Handle.Should().NotBe(IntPtr.Zero);
        buffer.Size.Should().Be(4096UL);
    }
}
