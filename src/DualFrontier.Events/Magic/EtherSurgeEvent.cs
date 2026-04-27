using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Ether surge. See GDD 4.2 "Mage progression — sources of experience":
/// when working with raw ether / a pure crystal, a mage risks an
/// uncontrolled energy discharge. Consequences — damage, statuses,
/// a local anomaly; on large surges — dispatch of <see cref="Combat.StatusAppliedEvent"/>
/// and/or <see cref="Combat.DamageEvent"/>.
/// </summary>
public sealed record EtherSurgeEvent : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float Magnitude { get; init; }   // surge strength
    // TODO: public GridVector Epicenter { get; init; }
}
