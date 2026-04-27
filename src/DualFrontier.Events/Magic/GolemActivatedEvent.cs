using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// A mage has activated their golem. From this point on PowerSystem starts
/// charging mana from the mage-owner to sustain the golem (GDD 5.3 "Mana
/// Economy"). When the mage is exhausted a deactivation event is published (Phase 5).
/// </summary>
public sealed record GolemActivatedEvent : IEvent
{
    // TODO: public required EntityId GolemId { get; init; }
    // TODO: public required EntityId OwnerId { get; init; }
    // TODO: public required int GolemTier { get; init; }  // 1..5 — GDD 5.1
}
