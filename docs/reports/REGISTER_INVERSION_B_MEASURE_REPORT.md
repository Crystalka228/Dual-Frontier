---
register_id: DOC-E-REGISTER_INVERSION_B_MEASURE_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-07-17
last_modified: 2026-07-17
content_language: en
next_review_due: null
title: REGISTER_INVERSION Cascade B — Measure Report (CD1; supersedes the 288-era counts)
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: Authored at CD1 with hand-authored schema-2.0 frontmatter (the 2.0 enrollment discipline); this pre-migration SoT entry mirrors that frontmatter so the migrator carries the file through the live flip (the control dry-run showed frontmatter-only enrollees would be auto-excluded by the migrator's orphan branch).
---

# REGISTER_INVERSION Cascade B — Measure Report

*The post-ratification re-measure (CD1 of CORPUS_CLOSURE_INVERSION_B Phase D). Supersedes
every stale count in `REGISTER_INVERSION_A_MEASURE_REPORT.md` (288-era). Basis: dry-run
`migrate --target <scratch>` of the post-merge corpus at `main` = `959d7ea` (register 2.32,
Phase C ratification merged). This report is the align/arm work order executed by CD2–CD4.*

- Instrument: `tools/DualFrontier.Governance` (net10.0), governance suite 64/64 green.
- Enrolled per schema-2.0 discipline: frontmatter in this file is hand-authored SoT.

## 1. Round-trip reconciliation (re-proof at 321)

| Property | Cascade A (288-era) | **Cascade B (this measure)** |
|---|---|---|
| documents migrated (.md frontmatter injected) | 285 | **318** |
| non-.md carried to provisional supplement | 2 | **2** (unchanged: the Launcher `.cs` pair) |
| DOC-G-REGISTER deterministic self-entry | 1 | **1** |
| **total derived documents** | 288 | **321** (= the live register's 321) |
| globals extracted REQ/RISK/CAPA/EVT | 41/14/17/47 | **41/14/17/55** |
| backfills `project` / `first_authored` | 285/276 | **318/276** |
| orphans auto-excluded on scratch | 27 | **25** (the known triage set) |
| reconciliation deltas beyond ratified drops | 0 | **0** |
| second sync byte-identical (idempotency) | true | **true** |
| sync exit on migrated corpus | 0 | **0** |
| dry-run verdict | CLEAN | **CLEAN** (exit 0) |

Ratified drops re-confirmed in effect: `register_view_url` (always), `last_modified_commit`
where `PENDING-*` (F-2 dissolution mechanics — the migrator skips the placeholder class);
`id` → `register_id`; `path` re-derived from file location; bare YAML null ≡ `'null'` sentinel.

## 2. Semantic gate findings on the migrated corpus

**Exactly the seven known findings — ZERO new classes. H-D1 not triggered.** Armed rehearsal
on the unaligned scratch: the same 7 became exit-affecting (nonzero exit) — armed mode proven
to bite pre-align.

| # | Gate | Document | Finding | CD2 action (ratified vocabulary) |
|---|---|---|---|---|
| 1 | G-TERMINAL | DOC-A-GODOT_INTEGRATION | SUPERSEDED with `next_review_due: 2027-05-12` | align → `'null'` |
| 2 | G-TERMINAL | DOC-A-VISUAL_ENGINE | SUPERSEDED with `next_review_due: 2027-05-12` | align → `'null'` |
| 3 | G-TERMINAL | DOC-A-A_PRIME_9_RECONNAISSANCE_REPORT | EXECUTED with `'post-A'.9.1 closure'` | **ruling (c)**: align → `'null'` (EXECUTED stays terminal) |
| 4 | G-SENTINEL | DOC-E-DOC_DRIFT_REFACTOR_PROGRESS | `'on-refactor-cascade-execution'` unsanctioned | align → `'post-doc-drift-refactor-cascade closure'` |
| 5 | G-SENTINEL | DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT | `'on-refactor-cascade-execution'` unsanctioned | align → `'post-doc-drift-refactor-cascade closure'` |
| 6 | G-SENTINEL | DOC-F-SRC-LAUNCHER | `'TBD — after Vanilla mods …'` unsanctioned | **ruling (d)**: align → `'post-Vanilla-mods-consumer-materialization closure'` |
| 7 | G-XREF | DOC-D-K8_2_V2 → DOC-D-K8_2_V1_DEPRECATED | target lacks resolving pointer | align: `DOC-D-K8_2_V1_DEPRECATED` gains `superseded_by: DOC-D-K8_2_V2` |

## 3. G-RATIO monitor — the FRAMEWORK §10 #5 instrument, second reading

> **G-RATIO: 132/318 = 41.5%** (monitor basis: parsed .md docs; register basis 132/321 = 41.1%).
> Cascade A read 33.0%. The threshold is 20%. **The per-rule breakdown (D5,
> `G_RATIO_PER_RULE_BREAKDOWN.md`) shows the headline number is NOT matrix pressure:**
> only **22 of the 132** rationales exempt a forbidden combination (12× D+4, 2× E+2,
> 8× tier-1+AUTHORED); **110 are populated on LEGAL combos** — the field doubles as a
> provenance/lineage note. Ruling (e) stays DEFERRED to its architect deliberation; D5 feeds it.

## 4. Orphan triage — execution plan (the ratified R4.2 verdicts, applied at CD2)

The 25 orphans auto-excluded on the scratch decompose per the ratified triage:

| Verdict | Count | CD2 action |
|---|---|---|
| enroll F/4 | 12 | the 11 `src/DualFrontier.Runtime/**/MODULE.md` + `tests/DualFrontier.Runtime.Tests/Vulkan/MODULE.md` (register entries at CD2; the migrator injects their frontmatter at CD3) |
| enroll per **ruling (b)** | 5 | 4 `tools/briefs/*` (BRIEF_SKELETON_FRAMEWORK, K10_3_EXECUTION, K10_AMENDMENTS_APPLICATION, K8_5_DEFERRAL_CASCADE) → **D/3/EXECUTED**; `docs/prompts/PHASE_BETA_PREP_EXECUTION_PROMPT.md` → **E/3/EXECUTED** |
| exclude | 8 | `SCOPE_EXCLUSIONS.yaml` entries with real rationales: 2 `tools/DualFrontier.Analyzers/AnalyzerReleases.*.md` (analyzer release-tracking ceremony files, tool-owned) + 6 `docs/scratch/**` (HALT reports ×4, manual-verification protocols ×2 — working notes) |

Pre-empting the orphan set BEFORE the live migrate keeps the migrator's auto-exclusion
branch at zero (no "Cascade A dry-run" rationale stamping on the live exclusion config —
the round-trip-loses-nothing directive, H-D2 clean).

## 5. Non-.md enrolled artifacts — ruling (a) applied at CD3/CD4

The two Launcher `.cs` entries (`DOC-F-SRC-LAUNCHER-PROCEDURAL-ATLAS`, `DOC-F-SRC-LAUNCHER-SCENE-STATE`)
carry into `REGISTER_SUPPLEMENT.yaml`, which **ruling (a)** formalizes as the STANDING
non-.md SoT file (header text updated from PROVISIONAL to standing at the arm commit).

## 6. CD2–CD4 work order (sized)

| Step | Content | Volume |
|---|---|---|
| CD2 align | 7 gate-finding value fixes (register SoT, last pre-migration edits) + 18 orphan enrollments (17 entries + 1 already-covered README check) + 8 exclusions | ~26 touches |
| CD3 migrate live | `migrate --target <repo> --i-understand-this-mutates-the-corpus` (typed once) + sync → derived archive + authority surface | 1 flagged run |
| CD4 arm + retire | `--armed` validate exit 0; failability suite; delete `sync_register.ps1` + `render_register.ps1`; REGISTER_RENDER.md → historical; supplement header formalized; meta-roles; FRAMEWORK §14.9 completion note (pre-authorized PATCH); F-34 + F-2 closed | 1 commit set |

**End of REGISTER_INVERSION_B_MEASURE_REPORT.md — 2026-07-17. CD1 of Phase D; the work order for CD2–CD4.**