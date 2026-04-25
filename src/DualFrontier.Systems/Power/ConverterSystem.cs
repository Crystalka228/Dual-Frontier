using System;
using DualFrontier.Components.Building;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Power;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Phase 4 power-to-mana converter (per GDD 9): point-to-point device that
/// reads its consumer side and emits 30% of the drawn watts via
/// <see cref="ConverterPowerOutputEvent"/> (deferred). Intentionally lossy to
/// keep the conversion situational rather than a scalable infrastructure
/// choice. A converter entity carries BOTH <see cref="PowerConsumerComponent"/>
/// and <see cref="PowerProducerComponent"/> — the latter is read only as a
/// marker that distinguishes converter entities from plain consumers.
/// <para>
/// This system used to write <c>PowerProducerComponent</c>; that created a
/// component-graph cycle with <c>ElectricGridSystem</c>
/// (ElectricGrid writes Consumer / reads Producer ↔ Converter reads Consumer
/// / writes Producer). Phase 4 closure: producer output now flows over the
/// bus only; the component cycle is gone, and the value reaches
/// ElectricGridSystem on the next tick (one-tick latency, see
/// <c>docs/FEEDBACK_LOOPS.md</c>).
/// </para>
/// Phase: 4. Tick: SLOW.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PowerConsumerComponent), typeof(PowerProducerComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Power)
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
            if (!consumer.IsPowered) continue;

            float watts = consumer.RequiredWatts * Efficiency;
            Services.Power.Publish(new ConverterPowerOutputEvent
            {
                ConverterId = entity,
                WattsOutput = watts
            });
        }
    }
}
