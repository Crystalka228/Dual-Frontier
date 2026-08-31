using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Contracts.Services;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sdk;

/// <summary>
/// W4 / D10 — a mod can register a system that needs a service at construction.
///
/// <para>
/// Both mod registration arms previously ended in a bare <c>Activator.CreateInstance</c>, so a
/// mod could only register types with a public parameterless constructor. That is not a niche
/// limitation. <c>MovementSystem</c> takes an <c>IPathfindingService</c>, so it could not be
/// registered from a mod at all — and because a throw out of <c>IMod.Initialize</c> rolls the
/// whole load batch back, one unconstructible system takes every other system in that mod down
/// with it. Ten systems would have failed because of one.
/// </para>
///
/// <para>
/// The same overload closes a second, quieter gap. <c>ModRegistry._systemServices</c> was read
/// at exactly one site, inside the CORE registration overload; the mod path never touched it.
/// A service provided for mods therefore reached nothing regardless of WHEN it was provided —
/// a wiring gap, not a timing one, and no ordering change would have fixed it. These facts pin
/// that a mod-registered system now genuinely receives what the host provided.
/// </para>
/// </summary>
public sealed class ModFactoryRegistrationTests
{
    [Fact]
    public void AModRegisteredSystemReceivesTheServicesTheHostProvided()
    {
        var registry = new ModRegistry();
        var pathfinding = new StubPathfinding();
        var grid = new StubNavGrid();
        registry.SetSystemServices(new SystemServices(pathfinding, grid));

        ServiceTakingSystem? built = null;
        registry.RegisterSystem<ServiceTakingSystem>("test.mod.factory", s =>
        {
            built = new ServiceTakingSystem(s.Pathfinding, s.NavGrid);
            return built;
        });

        built.Should().NotBeNull("the factory must run at registration");
        built!.Pathfinding.Should().BeSameAs(pathfinding,
            "the value must be the one the HOST provided, not a fresh instance — this is the " +
            "half that was previously unreachable from the mod path at any time");
        built.NavGrid.Should().BeSameAs(grid);
    }

    [Fact]
    public void AFactoryRegisteredSystemLandsInTheGraphAsAModSystem()
    {
        var registry = new ModRegistry();
        registry.SetSystemServices(new SystemServices(new StubPathfinding(), new StubNavGrid()));

        registry.RegisterSystem<ServiceTakingSystem>(
            "test.mod.factory", s => new ServiceTakingSystem(s.Pathfinding, s.NavGrid));

        IReadOnlyList<SystemRegistration> all = registry.GetAllSystems();

        all.Should().ContainSingle("exactly one system was registered");
        all[0].Origin.Should().Be(SystemOrigin.Mod, "it was registered by a mod, not by the core");
        all[0].ModId.Should().Be("test.mod.factory");
        registry.GetCoreSystemInstances().Should().BeEmpty(
            "the mod path must not leak into the core list — the pipeline's graph build reads " +
            "GetAllSystems, but the composition root's boot graph reads the core list alone");
    }

    [Fact]
    public void TheParameterlessModPathStillRefusesASystemNeedingAService()
    {
        var registry = new ModRegistry();
        registry.SetSystemServices(new SystemServices(new StubPathfinding(), new StubNavGrid()));

        Action act = () => registry.RegisterSystem("test.mod.factory", typeof(ServiceTakingSystem));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*parameterless constructor*",
                "the old path is unchanged and still refuses — the factory overload is an " +
                "addition, not a loosening. The diagnostic now names the overload as the remedy")
            .WithMessage("*RegisterSystem<T>(Func<ISystemServices, T>)*");
    }

    [Fact]
    public void TheFactoryPathEnforcesTheSameDeclarationsAsTheTypePath()
    {
        var registry = new ModRegistry();
        registry.SetSystemServices(new SystemServices(new StubPathfinding(), new StubNavGrid()));

        Action act = () => registry.RegisterSystem<UndeclaredSystem>(
            "test.mod.factory", _ => new UndeclaredSystem());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*[SystemAccess]*",
                "both arms share one validator: a factory must not be a way around the " +
                "declarations the scheduler needs to build a context and resolve cadence");
    }

    [Fact]
    public void AFactoryPathWithNoServicesInstalledFailsLoudly()
    {
        var registry = new ModRegistry();
        // SetSystemServices deliberately NOT called.

        Action act = () => registry.RegisterSystem<ServiceTakingSystem>(
            "test.mod.factory", s => new ServiceTakingSystem(s.Pathfinding, s.NavGrid));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*ISystemServices*",
                "fail-closed: a host that never provided services must not hand a mod a null " +
                "surface to dereference");
    }

    // ── Fixtures ─────────────────────────────────────────────────────────────

    public struct FactoryMarker : IComponent { }
    public struct UndeclaredMarker : IComponent { }

    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(FactoryMarker) })]
    internal sealed class ServiceTakingSystem : SystemBase
    {
        public ServiceTakingSystem(IPathfindingService pathfinding, INavGridService navGrid)
        {
            Pathfinding = pathfinding;
            NavGrid = navGrid;
        }

        public IPathfindingService Pathfinding { get; }

        public INavGridService NavGrid { get; }

        public override void Update(float delta) { }
    }

    /// <summary>Deliberately carries no <c>[SystemAccess]</c>, to prove the shared validator runs.</summary>
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    internal sealed class UndeclaredSystem : SystemBase
    {
        public override void Update(float delta) { }
    }
}
