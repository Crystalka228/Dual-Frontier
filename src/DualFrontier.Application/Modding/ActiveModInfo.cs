using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Public snapshot of an active mod. Carries only fields safe for
/// non-Application callers (the controller surface returned by
/// <see cref="ModIntegrationPipeline.GetActiveMods"/>) — never the
/// <see cref="LoadedMod"/> itself, since that record exposes
/// <see cref="ModLoadContext"/> and other implementation surfaces that
/// must stay inside <c>DualFrontier.Application</c>.
/// </summary>
/// <param name="ModId">Manifest id of the active mod.</param>
/// <param name="Manifest">
/// Parsed manifest. Contains the <c>HotReload</c> flag the menu uses to
/// gate the toggle button per MOD_OS_ARCHITECTURE v1.5 §9.6.
/// </param>
public sealed record ActiveModInfo(string ModId, ModManifest Manifest);
