using System;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Contracts.Distribution;
using DualFrontier.Contracts.Modding;
using DualFrontier.Systems.Inventory;
using DualFrontier.Systems.Pawn;

namespace DualFrontier.Mod.Vanilla.Scenario;

/// <summary>
/// The vanilla game, as a mod.
///
/// <para>
/// Everything registered here used to be hardcoded into the engine's composition root: 21
/// component types, ten gameplay systems, a walkability grid, a pathfinding service and the
/// starting colony. The engine referenced the game's four assemblies solely to do it. It does
/// not any more — this mod does, and a mod referencing game assemblies is a game-to-game edge,
/// which is what the boundary law has always permitted.
/// </para>
///
/// <para>
/// <b>The system FILES have not moved.</b> The ten gameplay systems still live in
/// <c>src/DualFrontier.Systems</c>; this mod registers them where they stand. Relocating them is
/// the next wave's work, and doing it here would have merged two independent moves into one
/// unreviewable change. What matters for this wave is that the ENGINE no longer names them.
/// </para>
///
/// <para>
/// <b>Their fault policy changes, and that was ratified rather than discovered.</b> Fault routing
/// keys on system ORIGIN: a core-origin throw fails the session fast, a mod-origin throw is
/// contained and its mod quarantined. Registering these ten as mod systems therefore converts
/// ten fail-fast paths into ten quarantine paths. The operator accepted that explicitly on the
/// grounds that there is no game yet — a quarantined colony is a tolerable failure mode for a
/// harness, and would not be for a shipped product.
/// </para>
/// </summary>
public sealed class ScenarioMod : IMod
{
    private NavGrid? _navGrid;

    public void Initialize(IModApi api)
    {
        if (api is null) throw new ArgumentNullException(nameof(api));

        ScenarioConfig scenario = api.Scenario
            ?? throw new InvalidOperationException(
                "The vanilla scenario mod requires a distribution scenario and the host provided " +
                "none. It deliberately does not substitute defaults of its own: a fabricated " +
                "colony would render as a working game while silently ignoring what the " +
                "distribution asked for.");

        VanillaComponents.RegisterAll(api);

        // The grid and the pathfinding over it are GAME content, built and owned here. The engine
        // cannot build them -- they live in a game assembly it no longer references -- and it does
        // not need to: the factory registration overload lets this mod close over what it built,
        // so nothing has to travel through the engine's construction-time service surface.
        var navGrid = new NavGrid(scenario.MapWidth, scenario.MapHeight);
        // Terrain belongs to the grid, not to the colony. A fresh grid is wholly passable and one
        // is built on EVERY initialisation, so the scatter has to happen here rather than behind
        // the seeder's once-only guard -- see ScenarioTerrain for what that cost.
        ScenarioTerrain.Scatter(navGrid, scenario);
        var pathfinding = new AStarPathfinding(navGrid);
        _navGrid = navGrid;

        // Seeding is NOT a system. The scheduler enforces one writer per component type across
        // the whole graph, so a seeder declaring the components it writes would collide with
        // every gameplay system that owns one, and declaring fewer would undermine the guarantee
        // that makes parallel dispatch safe. The world seeder runs once, outside the graph,
        // before the first tick.
        var seeder = new ScenarioSeeder(scenario, navGrid);
        api.RegisterWorldSeeder(seeder.Seed);
        api.RegisterSystem<PawnPresentationSystem>(_ => new PawnPresentationSystem());

        // The ten gameplay systems. Nine construct parameterlessly; movement takes the
        // pathfinding service, which is exactly the case the parameterless registration path
        // cannot express -- and because a throw out of Initialize rolls back the whole batch, it
        // would have taken the other nine down with it.
        api.RegisterSystem<NeedsSystem>(_ => new NeedsSystem());
        api.RegisterSystem<MoodSystem>(_ => new MoodSystem());
        api.RegisterSystem<JobSystem>(_ => new JobSystem());
        api.RegisterSystem<ConsumeSystem>(_ => new ConsumeSystem());
        api.RegisterSystem<SleepSystem>(_ => new SleepSystem());
        api.RegisterSystem<ComfortAuraSystem>(_ => new ComfortAuraSystem());
        api.RegisterSystem<MovementSystem>(_ => new MovementSystem(pathfinding));
        api.RegisterSystem<PawnStateReporterSystem>(_ => new PawnStateReporterSystem());
        api.RegisterSystem<InventorySystem>(_ => new InventorySystem());
        api.RegisterSystem<HaulSystem>(_ => new HaulSystem());

        api.Log(ModLogLevel.Info,
            $"vanilla scenario '{scenario.Id}' armed: {VanillaComponents.Count} component types, " +
            $"11 systems, {scenario.MapWidth}x{scenario.MapHeight} map, " +
            $"{scenario.ObstacleCount} obstacles.");
    }

    /// <summary>
    /// Nothing to release. The systems this mod registered are torn down by the unload chain,
    /// which disposes each one through the mod's sub-scheduler; the grid is plain managed memory
    /// and goes with the instance. The colony ITSELF survives an unload as inert entities,
    /// because a mod has no world handle here to clean up with -- a known and ledgered gap in the
    /// unload contract, not something this mod can close on its own.
    /// </summary>
    public void Unload() => _navGrid = null;
}
