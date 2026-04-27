using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.World;

/// <summary>
/// Ether node parameters have changed (level, radius, destroyed).
/// ManaSystem recalculates mana regeneration for pawns in the area of influence,
/// UI updates the minimap.
/// </summary>
public sealed record EtherNodeChangedEvent : IEvent
{
    // TODO: public required EntityId NodeId { get; init; }
    // TODO: public int NewTier { get; init; }
    // TODO: public float NewRadius { get; init; }
    // TODO: public bool Destroyed { get; init; }
}
