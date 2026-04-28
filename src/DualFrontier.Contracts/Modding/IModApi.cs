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
    /// TODO: Phase 2 — registers a new component type with the ECS.
    /// After registration, the component can be added to entities and
    /// declared in <c>[SystemAccess]</c> of the mod's systems.
    /// </summary>
    void RegisterComponent<T>() where T : IComponent;

    /// <summary>
    /// TODO: Phase 2 — registers a new system.
    /// The system must already be configured via <c>[SystemAccess]</c> and
    /// <c>[TickRate]</c>. The scheduler rebuilds the phase graph.
    /// </summary>
    void RegisterSystem<T>() where T : class;

    /// <summary>
    /// TODO: Phase 2 — publishes an event on the matching domain bus.
    /// ModApi resolves the bus from the event type (via markers/attributes).
    /// </summary>
    void Publish<T>(T evt) where T : IEvent;

    /// <summary>
    /// TODO: Phase 2 — subscribes to events of type T.
    /// ModApi tracks every mod subscription and removes it on Unload.
    /// </summary>
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    /// <summary>
    /// TODO: Phase 2 — publishes a contract for other mods.
    /// Other mods can fetch it through <see cref="TryGetContract{T}"/>.
    /// This is the only channel for mod-to-mod communication.
    /// </summary>
    void PublishContract<T>(T contract) where T : IModContract;

    /// <summary>
    /// TODO: Phase 2 — attempts to fetch another mod's contract.
    /// Returns <c>true</c> if the contract is registered. If the mod that
    /// publishes the contract is not loaded — returns <c>false</c>: the
    /// dependent mod must gracefully degrade.
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
}
