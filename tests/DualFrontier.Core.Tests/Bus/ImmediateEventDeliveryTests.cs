using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Bus;

/// <summary>
/// Verifies that <c>[Immediate]</c>-marked events are delivered synchronously
/// from <c>Publish</c> and never end up in the deferred queue. Documented
/// behaviour: «крайне редкий режим — событие прерывает текущую фазу,
/// немедленно доставляется всем подписчикам».
/// </summary>
public sealed class ImmediateEventDeliveryTests : IDisposable
{
    public ImmediateEventDeliveryTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
    }

    [Fact]
    public void Immediate_Publish_DeliveredInsidePublishCall()
    {
        var services = new GameServices();
        int count = 0;
        services.Combat.Subscribe<ImmediateTestEvent>(_ => count++);

        services.Combat.Publish(new ImmediateTestEvent());

        count.Should().Be(1, "[Immediate] events must run before Publish returns");
    }

    [Fact]
    public void Immediate_Publish_DoesNotEnterDeferredQueue()
    {
        var services = new GameServices();
        var observed = new List<string>();
        services.Combat.Subscribe<ImmediateTestEvent>(e => observed.Add(e.Tag));

        services.Combat.Publish(new ImmediateTestEvent { Tag = "first" });
        observed.Should().BeEquivalentTo(new[] { "first" });

        ((IDeferredFlush)services).FlushDeferred();

        observed.Should().BeEquivalentTo(new[] { "first" },
            o => o, "FlushDeferred must be a no-op for [Immediate] events that already ran during Publish");
    }

    [Immediate]
    internal sealed record ImmediateTestEvent : IEvent
    {
        public string Tag { get; init; } = "";
    }
}
