---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-METHODOLOGY
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.9"
next_review_due: 2027-05-21
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-METHODOLOGY
---
# Dual Frontier development methodology

*The project's central methodology document. Describes the architect-executor split with contracts as IPC across context boundaries, the verification cycle, economics, threat model, empirical results, and boundaries of applicability.*

*Version: 1.11 (2026-05-23). К-extensions cascade #2 closure — Godot Full Deprecation + Launcher Formalization. Lesson #N12 «Defensive Reserved Stub Pattern» captured as Provisional candidate с first-application evidence (Launcher's RenderCommandDispatcher 6 defensive throws per Q-G-3 + Q-G-6 (b1)). Lesson #25 refined per Crystalka 2026-05-23 deliberation framing: «Главное правило — не тестировать пустые интерфейсы и пустую реализацию которые врут в тестах, показывая что всё OK» — refined statement: «Design abstractions when consumer materializes AND structurally eliminate test-lying surface — empty stub implementations что pass tests by doing nothing constitute architectural debt independent of speculation discipline.» Paired discipline с Lesson #N12 (Defensive Reserved Stub Pattern covers cases where structural surface must ship ahead of consumer materialization). 10 DEFER candidates Provisional pool post-cascade-#2 (#18, #19, #N3, #N5, #N6, #N7, #N8, #N9, #N10, #N12). FORMALIZE: 12 → 12 (unchanged — #25 refinement не FORMALIZE bump; refinement within already-FORMALIZED Lesson). SUNSET: 1 (unchanged). §1-§11 substance unchanged.*

*Version: 1.10 (2026-05-23). A'.8 К-series formal closure — Lessons promotion batch landed per [K_CLOSURE_REPORT.md §6](../architecture/K_CLOSURE_REPORT.md#6--lessons-promotion-individual-decisions). 12 FORMALIZE actions (#7 strengthened, #8 strengthened, #9 + #9.1 sub, #10, #14 PROMOTED at A'.8, #16, #17, #21, #25 strengthened с A'.7.5 application, #26, #27 PROMOTED at A'.8, #N2 strengthened с A'.7.x δ1-δ3 application) per Q-N-8-5 LOCKED Session 2 Day 2. Lesson #14 PROMOTED criterion satisfied: second clean application surfaced (А'.7.x bus refactor on separate `claude/scheduler-stress-test-KmVM3` branch + Godot removal cascade on separate `claude/godot-removal-deliberation-Vfg2R` branch). Lesson #27 PROMOTED criterion satisfied: third application surfaced (А'.7.x bus stress workload exercised K-L13 wake-source diversity, surfacing 5 Bugs in К10.2 bus primitives). 9 DEFER candidates Provisional pool (#18, #19, #N3 carried; #N5/#N6/#N7/#N8/#N9 from А'.7.x; #N10 from Godot removal). 1 SUNSET (Lesson #15 subsumed by Lesson #20). К-L14 falsifiability criterion 6 (soft-halt rate exceeds X% across N consecutive cascades) introduced as Provisional per Q-N-8-7 LOCKED — threshold TBD pending 2nd soft-halt observation. К-closure report K_CLOSURE_REPORT.md authored Tier 1 AUTHORED Category A per Q-N-8-4 amendment к Meta-Q1 Session 1 LOCKED commitment. §1-§11 substance unchanged.*

*Version: 1.9 (2026-05-21). §12.7 closure protocol step 1 «Run final verification» expanded к explicit per-test-suite checklist — Core + Modding suite runs are both mandatory at every closure (not just Core), per CAPA-2026-05-21-A_PRIME_7_X-K10_3-V2-SOFT-HALT corrective action (c). K10.3 v2 closure (2026-05-20) ran only the Core suite + native selftest; the Modding suite was not exercised, which let a transient fixture-copy build-state issue ship undetected (initially mis-diagnosed as a К-L18 regression в the А'.7.x investigation report and brief Hypothesis 1). The CAPA fix re-frames the gate so that closure verification covers every test project в the solution explicitly, not just «dotnet test» applied к whichever subset the executor remembered. Lessons batch deferred к A'.8 closure (METHODOLOGY v1.9 → v1.10) per Q-N-7X-11 split. К-extensions cascade #0 designation (А'.7.x) does not change §1-§11 substance.*

*Version: 1.8 (2026-05-17). Lessons #11, #20, #22 formalized at К10 deliberation S6 lock. NEW «Provisional Lessons» section captures candidates pending promotion (9 candidates: #9, #10, #14, #15, #16, #17, #18, #19, #21). Lesson formalization model hybrid: high-confidence lessons promoted immediately when surfaced; low-confidence remain provisional pool, promoted at К-closure report (А'.8) timing per accumulated evidence.*

*Version: 1.7 (2026-05-12). Document Control Register integration per A'.4.5 closure — new §12 specifies the register as governance authority with post-session update protocol (Q-A45-X5); §7.1 «Data exists or it doesn't» extended with seventh formal invocation at the documentation layer (every document either has a register entry or is out-of-scope); §11 «See also» extended with [FRAMEWORK](../governance/FRAMEWORK.md) + [SYNTHESIS_RATIONALE](../governance/SYNTHESIS_RATIONALE.md) links. Closure protocol §12.7 is now canonical (MIGRATION_PROGRESS.md closure protocol section cross-references it).*

*Version: 1.6 (2026-05-10). Pipeline restructure rewrite per A'.0.7 deliberation session. §0 Abstract generalized к architect-executor abstract framing; §2.1 role distribution rewritten в abstract role categories с v1.6 current-configuration table; §2.2 contracts as IPC reframed across context boundaries с three-properties mechanism; §3 economics restructured к invariant + current-configuration с A'.0.5 empirical anchor; §4 throughput parallel-form case studies (Phase 4 closure v1.x + A'.0.5 closure v1.6); §5.2/§5.3 threat model restructured для v1.6 session-mode reality; §9 «degradation as codebase grows» reformulated к pipeline-agnostic; methodology corpus declared agent-as-primary-reader per Q-A07-6. K-Lessons sub-section expanded с A'.0.5 lesson «milestone consolidation under session-mode pipeline» per Q-A07-5.*

---

## 0. Abstract

This document describes a methodology for developing complex software solo through structured work with an LLM. The methodology tests a hypothesis: a human in the role of contract architect plus one or more LLM instances operating as executors inside strict contract boundaries can produce engineering systems of the same complexity class that usually requires teams of several developers over months.

The configuration is **N agents in an architect-executor split with rigid contracts at boundaries**: a human as direction owner; one or more LLM instances operating as architect (deliberation, brief authoring, QA review) and executor (mechanical application against authored briefs). There is no direct coordination between agents — formal contracts in code and documentation in the repository tie them together, acting as inter-process communication. The architect-executor split with contracts at boundaries is **invariant across configurations**; specific N, the boundary type between architect and executor (model-tier boundary, session-mode boundary, or mixed), and tier mix vary by pipeline configuration.

The methodology's main falsifiable claim: a working production-quality game, built by one developer through this methodology in 6–12 months, with measured pipeline performance, defect rate, and architectural integrity over the long haul. Empirical measurements are recorded in [PIPELINE_METRICS](./PIPELINE_METRICS.md) per-era — see particularly [§2 task-level metrics](./PIPELINE_METRICS.md#2-empirical-task-level-metrics) and [§4 sustained throughput](./PIPELINE_METRICS.md#4-sustained-throughput). Additional falsifiable claims appear in §2.2, §3.1, §4.5, and §5.3.

The methodology is not universal. Boundaries of applicability are recorded in §6.

*Footnote (v1.6, 2026-05-10).* At this methodology version, N=2: Crystalka as direction owner plus a unified Claude Desktop session that switches between deliberation mode (chat interface, architectural-decision recording per K8.0 / K-L3.1 / A'.0.7 precedent) and execution mode (Claude Code agent, tool-loop autonomous, per A'.0.5 precedent). The boundary type is **session-mode**, not model-tier. v1.x era (Phase 0–8, ending 2026-05-09) configured N=4 with model-tier boundaries (local Gemma executor + cloud Sonnet prompt-generator + cloud Opus architect + human); empirical record preserved in [PIPELINE_METRICS](./PIPELINE_METRICS.md) §1–§4 with per-metric transferability annotations. This document is authored under **agent-as-primary-reader assumption (Q-A07-6 lock 2026-05-10)** — readers unfamiliar with the project's cross-reference density should consult [README](../../README.md) first and use AI tooling for navigation.

---

## 1. Context and problem

Public discourse on LLM-driven development swings between two poles. On one side is *vibe coding*: development through dialogue with an LLM without a structural plan, in the hope that the model itself will build a working system. This approach prototypes quickly but does not scale. The absence of structure produces cascading rework, lost context between sessions, and architectural drift.

On the other side is the "AI will replace senior engineers" narrative: the LLM takes on architectural decisions, business-context understanding, and strategic planning. This approach does not work, because architectural decisions require an understanding of meaning (why we are building this, how it fits the existing context, which trade-offs are justified) that the LLM does not have.

Between these poles sits a third model: **the human as contract architect, the LLM as executor inside strict contract boundaries**. Architectural decisions, contract choice, and direction of development stay with the human. Implementation against precisely formulated contracts is delegated to the LLM. Between these two layers runs a formal verification infrastructure — types, tests, static analysis — that catches divergences between intent and result. The idea itself is not new; what is new is a concrete production configuration that makes the approach economically sustainable for a solo developer.

### Experimental control

The author has no formal computer science education and no
software engineering job history prior to this project. This is
not a disclaimer — it is a methodological control. If the author
held a CS background, the methodology's apparent success could
be attributed to unstated transferred expertise: the engine
would compile because the author already knew how to make
engines compile. Without that background, the methodology has
to do the work, or nothing does.

Author profile and prior work history: linked from the
[GitHub profile](https://github.com/Crystalka228).

---

## 2. Pipeline architecture

### 2.1 Role distribution

The pipeline configures three role categories operating across rigid contract boundaries:

**Direction owner** (human). Selects contracts, makes architectural decisions, frames phase goals, routes between sessions. Per unit of time, types noticeably fewer keystrokes than a classical developer and makes noticeably more architectural decisions.

**Architect** (LLM in deliberation mode). Architectural deliberation, brief authoring, QA review at phase closure. Operates on the corpus as a whole rather than individual files. Used for hard architectural tasks (the scheduler, the dependency graph, non-trivial algorithms, K-Lxx invariant deliberation) and for full reviews at phase closure. Brief authoring is itself a deliberation product — the architect formalizes decisions before any execution begins (see «Brief authoring as prerequisite step» in the Native layer methodology adjustments section).

**Executor** (LLM in execution mode against authored briefs). Mechanical application of briefs to the codebase: source edits, atomic commits, build verification, test runs. Makes no architectural decisions; escalates to the direction owner when a brief proves insufficient for mechanical execution (per §3 stop/escalate/lock).

There is no direct coordination between the architect and the executor — formal contracts in code, briefs in `tools/briefs/`, and amendment plans in `docs/architecture/` tie them together, acting as inter-process communication. The architect's deliberation output is the executor's input contract; the executor's commits are the architect's audit input at the next phase closure.

The boundary between architect and executor may be a **model-tier boundary** (different LLM models for each role), a **session-mode boundary** (same LLM, different session framing), or **mixed**. The split itself is invariant; the specific boundary type varies by pipeline configuration.

#### 2.1.1 Current configuration (v1.6, 2026-05-10)

| Role | Current binding |
|---|---|
| Direction owner | Crystalka (human) |
| Architect | Claude Desktop session, deliberation mode (chat interface, no autonomous tool loop) |
| Executor | Claude Desktop session, execution mode (Claude Code agent, tool-loop autonomous) |
| Boundary type | Session-mode (single underlying model; mode-switching by session framing) |
| Coordination surface | Repository (LOCKED docs, briefs, amendment plans); human as router between sessions |

Earlier configurations (v1.x era, Phase 0–8, ending 2026-05-09) used model-tier boundaries with N=4 (local quantized Gemma executor + cloud Sonnet prompt-generator + cloud Opus architect + human direction owner); empirical record preserved in [PIPELINE_METRICS §1](./PIPELINE_METRICS.md#1-pipeline-configuration) с per-metric era annotations.

### 2.2 Contracts as IPC across context boundaries

This is the central methodological device. In traditional development, contracts — interfaces, types, protocols — serve as a communication mechanism between subsystems of the code. In an LLM pipeline they additionally become a communication mechanism across **context boundaries** between architect and executor (§2.1) — whether those boundaries are model-tier, session-mode, or mixed.

A rigid contract is the point where interpretations converge. If a contract is formulated strictly enough to exclude ambiguity, any two entities working against it (whether different LLM models, the same LLM in different sessions, or the same LLM in different operating modes) will produce compatible artifacts. This works because the contract specifies a condition, not a preference — the compiler, tests, or a static analyzer can check satisfaction mechanically.

The IPC-equivalence depends on three properties of the contract:

1. **Falsifiability.** The contract states a condition checkable by tooling (types, tests, analyzers), not a preference checkable by judgement. An entity satisfying the contract is recognizable as satisfying it; an entity violating it triggers a known failure mode.

2. **Self-contained scope.** The contract carries enough context to be applied without consulting the entity that authored it. Briefs in `tools/briefs/` capture this property explicitly — Phase 0 reads + Phase N execution + closure criteria are scoped so an executor reading the brief at execution time has the same information the architect had at authoring time.

3. **Repository as coordination surface.** Contracts and their satisfaction artifacts (code, tests, commits) live in version-controlled documents. Cross-references between contracts use repo-rooted absolute paths and §-level addressability so any reader (any agent, any session) resolves them identically.

Coordination between architect and executor is resolved through these three properties operating on the repository, not through direct message exchange. This is the mechanism by which the architect-executor split (§2.1) remains tractable across any boundary type — model-tier, session-mode, or mixed.

**Falsifiable claim.** If the architecture is fixed as a set of contracts satisfying the three properties above, the executor produces artifacts that pass the contract's tooling-checkable conditions on the first build at a measurable rate. The measurement metric is the fraction of tasks requiring a second execution round after the first build; the target threshold is under 30%. An empirical example of asynchronous development against a formal contract appears in §4.2; per-era measurements live in [PIPELINE_METRICS §2](./PIPELINE_METRICS.md#2-empirical-task-level-metrics).

Contracts in Dual Frontier live in a separate `DualFrontier.Contracts` assembly that contains no implementation (see [CONTRACTS](/docs/architecture/CONTRACTS.md)). It acts as the single vocabulary for every layer of the system: the core, game systems, mods, and build tooling.

### 2.3 Verification cycle

The base sequence for one task:

```
Direction owner frames the task plus contract
  ↓
Architect-mode session formalizes the brief (Phase 0 reads + Phase N
  execution plan + closure criteria); brief becomes the executor's
  input contract
  ↓
Executor-mode session generates code against the brief
  ↓
Build verifies syntactic conformance
  ↓
Tests verify behavioral conformance to the contract
  ↓
Atomic commit with a scope prefix
```

Every step has a failure point with explicit diagnostics. The build is either clean or not. A test either passes or not. The contract is either satisfied or not. Verification is structural, not based on trust in the LLM.

After each substantial development phase closes, the **extended phase-review cycle** runs:

```
Architect-mode session: diagnose contradictions and debt across the
  corpus; validate prior closure reports against current code state;
  find endemic patterns; formulate architectural decisions with
  explicitly rejected alternatives; implement the decisions in code,
  write tests, debug, update documentation
  ↓
Direction owner: review the result, push the commits
  ↓
Self-teaching ritual (§4.5): systematize understanding
                             of the built system
```

This extended cycle runs less often than the base cycle — once every 1–2 weeks — and spends substantially more tokens, but its role is critical: it closes architectural debt and preserves corpus integrity over the long haul.

Per §2.1, the architect-mode and executor-mode sessions may be the same underlying LLM in different session framings (v1.6 era, session-mode boundary) or different LLMs (v1.x era, model-tier boundary). The structural shape of the cycle is invariant; the boundary type determines whether the «Architect-mode session» and «Executor-mode session» lines describe one model or two.

### 2.4 Atomicity of phase review

An architectural decision and its implementation during a phase review are **not separable**. When the architect formulates a decision, the architect verifies it through implementation: writes the code, runs the tests, sees one test fail, traces the cause, fixes the logic. Without the implementation step it is unknown whether the decision will work in reality or stay as an elegant formulation on paper.

This means that **splitting architecture and implementation between agents saves tokens in theory but creates desynchronization points in practice**. Each such point requires returning to the architect for analysis and reformulation, which in sum spends more tokens and more calendar time than an atomic session. An atomic architect session is not an expensive compromise — it is the methodologically correct way to use an architectural agent in the QA role.

---

## 3. Pipeline economics

### 3.1 Economic invariant

The architect-executor split has an economic correlate: architectural deliberation (architect work) and mechanical execution (executor work) have **structurally different cost profiles** regardless of boundary type between them.

Architectural deliberation is **context-intensive**: it reads the corpus as a whole, holds multiple alternatives in working memory, produces structured output (briefs, amendment plans, locked decisions). Token cost per architectural decision is high; calendar frequency is low (one deliberation session per phase closure, typically 1–2 weeks apart).

Mechanical execution is **scope-bounded**: it operates against an authored brief, applies pre-decided changes, verifies through tooling. Token cost per execution is bounded by brief scope; frequency is high (multiple executions per day during active phases).

**Invariant**: as long as architectural decisions are captured in briefs upfront — Phase 0 reads + locked design questions + Phase N execution plan + closure criteria — executor cost scales with **size of execution**, not **size of architectural surface** the executor must reason about. This is the economic mechanism that makes the pipeline sustainable under a fixed-cost configuration (subscription tier, self-hosted infrastructure, mixed).

**Falsifiable claim.** Pipeline operating against this discipline sustains development cadence within a fixed-cost ceiling over a 6+ month horizon. Falsification: at any point in the development window, session-cost or subscription-tier-utilization grows non-linearly with codebase size or feature count. Pipeline-restructure events (boundary type changes) re-baseline this measurement.

### 3.2 Current configuration economics (v1.6, 2026-05-10)

Boundary type: session-mode. Single underlying LLM (Claude Desktop) switches between deliberation mode and execution mode per session framing.

- **Deliberation sessions**: chat interface, manual turn-by-turn. Architectural decision recording brief shape (K8.0 / K-L3.1 / A'.0.7 precedent) constrains scope to one architectural surface per session. Token cost per session: large context (Phase 0 reads of relevant LOCKED docs + brief authoring); single session per architectural lock.
- **Execution sessions**: Claude Code agent, autonomous tool loop against authored brief. Per-execution-session scope bounded by brief; multiple sessions per day during active phase.
- **Subscription tier**: Claude Max 5×. Empirical headroom under v1.6 era reality — see [PIPELINE_METRICS §3](./PIPELINE_METRICS.md#3-subscription-headroom) (v1.x era measurement preserved verbatim with transferability annotation per Q10).
- **Empirical reference (v1.6)**: A'.0.5 execution session (commit range `27523ac..4e332bb`) delivered ~25 atomic commits, ~250 cross-reference updates, 36 file relocations, +4354/-653 LOC, in a single 2–4 hour session window, with test baseline preserved at 631 throughout. This is the v1.6 reference data point for execution-session scope vs cost.

v1.x era economics (model-tier boundary, N=4 — local Gemma executor free + cloud Sonnet prompt-generator paid + cloud Opus architect paid + human) operated under the same economic invariant but with different cost profile per role: executor cost was zero (local hardware), prompt-generator and architect cost was subscription-bounded. Empirical record preserved in [PIPELINE_METRICS §1–§4](./PIPELINE_METRICS.md).

---

## 4. Empirical results from Dual Frontier

### 4.1 State at publication

Snapshot of the `Crystalka228/Dual-Frontier` repository on 2026-05-10 after A'.0.5 closure (Phase A' in progress):

| Parameter | Value |
|---|---|
| Project age | 20 days (since 2026-04-20 origin) |
| Commits (across all branches) | 600+ |
| Tests | 631/631 passing |
| Production bugs | 0 |
| Architectural assemblies | 11 (post-K8 native split) |
| Architectural documents in `docs/architecture/` | ~30 (post-A'.0.5 reorg) |
| Methodology documents in `docs/methodology/` | 3 (METHODOLOGY + PIPELINE_METRICS + MAXIMUM_ENGINEERING_REFACTOR + supporting) |
| Completed phases | Phase 0–8 (K-series kernel foundation + K-L3.1 bridge formalization) |
| Current phase | Phase A' (between K-series closure and M-series mass migration) |

The architecture includes a custom ECS with both Path α (`unmanaged struct` via NativeWorld, K8.1 primitive wrappers) and Path β (managed `class` via per-mod `ManagedStore<T>` per K-L3.1 bridge formalization) storage paths, a parallel scheduler with topological phase sorting, a dependency graph with Kahn topological sort and formal access verification, an event bus with three delivery modes (synchronous, `[Deferred]`, `[Immediate]`), physical mod isolation via `AssemblyLoadContext`, and a persistence layer with RLE + StringPool independent of Godot. The native ECS kernel runs alongside the managed reference implementation (K-L11 «World-as-test-fixture»). Cross-references: [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md), [KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md), [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md).

### 4.2 Case study: asynchronous native-core development

The idea for an experimental branch with a native C++ core emerged during the author's shift work outside the home. The prompt was sent to the architect-mode session from a mobile device; by the time the author returned home, the `claude/cpp-core-experiment-cEsyH` branch had been built by the agent, published to the repository, and made available for local build. A local `dotnet build` of the solution passed on the first try with no edits.

Several hours of asynchronous work passed between task formulation and working artifact, during which the author could not physically participate. This works because `DualFrontier.Contracts` is rigid enough to play the IPC role between sessions, regardless of boundary type, without synchronous coordination. The architect-mode session wrote code against a formal interface; the local build verified conformance to the interface. Divergence between intent and result is physically impossible when the code passes the build and the tests, because the very notion of "correct code" is fixed in the contract, not in interpretation.

The experiment itself ended with a negative result — per-element P/Invoke ate the native gain on the benchmark (NativeAdd10k: ratio 3.92× against the managed baseline). This is a separate methodological result: the acceptance criterion was reframed from mean latency to p99, GC pause, and long-run drift; the batch-API decision is deferred to Phase 9. Details are in [NATIVE_CORE_EXPERIMENT.md](/docs/reports/NATIVE_CORE_EXPERIMENT.md). What matters for this document: the cycle "hypothesis → asynchronous implementation → measurement → criterion reframing → deferred decision" fit into hours, not weeks, with no drop in code quality in the repository.

### 4.3 Comparison with typical indie development

The architectural base that exists in the repository on day 5 of its life would normally require 9–15 months of work for a team of 2–3 people. Custom multithreaded ECS with formal verification: 2–3 months. Mod isolation through `AssemblyLoadContext` with physical isolation: 1–2 months. Event bus with three delivery modes: 1 month. Persistence layer with RLE and StringPool: 2–3 weeks. Integration of every layer plus the Godot bridge: 1–2 months.

This yields roughly 60–100× compression against the typical pace of indie development on the architectural portion.

**Falsifiable claim.** Compression applies to architectural work. Game content, balancing, and narrative shrink less, because the "good/bad" criterion is subjective and verifiable only through playtesting. Falsifying it: measure the same indicator on a domain with a subjective criterion and find comparable acceleration there.

### 4.4 Session wall-clock performance — case studies

Per Q-A07-3=β+γ (PIPELINE_METRICS preservation as historical), per-era throughput measurements live primarily in [PIPELINE_METRICS](./PIPELINE_METRICS.md). Two methodology-level case studies preserved here as falsifiability anchors:

**Case A: Phase 4 closure review (v1.x era, 2026-04-25).** Architect session ~35 wall-clock minutes from first to last commit (per session log; the session's commits are squashed in the git history, so the timeline cannot be reconstructed directly from timestamps). Within those 35 minutes the architect validated the prompt-generator's diagnostic (10 items), discovered 5 additional issues (including an endemic `NotImplementedException` pattern across 22 systems), formulated 6 architectural decisions with explicitly rejected alternatives, implemented the decisions in code, wrote 17 new tests while debugging two self-introduced failures, and updated five documents. Result: tests 65/65 → 82/82, Phase 4 closed, Phase 5 unblocked. Boundary type: model-tier (architect = cloud Opus session, executor = local Gemma). Full session log: [SESSION_PHASE_4_CLOSURE_REVIEW](/docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md). *(Russian-language audit trail; preserved verbatim per the i18n campaign rules.)*

**Case B: A'.0.5 closure session (v1.6 era, 2026-05-10).** Execution session — Claude Code agent against authored brief. Delivered ~25 atomic commits over commit range `27523ac..4e332bb`: 36 file relocations (Phase 3), ~250 cross-reference updates (Phase 4), 5 component README cleanups (Phase 5), 6+ module-local doc refreshes (Phase 6), 7 pipeline-terminology scrubs (Phase 7), 1 Tier 1 typo fix + Tier 2 flag artifact (Phase 8), 3 closure commits (Phase 9). Total diff: +4354/-653 LOC. Test baseline preserved at 631 throughout. Session window: 2–4 hours wall-clock. Boundary type: session-mode (architect-mode session authored brief upstream; A'.0.5 invocation = execution-mode session). Full closure entry: [MIGRATION_PROGRESS §A'.0.5](/docs/MIGRATION_PROGRESS.md).

Per-session wall-clock varies with session scope (architectural deliberation vs mechanical execution, codebase size at session time, brief depth). The methodology-level point is **case studies as falsifiability anchors**: each closure session is named, dated, boundary-type-tagged, and the per-session metrics live in [PIPELINE_METRICS](./PIPELINE_METRICS.md) per-era data sections.

In typical team development, an equivalent phase session would be 16–24 hours of total work by a team of 2–3 people, or several days on the calendar. Solo without an LLM: at least a week.

### 4.5 Self-teaching ritual between phases

After each substantial phase closes, the author systematizes their own understanding of the built system in the format of a learning artifact. Not for others — for the self, to reread a month later and quickly restore context. This is a **distinct methodological step**, not a byproduct.

The learning artifact functions as **prompt context for the next phase**. When the prompt generator formulates a prompt for the executor, it leans on systematized understanding of the architecture, not on scattered recollections. Without this step, architectural drift accumulates: the code grows, but the semantics of contracts blur between phases.

The first ritual artifact after Phase 1 (Core ECS) is [learning/PHASE_1.md](./learning/PHASE_1.md). The document covers: class vs struct and why `EntityId` is the canonical struct example; generics and how to read `ComponentStore<T>` signatures; interfaces as architectural promises; attributes as the system's declarative language (`[SystemAccess]` via reflection with mandatory `inherit: false`); the nullable context as an early contract guard; a race condition on the `DependencyGraph` example; `ThreadLocal` and why the guard is bound to the thread; the stack trace and how to read it correctly from bottom to top; tests as invariant proofs. Attached to the theory is a 14-day study path with concrete files and practical exercises.

**Falsifiable claim.** Skip self-teaching after one phase and measure whether defect rate rises in the next phase. If yes, the step is necessary. If no, it is redundant.

An additional effect: the self-teaching artifact serves as proof of the author's competence — a reader of the learning material can within minutes gauge the level at which the corpus is written. This distinguishes the case of disciplined solo LLM-driven development from the case of absent architectural control.

---

## 5. Pipeline threat model

### 5.1 The risk class of broad-permissions agents

Modern agentic coding tools often follow a "single agent with broad permissions" model: shell, filesystem, network, messengers, sometimes email and calendar. Added to this is a heartbeat mode of autonomous operation and a community-extensions repository with minimal security review.

This model creates a risk class: prompt injection through any incoming channel (email, message, web page, document) executes arbitrary actions on behalf of the user. The attacker has no direct access to the victim's machine, but has access to a channel the agent reads, and that is enough.

A representative example is OpenClaw, the open-source agentic AI assistant by Peter Steinberger (originally Clawdbot in November 2025; renamed Moltbot on January 27, 2026 after Anthropic trademark concerns, then to OpenClaw). At the time of this document's publication, publicly confirmed security incidents are on record.

**Cisco AI Security Research analysis (ClawHub).** Cisco AI Defense analyzed the "What Would Elon Do?" skill from the ClawHub community repository and found nine vulnerability classes; the skill turned out to be functionally malicious — it performed silent data exfiltration via embedded `curl` to attacker-controlled servers and used prompt injection to bypass safety guidelines. Across a broader sample, Cisco published an estimate of ~230 malicious skills in the catalog. In response, Cisco released an open-source Skill Scanner and announced DefenseClaw (released March 27, 2026).

**Maintainer warning.** One of the OpenClaw maintainers, known in the project's Discord under the handle Shadow, publicly stated: "if you can't understand how to run a command line, this is far too dangerous of a project for you to use safely." This is a warning from an ecosystem insider, not an outside critic.

**MoltMatch (February 2026).** The OpenClaw agent of 21-year-old user Jack Luo created a dating profile on the experimental MoltMatch platform without explicit instruction from its owner and screened candidates on his behalf. Luo publicly commented that the AI-generated profile "doesn't really show who I actually am, authentically." In a separately related case, AFP established that one of MoltMatch's popular profiles used photographs of Malaysian model June Chong without her consent — she reported that she does not use dating apps and has no AI agent.

**Restrictions in the PRC (March 2026).** Bloomberg reported on March 11, 2026 that government agencies, state-owned enterprises, and the largest banks had received directives not to install OpenClaw on office devices. CNCERT (National Cyber Security Emergency Response Team) named four risk classes, including operational errors from misinterpreted instructions and installation of malicious plugins with data theft. The PRC Ministry of State Security additionally cited the risk of agents being used to spread disinformation and fraud. On March 23, 2026 the regulator published a list of best practices for users, companies, and cloud providers.

Sources for independent verification: [OpenClaw Wikipedia article](https://en.wikipedia.org/wiki/OpenClaw); Cisco AI Defense publications (search: "Personal AI Agents like OpenClaw Are a Security Nightmare", Cisco blog); Bloomberg "China Moves to Limit Use of OpenClaw AI at Banks, Government Agencies" (March 11, 2026); Taipei Times (February 14, 2026) and AFP on the MoltMatch incident.

### 5.2 Pipeline structural defense

The architect-executor split (§2.1) structurally avoids the broad-permissions agent risk class through minimal permissions per session mode, not through «we will be careful». The defense is invariant across boundary types; specific mechanisms vary by boundary type.

**Deliberation sessions** (architect mode). Chat interface, no autonomous tool loop. Every tool invocation is initiated by a human turn. MCP connectors are connected explicitly per session and visible at every moment in the session UI. Read-only access is default; write actions surface explicit confirmations. No heartbeat mode, no scheduled work, no background actions.

**Execution sessions** (executor mode). Claude Code agent, autonomous tool loop within session scope. Tool access bounded by authored brief (Phase 0 reads + Phase N execution plan + closure criteria). Session ends when brief closes; agent does not persist across sessions. Filesystem access is scoped to the repository working directory; network actions are gated by brief specification. MCP tools are enabled per session, not globally.

**No persistent agent state between sessions.** Each session starts with clean context. Cross-session coordination happens through the repository (LOCKED docs, briefs, amendment plans, commit history) — not through agent memory or background state.

**Session-mode discipline as structural barrier.** Deliberation mode does not apply changes; if architectural deliberation requires mechanical changes to verify a hypothesis, that is a separate execution session against an authored brief. Execution mode does not make architectural decisions; the agent escalates on ambiguity per §3 stop/escalate/lock. The split is procedural — but the **briefs themselves** are the structural mechanism: the agent cannot do work outside brief scope without escalation.

v1.x era equivalent defense (model-tier boundary, N=4) operated under the same invariant principle through different mechanisms: local executor (Gemma) had no network access; prompt generator and architect (cloud sessions) had no heartbeat mode; no direct channel between the four agents. Empirical defense properties preserved across boundary type — the **principle** of minimal-permissions-per-role-via-structural-mechanism is invariant.

### 5.3 Falsifiable claims

Specific attack classes that are impossible in the v1.6 pipeline for structural reasons, not «carefulness»:

1. **Out-of-session autonomous actions are impossible** — no session mode includes scheduled work, heartbeat, or persistent background agent. All sessions are human-initiated with explicit scope.

2. **Cross-session agent compromise propagation is impossible** — no persistent agent state between sessions. Compromise of one session does not infect the next; only the repository carries forward, and the repository is human-reviewable.

3. **Out-of-brief execution is impossible** — execution agent operates against authored brief. Brief scope is explicit (Phase 0 reads, Phase N execution plan, closure criteria); the agent escalates on ambiguity rather than improvise.

4. **Installation of malicious extensions is impossible** — pipeline has no community-extensions repository with automatic installation. MCP connectors are connected explicitly per user action, visible per session.

5. **Architect-executor compromise crosstalk is impossible** — boundary type is session-mode; one session's compromise does not propagate to the other through a direct channel. Only the repository is shared, and repository state is human-auditable through git history.

These claims are verified by inspecting the pipeline architecture and do not depend on the behavior of individual models. Per Q-A07-4=γ, they survive any boundary-type change: if the pipeline restructures to a mixed boundary type or multi-model architecture, the structural defense reformulates per the new boundary type; the **principle** of minimal-permissions-per-role-via-structural-mechanism is invariant.

v1.x era equivalent claims (model-tier boundary, 4-agent) — see [PIPELINE_METRICS §1](./PIPELINE_METRICS.md#1-pipeline-configuration) historical record.

---

## 6. Boundaries of applicability

### 6.1 Where the methodology works

The methodology works for projects that simultaneously satisfy the following conditions:

- **Explicit architecture.** Formal interfaces, types, and protocols exist through which subsystems communicate. They can be fixed in code and checked mechanically.
- **Testable correctness.** Behavioral correctness is verified through unit tests, integration tests, and static analysis. The "good/bad" criterion is objective.
- **Long horizon.** The accumulating structure (documentation, decision corpus, commit history) becomes an asset. On a short horizon, the overhead of building this asset does not pay off.

Concrete system classes: compilers, engines, frameworks, libraries, infrastructure services, systems software.

### 6.2 Where the methodology does not work or works worse

- **Exploratory research without a target architecture.** When it is unclear in advance which system we are building, contract formalization is impossible — there is nothing to formalize.
- **Creative work with subjective criteria.** Game design, narrative, balance, art direction. The LLM can suggest options, but verification stays with the human and with playtests.
- **Narrow domain knowledge absent from the LLM.** If a task requires understanding of a specialized industry not covered by training data, the methodology does not help.
- **Systems with critical requirements on development cadence itself**, where the "build + tests" cycle is itself too slow for iterative work.

### 6.3 Typical objections and their falsifiable form

"Too many contracts for an empty project" and "too much documentation for a 5-day project" are objections from the YAGNI heuristic, which applies to development without an LLM pipeline. In the pipeline, contracts function as a formal grammar for the executor and documentation as prompt context. "The cadence is too high, so the code is bad" reduces to a checkable condition: if on a 6+ month horizon the cadence stays high with a low defect rate, the criticism is refuted; if the defect rate rises, it is confirmed.

---

## 7. Operating principles

### 7.1 Data exists or it doesn't

The project's central operating principle, applied at every layer: **state and data either have a real grounding artifact in the codebase, or they are removed.** There is no third option of "we will add the artifact later, and the placeholder represents what it will become." Placeholder state is a lie about what the system actually is, and lies in code compound across LLM-driven cycles.

The principle has been formally invoked at least seven times during the M7 batch, the pre-M8 technical-debt closure, and the A'.4.5 governance layer:

1. **Real pawn data** ([9141bd6](https://github.com/Crystalka228/Dual-Frontier/commit/9141bd6), M7 H3). Hardcoded Warhammer-flavored pawn names, hash-derived role labels, and hash-derived skill bars were removed in favor of `IdentityComponent`, `RandomPawnFactory`, and real `SkillsComponent` data. Eight dead files were deleted alongside the replacement (four stub UI components, one stub node, three undispatched bridge commands).

2. **Needs decay direction** ([ee12108](https://github.com/Crystalka228/Dual-Frontier/commit/ee12108), M7 H4). Decay-toward-zero was a placeholder lie implying automatic recovery while no module actually closed needs. The sign was flipped to deficit accumulation; the XML doc was rewritten to honestly describe ungrounded depletion.

3. **ModMenuPanel position** ([5f0b4f5](https://github.com/Crystalka228/Dual-Frontier/commit/5f0b4f5), M7 H5). The modal misposition surfaced during F5 — the centered modal was visually unreachable. The fix converted Control to CanvasLayer per the Phase 4 UI pattern; this was a structural correction, not a tweak to coordinates.

4. **Asset gitignore companion** ([805b882](https://github.com/Crystalka228/Dual-Frontier/commit/805b882), M7 H5). Extracted Kenney + Cinzel folders are derived state; the source `.zip` files are the in-git source of truth. Tracking both extracted folders and source archives would have been redundancy masquerading as honesty.

5. **Menu pauses simulation** ([9f87536](https://github.com/Crystalka228/Dual-Frontier/commit/9f87536), M7 H6). Two independent pause flags (`_isRunning` for Apply safety, `_paused` for tick advance) where `BeginEditing` only toggled the former. The orchestration layer was made to wire both lockstep; F5 verification confirmed.

6. **Needs semantic flip** ([f4a5839](https://github.com/Crystalka228/Dual-Frontier/commit/f4a5839), TD-3.1). The storage convention "0 = full, 1 = starving" combined with consumer expectations of wellness ("1 = best") forced a translation layer (`1f -` inversions in `MoodSystem` and `PawnStateReporterSystem`) that hid the mismatch. After the flip, the inversion logic disappeared naturally, because storage now matched consumer semantics. The principle did the work it is supposed to do.

7. **Documentation layer — every document either has a register entry or is out-of-scope** (A'.4.5 closure, 2026-05-12). Pre-A'.4.5 the principle applied to code (production data sources, placeholder fields) but not to documentation governance: a `.md` file could exist in the repository without any structural surface declaring its category, tier, lifecycle, or ownership. This permitted a class of placeholder lie at the documentation layer — e.g., `PHASE_A_PRIME_SEQUENCING.md` carrying status «A'.0.7 NEXT» after A'.0.7 closure completed (no mechanism enforced update); `INVENTORY.md` carrying baseline ~135 files when actual count had drifted to ~220 (no mechanism flagged drift). A'.4.5 introduces `docs/governance/REGISTER.yaml` as the structural surface: every `.md` file either has a register entry (with category × tier × lifecycle declared) or matches a pattern in `tools/governance/SCOPE_EXCLUSIONS.yaml`. There is no third option. `sync_register.ps1 --validate` enforces the invariant; the post-session update protocol (Q-A45-X5, §12.5) closes the «I forgot to update X» failure mode at every closure.

The principle constrains all participants in the pipeline (§2.1): the architect cannot author a brief that references data sources absent from disk; the executor cannot synthesize fields whose backing data is absent from the brief; the QA review at phase closure requires that every artifact be auditable against real existence; the direction owner cannot hide behind «we will fill it later» because the next session in the pipeline will refuse to operate against a stub. Each application of the principle is a moment where one role's output would have introduced a placeholder lie if not constrained.

**Falsifiable claim.** The track record across cycles is the falsifiable signal of whether the principle is load-bearing. As of A'.4.5 closure: seven applications, zero counter-examples — no case where a placeholder was deliberately preserved as a "we will fill it later" stub at either code or documentation layer. Forward target: the count continues to grow without counter-examples through M8–M10. A counter-example — a placeholder that survives a closure review, OR a `.md` file that ships without register entry (and is not in `SCOPE_EXCLUSIONS.yaml`) — would falsify the principle's load-bearing role and would force a methodological retraction.

---

## 8. Reproducibility

### 8.1 Minimal configuration

To reproduce the pipeline:

- **VS Code** with the **[Cline](https://github.com/cline/cline)** extension — orchestrator for the local model.
- **LM Studio** or a compatible backend for the local model via an OpenAI-compatible API on `localhost`.
- Local model: **Gemma 4 E4B** (Q4_K_M) or comparable Qwen 2.5 Coder, Llama 3.1 8B, Mistral Nemo 12B alternatives. Minimum 8 GB VRAM or 16 GB unified memory (Apple Silicon).
- **Claude Max 5×** subscription ($100/month) and the Claude desktop app for architectural work through Sonnet 4.6 and Opus 4.7.

### 8.2 Confirmed configuration

The pipeline runs stably under sustained development (4–5 hours per evening after a work shift) on the following hardware:

- ASUS TUF Gaming A16 (system name "Skarlet")
- AMD Ryzen 7 7435HS @ 3.10 GHz
- AMD Radeon RX 7600S 8 GB
- 32 GB DDR5 4800 MT/s
- Windows 11 Home 25H2

### 8.3 Process discipline

Beyond tools, the methodology requires the following discipline:

- **Contracts are declared formally** through interfaces, attributes, and types. Every architectural decision has a machine-checkable satisfaction condition.
- **Tests are written as invariant proofs**, not as checkboxes. Each test verifies a specific claim, not "the system works as a whole."
- **Commits are atomic** with a scope prefix (`feat(scope): ...`, `fix(scope): ...`, `docs: ...`, `refactor(scope): ...`). After every commit the project is in a working state.
- **`TreatWarningsAsErrors` is enabled** from the start. Technical debt does not accumulate invisibly.
- **Documentation is written in parallel with the code**, not after. After each substantial phase closes: update the architectural documents and run the self-teaching ritual (§4.5).
- **Phase reviews by the architect** run as atomic sessions: architecture, implementation, tests, debugging, and documentation in one pass (§2.4).

Without this discipline, the tools yield a worse result than without them.

---

## 9. Open questions

The methodology has been tested on a 5-day horizon with one formalized phase-review session. Several questions need further investigation.

**Long-term dynamics.** All falsifiable conditions cited are tested on a 5-day horizon. Pipeline behavior over 6–12 months is unknown. Does the cadence persist? Does defect rate grow? Does self-teaching get harder as the corpus grows? These questions require systematic metric collection across Phases 5–9.

**Drift of the developer's architectural understanding.** If the author goes years without writing code by hand, does their architectural understanding persist? The self-teaching ritual (§4.5) is meant to prevent this drift, but the ritual's effectiveness has not been measured quantitatively.

**Applicability to team development.** Every argument here applies to solo development. Whether formal contracts suffice as IPC between teams, or whether additional methodological devices are needed, is an open question with its own literature.

**Degradation as the codebase grows.** Session context windows place an upper bound on architectural-deliberation surface and on executor brief scope. With further project growth, the methodology may need restructuring: splitting the corpus into modules with independent context, switching to larger context windows, using retrieval instead of fully loaded context, or moving к multi-session deliberation/execution flows where one session can no longer hold the relevant surface. v1.x era hit this constraint first at 131k tokens (local Gemma, `Implement ItemAddedEvent` task в Phase 4 reached 132.9k); v1.6 era operates под higher ceiling (Claude Desktop Max 5× subscription session context) but the same fundamental constraint applies — context window is finite, codebase scope grows.

**Behavior on series of negative results.** Dual Frontier has one publicly recorded negative result ([NATIVE_CORE_EXPERIMENT.md](/docs/reports/NATIVE_CORE_EXPERIMENT.md)). The pipeline handled it correctly as a methodological artifact. But that is one case. The pipeline's systematic behavior on series, when several architectural hypotheses in a row prove wrong, has not been tested.

**Reproducibility by other developers.** The pipeline has been tested by one author on one project. Reproducibility by other developers with different backgrounds and on different tasks has not been tested. This is the most important open question: if the pipeline reproduces only for the author and only on this project, the methodology becomes a private finding rather than a method.

---

## 10. Change history

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-04-25 | First public version of the document after Phase 4 closure. |
| 1.1 | 2026-05-03 | Added §7 Operating principles, with §7.1 stating the "data exists or it doesn't" principle and its empirical record. Subsequent sections renumbered (§8 Reproducibility, §9 Open questions, §10 Change history, §11 See also). |
| 1.2 | 2026-05-07 | Expanded "Native layer methodology adjustments" section with descriptive vs prescriptive pre-flight principle, derived from K0 closure lesson. Brief-authoring checklist added. |
| 1.3 | 2026-05-07 | Post-K1 lessons added to «Native layer methodology adjustments»: ABI boundary exception completeness (throws и their boundary catches are inseparable; brief must enumerate explicitly) and brief authoring as prerequisite step (brief is its own commit, performed before execution begins). |
| 1.4 | 2026-05-07 | Post-K3 calibration lesson added к «Native layer methodology adjustments»: brief time estimates from architectural docs assume hobby pace (~1h/day manual typing); auto-mode execution actual time is 5-10x faster. Future briefs must state both hobby-pace и auto-mode estimates explicitly. K0-K3 measured data: 11-17 days hobby estimate vs ~6 hours actual auto-mode. |
| 1.5 | 2026-05-09 | Added "Pipeline closure lessons (K-series, post-K8.1)" sub-section under "Native layer methodology adjustments" with three lessons formalized from K8.1 and K8.1.1 closures: atomic commit as compilable unit (per K8.1 Phase 5 dependency-cycle bundling, `a62c1f3..059f712`), Phase 0.4 inventory as hypothesis (per K8.1 `Marshalling/` layout reconciliation), mod-scope test isolation (per K8.1.1 Stop condition #3 fix on `EqualsByContent_StaleGeneration_ReturnsFalse`, `fc4400d..63777ef`). |
| 1.6 | 2026-05-10 | Pipeline restructure rewrite per A'.0.7 — §0 Abstract generalized к architect-executor abstract framing; §2.1 role distribution rewritten в abstract role categories с v1.6 current-configuration table; §2.2 contracts as IPC reframed across context boundaries с three-properties mechanism; §3 economics restructured к invariant + current-configuration с A'.0.5 empirical anchor; §4 throughput parallel-form case studies (Phase 4 closure v1.x + A'.0.5 closure v1.6); §5.2/§5.3 threat model restructured для v1.6 session-mode reality; §9 «degradation as codebase grows» reformulated к pipeline-agnostic; methodology corpus declared agent-as-primary-reader per Q-A07-6. K-Lessons sub-section expanded с A'.0.5 lesson «milestone consolidation under session-mode pipeline» per Q-A07-5. |
| 1.8 | 2026-05-14 | A'.5 K8.3+K8.4 closure — Lesson #7 (brief prescribing API must transcribe API) + Lesson #8 (brief splitting change into N steps must prove each of N-1 intermediate states is valid) added to K-Lessons sub-section per brief v2.0 §9.4. Both lessons originated in the K8.3+K8.4 combined milestone authoring (v1.0 → patch v1 → v2.0 arc) and were formalized at A'.5 closure. (Note: A'.5 closure added these lessons in-place but did not bump frontmatter version; frontmatter bump к v1.8 actually landed at К10 deliberation S6 lock 2026-05-17 per row below.) |
| 1.7 | 2026-05-12 | Document Control Register integration per A'.4.5 closure. New §12 specifies the register as governance authority, classification model (Category×Tier×Lifecycle), seven sections, post-session update protocol (Q-A45-X5), and canonical closure protocol §12.7. §7.1 «Data exists or it doesn't» extended with seventh formal invocation (documentation layer: every `.md` either has a register entry or is in `SCOPE_EXCLUSIONS.yaml`; no third option). §11 «See also» extended with [FRAMEWORK](../governance/FRAMEWORK.md) + [SYNTHESIS_RATIONALE](../governance/SYNTHESIS_RATIONALE.md) links. |
| 1.8 | 2026-05-17 | К10 deliberation S6 lock — Lesson #11 (architectural reduction methodology, 6 strong applications + Q-G-2 composite namespace precedent) + Lesson #20 (tactical heuristics in research framework are category error) + Lesson #22 (read existing code + ask operational context before surfacing architectural concerns) added to «Phase A' lessons» sub-section. NEW «Provisional Lessons» section captures 9 candidates (#9, #10, #14, #15, #16, #17, #18, #19, #21) pending promotion at К-closure report (А'.8) per accumulated evidence. Frontmatter version bumped к 1.8 in this commit (back-fills A'.5 row above). |
| 1.9 | 2026-05-21 | A'.7.x К-extensions cascade #0 closure — §12.7 closure protocol step 1 expanded с explicit per-test-suite checklist (Core + Modding suite both mandatory) per CAPA-2026-05-21-A_PRIME_7_X-K10_3-V2-SOFT-HALT corrective action (c). К10.3 v2 verification #7 soft-halt root cause closure-protocol gap, retroactively closed by А'.7.x. Lessons batch deferred к A'.8 closure (v1.9 → v1.10). К-extensions cascade #0 designation (А'.7.x) does not change §1-§11 substance. |
| 1.10 | 2026-05-23 | A'.8 К-series formal closure — Lessons promotion batch per K_CLOSURE_REPORT.md §6. 12 FORMALIZE (10 carried Session 1 LOCKED Q4 + #14 PROMOTED + #27 PROMOTED at A'.8 per Q-N-8-5 LOCKED). 9 DEFER candidates Provisional pool (3 carried + 5 А'.7.x surfaced + 1 Godot removal surfaced). 1 SUNSET (Lesson #15 subsumed by Lesson #20). К-L14 falsifiability criterion 6 (soft-halt rate) added as Provisional per Q-N-8-7 LOCKED. К-closure report K_CLOSURE_REPORT.md authored Tier 1 AUTHORED per Q-N-8-4 amendment к Meta-Q1. §1-§11 substance unchanged. |

The document is updated after each substantial phase closes. Substantial methodological shifts (changes to pipeline configuration, changes to role distribution, additions or removals of methodological devices) are recorded as major versions.

---

## 11. See also

- [README](/README.md) — research framing, falsifiability conditions, and pointers to operational data.
- [PIPELINE_METRICS](./PIPELINE_METRICS.md) — empirical configuration, throughput data, and subscription economics measured while running this methodology.
- [PHASE_1](/docs/learning/PHASE_1.md) — self-teaching ritual artifact after Phase 1; direct empirical referent for §4.5.
- [SESSION_PHASE_4_CLOSURE_REVIEW](/docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md) — Phase 4 closure review session log; direct empirical referent for §4.4. *(Russian-language audit trail; preserved verbatim per the i18n campaign rules.)*
- [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md) — layers, dependency rules, scenarios.
- [CONTRACTS](/docs/architecture/CONTRACTS.md) — the contract system, six domain buses, contract evolution.
- [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) — hygiene checklist for every PR, the engine/game boundary.
- [ISOLATION](/docs/architecture/ISOLATION.md) — the isolation guard, types of violations, DEBUG vs RELEASE.
- [NATIVE_CORE_EXPERIMENT](/docs/reports/NATIVE_CORE_EXPERIMENT.md) — negative result of the C++ core, criterion reframing.
- [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) — **v1.0 LOCKED.** Unified Vulkan substrate (V) covering rendering + compute use cases per Q-G-1 LOCK; consolidates former RUNTIME_ARCHITECTURE.md v1.0 + GPU_COMPUTE.md v2.0. Substrate primitives V0 (foundation) / V1 (diffusion) / V2 (wave) per Q-G-2; M-V demonstrations per Q-R-1 format. Phase 3 `ProjectileSystem` deferral preserved as Domain B special case with substrate disposition TBD at M-V5 amendment.
- [ROADMAP](/docs/ROADMAP.md) — phases, dependency reasoning, the bridge pattern between Phases 5 and 6.
- [FRAMEWORK](../governance/FRAMEWORK.md) — Document Control Register governance framework v1.0 (LOCKED at A'.4.5). Governance authority over documentation lifecycle, classification, post-session protocol.
- [SYNTHESIS_RATIONALE](../governance/SYNTHESIS_RATIONALE.md) — Source-standard provenance for the register synthesis (DO-178C / ISO 9001 / ISO 26262 / IEC 61508 / FDA 21 CFR Part 11; 9 selected + 11 deselected elements).
- [REGISTER.yaml](../governance/REGISTER.yaml) — Operational SoT for the Document Control Register; 229 documents enrolled at A'.4.5 closure with classification + cross-references.

---

## 12. Document Control Register integration

A'.4.5 closure (2026-05-12) introduced the Document Control Register at `docs/governance/REGISTER.yaml` with framework specification at [FRAMEWORK.md](../governance/FRAMEWORK.md) v1.0 LOCKED. This section integrates the register's governance authority into the methodology corpus.

### 12.1 What the register is

The Document Control Register is the project's **operational navigation surface** for documentation governance — a YAML-format Source of Truth tracking every `.md` file's classification (category × tier × lifecycle), ownership, versioning, audit cadence, and cross-references. Three-tool PowerShell suite at `tools/governance/` provides write-side sync + validation (`sync_register.ps1`), read-side queries (`query_register.ps1`), and human-readable rendering (`render_register.ps1`).

The register is **agent-primary** by design (per Q-A07-6 audience contract inheritance, §0 v1.6 footnote). Human-readable rendering (`REGISTER_RENDER.md`) is derivative, auto-generated from the YAML SoT.

### 12.2 Why it exists — failure modes the register prevents

Pre-A'.4.5 the project accumulated five categories of governance-tracking failure:

1. **Stale referencing documents** — `PHASE_A_PRIME_SEQUENCING.md` carried «A'.0.7 NEXT» status after A'.0.7 closure completed; no mechanism enforced update of referencing documents on milestone closure.
2. **Inventory drift** — `INVENTORY.md` baseline ~135 files (A'.0.5 closure 2026-05-10); actual count ~220 by A'.4.5 deliberation pre-flight 2026-05-12. No mechanism tracked drift between snapshots.
3. **Unknown unknowns at closure** — agent executing milestone had no completeness check for «what governance bookkeeping was missed»; mental model lived in Crystalka's head between sessions.
4. **Cross-document drift between LOCKED specs** — amendment plans for one spec (K-L3.1, A'.0.7) might not propagate consistently across all referencing documents; surfaced as RISK-004.
5. **«I forgot to update X» pattern** — routine bookkeeping omissions at closure protocol; no structural surface to enforce per-milestone updates.

The register closes all five failure modes structurally. See [FRAMEWORK.md](../governance/FRAMEWORK.md) §2 for full enumeration.

### 12.3 Classification model — Category × Tier × Lifecycle

Three orthogonal axes per FRAMEWORK §3:

- **Category** (A–J, ten values) — content type: Architecture (A) / Methodology (B) / Live tracker (C) / Brief (D) / Discovery+closure+audit (E) / Module-local (F) / Project meta (G) / i18n (H) / Ideas Bank (I) / Game Mechanics (J).
- **Tier** (1–5) — governance regime: Architectural authority (T1) / Operational live state (T2) / Milestone instruments (T3) / Module-local (T4) / Speculative (T5).
- **Lifecycle** (8 states) — control state: Draft / Live / LOCKED / EXECUTED / AUTHORED / DEPRECATED / SUPERSEDED / STALE.

Not fully orthogonal — allowed-combinations matrix encoded in FRAMEWORK §3.4 + validated by `sync_register.ps1`. Forbidden combinations require `special_case_rationale` field populated.

This methodology document is `DOC-B-METHODOLOGY` — Category B (Methodology), Tier 1 (Architectural authority), Lifecycle LOCKED, version (this revision) 1.7.

### 12.4 Seven register sections

Per FRAMEWORK §4, the register has seven sections answering distinct agent query patterns:

1. **Document Control** (per-document) — ID, path, title, classification, ownership, version, audit cadence
2. **Architecture Case** (per-document) — decisions anchored + rationale_anchors (which brief justifies which K-Lxx invariant)
3. **Requirement → Test Traceability** (global) — K-Lxx, M-Lxx, G-Lxx, Q-lock requirements + verifying tests + verification status
4. **Architectural Risk Analysis** (global) — risk taxonomy by Type × Status × Likelihood × Impact + mitigation
5. **Internal Audit Schedule** (per-document + global calendar) — per-tier cadence enforcement, STALE flag
6. **Corrective Action Log (CAPA)** (global) — formal 5-field problem→solution shape for milestone-driven amendments
7. **Audit Trail** (global) — chronological governance-significant events (milestone closures, amendment landings) referencing git log

### 12.5 Post-session update protocol (Q-A45-X5 lock)

**Most load-bearing element of A'.4.5.** Every agent execution session that closes a milestone OR makes architectural decisions OR modifies any register-scope document **must** execute the post-session update protocol before commit closure:

```
1. Identify all documents modified during session (git status / git diff)
2. For each modified document: update register entry (last_modified, last_modified_commit,
   lifecycle if transitioning, governance_events if applicable). New documents: create entry.
   Renamed/moved/deleted: update path / supersession / deprecation cross-references.
3. If milestone is closure event: append audit_trail entry (EVT-{date}-{symbolic}),
   update affected documents' governance_events lists, create CAPA entry if opened,
   update risks collection, create requirement entries if any added.
4. Run sync_register.ps1 --validate
5. If validation passes: include REGISTER.yaml updates in milestone closure commit.
6. If validation fails: halt closure; surface errors.
```

Strict gate: validation must pass before final commit. Bypass via `git commit --no-verify` available but logged in `BYPASS_LOG.md` (Tier 2 Live tracker). Accumulated unresolved bypasses surface as governance hygiene debt.

Strict-vs-advisory split: cross-reference integrity violations, schema integrity violations, missing terminal-state references, and document-changed-without-register-update all block commit. STALE flag generation, translation candidacy warnings, brief-AUTHORED-aging warnings are advisory only.

### 12.6 Falsifiable claims

Per FRAMEWORK §10, the register is falsified if any of:

1. Navigation-aid claim fails — task-level metrics in PIPELINE_METRICS measurably degrade post-register
2. Decade-horizon scalability fails — disruptive MAJOR schema bumps every 6 months (vs anticipated 1-2 MINOR + occasional MAJOR in first year then stabilization)
3. Construction-by-rationale fails — false positives/negatives at rate requiring constant `--no-verify` bypass; BYPASS_LOG.md grows monotonically without cleanup
4. Post-session protocol staleness — sync_register --validate run yields STALE flags on >10% of Tier 1 documents at any sampled point
5. Cross-tier transitions blocked — `special_case_rationale` overrides accumulate to 20%+ of register entries, signaling allowed-combinations matrix is wrong

Falsification mechanism: PIPELINE_METRICS records per-milestone metrics; quarterly audit reviews register efficiency. If falsified, A'.4.5.X micro-milestones or A'.9 analyzer milestone re-deliberates.

### 12.7 Closure protocol — canonical post-A'.4.5

The closure protocol previously documented in `MIGRATION_PROGRESS.md` is extended for register integration. **This is the canonical version**; MIGRATION_PROGRESS.md preserves its historical version for reference but cross-references this section as authoritative:

```
1. Run final verification per-test-suite (v1.9 expansion: every test project, not just Core):
   - dotnet build -c Release <sln> → 0 warnings, 0 errors
   - cmake --build <native> --config Release → clean
   - df_native_selftest → ALL PASSED
   - dotnet test tests/DualFrontier.Core.Tests/ -c Release → all PASS (incl. Stress filter sweep
     if cascade touches stress harness)
   - dotnet test tests/DualFrontier.Core.Interop.Tests/ -c Release → all PASS
   - dotnet test tests/DualFrontier.Application.Tests/ -c Release → all PASS
   - **dotnet test tests/DualFrontier.Modding.Tests/ -c Release → all PASS** (MANDATORY per
     v1.9 — closures that touch ModIntegrationPipeline, mod_unload native, capability registry,
     fixture-mod copy, OR ANY shared interop surface that Modding-suite fixtures consume MUST
     exercise this suite explicitly; «we already ran Core» is not sufficient evidence)
   - Manual F5 verification → application starts cleanly, no regressions
2. Atomic commit with scope prefix (existing): feat(scope) / fix(scope) / docs / refactor(scope)
3. Update MIGRATION_PROGRESS.md (existing): closure entry recording outcomes, commits, deviations, lessons
4. Update brief Status field (existing): AUTHORED → EXECUTED transition with closure commit reference
5. NEW — Update REGISTER.yaml entries for all documents touched during milestone:
   - last_modified, last_modified_commit, lifecycle (if transitioning), governance_events
   - New documents: create entries with required fields populated
   - Renamed/moved/deleted: update path; update all cross-references
6. NEW — Append audit_trail entry for milestone closure (EVT-{date}-{symbolic-event}):
   - documents_affected list, commits range + key_commits, governance_impact narrative
   - cross-references to CAPA entries (if opened), lifecycle transitions
7. NEW — Run sync_register.ps1 --validate:
   - Must exit 0 before final commit
   - Bypass available via --no-verify but logged in BYPASS_LOG.md
8. NEW — If REGISTER.yaml or any meta-entry modified during milestone:
   - Final commit incorporates REGISTER.yaml updates
   - Bootstrap pattern for self-referential `last_modified_commit` field per FRAMEWORK §8.3
```

The new steps (5-8) constitute the post-session update protocol per §12.5. They are strict by default (validation blocks commit); bypass mechanism (§12.5) provides explicit-and-logged escape hatch when register state cannot be reconciled within the closing session.

**v1.9 expansion rationale**: К10.3 v2 closure (2026-05-20) ran the Core suite + native selftest + F5 verification but did not exercise the Modding suite explicitly. A transient fixture-copy build-state issue (missing Fixture.RegularMod_* / Fixture.PublisherMod manifest artifacts в `tests/DualFrontier.Modding.Tests/bin/Release/net8.0/Fixtures/`) was therefore not caught at closure ratification time. Crystalka's А'.7.x BUS_DESIGN_INVESTIGATION 2026-05-21 surfaced the resulting 14 fails (M51 ×4 + M52 ×3 + M62 ×5 + M73 ×2). A'.7.x Phase 0 «Pre-flight B» repro on the same branch с the same WT changes но a freshly-rebuilt Fixtures/ tree showed 0 fails — the build state, not the code, was the regression. К-L14 verification #7 was initially mis-annotated as a soft-halt against K10.3 v2's K-L18 amendment (Hypothesis 1 в А'.7.x brief §8.3); the closure-protocol gap is the actual root cause, не a К-L18 regression. The new step 1 wording explicitly enumerates Modding suite to prevent recurrence (CAPA-2026-05-21-A_PRIME_7_X-K10_3-V2-SOFT-HALT corrective action (c)).



Kernel и runtime native layers introduce specific methodology adjustments documented в:
- `KERNEL_ARCHITECTURE.md` Part 7 (kernel-specific adjustments)
- `VULKAN_SUBSTRATE.md` Part 7 (runtime-specific adjustments)

Common adjustments (apply к both, plus any subsequent native artifact per `MIGRATION_PROGRESS.md` D3 Lvl 1 pattern):
- C++ build verification mandatory pre-commit (CMake clean + selftest passing)
- P/Invoke marshalling check на every new `[DllImport]` declaration
- Cross-language commits acceptable when C++ + C# changes coupled
- Single ownership boundary preserved across managed/native communication

### Pre-flight checks: descriptive over prescriptive

Briefs для milestone execution include pre-flight checks — verifications agents выполняют before touching code. The temptation is to write these prescriptively: «выполни команду X, **должна вернуть Y, иначе STOP**». This pattern fails when the brief's expectation diverges from reality on the ground.

**Failure mode (observed at K0 closure, 2026-05-07).** K0 brief gated execution on:

```
git merge-base origin/main claude/cpp-core-experiment-cEsyH
Ожидаем: NO OUTPUT, exit code 1 — disjoint histories
Если выдаёт SHA — STOP
```

The expectation was based on `CPP_KERNEL_BRANCH_REPORT.md` Discovery (~2 weeks old at brief authoring). Reality: the experimental branch shared a base with main at commit `3e9a001`. A strict agent following the brief would have STOPPED on a false signal — the cherry-pick procedure works identically whether histories are disjoint or share a base, so the gate was protecting nothing.

**Principle: pre-flight checks describe state, не prescribe outcome.** Agents выполняют the command, **record what they see**, and only STOP on **destructive divergence** (working tree dirty, target paths already populated с unexpected content, baseline tests failing). Informational checks (history shape, branch topology, recent commit metadata) are recorded но never block execution alone.

**Two categories of pre-flight check** in any future brief:

1. **Hard gates** (STOP-eligible) — failure indicates the workspace would be corrupted by proceeding:
   - Working tree dirty (uncommitted changes would be lost)
   - Baseline tests failing (regression source not yet identified)
   - Required tooling absent (CMake / dotnet / compiler)
   - Target paths already contain unexpected content (collision risk)
   - Permissions or filesystem write failures

2. **Informational checks** (record-only) — describe environment for the audit trail:
   - History shape (merge-base existence, branch topology)
   - Commit metadata (HEAD SHAs, commit counts, author dates)
   - Discovery-report-derived facts that may have aged
   - Architecture-document-derived facts that may have evolved

When a brief's prediction diverges from observed reality:
- If it's an **informational check** — record the divergence, continue.
- If it's a **hard gate** — STOP, escalate to human, await guidance.

**Rationale**: brief authoring happens at one point in time; brief execution happens later. Reality between those moments shifts (commits land, branches move, Discovery reports age). Hard-gating on every observed state ties brief correctness to instantaneous knowledge — a fragile coupling. Informational checks decouple the two: brief stays useful even when its expectations age.

**Falsifiable claim**: Briefs authored under the descriptive principle complete с fewer false-stop interruptions than briefs with prescriptive gates. The measurement: count brief-execution sessions that STOP on informational-check divergence, before и after this principle adoption. Target: zero false stops on informational divergences from K1 onward.

### Brief authoring checklist (post-K0 update)

When authoring a milestone brief, separate pre-flight checks по category:

- [ ] Hard gates listed explicitly с STOP language
- [ ] Informational checks listed с «record и continue» language
- [ ] Discovery-report-derived facts marked как informational
- [ ] No prescriptive gate based на architecture document state alone (architecture LOCKs are not pre-flight checks; они are brief inputs)
- [ ] Defensive `Test-Path` / existence checks used over assumed-state assertions (K0 cleanup commit 3 precedent — `NATIVE_CORE.md` may or may not exist; brief covers both)

### ABI boundary exception completeness

When a brief introduces new throws into native code, the brief MUST explicitly enumerate ALL `extern "C"` boundary points through which those throws can propagate, и require each to be wrapped в try/catch. This is не optional cleanup — uncaught C++ exceptions across DLL boundaries are undefined behavior, manifesting as process termination, silent corruption, or platform-specific miscompiles depending on toolchain.

**Failure mode (observed at K1 closure, 2026-05-07).** K1 brief instructed adding `throw std::logic_error(...)` in `World::add_component`, `remove_component`, `destroy_entity`, `flush_destroyed` when spans active. The brief specified try/catch only for the **new** ABI functions (`df_world_add_components_bulk`, etc.). The **existing** wrappers — `df_world_add_component`, `df_world_remove_component`, `df_world_destroy_entity`, `df_world_flush_destroyed` — also propagate these new throws, but their try/catch was implicit, not explicit in brief.

The executing agent (Claude Code) caught the gap autonomously and wrapped the existing wrappers defensively. Without that catch, K1 would have shipped UB landmines в production code.

**Principle: throws и their boundary catches are inseparable.** Adding a throw to a function в native code requires identifying every `extern "C"` function that can call it directly или indirectly, и ensuring all of them have try/catch. The brief MUST state this explicitly — implicit safety expectations fail when briefs are executed by agents following instructions literally.

**Brief authoring requirement** (mandatory checklist item для any brief modifying native code):

- [ ] **Throw inventory**: list every `throw` или exception-throwing operation introduced or modified by this brief
- [ ] **Boundary trace**: для each throw, identify every `extern "C"` function that may propagate it (direct callers и indirect callers через native call chains)
- [ ] **Wrap inventory**: list every existing `extern "C"` function that needs try/catch added or verified
- [ ] **Default catch behavior**: specify what each catch site returns (typically: ABI failure code 0, void no-op, or sentinel value matching existing convention)

**Rationale**: native code review must check exception flow на all boundaries, не just newly-added ones. A throw introduced 100 lines deep в an existing function cascades через every wrapper that ever called that function. Existing wrappers без try/catch were safe before the throw was added; they are unsafe after. Brief must close this gap explicitly.

**Falsifiable claim**: Briefs that follow the throw inventory checklist will encounter zero «agent caught UB risk autonomously» deviations on native-code milestones from K2 onward. Counter-example would force checklist refinement.

### Brief authoring as prerequisite step

When a brief is authored на main (typically as expansion of a skeleton in `tools/briefs/`), the brief file itself becomes an unstaged modification. If the brief subsequently instructs the executor to verify clean working tree (pre-flight HG-1), the check fails on the brief's own presence.

**Failure mode (observed at K1 closure, 2026-05-07).** K1 brief skeleton (~31 lines) was expanded to full executable form (~1223 lines) on main. Pre-flight HG-1 («working tree clean») failed on the brief itself. Executing agent resolved by committing «brief authoring» on main as separate prerequisite step (`8fee2b1`) before creating the K1 feature branch.

This was correct resolution but not specified by the brief — agent improvised. Future briefs should specify this pattern explicitly to avoid relying on agent improvisation.

**Principle: brief authoring is a separate workflow step с its own commit, performed BEFORE any execution work begins.**

**Workflow (mandatory для any brief authored on main)**:

1. Author brief skeleton → full brief expansion on main
2. Commit brief: `git commit -m "docs(briefs): K{N} brief authored — full executable {scope}"`
3. (Push optional — brief commit can ride along with eventual feature branch merge)
4. **Now** execution can begin — pre-flight HG-1 will pass on clean tree
5. Execution creates `feat/k{N}-{scope}` branch off main
6. Brief authoring commit будет ancestor of the feature branch automatically

**Why this matters**: pre-flight HG-1 is a hard gate (workspace corruption signal). It cannot be relaxed для brief-itself exception case без weakening the gate's value. Separating brief authoring into its own commit preserves the gate's invariant («clean tree» means clean tree, no exceptions).

**Brief structure note**: Future briefs SHOULD include explicit «Step 0 — Brief authoring commit» before pre-flight checks, calling out this prerequisite. Without it, executing agents face self-bootstrapping problem and either improvise (best case) or fail HG-1 incorrectly (worst case).

**Falsifiable claim**: Briefs that include explicit «Step 0 — brief authoring commit» step will encounter zero «agent improvised brief-vs-clean-tree resolution» deviations from K2 onward.

### Calibrated time estimates

Time estimates in architectural documents (`KERNEL_ARCHITECTURE.md` Part 2, `VULKAN_SUBSTRATE.md` Part 2, brief skeletons in `tools/briefs/`) are written assuming **hobby solo developer pace — approximately one hour per day of manual typing**. This was the working assumption when those documents were authored.

**Auto-mode execution is fundamentally faster.** When briefs are executed by Claude Code agent в auto mode, three multipliers apply:

1. **No human typing limit** — agent generates code at LLM inference speed
2. **No re-thinking during execution** — architectural decisions and design discussions happen during brief authoring, executor performs implementation only
3. **No waiting between steps** — atomic commits chain immediately, build/test cycles minimize idle time

**Observed multiplier**: 5-10x faster than hobby-pace estimates (measured across K0-K3, 2026-05-07).

**Concrete K0-K3 data**:

| Milestone | Brief estimate (hobby pace) | Actual execution time |
|---|---|---|
| K0 | 1-2 days | ~20 min |
| K1 | 3-5 days | ~1 hour |
| K2 | 2-3 days | ~1.5 hours |
| K3 | 5-7 days | ~3.5 hours |
| **Total K0-K3** | **11-17 days** | **~6 hours** |

This is not exaggeration or one-time speed-up — it is the steady-state pace когда:
- Brief is authored carefully с full design decisions resolved upfront
- Execution is delegated к Claude Code agent в auto mode
- Crystalka acts as architectural reviewer, не keystroke producer

**Brief authoring requirement** (mandatory checklist item для any brief from K4 onward):

- [ ] **Estimate clarity**: brief states explicitly which pace it assumes
- [ ] **Both estimates**: provide hobby-pace estimate (matches roadmap docs) AND auto-mode estimate (calibrated к actual execution)

**Recommended phrasing**:

```
**Estimated time**: X hours auto-mode (Y days at hobby pace, ~1h/day manual typing).
```

Example: «**Estimated time**: 4-6 hours auto-mode (5-7 days at hobby pace).»

**Why this matters**:

- **Calibrated expectations**: when Crystalka asks «how long?», the answer should match reality, не roadmap fiction. False expectations lead к over-pessimistic sequencing decisions.
- **Sequencing decisions affected**: D2 sequencing decision (β5/β6/β3) was framed assuming hobby-pace estimates. Real timeline для β6 kernel-first = 1-2 calendar weeks, не 5-8 weeks. M9.x runtime sprint can start sooner than original plan implied.
- **Brief scope decisions affected**: a milestone estimated 2-3 weeks may seem too large to bundle additional concerns into; same milestone at 4-8 hours is comfortable scope для bundling related fixes.
- **Pipeline economics**: agent execution costs (compute, tokens, time) are proportional к real execution time, не fictional days. Tracking real time enables proper accounting.

**Falsifiable claim**: From K4 onward, brief estimates that follow the «X hours auto-mode (Y days hobby pace)» format will match actual execution time within 2x. Counter-example would force re-calibration of the multiplier.

**Caveat — what auto-mode does NOT speed up**:

- **Brief authoring itself** — design discussions, architectural decisions, throw inventory analysis, careful prose. These remain at human pace + LLM dialogue (~1-3 hours for a 1500-line brief). This is correct — quality of brief determines quality of execution.
- **Architectural design discussions** — Q1-Q4 style decision sessions before brief authoring. Crystalka must engage thoughtfully; cannot be auto-mode'd.
- **Closure review and lessons learned** — analysis of deviations, methodology updates. Requires human judgment.

**The pattern is**: think slowly, decide carefully, execute fast. Auto-mode multiplier applies only к execution phase.

### Pipeline closure lessons (K-series, post-K8.1)

Pipeline behaviour matures across closures. Lessons surfaced from K8.1 onward concern dependency-cycle commit shapes, drift between brief authoring time and execution time, and test isolation in mature multi-mod fixtures. They differ in character from the K0-K3 era foundation lessons recorded above (descriptive pre-flight, ABI boundary catches, brief-as-step, calibrated estimates) and are recorded here as a distinct section so future readers can trace methodology evolution chronologically.

#### Atomic commit as compilable unit

The pipeline's atomic-commit discipline is normally read as "small commits with focused scope." The intuitive proxy for "small" is "few files." This proxy works in the common case; it fails when types in different files form a dependency cycle that compiles only together.

**Failure mode (observed at K8.1 closure, 2026-05-09).** K8.1 brief Phase 5 specified five separate commits, one per managed bridge wrapper file (`InternedString.cs`, `NativeMap.cs`, `NativeComposite.cs`, `NativeSet.cs`, plus the wiring change in `NativeWorld.cs`). At execution time, Cloud Code discovered that `InternedString.Resolve` calls `NativeWorld.ResolveInternedString`, which in turn requires `InternedString` to be already defined. The wrappers and the World wiring formed a small dependency cycle that did not factor into independently-buildable per-file commits — any subset of the five would either fail to compile or require stub methods that would be deleted in the next commit (and stub-and-delete is the textbook structural костыль).

The brief's five-commit shape was abandoned in favour of a single atomic commit bundling all five files. The deviation was recorded in the K8.1 closure report and ratified post-hoc — splitting would have produced either broken-build commits or temporary stubs, both worse than bundling.

**Principle: atomic = minimum unit that compiles and passes tests, not minimum unit by file count.**

The pipeline's atomic-commit discipline is structural, not aesthetic. Each commit should leave the project in a working state — compilable, tests passing — so that any later `git checkout` lands on a coherent codebase. When type definitions cross files in a way that requires them to land together, the file-count proxy lies: "five separate commits" produces five broken intermediate states. The compilable-unit definition is robust against this; the file-count proxy is not.

In practice, most commits remain single-file or two-file because most type definitions don't form cycles. The compilable-unit rule is a reformulation that preserves the common case and handles the cycle case without temporary scaffolding.

**Brief authoring requirement** (mandatory checklist item for any brief introducing new types across multiple files):

- [ ] **Cycle inventory**: identify any cross-file type dependencies (calls, returns, generic parameters) introduced by the brief.
- [ ] **Commit-shape decision**: for cycles, specify the bundled commit explicitly rather than mandating per-file commits the executor must override.
- [ ] **Stub-and-delete prohibition**: never specify a per-file commit shape that requires temporary stub methods later removed; that is a structural костыль regardless of how clean each commit looks in isolation.

**Falsifiable claim**: from K8.2 onward, briefs that include the cycle inventory checklist will encounter zero "executor bundled commits the brief specified to split" deviations on milestones introducing new cross-file types. A counter-example would force the rule to be re-examined for missing cycle classes.

#### Phase 0.4 inventory as hypothesis, not authority

Briefs include Phase 0.4 inventory listing files and directories the brief expects to find on disk. The intuitive reading is "expected layout — verify and STOP if wrong." Closer inspection shows the inventory is brief-authoring-time hypothesis, not execution-time authority: code layout drifts between brief authoring and execution, and the executor sees the truth on disk that the brief author saw second-hand or remembered from earlier sessions.

**Failure mode (observed at K8.1 closure, 2026-05-09).** K8.1 brief Phase 0.4 expected the managed bridge wrappers to live under `src/DualFrontier.Core.Interop/Marshalling/`. At execution time, Cloud Code listed the actual `Marshalling/` directory and found it contained ID/registry helpers (`ComponentTypeRegistry.cs`, `EntityIdPacking.cs`, `NativeComponentType.cs`) — not primary handle wrappers. The actual project convention placed primary handles top-level (`NativeWorld.cs`, `SpanLease.cs`, `WriteBatch.cs`).

A strict reading of Phase 0.4 as authority would have triggered a STOP. Cloud Code instead recognized the inventory as hypothesis, recorded the divergence, and placed the new wrappers (`InternedString.cs`, `NativeMap.cs`, `NativeComposite.cs`, `NativeSet.cs`) top-level matching the convention. The decision was recorded in the K8.1 closure report and ratified.

This pattern recurs across milestones. Briefs author against a model of the codebase that the brief author held in their head at authoring time; the codebase moves between then and execution. The inventory documents what the brief author thought was true, not what is true.

**Principle: Phase 0.4 inventory is a hypothesis under test, not a hard gate.**

Hard gates from Phase 0 (working tree clean, baseline tests passing, prerequisite milestone closed) protect the executor from corrupting workspace state — those remain STOP-eligible. The inventory is a separate category: it documents the brief author's expectation for the audit trail. Mismatches between expectation and reality are recorded as deviations, not stops.

This separation is consistent with the descriptive-pre-flight principle established at K0 closure (see "Pre-flight checks: descriptive over prescriptive" above) and extends it: descriptive principle handles informational *checks*; this lesson extends the same logic to informational *inventories*.

**Brief authoring requirement** (mandatory checklist item for any brief introducing new files):

- [ ] **Inventory marked as hypothesis**: Phase 0.4 wording uses "Expected layout (brief-authoring-time hypothesis; record divergences and proceed)" rather than "Expected layout (STOP if mismatched)."
- [ ] **Convention reference**: where the brief proposes a placement, cite the convention being followed (e.g., "matches NativeWorld.cs / SpanLease.cs top-level convention") rather than asserting placement absolutely.
- [ ] **Halt category clarity**: separate hard gates (workspace state) from informational inventories (file layout) explicitly in the brief's Phase 0 sub-section ordering.

**Falsifiable claim**: from K8.2 onward, briefs that mark Phase 0.4 as hypothesis will encounter fewer false-stop interruptions on layout mismatches than briefs that mark it as authority. The measurement: count execution sessions where the executor halts on Phase 0.4 layout divergence vs. proceeds with recorded deviation. Target: zero false stops on layout-only divergences from K8.2 onward.

#### Mod-scope test isolation

Tests that exercise per-mod resource reclaim semantics (string-pool clear, ALC unload, mod-scoped registry teardown) must isolate every reference to the resource within the scope under test. Any reference taken outside the scope — including reads that look like read-only assertions — anchors the resource to a co-owning scope and prevents the test from observing reclaim.

**Failure mode (observed at K8.1.1 closure, 2026-05-09).** K8.1.1 brief Phase 5 included a test `EqualsByContent_StaleGeneration_ReturnsFalse` whose specified setup interned content under `BeginModScope("ModA") / EndModScope("ModA")`, then re-interned the same content from outside any scope as a "fresh-lookup sanity check," then called `ClearModScope("ModA")` and asserted the original handle resolved to null.

At execution, the test failed: the original handle still resolved successfully after `ClearModScope`. Per K8.1.1 brief Stop condition #3, Cloud Code read `string_pool.cpp::clear_mod_scope` and found that the brief-author-induced re-intern outside the scope had added the id to `ids_by_mod_[""]` (the empty/core scope's ownership list). `clear_mod_scope("ModA")` correctly skipped reclaim because the id was co-owned by core. The K8.1 implementation was correct; the brief's test setup had inadvertently anchored the id to a second scope.

The fix moved the fresh-lookup re-intern **inside** the `BeginModScope / EndModScope` pair, keeping the id uniquely owned by `ModA`. The test then observed the expected reclaim. The deviation was recorded in the K8.1.1 closure report.

This is a Stop-condition #3 success: the methodology caught the test-shape error before it shipped. But the underlying lesson is general — any test that asserts per-mod reclaim must keep all referencing inside the scope under test. The pattern recurs across mod-scoped resources beyond string interning (component-type registries, system-graph entries, ALC-loaded assemblies).

**Principle: tests asserting per-mod resource reclaim must hold all references to that resource inside the scope under test, including read-side references taken for assertion purposes.**

The architectural reason is structural: per-mod cleanup is implemented as "reclaim resources owned only by the scope being torn down." A reference taken from outside that scope creates co-ownership, which the cleanup correctly preserves. The test that intends to observe reclaim must therefore avoid creating that co-ownership. Read-side ergonomics (taking a handle outside a scope to "check" something inside the scope) silently changes ownership and breaks the test's intent.

This generalizes beyond string interning. K8.2 component conversion will produce tests that assert mod-scoped component-type teardown; M7-era ALC tests assert assembly unload; future mod-OS work will assert similar reclaim. All are subject to this rule.

**Brief authoring requirement** (mandatory checklist item for any brief authoring tests on per-mod reclaim):

- [ ] **Reclaim test isolation**: tests that assert reclaim of mod-owned resources must take all references — both for setup and for assertion — inside the `BeginModScope / EndModScope` pair (or equivalent for non-string mod-scoped resources).
- [ ] **Anchor warning**: the brief explicitly notes the anchoring failure mode for the test author, with a reference to this lesson.
- [ ] **Scope-leak proof obligation**: if the test must take a reference outside the scope (rare, but possible for cross-scope semantics), the test asserts that reclaim does not occur, and that intent is the test's documented purpose.

**Falsifiable claim**: from K8.2 onward, tests that follow the reclaim-test-isolation rule will assert reclaim correctly on the first build, without the executor needing to invoke Stop condition #3 to debug per-mod reclaim semantics. Counter-examples — Stop condition #3 invocations on reclaim assertions where the test setup was the cause — would force re-examination of the rule's coverage.

### Phase A' lessons (post-A'.0.5)

#### Milestone consolidation under session-mode pipeline

Milestone boundaries should respect handoff costs between sessions. In v1.x era с model-tier boundaries (N=4, local executor + cloud prompt-generator + cloud architect + human), each milestone carried handoff cost: outputs from one tier became inputs to the next, context loss between tiers required brief authoring as an intermediate artifact. Splitting work into multiple milestones reduced per-milestone scope but compounded handoff cost.

In v1.6 era с session-mode boundary (N=2, human + unified Claude Desktop session switching between deliberation and execution modes), handoff cost between sessions is captured by **brief authoring itself** (architectural-decision brief, fourth brief type per K8.0 / K-L3.1 / A'.0.7 precedent). One brief = one session = one milestone shape. Splitting work into multiple milestones no longer reduces handoff cost — it duplicates brief authoring overhead.

**Failure mode (observed at A'.0.5 authoring 2026-05-10).** A'.0.5 brief originally proposed two-milestone split: A'.0.5 mechanical (file reorganization + cross-ref refresh, Cloud Code session) and A'.0.6 semantic refresh (module-local doc rewrite, Opus deliberation + Cloud Code execution). Rationale was built on stale 4-agent assumption — split would reduce per-session context load for local executor. Crystalka surfaced «зачем сплит там окно контекста в 1 миллион токенов»; under v1.6 session-mode boundary с unified Claude Desktop session capacity, the split rationale collapsed.

The two-milestone proposal collapsed back to single A'.0.5 session. Delivered 19 atomic commits (Phase 0–9), 36 file relocations, ~250 cross-reference updates, 5 README cleanups, 6 module-local refreshes, pipeline terminology scrub, Tier 1 + Tier 2 audit. Test baseline preserved at 631 throughout. Session length: ~2-4 hours wall-clock (commit range `27523ac..4e332bb`). The proposed A'.0.6 milestone collapsed into A'.0.5 Phase 5 (README cleanup) at execution time per Stop #1 protocol; no handoff cost incurred.

**Principle: milestone boundaries should match session capacity boundaries, not legacy milestone-splitting habits.** Under session-mode boundary с large context windows, a single session can host multiple-phase work without handoff cost. Splitting introduces overhead (brief authoring duplication, context re-establishment, inter-milestone state management) without proportional benefit.

This generalizes K-Lessons «atomic commit as compilable unit» (K8.1 closure 2026-05-09) to milestone scope: atomic commit said «commit boundaries respect compilable units within milestone»; this lesson says «milestone boundaries respect handoff costs between sessions». Both share structural pattern: boundaries should match natural seams (compilable units / session capacity), not arbitrary fragmentation.

**Brief authoring requirement** (mandatory checklist item for authoring milestone briefs from A'.0.7 onward):

- [ ] **Session-capacity assessment**: brief author estimates whether proposed milestone scope fits one Claude Desktop session capacity (deliberation or execution mode as appropriate)
- [ ] **Handoff cost check**: if proposing multi-milestone split, brief author justifies handoff cost as bringing proportional benefit (unique к v1.6 session-mode reality — split was default-free in v1.x era)
- [ ] **Stop protocol clarity**: multi-phase single-session milestones have explicit Stop conditions between phases enabling mid-session deliberation (Q&A with direction owner) without milestone fragmentation

**Falsifiable claim**: milestones authored under this lesson will maintain or improve completion-rate vs legacy multi-milestone splits on equivalent scope, measured via «sessions per closure» counter. Counter-examples (milestone split that empirically delivered better than equivalent bundled milestone) would force re-examination of the bundle-default rule.

**Caveat — what session capacity does NOT include**:
- **Sequential dependencies**: if Milestone B requires architectural deliberation against Milestone A's output (e.g., A'.1 amendment requires A'.0.7 methodology rewrite to land first because amendments need to reflect post-A'.0.7 methodology framing), separate sessions are correct — this IS handoff cost, just deliberately deferred. Stop protocol detects this; bundles fail and split is the recovery.
- **Different boundary types within scope**: deliberation work and execution work require different session modes; cannot bundle into one session even с large context. K-L3.1 (deliberation) + amendment brief execution (execution) are correctly separate milestones.

**Compared с v1.x era pattern**: under model-tier boundary, splitting was default safe — each tier had bounded capacity, handoff cost was relatively low (brief-as-artifact was natural between tiers anyway). Under session-mode boundary, bundling is default safe — single tier с large capacity, handoff cost is brief-authoring duplication. The discipline inverted с pipeline restructure.

#### Lesson #7 — A brief that prescribes an API must transcribe the API, not paraphrase it

When a brief tells the executor to call a constructor, a helper, or a file path, the brief author must open the actual source and copy the real signature into the brief at authoring time. «K2-era registry ready» is a note, not a signature. CAPA-2026-05-13's lesson («read entry-point files in full») addressed transitional-state comments; this lesson addresses *API surface*. A brief is a contract for mechanical execution; a contract cannot reference an interface it has not read.

**Origin**: A'.5 K8.3+K8.4 combined milestone v1.0 (2026-05-14) prescribed `new ComponentTypeRegistry()` — no such ctor existed; invented a helper that already existed; gave a wrong sln path; the factory bulk-write shape was incompatible with the real K8.1-primitive structure. Caught at execution time by the executor reading the kernel files; resolved by patch v1 (5 findings). Formalized at A'.5 closure (2026-05-14, brief v2.0 §9.4). CAPA-2026-05-14-K8.34-API-SURFACE-MISS opened by patch v1, closed at v2.0 closure.

**Falsifiable claim**: briefs authored under this lesson will not incur API-surface halts during execution. Counter-examples (brief that paraphrased API and executed cleanly anyway) would force re-examination — but the failure mode is asymmetric (paraphrase that happens to be correct doesn't validate the practice; one paraphrase that's wrong invalidates it).

#### Lesson #8 — A brief that splits a change into N steps must prove each of the N−1 intermediate states is valid

Before a brief prescribes an incremental sequence, the author must walk each intermediate state and confirm it compiles, passes tests, and is architecturally coherent. If an intermediate state cannot be made valid, the change is not incrementally divisible — the atomic unit is larger than one step, and the brief must say so.

**Origin**: A'.5 K8.3+K8.4 combined milestone v1.0 split the storage cutover into 12 commits and never simulated what the data looked like between commit 9 and commit 20 — the answer was «two stores diverging per tick», and the dual-write bridge was the costyl that hid it. The third halt of the milestone (mid-transition drift, 2026-05-14) surfaced the issue before any incremental commit landed on `main`. Brief v2.0 §1.4 + §6.4 re-designed Phase 4+5 as a single atomic cutover; v2.0's Commit 2 (`54c6658`) is one indivisible commit that does not compile until the entire cutover is complete. Storage-backend cutover is the canonical example: it is binary, the atom is the whole thing.

**Corollary** (for METHODOLOGY): a brief cannot promise «zero halts»; it can promise «halts before damage». Three halts on the K8.3+K8.4 milestone, zero harmful commits — that is the system working. A brief's honest guarantee is that bad premises surface at Phase 0 / at deep-read / at the compile gate, before they reach `main`. CAPA-2026-05-14-K8.34-MID-TRANSITION-DRIFT opened by halt 3, closed at v2.0 closure.

**Falsifiable claim**: briefs authored under this lesson will not incur mid-transition-drift halts. Counter-examples (a brief that split an indivisible change and executed cleanly because the executor improvised) would force re-examination — but per the asymmetric failure mode, improvised executions do not validate the brief's split rationale.

#### Lesson #11 — Architectural reduction methodology

When designing mechanism, actively search для reductions where complex setup hides simpler model. Reduction patterns recur consistently в research-framework deliberation work; они shorten architectural surface, dissolve compromises, и preserve invariant integrity. Six strong applications across К10 deliberation arc (2026-05-16 → 2026-05-17) + Q-G-2 composite namespace precedent (2026-05-15) establish empirical foundation.

**Sub-patterns surfaced**:

1. **Reduction by recognizing assumption.** Single-latency display assumption hidden inside К-L16 lock until К-L17 questioned it; multi-layer composition emerged as alternative dissolving Mode C latency compromise.

2. **Reduction by invariant introduction.** К-L18 mod lifecycle quiescent state invariant dissolved coordination machinery — pipeline drain protocol unnecessary because system already quiescent at unload time.

3. **Reduction by ground truth check.** К-L19 hardware tier commitment eliminated defensive abstraction layer — target hardware (AMD RDNA 3, NVIDIA Turing+, Intel Arc) fully supports async compute; designing fallback layer для hypothetical absent capability was over-engineering.

4. **Reduction by recognizing complex setup hides simpler model.** Q-G-2 composite namespace deliberation (2026-05-15) had 6 successive reductions: storage as gameplay metadata, wave shader as gameplay-driven, two-layer model, distribution+navigation unified, CPU/GPU policy decoupled, autonomous=GPU persistent. Final architecture: 3 substrate primitives (V0/V1/V2) instead of original 7 (G0..G6).

5. **Reduction through multi-layer composition.** К-L17 split display tick state by latency tolerance — sim state pipeline-bound, intent overlay sub-tick latency, combat feedback sub-tick. Single-latency assumption dissolved without trading compromise.

6. **Reduction by redistributing complexity к cleaner architectural locus.** К-L15 filter primitive (S2 Q3b hybrid) redistributed cost asymmetrically — cold path preserves К-L7 atomic-from-observer performance, hot path same as naive without abstraction overhead.

**Mental checks к add when designing mechanism**:

- «Is there invariant that makes this mechanism unnecessary?»
- «Is there assumption I'm making that's not necessary?»
- «What's the ground truth I haven't verified?»
- «Can I redistribute complexity к cleaner location?»
- «Does this dissolve compromise, или just choose which compromise к accept?»

**Application discipline**: when deliberation surfaces architectural mechanism, run mental checks before committing к mechanism design. Reduction patterns не replace proper engineering — sometimes proper engineering choice is correct outcome. But absence of reduction search risks accepting compromise prematurely.

**Falsifiable claim**: K10 implementation (А'.7) and subsequent milestones authored under Lesson #11 discipline will surface fewer late-stage architectural revisions vs milestones without. Counter-example would force re-examination — но pattern recognition during К10 deliberation suggests reduction-aware authoring catches issues earlier.

**Origin**: Q-G-2 composite namespace deliberation 2026-05-15 (first strong application); К10 deliberation 2026-05-16 → 2026-05-17 (6 strong applications). Formalized at S6 lock 2026-05-17.

#### Lesson #20 — Tactical heuristics в research framework (category error)

Tactical heuristics never argue against strategic architectural decisions в decade-horizon research framework. Heuristics like «overengineered», «YAGNI», «premature optimization», «simpler architecture» are appropriate only when goal is shipping speed; они category error когда goal is architectural integrity.

**Refined element 1 — Effort/time cost не argues against architectural completeness**: When horizon is long, distributed-perpetual costs (rent) of architectural compromise outweigh front-loaded migration costs. Quote Crystalka 2026-05-17:

> «Не зачем архитектурно упрощать, если это дорого по времени для реализации, но дает большой плюс на дистанции — то оно того стоит.»

**Refined element 2 — «Simpler architecture» reasoning can be tactical heuristic disguised as architectural argument**: Mental check: «does this dissolve compromise, или just choose which compromise к accept?» Если answer is «choose compromise», argument is tactical. Architectural reasoning addresses **what is being architected**, не **how much effort к invest**.

**Refined element 3 — Default-inclusion bias appropriate для architectural items**: К-L14 explicit application. Burden of proof on **exclusion**, не inclusion. Architectural items dismissed only on specific architectural grounds (invariant violation, principle conflict).

**Refined element 4 — Performance reasoning is tactical; architectural integrity is strategic** (from #17 candidate, merged into #20 per scope): On decade horizons, distributed-perpetual costs of architectural compromise outweigh front-loaded migration costs. Migration motivation must engage с architectural integrity, не just performance optimization.

**Mental check before recommending architectural choice**:

«Am I reasoning from cost, или from correctness?»

- Cost reasoning belongs в product context (shipping speed, resource constraints)
- Correctness reasoning belongs в research framework context (architectural integrity, invariant preservation)

When goal is research framework, correctness wins.

**Application track record**:

- К-L14 invariant initial formulation (К10 deliberation session 2026-05-16)
- К10 scope expansion к 25 items (session log §3.10 — Crystalka correction prompted default-inclusion bias adoption)
- S8-Q3 Pattern (b) adaptive depth initial rejection corrected (this session — «performance optimization, not architectural cleanliness» was tactical-style rejection without considering governor framework framing)
- S8-Q3 Pattern (d) initial recommendation corrected (this session — «simpler architecture» was tactical heuristic disguised as architectural argument)
- S8-Q4 Pattern (d) initial recommendation corrected (this session — built complex coordination protocol для problem that doesn't exist under К-L18 invariant)

**Falsifiable claim**: Deliberation sessions conducted under Lesson #20 discipline will surface fewer Lesson #20 violations per session vs sessions without. Counter-example (deliberation following Lesson #20 still surfaces violations as if it weren't formalized) would force re-examination — но pattern recognition в К10 deliberation suggests formalization shifts discourse pattern.

**Origin**: К-L14 invariant formulation 2026-05-16; refined через 3 violations + corrections в К10 S8 deliberation 2026-05-17. Formalized at S6 lock 2026-05-17.

#### Lesson #22 — Read existing code + ask operational context before surfacing architectural concerns

Existing codebase часто имеет answer'ы которые would otherwise re-derive from first principles. Code-first reading reduces surface'ing redundant questions. Operational context (who triggers this, when, under what state) frequently eliminates concerns architectural surface assumes are present. Especially important для mature subsystems (modding, persistence) где multiple migration cycles уже surfaced edge cases.

**Refined element (this session) — extends к «ask about operational context»**: Before designing mechanism для hypothetical concern, verify if operational reality eliminates concern. Operational reality often surfaces invariants that dissolve mechanism need (per Lesson #11 sub-pattern 2 — invariant introduction).

**Mental check before designing mechanism**:

1. «Have I read the existing code в this area?»
2. «What's the operational context (who triggers this, when, under what state)?»
3. «Is there ground truth (hardware, target environment, configuration) I should verify?»

If any answer is «no», pause mechanism design и address gap first.

**Application track record**:

- **S3 Mod ALC lifecycle (S3-Q1)** — initially surfaced atomicity granularity options (atomic/incremental/hybrid) as if discussion новая. Crystalka redirected к reading existing `ModIntegrationPipeline.UnloadMod`. Reading revealed substantial existing 7-step chain с pre-pause discipline, best-effort step wrapping, WeakReference spin loop, JIT stack-frame discipline. Re-framed S3 questions accordingly (L1/L2/L3 layering pattern, not redo atomicity from scratch).
- **К-L18 trigger (S8-Q4)** — built complex (d) two-phase coordination protocol для pipeline drain timing concern. Crystalka surfaced operational reality («mods unload только через settings menu, не во время active simulation»). К-L18 quiescent state invariant emerged; coordination protocol unnecessary.
- **К-L19 trigger (S8-Q5)** — designed defensive abstraction layer (Pattern (d)) для hypothetical hardware concern. Crystalka surfaced actual hardware (own machine ASUS TUF Gaming A16 «Skarlet» с AMD RX 7600S RDNA 3, target hardware tier supports async compute). К-L19 hardware tier commitment emerged; abstraction layer unnecessary.

**Falsifiable claim**: Deliberation following Lesson #22 discipline will surface fewer «mechanism designed for non-existent problem» corrections vs deliberation without. Counter-example (deliberation that read code и checked operational context still designed for hypothetical concerns) would force re-examination.

**Connection к Lesson #11**: Lesson #22 is **input** к Lesson #11 reduction patterns. Reading code surfaces existing assumptions worth questioning (#11 sub-pattern 1); operational context surfaces invariants (#11 sub-pattern 2); ground truth check eliminates defensive architecture (#11 sub-pattern 3). Both lessons describe complementary deliberation discipline.

**Origin**: Surfaced repeatedly в К10 deliberation 2026-05-17. Formalized at S6 lock 2026-05-17.

### Provisional Lessons

Lessons surfaced but не yet formally promoted к Full Lessons. They are captured here для consultation during future deliberation; promotion к Full Lesson status happens когда application evidence accumulates (typically 3+ strong applications). К-closure report (А'.8) reviews provisional pool и promotes mature candidates.

**A'.8 К-series formal closure (2026-05-23) promotion outcomes** per Q-N-8-5 LOCKED Session 2 Day 2:

**12 FORMALIZED at A'.8 closure** (per [K_CLOSURE_REPORT.md §6.2](../architecture/K_CLOSURE_REPORT.md#62--per-lesson-formalize-rationale-12-lessons)):
- Lesson #7 (strengthened) — Marshal.SizeOf<T>() alignment audit + maturity curve [previously Formalized]
- Lesson #8 (strengthened) — Atomic compilable commits + К10.3 v2 multi-document evidence + А'.7.x 13-atomic precedent [previously Formalized]
- Lesson #9 + #9.1 sub — Survey phase before brief authoring + placeholder vs production status [PROMOTED from Provisional at A'.8]
- Lesson #10 — Architecture audit + technical debt inventory in one pass [PROMOTED from Provisional at A'.8]
- **Lesson #14 — Pre-existing drift cleanup as separate cascade [PROMOTED from Provisional at A'.8; second clean application surfaced — А'.7.x bus refactor + Godot removal both separate-branch atomic]**
- Lesson #16 — Brief length scales с deliberation complexity, не execution scope [PROMOTED from Provisional at A'.8]
- Lesson #17 — Tactical vs strategic performance reasoning [PROMOTED from Provisional at A'.8]
- Lesson #21 — Redundancy check complementing К-L14 default-inclusion bias [PROMOTED from Provisional at A'.8]
- Lesson #25 — Implementation depth follows consumer materialization [FORMALIZED at A'.8; A'.7.5 compile-time layer materialization third application]
- Lesson #26 — Cross-substrate scope splitting [FORMALIZED at A'.8]
- **Lesson #27 — Render/compute workload exercises prior substrate primitives [PROMOTED from Provisional at A'.8; third application surfaced — А'.7.x bus stress workload surfaced 5 Bugs in К10.2 primitives]**
- Lesson #N2 — Mid-session brief amendment via halt-before-damage Path 2 [FORMALIZED at A'.8; A'.7.x δ1-δ3 application добавлено]

**1 SUNSET at A'.8 closure**:
- Lesson #15 — Emotional projection avoidance — SUNSET (subsumed by Lesson #20 tactical heuristics correction discipline; preserved for historical record). См. K_CLOSURE_REPORT.md §6.4.

**10 DEFER (Provisional pool carry forward post-A'.8, post-cascade-#2)**:
- Lesson #18 — Boundary crossing batching symmetric pattern (carried)
- Lesson #19 — On-demand activation multi-wake-source (carried; long-term defer; sunset candidate если К-L13 invariant subsumes pattern)
- Lesson #N3 — К-L9 mod-facing boundary Contracts/Application (carried)
- Lesson #N5 (NEW А'.7.x) — Independent investigation branch as К-L14 evidence gathering
- Lesson #N6 (NEW А'.7.x) — Test fixture cleanup discipline as invariant
- Lesson #N7 (NEW А'.7.x) — Gap audit между AUTHORED and Q-N ratification
- Lesson #N8 (NEW А'.7.x) — Pre-flight reproduction disproves diagnostic hypothesis
- Lesson #N9 (NEW А'.7.x) — Closure-protocol gap as soft-halt class (tied к К-L14 falsifiability criterion 6)
- Lesson #N10 (NEW Godot removal) — Brief leaning vs Phase 0 evidence reduction discipline
- **Lesson #N12 (NEW К-extensions cascade #2) — Defensive Reserved Stub Pattern**: When interface либо implementation should exist structurally without real implementation (dormant abstractions, reserved tiers, forward-options), implementations MUST throw descriptive exceptions (NotImplementedException or similar) on invocation, не silently return defaults либо empty values. Empty stub implementations compile, pass type-checker, and pass tests by doing nothing — constituting lying-test surface. Defensive throws prevent this structurally: tests exercise stub method → throw fires → test FAILS loudly. **Paired discipline с Lesson #25 refined**: Lesson #25 says «design abstractions when consumer materializes»; Lesson #N12 says «if you must ship structural surface ahead of implementation, make it loudly defensive». **First application**: К-extensions cascade #2 (2026-05-23) — DualFrontier.Launcher's RenderCommandDispatcher все 6 dispatch handler methods throw NotImplementedException с descriptive message linking к cascade #3 visual implementation + Lesson #N12 reference. **Promotion gate**: second application с reusable pattern (different reserved stub surface — e.g. IDevKitRenderer materialization либо new architectural skeleton in future cascade).

Full per-Lesson promotion rationale + application history в [K_CLOSURE_REPORT.md §6](../architecture/K_CLOSURE_REPORT.md#6--lessons-promotion-individual-decisions).

#### Lesson #9 — Survey phase before brief authoring (provisional)

Architecture recon surveys precede brief authoring когда «fully update LOCKED» arises и scope is undefined. Briefs are for known scope; surveys define scope. Switch authoring → survey mode.

**Application count**: 1 (Architecture Recon 2026-05-15).
**Promotion gate**: 2+ more strong applications.

#### Lesson #10 — Architecture audit + technical debt inventory in one pass (provisional)

Debt signals where compromise hid architectural truth. Combining audit + debt inventory surfaces both в same documentation pass.

**Application count**: 1 (Architecture Recon 2026-05-15).
**Promotion gate**: 2+ more strong applications.

#### Lesson #14 — Pre-existing drift cleanup as separate cascade (refined: deferrals must be respected) (provisional)

Pre-existing drift surfaced during prior cleanup gets deferral notes. Subsequent cleanup must respect deferrals, не override them without re-deliberation. Self-consistent governance behavior across cleanup cycles.

**Application count**: 1 strong (DRIFT-016 SC-4 halt in cleanup cascade 2026-05-16).
**Promotion gate**: 2+ more strong applications.

#### Lesson #15 — Emotional projection avoidance (provisional)

Earlier session lesson. Awaiting more evidence.

#### Lesson #16 — Brief length correlates с deliberation complexity, не execution scope (provisional)

Brief length scales с amount of decision-recording needed, не amount of mechanical work. Comprehensive defensive briefs preferred for context safety across session gaps.

**Application count**: pattern reasonable, applications needed.

#### Lesson #17 — Performance reasoning tactical vs strategic (provisional, may merge с #20)

Performance optimization is tactical; architectural integrity is strategic. Migration motivation must engage с architectural integrity, не just performance.

**Status**: refined elements folded into Lesson #20 «Refined element 4». May merge entirely с #20 at next deliberation.

#### Lesson #18 — Boundary crossing batching pattern (symmetric forward/reverse) (provisional)

When boundary crossing performance is invoked as objection к migration, check if batching pattern from existing layer can extend symmetric direction. Forward batching (managed→native AcquireSpan/BeginBatch precedent) suggests reverse batching (native→managed callback batch) is natural и supported.

**Application count**: 1 strong (.NET 10 + К10 batched callback ABI surface).
**Promotion gate**: 2+ more strong applications.

#### Lesson #19 — On-demand activation pattern (TickScheduler one wake among many) (provisional)

On-demand activation pattern from OS process scheduling applies к ECS system scheduling. Time-driven и event/state-driven systems both legitimate; scheduler unifies via wake-up registry. Architectural reduction: TickScheduler is one wake source among many, не the sole scheduling mechanism.

**Application count**: 1 (К-L13 candidate, К10 deliberation §3.7).
**Promotion gate**: 2+ more strong applications.

#### Lesson #21 — Redundancy check before default-inclusion (К-L14 complement) (provisional)

Default-inclusion bias под К-L14 не применяется когда proposed item дублирует функцию existing item. Mental check: «что **уникально** добавляет item к architectural surface?» Если ответ «ничего» — exclude. Complement к Lesson #20 (don't reject on cost grounds), не contradiction.

**Application count**: 1 strong (К9.5 cancellation 2026-05-17).
**Promotion gate**: 2+ more strong applications.

#### Lesson #23 candidate — Register classification can drift from actual document state

**Origin**: K8.5 deferral cascade (2026-05-18). Lesson #22 reconnaissance surfaced register classification drift — DOC-D-K8_5 entry classified `lifecycle: AUTHORED`, actual content skeleton-grade. Mismatch existed since A'.4.5 enrollment (2026-05-12) and persisted through subsequent governance cycles without detection.

**Pattern**: Register classification applied at enrollment time based on enrollment evidence (file exists, brief authored). Actual content state may differ at enrollment OR drift post-enrollment as project context shifts (e.g., audience deferral, scope reframing, schema extensions making prior classification obsolete).

**Application**: Extend Lesson #22 (read existing code before authoring) к include register entry vs actual content reconciliation. Pre-flight reads for brief authoring against existing registered documents should:
1. Read register entry (lifecycle, version, special_case_rationale)
2. Read document content itself
3. Reconcile entry classification with actual content state
4. If mismatch surfaced, reclassification cascade may be required before main brief work

**Strong applications counted**: 1 (К8.5 deferral cascade 2026-05-18).

**Formalization gate**: 2+ strong applications + formalization review at А'.8 K-closure report per S6 lock 2026-05-17.

**Complementary к**: Lesson #22 (read existing code first); Lesson #11 (architectural reduction redundancy check — applied к exclude К8.5 from skeleton framework based on register state, then refuted by content state — Lesson #11 also benefits from content-based reduction check).

**Potential analyzer encoding**: register classification verification rules (DOC-D entries cross-referenced with actual file content); deferred к А'.9 analyzer rule specification phase.

### Reference: K0 lessons learned

Concrete K0 closure lessons live в `docs/MIGRATION_PROGRESS.md` K0 entry (5 items). The descriptive-pre-flight principle in this section generalizes from those lessons; it is не a complete account of K0 — that lives в the migration tracker.
