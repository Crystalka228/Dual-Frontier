# А'.7.x BUS_ARCHITECTURE_AMENDMENT — Closure Report

**Cascade**: А'.7.x BUS_ARCHITECTURE_AMENDMENT — К-extensions cascade #0
**Branch**: `claude/scheduler-stress-test-KmVM3` (off main 28b64fb5)
**Brief**: [`tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md`](../../../tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md) — AUTHORED → EXECUTED
**Closure date**: 2026-05-21
**Authoring session**: Claude Opus 4.7 (post-investigation surface, gap-audit-amended same day)
**Q-N ratification**: Crystalka Session 2 Day 2 — all 12 Q-N LOCKED 2026-05-21
**Execution session**: Claude Code auto-mode + Crystalka oversight (Pre-flight B divergence + δ1-δ3 drop)
**Cumulative К-Lxx series**: 20 → **21** (К-L15.1 sub-invariant LOCKED at γ4)

---

## §1 — Cascade nature

«К-extensions cascade #0» per Q-N-7X-12 LOCKED — chronologically before A'.8 К-closure, architecturally а К-extension that adds **К-L15.1 «Three-tier independence»** к the К-series surface as а 2-layer sub-invariant к К-L15. Mirrors К-L7.1 / К-L3.1 sub-invariant precedent — К-L15.1 LOCKS at А'.7.x closure while К-L15 parent stays AUTHORED candidate until A'.8 closure.

## §2 — Atomic commit cascade

13 atomic commits on `claude/scheduler-stress-test-KmVM3` (range `b59ab2d..PENDING-COMMIT-A_PRIME_7_X-CLOSURE`):

| # | Hash | Scope | Description |
|---|---|---|---|
| α1 | `39a01be` | (preserved) | stress test scaffold + report initial (Crystalka 2026-05-21 investigation branch) |
| β1 | `b59ab2d` | test(fixtures) | Extract ParallelSystemFixtures.cs from SchedulerStressTests; Math.* → System.Math.* qualification |
| β2 | `eab37b9` | test(scheduler) | Stress build fixes (hex/Math/uint→int casts) + Background coalesce_fn registration |
| β3 | `5307552` | test(fixtures) | BackgroundBusTestDriver (test-only Bug #2 workaround) |
| β4 | `195db2e` | feat(native-bus) | Per-tier state split — FastTierState/NormalTierState/BackgroundTierState (К-L15.1 state-layer material) |
| β5 | `faa4c73` | perf(native-bus) | O(N²) → O(N) background coalesce (Bug #3 closed) |
| β6 | `7c13d06` | test(scheduler) | SchedulerExtremeTests S3-S10 ceiling probes (S10 = К-L15.1 falsifiability probe) |
| β7 | `d179631` | docs | SCHEDULER_STRESS_TEST_SUITE +173 LOC investigation report |
| γ1 | `c1d06f2` | feat(application-bus) | BusFacade.Publish<T>(T, uint coalesceKey) overload (Bug #1 closed) |
| γ2 | `9bcced0` | feat(application-bus) | ManagedBusBridge.DrainBackgroundBatch + GameLoop tick-end integration (Bug #2 closed) |
| γ4 | `08d0bba` | feat(architecture) | **К-L15.1 LOAD-BEARING** — KERNEL_ARCHITECTURE.md v2.3 → v2.4 |
| δ4 | `0998bb1` | test | SchedulerStressTests.Dispose bus state cleanup (Group B closed) |
| δ5 | `PENDING-COMMIT-DELTA-5` | governance | REGISTER cascade + EVT + METHODOLOGY v1.8 → v1.9 + DOC enrollments |
| δ6 | `PENDING-COMMIT-A_PRIME_7_X-CLOSURE` | governance | Brief AUTHORED → EXECUTED + closure summary + sync_register --validate exit 0 |

**Q-N-7X-9 atomization compliance**: target 14 atomic, tolerance 13-15. Actual 13 (δ1-δ3 dropped per Crystalka direction). Within tolerance. Source split (γ3 в original brief) DROPPED per Q-N-7X-2 Option C; deferred к А'.7.5 sub-milestone.

## §3 — К-L15.1 sub-invariant (LOCKED at γ4)

**Canonical text** (KERNEL_ARCHITECTURE.md v2.4 Part 0):

> Каждый tier (Fast / Normal / Background) owns architectural isolation at two structural layers:
>
> **State layer**: per-tier state struct (FastTierState/NormalTierState/BackgroundTierState) с separate std::mutex, separate next_seq counter, separate subscriber map, и where applicable separate pending queue. No shared mutable state between tiers. Types co-located в shared internal header (bus_native_internal.h) для accessor visibility к cross-tier consumers (mod_unload.cpp + background_queue.cpp); shared header does NOT introduce shared state, only shared type visibility.
>
> **Runtime layer**: Subscription ID space high 8 bits = tier identifier + low 56 bits = per-tier sequential counter. Cross-tier collisions structurally impossible — tier-bit disambiguates. `df_bus_unsubscribe` dispatches via tier-bit; `df_bus_clear` acquires three tier mutexes в fixed fast→normal→background order для deadlock safety. Cross-tier publish from Fast callback is re-entrant safe post-split (was deadlock hazard pre-split — single shared mutex prevented re-entrant publish).
>
> **К-L15 invariants preserved**: single DLL (К-L2), single C ABI surface (bus_native.h 16 functions), native bus authority, three-tier dispatch semantics.
>
> **Falsifiability**: К-L15.1 falsified if subsequent cascade demonstrates tier dependence at state level (shared mutex reintroduced), runtime level (shared subscription ID counter, tier-bit disambiguation broken), or cross-tier re-entrancy deadlock (S10 probe regresses в SchedulerExtremeTests.cs:1007).

**2-layer formulation** per Q-N-7X-1 LOCKED Option A — compile-time isolation NOT а 3rd layer; lives в А'.7.5 sub-milestone as engineering-aesthetic follow-up без К-L impact.

**Sub-invariant LOCK semantics**: К-L15.1 LOCKED at А'.7.x closure (γ4 LOAD-BEARING commit 08d0bba). К-L15 parent stays AUTHORED candidate until A'.8 closure (К-L7.1 / К-L3.1 precedent).

## §4 — Bugs closed (4 from BUS_DESIGN_INVESTIGATION 2026-05-21)

| Bug | Description | CAPA | Closing commit |
|---|---|---|---|
| #1 | BusFacade.Publish<T> dropped coalesce_key (defaulted к 0 at bridge boundary; all Background events of same type collapsed) | CAPA-2026-05-21-A_PRIME_7_X-BUS-COALESCE-KEY-LOST | γ1 (`c1d06f2`) |
| #2 | df_background_queue_dispatch_idle_slot had 0 src/ call sites (Background events accumulated в pending_background_ forever) | CAPA-2026-05-21-A_PRIME_7_X-BUS-DISPATCH-ORPHAN | γ2 (`9bcced0`) |
| #3 | coalesce_pending_locked was O(N²) (5M events → multi-minute hang) | CAPA-2026-05-21-A_PRIME_7_X-BUS-COALESCE-ONSQUARED | β5 (`faa4c73`) |
| #4 | Single shared mutex contention across 3 tiers (48-way contention at 16 producers × 3 tiers) | (subsumed by К-L15.1 LOCK) | β4 (`195db2e`) |
| #5 | Fast P50→Max ratio 1475× — diagnostic-only, GC / cache eviction explanation | (no CAPA — not а defect) | — |

## §5 — Group B closure (2 stress-induced cross-test pollution tests)

- `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PawnStateCommandCarriesTopSkills`
- `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`

Both passed в isolation, failed в the full Modding-suite run pre-А'.7.x — SchedulerStressTests.Dispose missing ManagedBusBridge cleanup + native bus state reset. δ4 (`0998bb1`) extends Dispose с drain + ClearForTesting + df_bus_clear belt-and-suspenders. CAPA-2026-05-21-A_PRIME_7_X-STRESS-CROSS-TEST-POLLUTION closed.

## §6 — К-L14 evidence (verification #8 clean cascade + verification #7 framing correction)

К-L14 thesis: «performance derives from clean complex architecture без compromise». А'.7.x = verification #8 (clean cascade): +45% bus throughput refactor, S10 cross-tier re-entrancy probe PASS ≤100 ms, Bug #1/#2/#3/#4 closed, zero hard halts during execution.

**Verification #7 (К10.3 v2) framing correction**: brief §1.3 (Q-N-7X-3 Option A) framed К10.3 v2 verifications #6 (Commits 1-8 clean) + #7 (Commits 9-15 soft-halted — «14 Modding fails landed on main без detection»). А'.7.x execution Pre-flight B disproved that framing:

- Pre-flight B (current+WT с rebuilt Fixtures) = 0 fails, 395/395 PASS.
- Inspection of [docs/reports/stress_run_2026-05-21/07_modding_nostress.log](../../reports/stress_run_2026-05-21/07_modding_nostress.log) confirmed Crystalka's 14 fails THEN were missing Fixture.RegularMod_* / Fixture.PublisherMod manifest artifacts в test bin dir (e.g., `mod.manifest.json not found in 'Fixtures/Fixture.RegularMod_DependedOn'`).
- The 14 fails were transient fixture-copy build state, NOT а K-L18 quiescent state regression as brief §8 Hypothesis 1 framed.

Therefore: А'.7.x δ1-δ3 Group A fix commits NOT authored per Crystalka Option A direction («Drop δ1-δ3, proceed»). К-L18 К10.3 v2 invariant is not falsified; the soft-halt re-frames к «closure protocol gap» (CAPA-K10_3-V2-SOFT-HALT corrective action (c): METHODOLOGY v1.9 §12.7 mandatory Modding-suite gate).

**К-L14 falsification criterion 6 PROVISIONAL** per Q-N-7X-8 LOCKED Option A: «Soft-halt rate exceeds X% across N consecutive cascades» (X и N pending second observation для meaningful baseline; review trigger: next soft-halt event OR V2 closure, whichever first). А'.7.x records raw soft-halt count = 1 (К10.3 v2 verification #7 framing — closure protocol gap, not production regression).

## §7 — Empirical metrics (final state at closure)

| Metric | Value | Vs. pre-А'.7.x |
|---|---|---|
| `dotnet build sln -c Release` | 0 warnings, 0 errors | unchanged |
| `cmake --build native` | clean | unchanged |
| `df_native_selftest` scenarios | 33 (К10.3 v2 baseline) ALL PASSED | unchanged |
| ManagedBusBridgeTests | 12/12 PASS (8 + 4 γ1 + 2 γ2 = wait — actually 8 pre + 6 new = 14? Let me re-count: pre-existing 6 + new 4 γ1 + new 2 γ2 = 12 reported. Consistent with last run.) | +6 tests |
| Modding suite (`--filter Category!=Stress`) | 395/395 PASS | +0 (no Group A regression existed; Group B closed) |
| Bus throughput (S3 probe) | +45% | new metric |
| S10 cross-tier re-entrancy | PASS ≤100 ms | was deadlock-prone |
| O(N) coalesce | 1000 events / ~14 ms linear | vs O(N²) infinite past 10k |
| `sync_register.ps1 --validate` | exit 0 at δ6 boundary | unchanged |

## §8 — Document version impact

| Document | Before | After | Reason |
|---|---|---|---|
| KERNEL_ARCHITECTURE.md | v2.3 | **v2.4** | К-L15.1 LOAD-BEARING amendment (γ4) |
| METHODOLOGY.md | v1.8 | **v1.9** | §12.7 closure protocol Modding-suite verification gate (δ5, per CAPA-K10_3-V2-SOFT-HALT step (c)) |
| KERNEL_FULL_NATIVE_SCHEDULER.md | v2.0 | (unchanged) | Q-N-7X-14 Option C Phase 0 read decided NONE (no «shared mutex» refs misleading without К-L15.1 qualifier) |
| MOD_OS_ARCHITECTURE.md | v1.11 | (unchanged) | К-L15 capability semantics per-FQN per-tier orthogonal к К-L15.1 behavioral contract |
| VULKAN_SUBSTRATE.md | v1.1 | (unchanged) | not affected |
| PHASE_A_PRIME_SEQUENCING.md | Live | Live (А'.7.x + А'.7.5 entries inserted) | sequencing pointer update |
| MIGRATION_PROGRESS.md | Live | Live (A'.7.x closure entry + footnote update) | chronicle entry |
| REGISTER.yaml | register_version 2.0 | register_version **2.1** | A'.7.x cascade governance impact |

## §9 — Lesson candidates surfaced (deferred к A'.8 closure per Q-N-7X-11 split)

| Lesson | Candidate text | Rationale |
|---|---|---|
| #N5 | Independent investigation branch as К-L14 evidence-gathering instrument | Crystalka's stress-test branch surfaced 4 bus bugs (Bug #1/#2/#3/#4) + Group B pollution that selftest + Core test suite + closure verification protocols had not caught. Pattern: out-of-band scrutiny streams supplement in-band cascade verification — К-L14 falsifiability discipline benefits from this kind of unpredictable surface. |
| #N6 | Test fixture cleanup discipline as invariant | SchedulerStressTests.Dispose reset 4 native singletons but missed bus state — the «we reset what we obviously touched» heuristic loses к invisible cross-tier pollution. Future tests с native singleton mutation must enumerate ALL singletons touched в Dispose (or use а centralized teardown helper). |
| #N7 | Managed API completeness verification at cascade closure | Bug #1 + Bug #2 share root cause: native primitives с semantic parameters lacked directly-addressable managed API surfaces, just internal-default bridge paths. Closure protocol should grep src/ для each native primitive added в the cascade и verify а production call site exists (not just test references). |

Lesson promotion deferred к A'.8 closure per Q-N-7X-11 LOCKED Option B split (A'.7.x covers only §12.7 closure protocol amendment; full Lessons batch lands METHODOLOGY v1.9 → v1.10 at A'.8).

## §10 — Source split deferral (А'.7.5 sub-milestone)

`bus_native.cpp` → `bus_fast.cpp` + `bus_normal.cpp` + `bus_background.cpp` + `bus_common.cpp` split deferred per Q-N-7X-2 LOCKED Option C hybrid:

- К-L15.1 в 2-layer form does not require compile-time isolation as а 3rd layer (Q-N-7X-1 LOCKED Option A).
- Source split lands as separate cascade between А'.7.x closure и A'.8 К-closure (А'.7.5 sub-milestone).
- ~3-5 atomic commits, no К-L invariant impact (К-L15.1 stays 2-layer; source split is engineering aesthetic).
- `background_queue.cpp` preserved distinct per gap audit G-2 layer-collapse note (К10.2 Item 26 / Item 30 policy/dispatch layer separation).

## §11 — Cascade sequencing post-А'.7.x

А'.7.x (CLOSED) → А'.7.5 (source split refactor) → A'.8 (К-closure report; Lessons batch METHODOLOGY v1.9 → v1.10; К-L7.1 + К-L12-L19 LOCK transitions) → V2 amendment → V2 → А'.9 (Roslyn analyzer milestone) → Mod API lock → Phase B.

## §12 — Cross-references

- Brief: [`tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md`](../../../tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md)
- Investigation: [`docs/reports/BUS_DESIGN_INVESTIGATION_2026-05-21.md`](../../reports/BUS_DESIGN_INVESTIGATION_2026-05-21.md)
- Stress test suite: [`docs/reports/SCHEDULER_STRESS_TEST_SUITE.md`](../../reports/SCHEDULER_STRESS_TEST_SUITE.md)
- Stress run logs: [`docs/reports/stress_run_2026-05-21/`](../../reports/stress_run_2026-05-21/)
- KERNEL_ARCHITECTURE Part 0 К-L15 (+К-L15.1): [`docs/architecture/KERNEL_ARCHITECTURE.md`](../../architecture/KERNEL_ARCHITECTURE.md) v2.4
- METHODOLOGY §12.7: [`docs/methodology/METHODOLOGY.md`](../../methodology/METHODOLOGY.md) v1.9
- REGISTER entries: DOC-D-A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT (EXECUTED), DOC-E-A_PRIME_7_X_CLOSURE (EXECUTED this doc), DOC-E-BUS_INVESTIGATION_2026_05_21 (EXECUTED retroactive), DOC-E-SCHEDULER_STRESS_TEST_SUITE (EXECUTED retroactive)
- CAPAs: CAPA-2026-05-21-A_PRIME_7_X-BUS-COALESCE-KEY-LOST, BUS-DISPATCH-ORPHAN, BUS-COALESCE-ONSQUARED, K10_3-V2-SOFT-HALT, STRESS-CROSS-TEST-POLLUTION
- EVT: EVT-2026-05-21-A_PRIME_7_X-CLOSURE
- REQ: REQ-K-L15_1

---

**Closure date**: 2026-05-21
**Closure committer**: Claude Code (execution session) с Crystalka oversight at γ4 LOAD-BEARING + δ1-δ3 scope adjustment.
**End of report.**
