# K9 Brief Refresh Patch — Companion to K9_FIELD_STORAGE_BRIEF.md

**Status**: AUTHORED 2026-05-10 — companion patch to `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` (authored 2026-05-08).
**Scope**: Documents post-authoring deltas. Read BEFORE K9 brief. Apply mentally during K9 execution.
**Authority**: This patch overrides specific Phase 0 / Phase 8 / Status / Authoring-lineage statements in K9 brief. K9 architectural design (Phase 1–7, Phase 9) is unchanged.
**Milestone**: A'.4 (K9 + this patch bundled per Crystalka 2026-05-10 «всё в одну сессию, окно контекста позволяет»).

---

## ⚠ READ ORDER (critical — read this section first)

You are the executor of a Claude Code session opened to perform Milestone A'.4 (K9 — Field Storage Abstraction). Two briefs were attached to this session:

1. **`tools/briefs/K9_BRIEF_REFRESH_PATCH.md`** — this document
2. **`tools/briefs/K9_FIELD_STORAGE_BRIEF.md`** — the main K9 brief (authored 2026-05-08)

**Read THIS document FIRST.** Then read K9 brief. Then proceed with execution applying the overrides documented below.

Reason: K9 brief was authored 2026-05-08, before a cascade of milestone closures and a documentation reorganization. Some Phase 0 pre-flight statements in K9 brief check for prerequisites that resolved differently than the K9 brief author anticipated. If you read K9 brief first and run Phase 0 verbatim, you will halt in Phase 0.2 / 0.3 / 0.6 against pre-conditions that have moved on. This patch tells you what moved and what the correct expectation is for the 2026-05-10+ state.

---

## ⚠ COMMIT ORDERING (critical — affects branch topology)

The K9 feature branch convention from K9 brief Phase 0.8 is preserved with one addition:

```
git checkout main
git pull origin main                                  # verify HEAD = 38c2e19 or later
git checkout -b feat/k9-field-storage

# Commit #1 on feat/k9-field-storage:
git add tools/briefs/K9_BRIEF_REFRESH_PATCH.md
git commit -m "docs(briefs): A'.4.0 K9 brief refresh patch (companion to K9 brief)"

# Commit #2 on feat/k9-field-storage:
git add tools/briefs/K9_FIELD_STORAGE_BRIEF.md       # IF brief was modified during this session
                                                      # otherwise skip — brief was committed pre-session
git commit -m "docs(briefs): K9 brief on disk (no-op if already committed)"
                                                      # OR skip entirely if not modified

# Commits #2 (or #3) onwards: K9 Phase 1.1 through Phase 9.5 per K9 brief
```

**Patch brief is commit #1 on the feature branch.** This ordering is non-negotiable because:

1. If K9 brief execution begins before patch is committed, the feature branch will not contain the patch artifact when merged back to `main`. Patch must land alongside K9 in the same merge.
2. If patch lands on `main` directly (not on `feat/k9-field-storage`), it desynchronizes from K9 closure — patch becomes orphan documentation.
3. The fast-forward merge at K9 closure (Phase 9.7) must contain patch + K9 commits in correct temporal order so git history reads as «patch first, K9 after».

**Atomic commit log expected (revised from K9 brief)**: 17–19 commits total on `feat/k9-field-storage`:
1. **`docs(briefs): A'.4.0 K9 brief refresh patch (companion to K9 brief)`** — this patch
2. `docs(briefs): K9 skeleton expanded to full brief` — if K9 brief on disk is unstaged at session start; if already committed (which it should be — verify via `git log --oneline -5 tools/briefs/K9_FIELD_STORAGE_BRIEF.md`), skip this commit
3–18. K9 brief Phase 1.1 through Phase 9.4 atomic commits (16 commits per K9 brief log)
19. `docs(migration): K9 closure recorded` — Phase 9.5

---

## Overview — what changed since K9 brief authoring (2026-05-08)

K9 brief was authored on 2026-05-08. Between then and 2026-05-10 (this patch authoring), the following events occurred:

| Date | Event | K9 brief impact |
|---|---|---|
| 2026-05-09 | K6 closed (`cb3d6cf..af2b572`) | Phase 0.2 expectation now satisfiable |
| 2026-05-09 | K6.1 mod fault wiring closed (`fe03ed3..a642d65`) | Side milestone; no K9 brief impact |
| 2026-05-09 | K7 closed (`72ea8b5..e917220`) | Phase 0.2 expectation now satisfiable |
| 2026-05-09 | K8.0 closed (`9f9dc05..5fa3f1d`) — Solution A architectural decision recorded | Phase 0.2 reference to «K8» now resolves to K8.0–K8.5 series, of which K8.0–K8.2 closed |
| 2026-05-09 | K8.1 native reference primitives closed (`a62c1f3..812df98`) | Same |
| 2026-05-09 | K8.1.1 InternedString follow-up closed (`f8273ca..16afdf3`) | Same |
| 2026-05-09 | K-Lessons batch closed (`9df2709..071ae11`) — METHODOLOGY v1.5 + KERNEL v1.3 | KERNEL version reference now stale |
| 2026-05-09 | K8.2 v2 closed (`6ee1a85..7527d00`) — K-L3 selective per-component closure | Phase 0.3 baseline `538` is now `631`; K8.3–K8.5 remain NOT STARTED |
| 2026-05-10 | A'.0.5 documentation reorg (`4e332bb`) — `docs/*.md` → `docs/architecture/*.md` and `docs/methodology/*.md` | Phase 0.4 paths already corrected per surgical edit `180d66d`; Phase 9.5 paths already corrected per same. K9 brief on disk is post-A'.0.5 corrected. |
| 2026-05-10 | A'.0.7 methodology pipeline restructure (`86b721a..9d4da64`) — METHODOLOGY v1.5 → v1.6, PIPELINE_METRICS v0.1 → v0.2, MAXIMUM_ENGINEERING_REFACTOR v1.0 → v1.1, README rewrite | Methodology corpus referenced indirectly by K9 brief is now at v1.6 era; no direct K9 brief edit required (K9 brief does not cite METHODOLOGY versions in Phase 0) |
| 2026-05-10 | K-L3.1 bridge formalization (`2df5921..0789bd4`) — KERNEL v1.3 → v1.5, MOD_OS v1.6 → v1.7, MIGRATION_PLAN v1.0 → v1.1; IModApi v3 gains `RegisterManagedComponent<T>` for Path β | Phase 8.1–8.2 IModApi v3 surface description does not mention `RegisterManagedComponent<T>` (added by K-L3.1 amendment). Patch adds awareness. |
| 2026-05-10 | A'.0.5/A'.0.7/A'.1.K/A'.1.M/A'.3 — Phase A' deliberation foundation + amendment landing + push (`38c2e19`) | Repository fully synced to `origin/main`; baseline 631 preserved |
| 2026-05-10 | Surgical edit `180d66d` per K-L3.1 disposition B applied to K9 brief | Phase 0.4 paths + Phase 0.7 baseline 631+ + optional architectural reference paragraph already applied |

**Net effect on K9 brief**: surgical edit `180d66d` covered K-L3.1-relevant items (paths, versions, baseline). Non-K-L3.1 staleness (Phase 0.2 K6/K7/K8 reference, Phase 0.3 baseline `538 from K5`, Phase 0.6 «17 scenarios», Status line, MOD_OS v1.7 reference, IModApi v3 surface description) **remained out of scope of `180d66d`** and is the subject of this patch.

K9 architectural design (Phase 1 RawTileField, Phase 2 World registry extension, Phase 3 C ABI, Phase 4 selftest scenarios, Phase 5 managed bridge, Phase 6 CPU reference kernel, Phase 7 bridge tests, most of Phase 8 IModFieldApi wiring, Phase 9 closure) is **architecturally sound and remains authoritative as written**. This patch does not touch any architectural decision in K9 brief.

---

## Override Table — explicit Phase-by-Phase corrections

The table below maps every K9 brief location requiring a corrected reading at execution time. Each row: K9 brief Location → Original Statement → Corrected Statement → Rationale. Executor consults this table when reading the corresponding K9 brief Phase.

### Status line (K9 brief header)

| Field | Original (K9 brief line 3) | Corrected reading |
|---|---|---|
| Status | `AUTHORED — awaiting K6, K7, K8 closure per β6 sequencing before execution` | `AUTHORED + REFRESHED 2026-05-10 — K6/K7/K8.0/K8.1/K8.1.1/K8.2 v2 closed (per Phase A' sequencing); K8.3/K8.4/K8.5 deferred to post-K9 per Phase A' sequencing decision; ready for execution` |

**Rationale**: К-series sequencing was reframed by Phase A' deliberation: K9 (this milestone) runs **between K8.2 v2 closure and K8.3 start**, not after K8.5. The «awaiting K8 closure» wording was authored before the K8 split into K8.0–K8.5 sub-milestones (which itself was decided at K8.0 closure 2026-05-09).

### Phase 0.2 — Prerequisite milestones closed (K9 brief lines ~242–256)

| Field | Original | Corrected reading |
|---|---|---|
| Expected output | `K0–K5 closure commits visible (`89a4b24`, `e2c50b8`, `129a0a0`, `7629f57`, `2fc59d1`, `547c919`). K6, K7, K8 closure commits visible.` | `K0–K5 closure commits visible (same hashes). K6/K6.1/K7/K8.0/K8.1/K8.1.1/K-Lessons/K8.2 v2 closure commits visible (see git log range from 2026-05-09). K8.3/K8.4/K8.5 NOT STARTED is expected — they sequence after K9 per Phase A' decision.` |
| Halt condition | `K6, K7, or K8 not closed. K9 cannot run on an unfinished migration.` | `K6, K7, K8.0, K8.1, K8.1.1, K-Lessons, or K8.2 v2 not closed. K9 cannot run on an unfinished migration foundation. K8.3/K8.4/K8.5 not started is expected, NOT a halt condition — those sub-milestones explicitly sequence AFTER K9 per Phase A' sequencing decision 2026-05-10.` |

**Rationale**: K8 was split into K8.0–K8.5 sub-milestones at K8.0 closure (post-authoring). K9 brief's «K8 closed» was authored against the pre-split monolithic K8 framing. Phase A' sequencing places K9 between K8.2 v2 (foundation closed) and K8.3 (systems migration starts), not after the full K8 series.

### Phase 0.3 — MIGRATION_PROGRESS.md baseline (K9 brief lines ~258–264)

| Field | Original | Corrected reading |
|---|---|---|
| Test count baseline | `currently 538 from K5` | `currently 631 from K8.2 v2 (post-K8.2 v2 closure, +93 over K5 baseline of 538 via K6 +28, K7 +0, K-Lessons +0, K8.1 +23, K8.1.1 +3, K8.2 v2 +39)` |

**Rationale**: 8 milestones closed between K5 (538 baseline) and the current state (631). The baseline `538 from K5` was correct at K9 brief authoring (2026-05-08, before K6 cascade).

### Phase 0.6 — Selftest baseline (K9 brief lines ~290–298)

| Field | Original | Corrected reading |
|---|---|---|
| Expected output | `all 17 scenarios pass (post-K5 baseline: K0–K5 added scenarios cumulatively)` | `all post-K8.2 v2 scenarios pass — count is variable depending on what K6/K7/K8.1/K8.2 v2 added. **Executor action**: run `./Release/df_native_selftest.exe` first as inventory step; record the actual scenario count N as the baseline; Phase 4.2 expectation becomes `N + 8 scenarios pass` (not `17 + 8 = 25`). Halt condition: any scenario fails; mismatch on count is NOT a halt.` |

**Rationale**: K9 brief Phase 4.2 already signals inline «Adjust 17 to the actual post-K8 count if K6/K7/K8 added scenarios», but Phase 0.6 does not. This override aligns Phase 0.6 with Phase 4.2's self-correcting stance.

### Phase 0.7 — Managed test baseline (K9 brief lines ~300–308)

No override needed — `180d66d` already updated this to `631+ tests passing (post-K8.2 v2 baseline)`. ✓ Current.

### Phase 0.4 — Prerequisite documents at expected versions

No override needed — `180d66d` already updated paths to `docs/architecture/*.md` and versions to `KERNEL v1.5+ / MOD_OS v1.7+`. ✓ Current.

### Phase 8.1 — IModFieldApi interface (K9 brief lines ~1370–1385)

| Field | Original | Corrected reading |
|---|---|---|
| Reference target | `IModApi v3 surface per MOD_OS_ARCHITECTURE.md v1.6 §4.6` | `IModApi v3 surface per MOD_OS_ARCHITECTURE.md v1.7 §4.6 (v1.7 added RegisterManagedComponent<T> for K-L3.1 Path β; K9 wires Fields/ComputePipelines only — RegisterManagedComponent ships at K8.4 per Phase A' sequencing)` |

**Rationale**: MOD_OS bumped 1.6 → 1.7 on 2026-05-10 via K-L3.1 amendment. The IModApi v3 surface now contains three K-series additions: `RegisterManagedComponent<T>` (K-L3.1, ships K8.4), `Fields` (K9, ships this milestone), `ComputePipelines` (G0, ships G-series). K9 brief Phase 8 correctly scopes its deliverable to `Fields` + null `ComputePipelines`. The patch only adds awareness that the broader IModApi v3 surface contains `RegisterManagedComponent<T>` — **K9 does NOT implement it**.

### Phase 8.2 — Extend IModApi (K9 brief lines ~1387–1404)

| Field | Original | Corrected reading |
|---|---|---|
| Edit instruction | `Add at the end of the interface (before closing brace)` — two properties: `Fields` and `ComputePipelines` | `Add at the end of the interface (before closing brace) — two properties: `Fields` and `ComputePipelines`. **PRE-FLIGHT CHECK**: read `src/DualFrontier.Contracts/Modding/IModApi.cs` first. If `RegisterManagedComponent<T> where T : class, IComponent` is already declared in the interface (from a K-L3.1 amendment landing not yet executed but possibly applied), preserve that declaration and add K9's two properties alongside. If `RegisterManagedComponent<T>` is absent, do NOT add it — K9 scope is Fields + ComputePipelines null only; RegisterManagedComponent ships K8.4.` |

**Rationale**: K-L3.1 amendment landed v1.7 to MOD_OS spec but did NOT add `RegisterManagedComponent<T>` to `IModApi.cs` source code. The interface source ships with `RegisterManagedComponent<T>` at K8.4 closure per Phase A' sequencing. Patch makes this explicit so executor does not accidentally add `RegisterManagedComponent<T>` to interface during K9 wiring (out of scope) AND does not panic if it finds the method already present (hypothetically applied by a future K8.4 partial execution that K9 ran on top of — defensive pre-flight check).

### Phase 9.1 — Update MIGRATION_PROGRESS.md (K9 brief lines ~1657–1668)

| Field | Original | Corrected reading |
|---|---|---|
| Active phase update | `next planned (G0, or whichever is next)` | `next planned is **A'.5 K8.3** per Phase A' sequencing (K8.3 production system migration → K8.4 ManagedWorld retired + Mod API v3 → K8.5 mod ecosystem migration prep → A'.8 K-closure report → A'.9 architectural analyzer milestone → Phase B M8.4 Vanilla.World migration). G0 sequences AFTER A'.8/A'.9 closure, not directly after K9.` |
| Last completed milestone | `K9 with commit hash + date` | `**A'.4** (K9 + A'.4.0 patch bundled) with commit hash + date 2026-05-XX` (where 2026-05-XX is the execution date) |

**Rationale**: K9 brief was authored before Phase A' sequencing was formalized. G-series gates on K9 closure AND M9.0–M9.4 (Vulkan runtime), and runs after К-closure report (A'.8). Immediate post-K9 work is K8.3.

### Phase 9.2 — Update KERNEL_ARCHITECTURE.md status snapshot (K9 brief lines ~1670–1680)

| Field | Original | Corrected reading |
|---|---|---|
| Current snapshot text | `K0–K5 closed (`547c919`, 2026-05-08); 538 tests passing; K6 next per β6 sequencing.` | `K0–K8.2 v2 closed (cumulative `547c919..7527d00`, 2026-05-07 through 2026-05-09); K-L3.1 bridge formalization recorded 2026-05-10; 631 tests passing post-K8.2 v2; A'.4 K9 next per Phase A' sequencing.` |

**Rationale**: KERNEL_ARCHITECTURE.md header already at v1.5 (post-K-L3.1). The internal status snapshot line was NOT updated by `180d66d` (which only added v1.5 header note); it remains at the K5 closure wording.

### Phase 9.3 — Update FIELDS.md from Draft to Live (K9 brief lines ~1682–1693)

No override needed. ✓ Instruction correct as written.

### Phase 9.5 — Final atomic commit (K9 brief lines ~1714–1717)

| Field | Original | Corrected reading |
|---|---|---|
| `git add` paths | `docs/MIGRATION_PROGRESS.md docs/architecture/KERNEL_ARCHITECTURE.md docs/architecture/FIELDS.md tools/briefs/K9_FIELD_STORAGE_BRIEF.md` | Same paths (already correct post-`180d66d`). ✓ Current. |

### Authoring lineage (K9 brief lines ~1907–1920)

| Field | Original | Corrected reading |
|---|---|---|
| Source documents read | `KERNEL_ARCHITECTURE.md v1.0 LOCKED, MOD_OS_ARCHITECTURE.md v1.6 LOCKED` | At authoring time (2026-05-08), specs were at v1.0 / v1.6. By execution time (2026-05-10), specs are at v1.5 / v1.7. The architectural surfaces K9 implements **did not change** between v1.0 and v1.5 KERNEL (K-L3.1 added Path β as peer to Path α; K9 storage is orthogonal to Path α/β bridge — see K-L3.1 addendum §A2.5). Between v1.6 and v1.7 MOD_OS, the §4.6 surface added RegisterManagedComponent<T> alongside (not replacing) Fields/ComputePipelines. K9's Phase 1–7 implementation against the v1.0/v1.6-era contracts remains correct for v1.5/v1.7 contracts. |

**Rationale**: Executor should not panic that K9 brief cites old spec versions. The architectural surfaces K9 codes against are stable across the version bumps.

---

## What this patch does NOT change

K9 brief Phase 1 (RawTileField C++ core), Phase 2 (World field registry), Phase 3 (C ABI extension), Phase 4 (selftest scenarios), Phase 5 (managed bridge: FieldRegistry, FieldHandle, FieldSpanLease, FieldOperationFailedException), Phase 6 (CPU IsotropicDiffusionKernel reference), Phase 7 (bridge tests for FieldRegistry, FieldHandle, IsotropicDiffusionKernel — 27 tests total), and Phase 8 architectural design (IModFieldApi + IModComputePipelineApi placeholder + RestrictedFieldApi capability cross-check wrapper + capability parser extension + manifest validation tests) are **architecturally authoritative and unchanged**.

The Cross-cutting design constraints (K9 brief §1–§10) and Stop conditions (K9 brief §1–§6) are unchanged.

The atomic commit count from K9 brief (16–18 commits) is unchanged for K9 itself; this patch adds 1 commit (this document) on the `feat/k9-field-storage` branch as commit #1.

---

## Cross-cutting design constraint addendum

K9 brief Cross-cutting design constraint §11 is added (extending §1–§10 in K9 brief):

**11. Field storage is architecturally orthogonal to the K-L3.1 entity-component bridge** (per K-L3.1 addendum §A2.5). The Path α (`unmanaged struct`, NativeWorld) vs Path β (managed `class` via `[ManagedStorage]`, mod-side ManagedStore) decision applies to entity-keyed components, not to fields. Fields have their own identity (cell coordinate), storage (RawTileField), and capability verbs (`field.*` per MOD_OS §3.2). A field is never «Path α» or «Path β»; the dichotomy is undefined for fields. K9 implementation does not need to reference Path α/β anywhere.

Add to K9 brief Cross-cutting design constraints section if convenient during Phase 9.4 (execution summary append); not blocking otherwise.

---

## Execution session prompt (use this verbatim when opening Claude Code)

```
Execute milestone A'.4 = K9 (Field Storage Abstraction) bundled with A'.4.0 patch.

ATTACHED BRIEFS (read in this exact order):
1. tools/briefs/K9_BRIEF_REFRESH_PATCH.md   ← READ FIRST
2. tools/briefs/K9_FIELD_STORAGE_BRIEF.md    ← READ SECOND, apply patch overrides per patch §"Override Table"

REPOSITORY STATE (verify at start):
- HEAD = 38c2e19 or later on main; origin/main synced
- Baseline = 631 tests passing
- Working tree clean

COMMIT SEQUENCING:
- Create feat/k9-field-storage branch from main
- Commit #1: A'.4.0 patch brief on disk (this document, K9_BRIEF_REFRESH_PATCH.md)
- Commits #2-#N: K9 Phase 1–9 atomic commits per K9 brief commit log (16-18 commits)
- Final merge: --ff-only to main, then push origin main

OPERATIONAL REMINDERS:
1. Pre-`dotnet test`: kill leftover testhost.exe processes
   Get-Process testhost -ErrorAction SilentlyContinue | Stop-Process -Force
2. Use `dotnet test --logger "console;verbosity=minimal"` (NOT --verbosity quiet)
3. DualFrontier.Modding.Tests may take ~30 min (normal, not failure)
4. Native build: cd native/DualFrontier.Core.Native/build && cmake --build . --config Release
5. Native selftest: ./Release/df_native_selftest.exe

K9 PROFILE:
- First non-docs-only milestone Phase A' (real source changes)
- ~600-900 LOC native + ~400+ LOC managed bridge + 27 new bridge tests + 8 new native scenarios
- Estimated 8-12 hours auto-mode wall time
- Real test count drift expected: 631 → ~666-680

HALT-AND-ESCALATE if any unexpected situation arises that is not covered by:
- K9 brief Stop conditions §1-§6
- This patch's override table

Stash via `git stash push -m "k9-WIP-halt-$(date +%s)"` on halt.
```

---

## Brief authoring lineage (this patch)

- **2026-05-10** — Authored during Phase A' deliberation session (post-A'.3 push closure, HEAD `38c2e19`). Author: Claude Opus 4.7 deliberation session per Crystalka instruction «нужно просто написать дополнительный бриф к K9, я их запущу в одну сессию вместе, так как окно контекста в 1 миллион токенов позволяет это делать».
- **(date TBD)** — Committed on `feat/k9-field-storage` as commit #1 alongside K9 brief execution per A'.4 milestone closure.

This patch is companion documentation. K9 brief on disk is not modified — patch and K9 brief together constitute the contract for A'.4 execution.

---

**Patch end.** Companion to `tools/briefs/K9_FIELD_STORAGE_BRIEF.md`. Both required for A'.4 execution.
