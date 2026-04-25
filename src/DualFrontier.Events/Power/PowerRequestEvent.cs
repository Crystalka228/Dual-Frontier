using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// Published by a consumer building when it needs power.
/// ElectricGridSystem responds with PowerGrantedEvent
/// or leaves IsPowered=false if grid is overloaded.
/// </summary>
public sealed record PowerRequestEvent : IEvent
{
    /// <summary>Consumer entity requesting power.</summary>
    public required EntityId ConsumerId { get; init; }

    /// <summary>Watts requested.</summary>
    public required float WattsRequested { get; init; }

    /// <summary>Priority. Higher = served first on overload.</summary>
    public int Priority { get; init; } = 1;
}