using System;
using System.Reflection;
using System.Runtime.Loader;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Dedicated <see cref="AssemblyLoadContext"/> for a single mod. Created with
/// <c>isCollectible: true</c> so the mod can be unloaded without restarting
/// the game. Physically isolates the mod's assembly from the core
/// (TechArch 11.8).
/// </summary>
internal sealed class ModLoadContext : AssemblyLoadContext
{
    /// <summary>
    /// TODO: Phase 2 — creates a context for a mod with the given name.
    /// The name is used for diagnostics and hot unload.
    /// </summary>
    /// <param name="name">Unique context name (typically the mod id).</param>
    public ModLoadContext(string name)
        : base(name, isCollectible: true)
    {
    }

    /// <summary>
    /// Resolves a mod dependency by delegating to the default context.
    /// Returning <c>null</c> lets the parent load shared assemblies such as
    /// <c>DualFrontier.Contracts</c>, while the private context still owns
    /// the mod's own assemblies loaded via <c>LoadFromAssemblyPath</c> —
    /// keeping them isolated and collectible. A follow-up phase can filter
    /// the delegated set to harden isolation against accidental loads of
    /// <c>DualFrontier.Core</c> and siblings.
    /// </summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return null;
    }
}
