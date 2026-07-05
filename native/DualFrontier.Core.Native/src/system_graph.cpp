#include "system_graph.h"

#include <algorithm>
#include <sstream>
#include <unordered_map>
#include <unordered_set>

namespace dualfrontier {

SystemGraph::SystemGraph() = default;
SystemGraph::~SystemGraph() = default;

bool SystemGraph::register_system(uint32_t system_id,
                                  std::string fqn,
                                  std::vector<uint32_t> reads,
                                  std::vector<uint32_t> writes,
                                  int32_t priority_class,
                                  int32_t wake_type) {
    SingletonGuard guard(busy_);
    if (!guard.acquired()) {
        return false;  // F-29(a): concurrent entry — refuse without touching state
    }
    if (fqn.empty()) {
        return false;
    }
    if (system_ids_.count(system_id) != 0) {
        return false;  // D1 — O(1) duplicate check (was an O(N) linear scan → O(N^2) over N registrations)
    }
    SystemEntry entry;
    entry.id = system_id;
    entry.fqn = std::move(fqn);
    entry.reads = std::move(reads);
    entry.writes = std::move(writes);
    entry.priority_class = priority_class;
    entry.wake_type = wake_type;
    systems_.push_back(std::move(entry));
    system_ids_.insert(system_id);  // keep the index in lock-step
    static_phases_.clear();  // mark dirty
    per_tick_phases_.clear();
    return true;
}

bool SystemGraph::unregister_system(uint32_t system_id) {
    SingletonGuard guard(busy_);
    if (!guard.acquired()) {
        return false;  // F-29(a): concurrent entry
    }
    for (auto it = systems_.begin(); it != systems_.end(); ++it) {
        if (it->id == system_id) {
            systems_.erase(it);
            system_ids_.erase(system_id);  // keep the index in lock-step
            static_phases_.clear();
            per_tick_phases_.clear();
            return true;
        }
    }
    return false;
}

int32_t SystemGraph::set_phase_barrier(int32_t phase_index, int32_t barrier_type) noexcept {
    if (phase_index < 0) return 0;
    if (barrier_type < 0 || barrier_type > 2) return 0;
    if (static_cast<std::size_t>(phase_index) >= static_phase_barriers_.size()) {
        static_phase_barriers_.resize(static_cast<std::size_t>(phase_index) + 1, kBarrierDefault);
    }
    static_phase_barriers_[static_cast<std::size_t>(phase_index)] = barrier_type;
    return 1;
}

int32_t SystemGraph::get_phase_barrier(int32_t phase_index) const noexcept {
    if (phase_index < 0 || static_cast<std::size_t>(phase_index) >= static_phase_barriers_.size()) {
        return kBarrierDefault;
    }
    return static_phase_barriers_[static_cast<std::size_t>(phase_index)];
}

void SystemGraph::clear() noexcept {
    SingletonGuard guard(busy_);
    if (!guard.acquired()) {
        return;  // F-29(a): concurrent entry — refuse (no-op)
    }
    systems_.clear();
    system_ids_.clear();  // keep the index in lock-step
    static_phases_.clear();
    per_tick_phases_.clear();
    static_phase_barriers_.clear();
    last_error_.clear();
}

int32_t SystemGraph::compute_static_graph() {
    SingletonGuard guard(busy_);
    if (!guard.acquired()) {
        return kConcurrencyViolation;  // F-29(a): concurrent entry
    }
    last_error_.clear();
    std::vector<const SystemEntry*> all;
    all.reserve(systems_.size());
    for (const SystemEntry& s : systems_) {
        all.push_back(&s);
    }
    return kahn_topological_sort(all, static_phases_);
}

int32_t SystemGraph::compute_per_tick_graph(const uint32_t* runnable_ids,
                                            uint32_t runnable_count) {
    SingletonGuard guard(busy_);
    if (!guard.acquired()) {
        return kConcurrencyViolation;  // F-29(a): concurrent entry
    }
    last_error_.clear();
    per_tick_phases_.clear();
    if (runnable_count == 0) {
        return 1;  // empty graph is valid
    }
    // Build lookup id -> SystemEntry*
    std::unordered_map<uint32_t, const SystemEntry*> by_id;
    by_id.reserve(systems_.size());
    for (const SystemEntry& s : systems_) {
        by_id[s.id] = &s;
    }
    std::vector<const SystemEntry*> subset;
    subset.reserve(runnable_count);
    for (uint32_t i = 0; i < runnable_count; ++i) {
        auto it = by_id.find(runnable_ids[i]);
        if (it == by_id.end()) {
            std::ostringstream oss;
            oss << "[SCHEDULER ERROR] runnable id " << runnable_ids[i]
                << " is not registered";
            last_error_ = oss.str();
            return 0;
        }
        subset.push_back(it->second);
    }
    return kahn_topological_sort(subset, per_tick_phases_);
}

int32_t SystemGraph::static_phase_count() const noexcept {
    return static_cast<int32_t>(static_phases_.size());
}

int32_t SystemGraph::static_phase_size(int32_t phase_index) const noexcept {
    if (phase_index < 0 || static_cast<std::size_t>(phase_index) >= static_phases_.size()) {
        return 0;
    }
    return static_cast<int32_t>(static_phases_[phase_index].size());
}

int32_t SystemGraph::static_phase_systems(int32_t phase_index,
                                          uint32_t* out_buffer,
                                          int32_t out_capacity) const noexcept {
    if (phase_index < 0 || static_cast<std::size_t>(phase_index) >= static_phases_.size()) {
        return 0;
    }
    if (out_buffer == nullptr || out_capacity <= 0) {
        return 0;
    }
    const std::vector<uint32_t>& phase = static_phases_[phase_index];
    int32_t n = static_cast<int32_t>(phase.size());
    if (n > out_capacity) {
        n = out_capacity;
    }
    for (int32_t i = 0; i < n; ++i) {
        out_buffer[i] = phase[static_cast<std::size_t>(i)];
    }
    return n;
}

int32_t SystemGraph::per_tick_phase_count() const noexcept {
    return static_cast<int32_t>(per_tick_phases_.size());
}

int32_t SystemGraph::per_tick_phase_size(int32_t phase_index) const noexcept {
    if (phase_index < 0 || static_cast<std::size_t>(phase_index) >= per_tick_phases_.size()) {
        return 0;
    }
    return static_cast<int32_t>(per_tick_phases_[phase_index].size());
}

int32_t SystemGraph::per_tick_phase_systems(int32_t phase_index,
                                            uint32_t* out_buffer,
                                            int32_t out_capacity) const noexcept {
    if (phase_index < 0 || static_cast<std::size_t>(phase_index) >= per_tick_phases_.size()) {
        return 0;
    }
    if (out_buffer == nullptr || out_capacity <= 0) {
        return 0;
    }
    const std::vector<uint32_t>& phase = per_tick_phases_[phase_index];
    int32_t n = static_cast<int32_t>(phase.size());
    if (n > out_capacity) {
        n = out_capacity;
    }
    for (int32_t i = 0; i < n; ++i) {
        out_buffer[i] = phase[static_cast<std::size_t>(i)];
    }
    return n;
}

int32_t SystemGraph::kahn_topological_sort(
    const std::vector<const SystemEntry*>& subset,
    std::vector<std::vector<uint32_t>>& out_phases) {
    out_phases.clear();
    if (subset.empty()) {
        return 1;
    }

    // K10.1 Item 1 (F-29(b) rewrite) — index-keyed / work-queue rebuild. The
    // legacy O(N^2) write-conflict scan + O(N^2) edge build + O(P*N) per-phase
    // Kahn rescan are replaced by a single component->writers index plus a
    // layer-wise BFS. Output is byte-identical to the legacy build: same phase
    // composition, same ascending id sort per phase, same 1/-1/-2 return codes,
    // same last_error text.
    const std::size_t n = subset.size();

    // Index every written component to the subset indices that write it, in one
    // ascending pass. Reused by both the conflict check and the edge build.
    std::unordered_map<uint32_t, std::vector<std::size_t>> writer_of;
    for (std::size_t i = 0; i < n; ++i) {
        for (uint32_t w : subset[i]->writes) {
            writer_of[w].push_back(i);
        }
    }

    // 1. Write-write conflict detection. A component written by >1 subset system
    //    is a conflict. Preserve the legacy nested (i<j) scan's FIRST-reported
    //    pair: the lexicographically-smallest (i, j), i<j, sharing a written
    //    component (min lower-writer, then min partner > it), and the first
    //    shared component in j's writes order.
    {
        bool have_conflict = false;
        std::size_t best_i = 0;
        for (const auto& kv : writer_of) {
            if (kv.second.size() < 2) {
                continue;
            }
            // Ascending list -> [0] is this component's smallest lower-writer.
            std::size_t lower = kv.second.front();
            if (!have_conflict || lower < best_i) {
                have_conflict = true;
                best_i = lower;
            }
        }
        if (have_conflict) {
            const SystemEntry* a = subset[best_i];
            std::unordered_set<uint32_t> writes_a(a->writes.begin(), a->writes.end());
            std::size_t best_j = n;  // smallest partner index > best_i sharing a write with a
            for (uint32_t w : a->writes) {
                auto it = writer_of.find(w);
                if (it == writer_of.end()) {
                    continue;
                }
                for (std::size_t idx : it->second) {  // ascending
                    if (idx > best_i) {
                        if (idx < best_j) { best_j = idx; }
                        break;
                    }
                }
            }
            const SystemEntry* b = subset[best_j];
            uint32_t conflict_w = 0;  // first w in b's writes order that a also writes
            for (uint32_t w : b->writes) {
                if (writes_a.count(w) != 0) { conflict_w = w; break; }
            }
            std::ostringstream oss;
            oss << "[SCHEDULER ERROR] Write conflict: "
                << a->fqn << " and " << b->fqn
                << " both write component type " << conflict_w;
            last_error_ = oss.str();
            return -1;
        }
    }

    // 2. Build edges A -> B (B reads what A writes) via writer_of. Match the
    //    legacy "at most one edge per ordered (i, j)" semantic (the old code
    //    breaks after the first matching read) with a per-j visited-writers set,
    //    so every in_degree value is identical. adjacency[i] receives j in
    //    ascending order (outer j loop), as before.
    std::vector<std::vector<std::size_t>> adjacency(n);
    std::vector<int32_t> in_degree(n, 0);
    for (std::size_t j = 0; j < n; ++j) {
        const SystemEntry* b = subset[j];
        if (b->reads.empty()) {
            continue;
        }
        std::unordered_set<std::size_t> counted_writers;
        for (uint32_t r : b->reads) {
            auto it = writer_of.find(r);
            if (it == writer_of.end()) {
                continue;
            }
            for (std::size_t i : it->second) {
                if (i == j) {
                    continue;
                }
                if (counted_writers.insert(i).second) {
                    adjacency[i].push_back(j);
                    in_degree[j]++;
                }
            }
        }
    }

    // 3. Layer-wise BFS Kahn — one phase per layer (ids sorted ascending, as
    //    before); seed with in_degree-0 nodes, decrement neighbours, collect the
    //    newly-zero into the next layer. Each node + edge processed once -> O(N+E),
    //    no per-phase full rescan.
    std::vector<std::size_t> current;
    current.reserve(n);
    for (std::size_t i = 0; i < n; ++i) {
        if (in_degree[i] == 0) {
            current.push_back(i);
        }
    }
    std::size_t visited_count = 0;
    while (!current.empty()) {
        std::vector<uint32_t> phase_ids;
        phase_ids.reserve(current.size());
        for (std::size_t idx : current) {
            phase_ids.push_back(subset[idx]->id);
        }
        // Sort phase ids deterministically for stable output across runs.
        std::sort(phase_ids.begin(), phase_ids.end());
        out_phases.push_back(std::move(phase_ids));

        std::vector<std::size_t> next;
        for (std::size_t idx : current) {
            ++visited_count;
            for (std::size_t target : adjacency[idx]) {
                if (--in_degree[target] == 0) {
                    next.push_back(target);
                }
            }
        }
        current = std::move(next);
    }

    if (visited_count < n) {
        // Cycle: remaining = never-visited nodes (in_degree still > 0), in subset
        // order -- byte-identical last_error text.
        std::ostringstream oss;
        oss << "[SCHEDULER ERROR] Cyclic dependency detected — remaining systems:";
        for (std::size_t i = 0; i < n; ++i) {
            if (in_degree[i] > 0) {
                oss << " " << subset[i]->fqn;
            }
        }
        last_error_ = oss.str();
        return -2;
    }
    return 1;
}

SystemGraph& default_scheduler_graph() {
    static SystemGraph instance;
    return instance;
}

} // namespace dualfrontier
