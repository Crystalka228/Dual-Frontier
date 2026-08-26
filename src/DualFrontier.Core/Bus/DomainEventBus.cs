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
///
/// <para>
/// <b>Type-key lifetime (F-60(a) sibling).</b> Both maps here are keyed by
/// <see cref="Type"/>, and a strong <see cref="Type"/> key roots the
/// <c>AssemblyLoadContext</c> that produced it — for a mod that declares event
/// types in its own regular (collectible) assembly, an unevicted key means the mod
/// can never be reclaimed. The subscription map now holds the invariant that
/// <b>no Type key outlives its last subscriber</b>: <see cref="Unsubscribe"/> drops
/// the key when its list empties. Combined with unload step 1
/// (<c>RestrictedModApi.UnsubscribeAll</c>, which removes every subscription a mod
/// owns), a mod's event-type keys are gone by the time its ALC is checked.
/// </para>
///
/// <para>
/// The delivery-mode cache is bounded differently and the difference is
/// deliberate: its keys are the event types that have been PUBLISHED, and the bus
/// holds no owner index for those, so there is nothing to evict them BY at unload.
/// Building one would mean an owner-indexed mode cache, which is К10.4-adjacent
/// design and out of scope here. What bounds it today is the supported topology:
/// mod-authored event types are vended by shared-kind contracts assemblies
/// (MOD_OS §1.4), and <c>SharedModLoadContext</c> is non-collectible BY INVARIANT,
/// so those keys root nothing that was ever going to be unloaded. A regular-ALC
/// event type reaching <see cref="Publish"/> would escape that bound; it is
/// carried as a findings-ledger row (analyzer candidate), not silently assumed
/// impossible. Making the cache an instance field at least ends it with the
/// session rather than the process.
/// </para>
/// </summary>
internal sealed class DomainEventBus
{
    // F-60(a) sibling: INSTANCE, not static. There is exactly one DomainEventBus
    // per session (GameServices owns it; there is no production `new` site), so a
    // static bought nothing and cost the ability to ever release its keys -- a
    // process-lived Type-keyed map that outlived the session that filled it.
    private readonly ConcurrentDictionary<Type, DeliveryMode> _modeCache = new();

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

        SystemExecutionContext? captured = SystemExecutionContext.Current;
        Action<IEvent> invoker = e => handler((TEvent)e);
        var sub = new Subscription(handler, invoker, captured);

        while (true)
        {
            List<Subscription> list = _handlers.GetOrAdd(eventType, _ => new List<Subscription>());

            lock (list)
            {
                // Unsubscribe now drops the Type key when its list empties, so the list
                // we just fetched may have been evicted while we waited for its lock.
                // Adding to an evicted list would silently lose the subscription --
                // nothing would ever read it again. Re-check identity under the lock and
                // retry against whatever list is mapped now.
                if (!_handlers.TryGetValue(eventType, out List<Subscription>? current)
                    || !ReferenceEquals(current, list))
                    continue;

                for (int i = 0; i < list.Count; i++)
                {
                    // Delegate value equality (Target + Method), not ReferenceEquals: a method-group
                    // handler (e.g. OnFoo) allocates a fresh delegate instance on every conversion, so
                    // reference identity would never match and duplicates would accumulate (F19).
                    if (list[i].Original.Equals(handler))
                        return;
                }
                list.Add(sub);
                return;
            }
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
        Type eventType = typeof(TEvent);
        if (!_handlers.TryGetValue(eventType, out List<Subscription>? list))
            return;

        lock (list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                // Delegate value equality (Target + Method), not ReferenceEquals — see Subscribe (F19).
                if (list[i].Original.Equals(handler))
                {
                    list.RemoveAt(i);
                    break;
                }
            }

            if (list.Count == 0)
            {
                // F-60(a) sibling: the Type key dies with its last subscriber. Previously
                // the key and its empty list persisted for the session, which roots a
                // collectible ALC for any mod that declares its event types in its own
                // regular assembly. Compare-and-remove (the ICollection overload) drops the
                // entry ONLY while it still maps to THIS list, so a concurrent Subscribe
                // that already installed a replacement is never clobbered.
                ((ICollection<KeyValuePair<Type, List<Subscription>>>)_handlers)
                    .Remove(new KeyValuePair<Type, List<Subscription>>(eventType, list));
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

    private DeliveryMode GetDeliveryMode(Type eventType) =>
        _modeCache.GetOrAdd(eventType, ResolveDeliveryMode);

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
