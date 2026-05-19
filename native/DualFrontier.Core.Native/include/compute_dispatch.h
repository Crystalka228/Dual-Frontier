// V0.B compute dispatch — native side stub.
//
// Wraps a single compute dispatch invocation. V0.B implementation is a no-op
// that returns success — the actual VkCmdDispatch + VkQueueSubmit + fence
// sync sequence lands V1+ when compute is wired к K9 field storage. The C
// ABI surface is fixed at V0.B per S-LOCK-3 so managed-side consumers do
// not have к churn.

#pragma once

#include <cstdint>

namespace dualfrontier {

// V0.B no-op dispatch. Returns true to allow round-trip tests к pass; the
// actual Vulkan dispatch is deferred к V1+ when bound к K9 field storage.
bool dispatch_compute_noop(const char* field_name,
                           uint32_t pipeline_id,
                           uint32_t x, uint32_t y, uint32_t z);

}  // namespace dualfrontier
