using System;

namespace DualFrontier.Persistence.Snapshots;

/// <summary>
/// Header carried by every <see cref="WorldSnapshot"/>. Drives format-version
/// negotiation on load and gives a UI summary (timestamp, entity count) without
/// having to decode the body.
/// </summary>
public sealed record SaveMeta
{
    /// <summary>Snapshot format version. Bump when on-disk layout changes.</summary>
    public required int Version { get; init; } = 1;

    /// <summary>Simulation tick at which the snapshot was taken.</summary>
    public required long Tick { get; init; }

    /// <summary>Seed used to generate the world (stable for full reload).</summary>
    public required int WorldSeed { get; init; }

    /// <summary>Wall-clock time of capture.</summary>
    public required DateTime SavedAt { get; init; }

    /// <summary>Total live entity count at capture.</summary>
    public required int EntityCount { get; init; }

    /// <summary>Tile-map width in tiles.</summary>
    public required int MapWidth { get; init; }

    /// <summary>Tile-map height in tiles.</summary>
    public required int MapHeight { get; init; }
}
