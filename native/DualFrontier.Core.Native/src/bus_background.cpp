#include "bus_native.h"
#include "bus_native_internal.h"

#include <mutex>
#include <utility>

// Background tier C ABI — subscribe/publish/unsubscribe surface only.
// Dispatch policy + coalesce algorithm + save/load remain в
// background_queue.cpp (preserved distinct per A'.7.x §16.5 closure
// narrative: background_queue owns queue infrastructure as separate
// architectural concern from bus contract).
//
// Source split companion к bus_common.cpp / bus_fast.cpp / bus_normal.cpp.
// Tier-state singleton accessed via BusNative::background() (defined в bus_common.cpp).

using namespace dualfrontier;

extern "C" {

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

DF_API int32_t df_bus_unsubscribe_background_by_mod(uint32_t mod_id) {
    auto& tier = BusNative::background();
    std::lock_guard<std::mutex> lock(tier.mutex);
    return remove_by_mod_locked(tier.subscribers, mod_id);
}

DF_API int32_t df_bus_subscriber_count_background(uint32_t type_id) {
    auto& tier = BusNative::background();
    std::lock_guard<std::mutex> lock(tier.mutex);
    auto it = tier.subscribers.find(type_id);
    return it == tier.subscribers.end() ? 0 : static_cast<int32_t>(it->second.size());
}

} // extern "C"
