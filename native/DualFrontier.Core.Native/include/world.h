#pragma once

#include <atomic>
#include <cstdint>
#include <memory>
#include <string>
#include <unordered_map>
#include <vector>

#include "component_store.h"
#include "composite.h"
#include "entity_id.h"
#include "keyed_map.h"
#include "set_primitive.h"
#include "string_pool.h"
#include "tile_field.h"

namespace dualfrontier {

class WriteBatch;  // forward declaration for friend access

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

    // K5 Command Buffer — count of active write batches. Direct mutations
    // (add/remove/destroy/flush_destroyed) are rejected while > 0.
    [[nodiscard]] int32_t active_batches_count() const noexcept {
        return active_batches_.load(std::memory_order_acquire);
    }

    // K3 bootstrap state. is_bootstrapped() returns true after a successful
    // df_engine_bootstrap() has marked this World as ready. The flag is
    // not consulted by World's own methods; it exists so external bootstrap
    // logic can detect double-bootstrap attempts on the same handle.
    [[nodiscard]] bool is_bootstrapped() const noexcept {
        return bootstrapped_.load(std::memory_order_acquire);
    }

    // Marks engine as bootstrapped. Called by SignalEngineReady.
    // Throws std::logic_error if already bootstrapped.
    void mark_bootstrapped();

    // K8.1 — primitive accessors. Each get_or_create_* either returns an
    // existing primitive bound to the given (id, size) tuple or constructs
    // a fresh one. Returns nullptr if id == 0. Mismatched size on a
    // pre-existing id throws std::invalid_argument (same convention as
    // K2's register_component_type).
    StringPool& string_pool() noexcept { return string_pool_; }
    [[nodiscard]] const StringPool& string_pool() const noexcept { return string_pool_; }
    KeyedMap*     get_or_create_keyed_map(uint32_t map_id, int32_t key_size, int32_t value_size);
    Composite*    get_or_create_composite(uint32_t composite_id, int32_t element_size);
    SetPrimitive* get_or_create_set(uint32_t set_id, int32_t element_size);

    // K8.1 — mod scope orchestration (currently delegates to string_pool_;
    // future K8.x milestones may extend scope tracking to other primitives).
    void begin_mod_scope(const std::string& mod_id);
    void end_mod_scope(const std::string& mod_id);
    void clear_mod_scope(const std::string& mod_id);

    // K9 field registry. Field storage is orthogonal to component stores
    // (FIELDS.md): identity is (field_id, x, y) cell coordinate, not EntityId.
    // Each field is a type-erased dense 2D grid; managed bridge tracks
    // element type and copies cell_size bytes per cell on read/write.
    int32_t register_field(const std::string& field_id, int32_t width, int32_t height, int32_t cell_size);
    RawTileField* get_field(const std::string& field_id) noexcept;
    const RawTileField* get_field(const std::string& field_id) const noexcept;
    int32_t unregister_field(const std::string& field_id);
    [[nodiscard]] int32_t field_count() const noexcept {
        return static_cast<int32_t>(fields_.size());
    }

private:
    static constexpr std::size_t kInitialCapacity = 256;

    RawComponentStore* get_or_create_store(uint32_t type_id, int32_t size);
    const RawComponentStore* find_store(uint32_t type_id) const noexcept;

    // K5 — internal mutation paths called from WriteBatch::flush(). Skip the
    // active_batches_ check (the calling batch is itself one) but still
    // honour the active_spans_ contract.
    friend class WriteBatch;
    void increment_active_batches() noexcept {
        active_batches_.fetch_add(1, std::memory_order_acq_rel);
    }
    void decrement_active_batches() noexcept {
        active_batches_.fetch_sub(1, std::memory_order_acq_rel);
    }
    void add_component_unchecked(EntityId id, uint32_t type_id,
                                  const void* data, int32_t size);
    void remove_component_unchecked(EntityId id, uint32_t type_id);

    std::vector<int32_t> versions_;
    int32_t next_index_ = 1; // Index 0 reserved for Invalid.
    int32_t live_count_ = 0;
    std::vector<int32_t> free_slots_;
    std::vector<EntityId> pending_destroy_;
    std::unordered_map<uint32_t, std::unique_ptr<RawComponentStore>> stores_;
    std::atomic<int32_t> active_spans_{0};
    std::atomic<int32_t> active_batches_{0};
    std::atomic<bool> bootstrapped_{false};

    // K8.1 — primitives owned by this World. string_pool_ is value-typed
    // (single instance per World); the others are id-keyed maps so each
    // logical map/composite/set can be looked up by stable uint32_t id.
    StringPool string_pool_;
    std::unordered_map<uint32_t, std::unique_ptr<KeyedMap>> keyed_maps_;
    std::unordered_map<uint32_t, std::unique_ptr<Composite>> composites_;
    std::unordered_map<uint32_t, std::unique_ptr<SetPrimitive>> sets_;

    // K9 — field storage parallel to component stores.
    std::unordered_map<std::string, std::unique_ptr<RawTileField>> fields_;
};

// K5 Command Buffer pattern — write batching protocol.
//
// Managed code never directly mutates native memory. Mutations are recorded
// as commands, validated native-side, applied atomically at flush time.
//
// Lifecycle:
//   * ctor increments the owning World's active_batches_ counter.
//   * record_* methods append к command/data vectors (managed-side scratch
//     equivalent transmits one record per P/Invoke в K5; bulk transmit
//     deferred to K7+).
//   * flush() validates each command (entity liveness via stored version),
//     applies in record order, returns count of successful commands.
//   * cancel() discards commands without applying.
//   * dtor auto-flushes if not explicitly flushed/cancelled. Always
//     decrements active_batches_ counter.
//
// While ANY active batch exists, direct mutations (add/remove/destroy/flush_destroyed)
// throw std::logic_error (caught at C ABI). Multiple concurrent batches
// allowed; flush goes through internal *_unchecked paths so it does not
// deadlock against its own contribution или peer batches.

enum class CommandKind : uint8_t {
    Update = 1,
    Add = 2,
    Remove = 3,
};

struct WriteCommand {
    CommandKind kind;
    uint32_t entity_index;
    uint32_t entity_version;
};

class WriteBatch {
public:
    WriteBatch(World* world, uint32_t type_id, int32_t component_size);
    ~WriteBatch();

    WriteBatch(const WriteBatch&) = delete;
    WriteBatch& operator=(const WriteBatch&) = delete;
    WriteBatch(WriteBatch&&) = delete;
    WriteBatch& operator=(WriteBatch&&) = delete;

    int32_t record_update(uint64_t entity, const void* data);
    int32_t record_add(uint64_t entity, const void* data);
    int32_t record_remove(uint64_t entity);

    int32_t flush();
    void cancel();

    bool is_active() const noexcept { return !cancelled_ && !flushed_; }
    int32_t command_count() const noexcept {
        return static_cast<int32_t>(commands_.size());
    }

private:
    World* world_;
    uint32_t type_id_;
    int32_t component_size_;
    std::vector<WriteCommand> commands_;
    std::vector<uint8_t> command_data_;
    bool cancelled_;
    bool flushed_;
};

} // namespace dualfrontier
