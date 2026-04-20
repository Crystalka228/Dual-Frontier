using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Запрос на открытие непрерывной аренды маны: система просит
/// <c>ManaSystem</c> подписать кастера на постоянный дренаж
/// <paramref name="DrainPerTick"/> на срок от <paramref name="MinDurationTicks"/>
/// до <paramref name="MaxDurationTicks"/> тиков. Ответ — <c>ManaLeaseOpened</c>
/// или <c>ManaLeaseRefused</c>.
/// </summary>
/// <param name="Caster">Маг-кастер, у которого будет списываться мана.</param>
/// <param name="DrainPerTick">Расход маны за тик (единиц маны).</param>
/// <param name="MinDurationTicks">Минимальная длительность аренды, тиков.</param>
/// <param name="MaxDurationTicks">Максимальная длительность аренды, тиков (после чего
/// аренда закрывается автоматически с <c>CloseReason.Completed</c>).</param>
public sealed record ManaLeaseOpenRequest(
    EntityId Caster,
    float DrainPerTick,
    int MinDurationTicks,
    int MaxDurationTicks) : ICommand;
