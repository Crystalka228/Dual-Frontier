using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published from GameBootstrap after the ItemFactory.Spawn iteration. Carries
/// the spawned item's EntityId, position and kind (presentation hint).
/// Subscribed by GameBootstrap's bridge wiring, converted to
/// <c>ItemSpawnedCommand</c> and dispatched to <c>ItemLayer</c>.
///
/// Bus: published on the Pawns bus initially. Semantic naming limitation —
/// PawnSpawnedEvent and ItemSpawnedEvent both flow on the same channel.
/// A future refactor adds an Items bus to IGameServices when additional
/// item-domain events warrant it (despawn, durability changes, etc.).
///
/// Folder placement Events/Pawn/ is expedient: only one item-spawn event for
/// now, and the file name differentiates clearly. Folder rename to
/// Events/Items/ is future cleanup.
/// </summary>
public sealed record ItemSpawnedEvent : IEvent
{
    /// <summary>Newly created item entity.</summary>
    public required EntityId ItemId { get; init; }

    /// <summary>Initial grid X position.</summary>
    public required int X { get; init; }

    /// <summary>Initial grid Y position.</summary>
    public required int Y { get; init; }

    /// <summary>Presentation hint selecting atlas region.</summary>
    public required ItemKind Kind { get; init; }
}
