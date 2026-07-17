---
register_id: DOC-A-EXECUTION_AUTHORITY_MATRIX
project: Dual Frontier
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: 0.1.0
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: post-ratification closure
title: Execution Authority Matrix — sole-authority map, cutover gates, deletion triggers (A0 draft)
last_modified_commit: 8960085
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Tier 1 AUTHORED override (forbidden pair; precedent DOC-A-K_CLOSURE_REPORT): authored-proposal draft of the missing A0 cross-cutting contract per ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715 §7. Content is architecture-contract law (FRAMEWORK §3.4 hierarchy → Tier 1); lifecycle AUTHORED because unratified — mandatory preamble marks it normative-target, NOT current truth; conflicts resolve in favor of LOCKED docs until Crystalka ratification per FRAMEWORK §7.'
---

# Execution Authority Matrix (the A0 contract)

> **Document class: authored-proposal (normative-target). NOT current truth, NOT enforceable law.** Produced by the Architecture Decomposition & Contracts session 2026-07-15 ([docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)). Becomes normative only upon Crystalka ratification per FRAMEWORK.md §7. Until then no document may cite it as authority; conflicts resolve in favor of existing LOCKED docs.

**Ratification path** (per FRAMEWORK.md §7.2 Tier 1 amendment protocol; each destination bumps per its own amendment protocol):

| This document's section | Folds into, on ratification |
|---|---|
| §2 The matrix | [ARCHITECTURE.md](./ARCHITECTURE.md) — the umbrella gains a normative "who decides what" section (MINOR: additive) |
| §3 Cutover gates + deletion triggers | [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) — Part 0 annotations on the K-L12 / K-L15 rows + a chronicle entry (the gates are kernel-side law) |
| §4 Precedence rule | [FRAMEWORK.md](../governance/FRAMEWORK.md) — a §14.7 amendment (governance law, not architecture; §14.7 self-declares "a change to it is an amendment of this section") |
| §1 definitions, §5 observability, §6 open questions | Remain standalone here; this document then transitions authored-proposal → AUTHORED → LOCKED |

Baseline for every "today" claim: working tree at HEAD `6f39903`, 2026-07-15.

---

## 0. Why this document exists

The engine currently runs **two authority planes for the same decisions**:

- **Scheduling.** K-L12 (LOCKED) claims "Kernel scheduling decisions are made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement" (KERNEL_ARCHITECTURE.md Part 0, line 61). In production, at HEAD `6f39903`:
  - phases are built by the managed `DependencyGraph` (`src/DualFrontier.Application/Loop/GameBootstrap.cs:145-148`) and executed via `Parallel.ForEach` (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:149`);
  - the native graph is registered with "empty read/write component-id sets" (THREADING.md line 38; `GameBootstrap.cs:170-176`) and a blanket Timer wake at rate 1 (`GameBootstrap.cs:179`);
  - the batched-callback adapter is on disk but "Production per-phase dispatch does not yet route through it — the switch is `Planned`" (THREADING.md line 43).
- **Event routing.** K-L15 (LOCKED) claims "Native kernel owns sovereign event routing for kernel-space and cross-layer events" (KERNEL_ARCHITECTURE.md Part 0, line 64). In production, at the same HEAD:
  - five managed `DomainEventBus` instances are "the dispatch path production events actually travel today" (EVENT_BUS.md line 17);
  - `BusFacade.UseNativeBusForDispatch` defaults to `false` (`src/DualFrontier.Application/Bus/BusFacade.cs:49`) and "No production code constructs a `BusFacade`" (EVENT_BUS.md line 76);
  - the only live native touchpoints are the Background-tier drain and the (vacuous) per-mod bulk unsubscribe (EVENT_BUS.md line 77).

No existing document names a cutover gate or a deletion trigger for the managed side of either plane. FRAMEWORK.md §14.7 defines *which documents* are authoritative (`lifecycle == Live` OR (`lifecycle == LOCKED` AND `tier ∈ {1, 2}`), FRAMEWORK.md line 813) but no precedence rule *between* them — two surface members that disagree about one domain are unarbitrable today. Two whole domains (persistence, entity identity) have no named owner at all.

This document is the missing contract: one matrix that names the sole authority per decision domain, the honest de-facto owner, the cutover terms for every split row, and the arbitration rule for conflicting documents.

## 1. The rule

**One decision domain → one sole authority → any number of facades and adapters.**

Definitions used by the matrix:

- **Authority** — the component (or document, in §4) that *decides*: it holds the mutable state of record for the domain and its verdicts are final. Two authorities for one domain is a defect, even while they happen to agree — agreement is a runtime accident, not a contract.
- **Facade / adapter** — *translates* between the authority and a consumer: marshals, batches, re-shapes, forwards. A facade may not reorder, veto, filter, or originate decisions. The moment it does any of those, it has silently become a second authority.
- **Declarer** — *states intent* and decides nothing: attributes (`[SystemAccess]`, `[TickRate]`, `[EventTier]`, `[Deferred]`), manifests, capability lists. A declaration is input to exactly one authority. A declaration consumed by two authorities independently — as `[SystemAccess]` is meant to be by both graphs, and `[TickRate]` is by the managed `TickScheduler` today and native TimerWake rates tomorrow (THREADING.md line 86) — is a latent fork: the two consumers can drift in how they interpret the same text.

A component may hold authority in one row and be a facade in another (the kernel is storage authority in R1 but decides nothing about mod load order in R4). What it may never be is both authority and facade *in the same row*.

## 2. The matrix

Column semantics: **Sole authority (target)** is the ratification-target owner. **De-facto owner (today)** is code truth at 2026-07-15 — where the two columns disagree, §3 names the cutover. **Forbidden second authority** names the specific thing that must never (again) decide in this domain.

| # | Decision / state | Sole authority (target) | De-facto owner (2026-07-15 truth) | Facades / adapters | Forbidden second authority |
|---|---|---|---|---|---|
| R1 | Entity / component / field state | Native kernel `NativeWorld` storage (K-L11, KERNEL Part 0 line 60) | **Same** — native since the K8.3+K8.4 cutover 2026-05-14 | `NativeWorld` handle + `SpanLease<T>`/`WriteBatch<T>` (Core.Interop, K-L7); Path β per-mod `ManagedStore` is a declared carve-out, not a fork (K-L3.1) | The managed `World` (already demoted to test fixture `ManagedTestWorld` per K-L11) |
| R2 | System hazards, execution order, wakes (scheduling *decisions*) | Native `SystemGraph` + wake registry (K-L12/K-L13, KERNEL Part 0 lines 61-62) | **Managed `DependencyGraph`** builds the production phases (`GameBootstrap.cs:145-148`); the native graph holds the same systems with empty access sets (`GameBootstrap.cs:170-176`) and Timer-1 wakes for all (`GameBootstrap.cs:179`) | `SchedulerAdapter` + `ManagedSystemDispatcher.OnBatch` (`src/DualFrontier.Application/Scheduler/`, test-exercised only per THREADING.md line 43) | Managed `DependencyGraph` in production planning after the §3.1 cutover |
| R3 | Managed system invocation (dispatch of execution bodies) | Native scheduler dispatching batches over the K-L12 batched-callback ABI | `ParallelSystemScheduler.ExecutePhase` → `Parallel.ForEach`, MaxDoP = N−2 (`ParallelSystemScheduler.cs:149`, `:90`), driven by `GameLoop` | `ParallelSystemScheduler` as pure batch executor (see note below — today it exceeds facade rights) | Any managed component composing phases or arbitrating priority post-cutover |
| R4 | Mod package dependencies & load order | Managed Mod OS: `ModIntegrationPipeline` + `ContractValidator` (`src/DualFrontier.Application/Modding/`; MOD_OS_ARCHITECTURE.md line 135 "topological load order (M5)", line 275 manifest `dependencies`) | **Same** — managed, by design | Native per-mod unload primitive (executes teardown; decides nothing) | The native kernel — K-L12 owns per-tick *system* scheduling, not package ordering |
| R5 | Event routing, subscriptions, queues | Native three-tier bus (K-L15, KERNEL Part 0 line 64) | **Five managed `DomainEventBus` instances** behind `GameServices` (`src/DualFrontier.Core/Bus/`; EVENT_BUS.md line 17); `BusFacade.UseNativeBusForDispatch = false` (`BusFacade.cs:49`), not constructed in production (EVENT_BUS.md line 76) | `BusFacade` (dormant), `ManagedBusBridge` (live only for Background drain), `ModBusRouter` (capability check + routing) | Five domain buses as five independent routers after the §3.2 cutover — they become typed facades over one router |
| R6 | Event delivery invocation (when handlers actually run) | Per K-L15: publisher-thread sync (Fast), phase-boundary batched callbacks (Normal), idle-slot budget drain (Background) | `DomainEventBus.Publish` sync + `IDeferredFlush.FlushDeferred` after each phase barrier (EVENT_BUS.md line 47); the single live native invocation is `GameLoop`'s Background drain each tick (`GameLoop.cs:127`) | `GameServices` aggregator (`IDeferredFlush`) | Two flush drivers for the same queue class; a facade that re-times delivery |
| R7 | Vulkan resources, queues, swapchain | `DualFrontier.Runtime` composition — `Runtime.Create` owns `VulkanInstance`/`VulkanDevice`/`Surface`/`Swapchain` (`src/DualFrontier.Runtime/Runtime.cs:63-89`), K-L19 fail-fast at `Runtime.cs:83` | **Same** — Runtime, per VULKAN_SUBSTRATE.md §2 | `LauncherRenderer` (Launcher); `df_vulkan_unload_mod_resources` K-L18 Step 3.6 placeholder (vacuous success today) | The C++ kernel DLL — `DualFrontier.Core.Native` owns zero GPU objects |
| R8 | Window & input pump | Launcher main thread — `runtime.Window.PumpMessages()` (`src/DualFrontier.Launcher/Program.cs:77`) | **Same**, with an honesty note: input is drained-and-discarded (`Program.cs:81-84`) | `InputQueue` (Runtime) | A second message pump anywhere; the simulation thread touching the window |
| R9 | Engine lifecycle & composition | Proposed: the Application composition root (`GameBootstrap.CreateLoop`, `GameBootstrap.cs:70`) plus a to-be-named shutdown owner | De-facto `Program.Main` + `GameBootstrap.CreateLoop`; **ownerless in doc space** — no document names lifecycle authority | `GameContext` (carries loop + mod controller out of the factory) | A second composition root; subsystems self-composing at static-init time |
| R10 | Simulation time & tick | `GameLoop` fixed-step, `TargetTps = 30f` (`GameLoop.cs:29`) + `TickScheduler` monotonic counter | **Same** — managed; the native side holds no tick counter authority | Render loop reads via `PresentationBridge`; K-L16 slot tail defines the *display's view* of time, not its owner | The render loop ticking the sim (`Program.cs` step-3 comment already forbids); any second tick counter |
| R11 | Identity registries (entity / component-type / event-type / field / mod ids) | One umbrella identity contract, **currently unwritten** | Fragmented across five minting rules (see note below); no umbrella owner | n/a | A second mint for any of the five id spaces |
| R12 | Persistence snapshot (what is saved, when, by whom) | **TBD — explicitly ownerless today.** An owner must be named before any save cascade | Nobody (see note below) | Snapshot records exist unused (`src/DualFrontier.Persistence/Snapshots/`) | Everything, until an owner is named — two snapshot formats emerging independently is the failure this row exists to forbid |
| R13 | Configuration constants | **TBD — ownerless today**; every tunable is hardcoded | Scattered consts (see note below) | None | Per-subsystem config files introduced piecemeal without a named owner |

Reading rule: a row where target = de-facto and an owner is named is **settled** (R1, R4, R7, R8, R10). A row where they differ is **split** and demands §3 (R2, R3, R5, R6). A row marked TBD/ownerless is **vacant** and demands an ownership decision before any feature lands in it (R9, R11, R12, R13).

### 2.1 Row notes (the honest fine print)

- **R2 (split).** The native graph cannot decide anything today: with `ReadOnlySpan<uint>.Empty` reads/writes (`GameBootstrap.cs:173-174`) it has no edges, so THREADING.md line 22's "The same `[SystemAccess]`-derived edges are reused" is vacuously false, and Timer-1-for-everything (`GameBootstrap.cs:179`) reduces K-L13's five wake types to "run everything every tick". KERNEL_FULL_NATIVE_SCHEDULER.md line 17 nonetheless self-reports "native scheduler live" — installed is not deciding.
- **R3 (split).** THREADING.md line 41 names `ParallelSystemScheduler` "the **managed dispatch facade** retained at К10.1 closure", but a facade that consults `TickScheduler.ShouldRun` per system (`ParallelSystemScheduler.cs:149` ff.) is *selecting* the runnable subset — a scheduling decision, K-L13's job. Under §1 definitions it is today a second authority wearing a facade label.
- **R5/R6 (split).** Observability is inverted: the dormant native path carries a full diagnostic ABI while "The managed `DomainEventBus` keeps no counters" (EVENT_BUS.md line 123), and `FastTierContractMonitor` polices a tier with zero production subscribers. The canonical K-L15 text additionally lives in K_CLOSURE_REPORT.md §2.17 (line 571) — a Tier 1 **AUTHORED** document, off the §14.7 surface (see §4 P3).
- **R9 (vacant).** The shutdown half is real debt, not just a paperwork gap: production never calls `df_world_destroy` (the native world falls to the finalizer), `GameLoop.Stop` joins the sim thread with a 2 s timeout and may abandon it mid-tick, and `ModIntegrationPipeline.UnloadAll` has zero production call sites. Naming a lifecycle authority is the precondition for fixing any of that coherently.
- **R11 (vacant).** The five spaces: `EntityId(Index, Version)` minted by `NativeWorld` (`src/DualFrontier.Contracts/Core/EntityId.cs:21`); component-type ids via the explicit `ComponentTypeRegistry` (K-L4, "Sequential IDs (1, 2, 3, ...) deterministic per mod load order", KERNEL line 547); event-type ids minted by FNV-1a in `BusFacade` (EVENT_BUS.md line 76) — despite K-L4's own rationale "FNV-1a hash collision-prone" (KERNEL line 53); field ids as interned strings; mod ids manifest-declared. The version-0 idiom (`new EntityId(indices[i], 0)`, `GameBootstrap.cs:241`; `EntityEncoder.cs:85`) collapses generation validation to "hope the index was never reused".
- **R12 (vacant).** `DualFrontier.Persistence` is referenced by no production assembly (tests only); "SaveSystem currently stub (NotImplementedException)" (`src/DualFrontier.Core.Interop/PipelineSlotInterop.cs:202`); Path β components are "not persisted by save system" (KERNEL line 74); KFNS Item 31 defers background-queue persistence. Meanwhile the kernel's Background queue already ships "save-integrated serialization at the C ABI level (versioned wire format, schema v1)" (EVENT_BUS.md line 59) — a second snapshot format is already latent.
- **R13 (vacant).** The inventory: map 200×200 + seeds + item counts (`GameBootstrap.cs:58-68`), `TargetTps = 30f` (`GameLoop.cs:29`), MaxDoP = N−2 (`ParallelSystemScheduler.cs:90`), priorityClass 2 / wakeType 0 / Timer rate 1 (`GameBootstrap.cs:170-179`), window 1280×720 (`Program.cs:36-37`). No config file or loader exists anywhere in the repo.

### 2.2 What the matrix does NOT decide

Scope boundaries, to prevent this document being over-read:

- It does not choose *implementations* — R2's authority column says the native `SystemGraph` decides, not how Kahn's sort is coded. Implementation truth stays with the subsystem docs (THREADING, EVENT_BUS, VULKAN_SUBSTRATE).
- It does not schedule the cutovers — §3 defines *what must be true*, never *when*. Sequencing is ROADMAP territory ("Architecture documents answer 'what is'; the roadmap alone answers 'what's next'", ARCHITECTURE.md line 95).
- It does not resolve the vacant rows — R9/R11/R12/R13 record that a decision is missing; making it is a deliberation, not an edit to this file.
- It does not govern gameplay-protocol documents (COMBO_RESOLUTION, RESOURCE_MODELS, FEEDBACK_LOOPS) except where they touch a row's domain (e.g. deterministic ordering claims eventually collide with R2/R10).

## 3. Cutover contract for the split rows

### 3.0 What a cutover contract must contain

A split row is tolerable *only* under a contract with three mandatory elements:

1. **Named gate conditions** — checkable predicates, each independently falsifiable;
2. **An equivalence-proof obligation** — the two planes demonstrated equivalent on the *production* workload before the switch, recorded as evidence;
3. **A DELETION TRIGGER** — the event upon which the losing plane leaves production, named in advance.

Today none of the three exist anywhere for R2/R3 or R5/R6. The closest artifacts are a code comment (`GameBootstrap.cs:150-159`) and two `Planned — see docs/ROADMAP.md` pointers (THREADING.md line 43; EVENT_BUS.md line 78) — a roadmap pointer is a direction, not a gate.

### 3.1 Scheduling (R2 + R3): managed `DependencyGraph` → native `SystemGraph`

Gate conditions (all must hold):

- **GATE-S1 — real access sets.** The native registry receives the `[SystemAccess]`-derived component-type ids instead of empty spans (`GameBootstrap.cs:173-174`), so the native graph actually has the edges K-L12 assumes.
  - *Open while:* any production `RegisterSystem` call passes `ReadOnlySpan<uint>.Empty`.
- **GATE-S2 — phase-composition equivalence on the production set.** For the real 10-system Core set (not selftest synthetics), managed `DependencyGraph.GetPhases()` and native `compute_static_graph` + per-tick graphs produce equivalent phase partitions over N ≥ 1000 consecutive ticks. Today the only native-graph exercise is `df_native_selftest` with synthetic systems (`native/DualFrontier.Core.Native/test/selftest.cpp:1034` ff.); no managed-vs-native equivalence test exists under `tests/` (the repo's only "equivalence" suite is GPU diffusion, `V1DiffusionEquivalenceTests.cs`), and the K-L14 evidence dashboard carries bus verification rows but zero scheduler-cutover rows.
  - *Open while:* no test under `tests/` compares the two planes' phase partitions on the production system set.
- **GATE-S3 — dispatch switch.** Production per-phase dispatch routes through `ManagedSystemDispatcher.OnBatch` via `SchedulerAdapter`, with mod-ALC lifecycle context surrounding the call sites (the K10.2 clause, `GameBootstrap.cs:50-54`).
  - *Open while:* `SchedulerAdapter` has zero production call sites (its current state per THREADING.md line 43).
- **GATE-S4 — wake surface used beyond Timer-1.** The `[TickRate]` census maps to real TimerWake rates (not rate 1 for everything), and at least one non-Timer wake type (Event / StateChange / Init / Explicit) carries a production system — otherwise K-L13 is dead law.
  - *Open while:* every production wake subscription is `SubscribeTimer(i, 1)` (`GameBootstrap.cs:179`).

**Equivalence-proof obligation:** an N-tick lockstep harness asserting equal phase partitions and equal per-tick runnable subsets, recorded as a K-L14 evidence entry and as dashboard verification rows (matching the three the bus already has).

**DELETION TRIGGER:** when S1–S4 have held for one full release cycle, the managed `DependencyGraph` **leaves production planning** — removed from `GameBootstrap` (`GameBootstrap.cs:145-148`) and demoted to test oracle or deleted; `ParallelSystemScheduler` either reduces to a pure batch executor (no `ShouldRun` consultation, no phase-list ownership) or is deleted per the K10.1 Item 14 clause it was retained under ("may remain as managed scheduler adapter facade or be deleted", `GameBootstrap.cs:49-52`). The equivalence tests carry a second-order deletion gate: once the managed planner is gone they compare against a deleted implementation and must be retired in the same cascade — a zombie oracle is its own kind of debt.

### 3.2 Event routing (R5 + R6): five domain buses → one native router

Gate conditions (all must hold):

- **GATE-B1 — production construction.** `BusFacade` is constructed in the production composition root; every production event type is registered in the native type registry at startup; `UseNativeBusForDispatch` is set there, not in tests.
  - *Open while:* `BusFacade` has zero production constructor call sites (EVENT_BUS.md line 76).
- **GATE-B2 — dispatch equivalence.** For N ≥ 1000 ticks, the same publication set yields the same handler invocations at the same phase boundaries with the flag on vs off — including preservation of the captured-`SystemExecutionContext` law (handlers run under the *subscriber's* identity, EVENT_BUS.md line 47) across the batched-callback ABI, and of `[Deferred]` snapshot re-entrancy semantics (EVENT_BUS.md line 49).
  - *Open while:* no flag-on/flag-off lockstep harness exists under `tests/`.
- **GATE-B3 — capability enforcement live.** Fast/Background per-FQN per-tier capability declarations (K-L15 row, KERNEL line 64) are checked against at least one real mod manifest on the native path — the enforcement must have a non-vacuous population before it can be called sovereign.
  - *Open while:* production registers zero native subscribers (EVENT_BUS.md line 77 — the bulk unsubscribe is "vacuously today").
- **GATE-B4 — observability parity.** The production path has counters *before* it switches (closing the EVENT_BUS.md line 123 gap). Cutting over must not mean flying blind for the first time on the busiest path.
  - *Open while:* `DomainEventBus` exposes no publication/delivery counters.

**Equivalence-proof obligation:** the flag-on/flag-off lockstep run above, plus the existing S10 cross-tier re-entrancy probe (`SchedulerExtremeTests.cs:1007`, cited in the K-L15 row) kept green, recorded as dashboard rows.

**DELETION TRIGGER:** the five `IGameServices` buses become **typed facades over the one native router** — the five-property surface survives (it is canon per CONTRACTS.md and K-L9 uniformity) but the per-bus subscriber tables and `ConcurrentQueue` deferred queues leave `DomainEventBus`; `IDeferredFlush` re-targets to the Normal-tier batch drain; `FastTierContractMonitor` gains a real population or is deleted; and the `UseNativeBusForDispatch` flag itself is deleted — a permanent runtime flag choosing between two routers is a standing second authority.

### 3.3 Interim coexistence rules (in force while any row stays split)

Until a split row's deletion trigger fires, the duplicate planes must at least not drift apart silently:

1. **Dual registration stays mandatory.** Every production system registers with both graphs (`GameBootstrap.cs:145-181` already does this); every production event type remains representable in both bus vocabularies. A system or event visible to only one plane makes GATE-S2/GATE-B2 untestable.
2. **The declarer stays single.** `[SystemAccess]`, `[TickRate]`, `[EventTier]`, `[Deferred]` remain the only sources both planes derive from — neither plane may grow a private side-channel declaration (a native-only priority table, a managed-only tier override).
3. **No new consumers of the losing plane's internals.** New code may consume the *surfaces* (`IGameServices`, `SystemBase`) but not `DependencyGraph` phase lists or `DomainEventBus` queue internals directly — every such consumer is one more edge the deletion trigger has to cut.
4. **Semantics changes land on both planes in one commit** or not at all: a delivery-mode tweak implemented managed-only widens the equivalence gap the gates must later close.

### 3.4 The debt rule (PA-003 refinement)

PROJECT_AXIOMS.md PA-003 §3.4 cites the "Three-graph + two-scheduler architecture" as justified complexity ("Simpler architectures (single scheduler / single graph) were considered and rejected", PROJECT_AXIOMS.md line 132). This document proposes the refinement:

> **Duplicate authority is justified complexity only while a named cutover gate and deletion trigger exist. Duplicate authority without them is architectural debt** — precisely the "hidden coupling that will require unwinding в 6 months" that PA-003 §3.3 instructs against, wearing soundness clothing.

Adopting this refinement touches PA-003's anchor text and therefore requires the PROJECT_AXIOMS §5 amendment protocol (Crystalka-surfaced, never default-amended).

## 4. Precedence rule for the §14.7 authority surface (proposal)

FRAMEWORK.md §14.7 admits a document to the authority surface (`lifecycle == Live` OR (`lifecycle == LOCKED` AND `tier ∈ {1, 2}`), FRAMEWORK.md line 813) but says nothing about two admitted documents disagreeing. Proposed arbitration, in order:

1. **P1 — matrix wins on named domains.** For any domain with a row in §2, the document named in that row's authority column is the governing text for that domain; other surface members yield *for that domain only*. Example: scheduling semantics — KERNEL Part 0 (K-L12) + THREADING govern; ARCHITECTURE.md's one-line summary (line 15) yields.
2. **P2 — specificity wins on unnamed domains.** Absent a row, the more specific subsystem document (THREADING, EVENT_BUS, VULKAN_SUBSTRATE, MOD_OS_ARCHITECTURE, ECS, ISOLATION) beats the umbrella (ARCHITECTURE.md); the umbrella beats orientation/README text. Worked example: mods' reference surface — ARCHITECTURE.md line 52 says mods reference "**only** Contracts" while MOD_OS_ARCHITECTURE.md §5.4 admits "other shared mods"; under P2 MOD_OS governs and the umbrella needs a PATCH, instead of today's standoff between two LOCKED Tier-1 texts.
3. **P3 — evidence-class documents never win.** Reports, dashboards, recon documents, and closure reports (while AUTHORED) never override a surface member, whatever their tier. This bites today: the canonical K-L14 and K-L15 texts physically reside in K_CLOSURE_REPORT.md (§1.2, §2.17 line 571) — a Tier 1 **AUTHORED** document that is off the §14.7 surface by the predicate itself, yet self-describes as "load-bearing authority surface" (K_CLOSURE_REPORT.md line 75). Under P3 the abbreviated KERNEL row governs until the canonical text is rehomed or K_CLOSURE_REPORT transitions to LOCKED.

**Adoption note:** §14.7 is "tunable law: it is stated here, and a change to it is an amendment of this section" (FRAMEWORK.md line 815). P1–P3 therefore cannot be adopted by this document or by ARCHITECTURE.md — they require a FRAMEWORK §14.7 amendment through the §7.2 schema-amendment protocol. Until that amendment lands, conflicts between surface members remain formally unarbitrable and this section is advisory.

## 5. Observability of authority

An authority claim nobody can catch being violated is decoration. Where violations of each row are detectable today:

| Row | Analyzer rule | Test / runtime check | Honest status |
|---|---|---|---|
| R1 storage | DFK011 (Error, enforcing) + DFK003/DFK003_1 (ANALYZER_RULES.md §4) | Span/batch suites; native selftest | Enforced |
| R2 scheduling decisions | **DFK012 deferred to the K-L20 LOCK cascade** (ANALYZER_RULES.md line 269) | `df_native_selftest scenario_system_graph_*` (synthetic systems only, `selftest.cpp:1034` ff.); zero scheduler-cutover dashboard rows | **Unenforced today** — nothing detects the managed planner deciding |
| R3 dispatch | none | `GameBootstrapIntegrationTests` smoke | Unenforced today |
| R4 mod load order | none (DFK020 family deferred to K-L20) | `ContractValidator` Phases A–H at load time (MOD_OS_ARCHITECTURE.md line 297) — a *runtime* gate, the right shape here | Enforced at load time |
| R5 event routing | **DFK015 deferred to the K-L20 LOCK cascade** (ANALYZER_RULES.md line 271) | Native path exercised only by scheduler stress/extreme suites (EVENT_BUS.md line 76) | **Unenforced today**, observability inverted (live path counter-less, EVENT_BUS.md line 123) |
| R6 delivery invocation | none | S10 cross-tier re-entrancy probe (`SchedulerExtremeTests.cs:1007`); `FastTierContractMonitor` — polices a tier with no production subscribers | Partially observed |
| R7 Vulkan | DFK019_A (Warning, enforcing) | `HardwareCapabilityCheck.Verify` fail-fast (`Runtime.cs:83`); `V1DiffusionEquivalenceTests` | Enforced |
| R8 window/input | none | none — there is no input→sim contract to violate yet | Unenforced today (vacuous) |
| R9 lifecycle | none | none; no teardown test exists (a skipped `df_world_destroy` is invisible) | Unenforced today |
| R10 tick | none | single-writer by convention only | Unenforced today |
| R11 identity | DFK004 (Error, enforcing) covers the component-type registry only | Generation checks defeated wherever the version-0 idiom appears | Partially enforced (one of five id spaces) |
| R12 persistence | none | none | Unenforced (nothing to enforce — ownerless) |
| R13 configuration | none | none | Unenforced (ownerless) |

The pattern is exact and worth stating: **the two split rows (R2, R5) are also the two whose analyzer rules were deferred** (DFK012 / DFK015, plus DFK018 quiescence — ANALYZER_RULES.md lines 269-272). The domains with duplicate authority are precisely the domains where no tooling would notice the duplicate deciding. Any ratified version of this matrix should treat "split row" as an automatic priority argument for the corresponding deferred rule.

## 6. Open questions and conflicts with current LOCKED texts

### 6.1 Conflicts this proposal has with standing LOCKED law

1. **Rule 5 vs K-L12 batched callbacks.** KERNEL §1.5 Rule 5: "Native side has no callbacks к managed. Direction unidirectional (managed → native always)" (KERNEL line 319); Part 8 repeats "never reverse P/Invoke" (line 897). The same LOCKED document's K-L12 row mandates "Cross-layer communication uses C ABI with batched callbacks" (line 61), and the shipped bridge is an `[UnmanagedCallersOnly]` reverse entry point (THREADING.md line 43 — the only place the reconciliation is written down). This matrix sides with K-L12; ratification must amend Rule 5 and Part 8 inside KERNEL itself, not leave the reconciliation outsourced to THREADING.
2. **KERNEL §1.4 pre-K10 stratum.** "Thread pool idle (managed scheduler doesn't use it)" (line 286) and "Worker threads (existing ParallelSystemScheduler)" describe the pre-K10 world inside the same LOCKED doc that carries K-L12 — two normative layers, no precedence marker. R2/R3 contradict §1.4 by construction; the §1.4 diagram needs the same amendment. Likewise KERNEL Part 4's open-question table still says "Native event bus … Currently не planned" (line 668) while K-L15 in Part 0 of the same document is LOCKED law.
3. **PA-003 §3.4 three-graph example.** Presently readable as indefinite tenure for graph coexistence; §3.4 above narrows it to gated tenure. Requires the PROJECT_AXIOMS §5 protocol.
4. **KFNS self-status.** KERNEL_FULL_NATIVE_SCHEDULER.md line 17 states "native scheduler live"; with empty access sets and Timer-1 wakes (THREADING.md line 38) the scheduler is *installed*, not deciding. The scheduling-authority doc overstating its own plane's liveness is exactly what an R2 matrix row exists to correct. (Its lock rationale also points to `K10_DELIBERATION_STATE`, "external deliberation archive — not in repository", KFNS line 26 — unverifiable provenance for Tier-1 law, a §4 P3 concern.)
5. **K-L4 vs FNV-1a event ids.** K-L4's rationale rejected FNV-1a as "collision-prone" for component ids (KERNEL line 53); `BusFacade` mints event-type ids by FNV-1a (EVENT_BUS.md line 76). Either the rationale is scoped to component ids only (then KERNEL should say so) or R11's forbidden column applies to the event-id mint.

### 6.2 Open questions (not resolvable by this session; need deliberation)

- **Q1:** Does the R5 cutover require the K-L20 Mod API lock first, given that GATE-B3's capability grammar is manifest-coupled and DFK015 was deferred to that same cascade?
- **Q2:** Who owns R12 — Application, the Persistence assembly, or the kernel (whose Background queue already carries save-integrated serialization, EVENT_BUS.md line 59)? Two save formats are already latent.
- **Q3:** Is `TickScheduler` part of the R2 scheduling authority (dies with the managed planner at GATE-S4) or a legitimate declarer-facade over TimerWake rates?
- **Q4:** Must the version-0 `EntityId` idiom (`GameBootstrap.cs:241`, `EntityEncoder.cs:85`) be outlawed before R12 gains an owner, since persisted index-only ids make generation validation unrecoverable?
- **Q5:** Where does the input→sim contract live when R8's discard loop (`Program.cs:81-84`) is finally wired — a new matrix row, or an extension of R8?
- **Q6:** Does the §4 precedence amendment ride the register-inversion Cascade B (FRAMEWORK §14.9) or a standalone §14.7 amendment?

---

*End of authored-proposal. Nothing above is law until ratified; every "today" claim is falsifiable against HEAD `6f39903` and should be re-verified at ratification time.*