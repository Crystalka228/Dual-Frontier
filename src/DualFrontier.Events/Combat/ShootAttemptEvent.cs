using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// A pawn is attempting to fire a shot. Published by AI or the player.
/// CombatSystem checks the presence of weapon/ammo and publishes the
/// corresponding <see cref="AmmoIntent"/>.
/// </summary>
public sealed record ShootAttemptEvent : IEvent
{
    // TODO: public required EntityId AttackerId { get; init; }
    // TODO: public required EntityId TargetId { get; init; }
}
