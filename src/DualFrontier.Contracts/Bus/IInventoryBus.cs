using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Шина складского домена. События: запрос/выдача патронов, добавление и
/// удаление предметов, резервирование, заявки на крафт.
/// Пишут: <c>HaulSystem</c>, <c>CraftSystem</c>.
/// Читают: <c>InventorySystem</c>, <c>JobSystem</c>.
/// </summary>
public interface IInventoryBus : IEventBus
{
}
