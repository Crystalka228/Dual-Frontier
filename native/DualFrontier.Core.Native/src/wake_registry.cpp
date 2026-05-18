#include "wake_registry.h"

#include <algorithm>

namespace dualfrontier {

WakeRegistry::WakeRegistry() = default;
WakeRegistry::~WakeRegistry() = default;

bool WakeRegistry::subscribe_timer(uint32_t system_id, uint32_t ticks_per_update) {
    if (ticks_per_update == 0) return false;
    timer_subs_.push_back({system_id, ticks_per_update});
    return true;
}

bool WakeRegistry::subscribe_event(uint32_t system_id, uint32_t event_type_id) {
    event_subs_.push_back({system_id, event_type_id});
    return true;
}

bool WakeRegistry::subscribe_state(uint32_t system_id, uint32_t component_type_id) {
    state_subs_.push_back({system_id, component_type_id});
    return true;
}

bool WakeRegistry::subscribe_init(uint32_t system_id) {
    init_subs_.push_back({system_id, /*fired*/false});
    return true;
}

bool WakeRegistry::subscribe_explicit(uint32_t system_id, uint32_t wake_id) {
    explicit_subs_.push_back({system_id, wake_id});
    return true;
}

int32_t WakeRegistry::unsubscribe(uint32_t system_id, WakeType type) {
    int32_t removed = 0;
    auto remove_matching = [&](auto& vec, auto predicate) {
        auto before = vec.size();
        vec.erase(std::remove_if(vec.begin(), vec.end(), predicate), vec.end());
        removed += static_cast<int32_t>(before - vec.size());
    };
    switch (type) {
        case WakeType::Timer:
            remove_matching(timer_subs_, [system_id](const TimerSubscription& s) {
                return s.system_id == system_id;
            });
            break;
        case WakeType::Event:
            remove_matching(event_subs_, [system_id](const EventSubscription& s) {
                return s.system_id == system_id;
            });
            break;
        case WakeType::StateChange:
            remove_matching(state_subs_, [system_id](const StateSubscription& s) {
                return s.system_id == system_id;
            });
            break;
        case WakeType::Init:
            remove_matching(init_subs_, [system_id](const InitSubscription& s) {
                return s.system_id == system_id;
            });
            break;
        case WakeType::Explicit:
            remove_matching(explicit_subs_, [system_id](const ExplicitSubscription& s) {
                return s.system_id == system_id;
            });
            break;
    }
    return removed;
}

void WakeRegistry::add_to_runqueue(uint32_t system_id) {
    runqueue_.insert(system_id);
}

int32_t WakeRegistry::fire_timer(uint64_t current_tick) {
    int32_t fired = 0;
    for (const TimerSubscription& s : timer_subs_) {
        if (s.ticks_per_update != 0 && (current_tick % s.ticks_per_update) == 0) {
            add_to_runqueue(s.system_id);
            ++fired;
        }
    }
    return fired;
}

int32_t WakeRegistry::fire_event(uint32_t event_type_id) {
    int32_t fired = 0;
    for (const EventSubscription& s : event_subs_) {
        if (s.event_type_id == event_type_id) {
            add_to_runqueue(s.system_id);
            ++fired;
        }
    }
    return fired;
}

int32_t WakeRegistry::fire_state_change(uint32_t component_type_id,
                                        uint32_t /*entity_id*/) {
    // K10.1: condition evaluation is delegated к Item 17 write-through hook
    // (Commit 12). The K10.1 registry simply enumerates type-wide subscribers
    // and adds them к the runqueue. K10.2+ amend k-component-condition
    // matching once condition expressions cross C ABI cleanly.
    int32_t fired = 0;
    for (const StateSubscription& s : state_subs_) {
        if (s.component_type_id == component_type_id) {
            add_to_runqueue(s.system_id);
            ++fired;
        }
    }
    return fired;
}

int32_t WakeRegistry::fire_init() {
    int32_t fired = 0;
    for (InitSubscription& s : init_subs_) {
        if (!s.fired) {
            add_to_runqueue(s.system_id);
            s.fired = true;
            ++fired;
        }
    }
    return fired;
}

int32_t WakeRegistry::fire_explicit(uint32_t target_system_id, uint32_t wake_id) {
    int32_t fired = 0;
    for (const ExplicitSubscription& s : explicit_subs_) {
        if (s.system_id == target_system_id && s.wake_id == wake_id) {
            add_to_runqueue(s.system_id);
            ++fired;
        }
    }
    return fired;
}

int32_t WakeRegistry::drain_runqueue(uint32_t* out_buffer, int32_t out_capacity) {
    if (out_buffer == nullptr || out_capacity <= 0) {
        runqueue_.clear();
        return 0;
    }
    std::vector<uint32_t> ids(runqueue_.begin(), runqueue_.end());
    std::sort(ids.begin(), ids.end());
    int32_t n = static_cast<int32_t>(ids.size());
    if (n > out_capacity) n = out_capacity;
    for (int32_t i = 0; i < n; ++i) {
        out_buffer[i] = ids[static_cast<std::size_t>(i)];
    }
    runqueue_.clear();
    return n;
}

int32_t WakeRegistry::subscription_count(WakeType type) const noexcept {
    switch (type) {
        case WakeType::Timer:       return static_cast<int32_t>(timer_subs_.size());
        case WakeType::Event:       return static_cast<int32_t>(event_subs_.size());
        case WakeType::StateChange: return static_cast<int32_t>(state_subs_.size());
        case WakeType::Init:        return static_cast<int32_t>(init_subs_.size());
        case WakeType::Explicit:    return static_cast<int32_t>(explicit_subs_.size());
    }
    return 0;
}

void WakeRegistry::clear() noexcept {
    timer_subs_.clear();
    event_subs_.clear();
    state_subs_.clear();
    init_subs_.clear();
    explicit_subs_.clear();
    runqueue_.clear();
}

WakeRegistry& default_wake_registry() {
    static WakeRegistry instance;
    return instance;
}

} // namespace dualfrontier
