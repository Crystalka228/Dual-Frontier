using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Маркер-интерфейс события доменной шины.
/// События — это неизменяемые records, публикуемые через <c>IEventBus.Publish</c>.
/// Для отложенной доставки — пометь событие атрибутом <c>[Deferred]</c>.
/// Для мгновенной (прерывающей фазу) — <c>[Immediate]</c>.
/// </summary>
public interface IEvent
{
}
