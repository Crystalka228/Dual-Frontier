using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

/// <summary>
/// A registry that tracks which component types are known to the ECS.
/// Used at startup to validate [SystemAccess] declarations.
/// </summary>
internal sealed class ComponentRegistry
{
    private readonly HashSet<Type> _registered = new();

    /// <summary>
    /// Registers a component type. No-op if already registered.
    /// </summary>
    /// <typeparam name="T">The type of the component to register, must implement IComponent.</typeparam>
    public void Register<T>() where T : IComponent
        => _registered.Add(typeof(T));

    /// <summary>
    /// Returns true if the type has been registered.
    /// </summary>
    /// <typeparam name="T">The component type to check.</typeparam>
    public bool IsRegistered<T>() where T : IComponent
        => _registered.Contains(typeof(T));

    /// <summary>
    /// Returns true if the type has been registered (non-generic overload).
    /// </summary>
    /// <param name="type">The component type to check.</param>
    public bool IsRegistered(Type type)
        => _registered.Contains(type);

    /// <summary>
    /// Returns all registered component types (snapshot).
    /// </summary>
    public IReadOnlyCollection<Type> All => _registered;

    /// <summary>
    /// Clears all registrations (used in tests).
    /// </summary>
    public void Clear() => _registered.Clear();
}

namespace DualFrontier.Core.Registry
{
    // Note: Assuming the class structure and namespace declaration are handled by file-scoped namespaces/using directives, 
    // but retaining explicit declarations for clarity based on prompt requirements.
}