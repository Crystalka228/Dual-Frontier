using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Confirmation of a compound shot: both ammo and mana were charged successfully.
/// Published by <c>CombatSystem</c> after both buses
/// (Inventory and Magic) responded positively to <see cref="CompoundShotIntent"/>
/// with the same <paramref name="Id"/>.
/// </summary>
/// <param name="Id">Transaction identifier (matches the originating Intent).</param>
/// <param name="Shooter">The shooter.</param>
/// <param name="Target">The target.</param>
/// <param name="AmmoType">Type of ammo consumed.</param>
/// <param name="ManaCost">Mana charged.</param>
public sealed record ShootGranted(
    TransactionId Id,
    EntityId Shooter,
    EntityId Target,
    string AmmoType,
    float ManaCost) : IEvent;
