using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// Published by ElectricGridSystem when a consumer
/// receives power allocation.
/// </summary>
public sealed record PowerGrantedEvent : IEvent
{
    /// <summary>Consumer entity that received power.</summary>
    public required EntityId ConsumerId { get; init; }

    /// <summary>Watts actually granted (may be less than requested).</summary>
    public required float WattsGranted { get; init; }
}