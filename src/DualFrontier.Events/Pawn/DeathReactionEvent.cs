using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// A pawn noticed a fellow's death and reacted emotionally to it.
/// Published by MoodSystem when handling the deferred <c>DeathEvent</c>:
/// adds a mood penalty, may trigger <see cref="MoodBreakEvent"/>.
/// </summary>
public sealed record DeathReactionEvent : IEvent
{
    // TODO: public required EntityId ObserverId { get; init; }
    // TODO: public required EntityId VictimId { get; init; }
    // TODO: public required int Relationship { get; init; }  // from vanilla pawn social mod content (M-series)
}
