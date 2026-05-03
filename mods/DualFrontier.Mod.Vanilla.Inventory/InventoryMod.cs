using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Vanilla.Inventory;

/// <summary>
/// Vanilla Inventory mod skeleton. Currently empty — content lands in M10
/// Inventory (per MOD_OS_ARCHITECTURE v1.5 §1.3 strategic LOCKED decision #4
/// mapping: Power folds under Inventory for the economy pipeline). Establishes
/// the assembly, manifest, and IMod entry point so the mod is discoverable
/// and loadable while the DualFrontier.Systems.Inventory / .Power kernel-level
/// systems remain in place. Migration of those systems into this mod happens
/// in M10.
/// </summary>
public sealed class InventoryMod : IMod
{
    /// <summary>
    /// Empty initialization — no components, systems, or subscriptions
    /// registered in the M8 skeleton. Content lands in M10.
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
