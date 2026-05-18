using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.2 Item 32 — Native unload primitive managed binding
/// (S3-Q1 L3 layering, S3-Q6 single primitive contract).
///
/// Mirrors <c>ModUnloadResult</c> struct from <c>native/include/mod_unload.h</c>
/// verbatim — fixed-size error_messages array allows blittable
/// <see cref="LayoutKind.Sequential"/> marshalling без custom marshaller.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ModUnloadResult
{
    public int Success;

    public int FastSubscriptionsCleared;
    public int FastInFlightDropped;

    public int NormalSubscriptionsCleared;
    public int NormalEventsDrained;
    public int NormalBatchCommitCompleted;

    public int BackgroundSubscriptionsCleared;
    public int BackgroundEventsPreserved;
    public int BackgroundSubscriberCountRemaining;

    public int CapabilitiesRevoked;

    public int WakeSubscriptionsCleared;

    // 8 fixed-size 256-byte error message slots — char[8][256] в native layout.
    // Total: 2048 bytes.
    public fixed byte ErrorMessages[8 * 256];

    public int ErrorCount;

    /// <summary>
    /// Reads the i-th error message as a managed string (UTF-8 / ASCII;
    /// null-terminated per native side).
    /// </summary>
    public string GetErrorMessage(int index)
    {
        if (index < 0 || index >= ErrorCount) return string.Empty;
        fixed (byte* errors = ErrorMessages)
        {
            byte* slot = errors + (index * 256);
            int len = 0;
            while (len < 256 && slot[len] != 0) ++len;
            return Encoding.UTF8.GetString(slot, len);
        }
    }
}

/// <summary>
/// K10.2 Item 32 — P/Invoke binding for <c>df_scheduler_unload_mod_native_state</c>.
/// Called by <c>ModIntegrationPipeline</c> Step 3.5 (Commit 11).
/// </summary>
public static class ModUnloadInterop
{
    private const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int df_scheduler_set_sim_paused(int paused);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int df_scheduler_is_sim_paused();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int df_scheduler_unload_mod_native_state(
        uint mod_id, out ModUnloadResult out_result);

    public static bool SetSimPaused(bool paused)
        => df_scheduler_set_sim_paused(paused ? 1 : 0) == 1;

    public static bool IsSimPaused()
        => df_scheduler_is_sim_paused() == 1;

    /// <summary>
    /// Invokes the native unload primitive для <paramref name="modId"/>.
    /// Returns <see langword="true"/> if T0-T7 sequence completed (Result.Success=1);
    /// <see langword="false"/> on К-L18 precondition violation or fault
    /// (Result.ErrorMessages contains diagnostics).
    /// </summary>
    public static bool UnloadModNativeState(uint modId, out ModUnloadResult result)
    {
        int rc = df_scheduler_unload_mod_native_state(modId, out result);
        return rc == 1 && result.Success == 1;
    }
}
