using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Application.Scheduler;

/// <summary>
/// K10.1 Item 15 — custom delegate type для batched system execution.
/// Needed because <c>Action&lt;T1, T2&gt;</c> does not accept <c>ReadOnlySpan</c>
/// (ref struct) as a type argument.
/// </summary>
/// <param name="ids">System ids к dispatch this batch.</param>
/// <param name="delta">Tick delta in seconds.</param>
public delegate void BatchExecutor(ReadOnlySpan<uint> ids, float delta);

/// <summary>
/// K10.1 Item 15 — Managed-side adapter для the batched callback ABI
/// (К-L12 cross-layer bridge). The static <see cref="OnBatch"/> method is
/// the <see cref="UnmanagedCallersOnlyAttribute"/> entrypoint invoked via
/// reverse-P/Invoke from the native scheduler; it routes к the instance
/// referenced by <c>batch.UserData</c> (GCHandle).
///
/// Instances allocate their own <see cref="GCHandle"/> at construction and
/// free it on <see cref="Dispose"/>. The pinned handle pointer is passed
/// to the native registry via <see cref="SchedulerAdapter.Register"/>.
///
/// The actual system execution body is pluggable via the executor delegate;
/// К10.1 Commit 11 lands the round-trip primitive с a configurable executor.
/// К10.1 Commit 14 (load-bearing) wires this к a SystemBase[] indexed table
/// so the native scheduler can dispatch Core systems through the ABI.
///
/// Constraints (per .NET 10 research, Lesson #7 verbatim):
/// <list type="bullet">
///   <item>Callback method must be <c>static</c> ✓</item>
///   <item>All args blittable (pointer + primitives only) ✓</item>
///   <item>No generics ✓</item>
///   <item>No managed exceptions across boundary ✓ (try/catch absorbs)</item>
///   <item><c>SuppressGCTransition</c> forbidden for reverse P/Invoke</item>
///   <item>GCHandle for managed instance state — Alloc at construction,
///         Free at Dispose</item>
/// </list>
/// </summary>
public sealed class ManagedSystemDispatcher : IDisposable
{
    private GCHandle _selfHandle;
    private BatchExecutor? _executor;
    private bool _disposed;

    /// <summary>Create the dispatcher; allocates a strong GCHandle к this instance.</summary>
    /// <param name="executor">Optional initial execution body (settable later via SetExecutor).</param>
    public ManagedSystemDispatcher(BatchExecutor? executor = null)
    {
        _executor = executor;
        _selfHandle = GCHandle.Alloc(this);
    }

    /// <summary>Opaque pointer passed as <c>user_data</c> к the native registry.</summary>
    public IntPtr Handle => GCHandle.ToIntPtr(_selfHandle);

    /// <summary>
    /// Replace the execution body. Used in production wiring (К10.1 Commit 14)
    /// where the executor references the SystemBase[] table and SystemExecutionContext.
    /// </summary>
    public void SetExecutor(BatchExecutor executor)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    /// <summary>
    /// Reverse-P/Invoke entrypoint. Native scheduler calls this once per phase
    /// per managed-system batch. GC transition is automatic (cannot suppress).
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static unsafe void OnBatch(NativeManagedBatch* batch)
    {
        if (batch == null) return;
        try
        {
            var handle = GCHandle.FromIntPtr((IntPtr)batch->UserData);
            if (handle.Target is not ManagedSystemDispatcher dispatcher) return;
            var ids = new ReadOnlySpan<uint>(batch->SystemIds, (int)batch->Count);
            dispatcher._executor?.Invoke(ids, batch->Delta);
        }
        catch (Exception ex)
        {
            // Exceptions cannot cross к native — absorb at boundary.
            // К10.2 wiring routes к IModFaultSink; К10.1 traces к Debug.
            System.Diagnostics.Debug.WriteLine($"[K10.1] managed batch dispatch error: {ex}");
        }
    }

    /// <summary>Release the GCHandle. Idempotent.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_selfHandle.IsAllocated) _selfHandle.Free();
    }
}
