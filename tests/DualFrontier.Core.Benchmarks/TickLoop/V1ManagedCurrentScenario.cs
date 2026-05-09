using System;

namespace DualFrontier.Core.Benchmarks.TickLoop;

/// <summary>
/// K7 V1 scenario STUB — pre-K4 baseline (managed <see cref="DualFrontier.Core.ECS.World"/>
/// with class-based components). Cannot be implemented from <c>main</c>
/// because every component the production scenario uses (HealthComponent,
/// NeedsComponent, PositionComponent, …) is a struct on <c>main</c>; the
/// pre-K4 reality has them as classes with different mutation idioms.
///
/// Per K7 brief §1.6 LOCKED Option c, the V1 scenario is implemented inside
/// a temporary worktree at the pre-K4 commit. The real implementation
/// replaces this stub during Phase 3.3 of the brief execution; this file
/// exists in <c>main</c> as a placeholder so:
///
/// 1. <see cref="TickLoopScenarioBase"/> has a complete V1/V2/V3 trio in
///    the source tree from K7 brief authoring time, not just from V1
///    measurement time.
/// 2. <c>Program.cs</c> can compile a stable <c>--bdn-tick V1</c> /
///    <c>--long-run V1</c> dispatch path even before the worktree run.
///    The dispatch on <c>main</c> throws a clear <c>NotSupportedException</c>
///    instructing the operator to drive V1 from the worktree.
/// 3. The brief's expected file layout (<c>TickLoop/V1ManagedCurrentScenario.cs</c>)
///    is preserved so the worktree copy step is a single
///    <c>tests/DualFrontier.Core.Benchmarks</c> tree replication.
///
/// When the brief execution reaches Phase 3.3:
///   1. The benchmark project tree is copied into the worktree.
///   2. Inside the worktree, the executor reads pre-K4 component shapes
///      and replaces this stub's <see cref="SetupWorld"/> /
///      <see cref="ExecuteTick"/> / <see cref="TeardownWorld"/> bodies
///      with real implementations against the pre-K4 API surface.
///   3. The worktree's build runs cleanly; benchmarks measure V1.
///   4. Outputs (CSVs) copy back to <c>main</c>'s
///      <c>docs/benchmarks/k7-bdn-V1*.csv</c>
///      / <c>k7-long-run-V1.csv</c>.
///   5. The worktree is removed; this stub remains on <c>main</c> as the
///      historical placeholder.
///
/// Per K7 brief stop condition #6, if pre-K4 API drift is too severe to
/// reproduce the full vanilla system set inside the worktree, the V1
/// scenario simplifies to a managed-class equivalent of just the depletion
/// + mood + power loops V3 exercises. The divergence is noted in the
/// report's caveats section.
/// </summary>
internal sealed class V1ManagedCurrentScenario : TickLoopScenarioBase
{
    public override void SetupWorld(int pawnCount, int seed)
    {
        throw new NotSupportedException(
            "V1ManagedCurrentScenario is a stub on `main`. The actual V1 " +
            "implementation lives in a worktree at the pre-K4 commit per " +
            "K7 brief §1.6 LOCKED Option c. Drive V1 measurements from " +
            "that worktree, not from `main`.");
    }

    public override void ExecuteTick(float delta)
    {
        throw new NotSupportedException(
            "V1ManagedCurrentScenario.ExecuteTick is unreachable on `main`.");
    }

    public override void TeardownWorld()
    {
        // No-op — Setup throws before any state is allocated, so there
        // is nothing to tear down. Keeps the IDisposable contract honest.
    }
}
