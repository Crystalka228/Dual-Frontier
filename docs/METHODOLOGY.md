# Dual Frontier development methodology

*The project's central methodology document. Describes the four-agent LLM pipeline, contracts as IPC between agents, the verification cycle, economics, threat model, empirical results, and boundaries of applicability.*

*Version: 1.0 (2026-04-25). The document describes the methodology in its state after Phase 4 closure.*

---

## 0. Abstract

This document describes a methodology for developing complex software solo through structured work with an LLM. The methodology tests a hypothesis: a human in the role of contract architect plus LLMs in the role of executors inside strict contract boundaries can produce engineering systems of the same complexity class that usually requires teams of several developers over months.

The configuration is four agents with explicitly distributed roles: a local quantized model in the 4–8B parameter class as code executor, a mid-tier cloud model as prompt generator, a top-tier cloud model as architect and QA, the human as direction owner. There is no direct coordination between agents — formal contracts in code and documentation in the repository tie them together, acting as inter-process communication.

The methodology's main falsifiable claim: a working production-quality game, built by one developer through this methodology in 6–12 months, with measured pipeline performance, defect rate, and architectural integrity over the long haul. Empirical measurements of the pipeline operating against this methodology are recorded in [PIPELINE_METRICS](./PIPELINE_METRICS.md) — see particularly [§2 task-level metrics](./PIPELINE_METRICS.md#2-empirical-task-level-metrics) and [§4 sustained throughput](./PIPELINE_METRICS.md#4-sustained-throughput). Additional falsifiable claims appear in §2.2, §3.1, §4.5, and §5.3.

The methodology is not universal. Boundaries of applicability are recorded in §6.

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

The pipeline uses four agents. Each has a narrow specialized role and operates at the complexity level where its capacity is justified — no higher.

**Local executor.** A quantized model in the 4–8B parameter class, run locally via an inference server with an OpenAI-compatible API. In the Dual Frontier reference configuration this is Gemma 4 E4B Q4_K_M (6.33 GB, 131,072-token context window) through LM Studio. The orchestrator is the Cline extension in VS Code, which feeds the model the current project's context and applies the generated code to the filesystem. The executor's role is routine code generation from a self-contained prompt: 1 prompt → 1–2 files. It makes no architectural decisions. It receives a contract and produces code that satisfies the contract.

**Prompt generator.** A mid-tier cloud model. Turns a task plus its contract into a self-contained prompt for the local executor. Knows the architecture as a whole and formulates precise instructions with namespace, using directives, class signatures, field names, types, default values, and commit-message format. Additionally handles mid-complexity tasks directly, without delegating to the local executor.

**Architect and QA.** A top-tier cloud model with a large context window. Used sparingly — on hard architectural tasks (the scheduler, the dependency graph, non-trivial algorithms) and during full reviews at the closure of each development phase. The ability to operate on the corpus as a whole, rather than individual files, structurally distinguishes the architect from the executor.

**Human.** Direction owner. Selects contracts, makes architectural decisions, frames phase goals. Per unit of time, types noticeably fewer keystrokes than a classical developer and makes noticeably more architectural decisions.

### 2.2 Contracts as IPC between agents

This is the central methodological device. In traditional development, contracts — interfaces, types, protocols — serve as a communication mechanism between subsystems of the code. In an LLM pipeline they additionally become a communication mechanism between agents.

A rigid contract is the point where interpretations converge. If a contract is formulated strictly enough to exclude ambiguity, any two agents (a local quantized model and a cloud architect, asynchronously, without direct coordination) will produce compatible artifacts. This works because the contract specifies a condition, not a preference — the compiler, tests, or a static analyzer can check satisfaction mechanically.

The same role for contracts operates between agents in one pipeline during phase reviews: the mid-tier agent leaves formalized diagnostics as a numbered list of items with file and line references; the top-tier agent in the next session receives this diagnostic as input and treats it as a contract. Coordination between agents is resolved through the repository and formal documents, not through direct message exchange.

**Falsifiable claim.** If the architecture is fixed as a set of contracts satisfying a "sufficient rigor" criterion, a quantized model in the 4–8B parameter class on average produces code that passes tests on those contracts on the first build. The measurement metric is the fraction of tasks requiring a second generation round after the first build; the target threshold is under 30%. An empirical example of asynchronous development against a formal contract without human participation appears in §4.2.

Contracts in Dual Frontier live in a separate `DualFrontier.Contracts` assembly that contains no implementation (see [CONTRACTS.md](./CONTRACTS.md)). It acts as the single vocabulary for every layer of the system: the core, game systems, mods, and build tooling.

### 2.3 Verification cycle

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

### 2.4 Atomicity of phase review

An architectural decision and its implementation during a phase review are **not separable**. When the architect formulates a decision, the architect verifies it through implementation: writes the code, runs the tests, sees one test fail, traces the cause, fixes the logic. Without the implementation step it is unknown whether the decision will work in reality or stay as an elegant formulation on paper.

This means that **splitting architecture and implementation between agents saves tokens in theory but creates desynchronization points in practice**. Each such point requires returning to the architect for analysis and reformulation, which in sum spends more tokens and more calendar time than an atomic session. An atomic architect session is not an expensive compromise — it is the methodologically correct way to use an architectural agent in the QA role.

---

## 3. Pipeline economics

### 3.1 Principles

Division-of-labor principle between free local and paid cloud: routine work runs locally, architectural work runs in the cloud. This keeps pipeline operation sustainable under a fixed subscription, with no spillover into pay-as-you-go.

Cost-asymmetry principle: the bulk of the code is generated by the local quantized model for free; the subscription pays only for architectural work. This inverts the usual economics of LLM-driven development, where the cloud model is used for everything and quickly runs into limits or invoices. The phase-review atomicity principle (§2.4) caps the savings from splitting work between agents in favor of session integrity.

**Falsifiable claim.** Given properly formulated contracts, at least 70% of the lines of code in the repository are generated by the local model without calling a paid API. The measurement metric is the counter from the local agent's interaction history, cross-referenced with the git log by date and authorship.

### 3.2 Empirical record at Phase 4 closure

**Base cycle (local executor + prompt generator):**

| Task | Context (tokens) | Output (tokens) | Artifact size |
|---|---|---|---|
| Implement InventorySystem | 13 900 | 1 600 | 55.9 kB |
| Implement power grid events | 99 900 | 2 800 | 80.1 kB |
| Implement ItemReservedEvent | 79 300 | 2 100 | 63.9 kB |
| Implement ItemAddedEvent | 132 900 | 2 800 | 79.5 kB |
| Implement StorageComponent | 98 200 | 3 800 | 83.8 kB |

Variable cost per task is zero. Cloud prompt-generator usage is about 10% of the weekly subscription budget.

**Phase review (Phase 4 closure, 2026-04-25):**

| Parameter | Value |
|---|---|
| Session context window | ~444k tokens out of 1M (44%) |
| 5-hour limit consumption | ~30% in a single session |
| Weekly all-models budget consumption | ~34% in a single session |
| Session wall-clock time | ~35 minutes from first to last commit |

**Subscription-tier sustainability condition.** The current cadence (one Opus phase session every 1–2 weeks plus base prompt-generator and local-executor work in between) fits inside Claude Max 5× at $100/month. A faster cadence requires moving up to Max 20× ($200/month) or to API pay-as-you-go.

### 3.3 Comparison with alternative configurations

The same workload through direct API calls without a local model runs to tens of cents per task at Opus-tier prices — tens to hundreds of dollars per month. Through agentic coding tools with broad permissions (see §5) the economics are comparable, but additional risk classes appear that are structurally absent from the pipeline. Comparison with classical non-LLM development is in §4.3.

---

## 4. Empirical results from Dual Frontier

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

### 4.2 Case study: asynchronous native-core development

The idea for an experimental branch with a native C++ core emerged during the author's shift work outside the home. The prompt was sent to the cloud architect from a mobile device; by the time the author returned home, the `claude/cpp-core-experiment-cEsyH` branch had been built by the agent, published to the repository, and made available for local build. A local `dotnet build` of the solution passed on the first try with no edits.

Several hours of asynchronous work passed between task formulation and working artifact, during which the author could not physically participate. This works because `DualFrontier.Contracts` is rigid enough to play the IPC role between agents without synchronous coordination. The cloud agent wrote code against a formal interface; the local build verified conformance to the interface. Divergence between intent and result is physically impossible when the code passes the build and the tests, because the very notion of "correct code" is fixed in the contract, not in interpretation.

The experiment itself ended with a negative result — per-element P/Invoke ate the native gain on the benchmark (NativeAdd10k: ratio 3.92× against the managed baseline). This is a separate methodological result: the acceptance criterion was reframed from mean latency to p99, GC pause, and long-run drift; the batch-API decision is deferred to Phase 9. Details are in [NATIVE_CORE_EXPERIMENT.md](./NATIVE_CORE_EXPERIMENT.md). What matters for this document: the cycle "hypothesis → asynchronous implementation → measurement → criterion reframing → deferred decision" fit into hours, not weeks, with no drop in code quality in the repository.

### 4.3 Comparison with typical indie development

The architectural base that exists in the repository on day 5 of its life would normally require 9–15 months of work for a team of 2–3 people. Custom multithreaded ECS with formal verification: 2–3 months. Mod isolation through `AssemblyLoadContext` with physical isolation: 1–2 months. Event bus with three delivery modes: 1 month. Persistence layer with RLE and StringPool: 2–3 weeks. Integration of every layer plus the Godot bridge: 1–2 months.

This yields roughly 60–100× compression against the typical pace of indie development on the architectural portion.

**Falsifiable claim.** Compression applies to architectural work. Game content, balancing, and narrative shrink less, because the "good/bad" criterion is subjective and verifiable only through playtesting. Falsifying it: measure the same indicator on a domain with a subjective criterion and find comparable acceleration there.

### 4.4 Phase-review wall-clock performance

The Phase 4 closure review by the architect took **approximately 35 wall-clock minutes** from first to last commit (per session log; the session's commits are squashed in the git history, so the timeline cannot be reconstructed directly from timestamps). Within those 35 minutes the architect validated the prompt-generator's diagnostic (10 items), discovered 5 additional issues (including an endemic `NotImplementedException` pattern across 22 systems), formulated 6 architectural decisions with explicitly rejected alternatives, implemented the decisions in code, wrote 17 new tests while debugging two self-introduced failures, and updated five documents. Result: tests 65/65 → 82/82, Phase 4 closed, Phase 5 unblocked.

In typical team development, an equivalent phase session would be 16–24 hours of total work by a team of 2–3 people, or several days on the calendar. Solo without an LLM: at least a week.

Active developer involvement in those 35 minutes amounted to framing the task on input, reading the result, and pushing the commits. The rest of the time the architect worked autonomously, including debugging two self-introduced test failures.

Full session log: [SESSION_PHASE_4_CLOSURE_REVIEW.md](./SESSION_PHASE_4_CLOSURE_REVIEW.md). *(Russian-language audit trail; preserved verbatim per the i18n campaign rules.)*

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

The four-agent architecture structurally avoids the main risk class through minimal permissions for each agent's role, not through "we will be careful."

**Local executor.** No network access (LM Studio runs on localhost). No shell. The filesystem is limited to what the orchestrator (the editor extension) exposes. Prompt injection through the project's file contents is theoretically possible but does not lead to data exfiltration, because there is no outbound channel.

**Prompt generator via desktop app.** Every action is initiated by the user through the chat interface. No heartbeat mode. No autonomous scheduled work. MCP tools are connected explicitly and visible to the user at every moment. Filesystem access through MCP is limited to directories explicitly permitted in the settings.

**Architect via desktop app.** Same as above, plus used sparingly (once every 1–2 weeks for a phase review), which reduces the attack surface across time.

**No direct channel between agents.** Coordination happens through files in the repository and through the human. This means compromise of one agent does not propagate to the others automatically: the attacker must compromise each agent separately, through different channels.

### 5.3 Falsifiable claims

Specific attack classes that are impossible in the pipeline for structural reasons, not "carefulness":

1. **Data exfiltration through the local executor is impossible** — it has no network access.
2. **Scheduled autonomous actions are impossible** — no agent has a heartbeat mode.
3. **Compromise propagation between agents through a direct channel is impossible** — no direct channel exists, only the repository and the human.
4. **Installation of a malicious skill is impossible** — the pipeline has no community-extensions repository with automatic installation.

These claims are verified by inspecting the pipeline architecture and do not depend on the behavior of individual models.

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

## 7. Reproducibility

### 7.1 Minimal configuration

To reproduce the pipeline:

- **VS Code** with the **[Cline](https://github.com/cline/cline)** extension — orchestrator for the local model.
- **LM Studio** or a compatible backend for the local model via an OpenAI-compatible API on `localhost`.
- Local model: **Gemma 4 E4B** (Q4_K_M) or comparable Qwen 2.5 Coder, Llama 3.1 8B, Mistral Nemo 12B alternatives. Minimum 8 GB VRAM or 16 GB unified memory (Apple Silicon).
- **Claude Max 5×** subscription ($100/month) and the Claude desktop app for architectural work through Sonnet 4.6 and Opus 4.7.

### 7.2 Confirmed configuration

The pipeline runs stably under sustained development (4–5 hours per evening after a work shift) on the following hardware:

- ASUS TUF Gaming A16 (system name "Skarlet")
- AMD Ryzen 7 7435HS @ 3.10 GHz
- AMD Radeon RX 7600S 8 GB
- 32 GB DDR5 4800 MT/s
- Windows 11 Home 25H2

### 7.3 Process discipline

Beyond tools, the methodology requires the following discipline:

- **Contracts are declared formally** through interfaces, attributes, and types. Every architectural decision has a machine-checkable satisfaction condition.
- **Tests are written as invariant proofs**, not as checkboxes. Each test verifies a specific claim, not "the system works as a whole."
- **Commits are atomic** with a scope prefix (`feat(scope): ...`, `fix(scope): ...`, `docs: ...`, `refactor(scope): ...`). After every commit the project is in a working state.
- **`TreatWarningsAsErrors` is enabled** from the start. Technical debt does not accumulate invisibly.
- **Documentation is written in parallel with the code**, not after. After each substantial phase closes: update the architectural documents and run the self-teaching ritual (§4.5).
- **Phase reviews by the architect** run as atomic sessions: architecture, implementation, tests, debugging, and documentation in one pass (§2.4).

Without this discipline, the tools yield a worse result than without them.

---

## 8. Open questions

The methodology has been tested on a 5-day horizon with one formalized phase-review session. Several questions need further investigation.

**Long-term dynamics.** All falsifiable conditions cited are tested on a 5-day horizon. Pipeline behavior over 6–12 months is unknown. Does the cadence persist? Does defect rate grow? Does self-teaching get harder as the corpus grows? These questions require systematic metric collection across Phases 5–9.

**Drift of the developer's architectural understanding.** If the author goes years without writing code by hand, does their architectural understanding persist? The self-teaching ritual (§4.5) is meant to prevent this drift, but the ritual's effectiveness has not been measured quantitatively.

**Applicability to team development.** Every argument here applies to solo development. Whether formal contracts suffice as IPC between teams, or whether additional methodological devices are needed, is an open question with its own literature.

**Degradation as the codebase grows.** The local model's 131k-token context is already close to the limit for large tasks (132.9k for the `Implement ItemAddedEvent` task in Phase 4). With further project growth, the pipeline may need restructuring: splitting the corpus into modules with independent context, switching to models with larger context windows, or using retrieval instead of a fully loaded context.

**Behavior on series of negative results.** Dual Frontier has one publicly recorded negative result ([NATIVE_CORE_EXPERIMENT.md](./NATIVE_CORE_EXPERIMENT.md)). The pipeline handled it correctly as a methodological artifact. But that is one case. The pipeline's systematic behavior on series, when several architectural hypotheses in a row prove wrong, has not been tested.

**Reproducibility by other developers.** The pipeline has been tested by one author on one project. Reproducibility by other developers with different backgrounds and on different tasks has not been tested. This is the most important open question: if the pipeline reproduces only for the author and only on this project, the methodology becomes a private finding rather than a method.

---

## 9. Change history

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-04-25 | First public version of the document after Phase 4 closure. |

The document is updated after each substantial phase closes. Substantial methodological shifts (changes to pipeline configuration, changes to role distribution, additions or removals of methodological devices) are recorded as major versions.

---

## 10. See also

- [README.md](../README.md) — research framing, falsifiability conditions, and pointers to operational data.
- [PIPELINE_METRICS.md](./PIPELINE_METRICS.md) — empirical configuration, throughput data, and subscription economics measured while running this methodology.
- [learning/PHASE_1.md](./learning/PHASE_1.md) — self-teaching ritual artifact after Phase 1; direct empirical referent for §4.5.
- [SESSION_PHASE_4_CLOSURE_REVIEW.md](./SESSION_PHASE_4_CLOSURE_REVIEW.md) — Phase 4 closure review session log; direct empirical referent for §4.4. *(Russian-language audit trail; preserved verbatim per the i18n campaign rules.)*
- [ARCHITECTURE.md](./ARCHITECTURE.md) — layers, dependency rules, scenarios.
- [CONTRACTS.md](./CONTRACTS.md) — the contract system, six domain buses, contract evolution.
- [DEVELOPMENT_HYGIENE.md](./DEVELOPMENT_HYGIENE.md) — hygiene checklist for every PR, the engine/game boundary.
- [ISOLATION.md](./ISOLATION.md) — the isolation guard, types of violations, DEBUG vs RELEASE.
- [NATIVE_CORE_EXPERIMENT.md](./NATIVE_CORE_EXPERIMENT.md) — negative result of the C++ core, criterion reframing.
- [GPU_COMPUTE.md](./GPU_COMPUTE.md) — deferred decision, switchover threshold.
- [ROADMAP.md](./ROADMAP.md) — phases, dependency reasoning, the bridge pattern between Phases 5 and 6.
