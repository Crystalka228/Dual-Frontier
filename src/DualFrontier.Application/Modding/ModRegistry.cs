using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Contracts.Sdk;
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

    // K10.2 Item 21 — per-mod sub-scheduler instance ownership (К-L12 separation).
    // Each mod ALC owns a ModSubScheduler that tracks its registered systems.
    // Teardown consumed at Step 3.5 native primitive consumption pattern
    // (per S3-Q1 L3 layering).
    private readonly Dictionary<string, ModSubScheduler> _modSubSchedulers = new();

    // W1 BD-2 — the unified factory path resolves construction-time services
    // (day-one: pathfinding); the SimTick source is stamped onto SDK adapters so
    // an ISimulationSystem can read ISystemContext.CurrentTick. The tick source
    // defaults to zero until SetTickSource (tests, core-only builds).
    private ISystemServices? _systemServices;
    private Func<long> _tickSource = static () => 0L;

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
    /// W1 BD-2 — supplies the construction-time service surface the factory
    /// registration path passes to system factories. Set once at bootstrap.
    /// </summary>
    internal void SetSystemServices(ISystemServices services)
        => _systemServices = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// W1 BD-1 — supplies the SimTick accessor stamped onto SDK adapters so an
    /// <c>ISimulationSystem</c> reads <c>ISystemContext.CurrentTick</c>.
    /// </summary>
    internal void SetTickSource(Func<long> tickSource)
        => _tickSource = tickSource ?? throw new ArgumentNullException(nameof(tickSource));

    /// <summary>
    /// W1 BD-2 — the unified registration path. Constructs a CORE system through
    /// <paramref name="factory"/> (resolving construction-time services), so core
    /// and mod construction share ONE mechanism and the hand-instantiation
    /// bifurcation dies. The parameterless overload is the convenience form.
    /// </summary>
    public void RegisterSystem<T>(Func<ISystemServices, T> factory) where T : class
    {
        if (factory is null) throw new ArgumentNullException(nameof(factory));
        T instance = factory(RequireSystemServices());
        SystemBase asBase = instance as SystemBase
            ?? throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] The core factory produced '{instance.GetType().FullName}', " +
                "which is not a SystemBase. Core ISimulationSystem registration is a later-wave capability.");
        _coreSystems.Add(new SystemRegistration(asBase, SystemOrigin.Core, ModId: null));
    }

    /// <summary>Parameterless convenience overload of the factory path.</summary>
    public void RegisterSystem<T>() where T : class, new()
        => RegisterSystem<T>(static _ => new T());

    /// <summary>
    /// Returns the core system instances in registration order — for the bootstrap
    /// to feed the dependency graph and native scheduler interop after factory
    /// registration.
    /// </summary>
    public IReadOnlyList<SystemBase> GetCoreSystemInstances()
    {
        var list = new List<SystemBase>(_coreSystems.Count);
        foreach (SystemRegistration reg in _coreSystems)
            list.Add(reg.Instance);
        return list;
    }

    private ISystemServices RequireSystemServices()
        => _systemServices ?? throw new InvalidOperationException(
            "RegisterSystem<T>(factory) requires ISystemServices — call SetSystemServices first.");

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

        // W1 BD-1 — accept the SDK contract (ISimulationSystem) alongside the
        // SystemBase bridge. SystemBase authoring is the transitional path,
        // retiring at W5; ISimulationSystem is the durable SDK surface.
        bool isSystemBase = typeof(SystemBase).IsAssignableFrom(systemType);
        bool isContract = typeof(ISimulationSystem).IsAssignableFrom(systemType);
        if (!isSystemBase && !isContract)
        {
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] Type '{systemType.FullName}' " +
                "does not derive from SystemBase and does not implement ISimulationSystem. " +
                "Use 'public sealed class MySystem : ISimulationSystem' for SDK mod systems.");
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

        SystemBase instance = isSystemBase
            ? CreateSystemInstance(systemType)
            : CreateContractAdapter(modId, systemType);
        _modSystems.Add(new SystemRegistration(instance, SystemOrigin.Mod, modId));
        // W1-fix (Codex review) — track the (possibly adapter-wrapped) system in the mod's
        // sub-scheduler so the unload chain's RemoveSubScheduler.Teardown disposes it, firing
        // SystemBase.Dispose -> OnDispose (and, for an adapter, ISimulationSystem.OnDispose).
        // Previously the sub-scheduler was populated only in tests, so no mod system's dispose
        // hook fired on unload.
        GetOrCreateSubScheduler(modId).AddSystem(instance);
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

    // ===== K10.2 Item 21 — per-mod sub-scheduler ownership =====

    /// <summary>
    /// K10.2 Item 21 — Returns the existing <see cref="ModSubScheduler"/> for
    /// <paramref name="modId"/>, creating one if absent. К-L9 «Vanilla = mods»:
    /// vanilla mods are treated identically to third-party.
    /// </summary>
    public ModSubScheduler GetOrCreateSubScheduler(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (!_modSubSchedulers.TryGetValue(modId, out ModSubScheduler? sub))
        {
            sub = new ModSubScheduler(modId);
            _modSubSchedulers[modId] = sub;
        }
        return sub;
    }

    /// <summary>
    /// K10.2 Item 21 — Returns the sub-scheduler for <paramref name="modId"/>
    /// или <see langword="null"/> if none has been created.
    /// </summary>
    public ModSubScheduler? TryGetSubScheduler(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        return _modSubSchedulers.TryGetValue(modId, out ModSubScheduler? sub) ? sub : null;
    }

    /// <summary>
    /// K10.2 Item 21 — Removes the sub-scheduler for <paramref name="modId"/>
    /// and invokes <see cref="ModSubScheduler.Teardown"/>. Consumed by
    /// ModIntegrationPipeline Step 3.5 (Commit 11) after the native unload
    /// primitive returns (per S3-Q1 L3 layering: native primitive handles
    /// native state в parallel; this disposes managed-side mod-system instances).
    /// </summary>
    public bool RemoveSubScheduler(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (_modSubSchedulers.TryGetValue(modId, out ModSubScheduler? sub))
        {
            sub.Teardown();
            _modSubSchedulers.Remove(modId);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public ManagedStore<T>? Resolve<T>(string modId) where T : class, IComponent
    {
        if (modId is null) return null;
        return _restrictedModApis.TryGetValue(modId, out RestrictedModApi? api)
            ? api.GetManagedStore<T>()
            : null;
    }

    /// <summary>
    /// W1 — the owning mod's <see cref="RestrictedModApi"/> for a mod id, used by
    /// <see cref="SystemContextView"/> to route an SDK system's gated
    /// Publish/Subscribe through the existing capability gate. Null if unknown.
    /// </summary>
    internal RestrictedModApi? GetModApi(string modId)
        => modId is not null && _restrictedModApis.TryGetValue(modId, out RestrictedModApi? api)
            ? api
            : null;

    /// <summary>
    /// W1 BD-1 — constructs an SDK <see cref="ISimulationSystem"/> (parameterless)
    /// and wraps it in a distinct <c>SystemAdapter&lt;systemType&gt;</c> so the
    /// executor's type-keyed logic stays correct. Reflection is acceptable here —
    /// registration is cold (menu-thread, simulation stopped).
    /// </summary>
    private SystemBase CreateContractAdapter(string modId, Type systemType)
    {
        object? sim;
        try
        {
            sim = Activator.CreateInstance(systemType);
        }
        catch (MissingMethodException ex)
        {
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] System '{systemType.FullName}' " +
                "requires a public parameterless constructor.",
                ex);
        }

        if (sim is not ISimulationSystem)
        {
            throw new InvalidOperationException(
                $"[MOD REGISTRY ERROR] Activator.CreateInstance('{systemType.FullName}') " +
                "returned null or a non-ISimulationSystem instance.");
        }

        Type adapterType = typeof(SystemAdapter<>).MakeGenericType(systemType);
        return (SystemBase)Activator.CreateInstance(adapterType, sim, this, modId, _tickSource)!;
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
