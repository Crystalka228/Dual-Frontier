using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using DualFrontier.Core.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Weather;

/// <summary>
/// The W3 wave gate. Every assertion runs against the REAL mod artifacts on disk, loaded through
/// the REAL pipeline, rather than against unit doubles that can happily agree with a broken
/// design. If the SDK surface, the ledger wiring, the shared/regular split or the presentation
/// seam were wrong, these fail.
/// </summary>
[Collection("GameLoopSerial")]
public sealed class WeatherWaveGateTests
{
    private const string SharedId = "dualfrontier.weather.contracts";
    private const string RegularId = "dualfrontier.weather";
    private const string EventFqn = "DualFrontier.Mod.Weather.Contracts.WeatherChangedEvent";
    private const string ComponentFqn = "DualFrontier.Mod.Weather.WeatherStateComponent";

    private static string PublishToken => "mod." + SharedId + ".publish:" + EventFqn;
    private static string SubscribeToken => "mod." + SharedId + ".subscribe:" + EventFqn;

    // WeatherSystem.TransitionPeriodTicks is 300 and the system runs at NORMAL (every 15 ticks),
    // so ~340 ticks reliably crosses the first transition boundary.
    private const int TicksPastFirstTransition = 340;

    /// <summary>
    /// F-55 -- the end-to-end proof this finding was opened for: a MOD-AUTHORED event, vended by a
    /// SHARED mod, published by a REGULAR mod through the strict capability gate, delivered to a
    /// subscriber, and turned into an observable effect. Every link is production code; nothing is
    /// stubbed but the renderer.
    /// </summary>
    [Fact]
    public void F55_WeatherPair_LoadsTicksTransitionsAndRoundTripsItsOwnEventCrossOwner()
    {
        using var h = new WeatherHarness();

        PipelineResult result = h.ApplyWeatherPair();
        result.Success.Should().BeTrue(
            "the pair must load through the real pipeline; errors: " + WeatherHarness.Describe(result));
        result.LoadedModIds.Should().Contain(RegularId);

        h.World.EntityCount.Should().Be(0, "nothing exists before the mechanic runs");

        h.Tick(TicksPastFirstTransition);

        h.World.EntityCount.Should().Be(1,
            "the mechanic mints exactly ONE weather singleton and adopts it on every later tick");
        h.Sink.Calls.Should().NotBeEmpty(
            "a transition publishes WeatherChangedEvent across the owner boundary, the sibling system " +
            "receives it through the gate, and paints. If any link were broken this list would be empty.");
    }

    [Fact]
    public void WeatherHistory_IsDeterministic_AcrossTwoIndependentSessions()
    {
        List<(float R, float G, float B, float Strength)> Run()
        {
            using var h = new WeatherHarness();
            h.ApplyWeatherPair().Success.Should().BeTrue();
            h.Tick(TicksPastFirstTransition * 3);
            return h.Sink.Calls;
        }

        List<(float R, float G, float B, float Strength)> first = Run();
        List<(float R, float G, float B, float Strength)> second = Run();

        first.Should().NotBeEmpty("the run must actually produce transitions for this to mean anything");
        second.Should().Equal(first,
            "the transition law is a pure hash of (SimTick, current kind) over a compile-time seed: " +
            "no wall clock and no RNG state, so identical tick histories give identical weather histories");
    }

    [Fact]
    public void EveryTint_StaysInRange_AndClearRestoresTheUntintedScene()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();

        h.Tick(TicksPastFirstTransition * 4);

        h.Sink.Calls.Should().NotBeEmpty();
        foreach ((float r, float g, float b, float strength) in h.Sink.Calls)
        {
            r.Should().BeInRange(0f, 1f);
            g.Should().BeInRange(0f, 1f);
            b.Should().BeInRange(0f, 1f);
            strength.Should().BeInRange(0f, 1f);

            // Clear is the only kind whose table colour is pure white, and it is also the only
            // kind the mod gives intensity 0, so its strength must be exactly 0.
            bool isClear = r == 1f && g == 1f && b == 1f;
            if (isClear)
                strength.Should().Be(0f, "Clear must restore the untinted scene exactly, not approximately");
            else
                strength.Should().BeGreaterThan(0f, "non-Clear weather must be visible");
        }
    }

    [Fact]
    public void Unload_RemovesTheMechanicEntirely_AndLeavesTheEngineHealthy()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();
        h.Tick(TicksPastFirstTransition);
        h.Sink.Calls.Should().NotBeEmpty("precondition: the mechanic was running");

        h.Pipeline.UnloadMod(RegularId);
        int callsAtUnload = h.Sink.Calls.Count;

        h.Tick(TicksPastFirstTransition * 2);

        h.Sink.Calls.Count.Should().Be(callsAtUnload,
            "the systems left the graph and unload chain step 1 released the subscription, so no " +
            "further weather is simulated OR delivered");

        // G3 -- a mod cannot reclaim its own world state at unload. The singleton it minted
        // OUTLIVES it as inert residue. Pinned here so the gap is a measured fact with a test
        // attached rather than a remark. See the G3 F-row in ROADMAP.md.
        h.World.EntityCount.Should().Be(1,
            "EXPECTED residue (G3): OnDispose is parameterless, so a mod has no way to reach the " +
            "world and clean up; the entity survives the ALC that created it");

        Action keepTicking = () => h.Tick(50);
        keepTicking.Should().NotThrow("an unloaded mod must leave the engine healthy, not merely quiet");
    }

    [Fact]
    public void Unload_ClearsTheAmbientTint_SoTheSceneDoesNotStayPainted()
    {
        // PR #49 Codex review (P2). Dropping the subscription only stops FUTURE tinting. Unloading
        // mid-storm would otherwise leave the scene permanently dark with nothing left to explain
        // it -- "removes the mechanic entirely" has to include what the mechanic drew.
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();
        h.Tick(TicksPastFirstTransition);

        h.Sink.Calls.Should().NotBeEmpty("precondition: the mechanic painted something");

        h.Pipeline.UnloadMod(RegularId);

        (float R, float G, float B, float Strength) last = h.Sink.Calls[^1];
        last.Strength.Should().Be(0f,
            "the LAST thing an unloading weather mod does is un-paint: strength 0 is the identity " +
            "modulation, so the scene returns to exactly its untinted state");
        last.R.Should().Be(1f);
        last.G.Should().Be(1f);
        last.B.Should().Be(1f);
    }

    /// <summary>
    /// F-60 CLOSED (ID-A). Reload ADOPTS the surviving singleton: the mechanic resumes against the
    /// state its previous incarnation left behind instead of minting a second entity and resetting
    /// world weather. Component identity is the owner-scoped stable NAME, not the Type OBJECT
    /// (K-L4 as amended), so the reloaded collectible ALC's fresh WeatherStateComponent Type
    /// re-adopts the same identity, hence the same id, hence the same native store.
    ///
    /// <para>
    /// This assertion was pinned in its inverted form through W3 -- EntityCount 2, the defect as a
    /// measured fact with a test attached -- and flips here, in the commit that makes it true. The
    /// original W3 version passed for the wrong reason: the harness built a bare `new NativeWorld()`,
    /// whose legacy FNV1a(AssemblyQualifiedName) ids are stable across ALCs. The harness is
    /// production-faithful now, so this is the real behaviour of the real composition.
    /// </para>
    /// </summary>
    [Fact]
    public void Reload_AdoptsTheSurvivingSingleton_ComponentIdentitySurvivesAlcReload()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();
        h.Tick(TicksPastFirstTransition);
        h.Pipeline.UnloadMod(RegularId);
        h.World.EntityCount.Should().Be(1);

        // Reload the PAIR, exactly as a player toggling the mod back on would: the menu applies
        // the whole selected set. The shared vendor is still resident in the non-collectible
        // shared ALC and is REUSED rather than reloaded (C5c); passing it is also required,
        // because dependency presence is checked against the batch.
        PipelineResult again = h.ApplyWeatherPair();
        again.Success.Should().BeTrue(
            "the pipeline half of reload works -- C5c's shared-mod reuse is what makes even this " +
            "much possible; errors: " + WeatherHarness.Describe(again));

        int before = h.Sink.Calls.Count;
        h.Tick(TicksPastFirstTransition);

        h.World.EntityCount.Should().Be(1,
            "the reloaded ALC's component Type re-adopts the surviving identity, so its id is the " +
            "same id, the singleton span reads the survivor, and no second weather entity is minted");
        h.Sink.Calls.Count.Should().BeGreaterThan(before,
            "the reloaded mechanic runs -- and it runs against the survivor, so weather RESUMES");
    }

    /// <summary>
    /// F-60(a) CLOSED. The mod's ALC is reclaimed after it has ticked. This assertion was an
    /// EXPECTED-DEFECT pin for two cascades: W3 and the identity recon both concluded the root was
    /// the component type registry, ID-A disproved that by re-keying the registry so it holds no
    /// Type reference at all (and the leak survived), and this cascade found and removed the actual
    /// root.
    ///
    /// <para>
    /// Measured by bisection on the production composition at ID-A, holding everything else fixed
    /// and varying only how many ticks elapse between load and unload:
    /// 0 ticks -> 3 ms, no warnings; 1 tick -> 10,459 ms + ModUnloadTimeout; 16 -> 10,518 ms;
    /// 100 -> 10,456 ms; 340 -> 10,565 ms. A SINGLE ExecuteTick was sufficient to root the ALC, and
    /// more ticks changed nothing -- the signature of a memoising GetOrAdd, not of an accumulating
    /// queue. That table is kept because it is what located the root; the companion tests below and
    /// above pin its two ends (0 ticks and 1 tick) so the shape cannot silently drift.
    /// </para>
    ///
    /// <para>
    /// THE ROOT (F60A_TICK_PATH): <c>TickScheduler._tickRateCache</c>, a session-lived
    /// <c>ConcurrentDictionary&lt;Type,int&gt;</c> populated by <c>GetOrAdd(system.GetType(), ...)</c>
    /// on the FIRST <c>ShouldRun</c> call and evicted by nothing -- its only clearing method,
    /// <c>Reset()</c>, had zero production callers, and the TickScheduler instance survives every
    /// pipeline Rebuild as the scheduler's readonly field. For an SDK mod system the key is
    /// <c>SystemAdapter&lt;TSystem&gt;</c> closed over a type from the mod's collectible ALC, so a
    /// single tick planted a strong Type key that kept the whole ALC alive for the session.
    /// THE FIX: the tick rate is resolved at LOAD time into the scheduler's per-system
    /// <c>SystemMetadata</c> table -- which is keyed by instance and swapped wholesale at every load
    /// boundary, so it is evicted at unload for free -- and the lazy cache is deleted outright.
    /// </para>
    /// </summary>
    [Fact]
    public void Unload_AfterTicking_ReleasesTheModAlc_TickPathHoldsNoTypeKey()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();
        h.Tick(TicksPastFirstTransition);

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(RegularId);

        warnings.Should().BeEmpty(
            "F-60(a) is closed: nothing on the tick path holds a Type key that outlives unload, so " +
            "the step-7 WeakReference spin observes the release on its first GC pump pass instead " +
            "of running its full 10 s and advising a restart");
    }

    /// <summary>
    /// The minimal repro, pinned for the first time by F60A_TICK_PATH. The bisection recorded above
    /// established that ONE tick was enough to root the ALC, but no test pinned that datum -- the
    /// pair was 340-ticks-fails and 0-ticks-passes, which leaves the actual boundary untested.
    ///
    /// <para>
    /// This matters beyond tidiness: the defect was a first-call memoisation, so the single tick is
    /// exactly where it was created. A regression that reintroduced lazy per-type population on the
    /// tick path would fail HERE first and most cheaply, and a 340-tick pin passing while this one
    /// failed would be a contradiction worth stopping for.
    /// </para>
    /// </summary>
    [Fact]
    public void Unload_AfterSingleTick_ReleasesTheModAlc_Immediately()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();
        h.Tick(1);

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(RegularId);

        warnings.Should().BeEmpty(
            "one tick is the exact point at which the old per-type cache was populated; with the " +
            "rate resolved at load time there is nothing for that first tick to plant");
    }

    /// <summary>
    /// The control that turns the assertion above from a complaint into a measurement: the SAME mod
    /// pair, loaded through the SAME production composition, released cleanly when it is unloaded
    /// without ever being ticked. Initialize has run, the mod has claimed its component, ownership
    /// is recorded -- and unload still completes in milliseconds with no warning.
    ///
    /// <para>
    /// This is what proves the leak is not caused by loading a component-defining mod, and not by
    /// the registry's registration bookkeeping. Pinning it here means a future fix cannot quietly
    /// regress the clean case while chasing the dirty one, and a future reader cannot re-derive the
    /// discarded "the type registry roots it" explanation without this test contradicting them.
    /// </para>
    /// </summary>
    [Fact]
    public void Unload_WithoutTicking_ReleasesTheModAlc_Immediately()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(RegularId);

        warnings.Should().BeEmpty(
            "with no tick between load and unload nothing has rooted the ALC, so the step-7 spin " +
            "observes the release on its first GC pump pass");
    }

    /// <summary>
    /// ID-A / D2 -- the mod's component carries the MOD's identity, not the kernel's.
    /// Before this cascade the id was allocated lazily inside a tick, at a site in
    /// Core.Interop that knows nothing about mods, so the only namespace available to it
    /// was the kernel's own. Registering at Apply is what puts modId in scope, and this
    /// asserts the consequence: the reverse map answers with owner "mod.dualfrontier.weather".
    ///
    /// <para>
    /// This is the cross-mod isolation clause of К-L4 made observable. If the owner here
    /// were ever "kernel", two mods shipping the same component FullName would silently
    /// share one id and therefore one store.
    /// </para>
    /// </summary>
    [Fact]
    public void ModComponent_TakesAnOwnerScopedId_AtApply_NotAKernelOne()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();

        ComponentTypeRegistry registry = h.World.Registry!;
        registry.TryGetCachedId(ResolveWeatherComponentType(h), out uint id).Should().BeTrue(
            "the pipeline allocates the id at Apply, before any tick runs");

        ComponentIdentity identity = registry.Lookup(id)!.Value;
        identity.Owner.Should().Be("mod." + RegularId,
            "the component belongs to the mod that declared it, not to the engine");
        identity.TypeFullName.Should().Be(ComponentFqn);
    }

    /// <summary>
    /// ID-A / D3 -- a component type from a collectible load context that was never
    /// registered is a loud failure at resolution, naming the remedy. The alternative --
    /// quietly adopting it under the kernel owner -- would merge identities across owners
    /// and undo the isolation the re-key exists to provide, so the guard refuses instead
    /// of guessing.
    ///
    /// <para>
    /// The probe deliberately loads the mod's assembly into its OWN collectible context,
    /// bypassing the pipeline, because bypassing the pipeline is exactly the situation the
    /// guard exists for: a mod component that never went through IModApi.RegisterComponent.
    /// </para>
    /// </summary>
    [Fact]
    public void UnregisteredCollectibleType_Throws_NamingTheRemedy()
    {
        var probe = new AssemblyLoadContext("id-a-guard-probe", isCollectible: true);
        Assembly modAssembly = probe.LoadFromAssemblyPath(
            Path.Combine(WeatherHarness.WeatherPath, "DualFrontier.Mod.Weather.dll"));
        Type componentType = modAssembly.GetType(ComponentFqn)!;
        componentType.Should().NotBeNull();
        AssemblyLoadContext.GetLoadContext(modAssembly)!.IsCollectible.Should().BeTrue(
            "precondition: the probe context must be collectible for the guard to apply");

        using NativeWorld world = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        EntityId entity = world.CreateEntity();
        MethodInfo add = typeof(NativeWorld)
            .GetMethod(nameof(NativeWorld.AddComponent))!
            .MakeGenericMethod(componentType);

        Action act = () => add.Invoke(world, new[] { (object)entity, Activator.CreateInstance(componentType)! });

        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("*IModApi.RegisterComponent*",
                "the diagnostic must hand the mod author the fix, not just report a refusal");
    }

    /// <summary>
    /// ID-A / D3, the case a collectibility test would have missed (Codex P1 on PR #50). The
    /// shared-mod load context is deliberately NON-collectible -- shared assemblies must never
    /// unload mid-session -- yet it holds MOD-authored types. A guard keyed on collectibility
    /// therefore waved shared components straight into the kernel namespace, and two shared mods
    /// defining one component FullName would have landed on a single identity and a single native
    /// store: precisely the isolation this cascade exists to make structural.
    ///
    /// <para>
    /// The probe builds a NON-collectible context of its own, which is what the shared ALC is, and
    /// asserts the refusal still fires. The precondition is asserted first so the test cannot
    /// quietly start passing for the wrong reason.
    /// </para>
    /// </summary>
    [Fact]
    public void UnregisteredNonCollectibleModType_IsAlsoRefused_NotAdoptedByTheKernel()
    {
        var sharedLike = new AssemblyLoadContext("shared-like-probe", isCollectible: false);
        Assembly modAssembly = sharedLike.LoadFromAssemblyPath(
            Path.Combine(WeatherHarness.WeatherPath, "DualFrontier.Mod.Weather.dll"));
        Type componentType = modAssembly.GetType(ComponentFqn)!;

        AssemblyLoadContext probeContext = AssemblyLoadContext.GetLoadContext(modAssembly)!;
        probeContext.IsCollectible.Should().BeFalse(
            "precondition: this probe stands in for the shared-mod ALC, which is non-collectible");
        probeContext.Should().NotBeSameAs(AssemblyLoadContext.Default,
            "precondition: it is still not the kernel's own context");

        using NativeWorld world = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        EntityId entity = world.CreateEntity();
        MethodInfo add = typeof(NativeWorld)
            .GetMethod(nameof(NativeWorld.AddComponent))!
            .MakeGenericMethod(componentType);

        Action act = () => add.Invoke(world, new[] { (object)entity, Activator.CreateInstance(componentType)! });

        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("*IModApi.RegisterComponent*",
                "a mod type must be refused whether or not its context happens to be collectible");
    }

    /// <summary>
    /// Resolves the loaded mod's component Type through the registry's own cache, which is
    /// the only place the reloaded ALC's Type object is reachable from the test side.
    /// </summary>
    private static Type ResolveWeatherComponentType(WeatherHarness h)
    {
        foreach (ActiveModInfo info in h.Pipeline.GetActiveMods())
        {
            if (info.ModId != RegularId)
                continue;

            foreach (Type claimed in h.Registry.ComponentTypesOf(RegularId))
            {
                if (claimed.FullName == ComponentFqn)
                    return claimed;
            }
        }

        throw new InvalidOperationException(
            $"The loaded weather mod did not claim {ComponentFqn}; the harness precondition is broken.");
    }

    [Fact]
    public void OwnershipLifecycle_SharedPersistsAcrossRegularUnload()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();

        KernelCapabilityRegistry ledger = h.Ledger;
        ledger.Provides(PublishToken).Should().BeTrue("the shared mod owns the event type");
        ledger.Provides(SubscribeToken).Should().BeTrue();
        ledger.OwnerOf(EventFqn).Should().Be("mod." + SharedId);
        ledger.Owns("mod." + RegularId, ComponentFqn).Should().BeFalse(
            "the component carries no [ModAccessible], so it emits no capability token at all");

        h.Pipeline.UnloadMod(RegularId);

        ledger.Provides(PublishToken).Should().BeTrue(
            "SHARED ownership persists for the session: the assembly is still resolvable in the " +
            "non-collectible shared ALC, so revoking its ownership would make the ledger lie");
        ledger.OwnerOf(EventFqn).Should().Be("mod." + SharedId);
    }

    [Fact]
    public void ValidationFailureRollback_LeavesNoPhantomRegularOwnership()
    {
        using var h = new WeatherHarness();

        // The regular mod alone: its declared shared dependency is absent, so the batch fails
        // validation and rolls back AFTER pass [2] already registered it as an owner.
        PipelineResult result = h.Pipeline.Apply(new[] { WeatherHarness.WeatherPath });

        result.Success.Should().BeFalse("the declared shared dependency is absent from the batch");
        h.Ledger.Capabilities
            .Where(c => c.StartsWith("mod." + RegularId + ".", StringComparison.Ordinal))
            .Should().BeEmpty(
                "a rolled-back mod must leave NO ownership behind, or a later mod could silently " +
                "satisfy a capability token against a phantom provider");
    }
}
