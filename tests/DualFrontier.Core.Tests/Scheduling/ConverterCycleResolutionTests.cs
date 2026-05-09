using System;
using System.Collections.Generic;
using DualFrontier.Components.Building;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Systems.Power;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

/// <summary>
/// Closes Q6: <c>ElectricGridSystem</c> + <c>ConverterSystem</c> registered
/// together must build a clean DAG. The cycle by components
/// (PowerConsumer/PowerProducer) is broken by routing converter output
/// through a deferred event on <c>IPowerBus</c> instead of through component
/// writes; <c>DependencyGraph.Build</c> must therefore succeed without a
/// <c>Cyclic dependency</c> diagnostic.
/// </summary>
public sealed class ConverterCycleResolutionTests : IDisposable
{
    public ConverterCycleResolutionTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void Build_ElectricGridAndConverter_Together_DoesNotThrowCycle()
    {
        var graph = new DependencyGraph();
        graph.AddSystem(new ElectricGridSystem());
        graph.AddSystem(new ConverterSystem());

        Action act = () => graph.Build();

        act.Should().NotThrow();
        graph.GetPhases().Should().NotBeEmpty();
    }

    [Fact]
    public void Build_ElectricGridAndConverter_PlacedInOrderedPhases()
    {
        var graph = new DependencyGraph();
        var grid = new ElectricGridSystem();
        var converter = new ConverterSystem();
        graph.AddSystem(grid);
        graph.AddSystem(converter);

        graph.Build();

        var phases = graph.GetPhases();
        phases.Should().HaveCountGreaterThanOrEqualTo(1);

        int gridPhase = -1, converterPhase = -1;
        for (int i = 0; i < phases.Count; i++)
        {
            foreach (var s in phases[i].Systems)
            {
                if (ReferenceEquals(s, grid)) gridPhase = i;
                if (ReferenceEquals(s, converter)) converterPhase = i;
            }
        }

        gridPhase.Should().BeGreaterThanOrEqualTo(0);
        converterPhase.Should().BeGreaterThanOrEqualTo(0);
        // Edge ElectricGrid (writes PowerConsumer) → Converter (reads PowerConsumer)
        // means Converter must run after ElectricGrid in the same tick.
        converterPhase.Should().BeGreaterThan(
            gridPhase,
            "Converter reads PowerConsumerComponent that ElectricGrid writes — Converter must run after ElectricGrid");
    }

    [Fact]
    public void Scheduler_RunsBothSystems_WithoutIsolationViolation()
    {
        var world = new World();
        var services = new GameServices();
        var ticks = new TickScheduler();

        EntityId converter = world.CreateEntity();
        world.AddComponent(converter, new PowerConsumerComponent { RequiredWatts = 100f, IsPowered = false, Priority = 1 });
        world.AddComponent(converter, new PowerProducerComponent { MaxWatts = 50f, CurrentWatts = 0f, IsActive = true });

        EntityId generator = world.CreateEntity();
        world.AddComponent(generator, new PowerProducerComponent { MaxWatts = 200f, CurrentWatts = 200f, IsActive = true });

        var graph = new DependencyGraph();
        graph.AddSystem(new ElectricGridSystem());
        graph.AddSystem(new ConverterSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services);

        Action act = () => scheduler.ExecuteTick(0.016f);

        act.Should().NotThrow(
            "deferred ConverterPowerOutputEvent must drain at the phase boundary "
            + "without breaking either system's [SystemAccess] declaration");
    }
}
