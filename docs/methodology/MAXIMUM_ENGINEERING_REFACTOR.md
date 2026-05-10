---
title: Maximum Engineering Refactor Brief
nav_order: 109
---

# Maximum Engineering Refactor — Discipline Escalation Brief

*A focused refactoring proposal to escalate the project's engineering discipline from its current level (subjectively assessed at 7/10 deliberate over-engineering) toward the structural maximum (10/10). The refactor adds three load-bearing discipline layers: formal verification of critical paths, type-theoretic architecture specification, and a methodology replication kit. Each addition is independently valuable, internally consistent with existing LOCKED specifications, and architecturally compatible with the foundation laid by M0–M10.*

*Version: 1.0 (2026-05-06). RATIFIED. Three independent tracks (A: Verification, B: Type System, C: Replication Kit) added to backlog per §5.3. Activation of any track requires per-track brief at activation time.*

---

## 0. Preamble

This brief proposes three distinct discipline escalations, each scoped as an independent refactoring track. The tracks are not bundled — adopting one does not require adopting the others — but they share architectural framing and reference the same foundation.

Each track produces:

- A new LOCKED specification document under `docs/`.
- Concrete refactoring work in the existing source tree.
- Verification artifacts that participate in the existing audit discipline.

The brief itself follows the same authoring convention as other research-tier documents in the project: enumerated decisions, named rationale, deferred questions explicitly marked, ratification process inherited from [METHODOLOGY](./METHODOLOGY.md).

The escalation is **deliberate**. The current state is functional and proportionate to declared goals. The escalation is not corrective — it is intentional indulgence in discipline that the project's hobby framing permits and the architectural foundation supports.

---

## 1. Context and motivation

### 1.1 Current discipline level

Per the self-assessment captured in conversation logs (2026-05-06):

- ~30% of the project: clearly proportionate to stated goals (functional core).
- ~50% of the project: defensible under research artifact framing (methodology supports).
- ~20% of the project: deliberate over-engineering (FHE contract, reservoir entries with extensive cross-references).

Subjective rating: **7/10 over-engineered**, where 10/10 represents the structural maximum achievable while preserving project coherence.

The remaining 3 points reflect three specific gaps:

1. **No formal verification anywhere in the corpus.** All correctness claims rest on tests + types + audits. No mathematical proof of any property.
2. **Architecture specified in prose, not in types.** Cross-module relationships described in documentation, not enforced through a formal type theory.
3. **No replication protocol for the methodology.** [PIPELINE_METRICS](./PIPELINE_METRICS.md) §5.1–5.4 describe reproducibility requirements; no executable kit exists for actual replication.

This brief addresses each gap with a dedicated track.

### 1.2 Why escalate

Three motivations:

- **Hobby permits indulgence in discipline as primary goal.** The project is not under shipping pressure; engineering quality is itself part of the deliverable.
- **AI pipeline benefits from richer formal context.** Type-theoretic and verification artifacts are highest-density possible context for AI agents — denser than prose specifications.
- **Replication protocol matters most for the central research claim.** [METHODOLOGY](./METHODOLOGY.md) §8 explicitly identifies "reproducibility by other developers" as the most important open question. Closing it is the strongest possible support for the central hypothesis.

### 1.3 What this brief is not

- Not a commitment. Tracks may be adopted in any order, subset, or not at all.
- Not a substitute for shipping. Phase 5–7 sequence remains primary; this work is parallel-track for hobby development time.
- Not a critique of existing discipline. Existing LOCKED corpus is sound; this brief extends it, not replaces it.

---

## 2. Track A — Formal Verification of Critical Paths

### 2.1 Goal

Mathematical proof of correctness for one or more security-critical code paths, ratified as LOCKED specification with proof artifacts checked into the repository.

### 2.2 Target paths

Initial scope: **one** critical path. Candidate selection criteria:

- Security-critical (failure has external consequences).
- Stable (path has not changed across recent M-phases).
- Self-contained (proof does not require modeling the entire system).

Three viable candidates from existing code:

**Candidate A1: Capability validation in `ContractValidator`**

Property to verify:
> `ContractValidator.Validate(manifest) returns Success` ⟹ `every capability used by the mod's code is declared in `manifest.Capabilities.Required``

Strength: most security-critical path in the project. Proof closes the entire mod-isolation threat model formally.
Difficulty: requires modeling reflection scan and capability comparison. Moderate.

**Candidate A2: Isolation guard in `SystemExecutionContext`**

Property to verify:
> `SystemExecutionContext.GetComponent<T>()` does not return ⟹ `the calling system has declared read access to T via [SystemAccess]`

Strength: directly enforces architectural invariant.
Difficulty: simpler than A1; access declaration is a static attribute lookup.

**Candidate A3: Determinism preservation in `ParallelSystemScheduler`**

Property to verify:
> Two runs of `ParallelSystemScheduler.Execute()` over the same `World` state with the same input events produce bit-identical output state.

Strength: closes the determinism invariant from [METHODOLOGY](./METHODOLOGY.md) §7.1 formally.
Difficulty: highest. Requires modeling parallelism and scheduling policy.

**Recommendation**: start with **A2** as pilot integration. Lowest difficulty, still meaningful, establishes the verification toolchain for later application to A1 and A3.

### 2.3 Toolchain selection

Three candidate proof systems:

- **F\*** (Microsoft Research). Targets .NET ecosystem natively. Best fit for C# verification.
- **Lean 4**. Mature mathematical library. Better for pure math; harder for systems code.
- **Coq**. Most established; weakest .NET integration.

**Recommendation**: F\*. Project is C# / .NET native; F\* compiles to .NET and verifies pre/post conditions on .NET methods.

### 2.4 Deliverables

```
docs/VERIFICATION_CONTRACT.md     (v1.0 LOCKED)
src/DualFrontier.Verified/        (new project)
  ├── IsolationGuard.fst           (F* proof script)
  ├── IsolationGuard.fs            (extracted F# implementation)
  └── README.md                    (proof outline)
tests/DualFrontier.Verified.Tests/ (verifies extracted code matches behavior)
```

The extracted F# implementation replaces or wraps the existing C# `SystemExecutionContext.GetComponent<T>()`. The C# wrapper preserves API surface; the F#-extracted core carries the proof.

### 2.5 Acceptance criteria

- [ ] `docs/VERIFICATION_CONTRACT.md` v1.0 LOCKED.
- [ ] At least one property formally proved in F*.
- [ ] Extracted implementation passes existing isolation tests with no behavioral change.
- [ ] CI pipeline includes F* type-checking pass (proof verification on every commit).
- [ ] `PIPELINE_METRICS.md` §2 task-level metrics updated with verification overhead measurement.

### 2.6 Risk

- **Toolchain immaturity for game-shaped problems.** F* is research-tier; integration with Godot / .NET game runtime untested.
- **Proof maintenance burden.** Code changes may require proof updates; estimated +20-50% effort on touched files.
- **Single-developer toolchain.** Few engineers worldwide use F*; no community fallback for blockers.

Mitigation: pilot scope (A2 only). Falsifiable via: if pilot consumes >2 weeks total effort, abandon track and document negative result per [NATIVE_CORE_EXPERIMENT](/docs/reports/NATIVE_CORE_EXPERIMENT.md) precedent.

---

## 3. Track B — Type-Theoretic Architecture Specification

### 3.1 Goal

Architecture-level invariants currently expressed in prose specifications elevated to formal type-theoretic specification, enforced at compile time through C# type system extensions or external type checker.

### 3.2 Target invariants

Initial scope: invariants currently asserted only through prose + audits.

**Candidate B1: Layer dependency rules**

Currently in [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md):
> "Domain and Application never import `Godot;` and never import `Silk.NET;`. Contracts only."

Enforcement today: code review + audit passes.
Proposed enforcement: build-time analyzer that fails compilation if forbidden imports appear in forbidden assemblies.

**Candidate B2: Capability declaration completeness**

Currently in [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md):
> Every reflection-accessible component type used by a mod must have its capability declared in the manifest.

Enforcement today: runtime check at mod load time.
Proposed enforcement: Roslyn analyzer that scans mod source at compile time and verifies manifest declares every capability the code uses. Compile fails for missing declarations.

**Candidate B3: Bus channel correctness**

Currently in [CONTRACTS](/docs/architecture/CONTRACTS.md):
> Events targeting `ICombatBus` must inherit from `CombatEvent`; mismatches are a runtime error.

Enforcement today: runtime cast check.
Proposed enforcement: phantom type parameter on bus interfaces that makes mismatched publication a compile error.

### 3.3 Toolchain selection

Two candidate approaches:

**Approach B-α: Roslyn analyzers**

Native C# tooling. Analyzers run as part of compilation. Errors reported in IDE and CI. Mature ecosystem; many existing analyzers in production use.

**Approach B-β: External type checker (Idris-style)**

Formal type system implemented externally; runs as pre-build step. Allows richer types than C# permits (dependent types, refinement types). Higher expressiveness; lower integration ease.

**Recommendation**: Approach B-α. Mature ecosystem, clean integration with existing build pipeline, immediate IDE feedback.

### 3.4 Deliverables

```
docs/architecture/ARCHITECTURE_TYPE_SYSTEM.md   (v1.0 LOCKED)
src/DualFrontier.Analyzers/        (new project)
  ├── LayerDependencyAnalyzer.cs    (Candidate B1)
  ├── CapabilityCompletenessAnalyzer.cs (Candidate B2)
  ├── BusChannelAnalyzer.cs         (Candidate B3)
  └── README.md
tests/DualFrontier.Analyzers.Tests/ (analyzer behavior tests)
.editorconfig                      (analyzer severity configuration)
```

Each analyzer ships as separate type. CI fails build if any analyzer reports error severity. Warnings allowed during transition period; configurable via `.editorconfig`.

### 3.5 Acceptance criteria

- [ ] `docs/architecture/ARCHITECTURE_TYPE_SYSTEM.md` v1.0 LOCKED.
- [ ] At least one analyzer (B1 recommended as pilot) implemented and passing on existing codebase.
- [ ] CI pipeline includes analyzer run on every commit.
- [ ] Existing LOCKED specs that describe these invariants updated with cross-reference to enforcing analyzer.
- [ ] Per-analyzer test suite asserts both positive cases (correct code passes) and negative cases (incorrect code fails).

### 3.6 Risk

- **False positives on legitimate code.** Analyzer may flag patterns the codebase uses correctly; tuning required.
- **Drift between analyzer and spec.** If LOCKED spec changes, analyzer must update synchronously; triple-binding risk per [TRANSLATION_PLAN](./TRANSLATION_PLAN.md) §2.1 precedent.
- **Cognitive overhead.** Strict analyzer regime increases friction for legitimate refactoring.

Mitigation: pilot scope (B1 only). Severity starts at warning, escalates to error after observed false-positive rate stabilizes near zero.

---

## 4. Track C — Methodology Replication Kit

### 4.1 Goal

A complete, executable replication protocol that allows another developer to reproduce the methodology described in [METHODOLOGY](./METHODOLOGY.md) and the metrics measured in [PIPELINE_METRICS](./PIPELINE_METRICS.md), within statistical bounds.

### 4.2 Why this matters most

[METHODOLOGY](./METHODOLOGY.md) §8 explicitly identifies reproducibility as the most important open question:

> "Reproducibility by other developers. The pipeline has been tested by one author on one project. ... if the pipeline reproduces only for the author and only on this project, the methodology becomes a private finding rather than a method."

The current corpus describes the methodology with sufficient precision for understanding, but not for executable replication. A second developer attempting to reproduce would face many under-specified decisions: exact tool configurations, prompt templates, agent role boundaries, escalation criteria.

### 4.3 Replication kit contents

```
replication-kit/
  ├── README.md                        (overview + bootstrap instructions)
  ├── PROTOCOL.md                      (step-by-step procedure)
  ├── tools/
  │   ├── Dockerfile                   (reproducible environment)
  │   ├── tools-versions.lock          (exact versions of every tool)
  │   └── setup.sh                     (one-command environment bootstrap)
  ├── prompts/
  │   ├── opus-architect.md            (Opus role prompt template)
  │   ├── sonnet-decomposer.md         (Sonnet role prompt template)
  │   ├── gemma-executor.md            (Gemma role prompt template)
  │   └── escalation-criteria.md       (when to invoke which agent)
  ├── corpus/
  │   ├── task-001-baseline.md         (reference task with expected output)
  │   ├── task-002-medium.md           (medium-complexity reference task)
  │   └── task-003-complex.md          (high-complexity reference task)
  ├── benchmarks/
  │   ├── harness.py                   (measurement script)
  │   ├── expected-results.json        (baseline measurements)
  │   └── comparison-report.py         (replication vs baseline)
  └── analysis/
      ├── statistical-bounds.md        (acceptable variance per metric)
      └── interpretation-guide.md      (how to read results)
```

### 4.4 Reference task corpus

Three tasks with **known correct outputs** for replication:

- **Task 001 (baseline)**: Implement a simple component with tests. Tests defect rate, single-shot success rate.
- **Task 002 (medium)**: Implement a system that reads two components, publishes one event. Tests multi-step coordination.
- **Task 003 (complex)**: Implement a mod with manifest, capabilities, system, and event subscriber. Tests full pipeline coordination.

Each task includes:
- Exact specification (the prompt to feed the pipeline).
- Expected output structure (acceptable code shapes).
- Verification harness (automated check that output is correct).
- Expected metrics ranges (with statistical bounds).

### 4.5 Statistical methodology

Replication is statistical, not exact. Defined acceptable bounds:

- **Defect rate**: replication within ±10% of baseline.
- **Cycle time**: replication within ±25% of baseline (allows hardware variance).
- **Cost per task**: replication within ±15% of baseline.
- **Architectural integrity score**: replication within ±5% (most important metric).

Replications outside bounds are not failures but **data points** for refining the methodology specification.

### 4.6 Deliverables

```
docs/REPLICATION_PROTOCOL.md        (v1.0 LOCKED)
replication-kit/                    (new top-level directory)
  └── (contents per §4.3)
docs/methodology/PIPELINE_METRICS.md            (updated with §6 replication results, when available)
```

### 4.7 Acceptance criteria

- [ ] `docs/REPLICATION_PROTOCOL.md` v1.0 LOCKED.
- [ ] Replication kit complete per §4.3 file structure.
- [ ] One internal replication run completed by author (validates kit usability).
- [ ] Public invitation extended via repository README for external replications.
- [ ] At least one external replication attempted (success or failure both valuable data).

### 4.8 Risk

- **Cost.** External replication requires the same Anthropic Max subscription tier. Not all interested researchers can afford.
- **Hardware variance.** Local Gemma performance varies with GPU. Statistical bounds account for this but introduce noise.
- **Methodology drift.** As the project evolves, the kit must be kept current. Triple-binding risk again.
- **Negative result risk.** External replications may fail to reproduce key claims, falsifying the central hypothesis. This is the **point** of the kit — falsifiability requires the possibility of falsification — but psychologically difficult.

Mitigation: explicit framing in `REPLICATION_PROTOCOL.md` that negative results are publishable contributions, not failures.

---

## 5. Integration with existing roadmap

### 5.1 Sequencing

This brief proposes **parallel** tracks, not sequential. Each can begin independently of the others. Existing M0–M10 work is not blocked or modified by any track.

Recommended sequencing for hobby development time allocation:

```
Track A (Verification): Phase 6 sidecar (during Editor work)
                        Pilot: A2 (isolation guard)
                        Effort: ~2 weeks experiment, abort if blocked

Track B (Type system):  Phase 7 sidecar (during game shipping prep)
                        Pilot: B1 (layer dependency analyzer)
                        Effort: ~1 week initial analyzer

Track C (Replication):  Phase 8+ (post-shipping)
                        Effort: ~4 weeks for complete kit
                        Most valuable post-Phase-7 once baseline measurements stable
```

### 5.2 Parallel-track discipline

Each track adopts the same M-phase discipline as existing migration work:

- Brief authoring → Sonnet
- Implementation → Gemma
- Audit → Opus
- Closure review with byte-identical diff verification on the new LOCKED spec

This brief itself is the equivalent of an "M-phase prompt" but spanning multiple phases. Per-track briefs are written when the track activates.

### 5.3 ROADMAP.md integration

When this brief is ratified, append to `docs/ROADMAP.md` Backlog section:

```markdown
### Maximum engineering refactor (parallel track)

A three-track discipline escalation proposed in 
[MAXIMUM_ENGINEERING_REFACTOR](./MAXIMUM_ENGINEERING_REFACTOR.md) 
v1.0. Tracks adopt incrementally as Phase 6+ sidecar work:

- **Track A (Verification)**: F* proof of isolation guard property. 
  Pilot scope; falsifiable via 2-week effort budget.
- **Track B (Type system)**: Roslyn analyzers for architectural 
  invariants. Pilot scope; B1 layer dependency analyzer first.
- **Track C (Replication)**: Methodology replication kit for 
  external reproducibility validation. Post-Phase-7.

Adoption is opt-in per track. Tracks do not block shipping path.
```

---

## 6. Decision log

| ID | Decision | Rationale |
|----|----------|-----------|
| MR-1 | Three independent tracks, not bundled | Adopting one does not require adopting others. Reduces commitment threshold per track. |
| MR-2 | Each track has pilot scope before full commitment | Pilot scope falsifiable in bounded effort. Mirrors [NATIVE_CORE_EXPERIMENT](/docs/reports/NATIVE_CORE_EXPERIMENT.md) precedent of measured commitment with abort criteria. |
| MR-3 | Track A pilots A2 (isolation guard) not A1 (capability validator) | Lower difficulty establishes toolchain before tackling higher-stakes proof. |
| MR-4 | Track B uses Roslyn analyzers, not external type checker | Native ecosystem integration; mature tooling; immediate IDE feedback. |
| MR-5 | Track C is post-shipping (Phase 8+) | Replication of stable baseline matters more than replication of moving target. |
| MR-6 | Statistical bounds, not exact reproduction | Hardware and timing variance unavoidable. Statistical bounds preserve falsifiability while accepting noise. |
| MR-7 | Negative results from Track C are publishable | Falsifiability requires possibility of falsification. Failed external replications are data, not failures. |
| MR-8 | Briefs per track authored when track activates, not now | Avoid over-specification ahead of activation. Same discipline as MOD_OS_ARCHITECTURE deferred questions. |

---

## 7. Open questions deferred to per-track briefs

The following questions are deliberately not answered in this brief. They are addressed in per-track briefs when each track activates:

- Specific F* proof tactics for chosen property (Track A).
- Roslyn analyzer severity transition policy (warning → error timing) (Track B).
- Reference task corpus content beyond high-level shape (Track C).
- Statistical bound calibration (Track C, requires baseline measurement first).
- Cost model for external replication participants (Track C).
- Failure mode handling: what if a track partially succeeds and partially fails?

These are not gaps in v0.1. They are appropriately deferred to activation-time, when concrete context constrains answers usefully.

---

## 8. Falsifiability conditions for the brief itself

This brief is itself falsifiable:

- **If no track activates within 12 months of ratification**: the brief was speculative documentation, not actionable refactor proposal. Document as negative result, archive.
- **If pilot scope of any track exceeds 2× estimated effort**: the track is harder than anticipated; abort and document.
- **If three tracks all activate and complete successfully**: the discipline level genuinely reaches 10/10; record as proof of saturation.
- **If discipline saturation produces no architectural improvement**: over-engineering hypothesis confirmed; record limit of useful discipline escalation.

The brief is not a marketing document. It is a hypothesis about how much further the project's discipline can usefully scale.

---

## 9. Notes for the future architect

When activating any track, the following will likely be true:

- The toolchain selected here may have evolved (F* superseded by next-gen system; Roslyn deprecated for source generators v2; etc.).
- The project's needs may have shifted (a track that seemed valuable at v0.1 may seem optional later).
- One or more tracks may have been silently abandoned for legitimate reasons.

These outcomes are expected and acceptable. The brief is a snapshot of intent at 2026-05-06; activation reality determines actual scope.

The discipline this brief proposes is not corrective — the existing corpus is sound. The escalation is **deliberate indulgence** in a hobby that permits indulgence. If the indulgence stops being joyful, the brief should be archived without ceremony.

---

## 10. Ratification

Ratified at v1.0 on 2026-05-06 under the same discipline as
[IDEAS_RESERVOIR](./IDEAS_RESERVOIR.md) and [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md).

The brief joins the LOCKED corpus on equal standing with other research-tier
specifications. Per-track briefs will be authored when activation triggers
fire (see §5.1 for sequencing). Each track's activation is independent;
the brief itself remains stable across per-track activations unless
amended through the same ratification process.

### Version history

- **v1.0** (2026-05-06) — initial ratification. Three tracks (A/B/C) added
  to ROADMAP backlog. No track activated.

---

## See also

- [ROADMAP](./ROADMAP.md) — active surface; this brief proposes parallel-track addition to backlog.
- [METHODOLOGY](./METHODOLOGY.md) — methodology that Track C aims to make replicable; §8 open questions that motivate Track C.
- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) — capability model that Track A may verify formally; §3 isolation that Track B may enforce via analyzer.
- [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md) — layer rules that Track B (B1 analyzer) enforces.
- [PIPELINE_METRICS](./PIPELINE_METRICS.md) — baseline measurements that Track C aims to enable replication of.
- [ISOLATION](/docs/architecture/ISOLATION.md) — isolation guard that Track A's pilot (A2) targets.
- [NATIVE_CORE_EXPERIMENT](/docs/reports/NATIVE_CORE_EXPERIMENT.md) — precedent for measured commitment with abort criteria; cited in MR-2.
- [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md) — precedent for ratifying contracts before implementation activates; same discipline applies here.
- [IDEAS_RESERVOIR](./IDEAS_RESERVOIR.md) — reservoir for post-release ideas; this brief differs by proposing parallel-track work during active development.
