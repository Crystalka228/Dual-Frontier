using System;
using System.Collections.Generic;
using DualFrontier.Components.Building;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Power;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Phase 4 electric grid: each SLOW tick sums producer output (real
/// <see cref="PowerProducerComponent"/> entities + the previous tick's
/// converter-output events accumulated via <see cref="OnConverterOutput"/>),
/// sorts consumers by priority (highest first), and allocates supply until
/// exhausted. Consumers above the supply line stay powered; the rest are
/// switched off. Publishes <see cref="PowerGrantedEvent"/> per powered consumer
/// and <see cref="GridOverloadEvent"/> when any consumer is unpowered this tick.
/// <para>
/// Converter output flows through the bus, not the component graph: that
/// breaks the ElectricGrid↔Converter cycle that the dependency graph would
/// otherwise reject. See <see cref="ConverterPowerOutputEvent"/>.
/// </para>
/// Phase: 4. Tick: SLOW.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PowerProducerComponent) },
    writes: new[] { typeof(PowerConsumerComponent) },
    bus:    nameof(IGameServices.Power)
)]
[TickRate(TickRates.SLOW)]
public sealed class ElectricGridSystem : SystemBase
{
    private readonly List<(EntityId Entity, PowerConsumerComponent Consumer)> _consumers = new();
    private float _converterSupplyAccumulator;

    /// <summary>
    /// Subscribes to <see cref="ConverterPowerOutputEvent"/> on the
    /// <see cref="IGameServices.Power"/> bus. Converter outputs from the
    /// previous tick are summed in <see cref="_converterSupplyAccumulator"/>
    /// and folded into the supply at the start of the next <see cref="Update"/>.
    /// </summary>
    protected override void OnInitialize()
    {
        Services.Power.Subscribe<ConverterPowerOutputEvent>(OnConverterOutput);
    }

    private void OnConverterOutput(ConverterPowerOutputEvent e)
    {
        _converterSupplyAccumulator += e.WattsOutput;
    }

    public override void Update(float delta)
    {
        float totalSupply = _converterSupplyAccumulator;
        _converterSupplyAccumulator = 0f;

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
        // K8.3+K8.4 Phase 4 — single batch wraps all consumer flip writes;
        // legacy SetComponent mirrors for dual-write (removed Phase 5 commit 21).
        using var batch = NativeWorld.BeginBatch<PowerConsumerComponent>();
        foreach (var pair in _consumers)
        {
            var entity   = pair.Entity;
            var consumer = pair.Consumer;
            if (remaining >= consumer.RequiredWatts)
            {
                consumer.IsPowered = true;
                remaining -= consumer.RequiredWatts;
                batch.Update(entity, consumer);
                SetComponent(entity, consumer);

                Services.Power.Publish(new PowerGrantedEvent
                {
                    ConsumerId   = entity,
                    WattsGranted = consumer.RequiredWatts
                });
            }
            else
            {
                consumer.IsPowered = false;
                batch.Update(entity, consumer);
                SetComponent(entity, consumer);
                unpoweredCount++;
            }
        }

        if (unpoweredCount > 0)
        {
            Services.Power.Publish(new GridOverloadEvent
            {
                TotalDemand    = totalDemand,
                TotalSupply    = totalSupply,
                UnpoweredCount = unpoweredCount
            });
        }
    }
}
