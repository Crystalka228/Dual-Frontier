# RECON KICKOFF TEMPLATE -- the universal Dual Frontier read-only reconnaissance scaffold

This is the canonical scaffold for a Dual Frontier pre-brief reconnaissance
kickoff. It is derived from the project's real recon kickoffs (Standing-Law,
Architecture-Truth, Godot Eradication) and carries the union of their
load-bearing sections. A recon kickoff is a TRANSIENT dispatch artifact (not
enrolled, not committed): the operator runs it as a fresh read-only Claude Code
session. The recon WRITES ITS REPORT TO A DURABLE FILE at
`docs/reports/<CASCADE_NAME>_RECON_REPORT.md` (uncommitted, untracked) AND
presents it in chat -- so the brief-authoring session rests on the on-disk file
without manual cross-session transfer, and the cascade enrolls the report at its
first commit.

The recon exists because a brief authored on assumptions is guessing. Recon
routinely overturns load-bearing assumptions -- a tool that is not actually
read-only, a file that already exists where it was expected absent, a count off by
a large factor. The whole point is to replace guesses with measured facts.

## How to instantiate

1. Copy the scaffold block below into a dispatch file (delivered to the operator,
   not committed).
2. Replace every `<...>` placeholder; delete every `> Guidance:` line.
3. Keep every numbered section. Tailor the T-tasks to the cascade's three or four
   measurement fronts; keep the inviolable rules and the self-attestation verbatim.

## Hygiene

ASCII for prose; the genuine Cyrillic invariant IDs (K-L bi-script reality) may
appear in grep expressions where the repo uses them -- match BOTH scripts in any
census that targets them. Every count carries its grep/rg expression verbatim
(the reproducibility law; the lesson where a bare-pattern line count read 78 and
the canonical application-site count was 34 -- method, not drift).

---

## === RECON KICKOFF SCAFFOLD: copy from here ===

# KICKOFF -- Pre-Brief Reconnaissance: <CASCADE_NAME> (read-only)

**Purpose**: <one paragraph -- the TRUE state this recon measures before the
<CASCADE_NAME> brief is authored, named as the two-to-four measurement fronts the
T-tasks cover>. You produce ONE structured report in chat. Nothing else.

**Executor**: Claude Code (flagship model), LOCAL read-only session on the
operator's machine.
**Repository**: `D:\Colony_Simulator\Colony_Simulator`. <Name any read-only
reference tree, e.g. News Intelligence Hub, as FROZEN -- read, never modified.>

**Standing law in force** (cite, do not restate): census method discipline --
`TESTING_STRATEGY` (every count carries its verbatim expression). <Name the
instrument this recon uses if it is a classification recon, e.g. the
LIVE/DEAD/HISTORICAL verdict.>

## 1. Inviolable session rules

1. **ZERO writes except the report file.** The ONLY file this session creates is
   its own report at `docs/reports/<CASCADE_NAME>_RECON_REPORT.md` (new,
   untracked -- the durable deliverable). No edit or delete of any existing file
   in any tree; no other new file. Schema-2.0 rule: NEVER run the governance
   tool's `sync` (it regenerates the derived registers = a write); do not run
   `validate` either unless the kickoff explicitly sanctions it after confirming
   it is write-free -- read the derived `REGISTER.yaml` /
   `CURRENT_AUTHORITY_SURFACE.yaml` and per-document frontmatter directly (the
   frontmatter is the SoT).
2. **ZERO git mutations** -- no commit, checkout, switch, merge, fetch, stash. The
   working tree stays exactly as found. Inspect any branch not checked out via
   `git show <ref>:<path>` -- never switch branches.
3. **ZERO governance mutations** -- the `DualFrontier.Governance` tool's `sync`
   is never run; derived registers and `AUDIT_TRAIL.yaml` are never touched;
   frontmatter is never edited. Read state directly from the files.
4. Every census records its grep/rg expression **verbatim** beside its count.
5. If a measurement is impossible under these rules, record the gap explicitly --
   never estimate silently.
6. Output: write the full structured report to
   `docs/reports/<CASCADE_NAME>_RECON_REPORT.md` AND present the same content in
   chat. Do NOT commit it (zero git mutations) -- it stays untracked; the cascade
   enrolls it at its first commit.

## 2. Collection tasks

> Guidance: tailor T-tasks to this cascade's measurement fronts. T1 is always base
> state. Each task names exactly what to measure and the read-only command form.
> Use `git show <ref>:<path>` for cross-branch inspection; the Filesystem MCP for
> Windows paths; `.git/HEAD`, `.git/refs/heads/main`, `.git/logs/HEAD` for git
> state.

### T1 -- Base state
Current branch + HEAD hash; working-tree status (`git status --porcelain` -- full
modified + untracked list); `main` HEAD; any relevant branch HEAD + merge-base;
commits after the expected HEAD (verify none, or list); divergence vs origin
(local refs, no fetch -- say so); schema/register version, document count, and the
authority-surface count read directly from the DERIVED `REGISTER.yaml` and
`CURRENT_AUTHORITY_SURFACE.yaml` (never regenerate them); EVT count from
`AUDIT_TRAIL.yaml`.

### T2..Tn -- <measurement fronts>
> Guidance: one task per front. Common DF fronts:
> - **Document inventory**: per doc -- path, register_id, version, lifecycle, line
>   count, last commit, banner/fence presence; roadmap-load and stale-vocabulary
>   scores (rg, expressions recorded).
> - **Code-truth survey**: what is actually enforced (Directory.Build.props
>   switches, .editorconfig, CPM state, analyzer stub state); style as practiced
>   (sample N files); commit practice (sample git log).
> - **Marker / reserved-surface census**: the canonical pin (e.g. `[ReservedStub`
>   application sites, excluding the definition file -- exact count + one-sentence
>   composition rule); doc-tag families with verbatim patterns and baseline counts;
>   `#pragma disable DFK/DFL` (expect 0).
> - **Comment-reference census**: law citations in code (K-L / PA / Q / F / DFK /
>   doc-name / section), stale-vocabulary, and a stratified sample audit (N sites,
>   every-Nth, not cherry-picked) with per-site verdict
>   (VALID / STALE-TARGET / STALE-CLAIM / DANGLING) and an extrapolated rate.
> - **ROADMAP-vs-reality delta**: per track -- claimed state vs git/code evidence
>   vs verdict; unrepresented realized work; F-ledger cross-check.
> - **Classification census** (eradication kind): every target site classified by
>   the cascade's instrument (e.g. LIVE / DEAD / HISTORICAL) with the evidence that
>   decides it; a bare count without a verdict is an incomplete measurement.

### T(n+1) -- Reference accessibility (if a reference tree is used)
Confirm the read-only reference is readable; presence + line counts only; touch
nothing.

### T(last) -- Anomaly sweep + scale estimate
Anything diverging from this kickoff's expectations; any WIP not captured. Then the
sizing block the brief needs: counts per work class and a proposed commit-class
split.

## 3. Report schema (written to `docs/reports/<CASCADE_NAME>_RECON_REPORT.md` AND presented in chat -- exactly these sections)

```
# <CASCADE_NAME> RECON REPORT -- <date>

## R1 Base state
## R2..Rn <one section per measurement front, tables with verbatim expressions>
## R(last-1) Anomalies + scale estimate
## R(last) Self-attestation
  - Zero writes except the report file at docs/reports/ (validate NOT run): <confirm>
  - Report written to docs/reports/ AND presented in chat (uncommitted): <confirm>
  - Zero git mutations: <confirm>
  - Every census expression recorded verbatim: <confirm>
  - <Reference tree> untouched: <confirm, if applicable>
  - <Every site classified, if a classification recon>: <confirm>
```

Report values exactly as measured. Where a measurement is impossible under the
read-only rules, say so rather than estimating. Bez kostylei.

## === RECON KICKOFF SCAFFOLD: copy to here ===

---

**End of RECON_KICKOFF_TEMPLATE.md**
