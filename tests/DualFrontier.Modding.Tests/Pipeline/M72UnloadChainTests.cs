using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

[EventBus("Combat")]
public sealed record M72UnloadTestEvent(int Value) : IEvent;

public sealed record M72TestContract(string Marker) : IModContract;

/// <summary>
/// M7.2 — ALC unload chain steps 1–6 + best-effort failure semantics on
/// <see cref="ModIntegrationPipeline.UnloadMod"/>. Covers the per-mod
/// unload protocol from MOD_OS_ARCHITECTURE v1.4 §9.5 / §9.5.1:
/// <list type="bullet">
///   <item>Run-flag guard parity with M7.1's <see cref="ModIntegrationPipeline.UnloadAll"/>.</item>
///   <item>Idempotency for non-active mods.</item>
///   <item>Per-step verification: subscriptions dropped (1), contracts revoked
///   (2), systems removed from registry (3), scheduler rebuilt without the
///   mod's systems (4–5), <see cref="ModLoader.UnloadMod"/> invoked (6).</item>
///   <item>Best-effort failure discipline (§9.5.1): a step throwing emits a
///   <see cref="ValidationWarning"/> with <c>(modId, stepNumber)</c> and the
///   chain continues; the mod is removed from the active set regardless.</item>
///   <item>UnloadAll regression + refactor: bulk-unload semantics preserved,
///   M7.1 guard preserved, warnings accumulated across per-mod calls.</item>
/// </list>
/// Out of scope (M7.3): step 7 — <c>WeakReference</c> spin loop with GC pump
/// per v1.4 §9.5 step 7 — and the <c>ModUnloadTimeout</c> warning surface.
/// Those land in M7.3 alongside Phase 2's carried-debt unload tests.
/// </summary>
public sealed class M72UnloadChainTests
{
    private const string TestModId = "tests.regular.m72fullsurface";

    // 1
    [Fact]
    public void UnloadMod_WhenRunning_ThrowsInvalidOperationException_WithCanonicalMessage()
    {
        // §9.3 / §9.5 — calling UnloadMod while the simulation is running
        // must throw with the same exact message as UnloadAll's M7.1 guard.
        // Verbatim string locks the spec wording at assertion level so any
        // accidental paraphrase trips the test.
        var h = Harness.Build();
        h.Pipeline.Resume();

        Action act = () => h.Pipeline.UnloadMod("any.mod.id");

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("Pause the scheduler before unloading mods");
    }

    // 2
    [Fact]
    public void UnloadMod_OnNonActiveMod_ReturnsEmptyWarnings_NoThrow()
    {
        // Idempotency: unloading a mod the pipeline has never seen must
        // be a silent no-op returning an empty warning list, not an
        // exception. Mirrors the discipline that menu callers can re-issue
        // unload requests after a partial failure without special casing.
        var h = Harness.Build();

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod("tests.regular.nonexistent");

        warnings.Should().BeEmpty();
    }

    // 3
    [Fact]
    public void UnloadMod_DropsBusSubscriptions_Step1()
    {
        // §9.5 step 1 — RestrictedModApi.UnsubscribeAll must drop every
        // subscription the mod registered through IModApi.Subscribe.
        // Observable: after UnloadMod, publishing on the bus does NOT
        // invoke the mod's handler.
        var h = Harness.Build();
        var subscribingMod = new SubscribingMod();
        InjectAndApply(h, subscribingMod);

        h.Pipeline.UnloadMod(TestModId);

        h.Services.Combat.Publish(new M72UnloadTestEvent(42));
        subscribingMod.HandlerCallCount.Should().Be(0,
            "step 1 must remove the subscription from the bus before the mod is gone");
    }

    // 4
    [Fact]
    public void UnloadMod_RevokesContracts_Step2()
    {
        // §9.5 step 2 — IModContractStore.RevokeAll must drop every
        // contract the mod published via IModApi.PublishContract.
        // Observable: TryGet<TContract> returns false after UnloadMod.
        var h = Harness.Build();
        InjectAndApply(h, new ContractPublishingMod());

        h.ContractStore.TryGet<M72TestContract>(out _).Should().BeTrue(
            "the contract must be present immediately after Apply for the test to be meaningful");

        h.Pipeline.UnloadMod(TestModId);

        h.ContractStore.TryGet<M72TestContract>(out M72TestContract? contract).Should().BeFalse(
            "step 2 must revoke the mod's contract registrations");
        contract.Should().BeNull();
    }

    // 5
    [Fact]
    public void UnloadMod_RemovesSystemsFromRegistry_Step3()
    {
        // §9.5 step 3 — ModRegistry.RemoveMod must drop the mod's system
        // instances. Observable: after UnloadMod, no system in the
        // registry has Origin=Mod with this modId.
        var h = Harness.Build();
        InjectAndApply(h, new SystemRegisteringMod());

        h.Pipeline.UnloadMod(TestModId);

        foreach (SystemRegistration reg in h.Registry.GetAllSystems())
        {
            reg.ModId.Should().NotBe(TestModId,
                "step 3 must remove every system attributed to the unloaded mod");
        }
    }

    // 6
    [Fact]
    public void UnloadMod_RebuildsSchedulerWithoutModSystems_Step4_5()
    {
        // §9.5 steps 4–5 — graph rebuild + scheduler swap must drop the
        // mod's systems from the active phase list while preserving
        // kernel-origin systems.
        var h = Harness.Build(new CoreSurvivorSystem());
        InjectAndApply(h, new SystemRegisteringMod());

        SchedulerContains<M72TestSystem>(h.Scheduler).Should().BeTrue(
            "the mod's system must be in the scheduler immediately after Apply");
        SchedulerContains<CoreSurvivorSystem>(h.Scheduler).Should().BeTrue();

        h.Pipeline.UnloadMod(TestModId);

        SchedulerContains<M72TestSystem>(h.Scheduler).Should().BeFalse(
            "steps 4–5 must rebuild the scheduler without the mod's systems");
        SchedulerContains<CoreSurvivorSystem>(h.Scheduler).Should().BeTrue(
            "kernel-origin systems must survive a per-mod unload");
    }

    // 7
    [Fact]
    public void UnloadMod_CallsAlcUnload_Step6()
    {
        // §9.5 step 6 — ModLoader.UnloadMod must be invoked. Verified via
        // ModLoader.GetLoaded losing the modId. The actual ALC release
        // (WeakReference verification) is M7.3 territory; M7.2 stops at
        // ensuring the loader's UnloadMod is called.
        var h = Harness.Build();
        InjectAndApply(h, new ContractPublishingMod());

        h.Loader.GetLoaded().Should().Contain(TestModId);

        h.Pipeline.UnloadMod(TestModId);

        h.Loader.GetLoaded().Should().NotContain(TestModId,
            "step 6 must call ModLoader.UnloadMod which removes the mod from _loaded");
    }

    // 8
    [Fact]
    public void UnloadMod_Step2Throws_StepsContinueAndWarningCollected()
    {
        // §9.5.1 best-effort: when a single step throws, the chain
        // continues and a ValidationWarning is recorded. Step 2 is the
        // chosen injection point because IModContractStore is the only
        // pipeline collaborator behind an interface — sealed concrete
        // collaborators cannot be wrapped without changing production
        // surface (see M7.2 implementation note in ModIntegrationPipeline).
        var h = Harness.Build(throwingContractStore: true);
        InjectAndApply(h, new SystemRegisteringMod());

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(TestModId);

        warnings.Should().ContainSingle(w => w.ModId == TestModId);
        warnings[0].Message.Should().Contain("Unload step 2 failed");
        warnings[0].Message.Should().Contain("§9.5.1");

        // Step 1 effect — n/a (no subscribe in fixture). Step 3 effect:
        // registry no longer contains the mod's systems, proving the
        // chain continued past the step-2 throw.
        foreach (SystemRegistration reg in h.Registry.GetAllSystems())
            reg.ModId.Should().NotBe(TestModId,
                "step 3 must run even after step 2 throws (best-effort)");
        h.Loader.GetLoaded().Should().NotContain(TestModId,
            "step 6 must run even after step 2 throws (best-effort)");
    }

    // 9
    [Fact]
    public void UnloadMod_StepThrows_ModRemovedFromActiveSet()
    {
        // §9.5.1 invariant — "the mod is removed from the active set
        // regardless of whether the assembly actually unloaded". A second
        // UnloadMod call on the same id must therefore be a no-op
        // (idempotency, returning empty warnings) even after step 2 threw
        // on the first call.
        var h = Harness.Build(throwingContractStore: true);
        InjectAndApply(h, new SystemRegisteringMod());

        IReadOnlyList<ValidationWarning> first = h.Pipeline.UnloadMod(TestModId);
        first.Should().NotBeEmpty(
            "the test setup must produce at least one step-failure warning");

        IReadOnlyList<ValidationWarning> second = h.Pipeline.UnloadMod(TestModId);
        second.Should().BeEmpty(
            "after the first unload the mod is no longer in _activeMods regardless of step failures");
    }

    // 10
    [Fact]
    public void UnloadMod_ReturnsWarning_WithModIdAndStepNumber()
    {
        // Locks the warning shape (§9.5.1: "logs with (modId, stepNumber)")
        // so callers — primarily M7.5 menu UI — can rely on a stable
        // message format.
        var h = Harness.Build(throwingContractStore: true);
        InjectAndApply(h, new SystemRegisteringMod());

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(TestModId);

        ValidationWarning w = warnings.Should().ContainSingle().Subject;
        w.ModId.Should().Be(TestModId);
        w.Message.Should().Contain($"step 2 failed for mod '{TestModId}'");
    }

    // 11
    [Fact]
    public void UnloadAll_DelegatesToUnloadModForEach_AccumulatesWarnings()
    {
        // M7.2 refactor — UnloadAll iterates _activeMods and calls
        // UnloadMod for each. Two mods active → both must be removed,
        // and per-mod warnings must accumulate into the returned list.
        // Empty mods are used so two simultaneous Apply calls don't trip
        // DependencyGraph's duplicate-system-type guard — the assertion
        // here is about UnloadAll's accumulator, not about apply-time
        // graph mechanics.
        var h = Harness.Build(throwingContractStore: true);
        InjectAndApply(h, new EmptyMod(), modId: "tests.regular.m72.alpha");
        InjectAndApply(h, new EmptyMod(), modId: "tests.regular.m72.beta");

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadAll();

        warnings.Should().HaveCount(2,
            "each per-mod chain produces one step-2 warning under the throwing store");
        warnings.Should().Contain(w => w.ModId == "tests.regular.m72.alpha");
        warnings.Should().Contain(w => w.ModId == "tests.regular.m72.beta");
        h.Loader.GetLoaded().Should().BeEmpty(
            "every mod must be removed regardless of step-level failures");
    }

    // 12
    [Fact]
    public void UnloadAll_OnEmptyActiveSet_RebuildsKernelOnlyScheduler()
    {
        // Regression — the empty-active-set path must still install a
        // kernel-only graph (preserved from the pre-M7.2 UnloadAll
        // semantics).
        var h = Harness.Build(new CoreSurvivorSystem());

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadAll();

        warnings.Should().BeEmpty();
        SchedulerContains<CoreSurvivorSystem>(h.Scheduler).Should().BeTrue(
            "with no active mods the scheduler must hold the kernel-only graph");
    }

    // 13
    [Fact]
    public void UnloadAll_PreservesM71Guard_ThrowsWhenRunning()
    {
        // CRITICAL regression guard for M7.1 — UnloadAll's run-flag
        // guard must continue to fire after the M7.2 refactor. If this
        // breaks, the M7.1 invariant is gone and the M0–M7.1 baseline is
        // at risk. STOP and investigate before proceeding.
        var h = Harness.Build();
        h.Pipeline.Resume();

        Action act = () => h.Pipeline.UnloadAll();

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("Pause the scheduler before unloading mods");
    }

    // --- Harness ------------------------------------------------------------

    private sealed class Harness
    {
        public ModLoader Loader { get; }
        public ModRegistry Registry { get; }
        public IModContractStore ContractStore { get; }
        public GameServices Services { get; }
        public ParallelSystemScheduler Scheduler { get; }
        public ModIntegrationPipeline Pipeline { get; }

        private Harness(
            ModLoader loader,
            ModRegistry registry,
            IModContractStore contractStore,
            GameServices services,
            ParallelSystemScheduler scheduler,
            ModIntegrationPipeline pipeline)
        {
            Loader = loader;
            Registry = registry;
            ContractStore = contractStore;
            Services = services;
            Scheduler = scheduler;
            Pipeline = pipeline;
        }

        public static Harness Build(params SystemBase[] coreSystems)
            => Build(throwingContractStore: false, coreSystems);

        public static Harness Build(bool throwingContractStore, params SystemBase[] coreSystems)
        {
            var loader = new ModLoader();
            var registry = new ModRegistry();
            registry.SetCoreSystems(coreSystems);
            var validator = new ContractValidator();
            IModContractStore contractStore = throwingContractStore
                ? new ThrowingRevokeAllContractStore(new ModContractStore())
                : new ModContractStore();
            var services = new GameServices();
            var world = new World();
            var ticks = new TickScheduler();
            var graph = new DependencyGraph();
            foreach (SystemBase s in coreSystems)
                graph.AddSystem(s);
            graph.Build();
            var scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), ticks, world);
            var pipeline = new ModIntegrationPipeline(
                loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());
            return new Harness(loader, registry, contractStore, services, scheduler, pipeline);
        }
    }

    private static void InjectAndApply(Harness h, IMod mod, string? modId = null)
    {
        string id = modId ?? TestModId;
        IReadOnlyList<Type> declared = mod is SystemRegisteringMod
            ? new[] { typeof(M72TestSystem) }
            : Array.Empty<Type>();

        var manifest = new ModManifest
        {
            Id = id,
            Name = id,
            Version = "1.0.0",
            Author = "Test",
        };
        var context = new ModLoadContext(id);
        var loaded = new LoadedMod(id, manifest, mod, context, declared);
        h.Loader.RegisterLoaded(loaded);

        PipelineResult result = h.Pipeline.Apply(new[] { id });
        result.Success.Should().BeTrue(
            "the test fixture must apply cleanly so the unload step under test is meaningful " +
            $"(errors: {string.Join("; ", System.Linq.Enumerable.Select(result.Errors, e => e.Message))})");
    }

    private static bool SchedulerContains<T>(ParallelSystemScheduler scheduler) where T : SystemBase
    {
        foreach (SystemPhase phase in scheduler.Phases)
        {
            foreach (SystemBase s in phase.Systems)
            {
                if (s is T) return true;
            }
        }
        return false;
    }

    // --- Test seam: throwing contract store ---------------------------------

    /// <summary>
    /// Decorator that delegates everything to an inner real
    /// <see cref="IModContractStore"/> except <see cref="RevokeAll"/>,
    /// which always throws. Used to inject a step-2 failure into the
    /// unload chain so §9.5.1 best-effort discipline can be verified.
    /// </summary>
    private sealed class ThrowingRevokeAllContractStore : IModContractStore
    {
        private readonly IModContractStore _inner;

        public ThrowingRevokeAllContractStore(IModContractStore inner)
        {
            _inner = inner;
        }

        public void Publish<T>(string modId, T contract) where T : IModContract
            => _inner.Publish<T>(modId, contract);

        public bool TryGet<T>(out T? contract) where T : class, IModContract
            => _inner.TryGet<T>(out contract);

        public void RevokeAll(string modId)
            => throw new InvalidOperationException(
                "Test seam: ThrowingRevokeAllContractStore.RevokeAll always throws " +
                "to exercise §9.5.1 best-effort failure semantics.");
    }

    // --- Test fixture mods --------------------------------------------------

    private sealed class SubscribingMod : IMod
    {
        public int HandlerCallCount;

        public void Initialize(IModApi api)
        {
            api.Subscribe<M72UnloadTestEvent>(_ => HandlerCallCount++);
        }

        public void Unload() { }
    }

    private sealed class ContractPublishingMod : IMod
    {
        public void Initialize(IModApi api)
        {
            api.PublishContract(new M72TestContract("m72-marker"));
        }

        public void Unload() { }
    }

    private sealed class SystemRegisteringMod : IMod
    {
        public void Initialize(IModApi api)
        {
            api.RegisterSystem<M72TestSystem>();
        }

        public void Unload() { }
    }

    private sealed class EmptyMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }

    [SystemAccess(reads: new Type[0], writes: new Type[0], bus: nameof(IGameServices.Combat))]
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    public sealed class M72TestSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new Type[0], bus: nameof(IGameServices.World))]
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    public sealed class CoreSurvivorSystem : SystemBase
    {
        public override void Update(float delta) { }
    }
}
