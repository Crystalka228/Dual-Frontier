using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// API the core gives the mod. The single point through which a mod can
/// affect the game. The real implementation (<c>RestrictedModApi</c>) lives
/// in <c>DualFrontier.Application</c> and proxies calls into the core with
/// extra checks (permissions, quotas, logging).
///
/// A mod is NOT allowed to cast this interface to a concrete type — the
/// ModLoader detects the attempt and unloads the mod with an error.
/// </summary>
public interface IModApi
{
    /// <summary>
    /// Registers a new component type with the ECS. After registration, the
    /// component can be added to entities and declared in
    /// <c>[SystemAccess]</c> of the mod's systems.
    /// </summary>
    void RegisterComponent<T>() where T : IComponent;

    /// <summary>
    /// Registers a new system. The system must already be configured via
    /// <c>[SystemAccess]</c> and <c>[TickRate]</c>. The scheduler rebuilds
    /// the phase graph.
    /// </summary>
    void RegisterSystem<T>() where T : class;

    /// <summary>
    /// Publishes an event on the matching domain bus. The bus is resolved
    /// from the event type's <c>[EventBus]</c> marker attribute.
    /// </summary>
    void Publish<T>(T evt) where T : IEvent;

    /// <summary>
    /// Subscribes to events of type T. ModApi tracks every mod subscription
    /// and removes it on Unload via <c>RestrictedModApi.UnsubscribeAll</c>.
    /// </summary>
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    /// <summary>
    /// Publishes a contract for other mods. Other mods fetch it through
    /// <see cref="TryGetContract{T}"/>. This is the only channel for
    /// mod-to-mod communication.
    /// </summary>
    void PublishContract<T>(T contract) where T : IModContract;

    /// <summary>
    /// Attempts to fetch another mod's contract. Returns <c>true</c> if the
    /// contract is registered. If the mod that publishes the contract is
    /// not loaded — returns <c>false</c>: the dependent mod must gracefully
    /// degrade.
    /// </summary>
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;

    /// <summary>
    /// Returns the set of capability strings the current kernel build
    /// provides to mods. Used by a mod to inspect what it may declare in
    /// its manifest. Returns an empty set in M2; populated in M3.
    /// </summary>
    IReadOnlySet<string> GetKernelCapabilities();

    /// <summary>
    /// Returns the manifest of the mod making this call.
    /// </summary>
    ModManifest GetOwnManifest();

    /// <summary>
    /// Logs a structured message prefixed with the mod's id.
    /// </summary>
    void Log(ModLogLevel level, string message);

    /// <summary>
    /// Field-storage sub-API per MOD_OS_ARCHITECTURE.md v1.7 §4.6.
    /// Returns null on builds without K9 field storage support; mods check
    /// for null and degrade gracefully.
    /// </summary>
    IModFieldApi? Fields { get; }

    /// <summary>
    /// Compute-pipeline sub-API per MOD_OS_ARCHITECTURE.md v1.7 §4.6.
    /// Returns null on K9 (lands at G0). Mods check for null.
    /// </summary>
    IModComputePipelineApi? ComputePipelines { get; }
}
