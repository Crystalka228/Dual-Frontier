#include "background_queue.h"

#include <algorithm>
#include <atomic>
#include <mutex>

#include "bus_native_internal.h"
#include "event_type_registry.h"

namespace dualfrontier {
namespace {

constexpr uint32_t DEFAULT_MAX_BYTES        = 10u * 1024u * 1024u;  // 10 MB hard cap (Q-N-45)
constexpr uint32_t DEFAULT_WARN_THRESHOLD   = static_cast<uint32_t>(DEFAULT_MAX_BYTES * 0.80);

struct PolicyConfig {
    std::mutex                       mutex;
    uint32_t                          max_bytes        = DEFAULT_MAX_BYTES;
    uint32_t                          warn_threshold   = DEFAULT_WARN_THRESHOLD;
    df_bg_queue_saturation_strategy   strategy         = DF_BG_QUEUE_DROP_OLDEST;
    std::atomic<int32_t>              saturation_count{0};
};

PolicyConfig& policy() {
    static PolicyConfig cfg;
    return cfg;
}

// Compute total bytes used by pending events. Caller MUST hold BusNative mutex.
uint32_t compute_pending_bytes_locked(const std::vector<PendingBackgroundEventRecord>& q) {
    uint64_t total = 0;
    for (const auto& ev : q) total += static_cast<uint64_t>(ev.payload.size());
    return static_cast<uint32_t>(std::min<uint64_t>(total, UINT32_MAX));
}

// Apply Q-N-34 coalesce policy: for each (type_id, coalesce_key) match against
// existing pending events; coalesce in-place using event type registry's
// coalesce_fn. К10.2 К-L14 default-inclusion: coalesce is the architectural
// primitive, not an optional optimization.
//
// Caller MUST hold BusNative mutex.
void coalesce_pending_locked(std::vector<PendingBackgroundEventRecord>& q) {
    for (auto it = q.begin(); it != q.end(); ) {
        bool merged = false;
        for (auto prior = q.begin(); prior != it; ++prior) {
            if (prior->type_id == it->type_id && prior->coalesce_key == it->coalesce_key) {
                // Look up coalesce_fn from event type registry (Item 28)
                int32_t tier_value = 0;
                uint32_t payload_size = 0;
                const char* fqn = nullptr;
                void (*coalesce_fn)(void*, const void*) = nullptr;
                int32_t found = df_event_type_registry_lookup(
                    it->type_id, &tier_value, &payload_size, &fqn, &coalesce_fn);
                if (found == 1 && coalesce_fn != nullptr && !prior->payload.empty() && !it->payload.empty()) {
                    coalesce_fn(prior->payload.data(), it->payload.data());
                    it = q.erase(it);
                    merged = true;
                    break;
                }
            }
        }
        if (!merged) ++it;
    }
}

// Drop-oldest until queue size <= max_bytes. Returns count of events dropped.
// Caller MUST hold BusNative mutex.
int32_t apply_drop_oldest_locked(std::vector<PendingBackgroundEventRecord>& q, uint32_t max_bytes) {
    int32_t dropped = 0;
    uint32_t bytes = compute_pending_bytes_locked(q);
    while (bytes > max_bytes && !q.empty()) {
        bytes -= static_cast<uint32_t>(q.front().payload.size());
        q.erase(q.begin());
        ++dropped;
    }
    return dropped;
}

} // namespace
} // namespace dualfrontier

using namespace dualfrontier;

extern "C" {

DF_API int32_t df_background_queue_configure(
    uint32_t  max_bytes,
    uint32_t  warn_threshold_bytes,
    int32_t   strategy)
{
    if (max_bytes == 0) return 0;
    if (strategy < 0 || strategy > DF_BG_QUEUE_EXPAND) return 0;
    // К10.2 default: only DROP_OLDEST is implemented. BACKPRESSURE / EXPAND
    // deferred к К-extensions per scope discipline.
    if (strategy != DF_BG_QUEUE_DROP_OLDEST) {
        // Configuration accepted (records intent) но behavior remains
        // drop-oldest until К-extensions implementation lands.
    }
    auto& cfg = policy();
    std::lock_guard<std::mutex> lock(cfg.mutex);
    cfg.max_bytes        = max_bytes;
    cfg.warn_threshold   = warn_threshold_bytes;
    cfg.strategy         = static_cast<df_bg_queue_saturation_strategy>(strategy);
    return 1;
}

DF_API int32_t df_background_queue_dispatch_idle_slot(uint64_t available_budget_micros) {
    auto& bus = BusNative::instance();

    // Snapshot subscribers + drain pending under bus mutex; apply coalesce
    // before dispatch (idempotent с per-publish coalesce when publishers
    // call force_coalesce too).
    std::vector<PendingBackgroundEventRecord> drained;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>> subs_snapshot;
    {
        std::lock_guard<std::mutex> bus_lock(bus.mutex());
        auto& q = bus.pending_background_unsafe();
        coalesce_pending_locked(q);
        drained.swap(q);
        subs_snapshot = bus.background_subscribers_unsafe();
    }

    if (drained.empty()) return 0;

    // Sequential dispatch с budget check. Each event costs ~1 unit of budget
    // (К10.2 default — refined budgeting в К-extensions per Q-N-35).
    int32_t dispatched = 0;
    uint64_t cost_per_event = 100;  // microseconds (rough К10.2 default)
    uint64_t budget = available_budget_micros;

    for (const auto& ev : drained) {
        if (budget < cost_per_event && available_budget_micros != 0) {
            // Out of budget; requeue remaining events for next idle-slot.
            std::lock_guard<std::mutex> bus_lock(bus.mutex());
            auto& q = bus.pending_background_unsafe();
            for (auto it = drained.begin() + dispatched; it != drained.end(); ++it) {
                q.push_back(*it);
            }
            break;
        }
        auto it = subs_snapshot.find(ev.type_id);
        if (it == subs_snapshot.end()) {
            ++dispatched;
            if (budget >= cost_per_event) budget -= cost_per_event;
            continue;
        }
        for (const auto& sub : it->second) {
            df_managed_system_batch batch{};
            batch.system_ids = nullptr;
            batch.count      = 1;
            batch.delta      = 0.0f;
            batch.user_data  = sub.user_data;
            sub.callback(&batch);
        }
        ++dispatched;
        if (budget >= cost_per_event) budget -= cost_per_event;
    }
    return dispatched;
}

DF_API int32_t df_background_queue_size(uint32_t* out_event_count, uint32_t* out_bytes_used) {
    auto& bus = BusNative::instance();
    std::lock_guard<std::mutex> bus_lock(bus.mutex());
    auto& q = bus.pending_background_unsafe();
    if (out_event_count) *out_event_count = static_cast<uint32_t>(q.size());
    if (out_bytes_used)  *out_bytes_used  = compute_pending_bytes_locked(q);
    return 1;
}

DF_API int32_t df_background_queue_saturation_events(void) {
    return policy().saturation_count.load(std::memory_order_relaxed);
}

DF_API int32_t df_background_queue_force_coalesce(void) {
    auto& bus = BusNative::instance();
    auto& cfg = policy();
    std::lock_guard<std::mutex> bus_lock(bus.mutex());
    auto& q = bus.pending_background_unsafe();
    size_t before = q.size();
    coalesce_pending_locked(q);
    size_t after = q.size();

    // Apply saturation policy after coalesce (smaller queue post-coalesce
    // is more likely к fit cap; drop-oldest only kicks in если still over).
    uint32_t max_bytes = 0;
    {
        std::lock_guard<std::mutex> cfg_lock(cfg.mutex);
        max_bytes = cfg.max_bytes;
    }
    int32_t dropped = apply_drop_oldest_locked(q, max_bytes);
    if (dropped > 0) {
        cfg.saturation_count.fetch_add(dropped, std::memory_order_relaxed);
    }
    return static_cast<int32_t>(before - after);
}

} // extern "C"
