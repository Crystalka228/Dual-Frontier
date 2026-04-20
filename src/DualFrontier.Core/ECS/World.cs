using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Реестр всех entities и их компонентов.
/// Хранение по типам: для каждого типа компонента — свой ComponentStore.
/// Запросы Query&lt;T1, T2&gt; возвращают пересечение множеств entities
/// имеющих все указанные компоненты.
///
/// ВАЖНО: World.GetComponent&lt;T&gt;() напрямую из системы запрещён.
/// Система должна использовать SystemBase.GetComponent&lt;T&gt;(), который
/// проходит через SystemExecutionContext (сторож изоляции).
/// </summary>
internal sealed class World
{
    // TODO: Фаза 1 — private readonly ConcurrentDictionary<Type, IComponentStore> _stores = new();
    // TODO: Фаза 1 — private int _nextEntityId;

    /// <summary>
    /// TODO: Фаза 1 — Создать новую entity. Возвращает EntityId с версией.
    /// Версия нужна для обнаружения "мёртвых" ссылок после удаления.
    /// </summary>
    public EntityId CreateEntity()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }

    /// <summary>
    /// TODO: Фаза 1 — Удалить entity и все её компоненты.
    /// Инкрементирует версию — старые ссылки становятся невалидными.
    /// </summary>
    public void DestroyEntity(EntityId id)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }

    /// <summary>
    /// TODO: Фаза 1 — НЕБЕЗОПАСНЫЙ доступ — только для SystemExecutionContext.
    /// Не вызывать напрямую из игровой логики.
    /// </summary>
    internal T GetComponentUnsafe<T>(EntityId id) where T : IComponent
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }
}
