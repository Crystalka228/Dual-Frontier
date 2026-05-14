using System;
using System.Collections.Generic;

namespace DualFrontier.Contracts.Modding;

/// <summary>Identifies the structural role of a mod in the load graph.</summary>
public enum ModKind
{
    /// <summary>Default. Has an IMod entry point, registers systems and components.</summary>
    Regular,

    /// <summary>
    /// Pure type vendor. No IMod entry point. Assembly is loaded into the
    /// shared AssemblyLoadContext so all regular mods see identical Type instances.
    /// </summary>
    Shared,
}

/// <summary>
/// Metadata of a mod. Populated from <c>mod.manifest.json</c> at load time.
/// Used by <c>ModLoader</c> to resolve dependencies and by the UI to present
/// the mod to the user.
/// </summary>
public sealed class ModManifest
{
    /// <summary>
    /// K8.3+K8.4 — Manifest schema version. Distinguishes the IModApi
    /// surface available to the mod:
    /// <list type="bullet">
    ///   <item>"3" — IModApi v3 (the only supported version post-K8.3+K8.4).
    ///         RegisterComponent&lt;T&gt; constrained to unmanaged structs;
    ///         RegisterManagedComponent&lt;T&gt; available for Path β class
    ///         shapes annotated with [ManagedStorage]; Fields,
    ///         ComputePipelines accessible.</item>
    /// </list>
    /// Post-K8.3+K8.4 combined milestone, only "3" is accepted by
    /// <c>ManifestParser</c>. v1/v2 manifests are rejected with an
    /// <c>InvalidOperationException</c> at parse time (carries the
    /// <c>ValidationErrorKind.IncompatibleContractsVersion</c> semantic in
    /// the message). No grace period; no deprecation warnings; no
    /// backward compatibility — per Crystalka direction 2026-05-13.
    ///
    /// Default value "3" applies to programmatic construction (e.g. tests
    /// that instantiate <see cref="ModManifest"/> with <c>new { Id = "x", ... }</c>).
    /// On-disk JSON manifests must explicitly declare
    /// <c>"manifestVersion": "3"</c> — <c>ManifestParser.Parse</c> enforces
    /// presence and exact value even though the C# default would otherwise
    /// fire for a JSON object missing the field.
    /// </summary>
    public string ManifestVersion { get; init; } = "3";

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
    /// v1 field — minimum required <c>DualFrontier.Contracts</c> version in
    /// <c>MAJOR.MINOR.PATCH</c> format. Kept for backward compatibility; v2
    /// manifests should populate <see cref="ApiVersion"/> instead. Read by
    /// <see cref="EffectiveApiVersion"/> as a fallback when
    /// <see cref="ApiVersion"/> is <see langword="null"/>.
    /// </summary>
    public string RequiresContractsVersion { get; init; } = "1.0.0";

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

    /// <summary>
    /// Mod dependencies with optional version constraints and an optional
    /// flag. Absence of any required dependency blocks loading of the mod.
    /// </summary>
    public IReadOnlyList<ModDependency> Dependencies { get; init; } = Array.Empty<ModDependency>();

    /// <summary>
    /// Structural role of the mod. <see cref="ModKind.Regular"/> mods have an
    /// <c>IMod</c> entry point; <see cref="ModKind.Shared"/> assemblies act as
    /// pure type vendors loaded into the shared assembly load context.
    /// </summary>
    public ModKind Kind { get; init; } = ModKind.Regular;

    /// <summary>
    /// v2 API-version constraint against <c>DualFrontier.Contracts</c>. When
    /// <see langword="null"/> (v1 compatibility), the loader falls back to
    /// parsing <see cref="RequiresContractsVersion"/>.
    /// </summary>
    public VersionConstraint? ApiVersion { get; init; }

    /// <summary>
    /// When <see langword="true"/>, the mod opts in to mid-session reload.
    /// When <see langword="false"/> (default), the mod cannot be reloaded
    /// without a full session restart.
    /// </summary>
    public bool HotReload { get; init; }

    /// <summary>
    /// Fully-qualified type names of kernel bridge systems that this mod
    /// supersedes. Each target system must be marked
    /// <c>[BridgeImplementation(Replaceable = true)]</c>; otherwise the
    /// loader rejects the manifest with
    /// <c>ValidationErrorKind.ProtectedSystemReplacement</c>.
    /// </summary>
    public IReadOnlyList<string> Replaces { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Required and provided capabilities declared by the manifest. Defaults
    /// to <see cref="ManifestCapabilities.Empty"/> for v1 manifests that do
    /// not declare any capabilities.
    /// </summary>
    public ManifestCapabilities Capabilities { get; init; } = ManifestCapabilities.Empty;

    /// <summary>
    /// Returns the effective API version constraint for this manifest.
    /// Prefers <see cref="ApiVersion"/> when set; falls back to parsing
    /// <see cref="RequiresContractsVersion"/> for v1 manifests.
    /// </summary>
    public VersionConstraint EffectiveApiVersion =>
        ApiVersion ?? VersionConstraint.Parse(RequiresContractsVersion);
}
