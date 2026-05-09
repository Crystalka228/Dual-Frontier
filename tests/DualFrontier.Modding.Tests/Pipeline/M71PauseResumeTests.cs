using System;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// M7.1 — Pause/Resume + IsRunning state on
/// <see cref="ModIntegrationPipeline"/>. Covers the run-flag discipline
/// from MOD_OS_ARCHITECTURE v1.4 §9.2 / §9.3:
/// <list type="bullet">
///   <item>Default-paused construction (load-bearing for the 338 M0–M6
///   tests that never touch <c>Pause</c>/<c>Resume</c>).</item>
///   <item>Idempotent <see cref="ModIntegrationPipeline.Pause"/> /
///   <see cref="ModIntegrationPipeline.Resume"/> setters.</item>
///   <item>Guards on <see cref="ModIntegrationPipeline.Apply"/> and
///   <see cref="ModIntegrationPipeline.UnloadAll"/> with verbatim
///   canonical messages from §9.3.</item>
///   <item>Round-trip Resume→Pause→Apply.</item>
///   <item>Regression guard: fresh pipeline → empty <c>Apply</c>
///   succeeds without any pause/resume bookkeeping (mirrors the
///   construction pattern every existing test relies on).</item>
/// </list>
/// Out of scope (M7.2–M7.5): ALC unload chain, WeakReference spin loop,
/// build-pipeline override, and mod-menu UI integration. Those land in
/// later sub-phases per the M7 decomposition.
/// </summary>
public sealed class M71PauseResumeTests
{
    [Fact]
    public void Pipeline_DefaultState_IsRunningFalse()
    {
        // §9.2 step 1 + the load-bearing default: a freshly constructed
        // pipeline reports IsRunning == false so GameBootstrap and every
        // legacy test can call Apply without an explicit Pause().
        var pipeline = BuildPipeline();

        pipeline.IsRunning.Should().BeFalse(
            "fresh ModIntegrationPipeline must be in the paused state by default");
    }

    [Fact]
    public void Pause_SetsIsRunningToFalse()
    {
        // After Resume() flips the flag to true, Pause() must drop it
        // back to false — the §9.2 step 1 entry the mod menu relies on.
        var pipeline = BuildPipeline();
        pipeline.Resume();

        pipeline.Pause();

        pipeline.IsRunning.Should().BeFalse();
    }

    [Fact]
    public void Resume_SetsIsRunningToTrue()
    {
        // §9.2 step 4: after a successful Apply the menu calls Resume()
        // and the flag flips to true, re-arming the Apply/UnloadAll
        // guards.
        var pipeline = BuildPipeline();

        pipeline.Resume();

        pipeline.IsRunning.Should().BeTrue();
    }

    [Fact]
    public void Pause_Idempotent_NoThrow()
    {
        // Idempotency contract: the menu may re-enter the paused state
        // multiple times (fresh + Pause, or two consecutive Pauses) and
        // the second call must be a silent no-op rather than an
        // exception. Locks the simplest-possible setter contract.
        var pipeline = BuildPipeline();

        Action act = () =>
        {
            pipeline.Pause();
            pipeline.Pause();
        };

        act.Should().NotThrow();
        pipeline.IsRunning.Should().BeFalse();
    }

    [Fact]
    public void Resume_Idempotent_NoThrow()
    {
        // Mirror of Pause idempotency: a defensive Resume on an already
        // running pipeline (e.g. on dialog reentry) must not throw.
        var pipeline = BuildPipeline();

        Action act = () =>
        {
            pipeline.Resume();
            pipeline.Resume();
        };

        act.Should().NotThrow();
        pipeline.IsRunning.Should().BeTrue();
    }

    [Fact]
    public void Apply_WhenRunning_ThrowsInvalidOperationException_WithCanonicalMessage()
    {
        // §9.3 — calling Apply while the simulation is running must
        // throw with the exact canonical message. Asserting the verbatim
        // string locks the spec wording at assertion level so any
        // accidental paraphrase trips the test.
        var pipeline = BuildPipeline();
        pipeline.Resume();

        Action act = () => pipeline.Apply(Array.Empty<string>());

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("Pause the scheduler before applying mods");
    }

    [Fact]
    public void Apply_WhenPaused_Succeeds()
    {
        // Happy path: a paused pipeline applying an empty mod list
        // succeeds (no mods to load → trivially valid), exercising the
        // guard's pass-through branch with the simplest possible input.
        var pipeline = BuildPipeline();

        PipelineResult result = pipeline.Apply(Array.Empty<string>());

        result.Success.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.LoadedModIds.Should().BeEmpty();
    }

    [Fact]
    public void Apply_AfterResumeThenPause_Succeeds()
    {
        // Round-trip: Resume() → Pause() → Apply() must succeed. Locks
        // the menu's full pause/resume cycle: after a previous Apply the
        // menu calls Resume; before the next Apply it calls Pause.
        var pipeline = BuildPipeline();
        pipeline.Resume();
        pipeline.Pause();

        PipelineResult result = pipeline.Apply(Array.Empty<string>());

        result.Success.Should().BeTrue();
    }

    [Fact]
    public void UnloadAll_WhenRunning_ThrowsInvalidOperationException_WithCanonicalMessage()
    {
        // Parallel to the Apply guard: UnloadAll mutates the scheduler's
        // phase list too, so it must reject mid-tick invocation with
        // the same exception type and the parallel-form §9.3 message.
        var pipeline = BuildPipeline();
        pipeline.Resume();

        Action act = () => pipeline.UnloadAll();

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("Pause the scheduler before unloading mods");
    }

    [Fact]
    public void UnloadAll_WhenPaused_Succeeds()
    {
        // Happy path for UnloadAll: a paused pipeline with no active
        // mods rebuilds the scheduler with the kernel-only graph and
        // returns silently. Exercises the guard's pass-through branch.
        var pipeline = BuildPipeline();

        Action act = () => pipeline.UnloadAll();

        act.Should().NotThrow();
    }

    [Fact]
    public void Apply_OnFreshPipelineWithoutPauseResume_DoesNotThrow_RegressionGuard()
    {
        // CRITICAL regression guard for the "default false" invariant:
        // every M0–M6 test (currently 338 of them) constructs a fresh
        // pipeline and calls Apply without ever calling Pause/Resume.
        // If this test breaks, the new guard has changed the semantics
        // observable to existing tests and the entire 338-test baseline
        // is at risk. STOP and investigate before proceeding.
        var pipeline = BuildPipeline();

        Action act = () => pipeline.Apply(Array.Empty<string>());

        act.Should().NotThrow();
    }

    /// <summary>
    /// Minimal pipeline construction harness mirroring
    /// <c>M51PipelineIntegrationTests.BuildPipeline</c>: an empty
    /// kernel-system set, a default <see cref="DependencyGraph"/>, and a
    /// freshly constructed <see cref="ParallelSystemScheduler"/>. M7.1
    /// tests never touch any fixture mod, so this is the lightest
    /// construction that still produces a fully-wired pipeline.
    /// </summary>
    private static ModIntegrationPipeline BuildPipeline()
    {
        var loader = new ModLoader();
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        IGameServices services = new GameServices();
        var world = new World();
        var ticks = new TickScheduler();
        var graph = new DependencyGraph();
        graph.Build();
        var scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), ticks, world);
        return new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());
    }
}
