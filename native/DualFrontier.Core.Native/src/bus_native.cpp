#include "bus_native.h"

#include <algorithm>
#include <cstring>
#include <mutex>
#include <unordered_map>
#include <vector>

namespace dualfrontier {
namespace {

constexpr uint64_t TIER_SHIFT = 56;
constexpr uint64_t TIER_MASK  = uint64_t{0xFF} << TIER_SHIFT;
constexpr uint64_t ID_MASK    = (uint64_t{1} << TIER_SHIFT) - 1;

enum class Tier : uint8_t {
    Fast       = 0,
    Normal     = 1,
    Background = 2,
};

constexpr df_bus_subscription_id encode_id(Tier tier, uint64_t seq) {
    return (static_cast<uint64_t>(tier) << TIER_SHIFT) | (seq & ID_MASK);
}

constexpr Tier decode_tier(df_bus_subscription_id sid) {
    return static_cast<Tier>((sid & TIER_MASK) >> TIER_SHIFT);
}

struct FastSubscriber {
    df_bus_subscription_id    id;
    uint32_t                  mod_id;
    uint32_t                  type_id;
    df_bus_fast_subscriber_fn callback;
    void*                     user_data;
};

struct BatchedSubscriber {
    df_bus_subscription_id       id;
    uint32_t                     mod_id;
    uint32_t                     type_id;
    df_bus_batched_subscriber_fn callback;
    void*                        user_data;
};

// Pending Normal tier event — copied payload until phase-boundary drain.
struct PendingNormalEvent {
    uint32_t              type_id;
    std::vector<uint8_t>  payload;
};

// Pending Background tier event — Item 30 (Commit 5) will add coalesce
// semantics + idle-slot dispatch policy on top of this same storage.
struct PendingBackgroundEvent {
    uint32_t              type_id;
    uint32_t              coalesce_key;
    std::vector<uint8_t>  payload;
};

class BusNative {
public:
    static BusNative& instance() {
        static BusNative inst;
        return inst;
    }

    df_bus_subscription_id subscribe_fast(
        uint32_t type_id, uint32_t mod_id,
        df_bus_fast_subscriber_fn cb, void* user_data)
    {
        std::lock_guard<std::mutex> lock(mutex_);
        df_bus_subscription_id id = encode_id(Tier::Fast, next_seq_++);
        fast_[type_id].push_back(FastSubscriber{id, mod_id, type_id, cb, user_data});
        return id;
    }

    df_bus_subscription_id subscribe_normal(
        uint32_t type_id, uint32_t mod_id,
        df_bus_batched_subscriber_fn cb, void* user_data)
    {
        std::lock_guard<std::mutex> lock(mutex_);
        df_bus_subscription_id id = encode_id(Tier::Normal, next_seq_++);
        normal_[type_id].push_back(BatchedSubscriber{id, mod_id, type_id, cb, user_data});
        return id;
    }

    df_bus_subscription_id subscribe_background(
        uint32_t type_id, uint32_t mod_id,
        df_bus_batched_subscriber_fn cb, void* user_data)
    {
        std::lock_guard<std::mutex> lock(mutex_);
        df_bus_subscription_id id = encode_id(Tier::Background, next_seq_++);
        background_[type_id].push_back(BatchedSubscriber{id, mod_id, type_id, cb, user_data});
        return id;
    }

    int32_t publish_fast(uint32_t type_id, const void* payload, uint32_t payload_size) {
        // Snapshot subscribers under lock; invoke without holding lock.
        // К-L7 atomic-from-observer applies per-call (subscriber sees consistent
        // event payload), not transitively across the snapshot.
        std::vector<FastSubscriber> snapshot;
        {
            std::lock_guard<std::mutex> lock(mutex_);
            auto it = fast_.find(type_id);
            if (it == fast_.end()) return 0;
            snapshot = it->second;
        }
        int32_t invoked = 0;
        for (const auto& sub : snapshot) {
            sub.callback(type_id, payload, payload_size, sub.user_data);
            ++invoked;
        }
        return invoked;
    }

    int32_t publish_normal(uint32_t type_id, const void* payload, uint32_t payload_size) {
        std::lock_guard<std::mutex> lock(mutex_);
        if (normal_.find(type_id) == normal_.end()) return 0;
        PendingNormalEvent ev;
        ev.type_id = type_id;
        ev.payload.assign(
            static_cast<const uint8_t*>(payload),
            static_cast<const uint8_t*>(payload) + payload_size);
        pending_normal_.push_back(std::move(ev));
        return 1;
    }

    int32_t drain_normal_batch() {
        std::vector<PendingNormalEvent> pending;
        std::unordered_map<uint32_t, std::vector<BatchedSubscriber>> snapshot;
        {
            std::lock_guard<std::mutex> lock(mutex_);
            pending.swap(pending_normal_);
            snapshot = normal_;
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

    int32_t publish_background(uint32_t type_id, const void* payload, uint32_t payload_size, uint32_t coalesce_key) {
        // К10.2 Commit 4: simple FIFO store. Coalesce semantics + idle-slot
        // dispatch policy added in Commit 5 (Item 30) on top of the same
        // pending_background_ queue.
        std::lock_guard<std::mutex> lock(mutex_);
        PendingBackgroundEvent ev;
        ev.type_id      = type_id;
        ev.coalesce_key = coalesce_key;
        ev.payload.assign(
            static_cast<const uint8_t*>(payload),
            static_cast<const uint8_t*>(payload) + payload_size);
        pending_background_.push_back(std::move(ev));
        return 1;
    }

    int32_t unsubscribe(df_bus_subscription_id sid) {
        std::lock_guard<std::mutex> lock(mutex_);
        Tier tier = decode_tier(sid);
        switch (tier) {
            case Tier::Fast:       return remove_from_fast(sid) ? 1 : 0;
            case Tier::Normal:     return remove_from_normal(sid) ? 1 : 0;
            case Tier::Background: return remove_from_background(sid) ? 1 : 0;
        }
        return 0;
    }

    int32_t unsubscribe_fast_by_mod(uint32_t mod_id) {
        std::lock_guard<std::mutex> lock(mutex_);
        int32_t removed = 0;
        for (auto& [type_id, subs] : fast_) {
            auto before = subs.size();
            subs.erase(std::remove_if(subs.begin(), subs.end(),
                [mod_id](const FastSubscriber& s) { return s.mod_id == mod_id; }),
                subs.end());
            removed += static_cast<int32_t>(before - subs.size());
        }
        return removed;
    }

    int32_t unsubscribe_normal_by_mod(uint32_t mod_id) {
        std::lock_guard<std::mutex> lock(mutex_);
        int32_t removed = 0;
        for (auto& [type_id, subs] : normal_) {
            auto before = subs.size();
            subs.erase(std::remove_if(subs.begin(), subs.end(),
                [mod_id](const BatchedSubscriber& s) { return s.mod_id == mod_id; }),
                subs.end());
            removed += static_cast<int32_t>(before - subs.size());
        }
        return removed;
    }

    int32_t unsubscribe_background_by_mod(uint32_t mod_id) {
        std::lock_guard<std::mutex> lock(mutex_);
        int32_t removed = 0;
        for (auto& [type_id, subs] : background_) {
            auto before = subs.size();
            subs.erase(std::remove_if(subs.begin(), subs.end(),
                [mod_id](const BatchedSubscriber& s) { return s.mod_id == mod_id; }),
                subs.end());
            removed += static_cast<int32_t>(before - subs.size());
        }
        return removed;
    }

    int32_t subscriber_count_fast(uint32_t type_id) {
        std::lock_guard<std::mutex> lock(mutex_);
        auto it = fast_.find(type_id);
        return it == fast_.end() ? 0 : static_cast<int32_t>(it->second.size());
    }

    int32_t subscriber_count_normal(uint32_t type_id) {
        std::lock_guard<std::mutex> lock(mutex_);
        auto it = normal_.find(type_id);
        return it == normal_.end() ? 0 : static_cast<int32_t>(it->second.size());
    }

    int32_t subscriber_count_background(uint32_t type_id) {
        std::lock_guard<std::mutex> lock(mutex_);
        auto it = background_.find(type_id);
        return it == background_.end() ? 0 : static_cast<int32_t>(it->second.size());
    }

    // Internal handle для Item 30 background queue (Commit 5) к access pending
    // events with coalesce semantics. Single-writer / single-reader contract.
    std::vector<PendingBackgroundEvent>& mutable_pending_background_unsafe() {
        return pending_background_;
    }
    std::mutex& mutex() { return mutex_; }

    std::unordered_map<uint32_t, std::vector<BatchedSubscriber>>& background_subscribers_unsafe() {
        return background_;
    }

    void clear() {
        std::lock_guard<std::mutex> lock(mutex_);
        fast_.clear();
        normal_.clear();
        background_.clear();
        pending_normal_.clear();
        pending_background_.clear();
        next_seq_ = 1;
    }

private:
    BusNative() = default;

    bool remove_from_fast(df_bus_subscription_id sid) {
        for (auto& [type_id, subs] : fast_) {
            auto it = std::find_if(subs.begin(), subs.end(),
                [sid](const FastSubscriber& s) { return s.id == sid; });
            if (it != subs.end()) { subs.erase(it); return true; }
        }
        return false;
    }

    bool remove_from_normal(df_bus_subscription_id sid) {
        for (auto& [type_id, subs] : normal_) {
            auto it = std::find_if(subs.begin(), subs.end(),
                [sid](const BatchedSubscriber& s) { return s.id == sid; });
            if (it != subs.end()) { subs.erase(it); return true; }
        }
        return false;
    }

    bool remove_from_background(df_bus_subscription_id sid) {
        for (auto& [type_id, subs] : background_) {
            auto it = std::find_if(subs.begin(), subs.end(),
                [sid](const BatchedSubscriber& s) { return s.id == sid; });
            if (it != subs.end()) { subs.erase(it); return true; }
        }
        return false;
    }

    std::mutex mutex_;
    std::unordered_map<uint32_t, std::vector<FastSubscriber>>    fast_;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriber>> normal_;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriber>> background_;
    std::vector<PendingNormalEvent>                              pending_normal_;
    std::vector<PendingBackgroundEvent>                          pending_background_;
    uint64_t next_seq_ = 1;
};

} // namespace
} // namespace dualfrontier

using namespace dualfrontier;

extern "C" {

DF_API df_bus_subscription_id df_bus_subscribe_fast(
    uint32_t type_id, uint32_t mod_id,
    df_bus_fast_subscriber_fn cb, void* user_data)
{
    return BusNative::instance().subscribe_fast(type_id, mod_id, cb, user_data);
}

DF_API df_bus_subscription_id df_bus_subscribe_normal(
    uint32_t type_id, uint32_t mod_id,
    df_bus_batched_subscriber_fn cb, void* user_data)
{
    return BusNative::instance().subscribe_normal(type_id, mod_id, cb, user_data);
}

DF_API df_bus_subscription_id df_bus_subscribe_background(
    uint32_t type_id, uint32_t mod_id,
    df_bus_batched_subscriber_fn cb, void* user_data)
{
    return BusNative::instance().subscribe_background(type_id, mod_id, cb, user_data);
}

DF_API int32_t df_bus_publish_fast(
    uint32_t type_id, const void* payload, uint32_t payload_size)
{
    return BusNative::instance().publish_fast(type_id, payload, payload_size);
}

DF_API int32_t df_bus_publish_normal(
    uint32_t type_id, const void* payload, uint32_t payload_size)
{
    return BusNative::instance().publish_normal(type_id, payload, payload_size);
}

DF_API int32_t df_bus_publish_background(
    uint32_t type_id, const void* payload, uint32_t payload_size, uint32_t coalesce_key)
{
    return BusNative::instance().publish_background(type_id, payload, payload_size, coalesce_key);
}

DF_API int32_t df_bus_drain_normal_batch(void) {
    return BusNative::instance().drain_normal_batch();
}

DF_API int32_t df_bus_unsubscribe(df_bus_subscription_id sid) {
    return BusNative::instance().unsubscribe(sid);
}

DF_API int32_t df_bus_unsubscribe_fast_by_mod(uint32_t mod_id) {
    return BusNative::instance().unsubscribe_fast_by_mod(mod_id);
}

DF_API int32_t df_bus_unsubscribe_normal_by_mod(uint32_t mod_id) {
    return BusNative::instance().unsubscribe_normal_by_mod(mod_id);
}

DF_API int32_t df_bus_unsubscribe_background_by_mod(uint32_t mod_id) {
    return BusNative::instance().unsubscribe_background_by_mod(mod_id);
}

DF_API int32_t df_bus_subscriber_count_fast(uint32_t type_id) {
    return BusNative::instance().subscriber_count_fast(type_id);
}

DF_API int32_t df_bus_subscriber_count_normal(uint32_t type_id) {
    return BusNative::instance().subscriber_count_normal(type_id);
}

DF_API int32_t df_bus_subscriber_count_background(uint32_t type_id) {
    return BusNative::instance().subscriber_count_background(type_id);
}

DF_API void df_bus_clear(void) {
    BusNative::instance().clear();
}

} // extern "C"
