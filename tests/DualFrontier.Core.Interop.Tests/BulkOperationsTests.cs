using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class BulkOperationsTests
{
    private struct HealthComponent
    {
        public int Current;
        public int Maximum;
    }

    [Fact]
    public void AddComponents_bulk_adds_all_in_single_call()
    {
        using var world = new NativeWorld();
        const int count = 100;
        var entities = new EntityId[count];
        var components = new HealthComponent[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i, Maximum = 100 };
        }

        world.AddComponents<HealthComponent>(entities, components);

        world.GetComponentCount<HealthComponent>().Should().Be(count);
        for (int i = 0; i < count; i++)
        {
            world.GetComponent<HealthComponent>(entities[i]).Current.Should().Be(i);
        }
    }

    [Fact]
    public void GetComponents_bulk_reads_all_in_single_call()
    {
        using var world = new NativeWorld();
        const int count = 50;
        var entities = new EntityId[count];
        var components = new HealthComponent[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i * 2, Maximum = 100 };
        }

        world.AddComponents<HealthComponent>(entities, components);

        var output = new HealthComponent[count];
        int successful = world.GetComponents<HealthComponent>(entities, output);

        successful.Should().Be(count);
        for (int i = 0; i < count; i++)
        {
            output[i].Current.Should().Be(i * 2);
        }
    }

    [Fact]
    public void GetComponents_bulk_handles_mixed_alive_dead()
    {
        using var world = new NativeWorld();
        const int count = 10;
        var entities = new EntityId[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            world.AddComponent(entities[i], new HealthComponent { Current = i, Maximum = 100 });
        }

        // Destroy half + flush.
        for (int i = 0; i < count; i += 2)
        {
            world.DestroyEntity(entities[i]);
        }
        world.FlushDestroyedEntities();

        var output = new HealthComponent[count];
        int successful = world.GetComponents<HealthComponent>(entities, output);

        successful.Should().Be(count / 2);
    }

    [Fact]
    public void AddComponents_throws_on_length_mismatch()
    {
        using var world = new NativeWorld();
        var entities = new EntityId[5];
        var components = new HealthComponent[3];

        Action act = () => world.AddComponents<HealthComponent>(entities, components);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Mismatched lengths*");
    }

    [Fact]
    public void AddComponents_handles_empty_spans()
    {
        using var world = new NativeWorld();
        var entities = ReadOnlySpan<EntityId>.Empty;
        var components = ReadOnlySpan<HealthComponent>.Empty;

        world.AddComponents<HealthComponent>(entities, components);

        world.GetComponentCount<HealthComponent>().Should().Be(0);
    }

    [Fact]
    public void Bulk_then_span_iteration_consistent()
    {
        using var world = new NativeWorld();
        const int count = 20;
        var entities = new EntityId[count];
        var components = new HealthComponent[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i, Maximum = 100 };
        }

        world.AddComponents<HealthComponent>(entities, components);

        using var lease = world.AcquireSpan<HealthComponent>();
        lease.Count.Should().Be(count);

        int sum = 0;
        for (int i = 0; i < lease.Count; i++)
        {
            sum += lease.Span[i].Current;
        }
        sum.Should().Be(190);  // 0..19
    }

    [Fact]
    public void AddComponents_LargeBatch_DoesNotAllocateAfterWarmup()
    {
        using var world = new NativeWorld();
        const int count = 1000;  // > 256 threshold — exercises the ArrayPool path

        var entities = new EntityId[count];
        var components = new HealthComponent[count];
        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i, Maximum = 100 };
        }

        // Warmup — the first call may allocate while populating ArrayPool.
        world.AddComponents<HealthComponent>(entities, components);

        long allocBefore = GC.GetAllocatedBytesForCurrentThread();
        world.AddComponents<HealthComponent>(entities, components);
        long allocAfter = GC.GetAllocatedBytesForCurrentThread();
        long delta = allocAfter - allocBefore;

        // Tolerance for unrelated managed overhead. Pre-fix this delta was
        // ~8 KB (count * sizeof(ulong)) per call.
        delta.Should().BeLessThan(1024,
            because: "ArrayPool should eliminate per-call heap allocation for batches > 256.");
    }

    [Fact]
    public void GetComponents_LargeBatch_DoesNotAllocateAfterWarmup()
    {
        using var world = new NativeWorld();
        const int count = 1000;

        var entities = new EntityId[count];
        var components = new HealthComponent[count];
        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i, Maximum = 100 };
        }
        world.AddComponents<HealthComponent>(entities, components);

        var output = new HealthComponent[count];

        // Warmup.
        world.GetComponents<HealthComponent>(entities, output);

        long allocBefore = GC.GetAllocatedBytesForCurrentThread();
        world.GetComponents<HealthComponent>(entities, output);
        long allocAfter = GC.GetAllocatedBytesForCurrentThread();
        long delta = allocAfter - allocBefore;

        delta.Should().BeLessThan(1024,
            because: "ArrayPool should eliminate per-call heap allocation for batches > 256.");
    }
}
