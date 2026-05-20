// V1 compute pipeline registration — native side.
//
// V0.B implementation was in-process bookkeeping only — SPIR-V bytecode +
// binding count retained, but vkCreateComputePipelines не called. V1
// substrate primitives need real VkPipeline + VkPipelineLayout + descriptor
// set layout objects к support actual VkCmdDispatch wiring per
// VULKAN_SUBSTRATE.md §1.2.
//
// At registration, given а valid Vulkan attachment + parsed SPIR-V, this
// registry creates:
//   * VkShaderModule from SPIR-V bytes
//   * VkDescriptorSetLayout with N storage buffer bindings (compute stage)
//   * VkPipelineLayout с the descriptor set layout + а push constant range
//     covering VK_SHADER_STAGE_COMPUTE_BIT (size 0 if push_constant_size == 0)
//   * VkPipeline (compute, single shader stage = main, no specialization)
//
// Vulkan objects are destroyed when the pipeline is cleared or the registry
// is destroyed. The C ABI surface (df_world_register_compute_pipeline) takes
// а new push_constant_size argument к size the pipeline layout's push
// constant range; managed-side P/Invoke wrappers were updated to match
// (V1-5d).

#pragma once

#include <cstdint>
#include <string>
#include <unordered_map>
#include <vector>

#include <vulkan/vulkan.h>

namespace dualfrontier {

// Per-field shadow VkBuffer set used by V1+ compute dispatch. Lazily created
// при first dispatch against а given field, cached across iterations, freed
// when the registry is cleared / world is destroyed.
struct FieldShadowBuffers {
    VkBuffer input = VK_NULL_HANDLE;
    VkBuffer output = VK_NULL_HANDLE;
    VkBuffer conductivity = VK_NULL_HANDLE;
    VkDeviceMemory input_memory = VK_NULL_HANDLE;
    VkDeviceMemory output_memory = VK_NULL_HANDLE;
    VkDeviceMemory conductivity_memory = VK_NULL_HANDLE;
    int32_t byte_size = 0;            // width * height * cell_size
    int32_t conductivity_byte_size = 0;  // width * height * sizeof(float)
};

// Opaque Vulkan handle storage + per-world dispatch resources. Set via
// df_world_attach_vulkan; cast back к VkInstance/VkDevice/VkQueue when
// invoking Vulkan APIs. Dispatch resources (descriptor pool + command pool +
// fence + per-field shadow buffers) are lazily allocated by the V1 dispatch
// path and released в release_dispatch_resources().
struct VulkanAttachment {
    void* instance = nullptr;
    void* physical_device = nullptr;
    void* device = nullptr;
    void* async_compute_queue = nullptr;
    uint32_t async_compute_queue_family_index = 0;
    bool attached = false;

    // V1+ dispatch state. Owned by attachment lifetime — freed before the
    // VkDevice would otherwise outlive these handles.
    VkCommandPool command_pool = VK_NULL_HANDLE;
    VkCommandBuffer command_buffer = VK_NULL_HANDLE;
    VkFence dispatch_fence = VK_NULL_HANDLE;
    VkDescriptorPool descriptor_pool = VK_NULL_HANDLE;
    std::unordered_map<std::string, FieldShadowBuffers> field_buffers;

    // Memory type index satisfying HOST_VISIBLE | HOST_COHERENT, computed on
    // first allocation. UINT32_MAX = not yet probed.
    uint32_t host_coherent_memory_type = UINT32_MAX;
};

struct ComputePipelineEntry {
    std::string name;
    std::vector<uint8_t> spirv_bytecode;
    uint32_t descriptor_binding_count = 0;
    uint32_t push_constant_size = 0;

    // Vulkan objects created at registration time when the world has Vulkan
    // attached. Owned by the registry — destroyed in clear() / destructor.
    VkShaderModule shader_module = VK_NULL_HANDLE;
    VkDescriptorSetLayout descriptor_set_layout = VK_NULL_HANDLE;
    VkPipelineLayout pipeline_layout = VK_NULL_HANDLE;
    VkPipeline pipeline = VK_NULL_HANDLE;
};

class ComputePipelineRegistry {
public:
    ComputePipelineRegistry() = default;
    ~ComputePipelineRegistry();

    // Returns а non-zero pipeline_id on success; 0 on failure (e.g., duplicate
    // name, empty bytecode, Vulkan object creation failure).
    //
    // The attachment supplies the VkDevice consumed during Vulkan object
    // creation. push_constant_size sizes the pipeline layout's push constant
    // range covering VK_SHADER_STAGE_COMPUTE_BIT; pass 0 if the shader does
    // не use push constants.
    uint32_t register_pipeline(const std::string& name,
                               const uint8_t* spirv,
                               int32_t spirv_size,
                               uint32_t descriptor_binding_count,
                               uint32_t push_constant_size,
                               const VulkanAttachment& attachment);

    // Returns nullptr if pipeline_id is unknown.
    const ComputePipelineEntry* get_pipeline(uint32_t pipeline_id) const noexcept;

    [[nodiscard]] int32_t count() const noexcept { return static_cast<int32_t>(pipelines_.size()); }

    // Destroys all Vulkan objects + clears the map. Caller must ensure the
    // device is idle (no pending dispatches against any pipeline).
    void clear(const VulkanAttachment& attachment) noexcept;

private:
    std::unordered_map<uint32_t, ComputePipelineEntry> pipelines_;
    uint32_t next_id_ = 1;  // 0 reserved для "invalid".

    // Helper that destroys all Vulkan objects associated с а single entry.
    // Resets handles к VK_NULL_HANDLE. Idempotent.
    static void destroy_pipeline_objects(ComputePipelineEntry& entry,
                                         VkDevice device) noexcept;
};

}  // namespace dualfrontier
