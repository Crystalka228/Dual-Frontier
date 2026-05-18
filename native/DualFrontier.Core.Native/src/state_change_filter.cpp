#include "state_change_filter.h"

#include "wake_registry.h"

namespace dualfrontier {

StateChangeFilter::StateChangeFilter() {
    for (auto& slot : wake_subscriber_type_filter_) {
        slot.store(0, std::memory_order_relaxed);
    }
}

StateChangeFilter::~StateChangeFilter() = default;

bool StateChangeFilter::may_have_subscribers(uint32_t component_type_id) const noexcept {
    if (component_type_id >= kTypeSlotCount) {
        // Type id outside the 256-bit Level 1 range — conservative true.
        return true;
    }
    const uint32_t word = component_type_id / 64u;
    const uint32_t bit = component_type_id % 64u;
    uint64_t mask = uint64_t{1} << bit;
    return (wake_subscriber_type_filter_[word].load(std::memory_order_acquire) & mask) != 0;
}

bool StateChangeFilter::has_entity_specific_subscriber(
    uint32_t component_type_id, uint32_t entity_id) const noexcept {
    if (component_type_id >= kTypeSlotCount) return false;
    const auto& f = per_type_filters_[component_type_id];
    return f.subscribed_entities.count(entity_id) != 0;
}

void StateChangeFilter::subscribe_type(uint32_t component_type_id,
                                       uint32_t subscriber_system_id) {
    if (component_type_id >= kTypeSlotCount) return;
    type_subscriber_ids_[component_type_id].insert(subscriber_system_id);
    per_type_filters_[component_type_id].has_type_wide_subscribers = true;
    refresh_type_bit(component_type_id);
}

void StateChangeFilter::subscribe_entity(uint32_t component_type_id,
                                          uint32_t entity_id,
                                          uint32_t subscriber_system_id) {
    if (component_type_id >= kTypeSlotCount) return;
    entity_subscriber_ids_[component_type_id].insert(subscriber_system_id);
    per_type_filters_[component_type_id].subscribed_entities.insert(entity_id);
    refresh_type_bit(component_type_id);
}

void StateChangeFilter::unsubscribe_type(uint32_t component_type_id,
                                         uint32_t subscriber_system_id) {
    if (component_type_id >= kTypeSlotCount) return;
    type_subscriber_ids_[component_type_id].erase(subscriber_system_id);
    per_type_filters_[component_type_id].has_type_wide_subscribers =
        !type_subscriber_ids_[component_type_id].empty();
    refresh_type_bit(component_type_id);
}

void StateChangeFilter::unsubscribe_entity(uint32_t component_type_id,
                                            uint32_t entity_id,
                                            uint32_t subscriber_system_id) {
    if (component_type_id >= kTypeSlotCount) return;
    entity_subscriber_ids_[component_type_id].erase(subscriber_system_id);
    if (entity_subscriber_ids_[component_type_id].empty()) {
        per_type_filters_[component_type_id].subscribed_entities.erase(entity_id);
    }
    refresh_type_bit(component_type_id);
}

void StateChangeFilter::refresh_type_bit(uint32_t component_type_id) {
    if (component_type_id >= kTypeSlotCount) return;
    const auto& f = per_type_filters_[component_type_id];
    bool any = f.has_type_wide_subscribers || !f.subscribed_entities.empty();
    const uint32_t word = component_type_id / 64u;
    const uint32_t bit = component_type_id % 64u;
    uint64_t mask = uint64_t{1} << bit;
    if (any) {
        wake_subscriber_type_filter_[word].fetch_or(mask, std::memory_order_acq_rel);
    } else {
        wake_subscriber_type_filter_[word].fetch_and(~mask, std::memory_order_acq_rel);
    }
}

void StateChangeFilter::clear() noexcept {
    for (auto& slot : wake_subscriber_type_filter_) {
        slot.store(0, std::memory_order_release);
    }
    for (auto& f : per_type_filters_) {
        f.subscribed_entities.clear();
        f.has_type_wide_subscribers = false;
    }
    for (auto& s : type_subscriber_ids_) s.clear();
    for (auto& s : entity_subscriber_ids_) s.clear();
}

int32_t StateChangeFilter::type_wide_subscriber_count(uint32_t component_type_id) const noexcept {
    if (component_type_id >= kTypeSlotCount) return 0;
    return static_cast<int32_t>(type_subscriber_ids_[component_type_id].size());
}

int32_t StateChangeFilter::entity_subscriber_count(uint32_t component_type_id) const noexcept {
    if (component_type_id >= kTypeSlotCount) return 0;
    return static_cast<int32_t>(entity_subscriber_ids_[component_type_id].size());
}

void df_native_world_commit_hook_impl(uint32_t component_type_id, uint32_t entity_id) {
    auto& filter = default_state_change_filter();
    if (!filter.may_have_subscribers(component_type_id)) {
        return;  // cold path: 99% of writes hit here at low subscriber density
    }
    // Level 1 hit. Fire к wake registry — К10.1 simply triggers type-wide
    // wake; per-entity condition expression eval extends к К10.2 (condition
    // closures across C ABI).
    default_wake_registry().fire_state_change(component_type_id, entity_id);
}

StateChangeFilter& default_state_change_filter() {
    static StateChangeFilter instance;
    return instance;
}

} // namespace dualfrontier
