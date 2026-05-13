---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_4_5_REGISTER_BRIEF
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_4_5_REGISTER_BRIEF
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_4_5_REGISTER_BRIEF
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_4_5_REGISTER_BRIEF
---
# A'.4.5 — Document Control Register: Project-Synthesized Governance Framework

**Status**: AUTHORED 2026-05-10 — deliberation brief authoring; awaits Crystalka deliberation pass + lock + execution
**Brief type**: Architectural decision brief (fourth brief type — precedents: K8.0, K-L3.1, A'.0.7)
**Scope**: Authoring of project-specific governance framework (synthesized from DO-178C / ISO 9001 / ISO 26262 / IEC 61508 / FDA 21 CFR Part 11) + dual-format register schema (centralized YAML SoT + per-document frontmatter mirror via tooling) + full enrollment of all 179 .md files in repository
**Sequencing**: A'.4.5 — between A'.4 push closure and A'.5 K8.3
**Phase A' position**: 11th milestone in Phase A' sequence (after A'.0 / A'.0.5 / A'.0.7 / A'.1.M / A'.1.K / A'.3 / A'.4); precedes A'.5 K8.3 → A'.6 K8.4 → A'.7 K8.5 → A'.8 K-closure report → A'.9 architectural analyzer
**Deliberation session length estimate**: ~3-5 hours (deliberation Q1–QN locks + framework synthesis + schema design + enrollment plan); Phase 4 deliverable = amendment plan analogous to A'.0.7 amendment plan structure
**Execution session length estimate** (post-deliberation): ~12-20 hours auto-mode (framework authoring + schema + tooling + 179-file enrollment campaign + cross-validation + methodology integration)

---

## Executable contract notice

This brief is a **deliberation-mode** brief, not an execution-mode brief. Its Phase 1 Q-locks (analogous to A'.0.7 Q-A07-1..12) are resolved through Crystalka deliberation; the synthesis (§4) and amendment plan (§5) are the deliverable. Execution-mode brief for A'.4.5 landing is authored as the §5 amendment plan in a follow-up session, mirroring the A'.0.7 → A'.0.7 closure execution brief → A'.1.M landing pattern.

The deliberation brief assumes Crystalka's prior architectural commitments are stable foundations:
- «Сложная инженерия оправдана, если она даёт чистое архитектурное исполнение без костылей» (no-compromises commitment)
- «Open-source-separately property» (each native artifact, each governance framework, each subsystem must be valuable independently)
- «Десятилетия без костылей» (decade-scale planning horizon)
- Twin-framework synthesis precedent: K8.1 wrapper refactor (value-type structs synthesized from K-L11 + K8.1 primitives), K-L3.1 bridge formalization (Path α + Path β as first-class peers synthesized from K-L3 + Q1–Q6 deliberation), A'.0.7 abstract-primary + per-era-empirical (synthesized from Q-A07-1..12 with §4.A + §4.C split)

The framework synthesized here is intentionally **bespoke**:  не «derived from standard X», но «synthesized from standards X/Y/Z/W/V». This serves twin purposes — operational (governance fitness-for-purpose for solo gamedev with AI agent-mediated pipeline) + signaling (Phase A' marketing: «we synthesized governance from 5 industry standards into a unique framework rather than copying any one»).

---

## Phase 0 — Pre-flight reads

Before deliberation, the architect (this session) verifies five prerequisite states:

### 0.1 — Phase A' sequencing state

Read `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` to confirm A'.4.5 insertion point.

**Expected**: A'.4 push closure complete (HEAD on `origin/main` includes commits `d163341..80c9ba6` + housekeeping `25c2bfb5`); A'.5 K8.3 NOT STARTED; sequencing document anchors Phase A' structure but does not yet contain A'.4.5 entry — this brief proposes insertion.

### 0.2 — A'.0.5 inventory current count

Read `tools/scratch/A_05/INVENTORY.md` to confirm document count baseline.

**Expected**: ~179 total .md files classified across 8 categories (A=25, B=6, C=3, D=41, E=30, F=70, G=2, H=2). Count may have drifted slightly since A'.0.5 closure 2026-05-10 (+2 from K9 execution: K9_BRIEF_REFRESH_PATCH + K9 closure artifacts). Phase 0 of A'.4.5 execution does its own fresh count; ~179 is the planning baseline.

### 0.3 — Methodology state

Read `docs/methodology/METHODOLOGY.md` v1.6 §0–§4 + §10 + §11 to confirm the governance vocabulary register builds on:
- §2.1.1 Current pipeline configuration (Crystalka + Architect + Executor + boundary type = session-mode)
- §2.2 Contracts as IPC across context boundaries
- §3 Economics (architectural deliberation context-intensive low-frequency vs mechanical execution scope-bounded high-frequency)
- §7.1 «Data exists or it doesn't» principle
- §11 Cross-references (post-A'.0.5 paths)

**Expected**: methodology corpus at v1.6 era; no in-progress amendments competing for vocabulary.

### 0.4 — Existing governance artifacts inventory

Locate existing governance-shaped artifacts (already in the codebase, but not yet under a unified register):

- Status fields in every LOCKED spec ("AUTHORITATIVE LOCKED", "v1.X", "Date: YYYY-MM-DD")
- Change history sections (METHODOLOGY §10, KERNEL_ARCHITECTURE Part 4, MOD_OS_ARCHITECTURE version history)
- Brief Status fields ("AUTHORED", "EXECUTED", "DEPRECATED", "FULL EXECUTED")
- LIVE document patterns (MIGRATION_PROGRESS, ROADMAP, IDEAS_RESERVOIR)
- D-decisions log in MIGRATION_PROGRESS (D1..D5)
- OQ tracking in MIGRATION_PROGRESS (OQ1..OQ4)
- K-Lessons in METHODOLOGY (K-Lessons #1, #2)
- Lesson candidates accumulated for A'.8 K-closure report (5 items recorded in user memory)
- Phase A' sequencing document `PHASE_A_PRIME_SEQUENCING.md`

**Expected output**: confirmation that the register **formalizes existing practice**, not invents new bureaucracy. Each register section maps to an existing governance pattern already in use in the codebase.

### 0.5 — Lessons learned applicable to A'.4.5

Read:
- A'.0.5 closure entry «Lessons learned» (4 lessons — single-session pipeline collapses milestone splits; Stop #2 mechanical-vs-architectural boundary; cross-ref refresh script reusability; pre-staged work + Stop #1 protocol scales)
- A'.0.7 closure entry «Lessons learned» (5 lessons — audience contract surfaces as architectural decision; two-track synthesis cleaner than monolithic; era inversion observation in K-Lessons #2; Q-A07 cascade auxiliary locks; pipeline empirical validation)
- K9 execution closure «Lessons learned» (7 lessons — selftest style adaptation; byte* UTF-8 P/Invoke pattern; IFieldHandle moved to Contracts; capability regex extension; RegisterManagedComponent pre-flight; FieldRegistry plumbing deferred; CPU kernel write performance)
- Memory tracker «Phase A' execution operational lessons» (5 items: amendment plan completeness gap pattern; lesson-applier latency; construction-by-rationale robustness; testhost cleanup procedure; --logger verbosity verbosity gotcha)

**Expected**: these lessons inform A'.4.5 deliberation. Most relevant for register design:
- Two-track synthesis (A'.0.7 lesson) → register may need similar split: governance principles abstract vs per-document operational data per-era
- Construction-by-rationale (Phase A' lesson) → register's machine-validated cross-trace replaces per-commit retest for governance changes
- Amendment plan completeness gap (Phase A' lesson) → register's enrollment campaign explicitly enumerates every file or surgical scrubs surface at execution time
- Audience contract (A'.0.7 lock Q-A07-6) → register schema authored agent-primary; human-readable rendering derivative
- Bundle is default safe under session-mode (K-Lessons #2) → A'.4.5 unifies framework authoring + schema + tooling + enrollment + integration in single bundled milestone

---

## Phase 1 — Deliberation (Q-locks)

This phase resolves the architectural decisions that constitute the register. Each Q is presented to Crystalka with options + recommendation + rationale; the lock + rationale is recorded for the §4 synthesis.

Q1–Q3 are pre-session locks (resolved in this session's predecessor turn): A'.4.5 sequencing position (Q1=a), formality level (Q2=d project synthesis), ownership model (Q3=a human-named). Q4 onward surface in this session.

### Q1 — Sequencing position (LOCKED, pre-deliberation)

**Lock**: A'.4.5 — inserted between A'.4 push closure and A'.5 K8.3 in Phase A' sequence.

**Rationale**: Register's forward leverage is maximum when K8.3/K8.4/K8.5/K-closure/analyzer are conducted from within the governance framework. A'.10-positioning (after analyzer) loses this — register becomes archaeology rather than operational discipline. B'-positioning (parallel phase) violates METHODOLOGY v1.6 §4.4 case A «single architectural focus per period».

### Q2 — Formality level (LOCKED, pre-deliberation)

**Lock**: Project synthesis — synthesize best elements from DO-178C / ISO 9001 / ISO 26262 / IEC 61508 / FDA 21 CFR Part 11 into bespoke framework.

**Rationale**:
- Pure DO-178C: 60% structure addresses DAL A/B/C/D verification levels inappropriate for gamedev; would force-fit gamedev concerns into aerospace formalism (the kostyl pattern).
- Pure ISO 9001: too generic — register would read as «Quality Manual» rather than engineering-specific governance.
- Hybrid (ISO + DO-178C): better but loses elements from medical-device QSR (FMEA, CAPA) and functional-safety standards (ISO 26262 safety case, IEC 61508 SIL levels).
- **Project synthesis**: best operational fit + Twin marketing benefit («synthesized from 5 industry standards into a unique framework»).

### Q3 — Ownership model (LOCKED, pre-deliberation)

**Lock**: Human-named ownership (Crystalka for all in current state; future contributors get names).

**Rationale**: Honest. Role-based hides solo-developer reality; agent-typed records originating-context not accountability. Future contributor onboarding rewrites Owner field per-document; this is bounded migration cost, not structural cost.

### Q4 — Standards-element selection from 5 source standards

**Question**: Which specific elements from each source standard contribute to the synthesis?

**Options for each standard (analyst picks)**:

- **DO-178C (Software Considerations in Airborne Systems)**:
  - (a) Plan-level documents (PSAC, SDP, SVP, SCMP, SQAP)
  - (b) Configuration management (SCI — Software Configuration Index)
  - (c) Requirement-to-test traceability matrix
  - (d) Problem reports with root cause analysis
  - (e) Tool qualification levels (TQL1-5)

- **ISO 9001 (Quality Management Systems)**:
  - (a) Document Control (clause 7.5)
  - (b) Management Review (clause 9.3)
  - (c) Internal Audit (clause 9.2)
  - (d) Corrective Action (clause 10.2)
  - (e) Risk-based Thinking (clause 6.1)

- **ISO 26262 (Functional Safety — Automotive)**:
  - (a) Safety Case (decision rationale documentation)
  - (b) ASIL (Automotive Safety Integrity Level) decomposition
  - (c) Confirmation Reviews (independent verification)
  - (d) Safety Lifecycle phases
  - (e) Item Definition (system boundary documentation)

- **IEC 61508 (Functional Safety — General)**:
  - (a) SIL (Safety Integrity Level) classification
  - (b) Hazard and Risk Analysis (HARA)
  - (c) Verification & Validation planning
  - (d) Safety Manual (operating constraints documentation)
  - (e) Common Cause Failure Analysis

- **FDA 21 CFR Part 11 (Electronic Records, Electronic Signatures)**:
  - (a) Audit Trail (chronological record of changes)
  - (b) Electronic Signature (review approval with non-repudiation)
  - (c) Data Integrity (ALCOA+ principles)
  - (d) System Validation (validated changes)
  - (e) Record Retention (long-term archival)

**Recommendation**: 
- DO-178C: (b) Configuration management + (c) Traceability matrix + (d) Problem reports — three highest-impact transferable patterns
- ISO 9001: (a) Document Control + (c) Internal Audit + (d) Corrective Action — recognizable signaling + concrete process
- ISO 26262: (a) Safety Case (re-purposed as «Architecture Case» — decision-rationale documentation; precedent: K8.0 architectural decision brief shape, K-L3.1 brief)
- IEC 61508: (b) Hazard and Risk Analysis (re-purposed as «Architectural Risk Analysis» — risk register entries trace to specific architectural decisions; precedent: MIGRATION_PROGRESS Risk register §R1..R6)
- FDA 21 CFR Part 11: (a) Audit Trail — git log already provides this structurally; register inherits and references rather than duplicates

**Lock rationale (to confirm in deliberation)**: 14 elements selected, 11 elements de-selected. Selection criterion: «transferable to solo-gamedev + AI-agent pipeline + open-source-separately property». De-selected elements (PSAC formalism, ASIL/SIL classification, tool qualification levels, electronic signatures, validation lifecycle) require institutional ceremony not justified at this scale.

### Q5 — Register section structure

**Question**: How do the selected elements map to register sections?

**Option α (5-section)**: As Crystalka originally specified — owners / review-approval / requirement-test matrix / risk register / CAPA log. Maps cleanly to 5 source standards (one anchor per standard).

**Option β (synthesized)**: Custom section list driven by selection content:
1. **Document Control** (ISO 9001 anchor + DO-178C SCI) — ownership, status, version, location, classification
2. **Architecture Case** (ISO 26262 anchor) — decision rationale documentation; subsumes existing «D-decisions log» pattern from MIGRATION_PROGRESS
3. **Requirement → Test Traceability Matrix** (DO-178C anchor) — explicit cross-references from architectural requirements to verifying tests
4. **Architectural Risk Analysis** (IEC 61508 anchor) — risk register with likelihood × impact + mitigation status; subsumes MIGRATION_PROGRESS Risk register
5. **Internal Audit Schedule** (ISO 9001 anchor) — review cadence per Tier, last review date, next review due date, reviewer assignment
6. **Corrective Action Log** (ISO 9001 anchor) — CAPA entries with root cause + corrective action + verification of effectiveness; precedent: amendment plan landing pattern (A'.0.7 amendment plan as CAPA-shaped artifact)
7. **Audit Trail** (FDA 21 CFR Part 11 anchor) — references to git log for each change, with semantic-level annotation; precedent: MIGRATION_PROGRESS commit-hash references for every closure

**Option γ (Crystalka-specified + synthesis annotations)**: 5 sections per Q3 specification, with each section internally cross-linking to source standards.

**Recommendation**: **β-synthesized (7 sections)**. Argument: Crystalka's «5-section» specification was the desired minimum; the project synthesis lock (Q2=d) authorizes expansion to fully reflect 5-standard scope. 7 sections matches 14 selected elements (~2 elements/section average). Each section is internally cohesive (one concern per section); cross-cutting concerns visible through register-wide queries.

### Q6 — Document classification taxonomy

**Question**: What classification system tags each document?

The A'.0.5 INVENTORY used 8 categories (A–H): Architecture, Methodology, Live tracker, Brief, Discovery/closure/audit, Module-local, Project meta, i18n. This is descriptive but doesn't carry governance semantics (review cadence, change authority, applicable requirements).

**Option α** — Preserve A'.0.5 8-category taxonomy verbatim; register adds metadata fields on top.

**Option β** — Tier-based (Tier 1 LOCKED specs / Tier 2 LIVE docs / Tier 3 briefs and reports / Tier 4 module-local). Subsumes the 8 categories but loses category granularity.

**Option γ** — Dual axis: Category (A–H from A'.0.5) × Tier (1–4). Each document has both. Category describes content type; Tier describes governance regime. Crisper but doubles taxonomy complexity.

**Option δ** — Three-axis: Category × Tier × Lifecycle State (Draft/Live/LOCKED/Deprecated/Superseded). Adds state machine; closes brief Status field gap (current EXECUTED/AUTHORED/DEPRECATED is brief-specific, not unified across all documents).

**Recommendation**: **δ — Three-axis (Category × Tier × Lifecycle State)**. Argument: each axis answers a different governance question.
- Category = content type → which template, which writing voice, which audience
- Tier = governance regime → review cadence, change authority, applicable formal requirements
- Lifecycle state = current control state → can it be cited as authority? Is it actively maintained?

Three axes increase initial cost (enrollment campaign assigns three values per document) but yield far higher query power. The register becomes a queryable governance database, not just a table.

### Q7 — Tier definitions for the Tier axis

**Question**: How are the 4 tiers defined operationally?

**Recommendation**:

- **Tier 1 — Architectural authority** (LOCKED foundational): KERNEL/MOD_OS/RUNTIME/MIGRATION_PLAN/GPU_COMPUTE/FIELDS + methodology corpus (METHODOLOGY/PIPELINE_METRICS/MAXIMUM_ENGINEERING_REFACTOR/CODING_STANDARDS). Change authority = Crystalka via deliberation milestone + amendment plan + landing milestone. Review cadence = on every change + annual full review. Approval gate = explicit Crystalka lock in deliberation brief. ~10–14 documents.

- **Tier 2 — Operational live** (LIVE trackers): MIGRATION_PROGRESS/ROADMAP/IDEAS_RESERVOIR/PHASE_A_PRIME_SEQUENCING + key audit/closure reports. Change authority = Crystalka or executor session per milestone closure protocol. Review cadence = on every closure write + quarterly drift review. Approval gate = part of closure verification. ~5–10 documents.

- **Tier 3 — Milestone instruments** (briefs + execution artifacts): all `tools/briefs/*` + closure reports + amendment plans + verification logs. Change authority = depends on Status field (AUTHORED can be revised; EXECUTED is historical immutable; DEPRECATED is supersession-marked). Review cadence = on Status transition only. Approval gate = Status field updates + amendment-plan-shaped revisions for active briefs. ~50–60 documents.

- **Tier 4 — Module-local** (per-directory READMEs + MODULE.md): all `src/**/README.md`, `tests/**/README.md`, `mods/**/README.md`, `native/**/MODULE.md`. Change authority = updated as part of any commit touching the module's source. Review cadence = quarterly Phase X-led module-local sweep (precedent: A'.0.5 Phase 6 module-local refresh). Approval gate = grouped with source commit. ~70 documents.

Tier 4 may be sub-divided in future revisions (e.g. mods/ READMEs vs src/ READMEs vs tests/ READMEs as 4a/4b/4c) but starts as a single bucket; sub-division surfaces if review-cadence economics differ in practice.

### Q8 — Lifecycle state machine

**Question**: What states does the Lifecycle axis enumerate?

**Recommendation**:

- **Draft** — authored but not committed to a final form. May be revised freely.
- **Live** — actively maintained; mutable on every relevant milestone. (Tier 2 default state.)
- **LOCKED** — change authority via formal amendment milestone only. (Tier 1 default state.)
- **EXECUTED** — brief that has been run; historical immutable. Lessons learned section may be appended but contract section frozen. (Tier 3 brief state.)
- **AUTHORED** — brief authored, awaits execution. May be revised via patch brief (precedent: K9_BRIEF_REFRESH_PATCH).
- **DEPRECATED** — superseded by successor; retained for historical context. Cross-reference to successor mandatory.
- **SUPERSEDED** — replaced by a newer version of the same logical document (version supersession, not deprecation). Cross-reference to current version mandatory.
- **STALE** — known out-of-date; awaits update or formal archive. Surfaced via internal audit; not a steady state.

State transitions are documented in §6 (Corrective Action Log) when a document moves between states for non-trivial reasons (e.g. LOCKED → SUPERSEDED via amendment milestone; AUTHORED → EXECUTED via execution session; Live → STALE flagged at internal audit).

### Q9 — YAML schema design (centralized registry shape)

**Question**: What is the precise shape of the centralized YAML schema?

**Recommendation** — per-document entry shape:

```yaml
# docs/governance/REGISTER.yaml (centralized SoT)

documents:
  - id: DOC-A-001
    path: docs/architecture/KERNEL_ARCHITECTURE.md
    title: "DualFrontier Kernel — Architecture & Roadmap"
    
    # Three-axis classification (Q6 lock)
    category: A  # Architecture spec
    tier: 1  # Architectural authority
    lifecycle: LOCKED
    
    # Document Control section (§1)
    owner: Crystalka
    version: "1.5"
    last_modified: "2026-05-10"
    last_modified_commit: "2df5921"
    
    # Architecture Case section (§2) — decision-rationale documentation
    architecture_case:
      decisions:
        - K-L1: "Native language: C++20"
        - K-L2: "Bindings: Pure P/Invoke"
        - K-L3: "Component storage paths: Path α default + Path β opt-in"
        # ... K-L4..K-L11
      rationale_anchors:
        - tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md
        - tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md
    
    # Requirement → Test Traceability Matrix section (§3)
    requirements:
      - REQ-K-L7: "Read-only spans + write batching"
        verified_by:
          - tests/DualFrontier.Core.Interop.Tests/SpanLeaseTests.cs
          - tests/DualFrontier.Core.Interop.Tests/BulkOperationsTests.cs
        verification_status: VERIFIED
      - REQ-K-L11: "Single NativeWorld backbone"
        verified_by:
          - tests/DualFrontier.Core.Interop.Tests/NativeWorldTests.cs
          - native/DualFrontier.Core.Native/test/selftest.cpp # scenario_basic_crud
        verification_status: VERIFIED
      # ... per K-Lxx
    
    # Architectural Risk Analysis section (§4) — cross-reference to risk register entries
    risks_referenced:
      - RISK-R1: "Component struct refactor scope underestimated (Probability=Medium-High)"
      - RISK-R5: "Performance regression on weak hardware (Probability=Low, post-K7 evidence)"
      # ... per Risk register
    
    # Internal Audit Schedule section (§5)
    review_cadence: "on-change + annual"
    last_review_date: "2026-05-10"
    last_review_event: "K-L3.1 bridge formalization deliberation 2026-05-10"
    next_review_due: "2027-05-10"
    reviewer: Crystalka
    
    # Corrective Action Log section (§6) — references to CAPA entries
    capa_entries:
      - CAPA-2026-05-09-K8.2-V2-REFRAMING:
          trigger: "K-L3 «без exception» framing misalignment surfaced 2026-05-10"
          root_cause: "K8.2 v1 brief authored against pre-K-L3.1 framing"
          corrective_action: "K-L3.1 bridge formalization deliberation milestone + amendment plan + landing milestones"
          effectiveness_verified: "A'.1.K commit `0789bd4` 2026-05-10"
      # ... per CAPA log entry
    
    # Audit Trail section (§7) — references to git log
    audit_trail: "see git log --follow docs/architecture/KERNEL_ARCHITECTURE.md"
    
    # Cross-references to register-wide entries
    register_links:
      supersedes: []  # for SUPERSEDED state
      superseded_by: []  # for SUPERSEDED state
      deprecates: []  # for DEPRECATED state
      deprecated_by: []  # for DEPRECATED state
```

Plus separate sections for global entities not document-specific:

```yaml
# Risk register — global (cross-document risks)
risks:
  - id: RISK-R1
    title: "Component struct refactor scope underestimated"
    probability: Medium-High
    impact: Medium
    affected_documents:
      - DOC-A-001  # KERNEL_ARCHITECTURE
      - DOC-A-XXX  # K4 brief
    mitigation: "Incremental conversion (5-10 components per commit), tests verify each commit"
    mitigation_status: ACTIVE
    history:
      - 2026-05-07: "Risk surfaced at K0 brief authoring"
      - 2026-05-08: "Risk realized at K4 execution; mitigation worked (24 components × 7 batches)"
      - 2026-05-08: "Risk closure pending K8.2 final retirement of class-component state"
      - 2026-05-09: "Risk closed; K8.2 v2 verification 0789bd4"
  # ... per risk

# CAPA register — global (corrective actions cross-document)
capa_entries:
  - id: CAPA-2026-05-09-K8.2-V2-REFRAMING
    # ... full CAPA entry per format above
  # ... per CAPA

# Audit trail register — global (cross-document events)
audit_trail:
  - date: 2026-05-09
    event: "K8.2 v2 closure"
    documents_affected: [DOC-A-001, DOC-C-001, ...]
    commits: ["6ee1a85..7527d00"]
    governance_impact: "K-L3 selective per-component closure achieved"
  # ... per major event
```

Per-document frontmatter mirror is auto-generated from centralized YAML by a Python or PowerShell script in `tools/governance/`:

```yaml
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT manually
# Last generated: 2026-XX-XX from REGISTER.yaml commit <hash>
register_id: DOC-A-001
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.5"
next_review_due: "2027-05-10"
---

# DualFrontier Kernel — Architecture & Roadmap

(rest of document)
```

The frontmatter is read-only mirror; the SoT is the centralized YAML. The tooling regenerates frontmatter on every register change (precedent: A'.0.5 cross-ref refresh script tooling).

### Q10 — Tooling design (centralized YAML → frontmatter mirror)

**Question**: What is the tooling shape?

**Recommendation**: Single-script tooling, `tools/governance/sync_register.ps1` (or `.py` if non-Windows portability desired in future):

- Reads `docs/governance/REGISTER.yaml`
- For each document entry, locates the .md file at the entry's `path` field
- Updates the YAML frontmatter at top of .md file (insert if absent, replace if present)
- Validates: every register entry has a matching .md file; every .md file has a register entry (else flagged for enrollment); cross-references resolve (referenced risk IDs exist, referenced CAPA IDs exist, referenced documents exist)
- Outputs validation report to stdout; non-zero exit on validation failure

Validation runs are pre-commit hook + CI (after Crystalka adopts CI; for now manual invocation acceptable).

Tooling is itself a governance artifact — register's DOC-G-XXX entry includes tooling scripts.

### Q11 — Risk register taxonomy

**Question**: How are risks classified beyond likelihood × impact?

**Recommendation**:

- **Likelihood**: Low / Medium-Low / Medium / Medium-High / High (5-tier scale; matches existing MIGRATION_PROGRESS Risk register vocabulary)
- **Impact**: Low / Medium / High / Critical (4-tier scale)
- **Risk Type**: Technical / Architectural / Methodological / Operational / External
  - Technical = specific implementation challenge (e.g. P/Invoke marshalling correctness)
  - Architectural = decision-level risk affecting structural integrity (e.g. component conversion scope)
  - Methodological = process-level risk (e.g. amendment plan completeness gap pattern)
  - Operational = environment / tooling risk (e.g. testhost.exe file lock incidents)
  - External = dependency risk (e.g. Vulkan driver compatibility)
- **Status**: ACTIVE (under mitigation) / RESIDUAL (mitigation insufficient; accepted) / CLOSED (no longer applicable) / REALIZED (consequence occurred; CAPA opened)
- **Mitigation**: free-text description + optional reference to mitigation artifact (test, code, brief, methodology section)

Existing R1–R6 risks from MIGRATION_PROGRESS map cleanly into this taxonomy as the first 6 register risks; A'.4.5 enrollment campaign extends with risks surfaced during Phase A' (incl. environmental incidents, amendment plan completeness gaps, lesson-applier latency).

### Q12 — Requirement → Test Traceability format

**Question**: What format for traceability entries?

**Recommendation**: Per-requirement entry with explicit test file references:

```yaml
requirements:
  - id: REQ-K-L11
    title: "Single NativeWorld backbone — production storage"
    source_document: DOC-A-001  # KERNEL_ARCHITECTURE.md K-L11
    source_section: "Part 0, K-L11"
    requirement_text: "Production storage = NativeWorld single source of truth; ManagedWorld retained as test fixture only"
    verified_by:
      - file: tests/DualFrontier.Core.Interop.Tests/NativeWorldTests.cs
        test_method: "NativeWorld_CRUD_FullRoundTrip"
        verification_type: behavioral
      - file: native/DualFrontier.Core.Native/test/selftest.cpp
        test_method: "scenario_basic_crud"
        verification_type: native_equivalence
      - file: docs/reports/PERFORMANCE_REPORT_K7.md
        test_method: "V2 vs V3 comparison §8 metrics"
        verification_type: empirical_evidence
    verification_status: VERIFIED  # PENDING | PARTIAL | VERIFIED | FAILED
    verification_date: 2026-05-09  # K8.2 v2 closure date
```

**Cross-cutting**: every K-Lxx, every M-Lxx (future), every architectural lock from Phase A' deliberations becomes a register requirement entry. Tracing «which test verifies K-L11?» becomes a YAML query, not a search across 4 files.

### Q13 — Internal Audit Schedule cadence

**Question**: What cadence per Tier?

**Recommendation**:

- **Tier 1 (LOCKED specs)**: review on every change (mandatory: deliberation milestone produces «review pass» entry) + annual full review (calendar-anchored Q1 of each year)
- **Tier 2 (LIVE trackers)**: review on every closure write (mandatory: closure milestone updates last_review_date) + quarterly drift review (calendar-anchored Q1/Q2/Q3/Q4)
- **Tier 3 (briefs)**: review on Status transition only; AUTHORED→EXECUTED triggers review pass; no calendar cadence (briefs are ephemeral by design)
- **Tier 4 (module-local)**: quarterly Phase-led sweep (precedent: A'.0.5 Phase 6 module-local refresh; recurring cadence ~3-month between sweeps)

Calendar-anchored reviews are documented in register with `next_review_due` field; STALE state triggers when `next_review_due < today` without intervening review.

### Q14 — Corrective Action Log entry shape

**Question**: What shape for CAPA entries?

**Recommendation**:

```yaml
capa_entries:
  - id: CAPA-2026-05-09-K8.2-V2-REFRAMING
    opened_date: 2026-05-09
    trigger: "K-L3 «без exception» framing surfaced as misalignment at K8.2 v2 closure verification"
    root_cause: |
      K8.2 v1 brief authored 2026-05-08 against pre-K-L3.1 framing. After K8.2 v2 closure, framing was carried into closure entry as «K-L3 «без exception» state achieved». Crystalka observation 2026-05-10 «там был частичный перенос то что можно легко преобразовывать в struct было преобразовано» revealed closure outcome was selective per-component judgment, not universal mandate.
    immediate_action: "Halt K-series progress; surface K-L3 wording for retroactive principle reformulation"
    corrective_action: "K-L3.1 bridge formalization deliberation milestone (A'.0) + amendment plan (Phase 4 deliverable) + amendment landing milestones (A'.1.K)"
    effectiveness_verification:
      method: "Post-amendment cross-document audit"
      date_verified: 2026-05-10
      verification_commit: "0789bd4"
      verification_outcome: "All «без exception» / «class-based prohibited» wording reformulated or moved to historical/version-quote context. KERNEL v1.3 → v1.5; MOD_OS v1.6 → v1.7; MIGRATION_PLAN v1.0 → v1.1. K-L3.1 amendment plan §K.12.2 cross-document drift audit clean."
    closure_status: CLOSED
    lessons_learned_reference: "K9 brief execution closure section + Phase A' execution operational lessons (5 items)"
```

Existing amendment plan landings (A'.0.7 → A'.1.M; K-L3.1 → A'.1.K) retroactively fit this shape; A'.4.5 enrollment campaign retroactively populates 2-4 CAPA entries from Phase A' history.

### Q15 — Audit Trail integration with git log

**Question**: How does register reference git log?

**Recommendation**: Register treats git log as authoritative audit trail; references rather than duplicates.

Each document entry has `audit_trail: "see git log --follow docs/path/file.md"` as the boilerplate; explicit per-event entries in global audit_trail section only for **governance-significant events** (milestone closures, amendment landings, lifecycle transitions).

This avoids duplicating git log content while preserving register's narrative layer (e.g. «commits 27523ac..4e332bb constituted A'.0.5 reorganization» as semantic-level annotation that git log doesn't carry).

### Q16 — Methodology integration

**Question**: How does register integrate with existing METHODOLOGY corpus?

**Recommendation**: A'.4.5 amendment to METHODOLOGY v1.6 → v1.7:

- New section §12 «Document Control Register integration» — short text describing register as governance authority + cross-references to register sections
- Update §11 cross-references to include `docs/governance/REGISTER.yaml`
- Existing §7.1 «Data exists or it doesn't» principle extended: «documents exist in register or are out-of-scope» — register's `documents` field is the canonical list; out-of-register .md files are flagged as STALE-candidates or deletion-candidates by tooling validation
- Brief authoring checklist (§7.1 closing) extended: new mandatory item «Phase 0: read register entry for target document(s); update register entry on closure»

Estimated diff: ~150-300 lines added to METHODOLOGY.md.

### Q17 — Marketing/signaling artifact

**Question**: What artifact captures the «synthesized from 5 standards» marketing dimension?

**Recommendation**: A short companion document `docs/governance/SYNTHESIS_RATIONALE.md` (Tier 1 — LOCKED on creation) explicitly explaining:
- Which elements selected from each standard (Q4 lock)
- Why each element was selected (transferable to gamedev + solo + AI-pipeline context)
- Which elements de-selected and why (institutional ceremony not justified at scale)
- How the synthesis produces governance not available in any single standard (the «unique framework» claim)

This document is the marketing/signaling face of the register. Length: ~300-600 lines.

### Q18 — Tier 4 module-local enrollment scope

**Question**: Are all 70 module-local READMEs/MODULE.mds enrolled at A'.4.5 execution? Or staged?

**Options**:
- (a) **Big-bang enrollment** — all 70 enrolled in A'.4.5; 12-20 hour estimate accurate for this
- (b) **Tier-tiered enrollment** — Tier 1 (10-14 docs) + Tier 2 (5-10 docs) + Tier 3 (50-60 docs) at A'.4.5; Tier 4 deferred to subsequent A'.4.5.X follow-up milestone; ~8-12 hour estimate
- (c) **Sub-tiered Tier 4** — sub-divide Tier 4 into 4a (mods/ READMEs — likely thin placeholders) + 4b (src/ READMEs — varied) + 4c (tests/ READMEs — thin) + 4d (native/ MODULE.mds); enroll 4a + 4d at A'.4.5, defer 4b + 4c

**Recommendation**: **(a) Big-bang enrollment**. Argument: deferred enrollment perpetuates the «register isn't complete» state; partial register has marginal value (queries against partial data mislead). Big-bang at A'.4.5 takes the full ~12-20 hour hit but yields complete register. Future module documentation grows the register incrementally per source commit.

Tier 4 enrollment can be parallelized via simple loop: «for each module-local doc, classify, assign owner=Crystalka, status=Live (if thin placeholder) or Live (if has substantive content), no requirements/risks unless surfaces during enrollment».

### Q-cascade auxiliary locks (anticipated to surface during deliberation)

Per A'.0.7 lesson «Q-A07 cascade auxiliary locks», auxiliary locks may surface during Q1–Q18 deliberation. Anticipated cascade points:

- **Q-A45-X1** — register schema version. If schema evolves (predicted: 1-2 amendments in first year), how is schema-version itself tracked? Recommendation lock: register schema has its own version field at YAML root level; schema-version transitions surface as CAPA entries.

- **Q-A45-X2** — governance over governance (recursive). The register itself is a Tier 1 LOCKED document. Its entry in the register is DOC-G-001 (or similar). Recommendation lock: register entry for register itself maps to special meta-entry with self-reference handling in tooling.

- **Q-A45-X3** — language scope. METHODOLOGY v1.6 era documents are mixed English/Russian (per A'.0.7 era frame). Register schema is English. Frontmatter mirror in English. Does enrollment campaign mandate English-language register entries even for Russian-language documents? Recommendation lock: yes, register uses English schema + English-language entries; affected documents may still be Russian content but register metadata in English (precedent: METHODOLOGY abstract framing per Q-A07-1).

These auxiliary locks surface naturally during deliberation if Crystalka raises the corresponding concerns; locks recorded with `Q-A45-X` prefix and rationale.

---

## Phase 2 — Synthesis

After Q1–Q18 + auxiliary locks resolved, the synthesis produces the governance framework structure. Two-track synthesis (precedent: A'.0.7 §4.A + §4.C):

### Synthesis Track A — Framework (pipeline-agnostic governance principles)

A document `docs/governance/FRAMEWORK.md` (Tier 1 LOCKED) describing:

- Governance philosophy («without kostyli at the governance layer too»)
- Three-axis classification model (Category × Tier × Lifecycle)
- 7 register sections + cross-section relationships
- Source-standard provenance for each section element
- Audience contract (agent-as-primary-reader inheritance from A'.0.7 Q-A07-6)
- Falsifiable claims (under what conditions would the framework prove inadequate?)

This document is itself in the register as DOC-B-FRAMEWORK (Tier 1 / methodology / LOCKED).

### Synthesis Track B — Operational schema (per-era data)

The centralized YAML at `docs/governance/REGISTER.yaml` (Tier 2 LIVE — mutable on every milestone closure, but schema itself Tier 1 LOCKED).

This is the operational layer; the framework is the principle layer. Mirrors A'.0.7 «abstract methodology + per-era empirical record» pattern.

---

## Phase 3 — Phase A' integration

The A'.4.5 closure produces:

- New document `docs/governance/FRAMEWORK.md` (LOCKED v1.0)
- New document `docs/governance/SYNTHESIS_RATIONALE.md` (LOCKED v1.0, marketing/signaling)
- New document `docs/governance/REGISTER.yaml` (LIVE, schema v1.0)
- New tooling `tools/governance/sync_register.{ps1,py}` (executable, validation + frontmatter sync)
- New tooling validation report `tools/governance/VALIDATION_REPORT.md` (LIVE, regenerated per validation run)
- Updated `docs/methodology/METHODOLOGY.md` v1.6 → v1.7 (new §12 + extended §7.1 + extended §11)
- Updated `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (A'.4.5 entry inserted)
- 179 .md files with frontmatter mirror auto-applied via tooling first run
- Updated `docs/MIGRATION_PROGRESS.md` A'.4.5 closure entry

Estimated 8-15 atomic commits on `feat/a-prime-4-5-document-register` feature branch:

1. `docs(governance): author FRAMEWORK.md — synthesized governance framework v1.0`
2. `docs(governance): author SYNTHESIS_RATIONALE.md — 5-standard synthesis provenance v1.0`
3. `docs(governance): author REGISTER.yaml schema v1.0 — centralized governance SoT`
4. `feat(tooling): author sync_register tooling for SoT → frontmatter mirror`
5. `feat(governance): enroll Tier 1 documents in register (10-14 entries)`
6. `feat(governance): enroll Tier 2 documents in register (5-10 entries)`
7. `feat(governance): enroll Tier 3 documents in register (50-60 entries)`
8. `feat(governance): enroll Tier 4 module-local documents in register (~70 entries)`
9. `feat(governance): populate Risk register (~10-15 entries; R1-R6 from MIGRATION_PROGRESS + Phase A' surfaced risks)`
10. `feat(governance): populate Requirement-Test traceability matrix (K-Lxx + M-Lxx + Phase A' locks)`
11. `feat(governance): populate CAPA log (~3-5 retroactive entries from Phase A' history)`
12. `feat(governance): populate Audit trail register (~10-15 governance-significant events from Phase A')`
13. `feat(governance): run tooling first time + sync 179 frontmatter mirrors`
14. `docs(methodology): METHODOLOGY v1.6 → v1.7 — integrate register reference`
15. `docs(progress,sequencing): A'.4.5 closure recorded`

---

## Phase 4 — Deliverable (amendment plan)

After deliberation Phase 1 + synthesis Phase 2 + integration Phase 3 design, the Phase 4 deliverable is `docs/governance/A_PRIME_4_5_AMENDMENT_PLAN.md` — analog to `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md`:

- Per-document edit plan for METHODOLOGY v1.7 amendment
- Per-tier enrollment script (template entries to fill in execution)
- Per-source-standard element-selection rationale (synthesized SYNTHESIS_RATIONALE.md content)
- Tooling specification (sync_register.ps1 design + validation rules + output format)
- Pre-flight verification (Phase A' state check, A'.0.5 inventory baseline)
- Closure verification (validation report clean, all 179 documents enrolled, METHODOLOGY v1.7 landed)

Estimated plan length: ~1500-2500 lines (larger than A'.0.7 amendment plan due to 179-file enrollment campaign + tooling specification).

---

## Phase 5 — Execution-mode brief (post-amendment-plan)

Following A'.0.7 → A'.1.M precedent, after Phase 4 amendment plan is authored, a follow-up execution-mode brief is authored as Phase 5 deliverable: `tools/briefs/A_PRIME_4_5_EXECUTION_BRIEF.md`. This brief is consumed by the execution session.

The execution-mode brief carries:
- Pre-flight section (Phase 0 — verify state)
- Per-phase atomic-commit shape (15 commits per Phase 3 above)
- Operational reminders (testhost cleanup, --logger verbosity, file-lock pattern from Phase A' execution operational lessons)
- Stop conditions (analog K9 brief Stop conditions §1-§6)
- Closure verification

---

## Stop conditions (for this deliberation session)

The deliberation session halts and escalates if any of the following:

1. Any Q1–Q18 lock cannot be resolved through Crystalka deliberation pass (e.g. Crystalka surfaces a previously-unstated constraint that invalidates the recommendation).
2. Auxiliary Q-A45-X cascade locks proliferate beyond reasonable bound (~5+ auxiliary locks suggests framework underspecified at this scale).
3. Synthesis Track A + Track B produces internal contradictions that cannot be resolved without weakening one track.
4. Phase A' integration shows scope conflict with A'.5-A'.9 milestones that cannot be resolved without deferral.
5. Tier definitions (Q7) or lifecycle state machine (Q8) produce ambiguity for specific documents that cannot be resolved without expansion of axis vocabulary.

Halt mechanism: stash deliberation working notes; record block in MIGRATION_PROGRESS with explanation; resume deliberation in a follow-up session after Crystalka resolution.

---

## Cross-cutting design constraints

1. **Register as governance authority** — register itself is governance, not just documentation of governance. After A'.4.5 closure, register sections are normative; conflicts between register entries and document content trigger CAPA.

2. **Construction-by-rationale (Phase A' lesson)** — register validation is automated; manual review confirms tooling-detected issues only. The «every commit re-verifies governance» is replaced with «tooling validates governance state on every commit».

3. **Audience contract inheritance** — register is agent-primary (Q-A07-6 inheritance); human-readable rendering derivative via tooling.

4. **Open-source-separately property** — governance framework is independently valuable. `docs/governance/FRAMEWORK.md` could be open-sourced as «AI-pipeline-friendly engineering governance framework» without releasing the rest of the project. Compatible with K-L11 «NativeWorld single source of truth» (storage-layer principle), L-L1 «pure Vulkan» (runtime-layer principle), and now governance-L1 «register single source of truth for documentation governance».

5. **Bundle is default safe under session-mode (K-Lessons #2)** — A'.4.5 unifies framework + schema + tooling + enrollment + integration in single bundled milestone. Splitting risks dual-state mid-flight.

6. **Brief age × subsequent milestone closures = staleness density (K9 lesson)** — A'.4.5 amendment plan explicitly enumerates every required edit; surgical scrubs surface as §1.3-pattern fallback at execution time. Methodology lesson candidate from A'.4.5: «governance documents follow same staleness density pattern as briefs; review cadence per Tier matches density risk profile».

7. **Data exists or it doesn't (METHODOLOGY §7.1)** — every document either has a register entry or is out-of-scope. No middle state. Tooling enforces this structurally.

8. **No retroactive governance violations** — pre-A'.4.5 historical artifacts (closures, briefs, decisions) are retrofit into register at enrollment, not faulted for missing register entries. CAPA entries for retroactive enrollment may reference «governance era predates A'.4.5» as accepted root cause.

9. **Marketing dimension as deliberate signaling, not accidental aesthetics** — SYNTHESIS_RATIONALE.md is intentionally written to be marketable; FRAMEWORK.md is intentionally written to be operational. The split keeps each clean for its audience.

10. **Long-horizon planning** — register schema designed for decade-scale evolution. Schema v1.0 includes evolution mechanism (Q-A45-X1 auxiliary lock); v2.0+ amendments use same protocol as Tier 1 LOCKED amendments.

---

## Brief authoring lineage

- **2026-05-10** — Deliberation brief authored during Phase A' deliberation session (post-A'.3 push closure, HEAD `25c2bfb5`). Author: Claude Opus 4.7 deliberation session per Crystalka instruction «у меня есть более интересная идея для чистоты это внедрение единого document control register».
- **(date TBD)** — Phase 1 Q1–Q18 deliberation pass with Crystalka; Q-locks recorded; synthesis Phase 2 + integration Phase 3 finalized in same session.
- **(date TBD)** — Phase 4 amendment plan authored as Phase A' artifact.
- **(date TBD)** — Phase 5 execution-mode brief authored as next-session artifact.
- **(date TBD)** — A'.4.5 execution session executes amendment plan; ~15 atomic commits; ~12-20 hour wall time; 179 documents enrolled.

Source documents read during authoring:
- `tools/scratch/A_05/INVENTORY.md` (179-file classification baseline)
- `tools/scratch/A_05/BASELINE.md` (Phase 0 grep counts baseline)
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (Phase A' anchor)
- `docs/MIGRATION_PROGRESS.md` (existing risk register, D-decisions log, K-Lessons, OQ tracking)
- `docs/architecture/KERNEL_ARCHITECTURE.md` v1.5 (K-L1..K-L11 lock inventory)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.7 (governance vocabulary)
- `docs/methodology/METHODOLOGY.md` v1.6 (post-A'.0.7 era methodology vocabulary)
- `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` (amendment plan structure precedent)
- `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (architecture amendment plan precedent)
- DO-178C overview material (from training data; provenance: aerospace software development standards)
- ISO 9001:2015 overview material (from training data; provenance: quality management systems)
- ISO 26262 overview material (from training data; provenance: automotive functional safety)
- IEC 61508 overview material (from training data; provenance: general functional safety)
- FDA 21 CFR Part 11 overview material (from training data; provenance: medical-device electronic records)

---

**Brief end.** Awaits Crystalka deliberation pass to lock Q1–Q18 + auxiliary cascade.
