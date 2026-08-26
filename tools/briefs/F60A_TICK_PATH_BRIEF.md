---
register_id: DOC-D-F60A_TICK_PATH_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-08-26'
last_modified: '2026-08-26'
content_language: en
next_review_due: null
title: 'F60A_TICK_PATH -- F-60(a) closure: the tick-path ALC root is the lazy Type-keyed tick-rate cache; resolve the rate at load time, eliminate the cache class, flip the pinned leak test on the production composition; riders: cross-platform (Linux) native test wiring, bus-side Type-key eviction, BRIEF_TEMPLATE 8.4-v3.0.0 alignment, ledger seeding for the unreachable class members'
authored_by: Claude Fable (architect session, F-60(a) charter)
basis: 'DOC-E-F60A_TICK_PATH_RECON_REPORT (architect-seat recon 2026-08-25 at 3173d2b, R1-R5) + DOC-E-F60A_TICK_PATH_VERIFICATION_RECON (independent Opus subagent re-measurement 2026-08-26 at the same HEAD, A1-D3 + 11 discrepancies) + ratified lean set 2026-08-25 (fix shape (a): eliminate the lazy cache; class scope: fix the whole class; riders) + operator environment ruling 2026-08-26 (Linux is the PERMANENT development environment)'
---

# F60A_TICK_PATH -- Execution Brief

This cascade closes F-60 half (a) -- the identity family's last open item. A mod that
ticks even once leaks its collectible ALC at unload because the FIRST `ShouldRun` call
memoises the system's concrete `Type` (for SDK mods: `SystemAdapter<TSystem>` closed
over a collectible-ALC type) into `TickScheduler._tickRateCache`, a session-lived
`ConcurrentDictionary<Type,int>` with zero production evictions. The fix resolves the
tick rate at LOAD time and eliminates the lazy cache entirely; the proof is the pinned
leak test flipping on the production composition. Done = 
`Unload_LeaksTheModAlc_RootIsOnTheTickPath_NotTheTypeRegistry` asserts `BeEmpty` and
passes, the 0-tick control still passes, a new 1-tick minimal pin passes, and the full
gate set is green ON LINUX (the permanent development environment per the operator
ruling of 2026-08-26).

Executor: fresh Claude Code session (flagship model), LOCAL, repository
`/mnt/Work/Colony_Simulator` (GitHub `Crystalka228/Dual-Frontier`). MANAGED-ONLY:
the `native/` tree is byte-untouched (H7).

Brief-integration notice: this brief CITES standing law and does not restate it --
commit-body structure and markers per `CODING_STANDARDS.md`; push law per
`CODING_STANDARDS.md` section 8.4 (v3.0.0); census pins per `TESTING_STRATEGY.md`;
mutability license and `Skeleton revisions` form per `RESERVED_SURFACE_MUTABILITY.md`;
session closure per `METHODOLOGY.md`. A conflict between this brief and any standing
doc means THE BRIEF IS WRONG -- halt and escalate. A conflict between this brief and
the live code means THE CODE IS THE TRUTH -- record and proceed per section 2's
re-verify discipline.

## 1. Mission [CORE]

Close F-60(a) by eliminating the tick-path ALC root, and make the repository's test
surface actually runnable on the permanent Linux environment so the proof can be
measured where development now lives.

| #  | Deliverable | Action | Version |
| -- | ----------- | ------ | ------- |
| D1 | This brief + both recon reports | enroll (frontmatter + sync) | brief 1.0 Draft -> EXECUTED at closure; reports EXECUTED at enroll |
| D2 | `docs/methodology/BRIEF_TEMPLATE.md` | rider: 3 stale push mentions aligned to 8.4 v3.0.0 (exact text, section 8 below) | body-only, un-governed |
| D3 | Cross-platform native test wiring | test csproj Linux arm (`.so`) + canonical Linux native build convention documented | build wiring |
| D4 | The F-60(a) fix | load-time tick-rate resolution; `_tickRateCache` deleted; scheduler re-plumbed | code |
| D5 | Test truth | pinned leak test FLIPPED to `BeEmpty`; NEW 1-tick minimal pin; 0-tick control unchanged; `TickSchedulerThreadSafetyTests` retired with rationale | tests |
| D6 | Bus-side class eviction | `DomainEventBus.ModeCache` static -> instance; `_handlers` Type-key removal at last unsubscribe | code |
| D7 | Ledger truth | F-60 half (a) CLOSED with proof; new F-rows for the unreachable class members (BusFacade caches, IntentBatcher dead code, `NativeComponentTypeRegistry._byId`); ROADMAP write-back; closure EVT | governance |

Why this precedes LINUX_PRESENT_1: ratified program order 2026-08-25 (F-60(a) next,
LINUX_PRESENT_1 second). D3 additionally unblocks EVERY future cascade's gates on this
box.

## 2. Established facts [CORE]

Facts marked (RV) are re-verified by the executor at Phase 0; mismatch -> H1. Every
anchor is at HEAD `3173d2b0cf3ec3780ab0b5609ec23d00b478c03d` unless stated. Sources:
R# = DOC-E-F60A_TICK_PATH_RECON_REPORT; V/x = DOC-E-F60A_TICK_PATH_VERIFICATION_RECON.

**The root (R3, V/A1-A5, V/consolidated-verdict -- every structural claim confirmed
by two independent instruments):**

1. (RV) `TickScheduler._tickRateCache` -- `ConcurrentDictionary<Type,int>`,
   `src/DualFrontier.Core/Scheduling/TickScheduler.cs:30`; populated ONLY by
   `GetOrAdd(system.GetType(), ...)` in `ShouldRun` (:63-64); cleared ONLY by
   `Reset()` (:73-77), which also resets `_currentTick`.
2. (RV) `Reset()` production callers: **0**. Sole repo-wide caller:
   `tests/DualFrontier.Core.Tests/Scheduling/TickSchedulerThreadSafetyTests.cs:47`,
   which USES it to reopen the concurrent-population race its single test guards (V/A2).
3. (RV) `ShouldRun` production callers: **1** --
   `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:172` (inside
   `Parallel.ForEach`). `_ticks` is `readonly` (:52), assigned once (:103), untouched
   by `Rebuild` (:231-262). Of the scheduler's state, `_tickRateCache` is the ONLY
   Type-keyed container that survives `Rebuild` (V/A3).
4. `SystemAdapter<TSystem>` is generic per wrapped mod system
   (`src/DualFrontier.Application/Modding/SystemAdapter.cs:45`); sole construction
   site `ModRegistry.cs:456-457` (`MakeGenericType` + `Activator.CreateInstance`);
   the cache key for an SDK mod system is the closed generic over a collectible-ALC
   type; a `SystemBase`-authored mod system keys its own ALC-bound type directly
   (V/A4). The design intent is documented at `SystemAdapter.cs:19-22` ("the per-type
   tick-rate cache").
5. The cache buys NOTHING for adapter-wrapped systems: `SystemAdapter<T>.CachedTickRate`
   is already `static readonly` per closed generic (:51-52). Its only surviving value
   is for `SystemBase`-authored systems, whose `TickRateDeclaration`
   (`src/DualFrontier.Core/ECS/SystemBase.cs:79-80`) reflects per call (V/A1 nuance 1).
   The class doc `TickScheduler.cs:15-17` ("memoised... reflection") is drifted;
   `using System.Reflection;` at :3 is unused.
6. Bisection (ledger row F-60, EVT-2026-08-20): 0 ticks -> 3 ms clean; 1 -> 10,459 ms
   + `ModUnloadTimeout`; 16/100/340 -> identical. Matches `GetOrAdd` memoisation
   exactly. The §9.5 step-7 spin: `ModIntegrationPipeline.cs:135-137`
   (10,000 ms / 100 ms / 100 iterations), spin body :1090-1099, warning emission
   :1101-1106, WeakReference capture :967-968 + :834.
7. (RV) Pinned tests, `tests/DualFrontier.Modding.Tests/Weather/WeatherWaveGateTests.cs`:
   leak pin :227-241 (runs `TicksPastFirstTransition = 340` (:36), asserts
   `Contain("ModUnloadTimeout")` at :236, doc :222-225 instructs "Flip it to BeEmpty
   when the tick-path root is closed"); 0-tick control :256-267 (asserts `BeEmpty`
   :264). **No test pins the 1-tick case** (V/discrepancy 10). Harness
   `WeatherHarness.cs`: production-faithful (`Bootstrap.Run(useRegistry: true)` :58,
   own `TickScheduler` :64, `SchedulerTestFixture.BuildIsolated` :78-79,
   `Tick()` -> `Scheduler.ExecuteTick` :93-97).

**The design target (V/B1-B4):**

8. `SystemMetadata` is a 2-member positional record
   (`src/DualFrontier.Core/Scheduling/SystemMetadata.cs:15`,
   `record SystemMetadata(SystemOrigin Origin, string? ModId)`), internal in Core,
   with ONE production construction site:
   `SystemMetadataBuilder.cs:38` -- and the builder already holds the live
   `SystemBase` instance (`reg.Instance`). `InternalsVisibleTo` Core -> Application
   already granted (`DualFrontier.Core.csproj:17`).
9. The metadata table is rebuilt and SWAPPED at exactly the four load boundaries:
   `GameBootstrap.cs:210` (bootstrap), `ModIntegrationPipeline.cs:564` (Apply),
   :817 (UnloadMod step 4), :901 (rollback/UnloadAll) -- a rate carried there is
   evicted at unload FOR FREE.
10. `ModRegistry.cs:253-254` ALREADY reads `[TickRate]` from the raw `Type` at
    load-time validation (missing -> loud `InvalidOperationException`) -- load-time
    resolution requires no new capability, only carrying the value forward.
11. Composition root is `GameBootstrap` (NOT EngineSession): `TickScheduler` at :90,
    `ParallelSystemScheduler` at :212-219; `GameLoop._ticks` (:39, :54) and the
    `SetTickSource` closure (:146) are the other two long-lived referents.
    `CurrentTick` production readers: `GameLoop.cs:132`, `GameBootstrap.cs:146`,
    `ParallelSystemScheduler.cs:198` (V/A1).
12. (RV) `new TickScheduler()` sites: 25 = 1 production (`GameBootstrap.cs:90`) +
    24 tests. Scheduler tests passing EMPTY metadata tables exist
    (`ParallelExecutionTests.cs:32/62/87`, `SchedulerStressTests.cs:234`,
    `SchedulerExtremeTests.cs:283`, `SchedulerFaultDispatchTests.cs:156`,
    `DeferredEventDeliveryTests.cs:98/139`,
    `tests/DualFrontier.Modding.Tests/Fixtures/SchedulerTestFixture.cs:31`) -- the
    scheduler's documented absent-from-table fall-through (Core/null defaults,
    `ParallelSystemScheduler.cs:88`) must extend to the rate (default REALTIME).

**The class, re-classified by the verification recon (V/C1-C6 -- this SUPERSEDES the
primary recon's R4 framing):**

13. `DomainEventBus.ModeCache` (static, `DomainEventBus.cs:25`, sole write :228-229
    reached from `Publish` :96): CONFIRMED unevicted tick-path Type-keyed static.
    ONE `DomainEventBus` per session (`GameServices.cs:23`; zero production
    `new DomainEventBus()` sites; all genre getters -> one `UnifiedGenreBus` :70-79).
    In the F-60(a) repro its key (`WeatherChangedEvent`) lives in the SHARED ALC,
    which is **non-collectible BY INVARIANT** (`SharedModLoadContext.cs:31-34`,
    `isCollectible: false`, doc §1.4) -- so it cannot root the regular ALC, and a
    shared-ALC release assertion is impossible-by-design (V/C5: zero tests combine
    SharedModLoadContext + WeakReference, correctly). It IS a genuine unbounded root
    for any mod declaring event types in its regular (collectible) assembly.
14. `DomainEventBus._handlers` (:27): `Unsubscribe` (:65-83) removes subscriptions,
    NEVER the Type key (no `TryRemove`). Unload step 1 `RestrictedModApi.UnsubscribeAll`
    (`RestrictedModApi.cs:264-269`, per-mod `_subscriptions` list :50, invoked
    `ModIntegrationPipeline.cs:702-705`) removes all of a mod's HANDLERS by owner --
    the Type key + empty list persist. `DomainEventBus.Clear()` has no unload-path
    caller. Eviction model to imitate: `ModRegistry._componentOwners` (:36, per-mod
    Type-key removal at :327-334) -- the one Type-keyed container with correct
    per-mod eviction.
15. `BusFacade._typeIdCache`/`_tierCache` (`BusFacade.cs:41-42`, writes :62/:70-71):
    unevicted in shape but **ZERO production construction sites** at this HEAD
    (9 test-only), dispatch gated by `UseNativeBusForDispatch = false` (:49).
    NOT reachable until the К10.4 sovereign switch -> LEDGER ROW, not code scope.
16. `IntentBatcher` (`IntentBatcher.cs:12`): **DEAD CODE** -- one repo-wide hit, its
    own declaration. -> LEDGER ROW (disposal), not code scope.
17. `NativeComponentTypeRegistry._byId` (owning type is `NativeComponentTypeRegistry`
    at `NativeComponentType.cs:53`, NOT `NativeComponentType<T>` -- V/discrepancy 1):
    write-only (`Lookup` 0 callers), reachable ONLY on the legacy `_registry == null`
    arm (`NativeWorld.cs:661-691`); production and the harness run
    `useRegistry: true`, and the registry arm THROWS for non-Default-ALC types
    (:672-679). -> LEDGER ROW (deletion at the K8 cutover), not code scope.

**Environment (architect-measured 2026-08-25/26 on the permanent Linux box):**

18. Operator ruling 2026-08-26: **Linux (Ubuntu 26.04, kernel 7.0) is the PERMANENT
    development environment.** Toolchain verified: dotnet SDK 10.0.400 (user-local
    `~/.dotnet` -- apt carries only 10.0.111, BELOW the `global.json` 10.0.204 floor;
    never "fix" the SDK via apt), cmake 4.2.3, ninja 1.13.2, gcc 15.2.0,
    glslc 2026.1, Vulkan headers + live discrete GPU (RADV, API 1.4.335).
19. Native kernel FIRST Linux build: CLEAN (61/61 targets, GCC `-std=c++23`,
    `-Wall -Wextra -Wpedantic`, zero diagnostics); `df_native_selftest` **110/110
    ALL PASSED** -- exactly the ID-B closure baseline. The К-L1 MSVC
    `/std:c++23preview` pin is Windows-arm-only; the CMake GNU arm needed zero changes.
20. (RV) ABI proven end-to-end: with `DualFrontier.Core.Native.so` copied into
    `tests/DualFrontier.Modding.Tests/bin/Release/net10.0/`, the 0-tick control test
    PASSED (85 ms) through the real pipeline. `DllImport` name
    `"DualFrontier.Core.Native"` (`NativeMethods.cs:23`) resolves `<name>.so` by
    default .NET probing -- no managed rename needed.
21. (RV) THE WIRING GAP: test csproj native-copy items are Windows-only -- hardcoded
    `..\..\native\DualFrontier.Core.Native\build\Release\DualFrontier.Core.Native.dll`
    under an `Exists` condition (`tests/DualFrontier.Modding.Tests/....csproj:25-28`,
    `tests/DualFrontier.Runtime.Tests/....csproj:28-31`,
    `tests/DualFrontier.Core.Benchmarks/....csproj:40-43`; Phase 0 re-verifies the
    FULL list with the section-10 census). On Linux the condition is false and
    native-dependent suites cannot run without a manual copy.

## 3. Phase 0 -- preconditions and checkpoint [CORE]

Run serially before any edit.

1. **Verify recon facts**: HEAD is `3173d2b` (or a fast-forward whose diff touches
   none of the section-2 anchor files -- otherwise H1); every (RV) fact above,
   re-measured with the same instruments (grep counts included). Mismatch -> H1.
2. **Native build + placement**: `cmake -S native/DualFrontier.Core.Native -B
   native/DualFrontier.Core.Native/build -G Ninja -DCMAKE_BUILD_TYPE=Release &&
   cmake --build native/DualFrontier.Core.Native/build` -- this is the CANONICAL
   Linux native build location this cascade pins (D3 wiring points at it). Verify
   `df_native_selftest` reports 110 ALL PASSED; record as baseline. Verify the build
   directory is git-ignored (`git status --porcelain` shows no `native/` entries);
   if not ignored -> the .gitignore line rides C3.
3. **Baseline gates** (record verbatim as the regression anchor): managed
   `dotnet build DualFrontier.sln -c Release` (expect 0W/0E); full test sweep with
   the .so manually copied into each native-dependent suite's output dir (the C3
   wiring replaces this workaround; the manual copy is sanctioned ONLY for this
   baseline). Expected shape: ID-B closure baseline 1301 passed / 0 failed / 5
   skipped, PLUS the known EXPECTED-DEFECT leak pin passing in its pre-flip form,
   with the ~10.5 s spins x5 in the Weather suite (do not mistake the spins for a
   hang; never pipe `dotnet test` -- `CODING_STANDARDS`/`TESTING_STRATEGY` §8
   no-pipe law, file-redirect only). Any other failure -> H2.
4. **Validation checkpoint**:
   `dotnet run --project tools/DualFrontier.Governance -- validate --armed` exit 0
   -> else H3.
5. **Frontmatter-shape read** (Lesson #N14): FRAMEWORK 14.3/14.4 + the live
   frontmatter of one LOCKED doc + one existing `AUDIT_TRAIL.yaml` EVT as the
   verbatim append template. The two recon reports carry ready frontmatter -- verify
   their `register_id`s are absent from the register before C1 (collision -> H5).
6. **Mandatory reads**: both recon reports (FULL), `TickScheduler.cs`,
   `ParallelSystemScheduler.cs`, `SystemMetadata.cs`, `SystemMetadataBuilder.cs`,
   `SystemAdapter.cs`, `WeatherWaveGateTests.cs`, `WeatherHarness.cs`,
   `TickSchedulerThreadSafetyTests.cs`, `DomainEventBus.cs`, `GameServices.cs`,
   `RestrictedModApi.cs` (UnsubscribeAll), `ModIntegrationPipeline.cs` unload chain
   (:642-829, :1084-1107), the three named test csproj files, `METHODOLOGY.md`
   closure protocol, `CODING_STANDARDS.md` 8.4.

`sync` runs in EVERY frontmatter-touching commit; derived registers never
hand-edited; `AUDIT_TRAIL.yaml` append-only. At the closure boundary the executor
pushes the WORK BRANCH and opens a PR against `main`; pushing `main` and merging its
own PR are forbidden (`CODING_STANDARDS.md` 8.4, v3.0.0).

## 4. Topology [CORE]

Single orchestrator, serial, no wave. Rationale: the blast radius is one scheduler
seam + one bus seam + test wiring; both recons already did the survey work twice; a
wave would add coordination cost with no coverage gain.

## 5. Wave R -- survey agents

None -- the survey is DONE twice pre-brief (the two enrolled recon reports, same HEAD,
independent instruments). Phase 0's (RV) re-verification is the freshness check.

## 6. Checkpoints [CORE]

- **C-fix (after C4, before C5):** the falsification checkpoint. Run the Weather gate
  suite. REQUIRED outcome: the flipped leak pin passes (`BeEmpty`), the NEW 1-tick pin
  passes, the 0-tick control passes, and the previously-spinning unload tests complete
  WITHOUT the 10 s step-7 spin (wall-clock of the suite drops by roughly 5 x 10 s).
  The pinned test not flipping -> H8 (STOP -- the root is elsewhere; do not widen).
- **C-audit (before C6):** truth-law self-audit -- every enforcement claim in touched
  docs/comments names its on-disk enforcer; no roadmap load outside ROADMAP pointers;
  citation-form compliance; the retired test's rationale states the structural reason,
  not "obsolete".

## 7. Execution / writer specifications [CORE]

Global laws by reference (section 0 notice). Intended forms below are mutable surface
under `RESERVED_SURFACE_MUTABILITY` -- deviations recorded as `Skeleton revisions`,
architectural forks halt instead (H6).

**7.1 D3 -- cross-platform native test wiring (C3).** In each native-dependent test
csproj (the Phase 0 census list; at minimum Modding.Tests, Runtime.Tests,
Core.Benchmarks): keep the existing Windows item byte-unchanged; ADD a sibling item
for `$(MSBuildThisFileDirectory)..\..\native\DualFrontier.Core.Native\build\DualFrontier.Core.Native.so`
(same `Exists` conditioning, `CopyToOutputDirectory=PreserveNewest`,
`Link=DualFrontier.Core.Native.so`). Note the Linux path has NO `Release`
subdirectory -- Ninja is single-config (section 2 fact 19's build lands the .so at
the build root; Phase 0 step 2 verifies). MSBuild `Exists` on a relative path with
backslashes normalises on Linux -- verify by building one suite and confirming the
.so lands in output; if `Exists` misbehaves, the sanctioned alternative is an
OS-conditioned item (`Condition="'$(OS)' != 'Windows_NT' AND Exists(...)"`), recorded
as a Skeleton revision. Document the canonical Linux native build commands (Phase 0
step 2, verbatim) in `docs/methodology/DEVELOPMENT_HYGIENE.md` next to the existing
build-command law. Add the `native/**/build*/` ignore line if Phase 0 step 2 found it
missing.

**7.2 D4 -- the fix (C4).** Intended form:

- `SystemMetadata` (Core) gains a third positional member: `int TicksPerUpdate`.
  `SystemMetadataBuilder.Build` computes it from `reg.Instance.TickRateDeclaration`
  (null or non-positive -> `TickRates.REALTIME` -- exactly today's
  `ResolveTicksPerUpdate` semantics, `TickScheduler.cs:79-87`).
- `ParallelSystemScheduler.ExecutePhase`: ONE `_systemMetadata.TryGetValue` per
  system per tick serves BOTH the quarantine consult and the rate (fold
  `IsQuarantined`'s separate lookup); absent-from-table -> Core/null/REALTIME
  fall-through (preserves the documented empty-table test path, section 2 fact 12).
  The rate check `_ticks.ShouldRun(rate)`.
- `TickScheduler`: DELETE `_tickRateCache`, `ResolveTicksPerUpdate`, the
  `ShouldRun(SystemBase)` overload, `Reset()` (its only caller retires with it --
  7.3), and the unused `using System.Reflection;`. Keep: `_currentTick`,
  `CurrentTick`, `Advance()`, and a pure `ShouldRun(int ticksPerUpdate) =>
  _currentTick % ticksPerUpdate == 0`. Re-ground the class doc (counter + cadence
  arithmetic; no memoisation claim).
- `SystemAdapter.cs:19-22` comment: drop the "per-type tick-rate cache" clause; the
  generic-per-type rationale that SURVIVES is `DependencyGraph` duplicate detection --
  say only what remains true.
- `SystemBase.TickRateDeclaration` stays as-is (still the declaration hook the
  builder reads; its per-call reflection now runs once per system per REBUILD, off
  the tick path entirely).

Per-behaviour test obligation: builder computes REALTIME for null/zero/negative;
adapter-wrapped rate forwarding still honoured (NORMAL from the Weather systems);
empty-metadata fall-through ticks every tick.

**7.3 D5 -- test truth (C4, same commit as 7.2 -- the flip IS the fix's proof).**

- `Unload_LeaksTheModAlc_RootIsOnTheTickPath_NotTheTypeRegistry`: flip :236 to
  `warnings.Should().BeEmpty(...)`, rename to
  `Unload_AfterTicking_ReleasesTheModAlc_TickPathHoldsNoTypeKey` (or equivalent
  truthful name -- intended form), rewrite its doc block: the bisection table STAYS
  (historical measurement), the EXPECTED-DEFECT paragraph is replaced by the
  root-cause record (cache, fix, this cascade).
- NEW minimal pin `Unload_AfterSingleTick_ReleasesTheModAlc_Immediately`:
  `h.Tick(1)` then `UnloadMod(RegularId)` -> `warnings.Should().BeEmpty()` -- pins
  the exact bisection datum no test pinned before (V/discrepancy 10).
- 0-tick control: byte-unchanged.
- `TickSchedulerThreadSafetyTests.cs`: RETIRE (delete) with a commit-body rationale:
  the concurrent-population race it guards is structurally impossible once no lazy
  population exists; the metadata table is written only at load boundaries and read
  during ticks. (Its 32-type synthetic pool goes with it.)

**7.4 D6 -- bus-side class eviction (C5).** Intended form:

- `DomainEventBus.ModeCache`: static -> instance field `_modeCache` (one bus per
  session is a measured fact -- section 2 fact 13; the static gains nothing and
  outlives sessions). `GetDeliveryMode` becomes instance.
- `DomainEventBus.Unsubscribe`: when the list empties under its existing lock,
  `_handlers.TryRemove(eventType, out _)` -- guarded against the Subscribe race by
  the established lock-the-list discipline: re-check emptiness under the list lock,
  and tolerate the benign race where a concurrent `GetOrAdd` resurrects the key
  (state the invariant in the doc: no Type key outlives its last subscriber PLUS
  quiescence, which unload's step-1 sweep guarantees).
- `_modeCache` eviction at unload is NOT wired here: mode entries are keyed by event
  types a mod PUBLISHED, for which the bus holds no owner index. The honest
  contract, stated in the class doc: the mode cache is bounded by subscribed/
  published event-type count and holds only non-collectible-ALC types in the
  supported topology (shared-kind contracts assemblies, §1.4); a regular-ALC event
  type reaching Publish is the F-row seeded at D7 (analyzer-candidate). Do NOT
  build an owner-indexed mode cache -- that is К10.4-adjacent design, out of scope.
- Per-behaviour tests: subscribe/unsubscribe/resubscribe roundtrip leaves no key;
  unload of the Weather pair leaves `_handlers` without `WeatherChangedEvent` only
  if its subscriber count reached zero (assert via existing test seams -- no new
  reflection probes into private state; if no seam exists, assert behaviourally:
  re-subscribe works and delivery still functions post-unload).

**7.5 D2 -- template rider (C2).** Exact replacements in
`docs/methodology/BRIEF_TEMPLATE.md` (body-only, un-governed -- plain doc edit):

- Line 258: `> self-attestation (no pushes; ...` ->
  `> self-attestation (work branch pushed + PR opened, main untouched, no self-merge`
  `> (CODING_STANDARDS 8.4 v3.0.0); sync run in every frontmatter-touching commit;`
- Line 261: `> (push; the standing F-queue items that remain operator-owned).` ->
  `> (merge the PR; the standing F-queue items that remain operator-owned).`
- Line 266: `> it -- adjacent cascades, architect-owned findings, the reference tree, pushes,` ->
  `> it -- adjacent cascades, architect-owned findings, the reference tree, pushes to main,`

(Line numbers verified at this HEAD -- V/D1; re-verify before edit, content-match
governs over line numbers.)

**7.6 D7 -- ledger truth (C6, closure).** F-ledger writes (exact rows drafted by the
executor per the ledger's live row form, content mandated here):

- F-60: half (a) CLOSED. Root: `TickScheduler._tickRateCache` (proof: the flipped
  pin + the 1-tick pin at the fix commit hash; the falsification protocol: one
  variable varied -- the cache eliminated -- and the leak died while the 0-tick
  control held). The row records that BOTH recon instruments predicted the root and
  the fix confirmed empirically (bisect-not-infer honoured).
- NEW F-row: `BusFacade._typeIdCache`/`_tierCache` unevictable Type-keyed caches --
  unreachable at this HEAD (zero production constructions, dispatch fenced) -- MUST
  gain eviction-or-weak-keys before the К10.4 sovereign switch; reference F-57's
  precondition set.
- NEW F-row: `IntentBatcher` dead code (one repo-wide reference: its declaration) --
  disposal candidate, deletion needs its own commit with build-green-as-inertness
  proof.
- NEW F-row: `NativeComponentTypeRegistry._byId` write-only static (Lookup: 0
  callers), legacy `useRegistry:false` arm only -- deletion candidate at the K8
  legacy-path cutover the `[Obsolete]` message names.
- NEW F-row (analyzer-candidate, roadmap): "no strong Type-keyed container populated
  from the tick path may survive mod unload" -- DFK-rule candidate; until it exists
  the flipped tests are the empirical lock (truth law: no enforcement verb -- record
  as Planned with ROADMAP pointer).
- ROADMAP: identity family fully closed; forward queue per the ratified program
  order (LINUX_PRESENT_1 next).

## 8. Kind-specific machinery [KIND: phase-execution]

Per-behaviour test obligation as specified in 7.2-7.4. No multi-agent machinery.

## 9. S-LOCK invariants [CORE]

No new structural S-LOCK this cascade: the durable invariant ("tick path holds no
Type key that outlives unload") is EMPIRICALLY locked by the flipped pin + 1-tick pin
+ 0-tick control triple, and its structural (analyzer) form is seeded as an F-row
(7.6) -- claiming a structural lock without the analyzer would violate the truth law.

## 10. Census discipline [CORE]

- HARD pins, must be EXACT and unchanged at closure: DFK-WAIVER count **3**
  (`rg -n "DFK-WAIVER" src/ --count-matches` per TESTING_STRATEGY canonical form);
  native exports **209**; `native/` tree diff **0** (H7 census:
  `git status --porcelain native/` empty AND `git diff --stat HEAD -- native/` empty
  at every commit -- the .gitignore line, if C3 adds it, is repo-root or
  `.gitignore`-file scope, not a `native/` source change... place the ignore line in
  the ROOT `.gitignore` so the native tree stays byte-untouched).
- Windows-only native-copy csproj census (C3 work order):
  `rg -ln "Core.Native.*\.dll" --glob "tests/**/*.csproj"` -- the C3 commit touches
  exactly this set; count recorded before/after (each file gains one `.so` item).
- SOFT pins: none expected to move. A comment-only move (7.2's SystemAdapter
  comment) is not census-bearing.

## 11. Commit plan [CORE]

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(enroll): F60A_TICK_PATH brief + recon pair enrolled` | brief + both recon reports frontmatter-enrolled + sync + validate --armed |
| C2 | `docs(methodology): closure-guidance push mentions aligned to 8.4 v3.0.0` | the three exact replacements (7.5) |
| C3 | `build(tests): native library test wiring gains the Linux arm` | csproj .so items + DEVELOPMENT_HYGIENE Linux build commands + root .gitignore line if Phase 0 found it missing |
| C4 | `fix(scheduling): tick rate resolves at load; the lazy Type-keyed cache dies (F-60a)` | 7.2 + 7.3 in one atomic commit -- the flip is the proof of the fix |
| C5 | `fix(bus): mode cache to instance; handler Type keys die with their last subscriber` | 7.4 |
| C6 | `governance(closure): F60A_TICK_PATH EVT + ROADMAP write-back -- F-60 CLOSED, identity family complete` | AUDIT_TRAIL append + F-rows (7.6) + brief -> EXECUTED + sync + validate --armed |

Commit count is intended-form; a needed split is recorded in the closure report,
history never compressed to match the table.

## 12. REGISTER cascade [CORE]

Schema-2.0 discipline with ONLY the Phase 0 verbatim shapes. C1 enrolls three
documents (this brief D/3/Draft; the two DOC-E/3/EXECUTED reports -- their
frontmatter is already written; the executor verifies shape-conformance against the
Phase 0 exemplar, corrects only on H5-vocabulary grounds). C6 flips this brief's
`lifecycle` to EXECUTED (+ `last_modified`), appends the closure EVT to
`AUDIT_TRAIL.yaml` with the REAL hashes of C1-C5, and re-syncs. `validate --armed`
exit 0 at C1 and C6 (and any other frontmatter-touching commit). Enum needed that
the vocabulary lacks -> H5, never invent.

## 13. Halt conditions (H-series) [CORE]

- **H1** Phase 0 precondition/re-verify mismatch.
- **H2** build/test regression vs the Phase 0 baseline (native selftest included).
- **H3** `validate --armed` nonzero.
- **H4** (unused -- no wave; kept numbered for cross-brief consistency).
- **H5** REGISTER vocabulary gap -- escalate, never invent.
- **H6** an intended form proves architecturally wrong (not mere surface) -- e.g.
  the metadata-carried rate cannot serve a documented scheduler contract.
- **H7** ANY `native/` tree modification (this cascade is MANAGED-ONLY; the
  Phase 0 native BUILD is out-of-tree by the canonical commands).
- **H8** falsification failure: after C4 the leak pin does NOT flip (or the 1-tick
  pin fails) -- STOP, report verbatim, do not widen the fix. The next search front
  is recorded in DOC-E-F60A_TICK_PATH_VERIFICATION_RECON's residual-class note
  (statics on non-collectible-ALC generics closed over mod types); that search is
  ARCHITECT work, not executor improvisation.
- Standing rails: push law 8.4 v3.0.0 (branch + PR at closure; never `main`; no
  self-merge; atomicity settled before push); derived registers never hand-edited;
  `AUDIT_TRAIL.yaml` append-only; no history rewrite/force-push/squash;
  single-writer files honored; `historical/` read-only; never pipe `dotnet test`.

On halt: stop, report state verbatim, await the operator.

## 14. Closure protocol and report [CORE]

Execute the `METHODOLOGY` closure protocol: (a) ROADMAP write-back (7.6); (b)
frontmatter mutations + sync folded per commit; (c) closure EVT appended (single
append, prior entries byte-unchanged); (d) F-ledger rows (7.6) -- never chat-only;
(e) the closure report (chat) carrying: commits table (hash | subject); versions
table; census pins (HARD exact: DFK-WAIVER 3, exports 209, native diff 0; the csproj
census before/after); gates table -- baseline vs closure, must match-or-better, and
MUST include: full sln build 0W/0E, full test sweep counts (expected: baseline
passed-count MINUS the retired thread-safety test PLUS the new pins, 0 failed),
native selftest 110, `validate --armed` exit 0, and the Weather-suite wall-clock
delta (the five 10 s spins gone -- the leak's death measured twice, by assertion and
by clock); F-ledger final-state table; consolidated `Skeleton revisions`;
self-attestation (work branch pushed + PR opened per 8.4 v3.0.0, `main` untouched,
no self-merge; sync in every frontmatter-touching commit; single EVT append; no
history rewrites; native/ + `historical/` + reference trees untouched); operator
checklist (merge the PR; operator-owned F-queue: LINUX_PRESENT_1 next per the
ratified order).

## 15. Out of scope [CORE]

LINUX_PRESENT_1 (windowing/presentation -- next cascade); К10.4 sovereign-switch
preconditions incl. `BusFacade` eviction (F-row only); `IntentBatcher` deletion
(F-row only); `NativeComponentTypeRegistry._byId` deletion (F-row only); F-61
concurrency design; F-58 reclaim-at-unload (W7+); F-57 native-bus preconditions;
any `native/` source change; the DFK analyzer rule for tick-path Type retention
(F-row/ROADMAP only); the live Launcher smoke (owed at PR #51 -- pays after
LINUX_PRESENT_1); pushes to `main` and merging (operator's act).

---

**End of F60A_TICK_PATH_BRIEF.md v1.0**
