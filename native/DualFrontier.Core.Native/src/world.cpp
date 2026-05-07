#include "world.h"

#include <cstring>
#include <stdexcept>

namespace dualfrontier {

World::World() {
    versions_.assign(kInitialCapacity, 0);
}

EntityId World::create_entity() {
    if (!free_slots_.empty()) {
        const int32_t recycled = free_slots_.back();
        free_slots_.pop_back();
        ++live_count_;
        return EntityId{recycled, versions_[recycled]};
    }

    if (static_cast<std::size_t>(next_index_) >= versions_.size()) {
        versions_.resize(versions_.size() * 2, 0);
    }

    const int32_t index = next_index_++;
    ++live_count_;
    return EntityId{index, versions_[index]};
}

bool World::is_alive(EntityId id) const noexcept {
    if (id.index <= 0) return false;
    if (static_cast<std::size_t>(id.index) >= versions_.size()) return false;
    return id.version == versions_[id.index];
}

int32_t World::entity_count() const noexcept {
    return live_count_;
}

void World::destroy_entity(EntityId id) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!is_alive(id)) return;
    ++versions_[id.index];
    --live_count_;
    pending_destroy_.push_back(id);
}

void World::flush_destroyed() {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    for (const EntityId id : pending_destroy_) {
        for (auto& [type_id, store] : stores_) {
            (void)type_id;
            store->remove(id.index);
        }
        free_slots_.push_back(id.index);
    }
    pending_destroy_.clear();
}

void World::add_component(EntityId id, uint32_t type_id, const void* data,
                          int32_t size) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!is_alive(id)) return;
    RawComponentStore* store = get_or_create_store(type_id, size);
    store->add(id.index, data, size);
}

bool World::has_component(EntityId id, uint32_t type_id) const noexcept {
    if (!is_alive(id)) return false;
    const RawComponentStore* store = find_store(type_id);
    if (!store) return false;
    return store->has(id.index);
}

bool World::get_component(EntityId id, uint32_t type_id, void* out_data,
                          int32_t size) const noexcept {
    if (!is_alive(id)) return false;
    const RawComponentStore* store = find_store(type_id);
    if (!store) return false;
    return store->get(id.index, out_data, size);
}

void World::remove_component(EntityId id, uint32_t type_id) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!is_alive(id)) return;
    auto it = stores_.find(type_id);
    if (it == stores_.end()) return;
    it->second->remove(id.index);
}

int32_t World::component_count(uint32_t type_id) const noexcept {
    const RawComponentStore* store = find_store(type_id);
    return store ? store->count() : 0;
}

void World::add_components_bulk(const EntityId* entities, uint32_t type_id,
                                const void* component_data,
                                int32_t component_size, int32_t count) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!entities || !component_data || component_size <= 0 || count <= 0) {
        return;
    }

    RawComponentStore* store = get_or_create_store(type_id, component_size);
    const uint8_t* data_bytes = static_cast<const uint8_t*>(component_data);

    for (int32_t i = 0; i < count; ++i) {
        if (!is_alive(entities[i])) continue;  // skip dead entities silently
        store->add(entities[i].index,
                   data_bytes + static_cast<std::size_t>(i) * component_size,
                   component_size);
    }
}

int32_t World::get_components_bulk(const EntityId* entities, uint32_t type_id,
                                   void* out_data, int32_t component_size,
                                   int32_t count) const noexcept {
    if (!entities || !out_data || component_size <= 0 || count <= 0) return 0;

    const RawComponentStore* store = find_store(type_id);
    uint8_t* out_bytes = static_cast<uint8_t*>(out_data);

    if (!store) {
        // Deterministic output: zero-fill all slots, return 0.
        std::memset(out_bytes, 0,
                    static_cast<std::size_t>(count) * component_size);
        return 0;
    }

    int32_t successful = 0;
    for (int32_t i = 0; i < count; ++i) {
        uint8_t* slot = out_bytes +
                        static_cast<std::size_t>(i) * component_size;
        if (!is_alive(entities[i])) {
            std::memset(slot, 0, component_size);
            continue;
        }
        if (store->get(entities[i].index, slot, component_size)) {
            ++successful;
        } else {
            std::memset(slot, 0, component_size);
        }
    }
    return successful;
}

bool World::acquire_span(uint32_t type_id, const void** out_dense_ptr,
                         const int32_t** out_indices_ptr,
                         int32_t* out_count) noexcept {
    if (!out_dense_ptr || !out_indices_ptr || !out_count) return false;

    const RawComponentStore* store = find_store(type_id);
    if (!store || store->count() == 0) {
        *out_dense_ptr = nullptr;
        *out_indices_ptr = nullptr;
        *out_count = 0;
        // Increment counter even for empty span (release must match).
        active_spans_.fetch_add(1, std::memory_order_acquire);
        return true;
    }

    *out_dense_ptr = store->dense_data();
    *out_indices_ptr = store->dense_indices().data();
    *out_count = store->count();

    active_spans_.fetch_add(1, std::memory_order_acquire);
    return true;
}

void World::release_span(uint32_t type_id) noexcept {
    (void)type_id;  // counter is global, not per-type. Per-type tracking
                    // added in K5 if mutation granularity proves necessary.
    int32_t prev = active_spans_.fetch_sub(1, std::memory_order_release);
    // Underflow guard: clamp to 0 if released more than acquired.
    if (prev <= 0) {
        active_spans_.store(0, std::memory_order_release);
    }
}

RawComponentStore* World::get_or_create_store(uint32_t type_id, int32_t size) {
    auto it = stores_.find(type_id);
    if (it != stores_.end()) {
        return it->second.get();
    }
    auto [inserted_it, _] = stores_.emplace(
        type_id, std::make_unique<RawComponentStore>(size));
    return inserted_it->second.get();
}

const RawComponentStore* World::find_store(uint32_t type_id) const noexcept {
    auto it = stores_.find(type_id);
    return it == stores_.end() ? nullptr : it->second.get();
}

} // namespace dualfrontier
