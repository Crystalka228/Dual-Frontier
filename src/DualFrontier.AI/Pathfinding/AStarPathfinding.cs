namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// Реализация <see cref="IPathfindingService"/> через A* по
/// <see cref="NavGrid"/>. Синхронный, с лимитом итераций за
/// тик.
///
/// TODO: заменить сигнатуры на <c>GridVector</c> вместе с
/// <see cref="IPathfindingService"/>.
/// </summary>
public sealed class AStarPathfinding : IPathfindingService
{
    /// <inheritdoc />
    public bool TryFindPath(object from, object to, out IReadOnlyList<object> path)
    {
        throw new NotImplementedException(
            "TODO: Фаза 3 — A* по NavGrid: бинарная куча, эвристика Manhattan, лимит итераций"
        );
    }
}
