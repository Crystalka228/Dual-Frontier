using System;
using System.Runtime.CompilerServices;
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
/// M7.3 Phase 2 carried-debt closure — every regular mod loaded under
/// test must unload to <c>WeakReference.IsAlive == false</c> within the
/// §9.5 step 7 timeout (MOD_OS_ARCHITECTURE v1.4 §10.4 hard requirement).
/// Originally a Phase 2 backlog item, now closed here against two
/// representative real-mod fixtures already used by M5.1 and M6.2:
/// <list type="bullet">
///   <item><c>Fixture.RegularMod_DependedOn</c> — minimal regular mod
///   with empty <c>Initialize</c>; loads via <c>pipeline.Apply</c>
///   through the on-disk path so the assembly enters the
///   <c>ModLoadContext</c> for real.</item>
///   <item><c>Fixture.RegularMod_ReplacesCombat</c> — regular mod with
///   non-trivial <c>Initialize</c> (registers a replacement system,
///   declares <c>replaces</c>); locks the closure for real-mod
///   surfaces with system registration + bridge replacement.</item>
/// </list>
/// Per §11.4: "WeakReference unload tests are flaky — any failure rate
/// above 0%" → halt + Phase 0 re-entry as v1.5 ratification candidate.
/// These tests must pass across at least 3 consecutive runs (verified
/// before commit per the M7.3 acceptance criterion).
/// </summary>
public sealed class M73Phase2DebtTests
{
    [Fact]
    public void RegularMod_DependedOn_LoadsAndUnloadsCleanly_WrReleasedWithinTimeout()
    {
        // §10.4 closure for the simplest regular-mod surface — no
        // dependencies, no system registration, no replaces. The
        // ModLoadContext owns one assembly (Fixture.RegularMod_DependedOn.dll)
        // and the IMod instance lives inside it. Apply + UnloadMod must
        // release the WR within the 10 s spin.
        var pipeline = BuildPipeline();
        const string modId = "tests.regular.dependedon";
        PipelineResult applyResult = pipeline.Apply(new[] { M51FixturePaths.DependedOn });
        applyResult.Success.Should().BeTrue(
            $"the fixture must apply cleanly for the unload assertion to be meaningful " +
            $"(errors: {string.Join("; ", System.Linq.Enumerable.Select(applyResult.Errors, e => e.Message))})");

        WeakReference alcRef = CaptureAlcAndDropLocals(pipeline, modId);

        System.Collections.Generic.IReadOnlyList<ValidationWarning> warnings = pipeline.UnloadMod(modId);

        warnings.Should().BeEmpty(
            "a clean fixture mod must traverse steps 1–7 without warnings");
        ModUnloadAssertions.AssertAlcReleasedWithin(
            alcRef, context: "Fixture.RegularMod_DependedOn");
    }

    [Fact]
    public void RegularMod_ReplacesCombat_LoadsAndUnloadsCleanly_WrReleasedWithinTimeout()
    {
        // §10.4 closure for a real-mod surface with non-trivial
        // Initialize: ReplacesCombatMod registers
        // ReplacementCombatSystem (mod-origin) and declares
        // replaces=[CombatSystem]. The pipeline skips kernel
        // CombatSystem at graph build, so the ModLoadContext holds
        // both the assembly and the live mod-system instance through
        // the scheduler. Step 3 (RemoveMod) + steps 4-5 (graph
        // rebuild) + step 6 (ALC.Unload) must drop every strong ref
        // so the WR releases inside the spin.
        var pipeline = BuildPipeline(new CombatSystem());
        const string modId = "tests.regular.replacescombat";
        PipelineResult applyResult = pipeline.Apply(new[] { M62FixturePaths.ReplacesCombat });
        applyResult.Success.Should().BeTrue(
            $"the fixture must apply cleanly for the unload assertion to be meaningful " +
            $"(errors: {string.Join("; ", System.Linq.Enumerable.Select(applyResult.Errors, e => e.Message))})");

        WeakReference alcRef = CaptureAlcAndDropLocals(pipeline, modId);

        System.Collections.Generic.IReadOnlyList<ValidationWarning> warnings = pipeline.UnloadMod(modId);

        warnings.Should().BeEmpty(
            "a clean fixture mod must traverse steps 1–7 without warnings");
        ModUnloadAssertions.AssertAlcReleasedWithin(
            alcRef, context: "Fixture.RegularMod_ReplacesCombat");
    }

    /// <summary>
    /// Captures a <see cref="WeakReference"/> to the active mod's
    /// <c>ModLoadContext</c>. Marked
    /// <see cref="MethodImplOptions.NoInlining"/> for the same JIT
    /// stack-frame reason as the production
    /// <c>CaptureAlcWeakReference</c>: the <c>LoadedMod</c> local must
    /// live only inside this helper's frame so the calling test method
    /// never sees a stack-frame strong ref.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CaptureAlcAndDropLocals(
        ModIntegrationPipeline pipeline,
        string modId)
    {
        LoadedMod loaded = pipeline.GetActiveModForTests(modId)
            ?? throw new InvalidOperationException(
                $"GetActiveModForTests returned null for '{modId}'; the fixture did not apply.");
        return new WeakReference(loaded.Context);
    }

    private static ModIntegrationPipeline BuildPipeline(params SystemBase[] coreSystems)
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
        return new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());
    }
}
