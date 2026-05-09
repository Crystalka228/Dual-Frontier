using System;
using System.Diagnostics;

namespace DualFrontier.Core.Benchmarks.TickLoop;

/// <summary>
/// K7 helper — captures GC and process metrics in a single point-in-time
/// snapshot, then computes deltas between two snapshots. Used by the
/// long-run runner (10k-tick scenario) to quantify cumulative allocation,
/// generation collection counts, GC pause time, and OS process footprint
/// over the run. Avoids any third-party diagnostic dependency so the
/// numbers reflect the raw .NET 8 runtime, not a wrapper's interpretation.
/// </summary>
internal static class MetricsCollector
{
    public static GcSnapshot CaptureGc()
    {
        return new GcSnapshot(
            Gen0: GC.CollectionCount(0),
            Gen1: GC.CollectionCount(1),
            Gen2: GC.CollectionCount(2),
            AllocatedBytes: GC.GetTotalAllocatedBytes(precise: true),
            PauseDuration: GC.GetTotalPauseDuration());
    }

    public static GcDelta DiffGc(GcSnapshot start, GcSnapshot end)
    {
        return new GcDelta(
            Gen0Collections: end.Gen0 - start.Gen0,
            Gen1Collections: end.Gen1 - start.Gen1,
            Gen2Collections: end.Gen2 - start.Gen2,
            AllocatedBytes: end.AllocatedBytes - start.AllocatedBytes,
            PauseDuration: end.PauseDuration - start.PauseDuration);
    }

    public static ProcessSnapshot CaptureProcess()
    {
        Process p = Process.GetCurrentProcess();
        // Refresh first so the snapshot reflects current state, not the
        // values cached at process startup.
        p.Refresh();
        return new ProcessSnapshot(
            WorkingSet: p.WorkingSet64,
            PrivateMemory: p.PrivateMemorySize64,
            TotalProcessorTime: p.TotalProcessorTime);
    }

    public static ProcessDelta DiffProcess(ProcessSnapshot start, ProcessSnapshot end)
    {
        return new ProcessDelta(
            WorkingSetGrowth: end.WorkingSet - start.WorkingSet,
            PrivateMemoryGrowth: end.PrivateMemory - start.PrivateMemory,
            TotalProcessorTime: end.TotalProcessorTime - start.TotalProcessorTime);
    }
}

internal readonly record struct GcSnapshot(
    int Gen0,
    int Gen1,
    int Gen2,
    long AllocatedBytes,
    TimeSpan PauseDuration);

internal readonly record struct GcDelta(
    int Gen0Collections,
    int Gen1Collections,
    int Gen2Collections,
    long AllocatedBytes,
    TimeSpan PauseDuration);

internal readonly record struct ProcessSnapshot(
    long WorkingSet,
    long PrivateMemory,
    TimeSpan TotalProcessorTime);

internal readonly record struct ProcessDelta(
    long WorkingSetGrowth,
    long PrivateMemoryGrowth,
    TimeSpan TotalProcessorTime);
