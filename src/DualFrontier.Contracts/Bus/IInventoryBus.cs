using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Inventory-domain bus. Events: ammo request/grant, item add and remove,
/// reservations, craft requests.
/// Writers: <c>HaulSystem</c>, <c>CraftSystem</c>.
/// Readers: <c>InventorySystem</c>, <c>JobSystem</c>.
/// </summary>
public interface IInventoryBus : IEventBus
{
}
