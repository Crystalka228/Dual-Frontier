using System;
using DualFrontier.Components.Building;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Phase 4 power-to-mana converter (per GDD 9): point-to-point device that
/// reads its consumer side and emits 30% of the drawn watts as producer
/// output. Intentionally lossy to keep the conversion situational rather
/// than a scalable infrastructure choice.
/// Phase: 4. Tick: SLOW.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PowerConsumerComponent) },
    writes: new[] { typeof(PowerProducerComponent) },
    bus:    nameof(IGameServices.Inventory)
)]
[TickRate(TickRates.SLOW)]
public sealed class ConverterSystem : SystemBase
{
    private const float Efficiency = 0.30f;

    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        foreach (var entity in Query<PowerConsumerComponent, PowerProducerComponent>())
        {
            var consumer = GetComponent<PowerConsumerComponent>(entity);
            var producer = GetComponent<PowerProducerComponent>(entity);

            if (consumer.IsPowered)
            {
                producer.CurrentWatts = consumer.RequiredWatts * Efficiency;
                producer.IsActive     = true;
            }
            else
            {
                producer.CurrentWatts = 0f;
                producer.IsActive     = false;
            }

            SetComponent(entity, producer);
        }
    }
}
