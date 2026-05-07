#pragma once

#include <atomic>
#include <cstdint>
#include <memory>
#include <unordered_map>
#include <vector>

#include "component_store.h"
#include "entity_id.h"

namespace dualfrontier {

// Native mirror of src/DualFrontier.Core/ECS/World.cs.
//
// Intentional simplifications for the PoC:
//   * Not thread-safe. The managed World uses a ConcurrentDictionary for its
//     store map; here we use std::unordered_map because this experiment does
//     not yet exercise the parallel scheduler path.
//   * Deferred destruction is preserved — DestroyEntity only flips the
//     version and queues the id; components are dropped in
//     FlushDestroyedEntities.
//   * Index 0 is reserved as Invalid, same as the managed version.
class World {
public:
    World();

    EntityId create_entity();
    void     destroy_entity(EntityId id);
    [[nodiscard]] bool is_alive(EntityId id) const noexcept;
    [[nodiscard]] int32_t entity_count() const noexcept;
    void     flush_destroyed();

    void add_component(EntityId id, uint32_t type_id, const void* data,
                       int32_t size);
    [[nodiscard]] bool has_component(EntityId id, uint32_t type_id) const noexcept;
    bool get_component(EntityId id, uint32_t type_id, void* out_data,
                       int32_t size) const noexcept;
    void remove_component(EntityId id, uint32_t type_id);
    [[nodiscard]] int32_t component_count(uint32_t type_id) const noexcept;

    // K1 batching primitives.
    void add_components_bulk(const EntityId* entities, uint32_t type_id,
                             const void* component_data, int32_t component_size,
                             int32_t count);

    int32_t get_components_bulk(const EntityId* entities, uint32_t type_id,
                                void* out_data, int32_t component_size,
                                int32_t count) const noexcept;

    bool acquire_span(uint32_t type_id, const void** out_dense_ptr,
                      const int32_t** out_indices_ptr, int32_t* out_count) noexcept;

    void release_span(uint32_t type_id) noexcept;

    // K2 explicit type registration. Pre-creates an empty store for the
    // (type_id, component_size) pair. Idempotent for matching size; throws
    // std::invalid_argument on size mismatch or type_id == 0.
    bool register_component_type(uint32_t type_id, int32_t component_size);

    [[nodiscard]] int32_t active_spans_count() const noexcept {
        return active_spans_.load(std::memory_order_acquire);
    }

private:
    static constexpr std::size_t kInitialCapacity = 256;

    RawComponentStore* get_or_create_store(uint32_t type_id, int32_t size);
    const RawComponentStore* find_store(uint32_t type_id) const noexcept;

    std::vector<int32_t> versions_;
    int32_t next_index_ = 1; // Index 0 reserved for Invalid.
    int32_t live_count_ = 0;
    std::vector<int32_t> free_slots_;
    std::vector<EntityId> pending_destroy_;
    std::unordered_map<uint32_t, std::unique_ptr<RawComponentStore>> stores_;
    std::atomic<int32_t> active_spans_{0};
};

} // namespace dualfrontier
