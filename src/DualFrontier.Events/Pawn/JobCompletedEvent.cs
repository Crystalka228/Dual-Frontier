using DualFrontier.Contracts.Core;
using DualFrontier.Components.Pawn;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published when a pawn finishes a job successfully.
/// </summary>
public sealed record JobCompletedEvent : IEvent
{
    /// <summary>
    /// The ID of the pawn that completed the job.
    /// </summary>
    public required EntityId PawnId { get; init; }

    /// <summary>
    /// The job kind that was completed.
    /// </summary>
    public required JobKind CompletedJob { get; init; }

    /// <summary>
    /// Ticks spent on the job.
    /// </summary>
    public required int TicksSpent { get; init; }
}