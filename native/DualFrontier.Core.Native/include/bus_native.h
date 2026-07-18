#pragma once

#include <cstdint>

#include "df_capi.h"  // df_managed_system_batch, df_managed_batch_fn

// K10.2 Item 26 — Native bus implementation (three-tier dispatcher).
//
// Three subscriber registries (one per tier) with distinct dispatch paths
// per К-L15:
//   - Fast       — synchronous bypass; subscriber callback invoked on publisher
//                  thread; ≤1ms latency target. Bounded exec / no blocking /
//                  no GC alloc contract enforced at Item 29 load-time.
//   - Normal     — batched callback per-phase; reuses Item 15 batched ABI
//                  pattern (df_managed_system_batch shape). К-L7 atomic-from-
//                  observer preserved within batch.
//   - Background — coalesce + idle-slot dispatch. Forward к background queue
//                  (Item 30, next commit) via df_background_queue_publish.
//                  К10.2 Commit 4 lands the dispatch hook; Commit 5 implements
//                  the queue.
//
// Subscriber identity: (mod_id, type_id, callback_ptr). Per-mod bulk
// unsubscribe required by Step 3.5 native unload primitive (Item 32, T1-T3
// per-tier subscription clearing).
//
// Native authority (per К-L15): bus type registry, subscriber registry,
// payload dispatch, wake firing, tier-based delivery semantics all native.
// Managed bus facade (Item 27) routes к this ABI but retains IModApi surface
// для К-L9 «Vanilla = mods» uniformity.

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

// Fast tier subscriber callback: invoked synchronously on the publisher thread.
// Contract: bounded execution (≤1ms target), no blocking, no GC allocation.
//
//   type_id      — event type (matches registration at Item 28)
//   payload      — caller-owned event payload (lifetime = call duration)
//   payload_size — bytes; matches event type registry payload_size_bytes
//   user_data    — opaque (managed adapter typically passes GCHandle)
typedef void (*df_bus_fast_subscriber_fn)(
    uint32_t    type_id,
    const void* payload,
    uint32_t    payload_size,
    void*       user_data);

// Normal/Background tier subscriber callback: invoked via batched dispatch.
// Reuses df_managed_system_batch shape (К10.1 Item 15) for ABI symmetry.
//
// The batch payload pointer is shared across all subscribers of (type_id);
// callbacks dispatched sequentially within the batch boundary.
typedef void (*df_bus_batched_subscriber_fn)(
    const df_managed_system_batch* batch);

// Subscription handle returned by subscribe; passed к unsubscribe.
// Encoding: high 8 bits = tier; low 56 bits = sequential subscription id.
typedef uint64_t df_bus_subscription_id;

// =========================================================================
// Publish API — one entry point per tier
// =========================================================================

// Fast tier publish: synchronous dispatch к all registered Fast tier
// subscribers for `type_id`. Returns count of subscribers invoked.
// Bounded exec contract is the subscriber's responsibility; bus does not
// time-box individual callbacks here (Item 29 runtime monitor measures
// latency separately).
DF_API int32_t df_bus_publish_fast(
    uint32_t    type_id,
    const void* payload,
    uint32_t    payload_size);

// Normal tier publish: queues the event for next phase boundary dispatch.
// Returns 1 on success, 0 if no subscribers registered (event dropped).
DF_API int32_t df_bus_publish_normal(
    uint32_t    type_id,
    const void* payload,
    uint32_t    payload_size);

// Background tier publish: forwards к background queue (Item 30) with the
// supplied coalesce key. Returns 1 on success. Coalesce function is read
// from Item 28 event type registry at dispatch time.
DF_API int32_t df_bus_publish_background(
    uint32_t    type_id,
    const void* payload,
    uint32_t    payload_size,
    uint32_t    coalesce_key);

// =========================================================================
// Subscribe API — one entry point per tier
// =========================================================================
//
// `mod_id`: opaque uint32_t mod identifier (used by per-mod bulk unsubscribe
// during Step 3.5 native unload primitive). Use 0 для Core/vanilla subscribers
// that should not be reaped by mod unload.

DF_API df_bus_subscription_id df_bus_subscribe_fast(
    uint32_t                    type_id,
    uint32_t                    mod_id,
    df_bus_fast_subscriber_fn   callback,
    void*                       user_data);

DF_API df_bus_subscription_id df_bus_subscribe_normal(
    uint32_t                       type_id,
    uint32_t                       mod_id,
    df_bus_batched_subscriber_fn   callback,
    void*                          user_data);

DF_API df_bus_subscription_id df_bus_subscribe_background(
    uint32_t                       type_id,
    uint32_t                       mod_id,
    df_bus_batched_subscriber_fn   callback,
    void*                          user_data);

// =========================================================================
// Unsubscribe API — single (delete subscription_id) + bulk-by-mod
// =========================================================================

DF_API int32_t df_bus_unsubscribe(df_bus_subscription_id subscription_id);

// Per-mod bulk unsubscribe (consumed by Item 32 native unload primitive).
// Each function returns count of subscriptions removed.
DF_API int32_t df_bus_unsubscribe_fast_by_mod(uint32_t mod_id);
DF_API int32_t df_bus_unsubscribe_normal_by_mod(uint32_t mod_id);
DF_API int32_t df_bus_unsubscribe_background_by_mod(uint32_t mod_id);

// =========================================================================
// Normal tier batch dispatch — called by scheduler at phase boundary
// =========================================================================
//
// Drains the per-type pending queue and invokes batched subscribers с the
// accumulated payload buffer. Caller (scheduler / test) owns timing.
// Returns count of batches dispatched (one per (type_id) с pending events).
DF_API int32_t df_bus_drain_normal_batch(void);

// =========================================================================
// Diagnostic surface
// =========================================================================

DF_API int32_t df_bus_subscriber_count_fast(uint32_t type_id);
DF_API int32_t df_bus_subscriber_count_normal(uint32_t type_id);
DF_API int32_t df_bus_subscriber_count_background(uint32_t type_id);

DF_API void    df_bus_clear(void);  // production teardown (EQ_A2 M8/D6) + test reset

#ifdef __cplusplus
}
#endif
