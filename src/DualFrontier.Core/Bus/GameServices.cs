using System;
using DualFrontier.Contracts.Bus;

namespace DualFrontier.Core.Bus;

/// <summary>
/// Композиция пяти доменных шин событий. Реализация <see cref="IGameServices"/>.
/// Каждая шина — отдельный экземпляр <see cref="DomainEventBus"/>, типизированный
/// под конкретный доменный интерфейс через thin-обёртки (или через кастинг —
/// детали реализации). Это точка входа для систем, модов и приложения ко всем шинам.
/// </summary>
internal sealed class GameServices : IGameServices
{
    // TODO: Фаза 1 — private readonly DomainEventBus _combat = new();
    // TODO: Фаза 1 — private readonly DomainEventBus _inventory = new();
    // TODO: Фаза 1 — private readonly DomainEventBus _magic = new();
    // TODO: Фаза 1 — private readonly DomainEventBus _pawns = new();
    // TODO: Фаза 1 — private readonly DomainEventBus _world = new();

    /// <inheritdoc />
    public ICombatBus Combat
    {
        get => throw new NotImplementedException("TODO: Фаза 1 — вернуть типизированную обёртку над _combat");
    }

    /// <inheritdoc />
    public IInventoryBus Inventory
    {
        get => throw new NotImplementedException("TODO: Фаза 1 — вернуть типизированную обёртку над _inventory");
    }

    /// <inheritdoc />
    public IMagicBus Magic
    {
        get => throw new NotImplementedException("TODO: Фаза 1 — вернуть типизированную обёртку над _magic");
    }

    /// <inheritdoc />
    public IPawnBus Pawns
    {
        get => throw new NotImplementedException("TODO: Фаза 1 — вернуть типизированную обёртку над _pawns");
    }

    /// <inheritdoc />
    public IWorldBus World
    {
        get => throw new NotImplementedException("TODO: Фаза 1 — вернуть типизированную обёртку над _world");
    }
}
