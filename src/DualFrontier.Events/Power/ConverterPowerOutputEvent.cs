using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// Опубликовано <c>ConverterSystem</c> каждый тик, когда конвертор активен.
/// Помечено <see cref="DeferredAttribute"/>: разрывает цикл компонентов
/// ElectricGrid ↔ Converter (по PowerConsumer/PowerProducer) — поток данных
/// Converter→ElectricGrid идёт через шину, а не через компонентный граф,
/// поэтому DependencyGraph не видит цикла.
/// <para>
/// ElectricGridSystem подписывается, накапливает выход всех конверторов за
/// тик и складывает к сумме producer-supply следующего тика. Один тик
/// задержки эквивалентен паттерну <c>Mana[N-1]</c> из
/// <c>docs/FEEDBACK_LOOPS.md</c>; при <see cref="DualFrontier.Contracts.Attributes.TickRateAttribute"/>=SLOW
/// латентность игроку незаметна.
/// </para>
/// </summary>
[Deferred]
public sealed record ConverterPowerOutputEvent : IEvent
{
    /// <summary>Сущность конвертора, опубликовавшего выход.</summary>
    public required EntityId ConverterId { get; init; }

    /// <summary>Сколько ватт конвертор отдаёт в сеть в этом тике (после КПД).</summary>
    public required float WattsOutput { get; init; }
}
