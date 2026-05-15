using System;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Base class for all game systems in DualFrontier.
/// Systems contain game logic and run every tick via <see cref="DualFrontier.Core.Scheduling.ParallelSystemScheduler"/>.
///
/// K8.3+K8.4 cutover: the managed-<c>World</c> access surface (GetComponent /
/// SetComponent / Query / GetSystem) is removed. Systems read and write
/// component storage exclusively through <see cref="NativeWorld"/>'s
/// span/batch API. Cross-system communication remains via <see cref="Services"/>;
/// per-mod managed-class storage remains via <see cref="ManagedStore{T}"/>.
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
    /// Domain-bus aggregator supplied by the scheduler. Use for publishing
    /// events (<c>Services.Pawns.Publish(...)</c>) and subscribing in
    /// <see cref="OnInitialize"/>. Reads route through the active
    /// <see cref="SystemExecutionContext"/> so out-of-context calls
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

    /// <summary>
    /// Native world handle supplied by the scheduler. Sole production
    /// component-storage path post-K8.3+K8.4 cutover. Use the span/batch
    /// surface (<c>AcquireSpan&lt;T&gt;()</c>, <c>BeginBatch&lt;T&gt;()</c>,
    /// <c>HasComponent&lt;T&gt;</c>, <c>TryGetComponent&lt;T&gt;</c>) for
    /// component access, and the K8.1 primitive factories
    /// (<c>InternString</c>, <c>CreateMap</c>, <c>CreateSet</c>,
    /// <c>CreateComposite</c>) for component-owned reference fields.
    /// </summary>
    protected NativeWorld NativeWorld
    {
        get
        {
            var ctx = SystemExecutionContext.Current
                ?? throw new InvalidOperationException(
                    "SystemBase.NativeWorld accessed outside an active scheduler context.");
            return ctx.NativeWorld;
        }
    }

    /// <summary>
    /// K8.3+K8.4 — Path β bridge accessor. Returns the calling mod's
    /// <see cref="ManagedStore{T}"/> for component type <typeparamref name="T"/>,
    /// or <c>null</c> when:
    /// <list type="bullet">
    ///   <item>The active scheduler context has no <c>IManagedStorageResolver</c>
    ///         (tests, builds without mod loading).</item>
    ///   <item>The system origin is <see cref="SystemOrigin.Core"/> — no
    ///         owning mod and thus no per-mod managed store. Core systems
    ///         use NativeWorld for component storage.</item>
    ///   <item>The owning mod has not registered <typeparamref name="T"/>
    ///         via <c>IModApi.RegisterManagedComponent&lt;T&gt;</c>.</item>
    /// </list>
    /// Throws <see cref="InvalidOperationException"/> when called outside
    /// an active scheduler context (e.g. from the Godot main thread).
    /// </summary>
    /// <typeparam name="T">
    /// Class IComponent type previously registered via
    /// <c>RegisterManagedComponent&lt;T&gt;</c> with the
    /// <c>[ManagedStorage]</c> attribute.
    /// </typeparam>
    protected ManagedStore<T>? ManagedStore<T>() where T : class, IComponent
    {
        var ctx = SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "SystemBase.ManagedStore called outside an active scheduler context.");
        return ctx.ResolveManagedStore<T>();
    }
}
