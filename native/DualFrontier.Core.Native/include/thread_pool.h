#pragma once

#include <atomic>
#include <condition_variable>
#include <cstddef>
#include <functional>
#include <mutex>
#include <queue>
#include <thread>
#include <vector>

namespace dualfrontier {

// Internal thread pool for kernel scheduler dispatch.
//
// K3-era: bootstrap-only orchestration (per superseded K-L6 framing). The
// pool went idle after SignalEngineReady and stayed dormant.
//
// K10.1 (Item 2 / К-L12): extended к per-tick scheduler dispatch. Pool now
// persists across kernel lifecycle phases via Mode tracking. The bootstrap
// → scheduler mode transition is an atomic flag flip; worker loop semantics
// are unchanged (tasks drain the same queue in either mode).
//
// Pool sizing is caller-supplied: the engine pool is constructed with full
// std::thread::hardware_concurrency() (fallback 4 when it reports 0) — see
// capi.cpp InitThreadPool; the short-lived bootstrap pool caps at 4. No core
// is reserved at this tier (the managed ParallelSystemScheduler keeps its own
// N-2 rule for its Parallel.ForEach dispatch). See
// docs/architecture/THREADING.md.
//
// Thread-safe: submit() and shutdown() callable from any thread.
//
// Failure modes:
//   - Constructor: re-throws std::system_error (or any exception) raised
//     during std::thread spawn after cleaning up partially-spawned workers.
//   - submit() after shutdown: throws std::logic_error.
class ThreadPool {
public:
    using Task = std::function<void()>;

    // Pool lifecycle mode. Bootstrap = K3-era one-shot orchestration;
    // Scheduler = K10.1 per-tick dispatch. The transition is observable
    // via diagnostic API but does not change worker behavior — tasks drain
    // the same queue regardless of mode.
    enum class Mode {
        Bootstrap = 0,
        Scheduler = 1,
    };

    // Construct pool with N worker threads. N = std::thread::hardware_concurrency()
    // if zero passed (with a fallback of 4 if hardware_concurrency returns 0).
    // Pool starts in Bootstrap mode; call transition_to_scheduler_mode() to
    // switch when SignalEngineReady completes.
    explicit ThreadPool(std::size_t thread_count);

    // Joins all threads. Pending tasks completed first.
    ~ThreadPool();

    ThreadPool(const ThreadPool&) = delete;
    ThreadPool& operator=(const ThreadPool&) = delete;

    // Submit a single task. Throws std::logic_error if already shut down.
    void submit(Task task);

    // K10.1 Item 2 — submit a batch of tasks atomically (single queue lock
    // acquisition). Equivalent to repeated submit() calls but eliminates
    // notify_one storm и batch-mid contention. Throws std::logic_error if
    // already shut down.
    void submit_batch(std::vector<Task> tasks);

    // Block calling thread until queue is empty AND all in-flight tasks complete.
    void wait_idle();

    // K10.1 Item 2 — phase barrier semantics. Wait for all submitted tasks
    // в current batch к complete before next phase dispatches. Functionally
    // identical к wait_idle() at K10.1 (К-L13 Item 13 phase barrier types
    // land in Commit 10; here we install the canonical method name).
    void wait_phase_barrier() { wait_idle(); }

    // K10.1 Item 2 — bootstrap → scheduler mode transition. Idempotent.
    // Called once after SignalEngineReady completes (managed side). After
    // this call, the pool participates in per-tick scheduler dispatch.
    void transition_to_scheduler_mode() noexcept;

    // K10.1 Item 2 — scheduler → bootstrap mode reversion. Used during
    // graceful shutdown or hot reload (К10.2 mod lifecycle integration).
    void transition_to_bootstrap_mode() noexcept;

    // Current lifecycle mode (lock-free atomic read).
    [[nodiscard]] Mode current_mode() const noexcept {
        return mode_.load(std::memory_order_acquire);
    }

    // K10.1 Item 12 — Work-stealing enablement flag. К10.1 lands the policy
    // toggle; the actual per-thread deque implementation extends к К11+ when
    // contention measurements support the architectural choice. Default true
    // (per К-L14 default-inclusion).
    void set_work_stealing_enabled(bool enabled) noexcept {
        work_stealing_.store(enabled, std::memory_order_release);
    }
    [[nodiscard]] bool work_stealing_enabled() const noexcept {
        return work_stealing_.load(std::memory_order_acquire);
    }

    // Stop accepting new tasks. Drain in-flight, then exit. Idempotent.
    void shutdown();

    // Number of worker threads.
    [[nodiscard]] std::size_t thread_count() const noexcept {
        return workers_.size();
    }

private:
    std::vector<std::thread> workers_;
    std::queue<Task> task_queue_;
    std::mutex queue_mutex_;
    std::condition_variable queue_cv_;
    std::condition_variable idle_cv_;
    std::atomic<int> in_flight_{0};
    std::atomic<bool> stopping_{false};
    std::atomic<Mode> mode_{Mode::Bootstrap};
    std::atomic<bool> work_stealing_{true};

    void worker_loop();
};

} // namespace dualfrontier
