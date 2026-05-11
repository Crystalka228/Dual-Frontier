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
    if (active_spans_.load(std::memory_order_acquire) > 0 ||
        active_batches_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans or batches are active");
    }
    if (!is_alive(id)) return;
    ++versions_[id.index];
    --live_count_;
    pending_destroy_.push_back(id);
}

void World::flush_destroyed() {
    if (active_spans_.load(std::memory_order_acquire) > 0 ||
        active_batches_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans or batches are active");
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
    if (active_spans_.load(std::memory_order_acquire) > 0 ||
        active_batches_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans or batches are active");
    }
    if (!is_alive(id)) return;
    RawComponentStore* store = get_or_create_store(type_id, size);
    store->add(id.index, data, size);
}

void World::add_component_unchecked(EntityId id, uint32_t type_id,
                                     const void* data, int32_t size) {
    // K5 — internal path used by WriteBatch::flush(). The active_batches_
    // count is intentionally NOT consulted (the calling batch is itself one).
    // The active_spans_ contract still applies — flushing while a span is
    // outstanding violates the read-only-snapshot invariant.
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
    if (active_spans_.load(std::memory_order_acquire) > 0 ||
        active_batches_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans or batches are active");
    }
    if (!is_alive(id)) return;
    auto it = stores_.find(type_id);
    if (it == stores_.end()) return;
    it->second->remove(id.index);
}

void World::remove_component_unchecked(EntityId id, uint32_t type_id) {
    // K5 — internal path used by WriteBatch::flush(). See add_component_unchecked.
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
    if (active_spans_.load(std::memory_order_acquire) > 0 ||
        active_batches_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans or batches are active");
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

bool World::register_component_type(uint32_t type_id, int32_t component_size) {
    if (type_id == 0) {
        throw std::invalid_argument("type_id 0 is reserved");
    }
    if (component_size <= 0) {
        throw std::invalid_argument("component_size must be > 0");
    }
    auto it = stores_.find(type_id);
    if (it != stores_.end()) {
        if (it->second->component_size() != component_size) {
            throw std::invalid_argument(
                "type_id already registered with different component_size");
        }
        return true;  // idempotent
    }
    stores_.emplace(type_id, std::make_unique<RawComponentStore>(component_size));
    return true;
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

void World::mark_bootstrapped() {
    bool expected = false;
    if (!bootstrapped_.compare_exchange_strong(
            expected, true, std::memory_order_acq_rel)) {
        throw std::logic_error("World: double bootstrap detected");
    }
}

// =============================================================================
// K5 WriteBatch implementation — Command Buffer pattern.
// =============================================================================

WriteBatch::WriteBatch(World* world, uint32_t type_id, int32_t component_size)
    : world_(world)
    , type_id_(type_id)
    , component_size_(component_size)
    , cancelled_(false)
    , flushed_(false) {
    if (!world_) {
        throw std::invalid_argument("world is null");
    }
    if (type_id_ == 0) {
        throw std::invalid_argument("type_id 0 is reserved");
    }
    if (component_size_ <= 0) {
        throw std::invalid_argument("component_size must be positive");
    }
    world_->increment_active_batches();
}

WriteBatch::~WriteBatch() {
    if (!flushed_ && !cancelled_) {
        // Auto-flush — caller treated Dispose as "I'm done, apply" rather
        // than "abort". Mirrors managed-side `using var batch` semantics.
        try {
            // Inline flush body — calling flush() here would re-check
            // flushed_/cancelled_ which is fine, but we want the destructor
            // to remain noexcept-equivalent.
            flushed_ = true;
            std::size_t data_offset = 0;
            for (const WriteCommand& cmd : commands_) {
                EntityId id{static_cast<int32_t>(cmd.entity_index),
                            static_cast<int32_t>(cmd.entity_version)};
                if (!world_->is_alive(id)) {
                    if (cmd.kind == CommandKind::Update ||
                        cmd.kind == CommandKind::Add) {
                        data_offset += static_cast<std::size_t>(component_size_);
                    }
                    continue;
                }
                switch (cmd.kind) {
                    case CommandKind::Update:
                        if (world_->has_component(id, type_id_)) {
                            world_->add_component_unchecked(
                                id, type_id_,
                                command_data_.data() + data_offset,
                                component_size_);
                        }
                        data_offset += static_cast<std::size_t>(component_size_);
                        break;
                    case CommandKind::Add:
                        world_->add_component_unchecked(
                            id, type_id_,
                            command_data_.data() + data_offset,
                            component_size_);
                        data_offset += static_cast<std::size_t>(component_size_);
                        break;
                    case CommandKind::Remove:
                        if (world_->has_component(id, type_id_)) {
                            world_->remove_component_unchecked(id, type_id_);
                        }
                        break;
                }
            }
        } catch (...) {
            // Suppress — destructor must not throw.
        }
    }
    world_->decrement_active_batches();
}

int32_t WriteBatch::record_update(uint64_t entity, const void* data) {
    if (cancelled_ || flushed_) return 0;
    if (!data) return 0;

    EntityId id = unpack_entity(entity);
    try {
        commands_.push_back(WriteCommand{
            CommandKind::Update,
            static_cast<uint32_t>(id.index),
            static_cast<uint32_t>(id.version)});
        const std::size_t old_size = command_data_.size();
        command_data_.resize(old_size +
                             static_cast<std::size_t>(component_size_));
        std::memcpy(command_data_.data() + old_size, data,
                    static_cast<std::size_t>(component_size_));
        return 1;
    } catch (const std::bad_alloc&) {
        return 0;
    }
}

int32_t WriteBatch::record_add(uint64_t entity, const void* data) {
    if (cancelled_ || flushed_) return 0;
    if (!data) return 0;

    EntityId id = unpack_entity(entity);
    try {
        commands_.push_back(WriteCommand{
            CommandKind::Add,
            static_cast<uint32_t>(id.index),
            static_cast<uint32_t>(id.version)});
        const std::size_t old_size = command_data_.size();
        command_data_.resize(old_size +
                             static_cast<std::size_t>(component_size_));
        std::memcpy(command_data_.data() + old_size, data,
                    static_cast<std::size_t>(component_size_));
        return 1;
    } catch (const std::bad_alloc&) {
        return 0;
    }
}

int32_t WriteBatch::record_remove(uint64_t entity) {
    if (cancelled_ || flushed_) return 0;

    EntityId id = unpack_entity(entity);
    try {
        commands_.push_back(WriteCommand{
            CommandKind::Remove,
            static_cast<uint32_t>(id.index),
            static_cast<uint32_t>(id.version)});
        return 1;
    } catch (const std::bad_alloc&) {
        return 0;
    }
}

int32_t WriteBatch::flush() {
    if (cancelled_) {
        throw std::logic_error("Cannot flush cancelled batch");
    }
    if (flushed_) {
        throw std::logic_error("Batch already flushed");
    }

    flushed_ = true;

    int32_t successful = 0;
    std::size_t data_offset = 0;

    for (const WriteCommand& cmd : commands_) {
        EntityId id{static_cast<int32_t>(cmd.entity_index),
                    static_cast<int32_t>(cmd.entity_version)};

        // Liveness check uses stored version, so an entity destroyed
        // between record и flush is detected here.
        if (!world_->is_alive(id)) {
            if (cmd.kind == CommandKind::Update ||
                cmd.kind == CommandKind::Add) {
                data_offset += static_cast<std::size_t>(component_size_);
            }
            continue;
        }

        switch (cmd.kind) {
            case CommandKind::Update: {
                // Update is "set if exists". Skip if component absent.
                if (world_->has_component(id, type_id_)) {
                    world_->add_component_unchecked(
                        id, type_id_,
                        command_data_.data() + data_offset,
                        component_size_);
                    ++successful;
                }
                data_offset += static_cast<std::size_t>(component_size_);
                break;
            }
            case CommandKind::Add: {
                // Add is "set unconditionally" — overwrite if present.
                world_->add_component_unchecked(
                    id, type_id_,
                    command_data_.data() + data_offset,
                    component_size_);
                ++successful;
                data_offset += static_cast<std::size_t>(component_size_);
                break;
            }
            case CommandKind::Remove: {
                if (world_->has_component(id, type_id_)) {
                    world_->remove_component_unchecked(id, type_id_);
                    ++successful;
                }
                break;
            }
        }
    }

    return successful;
}

void WriteBatch::cancel() {
    if (flushed_) return;  // Already flushed — cancel is no-op.
    cancelled_ = true;
    commands_.clear();
    command_data_.clear();
}

// ---- K8.1 reference primitives ---------------------------------------------

KeyedMap* World::get_or_create_keyed_map(uint32_t map_id,
                                          int32_t key_size,
                                          int32_t value_size) {
    if (map_id == 0) {
        return nullptr;
    }
    auto it = keyed_maps_.find(map_id);
    if (it != keyed_maps_.end()) {
        if (it->second->key_size() != key_size ||
            it->second->value_size() != value_size) {
            throw std::invalid_argument(
                "World::get_or_create_keyed_map: size mismatch on existing id");
        }
        return it->second.get();
    }
    auto inserted = keyed_maps_.emplace(
        map_id, std::make_unique<KeyedMap>(key_size, value_size));
    return inserted.first->second.get();
}

Composite* World::get_or_create_composite(uint32_t composite_id,
                                            int32_t element_size) {
    if (composite_id == 0) {
        return nullptr;
    }
    auto it = composites_.find(composite_id);
    if (it != composites_.end()) {
        if (it->second->element_size() != element_size) {
            throw std::invalid_argument(
                "World::get_or_create_composite: size mismatch on existing id");
        }
        return it->second.get();
    }
    auto inserted = composites_.emplace(
        composite_id, std::make_unique<Composite>(element_size));
    return inserted.first->second.get();
}

SetPrimitive* World::get_or_create_set(uint32_t set_id,
                                        int32_t element_size) {
    if (set_id == 0) {
        return nullptr;
    }
    auto it = sets_.find(set_id);
    if (it != sets_.end()) {
        if (it->second->element_size() != element_size) {
            throw std::invalid_argument(
                "World::get_or_create_set: size mismatch on existing id");
        }
        return it->second.get();
    }
    auto inserted = sets_.emplace(
        set_id, std::make_unique<SetPrimitive>(element_size));
    return inserted.first->second.get();
}

void World::begin_mod_scope(const std::string& mod_id) {
    string_pool_.begin_mod_scope(mod_id);
}

void World::end_mod_scope(const std::string& mod_id) {
    string_pool_.end_mod_scope(mod_id);
}

void World::clear_mod_scope(const std::string& mod_id) {
    string_pool_.clear_mod_scope(mod_id);
}

int32_t World::register_field(const std::string& field_id, int32_t width, int32_t height, int32_t cell_size)
{
    if (field_id.empty()) {
        throw std::invalid_argument("World::register_field: field_id must be non-empty");
    }
    if (width <= 0 || height <= 0 || cell_size <= 0) {
        throw std::invalid_argument("World::register_field: dimensions and cell_size must be positive");
    }

    auto it = fields_.find(field_id);
    if (it != fields_.end()) {
        // Idempotent: same dimensions = no-op success; mismatch = throw.
        const RawTileField& existing = *it->second;
        if (existing.width() == width && existing.height() == height && existing.cell_size() == cell_size) {
            return 1;
        }
        throw std::invalid_argument("World::register_field: id already registered with different dimensions");
    }

    fields_.emplace(field_id, std::make_unique<RawTileField>(width, height, cell_size));
    return 1;
}

RawTileField* World::get_field(const std::string& field_id) noexcept
{
    auto it = fields_.find(field_id);
    return (it != fields_.end()) ? it->second.get() : nullptr;
}

const RawTileField* World::get_field(const std::string& field_id) const noexcept
{
    auto it = fields_.find(field_id);
    return (it != fields_.end()) ? it->second.get() : nullptr;
}

int32_t World::unregister_field(const std::string& field_id)
{
    auto it = fields_.find(field_id);
    if (it == fields_.end()) return 0;
    fields_.erase(it);
    return 1;
}

} // namespace dualfrontier
