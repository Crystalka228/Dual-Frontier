using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Building;

/// <summary>
/// Хранилище предметов. <c>Items</c> содержит EntityId каждого хранимого
/// предмета (предметы — отдельные сущности). Добавление/удаление —
/// только InventorySystem через события <c>ItemAddedEvent</c> / <c>ItemRemovedEvent</c>.
/// </summary>
public sealed class StorageComponent : IComponent
{
    // TODO: public int Capacity;
    // TODO: public List<EntityId> Items = new();
}
