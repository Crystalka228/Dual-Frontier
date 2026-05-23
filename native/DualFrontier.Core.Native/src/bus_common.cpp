#include "bus_native.h"
#include "bus_native_internal.h"

#include <mutex>

// Cross-tier bus surface — BusNative tier-state singleton accessors +
// functions that touch multiple tiers (df_bus_clear, df_bus_unsubscribe
// single-ID dispatch).
//
// Companion к bus_fast.cpp / bus_normal.cpp / bus_background.cpp per-tier
// C ABI implementations. Source split landed в A'.7.5 ε2 materializing the
// К-L15.1 «Three-tier independence» compile-time layer (parent A'.7.x γ4
// LOCKED 2-layer formulation: state + runtime; ε2 lands the third layer
// per source-level concern separation).

namespace dualfrontier {

// ─── Tier-state singletons ───────────────────────────────────────────────
// Each tier owns its own state; no shared mutex, no shared sequence counter.

FastTierState& BusNative::fast() {
    static FastTierState s;
    return s;
}

NormalTierState& BusNative::normal() {
    static NormalTierState s;
    return s;
}

BackgroundTierState& BusNative::background() {
    static BackgroundTierState s;
    return s;
}

} // namespace dualfrontier

using namespace dualfrontier;

extern "C" {

// ─── Unsubscribe (single-ID, decodes tier-bit к dispatch) ────────────────

DF_API int32_t df_bus_unsubscribe(df_bus_subscription_id sid) {
    TierTag tier_tag = decode_tier(sid);
    switch (tier_tag) {
        case TierTag::Fast: {
            auto& tier = BusNative::fast();
            std::lock_guard<std::mutex> lock(tier.mutex);
            return remove_by_id_locked(tier.subscribers, sid) ? 1 : 0;
        }
        case TierTag::Normal: {
            auto& tier = BusNative::normal();
            std::lock_guard<std::mutex> lock(tier.mutex);
            return remove_by_id_locked(tier.subscribers, sid) ? 1 : 0;
        }
        case TierTag::Background: {
            auto& tier = BusNative::background();
            std::lock_guard<std::mutex> lock(tier.mutex);
            return remove_by_id_locked(tier.subscribers, sid) ? 1 : 0;
        }
    }
    return 0;
}

// ─── Clear (all tiers, fixed-order lock fast → normal → background) ──────

DF_API void df_bus_clear(void) {
    auto& f = BusNative::fast();
    auto& n = BusNative::normal();
    auto& b = BusNative::background();
    // Fixed lock-ordering fast → normal → background prevents deadlock if
    // another thread happens к attempt cross-tier work concurrently.
    std::lock_guard<std::mutex> lf(f.mutex);
    std::lock_guard<std::mutex> ln(n.mutex);
    std::lock_guard<std::mutex> lb(b.mutex);
    f.subscribers.clear();
    f.next_seq = 1;
    n.subscribers.clear();
    n.pending.clear();
    n.next_seq = 1;
    b.subscribers.clear();
    b.pending.clear();
    b.next_seq = 1;
}

} // extern "C"
