using System;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Bus;

/// <summary>
/// Реализация одной доменной шины событий.
/// Подписки хранятся в <c>ConcurrentDictionary&lt;Type, Delegate&gt;</c>:
/// потокобезопасность подписки/отписки без общего lock. Доставка —
/// синхронная по умолчанию. Для <c>[Deferred]</c> событие кладётся в
/// очередь и доставляется в следующей фазе планировщиком.
/// </summary>
internal sealed class DomainEventBus : IEventBus
{
    // TODO: Фаза 1 — private readonly ConcurrentDictionary<Type, Delegate> _handlers = new();
    // TODO: Фаза 1 — private readonly ConcurrentQueue<(Type, object)> _deferredQueue = new();

    /// <summary>
    /// TODO: Фаза 1 — опубликовать событие. См. XML-док у <see cref="IEventBus.Publish{T}"/>.
    /// </summary>
    public void Publish<T>(T evt) where T : IEvent
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация DomainEventBus.Publish");
    }

    /// <summary>
    /// TODO: Фаза 1 — подписаться на события типа T.
    /// </summary>
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация DomainEventBus.Subscribe");
    }

    /// <summary>
    /// TODO: Фаза 1 — снять подписку.
    /// </summary>
    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация DomainEventBus.Unsubscribe");
    }
}
