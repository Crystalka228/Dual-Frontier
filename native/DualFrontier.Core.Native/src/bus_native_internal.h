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

} // namespace dualfrontier
