# DualFrontier Document Control Register — Rendered View

*Auto-generated from [REGISTER.yaml](./REGISTER.yaml) by `tools/governance/render_register.ps1`. Do not edit — edit REGISTER.yaml instead.*

*Last generated: 2026-05-20  |  Schema version: 1.0  |  Register version: 2.0*

---

## Statistics

- Total documents: 253
- Tier 1: 33  |  Tier 2: 14  |  Tier 3: 130  |  Tier 4: 76  |  Tier 5: 0
- Per category: A=31  |  B=6  |  C=3  |  D=68  |  E=59  |  F=76  |  G=8  |  H=2  |  I=0  |  J=0
- Open CAPA: 0  |  Active risks: 12  |  Stale documents: 0

---

## Table of contents

- [Category A (31 documents)](#category-A)
- [Category B (6 documents)](#category-B)
- [Category C (3 documents)](#category-C)
- [Category D (68 documents)](#category-D)
- [Category E (59 documents)](#category-E)
- [Category F (76 documents)](#category-F)
- [Category G (8 documents)](#category-G)
- [Category H (2 documents)](#category-H)
- [Global: Requirements](#global-requirements)
- [Global: Risks](#global-risks)
- [Global: CAPA log](#global-capa-log)
- [Global: Audit trail](#global-audit-trail)

---

<a name="category-A"></a>
## Category A

### DOC-A-A_PRIME_0_7_AMENDMENT_PLAN — A'.0.7 Amendment Plan — old/new text pairs

- **Path**: `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Amendment plans behave like briefs (EXECUTED post-landing, not LOCKED); Category A + Tier 3 + EXECUTED override per Pass 2 §1.3
- **Risks referenced**: RISK-008
- **CAPA referenced**: CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION

### DOC-A-ARCHITECTURE — Dual Frontier architecture (umbrella)

- **Path**: `docs/architecture/ARCHITECTURE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 0.4
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-ARCHITECTURE_TYPE_SYSTEM — Architecture Type System — Attribute-as-Declaration Verification

- **Path**: `docs/architecture/ARCHITECTURE_TYPE_SYSTEM.md`
- **Tier**: 1  |  **Lifecycle**: Draft  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: Draft (not LOCKED) — Track B verification activation pending; v0.1 sketch authoring stage

### DOC-A-COMBO_RESOLUTION — Combo damage resolution

- **Path**: `docs/architecture/COMBO_RESOLUTION.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 0.2
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: A vs J borderline classified as A by A'.4.5 execution agent — predominantly architectural pattern (deterministic ordering via ComboResolutionSystem); mechanic-design intent minimal. Cross-referenced from docs/mechanics/ index when future J-category combat-design doc authored

### DOC-A-COMPOSITE_NAMESPACE_DELIBERATION_STATE — Composite Milestone Namespace — Deliberation Final State

- **Path**: `docs/architecture/COMPOSITE_NAMESPACE_DELIBERATION_STATE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-15 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Deliberation state document behaves like amendment plan (EXECUTED post-deliberation, not LOCKED); Category A + Tier 3 + EXECUTED override per Pass 2 §1.3 precedent (cf. DOC-A-K_L3_1_AMENDMENT_PLAN, DOC-A-A_PRIME_0_7_AMENDMENT_PLAN)

### DOC-A-COMPOSITE_REQUESTS — Composite requests

- **Path**: `docs/architecture/COMPOSITE_REQUESTS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: A vs J borderline classified as A by A'.4.5 execution agent — engine-level composite request resolution pattern; design intent minimal

### DOC-A-CONTRACTS — Contract system

- **Path**: `docs/architecture/CONTRACTS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Risks referenced**: RISK-003

### DOC-A-ECS — Entity Component System

- **Path**: `docs/architecture/ECS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-EVENT_BUS — Event buses

- **Path**: `docs/architecture/EVENT_BUS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-FEEDBACK_LOOPS — Feedback-loop resolution

- **Path**: `docs/architecture/FEEDBACK_LOOPS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 0.2
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-FHE_INTEGRATION_CONTRACT — FHE Integration Contract

- **Path**: `docs/architecture/FHE_INTEGRATION_CONTRACT.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-06 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-FIELDS — Field Storage

- **Path**: `docs/architecture/FIELDS.md`
- **Tier**: 1  |  **Lifecycle**: Live  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-11
- **Special-case rationale**: Live (not LOCKED) — populated by K9 closure 2026-05-11; contract concrete but Save/load section TBD until persistence-integration milestone
- **Risks referenced**: RISK-003

### DOC-A-FRAMEWORK — Document Control Register — Governance Framework

- **Path**: `docs/governance/FRAMEWORK.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-17 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-17
- **Requirements authored**: REQ-Q-A45-X5
- **Risks referenced**: RISK-010, RISK-012
- **Meta entry**: yes (role=register_specification)

### DOC-A-GODOT_INTEGRATION — Godot integration (historical; superseded by V substrate)

- **Path**: `docs/architecture/historical/GODOT_INTEGRATION.md`
- **Tier**: 1  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: Superseded by VULKAN_SUBSTRATE.md v1.0 LOCKED per Q-G-1 LOCK (composite namespace deliberation 2026-05-15). Pre-V-substrate authority preserved at docs/architecture/historical/ for historical record. Moved per CLEANUP_CASCADE_BRIEF §1.3 (Crystalka lock 2026-05-16).

### DOC-A-ISOLATION — System isolation

- **Path**: `docs/architecture/ISOLATION.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-K_L3_1_AMENDMENT_PLAN — K-L3.1 Amendment Plan — old/new text pairs

- **Path**: `docs/architecture/K_L3_1_AMENDMENT_PLAN.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Amendment plans behave like briefs (EXECUTED post-landing, not LOCKED); Category A + Tier 3 + EXECUTED override per Pass 2 §1.3
- **Risks referenced**: RISK-004, RISK-008

### DOC-A-KERNEL — DualFrontier Kernel — Architecture & Roadmap

- **Path**: `docs/architecture/KERNEL_ARCHITECTURE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 2.3
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-20 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-18
- **Requirements authored**: REQ-K-L1, REQ-K-L2, REQ-K-L3, REQ-K-L4, REQ-K-L5, REQ-K-L6, REQ-K-L7, REQ-K-L7_1, REQ-K-L8, REQ-K-L9, REQ-K-L10, REQ-K-L11, REQ-K-L12, REQ-K-L13, REQ-K-L14, REQ-K-L15, REQ-K-L16, REQ-K-L17, REQ-K-L18, REQ-K-L19
- **Risks referenced**: RISK-001, RISK-002, RISK-003, RISK-004, RISK-013
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-A-KERNEL_FULL_NATIVE_SCHEDULER — К10 Native Kernel Scheduler — Architecture Specification

- **Path**: `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-17 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-17
- **Special-case rationale**: К10 specification document. Sister к K10_DELIBERATION_STATE.md (Project file, not register-tracked). Major amendment landed 2026-05-17 (v1.0 → v2.0) per К10 deliberation arc — 9 S surfaces ratified, 8 new К-L invariants + 2 sub-invariants (К-L6 SUPERSEDED + К-L7.1 sub + К-L12 through К-L19), 46 items, TLA+ scope. Tier 1 LOCKED status promoted at this enrollment. Requirements (К-L12 through К-L19) not yet enrolled as REQ entries; deferred к К-closure report (А'.8).
- **Risks referenced**: RISK-002, RISK-003, RISK-004, RISK-013

### DOC-A-MAX_ENG_REFACTOR_TRACK_B — Track B Activation — Type System Verification

- **Path**: `docs/architecture/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md`
- **Tier**: 1  |  **Lifecycle**: Draft  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: Draft — Track B activation pending Phase A'.9 analyzer milestone; v0.1 conceptual draft

### DOC-A-MIGRATION_PLAN — Migration Plan — Kernel-to-Vanilla (K-series → M-series)

- **Path**: `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.4
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-10
- **Risks referenced**: RISK-004
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-A-MOD_OS — Mod OS Architecture — Dual Frontier

- **Path**: `docs/architecture/MOD_OS_ARCHITECTURE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.11
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-20 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-18
- **Risks referenced**: RISK-002, RISK-004, RISK-005, RISK-006
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-A-MOD_PIPELINE — Mod Pipeline

- **Path**: `docs/architecture/MOD_PIPELINE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 0.3
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-MODDING — Writing mods

- **Path**: `docs/architecture/MODDING.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Risks referenced**: RISK-005

### DOC-A-OWNERSHIP_TRANSITION — Golem ownership transitions

- **Path**: `docs/architecture/OWNERSHIP_TRANSITION.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-PERFORMANCE — Performance

- **Path**: `docs/architecture/PERFORMANCE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-PHASE_A_PRIME_SEQUENCING — Phase A' sequencing — K-L3.1 to M-series begins

- **Path**: `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-18 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: Category A + Tier 2 + Live override: document is mutable per phase closure, subordinate to MIGRATION_PLAN_KERNEL_TO_VANILLA — not LOCKED architecture per Pass 2 §1.3

### DOC-A-RESOURCE_MODELS — Resource models

- **Path**: `docs/architecture/RESOURCE_MODELS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: A vs J borderline classified as A by A'.4.5 execution agent — architectural data structure approach for resource modeling; gameplay-design layer captured separately if/when J-category resource-balance doc authored

### DOC-A-SYNTHESIS_RATIONALE — Document Control Register — Synthesis Rationale

- **Path**: `docs/governance/SYNTHESIS_RATIONALE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Meta entry**: yes (role=register_provenance)

### DOC-A-THREADING — Multithreading

- **Path**: `docs/architecture/THREADING.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-VISUAL_ENGINE — Visual engine — DevKit and Native (historical; superseded by V substrate)

- **Path**: `docs/architecture/historical/VISUAL_ENGINE.md`
- **Tier**: 1  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: Superseded by VULKAN_SUBSTRATE.md v1.0 LOCKED per Q-G-1 LOCK (composite namespace deliberation 2026-05-15). Pre-V-substrate authority preserved at docs/architecture/historical/ for historical record. Moved per CLEANUP_CASCADE_BRIEF §1.3 (Crystalka lock 2026-05-16).

### DOC-A-VULKAN_SUBSTRATE — Vulkan Substrate (V) — Dual Frontier

- **Path**: `docs/architecture/VULKAN_SUBSTRATE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-20 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-16
- **Special-case rationale**: Unified V substrate per Q-G-1 LOCK (COMPOSITE_NAMESPACE_DELIBERATION_STATE.md §3.1). Supersedes prior DOC-A-RUNTIME (RUNTIME_ARCHITECTURE.md) + DOC-A-GPU_COMPUTE (GPU_COMPUTE.md); single Vulkan substrate covers rendering + compute use cases. Additionally supersedes G-series briefs DOC-D-G0..G9 per Q-G-2 LOCK + CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16). Additionally supersedes DOC-A-VISUAL_ENGINE + DOC-A-GODOT_INTEGRATION (visual-runtime authority moved to docs/architecture/historical/) per CLEANUP_CASCADE_BRIEF §1.3 (Crystalka lock 2026-05-16). Bidirectional integrity per FRAMEWORK §3.3.2.
- **Risks referenced**: RISK-004, RISK-013

---

<a name="category-B"></a>
## Category B

### DOC-B-CODING_STANDARDS — Coding standards

- **Path**: `docs/methodology/CODING_STANDARDS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-B-DEVELOPMENT_HYGIENE — Development hygiene

- **Path**: `docs/methodology/DEVELOPMENT_HYGIENE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Risks referenced**: RISK-011

### DOC-B-MAXIMUM_ENGINEERING_REFACTOR — Maximum Engineering Refactor — Discipline Escalation Brief

- **Path**: `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-10
- **CAPA referenced**: CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION

### DOC-B-METHODOLOGY — Dual Frontier development methodology

- **Path**: `docs/methodology/METHODOLOGY.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.8
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-18 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-17
- **Requirements authored**: REQ-Q-A07-6
- **Risks referenced**: RISK-007, RISK-008, RISK-009, RISK-010, RISK-011, RISK-014
- **CAPA referenced**: CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION

### DOC-B-PIPELINE_METRICS — Pipeline metrics — empirical record

- **Path**: `docs/methodology/PIPELINE_METRICS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 0.2
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-10
- **Risks referenced**: RISK-014
- **CAPA referenced**: CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION

### DOC-B-TESTING_STRATEGY — Testing strategy

- **Path**: `docs/methodology/TESTING_STRATEGY.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

---

<a name="category-C"></a>
## Category C

### DOC-C-IDEAS_RESERVOIR — Ideas Reservoir (index for docs/ideas/ Category I bank)

- **Path**: `docs/IDEAS_RESERVOIR.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-06 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: Post-A'.4.5 functions as index for Category I docs/ideas/ folder; existing content preserved verbatim; individual ideas grow as separate files in docs/ideas/ over time

### DOC-C-MIGRATION_PROGRESS — Native Migration — Progress Tracker

- **Path**: `docs/MIGRATION_PROGRESS.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

### DOC-C-ROADMAP — Roadmap

- **Path**: `docs/ROADMAP.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

---

<a name="category-D"></a>
## Category D

### DOC-D-A_PRIME_0_5_REORG_REFRESH — A'.0.5 — Reorg and Refresh

- **Path**: `tools/briefs/A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_0_7_CLOSURE_EXECUTION — A'.0.7 — Closure Execution

- **Path**: `tools/briefs/A_PRIME_0_7_CLOSURE_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_0_7_METHODOLOGY_RESTRUCTURE — A'.0.7 — Methodology Restructure

- **Path**: `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **CAPA referenced**: CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION

### DOC-D-A_PRIME_1_AMENDMENT_EXECUTION — A'.1 — Amendment Execution

- **Path**: `tools/briefs/A_PRIME_1_AMENDMENT_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_4_5_PASS_1 — A'.4.5 Pass 1 — Q4 Standards Selection Lock

- **Path**: `tools/briefs/A_PRIME_4_5_PASS_1_Q4_STANDARDS_SELECTION_LOCK.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_4_5_PASS_2 — A'.4.5 Pass 2 — Classification Model Lock

- **Path**: `tools/briefs/A_PRIME_4_5_PASS_2_CLASSIFICATION_MODEL_LOCK.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_4_5_PASS_3 — A'.4.5 Pass 3 — Schema + Tooling + Protocol Lock

- **Path**: `tools/briefs/A_PRIME_4_5_PASS_3_SCHEMA_TOOLING_PROTOCOL_LOCK.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_4_5_PASS_4 — A'.4.5 Pass 4 — Auxiliary Cascade Locks

- **Path**: `tools/briefs/A_PRIME_4_5_PASS_4_AUXILIARY_CASCADE_LOCKS.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_4_5_PASS_5 — A'.4.5 Pass 5 — Production Entries

- **Path**: `tools/briefs/A_PRIME_4_5_PASS_5_PRODUCTION_ENTRIES.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_4_5_REGISTER_BRIEF — A'.4.5 — Document Control Register (deliberation brief)

- **Path**: `tools/briefs/A_PRIME_4_5_DOCUMENT_CONTROL_REGISTER_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-A_PRIME_8_K_CLOSURE_REPORT — А'.8 K-Series Closure Report (SKELETON)

- **Path**: `tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED-SKELETON  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-17 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Skeleton brief for К-series formal closure report. Substantial scope (~3-6 weeks). 8-part structure (chronology, К-L invariants, empirical results, lessons, pipeline metrics, Roslyn analyzer prep, K→M transition, open work). Awaits full brief authoring at К10 execution closure (А'.7) timing.
- **Risks referenced**: RISK-014

### DOC-D-A_PRIME_9_ROSLYN_ANALYZER — А'.9 Roslyn Architectural Analyzer (SKELETON)

- **Path**: `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED-SKELETON  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-17 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Skeleton brief for Roslyn architectural analyzer encoding К-Lxx invariants. Dual purpose (M-series migration verifier + К-Lxx compile-time enforcement). 17-18 expected analyzer rules. Awaits full brief authoring at К-closure report (А'.8) landed timing.
- **Risks referenced**: RISK-004

### DOC-D-ARCHITECTURE_RECON_BRIEF — Architecture Reconnaissance Brief

- **Path**: `tools/briefs/ARCHITECTURE_RECON_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-15 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-CLEANUP_CASCADE_BRIEF — Cleanup Cascade Execution Brief — 18 of 19 audit findings

- **Path**: `tools/briefs/CLEANUP_CASCADE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Execution-mode brief addressing 18 of 19 findings from DOC-E-DOCUMENTATION_DRIFT_AUDIT_REPORT (DRIFT-019 historical residue no-action per audit; DRIFT-016 HALTED per CLEANUP_CASCADE_BRIEF §4 SC-4 — Q-K-1 A'-cycle renumbering deferral surfaced to Crystalka for K8.5-brief-time decision). Authority chain: audit findings + Crystalka 4 cleanup locks (§1.1-1.4) + VULKAN_SUBSTRATE.md v1.0 LOCKED + SystemExecutionContext.cs/IGameServices.cs/IModApi.cs canonical statements.

### DOC-D-COMPOSITE_NAMESPACE_RATIFICATION_BRIEF — Composite Milestone Namespace — Ratification Execution Brief

- **Path**: `tools/briefs/COMPOSITE_NAMESPACE_RATIFICATION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G0 — G0 — Vulkan Compute Plumbing (historical; consolidated into V0)

- **Path**: `tools/briefs/historical/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G0 consolidated into V0 (Vulkan substrate foundation, covers rendering + compute use cases). Brief content superseded by VULKAN_SUBSTRATE.md §1.1 + §3.4. Retained as historical record of pre-Q-G-1/Q-G-2 architectural intent. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).
- **Risks referenced**: RISK-013

### DOC-D-G1 — G1 — Mana Diffusion (historical; demonstrated by M-V1 on V1)

- **Path**: `tools/briefs/historical/G1_MANA_DIFFUSION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G1 reframed as M-V1 demonstration (Vanilla.Magic mana field) on V1 substrate primitive (isotropic diffusion). Brief content superseded by VULKAN_SUBSTRATE.md §1.2 + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G2 — G2 — Electricity Anisotropic (historical; demonstrated by M-V2 on V1)

- **Path**: `tools/briefs/historical/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G2 reframed as M-V2 demonstration (Vanilla.Electricity power field) on V1 anisotropic diffusion variant. Brief content superseded by VULKAN_SUBSTRATE.md §1.2 + §5.1 + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G3 — G3 — Storage Capacitance (historical; reduced to gameplay node config)

- **Path**: `tools/briefs/historical/G3_STORAGE_CAPACITANCE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G3 storage cells / capacitance reduced to gameplay-level node config (not shader feature, not substrate primitive). Brief content superseded by VULKAN_SUBSTRATE.md §5.1 + Lesson #12 candidate. Retained as historical record of reduction rationale. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G4 — G4 — Multi-Field Coexistence (historical; reframed as V substrate close criterion)

- **Path**: `tools/briefs/historical/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G4 multi-field coexistence reframed as V substrate close acceptance criterion (not separate primitive). Brief content superseded by VULKAN_SUBSTRATE.md §1.4 (V substrate close acceptance criteria). Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G5 — G5 — Projectile Domain B (historical; substrate disposition deferred)

- **Path**: `tools/briefs/historical/G5_PROJECTILE_DOMAIN_B_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G5 projectile Domain B substrate disposition deferred to M-V5 reactivation amendment authoring (whether V3 primitive, separate substrate, or consumer-level). M-V5 identifier reserved per Q-R-1. Brief content cross-referenced from VULKAN_SUBSTRATE.md §1.3.2 + §5.6. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G6 — G6 — Flow Field Infrastructure (historical; folded into V2)

- **Path**: `tools/briefs/historical/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G6 flow field infrastructure folded into V2 wave shader side products (distance/direction fields). Brief content superseded by VULKAN_SUBSTRATE.md §1.3 + §5.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G7 — G7 — Vanilla Movement (historical; demonstrated by M-V7 on V2)

- **Path**: `tools/briefs/historical/G7_VANILLA_MOVEMENT_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G7 reframed as M-V7 demonstration (Vanilla.Movement routed flow field pathfinding) on V2 substrate primitive (wave shader). Brief content superseded by VULKAN_SUBSTRATE.md §1.3 + §5.3 + §5.5 + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G8 — G8 — Local Avoidance (historical; demonstrated by M-V8, mod-level)

- **Path**: `tools/briefs/historical/G8_LOCAL_AVOIDANCE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G8 local avoidance reframed as M-V8 demonstration — mod-level concern, NOT V substrate primitive. Brief content superseded by VULKAN_SUBSTRATE.md §5.5 (local avoidance separate concern) + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-G9 — G9 — Eikonal Upgrade (historical; deferred — V2 tunable or V3)

- **Path**: `tools/briefs/historical/G9_EIKONAL_UPGRADE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Q-G-2 LOCK: G9 eikonal upgrade deferred — possibly folded into V2 tunable parameter (Option A vs Option B), or becomes separate V-N primitive — evidence-gated at amendment authoring. Brief content cross-referenced from VULKAN_SUBSTRATE.md §1.3 + §1.3.1 + §5.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).

### DOC-D-K_L3_1_ADDENDUM_1 — K-L3.1 — Brief Addendum 1

- **Path**: `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K_L3_1_BRIDGE — K-L3.1 — Bridge Formalization

- **Path**: `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-004, RISK-006
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-D-K_LESSONS_BATCH — K-Lessons Batch

- **Path**: `tools/briefs/K_LESSONS_BATCH_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-009

### DOC-D-K0 — K0 — Cherry Pick

- **Path**: `tools/briefs/K0_CHERRY_PICK_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-07 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K1 — K1 — Batching

- **Path**: `tools/briefs/K1_BATCHING_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-07 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-002

### DOC-D-K10_1 — К10.1 — Kernel Scheduler Core Execution

- **Path**: `tools/briefs/K10_1_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-18 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: К10.1 standalone execution brief — first of four К10 sub-milestones under Option III standalone-briefs structure (К10.1 = kernel scheduler core; К10.2 = native bus + mod ALC lifecycle native primitives; К10.3 = pipeline depth + display composition + hardware tier; К10.4 = TLA+ formal verification). Implements 17 of 46 items from KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED: §3.1 Items 1-5, §3.2 Items 6-8, §3.3 Items 9 + 11-13 (Item 10 NUMA deferred к К-extensions), §3.4 Items 15-16 (Item 14 К11+), §3.5 Items 17 + 19-20, §3.7 Item 24. Ratifies К-L6 SUPERSEDED + К-L12/L13/L14 architecturally established at Commit 14 (load-bearing). К10 as whole closes only after К10.4 sub-milestone; К-closure report (А'.8) waits for all four К10 sub-milestones. Brief authored from К10 deliberation arc 2026-05-16..2026-05-17 (9 S-locks ratified). Distinct from DOC-D-K10_EXECUTION (skeleton brief for original 10-sub-milestone partitioning, retained as historical record under Option III restructuring).
- **Risks referenced**: RISK-002, RISK-003, RISK-004, RISK-013

### DOC-D-K10_2 — К10.2 — Native bus three-tier dispatch + mod ALC lifecycle native primitives

- **Path**: `tools/briefs/K10_2_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-18 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: К10.2 standalone execution brief — second of four К10 sub-milestones under Option III standalone-briefs structure (К10.1 closed 2026-05-18; К10.2 = native bus three-tier dispatch + mod ALC lifecycle native primitives; К10.3 = pipeline depth + display composition + hardware tier; К10.4 = TLA+ formal verification). Implements 8 of 46 items from KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED: §3.8 Items 26-30 (native bus three-tier dispatcher + event type registry + subscriber contract + background queue), §3.9 Items 31-32 (background queue save-integrated storage + native unload primitive), §3.6 Item 21 (mod scheduler authority per-mod sub-schedulers). К-L15 «Native bus authority + three-tier event dispatch» AUTHORED at Commit 13 (load-bearing). Strategy: managed-facade-preserved per К10.1 precedent — native bus parallel infrastructure landed; managed bus remains dispatch authority; sovereign authority switch deferred к К10.4 / А'.8. Cumulative с К10.1: 25 of 46 К10 items closed; remaining 21 items distributed across К10.3 (12 items: 33-44), К10.4 (3 items: 18, 45, 46), Item 14 deferred к К11+, Item 25 cross-cutting к А'.8. Brief authored 2026-05-18 from К10 deliberation arc 2026-05-16..2026-05-17 (9 S-locks ratified) + К10.1 closure precedent (managed-facade-preserved pattern + native test convention + DF_CHECK runner + selftest infrastructure).
- **Risks referenced**: RISK-002, RISK-003, RISK-004, RISK-013

### DOC-D-K10_3 — К10.3 v2 — Pipeline depth + display composition + mod lifecycle quiescent (К-L7.1/L16/L17/L18)

- **Path**: `tools/briefs/K10_3_EXECUTION_BRIEF_v2.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-19 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: К10.3 v2 standalone execution brief — third of four К10 sub-milestones under Option III standalone-briefs structure. v2 supersedes v1 (AUTHORED 2026-05-18 from K10 deliberation arc closure, never enrolled, halted Phase 0 twice). К10.3 v2 implements 10 of 46 К10 items from KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED: §3.10 Items 33-37 (pipeline depth К-L16 + sub-invariant К-L7.1), §3.11 Items 38-40 (display composition К-L17), §3.12 Items 41-42 (mod lifecycle quiescent К-L18). Items 43-44 already landed V0.B as cross-stream prerequisite resolution (К-L19 + HardwareCapabilityCheck + QueueFamilyInfo + README hardware section). Cumulative К10 progress post-К10.3 v2 (if executed): 35 of 46 К10 items closed (К10.1 К-L12/L13/L14 12 items + К10.2 К-L15 8 items + V0.B К-L19 2 items + К10.3 v2 К-L7.1/L16/L17/L18 10 items = 32; + К10.4 3 items pending; + Item 14 deferred К11+; + Item 25 cross-cutting А'.8; + Items 10/22/23 К-extensions scope = 11 remaining). Strategy: managed-facade-preserved per К10.1+К10.2+V0.B precedent — К10.3 v2 architecture landings + helper wirings; full UI / pipeline-managed mod adoption deferred к К-closure or К-extensions. К-L7.1 introduces opt-in pipeline-managed pattern coexisting с V1 К-L7 sync (per К-L9 «Vanilla = mods» author choice per field). К-L17 display composition lives в src/DualFrontier.Application/Display/ NEW directory per S-LOCK-11 Crystalka 2026-05-19 lock. К-L18 UI integration scope = SimulationStateController helper + ModMenuController pause hook only per S-LOCK-12 (full settings menu deferred). VULKAN_SUBSTRATE.md v1.0 → v1.1 reconciliation per S-LOCK-14 consolidates V0.B-deferred K-L19 amendments + К10.3 v2 amendments. v1 brief retained on disk (tools/briefs/K10_3_EXECUTION_BRIEF.md) с superseded_by annotation per Commit 1 enrollment design.
- **Risks referenced**: RISK-002, RISK-003, RISK-004, RISK-013

### DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE — К10 Cross-Document Amendments Cascade (SKELETON)

- **Path**: `tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED-SKELETON  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-17 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Skeleton brief for К10 architectural propagation across 8 dependent documents per KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 Part 7. Awaits full brief authoring at К10 execution closure (А'.7) timing or earlier per Crystalka prioritization.
- **Risks referenced**: RISK-004

### DOC-D-K10_EXECUTION — К10 Native Kernel Scheduler — Execution (SKELETON)

- **Path**: `tools/briefs/K10_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED-SKELETON  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-17 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Skeleton brief awaiting full brief authoring at Opus deliberation session post-К8.5 closure. Captures 10 expected sub-milestones (K10.A through K10.J), 4 halt classes, 5 Q-N seeds. К10 specification authority at DOC-A-KERNEL_FULL_NATIVE_SCHEDULER v2.0 LOCKED.
- **Risks referenced**: RISK-003, RISK-004, RISK-013

### DOC-D-K2 — K2 — Registry Tests

- **Path**: `tools/briefs/K2_REGISTRY_TESTS_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-07 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K3 — K3 — Bootstrap Graph

- **Path**: `tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-08 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K4 — K4 — Component Struct Refactor

- **Path**: `tools/briefs/K4_COMPONENT_STRUCT_REFACTOR_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-08 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-001

### DOC-D-K4_STRUCT_REFACTOR — K4 — Struct Refactor (alternative draft)

- **Path**: `tools/briefs/historical/K4_STRUCT_REFACTOR_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Already register-SUPERSEDED (superseded_by DOC-D-K4). Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.2 to consolidate superseded artifacts (Crystalka lock 2026-05-16).

### DOC-D-K5 — K5 — Span Protocol

- **Path**: `tools/briefs/K5_SPAN_PROTOCOL_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-08 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-002, RISK-003

### DOC-D-K6_1_AFFECTED_TESTS — K6.1 — Affected Tests

- **Path**: `tools/briefs/K6_1_AFFECTED_TESTS.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K6_1_FAULT_WIRING — K6.1 — Fault Wiring

- **Path**: `tools/briefs/K6_1_FAULT_WIRING_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K6_MOD_REBUILD — K6 — Mod Rebuild

- **Path**: `tools/briefs/K6_MOD_REBUILD_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K6_VERIFICATION_LOG — K6 — Verification Log

- **Path**: `tools/briefs/K6_VERIFICATION_LOG.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Verification log is closure-style artifact but housed in briefs/ — classified D Tier 3 per location

### DOC-D-K7 — K7 — Performance Measurement

- **Path**: `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K8_0 — K8.0 — Solution A Recording

- **Path**: `tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K8_1 — K8.1 — Native Reference Primitives

- **Path**: `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-003

### DOC-D-K8_1_1 — K8.1.1 — Interned String Followup

- **Path**: `tools/briefs/K8_1_1_INTERNED_STRING_FOLLOWUP_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K8_2_CLASS_COMPONENT — K8.2 — Class Component Redesign

- **Path**: `tools/briefs/K8_2_CLASS_COMPONENT_REDESIGN_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K8_2_V1_DEPRECATED — K8.2 — Component Conversion v1 (DEPRECATED)

- **Path**: `tools/briefs/historical/K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md`
- **Tier**: 3  |  **Lifecycle**: DEPRECATED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Already register-DEPRECATED (deprecated_by DOC-D-K8_2_V2). Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.2 to consolidate superseded artifacts (Crystalka lock 2026-05-16).

### DOC-D-K8_2_V2 — K8.2 — Component Conversion v2

- **Path**: `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-001
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-D-K8_3 — K8.3 — Production System Migration

- **Path**: `tools/briefs/historical/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Absorbed into K8_34_COMBINED_V2 (EXECUTED at A'.5 closure 2026-05-14 commit 54c6658). Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.2 (Crystalka lock 2026-05-16).
- **Risks referenced**: RISK-007, RISK-008
- **CAPA referenced**: CAPA-2026-05-13-K8.3-PREMISE-MISS

### DOC-D-K8_3_BRIEF_REFRESH_PATCH — K8.3 v2 — Brief Refresh Patch (storage premise correction + K8.3/K8.4 order swap)

- **Path**: `tools/briefs/historical/K8_3_BRIEF_REFRESH_PATCH.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Patch brief for K8.3 v2.0 brief authoring premise miss (storage location vs struct shape conflation); K9_BRIEF_REFRESH_PATCH precedent. Parent K8_3 brief absorbed into K8_34_COMBINED_V2 (EXECUTED at A'.5 closure 2026-05-14 commit 54c6658), so this patch becomes vestigial. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.2 (Crystalka lock 2026-05-16).
- **Risks referenced**: RISK-007, RISK-008
- **CAPA referenced**: CAPA-2026-05-13-K8.3-PREMISE-MISS

### DOC-D-K8_34_COMBINED — K8.3+K8.4 Combined Kernel Cutover (v1.0) — SUPERSEDED

- **Path**: `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-14 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-005, RISK-007, RISK-008
- **CAPA referenced**: CAPA-2026-05-13-K8.3-PREMISE-MISS, CAPA-2026-05-14-K8.34-API-SURFACE-MISS, CAPA-2026-05-14-K8.34-MID-TRANSITION-DRIFT

### DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH — K8.3+K8.4 Combined Brief Refresh Patch (v1) — SUPERSEDED

- **Path**: `tools/briefs/K8_34_COMBINED_BRIEF_REFRESH_PATCH.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-14 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-005, RISK-007, RISK-008
- **CAPA referenced**: CAPA-2026-05-14-K8.34-API-SURFACE-MISS

### DOC-D-K8_34_COMBINED_V2 — K8.3+K8.4 Combined Kernel Cutover (v2.0) — EXECUTED

- **Path**: `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-14 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-005, RISK-007, RISK-008
- **CAPA referenced**: CAPA-2026-05-13-K8.3-PREMISE-MISS, CAPA-2026-05-14-K8.34-API-SURFACE-MISS, CAPA-2026-05-14-K8.34-MID-TRANSITION-DRIFT

### DOC-D-K8_4 — K8.4 — Managed World Retired

- **Path**: `tools/briefs/historical/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Absorbed into K8_34_COMBINED_V2 (EXECUTED at A'.5 closure 2026-05-14 commit 54c6658). Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.2 (Crystalka lock 2026-05-16).
- **Risks referenced**: RISK-005, RISK-007

### DOC-D-K8_5 — K8.5 — Mod Ecosystem Migration Prep

- **Path**: `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED-SKELETON  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-18 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: Skeleton brief awaiting full brief authoring at proper milestone timing. Content (mod ecosystem migration prep from v2 to v3) premised on external mod authors audience; vanilla mods deferred к Phase B per composite namespace ratification (PR #34, 2026-05-16) means no current audience. Promotion к AUTHORED triggers when Phase B initial M-series sprint begins establishing mod author audience. Deferred from Phase А'.6 slot 2026-05-18 per Crystalka direction.
- **Risks referenced**: RISK-005, RISK-007

### DOC-D-K8_DECISION — K8 — Decision Brief

- **Path**: `tools/briefs/K8_DECISION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K9 — K9 — Field Storage

- **Path**: `tools/briefs/K9_FIELD_STORAGE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-002, RISK-007

### DOC-D-K9_BRIEF_REFRESH_PATCH — K9 — Brief Refresh Patch

- **Path**: `tools/briefs/K9_BRIEF_REFRESH_PATCH.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Patch brief for stale K9 brief — precedent for staleness density mitigation per RISK-007

### DOC-D-MOD_OS_V16_AMENDMENT — Mod OS v1.6 — Amendment Brief

- **Path**: `tools/briefs/MOD_OS_V16_AMENDMENT_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-NAMESPACE_CASCADE_RESEARCH_BRIEF — M/G/R Namespace Cascade Map — Research Brief

- **Path**: `tools/briefs/NAMESPACE_CASCADE_RESEARCH_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-15 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-V0_A — V0.A — Win32 window + Vulkan instance + device + queue families + validation layer

- **Path**: `tools/briefs/V0_A_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-18 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: V0.A standalone execution brief — first of three V0 sub-milestones under V substrate foundation split (V0.A = Win32 window + Vulkan instance + device + queue families + validation; V0.B = swapchain + render pass + compute pipeline plumbing + memory allocator + SPIR-V toolchain; V0.C = sprite/text/atlas + PNG decoder + threading model integration + clear color → first textured quad). Implements V0 deliverables per VULKAN_SUBSTRATE.md v1.0 §1.1 rendering side baseline (first 4 of 11 rendering deliverables). Authored 2026-05-18 после K10.3 Phase 0 halt SC-14 (V substrate absent — Option B selected: build V substrate foundation first, then К10.3 restarts against real layer). First Vulkan code на проекте; substantial novel architectural surface vs K-series briefs. Per Crystalka ratification 2026-05-18: V substrate authoring stream inserts between K10.2 closure and K10.3 resumption. К10.3 brief restart pathway gated on V0.A + V0.B closure (compute pipeline plumbing lands V0.B). K-L19 hardware tier surface partially landed at V0.A (Vulkan 1.3 instance creation check); async compute queue selection deferred к V0.B. Per Lesson #22 (match existing convention) + Lesson #20 (no improvisation): pure P/Invoke к vulkan-1.dll (S-LOCK-6), zero third-party binding, ALWAYS-ON validation discipline в DEBUG (S-LOCK-4), .NET 8 target (S-LOCK-5 verified Phase 0 from Directory.Build.props).
- **Risks referenced**: RISK-013

### DOC-D-V0_B — V0.B — Swapchain + render pass + compute pipeline + memory allocator + SPIR-V + async compute + hardware check

- **Path**: `tools/briefs/V0_B_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-18 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: V0.B standalone execution brief — second of three V0 sub-milestones under V substrate foundation split. Implements V0 deliverables per VULKAN_SUBSTRATE.md v1.0 §1.1 rendering bullets 4-9 + compute use case implementation foundation. Authored 2026-05-18 post-V0.A closure 2026-05-18 (PR #36 merged; 11 atomic commits 1a1c772..1a56887; 685 tests baseline; AMD RX 7600S verified К-L19 hardware tier; VkPhysicalDeviceProperties alignment fix precedent landed — Lesson #7 strengthening). Per Crystalka ratification 2026-05-18 (V0.A/V0.B/V0.C split ratified post-V0.A closure): V substrate authoring stream continues. К10.3 brief restart pathway: V0.B implementation lands async compute queue selection (Item 43) + HardwareCapabilityCheck (Item 44) — К10.3 restarts post-V0.B closure с surgical amendments where V0.B shape differs from К10.3 brief assumptions. К-L19 invariant LOCKED at Commit 13 (load-bearing — KERNEL_ARCHITECTURE.md amendment + README.md hardware requirements section + REGISTER.yaml DOC-A-KERNEL version bump 2.1 → 2.2 + REQ-K-L19 enrollment all in same commit per Lesson #8). 11 S-LOCKs enumerate scope: S-LOCK-1 (V0.B scope = swapchain + render pass + compute + memory + SPIR-V + async compute + hardware check); S-LOCK-2 (monolithic approach NOT split V0.B.1/V0.B.2); S-LOCK-3 (native C ABI extension lands в existing DualFrontier.Core.Native module); S-LOCK-4 (memory allocator = bumper linear allocator only); S-LOCK-5 (SPIR-V toolchain = in-repo committed glslangValidator.exe); S-LOCK-6 (V0.B shaders = minimal clearcolor + noop only); S-LOCK-7 (mixed [LibraryImport] + [DllImport] convention formalized — V0.A executor precedent); S-LOCK-8 (C ABI alignment audit mandatory per V0.A executor finding — Lesson #7 strengthening); S-LOCK-9 (К-L19 invariant landing on V0.B post-К10.3 halt resolution); S-LOCK-10 (К10.3 brief restart pathway documented); S-LOCK-11 (atomic cascade preserves V0.A discipline). Per Lesson #22 (match existing convention) + Lesson #20 (no improvisation): same V0.A patterns inherited; mixed [LibraryImport]/[DllImport] formalized; pure P/Invoke к vulkan-1.dll + vulkan-1.dll linkage в native module via CMake find_package(Vulkan); ALWAYS-ON validation discipline в DEBUG preserved; .NET 8 target preserved.
- **Risks referenced**: RISK-013

### DOC-D-V0_C_1 — V0.C.1 — PNG decoder + textured sprite pipeline + input event types (R.1 + R.4)

- **Path**: `tools/briefs/V0_C_1_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-19 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: V0.C.1 standalone execution brief — third sub-milestone of V0 series under V substrate foundation split (V0.A → V0.B → V0.C.1 → V0.C.2). Authored 2026-05-19 post-V0.B closure (PR #37 merged) per Crystalka split ratification 2026-05-19 (V0.C → V0.C.1 + V0.C.2). Implements V0 deliverables per VULKAN_SUBSTRATE.md v1.0 §1.1 rendering bullets 5-9 partial coverage — specifically R.1 (first textured quad) + R.4 (input system) per §4.2 phase mapping. Excludes R.2 (batched sprite renderer at 10,000 sprites), R.3 (TileMap + Camera2D), R.5+ (Domain integration, UI primitives, lifecycle, cutover) — those belong V0.C.2 + post-V substrate close briefs. К10.3 brief restart pathway already open post-V0.B closure; V0.C.1 не gates К10.3 — runs parallel в same V substrate stream. 11 S-LOCKs enumerate scope: S-LOCK-1 (deliverables order: Vulkan struct + functions → PngDecoder → AssetManager → VulkanSampler → TextureUploader → VulkanPipelineLayout extension → sprite shaders → SpriteVertex + types → VulkanSpritePipeline → SpriteRenderer → input event types → Win32 dispatch → Runtime facade → smoke test → closure); S-LOCK-2 (PNG decoder scope = RGBA8 + RGB8 8-bit non-interlaced minimum coverage; interlaced + palette + 16-bit deferred к Lesson #25 pattern); S-LOCK-3 (sprite vertex format = pos+UV+color packed 20 bytes per S-LOCK-3); S-LOCK-4 (sprite shader scope = textured + tinted + camera MVP push constant); S-LOCK-5 (alpha blending = premultiplied alpha workflow standard); S-LOCK-6 (VulkanSampler default = nearest+repeat per pixel art aesthetic); S-LOCK-7 (threading model unchanged — V0.C.1 single-threaded; PresentationBridge + Domain integration R.5 post-V substrate close); S-LOCK-8 (push constants для Camera MVP; VulkanPipelineLayout backward-compat extension); S-LOCK-9 (C ABI alignment audit mandatory per Lesson #7 strengthening — 5 brief-estimate corrections V0.B precedent); S-LOCK-10 (input event types + Win32 dispatch completion per VULKAN_SUBSTRATE §2.2); S-LOCK-11 (atomic cascade preserves V0.A + V0.B discipline). Per Lesson #22 (match existing convention) + Lesson #20 (no improvisation): same V0.B patterns inherited; mixed [LibraryImport]/[DllImport] formalized continues; ALWAYS-ON validation discipline в DEBUG preserved; .NET 8 target preserved; default-inclusion bias per К-L14 (all 4 PNG filter predictors, sprite shader vertex+UV+color, premultiplied blending production-shape).
- **Risks referenced**: RISK-013

### DOC-D-V0_C_2 — V0.C.2 — Batched sprite renderer + Camera2D + TileMap (V substrate R.2 + R.3)

- **Path**: `tools/briefs/V0_C_2_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-19 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: V0.C.2 standalone execution brief — fourth and final sub-milestone of V0 series under V substrate foundation split (V0.A → V0.B → V0.C.1 → V0.C.2). Authored 2026-05-19 post-V0.C.1 closure (PR #38 merged) per Crystalka V0.C split ratification 2026-05-19. Implements V0 deliverables per VULKAN_SUBSTRATE.md v1.0 §4.2 R.2 (batched sprite renderer 10K sprites at 60+ FPS) + R.3 (TileMap + Camera2D 200×200 grid at 60+ FPS). V0.C.2 closure = V0 substrate close per Q8 ratification — unlocks V1 (scalar field + diffusion shader) + V2 (scalar field + wave shader) brief authoring + Phase B M-cycle vanilla content mass migration (also gated on Roslyn analyzer A'.9). К10.3 brief restart pathway independent — runs parallel в V substrate stream; V0.C.2 не gates К10.3. 11 S-LOCKs enumerate scope: S-LOCK-1 (V0.C.2 scope = batched sprite renderer + Camera2D + TileMap per VULKAN_SUBSTRATE §4.2 R.2 + R.3); S-LOCK-2 (vertex buffer = N-frame ring buffer per Q1c ratification — production-standard pattern matching swapchain image count); S-LOCK-3 (indexed quad rendering per Q2b ratification — 4 vertices + 6 uint16 indices per quad, ~33% vertex memory reduction); S-LOCK-3a (10K hard cap per BeginFrame/EndFrame cycle — uint16 index limit); S-LOCK-4 (Camera2D standard scope per Q3b ratification — position + zoom + rotation + viewport + matrices + transforms, ~150 LOC; culling deferred); S-LOCK-5 (TileMap one-sprite-per-tile per Q4a ratification — reuses batched infrastructure, 200×200 = 40K = 4× R.2 stress); S-LOCK-5a (multi-cycle BeginFrame/EndFrame для grids exceeding 10K cap); S-LOCK-6 (atlas regions code-defined per Q5a ratification — AtlasRegion.FromPixels static factory с validation guards; JSON manifest deferred к V0.C.2.1/V0.D); S-LOCK-7 (SpriteRenderer batched API rewrite — BeginFrame/Submit/EndFrame replaces single-sprite DrawSprite); S-LOCK-8 (K-L19 hardware capability preserved verbatim + Lesson #7 alignment audit continues); S-LOCK-9 (atomic cascade preserves V0.A + V0.B + V0.C.1 discipline + multi-session pause provision per Lesson #8 + Lesson #26); S-LOCK-10 (validation layer ALWAYS-ON DEBUG preserved); S-LOCK-11 (REGISTER governance discipline preserved per A'.4.5). Per Lesson #22 (match existing convention) + Lesson #20 (no improvisation): same V0.A + V0.B + V0.C.1 patterns inherited; mixed [LibraryImport]/[DllImport] formalized continues; ALWAYS-ON validation discipline в DEBUG preserved; .NET 8 target preserved; default-inclusion bias per К-L14 preserved through scope coherence (substantial cohesive scope, не aggregation). К-L14 thesis fourth verification window: V0.A + V0.B + V0.C.1 closed с three consecutive zero-hard-gate-halt cascades; V0.C.2 closure = fourth verification + V0 substrate close pattern reliability empirically validated.
- **Risks referenced**: RISK-013

### DOC-D-V1_V2 — V1+V2 — Scalar field + diffusion shader + wave shader (V substrate primitives, combined deliberation, sequential execution)

- **Path**: `tools/briefs/V1_V2_EXECUTION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-19 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: null
- **Special-case rationale**: V1+V2 combined deliberation execution brief — fifth + sixth sub-milestones of V substrate stream (V0.A → V0.B → V0.C.1 → V0.C.2 → V1 → V2). Authored 2026-05-19 post-V0.C.2 closure (PR #39 merged) per Crystalka substrate authoring stream protocol. Implements V substrate primitives per VULKAN_SUBSTRATE.md v1.0 §1.2 (V1 isotropic + anisotropic diffusion shader) + §1.3 (V2 wave shader baseline + direction extraction) + §1.4 (V substrate close acceptance — multi-field coexistence). V2 closure = V substrate FULL close per §1.4 — unlocks M-V demonstration mod paths (M-V1 Vanilla.Magic, M-V2 Vanilla.Electricity, M-V7 Vanilla.Movement) + Phase B M-cycle vanilla content mass migration (also gated on Roslyn analyzer A'.9). К10.3 brief restart pathway independent — runs parallel в V substrate stream; V1+V2 не gates К10.3. Q1-Q8 ratification 2026-05-19 (Crystalka): hybrid sub-milestone structure (Q1c — V1 monolithic + V2.A wave/distance + V2.B direction extraction split); minimal CPU reference scope (Q2a — single-iteration small grid synthetic tests); direct K9 consumption (Q3a — no new C ABI extensions); extend Directory.Build.props CompileShaders target (Q4a — substrate-side compilation); standard smoke test scenes (Q5b — minimal + multi-field coexistence); combined deliberation + sequential execution (Q6c — V1 PR #40 → V2 PR #41); diffusion baseline для V2 (Q7a — eikonal upgrade deferred TBD per spec); Phase A' naming А'.10 V1 + А'.11 V2 (Q8 — sequential post-V0 sub-milestones). 11 S-LOCKs enumerate scope: S-LOCK-1 (V1+V2 combined scope = scalar field substrate primitives per VULKAN_SUBSTRATE §1.2 + §1.3 + §1.4); S-LOCK-2 (compute shader sourcing = substrate-side compilation per Q4a — extends Directory.Build.props CompileShaders target); S-LOCK-3 (CPU/GPU equivalence test mandatory per §11 — every compute shader has CPU reference, tolerance-bounded comparison gates correctness); S-LOCK-4 (direct K9 consumption — no new C ABI extensions per Q3a; consumes existing FieldHandle<T> + FieldRegistry + V0.B compute pipeline surface verbatim); S-LOCK-5 (ping-pong buffer management via FieldHandle<T>.SwapBuffers — K9 contract preserved); S-LOCK-6 (V0 substrate inheritance preserved — K-L19, validation layer ALWAYS-ON, per-image semaphore, framebuffer recreation, mixed [LibraryImport]/[DllImport]); S-LOCK-7 (alignment audit mandatory per Lesson #7 strengthening — V0.A 1 + V0.B 5 + V0.C.1 0 + V0.C.2 1 maturity curve continues; new push constant structs DiffusionPushConstants/WavePushConstants/DirectionExtractPushConstants get Marshal.SizeOf test gates); S-LOCK-8 (atomic cascade discipline preserves V substrate stream pattern с V1→V2 pause point); S-LOCK-9 (validation layer ALWAYS-ON DEBUG preserved — zero validation messages tolerated as commit gate); S-LOCK-10 (REGISTER.yaml governance discipline preserved — EVT-V1-CLOSURE + EVT-V2-CLOSURE + EVT-V-SUBSTRATE-CLOSE audit_trail events); S-LOCK-11 (CPU reference inheritance + extension per Lesson #11 redundancy check — IsotropicDiffusionKernel inherited verbatim, AnisotropicDiffusionKernel + WaveKernel + DirectionExtractKernel new CPU references follow same pattern). Per Lesson #22 (read existing code first) + Lesson #11 (redundancy check) + Lesson #25 (consumer materialization): V1+V2 inherits substantial existing infrastructure — K9 FieldHandle<T> production-ready, V0.B compute pipeline registration round-trip verified, IsotropicDiffusionKernel CPU reference existing K9-era code consumed verbatim. К-L14 thesis fifth + sixth verification window: V0.A + V0.B + V0.C.1 + V0.C.2 closed с four consecutive zero-hard-gate-halt cascades; V1 + V2 = fifth + sixth verifications; V substrate full close per §1.4 = pattern reliability empirically validated across 6 consecutive substrate sub-milestones.
- **Risks referenced**: RISK-013

---

<a name="category-E"></a>
## Category E

### DOC-E-A_PRIME_0_5_BASELINE — A'.0.5 — Baseline grep counts

- **Path**: `tools/scratch/A_05/BASELINE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-A_PRIME_0_5_DISCOVERY_SUMMARY — A'.0.5 — Discovery Summary

- **Path**: `tools/scratch/A_05/DISCOVERY_SUMMARY.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-A_PRIME_0_5_INVENTORY — A'.0.5 — Inventory (point-in-time baseline 2026-05-10)

- **Path**: `tools/scratch/A_05/INVENTORY.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Inventory baseline; superseded by REGISTER.yaml as living inventory at A'.4.5; legacy artifact retained for historical reference
- **CAPA referenced**: CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT

### DOC-E-A_PRIME_0_5_PIPELINE_TERMINOLOGY — A'.0.5 — Pipeline Terminology

- **Path**: `tools/scratch/A_05/PIPELINE_TERMINOLOGY.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-A_PRIME_0_5_REORG_PLAN — A'.0.5 — Reorg Plan (approved)

- **Path**: `tools/scratch/A_05/REORG_PLAN_APPROVED.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-A_PRIME_0_5_STALENESS_REPORT — A'.0.5 — Staleness Report

- **Path**: `tools/scratch/A_05/STALENESS_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-A_PRIME_0_5_TIER2_FLAGS — A'.0.5 Phase 8 — Tier 2 surfaced debt (forward-flagged)

- **Path**: `tools/scratch/A_05/TIER2_FLAGS.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-A_PRIME_4_5_DELIBERATION_CLOSURE — A'.4.5 — Deliberation Closure (23 Q-locks + 39 entries)

- **Path**: `tools/scratch/A_05/A_PRIME_4_5_DELIBERATION_CLOSURE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-A_PRIME_5_K8_3_HALT_INVESTIGATION — A'.5 K8.3 v2.0 execution halt — partial-investigation artifact

- **Path**: `docs/scratch/A_PRIME_5/HALT_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-13 ($(System.Collections.Hashtable.last_modified_commit))
- **CAPA referenced**: CAPA-2026-05-13-K8.3-PREMISE-MISS

### DOC-E-ARCHITECTURE_RECON_REPORT — Architecture Reconnaissance Report

- **Path**: `docs/reports/ARCHITECTURE_RECON_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-15 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_CAMPAIGN_PLAN — Audit Campaign Plan

- **Path**: `docs/audit/AUDIT_CAMPAIGN_PLAN.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_1_INVENTORY — Audit Pass 1 — Inventory

- **Path**: `docs/audit/AUDIT_PASS_1_INVENTORY.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_1_PROMPT — Audit Pass 1 — Prompt

- **Path**: `docs/audit/AUDIT_PASS_1_PROMPT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_2_PROMPT — Audit Pass 2 — Prompt

- **Path**: `docs/audit/AUDIT_PASS_2_PROMPT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_2_RESUMPTION_PROMPT — Audit Pass 2 — Resumption Prompt

- **Path**: `docs/audit/AUDIT_PASS_2_RESUMPTION_PROMPT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_2_SPEC_CODE — Audit Pass 2 — Spec/Code

- **Path**: `docs/audit/AUDIT_PASS_2_SPEC_CODE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_3_PROMPT — Audit Pass 3 — Prompt

- **Path**: `docs/audit/AUDIT_PASS_3_PROMPT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_3_ROADMAP_REALITY — Audit Pass 3 — Roadmap Reality

- **Path**: `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_4_CROSSDOC_TRANSLATION — Audit Pass 4 — Cross-doc Translation

- **Path**: `docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_4_PROMPT — Audit Pass 4 — Prompt

- **Path**: `docs/audit/AUDIT_PASS_4_PROMPT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_5_PROMPT — Audit Pass 5 — Prompt

- **Path**: `docs/audit/AUDIT_PASS_5_PROMPT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_PASS_5_TRIAGE — Audit Pass 5 — Triage

- **Path**: `docs/audit/AUDIT_PASS_5_TRIAGE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-AUDIT_REPORT — Audit Report

- **Path**: `docs/audit/AUDIT_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-CPP_KERNEL_BRANCH_REPORT — C++ Kernel Branch Report

- **Path**: `docs/reports/CPP_KERNEL_BRANCH_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: E + Tier 2 Live override per Pass 2 §2.2

### DOC-E-DOCUMENTATION_DRIFT_AUDIT_REPORT — Documentation Drift Audit Report

- **Path**: `docs/audit/DOCUMENTATION_DRIFT_AUDIT_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-HOUSEKEEPING_MENU_PAUSES_SIMULATION — Housekeeping — Menu Pauses Simulation

- **Path**: `docs/prompts/HOUSEKEEPING_MENU_PAUSES_SIMULATION.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE — Housekeeping — Menu Position and Assets Gitignore

- **Path**: `docs/prompts/HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-HOUSEKEEPING_NEEDS_DECAY_DIRECTION — Housekeeping — Needs Decay Direction

- **Path**: `docs/prompts/HOUSEKEEPING_NEEDS_DECAY_DIRECTION.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-HOUSEKEEPING_REAL_PAWN_DATA — Housekeeping — Real Pawn Data

- **Path**: `docs/prompts/HOUSEKEEPING_REAL_PAWN_DATA.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-HOUSEKEEPING_TICKSCHEDULER_RACE — Housekeeping — TickScheduler Race

- **Path**: `docs/prompts/HOUSEKEEPING_TICKSCHEDULER_RACE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-HOUSEKEEPING_UI_HONESTY_PASS — Housekeeping — UI Honesty Pass

- **Path**: `docs/prompts/HOUSEKEEPING_UI_HONESTY_PASS.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-K7_BDN_TICK_REPORT — K7 BDN Tick Report

- **Path**: `docs/benchmarks/k7-bdn-tick-report.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-LEARNING_PHASE_1 — Self-teaching ritual artifact — Phase 1 (Самообучение фаза 1)

- **Path**: `docs/learning/PHASE_1.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: ru
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M3_CLOSURE_REVIEW — M3 Closure Review

- **Path**: `docs/audit/M3_CLOSURE_REVIEW.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M4_CLOSURE_REVIEW — M4 Closure Review

- **Path**: `docs/audit/M4_CLOSURE_REVIEW.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M5_CLOSURE_REVIEW — M5 Closure Review

- **Path**: `docs/audit/M5_CLOSURE_REVIEW.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M6_CLOSURE_REVIEW — M6 Closure Review

- **Path**: `docs/audit/M6_CLOSURE_REVIEW.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M7_CLOSURE — M7 Closure

- **Path**: `docs/prompts/M7_CLOSURE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M7_CLOSURE_REVIEW — M7 Closure Review

- **Path**: `docs/audit/M7_CLOSURE_REVIEW.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M7_HOUSEKEEPING_TICK_DISPLAY — M7 — Housekeeping Tick Display

- **Path**: `docs/prompts/M7_HOUSEKEEPING_TICK_DISPLAY.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M73_CODING_STANDARDS_UPDATE — M73 — Coding Standards Update

- **Path**: `docs/prompts/M73_CODING_STANDARDS_UPDATE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M74_BUILD_PIPELINE_OVERRIDE — M74 — Build Pipeline Override

- **Path**: `docs/prompts/M74_BUILD_PIPELINE_OVERRIDE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M75A_MOD_MENU_CONTROLLER — M75A — Mod Menu Controller

- **Path**: `docs/prompts/M75A_MOD_MENU_CONTROLLER.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M75B1_BOOTSTRAP_INTEGRATION — M75B1 — Bootstrap Integration

- **Path**: `docs/prompts/M75B1_BOOTSTRAP_INTEGRATION.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-M75B2_GODOT_UI_SCENE — M75B2 — Godot UI Scene

- **Path**: `docs/prompts/M75B2_GODOT_UI_SCENE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-MOD_OS_V16_AMENDMENT_CLOSURE — Mod OS v1.6 — Amendment Closure

- **Path**: `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Closure artifact lives in tools/briefs/ alongside its parent brief; classified E (closure) Tier 3 EXECUTED

### DOC-E-NAMESPACE_CASCADE_MAP — M/G/R Namespace Cascade Map

- **Path**: `docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md`
- **Tier**: 3  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-15 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-NATIVE_BUILD — Native Build Notes

- **Path**: `native/DualFrontier.Core.Native/build.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Closure-style notes housed in native/ source tree; tracked as Category E rather than F because content is build/closure narrative not module-local README

### DOC-E-NATIVE_CORE_EXPERIMENT — Native Core Experiment

- **Path**: `docs/reports/NATIVE_CORE_EXPERIMENT.md`
- **Tier**: 3  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: E + Tier 2 Live override per Pass 2 §2.2

### DOC-E-NORMALIZATION_REPORT — Normalization Report

- **Path**: `docs/reports/NORMALIZATION_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: E + Tier 2 Live override per Pass 2 §2.2 (live closure report; mutable while series open)

### DOC-E-PASS_2_NOTES — Pass 2 — Notes

- **Path**: `docs/audit/PASS_2_NOTES.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-PASS_3_NOTES — Pass 3 — Notes

- **Path**: `docs/audit/PASS_3_NOTES.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-PASS_4_REPORT — Pass 4 — Report

- **Path**: `docs/audit/PASS_4_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-PERFORMANCE_REPORT_K3 — Performance Report — K3

- **Path**: `docs/reports/PERFORMANCE_REPORT_K3.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-08 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-PERFORMANCE_REPORT_K7 — Performance Report — K7

- **Path**: `docs/reports/PERFORMANCE_REPORT_K7.md`
- **Tier**: 3  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: E default Tier 3 EXECUTED; Tier 2 Live override because K-series still open (K8.3/K8.4/K8.5 pending) and report mutates with subsequent measurement updates per Pass 2 §2.2

### DOC-E-Q_K_1_REPORT — Q-K-1 Execution Report — Verbatim Reconciliation Findings

- **Path**: `docs/scratch/RATIFICATION_EXECUTION/Q_K_1_REPORT.md`
- **Tier**: 3  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-16 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-SESSION_PHASE_4_CLOSURE_REVIEW — Session Phase 4 — Closure Review

- **Path**: `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-TD1_SONNET_BRIEF — TD1 — Sonnet Brief

- **Path**: `docs/prompts/TD1_SONNET_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-E-UI_REVIEW_PRE_M75B2 — UI Review — Pre-M75B2

- **Path**: `docs/audit/UI_REVIEW_PRE_M75B2.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

---

<a name="category-F"></a>
## Category F

### DOC-F-ASSETS-SCENES — Assets scenes

- **Path**: `assets/scenes/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS — Mods directory index

- **Path**: `mods/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS-EXAMPLE — Mod Example

- **Path**: `mods/DualFrontier.Mod.Example/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS-VANILLA-COMBAT — Mod Vanilla Combat

- **Path**: `mods/DualFrontier.Mod.Vanilla.Combat/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS-VANILLA-CORE — Mod Vanilla Core

- **Path**: `mods/DualFrontier.Mod.Vanilla.Core/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS-VANILLA-INVENTORY — Mod Vanilla Inventory

- **Path**: `mods/DualFrontier.Mod.Vanilla.Inventory/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS-VANILLA-MAGIC — Mod Vanilla Magic

- **Path**: `mods/DualFrontier.Mod.Vanilla.Magic/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS-VANILLA-PAWN — Mod Vanilla Pawn

- **Path**: `mods/DualFrontier.Mod.Vanilla.Pawn/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-MODS-VANILLA-WORLD — Mod Vanilla World

- **Path**: `mods/DualFrontier.Mod.Vanilla.World/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-NATIVE-CORE — Native Core module

- **Path**: `native/DualFrontier.Core.Native/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-NATIVE-CORE-INCLUDE — Native Core include

- **Path**: `native/DualFrontier.Core.Native/include/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-NATIVE-CORE-SRC — Native Core src

- **Path**: `native/DualFrontier.Core.Native/src/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-NATIVE-CORE-TEST — Native Core test

- **Path**: `native/DualFrontier.Core.Native/test/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-AI — DualFrontier.AI module

- **Path**: `src/DualFrontier.AI/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-AI-BEHAVIOURTREE — AI BehaviourTree submodule

- **Path**: `src/DualFrontier.AI/BehaviourTree/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-AI-JOBS — AI Jobs submodule

- **Path**: `src/DualFrontier.AI/Jobs/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-AI-PATHFINDING — AI Pathfinding submodule

- **Path**: `src/DualFrontier.AI/Pathfinding/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION — DualFrontier.Application module

- **Path**: `src/DualFrontier.Application/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-BRIDGE — Application Bridge submodule

- **Path**: `src/DualFrontier.Application/Bridge/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-BRIDGE-COMMANDS — Application Bridge Commands submodule

- **Path**: `src/DualFrontier.Application/Bridge/Commands/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-INPUT — Application Input submodule

- **Path**: `src/DualFrontier.Application/Input/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-LOOP — Application Loop submodule

- **Path**: `src/DualFrontier.Application/Loop/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-MODDING — Application Modding submodule

- **Path**: `src/DualFrontier.Application/Modding/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-RENDERING — Application Rendering submodule

- **Path**: `src/DualFrontier.Application/Rendering/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-SAVE — Application Save submodule

- **Path**: `src/DualFrontier.Application/Save/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-SCENARIO — Application Scenario submodule

- **Path**: `src/DualFrontier.Application/Scenario/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-APPLICATION-SCENE — Application Scene submodule

- **Path**: `src/DualFrontier.Application/Scene/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS — DualFrontier.Components module

- **Path**: `src/DualFrontier.Components/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS-BUILDING — Components Building submodule

- **Path**: `src/DualFrontier.Components/Building/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS-COMBAT — Components Combat submodule

- **Path**: `src/DualFrontier.Components/Combat/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS-ITEMS — Components Items submodule

- **Path**: `src/DualFrontier.Components/Items/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS-MAGIC — Components Magic submodule

- **Path**: `src/DualFrontier.Components/Magic/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS-PAWN — Components Pawn submodule

- **Path**: `src/DualFrontier.Components/Pawn/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS-SHARED — Components Shared submodule

- **Path**: `src/DualFrontier.Components/Shared/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-COMPONENTS-WORLD — Components World submodule

- **Path**: `src/DualFrontier.Components/World/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CONTRACTS — DualFrontier.Contracts module

- **Path**: `src/DualFrontier.Contracts/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CONTRACTS-ATTRIBUTES — Contracts Attributes submodule

- **Path**: `src/DualFrontier.Contracts/Attributes/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CONTRACTS-BUS — Contracts Bus submodule

- **Path**: `src/DualFrontier.Contracts/Bus/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CONTRACTS-CORE — Contracts Core submodule

- **Path**: `src/DualFrontier.Contracts/Core/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CONTRACTS-MODDING — Contracts Modding submodule

- **Path**: `src/DualFrontier.Contracts/Modding/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CORE — DualFrontier.Core module

- **Path**: `src/DualFrontier.Core/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CORE-BUS — Core Bus submodule

- **Path**: `src/DualFrontier.Core/Bus/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CORE-ECS — Core ECS submodule

- **Path**: `src/DualFrontier.Core/ECS/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CORE-INTEROP — DualFrontier.Core.Interop module

- **Path**: `src/DualFrontier.Core.Interop/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CORE-MATH — Core Math submodule

- **Path**: `src/DualFrontier.Core/Math/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CORE-REGISTRY — Core Registry submodule

- **Path**: `src/DualFrontier.Core/Registry/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-CORE-SCHEDULING — Core Scheduling submodule

- **Path**: `src/DualFrontier.Core/Scheduling/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-EVENTS — DualFrontier.Events module

- **Path**: `src/DualFrontier.Events/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-EVENTS-COMBAT — Events Combat submodule

- **Path**: `src/DualFrontier.Events/Combat/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-EVENTS-INVENTORY — Events Inventory submodule

- **Path**: `src/DualFrontier.Events/Inventory/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-EVENTS-MAGIC — Events Magic submodule

- **Path**: `src/DualFrontier.Events/Magic/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-EVENTS-PAWN — Events Pawn submodule

- **Path**: `src/DualFrontier.Events/Pawn/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-EVENTS-WORLD — Events World submodule

- **Path**: `src/DualFrontier.Events/World/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PERSISTENCE — DualFrontier.Persistence module

- **Path**: `src/DualFrontier.Persistence/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PRESENTATION — DualFrontier.Presentation module

- **Path**: `src/DualFrontier.Presentation/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PRESENTATION-ADDONS-DEVKIT — Presentation addons df_devkit

- **Path**: `src/DualFrontier.Presentation/addons/df_devkit/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PRESENTATION-INPUT — Presentation Input submodule

- **Path**: `src/DualFrontier.Presentation/Input/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PRESENTATION-NATIVE — DualFrontier.Presentation.Native module

- **Path**: `src/DualFrontier.Presentation.Native/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PRESENTATION-NODES — Presentation Nodes submodule

- **Path**: `src/DualFrontier.Presentation/Nodes/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PRESENTATION-SCENES — Presentation Scenes submodule

- **Path**: `src/DualFrontier.Presentation/Scenes/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-PRESENTATION-UI — Presentation UI submodule

- **Path**: `src/DualFrontier.Presentation/UI/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS — DualFrontier.Systems module

- **Path**: `src/DualFrontier.Systems/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS-COMBAT — Systems Combat submodule

- **Path**: `src/DualFrontier.Systems/Combat/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS-FACTION — Systems Faction submodule

- **Path**: `src/DualFrontier.Systems/Faction/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS-INVENTORY — Systems Inventory submodule

- **Path**: `src/DualFrontier.Systems/Inventory/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS-MAGIC — Systems Magic submodule

- **Path**: `src/DualFrontier.Systems/Magic/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS-MAGIC-INTERNAL — Systems Magic Internal submodule

- **Path**: `src/DualFrontier.Systems/Magic/Internal/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS-PAWN — Systems Pawn submodule

- **Path**: `src/DualFrontier.Systems/Pawn/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-SRC-SYSTEMS-WORLD — Systems World submodule

- **Path**: `src/DualFrontier.Systems/World/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-TESTS — Tests module index

- **Path**: `tests/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-TESTS-CORE — Core tests

- **Path**: `tests/DualFrontier.Core.Tests/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-TESTS-CORE-INTEROP — Core Interop tests

- **Path**: `tests/DualFrontier.Core.Interop.Tests/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-11 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-TESTS-MODDING — Modding tests

- **Path**: `tests/DualFrontier.Modding.Tests/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-TESTS-SYSTEMS — Systems tests

- **Path**: `tests/DualFrontier.Systems.Tests/README.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-TOOLS-BRIEFS — Tools briefs module

- **Path**: `tools/briefs/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-F-TOOLS-GOVERNANCE — Tools governance module (register tooling)

- **Path**: `tools/governance/MODULE.md`
- **Tier**: 4  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))

---

<a name="category-G"></a>
## Category G

### DOC-G-BYPASS_LOG — Register Validation Bypass Tracking

- **Path**: `docs/governance/BYPASS_LOG.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

### DOC-G-DOCS_README — Dual Frontier documentation index

- **Path**: `docs/README.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

### DOC-G-IDEAS_README — docs/ideas — Ideas Bank index

- **Path**: `docs/ideas/README.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

### DOC-G-MECHANICS_README — docs/mechanics — Game Mechanics index

- **Path**: `docs/mechanics/README.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

### DOC-G-README — Dual Frontier — root README

- **Path**: `README.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

### DOC-G-REGISTER — Document Control Register (operational SoT)

- **Path**: `docs/governance/REGISTER.yaml`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: Meta-entry self-reference (Q-A45-X2). YAML format exempt from .md-only included_extensions rule because this IS the SoT artifact. Cadence on-every-milestone-closure is load-bearing enforcement for Q-A45-X5 post-session protocol
- **Risks referenced**: RISK-010
- **Meta entry**: yes (role=register_source_of_truth)

### DOC-G-REGISTER_RENDER — Document Control Register — Rendered View

- **Path**: `docs/governance/REGISTER_RENDER.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: Auto-regenerated by render_register.ps1 on register changes; manual baseline at A'.4.5 closure for bootstrap
- **Meta entry**: yes (role=register_rendered_derivative)

### DOC-G-VALIDATION_REPORT — Register Validation Report

- **Path**: `docs/governance/VALIDATION_REPORT.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: Auto-regenerated by sync_register.ps1 on every validation run; not a meta_entry (operational artifact, not register specification)

---

<a name="category-H"></a>
## Category H

### DOC-H-TRANSLATION_GLOSSARY — Translation Glossary — Dual Frontier

- **Path**: `docs/TRANSLATION_GLOSSARY.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-04-26 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: Content LOCKED v1.0 (lexical commitments final); lifecycle Live because H category allows Live/EXECUTED only — content-stability tracked via version pinning rather than LOCKED lifecycle

### DOC-H-TRANSLATION_PLAN — Dual Frontier translation plan (Russian → English)

- **Path**: `docs/TRANSLATION_PLAN.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3

---

<a name="global-requirements"></a>
## Global: Requirements

| ID | Title | Status | Source document | Milestone |
|---|---|---|---|---|
| REQ-K-L1 | Native language: C++20 | VERIFIED | DOC-A-KERNEL | K0 closure |
| REQ-K-L10 | Native organicity Lvl 1 (each native artifact independent) | VERIFIED | DOC-A-KERNEL | K0 closure (D3 codification) |
| REQ-K-L11 | Single NativeWorld backbone — production storage | VERIFIED | DOC-A-KERNEL | K8.2 v2 closure |
| REQ-K-L12 | Native kernel scheduling — sovereign per-tick scheduling for kernel-space systems | VERIFIED | DOC-A-KERNEL | K10.1 closure |
| REQ-K-L13 | On-demand system activation — 5 wake types | VERIFIED | DOC-A-KERNEL | K10.1 closure |
| REQ-K-L14 | Performance derives from architectural cleanliness | VERIFIED | DOC-A-KERNEL | K10.1 closure (architecturally established; measurable evidence pending К11+ per S4 lock — К10.1 brief authoring section §6.1 #6 measurement plan) |
| REQ-K-L15 | Native bus authority + three-tier event dispatch | VERIFIED | DOC-A-KERNEL | K10.2 closure (architecturally established; sovereign authority switch deferred к K10.4 closure / А'.8 per managed-facade-preserved strategy — К10.1 precedent applied) |
| REQ-K-L16 | Simulation tick pipeline depth (D=2 default; configurable 1-3) | VERIFIED | DOC-A-KERNEL | К10.3 v2 load-bearing commit 1/3 (К-L7.1 + К-L16 grouped per Approach C) |
| REQ-K-L17 | Display composition multi-layer (К-L17, К10.3 v2 load-bearing 2/3) | VERIFIED | DOC-A-KERNEL | К10.3 v2 load-bearing commit 2/3 (К-L17 display composition framework + Items 38-40 implementation backing) |
| REQ-K-L18 | Mod lifecycle quiescent state (К-L18, К10.3 v2 load-bearing 3/3) | VERIFIED | DOC-A-KERNEL | К10.3 v2 load-bearing commit 3/3 (К-L18 mod lifecycle quiescent state + Items 41-42 implementation backing) |
| REQ-K-L19 | Hardware tier commitment (Vulkan 1.3 + async compute queue family mandate) | VERIFIED | DOC-A-KERNEL | V0.B closure (full implementation backing operational at landing per Lesson #8 + Lesson #11 — no architectural commitment без implementation behind it) |
| REQ-K-L2 | Bindings: Pure P/Invoke | VERIFIED | DOC-A-KERNEL | K0 closure |
| REQ-K-L3 | Component storage paths: Path α default + Path β opt-in | PARTIAL | DOC-A-KERNEL | K-L3.1 bridge formalization |
| REQ-K-L4 | Explicit component registry (no reflection-driven) | VERIFIED | DOC-A-KERNEL | K4 closure |
| REQ-K-L5 | Span protocol (zero-copy enumeration) | VERIFIED | DOC-A-KERNEL | K5 closure |
| REQ-K-L6 | Mod rebuild capability (load/unload/reload without process restart) | VERIFIED | DOC-A-KERNEL | K6 closure |
| REQ-K-L7 | Read-only spans + write batching | VERIFIED | DOC-A-KERNEL | K5 closure |
| REQ-K-L7_1 | GPU compute pipeline slot binding (sub-invariant к K-L7; opt-in coexistence) | VERIFIED | DOC-A-KERNEL | К10.3 v2 load-bearing commit 1/3 (К-L7.1 + К-L16 grouped per Approach C — pipeline depth + sub-invariant share physical reality «GPU pipeline slots») |
| REQ-K-L8 | Native scheduler (parallel system execution) | VERIFIED | DOC-A-KERNEL | K3 closure |
| REQ-K-L9 | Performance threshold met (V3 dominates V2 by 4-32× across metrics) | VERIFIED | DOC-A-KERNEL | K7 closure |
| REQ-Q-A07-6 | Audience contract: methodology corpus agent-as-primary-reader | VERIFIED | DOC-B-METHODOLOGY | A'.0.7 closure (methodology); A'.4.5 closure (governance inheritance) |
| REQ-Q-A45-X5 | Post-session update protocol mandatory | PENDING | DOC-A-FRAMEWORK | A'.4.5 closure |
| REQ-V0-A-VALIDATION_LAYER | Vulkan validation layer ALWAYS-ON в DEBUG + ValidationLog ring buffer (V0.A) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.A closure |
| REQ-V0-A-VULKAN_DEVICE | Vulkan physical + logical device + graphics queue family selection (V0.A) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.A closure |
| REQ-V0-A-VULKAN_INSTANCE | Vulkan instance с К-L19 Vulkan 1.3 verification + DEBUG validation layer (V0.A) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.A closure (К-L19 surface partial; hardware capability check startup logic deferred к V0.B alongside async compute queue selection) |
| REQ-V0-A-WIN32_WINDOW | Win32 window lifecycle + message pump (V0.A foundation) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.A closure |
| REQ-V0-B-ASYNC_COMPUTE_QUEUE | Async compute queue family selection (V0.B, K-L19 Item 43) | VERIFIED | DOC-A-KERNEL | V0.B closure |
| REQ-V0-B-COMPUTE_PIPELINE | VkPipeline (compute) + descriptor sets + dispatch (V0.B) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.B closure |
| REQ-V0-B-HARDWARE_CHECK | HardwareCapabilityCheck.Verify startup fail-fast (V0.B, K-L19 Item 44) | VERIFIED | DOC-A-KERNEL | V0.B closure |
| REQ-V0-B-MEMORY_ALLOCATOR | Bumper linear memory allocator + VulkanBuffer + VulkanImage primitives (V0.B) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.B closure |
| REQ-V0-B-RENDER_PASS | VkRenderPass + VkFramebuffer + command infrastructure (V0.B) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.B closure |
| REQ-V0-B-SPIRV_TOOLCHAIN | In-repo SPIR-V toolchain integration (V0.B) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.B closure |
| REQ-V0-B-SWAPCHAIN | VkSwapchainKHR + Win32 surface + recreation on resize (V0.B) | VERIFIED | DOC-A-VULKAN_SUBSTRATE | V0.B closure |
| REQ-V0-C-1-INPUT_EVENTS | Input event types complete (Key/MouseButton enums + 6 event records + VirtualKeyMapper) | VERIFIED | DOC-D-V0_C_1 | V0.C.1 closure |
| REQ-V0-C-1-PNG_DECODER | Manual PNG decoder (8-bit RGB/RGBA, non-interlaced, all 4 filter predictors) | VERIFIED | DOC-D-V0_C_1 | V0.C.1 closure |
| REQ-V0-C-1-SAMPLER | VulkanSampler primitive (nearest+repeat default per S-LOCK-6 pixel art aesthetic) | VERIFIED | DOC-D-V0_C_1 | V0.C.1 closure |
| REQ-V0-C-1-SPRITE_PIPELINE | VulkanSpritePipeline (vertex input + alpha blending + descriptor sets + push constants) | VERIFIED | DOC-D-V0_C_1 | V0.C.1 closure |
| REQ-V0-C-1-SPRITE_RENDERER | SpriteRenderer single-sprite-per-draw API с descriptor set caching | VERIFIED | DOC-D-V0_C_1 | V0.C.1 closure |
| REQ-V0-C-1-TEXTURE_UPLOAD | TextureUploader staging-buffer → device-local image transfer + layout transitions | VERIFIED | DOC-D-V0_C_1 | V0.C.1 closure |
| REQ-V0-C-1-WIN32_INPUT_DISPATCH | Win32 message dispatch (WM_KEYDOWN/UP/MOUSEMOVE/L|R|MBUTTONDOWN/UP/MOUSEWHEEL/SETFOCUS/KILLFOCUS → InputEventQueue) | VERIFIED | DOC-D-V0_C_1 | V0.C.1 closure |

<a name="global-risks"></a>
## Global: Risks

| ID | Title | Status | Type | Likelihood | Impact |
|---|---|---|---|---|---|
| RISK-001 | Component struct refactor scope underestimated | CLOSED | Architectural | Medium-High | Medium |
| RISK-002 | P/Invoke marshalling correctness (UTF-8 strings, byte* patterns, pinning) | RESIDUAL | Technical | Medium | High |
| RISK-003 | Native ↔ Managed ownership boundary leaks (handle lifetime, span escape) | ACTIVE | Technical | Medium | Critical |
| RISK-004 | Cross-document drift between LOCKED specs (KERNEL/MOD_OS/RUNTIME/MIGRATION_PLAN) | ACTIVE | Architectural | Medium-High | High |
| RISK-005 | Mod ecosystem compatibility breakage on IModApi version bumps | ACTIVE | Architectural | Medium | High |
| RISK-006 | Path α / Path β bridge complexity exceeds mental model for mod authors | ACTIVE | Architectural | Medium | Medium |
| RISK-007 | Brief staleness density grows with subsequent milestone closures | ACTIVE | Methodological | High | Medium |
| RISK-008 | Amendment plan completeness gap (incomplete enumeration of edits required) | ACTIVE | Methodological | Medium | Medium |
| RISK-009 | Lesson-applier latency (lessons learned not propagated to subsequent briefs) | ACTIVE | Methodological | Medium | Low |
| RISK-010 | Register itself degrades into stale artifact if post-session protocol not enforced | ACTIVE | Methodological | Medium | Critical |
| RISK-011 | Environmental incidents (testhost.exe file lock, dotnet test verbosity gotcha, tooling drift) | ACTIVE | Operational | Medium-High | Low |
| RISK-012 | Cross-platform tooling debt (PowerShell-only governance tooling locks Windows-only ops) | ACCEPTED | Operational | Medium | Low |
| RISK-013 | Vulkan driver compatibility on weak hardware (compute pipeline rollout G-series) | ACTIVE | External | Medium | High |
| RISK-014 | Long-horizon pipeline degradation (single-developer methodology over 6-12 months) | ACTIVE | Methodological | Medium | Critical |

<a name="global-capa-log"></a>
## Global: CAPA log

| ID | Opened | Status | Trigger (summary) |
|---|---|---|---|
| CAPA-2026-05-09-K8.2-V2-REFRAMING | 2026-05-09 | CLOSED | K-L3 «без exception» framing surfaced as misalignment at K8.2 v2 closure verification. |
| CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION | 2026-05-10 | CLOSED | Methodology corpus (METHODOLOGY/PIPELINE_METRICS/MAXIMUM_ENGINEERING_REFACTOR) |
| CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT | 2026-05-12 | CLOSED | A'.0.5 INVENTORY.md baseline ~135 .md files. A'.4.5 deliberation pre-flight |
| CAPA-2026-05-13-K8.3-PREMISE-MISS | 2026-05-13 | CLOSED | K8.3 v2.0 brief authoring (2026-05-13, Opus deliberation, commit f7e6d52) |
| CAPA-2026-05-14-K8.34-MID-TRANSITION-DRIFT | 2026-05-14 | CLOSED | K8.3+K8.4 combined brief v1.0 §6.5 split the storage cutover into 12 incremental |
| CAPA-2026-05-14-K8.34-API-SURFACE-MISS | 2026-05-14 | CLOSED | K8.3+K8.4 combined brief v1.0 (2026-05-14) prescribed `new ComponentTypeRegistry()` — |
| CAPA-2026-05-16-MOD-API-V3-AUTHORITY | 2026-05-16 | CLOSED | Audit DRIFT-011 (S4) + DRIFT-012 (S4): MOD_OS_ARCHITECTURE §4.6 + §4.6.3 framed |
| CAPA-2026-05-16-V-SUBSTRATE-SUPERSESSION | 2026-05-16 | CLOSED | Audit DRIFT-009 (S4) + DRIFT-013 (S3) + DRIFT-014 (S3) + DRIFT-015 (S3): |
| CAPA-2026-05-16-POWER-DELETION-PROPAGATION | 2026-05-16 | CLOSED | Audit DRIFT-006 (S4) + DRIFT-007 (S4) + DRIFT-008 (S4): CONTRACTS + EVENT_BUS |
| CAPA-2026-05-16-ISOLATION-AUTHORITY-RESTORATION | 2026-05-16 | CLOSED | Audit DRIFT-003 (S5) + DRIFT-005 (S5): README + ISOLATION + THREADING + PERFORMANCE |
| CAPA-2026-05-16-LIVE-STATE-CLOSURE-PROTOCOL-GAP | 2026-05-16 | CLOSED | Audit DRIFT-001 (S4): MIGRATION_PROGRESS.md last_updated 2026-05-12 pre-A'.5; |
| CAPA-2026-05-18-K8_5-DRIFT | 2026-05-18 | CLOSED | K8.5 brief reconnaissance per Lesson #22 (read existing code before brief authoring) surfaced |

<a name="global-audit-trail"></a>
## Global: Audit trail

| Date | Event | Type | Commits |
|---|---|---|---|
| 2026-05-10 | A'.0.7 methodology pipeline restructure deliberation | deliberation_milestone | pre-86b721a |
| 2026-05-10 | A'.0.7 methodology rewrite landing (A'.1.M) | amendment_landing | 86b721a..9d4da64 |
| 2026-05-10 | A'.3 push to origin | governance_event | all backlog through 38c2e19 |
| 2026-05-10 | K-L3.1 bridge formalization deliberation | deliberation_milestone | pre-2df5921 |
| 2026-05-10 | K-L3.1 amendment plan execution (A'.1.K) | amendment_landing | 2df5921..0789bd4 |
| 2026-05-10 | A'.0.5 documentation reorganization + cross-ref refresh | execution_milestone | 27523ac..4e332bb |
| 2026-05-11 | A'.4 K9 field storage execution | execution_milestone | ce4dba8..80c9ba6 |
| 2026-05-12 | A'.4.5 register execution closure | execution_milestone | TBD |
| 2026-05-12 | A'.4.5 document control register deliberation | deliberation_milestone | 7448267..411c284 |
| 2026-05-13 | K8.3 v2.0 execution halt — storage premise mismatch; first post-A'.4.5 stop-escalate-lock invocation | governance_event | f7e6d52..6c79914 |
| 2026-05-14 | A'.5 K8.3+K8.4 combined milestone v2.0 closure — atomic storage cutover landed; managed World retired from production | execution_milestone | 24e5f56..PENDING-COMMIT-4 |
| 2026-05-16 | Cleanup cascade closing 18 of 19 audit findings; 5 CAPAs opened+closed within same governance event; DRIFT-016 halted per SC-4 | governance_event | e68d799..PENDING-COMMIT-16 |
| 2026-05-17 | Brief skeleton framework — schema extension AUTHORED-SKELETON + 4 К/А' forward planning skeleton briefs authored | governance_event | 15ffa0a..PENDING-COMMIT-8 |
| 2026-05-17 | К10 deliberation amendments application — METHODOLOGY v1.8 + KERNEL_FULL_NATIVE_SCHEDULER v2.0 landed; new Tier 1 LOCKED enrollment | amendment_landing | 8c3cf5a..PENDING-COMMIT-5 |
| 2026-05-18 | V0.A sub-milestone closure — V substrate foundation prerequisite layer (Win32 + Vulkan instance + device + queue families + validation); первая Vulkan code на проекте; unblocks K10.3 brief restart pathway after V0.B compute plumbing closure | execution_milestone | 1a1c772..PENDING-COMMIT-V0_A-CLOSURE |
| 2026-05-18 | V0.B sub-milestone closure — V substrate foundation completion (swapchain + render pass + framebuffer + command infrastructure + minimal graphics pipeline + memory allocator + SPIR-V toolchain + compute pipeline plumbing + async compute queue + HardwareCapabilityCheck + native C ABI extension + FieldStorageBinding); К-L19 hardware tier invariant LOCKED с full implementation backing; smoke test exit criteria operational на real К-L19 hardware; К10.3 brief restart pathway opens | execution_milestone | d2c6627..PENDING-COMMIT-V0_B-CLOSURE |
| 2026-05-18 | К10.2 sub-milestone closure — native bus three-tier dispatch + mod ALC lifecycle (8 of 46 К10 items; 25 cumulative) | execution_milestone | a677388..PENDING-COMMIT-K10_2-CLOSURE |
| 2026-05-18 | K8.5 deferral cascade — DOC-D-K8_5 reclassified AUTHORED → AUTHORED-SKELETON, Phase A'.6 slot SKIPPED, milestone deferred к post-Phase B | governance_event | 4bc34c1..PENDING-COMMIT-K8_5-CLOSURE |
| 2026-05-18 | К10.1 sub-milestone closure — kernel scheduler core (17 of 46 К10 items) | execution_milestone | f439b74..PENDING-COMMIT-K10_1-CLOSURE |
| 2026-05-19 | V1 sub-milestone closure (PR #40) — V substrate primitive: scalar field + isotropic + anisotropic diffusion compute shader operational; AnisotropicDiffusionKernel CPU reference + diffusion.comp GLSL + DiffusionPushConstants alignment-audited (S-LOCK-7) + native VkCmdDispatch с per-field shadow VkBuffers + V1DiffusionPipeline managed wrapper + V1DiffusionPipeline Runtime factories + CPU/GPU equivalence gates (isotropic uniform D + corner reflective + decay-only + combined D+K + iteration count Theory + anisotropic wire-path + insulator column + insulator-with-gap + long-run mass-conservation 50-iter) + V1 200×200 isotropic + anisotropic wire-path smoke scenes + V1 dispatch latency benchmark + Compute MODULE.md V1 extension + V1 manual visual verification protocol; fifth consecutive zero-hard-gate-halt cascade on V substrate authoring stream (V0.A → V0.B → V0.C.1 → V0.C.2 → V1) — К-L14 thesis fifth verification accumulated. | execution_milestone | 9cbaed3..PENDING-COMMIT-V1-CLOSURE |
| 2026-05-19 | V0.C.2 sub-milestone closure — V substrate R.2 (batched sprite renderer 10K sprites at 60+ FPS target) + R.3 (TileMap + Camera2D 200×200 grid at 60+ FPS target) operational; VertexBufferRing N-frame ring buffer + SpriteIndexBuffer pre-populated uint16 pattern + Camera2D standard scope + TileMap one-sprite-per-tile + SpriteRenderer batched BeginFrame/Submit/EndFrame rewrite + AtlasRegion.FromPixels hardening + Runtime facade extension с Camera + RecordSpritesFrame batched + multi-cycle render pass helpers; V0 substrate close achieved per Q8 ratification — V1 + V2 brief authoring + Phase B M-cycle vanilla migration unblocked (latter also gated on Roslyn analyzer A'.9); 4 consecutive zero-hard-gate-halt cascades on V substrate authoring stream (V0.A → V0.B → V0.C.1 → V0.C.2) — К-L14 thesis empirically validated | execution_milestone | b4084f1..PENDING-COMMIT-V0_C_2-CLOSURE |
| 2026-05-19 | V0.C.1 sub-milestone closure — V substrate R.1 (first textured quad) + R.4 (input system) operational; PngDecoder + AssetManager + VulkanSampler + TextureUploader + sprite shaders + VulkanSpritePipeline + SpriteRenderer + 6 input event types + Win32 dispatch + Runtime facade composition; smoke test exit criteria operational на real К-L19 hardware (820 frames at 164 FPS, validation log 0 errors); V0.C.2 brief restart pathway opens | execution_milestone | 4c4be8f..PENDING-COMMIT-V0_C_1-CLOSURE |


