using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Исполнение заклинаний: проверяет ресурс/школу, публикует
/// <c>SpellCastEvent</c> и целевые эффекты (DamageEvent,
/// StatusApplied) в соответствующие шины. САМА НЕ ПИШЕТ
/// компоненты — только читает и публикует события.
///
/// Фаза: 4 (после ManaSystem).
/// Тик: FAST (3 фрейма) — отзывчивость заклинаний.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ManaComponent), typeof(SchoolComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.FAST)]
public sealed class SpellSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на SpellCastAttemptEvent, ManaGranted, ManaRefused.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 4 — подписка на события кастов");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 4 — продвижение активных кастов, публикация SpellCastEvent.
    }
}
