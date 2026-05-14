using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Registry of all components and systems contributed by mods. Acts as the
/// backing storage for <see cref="RestrictedModApi"/>: when a mod calls
/// <c>RegisterComponent</c> or <c>RegisterSystem</c> through the API, the
/// call lands here.
///
/// Core systems live in a separate, immutable list set once at start-up via
/// <see cref="SetCoreSystems"/>. <see cref="GetAllSystems"/> returns core
/// first and then mod systems in registration order so the dependency graph
/// builds deterministically.
///
/// K8.3+K8.4 combined milestone (2026-05-14) — implements
/// <see cref="IManagedStorageResolver"/> so <c>SystemBase.ManagedStore&lt;T&gt;()</c>
/// can resolve a Path β <see cref="ManagedStore{T}"/> from the calling
/// system's owning mod. <see cref="RegisterRestrictedModApi"/> /
/// <see cref="UnregisterRestrictedModApi"/> maintain the mod-id → API map
/// the resolver dispatches against.
///
/// Not thread-safe: the registry is only mutated from the menu thread when
/// the simulation is stopped, never from runtime.
/// </summary>
internal sealed class ModRegistry : IManagedStorageResolver
{
    private readonly Dictionary<Type, string> _componentOwners = new();
    private readonly List<SystemRegistration> _coreSystems = new();
    private readonly List<SystemRegistration> _modSystems = new();

    // K8.3+K8.4 — mod-id → RestrictedModApi mapping for IManagedStorageResolver
    // dispatch. ModLoader/pipeline registers each mod's API instance after
    // construction; UnregisterRestrictedModApi clears at unload. Resolver
    // returns null for unknown ids (e.g. Core-origin systems whose modId is
    // null — handled in SystemExecutionContext before the call reaches here).
    private readonly Dictionary<string, RestrictedModApi> _restrictedModApis = new();

    /// <summary>
    /// Sets the list of core systems once at start-up. Subsequent calls
    /// overwrite the core list but leave mod systems untouched — this
    /// supports test setups that rewire core between pipelines.
    /// </summary>
    /// <param name="coreSystems">Core system instances in their desired order.</param>
    public void SetCoreSystems(IReadOnlyList<SystemBase> coreSystems)
    {
        if (coreSystems is null)
            throw new ArgumentNullException(nameof(coreSystems));

        _coreSystems.Clear();
        foreach (SystemBase system in coreSystems)
        {
            // Every core system is registered as core-origin; modId is
            // meaningless for core, hence null.
            _coreSystems.Add(new SystemRegistration(system, SystemOrigin.Core, ModId: null));
        }
    }

    /// <summary>
    /// Registers a component type as belonging to the given mod. Throws
    /// <see cref="InvalidOperationException"/> when the component is already
    /// owned by another mod, naming both parties.
    /// </summary>
    /// <param name="modId">Identifier of the owning mod.</param>
    /// <param name="componentType">Concrete component type to register.</param>
    public void RegisterComponent(string modId, Type componentType)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (componentType is null) throw new ArgumentNullException(nameof(componentType));

        if (_componentOwners.TryGetValue(componentType, out string? existing))
        {
            // The diagnostic names both mods — the new mod's author can see at a glance who they conflict with.
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] Component '{componentType.FullName}' " +
                $"is already registered by mod '{existing}'. Mod '{modId}' cannot claim it.");
        }

        _componentOwners[componentType] = modId;
    }

    /// <summary>
    /// Registers a system type as belonging to the given mod. The type must
    /// carry both <see cref="SystemAccessAttribute"/> and
    /// <see cref="TickRateAttribute"/>; otherwise an
    /// <see cref="InvalidOperationException"/> with a remediation hint is
    /// thrown and the mod is considered to have failed initialization.
    /// </summary>
    /// <param name="modId">Identifier of the owning mod.</param>
    /// <param name="systemType">Concrete system type to register.</param>
    public void RegisterSystem(string modId, Type systemType)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (systemType is null) throw new ArgumentNullException(nameof(systemType));

        if (!typeof(SystemBase).IsAssignableFrom(systemType))
        {
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] Type '{systemType.FullName}' " +
                "does not derive from SystemBase. " +
                "Use 'public sealed class MySystem : SystemBase' for mod systems.");
        }

        SystemAccessAttribute? access =
            systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false);
        if (access is null)
        {
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] System '{systemType.FullName}' " +
                "has no [SystemAccess] attribute. " +
                "Add: [SystemAccess(reads: new[]{typeof(...)}, writes: new[]{typeof(...)}, bus: nameof(IGameServices.X))]");
        }

        TickRateAttribute? tickRate =
            systemType.GetCustomAttribute<TickRateAttribute>(inherit: false);
        if (tickRate is null)
        {
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] System '{systemType.FullName}' " +
                "has no [TickRate] attribute. " +
                "Add: [TickRate(TickRates.NORMAL)] or another TickRates.* constant.");
        }

        SystemBase instance = CreateSystemInstance(systemType);
        _modSystems.Add(new SystemRegistration(instance, SystemOrigin.Mod, modId));
    }

    /// <summary>
    /// Returns all registered systems: core systems first, then mod systems
    /// in registration order. The returned list is a snapshot — mutating the
    /// registry after the call does not affect it.
    /// </summary>
    public IReadOnlyList<SystemRegistration> GetAllSystems()
    {
        var result = new List<SystemRegistration>(_coreSystems.Count + _modSystems.Count);
        foreach (SystemRegistration reg in _coreSystems)
            result.Add(reg);
        foreach (SystemRegistration reg in _modSystems)
            result.Add(reg);
        return result;
    }

    /// <summary>
    /// Clears mod systems and mod-owned components, preserving core
    /// registrations. Called when the pipeline unloads all mods or rolls
    /// back a failed apply.
    /// </summary>
    public void ResetModSystems()
    {
        _modSystems.Clear();
        _componentOwners.Clear();
    }

    /// <summary>
    /// Removes mod systems and components that belong to the given mod id.
    /// Used when a single mod is unloaded while others remain active.
    /// K8.3+K8.4 — also clears the mod's Path β ManagedStore&lt;T&gt; instances
    /// (MOD_OS §9.5 chain step 5) and drops the resolver-dispatch entry.
    /// </summary>
    /// <param name="modId">Identifier of the mod to clear.</param>
    public void RemoveMod(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));

        // Path β cleanup before system/component removal so mod systems
        // can't observe inconsistent state mid-unload.
        if (_restrictedModApis.TryGetValue(modId, out RestrictedModApi? api))
        {
            api.ClearManagedStores();
            _restrictedModApis.Remove(modId);
        }

        // Reverse pass — indices do not shift on removal.
        for (int i = _modSystems.Count - 1; i >= 0; i--)
        {
            if (_modSystems[i].ModId == modId)
                _modSystems.RemoveAt(i);
        }

        var toRemove = new List<Type>();
        foreach (KeyValuePair<Type, string> pair in _componentOwners)
        {
            if (pair.Value == modId)
                toRemove.Add(pair.Key);
        }
        foreach (Type t in toRemove)
            _componentOwners.Remove(t);
    }

    /// <summary>
    /// Registers a mod's <see cref="RestrictedModApi"/> instance so the
    /// <see cref="IManagedStorageResolver"/> implementation can dispatch
    /// <see cref="ManagedStore{T}"/> queries to it. Called by the pipeline
    /// after constructing the API for a mod (K8.3+K8.4).
    /// </summary>
    internal void RegisterRestrictedModApi(string modId, RestrictedModApi api)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (api is null) throw new ArgumentNullException(nameof(api));
        _restrictedModApis[modId] = api;
    }

    /// <summary>
    /// Removes a mod's <see cref="RestrictedModApi"/> from the resolver
    /// dispatch table. <see cref="RemoveMod"/> calls this internally; the
    /// method is exposed separately for tests that exercise the resolver in
    /// isolation.
    /// </summary>
    internal void UnregisterRestrictedModApi(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        _restrictedModApis.Remove(modId);
    }

    /// <inheritdoc />
    public ManagedStore<T>? Resolve<T>(string modId) where T : class, IComponent
    {
        if (modId is null) return null;
        return _restrictedModApis.TryGetValue(modId, out RestrictedModApi? api)
            ? api.GetManagedStore<T>()
            : null;
    }

    private static SystemBase CreateSystemInstance(Type systemType)
    {
        try
        {
            object? instance = Activator.CreateInstance(systemType);
            if (instance is SystemBase system)
                return system;

            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] Activator.CreateInstance('{systemType.FullName}') " +
                "returned null or a non-SystemBase instance.");
        }
        catch (MissingMethodException ex)
        {
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] System '{systemType.FullName}' " +
                "requires a public parameterless constructor.",
                ex);
        }
    }
}
