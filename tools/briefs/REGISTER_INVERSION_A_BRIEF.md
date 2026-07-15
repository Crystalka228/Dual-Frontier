---
register_id: DOC-D-REGISTER_INVERSION_A_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft (-> LOCKED on Crystalka ratification -> EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-15'
content_language: en
authored_by: Claude Opus (deliberation session, drift-program arc)
basis: VALIDATOR_2 RECON REPORT R1-R8 (verified 2026-07-15) + the NIH governance model study (validators.mjs / sync-register.mjs / REGISTER.yaml read in full by the architect 2026-07-15) + operator ratifications (the inversion; the two-register architecture; DF collections kept; .NET port)
---

# REGISTER_INVERSION Cascade A -- Execution Brief (the instrument)

Dual Frontier's governance layer is being re-architected on the model proven in the
operator's News Intelligence Hub project: **the register inverts**. Today
`REGISTER.yaml` is the hand-edited source of truth and per-document frontmatter
blocks are machine-written mirrors; the VALIDATOR_2 recon measured what that
architecture costs -- 28 mirror-drift discrepancies invisible to the validator, 269
broken `register_view_url` anchors manufactured by the toolchain's own two writers
disagreeing, a 64%-placeholder `last_modified_commit` population growing every
cascade, and a validator that implements ~10 of ~27 declared gates while
`Errors: 0` reads as health. Under the inverted model **each document's frontmatter
is the source of truth**, and TWO derived artifacts are regenerated from it by one
tool: the full `REGISTER.yaml` (the ARCHIVE corpus -- everything that exists, all
lifecycles) and `CURRENT_AUTHORITY_SURFACE.yaml` (the boot subset a session loads
as authoritative NOW). The mirror-drift class, the anchor class, and the
register-vs-frontmatter desync class cease to exist structurally. Dual Frontier's
global collections -- requirements, risks, CAPA, the EVT audit trail -- are KEPT,
relocated to hand-edited globals files that the same tool validates and merges into
the derived archive.

This cascade (A) builds THE INSTRUMENT and changes no governed state: the
FRAMEWORK 2.0 law, the .NET toolchain (`DualFrontier.Governance` +
`Governance.Tests`), the corpus-migration logic proven against a scratch COPY, and
the dry-run measure report that becomes Cascade B's work order. Cascade B (its own
brief) applies the migration to the live corpus, aligns it, ARMS the gates, and
retires the PowerShell writers. Deployment discipline is NIH's, adopted verbatim:
**measure -> align -> arm**, and **every gate must prove red-once-then-green** ("a
never-red gate is presumed vacuous").

**Done** when: `FRAMEWORK.md` 2.0 states the inverted law with the DF schema, the
gate catalog, the sentinel sets, the authority-surface predicate, and the globals
protocol; `dotnet run --project tools/DualFrontier.Governance -- sync|validate|migrate`
works; `dotnet test` runs the governance suite green with every gate proven red and
green on fixtures; the migration dry-run against a full scratch copy of the repo
docs round-trips 288/288 documents and produces the measure report; and the OLD
governance regime is untouched and still validates exit 0 (this cascade closes
under the old regime -- REGISTER 2.24 -> 2.25).

Executor: Claude Code (flagship model), LOCAL on `SKARLET` against
`D:\Colony_Simulator\Colony_Simulator`. The NIH tree
`D:\Start-up_Projects\news-intelligence-hub` is a READ-ONLY REFERENCE -- read for
model fidelity, never modified.

This brief is the authority for this cascade and is STANDALONE. It cites standing
law by anchor. Where this brief and the live code differ, the code wins and the
conflict is recorded in the closure report. Where this brief and a standing doc
differ, the brief is wrong -- HALT and escalate (exception: the FRAMEWORK sections
this brief explicitly amends).

## 1. Mission [CORE]

Deliverable milestone: **the inverted-register instrument, proven on a dry-run
corpus, with the law amended and the old regime untouched.**

| ID | Artifact | Action | Version |
| -- | -------- | ------ | ------- |
| D1 | `docs/governance/FRAMEWORK.md` | MAJOR amendment: schema 2.0 -- the inversion law, DF schema v2, the gate catalog, sentinels, the authority-surface predicate, the globals protocol, the deployment discipline | 1.1.2 -> 2.0.0 |
| D2 | `tools/DualFrontier.Governance/` (new .NET console) + sln + CPM wiring | The toolchain: `sync`, `validate`, `migrate` commands over a pure validation core | new |
| D3 | `tests/DualFrontier.Governance.Tests/` (new xUnit project) | The executable specification: every gate fixture-proven green AND red | new |
| D4 | Migration logic (inside D2, `migrate` command) | Globals extraction + frontmatter injection + derived regeneration -- BUILT and dry-run-proven, NOT applied to the live corpus | new |
| D5 | `docs/reports/REGISTER_INVERSION_A_MEASURE_REPORT.md` | The dry-run measure: report-only gate findings over the migrated scratch corpus = Cascade B's alignment work order | new |
| D6 | `docs/ROADMAP.md` + `REGISTER.yaml` (old regime) | F-34 seeded (Cascade B); enroll brief + VALIDATOR_2 recon report; REGISTER 2.24 -> 2.25 | ledger + register |

## 2. Established facts [CORE]

`[RV]` = the executor re-confirms at Phase 0; mismatch -> HALT H1.

**Base state.**
- `[RV]` HEAD is `f0f76a8` on `main` (or later if the operator pushed + advanced --
  re-anchor and record); tree clean. `register_version` "2.24"; 288 documents / 41
  REQ / 14 RISK / 17 CAPA / 47 EVT (the id-prefix census from the VALIDATOR_2
  recon, R1).
- `[RV]` The VALIDATOR_2 recon report exists untracked at
  `docs/reports/VALIDATOR_2_RECON_REPORT.md` (enrolls at C1 alongside this brief).

**The verified defect surface this program dissolves** (VALIDATOR_2 recon,
architect-verified): 28 mirror-drift discrepancies (7 field-drift + 13
hand-authored non-canonical + 8 no-FM); 269 `register_view_url` anchors that the
sync writes and the render never emits; 186 `last_modified_commit` PENDING
placeholders (64%) with ~22% more stale and debt growing 123 -> 130
`PENDING-COMMIT` since June; ~35/71 `next_review_due` values structurally
un-evaluable by the string compare; G1 (doc-changed-by-milestone) and G2
(cross-reference resolution) declared strict but absent from code; 2 of
FRAMEWORK's own 5 falsification tests referencing instruments that do not exist.

**The NIH reference model** (architect-verified by full read, 2026-07-15; the
executor re-reads at Phase 0 for implementation fidelity):
- `[RV]` `D:\Start-up_Projects\news-intelligence-hub\tools\register\validators.mjs`:
  pure-function validation core consumed by BOTH the CLI and the test suite;
  `REQUIRED_FIELDS` + enum sets as exported constants; `validateFrontmatter`;
  `validateCrossReferences` with the authorizing-record supersession semantic;
  five semantic GATEs as data rules carrying `code` + `cite` + `violates` +
  `message` ("the tool never OWNS the law"); `special_case_rationale` = sanctioned
  deviation exempting GATE-1; `selectAuthoritySurface` + `renderAuthoritySurface`
  (deterministic, no volatile timestamp).
- `[RV]` `...\tools\sync-register.mjs`: walks `*.md`, extracts TOP-of-file
  frontmatter, validates (schema -> exit 1; xref -> exit 1; gates ARMED -> exit 1),
  sorts by register_id, emits the derived register under a 4-line generated-header,
  idempotent (semantic compare preserving timestamps + CRLF/LF normalization -->
  byte-identical when unchanged), regenerates the authority surface every run.
  No-frontmatter files are WARN + skip (the one fail-open residue -- the DF port
  hardens this, see D2).
- `[RV]` `...\tools\register\validators.test.mjs`: every gate exercised green AND
  red ("the failability proof that the gate is not vacuous").
- `[RV]` `...\docs\governance\REGISTER.yaml` header declares the inversion
  ("Source of truth: the frontmatter in each document file").

**DF-specific adaptation facts.**
- DF enums are richer than NIH's and are KEPT: categories A-J (10), tiers 1-5,
  lifecycles incl. AUTHORED-SKELETON and STALE, `is_meta_entry` (5 meta-entries),
  `special_case_rationale`.
- DF has FOUR global collections NIH lacks: requirements / risks / capa_entries /
  audit_trail. KEPT, relocated (D1/D4): hand-edited SoT files
  `docs/governance/REQUIREMENTS.yaml`, `RISKS.yaml`, `CAPA.yaml`,
  `AUDIT_TRAIL.yaml` (append-only discipline for the last), validated by the tool
  and MERGED into the derived archive register so the archive keeps today's full
  shape.
- `[RV]` DF README.md files carry END-placed register frontmatter (the documented
  GitHub-rendering policy in `sync_register.ps1`) -- the DF extractor must check
  EOF for the README class (NIH's reads top-only).
- `[RV]` `Directory.Packages.props` has 9 entries incl. xunit 2.9.2; YamlDotNet
  ABSENT (one CPM add). YamlDotNet reads the corpus losslessly (recon R6: 248 ms);
  the comment-loss limitation is IRRELEVANT under the inversion (the derived
  register is machine-owned; the hand-edited surfaces are small per-doc FM blocks
  and the four globals files, none machine-rewritten... except FM injection at
  migration, see D4's preservation rule).
- `[RV]` `tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs` `RepoRoot()`
  (walk up to `DualFrontier.sln`) is the repo-discovery pattern to reuse.
- The orphan triage (recon R4.2, ratified): 12 MODULE.md + 5 UNCLEAR -> ENROLL
  (they receive frontmatter at Cascade B); 2 AnalyzerReleases + 6 scratch ->
  EXCLUDE (the exclusion list). The old `SCOPE_EXCLUSIONS.yaml` seeds the new
  tool's exclusion config.
- Census pins: reserved-surface 34/13; DFK-WAIVER 2; marker families incl.
  `not yet` 10/9. This cascade adds NEW projects (not `src/`); the only `src/`
  risk is accidental -- pins must be byte-identically unchanged.

**Ratified design decisions (operator, 2026-07-15) -- not re-openable by the executor:**
1. The inversion itself; frontmatter = SoT.
2. TWO derived registers: full `REGISTER.yaml` = the ARCHIVE corpus;
   `CURRENT_AUTHORITY_SURFACE.yaml` = the session boot subset.
3. All DF collections and principles KEPT (REQ/RISK/CAPA/EVT, enum richness,
   meta-entries, rationale mechanism).
4. Substrate = .NET port of the NIH model (project-native, per NIH's own
   substrate principle); governance gates join the same `dotnet test` gate as
   `CensusMetaTests`. Node is NOT introduced.
5. DF hardening over NIH: a `.md` without frontmatter OUTSIDE the exclusion list
   is an ERROR, not a warn-skip (fail-closed).
6. Per-doc `last_modified_commit` is NOT part of DF schema v2 required fields;
   the PENDING class ceases to exist at migration (F-2 dissolves at Cascade B).
   Real curated hashes MAY be kept as optional fields where they exist and are
   real; `closed_commit` / `verification_commit` in globals records are kept.

## 3. Phase 0 -- preconditions and checkpoint [CORE]

1. **Verify the section-2 `[RV]` set** (repo base state; the NIH reference files
   readable; CPM/sln shapes; the README EOF-frontmatter reality on 2-3 live
   READMEs). Mismatch -> HALT H1.
2. **Baseline gates:** full build + the fast sweep green; OLD-regime
   `sync_register.ps1 -Validate` exit 0 (report churn folded into C1 per the
   standing convention). Record as baseline.
3. **NIH model grounding read** (implementation fidelity): `validators.mjs`,
   `sync-register.mjs`, `validators.test.mjs`, the NIH `REGISTER.yaml` header +
   2 entries, `CURRENT_AUTHORITY_SURFACE.yaml` -- read-only, LIST-confirm each.
4. **FRAMEWORK grounding read**: the CURRENT live `FRAMEWORK.md` in full (the D1
   amendment is section-surgical, not a rewrite -- grounding prevents collateral
   loss). Also `METHODOLOGY` section 12.7/12.9, `DEVELOPMENT_HYGIENE` section 4,
   `TESTING_STRATEGY` sections 4-5 (the meta-test + failability law anchors).
5. **REGISTER enum read (Lesson #N14)** from the live `REGISTER.yaml` -- both for
   the C-final old-regime closure AND as the empirical enum source for the D2
   constants (the tool's enums MUST equal the corpus's actual vocabulary, not this
   brief's memory).
6. **Validation checkpoint**: `-Validate` exit 0 (H3).

NEVER run `-Sync` outside the sanctioned commits; `render_register.ps1` runs only
at the final render commit if the old-regime closure requires it (ground the
F-29/CODEX precedent). The executor NEVER pushes. The NIH tree is read-only.

## 4. Topology [CORE]

Single orchestrator, serial. The dependency chain is linear (law -> tool -> tests
-> migration -> measure -> closure). `docs/ROADMAP.md` / `REGISTER.yaml` /
`FRAMEWORK.md` single-writer. No wave.

## 5. Wave R -- survey agents [KIND: phase-execution]

None. The VALIDATOR_2 recon + the architect's NIH study discharged the survey.

## 6. Checkpoints [CORE]

- **After C4 (tests):** `dotnet test` on the governance suite -- every gate MUST
  show a red fixture and a green fixture in the test names/asserts (the
  failability proof). A gate without a red fixture fails this checkpoint by
  definition.
- **After C5 (migration dry-run):** the round-trip proof on the scratch copy --
  (a) 288/288 documents receive valid frontmatter; (b) `sync` over the copy
  regenerates a derived register whose SEMANTIC content (ids, fields modulo the
  ratified drops) reconciles with the old `REGISTER.yaml` entry-by-entry: zero
  lost documents, zero invented fields; (c) the four globals files carry exactly
  41/14/17/47 entries; (d) a SECOND `sync` run over the unchanged copy is
  byte-identical (idempotency). Any reconciliation delta beyond the ratified
  drops -> HALT H(preserve).
- **After C6 (measure):** the measure report exists and classifies EVERY finding
  into Cascade-B work classes (align-edit / enroll / exclude / architect-ruling).
  A bare count without classification fails the checkpoint.
- **Old-regime integrity (before C-final):** `-Validate` still exit 0; the live
  corpus untouched by the dry-run (git status clean of doc changes).

## 7. Execution / writer specifications [CORE]

### 7.1 D1 -- FRAMEWORK 2.0 (MAJOR amendment)

Section-surgical amendment of the grounded live text; Appendix A carries the
load-bearing law verbatim; integration wording is executor-grounded. Content:
1. **The inversion law**: frontmatter = SoT; `REGISTER.yaml` = derived ARCHIVE
   (audit corpus, all lifecycles); `CURRENT_AUTHORITY_SURFACE.yaml` = derived boot
   subset. Both regenerated by the tool; both carry generated-headers; neither is
   hand-edited.
2. **DF schema v2 required fields**: `register_id, project, category, tier,
   lifecycle, owner, version, first_authored, last_modified, content_language,
   next_review_due` -- DF's enums (A-J, 1-5, the full lifecycle set) unchanged.
   Optional: `special_case_rationale`, `is_meta_entry`/`meta_role`, `supersedes`/
   `superseded_by`/`deprecated_by`, `last_modified_commit` (real hashes only --
   the PENDING vocabulary is outlawed by GATE), review fields.
3. **Sentinel law** (GATE-5 style): the `next_review_due` sanctioned literal set =
   `'null'` | ISO date | `YYYY-QN` | `post-<event> closure`; closed set; a new
   form requires a ratified extension.
4. **The gate catalog**: every machine gate listed as law with its ID, statement,
   and the citing section -- the tool mirrors, never owns (the NIH 9.4 principle).
   Initial set: schema/required/enums; unique ids; cross-ref RESOLUTION
   (supersedes/superseded_by/deprecated_by, the authorizing-record semantic);
   category x lifecycle coherence rules (DF's actual crisp prohibitions --
   grounded from the corpus, not invented); terminal-state coherence (terminal ->
   `next_review_due: 'null'`); sentinel set; path existence + no-FM-is-ERROR
   outside exclusions; namespace (`DOC-<cat>-` prefix match); globals enums;
   the rationale-ratio monitor (reports the override-vs-voluntary split -- the
   FRAMEWORK section-10 #5 instrument, closing the untestable-falsification gap).
5. **The globals protocol**: the four hand-edited SoT files; `AUDIT_TRAIL.yaml`
   append-only; merged into the derived archive by sync.
6. **The deployment discipline**: measure -> align -> arm; red-once-then-green
   required before any gate is trusted; gates run REPORT-ONLY until Cascade B
   arms them.
7. **Falsification tests reconciled**: section-10 #4/#5 re-pointed at instruments
   that now exist (the STALE scan; the ratio monitor).
8. Schema-history row 2.0; change-history entry. The OLD forward-register
   sections are marked as superseded-by-2.0 with the transition note (Cascade B
   executes the switch) -- the law describes BOTH regimes until B closes, honestly
   labeled.

### 7.2 D2 -- the toolchain (`tools/DualFrontier.Governance/`)

New .NET console project (net10.0, CPM, YamlDotNet added to
`Directory.Packages.props`), sln-wired next to `DualFrontier.Analyzers`.
Architecture mirrors NIH's extraction: a pure validation core (static, no I/O --
`Validators.cs`: required fields, enums, xref resolution, the gate rules as data
records carrying `Code`/`Cite`/`Violates`/`Message`) consumed by BOTH the console
commands and the test project. Commands:
- `validate` -- walk `*.md` (exclusion config seeded from `SCOPE_EXCLUSIONS.yaml`,
  extended per the ratified triage), extract FM (TOP-of-file; EOF for README.md),
  parse, run schema + xref + gates. Findings print structured; in this cascade the
  mode is REPORT-ONLY for the semantic gates (an `--armed` flag exists but
  defaults off until Cascade B flips it -- mirror NIH's
  `SEMANTIC_GATES_ENFORCING` constant discipline). Schema/parse errors are
  ALWAYS exit-affecting (they were never satisfied silently in NIH either).
  **No-frontmatter outside exclusions = ERROR (the DF hardening).**
- `sync` -- validate, then regenerate the TWO derived artifacts: the archive
  register (generated-header; documents sorted by id; globals merged from the four
  files; the DOC-G-REGISTER self-entry emitted deterministically) and the
  authority surface (**DF predicate: `lifecycle == Live` OR (`lifecycle == LOCKED`
  AND tier in {1, 2})** -- stated in D1 as tunable law). Idempotent: semantic
  compare + LF normalization; byte-identical when unchanged.
- `migrate` -- D4.
On this cascade the live corpus has NO frontmatter-SoT yet, so `validate`/`sync`
are exercised against fixtures and the D4 scratch copy only; running them against
the live tree is expected to fail loudly (that failure is CORRECT and is asserted
by a test).

### 7.3 D3 -- the executable specification (`tests/DualFrontier.Governance.Tests/`)

xUnit, references the D2 project, reuses the `RepoRoot()` walk-up pattern. For
EVERY gate and every schema rule: a green fixture and a red fixture (in-memory
frontmatter objects, the NIH `validDoc(overrides)` pattern). Plus: the
README-EOF extraction test; the idempotency test (two syncs over a fixture corpus
byte-identical); the no-FM-is-ERROR test; the globals merge test (fixture globals
files -> derived archive carries them); the authority-surface predicate test
(fixture corpus -> exactly the Live + LOCKED-tier-1/2 subset, sorted,
deterministic). The suite joins the standard `dotnet test` gate.

### 7.4 D4 -- migration logic (`migrate` command; BUILT, dry-run only)

Operates ONLY on an explicit `--target <path>` scratch copy this cascade. Steps it
implements: (1) extract the four globals collections from the old `REGISTER.yaml`
into the four SoT files (content-preserving, key order normalized); (2) for every
register document entry, inject/merge DF-v2 frontmatter into the doc file --
merging INTO the existing mirror block (replace the machine-mirror with the fuller
v2 block; PRESERVE any pre-existing non-register frontmatter; README class stays
EOF-placed); fields sourced from the register entry; the ratified drops applied
(`register_view_url` dropped; `last_modified_commit` dropped where PENDING, kept
where a real hash); docs with no register `next_review_due` receive `'null'`;
(3) the 12+5 ENROLL orphans receive freshly-authored v2 frontmatter (category/
tier per the ratified triage -- MODULE.md = F/4); the 8 EXCLUDE paths land in the
exclusion config; (4) regenerate both derived artifacts; (5) emit the
reconciliation table (the C5 checkpoint input). The command REFUSES to run
against the repo root without `--i-understand-this-mutates-the-corpus` (Cascade
B's flag) -- fail-closed by construction.

### 7.5 D5 -- the measure report

`docs/reports/REGISTER_INVERSION_A_MEASURE_REPORT.md` (untracked until enrolled at
its commit): the dry-run corpus's REPORT-ONLY gate findings, each classified into
Cascade-B work classes; the reconciliation table; the rationale-ratio monitor's
first real reading (override-vs-voluntary split of the 95); the sizing block for
the Cascade B brief.

### 7.6 D6 -- old-regime governance closure

Section 8. This cascade lives and closes UNDER THE OLD REGIME.

## 8. Governance-closure machinery [KIND: governance]

Ground live shapes at Phase 0 (#N14). Old-regime mutations only:
- **F-ledger**: seed **F-34** -- REGISTER_INVERSION Cascade B (corpus migration +
  align + arm + PS retirement + F-2 dissolution + the orphan enrollment), OPEN,
  architect-owned. Note in F-2's row (do not close it): "dissolves at F-34
  execution" appended to its resolution/owner cell.
- **REGISTER (old)**: enroll this brief (DOC-D, tier 3, Draft -> EXECUTED,
  rationale per convention); enroll `DOC-E-VALIDATOR_2_RECON_REPORT` (tier 3,
  EXECUTED, rationale); enroll the measure report (DOC-E, same convention) at its
  commit; the two new projects need NO register entries (code, not docs) but
  their `MODULE.md`s (if authored -- optional this cascade) would be orphans under
  the OLD validator: to keep `-Validate` warnings stable, either skip MODULE.md
  authoring until B or add them to the old `SCOPE_EXCLUSIONS.yaml` with a
  B-reverts note. LEAN: skip until B.
- `register_version` "2.24" -> "2.25"; EVT entry with real hashes; `-Validate`
  exit 0; render + header backfill at the final commit per the F-29/CODEX
  precedent.
- `FRAMEWORK.md` version 1.1.2 -> 2.0.0 recorded in its register entry + mirror.

## 9. S-LOCK invariants [CORE]

- **New S-LOCK candidate (armed at B): the derived-register integrity invariant**
  -- `REGISTER.yaml` and `CURRENT_AUTHORITY_SURFACE.yaml` are byte-reproducible
  from the corpus by `sync` (the regenerate-and-diff property). In A it exists as
  the idempotency test; B arms it as the closure-gate step.
- **The failability law joins TESTING_STRATEGY's meta-layer** (recorded in D1;
  formal TESTING_STRATEGY section landing deferred to the next methodology
  cascade with the queued lessons): no gate is trusted until proven
  red-once-then-green.

## 10. Census discipline [CORE]

New projects live under `tools/` and `tests/` -- zero `src/**/*.cs` touches
intended. HARD pins (34/13; DFK-WAIVER 2; marker families incl. `not yet` 10/9)
byte-identically unchanged; re-attest at closure; `CensusMetaTests` green within
the standard suite runs. New-code markers: do NOT introduce `TODO`/`not yet`/
`stub`-family vocabulary in the new projects' comments (they are outside `src/`
census scope, but keep the discipline anyway -- state intended-form
`Planned -- Cascade B` phrasing instead).

## 11. Commit plan [CORE]

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(register): enroll REGISTER_INVERSION_A brief + VALIDATOR_2 recon + validation checkpoint` | brief + recon report + refreshed VALIDATION_REPORT |
| C2 | `docs(framework): schema 2.0 -- the register inversion law (1.1.2 -> 2.0.0)` | D1 |
| C3 | `feat(governance-tool): DualFrontier.Governance skeleton + CPM YamlDotNet + sln wiring` | D2 shell: projects, config, RepoRoot, FM extraction (top + README-EOF) |
| C4 | `feat(governance-tool): validation core + gates + sync (report-only) with red/green fixtures` | D2 core + D3 suite; checkpoint 6.1 |
| C5 | `feat(governance-tool): migrate command + dry-run round-trip proof` | D4; checkpoint 6.2 |
| C6 | `docs(reports): REGISTER_INVERSION_A measure report (Cascade B work order)` | D5 + its enrollment; checkpoint 6.3 |
| C7 | `docs(roadmap): F-34 seeded (Cascade B); F-2 dissolution note` | ledger |
| C8 | `governance(register): REGISTER_INVERSION_A closure (2.24 -> 2.25)` | REGISTER mutations + EVT + validate folded |
| C9 | `governance(register): render regeneration + header backfill` | old-regime render precedent |

## 12. REGISTER cascade [CORE]

Old-regime, at C8/C9, Phase-0 verbatim shapes only (H5 on any absent enum value).
Document count 288 -> 291 (brief + recon report + measure report); EVT 47 -> 48;
`FRAMEWORK` mirror 2.0.0. Render assertion: statistics show 291 / 2.25; zero
`System.Collections.Hashtable` (the F-13 fix holds).

## 13. Halt conditions (H-series) [CORE]

- **H1** precondition mismatch. **H2** build/test regression vs baseline. **H3**
  `-Validate` nonzero. **H4** a live-code/NIH-reference fact contradicts a
  section-2 `[RV]` fact. **H5** absent enum value -- never invent.
- **H(preserve)** the C5 reconciliation shows ANY document lost, any field
  invented, or any delta beyond the ratified drops.
- **H(law)** a DF category-x-lifecycle prohibition needed for the gate catalog is
  NOT derivable from the grounded FRAMEWORK/corpus (i.e. the gate would be a
  tool-side invention) -- surface for architect ruling; never invent law (the NIH
  SC-8 principle: an unconstrained combination stays permissive).
- **H(governance)** any edit beyond the enumerated files; any live-corpus doc
  mutation by `migrate` (the flag guard failing is itself a halt); any NIH-tree
  write.
- Standing rails: no pushes; no `-Sync` outside sanctioned commits; single-writer
  ROADMAP/REGISTER/FRAMEWORK; NIH read-only.

## 14. Closure protocol and report [CORE]

Per METHODOLOGY 12.7. The closure report carries: the commits table (real
hashes); versions (FRAMEWORK 2.0.0, register 2.25); **the C5 round-trip
reconciliation summary** (288/288, globals 41/14/17/47, idempotency); **the C4
failability table** (every gate: red-fixture name + green-fixture name); the
measure-report headline numbers (Cascade B sizing); census pins re-attested;
`Skeleton revisions`; gates table (baseline vs closure -- the OLD regime must
still validate exit 0 and the full suite must be match-or-better); self-attestation
(no pushes; NIH untouched; live corpus doc-unchanged; single render); the
operator checklist (push; ratify the Cascade B brief authoring; the parked items
unchanged).

## 15. Out of scope [CORE]

- **The live-corpus migration, gate ARMING, PS retirement, F-2/orphan execution**
  -- all Cascade B (F-34).
- **CI (GitHub Actions regenerate-and-diff)** -- deferred (#N17); the idempotency
  property is built so CI can adopt it later without redesign.
- **The render's future** (keep-vs-retire `REGISTER_RENDER.md`) -- a Cascade B
  decision; untouched here beyond the old-regime C9.
- **TESTING_STRATEGY amendments** (the failability law's formal landing) -- the
  next methodology cascade, with the five queued lessons.
- **ISOLATION reconnection (DF-DOC-004 + F-23)** -- next in the program queue
  after B.
- **NIH back-port** of the DF hardenings (no-FM-is-ERROR) -- operator's separate
  NIH-side decision; noted, not executed.
- Pushes; the NIH tree; EXECUTED-doc content.

## Appendix A -- FRAMEWORK 2.0 load-bearing law text [KIND: governance]

The executor grounds the live FRAMEWORK and integrates these verbatim blocks as
the core of the D1 amendment (section numbering executor-grounded; prose seams
executor-authored; the LAW WORDING below lands verbatim).

=== A1: the inversion (the new register protocol core) ===
BEGIN
The register is INVERTED as of schema 2.0. The source of truth for every enrolled
document's governance metadata is the YAML frontmatter block in the document file
itself. `docs/governance/REGISTER.yaml` is a DERIVED, machine-generated ARCHIVE
index of the entire corpus -- every enrolled document in every lifecycle --
regenerated by the governance tool on every sync and never hand-edited.
`docs/governance/CURRENT_AUTHORITY_SURFACE.yaml` is a second DERIVED artifact: the
boot subset a working session loads as authoritative NOW, selected by the
authority predicate (a Live document, or a LOCKED document of tier 1 or 2),
deterministic and sorted, regenerated on every sync. The four global collections
-- requirements, risks, CAPA entries, and the audit trail -- remain first-class:
their sources of truth are the hand-edited files
`docs/governance/REQUIREMENTS.yaml`, `RISKS.yaml`, `CAPA.yaml`, and
`AUDIT_TRAIL.yaml` (append-only), validated by the tool and merged into the
derived archive register. Both derived artifacts carry generated-file headers and
are byte-reproducible from the corpus: an unchanged corpus yields byte-identical
output, which is the derived-register integrity invariant.
END

=== A2: the tool-law relation ===
BEGIN
The governance tool MIRRORS this framework's law and cites the governing section
in every rule it enforces; the tool never OWNS the law. A combination this
framework does not constrain stays permissive in the tool -- a tool-side
prohibition with no framework anchor is an invention and is forbidden. Every
machine gate must prove failability before it is trusted: a red result on a
synthetic violating fixture and a green result on a clean fixture, maintained in
the governance test suite. A gate that has never been red is presumed vacuous.
Gates deploy by measure -> align -> arm: they land REPORT-ONLY, the corpus is
aligned against their findings, and only then are they armed as exit-code
enforcing. A Markdown document inside the governed scope that carries no
frontmatter and is not on the explicit exclusion list is a validation ERROR, not
a warning: absence of governance metadata is a loud condition.
END

=== A3: the sentinel law ===
BEGIN
`next_review_due` carries exactly one of the sanctioned literal forms: 'null' (no
scheduled review -- mandatory for terminal lifecycles), an ISO-8601 date
(YYYY-MM-DD), a quarter label (YYYY-QN), or 'post-<event> closure'. The set is
CLOSED: a new form requires a ratified amendment of this section before the tool
accepts it.
END

---

**End of REGISTER_INVERSION_A_BRIEF.md v1.0**
