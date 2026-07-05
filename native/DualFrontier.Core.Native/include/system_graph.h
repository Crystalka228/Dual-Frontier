#pragma once

#include <atomic>
#include <cstdint>
#include <string>
#include <unordered_set>
#include <vector>

#include "singleton_guard.h"

namespace dualfrontier {

// K10.1 Item 1 — Native dependency graph for the kernel system scheduler.
//
// Replaces managed DependencyGraph.cs as the authoritative scheduler graph
// post-K10.1 (K-L6 SUPERSEDED, K-L12 AUTHORED). The managed graph remains
// operational during the K10.1 cascade and is switched к adapter-facade role
// at Commit 14 (Item 16).
//
// Semantics mirror managed DependencyGraph (Lesson #7 transcription):
//   - Edge A -> B means «B reads what A writes»
//   - Write-write conflicts on the same component type are errors
//   - Cycles are errors (detected after Kahn drains)
//   - Phase composition: each phase contains mutually independent systems
//
// Two graph modes:
//   1. Static graph — all registered systems, computed at registration changes
//      (system add/remove, mod load/unload). Cached until next change.
//   2. Per-tick graph — Kahn restricted к the runnable subset of systems for
//      the current tick (K-L13 on-demand activation). Per Item 5, this
//      produces tighter parallelism than static phase ordering would.
//
// Thread-safety: single-threaded registration / compute. Concurrent reads of
// already-computed phase composition are safe (immutable post-compute).
class SystemGraph {
public:
    struct SystemEntry {
        uint32_t id;
        std::string fqn;
        std::vector<uint32_t> reads;
        std::vector<uint32_t> writes;
        int32_t priority_class;  // K10.1 Item 6 — default 2 (Normal)
        int32_t wake_type;       // K10.1 Item 3 — default 0 (Timer)
    };

    // K10.1 Item 13 — Per-phase barrier semantics override. Mirrors
    // dualfrontier::BarrierType values: 0=Full (default), 1=Partial, 2=None.
    static constexpr int32_t kBarrierDefault = 0;

    SystemGraph();
    ~SystemGraph();

    SystemGraph(const SystemGraph&) = delete;
    SystemGraph& operator=(const SystemGraph&) = delete;

    // Register a system with its access declarations.
    // Returns true on success. Returns false on duplicate id or empty fqn.
    // Marks the static graph dirty — call compute_static_graph() to rebuild.
    bool register_system(uint32_t system_id,
                         std::string fqn,
                         std::vector<uint32_t> reads,
                         std::vector<uint32_t> writes,
                         int32_t priority_class,
                         int32_t wake_type);

    // Remove a system. Marks the static graph dirty.
    // Returns true if removed, false if not registered.
    bool unregister_system(uint32_t system_id);

    // Number of registered systems.
    [[nodiscard]] std::size_t system_count() const noexcept { return systems_.size(); }

    // Compute static graph over all registered systems.
    // Returns:
    //    1 — success, phases computed (queryable via phase_* accessors)
    //    0 — generic failure
    //   -1 — write-write conflict (last_error() carries detail)
    //   -2 — cycle detected (last_error() carries detail)
    //   -3 — concurrency violation (F-29(a): concurrent entry detected; no
    //        shared state was touched — kConcurrencyViolation)
    int32_t compute_static_graph();

    // Number of static phases after a successful compute_static_graph().
    [[nodiscard]] int32_t static_phase_count() const noexcept;

    // Number of systems in the given static phase index.
    [[nodiscard]] int32_t static_phase_size(int32_t phase_index) const noexcept;

    // Copy system ids of the given static phase into out_buffer. Returns
    // number written. Truncates if out_capacity < phase size.
    int32_t static_phase_systems(int32_t phase_index,
                                 uint32_t* out_buffer,
                                 int32_t out_capacity) const noexcept;

    // Compute per-tick graph: Kahn on the runnable subset.
    // Edges between runnable systems only; static [SystemAccess] reused.
    // Returns same codes as compute_static_graph().
    int32_t compute_per_tick_graph(const uint32_t* runnable_ids,
                                   uint32_t runnable_count);

    [[nodiscard]] int32_t per_tick_phase_count() const noexcept;
    [[nodiscard]] int32_t per_tick_phase_size(int32_t phase_index) const noexcept;
    int32_t per_tick_phase_systems(int32_t phase_index,
                                   uint32_t* out_buffer,
                                   int32_t out_capacity) const noexcept;

    // Last error message after a failing compute. Empty if no failure.
    [[nodiscard]] const std::string& last_error() const noexcept { return last_error_; }

    // K10.1 Item 13 — Per-phase barrier type accessors. Default 0 (Full).
    // Persists across compute_static_graph calls but reset by clear().
    int32_t set_phase_barrier(int32_t phase_index, int32_t barrier_type) noexcept;
    [[nodiscard]] int32_t get_phase_barrier(int32_t phase_index) const noexcept;

    // Reset to empty state (no registrations, no computed phases).
    void clear() noexcept;

private:
    // F-29(a) — fail-loud single-thread guard (see SingletonGuard). The design
    // contract is single-threaded mutation/compute; this atomic turns a
    // concurrent-entry violation into a visible rejection (kConcurrencyViolation
    // / false / no-op) instead of a silent heap corruption. Read-only phase
    // accessors are NOT guarded — concurrent reads of computed state are safe.
    std::atomic<bool> busy_{false};

    std::vector<SystemEntry> systems_;
    std::unordered_set<uint32_t> system_ids_;  // D1 — O(1) duplicate-id check, kept in lock-step with systems_
    std::vector<std::vector<uint32_t>> static_phases_;
    std::vector<std::vector<uint32_t>> per_tick_phases_;
    std::vector<int32_t> static_phase_barriers_;  // K10.1 Item 13
    std::string last_error_;

    // Run Kahn's topological sort over the given subset (pointers into systems_).
    // Populates out_phases on success. Returns 1, -1 (write conflict), or -2 (cycle).
    int32_t kahn_topological_sort(
        const std::vector<const SystemEntry*>& subset,
        std::vector<std::vector<uint32_t>>& out_phases);
};

// Process-global default scheduler graph. Singleton matching OS-faithful
// «one kernel scheduler per process» model. Tests may create their own
// SystemGraph instance directly to bypass the singleton.
SystemGraph& default_scheduler_graph();

} // namespace dualfrontier
