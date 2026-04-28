using System;
using System.Collections.Generic;
using System.IO;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Api;

[EventBus("Combat")]
public sealed record TestCombatEvent(int Value) : IEvent;

public sealed record TestUnboundEvent(int Value) : IEvent;

public sealed class RestrictedModApiV2Tests
{
    private const string TestModId = "test.mod";

    // 1
    [Fact]
    public void CapabilityViolationException_MessageCtor_PreservesMessage()
    {
        var ex = new CapabilityViolationException("custom msg");
        ex.Message.Should().Be("custom msg");
    }

    // 2
    [Fact]
    public void EventBusAttribute_EmptyName_ThrowsArgumentException()
    {
        Action act = () => _ = new EventBusAttribute("");
        act.Should().Throw<ArgumentException>();
    }

    // 3
    [Fact]
    public void EventBusAttribute_ValidName_ExposesBusName()
    {
        var attr = new EventBusAttribute("Combat");
        attr.BusName.Should().Be("Combat");
    }

    // 4
    [Fact]
    public void ModBusRouter_EventWithAttribute_ResolvesCombatBus()
    {
        var services = new GameServices();
        object? bus = ModBusRouter.Resolve(typeof(TestCombatEvent), services);
        bus.Should().NotBeNull();
        bus.Should().BeSameAs(services.Combat);
    }

    // 5
    [Fact]
    public void ModBusRouter_EventWithoutAttribute_ReturnsNull()
    {
        var services = new GameServices();
        object? bus = ModBusRouter.Resolve(typeof(TestUnboundEvent), services);
        bus.Should().BeNull();
    }

    // 6
    [Fact]
    public void Publish_NullEvent_ThrowsArgumentNullException()
    {
        var (api, _) = BuildApi(ManifestCapabilities.Empty);
        Action act = () => api.Publish<TestCombatEvent>(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // 7
    [Fact]
    public void Publish_V2ManifestWithCapability_DoesNotThrow()
    {
        var (api, _) = BuildApi(V2CapsPublishOnly());
        Action act = () => api.Publish(new TestCombatEvent(1));
        act.Should().NotThrow();
    }

    // 8
    [Fact]
    public void Publish_V2ManifestWithoutCapability_ThrowsCapabilityViolation()
    {
        // v2 manifest with subscribe-only token, missing publish.
        var (api, _) = BuildApi(V2CapsSubscribeOnly());
        Action act = () => api.Publish(new TestCombatEvent(1));
        act.Should().Throw<CapabilityViolationException>()
            .Which.Message.Should().Contain("kernel.publish:");
    }

    // 9
    [Fact]
    public void Publish_V1EmptyManifest_LogsWarningAndProceeds()
    {
        var (api, _) = BuildApi(ManifestCapabilities.Empty);
        var output = new StringWriter();
        TextWriter originalOut = Console.Out;
        try
        {
            Console.SetOut(output);
            Action act = () => api.Publish(new TestCombatEvent(1));
            act.Should().NotThrow();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
        output.ToString().Should().Contain($"[WARNING][{TestModId}]");
    }

    // 10
    [Fact]
    public void Subscribe_NullHandler_ThrowsArgumentNullException()
    {
        var (api, _) = BuildApi(ManifestCapabilities.Empty);
        Action act = () => api.Subscribe<TestCombatEvent>(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // 11
    [Fact]
    public void Subscribe_V2ManifestWithCapability_TracksSubscription()
    {
        var (api, _) = BuildApi(V2CapsSubscribeOnly());
        api.Subscribe<TestCombatEvent>(_ => { });
        api.SubscriptionCount.Should().Be(1);
    }

    // 12
    [Fact]
    public void Subscribe_V2ManifestWithoutCapability_ThrowsCapabilityViolation()
    {
        var (api, _) = BuildApi(V2CapsPublishOnly());
        Action act = () => api.Subscribe<TestCombatEvent>(_ => { });
        act.Should().Throw<CapabilityViolationException>()
            .Which.Message.Should().Contain("kernel.subscribe:");
    }

    // 13
    [Fact]
    public void PublishSubscribe_RoundTrip_DeliversEventToHandler()
    {
        var (api, _) = BuildApi(V2Caps());
        TestCombatEvent? received = null;
        api.Subscribe<TestCombatEvent>(e => received = e);
        api.Publish(new TestCombatEvent(42));
        received.Should().NotBeNull();
        received!.Value.Should().Be(42);
    }

    // 14
    [Fact]
    public void UnsubscribeAll_AfterSubscribe_HandlerNotInvokedOnPublish()
    {
        var (api, _) = BuildApi(V2Caps());
        int callCount = 0;
        api.Subscribe<TestCombatEvent>(_ => callCount++);
        api.UnsubscribeAll();
        api.Publish(new TestCombatEvent(0));
        callCount.Should().Be(0);
    }

    // 15
    [Fact]
    public void UnsubscribeAll_AfterSubscribe_SubscriptionCountIsZero()
    {
        var (api, _) = BuildApi(V2Caps());
        api.Subscribe<TestCombatEvent>(_ => { });
        api.UnsubscribeAll();
        api.SubscriptionCount.Should().Be(0);
    }

    // 16
    [Fact]
    public void GetOwnManifest_ReturnsConstructorManifest()
    {
        var manifest = new ModManifest { Id = TestModId };
        var api = new RestrictedModApi(
            TestModId,
            manifest,
            new ModRegistry(),
            new ModContractStore(),
            new GameServices(),
            KernelCapabilityRegistry.BuildFromKernelAssemblies());
        api.GetOwnManifest().Should().BeSameAs(manifest);
    }

    // 17
    [Fact]
    public void GetKernelCapabilities_ReturnsRegistryCapabilities_SameInstanceAcrossCalls()
    {
        var (api, _) = BuildApi(ManifestCapabilities.Empty);
        IReadOnlySet<string> first = api.GetKernelCapabilities();
        IReadOnlySet<string> second = api.GetKernelCapabilities();
        second.Should().BeSameAs(first);
    }

    // 18
    [Fact]
    public void Log_AnyLevel_DoesNotThrow()
    {
        var (api, _) = BuildApi(ManifestCapabilities.Empty);
        TextWriter originalOut = Console.Out;
        try
        {
            Console.SetOut(new StringWriter());
            foreach (ModLogLevel level in Enum.GetValues<ModLogLevel>())
            {
                Action act = () => api.Log(level, "test");
                act.Should().NotThrow();
            }
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // --- Subscribe handler wrapping (MOD_OS_ARCHITECTURE §4.3) -----------

    // 19
    [Fact]
    public void Subscribe_with_active_context_invokes_handler_with_same_context()
    {
        var (api, _) = BuildApi(V2Caps());
        SystemExecutionContext ctx = BuildTestContext();
        SystemExecutionContext? observed = null;

        SystemExecutionContext.PushContext(ctx);
        try
        {
            api.Subscribe<TestCombatEvent>(_ =>
                observed = SystemExecutionContext.Current);
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }

        api.Publish(new TestCombatEvent(1));

        observed.Should().BeSameAs(ctx);
    }

    // 20
    [Fact]
    public void Subscribe_without_active_context_invokes_handler_with_null_context()
    {
        var (api, _) = BuildApi(V2Caps());
        SystemExecutionContext? observed = null;
        bool handlerRan = false;

        api.Subscribe<TestCombatEvent>(_ =>
        {
            observed = SystemExecutionContext.Current;
            handlerRan = true;
        });

        Action act = () => api.Publish(new TestCombatEvent(1));
        act.Should().NotThrow();
        handlerRan.Should().BeTrue();
        observed.Should().BeNull();
    }

    // 21
    [Fact]
    public void Subscribe_handler_context_is_popped_after_handler_returns()
    {
        var (api, _) = BuildApi(V2Caps());
        SystemExecutionContext ctx = BuildTestContext();

        SystemExecutionContext.PushContext(ctx);
        try
        {
            api.Subscribe<TestCombatEvent>(_ => { });
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }

        api.Publish(new TestCombatEvent(1));

        SystemExecutionContext.Current.Should().BeNull();
    }

    // 22
    [Fact]
    public void Subscribe_handler_context_is_popped_when_handler_throws()
    {
        var (api, _) = BuildApi(V2Caps());
        SystemExecutionContext ctx = BuildTestContext();

        SystemExecutionContext.PushContext(ctx);
        try
        {
            api.Subscribe<TestCombatEvent>(_ =>
                throw new InvalidOperationException("test handler failure"));
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }

        try { api.Publish(new TestCombatEvent(1)); }
        catch (InvalidOperationException) { /* propagated from handler — ignore */ }

        SystemExecutionContext.Current.Should().BeNull();
    }

    private static SystemExecutionContext BuildTestContext(string name = "TestSystem")
    {
        return new SystemExecutionContext(
            new World(),
            name,
            Array.Empty<Type>(),
            Array.Empty<Type>(),
            new[] { "TestBus" },
            SystemOrigin.Core,
            modId: null,
            faultSink: new NullModFaultSink());
    }

    private static (RestrictedModApi api, GameServices services) BuildApi(ManifestCapabilities caps)
    {
        var manifest = new ModManifest { Id = TestModId, Capabilities = caps };
        var services = new GameServices();
        var api = new RestrictedModApi(
            TestModId,
            manifest,
            new ModRegistry(),
            new ModContractStore(),
            services,
            KernelCapabilityRegistry.BuildFromKernelAssemblies());
        return (api, services);
    }

    private static ManifestCapabilities V2Caps()
    {
        string fqn = typeof(TestCombatEvent).FullName!;
        return ManifestCapabilities.Parse(
            new[] { $"kernel.publish:{fqn}", $"kernel.subscribe:{fqn}" },
            null);
    }

    private static ManifestCapabilities V2CapsPublishOnly()
    {
        return ManifestCapabilities.Parse(
            new[] { $"kernel.publish:{typeof(TestCombatEvent).FullName}" },
            null);
    }

    private static ManifestCapabilities V2CapsSubscribeOnly()
    {
        return ManifestCapabilities.Parse(
            new[] { $"kernel.subscribe:{typeof(TestCombatEvent).FullName}" },
            null);
    }
}
