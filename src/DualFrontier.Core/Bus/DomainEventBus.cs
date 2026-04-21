using System;
using DualFrontier.Contracts.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DualFrontier.Core.Bus;

/// <summary>
/// A thread-safe, generic event bus responsible for handling domain events within a specific game domain (e.g., Combat, Inventory).
/// </summary>
internal sealed class DomainEventBus
{
    // Stores the list of delegates/handlers keyed by the type of the event they handle.
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    /// <summary>
    /// Subscribes a handler action to a specific type of domain event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that this bus handles.</typeparam>
    /// <param name="handler">The action delegate to be called when an event of type TEvent is published.</param>
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        // Get or create the list for this event type.
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Delegate>();
        }

        // We must lock on the specific list instance to ensure thread safety during modification.
        var handlersList = _handlers[eventType];

        lock (handlersList)
        {
            if (!handlersList.Contains(handler))
            {
                handlersList.Add(handler);
            }
        }
    }

    /// <summary>
    /// Unsubscribes a specific handler action from a given domain event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="handler">The specific action delegate to remove.</param>
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        if (!_handlers.ContainsKey(eventType))
        {
            return; // No subscribers for this type.
        }

        var handlersList = _handlers[eventType];

        lock (handlersList)
        {
            // Attempt to remove the handler. No-op if not found.
            handlersList.Remove(handler);
        }
    }

    /// <summary>
    /// Publishes an event, notifying all subscribed handlers for that event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event being published.</typeparam>
    /// <param name="evt">The concrete instance of the domain event.</param>
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        if (!_handlers.TryGetValue(eventType, out var handlersList))
        {
            return; // No subscribers for this event type.
        }

        // Critical Step 1: Copy the list under lock to prevent deadlocks if a handler modifies subscriptions.
        List<Delegate> handlersCopy;
        lock (handlersList)
        {
            // Create a shallow copy of the delegate list.
            handlersCopy = new List<Delegate>(handlersList);
        }

        // Critical Step 2: Invoke handlers outside the lock.
        foreach (var handler in handlersCopy)
        {
            try
            {
                // We must cast the generic event back to its concrete type before invoking,
                // but since we know all handlers stored here are Action<TEvent>,
                // we can invoke it directly with the casted argument.
                ((Action<TEvent>)handler)?.Invoke(evt);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions thrown by individual handlers without stopping others.
                Console.WriteLine($"Error publishing event {eventType.Name}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Clears all subscriptions from the bus. Used for testing or scene reloads.
    /// </summary>
    public void Clear()
    {
        // Remove all entries in the dictionary under a lock if necessary,
        // but since ConcurrentDictionary handles internal state well, we just clear it.
        _handlers.Clear();
    }
}