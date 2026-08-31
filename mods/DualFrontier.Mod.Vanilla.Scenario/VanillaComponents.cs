using DualFrontier.Components.Combat;
using DualFrontier.Components.Building;
using DualFrontier.Components.Items;
using DualFrontier.Components.Magic;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Components.World;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Vanilla.Scenario;

/// <summary>
/// The 21 vanilla component types, registered through the ordinary mod SDK.
///
/// <para>
/// These registrations used to live in the engine, in a helper the composition root called
/// before anything else — which is precisely why the engine had to reference the game's
/// component assembly. They arrive here unchanged in content and changed in mechanism: ids are
/// now owner-scoped rather than the engine's sequential 1..21.
/// </para>
///
/// <para>
/// That id change is safe because component ids were never persisted and never promised to be
/// stable across runs. The engine helper's own comment said so in as many words when it shifted
/// its ids by two after the power components were deleted: "acceptable because registry ids are
/// deterministic per-run, not persisted across versions." What matters is that registration
/// happens before first use, which the pipeline guarantees — components register during the
/// mod's Initialize, and no system ticks until the scheduler rebuild that follows it.
/// </para>
///
/// <para>
/// Two component types in the assembly are deliberately NOT registered, matching the engine
/// helper exactly: FactionComponent and WorkbenchComponent, which nothing constructs or reads.
/// Registering them would be inventing content this wave has no business inventing.
/// </para>
/// </summary>
internal static class VanillaComponents
{
    /// <summary>The number of types <see cref="RegisterAll"/> registers.</summary>
    internal const int Count = 21;

    internal static void RegisterAll(IModApi api)
    {
        // Shared (3)
        api.RegisterComponent<HealthComponent>();
        api.RegisterComponent<PositionComponent>();
        api.RegisterComponent<RaceComponent>();

        // Pawn (3)
        api.RegisterComponent<NeedsComponent>();
        api.RegisterComponent<MindComponent>();
        api.RegisterComponent<JobComponent>();

        // Items (5)
        api.RegisterComponent<BedComponent>();
        api.RegisterComponent<ConsumableComponent>();
        api.RegisterComponent<DecorativeAuraComponent>();
        api.RegisterComponent<ReservationComponent>();
        api.RegisterComponent<WaterSourceComponent>();

        // World (2)
        api.RegisterComponent<TileComponent>();
        api.RegisterComponent<EtherNodeComponent>();

        // Magic (3)
        api.RegisterComponent<EtherComponent>();
        api.RegisterComponent<GolemBondComponent>();
        api.RegisterComponent<ManaComponent>();

        // Combat (1)
        api.RegisterComponent<ArmorComponent>();

        // Required by the scenario seeding and the ten gameplay systems (4)
        api.RegisterComponent<IdentityComponent>();
        api.RegisterComponent<SkillsComponent>();
        api.RegisterComponent<MovementComponent>();
        api.RegisterComponent<StorageComponent>();
    }
}
