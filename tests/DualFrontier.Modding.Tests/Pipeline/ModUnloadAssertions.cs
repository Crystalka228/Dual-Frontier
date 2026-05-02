using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// Test helper for verifying <see cref="System.Runtime.Loader.AssemblyLoadContext"/>
/// release after <c>ModIntegrationPipeline.UnloadMod</c> returns. Mirrors
/// the production spin pattern (non-inlined, GC pump bracket per
/// MOD_OS_ARCHITECTURE v1.4 §9.5 step 7) so test-side verification follows
/// the same discipline as the production helper. Reusable surface for M8+
/// fixture tests as their per-mod load/unload coverage grows.
/// </summary>
internal static class ModUnloadAssertions
{
    /// <summary>
    /// Spins on <paramref name="alcRef"/>'s <c>IsAlive</c> with the
    /// mandatory <c>GC.Collect → WaitForPendingFinalizers → Collect</c>
    /// double-collect bracket each iteration, returning normally when
    /// the WR has been released. Throws an
    /// <see cref="Xunit.Sdk.XunitException"/> if the spin reaches
    /// <paramref name="timeoutMs"/> without observing release.
    ///
    /// Marked <see cref="MethodImplOptions.NoInlining"/> so the JIT
    /// cannot fold the WR's referent (the
    /// <see cref="System.Runtime.Loader.AssemblyLoadContext"/> instance)
    /// into the caller's stack frame via the WR boxing path. Pairs with
    /// the production helpers in <c>ModIntegrationPipeline</c> which
    /// follow the same convention.
    /// </summary>
    /// <param name="alcRef">
    /// <see cref="WeakReference"/> to the mod's
    /// <c>ModLoadContext</c>. Capture with
    /// <c>new WeakReference(loadedMod.Context)</c> through the
    /// <c>GetActiveModForTests</c> seam BEFORE invoking
    /// <c>pipeline.UnloadMod</c>; release every local strong ref to
    /// the <c>LoadedMod</c> before the assertion runs.
    /// </param>
    /// <param name="timeoutMs">
    /// Maximum time to wait for release. Default matches the production
    /// §9.5 step 7 cadence (10 s / 100 iterations × 100 ms).
    /// </param>
    /// <param name="context">
    /// Optional context string appended to the failure message — pass
    /// the fixture name so failed assertions name the offender.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AssertAlcReleasedWithin(
        WeakReference alcRef,
        int timeoutMs = 10_000,
        string? context = null)
    {
        if (alcRef is null) throw new ArgumentNullException(nameof(alcRef));

        int maxIter = timeoutMs / 100;
        for (int i = 0; i < maxIter; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            if (!alcRef.IsAlive) return;

            Thread.Sleep(100);
        }

        throw new Xunit.Sdk.XunitException(
            $"ALC reference still alive after {timeoutMs} ms" +
            (context is null ? "." : $" ({context})."));
    }
}
