---
title: Architecture Type System
nav_order: 110
---

# Architecture Type System — Attribute-as-Declaration Verification

**Version**: 0.1
**Date**: 2026-05-09
**Status**: LOCKED v0.1 — specification ratified ahead of activation; activation gated on K-series migration closure per §6.2. Implementation work begins only after K-series closure produces a stable target corpus.
**Companion documents**: `MAXIMUM_ENGINEERING_REFACTOR.md` (parent Track B specification), `MOD_OS_ARCHITECTURE.md` (capability model anchored here), `KERNEL_ARCHITECTURE.md` (post-migration corpus that anchors the verification target), `METHODOLOGY.md` (ratification process inherited).
**Scope**: Formal specification of the *attribute-as-declaration* invariant family used throughout the Dual Frontier corpus, the verification discipline that enforces these invariants at compile time via Roslyn analyzers, and the operational rules — severity, suppression policy, drift policy, anchoring rule — under which the analyzer family operates. Specifies the first two analyzers (`SystemAccessCompletenessAnalyzer`, `ModCapabilitiesHonestyAnalyzer`) as a composite pilot; subsequent analyzers (`LayerDependencyAnalyzer`, `BusChannelCorrectnessAnalyzer`, …) extend this specification in dedicated revisions.

**Version history:**

- v0.1 (this version, 2026-05-09) — initial ratification. Composite pilot (`SystemAccessCompletenessAnalyzer` + `ModCapabilitiesHonestyAnalyzer`) selected over the `MAXIMUM_ENGINEERING_REFACTOR.md` v1.0 §3 recommendation of B1 (`LayerDependencyAnalyzer`) on the structural-isomorphism rationale documented in §4.2. Activation deferred until K-series migration closure (§6.2) so the pilot operates on a stable post-migration corpus rather than a moving target. Strict over-declaration policy locked (§3.4 / TS-D-3) — declarations must match facts exactly, no forward-declaration is admitted. No-suppression discipline locked (§5.3 / TS-D-5). Severity = `error` from day one (§5.2 / TS-D-4) — no warning-mode transition period. Closes `MOD_OS_ARCHITECTURE.md` v1.2 §11.1 M3.4 deferred milestone (capability analyzer subsumed into pilot per §4.3).

---

## Preamble — How to use this document

**Authority.** This document is the architectural authority for compile-time verification of *attribute-as-declaration* invariants in the Dual Frontier corpus. Every Roslyn analyzer that participates in this verification family — present and future — derives its severity, anchoring rule, suppression policy, and drift handling from sections here. Disagreement with the specification is escalated through the `MAXIMUM_ENGINEERING_REFACTOR.md` ratification process (§5.2 of that document), never resolved by per-analyzer improvisation.

**Scope.** The specification governs:

- The *attribute-as-declaration* invariant family — what counts as one, how it is recognised, and how it is verified.
- The anchoring rule that binds compile-time analyzer behaviour to runtime guard behaviour through a single declarative source.
- Severity, suppression, and drift policy applied uniformly to every analyzer in this family.
- Over-declaration semantics (the analyzer's interpretation of declarations that exceed actual usage).
- The first two analyzers (composite pilot) and their relationship to runtime guards already in production.
- Activation sequencing relative to the K-series migration.
- Repair-pass discipline for the initial run against the post-migration corpus.

The specification does **not** govern:

- Analyzer implementation details (specific `SyntaxNodeAnalysisContext` walker patterns, `ISymbol` resolution choices) — covered by per-analyzer M-phase briefs at implementation time.
- Verification of invariants outside the *attribute-as-declaration* family. Layer dependency rules (`B1` per `MAXIMUM_ENGINEERING_REFACTOR.md` §3.2), bus channel correctness (`B3`), and other architectural invariants are specified in dedicated revisions of this document or in companion specifications.
- Formal verification of dynamic properties — see `MAXIMUM_ENGINEERING_REFACTOR.md` Track A.
- Capability *granularity* decisions — those remain in `MOD_OS_ARCHITECTURE.md` and are consumed here, not redefined.

**The "stop, escalate, lock" rule.** When implementation encounters a verification question not answered here — for example, a Roslyn-level corner case where attribute symbols cannot be resolved cleanly through `ISymbol.GetAttributes()` — the response is "stop, document, wait for the human to lock" — not "guess." This document and the M-phase brief at activation time are the only architectural authorities; analyzer code does not adjudicate ambiguity in either.

---

## 1. The attribute-as-declaration invariant family

### 1.1 Pattern definition

An *attribute-as-declaration* invariant has three parts:

1. **A declarative attribute** placed on a class (or other code element). The attribute carries data that scopes which subset of some operation set the element is permitted to perform — for example, "this system reads these component types, writes those component types, publishes on this bus."
2. **A runtime guard** that, given the attribute's data, validates each actual operation against the declaration when the operation occurs. The guard's contract is: "if the operation is in the declared subset, proceed; otherwise, raise a typed diagnostic."
3. **A site-of-use** — code where operations corresponding to the declaration are performed. The relationship that the invariant fixes is: *every operation at a site-of-use has a corresponding entry in the declaring attribute on the enclosing class.*

### 1.2 Instances in the current corpus

Two instances of this pattern exist in the Dual Frontier corpus at v0.1 ratification time:

| Instance | Declarative attribute | Runtime guard | Site-of-use |
|---|---|---|---|
| Kernel system access | `[SystemAccess(reads, writes, bus/buses)]` on classes deriving from `SystemBase` | `SystemExecutionContext` (DEBUG `IsolationViolationException` / Mod-origin `IModFaultSink` route) | `SystemBase.GetComponent<T>` / `SetComponent<T>` / `Query<T>` / `Query<T1,T2>` calls inside system bodies |
| Mod capability honesty | `[ModCapabilities(tokens)]` on classes deriving from `SystemBase` registered through `IModApi` | `RestrictedModApi.EnforceCapability` (runtime `CapabilityViolationException`) plus `ContractValidator` Phase D (load-time `MissingCapability`) | `RestrictedModApi.Publish<T>` / `Subscribe<T>` calls and `[ModAccessible]` component access inside mod system bodies |

These two instances are **structurally isomorphic** under §1.1 — same triple (attribute, guard, site-of-use), same fix-relationship (operations subset declarations). The §4 composite pilot is the consequence of this isomorphism: a single analyzer infrastructure handles both with one set of conventions, rather than two parallel infrastructures with two sets of conventions that drift over time.

Future instances are admitted by the same definition. When a new attribute-and-guard pair lands in the corpus, it joins this family by extending §1.2 in a non-semantic v0.x correction; no new philosophical work is required.

### 1.3 Non-instances

The following are *not* attribute-as-declaration invariants under §1.1, and are therefore not in scope for this specification:

- **Layer dependency rules** (Domain may not import `Godot;`, etc.). The "declaration" is the project-level `csproj` reference set, not an attribute on a code element. The site-of-use is the assembly-level `using` directive, not a method call inside a declaring class. Verification belongs in a dedicated analyzer (`LayerDependencyAnalyzer`, see §6.4) governed by a separate revision of this document.
- **`[Deferred]` / `[Immediate]` event marker semantics**. The attribute declares delivery timing, not a subset of allowed operations; there is no "site-of-use" relationship to fix.
- **`[BridgeImplementation(Phase, Replaceable)]`**. The attribute declares lifecycle metadata consumed by `ContractValidator` Phase H and the mod loader; it does not scope an operation set the class performs.
- **`[TickRate(rate)]`**. The attribute declares scheduling cadence consumed by `ParallelSystemScheduler`; it does not scope operations.

The non-instances are perfectly valid attributes — they are simply outside the family this specification verifies.

---

## 2. The anchoring rule

### 2.1 Statement

**The declarative attribute is the single source of truth.** The analyzer reads it through `ISymbol.GetAttributes()` (Roslyn surface). The runtime guard reads it through `Type.GetCustomAttribute<T>()` (reflection surface). Both consumers read the same attribute instance materialised on the same class. **There is no second copy of the declaration in any form.**

### 2.2 Why the rule is load-bearing

The alternative — analyzer logic that *re-states* the runtime guard's rules in a parallel form — is the project's textbook triple-binding pathology: spec, source, tests asserting the same invariant in three independent forms with synchronisation by hand. `TRANSLATION_PLAN.md` §2.1 names this pattern explicitly; the Russian-language guard message risk it documents is the same shape as the analyzer-versus-runtime-guard drift this rule prevents.

The anchoring rule reduces three potential consumers (spec, runtime guard, analyzer) to one declaration and N readers. Adding the analyzer to the system raises N from two (spec + runtime guard) to three (spec + runtime guard + analyzer); no new declaration is introduced; the synchronisation surface does not grow.

### 2.3 Implication for design

The analyzer must not re-derive what an attribute "means" — it must read the attribute and use it. Where a runtime guard exists for the same invariant (which is true for both v0.1 instances), the analyzer's logic is the **compile-time projection** of the runtime guard's logic onto the call site. The two share semantics by structural identity, not by parallel implementation.

Concretely for `SystemAccessCompletenessAnalyzer`:

- Runtime: `SystemExecutionContext.IsReadAllowed(t)` returns `_allowedReads.Contains(t) || _allowedWrites.Contains(t)` (write implicitly grants read).
- Compile-time: analyzer must implement the same predicate against the symbol-level attribute. The "write implicitly grants read" rule is a property of the invariant, not of the runtime; both consumers honour it.

If the runtime predicate changes (for example, if a future capability decision adds "read implicitly grants existence-check"), the change happens **in the predicate, in one place, consumed by both surfaces** — not in two places, in two forms, with a drift-detection ritual.

### 2.4 Consequence for the runtime guard

Once the analyzer ships, the DEBUG runtime guard remains in place — it is not redundant. The runtime guard catches violations that the analyzer cannot prove statically (reflection, dynamically-emitted code, code paths the analyzer's CFG cannot reach). The analyzer catches violations the runtime guard would catch only at execution time and possibly only under specific tick-state conditions. Both surfaces are necessary; their relationship is **complementary coverage**, not "the new one supersedes the old one."

The runtime guard's RELEASE-mode elision (`#if DEBUG` in `SystemExecutionContext.GetComponent<T>` / `SetComponent<T>`) is unaffected by this specification. The analyzer runs at compile time regardless of build configuration.

---

## 3. Operational rules

### 3.1 Severity

Every diagnostic emitted by every analyzer in this family ships with severity `Error`. CI fails the build on any emission. There is no warning tier, no info tier, no transition period.

The rationale is structural rather than ergonomic. A warning for an attribute-as-declaration violation would mean: "this code violates an architectural contract, but you may merge it anyway." That formulation contradicts the *contract* part of the term — a contract that may be violated at the violator's discretion is not a contract. The runtime guard already crashes the system on the same violation in DEBUG; the compile-time projection inherits the same hardness.

This decision is `TS-D-4` in §7.

### 3.2 Suppression

`#pragma warning disable`, `[SuppressMessage]`, `<NoWarn>` entries in csproj, and `dotnet_diagnostic.<id>.severity = none` in `.editorconfig` for any analyzer in this family are forbidden. Pull requests introducing such suppressions are rejected at review.

The legitimate response to an analyzer diagnostic is one of:

- **Fix the code** to remove the violation.
- **Fix the declaration** to admit the operation legitimately (which is itself an architectural decision and may require an audit pass).
- **Falsify the analyzer's invariant** through this specification's ratification process (§5.2 of `MAXIMUM_ENGINEERING_REFACTOR.md`) — the analyzer is wrong about what the architecture allows, the architecture's specification is updated, the analyzer follows.

The third response is rare but real. It is the only legitimate path to "the analyzer is wrong here." Per-occurrence suppression is not.

This decision is `TS-D-5` in §7. Per `TS-D-5`, the analyzer family explicitly does not participate in the .NET suppression mechanisms; an attempt to suppress a diagnostic from this family is itself a form of architectural violation that future analyzer versions may detect.

### 3.3 Drift policy

When a runtime guard's predicate changes in a non-semantic way (for example, the "write implicitly grants read" wording in `SystemExecutionContext.IsReadAllowed` is reformulated for clarity), the analyzer changes synchronously in the same commit. Both surfaces are re-tested; the behavioural test corpus is the regression guard.

When a runtime guard's predicate changes semantically (a new implication is admitted, an existing one is removed), the change is a §5.2-grade ratification event. This document is updated; the analyzer follows; the runtime guard follows; tests are added asserting the new shape from both surfaces. None of these updates happens in isolation.

The drift policy makes the cost of changing the predicate visible up front. It does not prevent legitimate evolution; it ensures the evolution moves both surfaces in lockstep, with the spec leading.

### 3.4 Over-declaration

An over-declaration is a declaration that admits an operation the site-of-use does not actually perform — for example, `[SystemAccess(reads: [A, B, C])]` on a system whose body reads only `A` and `B`. The analyzer's interpretation, locked by `TS-D-3`:

**Strict.** Over-declaration is a violation. The declaration must equal the actual operation set, not merely contain it.

The rationale is architectural rather than aesthetic:

- The declaration becomes a *bidirectional* contract. Today the runtime guard fixes only the under-declaration direction ("system performs an undeclared operation → crash"); over-declaration is a silent gap. Adding the over-declaration check at compile time closes the gap and elevates the contract to a complete specification of the system's interface.
- After K-series migration the corpus will contain over-declarations that are archaeological — declarations that were precise at the time of authorship but became loose as the implementation evolved through K0–K8. Strict semantics surface every such instance; permissive semantics hide them indefinitely. The repair pass (§6.3) is the structural mechanism for converting accumulated archaeology back into precise declarations.
- Forward-declaration ("I will read C in the next milestone, declaring it now") is a class of code-comment-as-spec that the project does not admit elsewhere. Comments documenting future intent live in `IDEAS_RESERVOIR.md` or in M-phase briefs, never in production attributes that runtime systems consume.

The two alternatives considered and rejected:

- *Permissive*: only under-declaration is a violation. Rejected as a one-sided contract that admits archaeology indefinitely.
- *Strict with explicit opt-out flag*: an `AllowOverDeclaration = true` argument lets a system mark its over-declaration as deliberate forward-declaration. Rejected because the opt-out becomes a soft-suppression mechanism over time; every opt-out is a suppression-shaped object whose accumulation degrades the contract.

This decision is `TS-D-3` in §7.

### 3.5 First-run acknowledgement

The first execution of any analyzer in this family against the post-K-series corpus will produce a non-trivial finding count. This is the specification's *intended* behaviour — the analyzer's job is to surface accumulated archaeology. The repair pass discipline (§6.3) handles this finding count in a structured way; the existence of the count is not a defect of the analyzer or of the specification.

The specification ratifies the analyzer's behaviour and the repair-pass discipline simultaneously, **before** the count is observed. This is deliberate: ratifying after observation would shift the psychological anchor toward leniency proportional to the count. Ratifying before observation fixes the contract on its merits.

---

## 4. The composite pilot

### 4.1 Goal

Two analyzers, one infrastructure, one specification, one M-phase: `SystemAccessCompletenessAnalyzer` and `ModCapabilitiesHonestyAnalyzer` ship together, in the same `DualFrontier.Analyzers/` project, governed by the rules in §3, anchored on the runtime guards already in production.

### 4.2 Selection rationale

`MAXIMUM_ENGINEERING_REFACTOR.md` v1.0 §3.3 recommends `LayerDependencyAnalyzer` (B1) as the pilot, on a *minimum-difficulty-establishes-toolchain* rationale. This document supersedes that recommendation for the following reasons:

1. **Structural isomorphism prefers consolidation.** §1.2 establishes that `[SystemAccess]` and `[ModCapabilities]` are instances of the same invariant family. Building the pilot around the family — rather than around an unrelated invariant (assembly imports) — makes the pilot exemplify the family. Subsequent analyzers in the same family then share the entire infrastructure, the specification, the discipline. A pilot built around B1 would establish infrastructure for one analyzer and one invariant class; the next analyzer in the family would have to re-establish conventions ad hoc.

2. **Anchoring rule (§2) requires runtime guards to anchor on.** Both v0.1 instances have a runtime guard already in production (`SystemExecutionContext`, `RestrictedModApi.EnforceCapability`); the analyzer is the compile-time projection of an existing semantic. B1 would have no analogous anchor — the layer dependency rule lives in prose in `ARCHITECTURE.md` §"Dependency rules" with no runtime guard to project from. Establishing the anchoring rule on a pilot that has no anchor would weaken the rule's structural force; establishing it on pilots that exemplify it strengthens the rule.

3. **Closes a deferred milestone.** `MOD_OS_ARCHITECTURE.md` v1.2 §11.1 records M3.4 (`[ModCapabilities]` analyzer) as deferred until "the first external mod author appears." This trigger is loose — the analyzer's value to internal mod development (vanilla mods M9, M10) is already real, the trigger merely deferred the implementation. Including `ModCapabilitiesHonestyAnalyzer` in the composite pilot resolves M3.4 on a structural trigger (post-K-migration closure, §6.2) instead of an indeterminate audience trigger. The deferred milestone is closed.

4. **Cost amortisation.** The Roslyn-analyzer csproj, the build-pipeline integration, the test harness (`Microsoft.CodeAnalysis.CSharp.Analyzers.Testing`), the diagnostic-id allocation scheme, the documentation conventions — all of these are one-time costs. Paying them once for two analyzers and reusing for N more is structurally cheaper than paying them once for one analyzer and re-establishing portions for each subsequent one. The §4.4 deliverables list exists in this form regardless of pilot scope; doubling the pilot scope doubles only the per-analyzer work, not the infrastructure work.

### 4.3 M3.4 closure

`MOD_OS_ARCHITECTURE.md` v1.2 §11.1 entry M3.4 is superseded by §4.1 of this document. The next non-semantic correction of `MOD_OS_ARCHITECTURE.md` (v1.7 or later) re-records M3.4 as "subsumed into `ARCHITECTURE_TYPE_SYSTEM.md` §4.1, see Track B activation". No work item is lost; the work moves out of the M-phase migration sequence and into the parallel-track Maximum Engineering Refactor sequence, which is the structurally appropriate location.

### 4.4 Deliverables (deferred to activation M-phase)

The full deliverables list is the responsibility of the activation M-phase brief (see `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md`). The pilot's structural shape is:

```
src/DualFrontier.Analyzers/
  DualFrontier.Analyzers.csproj
  AnalyzerReleases.Shipped.md        (Roslyn convention)
  AnalyzerReleases.Unshipped.md
  Diagnostics/
    DiagnosticIds.cs                  (DF_TS_001..., DF_TS_002...)
    DiagnosticDescriptors.cs          (severity = Error throughout)
  SystemAccess/
    SystemAccessCompletenessAnalyzer.cs
    SystemAccessSymbolReader.cs       (ISymbol.GetAttributes() → declared sets)
    SystemAccessCallSiteWalker.cs     (CFG walk over SystemBase subclasses)
  ModCapabilities/
    ModCapabilitiesHonestyAnalyzer.cs
    CapabilityTokenParser.cs          (tokens → typed declarations)
    CapabilityCallSiteWalker.cs       (Publish/Subscribe + [ModAccessible] reads)
  README.md
tests/DualFrontier.Analyzers.Tests/
  SystemAccessCompletenessAnalyzerTests.cs
  ModCapabilitiesHonestyAnalyzerTests.cs
  Fixtures/                           (positive + negative case fixtures)
```

The structural shape is illustrative — the activation M-phase brief is the authoritative source for file names, namespace choices, and exact diagnostic-id allocation.

### 4.5 Acceptance criteria (analyzer-level, deferred to activation M-phase)

For each analyzer in the composite pilot:

- [ ] Diagnostic id allocated under the `DF_TS_` prefix family. The prefix is locked here; specific numbers are allocated at implementation time and recorded in `AnalyzerReleases.Shipped.md`.
- [ ] Severity = `Error` (per §3.1, `TS-D-4`).
- [ ] No suppression mechanism, including the `[SuppressMessage]` path, accepted in the analyzer's tests as a valid bypass (per §3.2, `TS-D-5`).
- [ ] Anchoring rule (§2) honoured — analyzer reads the same attribute symbol the runtime guard reads through reflection. The shared predicate is documented at the symbol-reader's class header.
- [ ] Test corpus includes:
  - Positive cases — correctly-declared systems pass cleanly.
  - Negative cases — under-declaration produces the expected diagnostic.
  - Negative cases — over-declaration produces the expected diagnostic (per §3.4, `TS-D-3`).
  - Boundary cases — write implicitly granting read (per §2.3) tested explicitly on both surfaces.
  - Reflection-only access cases — analyzer documents its known unreachability for these (the runtime guard remains the safety net).
- [ ] Repair-pass closure (§6.3) on the corpus the analyzer first runs against. Repair pass closure is a precondition for analyzer enablement in CI; analyzer runs in dry-mode on PRs until the repair pass closes, then flips to error-mode.

---

## 5. Severity, suppression, drift — locked rules summary

This section is a summary of §3 in the structural form that future analyzer revisions of this document will mirror. New analyzers extend the table; they do not relax the rules.

| Rule | Position | Locked by | Restated |
|---|---|---|---|
| Severity = `Error` from day one, no warning transition | §3.1 | `TS-D-4` | No warning-mode transition lowers the cost of mis-design; Error from day one fixes the cost up front. |
| Suppression = forbidden | §3.2 | `TS-D-5` | The legitimate response is fix code, fix declaration, or ratify a spec change. Per-occurrence suppression is none of these. |
| Drift = synchronous lockstep with runtime guard | §3.3 | inherited from §2.2 | Spec leads, both surfaces follow in the same commit (non-semantic) or the same ratification cycle (semantic). |
| Over-declaration = strict equality with actual usage | §3.4 | `TS-D-3` | Declaration is bidirectional contract; archaeology surfaces; forward-declaration not admitted. |

---

## 6. Activation and sequencing

### 6.1 Sequencing principle

The composite pilot is **post-K-series**. The analyzer ships against a stable corpus, not a moving one.

### 6.2 The K-series gate

K0–K7 are closed (see `KERNEL_ARCHITECTURE.md` Part 2 status, `PERFORMANCE_REPORT_K7.md` closure note dated 2026-05-09). K8 is the cutover decision step; K9 (field storage abstraction) follows; K-series closure is the K9 closure point. Activation of this document's first M-phase is bound to K9 closure, not earlier.

The rationale is detailed in §6.3 of `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md`. Summary: pilot-against-moving-target produces analyzer code that requires rewrite as the target stabilises and produces ambiguous false-positive diagnostics during the migration. Pilot-against-stable-target produces a clean specification of a clean target.

### 6.3 Repair pass

The first run of each analyzer in the composite pilot, on the post-K9 corpus, produces a finding list. Each finding is one of:

- **Real archaeological violation.** Fix the code or fix the declaration (§3.2). Each fix is a normal scoped commit (`refactor(systems): tighten NeedsSystem [SystemAccess] declaration to match actual reads` and similar). Repair pass is not a single squash commit; it is a sequence of small commits, each independently reviewable.
- **Spec defect.** The analyzer is right by specification but the specification turns out to be wrong about what the architecture admits. Ratify a `v0.x` non-semantic correction or a `v1.x` semantic correction of this document. The analyzer follows.
- **Analyzer defect.** The analyzer's logic is wrong about what the specification says. Fix the analyzer; add a regression test; the diagnostic disappears from the finding list without code change.

Repair pass closure is a closure-review event with the same discipline as M-phase closure (`METHODOLOGY.md` §2.4). At closure, the finding count is zero, the analyzer is enabled in CI with severity = `Error`, no suppression entries exist anywhere in the codebase. The atomicity rule of M-phase review applies — once the repair pass is closed, the corpus is canonical at that snapshot.

### 6.4 Subsequent analyzers

After repair-pass closure of the composite pilot, additional analyzers in the *attribute-as-declaration* family extend §1.2 and ship as additional `DiagnosticAnalyzer` classes inside `DualFrontier.Analyzers/`. Each such addition is:

- A new entry in §1.2 (the family table) via a non-semantic v0.x correction of this document.
- A new `DF_TS_<id>` diagnostic.
- A new tests fixture.
- A new repair pass against the corpus, scoped to that analyzer's new finding set.

Analyzers in *other* invariant families (layer dependency, bus channel correctness, formal-verification-projected predicates) do not extend §1.2 — they receive their own specification documents (`ARCHITECTURE_LAYER_RULES.md`, etc.) anchored on this document's operational rules (§3, §5) but governing different invariant content.

The structural shape of this document is the template for those siblings: §1 family definition, §2 anchoring, §3 operational rules, §4 selected analyzers, §6 activation, §7 decision log.

---

## 7. Decision log

| ID | Decision | Rationale |
|---|---|---|
| `TS-D-1` | Roslyn analyzers are the toolchain | Native .NET ecosystem integration; mature; same csproj / CI pipeline as the rest of the project; `Microsoft.CodeAnalysis.CSharp.Analyzers.Testing` test harness is production-grade. Alternative (external Idris-style type checker per `MAXIMUM_ENGINEERING_REFACTOR.md` §3.3 Approach B-β) rejected — higher expressiveness not required for the v0.1 invariant family, integration cost not justified. |
| `TS-D-2` | Composite pilot, not B1 first | §4.2: structural isomorphism of the family (§1.2) prefers a pilot that exemplifies the family; anchoring rule (§2) requires runtime guards which B1 lacks; M3.4 deferred milestone closes structurally; infrastructure cost amortises over 2 analyzers from day one. Supersedes `MAXIMUM_ENGINEERING_REFACTOR.md` v1.0 §3.3 recommendation. |
| `TS-D-3` | Over-declaration = strict | §3.4: declaration becomes bidirectional contract; post-K archaeological accumulation surfaces explicitly; forward-declaration is comment-as-spec which the project rejects elsewhere. Permissive and strict-with-opt-out alternatives explicitly rejected with stated reasoning. |
| `TS-D-4` | Severity = `Error` from day one | §3.1: warning-mode admits architectural-contract violations as merge-eligible, which contradicts the term *contract*; runtime guard already crashes on the same violation in DEBUG; compile-time projection inherits the same hardness. No transition period. |
| `TS-D-5` | Suppression = forbidden | §3.2: any suppression mechanism converts diagnostics from contract violations into local stylistic preferences. The legitimate responses (fix code, fix declaration, ratify spec change) cover every legitimate case; per-occurrence suppression is structurally none of these. |
| `TS-D-6` | Activation = post-K9 closure | §6.2: pilot-against-stable-target produces clean specifications of clean targets; pilot-against-moving-target produces ambiguous false-positives and rewrites. K-series closure is the structural trigger. |
| `TS-D-7` | Repair pass = small atomic commits, not squash | §6.3: each archaeological finding is a separate architectural decision; bundling them into one squash commit destroys the per-decision audit trail; the small-commit discipline is the same as `housekeeping` passes already documented in `METHODOLOGY.md`. |
| `TS-D-8` | Diagnostic id family = `DF_TS_<n>` | The `DF_` prefix matches no other ecosystem analyzer namespace; `TS` for Type System / This Specification; `<n>` is monotonically allocated in `AnalyzerReleases.Shipped.md` per Roslyn convention. Future analyzer families use sibling prefixes (`DF_LR_` for layer rules, etc.). |
| `TS-D-9` | This document is the family authority, not per-analyzer documents | §6.4: subsequent analyzers extend §1.2 (the family table) here rather than spawning new documents. Cross-family analyzers spawn dedicated documents. The boundary between extending and spawning is the §1.1 definition. |

---

## 8. Open questions deferred to per-M-phase brief

The following questions are deliberately not answered in this document. They are addressed in the activation M-phase brief at implementation time:

- Specific Roslyn API surface choices (`SyntaxNodeAction` vs `SymbolAction` vs `OperationAction` for the call-site walker).
- Exact `DF_TS_<n>` allocation for the two pilot diagnostics.
- Treatment of `dynamic`-typed receivers calling `GetComponent<T>`/`SetComponent<T>` (the analyzer's known unreachability per §4.5 is documented at implementation time).
- Treatment of capability tokens whose component type is fully qualified versus short-named (parser ambiguity).
- Cross-mod token resolution (the §3.4 capability registry surface that `ModCapabilitiesHonestyAnalyzer` consults at compile time, which may require build-time mod-graph awareness — non-trivial; specifies in the M-phase brief, possibly as a Phase-2 enhancement).
- Performance budget per analyzer per compilation (Roslyn analyzer perf budgets are non-trivial; the M-phase brief specifies a measured target and the test harness for it).
- Behaviour during `dotnet build` of an individual mod project versus full-solution build (the cross-mod resolution from the previous bullet has different shapes in each).

These are not gaps in v0.1. They are appropriately deferred to activation-time, when concrete context constrains the answers.

---

## 9. Falsifiability conditions for this document

This specification is itself falsifiable, in the same shape as `MAXIMUM_ENGINEERING_REFACTOR.md` §8:

- **If the composite pilot's M-phase produces a finding count that the project cannot resolve within `2 ×` the M-phase's planned budget**: the corpus archaeology is deeper than expected. Document the fact, ratify a `v0.x` correction admitting partial repair-pass closure with a named residual finding set, schedule a follow-up pass. The specification stands; the activation reality requires a longer trajectory than projected.
- **If `TS-D-3` (Strict over-declaration) produces a false-positive rate that audit cannot reduce to zero through normal repair**: the strict semantics admit a class of legitimate code the specification did not anticipate. Ratify a `v1.x` correction either narrowing the strict rule (specifying the legitimate class explicitly) or weakening it to permissive with documented rationale. The decision log records the change with full reasoning; `TS-D-3` is updated.
- **If the anchoring rule (§2) cannot be honoured in practice for the second pilot analyzer (`ModCapabilitiesHonestyAnalyzer`)** — for example, if cross-mod token resolution at compile time turns out to be structurally impossible without a build-graph that the project does not have — narrow the analyzer's scope to within-mod tokens only and document the residual coverage gap. The runtime guard catches the rest. The specification stands; the analyzer's coverage shrinks.
- **If subsequent analyzer additions surface a fourth or fifth invariant in the family that breaks §1.1's three-part structure**: ratify a `v1.x` correction extending the structural definition. The family stays a family; its definition broadens.

The specification is not a marketing document. It is a hypothesis about how a verification family scales across the *attribute-as-declaration* invariant class in this corpus.

---

## 10. Notes for the future architect

When `ARCHITECTURE_TYPE_SYSTEM.md` v0.x or v1.x is revised, the following will likely be true:

- The K-series will have closed and the composite pilot will have shipped, so §6.2 / §6.3 will be historical record rather than forward planning. Update §6 to reflect closure; preserve §7 decision log.
- Additional analyzers will have been added under §1.2 / §6.4. The §1.2 table grows; the §1.1 definition does not change.
- The runtime guard semantics may have evolved (new capability decisions in `MOD_OS_ARCHITECTURE.md`; new system-access shapes in K-series successor work); the anchoring rule (§2) ensures these evolutions are reflected here through synchronous updates, not through drift.
- `MAXIMUM_ENGINEERING_REFACTOR.md` v1.x or v2.x may have ratified Track A (formal verification) or Track C (replication kit). Their interaction with this document, if any, is documented in the appropriate cross-reference; this document does not subsume them and is not subsumed by them.
- The discipline this document encodes is **deliberate**, not corrective. The Strict over-declaration, the no-suppression rule, the post-K activation gate — these are choices, taken with full awareness of the alternatives. If the future architect finds the discipline excessive for the project's then-current reality, the legitimate response is the §5.2 ratification process of `MAXIMUM_ENGINEERING_REFACTOR.md`, not silent suppression patterns or accumulated work-arounds.

If the indulgence stops being joyful, the document should be archived without ceremony — same disposition as the parent `MAXIMUM_ENGINEERING_REFACTOR.md` §9 specifies.

---

## 11. Ratification

Ratified at v0.1 on 2026-05-09 under the same discipline as
[MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) and
[MAXIMUM_ENGINEERING_REFACTOR](/docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md).

The document joins the LOCKED corpus on equal standing with other research-tier
specifications. The activation M-phase brief is authored separately as
[MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION](./MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md);
this document and that brief are companion artefacts, ratified on the same date.

### Pending integrations

- `ROADMAP.md` Backlog "Maximum engineering refactor (parallel track)" entry: Track B bullet to be reworded at activation time, replacing "B1 layer dependency analyzer recommended first" with the composite-pilot pointer to this document. Non-semantic edit.
- `MOD_OS_ARCHITECTURE.md` v1.7 (next non-semantic correction): §11.1 entry M3.4 to be reworded as "subsumed into `ARCHITECTURE_TYPE_SYSTEM.md` §4.1, see Track B activation." Non-semantic edit.

These integrations land at the natural next non-semantic correction of each document; they do not require a coordinated commit.

### Version history

- **v0.1** (2026-05-09) — initial ratification. Composite pilot specified; nine `TS-D-*` decisions locked; activation deferred to post-K9.

---

## See also

- [MAXIMUM_ENGINEERING_REFACTOR](/docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md) — parent v1.0 specification; this document realises Track B per §3, with composite-pilot supersession of §3.3 documented at `TS-D-2`.
- [MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION](./MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md) — activation brief; specifies the M-phase that builds the composite pilot.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — `[ModCapabilities]` runtime guard; v1.2 §11.1 M3.4 superseded per `TS-D-2` and §4.3.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — K-series migration that determines activation timing (`TS-D-6`).
- [PERFORMANCE_REPORT_K7](/docs/reports/PERFORMANCE_REPORT_K7.md) — K7 closure date establishing the K-series progression at v0.1 ratification time.
- [ARCHITECTURE](./ARCHITECTURE.md) — four layers; the layer dependency rules referenced in §1.3 / §6.4 live here, awaiting their own analyzer specification.
- [ISOLATION](./ISOLATION.md) — `SystemExecutionContext` runtime guard semantics anchored by §2.
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — ratification process inherited; §2.4 atomicity rule applied to repair-pass closure (§6.3); housekeeping commit discipline applied to repair-pass commits (`TS-D-7`).
- [TRANSLATION_PLAN](./TRANSLATION_PLAN.md) — §2.1 triple-binding anti-pattern; the anchoring rule (§2) is the structural defence against it for this analyzer family.
- [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md) — operational reference for diagnostic-message style, naming conventions in analyzer code (consumed by activation M-phase brief, not by this specification directly).
