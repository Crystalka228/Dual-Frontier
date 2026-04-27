using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// Published by <c>ConverterSystem</c> every tick while the converter is active.
/// Marked with <see cref="DeferredAttribute"/>: breaks the component cycle
/// ElectricGrid ↔ Converter (via PowerConsumer/PowerProducer) — the data flow
/// Converter → ElectricGrid goes through the bus rather than through the component
/// graph, so DependencyGraph does not see a cycle.
/// <para>
/// ElectricGridSystem subscribes, accumulates the output of all converters for
/// the tick and adds it to the producer-supply sum of the next tick. A one-tick
/// delay is equivalent to the <c>Mana[N-1]</c> pattern from
/// <c>docs/FEEDBACK_LOOPS.md</c>; with <see cref="DualFrontier.Contracts.Attributes.TickRateAttribute"/>=SLOW
/// the latency is imperceptible to the player.
/// </para>
/// </summary>
[Deferred]
public sealed record ConverterPowerOutputEvent : IEvent
{
    /// <summary>The converter entity that published the output.</summary>
    public required EntityId ConverterId { get; init; }

    /// <summary>How many watts the converter feeds into the grid this tick (after efficiency).</summary>
    public required float WattsOutput { get; init; }
}
