namespace DualFrontier.Contracts.Distribution;

/// <summary>
/// The starting-state description a distribution declares and the scenario mod reads.
///
/// <para>
/// This lives in Contracts, not in the engine, because the mod that seeds the world is its
/// consumer: the values travel from <c>game.manifest.json</c> through the host to
/// <c>IModApi.Scenario</c>. The engine itself reads none of them — it carries them.
/// </para>
///
/// <para>
/// Every field here was a hardcoded constant in the composition root before W4, which is what
/// made the configuration row of the execution-authority matrix ownerless: there was no config
/// file and no loader anywhere in the repository, so the tunables had no owner to name. They
/// have one now — the distribution manifest.
/// </para>
///
/// <para>
/// <b>On seeds.</b> Four fixed seeds are carried rather than derived, which is deliberate and
/// coordinates with the open finding that the project has no RNG service. The manifest gives the
/// seeds a home and a name; it does not build the service, and a later cascade that does can
/// take these fields as its input rather than re-inventing them.
/// </para>
/// </summary>
/// <param name="Id">Scenario identifier, for diagnostics and save scoping.</param>
/// <param name="WorldSeed">Seed for world generation as a whole.</param>
/// <param name="MapWidth">Map width in tiles.</param>
/// <param name="MapHeight">Map height in tiles.</param>
/// <param name="ObstacleCount">Number of impassable tiles to scatter.</param>
/// <param name="ObstacleSeed">Seed for obstacle scattering.</param>
/// <param name="FactorySeed">Seed for pawn generation.</param>
/// <param name="ItemFactorySeed">Seed for item placement.</param>
/// <param name="Counts">How many of each starting entity to place.</param>
public sealed record ScenarioConfig(
    string Id,
    int WorldSeed,
    int MapWidth,
    int MapHeight,
    int ObstacleCount,
    int ObstacleSeed,
    int FactorySeed,
    int ItemFactorySeed,
    ScenarioCounts Counts);

/// <summary>
/// The starting entity census for a scenario.
/// </summary>
/// <param name="Pawns">Colonists placed at start.</param>
/// <param name="Food">Consumable items placed at start.</param>
/// <param name="Water">Water sources placed at start.</param>
/// <param name="Beds">Beds placed at start.</param>
/// <param name="Decorations">Decorative items placed at start.</param>
public sealed record ScenarioCounts(
    int Pawns,
    int Food,
    int Water,
    int Beds,
    int Decorations);
