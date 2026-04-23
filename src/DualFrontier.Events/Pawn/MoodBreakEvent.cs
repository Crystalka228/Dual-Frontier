using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by MoodSystem when a pawn mood drops below break threshold.
/// Published once per transition — repeated breaks without recovery
/// do not fire a new event.
/// </summary>
public sealed record MoodBreakEvent : IEvent
{
    /// <summary>Pawn that suffered a mood break.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>Mood value at the moment of break (0..1).</summary>
    public required float MoodValue { get; init; }
}