namespace DualFrontier.Components.Items;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

/// <summary>
/// Bed entity supporting multi-tick sleep restoration. Tracks current
/// occupant (single-pawn capacity for now) and per-tick sleep restoration
/// rate. M8.6 SleepSystem (Vanilla.Pawn) implements the state machine:
/// pawn claims bed → <see cref="Occupant"/> assigned → sleep restored
/// each tick at <see cref="SleepRestorationPerTick"/> rate (and Comfort
/// secondary at 30% of Sleep rate per master plan AD-3 hybrid formula)
/// → pawn wakes when Sleep ≥ 0.95 → <see cref="Occupant"/> cleared.
///
/// Single-occupant only in M8 — bunk-bed multi-occupant model deferred
/// to future content commits.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public sealed class BedComponent : IComponent
{
    /// <summary>
    /// Current sleeping pawn entity, or null when bed is unoccupied.
    /// SleepSystem assigns on bed-claim, clears on wake or interrupt.
    /// </summary>
    public EntityId? Occupant { get; set; }

    /// <summary>
    /// Fraction of <see cref="DualFrontier.Components.Pawn.NeedKind.Sleep"/>
    /// restored per simulation tick while occupied ([0..1]). Calibrated by
    /// M8.4 ItemFactory per bed quality tier.
    /// </summary>
    public float SleepRestorationPerTick { get; set; }
}
