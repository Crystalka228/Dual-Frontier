using System;
using System.Collections.Generic;

namespace DualFrontier.Core.Math;

/// <summary>
/// Пространственное разбиение: мир режется на квадратные ячейки фиксированного
/// размера, объекты хранятся по ячейкам. Запрос "что в радиусе R от точки P"
/// обходит только соседние ячейки — O(k), где k — число объектов рядом, а не
/// весь мир.
/// </summary>
/// <typeparam name="T">Тип хранимых объектов (обычно EntityId).</typeparam>
internal sealed class SpatialGrid<T>
{
    // TODO: Фаза 3 — private readonly int _cellSize;
    // TODO: Фаза 3 — private readonly Dictionary<GridVector, List<T>> _cells = new();

    /// <summary>
    /// TODO: Фаза 3 — добавить объект в ячейку соответствующую <paramref name="position"/>.
    /// Повторный Insert того же объекта — undefined behaviour (вызывай Remove перед).
    /// </summary>
    public void Insert(T item, GridVector position)
    {
        throw new NotImplementedException("TODO: Фаза 3 — реализация SpatialGrid");
    }

    /// <summary>
    /// TODO: Фаза 3 — вернуть все объекты в радиусе <paramref name="radius"/>
    /// от точки <paramref name="center"/>. Возвращает IEnumerable для lazy-итерации.
    /// </summary>
    public IEnumerable<T> Query(GridVector center, int radius)
    {
        throw new NotImplementedException("TODO: Фаза 3 — реализация SpatialGrid");
    }
}
