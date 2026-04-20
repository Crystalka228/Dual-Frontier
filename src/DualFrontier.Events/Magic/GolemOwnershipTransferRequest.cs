using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Команда на смену владельца голема: передача другому магу, объявление
/// голема оспариваемым или покинутым. Обработчик — <c>GolemBondSystem</c>,
/// итог — событие <c>GolemOwnershipChanged</c>.
/// </summary>
/// <param name="Golem">Сущность голема, для которого меняется владение.</param>
/// <param name="NewMage">Новый маг-хозяин (или <c>null</c>, если голем
/// становится покинутым).</param>
/// <param name="Mode">Целевой режим владения (<see cref="OwnershipMode"/>).</param>
public sealed record GolemOwnershipTransferRequest(
    EntityId Golem,
    EntityId? NewMage,
    OwnershipMode Mode) : ICommand;
