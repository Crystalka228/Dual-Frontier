using System;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Канонические значения частот тиков.
/// Дублирует <c>DualFrontier.Contracts.Attributes.TickRates</c> —
/// значения обязаны совпадать, иначе планировщик и атрибуты разойдутся.
/// Дубль существует, чтобы системам Core не требовалось импортировать
/// namespace атрибутов для рантайм-сравнений.
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
