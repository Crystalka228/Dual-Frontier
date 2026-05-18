#include "scheduler_intrinsics.h"

#include <cstdio>
#include <cstring>

#include "scheduler_trace.h"
#include "system_graph.h"
#include "wake_registry.h"

namespace dualfrontier {

SchedulerIntrinsics::SchedulerIntrinsics() = default;
SchedulerIntrinsics::~SchedulerIntrinsics() = default;

void SchedulerIntrinsics::panic_halt(const char* message) {
    panic_.store(true, std::memory_order_release);
    suspended_.store(true, std::memory_order_release);
    panic_message_ = message != nullptr ? std::string(message) : std::string();
    // К10.1: no actual abort; managed side reads is_panic + panic_message и
    // surfaces. К10.2 may route к IModFaultSink or trigger graceful shutdown.
}

void SchedulerIntrinsics::reset() noexcept {
    suspended_.store(false, std::memory_order_release);
    panic_.store(false, std::memory_order_release);
    panic_message_.clear();
}

int32_t SchedulerIntrinsics::snapshot(char* out_buffer, int32_t out_capacity) const {
    if (out_buffer == nullptr || out_capacity <= 0) return 0;
    const auto& graph = default_scheduler_graph();
    const auto& registry = default_wake_registry();
    int n = std::snprintf(out_buffer, static_cast<size_t>(out_capacity),
        "K10.1 scheduler snapshot — systems=%zu runqueue=%d "
        "static_phases=%d per_tick_phases=%d suspended=%d panic=%d",
        graph.system_count(),
        registry.runqueue_size(),
        graph.static_phase_count(),
        graph.per_tick_phase_count(),
        is_suspended() ? 1 : 0,
        is_panic() ? 1 : 0);
    if (n < 0) return 0;
    if (n >= out_capacity) n = out_capacity - 1;
    return n;
}

SchedulerIntrinsics& default_scheduler_intrinsics() {
    static SchedulerIntrinsics instance;
    return instance;
}

} // namespace dualfrontier
