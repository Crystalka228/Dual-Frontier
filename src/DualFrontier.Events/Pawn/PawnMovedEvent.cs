using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by movement logic when a pawn changes grid position.
/// Presentation layer uses this to update the visual node position.
/// </summary>
public sealed record PawnMovedEvent : IEvent
{
    /// <summary>Pawn entity that moved.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>New grid X position.</summary>
    public required int X { get; init; }

    /// <summary>New grid Y position.</summary>
    public required int Y { get; init; }
}