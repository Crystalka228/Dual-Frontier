using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K8.2 v2 Phase 2.B.6 — round-trip and Path-walking semantics for the
/// post-conversion <see cref="MovementComponent"/> struct + NativeComposite +
/// PathStepIndex shape. Walks Path via index advancement (not RemoveAt)
/// because <see cref="NativeComposite{T}.RemoveAt"/> is swap-with-last and
/// would break FIFO order.
/// </summary>
public sealed class MovementComponentTests
{
    [Fact]
    public void Default_NoTarget_PathInvalid_StepIndexZero()
    {
        MovementComponent component = default;
        component.HasTarget.Should().BeFalse();
        component.Path.IsValid.Should().BeFalse();
        component.PathStepIndex.Should().Be(0);
    }

    [Fact]
    public void HasTarget_FieldSet_FlipsTrue()
    {
        var component = new MovementComponent
        {
            Target = new GridVector(5, 7),
            HasTarget = true,
        };
        component.HasTarget.Should().BeTrue();
        component.Target.X.Should().Be(5);
        component.Target.Y.Should().Be(7);
    }

    [Fact]
    public void Path_PopulatedAndWalkedViaIndex_PreservesFIFOOrder()
    {
        using var world = new NativeWorld();
        EntityId entity = new EntityId(1, 0);

        var component = new MovementComponent
        {
            Path = world.CreateComposite<GridVector>(),
        };

        var waypoints = new[]
        {
            new GridVector(1, 0),
            new GridVector(2, 0),
            new GridVector(3, 0),
        };
        foreach (GridVector wp in waypoints)
            component.Path.Add(entity, wp);

        component.Path.CountFor(entity).Should().Be(3);

        // Walk via index advancement — preserves insertion order.
        for (int i = 0; i < waypoints.Length; i++)
        {
            component.Path.TryGetAt(entity, component.PathStepIndex, out GridVector next).Should().BeTrue();
            next.X.Should().Be(waypoints[i].X);
            next.Y.Should().Be(waypoints[i].Y);
            component.PathStepIndex++;
        }

        component.PathStepIndex.Should().Be(waypoints.Length);
    }

    [Fact]
    public void Path_ClearForEntity_DropsAllSteps()
    {
        using var world = new NativeWorld();
        EntityId entity = new EntityId(1, 0);

        var component = new MovementComponent
        {
            Path = world.CreateComposite<GridVector>(),
        };
        component.Path.Add(entity, new GridVector(0, 0));
        component.Path.Add(entity, new GridVector(1, 0));
        component.Path.CountFor(entity).Should().Be(2);

        component.Path.ClearFor(entity).Should().BeTrue();
        component.Path.CountFor(entity).Should().Be(0);
    }

    [Fact]
    public void DistinctEntities_PathStorageIsolated_OnSameComposite()
    {
        // K8.2 v2 design: in production each MovementComponent gets its own
        // composite_id via CreateComposite, so this isolation is automatic.
        // Test verifies the underlying NativeComposite parent-keying still
        // discriminates entities correctly even when sharing one composite_id
        // (the legacy API surface kept for backward compatibility).
        using var world = new NativeWorld();
        EntityId entityA = new EntityId(1, 0);
        EntityId entityB = new EntityId(2, 0);

        NativeComposite<GridVector> sharedPath = world.CreateComposite<GridVector>();
        sharedPath.Add(entityA, new GridVector(10, 10));
        sharedPath.Add(entityB, new GridVector(20, 20));

        sharedPath.CountFor(entityA).Should().Be(1);
        sharedPath.CountFor(entityB).Should().Be(1);
        sharedPath.TryGetAt(entityA, 0, out GridVector a).Should().BeTrue();
        a.X.Should().Be(10);
        sharedPath.TryGetAt(entityB, 0, out GridVector b).Should().BeTrue();
        b.X.Should().Be(20);
    }
}
