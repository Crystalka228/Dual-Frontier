---
register_id: DOC-D-K10_AMENDMENTS_APPLICATION
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-05-17
last_modified: 2026-05-17
content_language: mixed
next_review_due: null
title: K10 Deliberation Amendments — Application Execution Brief
last_modified_commit: a1a7f9b
review_cadence: on-cascade-execution
reviewer: Crystalka
special_case_rationale: 'Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per operator ruling (b): briefs D/3/EXECUTED; real git provenance.'
---

# K10 Deliberation Amendments — Application Execution Brief

**Brief authored**: 2026-05-17 (Claude Opus 4.7, deliberation mode)
**Target session**: Claude Code execution mode
**Brief type**: Application / amendment execution
**Estimated time**: 3-5 hours auto-mode (4-6 days at hobby pace)
**Branch**: `claude/k10-amendments-application`
**Parent**: К10 deliberation arc 2026-05-16 → 2026-05-17 (all 9 S surfaces ratified)

---

## 0. Executive summary

К10 architectural deliberation closed 2026-05-17 (Crystalka + Opus 4.7 deliberation mode). All 9 S surfaces ratified. Three amendment documents attached к Claude Code session for application к target documents.

**Goal**: Land all К10 deliberation outcomes onto repository. Three target documents updated, one document enrollment к REGISTER.yaml added, closure protocol executed.

**Out of scope**: К10 specification implementation (separate К10 execution brief, future). Cross-document amendment cascade (KERNEL_ARCHITECTURE, VULKAN_SUBSTRATE, etc) — separate brief or discrete sessions.

**Three attached amendment documents**:

1. `K10_DELIBERATION_STATE.md` — full replacement of Project file (deliberation state document)
2. `METHODOLOGY_AMENDMENT_S6.md` — patch for `docs/methodology/METHODOLOGY.md` v1.7 → v1.8
3. `KERNEL_FULL_NATIVE_SCHEDULER_AMENDMENT.md` — patch for `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v1.0 → v2.0 (Tier 1 LOCKED candidate)

**Target outcomes**:

- 3 atomic commits landing amendments
- 1 atomic commit enrolling KERNEL_FULL_NATIVE_SCHEDULER.md в REGISTER.yaml
- 1 atomic commit closure (sync_register.ps1 --validate, EVT entry, register_version bump)
- All 12 К-L invariants (К-L3, К-L4, К-L5, К-L7, К-L7.1, К-L8, К-L11, К-L12, К-L13, К-L15, К-L16, К-L18) reflected в KERNEL_FULL_NATIVE_SCHEDULER.md post-amendment
- METHODOLOGY.md v1.8 contains Lessons #11, #20, #22 + Provisional Lessons section
- К10 spec promoted к Tier 1 LOCKED status (post-amendment)

**Lesson discipline reminders**:

- **Lesson #7**: APIs transcribed, not paraphrased. Если amendment specifies API surface or path, verify before authoring final commit content.
- **Lesson #8**: Each atomic commit produces compilable + test-passing state. METHODOLOGY/KERNEL/REGISTER amendments are documentation; «compilable» means «document well-formed, frontmatter valid, no broken cross-references».
- **Lesson #11**: Architectural reduction checks. When applying amendments, не invent additional changes. Apply specified amendments only.
- **Lesson #20**: Tactical heuristics не arguments against architectural completeness. Если amendment seems verbose, that's по design (research framework context).
- **Lesson #22**: Read existing target documents before applying patches. Patches specify insertion points, но existing structure must be verified.

---

## 1. Phase 0 — Preflight reads (mandatory)

Phase 0 reads establish ground truth before any edit. Lesson #22 applies — read existing code/docs before designing mechanism. In this case, мechanism = amendment application; «existing code» = target documents + REGISTER.yaml navigation.

**Required reads** (in order):

### 1.1 Attached amendment documents (3 files)

These are attached к Claude Code session by Crystalka. Read fully:

- `K10_DELIBERATION_STATE.md` (616 lines, 36 KB) — full deliberation state. Project file destination.
- `METHODOLOGY_AMENDMENT_S6.md` (217 lines, 16 KB) — patch для METHODOLOGY.md v1.7 → v1.8.
- `KERNEL_FULL_NATIVE_SCHEDULER_AMENDMENT.md` (477 lines, 32 KB) — patch для KERNEL_FULL_NATIVE_SCHEDULER.md v1.0 → v2.0.

### 1.2 REGISTER.yaml navigation

`docs/governance/REGISTER.yaml` (192 KB, 280+ document entries) — navigation source of truth. Use для resolving target document paths и lifecycle status.

**Key entries для this brief**:

| ID | Path | Lifecycle | Version | Notes |
|---|---|---|---|---|
| DOC-A-KERNEL | docs/architecture/KERNEL_ARCHITECTURE.md | LOCKED | 1.5 | NOT amended в this brief (К-L table update is cross-document amendment, separate brief) |
| DOC-A-MOD_OS | docs/architecture/MOD_OS_ARCHITECTURE.md | LOCKED | 1.8 | NOT amended в this brief (capability sections separate brief) |
| DOC-A-VULKAN_SUBSTRATE | docs/architecture/VULKAN_SUBSTRATE.md | LOCKED | 1.0 | NOT amended в this brief (substantial amendment separate brief) |
| DOC-A-MIGRATION_PLAN | docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md | LOCKED | 1.4 | NOT amended в this brief |
| DOC-B-METHODOLOGY | docs/methodology/METHODOLOGY.md | LOCKED | 1.7 | **AMENDED v1.7 → v1.8** |
| DOC-A-PHASE_A_PRIME_SEQUENCING | docs/architecture/PHASE_A_PRIME_SEQUENCING.md | Live | Live | NOT amended в this brief |
| DOC-G-REGISTER | docs/governance/REGISTER.yaml | Live | 1.4 | **AMENDED — new entry для KERNEL_FULL_NATIVE_SCHEDULER + version bump + EVT entry** |
| DOC-A-FRAMEWORK | docs/governance/FRAMEWORK.md | LOCKED | 1.0 | Read for register specification reference |
| DOC-G-VALIDATION_REPORT | docs/governance/VALIDATION_REPORT.md | Live | Live | Auto-regenerated by sync_register.ps1 |
| DOC-G-REGISTER_RENDER | docs/governance/REGISTER_RENDER.md | Live | 1.0 | Auto-regenerated by render_register.ps1 |

**New entry к add** (Phase 4):

| ID | Path | Lifecycle | Category | Tier |
|---|---|---|---|---|
| DOC-A-KERNEL_FULL_NATIVE_SCHEDULER | docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md | LOCKED | A | 1 |

### 1.3 Target documents (full reads required)

Before applying patches, read full target documents к verify structure:

- `docs/methodology/METHODOLOGY.md` v1.7 (96 KB) — read full document. Verify Lesson #8 section exists и «Reference: K0 lessons learned» follows it. Insertion point = between these two sections.
- `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v1.0 (75 KB, 1288 lines) — read full document. Verify Part 0, Part 2.4, Part 3, Part 4, Part 5, Part 6, Part 7, Part 8, Part 11 sections exist. Amendments target each of these parts.

### 1.4 Governance tooling

- `tools/governance/sync_register.ps1` — read full script. Understand `--validate` exit code semantics (exit 0 = pass, non-zero = fail, blocking condition for governance commits per Q-A45-X5).
- `tools/governance/render_register.ps1` — read full script. Understand REGISTER_RENDER.md regeneration (called в closure protocol).
- `docs/governance/FRAMEWORK.md` v1.0 — read «Post-session protocol» section (§6) + «PENDING-COMMIT» backfilling protocol (§8.3).

### 1.5 Recent governance state

Read recent commit log to understand current state:

```bash
git log --oneline -30 main
```

Expected recent activity (from session memory, verify via actual log):
- A'.5 K8.3+K8.4 combined v2.0 closure (commits 24e5f56..PENDING-COMMIT-4, 2026-05-14)
- Cleanup cascade (commits e68d799..PENDING-COMMIT-16, 2026-05-16)
- Namespace ratification (commit d303fb5 + cluster, 2026-05-16)

If recent commits show NEW governance events not captured above (e.g., A'.6 К8.5 execution started or closed), surface к Crystalka via Halt condition SC-1.

### 1.6 Working tree state

```bash
git status
```

Must be clean before starting. If not clean, surface к Crystalka via Halt condition HG-1 (hard gate).

---

## 2. Phase 1 — Document 1 application: K10_DELIBERATION_STATE.md

### 2.1 Scope

Replace existing Project file (если present) with attached version. K10_DELIBERATION_STATE.md is **Project file**, not commit-cascade target — it lives in Project files for future Claude session access, not committed к repository.

**Source**: Attached `K10_DELIBERATION_STATE.md` (616 lines).
**Target**: Crystalka manually copies из attached к Project files (overwrites existing pre-S8 version).

### 2.2 Claude Code action: NONE

This document is **Project file**, not repository file. **No commit, no edit, no patch**. Crystalka handles Project file replacement manually.

If К10_DELIBERATION_STATE.md is found in repository (e.g., `docs/architecture/K10_DELIBERATION_STATE.md`), surface к Crystalka via Halt condition SC-2 (scope mismatch — Project file vs repository file confusion).

### 2.3 Phase 1 outcome

Zero commits. К10_DELIBERATION_STATE.md handled outside repository. Proceed к Phase 2.

---

## 3. Phase 2 — Document 2 application: METHODOLOGY.md v1.7 → v1.8

### 3.1 Scope

Apply `METHODOLOGY_AMENDMENT_S6.md` к `docs/methodology/METHODOLOGY.md` v1.7. Three new full Lessons + new Provisional Lessons section.

### 3.2 Insertion point identification

Read METHODOLOGY.md v1.7. Locate exact insertion point:

```
[existing] #### Lesson #8 — A brief that splits a change into N steps must prove each of the N−1 intermediate states is valid
[existing content...]
[existing] **Falsifiable claim**: briefs authored under this lesson will not incur mid-transition-drift halts. [...]

<<< INSERT POINT — new Lessons #11, #20, #22 + Provisional Lessons section >>>

[existing] ### Reference: K0 lessons learned
[existing] Concrete K0 closure lessons live в `docs/MIGRATION_PROGRESS.md` K0 entry [...]
```

### 3.3 Content к insert

From `METHODOLOGY_AMENDMENT_S6.md`, insert these sections в order:

1. `#### Lesson #11 — Architectural reduction methodology` (full text from amendment doc «Amendment text к insert into METHODOLOGY.md» section, Lesson #11 subsection)
2. `#### Lesson #20 — Tactical heuristics в research framework (category error)` (full text, Lesson #20 subsection)
3. `#### Lesson #22 — Read existing code + ask operational context before surfacing architectural concerns` (full text, Lesson #22 subsection)
4. `## Provisional Lessons` (new section, full text from amendment doc «Provisional Lessons» section, all 9 candidates)

**Heading depth note**: Existing Lessons #7, #8 use `####` depth. New Lessons #11, #20, #22 use same `####` depth (consistency). New «Provisional Lessons» section is sibling of existing «Pipeline closure lessons (K-series, post-K8.1)» section — likely `###` depth, but verify against METHODOLOGY.md structure at insertion time.

**Lesson #22 reminder**: read METHODOLOGY.md structure first, match heading depth к local convention. Amendment text suggests depth; existing document is authority.

### 3.4 Version header update

METHODOLOGY.md top frontmatter:

```yaml
---
version: "1.7"
---
```

Update к:

```yaml
---
version: "1.8"
---
```

Add new version entry к METHODOLOGY.md prose top (existing pattern: «*Version: 1.7 (2026-05-12). [...]*» — new entry above):

> *Version: 1.8 (2026-05-17). Lessons #11, #20, #22 formalized at К10 deliberation S6 lock. NEW «Provisional Lessons» section captures candidates pending promotion (9 candidates: #9, #10, #14, #15, #16, #17, #18, #19, #21). Lesson formalization model hybrid: high-confidence lessons promoted immediately when surfaced; low-confidence remain provisional pool, promoted at К-closure report (А'.8) timing per accumulated evidence.*

### 3.5 Atomic commit 1

```
git checkout -b claude/k10-amendments-application
[edit METHODOLOGY.md]
git add docs/methodology/METHODOLOGY.md
git commit -m "docs(methodology): Lessons #11/#20/#22 + Provisional Lessons section per К10 S6 lock

METHODOLOGY.md v1.7 → v1.8 per К10 deliberation S6 lock (2026-05-17).

Three full Lessons formalized (immediate promotion):
- #11 Architectural reduction methodology (6 strong applications + Q-G-2 precedent)
- #20 Tactical heuristics в research framework (category error)
- #22 Read existing code + ask operational context before surfacing architectural concerns

NEW Provisional Lessons section (9 candidates pending promotion at А'.8):
- #9 Survey phase before brief authoring
- #10 Architecture audit + tech debt inventory in one pass
- #14 Pre-existing drift cleanup respect deferrals
- #15 Emotional projection avoidance
- #16 Brief length correlates с deliberation complexity
- #17 Performance reasoning tactical vs strategic
- #18 Boundary crossing batching pattern
- #19 On-demand activation pattern
- #21 Redundancy check before default-inclusion (К-L14 complement)

Source: К10 deliberation arc 2026-05-16..2026-05-17 (Crystalka + Opus 4.7); see K10_DELIBERATION_STATE.md Project file.

Refs: DOC-B-METHODOLOGY, EVT-2026-05-17-K10-AMENDMENTS-APPLICATION"
```

**Note on PENDING-COMMIT**: After commit, capture commit hash. Update REGISTER.yaml `DOC-B-METHODOLOGY.last_modified_commit` field в Phase 5 closure commit (per FRAMEWORK §8.3 backfilling protocol).

### 3.6 Phase 2 outcome

1 atomic commit. METHODOLOGY.md v1.7 → v1.8 with 3 new full Lessons + Provisional Lessons section. Proceed к Phase 3.

---

## 4. Phase 3 — Document 3 application: KERNEL_FULL_NATIVE_SCHEDULER.md v1.0 → v2.0

### 4.1 Scope

Apply `KERNEL_FULL_NATIVE_SCHEDULER_AMENDMENT.md` к `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v1.0.

This is **substantial amendment** — 477 amendment lines applied across 9 parts of 1288-line target document. Expected target document post-amendment: ~1700-1900 lines.

### 4.2 Amendment structure breakdown

Amendment specifies changes к following parts (read amendment doc «Amendment summary» table for full mapping):

| Target part | Change scope |
|---|---|
| Part 0 (closing notes) | Append paragraph re К10 deliberation completion + sister document reference |
| Part 2.4 (К-L invariant table) | Major extension — К-L7.1 sub + К-L15/16/17/18/19 + К-L6 SUPERSEDED note + 5 full invariant texts |
| Part 3 (К10 25-item scope) | Major extension — Items 26-46 (21 new items grouped by S lock origin) |
| Part 4 (К10 deliverables) | Extension — TLA+ specs + verification artifacts + documentation artifacts |
| Part 5 (performance predictions) | Restructure — split §5.1.A К10 (18 predictions) + §5.1.B К11+ (5 predictions) |
| Part 6 (Q-N surface) | Extension — Q-N-31 through Q-N-49+ + resolutions noted |
| Part 7 (cross-document amendments) | Massive extension — 9 documents listed with detailed amendment scopes |
| Part 8 (risk register) | Extension — R-K10-6, R-K10-7, R-K10-8, R-K10-9 |
| Part 11 (closing notes) | Replace existing с updated framing (К-L14 evidence state, К-L19 hardware tier, TLA+ scope, pipeline architecture post-К10) |

### 4.3 Application approach

**Approach choice**: Read amendment document fully, then read target document fully, then apply changes section-by-section.

For each amendment part, follow this pattern:

1. Locate corresponding section в target document (Part X)
2. Read existing content в that section
3. Apply amendment specification (insert/replace/restructure as specified)
4. Verify result is coherent с surrounding content

**Lesson #22 application**: Amendment specifies content; existing document specifies structure. If amendment says «replace existing Predictions section с new structure», read existing Predictions section first к identify exact boundaries, then replace.

**Lesson #8 application**: Document amendment is single atomic commit (entire document edited together). Intermediate states are not relevant — document is one logical unit. Verify final state is well-formed (no broken cross-references, headings hierarchical, version updated).

### 4.4 Specific Part-by-Part application

#### 4.4.1 Part 0 — closing notes amendment

**Action**: Append text. After existing Part 0 closing notes, append paragraph from amendment doc «Part 0 — closing notes amendment» section.

#### 4.4.2 Part 2.4 — К-L invariant table

**Action**: Major extension.

1. Update К-L6 entry к mark SUPERSEDED (existing entry probably says «LOCKED» — replace с «**~~К-L6~~** | **SUPERSEDED (К10)** | ...»)
2. Add К-L7.1 sub-invariant entry (between К-L7 и К-L8)
3. Add К-L12, К-L13, К-L14 entries (these may exist в v1.0 — verify; если present, keep; если absent, add)
4. Add К-L15, К-L16, К-L17, К-L18, К-L19 entries
5. Insert К-L6 SUPERSEDED rationale note immediately after К-L6 entry
6. Insert К-L7.1 sub-invariant full text after К-L7 entry (similar к К-L3.1 pattern)
7. Insert К-L15, К-L16, К-L17, К-L18, К-L19 full texts after table

**Read amendment doc «Part 2.4 — К-L invariant table amendment» section for exact text of all 5 new invariant statements + К-L7.1 sub + К-L6 SUPERSEDED note.**

#### 4.4.3 Part 3 — К10 scope (Items 26-46)

**Action**: Major extension. Add 21 new items grouped by S lock origin.

S1 group (Items 26-30):
- Item 26 — Native bus implementation (three-tier dispatcher)
- Item 27 — Managed bus facade + C ABI bridge
- Item 28 — Event type registry (tier-annotated)
- Item 29 — Subscriber contract enforcement (per-tier validation)
- Item 30 — Background work queue + idle-slot scheduling

S2 group (Item 17/18/19 amendments):
- Item 17 amended — Hybrid filter primitive
- Item 18 extended — TLA+ spec includes filter consistency invariants
- Item 19 extended — Filter hit/miss ratio metric

S3 group (Items 21 amend + 31, 32):
- Item 21 amended — Per-mod sub-scheduler teardown encapsulated
- Item 31 — Background queue save-integrated storage
- Item 32 — Native unload primitive + ModUnloadResult

S8-Q1 group (Items 33-35):
- Item 33 — Tick pipeline depth mechanism
- Item 34 — Pipeline drain/refill protocols
- Item 35 — Phase.Compute scheduler integration

S8-Q2 group (Items 33 extend, 36, 37):
- Item 33 extended — Pipeline slot data model includes FieldStorageSnapshot
- Item 36 — Pipeline slot read API
- Item 37 — Filter primitive integration с pipeline slot transitions

S8-Q3 group (Items 38-40):
- Item 38 — Display composition framework
- Item 39 — Intent overlay layer infrastructure
- Item 40 — Combat feedback layer infrastructure

S8-Q4 group (Items 41, 42):
- Item 41 — К-L18 quiescent state enforcement
- Item 42 — Settings menu / mod management UI integration с К10

S8-Q5 group (Items 43, 44):
- Item 43 — Async compute queue mandate
- Item 44 — Hardware capability check at startup

S-TLA group (Item 18 amend + 45, 46):
- Item 18 amended — TLA+ specification authoring covers 12 invariants
- Item 45 — Safety verification CI gate
- Item 46 — Liveness verification targeted

S5, S6 groups: zero К10 items.

**Read amendment doc «Part 3 — К10 scope amendments (Items 26-46)» section for full item descriptions.**

#### 4.4.4 Part 4 — К10 deliverables extension

**Action**: Append к existing К10 deliverables list. Three categories:

- Architectural artifacts (TLA+ specifications для 12 К-L invariants + CI integration + liveness scripts)
- Verification artifacts (safety verification CI test results + liveness verification preliminary results)
- Documentation artifacts (К-closure report contributions + cross-document amendment landings)

**Read amendment doc «Part 4 — К10 deliverables extension» section for full list.**

#### 4.4.5 Part 5 — performance predictions restructure

**Action**: Replace existing «Predictions» section с new structure.

§5.1.A — К10 architecture realization predictions (18 predictions, full text from amendment doc «§5.1.A» subsection)
§5.1.B — К11+ performance realization predictions (5 predictions K11-1 through K11-5, full text from amendment doc «§5.1.B» subsection)

**Read amendment doc «Part 5 — Performance predictions restructure» section for full prediction text.**

#### 4.4.6 Part 6 — Q-N surface extensions

**Action**: Append к existing Q-N list. Q-N-31 through Q-N-49+ + resolution notes.

Groups:
- From S1 lock (Q-N-31 through Q-N-37)
- From S2 lock (Q-N-38 through Q-N-42)
- From S3 lock (Q-N-43 through Q-N-49)
- From S8 sub-deliberation (Q-N-50 through Q-N-56)
- Q-N-22 status update (resolved by S-TLA)

**Read amendment doc «Part 6 — Q-N surface extensions» section for full Q-N descriptions.**

#### 4.4.7 Part 7 — Cross-document amendments massive extension

**Action**: Major extension. Replace или extend existing cross-document amendments list с 9-document queue.

Documents listed (each с detailed amendment scope):
- KERNEL_ARCHITECTURE.md
- VULKAN_SUBSTRATE.md
- MOD_OS_ARCHITECTURE.md
- DualFrontier.Persistence (project)
- KernelCapabilityRegistry.cs (source)
- METHODOLOGY.md (already addressed в this brief)
- PHASE_A_PRIME_SEQUENCING.md
- README.md
- К-closure report (А'.8) — pending creation

**Read amendment doc «Part 7 — Cross-document amendments massive extension» section for full scope per document.**

**Note**: METHODOLOGY.md entry is addressed by this brief. Other 8 entries are future work (separate briefs or sessions).

#### 4.4.8 Part 8 — Risk register extensions

**Action**: Append к existing risk register. Four new entries:

- R-K10-6 — Background queue save compatibility versioning
- R-K10-7 — Filter consistency races
- R-K10-8 — Hardware tier exclusion
- R-K10-9 — TLA+ state space explosion

**Read amendment doc «Part 8 — Risk register extensions» section for full entries.**

#### 4.4.9 Part 11 — Closing notes amendment

**Action**: Replace existing closing notes с updated framing. Major substantive change.

New closing notes cover:
- К10 architectural deliberation completion summary
- К-L14 evidence state per S4 lock
- К-L19 hardware tier commitment per S8-Q5 lock
- TLA+ specification authoring per S-TLA lock
- Pipeline architecture summary (data plane, control plane, event routing, tick lifecycle, display composition, mod lifecycle, hardware tier)
- Tier 1 LOCKED candidate status
- Reading order для К10 implementation brief authoring (6 documents)
- «Без костылей.» closing principle

**Read amendment doc «Part 11 — Closing notes amendment» section for full closing notes text.**

### 4.5 Version header update

KERNEL_FULL_NATIVE_SCHEDULER.md top frontmatter (если present):

```yaml
---
version: "1.0"
---
```

Update к:

```yaml
---
version: "2.0"
---
```

Add new version entry к document prose top (если version history pattern present):

> **Version: 2.0 (2026-05-17, this version)** — K10 architectural deliberation complete (all 9 S surfaces ratified). Major amendment per S deliberation arc: К-Lxx invariant series extended 11 → 20 (К-L6 SUPERSEDED + К-L7.1 sub + К-L12-К-L19); К10 scope extended 25 → 46 items; performance predictions restructured §5.1.A К10 + §5.1.B К11+; Q-N surface extended Q-N-1 through Q-N-49+; cross-document amendment list 9 documents; risk register extended с R-K10-6 through R-K10-9; Tier 1 LOCKED status (post-amendment).

> **Version: 1.0 (2026-05-16)** — Initial К10 specification authored.

### 4.6 Atomic commit 2

```
git add docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md
git commit -m "docs(architecture): KERNEL_FULL_NATIVE_SCHEDULER v1.0 → v2.0 — К10 deliberation outcomes

Major amendment per К10 deliberation arc 2026-05-16..2026-05-17. All 9 S surfaces ratified (К9.5 cancel + S1 + S2 + S3 + S4 + S8-Q1..Q5 + S-TLA + S5 + S6).

Changes:
- Part 0: closing notes updated с К10 deliberation completion
- Part 2.4: К-L invariant table extended 11 → 20 (К-L6 SUPERSEDED + К-L7.1 sub + К-L12 through К-L19)
- Part 3: К10 scope extended 25 → 46 items (Items 26-46 added)
- Part 4: deliverables extended с TLA+ specs + verification + documentation artifacts
- Part 5: performance predictions restructured §5.1.A К10 (18) + §5.1.B К11+ (5)
- Part 6: Q-N surface extended Q-N-31 through Q-N-49+ + resolutions noted
- Part 7: cross-document amendments queue 9 documents
- Part 8: risk register extended R-K10-6 through R-K10-9
- Part 11: closing notes updated с post-К10 pipeline architecture framing

Tier 1 LOCKED status candidate. Sister document K10_DELIBERATION_STATE.md (Project file) carries deliberation state.

Source: К10 deliberation arc 2026-05-16..2026-05-17 (Crystalka + Opus 4.7); see KERNEL_FULL_NATIVE_SCHEDULER_AMENDMENT.md for amendment specification.

Refs: DOC-A-KERNEL_FULL_NATIVE_SCHEDULER (new enrollment Phase 4), EVT-2026-05-17-K10-AMENDMENTS-APPLICATION"
```

### 4.7 Phase 3 outcome

1 atomic commit. KERNEL_FULL_NATIVE_SCHEDULER.md v1.0 → v2.0 with all S deliberation outcomes applied. Proceed к Phase 4.

---

## 5. Phase 4 — REGISTER.yaml enrollment + governance state update

### 5.1 Scope

Two changes к `docs/governance/REGISTER.yaml`:

1. Enroll new entry DOC-A-KERNEL_FULL_NATIVE_SCHEDULER (Tier 1 Architecture, LOCKED)
2. Update DOC-B-METHODOLOGY entry (version 1.7 → 1.8, last_modified, last_modified_commit)

### 5.2 New entry DOC-A-KERNEL_FULL_NATIVE_SCHEDULER

Add к Tier 1 Architecture (Category A) section of REGISTER.yaml. Location: after DOC-A-VULKAN_SUBSTRATE entry (alphabetical-ish ordering, or after DOC-A-FIELDS — verify against existing pattern).

Entry text:

```yaml
  - id: DOC-A-KERNEL_FULL_NATIVE_SCHEDULER
    path: docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md
    title: "К10 Native Kernel Scheduler — Architecture Specification"
    category: A
    tier: 1
    lifecycle: LOCKED
    owner: Crystalka
    version: "2.0"
    last_modified: "2026-05-17"
    last_modified_commit: "PENDING-COMMIT-2"  # filled in Phase 5
    content_language: mixed
    review_cadence: on-change+annual
    last_review_date: "2026-05-17"
    last_review_event: "К10 deliberation arc closure 2026-05-17 — all 9 S surfaces ratified"
    next_review_due: "2027-05-17"
    reviewer: Crystalka
    requirements_authored: []  # К-L12 through К-L19 not yet as REQ entries; defer к К-closure report
    risks_referenced: [RISK-002, RISK-003, RISK-004, RISK-013]
    special_case_rationale: "К10 specification document. Sister к K10_DELIBERATION_STATE.md (Project file, not register-tracked). Major amendment landed 2026-05-17 (v1.0 → v2.0) per К10 deliberation arc — 9 S surfaces ratified, 8 new К-L invariants + 2 sub-invariants, 46 items, TLA+ scope. Tier 1 LOCKED candidate promoted к LOCKED at this enrollment."
```

**Lesson #22 reminder**: Read existing Tier 1 entries to match indentation, field ordering, и any project-specific conventions. Amendment specifies fields; existing entries specify schema.

### 5.3 DOC-B-METHODOLOGY update

Locate existing entry:

```yaml
  - id: DOC-B-METHODOLOGY
    path: docs/methodology/METHODOLOGY.md
    [...]
    version: "1.7"
    last_modified: "2026-05-12"
    last_modified_commit: "34374f3"
    [...]
```

Update fields:

```yaml
    version: "1.8"
    last_modified: "2026-05-17"
    last_modified_commit: "PENDING-COMMIT-1"  # filled in Phase 5
    last_review_date: "2026-05-17"
    last_review_event: "К10 deliberation S6 lock — Lessons #11, #20, #22 + Provisional Lessons section"
    next_review_due: "2027-05-17"
```

### 5.4 register_version bump

Locate REGISTER.yaml schema metadata top:

```yaml
schema_version: "1.0"
register_version: "1.4"
last_modified: "2026-05-16"
last_modified_commit: "PENDING-INITIAL"
last_modified_by: "Claude Code"
```

Update:

```yaml
schema_version: "1.0"
register_version: "1.5"
last_modified: "2026-05-17"
last_modified_commit: "PENDING-COMMIT-3"  # filled in Phase 5 itself
last_modified_by: "Claude Code"
```

### 5.5 Atomic commit 3

```
git add docs/governance/REGISTER.yaml
git commit -m "governance(register): enroll DOC-A-KERNEL_FULL_NATIVE_SCHEDULER + bump DOC-B-METHODOLOGY к v1.8

REGISTER.yaml register_version 1.4 → 1.5 per К10 amendment landing.

Changes:
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER (NEW): Tier 1 Architecture, LOCKED, v2.0 — К10 specification document, sister к K10_DELIBERATION_STATE.md Project file
- DOC-B-METHODOLOGY (UPDATE): version 1.7 → 1.8, last_modified 2026-05-12 → 2026-05-17

PENDING-COMMIT placeholders filled in Phase 5 closure commit per FRAMEWORK §8.3 backfilling protocol.

Refs: EVT-2026-05-17-K10-AMENDMENTS-APPLICATION (added Phase 5)"
```

### 5.6 Phase 4 outcome

1 atomic commit. REGISTER.yaml updated с new entry + DOC-B-METHODOLOGY version bump + register_version 1.4 → 1.5.

---

## 6. Phase 5 — Closure protocol

### 6.1 Scope

Final closure operations:

1. Run sync_register.ps1 --validate (must exit 0)
2. Run render_register.ps1 (regenerate REGISTER_RENDER.md)
3. Backfill PENDING-COMMIT hashes (FRAMEWORK §8.3)
4. Add EVT-2026-05-17-K10-AMENDMENTS-APPLICATION audit trail entry
5. Final atomic commit с closure

### 6.2 Validation run

```powershell
pwsh tools/governance/sync_register.ps1 -Validate
```

Expected exit code: 0 (pass).

**If non-zero exit code**: Halt condition HG-2 (validation failure). Surface к Crystalka with full validation output. Common failure modes:
- Missing path: target file doesn't exist (verify path in REGISTER.yaml entry)
- Orphan file: file exists on disk но not in REGISTER.yaml (К10 document is now registered, no orphan expected)
- Frontmatter mismatch: target document frontmatter doesn't match REGISTER.yaml (verify version fields)
- Schema violation: YAML field invalid (verify entry syntax)

**Lesson #22 reminder**: read validation output fully. Don't infer cause from partial output.

### 6.3 Render regeneration

```powershell
pwsh tools/governance/render_register.ps1
```

This regenerates `docs/governance/REGISTER_RENDER.md` from REGISTER.yaml. Expected: REGISTER_RENDER.md now includes DOC-A-KERNEL_FULL_NATIVE_SCHEDULER entry в Tier 1 Architecture section.

### 6.4 PENDING-COMMIT backfill (FRAMEWORK §8.3)

After Phase 2, 3, 4 commits, capture hashes:

```bash
COMMIT_METHODOLOGY=$(git log -n 1 --format="%H" -- docs/methodology/METHODOLOGY.md)
COMMIT_KERNEL=$(git log -n 1 --format="%H" -- docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md)
COMMIT_REGISTER=$(git log -n 1 --format="%H" -- docs/governance/REGISTER.yaml)
```

Update REGISTER.yaml:

- DOC-B-METHODOLOGY.last_modified_commit: replace «PENDING-COMMIT-1» с actual hash
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER.last_modified_commit: replace «PENDING-COMMIT-2» с actual hash
- Schema metadata last_modified_commit: replace «PENDING-COMMIT-3» с this Phase 5 closure commit hash (will be captured at commit time)

**Note**: PENDING-COMMIT-3 will be the Phase 5 closure commit itself; impossible к know hash before commit. Two-step pattern: commit Phase 5 changes с PENDING-COMMIT-3 placeholder, then amend or follow-up commit с actual hash. **OR** use Option B from K8.x precedent: Phase 5 closure commit doesn't update its own hash; Crystalka or follow-up sweep updates последний reference (deferred). For this brief, **use Option B** — leave schema metadata `last_modified_commit` as PENDING-COMMIT-3 for backfill в next governance commit.

### 6.5 EVT-2026-05-17 audit trail entry

Add к REGISTER.yaml `audit_trail` collection:

```yaml
  - id: EVT-2026-05-17-K10-AMENDMENTS-APPLICATION
    date: "2026-05-17"
    event: "К10 deliberation amendments application — METHODOLOGY v1.8 + KERNEL_FULL_NATIVE_SCHEDULER v2.0 landed; new Tier 1 LOCKED enrollment"
    event_type: amendment_landing
    documents_affected:
      - DOC-B-METHODOLOGY              # v1.7 → v1.8
      - DOC-A-KERNEL_FULL_NATIVE_SCHEDULER  # NEW Tier 1 LOCKED enrollment, v2.0
      - DOC-G-REGISTER                 # register_version 1.4 → 1.5
      - DOC-G-REGISTER_RENDER          # regenerated
      - DOC-G-VALIDATION_REPORT        # regenerated by --validate run
    commits:
      range: "PENDING-COMMIT-1..PENDING-COMMIT-5"
      key_commits:
        - hash: "PENDING-COMMIT-1"
          summary: "METHODOLOGY v1.7 → v1.8 — Lessons #11/#20/#22 + Provisional Lessons section"
        - hash: "PENDING-COMMIT-2"
          summary: "KERNEL_FULL_NATIVE_SCHEDULER v1.0 → v2.0 — К10 deliberation outcomes applied"
        - hash: "PENDING-COMMIT-3"
          summary: "REGISTER enrollment DOC-A-KERNEL_FULL_NATIVE_SCHEDULER + DOC-B-METHODOLOGY bump + register_version 1.4 → 1.5"
        - hash: "PENDING-COMMIT-4"
          summary: "REGISTER_RENDER regeneration"
        - hash: "PENDING-COMMIT-5"
          summary: "Phase 5 closure — PENDING-COMMIT backfill + EVT-2026-05-17 added + register validation report regeneration"
    governance_impact: |
      К10 deliberation arc 2026-05-16..2026-05-17 closure landed. All 9 S surfaces ratified
      (К9.5 cancellation + S1 bus three-tier + S2 filter+commit + S3 mod lifecycle + S4 К10/К11+
      split + S8-Q1 pipeline depth + S8-Q2 slot tail read + S8-Q3 multi-layer composition +
      S8-Q4 quiescent state + S8-Q5 hardware tier + S-TLA spec authoring + S5 sequencing
      preservation + S6 lesson formalization).

      К-Lxx invariant series extended 11 → 20: К-L6 SUPERSEDED, К-L7.1 sub-invariant added,
      К-L12 through К-L19 authored (full native kernel scheduling, on-demand activation,
      performance from architecture, native bus three-tier, simulation tick pipeline depth,
      display composition multi-layer, mod lifecycle quiescent state, hardware tier commitment).

      К10 specification scope 25 → 46 items (Items 26-46 across S1/S2/S3/S8-Q1/Q2/Q3/Q4/Q5/S-TLA
      groups). Performance predictions restructured §5.1.A К10 closure gate (18 predictions) +
      §5.1.B К11+ deferred (5 predictions). Q-N surface extended Q-N-1 through Q-N-49+. Risk
      register extended с R-K10-6 through R-K10-9.

      METHODOLOGY: 3 full Lessons formalized (#11 architectural reduction methodology, #20
      tactical heuristics в research framework, #22 read existing code + ask operational
      context). NEW Provisional Lessons section с 9 candidates pending promotion at А'.8
      К-closure report timing.

      Future cross-document amendments queue (separate briefs): KERNEL_ARCHITECTURE.md,
      VULKAN_SUBSTRATE.md (substantial), MOD_OS_ARCHITECTURE.md, DualFrontier.Persistence,
      KernelCapabilityRegistry.cs, PHASE_A_PRIME_SEQUENCING.md wording cleanup, README.md
      hardware requirements section. К-closure report (А'.8) creation pending.

      Phase A' sequencing preserved (per S5 lock): А'.6 К8.5 → А'.7 К10 → А'.8 K-closure
      report → А'.9 Roslyn analyzer → Phase B M-series. К9.5 NOT inserted.

      К10 specification ready for execution brief authoring (separate session).
    cross_references:
      capa_entries: []  # no CAPAs opened или closed at this event
      risks:
        - RISK-004  # cross-document drift mitigation extended к К10 invariant series
```

### 6.6 Atomic commit 4 (Phase 5 closure)

```
git add docs/governance/REGISTER.yaml docs/governance/REGISTER_RENDER.md docs/governance/VALIDATION_REPORT.md
git commit -m "governance(closure): К10 amendments application closure — EVT + PENDING-COMMIT backfill + render regen

К10 deliberation amendments application closure per FRAMEWORK §6 post-session protocol.

Changes:
- REGISTER.yaml: EVT-2026-05-17-K10-AMENDMENTS-APPLICATION audit trail entry added
- REGISTER.yaml: PENDING-COMMIT-1, PENDING-COMMIT-2 backfilled с actual hashes (FRAMEWORK §8.3)
- REGISTER_RENDER.md: regenerated с new DOC-A-KERNEL_FULL_NATIVE_SCHEDULER entry
- VALIDATION_REPORT.md: regenerated post sync_register.ps1 --validate (exit 0)

К10 architectural deliberation arc 2026-05-16..2026-05-17 fully landed:
- 9 S surfaces ratified
- 8 new К-L invariants + 2 sub-invariants (К-L series 11 → 20)
- К10 scope 25 → 46 items
- 3 Lessons formalized + 9 provisional Lessons captured
- Tier 1 LOCKED specification document enrolled

Next operational phase: К10 execution brief authoring (separate session); А'.7 К10 implementation execution; cross-document amendment cascade (8 documents pending).

Refs: EVT-2026-05-17-K10-AMENDMENTS-APPLICATION"
```

### 6.7 Phase 5 outcome

1-2 atomic commits (depending on backfill split decision):

- 1 commit if Phase 5 single-commit pattern (REGISTER amend + render + validation report all together)
- 2 commits if backfill split (one for backfill + EVT, another for render + validation report regeneration)

Recommend single-commit pattern per Lesson #8 atomic-as-compilable. После this commit, всё К10 amendment work landed.

---

## 7. Phase 6 — Push к origin

### 7.1 Scope

Push branch `claude/k10-amendments-application` к origin. Pull request (или fast-forward к main if appropriate per repository convention).

### 7.2 Push command

```bash
git push -u origin claude/k10-amendments-application
```

**Lesson reminder (memory operational note)**: Claude Code auto-mode classifier blocks push-to-main even с explicit instruction в initial prompt. Push к feature branch is acceptable; merge к main requires Crystalka.

### 7.3 Phase 6 outcome

Branch pushed к origin. Crystalka reviews и merges (or fast-forwards) per repository convention.

---

## 8. Halt conditions

Per METHODOLOGY §3 stop-escalate-lock. Halt protocol: stop execution, surface к Crystalka via halt artifact, await direction.

### 8.1 Hard gates (HG-N)

**HG-1**: Working tree dirty before Phase 0. Surface к Crystalka; resolve workspace before proceeding.

**HG-2**: sync_register.ps1 --validate exit non-zero in Phase 5. Surface к Crystalka с full validation output. Common causes: schema violation, missing path, orphan file, frontmatter mismatch.

**HG-3**: Recent commit log shows governance state inconsistent с this brief assumptions (e.g., А'.6 К8.5 closed but not reflected; new EVT entries not anticipated). Surface к Crystalka.

### 8.2 Soft conditions (SC-N)

**SC-1**: Phase 0 reads surface state inconsistency between recent commits и this brief assumptions. Examples: К10_DELIBERATION_STATE.md found в repository (Phase 1 confusion), METHODOLOGY.md already at v1.8 (already amended), KERNEL_FULL_NATIVE_SCHEDULER.md already at v2.0. Surface к Crystalka.

**SC-2**: Scope mismatch — К10_DELIBERATION_STATE.md found как repository file (not Project file). Surface к Crystalka, не commit. Crystalka decides if it should be moved к Project files or treated as repository document.

**SC-3**: Amendment specifies API surface (path, signature) and verification на disk reveals mismatch. Lesson #7 application: don't paraphrase, halt и surface. Verify file paths against actual disk state during Phase 0 reads; if amendment specifies path that doesn't exist or version that's stale, halt.

**SC-4**: Lesson #8 violation discovered during Phase 3 — amendment splits into N steps where intermediate state не valid. For this brief, Phase 3 = single atomic commit, so this shouldn't fire. If amendment text suggests multi-commit split, halt и surface.

**SC-5**: Cross-document amendment opportunity surfaces (e.g., need к update KERNEL_ARCHITECTURE.md К-L table now). Out of scope for this brief; surface к Crystalka, не auto-extend scope.

### 8.3 Halt artifact pattern

If halt condition fires, create:

```
docs/scratch/K10_AMENDMENTS_APPLICATION/HALT_REPORT.md
```

Content: halt reason, condition fired (HG-N or SC-N), investigation findings, resolution options, recommended action.

Commit halt artifact, then await Crystalka direction.

---

## 9. Closure summary

### 9.1 Expected commit count

5 atomic commits (one possible split brings к 6):

1. METHODOLOGY.md v1.7 → v1.8
2. KERNEL_FULL_NATIVE_SCHEDULER.md v1.0 → v2.0
3. REGISTER.yaml enrollment + version bumps
4. (Phase 5) closure commit с EVT + backfill + render regen

If Phase 5 split for backfill timing: 5 commits. Single-commit Phase 5: 4 commits после Phase 4 (= 4 amend commits + closure ≈ 4 commits total если closure is part of REGISTER commit).

Recommend: **4 commits total** = METHODOLOGY + KERNEL_FULL_NATIVE_SCHEDULER + REGISTER (with EVT inline) + closure (render + validation report).

### 9.2 Expected document state at closure

- `docs/methodology/METHODOLOGY.md` — v1.8, contains Lessons #11/#20/#22 + Provisional Lessons section
- `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` — v2.0, all 9 S surfaces reflected, Tier 1 LOCKED, 46 items, 20 К-L invariants (К-L1..L19 + К-L3.1 + К-L7.1 sub, К-L6 SUPERSEDED)
- `docs/governance/REGISTER.yaml` — register_version 1.5, new DOC-A-KERNEL_FULL_NATIVE_SCHEDULER entry, DOC-B-METHODOLOGY v1.8 update, EVT-2026-05-17-K10-AMENDMENTS-APPLICATION audit trail entry
- `docs/governance/REGISTER_RENDER.md` — regenerated
- `docs/governance/VALIDATION_REPORT.md` — regenerated, all checks pass

### 9.3 Next operational phase

К10 specification ready для execution brief authoring (separate session). Expected:

1. К10 execution brief authoring (separate Opus deliberation session) — translates 46-item К10 spec к Claude Code execution material. Scope ~1500-2500 lines. Includes Phase 0 reads, atomic commit sequencing, halt conditions, closure protocol.
2. А'.7 К10 implementation execution (Claude Code session) — implements К10 per execution brief.
3. Cross-document amendment cascade (8 documents pending) — separate briefs or discrete sessions.
4. А'.8 К-closure report authoring (substantial scope ~3-6 weeks).
5. А'.9 Roslyn analyzer milestone.
6. Phase B M-series.

«Без костылей.» К10 deliberation arc complete; specification ready для execution.

---

## Appendix A — Reference: Three attached amendment documents

For executor reference, three amendment documents attached к this session:

### A.1 K10_DELIBERATION_STATE.md (Project file replacement)

- Authored: 2026-05-17
- Size: 616 lines, 36 KB
- Status: Complete — all 9 S surfaces locked
- Target: Crystalka's Project files (overwrites pre-S8 version)
- Sister documents: KERNEL_FULL_NATIVE_SCHEDULER.md (architectural spec, repository) + COMPOSITE_NAMESPACE_DELIBERATION_STATE.md (predecessor deliberation state, repository) + DUAL_FRONTIER_CHRONOLOGY.md + SESSION_LOG_2026_05_17_K10_DELIBERATION.md

### A.2 METHODOLOGY_AMENDMENT_S6.md (METHODOLOGY.md patch)

- Authored: 2026-05-17
- Size: 217 lines, 16 KB
- Target: `docs/methodology/METHODOLOGY.md` v1.7 → v1.8
- Insertion point: After «Lesson #8» section, before «Reference: K0 lessons learned» section
- Adds: Lesson #11 (architectural reduction methodology), Lesson #20 (tactical heuristics category error), Lesson #22 (read existing code + operational context), Provisional Lessons section с 9 candidates
- Version increment: v1.7 → v1.8

### A.3 KERNEL_FULL_NATIVE_SCHEDULER_AMENDMENT.md (KERNEL_FULL_NATIVE_SCHEDULER.md patch)

- Authored: 2026-05-17
- Size: 477 lines, 32 KB
- Target: `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v1.0 → v2.0
- Scope: 9 parts amended (Part 0, 2.4, 3, 4, 5, 6, 7, 8, 11)
- Adds: К-L7.1 sub-invariant, К-L15-19 invariants, К-L6 SUPERSEDED note, Items 26-46 (21 new items), §5.1.A/§5.1.B prediction split, Q-N-31..49+ + resolutions, cross-document amendment queue (9 docs), R-K10-6..9 risk entries
- Version increment: v1.0 → v2.0
- Lifecycle promotion: Tier 1 LOCKED candidate → Tier 1 LOCKED

---

## Appendix B — К10 deliberation arc lock summary (informational)

For executor context, locks ratified в К10 deliberation arc:

| Lock | Date | Decision summary |
|---|---|---|
| К9.5 CANCELLED | 2026-05-17 | Analyzer first run = audit tool, no separate phase |
| S1 | 2026-05-17 | Bus native + three-tier dispatch (Fast/Normal/Background) + К-L15 |
| S2 | 2026-05-17 | Filter+check+commit hook + Q3b hybrid filter (Level 1 bitset + Level 2 sparse) |
| S3 | 2026-05-17 | L3 primitive + per-tier policy + save-integrated + T2 per-FQN per-tier capabilities + single primitive ModUnloadResult |
| S4 | 2026-05-17 | Option B split §5.1.A К10 / §5.1.B К11+ + Prediction 4 reframed |
| S8-Q1 | 2026-05-17 | 7 sub-locks + К-L16 pipeline depth D=2 default (configurable 1-3); Crystalka surfaced |
| S8-Q2 | 2026-05-17 | Pattern C explicit pipeline slot tail read + К-L7.1 sub-invariant |
| S8-Q3 | 2026-05-17 | К-L17 multi-layer composition (sim state/intent/combat layers); Crystalka surfaced |
| S8-Q4 | 2026-05-17 | К-L18 quiescent state + Pattern (b) delegate; Crystalka surfaced |
| S8-Q5 | 2026-05-17 | К-L19 hardware tier + Pattern (a) async compute mandate; Crystalka surfaced |
| S-TLA | 2026-05-17 | (e) spec authoring в К10 deliverable + (c) targeted: safety verification CI all 12 invariants + liveness targeted К-L7.1/К-L12/К-L16 |
| S5 | 2026-05-17 | Status quo: А'.8 K-closure, А'.9 Roslyn analyzer; «А'.10 if А'.8 substantial» removed |
| S6 | 2026-05-17 | Hybrid lesson formalization: #11/#20/#22 immediate full + 9 provisional pool |

13 locks total ratified в the arc.

---

## Appendix C — Operational reminders

### C.1 Filesystem MCP create_file silent failure

Creator file via Filesystem MCP can silently fail (no error, file does not appear on disk). Confirmed 2026-05-15 с ARCHITECTURE_RECON_BRIEF.md first attempt.

For Claude Code session, prefer `git mv` / direct file edits / `cat > file <<EOF` patterns. If using Filesystem MCP create_file, verify file existence immediately after operation.

### C.2 testhost.exe file lock

Не relevant к this brief (documentation amendments, no tests run). Note for future code-touching milestones.

### C.3 PS 5.1 array/Hashtable gotcha

`Where-Object` on scalar returns Hashtable, not array. Force-array с `@(...)`. Relevant if Phase 5 closure tooling has any such pattern; sync_register.ps1 / render_register.ps1 should handle this correctly already (tested at A'.4.5 closure).

### C.4 Lesson application discipline

During execution, apply Lessons #7, #8, #11, #20, #22 actively:

- **#7**: Verify API surface (paths, file structures) against actual disk state during Phase 0
- **#8**: Each commit produces well-formed state; document amendments are atomic per document
- **#11**: Architectural reduction not applicable к this brief (mechanical amendment application)
- **#20**: Tactical heuristic check — don't truncate amendment content «to be concise»; specifications are verbose by design
- **#22**: Read existing target documents before applying patches; insertion points specified, but local structure is authority

---

**End of Brief**

К10 deliberation amendments application execution brief. ~3-5 hours auto-mode. 4 atomic commits. Phase 0 deep reads mandatory. К10 specification ready для execution brief authoring after this lands.
