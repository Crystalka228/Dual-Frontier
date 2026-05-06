namespace DualFrontier.Components.Items;

using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

/// <summary>
/// Item that closes a consumable need (Satiety or Hydration) when consumed
/// by a pawn. Single-tick consume model: pawn walks to entity, ConsumeSystem
/// applies the restoration to the appropriate <see cref="NeedsComponent"/>
/// field, decrements <see cref="Charges"/>; entity is destroyed when
/// Charges reaches 0.
///
/// Used by food entities (RestoresKind = Satiety) and packaged drink
/// containers (RestoresKind = Hydration). Persistent infinite water
/// sources use <see cref="WaterSourceComponent"/> instead.
///
/// Written by M8.5 ConsumeSystem (Vanilla.Pawn) on consume; read by
/// reservation / pathfinding / interaction systems.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public sealed class ConsumableComponent : IComponent
{
    /// <summary>
    /// Which need this consumable closes when consumed. Typically
    /// <see cref="NeedKind.Satiety"/> for food, <see cref="NeedKind.Hydration"/>
    /// for drinks.
    /// </summary>
    public NeedKind RestoresKind { get; set; }

    /// <summary>
    /// Fraction of the target need restored per consume action ([0..1]).
    /// E.g., 0.4 means a single consume restores 40% of the need from
    /// current value, clamped to 1.0. Calibrated per content commit
    /// (M8.4 ItemFactory).
    /// </summary>
    public float RestorationAmount { get; set; }

    /// <summary>
    /// Remaining consume actions before the entity is destroyed.
    /// Decremented by ConsumeSystem on each consume. When Charges reaches
    /// 0, the entity is removed from the world. Default 1 for single-use
    /// food; values &gt; 1 model multi-bite items (e.g. a loaf of bread with
    /// 4 portions).
    /// </summary>
    public int Charges { get; set; } = 1;
}
