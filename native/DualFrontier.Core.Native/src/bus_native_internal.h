#pragma once

// Internal header — NOT in include/, NOT part of public API.
// Shared between bus_native.cpp (Item 26) и background_queue.cpp (Item 30).
//
// 2026-05-21 refactor: state split per tier.
//   Each of Fast / Normal / Background now owns its own mutex, sequence
//   counter, subscriber map (and pending queue where applicable). Closes
//   the 48-way mutex contention observed at extreme scale (15 M events /
//   16 producers × 3 tiers) and the cross-tier re-entrancy hazard
//   (a Fast subscriber publishing a Normal event used to attempt
//   re-acquisition of the single non-recursive mutex → UB / deadlock).
//
// Public C ABI (bus_native.h) and subscription-ID wire format (56-bit
// per-tier seq + 8-bit tier high byte) are unchanged. The split is purely
// internal; managed callers, save format and selftest see no diff.

#include "bus_native.h"

#include <algorithm>
#include <mutex>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

struct FastSubscriberRecord {
    df_bus_subscription_id    id;
    uint32_t                  mod_id;
    uint32_t                  type_id;
    df_bus_fast_subscriber_fn callback;
    void*                     user_data;
};

struct BatchedSubscriberRecord {
    df_bus_subscription_id       id;
    uint32_t                     mod_id;
    uint32_t                     type_id;
    df_bus_batched_subscriber_fn callback;
    void*                        user_data;
};

struct PendingNormalEventRecord {
    uint32_t              type_id;
    std::vector<uint8_t>  payload;
};

struct PendingBackgroundEventRecord {
    uint32_t              type_id;
    uint32_t              coalesce_key;
    std::vector<uint8_t>  payload;
};

// Fast tier — synchronous dispatch, no pending queue.
struct FastTierState {
    std::mutex                                                       mutex;
    uint64_t                                                         next_seq = 1;
    std::unordered_map<uint32_t, std::vector<FastSubscriberRecord>>  subscribers;
};

// Normal tier — batched dispatch via drain at phase boundary.
struct NormalTierState {
    std::mutex                                                         mutex;
    uint64_t                                                           next_seq = 1;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>> subscribers;
    std::vector<PendingNormalEventRecord>                              pending;
};

// Background tier — coalesced idle-slot dispatch (policy layer in
// background_queue.cpp owns the dispatch trigger + coalesce).
struct BackgroundTierState {
    std::mutex                                                         mutex;
    uint64_t                                                           next_seq = 1;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>> subscribers;
    std::vector<PendingBackgroundEventRecord>                          pending;
};

// Tier-state accessors — each returns a process-singleton instance of the
// per-tier state. Replaces the previous BusNative::instance() monolithic
// singleton. background_queue.cpp uses background() directly instead of
// reaching into private fields via *_unsafe() accessors.
class BusNative {
public:
    static FastTierState&       fast();
    static NormalTierState&     normal();
    static BackgroundTierState& background();
};

// ─── Tier-bit primitives (К-L15.1 runtime layer) ────────────────────────
// Subscription IDs encode (tier_tag : 8 high bits) | (per-tier_seq : 56 low bits).
// Shared by per-tier subscribe (encode) и by df_bus_unsubscribe single-ID
// dispatch (decode). Migrated к internal header в A'.7.5 ε1 — used across
// 4 per-tier .cpp TUs after the bus_native.cpp source split.
constexpr uint64_t TIER_SHIFT = 56;
constexpr uint64_t TIER_MASK  = uint64_t{0xFF} << TIER_SHIFT;
constexpr uint64_t ID_MASK    = (uint64_t{1} << TIER_SHIFT) - 1;

enum class TierTag : uint8_t {
    Fast       = 0,
    Normal     = 1,
    Background = 2,
};

constexpr df_bus_subscription_id encode_id(TierTag tier, uint64_t seq) {
    return (static_cast<uint64_t>(tier) << TIER_SHIFT) | (seq & ID_MASK);
}

constexpr TierTag decode_tier(df_bus_subscription_id sid) {
    return static_cast<TierTag>((sid & TIER_MASK) >> TIER_SHIFT);
}

// ─── Per-tier subscriber removal templates ──────────────────────────────
// Generic over FastSubscriberRecord / BatchedSubscriberRecord per tier.
// Caller MUST hold the corresponding tier mutex.
template<typename Record>
bool remove_by_id_locked(std::unordered_map<uint32_t, std::vector<Record>>& subs, df_bus_subscription_id sid) {
    for (auto& [type_id, vec] : subs) {
        auto it = std::find_if(vec.begin(), vec.end(),
            [sid](const Record& s) { return s.id == sid; });
        if (it != vec.end()) { vec.erase(it); return true; }
    }
    return false;
}

template<typename Record>
int32_t remove_by_mod_locked(std::unordered_map<uint32_t, std::vector<Record>>& subs, uint32_t mod_id) {
    int32_t removed = 0;
    for (auto& [type_id, vec] : subs) {
        auto before = vec.size();
        vec.erase(std::remove_if(vec.begin(), vec.end(),
            [mod_id](const Record& s) { return s.mod_id == mod_id; }),
            vec.end());
        removed += static_cast<int32_t>(before - vec.size());
    }
    return removed;
}

} // namespace dualfrontier
