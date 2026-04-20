using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Registry;

/// <summary>
/// Реестр типов компонентов. Каждому зарегистрированному типу
/// присваивается стабильный числовой ID. ID используется для
/// сериализации (Save/Load) и как ключ в плотных структурах ECS.
/// </summary>
internal sealed class ComponentRegistry
{
    // TODO: Фаза 1 — private readonly Dictionary<Type, int> _typeIds = new();
    // TODO: Фаза 1 — private int _nextId;

    /// <summary>
    /// TODO: Фаза 1 — зарегистрировать тип компонента и выдать ему ID.
    /// Повторная регистрация того же типа — no-op, возвращает существующий ID.
    /// </summary>
    public int Register<T>() where T : IComponent
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ComponentRegistry");
    }

    /// <summary>
    /// TODO: Фаза 1 — получить ID по типу. Если не зарегистрирован — исключение.
    /// </summary>
    public int GetTypeId(Type componentType)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ComponentRegistry");
    }
}
