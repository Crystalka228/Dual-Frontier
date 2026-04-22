using DualFrontier.Contracts.Math;

namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// Реализация <see cref="IPathfindingService"/> через A* по
/// <see cref="NavGrid"/>. Синхронный, с лимитом итераций за
/// тик.
/// </summary>
public sealed class AStarPathfinding : IPathfindingService
{
    /// <inheritdoc />
    public bool TryFindPath(GridVector from, GridVector to, out IReadOnlyList<GridVector> path)
    {
        throw new NotImplementedException(
            "TODO: Фаза 3 — A* по NavGrid: бинарная куча, эвристика Manhattan, лимит итераций"
        );
    }
}
