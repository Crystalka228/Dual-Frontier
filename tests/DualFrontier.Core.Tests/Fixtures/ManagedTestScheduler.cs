using System;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Tests.Fixtures;

/// <summary>
/// K10.1 Item 24 — Test fixture mirror к <see cref="ManagedTestWorld"/> for
/// the К-L12 scheduler scope. Scheduler primitives (system graph, wake
/// registry, scheduling policies, state change filter, scheduler trace,
/// scheduler intrinsics, managed callback) are process-global singletons
/// в native code; tests that exercise scheduler behavior must reset these
/// between runs к maintain isolation.
///
/// Usage pattern:
/// <code>
/// using var scheduler = new ManagedTestScheduler();
/// // ... configure scheduler ...
/// // ... run scheduler logic ...
/// // (Dispose automatically resets singletons)
/// </code>
///
/// К10.1 K-L6 SUPERSEDED → K-L12 cascade: this fixture replaces the implicit
/// reset previously achieved by constructing a new managed
/// <c>ParallelSystemScheduler</c> с fresh DependencyGraph. Post-К-L12, the
/// scheduler state is process-global; explicit reset is the new isolation
/// mechanism.
///
/// К8.34 v2.0 precedent (<see cref="ManagedTestWorld"/>): test fixture
/// names use the «ManagedTest...» prefix to mark them as test-only types
/// retained for testability, parallel к the К-series cutover that retired
/// the managed types from production.
/// </summary>
public sealed class ManagedTestScheduler : IDisposable
{
    private bool _disposed;

    /// <summary>Construct the fixture and reset all scheduler singletons.</summary>
    public ManagedTestScheduler()
    {
        ResetAll();
    }

    /// <summary>Explicit reset (called automatically on Dispose).</summary>
    public static void ResetAll()
    {
        SystemGraphInterop.Clear();
        WakeRegistryInterop.Clear();
        SchedulingPoliciesInterop.Clear();
        ShmRegionInterop.Clear();
        // K10.1 Item 17 — state change filter
        NativeMethods.df_state_filter_clear();
        // K10.1 Item 19 — trace (disabled + cleared)
        NativeMethods.df_scheduler_trace_set_enabled(0);
        NativeMethods.df_scheduler_trace_clear();
        // K10.1 Item 20 — intrinsics (suspended/panic flags cleared)
        NativeMethods.df_scheduler_intrinsics_reset();
        // K10.1 Item 15 — managed callback registry
        NativeMethods.df_scheduler_clear_managed_callback();
    }

    /// <summary>Reset singletons on Dispose. Idempotent.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        ResetAll();
    }
}
