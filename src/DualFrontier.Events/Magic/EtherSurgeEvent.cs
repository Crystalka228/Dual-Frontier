using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Эфирный срыв. См. GDD 4.2 «Рост мага — источники опыта»:
/// при работе с сырым эфиром / чистым кристаллом маг рискует получить
/// неконтролируемый выброс энергии. Последствия — урон, статусы,
/// локальная аномалия; при больших срывах — раздача <see cref="Combat.StatusAppliedEvent"/>
/// и/или <see cref="Combat.DamageEvent"/>.
/// </summary>
public sealed record EtherSurgeEvent : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float Magnitude { get; init; }   // сила срыва
    // TODO: public GridVector Epicenter { get; init; }
}
