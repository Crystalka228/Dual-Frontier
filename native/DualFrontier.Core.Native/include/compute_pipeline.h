// V0.B compute pipeline registration — native side.
//
// Provides the data + bookkeeping layer for compute pipeline registration.
// V0.B implementation is in-process bookkeeping only — pipeline IDs allocated
// monotonically, SPIR-V bytecode + binding count retained per registration,
// Vulkan handles received via df_world_attach_vulkan but не consumed for
// vkCreateComputePipelines calls. K10.3 / V1+ briefs flesh out the actual
// Vulkan-backed pipeline creation when compute is wired к K9 field storage
// + K-L16 pipeline depth dispatches.
//
// The C ABI surface is fixed at V0.B (per S-LOCK-3 + S-LOCK-11 atomic cascade
// discipline) so managed-side FieldStorageBinding (V0.B Commit 15) can wire
// against the stable signatures.

#pragma once

#include <cstdint>
#include <string>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

// Opaque Vulkan handle storage. Native side does not interpret these — it just
// passes them through. V1+ implementation casts back to VkInstance/VkDevice/...
// when actually invoking Vulkan.
struct VulkanAttachment {
    void* instance = nullptr;
    void* physical_device = nullptr;
    void* device = nullptr;
    void* async_compute_queue = nullptr;
    uint32_t async_compute_queue_family_index = 0;
    bool attached = false;
};

struct ComputePipelineEntry {
    std::string name;
    std::vector<uint8_t> spirv_bytecode;
    uint32_t descriptor_binding_count = 0;
};

class ComputePipelineRegistry {
public:
    // Returns a non-zero pipeline_id on success; 0 on failure (e.g., duplicate name
    // or empty bytecode).
    uint32_t register_pipeline(const std::string& name,
                               const uint8_t* spirv,
                               int32_t spirv_size,
                               uint32_t descriptor_binding_count);

    // Returns nullptr if pipeline_id is unknown.
    const ComputePipelineEntry* get_pipeline(uint32_t pipeline_id) const noexcept;

    [[nodiscard]] int32_t count() const noexcept { return static_cast<int32_t>(pipelines_.size()); }

    void clear() noexcept;

private:
    std::unordered_map<uint32_t, ComputePipelineEntry> pipelines_;
    uint32_t next_id_ = 1;  // 0 reserved for "invalid".
};

}  // namespace dualfrontier
