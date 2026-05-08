using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class WriteBatchTests
{
    private struct TestComponent
    {
        public int Value;
    }

    [Fact]
    public void Update_RecordsAndAppliesOnFlush()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new TestComponent { Value = 10 });

        using (var batch = world.BeginBatch<TestComponent>())
        {
            batch.Update(entity, new TestComponent { Value = 100 }).Should().BeTrue();
            int applied = batch.Flush();
            applied.Should().Be(1);
        }

        world.GetComponent<TestComponent>(entity).Value.Should().Be(100);
    }

    [Fact]
    public void Add_AppliesWhenComponentMissing()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        // Pre-create the type's store via an unrelated entity so BeginBatch
        // doesn't need any prior registration.
        EntityId other = world.CreateEntity();
        world.AddComponent(other, new TestComponent { Value = 0 });
        world.RemoveComponent<TestComponent>(other);

        using (var batch = world.BeginBatch<TestComponent>())
        {
            batch.Add(entity, new TestComponent { Value = 42 }).Should().BeTrue();
            batch.Flush().Should().Be(1);
        }

        world.HasComponent<TestComponent>(entity).Should().BeTrue();
        world.GetComponent<TestComponent>(entity).Value.Should().Be(42);
    }

    [Fact]
    public void Remove_RemovesExistingComponent()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new TestComponent { Value = 10 });

        using (var batch = world.BeginBatch<TestComponent>())
        {
            batch.Remove(entity).Should().BeTrue();
            batch.Flush().Should().Be(1);
        }

        world.HasComponent<TestComponent>(entity).Should().BeFalse();
    }

    [Fact]
    public void Cancel_DoesNotApplyCommands()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new TestComponent { Value = 10 });

        using (var batch = world.BeginBatch<TestComponent>())
        {
            batch.Update(entity, new TestComponent { Value = 999 });
            batch.Cancel();
            // Implicit Dispose after cancel — must not auto-flush.
        }

        world.GetComponent<TestComponent>(entity).Value.Should().Be(10);
    }

    [Fact]
    public void MixedCommands_AppliedAtomically()
    {
        using var world = new NativeWorld();
        EntityId e1 = world.CreateEntity();
        EntityId e2 = world.CreateEntity();
        EntityId e3 = world.CreateEntity();

        world.AddComponent(e1, new TestComponent { Value = 1 });  // for Update
        world.AddComponent(e3, new TestComponent { Value = 3 });  // for Remove
        // e2 has no component — for Add

        using (var batch = world.BeginBatch<TestComponent>())
        {
            batch.Update(e1, new TestComponent { Value = 100 });
            batch.Add(e2, new TestComponent { Value = 200 });
            batch.Remove(e3);
            batch.Flush().Should().Be(3);
        }

        world.GetComponent<TestComponent>(e1).Value.Should().Be(100);
        world.GetComponent<TestComponent>(e2).Value.Should().Be(200);
        world.HasComponent<TestComponent>(e3).Should().BeFalse();
    }

    [Fact]
    public void EmptyBatch_FlushIsNoOp()
    {
        using var world = new NativeWorld();

        using var batch = world.BeginBatch<TestComponent>();
        batch.Flush().Should().Be(0);
    }

    [Fact]
    public void Dispose_AutoFlushesIfNotExplicitlyHandled()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new TestComponent { Value = 10 });

        using (var batch = world.BeginBatch<TestComponent>())
        {
            batch.Update(entity, new TestComponent { Value = 50 });
            // No explicit Flush — Dispose must auto-flush.
        }

        world.GetComponent<TestComponent>(entity).Value.Should().Be(50);
    }

    [Fact]
    public void DirectMutation_RejectedDuringActiveBatch()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new TestComponent { Value = 10 });

        using (var batch = world.BeginBatch<TestComponent>())
        {
            // Native side throws std::logic_error caught at C ABI boundary,
            // function silently no-ops. Value remains unchanged.
            world.AddComponent(entity, new TestComponent { Value = 999 });
            batch.Cancel();
        }

        world.GetComponent<TestComponent>(entity).Value.Should().Be(10);
    }

    [Fact]
    public void DoubleFlush_ThrowsInvalidOperation()
    {
        using var world = new NativeWorld();

        using var batch = world.BeginBatch<TestComponent>();
        batch.Flush();
        Action act = () => batch.Flush();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RecordAfterFlush_ThrowsInvalidOperation()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        using var batch = world.BeginBatch<TestComponent>();
        batch.Flush();
        Action act = () => batch.Update(entity, new TestComponent { Value = 42 });
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MultipleConcurrentBatches_DoNotInterfere()
    {
        using var world = new NativeWorld();
        EntityId e1 = world.CreateEntity();
        EntityId e2 = world.CreateEntity();
        world.AddComponent(e1, new TestComponent { Value = 1 });
        world.AddComponent(e2, new TestComponent { Value = 2 });

        using var batch1 = world.BeginBatch<TestComponent>();
        using var batch2 = world.BeginBatch<TestComponent>();

        batch1.Update(e1, new TestComponent { Value = 100 });
        batch2.Update(e2, new TestComponent { Value = 200 });

        batch1.Flush().Should().Be(1);
        batch2.Flush().Should().Be(1);

        world.GetComponent<TestComponent>(e1).Value.Should().Be(100);
        world.GetComponent<TestComponent>(e2).Value.Should().Be(200);
    }
}
