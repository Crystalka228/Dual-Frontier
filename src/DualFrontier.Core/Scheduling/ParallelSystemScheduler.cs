using System;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Параллельный планировщик систем. Принимает список фаз из
/// <see cref="DependencyGraph"/> и исполняет их по порядку: внутри одной
/// фазы — параллельно (разным потокам назначаются разные системы),
/// между фазами — синхронизация (барьер).
///
/// Перед каждым вызовом <c>SystemBase.Update</c> планировщик устанавливает
/// <c>SystemExecutionContext.Current</c> для текущего потока и снимает после.
///
/// v02 §12.4: <c>CompositeResolutionSystem</c> и <c>ComboResolutionSystem</c>
/// обеспечивают детерминированный порядок multi-bus запросов в пределах
/// одного тика — собирают частичные ответы от нескольких шин и сортируют
/// итоги по стабильному ключу перед публикацией.
/// </summary>
internal sealed class ParallelSystemScheduler
{
    // TODO: Фаза 1 — private readonly IReadOnlyList<SystemPhase> _phases;
    // TODO: Фаза 1 — private readonly TickScheduler _ticks;

    /// <summary>
    /// TODO: Фаза 1 — исполнить одну фазу: запустить все её системы
    /// параллельно и дождаться завершения всех.
    /// </summary>
    public void ExecutePhase(SystemPhase phase, float delta)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ParallelSystemScheduler");
    }

    /// <summary>
    /// TODO: Фаза 1 — исполнить один игровой тик: пройти все фазы по
    /// порядку, учитывая <c>[TickRate]</c> каждой системы.
    /// </summary>
    public void ExecuteTick(float delta)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ParallelSystemScheduler");
    }
}
