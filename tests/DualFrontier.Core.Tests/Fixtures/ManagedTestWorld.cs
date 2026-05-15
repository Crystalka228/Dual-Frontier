using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Tests.Fixtures;

/// <summary>
/// Managed-side ECS world preserved as a test-project type post-K8.3+K8.4
/// cutover. Production storage now flows through
/// <see cref="DualFrontier.Core.Interop.NativeWorld"/>; this type survives
/// because <c>DualFrontier.Core.Tests</c> still tests the managed-world
/// semantics directly (deferred destruction, sparse-set storage,
/// query enumeration).
///
/// Identical to the pre-cutover <c>DualFrontier.Core.ECS.World</c> in
/// behaviour — only the name and the visibility (widened to <c>public</c>
/// so test code in this assembly can construct it without
/// <c>InternalsVisibleTo</c> grants).
///
/// INVARIANT — deferred destruction:
///   <c>DestroyEntity</c> must NOT remove components from their stores immediately.
///   Removal happens at the END of the current scheduler phase, after all systems
///   in that phase have finished iterating. Between <c>DestroyEntity</c> and
///   end-of-phase cleanup, <c>IsAlive</c> returns <c>false</c> (version
///   incremented immediately), but <c>GetComponentUnsafe</c> may still return
///   stale data for that slot. Systems MUST check <c>IsAlive</c> before acting.
/// </summary>
public sealed class ManagedTestWorld
{
    private const int InitialCapacity = 256;

    private readonly ConcurrentDictionary<Type, IComponentStore> _stores = new();
    private int[] _versions;
    private int _nextIndex;
    private readonly Queue<int> _freeSlots = new();
    private readonly List<EntityId> _pendingDestroy = new();

    public ManagedTestWorld()
    {
        _versions = new int[InitialCapacity];
        _nextIndex = 1; // Index=0 reserved as Invalid
    }

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

    public bool IsAlive(EntityId id)
    {
        return id.Index > 0
            && id.Index < _versions.Length
            && id.Version == _versions[id.Index];
    }

    public void DestroyEntity(EntityId id)
    {
        if (!IsAlive(id)) return;
        _versions[id.Index]++;
        _pendingDestroy.Add(id);
    }

    public void FlushDestroyedEntities()
    {
        foreach (EntityId id in _pendingDestroy)
        {
            foreach (IComponentStore store in _stores.Values)
                RemoveFromStore(store, id);

            _freeSlots.Enqueue(id.Index);
        }
        _pendingDestroy.Clear();
    }

    public void AddComponent<T>(EntityId id, T component) where T : IComponent
    {
        GetOrCreateStore<T>().Add(id, component);
    }

    public void RemoveComponent<T>(EntityId id) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out IComponentStore? store))
            ((ComponentStore<T>)store).Remove(id);
    }

    public bool HasComponent<T>(EntityId id) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out IComponentStore? store))
            return ((ComponentStore<T>)store).Has(id);
        return false;
    }

    public bool TryGetComponent<T>(EntityId id, out T component) where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out IComponentStore? store))
            return ((ComponentStore<T>)store).TryGet(id, out component);
        component = default!;
        return false;
    }

    public T GetComponentUnsafe<T>(EntityId id) where T : IComponent
    {
        return GetOrCreateStore<T>().Get(id);
    }

    public void SetComponent<T>(EntityId id, T component) where T : IComponent
    {
        GetOrCreateStore<T>().Add(id, component);
    }

    public IEnumerable<EntityId> GetEntitiesWith<T>() where T : IComponent
    {
        if (!_stores.TryGetValue(typeof(T), out IComponentStore? store))
            yield break;

        foreach (int index in ((ComponentStore<T>)store).EnumerateIndices())
        {
            if (index > 0 && index < _versions.Length)
                yield return new EntityId(index, _versions[index]);
        }
    }

    public int GetComponentCount<T>() where T : IComponent
    {
        if (_stores.TryGetValue(typeof(T), out IComponentStore? store))
            return ((ComponentStore<T>)store).Count;
        return 0;
    }

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
/// Internal marker for storing all <see cref="ComponentStore{T}"/> in a
/// single non-generic collection. No methods may be added.
/// </summary>
internal interface IComponentStore
{
}

/// <summary>
/// Internal interface allowing <see cref="ManagedTestWorld"/> to call
/// <see cref="Remove"/> on a non-generic <see cref="IComponentStore"/> reference.
/// </summary>
internal interface IRemovable
{
    void Remove(EntityId id);
}

/// <summary>
/// Typed storage for components of a single type.
/// Implemented with the SparseSet pattern.
/// </summary>
internal sealed class ComponentStore<T> : IComponentStore, IRemovable where T : IComponent
{
    private int[] _sparse;
    private T[] _dense;
    private int[] _denseToIndex;

    public int Count { get; private set; } = 0;

    private const int DefaultCapacity = 256;

    internal ComponentStore()
    {
        _sparse = new int[DefaultCapacity];
        Array.Fill(_sparse, -1);
        _dense = Array.Empty<T>();
        _denseToIndex = Array.Empty<int>();
    }

    private void EnsureCapacity(EntityId id)
    {
        if (id.Index >= _sparse.Length)
        {
            int oldLength = _sparse.Length;
            int newCapacity = System.Math.Max(oldLength * 2, id.Index + 1);
            var newSparse = new int[newCapacity];

            Array.Copy(_sparse, newSparse, oldLength);
            Array.Fill(newSparse, -1, oldLength, newCapacity - oldLength);
            _sparse = newSparse;
        }
    }

    public void Add(EntityId id, T component)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Cannot add a component with an invalid EntityId.", nameof(id));
        }

        EnsureCapacity(id);

        int denseIndex = _sparse[id.Index];

        if (denseIndex != -1)
        {
            _dense[denseIndex] = component;
            return;
        }

        if (_dense.Length <= Count)
        {
            int requiredCapacity = System.Math.Max(_dense.Length * 2, Count + 1);
            Array.Resize(ref _dense, requiredCapacity);
            Array.Resize(ref _denseToIndex, requiredCapacity);
        }

        int newDenseIndex = Count;
        _dense[newDenseIndex] = component;
        _denseToIndex[newDenseIndex] = id.Index;
        _sparse[id.Index] = newDenseIndex;
        Count++;
    }

    public void Remove(EntityId id)
    {
        int denseIndexToRemove = _sparse[id.Index];
        if (denseIndexToRemove == -1)
        {
            return;
        }

        if (Count == 1 && denseIndexToRemove == 0)
        {
             _sparse[id.Index] = -1;
             Count = 0;
             return;
        }

        int lastDenseIndex = Count - 1;
        int idOfLastComponentIndex = _denseToIndex[lastDenseIndex];

        if (id.Index == idOfLastComponentIndex)
        {
            _sparse[id.Index] = -1;
            Count--;
            return;
        }

        _dense[denseIndexToRemove] = _dense[lastDenseIndex];
        _denseToIndex[denseIndexToRemove] = idOfLastComponentIndex;
        _sparse[idOfLastComponentIndex] = denseIndexToRemove;
        _sparse[id.Index] = -1;

        Count--;
    }

    public bool Has(EntityId id)
    {
        return _sparse[id.Index] != -1;
    }

    public T Get(EntityId id)
    {
        if (!Has(id))
        {
            throw new InvalidOperationException($"Component {typeof(T).Name} not found for entity {id}.");
        }

        int denseIndex = _sparse[id.Index];
        return _dense[denseIndex];
    }

    public bool TryGet(EntityId id, out T component)
    {
        if (Has(id))
        {
            component = _dense[_sparse[id.Index]];
            return true;
        }

        component = default!;
        return false;
    }

    public ReadOnlySpan<T> All()
    {
        return new ReadOnlySpan<T>(_dense, 0, Count);
    }

    public IEnumerable<int> EnumerateIndices()
    {
        for (int i = 0; i < Count; i++)
            yield return _denseToIndex[i];
    }
}
