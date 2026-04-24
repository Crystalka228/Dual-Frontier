using System;
using System.Diagnostics;
using BenchmarkDotNet.Running;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// Entry point for the native-vs-managed comparison.
///
/// Two modes:
///   * Default (no args) — runs a lightweight stopwatch smoke benchmark so
///     the experiment produces a comparable number on any machine without
///     needing the full BenchmarkDotNet warm-up cycle. Useful when the
///     native library has just been rebuilt and we want a quick
///     sanity check that the P/Invoke wiring works end to end.
///   * <c>--full</c> — hands off to BenchmarkDotNet for the rigorous run
///     (warm-up, statistics, memory diagnostics). Produces the numbers
///     referenced by the decision rule in <c>docs/NATIVE_CORE.md</c>.
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        if (Array.Exists(args, a => a == "--full"))
        {
            BenchmarkRunner.Run<NativeVsManagedBenchmark>();
            return 0;
        }

        RunSmoke();
        return 0;
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
