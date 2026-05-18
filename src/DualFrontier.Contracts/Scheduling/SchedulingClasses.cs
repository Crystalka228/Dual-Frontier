using System;

namespace DualFrontier.Contracts.Scheduling;

/// <summary>
/// K10.1 Item 6 — Five scheduling classes per spec §3.2. Lower numeric values
/// correspond к higher priority within a phase. Mirrors native
/// <c>dualfrontier::SchedulingClass</c> for C ABI parity.
/// </summary>
public enum SchedulingClass
{
    /// <summary>Strict latency requirement; preempts other classes; bounded execution.</summary>
    RealTime   = 0,
    /// <summary>Interactive / input handling; dispatched first in phase.</summary>
    High       = 1,
    /// <summary>Default; most systems.</summary>
    Normal     = 2,
    /// <summary>Non-critical work; deferred к phase end.</summary>
    Low        = 3,
    /// <summary>Runs в idle time; may be skipped if scheduler busy.</summary>
    Background = 4,
}

/// <summary>
/// K10.1 Item 8 — Preemption mode per spec §3.2.
/// </summary>
public enum PreemptionMode
{
    /// <summary>Run к completion; yield voluntarily.</summary>
    Cooperative = 0,
    /// <summary>RT class only; scheduler may interrupt at quota boundary.</summary>
    Forced      = 1,
}

/// <summary>
/// K10.1 Item 6 — System scheduling class + latency budget.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PriorityAttribute : Attribute
{
    /// <summary>Scheduling class (default Normal).</summary>
    public SchedulingClass Class { get; }
    /// <summary>Maximum allowed latency in microseconds. 0 = unbounded.</summary>
    public int MaxLatencyMicros { get; }
    /// <summary>Maximum allowed jitter in microseconds.</summary>
    public int MaxJitterMicros { get; }

    /// <summary>Create the priority attribute.</summary>
    public PriorityAttribute(SchedulingClass schedulingClass,
                              int maxLatencyMicros = 0,
                              int maxJitterMicros = 0)
    {
        Class = schedulingClass;
        MaxLatencyMicros = maxLatencyMicros;
        MaxJitterMicros = maxJitterMicros;
    }
}

/// <summary>
/// K10.1 Item 7 — Per-system CPU time budget per tick. Exceedance triggers
/// fault handler (ModFaultHandler for mod systems; log + optional throttle
/// for Core systems).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CpuQuotaAttribute : Attribute
{
    /// <summary>Maximum CPU time the system may consume per tick.</summary>
    public int MaxMicrosPerTick { get; }

    /// <summary>Create the CPU quota attribute.</summary>
    public CpuQuotaAttribute(int maxMicrosPerTick)
    {
        MaxMicrosPerTick = maxMicrosPerTick;
    }
}

/// <summary>
/// K10.1 Item 8 — Preemption mode opt-in marker. Default is Cooperative;
/// systems requiring forced preemption (RT class only) declare it explicitly.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PreemptAttribute : Attribute
{
    /// <summary>Preemption mode (default Cooperative).</summary>
    public PreemptionMode Mode { get; }

    /// <summary>Create the preempt attribute.</summary>
    public PreemptAttribute(PreemptionMode mode = PreemptionMode.Cooperative)
    {
        Mode = mode;
    }
}
