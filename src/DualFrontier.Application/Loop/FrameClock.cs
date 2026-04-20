using System;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Часовой таймер. Источник времени для цикла симуляции: <see cref="Now"/>
/// возвращает текущий момент, <see cref="DeltaTime"/> — сколько секунд прошло
/// с прошлого тика. Используется <see cref="GameLoop"/> как единственный
/// источник истины о ходе времени; системы видят только передаваемое им
/// <c>delta</c>.
/// </summary>
public sealed class FrameClock
{
    /// <summary>
    /// TODO: Фаза 1 — текущий момент. Стартует с нуля, монотонно растёт,
    /// не зависит от системных часов (никаких DateTime.Now).
    /// </summary>
    public TimeSpan Now
        => throw new NotImplementedException("TODO: Фаза 1 — FrameClock");

    /// <summary>
    /// TODO: Фаза 1 — сколько прошло секунд с прошлого обновления часов.
    /// </summary>
    public float DeltaTime
        => throw new NotImplementedException("TODO: Фаза 1 — FrameClock");
}
