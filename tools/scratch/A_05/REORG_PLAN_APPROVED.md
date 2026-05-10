# A'.0.5 Phase 2 — Reorganization plan (APPROVED — 2026-05-10)

**Authored**: 2026-05-10 by execution agent post-Phase-1 discovery
**Status**: APPROVED 2026-05-10 by Crystalka — Stop #1 cleared
**Approval**: Q0=A (commit pre-staged moves with rename detection), Q1=yes (move 14 subsystem-arch docs), Q2=yes (`docs/methodology/`), Q3=yes (live trackers stay), Q4=yes (briefs stay at tools/briefs/), Q5=yes (`docs/reports/`), Q6=yes (i18n stays), Q7=yes (Phase 0 brief authoring commit), Q8=yes (repo-rooted absolute cross-refs), Q9=yes (METHODOLOGY/PIPELINE_METRICS untouched in A'.0.5; only deferral markers), Q10=yes (Phase 0 deviation accepted)
**Acts on**: ~135 `.md` files repo-wide
**Derived from**: brief §4 plan template + Phase 1 discovery output

---

## §1 Source → destination mapping (RECOMMENDATION — needs confirmation)

### §1.1 Category A — Architecture spec → `docs/architecture/`

**Already pre-staged by Crystalka (just need commit; Phase 3 commit 1)**:
1. `docs/ARCHITECTURE.md` → `docs/architecture/ARCHITECTURE.md`
2. `docs/ARCHITECTURE_TYPE_SYSTEM.md` → `docs/architecture/ARCHITECTURE_TYPE_SYSTEM.md`
3. `docs/KERNEL_ARCHITECTURE.md` → `docs/architecture/KERNEL_ARCHITECTURE.md`
4. `docs/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` → `docs/architecture/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` *(classification ambiguous — could be B; current location signals A)*
5. `docs/MOD_OS_ARCHITECTURE.md` → `docs/architecture/MOD_OS_ARCHITECTURE.md`
6. `docs/MOD_PIPELINE.md` → `docs/architecture/MOD_PIPELINE.md`
7. `docs/RUNTIME_ARCHITECTURE.md` → `docs/architecture/RUNTIME_ARCHITECTURE.md`
8. `docs/VISUAL_ENGINE.md` → `docs/architecture/VISUAL_ENGINE.md`

**Recommended new moves (per brief §4.1)** — Phase 3 commit 2:
9. `docs/GPU_COMPUTE.md` → `docs/architecture/GPU_COMPUTE.md` (LOCKED v2.0 GPU compute spec)
10. `docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` → `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (LOCKED migration spec)
11. `docs/K_L3_1_AMENDMENT_PLAN.md` → `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (K-L3.1 amendment plan; architecture amendment artifact)

**Q1 to Crystalka — subsystem architecture docs (also recommended A; Phase 3 commit 3)**:
12. `docs/CONTRACTS.md` → `docs/architecture/CONTRACTS.md`
13. `docs/ECS.md` → `docs/architecture/ECS.md`
14. `docs/EVENT_BUS.md` → `docs/architecture/EVENT_BUS.md`
15. `docs/FIELDS.md` → `docs/architecture/FIELDS.md`
16. `docs/GODOT_INTEGRATION.md` → `docs/architecture/GODOT_INTEGRATION.md`
17. `docs/ISOLATION.md` → `docs/architecture/ISOLATION.md`
18. `docs/MODDING.md` → `docs/architecture/MODDING.md`
19. `docs/OWNERSHIP_TRANSITION.md` → `docs/architecture/OWNERSHIP_TRANSITION.md`
20. `docs/RESOURCE_MODELS.md` → `docs/architecture/RESOURCE_MODELS.md`
21. `docs/THREADING.md` → `docs/architecture/THREADING.md`
22. `docs/FHE_INTEGRATION_CONTRACT.md` → `docs/architecture/FHE_INTEGRATION_CONTRACT.md`
23. `docs/COMBO_RESOLUTION.md` → `docs/architecture/COMBO_RESOLUTION.md`
24. `docs/COMPOSITE_REQUESTS.md` → `docs/architecture/COMPOSITE_REQUESTS.md`
25. `docs/PERFORMANCE.md` → `docs/architecture/PERFORMANCE.md` *(or to docs/reports/ as Category E? performance ARCHITECTURE doc not performance REPORT; A appropriate)*

### §1.2 Category B — Methodology → `docs/methodology/` (recommended)

**Q2 to Crystalka — confirm new subdirectory; alternative: keep at docs/ flat**:
26. `docs/METHODOLOGY.md` → `docs/methodology/METHODOLOGY.md` *(target of A'.0.7 substantive rewrite; A'.0.5 only relocates + adds deferral marker)*
27. `docs/PIPELINE_METRICS.md` → `docs/methodology/PIPELINE_METRICS.md` *(same)*
28. `docs/MAXIMUM_ENGINEERING_REFACTOR.md` → `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md`
29. `docs/CODING_STANDARDS.md` → `docs/methodology/CODING_STANDARDS.md`
30. `docs/DEVELOPMENT_HYGIENE.md` → `docs/methodology/DEVELOPMENT_HYGIENE.md`
31. `docs/TESTING_STRATEGY.md` → `docs/methodology/TESTING_STRATEGY.md`

### §1.3 Category C — Live trackers → `docs/` (stay at root, recommended)

**Q3 to Crystalka — confirm**:
- `docs/MIGRATION_PROGRESS.md` — STAY at docs/
- `docs/ROADMAP.md` — STAY at docs/
- `docs/IDEAS_RESERVOIR.md` — STAY at docs/

Rationale: live trackers are mutable, contrasted with LOCKED architecture specs. Keeping them at docs/ root signals their distinct character.

### §1.4 Category D — Briefs → `tools/briefs/` (stay, recommended)

**Q4 to Crystalka — confirm**:
- `tools/briefs/*` — STAY (no moves)

Rationale: briefs are operational artifacts. The `tools/` prefix signals their character.

### §1.5 Category E — Discovery / closure / audit

**Q5 to Crystalka — consolidation: create `docs/reports/`**:

**Already in subdirectories (stay)**:
- `docs/audit/*` — STAY
- `docs/prompts/*` — STAY
- `docs/learning/*` — STAY
- `docs/benchmarks/*` — STAY

**Recommended consolidation to `docs/reports/`**:
- `docs/PERFORMANCE_REPORT_K3.md` → `docs/reports/PERFORMANCE_REPORT_K3.md`
- `docs/PERFORMANCE_REPORT_K7.md` → `docs/reports/PERFORMANCE_REPORT_K7.md`
- `docs/CPP_KERNEL_BRANCH_REPORT.md` → `docs/reports/CPP_KERNEL_BRANCH_REPORT.md`
- `docs/NATIVE_CORE_EXPERIMENT.md` → `docs/reports/NATIVE_CORE_EXPERIMENT.md`
- `docs/NORMALIZATION_REPORT.md` → `docs/reports/NORMALIZATION_REPORT.md`

Alternative: keep at docs/ flat (no moves). Crystalka decides.

### §1.6 Category F — Module-local → no moves

All `src/**/README.md`, `mods/**/README.md`, `tests/**/README.md`, `native/**/MODULE.md`, `assets/scenes/README.md` — STAY.

### §1.7 Category G — Project meta → no moves

`README.md`, `docs/README.md` — STAY.

### §1.8 Category H — i18n → STAY at docs/ root (recommended)

- `docs/TRANSLATION_GLOSSARY.md` — STAY
- `docs/TRANSLATION_PLAN.md` — STAY

---

## §2 Cross-reference impact preview

### Stale path references after Phase 3

| Source path | Target referenced | Files referencing (estimated) |
|---|---|---|
| `docs/KERNEL_ARCHITECTURE.md` | (after move: `docs/architecture/KERNEL_ARCHITECTURE.md`) | ~15-20 (per Phase 1 grep §1.4 staleness scan) |
| `docs/MOD_OS_ARCHITECTURE.md` | `docs/architecture/MOD_OS_ARCHITECTURE.md` | ~10-15 |
| `docs/RUNTIME_ARCHITECTURE.md` | `docs/architecture/RUNTIME_ARCHITECTURE.md` | ~10 |
| `docs/GPU_COMPUTE.md` | `docs/architecture/GPU_COMPUTE.md` | ~5-10 |
| `docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | ~5-10 |
| `docs/METHODOLOGY.md` | `docs/methodology/METHODOLOGY.md` (if relocated) | ~10-20 |
| `docs/PIPELINE_METRICS.md` | `docs/methodology/PIPELINE_METRICS.md` (if relocated) | ~5 |
| Each subsystem-arch doc (CONTRACTS, ECS, etc.) | `docs/architecture/CONTRACTS.md`, etc. | per-doc 5-15 |

Total estimated stale refs: ~150-300 across ~30-50 source files.

### Cross-ref convention (Q8 to Crystalka)

**Recommendation**: repo-rooted absolute paths (`/docs/architecture/KERNEL_ARCHITECTURE.md`) — works in GitHub markdown, unambiguous, no per-file relative-path computation.

Alternative: explicit relative paths (`../../architecture/KERNEL_ARCHITECTURE.md`) — more local but error-prone.

---

## §3 Estimated commit count

Per brief §13 estimates, recalibrated post-discovery:

| Phase | Commits | Notes |
|---|---|---|
| 0 | 1 | Brief authoring commit + .gitignore for tools/scratch (if Crystalka wants the scratch dir gitignored — small ask) |
| 1 | 0 | Discovery, no commits |
| 2 | 0-1 | Plan committed for audit trail (optional; this file goes into final closure) |
| 3 | 4-6 | Cat A pre-staged, Cat A new, Cat A subsystem (if approved), Cat B (if relocated), Cat E (if reports/ created), Cat H (if relocated) |
| 4 | 3-5 | Cross-ref refresh per category |
| 5 | 1 | 5 component READMEs cleanup |
| 6 | 6-10 | One per module: Core, Core.Interop, Components/{Combat,World,Magic,Pawn}, Systems/{Combat,World,Pawn,Magic,Faction,Events,top}, mods/, others as needed |
| 7 | 2 | Mechanical scrub + METHODOLOGY/PIPELINE_METRICS deferral markers |
| 8 | 1 | Tier 1 fixes |
| 9 | 3 | Sequencing update + MIGRATION_PROGRESS + brief EXECUTED |

**Total estimate**: 21-29 atomic commits.

---

## §4 Stop #1 questions consolidated

| # | Question | Recommendation | Crystalka decision |
|---|---|---|---|
| Q0 | Working-tree resolution: commit pre-staged moves (Option A) or revert+redo via `git mv` (Option B)? | A | **A** |
| Q1 | Move subsystem architecture docs (CONTRACTS, ECS, EVENT_BUS, etc., 14 files) to `docs/architecture/`? | yes | **yes** |
| Q2 | Methodology destination: `docs/methodology/` or stay at docs/ flat? | docs/methodology/ | **yes (docs/methodology/)** |
| Q3 | Live trackers: stay at docs/ flat? | yes | **yes** |
| Q4 | Briefs: stay at tools/briefs/? | yes | **yes** |
| Q5 | Create `docs/reports/` for stand-alone reports? | yes | **yes** |
| Q6 | i18n docs: stay at docs/ flat? | yes | **yes** |
| Q7 | Brief authoring commit on main as Phase 0 commit? | yes (per METHODOLOGY §«Brief authoring as prerequisite step») | **yes** |
| Q8 | Cross-ref convention: repo-rooted absolute paths? | yes | **yes** |
| Q9 | METHODOLOGY/PIPELINE_METRICS substance untouched in A'.0.5; only A'.0.7 deferral markers added in Phase 7? | yes | **yes** |
| Q10 | Brief deviation noted at Phase 0: working-tree pre-staged moves not flagged as STOP per recommended Option A. Accept? | yes | **yes** |

---

## §5 Plan acceptance protocol

When Crystalka responds with answers to Q0-Q10:

- If all answers match recommendations: I rename this file to `REORG_PLAN_APPROVED.md`, commit it as Phase 0 commit (alongside brief authoring commit), proceed to Phase 3.
- If any answer differs: I update relevant §1.x or §2 of this plan, re-present, await re-confirmation. Phase 3 does not begin without explicit approval per brief §4.2.
- If Crystalka has substantive disagreements with the plan structure (not just per-question): plan revisited collaboratively in chat; Phase 2 re-runs to resolve.
