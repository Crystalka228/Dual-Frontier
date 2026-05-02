using DualFrontier.Contracts.Modding;

namespace Fixture.VanillaMod_HotReloadOverride;

/// <summary>
/// M7.4 — minimal vanilla-mod-shaped fixture used to exercise the
/// build-pipeline override (`mods/Directory.Build.targets`). This
/// project carries `&lt;IsVanillaMod&gt;true&lt;/IsVanillaMod&gt;`,
/// so a Release build flips its `mod.manifest.json` from
/// `hotReload: true` to `hotReload: false` in
/// `bin/Release/net8.0/`. A Debug build leaves the manifest at
/// `hotReload: true`.
/// </summary>
public sealed class VanillaModFixture : IMod
{
    public void Initialize(IModApi api) { }
    public void Unload() { }
}
