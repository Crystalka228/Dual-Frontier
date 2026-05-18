#pragma once

#include <cstdint>

// K10.2 Item 28 — Event type registry (tier-annotated).
//
// Per-event-type metadata for native bus dispatch (К-L15):
//   - tier (Fast / Normal / Background) declared at registration time
//   - payload size constraint (uint32_t)
//   - FQN (for capability token construction per S-LOCK-4)
//   - coalesce function pointer (Background tier only; NULL elsewhere)
//
// Stored separately from system graph + wake registry: bus dispatch logic
// (Item 26) reads tier from this registry to choose Fast / Normal / Background
// dispatch path. Capability registry (Item 27 + Item 29) reads tier to build
// per-FQN per-tier capability tokens.

namespace dualfrontier {

// Three-tier dispatch enum per К-L15 / S-LOCK-3:
//   Fast       — synchronous bypass, preemption-aware, ≤1ms latency target
//   Normal     — batched callback per-phase, standard subscriber contract
//   Background — coalesce + idle-slot dispatch, multi-tick acceptable
enum class BusTier : int32_t {
    Fast       = 0,
    Normal     = 1,
    Background = 2,
};

// Coalesce function pointer signature for Background tier events.
// Invoked at publish time when an event with the same (type_id, coalesce_key)
// already exists in the queue: `existing` is the queued event payload pointer,
// `new_event` is the newly published event payload. Implementation merges the
// two into `existing` in-place (e.g., accumulate counters, take max, last-write).
typedef void (*CoalesceFn)(void* existing, const void* new_event);

// Per-event-type metadata. POD layout for C ABI marshalling.
struct EventTypeMetadata {
    uint32_t   type_id;
    BusTier    tier;
    uint32_t   payload_size_bytes;
    const char* fqn;             // owned by caller; lifetime ≥ registry
    CoalesceFn coalesce_fn;       // nullable; required for Background tier
};

} // namespace dualfrontier

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

// C ABI surface — single global registry instance per process.
//
// Return codes (consistent with df_capi.h convention):
//   1 — success
//   0 — failure (already registered, not found, invalid args)
//
// Default tier semantics (per S-LOCK-4 backward compatibility):
// Events that never call df_event_type_registry_register default к Normal
// tier when looked up. The C# binding (EventTypeRegistry.GetOrAssignId<T>)
// registers с Normal tier when the type has no [EventTier] attribute.

DF_API int32_t df_event_type_registry_register(
    uint32_t type_id,
    int32_t  tier,                  // dualfrontier::BusTier value
    uint32_t payload_size_bytes,
    const char* fqn,                 // null-terminated UTF-8; stored by pointer
    void (*coalesce_fn)(void*, const void*));

DF_API int32_t df_event_type_registry_lookup(
    uint32_t type_id,
    int32_t* out_tier,
    uint32_t* out_payload_size_bytes,
    const char** out_fqn,
    void (**out_coalesce_fn)(void*, const void*));

DF_API int32_t df_event_type_registry_get_tier(
    uint32_t type_id,
    int32_t* out_tier);

DF_API int32_t df_event_type_registry_count(void);

DF_API void    df_event_type_registry_clear(void);

#ifdef __cplusplus
}
#endif
