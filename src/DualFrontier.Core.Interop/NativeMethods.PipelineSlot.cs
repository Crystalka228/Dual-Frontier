using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.3 v2 Item 33 pipeline slot P/Invoke surface. Mirrors the
/// <c>df_pipeline_*</c> block в <c>native/DualFrontier.Core.Native/include/pipeline_slot.h</c>
/// 1:1. К-L7.1 sub-invariant binding + К-L16 pipeline depth (D=2 default).
/// </summary>
internal static partial class NativeMethods
{
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_pipeline_init(int depth);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_pipeline_reset();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_get_depth(int* outDepth);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_allocate_slot(
        ulong simTick, PipelineSlotInterop.PipelineSlot** outSlot);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_get_slot(
        int slotOffset, PipelineSlotInterop.PipelineSlot** outSlot);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_set_fence(
        PipelineSlotInterop.PipelineSlot* slot, nint vkFenceHandle);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_check_fences(int* outSlotsTransitioned);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_force_fence_completed(
        PipelineSlotInterop.PipelineSlot* slot);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_transition_to_tail(
        PipelineSlotInterop.PipelineSlot* slot);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_pipeline_is_quiescent(int* outIsQuiescent);
}
