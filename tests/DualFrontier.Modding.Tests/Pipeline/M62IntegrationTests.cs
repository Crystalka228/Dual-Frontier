using System;
using System.IO;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using DualFrontier.Systems.Combat;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// End-to-end coverage for M6.2 — bridge replacement mechanics driven
/// through <see cref="ModIntegrationPipeline.Apply"/>. Exercises every
/// scenario from MOD_OS_ARCHITECTURE §7.5 that is reachable inside a
/// single load batch (the mod-unloaded scenario belongs to M7 hot-reload
/// territory and is out of scope here):
/// <list type="number">
///   <item>Replaceable bridge → success, kernel system skipped, mod system runs.</item>
///   <item>Protected (non-Replaceable) target → ProtectedSystemReplacement, batch rejected.</item>
///   <item>Unknown FQN → UnknownSystemReplacement, batch rejected.</item>
///   <item>Two mods replacing the same FQN → BridgeReplacementConflict on both, batch rejected.</item>
///   <item>Regression guard: no replaces in batch → every kernel stub stays in the scheduler.</item>
/// </list>
/// Phase H (M6.1) gates rejections (2)–(4) before <c>IMod.Initialize</c> runs;
/// the rejection fixtures throw inside <c>Initialize</c> so any
/// order-of-operations regression (skip-before-validation) surfaces as a
/// loud test failure rather than a silent pass.
/// </summary>
public sealed class M62IntegrationTests
{
    [Fact]
    public void Apply_WithReplaceableBridge_SkipsBridgeAndUsesModSystem()
    {
        // Pipeline starts with the kernel CombatSystem registered as Core.
        // ReplacesCombatMod's manifest lists CombatSystem's FQN in
        // replaces. The pipeline must:
        //   1. Pass Phase H (CombatSystem is [BridgeImplementation(Replaceable=true)]).
        //   2. Run ReplacesCombatMod.Initialize, which registers
        //      ReplacementCombatSystem as a Mod-origin system.
        //   3. In step [5-7] skip the kernel CombatSystem entry and add
        //      ReplacementCombatSystem to the dependency graph.
        // Result: a fresh scheduler whose phases contain the replacement
        // and not the original.
        var pipeline = BuildPipeline(new CombatSystem());

        PipelineResult result = pipeline.Apply(new[] { M62FixturePaths.ReplacesCombat });

        result.Success.Should().BeTrue();
        result.LoadedModIds.Should().ContainSingle()
            .Which.Should().Be("tests.regular.replacescombat");
        result.Errors.Should().BeEmpty();

        ParallelSystemScheduler scheduler = pipeline.SchedulerForTests;
        ContainsSystemByFqn(scheduler, "DualFrontier.Systems.Combat.CombatSystem")
            .Should().BeFalse(
                "the replaced kernel bridge must be omitted from the scheduler's phase list");
        ContainsSystemByFqn(scheduler,
            "Fixture.RegularMod_ReplacesCombat.ReplacementCombatSystem")
            .Should().BeTrue(
                "the mod's replacement system must take the bridge's place in the graph");
    }

    [Fact]
    public void Apply_WithProtectedSystemReplacement_RejectsBatch()
    {
        // ReplacesProtectedMod targets DualFrontier.Systems.Pawn.SocialSystem,
        // which is annotated [BridgeImplementation(Phase = 3)] with the
        // default Replaceable=false. Phase H must reject the batch with
        // ProtectedSystemReplacement; the scheduler's old phase list must
        // remain referentially identical (atomic-rebuild contract). The
        // fixture's Initialize throws — if the test trips that, an
        // order-of-operations regression has skipped Phase H.
        var pipeline = BuildPipeline(new CombatSystem());
        IReadOnlyList<SystemPhase> phasesBefore = pipeline.SchedulerForTests.Phases;

        PipelineResult result = pipeline.Apply(new[] { M62FixturePaths.ReplacesProtected });

        result.Success.Should().BeFalse();
        ValidationError err = result.Errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.ProtectedSystemReplacement &&
            e.ModId == "tests.regular.replacesprotected").Subject;
        err.Message.Should().Contain("DualFrontier.Systems.Pawn.SocialSystem",
            "the diagnostic must echo the offending FQN");
        err.Message.Should().Contain("Replaceable=false",
            "Phase H must explain why the target is protected");
        result.Errors.Should().NotContain(e => e.Message.Contains("threw during Initialize"),
            "Phase H must reject the batch before pass [4] reaches IMod.Initialize");
        pipeline.SchedulerForTests.Phases.Should().BeSameAs(phasesBefore,
            "atomicity: a rejected apply must leave the scheduler's phase list untouched");
    }

    [Fact]
    public void Apply_WithUnknownFqn_RejectsBatch()
    {
        // ReplacesUnknownMod's manifest names DualFrontier.Phantom.NonExistentSystem,
        // which resolves to no type in any loaded assembly. Phase H
        // surfaces UnknownSystemReplacement and the pipeline rolls back.
        var pipeline = BuildPipeline(new CombatSystem());
        IReadOnlyList<SystemPhase> phasesBefore = pipeline.SchedulerForTests.Phases;

        PipelineResult result = pipeline.Apply(new[] { M62FixturePaths.ReplacesUnknown });

        result.Success.Should().BeFalse();
        ValidationError err = result.Errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.UnknownSystemReplacement &&
            e.ModId == "tests.regular.replacesunknown").Subject;
        err.Message.Should().Contain("DualFrontier.Phantom.NonExistentSystem",
            "the diagnostic must echo the offending FQN");
        result.Errors.Should().NotContain(e => e.Message.Contains("threw during Initialize"),
            "Phase H must reject the batch before pass [4] reaches IMod.Initialize");
        pipeline.SchedulerForTests.Phases.Should().BeSameAs(phasesBefore,
            "atomicity: a rejected apply must leave the scheduler's phase list untouched");
    }

    [Fact]
    public void Apply_WithTwoModsReplacingSameSystem_RejectsBatchWithConflict()
    {
        // Both ReplacesCombatMod and ReplacesCombatAltMod list
        // CombatSystem in their replaces fields. Phase H emits two
        // BridgeReplacementConflict errors, each cross-referencing the
        // other via ConflictingModId so the UI can flag both mod cards
        // simultaneously.
        var pipeline = BuildPipeline(new CombatSystem());
        IReadOnlyList<SystemPhase> phasesBefore = pipeline.SchedulerForTests.Phases;

        PipelineResult result = pipeline.Apply(new[]
        {
            M62FixturePaths.ReplacesCombat,
            M62FixturePaths.ReplacesCombatAlt,
        });

        result.Success.Should().BeFalse();
        result.Errors
            .Where(e => e.Kind == ValidationErrorKind.BridgeReplacementConflict)
            .Should().HaveCount(2,
                "both mods must produce a symmetric BridgeReplacementConflict");

        ValidationError errA = result.Errors.Single(e =>
            e.Kind == ValidationErrorKind.BridgeReplacementConflict &&
            e.ModId == "tests.regular.replacescombat");
        errA.ConflictingModId.Should().Be("tests.regular.replacescombat.alt");

        ValidationError errB = result.Errors.Single(e =>
            e.Kind == ValidationErrorKind.BridgeReplacementConflict &&
            e.ModId == "tests.regular.replacescombat.alt");
        errB.ConflictingModId.Should().Be("tests.regular.replacescombat");

        result.Errors.Should().NotContain(e => e.Message.Contains("threw during Initialize"),
            "Phase H must reject the batch before pass [4] reaches IMod.Initialize " +
            "(ReplacesCombatAltMod.Initialize would otherwise throw and surface here)");
        pipeline.SchedulerForTests.Phases.Should().BeSameAs(phasesBefore,
            "atomicity: a rejected apply must leave the scheduler's phase list untouched");
    }

    [Fact]
    public void Apply_WithoutAnyReplaces_BridgesAllRegistered()
    {
        // Regression guard: the skip path must NOT fire when no mod in the
        // batch declares replaces. A representative non-conflicting subset
        // of Phase 5 combat stubs is registered as Core. The full set
        // (Combat + Damage + StatusEffect all writing HealthComponent)
        // cannot coexist in a single graph today — the kernel relies on a
        // future vanilla mod to disambiguate via replaces. The five chosen
        // here cover every kind of Phase 5 stub surface (component write,
        // event write, self-write) without triggering the
        // HealthComponent triple-write conflict that would short-circuit
        // the test before the skip-set logic could be exercised.
        var pipeline = BuildPipeline(
            new CombatSystem(),
            new ProjectileSystem(),
            new ComboResolutionSystem(),
            new CompositeResolutionSystem());

        PipelineResult result = pipeline.Apply(new[] { M51FixturePaths.DependedOn });

        result.Success.Should().BeTrue();
        result.LoadedModIds.Should().ContainSingle()
            .Which.Should().Be("tests.regular.dependedon");

        ParallelSystemScheduler scheduler = pipeline.SchedulerForTests;
        ContainsSystemByFqn(scheduler, "DualFrontier.Systems.Combat.CombatSystem")
            .Should().BeTrue();
        ContainsSystemByFqn(scheduler, "DualFrontier.Systems.Combat.ProjectileSystem")
            .Should().BeTrue();
        ContainsSystemByFqn(scheduler, "DualFrontier.Systems.Combat.ComboResolutionSystem")
            .Should().BeTrue();
        ContainsSystemByFqn(scheduler,
            "DualFrontier.Systems.Combat.CompositeResolutionSystem")
            .Should().BeTrue();
    }

    private static M62Pipeline BuildPipeline(params SystemBase[] coreSystems)
    {
        var loader = new ModLoader();
        var registry = new ModRegistry();
        registry.SetCoreSystems(coreSystems);
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        IGameServices services = new GameServices();
        var world = new World();
        var ticks = new TickScheduler();
        var graph = new DependencyGraph();
        foreach (SystemBase s in coreSystems)
            graph.AddSystem(s);
        graph.Build();
        var scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), ticks, world);
        var pipeline = new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());
        return new M62Pipeline(pipeline, scheduler);
    }

    private static bool ContainsSystemByFqn(ParallelSystemScheduler scheduler, string fqn)
    {
        foreach (SystemPhase phase in scheduler.Phases)
        {
            foreach (SystemBase system in phase.Systems)
            {
                if (system.GetType().FullName == fqn)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Test seam — pairs the pipeline with the scheduler it owns so
    /// post-Apply assertions can read the (rebuilt or untouched) phase
    /// list. The production pipeline owns a private reference to the
    /// scheduler; tests have always reached it through a Harness pattern,
    /// preserved here.
    /// </summary>
    private sealed class M62Pipeline
    {
        private readonly ModIntegrationPipeline _inner;
        public ParallelSystemScheduler SchedulerForTests { get; }

        public M62Pipeline(ModIntegrationPipeline inner, ParallelSystemScheduler scheduler)
        {
            _inner = inner;
            SchedulerForTests = scheduler;
        }

        public PipelineResult Apply(System.Collections.Generic.IReadOnlyList<string> modPaths)
            => _inner.Apply(modPaths);
    }
}

internal static class M62FixturePaths
{
    private static readonly string FixturesRoot =
        Path.Combine(AppContext.BaseDirectory, "Fixtures");

    public static string ReplacesCombat => Path.Combine(FixturesRoot, "Fixture.RegularMod_ReplacesCombat");
    public static string ReplacesCombatAlt => Path.Combine(FixturesRoot, "Fixture.RegularMod_ReplacesCombat_Alt");
    public static string ReplacesProtected => Path.Combine(FixturesRoot, "Fixture.RegularMod_ReplacesProtected");
    public static string ReplacesUnknown => Path.Combine(FixturesRoot, "Fixture.RegularMod_ReplacesUnknown");
}
