using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Command to change a golem's owner: transfer to another mage, declare
/// the golem contested or abandoned. Handler — <c>GolemBondSystem</c>,
/// outcome — the <c>GolemOwnershipChanged</c> event.
/// </summary>
/// <param name="Golem">The golem entity whose ownership is changing.</param>
/// <param name="NewMage">New mage-owner (or <c>null</c> if the golem
/// becomes abandoned).</param>
/// <param name="Mode">Target ownership mode (<see cref="OwnershipMode"/>).</param>
public sealed record GolemOwnershipTransferRequest(
    EntityId Golem,
    EntityId? NewMage,
    OwnershipMode Mode) : ICommand;
