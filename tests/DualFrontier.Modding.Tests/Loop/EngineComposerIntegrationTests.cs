using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Application.Distribution;
using DualFrontier.Application.Loop;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Distribution;
using DualFrontier.Contracts.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Loop;

/// <summary>
/// Serializes the GameLoop integration tests against the rest of the Modding assembly. The
/// running-loop tests start the real GameLoop background thread and assert on commands published
/// within a bounded wall budget; under xUnit intra-assembly parallelism the CPU-heavy Modding
/// classes can starve that thread and empty a fixed sleep window. DisableParallelization keeps
/// this collection from running concurrently with other collections so the loop always gets a
/// quiet CPU (F-10 #2 fix; pairs with the poll-until-condition bodies below that replaced the
/// former fixed Thread.Sleep windows).
/// </summary>
[CollectionDefinition("GameLoopSerial", DisableParallelization = true)]
public sealed class GameLoopSerialCollection
{
}

/// <summary>
/// Production-side coverage of <see cref="EngineComposer.CreateSession"/> — the composition root
/// the Launcher's Program consumes. The harness is CreateSession itself: production wiring is the
/// unit under test.
///
/// <para>
/// <b>What changed at W4.</b> This suite replaces the GameBootstrap one, and it is not a rename.
/// The predecessor composed a session that already held the colony: 21 component registrations,
/// two spawn factories, ten gameplay systems and five bus-to-render subscriptions, all named by
/// the engine. Its tests could therefore assert on colonists straight out of CreateSession, and
/// one of them did. This composer builds ENGINE parts only and then loads whatever mods the
/// distribution manifest declares, so the equivalent assertion here is the inverse — an empty
/// root set produces an empty world — and the colony is asserted where it now comes from, in the
/// scenario mod's wave gate.
/// </para>
///
/// <para>
/// The two tests that asserted on PawnStateCommand went with the engine subscription that
/// produced it. Their subject moved DOWN a level, onto the domain event the reporter system
/// publishes, which is where the data actually exists; see the scenario wave gate.
/// </para>
///
/// <para>
/// Heavier mod-loading flows are covered by the controller-level suite (ModMenuControllerTests)
/// and the pipeline-level suites. The scope here is "the production constructor chain produces a
/// working session wired to the same scheduler / services / pipeline state, and refuses a
/// distribution it cannot honour".
/// </para>
/// </summary>
[Collection("GameLoopSerial")]
public sealed class EngineComposerIntegrationTests
{
    // ── the distribution under test ───────────────────────────────────────────

    private static readonly ScenarioConfig SmallScenario = new(
        Id: "composer-tests",
        WorldSeed: 0,
        MapWidth: 40,
        MapHeight: 40,
        ObstacleCount: 20,
        ObstacleSeed: 42,
        FactorySeed: 42,
        ItemFactorySeed: 43,
        Counts: new ScenarioCounts(Pawns: 6, Food: 4, Water: 2, Beds: 2, Decorations: 1));

    private static DistributionManifest Manifest(params string[] rootMods) => new(
        ManifestVersion: DistributionManifest.SupportedVersion,
        Product: new ProductInfo("tests.composer", "Composer Tests", "1.0.0"),
        RootMods: rootMods,
        Scenario: SmallScenario,
        AssetRoots: Array.Empty<string>(),
        SaveNamespace: "tests.composer",
        MinEngineCapabilities: Array.Empty<string>());

    private const string ScenarioAssembly = "DualFrontier.Mod.Vanilla.Scenario";
    private const string ScenarioModId = "dualfrontier.vanilla.scenario";

    private static EngineSession Compose(
        Distribution d, PresentationBridge? bridge = null, params string[] rootMods)
        => EngineComposer.CreateSession(bridge ?? new PresentationBridge(), Manifest(rootMods), d.Root);

    /// <summary>
    /// A throwaway distribution root. The composer derives the mods root from it, so a test that
    /// wants an empty, a populated, or an absent mods directory says so by shaping this.
    /// </summary>
    private sealed class Distribution : IDisposable
    {
        public string Root { get; }
        public string ModsRoot => Path.Combine(Root, EngineComposer.ModsDirectoryName);

        private Distribution(string root) { Root = root; }

        /// <summary>A root whose mods/ directory exists.</summary>
        public static Distribution WithModsDirectory()
        {
            Distribution d = New();
            Directory.CreateDirectory(d.ModsRoot);
            return d;
        }

        /// <summary>A root with NO mods/ directory at all — the first-launch shape.</summary>
        public static Distribution WithoutModsDirectory() => New();

        private static Distribution New()
        {
            string dir = Path.Combine(Path.GetTempPath(), $"df-composer-{Guid.NewGuid():N}");
            Directory.CreateDirectory(dir);
            return new Distribution(dir);
        }

        /// <summary>Writes a discoverable manifest-only mod directory (no assembly).</summary>
        public void WriteManifestOnlyMod(string directoryName, string id)
        {
            string dir = Path.Combine(ModsRoot, directoryName);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "mod.manifest.json"), ManifestJson(id));
        }

        public static string ManifestJson(string id) =>
            "{ \"manifestVersion\": \"3\", \"id\": \"" + id +
            "\", \"name\": \"Composer Fixture\", \"version\": \"1.0.0\" }";

        /// <summary>Copies a mod this test project's build deployed into its Fixtures/ tree.</summary>
        public void CopyDeployedFixture(string assemblyName)
        {
            string source = Path.Combine(AppContext.BaseDirectory, "Fixtures", assemblyName);
            Directory.Exists(source).Should().BeTrue(
                $"fixture '{assemblyName}' must have been deployed into Fixtures/ by its own build");

            string target = Path.Combine(ModsRoot, assemblyName);
            Directory.CreateDirectory(target);
            foreach (string file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)), overwrite: true);
        }

        public void Dispose()
        {
            try { Directory.Delete(Root, recursive: true); } catch { /* best effort */ }
        }
    }

    // ── session shape ─────────────────────────────────────────────────────────

    [Fact]
    public void CreateSession_ReturnsSessionWithLoopAndController()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);

        session.Should().NotBeNull();
        session.Loop.Should().NotBeNull();
        session.Controller.Should().NotBeNull();
    }

    [Fact]
    public void CreateSession_ReturnedController_BeginEditingSucceedsAndPauses()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);

        Action act = () => session.Controller.BeginEditing();

        act.Should().NotThrow();
        session.Controller.IsEditing.Should().BeTrue();
    }

    [Fact]
    public void CreateSession_ReturnedLoop_StartStopRoundTripsCleanly()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);

        Action roundTrip = () =>
        {
            session.Loop.Start();
            session.Loop.Stop();
        };

        roundTrip.Should().NotThrow();
    }

    [Fact]
    public void CreateSession_WithNoRootMods_DrawsNothing()
    {
        // The inverse of the assertion this replaces. The predecessor spawned 50 colonists from
        // inside the composition root and published one render command each, so the bridge was
        // already full before the loop ran. The composer knows no content at all: with nothing
        // declared, nothing is loaded, nothing is seeded and nothing is drawn.
        var bridge = new PresentationBridge();
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d, bridge);

        var observed = new List<IRenderCommand>();
        bridge.DrainCommands(observed.Add);

        observed.Should().BeEmpty(
            "the engine composes no content; every entity in the world arrives through a mod");
    }

    // ── the mods root ─────────────────────────────────────────────────────────

    [Fact]
    public void CreateSession_WithEmptyModsRoot_GetEditableStateReturnsEmpty()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);
        session.Controller.BeginEditing();

        session.Controller.GetEditableState().Should().BeEmpty();
    }

    [Fact]
    public void CreateSession_WithModsRootContainingFixture_GetEditableStateReturnsFixture()
    {
        using var d = Distribution.WithModsDirectory();
        const string id = "tests.composer.fixture";
        d.WriteManifestOnlyMod("tests.composer.fixture", id);

        using EngineSession session = Compose(d);
        session.Controller.BeginEditing();

        IReadOnlyList<EditableModInfo> rows = session.Controller.GetEditableState();
        rows.Should().ContainSingle();
        rows[0].ModId.Should().Be(id);
        rows[0].IsCurrentlyActive.Should().BeFalse("no mod was declared in the root set");
        rows[0].IsPendingActive.Should().BeFalse();
        rows[0].CanToggle.Should().BeTrue();
    }

    [Fact]
    public void CreateSession_WithNoModsDirectory_GetEditableStateReturnsEmpty_NoThrow()
    {
        // First-launch safety: an installation with no mods/ directory must still compose.
        using var d = Distribution.WithoutModsDirectory();
        Directory.Exists(d.ModsRoot).Should().BeFalse("this test's premise is that it is absent");

        EngineSession session = null!;
        Action compose = () => session = Compose(d);
        compose.Should().NotThrow();

        using (session)
        {
            session.Controller.BeginEditing();
            session.Controller.GetEditableState().Should().BeEmpty();
        }
    }

    [Fact]
    public void TheModsRootIsTheModsDirectoryOfTheDistributionRoot()
    {
        // The predecessor asserted, by reflection, that a modsRoot PARAMETER defaulted to the
        // literal "mods" — a default that only resolved correctly when the process happened to be
        // launched from the right working directory. There is no such parameter now: the root is
        // derived from wherever the manifest was found. This pins both halves of that derivation,
        // the name and the fact that only that one directory is searched.
        EngineComposer.ModsDirectoryName.Should().Be("mods");

        using var d = Distribution.WithModsDirectory();
        d.WriteManifestOnlyMod("inside", "tests.composer.inside");

        // A manifest directory sitting beside mods/ rather than inside it must not be found.
        string stray = Path.Combine(d.Root, "outside");
        Directory.CreateDirectory(stray);
        File.WriteAllText(
            Path.Combine(stray, "mod.manifest.json"),
            Distribution.ManifestJson("tests.composer.outside"));

        using EngineSession session = Compose(d);
        session.Controller.BeginEditing();

        session.Controller.GetEditableState().Select(r => r.ModId)
            .Should().Equal(
                new[] { "tests.composer.inside" },
                "discovery reads <distributionRoot>/mods and nothing else");
    }

    // ── the boot load ─────────────────────────────────────────────────────────

    [Fact]
    public void CreateSession_LoadsEveryRootModTheManifestDeclares()
    {
        // The wave's headline behaviour: production loads mods at boot. Before W4 it loaded none
        // — the pipeline was fully constructed and never asked to apply anything, because its
        // only call site sat behind a menu the Launcher never opened.
        using var d = Distribution.WithModsDirectory();
        d.CopyDeployedFixture(ScenarioAssembly);

        using EngineSession session = Compose(d, bridge: null, ScenarioModId);
        session.Controller.BeginEditing();

        IReadOnlyList<EditableModInfo> rows = session.Controller.GetEditableState();
        rows.Should().ContainSingle();
        rows[0].ModId.Should().Be(ScenarioModId);
        rows[0].IsCurrentlyActive.Should().BeTrue(
            "a declared root mod is loaded during composition, not left for the menu to apply");
    }

    [Fact]
    public void CreateSession_RefusesWhenADeclaredRootModIsAbsent()
    {
        // A distribution missing part of its root set is not a degraded distribution, it is a
        // broken one. The menu's warn-and-continue path exists for optional content.
        using var d = Distribution.WithModsDirectory();

        Action compose = () => Compose(d, bridge: null, "dualfrontier.absent");

        compose.Should().Throw<InvalidOperationException>()
            .WithMessage("*dualfrontier.absent*",
                "the refusal names the id the DISTRIBUTION declared, not a path derived from it");
    }

    [Fact]
    public void CreateSession_RefusesWhenTwoDirectoriesDeclareTheSameId()
    {
        // Found by the first live boot of this path: a rename had left a stale directory beside
        // the renamed one, both declaring the same id. Discovery returns filesystem order, so
        // taking either would make the product depend on directory ordering.
        using var d = Distribution.WithModsDirectory();
        d.WriteManifestOnlyMod("copy-a", "tests.composer.twin");
        d.WriteManifestOnlyMod("copy-b", "tests.composer.twin");

        Action compose = () => Compose(d, bridge: null, "tests.composer.twin");

        compose.Should().Throw<InvalidOperationException>()
            .WithMessage("*tests.composer.twin*")
            .WithMessage("*copy-a*")
            .WithMessage("*copy-b*", "both paths are named so the stale one can be identified");
    }

    // ── the running loop ──────────────────────────────────────────────────────

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateSession_RunningLoop_PublishesTickAdvancedCommandsThroughBridge()
    {
        // Locks the production publishing path GameLoop -> PresentationBridge ->
        // (RenderCommandDispatcher.HandleTickAdvanced on the render thread). Poll-until-condition
        // (F-10 #2): DrainCommands drains, so accumulate across polls up to a generous wall budget
        // rather than asserting after one fixed sleep.
        var bridge = new PresentationBridge();
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d, bridge);

        int tickCommandCount = 0;
        int lastTickValue = -1;
        void Drain() => bridge.DrainCommands(cmd =>
        {
            if (cmd is TickAdvancedCommand tac)
            {
                tickCommandCount++;
                lastTickValue = tac.Tick;
            }
        });

        try
        {
            session.Loop.Start();
            DateTime deadline = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            while (DateTime.UtcNow < deadline && tickCommandCount < 2)
            {
                Drain();
                if (tickCommandCount < 2) Thread.Sleep(25);
            }
        }
        finally
        {
            session.Loop.Stop();
        }
        Drain();

        tickCommandCount.Should().BeGreaterThanOrEqualTo(2,
            $"expected at least 2 TickAdvancedCommand publishes, observed {tickCommandCount}");
        lastTickValue.Should().BeGreaterThanOrEqualTo(tickCommandCount - 1,
            $"expected monotonic tick values, last={lastTickValue}, count={tickCommandCount}");
    }

    // ── the mod menu flow ─────────────────────────────────────────────────────

    [Fact]
    public void MenuFlow_OpenCommitClose_LeavesEditingFalse()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);

        session.Controller.BeginEditing();
        session.Controller.IsEditing.Should().BeTrue();

        CommitResult commit = session.Controller.Commit();
        commit.Success.Should().BeTrue();
        session.Controller.IsEditing.Should().BeFalse();
    }

    [Fact]
    public void MenuFlow_OpenCancelClose_LeavesEditingFalse()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);

        session.Controller.BeginEditing();
        session.Controller.IsEditing.Should().BeTrue();

        session.Controller.Cancel();
        session.Controller.IsEditing.Should().BeFalse();
    }

    [Fact]
    public void MenuFlow_OpenWithoutCommitOrCancel_StaysEditing()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);

        session.Controller.BeginEditing();
        session.Controller.IsEditing.Should().BeTrue();

        session.Controller.GetEditableState().Should().NotBeNull();
        session.Controller.IsEditing.Should().BeTrue();
    }

    [Fact]
    public void MenuFlow_BeginEditing_PausesGameLoop()
    {
        // MOD_OS_ARCHITECTURE §9.2 step 1 — the composer wires controller.OnEditingBegan to
        // loop.SetPaused(true). The user-visible behaviour an F5 verification once surfaced as
        // missing: the TICK counter kept advancing with the menu open.
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);

        session.Loop.IsPaused.Should().BeFalse("the loop is unpaused at construction");

        session.Controller.BeginEditing();

        session.Loop.IsPaused.Should().BeTrue();
    }

    [Fact]
    public void MenuFlow_Cancel_ResumesGameLoop()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);
        session.Controller.BeginEditing();
        session.Loop.IsPaused.Should().BeTrue("precondition for this test");

        session.Controller.Cancel();

        session.Loop.IsPaused.Should().BeFalse(
            "Cancel fires OnEditingEnded which calls loop.SetPaused(false)");
    }

    [Fact]
    public void MenuFlow_CommitSuccess_ResumesGameLoop()
    {
        using var d = Distribution.WithModsDirectory();
        using EngineSession session = Compose(d);
        session.Controller.BeginEditing();
        session.Loop.IsPaused.Should().BeTrue("precondition for this test");

        CommitResult result = session.Controller.Commit();

        result.Success.Should().BeTrue("no toggles applied — Commit on a clean session is a no-op");
        session.Loop.IsPaused.Should().BeFalse(
            "successful Commit fires OnEditingEnded which calls loop.SetPaused(false)");
    }

    // ── what actually ships ───────────────────────────────────────────────────

    [Fact]
    public void DefaultModDiscoverer_FindsEveryShippedModInTheProductionModsRoot()
    {
        // M8.1 — the vanilla mods are discoverable from the production mods/ directory. Locks the
        // shipped set per MOD_OS_ARCHITECTURE v1.5 §1.3: 5 regular slices (Combat, Magic,
        // Inventory, Pawn, World) + 1 shared (Vanilla.Core), alongside the preserved ExampleMod.
        // Resolves the production mods/ directory via a repo-root walk so the result is
        // independent of the test runner's working directory.
        //
        // W3 (C5): 7 -> 9. The Weather mod PAIR joined — dualfrontier.weather.contracts (shared
        // vendor) + dualfrontier.weather (regular mechanic). This count is a deliberate pin on
        // what ships in mods/, so the wave that adds a mod moves it and says why.
        //
        // W4: 9 -> 10. dualfrontier.vanilla.scenario joined. It is the mod the composition root
        // dissolved INTO: it owns the 21 component registrations, the walkability grid, the
        // pathfinding service, the world seeding and the ten gameplay system registrations the
        // engine used to hold. Its arrival is the reason the engine can stop referencing the
        // game's assemblies at all, so this is the one count movement in the wave that is the
        // point rather than a side effect.
        string modsRoot = Path.Combine(FindRepoRoot(), "mods");
        var discoverer = new DefaultModDiscoverer(modsRoot);

        IReadOnlyList<DiscoveredModInfo> discovered = discoverer.Discover();

        discovered.Should().HaveCount(10,
            "ExampleMod + 6 vanilla skeletons (5 regular + 1 shared) per §1.3, " +
            "+ the W3 Weather pair (1 shared vendor + 1 regular mechanic), " +
            "+ the W4 vanilla scenario mod");

        List<string> ids = discovered.Select(x => x.Manifest.Id).ToList();
        ids.Should().Contain("dualfrontier.example");
        ids.Should().Contain("dualfrontier.vanilla.core");
        ids.Should().Contain("dualfrontier.vanilla.combat");
        ids.Should().Contain("dualfrontier.vanilla.magic");
        ids.Should().Contain("dualfrontier.vanilla.inventory");
        ids.Should().Contain("dualfrontier.vanilla.pawn");
        ids.Should().Contain("dualfrontier.vanilla.world");
        ids.Should().Contain("dualfrontier.weather.contracts");
        ids.Should().Contain("dualfrontier.weather");
        ids.Should().Contain(ScenarioModId);

        // W3 — the Weather pair is shaped exactly as the shared/regular split requires.
        DiscoveredModInfo weatherContracts = discovered.Single(
            x => x.Manifest.Id == "dualfrontier.weather.contracts");
        weatherContracts.Manifest.Kind.Should().Be(ModKind.Shared);
        weatherContracts.Manifest.EntryAssembly.Should().BeEmpty();
        weatherContracts.Manifest.EntryType.Should().BeEmpty();

        DiscoveredModInfo weather = discovered.Single(x => x.Manifest.Id == "dualfrontier.weather");
        weather.Manifest.Kind.Should().Be(ModKind.Regular);
        weather.Manifest.Dependencies.Select(x => x.ModId)
            .Should().Contain("dualfrontier.weather.contracts",
                "a regular mod must LIST the shared vendor whose event type it uses");

        // Vanilla.Core is the shared mod with no IMod entry point.
        DiscoveredModInfo core = discovered.Single(
            x => x.Manifest.Id == "dualfrontier.vanilla.core");
        core.Manifest.Kind.Should().Be(ModKind.Shared, "Vanilla.Core is the pure type vendor per §1.2");
        core.Manifest.EntryAssembly.Should().BeEmpty("shared mods must have empty entryAssembly per §2.2");
        core.Manifest.EntryType.Should().BeEmpty("shared mods must have empty entryType per §2.2");

        // Each regular vanilla slice is regular kind and depends on Vanilla.Core.
        foreach (string slice in new[] { "combat", "magic", "inventory", "pawn", "world" })
        {
            string id = $"dualfrontier.vanilla.{slice}";
            DiscoveredModInfo mod = discovered.Single(x => x.Manifest.Id == id);
            mod.Manifest.Kind.Should().Be(ModKind.Regular, $"vanilla.{slice} is a regular mod per §1.3");
            mod.Manifest.Dependencies.Should().ContainSingle(
                $"vanilla.{slice} declares only the shared-Core dependency in M8.1");
            mod.Manifest.Dependencies[0].ModId.Should().Be("dualfrontier.vanilla.core");
        }
    }

    private static string FindRepoRoot()
    {
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
}
