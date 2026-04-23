using DualFrontier.Contracts.Core;
using DualFrontier.Components.Pawn;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by JobSystem when a pawn receives a new job assignment.
/// </summary>
public sealed record JobAssignedEvent : IEvent
{
    /// <summary>
    /// The ID of the pawn that received the job.
    /// </summary>
    public required EntityId PawnId { get; init; }

    /// <summary>
    /// The job kind assigned.
    /// </summary>
    public required JobKind Job { get; init; }
}