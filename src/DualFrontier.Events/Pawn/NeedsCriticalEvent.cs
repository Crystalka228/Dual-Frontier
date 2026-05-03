using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by NeedsSystem when a pawn's wellness drops to or below
/// CriticalThreshold. JobSystem subscribes to prioritise the matching
/// recovery job (Eat, Sleep).
/// </summary>
public sealed record NeedsCriticalEvent : IEvent
{
    /// <summary>The pawn whose need reached critical level.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>Which need is critical.</summary>
    public required NeedKind Need { get; init; }

    /// <summary>Current value of the need in [0..1].</summary>
    public required float Value { get; init; }
}
