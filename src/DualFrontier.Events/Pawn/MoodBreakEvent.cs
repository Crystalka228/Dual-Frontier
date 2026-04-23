using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Attributes;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by MoodSystem when a pawn's mood drops below breakdown threshold.
/// Decorated with [Deferred] — delivered in the next scheduler phase.
/// </summary>
[Deferred]
public sealed record MoodBreakEvent : IEvent
{
    /// <summary>
    /// The ID of the pawn experiencing the mental breakdown.
    /// </summary>
    public required EntityId PawnId { get; init; }

    /// <summary>
    /// Mood value at the time of breakdown trigger.
    /// </summary>
    public required float MoodAtBreak { get; init; }

    /// <summary>
    /// Estimated ticks the breakdown will last.
    /// </summary>
    public required int EstimatedDurationTicks { get; init; }
}