using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using DualFrontier.Contracts.Bus;

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
    // =====================================================================

    private const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_publish_fast(uint type_id, IntPtr payload, uint payload_size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_publish_normal(uint type_id, IntPtr payload, uint payload_size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_publish_background(uint type_id, IntPtr payload, uint payload_size, uint coalesce_key);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong df_bus_subscribe_fast(uint type_id, uint mod_id, IntPtr callback, IntPtr user_data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong df_bus_subscribe_normal(uint type_id, uint mod_id, IntPtr callback, IntPtr user_data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong df_bus_subscribe_background(uint type_id, uint mod_id, IntPtr callback, IntPtr user_data);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_unsubscribe(ulong subscription_id);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_drain_normal_batch();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_subscriber_count_fast(uint type_id);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_subscriber_count_normal(uint type_id);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_bus_subscriber_count_background(uint type_id);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void df_bus_clear();

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
            BusTier.Fast       => df_bus_publish_fast(typeId, payloadPtr, payloadSize),
            BusTier.Normal     => df_bus_publish_normal(typeId, payloadPtr, payloadSize),
            BusTier.Background => df_bus_publish_background(typeId, payloadPtr, payloadSize, coalesceKey),
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
        ulong sid = df_bus_subscribe_fast(typeId, modId, callbackPtr, GCHandle.ToIntPtr(userDataHandle));
        if (sid != 0) _handles[sid] = userDataHandle;
        return sid;
    }

    public ulong SubscribeNormal(uint typeId, uint modId, IntPtr callbackPtr, GCHandle userDataHandle)
    {
        ulong sid = df_bus_subscribe_normal(typeId, modId, callbackPtr, GCHandle.ToIntPtr(userDataHandle));
        if (sid != 0) _handles[sid] = userDataHandle;
        return sid;
    }

    public ulong SubscribeBackground(uint typeId, uint modId, IntPtr callbackPtr, GCHandle userDataHandle)
    {
        ulong sid = df_bus_subscribe_background(typeId, modId, callbackPtr, GCHandle.ToIntPtr(userDataHandle));
        if (sid != 0) _handles[sid] = userDataHandle;
        return sid;
    }

    public bool Unsubscribe(ulong subscriptionId)
    {
        int rc = df_bus_unsubscribe(subscriptionId);
        if (_handles.TryRemove(subscriptionId, out GCHandle handle) && handle.IsAllocated)
            handle.Free();
        return rc == 1;
    }

    /// <summary>
    /// Drains the Normal tier batched dispatch (invoked by scheduler at phase
    /// boundary). Returns count of dispatched batches.
    /// </summary>
    public int DrainNormalBatch() => df_bus_drain_normal_batch();

    /// <summary>Diagnostic: count of native subscribers per tier per type.</summary>
    public int SubscriberCount(BusTier tier, uint typeId) => tier switch
    {
        BusTier.Fast       => df_bus_subscriber_count_fast(typeId),
        BusTier.Normal     => df_bus_subscriber_count_normal(typeId),
        BusTier.Background => df_bus_subscriber_count_background(typeId),
        _ => 0,
    };

    /// <summary>Test-only: clears all native bus state. Releases GC handles too.</summary>
    public void ClearForTesting()
    {
        df_bus_clear();
        foreach (var kvp in _handles)
        {
            if (kvp.Value.IsAllocated) kvp.Value.Free();
        }
        _handles.Clear();
    }
}
