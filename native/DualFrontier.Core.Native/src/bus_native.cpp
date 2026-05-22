#include "bus_native.h"
#include "bus_native_internal.h"

#include <algorithm>
#include <cstring>

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

// ─── Subscribe ───────────────────────────────────────────────────────────

DF_API df_bus_subscription_id df_bus_subscribe_fast(
    uint32_t type_id, uint32_t mod_id,
    df_bus_fast_subscriber_fn cb, void* user_data)
{
    auto& tier = BusNative::fast();
    std::lock_guard<std::mutex> lock(tier.mutex);
    df_bus_subscription_id id = encode_id(TierTag::Fast, tier.next_seq++);
    tier.subscribers[type_id].push_back(FastSubscriberRecord{id, mod_id, type_id, cb, user_data});
    return id;
}

DF_API df_bus_subscription_id df_bus_subscribe_normal(
    uint32_t type_id, uint32_t mod_id,
    df_bus_batched_subscriber_fn cb, void* user_data)
{
    auto& tier = BusNative::normal();
    std::lock_guard<std::mutex> lock(tier.mutex);
    df_bus_subscription_id id = encode_id(TierTag::Normal, tier.next_seq++);
    tier.subscribers[type_id].push_back(BatchedSubscriberRecord{id, mod_id, type_id, cb, user_data});
    return id;
}

DF_API df_bus_subscription_id df_bus_subscribe_background(
    uint32_t type_id, uint32_t mod_id,
    df_bus_batched_subscriber_fn cb, void* user_data)
{
    auto& tier = BusNative::background();
    std::lock_guard<std::mutex> lock(tier.mutex);
    df_bus_subscription_id id = encode_id(TierTag::Background, tier.next_seq++);
    tier.subscribers[type_id].push_back(BatchedSubscriberRecord{id, mod_id, type_id, cb, user_data});
    return id;
}

// ─── Publish ─────────────────────────────────────────────────────────────

DF_API int32_t df_bus_publish_fast(
    uint32_t type_id, const void* payload, uint32_t payload_size)
{
    auto& tier = BusNative::fast();
    std::vector<FastSubscriberRecord> snapshot;
    {
        std::lock_guard<std::mutex> lock(tier.mutex);
        auto it = tier.subscribers.find(type_id);
        if (it == tier.subscribers.end()) return 0;
        snapshot = it->second;
    }
    // Callbacks fire WITHOUT holding the Fast mutex — synchronous dispatch
    // is single-thread per call but doesn't block other Fast publishers,
    // and (post-split) doesn't block Normal/Background either. A Fast
    // subscriber can now safely publish to any tier without re-entrancy
    // deadlock on the shared mutex (which no longer exists).
    int32_t invoked = 0;
    for (const auto& sub : snapshot) {
        sub.callback(type_id, payload, payload_size, sub.user_data);
        ++invoked;
    }
    return invoked;
}

DF_API int32_t df_bus_publish_normal(
    uint32_t type_id, const void* payload, uint32_t payload_size)
{
    auto& tier = BusNative::normal();
    std::lock_guard<std::mutex> lock(tier.mutex);
    if (tier.subscribers.find(type_id) == tier.subscribers.end()) return 0;
    PendingNormalEventRecord ev;
    ev.type_id = type_id;
    ev.payload.assign(
        static_cast<const uint8_t*>(payload),
        static_cast<const uint8_t*>(payload) + payload_size);
    tier.pending.push_back(std::move(ev));
    return 1;
}

DF_API int32_t df_bus_publish_background(
    uint32_t type_id, const void* payload, uint32_t payload_size, uint32_t coalesce_key)
{
    auto& tier = BusNative::background();
    std::lock_guard<std::mutex> lock(tier.mutex);
    PendingBackgroundEventRecord ev;
    ev.type_id      = type_id;
    ev.coalesce_key = coalesce_key;
    ev.payload.assign(
        static_cast<const uint8_t*>(payload),
        static_cast<const uint8_t*>(payload) + payload_size);
    tier.pending.push_back(std::move(ev));
    return 1;
}

// ─── Drain (Normal) ──────────────────────────────────────────────────────

DF_API int32_t df_bus_drain_normal_batch(void) {
    auto& tier = BusNative::normal();
    std::vector<PendingNormalEventRecord> pending;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>> snapshot;
    {
        std::lock_guard<std::mutex> lock(tier.mutex);
        pending.swap(tier.pending);
        snapshot = tier.subscribers;
    }
    if (pending.empty()) return 0;

    int32_t batches = 0;
    for (const auto& ev : pending) {
        auto it = snapshot.find(ev.type_id);
        if (it == snapshot.end()) continue;
        for (const auto& sub : it->second) {
            df_managed_system_batch batch{};
            batch.system_ids = nullptr;
            batch.count      = 1;
            batch.delta      = 0.0f;
            batch.user_data  = sub.user_data;
            sub.callback(&batch);
            ++batches;
        }
    }
    return batches;
}

// ─── Unsubscribe ─────────────────────────────────────────────────────────

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

DF_API int32_t df_bus_unsubscribe_fast_by_mod(uint32_t mod_id) {
    auto& tier = BusNative::fast();
    std::lock_guard<std::mutex> lock(tier.mutex);
    return remove_by_mod_locked(tier.subscribers, mod_id);
}

DF_API int32_t df_bus_unsubscribe_normal_by_mod(uint32_t mod_id) {
    auto& tier = BusNative::normal();
    std::lock_guard<std::mutex> lock(tier.mutex);
    return remove_by_mod_locked(tier.subscribers, mod_id);
}

DF_API int32_t df_bus_unsubscribe_background_by_mod(uint32_t mod_id) {
    auto& tier = BusNative::background();
    std::lock_guard<std::mutex> lock(tier.mutex);
    return remove_by_mod_locked(tier.subscribers, mod_id);
}

// ─── Subscriber count ────────────────────────────────────────────────────

DF_API int32_t df_bus_subscriber_count_fast(uint32_t type_id) {
    auto& tier = BusNative::fast();
    std::lock_guard<std::mutex> lock(tier.mutex);
    auto it = tier.subscribers.find(type_id);
    return it == tier.subscribers.end() ? 0 : static_cast<int32_t>(it->second.size());
}

DF_API int32_t df_bus_subscriber_count_normal(uint32_t type_id) {
    auto& tier = BusNative::normal();
    std::lock_guard<std::mutex> lock(tier.mutex);
    auto it = tier.subscribers.find(type_id);
    return it == tier.subscribers.end() ? 0 : static_cast<int32_t>(it->second.size());
}

DF_API int32_t df_bus_subscriber_count_background(uint32_t type_id) {
    auto& tier = BusNative::background();
    std::lock_guard<std::mutex> lock(tier.mutex);
    auto it = tier.subscribers.find(type_id);
    return it == tier.subscribers.end() ? 0 : static_cast<int32_t>(it->second.size());
}

// ─── Clear (all tiers, atomic per-tier) ──────────────────────────────────

DF_API void df_bus_clear(void) {
    auto& f = BusNative::fast();
    auto& n = BusNative::normal();
    auto& b = BusNative::background();
    // Fixed lock-ordering fast → normal → background prevents deadlock if
    // another thread happens to attempt cross-tier work concurrently.
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
