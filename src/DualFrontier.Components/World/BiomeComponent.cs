using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.World;

/// <summary>
/// Biome of a map region. Affects pawn mood modifiers, available flora/fauna,
/// weather effects, and resource distribution.
/// </summary>
public sealed class BiomeComponent : IComponent
{
    // TODO: introduce DualFrontier.Components.World.BiomeKind enum (TemperateForest, Desert, Tundra, Swamp, EtherWastes …) — Phase 2.
    // TODO: public BiomeKind Kind;
}
