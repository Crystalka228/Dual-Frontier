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
/// Closes the Phase 4 acceptance criterion "Electric grid: generator → wire
/// → consumer; overload on demand spike" (ROADMAP §"✅ Phase 4"). One
/// generator, three consumers of mixed priority where total demand exceeds
/// supply: high-priority consumers must stay powered, low-priority must be
/// switched off, <see cref="GridOverloadEvent"/> must fire with the correct
/// unpowered count.
/// </summary>
public sealed class ElectricGridOverloadTests : IDisposable
{
    public ElectricGridOverloadTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void Overload_HighPriorityWins_LowPriorityCutOff_OverloadEventFires()
    {
        var world = new World();
        var services = new GameServices();
        var ticks = new TickScheduler();

        EntityId producer = world.CreateEntity();
        world.AddComponent(producer, new PowerProducerComponent
        {
            MaxWatts     = 50f,
            CurrentWatts = 50f,
            IsActive     = true
        });

        EntityId high = world.CreateEntity();
        world.AddComponent(high, new PowerConsumerComponent { RequiredWatts = 20f, Priority = 3 });

        EntityId mid = world.CreateEntity();
        world.AddComponent(mid, new PowerConsumerComponent { RequiredWatts = 20f, Priority = 2 });

        EntityId low = world.CreateEntity();
        world.AddComponent(low, new PowerConsumerComponent { RequiredWatts = 20f, Priority = 1 });

        var grants = new List<PowerGrantedEvent>();
        var overloads = new List<GridOverloadEvent>();
        services.Power.Subscribe<PowerGrantedEvent>(e => grants.Add(e));
        services.Power.Subscribe<GridOverloadEvent>(e => overloads.Add(e));

        var graph = new DependencyGraph();
        graph.AddSystem(new ElectricGridSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<PowerConsumerComponent>(high, out var highC).Should().BeTrue();
        world.TryGetComponent<PowerConsumerComponent>(mid, out var midC).Should().BeTrue();
        world.TryGetComponent<PowerConsumerComponent>(low, out var lowC).Should().BeTrue();

        highC.IsPowered.Should().BeTrue();
        midC.IsPowered.Should().BeTrue();
        lowC.IsPowered.Should().BeFalse(
            "demand 60W > supply 50W; lowest priority consumer is the one cut off");

        grants.Should().HaveCount(2);
        overloads.Should().HaveCount(1);
        overloads[0].UnpoweredCount.Should().Be(1);
        overloads[0].TotalDemand.Should().Be(60f);
        overloads[0].TotalSupply.Should().Be(50f);
    }
}
