using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Refusal to perform a compound shot. Published by <c>CombatSystem</c>
/// if at least one of the two buses (Inventory or Magic) refused the
/// <see cref="CompoundShotIntent"/>. AI/player picks an alternative action.
/// </summary>
/// <param name="Id">Transaction identifier (matches the originating Intent).</param>
/// <param name="Shooter">The shooter that was refused.</param>
/// <param name="Reason">Refusal reason.</param>
public sealed record ShootRefused(
    TransactionId Id,
    EntityId Shooter,
    ShotRefusalReason Reason) : IEvent;
