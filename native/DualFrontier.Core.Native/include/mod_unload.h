#pragma once

#include <cstdint>

// K10.2 Item 32 — Native unload primitive (S3-Q1 L3 layering, S3-Q6 single
// primitive contract). Encapsulates T0-T7 internal sequence:
//   T0: Lock subscriber registries (acquire scheduler critical section)
//   T1: Fast tier — clear subscriptions + drop in-flight events
//   T2: Normal tier — drain current batch к commit boundary, then clear subs
//   T3: Background tier — clear subscriber registry; queue contents untouched
//       (S3-Q3/Q4 untargeted persistence — events available для future subs)
//   T4: Revoke fast/background tier capabilities per FQN (К10.3 wire-up)
//   T5: Clear ShmWriter/ShmReader registrations + CPU affinity (К10.3 wire-up)
//   T6: Clear wake registry subscriptions (5 wake types, orderly per Q-N-48)
//       (К10.3 wire-up — К10.2 lands the slot)
//   T7: Unregister system access declarations (К10.3 wire-up)
//
// К-L18 quiescent state precondition: primitive returns error if simulation
// thread не paused, OR (К10.3 v2 Item 41) if pipeline slots not quiescent
// (any slot Dispatched/FenceCompleted = in-flight compute work). К10.2
// landed the sim-paused stub; К10.3 v2 Item 41 extends с pipeline quiescence
// verification per К-L18 spec verbatim («pipeline slots quiescent (all fences
// completed); no concurrent compute dispatches in-flight»). К10.3 v2 Item 42
// lands UI helper integration enforcing the precondition (SimulationStateController
// + ModMenuController pause hook).

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

// ModUnloadResult struct verbatim from K10.2 spec §3.9 Item 32.
//
// Fixed-size error_messages array (8 messages × 256 bytes each = 2KB)
// trades flexibility for C ABI marshalling simplicity. Beyond 8 errors,
// error_count overflows и individual messages are silently dropped
// (К10.2 К-L14 default: error reporting is primitive, not exhaustive
// logging — fault handler integration К-extensions).
typedef struct {
    int32_t success;  // 1 = T0-T7 sequence completed; 0 = precondition violation or fault

    // Fast tier
    int32_t fast_subscriptions_cleared;
    int32_t fast_in_flight_dropped;

    // Normal tier
    int32_t normal_subscriptions_cleared;
    int32_t normal_events_drained;
    int32_t normal_batch_commit_completed;

    // Background tier
    int32_t background_subscriptions_cleared;
    int32_t background_events_preserved;
    int32_t background_subscriber_count_remaining;

    // Capabilities (К10.3 wire-up)
    int32_t capabilities_revoked;

    // Wake registry (К10.3 wire-up)
    int32_t wake_subscriptions_cleared;

    // Errors (fixed-size array для C ABI compatibility)
    char    error_messages[8][256];
    int32_t error_count;
} ModUnloadResult;

// Quiescent state guard (К-L18). К10.2 default: sim is treated as paused
// (suitable for tests + Step 3.5 invocation where ModIntegrationPipeline
// holds _isRunning=false check before calling unload). К10.3 wire-up
// connects this к the actual sim thread state from settings menu integration.
DF_API int32_t df_scheduler_set_sim_paused(int32_t paused);
DF_API int32_t df_scheduler_is_sim_paused(void);

// Single primitive contract per S3-Q6 — Item 32 encapsulates the T0-T7
// sequence behind one entry point.
DF_API int32_t df_scheduler_unload_mod_native_state(
    uint32_t        mod_id,
    ModUnloadResult* out_result);

#ifdef __cplusplus
}
#endif
