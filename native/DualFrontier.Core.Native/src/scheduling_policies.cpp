#include "scheduling_policies.h"

#include <algorithm>

namespace dualfrontier {

SchedulingPolicies::SchedulingPolicies() = default;
SchedulingPolicies::~SchedulingPolicies() = default;

bool SchedulingPolicies::set_policy(uint32_t system_id, const Policy& policy) {
    policies_[system_id] = policy;
    // Ensure stats record exists.
    stats_.try_emplace(system_id);
    return true;
}

SchedulingPolicies::Policy SchedulingPolicies::get_policy(uint32_t system_id) const {
    auto it = policies_.find(system_id);
    if (it == policies_.end()) return Policy{};
    return it->second;
}

int32_t SchedulingPolicies::record_execution(uint32_t system_id, int64_t micros) {
    auto& s = stats_[system_id];
    s.total_micros += micros;
    s.this_tick_micros += micros;

    Policy p = get_policy(system_id);
    if (p.cpu_quota_micros_per_tick > 0 &&
        s.this_tick_micros > p.cpu_quota_micros_per_tick) {
        ++s.quota_violations;
        return 1;
    }
    return 0;
}

int32_t SchedulingPolicies::quota_exceeded(uint32_t system_id) const {
    auto it = stats_.find(system_id);
    if (it == stats_.end()) return 0;
    Policy p = get_policy(system_id);
    if (p.cpu_quota_micros_per_tick <= 0) return 0;
    return (it->second.this_tick_micros > p.cpu_quota_micros_per_tick) ? 1 : 0;
}

int64_t SchedulingPolicies::total_micros(uint32_t system_id) const {
    auto it = stats_.find(system_id);
    return (it == stats_.end()) ? 0 : it->second.total_micros;
}

int32_t SchedulingPolicies::quota_violations(uint32_t system_id) const {
    auto it = stats_.find(system_id);
    return (it == stats_.end()) ? 0 : it->second.quota_violations;
}

void SchedulingPolicies::reset_tick_stats() noexcept {
    for (auto& kv : stats_) {
        kv.second.this_tick_micros = 0;
    }
}

int32_t SchedulingPolicies::order_by_priority(const uint32_t* in_ids,
                                              uint32_t in_count,
                                              uint32_t* out_buffer,
                                              int32_t out_capacity) const {
    if (in_ids == nullptr || out_buffer == nullptr || out_capacity <= 0) return 0;
    if (in_count == 0) return 0;

    struct OrderEntry {
        uint32_t id;
        int32_t scheduling_class;
    };
    std::vector<OrderEntry> entries;
    entries.reserve(in_count);
    for (uint32_t i = 0; i < in_count; ++i) {
        uint32_t id = in_ids[i];
        Policy p = get_policy(id);
        entries.push_back({id, static_cast<int32_t>(p.scheduling_class)});
    }
    // Stable sort: scheduling class ascending (lower numeric = higher priority),
    // then id ascending для determinism.
    std::sort(entries.begin(), entries.end(), [](const OrderEntry& a, const OrderEntry& b) {
        if (a.scheduling_class != b.scheduling_class) {
            return a.scheduling_class < b.scheduling_class;
        }
        return a.id < b.id;
    });
    int32_t n = static_cast<int32_t>(entries.size());
    if (n > out_capacity) n = out_capacity;
    for (int32_t i = 0; i < n; ++i) {
        out_buffer[i] = entries[static_cast<std::size_t>(i)].id;
    }
    return n;
}

void SchedulingPolicies::clear() noexcept {
    policies_.clear();
    stats_.clear();
}

SchedulingPolicies& default_scheduling_policies() {
    static SchedulingPolicies instance;
    return instance;
}

} // namespace dualfrontier
