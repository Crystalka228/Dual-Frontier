using System.Collections.Generic;
using DualFrontier.Contracts.Distribution;

namespace DualFrontier.Application.Distribution;

/// <summary>
/// What a distribution declares about itself: which product it is, which mods constitute it,
/// what scenario it starts in, and where its assets live.
///
/// <para>
/// This is the artifact boundary law B-5 names — "switching the game means switching the
/// manifest, not forking the bootstrap". Before W4 there was no such file and no loader for one
/// anywhere in the repository; the composition root hardcoded every value and the mod pipeline,
/// though fully built, was never asked to load anything.
/// </para>
///
/// <para>
/// The type is engine-side and internal. Mods do not read the manifest — they read
/// <see cref="ScenarioConfig"/>, which the host hands them. That split is deliberate: the
/// scenario is content a mod acts on, while the root mod set and the asset roots are host
/// concerns a mod has no business inspecting.
/// </para>
/// </summary>
internal sealed record DistributionManifest(
    string ManifestVersion,
    ProductInfo Product,
    IReadOnlyList<string> RootMods,
    ScenarioConfig Scenario,
    IReadOnlyList<string> AssetRoots,
    string SaveNamespace,
    IReadOnlyList<string> MinEngineCapabilities)
{
    /// <summary>The only manifest version this build accepts.</summary>
    internal const string SupportedVersion = "1";

    /// <summary>The manifest's own file name, at the distribution root.</summary>
    internal const string FileName = "game.manifest.json";
}

/// <summary>Identity of the shipped product.</summary>
/// <param name="Id">Stable product identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="Version">Product version string.</param>
internal sealed record ProductInfo(string Id, string Name, string Version);
