using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Vanilla.Combat;

/// <summary>
/// Vanilla Combat mod skeleton. Currently empty — content lands in M9 Combat
/// (per MOD_OS_ARCHITECTURE v1.5 §1.3 strategic LOCKED decision #4 mapping:
/// Faction folds under Combat for the raid pipeline). Establishes the
/// assembly, manifest, and IMod entry point so the mod is discoverable and
/// loadable while the DualFrontier.Systems.Combat / .Faction kernel-level
/// systems remain in place. Migration of those systems into this mod
/// happens in M9.
/// </summary>
public sealed class CombatMod : IMod
{
    /// <summary>
    /// Empty initialization — no components, systems, or subscriptions
    /// registered in the M8 skeleton. Content lands in M9.
    /// </summary>
    public void Initialize(IModApi api)
    {
        // TODO: register components, systems, subscriptions when content lands.
    }

    /// <summary>
    /// Empty unload — nothing to release in the skeleton. Body lands when
    /// Initialize starts registering things.
    /// </summary>
    public void Unload()
    {
        // TODO: unsubscribe events, release resources when content lands.
    }
}
