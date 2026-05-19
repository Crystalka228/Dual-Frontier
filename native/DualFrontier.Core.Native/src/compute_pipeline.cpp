#include "compute_pipeline.h"

namespace dualfrontier {

uint32_t ComputePipelineRegistry::register_pipeline(
    const std::string& name,
    const uint8_t* spirv,
    int32_t spirv_size,
    uint32_t descriptor_binding_count)
{
    if (spirv == nullptr || spirv_size <= 0 || (spirv_size % 4) != 0) {
        return 0;
    }
    // Check for duplicate name.
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
    pipelines_.emplace(id, std::move(entry));
    return id;
}

const ComputePipelineEntry* ComputePipelineRegistry::get_pipeline(uint32_t pipeline_id) const noexcept
{
    auto it = pipelines_.find(pipeline_id);
    return it == pipelines_.end() ? nullptr : &it->second;
}

void ComputePipelineRegistry::clear() noexcept
{
    pipelines_.clear();
    next_id_ = 1;
}

}  // namespace dualfrontier
