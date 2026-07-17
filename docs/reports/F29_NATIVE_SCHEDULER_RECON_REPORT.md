---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT
---
# F29_NATIVE_SCHEDULER RECON REPORT -- 2026-07-04

> Read-only reconnaissance for finding **F-29** (native/managed scheduler pathology).
> Executor: Claude Code (Opus), local session on `SKARLET`. Repository
> `D:\Colony_Simulator\Colony_Simulator` @ `main` `e628e16`. This report is the
> ONLY file the session creates inside the repo tree (untracked; the fix cascade
> enrolls it at its first commit). Standing law honored: `TESTING_STRATEGY §8`
> no-pipe + `Start-Process`/file-redirect/`WaitForExit`-watchdog for every
> `dotnet test`; every measurement carries its verbatim command / reproduction
> recipe. **Operator caveat recorded:** a parallel Claude session (different repo)
> ran on this machine during the recon, so absolute wall-clock numbers carry
> machine-contention noise -- the **growth orders, ratios, and crash signatures**,
> not the absolute milliseconds, are the load-bearing results. Bez kostylei.

---

## R1 Base state

| Item | Value | Source / command |
|---|---|---|
| Branch | `main` | `git rev-parse --abbrev-ref HEAD` |
| HEAD | `e628e160460fea949ce1fd82ab6741df65a468e0` (expected `e628e16` ✓) | `git rev-parse HEAD` |
| Working tree | **clean** (empty porcelain) | `git status --porcelain` |
| Commits after expected HEAD | none (top of log = `e628e16`) | `git log --oneline -6` |
| Divergence vs origin | none — `origin/main` == local `main` == `e628e16` (**local ref read, no fetch**) | `git log --oneline -1 origin/main`; `git branch -vv` |
| `register_version` | **`"2.22"`** (expected 2.22 ✓) | `docs/governance/REGISTER.yaml:11` (read directly; `sync_register.ps1` NOT run) |

**F-ledger (`docs/ROADMAP.md`) confirmations:**
- **F-29 OPEN**, severity **S2** (`ROADMAP.md:988`): (a) `SchedulerStressTests.NativeGraph_…` native TickBegin crash (testhost crash) under concurrent build/test load — green in per-project isolation; (b) `SchedulerExtremeTests` S1 (50k×3k), S2 (200k ticks), S7 (250k systems) do not complete within a 120–180 s watchdog. Owner: architect / dedicated native-scheduler cascade; "may share a root cause in SystemGraphInterop."
- **F-10 CLOSED**, S1 (`ROADMAP.md:970`): F-10 isolation cascade 2026-07-02; the TickBegin crash-under-load reassigned to **F-29(a)**; the Extreme non-completers reassigned to **F-29(b)** and Skip-quarantined (C6 `d8fc56c`). Crash tick recorded as **2692** ("reproduced twice under survey load").
- **F-28(a) CLOSED** (folded into F-10 C7 `87ceb90`), **F-28(b) OPEN** — consistent with the recon ground state.

**Quarantine / reclassify state (as expected):**
- `SchedulerExtremeTests.cs` S1 `:132`, S2 `:240`, S7 `:796` each carry
  `[Fact(Skip = "F-29(b): ExtremeScale non-completer -- native scheduler scale pathology; does not complete within CI budget. See docs/ROADMAP.md F-29.")]`. ✓
- `SchedulerStressTests.cs:43` carries `[Trait("Category", "Stress")]`. ✓
- Literal crash test: `SchedulerStressTests.NativeGraph_FiveThousandSystems_RandomDag_ComputesAndTicksWithoutError` (`:96-174`) — 5000 systems, 5000 ticks, `ComputePerTickGraph` + `TickBegin` (alternating).

**Environment / tooling reality (governs T3/T4 method):** dotnet SDK 10.0.204; **16 logical processors** (8c/16t, extreme suite does not early-skip); **`cmake`, `procdump`, `cdb`/`windbg`, `cl` all absent from PATH** (no fresh native build, no external native debugger this session); Debug native DLL **and PDBs already built** (`native/…/build/Debug/DualFrontier.Core.Native.pdb`, `out/build/x64-Debug/…pdb`); the binary the tests load is `tests/…/Core.Tests/bin/Release/net8.0/DualFrontier.Core.Native.dll` (Release, 2026-06-12).

---

## R2 Scheduler code-surface map

### Call-path map (managed TickBegin → C ABI → native tick)

```
SchedulerStressTests.NativeGraph…                              tests/…/Scheduling/SchedulerStressTests.cs:153-173
  SystemGraphInterop.TickBegin(tick)                           src/DualFrontier.Core.Interop/SystemGraphInterop.cs:195
    └ P/Invoke df_scheduler_tick_begin(tick)                   (DllImport "DualFrontier.Core.Native")
        capi.cpp:1495  df_scheduler_tick_begin                 [NO LOCK; try/catch(...) — SEH not caught]
          ├ default_wake_registry()   [LOCK-FREE singleton]    wake_registry.cpp:196
          │     .fire_timer / .runqueue_size / .drain_runqueue  wake_registry.cpp:77 / 138
          └ default_scheduler_graph()  [LOCK-FREE singleton]    system_graph.cpp:270  (static SystemGraph instance)
                .compute_per_tick_graph(runnable, n)            system_graph.cpp:87
                   ├ build by_id map: for (s : systems_) …      system_graph.cpp:97-99   ← RACE READ of systems_
                   └ kahn_topological_sort(subset, …)           system_graph.cpp:178
                        step 1  write-conflict scan  O(N²)      system_graph.cpp:187-206
                        step 2  edge build           O(N²)      system_graph.cpp:208-231
                        step 3  Kahn layering        O(P·N)     system_graph.cpp:236-266
```

Concurrent mutator — a **different xUnit collection running in parallel**:

```
SchedulerExtremeTests ctor & Dispose → ResetAllGlobals()       tests/…/SchedulerExtremeTests.cs:94-108
  SystemGraphInterop.Clear() → df_scheduler_clear()            capi.cpp:1042 → system_graph.cpp:69  systems_.clear()   ← RACE WRITE
  WakeRegistryInterop.Clear() → df_wake_registry_clear()       wake_registry.cpp:187                                  ← RACE WRITE
```

**Structural facts established by reading (not assumed):**
1. `df_scheduler_tick_begin` (`capi.cpp:1495-1514`) does **not** use the thread pool — it is pure wake-registry + graph compute. `ThreadPool` (`thread_pool.*`) and `phase_barrier.h` (`BarrierType`) are **not on this path** (dispatch is documented as "wired in the load-bearing Commit 14", `df_capi.h:855-856`). ⇒ **defect (a) is not a thread-pool / barrier bug.**
2. **Zero synchronization on the scheduler C ABI.** Grep of `capi.cpp` for `mutex|lock_guard|std::lock` across all `df_scheduler_*` → nothing. `SystemGraph` declares no mutex (`system_graph.h:112-123`: only `std::vector`s + a `std::string`). `WakeRegistry` is likewise lock-free. The header states the contract: *"Thread-safety: single-threaded registration / compute. Concurrent reads of already-computed phase composition are safe (immutable post-compute)."* (`system_graph.h:29-30`). But `compute_*` **writes** `per_tick_phases_`/`static_phases_` and `clear()` **frees** `systems_` — so a concurrent tick-vs-clear is a write/write + read/write race, **outside** the "concurrent reads only" contract.
3. `df_scheduler_tick_begin` wraps its body in `try { … } catch (...) { return 0; }` — a C++ `catch(...)` does **not** catch an access-violation / heap-corruption SEH, so a data-race corruption is **not** swallowed; it terminates the process (the "testhost crash").

### Ranked hot spots

**Defect (b) — scale (compute-bound, all in `system_graph.cpp`):**

| Rank | Site | Cost | Note |
|---|---|---|---|
| 1 | `:187-206` write-conflict scan | **O(N²·W)** (W = writes/system = 1) | pairs `i × j>i`; never early-exits in the tests (unique write ids ⇒ full N² pairs); rebuilds an `unordered_set` per outer `i` |
| 2 | `:208-231` edge build | **O(N²·R)** (R = reads/system, 0–4) | full `i × j` (all pairs); rebuilds `writes_a` per `i` |
| 3 | `:22-26` `register_system` dup-id scan | **O(N) per call → O(N²) over N registrations** | linear scan of `systems_` every register; the S7 "register-only" cost |
| 4 | `:236-266` Kahn layering | **O(P·N)** (P = phases; measured P plateaus at 27) | each layer re-scans **all** N for `in_degree==0`; sub-dominant while P is small |

Verbatim — the suspect write-conflict scan (`system_graph.cpp:187-206`):
```cpp
    // 1. Write-write conflict detection: pairs of systems writing the same component.
    for (std::size_t i = 0; i < subset.size(); ++i) {
        const SystemEntry* a = subset[i];
        if (a->writes.empty()) { continue; }
        std::unordered_set<uint32_t> writes_a(a->writes.begin(), a->writes.end());
        for (std::size_t j = i + 1; j < subset.size(); ++j) {
            const SystemEntry* b = subset[j];
            for (uint32_t w : b->writes) {
                if (writes_a.count(w) != 0) { … return -1; }
            }
        }
    }
```
Verbatim — the edge build, also all-pairs (`system_graph.cpp:212-231`):
```cpp
    for (std::size_t i = 0; i < subset.size(); ++i) {
        const SystemEntry* a = subset[i];
        if (a->writes.empty()) { continue; }
        std::unordered_set<uint32_t> writes_a(a->writes.begin(), a->writes.end());
        for (std::size_t j = 0; j < subset.size(); ++j) {
            if (i == j) { continue; }
            const SystemEntry* b = subset[j];
            for (uint32_t r : b->reads) {
                if (writes_a.count(r) != 0) { adjacency[i].push_back(j); in_degree[j]++; break; }
            }
        }
    }
```

**Defect (a) — concurrency (missing synchronization):**

| Rank | Site | Defect |
|---|---|---|
| 1 | `system_graph.cpp:270` `default_scheduler_graph()` (Meyers singleton) + `capi.cpp:1495/1042/1013` | process-global mutable graph, **no lock**; `tick_begin` read races `clear()`/`register` write |
| 2 | `wake_registry.cpp:196` `default_wake_registry()` + `capi.cpp` wrappers | second lock-free process-global on the same `tick_begin` path |
| 3 | native bus singletons (`bus_*.cpp`, `df_bus_clear`) | same class: process-global, reset in test ctor/Dispose, concurrently accessed under parallel collections (see R3 in-suite S3/S4 evidence) |

Per-tick lock/barrier usage — verbatim `df_scheduler_tick_begin` (`capi.cpp:1495-1514`), note the **absence** of any lock:
```cpp
DF_API int32_t df_scheduler_tick_begin(uint64_t current_tick) {
    try {
        auto& registry = dualfrontier::default_wake_registry();
        auto& graph = dualfrontier::default_scheduler_graph();
        registry.fire_timer(current_tick);
        int32_t runqueue_size = registry.runqueue_size();
        if (runqueue_size <= 0) { return graph.compute_per_tick_graph(nullptr, 0); }
        std::vector<uint32_t> runnable(static_cast<std::size_t>(runqueue_size));
        int32_t drained = registry.drain_runqueue(runnable.data(), runqueue_size);
        return graph.compute_per_tick_graph(runnable.data(), static_cast<uint32_t>(drained));
    } catch (...) { return 0; }
}
```
The thread pool *does* hold locks (`std::lock_guard<std::mutex> lock(queue_mutex_)` at `thread_pool.cpp:41,55,81,114`; `std::unique_lock` at `:72,91`) but only on the submit/wait path — **not reached by `tick_begin`**; off the crash path.

### Existing native selftest coverage (`native/…/test/selftest.cpp`)

Scheduler scenarios present — all **single-threaded**, all **tiny (≤3 systems)**: `scenario_system_graph_basic_register / linear_chain / parallel_layer / write_conflict / cycle_detection / per_tick_subset` (`:1033-1665`), `scenario_scheduler_tick_begin_orchestration` (`:1508`, ticks 0-2), plus policies/trace/intrinsics/diagnostics + `scenario_thread_pool_*`.
**Two gaps a fix must close:** (i) **no concurrency scenario** — nothing spawns two threads against `df_scheduler_*` + `df_scheduler_clear` (the defect-(a) shape); (ii) **no scale scenario** — max ~3 systems, nothing exercises the O(N²) path at 10k/50k/250k (defect (b)).

---

## R3 Defect (a) crash-under-load

The mechanism (R2) is a **data race on lock-free process-global native singletons** (`default_scheduler_graph()`, `default_wake_registry()`, and the native bus): the reader path (`TickBegin`→`compute_per_tick_graph` iterating `systems_`) runs concurrently — via xUnit's default cross-collection parallelism — with a writer path (`SchedulerExtremeTests` ctor/Dispose `Clear()` = `systems_.clear()` / `df_bus_clear`). The two classes are in **different** collections (`SchedulerStressTests` default; `SchedulerExtremeTests` `[Collection("ExtremeSerial")]`) and **no** assembly-level `CollectionBehavior`/`xunit.runner.json` disables parallelization (grep of the test project → nothing). **Latent enabler:** the `SchedulerExtremeTests.cs:42` comment claims `[Collection]` "forces … isolation from other classes" — but a collection only serializes *within itself*; against a *different* collection it still runs in parallel.

Reproduced **three independent ways**, all pointing at the same fault:

### (1) Deterministic out-of-repo harness — mechanism proof

Two threads on the exact Release DLL the tests load: reader loops `df_scheduler_compute_static_graph()` (the same `systems_`-iterating read path); writer loops `df_scheduler_clear()` + re-register (frees/reallocs `systems_`). No repo code involved.
Recipe (verbatim):
```
# harness built: dotnet build -c Release (net8.0, x64, AllowUnsafeBlocks); DllImport absolute path =
#   D:\Colony_Simulator\Colony_Simulator\tests\DualFrontier.Core.Tests\bin\Release\net8.0\DualFrontier.Core.Native.dll
$env:DOTNET_DbgEnableMiniDump=1; $env:DOTNET_DbgMiniDumpType=2; $env:DOTNET_DbgMiniDumpName="…\race_dump.%p.dmp"
Start-Process f29harness.exe -ArgumentList "race","4000","15000" -NoNewWindow \
  -RedirectStandardOutput race.out -RedirectStandardError race.err   # 45–60s WaitForExit watchdog
```
- **Crash rate 3/3** (also a 4th confirming run) — terminated at 1.9 s / 2.2 s / 3.0 s (and 2.9 s) vs a 15 s intended duration; **mean time-to-crash ≈ 2.4 s**.
- **Two SEH signatures of the same race** (Windows `Application Error` log, verbatim):
  - `System.AccessViolationException: Attempted to read or write protected memory…` at frame `Native.df_scheduler_compute_static_graph()`; **`Exception code: 0xc0000005`**, faulting module `coreclr.dll` (8.0.2726…), fault offset **`0x1d45aa`** — 3 of 4.
  - **`Exception code: 0xc0000374`** (STATUS_HEAP_CORRUPTION), faulting module `ntdll.dll`, offset `0x112265` — 1 of 4 (the "silent" run: the Windows heap manager *detecting* corruption on the writer's allocate/free path). **Heap corruption is only possible from concurrent heap mutation** — direct proof of the `systems_` race.
- **createdump minidump captured**: `race_dump.<pid>.dmp` (9.76 MB, heap) — via the runtime's built-in createdump; retained for a later native-symbol session (deleted with the harness at session end).

### (2) In-suite SOFT manifestation — state corruption without host death

Recipe (verbatim, `TESTING_STRATEGY §8` runner: `Start-Process dotnet test … --no-build --logger trx --blame` + `WaitForExit` watchdog + kill lingering `testhost`/`vstest`):
```
dotnet test …\DualFrontier.Core.Tests.csproj -c Release --no-build \
  --filter "FullyQualifiedName~SchedulerStressTests|FullyQualifiedName~SchedulerExtremeTests&FullyQualifiedName!~S6_" \
  --logger "trx;…" --blame     # under 6-core CPU-starvation load; 300s watchdog
```
Result (TRX = truth): outcome **Failed**, 9 passed / **2 failed** / 0 aborted — the host **survived** and wrote a complete TRX. The two failures are the race's soft face:
- `NativeGraph…`: **`Expected SystemGraphInterop.SystemCount to be 5000, but found 1758`** — a concurrent `SchedulerExtremeTests` `Clear()` wiped the graph mid-registration (definitive proof the two parallel classes mutate the same scheduler singleton).
- `S3_Bus_FiveMillionEventsPerTier…`: **`Expected s_extremeFastCount to be 5000000, but found 4375000`** (short by exactly 2×312 500 = two producer threads) — the shared native **bus** singleton corrupted by the parallel class's bus activity/reset.

### (3) In-suite HARD manifestation — the literal testhost crash

Same runner, across load regimes (0 / 2 / 3 CPU-starvation burners), 150 s watchdog, 4 attempts:
- **Testhost crash in 3 of 4 attempts** — vstest stderr (verbatim): *"The active Test Run was aborted because the host process exited unexpectedly … The test running when the crash occurred: `SchedulerStressTests.ManagedScheduler_WideLayerPlusDeepChain…` / `SchedulerExtremeTests.S4_Bus_FastTier_SixtyFourProducerThreads…`"*; `total` truncated to 6–7 (vs 14 clean). The 4th attempt soft-failed (as (2)).
- **Same fault as the harness**: Windows `Application Error` for `testhost.exe` → module `coreclr.dll`, **`Exception code: 0xc0000005`**, fault offset **`0x1d45aa`** — *identical module+offset+code* to the out-of-repo harness crash. Two independent reproductions crash at the same coreclr address.
- **Minimal load = ZERO external load**: attempt 1 (burn=0) crashed — the intra-process parallelism of the two collections **alone** kills the host. External load only shifts the manifestation between soft (fast-fail at registration) and hard (ticks-deep, then a freed-read).

### Baseline (control)

`dotnet test … --filter "FullyQualifiedName~NativeGraph_FiveThousandSystems_RandomDag"` (the stress test **alone**): **1/1 passed** in 44 s — confirming the F-ledger "green in per-project isolation." The defect is purely a *parallel-execution* / shared-singleton property.

### Summary of captured values

| Item | Value |
|---|---|
| Crash class | native **access violation `0xC0000005`** (+ **heap corruption `0xC0000374`**), faulting `coreclr.dll+0x1d45aa` (AV) / `ntdll.dll` (heap) |
| Faulting frame (managed boundary) | `Native.df_scheduler_compute_static_graph()` (harness); testhost coreclr+`0x1d45aa` (in-suite) |
| Crash rate | harness **3/3** (dedicated writer, ~2.4 s); in-suite **3/4 hard-crash + 1/4 soft** when the two classes are forced concurrent (essentially never clean under parallelism) |
| Tick number | filed 2692 is a **probabilistic collision point, not a constant**; not re-measured literally (the in-suite host died between `ManagedScheduler`∥`S4`, not tied to a NativeGraph tick counter) |
| Minimal load | **one concurrent writer** (harness) / **zero external load** in-suite — the parallel collections alone crash the host |

### Gaps (recorded, not estimated)

- **Full native call stack: not captured.** No `cdb`/`windbg`/`procdump`/`dotnet-dump` on PATH or under Windows Kits; `cmake`/`cl` absent (no dev shell). Partial evidence in its place: the managed boundary frame, the two SEH codes, the **stable `coreclr.dll+0x1d45aa`** offset across both reproductions, and a 9.76 MB heap minidump saved. Debug PDBs exist, so a future session with a native debugger can symbolicate the saved dump directly.

---

## R4 Defect (b) scale wall

### Harness measurement code (verbatim, out-of-repo)

`Register(n,…)` replicates the `SchedulerExtremeTests` registration exactly (unique write id `idx+1`, 0–4 random reads of strictly-lower components), with an internal elapsed budget so the register curve survives a watchdog kill:
```csharp
for (int idx = 0; idx < n; idx++) {
    writes[0] = (uint)(idx + 1);
    int readCount = rng.Next(0, 5);
    for (int r = 0; r < readCount; r++) {
        uint maxLower = (uint)Math.Min(idx, componentPool);
        reads[r] = maxLower == 0 ? 0u : (uint)rng.Next(1, (int)maxLower + 1);
    }
    int ok = Native.df_scheduler_register_system((uint)idx, fqn, rp, (uint)readCount, wp, 1u, 2, 0);
    if ((idx & 4095) == 0 && (Stopwatch.GetTimestamp() - t0) > budgetTicks) { /* REGISTER_DNF */ }
}
// BUILD: register loop, then time compute_static_graph() (one native call; external watchdog bounds a hang):
int r = Native.df_scheduler_compute_static_graph();
// TICK (faithful stress path): fire_timer -> runqueue_size -> drain_runqueue -> compute_per_tick_graph -> tick_begin (alt.)
```
Sweep driver: one **process per N**, per-N `WaitForExit` watchdog; register time recovered from `REGISTER_OK` even when compute is killed. Recipes:
```
foreach N in 1000,5000,10000,30000,50000,90000,120000,250000:  f29harness build N 110   (watchdog 130s)
foreach N in 1000,5000,10000,30000 (ticks=2000):               f29harness tick  N 2000  (watchdog 120s)
```

### N → BUILD cost (register loop + `compute_static_graph`)

| N | register (ms) | compute_static (ms) | phases | status |
|---|---|---|---|---|
| 1 000 | 0.6 | 7.0 | 19 | OK |
| 5 000 | 10.5 | 330.1 | 27 | OK |
| 10 000 | 39.0 | 1 354.3 | 27 | OK |
| 30 000 | 379.3 | 13 497.9 | 27 | OK |
| 50 000 | 1 047.5 | 39 398.1 | 27 | OK (last completing) |
| 90 000 | 3 510.2 | **> 130 000 (killed)** | — | **compute DNF — the knee** |
| 120 000 | 8 343.2 | **> 130 000 (killed)** | — | compute DNF |
| 250 000 | 66 946.9 | **> 130 000 (killed)** | — | compute DNF |

`compute_static_graph` adjacent-ratio fit: 5k→10k ×4.10, 10k→30k ×9.97, 30k→50k ×2.92 ⇒ exponents 2.04 / 2.09 / 2.10 ⇒ **O(N^2.05–2.10) — clean quadratic.** Extrapolating from 50k (39.4 s): 90k ≈ 128 s (= the 130 s watchdog, exactly why it is the first kill), 120k ≈ 227 s, 250k ≈ 985 s (~16 min). Register is independently quadratic (10k→30k ×9.73, 30k→50k ×2.76 ⇒ ~N²·⁰), steepening to ~N²·⁸ at 120k–250k as the linearly-rescanned `systems_` vector overruns cache (memory-bandwidth-bound, not a new algorithmic term).

### N → PER-TICK cost (faithful stress-path replica, 2000 ticks)

| N | loop total (ms) | mean/tick (µs) | p95/tick (µs) | max/tick (µs) | status |
|---|---|---|---|---|---|
| 1 000 | 693.4 | 346.6 | 1 229 | 4 593 | OK |
| 5 000 | 14 451.3 | 7 225.4 | 29 696 | 133 435 | OK |
| 10 000 | 69 701.5 | 34 850.4 | 160 809 | 685 997 | OK |
| 30 000 | — | — | — | — | **DNF (>120s)** |

Per-tick mean fit: 1k→5k ×20.85, 5k→10k ×4.82 ⇒ exponents 1.89 / 2.27 ⇒ **also O(N²)** — `compute_per_tick_graph` runs the same Kahn (O(R²)) over a runnable subset R∝N every tick. At 10k the mean tick is already 34.9 ms; 30k's per-tick (~314 ms) × 2000 + a 13.5 s static build blows the watchdog.

### Where the time goes / the knee / growth order / verdict

- **Resolution note:** timing is at the C-ABI boundary (register vs compute_static vs per-tick). Finer intra-function split (conflict-scan vs edge-build vs Kahn) is **inferred from code** (R2: two O(N²) loops + one O(P·N)), not measured — sub-function timing would need native instrumentation, out of read-only scope.
- **Knee:** `compute_static_graph` crosses ~130 s at **N ≈ 90 000** (50k = 39.4 s completes; 90k killed; quadratic extrapolation lands 90k at ~128 s). **The build cost — the O(N²) Kahn conflict + edge scan — is the wall, not the register loop** (register at 90k is only 3.5 s; register alone would not cross 130 s until N ≈ 550k).
- **Fitted growth order:** build **O(N^2.05–2.10)**; register **O(N^2.0)** (→ ~N^2.8 cache-bound at 120k+); per-tick **O(N^~2)**. All three quadratic.
- **Hypothesis verdict:** **the O(N² conflict/edge scan (H-b1) — decisively supported.** The curve is a *smooth* quadratic with no cliff/plateau; a mutex-deadlock / lock-contention wall (the S1 comment's alternate "native mutex above ~90k") would present as a sudden flat-line at a threshold, not a 7 ms → 330 ms → 1.35 s → 13.5 s → 39.4 s progression. The S1 comment's "hang at 90–100k / CPU to 0" is the O(N²) compute crossing ~2 minutes of pure CPU — a **compute wall, not a stall** — corroborated by R2: there is **no lock on the compute/tick path** to contend on.

---

## R5 Root-cause hypotheses + fix-direction candidates

### Defect (a) — ranked

**H-a1 (CONFIRMED, rank 1): data race on the lock-free process-global native singletons** (`default_scheduler_graph()` primarily; also `default_wake_registry()` and the bus). `df_scheduler_*`/`df_bus_*` take no lock; `SystemGraph` has no mutex; documented contract is single-threaded (`system_graph.h:29-30`). `SchedulerStressTests` and `SchedulerExtremeTests` run in parallel (different xUnit collections, parallelization not disabled) and both reset the process-global native state. Evidence: deterministic 3/3 harness crash with `0xc0000005` **and** `0xc0000374`; in-suite soft (SystemCount 5000→1758; Fast 5M→4.375M) and hard (testhost `0xc0000005` at `coreclr+0x1d45aa`, same fault as the harness). Lives in: `system_graph.cpp:270` + `capi.cpp:1495/1042/1013`; second surface `wake_registry.cpp:196`; third surface the bus singletons.

### Defect (b) — ranked

**H-b1 (CONFIRMED, rank 1): O(N²) in `kahn_topological_sort`** — the write-conflict scan (`system_graph.cpp:187-206`) and edge build (`:208-231`) compare systems pairwise. Confirmed by the O(N^2.08) build curve (R4). This is the S1 comment's own primary hypothesis.
**H-b2 (CONFIRMED, contributing): O(N²) registration** — `register_system` linear dup-id scan per call (`system_graph.cpp:22-26`) ⇒ O(N²) over N registrations. Confirmed by the O(N^2.0) register curve; dominant cost of S7's register phase (66.9 s @ 250k), though the *compute* is the harder wall.
**H-b3 (REFUTED): "native mutex above ~90k."** No mutex on the compute/tick path (R2); the smooth quadratic curve (R4) is compute-bound, not a lock/deadlock signature.

### Shared root or independent?

**Same component surface, independent mechanisms.** Both live in `SystemGraph`/the `default_scheduler_graph()` surface, but (a) is a **missing-synchronization** defect and (b) is an **algorithmic-complexity** defect. Fixing one does not fix the other. They can share **one cascade** (one file, one owner) but need **two distinct fixes + two distinct selftest scenarios**.

### Fix DIRECTIONS ONLY (architect deliberates; no implementation in-repo)

- **(a): first decide whether the "single-threaded scheduler" contract is permanent.** The in-suite repro proved the pollution spans the **bus** too (S3), not just the graph — because the two classes run in parallel and both reset process-global native state. Two non-exclusive directions:
  - **(a-i) Test-isolation / contract-enforcement (cheap, no native change):** put the scheduler/bus-touching test classes in **one shared xUnit collection** so they never overlap, honoring the documented single-threaded contract (`system_graph.h:29-30`). Correct *iff* production never touches these singletons from >1 thread ("one kernel scheduler per process, single-threaded registration/compute"). Closes the observed failures **and** the crash without touching the kernel.
  - **(a-ii) Native synchronization (durable, if concurrency becomes real):** a `std::mutex`/`std::shared_mutex` on `SystemGraph` (+ `WakeRegistry`, + bus) so `clear`/`register`/`compute`/`tick_begin` are mutually exclusive (readers may share post-compute). Required only if a real production path will ever tick concurrently with a mod-load `clear()`/`register()`. Bez kostylei: the C ABI currently makes an **undocumented, unenforced** single-threaded assumption — either enforce it (a-i) or remove it (a-ii).
  - Recon's read for the deliberation: (a-i) is the minimal correct fix for the *filed* defect (a test-isolation violation of a documented contract); (a-ii) is the answer only if the kernel's concurrency model is going to change.
- **(b): index-keyed conflict/edge detection.** Build a `writer_of[component_id] → system_idx` map in one O(N·W) pass (write-write conflict = a component with a second writer); for edges, iterate each system's `reads` and look up `writer_of[r]` → build reduces to **~O((N + E)·k)**. Replace the `register_system` linear dup-scan with an `unordered_set<id>` membership index → O(1) register. Replace the per-layer full re-scan in Kahn with a work-queue seeded from in-degree-0 nodes and decremented along edges → O(N + E).

### Native selftest coverage a fix must add (`selftest.cpp`)

- A **concurrency** scenario: two threads — one looping `df_scheduler_tick_begin`/`compute_per_tick_graph`, one looping `df_scheduler_clear`+`register` — asserting no crash (locks the (a) fix). Today this shape crashes 3/3.
- A **scale** scenario: register 50k+ systems and `compute_static_graph`, asserting completion under a wall-clock budget (locks the (b) O(N·k) build against an O(N²) regression).

### S1/S2/S7 re-enable prospect

- **S1 (50k×3k) and S7 (250k register+build): unblocked by (b).** S7 is dominated by the single 250k `compute_static_graph` (~16 min extrapolated); S1 by 3000× per-tick O(R²) recompute (~tens of minutes) + a ~39 s static build. Both collapse to seconds under the O(N·k) rewrite; their `[Fact(Skip=…)]` can then be removed with completion evidence.
- **S2 (200k ticks, 80 systems): NOT a native-scale case — the (b) fix does not unblock it.** With 80 systems the native graph is trivial and built once; S2 is a **managed `ParallelSystemScheduler` TPL marathon** (its own comment `:232-238` notes it does not rebuild the graph). Its non-completion is the 200 000-tick wall-clock × per-tick TPL barrier overhead. S2 re-enables on a **tick-count trim** (or a managed-dispatch optimization), independently of the native fix. S2 is **mis-grouped** under "native scheduler scale pathology."

---

## R6 Anomalies + sizing

### Divergences from the kickoff's stated expectations

1. **The `tick_begin` C-ABI path is scheduler+wake only, not the bus.** `df_scheduler_tick_begin` (`capi.cpp:1495`) never enters `bus_*.cpp`. *However*, defect (a) as a **class** (unsynchronized process-global singleton race under parallel collections) **does** also manifest on the bus singleton — the in-suite hard crash caught `ManagedScheduler`∥`S4` (bus), and S3's bus count corrupted. So the F-ledger's "SystemGraphInterop/bus … path" is right that both surfaces are implicated, but they are **two different singletons** raced by the same test-parallelism, not one bus TickBegin path.
2. **Crash tick 2692 is not a constant** — a probabilistic collision point (R3).
3. **"Native mutex above ~90k" is not real** — no mutex on the compute/tick path; the wall is pure O(N²) compute (R4/R5-H-b3).
4. **`register_system` is itself O(N²)** — a *second* quadratic site (`:22-26`) distinct from the Kahn conflict scan (`:187-206`); the kickoff's "register-conflict scan" phrasing conflates the two.
5. **In-code xUnit `[Collection]` misconception** (`SchedulerExtremeTests.cs:42`) — asserts cross-class isolation a collection does not provide; the latent enabler of (a).
6. **S2 is a managed marathon, not native-scale** (R5) — mis-grouped with S1/S7 in F-29(b).
7. **Manifestation spectrum + reliability** — the same race is **soft** (state corruption, host survives) or **hard** (testhost `0xC0000005`) depending on timing; forcing just the two classes concurrent makes it **reliable** (4/4 attempts failed or crashed), whereas a full "survey" run **dilutes** it to intermittent (the F-10 "reproduced twice") because dozens of other classes desynchronize the overlap.

### Sizing — one cascade or two?

**One native-scheduler cascade, two internal fixes** ((a) synchronization/isolation; (b) de-quadratic-ify), because both center on `system_graph.cpp` + the `capi.cpp` scheduler wrappers and one executor can hold both. Split into two only if the architect wants (a) — the *crashing* one — shipped ahead of (b).

- **Layer split:** (b) is **native-only** (`system_graph.cpp`/`.h`, ± `capi.cpp`). (a) is **either** a **test-layer** fix (a-i: a shared xUnit collection in `tests/` + a contract note — *no native change*) **or** a **native** fix (a-ii: locks in `system_graph.*`/`wake_registry.*`/bus). No `src/` managed *logic* change is required for (b); (a-i) touches only test code.
- **Estimated commit classes:**
  1. native fix (b): index-keyed conflict/edge build + O(1) register + work-queue Kahn — `system_graph.cpp`/`.h`.
  2. fix (a): **either** a `tests/` shared-collection + contract comment (a-i), **or** native synchronization across `system_graph.*` / `wake_registry.*` / bus (a-ii) — architect's choice.
  3. selftest scenarios: + concurrency scenario, + scale scenario (`selftest.cpp`).
  4. test un-quarantine: remove `[Fact(Skip=…)]` on **S1 + S7** with completion evidence (S2 separately, via a tick-trim, if at all).
  5. governance closure: ROADMAP F-29 → CLOSED, REGISTER cascade (`register_version` bump), EVT entry; enroll this report at the cascade's first commit.

---

## R7 Self-attestation

- **Zero writes to the repo except the report file at `docs/reports/` (validate NOT run):** confirmed — the only new/modified path in the repo tree is `docs/reports/F29_NATIVE_SCHEDULER_RECON_REPORT.md`; `sync_register.ps1`/`render_register.ps1` were never invoked; `REGISTER.yaml`/`ROADMAP.md` read-only.
- **No tracked file edited (SchedulerExtremeTests.cs NOT un-Skipped; no src/native/tests instrumentation):** confirmed — no `src/`/`native/`/`tests/` source was edited; the S1/S2/S7 `[Fact(Skip=…)]` were left intact; the T4 harness lives entirely outside the repo. (`dotnet build`/`dotnet test` refreshed only gitignored `bin/`/`obj/`/TRX artifacts.)
- **Any T4 harness was OUTSIDE the repo tree and deleted at session end; git status clean but the report:** confirmed — harness + sweep scripts + dumps lived under the session scratch dir (`%TEMP%\claude\…\scratchpad`), deleted at close; `git status --porcelain` shows only the untracked report.
- **Report written to `docs/reports/` AND presented in chat (uncommitted):** confirmed — not committed; the fix cascade enrolls it at its first commit.
- **Zero git mutations:** confirmed — no commit/checkout/switch/merge/fetch/stash/branch; working tree unchanged besides the untracked report.
- **Every reproduction recipe / measurement command recorded verbatim:** confirmed (R3/R4 carry the exact commands + filters + env).
- **Test invocation used the `TESTING_STRATEGY §8` no-pipe harness + watchdog throughout:** confirmed — every `dotnet test` ran via `Start-Process` + file redirection + a `WaitForExit` watchdog that kills lingering `testhost`/`vstest`; verdicts read from the TRX; nothing piped into a shell consumer.
