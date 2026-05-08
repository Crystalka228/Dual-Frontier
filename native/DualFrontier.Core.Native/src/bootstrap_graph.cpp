#include "bootstrap_graph.h"

#include "thread_pool.h"

#include <future>
#include <mutex>
#include <stdexcept>
#include <string>
#include <utility>

namespace dualfrontier {

BootstrapGraph::BootstrapGraph() = default;
BootstrapGraph::~BootstrapGraph() = default;

void BootstrapGraph::add_task(std::string name,
                              std::vector<std::string> dependencies,
                              TaskFn execute,
                              CleanupFn cleanup) {
    if (by_name_.find(name) != by_name_.end()) {
        throw std::invalid_argument(
            "BootstrapGraph: duplicate task name: " + name);
    }
    for (const auto& dep : dependencies) {
        if (by_name_.find(dep) == by_name_.end()) {
            throw std::invalid_argument(
                "BootstrapGraph: task '" + name +
                "' depends on unknown task '" + dep + "'");
        }
    }

    auto task = std::make_unique<Task>();
    task->name = std::move(name);
    task->dependencies = std::move(dependencies);
    task->execute = std::move(execute);
    task->cleanup = std::move(cleanup);
    task->remaining_deps.store(
        static_cast<int>(task->dependencies.size()),
        std::memory_order_release);

    by_name_[task->name] = task.get();
    tasks_.push_back(std::move(task));
}

std::vector<std::vector<BootstrapGraph::Task*>>
BootstrapGraph::kahn_topological_sort() {
    // Reset remaining_deps and completed (run() may be called more than
    // once in tests).
    for (auto& task : tasks_) {
        task->remaining_deps.store(
            static_cast<int>(task->dependencies.size()),
            std::memory_order_release);
        task->completed.store(false, std::memory_order_release);
    }

    // Build reverse adjacency: dep_name -> tasks_that_depend_on_it.
    std::unordered_map<std::string, std::vector<Task*>> dependents;
    for (auto& task : tasks_) {
        for (const auto& dep : task->dependencies) {
            dependents[dep].push_back(task.get());
        }
    }

    // Kahn: level 0 = tasks with no deps.
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
                int prev = dependent->remaining_deps.fetch_sub(
                    1, std::memory_order_acq_rel);
                if (prev == 1) {
                    next_level.push_back(dependent);
                }
            }
        }
        current_level = std::move(next_level);
    }

    if (processed != static_cast<int>(tasks_.size())) {
        throw std::logic_error(
            "BootstrapGraph: cycle detected (only " +
            std::to_string(processed) + " of " +
            std::to_string(tasks_.size()) + " tasks reachable)");
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

    std::vector<Task*> completed_order;
    std::mutex completed_mutex;

    std::atomic<bool> failed{false};
    std::string failure_message;
    std::mutex failure_message_mutex;

    for (const auto& level : levels) {
        if (failed.load(std::memory_order_acquire)) break;

        std::vector<std::future<void>> futures;
        futures.reserve(level.size());

        for (Task* task : level) {
            auto promise = std::make_shared<std::promise<void>>();
            futures.push_back(promise->get_future());
            pool.submit([task, promise, &failed, &failure_message,
                         &failure_message_mutex, &completed_order,
                         &completed_mutex]() {
                try {
                    if (failed.load(std::memory_order_acquire)) {
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
                    bool expected = false;
                    if (failed.compare_exchange_strong(
                            expected, true, std::memory_order_acq_rel)) {
                        std::lock_guard<std::mutex> lock(failure_message_mutex);
                        failure_message =
                            std::string("Task '") + task->name +
                            "' failed: " + e.what();
                    }
                    promise->set_value();
                } catch (...) {
                    bool expected = false;
                    if (failed.compare_exchange_strong(
                            expected, true, std::memory_order_acq_rel)) {
                        std::lock_guard<std::mutex> lock(failure_message_mutex);
                        failure_message =
                            std::string("Task '") + task->name +
                            "' failed: unknown exception";
                    }
                    promise->set_value();
                }
            });
        }

        for (auto& f : futures) {
            f.wait();
        }
    }

    if (failed.load(std::memory_order_acquire)) {
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
    for (auto it = completed_order.rbegin(); it != completed_order.rend();
         ++it) {
        try {
            (*it)->cleanup();
        } catch (...) {
            // Cleanup must not throw upward — best-effort cleanup of the
            // remaining tasks.
        }
    }
}

} // namespace dualfrontier
