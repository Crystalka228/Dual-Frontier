#pragma once

#include <atomic>
#include <array>
#include <cstdint>
#include <unordered_set>

namespace dualfrontier {

// K10.1 Item 17 — Write-through hook (filter primitive S2 hybrid).
//
// Two-level hybrid filter per spec §3.5 S2 amendment + Q3b lock:
//   Level 1 — per-component-type atomic bitset (256 bits = 4 × uint64_t).
//             Hot-path cold-path bypass via atomic load + bit test (~2ns).
//   Level 2 — per-(type, entity) sparse hint. Hot-path Level 1 hit checks
//             Level 2 to skip enumeration when no entity-specific subscriber.
//
// Hot path cost: cold path ~2ns (Level 1 miss bypass); Level 1 hit + Level 2
// miss ~4ns (one atomic load + one bool); Level 1 hit + Level 2 hit fall
// through to O(K) subscriber enumeration where K is typically small.
//
// К-L7 atomic-from-observer preserved: write-through hook fires at commit
// time (batch boundary), not per-write. No mid-transaction observer state.
//
// К10.1: 256 component-type slots cover вся registered components у К10.1
// component count; type ids exceeding 255 fall through to «may have
// subscribers = true» (conservative — К10.2 expands к dynamic bitset if
// type id density grows).
class StateChangeFilter {
public:
    static constexpr uint32_t kTypeSlotCount = 256;

    struct EntityFilter {
        std::unordered_set<uint32_t> subscribed_entities;
        bool has_type_wide_subscribers = false;
    };

    StateChangeFilter();
    ~StateChangeFilter();

    StateChangeFilter(const StateChangeFilter&) = delete;
    StateChangeFilter& operator=(const StateChangeFilter&) = delete;

    // Hot path: cold-path bypass via Level 1 atomic load + bit test (~2ns).
    [[nodiscard]] bool may_have_subscribers(uint32_t component_type_id) const noexcept;

    // Hot path Level 1 hit: Level 2 check для entity-specific subscriber.
    [[nodiscard]] bool has_entity_specific_subscriber(
        uint32_t component_type_id, uint32_t entity_id) const noexcept;

    // Subscribe/unsubscribe — atomic filter updates.
    void subscribe_type(uint32_t component_type_id, uint32_t subscriber_system_id);
    void subscribe_entity(uint32_t component_type_id, uint32_t entity_id,
                           uint32_t subscriber_system_id);
    void unsubscribe_type(uint32_t component_type_id, uint32_t subscriber_system_id);
    void unsubscribe_entity(uint32_t component_type_id, uint32_t entity_id,
                             uint32_t subscriber_system_id);

    // Reset to empty state.
    void clear() noexcept;

    // Diagnostic counts.
    [[nodiscard]] int32_t type_wide_subscriber_count(uint32_t component_type_id) const noexcept;
    [[nodiscard]] int32_t entity_subscriber_count(uint32_t component_type_id) const noexcept;

private:
    // Level 1: per-type bitset (256 bits packed in 4 × uint64_t atomics).
    // Bit set ⇒ at least one subscriber exists для that type id.
    std::array<std::atomic<uint64_t>, 4> wake_subscriber_type_filter_{};

    // Level 2: per-type sparse entity filter + tracking of type-wide subscribers.
    std::array<EntityFilter, kTypeSlotCount> per_type_filters_;

    // Track subscriber system ids per type для unsubscribe semantics.
    std::array<std::unordered_set<uint32_t>, kTypeSlotCount> type_subscriber_ids_;
    std::array<std::unordered_set<uint32_t>, kTypeSlotCount> entity_subscriber_ids_;

    void refresh_type_bit(uint32_t component_type_id);
};

// Write-through hook (called at commit time per К-L7 atomic-from-observer).
// Looks up subscribers (filter Level 1 fast path) and fires wake_registry
// state-change wakes for matching subscriptions.
//
// К10.1 integration: callable directly by NativeWorld.WriteBatch.Commit (С#
// side). К10.2 wires the commit-time callback automatically when the mod ALC
// lifecycle context surrounds NativeWorld write paths. К10.1 leaves the
// integration call site explicit so existing tests are unaffected.
void df_native_world_commit_hook_impl(uint32_t component_type_id, uint32_t entity_id);

StateChangeFilter& default_state_change_filter();

} // namespace dualfrontier
