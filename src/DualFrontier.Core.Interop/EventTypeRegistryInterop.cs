using System;
using System.Runtime.InteropServices;
using DualFrontier.Contracts.Bus;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed binding for К10.2 Item 28 — native event type registry (per-event
/// tier metadata; consumed by native bus dispatch per К-L15).
///
/// The native registry is process-global and read-only on the hot path
/// (lookups during publish). Registrations happen at startup и at mod load
/// time под the scheduler critical section (Step 3.5 atomicity is reserved
/// for the inverse operation — teardown).
/// </summary>
public static class EventTypeRegistryInterop
{
    private const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl,
               CharSet = CharSet.Ansi)]
    private static extern int df_event_type_registry_register(
        uint type_id,
        int tier,
        uint payload_size_bytes,
        IntPtr fqn_utf8,
        IntPtr coalesce_fn);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_event_type_registry_get_tier(
        uint type_id,
        out int out_tier);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_event_type_registry_count();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void df_event_type_registry_clear();

    /// <summary>
    /// Registers a managed event type with the native registry. Caller is
    /// responsible for keeping the UTF-8 FQN buffer alive for the registry's
    /// lifetime (typically interned strings via <see cref="Marshal.StringToHGlobalAnsi(string)"/>
    /// that are never freed).
    /// </summary>
    /// <returns><see langword="true"/> on success; <see langword="false"/> on
    /// invalid args or conflicting re-registration.</returns>
    public static bool Register(uint typeId, BusTier tier, int payloadSizeBytes, string fqn, IntPtr coalesceFnPtr = default)
    {
        if (fqn is null) throw new ArgumentNullException(nameof(fqn));
        if (payloadSizeBytes < 0) throw new ArgumentOutOfRangeException(nameof(payloadSizeBytes));
        // Intern the FQN: the native registry stores the raw pointer, so the
        // buffer must outlive every lookup. Allocating via Marshal.StringToHGlobalAnsi
        // and never freeing matches the «process-global registry» invariant.
        IntPtr fqnPtr = Marshal.StringToHGlobalAnsi(fqn);
        int rc = df_event_type_registry_register(
            typeId, (int)tier, (uint)payloadSizeBytes, fqnPtr, coalesceFnPtr);
        if (rc != 1)
        {
            // Re-registration с identical metadata returns 1; only true failures
            // reach here. Free the buffer we just allocated to avoid leaking.
            Marshal.FreeHGlobal(fqnPtr);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Reads the dispatch tier for a registered type. Unregistered type IDs
    /// return <see cref="BusTier.Normal"/> per S-LOCK-4 backward compatibility.
    /// </summary>
    public static BusTier GetTier(uint typeId)
    {
        int rc = df_event_type_registry_get_tier(typeId, out int tier);
        return rc == 1 ? (BusTier)tier : BusTier.Normal;
    }

    /// <summary>Count of registered event types (test/diagnostic surface).</summary>
    public static int Count => df_event_type_registry_count();

    /// <summary>Test surface: clears the registry. Not safe for production use.</summary>
    public static void ClearForTesting() => df_event_type_registry_clear();
}
