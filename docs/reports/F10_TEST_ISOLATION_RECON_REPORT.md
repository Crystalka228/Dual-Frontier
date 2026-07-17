---
register_id: DOC-E-F10_TEST_ISOLATION_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-07-02
last_modified: 2026-07-02
content_language: en
next_review_due: null
title: F10_TEST_ISOLATION RECON REPORT — 2026-07-02 (R1-R9) — pre-brief measurement recon for the F-10 test-isolation cascade (full failure census Debug + Release; Stress/Extreme/thread-safety characterization; bootstrap-timing sweep; the zombie-testhost stdout-pipe deadlock A/B; fixture-copy MSBuild wiring root-cause; anomalies + scale estimate + proposed decision surface)
last_modified_commit: 88b6f58
review_cadence: none-historical-record
last_review_date: 2026-07-03
last_review_event: Read-only measurement recon 2026-07-02 (executor session, flagship model, LOCAL Skarlet; validate deliberately NOT run -- the known write-trap; REGISTER.yaml read directly; every census/harness expression recorded verbatim). Its R2/R3 failure censuses, R6 stdout-pipe A/B, and R7 fixture-copy root-cause were the load-bearing brief inputs; the §2 [RV] re-verify set confirmed at Phase 0 of the execution session (HEAD e44c27b = origin/main, register 2.21 / 282 DOC / 44 EVT -- H1 clear). Enrolled at the F-10 REGISTER closure.
reviewer: Crystalka
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-A_PRIME_9_1_PHASE_GAMMA_RECON_REPORT, DOC-E-A_PRIME_9_1_PHASE_DELTA_RECON_REPORT). Basis of DOC-D-F10_TEST_ISOLATION_BRIEF; substituted for a survey wave per the brief basis line.'
---

# F10_TEST_ISOLATION RECON REPORT — 2026-07-02

**Session class:** read-only measurement recon (executor session, flagship model, LOCAL on operator machine `SKARLET`).
**Repository:** `D:\Colony_Simulator\Colony_Simulator` — branch `main` @ `e44c27b`.
**Scope:** measure the true runtime state of the managed test suite ahead of the F-10 isolation brief. Nothing fixed; proposed remedies recorded only.
**Deliverable:** this report (new, untracked). The F-10 cascade enrolls it at its first commit.

> **Invocation harness (used for every run below).** To defeat the F-10 stdout-pipe wedge, every suite was launched with `Start-Process dotnet -RedirectStandardOutput <file> -RedirectStandardError <file> -NoNewWindow -PassThru` (file redirection, **no** shell pipe) under a `WaitForExit(<timeout>)` watchdog that, on timeout, snapshots `Get-Process testhost*,vstest*,dotnet` and kills the tree + sweeps orphaned testhosts. Results captured via `--logger "trx;LogFileName=<label>.trx" --results-directory <scratchpad>\trx`. All TRX / blame dumps / console logs were written to the session scratchpad (outside the repo); builds wrote only to gitignored `bin/`/`obj/`. `%TEMP%\df_extreme_progress.log` is written by the Extreme suite itself.

---

## R1 Base state

**Git (local refs only — no `fetch` performed; divergence statements are against the local `origin/main` ref):**

| Fact | Value | Command (verbatim) |
|---|---|---|
| Current branch | `main` | `git rev-parse --abbrev-ref HEAD` |
| HEAD | `e44c27b386d3532c417f3d31f76971d344d34103` | `git rev-parse HEAD` |
| Working tree | **clean** (empty) | `git status --porcelain` |
| `main` HEAD | `e44c27b…` (= HEAD) | `git rev-parse main` |
| `origin/main` (local ref) | `e44c27b…` (= HEAD, no divergence) | `git rev-parse origin/main` |
| Commits after `e44c27b` | **none** | `git log e44c27b..HEAD --oneline` (empty) |

HEAD `e44c27b` = *"docs(reports): arc-closure report §10 Modding terminal-state correction (F-10 wedge released 398/398)"* — the expected pre-brief HEAD.

**Register (read directly from `docs/governance/REGISTER.yaml`; NOT via `sync_register.ps1`):**

| Field | Value | Command (verbatim) |
|---|---|---|
| `register_version` | `2.21` | `rg 'register_version' REGISTER.yaml` → line 11 |
| documents | **282** | `rg --count '^  - id: DOC-' REGISTER.yaml` |
| audit-trail events (EVT) | **44** | `rg --count 'id: EVT-' REGISTER.yaml` |
| `last_modified` / `_commit` | `2026-07-02` / `8ec64c5` | REGISTER.yaml lines 12–13 |

**Environment:** dotnet SDK **10.0.204**; `Environment.ProcessorCount` = **16** logical cores; test projects target **net8.0**. Native artifacts present at **both** configs: `native/DualFrontier.Core.Native/build/{Debug,Release}/DualFrontier.Core.Native.dll` and `df_native_selftest.exe` all exist ⇒ no `DllNotFoundException` risk (relevant because `Modding.Tests.csproj` copies the **Release** native DLL regardless of test config — see R7). `ProcessorCount ≥ 8` ⇒ the Extreme suite's `SkipIfUnderpowered` guard does **not** skip; the Stress suite's `ProcessorCount < 2` early-returns do not fire.

**Test-project inventory** (9 test projects confirmed; each references `Microsoft.NET.Test.Sdk` + `xunit` per `rg 'Microsoft.NET.Test.Sdk|xunit' tests/**/*.csproj`):

`Analyzers.Tests`, `Application.Tests`, `Core.Interop.Tests`, `Core.Tests`, `Mod.ManifestRewriter.Tests`, `Modding.Tests`, `Persistence.Tests`, `Runtime.Tests`, `Systems.Tests`.
Non-test under `tests/`: **19** `Fixture.*` (mod fixtures), `DualFrontier.Runtime.SmokeTest` (Exe), `DualFrontier.Core.Benchmarks` (BenchmarkDotNet Exe).

**Build-configuration convention.** `TESTING_STRATEGY.md` defines the closure/sweep baseline in **Release** (§2.1 landscape table, §2.7 "1036/1034 … Release, per-project", §8 invocation strings all `-c Release`). **This recon's kickoff names Debug as its sweep/regression anchor.** Both were measured; they diverge only on the Modding fixture-copy surface (R3/R7) and are otherwise identical. This Debug-vs-Release question is itself part of the F-10 decision surface (R8).

**F-ledger anchors (read from `docs/ROADMAP.md`):**
- **F-10** (S1, OPEN) — the finding this recon underwrites: two pre-existing failures on `main` (full-suite 1034/1036): `SchedulerStressTests.NativeGraph_…` (TickBegin fail @ tick 2692 + testhost crash, `Category=Stress`, reproduced twice under survey load) + `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` (timing-sensitive, empty stateCommands). Resolution owner: *"reproduce in isolation (no concurrent build load), then fix or reclassify as load-flake with an isolation gate."*
- **F-28(a)** (S3, OPEN) — `TESTING_STRATEGY §3.8` still declares meta-tests "NONE exist"; false since Phase β (`CensusMetaTests` live in `tests/DualFrontier.Analyzers.Tests`). Confirmed (R8).
- **F-25** (CLOSED) — `deferred` marker baseline is **82/51** (the ledger's 81/51 was a method artifact).

---

## R2 Full failure census, Debug (the recon sweep configuration)

`dotnet build DualFrontier.sln -c Debug` → **Build succeeded, 0 Warning(s), 0 Error(s)**, 25.7 s (confirms `TreatWarningsAsErrors` + the 17 shipped analyzer rules are clean on the tree). Per-project runs then used `--no-build` (clean isolation, no concurrent build load — the exact condition F-10's resolution asks for).

**Per-project command form (verbatim):** `dotnet test <csproj> -c Debug --no-build --logger "trx;LogFileName=<label>.trx" --results-directory <scratchpad>\trx --nologo [--filter <F>]` (launched via the R0 harness).

| Project | Passed / Total | Failed | Skipped | Wall (test dur) | Verdict |
|---|---:|---:|---:|---|---|
| `Analyzers.Tests` | 54 / 54 | 0 | 0 | 17.2 s (10 s) | green — the Phase β 54-test suite (not the stale 1-Fact placeholder) |
| `Application.Tests` | 45 / 45 | 0 | 0 | 4.5 s | green |
| `Core.Interop.Tests` | 202 / 202 | 0 | 0 | 4.5 s | green |
| `Core.Tests` — `Category!=Stress&Category!=Extreme` | 81 / 81 | 0 | 0 | 5.3 s | green (clean subset) |
| `Core.Tests` — `Category=Stress` | 4 / 4 | 0 | 0 | 58.3 s | **green in isolation** (F-10 #1 did NOT reproduce) |
| `Core.Tests` — `Category=Extreme` | 8 / 11 | 0 | 0 | see R4 | 8 pass; **S1, S2, S7 do not complete** within watchdog |
| `Mod.ManifestRewriter.Tests` | 7 / 7 | 0 | 0 | 4.0 s | green |
| `Modding.Tests` (full) | 398–399 / 399 | 0–1 | 0 | ~47–51 s | **flaky** — 1 `RunningLoop` integration test flakes under intra-suite load (R4) |
| `Persistence.Tests` | 4 / 4 | 0 | 0 | 4.0 s | green |
| `Runtime.Tests` | 292 / 292 | 0 | 0 | 14.3 s | green |
| `Systems.Tests` | 2 / 2 | 0 | 0 | 5.4 s | green |

**Aggregate suite size (Debug):** 54+45+202+96+7+399+4+292+2 = **1101 tests** (Core.Tests = 81 clean + 4 Stress + 11 Extreme = 96).

**Green baseline (proposed regression anchor).** In per-project isolation, **1098 / 1101 pass deterministically.** The 3 that do not reach a verdict are the Extreme extreme-scale scheduler tests **S1 / S2 / S7** (do-not-complete within a 120–180 s watchdog — R4/R6). Two further clusters pass in isolation but fail under concurrent load (so they are **not** deterministic failures): the 4 `SchedulerStressTests` and the 3 `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_*` tests.

**Failing / non-completing tests, Debug:**

| Test ID | Failure mode | Context |
|---|---|---|
| `…GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` | assertion-failure (`stateCommands` empty) | flakes only under full-Modding intra-suite load; **this is the F-10 #2 named test** |
| `…GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` | assertion-failure (`tickCommandCount`=1, needs ≥2) | same family; flaked in one full-Modding run |
| `…SchedulerExtremeTests.S1_NativeGraph_HundredThousandSystems_…` (50 000 systems × 3 000 ticks) | timeout / does-not-complete (compute-bound) | native scheduler scale — R4/R6 |
| `…SchedulerExtremeTests.S2_ParallelSystemScheduler_TwoHundredThousandTicks_…` | timeout / does-not-complete (compute-bound, testhost 444 CPU-s) | managed scheduler scale — R4 |
| `…SchedulerExtremeTests.S7_NativeGraph_QuarterMillionSystems_RegisterAndBuildOnly` (250 000 systems) | timeout / does-not-complete (compute-bound, testhost 1093 CPU-s) | native scheduler scale — R4/R6 |

Note: `SchedulerStressTests.NativeGraph_FiveThousandSystems_…` (F-10 #1) is **absent** from this table — it passed 4/4 in isolation.

---

## R3 Failure census, Release (fixture-copy gap surface)

`dotnet build DualFrontier.sln -c Release` → **0 Warning(s), 0 Error(s)**, 14 s.

**Release parity — all 8 non-Modding projects re-run `-c Release --no-build`** (command form as R2 with `-c Release`): identical to Debug —
`Analyzers 54/54 · Systems 2/2 · Persistence 4/4 · ManifestRewriter 7/7 · Application 45/45 · Core.Interop 202/202 · Runtime 292/292 · Core.Tests(clean) 81/81`. No Release-only failures in these.

**Modding under Release — the divergence surfaces (see R7 for the mechanism):**

| Invocation | Fixtures staged in `bin\Release\…\Fixtures\` | Result |
|---|---:|---|
| `dotnet test …Modding… -c Release --no-build` (against the **solution-build** output) | **5** | **Failed! 15 / 399** |
| `dotnet test …Modding… -c Release` (**per-project build**) | **18** | **Passed! 399 / 399** |

**Release-only failing tests (15) under the solution-build state — all in `Pipeline/` except the last:**

*14 fixture-gap failures* (each fails because a disk-only fixture dir is absent — e.g. *"Failed to read manifest for `…\bin\Release\net8.0\Fixtures\Fixture.RegularMod_DependedOn`: mod.manifest.json not found"*):
`M51PipelineIntegrationTests.{Apply_WithRegularModCycle_…, Apply_WithSatisfiedDeps_…, Apply_WithMissingRequiredDep_…, Apply_WithMissingOptionalDep_…}`, `M52IntegrationTests.{Apply_WithIncompatibleApiVersion_…, Apply_WithIncompatibleDepVersion_…, Apply_WithCascadeFailure_…}`, `M62IntegrationTests.{Apply_WithProtectedSystemReplacement_…, Apply_WithUnknownFqn_…, Apply_WithoutAnyReplaces_…, Apply_WithTwoModsReplacingSameSystem_…, Apply_WithReplaceableBridge_…}`, `M73Phase2DebtTests.{RegularMod_DependedOn_LoadsAndUnloadsCleanly_…, RegularMod_ReplacesCombat_LoadsAndUnloadsCleanly_…}` — failure mode: **assembly/fixture-load-failure** (missing manifest/dir).

*1 coincidental timing flake* (not fixture-related): `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`.

**Coverage note.** Run under Release: all 9 projects (8 via `--no-build` parity + Modding both ways). The documented full-solution command `dotnet test DualFrontier.sln -c Release` (§8) was **not** executed end-to-end (time-boxed; it is also the highest-risk run — solution-wide load is the exact trigger for the Stress crash and the Extreme scale hangs). **Inference (not measured):** because `dotnet test <sln>` builds the solution, it shares the solution-build fixture-staging gap, so the documented full-solution Release command is expected to reproduce the 14 fixture failures unless preceded by a per-project Modding build. This should be confirmed by the brief before relying on §8's full-solution string.

---

## R4 Stress / extreme / thread-safety cluster characterization

Isolation method (verbatim): `dotnet test <csproj> -c Debug --no-build --filter "<F>"` via the harness; repeat-runs via a lean `Start-Process`+watchdog loop capturing the `Passed!/Failed!` summary per iteration.

| Test / class | Isolation result | Flake rate observed | Classification | What it asserts / why fragile |
|---|---|---|---|---|
| `SchedulerStressTests` (4 tests: NativeGraph 5k, ManagedScheduler wide+chain, BusTiers 3-tier, SchedulingPolicies) | **4/4 pass** (58 s) | 0/1 in isolation; F-ledger: 2× crash under survey load | **environment-sensitive (load-induced)** | Hammers native `SystemGraphInterop`/bus under contention; heavy per-test `ResetNativeGlobals()` (news `ManagedBusBridge`, `df_bus_clear()`, resets 4 native singletons). F-10 #1's "TickBegin fail @ tick 2692 + testhost crash" did **not** reproduce without concurrent build/test load. |
| `SchedulerExtremeTests.S1_NativeGraph_HundredThousandSystems` (body = **50 000** systems × 3 000 ticks) | **does not complete** (>120 s) | 2/2 non-completion | **genuine scale pathology** (deterministic non-completion; time-boxed) | Registration reaches ≥40 000 in ~1 s (progress log), then stalls through `ComputeStaticGraph(50k)` + the 3 000-tick per-tick recompute. Code comment documents O(N²) register write-conflict scan / native mutex above ~90k. blame-hang dumped it (R6). |
| `SchedulerExtremeTests.S2_ParallelSystemScheduler_TwoHundredThousandTicks` | **does not complete** (>120 s, testhost 444 CPU-s) | 1/1 non-completion | **genuine scale pathology** (compute-bound) | Managed `ParallelSystemScheduler` 200k ticks. |
| `SchedulerExtremeTests.S7_NativeGraph_QuarterMillionSystems_RegisterAndBuildOnly` | **does not complete** (>110 s, testhost 1093 CPU-s) | 1/1 non-completion | **genuine scale pathology** (compute-bound) | 250 000-system register+build — squarely in the documented ~90k+ hang zone. |
| `SchedulerExtremeTests` — S3, S4, S5a, S5b, S6, S8, S9, S10 | **8/8 pass** | 0 | green (S6 = 60 s "SixtySecondMarathon" by design) | Bus-tier throughput/latency/coalesce + cross-tier no-deadlock. |
| `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_*` (3 tests: PawnStateCarriesRealName, CarriesTopSkills, PublishesTickAdvanced) | **5/5 runs green in isolation** (filter `~CreateLoop_RunningLoop`) | **4/5 full-Modding runs failed** (see below) | **nondeterministic-timing (flaky)** | `Loop.Start()` → `Thread.Sleep(250–500 ms)` → `Stop()`, then assert commands were published in the window. Under xUnit **intra-suite parallelism** the GameLoop background thread starves → 0–1 commands observed → assertion fails. |
| `TickSchedulerThreadSafetyTests` | **1/1 pass** | 0 | green (threading-sensitive by nature; no failure observed) | — |
| `ParallelExecutionTests` | **3/3 pass** | 0 | green | — |
| `DependencyGraphTests` | **10/10 pass** | 0 | green | — |
| `ModDependencyGraphStressTests` (Modding, `Category=Stress`) | **4/4 pass** | 0 | green | Ran clean both isolated and inside the full Modding suite. |

**Full-Modding flake rate (realistic, under intra-suite load), 5 runs of `dotnet test …Modding… -c Debug --no-build`:**

| Run | Result | Failing test |
|---|---|---|
| 1 | 398/399 | `…PublishesTickAdvancedCommandsThroughBridge` |
| 2 | 398/399 | `…PawnStateCommandCarriesRealName` |
| 3 | 398/399 | `…PawnStateCommandCarriesRealName` |
| 4 | **399/399** | — |
| 5 | 398/399 | `…PawnStateCommandCarriesRealName` |

⇒ **~80 % of full-Modding runs show one `RunningLoop` failure**; the specific member rotates (`PawnStateCommandCarriesRealName` 3/5 — the most fragile and the F-10-named one — `PublishesTickAdvanced` 1/5, all-green 1/5). **In isolation the family is 5/5 green.** Isolation cures it.

---

## R5 Bootstrap-timing failure

**Finding: there is no test that asserts on a bootstrap duration/timing threshold.** Sweep (verbatim): `rg -i "Stopwatch|Elapsed|TotalMilliseconds|BeLessThan|duration|timeout|Task.Delay|Thread.Sleep" tests/**/*.cs`.
- `Core.Interop.Tests/BootstrapTests.cs` (a kickoff candidate) contains **no** timing assertion — it is pure functional coverage (`Bootstrap.Run()` returns a ready `NativeWorld`, entity/component ops, disposal). 
- `GameBootstrapIntegrationTests` (the other candidate) is timing-**sensitive** (`Thread.Sleep(250–500 ms)` windows) but asserts on **content** (`stateCommands.NotBeEmpty()`, `tickCommandCount ≥ 2`), **not** elapsed time — it is the R4 flake, not a threshold test.

**The only hard timing-threshold assertions in the suite** are the two Extreme p99.9 latency gates (`SchedulerExtremeTests.cs:688`, `[Collection("ExtremeSerial")]`, ≥8-core-gated). Measured on this idle 16-core machine:

| Test | Gate (p999) | Measured p999 | P50 | P99 | Max (single sample) | Headroom |
|---|---|---:|---:|---:|---:|---:|
| `S5a_…PublishLatency_…OneSubscriber` | **< 1 ms** (`1_000_000` ns, "К-L15 contract") | **196.7 µs** | 0.5 µs | 61.0 µs | 5.35 ms | ~5.1× |
| `S5b_…PublishLatency_…ThirtyTwoSubscribers` | **< 5 ms** (`5_000_000` ns, "relaxed for 32 subs") | **56.2 µs** | 6.4 µs | 12.4 ms | ~89× |

Both **PASS** deterministically (isolated, 728 ms). They are machine-calibrated **round-number** contracts, not tight fits. **S5a is the load-fragile one** (~5× margin, and its own worst single sample already reached 5.35 ms > gate); under heavy concurrent load its p999 could breach 1 ms. `gateP999Ns` is a literal constant (source `SchedulerExtremeTests.cs:566` / `:594`), i.e. machine-agnostic — the calibration is "generous enough for any ≥8-core dev box", confirmed here.

---

## R6 Zombie-testhost shutdown wedge (the load-bearing measurement)

The "~50-minute wedge" resolves into **two distinct mechanisms**, only one of which is a shutdown/pipe phenomenon. Both were reproduced and instrumented.

### (A) Shutdown **stdout-pipe deadlock** — the true wedge

**A/B on the identical clean Modding suite** (399 tests, all passing):
- **File-redirect** (`… > file`, the harness): **EXITED cleanly at 50.9 s.** Full green buffer written.
- **Pipe** (`dotnet test … | Add-Content file`, the historical invocation): **WEDGED — no exit at 150 s** (killed). At the wedge the lingering **`testhost` PID 15160 held only 22 CPU-seconds** — i.e. **idle** (its tests were done), not computing. The **green summary never reached the piped log** (`Add-Content` was still blocked reading the pipe).

**Interpretation.** After all tests report complete, the testhost lingers briefly in shutdown holding the **write end of the inherited stdout pipe**; the PowerShell pipeline reader (`Add-Content`/`Tee-Object`/`Out-File` in a loop) blocks waiting for an EOF that does not arrive until the testhost finally exits → the shell pipeline wedges (the operator's ~50-min figure; time-boxed here at 150 s). This is a **harness/invocation defect, not a test defect** — the same suite is clean under file redirection.

- **Trigger project:** reproduced on **Modding** (largest suite; longer teardown window) but the mechanism is invocation-form-general, not Modding-specific.
- **Lifecycle point:** **after** all tests are reported complete (shutdown/teardown), not during a test.
- **Reliability:** pipe form 1/1 wedged; file-redirect 6/6 exited clean. Deterministic on the invocation variable.
- **What holds the process:** the idle lingering `testhost` (low CPU) holding the inherited stdout pipe handle.
- **Kill-releases-green?** **Yes — via the TRX, not via the piped stdout.** VSTest writes the TRX to disk independently of stdout; killing the tree releases the pipeline and the TRX carries the full green result (this is what "F-10 wedge released 398/398" in HEAD's commit message records). The **piped console buffer is lost** on kill (Add-Content never flushed) — which is exactly why the operator saw wedged pipe-loops with empty output files.
- **Proposed remedy (record only):** invoke via `Start-Process` + `-RedirectStandardOutput`/`-RedirectStandardError` to files + a `WaitForExit` watchdog + orphan-testhost sweep (as this recon's harness does); never pipe `dotnet test` through a streaming consumer. Optionally set `VSTEST_HOST_DEBUG=0` / rely on `--logger trx` for the durable result.

### (B) In-test **compute-hang** (Extreme scale tests) — a different animal

`dotnet test …Core.Tests… -c Debug --no-build --filter "Category=Extreme" --blame-hang --blame-hang-timeout 120s --blame-hang-dump-type full` (verbatim):
- 5 Extreme tests passed in ~63 s, then **`S1_NativeGraph_HundredThousandSystems` hung**; blame fired at 2 min inactivity, **dumped `testhost` PID 22440** and aborted. Artifacts (scratchpad): `…\322c6967-…\testhost_22440_20260702T225013_hangdump.dmp` (**190.8 MB**) + `Sequence_….xml` (shows **S1 `Completed="False"`**, the 5 others `True`). stderr: *"The test running when the crash occurred: …SchedulerExtremeTests.S1_…"*.
- **Localization without cracking the dump** (`dotnet-dump` is not installed; a global-tool install is out of scope): `%TEMP%\df_extreme_progress.log` shows S1 `BODY ENTER` 22:48:12.984 → registration `sysIdx=40,000` by 22:48:14.1 (~1 s) → **silence** through `ComputeStaticGraph` + the 3 000-tick loop. So S1 is **compute-bound in the native per-tick recompute**, not stuck in registration and not deadlocked-idle (the earlier loaded run showed the testhost at 1093–1116 CPU-s = actively burning cores). S2 (200k ticks, 444 CPU-s) and S7 (250k systems, 1093 CPU-s) reproduce the same non-completion.
- These are **DURING-test** hangs; **file redirection does not contain them** (the test itself never finishes) — only a watchdog/blame-hang timeout does. Distinct from (A).

**Relationship to the ~50-min wedge.** The heavy Core.Tests scheduler tests are the *aggravators*: run **piped** (invocation A) while a heavy testhost is crashing (Stress under load) or hanging (Extreme S1/S2/S7), the orphaned/lingering testhost holds the pipe and the shell wedges for a long time. File-redirect + watchdog + running the Stress/Extreme chunk **last and separately** (per the standing lesson) contains all three.

---

## R7 Fixture-copy MSBuild wiring

**Mechanism.** There is **no** `tests/Directory.Build.targets` and **no** copy target in `DualFrontier.Modding.Tests.csproj`. Staging is done by a per-fixture target duplicated in **every** `Fixture.*.csproj`:

```xml
<!-- tests/Fixture.<Name>/Fixture.<Name>.csproj  (lines 19–27) -->
<Target Name="DeployToTestFixtures" AfterTargets="Build">
  <ItemGroup>
    <FixtureFiles Include="$(TargetPath)" />
    <FixtureFiles Include="$(MSBuildThisFileDirectory)mod.manifest.json" />
  </ItemGroup>
  <Copy SourceFiles="@(FixtureFiles)"
        DestinationFolder="$(MSBuildThisFileDirectory)..\DualFrontier.Modding.Tests\bin\$(Configuration)\$(TargetFramework)\Fixtures\$(AssemblyName)\"
        SkipUnchangedFiles="true" />
</Target>
```

`Modding.Tests.csproj` references the 18 fixtures with `<ReferenceOutputAssembly>false</ReferenceOutputAssembly><Private>false</Private>` (they build but their types stay out of the compile graph — the cross-ALC projection pattern, `TESTING_STRATEGY §2.3`). The runtime consumer, `tests/DualFrontier.Modding.Tests/Sharing/TestModPaths.cs:15-16`, computes `Path.Combine(AppContext.BaseDirectory, "Fixtures")` and reads `Fixtures/<AssemblyName>/` (e.g. `Fixtures/tests.shared.events`, `Fixtures/Fixture.PublisherMod`).

**Destination is `$(Configuration)`-parametrized, NOT hardcoded** — so the divergence is not a hardcoded-Debug path.

**On-disk comparison after the R2/R3 solution builds:**

| `…\DualFrontier.Modding.Tests\bin\<cfg>\net8.0\Fixtures\` | subdir count | which |
|---|---:|---|
| **Debug** | **18** | all 18 referenced fixtures (dll + manifest each) |
| **Release** (after `dotnet build DualFrontier.sln -c Release`) | **5** | only the in-solution fixtures: `Fixture.BadRegularMod`, `Fixture.PublisherMod`, `Fixture.SubscriberMod`, `tests.bad-shared-imod`, `tests.shared.events` |

**Why Release diverges (root cause).** The **13 `Fixture.RegularMod_*` are not members of `DualFrontier.sln`** (they are disk-only, pulled only transitively via `Modding.Tests`' ProjectReferences — `TESTING_STRATEGY §2.3`). A **solution-scoped** `dotnet build <sln> -c Release` builds solution members in Release but resolves these non-member ProjectReferences to their **default (Debug)** output — the Release build log literally emits `Fixture.RegularMod_MissingOptional -> …\bin\Debug\net8.0\…`. Their `DeployToTestFixtures` therefore fires with `$(Configuration)=Debug` and stages into `bin\Debug\…\Fixtures`, so `bin\Release\…\Fixtures` never receives them. The Debug side "works" only because Debug happens to be the fixtures' default configuration. A **per-project** `dotnet test …Modding… -c Release` propagates Release through the ProjectReferences, rebuilds all 13 in Release, and stages **18/18** → 399/399 (R3).

**Responsible wiring (file:line):** `tests/Fixture.RegularMod_*/Fixture.*.csproj:24-26` (the `<Copy DestinationFolder="…\bin\$(Configuration)\…">`) **combined with** the 13 fixtures' absence from `DualFrontier.sln`. The `$(Configuration)` there resolves to the fixture project's *own* build config, which a solution build leaves at Debug for non-members.

**Also latent (native DLL):** `Modding.Tests.csproj:24-29` copies `native\…\build\**Release**\DualFrontier.Core.Native.dll` (hardcoded `Release`, `Condition="Exists(…Release…)"`) into the test output regardless of test config. Harmless today (both native configs exist on disk), but a clean tree with only a Debug native build would leave Modding.Tests with no native DLL under any config → `DllNotFoundException`. Record-only.

**Proposed remedies (record only, for the brief to choose):** (a) add the 13 `Fixture.RegularMod_*` to `DualFrontier.sln` so solution builds stage them in Release; or (b) hoist `DeployToTestFixtures` into a `tests/Directory.Build.targets` that force-copies from a config-correct source; or (c) codify **per-project `dotnet test` as the only supported Release route** in `TESTING_STRATEGY §8` and add a meta-test asserting `Fixtures/` count = 18 at runtime (fail fast instead of 14 cryptic manifest-not-found errors).

---

## R8 Anomalies + scale estimate + proposed decision surface

**Anomalies vs. kickoff expectations:**
1. **F-10 #1 does not reproduce in isolation.** `SchedulerStressTests` is 4/4 green alone; the tick-2692 crash is load-induced (confirms the F-ledger hypothesis).
2. **F-10 #2 is one member of a 3-test flake family**, and *which* member fails rotates run-to-run. The failing test in a given full-Modding run is not stable.
3. **New non-completers not named in F-10:** `SchedulerExtremeTests.S1` (50k×3k), `S2` (200k ticks), `S7` (250k systems) do not complete within 120–180 s. `S1`'s method name says "HundredThousand" but its body uses `SystemCount = 50_000`.
4. **The wedge is a stdout-pipe deadlock**, reproducible on an all-passing suite (Modding) — no crash/hang required (R6-A). This is broader than "the stress test wedges."
5. **§8's documented fast sweep (`Category!=Stress`) does not exclude Extreme** → any run honoring only that filter will hit S1/S2/S7 and hang. Extreme must also be excluded (or the scale tests gated). 
6. **`Fixture.VanillaMod_HotReloadOverride`** (the 19th disk fixture, an in-solution vanilla fixture) is **not** ProjectReferenced by `Modding.Tests.csproj` and does not appear in either `Fixtures/` listing; it is staged/consumed by a different path (hot-reload override via `mods/Directory.Build.targets` `RewriteHotReloadForRelease`, Release-only). Not a failure, but an inventory asymmetry the brief should note.
7. **F-28(a) confirmed live:** `TESTING_STRATEGY §3.8` still says meta-tests "NONE exist" while `CensusMetaTests` runs inside the 54-test `Analyzers.Tests` suite. Out of F-10 scope; flagged for its own hygiene PATCH.
8. **`--no-build` census caveat:** the R2 green baseline was measured `--no-build` after a clean solution build (deliberate — F-10's "no concurrent build load"). The historical 1034/1036 was a *solution-wide, concurrently-built* run; the delta (load-induced Stress crash + RunningLoop flake) is the whole point of F-10.

**Scale estimate — counts per work class:**

| Work class | Count | Items |
|---|---:|---|
| genuine deterministic defects | **0** | none reproduce deterministically as a *failure* in isolation |
| genuine scale non-completers (do-not-complete; may be defect *or* intentionally-uncapped) | **3** | Extreme `S1`, `S2`, `S7` |
| nondeterministic-timing flakes | **3** | `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_{PawnStateCommandCarriesRealName, CarriesTopSkills, PublishesTickAdvancedCommandsThroughBridge}` |
| environment/load-sensitive (crash under load, green isolated) | **4** | `SchedulerStressTests` (all 4) |
| the wedge | **1 issue** | stdout-pipe shutdown deadlock (invocation-harness class) |
| the fixture-copy gap | **14 tests, 1 root cause** | 13 disk-only fixtures × solution non-membership under Release |

**Proposed commit-class split (a PROPOSAL for the brief to ratify — not a decision):**

- **Code-fix (test infra / product):**
  - *Fixture-copy gap* — pick one R7 remedy (solution membership, or `tests/Directory.Build.targets` hoist, or per-project-only + runtime count meta-test). Closes 14 Release failures + prevents the class.
  - *RunningLoop flake family* — make the 3 tests robust to intra-suite starvation: `[Collection]` them onto a serial, non-parallel collection and/or replace the fixed `Thread.Sleep` window with a bounded poll-until-condition (retry up to N or a generous ceiling). Reclassify F-10 #2 from "known failure" to "fixed".
  - *(optional) native scheduler scale* — if S1/S2/S7 are meant to pass, this is a **product** fix in the native `SystemGraphInterop` register/recompute path (the documented O(N²)/mutex above ~90k). Larger than F-10; likely its own finding.
- **Reclassify / quarantine (skip + recorded rationale, or a trait):**
  - *Stress cluster* — keep `Category=Stress`; add the "isolation gate" F-10 asks for: run Stress **only** in a dedicated serial, no-build-load pass; document it as load-sensitive so a solution-wide crash is not read as a regression.
  - *Extreme scale tests S1/S2/S7* — if a native fix is deferred, gate them behind an explicit opt-in trait (e.g. `Category=ExtremeScale` or a `DF_RUN_EXTREME_SCALE` env guard) and **exclude Extreme from every default/closure sweep** so they never hang a run. Record the deferral in the F-ledger.
- **Standing-law codification (`TESTING_STRATEGY`):**
  - *§8 kill-and-read-buffer protocol* — codify the file-redirect + watchdog + orphan-testhost-sweep invocation and "the TRX is the source of truth on a killed run; never pipe `dotnet test`" (R6-A).
  - *§8 sweep filter* — the fast/closure sweep must exclude **both** `Stress` **and** `Extreme` (currently only Stress).
  - *Debug-vs-Release baseline* — if the fixture-copy gap is *fixed*, keep Release as the §2.1/§8 baseline; if *deferred*, codify the Debug-as-sweep-anchor convention (or "per-project Release only") so the gap cannot masquerade as 14 regressions.
  - *(pairs with F-28(a))* — the §3.8 "NONE exist" correction is adjacent standing-law hygiene the same cascade could fold.

---

## R9 Self-attestation

- **Zero writes to tracked/source files; only the report + gitignored build/test artifacts (validate/sync/render NOT run):** CONFIRMED. `git status --porcelain` empty before authoring; the only new file is this report under `docs/reports/` (untracked). All TRX / 190 MB hang dump / console logs went to the session scratchpad; builds wrote only to gitignored `bin/`/`obj/`; `df_extreme_progress.log` is under `%TEMP%`. `sync_register.ps1` (incl. `-Validate`), `render_register.ps1`, `git clean`, `git add` were **not** run. `REGISTER.yaml` was read directly.
- **Report written to `docs/reports/` AND presented in chat (uncommitted):** CONFIRMED (this file + full chat reproduction).
- **Zero git mutations (no clean/add/commit/switch/fetch):** CONFIRMED. HEAD is still `e44c27b`; no branch switch; divergence read from the local `origin/main` ref only (no fetch).
- **Nothing fixed — proposed remedies recorded only:** CONFIRMED (R6/R7/R8 remedies are proposals for the brief).
- **Every census command/expression recorded verbatim:** CONFIRMED (R1–R7 carry their exact `git`/`rg`/`dotnet test`/`--filter` strings and the harness form).

**Measurement gaps (time-boxed, per rule 6, stated not estimated):**
- Whether Extreme `S1`/`S2`/`S7` would *ever* complete given many minutes is undetermined (watchdog-bounded at 120–180 s; they are compute-bound, not proven-deadlocked).
- The natural ~50-min pipe wedge was reproduced and characterized but **not** waited out to natural release (bounded at 150 s, then killed).
- The 190.8 MB hang dump's managed stacks were **not** decoded (`dotnet-dump` absent; global-tool install out of scope) — localization used the progress log + Sequence file + source comment instead.
- The documented full-solution `dotnet test DualFrontier.sln -c Release` was not run end-to-end (R3 inference recorded, not measured).