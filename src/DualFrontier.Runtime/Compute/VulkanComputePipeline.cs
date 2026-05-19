using System.Runtime.InteropServices;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// VkPipeline with VK_PIPELINE_BIND_POINT_COMPUTE. Owns the pipeline lifetime + retains the
/// descriptors bundle (caller-supplied to allow sharing layouts across pipelines if needed).
/// </summary>
public sealed class VulkanComputePipeline : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _pipeline;
    private bool _disposed;

    public IntPtr Handle => _pipeline;
    public VulkanComputeDescriptors Descriptors { get; }

    public unsafe VulkanComputePipeline(
        VulkanDevice device,
        VulkanShaderModule computeShader,
        VulkanComputeDescriptors descriptors)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(computeShader);
        ArgumentNullException.ThrowIfNull(descriptors);

        _device = device.Handle;
        Descriptors = descriptors;

        IntPtr mainNamePtr = Marshal.StringToCoTaskMemUTF8("main");
        try
        {
            var createInfo = new VkComputePipelineCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_COMPUTE_PIPELINE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                stage = new VkPipelineShaderStageCreateInfo
                {
                    sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
                    pNext = IntPtr.Zero,
                    flags = 0,
                    stage = VkShaderStageFlags.VK_SHADER_STAGE_COMPUTE_BIT,
                    module = computeShader.Handle,
                    pName = (byte*)mainNamePtr,
                    pSpecializationInfo = IntPtr.Zero,
                },
                layout = descriptors.PipelineLayoutHandle,
                basePipelineHandle = IntPtr.Zero,
                basePipelineIndex = -1,
            };

            IntPtr pipeline = IntPtr.Zero;
            VkResult result = VkApi.vkCreateComputePipelines(
                _device,
                pipelineCache: IntPtr.Zero,
                createInfoCount: 1,
                pCreateInfos: &createInfo,
                pAllocator: IntPtr.Zero,
                pPipelines: &pipeline);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateComputePipelines failed: {result}");
            }
            _pipeline = pipeline;
        }
        finally
        {
            Marshal.FreeCoTaskMem(mainNamePtr);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_pipeline != IntPtr.Zero)
        {
            VkApi.vkDestroyPipeline(_device, _pipeline, IntPtr.Zero);
            _pipeline = IntPtr.Zero;
        }
        _disposed = true;
    }
}
