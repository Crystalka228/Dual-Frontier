using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Components.Combat;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Применение урона: читает DamageEvent из шины <c>Combat</c>,
/// учитывает броню и остаток щитов, пишет итог в
/// <see cref="HealthComponent"/>. При обнулении HP публикует
/// <c>DeathEvent</c>.
///
/// Фаза: 2 (после CombatSystem и ProjectileSystem).
/// Тик: FAST (3 фрейма).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ArmorComponent), typeof(ShieldComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
[BridgeImplementation(Phase = 5)]
public sealed class DamageSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to DamageEvent / ProjectileHitEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 2 — обработать накопленные DamageEvent, применить формулу, опубликовать DeathEvent.
    }
}
