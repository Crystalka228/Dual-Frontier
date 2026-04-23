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
    // Currently empty as no events are subscribed.
}

public override void Update(float delta)
{
    foreach (var entityId in Query<NeedsComponent>())
    {
        NeedsComponent needs = GetComponent<NeedsComponent>(entityId);

        // Apply decay rates multiplied by delta and clamp values to [0, 1]
        needs.Hunger += 0.005f * delta;
        needs.Thirst += 0.008f * delta;
        needs.Rest += 0.003f * delta;
        needs.Comfort += 0.002f * delta;

// Clamp values to [0, 1] using standard math functions
needs.Hunger = Math.Min(1f, Math.Max(0f, needs.Hunger));
needs.Thirst = Math.Min(1f, Math.Max(0f, needs.Thirst));
needs.Rest = Math.Min(1f, Math.Max(0f, needs.Rest));
needs.Comfort = Math.Min(1f, Math.Max(0f, needs.Comfort));

        // TODO comment: publish critical events when >= CriticalThreshold (e.g., eventBus.Publish(new NeedsCriticalEvent(entityId, needs.Hunger)))

        SetComponent(entityId, needs);
    }
}
}
