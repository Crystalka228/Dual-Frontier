using System;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.1 Item 9 — managed binding для the native shared memory registry
/// (process-global). High-frequency cross-system IPC primitive distinct from
/// bus events и NativeWorld component storage. Single-process implementation
/// in К10.1; multi-process scope deferred к post-К-series milestones.
/// </summary>
public static class ShmRegionInterop
{
    /// <summary>Create a region. Returns false on duplicate region id or size &lt;= 0.</summary>
    public static bool Create(uint regionId, int sizeBytes)
        => NativeMethods.df_shm_create(regionId, sizeBytes) == 1;

    /// <summary>
    /// Map the region и return a writable pointer. Returns IntPtr.Zero if
    /// the region does not exist.
    /// </summary>
    public static IntPtr Map(uint regionId)
    {
        unsafe { return new IntPtr(NativeMethods.df_shm_map(regionId)); }
    }

    /// <summary>Get the size in bytes of the region (0 if not created).</summary>
    public static int Size(uint regionId)
        => NativeMethods.df_shm_size(regionId);

    /// <summary>Unmap the region (no-op in single-process implementation).</summary>
    public static bool Unmap(uint regionId)
        => NativeMethods.df_shm_unmap(regionId) == 1;

    /// <summary>Destroy the region. Returns true if removed.</summary>
    public static bool Destroy(uint regionId)
        => NativeMethods.df_shm_destroy(regionId) == 1;

    /// <summary>Declare the writer system для the region.</summary>
    public static bool RegisterWriter(uint regionId, uint writerSystemId)
        => NativeMethods.df_shm_register_writer(regionId, writerSystemId) == 1;

    /// <summary>Get the registered writer (0 if unset / region absent).</summary>
    public static uint Writer(uint regionId)
        => (uint)NativeMethods.df_shm_writer(regionId);

    /// <summary>Count of registered regions.</summary>
    public static int RegionCount => NativeMethods.df_shm_region_count();

    /// <summary>Reset all regions.</summary>
    public static void Clear() => NativeMethods.df_shm_clear();
}
