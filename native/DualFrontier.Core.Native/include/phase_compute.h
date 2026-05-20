#pragma once

#include <cstdint>

#include "pipeline_slot.h"

// K10.3 v2 Item 35 — Phase.Compute scheduler integration.
//
// Introduces Phase enum (Update / Compute / Display) сейчас absent от K10.1+K10.2
// native scheduler (existing phase_barrier.h has BarrierType for inter-phase
// dependency types; this header introduces the named phases). Phase.Compute
// runs between Phase.Update (sim writes) и Phase.Display (display reads slot
// tail) для pipeline-managed compute dispatches per К-L16.
//
// VkQueueSubmit batching: all pipeline-managed compute dispatches collected
// per tick coalesce into single VkQueueSubmit call к async compute queue
// (К-L19 V0.B). Per Prediction 12: ~5-10μs × N submits → single submit per
// tick = ~50-100μs savings at ~10 active dispatch systems.
//
// S-LOCK-13 coexistence: V1's existing dispatch_compute_field (compute_dispatch.h)
// synchronous path remains operational и orthogonal. Phase.Compute is alternative
// dispatch path для pipeline-managed consumers; V1 sync consumers experience
// no behavior change. Future К-extensions могут deprecate sync path if
// pipeline-managed becomes universal.
//
// К10.3 v2 boundary: K10.3 v2 establishes Phase.Compute infrastructure +
// VkQueueSubmit batching mechanism. Actual pipeline-managed compute consumers
// (mods, vanilla systems opting in) outside К10.3 v2 scope.

#ifdef __cplusplus
extern "C" {
#endif

#if defined(_WIN32)
    #if defined(DF_NATIVE_BUILDING_DLL)
        #define DF_API __declspec(dllexport)
    #else
        #define DF_API __declspec(dllimport)
    #endif
#else
    #define DF_API __attribute__((visibility("default")))
#endif

// Phase enum — К10.3 v2 introduces named phases для pipeline depth lifecycle.
// Existing K10.1 scheduler uses topological-order phases without named taxonomy;
// Phase.Compute attaches к the scheduler's tick lifecycle as а distinct
// architectural slot между Update (sim) и Display (render).
typedef enum {
    Phase_Update = 0,    // Sim writes (existing K10.1+K10.2 scheduling)
    Phase_Compute = 1,   // K10.3 v2 NEW: GPU compute dispatches per pipeline slot
    Phase_Display = 2,   // Display reads slot tail (existing render path)
} Phase;

// Maximum compute dispatches batched в single Phase.Compute submission.
// Per К-L14 architectural cleanliness — large enough к not constrain consumers,
// small enough к keep per-tick allocation bounded. Configurable later если needed.
#define DF_PHASE_COMPUTE_MAX_DISPATCHES_PER_TICK 256

// Compute dispatch registration. Called by pipeline-managed consumers during
// system update (Phase.Update). Dispatches accumulate в registry, then submitted
// as single batch by df_scheduler_dispatch_phase_compute at Phase.Compute boundary.
//
// pipeline_handle: VkPipeline opaque (cast от void* in implementation).
// descriptor_set: VkDescriptorSet opaque.
// dispatch_x/y/z: vkCmdDispatch group counts.
//
// Returns 1 on success, 0 if registry full (per-tick dispatch limit exceeded —
// caller responsibility к surface or split).
DF_API int32_t df_scheduler_register_compute_dispatch(
    PipelineSlot* slot,
    void* pipeline_handle,
    void* descriptor_set,
    int32_t dispatch_x,
    int32_t dispatch_y,
    int32_t dispatch_z);

// Query count of dispatches registered against а slot since allocation.
DF_API int32_t df_scheduler_compute_dispatch_count(PipelineSlot* slot);

// Phase.Compute dispatch entry — invoked by scheduler at Phase.Compute boundary
// per tick. Coalesces accumulated dispatches into single VkQueueSubmit к async
// compute queue (К-L19 V0.B); sets slot fence (df_pipeline_set_fence) для later
// polling by df_pipeline_check_fences.
//
// K10.3 v2 Commit 4 scope: stub implementation — accumulates dispatch records
// + reports count, но actual VkQueueSubmit deferred к V-cycle integration when
// async compute queue handle wired через VulkanAttachment. Returns 1 if dispatch
// records consistent (count matches slot state); 0 на slot mismatch.
DF_API int32_t df_scheduler_dispatch_phase_compute(PipelineSlot* slot);

// VkQueueSubmit batching helper — exposes raw batch для external integration
// (V-cycle / К-extensions wiring). Returns count of dispatch records in batch.
// K10.3 v2: caller typically не direct-invoke this; use dispatch_phase_compute.
DF_API int32_t df_scheduler_submit_compute_batch(
    PipelineSlot* slot,
    void* async_compute_queue);

// Reset compute dispatch registry (test/teardown convenience).
// Clears all per-slot dispatch records без affecting slot state machine.
DF_API void df_scheduler_phase_compute_reset(void);

#ifdef __cplusplus
}
#endif
