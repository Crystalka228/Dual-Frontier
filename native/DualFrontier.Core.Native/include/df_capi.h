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

/*
 * K8.1 reference primitives (added 2026-05-09).
 *
 * Four native-side primitives expose reference-style operations that
 * future K8.2 component redesigns will consume:
 *   * String pool — generational, mod-scoped interning.
 *   * Keyed map  — type-erased, sorted-by-key dictionary.
 *   * Composite  — per-entity variable-length data (swap-with-last erase).
 *   * Set        — type-erased, sorted-by-element set.
 *
 * Lifetime ownership: World owns the primitives. Caller never frees the
 * returned df_keyed_map_handle / df_composite_handle / df_set_handle —
 * they're owned by the World and outlive the function call only as long
 * as the World does. Same convention as the existing K0-K7 raw store
 * pointers.
 *
 * Mod scope:
 *   begin/end_mod_scope wrap a window during which intern() calls record
 *   the current mod as a co-owner of the resulting id. clear_mod_scope
 *   drops the window's IDs that aren't co-owned by another mod and bumps
 *   their generation tag so any stale {id, gen} reference resolves to
 *   "not found" instead of silently aliasing a reused slot.
 *
 * String pool save/load semantics (LOCKED): callers serialise CONTENT,
 * not IDs. On reload they re-intern; a fresh {id, generation} pair is
 * issued. The generation tag is the safety net for any path that did
 * persist an id (e.g., in-memory snapshots taken across a mod reload).
 *
 * Iteration buffers: out_keys / out_values / out_elements buffers must be
 * sized for buffer_capacity * (key_size | value_size | element_size)
 * bytes. The function returns the count actually written, clipped to
 * buffer_capacity.
 *
 * Return codes follow existing conventions:
 *   0 — failure / not found / not inserted (already present)
 *   1 — success / present / newly inserted
 * Out-of-range or null inputs return 0.
 */

/* String pool — 8 functions */

DF_API uint32_t        df_world_intern_string(
                           df_world_handle world,
                           const char* utf8_data,
                           int32_t utf8_length);

DF_API int32_t         df_world_resolve_string(
                           df_world_handle world,
                           uint32_t string_id,
                           uint32_t generation,
                           char* out_buffer,
                           int32_t out_buffer_size);

DF_API uint32_t        df_world_string_generation(
                           df_world_handle world,
                           uint32_t string_id);

DF_API void            df_world_begin_mod_scope(
                           df_world_handle world,
                           const char* mod_id);

DF_API void            df_world_end_mod_scope(
                           df_world_handle world,
                           const char* mod_id);

DF_API void            df_world_clear_mod_scope(
                           df_world_handle world,
                           const char* mod_id);

DF_API int32_t         df_world_string_pool_count(df_world_handle world);

DF_API uint32_t        df_world_string_pool_current_generation(df_world_handle world);

/* Keyed map — 7 functions */

typedef void* df_keyed_map_handle;

DF_API df_keyed_map_handle df_world_get_keyed_map(
                           df_world_handle world,
                           uint32_t map_id,
                           int32_t key_size,
                           int32_t value_size);

DF_API int32_t         df_keyed_map_set(
                           df_keyed_map_handle map,
                           const void* key,
                           const void* value);

DF_API int32_t         df_keyed_map_get(
                           df_keyed_map_handle map,
                           const void* key,
                           void* out_value);

DF_API int32_t         df_keyed_map_remove(
                           df_keyed_map_handle map,
                           const void* key);

DF_API int32_t         df_keyed_map_count(df_keyed_map_handle map);

DF_API int32_t         df_keyed_map_iterate(
                           df_keyed_map_handle map,
                           void* out_keys_buffer,
                           void* out_values_buffer,
                           int32_t buffer_capacity);

DF_API int32_t         df_keyed_map_clear(df_keyed_map_handle map);

/* Composite — 7 functions */

typedef void* df_composite_handle;

DF_API df_composite_handle df_world_get_composite(
                           df_world_handle world,
                           uint32_t composite_id,
                           int32_t element_size);

DF_API int32_t         df_composite_add(
                           df_composite_handle composite,
                           uint64_t parent_entity,
                           const void* element);

DF_API int32_t         df_composite_get_count(
                           df_composite_handle composite,
                           uint64_t parent_entity);

DF_API int32_t         df_composite_get_at(
                           df_composite_handle composite,
                           uint64_t parent_entity,
                           int32_t index,
                           void* out_element);

DF_API int32_t         df_composite_remove_at(
                           df_composite_handle composite,
                           uint64_t parent_entity,
                           int32_t index);

DF_API int32_t         df_composite_clear_for(
                           df_composite_handle composite,
                           uint64_t parent_entity);

DF_API int32_t         df_composite_iterate(
                           df_composite_handle composite,
                           uint64_t parent_entity,
                           void* out_elements_buffer,
                           int32_t buffer_capacity);

/* Set — 6 functions */

typedef void* df_set_handle;

DF_API df_set_handle   df_world_get_set(
                           df_world_handle world,
                           uint32_t set_id,
                           int32_t element_size);

DF_API int32_t         df_set_add(
                           df_set_handle set,
                           const void* element);

DF_API int32_t         df_set_contains(
                           df_set_handle set,
                           const void* element);

DF_API int32_t         df_set_remove(
                           df_set_handle set,
                           const void* element);

DF_API int32_t         df_set_count(df_set_handle set);

DF_API int32_t         df_set_iterate(
                           df_set_handle set,
                           void* out_elements_buffer,
                           int32_t buffer_capacity);

/*
 * K9 field storage.
 *
 * Field storage is a parallel abstraction alongside component stores. Each
 * field is a typed dense 2D grid keyed by string id. The id must be
 * mod-namespaced (caller's responsibility — kernel does not enforce); the
 * loader-side capability cross-check (MOD_OS_ARCHITECTURE.md §3.4) gates
 * which mods can register and access which fields.
 *
 * Storage layout per field:
 *   - Primary buffer: width * height * cell_size bytes
 *   - Back buffer: identical layout (ping-pong target for compute kernels)
 *   - Conductivity map: width * height floats (default 1.0)
 *   - Storage flags: width * height bits, byte-packed (default 0)
 *
 * Lifecycle:
 *   - df_world_register_field — idempotent on identical dimensions; rejects
 *     mismatched re-registration.
 *   - df_world_field_unregister — removes the field; subsequent access
 *     to that id returns 0.
 *
 * Mutation rejection contract (parallels active_spans on component stores):
 *   While any span on the field is acquired, write_cell, set_conductivity,
 *   set_storage_flag, and swap_buffers all return 0 / no-op.
 *
 * Span lifetime contract:
 *   1. Caller calls df_world_field_acquire_span -> dense data ptr + dimensions.
 *   2. Caller iterates without further P/Invokes.
 *   3. Caller MUST call df_world_field_release_span before any mutation.
 *   4. Multiple concurrent spans on different field ids OR same field allowed.
 *   5. Mutation attempt while any span active returns 0 / no-op.
 *
 * Returns 1 on success, 0 on failure (out-of-bounds, size mismatch, field
 * not found, mutation during active span).
 */

DF_API int32_t df_world_register_field(
    df_world_handle world,
    const char* field_id,
    int32_t width,
    int32_t height,
    int32_t cell_size);

DF_API int32_t df_world_field_unregister(
    df_world_handle world,
    const char* field_id);

DF_API int32_t df_world_field_read_cell(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    void* out_value,
    int32_t size);

DF_API int32_t df_world_field_write_cell(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    const void* value,
    int32_t size);

DF_API int32_t df_world_field_acquire_span(
    df_world_handle world,
    const char* field_id,
    const void** out_data,
    int32_t* out_width,
    int32_t* out_height);

DF_API int32_t df_world_field_release_span(
    df_world_handle world,
    const char* field_id);

DF_API int32_t df_world_field_set_conductivity(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    float value);

DF_API float df_world_field_get_conductivity(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y);

DF_API int32_t df_world_field_set_storage_flag(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    int32_t enabled);

DF_API int32_t df_world_field_get_storage_flag(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y);

DF_API int32_t df_world_field_swap_buffers(
    df_world_handle world,
    const char* field_id);

DF_API int32_t df_world_field_count(
    df_world_handle world);

#ifdef __cplusplus
}
#endif

#endif /* DF_CAPI_H */
