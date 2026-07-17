---
register_id: DOC-E-K7_BDN_TICK_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-05-09
last_modified: 2026-05-09
content_language: en
next_review_due: null
title: K7 BDN Tick Report
review_cadence: on-status-transition
reviewer: Crystalka
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