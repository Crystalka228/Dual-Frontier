using System;
using DualFrontier.Contracts.Scheduling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.1 Item 3 — managed binding for the native wake registry (К-L13
/// on-demand system activation). Wraps the process-global default registry
/// singleton exposed via <c>df_wake_registry_*</c> C ABI.
///
/// Five wake types: Timer (periodic by [TickRate]), Event (bus publication),
/// StateChange (component value crossing), Init (one-shot at startup),
/// Explicit (API-driven). See <see cref="WakeType"/> для enum values.
///
/// K10.1 scope: subscription tables + runqueue. Bus integration (Event wake
/// payload delivery) lands в К10.2; condition evaluation for StateChange
/// extends с the Item 17 write-through hook.
/// </summary>
public static class WakeRegistryInterop
{
    /// <summary>Subscribe to a TimerWake at the given tick rate.</summary>
    public static bool SubscribeTimer(uint systemId, uint ticksPerUpdate)
        => NativeMethods.df_wake_registry_subscribe_timer(systemId, ticksPerUpdate) == 1;

    /// <summary>Subscribe to an EventWake on the given event type id.</summary>
    public static bool SubscribeEvent(uint systemId, uint eventTypeId)
        => NativeMethods.df_wake_registry_subscribe_event(systemId, eventTypeId) == 1;

    /// <summary>Subscribe к a StateChangeWake on the given component type id.</summary>
    public static bool SubscribeState(uint systemId, uint componentTypeId)
        => NativeMethods.df_wake_registry_subscribe_state(systemId, componentTypeId) == 1;

    /// <summary>Subscribe к the one-shot InitWake.</summary>
    public static bool SubscribeInit(uint systemId)
        => NativeMethods.df_wake_registry_subscribe_init(systemId) == 1;

    /// <summary>Subscribe к an ExplicitWake paired with the given wake id token.</summary>
    public static bool SubscribeExplicit(uint systemId, uint wakeId)
        => NativeMethods.df_wake_registry_subscribe_explicit(systemId, wakeId) == 1;

    /// <summary>
    /// Remove all subscriptions of the given wake type for the given system.
    /// Returns the count of subscriptions removed (0 if none matched).
    /// </summary>
    public static int Unsubscribe(uint systemId, WakeType type)
        => NativeMethods.df_wake_registry_unsubscribe(systemId, (int)type);

    /// <summary>Fire all timer subscriptions whose rate divides the current tick.</summary>
    public static int FireTimer(ulong currentTick)
        => NativeMethods.df_wake_registry_fire_timer(currentTick);

    /// <summary>Fire event wake к subscribers of the given event type id.</summary>
    public static int FireEvent(uint eventTypeId)
        => NativeMethods.df_wake_registry_fire_event(eventTypeId);

    /// <summary>Fire state-change wake к subscribers of the given component type id.</summary>
    public static int FireStateChange(uint componentTypeId, uint entityId)
        => NativeMethods.df_wake_registry_fire_state_change(componentTypeId, entityId);

    /// <summary>Fire the one-shot init wake. Subsequent calls fire zero subscribers.</summary>
    public static int FireInit() => NativeMethods.df_wake_registry_fire_init();

    /// <summary>Fire an explicit wake к the matching (system, wake_id) subscription.</summary>
    public static int FireExplicit(uint targetSystemId, uint wakeId)
        => NativeMethods.df_wake_registry_fire_explicit(targetSystemId, wakeId);

    /// <summary>Current runqueue size (count of woken system ids awaiting drain).</summary>
    public static int RunqueueSize => NativeMethods.df_wake_registry_runqueue_size();

    /// <summary>
    /// Drain the runqueue into an array of system ids (sorted ascending) and
    /// clear it. Returns the ids that fired since the last drain.
    /// </summary>
    public static uint[] DrainRunqueue()
    {
        int n = NativeMethods.df_wake_registry_runqueue_size();
        if (n <= 0)
        {
            // Even empty runqueue should still flush native state.
            unsafe { NativeMethods.df_wake_registry_drain_runqueue(null, 0); }
            return Array.Empty<uint>();
        }
        uint[] buffer = new uint[n];
        unsafe
        {
            fixed (uint* p = buffer)
            {
                NativeMethods.df_wake_registry_drain_runqueue(p, n);
            }
        }
        return buffer;
    }

    /// <summary>Count of registered subscriptions для the given wake type.</summary>
    public static int SubscriptionCount(WakeType type)
        => NativeMethods.df_wake_registry_subscription_count((int)type);

    /// <summary>Reset all subscriptions + runqueue.</summary>
    public static void Clear() => NativeMethods.df_wake_registry_clear();
}
