# BRIEF TEMPLATE -- the universal Dual Frontier execution-brief scaffold

This is the canonical scaffold for a Dual Frontier execution brief
(`DOC-D-<CASCADE_NAME>_BRIEF`). It is derived from the project's real briefs
(Standing-Law, Architecture-Truth, Godot Eradication) and carries the union of
their load-bearing sections so no later brief silently drops one.

A Dual Frontier brief is the SPECIFICATION an executor obeys, written to be
comprehensive and STANDALONE -- it carries the halt catalog, the Phase 0
preconditions, and the closure protocol inline (no separate runbook), for
context-window safety across a fresh executor session. It rests on a recon report
(author the recon kickoff first). The brief is authority; where it and the live
code differ, the code wins and the conflict is recorded; where it and a standing
doc differ, the brief is wrong -- halt.

## The CORE / KIND convention

Every section is tagged `[CORE]` or `[KIND: ...]`.

- `[CORE]` sections appear in EVERY brief. If one is genuinely empty for a cascade
  (no new S-LOCK, no new spec, no multi-agent topology), keep the header and write
  "none" with a one-line reason -- the explicit "none" is the proof the question
  was asked.
- `[KIND: ...]` sections appear only for the named cascade kinds. Delete a KIND
  section whose kind does not apply. The recognized kinds are: **phase-execution**,
  **doc-cascade**, **audit/recon**, **eradication/hygiene**, **governance**.

## How to instantiate

1. Copy the scaffold block below into the brief deliverable (delivered to the
   operator via `present_files`; not committed by you -- the executor enrolls it).
2. Replace the leading fenced `yaml` block with real `---`-delimited frontmatter.
3. Replace every `<...>` placeholder; delete every `> Guidance:` line once written.
4. Delete inapplicable `[KIND]` sections; keep every `[CORE]` section.
5. Finalize with `df-deliverable-hygiene` before presenting.

## Hygiene that applies to every brief

- **English deliverable.** The brief is English even though chat is Russian. The
  genuine Cyrillic invariant IDs (K-L bi-script reality, an open finding) may
  appear where the repo uses them; match both scripts in any census the brief
  specifies.
- **Cite standing law by anchor, never restate it** (brief-integration pattern).
- **PENDING-* is outlawed** (schema 2.0, G-SCHEMA): `last_modified_commit` is
  optional -- carry a real hash or omit the field; never a placeholder.
- **Atomic commits.** Each commit is one logical change with a structured body.
- **Register discipline (schema 2.0)**: frontmatter is the SoT; a governance
  mutation = frontmatter edit + `dotnet run --project
  tools/DualFrontier.Governance -- sync` in the same commit (the derived
  `REGISTER.yaml` + `CURRENT_AUTHORITY_SURFACE.yaml` fold in); `validate
  --armed` exit 0 gates every governance-touching commit; derived artifacts are
  never hand-edited; `AUDIT_TRAIL.yaml` is append-only.

---

## === BRIEF SCAFFOLD: copy from here ===

```yaml
register_id: DOC-D-<CASCADE_NAME>_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft (register path Draft -> EXECUTED at closure; the LOCKED ratification is a chat act -- armed G-CATLIFE forbids persisting tier-3 + LOCKED)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '<YYYY-MM-DD>'
content_language: en
authored_by: Claude Opus (deliberation session, <CASCADE_NAME> prep)
basis: <CASCADE_NAME> RECON REPORT <date> (R1-Rn)
```

# <CASCADE_NAME> -- Execution Brief

> Guidance: one or two paragraphs. State plainly what this cascade does and the
> single sentence that says "done". Name the executor (Claude Code, flagship model,
> LOCAL on the operator's machine) and the repository. Close with the
> brief-integration notice and the anti-pattern rule (a conflict between this brief
> and any standing doc means the brief is wrong -- halt and escalate).

## 1. Mission [CORE]

> Guidance: the deliverable milestone, then a deliverables table (D1, D2, ... |
> artifact | action | version). State why this cascade precedes whatever it
> unblocks, if anything.

## 2. Established facts [CORE]

> Guidance: the recon digest -- the measured facts this brief rests on, each
> traceable to the recon report. Mark with a re-verify symbol the facts the
> executor must re-confirm at Phase 0 (HEAD, clean tree, counts), halting on
> mismatch. Include the code-truths that anchor any rewrite and the census numbers
> the recon established as canonical (these govern over any stale figure).

## 3. Phase 0 -- preconditions and checkpoint [CORE]

> Guidance: run serially by the orchestrator before any agent spawns.

1. **Verify recon facts** (section 2 re-verify set). Any mismatch -> HALT H1.
2. **Baseline gates**: full managed + native build and full test run (commands per
   `DEVELOPMENT_HYGIENE`). Record the result as the regression anchor (state the
   expected pre-existing-failure shape so it is not mistaken for a halt). Closure
   must match-or-improve -> HALT H2 on regression.
3. **Merge / branch prep** (if applicable): state the exact sanctioned form (e.g.
   `git merge --ff-only <branch>`); a non-fast-forward -> HALT.
4. **Validation checkpoint** (schema 2.0):
   `dotnet run --project tools/DualFrontier.Governance -- validate --armed`.
   Exit code != 0 -> HALT H3. Governance test suite green.
5. **Frontmatter-shape read** (Lesson #N14, 2.0 edition): read the field law
   (FRAMEWORK section 14.3, sentinel forms 14.4) plus the LIVE frontmatter of one
   LOCKED document and, if the cascade appends a closure EVT, one existing
   `AUDIT_TRAIL.yaml` entry as the verbatim append template. These shapes are the
   only sanctioned templates for the REGISTER cascade.
6. **Mandatory reads**: the recon report (full), the target files, the standing law
   sections the brief cites, `METHODOLOGY` closure protocol. Confirm each was read
   before any wave spawns.

`sync` runs in EVERY frontmatter-touching commit (it is the derived-artifact
regeneration, not a ceremony); derived registers are never hand-edited;
`AUDIT_TRAIL.yaml` entries are never modified once appended. At the closure
boundary the executor pushes the WORK BRANCH to `origin` and opens a PR against
`main`; pushing `main` and merging its own PR are forbidden -- the merge is the
architect's act, and that is where ratification happens
(`CODING_STANDARDS.md` section 8.4, v3.0.0).

## 4. Topology [CORE]

> Guidance: serial (orchestrator only) for a small or deletion-class cascade, or
> the multi-agent wave shape for a larger one. The proven wave shape:
> Orchestrator -> Wave R (read-only survey agents, parallel) -> checkpoint -> Wave W
> (writer agents, parallel, DISJOINT files) -> serial closure. HARD RULES: only the
> orchestrator runs git add/commit (atomic discipline is incompatible with parallel
> committers); single-writer files (e.g. ROADMAP.md is orchestrator-only); no agent
> touches any out-of-scope or reference tree. If serial, state "single orchestrator,
> no wave" and why.

## 5. Wave R -- survey agents [KIND: phase-execution | doc-cascade with code survey]

> Guidance: each read-only agent returns a structured inventory on a fixed schema;
> the inventories are the code-truth substrate for Wave W (a writer may not state a
> claim absent from its inventory). One agent per survey front. Delete if the
> cascade needs no survey wave.

## 6. Checkpoints [CORE]

> Guidance: C-R (after Wave R): reconcile inventories against section 2; material
> contradiction beyond explained measurement-method deltas -> HALT H4; hand each
> writer its inventory slice. C-W (after Wave W): sample-audit every enforcement
> claim against the inventories (truth law); confirm zero roadmap load outside
> explicit ROADMAP pointers; confirm cross-citation integrity and citation-form
> compliance (no living-doc version pins, no URL anchors); confirm no standing-law
> stack residue. Violations -> return to the owning writer once; unresolvable
> without an architectural decision -> HALT H6. If serial, state the single
> truth-law self-audit the orchestrator runs.

## 7. Execution / writer specifications [CORE]

> Guidance: one sub-section per deliverable (or per writer). Global laws restated by
> reference: truth law (no enforcement verb without an on-disk enforcer; future
> capability only as `Planned -- see ROADMAP.md` pointers); citation-form (cite by
> anchor and stable ID, no version pins, no URL anchors); recon classifications are
> the work order, code is the truth (verify against code before writing). Each
> rewrite carries frontmatter per the REGISTER mirror shape and ends with an
> Amendment-protocol and a Change-history section.

## 8. <Kind-specific machinery> [KIND: governance | audit/recon | eradication/hygiene]

> Guidance: the load-bearing machinery for the kind. Examples:
> - governance (standing/LOCKED doc change): give EXACT replacement text; embed any
>   wholly new document in Appendix A so it is judged as final words. Ground the
>   target's CURRENT live version before computing a bump (never assume from
>   memory).
> - audit/recon: the read-only design, the report schema, the held-out oracle.
> - eradication/hygiene: the **symmetric preservation halt** (deleting a HISTORICAL
>   record is the inverse error and is itself halt-class -- name the KEEP set
>   explicitly) and the **build-green-as-inertness-proof** gate (if removing an
>   artifact does not break the green build, inertness is proven empirically; a
>   regression means the artifact was NOT inert -> halt, do not force).

## 9. S-LOCK invariants [CORE]

> Guidance: the structural locks this cascade adds or preserves, each enforced
> STRUCTURALLY (an analyzer rule, a meta-test, a build property), not by
> convention. A durable invariant a fix establishes becomes an S-LOCK here and a
> rule candidate for the analyzer. If none, write "none" and why.

## 10. Census discipline [CORE -- when reserved surface or markers are touched]

> Guidance: name the census method verbatim (the exact rg/grep expression).
> Distinguish HARD pins (syntax-anchored -- exact count is the invariant, e.g.
> `[ReservedStub` application sites, `#pragma disable DFK`) from SOFT pins
> (vocabulary counts -- advisory baselines updated by a census-delta record without
> ceremony). State in advance which edits may move SOFT pins so the movement is
> recorded as a delta, not raised as a finding. The meta-test asserts the EXACT
> pin (exactness, not monotonicity -- reserved surface may legitimately grow). If
> not applicable, write "none".

## 11. Commit plan [CORE]

> Guidance: one row per atomic commit in dependency order, each passing the gates.
> Use the cascade's scope-prefixed subjects. Commit count is intended-form -- a
> writer-defect iteration or a needed split may add a commit; record the deviation
> in the closure report, do not compress history to match the table.

| #  | Subject                                                              | Content |
| -- | -------------------------------------------------------------------- | ------- |
| C1 | `governance(<scope>): enroll <CASCADE_NAME> brief + review/recon report` | brief + report frontmatter-enrolled + sync + validate --armed |
| C2 | `<type>(<scope>): <subject>`                                         | <...>   |
| .. | <...>                                                                | <...>   |
| Cn | `governance(closure): <CASCADE_NAME> EVT append + ROADMAP write-back` | AUDIT_TRAIL.yaml append + frontmatter flips (brief -> EXECUTED) + sync + validate --armed |

## 12. REGISTER cascade [CORE]

> Guidance: schema-2.0 discipline using ONLY the Phase 0 verbatim shapes.
> Enrollment = frontmatter in the document itself + `sync` (this brief enrolls
> D/3/Draft at C1 and flips to EXECUTED in its own frontmatter at closure).
> State the version bumps per deliverable and the closure EVT appended to
> `AUDIT_TRAIL.yaml` with the real hashes of the prior commits.
> `validate --armed` exit 0 mandatory (HALT H3) -- fix only within the
> FRAMEWORK 14.3/14.4 closed vocabularies, never invent a field, enum, or
> sentinel form (HALT H5).

## 13. Halt conditions (H-series) [CORE]

> Guidance: the conditions under which the executor stops and surfaces rather than
> improvising. Always include the standing rails. Examples:

- **H1** precondition mismatch (section 3.1).
- **H2** build/test regression vs the Phase 0 baseline.
- **H3** validate nonzero.
- **H4** Wave-R / recon material contradiction.
- **H5** a REGISTER enum value is needed that the Phase 0 vocabulary lacks --
  escalate, never invent.
- **H6** truth-law unsatisfiable without an architectural decision.
- **H(kind)** for eradication: deleting or altering a HISTORICAL site (the inverse
  error). For governance: any semantic change to a LOCKED doc beyond the ratified
  scope.
- Standing rails: push law `CODING_STANDARDS.md` section 8.4 (v3.0.0) -- the work
  branch is pushed and a PR opened at the closure boundary; pushing `main` and
  merging one's own PR are forbidden; derived registers never hand-edited;
  `AUDIT_TRAIL.yaml` append-only (a recorded EVT is immutable); no history
  rewrite / force-push / squash (atomicity is settled BEFORE the push, section
  8.3); single-writer files honored; `historical/` read-only.

On halt: stop, report state verbatim, await the operator. In-session
re-confirmation before resuming is expected behavior (the auto-mode push-to-main
block is expected, not a fault).

## 14. Closure protocol and report [CORE]

> Guidance: execute the `METHODOLOGY` session closure protocol -- (a) tracker
> write-back; (b) REGISTER + validate folded; (c) render + backfill; (d) findings
> -> F-ledger entries (never chat-only); (e) the closure report. The report
> (chat) carries: the commits table (hash | subject); the versions table
> (before -> after, including the derived register state); the census pins
> recorded (HARD exact, SOFT deltas); the F-ledger final-state table; the
> consolidated `Skeleton revisions` list (every deviation from this brief's
> intended forms); the gates table (baseline vs closure -- must match-or-better);
> self-attestation (no pushes; `sync` run in every frontmatter-touching commit;
> single AUDIT_TRAIL append, prior entries byte-unchanged; no history rewrites;
> HISTORICAL / reference trees untouched); and the operator manual checklist
> (push; the standing F-queue items that remain operator-owned).

## 15. Out of scope [CORE]

> Guidance: what is explicitly excluded, named so the executor does not wander into
> it -- adjacent cascades, architect-owned findings, the reference tree, pushes,
> snapshots / EXECUTED-doc content beyond any sanctioned banner.

## Appendix A -- <embedded document> [KIND: governance with a new/amended doc]

> Guidance: for a governance cascade that creates or wholly replaces a standing
> document, embed the exact final content here between explicit BEGIN / END markers
> so the words that land are judged directly. Delete if none.

---

**End of <CASCADE_NAME>_BRIEF.md v1.0**

## === BRIEF SCAFFOLD: copy to here ===

---

**End of BRIEF_TEMPLATE.md**
