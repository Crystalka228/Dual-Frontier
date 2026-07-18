using System;
using DualFrontier.Application.Modding;
using DualFrontier.Core.ECS;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// K10.2 Item 21 — Per-mod sub-scheduler instance ownership tests.
/// </summary>
public sealed class ModSubSchedulerTests
{
    private sealed class FakeSystem : SystemBase
    {
        public bool Disposed { get; private set; }
        public override void Update(float deltaSeconds) { }
        protected override void OnDispose() => Disposed = true;
    }

    [Fact]
    public void Construct_StoresModId()
    {
        var sub = new ModSubScheduler("test.mod");
        sub.ModId.Should().Be("test.mod");
        sub.SystemCount.Should().Be(0);
    }

    [Fact]
    public void AddSystem_GrowsList()
    {
        var sub = new ModSubScheduler("test.mod");
        sub.AddSystem(new FakeSystem());
        sub.AddSystem(new FakeSystem());
        sub.SystemCount.Should().Be(2);
        sub.Systems.Count.Should().Be(2);
    }

    [Fact]
    public void Teardown_DisposesAndClears()
    {
        var sub = new ModSubScheduler("test.mod");
        var s1 = new FakeSystem();
        var s2 = new FakeSystem();
        sub.AddSystem(s1);
        sub.AddSystem(s2);

        sub.Teardown();

        s1.Disposed.Should().BeTrue();
        s2.Disposed.Should().BeTrue();
        sub.SystemCount.Should().Be(0);
    }

    [Fact]
    public void Teardown_AbsorbsDisposeExceptions()
    {
        // Per MOD_OS §9.5.1 failure semantics: best-effort sequential
        var sub = new ModSubScheduler("test.mod");
        sub.AddSystem(new ThrowingSystem());

        Action act = () => sub.Teardown();
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_NullModId_Throws()
    {
        Action act = () => new ModSubScheduler(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    private sealed class ThrowingSystem : SystemBase
    {
        public override void Update(float deltaSeconds) { }
        protected override void OnDispose() => throw new InvalidOperationException("boom");
    }
}

public sealed class ModRegistrySubSchedulerTests
{
    [Fact]
    public void GetOrCreateSubScheduler_CreatesPerModInstance()
    {
        var registry = new TestableModRegistry();
        var a = registry.GetOrCreateSubScheduler("mod.a");
        var b = registry.GetOrCreateSubScheduler("mod.b");
        a.Should().NotBeSameAs(b);
        a.ModId.Should().Be("mod.a");
        b.ModId.Should().Be("mod.b");
    }

    [Fact]
    public void GetOrCreateSubScheduler_ReturnsSameForSameModId()
    {
        var registry = new TestableModRegistry();
        var first = registry.GetOrCreateSubScheduler("mod.a");
        var second = registry.GetOrCreateSubScheduler("mod.a");
        first.Should().BeSameAs(second);
    }

    [Fact]
    public void RemoveSubScheduler_InvokesTeardownAndDrops()
    {
        var registry = new TestableModRegistry();
        var sub = registry.GetOrCreateSubScheduler("mod.x");
        sub.AddSystem(new TestSystem());

        bool removed = registry.RemoveSubScheduler("mod.x");
        removed.Should().BeTrue();
        sub.SystemCount.Should().Be(0);
        registry.TryGetSubScheduler("mod.x").Should().BeNull();
    }

    [Fact]
    public void RemoveSubScheduler_AbsentModId_ReturnsFalse()
    {
        var registry = new TestableModRegistry();
        bool removed = registry.RemoveSubScheduler("nonexistent.mod");
        removed.Should().BeFalse();
    }

    // TestableModRegistry exposes the internal ModRegistry for these tests.
    // ModRegistry is internal sealed; this test class is in the same internal-visible
    // assembly per InternalsVisibleTo declarations.
    private sealed class TestableModRegistry
    {
        private readonly ModRegistry _inner = new();
        public ModSubScheduler GetOrCreateSubScheduler(string modId) => _inner.GetOrCreateSubScheduler(modId);
        public ModSubScheduler? TryGetSubScheduler(string modId) => _inner.TryGetSubScheduler(modId);
        public bool RemoveSubScheduler(string modId) => _inner.RemoveSubScheduler(modId);
    }

    private sealed class TestSystem : SystemBase
    {
        public override void Update(float deltaSeconds) { }
    }
}
