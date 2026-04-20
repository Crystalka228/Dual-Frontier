using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Частота вызова метода <c>Update</c> системы в тиках.
/// Значение — "один вызов на N тиков игрового времени".
/// Константы см. в <see cref="TickRates"/>.
/// Планировщик группирует системы с одинаковым tick rate для лучшего кэш-поведения.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TickRateAttribute : Attribute
{
    /// <summary>
    /// Сколько тиков игрового времени должно пройти между двумя
    /// последовательными <c>Update</c> этой системы.
    /// </summary>
    public int TicksPerUpdate { get; }

    /// <summary>
    /// Создаёт атрибут с указанной частотой (см. <see cref="TickRates"/>).
    /// </summary>
    public TickRateAttribute(int ticksPerUpdate)
    {
        TicksPerUpdate = ticksPerUpdate;
    }
}

/// <summary>
/// Каноничные значения частот тиков для <see cref="TickRateAttribute"/>.
/// Определены здесь, чтобы атрибут можно было применять из любого места без
/// зависимости на <c>DualFrontier.Core.Scheduling</c>. Константы дублируются
/// в <c>DualFrontier.Core.Scheduling.TickRates</c> — значения обязаны совпадать.
/// </summary>
public static class TickRates
{
    /// <summary>Каждый тик — физика снарядов, UI-реактивность.</summary>
    public const int REALTIME = 1;

    /// <summary>Раз в 3 тика — бой, отзывчивые системы.</summary>
    public const int FAST = 3;

    /// <summary>Раз в 15 тиков — обычная логика: работы, навыки, мана.</summary>
    public const int NORMAL = 15;

    /// <summary>Раз в 60 тиков (~1 раз/сек) — нужды, настроение, рост эфира.</summary>
    public const int SLOW = 60;

    /// <summary>Раз в 3600 тиков (~1 раз/мин) — социалка, рейды, торговля.</summary>
    public const int RARE = 3600;
}
