using System;

namespace DualFrontier.Contracts.Scheduling;

/// <summary>
/// K10.3 v2 Item 37 — subscribes the system к pipeline slot transitions
/// (FenceCompleted → ReadableAsTail). К-L13 on-demand activation extended
/// с pipeline lifecycle signal.
///
/// К-L13 5-wake-type model (Timer/Event/StateChange/Init/Explicit) unchanged.
/// Slot transitions are pipeline lifecycle events, not а standard wake type —
/// per К-L14 architectural cleanness, kept as separate primitive orthogonal к
/// state_change_filter component-type filtering.
///
/// К10.3 v2 boundary: attribute landed для consumer surface; full subscriber
/// registry integration (subscribed system tracking + wake payload delivery via
/// pipeline transition hook) deferred к К-extensions when pipeline-managed
/// wake consumers surface. Pipeline transition fire counter
/// (PipelineSlotInterop.GetWakeFireCount) provides observability surface
/// для К10.3 v2 scope.
///
/// Future К-extensions consumer pattern:
/// <code>
/// [WakeOnSlotTransition(SlotOffset = -1)]  // wake on tail transition
/// public class MovementReactSystem : SystemBase {
///     public override void Update(float delta) {
///         // Read field state from slot tail (К-L7.1)
///     }
/// }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WakeOnSlotTransitionAttribute : Attribute
{
    /// <summary>
    /// Slot offset к subscribe к transitions on. 0 = current (most recently
    /// allocated). -1 = tail (К-L7.1 sim-thread read pattern). -(D-1) = display tail.
    /// Default 0.
    /// </summary>
    public int SlotOffset { get; init; } = 0;
}
