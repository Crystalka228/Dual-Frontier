using System.Runtime.InteropServices;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.1 scheduler P/Invoke surface. Mirrors the <c>df_scheduler_*</c> block at
/// the end of <c>native/DualFrontier.Core.Native/include/df_capi.h</c> 1:1.
/// </summary>
internal static partial class NativeMethods
{
    // ----- K10.1 Item 1 — system graph -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_register_system(
        uint systemId,
        byte* systemFqn,
        uint* readComponentIds,
        uint readCount,
        uint* writeComponentIds,
        uint writeCount,
        int priorityClass,
        int wakeType);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_unregister_system(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_system_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_clear();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_compute_static_graph();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_static_phase_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_static_phase_size(int phaseIndex);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_static_phase_systems(
        int phaseIndex,
        uint* outSystemIds,
        int outCapacity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_compute_per_tick_graph(
        uint* runnableIds,
        uint runnableCount);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_per_tick_phase_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_per_tick_phase_size(int phaseIndex);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_per_tick_phase_systems(
        int phaseIndex,
        uint* outSystemIds,
        int outCapacity);

    // ----- K10.1 Item 3 — wake registry -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_subscribe_timer(uint systemId, uint ticksPerUpdate);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_subscribe_event(uint systemId, uint eventTypeId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_subscribe_state(uint systemId, uint componentTypeId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_subscribe_init(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_subscribe_explicit(uint systemId, uint wakeId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_unsubscribe(uint systemId, int wakeType);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_fire_timer(ulong currentTick);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_fire_event(uint eventTypeId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_fire_state_change(uint componentTypeId, uint entityId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_fire_init();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_fire_explicit(uint targetSystemId, uint wakeId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_runqueue_size();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_wake_registry_drain_runqueue(uint* outSystemIds, int outCapacity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_wake_registry_subscription_count(int wakeType);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_wake_registry_clear();

    // ----- K10.1 Item 4 — diagnostic API -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_query_runnable(uint* outSystemIds, int outCapacity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_query_wake_subscriptions(uint systemId);

    // ----- K10.1 Item 5 — per-tick orchestration -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_tick_begin(ulong currentTick);

    // ----- K10.1 Items 6+7+8 — scheduling policies -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_set(
        uint systemId,
        int schedulingClass,
        int maxLatencyMicros,
        int maxJitterMicros,
        int cpuQuotaMicrosPerTick,
        int preemptionMode);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_get_class(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_get_quota(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_record_execution(uint systemId, long micros);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_quota_exceeded(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long df_scheduler_policies_total_micros(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_quota_violations(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_policies_reset_tick_stats();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_policies_order_by_priority(
        uint* inIds, uint inCount,
        uint* outIds, int outCapacity);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_policies_clear();

    // ----- K10.1 Item 9 — shared memory regions -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_shm_create(uint regionId, int sizeBytes);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void* df_shm_map(uint regionId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_shm_size(uint regionId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_shm_unmap(uint regionId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_shm_destroy(uint regionId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_shm_register_writer(uint regionId, uint writerSystemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_shm_writer(uint regionId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_shm_region_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_shm_clear();

    // ----- K10.1 Items 11+12+13 — affinity / work stealing / phase barriers -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_set_affinity(uint systemId, int affinityCoreId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_policies_get_affinity(uint systemId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_work_stealing_enabled();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_set_work_stealing_enabled(int enabled);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_set_phase_barrier(int phaseIndex, int barrierType);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_get_phase_barrier(int phaseIndex);

    // ----- K10.1 Item 15 — batched callback ABI -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_scheduler_register_managed_callback(
        delegate* unmanaged[Cdecl]<NativeManagedBatch*, void> cb,
        void* userData);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_scheduler_dispatch_managed_batch(NativeManagedBatch* batch);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_scheduler_managed_callback_registered();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_clear_managed_callback();

    // ----- K10.1 Item 17 — state change filter (test isolation needs clear) -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_state_filter_clear();

    // ----- K10.1 Items 19+20 — scheduler trace + intrinsics (test isolation) -----

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_trace_set_enabled(int enabled);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_trace_clear();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_scheduler_intrinsics_reset();
}
