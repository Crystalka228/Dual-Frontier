using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Tests.Scheduling.Fixtures;

// Shared fixtures for ParallelSystemScheduler stress/extreme scenarios.
// Extracted from SchedulerStressTests.cs:457-709 (2026-05-21) so that
// SchedulerExtremeTests (and any future stress siblings) can reuse the
// 64-wide + 16-chain system layout without duplicating ~250 lines of
// hand-written subclasses. [SystemAccess] is compile-time, so adding new
// system types still requires editing this file by hand.

internal sealed class TickCounter
{
    private long _total;
    private readonly ConcurrentDictionary<int, byte> _threadIds = new();

    public long Total => Interlocked.Read(ref _total);

    public void Tick()
    {
        Interlocked.Increment(ref _total);
        _threadIds.TryAdd(Thread.CurrentThread.ManagedThreadId, 0);
    }

    public HashSet<int> SnapshotThreadIds() => new(_threadIds.Keys);
}

// ─── Wide-layer fixture systems ────────────────────────────────────────
// 64 distinct SystemBase subclasses, each writing a unique component.
// The DependencyGraph will see no shared writes and no reads across them,
// placing every one into phase 0.

internal sealed class WC00 : IComponent { } internal sealed class WC01 : IComponent { }
internal sealed class WC02 : IComponent { } internal sealed class WC03 : IComponent { }
internal sealed class WC04 : IComponent { } internal sealed class WC05 : IComponent { }
internal sealed class WC06 : IComponent { } internal sealed class WC07 : IComponent { }
internal sealed class WC08 : IComponent { } internal sealed class WC09 : IComponent { }
internal sealed class WC10 : IComponent { } internal sealed class WC11 : IComponent { }
internal sealed class WC12 : IComponent { } internal sealed class WC13 : IComponent { }
internal sealed class WC14 : IComponent { } internal sealed class WC15 : IComponent { }
internal sealed class WC16 : IComponent { } internal sealed class WC17 : IComponent { }
internal sealed class WC18 : IComponent { } internal sealed class WC19 : IComponent { }
internal sealed class WC20 : IComponent { } internal sealed class WC21 : IComponent { }
internal sealed class WC22 : IComponent { } internal sealed class WC23 : IComponent { }
internal sealed class WC24 : IComponent { } internal sealed class WC25 : IComponent { }
internal sealed class WC26 : IComponent { } internal sealed class WC27 : IComponent { }
internal sealed class WC28 : IComponent { } internal sealed class WC29 : IComponent { }
internal sealed class WC30 : IComponent { } internal sealed class WC31 : IComponent { }
internal sealed class WC32 : IComponent { } internal sealed class WC33 : IComponent { }
internal sealed class WC34 : IComponent { } internal sealed class WC35 : IComponent { }
internal sealed class WC36 : IComponent { } internal sealed class WC37 : IComponent { }
internal sealed class WC38 : IComponent { } internal sealed class WC39 : IComponent { }
internal sealed class WC40 : IComponent { } internal sealed class WC41 : IComponent { }
internal sealed class WC42 : IComponent { } internal sealed class WC43 : IComponent { }
internal sealed class WC44 : IComponent { } internal sealed class WC45 : IComponent { }
internal sealed class WC46 : IComponent { } internal sealed class WC47 : IComponent { }
internal sealed class WC48 : IComponent { } internal sealed class WC49 : IComponent { }
internal sealed class WC50 : IComponent { } internal sealed class WC51 : IComponent { }
internal sealed class WC52 : IComponent { } internal sealed class WC53 : IComponent { }
internal sealed class WC54 : IComponent { } internal sealed class WC55 : IComponent { }
internal sealed class WC56 : IComponent { } internal sealed class WC57 : IComponent { }
internal sealed class WC58 : IComponent { } internal sealed class WC59 : IComponent { }
internal sealed class WC60 : IComponent { } internal sealed class WC61 : IComponent { }
internal sealed class WC62 : IComponent { } internal sealed class WC63 : IComponent { }

// Wide fixture base — every concrete subclass has its own [SystemAccess]
// declaring a unique write component. Per-tick work: increment shared
// counter + small SpinWait to give the worker thread observable load.
internal abstract class WideBase : SystemBase
{
    protected readonly TickCounter Counter;
    protected WideBase(TickCounter counter) { Counter = counter; }
    public override void Update(float delta)
    {
        Counter.Tick();
        Thread.SpinWait(2_000);
    }
}

[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC00) })]
internal sealed class W00 : WideBase { public W00(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC01) })]
internal sealed class W01 : WideBase { public W01(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC02) })]
internal sealed class W02 : WideBase { public W02(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC03) })]
internal sealed class W03 : WideBase { public W03(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC04) })]
internal sealed class W04 : WideBase { public W04(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC05) })]
internal sealed class W05 : WideBase { public W05(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC06) })]
internal sealed class W06 : WideBase { public W06(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC07) })]
internal sealed class W07 : WideBase { public W07(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC08) })]
internal sealed class W08 : WideBase { public W08(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC09) })]
internal sealed class W09 : WideBase { public W09(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC10) })]
internal sealed class W10 : WideBase { public W10(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC11) })]
internal sealed class W11 : WideBase { public W11(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC12) })]
internal sealed class W12 : WideBase { public W12(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC13) })]
internal sealed class W13 : WideBase { public W13(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC14) })]
internal sealed class W14 : WideBase { public W14(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC15) })]
internal sealed class W15 : WideBase { public W15(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC16) })]
internal sealed class W16 : WideBase { public W16(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC17) })]
internal sealed class W17 : WideBase { public W17(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC18) })]
internal sealed class W18 : WideBase { public W18(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC19) })]
internal sealed class W19 : WideBase { public W19(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC20) })]
internal sealed class W20 : WideBase { public W20(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC21) })]
internal sealed class W21 : WideBase { public W21(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC22) })]
internal sealed class W22 : WideBase { public W22(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC23) })]
internal sealed class W23 : WideBase { public W23(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC24) })]
internal sealed class W24 : WideBase { public W24(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC25) })]
internal sealed class W25 : WideBase { public W25(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC26) })]
internal sealed class W26 : WideBase { public W26(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC27) })]
internal sealed class W27 : WideBase { public W27(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC28) })]
internal sealed class W28 : WideBase { public W28(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC29) })]
internal sealed class W29 : WideBase { public W29(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC30) })]
internal sealed class W30 : WideBase { public W30(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC31) })]
internal sealed class W31 : WideBase { public W31(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC32) })]
internal sealed class W32 : WideBase { public W32(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC33) })]
internal sealed class W33 : WideBase { public W33(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC34) })]
internal sealed class W34 : WideBase { public W34(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC35) })]
internal sealed class W35 : WideBase { public W35(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC36) })]
internal sealed class W36 : WideBase { public W36(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC37) })]
internal sealed class W37 : WideBase { public W37(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC38) })]
internal sealed class W38 : WideBase { public W38(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC39) })]
internal sealed class W39 : WideBase { public W39(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC40) })]
internal sealed class W40 : WideBase { public W40(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC41) })]
internal sealed class W41 : WideBase { public W41(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC42) })]
internal sealed class W42 : WideBase { public W42(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC43) })]
internal sealed class W43 : WideBase { public W43(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC44) })]
internal sealed class W44 : WideBase { public W44(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC45) })]
internal sealed class W45 : WideBase { public W45(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC46) })]
internal sealed class W46 : WideBase { public W46(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC47) })]
internal sealed class W47 : WideBase { public W47(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC48) })]
internal sealed class W48 : WideBase { public W48(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC49) })]
internal sealed class W49 : WideBase { public W49(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC50) })]
internal sealed class W50 : WideBase { public W50(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC51) })]
internal sealed class W51 : WideBase { public W51(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC52) })]
internal sealed class W52 : WideBase { public W52(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC53) })]
internal sealed class W53 : WideBase { public W53(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC54) })]
internal sealed class W54 : WideBase { public W54(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC55) })]
internal sealed class W55 : WideBase { public W55(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC56) })]
internal sealed class W56 : WideBase { public W56(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC57) })]
internal sealed class W57 : WideBase { public W57(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC58) })]
internal sealed class W58 : WideBase { public W58(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC59) })]
internal sealed class W59 : WideBase { public W59(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC60) })]
internal sealed class W60 : WideBase { public W60(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC61) })]
internal sealed class W61 : WideBase { public W61(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC62) })]
internal sealed class W62 : WideBase { public W62(TickCounter c) : base(c) { } }
[SystemAccess(reads: new Type[0], writes: new[] { typeof(WC63) })]
internal sealed class W63 : WideBase { public W63(TickCounter c) : base(c) { } }

// ─── Deep-chain fixture systems ─────────────────────────────────────────
// C00 writes ChainC00; C01 reads ChainC00 and writes ChainC01; ... and so on.
// The graph builder must serialize them into ascending phases.

internal sealed class CC00 : IComponent { } internal sealed class CC01 : IComponent { }
internal sealed class CC02 : IComponent { } internal sealed class CC03 : IComponent { }
internal sealed class CC04 : IComponent { } internal sealed class CC05 : IComponent { }
internal sealed class CC06 : IComponent { } internal sealed class CC07 : IComponent { }
internal sealed class CC08 : IComponent { } internal sealed class CC09 : IComponent { }
internal sealed class CC10 : IComponent { } internal sealed class CC11 : IComponent { }
internal sealed class CC12 : IComponent { } internal sealed class CC13 : IComponent { }
internal sealed class CC14 : IComponent { } internal sealed class CC15 : IComponent { }

internal abstract class ChainBase : SystemBase
{
    protected readonly TickCounter Counter;
    protected ChainBase(TickCounter counter) { Counter = counter; }
    public override void Update(float delta)
    {
        Counter.Tick();
        Thread.SpinWait(2_000);
    }
}

[SystemAccess(reads: new Type[0], writes: new[] { typeof(CC00) })]
internal sealed class C00 : ChainBase { public C00(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC00) }, writes: new[] { typeof(CC01) })]
internal sealed class C01 : ChainBase { public C01(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC01) }, writes: new[] { typeof(CC02) })]
internal sealed class C02 : ChainBase { public C02(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC02) }, writes: new[] { typeof(CC03) })]
internal sealed class C03 : ChainBase { public C03(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC03) }, writes: new[] { typeof(CC04) })]
internal sealed class C04 : ChainBase { public C04(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC04) }, writes: new[] { typeof(CC05) })]
internal sealed class C05 : ChainBase { public C05(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC05) }, writes: new[] { typeof(CC06) })]
internal sealed class C06 : ChainBase { public C06(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC06) }, writes: new[] { typeof(CC07) })]
internal sealed class C07 : ChainBase { public C07(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC07) }, writes: new[] { typeof(CC08) })]
internal sealed class C08 : ChainBase { public C08(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC08) }, writes: new[] { typeof(CC09) })]
internal sealed class C09 : ChainBase { public C09(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC09) }, writes: new[] { typeof(CC10) })]
internal sealed class C10 : ChainBase { public C10(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC10) }, writes: new[] { typeof(CC11) })]
internal sealed class C11 : ChainBase { public C11(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC11) }, writes: new[] { typeof(CC12) })]
internal sealed class C12 : ChainBase { public C12(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC12) }, writes: new[] { typeof(CC13) })]
internal sealed class C13 : ChainBase { public C13(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC13) }, writes: new[] { typeof(CC14) })]
internal sealed class C14 : ChainBase { public C14(TickCounter c) : base(c) { } }
[SystemAccess(reads: new[] { typeof(CC14) }, writes: new[] { typeof(CC15) })]
internal sealed class C15 : ChainBase { public C15(TickCounter c) : base(c) { } }
