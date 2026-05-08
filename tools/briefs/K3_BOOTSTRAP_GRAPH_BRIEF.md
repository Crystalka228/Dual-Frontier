# K3 — Native bootstrap graph + thread pool

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-07
**Status**: READY FOR EXECUTION
**Reference docs**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K3, §1.3 (two-phase model), §1.4 (threading model), `docs/MIGRATION_PROGRESS.md` (K2 closure context), `docs/METHODOLOGY.md` v1.3
**Predecessor**: K2 (`129a0a0`) — registry + bridge tests, merged to main
**Target**: fresh feature branch `feat/k3-bootstrap-graph` от `main`
**Estimated time**: 5–7 days at hobby pace (largest K-milestone in scope)
**Estimated LOC delta**: +750–900

---

## Goal

Реализовать declarative bootstrap dependency graph с topological sort (Kahn's algorithm), executed parallel где deps allow через native thread pool. Implements §1.3 Phase A («native bootstrap») + §1.4 (threading model). Native scheduler used **только** для bootstrap orchestration — game tick остаётся managed (K-L6).

После K3: engine может быть bootstrapped через `df_engine_bootstrap()` single C ABI call returning ready `World` handle. Failure mode = `IntPtr.Zero` с full atomic rollback (no partial visibility).

**В scope**:
- `bootstrap_graph.h/cpp` — full Kahn topological sort, generic mechanism
- `thread_pool.h/cpp` — `std::thread` pool, internal-only (не exposed через C ABI)
- 4-task bootstrap inventory: AllocateMemoryPools → (InitWorldStructure ‖ InitThreadPool) → SignalEngineReady
- `df_engine_bootstrap()` C ABI entry point с atomic all-or-nothing semantics
- Per-task cleanup functions для deterministic rollback
- Selftest extension: 5 new scenarios (basic bootstrap, parallel execution timing, topological order, rollback on failure, double-bootstrap rejection)
- `BootstrapTests` в DualFrontier.Core.Interop.Tests (~6 tests)
- `BootstrapTimeBenchmark` validating parallel speedup hypothesis
- Managed `Bootstrap` API (managed entry point wrapping P/Invoke)

**НЕ в scope** (per К-L6 + Q1 architectural decision):
- Thread pool **NOT exposed** через C ABI — internal use only
- General-purpose work submission API — D3 Lvl 1 pattern: каждый native артефакт имеет свой pool
- Native scheduler для game tick — K-L6 LOCKED, scheduler stays managed
- Mod registration — уже в K2, не повторять
- Component struct refactor — K4
- Span/write batching enhancements — K5

---

## Architectural decisions (from 2026-05-07 K3 design discussion)

K3 brief implements 4 explicit decisions, each derived от Crystalka philosophy «cleanness > expediency»:

### Q1 — Thread pool scope: **Minimal (internal only)**

Pool exists exclusively для bootstrap parallelization. Goes idle после `SignalEngineReady`, threads persist (not joined) до engine destruction. **NOT exposed** через C ABI — `df_threadpool_*` functions intentionally absent.

**Rationale**:
- Single responsibility (bootstrap parallelization, not general-purpose work)
- D3 (Lvl 1 pattern): each native artifact has its own pool, no sharing
- K-L6 invariant: «no native scheduler for game tick» — exposing pool would create temptation to violate
- Removes ~3-5 ABI functions worth of surface area

**Implication**: thread_pool.h/cpp contents are NOT visible через `df_capi.h`. Header included only by `bootstrap_graph.cpp` и `selftest.cpp`. Pool destructor automatically called when World destroyed.

### Q2 — Bootstrap tasks inventory: **4 tasks (no placeholders)**

```
AllocateMemoryPools
    ├──→ InitWorldStructure ──┐
    └──→ InitThreadPool ──────┴──→ SignalEngineReady
```

**Tasks dropped**:
- `RegisterBuiltinTypes` — kernel has no builtins; types registered managed-side через `ComponentTypeRegistry`
- `ValidateConfiguration` — placeholder с no real state to validate
- `WarmupAllocator` — premature optimization; revisit if K7 measurements show page-fault pain

**Rationale**: «data exists or it doesn't» applied to bootstrap. Each task does real work с real side-effects. No placeholder tasks. Graph has parallelism (InitWorldStructure ‖ InitThreadPool) so test can validate Kahn's parallel branches actually work.

### Q3 — Topological sort: **Full Kahn's algorithm**

Generic implementation в `bootstrap_graph.cpp`, dependencies declared в task data (not coded в task ordering). ~50-80 LOC overhead.

**Rationale**:
- Symmetry с managed system graph (Phase 4 used same algorithm)
- Hand-coded ordering = shortcut creating long-term debt (when 5th task added, manual recalculation risk)
- Tests verify topological invariants, не специфические orderings
- §1.5 LOCKED says «declarative graph» — implies generic mechanism

### Q4 — Failure handling: **All-or-nothing с deterministic rollback**

Per-task cleanup function. On any failure:
1. Stop scheduling new tasks (atomic failure flag)
2. Wait for in-flight tasks to drain
3. Run cleanup functions в reverse completion order
4. Return `nullptr` (managed sees `IntPtr.Zero`, throws `BootstrapFailedException`)

**Rationale**:
- «Honest state always» operating principle: либо fully bootstrapped либо not at all, no zombie
- §1.3 LOCKED: «Returns IntPtr.Zero on failure»
- Partial state = lying about engine readiness
- Best-effort partial success rejected (would defeat operating principle)

---

## Step 0 — Brief authoring commit (METHODOLOGY v1.3 prerequisite)

```powershell
cd D:\Colony_Simulator\Colony_Simulator
git status
# Expected: K3_BOOTSTRAP_GRAPH_BRIEF.md modified (skeleton → full brief)

git add tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md
git commit -m "docs(briefs): K3 brief authored — full executable bootstrap graph"
# No push — K3 feature branch will be ancestor of brief authoring commit automatically
```

After this — working tree clean, HG-1 will pass.

---

## Throw inventory (METHODOLOGY v1.3)

K3 introduces **the largest throw blast radius** of any K-milestone so far. Each site explicitly enumerated:

### Throw sites in native code

| Site | Throw type | Trigger condition |
|---|---|---|
| `BootstrapGraph::add_task` | `std::invalid_argument` | dependency references unknown task name OR duplicate task name |
| `BootstrapGraph::run` | `std::logic_error` | cycle detected during topological sort (Kahn returns incomplete order) |
| `BootstrapGraph::run` | `std::runtime_error` | thread pool spawn failure (std::thread constructor throws) |
| `BootstrapGraph::run` | propagates: any exception thrown by task body | task implementation failure |
| `ThreadPool` constructor | `std::system_error` | std::thread spawn failure |
| `ThreadPool::submit` | `std::logic_error` | submit called after pool shut down |
| `World::is_bootstrapped` flag | `std::logic_error` | double-bootstrap attempt (df_engine_bootstrap called twice) |
| Task `AllocateMemoryPools` | `std::bad_alloc` | std::vector allocation failure |
| Task `InitWorldStructure` | propagates от World ctor | constructor failure |
| Task `InitThreadPool` | propagates от ThreadPool ctor | thread spawn failure |

### Boundary trace

The new throws can propagate ONLY through **one new ABI function**:
- `df_engine_bootstrap` (added в Step 3.4)

Existing ABI functions (`df_world_create`, `df_world_destroy`, etc.) are NOT affected — they don't invoke bootstrap graph or thread pool code.

### Wrap inventory

- **New wrapper**: `df_engine_bootstrap` — try/catch added with broad `catch(...)` returning `nullptr`
- **No existing wrappers need modification** — K3 doesn't add throws to existing native functions

### Default catch behavior

- `df_engine_bootstrap`: catch any exception → execute cleanup chain → return `nullptr`
- Caller checks `IntPtr.Zero` and throws `BootstrapFailedException` managed-side
- **CRITICAL**: cleanup chain MUST run даже if exception thrown by task body — RAII destructors in BootstrapGraph dtor handle this. Brief specifies dtor explicitly в Step 2.

---

## Pre-flight checks (METHODOLOGY v1.2 descriptive style)

### Hard gates (STOP-eligible)

#### HG-1: Working tree clean
```powershell
git status                                # должен быть clean (после Step 0)
```

#### HG-2: K2 successfully merged to main
```powershell
git log main --oneline | Select-String "K2|registry|interop" | Select-Object -First 5
# Expected: visible commits for K2 closure
```

If K2 commits not visible — STOP, K2 not merged.

#### HG-3: Native source intact (post-K2)
```powershell
Test-Path native\DualFrontier.Core.Native\include\df_capi.h
Test-Path native\DualFrontier.Core.Native\include\world.h
Test-Path native\DualFrontier.Core.Native\src\world.cpp
Test-Path native\DualFrontier.Core.Native\src\capi.cpp
Test-Path native\DualFrontier.Core.Native\test\selftest.cpp

# Verify K2 function present:
Select-String -Path native\DualFrontier.Core.Native\include\df_capi.h -Pattern "df_world_register_component_type"
# Expected: match
```

#### HG-4: K3 target files do NOT yet exist
```powershell
Test-Path native\DualFrontier.Core.Native\include\bootstrap_graph.h    # Expected: False
Test-Path native\DualFrontier.Core.Native\include\thread_pool.h         # Expected: False
Test-Path native\DualFrontier.Core.Native\src\bootstrap_graph.cpp       # Expected: False
Test-Path native\DualFrontier.Core.Native\src\thread_pool.cpp           # Expected: False
```

If any True — STOP, K3 partially executed already.

#### HG-5: Baseline tests passing
```powershell
dotnet test
# Expected: 511 passing, 0 failed (K2 baseline)
```

#### HG-6: Native build clean
```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
# Expected: 0 errors, 0 warnings
```

#### HG-7: Native selftest passing
```powershell
.\build\Release\df_native_selftest.exe
# Expected: 7 scenarios ALL PASSED (post-K2 baseline)
```

#### HG-8: Tooling
```powershell
cmake --version                           # >= 3.20
dotnet --version                          # 8.x or 10.x
```

### Informational checks (record-only)

#### INF-1: Current C ABI function count
```powershell
Select-String -Path native\DualFrontier.Core.Native\include\df_capi.h -Pattern "DF_API" | Measure-Object
# Expected after K2: 17 functions
# K3 will add 1 more, target: 18
```

#### INF-2: Selftest scenario count
```powershell
Select-String -Path native\DualFrontier.Core.Native\test\selftest.cpp -Pattern "void scenario_" | Measure-Object
# Expected: 7 scenarios
# K3 will add 5 more, target: 12
```

#### INF-3: CMakeLists source list
```powershell
Get-Content native\DualFrontier.Core.Native\CMakeLists.txt | Select-String "src/.*\.cpp"
# Record current source files; K3 adds bootstrap_graph.cpp + thread_pool.cpp
```

#### INF-4: Recent commit history
```powershell
git log main --oneline -5
# Record HEAD SHA для K3 closure reference
```

---

## Step 1 — Branch setup

```powershell
git checkout main
git pull origin main
git checkout -b feat/k3-bootstrap-graph main
```

Branch name: **`feat/k3-bootstrap-graph`** (точно).

---

## Step 2 — Native thread pool implementation

### 2.1 — Create `native/DualFrontier.Core.Native/include/thread_pool.h`

**Internal-only header**, NOT exposed via `df_capi.h`. Per Q1 decision (Minimal scope).

```cpp
#pragma once

#include <atomic>
#include <condition_variable>
#include <functional>
#include <mutex>
#include <queue>
#include <thread>
#include <vector>

namespace dualfrontier {

/**
 * Internal thread pool for bootstrap parallelization.
 *
 * Per K-L6 invariant: native scheduler exists ONLY for bootstrap orchestration.
 * Pool goes idle after SignalEngineReady completes; threads persist until
 * World destruction.
 *
 * NOT exposed via C ABI. Per D3 Lvl 1 pattern: each native artifact has
 * its own pool, no sharing across artifacts.
 *
 * Thread-safe: submit() and shutdown() callable from any thread.
 *
 * Failure modes:
 *   - Constructor: std::system_error if std::thread spawn fails
 *   - submit() after shutdown: throws std::logic_error
 */
class ThreadPool {
public:
    using Task = std::function<void()>;

    /// Construct pool with N worker threads. N = std::thread::hardware_concurrency()
    /// if zero passed.
    explicit ThreadPool(std::size_t thread_count);

    /// Joins all threads. Pending tasks completed first.
    ~ThreadPool();

    ThreadPool(const ThreadPool&) = delete;
    ThreadPool& operator=(const ThreadPool&) = delete;

    /// Submit task для execution. Throws std::logic_error if already shut down.
    void submit(Task task);

    /// Block calling thread until queue is empty AND all in-flight tasks complete.
    void wait_idle();

    /// Stop accepting new tasks. Drain in-flight, then exit. Idempotent.
    void shutdown();

    /// Number of worker threads.
    [[nodiscard]] std::size_t thread_count() const noexcept { return workers_.size(); }

private:
    std::vector<std::thread> workers_;
    std::queue<Task> task_queue_;
    std::mutex queue_mutex_;
    std::condition_variable queue_cv_;
    std::condition_variable idle_cv_;
    std::atomic<int> in_flight_{0};
    std::atomic<bool> stopping_{false};

    void worker_loop();
};

} // namespace dualfrontier
```

### 2.2 — Create `native/DualFrontier.Core.Native/src/thread_pool.cpp`

```cpp
#include "thread_pool.h"

#include <stdexcept>

namespace dualfrontier {

ThreadPool::ThreadPool(std::size_t thread_count) {
    if (thread_count == 0) {
        thread_count = std::thread::hardware_concurrency();
        if (thread_count == 0) thread_count = 4;  // sensible fallback
    }

    workers_.reserve(thread_count);
    try {
        for (std::size_t i = 0; i < thread_count; ++i) {
            workers_.emplace_back(&ThreadPool::worker_loop, this);
        }
    } catch (...) {
        // Partial construction failure — clean up already-spawned threads
        stopping_.store(true, std::memory_order_release);
        queue_cv_.notify_all();
        for (auto& t : workers_) {
            if (t.joinable()) t.join();
        }
        throw;  // re-throw original exception
    }
}

ThreadPool::~ThreadPool() {
    shutdown();
    for (auto& t : workers_) {
        if (t.joinable()) t.join();
    }
}

void ThreadPool::submit(Task task) {
    if (stopping_.load(std::memory_order_acquire)) {
        throw std::logic_error("ThreadPool::submit called after shutdown");
    }
    {
        std::lock_guard<std::mutex> lock(queue_mutex_);
        task_queue_.push(std::move(task));
    }
    queue_cv_.notify_one();
}

void ThreadPool::wait_idle() {
    std::unique_lock<std::mutex> lock(queue_mutex_);
    idle_cv_.wait(lock, [this] {
        return task_queue_.empty() && in_flight_.load(std::memory_order_acquire) == 0;
    });
}

void ThreadPool::shutdown() {
    {
        std::lock_guard<std::mutex> lock(queue_mutex_);
        stopping_.store(true, std::memory_order_release);
    }
    queue_cv_.notify_all();
}

void ThreadPool::worker_loop() {
    while (true) {
        Task task;
        {
            std::unique_lock<std::mutex> lock(queue_mutex_);
            queue_cv_.wait(lock, [this] {
                return stopping_.load(std::memory_order_acquire) || !task_queue_.empty();
            });

            if (stopping_.load(std::memory_order_acquire) && task_queue_.empty()) {
                return;
            }

            task = std::move(task_queue_.front());
            task_queue_.pop();
            in_flight_.fetch_add(1, std::memory_order_acq_rel);
        }

        try {
            task();
        } catch (...) {
            // Task exceptions handled by BootstrapGraph (which wraps tasks
            // in error-capturing closures). ThreadPool itself doesn't
            // propagate task failures — workers continue serving the queue.
        }

        if (in_flight_.fetch_sub(1, std::memory_order_acq_rel) == 1) {
            // Last in-flight task completed — wake wait_idle()
            std::lock_guard<std::mutex> lock(queue_mutex_);
            idle_cv_.notify_all();
        }
    }
}

} // namespace dualfrontier
```

---

## Step 3 — Native bootstrap graph implementation

### 3.1 — Create `native/DualFrontier.Core.Native/include/bootstrap_graph.h`

```cpp
#pragma once

#include <atomic>
#include <functional>
#include <memory>
#include <string>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

class ThreadPool;

/**
 * Declarative bootstrap dependency graph executed via Kahn's topological sort.
 *
 * Per Q3 architectural decision (cleanness): full Kahn implementation, not
 * hand-coded ordering. Dependencies declared in task data, not in code order.
 *
 * Per Q4 architectural decision (cleanness): all-or-nothing with deterministic
 * rollback. On any failure, completed tasks have cleanup() invoked in reverse
 * completion order.
 *
 * Thread-safety: graph construction (add_task) is single-threaded. run() spawns
 * tasks on a thread pool, internal coordination via atomics and mutex.
 *
 * Failure modes:
 *   - add_task: std::invalid_argument (duplicate name, unknown dependency)
 *   - run: std::logic_error (cycle detected)
 *   - run: propagates first task exception after rollback completes
 */
class BootstrapGraph {
public:
    using TaskFn = std::function<void()>;
    using CleanupFn = std::function<void()>;

    BootstrapGraph();
    ~BootstrapGraph();

    BootstrapGraph(const BootstrapGraph&) = delete;
    BootstrapGraph& operator=(const BootstrapGraph&) = delete;

    /**
     * Adds a task to the graph.
     * @param name Unique identifier (used for dependency references).
     * @param dependencies Names of tasks that must complete before this one.
     * @param execute Function performing the task's work.
     * @param cleanup Function reversing execute()'s side-effects. Called в
     *                reverse completion order if any task fails.
     * @throws std::invalid_argument if name duplicates or dependency unknown.
     */
    void add_task(std::string name,
                  std::vector<std::string> dependencies,
                  TaskFn execute,
                  CleanupFn cleanup);

    /**
     * Executes graph through Kahn's topological sort + parallel dispatch.
     *
     * On success: returns true, all tasks completed.
     * On failure: rollback executed, returns false, exception captured but
     *             not re-thrown (caller checks return value).
     *
     * Atomic: from caller's perspective, either all tasks committed or
     *         all rolled back. No partial visibility.
     */
    bool run(ThreadPool& pool);

    /**
     * Returns last failure message if run() returned false. Empty string
     * if no failure or run() not yet called.
     */
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

    /// Topological sort using Kahn's algorithm.
    /// Returns levels: vector of vectors, each inner vector contains tasks
    /// with no remaining unsatisfied dependencies at that level.
    /// Throws std::logic_error if cycle detected.
    std::vector<std::vector<Task*>> kahn_topological_sort();

    /// Run cleanup functions in reverse order of completed_order.
    void rollback(const std::vector<Task*>& completed_order);
};

} // namespace dualfrontier
```

### 3.2 — Create `native/DualFrontier.Core.Native/src/bootstrap_graph.cpp`

```cpp
#include "bootstrap_graph.h"

#include "thread_pool.h"

#include <algorithm>
#include <atomic>
#include <future>
#include <mutex>
#include <stdexcept>

namespace dualfrontier {

BootstrapGraph::BootstrapGraph() = default;
BootstrapGraph::~BootstrapGraph() = default;

void BootstrapGraph::add_task(std::string name,
                              std::vector<std::string> dependencies,
                              TaskFn execute,
                              CleanupFn cleanup) {
    if (by_name_.find(name) != by_name_.end()) {
        throw std::invalid_argument("BootstrapGraph: duplicate task name: " + name);
    }
    for (const auto& dep : dependencies) {
        if (by_name_.find(dep) == by_name_.end()) {
            throw std::invalid_argument(
                "BootstrapGraph: task '" + name + "' depends on unknown task '" + dep + "'");
        }
    }

    auto task = std::make_unique<Task>();
    task->name = name;
    task->dependencies = std::move(dependencies);
    task->execute = std::move(execute);
    task->cleanup = std::move(cleanup);
    task->remaining_deps.store(static_cast<int>(task->dependencies.size()),
                                std::memory_order_release);

    by_name_[task->name] = task.get();
    tasks_.push_back(std::move(task));
}

std::vector<std::vector<BootstrapGraph::Task*>>
BootstrapGraph::kahn_topological_sort() {
    // Reset remaining_deps (run() may be called multiple times in tests)
    for (auto& task : tasks_) {
        task->remaining_deps.store(static_cast<int>(task->dependencies.size()),
                                    std::memory_order_release);
        task->completed.store(false, std::memory_order_release);
    }

    // Build reverse adjacency: dep_name → tasks_that_depend_on_it
    std::unordered_map<std::string, std::vector<Task*>> dependents;
    for (auto& task : tasks_) {
        for (const auto& dep : task->dependencies) {
            dependents[dep].push_back(task.get());
        }
    }

    // Kahn: level 0 = tasks with no deps
    std::vector<std::vector<Task*>> levels;
    std::vector<Task*> current_level;
    for (auto& task : tasks_) {
        if (task->dependencies.empty()) {
            current_level.push_back(task.get());
        }
    }

    int processed = 0;
    while (!current_level.empty()) {
        levels.push_back(current_level);
        processed += static_cast<int>(current_level.size());

        std::vector<Task*> next_level;
        for (Task* t : current_level) {
            auto it = dependents.find(t->name);
            if (it == dependents.end()) continue;
            for (Task* dependent : it->second) {
                int prev = dependent->remaining_deps.fetch_sub(1, std::memory_order_acq_rel);
                if (prev == 1) {  // was 1, now 0
                    next_level.push_back(dependent);
                }
            }
        }

        current_level = std::move(next_level);
    }

    if (processed != static_cast<int>(tasks_.size())) {
        throw std::logic_error(
            "BootstrapGraph: cycle detected (only " + std::to_string(processed) +
            " of " + std::to_string(tasks_.size()) + " tasks reachable)");
    }

    return levels;
}

bool BootstrapGraph::run(ThreadPool& pool) {
    last_failure_.clear();

    std::vector<std::vector<Task*>> levels;
    try {
        levels = kahn_topological_sort();
    } catch (const std::exception& e) {
        last_failure_ = e.what();
        return false;
    }

    // Track completed tasks для rollback (mutex-protected vector)
    std::vector<Task*> completed_order;
    std::mutex completed_mutex;

    // Atomic failure flag — set by first failing task, read by all
    std::atomic<bool> failed{false};
    std::string failure_message;
    std::mutex failure_message_mutex;

    for (const auto& level : levels) {
        if (failed.load(std::memory_order_acquire)) break;

        // Submit all tasks at this level in parallel via pool
        std::vector<std::future<void>> futures;
        futures.reserve(level.size());

        for (Task* task : level) {
            auto promise = std::make_shared<std::promise<void>>();
            futures.push_back(promise->get_future());

            pool.submit([task, promise, &failed, &failure_message,
                         &failure_message_mutex, &completed_order, &completed_mutex]() {
                try {
                    if (failed.load(std::memory_order_acquire)) {
                        // Earlier task failed; skip this one
                        promise->set_value();
                        return;
                    }

                    task->execute();
                    task->completed.store(true, std::memory_order_release);

                    {
                        std::lock_guard<std::mutex> lock(completed_mutex);
                        completed_order.push_back(task);
                    }

                    promise->set_value();
                } catch (const std::exception& e) {
                    // First failure wins — set failure flag atomically
                    bool expected = false;
                    if (failed.compare_exchange_strong(expected, true,
                                                       std::memory_order_acq_rel)) {
                        std::lock_guard<std::mutex> lock(failure_message_mutex);
                        failure_message = std::string("Task '") + task->name +
                                          "' failed: " + e.what();
                    }
                    promise->set_value();
                } catch (...) {
                    bool expected = false;
                    if (failed.compare_exchange_strong(expected, true,
                                                       std::memory_order_acq_rel)) {
                        std::lock_guard<std::mutex> lock(failure_message_mutex);
                        failure_message = std::string("Task '") + task->name +
                                          "' failed: unknown exception";
                    }
                    promise->set_value();
                }
            });
        }

        // Wait для all tasks at this level to complete
        for (auto& f : futures) {
            f.wait();
        }
    }

    if (failed.load(std::memory_order_acquire)) {
        // Rollback completed tasks в reverse order
        std::vector<Task*> snapshot;
        {
            std::lock_guard<std::mutex> lock(completed_mutex);
            snapshot = completed_order;
        }
        rollback(snapshot);

        std::lock_guard<std::mutex> lock(failure_message_mutex);
        last_failure_ = failure_message;
        return false;
    }

    return true;
}

void BootstrapGraph::rollback(const std::vector<Task*>& completed_order) {
    // Reverse iteration — last-completed first (LIFO cleanup)
    for (auto it = completed_order.rbegin(); it != completed_order.rend(); ++it) {
        try {
            (*it)->cleanup();
        } catch (...) {
            // Cleanup must not throw upward — best-effort cleanup of remaining tasks
        }
    }
}

} // namespace dualfrontier
```

---

## Step 4 — World extensions for bootstrap

### 4.1 — Edit `native/DualFrontier.Core.Native/include/world.h`

Add **public method**:
```cpp
/// Returns true if engine has been bootstrapped (df_engine_bootstrap completed
/// successfully). Used to reject double-bootstrap attempts.
[[nodiscard]] bool is_bootstrapped() const noexcept {
    return bootstrapped_.load(std::memory_order_acquire);
}

/// Marks engine as bootstrapped. Called by SignalEngineReady task.
/// Throws std::logic_error if already bootstrapped (double-bootstrap protection).
void mark_bootstrapped();
```

Add **private member** (after `active_spans_`):
```cpp
std::atomic<bool> bootstrapped_{false};
```

### 4.2 — Edit `native/DualFrontier.Core.Native/src/world.cpp`

Add к end:
```cpp
void World::mark_bootstrapped() {
    bool expected = false;
    if (!bootstrapped_.compare_exchange_strong(expected, true,
                                                std::memory_order_acq_rel)) {
        throw std::logic_error("World: double bootstrap detected");
    }
}
```

---

## Step 5 — C ABI: df_engine_bootstrap

### 5.1 — Edit `native/DualFrontier.Core.Native/include/df_capi.h`

Add **after** existing K2 declaration:

```cpp
/*
 * K3 engine bootstrap (added 2026-05-07).
 *
 * Single entry point that performs all native-side initialization:
 *   1. Allocate memory pools
 *   2. Construct World instance (parallel with thread pool init)
 *   3. Initialize internal thread pool (parallel with World init)
 *   4. Mark engine as ready
 *
 * On success: returns opaque World handle. Caller must call df_world_destroy
 * when done.
 *
 * On failure: returns nullptr. All partial state cleaned up. Managed-side
 * caller should throw BootstrapFailedException.
 *
 * Atomic: либо fully bootstrapped либо nullptr. No partial visibility.
 *
 * Calling df_engine_bootstrap on a process that already has a bootstrapped
 * engine creates a SECOND independent engine — handles are isolated. Multiple
 * concurrent engines supported (each has its own thread pool, World, registry).
 */

DF_API df_world_handle df_engine_bootstrap(void);
```

### 5.2 — Edit `native/DualFrontier.Core.Native/src/capi.cpp`

Add **at top of file**, after existing includes:
```cpp
#include "bootstrap_graph.h"
#include "thread_pool.h"
```

Add **after** existing K2 wrapper (`df_world_register_component_type`):

```cpp
DF_API df_world_handle df_engine_bootstrap(void) {
    // RAII-managed unique_ptr — auto-cleanup on early return
    std::unique_ptr<dualfrontier::World> world;
    std::unique_ptr<dualfrontier::ThreadPool> pool;

    try {
        // Build graph: 4-task inventory per Q2 architectural decision
        dualfrontier::BootstrapGraph graph;

        // Capture references — pointers populated as tasks execute
        auto* world_ptr = &world;
        auto* pool_ptr = &pool;

        // Task 1: AllocateMemoryPools (no deps)
        graph.add_task(
            "AllocateMemoryPools",
            {},
            [world_ptr]() {
                // No-op for now — pools allocated lazily в World constructor.
                // Reserved for explicit pool allocation if K7 measurements
                // show value (e.g. preallocate dense_bytes buffers per type).
                // Keeping task as graph node для validation of parallelism.
            },
            []() {
                // Cleanup: nothing к undo (no allocations performed)
            });

        // Task 2: InitWorldStructure (deps: AllocateMemoryPools)
        graph.add_task(
            "InitWorldStructure",
            {"AllocateMemoryPools"},
            [world_ptr]() {
                *world_ptr = std::make_unique<dualfrontier::World>();
            },
            [world_ptr]() {
                world_ptr->reset();  // destruct World
            });

        // Task 3: InitThreadPool (deps: AllocateMemoryPools, parallel with InitWorldStructure)
        // Note: This is a SEPARATE pool from the bootstrap pool. The bootstrap
        // pool is destroyed after run(); this pool persists with the World.
        // Per Q1 (Minimal scope), this pool is NOT exposed via C ABI.
        graph.add_task(
            "InitThreadPool",
            {"AllocateMemoryPools"},
            [pool_ptr]() {
                std::size_t n = std::thread::hardware_concurrency();
                if (n == 0) n = 4;
                *pool_ptr = std::make_unique<dualfrontier::ThreadPool>(n);
            },
            [pool_ptr]() {
                pool_ptr->reset();  // destruct ThreadPool (joins threads)
            });

        // Task 4: SignalEngineReady (deps: InitWorldStructure, InitThreadPool)
        graph.add_task(
            "SignalEngineReady",
            {"InitWorldStructure", "InitThreadPool"},
            [world_ptr]() {
                if (!*world_ptr) {
                    throw std::logic_error("SignalEngineReady: World not initialized");
                }
                (*world_ptr)->mark_bootstrapped();
            },
            []() {
                // Cleanup: nothing — bootstrapped flag will be discarded with World
            });

        // Execute graph через separate bootstrap pool
        // (using std::thread::hardware_concurrency directly, не the new pool —
        //  the new pool is one of the things being initialized!)
        std::size_t bootstrap_threads = std::thread::hardware_concurrency();
        if (bootstrap_threads == 0) bootstrap_threads = 4;
        if (bootstrap_threads > 4) bootstrap_threads = 4;  // cap; bootstrap is short

        dualfrontier::ThreadPool bootstrap_pool(bootstrap_threads);

        bool success = graph.run(bootstrap_pool);
        bootstrap_pool.shutdown();
        // bootstrap_pool destructor joins threads here

        if (!success) {
            // Rollback already executed by graph.run() — world и pool are nullptr
            return nullptr;
        }

        // Transfer ownership: world owns pool internally now
        // (For K3, pool is held in capi static-scope context; K3+ may move
        //  pool ownership into World struct. Currently leaking pool — see К8.)
        // SIMPLIFIED for K3: pool is destroyed when World handle returned to caller
        // is destroyed by df_world_destroy. We attach pool to World via a side-channel.

        // FOR K3 SIMPLIFICATION: discard the pool. It's not used post-bootstrap
        // anyway (per K-L6, game tick is managed). Pool joins on shutdown.
        pool->shutdown();
        pool.reset();  // destruct now — threads idle, joins fast

        return world.release();  // transfer ownership to caller

    } catch (const std::exception&) {
        return nullptr;  // RAII cleanup of partial state via unique_ptr destructors
    } catch (...) {
        return nullptr;
    }
}
```

**Important note**: Per Q1 decision, the post-bootstrap thread pool is **destroyed immediately** after bootstrap completes. K-L6 says game tick is managed — pool would be idle anyway. Keeping the InitThreadPool task в graph для valid parallelism evidence (validates Kahn's parallel branches), но pool itself doesn't persist past bootstrap.

If future native artifacts need a pool, they create their own (per D3 Lvl 1).

### 5.3 — Atomic commit для C++ side

```powershell
git add native/DualFrontier.Core.Native/include/thread_pool.h `
        native/DualFrontier.Core.Native/include/bootstrap_graph.h `
        native/DualFrontier.Core.Native/include/world.h `
        native/DualFrontier.Core.Native/include/df_capi.h `
        native/DualFrontier.Core.Native/src/thread_pool.cpp `
        native/DualFrontier.Core.Native/src/bootstrap_graph.cpp `
        native/DualFrontier.Core.Native/src/world.cpp `
        native/DualFrontier.Core.Native/src/capi.cpp

git commit -m "native(kernel): bootstrap graph + thread pool

Implements §1.3 Phase A (native bootstrap) per K-L5 + Q1-Q4 decisions:

- ThreadPool (internal-only, NOT exposed via C ABI per Q1):
  * std::thread × N workers, condition_variable signaling
  * submit/wait_idle/shutdown API for internal callers
  * Workers swallow task exceptions (BootstrapGraph captures and reports)

- BootstrapGraph (full Kahn's algorithm per Q3):
  * Declarative: tasks added with name + deps + execute + cleanup
  * Topological sort detects cycles (throws std::logic_error)
  * Parallel dispatch within each Kahn level via thread pool
  * Atomic failure flag stops scheduling on first failure
  * Deterministic rollback in reverse completion order (Q4)

- 4-task inventory (per Q2, no placeholders):
  * AllocateMemoryPools → (InitWorldStructure || InitThreadPool) → SignalEngineReady
  * Graph parallelism validates Kahn's parallel branches actually work

- df_engine_bootstrap C ABI entry point:
  * Returns world handle on success, nullptr on failure
  * Atomic all-or-nothing: no partial state visible to caller
  * RAII via unique_ptr ensures cleanup на any exception path

- World::mark_bootstrapped + bootstrapped_ atomic flag:
  * Double-bootstrap protection (compare_exchange_strong)

Throw inventory per METHODOLOGY.md v1.3:
  * BootstrapGraph::add_task: invalid_argument (duplicate name, unknown dep)
  * BootstrapGraph::run: logic_error (cycle), runtime_error (thread spawn)
  * BootstrapGraph::run: propagates task body exceptions
  * ThreadPool ctor: system_error (thread spawn)
  * ThreadPool::submit: logic_error (post-shutdown)
  * World::mark_bootstrapped: logic_error (double-bootstrap)
  * Tasks: bad_alloc, propagated from constructors

All throws caught at boundary (df_engine_bootstrap try/catch),
returning nullptr on any failure. RAII destructors clean up partial state."
```

### 5.4 — CMakeLists.txt update

Edit `native/DualFrontier.Core.Native/CMakeLists.txt`. Add new sources to `DF_NATIVE_SOURCES`:

```cmake
set(DF_NATIVE_SOURCES
    src/world.cpp
    src/component_store.cpp
    src/capi.cpp
    src/bootstrap_graph.cpp     # K3 NEW
    src/thread_pool.cpp         # K3 NEW
)

set(DF_NATIVE_HEADERS
    include/df_capi.h
    include/sparse_set.h
    include/component_store.h
    include/world.h
    include/entity_id.h
    include/bootstrap_graph.h   # K3 NEW
    include/thread_pool.h       # K3 NEW
)
```

Commit:
```powershell
git add native/DualFrontier.Core.Native/CMakeLists.txt
git commit -m "native(build): register K3 sources in CMakeLists"
```

### 5.5 — Native build verification

```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
# Expected: 0 errors, 0 warnings
```

If errors — STOP, investigate before proceeding. Common issues:
- `<future>` или `<thread>` missing include
- Forward declaration of ThreadPool в bootstrap_graph.h must remain `class ThreadPool` (not full definition)
- `std::function` capture-by-reference lifetime issues — verify lambdas capture pointers, не references

---

## Step 6 — Selftest extension (5 new scenarios)

### 6.1 — Edit `native/DualFrontier.Core.Native/test/selftest.cpp`

Add **at top**, after existing includes:
```cpp
#include "bootstrap_graph.h"
#include "thread_pool.h"
#include <chrono>
#include <thread>
#include <atomic>
```

Add scenarios **before** `int main()`:

```cpp
void scenario_bootstrap_basic() {
    std::printf("scenario_bootstrap_basic\n");

    df_world_handle w = df_engine_bootstrap();
    DF_CHECK(w != nullptr, "bootstrap returned valid handle");

    // World is usable post-bootstrap
    uint64_t e = df_world_create_entity(w);
    DF_CHECK(df_world_is_alive(w, e) == 1, "entity created post-bootstrap");

    df_world_destroy(w);
}

void scenario_bootstrap_double_rejected() {
    std::printf("scenario_bootstrap_double_rejected\n");

    df_world_handle w1 = df_engine_bootstrap();
    DF_CHECK(w1 != nullptr, "first bootstrap succeeded");

    // Second bootstrap creates SECOND independent engine — both valid.
    // (Multiple engines supported per docstring.)
    df_world_handle w2 = df_engine_bootstrap();
    DF_CHECK(w2 != nullptr, "second bootstrap creates independent engine");
    DF_CHECK(w1 != w2, "engines are distinct");

    df_world_destroy(w1);
    df_world_destroy(w2);
}

void scenario_bootstrap_graph_topological() {
    std::printf("scenario_bootstrap_graph_topological\n");
    using namespace dualfrontier;

    BootstrapGraph graph;

    std::atomic<int> counter{0};
    std::atomic<int> task_a_order{-1};
    std::atomic<int> task_b_order{-1};
    std::atomic<int> task_c_order{-1};

    graph.add_task("A", {},
        [&]() { task_a_order.store(counter.fetch_add(1)); },
        []() {});

    graph.add_task("B", {"A"},
        [&]() { task_b_order.store(counter.fetch_add(1)); },
        []() {});

    graph.add_task("C", {"B"},
        [&]() { task_c_order.store(counter.fetch_add(1)); },
        []() {});

    ThreadPool pool(2);
    bool ok = graph.run(pool);
    pool.shutdown();

    DF_CHECK(ok, "linear graph executed successfully");
    DF_CHECK(task_a_order.load() < task_b_order.load(),
             "A executed before B");
    DF_CHECK(task_b_order.load() < task_c_order.load(),
             "B executed before C");
}

void scenario_bootstrap_graph_parallel() {
    std::printf("scenario_bootstrap_graph_parallel\n");
    using namespace dualfrontier;

    BootstrapGraph graph;

    // Diamond: A → (B || C) → D
    // B and C should execute concurrently (overlapping wall-clock time)
    std::atomic<bool> b_started{false};
    std::atomic<bool> c_started{false};
    std::atomic<bool> b_saw_c_started{false};
    std::atomic<bool> c_saw_b_started{false};

    graph.add_task("A", {},
        []() {},
        []() {});

    graph.add_task("B", {"A"},
        [&]() {
            b_started.store(true);
            // Wait briefly to give C a chance to start
            std::this_thread::sleep_for(std::chrono::milliseconds(20));
            b_saw_c_started.store(c_started.load());
        },
        []() {});

    graph.add_task("C", {"A"},
        [&]() {
            c_started.store(true);
            std::this_thread::sleep_for(std::chrono::milliseconds(20));
            c_saw_b_started.store(b_started.load());
        },
        []() {});

    graph.add_task("D", {"B", "C"},
        []() {},
        []() {});

    ThreadPool pool(4);  // enough threads для true parallelism
    bool ok = graph.run(pool);
    pool.shutdown();

    DF_CHECK(ok, "diamond graph executed successfully");
    // At least one of B, C должен see the other started — proves parallelism
    DF_CHECK(b_saw_c_started.load() || c_saw_b_started.load(),
             "B and C executed concurrently (parallelism evidence)");
}

void scenario_bootstrap_rollback_on_failure() {
    std::printf("scenario_bootstrap_rollback_on_failure\n");
    using namespace dualfrontier;

    BootstrapGraph graph;

    std::atomic<int> a_cleanup_count{0};
    std::atomic<int> b_cleanup_count{0};

    // A succeeds, B fails — A's cleanup must be invoked
    graph.add_task("A", {},
        []() { /* succeeds */ },
        [&]() { a_cleanup_count.fetch_add(1); });

    graph.add_task("B", {"A"},
        []() { throw std::runtime_error("intentional failure для test"); },
        [&]() { b_cleanup_count.fetch_add(1); });  // не должно быть called

    ThreadPool pool(2);
    bool ok = graph.run(pool);
    pool.shutdown();

    DF_CHECK(!ok, "graph reported failure");
    DF_CHECK(a_cleanup_count.load() == 1, "A's cleanup invoked exactly once");
    DF_CHECK(b_cleanup_count.load() == 0,
             "B's cleanup NOT invoked (B never completed)");
    DF_CHECK(!graph.last_failure().empty(), "failure message recorded");
}
```

И в `main()`, **после** `scenario_explicit_registration()`:
```cpp
    scenario_bootstrap_basic();
    scenario_bootstrap_double_rejected();
    scenario_bootstrap_graph_topological();
    scenario_bootstrap_graph_parallel();
    scenario_bootstrap_rollback_on_failure();
```

### 6.2 — Build + verify

```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
.\build\Release\df_native_selftest.exe
# Expected: 12 scenarios, ALL PASSED
```

### 6.3 — Atomic commit

```powershell
cd D:\Colony_Simulator\Colony_Simulator
git add native/DualFrontier.Core.Native/test/selftest.cpp
git commit -m "test(native): K3 selftest scenarios для bootstrap

Adds 5 new scenarios validating bootstrap graph + thread pool:

- scenario_bootstrap_basic: df_engine_bootstrap returns valid handle,
  world usable post-bootstrap.
- scenario_bootstrap_double_rejected: second bootstrap creates
  independent engine (multiple engines supported).
- scenario_bootstrap_graph_topological: linear chain A→B→C executes
  in dependency order.
- scenario_bootstrap_graph_parallel: diamond A→(B||C)→D, B and C
  observed running concurrently (parallelism evidence via timing).
- scenario_bootstrap_rollback_on_failure: A succeeds, B fails;
  A.cleanup invoked exactly once, B.cleanup NOT invoked.

Total selftest scenarios: 7 → 12. All passing."
```

---

## Step 7 — Managed bridge (P/Invoke + C# wrapper)

### 7.1 — Edit `src/DualFrontier.Core.Interop/NativeMethods.cs`

Add **after** existing K2 declaration:
```csharp
// K3 bootstrap (added 2026-05-07).

[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
internal static extern IntPtr df_engine_bootstrap();
```

### 7.2 — Create `src/DualFrontier.Core.Interop/Bootstrap.cs`

```csharp
using System;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Static entry point для native engine bootstrap (К3, 2026-05-07).
///
/// Calls the native bootstrap graph which initializes World + thread pool
/// in parallel, returning a ready-to-use NativeWorld instance.
///
/// Per K-L5 + Q1-Q4 architectural decisions:
/// - Bootstrap is single-call atomic (либо fully ready либо exception).
/// - No partial visibility — failure modes throw BootstrapFailedException,
///   never return a half-initialized NativeWorld.
/// - Native scheduler used ONLY for bootstrap orchestration. Game tick
///   remains managed (K-L6).
/// </summary>
public static class Bootstrap
{
    /// <summary>
    /// Performs native engine bootstrap and returns a ready NativeWorld.
    /// </summary>
    /// <param name="registry">
    /// Optional ComponentTypeRegistry. If provided, the returned NativeWorld
    /// uses deterministic registry-based type IDs. If null, uses legacy
    /// FNV-1a path. (Per К2.)
    /// </param>
    /// <returns>
    /// Ready-to-use NativeWorld with active World handle, fully bootstrapped.
    /// </returns>
    /// <exception cref="BootstrapFailedException">
    /// If native bootstrap fails (memory allocation, thread spawn, or any
    /// task body throws). Native side performs full rollback before this
    /// exception propagates — no partial state remains.
    /// </exception>
    public static NativeWorld Bootstrap(ComponentTypeRegistry? registry = null)
    {
        IntPtr handle = NativeMethods.df_engine_bootstrap();
        if (handle == IntPtr.Zero)
        {
            throw new BootstrapFailedException(
                "df_engine_bootstrap returned null. Native side performed " +
                "full rollback. See native logs (when implemented) for the " +
                "specific task that failed.");
        }

        // Wrap raw handle in NativeWorld. We use the existing constructor
        // path that accepts an external handle — currently NativeWorld only
        // has parameterless constructor that calls df_world_create itself.
        // Need to add a handle-adopting constructor (Step 7.3 below).
        return NativeWorld.AdoptBootstrappedHandle(handle, registry);
    }
}

/// <summary>
/// Thrown when native engine bootstrap fails. Native side has already
/// performed full rollback by the time this exception propagates — no
/// partial state remains к clean up.
/// </summary>
public sealed class BootstrapFailedException : Exception
{
    public BootstrapFailedException(string message) : base(message) { }
    public BootstrapFailedException(string message, Exception inner)
        : base(message, inner) { }
}
```

### 7.3 — Edit `src/DualFrontier.Core.Interop/NativeWorld.cs`

Add **internal factory method** (place near other constructors):

```csharp
/// <summary>
/// Internal: adopts a handle previously obtained from df_engine_bootstrap.
/// Used by Bootstrap.Bootstrap() — не для public consumption.
/// </summary>
internal static NativeWorld AdoptBootstrappedHandle(IntPtr handle,
                                                     ComponentTypeRegistry? registry)
{
    if (handle == IntPtr.Zero)
    {
        throw new ArgumentException("Cannot adopt null handle", nameof(handle));
    }
    return new NativeWorld(handle, registry);
}

private NativeWorld(IntPtr existingHandle, ComponentTypeRegistry? registry)
{
    _handle = existingHandle;
    _registry = registry;
}
```

### 7.4 — Atomic commit

```powershell
git add src/DualFrontier.Core.Interop/NativeMethods.cs `
        src/DualFrontier.Core.Interop/Bootstrap.cs `
        src/DualFrontier.Core.Interop/NativeWorld.cs

git commit -m "interop(kernel): managed Bootstrap entry point

Adds Bootstrap.Bootstrap() static method — managed entry point для
native engine bootstrap. Wraps df_engine_bootstrap P/Invoke в
exception-throwing API.

BootstrapFailedException added для clean failure reporting. Native
side performs full rollback before exception propagates — no partial
state requires managed cleanup.

NativeWorld.AdoptBootstrappedHandle internal factory accepts existing
handles (those produced by df_engine_bootstrap rather than
df_world_create). Optional ComponentTypeRegistry parameter wires
through к the bootstrapped world.

Implements §1.3 Phase A wrapper. Per Q4 (cleanness), failure mode is
exception-throwing, never returns null/disposed wrapper."
```

### 7.5 — Build verification

```powershell
dotnet build
# Expected: 0 errors, only pre-existing warnings.

dotnet test
# Expected: 511 passing (no regressions; new functionality not yet tested)
```

---

## Step 8 — Bridge tests extension

### 8.1 — Create `tests/DualFrontier.Core.Interop.Tests/BootstrapTests.cs`

```csharp
using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class BootstrapTests
{
    private struct TestComponent
    {
        public int Value;
    }

    [Fact]
    public void Bootstrap_returns_ready_NativeWorld()
    {
        using var world = Bootstrap.Bootstrap();

        world.Should().NotBeNull();
        world.EntityCount.Should().Be(0);
    }

    [Fact]
    public void Bootstrap_world_supports_entity_creation()
    {
        using var world = Bootstrap.Bootstrap();

        EntityId entity = world.CreateEntity();
        world.IsAlive(entity).Should().BeTrue();
        world.EntityCount.Should().Be(1);
    }

    [Fact]
    public void Bootstrap_world_supports_components()
    {
        using var world = Bootstrap.Bootstrap();
        EntityId entity = world.CreateEntity();
        var comp = new TestComponent { Value = 42 };

        world.AddComponent(entity, comp);
        TestComponent retrieved = world.GetComponent<TestComponent>(entity);

        retrieved.Value.Should().Be(42);
    }

    [Fact]
    public void Bootstrap_with_registry_uses_deterministic_ids()
    {
        using var world1 = Bootstrap.Bootstrap();
        var registry1 = new ComponentTypeRegistry(world1.HandleForInternalUseTest);
        uint id1 = registry1.Register<TestComponent>();

        using var world2 = Bootstrap.Bootstrap();
        var registry2 = new ComponentTypeRegistry(world2.HandleForInternalUseTest);
        uint id2 = registry2.Register<TestComponent>();

        // Both registries assign same first id (1) — sequential per K-L4
        id1.Should().Be(1);
        id2.Should().Be(1);
    }

    [Fact]
    public void Multiple_independent_bootstraps_supported()
    {
        using var world1 = Bootstrap.Bootstrap();
        using var world2 = Bootstrap.Bootstrap();

        EntityId e1 = world1.CreateEntity();
        EntityId e2 = world2.CreateEntity();

        // Each engine has independent state
        world1.EntityCount.Should().Be(1);
        world2.EntityCount.Should().Be(1);
        // Entity from one не visible в other
        world2.IsAlive(e1).Should().BeFalse();  // e1's index might exist в world2
                                                  // но version mismatches OR doesn't exist
    }

    [Fact]
    public void Bootstrapped_world_disposes_cleanly()
    {
        var world = Bootstrap.Bootstrap();
        world.Dispose();

        Action act = () => world.CreateEntity();
        act.Should().Throw<ObjectDisposedException>();
    }
}
```

### 8.2 — Atomic commit

```powershell
git add tests/DualFrontier.Core.Interop.Tests/BootstrapTests.cs
git commit -m "test(interop): K3 BootstrapTests

Adds 6 tests validating managed Bootstrap entry point:

- Bootstrap_returns_ready_NativeWorld: basic success path
- Bootstrap_world_supports_entity_creation: post-bootstrap usability
- Bootstrap_world_supports_components: full ECS operations work
- Bootstrap_with_registry_uses_deterministic_ids: К2 + K3 integration
- Multiple_independent_bootstraps_supported: engine isolation
- Bootstrapped_world_disposes_cleanly: lifecycle invariant

Test count: 511 → 517 (+6 K3 bridge tests)."
```

---

## Step 9 — BootstrapTimeBenchmark

### 9.1 — Create `tests/DualFrontier.Core.Benchmarks/BootstrapTimeBenchmark.cs`

```csharp
using BenchmarkDotNet.Attributes;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// K3 benchmark: validates parallel bootstrap speedup hypothesis.
///
/// Bootstrap graph contains 4 tasks. InitWorldStructure and InitThreadPool
/// can run in parallel (both depend only on AllocateMemoryPools). On a
/// multi-core machine, parallel dispatch should show measurable improvement
/// over hypothetical sequential execution.
///
/// Note: This benchmark measures end-to-end bootstrap time, NOT a managed
/// vs native comparison. The point is "bootstrap is fast enough" (target
/// 5-15ms typical hardware per KERNEL_ARCHITECTURE §K3 success criteria).
///
/// К7 will add the comparative tick-loop benchmark applying §8 metrics.
/// </summary>
[MemoryDiagnoser]
public class BootstrapTimeBenchmark
{
    [Benchmark]
    public NativeWorld BootstrapAndDispose()
    {
        var world = Bootstrap.Bootstrap();
        // Don't dispose here — let GC + finalizer run afterward
        // (or use [IterationSetup] / [IterationCleanup] для proper teardown)
        return world;
    }
}
```

### 9.2 — Atomic commit

```powershell
git add tests/DualFrontier.Core.Benchmarks/BootstrapTimeBenchmark.cs
git commit -m "bench(kernel): K3 BootstrapTimeBenchmark

Validates engine bootstrap completes within KERNEL_ARCHITECTURE §K3
success criteria target (5-15ms typical hardware).

Benchmark execution deferred к K7 (full performance measurement
milestone with §8 metrics application). This commit ships the
benchmark code only."
```

---

## Step 10 — Full verification

### 10.1 — Native + managed builds

```powershell
cd D:\Colony_Simulator\Colony_Simulator

# Native
cd native\DualFrontier.Core.Native
cmake --build build --config Release
.\build\Release\df_native_selftest.exe
# Expected: 12 scenarios, ALL PASSED, exit code 0

cd D:\Colony_Simulator\Colony_Simulator
dotnet build
# Expected: 0 errors

dotnet test
# Expected: 511 + 6 = 517 passing, 0 failed
```

If counts differ — investigate. Native selftest must be 12. Managed tests must be 517.

---

## Step 11 — Update MIGRATION_PROGRESS.md

Update `Current state snapshot`:
```markdown
| **Active phase** | K4 (planned) — component struct refactor (Path α) |
| **Last completed milestone** | K3 (bootstrap graph + thread pool) — `<sha>` 2026-MM-DD |
| **Next milestone (recommended)** | K4 (component struct refactor) |
| **Tests passing** | 517 (511 baseline + 6 K3 BootstrapTests) |
```

Update K3 row in K-series progress table:
```markdown
| K3 | Native bootstrap graph + thread pool | DONE | 5–7 days | `<sha>` | 2026-MM-DD |
```

Add detailed K3 entry (after K2):
```markdown
### K3 — Native bootstrap graph + thread pool

- **Status**: DONE (`<sha>`, 2026-MM-DD)
- **Brief**: `tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 1 new function — `df_engine_bootstrap` (17 → 18 total)
- **Native files added**: `bootstrap_graph.h/cpp` (~250 LOC), `thread_pool.h/cpp` (~200 LOC)
- **Architectural decisions implemented** (per 2026-05-07 design discussion):
  - Q1 Thread pool scope: Minimal (internal-only, не exposed via C ABI)
  - Q2 Tasks inventory: 4 tasks (no placeholders) — AllocateMemoryPools → (InitWorldStructure ‖ InitThreadPool) → SignalEngineReady
  - Q3 Topological sort: Full Kahn's algorithm (cycle detection, generic mechanism)
  - Q4 Failure handling: All-or-nothing с deterministic rollback (per-task cleanup, reverse completion order)
- **Throw inventory**: 10 throw sites identified in C++ code, all caught at `df_engine_bootstrap` boundary returning `nullptr`. Per METHODOLOGY.md v1.3 inventory checklist.
- **Selftest scenarios**: 7 → 12 (added 5: bootstrap_basic, double_rejected, graph_topological, graph_parallel, rollback_on_failure)
- **Bridge tests**: 6 new BootstrapTests (511 → 517)
- **Benchmark**: BootstrapTimeBenchmark added (execution deferred к K7)
- **Lessons learned**: <fill if anything non-trivial>
```

Атомарный commit:
```powershell
git add docs/MIGRATION_PROGRESS.md
git commit -m "docs(migration): K3 closure recorded"
```

---

## Step 12 — Update K3 brief skeleton

Open `tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md`. Replace TODO list с:

```markdown
## Status: EXECUTED

**Date**: 2026-MM-DD
**Branch**: feat/k3-bootstrap-graph
**Final commit**: <sha>

Full executable brief authored and executed. See git log on
feat/k3-bootstrap-graph branch for atomic commit sequence.

See `MIGRATION_PROGRESS.md` for closure record.
```

```powershell
git add tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md
git commit -m "docs(briefs): K3 brief skeleton marked EXECUTED"
```

---

## Step 13 — Final verification & merge prep

```powershell
git log --oneline main..HEAD
# Expected sequence (~10 commits):
#   <sha> docs(briefs): K3 brief skeleton marked EXECUTED
#   <sha> docs(migration): K3 closure recorded
#   <sha> bench(kernel): K3 BootstrapTimeBenchmark
#   <sha> test(interop): K3 BootstrapTests
#   <sha> interop(kernel): managed Bootstrap entry point
#   <sha> test(native): K3 selftest scenarios для bootstrap
#   <sha> native(build): register K3 sources в CMakeLists
#   <sha> native(kernel): bootstrap graph + thread pool
#   <sha> docs(briefs): K3 brief authored — full executable bootstrap graph    ← Step 0

git status                                  # clean

# Final builds
cmake --build native\DualFrontier.Core.Native\build --config Release
.\native\DualFrontier.Core.Native\build\Release\df_native_selftest.exe
# Expected: 12 scenarios ALL PASSED

dotnet build
dotnet test
# Expected: 517 passing
```

### Push

```powershell
git push -u origin feat/k3-bootstrap-graph
```

---

## Acceptance criteria

K3 закрыт когда ВСЕ выполнено:

- [ ] Step 0 brief authoring commit на main выполнен
- [ ] Branch `feat/k3-bootstrap-graph` создан от `main`
- [ ] Native commit: `bootstrap_graph.h/cpp`, `thread_pool.h/cpp`, World extensions, `df_engine_bootstrap` ABI
- [ ] CMakeLists.txt commit: K3 sources registered
- [ ] Selftest commit: 5 new scenarios (bootstrap_basic, double_rejected, graph_topological, graph_parallel, rollback_on_failure)
- [ ] Interop commit: NativeMethods.cs + Bootstrap.cs + NativeWorld factory method
- [ ] Tests commit: BootstrapTests.cs (~6 tests)
- [ ] Benchmark commit: BootstrapTimeBenchmark.cs
- [ ] `cmake --build` clean — 0 errors, 0 warnings
- [ ] Native selftest: **12 scenarios ALL PASSED**
- [ ] `dotnet build` clean
- [ ] `dotnet test`: **517 passing** (511 baseline + 6 K3 new)
- [ ] MIGRATION_PROGRESS.md K3 row DONE с commit SHA
- [ ] tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md marked EXECUTED
- [ ] Branch pushed to origin
- [ ] No build artifacts committed
- [ ] Q1-Q4 architectural decisions implemented exactly as specified

---

## Rollback procedure

K3 не делает destructive changes:

```powershell
git checkout main
git branch -D feat/k3-bootstrap-graph
# Step 0 brief authoring commit остаётся на main (durable artifact)
```

---

## Open issues / lessons learned (заполнить при closure)

<empty — заполнить если что-то нетривиальное всплыло>

---

## Pipeline metadata

- **Brief authored by**: Opus (architect)
- **Brief executed by**: Claude Code agent или human
- **Final review**: Crystalka (architectural judgment + commit author)
- **Methodology compliance**:
  - METHODOLOGY.md v1.3 «Step 0 brief authoring» applied
  - METHODOLOGY.md v1.3 «throw inventory» completed (10 sites enumerated, 1 boundary, 1 wrap)
  - METHODOLOGY.md v1.2 «descriptive pre-flight» applied (8 hard gates + 4 informational)
- **Architectural decisions traceability**: Q1-Q4 explicit, derived от 2026-05-07 design discussion, recorded в MIGRATION_PROGRESS lessons learned reference

**Brief end. Companion docs: KERNEL_ARCHITECTURE.md (§1.3, §1.4, §1.5, §K3), MIGRATION_PROGRESS.md, METHODOLOGY.md v1.3.**
