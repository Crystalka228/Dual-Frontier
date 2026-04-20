using System;
using System.Collections.Generic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Граф зависимостей систем по READ/WRITE компонентам.
/// Ребро A → B означает "B читает то, что A пишет" — B обязан выполниться
/// после A. Топологическая сортировка даёт список <see cref="SystemPhase"/>,
/// где в одной фазе все системы независимы и могут исполняться параллельно.
/// </summary>
internal sealed class DependencyGraph
{
    // TODO: Фаза 1 — private readonly List<SystemBase> _systems = new();
    // TODO: Фаза 1 — private readonly Dictionary<SystemBase, HashSet<SystemBase>> _edges = new();

    /// <summary>
    /// TODO: Фаза 1 — добавить систему в граф. Атрибут <c>[SystemAccess]</c>
    /// читается рефлексией для заполнения рёбер. Перед первым <see cref="Build"/>.
    /// </summary>
    public void AddSystem(SystemBase system)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация DependencyGraph");
    }

    /// <summary>
    /// TODO: Фаза 1 — построить граф: определить рёбра по атрибутам
    /// <c>[SystemAccess]</c> и выполнить топологическую сортировку.
    /// Должен вызываться один раз после добавления всех систем.
    /// </summary>
    public void Build()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация DependencyGraph");
    }

    /// <summary>
    /// TODO: Фаза 1 — вернуть список фаз, упорядоченный так, что системы
    /// фазы N могут использовать результаты фазы N-1.
    /// </summary>
    public IReadOnlyList<SystemPhase> GetPhases()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация DependencyGraph");
    }
}
