# Housekeeping — `TickScheduler.ShouldRun` cache race fix

## Context

Previous housekeeping commit (`3d800d2` — TICK display wiring, ROADMAP Backlog established) closed M7-cycle TICK plumbing. Manual F5 verification by user 2026-05-02: TICK counter advances visibly to 2785+ during gameplay. Wiring works.

During post-commit-3 verification, the new integration test `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` (`1b16e9e` commit 1; `21921887` commit 2) surfaced a **pre-existing latent concurrency race in `DualFrontier.Core/Scheduling/TickScheduler.ShouldRun`** that aborts the test host process roughly 60% of runs (3 of 5 isolated runs in agent's 5-run sample).

Stack trace anchor: `TickScheduler.ShouldRun` → `_tickRateCache.TryGetValue` → `Dictionary` mutation → `ParallelSystemScheduler.<>c__DisplayClass9_0.<ExecutePhase>b__0` → `Parallel.ForEach`.

Root cause: `_tickRateCache` is `Dictionary<Type, int>` (non-concurrent) yet `ParallelSystemScheduler.ExecutePhase` invokes `_ticks.ShouldRun(system)` from inside `Parallel.ForEach(phase.Systems, _parallelOptions, ...)`. On the very first tick the cache is empty; nine worker threads race to populate it concurrently; the dictionary's internal state corrupts and the next access throws `InvalidOperationException("Operations that change non-concurrent collections must have exclusive access")`.

The class's own XML doc says "Not thread-safe: accessed only from the scheduler's driver thread before parallel dispatch of a phase." This claim is false against the actual call site — `ShouldRun` is invoked **inside** the parallel dispatch, not before it.

**Latent since Phase 1.** Never surfaced previously because:
- existing Core scheduler tests use minimal system sets (3-4 systems);
- the M7.5.B.1 `StartStopRoundTripsCleanly` test starts and stops the loop too fast for `Parallel.ForEach` to actually iterate;
- M-cycle pipeline tests bypass production `ParallelSystemScheduler` via test harnesses that don't run the full graph in parallel.

The new bridge integration test from `21921887` is the first test in the project to actually run the production scheduler with all 9 `coreSystems` long enough for parallel dispatch to race on the cache. Surfacing the bug is correct — the test is a valid regression guard for any latent race in the production hot path.

User authorized fix on 2026-05-02 (Option 1 from agent's three-option report-back): cross the M-phase Core/Contracts boundary deliberately to fix the race, parallel pattern to `f4b2cb8` (CODING_STANDARDS housekeeping commit) but in code rather than docs.

## Out of scope

- M7.5.B.2 Godot UI scene — separate session immediately following this housekeeping closure.
- Any other Core changes — strict single-file scope on `TickScheduler.cs`.
- Any change to `ParallelSystemScheduler.cs` — its call to `_ticks.ShouldRun` from inside `Parallel.ForEach` is a valid pattern once the cache is thread-safe; no change needed there.
- Any change to `_currentTick` field — single-threaded by `Advance()` semantics (called by scheduler driver thread between phases per existing comment, which is true for that field even though the cache claim was false).
- Phase 4/5 Backlog items (NeedsSystem decay direction, NeedsComponent rename, BuildMenu stub) — remain Phase 5 territory.
- Modifications to `DualFrontier.Contracts` — empty diff invariant preserved against Contracts.

## Approved architectural decisions

1. **Single-file fix scope: `src/DualFrontier.Core/Scheduling/TickScheduler.cs`.** No other Core file modified. Boundary discipline preserved-by-documentation: this housekeeping commit is the new baseline for future M-phase boundary checks, parallel to how `f4b2cb8` (CODING_STANDARDS commit) became baseline for subsequent M7.x work.

2. **Mechanism: `ConcurrentDictionary<Type, int>`.** Replace `Dictionary<Type, int>` field declaration. Use `GetOrAdd(systemType, ResolveTicksPerUpdate)` in `ShouldRun`. The factory delegate matches `Func<Type, int>` (existing `ResolveTicksPerUpdate` static method already has this exact signature — clean drop-in). On race, losing thread's compute result is discarded, which is harmless: `ResolveTicksPerUpdate` is pure (reflection on a Type) and idempotent.

3. **`Reset()` works unchanged.** `ConcurrentDictionary.Clear()` exists and is the same call pattern. No code change in `Reset()`.

4. **XML doc class-level rewrite.** Remove the false "Not thread-safe: accessed only from the scheduler's driver thread before parallel dispatch of a phase." Replace with truthful: "Thread-safe: the type-rate cache uses `ConcurrentDictionary` so that parallel system dispatch (which queries `ShouldRun` from inside `Parallel.ForEach` over a phase's systems) does not race on cache population. The mutable scalar field `_currentTick` is written only by the scheduler's driver thread between phases via `Advance()` and read by `ShouldRun` callers within a phase; that field's single-writer pattern is unchanged."

5. **No changes to public surface.** `Advance()`, `Reset()`, `ShouldRun()`, `CurrentTick` getter all keep identical signatures and visibility. Behavioural contract identical from caller perspective; only internal storage and concurrency guarantee strengthen.

6. **Test surface: new Core-level stress test + repurpose existing integration test as regression guard.** New `tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs` — single focused test that exercises `ShouldRun` from many threads simultaneously with an empty cache, asserts no exception and correct cache population. Existing `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` becomes the integration-level regression guard — its 100-consecutive-run pass is part of acceptance criteria.

7. **No public API or contract changes.** XML doc tightens claims. `MOD_OS_ARCHITECTURE` spec untouched. No §-anchored ratification needed; the bug is in scheduling implementation, not in modding architecture.

8. **METHODOLOGY §2.4 atomic phase review** — implementation, test, ROADMAP update all in one session. Three commits per §7.3.

## Required reading

1. `src/DualFrontier.Core/Scheduling/TickScheduler.cs` — full file. The fix target.
2. `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` — read the `ExecutePhase` method (lines around 130-150) to confirm `_ticks.ShouldRun` is invoked from inside `Parallel.ForEach`. This proves the race site.
3. `tests/DualFrontier.Core.Tests/Scheduling/` — directory contains existing test files (`ConverterCycleResolutionTests.cs`, `DependencyGraphTests.cs`, `ParallelExecutionTests.cs`). The new `TickSchedulerThreadSafetyTests.cs` joins them; reference these files for the project's existing Core-test conventions (xUnit, FluentAssertions if present, namespacing).
4. `tests/DualFrontier.Core.Tests/DualFrontier.Core.Tests.csproj` — confirm existing test project reference structure; new test file does not need any new project references.
5. `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — the existing `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` test stays unchanged; just rerun for verification.
6. `docs/METHODOLOGY.md` — §2.4, §7.3.
7. `docs/CODING_STANDARDS.md` — full doc. Especially: one class per file, English-only comments, member order, `_camelCase` private fields. **Stack-frame retention** section is not directly relevant here (no GC-collection paths in this fix).
8. `docs/ROADMAP.md` — locate the Backlog section established by the prior housekeeping commit (`3d800d2`). New entry will be appended documenting the TickScheduler race discovery and resolution.

## Implementation

### 1. `TickScheduler.cs` field type change

```csharp
// before
private readonly Dictionary<Type, int> _tickRateCache = new();

// after
private readonly ConcurrentDictionary<Type, int> _tickRateCache = new();
```

Add `using System.Collections.Concurrent;` at the top of the file.

### 2. `ShouldRun` method body simplification

```csharp
// before
public bool ShouldRun(SystemBase system)
{
    if (system is null)
        throw new ArgumentNullException(nameof(system));

    Type systemType = system.GetType();
    if (!_tickRateCache.TryGetValue(systemType, out int ticksPerUpdate))
    {
        ticksPerUpdate = ResolveTicksPerUpdate(systemType);
        _tickRateCache[systemType] = ticksPerUpdate;
    }

    return _currentTick % ticksPerUpdate == 0;
}

// after
public bool ShouldRun(SystemBase system)
{
    if (system is null)
        throw new ArgumentNullException(nameof(system));

    Type systemType = system.GetType();
    int ticksPerUpdate = _tickRateCache.GetOrAdd(systemType, ResolveTicksPerUpdate);

    return _currentTick % ticksPerUpdate == 0;
}
```

The `ResolveTicksPerUpdate` static method matches `Func<Type, int>` exactly — clean delegate group conversion, no lambda needed.

### 3. `Reset()` unchanged

```csharp
// stays as-is
public void Reset()
{
    _currentTick = 0;
    _tickRateCache.Clear();
}
```

`ConcurrentDictionary.Clear()` is the same call.

### 4. XML doc class-level rewrite

Replace the "Not thread-safe..." paragraph at the class-level XML doc:

```csharp
/// <summary>
/// Tracks the monotonically increasing game tick counter and decides whether
/// a given system is due to run on the current tick according to its
/// <c>[TickRate]</c> attribute. Consumed by <c>ParallelSystemScheduler</c>
/// to filter systems before each phase.
///
/// The reflection lookup of <see cref="TickRateAttribute"/> is memoised per
/// concrete system type so the hot path of <see cref="ShouldRun"/> does no
/// reflection or allocation after the first call for each type.
///
/// Thread-safety: the type-rate cache uses <see cref="ConcurrentDictionary{TKey,TValue}"/>
/// so parallel system dispatch (which queries <see cref="ShouldRun"/> from
/// inside <c>Parallel.ForEach</c> over a phase's systems) does not race on
/// cache population. The mutable scalar field <c>_currentTick</c> is written
/// only by the scheduler's driver thread between phases via <see cref="Advance"/>
/// and read by <see cref="ShouldRun"/> callers within a phase; that field's
/// single-writer pattern is preserved.
/// </summary>
```

Method-level XML docs unchanged.

## Tests

### `tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs`

Single focused stress test. Match existing Core.Tests file conventions (xUnit `[Fact]`, namespace `DualFrontier.Core.Tests.Scheduling`).

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

public sealed class TickSchedulerThreadSafetyTests
{
    [Fact]
    public void ShouldRun_InvokedFromManyThreadsWithEmptyCache_DoesNotThrowAndReturnsConsistentResults()
    {
        var scheduler = new TickScheduler();

        // Build a diverse system set with varied [TickRate] attributes so
        // the cache has many distinct keys to populate concurrently. Use
        // 32 distinct synthetic types to exceed Parallel.ForEach worker
        // count on most CI machines.
        SystemBase[] systems = BuildSyntheticSystemPool(count: 32);

        // Every iteration repeats the race window: empty cache, then 32
        // parallel ShouldRun calls. Repeat 50 times to multiply
        // race opportunities; each iteration's Reset clears the cache.
        for (int iteration = 0; iteration < 50; iteration++)
        {
            scheduler.Reset();

            // Capture results so we can assert consistency post-race.
            var results = new bool[systems.Length];

            Parallel.For(0, systems.Length, i =>
            {
                results[i] = scheduler.ShouldRun(systems[i]);
            });

            // All systems on tick 0 with TickRate.REALTIME (1) should
            // return true; systems with higher TickRate may also return
            // true on tick 0 (0 % anything == 0). Just assert no
            // exception thrown and results are consistent with a second
            // serial pass.
            for (int i = 0; i < systems.Length; i++)
            {
                bool expected = scheduler.ShouldRun(systems[i]);
                Assert.Equal(expected, results[i]);
            }
        }
    }

    private static SystemBase[] BuildSyntheticSystemPool(int count)
    {
        // Real production system types from DualFrontier.Systems can't be
        // referenced from Core.Tests without a project reference. Instead,
        // construct anonymous SystemBase subclasses inline. xUnit's
        // ClassData / fixtures aren't needed — keep it self-contained.
        var pool = new List<SystemBase>(count);
        for (int i = 0; i < count; i++)
            pool.Add(new SyntheticSystem());
        return pool.ToArray();
    }

    // Trivial SystemBase subclass with an attributed override to populate
    // the cache. TickRate(1) means REALTIME (every tick).
    [TickRate(1)]
    private sealed class SyntheticSystem : SystemBase
    {
        protected override void OnInitialize() { }
        public override void Update(float delta) { }
    }
}
```

**Implementation notes for the agent:**

- If `SystemBase` cannot be subclassed from outside its assembly without `InternalsVisibleTo` already wiring `DualFrontier.Core.Tests` to Core, that's already the case (verified: `<InternalsVisibleTo Include="DualFrontier.Core.Tests" />` exists in Core.csproj).
- If `[TickRate]` attribute requires a non-zero positive value (the `ResolveTicksPerUpdate` checks `value > 0`), use `1` (REALTIME).
- If `TickRates.REALTIME` constant is publicly accessible from Core.Tests, prefer using that constant over the magic literal `1`.
- If 32 synthetic systems are insufficient to reliably trigger the original race in a single run, increase to 64 or 128. Empirically the original race triggered on 9 systems × ~6 ticks; the test deliberately runs much wider.
- If 50 iterations is empirically insufficient (i.e., test passes with old broken code in some runs), increase to 200. The test must reliably **fail** against `Dictionary<Type, int>` and reliably **pass** against `ConcurrentDictionary<Type, int>`. Verify both directions during commit 2's verification preamble.

### Existing test repurposed as regression guard

`CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` stays unchanged. Run **100 consecutive times** during commit 1's verification preamble; must pass 100/100. If any flake observed, STOP — fix not complete.

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors.
2. `dotnet test` — 416 existing pass; 1 new pass. Expected total: **417/417**.
3. **100 consecutive `dotnet test --filter "FullyQualifiedName~CreateLoop_RunningLoop"` runs pass 100/100.** This is the load-bearing acceptance check — the original 60%-flake pattern must collapse to zero.
4. **New stress test `ShouldRun_InvokedFromManyThreadsWithEmptyCache_DoesNotThrowAndReturnsConsistentResults` reliably fails against the pre-fix code** (briefly verify by reverting the field type to `Dictionary` in a scratch branch and re-running — agent does NOT need to commit this revert, just confirm empirically that the test catches the bug). If the test does not fail against pre-fix code, the test is not actually exercising the race and must be tightened (more threads, more iterations, or different timing).
5. `TickScheduler.cs` is the only file modified in `src/DualFrontier.Core/`. Verify:
   ```
   git diff <baseline>..HEAD --name-only -- src/DualFrontier.Core
   ```
   returns exactly `src/DualFrontier.Core/Scheduling/TickScheduler.cs` (one line).
6. `git diff <baseline>..HEAD --stat -- src/DualFrontier.Contracts` returns empty (Contracts untouched, strict).
7. M7.1 + M7.2 + M7.3 + M7.4 + M7.5.A + M7.5.B.1 + housekeeping (`3d800d2` baseline) regression guards still pass.
8. `dotnet sln list` count unchanged (no new projects).

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `fix(core): TickScheduler.ShouldRun cache thread-safety via ConcurrentDictionary`

- Modify `src/DualFrontier.Core/Scheduling/TickScheduler.cs`: field type change, ShouldRun body simplification, XML doc rewrite, new using.
- No test file changes in this commit. Verify existing M7.x suites + housekeeping smoke tests still pass via `dotnet test` at full count (416/416).
- **Verification preamble**: run `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` **100 consecutive times** via `for /L %i in (1,1,100) do dotnet test --filter "FullyQualifiedName~CreateLoop_RunningLoop" --no-build` (or equivalent shell loop). If any run fails, STOP — fix incomplete or test inadequate.
- Document in commit message body: "Fixes pre-Phase-1 latent race in TickScheduler.ShouldRun cache surfaced by housekeeping commit 21921887's integration test. ParallelSystemScheduler invokes ShouldRun from inside Parallel.ForEach; the Dictionary cache could not handle the concurrent first-tick population. Replaced with ConcurrentDictionary; semantics identical from caller perspective."

**2.** `test(core): parallel stress test for TickScheduler.ShouldRun`

- New file `tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs` with the single stress test.
- Run full suite. Confirm 417/417.
- **Verification preamble**: confirm test reliably fails against pre-fix code (empirical check on scratch branch — do not commit revert). If test passes against pre-fix code, tighten parameters (more threads, more iterations) and rerun until it reliably catches the bug.

**3.** `docs(roadmap): close TickScheduler race finding`

- `ROADMAP.md`:
  - Header status line: `*Updated: YYYY-MM-DD (housekeeping — TickScheduler.ShouldRun cache race fixed; M7.5.B.2 + M7-closure pending; Phase 5 backlog established).*`
  - Engine snapshot: 416 → 417 tests.
  - Backlog section: append new entry under a "Resolved" subsection (or inline noting fix SHA):
    ```
    - **Pre-Phase-1 latent race in `TickScheduler.ShouldRun` cache** —
      `Dictionary<Type, int>` populated from inside `Parallel.ForEach`
      via `ParallelSystemScheduler.ExecutePhase`, against the class's
      own "not thread-safe" claim. Surfaced by housekeeping commit
      `21921887`'s integration test (~60% flake rate, 5-run sample).
      Resolved by housekeeping commit `<sha-of-commit-1>`:
      `Dictionary` → `ConcurrentDictionary`, `TryGetValue + indexer`
      → `GetOrAdd`. New regression guard:
      `TickSchedulerThreadSafetyTests.ShouldRun_InvokedFromManyThreads...`.
    ```
  - Status overview table: no M-row change (this is housekeeping, not M-cycle).

**Special verification preamble — agent reads carefully:**

The 100-consecutive-run check on commit 1 is **load-bearing**. Don't skip it. The original race manifested at ~60% flake rate across 5 runs; statistical confidence on the fix requires substantially more runs. 100 is sufficient for >99% confidence given the original 0.6 flake-per-run rate; with the fix, expected flake rate is 0 and 100 consecutive passes confirm.

If 100 runs is impractically slow (each run is ~300 ms; 100 runs = ~30 seconds plus test discovery overhead, total ~2-3 minutes — should be fine), at minimum run 50.

**If 100 runs surface even one flake**: STOP. Do not proceed to commit 2. Report immediately. The fix is not complete or there's a second race we haven't found. Do not paper over with retry logic, test skip attributes, or window adjustments.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice. Per spec preamble "stop, escalate, lock — never guess".

**Hypothesis-falsification clause:**

This commit is **housekeeping, not an M-cycle phase**. The M-cycle datapoint sequence (M3=1, M4=1, M5=0, ..., M7.5.B.1=0; ten consecutive zeros post-M4) is not incremented by this commit.

However, this commit establishes a **new "Core latent bug discovery" sequence** as datapoint #1. The bug was latent since Phase 1 of project history. M-cycle pipeline instrumentation surfaced it. Future M-cycle work that exercises previously-untested Core code paths may surface additional latent bugs; the Core-discovery sequence tracks those.

Implication for M5 closure review §10 hypothesis: the M-cycle hypothesis ("contradiction-discovery rate decreases asymptotically with each disjoint-section pass") concerned **MOD_OS_ARCHITECTURE spec contradictions**, not Core implementation bugs. The hypothesis is unaffected — this finding is in a different domain. M-cycle hypothesis remains at ten consecutive zeros post-M4.

No plausible v1.6 ratification candidate. The MOD_OS_ARCHITECTURE spec doesn't speak to scheduler internal implementation; this fix is purely below-the-spec internals.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (416 + 1 = 417 expected, or actual with discrepancy noted).
- **Critical: 100-consecutive-run pass rate on `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`** (must be 100/100; report exact count if any flake observed).
- **New test reliably fails against pre-fix code** — confirmation that scratch-branch revert + re-run shows the test catching the bug.
- Per-test confirmation: 1 new test (`ShouldRun_InvokedFromManyThreadsWithEmptyCache_DoesNotThrowAndReturnsConsistentResults`) green; ran reliably across 50+ iterations within the test (the test itself loops internally) and on 3+ separate `dotnet test` invocations.
- Regression confirmation: M7.1 + M7.2 + M7.3 + M7.4 + M7.5.A + M7.5.B.1 + prior housekeeping (`3d800d2`) all green.
- Working tree state: clean.
- **Boundary discipline status**: `git diff <baseline>..HEAD --name-only -- src/DualFrontier.Core` returns exactly `src/DualFrontier.Core/Scheduling/TickScheduler.cs` (one file). `git diff <baseline>..HEAD --stat -- src/DualFrontier.Contracts` empty.
- **Solution file**: `dotnet sln list` count unchanged.
- **ROADMAP Backlog section**: confirm new "Resolved" entry present with both housekeeping commit SHAs (the discovery commit `21921887` and the resolution commit from this session).
- Any unexpected findings, especially any second race surfaced by the 100-run loop, or any unexpected dependency surface that prevents the new stress test from compiling.
