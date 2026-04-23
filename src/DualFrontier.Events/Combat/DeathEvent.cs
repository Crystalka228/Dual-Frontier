using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Published by DamageSystem when a pawn's health reaches zero.
/// Marked [Deferred] — MoodSystem and SocialSystem receive it
/// in the next scheduler phase, after the entity is already removed.
/// </summary>
[Deferred]
public sealed record DeathEvent : IEvent
{
    /// <summary>Entity that died.</summary>
    public required EntityId Who { get; init; }

    /// <summary>Grid X position where death occurred.</summary>
    public required int X { get; init; }

    /// <summary>Grid Y position where death occurred.</summary>
    public required int Y { get; init; }
}