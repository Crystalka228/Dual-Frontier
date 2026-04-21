namespace DualFrontier.Core.Bus;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

/// <summary>
/// Collects Intent events published during a scheduler phase and delivers them as a batch 
/// at the start of the next phase. This implements the two-step Intent→Granted/Refused pattern.
/// </summary>
internal sealed class IntentBatcher
{
    // Stores pending intents by event type.
    private readonly ConcurrentDictionary<Type, List<IEvent>> _pending = new();
    private readonly object _lock = new();

    /// <summary>
    /// Enqueues an intent for batch delivery in the next phase.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event (must inherit from IEvent).</typeparam>
    /// <param name="intent">The event instance to enqueue.</param>
    public void Enqueue<TEvent>(TEvent intent) where TEvent : IEvent
    {
        lock (_lock)
        {
            if (!_pending.ContainsKey(typeof(TEvent)))
            {
                _pending[typeof(TEvent)] = new List<IEvent>();
            }
            _pending[typeof(TEvent)].Add((IEvent)intent);
        }
    }

    /// <summary>
    /// Dequeues all pending intents of a given type.
    /// Returns snapshot list and clears the queue for that type.
    /// </summary>
    /// <typeparam name="TEvent">The expected type of the event.</typeparam>
    /// <returns>A list containing copies of all pending events of type {TEvent}. Returns an empty list if no intents are pending.</returns>
    public List<TEvent> Flush<TEvent>() where TEvent : IEvent
    {
        lock (_lock)
        {
            if (!_pending.ContainsKey(typeof(TEvent)) || _pending[typeof(TEvent)].Count == 0)
            {
                return new List<TEvent>();
            }

            // Take a snapshot of the current list by iterating and casting.
            List<IEvent> events = _pending[typeof(TEvent)];
            List<TEvent> snapshot = new List<TEvent>();
            foreach (IEvent item in events)
            {
                snapshot.Add((TEvent)item);
            }
            
            // Clear the original queue for this type
            events.Clear(); 
            
            return snapshot;
        }
    }

    /// <summary>
    /// Flushes all pending intents of all types.
    /// Returns dictionary mapping event Type to list of events. Clears all queues.
    /// </summary>
    /// <returns>A dictionary containing snapshots of all pending intents, keyed by their type.</returns>
    public Dictionary<Type, List<IEvent>> FlushAll()
    {
        lock (_lock)
        {
            var result = new Dictionary<Type, List<IEvent>>();

            // Copy contents and clear the original dictionary structure
            foreach (var kvp in _pending.ToList())
            {
                result[kvp.Key] = new List<IEvent>(kvp.Value); // Create snapshot copy
                kvp.Value.Clear(); // Clear original list
            }

            // Note: We must clear the dictionary itself if we want to fully reset state, 
            // but since we only modify the internal lists for now, let's just clear all underlying lists.
            _pending.Clear(); 
            
            return result;
        }
    }

    /// <summary>
    /// Returns count of pending intents for a given type without flushing them.
    /// </summary>
    /// <typeparam name="TEvent">The expected type of the event.</typeparam>
    /// <returns>The number of pending events of type {TEvent}. Returns 0 if no intents are pending.</returns>
    public int PendingCount<TEvent>() where TEvent : IEvent
    {
        lock (_lock)
        {
            if (!_pending.ContainsKey(typeof(TEvent)))
            {
                return 0;
            }
            return _pending[typeof(TEvent)].Count;
        }
    }

    /// <summary>
    /// Clears all pending intents without delivering them or generating any output.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _pending.Clear();
        }
    }
}