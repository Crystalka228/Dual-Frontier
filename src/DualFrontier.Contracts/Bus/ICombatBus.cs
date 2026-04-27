using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Combat-domain bus. Events: shoot attempt, damage dealt, death, status
/// effect applied.
/// Writers: <c>CombatSystem</c>, <c>ProjectileSystem</c>.
/// Readers: <c>DamageSystem</c>, <c>StatusEffectSystem</c>.
/// </summary>
public interface ICombatBus : IEventBus
{
}
