namespace DualFrontier.Events.Pawn;

/// <summary>
/// Categorises items for presentation rendering. Maps to sprite atlas regions
/// in <c>ItemVisual</c>. Domain truth is carried by component presence
/// (ConsumableComponent / WaterSourceComponent / BedComponent /
/// DecorativeAuraComponent); ItemKind is a presentation hint computed at
/// item-spawn time.
///
/// Lives in DualFrontier.Events because it is part of <see cref="ItemSpawnedEvent"/>'s
/// payload. The engine's render command carried this same enum until W4, through an
/// Application → Events reference that no longer exists; it carries an opaque int now,
/// because a game enum cannot appear in an engine assembly (boundary law B-2). Whoever
/// revives item visuals owns the mapping between the two.
/// </summary>
public enum ItemKind
{
    Food,
    Water,
    Bed,
    Decoration,
}
