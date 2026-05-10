using System;
using System.Collections.Generic;
using DualFrontier.Components.Building;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Events.Inventory;
using DualFrontier.Systems.Inventory;
using FluentAssertions;
using Xunit;

namespace DualFrontier.IntegrationTests.InventoryDomain;

/// <summary>
/// Closes backlog item #4 + Q3: <c>ItemReservedEvent</c> is delivered (deferred) and
/// HaulSystem's per-Update reservation set prevents same-tick double
/// allocation when multiple idle pawns compete for one stack.
/// </summary>
public sealed class HaulReservationTests : IDisposable
{
    public HaulReservationTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void TwoIdlePawns_OneStack_OnlyOneReservationFires_PerTick()
    {
        var world = new World();
        var services = new GameServices();
        var ticks = new TickScheduler();
        var nativeWorld = new NativeWorld();

        EntityId src = world.CreateEntity();
        var srcStore = new StorageComponent
        {
            Capacity = 5,
            Items = nativeWorld.CreateMap<InternedString, int>(),
            AcceptAll = true,
            AllowedItems = nativeWorld.CreateSet<InternedString>(),
        };
        srcStore.Items.Set(nativeWorld.InternString("wood"), 5);
        world.AddComponent(src, srcStore);

        EntityId dst = world.CreateEntity();
        world.AddComponent(dst, new StorageComponent
        {
            Capacity = 5,
            Items = nativeWorld.CreateMap<InternedString, int>(),
            AcceptAll = true,
            AllowedItems = nativeWorld.CreateSet<InternedString>(),
        });

        EntityId pawn1 = world.CreateEntity();
        world.AddComponent(pawn1, new PositionComponent { Position = new GridVector(0, 0) });
        world.AddComponent(pawn1, new JobComponent { Current = JobKind.Idle });

        EntityId pawn2 = world.CreateEntity();
        world.AddComponent(pawn2, new PositionComponent { Position = new GridVector(1, 0) });
        world.AddComponent(pawn2, new JobComponent { Current = JobKind.Idle });

        var reservations = new List<ItemReservedEvent>();
        services.Inventory.Subscribe<ItemReservedEvent>(e => reservations.Add(e));

        var graph = new DependencyGraph();
        graph.AddSystem(new HaulSystem());
        graph.AddSystem(new InventorySystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services,
            nativeWorld);

        // First HaulSystem.Update runs at tick 0 (NORMAL = 15: 0 % 15 == 0).
        scheduler.ExecuteTick(1f / 30f);

        reservations.Should().HaveCount(1,
            "both idle pawns try to haul, but HaulSystem's per-Update reservation set "
            + "prevents the second pawn from picking the same (storage, item) pair");
        reservations[0].StorageId.Should().Be(src);
        reservations[0].ItemId.Should().Be("wood");
        reservations[0].ReservedBy.Should().BeOneOf(pawn1, pawn2);
    }

    [Fact]
    public void ItemReservedEvent_Subscriber_ReceivesAfterPhaseBoundary()
    {
        var services = new GameServices();
        var fired = new List<ItemReservedEvent>();
        services.Inventory.Subscribe<ItemReservedEvent>(e => fired.Add(e));

        var pawn = new EntityId(1, 0);
        var storage = new EntityId(2, 0);
        services.Inventory.Publish(new ItemReservedEvent
        {
            StorageId  = storage,
            ItemId     = "ammo",
            Quantity   = 3,
            ReservedBy = pawn
        });

        fired.Should().BeEmpty("[Deferred] events are queued, not delivered synchronously");

        ((IDeferredFlush)services).FlushDeferred();

        fired.Should().HaveCount(1);
        fired[0].ItemId.Should().Be("ammo");
        fired[0].Quantity.Should().Be(3);
    }
}
