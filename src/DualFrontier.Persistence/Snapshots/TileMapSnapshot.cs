using DualFrontier.Components.World;

namespace DualFrontier.Persistence.Snapshots;

/// <summary>
/// Immutable snapshot of the tile grid at capture time. Stored as a flat
/// row-major <see cref="TerrainKind"/> array so the RLE encoder can stream
/// over it without per-tile object allocation.
/// </summary>
public sealed class TileMapSnapshot
{
    /// <summary>Map width in tiles.</summary>
    public required int Width { get; init; }

    /// <summary>Map height in tiles.</summary>
    public required int Height { get; init; }

    /// <summary>
    /// Row-major terrain array of length <c>Width * Height</c>. Index
    /// <c>y * Width + x</c> for tile <c>(x, y)</c>.
    /// </summary>
    public required TerrainKind[] Tiles { get; init; }
}
