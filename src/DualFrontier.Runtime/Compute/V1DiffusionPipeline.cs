using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// V1 diffusion compute pipeline managed wrapper per VULKAN_SUBSTRATE.md §1.2.
/// Owns the native compute pipeline registration (handle = pipeline id returned
/// by <see cref="FieldStorageBinding.Register"/>) and provides а typed
/// <see cref="ExecuteIteration"/> API which forwards к the V1-5c real dispatch
/// path with а <see cref="DiffusionPushConstants"/> payload bound к the pipeline
/// layout's push constant range.
/// </summary>
/// <remarks>
/// Substrate primitive vs mod separation per К-L9 vanilla = mods: this class
/// ships the diffusion shader пэйлоад but does не decide D / K / dt — those
/// are gameplay parameters supplied by consumer mods (M-V1 mana, M-V2
/// electricity) per dispatch via the <see cref="DiffusionPushConstants"/>
/// struct.
/// </remarks>
public sealed class V1DiffusionPipeline
{
    private readonly FieldStorageBinding _binding;
    private readonly uint _pipelineId;

    /// <summary>Native pipeline id returned by <see cref="FieldStorageBinding.Register"/>.</summary>
    public uint PipelineId => _pipelineId;

    /// <summary>
    /// Constructs and registers а V1 diffusion compute pipeline on the native side.
    /// SPIR-V is expected к match <c>tools/shaders/diffusion.comp</c> (3 storage
    /// buffer bindings: input / output / conductivity; 16-byte push constant block
    /// matching <see cref="DiffusionPushConstants"/>).
    /// </summary>
    /// <exception cref="InvalidOperationException">Pipeline registration failed.</exception>
    public V1DiffusionPipeline(FieldStorageBinding binding, string pipelineName, ReadOnlySpan<byte> diffusionSpirv)
    {
        ArgumentNullException.ThrowIfNull(binding);
        ArgumentException.ThrowIfNullOrEmpty(pipelineName);

        _binding = binding;
        _pipelineId = binding.Register(
            pipelineName, diffusionSpirv,
            descriptorBindingCount: 3,
            pushConstantSize: (uint)Marshal.SizeOf<DiffusionPushConstants>());

        if (_pipelineId == 0)
        {
            throw new InvalidOperationException(
                $"V1 diffusion pipeline registration failed for '{pipelineName}'. " +
                "Causes: duplicate name, Vulkan не attached, invalid SPIR-V, or " +
                "compute pipeline / descriptor set layout / pipeline layout creation error.");
        }
    }

    /// <summary>
    /// Executes one diffusion iteration on the named field. Caller controls the
    /// iteration loop — typical gameplay use is 5–10 iterations per tick per
    /// VULKAN_SUBSTRATE.md §1.2 economics. Field's primary CPU storage is updated
    /// в-place с the GPU result; ping-pong via
    /// <see cref="DualFrontier.Core.Interop.FieldHandle{T}.SwapBuffers"/> is the
    /// caller's responsibility if the consumer needs double-buffering across iterations.
    /// </summary>
    /// <returns><c>true</c> on success; <c>false</c> if dispatch failed (field
    /// не registered, Vulkan error). К-L7 atomic-from-observer — call returns
    /// after the compute fence signals.</returns>
    public bool ExecuteIteration(string fieldName, DiffusionPushConstants pushConstants,
                                 uint dispatchX, uint dispatchY)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);

        ReadOnlySpan<byte> pcBytes = MemoryMarshal.AsBytes(
            new ReadOnlySpan<DiffusionPushConstants>(in pushConstants));

        return _binding.DispatchField(fieldName, _pipelineId, pcBytes,
                                      dispatchX, dispatchY, z: 1);
    }
}
