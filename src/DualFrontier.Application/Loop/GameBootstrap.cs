using System;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Application.Modding;
using DualFrontier.Application.Scenario;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Events.Combat;
using DualFrontier.Events.Pawn;
using DualFrontier.Systems.Inventory;
using DualFrontier.Systems.Pawn;
using DualFrontier.Systems.Power;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Production entry point that assembles the simulation graph: a World,
/// the IGameServices aggregator, the TickScheduler, and the dependency
/// graph of registered systems. Bridges domain events to render commands
/// by subscribing on the appropriate buses. Constructs the modding stack
/// (<see cref="ModIntegrationPipeline"/> + <see cref="ModMenuController"/>
/// + <see cref="DefaultModDiscoverer"/>) atop the same scheduler /
/// services and returns both via <see cref="GameContext"/> — the loop the
/// caller starts plus the controller the mod menu (M7.5.B.2) drives.
///
/// Tests bypass this factory and wire their own scheduler so they can
/// register only the systems under test. Smoke coverage of the production
/// wiring lives in <c>GameBootstrapIntegrationTests</c> (M7.5.B.1).
/// </summary>
internal static class GameBootstrap
{
    private const int MapWidth = 50;
    private const int MapHeight = 50;
    private const int InitialPawnCount = 10;
    private const int FactorySeed = 42;
    private const int ObstacleSeed = 42;
    private const int ObstacleCount = 50;

    /// <summary>
    /// Builds the full production simulation context: domain world,
    /// kernel scheduler with the hard-coded ECS systems, modding
    /// pipeline owning the shared ALC, and the menu controller wired to
    /// that pipeline. The <c>TickScheduler</c> constructed here is also
    /// threaded into <see cref="GameLoop"/> so the loop publishes one
    /// <c>TickAdvancedCommand</c> per fixed-step tick onto the presentation
    /// bridge for HUD tick-label rendering. Returns both via
    /// <see cref="GameContext"/>.
    /// </summary>
    /// <param name="bridge">
    /// Presentation bridge the loop publishes render commands onto.
    /// </param>
    /// <param name="modsRoot">
    /// Directory the <see cref="DefaultModDiscoverer"/> scans for mods.
    /// Defaults to the literal string <c>"mods"</c>: production Godot
    /// launches with cwd = project root, so this resolves to
    /// <c>&lt;project&gt;/mods/</c>. Tests override with a temp path or
    /// fixture path. The discoverer handles a non-existent path by
    /// returning an empty list (first-launch safety, no exception).
    /// </param>
    /// <returns>
    /// A <see cref="GameContext"/> carrying the ready-to-start
    /// <see cref="GameLoop"/> and the <see cref="ModMenuController"/>
    /// the menu (M7.5.B.2) binds to its UI scene. The pipeline inside
    /// the controller is left in its default paused state — bootstrap
    /// does not call <c>Pause</c>/<c>Resume</c>; the menu drives the
    /// §9.2 Pause-Toggle-Apply-Resume sequence from
    /// <see cref="ModMenuController"/>.
    /// </returns>
    public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        services.Pawns.Subscribe<PawnSpawnedEvent>(e =>
            bridge.Enqueue(new PawnSpawnedCommand(e.PawnId, e.X, e.Y)));
        services.Pawns.Subscribe<PawnMovedEvent>(e =>
            bridge.Enqueue(new PawnMovedCommand(e.PawnId, e.X, e.Y)));
        services.Combat.Subscribe<DeathEvent>(e =>
            bridge.Enqueue(new PawnDiedCommand(e.Who)));
        services.Pawns.Subscribe<PawnStateChangedEvent>(e =>
            bridge.Enqueue(new PawnStateCommand(
                e.PawnId, e.Name, e.Satiety, e.Hydration, e.Sleep, e.Comfort,
                e.Mood, e.JobLabel, e.JobUrgent, e.TopSkills)));

        var navGrid = new NavGrid(MapWidth, MapHeight);
        var obstacleRng = new Random(ObstacleSeed);
        for (int i = 0; i < ObstacleCount; i++)
        {
            int ox = obstacleRng.Next(0, MapWidth);
            int oy = obstacleRng.Next(0, MapHeight);
            navGrid.SetTile(ox, oy, passable: false);
        }
        var pathfinding = new AStarPathfinding(navGrid);

        var pawnFactory = new RandomPawnFactory(FactorySeed, navGrid, MapWidth, MapHeight);
        pawnFactory.Spawn(world, services, InitialPawnCount);

        // Hard-coded kernel systems live in a local array so the same
        // instances flow into both the dependency graph (kernel
        // scheduling) and ModRegistry.SetCoreSystems (visible to the
        // mod-validation/replacement logic in the pipeline). Per AD #5
        // of the M7.5.B.1 prompt, the mod side must see the same
        // SystemBase instances the kernel scheduler is ticking.
        var coreSystems = new SystemBase[]
        {
            new NeedsSystem(),
            new MoodSystem(),
            new JobSystem(),
            new MovementSystem(pathfinding),
            new PawnStateReporterSystem(),
            new InventorySystem(),
            new HaulSystem(),
            new ElectricGridSystem(),
            new ConverterSystem(),
        };

        var graph = new DependencyGraph();
        foreach (SystemBase s in coreSystems)
            graph.AddSystem(s);
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            ticks,
            world,
            faultSink: null,
            services:  services);

        // M7.5.B.1 — modding stack. Pipeline starts in its default
        // paused state (M7.1 load-bearing default); bootstrap does not
        // call Pause/Resume. The controller (returned via GameContext)
        // is what M7.5.B.2's menu scene drives through Begin/Toggle/
        // Cancel/Commit. The same `services` aggregator is threaded
        // through both the kernel scheduler and the pipeline so mod-
        // published events route through the existing M2 wiring.
        var modLoader = new ModLoader();
        var modRegistry = new ModRegistry();
        modRegistry.SetCoreSystems(coreSystems);
        var modValidator = new ContractValidator();
        var modContractStore = new ModContractStore();
        var pipeline = new ModIntegrationPipeline(
            modLoader, modRegistry, modValidator, modContractStore, services, scheduler);
        var discoverer = new DefaultModDiscoverer(modsRoot);
        var controller = new ModMenuController(pipeline, discoverer);

        // Pass the TickScheduler into GameLoop so its accumulator can publish
        // a TickAdvancedCommand carrying CurrentTick after every ExecuteTick;
        // RenderCommandDispatcher routes the command to the HUD tick label.
        var loop = new GameLoop(scheduler, ticks, bridge);

        // MOD_OS_ARCHITECTURE §9.2 step 1 — menu opens => simulation pauses.
        // Pipeline.Pause() (already called by controller.BeginEditing) only
        // gates the Apply-mutation safety flag; the tick-advance gate is
        // GameLoop._paused. Wire both surfaces here at the orchestration
        // layer that owns both the loop and the controller.
        controller.OnEditingBegan = () => loop.SetPaused(true);
        controller.OnEditingEnded = () => loop.SetPaused(false);

        return new GameContext(loop, controller);
    }
}
