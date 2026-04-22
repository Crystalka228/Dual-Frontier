using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Деградация нужд пешки во времени: голод, жажда, сон, отдых.
/// Публикует сигнал в <c>Pawns</c> шину, когда нужда пересекает
/// критический порог.
///
/// Фаза: 3 (пешки).
/// Тик: SLOW (60 фреймов) — нужды меняются медленно.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent) },
    writes: new[] { typeof(NeedsComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.SLOW)]
public sealed class NeedsSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на FoodConsumedEvent, SleepEndedEvent.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 3 — подписка на события нужд");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 3 — итерация по всем пешкам, падение нужд, публикация порогов.
    }
}
