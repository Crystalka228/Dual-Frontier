using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.World;

/// <summary>
/// Тайл карты: тип рельефа и признак проходимости.
/// Проходимость используется PathfindingSystem; тип рельефа —
/// средой визуализации и системами влажности/температуры.
/// </summary>
public sealed class TileComponent : IComponent
{
    // TODO: создать DualFrontier.Components.World.TerrainKind enum (Grass, Rock, Sand, Water, Ice, Swamp, Arcane …) — Фаза 2.
    // TODO: public TerrainKind Terrain;
    // TODO: public bool Passable;
}
