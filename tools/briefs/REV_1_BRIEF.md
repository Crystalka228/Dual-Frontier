---
register_id: DOC-D-REV_1_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-11-21'
last_modified: '2026-11-21'
content_language: en
next_review_due: null
title: 'REV_1 -- the operational-truth law says how the project works now: CODING_STANDARDS 4.0.0 (comment law, debt markers, merge policy, scopes, roles, CI truth), DEVELOPMENT_HYGIENE 3.0.0 (Linux, the .NET governance tool, merge commits), TESTING_STRATEGY 2.3.0 (Linux invocation, live pins, failability), PIPELINE_METRICS 0.2.3 (era annotations) -- the second cascade of CORPUS_REVISION_1'
authored_by: Claude Opus (architect-orchestrator session, CORPUS_REVISION_1)
basis: 'REV_1 recon pair (/mnt/Work/Temporal/rev1-run/recon/R1.md -- Sonnet, DEVELOPMENT_HYGIENE/TESTING_STRATEGY/PIPELINE_METRICS; R2.md -- Opus, CODING_STANDARDS; read-only at 54a9577) + operator rulings L1-L8 ratified 2026-11-21 with their canonical wording K1-K10 (/mnt/Work/Temporal/rev1-run/CONTRACT.md) + the revision synthesis (/mnt/Work/Temporal/corpus-revision-1/REVISION_SYNTHESIS.md) + the current operating model (/mnt/Work/Temporal/skills-revision/OPERATING_MODEL_2026-11-21.md)'
---

# REV_1 -- Execution Brief

Four LOCKED tier-1 methodology documents are loaded as authority by every session
(they sit on `CURRENT_AUTHORITY_SURFACE.yaml`) and still describe a Windows,
PowerShell, single-executor project: drive-letter cmake paths, retired `sync_register.ps1`,
"CI: none", fast-forward-only merges, "the architect merges", comments that must cite
ledger rows and lessons, stale census tables. This cascade rewrites them to the
present truth and to the operator's 2026-11-21 rulings. Done = the four documents at
their new versions, every statement the recon pair listed either replaced or
recorded as deliberately kept, the rulings encoded once (CODING_STANDARDS) and pointed
at elsewhere, F-94 seeded, one closure EVT, gates green.

Executed by the architect-orchestrator through subagents (integrator commits on
`/mnt/Work/_wt/df-rev-1`, branch `claude/rev-1`; units in their own worktrees),
repository `/mnt/Work/Colony_Simulator`. Brief-integration notice: commit law per
`CODING_STANDARDS.md` section 8 (as it stands at the dispatch commit for this
cascade's own commits), push law section 8.4, closure per `METHODOLOGY.md`. A
conflict with a standing document that this cascade does NOT amend means THE BRIEF IS
WRONG -- halt. A conflict with live code means THE CODE IS THE TRUTH -- record it.

## 1. Mission [CORE]

| D | Artifact | Version | Owner |
| - | -------- | ------- | ----- |
| D1 | this brief | enroll D/3/Draft | integrator (C1) |
| D2 | `docs/methodology/CODING_STANDARDS.md` | 3.0.0 -> 4.0.0 (MAJOR) | U-CS |
| D3 | `docs/methodology/DEVELOPMENT_HYGIENE.md` | 2.2.0 -> 3.0.0 (MAJOR) | U-DH |
| D4 | `docs/methodology/TESTING_STRATEGY.md` | 2.2.3 -> 2.3.0 (MINOR) | U-TS |
| D5 | `docs/methodology/PIPELINE_METRICS.md` + `docs/methodology/BRIEF_TEMPLATE.md` (the ff-only line only) | 0.2.2 -> 0.2.3 (PATCH); template unversioned | U-PM |
| D6 | `docs/ROADMAP.md` (seed F-94), `docs/governance/AUDIT_TRAIL.yaml`, this brief | closure | integrator (C6) |

The classes follow each document's own versioning rule, measured by the recon (R1
section 1, R2 section A). Why now: every later revision cascade and every future
brief cites these four documents.

## 2. Established facts [CORE]

(RV) = re-verified at Phase 0; mismatch -> H1. Anchors at HEAD `54a9577`.

1. (RV) main = `54a9577` (merge of PR #57). Build 0W/0E; suite 1441 / 0 / 6 (the six
   named skips: SchedulerExtremeTests S2/S3/S4/S5a/S5b and
   RuntimeCompositionTests.Create_with_validation_enabled_composes_validation_layer);
   selftest ALL PASSED; dev smoke exit 124; `validate --armed` exit 0 (architect,
   2026-11-21, at the merge head).
2. (RV) Frontmatter at HEAD: CODING_STANDARDS B/1/LOCKED 3.0.0; DEVELOPMENT_HYGIENE
   B/1/LOCKED 2.2.0; TESTING_STRATEGY B/1/LOCKED 2.2.3; PIPELINE_METRICS B/1/LOCKED
   0.2.2. BRIEF_TEMPLATE is tracked and register-EXCLUDED
   (`tools/governance/SCOPE_EXCLUSIONS.yaml`).
3. (RV) Live census pins (`tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs`):
   CONTRACT K9. Documented tables disagree: TESTING_STRATEGY 4.2 deferred 87/54;
   CODING_STANDARDS 5.2 deferred 82/51 and TODO 136/53; CODING_STANDARDS 9 waiver pin
   2 (live 3), "17 rules" (live 18).
4. (RV) F-93 is the highest ledger id; F-94 is free.
5. The recon pair carries, per document, every stale statement with path:line, the
   evidence, and a PROPOSED replacement text: R1 sections 2-4 (DEVELOPMENT_HYGIENE,
   TESTING_STRATEGY, PIPELINE_METRICS), R2 section B (CODING_STANDARDS). Those
   replacement texts are the INTENDED FORMS (mutable surface: a unit may reword for
   flow and accuracy, recording a Skeleton revision); the CONTRACT K-sentences are NOT
   mutable -- where a recon text and a K-sentence differ, the K-sentence wins.
6. Section numbers stay stable in all four documents (csproj, tests and LOCKED
   architecture documents cite them: R1 cross-reference tables, R2 section D). New
   material goes into NEW subsections (CODING_STANDARDS 6.2 comment law, 6.3
   touched-file rule), never by renumbering.
7. This cascade edits NO file under `src/`, `tests/`, `mods/`, `native/`: no census
   pin moves (R2 item 41).

## 3. Phase 0 -- preconditions and checkpoint [CORE]

1. Verify section 2 (RV) facts; mismatch -> H1.
2. Native present (copied, never rebuilt); selftest ALL PASSED.
3. Baseline: build 0W/0E; full suite 1441 / 0 / 6 with the named skips (never pipe
   `dotnet test`); one dev smoke exit 124 clean.
4. `validate --armed` exit 0 (the brief itself carried untracked must add no error).
5. Frontmatter-shape read: FRAMEWORK section 14.3/14.4; each target's live
   frontmatter and change-history format (R1 section 1 "Conventions"); one
   `AUDIT_TRAIL.yaml` closure EVT as the append template (EVT-2026-11-21-REV_0).
6. Mandatory reads: this brief; CONTRACT.md; the recon section for the unit's document.

`sync` in every frontmatter-touching commit; derived registers never hand-edited;
`AUDIT_TRAIL.yaml` append-only. Closure: push the WORK branch once, open the PR.

## 4. Topology [CORE]

Integrator (opus/high, the only committer) + four units in parallel with DISJOINT
file sets, each one document: U-CS (opus/high), U-DH (opus/high), U-TS (opus/high),
U-PM (sonnet/medium). Every line a law unit writes is record, hence opus. Seams the
integrator owns: `docs/ROADMAP.md`, `AUDIT_TRAIL.yaml`, this brief, derived registers.
Cross-document coherence is carried by CONTRACT.md, not by units reading each other's
drafts.

## 5. Wave R -- survey agents

None: the recon pair at 54a9577 is the survey; Phase 0 re-verifies its anchors.

## 6. Checkpoints [CORE]

- **C-green** (after C1): validate exit 0; suite 1441 / 0 / 6.
- **C-law** (after C2-C5): validate exit 0; suite 1441 / 0 / 6 (no code moved);
  build 0W/0E; dev smoke exit 124; for each document, every recon row has an outcome
  (replaced, or kept with a reason) in the unit report; every K-sentence the document
  states appears verbatim; no document but CODING_STANDARDS restates the comment law
  (the others point at section 6.2); `git grep -n -E 'sync_register|render_register|ff-only|fast-forward-only'`
  plus a search for drive-letter paths (an uppercase letter, a colon, a backslash) over the four
  documents returns only deliberate historical mentions inside change
  history / dated records, each listed in the unit report.

## 7. Execution / writer specifications [CORE]

Every edit encodes a ratified ruling (CONTRACT) or replaces a statement the tree
disproves (recon). A needed NEW semantics is H6.

**7.1 C1 (integrator seam).** Enroll this brief (frontmatter as written). sync;
validate exit 0.

**7.2 U-CS (C2) -- CODING_STANDARDS 4.0.0.** File set: `docs/methodology/CODING_STANDARDS.md`.
Execute R2 section B WHOLE, with CONTRACT K1-K9 verbatim where the document states
those rules. Must include: new section 6.2 (the comment law, K3, adapted from the
Community Hub text R2 item 28 quotes, including the "what a case cannot distinguish"
clause and the string-literal reach for test names) and new section 6.3 (K4);
section 6 / 6.1 trimmed so they no longer REQUIRE F-#, Q-L-# or Lesson #N citations
(the 6.1 citation FORM stays); section 5.2 rewritten around K5/K6 with the live pins
(K9) and the deferral rule replaced; section 5.3 waiver form: authority = law, F-# only
as `retires with F-<n>` (K5); section 3.1's "verbatim" example replaced by a
law-compliant one; section 8.1 vocabulary + K7 and the false "100% in-vocabulary"
sentence replaced (R2 item 30); section 8.3 + K1; section 8.4 role naming per K2;
section 9 CI truth per K8, rule counts and waiver pin to live values; every Windows /
drive-letter / MSVC / `.exe` / PowerShell instruction replaced by the Linux form. Change
history entry 4.0.0 in the document's own format; frontmatter `version`,
`last_modified`, `last_review_date`/`last_review_event`; drop a stale
`last_modified_commit` rather than invent one.

**7.3 U-DH (C3) -- DEVELOPMENT_HYGIENE 3.0.0.** File set:
`docs/methodology/DEVELOPMENT_HYGIENE.md`. Execute R1 section 2 WHOLE (sections 1-6
and 8 as the recon lists): the real project set (R1: 57 projects, all in the sln --
re-count with the stated command), Linux-only build section with the canonical native
commands, the .NET governance tool replacing the retired PowerShell suite, section 5
merge policy per K1 and roles per K2, the local acceptance chain per K8. Points at
CODING_STANDARDS 6.2 for comments (never restates). Change history 3.0.0 entry.

**7.4 U-TS (C4) -- TESTING_STRATEGY 2.3.0.** File set:
`docs/methodology/TESTING_STRATEGY.md`. Execute R1 section 3 WHOLE: section 8 Linux
invocation (`dotnet test` redirected to a file, never piped, `MSBUILDDISABLENODEREUSE=1`,
TRX as the verdict), selftest path, "no CI" -> K8, the census table 4.2 to K9 with
the meta-test named as carrier (PATCH-class refresh inside the MINOR), the
host-capability skip class (the validation-layer skip), the deleted exemplar
replaced by a live test, the failability convention per K10 as an ADDITIVE
convention (not a section 7 closure obligation -- that would be MAJOR and is out of
scope). Change history 2.3.0 entry.

**7.5 U-PM (C5) -- PIPELINE_METRICS 0.2.3 + the template line.** File set:
`docs/methodology/PIPELINE_METRICS.md`, `docs/methodology/BRIEF_TEMPLATE.md`. Execute
R1 section 4 WHOLE (annotation-only under the document's era charter: Godot and .NET 8
marked as era survivals, the status line, broken links, one orchestrated-era paragraph
pointing forward -- no new section). In BRIEF_TEMPLATE replace ONLY the ff-only Phase
0 line (R1 X-03) with the K1 merge form; nothing else in the template moves (it is
rewritten later in the series).

**7.6 C6 (integrator seam, closure).** Seed **F-94** in the ledger: the comment law's
provenance half and the debt-marker form have no machine gate -- candidate: a Roslyn
trivia rule over `src/` C# comments (and test-name literals) that refuses the
forbidden families and ADMITS law-in-force citations, landing with a negative fixture,
plus an exact census of `TODO(F-` sites with each id resolving to an OPEN row (R2
section E). S3, OPEN, owner architect; seeded, not built. Copy the row form of F-93.
One closure EVT (real hashes; records the operator's chat ratification of this brief
and of rulings L1-L8 on 2026-11-21; lists the four version bumps); brief -> EXECUTED;
sync; validate exit 0.

## 8. Kind-specific machinery [KIND: doc-cascade]

LOCKED tier-1 B documents are amended in place (no lifecycle transition; precedent:
CODING_STANDARDS 3.0.0, DEVELOPMENT_HYGIENE 2.2.0 -- R1 section 1). The 6.1 citation
form binds the new prose: no version pins of living documents, no URL-fragment
anchors. ASCII in new prose ("section N"); existing `§N --` headings keep their form.

## 9. S-LOCK invariants [CORE]

None touched: no К-L, PA-00#, Q-lock or S-LOCK text changes. CODING_STANDARDS edits
are rules about how code is written, not architecture.

## 10. Census discipline [CORE]

No `src/` file is edited, so no pin moves (section 2.7). New prose in the four
documents may use the marker words as words about the census (outside `src/` they are
not counted); no unit edits `CensusMetaTests.cs`.

## 11. Commit plan [CORE]

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(enroll): REV_1 brief enrolled` | 7.1 |
| C2 | `docs(coding-standards): 4.0.0 -- comments carry law, not history; debt names its ledger row; merges are merge commits` | 7.2 + sync |
| C3 | `docs(hygiene): 3.0.0 -- the Linux box, the .NET governance tool and merge commits are the operational truth` | 7.3 + sync |
| C4 | `docs(testing): 2.3.0 -- Linux invocation, live pins, and every new gate is watched red once` | 7.4 + sync |
| C5 | `docs(metrics): 0.2.3 -- era annotations; the brief template stops requiring ff-only` | 7.5 + sync |
| C6 | `governance(closure): REV_1 EVT -- the operational law is current; F-94 seeded` | 7.6 + brief EXECUTED + sync |

## 12. REGISTER cascade [CORE]

C1 enrolls one document; C2-C5 bump four frontmatters (sync folds the derived
registers in the same commit); C6 flips the brief and appends ONE EVT.
`validate --armed` exit 0 at every commit.

## 13. Halt conditions (H-series) [CORE]

H1 Phase 0 mismatch. H2 gate regression. H3 validate nonzero. H5 register vocabulary
gap. H6 a change needs NEW semantics beyond the rulings and the recon (for example a
rule the operator did not rule on: the recon's open items "rebase-merge" and "build
scope" ARE ruled -- K1, K7). H7 any file outside the four documents, the template, the
ledger, AUDIT_TRAIL and the brief. Standing rails: push law; derived registers never
hand-edited; AUDIT_TRAIL append-only; no history rewrite; never pipe `dotnet test`;
units never commit. On halt: stop, report verbatim, await the orchestrator.

## 14. Closure protocol and report [CORE]

Commits table; versions table (before -> after, class, rule cited); gates table
baseline vs closure; per document the outcome table (every recon row: replaced /
kept-with-reason); the K-sentence presence check; the grep of 6 C-law; ledger
final-state (F-94); friction table; self-attestation (8.4, sync every governance
commit, single EVT, no rewrite, no code touched). Push the work branch once, open the
PR. The operator then re-points the df-* skills' flux blocks (architect act, outside
the repo).

## 15. Out of scope [CORE]

FRAMEWORK, METHODOLOGY, RISKS.yaml, ANALYZER_RULES, the templates beyond the one line,
the ledger split, the architecture re-anchoring, the comment backlog (touched-file rule
only), building the F-94 gate, any code.

---

**End of REV_1_BRIEF.md v1.0**
