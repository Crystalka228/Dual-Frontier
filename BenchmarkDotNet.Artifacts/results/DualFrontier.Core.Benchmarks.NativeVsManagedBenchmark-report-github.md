```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26200.8246)
Unknown processor
.NET SDK 10.0.202
  [Host]   : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method            | Mean      | Error    | StdDev    | Median    | Ratio | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
|------------------ |----------:|---------:|----------:|----------:|------:|--------:|--------:|--------:|--------:|----------:|------------:|
| ManagedSumCurrent | 101.93 μs | 1.111 μs |  0.928 μs | 101.45 μs |  1.00 |    0.00 |       - |       - |       - |         - |          NA |
| NativeSumCurrent  |  95.31 μs | 0.470 μs |  0.393 μs |  95.32 μs |  0.94 |    0.01 |       - |       - |       - |         - |          NA |
| ManagedAdd10k     | 218.24 μs | 4.353 μs | 10.261 μs | 214.22 μs |  2.19 |    0.10 | 83.0078 | 41.5039 | 41.5039 |  655606 B |          NA |
| NativeAdd10k      | 399.83 μs | 7.951 μs | 19.653 μs | 401.88 μs |  3.92 |    0.13 |       - |       - |       - |      24 B |          NA |
