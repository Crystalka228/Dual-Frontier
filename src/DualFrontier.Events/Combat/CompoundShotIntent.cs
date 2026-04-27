using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Intent to perform a "compound" shot (ammo + magical component)
/// as part of a two-phase commit: <c>CombatSystem</c> first asks
/// <c>InventoryBus</c> (is ammo available) and <c>MagicBus</c> (is there enough mana),
/// and only then publishes the result. The response is <see cref="ShootGranted"/> or
/// <see cref="ShootRefused"/> with the same <paramref name="Id"/>.
/// </summary>
/// <param name="Id">Transaction identifier used to correlate request and response.</param>
/// <param name="Shooter">The shooter.</param>
/// <param name="Target">The target.</param>
/// <param name="AmmoType">String identifier of the ammo type
/// (TODO: Phase 4 — replace with the <c>AmmoType</c> enum).</param>
/// <param name="ManaCost">Mana cost of the magical component of the shot.</param>
public sealed record CompoundShotIntent(
    TransactionId Id,
    EntityId Shooter,
    EntityId Target,
    string AmmoType,
    float ManaCost) : IQuery;
