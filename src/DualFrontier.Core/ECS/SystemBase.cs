using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Base class for all game systems in DualFrontier.
/// Systems contain game logic and run every tick via ParallelSystemScheduler.
/// </summary>
public abstract class SystemBase
{
    /// <summary>
    /// Gets the execution context, which provides system access to component data and event buses.
    /// This property is set by ParallelSystemScheduler after construction.
    /// Internal setter — only scheduler assigns this.
    /// </summary>
    public SystemExecutionContext Context { get; internal set; } = null!;

    /// <summary>
    /// Called once when system is registered. Use for bus subscriptions or initial setup logic.
    /// Default implementation is empty and can be overridden by derived classes when needed.
    /// </summary>
    protected virtual void OnInitialize() { }

    /// <summary>
    /// The main game logic update loop, called by the scheduler every tick according to [TickRate].
    /// All system-specific game logic must be implemented here.
    /// </summary>
    public abstract void Update(float delta);

    /// <summary>
    /// Called once when system is unregistered or game shuts down.
    /// Use this for cleanup, unsubscriptions, and resource release.
    /// Default implementation is empty and can be overridden by derived classes when needed.
    /// </summary>
    protected virtual void OnDispose() { }

    /// <summary>
    /// Internal method called by the scheduler to initialize the system's lifecycle hooks.
    /// </summary>
    internal void Initialize() => OnInitialize();

    /// <summary>
    /// Internal method called by the scheduler to dispose of the system's resources and logic.
    /// </summary>
    internal void Dispose() => OnDispose();

    /// <summary>
    /// Reads a component of type {T} from entity <paramref name="id"/> via the
    /// thread-local execution context. Delegates to
    /// <see cref="SystemExecutionContext.GetComponent{T}"/>, which enforces
    /// the isolation guard in DEBUG. Throws {InvalidOperationException} when
    /// called outside a scheduler-pushed context (e.g. from the Godot main
    /// thread).
    /// </summary>
    /// <typeparam name="T">Component type to read.</typeparam>
    /// <param name="id">Target entity.</param>
    /// <returns>The stored component instance.</returns>
    protected T GetComponent<T>(EntityId id) where T : IComponent
    {
        var ctx = SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "SystemBase.GetComponent called outside an active scheduler context.");
        return ctx.GetComponent<T>(id);
    }

    /// <summary>
    /// Writes a component of type {T} on entity <paramref name="id"/> via the
    /// thread-local execution context. Delegates to
    /// <see cref="SystemExecutionContext.SetComponent{T}"/>, which enforces
    /// the isolation guard in DEBUG. Throws {InvalidOperationException} when
    /// called outside a scheduler-pushed context.
    /// </summary>
    /// <typeparam name="T">Component type to write.</typeparam>
    /// <param name="id">Target entity.</param>
    /// <param name="value">New component value.</param>
    protected void SetComponent<T>(EntityId id, T value) where T : IComponent
    {
        var ctx = SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "SystemBase.SetComponent called outside an active scheduler context.");
        ctx.SetComponent<T>(id, value);
    }

    /// <summary>
    /// Enumerates entities that currently have a component of type {T}.
    /// Delegates to <see cref="SystemExecutionContext.Query{T}"/> — lazy,
    /// DEBUG-guarded. Throws {InvalidOperationException} when called outside
    /// an active scheduler context.
    /// </summary>
    /// <typeparam name="T">Component type whose owners to enumerate.</typeparam>
    /// <returns>Lazy sequence of entity ids carrying a component of type {T}.</returns>
    protected IEnumerable<EntityId> Query<T>() where T : IComponent
    {
        var ctx = SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "SystemBase.Query called outside an active scheduler context.");
        return ctx.Query<T>();
    }

    /// <summary>
    /// Enumerates entities that currently have both {T1} and {T2}
    /// components. Delegates to
    /// <see cref="SystemExecutionContext.Query{T1, T2}"/>. Throws
    /// {InvalidOperationException} when called outside an active scheduler
    /// context.
    /// </summary>
    /// <typeparam name="T1">First required component type.</typeparam>
    /// <typeparam name="T2">Second required component type.</typeparam>
    /// <returns>Lazy sequence of entity ids carrying both components.</returns>
    protected IEnumerable<EntityId> Query<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        var ctx = SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "SystemBase.Query called outside an active scheduler context.");
        return ctx.Query<T1, T2>();
    }

    /// <summary>
    /// Always throws <see cref="IsolationViolationException"/> via the
    /// thread-local execution context. Retained so that attempts to reach
    /// another system produce a clear, diagnosable error instead of a silent
    /// compile. Use {EventBus} for cross-system communication.
    /// </summary>
    /// <typeparam name="TSystem">System type the caller tried to resolve.</typeparam>
    /// <returns>Never returns — always throws.</returns>
    protected TSystem GetSystem<TSystem>() where TSystem : SystemBase
    {
        var ctx = SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "SystemBase.GetSystem called outside an active scheduler context.");
        return ctx.GetSystem<TSystem>();
    }

    /// <summary>
    /// Domain-bus aggregator supplied by the scheduler. Use for publishing
    /// events (<c>Services.Pawns.Publish(...)</c>) and subscribing in
    /// <see cref="OnInitialize"/>. Reads are routed through the active
    /// <see cref="SystemExecutionContext"/> so that out-of-context calls
    /// (e.g. from the Godot main thread) fail loudly, and tests that build
    /// a context without supplying services receive a diagnostic instead of
    /// a silent NullReferenceException.
    /// </summary>
    protected IGameServices Services
    {
        get
        {
            var ctx = SystemExecutionContext.Current
                ?? throw new InvalidOperationException(
                    "SystemBase.Services accessed outside an active scheduler context.");
            return ctx.Services
                ?? throw new InvalidOperationException(
                    "SystemBase.Services requested but the scheduler did not supply an IGameServices instance. " +
                    "Pass one to ParallelSystemScheduler's constructor.");
        }
    }
}
