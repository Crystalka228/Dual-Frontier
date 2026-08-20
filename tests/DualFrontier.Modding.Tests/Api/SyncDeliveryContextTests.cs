using System;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Api;

/// <summary>Event type owned by this test file alone, so nothing else can perturb the counts.</summary>
public sealed record SyncDeliveryProbeEvent(int Value) : IEvent;

/// <summary>
/// W3 defect regression (D-1). Before W3 a mod event published from inside a system Tick was
/// silently DROPPED: <c>RestrictedModApi.Subscribe</c> wrapped every handler in
/// <c>SystemExecutionContext.PushContext</c>, which throws on a nested push, and
/// <c>DomainEventBus.DeliverSync</c> swallows a mod-origin subscriber fault and continues. The
/// publisher saw success, the subscriber never ran, and nothing was reported anywhere.
///
/// <para>
/// The wave gate found it because W3 is the first thing to publish a mod event from one system
/// and consume it in another. These tests pin both halves of the fix so it cannot regress.
/// </para>
/// </summary>
[Collection("GameLoopSerial")]
public sealed class SyncDeliveryContextTests
{
    private static RestrictedModApi BuildApi(ModRegistry registry, GameServices services, string modId = "probe.mod")
    {
        string fqn = typeof(SyncDeliveryProbeEvent).FullName!;
        var manifest = new ModManifest
        {
            Id = modId,
            Capabilities = ManifestCapabilities.Parse(
                new[] { "kernel.publish:" + fqn, "kernel.subscribe:" + fqn }, null),
        };
        var api = new RestrictedModApi(
            modId, manifest, registry, new ModContractStore(), services,
            new KernelCapabilityRegistry());
        registry.RegisterRestrictedModApi(modId, api);
        return api;
    }

    private static SystemExecutionContext Ctx(string name, NativeWorld world, string modId = "probe.mod")
        => new(name, SystemOrigin.Mod, modId, new NullModFaultSink(), world);

    [Fact]
    public void PublishFromInsideASystemTick_ReachesASubscriberThatCapturedItsOwnContext()
    {
        using var world = new NativeWorld();
        var registry = new ModRegistry();
        var services = new GameServices();
        RestrictedModApi api = BuildApi(registry, services);

        int handled = 0;

        // Subscribe the way a mod system does: from inside its own Initialize, with ITS context
        // active, so RestrictedModApi captures a context to restore later.
        SystemExecutionContext.PushContext(Ctx("Subscriber", world));
        try { api.Subscribe<SyncDeliveryProbeEvent>(_ => handled++); }
        finally { SystemExecutionContext.PopContext(); }

        // Baseline: no context active (composition root, deferred flush, a test).
        api.Publish(new SyncDeliveryProbeEvent(1));
        handled.Should().Be(1, "delivery works when no execution context is active");

        // The case that was broken: a SIBLING system publishes from inside its own Tick, so a
        // DIFFERENT context is already active on this thread.
        SystemExecutionContext.PushContext(Ctx("Publisher", world));
        try { api.Publish(new SyncDeliveryProbeEvent(2)); }
        finally { SystemExecutionContext.PopContext(); }

        handled.Should().Be(2,
            "a synchronous publish from inside a system tick must still reach the subscriber; " +
            "before the D-1 fix the nested PushContext threw and DeliverSync swallowed it");
    }

    [Fact]
    public void Subscriber_RunsUnderItsOwnContext_NotThePublishers()
    {
        // PR #49 Codex review (P2). The first draft of the D-1 fix ran a synchronous subscriber
        // under whoever happened to publish. Both mods share a bus here but are DIFFERENT mods --
        // the original regression tests used one id for both and could not have seen this.
        using var world = new NativeWorld();
        var registry = new ModRegistry();
        var services = new GameServices();

        RestrictedModApi subscriberApi = BuildApi(registry, services, "subscriber.mod");
        RestrictedModApi publisherApi = BuildApi(registry, services, "publisher.mod");

        SystemExecutionContext subscriberCtx = Ctx("Subscriber", world, "subscriber.mod");
        SystemExecutionContext publisherCtx = Ctx("Publisher", world, "publisher.mod");

        SystemExecutionContext? seenDuringDelivery = null;

        SystemExecutionContext.PushContext(subscriberCtx);
        try { subscriberApi.Subscribe<SyncDeliveryProbeEvent>(_ => seenDuringDelivery = SystemExecutionContext.Current); }
        finally { SystemExecutionContext.PopContext(); }

        SystemExecutionContext.PushContext(publisherCtx);
        try { publisherApi.Publish(new SyncDeliveryProbeEvent(1)); }
        finally { SystemExecutionContext.PopContext(); }

        seenDuringDelivery.Should().BeSameAs(subscriberCtx,
            "a handler must run under the context it captured at Subscribe time; running it under " +
            "the publisher's would misattribute anything the handler does -- a nested Subscribe " +
            "would capture the WRONG mod, and fault routing could quarantine it");
    }

    [Fact]
    public void PublishInsideATick_LeavesThePublishersContextIntact()
    {
        using var world = new NativeWorld();
        var registry = new ModRegistry();
        RestrictedModApi api = BuildApi(registry, new GameServices());

        SystemExecutionContext.PushContext(Ctx("Subscriber", world));
        try { api.Subscribe<SyncDeliveryProbeEvent>(_ => { }); }
        finally { SystemExecutionContext.PopContext(); }

        SystemExecutionContext publisher = Ctx("Publisher", world);
        SystemExecutionContext.PushContext(publisher);
        try
        {
            api.Publish(new SyncDeliveryProbeEvent(3));

            SystemExecutionContext.Current.Should().BeSameAs(publisher,
                "delivery must not leave the publisher's context popped or replaced -- the rest of " +
                "the publishing Tick still needs it");
        }
        finally { SystemExecutionContext.PopContext(); }
    }
}
