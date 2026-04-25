using System.Collections.Generic;

namespace DualFrontier.Persistence.Snapshots;

/// <summary>
/// Composite immutable snapshot of the simulation state. Producers walk the
/// live <c>World</c> once and assemble a <see cref="WorldSnapshot"/> that
/// <c>DfCompressor</c> can serialise without touching ECS internals.
/// </summary>
public sealed class WorldSnapshot
{
    public required SaveMeta                       Meta     { get; init; }
    public required TileMapSnapshot                TileMap  { get; init; }
    public required IReadOnlyList<PawnSnapshot>    Pawns    { get; init; }
    public required IReadOnlyList<StorageSnapshot> Storages { get; init; }
}
