using System;
using DualFrontier.Components.Building;
using DualFrontier.Components.Combat;
using DualFrontier.Components.Items;
using DualFrontier.Components.Magic;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Components.World;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Systems.Tests.Fixtures;

/// <summary>
/// Shared test fixture for system-level tests post-K8.3+K8.4 cutover. Bootstraps a registry-bound
/// <see cref="NativeWorld"/> and registers the vanilla component types, so every system under
/// test can write before any assertion runs.
///
/// <para>
/// <b>Why the list is here.</b> Until W4 this fixture called the engine's vanilla-registration
/// helper, on the principle that a test must register components through the SAME helper
/// production calls. That helper no longer exists: the vanilla component set is content, it
/// belongs to the mod that owns it, and production registers it through <c>IModApi</c> during
/// that mod's initialisation. A fixture holding a bare <see cref="ComponentTypeRegistry"/> cannot
/// reach that path, and dragging a mod assembly into this project to borrow its list would couple
/// a systems test suite to a content package for no gain.
/// </para>
///
/// <para>
/// The single-source-of-truth property is preserved where it matters: production has exactly one
/// registration site. This list is a test PRECONDITION, and it is self-correcting rather than
/// silently stale — a component missing from it makes the first system that writes the type throw,
/// which is a loud failure in the test that needs it rather than a quiet gap in coverage.
/// </para>
/// </summary>
public sealed class NativeWorldTestFixture : IDisposable
{
    /// <summary>The number of component types <see cref="NativeWorldTestFixture"/> registers.</summary>
    public const int RegisteredComponentCount = 21;

    public NativeWorld NativeWorld { get; }
    public ComponentTypeRegistry Registry { get; }
    public IGameServices Services { get; }

    public NativeWorldTestFixture()
    {
        NativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        Registry = NativeWorld.Registry!;
        RegisterVanillaComponents(Registry);
        Services = new GameServices();
    }

    /// <summary>
    /// The 21 vanilla component types, in the order the vanilla scenario mod registers them.
    /// FactionComponent and WorkbenchComponent are deliberately absent, matching the mod: nothing
    /// constructs or reads either.
    /// </summary>
    private static void RegisterVanillaComponents(ComponentTypeRegistry registry)
    {
        // Shared (3)
        registry.Register<HealthComponent>();
        registry.Register<PositionComponent>();
        registry.Register<RaceComponent>();

        // Pawn (3)
        registry.Register<NeedsComponent>();
        registry.Register<MindComponent>();
        registry.Register<JobComponent>();

        // Items (5)
        registry.Register<BedComponent>();
        registry.Register<ConsumableComponent>();
        registry.Register<DecorativeAuraComponent>();
        registry.Register<ReservationComponent>();
        registry.Register<WaterSourceComponent>();

        // World (2)
        registry.Register<TileComponent>();
        registry.Register<EtherNodeComponent>();

        // Magic (3)
        registry.Register<EtherComponent>();
        registry.Register<GolemBondComponent>();
        registry.Register<ManaComponent>();

        // Combat (1)
        registry.Register<ArmorComponent>();

        // Required by scenario seeding and the ten gameplay systems (4)
        registry.Register<IdentityComponent>();
        registry.Register<SkillsComponent>();
        registry.Register<MovementComponent>();
        registry.Register<StorageComponent>();
    }

    public void Dispose() => NativeWorld.Dispose();
}
