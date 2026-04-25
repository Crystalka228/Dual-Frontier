namespace DualFrontier.Core.Bus;

/// <summary>
/// Internal capability implemented by aggregators that own one or more
/// <see cref="DomainEventBus"/> instances. Lets <c>ParallelSystemScheduler</c>
/// drain queued <c>[Deferred]</c> events at every phase boundary without
/// downcasting to a concrete service type. Not exposed on the public
/// <c>IGameServices</c> surface — deferred drain is a scheduler concern, not a
/// system or mod concern.
/// </summary>
internal interface IDeferredFlush
{
    /// <summary>
    /// Drains all deferred events queued in every owned bus, dispatching each
    /// event to its current subscribers with the original subscription's
    /// captured <c>SystemExecutionContext</c> re-pushed for the call.
    /// </summary>
    void FlushDeferred();
}
