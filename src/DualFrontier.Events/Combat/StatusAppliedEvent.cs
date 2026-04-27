using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// A status effect has been applied to an entity (burning, stun, freeze,
/// corruption — see GDD 6.1). StatusSystem adds/updates the corresponding
/// modifier component and schedules its expiration.
/// </summary>
public sealed record StatusAppliedEvent : IEvent
{
    // TODO: public required EntityId TargetId { get; init; }
    // TODO: public required StatusKind Kind { get; init; }       // enum — Phase 6
    // TODO: public required float Duration { get; init; }
    // TODO: public EntityId? SourceId { get; init; }
}
