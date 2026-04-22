using System;
using System.Collections.Generic;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Metadata of a mod. Populated from <c>mod.manifest.json</c> at load time.
/// Used by <c>ModLoader</c> to resolve dependencies and by the UI to present
/// the mod to the user.
/// </summary>
public sealed class ModManifest
{
    /// <summary>
    /// Unique mod identifier in reverse-domain style (e.g.
    /// <c>com.example.voidmagic</c>). Used as the key in all registries.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Human-readable name of the mod, shown in the mod menu.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// SemVer version of the mod itself (e.g. <c>1.2.0</c>).
    /// Independent from the contracts version.
    /// </summary>
    public string Version { get; init; } = "0.0.0";

    /// <summary>
    /// Author displayed in the mod menu; free-form string.
    /// </summary>
    public string Author { get; init; } = string.Empty;

    /// <summary>
    /// Minimum required version of <c>DualFrontier.Contracts</c>.
    /// Format: <c>MAJOR.MINOR.PATCH</c>. Loader rejects the mod if current
    /// Contracts version is lower than this value.
    /// </summary>
    public string RequiresContractsVersion { get; init; } = "1.0.0";

    /// <summary>
    /// List of mod ids the mod depends on. Absence of any dependency blocks
    /// loading of the mod.
    /// </summary>
    public IReadOnlyList<string> Dependencies { get; init; } = Array.Empty<string>();

    /// <summary>
    /// File name of the entry assembly inside the mod package (e.g.
    /// <c>VoidMagic.dll</c>). Empty string means the loader uses
    /// <c>{Id}.dll</c> by convention.
    /// </summary>
    public string EntryAssembly { get; init; } = string.Empty;

    /// <summary>
    /// Fully-qualified type name implementing <c>IMod</c> inside
    /// <see cref="EntryAssembly"/>. Empty string lets the loader locate the
    /// implementation by scanning the assembly for a single <c>IMod</c>.
    /// </summary>
    public string EntryType { get; init; } = string.Empty;
}
