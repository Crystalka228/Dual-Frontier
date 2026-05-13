# DualFrontier Document Control Register — Rendered View

*Auto-generated from [REGISTER.yaml](./REGISTER.yaml) by `tools/governance/render_register.ps1`. Do not edit — edit REGISTER.yaml instead.*

*Last generated: 2026-05-12 (initial manual baseline; subsequent generations via tooling)  |  Schema version: 1.0  |  Register version: 1.0*

---

## Statistics

- Total documents: ~195 enrolled at A'.4.5 closure (Tier 1 ~32 + Tier 2 ~15 + Tier 3 ~75 + Tier 4 ~73 + Tier 5 ~0)
- Per category: A ≈ 30  |  B = 6  |  C = 3  |  D ≈ 40  |  E ≈ 30  |  F ≈ 73  |  G = 6  |  H = 2  |  I = 0  |  J = 0
- Open CAPA: 1 (CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT — closes at A'.5 K8.3)
- Active risks: 11 (1 CLOSED + 11 ACTIVE/RESIDUAL + 1 ACCEPTED + 1 long-horizon)
- Stale documents: 0 (initial state; STALE state surfaces via validation per Q-A45-X5)

---

## Table of contents

- [Category A — Architecture](#category-a-architecture) (~30 documents)
- [Category B — Methodology](#category-b-methodology) (6)
- [Category C — Live tracker](#category-c-live-tracker) (3)
- [Category D — Brief](#category-d-brief) (~40)
- [Category E — Closure / audit / discovery](#category-e-closure-audit-discovery) (~30)
- [Category F — Module-local](#category-f-module-local) (~73)
- [Category G — Project meta](#category-g-project-meta) (6)
- [Category H — i18n](#category-h-i18n) (2)
- Category I — Ideas Bank (0 at A'.4.5; folder seeded, future authoring)
- Category J — Game Mechanics (0 at A'.4.5; folder seeded, future authoring)
- [Global: Requirements](#global-requirements)
- [Global: Risks](#global-risks)
- [Global: CAPA log](#global-capa-log)
- [Global: Audit trail](#global-audit-trail)

---

## Bootstrap notice

This manual baseline `REGISTER_RENDER.md` was authored at A'.4.5 closure 2026-05-12 because the `render_register.ps1` tooling depends on the `powershell-yaml` module not yet installed. Once `Install-Module powershell-yaml -Scope CurrentUser -Force` is run, this file is regenerated automatically by `render_register.ps1` to produce the structured per-entry detail view. The summary statistics above are accurate as of register state at A'.4.5 closure.

---

<a name="category-a-architecture"></a>
## Category A — Architecture

LOCKED Tier 1 (most): KERNEL_ARCHITECTURE v1.5, MOD_OS_ARCHITECTURE v1.7, RUNTIME_ARCHITECTURE v1.0, MIGRATION_PLAN_KERNEL_TO_VANILLA v1.1, GPU_COMPUTE v2.0, ARCHITECTURE v0.3, CONTRACTS, ISOLATION, THREADING, ECS, EVENT_BUS, MODDING, MOD_PIPELINE v0.2, GODOT_INTEGRATION, VISUAL_ENGINE, PERFORMANCE, OWNERSHIP_TRANSITION, FHE_INTEGRATION_CONTRACT v1.0, COMBO_RESOLUTION v0.2, COMPOSITE_REQUESTS, RESOURCE_MODELS, FEEDBACK_LOOPS v0.2.

Live Tier 1: FIELDS v0.1 (Live; populated by K9 closure).

Draft Tier 1: ARCHITECTURE_TYPE_SYSTEM v0.1, MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION v0.1.

Tier 2 Live (special-case override): PHASE_A_PRIME_SEQUENCING.

Tier 3 EXECUTED (special-case override): K_L3_1_AMENDMENT_PLAN, A_PRIME_0_7_AMENDMENT_PLAN.

Tier 1 Governance meta-entries: FRAMEWORK v1.0, SYNTHESIS_RATIONALE v1.0.

<a name="category-b-methodology"></a>
## Category B — Methodology

All Tier 1 LOCKED: METHODOLOGY v1.6, CODING_STANDARDS v1.0, DEVELOPMENT_HYGIENE v1.0, MAXIMUM_ENGINEERING_REFACTOR v1.1, PIPELINE_METRICS v0.2, TESTING_STRATEGY v1.0.

<a name="category-c-live-tracker"></a>
## Category C — Live tracker

All Tier 2 Live: MIGRATION_PROGRESS (mixed RU/EN), ROADMAP, IDEAS_RESERVOIR (post-A'.4.5 as index for docs/ideas/ Category I bank).

<a name="category-d-brief"></a>
## Category D — Brief

All Tier 3. Status distribution:

EXECUTED K-series: K0..K8.2 v2, K9, K_L3_1 + addendum, K_LESSONS_BATCH, K8 decision, K6 verification log, K9 brief refresh patch, K8.1.1, K8.0.

AUTHORED K-series: K8.3, K8.4, K8.5.

DEPRECATED: K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED (deprecated_by: K8_2_V2).

SUPERSEDED: K4_STRUCT_REFACTOR (superseded_by: K4).

EXECUTED A' deliberation: A'.0.5, A'.0.7 (methodology restructure + closure execution), A'.1 amendment execution, A'.4.5 (deliberation brief + Pass 1-5), MOD_OS v1.6 amendment.

AUTHORED G-series: G0..G9 (all 10 briefs; future Phase A'.6+ execution).

<a name="category-e-closure-audit-discovery"></a>
## Category E — Closure / audit / discovery

All Tier 3 EXECUTED:

Audit pass artifacts: AUDIT_CAMPAIGN_PLAN, AUDIT_PASS_1..5 (prompts + inventory + spec_code + resumption + triage + cross-doc), AUDIT_REPORT, PASS_2-4 notes/report, M3-M7_CLOSURE_REVIEW, SESSION_PHASE_4_CLOSURE_REVIEW, UI_REVIEW_PRE_M75B2.

Reports: PERFORMANCE_REPORT_K3 (K3 closed; Tier 3 EXECUTED).

Benchmarks: k7-bdn-tick-report.

Prompts: 14 housekeeping + M7X/M75X/TD1 prompts.

Learning: PHASE_1 (Russian content).

Native build notes: native/.../build.md (special_case: closure-style in source tree).

MOD_OS v1.6 amendment closure (special_case: closure in briefs/).

Scratch: tools/scratch/A_05/ — INVENTORY, BASELINE, DISCOVERY_SUMMARY, REORG_PLAN_APPROVED, STALENESS_REPORT, PIPELINE_TERMINOLOGY, A_PRIME_4_5_DELIBERATION_CLOSURE.

Tier 2 Live (special_case override): PERFORMANCE_REPORT_K7, NORMALIZATION_REPORT, CPP_KERNEL_BRANCH_REPORT, NATIVE_CORE_EXPERIMENT (live closure reports — K-series still open per Pass 2 §2.2).

<a name="category-f-module-local"></a>
## Category F — Module-local

All Tier 4 Live. Aggregated by source tree:

`src/` (54 entries): per-directory READMEs across AI, Application (Bridge/Commands/Input/Loop/Modding/Rendering/Save/Scenario/Scene), Components (Building/Combat/Items/Magic/Pawn/Shared/World), Contracts (Attributes/Bus/Core/Modding), Core (Bus/ECS/Math/Registry/Scheduling), Core.Interop/MODULE.md, Events (Combat/Inventory/Magic/Pawn/Power/World), Persistence, Presentation (Input/Nodes/Scenes/UI/addons/df_devkit), Presentation.Native, Systems (Combat/Faction/Inventory/Magic/Internal/Pawn/Power/World).

`tests/` (5 entries): tests/, Core.Tests, Core.Interop.Tests/MODULE.md, Modding.Tests, Systems.Tests.

`mods/` (8 entries): mods/, Mod.Example, Mod.Vanilla.{Combat,Core,Inventory,Magic,Pawn,World}.

`native/DualFrontier.Core.Native/` (4 entries): MODULE.md at root, include/, src/, test/.

`tools/` (2 entries): tools/briefs/MODULE.md, tools/governance/MODULE.md (NEW at A'.4.5).

`assets/scenes/README.md` (1 entry).

<a name="category-g-project-meta"></a>
## Category G — Project meta

All Tier 2 Live:

- DOC-G-README (`README.md`) — root project README
- DOC-G-DOCS_README (`docs/README.md`) — docs index
- DOC-G-IDEAS_README (`docs/ideas/README.md`) — Ideas Bank index (NEW)
- DOC-G-MECHANICS_README (`docs/mechanics/README.md`) — Game Mechanics index (NEW)
- DOC-G-REGISTER (`docs/governance/REGISTER.yaml`) — meta_entry, role=register_source_of_truth, cadence=on-every-milestone-closure
- DOC-G-BYPASS_LOG (`docs/governance/BYPASS_LOG.md`) — Tier 2 Live tracker

Note: DOC-G-REGISTER_RENDER (this file) and DOC-G-VALIDATION_REPORT entries to be added on first sync_register run when tooling becomes operational.

<a name="category-h-i18n"></a>
## Category H — i18n

Both Tier 2 Live (mixed content):

- TRANSLATION_GLOSSARY v1.0 (content LOCKED via version pin; lifecycle Live per H matrix)
- TRANSLATION_PLAN

---

<a name="global-requirements"></a>
## Global: Requirements

| ID | Title | Status | Source | Milestone |
|---|---|---|---|---|
| REQ-K-L1 | Native language: C++20 | VERIFIED | DOC-A-KERNEL | K0 closure |
| REQ-K-L2 | Bindings: Pure P/Invoke | VERIFIED | DOC-A-KERNEL | K0 closure |
| REQ-K-L3 | Component storage Path α default + Path β opt-in | PARTIAL | DOC-A-KERNEL | K-L3.1 bridge formalization |
| REQ-K-L4 | Explicit component registry (no reflection) | VERIFIED | DOC-A-KERNEL | K4 closure |
| REQ-K-L5 | Span protocol (zero-copy enumeration) | VERIFIED | DOC-A-KERNEL | K5 closure |
| REQ-K-L6 | Mod rebuild capability | VERIFIED | DOC-A-KERNEL | K6 closure |
| REQ-K-L7 | Read-only spans + write batching | VERIFIED | DOC-A-KERNEL | K5 closure |
| REQ-K-L8 | Native scheduler (parallel system execution) | VERIFIED | DOC-A-KERNEL | K3 closure |
| REQ-K-L9 | Performance threshold (V3 dominates V2 by 4-32×) | VERIFIED | DOC-A-KERNEL | K7 closure |
| REQ-K-L10 | Native organicity Lvl 1 | VERIFIED | DOC-A-KERNEL | K0 closure (D3 codification) |
| REQ-K-L11 | Single NativeWorld backbone | VERIFIED | DOC-A-KERNEL | K8.2 v2 closure |
| REQ-Q-A07-6 | Audience contract: methodology agent-as-primary-reader | VERIFIED | DOC-B-METHODOLOGY | A'.0.7 closure + A'.4.5 governance inheritance |
| REQ-Q-A45-X5 | Post-session update protocol mandatory | PENDING | DOC-A-FRAMEWORK | A'.4.5 closure |

<a name="global-risks"></a>
## Global: Risks

| ID | Title | Status | Type | Likelihood | Impact |
|---|---|---|---|---|---|
| RISK-001 | Component struct refactor scope underestimated | CLOSED | Architectural | Medium-High | Medium |
| RISK-002 | P/Invoke marshalling correctness | RESIDUAL | Technical | Medium | High |
| RISK-003 | Native↔Managed ownership boundary leaks | ACTIVE | Technical | Medium | Critical |
| RISK-004 | Cross-document drift between LOCKED specs | ACTIVE | Architectural | Medium-High | High |
| RISK-005 | Mod ecosystem compatibility breakage | ACTIVE | Architectural | Medium | High |
| RISK-006 | Path α/Path β bridge complexity exceeds mod author mental model | ACTIVE | Architectural | Medium | Medium |
| RISK-007 | Brief staleness density | ACTIVE | Methodological | High | Medium |
| RISK-008 | Amendment plan completeness gap | ACTIVE | Methodological | Medium | Medium |
| RISK-009 | Lesson-applier latency | ACTIVE | Methodological | Medium | Low |
| RISK-010 | Register itself degrades into stale artifact | ACTIVE | Methodological | Medium | Critical |
| RISK-011 | Environmental incidents (testhost lock, verbosity gotcha, tooling drift) | ACTIVE | Operational | Medium-High | Low |
| RISK-012 | Cross-platform tooling debt (PowerShell-only) | ACCEPTED | Operational | Medium | Low |
| RISK-013 | Vulkan driver compatibility on weak hardware | ACTIVE | External | Medium | High |
| RISK-014 | Long-horizon pipeline degradation | ACTIVE | Methodological | Medium | Critical |

<a name="global-capa-log"></a>
## Global: CAPA log

| ID | Opened | Status | Trigger summary |
|---|---|---|---|
| CAPA-2026-05-09-K8.2-V2-REFRAMING | 2026-05-09 | CLOSED | K-L3 «без exception» framing misalignment at K8.2 v2 verification |
| CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION | 2026-05-10 | CLOSED | Methodology corpus v1.x human-as-primary-reader assumption inverted to agent-primary |
| CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT | 2026-05-12 | OPEN | INVENTORY.md baseline ~135 vs actual ~220 at A'.4.5 pre-flight; closes at A'.5 K8.3 |

<a name="global-audit-trail"></a>
## Global: Audit trail

| Date | Event | Type | Commits |
|---|---|---|---|
| 2026-05-10 | K-L3.1 bridge formalization deliberation | deliberation_milestone | pre-2df5921 |
| 2026-05-10 | K-L3.1 amendment plan execution (A'.1.K) | amendment_landing | 2df5921..0789bd4 |
| 2026-05-10 | A'.0.5 documentation reorganization | execution_milestone | 27523ac..4e332bb |
| 2026-05-10 | A'.0.7 methodology pipeline restructure deliberation | deliberation_milestone | pre-86b721a |
| 2026-05-10 | A'.0.7 methodology rewrite landing (A'.1.M) | amendment_landing | 86b721a..9d4da64 |
| 2026-05-10 | A'.3 push to origin | governance_event | through 38c2e19 |
| 2026-05-11 | A'.4 K9 field storage execution | execution_milestone | ce4dba8..80c9ba6 |
| 2026-05-12 | A'.4.5 document control register deliberation | deliberation_milestone | 7448267..411c284 |
| 2026-05-12 | A'.4.5 register execution closure | execution_milestone | TBD (this milestone) |

---

## See also

- [FRAMEWORK.md](./FRAMEWORK.md) §4 register sections specification
- [SYNTHESIS_RATIONALE.md](./SYNTHESIS_RATIONALE.md) §1-§3 source-standard provenance
- [REGISTER.yaml](./REGISTER.yaml) operational SoT (this view is auto-generated derivative)
- [BYPASS_LOG.md](./BYPASS_LOG.md) validation bypass tracking
- [tools/governance/MODULE.md](../../tools/governance/MODULE.md) — PowerShell tooling module index
