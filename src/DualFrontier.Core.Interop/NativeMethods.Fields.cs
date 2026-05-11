using System;
using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K9 field storage P/Invoke surface. Mirrors the field-related block at
/// the end of <c>native/DualFrontier.Core.Native/include/df_capi.h</c> 1:1.
/// </summary>
/// <remarks>
/// Field ids are passed as null-terminated UTF-8 byte sequences via
/// <c>byte*</c>, matching the existing <c>df_world_begin_mod_scope</c> /
/// <c>df_world_intern_string</c> pattern. Encoding is the caller's
/// responsibility — see <see cref="FieldRegistry"/> for the stackalloc
/// UTF-8 helper used at every entry point.
/// </remarks>
internal static partial class NativeMethods
{
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_register_field(
        IntPtr world,
        byte* fieldId,
        int width, int height, int cellSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_unregister(
        IntPtr world,
        byte* fieldId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_read_cell(
        IntPtr world,
        byte* fieldId,
        int x, int y, void* outValue, int size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_write_cell(
        IntPtr world,
        byte* fieldId,
        int x, int y, void* value, int size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_acquire_span(
        IntPtr world,
        byte* fieldId,
        void** outData, int* outWidth, int* outHeight);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_release_span(
        IntPtr world,
        byte* fieldId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_set_conductivity(
        IntPtr world,
        byte* fieldId,
        int x, int y, float value);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe float df_world_field_get_conductivity(
        IntPtr world,
        byte* fieldId,
        int x, int y);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_set_storage_flag(
        IntPtr world,
        byte* fieldId,
        int x, int y, int enabled);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_get_storage_flag(
        IntPtr world,
        byte* fieldId,
        int x, int y);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_swap_buffers(
        IntPtr world,
        byte* fieldId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_field_count(IntPtr world);
}
