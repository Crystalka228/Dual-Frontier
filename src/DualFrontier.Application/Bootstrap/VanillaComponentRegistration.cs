using DualFrontier.Components.Building;
using DualFrontier.Components.Combat;
using DualFrontier.Components.Items;
using DualFrontier.Components.Magic;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Components.World;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Application.Bootstrap;

/// <summary>
/// Registers Vanilla production components with the native
/// ComponentTypeRegistry. Called from the Application bootstrap chain after
/// Bootstrap.Run() returns a ready NativeWorld.
///
/// Per K-L4 deterministic registry principle: registration order is stable,
/// type IDs are sequential (1, 2, ...). Mod-driven registration extends this
/// list at K6.
///
/// K8.3+K8.4 combined milestone extension (2026-05-14, K-L4 id stability):
/// the K4-era entries (ids 1-19) are preserved verbatim in order; the four
/// production component types previously absent from this helper
/// (IdentityComponent, SkillsComponent, MovementComponent, StorageComponent)
/// are appended at the very end of the helper rather than inserted into their
/// natural category position, so existing K4-era ids do not shift across
/// runs. Single source of truth for production component type-id assignment.
/// </summary>
public static class VanillaComponentRegistration
{
    /// <summary>
    /// Register Vanilla production components.
    /// </summary>
    /// <param name="registry">The component type registry to register with.</param>
    public static void RegisterAll(ComponentTypeRegistry registry)
    {
        // ── K4-era category blocks — ids 1-19, preserved verbatim ──────────

        // Shared category (3)
        registry.Register<HealthComponent>();         // id 1
        registry.Register<PositionComponent>();       // id 2
        registry.Register<RaceComponent>();           // id 3

        // Pawn category — trivial (3)
        registry.Register<NeedsComponent>();          // id 4
        registry.Register<MindComponent>();           // id 5
        registry.Register<JobComponent>();            // id 6

        // Items category (5)
        registry.Register<BedComponent>();            // id 7
        registry.Register<ConsumableComponent>();     // id 8
        registry.Register<DecorativeAuraComponent>(); // id 9
        registry.Register<ReservationComponent>();    // id 10
        registry.Register<WaterSourceComponent>();    // id 11

        // World category (2 — comment in K4-era helper said 3 but only 2 are registered)
        registry.Register<TileComponent>();           // id 12
        registry.Register<EtherNodeComponent>();      // id 13

        // Magic category (3 — comment in K4-era helper said 4 but only 3 are registered)
        registry.Register<EtherComponent>();          // id 14
        registry.Register<GolemBondComponent>();      // id 15
        registry.Register<ManaComponent>();           // id 16

        // Combat category (1)
        registry.Register<ArmorComponent>();          // id 17

        // ── K8.3+K8.4 cutover — Building category power components removed ──
        // PowerConsumerComponent + PowerProducerComponent were registered here
        // (ids 18-19) prior to A'.5; deleted with the Power subsystem (brief
        // v2.0 §2). Electricity will be redesigned on the GPU compute pipeline
        // in a separate future brief. Subsequent ids shift down by 2 (the
        // K8.3+K8.4 extension block below now occupies ids 18-21 instead of
        // 20-23) — acceptable because registry ids are deterministic per-run,
        // not persisted across versions.

        // ── K8.3+K8.4 extension — appended at end ─────────────────────────
        // Production components that the K4-era helper did not enumerate but
        // the factories (RandomPawnFactory + ItemFactory) and the 10
        // coreSystems require.

        registry.Register<IdentityComponent>();       // id 18 — RandomPawnFactory (per-pawn InternedString)
        registry.Register<SkillsComponent>();         // id 19 — RandomPawnFactory (per-pawn NativeMap × 2)
        registry.Register<MovementComponent>();       // id 20 — RandomPawnFactory (per-pawn NativeComposite path)
        registry.Register<StorageComponent>();        // id 21 — InventorySystem write / HaulSystem read
    }
}
