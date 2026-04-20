using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Владение големом изменилось. Помечено <see cref="DeferredAttribute"/>:
/// подписчики (AI, UI, аудит) обновляют своё состояние на следующем тике,
/// чтобы не гонять мутации в фазе, где работают другие системы магии.
/// </summary>
/// <param name="Golem">Сущность голема.</param>
/// <param name="PreviousMage">Прежний маг-хозяин (или <c>null</c>, если
/// голем был в <see cref="OwnershipMode.Abandoned"/>).</param>
/// <param name="NewMage">Новый маг-хозяин (или <c>null</c>, если голем
/// становится покинутым).</param>
/// <param name="Mode">Итоговый режим владения.</param>
[Deferred]
public sealed record GolemOwnershipChanged(
    EntityId Golem,
    EntityId? PreviousMage,
    EntityId? NewMage,
    OwnershipMode Mode) : IEvent;
