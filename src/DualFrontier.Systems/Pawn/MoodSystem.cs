using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Пересчитывает настроение пешки по её нуждам и здоровью.
/// При падении ниже порога публикует <c>MoodBreakEvent</c> в
/// <c>Pawns</c> шину.
///
/// Фаза: 3 (пешки).
/// Тик: SLOW (60 фреймов) — настроение инертно.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent), typeof(HealthComponent) },
    writes: new[] { typeof(MindComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.SLOW)]
public sealed class MoodSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на WitnessDeathEvent, GoodEventSeenEvent.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 3 — подписка на события настроения");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 3 — mood = f(needs, health, недавние события).
    }
}
