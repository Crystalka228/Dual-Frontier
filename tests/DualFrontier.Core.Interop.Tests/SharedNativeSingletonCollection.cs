using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// EQ_A2 / D9 — serialising collection for the Interop.Tests classes that mutate a
/// process-global native singleton (the <c>g_sim_paused</c> mod-unload flag, the
/// native event-bus tiers, or the pipeline-slot ring). Cross-CLASS parallelism over
/// that shared state was the measured F-40 exposure:
/// <see cref="ModUnloadInteropTests"/>'s vacuous unload (requires sim-paused = true
/// and reads bus-tier state) intermittently failed when a sibling class's
/// <c>df_bus_clear</c> / sim-paused flip / pipeline mutation raced it. xUnit runs a
/// class's own methods sequentially but parallelises ACROSS classes; a single
/// <c>[Collection("SharedNativeSingleton")]</c> with <c>DisableParallelization</c>
/// serialises the racing classes so none run concurrently.
///
/// Marker only (no ICollectionFixture) — each member resets its own native state in
/// its test bodies; the collection provides serialisation, not shared setup. Mirrors
/// the DualFrontier.Core.Tests device of the same name. A new interop class that
/// mutates any of those singletons MUST join this collection.
/// </summary>
[CollectionDefinition("SharedNativeSingleton", DisableParallelization = true)]
public sealed class SharedNativeSingletonCollection
{
}
