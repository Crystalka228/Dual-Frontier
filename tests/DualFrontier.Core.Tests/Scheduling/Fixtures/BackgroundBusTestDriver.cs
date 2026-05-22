using System.Runtime.InteropServices;

namespace DualFrontier.Core.Tests.Scheduling.Fixtures;

/// <summary>
/// Test-only P/Invoke wrapper for the native Background-tier bus surface that
/// production managed code does not currently expose.
///
/// Why this exists: the Background tier in <c>bus_native</c> +
/// <c>background_queue</c> is fully implemented natively (К10.2 Item 30),
/// but the production dispatch hook — `df_background_queue_dispatch_idle_slot` —
/// has zero call sites in managed code as of 2026-05-21. Background events
/// are queued but never dispatched. Per design intent the consumers are
/// vanilla mods that aren't yet implemented; the native side is ready
/// ahead of them.
///
/// This driver gives stress / extreme tests a sanctioned path to invoke the
/// native dispatch directly so they can verify the queue's behaviour
/// end-to-end without waiting for the vanilla mod layer.
///
/// Lives in tests/ only — production code is untouched.
/// </summary>
internal static class BackgroundBusTestDriver
{
    private const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_dispatch_idle_slot(ulong available_budget_micros);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_size(out uint out_event_count, out uint out_bytes_used);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_force_coalesce();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_saturation_events();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_configure(
        uint max_bytes,
        uint warn_threshold_bytes,
        int strategy);

    /// <summary>
    /// Drains the background queue and dispatches events to subscribers.
    /// Pass 0 for the budget to dispatch with no time limit (drains entire queue).
    /// Returns the count of events dispatched.
    /// </summary>
    public static int DispatchIdleSlot(ulong availableBudgetMicros = 0)
        => df_background_queue_dispatch_idle_slot(availableBudgetMicros);

    /// <summary>
    /// Reads the current pending background queue size — number of events
    /// queued, and total bytes used. Useful before/after coalesce to verify
    /// the merge behaviour.
    /// </summary>
    public static (uint EventCount, uint BytesUsed) GetQueueSize()
    {
        df_background_queue_size(out uint count, out uint bytes);
        return (count, bytes);
    }

    /// <summary>
    /// Forces immediate coalesce of the pending queue per (type_id, coalesce_key).
    /// In production the coalesce step happens implicitly at dispatch time;
    /// tests that need to observe the post-coalesce state without dispatching
    /// can call this directly.
    /// </summary>
    public static int ForceCoalesce() => df_background_queue_force_coalesce();

    /// <summary>Count of times drop-oldest saturation triggered since process start.</summary>
    public static int SaturationEvents() => df_background_queue_saturation_events();

    /// <summary>
    /// Configures size cap + saturation strategy. Defaults: 10 MB cap,
    /// 80 % warn threshold, drop-oldest.
    /// </summary>
    public static int Configure(uint maxBytes, uint warnThresholdBytes, int strategy)
        => df_background_queue_configure(maxBytes, warnThresholdBytes, strategy);
}
