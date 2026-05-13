---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-SYNTHESIS_RATIONALE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-SYNTHESIS_RATIONALE
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-SYNTHESIS_RATIONALE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-SYNTHESIS_RATIONALE
---
# Document Control Register — Synthesis Rationale

*Source-standard provenance for the Document Control Register synthesis. Documents which elements were borrowed from which industry standards, why each was selected, and which elements were deliberately rejected.*

*Version: 1.0 (2026-05-12). Authored at A'.4.5 closure subordinate to [FRAMEWORK.md](./FRAMEWORK.md) v1.0. Status: AUTHORITATIVE LOCKED.*

*Audience: agent-primary (Q-A07-6 inheritance). Secondary: human reader interested in provenance / marketing signaling of «engineered selection from 5 sources».*

---

## 0. Provenance summary

**9 elements selected from 5 industry standards** for the register synthesis. **11 elements deselected** with documented rationale.

| Standard | Anchor role | Selected | Deselected |
|---|---|---|---|
| DO-178C — Software Considerations in Airborne Systems | **Primary #1** | 3 (SCI, Traceability, Problem Reports) | 2 (PSAC/SDP/SVP/SCMP/SQAP plan documents, TQL) |
| ISO 9001:2015 — Quality Management Systems | **Primary #2** | 3 (Document Control 7.5, Internal Audit 9.2, Corrective Action 10.2) | 2 (Management Review 9.3, Risk-based Thinking 6.1) |
| ISO 26262 — Functional Safety (Automotive) | Targeted borrowing | 1 (Safety Case re-purposed as Architecture Case) | 4 (ASIL, Confirmation Reviews, Safety Lifecycle phases, Item Definition) |
| IEC 61508 — Functional Safety (General) | Targeted borrowing | 1 (HARA re-purposed as Architectural Risk Analysis) | 4 (SIL, V&V Planning, Safety Manual, CCFA) |
| FDA 21 CFR Part 11 — Electronic Records | Minimal targeted | 1 (Audit Trail as reference) | 4 (Electronic Signature, ALCOA+ Data Integrity, System Validation, Record Retention) |

**Honest framing**: «primary 2 + targeted 3», not «equal synthesis of 5». DO-178C and ISO 9001 contribute 6 of 9 elements between them; the remaining three standards contribute 1 element each. Reading this as «equal» misrepresents the design decision.

**Selection criterion**: transferable to solo-gamedev + AI-pipeline + open-source-separately property + decade-horizon planning.

**Deselection criterion**: institutional ceremony incompatible with scale.

**Authority**: this document is subordinate to FRAMEWORK.md. When schema evolves (per FRAMEWORK §7), this document is amended in lockstep to preserve source-standard provenance per element.

---

## 1. Source standards inventoried

### 1.1 DO-178C — Software Considerations in Airborne Systems

- **Origin**: RTCA / EUROCAE aerospace software development standard
- **Scope**: software engineering discipline for safety-critical avionics (DAL A/B/C/D levels)
- **Why considered**: stands at engineering-level discipline matching the project's engineering layer (vs ISO 9001 at business-level). Configuration management, traceability matrix, problem reports are software-discipline vocabulary directly applicable
- **Anchor role**: **PRIMARY #1** — semantically nearest to what project already does (atomic commits, K-Lxx invariant tracking, deviation recording in closure reports)

### 1.2 ISO 9001:2015 — Quality Management Systems

- **Origin**: International Organization for Standardization
- **Scope**: process discipline for quality management across industries (document control, audit, corrective action as business processes)
- **Why considered**: complementary to DO-178C engineering layer. Provides recognizable signaling («Quality Manual» framing) + concrete process discipline. Document Control clause 7.5 is reference-implementation of what register builds
- **Anchor role**: **PRIMARY #2** — complementary to DO-178C; together cover «how we do» (DO-178C engineering) + «how we verify we're doing it right» (ISO 9001 process)

### 1.3 ISO 26262 — Functional Safety (Automotive)

- **Origin**: ISO automotive functional safety standard
- **Scope**: safety-critical automotive software development; ASIL decomposition, safety lifecycle, item definition
- **Why considered**: Safety Case concept (structured argumentation showing system meets requirements with end-to-end decision rationale) maps cleanly to «Architecture Case» — structured argumentation showing architectural decisions are coherent
- **Anchor role**: **TARGETED BORROWING** — one concept; broader automotive vocabulary overshoots gamedev

### 1.4 IEC 61508 — Functional Safety (General)

- **Origin**: International Electrotechnical Commission generalized functional safety standard
- **Scope**: SIL classification, HARA (Hazard and Risk Analysis), V&V planning, safety manual
- **Why considered**: HARA provides systematic identification + analysis of hazards with likelihood/severity assessment + mitigation tracking — directly applicable as «Architectural Risk Analysis»
- **Anchor role**: **TARGETED BORROWING** — one concept; broader safety-integrity vocabulary not applicable

### 1.5 FDA 21 CFR Part 11 — Electronic Records, Electronic Signatures

- **Origin**: US Food and Drug Administration regulation for electronic records
- **Scope**: audit trail (tamper-evident), electronic signatures (non-repudiation), data integrity (ALCOA+), system validation, record retention
- **Why considered**: audit trail concept directly applicable; rest of standard heavy with medical-device-specific ceremony
- **Anchor role**: **MINIMAL TARGETED BORROWING** — one concept; treats existing git log as authoritative audit trail rather than duplicating

---

## 2. Selected elements per source (9 total)

### 2.1 DO-178C selected (3 elements)

#### 2.1.1 Software Configuration Index (SCI) → FRAMEWORK §4.1 Document Control

- **DO-178C definition**: enumeration of all configuration items (code, docs, test artifacts) with versions/baselines tied to release configurations
- **Project adaptation**: per-document register entry with `id`, `path`, `version`, `last_modified_commit`, `lifecycle`, `owner` — SCI semantics applied to documentation
- **Why selected**: project already had implicit SCI (LOCKED docs with v1.X, brief Status field, git log as audit). Register formalizes into queryable surface. Eliminates implicit-state problem where SCI lived in project members' heads
- **Adaptation rationale**: aerospace SCI is release-tied (snapshots per certified release). Project has mutable LIVE docs (Tier 2 trackers) + LOCKED docs (Tier 1 specs). Schema supports both via lifecycle field; LIVE entries don't require release-snapshot ceremony

#### 2.1.2 Requirement-to-test traceability matrix → FRAMEWORK §4.3

- **DO-178C definition**: bidirectional traceability from requirements (high-level) → low-level requirements → source code → tests, ensuring every requirement has verifying test and every test verifies a requirement
- **Project adaptation**: K-L1..K-L11 (and future M-Lxx, G-Lxx) are formal requirements; tests verifying each are explicit cross-reference in register §3
- **Why selected**: **highest-value DO-178C concept for project**. Currently «K-L11 single NativeWorld → which tests verify this?» is scattered across KERNEL_ARCHITECTURE Part 2 + test file names + PERFORMANCE_REPORT_K7. Register consolidates into single query
- **Adaptation rationale**: DO-178C requires bidirectional traceability with formal verification certificate. Project supports bidirectional via cross-references but primary direction is agent-query («what verifies K-L7?»). Verification types extended beyond aerospace «behavioral/structural» to: behavioral / native_equivalence / empirical_evidence / code_inspection / spec_inspection / build_evidence / build_configuration / tooling_evidence — matching project's diverse verification artifact types

#### 2.1.3 Problem Reports + RCA → FRAMEWORK §4.6 CAPA (joint with ISO 9001)

- **DO-178C definition**: problem reports formally track defects/deviations with root cause analysis, corrective action, verification of effectiveness
- **Project adaptation**: K8.2 v2 reframing CAPA, A'.0.7 audience inversion CAPA, A'.0.5 count drift CAPA — retroactive entries built from actual Phase A' history
- **Why selected**: project already had implicit problem reports — brief deviation sections, closure «Lessons learned», amendment plans. Register formalizes shape
- **Adaptation rationale**: aerospace problem reports often tied to defect tracking systems (JIRA-like). Project CAPA entries cross-reference git commits + briefs + lessons learned; no separate bug tracker. ISO 9001 10.2 provides the formal 5-field shape (trigger / root cause / immediate action / corrective action / effectiveness verification); DO-178C provides the «problem report» framing. Joint anchor

### 2.2 ISO 9001 selected (3 elements)

#### 2.2.1 Document Control (clause 7.5) → FRAMEWORK §4.1 (joint with DO-178C SCI)

- **ISO 9001 definition**: each controlled document has identifier, version, owner, status, review schedule, retention policy; review/approval workflow defined
- **Project adaptation**: §4.1 fields enumerate per FRAMEWORK schema. Tier-based review cadence. Owner field allows future contributor onboarding
- **Why selected**: precise semantic match to what register needs. ISO 9001 7.5 is reference-implementation of «agent-primary navigation surface»
- **Adaptation rationale**: ISO 9001 assumes business org with dedicated document control officer. Project has Crystalka as sole owner initially; schema supports future contributor onboarding without restructuring (Owner field rewritable per-document)

#### 2.2.2 Internal Audit (clause 9.2) → FRAMEWORK §4.5

- **ISO 9001 definition**: periodic audit of processes/documents for evidence of conformance to requirements
- **Project adaptation**: per-tier review cadence (Tier 1 annual + on-change, Tier 2 quarterly + on-closure, Tier 3 on-Status-transition, Tier 4 quarterly Phase-led sweep, Tier 5 none). STALE flag when `next_review_due < today`
- **Why selected**: project already does audit ad-hoc (A'.0.5 Phase 1 inventory was audit pass). Register formalizes cadence so audits become scheduled, not reactive
- **Adaptation rationale**: ISO 9001 9.2 typically requires independent auditors. Project has Crystalka self-audit + tooling validation. «Independent» property achieved through **construction-by-rationale** (Phase A' lesson) — tooling validates schema invariants, doesn't depend on subjective review

#### 2.2.3 Corrective Action (clause 10.2) → FRAMEWORK §4.6 (joint with DO-178C Problem Reports)

- **ISO 9001 definition**: structured 5-field CAPA template (trigger / root cause / immediate action / corrective action / effectiveness verification + closure)
- **Project adaptation**: 3 retroactive CAPA entries from Phase A' history (K8.2 v2 reframing CLOSED, A'.0.7 audience inversion CLOSED, A'.0.5 count drift OPEN)
- **Why selected**: ISO 9001 contributes formal CAPA shape; DO-178C contributes problem-report framing. Together: register §6 entries have formal structured shape, not free-text «lessons learned»
- **Adaptation rationale**: ISO 9001 CAPA closure requires independent verification of effectiveness. Project verification = «commit hash + date + cross-document drift audit clean» (as K-L3.1 closure 2026-05-10 demonstrated)

### 2.3 ISO 26262 selected (1 element)

#### 2.3.1 Safety Case → re-purposed as «Architecture Case» (FRAMEWORK §4.2)

- **ISO 26262 definition**: Safety Case = structured argumentation showing system meets safety requirements, with decision rationale documented end-to-end
- **Project re-purposing**: «Architecture Case» = structured argumentation showing architectural decisions are coherent, with rationale documented at decision point
- **Project adaptation**: K-Lxx invariants → for each, register §2 entry contains `rationale_anchors` list (e.g., `[K8.0_brief, K-L3.1_brief]`) showing **why** this invariant holds. Same pattern for Q-locks from deliberation milestones (Q-A07-1..12, Q1-Q6 K-L3.1, Q4-Q18 A'.4.5)
- **Why selected**: project already does decision rationale documentation **exceptionally well** — K8.0 brief, K-L3.1 brief, A'.0.7 brief all show «why» with rejected alternatives. Register formalizes the reference structure: «which brief justifies K-L7?» becomes agent-queryable
- **Adaptation rationale**: ISO 26262 Safety Case tied to safety integrity demonstration for certification. «Architecture Case» tied to architectural integrity demonstration for «без костылей десятилетие horizon» commitment. Same structural device; different domain

### 2.4 IEC 61508 selected (1 element)

#### 2.4.1 HARA (Hazard and Risk Analysis) → re-purposed as «Architectural Risk Analysis» (FRAMEWORK §4.4)

- **IEC 61508 definition**: HARA = systematic identification + analysis of hazards with likelihood/severity assessment + mitigation tracking
- **Project re-purposing**: «Architectural Risk Analysis» = systematic identification + analysis of architectural / methodological / operational risks with likelihood/impact + mitigation tracking
- **Project adaptation**: R-skeleton seed with 14 initial entries (5 risk types: Technical / Architectural / Methodological / Operational / External), taxonomy Likelihood × Impact × RiskType × Status
- **Why selected**: project already had implicit risk register (D-log rationales mention risks; OQ section names «what if» scenarios; METHODOLOGY §5 enumerates pipeline threats; closure «Lessons learned» implicitly retrospectively named risks). HARA provides **formal shape**
- **Adaptation rationale**: IEC 61508 HARA tied to safety integrity. Project risks are **architectural integrity** oriented — «risk to «без костылей десятилетие horizon» commitment». Crystalka direction 2026-05-12 («требуется создать скелет для R») confirms R-skeleton is new authoring (not migration of existing Risk register — that section does NOT exist in MIGRATION_PROGRESS as pre-flight verified)

### 2.5 FDA 21 CFR Part 11 selected (1 element)

#### 2.5.1 Audit Trail → FRAMEWORK §4.7 (as reference, not duplicate)

- **21 CFR Part 11 definition**: Audit Trail = chronological record of all changes to electronic records, tamper-evident, retention-controlled
- **Project adaptation**: **git log is already tamper-evident chronological audit trail**. Register §7 entries reference git log (`audit_trail_query: "git log --follow path/file.md"`) + add **semantic-level annotation** that git log doesn't carry (e.g., «commits 27523ac..4e332bb constituted A'.0.5 reorganization» as narrative layer)
- **Why selected**: minimal-overhead concept; doesn't duplicate git log + adds milestone-level semantics
- **Adaptation rationale**: 21 CFR Part 11 requires electronic signatures with non-repudiation. Git commits with author + date = de-facto electronic signature; formal e-signature ceremony not required. 21 CFR Part 11 requires retention policy; git history retention is forever (no archival ceremony)

---

## 3. Deselected elements per source (11 total)

### 3.1 Deselected from DO-178C (2 elements)

#### 3.1.1 Plan-level documents (PSAC, SDP, SVP, SCMP, SQAP)

- PSAC = Plan for Software Aspects of Certification
- SDP = Software Development Plan
- SVP = Software Verification Plan
- SCMP = Software Configuration Management Plan
- SQAP = Software Quality Assurance Plan
- **Why rejected**: project already has functional equivalents without DO-178C formal structure:
  - ROADMAP.md = development plan
  - KERNEL_ARCHITECTURE Part 2 = verification plan (milestone enumeration with verification criteria)
  - MIGRATION_PLAN_KERNEL_TO_VANILLA = configuration management plan
  - METHODOLOGY.md = quality assurance plan
- Force-fitting these into 5 separate plan-documents = **bureaucratic kostyl** without operational benefit. Project's «без костылей» commitment excludes this entirely

#### 3.1.2 Tool Qualification Levels (TQL1-5)

- TQL = qualifying development tools (compilers, code generators, analyzers) as themselves cert-suitable; required when certification depends on tool output correctness
- **Why rejected**: aerospace cert requires proving tools don't introduce defects. Gamedev context doesn't have this requirement. Roslyn analyzer (Phase A'.9) will be verification tool but **doesn't substitute for tests** — analyzer encodes K-Lxx invariants, doesn't replace test verification. Not TQL-qualified, doesn't need to be

### 3.2 Deselected from ISO 9001 (2 elements)

#### 3.2.1 Management Review (clause 9.3)

- ISO 9001 9.3 = periodic review by senior management of QMS performance
- **Why rejected**: «senior management» = solo Crystalka. Management review collapses into the same Crystalka deliberation sessions that already produce milestone closures. Adding formal «management review meeting» = institutional ceremony without operational value

#### 3.2.2 Risk-based Thinking (clause 6.1)

- ISO 9001 6.1 = systematic identification of risks/opportunities to process performance
- **Why rejected**: subsumed by IEC 61508 HARA (selected as §4). Don't duplicate risk vocabulary. HARA is more specific and operationally clearer

### 3.3 Deselected from ISO 26262 (4 elements)

#### 3.3.1 ASIL (Automotive Safety Integrity Level) decomposition

- Hierarchical safety levels A/B/C/D with decomposition rules
- **Why rejected**: automotive safety integrity not applicable to gamedev

#### 3.3.2 Confirmation Reviews

- Independent verifier requirement for safety-critical decisions
- **Why rejected**: solo + AI pipeline context — independent verifier requirement violates pipeline economics. Construction-by-rationale (Phase A' lesson) substitutes: tooling validates schema invariants without depending on independent reviewer

#### 3.3.3 Safety Lifecycle phases

- Heavyweight phase-gated lifecycle with safety milestones
- **Why rejected**: project already has Phase A / Phase A' / Phase B structure via PHASE_A_PRIME_SEQUENCING.md. Adding parallel safety-lifecycle phases = duplicate phase ceremony

#### 3.3.4 Item Definition

- System boundary documentation (what's in/out of scope)
- **Why rejected**: project already has system boundary docs (ARCHITECTURE.md, KERNEL_ARCHITECTURE.md, MOD_OS_ARCHITECTURE.md, RUNTIME_ARCHITECTURE.md). Adding «Item Definition» as separate document = redundancy

### 3.4 Deselected from IEC 61508 (4 elements)

#### 3.4.1 SIL (Safety Integrity Level) classification

- Hierarchical levels SIL1-SIL4 for safety-critical functions
- **Why rejected**: same as ASIL — safety integrity classification not applicable

#### 3.4.2 Verification & Validation Planning

- Formal V&V planning documents
- **Why rejected**: already covered by DO-178C traceability matrix (selected as §3). Duplication unnecessary

#### 3.4.3 Safety Manual

- Operating constraints documentation (what's safe to do with the system)
- **Why rejected**: project already has ARCHITECTURE.md + MOD_OS_ARCHITECTURE.md + RUNTIME_ARCHITECTURE.md serving same purpose for project's domain (architectural constraints rather than safety constraints). Renaming to «Safety Manual» adds nothing

#### 3.4.4 Common Cause Failure Analysis (CCFA)

- Analysis of failure modes shared across redundant systems
- **Why rejected**: overkill for architectural risks at project's scale. May return if multi-redundancy patterns emerge in future (e.g., GPU compute + CPU fallback redundancy beyond current «graceful degradation»). Currently YAGNI

### 3.5 Deselected from FDA 21 CFR Part 11 (4 elements)

#### 3.5.1 Electronic Signature (with non-repudiation)

- Formal electronic signature ceremony with cryptographic non-repudiation
- **Why rejected**: git commit authorship (author + date + signed if GPG configured) = de-facto electronic signature sufficient for project scale. Formal e-signature ceremony adds nothing

#### 3.5.2 Data Integrity (ALCOA+)

- Attributable / Legible / Contemporaneous / Original / Accurate principles for electronic records
- **Why rejected**: principles partially built into atomic commit discipline + structured logging conventions. Don't require formal ALCOA+ framework as separate compliance layer

#### 3.5.3 System Validation

- Formal validated-change-control procedures
- **Why rejected**: covered by methodology §closure-protocol + atomic commit discipline + register `sync_register.ps1 --validate` gate. Adding FDA-style System Validation = ceremony duplication

#### 3.5.4 Record Retention

- Long-term archival policy with retention durations
- **Why rejected**: git history retention is forever (no archival ceremony). Project doesn't have records that need deletion-after-N-years compliance

---

## 4. Selection criterion in operational terms

Each selected element was tested against four criteria:

1. **Transferability**: does the concept apply to gamedev + solo + AI-pipeline context without semantic distortion?
   - Pass: SCI (formal version tracking applicable), HARA (risk taxonomy applicable), Safety Case (decision rationale applicable), Document Control (per-doc metadata applicable), Internal Audit (review cadence applicable), CAPA (problem→solution shape applicable), Traceability (requirement→test applicable), Audit Trail (git log analog already exists)
   - Fail: ASIL/SIL (safety integrity not applicable), TQL (cert-tool-qualification not applicable), E-signature (overkill), ALCOA+ (overkill), Item Definition (already covered), Safety Manual (already covered)

2. **Open-source-separately property**: would this element add value to a hypothetical open-source fork of just-the-governance-framework?
   - Pass: all 9 selected elements — each is conceptually self-contained governance device
   - Fail: PSAC/SDP/SVP/SCMP/SQAP (project-specific plan documents not portable), Management Review (solo-developer-collapsed)

3. **Decade-horizon planning**: does this element scale to thousands of documents?
   - Pass: SCI (schema scales), Traceability (per-requirement entries scale), HARA (per-risk entries scale), CAPA (per-CAPA entries scale), Document Control (per-doc entries scale with tooling validation)
   - Fail: heavyweight Safety Lifecycle (phase ceremony doesn't scale to per-document discipline)

4. **Mechanical enforceability**: can tooling validate compliance with this element?
   - Pass: all selected — register schema invariants + cross-reference integrity + status transitions + audit cadence all tooling-checkable
   - Fail: Confirmation Reviews (requires human judgment, not tooling-checkable), Safety Lifecycle phase gates (requires human decision-making)

Elements scoring positive on all four were selected. Elements scoring negative on any were deselected with documented rationale.

---

## 5. Honest framing

The synthesis is **primary 2 + targeted 3**, not **equal 5**:

- **Primary anchors** (6 elements between them):
  - DO-178C contributes 3 elements (SCI / Traceability / Problem Reports)
  - ISO 9001 contributes 3 elements (Document Control / Internal Audit / Corrective Action)
- **Targeted borrowings** (3 elements):
  - ISO 26262 contributes 1 (Safety Case → Architecture Case)
  - IEC 61508 contributes 1 (HARA → Architectural Risk Analysis)
  - FDA 21 CFR Part 11 contributes 1 (Audit Trail as reference)

**Why this framing matters**:

1. **Operationally honest**: project did weight sources unevenly. DO-178C and ISO 9001 carry 6 of 9 elements; reading their contribution as «equal» misrepresents the design decision
2. **Signaling-stronger**: «engineered selection from 5 sources» reads as deliberate engineering choice; «equal synthesis» reads as buzzword sprawl. Honest framing is **stronger marketing claim**, not weaker
3. **Decision-traceable**: future amendments to schema reading «which source contributed this element?» get clean answer. No retroactive «well, kind of from X but really from Y» ambiguity
4. **Aligns with «без костылей» commitment**: false-equivalence framing would be a marketing kostyl that operational reality doesn't support. Project's no-compromise commitment extends to provenance accuracy

---

## 6. Evolution mechanism

When schema evolves (per FRAMEWORK §7), source-standard provenance is preserved per element:

- **Adding new selected element from existing source**: extend §2.X with new sub-section; document selection rationale per criterion §4
- **Adding selected element from new source**: extend §1 (source inventory) with new sub-section + extend §2 (selected) with new section + add 6th-or-later anchor designation. Honest framing must be updated («primary 2 + targeted 4» if new targeted, or «primary 3 + targeted 3» if new primary)
- **Deselecting previously-selected element**: move from §2 to §3 with retroactive rationale («at v1.X we used this; at v1.Y we removed because Z»). Migration step preserves audit trail
- **Bumping schema version**: §0 provenance summary updated to reflect new selection counts; FRAMEWORK schema version history table tracks the change

This document is **subordinate to FRAMEWORK**: amendments must follow Tier 1 LOCKED amendment protocol (deliberation milestone + amendment plan + landing). Per FRAMEWORK §8 meta-entry rule 4, schema version coupling enforced — this document's `schema_version` reference must match REGISTER.yaml top-level `schema_version`.

---

## 7. Cross-references

- [FRAMEWORK.md](./FRAMEWORK.md) — governance framework specification (this document is provenance layer for it)
- [REGISTER.yaml](./REGISTER.yaml) — operational SoT (schema implements selected elements)
- [REGISTER_RENDER.md](./REGISTER_RENDER.md) — human-readable rendered derivative

Source standards (background reading; not authoritative for project, contextual only):
- DO-178C — RTCA DO-178C / EUROCAE ED-12C, Software Considerations in Airborne Systems and Equipment Certification (2011)
- ISO 9001:2015 — ISO 9001:2015 Quality management systems — Requirements (International Organization for Standardization, 2015)
- ISO 26262 — ISO 26262 Road vehicles — Functional safety (ISO, 2018 second edition; multi-part)
- IEC 61508 — IEC 61508 Functional safety of electrical/electronic/programmable electronic safety-related systems (IEC, 2010 edition 2.0; multi-part)
- FDA 21 CFR Part 11 — Code of Federal Regulations Title 21 Part 11, Electronic Records; Electronic Signatures (FDA)

A'.4.5 deliberation artifacts:
- [tools/briefs/A_PRIME_4_5_DOCUMENT_CONTROL_REGISTER_BRIEF.md](../../tools/briefs/A_PRIME_4_5_DOCUMENT_CONTROL_REGISTER_BRIEF.md) — deliberation brief
- [tools/scratch/A_05/A_PRIME_4_5_DELIBERATION_CLOSURE.md](../../tools/scratch/A_05/A_PRIME_4_5_DELIBERATION_CLOSURE.md) — closure (23 Q-locks)
- [tools/briefs/A_PRIME_4_5_PASS_1_Q4_STANDARDS_SELECTION_LOCK.md](../../tools/briefs/A_PRIME_4_5_PASS_1_Q4_STANDARDS_SELECTION_LOCK.md) — Pass 1 execution-context brief (source for §2 / §3 content)
