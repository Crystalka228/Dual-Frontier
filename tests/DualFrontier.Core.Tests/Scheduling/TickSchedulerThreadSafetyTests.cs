using System.Threading.Tasks;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

/// <summary>
/// Regression guard for the cache race that
/// <c>TickScheduler.ShouldRun</c> exhibited prior to
/// commit <c>e0b0ecf</c>. <see cref="ParallelSystemScheduler.ExecutePhase"/>
/// invokes <c>ShouldRun</c> from inside <c>Parallel.ForEach</c>; on the
/// first tick the cache is empty and worker threads race to populate it.
/// With a non-concurrent <c>Dictionary&lt;Type, int&gt;</c> this race
/// surfaces as <c>InvalidOperationException("Operations that change
/// non-concurrent collections must have exclusive access")</c> when the
/// dictionary resizes during concurrent inserts. The fix swaps the field
/// to <c>ConcurrentDictionary&lt;Type, int&gt;</c> with <c>GetOrAdd</c>;
/// this test exercises the race window directly so that any future
/// regression on the storage type is caught at the unit level.
/// </summary>
public sealed class TickSchedulerThreadSafetyTests
{
    [Fact]
    public void ShouldRun_InvokedFromManyThreadsWithEmptyCache_DoesNotThrowAndReturnsConsistentResults()
    {
        var scheduler = new TickScheduler();

        // 32 distinct SystemBase subclasses — each is a separate cache key,
        // so concurrent first-tick population forces the underlying
        // dictionary through one or more resizes (capacity grows
        // 0 → 3 → 7 → 17 → ...). This is when the non-concurrent
        // Dictionary's invariants break under parallel writers.
        SystemBase[] systems = BuildSyntheticSystemPool();

        // Repeat the race window many times. Each iteration starts with an
        // empty cache (Reset()) and dispatches ShouldRun across the full
        // pool in parallel; the more iterations, the higher the chance any
        // surviving race surfaces. 500 iterations was empirically chosen
        // so the test reliably fails (>90%) against the pre-fix
        // Dictionary-based cache while remaining sub-second against the
        // ConcurrentDictionary-based fix.
        for (int iteration = 0; iteration < 500; iteration++)
        {
            scheduler.Reset();

            var results = new bool[systems.Length];

            Parallel.For(0, systems.Length, i =>
            {
                results[i] = scheduler.ShouldRun(systems[i]);
            });

            // Re-query each system serially; the results must agree with
            // the parallel pass. Any thread that observed a corrupt cache
            // entry (e.g. mid-resize) would return the wrong value or
            // throw — both surface here.
            for (int i = 0; i < systems.Length; i++)
            {
                bool expected = scheduler.ShouldRun(systems[i]);
                results[i].Should().Be(
                    expected,
                    "parallel ShouldRun must agree with serial ShouldRun on iteration {0} system index {1}",
                    iteration,
                    i);
            }
        }
    }

    private static SystemBase[] BuildSyntheticSystemPool() =>
        new SystemBase[]
        {
            new S00(), new S01(), new S02(), new S03(),
            new S04(), new S05(), new S06(), new S07(),
            new S08(), new S09(), new S10(), new S11(),
            new S12(), new S13(), new S14(), new S15(),
            new S16(), new S17(), new S18(), new S19(),
            new S20(), new S21(), new S22(), new S23(),
            new S24(), new S25(), new S26(), new S27(),
            new S28(), new S29(), new S30(), new S31(),
        };

    // ── Synthetic system types ──────────────────────────────────────────────
    // 32 distinct concrete SystemBase subclasses. Each is a separate Type
    // and therefore a separate cache key in TickScheduler. [SystemAccess]
    // is intentionally omitted: this test never goes through
    // ParallelSystemScheduler.BuildContext, only TickScheduler.ShouldRun,
    // which only inspects the runtime type for [TickRate].

    [TickRate(1)] private sealed class S00 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S01 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S02 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S03 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S04 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S05 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S06 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S07 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S08 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S09 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S10 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S11 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S12 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S13 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S14 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S15 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S16 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S17 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S18 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S19 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S20 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S21 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S22 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S23 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S24 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S25 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S26 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S27 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S28 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S29 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S30 : SystemBase { public override void Update(float delta) { } }
    [TickRate(1)] private sealed class S31 : SystemBase { public override void Update(float delta) { } }
}
