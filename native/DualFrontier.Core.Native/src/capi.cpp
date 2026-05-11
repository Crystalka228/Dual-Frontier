#include "df_capi.h"

#include <cstring>
#include <memory>
#include <string>
#include <thread>
#include <vector>

#include "bootstrap_graph.h"
#include "composite.h"
#include "entity_id.h"
#include "keyed_map.h"
#include "set_primitive.h"
#include "string_pool.h"
#include "thread_pool.h"
#include "world.h"

using dualfrontier::BootstrapGraph;
using dualfrontier::Composite;
using dualfrontier::EntityId;
using dualfrontier::KeyedMap;
using dualfrontier::pack_entity;
using dualfrontier::SetPrimitive;
using dualfrontier::StringPool;
using dualfrontier::ThreadPool;
using dualfrontier::unpack_entity;
using dualfrontier::World;
using dualfrontier::WriteBatch;

namespace {

World* as_world(df_world_handle handle) noexcept {
    return static_cast<World*>(handle);
}

} // namespace

extern "C" {

DF_API df_world_handle df_world_create(void) {
    try {
        return new World();
    } catch (...) {
        return nullptr;
    }
}

DF_API void df_world_destroy(df_world_handle world) {
    delete as_world(world);
}

DF_API uint64_t df_world_create_entity(df_world_handle world) {
    if (!world) return 0;
    try {
        return pack_entity(as_world(world)->create_entity());
    } catch (...) {
        return 0;
    }
}

DF_API void df_world_destroy_entity(df_world_handle world, uint64_t entity) {
    if (!world) return;
    try {
        as_world(world)->destroy_entity(unpack_entity(entity));
    } catch (...) {
        // K1: destroy_entity now throws if spans are active. Swallow to keep
        // the C ABI noexcept-equivalent — the caller saw the request rejected.
    }
}

DF_API int32_t df_world_is_alive(df_world_handle world, uint64_t entity) {
    if (!world) return 0;
    return as_world(world)->is_alive(unpack_entity(entity)) ? 1 : 0;
}

DF_API int32_t df_world_entity_count(df_world_handle world) {
    if (!world) return 0;
    return as_world(world)->entity_count();
}

DF_API void df_world_flush_destroyed(df_world_handle world) {
    if (!world) return;
    try {
        as_world(world)->flush_destroyed();
    } catch (...) {
        // K1: flush_destroyed throws if spans are active. Swallow.
    }
}

DF_API void df_world_add_component(df_world_handle world,
                                   uint64_t entity,
                                   uint32_t type_id,
                                   const void* data,
                                   int32_t size) {
    if (!world || !data || size <= 0) return;
    try {
        as_world(world)->add_component(unpack_entity(entity), type_id, data,
                                       size);
    } catch (...) {
        // Swallow — the C# side treats Add as void. Diagnostics live in docs.
    }
}

DF_API int32_t df_world_has_component(df_world_handle world,
                                      uint64_t entity,
                                      uint32_t type_id) {
    if (!world) return 0;
    return as_world(world)->has_component(unpack_entity(entity), type_id) ? 1
                                                                          : 0;
}

DF_API int32_t df_world_get_component(df_world_handle world,
                                      uint64_t entity,
                                      uint32_t type_id,
                                      void* out_data,
                                      int32_t size) {
    if (!world || !out_data || size <= 0) return 0;
    return as_world(world)->get_component(unpack_entity(entity), type_id,
                                          out_data, size)
               ? 1
               : 0;
}

DF_API void df_world_remove_component(df_world_handle world,
                                      uint64_t entity,
                                      uint32_t type_id) {
    if (!world) return;
    try {
        as_world(world)->remove_component(unpack_entity(entity), type_id);
    } catch (...) {
        // K1: remove_component throws if spans are active. Swallow.
    }
}

DF_API int32_t df_world_component_count(df_world_handle world,
                                        uint32_t type_id) {
    if (!world) return 0;
    return as_world(world)->component_count(type_id);
}

DF_API void df_world_add_components_bulk(df_world_handle world,
                                         const uint64_t* entities,
                                         uint32_t type_id,
                                         const void* component_data,
                                         int32_t component_size,
                                         int32_t count) {
    if (!world || !entities || !component_data || count <= 0) return;
    try {
        // Unpack ulong array to EntityId array. For typical batch sizes
        // (10-1000) heap allocation is acceptable; the K5 WriteCommandBuffer
        // will eliminate this for hot-path tick loops.
        std::vector<EntityId> ids(static_cast<std::size_t>(count));
        for (int32_t i = 0; i < count; ++i) {
            ids[i] = unpack_entity(entities[i]);
        }
        as_world(world)->add_components_bulk(ids.data(), type_id,
                                             component_data, component_size,
                                             count);
    } catch (...) {
        // Swallow — bulk add is void on the C# side.
    }
}

DF_API int32_t df_world_get_components_bulk(df_world_handle world,
                                            const uint64_t* entities,
                                            uint32_t type_id,
                                            void* out_data,
                                            int32_t component_size,
                                            int32_t count) {
    if (!world || !entities || !out_data || count <= 0) return 0;
    try {
        std::vector<EntityId> ids(static_cast<std::size_t>(count));
        for (int32_t i = 0; i < count; ++i) {
            ids[i] = unpack_entity(entities[i]);
        }
        return as_world(world)->get_components_bulk(ids.data(), type_id,
                                                    out_data, component_size,
                                                    count);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_acquire_span(df_world_handle world,
                                     uint32_t type_id,
                                     const void** out_dense_ptr,
                                     const int32_t** out_indices_ptr,
                                     int32_t* out_count) {
    if (!world) return 0;
    try {
        return as_world(world)->acquire_span(type_id, out_dense_ptr,
                                             out_indices_ptr, out_count)
                   ? 1
                   : 0;
    } catch (...) {
        return 0;
    }
}

DF_API void df_world_release_span(df_world_handle world, uint32_t type_id) {
    if (!world) return;
    as_world(world)->release_span(type_id);
}

DF_API int32_t df_world_register_component_type(df_world_handle world,
                                                uint32_t type_id,
                                                int32_t component_size) {
    if (!world) return 0;
    try {
        return as_world(world)->register_component_type(type_id, component_size)
                   ? 1
                   : 0;
    } catch (...) {
        // K2: register_component_type throws on type_id 0, non-positive size,
        // or size mismatch with existing registration. Swallow per ABI convention.
        return 0;
    }
}

DF_API df_world_handle df_engine_bootstrap(void) {
    // RAII slots — automatic cleanup on early return.
    std::unique_ptr<World> world;
    std::unique_ptr<ThreadPool> persistent_pool;

    try {
        BootstrapGraph graph;

        World** world_slot = reinterpret_cast<World**>(&world);
        ThreadPool** pool_slot =
            reinterpret_cast<ThreadPool**>(&persistent_pool);
        (void)world_slot;
        (void)pool_slot;

        // Capture pointers to the unique_ptrs so each task can populate
        // and roll back its own slot. Pointers are stable for the duration
        // of df_engine_bootstrap.
        auto* world_ptr = &world;
        auto* pool_ptr = &persistent_pool;

        // Task 1: AllocateMemoryPools (no deps).
        // Reserved for explicit pool allocation if future K7 measurements
        // show value (e.g. preallocate dense_bytes buffers per type).
        // Kept as a graph node now so the diamond shape (parallelism between
        // InitWorldStructure and InitThreadPool) is exercised.
        graph.add_task(
            "AllocateMemoryPools",
            {},
            []() {},
            []() {});

        // Task 2: InitWorldStructure (deps: AllocateMemoryPools).
        graph.add_task(
            "InitWorldStructure",
            {"AllocateMemoryPools"},
            [world_ptr]() {
                *world_ptr = std::make_unique<World>();
            },
            [world_ptr]() {
                world_ptr->reset();
            });

        // Task 3: InitThreadPool (deps: AllocateMemoryPools, parallel with
        // InitWorldStructure).
        graph.add_task(
            "InitThreadPool",
            {"AllocateMemoryPools"},
            [pool_ptr]() {
                std::size_t n = std::thread::hardware_concurrency();
                if (n == 0) n = 4;
                *pool_ptr = std::make_unique<ThreadPool>(n);
            },
            [pool_ptr]() {
                if (*pool_ptr) {
                    (*pool_ptr)->shutdown();
                }
                pool_ptr->reset();
            });

        // Task 4: SignalEngineReady (deps: InitWorldStructure, InitThreadPool).
        graph.add_task(
            "SignalEngineReady",
            {"InitWorldStructure", "InitThreadPool"},
            [world_ptr]() {
                if (!*world_ptr) {
                    throw std::logic_error(
                        "SignalEngineReady: World not initialized");
                }
                (*world_ptr)->mark_bootstrapped();
            },
            []() {});

        // Bootstrap pool is separate from the engine pool — it orchestrates
        // graph execution including the construction of the engine pool.
        // Kept small (cap 4) since bootstrap is short-lived.
        std::size_t bootstrap_threads = std::thread::hardware_concurrency();
        if (bootstrap_threads == 0) bootstrap_threads = 4;
        if (bootstrap_threads > 4) bootstrap_threads = 4;
        ThreadPool bootstrap_pool(bootstrap_threads);

        bool success = graph.run(bootstrap_pool);
        bootstrap_pool.shutdown();
        // bootstrap_pool destructor joins its workers.

        if (!success) {
            // Rollback already executed inside graph.run() — both unique_ptrs
            // are nullptr. Returning here drops to the catch-all below the
            // try block, but that's not the path used; we just return.
            return nullptr;
        }

        // Per Q1 (Minimal scope): the persistent pool is released after
        // bootstrap. K-L6 keeps game tick managed; native side has no
        // post-bootstrap consumer for this pool. Future native artifacts
        // that need a pool create their own (D3 Lvl 1 pattern).
        if (persistent_pool) {
            persistent_pool->shutdown();
            persistent_pool.reset();
        }

        return world.release();
    } catch (...) {
        // RAII via unique_ptr destructors cleans up partial state.
        return nullptr;
    }
}

// =============================================================================
// K5 Command Buffer C ABI (added 2026-05-08).
// =============================================================================

DF_API df_batch_handle df_world_begin_batch(df_world_handle world,
                                            uint32_t type_id,
                                            int32_t component_size) {
    if (!world) return nullptr;
    try {
        return new WriteBatch(as_world(world), type_id, component_size);
    } catch (...) {
        // K5: ctor throws on null world (already guarded), type_id 0, or
        // non-positive component_size. Boundary returns null sentinel.
        return nullptr;
    }
}

DF_API int32_t df_batch_record_update(df_batch_handle batch,
                                      uint64_t entity,
                                      const void* data) {
    if (!batch) return 0;
    try {
        return static_cast<WriteBatch*>(batch)->record_update(entity, data);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_batch_record_add(df_batch_handle batch,
                                   uint64_t entity,
                                   const void* data) {
    if (!batch) return 0;
    try {
        return static_cast<WriteBatch*>(batch)->record_add(entity, data);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_batch_record_remove(df_batch_handle batch,
                                      uint64_t entity) {
    if (!batch) return 0;
    try {
        return static_cast<WriteBatch*>(batch)->record_remove(entity);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_batch_flush(df_batch_handle batch) {
    if (!batch) return -1;
    try {
        return static_cast<WriteBatch*>(batch)->flush();
    } catch (const std::logic_error&) {
        // Double-flush or post-cancel flush.
        return -1;
    } catch (...) {
        return -1;
    }
}

DF_API void df_batch_cancel(df_batch_handle batch) {
    if (!batch) return;
    try {
        static_cast<WriteBatch*>(batch)->cancel();
    } catch (...) {
        // Suppress.
    }
}

DF_API void df_batch_destroy(df_batch_handle batch) {
    if (!batch) return;
    try {
        delete static_cast<WriteBatch*>(batch);
    } catch (...) {
        // Suppress — destructor must not throw.
    }
}

// ---- K8.1 reference primitives --------------------------------------------

DF_API uint32_t df_world_intern_string(df_world_handle world,
                                        const char* utf8_data,
                                        int32_t utf8_length) {
    if (!world || !utf8_data || utf8_length < 0) return 0;
    try {
        std::string content(utf8_data, static_cast<size_t>(utf8_length));
        return as_world(world)->string_pool().intern(content);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_resolve_string(df_world_handle world,
                                        uint32_t string_id,
                                        uint32_t generation,
                                        char* out_buffer,
                                        int32_t out_buffer_size) {
    if (!world || !out_buffer || out_buffer_size <= 0) return 0;
    try {
        const std::string* content =
            as_world(world)->string_pool().resolve(string_id, generation);
        if (!content) {
            return 0;
        }
        const int32_t to_copy =
            (static_cast<int32_t>(content->size()) < out_buffer_size)
                ? static_cast<int32_t>(content->size())
                : out_buffer_size;
        std::memcpy(out_buffer, content->data(), static_cast<size_t>(to_copy));
        return to_copy;
    } catch (...) {
        return 0;
    }
}

DF_API uint32_t df_world_string_generation(df_world_handle world,
                                            uint32_t string_id) {
    if (!world) return 0;
    try {
        return as_world(world)->string_pool().generation_for(string_id);
    } catch (...) {
        return 0;
    }
}

DF_API void df_world_begin_mod_scope(df_world_handle world,
                                      const char* mod_id) {
    if (!world || !mod_id) return;
    try {
        as_world(world)->begin_mod_scope(std::string(mod_id));
    } catch (...) {
        // Swallow — same convention as other void C ABI entries.
    }
}

DF_API void df_world_end_mod_scope(df_world_handle world,
                                    const char* mod_id) {
    if (!world || !mod_id) return;
    try {
        as_world(world)->end_mod_scope(std::string(mod_id));
    } catch (...) {
        // Swallow — mismatched scope is a programmer error; absorbed at the
        // boundary (same pattern as begin/end pairing on other ABI entries).
    }
}

DF_API void df_world_clear_mod_scope(df_world_handle world,
                                      const char* mod_id) {
    if (!world || !mod_id) return;
    try {
        as_world(world)->clear_mod_scope(std::string(mod_id));
    } catch (...) {
    }
}

DF_API int32_t df_world_string_pool_count(df_world_handle world) {
    if (!world) return 0;
    try {
        return as_world(world)->string_pool().count();
    } catch (...) {
        return 0;
    }
}

DF_API uint32_t df_world_string_pool_current_generation(df_world_handle world) {
    if (!world) return 0;
    try {
        return as_world(world)->string_pool().current_generation();
    } catch (...) {
        return 0;
    }
}

DF_API df_keyed_map_handle df_world_get_keyed_map(df_world_handle world,
                                                   uint32_t map_id,
                                                   int32_t key_size,
                                                   int32_t value_size) {
    if (!world || map_id == 0 || key_size <= 0 || value_size <= 0) {
        return nullptr;
    }
    try {
        return as_world(world)->get_or_create_keyed_map(map_id, key_size, value_size);
    } catch (...) {
        return nullptr;
    }
}

DF_API int32_t df_keyed_map_set(df_keyed_map_handle map,
                                 const void* key,
                                 const void* value) {
    if (!map || !key || !value) return 0;
    try {
        return static_cast<KeyedMap*>(map)->set(key, value);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_keyed_map_get(df_keyed_map_handle map,
                                 const void* key,
                                 void* out_value) {
    if (!map || !key || !out_value) return 0;
    try {
        return static_cast<const KeyedMap*>(map)->get(key, out_value);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_keyed_map_remove(df_keyed_map_handle map, const void* key) {
    if (!map || !key) return 0;
    try {
        return static_cast<KeyedMap*>(map)->remove(key);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_keyed_map_count(df_keyed_map_handle map) {
    if (!map) return 0;
    try {
        return static_cast<const KeyedMap*>(map)->count();
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_keyed_map_iterate(df_keyed_map_handle map,
                                     void* out_keys_buffer,
                                     void* out_values_buffer,
                                     int32_t buffer_capacity) {
    if (!map || !out_keys_buffer || !out_values_buffer || buffer_capacity < 0) {
        return 0;
    }
    try {
        return static_cast<const KeyedMap*>(map)->iterate(
            out_keys_buffer, out_values_buffer, buffer_capacity);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_keyed_map_clear(df_keyed_map_handle map) {
    if (!map) return 0;
    try {
        return static_cast<KeyedMap*>(map)->clear();
    } catch (...) {
        return 0;
    }
}

DF_API df_composite_handle df_world_get_composite(df_world_handle world,
                                                    uint32_t composite_id,
                                                    int32_t element_size) {
    if (!world || composite_id == 0 || element_size <= 0) {
        return nullptr;
    }
    try {
        return as_world(world)->get_or_create_composite(composite_id, element_size);
    } catch (...) {
        return nullptr;
    }
}

DF_API int32_t df_composite_add(df_composite_handle composite,
                                 uint64_t parent_entity,
                                 const void* element) {
    if (!composite || !element) return 0;
    try {
        return static_cast<Composite*>(composite)->add(parent_entity, element);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_composite_get_count(df_composite_handle composite,
                                       uint64_t parent_entity) {
    if (!composite) return 0;
    try {
        return static_cast<const Composite*>(composite)->get_count(parent_entity);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_composite_get_at(df_composite_handle composite,
                                    uint64_t parent_entity,
                                    int32_t index,
                                    void* out_element) {
    if (!composite || !out_element || index < 0) return 0;
    try {
        return static_cast<const Composite*>(composite)
            ->get_at(parent_entity, index, out_element);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_composite_remove_at(df_composite_handle composite,
                                       uint64_t parent_entity,
                                       int32_t index) {
    if (!composite || index < 0) return 0;
    try {
        return static_cast<Composite*>(composite)->remove_at(parent_entity, index);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_composite_clear_for(df_composite_handle composite,
                                       uint64_t parent_entity) {
    if (!composite) return 0;
    try {
        return static_cast<Composite*>(composite)->clear_for(parent_entity);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_composite_iterate(df_composite_handle composite,
                                     uint64_t parent_entity,
                                     void* out_elements_buffer,
                                     int32_t buffer_capacity) {
    if (!composite || !out_elements_buffer || buffer_capacity < 0) {
        return 0;
    }
    try {
        return static_cast<const Composite*>(composite)
            ->iterate(parent_entity, out_elements_buffer, buffer_capacity);
    } catch (...) {
        return 0;
    }
}

DF_API df_set_handle df_world_get_set(df_world_handle world,
                                        uint32_t set_id,
                                        int32_t element_size) {
    if (!world || set_id == 0 || element_size <= 0) {
        return nullptr;
    }
    try {
        return as_world(world)->get_or_create_set(set_id, element_size);
    } catch (...) {
        return nullptr;
    }
}

DF_API int32_t df_set_add(df_set_handle set, const void* element) {
    if (!set || !element) return 0;
    try {
        return static_cast<SetPrimitive*>(set)->add(element);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_set_contains(df_set_handle set, const void* element) {
    if (!set || !element) return 0;
    try {
        return static_cast<const SetPrimitive*>(set)->contains(element);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_set_remove(df_set_handle set, const void* element) {
    if (!set || !element) return 0;
    try {
        return static_cast<SetPrimitive*>(set)->remove(element);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_set_count(df_set_handle set) {
    if (!set) return 0;
    try {
        return static_cast<const SetPrimitive*>(set)->count();
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_set_iterate(df_set_handle set,
                               void* out_elements_buffer,
                               int32_t buffer_capacity) {
    if (!set || !out_elements_buffer || buffer_capacity < 0) return 0;
    try {
        return static_cast<const SetPrimitive*>(set)
            ->iterate(out_elements_buffer, buffer_capacity);
    } catch (...) {
        return 0;
    }
}

// K9 field storage C ABI implementations.

DF_API int32_t df_world_register_field(
    df_world_handle world, const char* field_id,
    int32_t width, int32_t height, int32_t cell_size)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        return w->register_field(field_id, width, height, cell_size);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_unregister(
    df_world_handle world, const char* field_id)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        return w->unregister_field(field_id);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_read_cell(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, void* out_value, int32_t size)
{
    if (world == nullptr || field_id == nullptr || out_value == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->read_cell(x, y, out_value, size);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_write_cell(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, const void* value, int32_t size)
{
    if (world == nullptr || field_id == nullptr || value == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->write_cell(x, y, value, size);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_acquire_span(
    df_world_handle world, const char* field_id,
    const void** out_data, int32_t* out_width, int32_t* out_height)
{
    if (world == nullptr || field_id == nullptr) return 0;
    if (out_data == nullptr || out_width == nullptr || out_height == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->acquire_span(out_data, out_width, out_height);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_release_span(
    df_world_handle world, const char* field_id)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        field->release_span();
        return 1;
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_set_conductivity(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, float value)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->set_conductivity(x, y, value);
    } catch (...) {
        return 0;
    }
}

DF_API float df_world_field_get_conductivity(
    df_world_handle world, const char* field_id, int32_t x, int32_t y)
{
    if (world == nullptr || field_id == nullptr) return 0.0f;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0.0f;
        return field->get_conductivity(x, y);
    } catch (...) {
        return 0.0f;
    }
}

DF_API int32_t df_world_field_set_storage_flag(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, int32_t enabled)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->set_storage_flag(x, y, enabled);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_get_storage_flag(
    df_world_handle world, const char* field_id, int32_t x, int32_t y)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->get_storage_flag(x, y);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_swap_buffers(
    df_world_handle world, const char* field_id)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        field->swap_buffers();
        return 1;
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_field_count(df_world_handle world)
{
    if (world == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        return w->field_count();
    } catch (...) {
        return 0;
    }
}

} // extern "C"
