#include "scheduler_trace.h"

#include <algorithm>

namespace dualfrontier {

SchedulerTrace::SchedulerTrace() = default;
SchedulerTrace::~SchedulerTrace() = default;

void SchedulerTrace::push(const TraceEvent& evt) noexcept {
    if (!enabled_.load(std::memory_order_acquire)) return;
    uint32_t idx = write_index_.fetch_add(1, std::memory_order_acq_rel);
    ring_[idx % kRingCapacity] = evt;
}

int32_t SchedulerTrace::dump(TraceEvent* out_buffer, int32_t out_capacity) const noexcept {
    if (out_buffer == nullptr || out_capacity <= 0) return 0;
    uint32_t total_written = write_index_.load(std::memory_order_acquire);
    int32_t recorded = static_cast<int32_t>(
        std::min<uint32_t>(total_written, static_cast<uint32_t>(kRingCapacity)));
    if (recorded == 0) return 0;
    int32_t n = std::min(recorded, out_capacity);
    // Copy в reverse-chronological: most recent first.
    uint32_t base = (total_written - 1) % kRingCapacity;
    for (int32_t i = 0; i < n; ++i) {
        uint32_t pos = (base + kRingCapacity - static_cast<uint32_t>(i)) % kRingCapacity;
        out_buffer[i] = ring_[pos];
    }
    return n;
}

void SchedulerTrace::clear() noexcept {
    write_index_.store(0, std::memory_order_release);
}

int32_t SchedulerTrace::event_count() const noexcept {
    uint32_t total = write_index_.load(std::memory_order_acquire);
    return static_cast<int32_t>(std::min<uint32_t>(total, kRingCapacity));
}

SchedulerTrace& default_scheduler_trace() {
    static SchedulerTrace instance;
    return instance;
}

} // namespace dualfrontier
