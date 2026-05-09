using System;
using System.Collections.Generic;
using DualFrontier.Components.Building;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Events.Power;
using DualFrontier.Systems.Power;
using FluentAssertions;
using Xunit;

namespace DualFrontier.IntegrationTests.PowerDomain;

/// <summary>
/// Closes backlog item #6 + Phase 4 acceptance criterion "Converter:
/// 100W → 30 mana and back; works pointwise". Verifies that ConverterSystem
/// is now registered alongside ElectricGridSystem and emits the expected
/// 30%-efficient <see cref="ConverterPowerOutputEvent"/> when its consumer
/// side is powered. The cycle is broken via deferred event coupling so the
/// scheduler accepts the combined graph.
/// </summary>
public sealed class ConverterEfficiencyTests : IDisposable
{
    public ConverterEfficiencyTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void Converter_PoweredAt100W_Emits30W_OnNextTick()
    {
        var world = new World();
        var services = new GameServices();
        var ticks = new TickScheduler();

        EntityId generator = world.CreateEntity();
        world.AddComponent(generator, new PowerProducerComponent
        {
            MaxWatts     = 200f,
            CurrentWatts = 200f,
            IsActive     = true
        });

        // Converter entity carries BOTH consumer and producer markers.
        EntityId converter = world.CreateEntity();
        world.AddComponent(converter, new PowerConsumerComponent { RequiredWatts = 100f, Priority = 5 });
        world.AddComponent(converter, new PowerProducerComponent { MaxWatts = 50f, CurrentWatts = 0f, IsActive = true });

        var outputs = new List<ConverterPowerOutputEvent>();
        services.Power.Subscribe<ConverterPowerOutputEvent>(e => outputs.Add(e));

        var graph = new DependencyGraph();
        graph.AddSystem(new ElectricGridSystem());
        graph.AddSystem(new ConverterSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services);

        scheduler.ExecuteTick(1f / 30f);

        outputs.Should().HaveCount(1, "powered converter publishes one output per SLOW tick");
        outputs[0].ConverterId.Should().Be(converter);
        outputs[0].WattsOutput.Should().BeApproximately(30f, 0.001f,
            "GDD 9 — converter efficiency is 30%; 100W input → 30W output");
    }

    [Fact]
    public void Converter_NotPowered_DoesNotEmitOutput()
    {
        var world = new World();
        var services = new GameServices();
        var ticks = new TickScheduler();

        // No generator at all — converter cannot get power.
        EntityId converter = world.CreateEntity();
        world.AddComponent(converter, new PowerConsumerComponent { RequiredWatts = 100f, Priority = 5 });
        world.AddComponent(converter, new PowerProducerComponent { MaxWatts = 50f, CurrentWatts = 0f, IsActive = true });

        var outputs = new List<ConverterPowerOutputEvent>();
        services.Power.Subscribe<ConverterPowerOutputEvent>(e => outputs.Add(e));

        var graph = new DependencyGraph();
        graph.AddSystem(new ElectricGridSystem());
        graph.AddSystem(new ConverterSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services);

        scheduler.ExecuteTick(1f / 30f);

        outputs.Should().BeEmpty(
            "an unpowered converter must not emit ConverterPowerOutputEvent");
    }
}
