using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Bus;

/// <summary>
/// F60A_TICK_PATH — the bus-side half of the F-60(a) defect class: Type-keyed
/// containers that outlive the mod whose types key them.
///
/// <para>
/// <c>Unsubscribe</c> used to remove the SUBSCRIPTION but never the <see cref="Type"/>
/// KEY, so a fully-unsubscribed event type left its key and an empty list in
/// <c>_handlers</c> for the session. For a mod that declares its event types in its
/// own regular (collectible) assembly, that key roots the mod's ALC exactly as the
/// tick-rate cache did. The key now dies with its last subscriber.
/// </para>
///
/// <para>
/// These assert BEHAVIOURALLY. <c>_handlers</c> is private and the bus exposes no
/// count seam, and adding a reflection probe into private state would pin the
/// implementation rather than the contract. What is observable — and what actually
/// matters — is that eviction does not break the re-subscribe path, does not lose a
/// subscription to a concurrent race, and leaves delivery working.
/// </para>
/// </summary>
public sealed class HandlerKeyEvictionTests
{
    [Fact]
    public void SubscribeUnsubscribeResubscribe_StillDelivers()
    {
        var bus = new DomainEventBus();
        var seen = new List<int>();
        Action<Ping> handler = e => seen.Add(e.Value);

        bus.Subscribe(handler);
        bus.Unsubscribe(handler);      // list empties -> the Type key is evicted here
        bus.Subscribe(handler);        // must re-create the key, not resurrect an orphan

        bus.Publish(new Ping(7));

        seen.Should().Equal(new[] { 7 },
            "evicting the key on the last unsubscribe must leave the type re-subscribable; " +
            "a subscription added to a list that had been detached from the map would be " +
            "silently unreachable and this would see nothing");
    }

    [Fact]
    public void UnsubscribeOfOneOfTwo_KeepsDeliveringToTheOther()
    {
        var bus = new DomainEventBus();
        int a = 0, b = 0;
        Action<Ping> first = _ => a++;
        Action<Ping> second = _ => b++;

        bus.Subscribe(first);
        bus.Subscribe(second);
        bus.Unsubscribe(first);

        bus.Publish(new Ping(1));

        a.Should().Be(0, "the removed handler must not run");
        b.Should().Be(1,
            "the key must survive while ANY subscriber remains -- eviction is on the LAST " +
            "unsubscribe, not on any unsubscribe");
    }

    [Fact]
    public void PublishWithNoSubscribers_AfterFullUnsubscribe_IsSilentNotThrowing()
    {
        var bus = new DomainEventBus();
        Action<Ping> handler = _ => { };

        bus.Subscribe(handler);
        bus.Unsubscribe(handler);

        Action act = () => bus.Publish(new Ping(1));

        act.Should().NotThrow(
            "an evicted key is indistinguishable from a type nobody ever subscribed to, and " +
            "publishing to no subscribers has always been a silent no-op");
    }

    [Fact]
    public void RepeatedUnsubscribeOfAnAlreadyEvictedType_IsANoOp()
    {
        var bus = new DomainEventBus();
        Action<Ping> handler = _ => { };
        bus.Subscribe(handler);
        bus.Unsubscribe(handler);

        Action act = () => bus.Unsubscribe(handler);

        act.Should().NotThrow(
            "the second call finds no key at all and must return quietly, exactly as an " +
            "unsubscribe of a never-subscribed handler does");
    }

    [Fact]
    public void ConcurrentSubscribeAndUnsubscribe_NeverLosesTheSurvivingSubscription()
    {
        // The race eviction opens: Subscribe fetches the list, Unsubscribe empties it
        // and detaches it from the map, and Subscribe then adds to a list nothing will
        // ever read. Subscribe re-checks list identity under the lock to close it; this
        // hammers the window.
        for (int iteration = 0; iteration < 200; iteration++)
        {
            var bus = new DomainEventBus();
            int delivered = 0;
            Action<Ping> transient = _ => { };
            Action<Ping> survivor = _ => delivered++;

            bus.Subscribe(transient);

            Parallel.Invoke(
                () => bus.Unsubscribe(transient),
                () => bus.Subscribe(survivor));

            bus.Publish(new Ping(1));

            delivered.Should().Be(1,
                "the surviving subscription must be reachable on iteration {0} no matter how " +
                "the eviction and the subscribe interleaved", iteration);
        }
    }

    private sealed record Ping(int Value) : IEvent;
}
