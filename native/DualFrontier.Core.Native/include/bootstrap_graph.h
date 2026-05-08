#pragma once

#include <atomic>
#include <functional>
#include <memory>
#include <string>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

class ThreadPool;

// Declarative bootstrap dependency graph executed via Kahn's topological sort.
//
// Per Q3 architectural decision (cleanness): full Kahn implementation, not
// hand-coded ordering. Dependencies declared in task data, not in code order.
//
// Per Q4 architectural decision (cleanness): all-or-nothing with deterministic
// rollback. On any failure, completed tasks have cleanup() invoked in reverse
// completion order.
//
// Thread-safety: graph construction (add_task) is single-threaded. run() spawns
// tasks on a thread pool, internal coordination via atomics and mutex.
//
// Failure modes:
//   - add_task: std::invalid_argument (duplicate name, unknown dependency)
//   - run: std::logic_error (cycle detected) is caught internally and reported
//          via last_failure(); run() returns false instead of throwing.
//   - run: task body exceptions captured into last_failure() and surfaced via
//          a false return value after rollback.
class BootstrapGraph {
public:
    using TaskFn = std::function<void()>;
    using CleanupFn = std::function<void()>;

    BootstrapGraph();
    ~BootstrapGraph();

    BootstrapGraph(const BootstrapGraph&) = delete;
    BootstrapGraph& operator=(const BootstrapGraph&) = delete;

    // Adds a task to the graph.
    //   name: unique identifier (used for dependency references).
    //   dependencies: names of tasks that must complete before this one.
    //   execute: function performing the task's work.
    //   cleanup: function reversing execute()'s side-effects. Called in
    //            reverse completion order if any task fails.
    // Throws std::invalid_argument on duplicate name or unknown dependency.
    void add_task(std::string name,
                  std::vector<std::string> dependencies,
                  TaskFn execute,
                  CleanupFn cleanup);

    // Executes the graph through Kahn's topological sort + parallel dispatch.
    //
    // On success: returns true, all tasks completed.
    // On failure: rollback executed, returns false. last_failure() carries
    //             the exception message captured from the first failing task.
    //
    // Atomic from caller's perspective: либо all tasks committed либо all
    // rolled back. No partial visibility.
    bool run(ThreadPool& pool);

    // Last failure message after a false return from run(). Empty if no
    // failure or run() not yet called.
    [[nodiscard]] const std::string& last_failure() const noexcept {
        return last_failure_;
    }

private:
    struct Task {
        std::string name;
        std::vector<std::string> dependencies;
        TaskFn execute;
        CleanupFn cleanup;
        std::atomic<int> remaining_deps{0};
        std::atomic<bool> completed{false};
    };

    std::vector<std::unique_ptr<Task>> tasks_;
    std::unordered_map<std::string, Task*> by_name_;
    std::string last_failure_;

    // Topological sort using Kahn's algorithm. Returns levels: vector of
    // vectors, where each inner vector contains tasks with no remaining
    // unsatisfied dependencies at that level (all schedulable in parallel).
    // Throws std::logic_error if a cycle is detected.
    std::vector<std::vector<Task*>> kahn_topological_sort();

    // Run cleanup functions in reverse order of completed_order.
    void rollback(const std::vector<Task*>& completed_order);
};

} // namespace dualfrontier
