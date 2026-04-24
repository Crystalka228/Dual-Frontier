#include "df_capi.h"

#include "entity_id.h"
#include "world.h"

using dualfrontier::EntityId;
using dualfrontier::pack_entity;
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
    as_world(world)->destroy_entity(unpack_entity(entity));
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
    as_world(world)->flush_destroyed();
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
    as_world(world)->remove_component(unpack_entity(entity), type_id);
}

DF_API int32_t df_world_component_count(df_world_handle world,
                                        uint32_t type_id) {
    if (!world) return 0;
    return as_world(world)->component_count(type_id);
}

} // extern "C"
