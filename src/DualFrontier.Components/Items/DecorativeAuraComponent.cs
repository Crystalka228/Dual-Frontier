namespace DualFrontier.Components.Items;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

/// <summary>
/// Passive ambient comfort emitter. Pawns within Chebyshev distance
/// <see cref="Radius"/> of an entity carrying this component receive
/// <see cref="ComfortPerTick"/> restoration к their
/// <see cref="DualFrontier.Components.Pawn.NeedKind.Comfort"/> per
/// simulation tick.
///
/// M8.7 ComfortAuraSystem (Vanilla.Pawn) drives the queries — uses
/// <c>DualFrontier.Core.Math.SpatialGrid.GetInRadius</c> centered
/// on each decoration entity, applies comfort to all pawns в результат.
///
/// Decoration entities are static — auras don't move or change state.
/// This is the secondary Comfort path; primary is bed-sleep proportional
/// formula (ΔComfort = ΔSleep × 0.3 per master plan AD-3).
/// </summary>
[ModAccessible(Read = true)]
public sealed class DecorativeAuraComponent : IComponent
{
    /// <summary>
    /// Chebyshev radius (in tiles) of comfort emission. Matches semantics
    /// of <c>DualFrontier.Core.Math.SpatialGrid.GetInRadius</c> —
    /// distance is max(|Δx|, |Δy|), so radius=2 forms a 5×5 square area.
    /// </summary>
    public int Radius { get; set; }

    /// <summary>
    /// Fraction of <see cref="DualFrontier.Components.Pawn.NeedKind.Comfort"/>
    /// restored per simulation tick для each pawn within
    /// <see cref="Radius"/> ([0..1]). Calibrated by M8.4 ItemFactory per
    /// decoration tier (rug = small, statue = large).
    /// </summary>
    public float ComfortPerTick { get; set; }
}
