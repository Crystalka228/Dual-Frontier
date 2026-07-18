using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Primary VkCommandBuffer wrapper. Begin/End around recording; Reset rewinds к unrecorded state.
/// SubmitTo submits the buffer к a queue с optional wait/signal semaphores + fence.
/// </summary>
public sealed class VulkanCommandBuffer : IDisposable
{
    private readonly IntPtr _device;
    private readonly IntPtr _commandPool;
    private IntPtr _buffer;
    private bool _disposed;

    public IntPtr Handle => _buffer;

    internal unsafe VulkanCommandBuffer(VulkanCommandPool pool, IntPtr device)
    {
        ArgumentNullException.ThrowIfNull(pool);
        _device = device;
        _commandPool = pool.Handle;

        var allocInfo = new VkCommandBufferAllocateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            pNext = IntPtr.Zero,
            commandPool = _commandPool,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandBufferCount = 1,
        };

        IntPtr buffer = IntPtr.Zero;
        VkResult result = VkApi.vkAllocateCommandBuffers(_device, in allocInfo, &buffer);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkAllocateCommandBuffers failed: {result}");
        }
        _buffer = buffer;
    }

    public void Begin(VkCommandBufferUsageFlagsPublic usage = VkCommandBufferUsageFlagsPublic.OneTimeSubmit)
    {
        var info = new VkCommandBufferBeginInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO,
            pNext = IntPtr.Zero,
            flags = (VkCommandBufferUsageFlags)(uint)usage,
            pInheritanceInfo = IntPtr.Zero,
        };
        VkResult result = VkApi.vkBeginCommandBuffer(_buffer, in info);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkBeginCommandBuffer failed: {result}");
        }
    }

    public void End()
    {
        VkResult result = VkApi.vkEndCommandBuffer(_buffer);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkEndCommandBuffer failed: {result}");
        }
    }

    public void Reset()
    {
        VkResult result = VkApi.vkResetCommandBuffer(_buffer, flags: 0);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkResetCommandBuffer failed: {result}");
        }
    }

    /// <summary>
    /// Submit this buffer к the given queue. Optional wait semaphore + stage mask + signal
    /// semaphore + signaling fence. V0.B exits через synchronous fence wait per К-L7
    /// atomic-from-observer.
    /// </summary>
    public unsafe void SubmitTo(
        IntPtr queue,
        IntPtr waitSemaphore = default,
        VkPipelineStageFlagsPublic waitStage = VkPipelineStageFlagsPublic.ColorAttachmentOutput,
        IntPtr signalSemaphore = default,
        VulkanFence? fence = null)
    {
        IntPtr buffer = _buffer;
        IntPtr wait = waitSemaphore;
        IntPtr signal = signalSemaphore;
        VkPipelineStageFlags stageMask = (VkPipelineStageFlags)(uint)waitStage;

        var submit = new VkSubmitInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO,
            pNext = IntPtr.Zero,
            waitSemaphoreCount = wait == IntPtr.Zero ? 0u : 1u,
            pWaitSemaphores = wait == IntPtr.Zero ? null : &wait,
            pWaitDstStageMask = wait == IntPtr.Zero ? null : &stageMask,
            commandBufferCount = 1,
            pCommandBuffers = &buffer,
            signalSemaphoreCount = signal == IntPtr.Zero ? 0u : 1u,
            pSignalSemaphores = signal == IntPtr.Zero ? null : &signal,
        };

        IntPtr fenceHandle = fence?.Handle ?? IntPtr.Zero;
        VkResult result = VkApi.vkQueueSubmit(queue, 1, in submit, fenceHandle);
        DeviceLost.ThrowIfLost(result, new DeviceLostContext(VulkanCall.QueueSubmit));
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkQueueSubmit failed: {result}");
        }
    }

    /// <summary>Wrapper for vkCmdDispatch (compute pipeline dispatch on this command buffer).</summary>
    public void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        VkApi.vkCmdDispatch(_buffer, groupCountX, groupCountY, groupCountZ);
    }

    public unsafe void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_buffer != IntPtr.Zero)
        {
            IntPtr buffer = _buffer;
            VkApi.vkFreeCommandBuffers(_device, _commandPool, 1, &buffer);
            _buffer = IntPtr.Zero;
        }
        _disposed = true;
    }
}

/// <summary>Public-facing mirror of VkCommandBufferUsageFlags.</summary>
[Flags]
public enum VkCommandBufferUsageFlagsPublic : uint
{
    OneTimeSubmit = 0x00000001,
    RenderPassContinue = 0x00000002,
    SimultaneousUse = 0x00000004,
}

/// <summary>Public-facing mirror of VkPipelineStageFlags (V0.B subset).</summary>
[Flags]
public enum VkPipelineStageFlagsPublic : uint
{
    TopOfPipe = 0x00000001,
    VertexInput = 0x00000004,
    VertexShader = 0x00000008,
    FragmentShader = 0x00000080,
    ColorAttachmentOutput = 0x00000400,
    ComputeShader = 0x00000800,
    Transfer = 0x00001000,
    BottomOfPipe = 0x00002000,
    AllCommands = 0x00010000,
}

