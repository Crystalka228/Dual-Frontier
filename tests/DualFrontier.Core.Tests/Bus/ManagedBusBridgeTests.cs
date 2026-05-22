using System;
using System.Runtime.InteropServices;
using DualFrontier.Application.Bus;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Tests.Scheduling.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Bus;

// К10.2 Item 27 — Managed bus facade + C ABI bridge tests.
//
// Verifies that managed-side BusFacade routes through native bus when the
// K10.2 default `UseNativeBusForDispatch` flag is flipped to true.
//
// К-L9 uniformity contract: IModApi surface preserved by facade (mod authors
// don't see native bus directly; tier dispatch transparent based on event's
// [EventTier] attribute).
public sealed class ManagedBusBridgeTests : IDisposable
{
    [EventTier(BusTier.Fast)]
    private struct FastTestEvent : IEvent
    {
        public int Value;
    }

    [EventTier(BusTier.Normal)]
    private struct NormalTestEvent : IEvent
    {
#pragma warning disable CS0649
        public int Value;
#pragma warning restore CS0649
    }

    [EventTier(BusTier.Background)]
    private struct BackgroundTestEvent : IEvent
    {
        public int Value;
    }

    private struct DefaultTierTestEvent : IEvent
    {
#pragma warning disable CS0649
        public int Value;
#pragma warning restore CS0649
    }

    private readonly ManagedBusBridge _bridge;
    private readonly BusFacade _facade;

    public ManagedBusBridgeTests()
    {
        _bridge = new ManagedBusBridge();
        _facade = new BusFacade(_bridge);
        _bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();
    }

    public void Dispose()
    {
        _bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();
    }

    [Fact]
    public void BusFacade_AssignsStableTypeIds()
    {
        uint id1 = _facade.GetOrAssignTypeId<FastTestEvent>();
        uint id2 = _facade.GetOrAssignTypeId<FastTestEvent>();
        id1.Should().Be(id2, "type id assignment must be stable across calls");

        uint differentId = _facade.GetOrAssignTypeId<NormalTestEvent>();
        differentId.Should().NotBe(id1, "different event types must get distinct type ids");
    }

    [Fact]
    public void BusFacade_ReadsTierFromAttribute()
    {
        _facade.GetTier<FastTestEvent>().Should().Be(BusTier.Fast);
        _facade.GetTier<NormalTestEvent>().Should().Be(BusTier.Normal);
    }

    [Fact]
    public void BusFacade_DefaultsTierToNormal_WhenAttributeAbsent()
    {
        // S-LOCK-4 backward compatibility: events без [EventTier] default к Normal
        _facade.GetTier<DefaultTierTestEvent>().Should().Be(BusTier.Normal);
    }

    [Fact]
    public void BusFacade_RegisterEventType_RegistersWithNativeRegistry()
    {
        int before = EventTypeRegistryInterop.Count;
        bool ok = _facade.RegisterEventType<FastTestEvent>();
        ok.Should().BeTrue();
        EventTypeRegistryInterop.Count.Should().Be(before + 1);

        // Verify tier reads back
        uint typeId = _facade.GetOrAssignTypeId<FastTestEvent>();
        EventTypeRegistryInterop.GetTier(typeId).Should().Be(BusTier.Fast);
    }

    [Fact]
    public void BusFacade_PublishDisabledByDefault_NoNativeDispatch()
    {
        // К10.2 managed-facade-preserved: UseNativeBusForDispatch defaults к false
        _facade.UseNativeBusForDispatch.Should().BeFalse();

        var evt = new FastTestEvent { Value = 42 };
        int invoked = _facade.Publish(evt);
        invoked.Should().Be(0, "publish с UseNativeBusForDispatch=false is no-op");
    }

    [Fact]
    public void BusFacade_PublishEnabled_RoutesThroughNativeBus()
    {
        // К10.2 verification mode (К-L15 sovereign authority candidate)
        _facade.UseNativeBusForDispatch = true;

        // Register the event type so native registry knows the tier
        _facade.RegisterEventType<FastTestEvent>();
        uint typeId = _facade.GetOrAssignTypeId<FastTestEvent>();

        // Subscribe via bridge directly (no managed adapter needed для this verification —
        // we check native dispatch path is invoked, не the reverse-callback marshalling
        // which is exercised by selftest's scenario_bus_fast_publish_subscribe_roundtrip)
        IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(s_fastSubscriberDelegate);
        var handle = GCHandle.Alloc(s_fastSubscriberDelegate);
        ulong sid = _bridge.SubscribeFast(typeId, /*mod_id=*/100u, callbackPtr, handle);
        sid.Should().NotBe(0u);

        s_fastSubscriberInvocations = 0;
        var evt = new FastTestEvent { Value = 99 };
        int invoked = _facade.Publish(evt);
        invoked.Should().Be(1, "1 native subscriber invoked");
        s_fastSubscriberInvocations.Should().Be(1, "managed callback fired via reverse P/Invoke");

        _bridge.Unsubscribe(sid);
    }

    // Reverse-P/Invoke callback per К10.1 Item 15 ABI constraints —
    // static method, blittable args, no managed exceptions across boundary.
    private static int s_fastSubscriberInvocations;
    private static readonly FastSubscriberDelegate s_fastSubscriberDelegate = FastSubscriberCallback;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FastSubscriberDelegate(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData);

    private static void FastSubscriberCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try
        {
            s_fastSubscriberInvocations++;
        }
        catch
        {
            // К-L15 fast tier contract: no managed exceptions across boundary
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // Bug #1 (A'.7.x γ1) — BusFacade.Publish<T>(T, uint coalesceKey) overload
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public void BusFacade_PublishBackground_WithDistinctCoalesceKeys_ProducesDistinctDispatches()
    {
        _facade.UseNativeBusForDispatch = true;

        // Background tier registration requires a coalesce_fn (К-L15 / Q-N-34).
        IntPtr coalesceFn = Marshal.GetFunctionPointerForDelegate(s_bgCoalesceDelegate);
        _facade.RegisterEventType<BackgroundTestEvent>(coalesceFn).Should().BeTrue();
        uint typeId = _facade.GetOrAssignTypeId<BackgroundTestEvent>();

        IntPtr cb = Marshal.GetFunctionPointerForDelegate(s_bgSubscriberDelegate);
        var handle = GCHandle.Alloc(s_bgSubscriberDelegate);
        try
        {
            ulong sid = _bridge.SubscribeBackground(typeId, modId: 100u, cb, handle);
            sid.Should().NotBe(0u);

            s_bgSubscriberInvocations = 0;
            const int N = 100;
            for (uint i = 1; i <= N; i++)
            {
                int rc = _facade.Publish(new BackgroundTestEvent { Value = (int)i }, coalesceKey: i);
                rc.Should().Be(1, "Background publish must queue the event");
            }

            int dispatched = BackgroundBusTestDriver.DispatchIdleSlot(0);
            dispatched.Should().Be(N,
                "{0} distinct coalesce keys must dispatch as {0} separate events", N);
            s_bgSubscriberInvocations.Should().Be(N,
                "managed subscriber must observe {0} callbacks (one per distinct key)", N);

            _bridge.Unsubscribe(sid);
        }
        finally
        {
            if (handle.IsAllocated) handle.Free();
        }
    }

    [Fact]
    public void BusFacade_PublishBackground_WithSameCoalesceKey_CollapsesToOneDispatch()
    {
        _facade.UseNativeBusForDispatch = true;

        IntPtr coalesceFn = Marshal.GetFunctionPointerForDelegate(s_bgCoalesceDelegate);
        _facade.RegisterEventType<BackgroundTestEvent>(coalesceFn).Should().BeTrue();
        uint typeId = _facade.GetOrAssignTypeId<BackgroundTestEvent>();

        IntPtr cb = Marshal.GetFunctionPointerForDelegate(s_bgSubscriberDelegate);
        var handle = GCHandle.Alloc(s_bgSubscriberDelegate);
        try
        {
            ulong sid = _bridge.SubscribeBackground(typeId, modId: 100u, cb, handle);
            sid.Should().NotBe(0u);

            s_bgSubscriberInvocations = 0;
            const int N = 100;
            for (int i = 0; i < N; i++)
            {
                _facade.Publish(new BackgroundTestEvent { Value = i }, coalesceKey: 7u);
            }

            int dispatched = BackgroundBusTestDriver.DispatchIdleSlot(0);
            dispatched.Should().Be(1,
                "{0} publishes sharing coalesceKey=7 must collapse to a single dispatch", N);
            s_bgSubscriberInvocations.Should().Be(1);

            _bridge.Unsubscribe(sid);
        }
        finally
        {
            if (handle.IsAllocated) handle.Free();
        }
    }

    [Fact]
    public void BusFacade_PublishWithCoalesceKey_OnFastTier_ThrowsInvalidOperation()
    {
        _facade.UseNativeBusForDispatch = true;

        Action act = () => _facade.Publish(new FastTestEvent { Value = 42 }, coalesceKey: 1u);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*coalesce_key parameter only valid for Background tier*");
    }

    [Fact]
    public void BusFacade_PublishWithCoalesceKey_OnNormalTier_ThrowsInvalidOperation()
    {
        _facade.UseNativeBusForDispatch = true;

        Action act = () => _facade.Publish(new NormalTestEvent { Value = 99 }, coalesceKey: 2u);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*coalesce_key parameter only valid for Background tier*");
    }

    // Background tier reverse-P/Invoke harness.
    private static int s_bgSubscriberInvocations;
    private static readonly BackgroundSubscriberDelegate s_bgSubscriberDelegate = BackgroundSubscriberCallback;
    private static readonly CoalesceDelegate s_bgCoalesceDelegate = BackgroundCoalesce;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void BackgroundSubscriberDelegate(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void CoalesceDelegate(IntPtr dst, IntPtr src);

    private static void BackgroundSubscriberCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try { s_bgSubscriberInvocations++; } catch { }
    }

    // Latest-wins coalesce — 4-byte int payload, newest overwrites old.
    private static void BackgroundCoalesce(IntPtr dst, IntPtr src)
    {
        Marshal.WriteInt32(dst, Marshal.ReadInt32(src));
    }
}
