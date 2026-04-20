namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// Сетка проходимости: битмап "можно ли ступить на этот тайл"
/// + кэш стоимости прохода (бездорожье, дорога, болото и т. д.).
/// Обновляется снаружи (из <c>MapSystem</c> / <c>Application</c>)
/// батчами, когда мир меняется.
///
/// Используется <see cref="AStarPathfinding"/> как read-only
/// структура в пределах одного запроса.
/// </summary>
public sealed class NavGrid
{
    // TODO: Фаза 3 — passability bitmap (`bool[]` или `BitArray`)
    //       + стоимости прохода (`byte[]`) размером Width*Height.
    // TODO: метод `IsPassable(x, y)` + `GetCost(x, y)`.
    // TODO: метод `MarkDirty(region)` для точечного пересчёта
    //       при постройке/сносе.
}
