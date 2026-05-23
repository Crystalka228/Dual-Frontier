#include "bus_native.h"
#include "bus_native_internal.h"

#include <mutex>
#include <vector>

// Fast tier C ABI — synchronous bypass dispatch, no pending queue.
// Subscriber callback invoked on publisher thread; bounded execution
// contract (≤1ms target, no blocking, no GC alloc) is the subscriber's
// responsibility (Item 29 runtime monitor measures latency separately).
//
// Source split companion к bus_common.cpp / bus_normal.cpp / bus_background.cpp.
// Tier-state singleton accessed via BusNative::fast() (defined в bus_common.cpp).

using namespace dualfrontier;

extern "C" {

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
    // and (post-A'.7.x β4 state split) doesn't block Normal/Background either.
    // A Fast subscriber can safely publish к any tier without re-entrancy
    // deadlock on the shared mutex (which no longer exists).
    int32_t invoked = 0;
    for (const auto& sub : snapshot) {
        sub.callback(type_id, payload, payload_size, sub.user_data);
        ++invoked;
    }
    return invoked;
}

DF_API int32_t df_bus_unsubscribe_fast_by_mod(uint32_t mod_id) {
    auto& tier = BusNative::fast();
    std::lock_guard<std::mutex> lock(tier.mutex);
    return remove_by_mod_locked(tier.subscribers, mod_id);
}

DF_API int32_t df_bus_subscriber_count_fast(uint32_t type_id) {
    auto& tier = BusNative::fast();
    std::lock_guard<std::mutex> lock(tier.mutex);
    auto it = tier.subscribers.find(type_id);
    return it == tier.subscribers.end() ? 0 : static_cast<int32_t>(it->second.size());
}

} // extern "C"
