using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.1 Item 1 — managed binding for the native scheduler system graph.
///
/// Wraps the process-global default scheduler graph singleton exposed via
/// <c>df_scheduler_*</c> C ABI. Mirrors managed <c>DependencyGraph</c>
/// (in <c>DualFrontier.Core.Scheduling</c>, internal) semantics: Kahn
/// topological sort over <c>[SystemAccess]</c> reads/writes. The managed
/// graph remains operational during the K10.1 cascade as a parallel adapter
/// facade; Commit 14 (Item 16) switches authoritative dispatch к the native
/// graph.
///
/// Thread-safety: registration / compute is single-threaded (caller
/// responsibility). Concurrent reads of computed phase composition are safe
/// once compute returns success.
/// </summary>
public static class SystemGraphInterop
{
    /// <summary>
    /// Compute return codes for graph operations.
    /// </summary>
    public enum ComputeResult : int
    {
        /// <summary>Success — phases are queryable.</summary>
        Success = 1,
        /// <summary>Generic failure (e.g. unknown id in per-tick subset).</summary>
        Failure = 0,
        /// <summary>Write-write conflict: two systems writing the same component type.</summary>
        WriteConflict = -1,
        /// <summary>Cyclic dependency detected after Kahn drains.</summary>
        Cycle = -2,
        /// <summary>
        /// Concurrency violation (F-29(a)): the native fail-loud guard detected
        /// concurrent entry into a single-threaded mutation/compute entry point
        /// and refused to touch shared state (returning without corrupting the
        /// scheduler graph). A distinct, non-swallowed outcome so a real
        /// production contract violation surfaces loudly instead of silently
        /// corrupting memory. Additive/append-only; no existing value renumbered.
        /// </summary>
        ConcurrencyViolation = -3,
    }

    /// <summary>
    /// Register a system in the native scheduler graph.
    /// Returns true on success; false on duplicate id or empty fqn.
    /// Marks the static graph dirty — call <see cref="ComputeStaticGraph"/> to rebuild.
    /// </summary>
    public static bool RegisterSystem(
        uint systemId,
        string systemFqn,
        ReadOnlySpan<uint> readComponentIds,
        ReadOnlySpan<uint> writeComponentIds,
        int priorityClass = 2,
        int wakeType = 0)
    {
        if (string.IsNullOrEmpty(systemFqn))
            throw new ArgumentException("systemFqn must be non-empty", nameof(systemFqn));

        byte[] fqnBytes = Encoding.UTF8.GetBytes(systemFqn);
        unsafe
        {
            fixed (byte* fqnPtr = fqnBytes)
            fixed (uint* readsPtr = readComponentIds)
            fixed (uint* writesPtr = writeComponentIds)
            {
                // Native expects null-terminated string. Encoding.GetBytes does
                // not append the null — write one extra byte via stackalloc.
                Span<byte> fqnZ = stackalloc byte[fqnBytes.Length + 1];
                for (int i = 0; i < fqnBytes.Length; i++) fqnZ[i] = fqnBytes[i];
                fqnZ[fqnBytes.Length] = 0;
                fixed (byte* fqnZPtr = fqnZ)
                {
                    int rc = NativeMethods.df_scheduler_register_system(
                        systemId,
                        fqnZPtr,
                        readsPtr,
                        (uint)readComponentIds.Length,
                        writesPtr,
                        (uint)writeComponentIds.Length,
                        priorityClass,
                        wakeType);
                    return rc == 1;
                }
            }
        }
    }

    /// <summary>Remove a system. Returns true if removed, false if unknown id.</summary>
    public static bool UnregisterSystem(uint systemId)
        => NativeMethods.df_scheduler_unregister_system(systemId) == 1;

    /// <summary>Number of registered systems.</summary>
    public static int SystemCount => NativeMethods.df_scheduler_system_count();

    /// <summary>Reset scheduler graph к empty state.</summary>
    public static void Clear() => NativeMethods.df_scheduler_clear();

    /// <summary>
    /// Compute static phase composition over all registered systems.
    /// Returns the compute result code.
    /// </summary>
    public static ComputeResult ComputeStaticGraph()
        => (ComputeResult)NativeMethods.df_scheduler_compute_static_graph();

    /// <summary>
    /// Number of phases in the last successful static compute. Returns 0 if
    /// compute has not been run or failed.
    /// </summary>
    public static int StaticPhaseCount => NativeMethods.df_scheduler_static_phase_count();

    /// <summary>System ids in the given static phase index, in ascending order.</summary>
    public static uint[] GetStaticPhaseSystems(int phaseIndex)
    {
        int size = NativeMethods.df_scheduler_static_phase_size(phaseIndex);
        if (size <= 0) return Array.Empty<uint>();
        uint[] buffer = new uint[size];
        unsafe
        {
            fixed (uint* p = buffer)
            {
                NativeMethods.df_scheduler_static_phase_systems(phaseIndex, p, size);
            }
        }
        return buffer;
    }

    /// <summary>
    /// Compute per-tick phase composition restricted к the runnable subset.
    /// Per K-L13 on-demand activation: only systems satisfying a wake condition
    /// for this tick are dispatched. Static [SystemAccess] data reused for edge
    /// construction; result is tighter parallelism than static phase ordering.
    /// </summary>
    public static ComputeResult ComputePerTickGraph(ReadOnlySpan<uint> runnableSystemIds)
    {
        unsafe
        {
            fixed (uint* p = runnableSystemIds)
            {
                int rc = NativeMethods.df_scheduler_compute_per_tick_graph(
                    p, (uint)runnableSystemIds.Length);
                return (ComputeResult)rc;
            }
        }
    }

    /// <summary>Number of per-tick phases in the last successful per-tick compute.</summary>
    public static int PerTickPhaseCount => NativeMethods.df_scheduler_per_tick_phase_count();

    /// <summary>System ids in the given per-tick phase index, in ascending order.</summary>
    public static uint[] GetPerTickPhaseSystems(int phaseIndex)
    {
        int size = NativeMethods.df_scheduler_per_tick_phase_size(phaseIndex);
        if (size <= 0) return Array.Empty<uint>();
        uint[] buffer = new uint[size];
        unsafe
        {
            fixed (uint* p = buffer)
            {
                NativeMethods.df_scheduler_per_tick_phase_systems(phaseIndex, p, size);
            }
        }
        return buffer;
    }

    /// <summary>
    /// Convenience: enumerate all static phases as a list of arrays.
    /// Each array contains the system ids for one phase, in ascending order.
    /// </summary>
    public static IReadOnlyList<uint[]> StaticPhases
    {
        get
        {
            int n = StaticPhaseCount;
            var phases = new uint[n][];
            for (int i = 0; i < n; i++) phases[i] = GetStaticPhaseSystems(i);
            return phases;
        }
    }

    /// <summary>Convenience: enumerate all per-tick phases as a list of arrays.</summary>
    public static IReadOnlyList<uint[]> PerTickPhases
    {
        get
        {
            int n = PerTickPhaseCount;
            var phases = new uint[n][];
            for (int i = 0; i < n; i++) phases[i] = GetPerTickPhaseSystems(i);
            return phases;
        }
    }

    /// <summary>
    /// K10.1 Item 5 — Begin a per-tick scheduling pass. Fires timer wakes,
    /// drains the runqueue into the runnable subset, and computes the per-tick
    /// graph. After this returns, <see cref="PerTickPhases"/> reflects the
    /// composition к dispatch.
    /// </summary>
    public static ComputeResult TickBegin(ulong currentTick)
        => (ComputeResult)NativeMethods.df_scheduler_tick_begin(currentTick);
}
