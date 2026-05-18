#pragma once

#include <cstdint>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

// K10.1 Items 6+7+8 — scheduling policies (priority class + CPU quotas +
// preemption semantics). Per-system policy metadata stored separately from
// the system graph; defaults apply when no explicit policy is set.

// Five scheduling classes per spec §3.2 Item 6. Lower numeric value = higher
// priority. Within phase, ordering is by class first, then by id for stability.
enum class SchedulingClass : int32_t {
    RealTime   = 0,  // strict latency; preempts other classes
    High       = 1,  // interactive / input handling
    Normal     = 2,  // default; most systems
    Low        = 3,  // non-critical work; deferred к phase end
    Background = 4,  // idle-time work; may be skipped
};

// Two-tier preemption model per spec §3.2 Item 8.
enum class PreemptionMode : int32_t {
    Cooperative = 0,  // run to completion; yield voluntarily
    Forced      = 1,  // RT class only; quota boundary may interrupt
};

class SchedulingPolicies {
public:
    struct Policy {
        SchedulingClass scheduling_class = SchedulingClass::Normal;
        int32_t max_latency_micros = 0;        // 0 = unbounded
        int32_t max_jitter_micros = 0;
        int32_t cpu_quota_micros_per_tick = 0; // 0 = no quota
        PreemptionMode preemption_mode = PreemptionMode::Cooperative;
    };

    struct ExecutionStats {
        int64_t total_micros = 0;         // cumulative across ticks
        int64_t this_tick_micros = 0;     // resets at reset_tick_stats()
        int32_t quota_violations = 0;     // count of exceedance events
    };

    SchedulingPolicies();
    ~SchedulingPolicies();

    SchedulingPolicies(const SchedulingPolicies&) = delete;
    SchedulingPolicies& operator=(const SchedulingPolicies&) = delete;

    // Set the per-system policy. Returns true (always — overwrites existing).
    bool set_policy(uint32_t system_id, const Policy& policy);

    // Get the per-system policy (returns default if unset).
    [[nodiscard]] Policy get_policy(uint32_t system_id) const;

    // Record execution time for a system (Item 7 quota tracking).
    // Returns 1 if quota exceeded for this tick, 0 otherwise.
    int32_t record_execution(uint32_t system_id, int64_t micros);

    // Quota status: 1 if system exceeded its budget this tick, 0 otherwise.
    [[nodiscard]] int32_t quota_exceeded(uint32_t system_id) const;

    // Execution stats accessors.
    [[nodiscard]] int64_t total_micros(uint32_t system_id) const;
    [[nodiscard]] int32_t quota_violations(uint32_t system_id) const;

    // Reset per-tick stats (called at tick boundary).
    void reset_tick_stats() noexcept;

    // Order an input set of system ids by scheduling class (lower numeric
    // value first). Within the same class, sorted by id ascending. Writes к
    // out_buffer up к out_capacity; returns count written.
    int32_t order_by_priority(const uint32_t* in_ids, uint32_t in_count,
                              uint32_t* out_buffer, int32_t out_capacity) const;

    void clear() noexcept;

private:
    std::unordered_map<uint32_t, Policy> policies_;
    std::unordered_map<uint32_t, ExecutionStats> stats_;
};

// Process-global default scheduling policies (paired с default_scheduler_graph
// + default_wake_registry).
SchedulingPolicies& default_scheduling_policies();

} // namespace dualfrontier
