#pragma once

#include <atomic>
#include <array>
#include <cstdint>

namespace dualfrontier {

// K10.1 Item 19 — Observability hooks (perf/ftrace-like tracing).
//
// Lock-free ring buffer of trace events. Default sampling rate = off (zero
// overhead unless explicitly enabled). К-L14 architectural completeness for
// sovereign kernel — real OS has ftrace, eBPF, perf_event subsystem.

enum class TraceEventType : int32_t {
    SystemWoken      = 0,
    SystemDispatched = 1,
    SystemCompleted  = 2,
    PhaseStarted     = 3,
    PhaseCompleted   = 4,
    QuotaViolation   = 5,
    FilterHit        = 6,
    FilterMiss       = 7,
};

struct TraceEvent {
    int32_t event_type;
    uint32_t arg0;  // system_id / phase_id / component_type
    uint32_t arg1;  // wake_type / thread_id / level
    int64_t timestamp_micros;  // monotonic, since pool start
    int64_t value;  // duration / count / budget / actual
};

class SchedulerTrace {
public:
    static constexpr int kRingCapacity = 1024;

    SchedulerTrace();
    ~SchedulerTrace();

    SchedulerTrace(const SchedulerTrace&) = delete;
    SchedulerTrace& operator=(const SchedulerTrace&) = delete;

    void set_enabled(bool enabled) noexcept {
        enabled_.store(enabled, std::memory_order_release);
    }
    [[nodiscard]] bool enabled() const noexcept {
        return enabled_.load(std::memory_order_acquire);
    }

    // Push an event (no-op when disabled). Thread-safe via atomic write index.
    void push(const TraceEvent& evt) noexcept;

    // Copy up к out_capacity recent events into out_buffer, returns count
    // written. К10.1: most-recent-first ordering.
    int32_t dump(TraceEvent* out_buffer, int32_t out_capacity) const noexcept;

    // Clear ring buffer.
    void clear() noexcept;

    [[nodiscard]] int32_t event_count() const noexcept;

private:
    std::atomic<bool> enabled_{false};
    std::array<TraceEvent, kRingCapacity> ring_{};
    std::atomic<uint32_t> write_index_{0};
};

SchedulerTrace& default_scheduler_trace();

} // namespace dualfrontier
