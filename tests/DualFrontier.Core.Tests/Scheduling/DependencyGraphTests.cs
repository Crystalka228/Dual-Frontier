using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

public sealed class DependencyGraphTests
{
    [Fact]
    public void Build_EmptyGraph_ReturnsNoPhases()
    {
        var graph = new DependencyGraph();
        graph.Build();
        graph.GetPhases().Should().BeEmpty();
    }

    [Fact]
    public void Build_IndependentSystems_SinglePhase()
    {
        var graph = new DependencyGraph();
        var a = new WriterASystem();
        var c = new WriterCSystem();
        graph.AddSystem(a);
        graph.AddSystem(c);

        graph.Build();
        var phases = graph.GetPhases();

        phases.Should().HaveCount(1);
        phases[0].Systems.Should().BeEquivalentTo(new SystemBase[] { a, c });
    }

    [Fact]
    public void Build_LinearChain_OrderedPhases()
    {
        var graph = new DependencyGraph();
        var writerA = new WriterASystem();
        var readAWriteB = new ReadAWriteBSystem();
        var readerB = new ReaderBSystem();
        graph.AddSystem(writerA);
        graph.AddSystem(readAWriteB);
        graph.AddSystem(readerB);

        graph.Build();
        var phases = graph.GetPhases();

        phases.Should().HaveCount(3);
        phases[0].Systems.Should().ContainSingle().Which.Should().Be(writerA);
        phases[1].Systems.Should().ContainSingle().Which.Should().Be(readAWriteB);
        phases[2].Systems.Should().ContainSingle().Which.Should().Be(readerB);
    }

    [Fact]
    public void Build_ParallelSafeInSamePhase_ThreeSystems()
    {
        var graph = new DependencyGraph();
        var writerA = new WriterASystem();
        var writerC = new WriterCSystem();
        var readAWriteB = new ReadAWriteBSystem();
        graph.AddSystem(writerA);
        graph.AddSystem(writerC);
        graph.AddSystem(readAWriteB);

        graph.Build();
        var phases = graph.GetPhases();

        phases.Should().HaveCount(2);
        phases[0].Systems.Should().BeEquivalentTo(new SystemBase[] { writerA, writerC });
        phases[1].Systems.Should().ContainSingle().Which.Should().Be(readAWriteB);
    }

    [Fact]
    public void Build_WriteWriteConflict_Throws()
    {
        var graph = new DependencyGraph();
        graph.AddSystem(new WriterASystem());
        graph.AddSystem(new WriterAConflictSystem());

        Action act = () => graph.Build();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Write conflict*")
            .Which.Message.Should().ContainAll("WriterASystem", "WriterAConflictSystem");
    }

    [Fact]
    public void Build_Cycle_Throws()
    {
        var graph = new DependencyGraph();
        graph.AddSystem(new CycleASystem());
        graph.AddSystem(new CycleBSystem());

        Action act = () => graph.Build();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cyclic dependency*");
    }

    [Fact]
    public void AddSystem_NoAttribute_Throws()
    {
        var graph = new DependencyGraph();

        Action act = () => graph.AddSystem(new UnattributedSystem());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*no [SystemAccess]*");
    }

    [Fact]
    public void AddSystem_Duplicate_Throws()
    {
        var graph = new DependencyGraph();
        graph.AddSystem(new WriterASystem());

        Action act = () => graph.AddSystem(new WriterASystem());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*duplicate*");
    }

    [Fact]
    public void GetPhases_BeforeBuild_Throws()
    {
        var graph = new DependencyGraph();

        Action act = () => graph.GetPhases();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Build*");
    }

    [Fact]
    public void Reset_ClearsState_AllowsRebuild()
    {
        var graph = new DependencyGraph();
        graph.AddSystem(new WriterASystem());
        graph.AddSystem(new ReadAWriteBSystem());
        graph.Build();

        graph.Reset();

        graph.AddSystem(new WriterCSystem());
        graph.Build();
        var phases = graph.GetPhases();

        phases.Should().HaveCount(1);
        phases[0].Systems.Should().ContainSingle().Which.Should().BeOfType<WriterCSystem>();
    }

    // ── Test components ─────────────────────────────────────────────────────

    internal sealed class CompA : IComponent { }
    internal sealed class CompB : IComponent { }
    internal sealed class CompC : IComponent { }

    // ── Test systems ────────────────────────────────────────────────────────

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(CompA) }, bus: "TestBus")]
    internal sealed class WriterASystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new[] { typeof(CompA) }, writes: new[] { typeof(CompB) }, bus: "TestBus")]
    internal sealed class ReadAWriteBSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new[] { typeof(CompB) }, writes: new Type[0], bus: "TestBus")]
    internal sealed class ReaderBSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(CompC) }, bus: "TestBus")]
    internal sealed class WriterCSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(CompA) }, bus: "TestBus")]
    internal sealed class WriterAConflictSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new[] { typeof(CompB) }, writes: new[] { typeof(CompA) }, bus: "TestBus")]
    internal sealed class CycleASystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new[] { typeof(CompA) }, writes: new[] { typeof(CompB) }, bus: "TestBus")]
    internal sealed class CycleBSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    internal sealed class UnattributedSystem : SystemBase
    {
        public override void Update(float delta) { }
    }
}
