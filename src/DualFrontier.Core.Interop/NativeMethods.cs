using System;
using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Flat P/Invoke surface for <c>DualFrontier.Core.Native</c>.
/// Mirrors <c>native/DualFrontier.Core.Native/include/df_capi.h</c> 1:1.
///
/// The native library must be discoverable by the OS loader at runtime.
/// On Windows that means <c>DualFrontier.Core.Native.dll</c> next to the
/// managed assembly; on Linux <c>DualFrontier.Core.Native.so</c>; on macOS
/// <c>DualFrontier.Core.Native.dylib</c>. See
/// <c>native/DualFrontier.Core.Native/build.md</c> for the build output
/// locations and the post-build copy recipe.
/// </summary>
internal static partial class NativeMethods
{
    /// <summary>
    /// Name without extension. The CLR adds the platform-specific prefix
    /// and extension per <c>DllImportSearchPath</c> rules.
    /// </summary>
    internal const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_world_create();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_world_destroy(IntPtr world);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong df_world_create_entity(IntPtr world);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_world_destroy_entity(IntPtr world, ulong entity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_is_alive(IntPtr world, ulong entity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_entity_count(IntPtr world);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_world_flush_destroyed(IntPtr world);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_world_add_component(
        IntPtr world,
        ulong entity,
        uint typeId,
        void* data,
        int size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_has_component(
        IntPtr world,
        ulong entity,
        uint typeId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_get_component(
        IntPtr world,
        ulong entity,
        uint typeId,
        void* outData,
        int size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_world_remove_component(
        IntPtr world,
        ulong entity,
        uint typeId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_component_count(IntPtr world, uint typeId);

    // K1 batching primitives.

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_world_add_components_bulk(
        IntPtr world,
        ulong* entities,
        uint typeId,
        void* componentData,
        int componentSize,
        int count);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_get_components_bulk(
        IntPtr world,
        ulong* entities,
        uint typeId,
        void* outData,
        int componentSize,
        int count);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_acquire_span(
        IntPtr world,
        uint typeId,
        void** outDensePtr,
        int** outIndicesPtr,
        int* outCount);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_world_release_span(IntPtr world, uint typeId);

    // K2 explicit type registration.

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_register_component_type(
        IntPtr world,
        uint typeId,
        int componentSize);

    // K3 engine bootstrap.

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_engine_bootstrap();

    // K5 Command Buffer write batching.

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_world_begin_batch(
        IntPtr world,
        uint typeId,
        int componentSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_batch_record_update(
        IntPtr batch,
        ulong entity,
        void* data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_batch_record_add(
        IntPtr batch,
        ulong entity,
        void* data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_batch_record_remove(
        IntPtr batch,
        ulong entity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_batch_flush(IntPtr batch);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_batch_cancel(IntPtr batch);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_batch_destroy(IntPtr batch);

    // K8.1 reference primitives.

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe uint df_world_intern_string(
        IntPtr world,
        byte* utf8Data,
        int utf8Length);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_resolve_string(
        IntPtr world,
        uint stringId,
        uint generation,
        byte* outBuffer,
        int outBufferSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint df_world_string_generation(IntPtr world, uint stringId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_world_begin_mod_scope(IntPtr world, byte* modId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_world_end_mod_scope(IntPtr world, byte* modId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_world_clear_mod_scope(IntPtr world, byte* modId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_string_pool_count(IntPtr world);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint df_world_string_pool_current_generation(IntPtr world);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_world_get_keyed_map(
        IntPtr world,
        uint mapId,
        int keySize,
        int valueSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_keyed_map_set(IntPtr map, void* key, void* value);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_keyed_map_get(IntPtr map, void* key, void* outValue);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_keyed_map_remove(IntPtr map, void* key);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_keyed_map_count(IntPtr map);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_keyed_map_iterate(
        IntPtr map,
        void* outKeysBuffer,
        void* outValuesBuffer,
        int bufferCapacity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_keyed_map_clear(IntPtr map);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_world_get_composite(
        IntPtr world,
        uint compositeId,
        int elementSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_composite_add(
        IntPtr composite,
        ulong parentEntity,
        void* element);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_composite_get_count(IntPtr composite, ulong parentEntity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_composite_get_at(
        IntPtr composite,
        ulong parentEntity,
        int index,
        void* outElement);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_composite_remove_at(
        IntPtr composite,
        ulong parentEntity,
        int index);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_composite_clear_for(IntPtr composite, ulong parentEntity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_composite_iterate(
        IntPtr composite,
        ulong parentEntity,
        void* outElementsBuffer,
        int bufferCapacity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_world_get_set(IntPtr world, uint setId, int elementSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_set_add(IntPtr set, void* element);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_set_contains(IntPtr set, void* element);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_set_remove(IntPtr set, void* element);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_set_count(IntPtr set);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_set_iterate(
        IntPtr set,
        void* outElementsBuffer,
        int bufferCapacity);
}
