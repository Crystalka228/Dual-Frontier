using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Bus;

/// <summary>
/// Thread-safe per-domain event bus. Supports three delivery modes resolved
/// from event-type attributes (cached): synchronous (default), <see cref="DeferredAttribute"/>
/// (queued for the next phase boundary), and <see cref="ImmediateAttribute"/>
/// (synchronous, never queued).
///
/// Deferred handlers are dispatched by <c>ParallelSystemScheduler</c> after
/// every phase via <see cref="FlushDeferred"/>. Each subscription captures the
/// <see cref="SystemExecutionContext"/> active at <see cref="Subscribe"/> time;
/// that context is re-pushed during deferred dispatch so handlers may mutate
/// components within their own declared <c>[SystemAccess]</c> rights.
/// </summary>
internal sealed class DomainEventBus
{
    private static readonly ConcurrentDictionary<Type, DeliveryMode> ModeCache = new();

    private readonly ConcurrentDictionary<Type, List<Subscription>> _handlers = new();
    private readonly ConcurrentQueue<DeferredItem> _deferred = new();

    /// <summary>
    /// Subscribes a handler for events of type <typeparamref name="TEvent"/>.
    /// Captures the calling thread's <see cref="SystemExecutionContext"/> (if
    /// any) so deferred dispatch can re-push the right guard. Duplicate
    /// subscriptions (same handler — matched by delegate value equality) are ignored.
    /// </summary>
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        Type eventType = typeof(TEvent);

        List<Subscription> list = _handlers.GetOrAdd(eventType, _ => new List<Subscription>());
        SystemExecutionContext? captured = SystemExecutionContext.Current;
        Action<IEvent> invoker = e => handler((TEvent)e);
        var sub = new Subscription(handler, invoker, captured);

        lock (list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                // Delegate value equality (Target + Method), not ReferenceEquals: a method-group
                // handler (e.g. OnFoo) allocates a fresh delegate instance on every conversion, so
                // reference identity would never match and duplicates would accumulate (F19).
                if (list[i].Original.Equals(handler))
                    return;
            }
            list.Add(sub);
        }
    }

    /// <summary>
    /// Unsubscribes a previously registered handler by delegate value equality, so a method-group
    /// handler unsubscribes correctly even though each conversion produces a distinct delegate
    /// instance. No-op if the handler was never registered.
    /// </summary>
    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        if (!_handlers.TryGetValue(typeof(TEvent), out List<Subscription>? list))
            return;

        lock (list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                // Delegate value equality (Target + Method), not ReferenceEquals — see Subscribe (F19).
                if (list[i].Original.Equals(handler))
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Publishes an event. Default and <see cref="ImmediateAttribute"/> events
    /// are delivered synchronously to current subscribers. <see cref="DeferredAttribute"/>
    /// events are queued and dispatched at the next <see cref="FlushDeferred"/>
    /// call (typically the next phase boundary).
    /// </summary>
    public void Publish<TEvent>(TEvent evt) where TEvent : IEvent
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        Type eventType = typeof(TEvent);

        if (GetDeliveryMode(eventType) == DeliveryMode.Deferred)
        {
            _deferred.Enqueue(new DeferredItem(eventType, evt));
            return;
        }

        DeliverSync(eventType, evt);
    }

    /// <summary>
    /// Drains the deferred queue, dispatching every queued event to its current
    /// subscribers. Each subscription's captured execution context is re-pushed
    /// for the duration of the handler invocation so mutating handlers stay
    /// within their declared <c>[SystemAccess]</c> rights.
    ///
    /// Snapshot-based: events queued by handlers during this drain go to the
    /// queue and will be dispatched on the NEXT call, not the current one. This
    /// keeps each phase boundary bounded and avoids re-entrant unbounded loops.
    /// </summary>
    public void FlushDeferred()
    {
        if (_deferred.IsEmpty) return;

        var batch = new List<DeferredItem>();
        while (_deferred.TryDequeue(out DeferredItem item))
            batch.Add(item);

        foreach (DeferredItem item in batch)
        {
            if (!_handlers.TryGetValue(item.EventType, out List<Subscription>? list))
                continue;

            Subscription[] snapshot;
            lock (list)
                snapshot = list.ToArray();

            foreach (Subscription sub in snapshot)
                InvokeDeferred(sub, item.Evt);
        }
    }

    /// <summary>
    /// Clears all subscriptions and pending deferred events. Used by tests and
    /// scene reloads.
    /// </summary>
    public void Clear()
    {
        _handlers.Clear();
        while (_deferred.TryDequeue(out _)) { }
    }

    /// <summary>
    /// Drops every queued deferred event without dispatching, returning the count
    /// discarded. Shutdown-transaction step S3 (RESOURCE_OWNERSHIP_AND_LIFETIME
    /// section 4.1): after the quiesce fence no handler may run, so pending
    /// deferred work is dropped -- counted for exit diagnostics -- not flushed.
    /// Handlers are left intact (nothing publishes post-quiesce).
    /// </summary>
    public int DropDeferred()
    {
        int dropped = 0;
        while (_deferred.TryDequeue(out _)) dropped++;
        return dropped;
    }

    private void DeliverSync(Type eventType, IEvent evt)
    {
        if (!_handlers.TryGetValue(eventType, out List<Subscription>? list))
            return;

        Subscription[] snapshot;
        lock (list)
            snapshot = list.ToArray();

        foreach (Subscription sub in snapshot)
        {
            try
            {
                sub.Invoker(evt);
            }
            catch (Exception ex)
            {
                // D2 origin-asymmetric fault policy (CONCURRENCY_AND_MEMORY_MODEL §7):
                // a mod-origin subscriber fault routes to the IModFaultSink and
                // delivery continues to the remaining subscribers; a core-origin
                // fault (or one whose subscription captured no system context) is
                // recorded and rethrown -- fail-fast. Replaces the former
                // swallow-to-console, which hid every fault from the mod-fault
                // lifecycle and from diagnostics (CMM §7 "even the good mode
                // under-reports").
                SystemExecutionContext? ctx = sub.CapturedContext;
                if (ctx is not null && ctx.RouteFault(ex, out _) == FaultDisposition.ContainedMod)
                    continue;
                throw;
            }
        }
    }

    private static void InvokeDeferred(Subscription sub, IEvent evt)
    {
        SystemExecutionContext? ctx = sub.CapturedContext;
        bool pushed = false;
        if (ctx is not null)
        {
            SystemExecutionContext.PushContext(ctx);
            pushed = true;
        }
        try
        {
            sub.Invoker(evt);
        }
        catch (Exception ex)
        {
            // D2 parity with DeliverSync (CONCURRENCY_AND_MEMORY_MODEL §7):
            // per-subscriber isolation MUST be symmetric across delivery modes.
            // Previously InvokeDeferred had no catch at all, so a faulting handler
            // unwound FlushDeferred -> ExecutePhase -> RunLoop and killed the
            // simulation thread -- a mod's queued-mode handler could crash the game
            // that its sync handler could not. Mod-origin faults route to the sink
            // and are contained (remaining subscribers still deliver); core-origin
            // faults rethrow (fail-fast). The finally still pops the context.
            if (ctx is not null && ctx.RouteFault(ex, out _) == FaultDisposition.ContainedMod)
                return;
            throw;
        }
        finally
        {
            if (pushed)
                SystemExecutionContext.PopContext();
        }
    }

    private static DeliveryMode GetDeliveryMode(Type eventType) =>
        ModeCache.GetOrAdd(eventType, ResolveDeliveryMode);

    private static DeliveryMode ResolveDeliveryMode(Type eventType)
    {
        if (eventType.GetCustomAttribute<DeferredAttribute>(inherit: false) is not null)
            return DeliveryMode.Deferred;
        if (eventType.GetCustomAttribute<ImmediateAttribute>(inherit: false) is not null)
            return DeliveryMode.Immediate;
        return DeliveryMode.Sync;
    }

    private enum DeliveryMode
    {
        Sync,
        Deferred,
        Immediate
    }

    private readonly record struct DeferredItem(Type EventType, IEvent Evt);

    private sealed record Subscription(
        Delegate Original,
        Action<IEvent> Invoker,
        SystemExecutionContext? CapturedContext);
}
