using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Marker interface for type-erased <see cref="ManagedStore{T}"/> instances.
/// Allows <c>RestrictedModApi</c> to hold a <c>Dictionary&lt;Type, IManagedStore&gt;</c>
/// without open generics. Concrete operations require downcast to
/// <see cref="ManagedStore{T}"/>.
/// </summary>
public interface IManagedStore
{
    /// <summary>Number of components currently stored.</summary>
    int Count { get; }

    /// <summary>
    /// Clear all stored components. Called from
    /// <c>RestrictedModApi.ClearManagedStores</c> on
    /// <c>AssemblyLoadContext.Unload</c>.
    /// </summary>
    void Clear();
}

/// <summary>
/// Per-mod Path β component storage per K-L3.1 bridge formalization.
///
/// Implementation: BCL <see cref="Dictionary{TKey,TValue}"/> keyed by
/// <see cref="EntityId"/> (single-threaded — Path β access serialized through
/// scheduler phase ordering per the existing single-threaded contract;
/// <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>
/// is not required).
///
/// Lifetime: scoped to the owning mod's <c>RestrictedModApi</c> instance.
/// Cleared on <c>AssemblyLoadContext.Unload</c> via
/// <c>ModRegistry.RemoveMod</c> (chain to
/// <c>RestrictedModApi.ClearManagedStores</c>) per
/// MOD_OS_ARCHITECTURE §9.5.
///
/// Path β components are runtime-only (Q4.b K-L3.1 lock) — not persisted
/// by save system; reconstructed on load post-G-series.
///
/// Lives in <c>DualFrontier.Contracts.Modding</c> so mod systems — which
/// subclass <c>SystemBase</c> in Core but cannot reference Application —
/// can receive a <see cref="ManagedStore{T}"/> from
/// <c>SystemBase.ManagedStore&lt;T&gt;()</c> and call Add/TryGet/Has/Remove
/// against it directly.
/// </summary>
/// <typeparam name="T">
/// Class IComponent type. Must be annotated with
/// <see cref="ManagedStorageAttribute"/> — absence is rejected by
/// <c>RegisterManagedComponent&lt;T&gt;</c> with the
/// <c>MissingManagedStorageAttribute</c> validation error (defined in
/// <c>DualFrontier.Application.Modding</c>; cannot be linked from this XML
/// doc).
/// </typeparam>
public sealed class ManagedStore<T> : IManagedStore where T : class, IComponent
{
    private readonly Dictionary<EntityId, T> _components = new();
    private readonly string _modId;

    /// <summary>Constructs an empty store bound to a specific mod's lifetime.</summary>
    /// <param name="modId">Owning mod identifier; preserved for diagnostics.</param>
    public ManagedStore(string modId)
    {
        _modId = modId ?? throw new ArgumentNullException(nameof(modId));
    }

    /// <summary>Owning mod identifier (diagnostic-only — not used for dispatch).</summary>
    public string ModId => _modId;

    /// <summary>Adds or overwrites the component for the given entity.</summary>
    public void Add(EntityId entity, T component)
    {
        if (component is null) throw new ArgumentNullException(nameof(component));
        _components[entity] = component;
    }

    /// <summary>
    /// Attempts to retrieve the component for the given entity. Returns
    /// <c>false</c> if absent. The out-parameter is non-null on a true
    /// return.
    /// </summary>
    public bool TryGet(EntityId entity, out T? component)
        => _components.TryGetValue(entity, out component);

    /// <summary>Returns true if a component exists for the given entity.</summary>
    public bool Has(EntityId entity) => _components.ContainsKey(entity);

    /// <summary>Removes the component for the given entity. No-op if absent.</summary>
    public bool Remove(EntityId entity) => _components.Remove(entity);

    /// <summary>
    /// Enumerates entities that have a component in this store. Lazy —
    /// safe to break out of mid-iteration without materializing the full
    /// snapshot.
    /// </summary>
    public IEnumerable<EntityId> Entities => _components.Keys;

    /// <summary>Count of stored components.</summary>
    public int Count => _components.Count;

    /// <summary>
    /// Clears the store. Called from
    /// <c>RestrictedModApi.ClearManagedStores</c> on unload (MOD_OS §9.5
    /// chain step 5 — Path β state reclamation).
    /// </summary>
    public void Clear() => _components.Clear();
}
