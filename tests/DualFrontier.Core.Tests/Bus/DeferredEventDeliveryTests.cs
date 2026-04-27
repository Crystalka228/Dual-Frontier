using System;
using System.Collections.Generic;
using System.Threading;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Bus;

/// <summary>
/// Closes backlog item #1: <c>[Deferred]</c> events must NOT be delivered synchronously
/// from <c>Publish</c>; they must be queued and dispatched at the next phase
/// boundary by <c>ParallelSystemScheduler</c> via <see cref="IDeferredFlush"/>.
/// </summary>
public sealed class DeferredEventDeliveryTests : IDisposable
{
    public DeferredEventDeliveryTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void Deferred_Publish_DoesNotInvokeHandlerSynchronously()
    {
        var services = new GameServices();
        int count = 0;
        services.Pawns.Subscribe<DeferredTestEvent>(_ => count++);

        services.Pawns.Publish(new DeferredTestEvent());

        count.Should().Be(0, "deferred events must not run inside Publish");
    }

    [Fact]
    public void Deferred_Publish_DeliveredAfterPhaseBoundary()
    {
        var services = new GameServices();
        var observed = new List<string>();
        services.Pawns.Subscribe<DeferredTestEvent>(e => observed.Add(e.Tag));

        services.Pawns.Publish(new DeferredTestEvent { Tag = "phase-1" });
        ((IDeferredFlush)services).FlushDeferred();

        observed.Should().Equal("phase-1");
    }

    [Fact]
    public void Deferred_PublishedDuringHandler_DeliveredOnNextFlush_NotCurrent()
    {
        var services = new GameServices();
        var observed = new List<string>();
        bool reentryFired = false;
        services.Pawns.Subscribe<DeferredTestEvent>(e =>
        {
            observed.Add(e.Tag);
            if (e.Tag == "first")
            {
                services.Pawns.Publish(new DeferredTestEvent { Tag = "reentry" });
            }
        });
        services.Pawns.Subscribe<DeferredTestEvent>(_ => reentryFired = true);

        services.Pawns.Publish(new DeferredTestEvent { Tag = "first" });
        ((IDeferredFlush)services).FlushDeferred();

        observed.Should().Equal("first");
        reentryFired.Should().BeTrue();

        ((IDeferredFlush)services).FlushDeferred();

        observed.Should().Equal("first", "reentry");
    }

    [Fact]
    public void Deferred_Drained_PerPhase_BySchedulerExecutePhase()
    {
        var world = new World();
        var services = new GameServices();
        var ticks = new TickScheduler();
        var emitter = new DeferredEmitterSystem();
        var listener = new DeferredListenerSystem();

        var graph = new DependencyGraph();
        graph.AddSystem(emitter);
        graph.AddSystem(listener);
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);

        scheduler.ExecuteTick(0.016f);

        listener.Received.Should().Be(1, "deferred event published in phase 1 must be drained at phase boundary and seen by listener");
    }

    [Fact]
    public void Synchronous_Event_StillDeliveredImmediately()
    {
        var services = new GameServices();
        int count = 0;
        services.Pawns.Subscribe<PlainTestEvent>(_ => count++);

        services.Pawns.Publish(new PlainTestEvent());

        count.Should().Be(1, "events without [Deferred]/[Immediate] keep synchronous semantics");
    }

    [Fact]
    public void Deferred_HandlerRunsWith_CapturedSystemExecutionContext()
    {
        var world = new World();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new DeferredMutableComponent { Value = 1 });

        var services = new GameServices();
        var ticks = new TickScheduler();
        var publisher = new DeferredPublisherSystem(entity);
        var mutator = new DeferredMutatorSystem();

        var graph = new DependencyGraph();
        graph.AddSystem(publisher);
        graph.AddSystem(mutator);
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);

        Action act = () => scheduler.ExecuteTick(0.016f);
        act.Should().NotThrow(
            "deferred handlers must run with the subscribing system's captured context, "
            + "letting them mutate components their declared writes cover");

        world.TryGetComponent<DeferredMutableComponent>(entity, out var comp).Should().BeTrue();
        comp.Value.Should().Be(42);
    }

    // ── Test fixture types ──────────────────────────────────────────────────

    [Deferred]
    internal sealed record DeferredTestEvent : IEvent
    {
        public string Tag { get; init; } = "";
    }

    internal sealed record PlainTestEvent : IEvent;

    internal sealed record DeferredMutateRequest(EntityId Target, int NewValue) : IEvent;

    [Deferred]
    internal sealed record DeferredMutateRequestDeferred(EntityId Target, int NewValue) : IEvent;

    internal sealed class DeferredMutableComponent : IComponent
    {
        public int Value;
    }

    internal sealed class EmitterMarker : IComponent { }
    internal sealed class ListenerMarker : IComponent { }
    internal sealed class PublisherMarker : IComponent { }
    internal sealed class MutatorMarker : IComponent { }

    // ── Test fixture systems ────────────────────────────────────────────────

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(EmitterMarker) }, bus: nameof(DualFrontier.Contracts.Bus.IGameServices.Pawns))]
    internal sealed class DeferredEmitterSystem : SystemBase
    {
        public override void Update(float delta)
        {
            Services.Pawns.Publish(new DeferredTestEvent { Tag = "emit" });
        }
    }

    [SystemAccess(reads: new[] { typeof(EmitterMarker) }, writes: new[] { typeof(ListenerMarker) }, bus: nameof(DualFrontier.Contracts.Bus.IGameServices.Pawns))]
    internal sealed class DeferredListenerSystem : SystemBase
    {
        public int Received;

        protected override void OnInitialize()
        {
            Services.Pawns.Subscribe<DeferredTestEvent>(_ => Received++);
        }

        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(PublisherMarker) }, bus: nameof(DualFrontier.Contracts.Bus.IGameServices.Pawns))]
    internal sealed class DeferredPublisherSystem : SystemBase
    {
        private readonly EntityId _target;
        public DeferredPublisherSystem(EntityId target) { _target = target; }

        public override void Update(float delta)
        {
            Services.Pawns.Publish(new DeferredMutateRequestDeferred(_target, 42));
        }
    }

    [SystemAccess(reads: new[] { typeof(PublisherMarker) }, writes: new[] { typeof(DeferredMutableComponent) }, bus: nameof(DualFrontier.Contracts.Bus.IGameServices.Pawns))]
    internal sealed class DeferredMutatorSystem : SystemBase
    {
        protected override void OnInitialize()
        {
            Services.Pawns.Subscribe<DeferredMutateRequestDeferred>(OnRequest);
        }

        public override void Update(float delta) { }

        private void OnRequest(DeferredMutateRequestDeferred req)
        {
            var c = GetComponent<DeferredMutableComponent>(req.Target);
            c.Value = req.NewValue;
            SetComponent(req.Target, c);
        }
    }
}
