---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-CLEANUP_CASCADE_BRIEF
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-CLEANUP_CASCADE_BRIEF
---
---
# Brief frontmatter (not REGISTER mirror — brief lives in tools/briefs/ as Tier 3 Category D)
brief_id: CLEANUP_CASCADE_BRIEF
status: EXECUTED
authored: 2026-05-16
executed: 2026-05-16
author: Claude Opus 4.7 (Crystalka deliberation session)
target_executor: Claude Code (auto-mode)
estimated_duration: 4-7 hours auto-mode
brief_type: cleanup execution
authority_chain:
  - DOCUMENTATION_DRIFT_AUDIT_REPORT.md (DOC-E-DOCUMENTATION_DRIFT_AUDIT_REPORT, committed 53697da on codex/audit; merged to main)
  - COMPOSITE_NAMESPACE_DELIBERATION_STATE.md (Crystalka Project file; Q-G-1, Q-G-2, Q-V-2 locks)
  - Crystalka deliberation 2026-05-16 evening session — 4 cleanup locks ratified
---

# Cleanup Cascade Execution Brief

**Brief shape**: Execution-mode brief targeting Claude Code auto-mode. Atomic commit cascade resolving 18 of 19 audit findings (DRIFT-019 is S1 historical residue, explicitly no-action per audit recommendation).

**Authority**: Direct execution against ratified deliberation locks. Authoring agent (Opus deliberation session) cites:
- Audit findings as drift evidence
- Crystalka 4 cleanup locks as architectural disposition
- VULKAN_SUBSTRATE.md v1.0 LOCKED as current authority replacing RUNTIME + GPU_COMPUTE
- SystemExecutionContext.cs verbatim as canonical statement for isolation guard removal
- IGameServices.cs verbatim as canonical statement for IPowerBus deletion

**Scope discipline** (Lesson #14 candidate application):
- In-scope: 18 audit findings (S2-S5 except DRIFT-019)
- Out-of-scope: METHODOLOGY revision (deferred per cleanup lock to post-cleanup timing)
- Out-of-scope: Architectural deliberation on items not audit-surfaced (e.g. Q-K-1 retroactive lock for A'-cycle renumbering pendant — DRIFT-016 addresses partial-renumbering note within PHASE_A_PRIME §2 only)

---

## §1 — Crystalka ratified locks (2026-05-16 deliberation)

The four cleanup-cascade architectural decisions Crystalka ratified before brief authoring. These are **execution authority**, not deliberation surface within this brief.

### §1.1 — G-skeleton brief disposition

**LOCK**: Move all 10 G-skeleton briefs (G0-G9) from `tools/briefs/` to `tools/briefs/historical/`.
- REGISTER paths update: `tools/briefs/G{N}_*.md` → `tools/briefs/historical/G{N}_*.md`
- REGISTER lifecycle transition: `AUTHORED` → `SUPERSEDED`
- Frontmatter banner inserted at top of each moved file documenting supersession source (Q-G-2 LOCK → VULKAN_SUBSTRATE.md sections)

### §1.2 — Superseded K-series brief disposition

**LOCK**: Move post-A'.5 superseded K-series briefs to `tools/briefs/historical/`.
- K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md → historical/ (superseded by K8_34_COMBINED v2)
- K8_4_MANAGED_WORLD_RETIRED_BRIEF.md → historical/ (superseded by K8_34_COMBINED v2)
- K8_3_BRIEF_REFRESH_PATCH.md → historical/ (parent superseded)
- K4_STRUCT_REFACTOR_BRIEF.md → historical/ (already register-SUPERSEDED)
- K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md → historical/ (already register-DEPRECATED)
- K8_2_CLASS_COMPONENT_REDESIGN_BRIEF.md — **verify status first via Phase 0 read**; if orphan / pre-K-L3.1 abandoned, move to historical/; if active reference, leave in place with note

### §1.3 — Visual-runtime authority disposition

**LOCK**: Move VISUAL_ENGINE.md + GODOT_INTEGRATION.md from `docs/architecture/` to `docs/architecture/historical/`.
- REGISTER paths update
- REGISTER lifecycle transition: `Live` → `SUPERSEDED` (or `Historical` if FRAMEWORK.md permits; verify lifecycle vocabulary in Phase 0)
- Frontmatter banner: «Superseded by VULKAN_SUBSTRATE.md v1.0 LOCKED 2026-05-16 per Q-G-1 LOCK. Pre-V-substrate authority preserved for historical record.»
- Cross-references in current Tier 1 docs that point to these files — update to point to VULKAN_SUBSTRATE.md or remove if no longer relevant

### §1.4 — Subsection-level legacy disposition (runtime isolation guard + power subsystem)

**LOCK**: Remove subsections describing runtime isolation guard and CPU power subsystem from current Tier 1 docs. Historical narrative preserved in:
- A'.5 closure narrative (already executed; HALT_REPORT.md + MIGRATION_PROGRESS A'.5 entry when updated)
- K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md (EXECUTED brief documents deletion rationale)
- Future K-closure report (A'.8) carries full narrative

**Pattern established (will be formalized in K-closure report)**:
- **Architectural authorities (entire Tier 1 docs)** → move to `*/historical/` folder
- **Subsection-level legacy content within current docs** → delete outright; closure narratives carry history
- **Superseded briefs** → move to `tools/briefs/historical/`

---

## §2 — Phase 0 — Pre-flight reads (mandatory before any edit)

Lesson #14 candidate application: execution must verify state before action. Pre-existing drift surfaced during earlier ratification execution is the case-study for this discipline.

### §2.1 — Verify post-merge state

Read and verify:

1. `git log --oneline -20` on `main` — confirm both PR #34 (composite namespace ratification) and codex/audit branch (53697da DOCUMENTATION_DRIFT_AUDIT_REPORT) merged. Halt if either absent.
2. `git status` — working tree clean before execution starts.
3. `docs/governance/REGISTER.yaml` head 20 lines — confirm `register_version: "1.2"`, `last_modified_by: "Codex"`, `last_modified: "2026-05-16"`. If register_version differs from 1.2 at execution start, halt and ask Crystalka.
4. `tools/governance/sync_register.ps1 --validate` — exit 0 required as baseline. If validation fails before execution, halt and surface failures.
5. `dotnet build` clean baseline. If build fails before execution, halt — drift surfaced elsewhere.
6. `dotnet test` baseline pass count recorded (target: 620 tests green per A'.5 closure). If suite fails or count diverges, halt and surface.

### §2.2 — Read audit findings as ground truth

Read in full, identify exact line numbers for each finding:
- `docs/audit/DOCUMENTATION_DRIFT_AUDIT_REPORT.md` — 19 findings with code anchors + recommended actions

Per Lesson #7 (transcribe verbatim, never paraphrase): the audit report's evidence sections are **authoritative for what needs changing**. Quote line numbers verbatim into commit messages.

### §2.3 — Read code anchors verbatim

Read these files for verbatim content to transcribe into Tier 1 doc rewrites. Do NOT paraphrase from synthesis — quote directly:

- `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` lines 14-33 — XML doc comment is canonical for DRIFT-003 + DRIFT-005 rewrites
- `src/DualFrontier.Core/ECS/SystemBase.cs` lines 1-100 — canonical for DRIFT-004 ECS rewrite (managed-World access surface removal)
- `src/DualFrontier.Contracts/Bus/IGameServices.cs` — canonical for DRIFT-006 (5 buses, no Power)
- `src/DualFrontier.Contracts/Modding/IModApi.cs` — canonical for DRIFT-011 + DRIFT-012 (strict v3, RegisterManagedComponent, Fields, ComputePipelines, manifest "3")
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` lines 30-70 — canonical for DRIFT-001 + DRIFT-002 (NativeWorld production)

### §2.4 — Read REGISTER.yaml G-series + visual-runtime entries

Identify exact line ranges for entries to update:
- DOC-D-G0 through DOC-D-G9 — path update + lifecycle update
- DOC-D-K8_3, DOC-D-K8_4, DOC-D-K8_3_BRIEF_REFRESH_PATCH, DOC-D-K4_STRUCT_REFACTOR — path update + lifecycle confirmation
- DOC-A-VISUAL_ENGINE, DOC-A-GODOT_INTEGRATION — path update + lifecycle transition
- Verify whether FRAMEWORK.md permits `SUPERSEDED` lifecycle for Tier 1 LOCKED documents. If not, use closest permitted value (e.g. SUPERSEDED + path under historical/ may be sufficient; if FRAMEWORK requires new lifecycle, halt and ask Crystalka)

---

## §3 — Atomic commit cascade (target ~13-15 commits)

Each commit: individually compilable + register-valid intermediate state per Lesson #8. `sync_register --validate` exit 0 at minimum at every governance-touching commit; `dotnet build` clean at every code-touching commit (none expected in this brief — all docs/register).

### Commit 1 — Create historical folders + G-skeleton + K-series brief moves + REGISTER updates atomic

**Files**:
- Create `tools/briefs/historical/` (folder)
- `git mv tools/briefs/G{0..9}_*.md → tools/briefs/historical/`
- `git mv tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md → tools/briefs/historical/`
- `git mv tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md → tools/briefs/historical/`
- `git mv tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md → tools/briefs/historical/`
- `git mv tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md → tools/briefs/historical/`
- `git mv tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md → tools/briefs/historical/`
- If K8_2_CLASS_COMPONENT_REDESIGN_BRIEF status verified orphan in Phase 0 — `git mv` to historical/ in this commit; else defer to Commit 2 with note

**REGISTER.yaml updates** (atomic with file moves to preserve sync_register --validate exit 0):
- 10 G-series entries: `path:` field update + `lifecycle: AUTHORED` → `lifecycle: SUPERSEDED`
- K-series superseded entries: same pattern
- Each entry's `special_case_rationale` field — append note: «Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 / §1.2 (Crystalka lock 2026-05-16).»
- `register_version: "1.2"` → `"1.3"`
- `last_modified: "2026-05-16"`
- `last_modified_by: "Claude Code"`
- `last_modified_commit: PENDING-INITIAL` (will be amended post-commit per A'.4.5 Q-A45-X2 self-reference pattern, OR left as PENDING-INITIAL and resolved at final closure commit per audit's choice)

**Frontmatter banners** at top of each moved file:
```markdown
> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
```
(Banner text adapted per file — G-series cites Q-G-2 LOCK; K-series cites A'.5 K8.3+K8.4 absorption.)

**Validation**: `sync_register.ps1 --validate` exit 0.

**Commit message**: `chore(briefs): move superseded G-skeleton + K-series briefs to historical/ per CLEANUP_CASCADE §1.1-1.2`

### Commit 2 — Visual-runtime authority move + REGISTER updates atomic

**Files**:
- Create `docs/architecture/historical/` (folder)
- `git mv docs/architecture/VISUAL_ENGINE.md → docs/architecture/historical/`
- `git mv docs/architecture/GODOT_INTEGRATION.md → docs/architecture/historical/`

**REGISTER.yaml updates**:
- DOC-A-VISUAL_ENGINE: `path` field update; `lifecycle: LOCKED` → `lifecycle: SUPERSEDED` (or per FRAMEWORK vocabulary)
- DOC-A-GODOT_INTEGRATION: same
- Both entries: `special_case_rationale` append: «Superseded by VULKAN_SUBSTRATE.md v1.0 LOCKED per Q-G-1 LOCK. Pre-V-substrate authority preserved at docs/architecture/historical/ for historical record.»

**Frontmatter banners** at top of each moved file (same shape as Commit 1, cites Q-G-1 LOCK).

**Cross-references to update** (search across all current Tier 1 docs + READMEs):
- Any `docs/architecture/VISUAL_ENGINE.md` reference → `docs/architecture/historical/VISUAL_ENGINE.md` OR replace with `docs/architecture/VULKAN_SUBSTRATE.md` based on context (verbatim per source)
- Same for GODOT_INTEGRATION.md

**Validation**: `sync_register.ps1 --validate` exit 0.

**Commit message**: `docs(architecture): move VISUAL_ENGINE + GODOT_INTEGRATION to historical/ per CLEANUP_CASCADE §1.3 (Q-G-1 LOCK supersession)`

### Commit 3 — README.md DRIFT-002 + DRIFT-003 rewrite (S5 cluster head)

**File**: `README.md`

**Drift surface**:
- DRIFT-002 (S4): Native core framed as experimental rather than production
- DRIFT-003 (S5): Runtime isolation guard advertised when deleted in code

**Rewrites required**:
1. Lines 68-70 (per audit evidence) — runtime isolation guard wording. Replace with verbatim transcription of SystemExecutionContext.cs lines 28-33 doc-comment language: «Isolation is enforced at compile time by the `[SystemAccess]` attribute (which `DependencyGraph` consumes for edge-building) and by the future A'.9 Roslyn analyzer; the runtime guard methods that previously threw `IsolationViolationException` are deleted.»
2. Lines 74-78 (per audit evidence) — native core wording. Replace «experimental replaceable boundary» with NativeWorld production framing per `GameBootstrap.cs` anchor: «NativeWorld is the sole production component-storage backend after A'.5 K8.3+K8.4. Managed World retired to test fixture.»

**Per Lesson #7**: transcribe verbatim from code XML doc comments and KERNEL_ARCHITECTURE.md §K-L11. Do not synthesize new wording where canonical wording exists.

**Validation**: No code changes, no test impact. README must parse as Markdown.

**Commit message**: `docs(readme): rewrite NativeWorld production + isolation safety per DRIFT-002 + DRIFT-003 (S5 + S4)`

### Commit 4 — ISOLATION.md DRIFT-005 runtime guard removal

**File**: `docs/architecture/ISOLATION.md`

**Drift surface** (per audit DRIFT-005 evidence):
- Line 15: «every component access passes through `SystemExecutionContext` and throws immediately on undeclared access» — false per current code
- Lines 106-122: DEBUG vs RELEASE runtime guard modes — entire section describes deleted mechanism

**Rewrite scope**:
- Replace runtime-guard-enforcement narrative with compile-time `[SystemAccess]` + DependencyGraph + future Roslyn analyzer narrative
- DEBUG/RELEASE distinction section — delete entirely; current model is single mode (compile-time + future analyzer)
- Preserve sections describing what isolation MEANS (system access boundaries, bus discipline, mod isolation via ALC) — those architectural concepts are intact, only enforcement mechanism changed

**Per Lesson #7**: SystemExecutionContext.cs XML doc-comment is canonical statement. Transcribe verbatim where applicable.

**Version bump**: ISOLATION.md frontmatter `version` field — increment per A'.5 pattern (1.0 → 1.1).

**Commit message**: `docs(architecture): rewrite ISOLATION.md runtime guard sections per DRIFT-005 (S5 — compile-time + analyzer enforcement)`

### Commit 5 — THREADING.md DRIFT-005 runtime guard removal

**File**: `docs/architecture/THREADING.md`

**Drift surface** (per audit DRIFT-005 evidence):
- Line 96: «isolation guard access check is O(1) via `HashSet`» — describes deleted mechanism
- Line 128: «DEBUG catches async/await through stack analysis» — same

**Rewrite scope**:
- Both runtime-guard references — delete or rewrite per compile-time model
- THREADING architectural content (thread pool design, scheduler semantics) — preserve untouched; only enforcement-mechanism wording affected

**Version bump**: 1.0 → 1.1.

**Commit message**: `docs(architecture): rewrite THREADING.md runtime guard references per DRIFT-005 (S5)`

### Commit 6 — PERFORMANCE.md DRIFT-005 runtime guard removal

**File**: `docs/architecture/PERFORMANCE.md`

**Drift surface** (per audit DRIFT-005 evidence):
- Lines 81-83: «SystemExecutionContext.GetComponent DEBUG check overhead» — describes deleted mechanism

**Rewrite scope**:
- Delete DEBUG check overhead section (the check itself is deleted)
- Performance section may reference `[SystemAccess]` compile-time cost (zero — it's metadata) if useful; otherwise simply remove the stale section

**Version bump**: 1.0 → 1.1.

**Commit message**: `docs(architecture): remove deleted-mechanism perf section per DRIFT-005 (S5)`

### Commit 7 — ECS.md DRIFT-004 NativeWorld rewrite

**File**: `docs/architecture/ECS.md`

**Drift surface** (per audit DRIFT-004 evidence):
- Line 21: «`World` is the registry for all entities and is created in `GameLoop`» — describes deleted managed-World
- Lines 111-117: documents `SystemBase.GetComponent`, `SetComponent`, `Query`, `GetSystem` — managed-World API deleted per `SystemBase.cs:9-14`

**Rewrite scope**:
- Storage section: rewrite to describe NativeWorld as production storage backend; ManagedTestWorld as test fixture only
- SystemBase API section: rewrite to describe current API (NativeWorld accessor, AcquireSpan, BeginBatch, ManagedStore for Path β)
- Per Lesson #7: transcribe verbatim from SystemBase.cs lines 9-14 and 73-90

**Preserve historical context where ECS architectural concepts unchanged** (entity-component model, system execution model, query patterns) — only storage backend wording affected.

**Version bump**: 1.0 → 1.1 (substantive content change).

**Commit message**: `docs(architecture): rewrite ECS.md NativeWorld production storage per DRIFT-004 (S4)`

### Commit 8 — CONTRACTS.md + EVENT_BUS.md DRIFT-006 IPowerBus removal

**Files**:
- `docs/architecture/CONTRACTS.md`
- `docs/architecture/EVENT_BUS.md`

**Drift surface** (per audit DRIFT-006 evidence):
- CONTRACTS.md lines 38-70: documents 6 domain buses including `IPowerBus Power` — Power is deleted per IGameServices.cs
- EVENT_BUS.md lines 24-32: repeats `IPowerBus Power` property — same

**Rewrite scope**:
- Both files: replace 6-bus list with 5-bus list (Combat, Inventory, Magic, Pawns, World) per IGameServices.cs verbatim
- Power bus + power events references — delete outright per §1.4 LOCK (no historical subsection; A'.5 closure narrative carries history)
- Cross-references to power events elsewhere in docs — update via Commit 11 (MIGRATION_PROGRESS state sync)

**Per Lesson #7**: IGameServices.cs is canonical statement. Transcribe verbatim.

**Version bumps**: CONTRACTS 1.0 → 1.1, EVENT_BUS 1.0 → 1.1.

**Commit message**: `docs(architecture): remove deleted IPowerBus from CONTRACTS + EVENT_BUS per DRIFT-006 (S4 — A'.5 deletion)`

### Commit 9 — ROADMAP.md DRIFT-007 + MIGRATION_PLAN DRIFT-008 power reconciliation

**Files**:
- `docs/ROADMAP.md`
- `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`

**Drift surface** (per audit DRIFT-007 + DRIFT-008 evidence):
- ROADMAP.md line 523: «M10.B migrates ElectricGridSystem and ConverterSystem, and kernel keeps bus contracts and deferred event types» — false (systems deleted, no kernel-keeps claim possible)
- MIGRATION_PLAN line 196: correctly says power subsystem deleted
- MIGRATION_PLAN line 211: still lists Power system slice (Converter/ElectricGrid/EtherGrid) — internal conflict with line 196
- MIGRATION_PLAN line 461: maps Systems/Power and power components to Vanilla.Inventory — stale
- MIGRATION_PLAN line 698: «Phase B includes M10 (Vanilla.Inventory + Power)» — power deleted

**Rewrite scope**:
- ROADMAP M10/M10.B: remove power migration; if electricity remains in future scope, route to V substrate field/compute work per VULKAN_SUBSTRATE.md
- MIGRATION_PLAN: reconcile internal conflict — make line 196 (correct, A'.5 closure note) authoritative; delete/rewrite stale sections lines 211, 461, 698 to match
- Per §1.4 LOCK: no historical subsections retained in current MIGRATION_PLAN; A'.5 closure narrative carries history

**Per Lesson #14 candidate refinement**: when active doc contains both correct closure note and stale legacy sections, the closure note is authoritative and stale sections should be rewritten/deleted, not preserved as «versioned history» within active doc.

**Version bumps**: MIGRATION_PLAN 1.1 → 1.2 (substantive amendment).

**Commit message**: `docs(plan): reconcile power deletion across ROADMAP + MIGRATION_PLAN per DRIFT-007 + DRIFT-008 (S4)`

### Commit 10 — ARCHITECTURE.md DRIFT-009 Silk.NET supersession

**File**: `docs/architecture/ARCHITECTURE.md`

**Drift surface** (per audit DRIFT-009 evidence):
- Line 65: «`DualFrontier.Presentation.Native` the Silk.NET production runtime» — superseded
- Line 90: «`Presentation.Native` depends on Silk.NET» — superseded

**Rewrite scope**:
- Replace Silk.NET production runtime references with VULKAN_SUBSTRATE.md authority per Q-G-1 LOCK
- Cross-references to VISUAL_ENGINE.md or GODOT_INTEGRATION.md — update to point to historical/ paths (now under docs/architecture/historical/) OR replace with VULKAN_SUBSTRATE.md as appropriate

**Note**: VISUAL_ENGINE.md and GODOT_INTEGRATION.md were already moved to historical/ in Commit 2. ARCHITECTURE.md should not point to active versions that no longer exist at original paths.

**Version bump**: ARCHITECTURE 0.3 → 0.4.

**Commit message**: `docs(architecture): supersede Silk.NET production references with VULKAN_SUBSTRATE per DRIFT-009 (S4)`

### Commit 11 — MOD_OS_ARCHITECTURE.md DRIFT-011 v3 strict

**File**: `docs/architecture/MOD_OS_ARCHITECTURE.md`

**Drift surface** (per audit DRIFT-011 evidence):
- Line 49: «v3 is additive and v2 mods continue unchanged» — false per `IModApi.cs:16-27` (v3 deleted v2 entirely)
- Line 563: «v2 mods continue to compile and load against v3» — false
- Line 56: partially reflects newer `RegisterManagedComponent<T>` surface — mixed authority within one doc

**Rewrite scope**:
- All v2-compatibility text — remove per current code reality (strict v3-only parser, manifest "3" verbatim)
- Lines describing RegisterManagedComponent + Fields + ComputePipelines — verify against `IModApi.cs` verbatim; rewrite per code
- Mod ALC isolation + capability annotations — preserve (those are correct and architecturally invariant)

**Per Lesson #7**: IModApi.cs is canonical statement. Transcribe verbatim.

**Version bump**: MOD_OS 1.7 → 1.8.

**Commit message**: `docs(architecture): rewrite MOD_OS v3 strict (no v2 compat) per DRIFT-011 (S4)`

### Commit 12 — MODDING.md + MOD_PIPELINE.md DRIFT-012 v3 rewrite

**Files**:
- `docs/architecture/MODDING.md`
- `docs/architecture/MOD_PIPELINE.md`

**Drift surface** (per audit DRIFT-012 evidence):
- MODDING.md lines 38-58: shows old `RegisterComponent<T>() where T : IComponent`, `Unsubscribe<T>`, split LogWarning/LogError methods — none present in current IModApi
- MODDING.md lines 76-78: «obtaining a system reference crashes via isolation guard» — guard deleted (DRIFT-005 cluster)
- MOD_PIPELINE.md lines 116-134: shows old `RestrictedModApi` with `RegisterComponent<T>() where T : IComponent` — stale
- MOD_PIPELINE.md lines 187-200: outdated unload chain — doesn't match current §9.5 chain in MOD_OS_ARCHITECTURE.md

**Rewrite scope**:
- Both files: full rewrite against Mod API v3 + strict manifest v3 + current unload semantics
- Per Lesson #7: transcribe verbatim from IModApi.cs + ModIntegrationPipeline.cs:57-60 (M7.2/M7.3 unload discipline)
- Example mod code blocks — update to v3 surface (RegisterComponent unmanaged-only, RegisterManagedComponent for class types, Fields, ComputePipelines)

**Note**: Isolation-guard-crash wording in MODDING.md line 76-78 — apply DRIFT-005 fix in same commit (subsection-level legacy per §1.4 LOCK; no historical subsection retained).

**Version bumps**: MODDING 1.0 → 1.1, MOD_PIPELINE 0.2 → 0.3.

**Commit message**: `docs(modding): rewrite MODDING + MOD_PIPELINE v3 strict + current unload per DRIFT-012 (S4)`

### Commit 13 — MIGRATION_PROGRESS.md DRIFT-001 state sync

**File**: `docs/MIGRATION_PROGRESS.md`

**Drift surface** (per audit DRIFT-001 evidence):
- Last updated 2026-05-12 (pre-A'.5)
- Line 47: last completed milestone A'.4.5, next A'.5 K8.3 — both wrong (A'.5 closed 2026-05-14, namespace ratification 2026-05-16)
- Lines 99-101: K8.3, K8.4, K8.5 listed `NOT STARTED` — K8.3 and K8.4 closed in A'.5; K8.5 awaits

**Rewrite scope**:
- Update «Current state snapshot» table: Active phase, Last completed milestone, Next milestone, Tests passing (620 per A'.5 closure)
- Add A'.5 K8.3+K8.4 combined closure entry per A'.5 actual landing (commits 24e5f56→fc8ecb6, 4 commits, 620 tests green)
- Add namespace ratification cascade closure entry per PR #34 merge (commits f3b3d68→ca9483d on main post-merge, 6 commits)
- Add audit + cleanup cascade entries (this brief execution will append a new entry at closure commit)
- K8.3 / K8.4 / K8.5 rows: K8.3+K8.4 status DONE (commits + date); K8.5 remains NOT STARTED with note «pending A'.6 brief authoring; absorbed into A'.5 was K8.3+K8.4, not K8.5»

**Note**: This commit also closes CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT (audit's «Live-state closure protocol gap» candidate) by demonstrating closure protocol now produces synchronized state.

**Commit message**: `docs(progress): sync MIGRATION_PROGRESS state through cleanup cascade per DRIFT-001 (S4)`

### Commit 14 — S3 cluster: G→V namespace + ARCHITECTURE_RECON + PHASE_A_PRIME + module-local

**Files** (S3 cleanup batch — all DRIFT-013, DRIFT-015, DRIFT-016, DRIFT-017):

**DRIFT-013 G→V namespace tail** in:
- `docs/architecture/FIELDS.md` line 45: «`VULKAN_SUBSTRATE` G0-G9 sits on top of field storage» — replace G0-G9 with V0/V1/V2 per VULKAN_SUBSTRATE.md §1.1-1.2 verbatim
- `docs/architecture/KERNEL_ARCHITECTURE.md` line 609: «K9 prerequisite for G-series GPU compute» — replace G-series with V-substrate
- `docs/architecture/KERNEL_ARCHITECTURE.md` line 775: «VULKAN_SUBSTRATE.md v2.0 and G-series roadmap» — version is v1.0 LOCKED (audit cited v2.0 in error or stale), roadmap is V0/V1/V2 not G-series
- `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` line 700: «Phase C is G-series GPU compute» — replace with V substrate primitives roadmap
- `docs/architecture/MOD_OS_ARCHITECTURE.md` line 446: «v3 gates G0-G9 Vulkan compute integration» — replace G0-G9 with V0/V1/V2

**DRIFT-015** in `docs/reports/ARCHITECTURE_RECON_REPORT.md`:
- Add annotation at top of file: «**NOTE (added 2026-05-16)**: This report was authored 2026-05-15, before Q-G-1/Q-G-2 LOCKs unified RUNTIME + GPU_COMPUTE into V substrate. Lines 196-198, 366-368 cite RUNTIME_ARCHITECTURE.md and GPU_COMPUTE.md as authority — these are superseded by VULKAN_SUBSTRATE.md v1.0 LOCKED 2026-05-16 per composite namespace ratification (PR #34 merged 2026-05-16). Report preserved as pre-V-unification reconnaissance evidence. For current authority navigate to VULKAN_SUBSTRATE.md.»

**DRIFT-016** in `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`:
- Q-K-1 retroactive lock surface — partial renumbering note in §2. This was deferred during composite namespace ratification execution per Q-K-1 retroactive lock mechanism.
- For this cleanup, apply minimal reconciliation: §2 body subsections + §3 duration table — propagate A'.5 closure note intent through body (A'.6 = K8.5, A'.7 = Roslyn analyzer, A'.8 = K-closure report) per closure-note-as-architect-intent reading. If propagation conflicts with another LOCKED authority, halt and surface — do not improvise.

**DRIFT-017** module-local READMEs:
- `src/DualFrontier.Core.Interop/MODULE.md` lines 40-45: «K8.3, K8.4, K8.5, K9 pending» — update per actual closure state (K8.3+K8.4 closed A'.5, K9 closed A'.4, K8.5 still pending)
- `src/DualFrontier.Components/Building/README.md` lines 11-20: lists deleted power components and overload events — remove deleted entries
- `src/DualFrontier.Contracts/Bus/README.md` line 19: lists `IPowerBus.cs` — remove (file deleted)
- `src/DualFrontier.Contracts/Modding/README.md` lines 14-24 + 43: omit v3 Fields/ComputePipelines/RegisterManagedComponent + says «RestrictedModApi remains to be implemented» — rewrite per current code state (RestrictedModApi is implemented; v3 surface complete)

**Commit message**: `docs: S3 cleanup wave — G→V namespace + recon annotation + PHASE_A_PRIME renumber + module-local sync (DRIFT-013/015/016/017)`

### Commit 15 — REGISTER amendments + 5 CAPA closures + audit_trail EVT entry

**Files**:
- `docs/governance/REGISTER.yaml`
- (potentially) `docs/audit/DOCUMENTATION_DRIFT_AUDIT_REPORT.md` — annotate findings as ADDRESSED where this brief executes them

**REGISTER amendments**:

1. **5 CAPA entries opened+closed within this commit** (governance traceability — opening separately would suggest these are emergent; they're audit-surfaced and addressed in cleanup):
   - CAPA-2026-05-16-ISOLATION-AUTHORITY-RESTORATION (DRIFT-003 + DRIFT-005) — opened 2026-05-16; closed 2026-05-16 by Commit 3-6 execution
   - CAPA-2026-05-16-LIVE-STATE-CLOSURE-PROTOCOL-GAP (DRIFT-001 + DRIFT-010) — opened 2026-05-16; closed 2026-05-16 by Commit 13 + Commit 16
   - CAPA-2026-05-16-POWER-DELETION-PROPAGATION (DRIFT-006 + DRIFT-007 + DRIFT-008) — opened 2026-05-16; closed 2026-05-16 by Commit 8 + Commit 9 + Commit 14 module-local
   - CAPA-2026-05-16-MOD-API-V3-AUTHORITY (DRIFT-011 + DRIFT-012) — opened 2026-05-16; closed 2026-05-16 by Commit 11 + Commit 12
   - CAPA-2026-05-16-V-SUBSTRATE-SUPERSESSION (DRIFT-009 + DRIFT-013 + DRIFT-014 + DRIFT-015) — opened 2026-05-16; closed 2026-05-16 by Commit 2 + Commit 10 + Commit 14

2. **audit_trail entry**:
   - `EVT-2026-05-16-CLEANUP-CASCADE` — type: governance_event; documents_affected: 18+ Tier 1/2/3/4; governance_impact: «Cleanup cascade closing 18 of 19 audit findings (DRIFT-019 historical residue no-action). 5 CAPAs opened+closed within same governance event per «audit-surfaced + execution-addressed» pattern. Pattern established: architectural authorities → historical/ folders; subsection-level legacy → delete; superseded briefs → tools/briefs/historical/.»

3. **DOCUMENTATION_DRIFT_AUDIT_REPORT.md annotations**: append «STATUS: ADDRESSED in commit {N}» to each DRIFT-N finding addressed by this cascade. DRIFT-018 (game-as-product wording in IDEAS_RESERVOIR) and DRIFT-019 (historical residue) — annotate disposition (DRIFT-018 deferred for next docs pass per audit own categorization S2; DRIFT-019 no-action per audit).

4. **Bump register_version**: 1.3 → 1.4.

**Validation**: `sync_register.ps1 --validate` exit 0.

**Commit message**: `governance: register amendments + 5 CAPA closures + EVT-2026-05-16-CLEANUP-CASCADE per CLEANUP_CASCADE §3.15`

### Commit 16 — REGISTER_RENDER regenerate + VALIDATION_REPORT regenerate + final validation gate

**Files**:
- `docs/governance/REGISTER_RENDER.md` (regenerated by `tools/governance/render_register.ps1`)
- `docs/governance/VALIDATION_REPORT.md` (regenerated by `sync_register.ps1`)

**Actions**:
1. Run `tools/governance/render_register.ps1` — produces fresh REGISTER_RENDER.md from REGISTER.yaml
2. Run `tools/governance/sync_register.ps1 --validate` — must exit 0; produces fresh VALIDATION_REPORT.md
3. Verify both regenerated artifacts reflect current state:
   - REGISTER_RENDER shows DOC-A-VULKAN_SUBSTRATE present, DOC-A-RUNTIME/DOC-A-GPU_COMPUTE absent
   - REGISTER_RENDER shows G-series at historical/ paths
   - REGISTER_RENDER shows 5 new CLOSED CAPAs
   - Total documents count reflects current state (was 231 in stale render; new count after cleanup additions/transitions)

**This commit closes DRIFT-010** (rendered register stale issue surfaced by codex P2 review + audit).

**Validation**: `sync_register.ps1 --validate` exit 0 — final gate before closing cleanup execution.

**Commit message**: `governance: regenerate REGISTER_RENDER + VALIDATION_REPORT per CLEANUP_CASCADE §3.16 (closes DRIFT-010)`

---

## §4 — Halt triggers

If execution agent encounters any of these conditions, **halt and surface to Crystalka**:

### SC-1 — Code anchor doesn't match audit evidence

If a code anchor (SystemExecutionContext.cs, IGameServices.cs, etc.) doesn't match audit's quoted line numbers + content, halt. Audit may have referenced different commit than current HEAD; resolution needed before proceeding.

### SC-2 — REGISTER lifecycle vocabulary gap

If FRAMEWORK.md doesn't permit `SUPERSEDED` lifecycle for Tier 1 LOCKED documents (VISUAL_ENGINE, GODOT_INTEGRATION), halt and ask Crystalka. Options: introduce new lifecycle, use closest permitted (e.g. retain LOCKED + path under historical/ is sufficient signal), or other disposition.

### SC-3 — Cross-reference cascade explosion

If updating cross-references for VISUAL_ENGINE or GODOT_INTEGRATION moves surfaces 50+ affected files, halt and surface scope. May need to be its own commit / micro-cascade rather than folded into Commit 2.

### SC-4 — A'-cycle renumbering propagation conflict (DRIFT-016)

If propagating A'.5 closure note intent through PHASE_A_PRIME §2 body conflicts with another LOCKED authority (MIGRATION_PLAN body, KERNEL Part 2, etc.), halt. Q-K-1 retroactive lock surface — this is exactly the architectural decision deferred. Crystalka lock needed before proceeding with that file's DRIFT-016 fix; rest of S3 cluster (DRIFT-013, DRIFT-015, DRIFT-017) can proceed.

### SC-5 — Test suite regression

If `dotnet test` shows any failures after any commit, halt immediately. No commit in this brief should affect code; failure means side-effect somewhere unanticipated.

### SC-6 — Validation regression

If `sync_register.ps1 --validate` exits non-zero after any commit, halt immediately. Cleanup itself must not introduce new validation errors.

### SC-7 — Scope creep

If execution encounters drift not in audit's 19 findings, halt and surface. Do not «fix while we're here» — that breaks scope discipline (Lesson #14 candidate). New drift goes to next audit cycle.

---

## §5 — Closure protocol (post-cleanup)

After Commit 16 lands clean:

1. **Verify final state**:
   - `git log --oneline` shows 16 commits added by this brief on cleanup branch
   - `git status` clean working tree
   - `sync_register.ps1 --validate` exit 0
   - `dotnet build` clean, `dotnet test` 620+ green
2. **Update brief status**: Set `status: EXECUTED` in this brief's frontmatter; add closure section with commit range + date.
3. **PR opening**: Push branch `claude/cleanup-cascade`; open PR titled «Cleanup cascade — 18 of 19 audit findings addressed (DRIFT-001..018, except DRIFT-019 no-action)»; body summarizes per-commit per-DRIFT mapping + verification metrics.
4. **Surface to Crystalka**: PR ready for review. Q-K-1 retroactive lock surface (DRIFT-016) — if SC-4 halt fired, surface as separate deliberation; otherwise note as «applied minimal reconciliation per closure-note intent» in PR body.

**DO NOT auto-push to main**. Crystalka reviews + merges per established protocol.

---

## §6 — Lesson surfacing during execution

Cleanup execution may surface new lesson candidates worth recording. Track them in scratchpad at `tools/scratch/CLEANUP_LESSONS.md` for K-closure report (A'.8) to formalize alongside Lessons #9-#14 deferred to post-cleanup timing.

Expected lesson categories:
- Audit + execution pairing pattern (auditor surfaces, executor closes within same governance event)
- Historical folder disposition pattern (architectural authorities move; subsection legacy deletes; closure narratives carry history)
- Pre-existing drift cleanup as separate cascade (Lesson #14 candidate refinement with empirical evidence)
- CAPA open+close within same commit pattern (audit-surfaced + execution-addressed events)

Do NOT formalize during cleanup. Post-cleanup → A'.8 K-closure report → METHODOLOGY revision.

---

## §7 — Brief authority + lifecycle

**Brief authority**: Deliberation session 2026-05-16 (Crystalka + Claude Opus 4.7). Four cleanup locks ratified per §1.

**Brief lifecycle**: AUTHORED at this commit → EXECUTED post-commit-16 closure → registered in tools/briefs/ as Tier 3 Category D (typical brief shape) per A'.4.5 governance.

**Brief enrollment**: Added to REGISTER.yaml in Commit 1 atomic with first scope edits, OR as separate pre-Commit-1 enrollment commit if Phase 0 verification reveals need. Auto-mode executor's call — match existing brief enrollment pattern in REGISTER.

**Brief location**: This file at `tools/briefs/CLEANUP_CASCADE_BRIEF.md` after Crystalka copies from `/mnt/user-data/outputs/` per Filesystem MCP workaround pattern.

---

**End of brief. 16 atomic commits across 18 audit findings + 5 CAPAs + 1 EVT + register render/validation regeneration. Expected 4-7 hours auto-mode execution.**

---

## §8 — Closure (added at brief EXECUTED transition 2026-05-16)

Execution closed 2026-05-16 by Claude Code auto-mode on branch `claude/cleanup-cascade` from `main` head `53697da`. Final commit `7bd7b4e`.

### Commit ledger (commits e68d799..7bd7b4e)

| # | Hash | Commit summary | DRIFTs |
|---|---|---|---|
| 1 | `e68d799` | G-skeleton + K-series brief moves to historical/ + REGISTER + brief enrollment | DRIFT-014 |
| 2 | `253c7ab` | VISUAL_ENGINE + GODOT_INTEGRATION moved to historical/ | DRIFT-009 (visual portion) |
| 3 | `438de49` | README NativeWorld production + isolation safety rewrite | DRIFT-002 + DRIFT-003 |
| 4 | `f5e27f3` | ISOLATION runtime guard removal | DRIFT-005 (ISOLATION) |
| 5 | `6cf1d77` | THREADING runtime guard refs cleanup | DRIFT-005 (THREADING) |
| 6 | `613e5ef` | PERFORMANCE deleted-mechanism section removal | DRIFT-005 (PERFORMANCE) |
| 7 | `11bf1c1` | ECS NativeWorld production storage rewrite | DRIFT-004 |
| 8 | `1a88ece` | CONTRACTS + EVENT_BUS IPowerBus removal | DRIFT-006 |
| 9 | `73c7bbf` | ROADMAP + MIGRATION_PLAN power reconciliation | DRIFT-007 + DRIFT-008 |
| 10 | `3aa3585` | ARCHITECTURE Silk.NET supersession | DRIFT-009 (ARCHITECTURE portion) |
| 11 | `c926382` | MOD_OS v3 strict (no v2 compat) | DRIFT-011 |
| 12 | `f7fe134` | MODDING + MOD_PIPELINE v3 + §9.5 unload | DRIFT-012 (+ DRIFT-005 MODDING portion) |
| 13 | `fa88f12` | MIGRATION_PROGRESS state sync | DRIFT-001 |
| 14 | `08e4fde` | S3 cleanup wave (G→V tail + RECON annotation + module-local) | DRIFT-013 + DRIFT-015 + DRIFT-017 (DRIFT-016 HALTED per SC-4) |
| 15 | `2371b41` | REGISTER amendments + 5 CAPA closures + EVT-2026-05-16-CLEANUP-CASCADE | DRIFT annotations on audit report |
| 16 | `7bd7b4e` | REGISTER_RENDER + VALIDATION_REPORT regeneration | DRIFT-010 |

### Verification metrics (final state)

- `git status`: clean working tree on branch `claude/cleanup-cascade`.
- `sync_register.ps1 --validate`: exit 0, 240 documents, 13 REQ, 14 RISK, 11 CAPA, 12 audit_trail, 2 advisory orphan warnings (pre-existing scratch files, out of scope).
- `dotnet build`: 0 warnings, 0 errors.
- `dotnet test`: 620 passed, 0 failed (matches A'.5 closure baseline).
- `register_version`: 1.2 → 1.4 (bumped 1.3 in Commit 1, 1.4 in Commit 15).
- `last_modified_by`: Codex → Claude Code.

### Halt protocol activations

- **DRIFT-016 HALTED per SC-4** (A'-cycle renumbering propagation conflict): Q-K-1 reconciliation note in `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` lines 55-63 explicitly defers A'-cycle renumbering propagation. The §2 body subsections (A'.6 / A'.7 / A'.8 / A'.9) and §3 duration table retain pre-renumbering structure intentionally. Surface to Crystalka for K8.5-brief-time decision on whether to propagate «A'.6 = K8.5, A'.7 = Roslyn analyzer» or to retain the dual-pointer pre-renumbering structure.

### Out-of-scope items deferred

- **DRIFT-018 (S2)**: IDEAS_RESERVOIR.md game-as-product framing — deferred to next docs pass per audit own categorization.
- **DRIFT-019 (S1)**: Historical residue in closure/audit reports — no-action per audit recommendation when register lifecycle is obeyed.
- **K8_2_CLASS_COMPONENT_REDESIGN_BRIEF.md** disposition: kept in tools/briefs/ (not moved to historical/) per ambiguous §1.2 case — registered EXECUTED, body says SKELETON. Defers to next audit cycle.
- **METHODOLOGY revision**: post-cleanup A'.8 K-closure report timing per cleanup lock.

### Pattern established (formalized in K-closure report at A'.8)

1. **Architectural authorities (Tier 1 docs) → `*/historical/` folder.** Both VISUAL_ENGINE and GODOT_INTEGRATION moved to `docs/architecture/historical/` with HISTORICAL banner + REGISTER lifecycle LOCKED → SUPERSEDED + bidirectional `superseded_by: DOC-A-VULKAN_SUBSTRATE`.
2. **Subsection-level legacy content within current docs → deleted outright.** Runtime isolation guard sections, IPowerBus + 4 power events, power slice rows in tables — all deleted; closure narratives (HALT_REPORT.md, K8_34_COMBINED_V2 brief EXECUTED, MIGRATION_PROGRESS A'.5 entry) carry history.
3. **Superseded briefs → `tools/briefs/historical/`.** G0-G9 + K8_3 + K8_4 + K8_3_BRIEF_REFRESH_PATCH + K4_STRUCT_REFACTOR + K8_2_V1_DEPRECATED.
4. **CAPA opened + closed within same governance event.** «Audit-surfaced + execution-addressed» pattern: audit opens the finding, cleanup brief execution closes within the same governance event. 5 CAPA entries demonstrating the pattern at this cleanup cascade closure.

### Lesson candidates (deferred to A'.8 K-closure report)

Tracked for METHODOLOGY revision per cleanup brief §6:
- Audit + execution pairing pattern (auditor surfaces, executor closes within same governance event).
- Historical folder disposition pattern (architectural authorities move; subsection legacy deletes; closure narratives carry history).
- Pre-existing drift cleanup as separate cascade (Lesson #14 candidate refinement with empirical evidence).
- CAPA open+close within same commit pattern (audit-surfaced + execution-addressed events).
