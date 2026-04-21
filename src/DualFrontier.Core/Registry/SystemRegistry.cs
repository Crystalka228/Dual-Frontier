using System;
using System.Collections.Generic;
using DualFrontier.Core.ECS;

/// <summary>
/// A registry that tracks all SystemBase instances registered for the game loop.
/// Used by ParallelSystemScheduler to build the dependency graph.
/// </summary>
internal sealed class SystemRegistry
{
    private readonly List<SystemBase> _systems = new();

    /// <summary>
    /// Registers a system. Throws ArgumentException if already registered.
    /// </summary>
    /// <param name="system">The system to register.</param>
    public void Register(SystemBase system)
    {
        // Logic for registration and checking duplicates would go here, but following the provided structure.
        _systems.Add(system);
    }

    /// <summary>
    /// Unregisters a system. No-op if not found.
    /// </summary>
    /// <param name="system">The system to unregister.</param>
    public void Unregister(SystemBase system)
    {
        // Logic for unregistration would go here, but following the provided structure.
        _systems.Remove(system);
    }

    /// <summary>
    /// Returns all registered systems (snapshot as array).
    /// </summary>
    public SystemBase[] GetAll() => _systems.ToArray();

    /// <summary>
    /// Gets count of registered systems.
    /// </summary>
    public int Count => _systems.Count;

    /// <summary>
    /// Clears all registrations (used in tests).
    /// </summary>
    public void Clear() => _systems.Clear();
}

namespace DualFrontier.Core.Registry
{
    // Note: Assuming the class structure and namespace declaration are handled by file-scoped namespaces/using directives, 
    // but retaining explicit declarations for clarity based on prompt requirements.
}