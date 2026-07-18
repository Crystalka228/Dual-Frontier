#include "mod_unload.h"

#include <atomic>
#include <cstring>
#include <mutex>

#include "bus_native.h"
#include "bus_native_internal.h"
#include "pipeline_slot.h"
#include "singleton_guard.h"

namespace {

// К-L18 quiescent state stub — К10.2 default: paused (suitable для tests +
// ModIntegrationPipeline Step 3.5 invocation which already holds the
// _isRunning=false check). К10.3 wire-up connects this к the actual sim
// thread state via settings menu integration.
std::atomic<int32_t> g_sim_paused{1};

// F-50 — fail-loud single-thread guard flag for the unload entry point below.
// The unload mutates process-global bus registries and runs under the K-L18
// sim-paused contract, so concurrent entry is a contract violation to reject,
// not a legitimate parallel call. Mirrors the F-29 SingletonGuard(busy_) shape.
std::atomic<bool> g_unload_busy{false};

void copy_error_message(ModUnloadResult* result, const char* msg) {
    if (result == nullptr || msg == nullptr) return;
    if (result->error_count < 8) {
        char* dst = result->error_messages[result->error_count];
        size_t i = 0;
        for (; i < 255 && msg[i] != '\0'; ++i) {
            dst[i] = msg[i];
        }
        dst[i] = '\0';
        ++result->error_count;
    }
}

} // namespace

extern "C" {

DF_API int32_t df_scheduler_set_sim_paused(int32_t paused) {
    g_sim_paused.store(paused != 0 ? 1 : 0, std::memory_order_release);
    return 1;
}

DF_API int32_t df_scheduler_is_sim_paused(void) {
    return g_sim_paused.load(std::memory_order_acquire);
}

DF_API int32_t df_scheduler_unload_mod_native_state(
    uint32_t mod_id, ModUnloadResult* out_result)
{
    if (out_result == nullptr) return 0;
    std::memset(out_result, 0, sizeof(*out_result));

    // F-50 — fail-loud on concurrent entry (a detector, not a lock). This path
    // mutates process-global bus registries; the single-threaded unload contract
    // makes concurrent entry a violation to reject rather than corrupt. Return 0
    // (this shipped entry point keeps its grandfathered 0/1 convention).
    dualfrontier::SingletonGuard guard(g_unload_busy);
    if (!guard.acquired()) {
        copy_error_message(out_result,
            "concurrent unload rejected: single-threaded unload contract violated");
        return 0;
    }

    // К-L18 precondition (К10.2 baseline): simulation thread must be paused.
    if (g_sim_paused.load(std::memory_order_acquire) == 0) {
        copy_error_message(out_result,
            "K-L18 quiescent state precondition violated: sim is not paused");
        return 0;
    }

    // К-L18 precondition extended (К10.3 v2 Item 41): pipeline slots quiescent
    // (no Dispatched/FenceCompleted slots = no in-flight compute work). When the
    // pipeline is не initialized (depth==0), df_pipeline_is_quiescent returns -1
    // and writes 0 — treat as quiescent for К10.3 v2 mod unload (no pipeline =
    // no in-flight compute by definition). When initialized, require all slots
    // Empty или ReadableAsTail per К-L18 spec verbatim: «pipeline slots quiescent
    // (all fences completed); no concurrent compute dispatches in-flight».
    int32_t pipeline_is_quiescent = 0;
    const int32_t pipeline_query = df_pipeline_is_quiescent(&pipeline_is_quiescent);
    if (pipeline_query == 1 && pipeline_is_quiescent == 0) {
        copy_error_message(out_result,
            "K-L18 quiescent state precondition violated: pipeline slots not "
            "quiescent (in-flight compute dispatches)");
        return 0;
    }

    // T0: Per-tier cleanup. After 2026-05-21 bus refactor each tier owns
    // its own state + mutex; each unsubscribe call acquires only the
    // relevant tier mutex. The cross-tier «single critical section»
    // concept is gone — К10.3 wire-up should reconsider whether atomicity
    // across tiers is actually required (it wasn't enforced before the
    // refactor either, since per-tier sub calls already lock-released).

    // T1: Fast tier — clear subscriptions + drop in-flight events.
    // Fast events не stored (synchronous dispatch); in_flight_dropped=0.
    out_result->fast_subscriptions_cleared = df_bus_unsubscribe_fast_by_mod(mod_id);
    out_result->fast_in_flight_dropped = 0;

    // T2: Normal tier — drain current batch к commit boundary, then clear subs.
    out_result->normal_events_drained = df_bus_drain_normal_batch();
    out_result->normal_batch_commit_completed = 1;
    out_result->normal_subscriptions_cleared = df_bus_unsubscribe_normal_by_mod(mod_id);

    // T3: Background tier — clear subscriber registry; queue contents preserved
    // per S3-Q3/Q4 untargeted persistence.
    out_result->background_subscriptions_cleared = df_bus_unsubscribe_background_by_mod(mod_id);

    int32_t bg_subs_remaining = 0;
    int32_t bg_events_preserved = 0;
    {
        auto& bg_tier = dualfrontier::BusNative::background();
        std::lock_guard<std::mutex> tier_lock(bg_tier.mutex);
        for (auto& [type_id, subs] : bg_tier.subscribers) {
            bg_subs_remaining += static_cast<int32_t>(subs.size());
        }
        bg_events_preserved = static_cast<int32_t>(bg_tier.pending.size());
    }
    out_result->background_subscriber_count_remaining = bg_subs_remaining;
    out_result->background_events_preserved = bg_events_preserved;

    // T4: Revoke fast/background tier capabilities per FQN — К10.3 wire-up
    out_result->capabilities_revoked = 0;

    // T5: Clear ShmWriter/ShmReader registrations + CPU affinity — К10.3 wire-up

    // T6: Clear wake registry subscriptions — К10.3 wire-up.
    // Per Q-N-48 orderly teardown: Explicit → Init → StateChange → Event → Timer
    out_result->wake_subscriptions_cleared = 0;

    // T7: Unregister system access declarations — К10.3 wire-up

    out_result->success = 1;
    return 1;
}

} // extern "C"
