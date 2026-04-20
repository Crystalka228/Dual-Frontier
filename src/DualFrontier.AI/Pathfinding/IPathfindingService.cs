namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// Сервис поиска пути A* по сетке проходимости.
///
/// Синхронный метод — async в системах запрещён, ломает
/// ThreadLocal сторожа изоляции.
///
/// TODO: заменить <c>object</c> на <c>GridVector</c> после того
/// как <c>GridVector</c> переедет в <c>Contracts</c> (или в
/// новую сборку <c>DualFrontier.Primitives</c>). Сейчас AI не
/// зависит от <c>Core</c>, где живёт <c>GridVector</c>.
/// </summary>
public interface IPathfindingService
{
    /// <summary>
    /// Попытка найти путь. Возвращает <c>false</c>, если путь не
    /// найден или превышен лимит итераций за тик (пешка
    /// попробует ещё раз).
    /// </summary>
    /// <param name="from">Старт (TODO: <c>GridVector</c>).</param>
    /// <param name="to">Финиш (TODO: <c>GridVector</c>).</param>
    /// <param name="path">Последовательность тайлов (TODO: <c>IReadOnlyList&lt;GridVector&gt;</c>).</param>
    bool TryFindPath(object from, object to, out IReadOnlyList<object> path);
}
