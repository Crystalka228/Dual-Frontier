using System;
using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// К-L15 native bus P/Invoke surface (<c>df_bus_*</c>), a partial of
/// <see cref="NativeMethods"/>. Relocated here from
/// <c>DualFrontier.Application.Bus.ManagedBusBridge</c> at A'.9.1 Phase β C9
/// (DFK002 triage): the bus native lives in <c>DualFrontier.Core.Native.dll</c>,
/// so its bindings belong on the canonical §8-sanctioned Core.Interop kernel
/// boundary — not scattered in the Application layer. <c>ManagedBusBridge</c>
/// keeps its public bridge API and reaches these via the existing
/// <c>InternalsVisibleTo(DualFrontier.Application)</c> grant, the same pattern
/// the scheduler adapter already uses for the native scheduler.
/// </summary>
internal static partial class NativeMethods
{
    // Forward (managed → native) — publish per tier.
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_publish_fast(uint type_id, IntPtr payload, uint payload_size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_publish_normal(uint type_id, IntPtr payload, uint payload_size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_publish_background(uint type_id, IntPtr payload, uint payload_size, uint coalesce_key);

    // Subscribe / unsubscribe per tier.
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong df_bus_subscribe_fast(uint type_id, uint mod_id, IntPtr callback, IntPtr user_data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong df_bus_subscribe_normal(uint type_id, uint mod_id, IntPtr callback, IntPtr user_data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong df_bus_subscribe_background(uint type_id, uint mod_id, IntPtr callback, IntPtr user_data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_unsubscribe(ulong subscription_id);

    // Batched drain (scheduler-invoked at phase boundary / tick-end).
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_drain_normal_batch();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_background_queue_dispatch_idle_slot(ulong available_budget_micros);

    // Diagnostics + teardown.
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_subscriber_count_fast(uint type_id);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_subscriber_count_normal(uint type_id);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_bus_subscriber_count_background(uint type_id);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_bus_clear();
}
