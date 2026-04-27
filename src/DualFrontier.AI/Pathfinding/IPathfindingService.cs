using DualFrontier.Contracts.Math;

namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// A* pathfinding service over the walkability grid.
///
/// Synchronous method — async is forbidden in systems; it would break the
/// ThreadLocal isolation guard.
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
