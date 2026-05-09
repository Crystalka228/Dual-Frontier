using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DualFrontier.Core.Benchmarks.TickLoop;

/// <summary>
/// K7 long-run runner — drives a chosen scenario through a fixed tick
/// count (default 10,000) at a fixed delta (1/30 s), recording per-tick
/// wall-clock time, total wall-clock, drift vs simulated time, GC
/// counts/pause time, and process memory growth. Writes a CSV with one
/// row per tick (for time-series analysis) and a summary footer.
///
/// Run via <c>Program.cs --long-run V&lt;n&gt;</c> dispatch.
///
/// The 10k-tick number is the K7 brief's default: enough samples for
/// drift, GC patterns, and percentile distributions to stabilize, not
/// so long that runs become tedious. Override via <c>--ticks N</c>.
/// </summary>
internal static class LongRunDriftRunner
{
    public const int DefaultTickCount = 10_000;
    public const float TickDelta = 1f / 30f;
    public const int PawnCount = 50;
    public const int Seed = 42;

    public static void Run(string variant, int tickCount, string outputPath)
    {
        if (string.IsNullOrEmpty(variant)) throw new ArgumentNullException(nameof(variant));
        if (tickCount <= 0) throw new ArgumentOutOfRangeException(nameof(tickCount));
        if (string.IsNullOrEmpty(outputPath)) throw new ArgumentNullException(nameof(outputPath));

        TickLoopScenarioBase scenario = variant switch
        {
            "V1" => new V1ManagedCurrentScenario(),
            "V2" => new V2ManagedStructsScenario(),
            "V3" => new V3NativeBatchedScenario(),
            _    => throw new ArgumentException($"Unknown variant '{variant}'. Expected V1 / V2 / V3.", nameof(variant)),
        };

        Console.WriteLine($"K7 long-run — variant {variant}, {tickCount} ticks @ {TickDelta:F4}s/tick (target {tickCount * TickDelta:F1}s simulated)");
        Console.WriteLine($"  output: {outputPath}");

        using (scenario)
        {
            scenario.SetupWorld(PawnCount, Seed);

            // Warmup: a few un-recorded ticks so JIT settles before
            // measurement starts. Without this the first ~50 ticks are
            // skewed by tiered compilation and can dominate the p99.
            for (int i = 0; i < 100; i++) scenario.ExecuteTick(TickDelta);

            // Force a full GC so the run starts from a deterministic
            // memory state — otherwise prior allocations tip the gen0/1
            // counts unpredictably across variants.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var tickTimes = new long[tickCount];
            GcSnapshot gcStart = MetricsCollector.CaptureGc();
            ProcessSnapshot procStart = MetricsCollector.CaptureProcess();
            var swTotal = Stopwatch.StartNew();

            long stopwatchFreq = Stopwatch.Frequency; // hoisted out of hot loop
            for (int i = 0; i < tickCount; i++)
            {
                long t0 = Stopwatch.GetTimestamp();
                scenario.ExecuteTick(TickDelta);
                tickTimes[i] = Stopwatch.GetTimestamp() - t0;
            }

            swTotal.Stop();
            GcSnapshot gcEnd = MetricsCollector.CaptureGc();
            ProcessSnapshot procEnd = MetricsCollector.CaptureProcess();
            GcDelta gcDelta = MetricsCollector.DiffGc(gcStart, gcEnd);
            ProcessDelta procDelta = MetricsCollector.DiffProcess(procStart, procEnd);

            WriteCsv(outputPath, variant, tickTimes, stopwatchFreq, swTotal.Elapsed, tickCount * TickDelta, gcDelta, procDelta);
            PrintSummary(variant, tickTimes, stopwatchFreq, swTotal.Elapsed, tickCount * TickDelta, gcDelta, procDelta);
        }
    }

    private static void WriteCsv(
        string path,
        string variant,
        long[] tickTimes,
        long stopwatchFreq,
        TimeSpan totalWallClock,
        float simulatedSeconds,
        GcDelta gcDelta,
        ProcessDelta procDelta)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)) ?? ".");

        using var writer = new StreamWriter(path);

        // Header
        writer.WriteLine("# K7 long-run drift CSV");
        writer.WriteLine($"# variant={variant}");
        writer.WriteLine($"# tickCount={tickTimes.Length}");
        writer.WriteLine($"# tickDeltaSeconds={TickDelta.ToString(CultureInfo.InvariantCulture)}");
        writer.WriteLine($"# stopwatchFrequency={stopwatchFreq}");
        writer.WriteLine($"# totalWallClockMs={totalWallClock.TotalMilliseconds.ToString("F3", CultureInfo.InvariantCulture)}");
        writer.WriteLine($"# simulatedSeconds={simulatedSeconds.ToString("F3", CultureInfo.InvariantCulture)}");
        writer.WriteLine($"# driftSeconds={(totalWallClock.TotalSeconds - simulatedSeconds).ToString("F3", CultureInfo.InvariantCulture)}");
        writer.WriteLine($"# gen0Collections={gcDelta.Gen0Collections}");
        writer.WriteLine($"# gen1Collections={gcDelta.Gen1Collections}");
        writer.WriteLine($"# gen2Collections={gcDelta.Gen2Collections}");
        writer.WriteLine($"# gcAllocatedBytes={gcDelta.AllocatedBytes}");
        writer.WriteLine($"# gcPauseDurationMs={gcDelta.PauseDuration.TotalMilliseconds.ToString("F3", CultureInfo.InvariantCulture)}");
        writer.WriteLine($"# workingSetGrowthBytes={procDelta.WorkingSetGrowth}");
        writer.WriteLine($"# privateMemoryGrowthBytes={procDelta.PrivateMemoryGrowth}");
        writer.WriteLine($"# totalProcessorTimeMs={procDelta.TotalProcessorTime.TotalMilliseconds.ToString("F3", CultureInfo.InvariantCulture)}");
        writer.WriteLine();
        writer.WriteLine("tick,tickTimeNs");

        // One row per tick — the BDN HTML report is for stats; this CSV
        // is for time-series so the report can describe distribution
        // over the run (warmup tail, drift accumulation, GC spike points).
        for (int i = 0; i < tickTimes.Length; i++)
        {
            // Convert ticks → nanoseconds via stopwatchFreq.
            long ns = tickTimes[i] * 1_000_000_000L / stopwatchFreq;
            writer.WriteLine($"{i},{ns}");
        }
    }

    private static void PrintSummary(
        string variant,
        long[] tickTimes,
        long stopwatchFreq,
        TimeSpan totalWallClock,
        float simulatedSeconds,
        GcDelta gcDelta,
        ProcessDelta procDelta)
    {
        // Compute percentiles + mean + stddev. Sort a copy — preserves
        // the original tick order for the CSV time-series.
        var sorted = (long[])tickTimes.Clone();
        Array.Sort(sorted);

        long p50 = sorted[sorted.Length / 2];
        long p90 = sorted[(int)(sorted.Length * 0.90)];
        long p95 = sorted[(int)(sorted.Length * 0.95)];
        long p99 = sorted[(int)(sorted.Length * 0.99)];
        long p999 = sorted[(int)(sorted.Length * 0.999)];
        long max = sorted[sorted.Length - 1];

        double meanTicks = tickTimes.Average();
        double sumSquaredDiff = 0;
        for (int i = 0; i < tickTimes.Length; i++)
        {
            double diff = tickTimes[i] - meanTicks;
            sumSquaredDiff += diff * diff;
        }
        double stddevTicks = System.Math.Sqrt(sumSquaredDiff / tickTimes.Length);

        long ToNs(long ticks) => ticks * 1_000_000_000L / stopwatchFreq;
        double ToNsD(double ticks) => ticks * 1_000_000_000.0 / stopwatchFreq;

        Console.WriteLine();
        Console.WriteLine($"=== K7 long-run summary — {variant} ===");
        Console.WriteLine($"  tick count       : {tickTimes.Length}");
        Console.WriteLine($"  total wall-clock : {totalWallClock.TotalMilliseconds,10:F3} ms");
        Console.WriteLine($"  simulated time   : {simulatedSeconds * 1000.0,10:F3} ms");
        Console.WriteLine($"  drift            : {(totalWallClock.TotalMilliseconds - simulatedSeconds * 1000.0),10:F3} ms");
        Console.WriteLine($"  per-tick mean    : {ToNsD(meanTicks) / 1000.0,10:F3} μs");
        Console.WriteLine($"  per-tick stddev  : {ToNsD(stddevTicks) / 1000.0,10:F3} μs");
        Console.WriteLine($"  p50              : {ToNs(p50) / 1000.0,10:F3} μs");
        Console.WriteLine($"  p90              : {ToNs(p90) / 1000.0,10:F3} μs");
        Console.WriteLine($"  p95              : {ToNs(p95) / 1000.0,10:F3} μs");
        Console.WriteLine($"  p99              : {ToNs(p99) / 1000.0,10:F3} μs");
        Console.WriteLine($"  p99.9            : {ToNs(p999) / 1000.0,10:F3} μs");
        Console.WriteLine($"  max              : {ToNs(max) / 1000.0,10:F3} μs");
        Console.WriteLine($"  GC gen0          : {gcDelta.Gen0Collections}");
        Console.WriteLine($"  GC gen1          : {gcDelta.Gen1Collections}");
        Console.WriteLine($"  GC gen2          : {gcDelta.Gen2Collections}");
        Console.WriteLine($"  GC alloc         : {gcDelta.AllocatedBytes / 1024.0 / 1024.0,10:F3} MB");
        Console.WriteLine($"  GC pause total   : {gcDelta.PauseDuration.TotalMilliseconds,10:F3} ms");
        Console.WriteLine($"  WS growth        : {procDelta.WorkingSetGrowth / 1024.0 / 1024.0,10:F3} MB");
        Console.WriteLine($"  CPU time total   : {procDelta.TotalProcessorTime.TotalMilliseconds,10:F3} ms");
    }
}
