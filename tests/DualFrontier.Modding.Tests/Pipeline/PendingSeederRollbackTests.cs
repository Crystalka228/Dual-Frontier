using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Sdk;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// The world-seeder queue must not survive a rollback.
///
/// <para>
/// W4 gave a mod a way to seed the world once, outside the system graph, because the scheduler's
/// one-writer-per-component rule makes seeding impossible to express as a system. The queue that
/// carries those delegates was added to the registry and then left out of the registry's two
/// teardown paths, which is the ordinary way a new piece of state acquires a rollback hole: the
/// paths were written before the state existed.
/// </para>
///
/// <para>
/// A mod registers its seeder during <c>Initialize</c> — before the graph build, before the
/// validation that can still reject the batch. So a rejected mod leaves a live delegate behind,
/// and the next apply that succeeds runs it. The world would then be authored by code the host
/// refused, and the delegate would hold that mod's unloaded collectible context alive to do it.
/// </para>
///
/// <para>
/// These assert the registry contract directly rather than through a failing <c>Apply</c>. That
/// is deliberate: <see cref="ModRegistry.ResetModSystems"/> is reached only from the pipeline's
/// two rollback blocks, so pinning it here pins both call sites at once, and does so without
/// building a mod whose only purpose is to fail in a particular place.
/// </para>
/// </summary>
public sealed class PendingSeederRollbackTests
{
    private static Action<ISystemContext> Noop => _ => { };

    [Fact]
    public void RollingBackAFailedApplyDropsThatModsQueuedSeeder()
    {
        var registry = new ModRegistry();
        registry.RegisterWorldSeeder("mod.rejected", Noop);

        registry.ResetModSystems();

        registry.TakePendingSeeders().Should().BeEmpty(
            "a mod whose apply was rolled back must not seed the world on somebody else's apply");
    }

    [Fact]
    public void UnloadingOneModDropsOnlyItsOwnSeeder()
    {
        var registry = new ModRegistry();
        registry.RegisterWorldSeeder("mod.leaving", Noop);
        registry.RegisterWorldSeeder("mod.staying", Noop);

        registry.RemoveMod("mod.leaving");

        IReadOnlyList<(string ModId, Action<ISystemContext> Seed)> remaining =
            registry.TakePendingSeeders();

        remaining.Should().ContainSingle(
            "a single-mod unload is not a batch rollback: the other mod's seeder is still owed");
        remaining[0].ModId.Should().Be("mod.staying");
    }

    [Fact]
    public void TheQueueIsStillDrainedExactlyOnceOnTheSuccessPath()
    {
        // The guard against over-correcting. Clearing on rollback must not clear on the way to a
        // successful seed, and nothing may drain the queue twice — a seeder that ran once and is
        // handed out again would re-seed a live world.
        var registry = new ModRegistry();
        registry.RegisterWorldSeeder("mod.good", Noop);

        registry.TakePendingSeeders().Should().ContainSingle();
        registry.TakePendingSeeders().Should().BeEmpty();
    }
}
