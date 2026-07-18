---
register_id: DOC-D-STACK_UPDATE_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-17'
last_modified: '2026-07-17'
content_language: en
next_review_due: null
title: STACK_UPDATE execution brief (net10.0 + C# 14 + C++23 amendment + package hygiene + AwesomeAssertions + doc census)
last_review_event: 'Enrolled EXECUTED at cascade closure 2026-07-17 (EVT-2026-07-17-STACK_UPDATE). Executed in the cloud Linux sandbox per the operator ruling recorded in DOC-E-LINUX_SANDBOX_ENV_BASELINE_REPORT; all D1-D8 deliverables landed, Phase G without escape; closure report DOC-E-STACK_UPDATE_CLOSURE_REPORT.'
reviewer: Volodymyr (Crystalka)
---

# STACK_UPDATE -- Execution Brief

One large Fable 5 session, deliberately: the whole toolchain update happens in a single
executor context so that update-induced errors are fixed WITH the context of what was just
changed. The cascade: measure the real warning-wave delta empirically; bump the TFM
`net8.0 -> net10.0` (LangVersion held); raise C# to 14; test-triad package hygiene; add the
ratified `global.json` pin; amend К-L1 to C++23 (invariant amendment, FRAMEWORK section 7.2);
migrate FluentAssertions -> AwesomeAssertions 9.4.0 behind an escape hatch; then a single
writer subagent updates the version facts across the doc corpus point-wise per the recon
census. Done means: every project builds and tests green on net10.0/C#14/C++23 under
`TreatWarningsAsErrors`, the license freeze is gone (or honestly escaped to an F-entry), the
corpus states the new stack truthfully, armed validate exit 0.

**Basis**: the 2026-07-17 chat-only STACK VERSION RECON (operator-sanctioned zero-write
deviation; its R1-R4 facts are cited below as REC-*) + operator rulings of 2026-07-17.
**Executor**: Claude Code (Fable 5), LOCAL. **Branch**: off `main` (= `bc93241` at authoring).
**Register regime**: schema 2.0 -- frontmatter SoT; mutation = frontmatter edit +
`dotnet run --project tools/DualFrontier.Governance -- sync` same commit; gate =
`validate --armed` exit 0; closure EVT = `AUDIT_TRAIL.yaml` append; derived registers never
hand-edited; PENDING-* outlawed.

**Brief-integration notice**: standing law cited by anchor, never restated; brief-vs-standing-
doc conflict = brief is wrong (halt); brief-vs-code conflict = code is true (record, proceed).

## 1. Mission [CORE]

| # | Deliverable | Action |
|---|---|---|
| D1 | `net10.0` corpus-wide (LTS until 2028-11-14) | TFM bump, mechanical diagnostic fixes in-context |
| D2 | C# `14.0` + analyzer LangVersion pinned explicit | staged after TFM-green |
| D3 | Test-triad hygiene: xunit 2.9.3, runner 3.1.5, Test.Sdk 18.8.1 | one commit each |
| D4 | `global.json` pin (Q1-beta ratified) + DEVELOPMENT_HYGIENE 206 amendment | deliberation executed |
| D5 | **К-L1 amended to C++23** via `/std:c++23preview` (section 7.2 protocol) | invariant amendment + CMake + native gates |
| D6 | **AwesomeAssertions 9.4.0** replaces FluentAssertions 6.12.1 (Apache-2.0 restored) | terminal phase, ESCAPE HATCH sanctioned |
| D7 | Doc census executed: version facts updated point-wise (REC-R4 worklist) | writer subagent pass |
| D8 | Closure: 2nd AUDIT_TRAIL append, ROADMAP write-back + new F-entries | -- |

## 2. Established facts (REC-* = recon; (R) = re-verify at Phase 0) [CORE]

- (R) `main` = `bc93241`, clean; register schema 2.0; surface 134 = 104 Live + 30 LOCKED;
  armed validate exit 0; tests 1166/0/5 (5 = F-30/F-31 skip-guards); governance suite 64/64;
  F-40 single-test cross-suite flake (`UnloadModNativeState_VacuousUnload_Succeeds`,
  isolated-pass) is a RECORDED KNOWN FLAKE, not an H2 regression.
- REC-R1: sole SDK **10.0.204**, no `global.json`, no NuGet.config; default TFM `net8.0`
  (`Directory.Build.props:3`), LangVersion `12.0` pinned (`:4`), TWAE true (`:7`);
  AnalysisLevel/WarningLevel set NOWHERE; analyzer `netstandard2.0` + `LangVersion latest`;
  governance tool + tests ALREADY `net10.0` green with the full test triad -- the decisive
  anchor: **the TFM bump needs zero package moves**.
- REC-R2/R3: .NET 8 EOL **2026-11-10**; .NET 10 = sole LTS target. AnalysisLevel tracks the
  SDK (CA rules already at 10, green today); WarningLevel tracks the TFM (bump enables waves
  9-10 under TWAE) -- BUT two official pages conflict, so the realized delta MUST BE MEASURED
  (Phase A). Marquee risks measured NO SURFACE: CS9265 (zero `ref` fields in src/), C#14 span
  overload change (zero `Expression<` in src/), DllImportSearchPath (comment-only), publish
  single-file/trim (absent).
- REC native: CMake 4.2.3 (VS-bundled), generator `Visual Studio 18 2026`, toolset v145 /
  MSVC 14.50.35717, `/std:c++20` emitted via `stdcpp20`, Vulkan SDK 1.4.350.0 = current,
  glslang 16.2.0. **MSVC fact (web-verified)**: the stable `/std:c++23` switch DOES NOT EXIST
  yet -- C++23 today = `/std:c++23preview` (or `latest`); stable switch arrives with Build
  Tools 14.52 (VS 2026 Insiders first). Microsoft: preview features may change and may not be
  ABI compatible across releases. MITIGATION accepted by the operator: the kernel's external
  boundary is the C ABI (`extern "C"`, IAC section 3) -- C++ standard-mode ABI does not cross
  it; the toolset is pinned by the installed VS.
- AwesomeAssertions (web-verified): community fork, Apache-2.0 permanent, latest **9.4.0**;
  since 9.0.0 namespaces renamed `FluentAssertions` -> `AwesomeAssertions`. Migration from FA
  6.12.1 therefore crosses the FA 6->8 API delta PLUS the using-sweep -- real risk on ~13
  suites, hence the escape hatch.
- FluentAssertions 8.x is a PAID commercial license -- never bump the FA package id.
- REC-R4 doc census: 11 live files / 53 sites; two hand mirrors (CODING_STANDARDS 4.2+4.3;
  TESTING_STRATEGY:49); highest-risk-if-missed: MODDING:215-216 (external copy-paste
  `-f net8.0`), DEVELOPMENT_HYGIENE:206, VULKAN_SUBSTRATE:638 (only live VS floor).
  KERNEL/FIELDS Vulkan-1.3 sites and K_CLOSURE canon do NOT move. `historical/` + EXECUTED
  reports + `tools/briefs/**` + REGISTER_RENDER.md: LEAVE wholesale.
- Operator rulings 2026-07-17 (binding): Q1-beta `global.json` ADD with pin; Q2 LangVersion
  14 in-cascade, staged; Q3 scope as in section 1; AA terminal phase WITH escape hatch;
  **C++23 amendment in-cascade** (operator wrote "22" -- read as 23 per the deliberation;
  no C++22 exists; confirm-or-halt at Phase 0 if the operator objects); big single session +
  doc subagent pass.

## 3. Phase 0 -- preconditions [CORE]

1. Verify every (R) fact; mismatch -> H1. 2. Baseline gates: full build, full test run,
native selftest, governance suite; record shapes verbatim (F-40 carve-out stated). 3.
`validate --armed` exit 0 -> else H3. 4. Frontmatter-shape read (FRAMEWORK 14.3/14.4 + one
LOCKED doc + one AUDIT_TRAIL entry as append template). 5. Mandatory reads: this brief; the
recon report content (in the dispatch context); FRAMEWORK sections 7.2 + 14; KERNEL Part 0
К-L1 row + falsifiability clause; DEVELOPMENT_HYGIENE 205-206; METHODOLOGY closure protocol.
The executor NEVER pushes; `historical/` read-only; derived registers never hand-edited.

## 4. Topology [CORE]

Single orchestrator, strictly serial, Phases A-G -- the operator's ratified design: the
updating context and the error-fixing context are ONE context. Phase H may spawn ONE writer
subagent for the doc edits (disjoint from governance files); ROADMAP.md, AUDIT_TRAIL.yaml,
and all commits are orchestrator-only.

## 5. Execution phases [CORE]

**Phase A -- MEASURE (mandatory empirical step).** In the working tree (no commit): set
`Directory.Build.props` TFM to `net10.0`, full build, CAPTURE the complete diagnostic delta
verbatim, then `git checkout -- .` to restore. Classify each diagnostic: MECHANICAL (fix
preserves semantics exactly: cast, rename, direct API equivalent, suppression-with-DFK-WAIVER
is NOT allowed here) vs SEMANTIC (any fix that would change behavior, touch К-L-relevant
logic, or require design). Any SEMANTIC item -> **HALT H-M** with the inventory; the operator
rules per item before Phase B. All-mechanical (expected per REC) -> proceed.

**Phase B -- TFM bump.** `net8.0 -> net10.0`, LangVersion HELD at `12.0`; fold the mechanical
fixes into the same atomic commit (the commit must be green under TWAE); commit body carries
the measured delta summary. Full gates.

**Phase C -- language.** LangVersion `12.0 -> 14.0` + pin the analyzer csproj LangVersion to
an explicit version (Microsoft: `latest` is machine-dependent). Own commit, full gates.

**Phase D -- test-triad hygiene.** Three commits, full test run each: xunit `2.9.3`;
xunit.runner.visualstudio `3.1.5`; Microsoft.NET.Test.Sdk `18.8.1`. NOT in scope: Roslyn
5.6.0, BenchmarkDotNet, YamlDotNet, xunit.v3 (deferred F-entries, section 8).

**Phase E -- global.json (ratified deliberation).** Add repo-root
`global.json`: `{ "sdk": { "version": "10.0.204", "rollForward": "latestFeature" } }`
(reproducibility: the commit history is the research dataset). Same commit amends
DEVELOPMENT_HYGIENE 205-206: the SDK statement updates and the prohibition sentence is
replaced by the record that the deliberation occurred (cite this cascade's EVT id). Doc
frontmatter maintenance + sync.

**Phase F -- К-L1 amendment to C++23 (FRAMEWORK section 7.2).** (1) KERNEL_ARCHITECTURE.md:
amend the К-L1 row, its falsifiability clause, and every Part 0 / section-4 C++20 site
(REC census: lines ~73,77,79,81,440,530 -- re-grep, the census expression recorded) to:
C++23 via `/std:c++23preview` -- with the amendment text HONESTLY recording the preview-flag
status and the C-ABI containment rationale; version bump lean MINOR `1.0.1 -> 1.1.0`
(ground against section 7.2 + existing amendment precedent; deviation = record). ARCHITECTURE.md:49
`C++20 -> C++23` (PATCH). (2) `CMakeLists.txt`: `CMAKE_CXX_STANDARD 20 -> 23` (REQUIRED ON,
EXTENSIONS OFF unchanged). (3) Regenerate and VERIFY THE EMITTED FLAG empirically in the
vcxproj (the recon pattern): the emitted mode MUST be `/std:c++23preview` (`stdcpp23preview`
or equivalent). CMake emitting `/std:c++latest` or anything else -> **HALT H-CXX** (latest
tracks C++26-in-progress -- not the ratified target; do not improvise a flag override without
the operator). (4) Full native selftest + full managed run (interop). (5) F-entry seeded:
migrate to the stable `/std:c++23` switch when Build Tools 14.52 becomes default (section 8).

**Phase G -- AwesomeAssertions (terminal, escape hatch).** Swap package id
`FluentAssertions 6.12.1 -> AwesomeAssertions 9.4.0` (Directory.Packages.props + every
PackageReference), using-sweep census `rg -n "using FluentAssertions" tests/` (record
before -> 0 after; sweep to `using AwesomeAssertions` incl. sub-namespaces), fix API breakage
ONLY where mechanical (renamed member, moved type, equivalent call). Full test run: 1166+
pass required. **ESCAPE**: if any suite needs a SEMANTIC test rewrite (assertion logic
change, behavioral re-specification), REVERT the whole phase (`git checkout` -- no partial
migration), record the measured breakage inventory verbatim, seed the F-entry, and close the
cascade without it. A reverted Phase G is a CLEAN outcome, not a failure.

**Phase H -- doc census (writer subagent).** Update the REC-R4 worklist to the FINAL landed
state (post-G truth: mirrors show AwesomeAssertions 9.4.0 only if Phase G landed):
CODING_STANDARDS 4.2 (net10.0, LangVersion 14.0) + 4.3 mirror rows; TESTING_STRATEGY:49
mirror sentence; ANALYZER_RULES:96 + the one-sentence `netstandard2.0` gap fix (the analyzer
TFM asserted on the LOCKED surface -- REC finding); MODDING:215-216 `-f net10.0`;
README stack lines; VULKAN_SUBSTRATE:638 VS floor -> `Visual Studio 2026 (18.0)+`;
PIPELINE_METRICS:256 annotation (ratified: edit the annotation, precedent exists; the frozen
body untouched); RESERVED_SURFACE_MUTABILITY:144 token `net8.0 -> net10.0`; adjacent:
`native/DualFrontier.Core.Native/build.md`, `src/**/MODULE.md`, example-mod README.
Every doc edit: body + frontmatter maintenance (PATCH bump, last_review_event naming this
cascade) + `sync` in the same commit. Group into 2-4 atomic commits by cluster. LEAVE:
historical/, EXECUTED reports, tools/briefs/**, REGISTER_RENDER.md, METHODOLOGY:1155.

**Closure.** ROADMAP write-back: forward-state (stack = net10.0/C#14/C++23preview; EQ queue
unblocked -- EQ-a next); F-entries per section 8; EVT append to AUDIT_TRAIL.yaml (2nd append;
prior entries byte-unchanged); brief frontmatter -> EXECUTED; `validate --armed` exit 0;
closure report per section 9.

## 6. Halt catalog [CORE]

H1 precondition mismatch. H2 gate regression beyond the F-40 shape. H3 armed validate
nonzero. **H-M** any SEMANTIC diagnostic in the Phase A measurement. H5 frontmatter/sentinel
outside FRAMEWORK 14.3/14.4. H6 any src/native semantic change beyond the mechanical class,
or any К-L-relevant behavior touch. **H-CXX** emitted C++ flag is not `c++23preview`-class,
or native selftest regression in Phase F. Phase G ESCAPE is a sanctioned revert path, not a
halt. Standing rails: no pushes; no history rewrite; historical/ read-only; derived registers
never hand-edited; AUDIT_TRAIL append-only; DFK-WAIVER suppressions FORBIDDEN as diagnostic
fixes (a needed waiver = H-M item).

## 7. Census discipline [CORE]

DFK-WAIVER HARD pin: `rg -n "DFK-WAIVER" src/` = 2 unchanged at closure. Phase F C++20-site
census and Phase G using-census recorded with verbatim expressions before/after. SOFT: the
version-vocabulary counts move by design (that is the cascade); record the R4-worklist
completion table (site | done/left) instead of a delta ceremony.

## 8. F-ledger actions at closure [CORE]

Seed: F-44 stable `/std:c++23` migration when Build Tools 14.52 defaults (tracks the К-L1
preview caveat); F-45 xunit v2 -> v3 (package-id change, Exe test projects -- own cascade);
F-46 Roslyn 5.3.0 -> 5.6.0 + BenchmarkDotNet + YamlDotNet hygiene bundle (no pressure);
F-47 LINQ convention documentary-not-enforced (`SpatialGrid.cs:3` + no analyzer rule -- REC
finding; candidate DFK rule); F-48 AA migration IF Phase G escaped (with the breakage
inventory). Numbers are intended-form -- take the next free F-ids from the live ledger.

## 9. Closure report [CORE]

Commits table; versions table (TFM/LangVersion/packages/CMake-emitted-flag/К-L1 before ->
after); the MEASURED Phase A delta verbatim; gates baseline vs closure; census results;
Phase G outcome (landed | escaped + inventory); F-ledger finals; skeleton revisions;
self-attestation (no pushes; sync every frontmatter commit; single AUDIT_TRAIL append with
priors byte-unchanged; historical/ untouched; no DFK-WAIVER added; live-flag n/a); operator
checklist (push + merge; EQ-a chartering next; G-RATIO deliberation standing).

## 10. Out of scope [CORE]

Vulkan (already current; К-L19 untouched); CMake minimum; MSVC/VS; glslang; FluentAssertions
package id in ANY version (paid license); xunit.v3; Roslyn/BDN/YamlDotNet bumps; EQ-a..EQ-d
engineering work; PSC; G-RATIO; NIH tree; pushes/merges.

---

**End of STACK_UPDATE_BRIEF.md v1.0**
