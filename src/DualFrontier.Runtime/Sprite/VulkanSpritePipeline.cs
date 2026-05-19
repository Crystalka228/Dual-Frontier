using System.Runtime.InteropServices;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Sprite-rendering Vulkan pipeline. Extends V0.B VulkanGraphicsPipeline pattern с:
/// - Vertex input (SpriteVertex 20 bytes, 3 attributes pos + uv + color)
/// - Alpha blending (premultiplied workflow per S-LOCK-5):
///     srcColorBlendFactor = ONE
///     dstColorBlendFactor = ONE_MINUS_SRC_ALPHA
/// - Descriptor set layout (1 combined image sampler, fragment stage)
/// - Push constant range (mat4 MVP, vertex stage, 64 bytes)
/// - Dynamic viewport + scissor (matches V0.B clearcolor pipeline для swapchain recreation flexibility)
/// </summary>
public sealed class VulkanSpritePipeline : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _pipeline;
    private bool _disposed;

    public IntPtr Handle => _pipeline;
    public VulkanPipelineLayout Layout { get; }
    public SpriteDescriptorSetLayout DescriptorSetLayout { get; }

    public unsafe VulkanSpritePipeline(
        VulkanDevice device,
        VulkanRenderPass renderPass,
        VulkanShaderModule vertexShader,
        VulkanShaderModule fragmentShader,
        SpriteDescriptorSetLayout descriptorSetLayout,
        VulkanPipelineLayout pipelineLayout)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(renderPass);
        ArgumentNullException.ThrowIfNull(vertexShader);
        ArgumentNullException.ThrowIfNull(fragmentShader);
        ArgumentNullException.ThrowIfNull(descriptorSetLayout);
        ArgumentNullException.ThrowIfNull(pipelineLayout);

        _device = device.Handle;
        Layout = pipelineLayout;
        DescriptorSetLayout = descriptorSetLayout;

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

            // Vertex input: SpriteVertex 20 bytes per S-LOCK-3
            var vertexBinding = new VkVertexInputBindingDescription
            {
                binding = 0,
                stride = 20,
                inputRate = VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX,
            };
            var vertexAttribs = stackalloc VkVertexInputAttributeDescription[3];
            vertexAttribs[0] = new() { location = 0, binding = 0, format = VkFormat.VK_FORMAT_R32G32_SFLOAT, offset = 0 };       // Position
            vertexAttribs[1] = new() { location = 1, binding = 0, format = VkFormat.VK_FORMAT_R32G32_SFLOAT, offset = 8 };       // Uv
            vertexAttribs[2] = new() { location = 2, binding = 0, format = VkFormat.VK_FORMAT_R8G8B8A8_UNORM, offset = 16 };     // TintRgba

            var vertexInput = new VkPipelineVertexInputStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                vertexBindingDescriptionCount = 1,
                pVertexBindingDescriptions = (IntPtr)(&vertexBinding),
                vertexAttributeDescriptionCount = 3,
                _padBeforeAttrPtr = 0,
                pVertexAttributeDescriptions = (IntPtr)vertexAttribs,
            };

            var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                topology = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                primitiveRestartEnable = 0,
                _padTrailing = 0,
            };

            var viewportState = new VkPipelineViewportStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                viewportCount = 1,
                pViewports = null,
                scissorCount = 1,
                _padBeforeScissorPtr = 0,
                pScissors = null,
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

            // Premultiplied alpha blending per S-LOCK-5
            var colorAttachment = new VkPipelineColorBlendAttachmentState
            {
                blendEnable = 1,
                srcColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                dstColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA,
                colorBlendOp = VkBlendOp.VK_BLEND_OP_ADD,
                srcAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                dstAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA,
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
                layout = pipelineLayout.Handle,
                renderPass = renderPass.Handle,
                subpass = 0,
                _padBeforeBase = 0,
                basePipelineHandle = IntPtr.Zero,
                basePipelineIndex = -1,
                _padTrailing = 0,
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
                throw new InvalidOperationException($"vkCreateGraphicsPipelines failed (sprite): {result}");
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
