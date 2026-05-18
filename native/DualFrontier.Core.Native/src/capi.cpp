#include "df_capi.h"

#include <cstring>
#include <memory>
#include <string>
#include <thread>
#include <vector>

#include <atomic>

#include "bootstrap_graph.h"
#include "composite.h"
#include "entity_id.h"
#include "keyed_map.h"
#include "set_primitive.h"
#include "managed_callback.h"
#include "scheduling_policies.h"
#include "shm_region.h"
#include "state_change_filter.h"
#include "string_pool.h"
#include "system_graph.h"
#include "thread_pool.h"
#include "wake_registry.h"
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

// =============================================================================
// K10.1 Item 1 — scheduler graph C ABI.
// =============================================================================

DF_API int32_t df_scheduler_register_system(
    uint32_t system_id,
    const char* system_fqn,
    const uint32_t* read_component_ids,
    uint32_t read_count,
    const uint32_t* write_component_ids,
    uint32_t write_count,
    int32_t priority_class,
    int32_t wake_type)
{
    if (system_fqn == nullptr) return 0;
    try {
        std::vector<uint32_t> reads;
        reads.reserve(read_count);
        if (read_component_ids != nullptr) {
            for (uint32_t i = 0; i < read_count; ++i) {
                reads.push_back(read_component_ids[i]);
            }
        }
        std::vector<uint32_t> writes;
        writes.reserve(write_count);
        if (write_component_ids != nullptr) {
            for (uint32_t i = 0; i < write_count; ++i) {
                writes.push_back(write_component_ids[i]);
            }
        }
        bool ok = dualfrontier::default_scheduler_graph().register_system(
            system_id,
            std::string(system_fqn),
            std::move(reads),
            std::move(writes),
            priority_class,
            wake_type);
        return ok ? 1 : 0;
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_scheduler_unregister_system(uint32_t system_id)
{
    try {
        return dualfrontier::default_scheduler_graph().unregister_system(system_id) ? 1 : 0;
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_scheduler_system_count(void)
{
    return static_cast<int32_t>(dualfrontier::default_scheduler_graph().system_count());
}

DF_API void df_scheduler_clear(void)
{
    try {
        dualfrontier::default_scheduler_graph().clear();
    } catch (...) {
        // swallow
    }
}

DF_API int32_t df_scheduler_compute_static_graph(void)
{
    try {
        return dualfrontier::default_scheduler_graph().compute_static_graph();
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_scheduler_static_phase_count(void)
{
    return dualfrontier::default_scheduler_graph().static_phase_count();
}

DF_API int32_t df_scheduler_static_phase_size(int32_t phase_index)
{
    return dualfrontier::default_scheduler_graph().static_phase_size(phase_index);
}

DF_API int32_t df_scheduler_static_phase_systems(
    int32_t phase_index,
    uint32_t* out_system_ids,
    int32_t out_capacity)
{
    return dualfrontier::default_scheduler_graph().static_phase_systems(
        phase_index, out_system_ids, out_capacity);
}

DF_API int32_t df_scheduler_compute_per_tick_graph(
    const uint32_t* runnable_ids,
    uint32_t runnable_count)
{
    try {
        return dualfrontier::default_scheduler_graph().compute_per_tick_graph(
            runnable_ids, runnable_count);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_scheduler_per_tick_phase_count(void)
{
    return dualfrontier::default_scheduler_graph().per_tick_phase_count();
}

DF_API int32_t df_scheduler_per_tick_phase_size(int32_t phase_index)
{
    return dualfrontier::default_scheduler_graph().per_tick_phase_size(phase_index);
}

DF_API int32_t df_scheduler_per_tick_phase_systems(
    int32_t phase_index,
    uint32_t* out_system_ids,
    int32_t out_capacity)
{
    return dualfrontier::default_scheduler_graph().per_tick_phase_systems(
        phase_index, out_system_ids, out_capacity);
}

// =============================================================================
// K10.1 Item 3 — wake registry C ABI.
// =============================================================================

DF_API int32_t df_wake_registry_subscribe_timer(uint32_t system_id, uint32_t ticks_per_update) {
    return dualfrontier::default_wake_registry().subscribe_timer(system_id, ticks_per_update) ? 1 : 0;
}

DF_API int32_t df_wake_registry_subscribe_event(uint32_t system_id, uint32_t event_type_id) {
    return dualfrontier::default_wake_registry().subscribe_event(system_id, event_type_id) ? 1 : 0;
}

DF_API int32_t df_wake_registry_subscribe_state(uint32_t system_id, uint32_t component_type_id) {
    return dualfrontier::default_wake_registry().subscribe_state(system_id, component_type_id) ? 1 : 0;
}

DF_API int32_t df_wake_registry_subscribe_init(uint32_t system_id) {
    return dualfrontier::default_wake_registry().subscribe_init(system_id) ? 1 : 0;
}

DF_API int32_t df_wake_registry_subscribe_explicit(uint32_t system_id, uint32_t wake_id) {
    return dualfrontier::default_wake_registry().subscribe_explicit(system_id, wake_id) ? 1 : 0;
}

DF_API int32_t df_wake_registry_unsubscribe(uint32_t system_id, int32_t wake_type) {
    using WT = dualfrontier::WakeRegistry::WakeType;
    if (wake_type < 0 || wake_type > 4) return 0;
    return dualfrontier::default_wake_registry().unsubscribe(system_id, static_cast<WT>(wake_type));
}

DF_API int32_t df_wake_registry_fire_timer(uint64_t current_tick) {
    return dualfrontier::default_wake_registry().fire_timer(current_tick);
}

DF_API int32_t df_wake_registry_fire_event(uint32_t event_type_id) {
    return dualfrontier::default_wake_registry().fire_event(event_type_id);
}

DF_API int32_t df_wake_registry_fire_state_change(uint32_t component_type_id, uint32_t entity_id) {
    return dualfrontier::default_wake_registry().fire_state_change(component_type_id, entity_id);
}

DF_API int32_t df_wake_registry_fire_init(void) {
    return dualfrontier::default_wake_registry().fire_init();
}

DF_API int32_t df_wake_registry_fire_explicit(uint32_t target_system_id, uint32_t wake_id) {
    return dualfrontier::default_wake_registry().fire_explicit(target_system_id, wake_id);
}

DF_API int32_t df_wake_registry_runqueue_size(void) {
    return dualfrontier::default_wake_registry().runqueue_size();
}

DF_API int32_t df_wake_registry_drain_runqueue(uint32_t* out_system_ids, int32_t out_capacity) {
    return dualfrontier::default_wake_registry().drain_runqueue(out_system_ids, out_capacity);
}

DF_API int32_t df_wake_registry_subscription_count(int32_t wake_type) {
    using WT = dualfrontier::WakeRegistry::WakeType;
    if (wake_type < 0 || wake_type > 4) return 0;
    return dualfrontier::default_wake_registry().subscription_count(static_cast<WT>(wake_type));
}

DF_API void df_wake_registry_clear(void) {
    dualfrontier::default_wake_registry().clear();
}

// =============================================================================
// K10.1 Item 4 — wake registry diagnostic API.
// =============================================================================

DF_API int32_t df_scheduler_query_runnable(uint32_t* out_system_ids, int32_t out_capacity) {
    return dualfrontier::default_wake_registry().peek_runqueue(out_system_ids, out_capacity);
}

DF_API int32_t df_scheduler_query_wake_subscriptions(uint32_t system_id) {
    return dualfrontier::default_wake_registry().wake_subscriptions_for(system_id);
}

// =============================================================================
// K10.1 Item 17 — write-through hook (state change filter).
// =============================================================================

DF_API int32_t df_state_filter_may_have_subscribers(uint32_t component_type_id) {
    return dualfrontier::default_state_change_filter()
        .may_have_subscribers(component_type_id) ? 1 : 0;
}

DF_API int32_t df_state_filter_has_entity_specific_subscriber(
    uint32_t component_type_id, uint32_t entity_id) {
    return dualfrontier::default_state_change_filter()
        .has_entity_specific_subscriber(component_type_id, entity_id) ? 1 : 0;
}

DF_API int32_t df_state_filter_subscribe_type(
    uint32_t component_type_id, uint32_t subscriber_system_id) {
    dualfrontier::default_state_change_filter()
        .subscribe_type(component_type_id, subscriber_system_id);
    return 1;
}

DF_API int32_t df_state_filter_subscribe_entity(
    uint32_t component_type_id, uint32_t entity_id, uint32_t subscriber_system_id) {
    dualfrontier::default_state_change_filter()
        .subscribe_entity(component_type_id, entity_id, subscriber_system_id);
    return 1;
}

DF_API int32_t df_state_filter_unsubscribe_type(
    uint32_t component_type_id, uint32_t subscriber_system_id) {
    dualfrontier::default_state_change_filter()
        .unsubscribe_type(component_type_id, subscriber_system_id);
    return 1;
}

DF_API int32_t df_state_filter_unsubscribe_entity(
    uint32_t component_type_id, uint32_t entity_id, uint32_t subscriber_system_id) {
    dualfrontier::default_state_change_filter()
        .unsubscribe_entity(component_type_id, entity_id, subscriber_system_id);
    return 1;
}

DF_API int32_t df_state_filter_type_wide_subscriber_count(uint32_t component_type_id) {
    return dualfrontier::default_state_change_filter()
        .type_wide_subscriber_count(component_type_id);
}

DF_API int32_t df_state_filter_entity_subscriber_count(uint32_t component_type_id) {
    return dualfrontier::default_state_change_filter()
        .entity_subscriber_count(component_type_id);
}

DF_API void df_state_filter_clear(void) {
    dualfrontier::default_state_change_filter().clear();
}

DF_API void df_native_world_commit_hook(uint32_t component_type_id, uint32_t entity_id) {
    try {
        dualfrontier::df_native_world_commit_hook_impl(component_type_id, entity_id);
    } catch (...) {
        // swallow — commit hook must not throw к caller
    }
}

// =============================================================================
// K10.1 Item 15 — batched callback ABI.
// =============================================================================

DF_API void df_scheduler_register_managed_callback(df_managed_batch_fn cb, void* user_data) {
    dualfrontier::default_managed_callback_registry().register_callback(cb, user_data);
}

DF_API int32_t df_scheduler_dispatch_managed_batch(const df_managed_system_batch* batch) {
    try {
        return dualfrontier::default_managed_callback_registry().dispatch_batch(batch);
    } catch (...) { return 0; }
}

DF_API int32_t df_scheduler_managed_callback_registered(void) {
    return dualfrontier::default_managed_callback_registry().has_callback();
}

DF_API void df_scheduler_clear_managed_callback(void) {
    dualfrontier::default_managed_callback_registry().clear();
}

// =============================================================================
// K10.1 Item 9 — shared memory regions C ABI.
// =============================================================================

DF_API int32_t df_shm_create(uint32_t region_id, int32_t size_bytes) {
    try {
        return dualfrontier::default_shm_registry().create(region_id, size_bytes) ? 1 : 0;
    } catch (...) { return 0; }
}

DF_API void* df_shm_map(uint32_t region_id) {
    try {
        return dualfrontier::default_shm_registry().map(region_id);
    } catch (...) { return nullptr; }
}

DF_API int32_t df_shm_size(uint32_t region_id) {
    return dualfrontier::default_shm_registry().size(region_id);
}

DF_API int32_t df_shm_unmap(uint32_t region_id) {
    return dualfrontier::default_shm_registry().unmap(region_id) ? 1 : 0;
}

DF_API int32_t df_shm_destroy(uint32_t region_id) {
    return dualfrontier::default_shm_registry().destroy(region_id) ? 1 : 0;
}

DF_API int32_t df_shm_register_writer(uint32_t region_id, uint32_t writer_system_id) {
    return dualfrontier::default_shm_registry().register_writer(region_id, writer_system_id) ? 1 : 0;
}

DF_API int32_t df_shm_writer(uint32_t region_id) {
    return static_cast<int32_t>(dualfrontier::default_shm_registry().writer(region_id));
}

DF_API int32_t df_shm_region_count(void) {
    return dualfrontier::default_shm_registry().region_count();
}

DF_API void df_shm_clear(void) {
    dualfrontier::default_shm_registry().clear();
}

// =============================================================================
// K10.1 Items 6+7+8 — scheduling policies C ABI.
// =============================================================================

DF_API int32_t df_scheduler_policies_set(
    uint32_t system_id,
    int32_t scheduling_class,
    int32_t max_latency_micros,
    int32_t max_jitter_micros,
    int32_t cpu_quota_micros_per_tick,
    int32_t preemption_mode)
{
    using namespace dualfrontier;
    if (scheduling_class < 0 || scheduling_class > 4) return 0;
    if (preemption_mode < 0 || preemption_mode > 1) return 0;
    SchedulingPolicies::Policy p;
    p.scheduling_class = static_cast<SchedulingClass>(scheduling_class);
    p.max_latency_micros = max_latency_micros;
    p.max_jitter_micros = max_jitter_micros;
    p.cpu_quota_micros_per_tick = cpu_quota_micros_per_tick;
    p.preemption_mode = static_cast<PreemptionMode>(preemption_mode);
    return default_scheduling_policies().set_policy(system_id, p) ? 1 : 0;
}

DF_API int32_t df_scheduler_policies_get_class(uint32_t system_id) {
    return static_cast<int32_t>(
        dualfrontier::default_scheduling_policies().get_policy(system_id).scheduling_class);
}

DF_API int32_t df_scheduler_policies_get_quota(uint32_t system_id) {
    return dualfrontier::default_scheduling_policies()
        .get_policy(system_id).cpu_quota_micros_per_tick;
}

DF_API int32_t df_scheduler_policies_record_execution(uint32_t system_id, int64_t micros) {
    return dualfrontier::default_scheduling_policies().record_execution(system_id, micros);
}

DF_API int32_t df_scheduler_policies_quota_exceeded(uint32_t system_id) {
    return dualfrontier::default_scheduling_policies().quota_exceeded(system_id);
}

DF_API int64_t df_scheduler_policies_total_micros(uint32_t system_id) {
    return dualfrontier::default_scheduling_policies().total_micros(system_id);
}

DF_API int32_t df_scheduler_policies_quota_violations(uint32_t system_id) {
    return dualfrontier::default_scheduling_policies().quota_violations(system_id);
}

DF_API void df_scheduler_policies_reset_tick_stats(void) {
    dualfrontier::default_scheduling_policies().reset_tick_stats();
}

DF_API int32_t df_scheduler_policies_order_by_priority(
    const uint32_t* in_ids, uint32_t in_count,
    uint32_t* out_ids, int32_t out_capacity)
{
    return dualfrontier::default_scheduling_policies().order_by_priority(
        in_ids, in_count, out_ids, out_capacity);
}

DF_API void df_scheduler_policies_clear(void) {
    dualfrontier::default_scheduling_policies().clear();
}

// K10.1 Item 11 — CPU affinity.
DF_API int32_t df_scheduler_policies_set_affinity(uint32_t system_id, int32_t affinity_core_id) {
    using namespace dualfrontier;
    auto& policies = default_scheduling_policies();
    auto p = policies.get_policy(system_id);
    p.cpu_affinity_core_id = affinity_core_id;
    return policies.set_policy(system_id, p) ? 1 : 0;
}

DF_API int32_t df_scheduler_policies_get_affinity(uint32_t system_id) {
    return dualfrontier::default_scheduling_policies().get_policy(system_id).cpu_affinity_core_id;
}

// K10.1 Item 12 — work stealing toggle. Single global thread pool for
// scheduler use is the К10.1 model; per-pool toggles arise in К11+. К10.1
// stores the policy on the system_graph scope (process-global) via a static
// flag external к the thread pool instance — this is the metadata layer the
// future pool reads. For К10.1 we maintain it as a module-local atomic.
namespace {
std::atomic<bool> g_work_stealing_enabled{true};
}

DF_API int32_t df_scheduler_work_stealing_enabled(void) {
    return g_work_stealing_enabled.load(std::memory_order_acquire) ? 1 : 0;
}

DF_API void df_scheduler_set_work_stealing_enabled(int32_t enabled) {
    g_work_stealing_enabled.store(enabled != 0, std::memory_order_release);
}

// K10.1 Item 13 — phase barrier semantics.
DF_API int32_t df_scheduler_set_phase_barrier(int32_t phase_index, int32_t barrier_type) {
    return dualfrontier::default_scheduler_graph().set_phase_barrier(phase_index, barrier_type);
}

DF_API int32_t df_scheduler_get_phase_barrier(int32_t phase_index) {
    return dualfrontier::default_scheduler_graph().get_phase_barrier(phase_index);
}

// =============================================================================
// K10.1 Item 5 — per-tick scheduler orchestration.
// =============================================================================

DF_API int32_t df_scheduler_tick_begin(uint64_t current_tick) {
    try {
        auto& registry = dualfrontier::default_wake_registry();
        auto& graph = dualfrontier::default_scheduler_graph();

        registry.fire_timer(current_tick);

        // Drain runqueue into a local buffer.
        int32_t runqueue_size = registry.runqueue_size();
        if (runqueue_size <= 0) {
            return graph.compute_per_tick_graph(nullptr, 0);
        }
        std::vector<uint32_t> runnable(static_cast<std::size_t>(runqueue_size));
        int32_t drained = registry.drain_runqueue(runnable.data(), runqueue_size);
        return graph.compute_per_tick_graph(runnable.data(),
                                            static_cast<uint32_t>(drained));
    } catch (...) {
        return 0;
    }
}

} // extern "C"
