using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Menu;

/// <summary>
/// M7.5.A — <see cref="ModMenuController"/> editing-session lifecycle
/// per MOD_OS_ARCHITECTURE v1.5 §9.2 (Pause-Toggle-Apply-Resume) and
/// §9.6 (hot-reload-disabled mods cannot be toggled mid-session).
///
/// Coverage areas (22 tests):
/// <list type="bullet">
///   <item>BeginEditing — pause behavior + idempotency.</item>
///   <item>Cancel — resume + idempotency.</item>
///   <item>Toggle — pending-set mutation, §9.6 rejection, no-session,
///   unknown-mod, AD #5 first-load-is-not-reload semantics.</item>
///   <item>CanToggle — UI hint mirroring the Toggle gate.</item>
///   <item>GetEditableState — combined active+discovered rows with
///   correct flags; throws without a session.</item>
///   <item>Commit — no-op success, add-only, remove-only,
///   add-and-remove, AD #4 failure-stays-paused, retry recovery.</item>
/// </list>
/// Out of scope (M7.5.B / M8 / future): Godot UI scene tests, real
/// disk discovery against vanilla mod paths, save-game policy.
/// </summary>
public sealed class ModMenuControllerTests
{
    // 1
    [Fact]
    public void BeginEditing_OnIdleController_PausesPipelineAndSetsIsEditing()
    {
        // §9.2 step 1 — Pause is invoked even on a default-paused
        // pipeline; the observable signal we lock here is IsEditing
        // flipping to true, which means BeginEditing's full sequence
        // (Pause + GetActiveMods + Discover + snapshot) ran.
        var h = Harness.Build();

        h.Controller.BeginEditing();

        h.Controller.IsEditing.Should().BeTrue();
        h.Pipeline.IsRunning.Should().BeFalse(
            "fresh pipeline starts paused; BeginEditing keeps it paused");
    }

    // 2
    [Fact]
    public void BeginEditing_OnRunningPipeline_PausesPipeline()
    {
        // §9.2 step 1 — the canonical entry: simulation running, menu
        // opens, pipeline must be paused before any Toggle/Apply work.
        var h = Harness.Build();
        h.Pipeline.Resume();

        h.Controller.BeginEditing();

        h.Pipeline.IsRunning.Should().BeFalse(
            "BeginEditing must drop the run flag per §9.2 step 1");
    }

    // 3
    [Fact]
    public void BeginEditing_Twice_Idempotent()
    {
        // AD #6 — re-entering Begin on an already-editing controller
        // must be a silent no-op so the menu can re-open without
        // special casing.
        var h = Harness.Build();

        h.Controller.BeginEditing();
        h.Controller.BeginEditing();

        h.Controller.IsEditing.Should().BeTrue();
    }

    // 4
    [Fact]
    public void Cancel_AfterBeginEditing_DiscardsPendingAndResumes()
    {
        // §9.2 — Cancel restores the simulation; pending state is
        // discarded so subsequent Toggle calls before another Begin
        // return NoSession.
        var h = Harness.Build();
        var b = TestMod.Build("tests.menu.b", hotReload: true);
        h.Discoverer.Mods.Add(new DiscoveredModInfo(b.Manifest.Id, b.Manifest));
        h.Loader.RegisterLoaded(b);

        h.Controller.BeginEditing();
        h.Controller.Toggle("tests.menu.b");
        h.Controller.Cancel();

        h.Controller.IsEditing.Should().BeFalse();
        h.Pipeline.IsRunning.Should().BeTrue();
        h.Controller.Toggle("tests.menu.b").Should().Be(ToggleResult.NoSession);
    }

    // 5
    [Fact]
    public void Cancel_WithoutBeginEditing_NoOp()
    {
        // AD #6 — defensive Cancel on a fresh controller must NOT
        // call Resume (otherwise a stray UI Cancel could accidentally
        // start the scheduler).
        var h = Harness.Build();

        h.Controller.Cancel();

        h.Controller.IsEditing.Should().BeFalse();
        h.Pipeline.IsRunning.Should().BeFalse(
            "Cancel without an active session must not call Resume");
    }

    // 6
    [Fact]
    public void Toggle_OnHotReloadFalseCurrentlyActiveMod_ReturnsRejected()
    {
        // §9.6 — currently-active mod with hotReload=false cannot be
        // toggled mid-session. Toggle returns RejectedHotReloadDisabled
        // and the pending set is unchanged.
        var h = Harness.Build();
        var a = TestMod.Build("tests.menu.a", hotReload: false);
        h.RegisterAndApply(a);

        h.Controller.BeginEditing();
        ToggleResult result = h.Controller.Toggle("tests.menu.a");

        result.Should().Be(ToggleResult.RejectedHotReloadDisabled);
        IReadOnlyList<EditableModInfo> rows = h.Controller.GetEditableState();
        rows.Should().ContainSingle(r => r.ModId == "tests.menu.a")
            .Which.IsPendingActive.Should().BeTrue(
                "the pending set must not change on a rejected toggle");
    }

    // 7
    [Fact]
    public void Toggle_OnHotReloadFalseNotActiveMod_AllowedSinceFirstLoadIsNotReload()
    {
        // AD #5 — §9.6 "cannot be reloaded mid-session" + §2.2
        // "loads only at session start" combine to allow first-load
        // of a hotReload=false mod inside the editing session. The
        // restriction is on reload, not initial load.
        var h = Harness.Build();
        var b = TestMod.Build("tests.menu.b", hotReload: false);
        h.Discoverer.Mods.Add(new DiscoveredModInfo(b.Manifest.Id, b.Manifest));
        h.Loader.RegisterLoaded(b);

        h.Controller.BeginEditing();
        ToggleResult result = h.Controller.Toggle("tests.menu.b");

        result.Should().Be(ToggleResult.Toggled);
        IReadOnlyList<EditableModInfo> rows = h.Controller.GetEditableState();
        rows.Should().ContainSingle(r => r.ModId == "tests.menu.b")
            .Which.IsPendingActive.Should().BeTrue();
    }

    // 8
    [Fact]
    public void Toggle_OnHotReloadTrueMod_TogglesPendingSet()
    {
        // Pending-set mutation contract: Toggle adds when absent,
        // removes when present. Two consecutive Toggle calls revert
        // the row to its starting state (here: discovered-only,
        // initially not pending).
        var h = Harness.Build();
        var b = TestMod.Build("tests.menu.b", hotReload: true);
        h.Discoverer.Mods.Add(new DiscoveredModInfo(b.Manifest.Id, b.Manifest));
        h.Loader.RegisterLoaded(b);

        h.Controller.BeginEditing();

        h.Controller.Toggle("tests.menu.b").Should().Be(ToggleResult.Toggled);
        h.Controller.GetEditableState()
            .Should().ContainSingle(r => r.ModId == "tests.menu.b")
            .Which.IsPendingActive.Should().BeTrue();

        h.Controller.Toggle("tests.menu.b").Should().Be(ToggleResult.Toggled);
        h.Controller.GetEditableState()
            .Should().ContainSingle(r => r.ModId == "tests.menu.b")
            .Which.IsPendingActive.Should().BeFalse(
                "second toggle must revert to the starting state");
    }

    // 9
    [Fact]
    public void Toggle_WithoutBeginEditing_ReturnsNoSession()
    {
        // Defensive guard — UI calling Toggle outside an editing
        // session must get a typed signal, not a silent no-op.
        var h = Harness.Build();

        h.Controller.Toggle("anything").Should().Be(ToggleResult.NoSession);
    }

    // 10
    [Fact]
    public void Toggle_OnUnknownModId_ReturnsUnknownMod()
    {
        // Defensive guard for misrouted UI calls — neither in
        // active nor in discovered set.
        var h = Harness.Build();

        h.Controller.BeginEditing();
        h.Controller.Toggle("tests.menu.nonexistent").Should().Be(ToggleResult.UnknownMod);
    }

    // 11
    [Fact]
    public void CanToggle_ForHotReloadFalseCurrentlyActive_ReturnsFalse()
    {
        // §9.6 UI hint — the menu greys out the toggle button using
        // CanToggle, mirroring the defensive Toggle rejection above.
        var h = Harness.Build();
        var a = TestMod.Build("tests.menu.a", hotReload: false);
        h.RegisterAndApply(a);

        h.Controller.BeginEditing();

        h.Controller.CanToggle("tests.menu.a").Should().BeFalse();
    }

    // 12
    [Fact]
    public void CanToggle_ForHotReloadFalseDiscoveredOnly_ReturnsTrue()
    {
        // AD #5 mirror — first-load is allowed, so CanToggle must
        // return true for a discovered-only hotReload=false mod.
        var h = Harness.Build();
        var b = TestMod.Build("tests.menu.b", hotReload: false);
        h.Discoverer.Mods.Add(new DiscoveredModInfo(b.Manifest.Id, b.Manifest));
        h.Loader.RegisterLoaded(b);

        h.Controller.BeginEditing();

        h.Controller.CanToggle("tests.menu.b").Should().BeTrue();
    }

    // 13
    [Fact]
    public void CanToggle_WithoutSession_ReturnsFalse()
    {
        // Defensive default — no session means no row should be
        // toggle-able.
        var h = Harness.Build();

        h.Controller.CanToggle("anything").Should().BeFalse();
    }

    // 14
    [Fact]
    public void GetEditableState_ReturnsCombinedActiveAndDiscovered_WithFlags()
    {
        // Active rows render first; flags reflect session-start state
        // (active=true, pending=true initially) and discovered-only
        // state (active=false, pending=false). CanToggle is true for
        // both since both have hotReload=true.
        var h = Harness.Build();
        var a = TestMod.Build("tests.menu.a", hotReload: true);
        var b = TestMod.Build("tests.menu.b", hotReload: true);
        h.RegisterAndApply(a);
        h.Discoverer.Mods.Add(new DiscoveredModInfo(b.Manifest.Id, b.Manifest));
        h.Loader.RegisterLoaded(b);

        h.Controller.BeginEditing();
        IReadOnlyList<EditableModInfo> rows = h.Controller.GetEditableState();

        rows.Should().HaveCount(2);
        EditableModInfo activeRow = rows.Should().ContainSingle(r => r.ModId == "tests.menu.a").Subject;
        activeRow.IsCurrentlyActive.Should().BeTrue();
        activeRow.IsPendingActive.Should().BeTrue();
        activeRow.CanToggle.Should().BeTrue();

        EditableModInfo discRow = rows.Should().ContainSingle(r => r.ModId == "tests.menu.b").Subject;
        discRow.IsCurrentlyActive.Should().BeFalse();
        discRow.IsPendingActive.Should().BeFalse();
        discRow.CanToggle.Should().BeTrue();
    }

    // 15
    [Fact]
    public void GetEditableState_WithoutSession_Throws()
    {
        // Rendering rows without a session would be meaningless —
        // throw rather than return an empty list, since callers must
        // gate UI on IsEditing first.
        var h = Harness.Build();

        Action act = () => h.Controller.GetEditableState();

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("*BeginEditing*");
    }

    // 16
    [Fact]
    public void Commit_NoChanges_NoOpSuccess_Resumes()
    {
        // The trivial happy path — Begin then immediate Commit. No
        // UnloadMod, no Apply. Success path closes the session and
        // resumes the simulation.
        var h = Harness.Build();

        h.Controller.BeginEditing();
        CommitResult result = h.Controller.Commit();

        result.Success.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Warnings.Should().BeEmpty();
        result.NewlyActiveModIds.Should().BeEmpty();
        result.NewlyInactiveModIds.Should().BeEmpty();
        h.Pipeline.IsRunning.Should().BeTrue();
        h.Controller.IsEditing.Should().BeFalse();
    }

    // 17
    [Fact]
    public void Commit_AddOnly_CallsApply_NoUnload_Resumes()
    {
        // Adding a discovered-only mod runs Apply with that mod's
        // path; no UnloadMod call is issued because nothing was
        // removed from the active set.
        var h = Harness.Build();
        var b = TestMod.Build("tests.menu.b", hotReload: true);
        h.Discoverer.Mods.Add(new DiscoveredModInfo(b.Manifest.Id, b.Manifest));
        h.Loader.RegisterLoaded(b);

        h.Controller.BeginEditing();
        h.Controller.Toggle("tests.menu.b");
        CommitResult result = h.Controller.Commit();

        result.Success.Should().BeTrue();
        result.NewlyActiveModIds.Should().Contain("tests.menu.b");
        result.NewlyInactiveModIds.Should().BeEmpty();
        h.Pipeline.IsRunning.Should().BeTrue();
    }

    // 18
    [Fact]
    public void Commit_RemoveOnly_CallsUnloadMod_NoApply_Resumes()
    {
        // Removing a single active mod runs UnloadMod and skips
        // Apply (no add paths). NewlyInactive records the removal;
        // NewlyActive is empty because Apply was not called.
        var h = Harness.Build();
        var a = TestMod.Build("tests.menu.a", hotReload: true);
        h.RegisterAndApply(a);

        h.Controller.BeginEditing();
        h.Controller.Toggle("tests.menu.a");
        CommitResult result = h.Controller.Commit();

        result.Success.Should().BeTrue();
        result.NewlyActiveModIds.Should().BeEmpty();
        result.NewlyInactiveModIds.Should().Contain("tests.menu.a");
        h.Pipeline.IsRunning.Should().BeTrue();
    }

    // 19
    [Fact]
    public void Commit_AddAndRemove_BothCalled_WarningsAccumulated()
    {
        // Mixed diff — both UnloadMod and Apply run. Result
        // aggregates both sides: NewlyInactive has the removed mod,
        // NewlyActive has the added one.
        var h = Harness.Build();
        var a = TestMod.Build("tests.menu.a", hotReload: true);
        var b = TestMod.Build("tests.menu.b", hotReload: true);
        h.RegisterAndApply(a);
        h.Discoverer.Mods.Add(new DiscoveredModInfo(b.Manifest.Id, b.Manifest));
        h.Loader.RegisterLoaded(b);

        h.Controller.BeginEditing();
        h.Controller.Toggle("tests.menu.a");
        h.Controller.Toggle("tests.menu.b");
        CommitResult result = h.Controller.Commit();

        result.Success.Should().BeTrue();
        result.NewlyActiveModIds.Should().Contain("tests.menu.b");
        result.NewlyInactiveModIds.Should().Contain("tests.menu.a");
        h.Pipeline.IsRunning.Should().BeTrue();
    }

    // 20
    [Fact]
    public void Commit_ApplyValidationFailure_StaysPaused_SessionOpen_ReturnsErrors()
    {
        // AD #4 — Apply validation failure (here: missing required
        // dep) leaves the simulation paused and the session open so
        // the user can fix and retry. Errors flow through Result.
        var h = Harness.Build();
        var bad = TestMod.BuildWithMissingDep(
            id: "tests.menu.bad", missingDepId: "tests.menu.absent");
        h.Discoverer.Mods.Add(new DiscoveredModInfo(bad.Manifest.Id, bad.Manifest));
        h.Loader.RegisterLoaded(bad);

        h.Controller.BeginEditing();
        h.Controller.Toggle("tests.menu.bad");
        CommitResult result = h.Controller.Commit();

        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.MissingDependency);
        h.Pipeline.IsRunning.Should().BeFalse(
            "AD #4 — failed Apply must leave the simulation paused");
        h.Controller.IsEditing.Should().BeTrue(
            "AD #4 — failed commit must leave the editing session open");
    }

    // 21
    [Fact]
    public void Commit_WithoutBeginEditing_Throws()
    {
        // Commit on a fresh controller is a programming error —
        // throw rather than silently no-op so callers cannot drift
        // out of the lifecycle invariant.
        var h = Harness.Build();

        Action act = () => h.Controller.Commit();

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("*BeginEditing*");
    }

    // 22
    [Fact]
    public void Commit_PostFailureRetry_AfterFixingPending_Succeeds()
    {
        // Recovery flow per AD #4 — after a failed Commit, the user
        // un-toggles the offending mod and Commit again succeeds
        // (empty diff, success path).
        var h = Harness.Build();
        var bad = TestMod.BuildWithMissingDep(
            id: "tests.menu.bad", missingDepId: "tests.menu.absent");
        h.Discoverer.Mods.Add(new DiscoveredModInfo(bad.Manifest.Id, bad.Manifest));
        h.Loader.RegisterLoaded(bad);

        h.Controller.BeginEditing();
        h.Controller.Toggle("tests.menu.bad");
        CommitResult firstAttempt = h.Controller.Commit();
        firstAttempt.Success.Should().BeFalse(
            "test setup must produce a failing first commit");

        h.Controller.Toggle("tests.menu.bad");  // remove offending mod
        CommitResult retry = h.Controller.Commit();

        retry.Success.Should().BeTrue();
        h.Pipeline.IsRunning.Should().BeTrue();
        h.Controller.IsEditing.Should().BeFalse();
    }

    // --- Harness ------------------------------------------------------------
    // Inline copy of M72UnloadChainTests.Harness extended with the
    // controller and a fake discoverer. Future consolidation: extract
    // a shared seam under tests/DualFrontier.Modding.Tests/Pipeline/
    // when M7.5.B/M8 force a third copy.

    private sealed class Harness
    {
        public ModLoader Loader { get; }
        public ModRegistry Registry { get; }
        public ModIntegrationPipeline Pipeline { get; }
        public FakeModDiscoverer Discoverer { get; }
        public ModMenuController Controller { get; }

        private Harness(
            ModLoader loader,
            ModRegistry registry,
            ModIntegrationPipeline pipeline,
            FakeModDiscoverer discoverer,
            ModMenuController controller)
        {
            Loader = loader;
            Registry = registry;
            Pipeline = pipeline;
            Discoverer = discoverer;
            Controller = controller;
        }

        public static Harness Build()
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
            var pipeline = new ModIntegrationPipeline(
                loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());
            var discoverer = new FakeModDiscoverer();
            var controller = new ModMenuController(pipeline, discoverer);
            return new Harness(loader, registry, pipeline, discoverer, controller);
        }

        public void RegisterAndApply(LoadedMod mod)
        {
            Loader.RegisterLoaded(mod);
            PipelineResult result = Pipeline.Apply(new[] { mod.ModId });
            result.Success.Should().BeTrue(
                $"setup mod '{mod.ModId}' must apply cleanly " +
                $"(errors: {string.Join("; ", System.Linq.Enumerable.Select(result.Errors, e => e.Message))})");
        }
    }

    private sealed class FakeModDiscoverer : IModDiscoverer
    {
        public List<DiscoveredModInfo> Mods { get; } = new();

        public IReadOnlyList<DiscoveredModInfo> Discover() => Mods;
    }

    // --- Test fixture mods --------------------------------------------------

    private static class TestMod
    {
        public static LoadedMod Build(string id, bool hotReload)
        {
            var manifest = new ModManifest
            {
                Id = id,
                Name = id,
                Version = "1.0.0",
                Author = "Test",
                HotReload = hotReload,
            };
            var context = new ModLoadContext(id);
            return new LoadedMod(id, manifest, new EmptyMod(), context, Array.Empty<Type>());
        }

        public static LoadedMod BuildWithMissingDep(string id, string missingDepId)
        {
            var manifest = new ModManifest
            {
                Id = id,
                Name = id,
                Version = "1.0.0",
                Author = "Test",
                HotReload = true,
                Dependencies = new[] { ModDependency.Required(missingDepId) },
            };
            var context = new ModLoadContext(id);
            return new LoadedMod(id, manifest, new EmptyMod(), context, Array.Empty<Type>());
        }
    }

    private sealed class EmptyMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
