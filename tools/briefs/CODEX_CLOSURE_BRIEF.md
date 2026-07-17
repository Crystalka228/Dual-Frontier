---
register_id: DOC-D-CODEX_CLOSURE_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: null
title: 'CODEX_CLOSURE -- Execution Brief (properly gates and closes the half-closed Codex closed-PR remediation cascade: FOLD the Skarlet-gate H2b census-delta, fix F-13 at root, retire the CX-14 no-op props, amend the report by APPEND, close the register 2.23 -> 2.24)'
last_modified_commit: eacf604
review_cadence: on-cascade-execution
last_review_date: 2026-07-15
last_review_event: 'CODEX_CLOSURE cascade executed 2026-07-15 by Claude Code (Opus 4.8, LOCAL Skarlet) per the operator-ratified brief (authored 2026-07-14; Windows-only per L7 + CX-01..CX-21 namespace). Phase-0 Skarlet gate (analyzers ON, all 10 suites): 1101 pass / 1 fail / 5 skip; the single H2b failure (CX-06 + CX-21 not-yet census-delta gap) operator-ruled FOLD, remediated at C2. Commits C1 eacf604 / C2 673f815 (FOLD, not-yet 8/7 -> 10/9) / C3 e025086 (F-13 render fix) / C4 247f88c (CX-14 props retirement) / C5 5967400 (report amendment 1.0 -> 1.1) / C6 ef286cf (F-13 CLOSED + F-32/F-33) / C7 register closure 2.23 -> 2.24 / C8 render + backfill.'
reviewer: Crystalka
risks_referenced: []
capa_entries_referenced: []
special_case_rationale: Category D Tier 3 -- the clean (non-forbidden) Category-D combo, per the design/decision-tier brief precedent (the K-series + F-10 + F-29 DOC-D Tier-3 briefs). Standalone execution brief carrying this cascade's authority; enrolled at the design tier per the D-brief convention. Entry id follows the brief's own frontmatter register_id; the brief FILE frontmatter is hand-authored, not synced.
---

---
register_id: DOC-D-CODEX_CLOSURE_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft (-> LOCKED on Crystalka ratification -> EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-14'
content_language: en
authored_by: Claude Opus (deliberation session, drift-program arc)
basis: CODEX_REVIEW_REMEDIATION_20260714 closure report + architect Phase-0 line-read verification 2026-07-14 + operator ratifications (Windows-only per L7; CX namespace)
---

# CODEX_CLOSURE -- Execution Brief

The Codex remediation cascade (21 closed-PR review findings, executed 2026-07-14 on a
Linux remote host, merged to `main` as a fast-forward to `61f08ef`) is HALF-CLOSED:
its code landed, but it was built with the Roslyn analyzers gated off (the host could
not load Roslyn 5.3), no native / stress / Win32 / Vulkan test ran there, and its
governance closure (render, sync, version bump) was honestly deferred to the
PowerShell tooling. Meanwhile the architect's Phase-0 line-read (2026-07-14) verified:
the F-13 render defect root cause is a one-line backtick-escape bug at
`tools/governance/render_register.ps1:105`; the Codex F14 native-artifact-copy fix is
a STRUCTURAL NO-OP (CMake emits unprefixed `DualFrontier.Core.Native.so`, the props
searches exclusively `lib`-prefixed names, every item `Exists`-gated -- silent
mismatch); and the closure report's outcome arithmetic does not close (23 outcome
slots for 21 findings). The operator has ratified: the product is WINDOWS-ONLY per
L7, and the Codex finding series is namespaced `CX-01`..`CX-21` to end the collision
with our F-ledger.

This cascade finishes the job: gate the ungated code on Skarlet FIRST, fix F-13 at
the root, retire the no-op props under the Windows-only ratification, correct the
report's arithmetic by amendment (never by history rewrite), land the open residuals
in the F-ledger under CX citations, and close the register at 2.24.

**Done** when: the full Skarlet gate (analyzers ON + all suites) has characterized
the Codex code and is green (or its findings are surfaced and ruled on); the
regenerated `REGISTER_RENDER.md` contains ZERO `System.Collections.Hashtable`
literals; `NativeArtifactCopy.props` and its imports are gone; the amendment and the
F-ledger rows exist; `register_version` is 2.24 and `-Validate` exits 0.

Executor: Claude Code (flagship model), LOCAL on `SKARLET` against
`D:\Colony_Simulator\Colony_Simulator`.

This brief is the authority for this cascade and is STANDALONE. It cites standing law
by anchor rather than restating it. Where this brief and the live code differ, the
code wins and the conflict is recorded in the closure report. Where this brief and a
standing doc differ, the brief is wrong -- HALT and escalate.

## 1. Mission [CORE]

Deliverable milestone: **the Codex remediation cascade properly gated and closed** --
its code verified by the real toolchain, its one regression-of-record (F-13) fixed,
its no-op artifact retired, its record corrected, its residuals in the ledger.

| ID | Artifact | Action | Version |
| -- | -------- | ------ | ------- |
| D1 | `tools/governance/render_register.ps1` | Fix the line-105 hash interpolation (F-13 root cause) | tooling, no version |
| D2 | `native/DualFrontier.Core.Native/NativeArtifactCopy.props` + its consumer imports | RETIRE (delete the props, remove every import) -- CX-14 disposition under the Windows-only ratification | build infra, no version |
| D3 | `docs/reports/CODEX_REVIEW_REMEDIATION_20260714.md` | Append the 2026-07-14 Amendment (Appendix A): corrected arithmetic, verification-strength table, CX namespace, CX-14 outcome correction | 1.0 -> 1.1 |
| D4 | `docs/ROADMAP.md` (F-ledger) | F-13 -> CLOSED; F-32 seeded (CX-02 + CX-06 GPU follow-ups) | ledger |
| D5 | `REGISTER.yaml` + derived artifacts | Register closure: 2.23 -> 2.24, mirrors, EVT, validate, render (the render is also the F-13 acceptance test) | register_version 2.23 -> 2.24 |

## 2. Established facts [CORE]

Verified by the architect against the live tree at HEAD `61f08ef` on 2026-07-14 by
direct line-read unless noted. `[RV]` = the executor re-confirms at Phase 0; any
mismatch is HALT H1.

**Base state.**
- `[RV]` HEAD is `61f08ef` on `main`; working tree clean. (`c8fa0e5 -> 61f08ef` was a
  fast-forward pull; the Codex branch `claude/closed-pr-accessibility-2yxtu8` merged remotely.)
- `[RV]` `REGISTER.yaml`: `register_version: "2.23"`, `last_modified: "2026-07-04"`,
  header anchor `1f55c05`. The Codex cascade enrolled `DOC-E-CODEX_REVIEW_REMEDIATION_20260714`
  (the 287th document) with `last_modified_commit: PENDING-INITIAL` and did NOT bump the version
  (honestly deferred -- its report section 6 says so).
- `[RV]` `REGISTER_RENDER.md`: "Last generated: 2026-07-04", 286 documents; every document row
  shows the literal `$(System.Collections.Hashtable.last_modified_commit)` (the F-13 defect).
- `[RV]` `VALIDATION_REPORT.md`: "Last run: 2026-07-05", 286 documents, Errors: 0.
- `[RV]` F-ledger: F-13 OPEN (render-script defect); F-30 OPEN; F-31 OPEN; next free row number
  is F-32 (confirm empirically).

**F-13 root cause (verified at `tools/governance/render_register.ps1:105`).** The line reads:

    if ($d.last_modified)        { [void]$sb.AppendLine("- **Last modified**: $($d.last_modified) (`$($d.last_modified_commit)`)") }

The author intended markdown inline-code backticks around the hash. In a PowerShell
double-quoted string the backtick is the ESCAPE character: `` `$ `` escapes the dollar,
the subexpression never evaluates, `$d` alone interpolates as its type name, and
`.last_modified_commit` remains literal -- producing
`$(System.Collections.Hashtable.last_modified_commit)` in every row. The fix is to
double both backticks (an escaped backtick emits a literal backtick and leaves the
following `$(...)` subexpression live):

    if ($d.last_modified)        { [void]$sb.AppendLine("- **Last modified**: $($d.last_modified) (``$($d.last_modified_commit)``)") }

**CX-14 no-op (verified in both files).**
- `[RV]` `native/DualFrontier.Core.Native/CMakeLists.txt` sets
  `OUTPUT_NAME "DualFrontier.Core.Native"` and `PREFIX ""` -- non-Windows outputs are
  `DualFrontier.Core.Native.so` / `.dylib`, WITHOUT the `lib` prefix.
- `[RV]` `native/DualFrontier.Core.Native/NativeArtifactCopy.props` contains ONLY
  `lib`-prefixed `Include`/`Exists` pairs (`libDualFrontier.Core.Native.so` / `.dylib`,
  in `build\Release\` and `build\`), each `Exists`-gated. The names can never match the
  CMake output; every item is a permanent silent no-op. The props contains NO Windows
  item -- on Windows it contributes nothing, so retiring it is build-neutral there.
- `[RV]` The props is imported by consumer projects (the Codex report says 9).
  Enumerate the ACTUAL import sites empirically at Phase 0 (Lesson #N18) -- grep all
  `*.csproj` / `*.props` for `NativeArtifactCopy`.

**Ratifications (2026-07-14, operator).**
- The product is **Windows-only per L7**. Linux is a limited host for managed unit
  tests only, not a supported runtime. Cross-platform native artifact copying is
  therefore out of scope BY POLICY, independent of the name mismatch.
- The Codex finding series is namespaced **CX-01..CX-21** (CX-NN maps to the report's
  FNN). Our ROADMAP F-ledger keeps its own F-NN series; any ledger row referencing a
  Codex finding cites its CX id.

**The report arithmetic (verified against the report's own table).** The header
claims 16 fixed + 5 already-closed + 1 architecturally-closed + 1 accepted-latent =
23 outcome slots for 21 findings. The correct mutually-exclusive split by the
report's own per-finding table: **14 changed code/docs** (CX-01..06, 08, 09, 11, 13,
14, 15, 19, 21), **5 confirmed already-closed** (CX-10, 12, 16, 17, 18), **1
architecturally closed with existing coverage** (CX-20), **1 accepted-latent**
(CX-07). 14 + 5 + 1 + 1 = 21. The diff-stat discrepancy (report: 51 files, +560/-197;
audit's git: 52 files, +742/-197) is resolved empirically at execution (section 7.3).

**The ungated-code fact.** The Codex host could not load Roslyn 5.3, so the managed
build that verified the 51 changed files ran with the analyzers GATED OFF -- 17 rules
(11 Error under `TreatWarningsAsErrors`) never inspected that code. Also never run
there: the native selftest (no Vulkan SDK -> no `.so`), the stress / extreme suites
(excluded by instruction), all Win32 / Vulkan / GPU paths (no platform). The local
gates (`DFSkipShaders` / `DFSkipAnalyzers`) were reverted before commit and are NOT
in the diff (verified honest by the report; spot-check both `Directory.Build.props`
at Phase 0). Consequence: Runtime code changed by CX-01/02/05/06/08 and the
`[WindowsOnlyFact]`-gated 108 test methods (CX-09) execute on their target platform
FOR THE FIRST TIME in the Phase-0 Skarlet gate.

**Census pins (canonical).** Reserved-surface census on `src/**/*.cs`: 34 sites / 13
files; DFK-WAIVER 2. This cascade touches NO `.cs` file at all (tooling `.ps1`,
build `.props`/`.csproj` XML, docs, YAML) -- the pins must be trivially unchanged;
re-attest at closure anyway.

## 3. Phase 0 -- preconditions, THE SKARLET GATE, checkpoint [CORE]

Run serially by the orchestrator BEFORE any commit.

1. **Verify the section-2 `[RV]` set.** Any mismatch -> HALT H1.

2. **THE SKARLET GATE (the load-bearing step of this cascade).** The Codex code is on
   `main` ungated; this gate is its first real verification.
   - Full solution build, Release, analyzers ON (no `DFSkipAnalyzers`, no gate
     properties), `TreatWarningsAsErrors` as configured. **Expected: 0 errors.** Any
     analyzer diagnostic on the Codex-changed files -> HALT H2a: report every
     diagnostic verbatim (rule id, file, line, message), await the operator ruling.
     Do not fix analyzer findings unilaterally -- the ruling decides whether fixes
     fold into this cascade as extra commits or spawn their own.
   - The full per-suite closure gate (all 9 suites per TESTING_STRATEGY section 12.7
     discipline), every `dotnet test` via the section-8 no-pipe harness (file
     redirection + watchdog, TRX = truth). Expected shape: match-or-better the F-29
     closure baseline (1097 pass / 0 fail / 5 skip: S2 under F-30, S3/S4/S5a/S5b
     under F-31). The Codex regression tests (CX-03 AssetManager case-sensitivity,
     CX-19 delegate equality, CX-21 TerrainKind) now run on WINDOWS for the first
     time; the 108 `[WindowsOnlyFact]` methods now EXECUTE instead of skipping; the
     CX-02 fail-fast ring guard and CX-05/06/08 Win32/Vulkan changes are exercised by
     Runtime.Tests on real Win32/Vulkan for the first time. ANY failure -> HALT H2b:
     report the TRX verbatim, await ruling.
   - Record the gate outcome (green, or the ruled-on deltas) in the closure report as
     the cascade baseline.

3. **Enumerate the props import sites** -- grep every `*.csproj` and `*.props` under
   the repo for `NativeArtifactCopy`. The resulting list IS the D2 removal work
   order; record it (expected ~9 + the props file itself; the empirical count wins).

4. **Spot-check gate-revert honesty:** both `Directory.Build.props` files contain no
   `DFSkipShaders` / `DFSkipAnalyzers` conditions. A residue -> HALT H1 (the report
   attested their removal).

5. **Validation checkpoint:**
   `powershell -NoProfile -ExecutionPolicy Bypass -File tools\governance\sync_register.ps1 -Validate`
   -- exit 0 required (HALT H3). The refreshed `VALIDATION_REPORT.md` folds into C1.

6. **REGISTER enum read (Lesson #N14):** extract the empirical category / tier /
   lifecycle vocabulary and the exact `DOC-` / `EVT-` entry shapes from
   `REGISTER.yaml`. These verbatim shapes are the only sanctioned templates for
   section 8. Never invent an enum value (HALT H5). Ground how the F-10 / F-29
   closures treated the ROADMAP register entry and mirror that treatment.

7. **Mandatory reads:** this brief; the Codex report (full); `render_register.ps1`
   (head through the row-render loop); `NativeArtifactCopy.props`;
   `CMakeLists.txt:95-110`; the ROADMAP F-ledger tail; `METHODOLOGY` session-closure
   protocol; `TESTING_STRATEGY` sections 8 and 12.7.

NEVER run `-Sync` outside the ratified REGISTER cascade (C6). `render_register.ps1`
runs exactly once, at C7. The executor NEVER pushes.

## 4. Topology [CORE]

**Single orchestrator, serial, no wave.** The architect's Phase-0 line-read already
discharged the survey; the file set is small and the dependency chain is linear
(F-13 fix must precede the render; the amendment precedes the ledger rows that cite
it). Only the orchestrator commits. `docs/ROADMAP.md` and `REGISTER.yaml` are
single-writer.

## 5. Wave R -- survey agents [KIND: phase-execution]

None. Section 2 is the verified substrate; Phase 0 re-verifies. No survey agents.

## 6. Checkpoints [CORE]

- **After C2 (F-13 fix):** dry-run the renderer
  (`powershell ... render_register.ps1`) and inspect the OUTPUT WITHOUT COMMITTING
  it: zero `System.Collections.Hashtable` occurrences; document rows show the real
  field values (hashes or `PENDING-*` literals) inside markdown backticks. Note:
  `PENDING-*` values now becoming VISIBLE in the render is CORRECT fail-loud
  truth-telling (F-2 remains open and now shows); it is not a defect of this fix.
  Then `git checkout` the render (it is regenerated for real at C7 on the closed
  register state). A surviving Hashtable literal -> the fix is wrong, iterate before
  committing C2.
- **After C3 (props retirement):** full solution build + the fast sweep (the
  section-8 filtered suite). Expected: identical to the Phase-0 gate result -- the
  props contributed nothing on Windows, so retirement is build-neutral. Any delta ->
  HALT H2b.
- **Governance self-audit (before C6):** truth law -- the amendment claims nothing
  without an anchor; the ledger rows cite CX ids and real commit hashes; the scope
  matches section 8 exactly (H(governance) otherwise).

## 7. Execution / writer specifications [CORE]

### 7.1 D1 -- F-13 fix (`render_register.ps1:105`)

Replace the line-105 append (exact current text in section 2) with the
doubled-backtick form (exact fixed text in section 2). Touch nothing else in the
script. The acceptance is the C2 checkpoint dry-run plus the real C7 render.

### 7.2 D2 -- retire `NativeArtifactCopy.props` (CX-14 disposition)

Delete `native/DualFrontier.Core.Native/NativeArtifactCopy.props`. Remove every
`<Import Project="...NativeArtifactCopy.props" ...>` line found by the Phase-0
enumeration (expected in 9 consumer `.csproj` files -- the empirical list wins; keep
each csproj otherwise byte-identical). Rationale recorded in the commit body: the
product is Windows-only per L7 (operator ratification 2026-07-14); independently,
the props was a structural no-op (CMake `PREFIX ""` emits unprefixed names; the
props searched `lib`-prefixed exclusively; `Exists`-gating silenced the mismatch).
Linux remains a limited host for MANAGED unit tests only -- which do not require the
native library.

### 7.3 D3 -- the report amendment (1.0 -> 1.1)

Append the Appendix-A amendment section VERBATIM to
`docs/reports/CODEX_REVIEW_REMEDIATION_20260714.md` (an EXECUTED report is corrected
by APPENDED amendment, never by rewriting its body -- the Option-gamma closure-
integrity precedent). Sanctioned minimal in-body touch: ONE pointer line directly
under the `## 1. Result` heading:

    > **Amended 2026-07-14** -- the outcome arithmetic in this section and the CX-14 outcome are corrected by the Amendment section at the end of this report.

Before writing the amendment's diff-stat paragraph, compute the definitive numbers
empirically: `git show --stat` over the Codex merge range (identify the merge /
fast-forward commits between `c8fa0e5` and `61f08ef` from `git log`), state the file
count and +/- WITH an explicit scope sentence (whether the report file itself and
`REGISTER.yaml` are included). Fill the two `<EMPIRICAL: ...>` slots in Appendix A;
everything else lands verbatim.

### 7.4 D4 -- F-ledger rows

- **F-13 -> CLOSED.** Resolution: root cause `render_register.ps1:105` -- a
  backtick-escaped `$` prevented subexpression evaluation, rendering the hashtable
  type name as a literal in every document row; fixed at commit <C2 hash> by
  doubling the backticks; acceptance -- the C7 regenerated render contains zero
  `System.Collections.Hashtable` occurrences across all 288 document rows.
- **F-32 -> NEW OPEN.** Title: GPU-validated Runtime follow-ups from the Codex
  remediation (CX-02 + CX-06). Body: CX-02 -- the sprite `VertexBufferRing` fix is a
  fail-fast guard (reusing a ring slot for a second unsubmitted batch now throws);
  the capacity redesign for genuine `>maxSpritesPerFrame` scenes needs a
  GPU-validated session. CX-06 -- Vulkan present support is now VALIDATED on the
  graphics family (fail-fast); selecting a distinct present-capable family remains
  unimplemented. Both require Skarlet GPU work; neither blocks anything today.
  Status OPEN, disposition architect-owned.
- Do NOT renumber or touch F-30 / F-31.

### 7.5 D5 -- REGISTER closure

Section 8 carries the exact mutations; section 12 the mechanics.

## 8. Governance-closure machinery [KIND: governance]

Ground every target's CURRENT live shape at Phase 0 (enum read, entry shapes, the
F-10/F-29 precedent treatment of ROADMAP's entry); never assume from memory.

- **Enroll this brief** as `DOC-D-CODEX_CLOSURE_BRIEF`, category D, tier 3, Draft ->
  EXECUTED at closure (the standing D-brief convention with
  `special_case_rationale`; the brief FILE frontmatter is hand-authored, not synced).
  Document count 287 -> 288.
- **`DOC-E-CODEX_REVIEW_REMEDIATION_20260714`:** version `1.0` -> `1.1`,
  `last_modified` -> 2026-07-14 (the amendment). Its `last_modified_commit:
  PENDING-INITIAL` stays a PENDING literal -- per-entry backfill is F-2 scope, NOT
  this cascade; only the register HEADER anchor is backfilled at C7 (Option-B, the
  F-29 C10 precedent: header only, one sanctioned residual mention in EVT prose).
- **Header:** `register_version` "2.23" -> "2.24"; `last_modified` -> execution date;
  `last_modified_commit` -> `PENDING-CODEX_CLOSURE` at C6, backfilled to the real C6
  hash at C7.
- **One audit-trail EVT** at C6 carrying the real hashes of C1-C5 (and naming C6/C7
  by role), the corrected outcome split (14/5/1/1), the CX namespace, the Windows-only
  ratification, and the Skarlet-gate outcome. Audit-trail count 46 -> 47.
- **`-Validate` exit 0 mandatory** (HALT H3); fix only within the empirical enum
  vocabulary (HALT H5).
- No other document's version or lifecycle moves. `TESTING_STRATEGY` is UNTOUCHED
  (its section-2.6 ASan correction is a separately-parked item).

## 9. S-LOCK invariants [CORE]

None new in this cascade. Noted for the record: the fail-loud doctrine (every fix
converts a silently-permissive path into a loud one) is the drift program's
cross-cutting acceptance criterion and a pending lesson / S-LOCK candidate -- tracked
in the architect journal, not built here. This cascade already conforms: F-13 makes
missing provenance VISIBLE instead of masked; CX-14 retirement removes a silent no-op
instead of leaving it plausible.

## 10. Census discipline [CORE]

This cascade touches zero `src/**/*.cs` files. HARD pins (reserved-surface 34 sites /
13 files; DFK-WAIVER 2) must be byte-identically unchanged; re-run the census greps at
closure and re-attest the exact numbers in the closure report. `CensusMetaTests` green
is subsumed by the Phase-0 / C3 suite runs. If any pin moves, something outside the
mandate was touched -> stop and investigate (do not adjust a pin).

## 11. Commit plan [CORE]

Serial, dependency order; each commit passes the gates. Count is intended-form; a
ruled-on Skarlet-gate remediation may add commits after C1 (record the deviation).

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(register): enroll CODEX_CLOSURE brief + validation checkpoint` | brief file + refreshed VALIDATION_REPORT |
| C2 | `fix(governance-tooling): render hash interpolation -- F-13 root cause` | D1, line 105 |
| C3 | `revert(native-artifacts): retire NativeArtifactCopy.props (CX-14, Windows-only per L7)` | D2, props + imports |
| C4 | `docs(codex-report): amendment 2026-07-14 -- outcome arithmetic + CX namespace (1.0 -> 1.1)` | D3, Appendix A |
| C5 | `docs(roadmap): F-13 -> CLOSED; F-32 seeded (CX-02 + CX-06 GPU follow-ups)` | D4 |
| C6 | `governance(register): CODEX_CLOSURE register closure (2.23 -> 2.24)` | D5 + EVT + validate folded |
| C7 | `governance(register): render regeneration + header backfill (F-13 acceptance)` | render (zero Hashtable literals) + Option-B header backfill |

## 12. REGISTER cascade [CORE]

Executed at C6/C7 using ONLY the Phase-0 verbatim shapes: the D-brief enrollment,
the DOC-E version bump + mirror, the header bump, the EVT append; then C7 runs
`render_register.ps1` ONCE and backfills the header anchor to the real C6 hash.
Post-render assertion (the F-13 acceptance): zero `System.Collections.Hashtable`
occurrences in `REGISTER_RENDER.md`; statistics header shows 288 documents /
register version 2.24. `-Validate` re-run after the render must remain exit 0.

## 13. Halt conditions (H-series) [CORE]

- **H1** precondition mismatch (section 3.1 / 3.4).
- **H2a** analyzer diagnostics on the ungated Codex code (Phase 0) -- report verbatim,
  await ruling; do not fix unilaterally.
- **H2b** test regression at Phase 0 or after C3 -- report the TRX verbatim, await ruling.
- **H3** `-Validate` nonzero.
- **H4** a live-code fact contradicts a section-2 `[RV]` fact.
- **H5** a needed REGISTER enum value is absent from the Phase-0 vocabulary -- never invent.
- **H(governance)** any edit to the Codex report beyond the one pointer line + the
  appended amendment; any `TESTING_STRATEGY` touch; any F-ledger change beyond F-13 /
  F-32; any other doc's version or lifecycle.
- Standing rails: no pushes; no `-Sync` outside C6; no history rewrite (the published
  Codex commit subjects and body are NEVER edited -- amendment only); single-writer
  ROADMAP / REGISTER.

On halt: stop, report state verbatim, await the operator.

## 14. Closure protocol and report [CORE]

Per the `METHODOLOGY` session-closure protocol. The closure report (chat) carries:
the commits table (real hashes C1-C7 + any ruled-on extras); the versions table
(register 2.23 -> 2.24; report 1.0 -> 1.1); **the Skarlet-gate outcome** (the
analyzer verdict + the 9-suite table vs the F-29 baseline -- this is the cascade's
headline result); the props-import enumeration as executed; census pins re-attested;
the F-ledger final state (F-13 CLOSED, F-32 OPEN); `Skeleton revisions` (every
deviation, e.g. the empirical import count, the diff-stat scope resolution); the
render assertion result; self-attestation (no pushes, single render run, no history
rewrite, only the enumerated files); the operator manual checklist (review range,
push, remaining parked items: TESTING_STRATEGY 2.6 micro-patch, F-30/F-31, the drift
program).

## 15. Out of scope [CORE]

- **F-2 / the `last_modified_commit` schema** (DF-DOC-003): per-entry PENDING backfill
  and the store-vs-derive redesign belong to the validator-2.0 / drift program. This
  cascade only makes the existing values RENDER truthfully.
- **The drift program itself** (all 24 DF-DOC findings beyond F-13 and the CX-14
  retirement): separate cascades under the four-class cut.
- **The ISOLATION soft-unload reconnection** (DF-DOC-004): its own cascade.
- **`TESTING_STRATEGY` section 2.6** ASan micro-patch: parked separately.
- **Windows fail-loud default for the native `.dll` copy** (DF-DOC-008): drift class 2.
- **`build.md` narrowing and any doc-role work**: drift classes 2 / 4.
- **The validator gap** (DF-DOC-002): validator-2.0 cascade.
- **CX-20 residual, CX-07 latent:** recorded in the amendment as accepted; no ledger
  row (theoretical-only; re-open on demand).
- Pushes, the reference tree, EXECUTED-doc content beyond the sanctioned amendment.

## Appendix A -- Amendment text for CODEX_REVIEW_REMEDIATION_20260714.md [KIND: governance]

Everything between BEGIN and END lands verbatim at the end of the report, except the
two `<EMPIRICAL: ...>` slots, which the executor fills from git as specified in 7.3.

BEGIN
## 8. Amendment -- 2026-07-14 (v1.1)

Appended per the closure-integrity convention (an EXECUTED report is corrected by
amendment, never by rewriting its body). Three corrections and one namespace ruling.

### 8.1 Finding-series namespace

To end the collision with the repository's own ROADMAP F-ledger (F-1..F-32), the 21
findings of this report are hereafter referenced as **CX-01..CX-21** (CX-NN = this
report's FNN). The body above retains its original FNN labels as published history.

### 8.2 Corrected outcome arithmetic

Section 1 above states 16 + 5 + 1 + 1 = 23 outcome slots for 21 findings. The
correct, mutually-exclusive split per this report's own section-3 table:

| Outcome | Count | Findings |
| --- | ---: | --- |
| Changed code/docs in this cascade | 14 | CX-01..06, 08, 09, 11, 13, 14, 15, 19, 21 |
| Confirmed already-closed, no change | 5 | CX-10, 12, 16, 17, 18 |
| Architecturally closed, existing coverage, no change | 1 | CX-20 |
| Accepted-latent, no change | 1 | CX-07 |

Verification strength of the 14 changed items (from the section-3 verified-by
column): runtime-tested 4 (CX-03, 15, 19, 21); SPIR-V-regenerated 1 (CX-04);
compile-verified 6 (CX-01, 02, 05, 06, 08, 09); doc/YAML-checked 2 (CX-11, 13);
inspection-only 1 (CX-14). Outcome category and verification strength are distinct
axes and are not conflated hereafter.

Definitive diff stat, computed from git at amendment time: <EMPIRICAL: N files,
+A/-D, over commits X..Y>. Scope: <EMPIRICAL: explicit statement of whether this
report file and REGISTER.yaml are included>.

### 8.3 CX-14 outcome correction (was counted "fixed"; actual: structural no-op, now retired)

The CX-14 change (`NativeArtifactCopy.props`) could never copy anything:
`CMakeLists.txt` sets `PREFIX ""`, so the non-Windows artifacts are
`DualFrontier.Core.Native.so` / `.dylib` WITHOUT the `lib` prefix, while the props
searched exclusively `lib`-prefixed names; `Exists`-gating turned the mismatch into a
silent no-op. Verified by line-read 2026-07-14. Under the operator ratification of
the same date -- **the product is Windows-only per L7**; Linux is a limited host for
managed unit tests only -- the props is RETIRED in the CODEX_CLOSURE cascade rather
than repaired. CX-14's outcome is reclassified: not "fixed" but
"retired-out-of-scope (Windows-only), original change was inoperative".

### 8.4 Standing accepted records

- **CX-07** (Vulkan callback calling convention on x86): accepted-latent -- no
  project targets x86; harden to `Winapi` only if a real Win32-x86 target is added.
- **CX-20 residual** (`ValidateRegularModContractTypes` scans `GetExportedTypes()`
  only; a non-public `internal IEvent` in a regular mod would slip past):
  accepted-theoretical -- the bus is `internal` to Core, so a regular mod has no
  sanctioned publish path. Re-open on demand as defense-in-depth.
- **CX-02 + CX-06** GPU-validated follow-ups: tracked as ROADMAP **F-32** (OPEN).
END

---

**End of CODEX_CLOSURE_BRIEF.md v1.0**