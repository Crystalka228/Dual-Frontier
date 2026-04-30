using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Single, process-wide <see cref="AssemblyLoadContext"/> that owns every
/// shared-mod assembly. The shared ALC is the solution to the cross-ALC type
/// identity problem (MOD_OS_ARCHITECTURE §5): when several regular mods
/// reference the same shared type, all of their isolated
/// <see cref="ModLoadContext"/> instances must resolve that type to the same
/// <c>Type</c> instance. They achieve this by delegating to this context for
/// any assembly name it has cached.
///
/// Lifecycle invariant (§1.4): <c>IsCollectible = false</c> — the shared ALC
/// is loaded once at game start and never unloaded during the session.
/// </summary>
internal sealed class SharedModLoadContext : AssemblyLoadContext
{
    private readonly Dictionary<string, Assembly> _assemblies = new(StringComparer.Ordinal);

    /// <summary>
    /// Creates the shared context. The base name <c>"shared"</c> matches the
    /// canonical name used in MOD_OS_ARCHITECTURE §5.1; <c>isCollectible</c>
    /// is fixed to <see langword="false"/> because shared assemblies must
    /// never unload mid-session.
    /// </summary>
    public SharedModLoadContext()
        : base("shared", isCollectible: false)
    {
    }

    /// <summary>
    /// Loads a shared mod assembly from the given absolute path into this
    /// context and indexes it by simple name. Subsequent
    /// <see cref="TryGetCachedAssembly"/> calls return the cached instance,
    /// which is how regular <see cref="ModLoadContext"/>s preserve type
    /// identity across mods.
    /// </summary>
    /// <param name="assemblyPath">Absolute path to a shared mod assembly.</param>
    /// <returns>The loaded assembly.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="assemblyPath"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// When an assembly with the same simple name has already been loaded
    /// into the shared ALC.
    /// </exception>
    public Assembly LoadSharedAssembly(string assemblyPath)
    {
        if (assemblyPath is null) throw new ArgumentNullException(nameof(assemblyPath));

        Assembly asm = LoadFromAssemblyPath(assemblyPath);
        string simpleName = asm.GetName().Name
            ?? throw new InvalidOperationException(
                $"Assembly at '{assemblyPath}' has no simple name.");

        if (_assemblies.ContainsKey(simpleName))
            throw new InvalidOperationException(
                $"Shared assembly '{simpleName}' is already loaded.");

        _assemblies[simpleName] = asm;
        return asm;
    }

    /// <summary>
    /// Resolves an assembly previously loaded through
    /// <see cref="LoadSharedAssembly"/> by its simple name.
    /// Used by <see cref="ModLoadContext"/> to delegate cross-mod type
    /// references to the shared ALC.
    /// </summary>
    internal bool TryGetCachedAssembly(string simpleName, [NotNullWhen(true)] out Assembly? assembly)
    {
        return _assemblies.TryGetValue(simpleName, out assembly);
    }

    /// <summary>
    /// Resolves a dependency by consulting the shared cache. Anything not
    /// owned by this context — notably <c>DualFrontier.Contracts</c> — is
    /// left to the runtime by returning <see langword="null"/> so that the
    /// default ALC can satisfy the load.
    /// </summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName?.Name is { } simpleName &&
            _assemblies.TryGetValue(simpleName, out Assembly? cached))
        {
            return cached;
        }
        return null;
    }
}
