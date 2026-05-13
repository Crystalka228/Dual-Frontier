---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-G-README
category: G
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-G-README
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-G-README
category: G
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-G-README
---
# Dual Frontier

A falsifiable test of LLM-augmented systems engineering at one-person scale.

---

## Claim under test

The hypothesis is that a single developer, in the role of contract architect,
can build a non-trivial systems-software artifact through structured work with
a tiered LLM pipeline, where the LLMs operate as executors inside contract
boundaries rather than as substitutes for engineering judgement.

The claim is operationalized as the production of a moddable simulation engine
with declared invariants — a multithreaded ECS, capability-based mod isolation,
and a replaceable native core — built solo, with measured pipeline throughput
and a recorded defect rate. The colony-simulator content sits on top of the
engine as a realistic load; the engine exists to stress-test the methodology
under non-trivial workload.

## Falsifiability conditions

The claim is rejected if any of the following hold over a sustained
development window:

1. **Defect rate.** The shipped artifact accumulates production-class defects
   that the contract-and-test infrastructure was supposed to prevent.
   Current state: 0 known production bugs across the closed phases; full test
   counts and acceptance criteria are recorded in
   [docs/ROADMAP.md](./docs/ROADMAP.md).
2. **Architectural integrity.** The architecture drifts under sustained
   activity — locked specifications stop reflecting the code, contracts
   weaken to accommodate executor limitations, or isolation guarantees erode.
   Current state: architectural decisions and their rejected alternatives are
   recorded in [docs/architecture/ARCHITECTURE.md](/docs/architecture/ARCHITECTURE.md),
   [docs/architecture/MOD_OS_ARCHITECTURE.md](/docs/architecture/MOD_OS_ARCHITECTURE.md), and the
   normalization audit in [docs/reports/NORMALIZATION_REPORT.md](/docs/reports/NORMALIZATION_REPORT.md).
3. **Pipeline economics.** The pipeline cannot sustain its own throughput
   under a fixed monthly subscription and spills into pay-as-you-go API
   consumption to keep moving. Current state: two consecutive weekly windows
   under different operational profiles converge on the same headroom band;
   measurements are recorded in
   [docs/methodology/PIPELINE_METRICS.md §3](/docs/methodology/PIPELINE_METRICS.md#3-subscription-headroom).

Each condition has a documented source of truth; the present README does not
restate the numbers.

## Pipeline

The pipeline configures N agents in an architect-executor split with rigid contracts at boundaries: a human as direction owner; one or more LLM instances operating as architect (deliberation, brief authoring, QA review) and executor (mechanical application against authored briefs). The architect-executor split with contracts at boundaries is invariant across configurations; specific N, the boundary type between architect and executor (model-tier boundary, session-mode boundary, or mixed), and tier mix vary by pipeline configuration.

The agents do not communicate directly; coordination happens through LOCKED documents in the repository and through the human as session router.

**Current configuration (v1.6, 2026-05-10).** N=2: Crystalka (direction owner) plus a unified Claude Desktop session that switches between deliberation mode (chat interface, architectural decision recording per K8.0 / K-L3.1 / A'.0.7 precedent) and execution mode (Claude Code agent, autonomous tool-loop per A'.0.5 precedent). Boundary type: session-mode.

v1.x era (Phase 0–8, ending 2026-05-09) used model-tier boundary с N=4 (local quantized Gemma executor + cloud Sonnet prompt-generator + cloud Opus architect + human direction owner). Empirical record preserved in [docs/methodology/PIPELINE_METRICS.md](/docs/methodology/PIPELINE_METRICS.md) с per-metric transferability annotations.

Full pipeline configuration, empirical task metrics, subscription headroom data, and reproducibility requirements documented in [docs/methodology/PIPELINE_METRICS.md](/docs/methodology/PIPELINE_METRICS.md). Full methodology documented in [docs/methodology/METHODOLOGY.md](/docs/methodology/METHODOLOGY.md). The methodology and deeper documents are authored under **agent-as-primary-reader assumption** — readers unfamiliar с the project's cross-reference density should use AI tooling for navigation through the documentation corpus.

If a contract is rigid enough that an executor produces correct code under it on the first build at a measurable rate (target <30% requiring second execution), the contract will hold under any stronger executor or any restructured boundary type. Isolation from executor errors is a structural property of the contract, not of the executor's specific capacity.

## What the engine demonstrates

The engine is the stress test. Without a non-trivial workload, the pipeline
claim reduces to a statement about toy problems. The engine carries three
properties that make the workload non-trivial:

- A multithreaded ECS with declarative system access (`[SystemAccess]`),
  a Kahn-sorted dependency graph, and a runtime isolation guard that crashes
  immediately on any access violation.
- Capability-based mod isolation: each mod loads into its own
  `AssemblyLoadContext`, sees only `DualFrontier.Contracts`, and interacts
  with the kernel through reflection-scanned capabilities. The architecture
  is documented as an OS-style design in
  [docs/architecture/MOD_OS_ARCHITECTURE.md](/docs/architecture/MOD_OS_ARCHITECTURE.md).
- A replaceable native core, treated as an experimental boundary rather than
  a load-bearing assumption. One negative result against this boundary is
  recorded with criterion reformulation in
  [docs/reports/NATIVE_CORE_EXPERIMENT.md](/docs/reports/NATIVE_CORE_EXPERIMENT.md).

## What this is not

This repository is not a game release, a competitor to Bevy or Unity DOTS,
or a claim that LLM pipelines can replace software engineers. It is also not
a claim about generalizability beyond systems software with formal,
machine-checkable contracts. The boundaries of applicability are recorded
in [docs/methodology/METHODOLOGY.md §6](/docs/methodology/METHODOLOGY.md).

## Three primary documents

- [docs/methodology/METHODOLOGY.md](/docs/methodology/METHODOLOGY.md) — the methodology as designed:
  pipeline architecture, contracts as inter-agent IPC, verification cycle,
  threat model, boundaries of applicability.
- [docs/architecture/MOD_OS_ARCHITECTURE.md](/docs/architecture/MOD_OS_ARCHITECTURE.md) — the
  capability-based mod isolation as an OS-style architecture; v1.0 LOCKED.
- [docs/reports/NATIVE_CORE_EXPERIMENT.md](/docs/reports/NATIVE_CORE_EXPERIMENT.md) — a
  measured negative result with explicit criterion reformulation.

## Repository layout

The full documentation index is in [docs/README.md](./docs/README.md).
Source layout is described in [docs/architecture/ARCHITECTURE.md](/docs/architecture/ARCHITECTURE.md);
without it the assembly structure looks excessive.

## License

This project is distributed under the
[PolyForm Noncommercial 1.0.0](./LICENSE) license. Commercial use of the
engine code requires a separate agreement.

### Architecture documents

- `docs/methodology/METHODOLOGY.md` — pipeline и methodology
- `docs/methodology/CODING_STANDARDS.md` — coding conventions
- `docs/architecture/MOD_OS_ARCHITECTURE.md` — modding architecture
- `docs/architecture/RUNTIME_ARCHITECTURE.md` — Vulkan rendering layer (M9.x)
- `docs/architecture/KERNEL_ARCHITECTURE.md` — native ECS kernel layer (K0-K8)
- `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` — Discovery report (experimental branch)
