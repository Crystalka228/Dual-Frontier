using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// M7.3 — §9.5 step 7 protocol on
/// <see cref="ModIntegrationPipeline.UnloadMod"/>. Covers the
/// <see cref="WeakReference"/> spin loop + GC pump bracket +
/// <c>ModUnloadTimeout</c> warning surface from
/// <see cref="MOD_OS_ARCHITECTURE">MOD_OS_ARCHITECTURE</see> v1.4 §9.5
/// step 7 / §9.5.1:
/// <list type="bullet">
///   <item>Happy path — empty in-memory ALC unloads instantly, no warning.</item>
///   <item>Timeout path — external strong ref retains the ALC, the
///   spin reaches the 10 s timeout and emits a <c>ModUnloadTimeout</c>
///   warning.</item>
///   <item>Warning shape — canonical substrings (modId,
///   <c>"§9.5 step 7"</c>, <c>"10000 ms"</c>) locked at substring level
///   so the future menu UI can pattern-match safely.</item>
///   <item>Step 7 runs after upstream step failure — the
///   <c>ThrowingRevokeAllContractStore</c> seam from M7.2 makes step 2
///   throw; the spin still runs and the timeout warning still fires
///   (AD #7: "step 7 always runs after step 6 attempt regardless of
///   step 6 outcome" — verified here via the structurally equivalent
///   case "step 7 always runs after step N attempt regardless of step
///   N outcome", N=2; the only step in 1–6 we have a non-disruptive
///   throwing seam for, since <c>ModLoader</c> is sealed without an
///   <c>IModLoader</c> abstraction — extracting one is out of scope
///   for M7.3).</item>
///   <item>Mod removed from active set after timeout — second
///   <c>UnloadMod</c> for the same id is a silent no-op.</item>
/// </list>
/// Phase 2 carried-debt tests against real-mod fixtures (§10.4
/// hard-required <c>WeakReference.IsAlive == false</c> check) live in
/// <c>M73Phase2DebtTests</c>.
/// </summary>
public sealed class M73Step7Tests
{
    private const string TestModId = "tests.regular.m73step7";

    // 1
    [Fact]
    public void UnloadMod_Step7_HappyPath_NoTimeoutWarning()
    {
        // Empty in-memory ALC has nothing loaded; on _activeMods.Remove +
        // ModLoader.UnloadMod the runtime can release it on the very
        // first GC pump pass. The chain returns no warnings — both step
        // 6 (clean) and step 7 (released within timeout).
        var h = Harness.Build();
        InjectAndApply(h, new EmptyMod());

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(TestModId);

        warnings.Should().BeEmpty(
            "an empty in-memory ALC must release inside the 10 s spin and emit no ModUnloadTimeout warning");
    }

    // 2
    [Fact]
    public void UnloadMod_Step7_TimeoutPath_EmitsModUnloadTimeoutWarning()
    {
        // External strong ref to the ALC keeps WR.IsAlive == true through
        // the entire spin → step 7 reaches its 10 s timeout and appends a
        // ModUnloadTimeout warning. Verifies the spin honours the timeout
        // contract rather than blocking forever.
        var h = Harness.Build();
        InjectAndApply(h, new EmptyMod());
        object alcRetainer = CaptureContextAndDropMod(h.Pipeline, TestModId);

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(TestModId);

        warnings.Should().ContainSingle(
            "the only warning expected is the §9.5 step 7 timeout — every other step succeeds for an EmptyMod");
        warnings[0].Message.Should().Contain("ModUnloadTimeout");
        GC.KeepAlive(alcRetainer);
    }

    // 3
    [Fact]
    public void UnloadMod_Step7_TimeoutWarning_HasCanonicalShape()
    {
        // Locks the warning text shape (modId, "§9.5 step 7", "10000 ms")
        // at substring level so future menu UI / log scrapers can
        // pattern-match safely. Mirrors the M7.2 substring-locking
        // discipline for §9.5.1 step-failure warnings.
        var h = Harness.Build();
        InjectAndApply(h, new EmptyMod());
        object alcRetainer = CaptureContextAndDropMod(h.Pipeline, TestModId);

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(TestModId);

        ValidationWarning w = warnings.Should().ContainSingle().Subject;
        w.ModId.Should().Be(TestModId);
        w.Message.Should().Contain("ModUnloadTimeout",
            "menu UI keys off the ModUnloadTimeout substring to surface 'leaked reference'");
        w.Message.Should().Contain(TestModId,
            "the warning must name the offending mod by id");
        w.Message.Should().Contain("§9.5 step 7",
            "the warning must cite the spec section so log readers can trace it back");
        w.Message.Should().Contain("10000 ms",
            "the warning must record the timeout value used (10 s default per §9.5)");
        GC.KeepAlive(alcRetainer);
    }

    // 4
    [Fact]
    public void UnloadMod_Step7_AfterEarlierStepFailure_StillRunsAndTimesOut()
    {
        // AD #7: step 7 always runs after step 6 attempt, regardless of
        // step 6 outcome. ModLoader is sealed without an IModLoader
        // abstraction so a step-6 throwing seam would require unseal-or-
        // interface (out of scope for M7.3). The structurally equivalent
        // assertion "step 7 always runs after step N attempt, regardless
        // of step N outcome" is verified here via the existing
        // ThrowingRevokeAllContractStore seam from M7.2 (step 2 throws).
        // Combined with the external ALC retainer that forces step 7 to
        // time out, the warnings list must contain BOTH a step-2-failed
        // entry AND a ModUnloadTimeout entry.
        var h = Harness.Build(throwingContractStore: true);
        InjectAndApply(h, new EmptyMod());
        object alcRetainer = CaptureContextAndDropMod(h.Pipeline, TestModId);

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(TestModId);

        warnings.Should().HaveCount(2,
            "one warning for the step 2 throw + one for the §9.5 step 7 timeout");
        warnings.Should().Contain(w => w.Message.Contains("step 2 failed"),
            "step 2 failure must surface even though step 7 also times out");
        warnings.Should().Contain(w => w.Message.Contains("ModUnloadTimeout"),
            "step 7 must run regardless of upstream step failures (AD #7)");
        GC.KeepAlive(alcRetainer);
    }

    // 5
    [Fact]
    public void UnloadMod_Step7_Timeout_ModRemovedFromActiveSet_OnRetryReturnsEmpty()
    {
        // §9.5.1 invariant — "mod removed from active set regardless of
        // whether the assembly actually unloaded". After a step 7
        // timeout, the mod is no longer in _activeMods, so a second
        // UnloadMod call for the same id is the idempotent no-op path
        // (returns empty warnings, does not throw).
        var h = Harness.Build();
        InjectAndApply(h, new EmptyMod());
        object alcRetainer = CaptureContextAndDropMod(h.Pipeline, TestModId);

        IReadOnlyList<ValidationWarning> first = h.Pipeline.UnloadMod(TestModId);
        first.Should().NotBeEmpty(
            "the test setup must produce at least one ModUnloadTimeout warning on the first call");
        first[0].Message.Should().Contain("ModUnloadTimeout");

        IReadOnlyList<ValidationWarning> second = h.Pipeline.UnloadMod(TestModId);

        second.Should().BeEmpty(
            "after the first unload the mod is no longer in _activeMods regardless of step 7 timeout");
        GC.KeepAlive(alcRetainer);
    }

    // --- Test helpers -----------------------------------------------------

    /// <summary>
    /// Captures a strong reference to the active mod's
    /// <c>ModLoadContext</c> and returns it, then drops the local
    /// <c>LoadedMod</c> reference inside this non-inlined method's
    /// frame so the caller never sees a stack-frame strong ref to the
    /// LoadedMod. The returned object is the only retainer keeping the
    /// ALC alive — pair it with <see cref="GC.KeepAlive(object)"/> at
    /// the end of the test to defeat any premature finalization.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static object CaptureContextAndDropMod(
        ModIntegrationPipeline pipeline,
        string modId)
    {
        LoadedMod loaded = pipeline.GetActiveModForTests(modId)
            ?? throw new InvalidOperationException(
                $"GetActiveModForTests returned null for '{modId}'; the test fixture did not apply.");
        return loaded.Context;
    }

    // --- Harness ----------------------------------------------------------

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

        public static Harness Build(bool throwingContractStore = false)
        {
            var loader = new ModLoader();
            var registry = new ModRegistry();
            registry.SetCoreSystems(Array.Empty<SystemBase>());
            var validator = new ContractValidator();
            IModContractStore contractStore = throwingContractStore
                ? new ThrowingRevokeAllContractStore(new ModContractStore())
                : new ModContractStore();
            var services = new GameServices();
            var world = new World();
            var ticks = new TickScheduler();
            var graph = new DependencyGraph();
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
        var manifest = new ModManifest
        {
            Id = id,
            Name = id,
            Version = "1.0.0",
            Author = "Test",
        };
        var context = new ModLoadContext(id);
        var loaded = new LoadedMod(id, manifest, mod, context, Array.Empty<Type>());
        h.Loader.RegisterLoaded(loaded);

        PipelineResult result = h.Pipeline.Apply(new[] { id });
        result.Success.Should().BeTrue(
            "the test fixture must apply cleanly so the step under test is meaningful " +
            $"(errors: {string.Join("; ", System.Linq.Enumerable.Select(result.Errors, e => e.Message))})");
    }

    // --- Test seam: throwing contract store -------------------------------

    /// <summary>
    /// Mirror of the M7.2 <c>ThrowingRevokeAllContractStore</c>
    /// decorator — delegates everything to an inner real
    /// <see cref="IModContractStore"/> except <see cref="RevokeAll"/>,
    /// which always throws. Used in test 4 to inject a step-2 failure
    /// so AD #7 ("step 7 always runs regardless of upstream step
    /// outcome") can be verified against the existing throwing seam
    /// without touching production <c>ModLoader</c>.
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
                "to exercise §9.5.1 best-effort failure semantics + AD #7 step-7-after-failure invariant.");
    }

    // --- Test fixture mods ------------------------------------------------

    private sealed class EmptyMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
