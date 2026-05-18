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

#ifdef __cplusplus
}
#endif
