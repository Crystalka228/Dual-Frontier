using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Combat;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Магические и технические щиты: регенерация, перекрытие от
/// урона (вместе с <c>DamageSystem</c>). Публикует
/// <c>ShieldBrokenEvent</c> при пробитии.
///
/// Фаза: 2.
/// Тик: FAST (3 фрейма).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ShieldComponent) },
    writes: new[] { typeof(ShieldComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
public sealed class ShieldSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на ShieldHitEvent, ShieldRechargeEvent.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 5 — подписка на события щитов");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 5 — регенерация щитов по тикам, поглощение входящего урона.
    }
}
