using Xunit;

namespace DualFrontier.Core.Tests;

/// <summary>
/// Shared xUnit collection for every test class that mutates a process-global
/// native singleton: the scheduler graph, the wake registry, the scheduling
/// policies registry, the event-type registry, or the native event bus. All
/// such classes carry <c>[Collection("SharedNativeSingleton")]</c>, which
/// serialises them so that no two ever run in parallel.
///
/// An xUnit <c>[Collection]</c> serialises only its own members. Two classes
/// that touch the same singleton from DIFFERENT collections run concurrently,
/// and one class's <c>Clear()</c>/<c>df_bus_clear()</c> then races the other's
/// tick/publish loop over the same lock-free singleton. One shared collection
/// removes that cross-class race structurally; the native fail-loud concurrency
/// detector (system_graph / wake_registry) makes an incomplete membership fail
/// loud instead of corrupting memory.
///
/// See TESTING_STRATEGY.md section 2.8, the shared-native-singleton
/// test-isolation law.
///
/// Current membership: SchedulerStressTests, SchedulerExtremeTests,
/// ManagedBusBridgeTests. A new test class that mutates any of the singletons
/// above MUST join this collection.
///
/// <para><c>DisableParallelization = true</c>: these are the heaviest suites in
/// the project. The scheduler and bus ceiling probes oversubscribe the thread
/// pool and churn the native heap under multi-million-event load; run in
/// parallel with other collections, that load intermittently corrupts the
/// native heap and crashes the test host, while the same tests pass in
/// isolation. The flag runs the collection in isolation from every other
/// collection, which also removes the thread-pool contention that fails
/// ManagedScheduler's fan-out assertion under a saturated pool
/// (TESTING_STRATEGY section 2.8).</para>
/// </summary>
[CollectionDefinition("SharedNativeSingleton", DisableParallelization = true)]
public sealed class SharedNativeSingletonCollection
{
    // Marker only: no ICollectionFixture. Each member class performs its own
    // singleton reset in its ctor/Dispose; the collection provides serialisation,
    // not shared setup state.
}
