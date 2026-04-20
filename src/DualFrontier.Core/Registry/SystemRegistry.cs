using System;
using System.Collections.Generic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Registry;

/// <summary>
/// Реестр зарегистрированных систем. Хранит единственный экземпляр каждой
/// системы. Предоставляет итератор для <see cref="Scheduling.DependencyGraph"/>
/// при построении графа. После построения графа реестр обычно "замораживается":
/// поздняя регистрация требует пересборки графа (дорого).
/// </summary>
internal sealed class SystemRegistry
{
    // TODO: Фаза 1 — private readonly List<SystemBase> _systems = new();

    /// <summary>
    /// TODO: Фаза 1 — зарегистрировать экземпляр системы. Дубликат того же
    /// типа системы — исключение.
    /// </summary>
    public void Register(SystemBase system)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация SystemRegistry");
    }

    /// <summary>
    /// TODO: Фаза 1 — вернуть все зарегистрированные системы в порядке регистрации.
    /// </summary>
    public IReadOnlyList<SystemBase> GetAll()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация SystemRegistry");
    }
}
