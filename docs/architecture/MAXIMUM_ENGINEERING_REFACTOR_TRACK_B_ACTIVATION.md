---
title: Maximum Engineering Refactor — Track B Activation
nav_order: 111
---

# Track B Activation — Type System Verification

**Version**: 0.1
**Date**: 2026-05-09
**Status**: RATIFIED v0.1 — activation brief authored ahead of activation per `MAXIMUM_ENGINEERING_REFACTOR.md` v1.0 §5.3. Activation gated on K-series closure per §3. Brief stands as the canonical specification of the M-phase scope; the M-phase itself opens at the activation trigger.
**Companion documents**: [MAXIMUM_ENGINEERING_REFACTOR](./MAXIMUM_ENGINEERING_REFACTOR.md) (parent v1.0; this brief realises §3 Track B), [ARCHITECTURE_TYPE_SYSTEM](./ARCHITECTURE_TYPE_SYSTEM.md) (v0.1 LOCKED specification of the analyzer family this brief activates), [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) (K-series progression that determines activation timing), [METHODOLOGY](./METHODOLOGY.md) (§2.4 atomicity rule applied to repair-pass closure), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) (v1.2 §11.1 M3.4 closed by §4.3 of this brief).
**Scope**: Activation specification for Track B of the Maximum Engineering Refactor. Defines the activation trigger, the M-phase shape, the deliverables, the acceptance criteria for the composite pilot (`SystemAccessCompletenessAnalyzer` + `ModCapabilitiesHonestyAnalyzer`) and the structured repair-pass that follows. Does **not** redefine the analyzer family — the family is specified in `ARCHITECTURE_TYPE_SYSTEM.md` v0.1 and consumed here.

**Version history:**

- **v0.1** (this version, 2026-05-09) — initial ratification. Activation trigger fixed at K-series closure (post-K9). Composite pilot scope inherits from `ARCHITECTURE_TYPE_SYSTEM.md` §4. M-phase decomposition at activation time (Sonnet's responsibility, per pipeline conventions). Repair-pass discipline locked. Closes `MOD_OS_ARCHITECTURE.md` v1.2 §11.1 M3.4 by integration into the composite pilot.

---

## Preamble — How to use this document

**Authority.** This document is the activation specification for Track B. The parent specification (`MAXIMUM_ENGINEERING_REFACTOR.md` §3) defines what Track B *is*; the family specification (`ARCHITECTURE_TYPE_SYSTEM.md`) defines what the Track B analyzers *enforce*; this document defines *when, in what order, with what discipline* the work is done. Disagreement at activation time is escalated through the §5.2 ratification process of the parent document, never resolved by improvising the M-phase scope.

**Authoring discipline.** This brief is authored *before* activation, not at activation time. The rationale is the same as for `ARCHITECTURE_TYPE_SYSTEM.md` v0.1 §3.5 and §9: ratification before observation fixes the contract on its merits, before the size of the post-K finding count creates a leniency anchor. Activating on a brief authored at activation time is structurally weaker — the brief becomes a rationalisation of the scope the project finds convenient at that moment rather than a specification of the scope the architecture requires.

**Section structure.** This brief follows the structure other activation-style briefs in the corpus follow: §1 trigger, §2 scope inheritance, §3 sequencing, §4 deliverables, §5 acceptance, §6 repair-pass, §7 risks, §8 decision log. Section §3 carries the activation gate.

---

## 1. Activation trigger

**Trigger:** K-series closure, defined as K9 closure-review event per `KERNEL_ARCHITECTURE.md` Part 2.

K9 is the field-storage-abstraction milestone; its closure marks the architectural settling point of the kernel migration. K0–K7 are closed at the time of this brief (K7 closure note dated 2026-05-09, see `PERFORMANCE_REPORT_K7.md`). K8 is the cutover decision step (§"Recommended K8 outcome direction" of the K7 report); K9 follows the chosen K8 outcome. Track B activation does **not** open at K8 closure — it waits for K9 closure to ensure the corpus has settled on the post-cutover access patterns end-to-end. K-series-internal milestones beyond K9 (if any are added through normal `KERNEL_ARCHITECTURE.md` v1.x evolution) re-evaluate this trigger at the time of their addition.

**Why K9 specifically:**

- K8 cutover decides whether managed-with-structs or native-with-batching wins. The decision is upstream; the analyzer's surface for `[SystemAccess]` site-of-use detection (`SystemBase.GetComponent<T>`, `SetComponent<T>`, `Query<T>`, plus K-L7's `SpanLease<T>` / `WriteBatch<T>` if Outcome 1 lands) depends on the decision. Activation before K8 forces the analyzer to support both surfaces speculatively or to be rewritten after the decision; both are costs the structural sequencing avoids.
- K9 introduces field types as a third access category (per `MOD_OS_ARCHITECTURE.md` v1.6 capability syntax extension). The analyzer's scope expands to include `[FieldAccess]` and `[ComputePipelineAccess]` if those attributes have shipped by then; activation-after-K9 admits the broader scope cleanly. Activation before K9 forces a Phase-2 amendment whose only purpose is to absorb the K9 surface — structurally the same work, paid in two installments instead of one.
- K9 closure is a structural event (a closure review per `METHODOLOGY.md` §2.4), not a date. The activation trigger is therefore unambiguous and externally verifiable: the K9 closure-review document exists and is approved, or it does not.

**What K-series closure does *not* mean for this trigger:**

- It does not mean the K-series implementation is "perfect." The analyzer's job is precisely to surface the residual archaeology that K-series closure leaves behind (per §6 of this brief and §6.3 of `ARCHITECTURE_TYPE_SYSTEM.md`).
- It does not mean further kernel evolution stops. Future K-series-style work (post-K9 milestones, if any) interacts with the analyzer through the §3.3 drift policy of the family specification — synchronous lockstep, not gating-with-rebuild.
- It does not mean Track A or Track C cannot activate first. Per `MAXIMUM_ENGINEERING_REFACTOR.md` §5.1, tracks activate independently. Track B's K9 gate is specific to Track B; Tracks A and C have their own activation conditions documented in their own briefs (when those are authored).

## 2. Scope inheritance

This brief does not redefine analyzer scope. The composite pilot's scope is fully specified by:

- `ARCHITECTURE_TYPE_SYSTEM.md` §1.2 — the two pilot instances of the *attribute-as-declaration* family.
- `ARCHITECTURE_TYPE_SYSTEM.md` §3 — operational rules (severity, suppression, drift, over-declaration).
- `ARCHITECTURE_TYPE_SYSTEM.md` §4.4 — deliverables structure (illustrative; this brief makes them concrete in §4 below).
- `ARCHITECTURE_TYPE_SYSTEM.md` §4.5 — analyzer-level acceptance criteria.

Where this brief and the family specification disagree, the family specification governs. This brief is the realisation; the family specification is the contract.

## 3. Sequencing

### 3.1 Pre-activation period

K-series closure is the trigger; the period from this brief's ratification (2026-05-09) to that trigger is the **pre-activation period**. During this period:

- Track B does not open as an active M-phase. No analyzer code is written. No `DualFrontier.Analyzers/` csproj is created.
- This brief and the family specification (`ARCHITECTURE_TYPE_SYSTEM.md` v0.1) are alive — non-semantic v0.x corrections are admitted if K8/K9 surface decisions reveal anchoring-rule details that v0.1 mis-stated. The §5.2 ratification process governs.
- `ROADMAP.md` Backlog records the pre-activation status; activation surfaces the M-phase into the active queue at trigger time.

### 3.2 Activation moment

K9 closure-review event is approved. At that moment:

1. Sonnet is invited to decompose this brief into a concrete M-phase brief in the standard `M{N}_CLAUDE_CODE_PROMPT.md` form. The decomposition produces the activation M-phase number (probably `M10` or `M11`, depending on the `ROADMAP.md` queue at the time; the number is not load-bearing).
2. Crystalka ratifies the decomposed M-phase brief in the usual pipeline shape — same gates as any other M-phase brief.
3. Implementation proceeds in the standard pipeline: Gemma writes the analyzer code per the M-phase prompt; Opus aud the result; Crystalka reviews and merges the closure.

The structural shape — analyzer csproj, two analyzers, test corpus, `AnalyzerReleases.Shipped.md` — is fixed by §4 below. The M-phase prompt fills in implementation specifics (Roslyn API surface choice, `DF_TS_<n>` allocation, etc.) per the deferred-questions list in `ARCHITECTURE_TYPE_SYSTEM.md` §8.

### 3.3 Post-implementation

Implementation closes when the analyzers are present in the codebase and pass their own test corpus, **but are not yet enabled in CI as errors**. CI integration is deferred to post-repair-pass per §6.4. The intermediate state — analyzers present, dry-mode against the corpus, finding list produced — is the entry state for the repair pass.

### 3.4 Critical sequencing constraint

The repair pass (§6) **must** complete before the analyzers are enabled in CI as errors. The reason is mechanical: enabling errors-on-master with N>0 known violations breaks the build, blocking all unrelated work. The analyzer's value is realised at zero violations, not before; sequencing the enable-in-CI step after closure of the repair pass is the structural discipline that admits the analyzer's value without shipping a broken build.

This is why §3.3's "implementation closes" is **not** "Track B closes." Track B closes at repair-pass closure — see §6.4.

## 4. Deliverables

### 4.1 Analyzer infrastructure

```
src/DualFrontier.Analyzers/
  DualFrontier.Analyzers.csproj          (netstandard2.0, Roslyn analyzer SDK)
  AnalyzerReleases.Shipped.md            (initially empty; populated at first release)
  AnalyzerReleases.Unshipped.md          (DF_TS_001..DF_TS_N at implementation)
  Diagnostics/
    DiagnosticIds.cs
    DiagnosticDescriptors.cs             (severity = Error, anchored on family §3.1)
    LocalisableStrings.resx              (English-only at v0.1; translation campaign per TRANSLATION_PLAN tracks resource files separately)
  Internals/
    AttributeReader.cs                   (shared symbol-level attribute resolver)
    SystemBaseDetector.cs                (recognises SystemBase subclasses through inheritance walk)
  README.md                              (developer-facing: how to add a new analyzer to the family)
```

### 4.2 SystemAccessCompletenessAnalyzer

```
src/DualFrontier.Analyzers/SystemAccess/
  SystemAccessCompletenessAnalyzer.cs    (DiagnosticAnalyzer entrypoint)
  SystemAccessSymbolReader.cs            (consumes [SystemAccess] via ISymbol.GetAttributes())
  SystemAccessCallSiteWalker.cs          (CFG/SyntaxWalker over Execute body and called helpers)
  SystemAccessRules.cs                   (the predicate: "actual operation set ⊆ declared set" + "writes implicitly grant reads", anchored on SystemExecutionContext.IsReadAllowed / IsWriteAllowed)
```

### 4.3 ModCapabilitiesHonestyAnalyzer (closes M3.4)

```
src/DualFrontier.Analyzers/ModCapabilities/
  ModCapabilitiesHonestyAnalyzer.cs
  CapabilityTokenParser.cs               (tokens → typed declarations; verb.subject grammar per MOD_OS_ARCHITECTURE §3.2)
  CapabilityCallSiteWalker.cs            (Publish<T> / Subscribe<T> + [ModAccessible] reads / writes / field operations / pipeline operations per v1.6 syntax)
  CapabilityResolutionScope.cs           (within-mod-only at v0.1 per ARCHITECTURE_TYPE_SYSTEM.md §9 falsifiability — cross-mod resolution deferred to Phase 2 if structural feasibility check passes at implementation time)
```

The within-mod-only scope at first release is the structural risk admission from `ARCHITECTURE_TYPE_SYSTEM.md` §9: cross-mod token resolution at compile time may be impossible without a build-graph the project does not have. The within-mod scope is a non-trivial subset and ships at first release; the cross-mod extension lands as a Phase-2 amendment with its own ratification cycle if and only if a feasibility check passes.

### 4.4 Test infrastructure

```
tests/DualFrontier.Analyzers.Tests/
  DualFrontier.Analyzers.Tests.csproj    (Microsoft.CodeAnalysis.CSharp.Analyzers.Testing)
  SystemAccessCompletenessAnalyzerTests.cs
  ModCapabilitiesHonestyAnalyzerTests.cs
  Fixtures/
    SystemAccess/
      Positive_*.cs                      (correctly-declared systems, no diagnostics)
      Negative_UnderDeclaration_*.cs     (read undeclared component → DF_TS_001)
      Negative_OverDeclaration_*.cs      (declared but unused → DF_TS_002 per TS-D-3)
      Boundary_WriteImpliesRead_*.cs     (writes-only declaration, reads same type → no diagnostic per family §2.3)
      Boundary_DynamicReceiver_*.cs      (analyzer's known unreachability documented)
    ModCapabilities/
      Positive_*.cs
      Negative_UndeclaredPublish_*.cs
      Negative_UndeclaredSubscribe_*.cs
      Negative_OverDeclared_*.cs
      Boundary_WithinModResolution_*.cs
```

### 4.5 Documentation

- `src/DualFrontier.Analyzers/README.md` — developer-facing entry point: how to consume the analyzers, how to add a new one to the family, link to family specification.
- `AnalyzerReleases.Shipped.md` — Roslyn convention for diagnostic-id history; first entry covers DF_TS_001..DF_TS_N at first release.
- `docs/architecture/ARCHITECTURE_TYPE_SYSTEM.md` v0.x non-semantic correction (if any) recording diagnostic-id allocation in §1.2 / §7 (TS-D-8). The correction lands in the same commit as the analyzer release.
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.7 non-semantic correction reworking §11.1 M3.4 entry as "subsumed into ARCHITECTURE_TYPE_SYSTEM.md §4.1, see Track B activation. Closed at <DATE>." — lands at the same commit as repair-pass closure.

### 4.6 Build pipeline integration

`Directory.Build.props` updates:

- Add `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` if not already set (verify at activation time).
- Add `<WarningsAsErrors>$(WarningsAsErrors);DF_TS_001;DF_TS_002;...</WarningsAsErrors>` for explicit promotion of every analyzer family diagnostic.
- Add the analyzer project as a `<ProjectReference>` with `<OutputItemType>Analyzer</OutputItemType>` and `<ReferenceOutputAssembly>false</ReferenceOutputAssembly>` to all consuming projects (DualFrontier.Core, DualFrontier.Systems, DualFrontier.Application, mods/*). The exact set is enumerated in the M-phase brief at activation.

## 5. Acceptance criteria

### 5.1 Analyzer-level

Per `ARCHITECTURE_TYPE_SYSTEM.md` §4.5, restated here for activation reference:

- [ ] Diagnostics under `DF_TS_<n>` family. Specific allocation in `AnalyzerReleases.Shipped.md`.
- [ ] Severity = `Error`. No warning-mode in any environment, including local development.
- [ ] No suppression mechanism accepted as a valid bypass. Analyzer tests include explicit assertion that `[SuppressMessage]` does not suppress (the diagnostic is `IsConfigurable = false` at descriptor level, or equivalent).
- [ ] Anchoring rule honoured. Each analyzer has a class-header comment naming the runtime guard it projects (`SystemExecutionContext.IsReadAllowed/IsWriteAllowed` for SystemAccess, `RestrictedModApi.EnforceCapability` + `ContractValidator.Phase D` for ModCapabilities).
- [ ] Test corpus complete per §4.4 (positive + under + over + boundary cases).
- [ ] Performance — analyzer adds no more than 200 ms to the full-solution `dotnet build` warm wall-clock (measured on the project's standard benchmark machine; budget revisable by §5.2 ratification).

### 5.2 Family-level (this brief's contribution)

- [ ] `DualFrontier.Analyzers/` csproj exists and is referenced as analyzer reference (not regular reference) by consuming projects.
- [ ] `Directory.Build.props` propagates the family's diagnostic ids to `WarningsAsErrors`.
- [ ] `AnalyzerReleases.Shipped.md` lists each diagnostic with id, severity, message format.
- [ ] Family-specification (`ARCHITECTURE_TYPE_SYSTEM.md`) §1.2 table is updated (non-semantic v0.x correction) recording the shipped analyzers and their diagnostic ids.
- [ ] M3.4 closure recorded in `MOD_OS_ARCHITECTURE.md` v1.7 (non-semantic correction).

### 5.3 Process-level

- [ ] M-phase brief authored by Sonnet, ratified by Crystalka, before any analyzer code is written (standard pipeline gate).
- [ ] Analyzer implementation by Gemma against the M-phase brief.
- [ ] Audit by Opus against this brief, the family specification, and the M-phase brief.
- [ ] Closure review per `METHODOLOGY.md` §2.4 — the *implementation closure*, not the Track B closure. Track B closes only at §6.4.

## 6. Repair pass

### 6.1 Definition

The repair pass is the structured run of each shipped analyzer against the post-K corpus, the resolution of every produced finding, and the closure event that flips the analyzers from dry-mode to error-mode in CI.

The repair pass is **part of Track B**, not a follow-up. Track B does not close at "the analyzers compile and self-test"; it closes at "the analyzers run on the corpus, all findings are resolved, and CI is gated." The structural reason: an analyzer that exists but does not gate CI is dead code from an architectural-enforcement perspective. Closure is a falsifiable claim that the corpus is clean; the repair pass is the work that makes the claim true.

### 6.2 Finding triage

Per `ARCHITECTURE_TYPE_SYSTEM.md` §6.3, each finding is one of three classes:

| Class | Resolution |
|---|---|
| Real archaeological violation | Fix the code or the declaration. Standard scoped commit. |
| Spec defect | Ratify a v0.x or v1.x correction of the family specification. Re-run analyzers; the diagnostic disappears. |
| Analyzer defect | Fix analyzer code; add regression test; re-run; diagnostic disappears. |

Triage discipline:

- Each finding is classified before resolution, not at resolution time. Classifying after resolving introduces post-hoc rationalisation; classifying before forces the per-finding architectural reasoning.
- Classifications are recorded in a per-pass log (`docs/audits/M{N}_repair_pass_log.md` or similar — exact location per M-phase convention at activation time). The log is not a temporary artefact; it lands in the repository as part of repair-pass closure.
- Class-2 (spec defect) findings are rare but not impossible. The repair pass is one of the few structural mechanisms for surfacing them; suppressing one to "ship the analyzer" is forbidden.

### 6.3 Commit discipline

Per `ARCHITECTURE_TYPE_SYSTEM.md` `TS-D-7`:

- Repair-pass commits are small. Each commit fixes one finding (or one tightly-coupled cluster of related findings — for example, a single system whose declaration tightens by removing two unused reads in one edit).
- Commit messages follow the project's standard scope-prefixed form: `refactor(systems): tighten NeedsSystem [SystemAccess] to actual reads`, `fix(mods/combat): declare ammo.consume capability used at WeaponFireSystem:142`, etc.
- The repair-pass commits are visible in `git log` as a recognisable cluster — they ship under the activation M-phase number, not under a new "repair" milestone. The cluster's bracketing is the M-phase opening commit and the M-phase closure commit.

### 6.4 Repair-pass closure

Repair-pass closure is the Track B closure event. Closure conditions:

- [ ] Finding count is zero on full-solution build with all analyzers in error-mode.
- [ ] No suppression entries in `Directory.Build.props`, csproj `<NoWarn>`, `.editorconfig`, or per-file `#pragma warning disable` (verified by repository-wide grep at closure).
- [ ] All Class-2 findings (if any) are reflected in committed v0.x or v1.x corrections of `ARCHITECTURE_TYPE_SYSTEM.md`.
- [ ] All Class-3 findings (if any) are reflected in committed analyzer-code fixes with regression tests.
- [ ] CI configuration switched from analyzer dry-mode to error-mode (the actual flip-the-flag commit is the closure marker).
- [ ] Closure-review document authored per `METHODOLOGY.md` §2.4 — same shape as any other M-phase closure.
- [ ] `MAXIMUM_ENGINEERING_REFACTOR.md` v1.x non-semantic correction landed: §3 Track B status updated from "ratified, deferred" to "active, repair-pass closed at <DATE>".
- [ ] `ROADMAP.md` Backlog "Maximum engineering refactor (parallel track)" Track B entry updated to closed status.

### 6.5 Atomicity

Per `METHODOLOGY.md` §2.4, once repair-pass closure is approved, the corpus is canonical at that snapshot. Subsequent work that re-introduces an analyzer-family violation is a regression caught at PR time by CI; it does not re-open the repair pass.

## 7. Risks

### 7.1 K9 closure delays activation indefinitely

K-series closure is open-ended. K8 cutover decision may extend; K9 implementation may extend. The longer the wait, the more archaeology accumulates in `[SystemAccess]` declarations across new content milestones (M-phases that ship between this brief's ratification and K9 closure).

**Mitigation:** the activation gate is structural, not aspirational. Accumulating archaeology is the *expected* behaviour of the gate — the analyzer is precisely the structural mechanism for surfacing it after the corpus settles. Activating earlier to "stay ahead" of accumulation is what `MAXIMUM_ENGINEERING_REFACTOR.md` §6.2 of `ARCHITECTURE_TYPE_SYSTEM.md` argues against.

**Residual risk:** if K-series closure does not land within (say) twelve months of this brief's ratification, the gate may need re-examination through the §5.2 ratification process. The threshold is not load-bearing; revisit when relevant.

### 7.2 Repair-pass finding count exceeds budget

The first run on the post-K corpus may produce more findings than the M-phase budget anticipated. This is `ARCHITECTURE_TYPE_SYSTEM.md` §9 falsifiability condition #1.

**Mitigation:** the repair pass is not budget-locked. If finding count exceeds budget, ratify a v0.x correction admitting a longer pass with named partial-closure milestones. The analyzers remain in dry-mode in CI until the repair pass closes; partial closure is admitted explicitly, not by silent suppression.

### 7.3 Within-mod-only scope of ModCapabilitiesHonestyAnalyzer turns out to leave significant residual coverage gap

If most capability honesty issues in the corpus involve cross-mod token resolution, the within-mod scope at first release is a thin slice.

**Mitigation:** measured at repair-pass time. If the within-mod scope produces few findings while runtime `CapabilityViolationException`s continue at non-trivial rate in playtesting, the cross-mod extension is escalated to Phase 2 with explicit ratification. The analyzer's value is admitted as partial in v0.1 documentation; partial-coverage analyzers are still better than no analyzer.

### 7.4 Roslyn API constraints on dynamic / reflection / generated code

The analyzer cannot prove violations through reflection or dynamic dispatch. The runtime guard remains the safety net; the analyzer's coverage is documented as "static-call coverage."

**Mitigation:** acknowledged as `ARCHITECTURE_TYPE_SYSTEM.md` §2.4 (complementary coverage). Documented at the analyzer's class header. No mitigation needed beyond clear scope statement.

### 7.5 The brief itself goes stale during pre-activation

K-series surface decisions in the K8/K9 work may invalidate specific deliverables in §4.

**Mitigation:** v0.x non-semantic corrections of this brief are admitted during pre-activation. Each correction lands a small commit with the standard ratification footer. The brief is alive, not frozen.

## 8. Decision log

| ID | Decision | Rationale |
|---|---|---|
| `TBA-D-1` | Activation trigger = K9 closure, not K8 | §1: K9 introduces field types and settles post-cutover access patterns end-to-end. K8-only would force Phase-2 amendments. |
| `TBA-D-2` | Repair pass is part of Track B, not a follow-up | §6.1: an analyzer not gating CI is dead code architecturally; closure must be the gating event. |
| `TBA-D-3` | Brief authored ahead of activation | §"Authoring discipline": ratification before observation defends against leniency anchor proportional to finding count. |
| `TBA-D-4` | Within-mod-only scope at v0.1 for ModCapabilitiesHonestyAnalyzer | §4.3 + §7.3: cross-mod resolution feasibility unproven; within-mod is non-trivial subset; cross-mod extension as Phase 2 with own ratification. |
| `TBA-D-5` | Performance budget = 200 ms on full-solution build (revisable) | §5.1: concrete target with measurable falsification, not aspirational. The number is rough; revision through standard ratification. |
| `TBA-D-6` | M3.4 closes by integration, not by separate work | §4.3 + §5.2: subsuming M3.4 into the composite pilot avoids two parallel infrastructures. The M3.4 work item is preserved by reference in `MOD_OS_ARCHITECTURE.md` v1.7 non-semantic correction. |

---

## 9. Falsifiability conditions for this brief

This brief is itself falsifiable in the same shape as `ARCHITECTURE_TYPE_SYSTEM.md` §9 and `MAXIMUM_ENGINEERING_REFACTOR.md` §8:

- **If K9 closure does not happen within twelve months** of this brief's ratification, the trigger may need re-examination. Threshold not load-bearing.
- **If the within-mod scope of `ModCapabilitiesHonestyAnalyzer` turns out to cover <20% of capability honesty issues observed in playtesting**, escalate cross-mod scope to Phase 2 with explicit ratification — `TBA-D-4` revisable.
- **If the performance budget (`TBA-D-5`) is consistently exceeded**, raise the budget through ratification or refactor the analyzer to fit. Suppressing the budget by ignoring the measurement is not a valid response.
- **If the repair-pass finding count exceeds 5× the activation M-phase planned budget**, the corpus archaeology is structurally larger than the brief anticipated; admit through v0.x correction with named partial-closure milestones.

The brief is a hypothesis about how Track B's activation discipline plays out under post-K corpus reality. Its predictions are testable; its decisions are revisable.

---

## 10. Notes for the future architect

When the M-phase brief at activation time is authored (Sonnet, post-K9), the following will likely be true:

- The K8 cutover outcome is fixed. The analyzer's site-of-use surface (§4.2) is concrete; the M-phase brief specifies which `SystemBase` API to walk in CFG terms (managed paths, native paths via K-L7's `SpanLease<T>`/`WriteBatch<T>` if Outcome 1, etc.).
- K9 field types have shipped. `[FieldAccess]` and `[ComputePipelineAccess]` attributes exist in the corpus. The composite pilot may need to absorb these as additional analyzers in the family, or — if their semantics are isomorphic enough to `[SystemAccess]` — as additional cases in `SystemAccessCompletenessAnalyzer`. The decision is made at M-phase brief time, with §1.2 of `ARCHITECTURE_TYPE_SYSTEM.md` updated as a v0.x non-semantic correction in the same cycle.
- The state of the corpus is concrete. The repair pass's anticipated finding count can be estimated by a representative sample run from a feature branch (the M-phase brief may include this estimate; estimating ahead is fine, locking it as budget is not).
- Tracks A and C may have activated independently. Cross-track interactions, if any, are documented in their own briefs. This brief does not subsume them.

When repair-pass closure is approved and Track B closes, this brief's status moves from "RATIFIED v0.1, active" to "FULFILLED v0.1, closed." The document remains in the corpus as historical record; subsequent analyzer additions to the *attribute-as-declaration* family ratify their own briefs (or extend `ARCHITECTURE_TYPE_SYSTEM.md` directly per §6.4 of that document, depending on scale).

---

## 11. Ratification

Ratified at v0.1 on 2026-05-09 under the same discipline as
[MAXIMUM_ENGINEERING_REFACTOR](./MAXIMUM_ENGINEERING_REFACTOR.md) v1.0 and
[ARCHITECTURE_TYPE_SYSTEM](./ARCHITECTURE_TYPE_SYSTEM.md) v0.1 (same date,
companion ratification).

The brief joins the Track B documentation set on equal standing with the family
specification. Activation opens at the §1 trigger; pre-activation period admits
non-semantic v0.x corrections through the standard ratification process.

### Pending integrations

- `MAXIMUM_ENGINEERING_REFACTOR.md` v1.x non-semantic correction: §3 Track B status updated from "added to backlog per §5.3" to "ratified, activation deferred to K9 closure per `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` §1; composite pilot supersedes the §3.3 B1-first recommendation per `ARCHITECTURE_TYPE_SYSTEM.md` `TS-D-2`." Lands at the next non-semantic correction of the parent.
- `ROADMAP.md` Backlog "Maximum engineering refactor (parallel track)" Track B entry: rewrite as "Track B activation brief authored (`MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` v0.1, 2026-05-09); activation gated on K9 closure per §1; family specification in `ARCHITECTURE_TYPE_SYSTEM.md` v0.1." Lands at the next ROADMAP refresh.
- `MOD_OS_ARCHITECTURE.md` v1.7 non-semantic correction: §11.1 M3.4 entry rewritten as "subsumed into `ARCHITECTURE_TYPE_SYSTEM.md` §4.1 / `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` §4.3. Closes at Track B repair-pass closure." Lands at the next non-semantic correction of MOD_OS, no later than activation time.

These integrations are non-blocking; they land at natural touch-points of each document, not as a coordinated event.

### Version history

- **v0.1** (2026-05-09) — initial ratification. Activation trigger fixed at K9 closure; six `TBA-D-*` decisions locked; falsifiability conditions and risks documented.

---

## See also

- [MAXIMUM_ENGINEERING_REFACTOR](./MAXIMUM_ENGINEERING_REFACTOR.md) — parent v1.0; this brief realises §3 Track B per §5.3 of the parent.
- [ARCHITECTURE_TYPE_SYSTEM](./ARCHITECTURE_TYPE_SYSTEM.md) — v0.1 LOCKED family specification; this brief is its activation companion.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — K-series progression; K9 closure is this brief's activation trigger (§1).
- [PERFORMANCE_REPORT_K7](./PERFORMANCE_REPORT_K7.md) — K7 closure note dated 2026-05-09; establishes K-series progression at this brief's ratification time.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — v1.2 §11.1 M3.4 closed by integration per §4.3 / `TBA-D-6`.
- [METHODOLOGY](./METHODOLOGY.md) — §2.4 atomicity rule applied to repair-pass closure (§6.5); standard pipeline conventions for the activation M-phase (§3.2).
- [ROADMAP](./ROADMAP.md) — Backlog entry for Track B updated per "Pending integrations" above.
