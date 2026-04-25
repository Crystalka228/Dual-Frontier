using System;
using System.Collections.Generic;
using DualFrontier.Components.Building;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Power;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Phase 4 electric grid: each SLOW tick sums producer output, sorts
/// consumers by priority (highest first), and allocates supply until
/// exhausted. Consumers above the supply line stay powered; the rest are
/// switched off. Publishes PowerGrantedEvent per powered consumer and
/// GridOverloadEvent when any consumer is unpowered this tick.
/// Phase: 4. Tick: SLOW.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PowerProducerComponent) },
    writes: new[] { typeof(PowerConsumerComponent) },
    bus:    nameof(IGameServices.Inventory)
)]
[TickRate(TickRates.SLOW)]
public sealed class ElectricGridSystem : SystemBase
{
    private readonly List<(EntityId Entity, PowerConsumerComponent Consumer)> _consumers = new();

    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        float totalSupply = 0f;
        foreach (var producerId in Query<PowerProducerComponent>())
        {
            var producer = GetComponent<PowerProducerComponent>(producerId);
            if (!producer.IsActive) continue;
            totalSupply += producer.CurrentWatts;
        }

        _consumers.Clear();
        float totalDemand = 0f;
        foreach (var consumerId in Query<PowerConsumerComponent>())
        {
            var consumer = GetComponent<PowerConsumerComponent>(consumerId);
            _consumers.Add((consumerId, consumer));
            totalDemand += consumer.RequiredWatts;
        }

        _consumers.Sort(static (a, b) => b.Consumer.Priority.CompareTo(a.Consumer.Priority));

        float remaining      = totalSupply;
        int   unpoweredCount = 0;
        foreach (var (entity, consumer) in _consumers)
        {
            if (remaining >= consumer.RequiredWatts)
            {
                consumer.IsPowered = true;
                remaining -= consumer.RequiredWatts;
                SetComponent(entity, consumer);

                Services.Inventory.Publish(new PowerGrantedEvent
                {
                    ConsumerId   = entity,
                    WattsGranted = consumer.RequiredWatts
                });
            }
            else
            {
                consumer.IsPowered = false;
                SetComponent(entity, consumer);
                unpoweredCount++;
            }
        }

        if (unpoweredCount > 0)
        {
            Services.Inventory.Publish(new GridOverloadEvent
            {
                TotalDemand    = totalDemand,
                TotalSupply    = totalSupply,
                UnpoweredCount = unpoweredCount
            });
        }
    }
}
