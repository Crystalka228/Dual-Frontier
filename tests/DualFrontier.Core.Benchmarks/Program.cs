using System;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Running;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Benchmarks.TickLoop;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// Entry point for the benchmark suite.
///
/// Modes:
///   * Default (no args) — lightweight stopwatch smoke benchmark
///     (K0/K1 baseline) so the wiring is exercised quickly without the
///     full BenchmarkDotNet warm-up cycle.
///   * <c>--full</c> — hands off to BenchmarkDotNet for the rigorous K0/K1
///     numbers (warm-up, statistics, memory diagnostics).
///   * <c>--bdn-tick V&lt;n&gt;</c> — K7 BDN tick-cost benchmark for the
///     given variant (V2 or V3 on <c>main</c>; V1 in worktree).
///   * <c>--long-run V&lt;n&gt;</c> — K7 10,000-tick custom Stopwatch loop
///     measuring drift + GC + cumulative allocations.
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        if (Array.Exists(args, a => a == "--full"))
        {
            BenchmarkRunner.Run(new[]
            {
                typeof(NativeVsManagedBenchmark),
                typeof(NativeBulkAddBenchmark),
            });
            return 0;
        }

        // K7 dispatch — order matters: more specific flags first.
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--bdn-tick":
                    return RunBdnTick(args, i);
                case "--long-run":
                    return RunLongRun(args, i);
            }
        }

        RunSmoke();
        return 0;
    }

    private static int RunBdnTick(string[] args, int flagIndex)
    {
        if (flagIndex + 1 >= args.Length)
        {
            Console.Error.WriteLine("--bdn-tick requires a variant argument (V1 / V2 / V3).");
            return 1;
        }
        string variant = args[flagIndex + 1];
        // BDN's filter argument selects which [Benchmark] methods run.
        // The benchmark class lives at DualFrontier.Core.Benchmarks.TickLoop.TickLoopBenchmark
        // and exposes TickV2_ManagedStructs / TickV3_NativeBatched. The
        // filter pattern matches the variant suffix.
        string filter = variant switch
        {
            "V1" => "*TickV1*",
            "V2" => "*TickV2*",
            "V3" => "*TickV3*",
            _    => throw new ArgumentException($"Unknown variant '{variant}'."),
        };

        BenchmarkRunner.Run<TickLoopBenchmark>(args: new[] { "--filter", filter });
        return 0;
    }

    private static int RunLongRun(string[] args, int flagIndex)
    {
        if (flagIndex + 1 >= args.Length)
        {
            Console.Error.WriteLine("--long-run requires a variant argument (V1 / V2 / V3).");
            return 1;
        }
        string variant = args[flagIndex + 1];
        int tickCount = LongRunDriftRunner.DefaultTickCount;
        // Optional --ticks N override.
        for (int j = 0; j < args.Length - 1; j++)
        {
            if (args[j] == "--ticks" && int.TryParse(args[j + 1], out int parsed) && parsed > 0)
            {
                tickCount = parsed;
                break;
            }
        }

        // Output goes under docs/benchmarks/ relative to repo root.
        // Resolve from the assembly's location upward — the benchmark
        // project lives at tests/DualFrontier.Core.Benchmarks/, so the
        // repo root is two parents up from bin/.
        string repoRoot = ResolveRepoRoot();
        string outputPath = Path.Combine(repoRoot, "docs", "benchmarks",
            $"k7-long-run-{variant}.csv");

        LongRunDriftRunner.Run(variant, tickCount, outputPath);
        return 0;
    }

    private static string ResolveRepoRoot()
    {
        // Walk up from the assembly directory until we find the .git
        // marker. Robust against running from worktree / temp BDN dir
        // / repo root / IDE launch configurations.
        string? dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            if (Directory.Exists(Path.Combine(dir, ".git"))
                || File.Exists(Path.Combine(dir, ".git")))
            {
                return dir;
            }
            DirectoryInfo? parent = Directory.GetParent(dir);
            if (parent is null) break;
            dir = parent.FullName;
        }
        // Fallback — current working directory. Caller's responsibility
        // to cd into repo root before invoking.
        return Directory.GetCurrentDirectory();
    }

    private static void RunSmoke()
    {
        const int n = 10_000;
        Console.WriteLine($"Native vs Managed smoke benchmark — {n} entities");
        Console.WriteLine("  (run with --full to invoke BenchmarkDotNet)");
        Console.WriteLine();

        // Managed side.
        var managed = new World();
        var managedIds = new EntityId[n];
        var swManagedAdd = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
        {
            managedIds[i] = managed.CreateEntity();
            managed.AddComponent(managedIds[i], new BenchHealthComponent(i, 100));
        }
        swManagedAdd.Stop();

        long managedSum = 0;
        var swManagedGet = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
        {
            managed.TryGetComponent(managedIds[i], out BenchHealthComponent c);
            managedSum += c.Current;
        }
        swManagedGet.Stop();

        // Native side.
        using var native = new NativeWorld();
        var nativeIds = new EntityId[n];
        var swNativeAdd = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
        {
            nativeIds[i] = native.CreateEntity();
            native.AddComponent(nativeIds[i], new BenchHealthComponent(i, 100));
        }
        swNativeAdd.Stop();

        long nativeSum = 0;
        var swNativeGet = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
        {
            native.TryGetComponent(nativeIds[i], out BenchHealthComponent c);
            nativeSum += c.Current;
        }
        swNativeGet.Stop();

        if (managedSum != nativeSum)
        {
            Console.Error.WriteLine(
                $"Checksum mismatch: managed={managedSum} native={nativeSum}. " +
                "The two worlds are not storing equivalent data — fix before trusting timings.");
            Environment.Exit(2);
        }

        Console.WriteLine($"Add  {n} entities+components:");
        Console.WriteLine($"  managed: {swManagedAdd.Elapsed.TotalMilliseconds,8:F3} ms");
        Console.WriteLine($"  native:  {swNativeAdd.Elapsed.TotalMilliseconds,8:F3} ms");
        Console.WriteLine($"  ratio:   {Ratio(swNativeAdd, swManagedAdd):F2}x (lower is faster for native)");
        Console.WriteLine();
        Console.WriteLine($"Get  {n} components:");
        Console.WriteLine($"  managed: {swManagedGet.Elapsed.TotalMilliseconds,8:F3} ms");
        Console.WriteLine($"  native:  {swNativeGet.Elapsed.TotalMilliseconds,8:F3} ms");
        Console.WriteLine($"  ratio:   {Ratio(swNativeGet, swManagedGet):F2}x (lower is faster for native)");
        Console.WriteLine();
        Console.WriteLine($"Checksum (both): {managedSum}");
    }

    private static double Ratio(Stopwatch native, Stopwatch managed)
    {
        double m = managed.Elapsed.TotalMilliseconds;
        if (m <= 0) return double.NaN;
        return native.Elapsed.TotalMilliseconds / m;
    }
}
