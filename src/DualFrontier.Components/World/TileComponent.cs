namespace DualFrontier.Components.World
{
    using DualFrontier.Contracts.Core;

    /// <summary>
    /// Tile data: terrain type and passability flag.
    /// One TileComponent per grid position. Passability is read by
    /// PathfindingService; Terrain is read by BiomeSystem and Presentation.
    /// </summary>
    public sealed class TileComponent : IComponent
    {
        /// <summary>Type of terrain on this tile.</summary>
        public TerrainKind Terrain;

        /// <summary>Whether pawns and projectiles can pass through this tile.</summary>
        public bool Passable;

        /// <summary>Default grass tile used when spawning the initial map.</summary>
        public static TileComponent Default =>
            new TileComponent { Terrain = TerrainKind.Grass, Passable = true };
    }
}