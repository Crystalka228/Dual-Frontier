#ifndef DF_CAPI_H
#define DF_CAPI_H

#include <stdint.h>

#if defined(_WIN32)
    #if defined(DF_NATIVE_BUILDING_DLL)
        #define DF_API __declspec(dllexport)
    #else
        #define DF_API __declspec(dllimport)
    #endif
#else
    #define DF_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

/*
 * DualFrontier.Core.Native — flat C ABI for P/Invoke.
 *
 * Entity encoding: an EntityId is packed into uint64_t as
 *     high 32 bits = Version (signed int, reinterpreted as uint32_t)
 *     low  32 bits = Index   (signed int, reinterpreted as uint32_t)
 * This matches the C# EntityId(Index, Version) record struct layout when
 * marshalled as a single 64-bit value.
 *
 * Component storage: the native world is type-erased. The C# side chooses a
 * stable uint32_t type_id for each component type (e.g. a hash of the CLR
 * type's full name) and declares the component's byte size. The native side
 * copies `size` bytes on Add and on Get. For the PoC we assume blittable
 * structs; reference-type components remain on the C# side and are not
 * supported by this API (see docs/NATIVE_CORE.md).
 *
 * Return codes:
 *   0 — failure / not found
 *   1 — success / present
 * Out-of-range inputs return 0 rather than crashing.
 */

typedef void* df_world_handle;

DF_API df_world_handle df_world_create(void);
DF_API void            df_world_destroy(df_world_handle world);

DF_API uint64_t        df_world_create_entity(df_world_handle world);
DF_API void            df_world_destroy_entity(df_world_handle world, uint64_t entity);
DF_API int32_t         df_world_is_alive(df_world_handle world, uint64_t entity);
DF_API int32_t         df_world_entity_count(df_world_handle world);
DF_API void            df_world_flush_destroyed(df_world_handle world);

DF_API void            df_world_add_component(
                           df_world_handle world,
                           uint64_t entity,
                           uint32_t type_id,
                           const void* data,
                           int32_t size);

DF_API int32_t         df_world_has_component(
                           df_world_handle world,
                           uint64_t entity,
                           uint32_t type_id);

DF_API int32_t         df_world_get_component(
                           df_world_handle world,
                           uint64_t entity,
                           uint32_t type_id,
                           void* out_data,
                           int32_t size);

DF_API void            df_world_remove_component(
                           df_world_handle world,
                           uint64_t entity,
                           uint32_t type_id);

DF_API int32_t         df_world_component_count(
                           df_world_handle world,
                           uint32_t type_id);

/*
 * K1 batching primitives (added 2026-05-07).
 *
 * Bulk operations eliminate per-entity P/Invoke overhead by transmitting
 * arrays of entities + components in a single crossing.
 *
 * Span access provides direct read-only view into native dense storage.
 * The span pointer is valid until df_world_release_span is called OR
 * until any mutation (add/remove/destroy) is attempted (which will fail
 * while spans are active).
 *
 * Span lifetime contract:
 *   1. Caller calls df_world_acquire_span -> receives dense ptr + indices ptr + count.
 *   2. Caller iterates without further P/Invokes.
 *   3. Caller MUST call df_world_release_span before any mutation.
 *   4. Multiple concurrent spans (different type_ids OR same type_id) allowed.
 *   5. Mutation attempt while any span is active throws (caught at boundary,
 *      function returns 0 / no-op).
 */

DF_API void            df_world_add_components_bulk(
                           df_world_handle world,
                           const uint64_t* entities,
                           uint32_t type_id,
                           const void* component_data,
                           int32_t component_size,
                           int32_t count);

DF_API int32_t         df_world_get_components_bulk(
                           df_world_handle world,
                           const uint64_t* entities,
                           uint32_t type_id,
                           void* out_data,
                           int32_t component_size,
                           int32_t count);

DF_API int32_t         df_world_acquire_span(
                           df_world_handle world,
                           uint32_t type_id,
                           const void** out_dense_ptr,
                           const int32_t** out_indices_ptr,
                           int32_t* out_count);

DF_API void            df_world_release_span(
                           df_world_handle world,
                           uint32_t type_id);

#ifdef __cplusplus
}
#endif

#endif /* DF_CAPI_H */
