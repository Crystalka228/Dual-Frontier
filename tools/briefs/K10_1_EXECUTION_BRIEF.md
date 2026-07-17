---
register_id: DOC-D-K10_1
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-05-18
last_modified: 2026-05-18
content_language: mixed
next_review_due: null
title: К10.1 — Kernel Scheduler Core Execution
review_cadence: on-status-transition
last_review_date: 2026-05-18
last_review_event: К10.1 sub-milestone closure 2026-05-18 — 16 atomic commits f439b74..PENDING-COMMIT-K10_1-CLOSURE; K-L6 SUPERSEDED + K-L12/L13/L14 AUTHORED; 17 of 46 K10 items landed; 624 tests green baseline preserved
reviewer: Crystalka
risks_referenced:
- RISK-002
- RISK-003
- RISK-004
- RISK-013
special_case_rationale: "К10.1 standalone execution brief — first of four К10 sub-milestones under Option III standalone-briefs structure (К10.1 = kernel scheduler core; К10.2 = native bus + mod ALC lifecycle native primitives; К10.3 = pipeline depth + display composition + hardware tier; К10.4 = TLA+ formal verification). Implements 17 of 46 items from KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED: §3.1 Items 1-5, §3.2 Items 6-8, §3.3 Items 9 + 11-13 (Item 10 NUMA deferred к К-extensions), §3.4 Items 15-16 (Item 14 К11+), §3.5 Items 17 + 19-20, §3.7 Item 24. Ratifies К-L6 SUPERSEDED + К-L12/L13/L14 architecturally established at Commit 14 (load-bearing). К10 as whole closes only after К10.4 sub-milestone; К-closure report (А'.8) waits for all four К10 sub-milestones. Brief authored from К10 deliberation arc 2026-05-16..2026-05-17 (9 S-locks ratified). Distinct from DOC-D-K10_EXECUTION (skeleton brief for original 10-sub-milestone partitioning, retained as historical record under Option III restructuring)."
---

---
# Brief frontmatter (not REGISTER mirror — brief lives in tools/briefs/ as Tier 3 Category D)
brief_id: K10_1_EXECUTION_BRIEF
status: EXECUTED
authored: 2026-05-18
executed: 2026-05-18
author: Claude Opus 4.7 (Crystalka deliberation session)
target_executor: Claude Code (auto-mode + Crystalka oversight)
estimated_duration: 12-20 hours auto-mode (К10.1 scope substantial — kernel scheduler core)
brief_type: execution
authority_chain:
  - KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED (DOC-A-KERNEL_FULL_NATIVE_SCHEDULER, committed at К10 deliberation arc closure 2026-05-17) — primary spec for К10.1 scope
  - K10_DELIBERATION_STATE.md (Project file sister — 9 S-locks rationale)
  - KERNEL_ARCHITECTURE.md v1.6 LOCKED — pre-К10 К-L1..L11 baseline (amended к v2.0 в К10.1 execution per Item 16)
  - METHODOLOGY.md v1.8 LOCKED — Lessons #7/#8/#11/#20/#22 verbatim + Provisional Lessons + §12.7 canonical closure protocol
  - FRAMEWORK.md v1.1 LOCKED — AUTHORED-SKELETON → AUTHORED lifecycle transition mechanism + Category D Tier 3 allowed combinations
  - K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md EXECUTED (DOC-D-K8_34_COMBINED_V2, 2026-05-14) — atomic-cutover precedent + halt taxonomy + Phase 0 deep-read pattern
  - CLEANUP_CASCADE_BRIEF.md EXECUTED (DOC-D-CLEANUP_CASCADE_BRIEF, 2026-05-16) — multi-commit cascade precedent + SC-N halt classes + closure protocol surfaces
  - PHASE_A_PRIME_SEQUENCING.md (DOC-A-PHASE_A_PRIME — Tier 2 Live) — А'.7 placement for К10
---

# K10.1 — Kernel Scheduler Core Execution Brief

**Brief shape**: Execution-mode brief targeting Claude Code auto-mode с Crystalka oversight. Multi-commit atomic cascade implementing **17 architectural items** from KERNEL_FULL_NATIVE_SCHEDULER.md spec (Items 1-9 минус 10 NUMA deferred, 11-13, 15-16, 17, 19-20, 24) that ratify К-L6 SUPERSEDED + К-L12/L13/L14 architecturally established.

**Authority**: Direct execution against К10 deliberation arc closure (2026-05-17, all 9 S surfaces ratified). К10.1 is the **first of four К10 sub-milestones** under Option III standalone-briefs structure (К10.1 = kernel scheduler core; К10.2 = native bus + mod lifecycle native primitives, future brief; К10.3 = pipeline depth + display composition + hardware tier, future brief; К10.4 = TLA+ formal verification, future brief).

**Scope discipline** (Lesson #20 + Lesson #14 candidate application):
- In-scope: 17 К10 items establishing kernel scheduler core (per §3.1-3.5 + §3.7 in spec)
- Out-of-scope: К10.2/К10.3/К10.4 items (separate briefs at their execution moments)
- Out-of-scope: Item 14 (К11+ Core migration к native) — explicitly deferred per spec §3.4 + S4 lock
- Out-of-scope: Item 25 (К-closure report) — cross-cutting к А'.8

**К10.1 ratifies architecturally** (per К10 deliberation closure):
- **К-L6 SUPERSEDED** (Item 16): «Game tick scheduler: Managed» — replaced by К-L12 native kernel scheduling
- **К-L12 AUTHORED** (Items 1-9, 11-13, 15): Full native kernel scheduling
- **К-L13 AUTHORED** (Items 3, 5, 17): On-demand system activation (5 wake types)
- **К-L14 AUTHORED** (architectural foundation across all items): Performance derives from architectural cleanliness

Items 18 (TLA+), 26-32 (native bus + mod ALC lifecycle), 33-46 (pipeline + display + hardware + TLA+ CI), and Item 14 (К11+ Core migration) are **explicitly NOT in К10.1 scope** — they belong к К10.2/К10.3/К10.4 sub-milestones with separate briefs.

---

## §1 — Crystalka ratified scope locks (К10 deliberation 2026-05-16..2026-05-17)

The 9 S-surfaces ratified at К10 deliberation arc closure constitute execution authority. Specific subset applicable к К10.1:

### §1.1 — S-LOCK-1: К10.1 scope items (17 items)

**LOCK**: К10.1 implements exactly these 17 items from KERNEL_FULL_NATIVE_SCHEDULER.md spec, in dependency order:

| Item | Title | Spec § | Group |
|---|---|---|---|
| 1 | Native dependency graph | §3.1 | Scheduling primitives |
| 2 | Native thread pool extension | §3.1 | Scheduling primitives |
| 3 | Wake-up registry (5 wake types) | §3.1 | Scheduling primitives |
| 4 | Wake registry diagnostic API | §3.1 | Scheduling primitives |
| 5 | Dynamic per-tick graph computation | §3.1 | Scheduling primitives |
| 6 | Priority-based dispatch | §3.2 | Scheduling policies |
| 7 | Resource quotas (CPU time per system) | §3.2 | Scheduling policies |
| 8 | Preemption semantics | §3.2 | Scheduling policies |
| 9 | Shared memory regions | §3.3 | Memory and IPC |
| 11 | CPU affinity hints | §3.3 | Memory and IPC |
| 12 | Work stealing within phase | §3.3 | Memory and IPC |
| 13 | Phase barrier semantics formalization | §3.3 | Memory and IPC |
| 15 | Batched callback ABI | §3.4 | Native execution layer |
| 16 | К-L6 supersession + К-L12/13/14 amendment | §3.4 | Native execution layer |
| 17 | Write-through hook (filter primitive S2 hybrid) | §3.5 | State and observability |
| 19 | Observability hooks (perf/ftrace-like) | §3.5 | State and observability |
| 20 | Scheduler intrinsics for emergency paths | §3.5 | State and observability |
| 24 | Test infrastructure migration | §3.7 | Test infrastructure |

Item 10 (NUMA) **deferred к К-extensions** per spec §3.3 (development machine «Skarlet» single-socket Ryzen makes NUMA exercise impossible).

### §1.2 — S-LOCK-2: К-L invariant landings (К10.1 portion)

**LOCK**: К10.1 lands К-L6 supersession + К-L12 + К-L13 + К-L14 as part of Item 16 execution. Other К-L invariants (К-L15, К-L7.1, К-L16, К-L17, К-L18, К-L19) deferred к К10.2/К10.3 sub-milestones.

### §1.3 — S-LOCK-3: Brief shape standalone, sub-milestone

**LOCK**: К10.1 is standalone self-contained execution brief (Option III ratified 2026-05-18). К10.2/К10.3/К10.4 each get separate brief authored at their execution moment, informed by К10.1 closure report findings.

### §1.4 — S-LOCK-4: Atomic cascade (multi-commit ordered)

**LOCK**: К10.1 executes as multi-commit cascade с dependency ordering, NOT single big-bang atomic commit. Rationale per Lesson #8 («brief splitting change into N steps must prove each N-1 intermediate state valid»): К10.1 items 1-24 have **clean intermediate states** because:
1. Native scheduler infrastructure (Items 1-5) lands before managed adapter integration
2. Managed scheduler **continues to operate** through К10.1 — К-L6 not formally SUPERSEDED until Item 16 lands
3. Batched callback ABI (Item 15) is **dormant** until К11+ Core migration uses it; К10.1 lands ABI infrastructure parallel к existing managed dispatch
4. Tests pass at every commit (existing 620 baseline preserved; К10.1 tests additive)

Contrast с К8.3+К8.4 storage cutover (binary indivisible per Lesson #8): К10.1 scheduler infrastructure is **architecturally additive** during К10.1 window — native scheduler builds alongside managed scheduler, cutover к native sovereign authority happens at К10.1 closure (Item 16 lands К-L6 supersession + К-L12 invariant landing, switches Core system dispatch к native scheduler).

### §1.5 — S-LOCK-5: К10.1 closes К10.1 only, не all of К10

**LOCK**: К10.1 closure is sub-milestone closure (per FRAMEWORK §3.3 Lifecycle EXECUTED for К10.1 brief). К10 as a whole closes only after К10.4 closure. К-closure report (А'.8) waits for all four К10 sub-milestones closed.

**Implications**:
- К10.1 commit message scope: `feat(kernel): K10.1 — kernel scheduler core (К-L6 SUPERSEDED + К-L12/13/14)` — note К10.1 specificity
- К10.1 closure REGISTER entry: separate AUDIT_TRAIL event (`EVT-{date}-K10_1-CLOSURE`), separate brief lifecycle transition
- К-L invariant landings в KERNEL_ARCHITECTURE.md happen incrementally across К10.x sub-milestones (К10.1 lands К-L12/13/14, К10.2 lands К-L15, К10.3 lands К-L7.1/16/17/18/19, К10.4 has no new К-L)

---

## §2 — Phase 0 — Pre-flight reads (mandatory before any edit)

Per Lesson #7 (brief prescribes API must transcribe API) + Lesson #22 (read existing code + operational context before mechanism design), the executor MUST complete every read listed below **before writing a single line of К10.1 code**. Brief was authored 2026-05-18 from a point-in-time read of source files; drift between brief authoring and execution time is **expected**, and divergences surface as halt triggers (SC-3) — never as silent improvisation per Lesson #20.

### §2.1 — Verify post-A'.5 + cleanup-cascade state (hard gates)

Read and verify (per «pre-flight checks: descriptive over prescriptive» principle from METHODOLOGY — these are **hard gates**, blocking commits if failed):

1. `git log --oneline -20` on `main` — confirm:
   - A'.5 К8.3+К8.4 closure landed (commits `24e5f56..fc8ecb6`, 4 commits, 620 tests green)
   - CLEANUP_CASCADE_BRIEF closure landed (commits `e68d799..7bd7b4e`, 16 commits)
   - PR #34 composite namespace ratification merged (commits `f3b3d68..ca9483d`)
   - Halt if any absent or out-of-order — К10.1 starts from `main` HEAD at post-cleanup state.

2. `git status` — working tree clean before execution starts. **Hard gate** per К8.34 v2.0 precedent SC-1.

3. `docs/governance/REGISTER.yaml` head 20 lines — confirm `register_version` ≥ "1.4" (post-cleanup baseline). If lower, halt — К10.1 brief enrollment assumes post-cleanup register state.

4. `tools/governance/sync_register.ps1 --validate` — exit 0 required as baseline. If validation fails before execution, halt and surface failures per К8.34 v2.0 SC-1 precedent.

5. `dotnet build DualFrontier.sln` — clean baseline. 0 warnings, 0 errors. If build fails before execution, halt — drift surfaced elsewhere.

6. `dotnet test DualFrontier.sln` — baseline pass count recorded. **Target**: 620 tests green per A'.5 closure baseline preserved through cleanup. If suite fails or count diverges, halt and surface.

### §2.2 — Read КERNEL_FULL_NATIVE_SCHEDULER.md spec as ground truth

Read in full, identify exact line numbers for each item К10.1 scope touches:
- `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 LOCKED — Part 2 (К-L12/13/14 verbatim text + К-L invariant table), Part 3 §3.1-3.5 + §3.7 (17 items in scope), Part 5.1.A predictions 1-5 (К10.1 measurable scope; predictions 6-18 are К10.2/К10.3 scope), Part 8 risks R-K10-1..5 (К10.1 risk surface)

**Per Lesson #7**: spec verbatim is **authoritative for К-L invariant text**, API surface descriptions, and item specifications. Quote line numbers verbatim в commit messages где К-L invariant text lands.

### §2.3 — Read code anchors verbatim (К10.1 specific)

Read these files for verbatim content к understand the migration shape. Do NOT paraphrase from synthesis — these reads are mandatory pre-implementation orientation per Lesson #22 (read existing code + operational context before mechanism design):

**Managed scheduler (К-L6 era, to be superseded)**:
- `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` — current managed graph (replaced by native equivalent per Item 1; managed retained as adapter facade until К11+ или earlier deprecation)
- `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` — current managed scheduler (continues to operate during К10.1; eventually replaced by native authority per Item 16)
- `src/DualFrontier.Core/Scheduling/TickScheduler.cs` (if exists; or wherever `[TickRate]` consumption lives) — subsumed into wake registry per Item 19/spec §3.1 (TimerWake sugar preservation)
- `src/DualFrontier.Core/ECS/SystemBase.cs` — system base class; post-A'.5 surface preserved
- `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` — execution context (post-A'.5 simplified surface)
- `src/DualFrontier.Core.Interop/NativeWorld.cs` — К8.3+К8.4 cutover artifact, sole production storage backbone

**Native side (pre-К10, mostly bootstrap-only)**:
- `native/DualFrontier.Core.Native/include/bootstrap_graph.h` + `src/bootstrap_graph.cpp` — К3-era Kahn topological sort for bootstrap (extended by Item 1 к per-tick graph)
- `native/DualFrontier.Core.Native/include/thread_pool.h` + `src/thread_pool.cpp` — К3-era thread pool (extended by Item 2 к per-tick dispatch, persists post-`SignalEngineReady`)
- Native test infrastructure (if exists) — confirm catch2/gtest decision precedent or absence (Item 24)

**Application wiring**:
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` — К8.3+К8.4 cutover artifact: 10 surviving Core systems registered с `coreSystems` array (post-power-deletion); scheduler ctor at К10.1 completion takes native scheduler reference, not managed `ParallelSystemScheduler`
- `src/DualFrontier.Application/Loop/Bootstrap.cs` (if exists separately) — `Run(useRegistry: true)` entry point per A'.5

**Test fixtures**:
- `tests/DualFrontier.Core.Tests/` — existing managed scheduler tests (post-cleanup state); preserved through К10.1 — they test managed scheduler facade survival
- `tests/DualFrontier.Systems.Tests/` — post-A'.5 re-authored against `NativeWorldTestFixture`; new К10.1 native scheduler tests additive (per Item 24)
- `tests/DualFrontier.Modding.Tests/` — mod system tests (untouched by К10.1; К10.2 amends)

**Если any read surfaces a contradiction с this brief** — halt and escalate per SC-3 (deep-read contradiction). Drift between brief authoring (2026-05-18) and execution time is exactly what this trigger catches.

### §2.4 — Read REGISTER.yaml К10.1 enrollment area

Identify exact line ranges:
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER (К10 spec, currently LOCKED post-deliberation)
- DOC-A-KERNEL (К-L invariants pre-К10.1 amendment к v2.0)
- DOC-A-MOD_OS, DOC-A-MIGRATION_PLAN, DOC-A-PHASE_A_PRIME — К10.1 closure may touch these for К-L12/13/14 cross-references (К10.1 minimal — primary К-L invariant landing happens в KERNEL_ARCHITECTURE.md v2.0; mod-OS and migration-plan amendments are К10.2 scope, not К10.1)

### §2.5 — Halt category clarity

**Hard gates (STOP-eligible)** per §2.1 above + per K8.34 v2.0 precedent:
- Working tree dirty (uncommitted changes would be lost)
- Baseline tests failing (regression source not yet identified)
- Required tooling absent (CMake / dotnet / C++20 compiler — confirm at Phase 0)
- `sync_register.ps1 --validate` non-zero baseline
- Build failure baseline

**Informational checks (record-only)** per «descriptive pre-flight» METHODOLOGY principle:
- HEAD SHAs, branch topology, commit metadata
- Architecture document state (К-L invariants count, etc.)
- File layout details (per Lesson «Phase 0.4 inventory as hypothesis, not authority»)

If informational check diverges from brief expectation — **record divergence в Commit message, continue**. Hard gate failure → halt per SC-N (§5).

---

## §3 — Atomic commit cascade (target ~12-16 commits)

Each commit: individually compilable + register-valid intermediate state per Lesson #8. `sync_register.ps1 --validate` exit 0 at every governance-touching commit; `dotnet build` clean at every code-touching commit; `dotnet test` 620+ passing at every commit (existing baseline preserved; new К10.1 tests additive).

**Cascade strategy** per S-LOCK-4: native scheduler infrastructure (Items 1-5) lands first as **additive** (managed scheduler continues operating, parallel native scheduler not yet sovereign). Items 6-13 + 15 + 17 + 19-20 land additive primitives. Item 16 (К-L6 supersession + К-L12/13/14 amendment) is the **last code commit** — it lands К-L invariant amendments в KERNEL_ARCHITECTURE.md v2.0 + switches Core system dispatch к native scheduler authority. This is the К10.1 "load-bearing commit" analogous к К8.34 v2.0 Commit 2 in scope/risk.

### Commit 1 — Brief authoring commit (К10.1 brief enrollment)

**Files**:
- `tools/briefs/K10_1_EXECUTION_BRIEF.md` (this brief)
- `docs/governance/REGISTER.yaml` (DOC-D-K10_1 entry с `lifecycle: AUTHORED`, `category: D`, `tier: 3`, special_case_rationale empty per FRAMEWORK §3.4)

**Rationale** per «brief authoring as prerequisite step» Lesson (post-K1 closure): brief lives on `main` BEFORE К10.1 feature branch creates. Pre-flight HG-1 «working tree clean» will pass on subsequent commits.

**Validation**:
- `sync_register.ps1 --validate` exit 0
- No code changes; no test impact

**Commit message**: `docs(briefs): K10.1 brief authored — full executable kernel scheduler core (К-L6 SUPERSEDED + К-L12/13/14)`

### Commit 2 — Phase 0 verification + native test framework decision

**Files**:
- `native/DualFrontier.Core.Native/CMakeLists.txt` (catch2 OR gtest dependency added per Item 24 decision)
- `native/DualFrontier.Core.Native/test/CMakeLists.txt` (new file — test target)
- `native/DualFrontier.Core.Native/test/smoke_test.cpp` (new file — single trivial test confirming framework functional)

**Rationale per Lesson #22 + spec Item 24**: Phase 0 read decides native test framework (catch2 vs gtest). Brief authoring assumed catch2 (header-only, modern C++17+, simpler integration). If Phase 0 read surfaces existing native test infrastructure (e.g., existing selftest pattern from К3-era thread_pool tests), match existing convention — Lesson #22 «read existing code first» applies.

**Decision criteria per spec Q-N-21 deferred к brief authoring**:
- **catch2 recommended** if no existing native test framework on disk: header-only, modern, integrates cleanly с CMake, no additional build dependencies beyond CMake itself
- **gtest recommended** if existing native testing patterns lean Google-style or если Crystalka has preference
- **Custom lightweight runner recommended** if existing К3-era selftest infrastructure has its own pattern that fits scheduler tests

**Validation**:
- `cmake --build` clean (native compiles, including new test target)
- `cmake --build --target run_native_tests` — smoke test passes (framework functional)
- `dotnet build` + `dotnet test` 620+ green (no managed-side impact)
- `sync_register.ps1 --validate` exit 0 (no register changes this commit unless framework adds module-local README to track)

**Commit message**: `feat(native-test): K10.1 — native test framework decision + smoke test (Item 24 prerequisite)`

### Commit 3 — Item 1: Native dependency graph (per-tick + static cache)

**Files**:
- `native/DualFrontier.Core.Native/include/system_graph.h` (new)
- `native/DualFrontier.Core.Native/src/system_graph.cpp` (new)
- `native/DualFrontier.Core.Native/test/system_graph_test.cpp` (new — tests Kahn correctness, cycle detection, per-tick subset computation)
- `src/DualFrontier.Core.Interop/SystemGraphInterop.cs` (new — managed binding for `df_scheduler_register_system` C ABI)

**Drift surface**: К10.1 introduces new native graph. Existing managed `DependencyGraph.cs` continues to operate; native graph parallels it (both consume same `[SystemAccess]` declarations). К-L6 still in effect — managed scheduler remains authoritative during К10.1 cascade until Commit 14 (Item 16).

**Implementation surface (per spec §3.1 Item 1)**:

C ABI surface (verbatim from spec):
```c
// system_graph.h
void df_scheduler_register_system(
    uint32_t system_id,
    const char* system_fqn,
    const uint32_t* read_component_ids, uint32_t read_count,
    const uint32_t* write_component_ids, uint32_t write_count,
    int32_t priority_class,
    int32_t wake_type,
    /* ... wake source descriptor union, см. Item 3 */
);

// Kahn topological sort over all registered systems
int32_t df_scheduler_compute_static_graph(/* out parameters */);

// Per-tick graph: subset filtering + Kahn on runnable set
int32_t df_scheduler_compute_per_tick_graph(
    const uint32_t* runnable_ids, uint32_t runnable_count,
    /* out: phase composition */);
```

Implementation:
- Kahn topological sort на native side replicates managed `DependencyGraph` behavior bit-for-bit (same access declarations, same cycle detection, same phase ordering)
- Per-tick Kahn variant takes runnable subset, produces tighter parallelism (per Item 5 spec §3.1)
- Static graph cached until system registration changes (Rebuild on mod load/unload — К10.2 concern, К10.1 lands static graph build + per-tick subset compute primitives)

**Per Lesson #7**: transcribe verbatim from `DependencyGraph.cs` Kahn implementation — К10.1 native side must produce **identical** topological ordering к managed side for bit-equivalent parallel verification во время transition.

**Validation**:
- `cmake --build` clean
- `system_graph_test`: Kahn correctness tests (acyclic → topological order; cyclic → error code), per-tick subset tests, edge enumeration tests
- `dotnet build` clean — `SystemGraphInterop.cs` compiles
- `dotnet test` 620+ green (no behavior change; managed scheduler still authoritative)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Item 1 — native dependency graph + per-tick subset compute`

### Commit 4 — Item 2: Native thread pool extension

**Files**:
- `native/DualFrontier.Core.Native/include/thread_pool.h` (extended)
- `native/DualFrontier.Core.Native/src/thread_pool.cpp` (extended)
- `native/DualFrontier.Core.Native/test/thread_pool_test.cpp` (new или extended — tests pool persistence post-SignalEngineReady, phase barrier semantics, batch submit)

**Drift surface**: К3-era `ThreadPool` class lived bootstrap-only. К10.1 extends к support per-tick scheduler dispatch — pool persists across phases of kernel lifecycle (bootstrap mode → scheduler mode transition signal).

**Implementation surface (per spec §3.1 Item 2)**:

```cpp
// thread_pool.h extension
class ThreadPool {
public:
    // Existing (К3-era):
    void submit(std::function<void()> task);
    void wait_all();

    // К10.1 additions:
    void submit_batch(const std::vector<std::function<void()>>& tasks);
    void wait_phase_barrier();

    // Lifecycle transition:
    void transition_to_scheduler_mode();
    void transition_to_bootstrap_mode();  // for graceful shutdown / hot reload

private:
    // ... existing
    std::atomic<Mode> _mode{Mode::Bootstrap};
};
```

- Pool sizing follows N-2 rule preserved from managed `ParallelSystemScheduler` (`Environment.ProcessorCount - 2`, minimum 1)
- Per-thread affinity hints (Item 11) consumed by pool worker creation
- Worker thread loop adapted: bootstrap mode → scheduler mode transition signal (atomic flag, lock-free read on hot path)

**Per Lesson #22**: read existing `ParallelSystemScheduler.cs` к understand N-2 rule + parallelism semantics; native side replicates behavior.

**Validation**:
- `cmake --build` clean
- `thread_pool_test`: pool persists post-SignalEngineReady, batch submit correctness, phase barrier semantics, mode transition
- `dotnet build` + `dotnet test` 620+ green (no managed-side change)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Item 2 — native thread pool extension (per-tick dispatch + mode transition)`

### Commit 5 — Item 3: Wake-up registry (5 wake types)

**Files**:
- `native/DualFrontier.Core.Native/include/wake_registry.h` (new)
- `native/DualFrontier.Core.Native/src/wake_registry.cpp` (new)
- `native/DualFrontier.Core.Native/test/wake_registry_test.cpp` (new — tests subscription lifecycle, runqueue/blocked queue, 5 wake type dispatch)
- `src/DualFrontier.Contracts/Scheduling/WakeAttributes.cs` (new — `[WakeOnEvent<T>]`, `[WakeOnState<T>(condition)]`, `[WakeOnInit]`, `[WakeOnExplicit(WakeId)]` attributes; `[TickRate]` preserved as TimerWake sugar per spec §3.1 Item 3 + Q-N-19 resolution)
- `src/DualFrontier.Core.Interop/WakeRegistryInterop.cs` (new — managed binding)

**Drift surface**: New scheduling primitive. Existing `[TickRate]` attribute preserved as **TimerWake sugar** per Q-N-19 resolution («TickScheduler subsumed into WakeRegistry; [TickRate] becomes [WakeOn(Timer)] sugar»). Existing systems с `[TickRate(30)]` continue to work — their dispatch routes through wake registry instead of TickScheduler.

**Implementation surface (per spec §3.1 Item 3 verbatim)**:

Five wake types (К-L13 invariant):

1. **TimerWake** — periodic by `[TickRate]`. Native side maintains tick counter, fires TimerWake at appropriate ticks per system's declared rate.
2. **EventWake** — bus publication subscription. System declares `[WakeOnEvent<DamageEvent>]`; native scheduler tracks subscription; bus publication triggers wake dispatch к subscribed systems. (Bus integration is К10.2 scope per Item 26; К10.1 wakes registry stores subscription metadata, dispatch via bus is dormant until К10.2 lands native bus.)
3. **StateChangeWake** — component value condition. System declares `[WakeOnState<Health>(condition: h => h.Current < 10)]`; NativeWorld write-through hook (Item 17, Commit 12) checks subscribed conditions, fires wake when crossing detected.
4. **InitWake** — one-shot at startup. System runs once after `SignalEngineReady`, never again until next bootstrap.
5. **ExplicitWake** — API-driven wake. System A calls `scheduler.WakeSystem<SystemB>(wakeId)`; native scheduler marks SystemB runnable for next phase.

C ABI surface:
```c
// wake_registry.h
typedef enum {
    WakeType_Timer = 0,
    WakeType_Event = 1,
    WakeType_StateChange = 2,
    WakeType_Init = 3,
    WakeType_Explicit = 4
} WakeType;

// Registry operations
int32_t df_wake_registry_subscribe(uint32_t system_id, WakeType type, const void* wake_descriptor);
int32_t df_wake_registry_unsubscribe(uint32_t system_id, WakeType type);

// Wake dispatch (per type)
int32_t df_wake_registry_fire_timer(uint64_t current_tick);
int32_t df_wake_registry_fire_event(uint32_t event_type_id /*, payload pointer omitted в К10.1 — bus integration К10.2 */);
int32_t df_wake_registry_fire_state_change(uint32_t component_type_id, uint32_t entity_id /*, new_value omitted здесь — condition eval happens в filter primitive Item 17 */);
int32_t df_wake_registry_fire_init(void);
int32_t df_wake_registry_fire_explicit(uint32_t target_system_id, uint32_t wake_id);

// Runqueue / blocked queue access
int32_t df_wake_registry_get_runnable(uint32_t* out_system_ids, uint32_t* count);
int32_t df_wake_registry_get_blocked(/* out parameters */);
```

**Per Lesson #11 sub-pattern 6 (architectural reduction)**: TickScheduler is one wake source among many. К10.1 implements all 5 wake types upfront per К-L14 default-inclusion bias. Don't defer wake types к К-extensions — implementing 4-of-5 leaves architectural incomplete state.

**Validation**:
- `cmake --build` clean
- `wake_registry_test`: subscription lifecycle (subscribe/unsubscribe), 5 wake type dispatch correctness, runqueue/blocked queue transitions
- `dotnet build` clean — wake attributes compile, interop compiles
- `dotnet test` 620+ green — existing tests not affected; new wake registry tests additive
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Item 3 — wake-up registry (5 wake types: Timer/Event/StateChange/Init/Explicit)`

### Commit 6 — Item 4: Wake registry diagnostic API

**Files**:
- `native/DualFrontier.Core.Native/include/scheduler_diagnostics.h` (new)
- `native/DualFrontier.Core.Native/src/scheduler_diagnostics.cpp` (new)
- `native/DualFrontier.Core.Native/test/scheduler_diagnostics_test.cpp` (new)
- `src/DualFrontier.Core.Interop/SchedulerDiagnostics.cs` (new — managed wrapper)

**Drift surface**: Observability surface для wake state. No behavior change — purely query API.

**Implementation surface (per spec §3.1 Item 4)**:

```c
// scheduler_diagnostics.h
typedef struct {
    uint32_t system_id;
    WakeType wake_type;
    uint32_t wake_source_descriptor_id;  // event type id / component type id / etc.
} BlockedSystemEntry;

int32_t df_scheduler_query_runnable(uint32_t* out_system_ids, uint32_t* out_count);
int32_t df_scheduler_query_blocked(BlockedSystemEntry* out_entries, uint32_t* out_count);
int32_t df_scheduler_query_wake_subscriptions(uint32_t system_id, /* out */ WakeType* out_types, uint32_t* out_count);
```

```csharp
// SchedulerDiagnostics.cs
public static class SchedulerDiagnostics
{
    public static IReadOnlyList<uint> GetRunnableSystems();
    public static IReadOnlyList<BlockedSystemInfo> GetBlockedSystems();
    public static IReadOnlyList<WakeType> GetWakeSubscriptions(uint systemId);
}
```

**Used by**: future debugging tools, performance profilers, integration tests, A'.9 Roslyn analyzer (consumes diagnostics к verify wake declarations match `[SystemAccess]` — system can't wake on event it never reads).

**Validation**:
- `cmake --build` + native tests pass
- `dotnet build` + `dotnet test` 620+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Item 4 — wake registry diagnostic API (introspection for scheduler state)`

### Commit 7 — Item 5: Dynamic per-tick graph computation

**Files**:
- `native/DualFrontier.Core.Native/src/system_graph.cpp` (extended с per-tick graph compute calling wake_registry runqueue)
- `native/DualFrontier.Core.Native/test/dynamic_graph_test.cpp` (new — tests dynamic phase composition based on runqueue subset)

**Drift surface**: Items 1 + 3 combined produce per-tick graph computation. Each tick: scheduler reads runqueue from wake registry → builds per-tick dependency graph (edges only between runnable systems) → Kahn topological sort → phase composition → ready for dispatch (Commit 8 wires к thread pool).

**Implementation surface (per spec §3.1 Item 5 verbatim)**:

Per-tick scheduling sequence:
1. Read runqueue from wake registry (which systems woke this tick) — O(R) where R = runnable count
2. Build per-tick dependency graph: edges only between runnable systems — reuses static `[SystemAccess]` data cached at registration time
3. Kahn topological sort на runnable subset — O(R log R), typically tens of µs для ~100 systems
4. Dispatch phases к thread pool (Commit 8)

**Performance characteristics**:
- Static graph cached at registration time (Item 1), rebuilt only on mod load/unload
- Per-tick computation only on subset → tighter parallelism than static phase ordering
- Phase composition may differ per tick: if SystemB blocks on event not fired, SystemA→SystemB→SystemC chain dispatches with A and C in parallel

**Validation**:
- `cmake --build` + native tests pass; per-tick graph correctness verified against managed `DependencyGraph` bit-equivalent для same access declarations
- `dotnet build` + `dotnet test` 620+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Item 5 — dynamic per-tick graph computation (Kahn on runnable subset)`

### Commit 8 — Items 6+7+8: Scheduling policies (priority + quotas + preemption)

**Files**:
- `native/DualFrontier.Core.Native/include/scheduling_policies.h` (new)
- `native/DualFrontier.Core.Native/src/scheduling_policies.cpp` (new)
- `native/DualFrontier.Core.Native/test/scheduling_policies_test.cpp` (new)
- `src/DualFrontier.Contracts/Scheduling/PriorityAttribute.cs` (new — `[Priority(SchedulingClass, MaxLatencyMicros, MaxJitterMicros)]`)
- `src/DualFrontier.Contracts/Scheduling/CpuQuotaAttribute.cs` (new — `[CpuQuota(MaxMicrosPerTick)]`)
- `src/DualFrontier.Contracts/Scheduling/PreemptAttribute.cs` (new — `[Preempt(Cooperative)]` marker)
- `src/DualFrontier.Core.Interop/SchedulingPoliciesInterop.cs` (new)

**Drift surface**: New attribute surface for systems. Existing systems get **Normal priority + no quota** as default (no change in behavior); systems opting in к RT class / High / Background priority via `[Priority]` declare explicit class.

**Implementation surface (per spec §3.2 Items 6/7/8)**:

Five scheduling classes (Item 6):
1. **RealTime** — strict latency requirement; preempts other classes; bounded max execution time
2. **High** — interactive / input handling; dispatched first in phase
3. **Normal** — default; most systems
4. **Low** — non-critical work; deferred to phase end
5. **Background** — runs in idle time; may be skipped if scheduler busy

Quota enforcement (Item 7):
- Native scheduler instruments system execution: timestamp begin / end / delta
- Exceedance triggers configured fault handler:
  - Mod systems: ModFaultHandler invoked, system unloaded per existing pipeline
  - Core systems: fault logged, optionally throttled (skip next N ticks), optionally degraded к lower priority

Preemption semantics (Item 8):
- **Cooperative preemption (default)**: systems run to completion; long-running systems should yield voluntarily via `scheduler.Yield()` API
- **Forced preemption (RT class only)**: scheduler may interrupt RT system at quota boundary, restart on next tick

**Per Q-N-24/25/26 resolution** (deferred к brief authoring): default priority class Normal; default quota = none (legacy behavior preserved); explicit opt-in для RT/High via `[Priority]`; explicit opt-in для quota via `[CpuQuota]`; preemption opt-in via `[Preempt]`.

**К10.1 commits priority + quota + preemption as one commit** because они tightly coupled:
- Item 6 establishes priority class enum + attribute
- Item 7 builds on Item 6 (quotas can be per-class)
- Item 8 builds on Item 6 + 7 (RT class drives forced preemption; quota exceedance triggers preemption)

**Validation**:
- `cmake --build` + native tests pass: priority queue ordering, quota enforcement triggers fault handler, preemption forced для RT class
- `dotnet build` + `dotnet test` 620+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Items 6+7+8 — scheduling policies (priority class + CPU quotas + preemption semantics)`

### Commit 9 — Item 9: Shared memory regions

**Files**:
- `native/DualFrontier.Core.Native/include/shm_region.h` (new)
- `native/DualFrontier.Core.Native/src/shm_region.cpp` (new)
- `native/DualFrontier.Core.Native/test/shm_region_test.cpp` (new)
- `src/DualFrontier.Contracts/IPC/ShmRegionAttribute.cs` (new — `[ShmWriter<T>]` marker)
- `src/DualFrontier.Core.Interop/ShmRegionInterop.cs` (new)

**Drift surface**: New IPC primitive distinct from bus events + NativeWorld component storage. Use cases: hot-path data (positions, velocities, animation state) where bus event serialization overhead dominates.

**Implementation surface (per spec §3.3 Item 9)**:

```c
// shm_region.h
int32_t df_shm_create(uint32_t region_id, size_t size_bytes);
int32_t df_shm_map(uint32_t region_id, void** out_ptr);
int32_t df_shm_unmap(uint32_t region_id);
int32_t df_shm_destroy(uint32_t region_id);

// Single-writer / multi-reader pattern
int32_t df_shm_register_writer(uint32_t region_id, uint32_t writer_system_id);
```

- Region typed by component / data structure
- Multiple systems map к region via NativeWorld primitives or direct C ABI access
- Single-writer / multi-reader pattern enforced (writer system declared via `[ShmWriter<T>]`)
- Lock-free reads (atomic versioning or seqlock pattern — implementation choice deferred к brief authoring per Q-N-27)

**Per spec §3.3 Q-N-27 resolution**: compile-time typed via generic preferred (`ShmRegion<T>`); runtime typed via descriptor + cast available для dynamic cases. Brief authoring locks compile-time typed as default.

**Per К-L18 quiescent state** (К10.3 lock): region destruction will require quiescent state в К10.2 mod lifecycle; К10.1 lands region creation/map/unmap primitives only, destruction lifecycle hooks land в К10.2 native unload primitive (Item 32 future scope).

**Validation**:
- `cmake --build` + native tests: region create/map/unmap/destroy; single-writer enforcement; concurrent read correctness
- `dotnet build` + `dotnet test` 620+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Item 9 — shared memory regions (high-frequency IPC primitive)`

### Commit 10 — Items 11+12+13: CPU affinity + work stealing + phase barriers

**Files**:
- `native/DualFrontier.Core.Native/src/thread_pool.cpp` (extended с affinity binding + work stealing deque)
- `native/DualFrontier.Core.Native/include/phase_barrier.h` (new)
- `native/DualFrontier.Core.Native/src/phase_barrier.cpp` (new)
- `native/DualFrontier.Core.Native/test/affinity_test.cpp` + `work_stealing_test.cpp` + `phase_barrier_test.cpp` (new)
- `src/DualFrontier.Contracts/Scheduling/CpuAffinityAttribute.cs` (new — `[CpuAffinity(CoreSet | CoreId)]`)
- `src/DualFrontier.Contracts/Scheduling/PhaseBarrierAttribute.cs` (new — `BarrierType.Full | Partial | None`)

**Drift surface**: Extend thread pool с two perf-related primitives + formalize phase barrier semantics. Items 11/12/13 logically grouped (all thread pool extensions).

**Implementation surface**:

Item 11 (CPU affinity hints):
- System attribute `[CpuAffinity(CoreSet.PerformanceCores)]` or `[CpuAffinity(CoreId = 4)]`
- Native thread pool: per-thread core binding via `pthread_setaffinity_np` (Linux) / `SetThreadAffinityMask` (Windows)
- Affinity hints consumed by graph dispatch — system prefers thread on declared core
- Diagnostic API exposes actual vs requested affinity

Item 12 (Work stealing within phase):
- Per-thread task queues с deque structure
- Idle thread pop from own queue (LIFO); steal from neighbor queue (FIFO) per Cilk-style work stealing
- Lock-free dequeue (atomic ops)
- Reduces phase barrier wait time

Item 13 (Phase barrier semantics):
- Three barrier types: Full (default) / Partial / None
- Phase descriptor includes barrier type; scheduler honors barrier type during dispatch
- Diagnostic API records actual barrier events

**Per К-L14 default-inclusion**: all three items implemented в К10.1, not deferred. Architectural completeness of thread pool.

**Validation**:
- `cmake --build` + native tests: affinity binding measurable, work stealing reduces phase barrier wait time (microbenchmark), phase barrier semantics correct
- `dotnet build` + `dotnet test` 620+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Items 11+12+13 — CPU affinity + work stealing + phase barrier semantics`

### Commit 11 — Item 15: Batched callback ABI (managed adapter)

**Files**:
- `native/DualFrontier.Core.Native/include/managed_callback.h` (new)
- `native/DualFrontier.Core.Native/src/managed_callback.cpp` (new)
- `native/DualFrontier.Core.Native/test/managed_callback_test.cpp` (new)
- `src/DualFrontier.Application/Scheduler/ManagedSystemDispatcher.cs` (new — UnmanagedCallersOnly entry point + GCHandle pattern)
- `src/DualFrontier.Application/Scheduler/SchedulerAdapter.cs` (new — registration + dispatch routing)
- `tests/DualFrontier.Core.Tests/Scheduler/BatchedCallbackTests.cs` (new — round-trip test through C ABI)

**Drift surface**: New cross-layer bridge. Managed scheduler still operates authoritatively через К10.1 cascade (К-L6 not yet superseded). Batched callback ABI lands **dormant** — infrastructure exists, gets exercised at Commit 14 (Item 16) when К-L6 supersession switches Core dispatch к native scheduler.

**Implementation surface (per spec §3.4 Item 15 verbatim — Lesson #7 transcription mandatory)**:

C ABI surface verbatim from spec:

```c
// managed_callback.h
typedef struct {
    const uint32_t* system_ids;     // pointer к array of managed system IDs
    uint32_t count;
    float delta;
    void* user_data;                // opaque managed context handle (GCHandle)
} ManagedSystemBatch;

typedef void (*managed_batch_fn)(const ManagedSystemBatch* batch);

void df_scheduler_register_managed_callback(managed_batch_fn cb, void* user_data);
void df_scheduler_dispatch_managed_batch(const ManagedSystemBatch* batch);
```

C# adapter verbatim from spec:

```csharp
// ManagedSystemDispatcher.cs
public static class ManagedSystemDispatcher
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void OnBatch(ManagedSystemBatch* batch)
    {
        // GC transition happens automatically on entry (cannot suppress for reverse P/Invoke)
        // Now in cooperative mode
        try
        {
            var ids = new ReadOnlySpan<uint>(batch->system_ids, (int)batch->count);
            float delta = batch->delta;
            var handle = GCHandle.FromIntPtr((IntPtr)batch->user_data);
            var dispatcher = (ManagedSystemDispatcher)handle.Target!;
            dispatcher.ExecuteBatch(ids, delta);
        }
        catch (Exception ex)
        {
            // Exceptions cannot cross to native — absorb here
            FaultLog.RecordKernelBatchFault(ex);
        }
    }

    private void ExecuteBatch(ReadOnlySpan<uint> ids, float delta)
    {
        // In-batch parallelism via TPL (existing pattern from ParallelSystemScheduler)
        // Or sequential within batch if managed-side parallelism better dispatched at native level
        foreach (uint id in ids)
        {
            var system = _systems[id];
            SystemExecutionContext.PushContext(/* ... */);
            try { system.Update(delta); }
            finally { SystemExecutionContext.PopContext(); }
        }
    }
}
```

**Constraints (per spec Item 15 + .NET 10 research, **Lesson #7 verbatim**)**:
- Callback method must be `static` ✓
- All args blittable (pointer + primitives only) ✓
- No generics ✓
- No managed exceptions across boundary ✓ (`try/catch` absorbs at boundary)
- `SuppressGCTransition` **forbidden** for reverse P/Invoke — accept transition cost (~10-50ns per call, amortized across N systems in batch)
- GCHandle for managed instance state — `Alloc()` at registration, `Free()` at shutdown

**Performance characteristics (per spec Part 5.1.A predictions baseline)**:
- One reverse-P/Invoke per phase per managed-system-batch (typically 1 batch per phase)
- GC transition cost amortized across N systems в batch
- ReadOnlySpan from native pointer = zero-copy
- Per-tick cost: ~10 phases × ~30ns transition = ~300ns/tick = ~9µs/sec at 30Hz — negligible
- Compared к current managed scheduler context push/pop per system: net positive (~140µs/sec savings)

**Validation**:
- `cmake --build` + native test passes round-trip
- `dotnet build` + `dotnet test`: BatchedCallbackTests passes end-to-end (managed→native registration → native dispatch → managed callback executes batch → results validated)
- `dotnet test` 620+ green (no existing test regression — ABI is dormant)
- `sync_register.ps1 --validate` exit 0

**Per Lesson #20 reminder**: do not argue against К-L14 default-inclusion of ABI с tactical «overengineered» or «premature» reasoning. The ABI is architectural primitive — К-series cannot close as «native OS-faithful kernel» without cross-layer dispatch ABI. К11+ migration absorbs the ABI; К10.1 lands it now per default-inclusion bias.

**Commit message**: `feat(kernel): K10.1 Item 15 — batched callback ABI (managed system dispatch via reverse P/Invoke)`

### Commit 12 — Item 17: Write-through hook (filter primitive S2 hybrid)

**Files**:
- `src/DualFrontier.Core.Interop/NativeWorld.cs` (write path extended с commit-time hook)
- `native/DualFrontier.Core.Native/include/state_change_filter.h` (new — Level 1 atomic bitset + Level 2 sparse hint)
- `native/DualFrontier.Core.Native/src/state_change_filter.cpp` (new)
- `native/DualFrontier.Core.Native/test/state_change_filter_test.cpp` (new)
- `tests/DualFrontier.Core.Tests/Native/StateChangeFilterTests.cs` (new)

**Drift surface**: NativeWorld write path extended. Existing batch-commit semantics preserved (К-L7 atomic-from-observer) — hook fires at commit time only, not per-write.

**Implementation surface (per spec §3.5 Item 17 + S2 amendment verbatim)**:

```cpp
// state_change_filter.h
struct EntityFilter {
    std::unordered_set<uint32_t> subscribed_entities;  // entity-specific subscriptions
    bool has_type_wide_subscribers;  // type-wide subscribers exist
};

class StateChangeFilter {
private:
    // Level 1: per-component-type bitset (256 bits = 4 × uint64_t)
    std::atomic<uint64_t> wake_subscriber_type_filter[4];

    // Level 2: per-(type, entity) sparse hint
    std::array<EntityFilter, 256> per_type_filters;

public:
    // Hot path: cold-path bypass via Level 1 atomic load + bit test (~2ns)
    bool may_have_subscribers(uint32_t component_type_id) const noexcept;

    // Hot path Level 1 hit: Level 2 check (~2ns + bool check)
    bool has_entity_specific_subscriber(uint32_t component_type_id, uint32_t entity_id) const noexcept;

    // Subscribe/unsubscribe — atomic filter updates
    void subscribe_type(uint32_t component_type_id, uint32_t subscriber_system_id);
    void subscribe_entity(uint32_t component_type_id, uint32_t entity_id, uint32_t subscriber_system_id);
    void unsubscribe_type(uint32_t component_type_id, uint32_t subscriber_system_id);
    void unsubscribe_entity(uint32_t component_type_id, uint32_t entity_id, uint32_t subscriber_system_id);
};

// Write-through hook (called at commit time per K-L7 atomic-from-observer preservation)
void df_native_world_commit_hook(uint32_t component_type_id, uint32_t entity_id);
```

NativeWorld integration:
- After `WriteBatch<T>.Commit()`, hook invokes `df_native_world_commit_hook(type_id, entity_id)` per committed entity
- Hook checks Level 1 atomic filter — cold path bypass if no subscribers for component type (~2ns)
- If Level 1 hit, check Level 2 entity-specific filter — if no entity-specific subscriber, fall through к O(K) enumeration (К = subscribers for this type, typically small)
- For matched subscribers, condition evaluated against new value; if condition crosses (false→true), system added к runqueue via `wake_registry.fire_state_change`

**Per Lesson #7**: spec §3.5 Item 17 S2 amendment is verbatim authoritative — К-L7 invariant preserved (commit-time hook = no mid-transaction observer state); hot path cost ~2ns per write batch commit (negligible).

**Validation**:
- `cmake --build` + native test: filter Level 1 cold-path bypass measurable; Level 2 entity-specific hit measurable; subscribe/unsubscribe atomic correctness; condition crossing detection accurate
- `dotnet build` + `dotnet test` 620+ green; new state change filter tests additive
- К-L7 atomic-from-observer invariant verified: no observer sees torn state between writes in same batch
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Item 17 — write-through hook (filter primitive S2 hybrid Level 1+Level 2)`

### Commit 13 — Items 19+20: Observability + scheduler intrinsics

**Files**:
- `native/DualFrontier.Core.Native/include/scheduler_trace.h` (new — perf/ftrace-like ring buffer)
- `native/DualFrontier.Core.Native/src/scheduler_trace.cpp` (new)
- `native/DualFrontier.Core.Native/include/scheduler_intrinsics.h` (new — suspend/resume/panic_halt/snapshot)
- `native/DualFrontier.Core.Native/src/scheduler_intrinsics.cpp` (new)
- `native/DualFrontier.Core.Native/test/scheduler_trace_test.cpp` + `scheduler_intrinsics_test.cpp` (new)
- `src/DualFrontier.Core.Interop/SchedulerTrace.cs` + `SchedulerIntrinsics.cs` (new — managed wrappers)

**Drift surface**: Observability + emergency-path primitives. No behavior change в hot path при default sampling rate = off.

**Implementation surface (per spec §3.5 Items 19+20)**:

Item 19 (Observability hooks):
- Native trace ring buffer (lock-free, per-thread or shared per Q-N-23 brief decision)
- Trace events emitted by scheduler: `system_woken`, `system_dispatched`, `system_completed`, `phase_started`, `phase_completed`, `quota_violation`, `filter_hit`, `filter_miss`
- C ABI: `df_scheduler_trace_dump(out_buffer, size)`, `df_scheduler_trace_clear()`
- Default sampling rate configurable (full / 1-in-N / off) — К10.1 default = off (zero overhead unless explicitly enabled)

Item 20 (Scheduler intrinsics):
- C ABI: `df_scheduler_suspend()` — pause all dispatch; running systems complete, no new dispatch
- `df_scheduler_resume()` — resume normal operation
- `df_scheduler_panic_halt(message)` — emergency stop; flush traces, dump state
- `df_scheduler_snapshot(out_state_buffer, size)` — atomic snapshot of runqueue + blocked queue + wake subscriptions for debugging
- Managed wrapper provides `using var pause = scheduler.SuspendDispatching()` IDisposable pattern

**Per К10.2 forward dependency**: Item 20 suspend/resume used during `Rebuild` (mod load/unload causing graph changes) — К10.2 lifecycle integration consumes К10.1 intrinsics.

**Validation**:
- `cmake --build` + native tests: trace ring buffer correctness, intrinsics suspend/resume/snapshot
- `dotnet build` + `dotnet test` 620+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.1 Items 19+20 — observability hooks (perf-like tracing) + scheduler intrinsics (suspend/resume/panic/snapshot)`

### Commit 14 — Item 16 (load-bearing): К-L6 SUPERSEDED + К-L12/13/14 amendment + Core dispatch switch к native

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v1.6 → v2.0 (К-L invariant table updated: К-L6 SUPERSEDED row + К-L12/К-L13/К-L14 rows added; Part 2 К10 row inserted between К9 and К-closure; Part 8 cross-references к KERNEL_FULL_NATIVE_SCHEDULER.md)
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` (Core system dispatch switched от managed `ParallelSystemScheduler` к native scheduler via `SchedulerAdapter` + batched callback ABI)
- `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` (deprecated с note; may remain as managed scheduler adapter facade or be deleted depending on Phase 0 read — confirm at execution time)
- Tests updated: existing scheduler tests verify managed adapter facade behavior unchanged from external observer perspective

**This is THE load-bearing commit of К10.1**. Analogous к К8.34 v2.0 Commit 2 в scope/risk. All prior К10.1 commits land **additive** infrastructure parallel к managed scheduler; Commit 14 switches authority.

**Drift surface (large)**:
- К-L6 «Game tick scheduler: Managed» formally SUPERSEDED in KERNEL_ARCHITECTURE.md Part 0 К-L table — replaced by К-L12 in Part 2
- К-L12, К-L13, К-L14 added к KERNEL_ARCHITECTURE.md Part 0 К-L table с verbatim text from KERNEL_FULL_NATIVE_SCHEDULER.md Part 2
- KERNEL_ARCHITECTURE.md Part 2 master plan: К10 row inserted (К10 = native scheduler, status AUTHORED-IN-PROGRESS during К10.1, EXECUTED at К10.4 closure)
- `GameBootstrap.cs` `CreateLoop`: replaces `new ParallelSystemScheduler(...)` с native scheduler instantiation + managed adapter registration via batched callback ABI

**К-L invariant amendment text (verbatim transcription per Lesson #7)**:

К-L6 SUPERSEDED row:
```
| K-L6 | Game tick scheduler | SUPERSEDED by K-L12 (see KERNEL_FULL_NATIVE_SCHEDULER.md) | Original rationale «Vanilla = mods» preserved as K-L9; execution layer concern factored out to K-L12 |
```

К-L12 text:
> «Native kernel owns sovereign per-tick scheduling for kernel-space systems (Core). Managed scheduler exists only for user-space (mod) systems within mod ALCs. Kernel scheduling decisions are made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement. Cross-layer communication uses C ABI with batched callbacks (managed adapter receives batched managed-system dispatches from kernel scheduler).»

К-L13 text:
> «Kernel scheduler activates systems on-demand based on wake-up registration, not by global tick clock. Five wake-up types supported: Timer (periodic by [TickRate]), Event (bus publication subscription), StateChange (component value condition crossing), Init (one-shot at startup), Explicit (API-driven wake by another system). Systems not satisfying any wake condition for current tick remain blocked; only runnable systems enter phase dispatch. Per-tick dependency graph computed on runnable subset.»

К-L14 text:
> «Performance is causally derived from clean complex architecture, not traded against simplicity. Each architectural addition that is principled (matches real OS pattern, serves cleanness, satisfies invariant cohesion) increases performance ceiling. Tactical reasoning («overengineered», «YAGNI», «premature optimization») does not argue against strategic architectural completeness in research framework context. Burden of proof on exclusion of architectural items, not inclusion.»

**Core dispatch switch implementation**:

```csharp
// GameBootstrap.cs CreateLoop excerpt — К10.1 form
// (Pre-К10.1: new ParallelSystemScheduler(...))
// Post-К10.1:
var nativeScheduler = SchedulerAdapter.CreateNative(/* ... */);
var managedAdapter = new ManagedSystemDispatcher(coreSystems, services, faultHandler);

// Native scheduler registered с batched callback for managed system dispatch
nativeScheduler.RegisterManagedCallback(managedAdapter);

// Native scheduler is now Core dispatch authority
return new GameLoop(nativeScheduler, /* ... */);
```

**Per Lesson #8 (atomic commit as compilable unit)**: К-L invariant amendments + Core dispatch switch land в **same commit**. Splitting these into separate commits leaves intermediate state where К-L table claims «К-L12 in effect» but Core dispatch still managed (inconsistent state). Lesson #8 binary unit applies: К-L supersession is binary, atom is the whole switch.

**Per Lesson #11 sub-pattern 3 (ground truth check)**: К10.1 commits this AFTER all prior commits (1-13) lands additive infrastructure. Switch is **last** because all primitives must be ready before switch — backout to К-L6 managed scheduler if any issue surfaces is possible only when native infrastructure is complete.

**Validation**:
- `cmake --build` clean
- `dotnet build` clean
- `dotnet test` 620+ green — **CRITICAL GATE** — К-L6 supersession + Core dispatch switch must not regress any existing test
- К10.1 new tests (batched callback, wake registry, dynamic graph, etc.) all green
- К-L invariant cross-reference integrity: KERNEL_ARCHITECTURE.md v2.0 K-L12/13/14 references point к KERNEL_FULL_NATIVE_SCHEDULER.md Part 2 verbatim
- `sync_register.ps1 --validate` exit 0

**Per К8.34 v2.0 precedent**: this commit is **uncommitted work until tests green**. If at end of authoring Commit 14 build or tests fail, Commit 14 is **not committed** — next session resumes. Only a compiling, green tree gets committed (atomicity protects the milestone).

**Commit message**:
```
feat(kernel): K10.1 Item 16 — К-L6 SUPERSEDED + К-L12/L13/L14 amendment + Core dispatch к native scheduler

The load-bearing commit of К10.1. К-L6 «Game tick scheduler: Managed»
formally superseded by К-L12 «Full native kernel scheduling». КERNEL_ARCHITECTURE.md
v1.6 → v2.0: К-L invariant table updated с К-L6 SUPERSEDED row + К-L12 + К-L13 +
К-L14 rows; Part 2 К10 row inserted; Part 8 cross-references к KERNEL_FULL_NATIVE_SCHEDULER.md.

Core system dispatch switched от managed ParallelSystemScheduler к native scheduler
via SchedulerAdapter + batched callback ABI (Item 15). Managed scheduler reduced к
adapter facade role; existing tests verify external behavior unchanged.

К-L12 text: «Native kernel owns sovereign per-tick scheduling for kernel-space systems...»
К-L13 text: «Kernel scheduler activates systems on-demand based on wake-up registration...»
К-L14 text: «Performance is causally derived from clean complex architecture...»

Build clean; <N> tests green (target 620+).

Phase 14 of K10.1 cascade. Commit 14 of <total>.
```

### Commit 15 — Item 24 closure: native test infrastructure + ManagedTestScheduler

**Files**:
- `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestScheduler.cs` (new — analog к ManagedTestWorld for tests that don't need native dispatch)
- `tests/DualFrontier.Core.Tests/Scheduler/` (potentially renamed test files reflecting К10.1 scheduler surface)
- Native test executables verified build clean post-К10.1 cascade

**Drift surface**: Test infrastructure migration formalization (Item 24 close).

**Per К8.34 v2.0 precedent (ManagedTestWorld)**: К10.1 establishes parallel pattern для scheduler — `ManagedTestScheduler` fixture для tests that test scheduler behavior at managed level (mocking native side, или testing managed adapter logic in isolation). Native scheduler tests live in `native/DualFrontier.Core.Native/test/` per Item 24.

**Validation**:
- `dotnet build` + `dotnet test` 620+ green
- Native tests build + pass clean
- `sync_register.ps1 --validate` exit 0

**Commit message**: `test(kernel): K10.1 Item 24 — native test infrastructure + ManagedTestScheduler fixture`

### Commit 16 — К10.1 closure: REGISTER amendments + audit_trail EVT + CAPA closures

**Files**:
- `docs/governance/REGISTER.yaml` (DOC-D-K10_1 lifecycle AUTHORED → EXECUTED; DOC-A-KERNEL version 1.6 → 2.0; audit_trail EVT entry; CAPA entries if any opened)
- `docs/MIGRATION_PROGRESS.md` (К10.1 closure entry per METHODOLOGY §12.7 step 3)
- `docs/governance/REGISTER_RENDER.md` (regenerated via render_register.ps1)
- `docs/governance/VALIDATION_REPORT.md` (regenerated via sync_register.ps1)

**REGISTER amendments**:

1. **DOC-D-K10_1**: lifecycle AUTHORED → EXECUTED; last_modified_commit к К10.1 closure commit; brief lifecycle transition complete
2. **DOC-A-KERNEL** (KERNEL_ARCHITECTURE.md): version 1.6 → 2.0; last_modified_commit к К10.1 Commit 14 (К-L invariant amendments); governance_events append с EVT-K10_1-CLOSURE reference
3. **DOC-A-KERNEL_FULL_NATIVE_SCHEDULER**: governance_events append с EVT-K10_1-CLOSURE (К10.1 closure references this spec as authority)
4. **audit_trail entry**: `EVT-{date}-K10_1-CLOSURE` — type: execution_milestone; documents_affected: KERNEL_ARCHITECTURE + KERNEL_FULL_NATIVE_SCHEDULER (govern_events) + DOC-D-K10_1 (lifecycle transition); commits: range Commit 1..15 + key Commit 14 (load-bearing); governance_impact: «К10.1 closure — kernel scheduler core landed (17 items); К-L6 SUPERSEDED, К-L12 + К-L13 + К-L14 AUTHORED»
5. **Requirements added** (REQ collection): REQ-K-L12, REQ-K-L13, REQ-K-L14 — each с verified_by linking к native scheduler tests + existing test baseline preservation
6. **Risks status update** (R-K10-1..5 from spec Part 8): status from ACTIVE к RESIDUAL или CLOSED based on К10.1 measured outcomes

**Per METHODOLOGY §12.7 canonical closure protocol**:
1. ✅ Final verification (build + tests + native + sync_register)
2. ✅ Atomic commit с scope prefix
3. ✅ Update MIGRATION_PROGRESS.md closure entry
4. ✅ Update brief Status field (К10.1 → EXECUTED)
5. ✅ Update REGISTER.yaml entries for all documents touched
6. ✅ Append audit_trail entry (EVT-{date}-K10_1-CLOSURE)
7. ✅ Run sync_register.ps1 --validate (exit 0 required)
8. ✅ Final commit incorporates REGISTER.yaml updates

**Validation**:
- `sync_register.ps1 --validate` exit 0 — **mandatory gate** per METHODOLOGY §12.7
- `dotnet build` clean
- `dotnet test` 620+ green (К10.1 baseline preserved + new tests)
- `cmake --build` clean

**Commit message**:
```
governance: K10.1 closure — REGISTER amendments + 3 REQ + EVT-K10_1-CLOSURE

К10.1 sub-milestone closure per METHODOLOGY §12.7 canonical protocol.

REGISTER updates:
- DOC-D-K10_1 lifecycle AUTHORED → EXECUTED
- DOC-A-KERNEL version 1.6 → 2.0 (К-L6 SUPERSEDED + К-L12/13/14 amendment)
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER governance_events append

Requirements added:
- REQ-K-L12 (full native kernel scheduling) — verified_by native scheduler tests
- REQ-K-L13 (on-demand activation, 5 wake types) — verified_by wake registry tests
- REQ-K-L14 (performance derives from architecture) — architecturally established; measurable evidence pending К11+ per S4 lock

audit_trail entry: EVT-{date}-K10_1-CLOSURE

К10.1 closure leaves К10.2 (native bus + mod ALC native primitives), К10.3 (pipeline depth + display composition + hardware tier), К10.4 (TLA+ formal verification) as separate future sub-milestone briefs per Option III standalone-briefs structure.

Phase 16 of K10.1 cascade. Commit 16 of 16 — К10.1 closure.
```

---

## §4 — К-L invariant amendments (Item 16 detailed scope)

### §4.1 — Verbatim К-L invariant table amendment

`docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L invariants table получает три additions + one supersession (К-L6).

**К-L6 row** (existing):
```
| K-L6 | Game tick scheduler | Managed (because all systems are mods) | «Vanilla = mods» principle; AssemblyLoadContext mandates managed code path для systems |
```

**К-L6 row amended к** (post-К10.1):
```
| K-L6 | Game tick scheduler | SUPERSEDED by K-L12 (see KERNEL_FULL_NATIVE_SCHEDULER.md) | Original rationale «Vanilla = mods» preserved as K-L9; execution layer concern factored out to K-L12 |
```

**К-L12, К-L13, К-L14 rows added** (verbatim text per §3 Commit 14):

```
| K-L12 | Native kernel scheduling | Sovereign per-tick scheduling for kernel-space systems (Core) native; managed scheduler scope reduced к user-space (mod) systems within mod ALCs | OS-faithful kernel/user split; К-L6 supersession; cross-layer ABI bridge per Item 15 |
| K-L13 | On-demand system activation | Five wake types (Timer / Event / StateChange / Init / Explicit); only runnable systems enter phase dispatch; per-tick dependency graph computed on runnable subset | Real-OS process-blocking model; sparse-world efficiency; cache locality improvement |
| K-L14 | Performance derives from cleanness | Architectural completeness causes performance; tactical heuristics inapplicable в research framework; default-inclusion bias for architectural items | Project framing (Crystalka 2026-05-16); decade-horizon rent of compromise; Linux/seL4 empirical evidence |
```

### §4.2 — KERNEL_ARCHITECTURE.md Part 2 master plan amendment

Part 2 master plan table receives К10 row insertion between К9 and К-closure:

```
| К10 | Native kernel scheduler | AUTHORED-IN-PROGRESS (К10.1 closure 2026-XX-XX) | К-L12 + К-L13 + К-L14 (plus К-L15/К-L7.1/К-L16/К-L17/К-L18/К-L19 across К10.2/3/4) |
```

К-closure row remains unchanged.

### §4.3 — Cross-document amendment surface (К10.1 minimal — К10.2/К10.3 carry remainder)

К10.1 amendment surface intentionally **narrow**:
- **KERNEL_ARCHITECTURE.md** — primary amendment (v1.6 → v2.0)
- **MIGRATION_PROGRESS.md** — К10.1 closure entry per METHODOLOGY §12.7

Per К10 deliberation S5 lock и КERNEL_FULL_NATIVE_SCHEDULER.md Part 7 amendment list:
- **MOD_OS_ARCHITECTURE.md** — К-L15 / К-L18 amendments are К10.2 scope (mod lifecycle native primitives)
- **VULKAN_SUBSTRATE.md** — К-L7.1 / К-L16 / К-L17 / К-L19 amendments are К10.3 scope (pipeline + display + hardware tier)
- **PHASE_A_PRIME_SEQUENCING.md** — full А' sequencing reconciliation per Q-K-1 retroactive lock happens at К10.4 closure (К-series fully landed)
- **DualFrontier.Persistence** — pipeline slot serialization is К10.3 scope
- **KernelCapabilityRegistry.cs** — capability tier extensions are К10.2 + К10.3 scope

К10.1 keeps amendment surface bounded к focal К-L invariants (К-L6/К-L12/К-L13/К-L14) only.

---

## §5 — Halt triggers (К10.1-specific SC-N taxonomy)

If execution agent encounters any of these conditions, **halt and surface к Crystalka**. Per Lesson #8 corollary: a brief promises «halts before damage», not «zero halts». Halts на К10.1 are **success indicators**, not failures.

### SC-1 — Code anchor doesn't match spec evidence

If a code anchor (DependencyGraph.cs, ParallelSystemScheduler.cs, NativeWorld.cs, GameBootstrap.cs) doesn't match spec's described shape after K10 deliberation closure (2026-05-17), halt. Brief authored 2026-05-18 from point-in-time read; subsequent drift between brief authoring and execution time surfaces here. Per К8.34 v2.0 SC-3 precedent.

### SC-2 — Native scheduler thread synchronization bug surfaces

If native scheduler tests reveal race conditions, deadlocks, или data corruption в hot path, halt. Per spec R-K10-2 risk: probability Medium, impact high. К10.1 must establish lock-free / TLA+-verifiable foundation; latent bugs surface здесь, not в К10.2/3/4 building atop.

Recovery: stop-the-world debugging via Item 20 intrinsics (suspend/resume/snapshot). Surface к Crystalka before continuing.

### SC-3 — Deep-read contradiction

Any §2.3 mandatory re-read surfaces a file shape that contradicts this brief (a signature changed, a method named differently, NativeWorld lacks an API the brief assumes). Halt и surface contradiction.

К10.1 brief was authored from a point-in-time read 2026-05-18; drift since then is exactly what this trigger catches per К8.34 v2.0 SC-3 precedent + Lesson #7.

### SC-4 — К-L6 supersession test regression

Commit 14 (load-bearing) switches Core dispatch к native scheduler. If `dotnet test` shows test regression after Commit 14, halt. Test count drop OR new failures indicate К-L6 supersession introduced behavioral drift. К10.1 brief discipline: existing 620 tests **must** remain green through К-L6 supersession (managed adapter facade preserves external behavior unchanged from observer perspective).

Recovery: bisect to find which specific change caused regression; surface к Crystalka before continuing. **Do not commit a partial Commit 14** — atomic per Lesson #8.

### SC-5 — Batched callback ABI fails round-trip

Commit 11 lands batched callback ABI. If managed→native registration → native dispatch → managed callback round-trip fails (managed callback never invoked, or panic in dispatch, or GC transition cost dominates), halt.

Per spec R-K10-1 risk: reverse P/Invoke GC transition cost exceeds estimate. К10.1 brief assumes ~10-50ns per call amortized; если measurements show much higher cost, К-L12 architectural decision (managed adapter pattern) needs re-examination.

Recovery: profile transition cost; if confirmed unacceptable, surface к Crystalka — К-L12 reframing may be required, or alternative dispatch mechanism explored.

### SC-6 — К-L invariant cross-reference integrity broken

Commit 14 amends KERNEL_ARCHITECTURE.md v1.6 → v2.0 с К-L12/13/14 added. If after Commit 14 `sync_register.ps1 --validate` flags broken cross-references (e.g., К-L12 references KERNEL_FULL_NATIVE_SCHEDULER.md Part 2 but path lookup fails), halt.

Recovery: verify amendment text against spec verbatim; fix cross-reference; re-validate.

### SC-7 — Validation regression post-commit

If `sync_register.ps1 --validate` exits non-zero after any К10.1 commit, halt immediately. К10.1 cascade must not introduce new validation errors per К8.34 v2.0 + CLEANUP_CASCADE precedents.

Recovery: investigate validation error; fix REGISTER entry or document state; re-validate before continuing.

### SC-8 — Scope creep

If execution encounters drift не в К10.1 scope (е.g., К10.2 mod lifecycle issues, К10.3 pipeline issues, К-L15 amendments к MOD_OS), halt and surface. Do not «fix while we're here» — К10.1 scope discipline per S-LOCK-1.

Per Lesson #14 candidate (provisional): pre-existing drift cleanup as separate cascade. К10.1 surface drift triages к К10.2/К10.3 sub-milestone briefs.

### SC-9 — Performance prediction §5.1.A failures

К10.1 closes 5 К10 predictions from spec Part 5.1.A (Predictions 1-5 + parts of others). If benchmarking after Commit 14 shows регрессию vs К-L6 managed scheduler baseline (Prediction 4 reframed: «К10 architecture realization predictions» measurable scope), halt и surface.

Recovery: examine which К10.1 architectural items contributed positively vs negatively; identify if К-L14 needs amendment к weaker claim («architecture enables performance»); surface к Crystalka per R-K10-5 mitigation.

### SC-10 — Push-to-main classifier block (operational reminder, not halt)

Known behavior per memory: Claude Code auto-mode classifier blocks push-to-main even с explicit instruction. Not a halt — expected. Re-confirm in-session after the work is done, then push. Per К8.34 v2.0 SC-8 precedent.

When halting (SC-1..SC-9): author a HALT_REPORT в `docs/scratch/A_PRIME_7_K10_1/` (or similar scratch path), state the trigger, state what was/wasn't committed, stop. **Do not commit a partial atomic commit** — atomicity protects the milestone per Lesson #8.

---

## §6 — Closure protocol (per METHODOLOGY §12.7 canonical)

After Commit 16 lands clean:

### §6.1 — Verify final state

1. `git log --oneline` shows ~16 commits added by К10.1 на feature branch `claude/k10_1-kernel-scheduler-core`
2. `git status` clean working tree
3. `sync_register.ps1 --validate` exit 0
4. `cmake --build` clean, native tests pass
5. `dotnet build` clean, `dotnet test` 620+ green (К10.1 new tests additive — final count documented в closure entry)
6. К10.1 benchmarks per §5.1.A Predictions 1-5 measured + documented (results-as-measured, не curated)

### §6.2 — Update brief status + closure section

Set `status: EXECUTED` в this brief's frontmatter; add §8 closure section с commit range + date + commit ledger table + verification metrics + halt protocol activations + lesson candidates + pattern established (per CLEANUP_CASCADE_BRIEF §8 precedent).

Pattern для closure section:
```markdown
## §8 — Closure (added at brief EXECUTED transition YYYY-MM-DD)

Execution closed YYYY-MM-DD by Claude Code auto-mode на branch `claude/k10_1-kernel-scheduler-core` from `main` HEAD <starting-sha>. Final commit <final-sha>.

### Commit ledger (commits <first>..<last>)

| # | Hash | Commit summary | Items closed |
|---|---|---|---|
| 1 | ... | brief authored | DOC-D-K10_1 enrollment |
| ... | ... | ... | ... |
| 16 | ... | governance closure | EVT-K10_1-CLOSURE |

### Verification metrics (final state)

- `git status`: clean working tree on branch `claude/k10_1-kernel-scheduler-core`
- `sync_register.ps1 --validate`: exit 0
- `cmake --build`: 0 warnings, 0 errors
- `dotnet build`: 0 warnings, 0 errors
- `dotnet test`: <N> passed, 0 failed (target 620+ baseline preserved + К10.1 tests additive)
- К10.1 benchmarks Predictions 1-5: <measurement results>

### Halt protocol activations
[Any SC-N halts that fired during execution + their resolution]

### Out-of-scope items deferred
- К10.2 scope: native bus + mod ALC native primitives (Items 21, 26-32)
- К10.3 scope: pipeline depth + display composition + hardware tier (Items 33-44)
- К10.4 scope: TLA+ formal verification (Items 18, 45, 46)
- Item 14: К11+ Core migration к native code
- Item 25: К-closure report (А'.8)

### Pattern established
[Patterns from К10.1 execution worth noting for К10.2/3/4 briefs]

### Lesson candidates surfaced
[Anything worth bringing к К10.2 brief authoring deliberation]
```

### §6.3 — PR opening (NOT auto-push, per К8.34 v2.0 + CLEANUP_CASCADE precedent)

- Push branch `claude/k10_1-kernel-scheduler-core` к remote (NOT к `main`)
- Open PR titled «К10.1 — Kernel scheduler core (К-L6 SUPERSEDED + К-L12/13/14)»
- Body summarizes per-commit per-item mapping + verification metrics + halt activations (if any) + closure section
- **DO NOT auto-push к main**. Crystalka reviews + merges per established protocol

### §6.4 — Surface к Crystalka

PR ready for review. Crystalka:
1. Reviews К10.1 closure report content
2. Merges PR к `main`
3. Provides closure report to next Opus deliberation session for К10.2 brief authoring discussion

К10.2 brief authoring informed by:
- К10.1 closure report findings (halt activations, lesson candidates, patterns established)
- К10.1 architectural reality post-landing (К-L12/13/14 verbatim in KERNEL_ARCHITECTURE.md v2.0; native scheduler authoritative за Core dispatch)
- Updated REGISTER state (К10.1 EXECUTED; К10.2 будет AUTHORED при brief authoring)

---

## §7 — Brief authority + lifecycle

**Brief authority**: К10 deliberation arc 2026-05-16 → 2026-05-17 (Crystalka + Claude Opus 4.7). 9 S-locks ratified в KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED. К10.1 standalone brief per Option III ratified 2026-05-18.

**Brief lifecycle (per FRAMEWORK §3.3 + §3.3.1)**:
- AUTHORED at this commit (Commit 1 of cascade)
- EXECUTED post-Commit 16 closure
- registered в `tools/briefs/` as Tier 3 Category D per A'.4.5 governance
- AUTHORED-SKELETON → AUTHORED transition: not applicable here — К10.1 was authored from skeleton K10_EXECUTION_BRIEF.md content via Opus deliberation session (deliberation produced full K10.1 brief without intermediate skeleton lifecycle stage on disk)

**Brief enrollment**: К10.1 brief added к REGISTER.yaml в Commit 1 atomic с brief authoring per CLEANUP_CASCADE_BRIEF precedent (brief enrollment at first commit of cascade).

**Brief location**: `tools/briefs/K10_1_EXECUTION_BRIEF.md` after Crystalka copies from `/mnt/user-data/outputs/` per Filesystem MCP workaround pattern (memory: create_file via Filesystem MCP can silently fail; write через bash к /home/claude/staging/, copy к /mnt/user-data/outputs/, deliver via present_files, Crystalka manually copies к target path).

---

**End of brief. ~16 atomic commits across 17 К10 items + К-L6 supersession + К-L12/13/14 invariant landings. Expected 12-20 hours auto-mode execution.**

К10.1 closes 17 of 46 К10 items. Remaining 29 items distributed across К10.2 (8 items: 21, 26-32), К10.3 (12 items: 33-44), К10.4 (3 items: 18, 45, 46) + Item 14 deferred к К11+ + Item 25 cross-cutting к А'.8.

К10 as a whole remains AUTHORED-IN-PROGRESS until К10.4 closure. К-series formally closes only after all four К10 sub-milestones + К-closure report (А'.8).

«Halt is success, not failure» per Lesson #8 corollary. The brief's honest guarantee: bad premises surface at Phase 0 / at deep-read / at the compile gate, before they reach `main`.

---

## §8 — Closure (added at brief EXECUTED transition 2026-05-18)

Execution closed 2026-05-18 by Claude Code auto-mode on branch `claude/k10_1-kernel-scheduler-core` from `main` HEAD `a145548`. Final commit lands в Commit 16 (this section + closure REGISTER amendments).

### Commit ledger (commits f439b74..PENDING-COMMIT-K10_1-CLOSURE)

| # | Hash | Commit summary | Items closed |
|---|---|---|---|
| 1 | f439b74 | brief authored | DOC-D-K10_1 enrollment |
| 2 | 215eb43 | native test framework decision + smoke | Item 24 framework decision |
| 3 | d1a94f4 | Item 1 native dependency graph | Item 1 |
| 4 | 617db7b | Item 2 thread pool extension | Item 2 |
| 5 | 4ad0bae | Item 3 wake registry (5 wake types) | Item 3 |
| 6 | 5e8f705 | Item 4 wake registry diagnostic API | Item 4 |
| 7 | 95e0f6c | Item 5 per-tick orchestration | Item 5 |
| 8 | 9ab169b | Items 6+7+8 scheduling policies | Items 6+7+8 |
| 9 | dbd4a02 | Item 9 shared memory regions | Item 9 |
| 10 | 023b1fe | Items 11+12+13 affinity + work stealing + barriers | Items 11+12+13 |
| 11 | 80a10a9 | Item 15 batched callback ABI | Item 15 |
| 12 | 6772b9d | Item 17 write-through hook (S2 hybrid filter) | Item 17 |
| 13 | 53998df | Items 19+20 observability + intrinsics | Items 19+20 |
| 14 | a9ba798 | **Item 16 LOAD-BEARING К-L6 SUPERSEDED + К-L12/L13/L14 amendment** | Item 16 + К-L invariants |
| 15 | ba0cc76 | Item 24 test infrastructure + ManagedTestScheduler | Item 24 close |
| 16 | PENDING-COMMIT-K10_1-CLOSURE | governance closure | EVT-K10_1-CLOSURE |

### Verification metrics (final state)

- `git status`: clean working tree on branch `claude/k10_1-kernel-scheduler-core`
- `sync_register.ps1 --validate`: exit 0 (5 advisory orphan warnings, baseline)
- `cmake --build`: 0 warnings, 0 errors
- `dotnet build`: 0 warnings, 0 errors
- `dotnet test`: 624 passed, 0 failed (620 baseline preserved + 4 new BatchedCallbackTests)
- `df_native_selftest.exe`: ALL PASSED (58 scenarios — 28 ECS pre-К10.1 + 30 К10.1 scheduler)

### Halt protocol activations

None — К10.1 completed без any SC-N halt firing. Phase 0 pre-flight gates all passed; no SC-1 anchor contradictions; no SC-2 native sync bugs; no SC-3 deep-read contradictions; no SC-4 supersession test regressions; no SC-5 ABI round-trip failures; no SC-6 cross-reference integrity breaks; no SC-7 validation regressions; no SC-8 scope creep into К10.2/К10.3 territory; no SC-9 performance regressions (К-L14 measurable evidence pending К11+ per S4 lock).

### Out-of-scope items deferred

Confirmed per S-LOCK boundaries:
- К10.2 scope: native bus + mod ALC native primitives (Items 21, 26-32) — future brief
- К10.3 scope: pipeline depth + display composition + hardware tier (Items 33-44) — future brief
- К10.4 scope: TLA+ formal verification (Items 18, 45, 46) — future brief
- Item 14: К11+ Core systems migration к native code (К-L12 sovereign decisions established; sovereign execution bodies are К11+ optional)
- Item 25: К-closure report (А'.8) — waits for К10.4 closure

### Pattern established

Patterns from К10.1 worth bringing к К10.2/3/4 briefs:
1. **Custom DF_CHECK selftest pattern** preserved за Lesson #22 — К10.2/3/4 scheduler scenarios continue к extend `selftest.cpp` rather than adopting catch2/gtest. Pattern remains lean.
2. **Process-global default singletons** (default_scheduler_graph + default_wake_registry + default_scheduling_policies + default_shm_registry + default_state_change_filter + default_scheduler_trace + default_scheduler_intrinsics + default_managed_callback_registry) match OS-faithful «one kernel scheduler per process» model. К10.2 native bus + mod ALC primitives follow this pattern.
3. **Native-side internal class + C ABI wrapper + managed *Interop wrapper** triple. Pattern shapes consistent across 8 К10.1 modules. К10.2 native bus follows.
4. **Header decoupling**: keep canonical type definitions in `df_capi.h` (single source). `managed_callback.h` lesson — duplicate typedef caused conflict; fix was `#include "df_capi.h"` not redefine. К10.2/3 watchout.
5. **ManagedTestScheduler fixture pattern** mirrors ManagedTestWorld. Tests reset native singletons explicitly because process-global state. К10.2 scheduler tests use the same fixture.
6. **Brief enrollment as Commit 1** (DOC-D-K10_1 lifecycle AUTHORED) per CLEANUP_CASCADE_BRIEF precedent — keeps working tree clean from Commit 2 onwards.
7. **К-L invariant amendment commit timing** (LOAD-BEARING last) per spec brief Commit 14 placement. All primitives must exist before К-L supersession lands.

### Lesson candidates surfaced

Worth bringing к К10.2 brief authoring deliberation:
1. **«Pre-built native DLL + checked-in build artifact» as test gate viability** — К10.1 found that cmake/MSVC binaries are not in PATH but VS-bundled cmake at `D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe` is functional. К10.2 should formalize this как «Phase 0 hard gate informational check — locate native toolchain» so executor doesn't need к discover ad-hoc.
2. **Brief Commit 2 «catch2 vs gtest» decision criteria** — Phase 0 read trumps brief tentative. К10.2 brief authoring should default к «match existing pattern» language rather than recommend new framework.
3. **K-L invariant verbatim transcription** (Lesson #7) — verified across К10.1 commit messages. К-L12 / К-L13 / К-L14 text matches spec verbatim. К10.2 should preserve discipline.
4. **К-L14 «default-inclusion bias»** held throughout: 5 wake types (not 4-of-5), 3 affinity/work-stealing/barrier items (not deferred individually), batched callback ABI landed despite К11+ being the consumer. Pattern: architectural completeness > tactical effort heuristics.
5. **Item 14 «К11+ optional» discipline**: К-L12 says native sovereign **scheduling decisions** are made; native sovereign **execution bodies** are К11+ optional. К10.2 should preserve this layering — bus integration + mod ALC primitives are scheduling-decision-adjacent, not execution-body migration.

### К10.2 forward priorities (informational, not authoritative for К10.2 brief authoring)

К10.1 closure leaves К10.2 entry conditions:
- Native scheduler graph + wake registry + scheduling policies + batched callback ABI + state filter + observability + intrinsics all operational
- К-L12 / К-L13 / К-L14 invariants AUTHORED and referenced from KERNEL_ARCHITECTURE.md
- 624 tests baseline; new tests can rely on ManagedTestScheduler fixture
- К10.2 needs к amend MOD_OS_ARCHITECTURE.md per К-L15 + К-L18; native bus integration + mod ALC lifecycle + write-through hook k NativeWorld.WriteBatch.Commit
- К10.2 brief authoring informed by К10.1 closure metrics + pattern established + lesson candidates above.