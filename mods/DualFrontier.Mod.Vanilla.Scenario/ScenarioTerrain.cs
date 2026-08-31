using System;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Contracts.Distribution;

namespace DualFrontier.Mod.Vanilla.Scenario;

/// <summary>
/// The scenario's terrain: which tiles a colonist may stand on.
///
/// <para>
/// <b>Why this is not part of seeding.</b> It used to be, and that fused two different lifetimes
/// into one method. Minting the colony must happen ONCE — a reload resumes a world, it does not
/// re-populate it. Scattering obstacles must happen on EVERY registration, because the grid is
/// rebuilt from scratch each time the mod initialises and a fresh grid is wholly passable. With
/// both behind the seeder's read-then-mint guard, a reload took the early return and left the new
/// grid empty, so the movement system it had just been handed pathfound over a world with no
/// walls in it while the colony carried on as if there were.
/// </para>
///
/// <para>
/// Binding the scatter to the grid's CONSTRUCTION instead of to the seeding hook makes
/// passability an invariant of the grid rather than a side effect of a hook that may or may not
/// run. It holds whether the world is already seeded, and whether the seeder faults — which
/// matters, because a seeder fault is contained per mod, and the old shape would have left a
/// quarantined mod's obstacle-free grid behind for the pathfinder to keep using.
/// </para>
///
/// <para>
/// The scatter is deterministic in the scenario's obstacle seed, so two grids built from the same
/// scenario are identical, and the stream is separate from the two placement streams so changing
/// the terrain does not move a single colonist.
/// </para>
/// </summary>
internal static class ScenarioTerrain
{
    /// <summary>
    /// Marks the scenario's obstacle tiles impassable on <paramref name="grid"/>.
    /// </summary>
    /// <returns>
    /// How many draws were made. Draws may repeat a tile, so this is the obstacle COUNT the
    /// scenario asked for and not the number of distinct tiles blocked; it is returned so the mod
    /// can say what it did rather than for anyone to compute a map from.
    /// </returns>
    internal static int Scatter(NavGrid grid, ScenarioConfig scenario)
    {
        if (grid is null) throw new ArgumentNullException(nameof(grid));
        if (scenario is null) throw new ArgumentNullException(nameof(scenario));

        var rng = new Random(scenario.ObstacleSeed);
        for (int i = 0; i < scenario.ObstacleCount; i++)
        {
            int x = rng.Next(0, scenario.MapWidth);
            int y = rng.Next(0, scenario.MapHeight);
            grid.SetTile(x, y, passable: false);
        }

        return scenario.ObstacleCount;
    }
}
