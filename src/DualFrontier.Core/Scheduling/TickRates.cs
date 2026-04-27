using System;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Canonical tick-rate values.
/// Duplicates <c>DualFrontier.Contracts.Attributes.TickRates</c> —
/// values must match, otherwise the scheduler and attributes diverge.
/// The duplicate exists so Core systems do not need to import the
/// attributes namespace for runtime comparisons.
/// </summary>
public static class TickRates
{
    /// <summary>Every tick — projectile physics, UI responsiveness.</summary>
    public const int REALTIME = 1;

    /// <summary>Every 3 ticks — combat, responsive systems.</summary>
    public const int FAST = 3;

    /// <summary>Every 15 ticks — normal logic: jobs, skills, mana.</summary>
    public const int NORMAL = 15;

    /// <summary>Every 60 ticks (~1/sec) — needs, mood, ether growth.</summary>
    public const int SLOW = 60;

    /// <summary>Every 3600 ticks (~1/min) — social, raids, trade.</summary>
    public const int RARE = 3600;
}
