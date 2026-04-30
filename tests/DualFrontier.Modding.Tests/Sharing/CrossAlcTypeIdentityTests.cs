using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sharing;

/// <summary>
/// Acceptance tests for MOD_OS_ARCHITECTURE §11.1 M4: a shared mod's record
/// is loaded into the shared ALC, regular mods that depend on it see the
/// same <see cref="Type"/> instance, and an event published from one regular
/// mod is received by a handler subscribed in another regular mod.
/// </summary>
public sealed class CrossAlcTypeIdentityTests
{
    private const string SharedEventTypeFqn = "Fixture.SharedEvents.SharedTestEvent";

    [Fact]
    public void SharedMod_DefiningEvent_LoadsIntoSharedAlc()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();

        LoadedSharedMod loaded = loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);

        loaded.ModId.Should().Be("tests.shared.events");
        loaded.Manifest.Kind.Should().Be(ModKind.Shared);
        loaded.Context.Should().BeSameAs(sharedAlc);
        loaded.Context.IsCollectible.Should().BeFalse(
            "shared ALC must persist for the session per MOD_OS_ARCHITECTURE §1.4");

        Type? sharedEventType = loaded.ExportedTypes
            .FirstOrDefault(t => t.FullName == SharedEventTypeFqn);
        sharedEventType.Should().NotBeNull(
            "the shared mod must export the SharedTestEvent record");
    }

    [Fact]
    public void RegularMod_DependingOnShared_ResolvesToSameTypeInstance()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();

        LoadedSharedMod shared = loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);
        LoadedMod publisher = loader.LoadRegularMod(TestModPaths.PublisherMod, sharedAlc);
        LoadedMod subscriber = loader.LoadRegularMod(TestModPaths.SubscriberMod, sharedAlc);

        Type sharedEventType = shared.ExportedTypes
            .Single(t => t.FullName == SharedEventTypeFqn);

        Type publisherSawType = ReadSharedEventTypeProperty(publisher.Instance);
        Type subscriberSawType = ReadSharedEventTypeProperty(subscriber.Instance);

        publisherSawType.Should().BeSameAs(sharedEventType,
            "publisher's typeof(SharedTestEvent) must resolve through the shared ALC");
        subscriberSawType.Should().BeSameAs(sharedEventType,
            "subscriber's typeof(SharedTestEvent) must resolve through the shared ALC");
    }

    [Fact]
    public void CrossAlcPubSub_DeliversEvent()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();
        var registry = new ModRegistry();
        var contractStore = new ModContractStore();
        var services = new GameServices();
        KernelCapabilityRegistry capabilities = KernelCapabilityRegistry.BuildFromKernelAssemblies();

        // Pass 1 — shared mod first so subsequent regular contexts can
        // delegate to its ALC.
        _ = loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);

        // Pass 2 — regular mods. Order matters for this test: subscriber
        // must subscribe before publisher publishes, since SharedTestEvent
        // uses synchronous delivery.
        LoadedMod subscriber = loader.LoadRegularMod(TestModPaths.SubscriberMod, sharedAlc);
        LoadedMod publisher = loader.LoadRegularMod(TestModPaths.PublisherMod, sharedAlc);

        var subApi = new RestrictedModApi(
            subscriber.ModId, subscriber.Manifest, registry, contractStore, services, capabilities);
        subscriber.Instance.Initialize(subApi);

        var pubApi = new RestrictedModApi(
            publisher.ModId, publisher.Manifest, registry, contractStore, services, capabilities);
        publisher.Instance.Initialize(pubApi);

        InvokePublic(publisher.Instance, "TriggerPublish", new object[] { "hello-cross-alc" });

        IReadOnlyList<string> received = ReadReceivedPayloads(subscriber.Instance);
        received.Should().ContainSingle()
            .Which.Should().Be("hello-cross-alc",
                "the subscriber's handler runs when the publisher publishes — " +
                "this is the M4 central acceptance demonstration of cross-ALC " +
                "type identity (MOD_OS_ARCHITECTURE §11.1)");
    }

    private static Type ReadSharedEventTypeProperty(IMod modInstance)
    {
        PropertyInfo prop = modInstance.GetType()
            .GetProperty("SharedEventType", BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException(
                $"Mod fixture '{modInstance.GetType().FullName}' does not expose SharedEventType.");
        object? value = prop.GetValue(modInstance);
        return value as Type
            ?? throw new InvalidOperationException("SharedEventType returned a non-Type value.");
    }

    private static IReadOnlyList<string> ReadReceivedPayloads(IMod modInstance)
    {
        PropertyInfo prop = modInstance.GetType()
            .GetProperty("ReceivedPayloads", BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException(
                $"Mod fixture '{modInstance.GetType().FullName}' does not expose ReceivedPayloads.");
        return prop.GetValue(modInstance) as IReadOnlyList<string>
            ?? throw new InvalidOperationException("ReceivedPayloads returned null or wrong type.");
    }

    private static void InvokePublic(IMod modInstance, string methodName, object?[] args)
    {
        MethodInfo method = modInstance.GetType()
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException(
                $"Mod fixture '{modInstance.GetType().FullName}' has no method '{methodName}'.");
        method.Invoke(modInstance, args);
    }
}
