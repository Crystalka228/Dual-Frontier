using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// A pawn's social relationships with others. Key — EntityId of the other
/// pawn, value — numeric affinity level (negative = enmity).
/// Updated by SocialSystem (RARE tick) on interactions.
/// </summary>
public sealed class SocialComponent : IComponent
{
    // TODO: public Dictionary<EntityId, int> Relations = new();
}
