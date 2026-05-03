using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Application.Loop;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Bootstrap;

/// <summary>
/// M7.5.B.1 — production-side smoke coverage of
/// <see cref="GameBootstrap.CreateLoop"/>. The harness is
/// <c>CreateLoop</c> itself: production wiring is the unit under test,
/// no separate harness is needed. Tests construct the full simulation
/// graph + modding stack via the same code path
/// <see cref="DualFrontier.Presentation.Nodes.GameRoot"/> consumes in
/// production, then assert the returned <see cref="GameContext"/> shape
/// and the controller's behavior under representative <c>modsRoot</c>
/// inputs (default literal "mods", empty temp dir, non-existent path,
/// temp dir containing a single fixture manifest).
///
/// Heavier mod-loading flows are covered by the controller-level suite
/// (<c>ModMenuControllerTests</c>) and the pipeline-level suites; the
/// scope here is "the production constructor chain produces a working
/// controller wired to the same scheduler/services/pipeline state."
///
/// Out of scope (M7.5.B.2): Godot UI scene smoke tests, manual click-
/// through of the menu → controller → pipeline → scheduler path, real
/// end-to-end mod toggling through <c>mods/DualFrontier.Mod.Example/</c>.
/// </summary>
public sealed class GameBootstrapIntegrationTests
{
    [Fact]
    public void CreateLoop_ReturnsContextWithLoopAndController()
    {
        // Smoke test of the new GameContext return shape: every
        // member must be populated by CreateLoop. Locks AD #1
        // (atomic refactor of the bootstrap signature) at the API
        // surface so an accidental future change to the return type
        // is visible.
        GameContext context = GameBootstrap.CreateLoop(new PresentationBridge());

        context.Should().NotBeNull();
        context.Loop.Should().NotBeNull();
        context.Controller.Should().NotBeNull();
    }

    [Fact]
    public void CreateLoop_ReturnedController_BeginEditingSucceedsAndPauses()
    {
        // The pipeline starts in its default paused state per M7.1's
        // load-bearing default; calling BeginEditing on the controller
        // must succeed and flip IsEditing to true. The controller's
        // internal Pause call is not directly observable from the
        // GameContext surface (no pipeline accessor), so the contract
        // we lock here is the controller-side observable side-effect:
        // BeginEditing does not throw and IsEditing becomes true.
        GameContext context = GameBootstrap.CreateLoop(new PresentationBridge());

        Action act = () => context.Controller.BeginEditing();

        act.Should().NotThrow();
        context.Controller.IsEditing.Should().BeTrue();
    }

    [Fact]
    public void CreateLoop_WithEmptyModsRoot_GetEditableStateReturnsEmpty()
    {
        // Production wiring through the discoverer: an existing-but-
        // empty modsRoot directory must produce an empty editing
        // state with no active and no discovered mods. Locks the
        // happy-path empty case for first-launch installations where
        // the user has not yet placed any mods under mods/.
        using var temp = TempDir.New();

        GameContext context = GameBootstrap.CreateLoop(
            new PresentationBridge(), modsRoot: temp.Path);
        context.Controller.BeginEditing();

        IReadOnlyList<EditableModInfo> rows = context.Controller.GetEditableState();
        rows.Should().BeEmpty();
    }

    [Fact]
    public void CreateLoop_WithModsRootContainingFixture_GetEditableStateReturnsFixture()
    {
        // End-to-end discovery through the production wiring: a
        // single valid manifest under modsRoot becomes one
        // EditableModInfo row with IsCurrentlyActive=false (no mods
        // were applied at bootstrap), IsPendingActive=false (user
        // has not toggled), CanToggle=true (it is not currently
        // active so §9.6 hot-reload restriction does not apply).
        using var temp = TempDir.New();
        const string id = "tests.bootstrap.fixture";
        WriteValidManifest(Path.Combine(temp.Path, id), id);

        GameContext context = GameBootstrap.CreateLoop(
            new PresentationBridge(), modsRoot: temp.Path);
        context.Controller.BeginEditing();

        IReadOnlyList<EditableModInfo> rows = context.Controller.GetEditableState();
        rows.Should().ContainSingle();
        EditableModInfo row = rows[0];
        row.ModId.Should().Be(id);
        row.IsCurrentlyActive.Should().BeFalse();
        row.IsPendingActive.Should().BeFalse();
        row.CanToggle.Should().BeTrue();
    }

    [Fact]
    public void CreateLoop_WithNonExistentModsRoot_GetEditableStateReturnsEmpty_NoThrow()
    {
        // First-launch safety: if the user has no mods/ directory
        // at all, CreateLoop must succeed and the discoverer must
        // return an empty list rather than throw. DefaultModDiscoverer
        // (M7.5.A) handles this case by returning an empty list when
        // Directory.Exists is false. Locks the production wiring
        // through that no-throw path.
        string nonexistent = Path.Combine(
            Path.GetTempPath(), $"dfm-bootstrap-nonexistent-{Guid.NewGuid():N}");
        Directory.Exists(nonexistent).Should().BeFalse(
            "Guid.NewGuid path must not collide with an existing directory");

        GameContext context = null!;
        Action createAct = () => context = GameBootstrap.CreateLoop(
            new PresentationBridge(), modsRoot: nonexistent);
        createAct.Should().NotThrow();

        context.Controller.BeginEditing();
        IReadOnlyList<EditableModInfo> rows = context.Controller.GetEditableState();
        rows.Should().BeEmpty();
    }

    [Fact]
    public void CreateLoop_DefaultModsRoot_IsLiteralStringMods()
    {
        // AD #3 lock — the default modsRoot must be the literal
        // string "mods" (not "./mods", not absolute). Production
        // Godot launches with cwd = project root, so this resolves
        // to <project>/mods/. Reflection on the parameter's
        // DefaultValue keeps an accidental refactor (e.g. someone
        // changing the default to an absolute path during a quick
        // local test) from silently shipping.
        MethodInfo method = typeof(GameBootstrap).GetMethod(
            nameof(GameBootstrap.CreateLoop),
            BindingFlags.Public | BindingFlags.Static)!;
        method.Should().NotBeNull(
            "CreateLoop must remain the public bootstrap entry point");

        ParameterInfo modsRootParam = method.GetParameters()[1];
        modsRootParam.Name.Should().Be("modsRoot");
        modsRootParam.HasDefaultValue.Should().BeTrue();
        modsRootParam.DefaultValue.Should().Be("mods");
    }

    [Fact]
    public void CreateLoop_ReturnedLoop_StartStopRoundTripsCleanly()
    {
        // Regression floor on the new GameContext shape — the loop
        // must behave identically to the previous direct-return
        // shape on the simulation side. Start spins up the background
        // thread; Stop cancels and joins. Zero exception is the
        // observable contract.
        GameContext context = GameBootstrap.CreateLoop(new PresentationBridge());

        Action roundTrip = () =>
        {
            context.Loop.Start();
            context.Loop.Stop();
        };

        roundTrip.Should().NotThrow();
    }

    [Fact]
    public void CreateLoop_Spawns10PawnsByDefault()
    {
        // Real-pawn-data housekeeping — the production factory emits a
        // PawnSpawnedCommand per colonist via the Pawns bus → bridge
        // subscription wired in CreateLoop. Locks the new 10-pawn
        // baseline (was 3 before housekeeping) at the bridge surface.
        var bridge = new PresentationBridge();
        var observedSpawns = new List<PawnSpawnedCommand>();

        GameContext context = GameBootstrap.CreateLoop(bridge);
        bridge.DrainCommands(c =>
        {
            if (c is PawnSpawnedCommand sp) observedSpawns.Add(sp);
        });

        observedSpawns.Should().HaveCount(10);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateLoop_RunningLoop_PawnStateCommandCarriesRealName()
    {
        // PawnStateReporterSystem reads IdentityComponent.Name; the
        // RandomPawnFactory always populates that field with a forename
        // + surname pair, so the published PawnStateCommand must carry
        // a non-empty space-separated name. Locks the end-to-end name
        // wiring (component → reporter → event → bridge → command).
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);

        try
        {
            context.Loop.Start();
            Thread.Sleep(500);
        }
        finally
        {
            context.Loop.Stop();
        }

        var stateCommands = new List<PawnStateCommand>();
        bridge.DrainCommands(c =>
        {
            if (c is PawnStateCommand ps) stateCommands.Add(ps);
        });

        stateCommands.Should().NotBeEmpty(
            "expected at least one PawnStateCommand publish during the loop window");
        foreach (var ps in stateCommands)
        {
            ps.Name.Should().NotBeNullOrWhiteSpace(
                "PawnStateCommand carries empty Name — IdentityComponent not wired correctly");
            ps.Name.Should().Contain(" ", "names follow forename + surname format");
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateLoop_RunningLoop_PawnStateCommandCarriesTopSkills()
    {
        // PawnStateReporterSystem computes top-3 skills from
        // SkillsComponent.Levels. The RandomPawnFactory populates all
        // 13 SkillKind values, so every PawnStateCommand must carry
        // exactly 3 entries in descending level order.
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);

        try
        {
            context.Loop.Start();
            Thread.Sleep(500);
        }
        finally
        {
            context.Loop.Stop();
        }

        var stateCommands = new List<PawnStateCommand>();
        bridge.DrainCommands(c =>
        {
            if (c is PawnStateCommand ps) stateCommands.Add(ps);
        });

        stateCommands.Should().NotBeEmpty(
            "expected at least one PawnStateCommand publish during the loop window");
        foreach (var ps in stateCommands)
        {
            ps.TopSkills.Should().NotBeNull();
            ps.TopSkills.Should().HaveCount(3);
            for (int i = 0; i < ps.TopSkills.Count - 1; i++)
            {
                ps.TopSkills[i].Level.Should().BeGreaterThanOrEqualTo(
                    ps.TopSkills[i + 1].Level,
                    "TopSkills must be sorted descending by Level");
            }
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge()
    {
        // TICK display housekeeping — locks the production publishing
        // path GameLoop → PresentationBridge → (RenderCommandDispatcher
        // → GameHUD.SetTick on the Godot main thread). Construct the
        // production context, run the loop briefly, then drain the
        // bridge and assert at least two TickAdvancedCommand publishes
        // observed with strictly monotonic Tick values. The window is
        // generous (250 ms targets ~6 ticks at 30 TPS) but the
        // assertion floor is just >= 2 so slow-CI does not flake.
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);

        try
        {
            context.Loop.Start();
            Thread.Sleep(250);
        }
        finally
        {
            context.Loop.Stop();
        }

        int tickCommandCount = 0;
        int lastTickValue = -1;
        bridge.DrainCommands(cmd =>
        {
            if (cmd is TickAdvancedCommand tac)
            {
                tickCommandCount++;
                lastTickValue = tac.Tick;
            }
        });

        tickCommandCount.Should().BeGreaterThanOrEqualTo(2,
            $"expected at least 2 TickAdvancedCommand publishes, observed {tickCommandCount}");
        // _ticks.CurrentTick advances exactly once per ExecuteTick, so the
        // last published value must be at least count - 1 (allowing for
        // tick 0 to be the first publish).
        lastTickValue.Should().BeGreaterThanOrEqualTo(tickCommandCount - 1,
            $"expected monotonic tick values, last={lastTickValue}, count={tickCommandCount}");
    }

    [Fact]
    public void MenuFlow_OpenCommitClose_LeavesEditingFalse()
    {
        // Apply button success path — Commit closes the session. M7.5.B.2's
        // ModMenuPanel.OnApplyPressed sets Visible = false on the success
        // branch; this test locks the controller-level invariant the UI
        // relies on (IsEditing flips from true → false on Commit success).
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);

        context.Controller.BeginEditing();
        context.Controller.IsEditing.Should().BeTrue();

        CommitResult commit = context.Controller.Commit();
        commit.Success.Should().BeTrue();
        context.Controller.IsEditing.Should().BeFalse();
    }

    [Fact]
    public void MenuFlow_OpenCancelClose_LeavesEditingFalse()
    {
        // Cancel button path — Cancel closes the session. M7.5.B.2's
        // ModMenuPanel.CloseAndCancel calls controller.Cancel and then
        // sets Visible = false; this test locks the controller-level
        // invariant (IsEditing flips from true → false on Cancel).
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);

        context.Controller.BeginEditing();
        context.Controller.IsEditing.Should().BeTrue();

        context.Controller.Cancel();
        context.Controller.IsEditing.Should().BeFalse();
    }

    [Fact]
    public void MenuFlow_OpenWithoutCommitOrCancel_StaysEditing()
    {
        // User keeps menu open without applying — session must persist
        // so the UI continues to allow Toggle calls and re-renders of
        // GetEditableState across user interactions. Locks the
        // editing-stays-live invariant M7.5.B.2's panel relies on
        // between OpenAndBegin and the next Apply/Cancel.
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);

        context.Controller.BeginEditing();
        context.Controller.IsEditing.Should().BeTrue();

        IReadOnlyList<EditableModInfo> rows = context.Controller.GetEditableState();
        rows.Should().NotBeNull();
        context.Controller.IsEditing.Should().BeTrue();
    }

    [Fact]
    public void MenuFlow_BeginEditing_PausesGameLoop()
    {
        // Second-housekeeping wiring (MOD_OS_ARCHITECTURE §9.2 step 1) —
        // GameBootstrap.CreateLoop wires controller.OnEditingBegan to
        // loop.SetPaused(true). Locks the controller-driven simulation
        // pause: the user-visible behavior the F5 verification surfaced
        // as missing (TICK counter kept advancing with the menu open).
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);

        context.Loop.IsPaused.Should().BeFalse(
            "loop is unpaused at construction");

        context.Controller.BeginEditing();

        context.Loop.IsPaused.Should().BeTrue(
            "BeginEditing fires OnEditingBegan which calls loop.SetPaused(true) " +
            "per MOD_OS_ARCHITECTURE §9.2 step 1");
    }

    [Fact]
    public void MenuFlow_Cancel_ResumesGameLoop()
    {
        // Cancel path — symmetric counterpart to the BeginEditing pause.
        // Locks loop.IsPaused flipping back to false on Cancel so closing
        // the menu without Apply restores tick advance.
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);
        context.Controller.BeginEditing();
        context.Loop.IsPaused.Should().BeTrue(
            "loop should be paused after BeginEditing (precondition for this test)");

        context.Controller.Cancel();

        context.Loop.IsPaused.Should().BeFalse(
            "Cancel fires OnEditingEnded which calls loop.SetPaused(false)");
    }

    [Fact]
    public void MenuFlow_CommitSuccess_ResumesGameLoop()
    {
        // Apply success path — Commit on a clean session is a no-op
        // success per M7.5.A test 16; OnEditingEnded must fire on the
        // success branch only (failed-commit-stays-paused is covered
        // by M7.5.A's Commit_ValidationFailure_LeavesSessionOpen plus
        // the controller's success-only RaiseHook placement).
        var bridge = new PresentationBridge();
        GameContext context = GameBootstrap.CreateLoop(bridge);
        context.Controller.BeginEditing();
        context.Loop.IsPaused.Should().BeTrue(
            "loop should be paused after BeginEditing (precondition for this test)");

        CommitResult result = context.Controller.Commit();

        result.Success.Should().BeTrue(
            "no toggles applied — Commit on a clean session is a no-op success " +
            "per M7.5.A test 16");
        context.Loop.IsPaused.Should().BeFalse(
            "successful Commit fires OnEditingEnded which calls loop.SetPaused(false)");
    }

    [Fact]
    public void DefaultModDiscoverer_FindsAll6VanillaSkeletonsInProductionModsRoot()
    {
        // M8.1 — vanilla mod skeletons are discoverable from the
        // production mods/ directory. Locks the 6-mod set as an
        // architectural invariant per MOD_OS_ARCHITECTURE v1.5 §1.3:
        // 5 regular slices (Combat, Magic, Inventory, Pawn, World) +
        // 1 shared (Vanilla.Core), alongside the preserved ExampleMod.
        // Test resolves the production mods/ directory via repo-root
        // walk so the result is independent of the test runner cwd.
        string modsRoot = Path.Combine(FindRepoRoot(), "mods");
        var discoverer = new DefaultModDiscoverer(modsRoot);

        IReadOnlyList<DiscoveredModInfo> discovered = discoverer.Discover();

        discovered.Should().HaveCount(7,
            "ExampleMod + 6 vanilla skeletons (5 regular + 1 shared) per §1.3");

        List<string> ids = discovered.Select(d => d.Manifest.Id).ToList();
        ids.Should().Contain("dualfrontier.example");
        ids.Should().Contain("dualfrontier.vanilla.core");
        ids.Should().Contain("dualfrontier.vanilla.combat");
        ids.Should().Contain("dualfrontier.vanilla.magic");
        ids.Should().Contain("dualfrontier.vanilla.inventory");
        ids.Should().Contain("dualfrontier.vanilla.pawn");
        ids.Should().Contain("dualfrontier.vanilla.world");

        // Vanilla.Core is the shared mod with no IMod entry point.
        DiscoveredModInfo core = discovered.Single(
            d => d.Manifest.Id == "dualfrontier.vanilla.core");
        core.Manifest.Kind.Should().Be(ModKind.Shared,
            "Vanilla.Core is the pure type vendor per §1.2");
        core.Manifest.EntryAssembly.Should().BeEmpty(
            "shared mods must have empty entryAssembly per §2.2");
        core.Manifest.EntryType.Should().BeEmpty(
            "shared mods must have empty entryType per §2.2");

        // Each regular vanilla mod is regular kind and depends on Vanilla.Core.
        foreach (string slice in new[] { "combat", "magic", "inventory", "pawn", "world" })
        {
            string id = $"dualfrontier.vanilla.{slice}";
            DiscoveredModInfo mod = discovered.Single(d => d.Manifest.Id == id);
            mod.Manifest.Kind.Should().Be(ModKind.Regular,
                $"vanilla.{slice} is a regular mod per §1.3");
            mod.Manifest.Dependencies.Should().ContainSingle(
                $"vanilla.{slice} declares only the shared-Core dependency in M8.1");
            mod.Manifest.Dependencies[0].ModId.Should().Be("dualfrontier.vanilla.core",
                $"vanilla.{slice} depends on the shared Vanilla.Core");
        }
    }

    private static string FindRepoRoot()
    {
        // Walks up from the test assembly's runtime directory until
        // a directory containing DualFrontier.sln is found. The sentinel
        // file pins the repo root unambiguously regardless of where
        // bin/ is configured. Same pattern as M74BuildPipelineTests.
        DirectoryInfo? dir = new(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException(
            $"Could not locate DualFrontier.sln walking up from {AppContext.BaseDirectory}");
    }

    private static void WriteValidManifest(string dir, string id)
    {
        Directory.CreateDirectory(dir);
        string json = "{ \"id\": \"" + id + "\", \"name\": \"Bootstrap Fixture\", " +
            "\"version\": \"1.0.0\" }";
        File.WriteAllText(Path.Combine(dir, "mod.manifest.json"), json);
    }

    private sealed class TempDir : IDisposable
    {
        public string Path { get; }

        private TempDir(string path) { Path = path; }

        public static TempDir New()
        {
            string dir = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                $"dfm-bootstrap-{Guid.NewGuid():N}");
            Directory.CreateDirectory(dir);
            return new TempDir(dir);
        }

        public void Dispose()
        {
            try { Directory.Delete(Path, recursive: true); } catch { /* best effort */ }
        }
    }
}
