#include "background_queue.h"

#include <algorithm>
#include <atomic>
#include <cstring>
#include <mutex>
#include <unordered_map>

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

// Compute total bytes used by pending events. Caller MUST hold the
// Background tier mutex.
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
// Caller MUST hold the Background tier mutex.
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
// Caller MUST hold the Background tier mutex.
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
    auto& tier = BusNative::background();

    // Snapshot subscribers + drain pending under Background tier mutex;
    // apply coalesce before dispatch (idempotent с per-publish coalesce
    // when publishers call force_coalesce too).
    std::vector<PendingBackgroundEventRecord> drained;
    std::unordered_map<uint32_t, std::vector<BatchedSubscriberRecord>> subs_snapshot;
    {
        std::lock_guard<std::mutex> tier_lock(tier.mutex);
        coalesce_pending_locked(tier.pending);
        drained.swap(tier.pending);
        subs_snapshot = tier.subscribers;
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
            std::lock_guard<std::mutex> tier_lock(tier.mutex);
            for (auto it = drained.begin() + dispatched; it != drained.end(); ++it) {
                tier.pending.push_back(*it);
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
    auto& tier = BusNative::background();
    std::lock_guard<std::mutex> tier_lock(tier.mutex);
    if (out_event_count) *out_event_count = static_cast<uint32_t>(tier.pending.size());
    if (out_bytes_used)  *out_bytes_used  = compute_pending_bytes_locked(tier.pending);
    return 1;
}

DF_API int32_t df_background_queue_saturation_events(void) {
    return policy().saturation_count.load(std::memory_order_relaxed);
}

// =========================================================================
// K10.2 Item 31 — Save-integrated storage (S3-Q3 untargeted persistence)
// =========================================================================

namespace {

// Header bytes per Item 31 wire format (К10.2 schema v1)
constexpr uint32_t HEADER_BYTES         = 12;
constexpr uint32_t EVENT_FIXED_BYTES    = 12;  // type_id + coalesce_key + payload_size

void write_u32(uint8_t* dst, uint32_t v) {
    dst[0] = static_cast<uint8_t>(v & 0xFFu);
    dst[1] = static_cast<uint8_t>((v >> 8) & 0xFFu);
    dst[2] = static_cast<uint8_t>((v >> 16) & 0xFFu);
    dst[3] = static_cast<uint8_t>((v >> 24) & 0xFFu);
}

uint32_t read_u32(const uint8_t* src) {
    return static_cast<uint32_t>(src[0])
        | (static_cast<uint32_t>(src[1]) << 8)
        | (static_cast<uint32_t>(src[2]) << 16)
        | (static_cast<uint32_t>(src[3]) << 24);
}

} // namespace

DF_API int32_t df_background_queue_compute_save_size(uint32_t* out_required_bytes) {
    if (out_required_bytes == nullptr) return 0;
    auto& tier = dualfrontier::BusNative::background();
    std::lock_guard<std::mutex> tier_lock(tier.mutex);
    uint64_t total = HEADER_BYTES;
    for (const auto& ev : tier.pending) {
        total += EVENT_FIXED_BYTES + ev.payload.size();
    }
    if (total > UINT32_MAX) return 0;
    *out_required_bytes = static_cast<uint32_t>(total);
    return 1;
}

DF_API int32_t df_background_queue_serialize(
    void* out_buffer, uint32_t buffer_size, uint32_t* out_bytes_written)
{
    if (out_buffer == nullptr || out_bytes_written == nullptr) return 0;
    auto& tier = dualfrontier::BusNative::background();
    std::lock_guard<std::mutex> tier_lock(tier.mutex);

    uint64_t required = HEADER_BYTES;
    uint32_t total_payload = 0;
    for (const auto& ev : tier.pending) {
        required += EVENT_FIXED_BYTES + ev.payload.size();
        total_payload += static_cast<uint32_t>(ev.payload.size());
    }
    if (required > buffer_size) return 0;

    uint8_t* p = static_cast<uint8_t*>(out_buffer);
    write_u32(p,     DF_BG_QUEUE_SCHEMA_VERSION); p += 4;
    write_u32(p,     static_cast<uint32_t>(tier.pending.size())); p += 4;
    write_u32(p,     total_payload); p += 4;

    for (const auto& ev : tier.pending) {
        write_u32(p, ev.type_id);      p += 4;
        write_u32(p, ev.coalesce_key); p += 4;
        write_u32(p, static_cast<uint32_t>(ev.payload.size())); p += 4;
        if (!ev.payload.empty()) {
            std::memcpy(p, ev.payload.data(), ev.payload.size());
            p += ev.payload.size();
        }
    }
    *out_bytes_written = static_cast<uint32_t>(required);
    return 1;
}

DF_API int32_t df_background_queue_deserialize(
    const void* buffer, uint32_t buffer_size, uint32_t* out_events_loaded)
{
    if (buffer == nullptr) return 0;
    if (buffer_size < HEADER_BYTES) return 0;
    const uint8_t* p = static_cast<const uint8_t*>(buffer);
    const uint8_t* end = p + buffer_size;

    uint32_t schema   = read_u32(p); p += 4;
    uint32_t count    = read_u32(p); p += 4;
    uint32_t total_pl = read_u32(p); p += 4;
    (void)total_pl;
    if (schema != DF_BG_QUEUE_SCHEMA_VERSION) return 0;  // unsupported version

    std::vector<dualfrontier::PendingBackgroundEventRecord> loaded;
    loaded.reserve(count);
    for (uint32_t i = 0; i < count; ++i) {
        if (end - p < static_cast<ptrdiff_t>(EVENT_FIXED_BYTES)) return 0;
        uint32_t type_id      = read_u32(p); p += 4;
        uint32_t coalesce_key = read_u32(p); p += 4;
        uint32_t payload_size = read_u32(p); p += 4;
        if (end - p < static_cast<ptrdiff_t>(payload_size)) return 0;
        dualfrontier::PendingBackgroundEventRecord ev;
        ev.type_id      = type_id;
        ev.coalesce_key = coalesce_key;
        ev.payload.assign(p, p + payload_size);
        p += payload_size;
        loaded.push_back(std::move(ev));
    }

    auto& tier = dualfrontier::BusNative::background();
    std::lock_guard<std::mutex> tier_lock(tier.mutex);
    tier.pending = std::move(loaded);
    if (out_events_loaded) *out_events_loaded = count;
    return 1;
}

DF_API int32_t df_background_queue_force_coalesce(void) {
    auto& tier = BusNative::background();
    auto& cfg = policy();
    std::lock_guard<std::mutex> tier_lock(tier.mutex);
    size_t before = tier.pending.size();
    coalesce_pending_locked(tier.pending);
    size_t after = tier.pending.size();

    // Apply saturation policy after coalesce (smaller queue post-coalesce
    // is more likely к fit cap; drop-oldest only kicks in если still over).
    uint32_t max_bytes = 0;
    {
        std::lock_guard<std::mutex> cfg_lock(cfg.mutex);
        max_bytes = cfg.max_bytes;
    }
    int32_t dropped = apply_drop_oldest_locked(tier.pending, max_bytes);
    if (dropped > 0) {
        cfg.saturation_count.fetch_add(dropped, std::memory_order_relaxed);
    }
    return static_cast<int32_t>(before - after);
}

} // extern "C"
