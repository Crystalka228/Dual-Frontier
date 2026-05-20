#include "compute_pipeline.h"

#include <cstring>

namespace dualfrontier {

namespace {

// Helper: create the compute shader pipeline. Returns true on success — caller
// reads handles from `out_entry`. On failure, all created intermediate handles
// are torn down before return (entry's handles remain VK_NULL_HANDLE).
//
// Failure modes returned via false:
//   * vkCreateShaderModule failure (bad SPIR-V bytecode)
//   * vkCreateDescriptorSetLayout failure
//   * vkCreatePipelineLayout failure
//   * vkCreateComputePipelines failure
//
// All Vulkan error returns are observed via VkResult != VK_SUCCESS.
bool create_compute_pipeline_objects(VkDevice device,
                                     const uint8_t* spirv,
                                     int32_t spirv_size,
                                     uint32_t descriptor_binding_count,
                                     uint32_t push_constant_size,
                                     ComputePipelineEntry& out_entry)
{
    // 1. VkShaderModule.
    VkShaderModuleCreateInfo shader_info{};
    shader_info.sType = VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;
    shader_info.codeSize = static_cast<size_t>(spirv_size);
    shader_info.pCode = reinterpret_cast<const uint32_t*>(spirv);

    VkShaderModule shader_module = VK_NULL_HANDLE;
    if (vkCreateShaderModule(device, &shader_info, nullptr, &shader_module) != VK_SUCCESS) {
        return false;
    }

    // 2. VkDescriptorSetLayout — all bindings are STORAGE_BUFFER в compute stage
    // per V1 substrate primitive convention (V2 will follow same convention).
    std::vector<VkDescriptorSetLayoutBinding> bindings(descriptor_binding_count);
    for (uint32_t i = 0; i < descriptor_binding_count; ++i) {
        bindings[i] = {};
        bindings[i].binding = i;
        bindings[i].descriptorType = VK_DESCRIPTOR_TYPE_STORAGE_BUFFER;
        bindings[i].descriptorCount = 1;
        bindings[i].stageFlags = VK_SHADER_STAGE_COMPUTE_BIT;
        bindings[i].pImmutableSamplers = nullptr;
    }

    VkDescriptorSetLayoutCreateInfo set_layout_info{};
    set_layout_info.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO;
    set_layout_info.bindingCount = descriptor_binding_count;
    set_layout_info.pBindings = descriptor_binding_count == 0 ? nullptr : bindings.data();

    VkDescriptorSetLayout set_layout = VK_NULL_HANDLE;
    if (vkCreateDescriptorSetLayout(device, &set_layout_info, nullptr, &set_layout) != VK_SUCCESS) {
        vkDestroyShaderModule(device, shader_module, nullptr);
        return false;
    }

    // 3. VkPipelineLayout — 1 descriptor set layout + optional push constant range.
    VkPushConstantRange push_range{};
    push_range.stageFlags = VK_SHADER_STAGE_COMPUTE_BIT;
    push_range.offset = 0;
    push_range.size = push_constant_size;

    VkPipelineLayoutCreateInfo layout_info{};
    layout_info.sType = VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO;
    layout_info.setLayoutCount = 1;
    layout_info.pSetLayouts = &set_layout;
    layout_info.pushConstantRangeCount = (push_constant_size > 0) ? 1 : 0;
    layout_info.pPushConstantRanges = (push_constant_size > 0) ? &push_range : nullptr;

    VkPipelineLayout pipeline_layout = VK_NULL_HANDLE;
    if (vkCreatePipelineLayout(device, &layout_info, nullptr, &pipeline_layout) != VK_SUCCESS) {
        vkDestroyDescriptorSetLayout(device, set_layout, nullptr);
        vkDestroyShaderModule(device, shader_module, nullptr);
        return false;
    }

    // 4. VkPipeline (compute) — single stage = main, no specialization.
    VkComputePipelineCreateInfo pipeline_info{};
    pipeline_info.sType = VK_STRUCTURE_TYPE_COMPUTE_PIPELINE_CREATE_INFO;
    pipeline_info.stage = {};
    pipeline_info.stage.sType = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
    pipeline_info.stage.stage = VK_SHADER_STAGE_COMPUTE_BIT;
    pipeline_info.stage.module = shader_module;
    pipeline_info.stage.pName = "main";
    pipeline_info.layout = pipeline_layout;
    pipeline_info.basePipelineHandle = VK_NULL_HANDLE;
    pipeline_info.basePipelineIndex = -1;

    VkPipeline pipeline = VK_NULL_HANDLE;
    if (vkCreateComputePipelines(device, VK_NULL_HANDLE, 1, &pipeline_info, nullptr, &pipeline) != VK_SUCCESS) {
        vkDestroyPipelineLayout(device, pipeline_layout, nullptr);
        vkDestroyDescriptorSetLayout(device, set_layout, nullptr);
        vkDestroyShaderModule(device, shader_module, nullptr);
        return false;
    }

    out_entry.shader_module = shader_module;
    out_entry.descriptor_set_layout = set_layout;
    out_entry.pipeline_layout = pipeline_layout;
    out_entry.pipeline = pipeline;
    return true;
}

}  // anonymous namespace

void ComputePipelineRegistry::destroy_pipeline_objects(ComputePipelineEntry& entry,
                                                       VkDevice device) noexcept
{
    if (entry.pipeline != VK_NULL_HANDLE) {
        vkDestroyPipeline(device, entry.pipeline, nullptr);
        entry.pipeline = VK_NULL_HANDLE;
    }
    if (entry.pipeline_layout != VK_NULL_HANDLE) {
        vkDestroyPipelineLayout(device, entry.pipeline_layout, nullptr);
        entry.pipeline_layout = VK_NULL_HANDLE;
    }
    if (entry.descriptor_set_layout != VK_NULL_HANDLE) {
        vkDestroyDescriptorSetLayout(device, entry.descriptor_set_layout, nullptr);
        entry.descriptor_set_layout = VK_NULL_HANDLE;
    }
    if (entry.shader_module != VK_NULL_HANDLE) {
        vkDestroyShaderModule(device, entry.shader_module, nullptr);
        entry.shader_module = VK_NULL_HANDLE;
    }
}

uint32_t ComputePipelineRegistry::register_pipeline(
    const std::string& name,
    const uint8_t* spirv,
    int32_t spirv_size,
    uint32_t descriptor_binding_count,
    uint32_t push_constant_size,
    const VulkanAttachment& attachment)
{
    if (spirv == nullptr || spirv_size <= 0 || (spirv_size % 4) != 0) {
        return 0;
    }
    if (!attachment.attached || attachment.device == nullptr) {
        return 0;
    }
    // Check для duplicate name.
    for (const auto& entry : pipelines_) {
        if (entry.second.name == name) {
            return 0;
        }
    }

    uint32_t id = next_id_++;
    ComputePipelineEntry entry{};
    entry.name = name;
    entry.spirv_bytecode.assign(spirv, spirv + spirv_size);
    entry.descriptor_binding_count = descriptor_binding_count;
    entry.push_constant_size = push_constant_size;

    VkDevice device = static_cast<VkDevice>(attachment.device);
    if (!create_compute_pipeline_objects(device, spirv, spirv_size,
                                         descriptor_binding_count,
                                         push_constant_size, entry)) {
        // Rollback id allocation. Since IDs are monotonic and не stored elsewhere
        // until the emplace below succeeds, no other cleanup is needed.
        --next_id_;
        return 0;
    }

    pipelines_.emplace(id, std::move(entry));
    return id;
}

const ComputePipelineEntry* ComputePipelineRegistry::get_pipeline(uint32_t pipeline_id) const noexcept
{
    auto it = pipelines_.find(pipeline_id);
    return it == pipelines_.end() ? nullptr : &it->second;
}

void ComputePipelineRegistry::clear(const VulkanAttachment& attachment) noexcept
{
    VkDevice device = attachment.device ? static_cast<VkDevice>(attachment.device) : VK_NULL_HANDLE;
    if (device != VK_NULL_HANDLE) {
        for (auto& kv : pipelines_) {
            destroy_pipeline_objects(kv.second, device);
        }
    }
    pipelines_.clear();
    next_id_ = 1;
}

ComputePipelineRegistry::~ComputePipelineRegistry()
{
    // Caller is responsible для invoking clear() with the live attachment
    // before destroying the registry — at world destruction the Vulkan device
    // may already be torn down. As а safety net, free the map entries (which
    // releases SPIR-V bytecode); Vulkan handles will leak if clear() was not
    // called, but the OS reclaims them at process exit.
    pipelines_.clear();
}

}  // namespace dualfrontier
