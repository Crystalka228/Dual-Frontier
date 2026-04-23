using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by NeedsSystem when a need crosses CriticalThreshold.
/// JobSystem subscribes to assign Eat or Sleep jobs immediately.
/// </summary>
public sealed record NeedsCriticalEvent : IEvent
{
    /// <summary>
    /// The ID of the pawn whose need reached critical level.
    /// </summary>
    public required EntityId PawnId { get; init; }

    /// <summary>
    /// Name of the critical need: "Hunger", "Thirst", "Rest", "Comfort".
    /// </summary>
    public required string NeedName { get; init; }

    /// <summary>
    /// Current value of the need (0..1).
    /// </summary>
    public required float Value { get; init; }
}