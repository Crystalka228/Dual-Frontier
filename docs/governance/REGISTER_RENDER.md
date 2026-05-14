# DualFrontier Document Control Register — Rendered View

*Auto-generated from [REGISTER.yaml](./REGISTER.yaml) by `tools/governance/render_register.ps1`. Do not edit — edit REGISTER.yaml instead.*

*Last generated: 2026-05-13  |  Schema version: 1.0  |  Register version: 1.1*

---

## Statistics

- Total documents: 231
- Tier 1: 33  |  Tier 2: 18  |  Tier 3: 102  |  Tier 4: 78  |  Tier 5: 0
- Per category: A=30  |  B=6  |  C=3  |  D=49  |  E=55  |  F=78  |  G=8  |  H=2  |  I=0  |  J=0
- Open CAPA: 2  |  Active risks: 12  |  Stale documents: 0

---

## Table of contents

- [Category A (30 documents)](#category-A)
- [Category B (6 documents)](#category-B)
- [Category C (3 documents)](#category-C)
- [Category D (49 documents)](#category-D)
- [Category E (55 documents)](#category-E)
- [Category F (78 documents)](#category-F)
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
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 0.3
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
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

### DOC-A-COMPOSITE_REQUESTS — Composite requests

- **Path**: `docs/architecture/COMPOSITE_REQUESTS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: A vs J borderline classified as A by A'.4.5 execution agent — engine-level composite request resolution pattern; design intent minimal

### DOC-A-CONTRACTS — Contract system

- **Path**: `docs/architecture/CONTRACTS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Risks referenced**: RISK-003

### DOC-A-ECS — Entity Component System

- **Path**: `docs/architecture/ECS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-EVENT_BUS — Event buses

- **Path**: `docs/architecture/EVENT_BUS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
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
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Requirements authored**: REQ-Q-A45-X5
- **Risks referenced**: RISK-010, RISK-012
- **Meta entry**: yes (role=register_specification)

### DOC-A-GODOT_INTEGRATION — Godot integration

- **Path**: `docs/architecture/GODOT_INTEGRATION.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-GPU_COMPUTE — GPU Compute

- **Path**: `docs/architecture/GPU_COMPUTE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-06 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Risks referenced**: RISK-013

### DOC-A-ISOLATION — System isolation

- **Path**: `docs/architecture/ISOLATION.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
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
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.5
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-10
- **Requirements authored**: REQ-K-L1, REQ-K-L2, REQ-K-L3, REQ-K-L4, REQ-K-L5, REQ-K-L6, REQ-K-L7, REQ-K-L8, REQ-K-L9, REQ-K-L10, REQ-K-L11
- **Risks referenced**: RISK-001, RISK-002, RISK-003, RISK-004
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-A-MAX_ENG_REFACTOR_TRACK_B — Track B Activation — Type System Verification

- **Path**: `docs/architecture/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md`
- **Tier**: 1  |  **Lifecycle**: Draft  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: Draft — Track B activation pending Phase A'.9 analyzer milestone; v0.1 conceptual draft

### DOC-A-MIGRATION_PLAN — Migration Plan — Kernel-to-Vanilla (K-series → M-series)

- **Path**: `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-10
- **Risks referenced**: RISK-004
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-A-MOD_OS — Mod OS Architecture — Dual Frontier

- **Path**: `docs/architecture/MOD_OS_ARCHITECTURE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.7
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-10
- **Risks referenced**: RISK-002, RISK-004, RISK-005, RISK-006
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-A-MOD_PIPELINE — Mod Pipeline

- **Path**: `docs/architecture/MOD_PIPELINE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 0.2
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-MODDING — Writing mods

- **Path**: `docs/architecture/MODDING.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
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
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-PHASE_A_PRIME_SEQUENCING — Phase A' sequencing — K-L3.1 to M-series begins

- **Path**: `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: Category A + Tier 2 + Live override: document is mutable per phase closure, subordinate to MIGRATION_PLAN_KERNEL_TO_VANILLA — not LOCKED architecture per Pass 2 §1.3

### DOC-A-RESOURCE_MODELS — Resource models

- **Path**: `docs/architecture/RESOURCE_MODELS.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Special-case rationale**: A vs J borderline classified as A by A'.4.5 execution agent — architectural data structure approach for resource modeling; gameplay-design layer captured separately if/when J-category resource-balance doc authored

### DOC-A-RUNTIME — Runtime Architecture — Dual Frontier

- **Path**: `docs/architecture/RUNTIME_ARCHITECTURE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-06 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Risks referenced**: RISK-004, RISK-013

### DOC-A-SYNTHESIS_RATIONALE — Document Control Register — Synthesis Rationale

- **Path**: `docs/governance/SYNTHESIS_RATIONALE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
- **Meta entry**: yes (role=register_provenance)

### DOC-A-THREADING — Multithreading

- **Path**: `docs/architecture/THREADING.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

### DOC-A-VISUAL_ENGINE — Visual engine — DevKit and Native

- **Path**: `docs/architecture/VISUAL_ENGINE.md`
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12

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
- **Tier**: 1  |  **Lifecycle**: LOCKED  |  **Version**: 1.7
- **Owner**: Crystalka  |  **Content language**: mixed
- **Last modified**: 2026-05-12 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2027-05-12
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

### DOC-D-G0 — G0 — Vulkan Compute Plumbing

- **Path**: `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-013

### DOC-D-G1 — G1 — Mana Diffusion

- **Path**: `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G2 — G2 — Electricity Anisotropic

- **Path**: `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G3 — G3 — Storage Capacitance

- **Path**: `tools/briefs/G3_STORAGE_CAPACITANCE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G4 — G4 — Multi-Field Coexistence

- **Path**: `tools/briefs/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G5 — G5 — Projectile Domain B

- **Path**: `tools/briefs/G5_PROJECTILE_DOMAIN_B_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G6 — G6 — Flow Field Infrastructure

- **Path**: `tools/briefs/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G7 — G7 — Vanilla Movement

- **Path**: `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G8 — G8 — Local Avoidance

- **Path**: `tools/briefs/G8_LOCAL_AVOIDANCE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-G9 — G9 — Eikonal Upgrade

- **Path**: `tools/briefs/G9_EIKONAL_UPGRADE_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-XX ($(System.Collections.Hashtable.last_modified_commit))

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

- **Path**: `tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: SUPERSEDED  |  **Version**: 0.1
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-08 ($(System.Collections.Hashtable.last_modified_commit))

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

- **Path**: `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md`
- **Tier**: 3  |  **Lifecycle**: DEPRECATED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))

### DOC-D-K8_2_V2 — K8.2 — Component Conversion v2

- **Path**: `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-001
- **CAPA referenced**: CAPA-2026-05-09-K8.2-V2-REFRAMING

### DOC-D-K8_3 — K8.3 — Production System Migration

- **Path**: `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 2.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-13 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-007, RISK-008
- **CAPA referenced**: CAPA-2026-05-13-K8.3-PREMISE-MISS

### DOC-D-K8_3_BRIEF_REFRESH_PATCH — K8.3 v2 — Brief Refresh Patch (storage premise correction + K8.3/K8.4 order swap)

- **Path**: `tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-13 ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Patch brief for K8.3 v2.0 brief authoring premise miss (storage location vs struct shape conflation); K9_BRIEF_REFRESH_PATCH precedent; lifecycle transitions to EXECUTED at A'.6 K8.3 closure post-K8.4 landing
- **Risks referenced**: RISK-007, RISK-008
- **CAPA referenced**: CAPA-2026-05-13-K8.3-PREMISE-MISS

### DOC-D-K8_4 — K8.4 — Managed World Retired

- **Path**: `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
- **Risks referenced**: RISK-005, RISK-007

### DOC-D-K8_5 — K8.5 — Mod Ecosystem Migration Prep

- **Path**: `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md`
- **Tier**: 3  |  **Lifecycle**: AUTHORED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-10 ($(System.Collections.Hashtable.last_modified_commit))
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
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: E + Tier 2 Live override per Pass 2 §2.2

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

### DOC-E-NATIVE_BUILD — Native Build Notes

- **Path**: `native/DualFrontier.Core.Native/build.md`
- **Tier**: 3  |  **Lifecycle**: EXECUTED  |  **Version**: 1.0
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Special-case rationale**: Closure-style notes housed in native/ source tree; tracked as Category E rather than F because content is build/closure narrative not module-local README

### DOC-E-NATIVE_CORE_EXPERIMENT — Native Core Experiment

- **Path**: `docs/reports/NATIVE_CORE_EXPERIMENT.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-04-XX ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: E + Tier 2 Live override per Pass 2 §2.2

### DOC-E-NORMALIZATION_REPORT — Normalization Report

- **Path**: `docs/reports/NORMALIZATION_REPORT.md`
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
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
- **Tier**: 2  |  **Lifecycle**: Live  |  **Version**: Live
- **Owner**: Crystalka  |  **Content language**: en
- **Last modified**: 2026-05-09 ($(System.Collections.Hashtable.last_modified_commit))
- **Next review due**: 2026-Q3
- **Special-case rationale**: E default Tier 3 EXECUTED; Tier 2 Live override because K-series still open (K8.3/K8.4/K8.5 pending) and report mutates with subsequent measurement updates per Pass 2 §2.2

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

### DOC-F-SRC-EVENTS-POWER — Events Power submodule

- **Path**: `src/DualFrontier.Events/Power/README.md`
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

### DOC-F-SRC-SYSTEMS-POWER — Systems Power submodule

- **Path**: `src/DualFrontier.Systems/Power/README.md`
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
| REQ-K-L2 | Bindings: Pure P/Invoke | VERIFIED | DOC-A-KERNEL | K0 closure |
| REQ-K-L3 | Component storage paths: Path α default + Path β opt-in | PARTIAL | DOC-A-KERNEL | K-L3.1 bridge formalization |
| REQ-K-L4 | Explicit component registry (no reflection-driven) | VERIFIED | DOC-A-KERNEL | K4 closure |
| REQ-K-L5 | Span protocol (zero-copy enumeration) | VERIFIED | DOC-A-KERNEL | K5 closure |
| REQ-K-L6 | Mod rebuild capability (load/unload/reload without process restart) | VERIFIED | DOC-A-KERNEL | K6 closure |
| REQ-K-L7 | Read-only spans + write batching | VERIFIED | DOC-A-KERNEL | K5 closure |
| REQ-K-L8 | Native scheduler (parallel system execution) | VERIFIED | DOC-A-KERNEL | K3 closure |
| REQ-K-L9 | Performance threshold met (V3 dominates V2 by 4-32× across metrics) | VERIFIED | DOC-A-KERNEL | K7 closure |
| REQ-Q-A07-6 | Audience contract: methodology corpus agent-as-primary-reader | VERIFIED | DOC-B-METHODOLOGY | A'.0.7 closure (methodology); A'.4.5 closure (governance inheritance) |
| REQ-Q-A45-X5 | Post-session update protocol mandatory | PENDING | DOC-A-FRAMEWORK | A'.4.5 closure |

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
| CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT | 2026-05-12 | OPEN | A'.0.5 INVENTORY.md baseline ~135 .md files. A'.4.5 deliberation pre-flight |
| CAPA-2026-05-13-K8.3-PREMISE-MISS | 2026-05-13 | OPEN | K8.3 v2.0 brief authoring (2026-05-13, Opus deliberation, commit f7e6d52) |

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
| 2026-05-13 | K8.3 v2.0 execution halt — storage premise mismatch; first post-A'.4.5 stop-escalate-lock invocation | governance_event | f7e6d52..06d3b1f |


