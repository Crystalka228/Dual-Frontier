using System;
using System.Collections.Generic;
using DualFrontier.Components.Building;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Systems.Inventory;
using FluentAssertions;
using Xunit;

namespace DualFrontier.IntegrationTests.InventoryDomain;

/// <summary>
/// Closes backlog item #3 + Q4: <c>HaulSystem</c> publishes <c>ItemRemovedEvent</c>
/// and <c>ItemAddedEvent</c> while declaring <c>writes: []</c>;
/// <c>InventorySystem</c> subscribes and applies the mutation. Pre-fix this
/// would crash with <c>IsolationViolationException</c> because the handler
/// would run inside HaulSystem's execution context. After the fix the events
/// are <c>[Deferred]</c> and dispatched at the phase boundary inside
/// InventorySystem's captured context, so <c>SetComponent&lt;StorageComponent&gt;</c>
/// stays within InventorySystem's declared writes.
/// </summary>
public sealed class CrossSystemMutationIsolationTests : IDisposable
{
    public CrossSystemMutationIsolationTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void Haul_StackBetweenStorages_AppliesAtNextPhase_NoIsolationViolation()
    {
        var world = new World();
        var services = new GameServices();
        var ticks = new TickScheduler();

        // Two real StorageComponent entities — pre-fix this would have been
        // enough to surface the cross-context isolation violation.
        EntityId src = world.CreateEntity();
        var srcStore = new StorageComponent { Capacity = 5 };
        srcStore.Items["wood"] = 7;
        world.AddComponent(src, srcStore);

        EntityId dst = world.CreateEntity();
        world.AddComponent(dst, new StorageComponent { Capacity = 5 });

        EntityId pawn = world.CreateEntity();
        world.AddComponent(pawn, new PositionComponent { Position = new GridVector(0, 0) });
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Idle });

        var graph = new DependencyGraph();
        graph.AddSystem(new HaulSystem());
        graph.AddSystem(new InventorySystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services);

        // Single tick: HaulSystem (NORMAL=15) fires at tick 0 and the
        // deferred events drain at the phase boundary inside
        // InventorySystem's captured context. Multi-tick runs would oscillate
        // the stack between storages — that orthogonal concern is out of
        // scope for this isolation check.
        Action act = () => scheduler.ExecuteTick(1f / 30f);

        act.Should().NotThrow("[Deferred] events deliver inside InventorySystem's captured context, "
                              + "so SetComponent<StorageComponent> respects HaulSystem's empty writes "
                              + "and InventorySystem's writes=[StorageComponent]");

        world.TryGetComponent<StorageComponent>(src, out var srcAfter).Should().BeTrue();
        world.TryGetComponent<StorageComponent>(dst, out var dstAfter).Should().BeTrue();

        srcAfter.Items.ContainsKey("wood").Should().BeFalse(
            "the entire wood stack moved out of the source storage");
        dstAfter.Items.TryGetValue("wood", out int dstQty).Should().BeTrue();
        dstQty.Should().Be(7);
    }
}
