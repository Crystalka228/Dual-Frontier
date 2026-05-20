#include "phase_compute.h"

#include <array>
#include <atomic>
#include <cstring>

namespace {

// Compute dispatch record — one entry per registered dispatch call (Phase.Update
// time). Accumulates until Phase.Compute boundary, where df_scheduler_dispatch_phase_compute
// coalesces all records into single VkQueueSubmit batch.
struct ComputeDispatchRecord {
    PipelineSlot* slot;
    void* pipeline_handle;       // VkPipeline opaque
    void* descriptor_set;        // VkDescriptorSet opaque
    int32_t dispatch_x;
    int32_t dispatch_y;
    int32_t dispatch_z;
};

// Global Phase.Compute dispatch registry. Bounded per-tick capacity per
// DF_PHASE_COMPUTE_MAX_DISPATCHES_PER_TICK; cleared after each Phase.Compute
// boundary by df_scheduler_dispatch_phase_compute.
struct PhaseComputeState {
    std::array<ComputeDispatchRecord, DF_PHASE_COMPUTE_MAX_DISPATCHES_PER_TICK> records{};
    std::atomic<int32_t> record_count{0};
};

PhaseComputeState g_phase_compute;

}  // namespace

extern "C" {

int32_t df_scheduler_register_compute_dispatch(
    PipelineSlot* slot,
    void* pipeline_handle,
    void* descriptor_set,
    int32_t dispatch_x,
    int32_t dispatch_y,
    int32_t dispatch_z) {
    if (!slot) {
        return 0;
    }
    // Bounded per-tick registry — fail if exceeded (caller surfaces / splits).
    int32_t index = g_phase_compute.record_count.fetch_add(1);
    if (index >= DF_PHASE_COMPUTE_MAX_DISPATCHES_PER_TICK) {
        g_phase_compute.record_count.store(DF_PHASE_COMPUTE_MAX_DISPATCHES_PER_TICK);
        return 0;
    }
    ComputeDispatchRecord& record = g_phase_compute.records[index];
    record.slot = slot;
    record.pipeline_handle = pipeline_handle;
    record.descriptor_set = descriptor_set;
    record.dispatch_x = dispatch_x;
    record.dispatch_y = dispatch_y;
    record.dispatch_z = dispatch_z;
    return 1;
}

int32_t df_scheduler_compute_dispatch_count(PipelineSlot* slot) {
    if (!slot) {
        return 0;
    }
    int32_t count = 0;
    const int32_t total = g_phase_compute.record_count.load();
    const int32_t bounded = total > DF_PHASE_COMPUTE_MAX_DISPATCHES_PER_TICK
                                ? DF_PHASE_COMPUTE_MAX_DISPATCHES_PER_TICK
                                : total;
    for (int32_t i = 0; i < bounded; ++i) {
        if (g_phase_compute.records[i].slot == slot) {
            ++count;
        }
    }
    return count;
}

int32_t df_scheduler_dispatch_phase_compute(PipelineSlot* slot) {
    if (!slot) {
        return 0;
    }
    if (slot->state != SlotState_Dispatched) {
        return 0;
    }
    // K10.3 v2 Commit 4 scope: stub — accumulated dispatches present; real
    // VkQueueSubmit batching wired через VulkanAttachment в V-cycle integration.
    // Slot fence binding stays caller responsibility (set_fence after vkQueueSubmit).
    //
    // Per S-LOCK-13: Phase.Compute coexists с V1 dispatch_compute_field sync
    // path; pipeline-managed consumers route here, V1 consumers continue
    // unchanged через compute_dispatch.h.
    const int32_t count = df_scheduler_compute_dispatch_count(slot);
    if (count == 0) {
        // No dispatches на этот slot — nothing к submit, но slot still valid.
        // Caller may want к force_fence_completed себя (slot completes vacuously).
        return 1;
    }
    // Real VkQueueSubmit happens в V-cycle integration; this commit lands
    // infrastructure только. Slot remains в Dispatched state — caller's
    // df_pipeline_check_fences (Commit 4+ wires actual vkGetFenceStatus)
    // или force_fence_completed (test path) drives state machine forward.
    return 1;
}

int32_t df_scheduler_submit_compute_batch(
    PipelineSlot* slot,
    void* async_compute_queue) {
    (void)async_compute_queue;  // K10.3 v2 stub — V-cycle integration uses this.
    return df_scheduler_compute_dispatch_count(slot);
}

void df_scheduler_phase_compute_reset(void) {
    g_phase_compute.record_count.store(0);
    // Records themselves не cleared — overwritten on next register call.
}

}  // extern "C"
