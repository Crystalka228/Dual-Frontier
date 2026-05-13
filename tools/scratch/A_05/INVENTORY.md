---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-A_PRIME_0_5_INVENTORY
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-A_PRIME_0_5_INVENTORY
---
# A'.0.5 Phase 1 — full repository documentation inventory

**Total `.md` files in repository**: ~135 (counted Phase 1 §3.1 scan)

---

## Top-level (G)
- `README.md` — research framing, falsifiability conditions

## docs/ flat-level (mixed A/B/C/E/H)
- docs/CODING_STANDARDS.md (B — methodology)
- docs/COMBO_RESOLUTION.md (A — subsystem architecture spec)
- docs/COMPOSITE_REQUESTS.md (A — subsystem architecture spec)
- docs/CONTRACTS.md (A — contracts assembly architecture)
- docs/CPP_KERNEL_BRANCH_REPORT.md (E — Discovery)
- docs/DEVELOPMENT_HYGIENE.md (B — methodology)
- docs/ECS.md (A — ECS architecture)
- docs/EVENT_BUS.md (A — event bus architecture)
- docs/FEEDBACK_LOOPS.md (B/H? — process notes; ambiguous)
- docs/FHE_INTEGRATION_CONTRACT.md (A — integration contract)
- docs/FIELDS.md (A — field-storage architecture)
- docs/GODOT_INTEGRATION.md (A — engine-integration architecture)
- docs/GPU_COMPUTE.md (A — LOCKED v2.0 GPU compute spec) ← brief explicitly lists for `docs/architecture/`
- docs/IDEAS_RESERVOIR.md (C — live tracker)
- docs/ISOLATION.md (A — cross-cutting isolation architecture)
- docs/K_L3_1_AMENDMENT_PLAN.md (A — architecture amendment plan; K-L3.1 deliverable)
- docs/MAXIMUM_ENGINEERING_REFACTOR.md (B — methodology / refactor doctrine)
- docs/METHODOLOGY.md (B — central methodology, target of A'.0.7 substantive rewrite)
- docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md (A — LOCKED migration spec) ← brief explicitly lists for `docs/architecture/`
- docs/MIGRATION_PROGRESS.md (C — live tracker, mutable)
- docs/MODDING.md (A — modding architecture)
- docs/NATIVE_CORE_EXPERIMENT.md (E — negative-result experimental report)
- docs/NORMALIZATION_REPORT.md (E — i18n campaign report)
- docs/OWNERSHIP_TRANSITION.md (A — architectural transition note)
- docs/PERFORMANCE.md (A/B? — performance architecture; ambiguous)
- docs/PERFORMANCE_REPORT_K3.md (E — K3 performance closure report)
- docs/PERFORMANCE_REPORT_K7.md (E — K7 performance closure report)
- docs/PIPELINE_METRICS.md (B — methodology empirics, target of A'.0.7)
- docs/README.md (G — docs index)
- docs/RESOURCE_MODELS.md (A — resource models architecture)
- docs/ROADMAP.md (C — live tracker; Phase 5 backlog)
- docs/TESTING_STRATEGY.md (B — methodology)
- docs/THREADING.md (A — threading architecture)
- docs/TRANSLATION_GLOSSARY.md (H — i18n campaign)
- docs/TRANSLATION_PLAN.md (H — i18n campaign)

## docs/architecture/ (already populated; A category — pre-staged moves)
- docs/architecture/ARCHITECTURE.md (A) ← pre-staged
- docs/architecture/ARCHITECTURE_TYPE_SYSTEM.md (A) ← pre-staged
- docs/architecture/KERNEL_ARCHITECTURE.md (A — LOCKED v1.4) ← pre-staged
- docs/architecture/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md (A?/B?) ← pre-staged; classification ambiguous
- docs/architecture/MOD_OS_ARCHITECTURE.md (A — LOCKED v1.6) ← pre-staged
- docs/architecture/MOD_PIPELINE.md (A) ← pre-staged
- docs/architecture/PHASE_A_PRIME_SEQUENCING.md (A — Phase A' anchor; from K-L3.1 closure 45d831c)
- docs/architecture/RUNTIME_ARCHITECTURE.md (A — LOCKED v1.0) ← pre-staged
- docs/architecture/VISUAL_ENGINE.md (A) ← pre-staged

## docs/audit/ (E)
- docs/audit/AUDIT_CAMPAIGN_PLAN.md
- docs/audit/AUDIT_PASS_1_INVENTORY.md
- docs/audit/AUDIT_PASS_1_PROMPT.md
- docs/audit/AUDIT_PASS_2_PROMPT.md
- docs/audit/AUDIT_PASS_2_RESUMPTION_PROMPT.md
- docs/audit/AUDIT_PASS_2_SPEC_CODE.md
- docs/audit/AUDIT_PASS_3_PROMPT.md
- docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md
- docs/audit/AUDIT_PASS_4_PROMPT.md
- docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md
- docs/audit/AUDIT_PASS_5_PROMPT.md
- docs/audit/AUDIT_PASS_5_TRIAGE.md
- docs/audit/AUDIT_REPORT.md
- docs/audit/M3_CLOSURE_REVIEW.md
- docs/audit/M4_CLOSURE_REVIEW.md
- docs/audit/M5_CLOSURE_REVIEW.md
- docs/audit/M6_CLOSURE_REVIEW.md
- docs/audit/M7_CLOSURE_REVIEW.md
- docs/audit/PASS_2_NOTES.md
- docs/audit/PASS_3_NOTES.md
- docs/audit/PASS_4_REPORT.md
- docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md
- docs/audit/UI_REVIEW_PRE_M75B2.md

## docs/benchmarks/ (E)
- docs/benchmarks/k7-bdn-tick-report.md

## docs/learning/ (E)
- docs/learning/PHASE_1.md (Russian-language learning artifact per i18n)

## docs/prompts/ (E — historical prompts, not active)
- docs/prompts/HOUSEKEEPING_*.md (8)
- docs/prompts/M73_CODING_STANDARDS_UPDATE.md
- docs/prompts/M74_BUILD_PIPELINE_OVERRIDE.md
- docs/prompts/M75A_MOD_MENU_CONTROLLER.md
- docs/prompts/M75B1_BOOTSTRAP_INTEGRATION.md
- docs/prompts/M75B2_GODOT_UI_SCENE.md
- docs/prompts/M7_CLOSURE.md
- docs/prompts/M7_HOUSEKEEPING_TICK_DISPLAY.md
- docs/prompts/TD1_SONNET_BRIEF.md (high Gemma/Cline density)

## tools/briefs/ (D — 41 briefs)
All 41 brief filenames listed in Phase 1 glob; categorized as Category D for milestone briefs. Notable:
- tools/briefs/A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md (untracked; this brief)
- tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md (EXECUTED)
- tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md (APPLIED)
- tools/briefs/K_LESSONS_BATCH_BRIEF.md (post-execution historical)
- tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md (explicit deprecation marker)
- tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md (executed)
- tools/briefs/K9_FIELD_STORAGE_BRIEF.md (NEXT)
- tools/briefs/G0..G9_*.md (10 GPU compute briefs, pending)
- tools/briefs/K0..K8_*.md (kernel briefs, mix of EXECUTED and pending)
- tools/briefs/MOD_OS_V16_AMENDMENT_BRIEF.md (executed)
- tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md (E — closure report)
- tools/briefs/K6_VERIFICATION_LOG.md (E — execution log)
- tools/briefs/K6_1_AFFECTED_TESTS.md (E — execution artifact)
- tools/briefs/MODULE.md — actually module-level brief? Need check.

## src/ module-local docs (F — 60 files)
[See full list in tools/scratch/A_05/CLASSIFIED.md §F]

## tests/ module-local docs (F — 5 files)
- tests/README.md
- tests/DualFrontier.Core.Tests/README.md
- tests/DualFrontier.Modding.Tests/README.md
- tests/DualFrontier.Systems.Tests/README.md
- tests/DualFrontier.Core.Interop.Tests/MODULE.md
- (BenchmarkDotNet artifact reports excluded — auto-generated by test runs)

## mods/ (F — 8 files)
- mods/README.md
- mods/DualFrontier.Mod.Example/README.md
- mods/DualFrontier.Mod.Vanilla.Core/README.md
- mods/DualFrontier.Mod.Vanilla.Combat/README.md
- mods/DualFrontier.Mod.Vanilla.Magic/README.md
- mods/DualFrontier.Mod.Vanilla.Inventory/README.md
- mods/DualFrontier.Mod.Vanilla.Pawn/README.md
- mods/DualFrontier.Mod.Vanilla.World/README.md

## native/ (F — 5 files)
- native/DualFrontier.Core.Native/MODULE.md
- native/DualFrontier.Core.Native/include/MODULE.md
- native/DualFrontier.Core.Native/src/MODULE.md
- native/DualFrontier.Core.Native/test/MODULE.md
- native/DualFrontier.Core.Native/build.md

## assets/ (F — 1 file)
- assets/scenes/README.md

---

## Counts by category

| Category | Approx count | Notes |
|---|---|---|
| A — Architecture spec | ~25 | LOCKED + subsystem specs; some at docs/, some pre-staged in docs/architecture/ |
| B — Methodology | ~6 | METHODOLOGY, PIPELINE_METRICS, MAXIMUM_ENGINEERING_REFACTOR, CODING_STANDARDS, DEVELOPMENT_HYGIENE, TESTING_STRATEGY |
| C — Live tracker | ~3 | MIGRATION_PROGRESS, ROADMAP, IDEAS_RESERVOIR |
| D — Brief | 41 | tools/briefs/* |
| E — Discovery / closure / audit | ~30 | docs/audit/, docs/prompts/, docs/learning/, docs/benchmarks/, plus PERFORMANCE_REPORT_K{3,7}.md, NATIVE_CORE_EXPERIMENT.md, CPP_KERNEL_BRANCH_REPORT.md, NORMALIZATION_REPORT.md |
| F — Module-local | ~70 | src/**/README.md, mods/**/README.md, tests/**/README.md, native/**/MODULE.md, assets/scenes/README.md |
| G — Project meta | 2 | README.md (root), docs/README.md |
| H — i18n | 2 | TRANSLATION_GLOSSARY.md, TRANSLATION_PLAN.md |
