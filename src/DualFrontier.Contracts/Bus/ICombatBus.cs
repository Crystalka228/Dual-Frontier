using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Шина боевого домена. События: попытка выстрела, нанесение урона, смерть,
/// применение статус-эффекта.
/// Пишут: <c>CombatSystem</c>, <c>ProjectileSystem</c>.
/// Читают: <c>DamageSystem</c>, <c>StatusEffectSystem</c>.
/// </summary>
public interface ICombatBus : IEventBus
{
}
