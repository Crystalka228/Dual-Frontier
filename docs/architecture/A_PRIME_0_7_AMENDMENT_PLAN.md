---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-A_PRIME_0_7_AMENDMENT_PLAN
category: A
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-A_PRIME_0_7_AMENDMENT_PLAN
---
# A'.0.7 Amendment Plan — old/new text pairs

**Status**: AUTHORED 2026-05-10 (Phase 3 deliverable of A'.0.7 session)
**Authoring**: Crystalka + Claude Desktop session, single deliberation 2026-05-10
**Authority**: A'.0.7 session locks Q-A07-1 through Q-A07-8 + Q1 through Q12 + synthesis form per `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` (committed `55d9e36`)
**Execution target**: separate amendment brief, executed post-A'.0.7 closure (likely folded into A'.1 К-L3.1 amendment per brief §6.2 fold optionality, или standalone follow-up if scope warrants separation)
**Estimated execution**: 30–60 min auto-mode (docs-only; per-document atomic commit shape)
**Test count delta**: zero (no source changes)

---

## §0 Locks summary (recap from session)

### Pre-session locks (Q-A07-1 through Q-A07-5, accepted 2026-05-10 pre-authoring)

| # | Lock | Mechanism |
|---|---|---|
| Q-A07-1 | (α) Pure deliberation session | Brief shape analog K-L3.1; no inline execution; Phase 4 amendment plan = deliverable |
| Q-A07-2 | (β) Mixed disposition per section | Per-section judgment in Q-А07-X / Q-X cascade; some current-pipeline rewrite, some preserved historical, some discarded |
| Q-A07-3 | (β+γ) PIPELINE_METRICS preserve as historical с per-metric reassessment | Implementation: Q10.a-with-standardized-labels |
| Q-A07-4 | (γ) Falsifiable claim generalized к architect/executor abstract framing | Pipeline-agnostic methodology framing across METHODOLOGY corpus |
| Q-A07-5 | (a) A'.0.5 lesson #1 formalized | Implementation: Q8.c-formulation (new «Phase A' lessons (post-A'.0.5)» sub-section) |

### Session locks (Q-A07-6 through Q-A07-8, locked during 2026-05-10 session)

| # | Lock | Mechanism |
|---|---|---|
| Q-A07-6 | Audience contract: agent-as-primary-reader | Methodology corpus authored under agent-as-primary-reader assumption; human reader pathway preserved at README level only; high cross-reference density, FQN-style references, §-level addressability, terse compression are declared design features, not coincidental complexity. Session-level invariant frame for Q2–Q12 |
| Q-A07-7 | (b) Defer K-L3.1-derived methodology lesson к A'.8 K-closure report | K-L3.1 closure clarification observation («closure clarification triggers retroactive principle reformulation») belongs in A'.8 K-series closure narrative, not A'.0.7 pipeline-restructure scope |
| Q-A07-8 | (c) Inline §6 forward measurement plan в PIPELINE_METRICS | PIPELINE_METRICS evolution backlog tracked in same document as data; agent-reader compositional density |

### Question locks (Q1 through Q12)

| # | Lock | Recording target |
|---|---|---|
| Q1 | (b) Abstract primary + footnote | METHODOLOGY §0 Abstract — §1.3 of this plan |
| Q2 | (α) Structural rewrite, abstract role categories + current configuration table | METHODOLOGY §2.1 — §1.5 of this plan |
| Q3 | (c-reformulated) Top-layer principle + three-properties mechanism (falsifiability + self-contained scope + repository as coordination surface) | METHODOLOGY §2.2 — §1.6 of this plan |
| Q4 | (b-reformulated) Economic invariant + current configuration с A'.0.5 empirical anchor | METHODOLOGY §3 — §1.9 of this plan |
| Q5 | (c-decomposed) Per-sub-section disposition с parallel-form case studies (Phase 4 closure v1.x + A'.0.5 closure v1.6) | METHODOLOGY §4 — §1.10 of this plan |
| Q6 | (α-b) §5 substantial rewrite preserving OpenClaw case study + §6 verify clean | METHODOLOGY §5 + §6 — §1.11 + §1.12 of this plan |
| Q7 | (a-table) Per-section judgment с explicit dispositions table | METHODOLOGY §1 + §2.3 + §2.4 + §7 + §9 + §10 + §11 — §1.4 + §1.7 + §1.8 + §1.13 + §1.15 + §1.16 + §1.17 of this plan |
| Q8 | (c-formulation) Phase A' lessons new sub-section с A'.0.5 lesson full formulation | METHODOLOGY «Native layer methodology adjustments» — §1.18 of this plan |
| Q9 | (b) Verify clean Native layer methodology adjustments existing sub-sections | METHODOLOGY «Native layer methodology adjustments» — §1.18 of this plan |
| Q10 | (a-with-standardized-labels) 5 standardized labels + per-metric annotations + v0.1→v0.2 bump | PIPELINE_METRICS — §2 of this plan |
| Q11 | (table) Per-sub-section MAXIMUM_ENGINEERING_REFACTOR dispositions | MAXIMUM_ENGINEERING_REFACTOR — §3 of this plan |
| Q12 | (c-formulation) README hybrid Pipeline section + audience contract declaration | README.md Pipeline section — §4 of this plan |

### Synthesis form

| Document | Form | Rationale |
|---|---|---|
| METHODOLOGY.md | §4.A (abstract primary + concrete examples threaded) | Q-A07-4=γ + Q1.b + Q2.α + Q3.c locked abstract primary framing |
| PIPELINE_METRICS.md | §4.C (versioned-empirical-record) | Q-A07-3=β+γ + Q10.a locks v1.x preservation verbatim + per-metric era annotations |
| MAXIMUM_ENGINEERING_REFACTOR.md | §4.A (abstract primary с surgical updates) | Q11-table; research-tier brief, methodology-character |
| README.md | §4.A (gateway abstract + current + historical) | Q12.c-formulation gateway role |

Two-track principle: methodology corpus (METHODOLOGY + MAXIMUM_ENGINEERING_REFACTOR + README) describes invariants pipeline-agnostically; empirical record (PIPELINE_METRICS) preserves per-era data verbatim. Pipeline-agnostic principles + per-era empirical data are different epistemic categories; different document shapes correct.

---

## §1 METHODOLOGY.md amendments

**Target version**: v1.5 → v1.6 (substantive pipeline-restructure amendment per A'.0.7 session locks)
**Synthesis form**: §4.A — abstract primary + concrete examples threaded

### §1.1 Header version line

**Old text** (current v1.5 header, line 32 area):

```
*Version: 1.5 (2026-05-09). The document describes the methodology in its state after Phase 4 closure, with the M7 operating-principle elevation appended in §7, the post-K0 / post-K1 / post-K3 native-layer adjustments appended to the native-layer adjustments section (descriptive pre-flight checks, ABI boundary exception completeness, brief authoring as prerequisite step, calibrated time estimates), and the post-K8.1 / post-K8.1.1 pipeline closure lessons sub-section (atomic commit as compilable unit, Phase 0.4 inventory as hypothesis, mod-scope test isolation).*
```

**New text**:

```
*Version: 1.6 (2026-05-10). Pipeline restructure rewrite per A'.0.7 deliberation session. §0 Abstract generalized к architect-executor abstract framing; §2.1 role distribution rewritten в abstract role categories с v1.6 current-configuration table; §2.2 contracts as IPC reframed across context boundaries с three-properties mechanism; §3 economics restructured к invariant + current-configuration с A'.0.5 empirical anchor; §4 throughput parallel-form case studies (Phase 4 closure v1.x + A'.0.5 closure v1.6); §5.2/§5.3 threat model restructured для v1.6 session-mode reality; §9 «degradation as codebase grows» reformulated к pipeline-agnostic; methodology corpus declared agent-as-primary-reader per Q-A07-6. K-Lessons sub-section expanded с A'.0.5 lesson «milestone consolidation under session-mode pipeline» per Q-A07-5.*
```

### §1.2 §0 Abstract — Q1.b implementation

**Old text** (current v1.5 §0 Abstract):

```
This document describes a methodology for developing complex software solo through structured work with an LLM. The methodology tests a hypothesis: a human in the role of contract architect plus LLMs in the role of executors inside strict contract boundaries can produce engineering systems of the same complexity class that usually requires teams of several developers over months.

The configuration is four agents with explicitly distributed roles: a local quantized model in the 4–8B parameter class as code executor, a mid-tier cloud model as prompt generator, a top-tier cloud model as architect and QA, the human as direction owner. There is no direct coordination between agents — formal contracts in code and documentation in the repository tie them together, acting as inter-process communication.

The methodology's main falsifiable claim: a working production-quality game, built by one developer through this methodology in 6–12 months, with measured pipeline performance, defect rate, and architectural integrity over the long haul. Empirical measurements of the pipeline operating against this methodology are recorded in [PIPELINE_METRICS](./PIPELINE_METRICS.md) — see particularly [§2 task-level metrics](./PIPELINE_METRICS.md#2-empirical-task-level-metrics) and [§4 sustained throughput](./PIPELINE_METRICS.md#4-sustained-throughput). Additional falsifiable claims appear in §2.2, §3.1, §4.5, and §5.3.

The methodology is not universal. Boundaries of applicability are recorded in §6.
```

**New text**:

```
This document describes a methodology for developing complex software solo through structured work with an LLM. The methodology tests a hypothesis: a human in the role of contract architect plus one or more LLM instances operating as executors inside strict contract boundaries can produce engineering systems of the same complexity class that usually requires teams of several developers over months.

The configuration is **N agents in an architect-executor split with rigid contracts at boundaries**: a human as direction owner; one or more LLM instances operating as architect (deliberation, brief authoring, QA review) and executor (mechanical application against authored briefs). There is no direct coordination between agents — formal contracts in code and documentation in the repository tie them together, acting as inter-process communication. The architect-executor split with contracts at boundaries is **invariant across configurations**; specific N, the boundary type between architect and executor (model-tier boundary, session-mode boundary, or mixed), and tier mix vary by pipeline configuration.

The methodology's main falsifiable claim: a working production-quality game, built by one developer through this methodology in 6–12 months, with measured pipeline performance, defect rate, and architectural integrity over the long haul. Empirical measurements are recorded in [PIPELINE_METRICS](./PIPELINE_METRICS.md) per-era — see particularly [§2 task-level metrics](./PIPELINE_METRICS.md#2-empirical-task-level-metrics) and [§4 sustained throughput](./PIPELINE_METRICS.md#4-sustained-throughput). Additional falsifiable claims appear in §2.2, §3.1, §4.5, and §5.3.

The methodology is not universal. Boundaries of applicability are recorded in §6.

*Footnote (v1.6, 2026-05-10).* At this methodology version, N=2: Crystalka as direction owner plus a unified Claude Desktop session that switches between deliberation mode (chat interface, architectural-decision recording per K8.0 / K-L3.1 / A'.0.7 precedent) and execution mode (Claude Code agent, tool-loop autonomous, per A'.0.5 precedent). The boundary type is **session-mode**, not model-tier. v1.x era (Phase 0–8, ending 2026-05-09) configured N=4 with model-tier boundaries (local Gemma executor + cloud Sonnet prompt-generator + cloud Opus architect + human); empirical record preserved in [PIPELINE_METRICS](./PIPELINE_METRICS.md) §1–§4 with per-metric transferability annotations. This document is authored under **agent-as-primary-reader assumption (Q-A07-6 lock 2026-05-10)** — readers unfamiliar with the project's cross-reference density should consult [README](../../README.md) first and use AI tooling for navigation.
```

### §1.3 §1 Context and problem — Q7.a-table verify clean

**Operation**: read pass для incidental 4-agent terminology. Expected zero hits beyond «third model» framing (pipeline-agnostic).

**Sub-sections to verify**:
- §1 opening paragraphs («Public discourse on LLM-driven development swings between two poles...») — pipeline-agnostic third-model framing; should survive
- «Experimental control» sub-section — author background; pipeline-agnostic; should survive

**Grep targets** (must return zero outside this section or verify-acceptable contexts):
- `grep -n "four agents" docs/methodology/METHODOLOGY.md` — §0 was the only occurrence pre-amendment; post-amendment expect zero in §1
- `grep -n "local quantized" docs/methodology/METHODOLOGY.md` — should be zero in §1 region
- `grep -n "Gemma\|Cline\|LM Studio" docs/methodology/METHODOLOGY.md` — should be zero in §1 region

**Action**: if grep returns hits, surgical scrub at amendment brief execution time.

### §1.4 §2.1 Role distribution — Q2.α implementation

**Old text** (current v1.5 §2.1):

```
### 2.1 Role distribution

The pipeline uses four agents. Each has a narrow specialized role and operates at the complexity level where its capacity is justified — no higher.

**Local executor.** A quantized model in the 4–8B parameter class, run locally via an inference server with an OpenAI-compatible API. In the Dual Frontier reference configuration this is Gemma 4 E4B Q4_K_M (6.33 GB, 131,072-token context window) through LM Studio. The orchestrator is the Cline extension in VS Code, which feeds the model the current project's context and applies the generated code to the filesystem. The executor's role is routine code generation from a self-contained prompt: 1 prompt → 1–2 files. It makes no architectural decisions. It receives a contract and produces code that satisfies the contract.

**Prompt generator.** A mid-tier cloud model. Turns a task plus its contract into a self-contained prompt for the local executor. Knows the architecture as a whole and formulates precise instructions with namespace, using directives, class signatures, field names, types, default values, and commit-message format. Additionally handles mid-complexity tasks directly, without delegating to the local executor.

**Architect and QA.** A top-tier cloud model with a large context window. Used sparingly — on hard architectural tasks (the scheduler, the dependency graph, non-trivial algorithms) and during full reviews at the closure of each development phase. The ability to operate on the corpus as a whole, rather than individual files, structurally distinguishes the architect from the executor.

**Human.** Direction owner. Selects contracts, makes architectural decisions, frames phase goals. Per unit of time, types noticeably fewer keystrokes than a classical developer and makes noticeably more architectural decisions.
```

**New text**:

```
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
```


### §1.5 §2.2 Contracts as IPC — Q3.c-reformulated implementation

**Old text** (current v1.5 §2.2):

```
### 2.2 Contracts as IPC between agents

This is the central methodological device. In traditional development, contracts — interfaces, types, protocols — serve as a communication mechanism between subsystems of the code. In an LLM pipeline they additionally become a communication mechanism between agents.

A rigid contract is the point where interpretations converge. If a contract is formulated strictly enough to exclude ambiguity, any two agents (a local quantized model and a cloud architect, asynchronously, without direct coordination) will produce compatible artifacts. This works because the contract specifies a condition, not a preference — the compiler, tests, or a static analyzer can check satisfaction mechanically.

The same role for contracts operates between agents in one pipeline during phase reviews: the mid-tier agent leaves formalized diagnostics as a numbered list of items with file and line references; the top-tier agent in the next session receives this diagnostic as input and treats it as a contract. Coordination between agents is resolved through the repository and formal documents, not through direct message exchange.

**Falsifiable claim.** If the architecture is fixed as a set of contracts satisfying a "sufficient rigor" criterion, a quantized model in the 4–8B parameter class on average produces code that passes tests on those contracts on the first build. The measurement metric is the fraction of tasks requiring a second generation round after the first build; the target threshold is under 30%. An empirical example of asynchronous development against a formal contract without human participation appears in §4.2.

Contracts in Dual Frontier live in a separate `DualFrontier.Contracts` assembly that contains no implementation (see [CONTRACTS.md](/docs/architecture/CONTRACTS.md)). It acts as the single vocabulary for every layer of the system: the core, game systems, mods, and build tooling.
```

**New text**:

```
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
```

### §1.6 §2.3 Verification cycle — Q7.a-table mechanical scrub

**Old text** (current v1.5 §2.3 base cycle diagram + extended cycle):

```
The base sequence for one task:

```
Human frames the task plus contract
  ↓
Prompt generator turns it into a self-contained prompt for the executor
  ↓
Local executor generates 1–2 files (free, local)
  ↓
Local build verifies syntactic conformance
  ↓
Tests verify behavioral conformance to the contract
  ↓
Atomic commit with a scope prefix
```

Every step has a failure point with explicit diagnostics. The build is either clean or not. A test either passes or not. The contract is either satisfied or not. Verification is structural, not based on trust in the LLM.

After each substantial development phase closes, the **extended phase-review cycle** runs:

```
Prompt generator: diagnose contradictions and debt across the corpus
  ↓
Architect: validate the diagnostic, find endemic patterns,
           formulate architectural decisions with explicitly
           rejected alternatives, implement the decisions in code,
           write tests, debug, update documentation
  ↓
Human: review the result, push the commits
  ↓
Self-teaching ritual (§4.5): systematize understanding
                             of the built system
```

This extended cycle runs less often than the base cycle — once every 1–2 weeks — and spends substantially more tokens, but its role is critical: it closes architectural debt and preserves corpus integrity over the long haul.
```

**New text**:

```
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
```

### §1.7 §2.4 Atomicity of phase review — Q7.a-table untouched, verify

**Operation**: read pass; expected zero hits (pipeline-agnostic methodological principle).

**Grep targets**:
- `grep -n "Sonnet\|Gemma\|four-agent\|Opus" docs/methodology/METHODOLOGY.md` within §2.4 region — expect zero

**Content preservation rationale**: «architectural decision and its implementation during a phase review are not separable» principle survives any boundary type. The reasoning («without implementation step it is unknown whether decision will work in reality») holds whether the architect-mode session and executor-mode session are the same LLM or different ones — atomicity is about session integrity, не tier identity.


### §1.8 §3 Economics — Q4.b-reformulated implementation

**Old text** (current v1.5 §3 entire section — §3.1 Principles + §3.2 Empirical record at Phase 4 closure + §3.3 Comparison with alternative configurations):

[Full v1.5 §3 preserved here in old/new pair as it stands in current document; amendment brief replaces in full.]

**New text**:

```
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
```

**§3.3 Comparison with alternative configurations**: **discarded**. v1.5 §3.3 «direct API calls without a local model runs to tens of cents per task at Opus-tier prices» was v1.x-era specific comparison; under v1.6 unified subscription, агент-tier comparison не applies. Per Q-A07-6 agent-reader frame, agent reading methodology не нуждается в subscription-choice decision support — that's gateway-tier concern (README) or PIPELINE_METRICS §5 reproducibility concern.

### §1.9 §4 Empirical results — Q5.c-decomposed implementation

**Per-sub-section operations table**:

| Sub-section | Operation |
|---|---|
| §4.1 State at publication | **Refresh**: date 2026-04-25 → 2026-05-10; snapshot table update; architectural inventory paragraph rewritten |
| §4.2 Native-core case study | **Mechanical scrub**: «cloud architect» → «architect-mode session»; «between agents» → «between sessions, regardless of boundary type» |
| §4.3 Indie comparison | **Untouched** |
| §4.4 Phase-review wall-clock | **Parallel-form rewrite**: rename к «Session wall-clock performance — case studies»; preserve Case A (Phase 4 closure v1.x) + add Case B (A'.0.5 closure v1.6) |
| §4.5 Self-teaching ritual | **Untouched** |

#### §1.9.1 §4.1 refresh

**Old text** (current v1.5 §4.1):

```
### 4.1 State at publication

Snapshot of the `Crystalka228/Dual-Frontier` repository on 2026-04-25 after Phase 4 closure:

| Parameter | Value |
|---|---|
| Project age | 5 days |
| Commits (across all branches) | 188 |
| Tests | 82/82 passing |
| Production bugs | 0 |
| Architectural assemblies | 9 |
| Architectural documents in `docs/` | 23 |
| Completed phases | 4 (Core ECS, Verification, Pawns, Economy) |
| Current phase | Phase 5 (Combat) |

The architecture includes a custom ECS with `SparseSet` storage and a parallel scheduler, a dependency graph with Kahn topological sort and formal verification of access declarations, an event bus with three delivery modes (synchronous, `[Deferred]`, `[Immediate]`), physical mod isolation via `AssemblyLoadContext`, a persistence layer with RLE and StringPool independent of Godot, and Godot 4 integration through `PresentationBridge` with a "Domain knows nothing about Godot" rule.
```

**New text**:

```
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
```

#### §1.9.2 §4.2 mechanical scrub

**Edits within §4.2** (current v1.5 narrative preserved structurally):

- «the cloud architect from a mobile device» → «the architect-mode session from a mobile device»
- «the agent had built» → preserved (generic «agent» wording survives)
- «`DualFrontier.Contracts` is rigid enough to play the IPC role between agents» → «`DualFrontier.Contracts` is rigid enough to play the IPC role between sessions, regardless of boundary type»
- «The cloud agent wrote code against a formal interface» → «The architect-mode session wrote code against a formal interface»
- Falsifiability anchor (NATIVE_CORE_EXPERIMENT.md cross-ref + criterion reformulation) preserved unchanged

**Rationale**: §4.2 case study (asynchronous architecture-tier work, hours-scale cycle, contract-as-IPC validation) is architecturally meaningful; only specific phrasings need scrubbing for v1.6 framing consistency.

#### §1.9.3 §4.3 untouched

**Operation**: no edits. Indie comparison + 60–100× compression + falsifiable claim about content/balancing scope all pipeline-agnostic.

#### §1.9.4 §4.4 parallel-form rewrite

**Old text** (current v1.5 §4.4):

```
### 4.4 Phase-review wall-clock performance

The Phase 4 closure review by the architect took **approximately 35 wall-clock minutes** from first to last commit (per session log; the session's commits are squashed in the git history, so the timeline cannot be reconstructed directly from timestamps). Within those 35 minutes the architect validated the prompt-generator's diagnostic (10 items), discovered 5 additional issues (including an endemic `NotImplementedException` pattern across 22 systems), formulated 6 architectural decisions with explicitly rejected alternatives, implemented the decisions in code, wrote 17 new tests while debugging two self-introduced failures, and updated five documents. Result: tests 65/65 → 82/82, Phase 4 closed, Phase 5 unblocked.

In typical team development, an equivalent phase session would be 16–24 hours of total work by a team of 2–3 people, or several days on the calendar. Solo without an LLM: at least a week.

Active developer involvement in those 35 minutes amounted to framing the task on input, reading the result, and pushing the commits. The rest of the time the architect worked autonomously, including debugging two self-introduced test failures.

Full session log: [SESSION_PHASE_4_CLOSURE_REVIEW.md](./SESSION_PHASE_4_CLOSURE_REVIEW.md). *(Russian-language audit trail; preserved verbatim per the i18n campaign rules.)*
```

**New text**:

```
### 4.4 Session wall-clock performance — case studies

Per Q-A07-3=β+γ (PIPELINE_METRICS preservation as historical), per-era throughput measurements live primarily in [PIPELINE_METRICS](./PIPELINE_METRICS.md). Two methodology-level case studies preserved here as falsifiability anchors:

**Case A: Phase 4 closure review (v1.x era, 2026-04-25).** Architect session ~35 wall-clock minutes from first to last commit (per session log; the session's commits are squashed in the git history, so the timeline cannot be reconstructed directly from timestamps). Within those 35 minutes the architect validated the prompt-generator's diagnostic (10 items), discovered 5 additional issues (including an endemic `NotImplementedException` pattern across 22 systems), formulated 6 architectural decisions with explicitly rejected alternatives, implemented the decisions in code, wrote 17 new tests while debugging two self-introduced failures, and updated five documents. Result: tests 65/65 → 82/82, Phase 4 closed, Phase 5 unblocked. Boundary type: model-tier (architect = cloud Opus session, executor = local Gemma). Full session log: [SESSION_PHASE_4_CLOSURE_REVIEW](./SESSION_PHASE_4_CLOSURE_REVIEW.md). *(Russian-language audit trail; preserved verbatim per the i18n campaign rules.)*

**Case B: A'.0.5 closure session (v1.6 era, 2026-05-10).** Execution session — Claude Code agent against authored brief. Delivered ~25 atomic commits over commit range `27523ac..4e332bb`: 36 file relocations (Phase 3), ~250 cross-reference updates (Phase 4), 5 component README cleanups (Phase 5), 6+ module-local doc refreshes (Phase 6), 7 pipeline-terminology scrubs (Phase 7), 1 Tier 1 typo fix + Tier 2 flag artifact (Phase 8), 3 closure commits (Phase 9). Total diff: +4354/-653 LOC. Test baseline preserved at 631 throughout. Session window: 2–4 hours wall-clock. Boundary type: session-mode (architect-mode session authored brief upstream; A'.0.5 invocation = execution-mode session). Full closure entry: [MIGRATION_PROGRESS §A'.0.5](/docs/MIGRATION_PROGRESS.md).

Per-session wall-clock varies with session scope (architectural deliberation vs mechanical execution, codebase size at session time, brief depth). The methodology-level point is **case studies as falsifiability anchors**: each closure session is named, dated, boundary-type-tagged, and the per-session metrics live in [PIPELINE_METRICS](./PIPELINE_METRICS.md) per-era data sections.

In typical team development, an equivalent phase session would be 16–24 hours of total work by a team of 2–3 people, or several days on the calendar. Solo without an LLM: at least a week.
```

#### §1.9.5 §4.5 untouched

**Operation**: no edits. Self-teaching ritual + learning artifact + falsifiable claim about defect-rate-if-skipped + PHASE_1.md cross-ref all pipeline-agnostic.

### §1.10 §5 Threat model — Q6.α implementation

**Per-sub-section operations**:

| Sub-section | Operation |
|---|---|
| §5.1 Risk class of broad-permissions agents | **Untouched** — OpenClaw case study pipeline-agnostic; external citations preserved verbatim |
| §5.2 Pipeline structural defense | **Substantial rewrite** — replace v1.5 4-agent enumeration with v1.6 session-mode enumeration |
| §5.3 Falsifiable claims | **Reformulated** — 4 attack classes restated для v1.6 session-mode mechanism; +1 new claim («architect-executor crosstalk impossible») |

#### §1.10.1 §5.1 untouched

**Operation**: no edits. OpenClaw / Clawdbot / Moltbot case study with named incidents (Cisco AI Defense ClawHub analysis, Shadow maintainer warning, MoltMatch dating profile incident, PRC restrictions March 2026) survives v1.5 verbatim. Pipeline-agnostic warning structure; external citations (Wikipedia, Cisco blog, Bloomberg, Taipei Times, AFP) preserved.

**Note on copyright compliance**: existing v1.5 §5.1 contains a 24-word Shadow maintainer quote that exceeds current 15-word quote ceiling. Per A'.0.7 scope (pipeline-restructure rewrite), §5.1 disposition is «untouched» — legacy content predates A'.0.7 amendment scope; compliance audit is separate concern. If surfaced as cleanup item, separate milestone outside A'.0.7.

#### §1.10.2 §5.2 substantial rewrite

**Old text** (current v1.5 §5.2):

```
### 5.2 Pipeline structural defense

The four-agent architecture structurally avoids the main risk class through minimal permissions for each agent's role, not through "we will be careful."

**Local executor.** No network access (LM Studio runs on localhost). No shell. The filesystem is limited to what the orchestrator (the editor extension) exposes. Prompt injection through the project's file contents is theoretically possible but does not lead to data exfiltration, because there is no outbound channel.

**Prompt generator via desktop app.** Every action is initiated by the user through the chat interface. No heartbeat mode. No autonomous scheduled work. MCP tools are connected explicitly and visible to the user at every moment. Filesystem access through MCP is limited to directories explicitly permitted in the settings.

**Architect via desktop app.** Same as above, plus used sparingly (once every 1–2 weeks for a phase review), which reduces the attack surface across time.

**No direct channel between agents.** Coordination happens through files in the repository and through the human. This means compromise of one agent does not propagate to the others automatically: the attacker must compromise each agent separately, through different channels.
```

**New text**:

```
### 5.2 Pipeline structural defense

The architect-executor split (§2.1) structurally avoids the broad-permissions agent risk class through minimal permissions per session mode, not through «we will be careful». The defense is invariant across boundary types; specific mechanisms vary by boundary type.

**Deliberation sessions** (architect mode). Chat interface, no autonomous tool loop. Every tool invocation is initiated by a human turn. MCP connectors are connected explicitly per session and visible at every moment in the session UI. Read-only access is default; write actions surface explicit confirmations. No heartbeat mode, no scheduled work, no background actions.

**Execution sessions** (executor mode). Claude Code agent, autonomous tool loop within session scope. Tool access bounded by authored brief (Phase 0 reads + Phase N execution plan + closure criteria). Session ends when brief closes; agent does not persist across sessions. Filesystem access is scoped to the repository working directory; network actions are gated by brief specification. MCP tools are enabled per session, not globally.

**No persistent agent state between sessions.** Each session starts with clean context. Cross-session coordination happens through the repository (LOCKED docs, briefs, amendment plans, commit history) — not through agent memory or background state.

**Session-mode discipline as structural barrier.** Deliberation mode does not apply changes; if architectural deliberation requires mechanical changes to verify a hypothesis, that is a separate execution session against an authored brief. Execution mode does not make architectural decisions; the agent escalates on ambiguity per §3 stop/escalate/lock. The split is procedural — but the **briefs themselves** are the structural mechanism: the agent cannot do work outside brief scope without escalation.

v1.x era equivalent defense (model-tier boundary, N=4) operated under the same invariant principle through different mechanisms: local executor (Gemma) had no network access; prompt generator and architect (cloud sessions) had no heartbeat mode; no direct channel between the four agents. Empirical defense properties preserved across boundary type — the **principle** of minimal-permissions-per-role-via-structural-mechanism is invariant.
```

#### §1.10.3 §5.3 reformulated

**Old text** (current v1.5 §5.3):

```
### 5.3 Falsifiable claims

Specific attack classes that are impossible in the pipeline for structural reasons, not "carefulness":

1. **Data exfiltration through the local executor is impossible** — it has no network access.
2. **Scheduled autonomous actions are impossible** — no agent has a heartbeat mode.
3. **Compromise propagation between agents through a direct channel is impossible** — no direct channel exists, only the repository and the human.
4. **Installation of a malicious skill is impossible** — the pipeline has no community-extensions repository with automatic installation.

These claims are verified by inspecting the pipeline architecture and do not depend on the behavior of individual models.
```

**New text**:

```
### 5.3 Falsifiable claims

Specific attack classes that are impossible in the v1.6 pipeline for structural reasons, not «carefulness»:

1. **Out-of-session autonomous actions are impossible** — no session mode includes scheduled work, heartbeat, or persistent background agent. All sessions are human-initiated with explicit scope.

2. **Cross-session agent compromise propagation is impossible** — no persistent agent state between sessions. Compromise of one session does not infect the next; only the repository carries forward, and the repository is human-reviewable.

3. **Out-of-brief execution is impossible** — execution agent operates against authored brief. Brief scope is explicit (Phase 0 reads, Phase N execution plan, closure criteria); the agent escalates on ambiguity rather than improvise.

4. **Installation of malicious extensions is impossible** — pipeline has no community-extensions repository with automatic installation. MCP connectors are connected explicitly per user action, visible per session.

5. **Architect-executor compromise crosstalk is impossible** — boundary type is session-mode; one session's compromise does not propagate to the other through a direct channel. Only the repository is shared, and repository state is human-auditable through git history.

These claims are verified by inspecting the pipeline architecture and do not depend on the behavior of individual models. Per Q-A07-4=γ, they survive any boundary-type change: if the pipeline restructures to a mixed boundary type or multi-model architecture, the structural defense reformulates per the new boundary type; the **principle** of minimal-permissions-per-role-via-structural-mechanism is invariant.

v1.x era equivalent claims (model-tier boundary, 4-agent) — see [PIPELINE_METRICS §1](./PIPELINE_METRICS.md#1-pipeline-configuration) historical record.
```


### §1.11 §6 Boundaries of applicability — Q6.b verify clean

**Operation**: read pass per Q6.2.b lock; expected zero hits (pipeline-agnostic applicability scope content); surgical scrub only if found.

**Grep targets** (within §6 region):
- `grep -n "Sonnet\|Gemma\|Cline\|LM Studio\|four-agent\|local quantized" docs/methodology/METHODOLOGY.md` — expect zero hits in §6

**Content preservation rationale**: §6.1 «Where methodology works» (explicit architecture / testable correctness / long horizon — concrete system classes), §6.2 «Where methodology does not work» (exploratory research / creative work / narrow domain / critical cadence), §6.3 «Typical objections and their falsifiable form» — all pipeline-agnostic applicability framing. Survives untouched.

### §1.12 §7 Operating principles — Q7.a-table untouched, verify

**Operation**: read pass; expected zero hits (§7.1 «Data exists or it doesn't» pipeline-agnostic operating principle с 6 empirical applications c commit references).

**Content preservation rationale**: §7.1 enumerates 6 applications across M7 batch (9141bd6, ee12108, 5f0b4f5, 805b882, 9f87536, f4a5839 TD-3.1) all of which are pre-A'.0.7 — unchanged by pipeline restructure. Falsifiable claim about track record across cycles invariant.

### §1.13 §8 Reproducibility — Q7.a-table verify clean

**Per-sub-section operations**:

| Sub-section | Operation |
|---|---|
| §8.1 Minimum configuration | **Verify clean** — pipeline-tier-specific tooling (VS Code+Cline, LM Studio+Gemma, Anthropic Max) listed; v1.x-era specific items survive in this section because §8.1 IS the reproducibility-tier specification |
| §8.2 Confirmed configuration | **Untouched** — ASUS TUF A16 hardware fact pipeline-agnostic |
| §8.3 Process discipline | **Untouched** — contracts/tests/atomic commits/TreatWarningsAsErrors/parallel documentation/atomic phase reviews all pipeline-agnostic |

**Note on §8.1**: this sub-section enumerates v1.x-era reproducibility requirements. Under Q-A07-3=β+γ (PIPELINE_METRICS as historical record), v1.x reproducibility surface is **historical specification**. Q10 PIPELINE_METRICS §5.2 Software dependencies + §5.3 Subscription tier carry `[transfers с reframing]` annotations; methodology §8.1 currently mirrors that data. **Decision**: leave §8.1 verbatim для now; A'.0.7 amendment plan touches methodology methodology rewrite, not reproducibility-tier reformulation. PIPELINE_METRICS §6 forward measurement plan tracks the reformulation work; when v1.6 reproducibility data collected (post-A'.0.7), §8.1 receives surgical update through subsequent methodology amendment.

Alternative consideration: add v1.6 sub-section §8.1.1 «Current configuration reproducibility» с Claude Desktop + Claude Code references — но this would expand A'.0.7 scope beyond locked Q-A07-X surface. Defer.

### §1.14 §9 Open questions — Q7.a-table per-question judgment

**Per-question operations**:

| Question | Operation |
|---|---|
| Long-term dynamics | **Untouched** — pipeline-agnostic |
| Drift of architectural understanding | **Untouched** — pipeline-agnostic |
| Applicability to team development | **Untouched** — pipeline-agnostic |
| **Degradation as codebase grows** | **Reformulate** (key change) — see §1.14.1 below |
| Behavior on series of negative results | **Untouched** — pipeline-agnostic |
| Reproducibility by other developers | **Untouched** — pipeline-agnostic open question |

#### §1.14.1 «Degradation as codebase grows» reformulation

**Old text** (current v1.5):

```
**Degradation as the codebase grows.** The local model's 131k-token context is already close to the limit for large tasks (132.9k for the `Implement ItemAddedEvent` task in Phase 4). With further project growth, the pipeline may need restructuring: splitting the corpus into modules with independent context, switching to models with larger context windows, or using retrieval instead of a fully loaded context.
```

**New text**:

```
**Degradation as the codebase grows.** Session context windows place an upper bound on architectural-deliberation surface and on executor brief scope. With further project growth, the methodology may need restructuring: splitting the corpus into modules with independent context, switching to larger context windows, using retrieval instead of fully loaded context, or moving к multi-session deliberation/execution flows where one session can no longer hold the relevant surface. v1.x era hit this constraint first at 131k tokens (local Gemma, `Implement ItemAddedEvent` task в Phase 4 reached 132.9k); v1.6 era operates под higher ceiling (Claude Desktop Max 5× subscription session context) but the same fundamental constraint applies — context window is finite, codebase scope grows.
```

**Rationale**: v1.5 phrasing tied degradation hypothesis к specific local model token count. Under Q-A07-4=γ generalization, reformulation makes constraint pipeline-agnostic (session context window finiteness) while preserving v1.x specific data point as illustrative historical reference. Per Q-A07-6 agent-reader frame, density preserved through dense parenthetical historical reference.

### §1.15 §10 Change history — Q7.a-table append v1.6 row

**Operation**: append new row к existing change history table.

**New row text**:

```
| 1.6 | 2026-05-10 | Pipeline restructure rewrite per A'.0.7 — §0 Abstract generalized к architect-executor abstract framing; §2.1 role distribution rewritten в abstract role categories с v1.6 current-configuration table; §2.2 contracts as IPC reframed across context boundaries с three-properties mechanism; §3 economics restructured к invariant + current-configuration с A'.0.5 empirical anchor; §4 throughput parallel-form case studies (Phase 4 closure v1.x + A'.0.5 closure v1.6); §5.2/§5.3 threat model restructured для v1.6 session-mode reality; §9 «degradation as codebase grows» reformulated к pipeline-agnostic; methodology corpus declared agent-as-primary-reader per Q-A07-6. K-Lessons sub-section expanded с A'.0.5 lesson «milestone consolidation under session-mode pipeline» per Q-A07-5. |
```

**Insertion location**: after v1.5 row, before «The document is updated after each substantial phase closes...» footer text.

### §1.16 §11 See also — Q7.a-table verify paths

**Operation**: verify cross-ref paths post-A'.0.5 reorg; expected most paths already correct (A'.0.5 Phase 4 cross-ref refresh updated ~250 references repo-wide).

**Cross-refs к check** (from v1.5 §11 list):
- `[README.md](../README.md)` — post-A'.0.5 location of methodology: `docs/methodology/METHODOLOGY.md`; relative path к root README should be `../../README.md`
- `[PIPELINE_METRICS.md](./PIPELINE_METRICS.md)` — sibling, OK
- `[learning/PHASE_1.md](./learning/PHASE_1.md)` — verify learning directory location post-A'.0.5 reorg
- `[SESSION_PHASE_4_CLOSURE_REVIEW.md](./SESSION_PHASE_4_CLOSURE_REVIEW.md)` — verify
- `[ARCHITECTURE.md](/docs/architecture/ARCHITECTURE.md)` — repo-rooted absolute, OK
- `[CONTRACTS.md](/docs/architecture/CONTRACTS.md)` — repo-rooted absolute, OK
- `[DEVELOPMENT_HYGIENE.md](./DEVELOPMENT_HYGIENE.md)` — sibling or moved? verify
- `[ISOLATION.md](/docs/architecture/ISOLATION.md)` — repo-rooted absolute, OK
- `[NATIVE_CORE_EXPERIMENT.md](/docs/reports/NATIVE_CORE_EXPERIMENT.md)` — repo-rooted absolute, post-reorg path
- `[GPU_COMPUTE.md](/docs/architecture/GPU_COMPUTE.md)` — repo-rooted absolute, OK
- `[ROADMAP.md](./ROADMAP.md)` — verify (ROADMAP location post-A'.0.5)

**Grep targets**: cross-document drift audit per §6 of this plan covers final verification.

### §1.17 Native layer methodology adjustments — Q9.b verify clean + Q8.c new sub-section

**Operation**: split into two parts per locks.

#### §1.17.1 Existing sub-sections — Q9.b verify clean

**Sub-sections to verify clean**:

| Sub-section | Operation |
|---|---|
| Opening paragraph | Verify clean |
| Pre-flight checks: descriptive over prescriptive | Verify clean (K0 closure 2026-05-07; pipeline-agnostic) |
| Brief authoring checklist (post-K0 update) | Verify clean |
| ABI boundary exception completeness | Verify clean (K1 closure 2026-05-07; native-layer-specific not pipeline-tier-specific) |
| Brief authoring as prerequisite step | Verify clean |
| Calibrated time estimates | Verify clean — «Claude Code agent в auto mode» phrasing preserved; K0-K3 measured data preserved |
| Pipeline closure lessons (K-series, post-K8.1) opener | Verify clean |
| Atomic commit as compilable unit | Verify clean (K8.1 closure principle) |
| Phase 0.4 inventory as hypothesis, not authority | Verify clean (K8.1 closure principle) |
| Mod-scope test isolation | Verify clean (K8.1.1 closure principle) |
| Reference: K0 lessons learned | Verify path (post-A'.0.5 reorg) |

**Expected outcome**: zero scrub hits. If any found, surgical fix at amendment brief execution.

#### §1.17.2 New «Phase A' lessons (post-A'.0.5)» sub-section — Q8.c-formulation

**Insertion location**: after «Pipeline closure lessons (K-series, post-K8.1)» sub-section (containing the three K8.1 lessons), as parallel era sub-section.

**New text** (full):

```
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
```

### §1.18 A'.0.7 deferral marker removal

**Old text** (top of METHODOLOGY.md, lines 1–26):

```
<!--
A'.0.7 DEFERRAL MARKER (added 2026-05-10 in A'.0.5 Phase 7).

This document describes the methodology in its 4-agent pipeline state
(Crystalka + local quantized executor + cloud prompt-generator + cloud
architect). Crystalka direction 2026-05-10 restructured the pipeline
to 2-agent (Crystalka + unified Claude Desktop session). Substantive
sections of this document — §0 Abstract, §2.1 Role distribution, §2.2
Contracts as IPC, §3 Pipeline economics, §4 Empirical results, §5
Threat model, §8 Reproducibility — describe the prior 4-agent shape
and require A'.0.7 architectural-deliberation rewrite (analog to
K-L3.1) before they reflect current pipeline reality.

A'.0.7 milestone: documentation methodology rewrite. Reads METHODOLOGY
+ PIPELINE_METRICS + MAXIMUM_ENGINEERING_REFACTOR; deliberates each
substantive section against new 2-agent reality; produces revised
documents with falsifiable claims re-grounded in current configuration.

A'.0.5 (this milestone) only relocated the file to docs/methodology/
and applied mechanical pipeline-terminology scrubs in non-substantive
contexts. The 4-agent prose below is intentionally preserved as the
state to be rewritten in A'.0.7.

See /docs/architecture/PHASE_A_PRIME_SEQUENCING.md for the Phase A'
sequence locating A'.0.7 between A'.0.5 and A'.1.
-->
```

**New text**: REMOVED (deferral resolved by amendment brief execution).


---

## §2 PIPELINE_METRICS.md amendments

**Target version**: v0.1 → v0.2 (per-metric annotations added per A'.0.7 Q10.a-with-standardized-labels)
**Synthesis form**: §4.C — versioned-empirical-record

### §2.1 Top-of-document frame note

**Insertion location**: after A'.0.7 deferral marker removal (§2.6), before existing «# Pipeline metrics — Dual Frontier» H1 heading.

**New text**:

```
## Document era annotation (added 2026-05-10 per A'.0.7 amendment)

This document was authored at v0.1 (2026-04-28) under the v1.x era pipeline configuration (4-agent model-tier boundary: human + local quantized Gemma executor + cloud Sonnet prompt-generator + cloud Opus architect). Empirical data in §2–§4 are v1.x era measurements.

A'.0.7 amendment (2026-05-10) restructured the pipeline к v1.6 era (2-agent session-mode boundary: Crystalka + unified Claude Desktop session switching between deliberation and execution modes). v1.x era measurements are preserved verbatim as historical record; each section/sub-section receives a per-metric annotation declaring transferability к v1.6 reality.

**Annotation labels** (used throughout §1–§5 below):

| Label | Meaning |
|---|---|
| `[v1.x era specific]` | Metric tied к 4-agent boundary type; doesn't transfer к v1.6 reality |
| `[transfers с reframing]` | Metric structurally meaningful under v1.6; needs reformulation language |
| `[transfers as-is]` | Metric measures invariant property; survives boundary-type change |
| `[uncertain — needs v1.6 measurement]` | Metric category exists в v1.6 reality but no measurement yet collected |
| `[v1.x historical record]` | Pure archaeological data; transferability not applicable |

v1.6 era data collection is forward-looking; first v1.6 data points emerge from A'.0.5 closure (`27523ac..4e332bb`, commit `4e332bb` on main 2026-05-10) and onward Phase A' milestones. See [METHODOLOGY §3.2 Current configuration economics](./METHODOLOGY.md#32-current-configuration-economics) + [METHODOLOGY §4.4 Session wall-clock performance — case studies](./METHODOLOGY.md#44-session-wall-clock-performance--case-studies) for v1.6-era references located in the methodology corpus. See [§6 Forward measurement plan](#6-forward-measurement-plan-v16-era-data-collection) below for v1.6 era data collection backlog tracked в this document.

Cross-reference: this document accompanies [METHODOLOGY.md](./METHODOLOGY.md) (v1.6 era, post-A'.0.7). METHODOLOGY documents the methodology в pipeline-agnostic form; PIPELINE_METRICS preserves per-era empirical record.
```

### §2.2 Per-metric annotations

**Operation**: insert annotation block after each section/sub-section per Q10 standardized labels.

**Annotation block format** (consistent across all 17 insertions):

```
**Annotation** (A'.0.7, 2026-05-10): `[LABEL]`
[1-3 sentence rationale]
[Optional: pointer к v1.6 equivalent metric or forward-looking measurement plan]
```

**Per-sub-section annotation specifications**:

#### §2.2.1 §1.1 Four-agent role distribution table

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

The 4-agent role distribution table describes the model-tier boundary configuration with N=4 agents (Gemma 4 E4B Q4_K_M executor, Sonnet 4.6 prompt generator, Opus 4.7 architect, human direction owner). Under v1.6 session-mode boundary (N=2: Crystalka + unified Claude Desktop session), this table doesn't transfer. Replaced functionally by [METHODOLOGY §2.1.1 Current configuration table](./METHODOLOGY.md#211-current-configuration-v16-2026-05-10).

#### §2.2.2 §1.2 Workflow per task

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

The workflow diagram («Sonnet turns it into a self-contained prompt for Gemma...») describes v1.x 4-tier base cycle. Under v1.6, base cycle is reframed in [METHODOLOGY §2.3 Verification cycle](./METHODOLOGY.md#23-verification-cycle) с architect-mode + executor-mode sessions instead of tier-specific agents.

#### §2.2.3 §1.3 Hardware reference

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

ASUS TUF A16 hardware unchanged across era boundary; the «local model host» framing is v1.x-specific (RX 7600S 8 GB VRAM hosted Gemma 4 E4B Q4_K_M locally). Under v1.6, the same hardware runs the Claude Desktop client but doesn't host local inference; the hardware capability survives, but the v1.x-specific GPU-VRAM-for-quantized-inference requirement does not. See §6 Forward measurement plan for v1.6 reformulation task.

#### §2.2.4 §1.4 Software stack

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

VS Code+Cline orchestration, LM Studio+Gemma local backend are v1.x-tier-specific tooling. Under v1.6, software stack collapses к Claude Desktop client + Godot 4.3+ + .NET 8.0 (the latter two survive). See §6 Forward measurement plan for v1.6 reformulation task.

#### §2.2.5 §2.1 Phase 4 baseline 5-task table

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

5-task Gemma context/output sizes (InventorySystem 13.9k/1.6k, power grid events 99.9k/2.8k, ItemReservedEvent 79.3k/2.1k, ItemAddedEvent 132.9k/2.8k, StorageComponent 98.2k/3.8k) are 2026-04 Phase 4 archaeological data. Pure historical record; transferability к v1.6 measurement classes not applicable (v1.6 doesn't measure local executor token I/O per task; v1.6 metric class would be «execution session token cost per brief scope»).

#### §2.2.6 §2.2 M0-M3 evening session timeline

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

M-series migration timing 2026-04-27 under 4-agent pipeline; 13-commit timeline (a97dcbf..2e5216b, 19:47–22:51 UTC-4) with M1 manifest schema + M2 IModApi v2 + M3.1 capability registry + M3.3 capability validation closures. Archaeological data; v1.6 equivalent would be Phase A' execution session timing (A'.0.5 closure as first v1.6 era data point, recorded в METHODOLOGY §4.4 Case B).

#### §2.2.7 §2.3 Cost-per-commit at executor tier

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

«All Gemma execution at the syntax-tier runs locally; cost-per-commit at the executor tier is $0.00» describes v1.x free-local + paid-cloud divide. Under v1.6 single-tier Claude Desktop subscription, executor tier doesn't have separate cost profile from architect tier. Replaced by [METHODOLOGY §3.1 Economic invariant](./METHODOLOGY.md#31-economic-invariant) (cost asymmetry between deliberation-context-intensive and execution-scope-bounded work).

#### §2.2.8 §3.1 Stress-test methodology

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

Stress-test procedure (week ending 2026-04-26 deliberate Opus-heavy stress) is v1.x-specific methodology for measuring subscription headroom. Under v1.6 unified Claude Desktop subscription, equivalent stress-test would measure session-volume-vs-rate-limit; documented procedure preserved as archaeological reference.

#### §2.2.9 §3.2 Measurements (two weekly windows)

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

Subscription headroom IS still measurable under v1.6 — the **measurement category** transfers; the specific numbers (73% unused at reset, 25% remaining at 10.5h before reset) are v1.x-era data points. v1.6 measurement collection forward-looking; first v1.6 data points emerge from A'.0.5 closure + A'.0.7 deliberation session + subsequent Phase A' milestones. See §6 Forward measurement plan.

#### §2.2.10 §3.3 Architectural dividend hypothesis

**Annotation** (A'.0.7, 2026-05-10): `[transfers as-is]`

Hypothesis is invariant: «when architecture is fixed as LOCKED specifications с §-level addressability, the architecture-tier agent operates predominantly in implementor mode rather than discovery mode, which avoids reasoning-from-ambiguity overhead». Survives boundary-type change. Supports [METHODOLOGY §3.1 Economic invariant](./METHODOLOGY.md#31-economic-invariant) (context-intensive deliberation work vs scope-bounded execution work).

#### §2.2.11 §4.1 8-day commit history aggregate

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

254 commits over 8 calendar days (2026-04-20..2026-04-27) — Phase 1–4 ramp + i18n campaign + M0-M3 closure. Archaeological data; codebase end-of-window snapshot (17,462 lines C# / 8,277 lines docs / 2:1 ratio) preserved as v1.x reference. v1.6 equivalent would be ongoing daily-commit-rate measurement under Phase A' execution; not collected in this document version.

#### §2.2.12 §4.2 M0-M3 minute-by-minute

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

Per-commit timing of the M-phase evening session 2026-04-27 (3h 4min wall-clock for four migration phases + audit + spec sync). Same era as §2.2. Wall-clock ratio claim («~300× compared к conventional industry baseline of 6–9 weeks senior engineer team setting») is v1.x-era observation; v1.6 equivalent ratios will emerge from Phase A' / B execution measurements.

#### §2.2.13 §4.3 Concurrent workflow observation

**Annotation** (A'.0.7, 2026-05-10): `[transfers as-is]`

Two-workflow concurrency claim is invariant: «pipeline sustained two independent workflows — architectural design (M-phases) and large-scale refactoring (i18n) — within the same window without cross-contamination». Mechanism (clean phase separation + independent contract surfaces) survives boundary-type change. v1.6 equivalent: deliberation session and execution session can run on independent surfaces в parallel (e.g., K-L3.1 deliberation + A'.0.5 execution on adjacent days, both clean).

#### §2.2.14 §5.1 Minimum hardware

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

«Machine with discrete GPU and 8 GB VRAM, or Apple Silicon с 16 GB unified memory» requirement is tied к local Gemma quantization (Q4_K_M Gemma 4 E4B needs 6.33 GB VRAM minimum). Under v1.6, local inference is not part of the pipeline — minimum hardware reduces к whatever the Claude Desktop client requires (modest by comparison). See §6 Forward measurement plan for v1.6 reformulation.

#### §2.2.15 §5.2 Software dependencies

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

.NET 8.0 SDK + Godot 4.3+ survive across era boundary (project tech stack). VS Code+Cline + LM Studio+Gemma are v1.x-specific tooling that doesn't transfer. Anthropic Claude desktop app survives across boundary; subscription-tier guidance same (Max 5×). See §6 Forward measurement plan for v1.6 reformulation.

#### §2.2.16 §5.3 Subscription tier

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

Claude Max 5× ($100/month) subscription requirement persists across era boundary. «Lower tiers (Pro, Max 1×) may be sufficient for lighter operational modes» observation retains validity. «Pipeline does not require API access» — true under v1.6 (Claude Desktop client uses subscription, not API key); verify for v1.6 era through Phase A' / B execution. See §6 Forward measurement plan.

#### §2.2.17 §5.4 What these measurements tell you / what they don't

**Annotation** (A'.0.7, 2026-05-10): `[transfers as-is]`

Methodological framing about measurement validity («figures show pipeline behavior in architecturally-mature mode... not behavior in early-stage open exploration») is invariant. The distinction между Phase 0 lock work and implementor work persists across boundary types. The «reproduction attempt without a Phase 0 lock step will not see comparable ratios» caveat applies in any era.

### §2.3 New §6 Forward measurement plan

**Insertion location**: after §5.4 (before existing «**See also:**» footer line).

**New text**:

```
## §6 Forward measurement plan (v1.6 era data collection)

Per Q10.a annotations (A'.0.7 amendment, 2026-05-10), the following sections require v1.6 era data collection or reformulation. Tasks are not A'.0.7 amendment scope; they form backlog для post-A'.0.7 methodology corpus evolution.

| Section | Task | Trigger |
|---|---|---|
| §1.3 Hardware reference | Reframe «local model host» к v1.6 reality (cloud-tier model via Claude Desktop client; no local inference; minimum hardware reduces к Claude Desktop runtime + project IDE) | Post-A'.0.7 amendment landing |
| §3.2 Subscription headroom | Collect v1.6 era subscription utilization data — first data points: A'.0.5 closure (2026-05-10, single execution session), A'.0.7 deliberation session (2026-05-10, single deliberation session), subsequent Phase A' milestones | Phase A' execution cadence |
| §5.1 Minimum hardware | Reframe — Claude Desktop client minimum hardware (modest), no GPU/VRAM-for-local-inference requirement | Post-A'.0.7 amendment landing |
| §5.2 Software dependencies | Reframe tool stack — Claude Desktop client primary; VS Code+Cline + LM Studio v1.x-specific (removed for v1.6); Godot 4.3+ + .NET 8.0 survive | Post-A'.0.7 amendment landing |
| §5.3 Subscription tier | Verify «API access not required» under v1.6 Claude Desktop subscription; update subscription tier guidance with v1.6-era practical experience | Post-A'.0.7 amendment landing |

Task completion lifts content from this backlog к main document body; this section itself stays present с completed rows marked or condensed format as historical record of A'.0.7-era forward-measurement plan.
```

### §2.4 Version history sub-section

**Insertion location**: end of document (before existing «See also» footer).

**New text**:

```
## Version history

| Version | Date | Change |
|---|---|---|
| 0.1 | 2026-04-28 | Initial collection covering Phases 0–4 baseline plus M0–M3 Mod-OS migration; gathered under v1.x era pipeline (4-agent model-tier boundary: human + Gemma local executor + Sonnet prompt generator + Opus architect). |
| 0.2 | 2026-05-10 | Per-metric annotations added per A'.0.7 amendment (Q10.a-with-standardized-labels): 5 standardized transferability labels applied к each section; top-of-document era frame note; new §6 Forward measurement plan tracks v1.6 era data collection backlog. v1.x era measurements preserved verbatim. |
```

### §2.5 Version line bump

**Old text** (current document status line, around line 8):

```
**Status:** v0.1 (2026-04-28). Initial collection covering Phases 0–4 baseline
plus M0–M3 Mod-OS migration. Updated as further phases close.
```

**New text**:

```
**Status:** v0.2 (2026-05-10). Per-metric annotations added per A'.0.7 amendment; v1.x era measurements preserved verbatim. Updated as further phases close. See [§«Version history»](#version-history) for change log; [§6 Forward measurement plan](#6-forward-measurement-plan-v16-era-data-collection) for v1.6 era data collection backlog.
```

### §2.6 A'.0.7 deferral marker removal

**Old text** (top of PIPELINE_METRICS.md, lines 6–26):

```
<!--
A'.0.7 DEFERRAL MARKER (added 2026-05-10 in A'.0.5 Phase 7).

Empirical metrics in this document were gathered with the 4-agent
pipeline (Crystalka + local quantized executor + cloud prompt-generator
+ cloud architect). Crystalka direction 2026-05-10 restructured the
pipeline to 2-agent (Crystalka + unified Claude Desktop session).
Continued accuracy of these metrics under the new pipeline is not
established. A'.0.7 milestone deliberates whether to:
- discard prior metrics (no longer representative)
- reframe as historical record at a labeled time-of-measurement
- recollect under the new pipeline configuration before publication

A'.0.5 (this milestone) only relocated the file to docs/methodology/
and applied mechanical pipeline-terminology scrubs in non-substantive
contexts. Substantive empirical claims are preserved verbatim as the
state to be re-evaluated in A'.0.7.

See /docs/architecture/PHASE_A_PRIME_SEQUENCING.md for the Phase A'
sequence locating A'.0.7 between A'.0.5 and A'.1.
-->
```

**New text**: REMOVED (deferral resolved by amendment brief execution per Q-A07-3=β+γ; Q10.a-with-standardized-labels implementation captures the «reframe as historical record at labeled time-of-measurement» option chosen).


---

## §3 MAXIMUM_ENGINEERING_REFACTOR.md amendments

**Target version**: v1.0 → v1.1 (A'.0.7 pipeline-mapping update per Q11-table)
**Synthesis form**: §4.A — abstract primary с surgical updates

### §3.1 Per-sub-section disposition table

**Operation**: amendment brief executes per Q11-table dispositions per A'.0.7 session lock.

**Bulk of document untouched** — Tracks A/B/C architectural content survives verbatim:
- §0 Preamble (untouched)
- §1.1, §1.2, §1.3 (verify clean / untouched)
- §2 Track A all sub-sections §2.1–§2.6 (untouched)
- §3 Track B all sub-sections §3.1–§3.6 (untouched)
- §4.1, §4.2, §4.4, §4.5, §4.6, §4.7, §4.8 (untouched / verify clean)
- §5.1, §5.3 (untouched / verify clean)
- §6, §7, §8, §9 (untouched)
- §10 partial untouched (add v1.1 row)

**Substantial rewrites isolated к three sub-sections**:
- §4.3 prompts/ sub-tree (replication kit prompts directory)
- §5.2 parallel-track discipline mapping
- §10 Ratification (add v1.1 row)

### §3.2 §4.3 Replication kit contents (prompts/ sub-tree rewrite)

**Old text** (current v1.0 §4.3 prompts/ block):

```
  ├── prompts/
  │   ├── opus-architect.md            (Opus role prompt template)
  │   ├── sonnet-decomposer.md         (Sonnet role prompt template)
  │   ├── gemma-executor.md            (Gemma role prompt template)
  │   └── escalation-criteria.md       (when to invoke which agent)
```

**New text**:

```
  ├── prompts/
  │   ├── architect-mode.md            (deliberation-mode session prompt template
  │   │                                  — architectural decision recording brief
  │   │                                  shape per K8.0 / K-L3.1 / A'.0.7 precedent;
  │   │                                  per-milestone briefs per K9 / K8.3 / K8.4
  │   │                                  precedent)
  │   ├── executor-mode.md             (execution-mode session prompt template
  │   │                                  — autonomous tool-loop against authored
  │   │                                  brief; analog A'.0.5 execution precedent)
  │   ├── boundary-type-config.md      (deliberation/execution mode-switching
  │   │                                  protocol; agent role specification
  │   │                                  per METHODOLOGY §2.1 role categories)
  │   └── escalation-criteria.md       (when to invoke deliberation mode vs
  │                                     execution mode per METHODOLOGY §3
  │                                     stop/escalate/lock)
```

**Revise surrounding §4.3 text where needed** to reference v1.6 architect-mode/executor-mode sessions instead of v1.x per-tier prompts. Specific phrasing updates left к amendment brief execution time (verify-while-editing pattern).

### §3.3 §5.2 Parallel-track discipline mapping rewrite

**Old text** (current v1.0 §5.2):

```
### 5.2 Parallel-track discipline

Each track adopts the same M-phase discipline as existing migration work:

- Brief authoring → Sonnet
- Implementation → Gemma
- Audit → Opus
- Closure review with byte-identical diff verification on the new LOCKED spec

This brief itself is the equivalent of an "M-phase prompt" but spanning multiple phases. Per-track briefs are written when the track activates.
```

**New text**:

```
### 5.2 Parallel-track discipline

Each track adopts the same milestone discipline as existing migration work:

- Brief authoring → architect-mode session (analog K8.0 / K-L3.1 / A'.0.7 precedent для architectural decision recording brief; analog K9 / K8.3 / K8.4 / K8.5 per-milestone briefs for execution scope specification)
- Implementation → executor-mode session (autonomous tool-loop against authored brief; analog A'.0.5 execution precedent)
- Audit → architect-mode session at phase closure (analog Phase 4 closure review precedent; analog K-L3.1 closure deliberation precedent)
- Closure review с byte-identical diff verification on the new LOCKED spec

This brief itself is the equivalent of an «M-phase prompt» but spanning multiple phases. Per-track briefs are written when the track activates.
```

### §3.4 §10 Ratification — append v1.1 row

**Operation**: append к existing version history (in §10 Ratification's «### Version history» sub-list).

**New entry text**:

```
- **v1.1** (2026-05-10) — A'.0.7 amendment: §4.3 replication kit prompt directory rewritten к v1.6 reality (architect-mode / executor-mode prompts replace per-agent prompts; boundary-type-config.md added); §5.2 parallel-track discipline mapping rewritten к v1.6 reality с K8.0 / K-L3.1 / A'.0.7 / A'.0.5 / K9 / K8.3 / K8.4 / K8.5 / Phase 4 closure precedent cross-refs; document body otherwise unchanged. Tracks A / B / C architectural content untouched.
```

**Insertion location**: after «v1.0 (2026-05-06)» entry, in same bullet list.

### §3.5 Verify-clean grep targets

**Targets** (each must return zero hits outside acceptable contexts):

- `grep -n "Sonnet\|Gemma\|Opus" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md`
  - Acceptable contexts: §1.2 motivations historical note («pipeline-tier-specific»), §10 Ratification v1.0 / v1.1 history entries, §4.3 replication kit description if referring к prompt template selection
  - Expected hits: only in v1.0 history entry, и possibly §1.2 if framing requires
- `grep -n "four-agent\|four agents" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md`
  - Expected hits: zero outside §10 history entries
- `grep -n "local quantized\|Cline\|LM Studio" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md`
  - Expected hits: zero outside §10 history entries

**Action on hits**: amendment brief execution applies surgical scrub.

### §3.6 A'.0.7 deferral marker removal

**Old text** (top of MAXIMUM_ENGINEERING_REFACTOR.md, lines 5–22):

```
<!--
A'.0.7 DEFERRAL MARKER (added 2026-05-10 in A'.0.5 Phase 7).

Sections of this document referencing the 4-agent pipeline (especially
the directory-tree layout sub-sections naming gemma-executor.md role
templates, hardware-variance discussions of local Gemma performance,
and pipeline-mapping tables of Implementation→Gemma) describe the
prior 4-agent shape. Crystalka direction 2026-05-10 restructured the
pipeline to 2-agent (Crystalka + unified Claude Desktop session).
Substantive sections require A'.0.7 architectural-deliberation rewrite.

A'.0.5 (this milestone) only relocated the file to docs/methodology/
and applied mechanical pipeline-terminology scrubs in non-substantive
contexts. Substantive 4-agent prose is preserved verbatim as the
state to be rewritten in A'.0.7.

See /docs/architecture/PHASE_A_PRIME_SEQUENCING.md for the Phase A'
sequence locating A'.0.7 between A'.0.5 and A'.1.
-->
```

**New text**: REMOVED.

---

## §4 README.md amendments

**Synthesis form**: §4.A gateway abstract + current + historical

### §4.1 Pipeline section old/new replacement (Q12.c-formulation)

**Old text** (current README.md Pipeline section, around lines 49–81):

```
## Pipeline

<!-- TODO: A'.0.7 — pipeline restructure к 2-agent (Crystalka + unified Claude Desktop session); current section describes pre-restructure 4-agent shape and is preserved verbatim as historical state until A'.0.7 substantive rewrite lands -->

The pipeline uses four agents separated by level of abstraction, not by
development phase. The agents do not communicate directly; coordination
happens through LOCKED documents in the repository and through the human as
session router.

- **Syntax-tier executor.** A local quantized model in the 4–8B parameter
  class, hosted through a local OpenAI-compatible backend and orchestrated by
  an editor extension. Receives a self-contained prompt and produces 1–2
  files of code against a contract. Makes no architectural decisions.
- **Prompt-generation tier.** A mid-tier hosted model. Turns a task plus its
  contract into a self-contained prompt for the syntax-tier executor; handles
  routine tasks directly.
- **Architect-tier executor.** A top-tier hosted model with a large context
  window. Used sparingly, on hard architectural tasks and on full reviews at
  phase closure.
- **Direction owner.** The human. Selects contracts, makes architectural
  decisions, frames phase goals, routes between sessions.

Full configuration, empirical task metrics, subscription headroom data, and
reproducibility requirements are documented in
[docs/methodology/PIPELINE_METRICS.md](/docs/methodology/PIPELINE_METRICS.md):

- [§1 Pipeline configuration](/docs/methodology/PIPELINE_METRICS.md#1-pipeline-configuration) —
  agent assignments, workflow, hardware, and software stack.
- [§3 Subscription headroom](/docs/methodology/PIPELINE_METRICS.md#3-subscription-headroom) —
  empirical measurements across two consecutive weekly windows.
- [§5 Reproducibility requirements](/docs/methodology/PIPELINE_METRICS.md#5-reproducibility-requirements) —
  what is required to reproduce these measurements and what these
  measurements do not show.

If a contract is rigid enough that a 4-bit quantized local model produces
correct code under it, the contract will hold under any stronger executor.
Isolation from hallucinations is a structural property of the contract, not
of the executor's capacity.
```

**New text**:

```
## Pipeline

The pipeline configures N agents in an architect-executor split with rigid contracts at boundaries: a human as direction owner; one or more LLM instances operating as architect (deliberation, brief authoring, QA review) and executor (mechanical application against authored briefs). The architect-executor split with contracts at boundaries is invariant across configurations; specific N, the boundary type between architect and executor (model-tier boundary, session-mode boundary, or mixed), and tier mix vary by pipeline configuration.

The agents do not communicate directly; coordination happens through LOCKED documents in the repository and through the human as session router.

**Current configuration (v1.6, 2026-05-10).** N=2: Crystalka (direction owner) plus a unified Claude Desktop session that switches between deliberation mode (chat interface, architectural decision recording per K8.0 / K-L3.1 / A'.0.7 precedent) and execution mode (Claude Code agent, autonomous tool-loop per A'.0.5 precedent). Boundary type: session-mode.

v1.x era (Phase 0–8, ending 2026-05-09) used model-tier boundary с N=4 (local quantized Gemma executor + cloud Sonnet prompt-generator + cloud Opus architect + human direction owner). Empirical record preserved in [docs/methodology/PIPELINE_METRICS.md](/docs/methodology/PIPELINE_METRICS.md) с per-metric transferability annotations.

Full pipeline configuration, empirical task metrics, subscription headroom data, and reproducibility requirements documented in [docs/methodology/PIPELINE_METRICS.md](/docs/methodology/PIPELINE_METRICS.md). Full methodology documented in [docs/methodology/METHODOLOGY.md](/docs/methodology/METHODOLOGY.md). The methodology and deeper documents are authored under **agent-as-primary-reader assumption** — readers unfamiliar с the project's cross-reference density should use AI tooling for navigation through the documentation corpus.

If a contract is rigid enough that an executor produces correct code under it on the first build at a measurable rate (target <30% requiring second execution), the contract will hold under any stronger executor or any restructured boundary type. Isolation from executor errors is a structural property of the contract, not of the executor's specific capacity.
```

### §4.2 Cross-ref path verification

**Operation**: verify all cross-refs in README post-A'.0.5 reorg.

**Targets**:
- `[docs/ROADMAP.md](./docs/ROADMAP.md)` — verify ROADMAP location post-A'.0.5 (likely moved или unchanged)
- `[docs/architecture/ARCHITECTURE.md](/docs/architecture/ARCHITECTURE.md)` — OK (post-A'.0.5 path)
- `[docs/architecture/MOD_OS_ARCHITECTURE.md](/docs/architecture/MOD_OS_ARCHITECTURE.md)` — OK
- `[docs/reports/NORMALIZATION_REPORT.md](/docs/reports/NORMALIZATION_REPORT.md)` — verify (post-A'.0.5)
- `[docs/methodology/PIPELINE_METRICS.md §3](/docs/methodology/PIPELINE_METRICS.md#3-subscription-headroom)` — OK
- `[docs/methodology/METHODOLOGY.md §6](/docs/methodology/METHODOLOGY.md)` — verify anchor (§6 boundaries section, post-A'.0.7 unchanged structurally per Q7.a-table)
- `[docs/reports/NATIVE_CORE_EXPERIMENT.md](/docs/reports/NATIVE_CORE_EXPERIMENT.md)` — OK (post-A'.0.5 path)
- `[docs/README.md](./docs/README.md)` — verify (post-A'.0.5 reorg may have changed docs/ README pattern)

**Action**: amendment brief execution time, run grep + manually verify each link resolves. Surgical fix any broken paths.

### §4.3 Audience contract declaration verbiage

**Operation**: ensure audience contract declaration appears в Pipeline section per Q12.c-formulation lock.

**Concrete phrasing locked** (from §4.1 new text):

> «The methodology and deeper documents are authored under **agent-as-primary-reader assumption** — readers unfamiliar с the project's cross-reference density should use AI tooling for navigation through the documentation corpus.»

**Cross-document consistency check** (per §6 cross-document drift audit below): same audience contract declaration appears in:
- README Pipeline section (this §4.1) — primary, gateway role
- METHODOLOGY §0 Abstract footnote (per Q1.b §1.2) — secondary, in methodology corpus
- PIPELINE_METRICS top-of-document frame note (per Q10.a §2.1) — tertiary, in metrics document

All three references say «agent-as-primary-reader assumption» and reference Q-A07-6 lock (где applicable for tracing).

### §4.4 A'.0.7 deferral marker removal

**Old text** (HTML comment около README line 50):

```
<!-- TODO: A'.0.7 — pipeline restructure к 2-agent (Crystalka + unified Claude Desktop session); current section describes pre-restructure 4-agent shape and is preserved verbatim as historical state until A'.0.7 substantive rewrite lands -->
```

**New text**: REMOVED (deferral resolved by Pipeline section rewrite per §4.1).

---

## §5 K-L3.1 conflict check (per A'.0.7 brief §5.6)

### §5.1 A'.0.7 amendment scope: methodology corpus only

**Documents touched by A'.0.7 amendment plan**:
- `docs/methodology/METHODOLOGY.md` (per §1 of this plan)
- `docs/methodology/PIPELINE_METRICS.md` (per §2)
- `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` (per §3)
- `README.md` (per §4)

**Scope characterization**: methodology corpus (process-describing documents) — how the pipeline works, what the methodology claims, how to reproduce.

### §5.2 K-L3.1 amendment scope: architecture corpus only

**Documents touched by К-L3.1 amendment plan** (per `docs/architecture/K_L3_1_AMENDMENT_PLAN.md`):
- `docs/architecture/KERNEL_ARCHITECTURE.md` (v1.3 → v1.5)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` (v1.6 → v1.7)
- `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (v1.0 → v1.1)
- `docs/MIGRATION_PROGRESS.md` (content sync)
- `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` (Disposition B surgical)
- `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` (Disposition B-to-C surgical)
- `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` (Disposition C.1 surgical)
- `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` (Disposition B surgical)

**Scope characterization**: architecture corpus (product-describing documents) — kernel architecture, mod isolation model, migration plan structure, skeleton briefs for downstream К-series execution.

### §5.3 No overlap confirmation

**Verification**:
- A'.0.7 touches `docs/methodology/*` + `README.md` only.
- К-L3.1 touches `docs/architecture/*` + `docs/MIGRATION_PROGRESS.md` + `tools/briefs/*` only.
- Zero file overlap. Zero section overlap. Amendments executable independently in either order.

**Default order** (per Phase A' sequencing): A'.0.7 amendment → A'.1 (К-L3.1 amendment). Reasoning: A'.0.7 lands methodology framing (architect-mode / executor-mode terminology, two-track principle, etc.); A'.1 К-L3.1 amendment commits can reference architectural decision precedents (K-L3.1) and methodology framing (architect/executor sessions) in their commit messages with consistent terminology.

**Reverse order acceptable**: if A'.1 amendment executes first, K-L3.1 amendments use К-L3.1-era terminology в commit messages; A'.0.7 amendment lands afterwards с methodology terminology updates that don't conflict with already-landed К-L3.1 amendments (К-L3.1 amends architectural product docs; A'.0.7 amends methodology process docs).


---

## §6 Cross-document drift audit (Phase 5 of amendment brief)

After commits 1–N land per atomic commit shape (§7 below), amendment brief executes a cross-document grep verification.

### §6.1 Role label consistency check

**Canonical short-form role labels** (locked in §0 synthesis form):

- «Direction owner» (human)
- «Architect» (LLM in deliberation mode) или «architect-mode session»
- «Executor» (LLM in execution mode) или «executor-mode session»
- Boundary type: «session-mode» (v1.6) / «model-tier» (v1.x)

**Occurrence inventory** (8 locations across 4 docs):

| Document | Section | Role label usage |
|---|---|---|
| METHODOLOGY | §0 Abstract footnote | «N=2: Crystalka as direction owner plus a unified Claude Desktop session that switches between deliberation mode... and execution mode...» |
| METHODOLOGY | §2.1.1 Current configuration table | Direction owner / Architect / Executor / Boundary type rows |
| METHODOLOGY | §2.3 Verification cycle diagram | «Architect-mode session... Executor-mode session...» |
| METHODOLOGY | §3.2 Current configuration economics | «Deliberation sessions / Execution sessions...» |
| METHODOLOGY | §4.4 Case B A'.0.5 closure | «Boundary type: session-mode» |
| METHODOLOGY | §5.2 Pipeline structural defense | «Deliberation sessions (architect mode) / Execution sessions (executor mode)» |
| MAXIMUM_ENGINEERING_REFACTOR | §5.2 Parallel-track discipline mapping | «architect-mode session... executor-mode session...» |
| README | Pipeline section current configuration | «Crystalka (direction owner) plus a unified Claude Desktop session... deliberation mode... execution mode...» |

**Drift detection grep**:
```
grep -rn "Sonnet\|Gemma" docs/methodology/ docs/architecture/ README.md
grep -rn "four agents\|four-agent" docs/methodology/ docs/architecture/ README.md
grep -rn "syntax-tier\|architect-tier\|prompt-generation tier" docs/methodology/ docs/architecture/ README.md
```

**Acceptable hits**: only in v1.x era historical references (METHODOLOGY §0 footnote v1.x era paragraph, §2.1 v1.x mention, §3.2 v1.x era economics paragraph, §4.4 Case A description, §5.2 v1.x equivalent defense paragraph, §5.3 v1.x equivalent claims line; MAXIMUM_ENGINEERING_REFACTOR §10 v1.0 history entry; README Pipeline v1.x era paragraph; PIPELINE_METRICS frame note + per-metric annotations + §6 forward measurement plan). All hits in active-spec wording requiring scrub.

### §6.2 Era classification label consistency

**Canonical era labels**:
- «v1.x era» (4-agent, model-tier boundary, Phase 0–8, ending 2026-05-09)
- «v1.6 era» (2-agent, session-mode boundary, Phase A' onward, starting 2026-05-10)

**Drift detection**:
```
grep -rn "v1.0 era\|v1.1 era\|v1.2 era\|v1.3 era\|v1.4 era\|v1.5 era" docs/methodology/ docs/architecture/ README.md
```

Expected: zero hits. Canonical labels are «v1.x» (covering all v1.0–v1.5) и «v1.6». No individual sub-version era labels used.

### §6.3 Cross-ref path verification post-A'.0.5 reorg

**Operation**: spot-check key cross-refs across amended documents.

**Per-doc cross-ref targets**:

| Source doc | Cross-ref | Expected post-A'.0.7 |
|---|---|---|
| METHODOLOGY §0 footnote | `[PIPELINE_METRICS](./PIPELINE_METRICS.md)` | sibling, OK |
| METHODOLOGY §0 footnote | `[README](../../README.md)` | up 2 levels from docs/methodology/ |
| METHODOLOGY §2.2 | `[CONTRACTS](/docs/architecture/CONTRACTS.md)` | repo-rooted absolute, OK |
| METHODOLOGY §3.2 | `[PIPELINE_METRICS §3](./PIPELINE_METRICS.md#3-subscription-headroom)` | sibling с anchor, verify anchor |
| METHODOLOGY §4.1 | `[ARCHITECTURE](/docs/architecture/ARCHITECTURE.md)` | repo-rooted, OK |
| METHODOLOGY §4.1 | `[KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md)` | repo-rooted, OK |
| METHODOLOGY §4.1 | `[MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md)` | repo-rooted, OK |
| METHODOLOGY §4.4 Case A | `[SESSION_PHASE_4_CLOSURE_REVIEW](./SESSION_PHASE_4_CLOSURE_REVIEW.md)` | sibling, verify still in methodology/ |
| METHODOLOGY §4.4 Case B | `[MIGRATION_PROGRESS §A'.0.5](/docs/MIGRATION_PROGRESS.md)` | repo-rooted, OK (no anchor; section heading may not have stable HTML anchor) |
| PIPELINE_METRICS frame note | `[METHODOLOGY §3.2 Current configuration economics](./METHODOLOGY.md#32-current-configuration-economics)` | sibling с anchor |
| PIPELINE_METRICS frame note | `[METHODOLOGY §4.4 Session wall-clock performance...](./METHODOLOGY.md#44-session-wall-clock-performance--case-studies)` | sibling с anchor, verify anchor format |
| MAXIMUM_ENGINEERING_REFACTOR §5.2 | references К8.0 / К-L3.1 / A'.0.7 / A'.0.5 / Phase 4 — verify if hard cross-refs added | precedents named, not necessarily hard-linked |
| README Pipeline | `[docs/methodology/PIPELINE_METRICS.md](/docs/methodology/PIPELINE_METRICS.md)` | repo-rooted, OK |
| README Pipeline | `[docs/methodology/METHODOLOGY.md](/docs/methodology/METHODOLOGY.md)` | repo-rooted, OK |

**Action**: amendment brief execution-time verification; surgical fix any broken links.

### §6.4 Falsifiability claim wording consistency

**Pipeline-agnostic falsifiable claim** (from Q-A07-4=γ + Q3.c-reformulated + Q12.c-formulation):

> «Executor produces correct code on first build at a measurable rate; target threshold under 30% requiring second execution.»

**Occurrence inventory**:

| Document | Section | Wording |
|---|---|---|
| METHODOLOGY | §2.2 Contracts as IPC, «Falsifiable claim» | «executor produces artifacts that pass the contract's tooling-checkable conditions on the first build at a measurable rate. The measurement metric is the fraction of tasks requiring a second execution round after the first build; the target threshold is under 30%.» |
| README | Pipeline section closing paragraph | «executor produces correct code under it on the first build at a measurable rate (target <30% requiring second execution)» |

**Consistency check**: both phrasings convey same falsifiable claim («first build correct, target <30% second execution»). Minor wording variance acceptable (each document's prose style). Test: a reader of either passage independently arrives at the same falsifiability test methodology.

### §6.5 Audience contract declaration consistency

**Canonical declaration**:

> «authored under agent-as-primary-reader assumption (Q-A07-6 lock 2026-05-10) — readers unfamiliar с the project's cross-reference density should use AI tooling for navigation»

**Occurrence inventory**:

| Document | Section | Wording style |
|---|---|---|
| METHODOLOGY | §0 Abstract footnote | Full declaration с Q-A07-6 reference |
| PIPELINE_METRICS | Top-of-document frame note | Implicit (whole frame note IS the audience contract declaration); explicit reference к METHODOLOGY corpus |
| README | Pipeline section closing | Brief declaration + AI tooling guidance (gateway tone, fewer technical references) |

**Consistency check**: three audience contract declarations point к same Q-A07-6 lock, use consistent «agent-as-primary-reader» phrasing. README declaration shorter (gateway role); METHODOLOGY declaration full с lock reference; PIPELINE_METRICS implicit through frame structure.

---

## §7 Atomic commit shape recommendation

Per K-Lessons «atomic commit as compilable unit» (METHODOLOGY v1.5 + Q8.c-formulation «milestone consolidation under session-mode pipeline»): docs-only milestones use per-document atomic commits where each commit leaves the document в coherent reviewable state.

A'.0.7 amendment brief execution splits into 4 atomic commits.

### §7.1 Per-document atomic commits

| # | Commit | Files | Scope |
|---|---|---|---|
| 1 | `docs(methodology): A'.0.7 amendment — METHODOLOGY v1.5 → v1.6 pipeline restructure rewrite` | `docs/methodology/METHODOLOGY.md` | All edits per §1 of this plan (§1.1–§1.18) |
| 2 | `docs(methodology): A'.0.7 amendment — PIPELINE_METRICS v0.1 → v0.2 per-metric annotations + forward measurement plan` | `docs/methodology/PIPELINE_METRICS.md` | All edits per §2 of this plan (§2.1–§2.6) |
| 3 | `docs(methodology): A'.0.7 amendment — MAXIMUM_ENGINEERING_REFACTOR v1.0 → v1.1 pipeline-mapping update` | `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` | All edits per §3 of this plan (§3.1–§3.6) |
| 4 | `docs: A'.0.7 amendment — README Pipeline section v1.6 reality + agent-as-primary-reader audience contract` | `README.md` | All edits per §4 of this plan (§4.1–§4.4) |

**Plus closure commit**:

| # | Commit | Files | Scope |
|---|---|---|---|
| 5 | `docs(briefs): mark A'.0.7 brief EXECUTED; docs(progress): record A'.0.7 closure` | `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` + `docs/MIGRATION_PROGRESS.md` | Brief status line: AUTHORED → EXECUTED 2026-05-10 commit `<SHA>` reference; MIGRATION_PROGRESS A'.0.7 closure entry filled per Phase 4 of session |

**Total**: 5 atomic commits.

**Test count delta per commit**: zero (docs-only).

**Each commit leaves repo в compilable + tests-passing state** (docs-only; baseline 631 preserved throughout).

**Estimated execution time**: 30–60 min auto-mode total.

### §7.2 Possible fold с A'.1 К-L3.1 amendment per brief §6.2

**A'.0.7 brief §6.2 fold optionality**: «A'.0.7 amendment brief may fold into A'.1 К-L3.1 amendment brief execution as combined Phase. Decision at amendment brief authoring time».

**Decision surface** (deferred к amendment brief authoring time, not А'.0.7 closure scope):

**Path α — Standalone A'.0.7 amendment brief**:
- 5 commits per §7.1 land in single A'.0.7 amendment session (Cloud Code execution mode)
- A'.1 (К-L3.1 amendment) executes separately afterwards, 10 commits per К-L3.1 amendment plan §6.2
- Total Phase A' commit cadence: A'.0.7 amendment (5) → A'.1 К-L3.1 amendment (10)

**Path β — Combined A'.0.7 + A'.1 amendment brief**:
- Single Cloud Code execution session lands both amendment plans
- 5 + 10 = 15 commits в one execution session
- A'.0.7 amendment commits land first (methodology corpus established as terminology source), then К-L3.1 amendment commits land (architecture corpus updated с post-А'.0.7 terminology where it surfaces)

**Recommendation surface**: Path α (standalone) likely cleaner — different document scopes (methodology vs architecture); separate atomic commit groupings; easier review per-amendment. Path β saves one session invocation overhead but mixes scopes within single session. Decision deferred к amendment brief authoring time.

### §7.3 Push to origin sequencing

Per Phase A' sequencing §A'.3, push to origin deferred к after A'.1 К-L3.1 amendment execution. A'.0.7 amendment commits stay local until then.

Per memory edit и А'.0.7 brief §6.3 «No push to origin — deferred к A'.3 per Phase A' default sequencing». A'.0.7 amendment lands on `main` locally; push happens after A'.1.

---

## §8 What this plan deliberately does not do

- **No source code changes**. Plan covers documentation only.
- **No test additions**. Plan is docs-only; test count delta zero per commit.
- **No К-L3.1 amendment work**. К-L3.1 amendment plan (`docs/architecture/K_L3_1_AMENDMENT_PLAN.md`) is separate; A'.1 milestone scope.
- **No К-closure report drafting**. К-closure report is A'.8 milestone scope.
- **No К-L3.1-derived methodology lesson formalization**. Per Q-A07-7.b lock, К-L3.1 closure clarification observation defers к A'.8 К-closure report scope.
- **No architectural analyzer milestone work**. Analyzer is A'.9 milestone scope.
- **No PIPELINE_METRICS v1.6 era data refresh**. Per Q10.a annotations, v1.6 measurement collection is forward-looking; tracked в new §6 Forward measurement plan as backlog.
- **No PIPELINE_METRICS v1.6 era reproducibility refresh**. §5.1 / §5.2 / §5.3 receive `[transfers с reframing]` annotations; reformulation tasks live в §6 Forward measurement plan; not А'.0.7 amendment execution scope.
- **No methodology version-2.0 commitment**. v1.5 → v1.6 (semantic minor for pipeline reality update; principles unchanged). MAXIMUM_ENGINEERING_REFACTOR v1.0 → v1.1 (similar minor). PIPELINE_METRICS v0.1 → v0.2 (annotations layer addition).
- **No К-L3.1 lock re-deliberation**. К-L3.1 amendment plan separately authored; this A'.0.7 amendment plan doesn't override or reformulate К-L3.1 locks (Q1–Q6 there are distinct from Q1–Q12 here).
- **No push к origin**. Deferred к Phase A'.3.

---

## §9 Document provenance

- **Plan authored**: 2026-05-10, Claude Desktop session, deliberation mode (A'.0.7 session Phase 3 deliverable)
- **Authoring approach**: Option P3-2 — structural + key drafts only (substantive sub-sections с full old/new pairs; verify-clean sub-sections с lock references + grep targets). Phase 3 of A'.0.7 deliberation session
- **Authority**: A'.0.7 session locks Q-A07-1 through Q-A07-8 + Q1 through Q12 + synthesis form (§4.A-primary с document-specific §4.C for PIPELINE_METRICS)
- **Execution target**: separate amendment brief (Cloud Code execution mode, 30–60 min auto-mode estimate); 5 atomic commits per §7.1
- **Companion documents**:
   - `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` (commit `55d9e36`, EXECUTED via this plan landing)
   - `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (К-L3.1 amendment plan, A'.1 scope)
   - `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (Phase A' anchor reference)
- **Memory tracker**: `userMemories` 2026-05-10 entries (4 edits): К-L3.1 closure + skeleton brief states + analyzer milestone purpose + pipeline restructure с session mode distinction
- **Session participants**: Crystalka (direction owner) + Claude Opus 4.7 (architect, deliberation mode)
- **Session length**: ~3 hours (from Phase 0 pre-flight reads through Phase 3 amendment plan authoring)

---

**Plan end. Amendment brief authoring + execution is a separate session deliverable; this plan is the input contract.**
