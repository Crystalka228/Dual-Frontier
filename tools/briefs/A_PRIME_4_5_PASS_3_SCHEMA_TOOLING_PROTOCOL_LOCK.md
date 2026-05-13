---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_4_5_PASS_3
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_4_5_PASS_3
---
# A'.4.5 Pass 3 — Schema + Tooling + Post-Session Protocol Lock (Execution-Ready Brief)

**Status**: LOCKED 2026-05-12 (A'.4.5 deliberation Pass 3, Q9 + Q10 + Q-A45-X5)
**Type**: Execution-ready brief for:
- Authoring `docs/governance/FRAMEWORK.md` §4 (seven sections detail) + §6 (post-session protocol) prose
- Authoring `docs/governance/REGISTER.yaml` schema v1.0 structure
- Implementing `tools/governance/sync_register.ps1` + `query_register.ps1` + `render_register.ps1`
- Specifying closure protocol integration with METHODOLOGY §12.7

**Authority subordinate to**: A'.4.5 deliberation closure document
**Consumes**: Pass 1 (Q4 standards) + Pass 2 (classification model) — these define what schema represents; this Pass 3 defines how it's represented and enforced
**Audience**: Opus session authoring FRAMEWORK + REGISTER.yaml; Claude Code execution session implementing tooling

---

## Lock summary

Three integrated lockes constituting register's operational layer:

- **Q9 — YAML schema design**: 5 top-level collections (documents + 4 globals); per-document entry with 7 field-groups + cross-refs; ID conventions stable across schema versions; minimal frontmatter mirror (~7 fields)
- **Q10 — Tooling design**: three-tool PowerShell suite (sync_register / query_register / render_register); validation rules enumerated; scope exclusions via separate YAML; tooling scripts tracked via parent module README, not as register entries
- **Q-A45-X5 — Post-session update protocol**: mandatory for every execution session closing milestone or modifying register-scope documents; strict pre-commit gate via sync_register.ps1 --validate; bypass available via --no-verify but logged in BYPASS_LOG.md; integrates with existing closure protocol

Critical architectural inversion locked here: **schema agent-primary** (optimized for query patterns), **human readability derivative** via render_register tool. This is the load-bearing inversion that makes register navigation tool, not just governance audit artifact.

---

## 1. YAML schema design (Q9)

### 1.1 Top-level structure

```yaml
# docs/governance/REGISTER.yaml
# DualFrontier Document Control Register
# Schema version locked at A'.4.5; register version bumps on every closure write

schema_version: "1.0"            # locked at A'.4.5; future amendments bump (Q-A45-X1)
register_version: "1.0"           # bumps on every closure write
last_modified: "2026-05-XX"
last_modified_commit: "<hash>"    # tooling auto-fills on write
last_modified_by: "<session_id>"  # agent session identifier OR "Crystalka"

# === Audit calendar (Q13 lock anchor) ===
audit_calendar:
  tier_1_annual_review_date: "2027-Q1"
  tier_2_quarterly_review_dates:
    - "2026-Q3"
    - "2026-Q4"
    - "2027-Q1"
    - "2027-Q2"
  tier_4_phase_led_sweep_dates:
    next_scheduled: "TBD — after A'.5 K8.3 closure"
    cadence: "approximately quarterly, or at next major Phase boundary"

# === 5 top-level collections ===
documents: [...]       # §1 Document Control + §2 Architecture Case + §5 Audit Schedule
                       # (per-doc entries; §1/§2/§5 are fields within each entry)
requirements: [...]    # §3 Traceability — global requirement collection
risks: [...]           # §4 Risk Register — global risk collection
capa_entries: [...]    # §6 CAPA — global corrective action collection
audit_trail: [...]     # §7 Governance events — global chronological collection
```

**Design rationale for collection layout**:
- `documents` is per-document; §1/§2/§5 fields are part of each entry because they're document-scoped
- `requirements`, `risks`, `capa_entries`, `audit_trail` are **global** because they're cross-document by nature — risk affects multiple docs, CAPA spans multiple docs, requirement is verified by multiple test files, event touches multiple docs
- Cross-references from documents к global collections via ID (`risks_referenced: [RISK-001, RISK-007]`), not nested objects — preserves single source of truth (SoT)

### 1.2 Document entry shape (full schema)

```yaml
documents:
  - # === §1 Document Control fields ===
    id: DOC-A-001                          # canonical register ID per §5 conventions
    path: docs/architecture/KERNEL_ARCHITECTURE.md
    title: "DualFrontier Kernel — Architecture & Roadmap"
    
    # Three-axis classification (per Pass 2)
    category: A                            # one of A-J
    tier: 1                                # one of 1-5
    lifecycle: LOCKED                      # one of 8 states
    
    # Optional: required if Category × Tier × Lifecycle outside default matrix
    special_case_rationale: null           # filled per Pass 2 §4.3
    
    # Meta-entry flag (Q-A45-X2)
    is_meta_entry: false                   # true only for REGISTER.yaml + FRAMEWORK.md + SYNTHESIS_RATIONALE.md + REGISTER_RENDER.md
    meta_role: null                        # populated if is_meta_entry: true
    
    # Ownership and versioning
    owner: Crystalka
    version: "1.5"
    first_authored: "2026-05-07"
    last_modified: "2026-05-10"
    last_modified_commit: "2df5921"
    
    # Language tracking (Q-A45-X3)
    content_language: en                   # enum: en | ru | mixed
    
    # === §2 Architecture Case fields ===
    architecture_case:
      decisions:                           # architectural locks anchored in this document
        - id: K-L1
          summary: "Native language: C++20"
        - id: K-L7
          summary: "Read-only spans + write batching"
        - id: K-L11
          summary: "Single NativeWorld backbone"
      rationale_anchors:                   # IDs of briefs/sessions where decisions were made
        - DOC-D-K8_0                       # K8.0 Solution A recording brief
        - DOC-D-K_L3_1_BRIDGE              # K-L3.1 bridge formalization
    
    # === §3 Traceability cross-reference ===
    requirements_authored:                 # requirement IDs first introduced in this document
      - REQ-K-L1
      - REQ-K-L7
      - REQ-K-L11
    
    # === §4 Risk Register cross-reference ===
    risks_referenced:                      # risk IDs affecting this document
      - RISK-001
      - RISK-005
    
    # === §5 Internal Audit Schedule fields ===
    review_cadence: "on-change+annual"     # enum: on-change+annual | on-closure+quarterly | on-status-transition | on-source-commit+quarterly | none
    last_review_date: "2026-05-10"
    last_review_event: "K-L3.1 bridge formalization 2026-05-10"
    next_review_due: "2027-05-10"
    reviewer: Crystalka
    
    # === §6 CAPA cross-reference ===
    capa_entries_referenced:
      - CAPA-2026-05-09-K8.2-V2-REFRAMING
    
    # === §7 Audit Trail integration ===
    audit_trail_query: "git log --follow docs/architecture/KERNEL_ARCHITECTURE.md"
    governance_events:                     # IDs of register-tracked events affecting this doc
      - EVT-2026-05-10-K-L3.1-CLOSURE
    
    # === Cross-references to register-wide entries (terminal state) ===
    supersedes: []
    superseded_by: []
    deprecates: []
    deprecated_by: []
```

**Field grouping rationale**: 7 field-groups map directly to 7 register sections (§1-§7); cross-references at bottom handle supersession/deprecation. Agent reading entry sees same structure as FRAMEWORK §4 explanation.

### 1.3 Requirement entry shape (§3 global collection)

```yaml
requirements:
  - id: REQ-K-L11
    title: "Single NativeWorld backbone — production storage"
    
    # Source declaration
    source_document: DOC-A-001             # KERNEL_ARCHITECTURE.md
    source_section: "Part 0, K-L11"
    requirement_text: |
      Production storage = NativeWorld single source of truth;
      ManagedWorld retained as test fixture only.
    
    # Verification artifacts (DO-178C traceability shape)
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
    
    verification_status: VERIFIED          # PENDING | PARTIAL | VERIFIED | FAILED
    verification_date: "2026-05-09"
    verification_milestone: "K8.2 v2 closure"
```

**Verification type enum**:
- `behavioral` — test method asserts behavioral contract
- `native_equivalence` — native selftest scenario asserts equivalence with managed counterpart
- `empirical_evidence` — measurement document records performance/property
- `code_inspection` — code structure matches requirement (manual or analyzer-verified)
- `spec_inspection` — spec document declares requirement (LOCKED status verification)
- `build_evidence` — build configuration enforces requirement (e.g., CMAKE_CXX_STANDARD)
- `tooling_evidence` — tooling validates compliance (e.g., sync_register.ps1 validation)

### 1.4 Risk entry shape (§4 global collection)

```yaml
risks:
  - id: RISK-001
    title: "Component struct refactor scope underestimated"
    
    # Taxonomy (Q11 lock from Pass 5)
    likelihood: Medium-High                # Low | Medium-Low | Medium | Medium-High | High
    impact: Medium                         # Low | Medium | High | Critical
    risk_type: Architectural               # Technical | Architectural | Methodological | Operational | External
    status: CLOSED                         # ACTIVE | RESIDUAL | CLOSED | REALIZED | ACCEPTED
    
    # Cross-references
    affected_documents:
      - DOC-A-001                          # KERNEL_ARCHITECTURE
      - DOC-D-K4                           # K4 brief
      - DOC-D-K8_2_V2                      # K8.2 v2 brief
    
    # Mitigation
    mitigation:
      strategy: "Incremental conversion (5-10 components per commit), tests verify each commit"
      mitigation_artifact: DOC-D-K4        # brief where mitigation strategy authored
      mitigation_status: APPLIED           # PROPOSED | APPLIED | PARTIAL | ACCEPTED | FAILED
    
    # History
    history:
      - date: "2026-05-07"
        event: "Risk surfaced at K0 brief authoring"
      - date: "2026-05-08"
        event: "Risk realized at K4 execution; mitigation worked (24 components × 7 batches)"
      - date: "2026-05-09"
        event: "Risk closed; K8.2 v2 verification 0789bd4"
```

### 1.5 CAPA entry shape (§6 global collection)

```yaml
capa_entries:
  - id: CAPA-2026-05-09-K8.2-V2-REFRAMING
    opened_date: "2026-05-09"
    closure_status: CLOSED                 # OPEN | CLOSED
    
    trigger: |
      K-L3 «без exception» framing surfaced as misalignment
      at K8.2 v2 closure verification.
    
    affected_documents:
      - DOC-A-001                          # KERNEL_ARCHITECTURE
      - DOC-A-002                          # MOD_OS_ARCHITECTURE
      - DOC-A-003                          # MIGRATION_PLAN
    
    root_cause: |
      K8.2 v1 brief authored 2026-05-08 against pre-K-L3.1 framing.
      Closure entry encoded universal-mandate reading.
    
    immediate_action: |
      Halt K-series progress; surface K-L3 wording for retroactive reformulation.
    
    corrective_action: |
      K-L3.1 bridge formalization deliberation milestone (A'.0) +
      amendment plan (Phase 4 deliverable) + amendment landing milestones (A'.1.K).
    
    effectiveness_verification:
      method: "Post-amendment cross-document audit per K-L3.1 §K.12.2"
      date_verified: "2026-05-10"
      verification_commit: "0789bd4"
      verification_outcome: |
        All «без exception» wording reformulated. KERNEL v1.3→v1.5;
        MOD_OS v1.6→v1.7; MIGRATION_PLAN v1.0→v1.1. Audit clean.
    
    lessons_learned_reference: DOC-D-K9_BRIEF_REFRESH_PATCH
```

### 1.6 Audit trail entry shape (§7 global collection)

```yaml
audit_trail:
  - id: EVT-2026-05-10-K-L3.1-CLOSURE
    date: "2026-05-10"
    event: "K-L3.1 bridge formalization closure"
    event_type: deliberation_milestone     # deliberation_milestone | execution_milestone | amendment_landing | lifecycle_transition | governance_event
    
    documents_affected:
      - DOC-A-001                          # KERNEL_ARCHITECTURE 1.4 → 1.5
      - DOC-A-002                          # MOD_OS 1.6 → 1.7
      - DOC-A-003                          # MIGRATION_PLAN 1.0 → 1.1
      - DOC-D-K_L3_1_BRIDGE                # bridge formalization brief
    
    commits:
      range: "2df5921..0789bd4"
      key_commits:
        - hash: "45d831c"
          summary: "K-L3.1 amendment plan landed"
    
    governance_impact: |
      Path α (unmanaged struct) + Path β (managed class via [ManagedStorage])
      formalized as first-class peers. IModApi v3 gains RegisterManagedComponent<T>
      for Path β (ships K8.4).
    
    cross_references:
      capa_entries:
        - CAPA-2026-05-09-K8.2-V2-REFRAMING
      lifecycle_transitions:
        - document: DOC-D-K_L3_1_BRIDGE
          from: AUTHORED
          to: EXECUTED
```

### 1.7 ID conventions (stable across schema versions)

```
DOC-{category-letter}-{counter|symbolic}
  Examples:
    DOC-A-001            — numeric for non-symbolic anchors (Architecture)
    DOC-A-KERNEL         — alternative symbolic for major specs
    DOC-D-K9             — symbolic matches brief name (Briefs)
    DOC-D-K_L3_1_BRIDGE  — symbolic
    DOC-F-COMPONENTS-PAWN — directory-path-flattened (Module-local)
    DOC-I-MAGIC-SCHOOL-CONCEPT — symbolic slug (Ideas Bank)
    DOC-J-COMBAT-RESOLUTION — symbolic slug (Game Mechanics)

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
- Numeric IDs for A / B / E / F / G / H / RISK (no natural symbolic anchor)
- ID immutable once assigned (path changes don't change ID; ID is the stable cross-reference)
- Conventions stable across schema versions (Q-A45-X1) — schema MAJOR bump may extend conventions; cannot break existing IDs

### 1.8 Frontmatter mirror shape (auto-generated)

```yaml
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next validation run
# 
# Last generated: 2026-05-12T14:32:00Z from REGISTER.yaml commit <hash>
register_id: DOC-A-001
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.5"
next_review_due: "2027-05-10"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-001
---

# DualFrontier Kernel — Architecture & Roadmap

(rest of document)
```

**Minimal-field design rationale**: frontmatter mirror is for «agent opens this file directly, wants quick orientation». 7 fields = most-queried subset:
- `register_id` for cross-reference
- `category` / `tier` / `lifecycle` for axis position
- `owner` / `version` for control
- `next_review_due` for STALE warning
- `register_view_url` for full entry navigation

Full entry data lives in REGISTER.yaml; frontmatter is read-only mirror.

---

## 2. Tooling design (Q10)

Tooling is **three-tool PowerShell suite**, not single sync script. Expansion from brief original «single sync_register.ps1» because navigation-primary use case requires query support, not just write/sync.

### 2.1 Tool 1 — `sync_register.ps1` (write-side sync + validation)

**Purpose**: keep REGISTER.yaml ↔ frontmatter mirrors in sync; validate schema integrity.

**Invocation modes**:

```powershell
# Validate only (no writes; pre-commit gate)
./tools/governance/sync_register.ps1 --validate

# Sync REGISTER.yaml → all frontmatter mirrors (after register changes)
./tools/governance/sync_register.ps1 --sync

# Sync + validate (default; both operations)
./tools/governance/sync_register.ps1
```

**Operations**:
1. Load REGISTER.yaml; parse schema using PowerShell YAML library (recommended: `powershell-yaml` module or `ConvertFrom-Yaml`)
2. For each document entry: locate .md file at entry's `path`; update YAML frontmatter (insert if absent, replace if present)
3. Validate per §2.2 validation rules
4. Output structured report to stdout + `docs/governance/VALIDATION_REPORT.md`
5. Exit codes:
   - `0` — all green
   - `1` — validation errors (commit blocked at gate)
   - `2` — write errors (file lock, permission, tooling failure)

### 2.2 Validation rules (full enumeration)

`sync_register.ps1 --validate` enforces:

**Path integrity**:
- Every register entry has matching .md file at `path` (file-exists check)
- Every .md file in scope (not in SCOPE_EXCLUSIONS.yaml) has register entry (no orphan documents)
- File path canonical (forward-slash, repo-relative; tooling normalizes Windows backslashes)

**Classification integrity (Pass 2 rules)**:
- Tier × Lifecycle combination matches allowed matrix OR `special_case_rationale` non-null
- Tier 1 + Lifecycle AUTHORED → reject (no «LOCKED-кандидат» limbo)
- Tier 3 + Lifecycle LOCKED → reject (briefs aren't LOCKED specs)
- Tier 5 + Lifecycle STALE → reject (ideas don't stale per Pass 2)
- Tier 4 + Lifecycle LOCKED → reject (module-local mutable per commit)
- Category D + Tier 1, 2, 4, or 5 → reject (briefs are Tier 3)
- Category E + Tier 1, 2, 4, or 5 → reject (closures are Tier 3)
- Category F + Tier 1, 2, 3, or 5 → reject (module-local Tier 4)

**Cross-reference integrity**:
- DEPRECATED entries have `deprecated_by` non-empty
- SUPERSEDED entries have `superseded_by` non-empty
- Bidirectional check: target document's `supersedes`/`deprecates` list includes this ID
- Risk IDs referenced in documents exist in risks collection
- CAPA IDs referenced in documents exist in capa_entries collection
- Requirement IDs referenced in documents exist in requirements collection
- Audit trail event IDs referenced in documents exist
- IDs unique across all collections (no duplicate DOC-A-001)

**ID convention compliance**:
- IDs match regex per category (e.g., `^DOC-A-\d{3}$|^DOC-A-[A-Z_]+$` for A; `^RISK-\d{3}$` for risks)

**Git integration**:
- `last_modified_commit` resolves in git (commit exists)
- Tier 3 brief with `lifecycle: AUTHORED`: warn if `last_modified_commit` > 30 days ago (likely stalled or transition forgotten)

**Audit cadence (advisory warnings, not errors)**:
- For Live entries: STALE flag set if `next_review_due < today` AND no review event recorded since `last_review_date`
- For Tier 1 Russian-only entries: flag for translation candidacy (Q-A45-X3 advisory)

**Meta-entry validation (Q-A45-X2)**:
- meta_entry.path must exist in `docs/governance/` directory
- meta_entry's last_modified_commit does NOT require external verification (register tracks itself)
- meta_entry cannot transition to DEPRECATED without successor register declared
- meta_entry's schema_version must match REGISTER.yaml's top-level schema_version

**Schema version compatibility (Q-A45-X1)**:
- Tooling expected schema version vs YAML schema_version:
  - Patch mismatch (1.0.0 vs 1.0.1): warn, proceed
  - Minor mismatch YAML newer: error «update tooling»; halt
  - Minor mismatch tooling newer: warn, proceed (forward-compatible to old schemas)
  - Major mismatch: error «schema migration required»; halt with pointer to amendment plan

**Schema integrity**:
- schema_version present and parseable
- All required fields per entry shape populated
- Enum values from allowed sets (category A-J, tier 1-5, lifecycle 8 states, etc.)

### 2.3 Out-of-scope files (SCOPE_EXCLUSIONS.yaml)

Documents NOT enrolled in register; not flagged as orphans:

```yaml
# tools/governance/SCOPE_EXCLUSIONS.yaml
# Documents NOT enrolled in register; not orphans.
schema_version: "1.0"

excluded_paths:
  - pattern: ".godot/**"
    rationale: "Godot generated artifacts"
  - pattern: "bin/**"
    rationale: "Build outputs"
  - pattern: "obj/**"
    rationale: "Build outputs"
  - pattern: "**/BenchmarkDotNet.Artifacts/**"
    rationale: "Auto-generated benchmark artifacts"
  - pattern: "**/LICENSE.txt"
    rationale: "Legal documents, not project documentation"
  - pattern: ".git/**"
    rationale: "Git internals"
  - pattern: "**/*.csproj.lscache"
    rationale: "Lscache files"
  - pattern: "**/.gdignore"
    rationale: "Godot internal markers"

# Excluded by file extension (allow-list inverse)
included_extensions:
  - ".md"
  # All other extensions excluded by default
```

SCOPE_EXCLUSIONS.yaml is itself a Tier 1 LOCKED governance artifact (its own register entry).

### 2.4 Tool 2 — `query_register.ps1` (read-side query)

**Purpose**: agent and human query interface; returns structured results without manual YAML parsing.

**Invocation patterns**:

```powershell
# Query by document property
./tools/governance/query_register.ps1 --tier 1 --lifecycle LOCKED
# Returns: all Tier 1 LOCKED documents (id, path, version)

# Query by requirement
./tools/governance/query_register.ps1 --requirement REQ-K-L11
# Returns: requirement details + all documents/tests verifying it

# Query risks affecting document
./tools/governance/query_register.ps1 --risks-affecting DOC-A-001
# Returns: all active risks affecting KERNEL_ARCHITECTURE

# Query stale documents
./tools/governance/query_register.ps1 --stale
# Returns: documents with STALE flag OR next_review_due < today

# Query post-session targets (KEY for agent post-session protocol)
./tools/governance/query_register.ps1 --affected-by-milestone K9
# Returns: documents register tracks as «typically affected by this milestone type»
# Heuristic based on past similar milestones; agent compares against actual git-modified-files

# Query open CAPA
./tools/governance/query_register.ps1 --capa-open
# Returns: CAPA entries with closure_status OPEN

# Query lifecycle transitions needed
./tools/governance/query_register.ps1 --lifecycle-pending
# Returns: AUTHORED briefs with execution commits = should transition to EXECUTED

# Query by category
./tools/governance/query_register.ps1 --category J --tier 1
# Returns: foundational game mechanics

# Combinations
./tools/governance/query_register.ps1 --tier 1 --content-language ru
# Returns: Russian-only Tier 1 docs (translation candidates per Q-A45-X3)
```

**Output formats**:
- Default: human-readable table
- `--json`: structured JSON for agent consumption
- `--paths-only`: just file paths (for piping into `cat`, etc.)

**Performance**: in-memory load of REGISTER.yaml on each invocation. At ~200 entries this is <100ms. No database or caching needed; YAML is sufficient SoT at project scale. If grows beyond ~15000 lines in future years, schema amendment splits into multiple YAML files (Q-A45-X1 evolution mechanism).

### 2.5 Tool 3 — `render_register.ps1` (human-readable derivative)

**Purpose**: generate `docs/governance/REGISTER_RENDER.md` from REGISTER.yaml as human-browsable artifact. This is the **derivative human view**; YAML is primary.

**Invocation**:

```powershell
./tools/governance/render_register.ps1
# Writes docs/governance/REGISTER_RENDER.md
# Indexed by category, sorted by id within category
# Each entry: id, path, title, tier, lifecycle, last_modified, key cross-references
```

**Rendered output structure**:

```markdown
# DualFrontier Document Control Register — Rendered View

*Auto-generated from REGISTER.yaml. Do not edit; edit REGISTER.yaml instead.*

## Statistics
- Total documents: 195
- Tier 1: 22 | Tier 2: 12 | Tier 3: 55 | Tier 4: 76 | Tier 5: 3 (governance: 3)
- Open CAPA: 0 | Active risks: 7 | Stale documents: 0
- Last validation: 2026-05-12 (green)

## Table of contents

### Category A — Architecture (22 documents)
[Auto-generated TOC with anchor links to each entry]

[... categories B-J ...]

---

## Category A — Architecture (22 documents)

### DOC-A-001 — KERNEL_ARCHITECTURE.md
- Path: `docs/architecture/KERNEL_ARCHITECTURE.md`
- Tier: 1 | Lifecycle: LOCKED | Version: 1.5
- Owner: Crystalka | Last modified: 2026-05-10 (`2df5921`)
- Requirements authored: REQ-K-L1, REQ-K-L7, REQ-K-L11 (11 total — full list in YAML)
- Risks: RISK-001 (closed), RISK-005 (active)
- CAPA: CAPA-2026-05-09-K8.2-V2-REFRAMING (closed)
- Last review: 2026-05-10 (K-L3.1 closure) | Next review: 2027-05-10
- Content language: en

### DOC-A-002 — MOD_OS_ARCHITECTURE.md
[next entry]

[... all entries by category ...]

---

## Global collections

### Risks (14 entries)
[Brief table: id, title, status, type, impact]

### Requirements (13 entries)
[Brief table: id, title, verification_status, source_document]

### CAPA log (3 entries)
[Brief table: id, opened_date, closure_status, trigger summary]

### Audit trail (9 events)
[Chronological table: date, event, type, commits range]
```

REGISTER_RENDER.md is **Tier 2 Live** (its own register entry — auto-regenerated, so register tracks generation events, not edits).

### 2.6 Tooling tracking decision

Tooling scripts themselves are **not register entries** (they're .ps1 not .md). They're tracked via parent module README: `tools/governance/MODULE.md` (Tier 4 Category F) enumerates the three scripts as part of module documentation.

This avoids extending Category taxonomy to cover non-markdown artifacts. Code already tracked through git; module README provides governance-layer awareness.

---

## 3. Post-session update protocol (Q-A45-X5)

**The most load-bearing lock of A'.4.5**. Without enforcement, register degrades into stale artifact (this is RISK-010 explicitly tracked).

### 3.1 Protocol definition

Every agent execution session that:
- Closes a milestone, OR
- Makes architectural decisions, OR
- Modifies any document in register scope

...**must** execute the following protocol before commit closure:

```
1. Identify all documents modified during session (git status / git diff)
2. For each modified document:
   a. If document has register entry: update entry's last_modified, last_modified_commit, 
      lifecycle (if transitioning), governance_events (if applicable)
   b. If document is new (no register entry): create entry with required fields populated
   c. If document is renamed/moved: update entry's path; update all cross-references
   d. If document is deleted: transition entry to DEPRECATED with deprecation rationale,
      OR remove entry if document was Draft never finalized
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

### 3.2 Enforcement mechanism

**Strict gate (per A'.4.5 deliberation confirmation)**:

Manual invocation in closure protocol initially (pre-commit hook deferred to A'.4.5.bis):

```powershell
# At milestone closure, before final commit:
./tools/governance/sync_register.ps1 --validate

# Exit code 0 → proceed with commit
# Exit code 1 → halt; resolve validation errors; re-validate
# Exit code 2 → tooling failure; report to Crystalka
```

**Future upgrade (A'.4.5.bis or A'.9 analyzer integration)**:

```powershell
# .git/hooks/pre-commit (or CI equivalent)
$result = & ./tools/governance/sync_register.ps1 --validate
if ($LASTEXITCODE -ne 0) {
    Write-Error "Register validation failed. See VALIDATION_REPORT.md."
    Write-Error "Either fix register OR commit with --no-verify if explicitly waived by Crystalka."
    exit 1
}
```

### 3.3 Bypass mechanism

**Bypass via `git commit --no-verify`** available но **logged**:

Every bypass creates entry in `docs/governance/BYPASS_LOG.md` (Tier 2 Live document):

```markdown
# BYPASS_LOG — Register Validation Bypass Tracking

*Each entry: timestamp + commit hash + bypassing-author + rationale.*
*Validation includes check: if bypass_log has entries since last validation, flag for review.*

## 2026-XX-XX

### Bypass: <commit-hash>
- **Time**: 2026-XX-XXT14:32:00Z
- **Author**: <session_id> OR Crystalka
- **Rationale**: <why bypass was necessary>
- **Follow-up**: <how to resolve register state>
```

Validation includes review check: «if BYPASS_LOG.md has entries since last validation, flag for follow-up» — bypasses surface for cleanup but don't block subsequent work.

### 3.4 Strict vs advisory split

Per A'.4.5 deliberation Pass 3 confirmation:

**Strict (validation failure blocks commit)**:
- Document-changed-by-milestone enforcement: if files in git diff have no corresponding register update, fail
- Cross-reference integrity violations (dangling IDs, missing terminal-state references)
- Schema integrity violations (unknown enum values, missing required fields)
- Allowed-combinations matrix violations without special_case_rationale

**Advisory (warning only, doesn't block commit)**:
- STALE flag generation (next_review_due < today; doesn't block, surfaces in VALIDATION_REPORT.md)
- Tier 1 Russian-only document translation candidacy flag
- Tier 3 brief AUTHORED > 30 days warning
- Bypass log review pending

Rationale: agent must update register entries for changed documents (strict — prevents drift), but tooling shouldn't harass agent about future-dated reviews (advisory — informational).

### 3.5 Closure protocol integration

Existing closure protocol in `MIGRATION_PROGRESS.md`:

```markdown
Когда milestone закрывается:
1. Run final verification
2. Atomic commit
3. Update этот документ
4. Update brief
```

**Extends to** (METHODOLOGY §12.7 canonical post-A'.4.5):

```markdown
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
   - Bypass available via --no-verify but logged in BYPASS_LOG.md
```

MIGRATION_PROGRESS.md closure protocol section cross-references METHODOLOGY §12.7 as canonical:

```markdown
> **Note (post-A'.4.5)**: METHODOLOGY §12.7 is the canonical closure protocol.
> The version preserved below is retained for historical reference.

[Existing protocol preserved verbatim]
```

### 3.6 Agent self-check pattern

When agent executes a milestone, near the end of execution session:

```
Agent internally: «I have modified the following files this session: [list].
                  Let me query register for what I should have updated:»

$ ./tools/governance/query_register.ps1 --affected-by-milestone <current-milestone-id>

Returns: documents that register tracks as «typically affected by this milestone type»
         (heuristic based on past similar milestones)

Agent: «Comparing actual modifications to register expectations:
        - Match: [X documents touched as expected]
        - Surprise modifications: [Y documents not predicted — need register entry update]
        - Missing modifications: [Z documents predicted but not touched — flag for review]
       
        Updating REGISTER.yaml for all modified documents...»
```

The `--affected-by-milestone` query is **heuristic, not strict**. It surfaces what similar milestones touched (e.g., «K-series milestones typically touch KERNEL_ARCHITECTURE Part 2 + MIGRATION_PROGRESS K-section + brief Status field»). Agent uses it as **completeness check**, not as constraint.

### 3.7 Failure modes prevented

This protocol prevents:

**Failure 1: PHASE_A_PRIME_SEQUENCING staleness pattern**
- Pre-A'.4.5 observed: PHASE_A_PRIME_SEQUENCING.md A'.0.7 status «NEXT» despite A'.0.7 closure completed 2026-05-10
- Without register: no enforcement that closure milestone updates referencing documents
- With register: closure protocol step 5 requires update; validation gate enforces

**Failure 2: «I forgot to update the tracker»**
- Pre-A'.4.5 observed: MIGRATION_PROGRESS.md occasionally diverges from actual milestone state; Crystalka catches at next session
- Without register: silent drift accumulates between sessions
- With register: tooling validation surfaces drift on every commit

**Failure 3: Unknown unknowns**
- Pre-A'.4.5: agent doesn't know what governance bookkeeping was missed
- Without register: bookkeeping mental model lives in Crystalka's head, lost between sessions
- With register: `query_register --affected-by-milestone` surfaces expected updates

---

## 4. Authoring guidance

### 4.1 For FRAMEWORK.md §4 (Seven register sections) prose

This brief contains:
- **§1.2 content**: full schema for §1 Document Control (FRAMEWORK §4.1)
- **§1.3 content**: full schema for §3 Traceability (FRAMEWORK §4.3)
- **§1.4 content**: full schema for §4 Risk Register (FRAMEWORK §4.4)
- **§1.5 content**: full schema for §6 CAPA (FRAMEWORK §4.6)
- **§1.6 content**: full schema for §7 Audit Trail (FRAMEWORK §4.7)
- **§1.7 content**: full ID conventions (FRAMEWORK §5)
- **§1.8 content**: frontmatter mirror shape (FRAMEWORK §4.1 closing or §5 if separated)

Authoring session adds (per Pass 7 Track A outline):
- §4.1 Document Control: ISO 9001 7.5 + DO-178C SCI provenance prose (from Pass 1 Pass 1 §2.1.1 + §2.2.1)
- §4.2 Architecture Case: ISO 26262 Safety Case re-purposed provenance (from Pass 1 §2.3.1)
- §4.5 Internal Audit Schedule: ISO 9001 9.2 provenance + cadence table from Pass 2 §2.6
- §4.6 Joint provenance prose for DO-178C Problem Reports + ISO 9001 10.2
- §4.7 git log as authoritative + FDA 21 CFR Part 11 conceptual reference

### 4.2 For FRAMEWORK.md §6 (Post-session protocol) prose

This brief contains:
- **§3.1 content**: full protocol definition (FRAMEWORK §6.1)
- **§3.2 content**: enforcement mechanism + manual-initially + pre-commit-hook-deferred (FRAMEWORK §6.2)
- **§3.3 content**: bypass mechanism + BYPASS_LOG.md (FRAMEWORK §6.3)
- **§3.4 content**: strict vs advisory split rationale (FRAMEWORK §6.2 closing)
- **§3.5 content**: closure protocol integration + METHODOLOGY §12.7 cross-reference (FRAMEWORK §6.4)
- **§3.6 content**: agent self-check pattern (FRAMEWORK §6.5)
- **§3.7 content**: failure modes prevented (FRAMEWORK §6 closing — falsifiability anchor)

Authoring session adds:
- Cross-references to other FRAMEWORK sections (§3 classification — what's tracked, §4 sections — what's recorded, §7 schema versioning — how protocol evolves)
- METHODOLOGY §12.5 / §12.7 bidirectional cross-references

Total FRAMEWORK.md §4 prose target: ~150-200 lines.
Total FRAMEWORK.md §6 prose target: ~80-120 lines.

### 4.3 For REGISTER.yaml schema v1.0 authoring

This brief contains:
- **§1.1 content**: top-level structure (copy verbatim to schema header)
- **§1.2 content**: document entry shape template (use for all ~195 entries)
- **§1.3-1.6 content**: global collections entry shapes (templates for requirements/risks/CAPA/audit_trail)
- **§1.7 content**: ID conventions (apply consistently during enrollment)
- **§1.8 content**: frontmatter mirror shape (auto-generated by tooling first run)

Pre-authored entries from Pass 5 (DELIBERATION_CLOSURE.md §5-§8): 14 risks + 13 requirements + 3 CAPA + 9 audit_trail events ready to commit verbatim.

### 4.4 For tooling implementation (3 PowerShell scripts)

**sync_register.ps1**:
- §2.1 invocation modes (--validate, --sync, default)
- §2.2 validation rules (full enumeration — 30+ rules across 6 categories)
- §2.3 SCOPE_EXCLUSIONS.yaml integration
- Output format: stdout report + VALIDATION_REPORT.md write
- Exit codes 0/1/2

**query_register.ps1**:
- §2.4 invocation patterns (8+ query types enumerated)
- Output formats: table default, --json, --paths-only
- Performance budget: <100ms for ~200 entries

**render_register.ps1**:
- §2.5 rendered output structure (statistics + TOC + per-category sections + global collections)
- Single-file output: docs/governance/REGISTER_RENDER.md

Authoring session adds:
- Error handling patterns (file not found, YAML parse errors, etc.)
- PowerShell module dependency declaration (powershell-yaml or equivalent)
- Comment-based help for each script (Get-Help integration)

---

## 5. Execution-side checklist for Claude Code

When Claude Code execution session implements A'.4.5 tooling + schema:

### 5.1 Tooling implementation phase

- [ ] Create `tools/governance/` directory
- [ ] Author `sync_register.ps1` per §2.1-§2.3 specification
- [ ] Author `query_register.ps1` per §2.4 specification
- [ ] Author `render_register.ps1` per §2.5 specification
- [ ] Author `tools/governance/MODULE.md` (Tier 4 module-local README enumerating scripts)
- [ ] Author `tools/governance/SCOPE_EXCLUSIONS.yaml` per §2.3 content
- [ ] Test tooling on minimal REGISTER.yaml (5-10 sample entries) before full enrollment

### 5.2 Schema authoring phase

- [ ] Create `docs/governance/REGISTER.yaml` with top-level structure per §1.1
- [ ] Populate metadata header (schema_version, register_version, last_modified placeholders)
- [ ] Populate audit_calendar per §1.1
- [ ] Initialize 5 collections empty (documents, requirements, risks, capa_entries, audit_trail)
- [ ] Populate requirements collection with 13 entries from DELIBERATION_CLOSURE.md §6
- [ ] Populate risks collection with 14 entries from DELIBERATION_CLOSURE.md §5
- [ ] Populate capa_entries collection with 3 entries from DELIBERATION_CLOSURE.md §7
- [ ] Populate audit_trail collection with 9 events from DELIBERATION_CLOSURE.md §8

### 5.3 Validation phase (Phase 3 in execution brief)

- [ ] Run `sync_register.ps1 --validate` after schema populated
- [ ] Resolve validation errors (cross-reference integrity, missing required fields)
- [ ] Run `sync_register.ps1 --sync` first time → frontmatter mirrors written to ~195 documents
- [ ] Run `render_register.ps1` → REGISTER_RENDER.md generated
- [ ] Final validation pass green
- [ ] Generate `docs/governance/VALIDATION_REPORT.md`
- [ ] Create `docs/governance/BYPASS_LOG.md` (empty header)

### 5.4 Closure protocol integration phase

- [ ] Apply METHODOLOGY §12.7 amendment per Pass 6 (next brief)
- [ ] Cross-reference MIGRATION_PROGRESS.md closure protocol to METHODOLOGY §12.7
- [ ] Bootstrap: `git commit --amend` to fill REGISTER.yaml `last_modified_commit` field after initial commit (Q-A45-X2 bootstrap procedure)

### 5.5 Operational reminders

- PowerShell execution policy: may need `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned` for scripts to run
- YAML formatting: 2-space indentation strict; tooling fails on tabs
- Cross-platform note: PowerShell Core (pwsh) preferred over Windows PowerShell 5.1 for forward compatibility; current solo-Windows context accepts either
- File encoding: UTF-8 without BOM for .yaml and .md files; tooling normalizes on write

---

## 6. Brief authoring lineage

- **2026-05-12** — Pass 3 (Q9 + Q10 + Q-A45-X5) locked during A'.4.5 deliberation session (Claude Opus 4.7). Crystalka direction 2026-05-12: «Да рекомендации принимаются» (Pass 3 base + pre-commit hook deferred to A'.4.5.bis)
- **2026-05-12** — This execution-ready brief extracted from A'.4.5 deliberation closure §2.4 + §5/6/7/8 entry templates at Crystalka request («да пиши»)
- **(TBD)** — Consumed by next Opus session authoring FRAMEWORK.md §4 + §6 prose v1.0
- **(TBD)** — Consumed by Claude Code execution session implementing tooling + schema population + validation gate
- **(TBD)** — FRAMEWORK.md + REGISTER.yaml + 3 tooling scripts + SCOPE_EXCLUSIONS.yaml + VALIDATION_REPORT.md + BYPASS_LOG.md ship as A'.4.5 closure deliverables

---

**Brief end. Execution-ready content for schema authoring + tooling implementation + post-session protocol integration.**
