namespace DualFrontier.Components.World
{
    /// <summary>
    /// Type of terrain tile for pathfinding and visuals. Values are explicit and stable: the tile
    /// RLE persistence format (<c>TileEncoder</c>, save format v2) writes the numeric value as one
    /// byte, so members must never be renumbered without a save-format migration.
    /// </summary>
    public enum TerrainKind
    {
        /// <summary>
        /// Unknown or uninitialized terrain. Deliberately the zero value so that
        /// <c>default(TerrainKind)</c> and any zero-initialized tile storage read as
        /// "not yet set" rather than silently as a valid biome (was Grass — F21).
        /// </summary>
        Unknown = 0,
        /// <summary>Open grass — passable, default gameplay terrain.</summary>
        Grass = 1,
        /// <summary>Rocky ground — passable but slow.</summary>
        Rock = 2,
        /// <summary>Sandy terrain — passable.</summary>
        Sand = 3,
        /// <summary>Water — impassable without bridge.</summary>
        Water = 4,
        /// <summary>Ice — passable, movement penalty.</summary>
        Ice = 5,
        /// <summary>Swamp — passable, heavy movement penalty.</summary>
        Swamp = 6,
        /// <summary>Arcane ground — passable, boosts ether regen.</summary>
        Arcane = 7
    }
}