#pragma once

#include <cstdint>
#include <unordered_set>
#include <vector>

namespace dualfrontier {

// K10.1 Item 3 — Wake-up registry (К-L13 on-demand system activation).
//
// Maps wake sources к subscribed systems. Five wake types supported per
// К-L13 verbatim:
//
//   1. TimerWake       — periodic by [TickRate] (subsumes K3 TickScheduler)
//   2. EventWake       — bus publication subscription (К10.2 bus integration
//                        delivers payloads; K10.1 stores subscription metadata
//                        and fires by event_type_id only)
//   3. StateChangeWake — component value condition crossing (Item 17
//                        write-through hook drives firing at commit time)
//   4. InitWake        — one-shot at startup (after SignalEngineReady)
//   5. ExplicitWake    — API-driven wake by another system
//
// The registry maintains:
//   - Per-type subscription tables (5 tables; one per wake type)
//   - A runqueue of system ids whose wake fired since last drain
//
// Per K-L14 default-inclusion: all 5 wake types implemented upfront in K10.1.
class WakeRegistry {
public:
    // Wake type enum. Values are stable for C ABI cross-layer use.
    enum class WakeType : int32_t {
        Timer       = 0,
        Event       = 1,
        StateChange = 2,
        Init        = 3,
        Explicit    = 4,
    };

    struct TimerSubscription {
        uint32_t system_id;
        uint32_t ticks_per_update;  // mirrors [TickRate] value
    };
    struct EventSubscription {
        uint32_t system_id;
        uint32_t event_type_id;
    };
    struct StateSubscription {
        uint32_t system_id;
        uint32_t component_type_id;
    };
    struct InitSubscription {
        uint32_t system_id;
        bool fired;  // one-shot — flipped к true after first fire
    };
    struct ExplicitSubscription {
        uint32_t system_id;
        uint32_t wake_id;  // opaque token, paired с fire_explicit()
    };

    WakeRegistry();
    ~WakeRegistry();

    WakeRegistry(const WakeRegistry&) = delete;
    WakeRegistry& operator=(const WakeRegistry&) = delete;

    // Subscribe API. Each returns true on success, false on invalid input
    // (e.g. ticks_per_update == 0 для TimerWake).
    bool subscribe_timer(uint32_t system_id, uint32_t ticks_per_update);
    bool subscribe_event(uint32_t system_id, uint32_t event_type_id);
    bool subscribe_state(uint32_t system_id, uint32_t component_type_id);
    bool subscribe_init(uint32_t system_id);
    bool subscribe_explicit(uint32_t system_id, uint32_t wake_id);

    // Unsubscribe a system from a specific wake type. Returns count of
    // subscriptions removed (0 if none matched).
    int32_t unsubscribe(uint32_t system_id, WakeType type);

    // Fire wake events per type. Each adds matching subscribers к the
    // runqueue. Returns count of systems woken.
    int32_t fire_timer(uint64_t current_tick);
    int32_t fire_event(uint32_t event_type_id);
    int32_t fire_state_change(uint32_t component_type_id, uint32_t entity_id);
    int32_t fire_init();
    int32_t fire_explicit(uint32_t target_system_id, uint32_t wake_id);

    // Runqueue access. drain_runqueue copies up к out_capacity ids into
    // out_buffer, returns count written, and clears the runqueue.
    int32_t drain_runqueue(uint32_t* out_buffer, int32_t out_capacity);
    [[nodiscard]] int32_t runqueue_size() const noexcept {
        return static_cast<int32_t>(runqueue_.size());
    }

    // Diagnostic: count subscriptions per type.
    [[nodiscard]] int32_t subscription_count(WakeType type) const noexcept;

    // Reset to empty state.
    void clear() noexcept;

    // Iteration accessors (used by diagnostic API in Item 4).
    const std::vector<TimerSubscription>& timer_subs() const noexcept { return timer_subs_; }
    const std::vector<EventSubscription>& event_subs() const noexcept { return event_subs_; }
    const std::vector<StateSubscription>& state_subs() const noexcept { return state_subs_; }
    const std::vector<InitSubscription>&  init_subs()  const noexcept { return init_subs_; }
    const std::vector<ExplicitSubscription>& explicit_subs() const noexcept { return explicit_subs_; }

private:
    std::vector<TimerSubscription> timer_subs_;
    std::vector<EventSubscription> event_subs_;
    std::vector<StateSubscription> state_subs_;
    std::vector<InitSubscription> init_subs_;
    std::vector<ExplicitSubscription> explicit_subs_;
    std::unordered_set<uint32_t> runqueue_;

    void add_to_runqueue(uint32_t system_id);
};

// Process-global default wake registry (paired with default_scheduler_graph).
WakeRegistry& default_wake_registry();

} // namespace dualfrontier
