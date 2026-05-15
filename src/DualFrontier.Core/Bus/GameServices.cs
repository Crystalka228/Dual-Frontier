namespace DualFrontier.Core.Bus;

using System;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

/// <summary>
/// Aggregates all domain event buses into a single service locator pattern,
/// implementing the IGameServices interface. This class provides access points
/// for various core game systems' event buses. Also implements internal
/// <see cref="IDeferredFlush"/> so the scheduler can drain deferred events
/// per phase without downcasting.
/// </summary>
internal sealed class GameServices : IGameServices, IDeferredFlush
{
    private readonly CombatBus _combatBus = new();
    private readonly InventoryBus _inventoryBus = new();
    private readonly MagicBus _magicBus = new();
    private readonly WorldBus _worldBus = new();
    private readonly PawnBus _pawnBus = new();

    /// <inheritdoc/>
    public ICombatBus Combat => _combatBus;

    /// <inheritdoc/>
    public IInventoryBus Inventory => _inventoryBus;

    /// <inheritdoc/>
    public IMagicBus Magic => _magicBus;

    /// <inheritdoc/>
    public IWorldBus World => _worldBus;

    /// <inheritdoc/>
    public IPawnBus Pawns => _pawnBus;

    /// <summary>
    /// Clears all underlying event buses. Should be called between scenes or during testing
    /// to prevent stale events from affecting subsequent game states.
    /// </summary>
    public void Clear()
    {
        _combatBus.Clear();
        _inventoryBus.Clear();
        _magicBus.Clear();
        _worldBus.Clear();
        _pawnBus.Clear();
    }

    /// <summary>
    /// Drains the deferred queue of every owned bus. Called by
    /// <c>ParallelSystemScheduler.ExecutePhase</c> after the parallel barrier
    /// of each phase. See <see cref="DomainEventBus.FlushDeferred"/>.
    /// </summary>
    public void FlushDeferred()
    {
        _combatBus.FlushDeferred();
        _inventoryBus.FlushDeferred();
        _magicBus.FlushDeferred();
        _worldBus.FlushDeferred();
        _pawnBus.FlushDeferred();
    }
}

internal sealed class CombatBus : ICombatBus
{
    private readonly DomainEventBus _bus = new();
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Subscribe(handler);
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Unsubscribe(handler);
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent => _bus.Publish(evt);
    public void Clear() => _bus.Clear();
    public void FlushDeferred() => _bus.FlushDeferred();
}

internal sealed class InventoryBus : IInventoryBus
{
    private readonly DomainEventBus _bus = new();
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Subscribe(handler);
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Unsubscribe(handler);
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent => _bus.Publish(evt);
    public void Clear() => _bus.Clear();
    public void FlushDeferred() => _bus.FlushDeferred();
}

internal sealed class MagicBus : IMagicBus
{
    private readonly DomainEventBus _bus = new();
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Subscribe(handler);
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Unsubscribe(handler);
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent => _bus.Publish(evt);
    public void Clear() => _bus.Clear();
    public void FlushDeferred() => _bus.FlushDeferred();
}

internal sealed class WorldBus : IWorldBus
{
    private readonly DomainEventBus _bus = new();
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Subscribe(handler);
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Unsubscribe(handler);
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent => _bus.Publish(evt);
    public void Clear() => _bus.Clear();
    public void FlushDeferred() => _bus.FlushDeferred();
}

internal sealed class PawnBus : IPawnBus
{
    private readonly DomainEventBus _bus = new();
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Subscribe(handler);
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Unsubscribe(handler);
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent => _bus.Publish(evt);
    public void Clear() => _bus.Clear();
    public void FlushDeferred() => _bus.FlushDeferred();
}
