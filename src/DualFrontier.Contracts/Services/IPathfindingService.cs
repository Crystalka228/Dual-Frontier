using System.Collections.Generic;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Contracts.Services;

/// <summary>
/// A* pathfinding service over the walkability grid.
///
/// Synchronous method — async is forbidden in systems; it would break the
/// ThreadLocal isolation guard.
///
/// <para>
/// Lives in <c>DualFrontier.Contracts</c> (relocated from
/// <c>DualFrontier.AI</c> at W1) so it can be exposed through
/// <see cref="Sdk.ISystemServices"/>: a mod referencing Contracts alone must be
/// able to name it (boundary law B-3). Its only types — <see cref="GridVector"/>
/// and the BCL <see cref="IReadOnlyList{T}"/> — are already Contracts-safe, so
/// the move added no reference. The concrete <c>AStarPathfinding</c>
/// implementation stays engine-side in <c>DualFrontier.AI</c>.
/// </para>
/// </summary>
public interface IPathfindingService
{
    /// <summary>
    /// Attempts to find a path. Returns <c>false</c> if a path was not found
    /// or the per-tick iteration budget was exceeded (the pawn will retry).
    /// </summary>
    /// <param name="from">Start.</param>
    /// <param name="to">Goal.</param>
    /// <param name="path">Sequence of tiles.</param>
    bool TryFindPath(GridVector from, GridVector to, out IReadOnlyList<GridVector> path);
}
