using System;
using System.Collections.Concurrent;
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
private int[] _versions = Array.Empty<int>();          // slot version array, indexed by entity Index
private int _nextIndex;           // next slot to allocate
private readonly Queue<int> _freeSlots = new();           // recycled indices
private readonly List<EntityId> _pendingDestroy = new();  // flushed end-of-phase

    /// <summary>
    /// Creates a new entity. Returns an <see cref="EntityId"/> with the slot index
    /// and its current version. Recycles freed slots when available.
    /// TODO: Фаза 1 — реализация.
    /// </summary>
    public EntityId CreateEntity()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }

    /// <summary>
    /// Marks entity for destruction. Increments the slot version immediately
    /// (so <see cref="IsAlive"/> returns false at once), but defers component
    /// removal to <see cref="FlushDestroyedEntities"/> called by the scheduler
    /// at end-of-phase.
    /// TODO: Фаза 1 — реализация.
    /// </summary>
    public void DestroyEntity(EntityId id)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }

    /// <summary>
    /// Returns <c>true</c> if the entity is still alive — i.e. the slot's current
    /// version matches <paramref name="id"/>'s version.
    /// Safe to call from any thread during a phase (versions are only written by
    /// <see cref="DestroyEntity"/>, which is serialised through the scheduler).
    /// TODO: Фаза 1 — реализация.
    /// </summary>
    public bool IsAlive(EntityId id)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }

    /// <summary>
    /// Flushes pending entity destruction: removes components from all stores
    /// and recycles slot indices. Called by <c>ParallelSystemScheduler</c> at
    /// the end of each phase, after all systems in the phase have completed.
    /// NOT called mid-phase — see INVARIANT in class doc.
    /// TODO: Фаза 1 — реализация.
    /// </summary>
    internal void FlushDestroyedEntities()
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }

    /// <summary>
    /// UNSAFE direct component access — only for <see cref="SystemExecutionContext"/>.
    /// Bypasses isolation checks. Do not call from game logic.
    /// TODO: Фаза 1 — реализация.
    /// </summary>
    internal T GetComponentUnsafe<T>(EntityId id) where T : IComponent
    {
        throw new NotImplementedException("TODO: Фаза 1 — реализация ядра ECS");
    }
}