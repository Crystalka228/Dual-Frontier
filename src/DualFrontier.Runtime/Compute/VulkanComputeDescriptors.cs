using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// Descriptor set layout + descriptor pool + pipeline layout bundle for a compute pipeline.
/// V0.B `noop.comp` round-trip uses an empty layout (no descriptors); V1+ substrate primitives
/// pass actual storage buffer + uniform buffer bindings.
/// </summary>
public sealed class VulkanComputeDescriptors : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _setLayout;
    private IntPtr _pool;
    private IntPtr _pipelineLayout;
    private bool _disposed;

    public IntPtr DescriptorSetLayoutHandle => _setLayout;
    public IntPtr PipelineLayoutHandle => _pipelineLayout;

    public unsafe VulkanComputeDescriptors(
        VulkanDevice device,
        IReadOnlyList<ComputeDescriptorBinding> bindings)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(bindings);
        _device = device.Handle;

        // ---- Descriptor set layout ----
        var nativeBindings = new VkDescriptorSetLayoutBinding[bindings.Count];
        for (int i = 0; i < bindings.Count; i++)
        {
            nativeBindings[i] = new VkDescriptorSetLayoutBinding
            {
                binding = bindings[i].Binding,
                descriptorType = (VkDescriptorType)(int)bindings[i].Type,
                descriptorCount = 1,
                stageFlags = VkShaderStageFlags.VK_SHADER_STAGE_COMPUTE_BIT,
                pImmutableSamplers = IntPtr.Zero,
            };
        }

        fixed (VkDescriptorSetLayoutBinding* nbPtr = nativeBindings)
        {
            var info = new VkDescriptorSetLayoutCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                bindingCount = (uint)bindings.Count,
                pBindings = bindings.Count == 0 ? null : nbPtr,
            };
            VkResult result = VkApi.vkCreateDescriptorSetLayout(_device, in info, IntPtr.Zero, out _setLayout);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateDescriptorSetLayout failed: {result}");
            }
        }

        // ---- Descriptor pool (only allocated if there are bindings) ----
        if (bindings.Count > 0)
        {
            var poolSizes = new VkDescriptorPoolSize[bindings.Count];
            for (int i = 0; i < bindings.Count; i++)
            {
                poolSizes[i] = new VkDescriptorPoolSize
                {
                    type = (VkDescriptorType)(int)bindings[i].Type,
                    descriptorCount = 1,
                };
            }
            fixed (VkDescriptorPoolSize* psPtr = poolSizes)
            {
                var poolInfo = new VkDescriptorPoolCreateInfo
                {
                    sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO,
                    pNext = IntPtr.Zero,
                    flags = 0,
                    maxSets = 1,
                    poolSizeCount = (uint)bindings.Count,
                    pPoolSizes = psPtr,
                };
                VkResult result = VkApi.vkCreateDescriptorPool(_device, in poolInfo, IntPtr.Zero, out _pool);
                if (result != VkResult.VK_SUCCESS)
                {
                    throw new InvalidOperationException($"vkCreateDescriptorPool failed: {result}");
                }
            }
        }

        // ---- Pipeline layout (1 descriptor set layout, no push constants) ----
        IntPtr setLayout = _setLayout;
        var pipeLayoutInfo = new VkPipelineLayoutCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            setLayoutCount = 1,
            pSetLayouts = &setLayout,
            pushConstantRangeCount = 0,
            pPushConstantRanges = IntPtr.Zero,
        };
        VkResult plResult = VkApi.vkCreatePipelineLayout(_device, in pipeLayoutInfo, IntPtr.Zero, out _pipelineLayout);
        if (plResult != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreatePipelineLayout (compute) failed: {plResult}");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_pipelineLayout != IntPtr.Zero)
        {
            VkApi.vkDestroyPipelineLayout(_device, _pipelineLayout, IntPtr.Zero);
            _pipelineLayout = IntPtr.Zero;
        }
        if (_pool != IntPtr.Zero)
        {
            VkApi.vkDestroyDescriptorPool(_device, _pool, IntPtr.Zero);
            _pool = IntPtr.Zero;
        }
        if (_setLayout != IntPtr.Zero)
        {
            VkApi.vkDestroyDescriptorSetLayout(_device, _setLayout, IntPtr.Zero);
            _setLayout = IntPtr.Zero;
        }
        _disposed = true;
    }
}

/// <summary>Public-facing descriptor binding for compute pipeline registration.</summary>
public sealed record ComputeDescriptorBinding(uint Binding, ComputeDescriptorType Type);

/// <summary>Public-facing mirror of VkDescriptorType (compute-relevant subset).</summary>
public enum ComputeDescriptorType : int
{
    UniformBuffer = 6,
    StorageBuffer = 7,
    StorageImage = 3,
}
