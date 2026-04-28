using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Catalogue of capability tokens the kernel offers to mods. Built once at
/// startup by reflecting over kernel assemblies — the result is the
/// authoritative set of <c>kernel.publish:*</c>, <c>kernel.subscribe:*</c>,
/// <c>kernel.read:*</c> and <c>kernel.write:*</c> tokens that mods may
/// declare in <c>capabilities.required</c>.
///
/// Token rules (MOD_OS_ARCHITECTURE §3.2, §3.5, D-1):
/// <list type="bullet">
///   <item>Public, concrete <see cref="IEvent"/> implementer →
///     <c>kernel.publish:{FQN}</c> and <c>kernel.subscribe:{FQN}</c>.</item>
///   <item>Public, concrete <see cref="IComponent"/> implementer with
///     <c>[ModAccessible(Read = true)]</c> → <c>kernel.read:{FQN}</c>.</item>
///   <item>Public, concrete <see cref="IComponent"/> implementer with
///     <c>[ModAccessible(Write = true)]</c> → <c>kernel.write:{FQN}</c>.</item>
/// </list>
/// Generic and nested types (FQN containing <c>`</c> or <c>+</c>) are
/// silently skipped — they are not addressable through the capability model.
/// </summary>
internal sealed class KernelCapabilityRegistry
{
    private readonly IReadOnlySet<string> _capabilities;

    /// <summary>
    /// Scans the given assemblies for kernel-provided capabilities. Duplicate
    /// assembly references are deduplicated — passing the same assembly more
    /// than once does not double-count its tokens.
    /// </summary>
    public KernelCapabilityRegistry(IEnumerable<Assembly> assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));

        var capabilities = new HashSet<string>();
        var visited = new HashSet<Assembly>();

        foreach (Assembly assembly in assemblies)
        {
            if (assembly is null) continue;
            if (!visited.Add(assembly)) continue;

            ScanAssembly(assembly, capabilities);
        }

        _capabilities = capabilities;
    }

    /// <summary>
    /// Returns the complete set of capability tokens the kernel provides.
    /// </summary>
    public IReadOnlySet<string> Capabilities => _capabilities;

    /// <summary>
    /// Returns <see langword="true"/> when the given token is in the
    /// kernel-provided set.
    /// </summary>
    public bool Provides(string token) => _capabilities.Contains(token);

    /// <summary>
    /// Builds a registry from the assemblies that contain
    /// <c>DualFrontier.Contracts</c> and <c>DualFrontier.Components</c> types.
    /// When both marker types currently resolve to the same assembly, the
    /// constructor's deduplication keeps the scan single-pass.
    /// </summary>
    internal static KernelCapabilityRegistry BuildFromKernelAssemblies()
        => new(new[] { typeof(IEvent).Assembly, typeof(IComponent).Assembly });

    private static void ScanAssembly(Assembly assembly, HashSet<string> capabilities)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsPublic) continue;
            if (type.IsAbstract) continue;

            string? fqn = type.FullName;
            if (fqn is null) continue;
            if (fqn.IndexOf('`') >= 0) continue;
            if (fqn.IndexOf('+') >= 0) continue;

            if (typeof(IEvent).IsAssignableFrom(type))
            {
                capabilities.Add($"kernel.publish:{fqn}");
                capabilities.Add($"kernel.subscribe:{fqn}");
            }

            if (typeof(IComponent).IsAssignableFrom(type))
            {
                ModAccessibleAttribute? attr =
                    type.GetCustomAttribute<ModAccessibleAttribute>();
                if (attr is null) continue;

                if (attr.Read)
                    capabilities.Add($"kernel.read:{fqn}");
                if (attr.Write)
                    capabilities.Add($"kernel.write:{fqn}");
            }
        }
    }
}
