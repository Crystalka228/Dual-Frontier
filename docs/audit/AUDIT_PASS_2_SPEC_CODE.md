---
title: Audit Pass 2 — Spec ↔ Code Drift
nav_order: 105
---

> **STATUS: INCOMPLETE — TIER 0 ESCALATION**
>
> **Triggered at:** §11 Sequence integrity check, sequence #24 (`MOD_OS_ARCHITECTURE.md` §11.2 ValidationErrorKind baseline enumeration vs. `src/DualFrontier.Application/Modding/ValidationError.cs`).
>
> **Pass 2 stopped at step 2 of methodology (§10 of `AUDIT_PASS_2_PROMPT.md` — sequence integrity check). §1–§10 spec ↔ code mapping (steps 3–12 of methodology) was NOT performed.**
>
> **Subsequent passes (3, 4, 5) cannot proceed until the human resolves this Tier 0 finding.**
>
> **See §13 Tier 0 — Spec drift (BLOCKING) for full diagnostic.**

# Audit Pass 2 — Spec ↔ Code Drift

**Date:** 2026-05-01
**Branch:** `main` (per `.git/HEAD` line 1)
**HEAD:** `1d43858a36c17b956a345e9bfe07a9ccf82daddb` (per `.git/refs/heads/main` line 1)
**Spec under audit:** `MOD_OS_ARCHITECTURE.md` LOCKED v1.4
**Pass 1 inventory consumed:** `docs/audit/AUDIT_PASS_1_INVENTORY.md` (9/9 PASSED)
**Scope:** Spec ↔ Code drift verification. Each normative statement in
v1.4 §1–§10 was to be matched to a code or test artifact, or classified
as Tier 0/1/2/3/4 finding. Sequence integrity check applied to all 53
sequences from Pass 1 §9 catalogue. **Pass eager-escalated on Tier 0
finding during sequence integrity check; §1–§10 spec ↔ code mapping was
NOT performed.**

---

## §0 Executive summary

| # | Check | Status | Tier 0 / 1 / 2 / 3 / 4 counts |
|---|---|---|---|
| 1 | §1 Mod topology | NOT EXECUTED | — / — / — / — / — |
| 2 | §2 Manifest schema | NOT EXECUTED | — / — / — / — / — |
| 3 | §3 Capability model | NOT EXECUTED | — / — / — / — / — |
| 4 | §4 IModApi v2 | NOT EXECUTED | — / — / — / — / — |
| 5 | §5 Type sharing across ALCs | NOT EXECUTED | — / — / — / — / — |
| 6 | §6 Three contract levels | NOT EXECUTED | — / — / — / — / — |
| 7 | §7 Bridge replacement | NOT EXECUTED | — / — / — / — / — |
| 8 | §8 Versioning | NOT EXECUTED | — / — / — / — / — |
| 9 | §9 Lifecycle and hot reload | NOT EXECUTED | — / — / — / — / — |
| 10 | §10 Threat model | NOT EXECUTED | — / — / — / — / — |
| 11 | §11 Sequence integrity (53 sequences) | **FAILED** | 1 / — / — / 2 / 1 |

`PASSED` = no Tier 0 or Tier 1 findings in this check. `FAILED` = at
least one Tier 0 or Tier 1 finding present. `NOT EXECUTED` = check
not performed because the pass eager-escalated on a prior Tier 0
finding.

**INCOMPLETE — eager-escalated on Tier 0** at §11 sequence #24
(`MOD_OS_ARCHITECTURE.md` §11.2 ValidationErrorKind baseline enumeration
vs. `ValidationError.cs:9–83` enum members). See §13 Tier 0 for full
diagnostic.

**Tier breakdown across all checks (sequence integrity only):**

- Tier 0: 1 finding — eager escalation triggered: **YES**
- Tier 1: 0 findings (pass did not reach §1–§10 mapping)
- Tier 2: 0 findings (whitelist verifications NOT performed; pass stopped before §13 whitelist confirmation step)
- Tier 3: 2 findings (sequence #17 lifecycle states; sequence #14 constraint syntaxes — both wording clarity)
- Tier 4: 1 finding (sequence #37/#38 ContractValidator phase enumeration vs. invocation order)

---

## §1 §1 Mod topology — three mod kinds

(NOT EXECUTED — Pass 2 eager-escalated at §11 sequence integrity check
before reaching §1–§10 spec ↔ code mapping. Per `AUDIT_PASS_2_PROMPT.md`
§8.1, §1–§10 sections preserved with placeholder pending Tier 0
resolution.)

---

## §2 §2 Manifest schema

(NOT EXECUTED — see §1 banner. Pass 1 anomaly #11 (`description` field
in `mods/DualFrontier.Mod.Example/mod.manifest.json` not in spec §2.2)
remains unclassified by Pass 2.)

---

## §3 §3 Capability model

(NOT EXECUTED — see §1 banner.)

---

## §4 §4 IModApi v2

(NOT EXECUTED — see §1 banner.)

---

## §5 §5 Type sharing across ALCs

(NOT EXECUTED — see §1 banner.)

---

## §6 §6 Three contract levels

(NOT EXECUTED — see §1 banner.)

---

## §7 §7 Bridge replacement

(NOT EXECUTED — see §1 banner.)

---

## §8 §8 Versioning

(NOT EXECUTED — see §1 banner. Verification of M5.2 cascade-failure
ratification (§5.2 of `AUDIT_PASS_2_PROMPT.md`) NOT performed.)

---

## §9 §9 Lifecycle and hot reload

(NOT EXECUTED — see §1 banner. Verification of M7 §9.2/§9.3 run-flag
and M7 §9.5/§9.5.1 step 7 ordering ratifications NOT performed.)

---

## §10 §10 Threat model

(NOT EXECUTED — see §1 banner.)

---

## §11 Sequence integrity findings

Performed for all 53 sequences from `AUDIT_PASS_1_INVENTORY.md` §9.
Per `AUDIT_PASS_2_PROMPT.md` §7.1, each sequence checked for gap,
duplicate, order, count_mismatch, and cross_inconsistency. Per
`AUDIT_PASS_2_PROMPT.md` §7.2, special focus on Pass 1 anomalies
#3 (§9.1 lifecycle), #4 (validator phases), and #5
(ValidationErrorKind enumeration).

**Per-sequence verdicts:**

| Sequence # (from Pass 1 §9) | Source | Check type | Verdict | Tier |
|---|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE.md` Preamble — strategic locked decisions | count_mismatch | 5 entries vs. "Five top-level decisions" — match | ✓ |
| 2 | `MOD_OS_ARCHITECTURE.md` §0 OS mapping table | (no count claim) | n/a | ✓ |
| 3 | `MOD_OS_ARCHITECTURE.md` §1 mod kinds | count_mismatch | 3 sub-sections (§1.1 Regular, §1.2 Shared, §1.3 Vanilla) vs. "three mod kinds" / "three categories" — match | ✓ |
| 4 | `MOD_OS_ARCHITECTURE.md` §1.4 load-graph invariants | (no count claim) | 4 bullets observed | ✓ |
| 5 | `MOD_OS_ARCHITECTURE.md` §2.2 manifest field reference | count_mismatch | 13 rows observed; field reference table — match | ✓ |
| 6 | `MOD_OS_ARCHITECTURE.md` §2.3 parse-time validation steps | count_mismatch | 6 numbered steps observed; "1, 2, 3, 4, 5, 6" — match | ✓ |
| 7 | `MOD_OS_ARCHITECTURE.md` §3.6 runtime cases load-time gate cannot reach | count_mismatch | 3 numbered vs. "three cases" — match | ✓ |
| 8 | `MOD_OS_ARCHITECTURE.md` §4.1 IModApi v2 surface methods | count_mismatch | 9 method declarations vs. "v1 had 6, v2 adds 3" (=9) — match | ✓ |
| 9 | `MOD_OS_ARCHITECTURE.md` §5.2 shared mod loader steps | count_mismatch | 6 numbered steps — match | ✓ |
| 10 | `MOD_OS_ARCHITECTURE.md` §5.3 regular mod loader steps | count_mismatch | 5 numbered steps — match | ✓ |
| 11 | `MOD_OS_ARCHITECTURE.md` §5.4 shared mod restrictions | (no count claim) | 4 bullets observed | ✓ |
| 12 | `MOD_OS_ARCHITECTURE.md` §7.1 bridge-replacement mechanism steps | count_mismatch | 4 numbered steps — match | ✓ |
| 13 | `MOD_OS_ARCHITECTURE.md` §7.5 bridge-replacement test scenarios | (no count claim) | 5 bullets observed | ✓ |
| 14 | `MOD_OS_ARCHITECTURE.md` §8.4 constraint syntaxes | **count_mismatch (wording)** | Spec line 631 verbatim: «Three syntaxes are supported, all are subsets of npm/Cargo conventions:». Three subsections enumerated: Exact (supported), Caret (supported), Tilde («**not supported** in v1»; «the parser rejects with a clear error message»). Code (`VersionConstraint.cs:7–14`) has `enum VersionConstraintKind { Exact, Caret }` — 2 members, no Tilde. Per spec §8.5 step 2, parser throws `FormatException` for `~`. The wording «three syntaxes are supported» is overloaded: «supported» means «recognized/handled by parser» (covering 2 accepted + 1 explicitly-rejected = 3), not «accepted». **Tier 3 (wording clarity)** — spec is internally consistent if «supported» is read as «handled»; code matches spec by accepting Exact+Caret and rejecting Tilde. | Tier 3 |
| 15 | `MOD_OS_ARCHITECTURE.md` §8.6 where each axis applies | count_mismatch | 3 rows — match | ✓ |
| 16 | `MOD_OS_ARCHITECTURE.md` §8.7 resolution algorithm | count_mismatch | 4 numbered top-level steps — match | ✓ |
| 17 | `MOD_OS_ARCHITECTURE.md` §9.1 lifecycle states | **count_mismatch — Pass 1 anomaly #3** | Spec line 692 verbatim: «The mod lifecycle has six well-defined states.» Diagram (lines 696–724): 6 boxes labelled `Disabled` → `Pending` → `Loaded` → `Active` → `Stopping` → `Disabled`. Unique state-name labels: 5 (Disabled appears at both initial and terminal positions). The diagram is internally consistent (6 boxes drawn deliberately; the cycle returns to `Disabled`, representing «user re-enables a disabled mod» per arrow «user enables» on line 700). No code representation of these states exists (Pass 1 §5 inventory: no `ModLifecycleState` enum in `DualFrontier.Contracts`). The wording «six well-defined states» matches box count but is imprecise relative to unique state-name count (5). **Tier 3 (wording clarity)** — diagram and prose are mutually consistent if «states» is read as «lifecycle positions/boxes»; precision could be improved with wording like «six lifecycle positions, five distinct states». No code drift (no enum to compare against). | Tier 3 |
| 18 | `MOD_OS_ARCHITECTURE.md` §9.2 hot reload menu flow steps | count_mismatch | 4 numbered steps — match | ✓ |
| 19 | `MOD_OS_ARCHITECTURE.md` §9.5 unload chain steps | count_mismatch | 7 numbered steps. v1.4 changelog explicitly references «step 7». Match. | ✓ |
| 20 | `MOD_OS_ARCHITECTURE.md` §10.1 architectural threats caught | (no count claim) | 8 rows observed | ✓ |
| 21 | `MOD_OS_ARCHITECTURE.md` §10.2 architectural threats not caught | (no count claim) | 5 bullets observed | ✓ |
| 22 | `MOD_OS_ARCHITECTURE.md` §10.4 required test categories | (no count claim) | 7 bullets observed | ✓ |
| 23 | `MOD_OS_ARCHITECTURE.md` §11.1 migration phases | (no count claim) | 12 rows observed (M0..M10 + M3.4 deferred) | ✓ |
| 24 | `MOD_OS_ARCHITECTURE.md` §11.2 ValidationErrorKind entries | **cross_inconsistency + count_mismatch — Pass 1 anomaly #5 — TIER 0** | Spec lines 853–864 verbatim opening: «The current enum has `MissingDependency` and `CyclicDependency`. The migration adds:» followed by 8 bullets including `CapabilityViolation` flagged «not part of the validation set, but listed here for completeness». Spec implies enum cardinality = 2 baseline + 7 validation additions = **9 validation members**. Code (`ValidationError.cs:9–83`) has **11 enum members** (see sequence #39 below). Two members in code are NOT enumerated by spec §11.2 in either baseline or migration-additions: `IncompatibleContractsVersion` (line 15) and `WriteWriteConflict` (line 22). The spec wording «The current enum has X and Y» is declarative without «for example» / «among others» qualifier; on a strict reading the spec claims a complete 2-member baseline. Code expanded baseline by 2 unenumerated members. v1.x changelog (lines 10–29) records ratifications for §3.6, §3.5+§2.1, §11.1 M3.4, §2.2 entryAssembly/entryType wording, and §9.5 step 7 + §9.5.1 — **none** ratified §11.2 to expand baseline enumeration. **Tier 0** — spec drift against LOCKED v1.4. See §13 Tier 0 for full diagnostic. | **Tier 0** |
| 25 | `MOD_OS_ARCHITECTURE.md` §11.4 stop conditions | (no count claim) | 3 bullets observed | ✓ |
| 26 | `MOD_OS_ARCHITECTURE.md` §12 detail decisions | count_mismatch | 7 entries (D-1..D-7) vs. «seven detail decisions» — match | ✓ |
| 27 | `ROADMAP.md` Status overview phase rows | (no count claim) | 20 rows observed | ✓ |
| 28 | `ROADMAP.md` Closed phases headings | (no count claim) | 7 closed-phase headings observed | ✓ |
| 29 | `ROADMAP.md` M3 sub-phases | (no count claim) | M3.1, M3.2, M3.3, M3.4 (deferred) — 4 sub-phases observed | ✓ |
| 30 | `ROADMAP.md` M4 sub-phases | (no count claim) | M4.1, M4.2, M4.3 — 3 observed | ✓ |
| 31 | `ROADMAP.md` M5 sub-phases | (no count claim) | M5.1, M5.2 — 2 observed | ✓ |
| 32 | `ROADMAP.md` M6 sub-phases | (no count claim) | M6.1, M6.2 (M6.3 closure-sync mechanism, line 318) — 2 observed | ✓ |
| 33 | `ROADMAP.md` M7 sub-phases | count_mismatch | 5 sub-phases (M7.1–M7.5) + 1 closure session vs. «five implementation sub-phases (M7.1 – M7.5) plus a closure session» — match | ✓ |
| 34 | `ROADMAP.md` M10 vanilla slices | (no count claim) | M10.A, M10.B, M10.C, M10.D — 4 observed | ✓ |
| 35 | `ROADMAP.md` Phase 4 v0.3 architectural fixes block | (no count claim) | 6 bullets observed | ✓ |
| 36 | `ROADMAP.md` Engine snapshot progressive test counts | sequence | Observed: 60, 82, 247, 260, 281, 311, 328, 333, 338, 369. Final claim line 36 verbatim: «Total at M7.3 closure: 369/369 passed». Last entry matches claim. (Pass 1 anomaly #1 records source-level `[Fact]+[Theory]` count = 359 vs. ROADMAP-stated 369; routed to Pass 3 for runtime-test-count vs. source-attribute-count reconciliation.) | ✓ |
| 37 | `ContractValidator.cs:12–48` validator phases (class XML-doc) | count_mismatch | «Eight-phase validator»; phases A, B, C, D, E, F, G, H all enumerated. Count = 8. Match. | ✓ |
| 38 | `ContractValidator.cs:88–103` `Validate()` method body invocation order | **cross_inconsistency vs. #37 — Pass 1 anomaly #4** | Class doc per-phase prose enumeration order: A, B, E, C, D, F, G, H. Method body invocation order: A (line 88) → B (89) → E (90) → G (91) → H (92) → conditionally C, D (lines 94–98) → conditionally F (lines 100–103). Class doc lines 41–44 verbatim: «Phases A, B, E, G and H run unconditionally; phases C and D run only when a `KernelCapabilityRegistry` is supplied; phase F runs only when a shared-mod list is supplied to `Validate`.» This conditionality summary IS consistent with invocation: unconditional phases A/B/E/G/H run first; conditional C/D run when `kernelCapabilities is not null`; conditional F runs when `sharedMods is not null`. The class doc per-phase prose enumeration order (A, B, E, C, D, F, G, H) is **descriptive, not prescriptive** — it describes each phase's responsibility and does not claim alphabetical or invocation-order semantics. The class doc never asserts «phases run in this order». No spec drift; no code drift; minor cosmetic mismatch in prose enumeration vs. invocation. **Tier 4 (cosmetic)** — could be improved by reordering per-phase prose to match invocation, but no behaviour or contract impact. | Tier 4 |
| 39 | `ValidationError.cs:9–83` `enum ValidationErrorKind` members | **cross_inconsistency vs. #24 — Pass 1 anomaly #5 — TIER 0** | 11 members observed in source order: IncompatibleContractsVersion (line 15), WriteWriteConflict (22), CyclicDependency (27), MissingDependency (32), IncompatibleVersion (40), MissingCapability (47), SharedModWithEntryPoint (55), ContractTypeInRegularMod (62), BridgeReplacementConflict (69), ProtectedSystemReplacement (76), UnknownSystemReplacement (82). Spec §11.2 enumerates 2 baseline (`MissingDependency`, `CyclicDependency`) + 7 validation additions (`MissingCapability`, `BridgeReplacementConflict`, `ProtectedSystemReplacement`, `UnknownSystemReplacement`, `IncompatibleVersion`, `SharedModWithEntryPoint`, `ContractTypeInRegularMod`) = **9** total validation members. Code has **11**. Delta of +2: `IncompatibleContractsVersion` and `WriteWriteConflict`. **Tier 0** — see sequence #24 and §13 Tier 0 for full diagnostic. | **Tier 0** |
| 40 | `IModApi.cs:16–75` API methods declaration order | cross_inconsistency vs. #8 | 9 declarations observed: RegisterComponent, RegisterSystem, Publish, Subscribe, PublishContract, TryGetContract, GetKernelCapabilities, GetOwnManifest, Log. Spec §4.1 enumeration order (per Pass 1 §9 entry 8): RegisterComponent, RegisterSystem, Publish, Subscribe, PublishContract, TryGetContract, GetKernelCapabilities, GetOwnManifest, Log — match. Same order, same count. ✓ | ✓ |
| 41 | `ModLogLevel.cs:7–20` severity levels | count_mismatch | 4 members (Debug, Info, Warning, Error). Spec v1.1 ratification clarified Log parameter as `ModLogLevel` (not `LogLevel`). No spec count claim for ModLogLevel. ✓ | ✓ |
| 42 | `VersionConstraint.cs:7–14` constraint kinds | cross_inconsistency vs. #14 | 2 members (Exact, Caret). Spec §8.4 lists 3 syntaxes (Exact accepted, Caret accepted, Tilde rejected). Enum represents only the 2 accepted syntaxes; Tilde is rejected at parse time without enum representation. Match (see #14 verdict). | ✓ |
| 43 | `ModManifest.cs:7–17` `enum ModKind` members | cross_inconsistency vs. #3 | 2 members (Regular, Shared). Spec §1 «three mod kinds» = Regular + Shared + Vanilla, but spec §1.3 explicitly states «Vanilla is not a separate kind» — the third is convention/editorial, not enum-represented. Code correctly has 2 enum members. ✓ | ✓ |
| 44 | `OwnershipMode.cs:10–37` ownership modes | count_mismatch | 4 members (Bonded, Contested, Abandoned, Transferred). No spec count claim. ✓ | ✓ |
| 45 | `TickRateAttribute.cs:37–53` `static class TickRates` constants | count_mismatch | 5 constants (REALTIME, FAST, NORMAL, SLOW, RARE). No spec count claim. ✓ | ✓ |
| 46 | `IGameServices.cs:13–57` bus accessors | count_mismatch | 6 properties (Combat, Inventory, Magic, Pawns, World, Power). Pass 1 anomaly #8 records `Contracts/README.md:17` listing only 5 buses (cross-doc — Pass 4 territory; not flagged here). Within `IGameServices` itself: 6 properties, internally consistent. ✓ | ✓ |
| 47 | `SystemExecutionContext.cs:270–319` violation paths | (no spec count claim) | 3 paths (BuildReadViolationMessage, BuildWriteViolationMessage, GetSystem) ✓ | ✓ |
| 48 | `M3_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows. Closure-review pattern from `M6_CLOSURE_REVIEW.md`. ✓ | ✓ |
| 49 | `M4_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows. ✓ | ✓ |
| 50 | `M5_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows. ✓ | ✓ |
| 51 | `M6_CLOSURE_REVIEW.md` §0 Executive summary checks | count_mismatch | 8 rows vs. «All 8 checks PASSED» (line 30) — match. ✓ | ✓ |
| 52 | `AUDIT_CAMPAIGN_PLAN.md` §2 campaign passes | count_mismatch | 5 passes (Pass 1..Pass 5) vs. «пять последовательных проходов» — match. ✓ | ✓ |
| 53 | `AUDIT_CAMPAIGN_PLAN.md` §7 ratified decisions | count_mismatch | 6 (§7.1..§7.6) vs. «шесть открытых решений §7 ratified» — match. ✓ | ✓ |

**Sequence integrity summary:**

- Total sequences checked: 53
- Tier 0 (sequence integrity violations): **1** (sequence #24, with cross-reference to #39)
- Tier 3 (wording clarity, no spec ↔ code drift): 2 (sequence #14 «three syntaxes are supported»; sequence #17 «six well-defined states»)
- Tier 4 (cosmetic): 1 (sequence #37/#38 prose enumeration order vs. invocation order)
- PASS (no integrity issue): 49

**Per `AUDIT_PASS_2_PROMPT.md` §7.2 special-focus anomalies:**

- **Pass 1 anomaly #3 (sequence #17 — §9.1 lifecycle).** Verdict: **Tier 3 (wording clarity)**. Diagram is internally consistent: 6 boxes drawn deliberately (initial `Disabled` and terminal `Disabled` represent the lifecycle cycle). Spec wording «six well-defined states» matches box count but is imprecise relative to unique-state-name count (5). No code representation exists (no `ModLifecycleState` enum in `DualFrontier.Contracts`), so no spec ↔ code drift. Wording could be improved («six lifecycle positions, five distinct states»), but neither code nor diagram is wrong. Not eager-escalated as Tier 0 because no code drift and diagram-prose are mutually consistent under «boxes» reading.
- **Pass 1 anomaly #4 (sequence #37/#38 — ContractValidator phases).** Verdict: **Tier 4 (cosmetic)**. Class doc per-phase prose enumeration (A, B, E, C, D, F, G, H) does not match `Validate()` invocation order (A, B, E, G, H, then conditionally C/D, then conditionally F). The class doc's normative ordering claim is at lines 41–44 (conditionality summary), which IS consistent with invocation. The per-phase prose enumeration is descriptive, not prescriptive about execution order. No spec drift, no code drift. Cosmetic mismatch only.
- **Pass 1 anomaly #5 (sequence #24/#39 — ValidationErrorKind enumeration).** Verdict: **Tier 0 — eager escalation triggered**. See §13 Tier 0 for full diagnostic.

**Eager Tier 0 escalation triggered at sequence #24.** Per `AUDIT_PASS_2_PROMPT.md` §8.3, sequence integrity check completed across all 53 sequences before escalation; no other Tier 0 candidates found.

---

## §12 Surgical fixes applied this pass

None. Pass 2 is read-only by contract.

---

## §13 Items requiring follow-up

### Tier 0 — Spec drift (BLOCKING)

| # | Spec section | Code path | Description | Recommendation |
|---|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE.md` §11.2 lines 853–864 | `src/DualFrontier.Application/Modding/ValidationError.cs:9–83` (`enum ValidationErrorKind`) | **Spec drift on `ValidationErrorKind` baseline enumeration.** Spec line 854 verbatim: «The current enum has `MissingDependency` and `CyclicDependency`. The migration adds:». Spec lines 855–864 list 8 migration additions: `MissingCapability` (M3), `BridgeReplacementConflict` (M6), `ProtectedSystemReplacement` (M6), `UnknownSystemReplacement` (M6), `IncompatibleVersion` (M5), `SharedModWithEntryPoint` (M4), `ContractTypeInRegularMod` (M4), `CapabilityViolation` (M3, runtime, «not part of the validation set, but listed here for completeness»). Spec implies enum cardinality = 2 baseline + 7 validation additions = **9 validation members**. Code at `ValidationError.cs:9–83` has **11 enum members**: IncompatibleContractsVersion (line 15), WriteWriteConflict (22), CyclicDependency (27), MissingDependency (32), IncompatibleVersion (40), MissingCapability (47), SharedModWithEntryPoint (55), ContractTypeInRegularMod (62), BridgeReplacementConflict (69), ProtectedSystemReplacement (76), UnknownSystemReplacement (82). Delta = +2 unenumerated members: `IncompatibleContractsVersion` (used by Phase A `ValidateContractsVersions` per `ContractValidator.cs:88` for `RequiresContractsVersion` failures) and `WriteWriteConflict` (used by Phase B `ValidateWriteWriteConflicts` per `ContractValidator.cs:89` for component write-collision detection per spec §10.1 «Mod registers a system that conflicts with another mod's system»). The spec wording «The current enum has X and Y» is declarative without «for example» / «among others» qualifier — strict reading implies a complete 2-member baseline at the time of v1.0 spec writing. Whether the enum at v1.0 actually had only 2 members (and code drifted by silently adding 2 baseline checks) or whether the enum already had 4 members (and spec was incorrect from v1.0) cannot be determined from filesystem-only access; either way, the LOCKED v1.4 wording at HEAD (`1d43858a36c17b956a345e9bfe07a9ccf82daddb`) contradicts the actual code at HEAD. v1.x changelog entries (lines 10–29) record ratifications for §3.6 (v1.2), §3.5+§2.1 (v1.2), §11.1 M3.4 deferred (v1.2), §2.2 entryAssembly/entryType wording (v1.3), §9.5 step 7 (v1.4), and §9.5.1 (v1.4) — **none** ratified §11.2 to expand baseline enumeration. The §11.2 enumeration is therefore unwhitelisted by `AUDIT_PASS_2_PROMPT.md` §5 (no Tier 2 downgrade applies). Per `AUDIT_PASS_2_PROMPT.md` §6 Tier 0 definition («Spec drift против LOCKED v1.4. Код или wording спеки противоречат друг другу.»), this is a Tier 0 spec drift. Per `AUDIT_PASS_2_PROMPT.md` §7 («Sequence integrity violations автоматически Tier 0»), this is a `sequence_integrity:count_mismatch` (cross-referenced with `sequence_integrity:cross_inconsistency` between sequence #24 spec enumeration and sequence #39 code enumeration). | **Human resolution required.** Two ratification options: **(a) Amend `MOD_OS_ARCHITECTURE.md` v1.5** with non-semantic correction expanding §11.2 baseline to enumerate `IncompatibleContractsVersion` and `WriteWriteConflict` alongside `MissingDependency` and `CyclicDependency`, mirroring v1.1/v1.2/v1.3/v1.4 non-semantic-correction ratification pattern. Suggested wording: «The current enum has `IncompatibleContractsVersion`, `WriteWriteConflict`, `MissingDependency`, and `CyclicDependency`. The migration adds:». Add v1.5 changelog entry crediting the audit (this Pass 2). **(b) Reclassify the spec wording as «relevant baseline for migration discussion, not exhaustive enum reference»** by adding a qualifier («e.g.», «among others», or «pre-migration baseline relevant to this section»). This option preserves the v1.0 spec wording shape but accepts that §11.2 is not an exhaustive enum reference. **Both options unblock Pass 2.** Recommendation: option (a) — explicit enumeration is more precise and matches the LOCKED-spec-as-architectural-authority discipline of the project. Pass 2 cannot resolve unilaterally. Subsequent passes (3, 4, 5) cannot proceed until human resolves this Tier 0 finding. |

### Tier 1 — Missing required implementation

(NOT EVALUATED — Pass 2 eager-escalated before §1–§10 spec ↔ code mapping was performed.)

### Tier 2 — Whitelist deviations confirmed compatible with v1.4

(NOT EVALUATED — whitelist verifications per `AUDIT_PASS_2_PROMPT.md` §5 deferred until Pass 2 resumes after Tier 0 resolution.)

| # | Whitelist entry (per prompt §5) | Spec section | Verification status |
|---|---|---|---|
| 1 | M5.2 cascade-failure accumulation | §8.7 (lines 678–684) | NOT VERIFIED — pass eager-escalated |
| 2 | M7 §9.2/§9.3 run-flag location | §9.2, §9.3 | NOT VERIFIED — pass eager-escalated |
| 3 | M7 §9.5/§9.5.1 step 7 ordering | §9.5, §9.5.1 | NOT VERIFIED — pass eager-escalated |
| 4 | M3.4 deferred | §11.1 M3.4 | NOT VERIFIED — pass eager-escalated |
| 5 | Phase 3 SocialSystem/SkillSystem Replaceable=false | (no spec section; ROADMAP M10.C) | NOT VERIFIED — pass eager-escalated |

### Tier 3 — Spec ↔ code minor mismatch (wording clarity)

| # | Spec section | Code path | Description | Recommendation |
|---|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE.md` §8.4 line 631 | `VersionConstraint.cs:7–14` | Spec opening «Three syntaxes are supported, all are subsets of npm/Cargo conventions:» followed by Tilde explicitly «**not supported** in v1». «Supported» is overloaded between «handled by parser» (3) and «accepted» (2). Code matches the «accepted» reading: enum has 2 members; parser rejects Tilde with error per spec §8.5 step 2. Wording could be improved to «Three syntaxes are recognised: two accepted and one rejected.» | Backlog non-semantic correction in next non-semantic-correction ratification cycle. Not blocking. |
| 2 | `MOD_OS_ARCHITECTURE.md` §9.1 line 692 + diagram lines 696–724 | (no code representation; no `ModLifecycleState` enum) | Spec wording «six well-defined states» counts diagram boxes (6) but matches only 5 unique state-name labels — `Disabled` appears at both initial and terminal positions in the lifecycle cycle. Diagram is internally consistent (the cycle returns to `Disabled` deliberately). No code drift (no enum exists). Wording could be improved to «six lifecycle positions, five distinct states» or analogous. | Backlog non-semantic correction in next non-semantic-correction ratification cycle. Not blocking. |

### Tier 4 — Cosmetic

| # | Spec section / code path | Description | Recommendation |
|---|---|---|---|
| 1 | `ContractValidator.cs:12–48` (class XML-doc) vs. `ContractValidator.cs:88–103` (`Validate()` body) | Class doc per-phase prose enumeration order (A, B, E, C, D, F, G, H) does not match invocation order (A, B, E, G, H, then conditionally C/D, then conditionally F). The class doc's normative claim is the conditionality summary at lines 41–44, which IS consistent with invocation. Per-phase prose enumeration is descriptive (introduces each phase by responsibility), not prescriptive about execution order. No behaviour impact, no contract impact. | Optional surgical fix in a future review batch: reorder per-phase prose enumeration in the class XML-doc to match invocation order (A, B, E, G, H, C, D, F), keeping the conditionality summary intact. Not blocking. |

### Out-of-scope items observed (for Pass 3/4)

| # | Anomaly | Source | Routing |
|---|---|---|---|
| 1 | Pass 1 anomaly #1 — source-level test-attribute count 359 vs. ROADMAP-stated runtime test count 369 | `tests/`, `ROADMAP.md:36` | Pass 3 (engine snapshot verification) |
| 2 | Pass 1 anomaly #2 — closure review locations (`docs/audit/` vs. previously `docs/`); session-start `git status` shows `D docs/M*_CLOSURE_REVIEW.md` and `?? docs/audit/` | `docs/audit/`, session-start git status | Resolved structurally before Pass 1 (per Pass 1 §11 #2 note); Pass 4 verifies clean working-tree state and audit-trail integrity |
| 3 | Pass 1 anomaly #6 — `tests/DualFrontier.Modding.Tests/README.md` claims «Real tests will arrive in Phase 2» but folder has 31 `.cs` files | `tests/DualFrontier.Modding.Tests/README.md` | Pass 4 (sub-folder README accuracy) |
| 4 | Pass 1 anomaly #7 — `tests/DualFrontier.Systems.Tests/README.md` claims «Real tests will arrive in Phase 2+» but folder has 6 test `.cs` files | `tests/DualFrontier.Systems.Tests/README.md` | Pass 4 (sub-folder README accuracy) |
| 5 | Pass 1 anomaly #8 — bus count cross-doc inconsistency: `Contracts/README.md:17` lists 5 buses, `IGameServices.cs:13–57` declares 6 properties, `Contracts/Bus/README.md:5` describes 6 buses | `src/DualFrontier.Contracts/README.md:17`, `src/DualFrontier.Contracts/Bus/IGameServices.cs:13–57`, `src/DualFrontier.Contracts/Bus/README.md:5` | Pass 4 (sub-folder README accuracy and cross-doc consistency) |
| 6 | Pass 1 anomaly #9 — `AUDIT_PASS_1_PROMPT.md` §4 contract table referenced closure reviews under `docs/M*_CLOSURE_REVIEW.md` rather than `docs/audit/M*` | `docs/audit/AUDIT_PASS_1_PROMPT.md:84–88` | Inventory observation; Pass 1 prompt-quality issue, not Pass 2 scope |
| 7 | Pass 1 anomaly #10 — `AUDIT_PASS_1_PROMPT.md` Appendix A example branch is `feat/m4-shared-alc`; actual `.git/HEAD` is `main` | `docs/audit/AUDIT_PASS_1_PROMPT.md:691–711`, `.git/HEAD:1` | Inventory observation; Pass 1 prompt-quality issue, not Pass 2 scope |
| 8 | Pass 1 anomaly #11 — `mods/DualFrontier.Mod.Example/mod.manifest.json` contains `description` field; spec §2.2 field reference table does not list `description`. Manifest also omits `kind` (defaults to `regular` per `ModManifest.cs:82`). | `mods/DualFrontier.Mod.Example/mod.manifest.json`, `MOD_OS_ARCHITECTURE.md:209–224`, `src/DualFrontier.Contracts/Modding/ModManifest.cs:82` | **Pass 2 §2 Manifest schema** — NOT CLASSIFIED in this Pass (eager-escalated before §2 mapping). Will be classified upon Pass 2 resumption. Hypothesis (per prompt §15.1): Tier 1 if field required and absent; Tier 3 if parser ignores informational field. |
| 9 | Pass 1 anomaly #12 — M5/M6 closure-review header line 9 reports branch `feat/m4-shared-alc`; HEAD branch is `main`; `.git/logs/HEAD` shows `checkout: moving from feat/m4-shared-alc to main` event between commits `c7210ca` and `b504813` | `docs/audit/M5_CLOSURE_REVIEW.md:9`, `docs/audit/M6_CLOSURE_REVIEW.md:9`, `.git/HEAD`, `.git/logs/HEAD` | Pass 3 (three-commit invariant / branch state consistency in roadmap-reality verification) |

---

## §14 Verification end-state

- **§0 Executive summary:** 0/11 PASSED, 1/11 FAILED (§11 Sequence integrity), 10/11 NOT EXECUTED.
- **Total findings (sequence integrity sub-pass only):** Tier 0: **1**, Tier 1: 0 (not evaluated), Tier 2: 0 (not evaluated), Tier 3: 2, Tier 4: 1.
- **Eager escalation triggered:** **YES** — at §11 sequence #24 (`MOD_OS_ARCHITECTURE.md` §11.2 ValidationErrorKind baseline enumeration vs. `ValidationError.cs:9–83`).
- **Surgical fixes applied:** 0 (per contract).
- **Pass 2 status:** **INCOMPLETE — eager-escalated on Tier 0**. §1–§10 spec ↔ code mapping (steps 3–12 of methodology in `AUDIT_PASS_2_PROMPT.md` §10) was NOT performed. Whitelist verifications per `AUDIT_PASS_2_PROMPT.md` §5 NOT performed. Subsequent passes (3, 4, 5) cannot proceed until the human resolves the Tier 0 finding documented in §13 Tier 0.
- **Resumption path:** After Tier 0 resolution (option (a) v1.5 spec amendment or option (b) wording reclassification per §13 Tier 0 recommendation), Pass 2 may be resumed from methodology step 3 (§1 Mod topology) of `AUDIT_PASS_2_PROMPT.md` §10. The sequence integrity sub-pass is complete and need not be re-run unless the v1.5 amendment introduces additional sequence claims.
