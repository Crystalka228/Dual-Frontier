using DualFrontier.Core.Interop;
using DualFrontier.Runtime.Graphics;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// V0.B managed wrapper bridging K9 RawTileField storage к Vulkan compute pipeline
/// registrations on the native side. Consumes <see cref="NativeWorld"/> C ABI extensions
/// (AttachVulkan + RegisterComputePipeline + DispatchFieldCompute).
///
/// V0.B scope: orchestration only — native side performs actual VkBuffer binding в
/// V1+ when wired к real compute shaders consuming field storage. V0.B exit criterion
/// = registration round-trip + no-op dispatch succeeds.
/// </summary>
public sealed class FieldStorageBinding
{
    private readonly NativeWorld _world;

    public FieldStorageBinding(NativeWorld world)
    {
        ArgumentNullException.ThrowIfNull(world);
        _world = world;
    }

    /// <summary>
    /// Attach Vulkan handles from <see cref="Runtime"/> к native world. Caller invokes
    /// once при Runtime composition; subsequent re-attach overwrites previous handles
    /// (e.g., after swapchain recreation if devicewere to change, though V0.B reuses
    /// same device).
    /// </summary>
    public bool Attach(VulkanInstance instance, VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(device);
        if (device.AsyncComputeQueueFamilyIndex is null)
        {
            throw new InvalidOperationException(
                "FieldStorageBinding.Attach requires async compute queue family — К-L19 hardware tier mandate.");
        }
        return _world.AttachVulkan(
            instance.Handle,
            device.PhysicalDevice,
            device.Handle,
            device.AsyncComputeQueue,
            device.AsyncComputeQueueFamilyIndex.Value);
    }

    /// <summary>
    /// Register a named compute pipeline on the native side. Returns pipeline_id
    /// for use в later DispatchField calls. Returns 0 on failure.
    /// </summary>
    public uint Register(string pipelineName, ReadOnlySpan<byte> spirvBytecode, uint descriptorBindingCount)
    {
        return _world.RegisterComputePipeline(pipelineName, spirvBytecode, descriptorBindingCount);
    }

    /// <summary>
    /// Dispatch a registered pipeline against a named K9 field. V0.B no-op success
    /// path; V1+ implements actual VkCmdDispatch sequence.
    /// </summary>
    public bool DispatchField(string fieldName, uint pipelineId, uint x, uint y, uint z)
    {
        return _world.DispatchFieldCompute(fieldName, pipelineId, x, y, z);
    }

    public int PipelineCount => _world.ComputePipelineCount();
}
