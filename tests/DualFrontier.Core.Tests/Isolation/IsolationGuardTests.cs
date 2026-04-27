using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Isolation;

public sealed class IsolationGuardTests : IDisposable
{
    public IsolationGuardTests()
    {
        // Ensure a clean slot on the calling test thread — previous test runs
        // in other fixtures may not have popped their context.
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void GetComponent_Declared_ReturnsComponent()
    {
        var world = new World();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestHealthComponent { Value = 42 });
        var ctx = BuildContext(world, "CoreSystem", reads: new[] { typeof(TestHealthComponent) });
        SystemExecutionContext.PushContext(ctx);

        var comp = ctx.GetComponent<TestHealthComponent>(id);

        comp.Should().NotBeNull();
        comp.Value.Should().Be(42);
    }

    [Fact]
    public void GetComponent_Undeclared_Throws_ForCore()
    {
        var world = new World();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestHealthComponent { Value = 1 });
        var ctx = BuildContext(world, "CoreSystem", reads: new[] { typeof(TestPositionComponent) });
        SystemExecutionContext.PushContext(ctx);

        Action act = () => ctx.GetComponent<TestHealthComponent>(id);

        act.Should().Throw<IsolationViolationException>()
            .Which.Message.Should().ContainAll(
                nameof(TestHealthComponent),
                IsolationDiagnostics.UndeclaredAccessToken,
                IsolationDiagnostics.HintToken);
    }

    [Fact]
    public void GetComponent_WriteGrantsRead()
    {
        var world = new World();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestHealthComponent { Value = 7 });
        var ctx = BuildContext(world, "CoreSystem", writes: new[] { typeof(TestHealthComponent) });
        SystemExecutionContext.PushContext(ctx);

        Action act = () => ctx.GetComponent<TestHealthComponent>(id);

        act.Should().NotThrow();
    }

    [Fact]
    public void SetComponent_Undeclared_Throws()
    {
        var world = new World();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestHealthComponent { Value = 1 });
        var ctx = BuildContext(world, "CoreSystem", reads: new[] { typeof(TestHealthComponent) });
        SystemExecutionContext.PushContext(ctx);

        Action act = () => ctx.SetComponent(id, new TestHealthComponent { Value = 99 });

        act.Should().Throw<IsolationViolationException>()
            .Which.Message.Should().ContainAll(
                nameof(TestHealthComponent),
                IsolationDiagnostics.WriteVerbToken,
                IsolationDiagnostics.HintToken);
    }

    [Fact]
    public void GetSystem_AlwaysThrows()
    {
        var world = new World();
        var ctx = BuildContext(world, "CoreSystem");
        SystemExecutionContext.PushContext(ctx);

        Action act = () => ctx.GetSystem<TestSystemStub>();

        act.Should().Throw<IsolationViolationException>()
            .Which.Message.Should().Contain(IsolationDiagnostics.DirectSystemAccessToken);
    }

    [Fact]
    public void ModOrigin_Violation_ReportsToSink()
    {
        var world = new World();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestHealthComponent { Value = 1 });
        var sink = new RecordingFaultSink();
        var ctx = BuildContext(
            world,
            "ModSystem",
            reads: new[] { typeof(TestPositionComponent) },
            origin: SystemOrigin.Mod,
            modId: "test.mod.id",
            sink: sink);
        SystemExecutionContext.PushContext(ctx);

        Action act = () => ctx.GetComponent<TestHealthComponent>(id);

        act.Should().Throw<IsolationViolationException>();
        sink.Faults.Should().HaveCount(1);
        sink.Faults[0].modId.Should().Be("test.mod.id");
        sink.Faults[0].message.Should().ContainAll(
            nameof(TestHealthComponent),
            IsolationDiagnostics.UndeclaredAccessToken);
    }

    [Fact]
    public void Push_TwiceWithoutPop_Throws()
    {
        var world = new World();
        var ctx1 = BuildContext(world, "First");
        var ctx2 = BuildContext(world, "Second");
        SystemExecutionContext.PushContext(ctx1);

        Action act = () => SystemExecutionContext.PushContext(ctx2);

        act.Should().Throw<InvalidOperationException>()
            .Which.Message.Should().Contain("already set");
    }

    [Fact]
    public void PopContext_ClearsCurrent()
    {
        var world = new World();
        var ctx = BuildContext(world, "CoreSystem");
        SystemExecutionContext.PushContext(ctx);
        SystemExecutionContext.Current.Should().NotBeNull();

        SystemExecutionContext.PopContext();

        SystemExecutionContext.Current.Should().BeNull();
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static SystemExecutionContext BuildContext(
        World world,
        string name,
        Type[]? reads = null,
        Type[]? writes = null,
        SystemOrigin origin = SystemOrigin.Core,
        string? modId = null,
        IModFaultSink? sink = null)
    {
        return new SystemExecutionContext(
            world,
            name,
            reads ?? Array.Empty<Type>(),
            writes ?? Array.Empty<Type>(),
            new[] { "TestBus" },
            origin,
            modId,
            sink ?? new RecordingFaultSink());
    }

    // ── Test fixture types ──────────────────────────────────────────────────

    internal sealed class TestHealthComponent : IComponent
    {
        public int Value;
    }

    internal sealed class TestPositionComponent : IComponent
    {
    }

    internal sealed class TestSystemStub : SystemBase
    {
        public override void Update(float delta) { }
    }

    internal sealed class RecordingFaultSink : IModFaultSink
    {
        public List<(string modId, string message)> Faults { get; } = new();
        public void ReportFault(string modId, string message) => Faults.Add((modId, message));
    }
}
