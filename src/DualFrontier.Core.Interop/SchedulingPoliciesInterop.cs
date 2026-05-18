using System;
using DualFrontier.Contracts.Scheduling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.1 Items 6+7+8 — managed binding для the native scheduler policies
/// (process-global default <c>SchedulingPolicies</c> singleton). Wraps
/// <c>df_scheduler_policies_*</c> C ABI.
///
/// Three policy axes (per spec §3.2):
///   - Priority class (Item 6 — 5 classes; default Normal)
///   - CPU quota (Item 7 — micros per tick; 0 = unbounded)
///   - Preemption mode (Item 8 — Cooperative default; Forced for RT)
///
/// Systems that don't declare attributes use defaults (Normal + no quota +
/// Cooperative) — preserves К-L6-era behavior for un-attributed systems.
/// </summary>
public static class SchedulingPoliciesInterop
{
    /// <summary>Set the full per-system policy. Returns true (always overwrites).</summary>
    public static bool SetPolicy(uint systemId,
                                  SchedulingClass schedulingClass = SchedulingClass.Normal,
                                  int maxLatencyMicros = 0,
                                  int maxJitterMicros = 0,
                                  int cpuQuotaMicrosPerTick = 0,
                                  PreemptionMode preemptionMode = PreemptionMode.Cooperative)
    {
        int rc = NativeMethods.df_scheduler_policies_set(
            systemId,
            (int)schedulingClass,
            maxLatencyMicros,
            maxJitterMicros,
            cpuQuotaMicrosPerTick,
            (int)preemptionMode);
        return rc == 1;
    }

    /// <summary>Get the scheduling class for the given system (Normal if unset).</summary>
    public static SchedulingClass GetClass(uint systemId)
        => (SchedulingClass)NativeMethods.df_scheduler_policies_get_class(systemId);

    /// <summary>Get the CPU quota in micros per tick (0 if unset / unbounded).</summary>
    public static int GetQuota(uint systemId)
        => NativeMethods.df_scheduler_policies_get_quota(systemId);

    /// <summary>
    /// Record execution time for a system. Returns true if this recording
    /// pushed the system past its per-tick quota.
    /// </summary>
    public static bool RecordExecution(uint systemId, long micros)
        => NativeMethods.df_scheduler_policies_record_execution(systemId, micros) == 1;

    /// <summary>True iff the system has exceeded its quota this tick.</summary>
    public static bool QuotaExceeded(uint systemId)
        => NativeMethods.df_scheduler_policies_quota_exceeded(systemId) == 1;

    /// <summary>Total cumulative micros consumed by this system across all ticks.</summary>
    public static long TotalMicros(uint systemId)
        => NativeMethods.df_scheduler_policies_total_micros(systemId);

    /// <summary>Count of quota violation events for this system.</summary>
    public static int QuotaViolations(uint systemId)
        => NativeMethods.df_scheduler_policies_quota_violations(systemId);

    /// <summary>Reset per-tick stats (called at tick boundary).</summary>
    public static void ResetTickStats() => NativeMethods.df_scheduler_policies_reset_tick_stats();

    /// <summary>
    /// Order an input set of system ids by scheduling class (RealTime first,
    /// Background last). Within the same class, ascending by id для determinism.
    /// </summary>
    public static uint[] OrderByPriority(ReadOnlySpan<uint> inIds)
    {
        if (inIds.IsEmpty) return Array.Empty<uint>();
        uint[] buffer = new uint[inIds.Length];
        unsafe
        {
            fixed (uint* inPtr = inIds)
            fixed (uint* outPtr = buffer)
            {
                int n = NativeMethods.df_scheduler_policies_order_by_priority(
                    inPtr, (uint)inIds.Length, outPtr, buffer.Length);
                if (n == buffer.Length) return buffer;
                Array.Resize(ref buffer, n);
                return buffer;
            }
        }
    }

    /// <summary>Reset all policies + stats.</summary>
    public static void Clear() => NativeMethods.df_scheduler_policies_clear();
}
