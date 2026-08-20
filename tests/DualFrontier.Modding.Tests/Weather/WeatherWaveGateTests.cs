using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.Application.Modding;
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
    /// F-60 (PR #49 Codex review, P1). Reload SUCCEEDS at the pipeline level -- the C5c shared-mod
    /// reuse fix holds -- but the mechanic does NOT resume: it mints a SECOND singleton and world
    /// weather resets. Cause: component type ids come from the explicit ComponentTypeRegistry
    /// (K-L4), keyed on the Type OBJECT. The reloaded mod's collectible ALC produces a DIFFERENT
    /// WeatherStateComponent Type, which is assigned a NEW id, so the span over it is empty and the
    /// ensure-singleton path seeds again.
    ///
    /// <para>
    /// This test asserts the DEFECT, deliberately, so the gap is a measured fact with a test
    /// attached instead of a claim nobody re-checks. It flips to the adopt-assertion when F-60 is
    /// resolved. The original W3 version of this test PASSED only because the harness built a bare
    /// `new NativeWorld()`, whose legacy FNV1a(AssemblyQualifiedName) ids are stable across ALCs --
    /// a harness that was not production-faithful, which is the real lesson here.
    /// </para>
    /// </summary>
    [Fact]
    public void Reload_DoesNotYetAdoptTheSurvivingSingleton_BecauseComponentIdentityIsPerAlc()
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

        h.World.EntityCount.Should().Be(2,
            "EXPECTED DEFECT (F-60): the reloaded ALC's component Type gets a NEW registry id, so " +
            "the singleton span reads empty and a SECOND weather entity is minted. Adoption is what " +
            "this should do; flip this assertion to 1 when F-60 is fixed");
        h.Sink.Calls.Count.Should().BeGreaterThan(before,
            "the reloaded mechanic does run -- it just runs against fresh state instead of the survivor");
    }

    /// <summary>
    /// F-60 (PR #49 Codex review, P1), the other half: the mod's ALC is never reclaimed, because
    /// ComponentTypeRegistry holds a STRONG reference to the mod's component Type and nothing
    /// removes it at unload. Unload therefore spends the full §9.4 step-7 timeout and reports
    /// "restart the game to fully reclaim memory". Pinned so the leak cannot be rediscovered as a
    /// surprise; the assertion inverts when F-60 is resolved.
    /// </summary>
    [Fact]
    public void Unload_LeaksTheModAlc_BecauseTheTypeRegistryStillHoldsItsComponentType()
    {
        using var h = new WeatherHarness();
        h.ApplyWeatherPair().Success.Should().BeTrue();
        h.Tick(TicksPastFirstTransition);

        IReadOnlyList<ValidationWarning> warnings = h.Pipeline.UnloadMod(RegularId);

        warnings.Should().Contain(w => w.Message.Contains("ModUnloadTimeout"),
            "EXPECTED DEFECT (F-60): the ComponentTypeRegistry roots the collectible ALC, so the " +
            "step-7 WeakReference spin runs its full 10 s and advises a restart. A clean unload " +
            "would return NO warnings -- flip this to BeEmpty when F-60 is fixed");
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
