# A'.0.5 Phase 0 — pre-flight baseline grep

**Date**: 2026-05-10
**HEAD**: `45d831c` (K-L3.1 closure) — ✓ verified
**Build**: `dotnet build` exit 0 — ✓ verified
**Tests**: not yet run; deferred to Phase 9 verification (baseline 631 per `MIGRATION_PROGRESS.md` last update 2026-05-09)

---

## Working-tree state mismatch (Phase 0 deviation)

Brief §2.1 expects clean working tree. Reality:

```
 D docs/ARCHITECTURE.md                        ← physically moved
 D docs/ARCHITECTURE_TYPE_SYSTEM.md            ← physically moved
 D docs/KERNEL_ARCHITECTURE.md                 ← physically moved
 D docs/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md ← physically moved
 D docs/MOD_OS_ARCHITECTURE.md                 ← physically moved
 D docs/MOD_PIPELINE.md                        ← physically moved
 D docs/RUNTIME_ARCHITECTURE.md                ← physically moved
 D docs/VISUAL_ENGINE.md                       ← physically moved
?? docs/architecture/ARCHITECTURE.md           ← new location (untracked)
?? docs/architecture/ARCHITECTURE_TYPE_SYSTEM.md
?? docs/architecture/KERNEL_ARCHITECTURE.md
?? docs/architecture/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md
?? docs/architecture/MOD_OS_ARCHITECTURE.md
?? docs/architecture/MOD_PIPELINE.md
?? docs/architecture/RUNTIME_ARCHITECTURE.md
?? docs/architecture/VISUAL_ENGINE.md
?? tools/briefs/A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md   ← brief itself
```

**Diagnosis**: Crystalka pre-staged 8 file moves (without `git mv`) and authored the brief itself before invoking this session. The pre-staged moves are consistent with Phase 3 intent (architectural docs → `docs/architecture/`) but were not executed via `git mv`, so git's rename detection will rely on similarity index at commit time.

**Resolution recommendation** (for Stop #1 confirmation):
- **Option A** (preferred): treat the 8 pre-staged moves as Phase 3 work-in-progress; `git add -A` for the move pairs at Phase 3 commit 1; rename detection will activate via content similarity (default 50% threshold; content unchanged → 100% similarity → clean rename in git history). The brief itself becomes the «brief authoring» commit per METHODOLOGY §«Brief authoring as prerequisite step».
- **Option B**: revert the 8 moves (`git checkout` removed paths, `rm` new untracked paths), then redo cleanly via `git mv` per brief §5.1. Identical end state, more steps. No git-history advantage over Option A.

Option A recommended.

---

## §2.2 baseline grep counts (incl. brief itself)

| Pattern | Files | Hits | Brief share | Net (excl. brief) |
|---|---|---|---|---|
| `Gemma` | 10 | 55 | 19 | 36 across 9 files |
| `LM Studio` | 4 | 13 | 5 | 8 across 3 files |
| `Cline` | 5 | 23 | 11 | 12 across 4 files |
| `four[- ]agent\|4-agent\|four agents` | 12 | 31 | 14 | 17 across 11 files |
| `local executor\|local quantized\|prompt generator\|quantized model` | 6 | 22 | 6 | 16 across 5 files |
| `class component\|class-based component(s)?\|class components` | 15 | 65 | 4 | 61 across 14 files (mostly briefs/amendment plan — historical, K-L3.1-pre-locked) |
| `Path α\|Path β` | 11 | 77 | 3 | 74 across 10 files (concentrated in K_L3_1_AMENDMENT_PLAN.md = 51) |
| `без exception\|K-L3 violation` | 10 | 68 | 5 | 63 across 9 files (K-L3.1 amendment scope — A'.1, NOT A'.0.5) |
| Deleted stubs (`Ammo\|Shield\|Weapon\|School\|Social\|Biome` Component/System) in `*.md` | 50 | 222 | 8 | 214 across 49 files (most in briefs as historical record; only 5+ Components READMEs and several Systems READMEs need active cleanup) |

---

## Targets

| Pattern | Phase 7 target (post-A'.0.5) | Phase 9 verification |
|---|---|---|
| `Gemma` | ≤ 23 (METHODOLOGY 2 + PIPELINE_METRICS 7 + TRANSLATION_GLOSSARY 2 + TRANSLATION_PLAN 4 + TD1_SONNET_BRIEF 14 deferred to A'.0.7); auto-fix in IDEAS_RESERVOIR/MAXIMUM_ENGINEERING_REFACTOR/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION/AUDIT_CAMPAIGN_PLAN | confirm only A'.0.7-deferred files retain Gemma |
| `LM Studio` | ≤ 7 (METHODOLOGY 3 + PIPELINE_METRICS 4 deferred to A'.0.7); TRANSLATION_GLOSSARY 1 auto-fix | confirm |
| `Cline` | ≤ 12 (METHODOLOGY 2 + PIPELINE_METRICS 6 deferred; TRANSLATION_GLOSSARY 1 + TD1_SONNET_BRIEF 3 auto-fix where mechanical) | confirm |
| `four[- ]agent` | ≤ 6 (METHODOLOGY 5 deferred); README/PIPELINE_METRICS/MOD_OS_V16/PASS_4/PASS_2/NORMALIZATION_REPORT/ROADMAP/TRANSLATION_GLOSSARY auto-fix; RUNTIME_ARCHITECTURE 1 verify | confirm |
| Deleted stubs in 5 component READMEs (§7.1 scope) | 0 | confirm |
| `K-L3 wording` | unchanged (A'.1 scope) | not in A'.0.5 verification |
