#include "thread_pool.h"

#include <stdexcept>

namespace dualfrontier {

ThreadPool::ThreadPool(std::size_t thread_count) {
    if (thread_count == 0) {
        thread_count = std::thread::hardware_concurrency();
        if (thread_count == 0) thread_count = 4;
    }

    workers_.reserve(thread_count);
    try {
        for (std::size_t i = 0; i < thread_count; ++i) {
            workers_.emplace_back(&ThreadPool::worker_loop, this);
        }
    } catch (...) {
        // Partial construction failure — clean up already-spawned threads.
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
        return task_queue_.empty() &&
               in_flight_.load(std::memory_order_acquire) == 0;
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
                return stopping_.load(std::memory_order_acquire) ||
                       !task_queue_.empty();
            });
            if (stopping_.load(std::memory_order_acquire) &&
                task_queue_.empty()) {
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
            std::lock_guard<std::mutex> lock(queue_mutex_);
            idle_cv_.notify_all();
        }
    }
}

} // namespace dualfrontier
