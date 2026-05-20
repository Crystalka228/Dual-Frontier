using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K10.3 v2 Item 33 — pipeline slot state machine managed binding tests.
/// Mirrors native pipeline_slot_test scenarios на managed side для cross-layer
/// roundtrip validation. Per Lesson #7 strengthening pattern: explicit struct
/// layout size verification catches native↔managed shape drift.
/// </summary>
public sealed class PipelineSlotInteropTests
{
    /// <summary>
    /// Lesson #7 strengthening: explicit struct layout size verification.
    /// Per V0.C.2 closure lesson, Marshal.SizeOf на enums fails в .NET 8;
    /// use unsafe sizeof() для compile-time accurate size including enum field.
    ///
    /// Native PipelineSlot (pipeline_slot.h):
    ///   uint64_t sim_tick           = 8 bytes
    ///   void* world_snapshot_ptr    = 8 bytes
    ///   void* fields_snapshot_ptr   = 8 bytes
    ///   void* compute_fence_handle  = 8 bytes
    ///   int32_t state               = 4 bytes
    ///   (trailing pad для 8-byte alignment) = 4 bytes
    ///   Total = 40 bytes
    /// </summary>
    [Fact]
    public void PipelineSlot_StructLayoutMatchesNative()
    {
        unsafe
        {
            int size = sizeof(PipelineSlotInterop.PipelineSlot);
            size.Should().Be(40, "PipelineSlot must marshal к native layout (40 bytes) без padding drift");
        }
    }

    [Fact]
    public void Init_AndGetDepth_Roundtrip()
    {
        PipelineSlotInterop.Reset();
        PipelineSlotInterop.GetDepth().Should().Be(0, "depth=0 before init");

        bool initialized = PipelineSlotInterop.Init(PipelineSlotInterop.DefaultDepth);
        initialized.Should().BeTrue("init D=2 succeeds");
        PipelineSlotInterop.GetDepth().Should().Be(2, "depth=2 after init D=2");

        PipelineSlotInterop.Reset();
        PipelineSlotInterop.GetDepth().Should().Be(0, "depth=0 after reset");
    }

    [Fact]
    public void Init_OutOfRangeDepth_Rejected()
    {
        PipelineSlotInterop.Reset();
        PipelineSlotInterop.Init(0).Should().BeFalse("depth=0 out-of-range");
        PipelineSlotInterop.Init(4).Should().BeFalse("depth=4 > max=3");
        PipelineSlotInterop.GetDepth().Should().Be(0, "не initialized after rejected attempts");
    }

    [Fact]
    public void AllocateSlot_StateTransitions()
    {
        PipelineSlotInterop.Reset();
        PipelineSlotInterop.Init(2);

        unsafe
        {
            PipelineSlotInterop.PipelineSlot* slot = PipelineSlotInterop.AllocateSlot(simTick: 1000);
            ((nint)slot).Should().NotBe(0, "slot pointer non-null");
            slot->SimTick.Should().Be(1000UL, "sim_tick assigned");
            slot->State.Should().Be(PipelineSlotInterop.SlotState.Dispatched);

            PipelineSlotInterop.ForceFenceCompleted(slot).Should().BeTrue();
            slot->State.Should().Be(PipelineSlotInterop.SlotState.FenceCompleted);

            PipelineSlotInterop.TransitionToTail(slot).Should().BeTrue();
            slot->State.Should().Be(PipelineSlotInterop.SlotState.ReadableAsTail);
        }

        PipelineSlotInterop.Reset();
    }

    [Fact]
    public void GetSlot_OffsetSemantics()
    {
        PipelineSlotInterop.Reset();
        PipelineSlotInterop.Init(2);

        unsafe
        {
            PipelineSlotInterop.PipelineSlot* s0 = PipelineSlotInterop.AllocateSlot(2000);
            PipelineSlotInterop.PipelineSlot* s1 = PipelineSlotInterop.AllocateSlot(2001);

            PipelineSlotInterop.PipelineSlot* current = PipelineSlotInterop.GetSlot(0);
            ((nint)current).Should().Be((nint)s1, "offset 0 = current");

            PipelineSlotInterop.PipelineSlot* tail = PipelineSlotInterop.GetSlot(-1);
            ((nint)tail).Should().Be((nint)s0, "offset -1 = tail (К-L7.1 sim read pattern)");

            PipelineSlotInterop.PipelineSlot* outOfRange = PipelineSlotInterop.GetSlot(-2);
            ((nint)outOfRange).Should().Be(0, "offset -D rejected (D=2)");
        }

        PipelineSlotInterop.Reset();
    }

    [Fact]
    public void IsQuiescent_K_L18_Precondition()
    {
        PipelineSlotInterop.Reset();
        PipelineSlotInterop.IsQuiescent().Should().BeFalse("pre-init = not quiescent");

        PipelineSlotInterop.Init(2);
        PipelineSlotInterop.IsQuiescent().Should().BeTrue("post-init all Empty = quiescent");

        unsafe
        {
            PipelineSlotInterop.PipelineSlot* slot = PipelineSlotInterop.AllocateSlot(3000);
            PipelineSlotInterop.IsQuiescent().Should().BeFalse("Dispatched slot = not quiescent");

            PipelineSlotInterop.ForceFenceCompleted(slot);
            PipelineSlotInterop.IsQuiescent().Should().BeFalse("FenceCompleted = not quiescent");

            PipelineSlotInterop.TransitionToTail(slot);
            PipelineSlotInterop.IsQuiescent().Should().BeTrue("ReadableAsTail = quiescent");
        }

        PipelineSlotInterop.Reset();
    }
}
