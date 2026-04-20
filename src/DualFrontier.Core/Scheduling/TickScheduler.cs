using System;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Решает, на каком тике игрового времени запускать какую систему, согласно
/// её атрибуту <c>[TickRate]</c>. Держит счётчик и для каждой системы
/// запоминает, когда она последний раз выполнялась.
/// Системы с одинаковым rate группируются — это улучшает локальность кэша.
/// </summary>
internal sealed class TickScheduler
{
    // TODO: Фаза 1 — private long _currentTick;
    // TODO: Фаза 1 — private readonly Dictionary<SystemBase, long> _lastExecuted = new();

    /// <summary>
    /// TODO: Фаза 1 — инкрементировать _currentTick, вызывается раз за ExecuteTick.
    /// </summary>
    public void Advance()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация TickScheduler");
    }

    /// <summary>
    /// TODO: Фаза 1 — проверить, пора ли выполнять систему на текущем тике
    /// согласно её <c>TicksPerUpdate</c>.
    /// </summary>
    public bool ShouldRun(SystemBase system)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация TickScheduler");
    }
}
