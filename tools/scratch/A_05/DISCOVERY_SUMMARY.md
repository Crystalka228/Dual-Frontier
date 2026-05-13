---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-A_PRIME_0_5_DISCOVERY_SUMMARY
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-A_PRIME_0_5_DISCOVERY_SUMMARY
---
# A'.0.5 Phase 1 — discovery summary report

**Discovery completed**: 2026-05-10
**HEAD**: 45d831c
**Test baseline**: 631 (per `MIGRATION_PROGRESS.md` post-K8.2v2; not re-verified yet — Phase 9 task)
**Build**: passing (Phase 0 verified)

---

## §1 Working-tree state mismatch (Phase 0 §2.1 deviation)

8 file moves pre-staged by Crystalka without `git mv`; brief itself untracked. Resolution: Option A (treat as Phase 3 work; commit via `git add -A` with rename detection on first move-commit) recommended at Stop #1.

---

## §2 Inventory totals

- ~135 `.md` files across the repository
- 8 categories per brief §3.2; counts in `INVENTORY.md`
- 8 docs already pre-staged in `docs/architecture/` (Crystalka's partial Phase 3); 9 docs total there (incl. PHASE_A_PRIME_SEQUENCING from K-L3.1)

---

## §3 Pre-flight grep findings (delta from brief baseline)

- **Gemma**: 55 hits across 10 files (incl. 19 in brief itself); 23 retain post-A'.0.5 (substantive A'.0.7 deferral); 13 mechanical scrubs in Phase 7
- **LM Studio**: 13 hits; 7 retained for A'.0.7; 1 mechanical
- **Cline**: 23 hits; 12 retained for A'.0.7; remainder either historical (TD1_SONNET_BRIEF) or mechanical
- **4-agent / four agent**: 31 hits; ~5 retained for A'.0.7 (METHODOLOGY); ~10 historical (audit reports, brief MOD_OS_V16_AMENDMENT_CLOSURE, TD1_SONNET_BRIEF); ~6-8 mechanical scrub
- **Deleted stubs in Category F**: ~10 module-local READMEs need cleanup; 5 in §7.1 explicit scope plus Systems/Combat, Systems/World, Systems/Pawn, Systems/Magic, Systems/Faction, Systems/README, Events/Combat, Events/Pawn, Core/Math, Contracts/Attributes
- **K-L3 wording staleness**: 68 hits unchanged in A'.0.5 (A'.1 scope per brief §1.4)
- **Path α/β**: 77 hits across 11 files; concentration in K_L3_1_AMENDMENT_PLAN.md (51) + KERNEL_ARCHITECTURE.md (10) — appropriate. Module-local references TBD per Phase 6 read-pass

---

## §4 Reorganization preview (per brief §4.1; details in REORG_PLAN.md)

### Confirmed by Crystalka pre-staging (8 files, already moved physically)
- ARCHITECTURE.md, ARCHITECTURE_TYPE_SYSTEM.md, KERNEL_ARCHITECTURE.md, MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md, MOD_OS_ARCHITECTURE.md, MOD_PIPELINE.md, RUNTIME_ARCHITECTURE.md, VISUAL_ENGINE.md → `docs/architecture/` (DONE physically; needs commit)

### Recommended additional Category A moves (not pre-staged but per brief §4.1 plan)
- `docs/GPU_COMPUTE.md` → `docs/architecture/GPU_COMPUTE.md` (LOCKED v2.0)
- `docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` → `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (LOCKED migration spec)
- `docs/K_L3_1_AMENDMENT_PLAN.md` → `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (architecture amendment plan, K-L3.1 deliverable)

### Ambiguous Category A subsystem-architecture docs (Stop #1 question)
The following docs are at `docs/` root and have architectural character. Crystalka did not pre-stage them. Disposition options for each:
- (a) move to `docs/architecture/` (consistent with «всё что связано с архитектурой» principle)
- (b) keep at `docs/` root (the eight pre-staged moves are a deliberate subset; subsystem specs stay flat)

Files: CONTRACTS.md, ECS.md, EVENT_BUS.md, FIELDS.md, GODOT_INTEGRATION.md, ISOLATION.md, MODDING.md, OWNERSHIP_TRANSITION.md, RESOURCE_MODELS.md, THREADING.md, FHE_INTEGRATION_CONTRACT.md, COMBO_RESOLUTION.md, COMPOSITE_REQUESTS.md, PERFORMANCE.md (perf architecture).

Recommendation: **(a) move them** — Crystalka's principle is broad («всё что связано с архитектурой»); the 8 pre-staged moves were the obvious top-level «ARCHITECTURE»-named docs, but subsystem specs are architectural per their content. Move on Crystalka confirmation at Stop #1.

### Methodology (Category B) destination
Options:
- (a) `docs/methodology/` (new subdirectory)
- (b) `docs/` flat (status quo)
- (c) `docs/architecture/methodology/` (under architecture umbrella)

Recommendation: **(a) docs/methodology/**. Categories A and B are distinct in nature; nesting B under A would conflate. Files: METHODOLOGY.md, PIPELINE_METRICS.md, MAXIMUM_ENGINEERING_REFACTOR.md, CODING_STANDARDS.md, DEVELOPMENT_HYGIENE.md, TESTING_STRATEGY.md.

### Live tracker (Category C) destination
Options:
- (a) `docs/migration/` for migration-related; `docs/` flat for ROADMAP
- (b) `docs/` flat (status quo)
- (c) `docs/tracking/`

Recommendation: **(b) keep at docs/ flat**. Live trackers are mutable; not LOCKED. Mixing into docs/architecture/ violates the «architecture = LOCKED specifications» implicit principle. Keep MIGRATION_PROGRESS.md, ROADMAP.md, IDEAS_RESERVOIR.md at docs/ root.

### Briefs (Category D) destination
Options:
- (a) keep at `tools/briefs/`
- (b) move to `docs/briefs/`

Recommendation: **(a) keep at tools/briefs/**. They are operational tooling, not authoritative architecture or live state. The `tools/` location signals operational character.

### Discovery / closure / audit (Category E) destination
Options:
- (a) keep `docs/audit/`, `docs/prompts/`, `docs/learning/`, `docs/benchmarks/` as-is; move bare `docs/PERFORMANCE_REPORT_*.md`, `docs/CPP_KERNEL_BRANCH_REPORT.md`, `docs/NATIVE_CORE_EXPERIMENT.md`, `docs/NORMALIZATION_REPORT.md` to a sub-folder
- (b) keep all at current locations

Recommendation: **(a) consolidate**: create `docs/reports/` for `PERFORMANCE_REPORT_K{3,7}.md`, `CPP_KERNEL_BRANCH_REPORT.md`, `NATIVE_CORE_EXPERIMENT.md`, `NORMALIZATION_REPORT.md`. Audit/prompts/learning/benchmarks already have their subdirectories. (Or option (b) status quo if Crystalka prefers minimum churn.)

### Project meta (Category G), Module-local (Category F)
- Stay at current locations. No moves.

### i18n (Category H)
- TRANSLATION_GLOSSARY.md, TRANSLATION_PLAN.md → keep at docs/ root, OR move to `docs/i18n/` subfolder if other i18n artifacts accumulate.

Recommendation: **keep at docs/ root** (only 2 files; subfolder overhead not justified).

---

## §5 Phase A' adjustments preview (Phase 9 closure)

After A'.0.5 closes, `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` updated:
- A'.0.5 status: DONE
- A'.2 README cleanup folded into A'.0.5 Phase 5 (already noted in brief §1.2)
- A'.0.7 methodology pipeline restructure inserted between A'.0.5 and A'.1 (already noted in brief §16)

---

## §6 Stop #1 questions for Crystalka

1. **Confirm working-tree resolution**: Option A (commit pre-staged moves as Phase 3 commit 1) acceptable? Or Option B (revert + redo via `git mv`)?
2. **Confirm additional Category A moves**: GPU_COMPUTE.md, MIGRATION_PLAN_KERNEL_TO_VANILLA.md, K_L3_1_AMENDMENT_PLAN.md → `docs/architecture/`? (Brief §4.1 lists them.)
3. **Subsystem architecture docs**: should CONTRACTS.md, ECS.md, EVENT_BUS.md, FIELDS.md, GODOT_INTEGRATION.md, ISOLATION.md, MODDING.md, OWNERSHIP_TRANSITION.md, RESOURCE_MODELS.md, THREADING.md, FHE_INTEGRATION_CONTRACT.md, COMBO_RESOLUTION.md, COMPOSITE_REQUESTS.md, PERFORMANCE.md also move to `docs/architecture/`? Recommendation: yes.
4. **Methodology destination**: `docs/methodology/` (recommended) or stay at docs/ flat?
5. **Live trackers (MIGRATION_PROGRESS, ROADMAP, IDEAS_RESERVOIR)**: stay at docs/ flat? (Recommended.)
6. **Briefs (tools/briefs/*)**: stay at tools/briefs/? (Recommended.)
7. **Reports consolidation**: create `docs/reports/` for PERFORMANCE_REPORT_*, CPP_KERNEL_BRANCH_REPORT, NATIVE_CORE_EXPERIMENT, NORMALIZATION_REPORT? Or keep at docs/ flat?
8. **Cross-reference convention**: repo-rooted absolute paths (`/docs/architecture/X.md`) or explicit relative traversal (`../../architecture/X.md`)? Recommendation: repo-rooted absolute for unambiguity (works in GitHub markdown rendering).
9. **Pipeline-terminology scrub depth**: confirm METHODOLOGY.md and PIPELINE_METRICS.md substantive content untouched in A'.0.5 (A'.0.7-deferred) — only top-of-file `<!-- TODO: A'.0.7 -->` markers added in Phase 7.
