// V1 compute dispatch — native side.
//
// V0.B was а pure no-op success path; V1+ wires actual VkCmdDispatch against
// а K9 field's CPU-side RawTileField storage. The native side maintains
// shadow VkBuffers per registered field (input + output + conductivity),
// uploads field state CPU→GPU before dispatch, runs the compute shader,
// and reads back GPU→CPU to the field's primary buffer.
//
// Sync model: К-L7 atomic-from-observer. Dispatch is synchronous — the call
// blocks until VkFence signals. V1+ may switch к multi-frame async dispatch
// when consumers need it (M-V demonstration mods).

#pragma once

#include <cstdint>
#include <string>

namespace dualfrontier {

class World;

// V1+ real dispatch. Returns true on success, false on failure. Failure modes:
//   * unknown pipeline_id
//   * field not registered (по name)
//   * Vulkan operation failure (buffer alloc, descriptor alloc, queue submit)
//   * push_constant_size > pipeline's registered push constant range
//   * field not attached к Vulkan (world.has_vulkan_attached() == false)
//
// On success, the field's primary CPU storage is updated в-place с the GPU
// result. Push constant data is bound к the pipeline layout's compute push
// constant range starting at offset 0; pass nullptr + 0 if the pipeline does
// не use push constants.
bool dispatch_compute_field(World& world,
                            const std::string& field_name,
                            uint32_t pipeline_id,
                            const uint8_t* push_constant_data,
                            int32_t push_constant_size,
                            uint32_t dispatch_x,
                            uint32_t dispatch_y,
                            uint32_t dispatch_z);

// Releases all V1 dispatch resources (command pool, descriptor pool, fence,
// per-field shadow buffers) held by the attachment. Called from World
// destructor before the VkDevice is implicitly invalidated.
struct VulkanAttachment;
void release_dispatch_resources(VulkanAttachment& attachment) noexcept;

}  // namespace dualfrontier
