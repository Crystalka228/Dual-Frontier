---
register_id: DOC-A-ANALYZER_RULES_V2
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.1
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: 2027-Q3
title: Analyzer rules (authored rework; single family authority as shipped — retires the DF_TS program, resolves schism N-13)
supersedes:
- DOC-A-ANALYZER_RULES
- DOC-A-ARCHITECTURE_TYPE_SYSTEM
- DOC-A-MAX_ENG_REFACTOR_TRACK_B
last_modified_commit: edb267a
review_cadence: on-change+annual
last_review_date: 2026-07-17
last_review_event: 'DRAFTS_RATIFICATION MC-1 (C5): candidate-banner class retired - banner to ratified-successor note (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION carried), checklist line removed, Role to normative (ratified successor) where the candidate token was present, pending-amendment sentence to LOCKED form (ARCHITECTURE, CONTRACTS). Changelog status cells left as authored-session history per HALT-1 OD-2. PATCH 1.0.0 to 1.0.1.'
reviewer: Crystalka
special_case_rationale: Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist item [1]). Successor of DOC-A-ANALYZER_RULES and retirement carrier for the DF_TS program (N-13 resolved) per EVT-2026-07-15-CORPUS_REWORK_R2_PLATFORM; family authority as shipped (17 rules; 12 deferred + 1 scope-exclusion; [SystemAccess]-completeness = unassigned-ID DEFERRED candidate, К-L20 scope).
---

# DualFrontier Roslyn Analyzer Rule Specifications

Specifies the shipped Roslyn analyzer rule surface — IDs, namespaces, severities, and suppression law as released — and formally retires the never-built DF_TS analyzer program.

> **Ratified successor (LOCKED v1.0.0 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION, 2026-07-17).** Successor of three predecessors, all now SUPERSEDED: `docs/architecture/historical/ANALYZER_RULES.md` (DOC-A-ANALYZER_RULES — the shipped-rule-surface authority, AUTHORED-SKELETON, "states what exists"); `docs/architecture/historical/ARCHITECTURE_TYPE_SYSTEM.md` (DOC-A-ARCHITECTURE_TYPE_SYSTEM — Draft, the never-built DF_TS family specification); `docs/architecture/historical/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` (DOC-A-MAX_ENG_REFACTOR_TRACK_B — Draft, that family's activation brief). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md) §3 C5, §6.1 N-13/N-14, §6.4 N-26); content verified against code at HEAD `35364c2`. The two Draft predecessors were never on the FRAMEWORK §14.7 authority surface; this document's §5 is their formal retirement record, not a promotion of their content.

## Status

| Field | Value |
|---|---|
| Role | normative (ratified successor) |
| Successor of | `docs/architecture/historical/ANALYZER_RULES.md` (DOC-A-ANALYZER_RULES) · `docs/architecture/historical/ARCHITECTURE_TYPE_SYSTEM.md` (DOC-A-ARCHITECTURE_TYPE_SYSTEM) · `docs/architecture/historical/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` (DOC-A-MAX_ENG_REFACTOR_TRACK_B) — all SUPERSEDED |
| Scope | The shipped DualFrontier Roslyn analyzer rule surface: rule IDs, namespaces, categories, the per-rule template, severities and suppression law as released at Release 1.0, the deferred-rule registry with named activation gates, and the formal retirement record of the DF_TS analyzer program that never built. |
| Non-goals | Does not restate К-Lxx invariant canonical text (KERNEL_ARCHITECTURE.md Part 0 and K_CLOSURE_REPORT.md §7 own that). Does not restate DFK-WAIVER pragma syntax as law (CODING_STANDARDS.md §5.3 owns it; this document cites, never restates). Does not schedule new analyzer work (ROADMAP.md «Analyzer track» owns sequencing). Does not adjudicate MOD_OS_ARCHITECTURE.md's own §3.7 (D-2, historically "M3.4") capability-analyzer milestone — that milestone is unrelated to, and unclosed by, this document or either retired predecessor. |
| Authority domains | The analyzer rule surface (IDs, namespaces, categories, per-rule template) and the severity/suppression law, both as shipped. |
| Defers to | [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) Part 0 — К-Lxx invariant canonical text · [CODING_STANDARDS.md](../methodology/CODING_STANDARDS.md) §5.3 — the DFK-WAIVER suppression law (cited, not restated) · [ROADMAP.md](../ROADMAP.md) «Analyzer track» — phase history and deferred-rule sequencing · [K_CLOSURE_REPORT.md](./K_CLOSURE_REPORT.md) §7 — historical detection-narrative provenance (pre-rename DF### numbering, superseded by §1 below, retained for lineage) |

---

## §1 — Shipped surface census (verified on disk)

Ground truth at HEAD `35364c2`, re-verified rule-by-rule this rework by opening every file under `tools/DualFrontier.Analyzers/Rules/`: **17 rules carry real detection logic and are enforced at their Release 1.0 severities** — 11 Error + 5 Warning + `DFL025_B` at descriptor Info (`.editorconfig` `suggestion`, IDE-only). 16 of the 17 are build-breaking under `TreatWarningsAsErrors`. On-disk layout (matches the predecessor's census exactly, re-confirmed by directory listing):

```
tools/DualFrontier.Analyzers/Rules/
├── Architecture/    (9) DFK003 · DFK003_1 · DFK004 · DFK005 · DFK007 · DFK011 ·
│                        DFK013 · DFK016 · DFK017
├── NativeBoundary/  (5) DFK001 · DFK002 · DFK007_1 · DFK015_1 · DFK019_A
└── Discipline/      (3) DF999 · DFL025_A · DFL025_B
```

This document, not scattered mentions elsewhere, is the current-truth surface for analyzer status: `historical/ISOLATION.md:15` still reads "detection logic pending Phase β — see §Enforcement model item 3" (and item 3 itself, dated 2026-06-02, repeats the same claim) — it predates Phase β/γ (2026-07-01) and now understates a reality the *non-authoritative* register standing of the old ANALYZER_RULES.md happened to get right first (session finding N-26); the THREADING successor of this same rework has already corrected its analyzer references (its §8), while `historical/THREADING.md` retains the pre-Phase-β snapshot. Nothing below should be read through those stale lenses.

### §1.1 — Registry (17 rules)

| Rule ID | К-L / Lesson anchor | Intent — verified detection pattern | Severity (Release 1.0) | Status |
|---|---|---|---|---|
| DFK001 | К-L1 | Native language discipline, managed side. Flags `Marshal.GetDelegateForFunctionPointer` and `NativeLibrary.Load`/`TryLoad`/`GetExport`/`TryGetExport` calls outside the sanctioned interop surface (§6) — the dynamic-loading bypass complementing DFK002's static one. | Error | enforcing |
| DFK002 | К-L2 | P/Invoke bindings. Flags any `[DllImport]`/`[LibraryImport]`-carrying method declared outside the sanctioned interop surface (§6). | Error | enforcing |
| DFK003 | К-L3 | Storage ownership. A class implementing `IComponent` (Path β, managed store) without `[ManagedStorage]` is flagged; a struct `IComponent` is Path α by construction (`[ManagedStorage]` is class-only, CS0592-prevented — the violation is compile-unreachable on that side). | Error | enforcing |
| DFK003_1 | К-L3.1 | Storage bridge. Flags `new ManagedStore<T>(...)` anywhere outside the `DualFrontier.Application.Modding` provider namespace — `SystemBase.ManagedStore<T>()` is the sanctioned path. | Error | enforcing |
| DFK004 | К-L4 | Type ID registry. Flags a hash taken over a `typeof(...)` expression (`typeof(X).GetHashCode()` / `.FullName.GetHashCode()` / `.Name.GetHashCode()`); plain `typeof(X)` (137 legitimate sites at Phase β) stays silent by construction. | Error | enforcing |
| DFK005 | К-L5 | Declarative bootstrap. Flags an additional managed class named `*Bootstrap` other than `GameBootstrap` itself or a type under the `DualFrontier.Core.Interop` boundary. | Error | enforcing |
| DFK007 | К-L7 | Span protocol. Flags a class field or property typed `SpanLease<T>` — spans are transient per-tick leases, not storage state; struct enumerators holding a lease for iteration lifetime are exempt (their lifetime is bounded by the lease). | Error | enforcing |
| DFK011 | К-L11 | NativeWorld SSoT. Flags `new ManagedWorld(...)` in any namespace not containing `Test` — `ManagedWorld` is the retired managed backbone, test-fixture-only since the A'.5 K8.3+K8.4 cutover. | Error | enforcing |
| DFK013 | К-L13 | Wake-type discipline (efficiency, not correctness). Flags a concrete, non-abstract `SystemBase` subclass carrying none of `[TickRate]` / `[WakeOnEvent]` / `[WakeOnState]` / `[WakeOnInit]` / `[WakeOnExplicit]` / `[WakeOnSlotTransition]`. | Warning | enforcing |
| DFK016 | К-L16 (retain α — Phase 0 ratified per Q-L-16) | Pipeline depth. Flags an explicit integer literal `1`/`2`/`3` passed as `PipelineSlotInterop.Init(depth:)`'s `depth` argument; a named-constant reference (`.DefaultDepth`/`.MaxDepth`) or an omitted (defaulted) argument is not flagged. | Warning | enforcing |
| DFK017 | К-L17 | Display composition. Flags a class carrying `[Layer(...)]` that does not derive from the `Layer` base — the alternate-registration-surface bypass; the real hierarchy registers via the base and stays silent. | Error | enforcing |
| DFK007_1 | К-L7.1 | GPU pipeline slot. Flags a call to `PipelineSlotInterop.GetSlot` (the raw slot pointer) from outside `DualFrontier.Core.Interop` — `ReadSlotTail` is the sanctioned managed read. | Error | enforcing |
| DFK015_1 | К-L15.1 | Three-tier mutex managed facade. Flags `new Mutex(...)` / `new Semaphore(...)` — raw OS synchronization primitives bypassing the three-tier facade. | Error | enforcing |
| DFK019_A | К-L19 (static API surface half of the Q-L-8 split) | Static Vulkan API surface. Flags a `using` directive rooted at an alternate-graphics namespace (`OpenGL`/`OpenTK`/`DirectX`/`Direct3D`/`SharpDX`/`Vortice`/`Metal`); the 871-occurrence `Runtime.Native.Vulkan` interop surface is silent by construction. Hardware-tier runtime capability probing is DFK019_B — deferred (§4). | Warning | enforcing |
| DFL025_A | Lesson #25 (refined 3rd extension) | `[ReservedStub]` behavior-invocation discipline. Flags a `[Fact]`/`[Theory]` test method that touches a `[ReservedStub]`-tagged type without `[Trait("Category","ReservedStub")]` on the method or its class. | Warning | enforcing |
| DFL025_B | Lesson #25 (refined 3rd extension) | Standalone-test Skip discipline. Flags the same touch pattern on a `[Fact]`/`[Theory]` lacking a non-empty `Skip` argument — edge-case discipline, informational only. | Info (`.editorconfig` `suggestion`) | detecting, IDE-only |
| DF999 | self-policing (Q-L-18 default) | Solution-wide suppression ban. Flags `[assembly: SuppressMessage]` and any file named `GlobalSuppressions.cs`. | Warning | enforcing |

Composition: 9 Architecture + 5 NativeBoundary + 3 Discipline = 17, matching the on-disk tree above exactly. Naming: descriptor ID strings, file names, and class names all use the underscore form for sub-rules and variants — see §2.

### §1.2 — Wiring truth

`src/Directory.Build.props:30-35` adds the analyzer project as a `ProjectReference` with `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"` to all 12 managed `src/` projects (verified by directory listing: AI, Application, Components, Contracts, Core, Core.Interop, Crypto.Future, Events, Launcher, Persistence, Runtime, Systems):

```xml
<ItemGroup Label="DualFrontier.Analyzers (К-extensions cascade #5)">
  <ProjectReference
    Include="$(MSBuildThisFileDirectory)..\tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj"
    OutputItemType="Analyzer"
    ReferenceOutputAssembly="false" />
</ItemGroup>
```

This `src/`-scoped file explicitly imports the repo-root `Directory.Build.props` (`src/Directory.Build.props:28`) — MSBuild's directory-walk auto-discovery uses "first found wins," so the explicit `<Import>` is what lets this file inherit `net8.0` + `TreatWarningsAsErrors` + `Nullable` + `ImplicitUsings` rather than silently shadowing them. `tools/` and `tests/` projects sit outside `src/`'s scope by placement — the analyzer does not analyze its own source or its own tests. Three independent surfaces must agree on each rule's shipped severity, and were re-checked rule-by-rule this rework: the descriptor's `defaultSeverity` (source), the root `.editorconfig`'s `dotnet_diagnostic.<ID>.severity` key, and `AnalyzerReleases.Shipped.md`'s Release 1.0 row. All 17 agree; a divergence in any one would be a defect, not a valid override (§3).

### §1.3 — Tests

`tests/DualFrontier.Analyzers.Tests` carries 20 `.cs` files: 17 per-rule test classes (one per shipped rule, mirrored under `Rules/{Architecture,Discipline,NativeBoundary}/`), `CensusMetaTests.cs` (repo-discipline meta-tests, §1.4), `HarnessTests.cs` (verifier-scaffolding smoke tests), and the `CSharpAnalyzerVerifier<T>` harness itself (no test methods). A precise re-count at HEAD `35364c2`, counting only genuine attribute lines (a naive grep over-counts by matching `[Theory]` inside an XML-doc comment) finds **57 test methods** — 55 bare `[Fact]`, 1 `[Fact(Skip=...)]`, and 1 `[Theory]` carrying 4 `InlineData` rows (`CensusMetaTests.MarkerFamilyCensus_MatchesPin`) — which xUnit's test explorer expands to **60 executed test cases**. This is close to, but does not exactly reproduce, the predecessor's flat "54 tests" figure; carried forward here as order-of-magnitude verified rather than re-pinned to a number the source doesn't unambiguously yield on its own (the predecessor's count did not specify whether it meant source-level methods or Theory-expanded cases, and the two now differ by several).

### §1.4 — Waiver census

Exactly 2 `DFK-WAIVER`s exist in the tree, both `DFK001`, both in `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs` (lines 90/93/95 and 137/140/142), both citing the same К-L19-sanctioned rationale — VK_EXT_debug_utils Vulkan debug-messenger interop:

```csharp
// DFK-WAIVER(DFK001): sanctioned Vulkan debug-messenger interop
#pragma warning disable DFK001
...
#pragma warning restore DFK001
```

`CensusMetaTests.DfkWaiverCensus_MatchesPin` hard-pins this count (asserts exactly 2 `#pragma warning disable (DFK|DFL|DF9)` sites, each paired with a `// DFK-WAIVER(` marker) and `CensusMetaTests.SuppressMessageCensus_IsZero` independently confirms zero `[SuppressMessage]` occurrences anywhere in `src/` — the DF999 baseline holds structurally, enforced by a compiled test, not merely by convention. Provenance: the KERNEL_ARCHITECTURE.md chronicle records the Phase β triage's first run at 23 diagnostics across the newly-detecting 17 rules (Q-L-1 adaptive gate: ≤80 → continue in one cascade), resolved into these 2 sanctioned waivers plus a further relocation cluster — see §6.

---

## §2 — ID namespace law

Three-tier rule-ID namespace, forward-locked at Brief A'.9.1 batch 1 deliberation (2026-05-24, Q-L-3), unchanged through shipped Release 1.0:

- **`DFK###`** — К-Lxx invariant rules (architectural / native-boundary). Numbered 1:1 with the К-L# each encodes (`DFK001` ↔ К-L1, `DFK002` ↔ К-L2, …). Sub-invariants and split variants carry an **underscore** suffix: `DFK003_1` (К-L3.1), `DFK007_1` (К-L7.1), `DFK015_1` (К-L15.1), `DFK019_A` (К-L19's static-surface half of the Q-L-8 split).
- **`DFL###`** — Lesson-derived rules (discipline). Numbered by METHODOLOGY Lesson number: the `DFL025_A`/`DFL025_B` pair encodes Lesson #25's refined 3rd extension.
- **`DFC###`** — reserved. No `DFC###` stub exists at Release 1.0; reserved for cascade-specific drift-detection rules, scheduled for the К-L20 LOCK cascade (§4) per Q-L-11.
- **`DF999`** — the one rule outside the three-tier scheme: analyzer self-policing (bans solution-wide suppression), Q-L-18 default.

**Underscore, not dot or hyphen.** Descriptor ID strings, file names, and class names all use the underscore form for sub-rules and variants — this is a *corrected* convention, not the original one. A dotted or hyphenated diagnostic ID is **rejected by Roslyn `ReportDiagnostic` as an invalid identifier** — a Phase β empirical finding, adjudicated by Crystalka 2026-07-01. Dotted/hyphenated forms from before that date (`DFK003.1`, `DFL025-A`) that still surface in older evidence documents (K_CLOSURE_REPORT.md §7, the Phase-β briefs) are the superseded spelling of the *same* rules, not different rules — a pragma naming the old form would suppress nothing. `DFL025-C` (§4) is a shell-level `dotnet test --filter` category, never a Roslyn descriptor ID; the hyphen there is unaffected by the adjudication.

**Categories** — three in use, one reserved: `DualFrontier.Architecture` (9 rules) · `DualFrontier.NativeBoundary` (5 rules) · `DualFrontier.Discipline` (3 rules) · `DualFrontier.ModSurface` — reserved, no shipped rule carries it, scheduled for the К-L20 LOCK cascade alongside the `DFC###` namespace.

---

## §3 — Severity & suppression law, as shipped

**Three severity tiers exist**, not one. Of the 17 shipped rules: 11 ship `Error` (the correctness-class К-L invariants — storage ownership, span protocol, native-boundary discipline), 5 ship `Warning` (the efficiency-class invariants — К-L13 wake discipline, К-L16 pipeline-depth literals, К-L19's static-surface half — plus `DFL025_A` and the `DF999` self-policing rule), and 1 (`DFL025_B`) ships descriptor `Info`, restated by `.editorconfig` as `suggestion` — IDE-only, not build-breaking. `TreatWarningsAsErrors=true` at the repo-root `Directory.Build.props` (inherited by the `src/`-scoped file, §1.2) makes both `Error` and `Warning` diagnostics fail the build: **16 of the 17 rules are build-breaking; only `DFL025_B` is not.**

**Suppression is sanctioned, not banned.** [CODING_STANDARDS.md](../methodology/CODING_STANDARDS.md) §5.3, "`DFK-WAIVER` — the suppression law," is the standing law this document defers to and does not restate; the operative shape:

- **Form** — a `// DFK-WAIVER(<id>): <reason>` comment immediately precedes a narrowly-scoped `#pragma warning disable <id>` / `#pragma warning restore <id>` pair, never a file- or project-wide disable.
- **Three allowed reason classes** — false-positive pending a named ROADMAP refinement entry; a sanctioned architectural exception citing a locked decision ID (Q-L-#, К-L#); or generated/interop-mandated code the rule does not meaningfully apply to.
- **No blanket suppression** — file- or project-scope `<NoWarn>` for any `DFK`/`DFL` diagnostic is forbidden outright (the one root-props `NoWarn` entry, `CS1591`, is an unrelated compiler documentation warning).
- **Census-tracked** — every waiver is counted by the compiled `CensusMetaTests.DfkWaiverCensus_MatchesPin` meta-test (§1.4); the current pin is 2, both `DFK001`, both cited to the same К-L19 exception. No orphan waivers: a citation that doesn't resolve fails review.
- **`GlobalSuppressions.cs` is banned outright** — `DF999` (§2) detects and enforces the ban itself, at `Warning` (build-breaking) since Release 1.0.

**RS release-tracking ceremony.** `AnalyzerReleases.Shipped.md` carries a single "Release 1.0" block listing all 17 rules at their shipped severity; `AnalyzerReleases.Unshipped.md` is empty-with-header. Roslyn's release-tracking analyzer family cross-checks descriptor severity against the `Shipped.md` table mechanically at build time — any future severity change requires a "Changed Rules" ceremony entry in `Shipped.md`, landing in the same commit as the descriptor and `.editorconfig` edits. This is what makes severity drift structurally visible rather than a silent one-line diff.

**This supersedes the DF_TS family's laws wholesale, not partially.** The retired Draft family (§5) legislated the opposite of every rule above: `TS-D-4` fixed severity at `Error` "from day one, no warning-mode transition period" for every diagnostic in the family; `TS-D-5` forbade suppression outright, including through `[SuppressMessage]`, with no waiver mechanism and no release-tracking ceremony (the family never shipped a `Shipped.md` entry because it never shipped a rule). The shipped DFK/DFL/DF999 regime is not a partial or compromised implementation of those laws — it is a different, separately ratified design (Q-L-3 through Q-L-20, F-12) for which a Warning tier and a cited-waiver mechanism were deliberate choices, not omissions falling short of an unmet target.

---

## §4 — Deferred registry (13 declared, not implemented)

30 rule IDs are declared across the analyzer's history; 17 are shipped (§1). Of the remaining 13, 12 are honestly deferred, each to a named gate, and the 13th (`DFL025-C`) is a scope exclusion, not a deferral — no silent drops (the one true drop, `DFK010`, says so explicitly):

| Rule | К-L / scope | Disposition | Gate |
|---|---|---|---|
| DFK006 | К-L6, SUPERSEDED by К-L12 | Never activates — historical traceability only | none; permanent reserved entry |
| DFK008 | К-L8 process invariant (storage-path discipline) | Outside Roslyn scope — a code-pattern rule can't enforce a process invariant | Git pre-commit hook (designated, not built) |
| DFK010 | К-L10 decision rule | **PERMANENTLY DROPPED** — Q-L-9 + PA-002 axiom ("без костылей"): К-L10 governs decision attribution at the document/methodology layer, not the code layer; a code-layer marker masquerading as doc-layer reasoning is exactly the kostyl pattern PA-002 forbids | none; FRAMEWORK + METHODOLOGY documentation discipline only |
| DFK014 | К-L14 meta-invariant | Outside Roslyn scope — "architectural integrity" isn't a syntax-tree pattern | [K_L14_EVIDENCE_DASHBOARD.md](./K_L14_EVIDENCE_DASHBOARD.md) cumulative evidence tracking |
| DFK009 | К-L9 Vanilla=mods | Deferred | К-L20 LOCK cascade — the IModApi surface is volatile pre-LOCK |
| DFK012 | К-L12 native scheduler sovereignty | Deferred | К-L20 LOCK cascade — the managed-facade boundary (К-L9 "facade preserves Vanilla=mods") is undefined pre-LOCK |
| DFK015 | К-L15 bus capability declaration | Deferred | К-L20 LOCK cascade — capability-token vocabulary ties to the mod-API surface |
| DFK018 | К-L18 mod-unload quiescence | Deferred | К-L20 LOCK cascade — the unload sequence (К10.3 v2 §9.5) is still refining |
| DFK020 family | К-L20 (~20 candidate sub-rules) | Deferred | К-L20 LOCK cascade — the invariant this family enforces has no canonical text yet |
| DFC001.A / DFC001.B | Bridge `IRenderCommand` marker purity / record purity | Deferred | К-L20 LOCK cascade (Q-L-11) — Bridge surface is Mod-API-coupled |
| DFK019_B | К-L19 hardware-tier runtime capability check | Deferred | Hardware tier expansion cascade — audience-driven (Lesson #N17 Provisional); a single tier (T1, high-end Vulkan 1.3) is the current substrate baseline |
| DFL025-C | Lesson #25 shell-level filter (`dotnet test --filter "Category!=ReservedStub"`) | Not a Roslyn analyzer — scope exclusion, not a deferral | Closure-protocol tooling, out of this document's scope entirely |

The К-L20 LOCK cascade (7 of the 13 rows) is gated on the Mod API surface reaching LOCK — pre-emptive enforcement against a moving target is the exact kostyl pattern PA-002 forbids (Q-L-11). Both the `DFC###` namespace and the `DualFrontier.ModSurface` category (§2) activate at that same cascade. `MOD_API_CONTRACT.md` does not exist yet; every reference to it anywhere in the corpus is a forward reference until the К-L20 LOCK cascade authors it. Full rationale and activation triggers: [ROADMAP.md](../ROADMAP.md) «Analyzer track».

*See also §5 — a `[SystemAccess]`-completeness analyzer, with no rule ID assigned, is registered as a further К-L20-cascade-scoped DEFERRED candidate, distinct from the 13 rows above (it was never one of the 30 declared IDs; it is new to this document).*

---

## §5 — The DF_TS program — formally retired

Session finding N-13: two disjoint analyzer programs coexisted in the corpus, neither citing the other. The Draft family — `ARCHITECTURE_TYPE_SYSTEM.md` + `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` — legislated a composite pilot (`SystemAccessCompletenessAnalyzer` + `ModCapabilitiesHonestyAnalyzer`, diagnostic prefix `DF_TS_`) under `Error`-only severity with no warning tier (`TS-D-4`), an absolute suppression ban (`TS-D-5`), and CI dry-mode-until-repair-pass-closure gating — anchored throughout on `SystemExecutionContext`'s DEBUG runtime guard ("the DEBUG runtime guard remains in place — it is not redundant," ARCHITECTURE_TYPE_SYSTEM.md §2.4). That guard was deleted at the K8.3+K8.4 cutover, A'.5 closure, **2026-05-14** (`SystemExecutionContext.cs:24-31`; `SystemAccessAttribute.cs:11-13`; `ISOLATION.md:25` — "deleted in K8.3+K8.4 cutover (A'.5 closure 2026-05-14)"). Both Draft documents' v0.1.1 patch, dated a full month later (2026-06-12), added a reclassification banner but asserted "specification content unchanged" — the patch never noticed its own anchor was gone. No `DF_TS_*` diagnostic and neither analyzer class was ever built anywhere in the tree. The shipped DFK/DFL/DF999 regime (§§1–4) — a Warning tier, cited waivers, no CI at all — supersedes the family wholesale; this document is that supersession, not a reconciliation of the two.

One idea survives the retirement: a `[SystemAccess]`-completeness check — verifying that a `[SystemAccess(reads, writes, bus)]` declaration matches the `GetComponent`/`SetComponent`/`Query` call sites actually present in a system body. It is re-registered here as a **DEFERRED candidate**: no rule ID assigned (it was never one of the 30 IDs counted in §4), scoped to the К-L20 LOCK cascade alongside `[ModCapabilities]` honesty — both are the same shape (declaration vs. call-site) and share that cascade's gate. Re-registering it here replaces three now-dangling references to "the future A'.9 Roslyn analyzer" that this document's retirement makes stale on sight: `historical/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:233` ("the future A'.9 Roslyn analyzer to catch undeclared accesses"), `historical/CONTRACTS.md:69` ("the dependency graph + future A'.9 Roslyn analyzer verify that publication targets only that bus" — its successor CONTRACTS.md §6 already replaced the pointer with verified enforcement reality), and `SystemExecutionContext.cs:29` itself, in the shipped source's own doc comment — the one genuinely live in-code carrier. None of those three sites names a program that exists; this section is where that program's one salvageable idea now lives, named as Planned, not implied as already scheduled.

---

## §6 — DFK002 federated interop model

Per the Phase β §8 ratification (2026-07-01; recorded in the KERNEL_ARCHITECTURE.md chronicle), the sanctioned P/Invoke surface is federated across two namespace roots, defined once in `tools/DualFrontier.Analyzers/Rules/NativeBoundary/SanctionedInteropSurface.cs:30-34` and shared by DFK001 (dynamic-interop bypass, §1.1) and DFK002 (`[DllImport]`/`[LibraryImport]` placement): `DualFrontier.Core.Interop` (the C++20 kernel boundary, К-L1/К-L3) and `DualFrontier.Runtime.Native` (the native runtime boundary — `Runtime.Native.Vulkan` GPU substrate К-L19, `Runtime.Native.Win32` Launcher OS surface), including nested namespaces under either root. At the Phase β triage, the one genuine DFK002 violation on record — a 13-`DllImport` block in `ManagedBusBridge.cs` (`DualFrontier.Application.Bus`, outside both roots) — was relocated into `Core.Interop` (now `NativeMethods.Bus.cs`). A repo-wide re-check at HEAD `35364c2` (every `.cs` file under `src/` declaring `[DllImport]`/`[LibraryImport]`, filtered against both sanctioned roots) finds **zero** declarations outside the federated surface today. `SanctionedInteropSurface.cs:24`'s own in-code comment still names `ManagedBusBridge` in the present tense as "the one genuine DFK002 violation" — that phrasing is stale, describing a state the same Phase β triage already resolved by relocation, not a live finding.

---

## §7 — Per-rule detail template

The predecessor's per-rule template — one entry per shipped rule, giving the К-L/Lesson invariant reference, category, shipped severity, status, a 3–5 sentence detection narrative, diagnostic message text, example violation patterns, a suppression-policy citation to §3 (never a restatement), and positive/negative test cases — remains the intended shape for detail beyond §1.1's registry table.

> **FENCED (target / planned — not current truth):** per-rule population against this template has not started. Carried forward honestly from the predecessor's "§10 — empty at v0.4.0" note: nothing above should be read as implying per-rule detail pages exist. [ROADMAP.md](../ROADMAP.md) «Analyzer track» owns this as a future item; until it lands, §1.1's registry table is the single populated, current-truth surface for what each rule does.

---

## Cross-references

| Document | Relation | Note |
|---|---|---|
| [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) | defers-to | Part 0 carries the К-Lxx invariant canonical rows every `DFK###` rule encodes 1:1 |
| [CODING_STANDARDS.md](../methodology/CODING_STANDARDS.md) | defers-to | §5.3 "DFK-WAIVER — the suppression law" — cited in §3, never restated |
| [ROADMAP.md](../ROADMAP.md) | defers-to | «Analyzer track» owns phase history (β/γ/δ), the Q-L-1 adaptive gate, and every deferred-rule activation trigger in §4 |
| [K_CLOSURE_REPORT.md](./K_CLOSURE_REPORT.md) | cites | §7 carries the historical pre-rename DF-namespace detection narratives (18 active + 4 reserved); numbering superseded by §1.1 above, kept here for lineage only |
| [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md) | cites | §3 C5, §6.1 N-13/N-14, §6.4 N-26 — the findings this document resolves |
| [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) | cites | owns its own §3.7 (D-2, historically "M3.4"; `[ModCapabilities]` honesty analyzer) — untouched by this document or by the retirement in §5 |
| [FRAMEWORK.md](../governance/FRAMEWORK.md) | cites | §7 ratification protocol; §14.7 authority-surface predicate that excluded both retired Draft predecessors |

## Amendment protocol

Non-semantic v0.x corrections (registry additions, severity corrections already ratified elsewhere, deferred-registry moves) land as ordinary edits with a change-history row. A severity or diagnostic-ID change to a shipped rule must land together with its `AnalyzerReleases.Shipped.md` "Changed Rules" entry and matching `.editorconfig` key in the same commit (§3) — the three-surface agreement is re-verified at every future edit, not only at this rework. Un-retiring or reconstituting any part of the DF_TS program (§5) is a semantic v1.x correction requiring FRAMEWORK.md §7 ratification, not a routine edit.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R2-15..R2-19): §1.3 test-tree census 19→20 `.cs` files (matches the section's own itemization); §1 stale-lens paragraph corrected — the THREADING successor already fixed its analyzer references, only `historical/` copies retain the pre-Phase-β snapshot; §5 dangling-reference list re-anchored to `historical/` paths with the CONTRACTS repair acknowledged (sole live carrier = `SystemExecutionContext.cs:29`); "§11.1 M3.4" pointers → "§3.7 (D-2, historically 'M3.4')" ×2; §4 header split 13 → 12 deferred + 1 scope-exclusion (DFL025-C, per its own row). Register version 0.1.0 → 0.1.1; the row below's "1.0.0-draft" is the authoring session's in-doc label for that same 0.1.0 enrollment. |
| 1.0.0-draft | 2026-07-15 | Initial authored-rework superseding DOC-A-ANALYZER_RULES + DOC-A-ARCHITECTURE_TYPE_SYSTEM + DOC-A-MAX_ENG_REFACTOR_TRACK_B. Re-verified the 17-rule shipped surface, wiring, test census, and waiver census against code at HEAD `35364c2`; formally retired the DF_TS analyzer program (session finding N-13) and re-registered its one surviving idea as a DEFERRED, unassigned-ID candidate (§5). |