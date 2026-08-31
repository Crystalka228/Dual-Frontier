using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Carries a newly created colonist's EntityId and starting position.
///
/// NOTHING PUBLISHES THIS, and the presentation layer no longer listens. The engine's composition
/// root published one per colonist so that its own subscription could turn each into a render
/// command; W4 moved seeding into the vanilla scenario mod, which reports colonists by diffing the
/// identity span rather than by raising events, so both ends of that round trip are gone. The
/// named publisher in the previous summary, ScenarioInitializer, had already ceased to exist
/// before this wave.
///
/// The type is left standing for whoever wires spawn notification back up. Its four siblings in
/// this folder carry the same notice; this one was missed on the first pass and added when an
/// independent review measured the producer/consumer census the wave had not taken.
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