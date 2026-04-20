using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Не-дженерик маркер для хранения всех <see cref="ComponentStore{T}"/>
/// в одной коллекции (обычно <c>ConcurrentDictionary&lt;Type, IComponentStore&gt;</c>
/// внутри <see cref="World"/>). Никаких методов сюда добавлять нельзя —
/// иначе теряется типобезопасность конкретных store-ов.
/// </summary>
internal interface IComponentStore
{
}

/// <summary>
/// Типизированный storage для компонентов одного типа.
/// Планируется реализация через SparseSet: плотный массив значений +
/// разреженный массив индексов по EntityId. Это даёт O(1) Add/Remove/Get
/// и cache-friendly итерацию по всем компонентам типа <typeparamref name="T"/>.
///
/// ВАЖНО: ComponentStore сам по себе НЕ потокобезопасен. Безопасность
/// обеспечивает <c>ParallelSystemScheduler</c> на уровне графа: системы,
/// конфликтующие по WRITE на один тип, никогда не исполняются параллельно.
/// </summary>
/// <typeparam name="T">Тип хранимого компонента.</typeparam>
internal sealed class ComponentStore<T> : IComponentStore where T : IComponent
{
    // TODO: Фаза 1 — private T[] _dense = Array.Empty<T>();
    // TODO: Фаза 1 — private int[] _sparse = Array.Empty<int>();
    // TODO: Фаза 1 — private int _count;

    /// <summary>
    /// TODO: Фаза 1 — добавить компонент для указанной entity.
    /// Если у entity уже есть компонент этого типа — перезаписать.
    /// </summary>
    public void Add(EntityId id, T component)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ComponentStore");
    }

    /// <summary>
    /// TODO: Фаза 1 — удалить компонент у указанной entity.
    /// Если компонента не было — no-op.
    /// </summary>
    public void Remove(EntityId id)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ComponentStore");
    }

    /// <summary>
    /// TODO: Фаза 1 — получить компонент по EntityId. Бросить, если не найден.
    /// </summary>
    public T Get(EntityId id)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ComponentStore");
    }

    /// <summary>
    /// TODO: Фаза 1 — проверить наличие компонента у entity.
    /// </summary>
    public bool Has(EntityId id)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ComponentStore");
    }
}
