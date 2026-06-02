---
register_id: DOC-D-DOC_DRIFT_RECONNAISSANCE_BRIEF
category: D
tier: 4
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
last_modified: "2026-06-02"
last_modified_commit: "PENDING-COMMIT-DOC_DRIFT_RECON-ENROLLMENT"
content_language: en
review_cadence: on-cascade-execution
next_review_due: post-reconnaissance closure
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-DOC_DRIFT_RECONNAISSANCE_BRIEF
---
# Brief — Documentation Dual-Load Drift Reconnaissance

**Designation**: Documentation Dual-Load Drift Reconnaissance (working name; formal cascade ID pending Crystalka ratification — proposed A'.4.6 governance-lineage OR standalone DOC-DRIFT reconnaissance)
**Type**: Read-only reconnaissance + report authoring (multi-agent search session)
**Execution model**: Claude Code orchestrator (Opus) + parallel sub-agents (Task tool) decomposed по document category
**Output**: Single comprehensive report `docs/reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md`
**Authoring date**: 2026-05-25
**Authored by**: Claude Opus 4.7 (deliberation session)
**Status**: EXECUTED — reconnaissance executed 2026-06-02 by Claude Code (Opus); report placed at `docs/reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md`. See §closure footnote.

---

## §0 — Framing

### 0.1 — Problem diagnosis (Crystalka root-cause identification 2026-05-25)

Crystalka identified the **single largest source of documentation drift** (verbatim):

> «Я нашел самый главный источник дрифта, моя ошибка что у меня архитектурные документы выполняют ещё роль дорожной карты, это косяк и источник дрифта по отношению к коду и фазам.»

**Diagnosis — dual-load (двойная нагрузка)**:

Living-spec architecture documents carry TWO incompatible roles simultaneously:

1. **Descriptive / specification role** — encoding the *current* invariant state of the system (what *is*: К-L invariants, current architecture, implemented behavior).
2. **Roadmap / planning role** — encoding *future* planned state (what *will be*: deferred rules, future cascades, milestone plans, forward-implementation sequences).

These two roles have **opposite temporal contracts**:
- Spec content is meant to be **kept synchronized with code** — it drifts when code moves and spec doesn't follow.
- Roadmap content is meant to be **consumed and retired** — a plan is satisfied (or revised) by execution; once a phase ships, that roadmap entry should *shrink or disappear*, not persist as if still pending.

When both live interwoven в a single document section, every cascade execution worsens the rasсинхрон:
- Code advances → spec content must update forward
- Phase ships → roadmap content must shrink backward (realized portion removed)
- But без structural separation, these two operations **conflict in the same paragraph**, and the document accumulates a hybrid state that is neither accurate spec nor clean plan.

This is the classic **specification-vs-plan conflation** failure. Mature systems separate them: spec is a near-timeless invariant surface; roadmap/plan is an explicitly time-bound artifact expected to churn and retire.

### 0.2 — Why this is the root drift source

Prior cascades surfaced drift symptoms treated individually:
- Lesson #N18 candidate (pre-flight empirical scope verification) — estimates off 2.7× because doc-claimed scope diverged from code reality
- A'.9.1 Phase α 5 mid-cascade deltas (ProjectReference drop, CPM count, 17-rule count, repo URL, Commit 7 scope) — each a doc-vs-code divergence
- ANALYZER_RULES.md §4.6 self-audit — «Brief §1.3 «15-16» appears arithmetic carryover from pre-Q-L-9 drafting» — a roadmap-realized drift caught in-flight

Each was patched tactically. The **structural cause** beneath all of them: living-spec documents that also serve as roadmap accumulate divergence faster than any single-cascade audit can catch, because the document's own structure does not distinguish «this is current truth» from «this is future intention».

**This reconnaissance addresses the root, not the symptoms.**

### 0.3 — К-L14 thesis alignment

К-L14 thesis (per K_CLOSURE §2.20): «substrate minimal; falsifiability tracked through **defect rate, architectural integrity, pipeline economics**».

Documentation dual-load drift **IS architectural-integrity decay** — one of the three К-L14 falsifiability metrics. This reconnaissance directly serves К-L14:
- Measures architectural integrity (doc-vs-code divergence magnitude)
- Produces a remediation plan that restores integrity (spec/roadmap separation)
- Touches zero substrate (read-only analysis + one report artifact)

This is not a digression from the К-L14 research framework — it is **К-L14 falsifiability instrumentation applied to the documentation layer itself**.

### 0.4 — Living-spec vs Historical-snapshot (critical analytical distinction)

Not all document types are dual-load susceptible. The scan covers ALL document types (per Crystalka ratification 2026-05-25), but classification precedes deep analysis:

| Class | Definition | Drift contract | Analysis depth |
|---|---|---|---|
| **LIVING-SPEC** | Lifecycle Live / LOCKED / AUTHORED-SKELETON documents meant to track current invariants/state (KERNEL, K_CLOSURE, ANALYZER_RULES, FRAMEWORK, SYNTHESIS_RATIONALE, PROJECT_AXIOMS, METHODOLOGY, LEDGER, EVIDENCE_DASHBOARD) | Must stay synchronized with code; dual-load = drift | DEEP dual-load analysis |
| **HISTORICAL-SNAPSHOT** | Point-in-time records (briefs, closure reports, reconnaissance reports, session logs, amendments logs) | Expected to be «stale»; they record a moment, не track current state | QUICK confirmation pass (verify snapshot nature; flag misclassification only) |
| **MISCLASSIFIED** | Document whose actual usage diverges from its lifecycle/role declaration (a «brief» used as living spec; a living doc treated as frozen snapshot) | Role mismatch IS a drift category | FLAG for reclassification |

**Crucial implication**: Historical snapshots being «out of date» is NOT drift — it is their nature. The drift problem is specifically living-spec documents serving dual roles. The reconnaissance must not generate false alarms on historical artifacts (a recon report from A'.9.0 SHOULD describe A'.9.0-era state, not current state).

### 0.5 — Read-only discipline (S-LOCK equivalent)

This reconnaissance touches **zero production code, zero substrate, zero existing document content**. The ONLY write is the report artifact placed in `docs/reports/`.

- No К-L invariant edits (KERNEL Part 0 untouched)
- No existing doc edits (the refactor that follows is a SEPARATE future cascade determined FROM this report)
- No production code touched
- One new file: the report (+ optional REGISTER enrollment of the report — pending Q-DD-7 ratification §11)

The refactor execution is explicitly **out of scope** — this session produces the *plan*; Crystalka determines refactor work-volume FROM the report.

---

## §1 — Scope

### 1.1 — Document scope (ALL types — per Crystalka ratification 2026-05-25)

Per Crystalka correction (verbatim): «Да на все типы документов я тут ошибся в пункте».

**Full repository document scope** — every document enrolled in REGISTER.yaml (267 documents per register_version 2.10 state) is in-scope for the classification pass. Deep dual-load analysis applies to the LIVING-SPEC subset surfaced by classification.

Navigation authority: **REGISTER.yaml is the document inventory index** (per Crystalka: «вся информация для навигации есть в регистре документов в репо»). The orchestrator reads REGISTER.yaml first, builds the full document inventory by category × tier × lifecycle, then decomposes.

**Category inventory** (empirical — orchestrator confirms from REGISTER at R0):
- Category A (architecture + governance trio) — ~37 documents
- Category B (methodology) — METHODOLOGY + related
- Category D (working docs / briefs) — ~77 documents
- Category G (register itself) — REGISTER.yaml
- Other categories (C/E/F if present) — orchestrator confirms from REGISTER

**Lesson #N18 application**: orchestrator MUST empirically count documents per category from REGISTER before sub-agent allocation. Do NOT estimate — read REGISTER, count, allocate.

### 1.2 — Code-truth scope (ground-truth baseline)

To detect descriptive-stale and roadmap-realized drift, agents must compare doc claims against **what the code actually does**. Code-truth grounding scope:
- **Project inventory** — all csproj under `src/`, `native/`, `tools/`, `tests/`, `mods/`
- **Key type inventory** — public types/interfaces docs reference (К-L anchor types)
- **Phase completion ledger** — what has actually shipped per `.git/logs/HEAD` + KERNEL chronicle + K_EXTENSIONS_LEDGER + REGISTER audit_trail
- **Native vs managed surface** — which К-L invariants are enforced where

Code-truth is read-only reference; agents do not modify code.

### 1.3 — Out of scope

- **Refactor execution** — this session produces the plan; refactor is a separate future cascade
- **Existing document edits** — no living-spec document is edited in this session
- **Substrate analysis** — K-L invariant correctness is NOT questioned; only doc-vs-code synchronization + dual-load structure
- **A'.9.1 Phase β blockage** — this reconnaissance runs independently
- **New analyzer rules** — orthogonal to A'.9.1 analyzer cascade

### 1.4 — Relationship к A'.9.1 (in-flight cascade)

A'.9.1 Analyzer Infrastructure cascade is mid-execution. This drift reconnaissance is read-only, can run in parallel OR before A'.9.1 Phase β, and surfaces a sequencing question (Q-DD-8): ANALYZER_RULES.md is itself a prime dual-load specimen.

---

## §2 — Mission

### 2.1 — Orchestrator mission (Opus)
R0 navigation + code-truth baseline → R1 dispatch category sub-agents → R2 aggregate → R3 synthesize refactor plan → R4 author + place report. STOP.

### 2.2 — Sub-agent mission (per category cluster)
Read assigned set → classify (LIVING-SPEC / HISTORICAL-SNAPSHOT / MISCLASSIFIED) → section-by-section dual-load analysis (living-spec) → cross-reference code-truth → return structured inventory. Read-only.

### 2.3 — Report mission (R4 output)
The report must enable Crystalka к determine refactor work-volume (executive summary, methodology, code-truth manifest, per-category inventory, systemic patterns, classification summary, target architecture proposal, refactor plan, work-volume estimate, appendices).

### 2.4 — Sequencing relative к A'.9.1
Parallel reconnaissance recommended (read-only, blocks nothing); Crystalka decides refactor-vs-A'.9.1-Phase-β ordering FROM the report.

---

## §3 — Multi-agent architecture

Decomposition по document category (Crystalka ratification). Orchestrator reads REGISTER at R0, counts per category, allocates sub-agents (~5-12 docs per sub-agent for deep analysis). Proposed clusters: C1 core architecture, C2 analyzer + rules, C3 ledger + evidence, C4 governance trio, C5 methodology, C6 architecture working docs, C7 briefs + closure reports, C8 remaining categories. Orchestrator MAY split clusters empirically (Lesson #N18).

Code-truth grounding (R0): Option (c) hybrid recommended — orchestrator establishes project inventory + phase ledger + key-type locations; sub-agents do targeted code-reads for specific claims.

Aggregation protocol (R2): merge → deduplicate cross-references → cluster systemic patterns → quantify → rank.

Special handling §3.5 — append-only temporal documents (K_EXTENSIONS_LEDGER, K_L14_EVIDENCE_DASHBOARD): past entries = snapshot (exempt); forward-looking sections = roadmap-load (in-scope).

---

## §4 — Drift taxonomy (analytical core)

| Type | Code | Health | Temporal nature | Refactor action |
|---|---|---|---|---|
| SPEC-CURRENT | A | ✓ healthy | present, accurate | keep in spec |
| SPEC-STALE | type A | ✗ drift | past-as-present | update к code-truth |
| ROADMAP-PENDING | type B | ~ misplaced | future, accurate | extract к roadmap layer |
| ROADMAP-REALIZED | type C | ✗ drift | future-as-unrealized (but realized) | move к spec OR retire |
| MIXED-INTERWOVEN | type D | ✗ drift (root) | entangled | structural separation |
| ORPHAN-REFERENCE | type E | ✗ drift | dangling | resolve/remove |

---

## §5 — Phase progression (R0–R4)

R0 navigation + code-truth baseline → R1 sub-agent dispatch (parallel waves) → R2 aggregation → R3 refactor plan synthesis → R4 report authoring + placement. Filesystem write discipline: verify report exists at `docs/reports/` path before declaring R4 complete.

---

## §6 — Report structure

§6.0 executive summary · §6.1 methodology · §6.2 code-truth manifest · §6.3 per-category drift inventory · §6.4 systemic drift pattern analysis · §6.5 drift classification summary · §6.6 target architecture proposal · §6.7 spec/roadmap separation option space (α/β/γ/δ/ε) · §6.8 refactor plan · §6.9 REGISTER cascade outline · §6.10 work-volume estimate · §6.11 appendices.

§6.7 option space: α single ROADMAP.md · β per-domain roadmap docs · γ fenced roadmap section convention · δ roadmap-in-REGISTER · ε hybrid (δ+γ). Report recommends per PA-001/PA-002/PA-003.

---

## §7 — Sub-agent output schema

Per-document YAML-ish inventory: register_id, path, category, tier, lifecycle, classification, classification_evidence, analysis_depth, drift_items (section / drift_type / claim / code_truth / action / evidence), summary (drift_counts, dual_load_density, highest_value_finding). Cluster-level rollup appended.

---

## §8 — Verification requirements

R0: REGISTER fully inventoried; code-truth manifest complete; cluster decomposition covers 100%. R1: every cluster returns inventory; every doc classified; every living-spec has section-level items; every SPEC-STALE/ROADMAP-REALIZED cited. R2: master inventory = union; deduplication complete; counts reconcile. R3: target architecture recommendation with rationale; per-document migration; new-doc proposals; REGISTER cascade outline; work-volume estimate. R4: report exists + covers all §6 + appendices populated. §8.6 К-L14 preservation: zero K-L invariant change, zero existing doc edits, zero production code.

---

## §9 — Halt conditions

1. REGISTER inventory mismatch (≠267). 2. Code-truth ambiguity blocking classification. 3. Misclassification cluster surfaces structural governance issue. 4. Sub-agent inconsistent schema. 5. Report scope explosion (1000+ items). 6. Refactor touches LOCKED Tier 1 substrate invariant text (S-LOCK-1). 7. A'.9.1 interaction conflict (Q-DD-8).

---

## §10 — Cross-references

Navigation authority: `docs/governance/REGISTER.yaml`. Code-truth surfaces: `src/`, `native/`, `tools/`, `tests/`, `mods/` + git logs. Prime dual-load specimens (a priori): ANALYZER_RULES.md, KERNEL_ARCHITECTURE.md, PHASE_A_PRIME_SEQUENCING.md. Methodology grounding: METHODOLOGY.md (Lesson #N14 + #N18). Governance protocol: FRAMEWORK.md, SYNTHESIS_RATIONALE.md, PROJECT_AXIOMS.md. In-flight cascade: A'.9.1 Analyzer Infrastructure.

---

## §11 — Ratification surface (Q-DD decisions)

| Q-DD | Decision | Default |
|---|---|---|
| Q-DD-1 | Cascade formal ID | standalone «Documentation Dual-Load Drift Reconnaissance» |
| Q-DD-2 | Multi-agent decomposition | orchestrator finalizes cluster count from empirical REGISTER counts |
| Q-DD-3 | Code-truth grounding | (c) hybrid |
| Q-DD-4 | Snapshot exemption | briefs/closure/recon/logs HISTORICAL-SNAPSHOT, exempt |
| Q-DD-5 | Append-only ledger | §3.5 special handling |
| Q-DD-6 | Report depth | single comprehensive report |
| Q-DD-7 | Report REGISTER enrollment | enroll |
| Q-DD-8 | Sequencing vs A'.9.1 Phase β | parallel reconnaissance |
| Q-DD-9 | Target architecture pre-lean | report recommends freely |
| Q-DD-10 | docs/reports/ folder | create if absent |

---

## §12 — Authoring metadata

**Brief authored**: 2026-05-25 by Claude Opus 4.7 (deliberation session)
**Trigger**: Crystalka root-cause drift identification 2026-05-25 («архитектурные документы выполняют ещё роль дорожной карты»)
**К-L14 thesis preservation**: read-only reconnaissance; zero substrate touch; one report artifact.

### 12.1 — Ratification checklist (Crystalka)
1. §0 problem diagnosis (dual-load) matches root-cause. 2. §0.4 living-spec vs historical-snapshot distinction sound. 3. §1 scope = all document types. 4. §3 decomposition by-category. 5. §4 drift taxonomy (6 types). 6. §6 report structure produces full refactor plan. 7. §6.7 option space. 8. §11 Q-DD surface. 9. §8.6 + §9 halt-6 К-L14 / S-LOCK-1 preservation. 10. Sequencing vs A'.9.1 (Q-DD-8) parallel default.

---

## §closure — Execution footnote (2026-06-02)

**EXECUTED** by Claude Code (Opus) on branch `claude/doc-drift-reconnaissance-503aH`, tip `f94bb84` at R0.

- **R0**: REGISTER inventoried — **267 documents confirmed** (A=37, B=6, C=3, D=77, E=62, F=72, G=8, H=2); no §9 halt-1 mismatch. Code-truth manifest established (12 src projects, native kernel, 17 analyzer rule stubs, 21 К-L invariants, key-type locations verified).
- **R1**: 10 read-only sub-agents dispatched по category cluster (8 deep + 2 quick-pass), two waves. All returned structured §7-schema inventories.
- **R2/R3**: ~105 living-spec drift line-items aggregated; 139-doc snapshot population confirmed clean (0 hard misclassifications); 7-doc misclassification cluster surfaced (§9 halt-3 meta-finding). Five systemic generators identified. Recommended target architecture: **Option ε (δ+γ)** anchored on existing `docs/ROADMAP.md`, decomposed into 3 refactor cascades (DD-1 spec-truth / DD-2 structural separation / DD-3 hygiene).
- **R4**: report placed at `docs/reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md` (`DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT`).
- **Q-DD dispositions**: Q-DD-1 standalone · Q-DD-3 hybrid · Q-DD-6 single report · Q-DD-7 ENROLLED (brief + report) · Q-DD-9 Option ε · Q-DD-10 existing folder. Halt-6 preserved (zero К-L invariant text edits required).
- **К-L impact**: zero. Read-only discipline held; report + this brief are the only artifacts.

**Forward**: Crystalka determines refactor work-volume FROM the report → ratifies Q-DD-8 sequencing → (future) DD-1/DD-2/DD-3 refactor cascades separate spec from roadmap.

---

**End of Brief — Documentation Dual-Load Drift Reconnaissance (EXECUTED)**
