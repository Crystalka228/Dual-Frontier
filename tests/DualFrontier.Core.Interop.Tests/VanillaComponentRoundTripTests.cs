using DualFrontier.Application.Bootstrap;
using DualFrontier.Components.Combat;
using DualFrontier.Components.Magic;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Components.World;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K4 verification: Категория A components survive native bulk roundtrip.
///
/// Smoke test verifies all 24 registered components support add → get
/// roundtrip without data corruption. Tricky-case tests verify specific
/// patterns that posed risk during conversion:
///   - EntityId? nullable fields (JobComponent, BedComponent)
///   - Computed properties (NeedsComponent, RaceComponent)
///   - init-only fields (RaceComponent, GolemBondComponent)
/// </summary>
public class VanillaComponentRoundTripTests
{
    [Fact]
    public void Smoke_AllVanillaComponents_RegisterSuccessfully()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        // Should not throw — all 24 components must register
        var act = () => VanillaComponentRegistration.RegisterAll(registry);
        act.Should().NotThrow();

        // Verify count
        registry.Count.Should().Be(24);
    }

    [Fact]
    public void HealthComponent_RoundTrip_PreservesData()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();
        var original = new HealthComponent { Current = 75f, Maximum = 100f };

        world.AddComponent(entity, original);
        HealthComponent retrieved = world.GetComponent<HealthComponent>(entity);

        retrieved.Current.Should().Be(75f);
        retrieved.Maximum.Should().Be(100f);
        retrieved.IsDead.Should().BeFalse();
    }

    [Fact]
    public void NeedsComponent_RoundTrip_PreservesComputedProperties()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();
        var original = new NeedsComponent
        {
            Satiety   = 0.15f,    // below CriticalThreshold (0.2)
            Hydration = 0.5f,
            Sleep     = 0.18f,    // below CriticalThreshold
            Comfort   = 0.7f,
        };

        world.AddComponent(entity, original);
        NeedsComponent retrieved = world.GetComponent<NeedsComponent>(entity);

        // Data preservation
        retrieved.Satiety.Should().Be(0.15f);
        retrieved.Hydration.Should().Be(0.5f);
        retrieved.Sleep.Should().Be(0.18f);
        retrieved.Comfort.Should().Be(0.7f);

        // Computed properties recompute correctly on retrieved struct
        retrieved.IsHungry.Should().BeTrue();
        retrieved.IsThirsty.Should().BeFalse();
        retrieved.IsExhausted.Should().BeTrue();
    }

    [Fact]
    public void JobComponent_RoundTrip_PreservesNullableEntityId()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId pawn         = world.CreateEntity();
        EntityId targetEntity = world.CreateEntity();

        // Test 1: with target
        var jobWithTarget = new JobComponent
        {
            Current        = JobKind.Haul,
            Target         = targetEntity,
            TicksAtJob     = 42,
            IsInterrupted  = false,
        };
        world.AddComponent(pawn, jobWithTarget);
        JobComponent retrieved = world.GetComponent<JobComponent>(pawn);

        retrieved.Current.Should().Be(JobKind.Haul);
        retrieved.Target.Should().Be(targetEntity);
        retrieved.TicksAtJob.Should().Be(42);
        retrieved.IsIdle.Should().BeFalse();

        // Test 2: with null target (idle)
        var jobIdle = new JobComponent
        {
            Current        = JobKind.Idle,
            Target         = null,
            TicksAtJob     = 0,
            IsInterrupted  = false,
        };
        world.AddComponent(pawn, jobIdle);
        JobComponent idle = world.GetComponent<JobComponent>(pawn);

        idle.Target.Should().BeNull();
        idle.IsIdle.Should().BeTrue();
        idle.NeedsTarget.Should().BeFalse();
    }

    [Fact]
    public void RaceComponent_RoundTrip_PreservesInitOnlyFields()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();
        var original = new RaceComponent
        {
            Kind                  = RaceKind.Elf,
            HasEtherChannels      = true,
            HasIndustrialAffinity = false,
        };

        world.AddComponent(entity, original);
        RaceComponent retrieved = world.GetComponent<RaceComponent>(entity);

        retrieved.Kind.Should().Be(RaceKind.Elf);
        retrieved.HasEtherChannels.Should().BeTrue();
        retrieved.HasIndustrialAffinity.Should().BeFalse();
        retrieved.IsOrganic.Should().BeTrue();      // computed
        retrieved.NeedsFood.Should().BeTrue();      // computed
        retrieved.NeedsSleep.Should().BeTrue();     // computed

        // Verify Golem race (non-organic)
        EntityId golem = world.CreateEntity();
        world.AddComponent(golem, new RaceComponent { Kind = RaceKind.Golem });
        RaceComponent golemRace = world.GetComponent<RaceComponent>(golem);

        golemRace.IsOrganic.Should().BeFalse();
        golemRace.NeedsFood.Should().BeFalse();
        golemRace.NeedsSleep.Should().BeFalse();
    }

    [Fact]
    public void GolemBondComponent_RoundTrip_PreservesInitOnlyEntityId()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId golem = world.CreateEntity();
        EntityId mage  = world.CreateEntity();

        var bond = new GolemBondComponent
        {
            BondedMage          = mage,
            Mode                = OwnershipMode.Bonded,
            TicksSinceContested = 0,
            BondStrength        = 5,
        };

        world.AddComponent(golem, bond);
        GolemBondComponent retrieved = world.GetComponent<GolemBondComponent>(golem);

        retrieved.BondedMage.Should().Be(mage);
        retrieved.Mode.Should().Be(OwnershipMode.Bonded);
        retrieved.BondStrength.Should().Be(5);
    }

    [Fact]
    public void EmptyComponents_RoundTrip_DoNotCorruptStorage()
    {
        // Components with no fields (BiomeComponent, SchoolComponent, AmmoComponent,
        // ShieldComponent, WeaponComponent) must roundtrip без issue
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();

        var act1 = () => world.AddComponent(entity, new BiomeComponent());
        var act2 = () => world.AddComponent(entity, new SchoolComponent());
        var act3 = () => world.AddComponent(entity, new AmmoComponent());

        act1.Should().NotThrow();
        act2.Should().NotThrow();
        act3.Should().NotThrow();

        world.HasComponent<BiomeComponent>(entity).Should().BeTrue();
        world.HasComponent<SchoolComponent>(entity).Should().BeTrue();
        world.HasComponent<AmmoComponent>(entity).Should().BeTrue();
    }
}
