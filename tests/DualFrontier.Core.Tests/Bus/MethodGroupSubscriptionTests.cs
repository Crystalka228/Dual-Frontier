using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Bus;

/// <summary>
/// Regression tests for F19: the bus must match handlers by delegate <em>value</em> equality
/// (Target + Method), not reference identity. A method-group conversion (passing <c>Handler</c>)
/// allocates a fresh delegate instance on every conversion, so a <c>ReferenceEquals</c> match would
/// never fire on <c>Unsubscribe</c>, and duplicate <c>Subscribe</c> calls would accumulate — the
/// exact failure that lets handlers run N+1 times after a scheduler Rebuild re-initialises the same
/// system instances.
/// </summary>
public sealed class MethodGroupSubscriptionTests : IDisposable
{
    private int _calls;

    public MethodGroupSubscriptionTests() => SystemExecutionContext.PopContext();

    public void Dispose() => SystemExecutionContext.PopContext();

    private void Handler(MethodGroupTestEvent e) => _calls++;

    [Fact]
    public void Unsubscribe_ByMethodGroup_RemovesHandler_AcrossDistinctDelegateInstances()
    {
        var services = new GameServices();
        services.Combat.Subscribe<MethodGroupTestEvent>(Handler);
        // A separate method-group conversion — a DIFFERENT delegate instance for the same method.
        // Under the old ReferenceEquals match this Unsubscribe was a silent no-op.
        services.Combat.Unsubscribe<MethodGroupTestEvent>(Handler);

        services.Combat.Publish(new MethodGroupTestEvent());

        _calls.Should().Be(0, "a value-equal method-group delegate must unsubscribe (F19)");
    }

    [Fact]
    public void Subscribe_SameMethodGroupTwice_IsDeduplicated()
    {
        var services = new GameServices();
        services.Combat.Subscribe<MethodGroupTestEvent>(Handler);
        services.Combat.Subscribe<MethodGroupTestEvent>(Handler); // duplicate — must be ignored

        services.Combat.Publish(new MethodGroupTestEvent());

        _calls.Should().Be(1, "duplicate method-group subscriptions must dedup, not accumulate (F19)");
    }

    [Immediate]
    internal sealed record MethodGroupTestEvent : IEvent;
}
