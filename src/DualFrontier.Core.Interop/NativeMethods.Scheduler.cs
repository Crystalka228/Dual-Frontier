using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.1 scheduler P/Invoke surface. Mirrors the <c>df_scheduler_*</c> block at
/// the end of <c>native/DualFrontier.Core.Native/include/df_capi.h</c> 1:1.
/// </summary>
internal static partial class NativeMethods
{
    // ----- K10.1 Item 1 — system graph -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_register_system(
        uint systemId,
        byte* systemFqn,
        uint* readComponentIds,
        uint readCount,
        uint* writeComponentIds,
        uint writeCount,
        int priorityClass,
        int wakeType);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_unregister_system(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_system_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_clear();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_compute_static_graph();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_static_phase_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_static_phase_size(int phaseIndex);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_static_phase_systems(
        int phaseIndex,
        uint* outSystemIds,
        int outCapacity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_compute_per_tick_graph(
        uint* runnableIds,
        uint runnableCount);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_per_tick_phase_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_per_tick_phase_size(int phaseIndex);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_per_tick_phase_systems(
        int phaseIndex,
        uint* outSystemIds,
        int outCapacity);
}
