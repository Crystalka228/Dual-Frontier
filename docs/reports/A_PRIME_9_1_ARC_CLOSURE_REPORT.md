# A'.9.1 ARC CLOSURE REPORT — Analyzer Infrastructure (К-extensions cascade #5) — 2026-07-02

The durable record of the complete A'.9.1 arc, written at Phase δ closure per
`tools/briefs/PHASE_DELTA_BRIEF.md` §9 (D7) and enrolled at the same cascade's
REGISTER closure. Facts are cited from the phase EVTs
(EVT-2026-07-01-A_PRIME_9_1-PHASE_BETA-CLOSURE,
EVT-2026-07-01-A_PRIME_9_1-PHASE_GAMMA-CLOSURE), the durable recon reports at
`docs/reports/`, and this session's own git-verified measurements. The arc brief
is `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` v1.0 — EXECUTED
at this closure.

---

## §1 — Arc summary

A'.9.1 shipped the in-repo Roslyn analyzer: **17 rules detecting since Phase β
and enforcing at ratified shipped severities since Phase γ (Release 1.0)** —
11 Error + 5 Warning, build-breaking under `TreatWarningsAsErrors`, plus
DFL025_B at descriptor Info (`.editorconfig` `suggestion`, IDE-only). The
analyzer (netstandard2.0, no ProjectReference to any src assembly — Lesson
#N19) is wired into all 12 `src/` projects via `src/Directory.Build.props`
(`ProjectReference` with `OutputItemType="Analyzer"`). Substrate touch: zero
across all five phases (S-LOCK-1 held arc-wide); К-L count unchanged at 21
final; zero soft-halts and zero hard-halts across the arc. К-L14 verification
#14 (first analyzer-implementation evidence, Type 6 NEW category) recorded;
the evidence dashboard promoted AUTHORED-SKELETON → Live at the #14 gate.

## §2 — Phase table (git-verified ranges)

| Phase | Range | Commits | Date | Closure record |
|---|---|---|---|---|
| Phase 0 (recon + brief) | `bb6807c` → `4fa76ed` | 2 | 2026-05-24 | A_PRIME_9_1_PHASE_0_CLOSURE_REPORT (DOC-D) |
| Phase α (scaffolding) | `5030fa2` → `a23556f` | 9 | 2026-05-24 | arc brief §6 executed 1:1 |
| Phase β-prep (stubs + wiring) | `588c667` → `a213954` (+ `f94bb84` prompt artifact) | 4 (+1) | 2026-05-25 | 17 descriptor-only stubs at Info |
| Phase β (detection) | `1bc0df2` → `b116727` | 12 | 2026-07-01 | EVT-…-PHASE_BETA-CLOSURE (register 2.18 → 2.19) |
| Phase γ (severity promotion) | `524dd31` → `cc2f71a` + residue `4cc5e7e` | 8 + 1 | 2026-07-01 | EVT-…-PHASE_GAMMA-CLOSURE (register 2.19 → 2.20) |
| Phase δ (governance closure) | `f2841c1` → this cascade's render commit | 9 planned (C1-C9) | 2026-07-02 | EVT-2026-07-02-A_PRIME_9_1-CASCADE-CLOSURE (register 2.20 → 2.21) |

Total pre-δ: 37 commits (2 + 9 + 4(+1) + 12 + 8(+1)).

## §3 — Deliverable summary per phase

- **Phase 0**: arc brief AUTHORED (17 forward-locked Q-L decisions + Axiom
  Option (VII)); Phase 0 closure report (14 mandatory reads, 10 findings,
  DF→DFK scope 531 measured vs ~195 estimated); Lesson #N17 Provisional
  (METHODOLOGY 1.12.1).
- **Phase α**: `tools/DualFrontier.Analyzers/` (netstandard2.0) +
  `tests/DualFrontier.Analyzers.Tests/`; CPM via `Directory.Packages.props`;
  ANALYZER_RULES DF→DFK rename + structural reorganization; `[ReservedStub]`
  attribute infrastructure (`DualFrontier.Contracts.Analyzer`); cascade-#3
  deferred dispatch-arm annotation pass; PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED;
  FRAMEWORK/SYNTHESIS_RATIONALE cross-refs.
- **Phase β-prep**: 17 rule stubs (descriptors at Info, no analysis actions);
  `src/Directory.Build.props` centralized analyzer wiring.
- **Phase β**: detection populated into all 17 rules across four writer waves
  (W1 SYNTAX / W2 FQN-STRING per Lesson #N19 / W3 semantic Architecture / W4
  semantic NativeBoundary); verifier harness upgraded (CSharpAnalyzerVerifier
  / DefaultVerifier under CPM-pinned CodeAnalysis 5.3.0); 54-test suite (≥1
  positive + ≥1 negative per rule + harness + census meta-tests); descriptor-ID
  underscore adjudication (H6 escalated, Crystalka-ratified); Q-L-1 gate +
  triage (§4); violation inventory at
  `docs/reports/PHASE_BETA_VIOLATION_INVENTORY.yaml` (EVT-referenced gate
  evidence, assessed not-enrolled).
- **Phase γ**: 17 descriptors Info → ratified severities; AnalyzerReleases
  Release 1.0 (Unshipped → Shipped, RS-tracked); root `.editorconfig` primed
  with 17 descriptor-identical keys; ANALYZER_RULES 0.4.0 enforcement
  truth-up; SYNTH-2 standing-law PATCH (CODING_STANDARDS 2.1.1 +
  TESTING_STRATEGY 2.0.1); F-12 + F-25 CLOSED; dotted-ID migration on living
  ROADMAP lines.
- **Phase δ** (this cascade): METHODOLOGY 1.14.0 (#N17/#N18/#N19/#N20
  FORMALIZED + #N14 PROMOTED + F-7 registry note) `5a5bf75`; К-L14 Evidence
  #14 + dashboard Live (0.2.0) `5c34946`; KERNEL 2.6.2 arc chronicle
  `11f82bb`; K_EXTENSIONS_LEDGER §3.6 `59c1802`; F-27 riders (CODING_STANDARDS
  2.1.2 + PIPELINE_METRICS 0.2.1 + RESERVED_SURFACE_MUTABILITY 1.0.1 +
  TESTING_STRATEGY 2.0.2 + ANALYZER_RULES 0.4.1 + 17 descriptor strings)
  `0411bb0`; ROADMAP Analyzer-track closure flips + MIGRATION_PROGRESS
  chronicle catch-up + F-27/F-7 CLOSED + F-28 seeded `c4119b0`; REGISTER
  2.20 → 2.21 + 3 enrollments + arc brief EXECUTED (the commit carrying this
  report); render + Option-B backfill (C9).

## §4 — Q-L-1 adaptive gate result (Phase β record of record)

First-run violation count over the 12 src projects: **23 real Info
diagnostics** → 23 ≤ 80 → **CONTINUE in a single cascade** (Q-L-1). Per-rule:
DFK002 ×13 (the ManagedBusBridge private `[DllImport]` cluster), DFK017 ×5,
DFK007 ×3, DFK001 ×2. Triage: **15 genuine violations in 2 clusters** — the
DFK002 13-site cluster relocated to `src/DualFrontier.Core.Interop/
NativeMethods.Bus.cs` (the §8-sanctioned canonical P/Invoke surface;
behavior-neutral, fast sweep 1082/1082 held; recorded H6-adjacent target
deviation — brief named Runtime.Native.Bus, which Application does not
reference), and the DFK001 2-site cluster (К-L19-sanctioned VK_EXT_debug_utils
debug-messenger interop in `ValidationLayer.cs`) suppressed per the DFK-WAIVER
law with Crystalka authorization; the remaining 8 (DFK007 ×3 + DFK017 ×5) were
heuristic false-positives refined out of the rules. Post-triage: 0 active
diagnostics + 2 census-pinned waivers (SARIF-confirmed 0 active + 2
suppressed-inSource at the γ promotion-safety gate).

## §5 — Shipped severity matrix (Release 1.0, F-12-ratified)

| Severity | Rules | Count |
|---|---|---|
| Error (build-breaking) | DFK001, DFK002, DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK007_1, DFK011, DFK015_1, DFK017 | 11 |
| Warning (build-breaking under TWAE) | DFK013, DFK016 (К-L13/К-L16 efficiency-class), DFK019_A (F-12), DFL025_A, DF999 | 5 |
| Suggestion-tier (descriptor Info ≡ `.editorconfig` `suggestion`, IDE-only) | DFL025_B | 1 |

16 build-breaking + 1 IDE-only = 17. Three agreeing severity surfaces:
descriptors = `AnalyzerReleases.Shipped.md` (Release 1.0, RS-tracked) =
`.editorconfig`.

## §6 — Census pins final state (re-measured 2026-07-02, canonical expressions)

| Census | Pin | Measured at δ | State |
|---|---|---|---|
| `\[ReservedStub` (src, excl. definition file) | 34 / 13 | 34 / 13 | HARD pin EXACT |
| `stub` (rg --count-matches -i) | 51 / 20 | 51 / 20 | unchanged |
| `deferred` | 82 / 51 | 82 / 51 | unchanged |
| `TODO` (case-sensitive) | 136 / 53 | 136 / 53 | unchanged |
| `Phase 6` | 23 / 11 | 23 / 11 | unchanged |
| `not yet` | 8 / 7 | 8 / 7 | unchanged |
| DFK-WAIVER | 2 | 2 (both К-L19-cited, ValidationLayer.cs) | unchanged |

Method note: an initial δ measure of `deferred` read 81 via `rg -c` (matching
lines); the canonical `--count-matches` expression reads 82 (one line carries
two matches). The pins are defined on `--count-matches` — no delta.

## §7 — Lessons formalized (METHODOLOGY 1.14.0, `5a5bf75`)

- **#N14 — Phase 0 empirical assumed-state coverage** — PROMOTED (4-application
  criterion met; #N16 recorded absorbed per its own K-ext-#3 disposition).
- **#N17 — Audience-driven tooling deferral** — FORMALIZED (five simultaneous
  Q-L applications at this arc's deliberation; promotion gate retained as the
  standing falsifiability watch; F-27(e) dotted forms fixed in-span).
- **#N18 — Pre-flight empirical scope verification** — FORMALIZED (five-instance
  track record: 195→229 enrollment, 195→531 rename, 30→11 CPM, β phantom
  projects, F-25 census pre-measure).
- **#N19 — Analyzer detection via canonical FQN strings** — FORMALIZED (the
  Phase β W2 global writer law).
- **#N20 — Eradication class derived from the repository's own classification**
  — FORMALIZED (the F-26 mandate from the Godot Eradication pressure-test).
- **Lesson-number registry note** (F-7 adjudication): #N1/#N11 free;
  #N4/#N15/#N16 reserved-with-semantics; `#Nxx` citation form mandatory.

## §8 — Falsifiability-anchor readout

- **Defect classes caught by the shipped enforcement**: at first detection the
  394-file src/ surface carried exactly one architectural-boundary violation
  class (the DFK002 interop-placement cluster, 13 sites in one file) and one
  sanctioned-exception class (DFK001 ×2, waived with citation) — a near-clean
  pre-analyzer codebase; discipline evidence for the К-L14 thesis
  (architectural cleanliness preceded its enforcement mechanism).
- **Architectural integrity**: trust-by-discipline → trust-by-enforcement (the
  Roslyn-expressible К-L subset is machine-enforced at build time; the γ exit
  gate — build exit 0 with every correctness DFK### at `error` — passed at
  `3272d74`).
- **Pipeline economics** (arc brief §3.2 facts, carried honestly): expectation
  «+1-3s per `dotnet build` / +5-10s per `dotnet test`» for the analyzer pass;
  not separately instrumented at β/γ — build/test gates ran green within the
  normal wall-clock envelope; no measured per-pass baseline is claimed. The
  per-commit analyzer pass replaces the manual cross-document audit cadence.
- **Defect-rate expectation** (zero post-cleanup src/ violations): realized —
  0 active diagnostics since β C9, held through γ promotion and the δ C6 gate.

## §9 — Operator ratifications ledger (the arc's Crystalka decisions)

| Ratification | Date | Record |
|---|---|---|
| Arc brief + 17 Q-L decisions + Axiom Option (VII) | 2026-05-24 | Phase 0 EVT; PROJECT_AXIOMS v1.0 LOCKED |
| S-LOCK-4 «+0 or +1» rule-count conditional honored (16→17 with DF999) | 2026-05-24/25 | arc brief §5; β-prep stubs |
| DFK002 federated interop surface (§8: Core.Interop + Runtime.Native roots) | 2026-07-01 | PHASE_BETA_BRIEF §8; SanctionedInteropSurface.cs |
| Descriptor-ID underscore normalization (dotted/hyphen forms Roslyn-invalid) | 2026-07-01 | β C3 `d7cff93`; ANALYZER_RULES 0.2.2; H6 escalated → ratified |
| DFK-WAIVER of the two К-L19 DFK001 sites | 2026-07-01 | β C9 `c74ede7`; census pin 2 |
| F-12: DFK019_A = Warning (K_CLOSURE §7.2 canonical over the brief's blanket Error) | 2026-07-01 | γ C2 `3272d74`; F-12 CLOSED |
| Info/suggestion mapping: DiagnosticSeverity has no Suggestion member — DFL025_B ships descriptor Info ≡ `.editorconfig` `suggestion` (IDE-only) | 2026-07-01 | γ EVT; ratified severity matrix |
| Phase δ brief (this cascade's delta-set §3: single #14, H4 house form, F-7 disposition, ANALYZER_RULES re-gate, all riders) | 2026-07-02 | PHASE_DELTA_BRIEF ratification |

## §10 — Phase δ execution record

- **Commits**: C1 `f2841c1` (brief + recon + validation checkpoint), C2
  `5a5bf75` (METHODOLOGY 1.14.0), C3 `5c34946` (dashboard #14 + 0.2.0), C4
  `11f82bb` (KERNEL 2.6.2), C5 `59c1802` (ledger §3.6), C6 `0411bb0` (F-27
  riders; the build-gated commit), C7 `c4119b0` (ROADMAP + tracker + F-sweep),
  C8 = the REGISTER closure carrying this report, C9 = render + Option-B
  backfill.
- **Gates**: C6 H2 gate — analyzer rebuild `dotnet build
  tools/DualFrontier.Analyzers -c Debug` 0W/0E exit 0; analyzer suite `dotnet
  test tests/DualFrontier.Analyzers.Tests -c Debug` **54/54**, exactly matching
  the pre-change Phase 0 baseline 54/54. C8 H3 gate — `sync_register.ps1
  -Validate` exit 0 (folded).
- **Phase 0 baseline sweep** (full unfiltered `dotnet test DualFrontier.sln -c
  Debug`, launched on the pristine `4cc5e7e` tree): green —
  ManifestRewriter 7/7, Persistence 4/4, Runtime 292/292, **Analyzers 54/54**,
  Systems 2/2, Core.Interop 202/202, Application 45/45. Core.Tests aborted in
  the stress chunk by the TESTING_STRATEGY §2.6-documented testhost crash
  (known F-10 shape) after 89 passes + 4 failures, all in the known
  stress/extreme flaky family (`SchedulerStressTests.BusTiers_AllThreeTiers_
  HighContentionPublish_RespectsTierContract`, `SchedulerStressTests.
  SchedulingPolicies_OrderByPriority_ParallelCalls_DeterministicAndCorrectly
  Ordered`, `SchedulerExtremeTests.S3_Bus_FiveMillionEventsPerTier_HoldsAll
  DeliveryContracts`, `SchedulerExtremeTests.S6_Bus_FastTier_SixtySecond
  Marathon_ThroughputStable`) — pre-existing class on the pre-cascade tree by
  construction (F-10; recorded, not absorbed). The Modding suite finished
  **398/398 green** (15 m 8 s) — its summary was released only after the
  session force-killed the shutdown-wedged Modding testhost (~50 minutes of
  zero-output hang past suite completion; the documented F-10 zombie-testhost
  wedge shape — the run was complete, the zombie held the pipe). Terminal
  state recorded here by the C10 correction commit; the sentence originally
  pointed to the C8 commit body, authored while the suite was still in
  flight — that pointer was never fulfilled and is corrected to this inline
  record. Sweep totals: 1093 passed across the nine suites / 4 known-class
  stress failures / Core.Tests truncated at the §2.6 crash; the known Modding
  F-10 flake (CreateLoop_RunningLoop_…) did not manifest this run. The
  β/γ-gate fast sweep record (1082/1082) remains the arc's green-suite
  anchor; no production code was touched by Phase δ.
- **Skeleton revisions (consolidated)**: arc brief §8/§9 template stale values
  (register "2.10"→ live 2.21; METHODOLOGY "v1.12→v1.13" → 1.13.0→1.14.0;
  KERNEL "v2.5.3→v2.5.4" → 2.6.1→2.6.2; "15-16 active rules" → 17 shipped;
  dotted IDs → underscore) — live values governed; arc brief §3.3 forward
  evidence reservations #15/#16/#17 → superseded by the single arc-wide #14;
  arc brief §3.2 YAML entry template → dashboard §3 markdown template (facts
  folded into narrative); arc brief "1-2 atomic commits" δ estimate → the
  9-commit accumulated governance scope; brief §8 D6 Godot-range citation →
  git-verified 077a8c8..02be616 (the quoted 0890645..187c46c is Architecture
  Truth's); F-27(d) "17 strings" → 16 plain + 1 mid-string (DFK019_A, deferral
  tail preserved).
- **Self-attestation**: no pushes; no `-Sync`; no history rewrite; single
  render (C9); PIPELINE_METRICS §1–§5 historical bodies, phase-brief snapshot
  bodies, and KERNEL Part 0 untouched; `#Nxx` forms used exclusively in all
  delta prose; census HARD pin 34/13 EXACT; new findings ledgered (F-28),
  never chat-only.

**End of A_PRIME_9_1_ARC_CLOSURE_REPORT.md — 2026-07-02. The A'.9.1 arc is CLOSED.**
