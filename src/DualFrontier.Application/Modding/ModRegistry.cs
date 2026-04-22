using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
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
/// Not thread-safe: the registry is only mutated from the menu thread when
/// the simulation is stopped, never from runtime.
/// </summary>
internal sealed class ModRegistry
{
    private readonly Dictionary<Type, string> _componentOwners = new();
    private readonly List<SystemRegistration> _coreSystems = new();
    private readonly List<SystemRegistration> _modSystems = new();

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
            // Каждая Core-система регистрируется как происходящая из ядра,
            // modId не имеет смысла для Core — поэтому null.
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
            // Диагностика называет оба мода — автор нового мода сразу видит, с кем конфликт.
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
    /// </summary>
    /// <param name="modId">Identifier of the mod to clear.</param>
    public void RemoveMod(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));

        // Обратный проход — индексы не сдвигаются при удалении.
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
