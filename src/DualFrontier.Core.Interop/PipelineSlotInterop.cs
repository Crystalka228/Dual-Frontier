using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.3 v2 Item 33 — managed binding для native pipeline_slot state machine.
/// Wraps process-global pipeline (D=1-3 slots, default 2) per К-L16 pipeline depth.
///
/// К-L7.1 sub-invariant binding: PipelineSlot.FieldsSnapshotPtr holds opt-in
/// pipeline-managed FieldStorageSnapshot pointer; sim-thread reads of pipeline-
/// managed fields see slot-tail state (sim_tick - 1). К-L7 atomic-from-observer
/// preserved within slot boundary.
///
/// S-LOCK-10/13 coexistence: V1's existing dispatch_compute_field synchronous
/// path (К-L7) remains operational. Pipeline-managed dispatch path opt-in per
/// field per К-L9 «Vanilla = mods» author choice.
///
/// K-L18 forward consumer (Item 41): IsQuiescent surfaces к mod unload primitive
/// (ModUnloadInterop) для quiescent state precondition check before mod operations.
///
/// K10.3 v2 Commit 3 scope: state machine + cycling + tail offset reads. Fence
/// integration with real vkGetFenceStatus polling lands в Commit 4 (Phase.Compute);
/// until then ForceFenceCompleted advances state machine for test/integration
/// paths без VkFence dependency.
/// </summary>
public static class PipelineSlotInterop
{
    /// <summary>Per-К-L16 default pipeline depth (D=2).</summary>
    public const int DefaultDepth = 2;

    /// <summary>Per-К-L16 maximum configurable pipeline depth (D=3).</summary>
    public const int MaxDepth = 3;

    /// <summary>
    /// Slot state machine: Empty → Dispatched → FenceCompleted → ReadableAsTail
    /// (then recycles к Empty/Dispatched on next allocation). Per spec §3.10
    /// Item 33 verbatim.
    /// </summary>
    public enum SlotState
    {
        /// <summary>Initial state, no sim_tick assigned.</summary>
        Empty = 0,

        /// <summary>Sim thread dispatched compute work к GPU async compute queue.</summary>
        Dispatched = 1,

        /// <summary>GPU finished, results available (fence signaled).</summary>
        FenceCompleted = 2,

        /// <summary>Display/sim thread reads from here (К-L16 tail per К-L7.1).</summary>
        ReadableAsTail = 3
    }

    /// <summary>
    /// PipelineSlot struct verbatim from spec §3.10 Item 33.
    /// Layout matches native <c>PipelineSlot</c> (pipeline_slot.h): 8+8+8+8+4 = 36
    /// bytes user content + 4 bytes alignment padding = 40 bytes total (sequential layout).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PipelineSlot
    {
        public ulong SimTick;
        public nint WorldSnapshotPtr;
        public nint FieldsSnapshotPtr;
        public nint ComputeFenceHandle;
        public SlotState State;
        // Implicit 4-byte trailing pad для 8-byte struct alignment.
    }

    /// <summary>
    /// Initialize pipeline с specified depth (1-3 per К-L16, default 2 per S-LOCK-4).
    /// </summary>
    /// <returns>true on success; false if depth out-of-range or already initialized.</returns>
    public static bool Init(int depth = DefaultDepth)
        => NativeMethods.df_pipeline_init(depth) == 1;

    /// <summary>Reset pipeline state (test/teardown convenience).</summary>
    public static void Reset() => NativeMethods.df_pipeline_reset();

    /// <summary>Get current configured depth (0 = не initialized).</summary>
    public static int GetDepth()
    {
        unsafe
        {
            int d = 0;
            NativeMethods.df_pipeline_get_depth(&d);
            return d;
        }
    }

    /// <summary>
    /// Allocate next slot для simulation tick. Returns slot pointer на success;
    /// null if all slots in-flight (К-L16 backpressure).
    /// </summary>
    public static unsafe PipelineSlot* AllocateSlot(ulong simTick)
    {
        PipelineSlot* slot = null;
        return NativeMethods.df_pipeline_allocate_slot(simTick, &slot) == 1 ? slot : null;
    }

    /// <summary>
    /// Get slot at offset relative к current allocation cursor.
    /// offset=0 = current; offset=-1 = tail (К-L7.1 sim-thread read); -2..-D = display tail.
    /// </summary>
    public static unsafe PipelineSlot* GetSlot(int slotOffset)
    {
        PipelineSlot* slot = null;
        return NativeMethods.df_pipeline_get_slot(slotOffset, &slot) == 1 ? slot : null;
    }

    /// <summary>Bind VkFence opaque handle к slot after compute dispatch submitted.</summary>
    public static unsafe bool SetFence(PipelineSlot* slot, nint vkFenceHandle)
        => NativeMethods.df_pipeline_set_fence(slot, vkFenceHandle) == 1;

    /// <summary>
    /// Poll fences для all Dispatched slots; transitions slots Dispatched →
    /// FenceCompleted on fence signal. К10.3 v2 Commit 3: stub returning 0;
    /// Commit 4 wires actual vkGetFenceStatus integration via VulkanAttachment.
    /// </summary>
    public static int CheckFences()
    {
        unsafe
        {
            int transitioned = 0;
            NativeMethods.df_pipeline_check_fences(&transitioned);
            return transitioned;
        }
    }

    /// <summary>
    /// Test/integration helper: explicitly mark slot Dispatched → FenceCompleted.
    /// Used by test scenarios without real VkFence handles; Commit 4 wires this
    /// as primitive callable from Phase.Compute fence-poll integration.
    /// </summary>
    public static unsafe bool ForceFenceCompleted(PipelineSlot* slot)
        => NativeMethods.df_pipeline_force_fence_completed(slot) == 1;

    /// <summary>Atomic transition FenceCompleted → ReadableAsTail (fires wake per Item 37).</summary>
    public static unsafe bool TransitionToTail(PipelineSlot* slot)
        => NativeMethods.df_pipeline_transition_to_tail(slot) == 1;

    /// <summary>
    /// K-L18 quiescent state check (Item 41 forward consumer). Quiescent: all
    /// slots Empty or ReadableAsTail (no in-flight compute). Returns false
    /// если pipeline not initialized или any slot Dispatched/FenceCompleted.
    /// </summary>
    public static bool IsQuiescent()
    {
        unsafe
        {
            int q = 0;
            int rc = NativeMethods.df_pipeline_is_quiescent(&q);
            return rc == 1 && q == 1;
        }
    }
}
