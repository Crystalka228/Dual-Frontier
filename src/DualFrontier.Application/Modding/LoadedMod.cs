using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Result of loading a single regular mod package — the manifest, the
/// instantiated <see cref="IMod"/>, the isolated <see cref="ModLoadContext"/>
/// and the discovered list of system types the mod declares. Produced by
/// <see cref="ModLoader.LoadRegularMod"/> before the mod is registered with
/// the scheduler. Shared mods (no entry point) use
/// <see cref="LoadedSharedMod"/> instead.
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
    IReadOnlyList<Type> DeclaredSystemTypes)
{
    /// <summary>
    /// The <see cref="RestrictedModApi"/> instance issued to this mod by
    /// <see cref="ModIntegrationPipeline.Apply"/> step [4]. Retained on the
    /// mod so the unload chain (MOD_OS_ARCHITECTURE v1.4 §9.5 step 1) can
    /// invoke <see cref="RestrictedModApi.UnsubscribeAll"/> to drop bus
    /// subscriptions before the assembly is released.
    ///
    /// Null until the pipeline assigns it during <c>Apply</c>; remains
    /// non-null for the lifetime of the loaded mod. Settable internally
    /// only — mods themselves cannot reach <see cref="LoadedMod"/> through
    /// <see cref="IModApi"/> per the §4.4 cast-prevention rule. Adding the
    /// field as an out-of-header member rather than a positional record
    /// parameter preserves every existing <c>new LoadedMod(...)</c> call
    /// site from M0–M6.
    /// </summary>
    public RestrictedModApi? Api { get; internal set; }
}
