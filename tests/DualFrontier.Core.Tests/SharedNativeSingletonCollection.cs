using Xunit;

namespace DualFrontier.Core.Tests;

/// <summary>
/// Shared xUnit collection for every test class that mutates a process-global
/// native singleton — the scheduler graph, the wake registry, the scheduling
/// policies registry, the event-type registry, or the native event bus. All
/// such classes carry <c>[Collection("SharedNativeSingleton")]</c>, which
/// serialises them so that no two ever run in parallel.
///
/// This closes the F-29(a) cross-class data race. An xUnit <c>[Collection]</c>
/// serialises only its own members; the race arose because
/// <see cref="Scheduling.SchedulerStressTests"/> (default collection) and
/// <see cref="Scheduling.SchedulerExtremeTests"/> (formerly
/// <c>[Collection("ExtremeSerial")]</c>) sat in DIFFERENT collections and thus
/// ran concurrently, so one class's <c>Clear()</c>/<c>df_bus_clear()</c> raced
/// the other's tick/publish loop over the same lock-free singleton. One shared
/// collection is the structural fix; the native fail-loud concurrency detector
/// (system_graph / wake_registry) is the durable net that makes an incomplete
/// membership fail loud instead of corrupting memory.
///
/// See TESTING_STRATEGY.md — shared-native-singleton test-isolation law.
///
/// Current membership: SchedulerStressTests, SchedulerExtremeTests,
/// ManagedBusBridgeTests. A new test class that mutates any of the singletons
/// above MUST join this collection.
///
/// <para><c>DisableParallelization = true</c>: these are the heaviest suites in
/// the project — the scheduler/bus ceiling probes oversubscribe the thread pool
/// (S4 alone fans out to 64 threads on a 16-core host) and churn the native heap
/// under multi-million-event load. Left parallel with other collections, that
/// concurrent load intermittently corrupts the native heap and crashes the test
/// host (F-29 investigation, 2026-07-04: the heavy tests pass in isolation but
/// the full suite crashes 3/3; isolating this collection makes it green). The
/// flag runs the collection in isolation from every other collection — the same
/// device GameBootstrapIntegrationTests uses for its GameLoopSerial collection.
/// It also removes the thread-pool contention that failed
/// ManagedScheduler's fan-out assertion under a saturated pool.</para>
/// </summary>
[CollectionDefinition("SharedNativeSingleton", DisableParallelization = true)]
public sealed class SharedNativeSingletonCollection
{
    // Marker only — no ICollectionFixture. Each member class performs its own
    // singleton reset in its ctor/Dispose; the collection provides serialisation,
    // not shared setup state.
}
