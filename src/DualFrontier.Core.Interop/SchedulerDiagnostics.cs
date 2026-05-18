using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Scheduling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.1 Item 4 — Diagnostic accessors over the native scheduler state.
/// Mirrors real OS introspection primitives (Linux <c>/proc/&lt;pid&gt;/wchan</c>,
/// <c>ftrace</c> scheduler events). Used by debugging tools, performance
/// profilers, integration tests, and the future A'.9 Roslyn analyzer that
/// will verify wake declarations match <c>[SystemAccess]</c> declarations.
///
/// Non-mutating: these methods do not drain the runqueue or change
/// subscription state.
/// </summary>
public static class SchedulerDiagnostics
{
    /// <summary>
    /// Peek the runqueue without draining. Returns the set of system ids
    /// currently woken (in ascending order).
    /// </summary>
    public static uint[] GetRunnableSystems()
    {
        int n = NativeMethods.df_wake_registry_runqueue_size();
        if (n <= 0) return Array.Empty<uint>();
        uint[] buffer = new uint[n];
        unsafe
        {
            fixed (uint* p = buffer)
            {
                NativeMethods.df_scheduler_query_runnable(p, n);
            }
        }
        return buffer;
    }

    /// <summary>
    /// Returns the set of wake types the given system is subscribed к.
    /// Empty list if the system has no wake subscriptions.
    /// </summary>
    public static IReadOnlyList<WakeType> GetWakeSubscriptions(uint systemId)
    {
        int mask = NativeMethods.df_scheduler_query_wake_subscriptions(systemId);
        if (mask == 0) return Array.Empty<WakeType>();
        var types = new List<WakeType>(5);
        if ((mask & (1 << 0)) != 0) types.Add(WakeType.Timer);
        if ((mask & (1 << 1)) != 0) types.Add(WakeType.Event);
        if ((mask & (1 << 2)) != 0) types.Add(WakeType.StateChange);
        if ((mask & (1 << 3)) != 0) types.Add(WakeType.Init);
        if ((mask & (1 << 4)) != 0) types.Add(WakeType.Explicit);
        return types;
    }

    /// <summary>Count of subscriptions for the given wake type (diagnostic).</summary>
    public static int SubscriptionCountForType(WakeType type)
        => NativeMethods.df_wake_registry_subscription_count((int)type);
}
