using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// How often a system's <c>Update</c> method runs, in ticks.
/// Value semantics: "one call every N game-time ticks".
/// See <see cref="TickRates"/> for the canonical constants.
/// The scheduler groups systems with the same tick rate for better cache
/// behaviour.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TickRateAttribute : Attribute
{
    /// <summary>
    /// How many game-time ticks must pass between two consecutive
    /// <c>Update</c> calls of this system.
    /// </summary>
    public int TicksPerUpdate { get; }

    /// <summary>
    /// Creates the attribute with the given rate (see <see cref="TickRates"/>).
    /// </summary>
    public TickRateAttribute(int ticksPerUpdate)
    {
        TicksPerUpdate = ticksPerUpdate;
    }
}

/// <summary>
/// Canonical tick-rate constants for <see cref="TickRateAttribute"/>.
/// Defined here so the attribute can be applied from anywhere without taking
/// a dependency on <c>DualFrontier.Core.Scheduling</c>. The constants are
/// duplicated in <c>DualFrontier.Core.Scheduling.TickRates</c> — values must
/// match.
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
