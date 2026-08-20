using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Weather;

/// <summary>
/// The strict-gate half of the W3 wave gate. The Weather pair is the first on-disk mod to declare
/// capabilities and therefore the first to leave the v1 grace path, so these tests establish what
/// "strictly gated" actually costs an author who gets it wrong -- and prove the two independent
/// checks (load-time Phase C, runtime EnforceCapability) really are independent.
/// </summary>
[Collection("GameLoopSerial")]
public sealed class WeatherCapabilityGateTests
{
    private const string SharedId = "dualfrontier.weather.contracts";
    private const string EventFqn = "DualFrontier.Mod.Weather.Contracts.WeatherChangedEvent";

    private static string PublishToken => "mod." + SharedId + ".publish:" + EventFqn;
    private static string SubscribeToken => "mod." + SharedId + ".subscribe:" + EventFqn;

    /// <summary>
    /// The runtime gate, exercised through the real pipeline. The negative fixture declares the
    /// SUBSCRIBE token (so Phase C is satisfied and it loads) and then publishes anyway. The gate
    /// must reject it loudly, and the pipeline must surface that as a failed batch.
    /// </summary>
    [Fact]
    public void UndeclaredCrossOwnerPublish_IsRejectedByTheRuntimeGate()
    {
        using var h = new WeatherHarness();

        PipelineResult result = h.Pipeline.Apply(
            new[] { WeatherHarness.ContractsPath, WeatherHarness.GateNegativePath });

        result.Success.Should().BeFalse(
            "the fixture publishes an event it never declared PUBLISH for, and its manifest is " +
            "non-empty so it gets no grace-path leniency");

        string all = WeatherHarness.Describe(result);
        all.Should().Contain("without declaring capability",
            "the diagnostic must name the missing declaration, not fail vaguely");
        all.Should().Contain(PublishToken,
            "and it must name the OWNER-NAMESPACED token, resolved from the live ledger -- not a " +
            "kernel.* token, which is what a pre-W2 gate would have produced");
    }

    /// <summary>
    /// The load-time gate. A mod that requires a token owned by another mod without LISTING that
    /// mod as a dependency is rejected at Phase C, before a single line of it runs. This is the
    /// same rule from the other side: the runtime test above shows an under-declared capability,
    /// this one shows an under-declared dependency.
    /// </summary>
    [Fact]
    public void RequiringAnOwnedToken_WithoutDeclaringTheProvider_IsRejectedAtPhaseC()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue("precondition: the ledger knows the owner");

        var manifest = new ModManifest
        {
            Id = "tests.weather.undeclared",
            Name = "tests.weather.undeclared",
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = "2.0.0",
            Capabilities = ManifestCapabilities.Parse(new[] { SubscribeToken }, null),
            Dependencies = Array.Empty<ModDependency>(),
        };
        var consumer = new LoadedMod(
            manifest.Id, manifest, new NoOpMod(), new ModLoadContext(manifest.Id), Array.Empty<Type>());

        ValidationReport report = new ContractValidator().Validate(
            new[] { consumer },
            Array.Empty<DualFrontier.Core.ECS.SystemBase>(),
            kernelCapabilities: h.Ledger);

        report.IsValid.Should().BeFalse();
        report.Errors.Should().Contain(e => e.Kind == ValidationErrorKind.MissingCapability,
            "the ledger HAS the token, but implicit satisfaction stays rejected (MOD_OS 3.4) -- " +
            "a consumer must name the provider it depends on");
    }

    /// <summary>
    /// F-56 -- re-entrant publish. A subscriber publishes a SECOND event of the same type from
    /// inside delivery of the first, through the real mod API and the real gate. Both deliveries
    /// must complete, ordering must stay coherent (the inner event is fully delivered before the
    /// outer handler returns), and nothing may deadlock or corrupt the subscriber list.
    /// </summary>
    [Fact]
    public void F56_ReEntrantPublishFromInsideDelivery_CompletesCoherently()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();

        Type eventType = ResolveEventType(h);
        RestrictedModApi api = BuildFullyDeclaredApi(h);

        var order = new List<string>();
        int depth = 0;
        int maxDepth = 0;

        SubscribeReflectively(api, eventType, evt =>
        {
            depth++;
            maxDepth = Math.Max(maxDepth, depth);
            order.Add("enter:" + depth);

            if (depth == 1)
            {
                // Depth-guarded to 1: publish exactly one nested event from inside delivery.
                PublishReflectively(api, eventType, NewEvent(eventType));
            }

            order.Add("exit:" + depth);
            depth--;
        });

        Action act = () => PublishReflectively(api, eventType, NewEvent(eventType));

        act.Should().NotThrow("a re-entrant publish must not deadlock or corrupt the bus");
        maxDepth.Should().Be(2, "the nested publish must actually re-enter the handler");
        order.Should().Equal(new[] { "enter:1", "enter:2", "exit:2", "exit:1" },
            "delivery must nest coherently: the inner event completes before the outer handler returns");
    }

    // --- Reflection helpers -------------------------------------------------
    //
    // The event type lives in the SHARED mod's ALC, so this test assembly cannot reference it at
    // compile time. That constraint is the whole point of the shared-mod design, so the test
    // honours it and reaches the type the way any other mod would: through the loaded assembly.

    private static Type ResolveEventType(WeatherHarness h)
    {
        LoadedMod? weather = h.Pipeline.GetActiveModForTests("dualfrontier.weather");
        weather.Should().NotBeNull();

        // The regular mod's ALC does NOT list the shared assembly in Assemblies -- delegation
        // means the type resolves through the SHARED ALC and belongs to it. So reach the shared
        // assembly the way any consumer does: through a type the regular mod exposes that is
        // DECLARED in terms of it. WeatherPresentationSystem.TintFor takes a WeatherKind, whose
        // assembly is the shared vendor.
        Assembly modAsm = weather!.Context.Assemblies
            .First(a => a.GetName().Name == "DualFrontier.Mod.Weather");
        Type presentation = modAsm.GetType("DualFrontier.Mod.Weather.WeatherPresentationSystem")!;
        Assembly sharedAsm = presentation
            .GetMethod("TintFor", BindingFlags.Static | BindingFlags.NonPublic)!
            .GetParameters()[0].ParameterType.Assembly;

        Type? found = sharedAsm.GetType(EventFqn);
        found.Should().NotBeNull("the shared ALC must have vended the event type to the regular mod");
        return found!;
    }

    private static RestrictedModApi BuildFullyDeclaredApi(WeatherHarness h)
    {
        var manifest = new ModManifest
        {
            Id = "tests.weather.reentrant",
            Name = "tests.weather.reentrant",
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = "2.0.0",
            Capabilities = ManifestCapabilities.Parse(new[] { PublishToken, SubscribeToken }, null),
        };

        // Built against the harness's LIVE ledger, so owner resolution is the production one.
        return new RestrictedModApi(
            manifest.Id, manifest, h.Registry, new ModContractStore(), new GameServices(), h.Ledger);
    }

    private static object NewEvent(Type eventType)
    {
        object evt = Activator.CreateInstance(eventType)!;
        Type kindType = eventType.GetProperty("Kind")!.PropertyType;
        eventType.GetProperty("Kind")!.SetValue(evt, Enum.ToObject(kindType, 2));
        eventType.GetProperty("PreviousKind")!.SetValue(evt, Enum.ToObject(kindType, 0));
        eventType.GetProperty("Intensity")!.SetValue(evt, 1f);
        return evt;
    }

    private static void SubscribeReflectively(RestrictedModApi api, Type eventType, Action<object> handler)
    {
        Type actionType = typeof(Action<>).MakeGenericType(eventType);
        Delegate typed = Delegate.CreateDelegate(
            actionType, handler.Target, handler.Method);
        typeof(RestrictedModApi).GetMethod(nameof(RestrictedModApi.Subscribe))!
            .MakeGenericMethod(eventType)
            .Invoke(api, new object[] { typed });
    }

    private static void PublishReflectively(RestrictedModApi api, Type eventType, object evt)
    {
        try
        {
            typeof(RestrictedModApi).GetMethod(nameof(RestrictedModApi.Publish))!
                .MakeGenericMethod(eventType)
                .Invoke(api, new[] { evt });
        }
        catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }

    private sealed class NoOpMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
