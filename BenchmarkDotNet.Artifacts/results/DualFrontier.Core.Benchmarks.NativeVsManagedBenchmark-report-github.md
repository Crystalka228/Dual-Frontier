```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26200.8246)
Unknown processor
.NET SDK 10.0.202
  [Host]   : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method            | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|------------------ |-----:|------:|------:|--------:|------------:|
| ManagedSumCurrent |   NA |    NA |     ? |       ? |           ? |
| NativeSumCurrent  |   NA |    NA |     ? |       ? |           ? |
| ManagedAdd10k     |   NA |    NA |     ? |       ? |           ? |
| NativeAdd10k      |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  NativeVsManagedBenchmark.ManagedSumCurrent: .NET 8.0(Runtime=.NET 8.0)
  NativeVsManagedBenchmark.NativeSumCurrent: .NET 8.0(Runtime=.NET 8.0)
  NativeVsManagedBenchmark.ManagedAdd10k: .NET 8.0(Runtime=.NET 8.0)
  NativeVsManagedBenchmark.NativeAdd10k: .NET 8.0(Runtime=.NET 8.0)
