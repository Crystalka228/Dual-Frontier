using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// Synchronous compute dispatch helper. Records a command buffer (bind pipeline + dispatch),
/// submits к async compute queue, waits на fence per К-L7 atomic-from-observer.
/// V1+ substrate primitives may use async dispatch с fence stored for next-tick read.
/// </summary>
public sealed class ComputeDispatch : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly VulkanCommandPool _pool;
    private readonly VulkanCommandBuffer _commandBuffer;
    private readonly VulkanFence _fence;
    private bool _disposed;

    public ComputeDispatch(VulkanDevice device, VulkanCommandPool pool)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(pool);
        _device = device;
        _pool = pool;
        _commandBuffer = pool.AllocateBuffer();
        _fence = new VulkanFence(device);
    }

    /// <summary>
    /// Execute compute dispatch synchronously. Submits к async compute queue + waits на fence.
    /// </summary>
    public void ExecuteSync(VulkanComputePipeline pipeline, uint x, uint y, uint z)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        ObjectDisposedException.ThrowIf(_disposed, this);

        _commandBuffer.Begin();
        VkApi.vkCmdBindPipeline(
            _commandBuffer.Handle,
            VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_COMPUTE,
            pipeline.Handle);
        _commandBuffer.Dispatch(x, y, z);
        _commandBuffer.End();

        // Submit к async compute queue (selected by V0.B Commit 4).
        _commandBuffer.SubmitTo(_device.AsyncComputeQueue, fence: _fence);
        _fence.Wait();
        _fence.Reset();
        _commandBuffer.Reset();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _fence.Dispose();
        _commandBuffer.Dispose();
        _disposed = true;
    }
}
