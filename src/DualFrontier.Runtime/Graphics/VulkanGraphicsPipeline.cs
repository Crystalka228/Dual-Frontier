using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Minimal V0.B graphics pipeline: fullscreen triangle, no vertex input, fill polygon mode,
/// no cull, no MSAA, single color attachment с no blending, dynamic viewport + scissor.
/// Sufficient для V0.B clear color exit criterion (Commit 17 smoke test).
/// Full sprite pipeline (с vertex input, blending, descriptor sets) lands V0.C.
/// </summary>
public sealed class VulkanGraphicsPipeline : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _pipeline;
    private bool _disposed;

    public IntPtr Handle => _pipeline;
    public VulkanPipelineLayout Layout { get; }

    public unsafe VulkanGraphicsPipeline(
        VulkanDevice device,
        VulkanPipelineLayout layout,
        VulkanRenderPass renderPass,
        VulkanShaderModule vertexShader,
        VulkanShaderModule fragmentShader)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(layout);
        ArgumentNullException.ThrowIfNull(renderPass);
        ArgumentNullException.ThrowIfNull(vertexShader);
        ArgumentNullException.ThrowIfNull(fragmentShader);

        _device = device.Handle;
        Layout = layout;

        // Pin entry-point name "main" as UTF-8.
        IntPtr mainNamePtr = Marshal.StringToCoTaskMemUTF8("main");
        try
        {
            var stages = stackalloc VkPipelineShaderStageCreateInfo[2];
            stages[0] = new VkPipelineShaderStageCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                stage = VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
                module = vertexShader.Handle,
                pName = (byte*)mainNamePtr,
                pSpecializationInfo = IntPtr.Zero,
            };
            stages[1] = new VkPipelineShaderStageCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                stage = VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT,
                module = fragmentShader.Handle,
                pName = (byte*)mainNamePtr,
                pSpecializationInfo = IntPtr.Zero,
            };

            var vertexInput = new VkPipelineVertexInputStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                vertexBindingDescriptionCount = 0,
                pVertexBindingDescriptions = IntPtr.Zero,
                vertexAttributeDescriptionCount = 0,
                pVertexAttributeDescriptions = IntPtr.Zero,
            };

            var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                topology = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                primitiveRestartEnable = 0,
            };

            var viewportState = new VkPipelineViewportStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                viewportCount = 1,
                pViewports = null,    // dynamic
                scissorCount = 1,
                pScissors = null,     // dynamic
            };

            var rasterizer = new VkPipelineRasterizationStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                depthClampEnable = 0,
                rasterizerDiscardEnable = 0,
                polygonMode = VkPolygonMode.VK_POLYGON_MODE_FILL,
                cullMode = VkCullModeFlags.VK_CULL_MODE_NONE,
                frontFace = VkFrontFace.VK_FRONT_FACE_COUNTER_CLOCKWISE,
                depthBiasEnable = 0,
                depthBiasConstantFactor = 0,
                depthBiasClamp = 0,
                depthBiasSlopeFactor = 0,
                lineWidth = 1.0f,
            };

            var multisample = new VkPipelineMultisampleStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                rasterizationSamples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
                sampleShadingEnable = 0,
                minSampleShading = 0,
                pSampleMask = null,
                alphaToCoverageEnable = 0,
                alphaToOneEnable = 0,
            };

            var colorAttachment = new VkPipelineColorBlendAttachmentState
            {
                blendEnable = 0,
                srcColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                dstColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ZERO,
                colorBlendOp = VkBlendOp.VK_BLEND_OP_ADD,
                srcAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                dstAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ZERO,
                alphaBlendOp = VkBlendOp.VK_BLEND_OP_ADD,
                colorWriteMask = VkColorComponentFlags.VK_COLOR_COMPONENT_RGBA,
            };
            VkPipelineColorBlendStateCreateInfo colorBlend = default;
            colorBlend.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
            colorBlend.pNext = IntPtr.Zero;
            colorBlend.flags = 0;
            colorBlend.logicOpEnable = 0;
            colorBlend.logicOp = VkLogicOp.VK_LOGIC_OP_COPY;
            colorBlend.attachmentCount = 1;
            colorBlend.pAttachments = &colorAttachment;
            // blendConstants left zero (fixed buffer default-initialized).

            var dynamicStates = stackalloc VkDynamicState[2]
            {
                VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT,
                VkDynamicState.VK_DYNAMIC_STATE_SCISSOR,
            };
            var dynamicState = new VkPipelineDynamicStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                dynamicStateCount = 2,
                pDynamicStates = dynamicStates,
            };

            var createInfo = new VkGraphicsPipelineCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                stageCount = 2,
                pStages = stages,
                pVertexInputState = &vertexInput,
                pInputAssemblyState = &inputAssembly,
                pTessellationState = IntPtr.Zero,
                pViewportState = &viewportState,
                pRasterizationState = &rasterizer,
                pMultisampleState = &multisample,
                pDepthStencilState = IntPtr.Zero,
                pColorBlendState = &colorBlend,
                pDynamicState = &dynamicState,
                layout = layout.Handle,
                renderPass = renderPass.Handle,
                subpass = 0,
                basePipelineHandle = IntPtr.Zero,
                basePipelineIndex = -1,
            };

            IntPtr pipeline = IntPtr.Zero;
            VkResult result = VkApi.vkCreateGraphicsPipelines(
                _device,
                pipelineCache: IntPtr.Zero,
                createInfoCount: 1,
                pCreateInfos: &createInfo,
                pAllocator: IntPtr.Zero,
                pPipelines: &pipeline);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateGraphicsPipelines failed: {result}");
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
