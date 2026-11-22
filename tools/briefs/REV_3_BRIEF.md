---
register_id: DOC-D-REV_3_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-11-22'
last_modified: '2026-11-22'
content_language: en
next_review_due: null
title: 'REV_3 -- the F-ledger leaves ROADMAP: docs/LEDGER.md holds the live rows and docs/LEDGER_ARCHIVE.md the closed ones, byte-identical; one legend (S4 latent joins it); G-ROWFORM guards both files with the live vocabulary armed; every live pointer re-pointed -- the fourth cascade of CORPUS_REVISION_1'
authored_by: Claude Opus (architect-orchestrator session, CORPUS_REVISION_1)
last_review_event: 'REV_3 EXECUTED 2026-11-22 (EVT-2026-11-22-REV_3) on claude/rev-3, C1-C4, four review-repair commits and the C5 closure. Operator-ratified input: the F-ledger leaves ROADMAP into docs/LEDGER.md and docs/LEDGER_ARCHIVE.md, both at once; the architect rulings B-1..B-10 (B-10 added at the review of the repairs) are PENDING OPERATOR REVIEW at the PR. Lifecycle Draft -> EXECUTED. Wording note: the brief says B-1..B-9 and calls the severity monitors 14; the S4 rows are 13 and the 14th monitor is F-54 W2.'
basis: 'operator ruling 2026-11-21 (LEDGER + LEDGER_ARCHIVE, both at once) + the Community Hub precedent LEDGER_SPLIT_1 (CH docs/LEDGER.md, docs/LEDGER_ARCHIVE.md headers) + architect rulings B-1..B-9 (/mnt/Work/Temporal/rev3-run/RULINGS.md, pending operator review) + the ledger census R-D (/mnt/Work/Temporal/corpus-revision-1/R-D.md) + the REV_2 G-ROWFORM gate (tools/DualFrontier.Governance/LedgerRowForm.cs, tools/governance/LEDGER_FORM.yaml)'
---

# REV_3 -- Execution Brief

The F-ledger -- 95 rows, about a third of ROADMAP's bytes -- moves out of ROADMAP into
its own two documents, the way Community Hub did it: live rows in `docs/LEDGER.md`,
closed rows in `docs/LEDGER_ARCHIVE.md`, each row byte-identical. The legend is declared
once and gains `S4` (latent); the one non-legend live state is normalized; G-ROWFORM
guards both files with the live vocabulary armed; every live pointer to "the ROADMAP
ledger" is re-pointed. Done = the two files enrolled C/2/Live, ROADMAP carries a pointer
section, `validate --armed` exit 0 with G-ROWFORM findings 0 on the live file, gates
green, one closure EVT.

Executed by the architect-orchestrator through Sonnet subagents (ruling B-9), integrator
on `/mnt/Work/_wt/df-rev-3`, branch `claude/rev-3`. Commit law CODING_STANDARDS 8; per-
commit duty by tier DEVELOPMENT_HYGIENE 3; tests TESTING_STRATEGY 8. Conflict with a
standing document this cascade does not amend -> halt; with live code -> code wins.

## 1. Mission [CORE]

| D | Artifact | Owner |
| - | -------- | ----- |
| D1 | this brief | integrator C1 |
| D2 | `docs/LEDGER.md` (new, C/2/Live), `docs/LEDGER_ARCHIVE.md` (new, C/2/Live), `docs/ROADMAP.md` (ledger section -> pointer) | U-SPLIT -> C2 |
| D3 | `tools/governance/LEDGER_FORM.yaml` | integrator seam (C2 path, C3 two-file form) |
| D4 | `tools/DualFrontier.Governance/**`, `tests/DualFrontier.Governance.Tests/**`, `tools/governance/MODULE.md` | U-TOOL -> C3 |
| D5 | pointer re-points in the live documents of section 2.4 | U-PTR -> C4 |
| D6 | AUDIT_TRAIL EVT, brief | integrator C5 |

## 2. Established facts [CORE]

(RV) re-verified at Phase 0. Anchors at main `8032e01`.

1. (RV) Gates at `8032e01`: build 0W/0E; suite 1494/0/6, 10 TRX; selftest ALL PASSED;
   smoke 124; `validate --armed` exit 0 (G-ROWFORM: 0 structural, 15 vocabulary monitors
   -- 14 severity, 1 state).
2. (RV) The ledger table is in `docs/ROADMAP.md` under "Findings ledger (F-series)":
   legend paragraph, header `| F-# | Finding | Severity | State | Resolution / owner |`,
   separator, 95 rows F-1..F-95 (re-count). R-D's census: OPEN 55 at the time plus rows
   seeded since; CLOSED variants (bold, dated, glyph); F-65 "Planned".
3. (RV) The REV_2 gate reads ONE path from `tools/governance/LEDGER_FORM.yaml`
   (`ledger_path`), inert when the file is absent.
4. (RV) Live documents pointing at the ROADMAP ledger (architect grep at 8032e01):
   `docs/governance/FRAMEWORK.md`, `docs/methodology/{CODING_STANDARDS,DEVELOPMENT_HYGIENE,METHODOLOGY,RESERVED_SURFACE_MUTABILITY,TESTING_STRATEGY}.md`,
   `docs/methodology/{BRIEF_TEMPLATE,RECON_KICKOFF_TEMPLATE}.md` (register-excluded),
   `docs/MIGRATION_PROGRESS.md`, `docs/architecture/{ECS,IDENTITY_AND_ABI_CONTRACT,MOD_OS_ARCHITECTURE}.md`.
   U-PTR re-greps (`findings ledger`, `F-ledger`, `ROADMAP` near an `F-<n>`) and adds any
   live document the grep finds; frozen records are excluded (ruling B-7).
5. Rulings B-1..B-9 (`/mnt/Work/Temporal/rev3-run/RULINGS.md`) are binding; the CH files
   `/mnt/Work/Start-up_Projects/Community_Hub/docs/LEDGER.md` and `LEDGER_ARCHIVE.md` are
   the model for the two headers (read-only, another project).

## 3. Phase 0 [CORE]

Verify section 2 (RV); native copied (native/ unchanged); baseline per 2.1; shape read:
FRAMEWORK 14.3/14.4, the live `docs/ROADMAP.md` frontmatter (C/2/Live shape), EVT-2026-11-22-REV_2.

## 4. Topology [CORE]

Integrator (sonnet/high) the only committer; three units in parallel, disjoint sets,
sonnet/high: U-SPLIT (D2), U-TOOL (D4), U-PTR (D5). Seams: LEDGER_FORM.yaml,
AUDIT_TRAIL, the brief, derived registers.

## 5. Wave R

None (R-D + the architect grep; Phase 0 re-verifies).

## 6. Checkpoints [CORE]

- After C2: `validate --armed` exit 0 (the REV_2 single-path tool reads docs/LEDGER.md).
- Stage gate after C4 (full chain once): build, full suite (1494 + U-TOOL's named added
  cases), 10 TRX, validate exit 0 with G-ROWFORM structural 0 in both files and
  vocabulary 0 on the live file, smoke 124; the split proof (below) re-run on the tip.

## 7. Writer specifications [CORE]

**7.1 C1** enroll the brief.

**7.2 U-SPLIT (C2).** A python3 script kept OUTSIDE the tree
(`/mnt/Work/Temporal/rev3-run/split.py`) moves every ledger row byte-identically: rows
whose State's leading member (emphasis and a leading glyph stripped) is not CLOSED ->
LEDGER.md; CLOSED -> LEDGER_ARCHIVE.md; ascending id order in each. Then by hand: F-65's
state cell -> `OPEN (planned)` (B-3, the only edited row); the two headers modelled on
CH's (purpose, B-1 split rule, row-read-whole rule, row form, B-4 mint and closing rule,
the B-2 legend in LEDGER.md only, the archive's relocation-only rule); C/2/Live
frontmatter shaped on ROADMAP's (`register_id` DOC-C-LEDGER / DOC-C-LEDGER-ARCHIVE); the
ROADMAP ledger section replaced by the B-5 pointer section (ROADMAP's `last_modified`
bumped). PROOF (a second script, outputs in the report): the multiset of row lines in
LEDGER.md + LEDGER_ARCHIVE.md equals the old table's rows except F-65's one cell; no id
lost or duplicated; each file ascending. Do NOT run sync; do not touch LEDGER_FORM.yaml.

**7.3 C2 seam** (integrator, in the U-SPLIT landing): set `ledger_path:
"docs/LEDGER.md"` in LEDGER_FORM.yaml; sync; validate exit 0.

**7.4 U-TOOL (C3).** B-6: the config lists both files (live + archive) with a per-file
vocabulary severity (live Report, archive Monitor); structure armed in both; id unique
across both files and ascending within each; the live-ledger test reads both real files;
fixtures for every new rule; the OV-1 hole pinned (a detached row after the table is
located at its own line; a two-digit id is captured whole); each new rule watched red
once. MODULE.md names the two-file config. Report the exact LEDGER_FORM.yaml content for
the integrator (seam) and the added test count. Code tier: tool build + governance tests.

**7.5 U-PTR (C4).** Re-point every live pointer of 2.4 (and any the re-grep adds) from the
ROADMAP ledger to `docs/LEDGER.md` (live rows / the legend / the mint rule) or
`docs/LEDGER_ARCHIVE.md` (closed rows) as the sentence means; LOCKED documents take a
PATCH bump with a change-history row in their own format; a register-excluded template
is edited without frontmatter. Change nothing else. Report one line per pointer.

**7.6 C5 (closure).** ONE EVT `EVT-2026-11-22-REV_3` (the split, the legend, the rulings
B-1..B-9 marked PENDING OPERATOR REVIEW, real hashes); brief EXECUTED; sync; validate.

## 8. Kind-specific [KIND: governance]

Landing order C1, C2 (split + config path), C3 (tool + two-file config), C4 (pointers),
C5. The archive is checked from C3 on.

## 9. S-LOCK [CORE]

None touched.

## 10. Census [CORE]

No `src/` change; no pin moves.

## 11. Commit plan [CORE]

| # | Subject |
| - | ------- |
| C1 | `governance(enroll): REV_3 brief enrolled` |
| C2 | `governance(ledger): the F-ledger leaves ROADMAP -- live rows in LEDGER, closed rows in LEDGER_ARCHIVE, byte-identical` |
| C3 | `feat(governance): G-ROWFORM guards both ledger files; the live vocabulary is armed` |
| C4 | `docs(governance): every live pointer names the ledger's new home` |
| C5 | `governance(closure): REV_3 EVT -- the ledger has its own home` |

## 12. REGISTER cascade [CORE]

C2 enrolls two documents and bumps ROADMAP; C4 bumps the LOCKED documents it edits; C5
flips the brief and appends ONE EVT. sync in each; validate exit 0 at every landing.

## 13. Halts [CORE]

H1 Phase 0 mismatch; H2 gate regression; H3 validate nonzero; H6 a change needs semantics
beyond B-1..B-9 (e.g. rewriting a closed row, renumbering, a new state); H7 any
`src/`, `mods/`, `native/` file. Standing rails as REV_2.

## 14. Closure [CORE]

Commits; the split proof; row counts per file; legend before/after; G-ROWFORM before/after
(vocabulary monitors 15 -> 0 live, archive legacy listed); pointer table; gates; friction
table; push once; PR body lists B-1..B-9 for the operator's review.

## 15. Out of scope [CORE]

ROADMAP's own history archive (REV-6); METHODOLOGY's text beyond the pointer (REV-4);
code comments that cite ROADMAP (touched-file rule); building the F-94/F-95 gates.

---

**End of REV_3_BRIEF.md v1.0**
