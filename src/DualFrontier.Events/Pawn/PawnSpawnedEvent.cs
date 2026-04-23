using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by ScenarioInitializer when a pawn entity is created
/// and placed on the map. Presentation layer uses this to spawn
/// a visual node for the pawn.
/// </summary>
public sealed record PawnSpawnedEvent : IEvent
{
    /// <summary>Newly created pawn entity.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>Initial grid X position.</summary>
    public required int X { get; init; }

    /// <summary>Initial grid Y position.</summary>
    public required int Y { get; init; }
}