using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Public record returned by <see cref="IModDiscoverer.Discover"/>.
/// Carries the on-disk path so the controller can pass it to
/// <see cref="ModIntegrationPipeline.Apply"/>, plus the parsed manifest
/// so the menu can display name / version / hotReload flag without
/// re-parsing the JSON file.
/// </summary>
/// <param name="Path">
/// Pipeline-acceptable path — same string later passed to
/// <see cref="ModIntegrationPipeline.Apply"/> when the user commits the
/// mod into the active set.
/// </param>
/// <param name="Manifest">Parsed mod manifest.</param>
public sealed record DiscoveredModInfo(string Path, ModManifest Manifest);
