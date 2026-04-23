using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Event bus for a single domain (Combat, Inventory, Magic, Pawn, World).
/// Per-domain buses reduce lock contention and simplify profiling.
///
/// Publish and Subscribe are thread-safe.
/// Delivery is synchronous within the current scheduler phase by default;
/// mark the event type with <c>[Deferred]</c> to queue for the next phase.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to all current subscribers.
    /// Default delivery is synchronous within the current phase.
    /// If the event type carries <c>[Deferred]</c>, delivery is queued for
    /// the next phase. If it carries <c>[Immediate]</c>, delivery interrupts
    /// the current phase.
    /// </summary>
    void Publish<T>(T evt) where T : IEvent;

    /// <summary>
    /// Subscribes a handler for events of type <typeparamref name="T"/>.
    /// Handlers are invoked synchronously from <see cref="Publish{T}(T)"/>;
    /// they must not block, since doing so stalls the entire phase.
    /// </summary>
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    /// <summary>
    /// Unsubscribes a previously registered handler. Mandatory when the
    /// subscriber is disposed — otherwise the delegate keeps a reference to
    /// the subscriber and leaks memory.
    /// </summary>
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;
}
