namespace DualFrontier.Contracts.Services;

/// <summary>
/// The walkability grid a scenario authors and a pathfinding service reads.
///
/// <para>
/// <b>Why this is separate from <see cref="IPathfindingService"/>.</b> That interface answers
/// one question — find me a path — and has a non-engine implementer, so widening it would be a
/// breaking change for implementers and would push the contracts version to a new MAJOR,
/// stranding every shipped <c>apiVersion "^2.0.0"</c> manifest. It also conflates authoring
/// terrain with querying a route. A new type is additive and keeps the surface honest.
/// </para>
///
/// <para>
/// <b>Why the SDK needs it at all.</b> The grid lives in a GAME assembly, so after the engine
/// sheds its game references a mod is the only thing that can build one — and the two vanilla
/// factories both consult passability while placing entities. Without this the scenario could
/// not be authored from mod code at all, which is the SDK-sufficiency obligation of boundary
/// law B-3: what vanilla needs must arrive through the surface any third-party mod has.
/// </para>
///
/// <para>
/// Coordinates are tile indices, origin at the top-left, x rightward and y downward. Reads
/// outside the bounds answer as impassable at maximum cost rather than throwing, so a caller
/// sweeping a neighbourhood needs no bounds arithmetic of its own; writes outside the bounds are
/// ignored. This is the shipped <c>NavGrid</c> behaviour, stated rather than re-specified.
/// </para>
/// </summary>
public interface INavGridService
{
    /// <summary>Grid width in tiles.</summary>
    int Width { get; }

    /// <summary>Grid height in tiles.</summary>
    int Height { get; }

    /// <summary>True when a walker may occupy the tile. Out-of-bounds reads as false.</summary>
    bool IsPassable(int x, int y);

    /// <summary>
    /// Relative traversal cost of the tile; 1 is the default. Out-of-bounds reads as
    /// <see cref="byte.MaxValue"/>.
    /// </summary>
    byte GetCost(int x, int y);

    /// <summary>
    /// Sets passability and traversal cost for a tile. Out-of-bounds writes are ignored.
    /// </summary>
    void SetTile(int x, int y, bool passable, byte cost = 1);
}
