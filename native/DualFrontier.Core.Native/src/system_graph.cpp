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
    if (fqn.empty()) {
        return false;
    }
    for (const SystemEntry& s : systems_) {
        if (s.id == system_id) {
            return false;  // duplicate
        }
    }
    SystemEntry entry;
    entry.id = system_id;
    entry.fqn = std::move(fqn);
    entry.reads = std::move(reads);
    entry.writes = std::move(writes);
    entry.priority_class = priority_class;
    entry.wake_type = wake_type;
    systems_.push_back(std::move(entry));
    static_phases_.clear();  // mark dirty
    per_tick_phases_.clear();
    return true;
}

bool SystemGraph::unregister_system(uint32_t system_id) {
    for (auto it = systems_.begin(); it != systems_.end(); ++it) {
        if (it->id == system_id) {
            systems_.erase(it);
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
    systems_.clear();
    static_phases_.clear();
    per_tick_phases_.clear();
    static_phase_barriers_.clear();
    last_error_.clear();
}

int32_t SystemGraph::compute_static_graph() {
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

    // 1. Write-write conflict detection: pairs of systems writing the same component.
    for (std::size_t i = 0; i < subset.size(); ++i) {
        const SystemEntry* a = subset[i];
        if (a->writes.empty()) {
            continue;
        }
        std::unordered_set<uint32_t> writes_a(a->writes.begin(), a->writes.end());
        for (std::size_t j = i + 1; j < subset.size(); ++j) {
            const SystemEntry* b = subset[j];
            for (uint32_t w : b->writes) {
                if (writes_a.count(w) != 0) {
                    std::ostringstream oss;
                    oss << "[SCHEDULER ERROR] Write conflict: "
                        << a->fqn << " and " << b->fqn
                        << " both write component type " << w;
                    last_error_ = oss.str();
                    return -1;
                }
            }
        }
    }

    // 2. Build edges A -> B (B depends on A: A writes something B reads).
    // adjacency[a_idx] = vector of dependent b indices.
    std::vector<std::vector<std::size_t>> adjacency(subset.size());
    std::vector<int32_t> in_degree(subset.size(), 0);
    for (std::size_t i = 0; i < subset.size(); ++i) {
        const SystemEntry* a = subset[i];
        if (a->writes.empty()) {
            continue;
        }
        std::unordered_set<uint32_t> writes_a(a->writes.begin(), a->writes.end());
        for (std::size_t j = 0; j < subset.size(); ++j) {
            if (i == j) {
                continue;
            }
            const SystemEntry* b = subset[j];
            for (uint32_t r : b->reads) {
                if (writes_a.count(r) != 0) {
                    adjacency[i].push_back(j);
                    in_degree[j]++;
                    break;
                }
            }
        }
    }

    // 3. Kahn's algorithm — phase per layer of in_degree 0.
    std::vector<bool> visited(subset.size(), false);
    std::size_t visited_count = 0;
    while (visited_count < subset.size()) {
        std::vector<uint32_t> phase_ids;
        std::vector<std::size_t> phase_indices;
        for (std::size_t i = 0; i < subset.size(); ++i) {
            if (!visited[i] && in_degree[i] == 0) {
                phase_ids.push_back(subset[i]->id);
                phase_indices.push_back(i);
            }
        }
        if (phase_ids.empty()) {
            std::ostringstream oss;
            oss << "[SCHEDULER ERROR] Cyclic dependency detected — remaining systems:";
            for (std::size_t i = 0; i < subset.size(); ++i) {
                if (!visited[i]) {
                    oss << " " << subset[i]->fqn;
                }
            }
            last_error_ = oss.str();
            return -2;
        }
        // Sort phase ids deterministically for stable output across runs.
        std::sort(phase_ids.begin(), phase_ids.end());
        out_phases.push_back(std::move(phase_ids));
        for (std::size_t idx : phase_indices) {
            visited[idx] = true;
            visited_count++;
            for (std::size_t target : adjacency[idx]) {
                in_degree[target]--;
            }
        }
    }
    return 1;
}

SystemGraph& default_scheduler_graph() {
    static SystemGraph instance;
    return instance;
}

} // namespace dualfrontier
