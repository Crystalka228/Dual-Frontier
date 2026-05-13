---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-FRAMEWORK
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-FRAMEWORK
---
# Document Control Register — Governance Framework

*Project synthesis from DO-178C / ISO 9001 / ISO 26262 / IEC 61508 / FDA 21 CFR Part 11. Bespoke framework fitted to solo-developer + AI-pipeline + decade-horizon planning context.*

*Version: 1.0 (2026-05-12). Schema lock at A'.4.5 closure. Status: AUTHORITATIVE LOCKED.*

*Authored under agent-as-primary-reader assumption per Q-A07-6 lock 2026-05-10. Provenance for source-standard borrowings documented in [SYNTHESIS_RATIONALE.md](./SYNTHESIS_RATIONALE.md).*

---

## 0. Abstract

The Document Control Register is the project's operational navigation surface for documentation governance. It provides:

- **Per-document register entry** (`docs/governance/REGISTER.yaml`) — canonical Source of Truth for classification (Category × Tier × Lifecycle), ownership, versioning, audit cadence, cross-references
- **Global collections** — requirements, risks, CAPA log, audit trail — cross-document tracking that lives outside individual documents
- **Three-tool PowerShell suite** at `tools/governance/` — write-side sync + validation (`sync_register.ps1`), read-side queries (`query_register.ps1`), human-readable rendering (`render_register.ps1`)
- **Post-session update protocol** — every execution session that closes a milestone OR modifies any register-scope document must update REGISTER.yaml and pass validation before commit closure

Falsifiable claim: register reduces governance-tracking cognitive load to the point where solo-developer + AI-pipeline can sustain decade-horizon architectural discipline without ad-hoc tracking devolving into stale artifacts. Failure mode: register itself stales (RISK-010); mitigation Q-A45-X5 enforcement gate.

---

## 1. What the register is

### 1.1 Primary role — agent navigation surface

The register's primary function is **navigation aid for AI agents** operating in the project's session-mode pipeline (per [METHODOLOGY](../methodology/METHODOLOGY.md) §2.1.1). Without the register, knowledge about «which test verifies K-L11?» or «what's the architectural authority for component storage?» or «what documents typically change when K-series milestones close?» lives implicitly in:

- Cross-references within individual documents (linearly read, hard to query)
- Author memory (lost across sessions)
- Brief content (scattered across `tools/briefs/*`)
- Closure reports (retrospective, not queryable)

The register consolidates these into a queryable YAML surface. Agent queries replace human-memory queries.

### 1.2 Secondary roles

Beyond navigation, the register serves:

- **Governance audit trail** — every register state captures «which decisions were made, what risks were tracked, which CAPA actions closed» as structured record
- **Contributor onboarding** — future contributors get explicit handoff surface (Owner field, audit cadence, change authority per tier)
- **Marketing signal** — synthesis from 5 industry standards is deliberate signaling that governance was engineered, not adopted off-the-shelf (see [SYNTHESIS_RATIONALE.md](./SYNTHESIS_RATIONALE.md))

### 1.3 Relationship to existing project infrastructure

The register **formalizes existing practice**, not invents new bureaucracy. Each register section maps to an existing governance pattern already in use:

| Register section | Existing project pattern |
|---|---|
| §1 Document Control | LOCKED spec version fields, brief Status fields, LIVE document patterns |
| §2 Architecture Case | K-Lxx invariant decisions + rationale anchored in K8.0, K-L3.1 briefs |
| §3 Traceability | Cross-references «which test verifies X» scattered across spec + brief + report |
| §4 Risk Register | OQ tracking + D-decisions log in MIGRATION_PROGRESS + METHODOLOGY §5 threat model |
| §5 Internal Audit | Ad-hoc inventory passes (A'.0.5 Phase 1) |
| §6 CAPA Log | Amendment plans (A'.0.7, K-L3.1) + closure «Lessons learned» |
| §7 Audit Trail | Git log (authoritative) + commit-hash references in MIGRATION_PROGRESS |

Register operationalizes these patterns into a uniform queryable surface.

---

## 2. Why it exists — failure modes the register prevents

### 2.1 PHASE_A_PRIME_SEQUENCING staleness pattern

Pre-A'.4.5 observed: `PHASE_A_PRIME_SEQUENCING.md` carried A'.0.7 status «NEXT» despite A'.0.7 closure completed 2026-05-10. No structural mechanism existed to enforce that closure milestones update referencing documents. Register's post-session protocol (Q-A45-X5) + validation gate (sync_register.ps1) catches this on every commit.

### 2.2 Inventory drift between snapshots

A'.0.5 INVENTORY.md authored 2026-05-10 as point-in-time snapshot recorded ~135 .md files. By A'.4.5 deliberation pre-flight (2026-05-12), actual count ~220 — drift of 60+ documents without tracking mechanism. CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT records this; register's living inventory + validation flags closes the failure mode structurally.

### 2.3 Unknown unknowns at closure

Pre-A'.4.5 pattern: agent executes milestone, mental model lives in Crystalka's head about «what governance bookkeeping was missed». Drift accumulates between sessions; surfaces randomly at next session. Register's `query_register.ps1 --affected-by-milestone` heuristic surfaces expected updates so agent has explicit completeness check.

### 2.4 Cross-document drift

Pre-A'.4.5 risk RISK-004: amendment plans for one LOCKED spec (K-L3.1, A'.0.7) may not propagate consistently across all referencing documents. Register §3 requirement traceability matrix detects when REQ-K-Lxx changes don't propagate to verifying tests; §6 CAPA log records propagation gaps.

### 2.5 «I forgot to update X» pattern

Routine bookkeeping omission. Register validation surfaces this on every closure attempt via diff-against-register check: «files in git diff have no corresponding register update → fail». No mental-model dependency.

---

## 3. Classification model — Category × Tier × Lifecycle

Three orthogonal axes; each answers a different governance question.

| Axis | Answers | Discriminator |
|---|---|---|
| **Category** (A–J) | What is this content? | Template, voice, audience, organizing folder |
| **Tier** (1–5) | What governance regime? | Change authority, review cadence, approval gate |
| **Lifecycle** (8 states) | Can this be cited as authority right now? | Mutability, terminal state, cross-reference rules |

Three-axis classification is **not** fully orthogonal — strong correlations exist between Category-default Tier and allowed Lifecycle states. The allowed-combinations matrix (§3.4) encodes legal combinations; tooling validates.

### 3.1 Ten categories (A–J)

| Code | Name | Location | Examples |
|---|---|---|---|
| **A** | Architecture spec | `docs/architecture/` | KERNEL_ARCHITECTURE, MOD_OS_ARCHITECTURE, RUNTIME_ARCHITECTURE, ARCHITECTURE, ECS, EVENT_BUS, CONTRACTS, ISOLATION, THREADING, MODDING, FIELDS, GPU_COMPUTE |
| **B** | Methodology | `docs/methodology/` | METHODOLOGY, CODING_STANDARDS, DEVELOPMENT_HYGIENE, MAXIMUM_ENGINEERING_REFACTOR, PIPELINE_METRICS, TESTING_STRATEGY |
| **C** | Live tracker | `docs/` root | MIGRATION_PROGRESS, ROADMAP, IDEAS_RESERVOIR (post-A'.4.5 as index) |
| **D** | Brief | `tools/briefs/` | K0..K9, G0..G9, K_L3_1, A'.0.5/A'.0.7/A'.4.5 briefs |
| **E** | Discovery / closure / audit | `docs/audit/`, `docs/reports/`, `docs/prompts/`, `docs/benchmarks/`, `docs/learning/` | AUDIT_PASS_*, PERFORMANCE_REPORT_K7, NORMALIZATION_REPORT, M*_CLOSURE_REVIEW |
| **F** | Module-local | `src/**`, `tests/**`, `mods/**`, `native/**`, `assets/**`, `tools/**/MODULE.md` | per-folder README.md / MODULE.md files |
| **G** | Project meta | repo root, `docs/` root | README.md (root), docs/README.md, index documents, REGISTER.yaml, REGISTER_RENDER.md |
| **H** | i18n | `docs/` root | TRANSLATION_GLOSSARY, TRANSLATION_PLAN |
| **I** | Ideas Bank | `docs/ideas/` | Brainstorming, what-ifs, shelved alternatives, future feature speculation |
| **J** | Game Mechanics | `docs/mechanics/` | Race biology, magic schools, combat resolution rules, faction relations, crafting trees, balance numbers |

#### 3.1.1 Anti-overlap discrimination rules

When a document candidate could fit multiple categories:

**A vs J** — A describes how engine supports mechanics; J describes what mechanics are. Test: would another game engine implementing same mechanic produce different J document but similar A document? If yes → split clean.

**A vs F** — A is cross-module architectural authority; F is per-directory technical notes scope-limited to that directory.

**C vs G** — C tracks ongoing state with mutation cadence; G is static project orientation (READMEs, indices).

**D vs E** — D is prospective milestone instrument («what to do»); E is retrospective closure/audit artifact («what was done»).

**I vs C** — I is speculative not-yet-authoritative; C is ongoing operational state.

**I vs A** — I is «what if?»; A is authoritative. Promotion path: I matures → deliberation milestone → if accepted → A spec drafted; I entry transitions to SUPERSEDED with cross-reference.

**J Tier 1 vs J Tier 2** — Tier 1 J constrains architecture; Tier 2 J doesn't. Test: would changing this require architectural deliberation milestone?

### 3.2 Five tiers (1–5)

Each tier defines governance regime: change authority, review cadence, approval gate, agent navigation role.

#### 3.2.1 Tier 1 — Architectural authority

- **Documents**: LOCKED specs (KERNEL/MOD_OS/RUNTIME/MIGRATION_PLAN/GPU_COMPUTE/FIELDS/CONTRACTS/ISOLATION/THREADING/ECS/EVENT_BUS/ARCHITECTURE/MODDING/MOD_PIPELINE) + methodology corpus (METHODOLOGY/MAXIMUM_ENGINEERING_REFACTOR/CODING_STANDARDS/DEVELOPMENT_HYGIENE/TESTING_STRATEGY/PIPELINE_METRICS) + governance (FRAMEWORK/SYNTHESIS_RATIONALE) + foundational mechanics (Tier 1 J)
- **Change authority**: Crystalka via deliberation milestone + amendment plan + landing milestone (precedent: K-L3.1, A'.0.7, A'.4.5)
- **Review cadence**: `on-change+annual` — review on every change + annual full review (calendar Q1)
- **Approval gate**: explicit Crystalka lock in deliberation brief
- **Agent navigation role**: canonical reference surface. Agent reads Tier 1 during Phase 0 brief reads + architectural deliberation

#### 3.2.2 Tier 2 — Operational live (state of the project)

- **Documents**: LIVE trackers (MIGRATION_PROGRESS, ROADMAP, IDEAS_RESERVOIR-as-index, PHASE_A_PRIME_SEQUENCING) + governance meta (REGISTER, REGISTER_RENDER, VALIDATION_REPORT, BYPASS_LOG) + live closure reports (PERFORMANCE_REPORT_K7, NORMALIZATION_REPORT, CPP_KERNEL_BRANCH_REPORT, NATIVE_CORE_EXPERIMENT) + G-category indices (README.md, docs/README.md, folder indices) + J-category tunable mechanics
- **Change authority**: executor session per milestone closure protocol (auto-update); Crystalka for strategic adjustments
- **Review cadence**: `on-closure+quarterly` — review on every closure write + quarterly drift review
- **Approval gate**: part of closure verification (existing MIGRATION_PROGRESS closure protocol; extended in METHODOLOGY §12.7)
- **Agent navigation role**: «what's happening now?» entry point. Read during Phase 0 milestone orientation

#### 3.2.3 Tier 3 — Milestone instruments

- **Documents**: All `tools/briefs/*.md` + closure reports + amendment plans + verification logs
- **Change authority** depends on Lifecycle state:
  - AUTHORED → revisable via patch brief (precedent: K9_BRIEF_REFRESH_PATCH)
  - EXECUTED → immutable contract section; only «Lessons learned» appendable
  - DEPRECATED/SUPERSEDED → cross-reference mandatory
- **Review cadence**: `on-status-transition` — Lifecycle Status transition only; no calendar cadence
- **Approval gate**: Status field updates + amendment-plan-shaped revisions
- **Agent navigation role**: historical record + contract for in-flight milestones. Most active tier (every milestone execution adds 1+ entries; every closure transitions AUTHORED → EXECUTED)

#### 3.2.4 Tier 4 — Module-local

- **Documents**: src/ READMEs + tests/ READMEs + mods/ READMEs + native/ MODULE.mds + assets/scenes/README.md + tools/**/MODULE.md
- **Change authority**: updated as part of any commit touching the module's source
- **Review cadence**: `on-source-commit+quarterly` — with source commits + quarterly Phase-led sweep (precedent: A'.0.5 Phase 6)
- **Approval gate**: grouped with source commit (no separate gate)
- **Agent navigation role**: just-in-time context for that specific module. Largest tier but lowest per-entry cognitive cost — most fields default-populated mechanically

Tier 4 sub-division (4a/4b/4c per src/tests/mods/native split) deferred to future A'.4.5.X micro-milestone if quarterly sweep economics differ. Initially single bucket (YAGNI).

#### 3.2.5 Tier 5 — Speculative

- **Documents**: `docs/ideas/*.md` (Category I individual idea files)
- **Change authority**: anyone (Crystalka primary; agent may author with `proposed_by: <session_id>` field)
- **Review cadence**: `none` — no scheduled cadence; periodic harvest passes only
- **Approval gate**: none for entry; deliberation milestone for promotion-out
- **Agent navigation role**: «what hasn't been decided yet?» Agent reads when scoping future work or checking whether architectural proposal already considered-and-rejected
- **Lifecycle restriction**: STALE state forbidden on Tier 5 — ideas don't go stale. Allowed lifecycle: Draft, Live, DEPRECATED, SUPERSEDED. Friction-free brainstorming.

Why Tier 5 needed: ideas cannot fit Tier 1 (not authority), Tier 2 (not state), Tier 3 (not instrument), or Tier 4 (not local). Explicit «not authoritative» tier critical for agent navigation: agent querying «what does the project say about X?» must differentiate authoritative vs aspirational.

### 3.3 Eight lifecycle states

| State | Description | Mutability |
|---|---|---|
| **Draft** | Authored but not finalized; revisable freely | Full mutation |
| **Live** | Actively maintained; mutable on every relevant milestone | Full mutation per closure |
| **LOCKED** | Change authority via formal amendment milestone only | Restricted mutation |
| **EXECUTED** | Brief that has been run; historical immutable | Only «Lessons learned» appendable |
| **AUTHORED** | Brief authored, awaits execution; revisable via patch brief | Patch-brief mutation pattern |
| **DEPRECATED** | Superseded by successor; retained for historical context | Read-only with cross-reference |
| **SUPERSEDED** | Replaced by newer version of same logical document | Read-only with cross-reference |
| **STALE** | Known out-of-date; awaits update or formal archive | Surfaced by audit; not steady state |

#### 3.3.1 Transition matrix

Allowed transitions (any not listed = forbidden; tooling validation blocks):

```
Draft → Live           (Tier 2 documents finalized for active use)
Draft → LOCKED         (Tier 1 documents reach first stable version)
Draft → AUTHORED       (Tier 3 brief authoring complete)
Draft → EXECUTED       (Tier 3 closure report authored)

Live → STALE           (audit flags review overdue)
Live → DEPRECATED      (live tracker retired; example: PHASE_A_PRIME_SEQUENCING absorbed by MIGRATION_PLAN v1.1)
Live → SUPERSEDED      (rare; live tracker replaced by new structure)

LOCKED → SUPERSEDED    (amendment milestone produces new version)
LOCKED → DEPRECATED    (architectural retirement; rare — example: K8.2 v1 brief)

AUTHORED → EXECUTED    (execution session closes milestone)
AUTHORED → DEPRECATED  (brief abandoned without execution)
AUTHORED → SUPERSEDED  (brief replaced by amendment; example: K8.2 v1 → K8.2 v2)

EXECUTED → DEPRECATED  (historical brief no longer reflects authoritative state)
EXECUTED → (no further transitions; immutable contract)

STALE → Live           (review completed, document brought up to date)
STALE → DEPRECATED     (review determines obsolete)
STALE → SUPERSEDED     (review produces replacement)

DEPRECATED → (terminal; no transitions out)
SUPERSEDED → (terminal; no transitions out)
```

#### 3.3.2 Mandatory cross-references on terminal states

- **DEPRECATED** entries must have `deprecated_by: <successor_id>` populated (else validation fails)
- **SUPERSEDED** entries must have `superseded_by: <successor_id>` populated
- Bidirectional integrity enforced: target document's `supersedes`/`deprecates` list must include this ID
- Validation rejects one-sided supersession references

### 3.4 Allowed-combinations matrix

| Category | Default Tier | Allowed Lifecycle states | Notes |
|---|---|---|---|
| A | 1 | LOCKED, SUPERSEDED, Draft (pre-lock only) | Tier 1 dominant |
| B | 1 | LOCKED, SUPERSEDED, Draft | Same as A |
| C | 2 | Live, STALE | STALE flagged by audit |
| D | 3 | AUTHORED, EXECUTED, DEPRECATED, SUPERSEDED | Brief-specific |
| E | 3 | EXECUTED, DEPRECATED | Historical immutable on creation |
| F | 4 | Live, STALE | Updated with source commits |
| G | 2 | Live | README and index files |
| H | 2 (or 3) | Live, EXECUTED | TRANSLATION_PLAN EXECUTED post-campaign; GLOSSARY Live |
| I | 5 | Draft, Live, DEPRECATED, SUPERSEDED | No STALE — ideas don't stale |
| J | 1 OR 2 | LOCKED + SUPERSEDED + Draft (if Tier 1); Live + STALE (if Tier 2) | Per-document tier field |

#### 3.4.1 Forbidden combinations

Tooling rejects without `special_case_rationale`:

- Tier 1 + Lifecycle AUTHORED («LOCKED-кандидат limbo» not permitted)
- Tier 3 + Lifecycle LOCKED (briefs aren't LOCKED specs; promote to A/B if authoritative)
- Tier 5 + Lifecycle STALE (ideas don't stale)
- Tier 4 + Lifecycle LOCKED (module-local mutable per commit)
- Category D + Tier 1/2/4/5 (briefs are Tier 3)
- Category E + Tier 1/2/4/5 (closures are Tier 3)
- Category F + Tier 1/2/3/5 (module-local stays Tier 4)

#### 3.4.2 Special-case override mechanism

Documents whose Category × Tier × Lifecycle combination is outside default matrix require `special_case_rationale` field populated. Validation passes if rationale present; rejects if null on non-default combination.

Examples:
- `PHASE_A_PRIME_SEQUENCING.md` — Category A + Tier 2 + Live; rationale «mutable per phase closure, subordinate to MIGRATION_PLAN — not LOCKED architecture»
- `K_L3_1_AMENDMENT_PLAN.md`, `A_PRIME_0_7_AMENDMENT_PLAN.md` — Category A + Tier 3 + EXECUTED; rationale «amendment plans behave like briefs»
- `IDEAS_RESERVOIR.md` (post-A'.4.5) — Category C + Tier 2 + Live; informational note (no override): «functions as index for Category I folder docs/ideas/»

#### 3.4.3 Per-document tier for Category J

Category J uniquely splits into foundational (Tier 1) vs tunable (Tier 2). Per-document `tier` field directly populated. Tier 1 J entries include `constrains: [DOC-A-XXX]` cross-references to architecture surfaces.

---

## 4. Seven register sections

Each section answers a distinct agent query pattern. Sections §1, §2, §5 live as fields within per-document entries (document-scoped). Sections §3, §4, §6, §7 are global top-level collections (cross-document).

### 4.1 Document Control (ISO 9001 7.5 + DO-178C SCI)

Per-document control fields: `id`, `path`, `title`, `category`, `tier`, `lifecycle`, `special_case_rationale`, `is_meta_entry`, `meta_role`, `owner`, `version`, `first_authored`, `last_modified`, `last_modified_commit`, `content_language`.

ISO 9001 Document Control clause 7.5 contributes the ownership/version/status field semantics; DO-178C SCI (Software Configuration Index) contributes the configuration-tracking framing. Adapted: ISO 9001 7.5 assumes business org with dedicated document control officer; project has Crystalka as sole owner initially (schema supports future contributor onboarding without restructuring).

### 4.2 Architecture Case (ISO 26262 Safety Case re-purposed)

Per-document architectural decision rationale: `architecture_case.decisions` (list of K-Lxx invariants anchored here) + `architecture_case.rationale_anchors` (IDs of briefs/sessions where decisions were made).

ISO 26262 Safety Case = structured argumentation showing system meets safety requirements with decision rationale documented end-to-end. Re-purposed as «Architecture Case» = structured argumentation showing architectural decisions are coherent. Same structural device; different domain (architectural integrity demonstration for «без костылей десятилетие horizon» commitment vs safety-integrity certification).

Project already does decision rationale documentation exceptionally well (K8.0 Solution A brief, K-L3.1 bridge formalization brief, A'.0.7 deliberation brief). Register formalizes the reference structure: «which brief justifies K-L7?» becomes agent-queryable.

### 4.3 Requirement → Test Traceability (DO-178C)

Global `requirements:` collection. Each requirement entry has: `id`, `title`, `source_document`, `source_section`, `requirement_text`, `verified_by` (list of verification artifacts with file + test_method + verification_type), `verification_status` (PENDING/PARTIAL/VERIFIED/FAILED), `verification_date`, `verification_milestone`.

DO-178C traceability requires bidirectional cross-references from requirements to verifying tests. Project supports bidirectional via cross-references but primary direction is agent-query («what verifies K-L7?»).

Verification types extended beyond aerospace «behavioral/structural» to project context:
- `behavioral` — test method asserts behavioral contract
- `native_equivalence` — native selftest scenario asserts equivalence with managed counterpart
- `empirical_evidence` — measurement document records performance/property
- `code_inspection` — code structure matches requirement (manual or analyzer-verified)
- `spec_inspection` — spec document declares requirement (LOCKED status verification)
- `build_evidence` / `build_configuration` — build configuration enforces requirement (e.g., CMAKE_CXX_STANDARD)
- `tooling_evidence` — tooling validates compliance

K-L1..K-L11, Q-A07-6, Q-A45-X5, and future M-Lxx / G-Lxx / L-Lxx invariants are register requirement entries. Tracing «which test verifies K-L11?» becomes a YAML query, not a search across 4 files.

### 4.4 Architectural Risk Analysis (IEC 61508 HARA re-purposed)

Global `risks:` collection. Each risk entry has: `id`, `title`, `likelihood` (5-tier: Low / Medium-Low / Medium / Medium-High / High), `impact` (4-tier: Low / Medium / High / Critical), `risk_type` (Technical / Architectural / Methodological / Operational / External), `status` (ACTIVE / RESIDUAL / CLOSED / REALIZED / ACCEPTED), `affected_documents`, `mitigation` (strategy + artifact + status), `history`.

IEC 61508 HARA (Hazard and Risk Analysis) provides systematic identification + likelihood/severity assessment + mitigation tracking. Re-purposed as «Architectural Risk Analysis» — same structural device anchored on architectural integrity rather than safety integrity.

Risk types:
- **Technical** — specific implementation challenge (e.g., P/Invoke marshalling correctness)
- **Architectural** — decision-level risk affecting structural integrity (e.g., component conversion scope)
- **Methodological** — process-level risk (e.g., amendment plan completeness gap pattern)
- **Operational** — environment / tooling risk (e.g., testhost.exe file lock incidents)
- **External** — dependency risk (e.g., Vulkan driver compatibility)

Initial 14-entry R-skeleton authored at A'.4.5 closure per Crystalka direction 2026-05-12.

### 4.5 Internal Audit Schedule (ISO 9001 9.2)

Per-document audit fields: `review_cadence` enum + `last_review_date` + `last_review_event` + `next_review_due` + `reviewer`. Top-level `audit_calendar` block tracks anchor dates (Tier 1 annual review, Tier 2 quarterly reviews, Tier 4 Phase-led sweep next-scheduled).

Per-tier cadence:

| Tier | Cadence enum | Trigger | Calendar anchor |
|---|---|---|---|
| 1 | `on-change+annual` | every change + Q1 annual | annual Q1 of each year |
| 2 | `on-closure+quarterly` | every closure write + quarterly drift | Q1/Q2/Q3/Q4 |
| 3 | `on-status-transition` | Lifecycle Status transition | none |
| 4 | `on-source-commit+quarterly` | source commits + Phase-led sweep | approximately quarterly |
| 5 | `none` | no scheduled cadence | none |

Calendar-anchored reviews: STALE state triggers when `next_review_due < today` without intervening review event. Validation surfaces STALE as advisory warning (not error) in VALIDATION_REPORT.md.

ISO 9001 9.2 typically requires independent auditors. Project has Crystalka self-audit + tooling validation. «Independent» property achieved via **construction-by-rationale** (Phase A' lesson) — tooling validates schema invariants without depending on subjective review.

### 4.6 Corrective Action Log — CAPA (DO-178C Problem Reports + ISO 9001 10.2)

Global `capa_entries:` collection. Each CAPA entry has: `id`, `opened_date`, `closure_status` (OPEN / CLOSED), `trigger`, `affected_documents`, `root_cause`, `immediate_action`, `corrective_action`, `effectiveness_verification` (method + date + commit + outcome), `lessons_learned_reference`.

Joint anchor: ISO 9001 10.2 contributes 5-field structured shape (trigger / root cause / immediate action / corrective action / effectiveness verification); DO-178C Problem Reports contribute the «problem report» framing.

Adaptation: ISO 9001 CAPA closure requires independent verification of effectiveness. Project verification = «commit hash + date + cross-document drift audit clean» (precedent: K-L3.1 closure 2026-05-10 commit `0789bd4`).

Three retroactive CAPA entries authored at A'.4.5 closure from actual Phase A' history (K8.2 v2 reframing CLOSED, A'.0.7 audience inversion CLOSED, A'.0.5 count drift OPEN).

### 4.7 Audit Trail (FDA 21 CFR Part 11)

Global `audit_trail:` collection. Each event entry has: `id` (EVT-{date}-{symbolic}), `date`, `event`, `event_type` (deliberation_milestone / execution_milestone / amendment_landing / lifecycle_transition / governance_event), `documents_affected`, `commits` (range + key_commits), `governance_impact`, `cross_references` (capa_entries + lifecycle_transitions).

21 CFR Part 11 Audit Trail = chronological tamper-evident record. **Git log is already tamper-evident chronological audit trail**. Register §7 references git log via per-document `audit_trail_query: "git log --follow <path>"` boilerplate; explicit per-event entries in global `audit_trail:` only for **governance-significant events** (milestone closures, amendment landings, lifecycle transitions).

This avoids duplicating git log content while preserving register's narrative layer (e.g., «commits 27523ac..4e332bb constituted A'.0.5 reorganization» as semantic-level annotation that git log doesn't carry).

21 CFR Part 11 requires retention policy; git history retention is forever (no archival ceremony). 21 CFR Part 11 requires electronic signatures with non-repudiation; git commit authorship (author + date + signed if GPG configured) = de-facto electronic signature sufficient at project scale.

---

## 5. ID conventions (stable across schema versions)

```
DOC-{category-letter}-{counter|symbolic}
  Examples:
    DOC-A-KERNEL                 — symbolic for major specs (Architecture)
    DOC-A-001                    — numeric for non-symbolic A anchors
    DOC-D-K9                     — symbolic matches brief name (Briefs)
    DOC-D-K_L3_1_BRIDGE          — symbolic with underscores for compound names
    DOC-F-SRC-CORE               — directory-path-flattened (Module-local)
    DOC-I-MAGIC-SCHOOL-CONCEPT   — symbolic slug (Ideas Bank)
    DOC-J-COMBAT-RESOLUTION      — symbolic slug (Game Mechanics)

REQ-{lock-id}
  Examples: REQ-K-L11, REQ-K-L7, REQ-M-L1 (future), REQ-Q-A07-6 (Q-lock as requirement)

RISK-{counter:03d}
  Examples: RISK-001, RISK-015

CAPA-{date}-{symbolic-trigger}
  Examples: CAPA-2026-05-09-K8.2-V2-REFRAMING, CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION

EVT-{date}-{symbolic-event}
  Examples: EVT-2026-05-10-K-L3.1-CLOSURE, EVT-2026-05-10-A_PRIME_0_5-REORG
```

**ID rules**:
- IDs **never reused** (deleted entries leave gap; new entries get next counter)
- Symbolic IDs preferred for D / I / J / CAPA / EVT (human-recognizable)
- Numeric IDs for A / B / E / F / G / H / RISK when no natural symbolic anchor
- ID immutable once assigned (path changes don't change ID; ID is the stable cross-reference)
- Conventions stable across schema versions (per §7); schema MAJOR bump may extend conventions, cannot break existing IDs

---

## 6. Post-session update protocol

The most load-bearing element of A'.4.5. Without enforcement, register degrades into stale artifact (RISK-010 explicitly tracked).

### 6.1 Protocol definition

Every agent execution session that:
- Closes a milestone, OR
- Makes architectural decisions, OR
- Modifies any document in register scope

...**must** execute the following before commit closure:

```
1. Identify all documents modified during session (git status / git diff)
2. For each modified document:
   a. If has register entry: update entry's last_modified, last_modified_commit,
      lifecycle (if transitioning), governance_events (if applicable)
   b. If new: create entry with required fields populated
   c. If renamed/moved: update entry's path; update all cross-references
   d. If deleted: transition to DEPRECATED with rationale, OR remove if Draft never finalized
3. If milestone is closure event:
   a. Append entry to audit_trail collection (EVT-{date}-{symbolic-event})
   b. Update affected documents' governance_events lists
   c. If CAPA opened during milestone: create CAPA entry
   d. If risks identified or status-changed: update risks collection
   e. If requirements added: create requirement entries
4. Run sync_register.ps1 --validate
5. If validation passes: include REGISTER.yaml updates in milestone closure commit
6. If validation fails: halt closure; surface validation errors to Crystalka
```

### 6.2 Enforcement mechanism

**Strict gate** via manual invocation in closure protocol at A'.4.5; pre-commit hook upgrade deferred to A'.4.5.bis or A'.9 analyzer integration:

```powershell
# At milestone closure, before final commit:
./tools/governance/sync_register.ps1 --validate
# Exit code 0 → proceed with commit
# Exit code 1 → halt; resolve validation errors; re-validate
# Exit code 2 → tooling failure; report to Crystalka
```

**Strict vs advisory split**:

*Strict (blocks commit)*:
- Document-changed-by-milestone enforcement (files in git diff have no register update)
- Cross-reference integrity violations (dangling IDs, missing terminal-state references)
- Schema integrity violations (unknown enum values, missing required fields)
- Allowed-combinations matrix violations without `special_case_rationale`

*Advisory (warning only)*:
- STALE flag generation (`next_review_due < today`; doesn't block, surfaces in VALIDATION_REPORT.md)
- Tier 1 Russian-only document translation candidacy flag (per §9.3)
- Tier 3 brief AUTHORED > 30 days warning
- Bypass log review pending

Rationale: agent must update register entries for changed documents (strict — prevents drift); tooling shouldn't harass agent about future-dated reviews (advisory).

### 6.3 Bypass mechanism

Bypass via `git commit --no-verify` available **but logged** in `docs/governance/BYPASS_LOG.md` (Tier 2 Live). Every bypass creates an entry with timestamp + commit hash + author + rationale + follow-up plan. Validation surfaces «BYPASS_LOG.md has entries since last validation» as advisory for cleanup.

### 6.4 Closure protocol integration (METHODOLOGY §12.7 canonical)

Existing closure protocol extended:

```
1. Run final verification (existing): dotnet build, dotnet test, native selftest, F5 verification
2. Atomic commit with scope prefix (existing)
3. Update MIGRATION_PROGRESS.md (existing)
4. Update brief Status field (existing)
5. NEW: Update REGISTER.yaml entries for all documents touched during milestone
   - last_modified, last_modified_commit, lifecycle (if transitioning), governance_events
   - New documents: create entries with required fields
   - Renamed/moved/deleted: update path / supersession / deprecation cross-references
6. NEW: Append audit_trail entry for milestone closure (EVT-{date}-{symbolic-event})
7. NEW: Run sync_register.ps1 --validate
   - Must exit 0 before final commit
   - Bypass via --no-verify logged in BYPASS_LOG.md
```

MIGRATION_PROGRESS.md closure protocol section cross-references METHODOLOGY §12.7 as canonical.

### 6.5 Agent self-check pattern

Near end of milestone execution:

```
Agent: «I modified files [list]. Let me query register for what I should have updated:»

$ ./tools/governance/query_register.ps1 --affected-by-milestone <current-milestone-id>

Returns: documents register tracks as «typically affected by this milestone type» (heuristic)

Agent: «Comparing actual modifications to register expectations:
        - Match: [X documents touched as expected]
        - Surprise: [Y not predicted — need register entry update]
        - Missing: [Z predicted but not touched — flag for review]
       Updating REGISTER.yaml for all modified documents...»
```

The `--affected-by-milestone` query is **heuristic, not strict** — surfaces what similar milestones touched as completeness check, not constraint.

---

## 7. Schema versioning evolution

Schema v1.0 locked at A'.4.5 closure. Anticipated 1-2 amendments in first year; more thereafter.

### 7.1 Versioning rules (semantic)

`schema_version` field at REGISTER.yaml top level. Bump rules:

- **MAJOR (1.0 → 2.0)** — breaking change requiring entry migration. Field renamed/removed; ID convention changed in incompatible way; required field added without default; collection renamed/restructured
- **MINOR (1.0 → 1.1)** — additive non-breaking. New optional field, new category, new lifecycle state, new top-level collection, new enum value
- **PATCH (1.0 → 1.0.1)** — clarifications without schema change. Validation rule wording refinement; tooling bug fixes that don't change schema interpretation

### 7.2 Amendment milestone protocol

Schema amendment is Tier 1 LOCKED amendment following K-L3.1 / A'.0.7 / A'.4.5 precedent:

1. Deliberation milestone (brief + Q-lock pass)
2. Amendment plan authoring (per-entry migration logic, tooling compatibility analysis)
3. Execution milestone:
   - MINOR: re-validate all entries with new schema; populate new optional fields where applicable
   - MAJOR: explicit migration step per entry; old field values transformed or archived; migration script runs before validation
4. CAPA entry opened recording trigger + rationale + effectiveness verification
5. Audit trail event appended (EVT-{date}-SCHEMA-VX-TO-VY-AMENDMENT)

### 7.3 Tooling compatibility behavior

`sync_register.ps1 --validate` compares YAML `schema_version` against tooling's expected version:

| YAML schema | Tooling expects | Behavior |
|---|---|---|
| 1.0.0 | 1.0.1 | Warn «tooling slightly newer; forward-compatible»; proceed |
| 1.0.1 | 1.0.0 | Warn «tooling slightly outdated»; proceed |
| 1.1 | 1.0 | **Error**: «schema v1.1 requires tooling v1.1+»; halt |
| 1.0 | 1.1 | Warn «tooling forward-compatible to v1.0»; proceed |
| 2.0 | 1.X | **Error**: «MAJOR version mismatch; migration required»; halt |
| 1.X | 2.0 | **Error**: «register pre-migration; run migration script»; halt |

Forward-compatible within MAJOR; halts across MAJOR.

### 7.4 Schema version history

| Version | Date | Closure milestone | Change |
|---|---|---|---|
| 1.0 | 2026-05-12 | A'.4.5 closure | Initial schema lock |

Table is append-only; old versions never removed. Tooling validates table presence + monotonic version progression.

### 7.5 Schema-version vs register-version distinction

- `schema_version` — structure of YAML (changes only via amendment milestones)
- `register_version` — content of YAML (bumps on every closure write; minor for content, major if `schema_version` bumps)

Example: schema_version stays "1.0" for many closures while register_version bumps 1.0 → 1.1 → 1.2 → ...

---

## 8. Governance-over-governance (recursion)

REGISTER.yaml describes documents. REGISTER.yaml is a document. Therefore REGISTER.yaml describes itself.

### 8.1 Four meta-entries

At A'.4.5 closure, four meta-entries flagged with `is_meta_entry: true`:

| ID | Path | Category | Tier | Lifecycle | meta_role |
|---|---|---|---|---|---|
| DOC-G-REGISTER | docs/governance/REGISTER.yaml | G | 2 | Live | register_source_of_truth |
| DOC-A-FRAMEWORK | docs/governance/FRAMEWORK.md | A | 1 | LOCKED | register_specification |
| DOC-A-SYNTHESIS_RATIONALE | docs/governance/SYNTHESIS_RATIONALE.md | A | 1 | LOCKED | register_provenance |
| DOC-G-REGISTER_RENDER | docs/governance/REGISTER_RENDER.md | G | 2 | Live | register_rendered_derivative |

VALIDATION_REPORT.md and BYPASS_LOG.md are operational artifacts (regular Tier 2), not meta-entries.

### 8.2 Special validation rules for meta-entries

`sync_register.ps1 --validate` applies extra rules:

1. **Path constraint** — `meta_entry.path` must exist in `docs/governance/`. Validation fails if outside (prevents accidental meta-promotion)
2. **Self-tracking exemption** — meta_entry `last_modified_commit` does NOT require external git-log resolution (chicken-and-egg at initial creation)
3. **DEPRECATED transition constraint** — meta_entry cannot transition to DEPRECATED without successor declared; specifically REGISTER.yaml DEPRECATED requires `deprecated_by` pointing to new register URL/path
4. **Schema version coupling** — FRAMEWORK.md and SYNTHESIS_RATIONALE.md schema-version reference must match REGISTER.yaml `schema_version`; schema bumps trigger meta-entry version updates
5. **Cadence constraint** — DOC-G-REGISTER has `review_cadence: on-every-milestone-closure`; STALE flag fires if any milestone closes without updating REGISTER.yaml — load-bearing enforcement for post-session protocol

### 8.3 Bootstrap procedure at A'.4.5 closure

At first creation, REGISTER.yaml cannot have its own commit hash in `last_modified_commit` (chicken-and-egg). Resolution: `git commit --amend` pattern.

```powershell
# Author REGISTER.yaml with placeholder
last_modified_commit: "PENDING-INITIAL"

# Stage closure files + commit
git add docs/governance/* tools/governance/* docs/methodology/METHODOLOGY.md ...
git commit -m "feat(governance): A'.4.5 closure — Document Control Register operational"

# Capture hash + update REGISTER.yaml
$commitHash = git rev-parse HEAD
(Get-Content REGISTER.yaml) -replace "PENDING-INITIAL", $commitHash | Set-Content REGISTER.yaml

# Amend (single clean commit)
git add docs/governance/REGISTER.yaml
git commit --amend --no-edit
```

Single closure commit with REGISTER.yaml containing its own hash. Meta-entry exemption (Rule 2) means tooling doesn't fail during pre-validation.

---

## 9. Language scope

### 9.1 Register entries English-mandatory

Register entries (schema fields, enum values, free-text title/notes/rationale fields) are **English-language**. Rationale:
- Schema as machine-readable contract — English matches code, tooling, OSS readability
- Agent-primary use case — agent operates fluently in English by default
- Mixed-language registers complicate query patterns (case sensitivity, transliteration, search-by-substring)

**Exception**: `title` field MAY include original-language title in parentheses if document's actual title is Russian. Other free-text fields English even when describing Russian-content documents, except quoting Crystalka directives verbatim.

### 9.2 content_language metadata field

Per-document `content_language` enum: `en` / `ru` / `mixed`.

- `en` — clear English-only document
- `ru` — clear Russian-only document (legacy or i18n campaign artifact)
- `mixed` — substantive content in both languages (Live trackers, briefs quoting Crystalka extensively)

Forward-compatible expansion (schema MINOR bump) for future multilingual contributors: new ISO 639-1 codes added to enum; existing entries remain valid.

### 9.3 Advisory validation — Tier 1 Russian-only flag

`sync_register.ps1 --validate` includes advisory warning (not error) for Tier 1 documents with `content_language: ru`:

> Warning: <id> at <path> is Tier 1 LOCKED but content_language=ru.
> Tier 1 documents are architectural authority surfaces; English content preferred for agent-primary readership. Consider translation candidacy.

Tier 1 mixed not flagged (intentional design — METHODOLOGY has Russian footnotes quoting Crystalka). Tier 2-5 Russian not flagged (legacy preservation acceptable).

---

## 10. Falsifiable claims

This framework is falsified if any of:

1. **Navigation aid claim fails**: agent's session-mode pipeline becomes slower or less reliable with register than without (measurable via task-level metrics in PIPELINE_METRICS over 6-12 months post-A'.4.5)
2. **Decade-horizon scalability fails**: register schema doesn't accommodate genuine evolution needs without disruptive MAJOR bumps every 6 months (vs anticipated 1-2 MINOR amendments + occasional MAJOR within first year, then stabilization)
3. **Construction-by-rationale fails**: tooling validation produces false positives or false negatives at rate that requires constant `--no-verify` bypass usage (BYPASS_LOG.md grows monotonically without successful follow-up cleanup)
4. **Post-session protocol staleness**: register itself becomes stale despite Q-A45-X5 enforcement; sync_register --validate run yields STALE flags on >10% of Tier 1 documents at any sampled point
5. **Cross-tier transitions blocked**: real workflows require transitions that the allowed-combinations matrix forbids, accumulating `special_case_rationale` overrides into 20%+ of register entries (signaling matrix wrong, not just edge-case-tolerant)

Falsification mechanism: PIPELINE_METRICS records per-milestone metrics; quarterly audit reviews register efficiency. If falsified, A'.4.5.X micro-milestones or A'.9 analyzer milestone re-deliberates governance shape.

---

## 11. Boundaries of applicability

This framework is intentionally bespoke for:

- **Solo developer + AI agent pipeline** — single owner; AI as primary register reader/writer
- **Session-mode boundary** (per METHODOLOGY §2.1.1) — deliberation sessions + execution sessions both touch register
- **Decade-horizon planning** — schema designed for 10+ year evolution
- **Open-source-separately property** — framework portable as standalone governance device

Framework is **less appropriate** for:

- Teams with dedicated document-control officers (ISO 9001 7.5 ceremony adapts but reduces value-add)
- Sub-month iteration cycles (Tier 1 amendment milestone overhead doesn't amortize)
- Strict regulatory compliance contexts requiring electronic signatures, formal e-signature ceremony, or certified audit trails (DO-178C / 21 CFR Part 11 ceremony deselected; project's tolerance of git log as audit trail wouldn't satisfy certified contexts)

If project scope or pipeline configuration changes substantially, framework adaptability is itself a falsifiable claim — A'.X.Y deliberation re-deliberates as needed.

---

## 12. Change history

| Version | Date | Closure | Change |
|---|---|---|---|
| 1.0 | 2026-05-12 | A'.4.5 | Initial framework lock from A'.4.5 deliberation closure |

---

## 13. See also

- [SYNTHESIS_RATIONALE.md](./SYNTHESIS_RATIONALE.md) — 5-standard provenance for selected elements
- [REGISTER.yaml](./REGISTER.yaml) — operational SoT
- [REGISTER_RENDER.md](./REGISTER_RENDER.md) — human-readable rendered derivative
- [tools/governance/MODULE.md](../../tools/governance/MODULE.md) — PowerShell tooling module index
- [docs/methodology/METHODOLOGY.md](../methodology/METHODOLOGY.md) §12 — register integration into closure protocol
- [docs/MIGRATION_PROGRESS.md](../MIGRATION_PROGRESS.md) — Phase A' history including A'.4.5 closure
- [docs/architecture/PHASE_A_PRIME_SEQUENCING.md](../architecture/PHASE_A_PRIME_SEQUENCING.md) — Phase A' position of A'.4.5

A'.4.5 deliberation artifacts (Tier 3 design + execution-context):
- [tools/briefs/A_PRIME_4_5_DOCUMENT_CONTROL_REGISTER_BRIEF.md](../../tools/briefs/A_PRIME_4_5_DOCUMENT_CONTROL_REGISTER_BRIEF.md) — deliberation brief (design tier)
- [tools/scratch/A_05/A_PRIME_4_5_DELIBERATION_CLOSURE.md](../../tools/scratch/A_05/A_PRIME_4_5_DELIBERATION_CLOSURE.md) — deliberation closure with 23 Q-locks (design tier)
- Pass 1-5 execution-context briefs in `tools/briefs/A_PRIME_4_5_PASS_*.md`

Source standards (background reading — not authoritative for project, contextual only):
- DO-178C — Software Considerations in Airborne Systems (RTCA / EUROCAE)
- ISO 9001:2015 — Quality Management Systems
- ISO 26262 — Functional Safety — Automotive
- IEC 61508 — Functional Safety — General
- FDA 21 CFR Part 11 — Electronic Records, Electronic Signatures
