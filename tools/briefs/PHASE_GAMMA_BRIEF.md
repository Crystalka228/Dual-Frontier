---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_9_1_PHASE_GAMMA_BRIEF
category: D
tier: 4
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_9_1_PHASE_GAMMA_BRIEF
---
---
register_id: DOC-D-A_PRIME_9_1_PHASE_GAMMA_BRIEF
project: Dual Frontier
category: D
tier: 4
lifecycle: Draft (-> LOCKED on Crystalka ratification -> EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-02'
content_language: en
authored_by: Claude Opus (deliberation session, Phase gamma prep)
basis: docs/reports/PHASE_GAMMA_RECON_REPORT.md (2026-07-01, R1-R8; first durable-report recon)
f12_authorization: Crystalka ratified DFK019_A = Warning on 2026-07-01 (F-12)
---

# A'.9.1 PHASE gamma SEVERITY-PROMOTION -- Execution Brief

Single-pass execution. Executor: **Claude Code, flagship model, LOCAL on Skarlet**. Repository: `D:\Colony_Simulator\Colony_Simulator`. No other tree involved.

**Brief-integration notice** (`TESTING_STRATEGY` v2.0.0 §6): this brief CITES standing law by anchor. Binding by citation: commit-body + marker law -> `CODING_STANDARDS` §8/§5; DFK-WAIVER law -> `CODING_STANDARDS` §5.3; truth law -> the v2.0.0 docs; census pins -> `TESTING_STRATEGY` §4; mutability + `Skeleton revisions` -> `RESERVED_SURFACE_MUTABILITY`; session closure -> `METHODOLOGY` v1.13.0; the ratified severity matrix -> `ANALYZER_RULES` §4.1 (Phase-gamma column) cross-checked `K_CLOSURE_REPORT` §7.2 (historical snapshot, old dotted IDs). **Anti-pattern rule:** a conflict between this brief and any standing doc means THIS BRIEF IS WRONG -- halt and escalate.

**The durable recon substitutes for a survey wave.** `docs/reports/PHASE_GAMMA_RECON_REPORT.md` is the code-truth substrate (read it in full at Phase 0); this cascade enrolls it at C1.

---

## 1. Mission [CORE]

Promote the 17 analyzer rules from Info to their ratified shipped severities, transition them AnalyzerReleases Unshipped -> Shipped (Release 1.0), prime `.editorconfig` with the same severities, truth-up `ANALYZER_RULES` to enforcement wording, close F-12, and migrate the last living dotted rule-IDs. After this cascade the analyzer **ENFORCES**: 16 build-breaking rules (11 Error + 5 Warning, both fail the build under `TreatWarningsAsErrors`) + 1 IDE-only Suggestion. The empirical proof of safe promotion is the post-flip full-solution rebuild staying 0W/0E with all 16 live.

| # | Deliverable | Action |
|---|---|---|
| D1 | 17 rule descriptors: `DiagnosticSeverity.Info` -> per-matrix targets | severity flip (17 files) |
| D2 | `AnalyzerReleases`: all 17 Unshipped -> Shipped under `## Release 1.0` / `### New Rules` | release transition (ATOMIC with D1) |
| D3 | `.editorconfig`: 17 `dotnet_diagnostic.<ID>.severity` keys, agreeing with descriptors | explicit restatement |
| D4 | `ANALYZER_RULES` 0.3.0 -> 0.4.0: status column + enforcement wording (truth-law compliant NOW -- the enforcer exists) | doc truth-up |
| D5 | F-12 closure (S2 OPEN -> CLOSED, ratified 2026-07-01) | ROADMAP ledger |
| D6 | ROADMAP dotted -> underscore rule-ID migration (~6 living sites) | citation-surface fix |
| D7 | Recon report enrollment + REGISTER closure 2.19 -> 2.20 | governance |

## 2. Established facts (recon digest -- re-verify markers) [CORE]

- HEAD `b116727` = origin/main; tree clean but for the untracked recon report; register **2.19** / 277 docs / 43 EVT. **[re-verify at Phase 0 -> H1]**
- **Promotion-safety gate: GREEN -- 0 active + 2 suppressed** (the DFK001 `ValidationLayer` waivers), measured via command-line `/p:ErrorLog` SARIF over the 12 wired src projects, tracked tree clean post-build. **[re-verify at Phase 0 -> H-gate]**
- **Ratified severity matrix** (`ANALYZER_RULES` §4.1 Phase-gamma column; F-12 ratified):
  - **Error (11):** DFK001, DFK002, DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK007_1, DFK011, DFK015_1, DFK017.
  - **Warning (5):** DFK013, DFK016, DFK019_A, DF999, DFL025_A.
  - **Suggestion (1):** DFL025_B.
  Under `TreatWarningsAsErrors` both Error and Warning fail the build -> 16 BUILD-BREAKING, 1 IDE-ONLY.
- Descriptors: `DiagnosticSeverity.Info` x17, `isEnabledByDefault: true` x17. `AnalyzerReleases.Unshipped.md` carries all 17 at Info (table `Rule ID | Category | Severity | Notes`); `Shipped.md` empty with the placeholder comment. RS2008 release-tracking ACTIVE -- it flags any descriptor-vs-table mismatch, hence D1+D2 atomicity.
- `.editorconfig`: root=true, `[*.cs]` sections present, ZERO `dotnet_diagnostic.DFK*|DFL*|DF999` keys (unprimed). Underscore IDs give clean keys (`dotnet_diagnostic.DFK019_A.severity`).
- Analyzer tests are **severity-transparent**: `CSharpAnalyzerVerifier` derives expected severity from the descriptor; 0 hardcoded severities in the test suite. **[verify by grep at Phase 0; a post-flip test failure on severity means a hardcoded expectation -> fix the test to descriptor-derived]**
- The 2 DFK-WAIVER pragmas are severity-agnostic (`#pragma warning disable` suppresses at any severity) -- NO re-triage at promotion; census pin 2 with K-L19 / VK_EXT_debug_utils citations intact.
- Living dotted-ID surface: `docs/ROADMAP.md` Analyzer-track rows (~6 sites: DFK019.A, DFK003.1 forms). `K_CLOSURE` §7.2 and the A'.9.1 brief are snapshots -- historical, UNTOUCHED.
- Known baseline shapes (not halts): F-10 stress pair; Modding.Tests Release fixture gap (Debug is the sweep config).

## 3. Phase 0 [CORE]

1. **Verify** the §2 re-verify set (HEAD, clean tree, register 2.19). Mismatch -> **H1**.
2. **Re-run the promotion-safety measurement** exactly as the recon did (command-line `/p:ErrorLog` to scratch, no tracked edits): active DFK/DFL/DF999 count MUST be **0** (+ 2 suppressed). Any active site -> **H-gate**: promotion would break the build; halt with the site list -- triage precedes promotion, and severity flips do NOT proceed.
3. **Baseline gates**: full solution build 0W/0E + full test sweep recorded (Debug config; the pre-existing failure shape is the anchor). Regression at closure -> **H2**.
4. Grep the analyzer test suite for hardcoded `DiagnosticSeverity.` expectations (expect 0 -- confirm severity transparency).
5. **REGISTER enum read** (Lesson #N14) from `REGISTER.yaml`.
6. **Mandatory reads**: the recon report (full); `ANALYZER_RULES` §4.1; `CODING_STANDARDS` §5.3 + §8; `RESERVED_SURFACE_MUTABILITY` §3-§5; `METHODOLOGY` closure protocol; the F-12 ledger entry.
7. Validate-fold protocol; `-Sync` forbidden; render once at the end; executor never pushes.

## 4. Topology [CORE]

**Serial, orchestrator only -- no writer wave.** The three severity surfaces are tightly coupled per rule and the edit set is small; parallelism buys nothing and risks RS2008 mid-state breakage. (The durable recon is the survey; Wave R: none.)

## 5. Design decision -- severity source of truth [CORE, ratified default]

**Descriptor-as-truth + `.editorconfig` as explicit restatement.** The rule code carries its severity (`defaultSeverity` = the shipped target) -- code-truth, structurally cross-checked by RS2008 against the Shipped table. `.editorconfig` keys restate the SAME severities: a greppable, explicit declaration + IDE live-analysis behavior. Both surfaces agree; **no override semantics anywhere** -- an editorconfig key differing from its descriptor is a defect (**H-integrity**). The rejected alternative (descriptors stay Info, editorconfig overrides upward) would leave the rule code lying about its own severity -- a truth-law violation in the analyzer itself.

## 6. Execution specification [CORE]

- **D1+D2 (ONE atomic commit, C2).** Flip the 17 `defaultSeverity` values per the §2 matrix (nothing else in the descriptors changes -- IDs, titles, categories, `isEnabledByDefault: true` all stay). In the SAME commit: move all 17 rows from `Unshipped.md` into `Shipped.md` under `## Release 1.0` / `### New Rules`, carrying the SHIPPED severities in the table; `Unshipped.md` becomes the empty-with-header form. Atomicity rationale: RS2008 flags descriptor-vs-table mismatch -- a split commit leaves the tree build-broken between them.
- **D3 (C3).** Add the 17 `dotnet_diagnostic.<UNDERSCORE_ID>.severity = <error|warning|suggestion>` keys in the `[*.cs]` scope as a titled DFK/DFL block, values IDENTICAL to the descriptors.
- **Post-flip gate (after C2 and after C3):** full solution rebuild MUST stay 0W/0E with 16 build-breaking rules live -- this is the empirical promotion proof. A failure here means a violation escaped the Phase 0 gate OR an RS2008/editorconfig mismatch: **fix the cause; NEVER suppress, `<NoWarn>`, or severity-downgrade to pass (H2)**. Full test sweep must match the baseline (severity-transparent).
- **D4 (C4).** `ANALYZER_RULES` 0.3.0 -> 0.4.0: §4.1 status column Info -> shipped severities; wording "detects at Info" -> "enforced at shipped severity (Error/Warning fail the build under TreatWarningsAsErrors; Suggestion is IDE-only)". This enforcement claim is truth-law compliant NOW -- the on-disk enforcer is the shipped analyzer + the TWAE property. Change-history entry.
- **D5+D6 (C5).** ROADMAP: F-12 -> CLOSED (ratified 2026-07-01, closing hash recorded) + the ~6 dotted rule-ID sites -> underscore forms (mutable citation surface; snapshots untouched).
- **D7 (C6).** Enrollment of `docs/reports/PHASE_GAMMA_RECON_REPORT.md` happens at C1 alongside the brief (both new files, one governance commit); the REGISTER closure lands at C6.

## 7. S-LOCK invariants [CORE]

S-LOCK-4 (the 17-rule set) preserved -- promotion changes no rule ID, adds none, removes none. **New structural lock:** the shipped severity matrix is RS2008-tracked -- any future severity change requires the `### Changed Rules` ceremony in `Shipped.md`, making severity drift structurally visible. The 2-waiver census and the 34/13 reserved-surface pin are asserted unchanged by the existing meta-tests.

## 8. Census discipline [CORE]

All pins expected UNCHANGED (this cascade touches no `src/` production code): `[ReservedStub` 34/13 HARD; DFK-WAIVER = 2 with citations; marker families at their SOFT baselines. Any movement is a defect, not a delta (**H2**) -- the meta-tests enforce.

## 9. Commit plan [CORE]

| # | Subject | Content |
|---|---|---|
| C1 | `governance(analyzer): enroll Phase gamma brief + recon report + validation checkpoint` | brief + recon report + VALIDATION_REPORT |
| C2 | `feat(analyzer): ship ruleset -- promote 17 severities + AnalyzerReleases Release 1.0` | D1+D2 atomic; post-flip gate |
| C3 | `chore(analyzer): prime .editorconfig with shipped DFK/DFL severities` | D3; gate re-run |
| C4 | `docs(analyzer): ANALYZER_RULES 0.4.0 -- enforcement truth-up` | D4 |
| C5 | `governance(roadmap): close F-12 + migrate dotted rule-IDs to underscore forms` | D5+D6 |
| C6 | `governance(register): Phase gamma REGISTER closure (2.19 -> 2.20)` | D7 + validate folded |
| C7 | `governance(register): render regeneration + header backfill` | render + Option-B backfill |

Commit count is intended-form; deviations per the mutability license, recorded.

## 10. REGISTER cascade (C6) [CORE]

Empirical enum shapes only. Enroll: this brief (DOC-D -> EXECUTED at closure) + the recon report (DOC-C/E per the reports convention -- follow the PHASE_BETA_VIOLATION_INVENTORY precedent). Bumps: `ANALYZER_RULES` 0.4.0; ROADMAP Live-touch. EVT `EVT-PHASE_GAMMA-CLOSURE` (43 -> 44) with C1-C5 real hashes; no new PENDING-COMMIT beyond the header self-reference (backfilled C7). register_version 2.19 -> 2.20. Validate exit 0 (**H3**); enum gap -> **H5**.

## 11. Halt conditions (H-series) [CORE]

- **H1** base-state mismatch. **H-gate** active violations > 0 at the Phase 0 re-measure -- promotion halts, triage first. **H2** post-flip build/test regression -- fix the cause, never suppress/`<NoWarn>`/downgrade to pass; census-pin movement is this class. **H-integrity** descriptor vs editorconfig vs Shipped-table severity disagreement anywhere. **H3** validate nonzero. **H5** enum gap. **H6** anything requiring an architectural decision (e.g. a rule whose promotion surfaces a design conflict).
- Standing rails: no pushes; no `-Sync` outside C6; no history rewrite; snapshots (K_CLOSURE, A'.9.1 brief) untouched; no production `src/` edits (this cascade is analyzer + docs + governance only).

On halt: stop, report verbatim, await Crystalka.

## 12. Closure protocol & report [CORE]

Execute `METHODOLOGY` v1.13.0 session closure. Report (chat): commits table; the **shipped severity matrix as landed** (17 rows); the **empirical promotion proof** (post-flip full build 0W/0E with 16 build-breaking rules live, test sweep vs baseline); census pins (34/13 held, waivers 2, SOFT unchanged); F-12 CLOSED with hash; versions (ANALYZER_RULES 0.4.0, register 2.20); `Skeleton revisions` consolidated; self-attestation (no pushes; single render; no `-Sync`; snapshots untouched; descriptor==editorconfig==Shipped everywhere; no src/ production edits); operator checklist (push; ratify lifecycle; **Phase delta is next** -- A'.9.1 governance closure where the timing-locked lessons #N17/#N18/#N19/#N20 formalize, K-L14 Evidence #14 lands, and the A'.9.1 brief goes EXECUTED).

## 13. Out of scope [CORE]

Phase delta (governance closure + lessons formalization -- next cascade) | DFK019_B runtime tier (deferred) | new rules (DF009/012/015/018/020 -> K-L20 LOCK cascade) | the Release fixture gap (F-queue) | F-10 stress pair (baseline) | the native C++20 kernel (S-LOCK-2) | KERNEL rewrites (F-4/F-9) | doc_role / hybrid register (F-2/F-13) | branch pruning (F-11) | NIH | pushes.

---

*Authored 2026-07-02 from docs/reports/PHASE_GAMMA_RECON_REPORT.md. Descriptor-as-truth design ratified as default (section 5). F-12 ratified 2026-07-01. Ratification: Crystalka. Bez kostylei.*

**End of PHASE_GAMMA_BRIEF v1.0**
