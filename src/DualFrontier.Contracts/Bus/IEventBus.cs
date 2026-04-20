using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Базовая шина событий одного домена.
/// Каждый домен (Combat, Inventory, Magic, Pawn, World) имеет
/// свою реализацию этой шины. Это снижает lock contention и
/// упрощает профилирование.
///
/// Публикация и подписка — потокобезопасные.
/// Синхронная доставка — в текущей фазе планировщика.
/// Для отложенной доставки — пометь событие [Deferred].
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// TODO: Фаза 1 — Публикует событие всем подписчикам.
    /// Если событие помечено [Deferred] — доставка в следующей фазе.
    /// Если [Immediate] — прерывает текущую фазу для доставки.
    /// По умолчанию — синхронная доставка в текущей фазе.
    /// </summary>
    void Publish<T>(T evt) where T : IEvent;

    /// <summary>
    /// TODO: Фаза 1 — Подписка на события типа T.
    /// Обработчик вызывается синхронно при Publish.
    /// Важно: не блокировать в обработчике — это блокирует всю фазу.
    /// </summary>
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    /// <summary>
    /// TODO: Фаза 1 — Отписка. Обязательна при утилизации подписчика,
    /// иначе утечка памяти (обработчик держит ссылку на подписчика).
    /// </summary>
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;
}
