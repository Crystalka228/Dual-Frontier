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

/*
 * K2 explicit type registration (added 2026-05-07).
 *
 * Replaces implicit FNV-1a hash-based type identification (K0 inheritance)
 * with explicit deterministic registry. Caller assigns sequential type_ids
 * (1, 2, 3, ...) and declares the byte size for each type.
 *
 * Native side:
 * - Records (type_id, size) mapping at registration time.
 * - Re-registration with same (type_id, size) is idempotent (no-op).
 * - Re-registration with same type_id but DIFFERENT size throws invalid_argument
 *   (caught at boundary, returns 0).
 * - type_id == 0 is reserved for "invalid" sentinel; registration with 0 fails.
 *
 * Migration note: existing functions (df_world_add_component, df_world_get_component,
 * df_world_acquire_span, etc.) accept type_ids whether or not they were
 * pre-registered. Pre-registration enables the registry-based path; legacy
 * FNV-1a-derived ids continue to work for backward compat with K0/K1 callers.
 *
 * Returns 1 on success, 0 on failure (invalid type_id, size mismatch, or
 * world disposed).
 */
DF_API int32_t         df_world_register_component_type(
                           df_world_handle world,
                           uint32_t type_id,
                           int32_t component_size);

/*
 * K3 engine bootstrap (added 2026-05-07).
 *
 * Single entry point that performs all native-side initialization:
 *   1. Allocate memory pools (placeholder — currently no-op, reserved).
 *   2. Construct the World instance (parallel with thread pool init).
 *   3. Initialize an internal thread pool (parallel with World init).
 *   4. Mark engine as ready.
 *
 * On success: returns an opaque World handle. Caller must call
 * df_world_destroy when done.
 *
 * On failure: returns nullptr. All partial state is rolled back via the
 * graph's per-task cleanup functions before returning. Managed-side caller
 * should treat IntPtr.Zero as a hard failure.
 *
 * Atomic: либо fully bootstrapped либо nullptr. No partial visibility.
 *
 * Calling df_engine_bootstrap on a process that already has a bootstrapped
 * engine creates a SECOND independent engine — handles are isolated. Multiple
 * concurrent engines are supported (each has its own World; the bootstrap
 * thread pool is an ephemeral construct, not retained).
 */
DF_API df_world_handle df_engine_bootstrap(void);

/*
 * K5 Command Buffer write batching (added 2026-05-08).
 *
 * Replaces direct in-place mutation rejected by Q2 architectural decision.
 * Managed code records mutations as commands; native side validates and
 * applies atomically at flush time.
 *
 * Lifecycle:
 *   1. Caller calls df_world_begin_batch -> opaque batch handle.
 *   2. Caller records updates/adds/removes via df_batch_record_*.
 *      Each record is validated immediately (data not null), returns 1/0.
 *   3. Caller calls df_batch_flush -> all commands applied atomically.
 *      Returns count of successful commands (entities still alive at flush time).
 *   4. Caller calls df_batch_destroy -> releases batch handle.
 *
 * Auto-flush: df_batch_destroy on a non-flushed, non-cancelled batch
 * implicitly flushes (matches managed `using var batch` Dispose semantics).
 *
 * Cancellation: df_batch_cancel discards all recorded commands without
 * applying. Subsequent df_batch_destroy is no-op.
 *
 * Mutation rejection contract:
 *   While ANY active batch exists, direct mutations (df_world_add_component,
 *   df_world_remove_component, df_world_destroy_entity, df_world_flush_destroyed,
 *   df_world_add_components_bulk) are rejected. Multiple concurrent batches
 *   (same OR different type_id) are allowed. Spans and batches coexist
 *   (independent counters).
 *
 * Returns:
 *   df_world_begin_batch: batch handle on success, nullptr on failure.
 *   df_batch_record_*: 1 on success (recorded), 0 on failure (validation).
 *   df_batch_flush: count of successful commands (>=0), -1 on logic error.
 *   df_batch_destroy: void (failures absorbed).
 *   df_batch_cancel: void.
 */

typedef void* df_batch_handle;

DF_API df_batch_handle df_world_begin_batch(
                           df_world_handle world,
                           uint32_t type_id,
                           int32_t component_size);

DF_API int32_t         df_batch_record_update(
                           df_batch_handle batch,
                           uint64_t entity,
                           const void* data);

DF_API int32_t         df_batch_record_add(
                           df_batch_handle batch,
                           uint64_t entity,
                           const void* data);

DF_API int32_t         df_batch_record_remove(
                           df_batch_handle batch,
                           uint64_t entity);

DF_API int32_t         df_batch_flush(df_batch_handle batch);

DF_API void            df_batch_cancel(df_batch_handle batch);

DF_API void            df_batch_destroy(df_batch_handle batch);

#ifdef __cplusplus
}
#endif

#endif /* DF_CAPI_H */
