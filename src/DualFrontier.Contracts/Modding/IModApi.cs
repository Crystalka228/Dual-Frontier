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
///
/// K8.3+K8.4 combined milestone (2026-05-14) — IModApi v3 surface. The K-L3
/// «Path α default + Path β bridge» split is now expressed via the constraint
/// system: <see cref="RegisterComponent{T}"/> requires
/// <c>where T : unmanaged, IComponent</c> (Path α — NativeWorld-backed
/// struct storage, K-L3 default); <see cref="RegisterManagedComponent{T}"/>
/// requires <c>where T : class, IComponent</c> and the type to be annotated
/// with <c>[ManagedStorage]</c> (Path β — per-mod managed-class storage,
/// K-L3.1 bridge). v2 IModApi deleted entirely — no backward compatibility.
/// Mods that registered class-shape components without <c>RegisterManagedComponent</c>
/// fail to compile post-K8.3+K8.4; the manifest parser also rejects any
/// <c>manifestVersion</c> other than <c>"3"</c>.
/// </summary>
public interface IModApi
{
    /// <summary>
    /// Registers a new Path α component type (NativeWorld-backed struct
    /// storage, K-L3 default). After registration, the component can be
    /// added to entities and declared in <c>[SystemAccess]</c> of the mod's
    /// systems. Type <typeparamref name="T"/> must be <c>unmanaged</c> —
    /// fixed-size value type with no references — to cross the P/Invoke
    /// boundary without GCHandle pinning.
    /// </summary>
    void RegisterComponent<T>() where T : unmanaged, IComponent;

    /// <summary>
    /// Registers a Path β managed-class component type (per-mod managed
    /// storage, K-L3.1 bridge). Type <typeparamref name="T"/> must be a
    /// class implementing <see cref="IComponent"/>, and must be annotated
    /// with <c>[ManagedStorage]</c> — absence raises
    /// <c>ValidationErrorKind.MissingManagedStorageAttribute</c>.
    ///
    /// Storage lives in the per-mod <c>RestrictedModApi.ManagedStore&lt;T&gt;</c>
    /// (BCL <c>Dictionary&lt;EntityId, T&gt;</c>); reclaimed deterministically
    /// on <c>AssemblyLoadContext.Unload</c> per MOD_OS_ARCHITECTURE §9.5
    /// unload chain.
    ///
    /// Path β components are runtime-only (Q4.b K-L3.1 lock) — not persisted
    /// by save system; reconstructed on load post-G-series.
    ///
    /// Mods that need Path β registration must declare
    /// <c>manifestVersion: "3"</c> — the manifest parser rejects non-v3
    /// manifests at load time.
    /// </summary>
    void RegisterManagedComponent<T>() where T : class, IComponent;

    /// <summary>
    /// Registers a new system. The system must already be configured via
    /// <c>[SystemAccess]</c> and <c>[TickRate]</c>. The scheduler rebuilds
    /// the phase graph.
    /// </summary>
    void RegisterSystem<T>() where T : class;

    /// <summary>
    /// Registers a simulation system the engine constructs through <paramref name="factory"/>,
    /// handing it the construction-time service surface (<see cref="Sdk.ISystemServices"/>).
    ///
    /// <para>
    /// Use this whenever the system needs a service at construction. The parameterless overload
    /// builds the type with <c>Activator.CreateInstance</c> and therefore requires a public
    /// parameterless constructor; a system taking, say, a pathfinding service cannot be
    /// registered through it at all, and because a throw out of <see cref="IMod.Initialize"/>
    /// rolls back the whole load batch, one such system takes every other system in the mod
    /// down with it.
    /// </para>
    ///
    /// <para>
    /// The factory runs at registration time, on the menu thread with the simulation stopped.
    /// It must not touch the world: <see cref="Sdk.ISystemServices"/> carries services only, and
    /// world access arrives later through <c>ISystemContext</c>.
    /// </para>
    /// </summary>
    void RegisterSystem<T>(Func<Sdk.ISystemServices, T> factory) where T : class;

    /// <summary>
    /// Publishes an event on the single managed event dispatch. W2/BD-3 removed the
    /// genre taxonomy, so events route by type -- no <c>[EventBus]</c> marker or bus
    /// resolution. Gated first by the mod's declared capabilities (kernel.publish:&lt;FQN&gt;).
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
    /// The starting-state description the host's distribution declares, or <c>null</c> when the
    /// host has none.
    ///
    /// <para>
    /// Null is the ordinary case for a test harness or a tool that stands the pipeline up without
    /// a distribution, so a mod that wants it must check. A mod whose whole purpose is to seed a
    /// scenario should refuse loudly on null rather than substituting numbers of its own: a
    /// fabricated default would look like a working colony while silently ignoring what the
    /// distribution asked for.
    /// </para>
    /// </summary>
    Distribution.ScenarioConfig? Scenario { get; }

    /// <summary>
    /// Logs a structured message prefixed with the mod's id.
    /// </summary>
    void Log(ModLogLevel level, string message);

    /// <summary>
    /// Field-storage sub-API per MOD_OS_ARCHITECTURE §4.6 (Fields and
    /// Compute Pipelines). Returns null on builds without K9 field storage
    /// support; mods check for null and degrade gracefully.
    /// </summary>
    IModFieldApi? Fields { get; }

    /// <summary>
    /// Compute-pipeline sub-API per MOD_OS_ARCHITECTURE §4.6 (Fields and
    /// Compute Pipelines). Returns null — the mod-facing compute surface is
    /// unwired (the former G0 milestone was superseded by the V substrate
    /// per Q-G-2; V0/V1 shipped kernel-side). Planned — see docs/ROADMAP.md
    /// §V substrate (M-V demonstrations). Mods check for null.
    /// </summary>
    IModComputePipelineApi? ComputePipelines { get; }
}
