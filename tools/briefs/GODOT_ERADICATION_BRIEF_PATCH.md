---
register_id: DOC-D-GODOT_ERADICATION_BRIEF_PATCH
project: Dual Frontier
category: D
tier: 4
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-06-29
last_modified: 2026-06-29
content_language: en
next_review_due: null
title: "GODOT ERADICATION BRIEF — PATCH 1 (folds the pre-ratification pressure-test's 8 findings: F-1 .uid-class blocker + F-2..F-8)"
last_modified_commit: 077a8c8
review_cadence: on-cascade-execution
last_review_date: 2026-06-29
last_review_event: 'Authored 2026-06-29 by Claude Opus post pressure-test. First-class delta artifact folding the 8 pressure-test findings into the base brief: F-1 (blocker) extends section 5-C + the F-5 authorization to the 204 .cs.uid class (default DELETE); F-2 full .gitignore Godot block; F-3 recursive .import glob; F-4 DEV_HYGIENE section 2:59 scrub; F-5 lifecycle gate widened to MAX_ENG:153; F-6 M75B2/UI_REVIEW HISTORICAL-KEEP; F-7 DOC-F-ASSETS-SCENES last_modified (not PATCH); F-8 section 1 attribution. Ratified by Crystalka 2026-06-29. Enrolled Draft at C1; flips to EXECUTED at C7.'
reviewer: Crystalka
risks_referenced: []
capa_entries_referenced: []
special_case_rationale: First-class brief-refresh delta artifact (patch precedent DOC-D-K9_BRIEF_REFRESH_PATCH / DOC-D-K8_3_BRIEF_REFRESH_PATCH); patches DOC-D-GODOT_ERADICATION_BRIEF v1.0 and folds into it at ratification, both transitioning to EXECUTED together at closure. DOC-D Category D Tier 4 per the empirical execution-brief convention (matches the patched brief's tier).
---

---
register_id: DOC-D-GODOT_ERADICATION_BRIEF_PATCH
project: Dual Frontier
category: D
tier: 4
lifecycle: Draft (folds into GODOT_ERADICATION_BRIEF at ratification; both -> LOCKED together)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-06-29'
content_language: en
authored_by: Claude Opus (deliberation session, post pressure-test)
basis: GODOT ERADICATION pre-ratification pressure-test 2026-06-29 (read-only, live-tree re-verification)
patches: DOC-D-GODOT_ERADICATION_BRIEF v1.0
---

# GODOT ERADICATION BRIEF -- PATCH 1

A pre-ratification read-only pressure-test re-verified the Godot Eradication brief
against the live tree (not trusting its inherited 2026-06-29 recon digest) and
confirmed its backbone is execution-ready -- every D1/D2 site, census, register
count, and "after"-text target verifies at its exact location -- while surfacing
eight findings, one of them a ratification blocker. This patch folds them in. It is
a first-class delta artifact (K9_BRIEF_REFRESH_PATCH precedent): read it alongside
the base brief; where they differ, this patch governs. Both transition to LOCKED
together at ratification.

The pressure-test itself is the verification: the brief's recon digest inherited a
gap, and only an independent live-disk re-check caught it. That re-check is the
correct architect move on a Draft brief -- it is read-only, so thorough is free.

---

## A. F-1 resolution (the blocker) -- ratified intent applied

**Finding:** the brief's file-surface claim "complete (recon R2)" is false. `find`
returns **204 untracked `*.cs.uid` Godot sidecar files** (one per `.cs`, across
`src/`, `tests/`, `mods/`; `git ls-files '*.uid'` = 0). They are the SAME inert,
untracked, Godot-generated residue class as the 11 `.import` files the brief
already deletes -- the repo's own `.gitignore` files both under "# Godot 4+ specific
ignores". Two recon passes missed them because both swept by an enumerated
extension list that did not include `.uid` (Lesson #N20 candidate: derive the
eradication class from the repo's own ignore/config classification, not a fixed
list). Consequence under the brief's own terms: section 0 lifts the deletion
prohibition for "exactly the artifacts enumerated in section 5-C," and `.uid` is
not in 5-C -- so an executor could not sweep them without exceeding the F-5 grant,
and the section 1 / section 9 "every remnant" claim would be false with 204
remnants surviving.

**Resolution (delete -- extends the existing Godot-deletion authorization to the
same class):** the operator's authorization ("Godot is already gone, only tails
remain; the engine is removed from the laptop") is categorical over Godot-generated
residue. The `.uid` files are precisely that class. Section 0's grant and section
5-C are extended to enumerate the `.uid` class. They are untracked, so their
deletion is a working-tree cleanup recorded in the C4 commit body as a manual step
(identical mechanism to the 11 `.import` files), producing no tracked diff of its
own; the build-green-as-inertness-proof gate covers it.

> One-line flip if the operator disagrees: replace "delete the `.uid` class" with
> "scope the `.uid` class OUT with a recorded rationale" in section 5-C, and the
> section 1 / section 9 wording is qualified accordingly. Default applied here:
> delete.

---

## B. Section patches

### Section 1 (Mission)
- The "every Godot remnant" claim becomes TRUE once F-1 is applied; keep it, now
  backed by the `.uid` inclusion.
- **F-8 (cosmetic):** the section 1 prose attributes the dead "Godot main thread"
  execution-model phrase to `GameContext`; that phrase lives in
  `GameBootstrapIntegrationTests.cs:288`, not `GameContext`. Correct the
  attribution; the per-site section 5-A list is already accurate.

### Section 2 (Established facts)
- Replace "File surface (complete, recon R2)" with the corrected surface: tracked
  `project.godot`, `icon.svg`, `icon.svg.import`; **untracked 11 `.import` +
  204 `.cs.uid`** Godot-generated sidecars (the full inert class per the repo's
  `.gitignore` Godot block); `.godot/`/`addons/` absent. `sample.dfscene` remains
  KEEP (project's own JSON, not Godot).
- Record that the surface claim is now derived from the repo's own `.gitignore`
  Godot classification, not an enumerated extension list.

### Section 3 (Phase 0) -- lifecycle-gate scope widened (F-5)
- The mandatory lifecycle determination is no longer PIPELINE_METRICS-only. It
  covers BOTH same-class tier-1 LOCKED docs: `PIPELINE_METRICS.md` AND
  `MAXIMUM_ENGINEERING_REFACTOR.md:153`. Read each doc's REGISTER lifecycle and its
  own charter before any edit. Both are pre-resolved in section C below; the gate
  still runs to confirm the determination against the live charter.

### Section 5-A (dead-code/comment cleanup) -- unchanged
All 7 sites verified at their exact line numbers; "after" targets grounded in real
code (real prod caller = Launcher `Program.cs:55` `GameBootstrap.CreateLoop`;
Application IVT = `Modding.Tests / Launcher / Core.Benchmarks`, no Presentation ->
`GameContext:16` confirmed doubly dead; live tick path =
`GameLoop -> PresentationBridge -> RenderCommandDispatcher.HandleTickAdvanced` on
the main render thread, `GameHUD` absent -> the `:288` fix is a clean swap). No
change.

### Section 5-B (living-doc de-Godot) -- two corrections
- **F-7:** `DOC-F-ASSETS-SCENES` carries register version literally `"Live"` -- there
  is no semver to PATCH. Align the instruction to a `last_modified` touch per its
  Live-doc convention, not a "PATCH" bump. (Section 7 mirrors this.)
- **ROADMAP:166 historicization must spare the live Native seams (pre-resolved C):**
  the line reads "contracts exist with both Godot and Native backends." The Native
  `IRenderer` / `ISceneLoader` / `IInputSource` seams are STILL live reserved
  surface. Past-tense ONLY the Godot backends; do NOT past-tense the Native
  contracts (that would falsely retire live surface -- an H5-adjacent error).
- `PIPELINE_METRICS.md` and `MAXIMUM_ENGINEERING_REFACTOR.md:153` move OUT of the
  mechanical D2 set and into the section 3 gate (pre-resolved LEAVE -- see C).

### Section 5-C (file-artifact deletion) -- F-1, F-2, F-3 applied
- **Tracked deletions (git rm):** `project.godot`, `icon.svg`, `icon.svg.import`
  (unchanged).
- **Untracked working-tree deletions (manual, recorded in C4 body):** **all 11**
  `.import` files via the recursive glob `assets/**/*.import` -- **F-3:** the brief's
  single-level `assets/kenney/*.import` matches only 10 of 11, missing
  `assets/kenney/terrain/roguelikeSheet_transparent.png.import`; the F-5 ledger's
  own scope `assets/**/*.import` (ROADMAP.md:965) is already correct, so use it --
  PLUS the **204 `*.cs.uid`** files (F-1). Verify post-delete: `find . -name '*.uid'
  -not -path './.git/*'` = 0 and `find . -name '*.import' -not -path './.git/*'` = 0.
- **`.gitignore` (F-2):** remove the entire "# Godot 4+ specific ignores" block --
  the pressure-test measured it as lines 1-9, not the brief's "1-3,9" (it leaves
  `/android/`, `export_presets.cfg`, `*.translation`, and critically `*.uid`, whose
  retention would re-mask F-1's residue). Cite the block by its comment header and
  remove what it delimits. The one arguably-generic entry (`*.tmp`) may be retained,
  relocated out of the Godot block under a generic comment; everything Godot-specific
  goes.
- `SCOPE_EXCLUSIONS.yaml` `.godot/**` patterns removed (unchanged).
- Empty-on-disk classes (`*.translation`, `export_presets.cfg`, `/android/`) have no
  files to delete -- only their ignore lines are removed (handled by the `.gitignore`
  edit). No deletion step needed for them.

### Section 5-D (DEV_HYGIENE flip) -- F-4 applied
- C5 flips section 7 to "Godot fully eradicated" -- now TRUE once F-1 lands (no
  surviving residue). Do NOT write "complete" if F-1 is scoped out.
- **F-4 (the doc-internal dangling-ref trap):** DEVELOPMENT_HYGIENE *also* names
  `project.godot` at **section 2 line 59** ("tracked legacy file -- do not delete
  (section 7)"). Deleting `project.godot` without scrubbing section 2:59 creates a
  fresh dangling reference to a deleted file INSIDE the very document certifying the
  eradication. C5 MUST scrub section 2:59 in the same commit (retire the line, or
  past-tense it to the completed eradication).

### Section 6 (Commit plan) -- scope expansions only, no new commits
- **C4** absorbs F-1/F-2/F-3 (the `.uid` + corrected `.import` glob + full
  `.gitignore` block); its tracked diff is unchanged in shape (3 `git rm` + 2 config
  edits), its commit body's manual-delete step grows from "11 `.import`" to
  "11 `.import` + 204 `.uid`."
- **C5** absorbs F-4 (section 2:59 scrub alongside the section 7 flip).
- **C6** scope-closure note absorbs the `.uid` class (section D below).
- No commit is added or removed; the count is intended-form per the base brief.

### Section 7 (REGISTER cascade) -- F-6, F-7 applied
- **F-6:** name the registered Godot-titled docs the brief never mentioned and
  RECORD their KEEP determination: **DOC-E-M75B2_GODOT_UI_SCENE** (REGISTER:3134,
  tier-3, lifecycle EXECUTED, titled "Godot UI Scene") and the lower-priority
  **DOC-E-UI_REVIEW_PRE_M75B2** are EXECUTED prompt/review records of
  work-done-then-retired -> **HISTORICAL-KEEP** (same class as the quarantined
  `historical/*` Godot docs). They are NOT deletion targets; naming + recording the
  keep closes the silent hole in the section 1 "every remnant" and section 9 census
  claims. Likewise the tracked `docs/prompts/M75B2_GODOT_UI_SCENE.md` is a
  HISTORICAL prompt snapshot -- KEEP.
- **F-7:** `DOC-F-ASSETS-SCENES` is a `last_modified` touch, not a PATCH (no semver).
- Document count delta unchanged (no registered doc is deleted -- the deleted FILES
  carry no `register_id`, confirmed; H6 holds).

### Section 9 (Closure census) -- F-1 additions
- Add to the closure census re-runs: `find . -name '*.uid' -not -path './.git/*'`
  -> 0 and `find . -name '*.import' -not -path './.git/*'` -> 0 (the full Godot
  file surface gone), alongside the existing `\bgodot\b` src = 9 HISTORICAL,
  native = 0, tracked artifacts = 0 checks.
- Record the F-6 KEEP set in the closure (the HISTORICAL Godot-titled docs that
  correctly survive).

---

## C. Adopted determinations (close the brief's open items)

1. **PIPELINE_METRICS.md -> HISTORICAL, LEAVE (exempt from D2).** Tier-1 LOCKED;
   its charter preserves "v1.x era measurements verbatim as historical record"
   with per-metric era annotations. Its 5 Godot refs are era-stack record plus a
   2026-05-10 forward-projection since overtaken by cascade #2. Scrubbing a LOCKED
   verbatim-historical record is the H5 inverse error. C3 does NOT touch it; record
   the determination. (Out of scope for this cascade, for the operator's awareness:
   the doc's own section 5.2/section 6 "Godot survives across boundary" forward-claim
   is now falsified -- a future methodology-corpus amendment, not this cascade.)
2. **MAXIMUM_ENGINEERING_REFACTOR.md:153 -> run the section 3 gate, default
   LEAVE + note.** LOCKED/RATIFIED snapshot; the line is a forward-caveat about an
   un-activated Track A. Genuinely ambiguous -> resolve by live-charter determination
   at Phase 0, not by pre-classifying it as a mechanical fix (F-5).
3. **ROADMAP:166 -> historicize the Godot backends only, spare the Native seams**
   (detailed in 5-B above).

---

## D. F-ledger additions (C6 / closure)

- **F-5 closure** extended: the F-5 scope-closure record now enumerates the full
  Godot file surface -- `project.godot`, `icon.svg`, `icon.svg.import`, 11
  `.import`, **204 `.uid`**, and the `.gitignore` / `SCOPE_EXCLUSIONS` Godot config
  -- so the entire inert surface closes under the one operator decision. F-5 -> CLOSED.
- **F-26 (NEW, seed):** "Eradication/cleanup recon swept artifacts by enumerated
  extension list and missed the `.uid` Godot residue class (204 files); two passes
  affected." Severity S2. State: CLOSED-by-this-cascade for Godot specifically;
  the METHODOLOGY lesson (derive the class from the repo's own ignore/config
  classification) is the Lesson #N20 candidate, owner: architect, formalize at next
  closure. Recorded so the next cleanup cascade inherits the corrected method.

---

## Ratification

Fold A-D into GODOT_ERADICATION_BRIEF at ratification; both transition Draft ->
LOCKED together. F-1 default is DELETE (the `.uid` + ignore classes); flip to
SCOPE-OUT with one line if the operator directs. Everything else (F-2..F-8 + the
three determinations) is unambiguous and pre-resolved.

Executor-seat note (minor, operator's call): the base brief names the executor as
"Fable 5"; the pressure-test ran on Opus (the architect seat) read-only, which is
correct for a Draft review. For EXECUTION, name whichever flagship seat is
available -- the cascade mechanics are seat-independent.

Bez kostylei.

**End of GODOT_ERADICATION_BRIEF_PATCH 1 v1.0**