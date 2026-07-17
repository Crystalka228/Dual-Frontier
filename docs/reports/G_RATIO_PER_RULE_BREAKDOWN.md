---
register_id: DOC-E-G_RATIO_PER_RULE_BREAKDOWN
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
first_authored: "2026-07-17"
last_modified: "2026-07-17"
content_language: en
next_review_due: "null"
reviewer: Crystalka
---
# G-RATIO Per-Rule Breakdown (D5)

*Deliverable D5 of CORPUS_CLOSURE_INVERSION_B (ruling (e) input). Computed over the
migrated corpus at `main` = `959d7ea` / register 2.32 (321 register entries; the G-RATIO
monitor's basis is the 318 parsed .md documents). This report FEEDS the deferred
architect deliberation on the FRAMEWORK §3.4 allowed-combinations matrix and the §10 #5
falsification threshold — it decides nothing.*

## 1. Headline

| Metric | Value |
|---|---|
| Register entries | 321 |
| Entries carrying `special_case_rationale` | **132** |
| Ratio (register basis) | **41.1%** |
| Ratio (monitor basis, 318 parsed .md) | **41.5%** |
| §10 #5 threshold | 20% |
| Cascade A reading (288-era) | 33.0% |

## 2. The structural finding: exemption vs provenance-note

The §10 #5 falsification reads the ratio as "the matrix forbids combinations real workflows
need." The per-rule decomposition refutes that reading for this corpus:

| Class | Count | Share of 132 |
|---|---|---|
| **Genuine exemptions** (rationale on a FORBIDDEN combination) | **22** | 16.7% |
| **Decorative** (rationale on a LEGAL combination — provenance/lineage note) | **110** | 83.3% |

**The matrix is under almost no real pressure.** The override share crossed the threshold
because the corpus uses `special_case_rationale` as a general provenance field (supersession
lineage, ratification history, merge notes), not because forbidden combinations accumulate.

## 3. Per-rule exemption counts (a document may exempt two rules; none do today)

| Forbidden rule (G-CATLIFE) | Exempting docs | Members |
|---|---|---|
| category×tier **D+4** | **12** | A_PRIME_9_0_AMENDMENTS_LOG, A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF, A_PRIME_9_1_PHASE_0_CLOSURE_REPORT, A_PRIME_9_1_PHASE_DELTA_BRIEF, A_PRIME_9_1_PHASE_GAMMA_BRIEF, ARCHITECTURE_TRUTH_CASCADE_BRIEF, DOC_DRIFT_RECONNAISSANCE_BRIEF, GODOT_ERADICATION_BRIEF, GODOT_ERADICATION_BRIEF_PATCH, PHASE_BETA_BRIEF, PHASE_BETA_RECON_REPORT, STANDING_LAW_CASCADE_BRIEF |
| category×tier **E+2** | **2** | DOC_DRIFT_REFACTOR_PROGRESS, DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT |
| tier×lifecycle **1+AUTHORED** | **8** | K_CLOSURE_REPORT + the 7 cross-cutting drafts (EXECUTION_AUTHORITY_MATRIX, CONCURRENCY_AND_MEMORY_MODEL, RESOURCE_OWNERSHIP_AND_LIFETIME, ENGINE_LIFECYCLE_AND_TRANSACTIONS, TIME_AND_CONSISTENCY_MODEL, IDENTITY_AND_ABI_CONTRACT, PERSISTENCE_SNAPSHOT_CONTRACT) |
| every other forbidden rule (D→1/2/5, E→1/4/5, F→1/2/3/5, 1+SKELETON, 2+SKELETON, 3+LOCKED, 4+LOCKED, 4+SKELETON, 5+STALE, 5+SKELETON) | **0** | — |

Note: the Phase C ratification REDUCED genuine tier-1+AUTHORED pressure from 22 to 8 (the
14 successors flipped to the legal 1+LOCKED); the D+4 dozen is the brief/log family the
tier-4 module row was never meant to host — a matrix-row question, not an override-abuse
question.

## 4. Decorative-class composition (110 docs)

| Cohort | ≈Count | Nature of the note |
|---|---|---|
| A/1/SUPERSEDED historical predecessors | 24 | supersession lineage + authority-gap/closure record |
| A/1/LOCKED rework successors | 14 | ratification lineage (this cascade's own wording) |
| D/3/EXECUTED + D/3/SUPERSEDED + D/3/AUTHORED(-SKELETON) briefs | ~40 | D-brief convention note ("clean non-forbidden combo per precedent…") |
| E/3 reports (EXECUTED/Live) | 19 | enrollment/provenance notes |
| A/2 + A/3, B, C, G, H singles | ~13 | assorted lineage/meta notes |

The D-brief cohort is the purest irony: dozens of rationales EXPLAIN that their combination
is legal ("the clean (non-forbidden) Category-D combo…") — the field used to document
non-exemption.

## 5. Inputs to the ruling-(e) deliberation (decisions NOT taken here)

1. **Split the field**: a dedicated `provenance_note`/`lineage` field (or moving lineage into
   the audit trail) would drop the monitor to 22/318 ≈ **6.9%** — far under threshold — with
   zero information loss.
2. **Scope the monitor**: count only rationales on forbidden combinations (the gate already
   computes forbiddenness); the monitor then measures what §10 #5 actually falsifies.
3. **Matrix rows worth a look**: D+4 (12 real occupants — briefs/logs living at tier 4) and
   1+AUTHORED (a deliberate, precedent-backed workflow state for drafts awaiting ratification).
4. **Threshold**: if the field stays dual-use, 20% is measuring convention, not matrix error.

**End of G_RATIO_PER_RULE_BREAKDOWN.md (D5) — 2026-07-17.**
