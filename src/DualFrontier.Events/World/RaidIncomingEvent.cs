using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.World;

/// <summary>
/// A raid is approaching the colony. Published by RaidDirectorSystem with a margin
/// of in-game time so that the colony has time to prepare. Specific
/// skirmishes are then triggered by Combat-domain events.
/// </summary>
public sealed record RaidIncomingEvent : IEvent
{
    // TODO: public required string FactionId { get; init; } = string.Empty;
    // TODO: public required int EnemyCount { get; init; }
    // TODO: public required float ThreatPoints { get; init; }
    // TODO: public required float EtaSeconds { get; init; }   // how much in-game time until the attack
}
