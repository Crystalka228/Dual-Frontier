using System;

namespace DualFrontier.Contracts.Scheduling;

/// <summary>
/// K10.1 Item 11 — CPU affinity hint. K10.1 stores metadata only; native
/// <c>pthread_setaffinity_np</c> / <c>SetThreadAffinityMask</c> binding
/// extends к К11+ when scheduling sites consume affinity.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CpuAffinityAttribute : Attribute
{
    /// <summary>Preferred core id (0..N-1). -1 = no preference.</summary>
    public int CoreId { get; }

    /// <summary>Create affinity attribute for the given core id.</summary>
    public CpuAffinityAttribute(int coreId)
    {
        CoreId = coreId;
    }
}

/// <summary>
/// K10.1 Item 13 — Phase barrier type per spec §3.3.
/// </summary>
public enum BarrierType
{
    /// <summary>All systems в phase N complete before phase N+1 starts (default).</summary>
    Full    = 0,
    /// <summary>Data-flow-driven: subset of phase N's outputs unblock subset of N+1.</summary>
    Partial = 1,
    /// <summary>No barrier — overlap permitted (diagnostic / observability phases).</summary>
    None    = 2,
}

/// <summary>
/// K10.1 Item 13 — Per-phase barrier override marker. Default Full preserves
/// correctness; Partial/None opt-in for optimization.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PhaseBarrierAttribute : Attribute
{
    /// <summary>Barrier semantics for the phase containing this system.</summary>
    public BarrierType Type { get; }

    /// <summary>Create the phase barrier attribute.</summary>
    public PhaseBarrierAttribute(BarrierType type)
    {
        Type = type;
    }
}
