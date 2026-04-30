using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Result of loading a single shared mod (MOD_OS_ARCHITECTURE §1.2, §5) —
/// the manifest, the singleton <see cref="SharedModLoadContext"/> the
/// assembly is hosted in, the loaded assembly itself, and the list of
/// public types it exports for use by regular mods.
///
/// Shared mods have no <see cref="IMod"/> entry point: they are pure type
/// vendors. <see cref="LoadedMod"/> is the corresponding record for regular
/// mods (which do have an entry point).
/// </summary>
/// <param name="ModId">Unique identifier from <see cref="ModManifest.Id"/>.</param>
/// <param name="Manifest">Parsed manifest metadata; <c>Manifest.Kind</c> equals <see cref="ModKind.Shared"/>.</param>
/// <param name="Context">
/// Process-wide shared ALC that owns this assembly. Always the same
/// instance for every <c>LoadedSharedMod</c> in the session per §5.1.
/// </param>
/// <param name="Assembly">The shared mod's loaded assembly.</param>
/// <param name="ExportedTypes">
/// Public types exported by the assembly (records, classes, interfaces,
/// enums, structs). Captured for downstream M4.2/M4.3 work
/// (capability registry refresh, contract type scans); M4.1 does not
/// consume them.
/// </param>
internal sealed record LoadedSharedMod(
    string ModId,
    ModManifest Manifest,
    AssemblyLoadContext Context,
    Assembly Assembly,
    IReadOnlyList<Type> ExportedTypes);
