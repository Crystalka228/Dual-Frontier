using System;
using DualFrontier.Persistence.Snapshots;

namespace DualFrontier.Persistence.Compression;

/// <summary>
/// Public façade for the persistence pipeline. This file owns the high-level
/// entry points; the actual byte-level work is delegated to the per-domain
/// encoders (<see cref="TileEncoder"/>, <see cref="ComponentEncoder"/>,
/// <see cref="EntityEncoder"/>, <see cref="StringPool"/>).
///
/// Phase 4 ships the building blocks only — full <c>WorldSnapshot</c>
/// composition, header framing, and incremental delta encoding land with the
/// save-system integration in a later phase. Each block is exercised
/// independently by the persistence test suite.
/// </summary>
public static class DfCompressor
{
    /// <summary>Re-exposes <see cref="TileEncoder.Encode"/> as the canonical entry.</summary>
    public static byte[] EncodeTileMap(TileMapSnapshot map) => TileEncoder.Encode(map);

    /// <summary>Re-exposes <see cref="TileEncoder.Decode"/> as the canonical entry.</summary>
    public static TileMapSnapshot DecodeTileMap(byte[] data, int width, int height)
        => TileEncoder.Decode(data, width, height);

    /// <summary>
    /// Produces a self-contained byte payload for a full <see cref="WorldSnapshot"/>.
    /// Not implemented in Phase 4 — slated for the save-system integration phase
    /// where header framing, section ordering and delta encoding are decided
    /// jointly with the on-disk format.
    /// </summary>
    public static byte[] Compress(WorldSnapshot snapshot)
        => throw new NotImplementedException(
            "Phase 5+: aggregate encode pipeline with header framing and incremental deltas.");

    /// <summary>
    /// Inverse of <see cref="Compress"/>. Same phase ownership.
    /// </summary>
    public static WorldSnapshot Decompress(byte[] data)
        => throw new NotImplementedException(
            "Phase 5+: aggregate decode pipeline; pairs with Compress.");
}
