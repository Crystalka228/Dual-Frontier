using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Interop;

namespace DualFrontier.Application.Bus;

/// <summary>
/// К10.2 Item 27 — C ABI bridge between <see cref="BusFacade"/> и native bus
/// (К-L15). Forward path: P/Invoke к <c>df_bus_publish_*</c>; reverse path:
/// reverse-P/Invoke callbacks dispatched from native back к managed
/// subscriber adapters.
///
/// Reverse-P/Invoke constraints (per К10.1 Item 15 baseline + .NET 10
/// research, Lesson #7 verbatim):
///   - Callback method must be static + <see cref="UnmanagedCallersOnlyAttribute"/>
///   - All args blittable
///   - No generics on the callback signature
///   - No managed exceptions across the boundary (try/catch absorbs)
///   - SuppressGCTransition forbidden for reverse P/Invoke
///   - GCHandle for managed instance state — alloc'd at subscribe, freed at
///     unsubscribe
///
/// К10.2 К-L15 strategy «managed-facade-preserved»: this bridge lands as
/// additive parallel infrastructure. <see cref="BusFacade"/> consumes either
/// the managed bus (default) or this bridge (opt-in for verification);
/// sovereign authority switch defers к К10.4 closure.
/// </summary>
public sealed class ManagedBusBridge
{
    // GCHandle pool — keeps managed subscriber adapter alive while a native
    // subscription exists. Keyed by native subscription_id returned from
    // df_bus_subscribe_*. Cleared on unsubscribe.
    private readonly ConcurrentDictionary<ulong, GCHandle> _handles = new();

    // =====================================================================
    // Forward (managed → native) — publish per tier
    //
    // К-L15 native bus P/Invoke (df_bus_*) relocated to
    // DualFrontier.Core.Interop.NativeMethods at A'.9.1 Phase β C9 (DFK002
    // triage): the bindings belong on the §8-sanctioned Core.Interop kernel
    // boundary — df_bus_* lives in DualFrontier.Core.Native.dll — not the
    // Application layer. Reached via the existing
    // InternalsVisibleTo(DualFrontier.Application) grant, the same pattern
    // the scheduler adapter uses for the native scheduler.
    // =====================================================================

    /// <summary>
    /// Publishes a fixed-blittable-payload event to the native bus via the
    /// tier appropriate to its event type registry registration.
    /// <paramref name="payloadPtr"/> is a pinned buffer pointer of
    /// <paramref name="payloadSize"/> bytes.
    /// </summary>
    public int PublishViaNative(uint typeId, BusTier tier, IntPtr payloadPtr, uint payloadSize, uint coalesceKey = 0)
    {
        return tier switch
        {
            BusTier.Fast       => NativeMethods.df_bus_publish_fast(typeId, payloadPtr, payloadSize),
            BusTier.Normal     => NativeMethods.df_bus_publish_normal(typeId, payloadPtr, payloadSize),
            BusTier.Background => NativeMethods.df_bus_publish_background(typeId, payloadPtr, payloadSize, coalesceKey),
            _ => 0,
        };
    }

    /// <summary>
    /// Registers a Fast tier subscriber. Caller provides the unmanaged
    /// callback pointer (delegate marshalled via
    /// <see cref="Marshal.GetFunctionPointerForDelegate{TDelegate}(TDelegate)"/>)
    /// и a <see cref="GCHandle"/>-pinned user-data block describing the managed
    /// adapter. Returns the native subscription id, or 0 on failure.
    /// </summary>
    public ulong SubscribeFast(uint typeId, uint modId, IntPtr callbackPtr, GCHandle userDataHandle)
    {
        ulong sid = NativeMethods.df_bus_subscribe_fast(typeId, modId, callbackPtr, GCHandle.ToIntPtr(userDataHandle));
        if (sid != 0) _handles[sid] = userDataHandle;
        return sid;
    }

    public ulong SubscribeNormal(uint typeId, uint modId, IntPtr callbackPtr, GCHandle userDataHandle)
    {
        ulong sid = NativeMethods.df_bus_subscribe_normal(typeId, modId, callbackPtr, GCHandle.ToIntPtr(userDataHandle));
        if (sid != 0) _handles[sid] = userDataHandle;
        return sid;
    }

    public ulong SubscribeBackground(uint typeId, uint modId, IntPtr callbackPtr, GCHandle userDataHandle)
    {
        ulong sid = NativeMethods.df_bus_subscribe_background(typeId, modId, callbackPtr, GCHandle.ToIntPtr(userDataHandle));
        if (sid != 0) _handles[sid] = userDataHandle;
        return sid;
    }

    public bool Unsubscribe(ulong subscriptionId)
    {
        int rc = NativeMethods.df_bus_unsubscribe(subscriptionId);
        if (_handles.TryRemove(subscriptionId, out GCHandle handle) && handle.IsAllocated)
            handle.Free();
        return rc == 1;
    }

    /// <summary>
    /// Drains the Normal tier batched dispatch (invoked by scheduler at phase
    /// boundary). Returns count of dispatched batches.
    /// </summary>
    public int DrainNormalBatch() => NativeMethods.df_bus_drain_normal_batch();

    /// <summary>
    /// Drains the Background tier coalesce + dispatch loop within the supplied
    /// idle-slot budget (microseconds). Pass 0 для unbounded — caller covers
    /// the whole pending queue. К-L15 §3.8 Item 30 names the scheduler as the
    /// invoker of this surface at tick-end; <see cref="DualFrontier.Application.Loop.GameLoop"/>
    /// calls it after each fixed step с budget = remaining tick period.
    /// </summary>
    /// <returns>Count of background events dispatched in this call.</returns>
    public int DrainBackgroundBatch(ulong availableBudgetMicros)
        => NativeMethods.df_background_queue_dispatch_idle_slot(availableBudgetMicros);

    /// <summary>Diagnostic: count of native subscribers per tier per type.</summary>
    public int SubscriberCount(BusTier tier, uint typeId) => tier switch
    {
        BusTier.Fast       => NativeMethods.df_bus_subscriber_count_fast(typeId),
        BusTier.Normal     => NativeMethods.df_bus_subscriber_count_normal(typeId),
        BusTier.Background => NativeMethods.df_bus_subscriber_count_background(typeId),
        _ => 0,
    };

    /// <summary>Test-only: clears all native bus state. Releases GC handles too.</summary>
    public void ClearForTesting()
    {
        NativeMethods.df_bus_clear();
        foreach (var kvp in _handles)
        {
            if (kvp.Value.IsAllocated) kvp.Value.Free();
        }
        _handles.Clear();
    }
}
