---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-CODING_STANDARDS
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "2.1.3"
next_review_due: 2027-06-11
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-CODING_STANDARDS
---
# Coding Standards

A single code style is not aesthetics — it is a navigation tool. The primary and
permanent reader of this codebase is an AI agent operating in session-mode
pipelines (PA-001): executor sessions, survey agents, and audit passes all read
the repository top-down, file-by-file, with no prior context. Every habit a
reader must decode is attention spent on style instead of architecture. The rules
below codify the **real, observed** norm of the Dual Frontier source tree, and
every claim of enforcement names the on-disk artifact that realizes it. Where the
codebase is not yet internally consistent, the inconsistency is recorded as a
non-normative observation, never retro-fitted into an obligation.

## §1 — Authority & Scope

This document is the standing law for how Dual Frontier code is written. It is
Tier 1 LOCKED (REGISTER `DOC-B-CODING_STANDARDS`). It is the single source of
truth for naming, file organization, nullability, comment language, marker
families, exception discipline, and commit shape across:

- the **C# managed layer** (`src/**`, `tests/**`, `tools/**` C# projects);
- the **C++20 native kernel** (`native/DualFrontier.Core.Native/**`);
- **tooling scripts** (`tools/**`, PowerShell 5.1);
- **commit discipline** (subject/body shape, trailers, push policy).

This document does **not** govern, and defers to, the following authorities —
cite them, do not restate them here:

- **Architecture** — substrate invariants (К-Lxx), kernel design, the analyzer
  rule catalogue: `docs/architecture/KERNEL_ARCHITECTURE.md`,
  `docs/architecture/ANALYZER_RULES.md`. ANALYZER_RULES owns the rule inventory;
  it defers the suppression law to this document §5.3.
- **Brief / methodology content** — how cascades are scoped, deliberated, and
  closed: `docs/methodology/METHODOLOGY.md`.
- **Test methodology** — layer taxonomy, census meta-tests, isolation patterns,
  coverage philosophy: `docs/methodology/TESTING_STRATEGY.md` (cited from §5 and
  §9; never restated).
- **Reserved-surface mutability** — which symbols/paths a cascade may revise and
  the `Skeleton revisions:` commit-body record:
  `docs/methodology/RESERVED_SURFACE_MUTABILITY.md` (cited from §8).
- **Future analyzer capability** — every not-yet-built detection, severity
  promotion, or rule-family activation: `docs/ROADMAP.md` §Analyzer track.

### §1.1 — Stop, escalate, lock

When implementation meets a style question this document does not answer, the
response is **amend this document** (per §10) and obtain ratification — never
improvise a per-file answer. An unanswered style question resolved ad hoc in code
is a latent drift source: the next agent reads the improvisation as precedent.
The structural strength of the codebase depends on this document being the only
place where style truth is decided. This rule is the direct application of
PA-002 («без костылей») to the documentation layer: an unmarked, undocumented
local convention is a shortcut.

## §2 — C# Conventions

### §2.1 — Naming

| Surface | Convention | Example |
|---|---|---|
| Public / protected members, types | `PascalCase` | `HealthComponent`, `TakeDamage` |
| Constants | `PascalCase` | `public const int MaxPawnsPerColony = 100;` |
| Private fields | `_camelCase` | `private readonly World _world;` |
| Interfaces | `I`-prefix | `IComponentStore`, `IEventBus` |
| Generic parameters | `T` / `TContext`-style | `IComponentStore<T>`, `Dispatch<TEvent>` |
| Namespaces / files | mirror the path | `src/DualFrontier.Core/ECS/World.cs` → `namespace DualFrontier.Core.ECS;` |

The leading underscore distinguishes a private field from a parameter or local
at a glance in a `git diff`, where IDE colouring is absent. Namespace mirrors the
physical directory; the reverse holds too — a type lives where its namespace
says it does.

### §2.2 — File-scoped namespaces

New code uses only the file-scoped form (`namespace X;`). The convention is the
prevailing norm of the tree (~250 files). Less indentation, more code on screen.

```csharp
// Convention
namespace DualFrontier.Core.ECS;

public sealed class World { /* ... */ }
```

This is a **convention**, not a machine-checked rule: `.editorconfig` is
charset-only (no IDE0161 severity), so the build does not reject a block-braced
namespace. One historical exception survives (`GameLoop.cs`, see §2.8); new code
does not add a second.

### §2.3 — One class per file

Open the IDE, type the class name, see the file. The narrow exceptions: a small
paired enum or `readonly struct` logically inseparable from the main type, and
`private` nested types owned by their enclosing class. No multi-class dumping
grounds; no 500-line `Helpers.cs`.

### §2.4 — Member order

The prevailing top-down order, by what the reader needs first:

1. `const` fields
2. `static readonly` fields
3. `private readonly` fields
4. `private` mutable fields
5. Constructors
6. `public` properties
7. `public` methods
8. `protected` methods
9. `private` methods
10. Nested types

The reviewer sees, in one pass: which constants are baked in, what the type
holds, how it is built, what callers can reach, then how the work is done.

### §2.5 — Explicit types over `var`

New code uses explicit types. `var` is not part of the observed style of the
tree; an explicit type carries the contract at the read site without an inference
hop. (This is the codified real norm, not an imported preference.)

### §2.6 — Nullable reference types

Every project builds with `<Nullable>enable</Nullable>`
(`Directory.Build.props`). An API that may return absence returns `T?`; one that
may not returns `T`. No `#nullable disable`; no `!` (null-forgiving) without an
inline justification.

**Enforced.** Nullable violations surface as compiler warnings, and
`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` (`Directory.Build.props`)
promotes every warning to a build-breaking error. A nullability annotation
mismatch fails `dotnet build` (§9).

### §2.7 — No LINQ, no async in system / hot-path code

System code in `src/` does not use LINQ and does not use `async`. The kernel is a
fixed-timestep simulation substrate; query allocation and continuation
scheduling are both alien to the per-tick hot path. New `src/` system code holds
this line by convention.

This is a **convention** today — no analyzer enforces it (detection of forbidden
namespaces and async surfaces is `Planned — see docs/ROADMAP.md §Analyzer
track`).

### §2.8 — Observed inconsistencies (non-normative)

These are baseline observations of the tree as it stands, recorded so a future
reader does not mistake them for sanctioned patterns or for hidden rules. They
impose **no** retroactive obligation and are normalized opportunistically:

- `src/DualFrontier.Application/Loop/GameLoop.cs` is the **sole** block-braced
  namespace among ~250 file-scoped files.
- `src/DualFrontier.Core/Math/SpatialGrid.cs` carries the **sole**
  `using System.Linq;` in `src/`.
- `SimulationStateController.WaitForQuiescenceAsync`
  (`src/DualFrontier.Application/Loop/SimulationStateController.cs`) is the
  **sole** production `async` method — a lifecycle controller polling for
  scheduler quiescence, not a per-tick system; it is the named exception to §2.7,
  not a counterexample to it.
- `TickRates` constants use `SCREAMING_CASE` rather than the §2.1 `PascalCase`
  constant form.

### §2.9 — Stack-frame retention for collected resources

Some code paths must give the GC a chance to reclaim a resource —
`AssemblyLoadContext` unload, finalizer cleanup, weak-reference cache eviction —
and run only after **every** strong reference has left the executing thread's
stack frames. Two C# constructs silently retain such references: a `foreach`
iteration variable (in DEBUG, until the method returns) and — always, in any
build — **lambda closure display classes**. A lambda capturing a local makes the
compiler synthesize a heap `<>c__DisplayClass` holding that local, rooted from
the enclosing frame for the whole method scope, however out-of-scope the local
looks textually.

**Rule.** A method that must let the GC collect a resource it referenced splits
into **(a)** a non-inlined helper that captures, uses, and releases the strong
references, returning only a `WeakReference`, and **(b)** the caller, which runs
the wait/spin phase with no resource-holding locals or lambdas in its frame.
`[MethodImpl(MethodImplOptions.NoInlining)]` is non-negotiable on the helper —
without it the JIT may inline it back and recreate the display class in the
caller's frame.

```csharp
public IReadOnlyList<ValidationWarning> UnloadMod(string modId)
{
    if (!_activeMods.TryGetValue(modId, out LoadedMod? mod)) return [];

    var warnings = new List<ValidationWarning>();
    // Steps that capture `mod` in lambdas are confined to the helper's frame.
    WeakReference alcRef = RunUnloadSteps1Through6AndCaptureAlc(modId, mod, warnings);

    // mod is unreferenced here; the display class lived only in the returned
    // helper frame. This frame is clean, so the spin can observe the release.
    TryStep7AlcVerification(modId, alcRef, warnings);
    return warnings;
}

[MethodImpl(MethodImplOptions.NoInlining)]
private WeakReference RunUnloadSteps1Through6AndCaptureAlc(
    string modId, LoadedMod mod, List<ValidationWarning> warnings)
{
    TryUnloadStep(1, modId, warnings, () => mod.Api?.UnsubscribeAll());
    var alcRef = new WeakReference(mod.Context);
    _activeMods.Remove(mod);
    return alcRef;
}
```

The pattern surfaced empirically in M7.3 (commit `9bed1a4`): keeping `mod` as a
local in `UnloadMod` let Step 1's lambda (`mod.Api?.UnsubscribeAll()`) hoist
`mod` into a display class rooted by `UnloadMod`'s frame, so the ALC spin never
observed release and emitted spurious `ModUnloadTimeout` warnings. The same
discipline applies to finalizer cleanup, weak-reference caches, and any
GC-dependent assertion; test helpers follow it identically. The rule is live:
NoInlining helpers exist in
`src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` and
`tests/DualFrontier.Modding.Tests/Pipeline/ModUnloadAssertions.cs` (verified at
this rewrite).

## §3 — C++20 Native Kernel Conventions

The native kernel (`native/DualFrontier.Core.Native/`) follows its own
language-idiomatic style, distinct from the managed layer.

- **`snake_case` throughout** — types, functions, variables, free functions.
- **Trailing-underscore private members** (`registry_`, `entities_`).
- **`namespace dualfrontier`** wraps the implementation.
- **Header guards**: the public C API header (`include/df_capi.h`) uses an
  include guard; internal headers use `#pragma once`.

### §3.1 — The `extern "C"` ABI boundary

The native kernel exposes a flat C ABI. Error vocabulary is plain `int32` status
codes and handle-or-null returns; **C++ exceptions never cross the boundary**.
Every exported `DF_API` entry point follows one shape: null-guard the handle →
`try { real work } catch (...) { swallow, return 0 / nullptr / no-op }`. The
caller observes a rejected request through the return value, never through an
exception that would be undefined behaviour across the `extern "C"` line.

```cpp
DF_API void df_world_destroy_entity(df_world_handle world, uint64_t entity) {
    if (!world) return;
    try {
        as_world(world)->destroy_entity(unpack_entity(entity));
    } catch (...) {
        // K1: destroy_entity now throws if spans are active. Swallow to keep
        // the C ABI noexcept-equivalent — the caller saw the request rejected.
    }
}
```

(Verbatim from `native/DualFrontier.Core.Native/src/capi.cpp`.) A throw and its
boundary catch are **inseparable**: any native method reachable across the ABI
that can throw must have its catch at the boundary. This is the
"ABI boundary exception completeness" obligation; the architectural rationale
lives in `METHODOLOGY.md` — cite it, do not restate it.

## §4 — Project & Build Organization

### §4.1 — The twelve managed src projects

The managed substrate is twelve `src/` projects: **AI, Application, Components,
Contracts, Core, Core.Interop, Crypto.Future, Events, Launcher, Persistence,
Runtime, Systems**. Project-to-layer assignment is immutable structure (see
RESERVED_SURFACE_MUTABILITY.md); symbol names and file layout below the project
level are mutable surface.

### §4.2 — The `Directory.Build.props` chain

The repo-root `Directory.Build.props` carries the canonical project properties
for the whole solution:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <LangVersion>12.0</LangVersion>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <!-- CS1591: Missing XML comment for publicly visible type or member. -->
  <!-- Отключаем на уровне всего решения — XML docs обязательны только для публичных контрактов. -->
  <NoWarn>$(NoWarn);CS1591</NoWarn>
  <Company>DualFrontier</Company>
  <Product>Dual Frontier</Product>
</PropertyGroup>
```

`CS1591` (missing XML comment) is the **only** suppressed warning, and it is a
compiler documentation warning — not a DFK/DFL analyzer diagnostic. The root file
also carries a `CompileShaders` MSBuild target (glslangValidator, conditioned to
the `DualFrontier.Runtime` build only).

The `src/`-scoped `Directory.Build.props` imports the root, then wires the
analyzer to all twelve src projects:

```xml
<Import Project="$(MSBuildThisFileDirectory)..\Directory.Build.props" />

<ItemGroup Label="DualFrontier.Analyzers (К-extensions cascade #5)">
  <ProjectReference
    Include="$(MSBuildThisFileDirectory)..\tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj"
    OutputItemType="Analyzer"
    ReferenceOutputAssembly="false" />
</ItemGroup>
```

(Header comment cites Q-L-5 + the β4 ratification 2026-05-25.) Tests and tools
projects sit outside `src/` and do not inherit the analyzer wiring.

### §4.3 — Central Package Management

`Directory.Packages.props` sets `<ManagePackageVersionsCentrally>true</…>`;
package versions live in exactly one file. The nine pinned packages:

| Package | Version | Role |
|---|---|---|
| Microsoft.CodeAnalysis.CSharp | 5.3.0 | analyzer host |
| Microsoft.CodeAnalysis.CSharp.Workspaces | 5.3.0 | analyzer host |
| Microsoft.CodeAnalysis.Analyzers | 5.3.0 | analyzer host |
| Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit | 1.1.2 | analyzer test harness |
| Microsoft.NET.Test.Sdk | 17.11.1 | test platform |
| xunit | 2.9.2 | test framework |
| xunit.runner.visualstudio | 2.8.2 | test runner |
| FluentAssertions | 6.12.1 | assertion library |
| BenchmarkDotNet | 0.13.12 | benchmark harness |

Three deliberate exclusions are recorded in the file with their locked
rationale: **PublicApiAnalyzers** (Q-L-13 — audience-driven deferral, community
ecosystem absent per PA-001), **code-fix testing** (Q-L-15 — code-fix providers
permanently dropped, AI-agent-first profile per PA-001), and
**BannedApiAnalyzers** (Q-L-12 — closed historical concern, Godot removed).

## §5 — Marker Family Registry

A marker family is a textual convention with a defined form, semantics,
introduction rule, closure semantics, and a **verbatim census method** so the
population is reproducibly measurable. Census meta-tests are the enforcement
mechanism and live in `TESTING_STRATEGY.md` §4 (cited, not restated). The counts
below are the measured baseline at this rewrite and are pinned there.

### §5.1 — `[ReservedStub]` attribute

**Form.** `[ReservedStub(ReservedStubPurpose purpose, string reason)]` —
declared at `src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs` with
the purpose enum at `…/ReservedStubPurpose.cs` (`BuildComposition` /
`ArchitecturalSketch`). The `reason` argument is mandatory; the constructor
throws `ArgumentException` on null/whitespace (Q-L-10 + PA-002).

**Semantics.** Marks a type, method, or property as a **structurally-present,
functionally-inert** surface — required for build composition or architectural
sketching, not a runtime implementation. This is the canonical "skeleton" marker
of the codebase (METHODOLOGY Lesson #N12, Defensive Reserved Stub Pattern;
Lesson #25 on lying tests — cite, do not restate).

**Introduction rule.** Every reserved surface carries the attribute with a
specific `reason` naming its activation trigger (e.g. a К-L LOCK cascade or a
milestone). All 34 current sites use the parameterized form — there are no
bare `[ReservedStub]` applications.

**Closure semantics.** A site closes when the per-feature realization commit
replaces the inert surface with a real implementation and removes the attribute.
Closure is per-feature; the census is therefore **not** monotonic — new reserved
surface may legitimately raise the count.

**Census method (verbatim).** The reserved-surface census counts `[ReservedStub`
attribute application sites in `src/**/*.cs` (rg `--type cs`), excluding the
attribute definition file
`src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs`, matching rg
pattern `\[ReservedStub`; **current pin: 34 application sites across 13 files**.
(The definition file contributes 0 under this pattern and is excluded by rule
regardless.) The meta-test asserting the exact pin lives in
`TESTING_STRATEGY.md` §4.

Compile-time enforcement: rule **DFL025_A** (behavior invocation against tagged
members requires the declaring `[Trait]`) detects since A'.9.1 Phase β and
enforces at Warning — build-breaking under `TreatWarningsAsErrors` — since
Phase γ (Release 1.0).

### §5.2 — Doc-tag families

These are free-text tags in comments and XML docs. They are registered **as-is**
with their verbatim census patterns and baseline counts. The baseline imposes
**no retroactive obligation** on existing sites — they are baseline-registered
and normalized opportunistically. **Forward rule:** a new deferral marker names
its closing phase or cascade.

| Family | Census pattern (rg, on `src/ --type cs`) | Baseline |
|---|---|---|
| `stub` | `rg --count-matches -i '\bstub\b' src/ --type cs` | 51 matches / 20 files |
| `deferred` | `rg --count-matches -i '\bdeferred\b' src/ --type cs` | 82 matches / 51 files |
| `TODO` | `rg --count-matches '\bTODO\b' src/ --type cs` (case-sensitive) | 136 matches / 53 files |
| `Phase 6` | `rg --count-matches 'Phase 6' src/ --type cs` | 23 matches / 11 files |
| `not yet` | `rg --count-matches -i 'not yet' src/ --type cs` | 10 matches / 9 files |

Baselines refreshed 2026-07-01 (the F-25 owed fold): `stub` 48/18 → 51/20 and
`deferred` 79/48 → 82/51 had drifted at the 2026-06-12 comment-citation pass
(F-25 census-delta record); the compiled census meta-tests have carried the live
values since Phase β. `not yet` refreshed 2026-07-15: 8/7 → 10/9 — the CX-06 +
CX-21 fix-comment markers landed at Codex `61f08ef` without a same-commit delta,
recorded here as the F-33 census-delta (CODEX_CLOSURE C2). `TODO` / `Phase 6`
unchanged.

### §5.3 — `DFK-WAIVER` — the suppression law

**Status.** NEW with this rewrite; load-bearing for A'.9.1 Phase β, where
analyzer detection lands and per-diagnostic triage (fix / suppress / refine)
begins. This section is the standing law for **how** a DFK/DFL diagnostic may be
suppressed once detection exists.

**Form.**

```csharp
// DFK-WAIVER(DFK013): <one-line reason citing authority — Q-L-#, К-L#, F-#, or
//                     "false positive pending rule refinement, see ROADMAP.md §Analyzer track">
#pragma warning disable DFK013
...minimal scope...
#pragma warning restore DFK013
```

**Rules.**

1. **Adjacency.** The `// DFK-WAIVER(...)` comment immediately precedes the
   `#pragma warning disable`.
2. **Narrow scope.** The disable spans the narrowest possible region and is
   **always** paired with a matching `restore`. Never disable a diagnostic for
   the remainder of a file.
3. **Allowed reason classes** — a waiver's authority citation must be one of:
   - *false-positive pending refinement* — must reference the ROADMAP
     Analyzer-track refinement entry that will fix the rule;
   - *sanctioned architectural exception* — must cite the locked decision ID
     (Q-L-#, К-L#);
   - *generated or interop-mandated code* — the diagnostic does not apply to
     machine-generated or ABI-shaped surface.
4. **No blanket suppression.** File-scope or project-scope `<NoWarn>` for any
   DFK/DFL diagnostic is **forbidden**. (The one existing root-props `NoWarn`
   entry is `CS1591` only — a compiler documentation warning, not a DFK/DFL
   diagnostic; see §4.2.)
5. **Census-tracked.** Every waiver is counted by the DFK-WAIVER census
   (`TESTING_STRATEGY.md` §4 — expressions and the live pin). Baseline = 0 at
   this section's authoring (2026-06-11); **current pin = 2** since the A'.9.1
   Phase β triage — the two К-L19-sanctioned DFK001 waivers in
   `ValidationLayer.cs`, asserted by the compiled census meta-test. Every
   increase requires a citation satisfying rule 3.
6. **No orphan waivers.** A waiver whose authority citation does not resolve
   (dangling Q-L/К-L/F-# or a refinement entry that does not exist) fails review.

**Diagnostic-ID form.** A waiver names the **descriptor ID string** — the
**underscore form** for sub-rules and variants (`DFK003_1`, `DFL025_A`),
identical to the file/class form since the Phase β descriptor-ID adjudication
(Crystalka-ratified 2026-07-01; ANALYZER_RULES §4 naming convention). The
superseded dotted/hyphen forms (`DFK003.1`, `DFL025-A`) are rejected by Roslyn
`ReportDiagnostic` as invalid identifiers — a pragma naming them would suppress
nothing.

**Supersession.** This law **supersedes** the `// DFK###-SUPPRESS:` suppression
sketch in `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` §7.3. That
brief is execution-context tier (a derived plan); this document is standing law.
Where the §7.3 sketch and this section differ, this section governs: the marker
keyword is `DFK-WAIVER(<id>)` (not `DFK###-SUPPRESS`), the comment **precedes**
the disable, and the authority-citation classes of rule 3 are mandatory.

**GlobalSuppressions ban.** A `GlobalSuppressions.cs` file (assembly-level
`[assembly: SuppressMessage]`) is **forbidden** — it is the file-scope blanket of
rule 4 at assembly granularity. The ban is law **now**; no `GlobalSuppressions.cs`
exists in the tree. The self-policing analyzer rule DF999 detects the pattern
since A'.9.1 Phase β and enforces at Warning (build-breaking under
`TreatWarningsAsErrors`) since Phase γ (Release 1.0).

## §6 — Comments & Documentation Language

All new artifacts — code comments, XML docs, commit bodies, governance prose —
are **English**. English is the lowest-friction shared encoding for the
agent-primary reader (PA-001) and for `git diff`, search, and string equality.

**Comments explain WHY, not WHAT.** The code already shows what it does. A
comment earns its place by explaining why the code exists, which edge case it
guards, what would break if it were removed, and **which internal law it answers
to**. Authority is cited by ID, and the chain stays unbroken: `К-L#`, `PA-00#`,
`Q-L-#`, `F-#`, `Lesson #N…`, or a document section. A surface with no citable
authority is a surface whose justification has not been authored — which usually
means the code is premature.

```csharp
// Per К-L1 the native kernel owns entity lifetime; this managed wrapper only
// translates the int32 status. Returning the raw handle here would leak the
// ABI boundary into Application — see §3.1.
```

**Bi-script citation reality (finding F-4).** Existing code cites invariant IDs
in both scripts: Cyrillic «К-L» (140 occurrences across 43 files) and Latin
"K-L" (38 across 19). Any census or grep over invariant citations **must match
both scripts**. Forward convention for new code: either script is acceptable, but
the numeric ID must be exact (`К-L14` and `K-L14` denote the same invariant;
`K-L4` is a different one). No narration-of-what comments; no decorative
banners.

### §6.1 — Citation form

Internal citations — from code comments, XML docs, and living documents — use
**stable identifiers and section topics**:

- a law/invariant/decision ID alone where one exists: `К-L13`, `PA-002`,
  `Q-L-10`, `F-5`, `Lesson #N12`;
- a document name plus section anchor *with the section's topic in words*:
  `MOD_OS_ARCHITECTURE §3.2 (capability verbs)`, `ANALYZER_RULES §4 (rule
  registry)`, `TESTING_STRATEGY §4.1 (reserved-surface census pin)`. The topic
  keeps the citation resolvable when the owning document renumbers — the same
  rule `RESERVED_SURFACE_MUTABILITY.md` §3 (mutable catalog, doc-section item)
  states for cascade briefs.

**Forbidden** in living prose and in code comments:

1. **Version pins of living documents** (`MOD_OS_ARCHITECTURE.md v1.7 §4.6`,
   `VULKAN_SUBSTRATE v2.0 LOCKED`) — the register owns versions; a pinned
   citation goes stale the moment the cited document moves, then silently
   misdirects every later reader. Pins are permitted only inside dated
   snapshots and records (change-history rows, EXECUTED briefs, closure
   reports), where they describe a moment in time.
2. **URL-fragment anchors** (`…/ANALYZER_RULES.md#4--…`) — heading slugs break
   under restructure; the v0.2.0 ANALYZER_RULES restructure broke both
   anchor-class citations in `ReservedStubAttribute.cs` (recon Anomaly 4,
   2026-06-11). Cite by section number + topic instead. The auto-generated
   `register_view_url` frontmatter line is the sanctioned exception
   (machine-maintained, regenerated with the render).

Enforcement: review-bound today — no analyzer rule detects citation form; any
future detection appears first as `Planned — see docs/ROADMAP.md §Analyzer
track`. This rule landed in the same cascade that fixed the found instances of
both classes (METHODOLOGY §12.7 step 9, SYNTH-2 same-cascade obligation).

## §7 — Exception & Error Discipline

The codebase distinguishes three throw shapes, and keeping them distinct is what
keeps the §5.1 census clean:

1. **Guard-clause throws** — `InvalidOperationException` (or `ArgumentException`)
   with a diagnostic message describing the violated precondition. These are
   ordinary defensive validation and are **not** a stub signal.
   `ManifestParser` is the exemplar, with ~32 such guards. A guard throw never
   carries `[ReservedStub]`.
2. **Reserved-stub throws** — an inert surface that throws to signal "not yet
   realized" **always** carries `[ReservedStub]` (§5.1). The attribute is what
   separates a deliberate skeleton from a guard clause in any census; an
   unmarked "not implemented" throw is a §1.1 / PA-002 violation.
3. **Native boundary** — native error codes (`int32`, handle-or-null) cross the
   `extern "C"` ABI per §3.1. The managed interop wrapper translates `0` /
   `nullptr` into a typed managed exception **at the boundary**, so managed
   callers never see a raw native status code leak past `Core.Interop`.

## §8 — Atomic Commit Discipline

A commit is the smallest unit of architectural reasoning that survives in
history. **In a falsifiable research framework the commit history IS the
dataset:** defect rate, pipeline economics, and the architectural-integrity
claims of К-L14 are all measured from it. History integrity is therefore
research-data integrity — this is why the prohibitions below are absolute, not
stylistic.

### §8.1 — Subject line

```
<scope>(<sub-scope>): <imperative description>
```

The observed `<scope>` vocabulary, codified: **`analyzer`, `chore`, `docs`,
`feat`, `fix`, `governance`, `perf`, `refactor`, `reports`, `test`**.
Parenthesized `<sub-scope>` is used when the scope has internal structure —
observed values include `standing-law`, `register`, `brief`, `axioms`, `cpm`,
`tests`, `stubs-architecture`, `rename`, `dd-1`, `dd-2`. The legacy engine
prefixes `core:` / `contracts:` / `interop:` / `native:` / `modding:` are
historical (visible in old history). The full pre-law census (recon 2026-07-01,
1054-commit history at `4cc5e7e`) additionally observed the scope tokens
`tests`, `native`, `sprite`, `experiment`, `interop`, `compute`, `bench`,
`vulkan`, `src`, `shaders`, `scaffold`, `runtime`, `revert`, `kernels`,
`build`, `UI` — 53 commits, every one predating the 2026-06-11 v2.0.0
codification — plus 81 unprefixed subjects from the same pre-law era (42 merge
commits + 39 plain GitHub-web-era subjects, including one hybrid
`native+compute:` form that escapes the scope grammar). Post-codification
history is 100% in-vocabulary. None of the historical forms are forbidden
retroactively, but **new commits use the `prefix(sub-scope):` form**. A new
scope prefix requires a `governance` commit that amends this section first.

### §8.2 — Body — seven sections

Every non-trivial commit carries a structured body:

1. **Authority** — the document section(s) or locked decision the change answers
   to.
2. **Scope** — files touched, with NEW / MODIFIED / DELETED markers.
3. **Rationale** — why this approach, especially when alternatives were weighed.
4. **Scope boundaries** — what is deliberately **not** touched (this is what
   makes the commit auditably atomic).
5. **Skeleton revisions** — *when applicable*, the reserved-surface deltas, one
   conceptual change per entry (`<from> → <to>` + one-line rationale), per
   `RESERVED_SURFACE_MUTABILITY.md`.
6. **Verification** — the exact gates run (§9) and their results.
7. **References** — documents cited, with versions/sections.

Followed by a `Co-Authored-By:` trailer naming the model that produced the
commit (e.g. `Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>` or the
`Claude Opus 4.7` form). Other body elements seen in history — `register_version
X.Y -> X.Z` lines, `К-L impact` lines, a bare claude.ai session URL, verbatim
Crystalka direction quotes — are **observed conventions**, recorded here as
descriptive, not mandated.

### §8.3 — Hard prohibitions

- **No force-push** (`--force` or `--force-with-lease`) — once a branch is
  pushed, its history is append-only.
- **No history rewrite on pushed branches** — interactive rebase, amend, or
  filter only on local, never-pushed commits.
- **No squash** — if the work is fifteen commits, fifteen commits ship; the
  reviewer's view of the reasoning is exactly the sequence as authored.

### §8.4 — Executor never pushes

Pushes to `origin` are the **architect's** act (Crystalka). An execution session
authors commits locally and stops at the closure boundary; the push to the
remote is a deliberate human ratification step, never an automated tail of a
cascade.

## §9 — Verification Gates

These are the gates that **exist**. Each names its artifact. (Operational depth —
exact invocation forms, PowerShell 5.1 specifics — lives in
`DEVELOPMENT_HYGIENE.md`; test law lives in `TESTING_STRATEGY.md`. Cited, not
restated.)

- **Managed build.** `dotnet build DualFrontier.sln -c Release`. Compile-clean is
  expected; `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
  (`Directory.Build.props`) makes any warning a failure. Caveat: a running test
  host file-locks output assemblies — close test runners before building.
- **Native build.** The VS-bundled CMake is not on `PATH`; invoke by full path:
  `"D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe"
  --build native\DualFrontier.Core.Native\build --config Release`.
- **Native selftest.** `df_native_selftest.exe` (built from
  `native/DualFrontier.Core.Native/test/selftest.cpp`).
- **Solution tests.** Run per `TESTING_STRATEGY.md` (the test-law authority);
  this document does not restate the test layer taxonomy or run command.
- **Register validation.** `& .\tools\governance\sync_register.ps1 -Validate`
  (PowerShell 5.1). **Side-effect:** the script unconditionally rewrites
  `docs/governance/VALIDATION_REPORT.md` (around line 380), so **every** validate
  run produces a tracked change that must be folded into the same commit — see
  `DEVELOPMENT_HYGIENE.md` §4 for the commit-folding protocol. Never run
  `sync_register.ps1 -Sync` outside a ratified register cascade.

**Analyzer enforcement: live (A'.9.1 Phase γ, Release 1.0).** The
DualFrontier.Analyzers project is wired to all twelve src projects (§4.2) and
ships **17 detecting rules** (9 Architecture / 3 Discipline / 5 NativeBoundary)
at their shipped severities: 11 Error + 5 Warning — both build-breaking under
`TreatWarningsAsErrors` — plus DFL025_B at descriptor Info (`.editorconfig`
`suggestion`, IDE-only). `AnalyzerReleases.Shipped.md` records Release 1.0 and
RS release tracking cross-checks it against the descriptors; the root
`.editorconfig` restates all 17 severities as `dotnet_diagnostic.<ID>.severity`
keys, values identical to the descriptors. A DFK/DFL/DF999 violation therefore
fails the build; suppression only per the §5.3 DFK-WAIVER law (census pin: 2).
The rule inventory authority is `ANALYZER_RULES.md` §4.1 (rule registry).

**CI: none.** No CI exists; none is currently scheduled. There are no
`.github/workflows`, Azure Pipelines, or AppVeyor configurations in the tree, and
no CI item appears on the roadmap — the truthful present-tense state, not a
deferred capability.

## §10 — Amendment Protocol & Change History

### §10.1 — Amendment protocol

This document is Tier 1 LOCKED. An amendment proceeds:

1. **Surface** — propose the change to the owner (Crystalka) as an explicit
   direction; the agent does not default-amend a LOCKED standing law.
2. **Rationale** — document what changes, why, and what it deprecates.
3. **Semver** — PATCH for clarification/correction, MINOR for a new rule or
   family, MAJOR for inverting an existing rule.
4. **REGISTER update** — bump `version` and `next_review_due` in the frontmatter
   mirror; the change rides a `governance(...)` commit (§8) with a validate run
   folded in (§9).
5. **Cross-doc propagation** — when an amendment touches a rule cited elsewhere,
   update those citations in the same cascade. Documents that cite this one:
   `TESTING_STRATEGY.md` (census meta-tests, §5/§9), `DEVELOPMENT_HYGIENE.md`
   (build/validate gates, §9), `ANALYZER_RULES.md` (defers suppression law to
   §5.3), `RESERVED_SURFACE_MUTABILITY.md` (the `Skeleton revisions` body law,
   §8.2), and `METHODOLOGY.md` (commit-discipline and brief-integration rules).

An amendment that contradicts a brief does not lose: a brief contradicting this
standing law is wrong by default — correct the brief, or ratify the amendment
here **before** the brief locks.

### §10.2 — Change history

| Version | Date | Change |
|---|---|---|
| **2.1.3** | 2026-07-15 | CODEX_CLOSURE cascade census-pin refresh (§5.2): `not yet` 8/7 → 10/9 — the CX-06 + CX-21 fix-comment markers landed at Codex `61f08ef` without a same-commit census-delta; recorded here + as ROADMAP F-33 (operator-ruled FOLD of the Skarlet-gate H2b). CensusMetaTests green post-refresh. **PATCH.** |
| **2.1.2** | 2026-07-02 | A'.9.1 Phase δ rider — commit-vocabulary historical reconciliation: §8.1 historical-prefix note extended with the full pre-law observed scope-token census (16 tokens over 53 commits, all predating the 2026-06-11 codification) + the unprefixed-era note (81 subjects: 42 merges + 39 plain). Descriptive correction of the incomplete observation — no normative change; post-codification history is 100% in-vocabulary (no live violation). **PATCH.** |
| **2.1.1** | 2026-07-01 | A'.9.1 Phase γ propagation PATCH (METHODOLOGY §12.7 step 9 / SYNTH-2 + §10.1 rule 5 cross-doc propagation): §9 «Analyzer enforcement: none today» → live Release 1.0 enforcement (16 build-breaking + 1 IDE-only; `.editorconfig` primed; `Shipped.md` populated); §5.1 + §5.3 DFL025_A / DF999 «Planned»-stub claims → detecting-since-β, enforcing-since-γ; §5.3 Diagnostic-ID form → underscore per the Phase β Crystalka-ratified descriptor-ID adjudication (the dotted/hyphen forms are Roslyn-invalid — the paragraph as written would have produced no-op waivers); §5.3 rule 5 waiver census → current pin 2 (dated); §5.2 `stub`/`deferred` baselines 48/18 → 51/20, 79/48 → 82/51 (the F-25 owed fold; live values carried by the compiled meta-tests since Phase β); stale v2.0.0 end-marker synced. **PATCH.** |
| **2.1.0** | 2026-06-12 | §6.1 citation-form rule added (Architecture Truth Cascade, D11): internal citations use stable identifiers + section topics; version pins of living documents and URL-fragment anchors forbidden in living prose and code comments (pins live only inside dated records). Codifies the two breakage classes the 2026-06-11 ARCHITECTURE TRUTH RECON surfaced, in the same cascade that fixed the found instances (SYNTH-2). **MINOR.** |
| **2.0.0** | 2026-06-11 | Full rewrite to code-truth: marker-family registry (§5: `[ReservedStub]`, doc-tag families, the new `DFK-WAIVER` suppression law superseding `A_PRIME_9_1_…BRIEF` §7.3), C++ ABI-boundary discipline (§3), build-config quoted verbatim (§4), enforcement claims reduced to existing artifacts (§9 — analyzer stated as 17 non-detecting stubs; CI stated absent). **MAJOR.** Authored per `tools/briefs/STANDING_LAW_CASCADE_BRIEF.md`. |
| 1.0 | 2026-05-12 (era) | Initial codification — naming, file-scoped namespaces, nullability, one-class-per-file, member order, stack-frame retention discipline. Preserved as historical; superseded by 2.0.0. |

---

**End of CODING_STANDARDS.md v2.1.3 LOCKED**
