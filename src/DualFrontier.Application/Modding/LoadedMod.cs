using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Result of loading a single mod package — the manifest, the instantiated
/// <see cref="IMod"/>, the isolated <see cref="ModLoadContext"/> and the
/// discovered list of system types the mod declares. Produced by
/// <see cref="ModLoader"/> before the mod is registered with the scheduler.
/// </summary>
/// <param name="ModId">Unique identifier taken from <see cref="ModManifest.Id"/>.</param>
/// <param name="Manifest">Parsed manifest metadata.</param>
/// <param name="Instance"><c>IMod</c> instance ready for <c>Initialize</c>.</param>
/// <param name="Context">Assembly load context owning the mod's assemblies.</param>
/// <param name="DeclaredSystemTypes">
/// All concrete <see cref="DualFrontier.Core.ECS.SystemBase"/> subclasses found
/// in the mod assemblies. Used by <see cref="ContractValidator"/> to detect
/// write-write conflicts before <c>IMod.Initialize</c> runs.
/// </param>
internal sealed record LoadedMod(
    string ModId,
    ModManifest Manifest,
    IMod Instance,
    ModLoadContext Context,
    IReadOnlyList<Type> DeclaredSystemTypes);
