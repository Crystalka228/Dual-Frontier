using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Движение снарядов по прямой, регистрация столкновений.
/// При попадании публикует <c>ProjectileHitEvent</c> в
/// <c>Combat</c> шину — <c>DamageSystem</c> конвертирует попадание
/// в урон.
///
/// Фаза: 1 (параллельно с CombatSystem).
/// Тик: REALTIME (1 фрейм) — визуально корректный полёт.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PositionComponent) },
    writes: new[] { typeof(PositionComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.REALTIME)]
[BridgeImplementation(Phase = 5)]
public sealed class ProjectileSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to ProjectileSpawnedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 5 — продвижение снарядов на delta, поиск коллизий.
    }
}
