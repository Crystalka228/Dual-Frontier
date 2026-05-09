using System;

namespace DualFrontier.Core.Benchmarks.TickLoop;

/// <summary>
/// K7 abstract — common shape across V1/V2/V3 tick-loop scenarios. Each
/// concrete impl owns its own storage backend (managed <c>World</c> with
/// class components for V1, managed <c>World</c> with struct components
/// for V2, <c>NativeWorld</c> for V3) and its own system harness that
/// runs against that storage. The benchmark / long-run runners drive the
/// abstract surface — Setup → 10,000 × ExecuteTick → Teardown — so the
/// driver is variant-agnostic.
/// </summary>
internal abstract class TickLoopScenarioBase : IDisposable
{
    /// <summary>
    /// Spawns the workload (50 pawns, 255 items per K7 brief §1.2) using
    /// <paramref name="seed"/>-derived deterministic placement. Must be
    /// called exactly once per scenario instance, before
    /// <see cref="ExecuteTick"/>. Implementations that build a managed
    /// scheduler must run <c>InitializeAllSystems</c> here so the systems
    /// reach their post-Initialize state before the first measured tick.
    /// </summary>
    public abstract void SetupWorld(int pawnCount, int seed);

    /// <summary>
    /// Runs one tick of the simulation at <paramref name="delta"/> seconds.
    /// Called 10,000 times by the long-run runner; called inside BDN's
    /// measurement loop by the BDN harness. Must be non-allocating beyond
    /// what the scenario's storage layer naturally allocates — the long-run
    /// numbers depend on this.
    /// </summary>
    public abstract void ExecuteTick(float delta);

    /// <summary>
    /// Releases scenario-owned resources. Critical for V3 (NativeWorld
    /// disposal) and harmless for V1/V2. <see cref="IDisposable.Dispose"/>
    /// delegates here so the driver can use a <c>using</c> block.
    /// </summary>
    public abstract void TeardownWorld();

    public void Dispose() => TeardownWorld();
}
