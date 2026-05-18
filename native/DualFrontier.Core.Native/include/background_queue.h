#pragma once

#include <cstdint>

// K10.2 Item 30 — Background work queue + idle-slot scheduling.
//
// Background tier dispatch policy applied on top of bus_native's
// pending_background_ storage (Commit 4). Adds:
//   - Per-(type_id, coalesce_key) coalesce-on-publish (Q-N-34 author-control)
//   - Idle-slot dispatch: only invoked when scheduler reports available
//     CPU budget at phase boundary (Q-N-35 К10.2 default)
//   - Saturation strategy: drop-oldest с warning when size cap exceeded
//     (Q-N-36 К10.2 default; backpressure/expand deferred к К-extensions)
//   - Size cap: configurable shared pool, default 10MB hard cap; warn at 80%
//     (Q-N-45 К10.2 default)
//
// Per К-L14 + scope discipline: К10.2 lands functional Background tier с
// simplest viable defaults; refinement (per-mod budgets, NUMA-aware queuing)
// deferred к К-extensions, not omitted.
//
// Per К-L15: background dispatch is native authority. Item 27 managed bus
// facade routes к df_bus_publish_background (Commit 4) which delegates к
// bus's internal pending_background_ storage; this module adds the policy
// layer на top — coalesce at publish, dispatch on idle-slot.

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

// Saturation strategy enum per Q-N-36 К10.2 default + future extensions.
typedef enum {
    DF_BG_QUEUE_DROP_OLDEST  = 0,  // К10.2 default: oldest event dropped, warning logged
    DF_BG_QUEUE_BACKPRESSURE = 1,  // deferred к К-extensions
    DF_BG_QUEUE_EXPAND       = 2,  // deferred к К-extensions
} df_bg_queue_saturation_strategy;

// Configures the size cap and saturation strategy. К10.2 defaults:
//   max_bytes      = 10 * 1024 * 1024 (10 MB)
//   warn_threshold = 80% of max_bytes
//   strategy       = DF_BG_QUEUE_DROP_OLDEST
DF_API int32_t df_background_queue_configure(
    uint32_t  max_bytes,
    uint32_t  warn_threshold_bytes,
    int32_t   strategy);

// Idle-slot dispatch: invoked by scheduler at phase boundary с available
// CPU budget. Drains as many events as fit в budget, invoking the matching
// Background tier subscribers from bus_native. Returns count of events
// dispatched.
//
// `available_budget_micros` — CPU time available before next tick boundary;
// driver may stop dispatching mid-queue if budget exhausted.
DF_API int32_t df_background_queue_dispatch_idle_slot(uint64_t available_budget_micros);

// Diagnostic + saturation state read.
DF_API int32_t df_background_queue_size(uint32_t* out_event_count, uint32_t* out_bytes_used);

// Saturation warning counter (incremented each time drop-oldest applied).
DF_API int32_t df_background_queue_saturation_events(void);

// Test-only: forces immediate coalesce of pending events (e.g., merge
// (type_id, coalesce_key) duplicates). Production driver invokes coalesce
// at publish; this is для test scenarios that publish before coalesce policy
// is exercised by dispatch.
DF_API int32_t df_background_queue_force_coalesce(void);

// =========================================================================
// K10.2 Item 31 — Save-integrated storage (S3-Q3 untargeted persistence)
// =========================================================================
//
// Saves the pending background queue к a caller-provided buffer; loads from
// the same wire format. Saved events are available к subscribers in any
// future session (S3-Q4 untargeted persistence — events aren't queued
// specifically для unloading mod's subscribers; allows mod replacement
// pattern).
//
// Wire format (К10.2 schema version 1):
//   Header (12 bytes):
//     - uint32 schema_version            (currently 1)
//     - uint32 event_count
//     - uint32 total_payload_bytes
//   Event records (variable, repeated event_count times):
//     - uint32 type_id
//     - uint32 coalesce_key
//     - uint32 payload_size
//     - byte[] payload (payload_size bytes)
//
// Cross-version compatibility (per Q-N-44 + R-K10-6 risk mitigation):
// schema version field allows older readers к detect newer formats and
// emit a warning (graceful degradation rather than silent corruption).
// Future schema extensions (e.g. per-event timestamps) increment the
// version и add fields после existing fields.

// Computes the bytes required к serialize the current pending queue.
// Returns 1 on success; out_required_bytes filled с the buffer size needed.
DF_API int32_t df_background_queue_compute_save_size(uint32_t* out_required_bytes);

// Serializes the pending background queue к the supplied buffer. The buffer
// must be at least the size returned by df_background_queue_compute_save_size.
// Returns 1 on success; out_bytes_written filled.
DF_API int32_t df_background_queue_serialize(
    void*     out_buffer,
    uint32_t  buffer_size,
    uint32_t* out_bytes_written);

// Deserializes pending events from a buffer (typically loaded from save).
// Replaces the current pending queue (caller is responsible for calling
// during quiescent state per К-L18 — save/load lifecycle pauses simulation).
// Returns 1 on success; out_events_loaded filled с count.
// Returns 0 with no state mutation if buffer is malformed.
//
// Behavior on schema version mismatch:
//   - Schema version > current supported (1): returns 0 (caller logs warning).
//   - Schema version < current supported: not yet defined (К10.2 ships v1
//     as initial schema; downgrades are not supported and reserved by
//     R-K10-6 mitigation).
DF_API int32_t df_background_queue_deserialize(
    const void* buffer,
    uint32_t    buffer_size,
    uint32_t*   out_events_loaded);

// Schema version constant — К10.2 ships v1.
#define DF_BG_QUEUE_SCHEMA_VERSION 1u

#ifdef __cplusplus
}
#endif
