#include "world.h"

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
    if (!is_alive(id)) return;
    ++versions_[id.index];
    --live_count_;
    pending_destroy_.push_back(id);
}

void World::flush_destroyed() {
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
    if (!is_alive(id)) return;
    auto it = stores_.find(type_id);
    if (it == stores_.end()) return;
    it->second->remove(id.index);
}

int32_t World::component_count(uint32_t type_id) const noexcept {
    const RawComponentStore* store = find_store(type_id);
    return store ? store->count() : 0;
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
