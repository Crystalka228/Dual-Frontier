#pragma once

#include <cstdint>

// K10.3 v2 Item 33 — Pipeline depth mechanism (S8-Q1 + S8-Q2 lock).
//
// PipelineSlot state machine establishes sim-tick pipeline depth D=2 (default,
// configurable 1-3) per К-L16. Simulation thread allocates new slot at start of
// pipeline-managed tick; compute dispatches submitted к async compute queue (K-L19,
// V0.B); fence orchestration tracks slot transitions Empty→Dispatched→
// FenceCompleted→ReadableAsTail.
//
// К-L7.1 sub-invariant binding: FieldStorageSnapshot bound к slot via
// fields_snapshot_ptr; sim-thread reads of pipeline-managed fields see slot tail
// state (sim_tick - 1). One-tick lag bounded и deterministic. К-L7 atomic-from-
// observer preserved within slot boundary; cross-slot reads see different snapshots.
//
// S-LOCK-10/13 coexistence: V1's existing dispatch_compute_field synchronous path
// (per compute_dispatch.h sync model К-L7 atomic-from-observer) remains operational.
// Pipeline slot mechanism is opt-in для pipeline-managed dispatch consumers. К-L9
// «Vanilla = mods» — author choice per field.
//
// K-L18 forward dependency (Item 41): df_pipeline_is_quiescent consumed by mod
// unload primitive (mod_unload.h) к verify all slots quiescent (Empty or
// ReadableAsTail) before accepting mod operation.

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

// Configurable depth range per К-L16 (D=1-3, default 2).
#define DF_PIPELINE_DEFAULT_DEPTH 2
#define DF_PIPELINE_MAX_DEPTH 3

// Slot state machine per spec §3.10 Item 33 verbatim.
typedef enum {
    SlotState_Empty = 0,           // Initial state, no sim_tick assigned
    SlotState_Dispatched = 1,      // Sim thread dispatched compute work к GPU
    SlotState_FenceCompleted = 2,  // GPU finished, results available
    SlotState_ReadableAsTail = 3   // Display/sim thread reads from here
} SlotState;

// PipelineSlot struct verbatim from spec §3.10 Item 33.
//
// fields_snapshot_ptr: К-L7.1 binding subject — pipeline-managed FieldStorageSnapshot.
// compute_fence_handle: VkFence opaque (cast к/от VkFence в integration code).
// world_snapshot_ptr: NativeWorld snapshot pointer (К-L7.1 binding subject).
typedef struct {
    uint64_t sim_tick;
    void* world_snapshot_ptr;
    void* fields_snapshot_ptr;
    void* compute_fence_handle;
    int32_t state;  // SlotState enum value (int32 для C ABI portability)
} PipelineSlot;

// Initialize pipeline с specified depth (1-3 per К-L16 default 2).
// Returns 1 on success, 0 on failure (invalid depth, already initialized).
// Subsequent calls without explicit reset return 0 (idempotency through error).
DF_API int32_t df_pipeline_init(int32_t depth);

// Reset pipeline state (test/teardown convenience — empties all slots, depth=0).
DF_API void df_pipeline_reset(void);

// Get current configured depth. Returns 0 if не initialized.
DF_API int32_t df_pipeline_get_depth(int32_t* out_depth);

// Allocate next slot для simulation tick. Cycles through D slots.
// Returns pointer к slot on success (state transitions Empty/ReadableAsTail →
// Dispatched), null if no slot available (all D slots in flight — К-L16 backpressure).
DF_API int32_t df_pipeline_allocate_slot(uint64_t sim_tick, PipelineSlot** out_slot);

// Get slot at offset relative к current allocation cursor.
// slot_offset: 0 = current (most recently allocated), -1 = previous (sim-thread
// tail read per К-L7.1), -2..-D = display tail (К-L16).
// Returns 1 on success, 0 if offset out of range or no slot at that position.
DF_API int32_t df_pipeline_get_slot(int32_t slot_offset, PipelineSlot** out_slot);

// Bind VkFence opaque handle к slot после compute dispatch submitted.
// Caller responsible для actual VkQueueSubmit; this just stores handle для
// later vkGetFenceStatus polling via df_pipeline_check_fences.
DF_API int32_t df_pipeline_set_fence(PipelineSlot* slot, void* vk_fence_handle);

// Poll fences для all Dispatched slots; transitions slots Dispatched →
// FenceCompleted when fence signaled.
//
// K10.3 v2 Commit 3 scope: stub returning zero «no fences checked» — actual
// vkGetFenceStatus integration lands в Commit 4 (Phase.Compute) когда we have
// VulkanAttachment context. Until then, callers can directly call
// df_pipeline_force_fence_completed для testing state transitions.
DF_API int32_t df_pipeline_check_fences(int32_t* out_slots_transitioned);

// Test/integration helper: explicitly mark slot Dispatched → FenceCompleted.
// Used by test scenarios that don't have real VkFence handles; will also be
// callable from Commit 4 Phase.Compute integration as a transition primitive.
DF_API int32_t df_pipeline_force_fence_completed(PipelineSlot* slot);

// Atomic transition FenceCompleted → ReadableAsTail.
// Fires StateChangeWake per К10.3 v2 Item 37 (filter primitive integration —
// wired в Commit 7). Returns 1 on success, 0 if slot не в FenceCompleted state.
DF_API int32_t df_pipeline_transition_to_tail(PipelineSlot* slot);

// K-L18 quiescent state check (Item 41 forward consumer).
// All slots must be Empty или ReadableAsTail для quiescent state (no Dispatched
// slots = no in-flight compute work).
// Returns 1 if quiescent, 0 если any slot Dispatched/FenceCompleted, -1 if
// pipeline не initialized.
DF_API int32_t df_pipeline_is_quiescent(int32_t* out_is_quiescent);

// K10.3 v2 Item 34 — Pipeline drain/refill protocols.
//
// Save protocol per S8-Q1.5: snapshot display tick state (CurrentSimTick - D).
// Display already sees coherent world; pipeline drain не required at save time.
// Faster save (no waiting для in-flight compute completion).
//
// Pause protocol: natural convergence — sim thread completes current tick, no
// new dispatch. Pipeline depth naturally absorbs already-dispatched work.
// К-L18 quiescent state precondition (Item 41) verifies pipeline quiesced
// before mod operations.
//
// Load protocol: orderly refill — sim thread starts at saved tick, refills
// pipeline incrementally; display unblocks once D slots populated.
//
// Resume protocol: refill от pause point; sim thread resumes от last saved sim_tick.
//
// К10.3 v2 boundary: native serialize/deserialize land C ABI surface +
// state machine; managed-side Persistence integration (PipelineSlotSerializer
// + SaveSystem integration) deferred — SaveSystem currently stub
// (NotImplementedException). Full persistence wiring lands когда SaveSystem
// Phase 1/3 closes per its own roadmap.

// Snapshot serialization size constants. Snapshot captures display tick state:
// sim_tick (8) + state (4) + fields_snapshot_size hint (4) = 16 bytes per slot,
// plus pipeline depth (4) at header = 4 + D*16 bytes max.
//
// Note: fields_snapshot_ptr/world_snapshot_ptr/compute_fence_handle are runtime
// pointers — НЕ persisted (regenerated на load from saved sim_tick через
// re-dispatch). Serialization holds metadata только.
#define DF_PIPELINE_SNAPSHOT_HEADER_SIZE 4
#define DF_PIPELINE_SNAPSHOT_PER_SLOT_SIZE 16
#define DF_PIPELINE_SNAPSHOT_MAX_SIZE (DF_PIPELINE_SNAPSHOT_HEADER_SIZE + DF_PIPELINE_MAX_DEPTH * DF_PIPELINE_SNAPSHOT_PER_SLOT_SIZE)

// Serialize pipeline display state per S8-Q1.5. Writes depth + per-slot
// sim_tick + state. Returns bytes written, 0 on failure (buffer too small).
DF_API int32_t df_pipeline_serialize_display_state(
    void* buffer,
    int32_t buffer_size,
    int32_t* out_bytes_written);

// Deserialize pipeline display state per S8-Q1.5. Pipeline must be initialized
// с matching depth; slots restored к saved sim_tick + state. Returns 1 on
// success, 0 on schema mismatch / depth mismatch / malformed buffer.
DF_API int32_t df_pipeline_deserialize_display_state(
    const void* buffer,
    int32_t buffer_size);

// Pause protocol: natural convergence — no new allocations accepted while
// paused. Existing in-flight slots drain naturally via fence check + transition.
// К-L18 quiescent state precondition (Item 41) verifies pause completed.
DF_API int32_t df_pipeline_pause(void);

// Resume protocol: re-enables slot allocation after pause.
DF_API int32_t df_pipeline_resume(void);

// Query pause state. Returns 1 if paused, 0 if running, -1 if не initialized.
DF_API int32_t df_pipeline_is_paused(int32_t* out_is_paused);

// K10.3 v2 Item 37 — Filter primitive integration с pipeline slot transitions.
//
// К-L13 on-demand activation (К10.1 Item 3 + Item 17 state_change_filter)
// extended с pipeline slot transitions. Downstream pipeline-managed read systems
// can wake when fence completes на slot tail (drives sim-side reactivity post-
// GPU dispatch).
//
// К10.3 v2 boundary: lands wake fire counter + transition hook infrastructure.
// Full subscriber registry integration (subscribed system tracking + wake
// payload delivery) deferred к К-extensions when pipeline-managed wake
// consumers surface. K-L13 5-wake-type model unchanged; slot transitions
// are а pipeline lifecycle signal, not а standard wake type.
//
// Architectural rationale per К-L14: keep pipeline slot wake separate primitive
// rather than overloading state_change_filter с per-component-type semantics
// что awkwardly compose с slot-level transitions.

// Returns total count of slot transitions (FenceCompleted → ReadableAsTail)
// that fired the wake hook since pipeline init. Used by test scenarios + future
// observability (К-L13 diagnostic surface).
DF_API int32_t df_pipeline_get_wake_fire_count(int32_t* out_count);

// Reset wake fire counter (test convenience). Pipeline state unchanged.
DF_API void df_pipeline_reset_wake_fire_count(void);

// K-L7.1 sub-invariant — pipeline slot tail read API (S8-Q2 Pattern C).
//
// Convenience wrapper around df_pipeline_get_slot that extracts fields_snapshot_ptr
// + sim_tick from а slot at given offset. Used by pipeline-managed field
// consumers к read slot tail state per К-L7.1: «sim tick T+D reads dispatched-
// at-(T+D-1) state. One-tick lag from sim-perspective bounded и deterministic.»
//
// slot_offset semantics (matches df_pipeline_get_slot):
//   0  = current (most recently allocated)
//   -1 = sim-thread tail (К-L7.1 read pattern)
//   -2..-(D-1) = display tail (К-L16 governed)
//
// Validation: slot must be в ReadableAsTail or FenceCompleted state — reading
// от Dispatched slot returns 0 (fence не signaled). К-L7 atomic-from-observer
// preserved within slot boundary; cross-slot reads see different snapshots
// (К-L7.1).
//
// К10.3 v2 boundary: actual consumer integration (FieldHandle opt-in pattern,
// per spec §3.10 Item 36 RawTileField example) deferred к К-extensions when
// pipeline-managed field consumers surface. К10.3 v2 establishes API surface.
DF_API int32_t df_pipeline_read_slot_tail(
    int32_t slot_offset,
    void** out_field_snapshot,
    uint64_t* out_sim_tick);

#ifdef __cplusplus
}
#endif
