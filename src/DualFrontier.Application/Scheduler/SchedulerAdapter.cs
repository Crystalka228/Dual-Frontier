using System;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Application.Scheduler;

/// <summary>
/// K10.1 Item 15 — Registration helper для the native scheduler batched
/// callback ABI. Registers <see cref="ManagedSystemDispatcher.OnBatch"/>
/// against the native registry, providing the opaque GCHandle as user_data.
///
/// Used by Commit 14 (load-bearing K-L6 supersession) to wire the native
/// scheduler's per-phase dispatch к the managed system table.
/// </summary>
public static class SchedulerAdapter
{
    /// <summary>
    /// Register the managed dispatcher с the native scheduler. After this,
    /// native dispatch calls <see cref="ManagedSystemDispatcher.OnBatch"/>
    /// passing the dispatcher's GCHandle through user_data.
    /// </summary>
    public static unsafe void Register(ManagedSystemDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        delegate* unmanaged[Cdecl]<NativeManagedBatch*, void> fp =
            &ManagedSystemDispatcher.OnBatch;
        NativeMethods.df_scheduler_register_managed_callback(
            fp, dispatcher.Handle.ToPointer());
    }

    /// <summary>
    /// Dispatch a batch to the registered managed callback. Used by tests
    /// и by the native scheduler (which calls the C ABI directly при
    /// phase boundary). Returns true on success, false если no callback
    /// is registered.
    /// </summary>
    public static unsafe bool DispatchBatch(
        ReadOnlySpan<uint> systemIds,
        float delta,
        IntPtr userData = default)
    {
        if (systemIds.IsEmpty) return false;
        fixed (uint* idsPtr = systemIds)
        {
            NativeManagedBatch batch;
            batch.SystemIds = idsPtr;
            batch.Count = (uint)systemIds.Length;
            batch.Delta = delta;
            batch.UserData = userData.ToPointer();
            return NativeMethods.df_scheduler_dispatch_managed_batch(&batch) == 1;
        }
    }

    /// <summary>True iff a managed callback has been registered.</summary>
    public static bool IsCallbackRegistered
        => NativeMethods.df_scheduler_managed_callback_registered() == 1;

    /// <summary>Clear the registered managed callback.</summary>
    public static void ClearCallback() => NativeMethods.df_scheduler_clear_managed_callback();
}
