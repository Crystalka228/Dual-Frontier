---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-METHODOLOGY
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.6"
next_review_due: 2027-05-10
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-METHODOLOGY
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-METHODOLOGY
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.6"
next_review_due: 2027-05-10
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-METHODOLOGY
---
# Dual Frontier development methodology

*The project's central methodology document. Describes the architect-executor split with contracts as IPC across context boundaries, the verification cycle, economics, threat model, empirical results, and boundaries of applicability.*

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

The principle has been formally invoked at least six times during the M7 batch and the pre-M8 technical-debt closure:

1. **Real pawn data** ([9141bd6](https://github.com/Crystalka228/Dual-Frontier/commit/9141bd6), M7 H3). Hardcoded Warhammer-flavored pawn names, hash-derived role labels, and hash-derived skill bars were removed in favor of `IdentityComponent`, `RandomPawnFactory`, and real `SkillsComponent` data. Eight dead files were deleted alongside the replacement (four stub UI components, one stub node, three undispatched bridge commands).

2. **Needs decay direction** ([ee12108](https://github.com/Crystalka228/Dual-Frontier/commit/ee12108), M7 H4). Decay-toward-zero was a placeholder lie implying automatic recovery while no module actually closed needs. The sign was flipped to deficit accumulation; the XML doc was rewritten to honestly describe ungrounded depletion.

3. **ModMenuPanel position** ([5f0b4f5](https://github.com/Crystalka228/Dual-Frontier/commit/5f0b4f5), M7 H5). The modal misposition surfaced during F5 — the centered modal was visually unreachable. The fix converted Control to CanvasLayer per the Phase 4 UI pattern; this was a structural correction, not a tweak to coordinates.

4. **Asset gitignore companion** ([805b882](https://github.com/Crystalka228/Dual-Frontier/commit/805b882), M7 H5). Extracted Kenney + Cinzel folders are derived state; the source `.zip` files are the in-git source of truth. Tracking both extracted folders and source archives would have been redundancy masquerading as honesty.

5. **Menu pauses simulation** ([9f87536](https://github.com/Crystalka228/Dual-Frontier/commit/9f87536), M7 H6). Two independent pause flags (`_isRunning` for Apply safety, `_paused` for tick advance) where `BeginEditing` only toggled the former. The orchestration layer was made to wire both lockstep; F5 verification confirmed.

6. **Needs semantic flip** ([f4a5839](https://github.com/Crystalka228/Dual-Frontier/commit/f4a5839), TD-3.1). The storage convention "0 = full, 1 = starving" combined with consumer expectations of wellness ("1 = best") forced a translation layer (`1f -` inversions in `MoodSystem` and `PawnStateReporterSystem`) that hid the mismatch. After the flip, the inversion logic disappeared naturally, because storage now matched consumer semantics. The principle did the work it is supposed to do.

The principle constrains all participants in the pipeline (§2.1): the architect cannot author a brief that references data sources absent from disk; the executor cannot synthesize fields whose backing data is absent from the brief; the QA review at phase closure requires that every artifact be auditable against real existence; the direction owner cannot hide behind «we will fill it later» because the next session in the pipeline will refuse to operate against a stub. Each application of the principle is a moment where one role's output would have introduced a placeholder lie if not constrained.

**Falsifiable claim.** The track record across cycles is the falsifiable signal of whether the principle is load-bearing. As of M7 closure: six applications, zero counter-examples — no case where a placeholder was deliberately preserved as a "we will fill it later" stub. Forward target: the count continues to grow without counter-examples through M8–M10. A counter-example — a placeholder that survives a closure review — would falsify the principle's load-bearing role and would force a methodological retraction.

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
- [GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) — **v2.0 LOCKED.** Field-based GPU compute as a foundational architectural capability; K9 field storage + G0–G5 Vulkan compute roadmap. Phase 3 `ProjectileSystem` deferral preserved as Domain B special case.
- [ROADMAP](/docs/ROADMAP.md) — phases, dependency reasoning, the bridge pattern between Phases 5 and 6.

## Native layer methodology adjustments

Kernel и runtime native layers introduce specific methodology adjustments documented в:
- `KERNEL_ARCHITECTURE.md` Part 7 (kernel-specific adjustments)
- `RUNTIME_ARCHITECTURE.md` Part 7 (runtime-specific adjustments)

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

Time estimates in architectural documents (`KERNEL_ARCHITECTURE.md` Part 2, `RUNTIME_ARCHITECTURE.md` Part 2, brief skeletons in `tools/briefs/`) are written assuming **hobby solo developer pace — approximately one hour per day of manual typing**. This was the working assumption when those documents were authored.

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

### Reference: K0 lessons learned

Concrete K0 closure lessons live в `docs/MIGRATION_PROGRESS.md` K0 entry (5 items). The descriptive-pre-flight principle in this section generalizes from those lessons; it is не a complete account of K0 — that lives в the migration tracker.
