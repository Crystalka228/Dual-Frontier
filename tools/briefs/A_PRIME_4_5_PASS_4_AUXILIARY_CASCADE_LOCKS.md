---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_4_5_PASS_4
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_4_5_PASS_4
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_4_5_PASS_4
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_4_5_PASS_4
---
# A'.4.5 Pass 4 — Auxiliary Cascade Locks (Execution-Ready Brief)

**Status**: LOCKED 2026-05-12 (A'.4.5 deliberation Pass 4, Q-A45-X1 + Q-A45-X2 + Q-A45-X3)
**Type**: Execution-ready brief for authoring `docs/governance/FRAMEWORK.md` §7 (schema versioning) + §8 (governance recursion) + §9 (language scope) prose
**Authority subordinate to**: A'.4.5 deliberation closure document
**Consumes**: Pass 1-3 (defining what register tracks, how it's classified, how it's enforced); this Pass 4 closes recursive/edge-case ambiguity that если не залочены, surface as validation errors in execution
**Audience**: Opus session authoring FRAMEWORK prose; Claude Code execution session implementing schema versioning + meta-entry validation + bootstrap procedure

---

## Lock summary

Three auxiliary locks closing structural concerns that would otherwise surface as validation errors:

- **Q-A45-X1 — Schema versioning evolution**: semantic versioning at REGISTER.yaml top-level; amendments follow Tier 1 LOCKED protocol; tooling validates schema compatibility on every run
- **Q-A45-X2 — Governance-over-governance (recursion)**: meta-entries with `is_meta_entry: true` flag for REGISTER.yaml + FRAMEWORK.md + SYNTHESIS_RATIONALE.md + REGISTER_RENDER.md; special validation rules apply; bootstrap via `git commit --amend` at A'.4.5 closure
- **Q-A45-X3 — Language scope**: register entries English-mandatory (machine-readable invariant); `content_language` metadata field tracks document content language (enum: en / ru / mixed); advisory validation flags Tier 1 Russian-only documents для translation candidacy

These three locks are **structural infrastructure** for register longevity. Without them: schema evolution turns into ad-hoc breakage; register can't describe itself; language assumptions break agent queries.

---

## 1. Schema versioning evolution (Q-A45-X1)

### 1.1 Sources of schema-evolution pressure

Register schema v1.0 локается at A'.4.5 closure. Schema will evolve — anticipated 1-2 amendments в первый year; больше в будущем. Pre-emptive enumeration of evolution triggers ensures tooling and protocol handle them gracefully:

**Predictable amendment triggers**:

1. **New category needed** — Q-A45-X4 уже показал precedent: даже в deliberation сессии Crystalka добавил две категории (I Ideas Bank, J Game Mechanics). В будущем — новые категории под audio engine, networking, AI inference и т.д. (per D3 «native organicity» list of potential artifacts in MIGRATION_PROGRESS).

2. **New field on document entry** — например, в будущем добавится `localization_status` поле когда проект войдёт в i18n release prep, или `performance_budget` для performance-critical specs.

3. **New collection at top level** — например, `decisions: [...]` если D-log переезжает из MIGRATION_PROGRESS в register; или `lessons_learned: [...]` если K-Lessons section в METHODOLOGY миграется в structured collection.

4. **New lifecycle state** — например, `ARCHIVED` (terminal-not-deprecated, для historical docs that aren't superseded but no longer relevant); или `EMBARGOED` для documents pending review release.

5. **Validation rule changes** — relaxing/tightening allowed-combination matrix; adding new ID convention; changing required-fields list.

6. **New tier introduced** — Tier 5 Speculative was added at A'.4.5; future tiers may surface (e.g., Tier 6 «External references» for third-party docs governed-by-reference rather than maintained).

### 1.2 Versioning rules

**Semantic versioning at top of REGISTER.yaml**:

```yaml
schema_version: "1.0"
```

**Bump rules**:

- **MAJOR bump (1.0 → 2.0)**: breaking schema change requiring entry migration
  - Field renamed (e.g., `last_modified_commit` → `commit_hash`)
  - Field removed (e.g., dropping `first_authored` field)
  - ID convention changed in incompatible way
  - Required field added without default value
  - Collection renamed or restructured

- **MINOR bump (1.0 → 1.1)**: additive non-breaking change
  - New optional field on entry (e.g., adding `localization_status`)
  - New category (e.g., adding K Audio Engine)
  - New lifecycle state (e.g., adding ARCHIVED)
  - New collection at top level
  - New enum value in existing field (e.g., new `risk_type` value)
  - New verification_type value
  - New event_type value in audit_trail

- **PATCH bump (1.0 → 1.0.1)**: clarifications without schema change
  - Validation rule wording refinement
  - Documentation comment updates in FRAMEWORK
  - Bug fixes in tooling that don't change schema interpretation

### 1.3 Amendment milestone protocol

Schema amendment is **Tier 1 LOCKED amendment** following same protocol as KERNEL_ARCHITECTURE / MOD_OS_ARCHITECTURE amendments (precedent: K-L3.1, A'.0.7):

1. **Deliberation milestone** (analog A'.0.7 / K-L3.1 / A'.4.5):
   - Authoring brief explaining trigger + proposed change
   - Q-lock pass for design decisions
   - Synthesis Track A (FRAMEWORK update) + Track B (REGISTER.yaml schema update)

2. **Amendment plan authoring** (analog A_PRIME_X_Y_AMENDMENT_PLAN.md):
   - Per-entry migration logic
   - Tooling compatibility analysis
   - Validation rule changes enumerated
   - Cross-reference impact assessment

3. **Execution milestone** — migration logic for existing entries:
   - **For MINOR**: re-validate all entries with new schema; populate new optional fields where applicable (defaults); old entries remain valid
   - **For MAJOR**: explicit migration step per entry; old field values transformed or archived; tooling runs migration script before validation

4. **CAPA entry** opened for the amendment, recording:
   - Trigger that drove schema evolution
   - Rationale for chosen approach (vs alternatives considered)
   - Effectiveness verification (post-amendment all entries validate clean)

5. **Audit trail event** appended:
   - EVT-{date}-SCHEMA-V{old}-TO-V{new}-AMENDMENT
   - Documents affected: meta-entries + any entries requiring field updates
   - Governance impact: schema evolution narrative

### 1.4 Schema version history in FRAMEWORK.md

FRAMEWORK.md §7 contains schema version history table tracking all bumps:

```markdown
## §7.4 Schema version history

| Version | Date | Closure milestone | Change |
|---|---|---|---|
| 1.0 | 2026-05-XX | A'.4.5 closure | Initial schema lock |
| 1.1 | TBD | TBD | Example: added `localization_status` field to document entries |
| 2.0 | TBD | TBD | Example: restructured `audit_trail` into `events` + `transitions` separate collections |
```

This table is **append-only**; old versions never removed. Tooling validates table presence + monotonic version progression.

### 1.5 Tooling behavior on schema mismatch

`sync_register.ps1 --validate` reads `schema_version` field at REGISTER.yaml top. Compares against tooling's expected schema version.

**Mismatch handling table**:

| YAML schema | Tooling expects | Behavior |
|---|---|---|
| 1.0.0 | 1.0.1 | Warn «tooling slightly newer; forward-compatible»; proceed |
| 1.0.1 | 1.0.0 | Warn «tooling slightly outdated; consider updating»; proceed |
| 1.1 | 1.0 | **Error**: «schema v1.1 requires tooling v1.1+; update tooling»; halt |
| 1.0 | 1.1 | Warn «tooling forward-compatible to v1.0»; proceed |
| 2.0 | 1.X | **Error**: «MAJOR version mismatch; migration required»; halt with pointer to amendment plan |
| 1.X | 2.0 | **Error**: «register pre-migration; run migration script»; halt |

**Rationale for asymmetric handling**:
- Tooling forward-compatible to MINOR-older schemas (gracefully degrades)
- Tooling halts on MINOR-newer schemas (cannot validate fields it doesn't know)
- Both halt on MAJOR mismatch (data integrity at stake)

### 1.6 Special considerations for evolution mechanism

**Backward compatibility commitment**: tooling at version v1.X validates schemas v1.0 through v1.X (forward-compatibility within MAJOR). Tooling at v2.0 may drop v1.X compatibility if migration tooling provides bridge.

**Migration tooling pattern** (for MAJOR bumps):
- `tools/governance/migrate_schema.ps1 --from 1.X --to 2.0`
- Reads REGISTER.yaml v1.X format; writes REGISTER.yaml v2.0 format
- Outputs migration report to MIGRATION_REPORT_v1X_TO_v2.md
- Validates post-migration against v2.0 schema

**Schema-version vs register-version distinction**:
- `schema_version`: structure of YAML (changes only via amendment milestones)
- `register_version`: content of YAML (bumps on every closure write; minor for content changes, major if schema_version bumps)

Example: schema_version stays "1.0" for many closures while register_version bumps 1.0 → 1.1 → 1.2 → ... Then schema_version 1.0 → 1.1 amendment lands; register_version 1.X → 2.0 reflecting structural shift.

---

## 2. Governance-over-governance recursion (Q-A45-X2)

### 2.1 The recursion problem

`REGISTER.yaml` describes documents. `REGISTER.yaml` is a document. Therefore `REGISTER.yaml` describes itself.

This is **necessary, not optional**. Without self-entry:
- Agent querying «what Tier 2 Live documents exist?» misses REGISTER.yaml itself
- Audit trail for register modifications не tracked в register
- Schema version history не register-tracked
- Same problem applies to FRAMEWORK.md, SYNTHESIS_RATIONALE.md, REGISTER_RENDER.md — they're governance documents describing what register is and how to use it

But the recursion has to handle special cases the standard validation rules don't anticipate by default.

### 2.2 Meta-entries in register

Four documents have meta-status at A'.4.5:

```yaml
documents:
  - id: DOC-G-REGISTER
    path: docs/governance/REGISTER.yaml
    title: "Document Control Register"
    category: G                          # Project meta
    tier: 2                              # Live (mutable on every milestone)
    lifecycle: Live
    
    # Meta-entry flag
    is_meta_entry: true
    meta_role: register_source_of_truth
    
    owner: Crystalka
    version: "1.0"                       # follows register_version
    last_modified: "2026-05-XX"
    last_modified_commit: "<hash>"       # bootstrap pattern handles initial value
    
    # Special review cadence
    review_cadence: "on-every-milestone-closure"
    
    # Self-reference: register tracks its own audit trail
    audit_trail_query: "git log --follow docs/governance/REGISTER.yaml"
    governance_events: []                # appended to as register evolves
    
    content_language: en
  
  - id: DOC-A-FRAMEWORK
    path: docs/governance/FRAMEWORK.md
    title: "Document Control Register — Governance Framework"
    category: A                          # Architecture (governance framework is architecture-class)
    tier: 1
    lifecycle: LOCKED
    is_meta_entry: true
    meta_role: register_specification
    
    # Standard Tier 1 fields
    owner: Crystalka
    version: "1.0"
    first_authored: "2026-05-XX"
    last_modified: "2026-05-XX"
    last_modified_commit: "<hash>"
    
    review_cadence: "on-change+annual"
    last_review_date: "2026-05-XX"
    last_review_event: "A'.4.5 closure"
    next_review_due: "2027-05-XX"
    reviewer: Crystalka
    
    content_language: en
  
  - id: DOC-A-SYNTHESIS_RATIONALE
    path: docs/governance/SYNTHESIS_RATIONALE.md
    title: "Document Control Register — Synthesis Rationale"
    category: A
    tier: 1
    lifecycle: LOCKED
    is_meta_entry: true
    meta_role: register_provenance
    
    # Standard Tier 1 fields...
    content_language: en
  
  - id: DOC-G-REGISTER_RENDER
    path: docs/governance/REGISTER_RENDER.md
    title: "Document Control Register — Rendered View"
    category: G                          # Project meta (human-readable derivative)
    tier: 2
    lifecycle: Live
    is_meta_entry: true
    meta_role: register_rendered_derivative
    
    # Auto-regenerated; tracks last-generation event
    owner: Crystalka                     # nominal; actual writer is tooling
    version: null                        # version-less, regenerated on demand
    last_modified: "2026-05-XX"
    last_modified_commit: "<hash>"
    
    review_cadence: "on-register-change"
    
    content_language: en
```

**The `is_meta_entry: true` flag signals tooling**: this entry describes part of the register itself. Validation handles meta-entries specially.

### 2.3 Meta-role enum values

`meta_role` field has enumerated values describing each meta-entry's function:

- `register_source_of_truth` — the YAML SoT itself (REGISTER.yaml)
- `register_specification` — governance framework declaring how register works (FRAMEWORK.md)
- `register_provenance` — synthesis rationale documenting source-standard borrowings (SYNTHESIS_RATIONALE.md)
- `register_rendered_derivative` — human-readable view auto-generated from SoT (REGISTER_RENDER.md)

Future meta-roles если register expands:
- `register_validation_report` — if VALIDATION_REPORT.md flagged as meta (currently Tier 2 Live, regular entry)
- `register_bypass_log` — if BYPASS_LOG.md flagged as meta
- `register_scope_exclusions` — for SCOPE_EXCLUSIONS.yaml

Decision: at A'.4.5 closure, **only 4 meta-entries** flagged (REGISTER + FRAMEWORK + SYNTHESIS_RATIONALE + REGISTER_RENDER). VALIDATION_REPORT and BYPASS_LOG are operational artifacts, not register specification — they're regular Tier 2 entries.

### 2.4 Special validation rules for meta-entries

`sync_register.ps1 --validate` applies extra rules to entries with `is_meta_entry: true`:

**Rule 1 — Path constraint**:
- meta_entry.path must exist in `docs/governance/` directory
- Validation fails if meta_entry path outside governance directory (prevents accidental meta-promotion of regular docs)

**Rule 2 — Self-tracking exemption**:
- meta_entry's `last_modified_commit` does NOT require external verification (register tracks itself; chicken-and-egg при initial creation)
- For non-meta-entries, validation checks `last_modified_commit` resolves в git log; meta-entries exempt

**Rule 3 — DEPRECATED transition constraint**:
- meta_entry cannot transition to DEPRECATED without successor register declared
- «You can't deprecate the register without proposing what replaces it»
- Specifically: REGISTER.yaml DEPRECATED requires `deprecated_by` pointing к new register URL/path
- This prevents accidental governance collapse

**Rule 4 — Schema version coupling**:
- meta_entry's reference to schema_version must match REGISTER.yaml's top-level schema_version
- FRAMEWORK.md and SYNTHESIS_RATIONALE.md for schema v1.0 must reflect v1.0 design
- Schema bumps trigger meta-entry version updates as well (FRAMEWORK and SYNTHESIS_RATIONALE bump versions when schema bumps)

**Rule 5 — Cadence constraint**:
- DOC-G-REGISTER has special review_cadence «on-every-milestone-closure»
- STALE flag fires if any milestone closes без updating REGISTER.yaml
- This is the **load-bearing enforcement** for post-session protocol (Q-A45-X5)

### 2.5 Bootstrap problem at A'.4.5 closure

At A'.4.5 closure, REGISTER.yaml is first created. The very first commit creating REGISTER.yaml cannot have its hash в `last_modified_commit` (chicken-and-egg: hash exists only after commit is created).

**Resolution: two-option pattern, recommend amend**:

**Option A — `git commit --amend` (recommended; cleaner history)**:

```powershell
# Step 1: Author REGISTER.yaml with last_modified_commit placeholder
last_modified_commit: "PENDING-INITIAL"

# Step 2: Stage all A'.4.5 closure files
git add docs/governance/* tools/governance/* docs/methodology/METHODOLOGY.md docs/MIGRATION_PROGRESS.md ...

# Step 3: Commit (commit N created)
git commit -m "feat(governance): A'.4.5 closure — Document Control Register operational"

# Step 4: Capture commit hash
$commitHash = git rev-parse HEAD

# Step 5: Update REGISTER.yaml with actual hash
# (sed or PowerShell -replace)
(Get-Content REGISTER.yaml) -replace "PENDING-INITIAL", $commitHash | Set-Content REGISTER.yaml

# Step 6: Amend commit (single commit in history)
git add docs/governance/REGISTER.yaml
git commit --amend --no-edit
```

**Result**: single closure commit with REGISTER.yaml containing its own commit hash. Clean history; no «backfill» commit.

**Option B — Separate backfill commit (acceptable; less clean)**:

```powershell
# Step 1: Commit closure as commit N
git commit -m "feat(governance): A'.4.5 closure..."

# Step 2: Capture hash
$commitHash = git rev-parse HEAD

# Step 3: Update + commit as separate commit N+1
(Get-Content REGISTER.yaml) -replace "PENDING-INITIAL", $commitHash | Set-Content REGISTER.yaml
git add REGISTER.yaml
git commit -m "chore(governance): backfill REGISTER initial commit hash"
```

**Result**: two commits; «chore: backfill» commit is auditable but pollutes history.

**Lock**: Option A (amend) recommended. Both tolerated by tooling — meta-entry exemption (Rule 2 above) means tooling doesn't fail if `last_modified_commit` references commit not yet existing during pre-validation.

### 2.6 Future meta-entry additions

If new governance documents added (e.g., FRAMEWORK_DETAILS.md split-out from FRAMEWORK.md if size grows beyond comfortable), they flag as meta:

```yaml
- id: DOC-A-FRAMEWORK_DETAILS
  path: docs/governance/FRAMEWORK_DETAILS.md
  is_meta_entry: true
  meta_role: register_specification_appendix
  # ... standard fields
```

Meta-role enum extends via schema MINOR bump.

---

## 3. Language scope (Q-A45-X3)

### 3.1 The state of language in the project

Per A'.0.7 Q-A07-2 lock (audience contract «agent-primary reader, abstract framing English»):

- **METHODOLOGY substantive prose**: English with selective Russian for direct quotes from Crystalka or v1.x era artifacts
- **Briefs**: English for new ones; some legacy Russian content preserved verbatim
- **Code**: English (per CODING_STANDARDS)
- **Closure reports / amendment plans**: mostly English; some Russian quotes preserved
- **Architectural specs (KERNEL, MOD_OS, RUNTIME)**: English
- **Live trackers (MIGRATION_PROGRESS)**: mixed — Russian narrative sections + English commit messages
- **Module-local READMEs**: variable, some Russian (legacy), some English (newer)
- **Learning artifacts (`docs/learning/PHASE_1.md`)**: Russian (preserved per i18n campaign)

Register schema is English. Register entries are English. But document content can be Russian, mixed, or English. Need explicit lock + tracking.

### 3.2 Lock decision — register entries English-mandatory

**Rule**: register entries (schema fields, enum values, free-text fields like title/notes/rationale) are **English-language**.

**Rationale**:
- Schema as machine-readable contract — English convention matches code, tooling, OSS readability
- Agent-primary use case — agent operates fluently в English по умолчанию
- Mixed-language registers would complicate query patterns (case sensitivity, transliteration, search-by-substring)
- Doesn't affect document content — Russian content documents remain Russian; only their register metadata is English

**Exception**: `title` field MAY include original-language title in parentheses if the document's actual title is Russian:

```yaml
- id: DOC-E-LEARNING-PHASE-1
  path: docs/learning/PHASE_1.md
  title: "Self-teaching ritual artifact — Phase 1 (Самообучение фаза 1)"
  # English primary + Russian original in parens
  content_language: ru
  
  # Free-text fields about Russian content can include Russian phrases for clarity
  notes: "Russian-language artifact preserved verbatim per i18n campaign rules."
```

Limit: Russian in parens only for `title`. Other free-text fields (rationale, notes, governance_impact) should be authored in English even when describing Russian-content documents, except when quoting Crystalka directives verbatim (precedent: METHODOLOGY footnote quotes Crystalka Russian phrases inline).

### 3.3 Lock decision — content_language metadata field

Register tracks `content_language` per document entry:

```yaml
- id: DOC-A-001
  content_language: en               # ISO 639-1 code subset
  
- id: DOC-E-LEARNING-PHASE-1
  content_language: ru               # Russian content
  
- id: DOC-C-MIGRATION-PROGRESS
  content_language: mixed            # mixed Russian/English
```

**Enum values**: `en`, `ru`, `mixed`. Three-value enum chosen because:
- `en` — clear English-only document
- `ru` — clear Russian-only document (legacy or i18n campaign artifact)
- `mixed` — substantive content в both languages (most Live trackers fall here; brief that quotes Crystalka extensively)

Future expansion if other languages join (none anticipated short-term): schema MINOR bump adds new enum values (e.g., `de`, `fr` if community contributors author docs). Forward-compatible — existing entries with `en`/`ru`/`mixed` remain valid.

### 3.4 Use cases for content_language tracking

**Use case 1 — Agent query: «what Russian-language documents exist?»**:

```powershell
./tools/governance/query_register.ps1 --content-language ru
# Returns: legacy artifacts + i18n-campaign-pending docs
```

Useful for: audit pass; translation campaign planning; understanding language distribution в corpus.

**Use case 2 — Translation status tracking без separate registry**:

TRANSLATION_PLAN.md is currently a Tier 2 Live tracker (Category H). Future possibility: subsume into register via:
- Each Russian/mixed document's register entry has `translation_status` field (if translation campaign active)
- Query: «what documents await translation?» = `content_language: ru OR mixed` + `translation_status: pending`
- TRANSLATION_PLAN.md migrates to «index of translation campaign» role

(This is post-A'.4.5 future evolution; not part of A'.4.5 scope. Current A'.4.5 scope: just `content_language` tracking; campaign planning remains in TRANSLATION_PLAN.md.)

**Use case 3 — Agent navigation hint**:

When agent queries register для documents related to a task, `content_language` informs reading strategy:
- `en` → standard reading
- `ru` → may require translation pass or careful interpretation
- `mixed` → expect language switches; track which sections in which language

### 3.5 Advisory validation rule — Tier 1 Russian-only flag

`sync_register.ps1 --validate` includes **advisory warning** (not error) для Tier 1 Russian-only documents:

```
Warning: DOC-X-NNN at <path> is Tier 1 LOCKED but content_language=ru.
Tier 1 documents are architectural authority surfaces; English content
preferred for agent-primary readership. Consider translation candidacy.
```

**Rationale**:
- Tier 1 documents are most-read by agents during Phase 0 brief reads
- Russian-only Tier 1 forces translation cost on every agent session
- Translation candidacy means: «this document is candidate for translation campaign»; не enforced as error because translation has cost; flagged for awareness

**Tier 1 + content_language=mixed**: not flagged. Mixed Tier 1 docs are common (METHODOLOGY has Russian footnotes quoting Crystalka; this is intentional, not a deficiency).

**Tier 2-5 + content_language=ru**: not flagged. These tiers tolerate Russian content (e.g., `docs/learning/PHASE_1.md` is Russian historical artifact; deliberate preservation per i18n campaign).

### 3.6 Future evolution — multilingual support

Current locks cover en/ru/mixed adequate for project state. If project expands к multilingual contributors:

**Schema evolution path** (Q-A45-X1 MINOR bump):
- Add enum values (e.g., `de`, `fr`, `zh`) to `content_language`
- Optionally add `translations` array field: `translations: [{lang: en, path: docs/translated/X-en.md, status: complete}]` для tracking parallel-translated documents
- Tooling adds query patterns: `--content-language fr`, `--has-translation en`

Forward-compatible: existing entries with en/ru/mixed remain valid; new entries can use new language codes.

---

## 4. Cross-cutting integration

### 4.1 Q-A45-X1 + Q-A45-X2 interaction

Schema versioning intersects with governance recursion: when schema v1.0 → v1.1 lands, meta-entries (REGISTER + FRAMEWORK + SYNTHESIS_RATIONALE + REGISTER_RENDER) must update their version coupling.

**Procedure**:
1. Schema amendment milestone (Tier 1 LOCKED amendment per Q-A45-X1 §1.3)
2. FRAMEWORK.md bumps version to match (e.g., v1.0 → v1.1)
3. SYNTHESIS_RATIONALE.md bumps version to match (e.g., v1.0 → v1.1; if synthesis changes; otherwise v1.0 preserved with «schema vX.Y compatible» annotation)
4. REGISTER.yaml schema_version field updated; register_version may bump major
5. REGISTER_RENDER.md regenerated с new schema
6. Audit trail event EVT-{date}-SCHEMA-VX-TO-VY-AMENDMENT appended
7. Meta-entry validation rules (Q-A45-X2 §2.4) verify version coupling holds

### 4.2 Q-A45-X1 + Q-A45-X3 interaction

If multilingual support added (schema MINOR bump per Q-A45-X3 §3.6), it's a schema evolution requiring Q-A45-X1 amendment milestone protocol.

**Bootstrap consideration**: existing entries with `en` / `ru` / `mixed` remain valid post-amendment (forward-compatible). New entries can use new language codes.

### 4.3 Q-A45-X2 + Q-A45-X3 interaction

Meta-entries (FRAMEWORK + SYNTHESIS_RATIONALE) are English-content по умолчанию (consistent with register-entries-English-mandatory rule).

REGISTER.yaml itself: YAML keys English; free-text fields English; document `content_language` enum values English.

REGISTER_RENDER.md (auto-generated): English (derivative of English register data).

This means: all 4 meta-entries have `content_language: en`. Advisory validation does NOT flag them (Tier 1 Russian-only flag inapplicable; они English).

---

## 5. Authoring guidance for FRAMEWORK.md prose

### 5.1 For FRAMEWORK.md §7 (Schema versioning) prose

This brief contains:
- **§1.1 content**: full enumeration of evolution triggers (FRAMEWORK §7.1)
- **§1.2 content**: semantic versioning rules table (FRAMEWORK §7.1)
- **§1.3 content**: amendment milestone protocol step-by-step (FRAMEWORK §7.2)
- **§1.4 content**: schema version history table template (FRAMEWORK §7.4)
- **§1.5 content**: tooling compatibility behavior table (FRAMEWORK §7.3)
- **§1.6 content**: migration tooling pattern + schema-version vs register-version distinction

Authoring session adds:
- Cross-references to Q-A45-X2 (meta-entries also version-coupled) and Q-A45-X1's role within broader register evolution
- Concrete example walkthrough of hypothetical v1.0 → v1.1 amendment (e.g., adding `localization_status` field)

Target length: ~60-80 lines (per Pass 7 Track A outline §7).

### 5.2 For FRAMEWORK.md §8 (Governance recursion) prose

This brief contains:
- **§2.1 content**: recursion problem statement (FRAMEWORK §8.1 opening)
- **§2.2 content**: 4 meta-entries with full schema examples (FRAMEWORK §8.1)
- **§2.3 content**: meta_role enum values (FRAMEWORK §8.1)
- **§2.4 content**: special validation rules for meta-entries (FRAMEWORK §8.2)
- **§2.5 content**: bootstrap problem + two-option solution + recommendation (FRAMEWORK §8.3)
- **§2.6 content**: future meta-entry additions (FRAMEWORK §8 closing)

Authoring session adds:
- Cross-references to Q-A45-X1 (schema versioning affects meta-entry version coupling)
- Bootstrap pattern as «how A'.4.5 closure works in practice» concrete example

Target length: ~40-60 lines (per Pass 7 Track A outline §8).

### 5.3 For FRAMEWORK.md §9 (Language scope) prose

This brief contains:
- **§3.1 content**: state of language in project (FRAMEWORK §9 opening prose)
- **§3.2 content**: register entries English-mandatory rule (FRAMEWORK §9.1)
- **§3.3 content**: content_language metadata field + enum values (FRAMEWORK §9.2)
- **§3.4 content**: use cases (FRAMEWORK §9.2)
- **§3.5 content**: advisory validation rule + Tier 1 Russian-only flag (FRAMEWORK §9.3)
- **§3.6 content**: future multilingual support evolution path (FRAMEWORK §9 closing)

Authoring session adds:
- Cross-references to A'.0.7 Q-A07-2 lock (audience contract inheritance)
- Cross-reference to Q-A45-X1 (multilingual expansion follows schema amendment protocol)

Target length: ~40-60 lines (per Pass 7 Track A outline §9).

---

## 6. Execution-side checklist for Claude Code

When Claude Code execution session implements A'.4.5 auxiliary cascade locks:

### 6.1 Schema versioning implementation

- [ ] REGISTER.yaml top-level `schema_version: "1.0"` field present
- [ ] REGISTER.yaml top-level `register_version: "1.0"` field present
- [ ] FRAMEWORK.md §7.4 schema version history table populated with v1.0 row
- [ ] sync_register.ps1 validates schema_version field present + parseable
- [ ] sync_register.ps1 implements compatibility check per §1.5 table (warn/error per mismatch type)
- [ ] sync_register.ps1 hardcoded expected schema version matches REGISTER.yaml at A'.4.5 closure (both "1.0")

### 6.2 Meta-entry validation implementation

- [ ] 4 meta-entries created in REGISTER.yaml documents collection:
  - DOC-G-REGISTER (path: docs/governance/REGISTER.yaml)
  - DOC-A-FRAMEWORK (path: docs/governance/FRAMEWORK.md)
  - DOC-A-SYNTHESIS_RATIONALE (path: docs/governance/SYNTHESIS_RATIONALE.md)
  - DOC-G-REGISTER_RENDER (path: docs/governance/REGISTER_RENDER.md)
- [ ] Each meta-entry has `is_meta_entry: true` flag set
- [ ] Each meta-entry has `meta_role` populated with appropriate enum value
- [ ] sync_register.ps1 implements 5 special validation rules (§2.4) for meta-entries
- [ ] DOC-G-REGISTER has special `review_cadence: "on-every-milestone-closure"`

### 6.3 Bootstrap procedure (A'.4.5 closure final commit)

- [ ] All A'.4.5 deliverables staged for commit
- [ ] REGISTER.yaml authored with `last_modified_commit: "PENDING-INITIAL"` placeholder
- [ ] Closure commit created (capture hash)
- [ ] REGISTER.yaml updated to replace `PENDING-INITIAL` with actual commit hash
- [ ] `git commit --amend --no-edit` to produce single clean commit
- [ ] Post-amend: `git log -1` shows REGISTER.yaml with correct self-referenced hash

### 6.4 Language scope implementation

- [ ] Every document entry in REGISTER.yaml has `content_language` field populated (en/ru/mixed)
- [ ] Russian content detection: agent reads first ~200 chars of document during enrollment; if Cyrillic-dominant → ru; if substantive both languages → mixed; else en
- [ ] sync_register.ps1 implements advisory warning для Tier 1 + content_language=ru
- [ ] VALIDATION_REPORT.md surfaces translation-candidate documents (advisory section, not errors section)

### 6.5 Cross-cutting consistency checks

- [ ] FRAMEWORK.md §7.4 history table version matches REGISTER.yaml schema_version
- [ ] SYNTHESIS_RATIONALE.md schema reference matches REGISTER.yaml schema_version
- [ ] All 4 meta-entries reference same schema_version (1.0 at A'.4.5)
- [ ] BYPASS_LOG.md created (empty, header only); NOT flagged as meta-entry (regular Tier 2)
- [ ] VALIDATION_REPORT.md generated post-validation; NOT flagged as meta-entry (regular Tier 2)

---

## 7. Brief authoring lineage

- **2026-05-12** — Pass 4 (Q-A45-X1 + Q-A45-X2 + Q-A45-X3) locked during A'.4.5 deliberation session (Claude Opus 4.7). Crystalka direction 2026-05-12: «Да всё хорошо» (Pass 4 confirmation)
- **2026-05-12** — This execution-ready brief extracted from A'.4.5 deliberation closure §2.5 + Pass 7 outline §7-§9 at Crystalka request («теперь Pass 4»)
- **(TBD)** — Consumed by next Opus session authoring FRAMEWORK.md §7 + §8 + §9 prose v1.0
- **(TBD)** — Consumed by Claude Code execution session implementing schema versioning checks + meta-entry validation + bootstrap procedure + content_language tracking
- **(TBD)** — FRAMEWORK.md §7-§9 ship as part of A'.4.5 closure deliverable (Tier 1 LOCKED v1.0)

---

**Brief end. Execution-ready content for schema versioning + governance recursion handling + language scope.**
