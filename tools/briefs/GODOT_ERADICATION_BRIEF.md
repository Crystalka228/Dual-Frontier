---
register_id: DOC-D-GODOT_ERADICATION_BRIEF
project: Dual Frontier
category: D
tier: 4
lifecycle: Draft (→ LOCKED on Crystalka ratification → EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-06-29'
content_language: en
authored_by: Claude Opus (deliberation session, Godot Eradication prep)
basis: GODOT ERADICATION RECON REPORT 2026-06-29 (R1–R8)
f5_authorization: Crystalka granted deletion 2026-06-29 (engine already removed from Skarlet; artifacts proven inert)
---

# GODOT ERADICATION CASCADE — Execution Brief

Single-pass execution. Executor: **Claude Code, Fable 5, LOCAL on Skarlet**. Repository: `D:\Colony_Simulator\Colony_Simulator`. No other tree involved.

**Brief-integration notice** (TESTING_STRATEGY v2.0.0 §6): this brief CITES standing law by anchor. Binding by citation: commit-body structure & marker law — CODING_STANDARDS v2.0.0 §8/§5 + §6.1 citation-form; truth law (existence test, forbidden enforcement verbs) — the v2.0.0 docs; mutability license & `Skeleton revisions` form — RESERVED_SURFACE_MUTABILITY v1.0; session closure — METHODOLOGY v1.13.0; census-delta discipline (hard/soft pin distinction) — TESTING_STRATEGY v2.0.0 §4.2. A conflict between this brief and any standing doc means THIS BRIEF IS WRONG — halt and escalate.

**F-5 is RESOLVED**: Crystalka authorized deletion of the entire Godot file surface on 2026-06-29 (engine already uninstalled from Skarlet; recon R2/R6 proved every artifact inert — zero build/tooling/package reference). This cascade closes F-5 (S3 OPEN → CLOSED) and executes the deletion. The autonomous-deletion prohibition in DEVELOPMENT_HYGIENE §7 is lifted BY this ratification for exactly the artifacts enumerated in §5-C.

---

## §1 Mission

Eradicate every Godot remnant — the tails left after cascade #2 deprecated the engine. Recon established the headline: **LIVE Godot sites = 0** (cascade #2 was complete at code+build), so this is cleanup of DEAD residue and inert file artifacts, not a dependency extraction. Three classes:

- **A — Dead code/comment cleanup** (ungated): 7 code-comment sites in `tests/` + `GameContext.cs` referencing deleted types (`GameHUD`, `DualFrontier.Presentation`, `GameRoot`) and the dead "Godot main thread" execution model.
- **B — Dead doc-reference cleanup** (ungated): living docs with present-tense Godot claims or instructions to deleted tools (`assets/scenes/README.md`, `PIPELINE_METRICS.md`*, `MAXIMUM_ENGINEERING_REFACTOR.md`, `ROADMAP.md` two sites). *pending the lifecycle check in §3.
- **C — File-artifact deletion** (gated → now authorized): `project.godot`, `icon.svg`, `icon.svg.import`, 11× `assets/kenney/*.import`, and the now-obsolete `.gitignore` / `SCOPE_EXCLUSIONS.yaml` Godot-config blocks.

Hard preservation rule: the **9 `src/**/*.cs` Godot comments are HISTORICAL and STAY** (recon R3.6 confirmed the prior grade correct — past-tense retirement narration on live/reserved types naming the current truth). The reserved seams `ISceneLoader`/`IInputSource`/`IDevKitRenderer` are governed reserved surface, NOT removal targets. Deleting HISTORICAL narration would falsify the record — that is the inverse error and is itself a halt-class mistake (**H5**).

Deliverables:

| # | Artifact | Action |
|---|---|---|
| D1 | 7 code-comment sites (A) | comment-only edits (dead-host/dead-type refs → current truth or removed) |
| D2 | Living-doc de-Godot (B): `assets/scenes/README.md` (+ `DOC-F-ASSETS-SCENES` frontmatter re-sync), `MAXIMUM_ENGINEERING_REFACTOR.md:153`, `ROADMAP.md:98/166`, `PIPELINE_METRICS.md`* | doc edits per §3 lifecycle finding |
| D3 | File-artifact deletion (C): 14 files + `.gitignore`/`SCOPE_EXCLUSIONS` Godot blocks | `git rm` + config edit |
| D4 | F-5 resolution record + scope-closure note (incl. the `icon.svg` scope-gap recon surfaced) | ROADMAP F-ledger |
| D5 | REGISTER cascade + validate + render; marker-census refresh (hard/soft per §4.2) | governance closure 2.17 → 2.18 |

## §2 Established facts (recon digest — re-verify ▲ in Phase 0)

▲ main @ `187c46c`, tree clean but for untracked `build/` (not Godot, leave it), register 2.17 / 273 docs / 40 EVT. Origin in sync (local ref).
- **Zero** `using Godot;`, **zero** `Godot.` qualifiers, **zero** native `\bgodot\b`, **zero** Godot package references (only a comment in `Directory.Packages.props:37`). Build green at `187c46c` ⇒ managed tree has no Godot compile dependency; the entire managed Godot surface is comment text.
- DEAD code set (7 sites, comment-only, no compile impact): `GameContext.cs:8` ("GameRoot in production"), `GameContext.cs:16` ("Reachable from DualFrontier.Presentation via InternalsVisibleTo" — doubly dead: deleted project + csproj already lists Modding.Tests/Launcher/Core.Benchmarks, no Presentation), `GameBootstrapIntegrationTests.cs:23` (`<see cref="DualFrontier.Presentation.Nodes.GameRoot"/>` dangling cref), `:34` (stale "Out of scope: Godot UI" note), `:146` ("Godot launches with cwd…" present-tense false), `:288` ("GameHUD.SetTick on the Godot main thread" — `GameHUD` deleted), `ModMenuControllerTests.cs:33` (stale "Godot UI scene tests" scope note).
- DEAD doc set: `assets/scenes/README.md:4,10,21` (Godot-build/DevKit-plugin instructions to deleted tools + stale `VISUAL_ENGINE.md` path — and it is a REGISTERED LIVE doc `DOC-F-ASSETS-SCENES`), `MAXIMUM_ENGINEERING_REFACTOR.md:153` ("Godot/.NET game runtime untested"), `ROADMAP.md:98` ("Phase 5 introduces them as real `Godot.Control`/`Godot.Node2D`" — forward plan to use Godot), `ROADMAP.md:166` (closed-phase record with present-tense "Godot works as both editor and temporary runtime").
- **DEAD-living-spec vs HISTORICAL-snapshot UNDECIDED**: `PIPELINE_METRICS.md` (5 refs, "Godot survives across era boundary") — recon's one declared measurement gap; §3 decides.
- File surface (complete, recon R2): tracked `project.godot` (306 B, targets nonexistent `main.tscn` + deleted Presentation), `icon.svg`, `icon.svg.import`; gitignored 11× `assets/kenney/*.import`; `.godot/`/`addons/` absent. `sample.dfscene` is the project's OWN engine-neutral JSON format, loaded by a live test — **KEEP, not Godot**.
- HISTORICAL KEEP set ≈ 573 sites (9 src + ~564 docs/tools); quarantine of `historical/{GODOT_INTEGRATION,VISUAL_ENGINE}.md` intact; `project.godot` doc-narration (31/11) is the record of the (now-closing) F-5 decision.

## §3 Phase 0 (orchestrator, serial)

1. ▲ Verify §2 base state; any post-`187c46c` commit classified governance-only or **H1**.
2. **Baseline gates**: full managed + native build + full test run (commands per DEVELOPMENT_HYGIENE v2.0.0 §3). Record the baseline (expected ≈1034/1036 with the two F-10 pre-existing failures + the known zombie-testhost operational note — NOT a halt). Closure must match-or-improve (**H2** on regression). Comment-only and file-deletion commits especially must not move test results.
3. **Resolve the `PIPELINE_METRICS.md` lifecycle gap** (recon's declared gap): read its REGISTER entry. If lifecycle = Live/LOCKED living-spec → its 5 Godot refs are DEAD, fix in D2 (era-boundary stack claim → current Vulkan/.NET truth). If lifecycle = snapshot/era-record (EXECUTED/Live-chronicle) → they are HISTORICAL, LEAVE them, and record the determination in the closure report. No autonomous amend without this determination.
4. REGISTER enum vocabulary read (Lesson #N14) — empirical shapes for D5.
5. Mandatory reads: the GODOT ERADICATION RECON report (full), the 7 D1 files, the D2 docs, `assets/scenes/README.md`, DEVELOPMENT_HYGIENE v2.0.0 §7 (the Godot-status record this cascade makes obsolete), RESERVED_SURFACE_MUTABILITY v1.0, CODING_STANDARDS v2.0.0 §5/§6/§8, TESTING_STRATEGY v2.0.0 §4.2.
6. Validate-gate protocol as established (VALIDATION_REPORT.md folds into the running commit). `-Sync` forbidden. No pushes.

## §4 Topology

Light cascade — **no parallel writer wave needed** (the surface is small, sequential, and the deletion class wants a single careful hand). Orchestrator executes serially. Optional: one read-only verification agent re-confirms the 7 D1 sites + file-artifact inertness against the recon before edits (cheap insurance; not required).

Hard rules: only the orchestrator stages/commits; ROADMAP single-writer; no agent touches any other tree. Comment edits and doc edits are committed by class (§6), each behind its gate.

### §4.2 reference — hard/soft census pins
Per TESTING_STRATEGY v2.0.0 §4.2: HARD pins (syntax-anchored: `[ReservedStub]` 34/13, `#pragma disable DFK/DFL` 0) must stay EXACT. SOFT pins (vocabulary: stub/deferred/TODO/etc. counts) are advisory baselines updated by census-delta record without ceremony. Godot eradication does not touch `[ReservedStub]` sites, but D1/D2 comment edits MAY move SOFT vocabulary counts (e.g. removing a comment containing "stub") — record any movement as a census-delta in the closure, do not treat it as a finding (this is exactly the F-25 lesson).

## §5 Execution specification

Truth law binds: no enforcement verbs without an on-disk enforcer; future capability only as ROADMAP pointers; citations per §6.1 form (no living-doc version pins, no URL anchors).

### A — D1 dead-code/comment cleanup (comment-only, zero code tokens)
- `GameContext.cs:8` — "GameRoot in production" → name the real prod caller (Launcher Program per recon); keep any genuinely-historical framing past-tense-correct.
- `GameContext.cs:16` — remove the dead "Reachable from DualFrontier.Presentation via InternalsVisibleTo" claim; if a current reachability note is useful, state the real one (Launcher / the actual `InternalsVisibleTo` set). Doubly-dead claim dies.
- `GameBootstrapIntegrationTests.cs:23` — dangling `<see cref="DualFrontier.Presentation.Nodes.GameRoot"/>` → remove the cref or repoint to the live entry type (GameBootstrap/Launcher Program); a dangling cref is the cleanest DEAD case.
- `:34` and `ModMenuControllerTests.cs:33` — stale "Out of scope: Godot UI scene tests" notes → rewrite the scope note in current terms (Launcher UI) or remove if the exclusion is now meaningless.
- `:146` — "Godot launches with cwd = project root" → current launch truth (Launcher process cwd) or remove if the test no longer depends on it.
- `:288` — "GameHUD.SetTick on the Godot main thread" → current truth (`GameHUD` deleted; name the real tick path / render thread). The architect's exact archetype — this is the headline DEAD fix.
- Build gate after the commit (H2 insurance); comment-only ⇒ results must be identical.

### B — D2 living-doc de-Godot
- `assets/scenes/README.md` — :4 "readable by the Godot build" → engine-neutral truth (`.dfscene` is the project's own JSON, consumed by the Native/Vulkan path + the live serialization test); :10 "Preferred path: the Godot DevKit plugin (Tools → Export)" → current authoring truth or, if no replacement tool exists, a `Planned — see ROADMAP` pointer (do NOT invent a tool — truth law); :21 stale `docs/architecture/VISUAL_ENGINE.md` → `…/historical/VISUAL_ENGINE.md` per §6.1 form. Then re-sync `DOC-F-ASSETS-SCENES` frontmatter (it is a registered Live doc — version touch per its convention).
- `MAXIMUM_ENGINEERING_REFACTOR.md:153` — "integration with Godot/.NET game runtime untested" → reframe to the current runtime (Native/Vulkan + Launcher); if the line is itself an era-snapshot inside an EXECUTED doc, leave + note (check its lifecycle the same way as §3).
- `ROADMAP.md:98` — "Phase 5 introduces them as real `Godot.Control`/`Godot.Node2D` subclasses" → the real Phase 5 UI plan (Launcher/Vulkan UI), or mark the line obsolete; a forward plan naming Godot is unambiguously DEAD.
- `ROADMAP.md:166` — closed-phase record "Godot works as both editor and temporary runtime" → historicize honestly ("Phase 3.5 used Godot as a temporary runtime; retired at cascade #2") — this is a record, so past-tense it rather than deleting (preserves the closed-phase truth without the present-tense falsehood).
- `PIPELINE_METRICS.md` — per §3 determination only.

### C — D3 file-artifact deletion (F-5 authorized)
`git rm` the tracked set: `project.godot`, `icon.svg`, `icon.svg.import`. Remove the 11 gitignored `assets/kenney/*.import` from disk (they are untracked — delete from working tree; note they were never in git, so this is a filesystem delete, recorded in the commit body as a manual step, not a `git rm`). Edit `.gitignore` — remove the now-pointless Godot block (lines 1–3,9: `.godot/`, `.import/`, `*.import`) since no Godot tooling will regenerate them; edit `SCOPE_EXCLUSIONS.yaml` — remove the `.godot/**` patterns. Verify post-deletion: build still green (the linchpin — proves inertness empirically), `git ls-files | rg -i 'godot|\.import'` → only the surviving comment in Directory.Packages.props (which STAYS — it is HISTORICAL record of the removal) and zero file artifacts.

### D — D4 F-5 resolution
ROADMAP F-5 (S3 OPEN → **CLOSED**, this cascade) with the resolution note: deletion authorized by Crystalka 2026-06-29, executed in commit C-(deletion), engine already removed from Skarlet, all artifacts proven inert (zero build/tooling/package reference). Record the scope-closure: the original F-5 scope (`project.godot` + `assets/**/*.import`) is extended at closure to include root `icon.svg`/`icon.svg.import` + the `.gitignore`/`SCOPE_EXCLUSIONS` Godot config — the full inert surface closes under this one decision. Update DEVELOPMENT_HYGIENE §7 accordingly (its "do not delete until F-5 closes" instruction becomes "Godot fully eradicated at the Godot Eradication Cascade" — the §7 Godot-status record flips from "tails remain, gated by F-5" to "complete").

## §6 Commit plan (intended form; deviations per mutability license, recorded)

| # | Subject | Content |
|---|---|---|
| C1 | `governance(godot): enroll Godot Eradication brief + validation checkpoint` | brief + VALIDATION_REPORT |
| C2 | `src(comments): eradicate dead Godot-host references` | D1 (7 sites, comment-only; build gate) |
| C3 | `docs(godot): de-Godot living docs — assets README, ROADMAP, refactor track` | D2 (incl. PIPELINE_METRICS per §3 or its recorded exemption) |
| C4 | `chore(godot): delete inert Godot file artifacts (F-5 authorized)` | D3 (git rm + disk delete + .gitignore/SCOPE_EXCLUSIONS edit; build gate — inertness proof) |
| C5 | `docs(hygiene): DEVELOPMENT_HYGIENE §7 — Godot eradication complete` | §5-D DEV_HYGIENE flip |
| C6 | `governance(roadmap): close F-5 (S3) + scope-closure record` | D4 ROADMAP F-ledger |
| C7 | `governance(register): Godot Eradication REGISTER closure (2.17 → 2.18)` | D5 + validate folded |
| C8 | `governance(register): render regeneration + header backfill` | render + Option-B backfill |

Census-delta (if D1/D2 moved SOFT vocabulary pins) is recorded in C7's body or the closure report, per §4.2 — not a separate finding.

## §7 D5 — REGISTER cascade (C7)

Empirical shapes only (Phase 0.4). Enroll this brief (DOC-D → EXECUTED at closure). Document count: 273 → 272 if `DOC-F-ASSETS-SCENES` survives (it does — README stays, only its Godot lines change) so **count stays 273 + 1 brief = 274**; verify no registered doc is deleted (the deleted FILES are not registered docs — `project.godot`/`icon.svg` carry no register_id; confirm via `rg 'project.godot|icon.svg' REGISTER.yaml` shows only narration, not a DOC- entry). Bumps: `DOC-F-ASSETS-SCENES` PATCH, DEVELOPMENT_HYGIENE PATCH, ROADMAP Live-touch, any D2-touched living doc PATCH. EVT `EVT-GODOT_ERADICATION-CLOSURE` (40 → 41) with C1–C6 real hashes; no new PENDING-COMMIT except the header self-reference, backfilled at C8. register_version 2.17 → 2.18. Validate exit 0 (**H3**).

## §8 Halt conditions

H1 base-state mismatch beyond governance-only · H2 build/test regression vs baseline (comment + deletion commits included — the deletion's build-green is the inertness proof; regression here means an artifact was NOT inert → halt, do not force) · H3 validate nonzero · H4 a DEAD/HISTORICAL call recon left genuinely ambiguous that code-inspection cannot resolve (escalate; do not guess) · **H5 deleting or altering a HISTORICAL site** — the 9 `src/` comments, the reserved seams, the quarantined historical docs, or past-tense-correct chronicle records are OUT of scope; touching them is the inverse error · H6 any registered doc would be deleted (only inert non-registered FILES are deletion targets) · H7 non-orchestrator ROADMAP write · no pushes, no `-Sync`, no history rewrites. On halt: stop, report verbatim, await Crystalka.

## §9 Closure protocol & report

METHODOLOGY v1.13.0 session closure. Report (chat): commits table; the **`PIPELINE_METRICS` lifecycle determination** (DEAD-fixed vs HISTORICAL-exempt, with evidence); file-deletion manifest (14 files + 2 config blocks) with the post-deletion build-green inertness proof; comment/doc fix table (site | before-verdict | after); census re-runs (`\bgodot\b` in src → 9 HISTORICAL unchanged; in native → 0; `using Godot` → 0; tracked Godot file artifacts → 0; dangling `DualFrontier.Presentation`/`GameRoot` → 0); SOFT-pin census-delta if any; F-5 CLOSED record + scope-closure; gates table (baseline vs closure, 1034/1036-or-better); Skeleton revisions consolidated; self-attestation (no pushes; single render; HISTORICAL set untouched — the 9 src comments verified present; snapshots unedited; no registered doc deleted). Crystalka manual checklist: push; ratify lifecycle; remaining standing queue (F-4/F-9, F-7, F-10, F-12 — F-5 now CLOSED, F-11 branch list may add `claude/godot-removal-deliberation-Vfg2R` as a now-fully-superseded keeper).

## §10 Out of scope

KERNEL rewrites (F-4/F-9) · A'.9.1 Phase β (next cascade, now unblocked by clean code) · the 9 HISTORICAL src comments + reserved seams (KEEP) · `sample.dfscene` and its live test (KEEP — not Godot) · quarantined `historical/*` docs (KEEP) · the `Directory.Packages.props:37` removal-record comment (KEEP — HISTORICAL) · untracked `build/` dir (not Godot; leave) · doc_role schema / hybrid register (F-2/F-13 tooling cascade) · the two failing tests (F-10 — baseline) · branch pruning (F-11) · NIH · pushes.

---

*Authored 2026-06-29 from GODOT ERADICATION RECON (R1–R8). F-5 deletion authorized by Crystalka 2026-06-29. Ratification: Crystalka. Без костылей.*
