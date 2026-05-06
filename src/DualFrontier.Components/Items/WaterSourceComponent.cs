namespace DualFrontier.Components.Items;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

/// <summary>
/// Persistent infinite source of <see cref="DualFrontier.Components.Pawn.NeedKind.Hydration"/>.
/// Unlike <see cref="ConsumableComponent"/>, the entity is never destroyed —
/// pawns drink as many times as needed. Used for natural water sources
/// (rivers, wells) and infinite plumbing fixtures.
///
/// M8.5 ConsumeSystem reads <see cref="RestorationAmount"/> to apply
/// hydration restoration on drink action. No write paths — sources don't
/// deplete.
/// </summary>
[ModAccessible(Read = true)]
public sealed class WaterSourceComponent : IComponent
{
    /// <summary>
    /// Fraction of <see cref="DualFrontier.Components.Pawn.NeedKind.Hydration"/>
    /// restored per drink action ([0..1]). E.g., 0.6 restores 60% of
    /// hydration per drink.
    /// </summary>
    public float RestorationAmount { get; set; }
}
