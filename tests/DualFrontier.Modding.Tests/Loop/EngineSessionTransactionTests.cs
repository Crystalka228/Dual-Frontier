using System;
using System.Collections.Generic;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Loop;

/// <summary>
/// EQ_A2 C5 — behavioral coverage of the EngineSession world-shutdown transaction
/// (RESOURCE_OWNERSHIP_AND_LIFETIME §4.4 / CONCURRENCY_AND_MEMORY_MODEL §6.2 /
/// ENGINE_LIFECYCLE_AND_TRANSACTIONS §2.6; seats К-L20). Constructs a real session via
/// <see cref="GameBootstrap.CreateSession"/> and drives Dispose through the
/// <see cref="ShutdownTransactionHooks"/> injection seam (deadline / sim-fence /
/// quiescence / abort / step recorder), so the teardown ORDER and the
/// abort-not-teardown path are asserted deterministically WITHOUT starting the sim
/// thread or terminating the host (the H-SEAM resolution).
///
/// <c>[Collection("GameLoopSerial")]</c>: each test builds a native world and
/// registers the process-global native scheduler, so they must not run concurrently
/// with each other or the sibling bootstrap suites (the F-10 device).
/// </summary>
[Collection("GameLoopSerial")]
public sealed class EngineSessionTransactionTests
{
    private static EngineSession NewSession(ShutdownTransactionHooks hooks)
        => GameBootstrap.CreateSession(new PresentationBridge(), modsRoot: "mods", shutdownHooks: hooks);

    [Fact]
    public void Dispose_RunsTeardownStepsInReverseAcquisitionOrder()
    {
        // К-L20 falsifiability (b): a teardown-order assertion must fail if any step
        // runs before the fence passes. The fence passes vacuously here (loop never
        // started -> TryStop true; pipeline depth 0 -> quiescent), then teardown runs
        // in reverse acquisition order.
        var steps = new List<ShutdownStep>();
        EngineSession session = NewSession(new ShutdownTransactionHooks { OnStep = steps.Add });

        session.Dispose();

        steps.Should().Equal(
            ShutdownStep.FencePassed,
            ShutdownStep.DeferredDropped,
            ShutdownStep.ModsUnloaded,
            ShutdownStep.NativeSchedulerCleared,
            ShutdownStep.NativeBusCleared,
            ShutdownStep.WorldDisposed);
    }

    [Fact]
    public void Dispose_RunsUnloadAllAndNativeBusClearExactlyOnce()
    {
        var steps = new List<ShutdownStep>();
        EngineSession session = NewSession(new ShutdownTransactionHooks { OnStep = steps.Add });

        session.Dispose();

        steps.Should().ContainSingle(s => s == ShutdownStep.ModsUnloaded,
            "UnloadAll (S4, its first production caller) runs exactly once per dispose");
        steps.Should().ContainSingle(s => s == ShutdownStep.NativeBusCleared,
            "df_bus_clear (S5) is present as a promoted production teardown step");
    }

    [Fact]
    public void Dispose_OnFenceTimeout_AbortsWithoutTeardown()
    {
        // К-L20 falsifiability (a): an injected fence timeout must produce
        // abort-not-teardown. SimFenceOverride simulates the bounded-join miss (D4);
        // OnAbort observes the abort so no Environment.FailFast fires.
        var steps = new List<ShutdownStep>();
        ShutdownAbortReport? abort = null;
        EngineSession session = NewSession(new ShutdownTransactionHooks
        {
            FenceDeadline = TimeSpan.FromMilliseconds(50),
            SimFenceOverride = _ => false,
            OnStep = steps.Add,
            OnAbort = r => abort = r,
        });

        session.Dispose();

        abort.Should().NotBeNull("a missed fence must abort the transaction");
        abort!.ThreadStopped.Should().BeFalse();
        steps.Should().Equal(ShutdownStep.Aborted);
        steps.Should().NotContain(ShutdownStep.WorldDisposed,
            "abort must NOT tear down native state under a possibly-live sim thread (leak-on-abort, D4)");
    }

    [Fact]
    public void Dispose_IsIdempotent_SecondCallIsNoOp()
    {
        var steps = new List<ShutdownStep>();
        EngineSession session = NewSession(new ShutdownTransactionHooks { OnStep = steps.Add });

        session.Dispose();
        int afterFirst = steps.Count;
        session.Dispose();

        steps.Count.Should().Be(afterFirst, "double-Dispose is a no-op");
        steps.Should().ContainSingle(s => s == ShutdownStep.WorldDisposed,
            "the world is disposed exactly once across repeated Dispose calls");
    }
}

/// <summary>
/// EQ_A2 C5 — the Degraded / EngineHealth surface (ENGINE_LIFECYCLE_AND_TRANSACTIONS
/// §4.1): a session-scoped annotation with structured reasons, a lifecycle event on
/// every entry/exit, and mod-id dedupe. <c>using</c> disposes each session cleanly
/// (fence passes vacuously; teardown runs).
/// </summary>
[Collection("GameLoopSerial")]
public sealed class EngineHealthTests
{
    [Fact]
    public void Health_StartsNormal_ReportDegraded_EntersDegraded_AndEmitsEnteredEvent()
    {
        using EngineSession session = GameBootstrap.CreateSession(new PresentationBridge());
        var events = new List<EngineHealthChanged>();
        session.HealthChanged += events.Add;

        session.Health.Kind.Should().Be(EngineHealthKind.Normal);

        session.ReportDegraded(DegradedReason.ForQuarantinedMod("dualfrontier.example", tickId: 7));

        session.Health.Kind.Should().Be(EngineHealthKind.Degraded);
        session.Health.Reasons.Should().ContainSingle()
            .Which.ModId.Should().Be("dualfrontier.example");
        events.Should().ContainSingle();
        events[0].Entered.Should().BeTrue("the first reason crosses Normal -> Degraded");
        events[0].ResultingKind.Should().Be(EngineHealthKind.Degraded);
    }

    [Fact]
    public void ReportDegraded_DedupesByModId()
    {
        using EngineSession session = GameBootstrap.CreateSession(new PresentationBridge());

        session.ReportDegraded(DegradedReason.ForQuarantinedMod("mod.a", 1));
        session.ReportDegraded(DegradedReason.ForQuarantinedMod("mod.a", 2));

        session.Health.Reasons.Should().ContainSingle(
            "a mod contributes one Degraded reason however many of its systems fault");
    }

    [Fact]
    public void ClearDegradedForMod_ExitsToNormal_AndEmitsExitEvent()
    {
        using EngineSession session = GameBootstrap.CreateSession(new PresentationBridge());
        session.ReportDegraded(DegradedReason.ForQuarantinedMod("mod.a", 1));
        var events = new List<EngineHealthChanged>();
        session.HealthChanged += events.Add;

        session.ClearDegradedForMod("mod.a");

        session.Health.Kind.Should().Be(EngineHealthKind.Normal);
        events.Should().ContainSingle();
        events[0].Entered.Should().BeFalse();
        events[0].ResultingKind.Should().Be(EngineHealthKind.Normal);
    }
}
