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
internal static class NativeMethods
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
}
