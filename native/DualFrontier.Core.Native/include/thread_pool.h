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

// Internal thread pool for bootstrap parallelization.
//
// Per K-L6 invariant: native scheduler exists ONLY for bootstrap orchestration.
// Pool goes idle after SignalEngineReady completes; threads persist until
// explicit shutdown.
//
// NOT exposed via C ABI. Per D3 Lvl 1 pattern: each native artifact has
// its own pool, no sharing across artifacts.
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

    // Construct pool with N worker threads. N = std::thread::hardware_concurrency()
    // if zero passed (with a fallback of 4 if hardware_concurrency returns 0).
    explicit ThreadPool(std::size_t thread_count);

    // Joins all threads. Pending tasks completed first.
    ~ThreadPool();

    ThreadPool(const ThreadPool&) = delete;
    ThreadPool& operator=(const ThreadPool&) = delete;

    // Submit task for execution. Throws std::logic_error if already shut down.
    void submit(Task task);

    // Block calling thread until queue is empty AND all in-flight tasks complete.
    void wait_idle();

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

    void worker_loop();
};

} // namespace dualfrontier
