#pragma once

#include <cstdint>

// The df_managed_system_batch struct + df_managed_batch_fn typedef live in
// df_capi.h (single source for the C ABI types). Include here so the
// ManagedCallbackRegistry can use them.
#include "df_capi.h"

namespace dualfrontier {

// K10.1 Item 15 — Batched callback ABI (К-L12 cross-layer bridge).
//
// Native scheduler dispatches managed systems (mod systems + transitional Core
// systems) via batched reverse-P/Invoke. One C ABI boundary crossing per phase
// per origin, not per system — GC transition cost (~10-50ns) amortized across
// N systems в batch.
//
// Implementation: pointer + count + delta + GCHandle (caller-managed context).
// Native side does not own the GCHandle — managed adapter alloc()s before
// register and free()s on shutdown.
//
// Performance characteristics (К-L12 baseline):
//   - One reverse-P/Invoke per phase per managed-system-batch (≈1/phase)
//   - GC transition cost amortized across N systems в batch
//   - ReadOnlySpan from native pointer = zero-copy
//   - Per-tick cost: ~10 phases × ~30ns transition = ~300ns/tick at 30Hz
//     → ~9µs/sec, negligible.
//
// Constraints (per .NET 10 research, Lesson #7 verbatim):
//   - Callback method must be static
//   - All args blittable (pointer + primitives only)
//   - No generics
//   - No managed exceptions across boundary (try/catch absorbs at boundary)
//   - SuppressGCTransition forbidden for reverse P/Invoke
//   - GCHandle для managed instance state — Alloc() at registration, Free()
//     at shutdown

// Registry interface (caller-managed singleton matching real OS dispatcher).
class ManagedCallbackRegistry {
public:
    ManagedCallbackRegistry();
    ~ManagedCallbackRegistry();

    ManagedCallbackRegistry(const ManagedCallbackRegistry&) = delete;
    ManagedCallbackRegistry& operator=(const ManagedCallbackRegistry&) = delete;

    // Register the managed batch callback. Returns true (overwrites any prior).
    bool register_callback(df_managed_batch_fn cb, void* user_data);

    // Dispatch a batch (called by the native scheduler on phase boundary or by
    // tests directly). Returns 1 on success, 0 if no callback registered.
    int32_t dispatch_batch(const df_managed_system_batch* batch);

    // Diagnostic: 1 if a callback is registered, 0 otherwise.
    [[nodiscard]] int32_t has_callback() const noexcept;

    void clear() noexcept;

private:
    df_managed_batch_fn callback_ = nullptr;
    void* user_data_ = nullptr;
};

ManagedCallbackRegistry& default_managed_callback_registry();

} // namespace dualfrontier
