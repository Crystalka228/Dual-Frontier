using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Не-дженерик маркер для хранения всех ComponentStore{T}
/// в одной коллекции (обычно <c>ConcurrentDictionary{Type, IComponentStore}</c>
/// внутри <see cref="World"/>). Никаких методов сюда добавлять нельзя —
/// иначе теряется типобезопасность конкретных store-ов.
/// </summary>
internal interface IComponentStore
{
}

/// <summary>
/// Типизированный storage для компонентов одного типа.
/// Реализован с использованием паттерна SparseSet: плотный массив значений +
/// разреженный массив индексов по EntityId. Это даёт O(1) Add/Remove/Get
/// и cache-friendly итерацию по всем компонентам типа <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Тип хранимого компонента.</typeparam>
internal sealed class ComponentStore<T> : IComponentStore where T : IComponent
{
    // SparseSet implementation fields
    private int[] _sparse;      // Indexed by EntityId.Id, stores position in dense array or -1 if absent.
    private T[] _dense;         // Packed array of components. Size = Count.
    private int[] _denseToIndex; // Parallel to _dense, stores EntityId.Index for each dense slot (needed for swap-on-remove).

    /// <summary>
    /// The number of components currently stored.
    /// </summary>
    public int Count { get; private set; } = 0;

    private const int DefaultCapacity = 256;

/// <summary>
/// Initializes a new instance of the ComponentStore{T} class.
/// </summary>
    internal ComponentStore()
    {
        _sparse = new int[DefaultCapacity];
        Array.Fill(_sparse, -1); // Initialize all sparse slots as absent.
        _dense = Array.Empty<T>();
        _denseToIndex = Array.Empty<int>();
        // Count is initialized to 0 via the property initializer above.
    }

    /// <summary>
    /// Ensures the underlying sparse array has enough capacity for the given EntityId index. Doubles capacity if required.
    /// </summary>
    private void EnsureCapacity(EntityId id)
    {
        if (id.Index >= _sparse.Length)
        {
            int newCapacity = System.Math.Max(_sparse.Length * 2, id.Index + 1);
            var newSparse = new int[newCapacity];

            // Copy existing values to the new array
            Array.Copy(_sparse, newSparse, _sparse.Length);
            _sparse = newSparse;
        }
    }

    /// <summary>
    /// Adds a component for the specified entity. Overwrites if an existing component is present.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="component">The component data to store.</param>
    public void Add(EntityId id, T component)
    {
        if (!id.IsValid)
        {
            throw new ArgumentException("Cannot add a component with an invalid EntityId.", nameof(id));
        }

        EnsureCapacity(id);

        int denseIndex = _sparse[id.Index];

        // Case 1: Component exists (Overwrite)
        if (denseIndex != -1)
        {
            // Overwrite the component at the existing dense index
            _dense[denseIndex] = component;
            return;
        }

        // Case 2: Component is new (Add/Overwrite non-existent)

        // Check if we need to resize the dense arrays based on the current count.
        if (_dense.Length <= Count) // This check handles the initial Array.Empty<T>() case gracefully when Count increases from 0.
        {
            int requiredCapacity = System.Math.Max(_dense.Length * 2, Count + 1);
            Array.Resize(ref _dense, requiredCapacity);
            Array.Resize(ref _denseToIndex, requiredCapacity);
        }

        // The new component is placed at the current end of the dense array.
        int newDenseIndex = Count;
        
        _dense[newDenseIndex] = component;
        _denseToIndex[newDenseIndex] = id.Index;
        _sparse[id.Index] = newDenseIndex;
        Count++;
    }

    /// <summary>
    /// Removes a component from the specified entity. If present, the slot is filled by swapping with the last stored component 
    /// to maintain array density (O(1) removal). </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    public void Remove(EntityId id)
    {
        // 1. Check if the component exists
        int denseIndexToRemove = _sparse[id.Index];
        if (denseIndexToRemove == -1)
        {
            return; // No-op if absent
        }

        // If there is only one element, we just clear it and set count to zero.
        if (Count == 1 && denseIndexToRemove == 0)
        {
             _sparse[id.Index] = -1;
             Count = 0;
             return;
        }

        // Get the EntityId that currently occupies the last slot
        int lastDenseIndex = Count - 1;
        int idOfLastComponentIndex = _denseToIndex[lastDenseIndex];

        // If the component to remove IS the last element, we just clear its sparse slot.
        if (id.Index == idOfLastComponentIndex) // Simplified ID check for comparison purposes
        {
            _sparse[id.Index] = -1;
            Count--;
            return;
        }
        
        // Perform the swap: Move data from the last slot to the target slot
        _dense[denseIndexToRemove] = _dense[lastDenseIndex];
        _denseToIndex[denseIndexToRemove] = idOfLastComponentIndex;

        // Update sparse array mapping for the component that was moved (the formerly "last" component)
        _sparse[idOfLastComponentIndex] = denseIndexToRemove;

        // Clear the last slot's sparse map entry, as it now contains data from the target slot.
        _sparse[id.Index] = -1;; 
            
        Count--;
    }

    /// <summary>
    /// Determines whether or not the specified entity has a component of this type.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>True if the component is present; otherwise, false.</returns>
    public bool Has(EntityId id)
    {
        return _sparse[id.Index] != -1;
    }

    /// <summary>
    /// Gets the component for the specified entity. Throws InvalidOperationException if the component is absent.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The stored component instance.</returns>
    public T Get(EntityId id)
    {
        if (!Has(id))
        {
            throw new InvalidOperationException($"Component {typeof(T).Name} not found for entity {id}.");
        }

        int denseIndex = _sparse[id.Index];
        return _dense[denseIndex];
    }

    /// <summary>
    /// Attempts to get the component for the specified entity. The operation is safe and returns false if absent.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="component">When this method returns, contains the stored component instance, or default(T) if not found.</param>
    /// <returns>True if the component was successfully retrieved; otherwise, false.</returns>
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

    /// <summary>
    /// Returns a read-only span over all component instances currently stored in the dense array. 
    /// This provides an efficient way to iterate over all components without allocation overhead (hot path).
    /// </summary>
/// <returns>ReadOnlySpan{T} containing all active components.</returns>
    public ReadOnlySpan<T> All()
    {
        return new ReadOnlySpan<T>(_dense, 0, Count);
    }
}