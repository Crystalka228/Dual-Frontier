namespace DualFrontier.Components.World
{
    /// <summary>Type of terrain tile for pathfinding and visuals.</summary>
    public enum TerrainKind
    {
        /// <summary>Open grass — passable, default terrain.</summary>
        Grass,
        /// <summary>Rocky ground — passable but slow.</summary>
        Rock,
        /// <summary>Sandy terrain — passable.</summary>
        Sand,
        /// <summary>Water — impassable without bridge.</summary>
        Water,
        /// <summary>Ice — passable, movement penalty.</summary>
        Ice,
        /// <summary>Swamp — passable, heavy movement penalty.</summary>
        Swamp,
        /// <summary>Arcane ground — passable, boosts ether regen.</summary>
        Arcane,
        /// <summary>Unknown or uninitialized terrain.</summary>
        Unknown
    }
}