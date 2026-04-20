using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.World;

/// <summary>
/// Биом региона карты. Влияет на мудовые модификаторы пешек, доступную
/// флору/фауну, погодные эффекты и распределение ресурсов.
/// </summary>
public sealed class BiomeComponent : IComponent
{
    // TODO: создать DualFrontier.Components.World.BiomeKind enum (TemperateForest, Desert, Tundra, Swamp, EtherWastes …) — Фаза 2.
    // TODO: public BiomeKind Kind;
}
