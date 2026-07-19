using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Contracts.Modding;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Contracts.Services;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using AwesomeAssertions;
using Xunit;
using TickRates = DualFrontier.Contracts.Attributes.TickRates;

namespace DualFrontier.Modding.Tests.Sdk;

// ---- W1 SDK test doubles (Contracts-only, tests-local) ----

internal struct SdkTestComponent : IComponent
{
    public int Value;
}

[EventBus("Combat")]
public sealed record SdkTestEvent(int Value) : IEvent;

[SystemAccess(reads: new Type[0], writes: new Type[0], bus: nameof(IGameServices.World))]
[TickRate(TickRates.REALTIME)]
public sealed class FaultingSdkSystem : ISimulationSystem
{
    public void Initialize(ISystemContext context) { }
    public void Tick(ISystemContext context) => throw new InvalidOperationException("deliberate SDK fault");
    public void OnDispose() { }
}

[SystemAccess(reads: new Type[0], writes: new Type[0], bus: nameof(IGameServices.World))]
[TickRate(TickRates.NORMAL)]
public sealed class SdkStubSystemA : SystemBase
{
    public override void Update(float delta) { }
}

[SystemAccess(reads: new Type[0], writes: new Type[0], bus: nameof(IGameServices.World))]
[TickRate(TickRates.NORMAL)]
public sealed class SdkStubSystemB : SystemBase
{
    public override void Update(float delta) { }
}

internal sealed class StubPathfinding : IPathfindingService
{
    public bool TryFindPath(GridVector from, GridVector to, out IReadOnlyList<GridVector> path)
    {
        path = Array.Empty<GridVector>();
        return false;
    }
}

/// <summary>
/// W1 C5 — behavioural proofs of the SDK surface: per-tick freshness, the
/// Contracts-safe access forms against a live world, both factory registration
/// paths, capability-gated events routed through the live gate, and adapter
/// fault-route parity (a mod ISimulationSystem's faulted Tick is contained by D2
/// exactly as a SystemBase mod system).
/// </summary>
public sealed class SdkContextTests
{
    // ---- Per-tick freshness ----

    [Fact]
    public void CurrentTick_ReadsTheLiveSource_NotACachedValue()
    {
        long tick = 0;
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => tick);

        view.CurrentTick.Should().Be(0);
        tick = 42;
        view.CurrentTick.Should().Be(42, "the context reads SimTick live each call, so it is fresh per tick");
    }

    // ---- Access-form semantics against a live world ----

    [Fact]
    public void AccessForms_DelegateToTheLiveWorld_PreservingSemantics()
    {
        using var world = new NativeWorld();
        EntityId e0 = world.CreateEntity();
        world.AddComponent(e0, new SdkTestComponent { Value = 10 });
        EntityId e1 = world.CreateEntity();
        world.AddComponent(e1, new SdkTestComponent { Value = 20 });

        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        var ctx = new SystemExecutionContext(
            "T", SystemOrigin.Mod, "test.mod", new NullModFaultSink(), world);

        SystemExecutionContext.PushContext(ctx);
        try
        {
            // per-id
            view.HasComponent<SdkTestComponent>(e0).Should().BeTrue();
            view.TryGetComponent<SdkTestComponent>(e0, out SdkTestComponent c0).Should().BeTrue();
            c0.Value.Should().Be(10);
            view.GetComponent<SdkTestComponent>(e1).Value.Should().Be(20);

            // span-read (+ Pairs)
            int sum = 0;
            int count;
            using (SpanScope<SdkTestComponent> span = view.AcquireSpan<SdkTestComponent>())
            {
                count = span.Count;
                foreach ((EntityId _, SdkTestComponent comp) in span.Pairs)
                    sum += comp.Value;
            }
            count.Should().Be(2);
            sum.Should().Be(30);

            // batch-write (commit on scope exit)
            using (WriteScope<SdkTestComponent> batch = view.BeginBatch<SdkTestComponent>())
            {
                batch.Update(e0, new SdkTestComponent { Value = 99 }).Should().BeTrue();
            }
            view.GetComponent<SdkTestComponent>(e0).Value.Should().Be(99, "the batch flushed on scope exit");

            // intern / resolve
            StringHandle handle = view.InternString("hello");
            handle.IsEmpty.Should().BeFalse();
            view.Resolve(handle).Should().Be("hello");

            // composite create + use
            CompositeHandle<int> composite = view.CreateComposite<int>();
            composite.IsValid.Should().BeTrue();
            view.CompositeAdd(composite, e0, 7).Should().BeTrue();
            view.CompositeCountFor(composite, e0).Should().Be(1);
            view.CompositeTryGetAt(composite, e0, 0, out int got).Should().BeTrue();
            got.Should().Be(7);
            view.CompositeClearFor(composite, e0).Should().BeTrue();
            view.CompositeCountFor(composite, e0).Should().Be(0);
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }
    }

    [Fact]
    public void AccessForm_OutsideAContext_FailsLoudly()
    {
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        Action act = () => view.HasComponent<SdkTestComponent>(new EntityId(1, 1));
        act.Should().Throw<InvalidOperationException>("world access outside an active scheduler context must fail loudly");
    }

    // ---- Factory registration: both paths ----

    [Fact]
    public void RegisterSystem_FactoryAndParameterless_BothRegisterCore()
    {
        var registry = new ModRegistry();
        registry.SetSystemServices(new SystemServices(new StubPathfinding()));

        registry.RegisterSystem<SdkStubSystemA>(_ => new SdkStubSystemA());  // factory delegate
        registry.RegisterSystem<SdkStubSystemB>();                            // parameterless convenience

        registry.GetCoreSystemInstances().Should().HaveCount(2);
        registry.GetCoreSystemInstances().Select(s => s.GetType())
            .Should().BeEquivalentTo(new[] { typeof(SdkStubSystemA), typeof(SdkStubSystemB) });
    }

    // ---- Capability-gated events routed through the live gate ----

    [Fact]
    public void ContextPublish_UndeclaredEvent_ThrowsCapabilityViolationLoudly()
    {
        var registry = new ModRegistry();
        RegisterModApi(registry, subscribeOnly: true);   // declares subscribe but NOT publish
        var view = new SystemContextView(registry, "test.mod", () => 0L);

        Action act = () => view.Publish(new SdkTestEvent(1));

        act.Should().Throw<CapabilityViolationException>("the SDK context routes Publish through the live capability gate")
            .Which.Message.Should().Contain("kernel.publish:");
    }

    [Fact]
    public void ContextPublish_DeclaredEvent_IsAdmitted()
    {
        var registry = new ModRegistry();
        RegisterModApi(registry, subscribeOnly: false);  // declares publish
        var view = new SystemContextView(registry, "test.mod", () => 0L);

        Action act = () => view.Publish(new SdkTestEvent(1));

        act.Should().NotThrow();
    }

    // ---- Adapter fault-route parity (D2) ----

    [Fact]
    public void FaultedSdkTick_IsContainedByD2_AndQuarantinesTheMod()
    {
        var registry = new ModRegistry();
        registry.RegisterSystem("test.mod", typeof(FaultingSdkSystem));  // widened path -> SystemAdapter

        SystemBase adapter = registry.GetAllSystems()
            .Single(r => r.Origin == SystemOrigin.Mod).Instance;
        IReadOnlyDictionary<SystemBase, SystemMetadata> metadata = SystemMetadataBuilder.Build(registry);

        using var world = new NativeWorld();
        var graph = new DependencyGraph();
        graph.AddSystem(adapter);
        graph.Build();
        var ticks = new TickScheduler();
        ParallelSystemScheduler scheduler = SchedulerTestFixture.BuildIsolated(
            graph.GetPhases(), ticks, world, new ModFaultHandler(), systemMetadata: metadata);

        string? quarantined = null;
        scheduler.OnModQuarantined += (modId, _) => quarantined = modId;

        Action act = () => scheduler.ExecuteTick(1f);

        act.Should().NotThrow("a mod-origin fault is CONTAINED by the D2 route, never rethrown");
        quarantined.Should().Be("test.mod",
            "the adapter is a SystemBase, so a faulted ISimulationSystem Tick routes through the scheduler's " +
            "existing D2 catch exactly as a SystemBase mod system — the mod is quarantined, not fatal");
    }

    private static void RegisterModApi(ModRegistry registry, bool subscribeOnly)
    {
        string fqn = typeof(SdkTestEvent).FullName!;
        string[] tokens = subscribeOnly
            ? new[] { $"kernel.subscribe:{fqn}" }
            : new[] { $"kernel.publish:{fqn}" };
        var manifest = new ModManifest { Id = "test.mod", Capabilities = ManifestCapabilities.Parse(tokens, null) };
        var api = new RestrictedModApi(
            "test.mod",
            manifest,
            registry,
            new ModContractStore(),
            new GameServices(),
            KernelCapabilityRegistry.BuildFromKernelAssemblies());
        registry.RegisterRestrictedModApi("test.mod", api);
    }
}
