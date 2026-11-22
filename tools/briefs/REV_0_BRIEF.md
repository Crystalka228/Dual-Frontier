---
register_id: DOC-D-REV_0_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-11-21'
last_modified: '2026-11-21'
content_language: en
next_review_due: null
title: 'REV_0 -- main is green again and the W4 governance residue is paid: docs/SECRET_SCANNING.md joins the register, the 16-item W4 residue work-order is executed, F-57/F-85/F-89/F-42 are settled, and F-90/F-91/F-92 are seeded -- the first cascade of CORPUS_REVISION_1'
authored_by: Claude Opus (architect-orchestrator session, CORPUS_REVISION_1)
last_review_event: 'REV_0 EXECUTED 2026-11-21 (EVT-2026-11-21-REV_0) on claude/rev-0, C1-C3 plus 15 review-repair commits and the C4 closure. Ratified by the operator in chat on 2026-11-21; lifecycle Draft -> EXECUTED. Orchestrator rulings R-1..R-4 supplemented the brief: R-1 voided a driver reference to a non-existent v1.1; R-2 landed the moved deferred census pin (87/54 -> 86/53) as a C3 integrator seam; R-3 ruled the lens-B repairs, including F-84 widened and F-93 seeded; R-4 ruled the two P3 repairs of the lens over the repairs. Gates: validate --armed exit 0, suite 1441/0/6, dev smoke exit 124.'
basis: 'CORPUS_REVISION_1 recon deposits R-A/R-B/R-C/R-D + REVISION_SYNTHESIS (/mnt/Work/Temporal/corpus-revision-1/, read-only, 2026-11-21 at 3000285) + the DIST_1 Wave R residue re-verification R2 (/mnt/Work/Temporal/dist1-run/recon/R2.journal.md, Haiku, 2026-11-21 at 3000285) + the W4 residue work-order (/mnt/Work/Temporal/recon-2026-09-01/H0_RESIDUE_WORKORDER.md) + operator rulings 2026-11-21 (documents first; DIST_1 paused; its document part moves here; comment law adopted from Community Hub)'
---

# REV_0 -- Execution Brief

`main` is governance-red: PR #55 merged a frontmatter-less `docs/SECRET_SCANNING.md`,
so `validate --armed` exits 1 and `Validate_AgainstLiveTree_Succeeds` fails. The W4
governance residue (16 items, verified at W4 closure, re-verified 2026-11-21) is
still unpaid. This cascade restores a green `main` and pays the residue -- the
document part of DIST_1, moved here by the operator's documents-first ruling. Done =
`validate --armed` exit 0, suite 1441 / 0 / 6, the residue items each executed or
recorded no-action, the ledger rows settled and seeded, one closure EVT.

Executed by the architect-orchestrator through subagents (integrator commits on
`/mnt/Work/_wt/df-rev-0`, branch `claude/rev-0`; units in their own worktrees),
repository `/mnt/Work/Colony_Simulator`. Brief-integration notice: this brief CITES
standing law -- commit body per `CODING_STANDARDS.md` section 8, push law section 8.4
(v3.0.0), census per `TESTING_STRATEGY.md`, mutability per
`RESERVED_SURFACE_MUTABILITY.md`, closure per `METHODOLOGY.md`. A conflict with a
standing doc means THE BRIEF IS WRONG -- halt. A conflict with live code means THE
CODE IS THE TRUTH -- record and proceed per section 2.

## 1. Mission [CORE]

| D | Artifact | Action |
| - | -------- | ------ |
| D1 | this brief; `docs/SECRET_SCANNING.md` | enroll (brief D/3/Draft; SECRET_SCANNING C/2/Live, body byte-untouched) |
| D2 | `docs/ROADMAP.md` ledger + four law/plan docs | residue items 1-3, 6-8, 11, 13-15 + F-57/F-85/F-89 + seed F-92 |
| D3 | `DualFrontier.Systems.csproj`, `ISystemServices.cs`, both Weather csprojs, two test files | residue items 4, 5, 9, 10 (code plane: one dead edge, comments) |
| D4 | `AUDIT_TRAIL.yaml`, this brief | closure EVT; brief EXECUTED |

Why first: every later revision cascade gates on `validate --armed` exit 0, and a
red `main` makes every baseline ambiguous.

## 2. Established facts [CORE]

(RV) = re-verified at Phase 0; mismatch -> H1. Anchors at HEAD `3000285`.

1. (RV) `main` = `3000285` (PR #55 + #56 on top of `4d8ad86`). `validate --armed`
   exits 1 with exactly one error: `[G-PATH] docs/SECRET_SCANNING.md: no frontmatter
   block and not on the exclusion list`. Suite 1440 / 1 / 6, the failure being
   exactly `SyncIntegrationTests.Validate_AgainstLiveTree_Succeeds`; the 6 skips are
   SchedulerExtremeTests S2/S3/S4/S5a/S5b and
   RuntimeCompositionTests.Create_with_validation_enabled_composes_validation_layer.
   Build 0W/0E; selftest ALL PASSED; dev smoke exit 124 clean (measured twice
   2026-11-21, architect + DIST_1 pre-flight).
2. (RV) `DOC-C-SECRET_SCANNING` and `DOC-D-REV_0_BRIEF` are absent from
   `REGISTER.yaml`, `CURRENT_AUTHORITY_SURFACE.yaml`, `AUDIT_TRAIL.yaml`.
3. (RV) Residue items 1-11 and 13-15 are STILL PRESENT; items 12 and 16 are
   no-action, confirmed (R2). Corrections R2 measured against the work-order:
   item 3's `MovementSystem.cs` anchor is `:34` (not `:35`); `RandomPawnFactory.cs`
   no longer exists -- its seed sites are now
   `mods/DualFrontier.Mod.Vanilla.Scenario/ScenarioSeeder.cs:180` and `:243`;
   item 7's system count measured **27** (classes deriving `SystemBase` under
   `src/DualFrontier.Systems/`), against 29 (`VANILLA_SEPARATION_MIGRATION_PLAN.md:217`)
   and 30 (`GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md:95`) -- the unit RE-COUNTS
   with a stated predicate and records the command; item 10 has exactly two code
   sites (`tests/DualFrontier.Core.Tests/SharedNativeSingletonCollection.cs:37`,
   `tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs:61`); item 15
   ("46 distinct tests", `tools/briefs/W4_COMPOSITION_ROOT_BRIEF.md:15`, `:498`) is
   still underived -- the unit derives it.
4. (RV) Ledger: 89 rows F-1..F-89, no gaps; F-90, F-91, F-92 are free (R-D). F-57's
   severity cell is `native-switch` (`docs/ROADMAP.md:1117`); F-85 (`:1145`) and
   F-89 (`:1149`) are OPEN; F-42 (`:1102`) anchors deleted files.
5. **Comment law (operator ruling 2026-11-21, enrolled at a later revision
   cascade; binding on every comment this cascade writes):** a comment states what
   the code cannot express (purpose, constraint, non-obvious behaviour) and may cite
   architecture law in force only -- К-L#, PA-00#, enrolled LOCKED/Live document
   sections, analyzer rule ids. It NEVER carries provenance: F-# rows, Lesson #N,
   cascade or wave names, PR numbers, commit hashes, briefs, "Phase N", repair or
   history narratives, counts. Model: Community Hub
   `docs/architecture/CODING_STANDARDS.md` "Comments and documentation".
6. `docs/ROADMAP.md` frontmatter `last_review_event` is a multi-kilobyte history
   log (R-B/R-A finding); this cascade only advances it per item 13 and does NOT
   restructure it (REV-3/REV-6 territory).

## 3. Phase 0 -- preconditions and checkpoint [CORE]

1. Verify section 2 (RV) facts; mismatch -> H1.
2. Native present (copied into the worktree, never rebuilt); selftest ALL PASSED.
3. Baseline: build 0W/0E; full suite expected **1440 / 1 / 6** with the named
   failure (never pipe `dotnet test`); one dev smoke exit 124 clean.
4. `validate --armed`: exit 1 with EXACTLY the one G-PATH error of 2.1; any other
   error -> H3. From C1 onward: exit 0 at every commit, else H3.
5. Frontmatter-shape read: FRAMEWORK section 14.3/14.4; the live `docs/ROADMAP.md`
   frontmatter (the C/2/Live shape for SECRET_SCANNING); one existing
   `AUDIT_TRAIL.yaml` closure EVT as the append template.
6. Mandatory reads: this brief; the work-order; R2; the files each item edits.

`sync` in every frontmatter-touching commit; derived registers never hand-edited;
`AUDIT_TRAIL.yaml` append-only. Closure: push the WORK branch once, open the PR;
never `main`, never merge.

## 4. Topology [CORE]

Orchestrator -> integrator (opus/high, the only committer) -> two units in parallel
with DISJOINT file sets: U-DOC (opus/high -- every line it writes is record) owns D2;
U-CODE (sonnet/medium, precise spec) owns D3. Single-writer: `docs/ROADMAP.md` is
U-DOC's alone; `AUDIT_TRAIL.yaml` and the brief are the integrator's alone (C1, C4).

## 5. Wave R -- survey agents

None: the recon is done and re-verified (section 2 basis). Units read the work-order
and R2 directly.

## 6. Checkpoints [CORE]

- **C-green** (after C1): `validate --armed` exit 0; suite 1441 / 0 / 6.
- **C-residue** (after C2-C3): validate exit 0; suite 1441 / 0 / 6 (no test added or
  removed -- only comments and one dead ProjectReference change); build 0W/0E; dev
  smoke exit 124 clean; every work-order item has an outcome line (executed with
  locus, or no-action with reason) in the closure report.

## 7. Execution / writer specifications [CORE]

Every edit is a CONSOLIDATION of an already-ratified decision; a needed new
semantics is H6.

**7.1 C1 (integrator seam).** Enroll this brief (frontmatter as written) and
`docs/SECRET_SCANNING.md`: prepend a frontmatter block shaped on the live
`docs/ROADMAP.md` block -- `register_id: DOC-C-SECRET_SCANNING`, category C, tier 2,
lifecycle Live, `version: Live`, `first_authored: 2026-10-12` (the PR #55 commit
date), owner and review fields per the ROADMAP shape, a quoted title. The body stays
byte-identical (`git diff` shows only the prepended block). sync; validate exit 0;
suite 1441 / 0 / 6.

**7.2 U-DOC (C2).** File set: `docs/ROADMAP.md`,
`docs/architecture/EXECUTION_AUTHORITY_MATRIX.md`,
`docs/architecture/VANILLA_SEPARATION_MIGRATION_PLAN.md`,
`docs/architecture/GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md`,
`docs/architecture/MOD_OS_ARCHITECTURE.md` (+ any frontmatter `last_modified`
bump these edits require). Items (work-order numbering):
- 1: seed **F-90** -- DFK005's GameBootstrap carve-out was removed at W4 (stronger
  than the owed transfer); the row records the sunset and the analyzer's present shape.
- 2: seed **F-91** -- `GameServices`/`IGameServices` genre-noun vocabulary
  (Combat/Inventory/Magic/Pawns/World) survives in engine `Core.Bus` as cosmetic
  bridges over one `DomainEventBus`; honest-record row, vocabulary territory.
- seed **F-92** -- no CI job runs `validate --armed`, the build, or the suite;
  `.github/workflows/gitleaks.yml` is the only workflow, so `main` went
  governance-red unseen (PR #55; paid at this cascade's C1). S2, OPEN, owner
  architect; the cheapest first slice (a pure-.NET validate-only job) is named as
  the candidate, not executed; making a check required is the operator's
  branch-protection setting.
- 3: append the F-42 coordination note (its anchors died with GameBootstrap; the
  manifest now carries every seed; the RNG service remains unbuilt) and re-anchor
  the row on `game.manifest.json` + `ScenarioSeedSystem` + the live seed sites of 2.3.
- 6: resolve the EAM R13 self-contradiction -- strike R13 from the vacant list (the
  reading-rule mention and the status table; re-locate both by text, not by the
  work-order's line numbers) -- the row itself already says settled.
- 7: align the system-count stock in VANILLA_SEPARATION_MIGRATION_PLAN and
  GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY to the RE-COUNTED value; state the
  predicate and command in the commit body.
- 8: record the fault-policy inversion where readers live -- a note in
  MOD_OS_ARCHITECTURE (the section 9.5 / 10 origin split) that the ten gameplay
  systems moved from fail-fast to quarantine, citing the W4 ruling as recorded in
  the W4 brief (a document citation is lawful in a DOCUMENT; the comment law binds
  code comments only).
- 11: the consolidated post-hoc `Skeleton revisions` record for 69ed79c and 375b702
  as a ROADMAP W4 annotation (history itself is immutable).
- 13: advance ROADMAP `last_review_date` / `last_review_event` to the W4 closure --
  PREPEND a short W4 sentence; do not restructure the field (2.6).
- 14: the explicit-none line in the W4 orphan-deletion record ("none of these is a
  historical record").
- 15: derive the W4 brief's "46 distinct tests" from the W4 diffs; record the true
  number in the ROADMAP W4 annotation (the EXECUTED brief keeps its bytes).
- F-57: severity `native-switch` -> `S2`. F-85: CLOSED (the four STALE banners
  discharge the obligation; full re-anchoring belongs to the architecture
  re-anchoring cascade of this revision series). F-89: CLOSED by the EAM R13 note --
  item placement excludes colonist tiles by the seeder's own rule (doc update only;
  write it into EAM in the same edit as item 6).
Report: one outcome line per item; the re-count and re-derivation commands verbatim.

**7.3 U-CODE (C3).** File set: `src/DualFrontier.Systems/DualFrontier.Systems.csproj`,
`src/DualFrontier.Contracts/Sdk/ISystemServices.cs`, both Weather csprojs
under `mods/`, `tests/DualFrontier.Core.Tests/SharedNativeSingletonCollection.cs`,
`tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs`. Items:
- 4: cut the dead `Systems -> AI` ProjectReference -- PROVE zero uses first
  (`git grep` for the AI namespace and types under `src/DualFrontier.Systems/`, and a
  green build with the edge removed); game->game, outside the ratchet; prose
  census-delta only.
- 5: `ISystemServices.cs` -- the doc comment that calls the mod-facing factory
  overload deferred is false (`IModApi.RegisterSystem<T>(Func<ISystemServices,T>)`
  exists): rewrite it to the live truth UNDER THE COMMENT LAW (2.5).
- 9: both Weather csproj comment blocks -- the discoverer takes an explicit root
  derived from the distribution root, there is no cwd scan; rewrite under the
  comment law.
- 10: remove the two stale `GameBootstrapIntegrationTests` mentions; if the
  surrounding comment still carries a reason, keep the reason and drop the dead name.
Gate: build 0W/0E; Core.Tests + Modding.Tests green over the staged tree.

**7.4 C4 (integrator seam, closure).** Ledger final-state table in the closure
report; ONE closure EVT appended (real C1-C3 hashes; it records that the brief was
ratified by the operator in chat on its ratification date -- ruling 2026-11-21);
brief -> EXECUTED; sync; validate exit 0.

## 8. Kind-specific machinery [KIND: governance]

Each ledger edit keeps the row's existing form (pipes, escaped pipes); a new row
copies the form of F-89. New ids are minted as the maximum existing id + 1 in order
F-90, F-91, F-92.

## 9. S-LOCK invariants [CORE]

None new; no К-L invariant, Q-lock or S-LOCK text is touched.

## 10. Census discipline [CORE]

C3 touches comments in `.cs` files: run the `CensusMetaTests` pins before and after;
any moved pin carries its census-delta in the C3 body. New prose must not introduce
census marker words (`deferred`/`stub`/`TODO`/`not yet`/`Phase 6`) -- U-CODE checks
its added lines.

## 11. Commit plan [CORE]

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(enroll): REV_0 brief enrolled; SECRET_SCANNING joins the register` | 7.1 + sync + validate exit 0 |
| C2 | `governance(residue): the W4 document residue is paid -- F-90/F-91/F-92 seeded, F-57/F-85/F-89/F-42 settled` | 7.2 + sync + validate |
| C3 | `chore(residue): the W4 code-plane residue is paid -- one dead edge, four stale comments` | 7.3 + census-deltas |
| C4 | `governance(closure): REV_0 EVT -- main green, W4 residue paid` | 7.4 + brief EXECUTED + sync + validate |

## 12. REGISTER cascade [CORE]

C1 enrolls two documents; C2 bumps frontmatter of the docs it edits; C4 flips the
brief EXECUTED and appends ONE EVT. `validate --armed` exit 0 at C1-C4.

## 13. Halt conditions (H-series) [CORE]

H1 Phase 0 mismatch. H2 gate regression vs baseline. H3 validate nonzero (beyond the
2.1 baseline before C1). H5 register vocabulary gap. H6 an item needs NEW semantics.
H7 any `native/` change. Standing rails: push law; derived registers never
hand-edited; AUDIT_TRAIL append-only; no history rewrite; never pipe `dotnet test`;
units never commit. On halt: stop, report verbatim, await the orchestrator.

## 14. Closure protocol and report [CORE]

Commits table; gates table baseline vs closure (build, suite with arithmetic,
selftest, validate, dev smoke); one outcome line per work-order item; ledger
final-state table (F-42, F-57, F-85, F-89, F-90, F-91, F-92); the re-count and
re-derivation readings; census before/after; friction table (ruling 2026-11-21);
self-attestation (8.4 v3.0.0, sync every governance commit, single EVT, no
rewrite, native untouched). Then push the work branch once and open the PR.

## 15. Out of scope [CORE]

The DIST_1 code part (F-63/F-76 deletions, the IVT cut, the orchestrator, the
deaths, the Weather targets) -- DIST_1 v1.2 after the revision; the comment-law
enrollment and its backlog sweep; the ledger split; the architecture re-anchoring
(GameBootstrap anchors, F-84, F-88); every other revision finding.

---

**End of REV_0_BRIEF.md v1.0**
