using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Request to open a continuous mana lease: a system asks
/// <c>ManaSystem</c> to subscribe the caster to a continuous drain of
/// <paramref name="DrainPerTick"/> for a duration between <paramref name="MinDurationTicks"/>
/// and <paramref name="MaxDurationTicks"/> ticks. The response is <c>ManaLeaseOpened</c>
/// or <c>ManaLeaseRefused</c>.
/// </summary>
/// <param name="Caster">The mage caster from whom mana will be drained.</param>
/// <param name="DrainPerTick">Mana drain per tick (mana units).</param>
/// <param name="MinDurationTicks">Minimum lease duration, in ticks.</param>
/// <param name="MaxDurationTicks">Maximum lease duration, in ticks (after which
/// the lease is closed automatically with <c>CloseReason.Completed</c>).</param>
public sealed record ManaLeaseOpenRequest(
    EntityId Caster,
    float DrainPerTick,
    int MinDurationTicks,
    int MaxDurationTicks) : ICommand;
