namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Tracks the monotonically increasing game tick counter and answers whether a
/// given cadence is due on the current tick. Consumed by
/// <c>ParallelSystemScheduler</c> to filter systems before each phase.
///
/// This type holds a counter and does cadence arithmetic on it — nothing else.
/// It performs no reflection, allocates nothing, resolves no rates, and knows
/// nothing about systems or their types: the caller supplies the already-resolved
/// <c>ticksPerUpdate</c> it read from the scheduler's per-system metadata table
/// (F-60(a)).
///
/// Thread-safety: <c>_currentTick</c> is written only by the scheduler's driver
/// thread between phases via <see cref="Advance"/>, and read by
/// <see cref="ShouldRun"/> callers within a phase — including from inside
/// <c>Parallel.ForEach</c> over a phase's systems. That single-writer pattern is
/// the whole of this type's concurrency story; with no lazily populated state
/// left, there is nothing here for parallel dispatch to race on.
/// </summary>
internal sealed class TickScheduler
{
    private long _currentTick;

    /// <summary>
    /// The current tick number. Starts at 0 and is incremented by
    /// <see cref="Advance"/> once per full <c>ExecuteTick</c> call.
    /// </summary>
    public long CurrentTick => _currentTick;

    /// <summary>
    /// Increments the tick counter by 1. Called by
    /// <c>ParallelSystemScheduler.ExecuteTick</c> after every phase in the
    /// current tick has completed, so <see cref="ShouldRun"/> queries during
    /// the tick always see the same value.
    /// </summary>
    public void Advance()
    {
        _currentTick++;
    }

    /// <summary>
    /// Returns <c>true</c> if a system running at <paramref name="ticksPerUpdate"/>
    /// is due on the current tick.
    /// </summary>
    /// <param name="ticksPerUpdate">
    /// Cadence in ticks, which callers supply already normalised to a positive
    /// value — <c>SystemMetadataBuilder</c> normalises every rate it resolves,
    /// and <c>ParallelSystemScheduler</c> substitutes
    /// <c>TickRates.REALTIME</c> for systems absent from the metadata table.
    /// Deliberately unguarded: this sits inside the per-system, per-tick fan-out,
    /// and re-checking an invariant both producers already establish would buy
    /// nothing.
    /// </param>
    public bool ShouldRun(int ticksPerUpdate) => _currentTick % ticksPerUpdate == 0;
}
