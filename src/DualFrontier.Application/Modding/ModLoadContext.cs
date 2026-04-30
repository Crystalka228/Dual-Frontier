using System;
using System.Reflection;
using System.Runtime.Loader;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Dedicated <see cref="AssemblyLoadContext"/> for a single regular mod.
/// Created with <c>isCollectible: true</c> so the mod can be unloaded without
/// restarting the game. Physically isolates the mod's assembly from the core
/// (TechArch 11.8). When a <see cref="SharedModLoadContext"/> is supplied,
/// the resolver delegates to it for any assembly already loaded into the
/// shared ALC — this is how cross-mod type identity is preserved per
/// MOD_OS_ARCHITECTURE §5.3.
/// </summary>
internal sealed class ModLoadContext : AssemblyLoadContext
{
    private readonly SharedModLoadContext? _sharedAlc;

    /// <summary>
    /// Creates a context for a mod with the given name. <paramref name="sharedAlc"/>
    /// is the singleton shared ALC the resolver consults for cross-mod type
    /// references; pass <see langword="null"/> for legacy single-mod usage
    /// (tests, ad-hoc loads) — the resolver then defers everything to the
    /// default context.
    /// </summary>
    /// <param name="name">Unique context name (typically the mod id).</param>
    /// <param name="sharedAlc">Shared ALC to delegate cross-mod resolves to, or null.</param>
    public ModLoadContext(string name, SharedModLoadContext? sharedAlc = null)
        : base(name, isCollectible: true)
    {
        _sharedAlc = sharedAlc;
    }

    /// <summary>
    /// Resolves a mod dependency. When a shared assembly with the requested
    /// simple name is cached in the shared ALC, returns that assembly so
    /// dependent mods see the same <see cref="Type"/> instance for shared
    /// types (MOD_OS_ARCHITECTURE §5.3). Otherwise returns <see langword="null"/>
    /// and lets the parent default ALC handle <c>DualFrontier.Contracts</c>
    /// and the rest. The mod's own assemblies (loaded via
    /// <c>LoadFromAssemblyPath</c>) remain in this private context — keeping
    /// them isolated and collectible.
    /// </summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName?.Name is { } simpleName &&
            _sharedAlc is not null &&
            _sharedAlc.TryGetCachedAssembly(simpleName, out Assembly? shared))
        {
            return shared;
        }
        return null;
    }
}
