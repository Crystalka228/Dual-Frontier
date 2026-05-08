#include "df_capi.h"

#include <memory>
#include <thread>
#include <vector>

#include "bootstrap_graph.h"
#include "entity_id.h"
#include "thread_pool.h"
#include "world.h"

using dualfrontier::BootstrapGraph;
using dualfrontier::EntityId;
using dualfrontier::pack_entity;
using dualfrontier::ThreadPool;
using dualfrontier::unpack_entity;
using dualfrontier::World;

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

} // extern "C"
