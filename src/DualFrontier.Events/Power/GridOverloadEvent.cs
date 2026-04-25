using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// Published by ElectricGridSystem when total demand
/// exceeds total supply. Low-priority consumers lose power.
/// </summary>
public sealed record GridOverloadEvent : IEvent
{
    /// <summary>Total watts demanded by all consumers.</summary>
    public required float TotalDemand { get; init; }

    /// <summary>Total watts available from all producers.</summary>
    public required float TotalSupply { get; init; }

    /// <summary>Number of consumers that lost power.</summary>
    public required int UnpoweredCount { get; init; }
}