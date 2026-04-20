using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Bus;

/// <summary>
/// Собирает intents (намерения) одного типа в пределах одной фазы и отдаёт
/// их батчем в следующей фазе через <see cref="Drain"/>. Ключевая оптимизация
/// двухшаговой модели Intent → Granted/Refused (TechArch 11.5): вместо N
/// отдельных проходов по кэшу — один батч-проход.
/// </summary>
/// <typeparam name="TIntent">Тип intent-события (обычно record : IEvent).</typeparam>
internal sealed class IntentBatcher<TIntent> where TIntent : IEvent
{
    // TODO: Фаза 1 — private readonly ConcurrentBag<TIntent> _buffer = new();

    /// <summary>
    /// TODO: Фаза 1 — подписчик вызывает Collect при получении intent.
    /// Intent копится в буфере до конца фазы.
    /// </summary>
    public void Collect(TIntent intent)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация IntentBatcher.Collect");
    }

    /// <summary>
    /// TODO: Фаза 1 — вернуть все собранные intents и очистить буфер.
    /// Вызывается планировщиком на границе фаз.
    /// </summary>
    public IReadOnlyList<TIntent> Drain()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация IntentBatcher.Drain");
    }
}
