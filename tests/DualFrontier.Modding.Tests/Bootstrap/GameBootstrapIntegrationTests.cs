using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using DualFrontier.Application.Modding;
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
