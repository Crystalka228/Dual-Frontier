#pragma once

// Internal header — NOT in include/, NOT part of public API.
// Shared between bus_native.cpp (Item 26) и background_queue.cpp (Item 30).
//
// К10.2 Commit 5 introduces this internal header so Item 30 (background
// queue policy) can read+drain bus_native's pending_background_ queue +
// iterate Background tier subscribers without exposing BusNative class
// through the public C ABI surface.

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

class BusNative {
public:
    static BusNative& instance();

    df_bus_subscription_id subscribe_fast(
        uint32_t type_id, uint32_t mod_id,
        df_bus_fast_subscriber_fn cb, void* user_data);
    df_bus_subscription_id subscribe_normal(
        uint32_t type_id, uint32_t mod_id,
        df_bus_batched_subscriber_fn cb, void* user_data);
    df_bus_subscription_id subscribe_background(
        uint32_t type_id, uint32_t mod_id,
        df_bus_batched_subscriber_fn cb, void* user_data);

    int32_t publish_fast(uint32_t type_id, const void* payload, uint32_t payload_size);
    int32_t publish_normal(uint32_t type_id, const void* payload, uint32_t payload_size);
    int32_t publish_background(uint32_t type_id, const void* payload, uint32_t payload_size, uint32_t coalesce_key);
    int32_t drain_normal_batch();

    int32_t unsubscribe(df_bus_subscription_id sid);
    int32_t unsubscribe_fast_by_mod(uint32_t mod_id);
    int32_t unsubscribe_normal_by_mod(uint32_t mod_id);
    int32_t unsubscribe_background_by_mod(uint32_t mod_id);

    int32_t subscriber_count_fast(uint32_t type_id);
    int32_t subscriber_count_normal(uint32_t type_id);
    int32_t subscriber_count_background(uint32_t type_id);

    void clear();

    // Internal cross-TU access for Item 30 background queue policy.
    // Caller must hold mutex() during access; helper accessors below.
    std::mutex& mutex() noexcept { return mutex_; }
    std::vector<PendingBackgroundEventRecord>& pending_background_unsafe() { return pending_background_; }
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>>& background_subscribers_unsafe() {
        return background_;
    }

private:
    BusNative() = default;

    bool remove_from_fast(df_bus_subscription_id sid);
    bool remove_from_normal(df_bus_subscription_id sid);
    bool remove_from_background(df_bus_subscription_id sid);

    std::mutex mutex_;
    std::unordered_map<uint32_t, std::vector<FastSubscriberRecord>>    fast_;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>> normal_;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>> background_;
    std::vector<PendingNormalEventRecord>                              pending_normal_;
    std::vector<PendingBackgroundEventRecord>                          pending_background_;
    uint64_t next_seq_ = 1;
};

} // namespace dualfrontier
