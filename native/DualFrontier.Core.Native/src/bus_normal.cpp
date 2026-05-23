#include "bus_native.h"
#include "bus_native_internal.h"

#include <mutex>
#include <unordered_map>
#include <utility>
#include <vector>

// Normal tier C ABI — batched dispatch via drain at phase boundary.
// Reuses df_managed_system_batch shape (К10.1 Item 15) for ABI symmetry.
// К-L7 atomic-from-observer preserved within batch boundary.
//
// Source split companion к bus_common.cpp / bus_fast.cpp / bus_background.cpp.
// Tier-state singleton accessed via BusNative::normal() (defined в bus_common.cpp).

using namespace dualfrontier;

extern "C" {

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

DF_API int32_t df_bus_unsubscribe_normal_by_mod(uint32_t mod_id) {
    auto& tier = BusNative::normal();
    std::lock_guard<std::mutex> lock(tier.mutex);
    return remove_by_mod_locked(tier.subscribers, mod_id);
}

DF_API int32_t df_bus_subscriber_count_normal(uint32_t type_id) {
    auto& tier = BusNative::normal();
    std::lock_guard<std::mutex> lock(tier.mutex);
    auto it = tier.subscribers.find(type_id);
    return it == tier.subscribers.end() ? 0 : static_cast<int32_t>(it->second.size());
}

} // extern "C"
