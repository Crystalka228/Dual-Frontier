namespace DualFrontier.Core.Bus;

using System;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

/// <summary>
/// Aggregates the managed domain-event dispatch behind the <see cref="IGameServices"/>
/// service-locator surface. W2/BD-3 collapsed the former five genre buses
/// (Combat/Inventory/Magic/Pawns/World) into ONE type-keyed <see cref="DomainEventBus"/>:
/// the genre taxonomy no longer partitions routing, and every <see cref="IGameServices"/>
/// getter returns the same unified dispatch through <see cref="UnifiedGenreBus"/>. The
/// getters survive only as an engine-internal harness bridge, so the live harness systems
/// keep their <c>Services.&lt;Bus&gt;.Publish</c> call shape until W5, when <c>SystemBase</c>
/// (and this bridge) retires. Delivery is type-keyed, so merging the five per-genre
/// subscriber tables into one preserves every existing publisher/subscriber pair -- no
/// production event type spans genres -- while letting mod publication reach subscribers
/// uniformly. Also implements internal <see cref="IDeferredFlush"/> so the scheduler can
/// drain deferred events per phase without downcasting.
/// </summary>
internal sealed class GameServices : IGameServices, IDeferredFlush
{
    private readonly DomainEventBus _bus = new();
    private readonly UnifiedGenreBus _genre;

    public GameServices() => _genre = new UnifiedGenreBus(_bus);

    /// <inheritdoc/>
    public ICombatBus Combat => _genre;

    /// <inheritdoc/>
    public IInventoryBus Inventory => _genre;

    /// <inheritdoc/>
    public IMagicBus Magic => _genre;

    /// <inheritdoc/>
    public IPawnBus Pawns => _genre;

    /// <inheritdoc/>
    public IWorldBus World => _genre;

    /// <summary>
    /// Clears the unified event bus. Should be called between scenes or during testing
    /// to prevent stale events from affecting subsequent game states.
    /// </summary>
    public void Clear() => _bus.Clear();

    /// <summary>
    /// Drains the deferred queue of the unified bus. Called by
    /// <c>ParallelSystemScheduler.ExecutePhase</c> after the parallel barrier of each
    /// phase. See <see cref="DomainEventBus.FlushDeferred"/>.
    /// </summary>
    public void FlushDeferred() => _bus.FlushDeferred();

    /// <summary>
    /// Drops the deferred queue of the unified bus without dispatching, returning the
    /// total count discarded. Shutdown-transaction step S3 (EQ_A2).
    /// </summary>
    public int DropDeferred() => _bus.DropDeferred();
}

/// <summary>
/// The single managed dispatch behind all five <see cref="IGameServices"/> genre getters
/// after the W2/BD-3 taxonomy collapse. It implements the five (empty) genre marker
/// interfaces so the harness bridge's <c>Services.Combat</c> / <c>Services.Pawns</c> / ...
/// call sites keep compiling unchanged, but each routes to the same type-keyed
/// <see cref="DomainEventBus"/>. Retires with the genre markers at W5.
/// </summary>
internal sealed class UnifiedGenreBus : ICombatBus, IInventoryBus, IMagicBus, IPawnBus, IWorldBus
{
    private readonly DomainEventBus _bus;

    public UnifiedGenreBus(DomainEventBus bus) => _bus = bus;

    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Subscribe(handler);
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent => _bus.Unsubscribe(handler);
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent => _bus.Publish(evt);
}
