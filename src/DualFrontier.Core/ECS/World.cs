using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Registry of all entities and their components.
/// One store per component type; queries return intersections of those stores.
///
/// INVARIANT — deferred destruction:
///   <c>DestroyEntity</c> must NOT remove components from their stores immediately.
///   Removal happens at the END of the current scheduler phase, after all systems
///   in that phase have finished iterating. This makes lazy Query iterators safe:
///   a system that holds a reference to an entity mid-iteration will not see a
///   "swap-with-last" hole in the SparseSet dense array.
///
///   Consequence: between <c>DestroyEntity</c> and end-of-phase cleanup,
///   <c>IsAlive</c> returns <c>false</c> (version incremented immediately),
///   but <c>GetComponentUnsafe</c> may still return stale data for that slot.
///   Systems MUST check <c>IsAlive</c> before acting on a component.
///
///   WARNING: if you ever relax the no-async rule in Domain, this invariant
///   breaks — <c>await</c> points can suspend a system mid-iteration and let
///   another system run, defeating the phase-boundary protection. Do not
///   introduce async/Task in Domain code.
///
/// ACCESS RULE:
///   Systems must never call World methods directly. All access goes through
///   <see cref="SystemExecutionContext"/>, which enforces the [SystemAccess]
///   declaration and throws <see cref="IsolationViolationException"/> on violations.
/// </summary>
internal sealed class World
{
    private const int InitialCapacity = 256;

    private readonly ConcurrentDictionary<Type, IComponentStore> _stores = new();
    private int[] _versions;
    private int _nextIndex;
    private readonly Queue<int> _freeSlots = new();
    private readonly List<EntityId> _pendingDestroy = new();

    internal World()
    {
        _versions = new int[InitialCapacity];
        _nextIndex = 1; // Index=0 reserved as Invalid
    }

    /// <summary>
    /// Creates a new entity. Recycles freed slots when available;
    /// otherwise allocates the next slot, growing the version array if needed.
    /// </summary>
    public EntityId CreateEntity()
    {
        if (_freeSlots.Count > 0)
        {
            int recycled = _freeSlots.Dequeue();
            return new EntityId(recycled, _versions[recycled]);
        }

        if (_nextIndex >= _versions.Length)
            Array.Resize(ref _versions, _versions.Length * 2);

        int index = _nextIndex++;
        return new EntityId(index, _versions[index]);
    }

    /// <summary>
    /// Returns <c>true</c> if the entity slot version matches — entity is alive.
    /// Safe to call from any thread (versions written only by
    /// <see cref="DestroyEntity"/>, serialised through the scheduler).
    /// </summary>
    public bool IsAlive(EntityId id)
    {
        return id.Index > 0
            && id.Index < _versions.Length
            && id.Version == _versions[id.Index];
    }

    /// <summary>
    /// Marks entity for destruction. Increments slot version immediately
    /// (so <see cref="IsAlive"/> returns <c>false</c> at once), then queues
    /// the entity for component removal at end-of-phase via
    /// <see cref="FlushDestroyedEntities"/>.
    /// </summary>
    public void DestroyEntity(EntityId id)
    {
        if (!IsAlive(id)) return;
        _versions[id.Index]++;
        _pendingDestroy.Add(id);
    }

    /// <summary>
    /// Flushes pending destruction: removes all components from stores and
    /// recycles slot indices. Called by <c>ParallelSystemScheduler</c> at
    /// end-of-phase only — never mid-phase.
    /// </summary>
    internal void FlushDestroyedEntities()
    {
        foreach (EntityId id in _pendingDestroy)
        {
            foreach (IComponentStore store in _stores.Values)
                RemoveFromStore(store, id);

            _freeSlots.Enqueue(id.Index);
        }
        _pendingDestroy.Clear();
    }

    /// <summary>
    /// Adds or overwrites a component for the given entity.
    /// </summary>
    public void AddComponent<T>(EntityId id, T component) where T : IComponent
    {
        GetOrCreateStore<T>().Add(id, component);
    }

    /// <summary>
    /// Removes a component from the given entity. No-op if absent.
    /// </summary>
    public void RemoveComponent<T>(EntityId id) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out IComponentStore? store))
            ((ComponentStore<T>)store).Remove(id);
    }

    /// <summary>
    /// Returns <c>true</c> if the entity has a component of type {T}.
    /// </summary>
    public bool HasComponent<T>(EntityId id) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out IComponentStore? store))
            return ((ComponentStore<T>)store).Has(id);
        return false;
    }

    /// <summary>
    /// Tries to get a component. Returns <c>false</c> if absent.
    /// </summary>
    public bool TryGetComponent<T>(EntityId id, out T component) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out IComponentStore? store))
            return ((ComponentStore<T>)store).TryGet(id, out component);
        component = default!;
        return false;
    }

    /// <summary>
    /// UNSAFE direct component access — only for <see cref="SystemExecutionContext"/>.
    /// Bypasses isolation checks. Do not call from game logic.
    /// </summary>
    internal T GetComponentUnsafe<T>(EntityId id) where T : IComponent
    {
        return GetOrCreateStore<T>().Get(id);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private ComponentStore<T> GetOrCreateStore<T>() where T : IComponent
    {
        return (ComponentStore<T>)_stores.GetOrAdd(typeof(T), _ => new ComponentStore<T>());
    }

    private static void RemoveFromStore(IComponentStore store, EntityId id)
    {
        if (store is IRemovable removable)
            removable.Remove(id);
    }
}

/// <summary>
/// Internal interface allowing <see cref="World"/> to call
/// <see cref="Remove"/> on a non-generic <see cref="IComponentStore"/> reference.
/// </summary>
internal interface IRemovable
{
    void Remove(EntityId id);
}