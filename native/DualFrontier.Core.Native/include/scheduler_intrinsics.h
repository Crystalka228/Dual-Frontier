#pragma once

#include <atomic>
#include <cstdint>
#include <string>

namespace dualfrontier {

// K10.1 Item 20 — Scheduler intrinsics for emergency paths.
//
// Atomic scheduler self-modification primitives. Used during hot reload,
// state migration, debugging snapshots, panic conditions. Mirrors Linux
// cli/sti, stop_machine semantics.
class SchedulerIntrinsics {
public:
    SchedulerIntrinsics();
    ~SchedulerIntrinsics();

    SchedulerIntrinsics(const SchedulerIntrinsics&) = delete;
    SchedulerIntrinsics& operator=(const SchedulerIntrinsics&) = delete;

    // Suspend all dispatch. Running systems complete; no new dispatch.
    void suspend() noexcept { suspended_.store(true, std::memory_order_release); }

    // Resume normal operation.
    void resume() noexcept { suspended_.store(false, std::memory_order_release); }

    [[nodiscard]] bool is_suspended() const noexcept {
        return suspended_.load(std::memory_order_acquire);
    }

    // Emergency stop. Flushes any pending trace flush; sets fatal flag.
    // Subsequent calls к is_panic() return true.
    void panic_halt(const char* message);

    [[nodiscard]] bool is_panic() const noexcept {
        return panic_.load(std::memory_order_acquire);
    }

    [[nodiscard]] const std::string& panic_message() const noexcept {
        return panic_message_;
    }

    // Clear panic + suspend state (for test isolation).
    void reset() noexcept;

    // Snapshot the current scheduler state into a caller-provided buffer.
    // К10.1: simple textual representation; К10.2 expands к binary structured
    // format когда richer debugger tooling lands.
    // Returns count of bytes written (excluding null terminator).
    int32_t snapshot(char* out_buffer, int32_t out_capacity) const;

private:
    std::atomic<bool> suspended_{false};
    std::atomic<bool> panic_{false};
    std::string panic_message_;
};

SchedulerIntrinsics& default_scheduler_intrinsics();

} // namespace dualfrontier
