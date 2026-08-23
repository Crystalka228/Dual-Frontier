using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Application.Modding;
using DualFrontier.Core.Modding;
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

public sealed record SdkTestEvent(int Value) : IEvent;

[SystemAccess(reads: new Type[0], writes: new Type[0])]
[TickRate(TickRates.REALTIME)]
public sealed class FaultingSdkSystem : ISimulationSystem
{
    public void Initialize(ISystemContext context) { }
    public void Tick(ISystemContext context) => throw new InvalidOperationException("deliberate SDK fault");
    public void OnDispose() { }
}

[SystemAccess(reads: new Type[0], writes: new Type[0])]
[TickRate(TickRates.NORMAL)]
public sealed class SdkStubSystemA : SystemBase
{
    public override void Update(float delta) { }
}

[SystemAccess(reads: new Type[0], writes: new Type[0])]
[TickRate(TickRates.NORMAL)]
public sealed class SdkStubSystemB : SystemBase
{
    public override void Update(float delta) { }
}

/// <summary>
/// W3/G2 test double for the engine-internal presentation seam: records what a mod asked
/// for so a test can assert presentation WITHOUT standing up a Vulkan renderer.
/// </summary>
internal sealed class RecordingPresentationSink : IPresentationSink
{
    public List<(float R, float G, float B, float Strength)> Calls { get; } = new();

    public void SetAmbientTint(float r, float g, float b, float strength)
        => Calls.Add((r, g, b, strength));
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

    // ---- W3 C2: entity lifecycle on the SDK surface (Contracts 2.1.0) ----

    [Fact]
    public void EntityLifecycle_MintAttachRead_RoundTripsEntirelyThroughTheSdk()
    {
        using var world = new NativeWorld();
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        var ctx = new SystemExecutionContext(
            "T", SystemOrigin.Mod, "test.mod", new NullModFaultSink(), world);

        SystemExecutionContext.PushContext(ctx);
        try
        {
            EntityId minted = view.CreateEntity();
            view.IsEntityAlive(minted).Should().BeTrue("a freshly minted entity is live");

            using (WriteScope<SdkTestComponent> batch = view.BeginBatch<SdkTestComponent>())
            {
                batch.Add(minted, new SdkTestComponent { Value = 5 }).Should().BeTrue();
            }

            view.GetComponent<SdkTestComponent>(minted).Value.Should().Be(5,
                "G1 closed: a mod can mint an entity AND attach its own component without " +
                "ever naming an engine assembly — before W3 there was no way to obtain an id");
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }
    }

    [Fact]
    public void DestroyEntity_EndsLivenessAtOnce_AndReclaimsStorageOnTheEngineFlush()
    {
        using var world = new NativeWorld();
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        var ctx = new SystemExecutionContext(
            "T", SystemOrigin.Mod, "test.mod", new NullModFaultSink(), world);

        EntityId doomed;
        SystemExecutionContext.PushContext(ctx);
        try
        {
            doomed = view.CreateEntity();
            using (WriteScope<SdkTestComponent> batch = view.BeginBatch<SdkTestComponent>())
            {
                batch.Add(doomed, new SdkTestComponent { Value = 7 }).Should().BeTrue();
            }

            view.DestroyEntity(doomed);

            view.IsEntityAlive(doomed).Should().BeFalse(
                "liveness flips immediately — it is the component STORAGE reclamation that the " +
                "engine defers to its flush (NativeWorldTests pins the same split)");
            world.GetComponentCount<SdkTestComponent>().Should().Be(1, "pre-flush: the row survives");
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }

        // The flush is the engine's to schedule; the SDK deliberately vends no flush member.
        world.FlushDestroyedEntities();
        world.GetComponentCount<SdkTestComponent>().Should().Be(0, "post-flush: the row is reclaimed");
    }

    [Fact]
    public void DestroyEntity_WhileASpanIsLive_FailsLoudly_InsteadOfBeingSilentlyRejected()
    {
        // PR #49 Codex review (P1). The native side silently rejects a destroy while the world is
        // borrowed. Silent rejection against a contract that promises immediate liveness loss is
        // the fail-open shape; the SDK turns it into a diagnostic.
        using var world = new NativeWorld();
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        var ctx = new SystemExecutionContext(
            "T", SystemOrigin.Mod, "test.mod", new NullModFaultSink(), world);

        SystemExecutionContext.PushContext(ctx);
        try
        {
            EntityId doomed = view.CreateEntity();

            using (SpanScope<SdkTestComponent> span = view.AcquireSpan<SdkTestComponent>())
            {
                Action act = () => view.DestroyEntity(doomed);

                act.Should().Throw<InvalidOperationException>(
                        "a destroy under a live span would be silently dropped by the native side")
                    .Which.Message.Should().Contain("span");
            }

            // With the borrow released, the same call works and liveness ends at once.
            view.DestroyEntity(doomed);
            view.IsEntityAlive(doomed).Should().BeFalse();
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }
    }

    [Fact]
    public void EntityLifecycle_OutsideAContext_FailsLoudly()
    {
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        var stale = new EntityId(1, 1);

        ((Action)(() => view.CreateEntity())).Should()
            .Throw<InvalidOperationException>("minting is world access, and world access outside a scheduler context is loud");
        ((Action)(() => view.DestroyEntity(stale))).Should().Throw<InvalidOperationException>();
        ((Action)(() => view.IsEntityAlive(stale))).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ContractsVersion_IsMinorBumped_AndStillSatisfiesCaret2_0_0_Manifests()
    {
        ContractsVersion.Current.Should().Be(new ContractsVersion(2, 1, 1),
            "W3's 2.1.0 added SDK members without removing or reshaping any (MINOR); ID-B " +
            "then corrected what flows THROUGH those members — SpanScope.Pairs yields true " +
            "versions and EntityId.IsValid is Index > 0 — with no surface change at all (PATCH)");

        VersionConstraint.Parse("^2.0.0").IsSatisfiedBy(ContractsVersion.Current).Should().BeTrue(
            "every on-disk manifest pins apiVersion ^2.0.0; neither bump must strand them");
    }

    // ---- W3 C3: the presentation primitive (SDK 2.1.0) ----

    [Fact]
    public void SetAmbientTint_RoutesToTheInstalledPresentationSink()
    {
        var registry = new ModRegistry();
        var sink = new RecordingPresentationSink();
        registry.SetPresentationSink(sink);
        var view = new SystemContextView(registry, "test.mod", () => 0L);

        view.SetAmbientTint(0.2f, 0.3f, 0.9f, 0.5f);

        sink.Calls.Should().ContainSingle().Which.Should().Be((0.2f, 0.3f, 0.9f, 0.5f));
    }

    [Fact]
    public void SetAmbientTint_WithNoSinkInstalled_ThrowsLoudly_NeverSilentlyNoOps()
    {
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);

        Action act = () => view.SetAmbientTint(1f, 0f, 0f, 1f);

        act.Should().Throw<InvalidOperationException>(
                "fail-open is the forbidden shape here (K-L19): a presentation call on a host with " +
                "no sink must diagnose, never vanish and leave the author hunting for missing visuals")
            .Which.Message.Should().Contain("presentation sink");
    }

    [Fact]
    public void SetAmbientTint_NeedsNoWorldContext_SoItIsSafeFromAnEventHandler()
    {
        // The sink route never touches SystemExecutionContext.Current — unlike every
        // component member — so presentation is callable from a subscriber handler
        // regardless of whether the handler was wrapped in a captured scheduler context.
        var registry = new ModRegistry();
        var sink = new RecordingPresentationSink();
        registry.SetPresentationSink(sink);
        var view = new SystemContextView(registry, "test.mod", () => 0L);

        SystemExecutionContext.Current.Should().BeNull("no context is pushed in this test");
        Action act = () => view.SetAmbientTint(0f, 0f, 1f, 1f);

        act.Should().NotThrow();
        sink.Calls.Should().ContainSingle();
    }

    [Fact]
    public void BridgePresentationSink_TranslatesTheCallIntoAnEnqueuedRenderCommand()
    {
        var bridge = new PresentationBridge();
        var sink = new BridgePresentationSink(bridge);

        sink.SetAmbientTint(0.1f, 0.2f, 0.3f, 0.4f);

        bridge.QueueDepth.Should().Be(1);

        var drained = new List<IRenderCommand>();
        bridge.DrainCommands(drained.Add);

        drained.Should().ContainSingle().Which.Should()
            .BeOfType<AmbientTintCommand>()
            .Which.Should().Be(new AmbientTintCommand(0.1f, 0.2f, 0.3f, 0.4f));
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
            new KernelCapabilityRegistry());
        registry.RegisterRestrictedModApi("test.mod", api);
    }
}
