# PHASE GAMMA RECON REPORT — 2026-07-01

Pre-brief reconnaissance for A'.9.1 Phase γ severity promotion. Read-only session;
the only file created is this report (untracked — enrolled by the Phase γ cascade at C1
per the Phase β recon-enrollment precedent). Standing law cited, not restated:
census discipline `TESTING_STRATEGY` v2.0.0 §4; severity targets `ANALYZER_RULES` v0.2.2
§4.1 + `K_CLOSURE_REPORT` v1.0.1 §7.2; waiver law `CODING_STANDARDS` v2.1.0 §5.3.

---

## R1 Base state

- **Branch**: `main` (`.git/HEAD` → `ref: refs/heads/main`).
- **HEAD**: `b116727a2f1f318384fc94a5e62c689dc02c6cca` (`b116727` — `governance(register): render regeneration + header backfill`) — exactly the expected post-Phase-β closing commit; the operator landed nothing further.
- **`refs/heads/main`** = `b116727a…` (read from the loose ref file directly).
- **origin/main** (LOCAL ref only — **no fetch performed**, per session rule 2): `b116727a…` — local and remote-tracking refs identical; true remote divergence not re-measured.
- **Working tree**: clean at session start and clean after every build (`git status --porcelain=v1` → empty). Only delta at session end: this untracked report.
- **REGISTER.yaml** (read directly; `sync_register.ps1 -Validate` NOT run):
  - `register_version: "2.19"`, `last_modified: "2026-07-01"`, `last_modified_commit: "7d1fdb9"`.
  - Documents: **277** — census verbatim: Grep `^  - id: DOC-` on `docs/governance/REGISTER.yaml` → 277.
  - Audit events: **42** — census verbatim: Grep `^  - id: EVT-` → 42. (Combined `^  - id: (DOC|EVT)` → 319.)

## R2 Promotion-safety gate

**Verdict: PASS — 0 active DF diagnostics + 2 suppressed (the two Phase β DFK001 waivers). Promotion is build-safe; no pre-promotion blockers.** This re-confirms Phase β's closing state (REGISTER 2.19 closure event: «SARIF confirms 0 active + 2 suppressed»; post-triage line of `PHASE_BETA_VIOLATION_INVENTORY.yaml`).

**Method (zero tracked-file writes — command-line MSBuild property only):**

1. Warm pass (dependency outputs), PowerShell, per src project `<P>` (×12):
   `dotnet build src\<P>\<P>.csproj -c Release -v minimal --nologo` → 12 × «0 Error(s)».
2. SARIF log pass, bash, per src project `<P>` (×12), scratch = session scratchpad `…\scratchpad\sarif` (outside the repo):
   `dotnet build "src/<P>/<P>.csproj" -c Release --no-incremental -p:BuildProjectReferences=false "-p:ErrorLog=$scratch/<P>.sarif%2Cversion=2.1" -v quiet --nologo`
   - `--no-incremental` forces Csc + analyzers to run; `-p:BuildProjectReferences=false` makes each SARIF hold exactly that project's compile (deps prebuilt by the warm pass).
   - `%2C` is the MSBuild escape for a literal comma inside a command-line property value. **Method note (recorded for reuse):** a bare comma (`-p:ErrorLog=path,version=2.1`) is parsed by MSBuild as TWO properties (`ErrorLog=path` + stray `version=2.1`) — the first attempt therefore emitted SARIF 1.0.0 (no `suppressions` annotation); re-run with `%2C` emitted 2.1.0. Config = Release, mirroring Phase β C8/C9 (inventory header: «--no-incremental Release build», `ErrorLog=…,version=2.1`).
   - 12/12 builds succeeded; 12 SARIF files written to scratch only.
3. Parse (PowerShell `ConvertFrom-Json` over the 12 files): results filtered `ruleId -match '^(DFK|DFL|DF9)'`; suppression state = SARIF v2.1 `suppressions[].kind`; site = `locations[0].physicalLocation.artifactLocation.uri` + `region.startLine`.

**Per-rule result (17 rules × 12 projects):**

| Rule | Active | Suppressed |
|---|---|---|
| DFK001 | **0** | **2** — both `suppressions:[{kind:"inSource"}]`: `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs:94` and `:141` (the `Marshal.GetDelegateForFunctionPointer` lines inside the two waiver pragma blocks) |
| DFK002, DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK007_1, DFK011, DFK013, DFK015_1, DFK016, DFK017, DFK019_A, DFL025_A, DFL025_B, DF999 | **0 each** | 0 each |

Phase β's triaged rules all hold: DFK002 (13 sites relocated to `Core.Interop/NativeMethods.Bus.cs`) → 0; DFK007 (3 FP, refined) → 0; DFK017 (5 FP, refined) → 0.

**Non-DF context in the same SARIF** (out of Phase γ scope; recorded so the brief does not mistake it for DF surface): CS0618 ×2 `suppressed(inSource)` (`NativeWorld.cs:528/:529`, the pre-existing legacy-fallback pragma); SDK built-in analyzer Info notes ×340 active (SYSLIB1054 ×174, CA1510 ×90, CA1822 ×17, CA1859 ×15, CA1806 ×11, CA1512 ×10, CA1513 ×10, CA1401 ×3, CA2208 ×3, CA2249 ×2, SYSLIB1045 ×2, CA1854/CA1865/CA1868 ×1) — level `note`, unaffected by DF promotion, do not trip `TreatWarningsAsErrors`.

**Clean-tree confirm post-build:** `git status --porcelain=v1` → empty. The Runtime `CompileShaders` target regenerated `assets/shaders/*.spv` (6 tracked files) byte-identically — no dirt.

## R3 Severity matrix

Source of targets: `ANALYZER_RULES.md` v0.2.2 §4.1 «Phase-γ target¹» column (single registry), cross-checked against `K_CLOSURE_REPORT.md` v1.0.1 §7.2 (historical DF###/dotted IDs) and the ROADMAP «Analyzer track» Phase γ promotion map — all three agree. Current descriptor severity verified on disk: Grep `DiagnosticSeverity\.\w+` over `tools/DualFrontier.Analyzers/Rules/` → exactly 17 lines, all `defaultSeverity: DiagnosticSeverity.Info`.

Build-impact classes under `TreatWarningsAsErrors=true` (root `Directory.Build.props`, inherited by all 12 src projects): **BUILD-BREAKING** = Error or Warning (both fail the build); **IDE-ONLY** = Suggestion/Hidden.

| Rule ID | Current | Phase-γ target | Source | Build impact (TWAE) |
|---|---|---|---|---|
| DFK001 | Info | **Error** | §4.1; K_CL §7.2 DF001 | BUILD-BREAKING |
| DFK002 | Info | **Error** | §4.1; K_CL §7.2 DF002 | BUILD-BREAKING |
| DFK003 | Info | **Error** | §4.1; K_CL §7.2 DF003 | BUILD-BREAKING |
| DFK003_1 | Info | **Error** | §4.1; K_CL §7.2 DF003.1 | BUILD-BREAKING |
| DFK004 | Info | **Error** | §4.1; K_CL §7.2 DF004 | BUILD-BREAKING |
| DFK005 | Info | **Error** | §4.1; K_CL §7.2 DF005 | BUILD-BREAKING |
| DFK007 | Info | **Error** | §4.1; K_CL §7.2 DF007 | BUILD-BREAKING |
| DFK011 | Info | **Error** | §4.1; K_CL §7.2 DF011 | BUILD-BREAKING |
| DFK007_1 | Info | **Error** | §4.1; K_CL §7.2 DF007.1 | BUILD-BREAKING |
| DFK015_1 | Info | **Error** | §4.1; K_CL §7.2 DF015.1 | BUILD-BREAKING |
| DFK017 | Info | **Error** | §4.1; K_CL §7.2 DF017 | BUILD-BREAKING |
| DFK019_A | Info | **Warning** ² | §4.1 footnote ²; K_CL §7.2 DF019 (Warning, «V substrate contract, configurable»); F-12 ratified | BUILD-BREAKING |
| DFK013 | Info | **Warning** | §4.1 (efficiency); K_CL §7.2 DF013 | BUILD-BREAKING |
| DFK016 | Info | **Warning** | §4.1 (efficiency, retain α per Q-L-16); K_CL §7.2 DF016 | BUILD-BREAKING |
| DFL025_A | Info | **Warning** | §4.1 (discipline); Unshipped note «Warning post-promotion» | BUILD-BREAKING |
| DFL025_B | Info | **Suggestion** | §4.1 (discipline); Unshipped note «Suggestion post-promotion» | IDE-ONLY |
| DF999 | Info | **Warning** | §4.1 (self-policing, Q-L-18); Unshipped note | BUILD-BREAKING |

**Tally: 16 BUILD-BREAKING (11 Error + 5 Warning) / 1 IDE-ONLY (DFL025_B).** Every build-breaking rule measured 0 active in R2 — the tally carries no promotion risk at the current tree.

**DFK019_A = Warning CONFIRMED** in every living source (§4.1 row + footnote ², ROADMAP promotion map, K_CLOSURE §7.2 DF019 row). No living doc carries the stale blanket-Error reading — it survives only in the execution-context-tier brief §8.1 (the documented F-12 origin, subordinate per the deliberation-artifact taxonomy) and inside the F-12 ledger row's own description of the discrepancy.

## R4 Three severity surfaces

### R4.1 Descriptors (17 rule `.cs` files)

- Layout: `tools/DualFrontier.Analyzers/Rules/{Architecture ×9, NativeBoundary ×5, Discipline ×3}` (+2 non-rule helpers `ReservedStubAnalysis.cs`, `SanctionedInteropSurface.cs`).
- Form (exemplar DFK019_A, representative of all 17): `new DiagnosticDescriptor(id: DiagnosticId, title, messageFormat, category: "DualFrontier.<Category>", defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true, description, helpLinkUri: "…/ANALYZER_RULES.md#<id-lowercase>")`.
- ID consts underscore-form ×17 verified: Grep `DiagnosticId = ` over `Rules/` → `"DFK001"…"DFK019_A", "DFL025_A", "DFL025_B", "DF999"` (matches ANALYZER_RULES §4 naming convention, v0.2.2 adjudication).
- **Severity is declared in the descriptor ONLY today.** No config-side severity exists anywhere: `.editorconfig` carries zero severity keys (R4.3); Glob `**/*.globalconfig` → 0 files; no ruleset files. There is no existing DF convention of editorconfig-declared severity to follow — Phase γ creates whichever surface it ratifies.
- **The design fork (brief decides):**
  - (a) **descriptor-as-truth** — flip `defaultSeverity` in the 17 descriptors to the R3 targets; release tables must move in the same change (R4.2 coupling).
  - (b) **editorconfig-as-truth** — descriptors stay Info; 17 `dotnet_diagnostic.<ID>.severity` keys carry enforcement.
  - (c) **both aligned**.
  - Written intent so far leans editorconfig: brief §8.1 («single .editorconfig edit»), ROADMAP Phase γ («per-rule severity lines land in .editorconfig»), and the `src/Directory.Build.props` wiring comment («per-rule severity tunable via .editorconfig at cleanup-phase entry»). But those texts presuppose a Phase β editorconfig `suggestion` baseline that was **never primed** — Phase β ran detection from descriptor-Info directly. Note also the release-table semantic: the Shipped «Severity» column records the **descriptor** `defaultSeverity`; an editorconfig-only promotion ships a table that reads `Info` forever while behavior is Warning/Error — legal under RS tracking but a truth-law smell. Recommendation shape in R7.

### R4.2 AnalyzerReleases (RS release tracking)

- **`AnalyzerReleases.Unshipped.md`**: all 17 rules at Info. Exact table format (verbatim):
  ```
  ### New Rules

  Rule ID  | Category                    | Severity | Notes
  ---------|-----------------------------|----------|-------
  DFK003   | DualFrontier.Architecture   | Info     | К-L3 storage ownership — native owns ECS storage. [Documentation](…/ANALYZER_RULES.md#dfk003)
  ```
  …one row per rule, underscore IDs, each Notes cell = К-L/Lesson anchor + `[Documentation]` link to the rule's ANALYZER_RULES anchor. Rows: DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017 (Architecture); DFK001, DFK002, DFK007_1, DFK015_1, DFK019_A (NativeBoundary); DFL025_A, DFL025_B, DF999 (Discipline). Header comment records the β1/β2/β3 provenance.
- **`AnalyzerReleases.Shipped.md`**: 5 comment lines, **no table** — «Empty at A'.9.1 Phase β-prep. First release entry added at A'.9.1 closure (Phase γ severity promotion + Phase δ governance cascade) when ruleset ships.»
- **Transition on shipping** (per the release-tracking help doc linked in both file headers): all 17 move Unshipped → Shipped under a `## Release <version>` heading with a `### New Rules` table (first shipping = New Rules — nothing has ever shipped, so no Changed Rules block), Severity column = descriptor `defaultSeverity` at ship time; Unshipped.md returns to header-only.
- **Tracking is ACTIVE**: `Microsoft.CodeAnalysis.Analyzers` referenced (`PrivateAssets="all"`, `IncludeAssets` includes `analyzers`) + both files wired as `<AdditionalFiles>` (csproj comment: «RS2008 release tracking infrastructure») — RS2008 (enable-tracking) is satisfied by the files' presence, and the same release-tracking rule family (RS2000–RS2005; the stale-entry member is RS2001 «ensure up-to-date entry») raises diagnostics on any descriptor↔table mismatch. The analyzer project inherits `TreatWarningsAsErrors=true`, so **a descriptor severity flip without the matching release-table move fails the analyzer build mechanically** — descriptor and release surfaces are same-commit-coupled under fork (a)/(c).

### R4.3 `.editorconfig`

- Exactly one `.editorconfig` in the repo (Glob `**/.editorconfig` → 1, at root). Full current content (4 lines, verbatim): `root = true` / blank / `[*]` / `charset = utf-8`.
- **Zero** `dotnet_diagnostic.*` lines of any kind (DF or non-DF), no `[*.cs]` section — unprimed, exactly as the Phase β recon found. Grep `dotnet_diagnostic` over `.editorconfig` → 0.
- Landing shape for the DFK keys: a new `[*.cs]` section with 17 `dotnet_diagnostic.<ID>.severity = <error|warning|suggestion>` lines. A root-level placement is effectively src-scoped anyway: the DF analyzer only loads where it is wired (`src/Directory.Build.props`, the 12 src projects) — the keys are inert for tests/ and tools/ builds.
- **Underscore-ID dividend (Phase β normalization)**: `dotnet_diagnostic.DFK019_A.severity` is a clean, unambiguous key; the old dotted form would have produced `dotnet_diagnostic.DFK019.A.severity` — an ambiguous key path (and the dotted ID is rejected by Roslyn `ReportDiagnostic` regardless, per ANALYZER_RULES §4 naming note).

## R5 Waiver-under-promotion

- **Sites (2, both DFK001, `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs`)**:
  - `:90` `// DFK-WAIVER(DFK001): sanctioned Vulkan debug-messenger interop (VK_EXT_debug_utils, К-L19) …` + `:93/:95` `#pragma warning disable/restore DFK001` (around `Marshal.GetDelegateForFunctionPointer<CreateDebugUtilsMessengerDelegate>`);
  - `:137` same marker text + `:140/:142` disable/restore (destroy-path delegate).
  - Both carry the CODING_STANDARDS §5.3 rule-3 authority class «sanctioned architectural exception» citing К-L19 / VK_EXT_debug_utils — citations intact.
- **Census verbatim (TESTING_STRATEGY §4.3 expressions)**:
  - `rg '#pragma warning disable (DFK|DFL|DF9)'` over `*.cs` → 3 raw matches = the 2 real waiver pragmas (`ValidationLayer.cs:93/:140`) + 1 **string literal inside the census meta-test itself** (`tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs:150` — assertion message text, not a pragma). src-scoped truth = **2**, matching the compiled pin: `CensusMetaTests.DfkWaiverCensus_MatchesPin` (corpus = `git ls-files -- src` `*.cs`) asserts `disables == 2` and `markers == disables`, with the К-L19/VK_EXT_debug_utils citation in its comment. **Census pin (2) intact.**
  - `rg 'SuppressMessage' src/ tests/` → 0 in `src/`; 6 in `tests/` — all string literals/test fixtures (the meta-test + DF999 positive-case source strings), zero attribute usage in production.
  - `rg --files -g 'GlobalSuppressions.cs'` → none.
- **Pragma suppression is severity-agnostic**: `#pragma warning disable DFK001` suppresses by diagnostic ID at ANY configured severity (Info, Warning, or Error) — the diagnostic is reported as suppressed-in-source and never reaches TWAE. Empirically corroborated in this session's own SARIF: the CS0618 pair is an **error-level** diagnostic (obsolete-usage under TWAE) and is fully neutralized by the `NativeWorld.cs` pragma — the identical mechanism DFK001 relies on post-promotion. SARIF cross-check: both DFK001 results carry `suppressions:[{kind:"inSource"}]`, 0 active. **The brief does not need to re-triage the waived sites at promotion.**

## R6 Citation anchors + F-12

| Anchor | Exists | Form / what it says |
|---|---|---|
| `ANALYZER_RULES.md` §4.1 Phase-γ column | YES (v0.2.2, DOC-A-ANALYZER_RULES, AUTHORED-SKELETON) | «Phase-γ target¹» column in the single registry; footnote ¹ (target is spec, not current behavior; sequencing owned by ROADMAP); footnote ² (registry-drift resolution → DFK019_A Warning per canonical K_CLOSURE §7.2) |
| `K_CLOSURE_REPORT.md` §7.2 | YES (v1.0.1, line 1660) | 18-row active-rules table in the OLD DF###/dotted IDs (DF001…DF019, incl. the deferred DF009/DF010/DF012/DF015/DF018) — historical record; underscore forms current per ANALYZER_RULES §4. DF019 row = **Warning** |
| `CODING_STANDARDS.md` §5.3 DFK-WAIVER | YES (v2.1.0 LOCKED) | Waiver form + 6 rules + GlobalSuppressions ban. **Defect flag**: the §5.3 «Diagnostic-ID form» paragraph still mandates dotted/hyphen IDs (`DFK003.1`, `DFL025-A`) — superseded by the Phase β adjudication; see R7 anomaly 1 |
| F-12 ledger entry, `docs/ROADMAP.md:972` | YES | Verbatim: «F-12 \| DFK019.A Phase-γ severity target discrepancy: K_CLOSURE §7.2 + ANALYZER_RULES v0.1 §10.1 say Warning; A'.9.1 brief §8.1 blanket promotion reads Error. v0.2.0 registry records Warning per the canonical Tier-1 source, footnoted in ANALYZER_RULES §4.1 + ROADMAP «Analyzer track» \| S2 \| OPEN \| Crystalka ratifies before Phase γ promotion executes». Ledger still S2 OPEN; the ratification (DFK019_A = Warning) is recorded in this kickoff — the OPEN → CLOSED flip is Phase γ cascade work, citing the ratification + this recon |
| `docs/reports/PHASE_BETA_VIOLATION_INVENTORY.yaml` | YES | The triage-resolved baseline: 23 raw (Q-L-1 gate CONTINUE), totals DFK002 13 / DFK017 5 / DFK007 3 / DFK001 2; dispositions relocate / refine / waiver — re-confirmed by R2 at 0 active + 2 suppressed |
| A'.9.1 brief §8 (`tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md:1801`) | YES | §7.4 Phase β closure gate (4 conditions + Q-L-1 adaptive gate) precedes it; §8.1 promotion mechanics = «single .editorconfig edit» suggestion → error with DFK013/DFK016 → warning exceptions (the superseded blanket-Error text — F-12's origin) + verification bullets (build exit 0; Modding suite + analyzer tests green; no unjustified pragma); §8.2 Phase δ closure deliverables. Execution-context tier: its severity map is superseded by §4.1 (11 E / 5 W / 1 S); its verification bullets remain usable |
| ROADMAP «Analyzer track» (`docs/ROADMAP.md:848`; Phase γ at `:873`) | YES | Phase γ subsection: editorconfig lines land (file currently charset-only); promotion map matching §4.1; **exit gate** «`dotnet build` exit 0 with every correctness DFK### at `error` — the moment К-Lxx compile-time enforcement goes live»; Shipped.md first entry at closure |

No dangling citations found — every anchor the Phase γ brief must cite exists in citable form.

## R7 Anomalies + scale/promotion estimate

**Build-safety verdict: 0 active (R2) → promotion is build-safe.** Phase γ is a severity-flip + release-transition + editorconfig-prime + F-12/ledger closure + REGISTER closure. No pre-promotion triage required.

**Risk tally (R3)**: 16 BUILD-BREAKING / 1 IDE-ONLY; all 16 measured at 0 active. The 2 waived DFK001 sites are severity-immune (R5).

**Anomalies (none blocking; all are brief-scope amendment candidates):**

1. **`CODING_STANDARDS.md` §5.3 «Diagnostic-ID form» is a living-law defect** (lines ~449–451): it mandates dotted/hyphen waiver IDs (`DFK003.1`, `DFL025-A`). Post-adjudication descriptor IDs are underscore; a waiver written to §5.3's letter for a sub-rule (`#pragma warning disable DFK003.1`) would suppress NOTHING (invalid ID). The two existing waivers use plain `DFK001` and are unaffected. Amend at Phase γ (LOCKED doc — amendment discipline applies).
2. **`RESERVED_SURFACE_MUTABILITY.md` lines 57–58** assert `DiagnosticId = "DFK003.1"` / `"DFL025-A"` («underscore file/class form, dot/hyphen descriptor form») — factually false since the Phase β normalization; drift.
3. **`TESTING_STRATEGY.md` §4.3** second census expression («`[SuppressMessage]` over src/ and tests/ returns 0») is no longer literally true for `tests/` — DF999 test fixtures + the meta-test contain the string as literals; the compiled meta-test corpus is src-only. Precision amendment candidate (attribute-usage vs string-literal), not a violation. Same section's «Baseline: 0» is a dated historical statement — the current pin (2) lives in the meta-test + REGISTER closure event; a 0 → 2 note belongs in the doc when next touched.
4. **`ANALYZER_RULES.md` in-body version self-references lag**: body header «**Version**: 0.2.1» (line 16) and end-marker «End of ANALYZER_RULES.md v0.2.0» (line 313) vs frontmatter/change-history 0.2.2. Cosmetic sync at Phase γ.
5. **Dotted→underscore living-doc migration surface** — sweep verbatim: `rg -n 'DFK[0-9]{3}\.[0-9]|DFK019\.A|DFL025-A|DFL025-B' docs/ src/ tests/ tools/DualFrontier.Analyzers/ -g '!*.yaml'` → 45 lines / 12 files. Living docs still citing dotted forms: `ROADMAP.md` ×4 (Analyzer-track Phase β/γ rule lists + promotion map + F-12 row), `CODING_STANDARDS.md` ×2 (§5.2-adjacent DFL025-A mention line 386 + the §5.3 law of anomaly 1), `TESTING_STRATEGY.md` ×2 (lines 148, 284), `RESERVED_SURFACE_MUTABILITY.md` ×2 (anomaly 2), `METHODOLOGY.md` ×1 (Lesson #N17 narrative, incl. DFK019.A/.B), `ANALYZER_RULES.md` ×6 (self-aware: the naming-convention supersession note + DFL025-C exclusion + DFK019.B deferral — mostly legitimate mentions-as-superseded). Historical/execution-tier (LEAVE per taxonomy): K_CLOSURE §7.2, `A_PRIME_9_0_AMENDMENTS_LOG` ×5, `PHASE_A_PRIME_SEQUENCING` ×2, `docs/prompts/PHASE_BETA_PREP_EXECUTION_PROMPT` ×18, brief §8.1. Code comments (cosmetic): DFK015_1 analyzer ×1, analyzer csproj ×1, `ReservedStubAttribute.cs` XML doc ×1. «DFK019.B» names a deferred future rule with no descriptor — normalize to `DFK019_B` opportunistically when touched, not a defect today.
6. **Non-DF Info noise** (R2): 340 SDK-analyzer notes (SYSLIB1054 ×174 dominates — DllImport → LibraryImport suggestions across the interop surface) + CS0618 ×2 suppressed. Pre-existing, promotion-inert, out of scope.
7. **No WIP found**: clean tree, HEAD = pushed origin/main (local refs), no stray branches touched, no divergence signals.

**Design-fork recommendation shape (T4.1, for the brief to ratify):** flip the **descriptors** to the ratified severities (single source of truth; the Shipped.md table then records the true shipped severities — the RS tracking family enforces descriptor↔table sync mechanically under TWAE) **and** prime `.editorconfig` with the same 17 keys as the explicit, greppable enforcement statement + operator-tuning surface (fork (c), both-aligned). The editorconfig-only alternative (b) matches the older written intent (brief §8.1 / ROADMAP wording) but leaves Shipped.md permanently reading `Info` while behavior is Error/Warning — a truth-law smell this project's governance would have to footnote forever. Whichever fork is ratified, ROADMAP's «suggestion → error» promotion-map wording needs refreshing to the actual mechanism (no `suggestion` baseline ever existed).

**Proposed commit-class split (serial; wave topology unnecessary — the three surfaces are per-rule coupled and the total diff is small):**

1. **C1** — recon enrollment (this report) per Phase β precedent.
2. **C2** — the promotion commit: descriptor `defaultSeverity` flip ×17 **+ AnalyzerReleases Unshipped → Shipped transition in the same commit** (RS-coupled under TWAE, R4.2) — or editorconfig-first if fork (b) is ratified.
3. **C3** — `.editorconfig` priming (`[*.cs]` + 17 `dotnet_diagnostic.<ID>.severity` keys).
4. **C4** — verification class: full Release+Debug sweep + analyzer tests at promoted severities — the ROADMAP exit gate («build exit 0 with every correctness DFK### at error») + brief §8.1 verification bullets.
5. **C5** — living-doc alignment riders: F-12 S2 OPEN → CLOSED; CODING_STANDARDS §5.3 ID-form amendment; RESERVED_SURFACE_MUTABILITY §57–58; TESTING_STRATEGY §4.3 precision; ROADMAP Analyzer-track dotted-ID refresh + promotion-wording refresh; ANALYZER_RULES version-marker sync (may split per amendment discipline — two LOCKED Tier-1 docs are touched).
6. **C6** — REGISTER closure (2.19 → 2.20 + EVT + render regeneration).

Scale estimate: one serial cascade, ~6–8 commits, single session. The only hard coupling is descriptor-flip ↔ release-tables (same commit); everything else is free-standing.

## R8 Self-attestation

- **Zero writes except the report file at `docs/reports/PHASE_GAMMA_RECON_REPORT.md`**: CONFIRMED — `sync_register.ps1 -Validate` NOT run (REGISTER.yaml read directly); ErrorLog passed as a command-line MSBuild property (`%2C`-escaped) to the session scratchpad outside the repo; no tracked file edited or created; post-build `git status --porcelain=v1` empty; the only repo delta at session end is this untracked report.
- **Report written to docs/reports/ AND presented in chat (uncommitted)**: CONFIRMED.
- **Zero git mutations**: CONFIRMED — only read commands (`status`, `rev-parse`, `log`, `ls-files`, `diff --stat`); `.git/HEAD` + `refs/heads/main` read as files; NO fetch (origin/main from local ref, stated in R1).
- **Every census/MSBuild expression verbatim**: CONFIRMED — recorded inline beside each count (R1 register censuses; R2 build + parse expressions incl. the comma-escape method note; R3 descriptor-severity grep; R4 editorconfig/globalconfig probes; R5 §4.3 census expressions; R7 dotted-ID sweep).
- **Active-violation counts measured (not assumed)**: CONFIRMED — SARIF 2.1.0 parsed across all 12 src projects: **0 active + 2 suppressed(inSource)**, per-rule table in R2.
