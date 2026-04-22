using DualFrontier.Contracts.Math;

namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// Сервис поиска пути A* по сетке проходимости.
///
/// Синхронный метод — async в системах запрещён, ломает
/// ThreadLocal сторожа изоляции.
/// </summary>
public interface IPathfindingService
{
    /// <summary>
    /// Попытка найти путь. Возвращает <c>false</c>, если путь не
    /// найден или превышен лимит итераций за тик (пешка
    /// попробует ещё раз).
    /// </summary>
    /// <param name="from">Старт.</param>
    /// <param name="to">Финиш.</param>
    /// <param name="path">Последовательность тайлов.</param>
    bool TryFindPath(GridVector from, GridVector to, out IReadOnlyList<GridVector> path);
}
