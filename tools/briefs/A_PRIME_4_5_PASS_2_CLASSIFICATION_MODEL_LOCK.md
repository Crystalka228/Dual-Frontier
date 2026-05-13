# A'.4.5 Pass 2 — Classification Model Lock (Execution-Ready Brief)

**Status**: LOCKED 2026-05-12 (A'.4.5 deliberation Pass 2 + Q-A45-X4 extension)
**Type**: Execution-ready brief for authoring `docs/governance/FRAMEWORK.md` §3 (classification model) prose + new folder structure scaffolding
**Authority subordinate to**: A'.4.5 deliberation closure document
**Consumes**: this lock + Pass 7 Track A outline (FRAMEWORK §3 structural skeleton) — together produce FRAMEWORK §3 prose v1.0
**Audience**: Opus session authoring FRAMEWORK prose; Claude Code execution session creating folders + enrolling documents

---

## Lock summary

**Three-axis classification model**: Category × Tier × Lifecycle, with explicit allowed-combinations matrix preventing illegal states.

- **10 categories** (A-J) — content type discriminator; 2 new categories introduced at A'.4.5 (I Ideas Bank, J Game Mechanics)
- **5 tiers** (1-5) — governance regime discriminator; new Tier 5 «Speculative» introduced для Ideas Bank
- **8 lifecycle states** — control state discriminator with explicit transition matrix + mandatory cross-references on terminal states
- **2 new folders** at A'.4.5 closure: `docs/ideas/` + `docs/mechanics/`
- **IDEAS_RESERVOIR.md migration**: stays at root as **index** (option (a) per Crystalka lock); individual ideas live as separate files in `docs/ideas/`
- **Borderline cases** (COMBO_RESOLUTION / COMPOSITE_REQUESTS / RESOURCE_MODELS architecture-vs-mechanics call): defer to Claude Code execution session — agent classifies inline reading content + records rationale in `special_case_rationale` field

---

## 1. Category taxonomy — 10 categories (A-J)

Each document classified by content type. Category determines: which template, voice, audience, organizing folder.

### 1.1 Full taxonomy

| Code | Name | Location | Examples |
|---|---|---|---|
| **A** | Architecture spec | `docs/architecture/` | KERNEL_ARCHITECTURE, MOD_OS_ARCHITECTURE, RUNTIME_ARCHITECTURE, ARCHITECTURE, ECS, EVENT_BUS, CONTRACTS, ISOLATION, THREADING, MODDING |
| **B** | Methodology | `docs/methodology/` | METHODOLOGY, CODING_STANDARDS, DEVELOPMENT_HYGIENE, MAXIMUM_ENGINEERING_REFACTOR, PIPELINE_METRICS, TESTING_STRATEGY |
| **C** | Live tracker | `docs/` root | MIGRATION_PROGRESS, ROADMAP, IDEAS_RESERVOIR (post-A'.4.5 as index) |
| **D** | Brief | `tools/briefs/` | K0..K9 briefs, G0..G9 briefs, K_L3_1 briefs, A'.0.5/A'.0.7/A'.4.5 briefs, K9_BRIEF_REFRESH_PATCH |
| **E** | Discovery / closure / audit | `docs/audit/`, `docs/reports/`, `docs/prompts/`, `docs/benchmarks/`, `docs/learning/` | AUDIT_PASS_*, PERFORMANCE_REPORT_K7, NORMALIZATION_REPORT, NATIVE_CORE_EXPERIMENT, M*_CLOSURE_REVIEW |
| **F** | Module-local | `src/**`, `tests/**`, `mods/**`, `native/**`, `assets/**`, `tools/briefs/MODULE.md`, `tools/governance/MODULE.md` | per-folder README.md / MODULE.md files |
| **G** | Project meta | repo root, `docs/` root | README.md (root), docs/README.md, index documents |
| **H** | i18n | `docs/` root | TRANSLATION_GLOSSARY, TRANSLATION_PLAN |
| **I** | **Ideas Bank** | **`docs/ideas/`** | **Brainstorming, what-ifs, shelved alternatives, future feature speculation** (NEW at A'.4.5) |
| **J** | **Game Mechanics** | **`docs/mechanics/`** | **Gameplay design at design level — race biology, magic schools, combat resolution rules, faction relations, crafting trees, balance numbers** (NEW at A'.4.5) |

### 1.2 Category distinction rules (anti-overlap)

When document candidate could fit multiple categories, use these discrimination rules:

**A vs J (architecture vs game mechanics)**:
- Category A describes **how the engine supports** mechanics (data structures, system interactions, performance properties)
- Category J describes **what mechanics are** (game design intent, player-facing rules, balance considerations)
- Test: would another game engine implementing same mechanic produce different J document but similar A document? If yes → split clean
- Borderline (handled by execution agent): COMBO_RESOLUTION, COMPOSITE_REQUESTS, RESOURCE_MODELS — currently in `docs/architecture/`; agent reads content + decides classification

**A vs F (architecture spec vs module-local README)**:
- Category A = cross-module architectural authority
- Category F = per-directory technical notes for that directory only
- Test: does the document govern other modules? If yes → A; if scope-limited to its directory → F

**C vs G (live tracker vs project meta)**:
- Category C = tracks ongoing project state with mutation cadence
- Category G = static project orientation (README files, indices)
- Test: does the document update on every milestone closure? If yes → C; if updates rare/structural-only → G

**D vs E (brief vs closure/audit)**:
- Category D = milestone instrument (prospective: «here's what to do»)
- Category E = closure / audit artifact (retrospective: «here's what was done / what we found»)
- Test: temporal stance — brief authored before execution, closure report authored after

**I vs C (ideas bank vs live tracker)**:
- Category I = speculative, not yet authoritative; ideas don't track ongoing state
- Category C = ongoing operational state with cadence
- IDEAS_RESERVOIR.md special case: was C historically; post-A'.4.5 remains C as **index** to Category I folder. Individual idea files in `docs/ideas/` are Category I.

**I vs A (ideas vs architecture)**:
- Category I = «what if?» — proposals not yet decided
- Category A = authoritative; LOCKED specs binding implementation
- Promotion path: Category I idea matures → architectural deliberation milestone → if accepted → Category A spec drafted; original Category I entry transitions to SUPERSEDED with cross-reference

**J Tier 1 vs J Tier 2 (foundational mechanics vs tunable mechanics)**:
- Tier 1 J = mechanics that constrain architecture (race biology determining tech branches, combat resolution model determining ECS Combat domain, mana lease model in code)
- Tier 2 J = mechanics that don't constrain architecture (HP/damage numbers, recipe trees, faction relation matrices, balance values)
- Test: would changing this require architectural deliberation milestone? If yes → Tier 1; if playtesting iteration sufficient → Tier 2

### 1.3 Special-case overrides (special_case_rationale field)

Documents whose Category-default Tier doesn't fit:

| Document | Category | Default Tier | Override Tier | Rationale (populates `special_case_rationale`) |
|---|---|---|---|---|
| MAXIMUM_ENGINEERING_REFACTOR.md | B | (Tier 1 by default for B) | Tier 1 | confirmed; «refactor doctrine» is LOCKED Tier 1 like other methodology |
| IDEAS_RESERVOIR.md (post-A'.4.5) | C (still) OR G (as index) | 2 | 2 | reclassified as Tier 2 index; «no quarterly cadence — index document»; individual ideas in `docs/ideas/` are Category I Tier 5 |
| PHASE_A_PRIME_SEQUENCING.md | A | 1 (by default for A) | 2 | «mutable per phase closure, not LOCKED architecture; subordinate to MIGRATION_PLAN»; will be amended at A'.4.5 closure to fix stale A'.0.7 status |
| K_L3_1_AMENDMENT_PLAN.md, A_PRIME_0_7_AMENDMENT_PLAN.md | A by content | (Tier 1 by default for A) | Tier 3 | «amendment plans behave like briefs — EXECUTED post-landing, not LOCKED»; lifecycle follows D-category pattern |

Special-case overrides validated by tooling: any combination outside default matrix requires `special_case_rationale` non-null.

### 1.4 Borderline classification deferral (Q-A45-X4 lock detail)

Three documents currently in `docs/architecture/` are borderline between Category A and Category J. Classification deferred to Claude Code execution session per Pass 2 (b) lock:

1. **COMBO_RESOLUTION.md** — describes how combat combos resolve. Mostly architectural (engine resolution mechanism), but some design intent for combo design philosophy
2. **COMPOSITE_REQUESTS.md** — similar position; engine-level composite request resolution
3. **RESOURCE_MODELS.md** — describes resource modeling for game systems; design intent (resource semantics) vs architecture (data structure approach)

Execution agent reads each document content and classifies:
- If document is **primarily architectural pattern description** → keep Category A in `docs/architecture/`
- If document is **primarily mechanic design intent** → migrate to Category J in `docs/mechanics/` with appropriate Tier 1 (foundational) or Tier 2 (tunable)
- Either way, `special_case_rationale` populated with classification rationale (e.g., «60% architectural content + 40% mechanic intent; classified A but cross-referenced from `docs/mechanics/` index»)

---

## 2. Tier model — 5 tiers

Each tier defines governance regime: change authority, review cadence, approval gate, agent navigation role.

### 2.1 Tier 1 — Architectural authority

**Documents** (~22):
- Architecture: KERNEL_ARCHITECTURE, MOD_OS_ARCHITECTURE, RUNTIME_ARCHITECTURE, MIGRATION_PLAN_KERNEL_TO_VANILLA, GPU_COMPUTE, FIELDS, CONTRACTS, ISOLATION, THREADING, ECS, EVENT_BUS, ARCHITECTURE, ARCHITECTURE_TYPE_SYSTEM, MODDING, MOD_PIPELINE
- Methodology: METHODOLOGY, MAXIMUM_ENGINEERING_REFACTOR, CODING_STANDARDS, DEVELOPMENT_HYGIENE, TESTING_STRATEGY, PIPELINE_METRICS
- Governance (NEW): FRAMEWORK, SYNTHESIS_RATIONALE
- Foundational game mechanics (potential): race biology, combat resolution model, magic foundational rules (migration into Category J Tier 1 if applicable)

**Change authority**: Crystalka via deliberation milestone + amendment plan + landing milestone (analog A'.0.7 / K-L3.1 / A'.4.5 pattern)

**Review cadence**: on every change + annual full review (calendar Q1)

**Approval gate**: explicit Crystalka lock in deliberation brief

**Agent navigation role**: **canonical reference surface** — when agent needs «what's the architectural authority for X?», answer is in Tier 1. Agent reads Tier 1 documents during Phase 0 brief reads + during architectural deliberation.

### 2.2 Tier 2 — Operational live (state of the project)

**Documents** (~12):
- C-category trackers: MIGRATION_PROGRESS, ROADMAP, IDEAS_RESERVOIR (as index post-A'.4.5)
- A-category special case: PHASE_A_PRIME_SEQUENCING
- Governance meta: REGISTER (the YAML itself), REGISTER_RENDER, VALIDATION_REPORT, BYPASS_LOG
- Live closure reports (mutable while series open): PERFORMANCE_REPORT_K7, NORMALIZATION_REPORT, CPP_KERNEL_BRANCH_REPORT, NATIVE_CORE_EXPERIMENT
- G-category indices: README.md root, docs/README.md, docs/ideas/README.md (NEW), docs/mechanics/README.md (NEW)
- J-category tunable mechanics (when added)

**Change authority**: executor session per milestone closure protocol (auto-update); Crystalka for strategic adjustments

**Review cadence**: on every closure write + quarterly drift review (calendar Q1/Q2/Q3/Q4)

**Approval gate**: part of closure verification (already exists in MIGRATION_PROGRESS closure protocol; post-A'.4.5 register validation added per Q-A45-X5)

**Agent navigation role**: **«what's happening now?»** — entry point for understanding current state, in-flight milestones, recent decisions. Agent reads Tier 2 during Phase 0 milestone orientation.

### 2.3 Tier 3 — Milestone instruments

**Documents** (~55):
- All `tools/briefs/*.md` (currently 44; growing)
- Closure reports / audit artifacts (E-category)
- Amendment plans (A_PRIME_0_7_AMENDMENT_PLAN, K_L3_1_AMENDMENT_PLAN, A_PRIME_4_5_AMENDMENT_PLAN when authored)
- Verification logs (K6_VERIFICATION_LOG)

**Change authority**: depends on Lifecycle state:
- AUTHORED → revisable via patch brief (precedent K9_BRIEF_REFRESH_PATCH)
- EXECUTED → immutable contract section; only «Lessons learned» appendable
- DEPRECATED → supersession-marked; cross-reference mandatory
- SUPERSEDED → replaced by newer version; cross-reference mandatory

**Review cadence**: on Lifecycle Status transition only (no calendar cadence — briefs are ephemeral by design)

**Approval gate**: Status field updates + amendment-plan-shaped revisions for active briefs

**Agent navigation role**: **historical record + contract for in-flight milestones** — agent reads when executing a specific milestone or auditing past one. Tier 3 is the most active tier (every milestone execution adds 1+ entries; every closure transitions AUTHORED → EXECUTED).

### 2.4 Tier 4 — Module-local

**Documents** (~76):
- src/ READMEs: ~56 files (one per subdirectory in src/)
- tests/ READMEs: 5 files
- mods/ READMEs: 8 files
- native/ MODULE.mds: 5 files
- assets/scenes/README.md: 1 file
- tools/briefs/MODULE.md: 1 file
- tools/governance/MODULE.md: 1 file (NEW at A'.4.5)

**Change authority**: updated as part of any commit touching the module's source

**Review cadence**: quarterly Phase-led sweep (precedent: A'.0.5 Phase 6 module-local refresh)

**Approval gate**: grouped with source commit (no separate gate)

**Agent navigation role**: **just-in-time context** — agent reads when working in that specific module, not for broad orientation. Tier 4 is largest but lowest per-entry cognitive cost — most fields default-populated mechanically.

**Tier 4 sub-division punt**: брифа original Q18 proposed sub-divide Tier 4 into 4a/4b/4c (mods/ READMEs vs src/ READMEs vs tests/ READMEs as separate tier-types). **Locked: NOT sub-divided initially**. If quarterly sweep economics differ between sub-types in first 6 months of register use, sub-division surfaces as A'.4.5.X micro-milestone. YAGNI applied to taxonomy expansion.

### 2.5 Tier 5 — Speculative (NEW at A'.4.5)

**Why new tier needed**: Ideas Bank documents cannot fit existing tiers without semantic distortion:
- Tier 1 «Architectural authority»: ideas aren't authority
- Tier 2 «Operational live state»: ideas aren't state
- Tier 3 «Milestone instruments»: ideas aren't instruments
- Tier 4 «Module-local»: ideas aren't local to any module

Tier 5 = **explicit «not authoritative» tier**. Critical for agent navigation: agent querying «what does the project say about X?» must **differentiate** authoritative (Tier 1) vs aspirational/speculative (Tier 5).

**Documents** (initial: ~1-3 at A'.4.5 closure; growing post-closure as ideas accumulate):
- `docs/ideas/*.md` (Category I individual idea files)
- Note: `docs/ideas/README.md` is **Category G Tier 2** (index document), not Tier 5

**Change authority**: anyone (Crystalka primary; agent can author ideas with `proposed_by: <session_id>` field as part of post-session protocol per Q-A45-X5)

**Review cadence**: **no scheduled cadence**; periodic harvest passes («which ideas matured for architectural deliberation pickup?»)

**Approval gate**:
- For entry into bank: none required
- For promotion out of bank: architectural deliberation milestone required (Idea → A'.X.Y deliberation → if accepted → Category A spec drafted; original idea transitions to SUPERSEDED with cross-reference to resulting A document)

**Agent navigation role**: **«what hasn't been decided yet?»** — agent reads when scoping future work, brainstorming, or checking whether architectural proposal already considered-and-rejected (e.g., agent proposing «what about X?» can query Tier 5 to find «X was considered as `idea-x.md`, rejected because Y»)

**Lifecycle restriction**: Tier 5 documents allowed lifecycle: **Draft, Live, DEPRECATED, SUPERSEDED**. **STALE state does not apply to Tier 5** — ideas don't go stale; they're either alive, picked up, or deprecated-with-rationale. Critical: removing STALE from Tier 5 prevents tooling from harassing agent about review cadence on speculation. Brainstorming should be friction-free.

### 2.6 Tier distribution summary

| Tier | Count | % of total ~195 |
|---|---|---|
| Tier 1 | ~22 | ~11% |
| Tier 2 | ~12 | ~6% |
| Tier 3 | ~55 | ~28% |
| Tier 4 | ~76 | ~39% |
| Tier 5 | ~3 initial (growing) | ~1-2% (initially) |
| Total | ~168 + governance new (FRAMEWORK + SYNTHESIS_RATIONALE + REGISTER + REGISTER_RENDER + BYPASS_LOG + 2 folder indices + tools/governance/MODULE.md = ~7) = ~175-195 | 100% |

(Range reflects uncertainty in actual count; Claude Code execution Phase 0 produces definitive count.)

---

## 3. Lifecycle model — 8 states + transition matrix

Each document has current lifecycle state; transitions are protocol-driven and tooling-validated.

### 3.1 State enumeration

| State | Description | Mutability |
|---|---|---|
| **Draft** | Authored but not finalized; may be revised freely | Full mutation |
| **Live** | Actively maintained; mutable on every relevant milestone | Full mutation per closure |
| **LOCKED** | Change authority via formal amendment milestone only | Restricted mutation |
| **EXECUTED** | Brief that has been run; historical immutable | Only «Lessons learned» appendable |
| **AUTHORED** | Brief authored, awaits execution; may be revised via patch brief | Patch-brief mutation pattern |
| **DEPRECATED** | Superseded by successor; retained for historical context | Read-only with cross-reference |
| **SUPERSEDED** | Replaced by newer version of same logical document | Read-only with cross-reference |
| **STALE** | Known out-of-date; awaits update or formal archive | Surfaced by audit; not steady state |

### 3.2 Transition matrix

Allowed transitions (any not listed = forbidden; tooling validation blocks):

```
Draft → Live           (Tier 2 documents finalized for active use)
Draft → LOCKED         (Tier 1 documents reach first stable version)
Draft → AUTHORED       (Tier 3 brief authoring complete)
Draft → EXECUTED       (Tier 3 closure report authored)

Live → STALE           (audit flags review overdue)
Live → DEPRECATED      (live tracker retired; example: PHASE_A_PRIME_SEQUENCING when MIGRATION_PLAN v1.1 absorbs it)
Live → SUPERSEDED      (rare; live tracker replaced by new structure)

LOCKED → SUPERSEDED    (amendment milestone produces new version; old version SUPERSEDED)
LOCKED → DEPRECATED    (architectural retirement; rare — example: K8.2_v1_DEPRECATED brief precedent for spec-level retirement)

AUTHORED → EXECUTED    (execution session closes milestone)
AUTHORED → DEPRECATED  (brief abandoned without execution)
AUTHORED → SUPERSEDED  (brief replaced by amendment; example: K8.2_v1 → K8.2_v2)

EXECUTED → DEPRECATED  (historical brief no longer reflects authoritative state)
EXECUTED → (no further transitions; immutable contract)

STALE → Live           (review completed, document brought up to date)
STALE → DEPRECATED     (review determines document obsolete)
STALE → SUPERSEDED     (review produces replacement)

DEPRECATED → (terminal; no transitions out)
SUPERSEDED → (terminal; no transitions out)
```

### 3.3 Transition triggers

| Transition | Trigger | Mandatory action |
|---|---|---|
| Draft → LOCKED | Initial deliberation closure | CAPA-style entry in §6: «Document locked at vX.Y, commit hash» |
| LOCKED → SUPERSEDED | Amendment milestone lands | New version entry created; old entry `superseded_by: <new_id>` filled; CAPA entry recording reason |
| Live → STALE | `next_review_due < today` AND no review event recorded since | Tooling auto-flags during validation; surfaces in `VALIDATION_REPORT.md` |
| STALE → Live | Review pass completed | `last_review_date` updated; STALE flag cleared |
| AUTHORED → EXECUTED | Milestone closure protocol step «mark brief as EXECUTED» | Execution commit hash recorded; «Lessons learned» section opened |
| AUTHORED → SUPERSEDED | New brief version replaces old | Bidirectional supersession links; old marked SUPERSEDED |
| Any → DEPRECATED | Explicit decision (architectural retirement) | `deprecated_by: <successor_id>` mandatory; cross-reference enforced |

### 3.4 Mandatory cross-references on terminal states

- **DEPRECATED** entries must have `deprecated_by: <successor_id>` populated (else validation fails)
- **SUPERSEDED** entries must have `superseded_by: <successor_id>` populated
- New documents creating supersession/deprecation must update **both ends**: their `supersedes: [...]` field AND the old document's `superseded_by: ...` field
- Tooling enforces bidirectional integrity: `sync_register.ps1 --validate` rejects if supersession references one-sided

### 3.5 Most active lifecycle: Tier 3 brief cycle

Briefs cycle through AUTHORED → (optional patch) → EXECUTED quickly. Each cycle: agent must update register entry (this is post-session update protocol per Q-A45-X5).

Pre-condition violation example: agent executes K9 milestone, doesn't update K9 brief's Lifecycle from AUTHORED to EXECUTED. Validation catches because:
1. Git log shows K9 commits on `feat/k9-field-storage` merged to main
2. Register entry for K9 brief still says `lifecycle: AUTHORED`
3. Tooling validation rule: «Tier 3 brief with last_modified_commit on main branch since `last_lifecycle_transition` requires explicit Lifecycle state update»

---

## 4. Allowed-combinations matrix (Category × Tier × Lifecycle)

Three-axis classification is **not** fully orthogonal — strong correlations exist between Category-default Tier and allowed Lifecycle states. Matrix encodes legal combinations; tooling validates.

### 4.1 Default matrix

| Category | Default Tier | Allowed Lifecycle states | Notes |
|---|---|---|---|
| A | 1 | LOCKED, SUPERSEDED, Draft (pre-lock only) | Tier 1 dominant; LOCKED on completion; Draft only during initial authoring |
| B | 1 | LOCKED, SUPERSEDED, Draft | Same as A |
| C | 2 | Live, STALE | Live default; STALE flagged by audit |
| D | 3 | AUTHORED, EXECUTED, DEPRECATED, SUPERSEDED | Brief-specific lifecycle |
| E | 3 | EXECUTED, DEPRECATED | Historical immutable on creation |
| F | 4 | Live, STALE | Updated with source commits |
| G | 2 | Live | README and index files |
| H | 2 (or 3) | Live, EXECUTED | TRANSLATION_PLAN.md EXECUTED post-campaign; GLOSSARY.md Live |
| **I** | **5** | **Draft, Live, DEPRECATED, SUPERSEDED** | **No STALE — ideas don't stale** |
| **J** | **1 OR 2** | **LOCKED + SUPERSEDED + Draft (if Tier 1); Live + STALE (if Tier 2)** | **Per-document tier in `tier` field** |

### 4.2 Forbidden combinations (tooling rejects without special_case_rationale)

Examples of validation-rejected combinations:
- **Tier 1 + Lifecycle AUTHORED**: «LOCKED-кандидат limbo» not permitted; either Draft (pre-lock) or LOCKED (post-deliberation)
- **Tier 3 + Lifecycle LOCKED**: briefs aren't LOCKED specs; if a brief becomes authoritative, it's promoted to Category A and Tier 1
- **Tier 5 + Lifecycle STALE**: ideas don't go stale; STALE state forbidden on Tier 5
- **Tier 4 + Lifecycle LOCKED**: module-local docs are mutable per source commit; LOCKED ceremony not applicable
- **Category D + Tier 1**: briefs are Tier 3; if document is Tier 1 authority, it's Category A or B
- **Category E + Tier 1**: closure reports are Tier 3; historical retrospective doesn't become Tier 1 authority
- **Category F + Tier 1/2/3**: module-local stays Tier 4

### 4.3 Special-case override mechanism

Documents whose Category × Tier × Lifecycle combination is outside default matrix require **`special_case_rationale`** field populated with explanation. Validation passes if rationale present; rejects if field null on non-default combination.

Examples requiring rationale (enumerated in §1.3 above):
- MAXIMUM_ENGINEERING_REFACTOR.md: Category B + Tier 1 + Lifecycle LOCKED (default for B is Tier 1, so no override needed actually — this is in-matrix)
- IDEAS_RESERVOIR.md post-A'.4.5: Category C + Tier 2 + Lifecycle Live (default for C — in matrix) BUT with `notes: "Functions as index for Category I folder docs/ideas/; individual ideas are Category I Tier 5"` — informational, not override
- PHASE_A_PRIME_SEQUENCING.md: Category A + Tier 2 + Lifecycle Live (Category A default is Tier 1; override Tier 2; requires rationale «mutable per phase closure, subordinate to MIGRATION_PLAN — not LOCKED architecture»)
- K_L3_1_AMENDMENT_PLAN.md: Category A + Tier 3 + Lifecycle EXECUTED (Category A default Tier 1; override Tier 3; rationale «amendment plans behave like briefs»)

### 4.4 Per-document `tier` field for Category J

Category J unique: per-document Tier assignment because game mechanics split into foundational (Tier 1) vs tunable (Tier 2).

Schema field `tier` directly populated with 1 or 2 for J documents:
- Tier 1 J: mechanics constraining architecture; LOCKED-like change protocol; review on every change + annual full review
- Tier 2 J: tunable mechanics; Live with playtesting iteration; no formal amendment milestone required for balance changes

Cross-reference to architectural documents in J Tier 1 entries: `constrains: [DOC-A-XXX]` field links foundational mechanic to architecture it constrains.

---

## 5. Folder structure decisions (post-A'.4.5)

### 5.1 New folders created at A'.4.5 execution

```
docs/
├── architecture/          (Category A, existing)
├── methodology/           (Category B, existing)
├── audit/                 (Category E, existing)
├── reports/               (Category E, existing)
├── prompts/               (Category E, existing)
├── benchmarks/            (Category E artifacts, existing)
├── learning/              (Category E, existing)
├── governance/            (NEW — FRAMEWORK + SYNTHESIS_RATIONALE + REGISTER.yaml + REGISTER_RENDER + BYPASS_LOG + VALIDATION_REPORT)
├── ideas/                 (NEW — Category I individual idea files; Tier 5)
├── mechanics/             (NEW — Category J game mechanic docs; Tier 1 or Tier 2 per-doc)
├── MIGRATION_PROGRESS.md  (Category C, existing)
├── ROADMAP.md             (Category C, existing)
├── IDEAS_RESERVOIR.md     (Category C, existing — now functions as index for docs/ideas/)
├── FEEDBACK_LOOPS.md      (Category B — see classification note)
├── TRANSLATION_GLOSSARY.md, TRANSLATION_PLAN.md  (Category H, existing)
└── README.md              (Category G, existing)

tools/
├── briefs/                (Category D + MODULE.md as F, existing)
└── governance/            (NEW — sync_register.ps1 + query_register.ps1 + render_register.ps1 + MODULE.md + SCOPE_EXCLUSIONS.yaml)
```

### 5.2 IDEAS_RESERVOIR.md migration (lock decision (a))

**Lock**: IDEAS_RESERVOIR.md stays at `docs/` root as **index** of Category I bank; individual idea files live in `docs/ideas/`.

**Rationale per Crystalka direction 2026-05-12 confirmation**:
- Preserves discoverability — «where do I find ideas? look at IDEAS_RESERVOIR.md» works for both pre-A'.4.5 readers and post-A'.4.5 navigation
- Index document is small; can be either manually curated or auto-generated from register query `--category I --tier 5 --lifecycle Live`
- Existing IDEAS_RESERVOIR.md content preserved verbatim at A'.4.5 closure; can be reorganized post-closure as individual idea files migrate (incremental, no big-bang content migration required)

**Post-A'.4.5 IDEAS_RESERVOIR.md structure**:
```markdown
# IDEAS_RESERVOIR — Index to Ideas Bank

*This document is the index for `docs/ideas/` Ideas Bank.*
*Individual ideas live as separate .md files in the folder.*
*Existing content below preserved for backward compatibility; new ideas should be authored as separate files in `docs/ideas/<idea-slug>.md`.*

## Active ideas (Category I, Tier 5, Lifecycle Live)
- [idea-x.md](./ideas/idea-x.md) — short summary

## Deprecated / rejected ideas
- [idea-y.md](./ideas/idea-y.md) — DEPRECATED 2026-XX-XX; rationale: ...

## Legacy content (pre-A'.4.5)
[Existing content preserved verbatim]
```

### 5.3 `docs/mechanics/` folder seeding

**Initial state at A'.4.5 closure**:
- `docs/mechanics/README.md` (Category G, Tier 2 Live) — index document explaining purpose and growth pattern

**Potential migration candidates** (Claude Code execution session decides per content reading):
- COMBO_RESOLUTION.md (currently `docs/architecture/`)
- COMPOSITE_REQUESTS.md (currently `docs/architecture/`)
- RESOURCE_MODELS.md (currently `docs/architecture/`)

Decision deferred to execution session per Pass 2 (b) lock. Each gets explicit `special_case_rationale` populated.

**Growth pattern post-A'.4.5**: as project develops game mechanic design content, individual mechanic files authored in `docs/mechanics/<mechanic-slug>.md`. Examples expected: race-biology.md, magic-schools.md, combat-resolution.md, faction-relations.md, crafting-trees.md, balance-numbers.md.

### 5.4 `docs/governance/` folder population (full A'.4.5 deliverable)

At A'.4.5 closure, folder contains:
- `FRAMEWORK.md` — Tier 1 LOCKED v1.0 (governance principles)
- `SYNTHESIS_RATIONALE.md` — Tier 1 LOCKED v1.0 (provenance from 5 standards)
- `REGISTER.yaml` — Tier 2 LIVE (operational SoT; schema is LOCKED v1.0)
- `REGISTER_RENDER.md` — Tier 2 LIVE (auto-generated human-readable view)
- `BYPASS_LOG.md` — Tier 2 LIVE (post-session protocol bypass log; empty at closure)

Plus during A'.4.5 execution but lives at `tools/governance/`:
- `sync_register.ps1`, `query_register.ps1`, `render_register.ps1` — PowerShell tooling
- `MODULE.md` — Tier 4 module-local for governance tooling directory
- `SCOPE_EXCLUSIONS.yaml` — Tier 1 LOCKED (its own register entry as meta-config)
- `VALIDATION_REPORT.md` — Tier 2 LIVE (regenerated per validation run)

---

## 6. Authoring guidance for FRAMEWORK.md §3 prose

This brief contains:
- **§1 content**: full text for FRAMEWORK.md §3.1 «Ten categories» (table + distinction rules + special-case overrides + borderline deferral)
- **§2 content**: full text for FRAMEWORK.md §3.2 «Five tiers» (per-tier definitions with agent navigation role)
- **§3 content**: full text for FRAMEWORK.md §3.3 «Eight lifecycle states» (state table + transition matrix + triggers + cross-reference rules)
- **§4 content**: full text for FRAMEWORK.md §3.4 «Allowed-combinations matrix» (default matrix + forbidden combinations + special-case override mechanism + per-document tier for J)
- **§5 content**: full text for FRAMEWORK.md §3.5 «Folder structure» (post-A'.4.5 directory tree + migration decisions + seeding)

Authoring session adds:
- Cross-references to other FRAMEWORK.md sections (§1 What is the register, §4 Seven sections, §5 ID conventions, §6 Post-session protocol)
- Examples per category drawn from actual project documents
- Sub-section numbering consistent with FRAMEWORK.md outline (per Pass 7 Track A)

Total FRAMEWORK.md §3 prose target: ~150-200 lines (per Pass 7 outline §3 estimate; sub-sections 3.1-3.5).

---

## 7. Execution-side checklist for Claude Code

When Claude Code execution session runs A'.4.5 enrollment:

### 7.1 Folder creation step (Phase 1)
- [ ] Create `docs/governance/` directory
- [ ] Create `docs/ideas/` directory with index README (Category G Tier 2 Live; «This folder contains Category I idea documents. See IDEAS_RESERVOIR.md for index.»)
- [ ] Create `docs/mechanics/` directory with index README (Category G Tier 2 Live; «This folder contains Category J game mechanic documents. Tier 1 foundational + Tier 2 tunable.»)
- [ ] Create `tools/governance/` directory

### 7.2 Borderline document classification (Phase 2 inline)
- [ ] Read COMBO_RESOLUTION.md content; classify A or J; if J, decide Tier 1 or 2; populate `special_case_rationale` with classification rationale
- [ ] Read COMPOSITE_REQUESTS.md content; same protocol
- [ ] Read RESOURCE_MODELS.md content; same protocol
- [ ] If any reclassified to J, move file to `docs/mechanics/` and update cross-references in callers (likely KERNEL_ARCHITECTURE references)

### 7.3 IDEAS_RESERVOIR.md update (Phase 2)
- [ ] Add header section explaining post-A'.4.5 role as index
- [ ] Preserve existing content under «Legacy content» section
- [ ] Initial individual idea files NOT migrated at A'.4.5 closure — defer to incremental authoring post-closure

### 7.4 Per-document classification (Phase 2 main loop)
- [ ] Each enrolled document gets Category × Tier × Lifecycle assignment
- [ ] Validation: combination must be in default matrix OR `special_case_rationale` populated
- [ ] Sub-section rules from §1.2 anti-overlap rules applied for ambiguous documents

### 7.5 Validation gate (Phase 3)
- [ ] Run `sync_register.ps1 --validate`
- [ ] All combinations validate (in matrix or with rationale)
- [ ] Transition matrix not violated (no Tier 1 + AUTHORED, no Tier 5 + STALE, etc.)
- [ ] All terminal-state documents have mandatory cross-references populated

---

## 8. Brief authoring lineage

- **2026-05-12** — Pass 2 + Q-A45-X4 extension locked during A'.4.5 deliberation session (Claude Opus 4.7). Crystalka direction 2026-05-12: «Принимается» (Pass 2 base), «Принимается» (Q-A45-X4 Ideas Bank + Game Mechanics extension)
- **2026-05-12** — This execution-ready brief extracted from A'.4.5 deliberation closure §2.3 + §2.5 (Q-A45-X4) + §3.1 (FRAMEWORK §3 outline) at Crystalka request
- **(TBD)** — Consumed by next Opus session authoring `docs/governance/FRAMEWORK.md` §3 prose v1.0
- **(TBD)** — Consumed by Claude Code execution session for folder creation + document classification + IDEAS_RESERVOIR migration + borderline document classification
- **(TBD)** — FRAMEWORK.md ships as Tier 1 LOCKED at A'.4.5 execution closure

---

**Brief end. Execution-ready content for FRAMEWORK.md §3 authoring + folder structure scaffolding + document classification protocols.**
