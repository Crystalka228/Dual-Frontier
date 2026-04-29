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
   recorded in [docs/ARCHITECTURE.md](./docs/ARCHITECTURE.md),
   [docs/MOD_OS_ARCHITECTURE.md](./docs/MOD_OS_ARCHITECTURE.md), and the
   normalization audit in [docs/NORMALIZATION_REPORT.md](./docs/NORMALIZATION_REPORT.md).
3. **Pipeline economics.** The pipeline cannot sustain its own throughput
   under a fixed monthly subscription and spills into pay-as-you-go API
   consumption to keep moving. Current state: two consecutive weekly windows
   under different operational profiles converge on the same headroom band;
   measurements are recorded in
   [docs/PIPELINE_METRICS.md §3](./docs/PIPELINE_METRICS.md#3-subscription-headroom).

Each condition has a documented source of truth; the present README does not
restate the numbers.

## Pipeline

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
[docs/PIPELINE_METRICS.md](./docs/PIPELINE_METRICS.md):

- [§1 Pipeline configuration](./docs/PIPELINE_METRICS.md#1-pipeline-configuration) —
  agent assignments, workflow, hardware, and software stack.
- [§3 Subscription headroom](./docs/PIPELINE_METRICS.md#3-subscription-headroom) —
  empirical measurements across two consecutive weekly windows.
- [§5 Reproducibility requirements](./docs/PIPELINE_METRICS.md#5-reproducibility-requirements) —
  what is required to reproduce these measurements and what these
  measurements do not show.

If a contract is rigid enough that a 4-bit quantized local model produces
correct code under it, the contract will hold under any stronger executor.
Isolation from hallucinations is a structural property of the contract, not
of the executor's capacity.

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
  [docs/MOD_OS_ARCHITECTURE.md](./docs/MOD_OS_ARCHITECTURE.md).
- A replaceable native core, treated as an experimental boundary rather than
  a load-bearing assumption. One negative result against this boundary is
  recorded with criterion reformulation in
  [docs/NATIVE_CORE_EXPERIMENT.md](./docs/NATIVE_CORE_EXPERIMENT.md).

## What this is not

This repository is not a game release, a competitor to Bevy or Unity DOTS,
or a claim that LLM pipelines can replace software engineers. It is also not
a claim about generalizability beyond systems software with formal,
machine-checkable contracts. The boundaries of applicability are recorded
in [docs/METHODOLOGY.md §6](./docs/METHODOLOGY.md).

## Three primary documents

- [docs/METHODOLOGY.md](./docs/METHODOLOGY.md) — the methodology as designed:
  pipeline architecture, contracts as inter-agent IPC, verification cycle,
  threat model, boundaries of applicability.
- [docs/MOD_OS_ARCHITECTURE.md](./docs/MOD_OS_ARCHITECTURE.md) — the
  capability-based mod isolation as an OS-style architecture; v1.0 LOCKED.
- [docs/NATIVE_CORE_EXPERIMENT.md](./docs/NATIVE_CORE_EXPERIMENT.md) — a
  measured negative result with explicit criterion reformulation.

## Repository layout

The full documentation index is in [docs/README.md](./docs/README.md).
Source layout is described in [docs/ARCHITECTURE.md](./docs/ARCHITECTURE.md);
without it the assembly structure looks excessive.

## License

This project is distributed under the
[PolyForm Noncommercial 1.0.0](./LICENSE) license. Commercial use of the
engine code requires a separate agreement.
