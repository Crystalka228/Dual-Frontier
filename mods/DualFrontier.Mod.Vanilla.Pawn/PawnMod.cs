using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Vanilla.Pawn;

/// <summary>
/// Vanilla Pawn mod skeleton. Currently empty — content lands in
/// M8.5–M8.7 (ConsumeSystem / SleepSystem / ComfortAuraSystem) per the
/// MOD_OS_ARCHITECTURE v1.5 §1.3 strategic LOCKED decision #4. Establishes
/// the assembly, manifest, and IMod entry point so the mod is discoverable
/// and loadable while the DualFrontier.Systems.Pawn kernel-level systems
/// remain in place. Migration of those systems into this mod happens
/// across M8.5–M8.7.
/// </summary>
public sealed class PawnMod : IMod
{
    /// <summary>
    /// Empty initialization — no components, systems, or subscriptions
    /// registered in the M8 skeleton. Content lands in M8.5–M8.7.
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
