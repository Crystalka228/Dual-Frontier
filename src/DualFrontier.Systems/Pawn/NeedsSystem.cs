using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Events.Pawn;
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
    public override void Update(float delta)
    {
        foreach (var entityId in Query<NeedsComponent>())
        {
            NeedsComponent needs = GetComponent<NeedsComponent>(entityId);

            needs.Hunger += 0.005f * delta;
            needs.Thirst += 0.008f * delta;
            needs.Rest += 0.003f * delta;
            needs.Comfort += 0.002f * delta;

            needs.Hunger = Math.Min(1f, Math.Max(0f, needs.Hunger));
            needs.Thirst = Math.Min(1f, Math.Max(0f, needs.Thirst));
            needs.Rest = Math.Min(1f, Math.Max(0f, needs.Rest));
            needs.Comfort = Math.Min(1f, Math.Max(0f, needs.Comfort));

            if (needs.IsHungry)
                Services.Pawns.Publish(new NeedsCriticalEvent
                {
                    PawnId = entityId,
                    NeedName = "Hunger",
                    Value = needs.Hunger
                });
            if (needs.IsExhausted)
                Services.Pawns.Publish(new NeedsCriticalEvent
                {
                    PawnId = entityId,
                    NeedName = "Rest",
                    Value = needs.Rest
                });

            SetComponent(entityId, needs);
        }
    }
}
