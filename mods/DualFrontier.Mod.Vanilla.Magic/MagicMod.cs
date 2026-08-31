using DualFrontier.Contracts.Modding;
using DualFrontier.Mod.Vanilla.Core;

namespace DualFrontier.Mod.Vanilla.Magic;

/// <summary>
/// Vanilla Magic slice. Carries NO mechanics yet — the systems it will own still live in
/// <c>src/DualFrontier.Systems</c> and move here at W5, which implements each slice clean in
/// its owning mod and deletes the src/ original in the same closure.
///
/// <para>
/// What it does today is announce its promise at load. That is deliberate and load-bearing:
/// before W4 this assembly registered nothing, so nothing ever loaded it, and two defects that
/// would have blocked a real load went unnoticed — a shared-vendor assembly name that
/// <c>ModLoader</c> could not resolve, and no route at all from the build output to a mods
/// root. Announcing the promise makes the mod exercise the real pipeline, so both are held
/// closed by a test rather than by hope.
/// </para>
///
/// <para>
/// The promise text is fetched from <see cref="VanillaSlices.PromiseFor"/> in the SHARED
/// <c>dualfrontier.vanilla.core</c> mod rather than written inline, so the load also proves the
/// shared-vendor seam: a cross-ALC call whose parameter type is owned by the shared assembly.
/// </para>
/// </summary>
public sealed class MagicMod : IMod
{
    /// <summary>
    /// Announces what this slice will own. No components, systems or subscriptions are
    /// registered — there are none to register until W5.
    /// </summary>
    public void Initialize(IModApi api)
        => api.Log(ModLogLevel.Info, VanillaSlices.PromiseFor(VanillaSlice.Magic));

    /// <summary>
    /// Nothing to release: <see cref="Initialize"/> registers nothing and subscribes to nothing.
    /// The body lands with the mechanics at W5.
    /// </summary>
    public void Unload()
    {
    }
}
