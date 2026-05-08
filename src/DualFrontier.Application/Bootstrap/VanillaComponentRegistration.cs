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
/// Registers all 24 Vanilla trivial-POCO components with the native
/// ComponentTypeRegistry. Called from the Application bootstrap chain after
/// Bootstrap.Run() returns a ready NativeWorld.
///
/// Per K4 Hybrid Path: only Категория A trivial structs are registered.
/// Категория B (collections) and Категория C (strings) components stay
/// on the managed path — not registered with the native registry.
///
/// Per K-L4 deterministic registry principle: registration order is
/// stable, type IDs are sequential (1, 2, 3, ... 24). Mod-driven
/// registration extends this list at K6.
/// </summary>
public static class VanillaComponentRegistration
{
    /// <summary>
    /// Register all 24 Vanilla trivial-POCO components.
    /// </summary>
    /// <param name="registry">The component type registry to register with.</param>
    public static void RegisterAll(ComponentTypeRegistry registry)
    {
        // Shared category (3)
        registry.Register<HealthComponent>();
        registry.Register<PositionComponent>();
        registry.Register<RaceComponent>();

        // Pawn category — trivial (3)
        registry.Register<NeedsComponent>();
        registry.Register<MindComponent>();
        registry.Register<JobComponent>();

        // Items category (5)
        registry.Register<BedComponent>();
        registry.Register<ConsumableComponent>();
        registry.Register<DecorativeAuraComponent>();
        registry.Register<ReservationComponent>();
        registry.Register<WaterSourceComponent>();

        // World category (3)
        registry.Register<TileComponent>();
        registry.Register<BiomeComponent>();
        registry.Register<EtherNodeComponent>();

        // Magic category (4)
        registry.Register<EtherComponent>();
        registry.Register<GolemBondComponent>();
        registry.Register<ManaComponent>();
        registry.Register<SchoolComponent>();

        // Combat category (4)
        registry.Register<AmmoComponent>();
        registry.Register<ArmorComponent>();
        registry.Register<ShieldComponent>();
        registry.Register<WeaponComponent>();

        // Building category — trivial (2)
        registry.Register<PowerConsumerComponent>();
        registry.Register<PowerProducerComponent>();
    }
}
