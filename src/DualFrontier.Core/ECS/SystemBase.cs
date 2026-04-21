using System;
using DualFrontier.Contracts.Attributes;

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
}