using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Published by DamageSystem when a pawn's health reaches zero.
/// Marked [Deferred] — MoodSystem and SocialSystem receive it
/// in the next scheduler phase, after the entity is already removed.
/// </summary>
public sealed record DeathEvent : IEvent
{
    /// <summary>Entity that died.</summary>
    public required EntityId Who { get; init; }

    /// <summary>Grid position where death occurred.</summary>
    public required int X { get; init; }

    /// <summary>Grid position where death occurred.</summary>
    public required int Y { get; init; }
}