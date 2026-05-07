namespace DualFrontier.Events.Pawn;

/// <summary>
/// Categorises items for presentation rendering. Maps to sprite atlas regions
/// in <c>ItemVisual</c>. Domain truth is carried by component presence
/// (ConsumableComponent / WaterSourceComponent / BedComponent /
/// DecorativeAuraComponent); ItemKind is a presentation hint computed at
/// item-spawn time.
///
/// Lives in DualFrontier.Events because it is part of <see cref="ItemSpawnedEvent"/>'s
/// payload. Application's <c>ItemSpawnedCommand</c> reuses the same enum via
/// the existing Application → Events reference; placing the enum in
/// Application would create a circular dependency since Events cannot
/// reference Application.
/// </summary>
public enum ItemKind
{
    Food,
    Water,
    Bed,
    Decoration,
}
