using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace DualFrontier.Launcher;

/// <summary>
/// Lets the default load context find assemblies that ship in the distribution root but are not
/// listed in the host's dependency file.
///
/// <para>
/// <b>Why this is needed at all.</b> W4 removed the engine's references to the game's assemblies,
/// which is the point of the wave — but a reference is also what puts an assembly into
/// <c>DualFrontier.Launcher.deps.json</c>, and the default context resolves from that list rather
/// than by scanning its own directory. So the moment the engine stopped naming
/// <c>DualFrontier.Components</c>, the runtime stopped being able to load it, even though the
/// file was sitting beside the executable the whole time. The failure is not subtle: the mod that
/// owns the colony refuses to validate, and the product will not start.
/// </para>
///
/// <para>
/// <b>Why the DEFAULT context and not the mod's.</b> A mod's own collectible context could load a
/// private copy, and that is the wrong answer here: component types must have ONE identity across
/// every mod that touches them, and two mods each carrying their own
/// <c>DualFrontier.Components</c> would see two incompatible <c>PositionComponent</c> types. The
/// mod OS already has a mechanism for shared identity (the shared ALC, fed by shared mods), and
/// these assemblies are not a mod. Resolving them once, in the context every mod falls back to,
/// keeps one identity without inventing a second sharing mechanism.
/// </para>
///
/// <para>
/// <b>What this is not.</b> It is not a dependency resolver for mods. A mod that ships its own
/// private dependencies still cannot load them — <c>ModLoadContext</c> never probes the mod
/// directory and carries no <c>AssemblyDependencyResolver</c>. That gap is ledgered; this class
/// closes only the host's half, which is the half the boundary cut opened.
/// </para>
/// </summary>
internal static class DistributionAssemblyProbe
{
    /// <summary>
    /// Registers the probe on the default context. Call once, before anything that loads content.
    /// </summary>
    /// <param name="distributionRoot">The directory the distribution manifest was found in.</param>
    internal static void Install(string distributionRoot)
    {
        ArgumentNullException.ThrowIfNull(distributionRoot);
        AssemblyLoadContext.Default.Resolving += (context, name) =>
        {
            string? path = PathFor(distributionRoot, name);
            return path is null ? null : context.LoadFromAssemblyPath(path);
        };
    }

    /// <summary>
    /// The file a requested assembly would be loaded from, or <see langword="null"/> when the
    /// distribution does not carry it.
    ///
    /// <para>
    /// Simple-name lookup with no version matching, deliberately: everything in the distribution
    /// root was built together from this repository, so a version mismatch is not a case that can
    /// arise, and pretending to check for one would be theatre. A request for an assembly the
    /// distribution does not carry returns null and the runtime reports its own error, which names
    /// the assembly and is more useful than anything this could throw.
    /// </para>
    /// </summary>
    internal static string? PathFor(string distributionRoot, AssemblyName name)
    {
        if (string.IsNullOrEmpty(name?.Name)) return null;

        // A simple name never contains a separator; refusing one that does keeps a hostile or
        // malformed name from reaching outside the distribution root.
        if (name.Name.Contains(Path.DirectorySeparatorChar) ||
            name.Name.Contains(Path.AltDirectorySeparatorChar))
        {
            return null;
        }

        string candidate = Path.Combine(distributionRoot, name.Name + ".dll");
        return File.Exists(candidate) ? candidate : null;
    }
}
