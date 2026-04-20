using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Тики статус-эффектов (горение, отравление, страх и т. д.):
/// накладывает урон на здоровье и/или влияет на настроение.
/// Публикует <c>StatusAppliedEvent</c> / <c>StatusExpiredEvent</c>.
///
/// Фаза: 2.
/// Тик: FAST (3 фрейма).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(MindComponent), typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
public sealed class StatusEffectSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на StatusAppliedEvent, StatusRemovedEvent.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 5 — подписка на события статус-эффектов");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 5 — продвинуть все активные эффекты, применить их урон/настроение, убрать истёкшие.
    }
}
