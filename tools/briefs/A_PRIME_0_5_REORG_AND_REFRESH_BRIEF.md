---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_0_5_REORG_REFRESH
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_0_5_REORG_REFRESH
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_0_5_REORG_REFRESH
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_0_5_REORG_REFRESH
---
# A'.0.5 — Documentation reorganization + cross-reference refresh + module-local refresh + cleanup campaign

**Brief type**: Operational milestone brief (mechanical reorganization + selective semantic refresh)
**Authored**: 2026-05-10 (Opus 4.7, post-K-L3.1 closure 45d831c)
**Phase A' position**: A'.0.5 — between A'.0 (K-L3.1, DONE) and A'.0.7 (methodology rewrite, NEXT)
**Target session**: single Claude Desktop session, full MCP tool access (Filesystem MCP, bash, git)
**Estimated session length**: 2–4 hours auto-mode, ~25–40 atomic commits
**Status**: EXECUTED — closure 2026-05-10, commits `27523ac`..`28a9c8d` on main; MIGRATION_PROGRESS A'.0.5 entry filled at `28a9c8d`; final brief-EXECUTED commit lands per Phase 9 commit 3 of 3
**Prerequisite**: A'.0 K-L3.1 closure (commit `45d831c` on main)
**Blocks**: A'.0.7 methodology rewrite, A'.1 amendment brief execution, A'.3 push to origin, A'.4 K9 execution

---

## §1 Why this milestone exists

### §1.1 Operational state at brief authoring

After K-L3.1 closure (commit `45d831c`, 2026-05-10), the repository is in this state:

- **`docs/architecture/`** is a new directory (created during K-L3.1 closure, currently contains only `PHASE_A_PRIME_SEQUENCING.md`)
- **All other `.md` files** remain at their pre-A'.0 locations: `docs/*.md` flat-level, `tools/briefs/*.md`, per-module folder docs (`src/DualFrontier.*/README.md`, etc.)
- **Crystalka direction 2026-05-10** (declared during this cleanup planning session): «всё что связано с архитектурой в доках я засунул в папку архитектура» — establishes new organizational principle that **architecture documentation lives at `docs/architecture/`**, but the migration of existing artifacts to that structure has **not yet been executed**
- **Crystalka direction 2026-05-10** (later in the same cleanup planning session): «привести локальные документы в в папках модулей в порядок, а то некоторые изрядно устарели особенно те что про ядро» — establishes scope for module-local doc refresh (especially kernel-area)
- **Crystalka direction 2026-05-10** (mid-session pipeline declaration): «всё делается через десктопное приложение Claude» + «удалить использование в документах упоминание о локальной модели gemma» — establishes pipeline restructure to 2-agent (Crystalka + Claude Desktop) with terminology scrub for stale Gemma/Cline/4-agent references

### §1.2 Problem statement

Three categories of operational debt accumulated:

1. **Organizational drift**: architecture documents scattered across `docs/`, `tools/briefs/`, possibly other locations. Crystalka's organizational principle (architecture docs in `docs/architecture/`) requires migration of existing artifacts.

2. **Cross-reference staleness**: documents reference each other by path. When files move, references stale. When K-L3.1 closure created `docs/architecture/`, no existing document was updated to reference the new directory; future amendments will multiply this.

3. **Module-local doc staleness**: per-module docs (`src/DualFrontier.<Module>/README.md` and similar) accumulated drift through K0-K8.2v2 evolution. Kernel-area docs especially suspect (per Crystalka direction). Some claim component shapes that no longer exist (post-K8.2v2 conversions/deletions); some reference systems retired (ShieldSystem, SocialSystem, BiomeSystem); some reference Path α / class component patterns now obsolete.

4. **Pipeline-restructure terminology debt**: Gemma, Cline, "4-agent", "local executor", LM Studio references throughout `METHODOLOGY.md` and possibly other docs. Pipeline now 2-agent unified Claude Desktop. Terminology mechanically incorrect.

5. **Stub-reference debt** (carried over from cleanup session pre-K-L3.1): 5 READMEs (`Components/README.md` top-level, `Components/Combat/README.md`, `Components/World/README.md`, `Components/Magic/README.md`, `Components/Pawn/README.md`) reference deleted-stub components from K8.2 v2 closure (Ammo, Shield, Weapon, School, Social, Biome). Was originally A'.2 separate phase, **now folded into A'.0.5**.

### §1.3 Why one brief, not multiple

Under unified Claude Desktop pipeline (1M context), **single-session execution is reliable** for the entire scope. Earlier framing assumed quantized executor capability bounds; that framing is obsolete.

Splitting concerns 1-5 into separate milestones would:
- Multiply closure overhead (each milestone has Phase 0 pre-flight, Phase N closure, MIGRATION_PROGRESS update)
- Multiply cross-reference refresh work (every reorganization requires its own cross-ref pass)
- Create inter-milestone ordering questions (cross-refs assume reorganization done, refresh assumes cross-refs done, etc.)

Single brief with phased structure preserves discipline through **stop conditions**, not through milestone splits. Each phase is atomic in its commit shape; the milestone is atomic at closure boundary.

### §1.4 What this milestone does NOT do

To prevent scope creep into adjacent milestones:

- **No K-L3.1 amendment work**: amendments to KERNEL_ARCHITECTURE.md, MOD_OS_ARCHITECTURE.md, MIGRATION_PLAN_KERNEL_TO_VANILLA.md, MIGRATION_PROGRESS.md (4 LOCKED docs) for K-L3.1 lock propagation are A'.1 scope. A'.0.5 only **moves** these files (per organizational principle) and **updates references to them**, not their content.
- **No methodology architectural rewrite**: METHODOLOGY.md substantive rewrite (§0 abstract, §2.1 role redistribution, §2.2 IPC framing, §4-§6 economics/throughput) is A'.0.7 scope (separate architectural deliberation session). A'.0.5 only **scrubs terminology** (Gemma → unified Claude Desktop, "4-agent" → "2-agent", Cline references removed) — mechanical, not architectural.
- **No K-closure report authoring**: A'.8 scope. A'.0.5 does not author closure reports.
- **No analyzer milestone work**: A'.9 scope.
- **No code changes** beyond comment/doc edits within source files (XML doc-comments referencing deleted types, `// TODO:` references to retired patterns, etc.). Method bodies untouched. No refactoring.
- **No test changes**. Test count delta zero throughout closure.
- **No commit message rewriting** of historical commits.
- **No git history rewrite** (rebase, squash, force-push). Only forward commits.

---

## §2 Pre-flight (Phase 0)

### §2.1 Prerequisite verification

Session begins by verifying:

- [ ] HEAD on `main` is `45d831c` or descendant (K-L3.1 closure commit)
- [ ] Working tree clean (`git status` shows nothing modified, nothing staged, nothing untracked)
- [ ] Tests baseline holds: `dotnet build` succeeds; `dotnet test` shows 631 passing (or current count if K9 has closed since — unlikely given Phase A' sequencing)
- [ ] No active feature branch in mid-execution state (`git branch` shows main + possibly clean feature branches; no in-progress migration branches)
- [ ] `docs/architecture/` directory exists and contains `PHASE_A_PRIME_SEQUENCING.md` (verifies K-L3.1 closure structurally)

If any check fails: STOP, escalate to Crystalka, do not proceed with Phase 1.

### §2.2 Pre-flight sanity grep

Run pre-flight grep for terms that will be search-and-replaced later, to record baseline counts:

```bash
# Gemma references (target: 0 after Phase 7)
grep -r -n "Gemma" docs/ tools/ src/ --include="*.md" | wc -l
grep -r -n "LM Studio" docs/ tools/ src/ --include="*.md" | wc -l
grep -r -n "Cline" docs/ tools/ src/ --include="*.md" | wc -l

# 4-agent framing (target: ≤ 1 historical reference per Phase 7 disposition)
grep -r -n "four agent" docs/ --include="*.md" | wc -l
grep -r -n "4-agent" docs/ --include="*.md" | wc -l

# K-L3 wording staleness (target: unchanged in A'.0.5; addressed in A'.1)
grep -r -n "без exception" docs/ --include="*.md" | wc -l
grep -r -n "K-L3 violation" docs/ --include="*.md" | wc -l

# Deleted stub references (target: 0 in 5 READMEs after Phase 5)
grep -r -n "AmmoComponent\|ShieldComponent\|WeaponComponent\|SchoolComponent\|SocialComponent\|BiomeComponent" \
  src/DualFrontier.Components/ --include="README.md" | wc -l
```

Record baseline counts in scratch file `/tmp/A_05_BASELINE.md`. Phase 8 (closure verification) re-runs the same greps and confirms expected reductions.

---

## §3 Discovery scan (Phase 1)

### §3.1 Full repository documentation inventory

Scan and inventory all documentation files in repo. Use `directory_tree` then `find` for `.md` files specifically.

Record full inventory in scratch file `/tmp/A_05_INVENTORY.md` with this structure:

```
## Top-level
- README.md
- LICENSE.md (if exists)
- CONTRIBUTING.md (if exists)

## docs/ flat-level
- docs/ARCHITECTURE.md
- docs/CONTRACTS.md
- ...

## docs/architecture/
- docs/architecture/PHASE_A_PRIME_SEQUENCING.md

## docs/audit/
- (if exists)

## tools/briefs/
- tools/briefs/K0_*.md
- tools/briefs/K1_*.md
- ...

## src/DualFrontier.<Module>/
- src/DualFrontier.Core/README.md
- src/DualFrontier.Core/<other docs>
- src/DualFrontier.Components/README.md
- src/DualFrontier.Components/<slice>/README.md (per slice)
- ...

## mods/ (if any docs in mod folders)
- ...

## Other locations (if any)
```

Each file gets one line; later phases reference inventory line.

### §3.2 Per-file classification

For each `.md` file in inventory, classify by category. Categories:

- **A — Architecture spec (LOCKED)**: K-L specifications, M-OS specifications, runtime specs, GPU compute specs. Stable architectural authority documents.
  Examples: `KERNEL_ARCHITECTURE.md`, `MOD_OS_ARCHITECTURE.md`, `RUNTIME_ARCHITECTURE.md`, `GPU_COMPUTE.md`, `MIGRATION_PLAN_KERNEL_TO_VANILLA.md`
  
- **B — Methodology**: pipeline methodology, lessons, process. Stable but evolving.
  Examples: `METHODOLOGY.md`, `MAXIMUM_ENGINEERING_REFACTOR.md`, `PIPELINE_METRICS.md`
  
- **C — Live tracker**: not LOCKED, mutable, tracks state across milestones.
  Examples: `MIGRATION_PROGRESS.md`, `ROADMAP.md`
  
- **D — Brief**: per-milestone execution brief.
  Examples: `tools/briefs/K0_*.md`, `tools/briefs/K8_2_*.md`, `tools/briefs/K_L3_1_*.md`
  
- **E — Discovery / closure / audit**: per-milestone artifact.
  Examples: `tools/briefs/K_LESSONS_BATCH_BRIEF.md` (post-execution remained as historical artifact), `docs/PERFORMANCE_REPORT_K7.md`, `docs/audit/M7_CLOSURE_REVIEW.md`
  
- **F — Module-local doc**: per-module README, design notes, narrative.
  Examples: `src/DualFrontier.Core/README.md`, `src/DualFrontier.Components/Combat/README.md`
  
- **G — Project-level meta**: top-level README, contributing guides, license.
  Examples: `README.md` at repo root
  
- **H — Other / uncategorized**: flag for Crystalka decision.

For each file, record `(path, category, brief notes if relevant)`. Output: `/tmp/A_05_CLASSIFIED.md`.

### §3.3 Stale-content detection (mechanical)

For category F files (module-local docs), perform mechanical staleness scan. For each file, grep for:

- **Deleted type names**: `AmmoComponent`, `ShieldComponent`, `WeaponComponent` (deleted in K8.2 v2), `SchoolComponent`, `SocialComponent`, `BiomeComponent`, `ShieldSystem`, `SocialSystem`, `BiomeSystem`. **Mechanical match → flag**.
- **Stale architectural framing**: `Path α` / `Path β` (without K-L3.1 bridge context), `class component` / `class-based components`, `K-L3 violation`, `K-L3 без exception`, `unmanaged struct mandate`. **Mechanical match → flag for Phase 6 refresh**.
- **Stale pipeline terms**: `Gemma`, `Cline`, `LM Studio`, `local executor`, `quantized model`, `4-agent`, `four agents`, `prompt generator` (when referring to Sonnet role). **Mechanical match → flag for Phase 7 scrub**.
- **Stale code references**: types/methods that no longer exist in `src/`. Cross-reference each named type in module-local doc against actual `src/` content.
- **Stale path references** to other docs: `(./KERNEL_ARCHITECTURE.md)`, `(/docs/MOD_OS_ARCHITECTURE.md)` — these become stale post-Phase 3 reorganization.

For each flagged file, record:
- File path
- Flag category (deleted-type / stale-framing / stale-pipeline / stale-code-ref / stale-path-ref)
- Specific match (line number + text)
- Disposition recommendation (auto-fix terminology / refresh prose / Crystalka deliberation)

Output: `/tmp/A_05_STALENESS_REPORT.md`.

### §3.4 Pipeline-terminology scan

Cross-cutting scan for pipeline-restructure terminology beyond category F. For all `.md` files in repo:

```bash
grep -r -l -E "Gemma|LM Studio|Cline|4-agent|four[- ]agent|four agents|local executor|local quantized|prompt generator" \
  docs/ tools/ src/ --include="*.md"
```

Record each affected file. Some files may have **substantial** Gemma references (METHODOLOGY.md likely highest density). Phase 7 disposition decisions per file:

- **Mechanical scrub**: terminology replacement, surrounding prose stays meaningful → auto-fix
- **Substantial rewrite needed**: removing Gemma references would invalidate paragraph meaning → flag for A'.0.7 scope (NOT A'.0.5)
- **Historical reference appropriate to retain**: e.g., a K1 lesson referencing "the K1 brief assumed quantized executor would..." — the historical claim is true even if the architecture has moved; keep as historical → flag for Crystalka decision

Output: `/tmp/A_05_PIPELINE_TERMINOLOGY.md`.

### §3.5 Discovery summary report

Compose `/tmp/A_05_DISCOVERY_SUMMARY.md` aggregating:

- Total file counts per category
- Total stale items detected (per type)
- Recommended Phase 2 reorganization mapping (preview)
- Tier 2 / escalation candidates (files needing Crystalka decision before action)

This report is presented at **Stop #1** (Phase 2 entry) for Crystalka approval.

---

## §4 Reorganization plan authoring (Phase 2)

### §4.1 Stop #1 — Reorganization plan presented to Crystalka

Phase 2 is **plan authoring + Crystalka approval**, not execution. After Phase 1 discovery completes, Phase 2 produces a structured plan:

**File**: `/tmp/A_05_REORG_PLAN.md`

Structure of plan:

```
## Source → destination mapping

### Architecture spec (Category A) → docs/architecture/
- docs/KERNEL_ARCHITECTURE.md → docs/architecture/KERNEL_ARCHITECTURE.md
- docs/MOD_OS_ARCHITECTURE.md → docs/architecture/MOD_OS_ARCHITECTURE.md
- docs/RUNTIME_ARCHITECTURE.md → docs/architecture/RUNTIME_ARCHITECTURE.md
- docs/GPU_COMPUTE.md → docs/architecture/GPU_COMPUTE.md
- docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md → docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md
- docs/K_L3_1_AMENDMENT_PLAN.md → docs/architecture/K_L3_1_AMENDMENT_PLAN.md (if amendment plan classified A)

### Methodology (Category B) → docs/architecture/methodology/ ? OR docs/architecture/ flat ? OR docs/ stays as is?
- docs/METHODOLOGY.md → ? (decision needed)
- docs/MAXIMUM_ENGINEERING_REFACTOR.md → ?
- docs/PIPELINE_METRICS.md → ?

### Live tracker (Category C) → docs/migration/ ? OR stays at docs/?
- docs/MIGRATION_PROGRESS.md → ? (decision needed — tracker is mutable, may not belong with LOCKED archs)
- docs/ROADMAP.md → ? (decision needed)

### Brief (Category D) → docs/briefs/ ? OR tools/briefs/ stays?
- tools/briefs/K0_*.md → ? (decision: keep at tools/briefs/ or migrate to docs/briefs/)

### Discovery / closure / audit (Category E) → ?
- docs/PERFORMANCE_REPORT_K7.md → docs/architecture/reports/ ?
- docs/audit/M7_CLOSURE_REVIEW.md → ?

### Module-local doc (Category F) → STAYS in src/<Module>/
(no path changes; only content refreshes in Phase 6)

### Project meta (Category G) → STAYS at repo root
(README.md etc unchanged)

## Ambiguous classifications (Crystalka decision needed)
- file X: classified ?, possible categories: ...
- ...

## Cross-reference impact preview
- N files reference KERNEL_ARCHITECTURE.md by path; will need update
- ...
```

### §4.2 Plan reviewed by Crystalka

Crystalka receives the plan in chat (or reads `/tmp/A_05_REORG_PLAN.md`). Crystalka responses needed:

- **Confirm or adjust** each Category B/C/D/E destination (the categories with ambiguous default destinations)
- **Resolve** any "Ambiguous classifications" entries
- **Approve or amend** ordering of subsequent phases

If Crystalka has substantive disagreements with the plan: STOP, revise plan, present again. Do not proceed to Phase 3 without explicit Crystalka approval.

If Crystalka has minor adjustments: edit plan in place, get final approval, then proceed.

### §4.3 Plan finalization

After Crystalka approval, rename `/tmp/A_05_REORG_PLAN.md` to `/tmp/A_05_REORG_PLAN_APPROVED.md` (or commit the plan as part of A'.0.5 closure for audit trail). Phase 3 executes against approved plan.

---

## §5 File reorganization execution (Phase 3)

### §5.1 Execute file moves via `git mv`

For each move in approved plan, use `git mv <source> <destination>` to preserve git history (history follows the file).

**Atomic commit per category move**:

- Commit 1: `docs(architecture): move LOCKED architecture specs to docs/architecture/`
  - `git mv docs/KERNEL_ARCHITECTURE.md docs/architecture/KERNEL_ARCHITECTURE.md`
  - `git mv docs/MOD_OS_ARCHITECTURE.md docs/architecture/MOD_OS_ARCHITECTURE.md`
  - ... (per approved plan)
  - `git commit -m "docs(architecture): move LOCKED architecture specs to docs/architecture/`<newline><newline>`Per Crystalka direction 2026-05-10 reorganization principle. Cross-references will be updated in subsequent commits.`<newline><newline>`Affected files:`<newline>`- KERNEL_ARCHITECTURE.md`<newline>`- MOD_OS_ARCHITECTURE.md`<newline>`- ...`<newline><newline>`Co-Authored-By: Claude <noreply@anthropic.com>`<newline>`A'.0.5 Phase 3 commit 1 of N."`

- Commit 2: `docs(methodology): relocate methodology docs per organizational principle`
  - (per approved plan; only if methodology docs relocate — depends on Phase 2 decision)

- Commit 3: `docs(migration): relocate live trackers per organizational principle`
  - (per approved plan; only if live trackers relocate)

- Commit 4: `docs(briefs): relocate briefs per organizational principle`
  - (per approved plan; only if briefs relocate)

- Commit 5: ... per remaining categories per approved plan.

### §5.2 Each commit must leave repo buildable

Per K-Lessons Lesson 1 «atomic commit as compilable unit»: each commit, even file moves, must leave the repo in a state where `dotnet build` succeeds. File moves don't break build directly (no source code references doc paths), but verify after each commit:

```bash
dotnet build  # confirm no breakage
```

If build breaks unexpectedly: STOP, investigate, escalate to Crystalka.

### §5.3 Verification after Phase 3

After all moves complete:

- `git status` shows clean (no leftover untracked or modified files from move operations)
- `find docs/ -name "*.md"` matches expected post-reorganization layout
- `find tools/briefs -name "*.md"` matches expected (if briefs relocated, this should be empty or only contain remnants per plan)
- `dotnet build` succeeds
- `dotnet test` succeeds, count unchanged (still 631 or current baseline)

---

## §6 Cross-reference refresh (Phase 4)

### §6.1 Identify all stale path references

After Phase 3, every reference to a moved file is now stale. Identify them:

```bash
# Find stale references to moved architecture files
grep -r -n -E "\(\.?\.?/?docs/KERNEL_ARCHITECTURE\.md\)|\(\.?\.?/?docs/MOD_OS_ARCHITECTURE\.md\)|\(\.?\.?/?docs/RUNTIME_ARCHITECTURE\.md\)|\(\.?\.?/?docs/GPU_COMPUTE\.md\)|\(\.?\.?/?docs/MIGRATION_PLAN_KERNEL_TO_VANILLA\.md\)" \
  docs/ tools/ src/ --include="*.md"

# Find stale references to moved methodology files (if any moved)
grep -r -n "docs/METHODOLOGY\.md" docs/ tools/ src/ --include="*.md"

# Find stale references to moved live trackers (if any moved)
grep -r -n "docs/MIGRATION_PROGRESS\.md\|docs/ROADMAP\.md" docs/ tools/ src/ --include="*.md"

# ...etc per category
```

Compile inventory of stale references: file path + line number + current text + target replacement.

Output: `/tmp/A_05_STALE_REFS.md`.

### §6.2 Atomic commit per cross-ref category

Update references in atomic commits, per category:

- Commit: `docs: update cross-references to relocated architecture specs`
  - All `(./KERNEL_ARCHITECTURE.md)` → `(./architecture/KERNEL_ARCHITECTURE.md)` (or correct relative path per where the referencing file lives)
  - Same for MOD_OS, RUNTIME, GPU_COMPUTE, etc.
  - Use `sed` or bulk find-and-replace; verify before commit

- Commit: `docs: update cross-references to relocated methodology docs` (if applicable)

- Commit: `docs: update cross-references to relocated live trackers` (if applicable)

- Commit: `docs: update cross-references to relocated briefs` (if applicable)

### §6.3 Per-file relative path correction

When file A at location `docs/X/Y/A.md` references file B at `docs/architecture/B.md`, the relative path depends on A's location. After moves, every reference needs path-correctness check.

Standard approach: use **repo-rooted relative paths** where possible (`/docs/architecture/B.md`) or **explicit traversal** (`../../architecture/B.md`). Either works; pick one convention per Crystalka preference (record decision in commit message).

Default convention if Crystalka has no preference: **repo-rooted absolute paths** (start with `/docs/...`) for clarity. Markdown rendering on GitHub treats these as repo-relative.

### §6.4 Verification after Phase 4

```bash
# Verify no stale references remain
grep -r -n -E "\(\.?\.?/?docs/KERNEL_ARCHITECTURE\.md\)" docs/ tools/ src/ --include="*.md"
# Should return zero results (all references updated)

# Verify all new references resolve
# (manual check or scripted verification of file existence at referenced paths)
```

---

## §7 README stub-reference cleanup (Phase 5)

### §7.1 Scope

The 5 module READMEs flagged in pre-K-L3.1 cleanup session (and detailed in `/mnt/user-data/uploads/SESSION_LOG_K_SERIES_2026_05_09.md` Раздел 8.1):

1. `src/DualFrontier.Components/README.md` (top-level) — STALE (TODO list, class references)
2. `src/DualFrontier.Components/Combat/README.md` — HEAVILY STALE (3 of 4 components deleted)
3. `src/DualFrontier.Components/World/README.md` — STALE (Biome deleted)
4. `src/DualFrontier.Components/Magic/README.md` — STALE (School deleted, references to MagicSchool enum)
5. `src/DualFrontier.Components/Pawn/README.md` — STALE (Social deleted, SocialSystem references)

Also possibly affected:
- `src/DualFrontier.Components/Shared/README.md` — was CLEAN per session log; verify
- `src/DualFrontier.Components/Items/README.md` — was CLEAN per session log; verify
- `src/DualFrontier.Components/Building/README.md` — was CLEAN per session log; verify

### §7.2 Cleanup operation per file

For each of the 5 stale READMEs:

- **Read current content**
- **Identify stub references**: explicit mentions of `AmmoComponent`, `ShieldComponent`, `WeaponComponent`, `SchoolComponent`, `SocialComponent`, `BiomeComponent`, `ShieldSystem`, `SocialSystem`, `BiomeSystem`, plus consequences (e.g., "MagicSchool enum" in Magic README)
- **Remove**: deleted-component references, the related TODO bullets, narrative paragraphs that are now meaningless without the deleted component
- **Preserve**: surviving content unchanged
- **DO NOT rewrite narrative wholesale**: this is **stub-reference cleanup** (mechanical removal of references to deleted things), NOT module-local doc refresh (semantic). Module-local doc refresh is Phase 6.

### §7.3 Atomic commit shape

Single commit covering all 5 READMEs:

```
docs(components): remove deleted-stub references from module READMEs

K8.2 v2 closure (commit 7527d00) deleted 6 empty TODO stub components
(Ammo, Shield, Weapon, School, Social, Biome) and 3 orphan systems
(ShieldSystem, SocialSystem, BiomeSystem) per METHODOLOGY §7.1 «data
exists or it doesn't». Module READMEs continued to reference these
deleted types. This commit removes those references mechanically.

Affected files:
- src/DualFrontier.Components/README.md
- src/DualFrontier.Components/Combat/README.md
- src/DualFrontier.Components/World/README.md
- src/DualFrontier.Components/Magic/README.md
- src/DualFrontier.Components/Pawn/README.md

Scope: stub-reference removal only. Substantive narrative refresh
deferred to A'.0.5 Phase 6 (module-local doc refresh).

Co-Authored-By: Claude <noreply@anthropic.com>
A'.0.5 Phase 5.
```

### §7.4 Verification after Phase 5

```bash
grep -r -n "AmmoComponent\|ShieldComponent\|WeaponComponent\|SchoolComponent\|SocialComponent\|BiomeComponent\|ShieldSystem\|SocialSystem\|BiomeSystem" \
  src/DualFrontier.Components/ --include="README.md"
# Should return zero results
```

---

## §8 Module-local doc refresh (Phase 6)

### §8.1 Stop #2 — Architectural-meaning boundary

Phase 6 refreshes per-module documentation prose against actual post-K8.2v2 reality. **Critical discipline**: refresh updates terminology and removes references to deleted/renamed/moved entities. Refresh **does not** introduce new architectural claims, decisions, or interpretations.

If during Phase 6 a refresh would change architectural meaning (not just terminology), STOP, escalate to Crystalka. Examples:

- **Mechanical (proceed)**: "Combat module contains AmmoComponent, ShieldComponent, WeaponComponent, ArmorComponent" → "Combat module contains ArmorComponent" (after removing deleted stubs).
- **Mechanical (proceed)**: "Components are class-based; conversion to struct planned" → "Components are unmanaged structs (post-K8.2v2)" (terminology refresh per current reality).
- **Architectural (STOP)**: "Combat module is the entry point for damage routing; future expansion will add ranged combat" → considering renaming "future expansion" or removing it entirely. The doc claims architectural intent (ranged combat plan); whether to retain or modify is decision-bearing → escalate.
- **Architectural (STOP)**: README claims K8.2v2 closed «K-L3 «без exception» state achieved». Replacing with updated language requires K-L3.1 amendment plan synthesis, which is A'.1 scope, not A'.0.5. → escalate per disposition (likely flag for A'.1 propagation).
- **Architectural (STOP)**: a module README describes a system's interaction with another module that no longer exists in current code. Whether the README's narrative claim is reframed or deleted depends on what the module actually does now → requires reading current source, possibly architectural deliberation → escalate.

When in doubt: STOP and escalate. Defensive discipline preferred over architectural improvisation.

### §8.2 Per-module refresh scope

For each Category F file (per Phase 1 classification) flagged in Phase 1 staleness report:

1. **Read current source** in module folder (key types, key systems, key public surface)
2. **Read current architectural state** in relevant LOCKED docs (now at `docs/architecture/`)
3. **Read current doc claims**
4. **Diff**: claims vs reality
5. **Disposition**:
   - **Auto-fix**: terminology updates, deleted-type removal, code-reference correction (see §8.3 mechanical patterns)
   - **Refresh prose**: prose paragraph that's now meaningless or partially meaningless after auto-fix; rewrite the paragraph to accurately describe current module reality. Only if the rewrite is **descriptive** (describes what is), not **prescriptive** (describes what should be).
   - **Flag for Crystalka**: anything decision-bearing per §8.1.
   - **Defer to A'.0.7**: if doc has methodology framing that requires A'.0.7 work to refresh coherently (e.g., narrative discusses pipeline architecture).

### §8.3 Mechanical patterns auto-fixable in Phase 6

Without escalation:

- **Deleted type names**: remove or replace per current reality
- **Renamed type names**: replace with current name
- **Stale K-Lxx wording**: «без exception» → «selective per-component path per K-L3.1»
- **Stale class/struct framing**: «class-based components» → «unmanaged struct components (post-K8.2v2)»
- **Stale K-numbers**: K8.2 references describing old (Movement/Identity/Skills/Social/Storage/Workbench/Faction-only) scope → updated per K8.2 v2 actual scope (31 components)
- **Stale managed/native framing**: «managed World as production storage» → «native World production; managed retained as test fixture per K-L11»
- **Path α references** (without bridge context): updated per K-L3.1 lock (Path α default + Path β bridge for managed-class fallback)

### §8.4 Per-module commit shape

One atomic commit per module (or per related-modules group), e.g.:

- Commit: `docs(core): refresh README and module docs for post-K8.2v2 reality`
  - Affected files: `src/DualFrontier.Core/README.md`, related design notes if exist
- Commit: `docs(components): refresh module-level docs per post-K8.2v2 component shapes`
- Commit: `docs(systems): refresh module-level docs per post-K8.2v2 system inventory`
- Commit: `docs(modding): refresh modding module docs for current ModApi v2 state`
- Commit: `docs(interop): refresh interop module docs for K8.1 wrapper value-type state`
- Commit: ... per remaining modules.

Within commit messages, reference specific architectural changes that motivated each refresh:

```
docs(components): refresh module-level docs per post-K8.2v2 component shapes

K8.2 v2 closure (commit 7527d00) converted 6 class components to
unmanaged structs, deleted 6 empty TODO stubs, retained 25 components
all carrying [ModAccessible]. Module-level READMEs continued to claim
the pre-K8.2v2 component inventory. This commit refreshes the prose
to describe the current (post-K8.2v2) reality.

Refresh discipline (per A'.0.5 §8 Stop #2): terminology and
deleted-type references updated; architectural meaning preserved.
Items flagged for Crystalka (architectural framing requiring A'.1
amendment context) excluded from this commit per Stop #2.

Co-Authored-By: Claude <noreply@anthropic.com>
A'.0.5 Phase 6.
```

### §8.5 Kernel-area priority

Per Crystalka direction («особенно те что про ядро»), kernel-area module docs get priority attention:

- `src/DualFrontier.Core/`
- `src/DualFrontier.Core.Interop/`
- `src/DualFrontier.Contracts/` (especially `Contracts/Modding/IModApi.cs` doc-comments)

These are processed first in Phase 6. Other modules (Components/, Systems/, Modding/, Application/, Persistence/, Events/, Runtime/, Presentation/) processed after kernel-area complete.

### §8.6 Output

After Phase 6 closes:

- All Category F files refreshed per disposition
- Auto-fixable items applied
- Architectural items flagged in `/tmp/A_05_FLAGGED_FOR_A1.md` (forward to A'.1)
- A'.0.7 candidates flagged in `/tmp/A_05_FLAGGED_FOR_A07.md` (forward to A'.0.7)

### §8.7 Verification after Phase 6

- Re-run staleness scan from Phase 1 §3.3 against Category F files
- Confirm: deleted type names → 0 matches in module docs
- Confirm: stale K-Lxx wording → 0 in module docs
- Architectural-flagged items → counted, output present in flagged-forward files

---

## §9 Pipeline-terminology scrub (Phase 7)

### §9.1 Stop #3 — Substantive rewrite boundary

Phase 7 scrubs Gemma/Cline/4-agent terminology mechanically. Substantive methodology rewrite is A'.0.7 scope, not A'.0.5.

If during Phase 7 a scrub would require substantive rewrite (paragraph meaning depends on Gemma-era framing), STOP. Flag for A'.0.7. Do not improvise replacement prose for substantive sections.

Examples:

- **Mechanical (proceed)**: README mentions "Cline orchestrates Gemma local executor for batch tasks" → remove Cline reference, replace Gemma → Claude Desktop session, retain otherwise.
- **Mechanical (proceed)**: A brief reference says "The four-agent pipeline (Opus, Sonnet, Gemma, Crystalka) processes briefs via..." → revise to "The pipeline (Crystalka + Claude Desktop session) processes briefs via..." (this rewrite is mechanical because it preserves descriptive content)
- **Substantive (STOP, flag for A'.0.7)**: METHODOLOGY.md §0 Abstract: "The configuration is four agents with explicitly distributed roles: a local quantized model in the 4–8B parameter class as code executor, a mid-tier cloud model as prompt generator, a top-tier cloud model as architect and QA, the human as direction owner." This is a substantive architectural claim about the methodology; replacing requires deliberation → escalate to A'.0.7.
- **Substantive (STOP, flag for A'.0.7)**: METHODOLOGY.md §2.1 «Local executor» entire section: detailed technical claims about quantized model. Removing entirely vs reframing as historical vs replacing with new section requires architectural decision → A'.0.7.
- **Substantive (STOP, flag for A'.0.7)**: PIPELINE_METRICS.md (referenced in METHODOLOGY §0): empirical metrics presumably gathered with Gemma in pipeline. Continued accuracy under new pipeline unknown → A'.0.7 deliberates whether to discard, reframe as historical, or recollect.

### §9.2 Mechanical scrub patterns

For Phase 7 auto-fixable items:

- **Brief incidental mentions**: a sentence in some brief mentioning "via the Gemma executor" → rewrite to "via the Claude Desktop session" or similar; keeps surrounding context meaningful
- **Tool reference deletions**: where Cline appears as orchestrator without other architectural meaning, delete the reference entirely; sentence often readable without it
- **"4-agent" → "2-agent" terminology**: where the term appears as labeling without substantive elaboration, replace mechanically

### §9.3 Affected files (likely)

Per Phase 1 §3.4 pipeline-terminology scan, expected affected files:

- `docs/METHODOLOGY.md` — high density of pipeline references; expect mostly STOP/flag for A'.0.7
- `docs/PIPELINE_METRICS.md` — likely STOP/flag entirely for A'.0.7
- Various briefs (`tools/briefs/K0_*.md`, etc.) — possibly incidental mentions; mechanical scrub likely
- `docs/MAXIMUM_ENGINEERING_REFACTOR.md` — possibly mentions; check
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` — Phase A' doc was authored 2026-05-10 against 4-agent assumption; mechanical scrub likely sufficient (it doesn't substantively analyze pipeline architecture, only mentions Cloud Code as executor)
- Top-level `README.md` (if it discusses methodology) — check
- Module-local READMEs may have incidental pipeline mentions; check during Phase 6 already

### §9.4 Atomic commit shape

One commit per category of files:

- Commit: `docs(briefs): scrub Gemma/Cline references from historical briefs`
  - Mechanical scrub of incidental Gemma references in `tools/briefs/K*.md` files
  - Surrounding prose preserved; meaning intact
- Commit: `docs(architecture): scrub pipeline terminology from Phase A' doc and similar`
  - Phase A' doc, top-level README if affected, etc.
  - All mechanical; substantive rewrite explicitly NOT included
- Commit: `docs(methodology): mark METHODOLOGY substantive sections as deferred to A'.0.7`
  - May NOT actually edit METHODOLOGY substantively; may instead add a `<!-- TODO: A'.0.7 -->` HTML comment marker at top with explicit deferral note
  - This commit's role is to document the deferral, not to edit substance

### §9.5 Verification after Phase 7

```bash
# Mechanical scrub verification
grep -r -n -E "Gemma|Cline|LM Studio" tools/briefs/ docs/architecture/ --include="*.md"
# Expect: zero or only historical references in commit messages / closure reports

grep -r -n "4-agent\|four[- ]agent\|four agents" docs/architecture/ tools/ --include="*.md"
# Expect: zero or only historical references

# METHODOLOGY.md substantive scrubs NOT yet applied (deferred)
grep -r -n "Gemma" docs/architecture/METHODOLOGY.md
# Expect: matches still present, with A'.0.7 deferral marker at top of file
```

---

## §10 Discovery cleanup pass (Phase 8)

### §10.1 Stop #4 — Architectural-character debt boundary

Throughout Phases 1-7, surfaced debt items not directly in scope. Phase 8 categorizes:

- **Tier 1 — Auto-fix now**: items mechanical in nature, surfaced incidentally, fixable in single commit
  - Examples: typo in a doc, missing word, broken markdown formatting, inconsistent capitalization in doc headings, dead `<!-- TODO -->` markers from completed work
- **Tier 2 — Flag for Crystalka, defer**: items with architectural character or scope ambiguity
  - Examples: inconsistencies between two LOCKED docs that suggest one of them is wrong, narrative claims that may have been right at authoring time but are now subtly off

### §10.2 Tier 1 atomic commit

One commit covering Tier 1 fixes:

```
docs: incidental cleanup discovered during A'.0.5 reorganization

Items fixed in this commit (mechanical, non-architectural):
- typo: "comprehensive" misspelled in <file:line>
- markdown: header level inconsistency in <file:line>
- ...

Tier 1 items only. Tier 2 items flagged separately for Crystalka
deliberation (see /tmp/A_05_TIER2_FLAGS.md, forwarded to MIGRATION_PROGRESS).

Co-Authored-By: Claude <noreply@anthropic.com>
A'.0.5 Phase 8.
```

### §10.3 Tier 2 flag aggregation

Tier 2 items aggregated into `/tmp/A_05_TIER2_FLAGS.md`. Each entry includes:

- File + line
- Surfaced text
- Suspected issue
- Recommended next step (e.g., "raise in K-L3.1 amendment for resolution", "raise in A'.0.7 methodology session", "open question for Crystalka deliberation outside any specific milestone")

This file forwards into MIGRATION_PROGRESS A'.0.5 closure entry "Tier 2 surfaced debt" sub-section.

### §10.4 Verification after Phase 8

- All Tier 1 items resolved
- All Tier 2 items recorded in flags file
- No new in-scope debt outstanding

---

## §11 Closure (Phase 9)

### §11.1 Update Phase A' sequencing document

Edit `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (now at its post-Phase-3 location) reflecting:

- A'.0.5 closure status (DONE, with commit reference TBD until commit lands)
- A'.2 README cleanup explicitly folded into A'.0.5 (note in §2 Phase A' sequence)
- A'.0.7 methodology rewrite milestone inserted between A'.0.5 and A'.1 (per pipeline restructure deliberation requirement)
- Updated sequence diagram in §2

Atomic commit:

```
docs(architecture): update Phase A' sequencing per A'.0.5 closure

A'.2 README cleanup folded into A'.0.5 Phase 5 per execution
unification. A'.0.7 methodology pipeline restructure inserted
between A'.0.5 and A'.1 (architectural deliberation milestone
analog to K-L3.1).

Updated §2 sequence diagram, §3 cumulative duration estimate,
§5 document amendment schedule.

Co-Authored-By: Claude <noreply@anthropic.com>
A'.0.5 Phase 9 commit 1 of 3.
```

### §11.2 Update MIGRATION_PROGRESS

Add A'.0.5 closure entry to MIGRATION_PROGRESS (now at its post-Phase-3 location, possibly `docs/architecture/MIGRATION_PROGRESS.md` or `docs/migration/MIGRATION_PROGRESS.md` per Phase 2 plan).

Entry shape (analog K-Lessons closure entry):

```markdown
### A'.0.5 — Documentation reorganization + cross-ref refresh + module-local refresh + cleanup campaign

**Status**: DONE
**Closure**: <SHA range> on <branch> (fast-forward merged to main if branch used; otherwise direct on main)
**Brief**: tools/briefs/A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md (FULL EXECUTED)
**Test count**: 631 unchanged (documentation-only milestone)

**Phases completed**:
- Phase 0 pre-flight verified
- Phase 1 discovery: <N> .md files inventoried, <N> classified, <N> stale items detected
- Phase 2 reorganization plan approved by Crystalka
- Phase 3 file moves: <N> files relocated
- Phase 4 cross-reference refresh: <N> stale refs updated
- Phase 5 README stub-reference cleanup: 5 READMEs refreshed
- Phase 6 module-local doc refresh: <N> module docs refreshed
- Phase 7 pipeline terminology scrub: <N> mechanical fixes; METHODOLOGY substantive scrubs deferred to A'.0.7
- Phase 8 discovery cleanup: Tier 1 fixed, Tier 2 flagged
- Phase 9 closure (this update)

**Tier 2 surfaced debt** (forwarded for future deliberation):
- (per /tmp/A_05_TIER2_FLAGS.md aggregated content)

**Items forwarded to A'.0.7**:
- (per /tmp/A_05_FLAGGED_FOR_A07.md aggregated content)

**Items forwarded to A'.1**:
- (per /tmp/A_05_FLAGGED_FOR_A1.md aggregated content)

**Brief deviations**:
- (any deviations from this brief, recorded honestly)

**Lessons learned**:
- (any lessons surfaced during execution worth formalizing in K-Lessons or methodology)
```

Atomic commit:

```
docs(migration): record A'.0.5 closure in MIGRATION_PROGRESS

Documentation reorganization + cross-reference refresh + module-local
refresh + pipeline-terminology scrub + cleanup campaign closed
successfully. Tier 2 debt and forward-flagged items recorded for
A'.0.7, A'.1 deliberation.

Co-Authored-By: Claude <noreply@anthropic.com>
A'.0.5 Phase 9 commit 2 of 3.
```

### §11.3 Mark brief EXECUTED

Edit `tools/briefs/A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md` (now possibly at `docs/briefs/` per Phase 2 plan) status frontmatter:

```yaml
**Status**: EXECUTED — closure SHA <range>, MIGRATION_PROGRESS A'.0.5 entry filled
```

Atomic commit:

```
docs(briefs): mark A'.0.5 brief EXECUTED

A'.0.5 closure verified per MIGRATION_PROGRESS entry.

Co-Authored-By: Claude <noreply@anthropic.com>
A'.0.5 Phase 9 commit 3 of 3.
```

### §11.4 Final verification

- `git status` clean
- `dotnet build` succeeds
- `dotnet test` 631 passing (or current baseline if K9 has closed since)
- Re-run pre-flight grep from §2.2; record final counts. Expected reductions confirmed.
- Final commit count: ~25-40 atomic commits across Phases 3-9 (exact count depends on Phase 1 discovery and per-category complexity)

---

## §12 Stop conditions summary

Five stop conditions across phases:

| # | Phase | Trigger | Action |
|---|---|---|---|
| 1 | 2 | Phase 1 discovery complete; reorganization plan written | Present plan, await Crystalka approval, do not proceed to Phase 3 without explicit approval |
| 2 | 6 | Module-local refresh would change architectural meaning, not just terminology | STOP, escalate to Crystalka, flag for A'.1 or A'.0.7 per disposition |
| 3 | 7 | Pipeline terminology scrub would require substantive rewrite | STOP, flag for A'.0.7, do not improvise substantive replacement prose |
| 4 | 8 | Surfaced debt has architectural character | Tier 2 flag, do not Tier 1 auto-fix |
| 5 | any | Working tree gets corrupted, build breaks unexpectedly, test count regresses | STOP, escalate to Crystalka, do not auto-recover |

Plus general gates:
- Phase 0 pre-flight failures → STOP
- Any commit verification failure (build broken, tests broken) → STOP

---

## §13 Estimated commit shape

Approximate commit count by phase:

| Phase | Approx commits | Notes |
|---|---|---|
| 0 | 0 | Pre-flight, no commits |
| 1 | 0 | Discovery, no commits |
| 2 | 0-1 | Plan authoring; possibly 1 commit if plan committed for audit trail |
| 3 | 4-8 | One per category move (architecture, methodology, trackers, briefs, etc. — exact count per Phase 2 plan) |
| 4 | 3-5 | One per cross-ref category |
| 5 | 1 | Single README cleanup commit |
| 6 | 6-12 | One per module (kernel-area first, then others) |
| 7 | 2-4 | Mechanical scrubs by category + METHODOLOGY deferral marker |
| 8 | 1 | Tier 1 fixes single commit |
| 9 | 3 | Sequencing update + MIGRATION_PROGRESS entry + brief EXECUTED |

**Total estimate**: 20-34 commits.

Range driven primarily by Phase 1 discovery results (how many files in each category) and Phase 6 module count.

---

## §14 LOC delta estimate

Approximate change scope:

- Phase 3 file moves: zero LOC delta (moves preserve content)
- Phase 4 cross-ref updates: ~50-200 LOC (path string replacements)
- Phase 5 README cleanup: ~50-150 LOC removed (stub-reference paragraphs deleted)
- Phase 6 module-local refresh: ~200-600 LOC modified (mix of additions and deletions, net possibly slightly negative as stale claims removed and concise refresh prose written)
- Phase 7 mechanical scrub: ~30-100 LOC (terminology replacements)
- Phase 8 Tier 1 fixes: ~10-30 LOC
- Phase 9 closure docs: ~50-150 LOC (sequencing update + MIGRATION_PROGRESS entry)

**Total estimated LOC delta: ~ +200/-400 net** (modest net deletion as stale claims removed across many docs).

**Test count delta: zero**.

---

## §15 What this brief deliberately does not include

To prevent scope creep:

- **Source code changes**: only XML doc-comments and `// TODO:` references inside source files are touched, and only if Phase 1 staleness scan flagged specific lines. Method bodies, type definitions, attributes — all untouched.
- **Test changes**: zero. Test count baseline preserved throughout.
- **Git history rewrite**: no rebase, no squash, no force-push. All changes forward-only commits.
- **Architectural decisions**: Phase 6 §8.1 Stop #2, Phase 7 §9.1 Stop #3 explicitly forbid improvisation.
- **K-L3.1 amendment work**: forwarded to A'.1.
- **Methodology substantive rewrite**: forwarded to A'.0.7.
- **K-closure report**: forwarded to A'.8.
- **Push to origin**: forwarded to A'.3 (after A'.0.7 and A'.1 close).

---

## §16 Provenance

- **Authored**: 2026-05-10, Opus 4.7, post-K-L3.1 closure cleanup planning session
- **Authority**: Crystalka direction sequence 2026-05-10:
  - «всё что связано с архитектурой в доках я засунул в папку архитектура» (organizational principle)
  - «привести локальные документы в в папках модулей в порядок, а то некоторые изрядно устарели особенно те что про ядро» (module-local refresh scope)
  - «всё делается через десктопное приложение Claude» + «удалить использование в документах упоминание о локальной модели gemma» (pipeline restructure, terminology scrub scope)
  - «Без костылей у меня много времени, а также требуется архитектурная чистота, чтобы проект жил десятилетиями» (overarching discipline)
- **Precedent**: K-Lessons batch brief (documentation-only milestone shape); Audit Campaign Pass 2 (cumulative drift audit pattern, MOD_OS v1.5)
- **Pipeline reality**: 2-agent unified Claude Desktop (Crystalka + Claude session). Brief written for execution within single Claude Desktop session; no agent handoff implied.
- **Memory tracker**: `userMemories` 2026-05-10 entries (4 edits) capture full state including K-L3.1 closure, K9 brief state, analyzer milestone, pipeline restructure
- **Companion documents**:
  - `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (EXECUTED via 45d831c)
  - `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (APPLIED via 45d831c)
  - `docs/K_L3_1_AMENDMENT_PLAN.md` (Phase 4 deliverable, awaits A'.1 execution)
  - `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (Phase A' anchor, updated in this brief Phase 9)

---

**Brief end. Pre-flight (Phase 0) begins when Crystalka invokes the session. Stop #1 fires after Phase 1 discovery completes; Crystalka approves reorganization plan before Phase 3 begins.**
