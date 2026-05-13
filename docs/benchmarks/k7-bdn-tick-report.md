---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-K7_BDN_TICK_REPORT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-K7_BDN_TICK_REPORT
---
```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26200.8246)
Unknown processor
.NET SDK 10.0.202
  [Host]   : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                | Mean      | Error     | StdDev    | Gen0   | Completed Work Items | Lock Contentions | Allocated |
|---------------------- |----------:|----------:|----------:|-------:|---------------------:|-----------------:|----------:|
| TickV2_ManagedStructs | 19.905 μs | 71.801 μs | 3.9357 μs | 0.7935 |               5.1911 |           0.0006 |    6985 B |
| TickV3_NativeBatched  |  5.224 μs |  5.138 μs | 0.2816 μs | 0.0420 |                    - |                - |     360 B |
