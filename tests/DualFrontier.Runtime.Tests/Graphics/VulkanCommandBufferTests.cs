using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// V0.B command infrastructure lifecycle tests на real К-L19 hardware. Verifies
/// VulkanCommandPool + VulkanCommandBuffer + VulkanFence + VulkanSemaphore round-trip.
/// </summary>
public sealed class VulkanCommandBufferTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;

    public VulkanCommandBufferTests()
    {
        var opts = new WindowOptions { Title = "CmdBuf", Width = 320, Height = 240 };
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
    public void CommandPool_construct_returns_non_zero_handle()
    {
        using var pool = new VulkanCommandPool(_device, _device.GraphicsQueueFamilyIndex);
        pool.Handle.Should().NotBe(IntPtr.Zero);
        pool.QueueFamilyIndex.Should().Be(_device.GraphicsQueueFamilyIndex);
    }

    [Fact]
    public void CommandBuffer_allocate_begin_end_round_trip()
    {
        using var pool = new VulkanCommandPool(_device, _device.GraphicsQueueFamilyIndex);
        using var buffer = pool.AllocateBuffer();

        buffer.Handle.Should().NotBe(IntPtr.Zero);
        buffer.Begin();
        buffer.End();
    }

    [Fact]
    public void Fence_create_wait_with_signaled_start_returns_immediately()
    {
        using var fence = new VulkanFence(_device, startSignaled: true);
        fence.Handle.Should().NotBe(IntPtr.Zero);
        fence.Wait(timeoutNs: 0);  // already signaled — immediate return
        fence.Reset();
    }

    [Fact]
    public void Semaphore_create_returns_non_zero_handle()
    {
        using var semaphore = new VulkanSemaphore(_device);
        semaphore.Handle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void Submit_empty_command_buffer_to_graphics_queue_with_fence_signals_fence()
    {
        using var pool = new VulkanCommandPool(_device, _device.GraphicsQueueFamilyIndex);
        using var buffer = pool.AllocateBuffer();
        using var fence = new VulkanFence(_device);

        buffer.Begin();
        buffer.End();
        buffer.SubmitTo(_device.GraphicsQueue, fence: fence);

        // Fence should signal без timeout (empty command buffer completes immediately).
        fence.Wait();
    }
}
