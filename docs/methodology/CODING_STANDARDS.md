---
register_id: DOC-B-CODING_STANDARDS
project: Dual Frontier
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 4.0.0
first_authored: 2026-07-15
last_modified: 2026-11-21
content_language: en
next_review_due: 2027-11-21
title: Coding standards
review_cadence: on-change+annual
last_review_date: 2026-11-21
last_review_event: 'MAJOR 3.0.0 -> 4.0.0 2026-11-21 (operator rulings 2026-11-21, REV_1): section 6 comment law INVERTED -- a comment states what the code cannot express and cites only law in force, never provenance (new section 6.2); transition by touched file, no backlog sweep (new section 6.3). Section 5 reconciled -- a deferral names the OPEN ledger row that owns the debt in the closed form TODO(F-<n>), a ledger row is never waiver authority, Phase 6 is a counted legacy backlog, the census pins restated to the live CensusMetaTests values. Section 8.1 gains the ci and build scopes and the ! breaking-change suffix; section 8.3 a pull request lands as a two-parent merge commit made by the operator, squash and rebase merges forbidden, fast-forward-only not required; section 8.4 names the operator, the architect and the integrator; section 9 states the Linux gates, the .NET governance tool and Gitleaks as the only CI. MAJOR per section 10.1 rule 3 (inverting existing rules). No lifecycle transition (LOCKED).'
reviewer: Crystalka
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
truth for naming, file organization, nullability, comment law, marker
families, exception discipline, and commit shape across:

- the **C# managed layer** (`src/**`, `tests/**`, `tools/**` C# projects);
- the **C++23 native kernel** (`native/DualFrontier.Core.Native/**`);
- **tooling** (`tools/**`: the .NET tool projects and the remaining scripts);
- **commit discipline** (subject/body shape, trailers, push and merge policy).

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

## §3 — C++23 Native Kernel Conventions

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
        // destroy_entity throws while a span is active (К-L7, span protocol).
        // Swallowing keeps the C ABI noexcept-equivalent.
    }
}
```

(The shape of `df_world_destroy_entity` in
`native/DualFrontier.Core.Native/src/capi.cpp`; the comment is written to section
6.2.) A throw and its
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
  <TargetFramework>net10.0</TargetFramework>
  <LangVersion>14.0</LangVersion>
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
also carries a `CompileShaders` MSBuild target (glslangValidator), conditioned to
the `DualFrontier.Runtime` build on a Windows host only; on any other host the
target is skipped and the committed `assets/shaders/*.spv` are the artifacts.

The SDK is pinned: the repo-root `global.json` sets `10.0.204` with
`rollForward: latestFeature`, so a build resolves to the highest installed 10.0
SDK at or above that floor (`DEVELOPMENT_HYGIENE.md` section 6, environment
notes, names the installed SDK).

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

Tests and tools projects sit outside `src/` and do not inherit the analyzer
wiring.

### §4.3 — Central Package Management

`Directory.Packages.props` sets `<ManagePackageVersionsCentrally>true</…>`;
package versions live in exactly one file. The nine pinned packages:

| Package | Version | Role |
|---|---|---|
| Microsoft.CodeAnalysis.CSharp | 5.3.0 | analyzer host |
| Microsoft.CodeAnalysis.CSharp.Workspaces | 5.3.0 | analyzer host |
| Microsoft.CodeAnalysis.Analyzers | 5.3.0 | analyzer host |
| Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit | 1.1.2 | analyzer test harness |
| Microsoft.NET.Test.Sdk | 18.8.1 | test platform |
| xunit | 2.9.3 | test framework |
| xunit.runner.visualstudio | 3.1.5 | test runner |
| AwesomeAssertions | 9.4.0 | assertion library (Apache-2.0 community fork replacing FluentAssertions 6.12.1 — the FluentAssertions id is never bumped, its 8.x line is paid-commercial) |
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
mechanism and live in `TESTING_STRATEGY.md` §4 (cited, not restated), which owns
the pins; `CensusMetaTests` (`tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs`)
asserts them and is the carrier of the live values. The pins this section states
are read from it (section 5.2, live pins); where they and the meta-test disagree,
the meta-test is right and this section is refreshed.

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
specific `reason` that states what the surface is for and what activates it: the
OPEN ledger row that owns the realization (`F-<n>`, under the section 5.2
debt-marker rule), or the section of an enrolled LOCKED or Live document whose
design it awaits (section 6.1 form). A reason never names a phase, cascade, wave,
milestone label or PR. The parameterized form is the only form; a bare
`[ReservedStub]` is not introduced.

**Closure semantics.** A site closes when the per-feature realization commit
replaces the inert surface with a real implementation and removes the attribute.
Closure is per-feature; the census is therefore **not** monotonic — new reserved
surface may legitimately raise the count. When the owning row closes, the closing
landing realizes the surface or re-homes its reason to an open row.

**Census method (verbatim).** The reserved-surface census counts `[ReservedStub`
attribute application sites in `src/**/*.cs` (rg `--type cs`), excluding the
attribute definition file
`src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs`, matching rg
pattern `\[ReservedStub`; **current pin: 34 application sites across 13 files**.
(The definition file contributes 0 under this pattern and is excluded by rule
regardless.) The pin is owned by `TESTING_STRATEGY.md` section 4.1
(reserved-surface census pin) and asserted by
`CensusMetaTests.ReservedSurfaceCensus_MatchesExactPin`, the carrier of the live
value. The registered rg method walks the filesystem; the meta-test counts
git-tracked files only, so an untracked file under `src/` moves the first and not
the second.

Compile-time enforcement: rule **DFL025_A** (behavior invocation against tagged
members requires the declaring `[Trait]`) enforces at Warning, build-breaking
under `TreatWarningsAsErrors` (`ANALYZER_RULES.md` section 1.1, rule registry).

### §5.2 — Doc-tag families

These are free-text tags in comments, XML docs and marker strings. Each is
registered with its census expression and its meaning; the pins are owned by
`TESTING_STRATEGY.md` section 4.2 (marker-family censuses) and asserted by
`CensusMetaTests`, the carrier of the live values.

| Family | Census pattern (rg, on `src/ --type cs`) | Registered meaning |
|---|---|---|
| `stub` | `rg --count-matches -i '\bstub\b' src/ --type cs` | the prose trace of a skeleton surface (section 5.1 is the structural marker) |
| `deferred` | `rg --count-matches -i '\bdeferred\b' src/ --type cs` | a deferral note -- and, because the expression counts the word, any mechanism named "deferred" (a deferred-dispatch path, a deferred-destroy flush) |
| `TODO` | `rg --count-matches '\bTODO\b' src/ --type cs` (case-sensitive) | a deferral marker; the forward form is `TODO(F-<n>)` |
| `Phase 6` | `rg --count-matches 'Phase 6' src/ --type cs` | **legacy** -- a deferral target naming a ROADMAP phase that no longer exists (the ROADMAP status overview dissolves the original Phase 6 into the Mod-OS Migration) |
| `not yet` | `rg --count-matches -i 'not yet' src/ --type cs` | a deferral note |

**Live pins** (CensusMetaTests at 54a9577): ReservedStub 34/13, stub 51/20, deferred 86/53, TODO 132/51, not yet 10/9, Phase 6 23/11, waiver disables 3, SuppressMessage 0.

Each pair is matches / files. `CensusMetaTests` is the carrier of these values;
where this line and the meta-test disagree, the meta-test is right and this line
is refreshed (PATCH, section 10.1 rule 3).

**Forward rule -- a deferral names its owner.** A deferral names the ledger row that owns the debt, in the closed form `TODO(F-<n>)`; the row must be OPEN; the landing that closes the row removes its markers. An F-# id appears in code ONLY in that form, in a `[ReservedStub]` reason, or as a waiver's retirement clause `retires with F-<n>`; a waiver's AUTHORITY is law (К-L#, PA-00#, an enrolled section), never a ledger row.

The full marker reads `TODO(F-<n>): <what remains>`: `F-<n>` is a row of the
findings ledger (`docs/ROADMAP.md`, section "Findings ledger (F-series)") and the
text states in the present tense what remains. A marker never names a phase,
cascade, wave, milestone, PR or date (section 6.2). After the landing that closes
a row, a search of the code plane for that row id returns zero. A deferral that
has no ledger row is given one or is not written.

**Legacy family** -- the census family "Phase 6" is a counted LEGACY backlog: no new site; the pin only falls, through touched-file conversions, each with its census-delta.

The census contract asserts the exact pin, not its direction, so review is what
holds "no new site".

**Legacy sites.** The existing sites of every family are a measured backlog, not
a sweep: a landing that edits a file brings every marker in that file to the
forward rule (section 6.3). A conversion moves family pins -- `TODO: Phase 6 ...`
becoming `TODO(F-<n>): ...` leaves `TODO` where it is and lowers `Phase 6` by
one -- and the move is a census delta recorded in the same commit per
`RESERVED_SURFACE_MUTABILITY.md` section 5 (how a cascade commit records a
surface revision), never reworded away.

### §5.3 — `DFK-WAIVER` — the suppression law

**Status.** This section is the standing law for **how** a DFK/DFL/DF999
diagnostic may be suppressed.

**Form.**

```csharp
// DFK-WAIVER(DFK013): <one-line reason citing law in force -- К-L#, PA-00# or an
//                     enrolled document section, or "false positive pending rule
//                     refinement, see ROADMAP.md §Analyzer track">[; retires with F-<n>]
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
   - *sanctioned architectural exception* — must cite the law that sanctions
     it: the К-L# or PA-00#, or the section of the enrolled LOCKED or Live
     document;
   - *generated or interop-mandated code* — the diagnostic does not apply to
     machine-generated or ABI-shaped surface.

   A ledger row is never a waiver's authority. A waiver whose exception is
   temporary names its retirement owner as `retires with F-<n>`: the OPEN row
   that owns the debt, under the section 5.2 debt-marker rule.
4. **No blanket suppression.** File-scope or project-scope `<NoWarn>` for any
   DFK/DFL diagnostic is **forbidden**. (The one existing root-props `NoWarn`
   entry is `CS1591` only — a compiler documentation warning, not a DFK/DFL
   diagnostic; see §4.2.)
5. **Census-tracked.** Every waiver is counted by the DFK-WAIVER census, whose
   expressions and pin are owned by `TESTING_STRATEGY.md` section 4.3
   (DFK-WAIVER census) and asserted by `CensusMetaTests.DfkWaiverCensus_MatchesPin`,
   the carrier of the live value: **current pin = 3** disables (section 5.2, live
   pins). Every increase requires a citation satisfying rule 3.
6. **No orphan waivers.** A waiver whose authority does not resolve (a К-L#,
   PA-00# or document section that does not exist, a refinement entry that does
   not exist, or a retirement row that is not OPEN) fails review.

**Diagnostic-ID form.** A waiver names the **descriptor ID string** — the
**underscore form** for sub-rules and variants (`DFK003_1`, `DFL025_A`),
identical to the file/class form (`ANALYZER_RULES.md` section 2, ID namespace
law). The
superseded dotted/hyphen forms (`DFK003.1`, `DFL025-A`) are rejected by Roslyn
`ReportDiagnostic` as invalid identifiers — a pragma naming them would suppress
nothing.

**GlobalSuppressions ban.** A `GlobalSuppressions.cs` file (assembly-level
`[assembly: SuppressMessage]`) is **forbidden** — it is the file-scope blanket of
rule 4 at assembly granularity. The ban holds: no `GlobalSuppressions.cs` exists
in the tree. The self-policing rule DF999 enforces it at Warning, build-breaking
under `TreatWarningsAsErrors` (`ANALYZER_RULES.md` section 1.1, rule registry).

## §6 — Comments & Documentation

All new artifacts — code comments, XML docs, commit bodies, governance prose —
are **English**. English is the lowest-friction shared encoding for the
agent-primary reader (PA-001) and for `git diff`, search, and string equality.

**Bi-script citation reality.** Invariant IDs appear in both scripts -- Cyrillic
"К-L" and Latin "K-L" (KERNEL_ARCHITECTURE Part 0 table rows are Latin, its
prose Cyrillic). Any census or grep over invariant citations **must match both
scripts**. Either script is acceptable in new text, but the numeric ID must be
exact (`К-L14` and `K-L14` denote the same invariant; `K-L4` is a different
one).

What a comment may say, and what it may cite, is section 6.2; how the tree
written before section 6.2 is brought to it is section 6.3.

### §6.1 — Citation form

Internal citations — from code comments, XML docs, and living documents — use
**stable identifiers and section topics**:

- an invariant, axiom or rule ID alone where one exists: `К-L13`, `PA-002`,
  `DFK022`; a living document may also cite a ledger row (`F-5`) or a decision
  record (`Q-L-10`), but a code comment cites only what section 6.2 admits;
- a document name plus section anchor *with the section's topic in words*:
  `MOD_OS_ARCHITECTURE §3.2 (capability verbs)`, `ANALYZER_RULES §1.1 (rule
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
   under restructure. Cite by section number + topic instead. The auto-generated
   `register_view_url` frontmatter line is the sanctioned exception
   (machine-maintained, regenerated with the render).

Enforcement: review-bound -- no analyzer rule detects citation form; any future
detection appears first as `Planned — see docs/ROADMAP.md §Analyzer track`.

### 6.2 -- Comment law: purpose and constraint, never provenance

CODING_STANDARDS section 6.2 (new) is the single home; other documents POINT at it and do not restate it.

Citable in a comment: К-L#, PA-00#, a section of an enrolled LOCKED or Live document, an analyzer rule id (DFK###, DFL###, DF999), a named test lock. Never: cascade/wave/pass/phase names, PR numbers, hashes, Category D documents, Q-L-# ids, Lesson #N, repair or history narratives, counts. The ban also covers a string literal that NAMES a test (display name / theory label).

- **What the code cannot express.** A comment states what the file or member is
  FOR, the constraint that shapes it, the non-obvious behaviour, and what would
  break if it were removed. Restating the code is noise; no decorative banners.
- **Law in force, or nothing.** A comment MAY cite law in force -- a reference a
  reader can open and that stays true while the law stands: a К-L# invariant
  (KERNEL_ARCHITECTURE Part 0); a PA-00# axiom (PROJECT_AXIOMS); a section of an
  enrolled LOCKED or Live document, cited in the section 6.1 form; an analyzer
  rule id (`DFK###`, `DFL###`, `DF999`, with their underscore variants); a named
  test lock (`<TestClass>.<TestMethod>` of a test that asserts the property). A
  comment that needs no citation carries none: a citation is a pointer for the
  reader, not a licence for the code.
- **Never provenance.** A comment never carries history: cascade, wave, pass or
  phase names (`W4`, `ID-B`, `A'.9.1`, `Phase 4`, `Phase 6`); pull-request
  numbers; commit hashes; references to a brief, kickoff or other Category D
  document (frozen at its closure, so the pointer dangles by construction);
  decision-record ids whose record lives in a brief (`Q-L-#`); METHODOLOGY lesson
  numbers (`Lesson #N`); ledger row ids (`F-#`) except as the debt owner in the
  forms section 5.2 admits (a `TODO(F-<n>)` marker, a `[ReservedStub]` reason, a
  waiver's `retires with F-<n>` clause); added-at, fixed-at or repaired-at
  narratives and accounts of superseded behaviour; and counts -- of rules, tests,
  sites, files or members. That history is recorded once, in the ledger, the
  closure records and git; a second copy in code goes stale the moment the next
  landing moves. The owner of a count is the register, census or lock test that
  asserts it; a comment that needs the figure cites the owner.
- **What a test CANNOT distinguish is a property of the test**, and the ban
  above does not reach it. "This used to fail until a later fix" is the
  forbidden shape. "A count over the whole file would still pass if the scan
  narrowed, so this case pins the per-file answer" is not: it names the weaker
  check the assertion must never collapse into, which a reader cannot recover
  from the assertion alone. Write it in the present tense, naming no landing and
  no prior state; if the sentence needs a date, a cascade or a repair to be
  understood, it is history and the ban holds.
- **Density.** The comment budget of a file or member is a few lines -- the
  purpose, the non-obvious behaviour, the citation. Design rationale, rejected
  alternatives and deliberation narratives belong in an enrolled document, which
  the comment cites.
- **Reach.** These rules bind comments and XML docs in C#, C++ and scripts, and a
  string literal that names a test (a display name, a theory or case label). The
  free text of a marker attribute follows its own section 5 form (section 5.1
  for a `[ReservedStub]` reason, section 5.3 for a waiver).

```csharp
// Per К-L8 the native kernel owns component storage; this managed wrapper only
// translates the int32 status. Returning the raw handle here would leak the
// ABI boundary into Application (CODING_STANDARDS section 3.1, extern "C" ABI
// boundary).
```

Enforcement: review-bound -- the review of a landing reads every comment in every
file the landing edits (section 6.3). No analyzer rule or meta-test reads comment
text for provenance; a gate for the provenance half is a ledgered candidate
(F-94). If built, it must not refuse any citation this section admits, and it
lands with a negative fixture proving, in one run, that it admits a lawful
citation and refuses a forbidden one.

### 6.3 -- Transition: the touched file

CODING_STANDARDS section 6.3 (new): there is no backlog sweep; a landing that edits a file brings every comment in that file to section 6.2; the review lens checks the touched files.

Section 6.2 binds every comment written or rewritten from its enrollment onward.
The tree enrolled before it carries a backlog, and that backlog is NOT discharged
by a sweep. The unit is the file, not the hunk: a landing that edits a file and
leaves an old-law comment in it is incomplete. A file no landing edits keeps its
comments until one does. A rewrite that moves a section 5.2 family pin records
the census delta in the same commit (`RESERVED_SURFACE_MUTABILITY.md` section 5).

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

The `<scope>` vocabulary: **`analyzer`, `build`, `chore`, `ci`, `docs`, `feat`,
`fix`, `governance`, `perf`, `refactor`, `reports`, `test`**.

Vocabulary amendment: the vocabulary gains `ci` (workflow files under `.github/workflows/`) and `build` (MSBuild/CMake build wiring); the `!` suffix marks a breaking change (`refactor(boundary)!:`); the GitHub merge subject is the one sanctioned unprefixed subject. Other post-codification out-of-vocabulary subjects are recorded exceptions.

The GitHub merge subject is the one GitHub generates for a pull-request merge
commit (`Merge pull request #<n> from <branch>`, section 8.3).

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
history is not fully in-vocabulary: the scopes `native`, `src`, `tests` and
`revert`, and one unprefixed subject, occur after the codification
(`git log --no-merges 4e584b3..main`, read at `54a9577`), and the `build` and
`ci` subjects predate their entry in the vocabulary. They are recorded
exceptions, not precedent. None of the historical forms are forbidden
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
  filter only on local, never-pushed commits. Under §8.4 a work branch is pushed
  at closure as a matter of course, so this prohibition attaches routinely:
  settle commit boundaries before the push, and correct anything after it with a
  further commit.
- **No squash** — if the work is fifteen commits, fifteen commits ship; the
  reviewer's view of the reasoning is exactly the sequence as authored.
- **A PR lands as a merge commit.** A pull request lands on `main` as a two-parent merge commit made by the operator. Squash-merge and rebase-merge are forbidden: each replaces the authored sequence with commits nobody authored. Fast-forward-only is not required. Recorded exceptions, not precedent: PRs #55 and #56 reached `main` squash-merged (`17b3719`, `3000285`); history is not rewritten to repair them.

  The merge commit is the record of the ratification act (section 8.4).

### §8.4 — The integrator opens the PR; the operator merges

Roles: the OPERATOR ratifies and merges; the ARCHITECT authors briefs and orchestrates the cascade through subagents; the INTEGRATOR (a subagent) is the only committer on the work branch, pushes it once at closure and opens the PR.

At the closure boundary the integrator **pushes its work branch to `origin` and
opens a pull request against `main`**. No agent **pushes to `main`**, and no
agent **merges a PR**. The merge is the operator's act (Crystalka) -- that is
where ratification happens. The architect authors the brief and orchestrates the
cascade; it does not merge.

**Why the branch is pushed.** The old rule made the push itself the
ratification step. That was unenforceable in a remote or cloud session, where an
unpushed branch is simply lost work — so the rule could
not hold in a case the project actually runs, and a law that cannot hold is not
a safeguard, it is an argument the agent has to have every time. The protective
intent is fully preserved and strengthened:

| Concern | Push as ratification | PR + operator merge |
|---|---|---|
| `main` mutated by an agent | prevented | prevented — pushing to `main` is still forbidden |
| Human ratification of a cascade | the push | the merge |
| Work survives a session ending | not addressed (lost in cloud sessions) | the branch is on `origin` |
| Review surface | none — a purely local branch | the PR diff, per-commit |

**Consequence — get atomicity right BEFORE pushing.** §8.3's no-force-push and
no-history-rewrite prohibitions attach the moment a branch reaches `origin`, and
under this rule that moment is routine rather than exceptional. A commit split
or re-scope is therefore permitted only while the branch has never been pushed;
after the push, a mistake is corrected by a further commit, never by rewriting.

A PR that has been opened is not a claim that the work is finished to the
operator's satisfaction — it is the handoff. Anything the integrator could not
verify itself (a live smoke, a visual check, hardware-dependent behaviour) is
stated in the PR body as owed, not implied to be done.

## §9 — Verification Gates

These are the gates that **exist**. Each names its artifact. Invocation forms
and host specifics live in `DEVELOPMENT_HYGIENE.md` section 3 (build commands)
and section 4 (tooling reality); test law and the test invocation live in
`TESTING_STRATEGY.md` section 8 (test invocation truth). Cited, not restated.

- **Managed build.** `dotnet build DualFrontier.sln -c Release`. Compile-clean is
  expected; `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
  (`Directory.Build.props`) makes any warning a failure. The host hazards of a
  build or test run (MSBuild node reuse, a test host lingering in shutdown) are
  handled per `DEVELOPMENT_HYGIENE.md` section 6 (environment notes) and
  `TESTING_STRATEGY.md` section 8.
- **Native build.** The CMake tree `native/DualFrontier.Core.Native/build`
  (Ninja, Release), configured and built per `DEVELOPMENT_HYGIENE.md` section 3;
  built before the managed solution, whose tests load the library it produces.
- **Native selftest.** `native/DualFrontier.Core.Native/build/df_native_selftest`
  (built from `native/DualFrontier.Core.Native/test/selftest.cpp`).
- **Solution tests.** Run per `TESTING_STRATEGY.md` (the test-law authority);
  this document does not restate the test layer taxonomy or run command.
- **Register validation.**
  `dotnet run --project tools/DualFrontier.Governance -- validate --armed`,
  exit 0. `validate` writes nothing. `sync` regenerates the
  derived `REGISTER.yaml` and `CURRENT_AUTHORITY_SURFACE.yaml` from document
  frontmatter and runs in every commit that changes a frontmatter block
  (`FRAMEWORK.md` section 14.1); derived files are never hand-edited.

**Analyzer enforcement: live.** The DualFrontier.Analyzers project is wired to
all twelve src projects (section 4.2) and ships **18 rules** (9 Architecture /
3 Discipline / 6 NativeBoundary) at the severity each descriptor declares: 12
Error + 5 Warning -- both build-breaking under `TreatWarningsAsErrors` -- plus
DFL025_B at descriptor Info (`.editorconfig` `suggestion`, IDE-only).
`AnalyzerReleases.Shipped.md` and `AnalyzerReleases.Unshipped.md` record the
releases, RS release tracking cross-checks them against the descriptors, and the
root `.editorconfig` restates all 18 severities as
`dotnet_diagnostic.<ID>.severity` keys. A DFK/DFL/DF999 violation therefore
fails the build; suppression only per the section 5.3 DFK-WAIVER law (census
pin: 3). The rule inventory authority is `ANALYZER_RULES.md` section 1.1 (rule
registry).

**CI truth:** the repository's only workflow is `.github/workflows/gitleaks.yml` (secret scan); no CI job runs the build, the suite or `validate --armed` -- the ledger row F-92 owns that gap. Acceptance is the local chain.

The Gitleaks workflow scans the full reachable history on push, pull request and
manual dispatch (`docs/SECRET_SCANNING.md`). The local chain is the gates above,
run before the PR is opened.

## §10 — Amendment Protocol & Change History

### §10.1 — Amendment protocol

This document is Tier 1 LOCKED. An amendment proceeds:

1. **Surface** — propose the change to the owner (Crystalka) as an explicit
   direction; the agent does not default-amend a LOCKED standing law.
2. **Rationale** — document what changes, why, and what it deprecates.
3. **Semver** — PATCH for clarification/correction, MINOR for a new rule or
   family, MAJOR for inverting an existing rule.
4. **REGISTER update** — bump `version`, `last_modified` and `next_review_due`
   in this document's frontmatter, which is the source of truth (`FRAMEWORK.md`
   section 14.1); run `sync` in the same commit and `validate --armed` green
   (section 9). The change rides a `governance(...)` commit (section 8).
5. **Cross-doc propagation** — when an amendment touches a rule cited elsewhere,
   update those citations in the same cascade. Documents that cite this one:
   `TESTING_STRATEGY.md` (census contracts and the waiver citation rule,
   sections 5 and 5.3), `DEVELOPMENT_HYGIENE.md` (build and validate gates,
   section 9; push and merge policy, sections 8.3 and 8.4), `ANALYZER_RULES.md`
   (defers the suppression law to section 5.3 and restates its reason classes),
   `RESERVED_SURFACE_MUTABILITY.md` (the `Skeleton revisions` body law, section
   8.2; marker-family semantics, section 5; atomic-commit discipline, section 8),
   `METHODOLOGY.md` (commit-discipline and brief-integration rules),
   `BRIEF_TEMPLATE.md` (push law, section 8.4; no-squash and merge form, section
   8.3), and the architecture documents that cite section 6.1 (citation form).

An amendment that contradicts a brief does not lose: a brief contradicting this
standing law is wrong by default — correct the brief, or ratify the amendment
here **before** the brief locks.

### §10.2 — Change history

| Version | Date | Change |
|---|---|---|
| **4.0.0** | 2026-11-21 | Operator rulings 2026-11-21. **Section 6 comment law INVERTED** (new section 6.2): a comment states what the code cannot express and may cite only law in force (К-L#, PA-00#, a section of an enrolled LOCKED or Live document, an analyzer rule id, a named test lock); provenance is forbidden -- cascade, wave, pass and phase names, PR numbers, hashes, Category D documents, Q-L-# ids, Lesson #N, ledger row ids outside a debt pointer, repair and history narratives, counts; the ban reaches a string literal that names a test; what a test cannot distinguish stays, in the present tense. Section 6 and 6.1 no longer require F-#, Q-L-# or Lesson #N citations; the 6.1 citation form stands. Transition by touched file, no backlog sweep (new section 6.3). **Sections 5.1-5.3 reconciled**: a deferral names the OPEN ledger row that owns the debt in the closed form `TODO(F-<n>)`, and the landing that closes the row removes its markers; an F-# id appears in code only in that form, in a `[ReservedStub]` reason or as a waiver's `retires with F-<n>` clause; a waiver's authority is law, never a ledger row; `Phase 6` re-registered as a counted legacy backlog; the restated `deferred` (82/51 -> 86/53), `TODO` (136/53 -> 132/51) and waiver (2 -> 3) pins corrected to the live `CensusMetaTests` values, which the document now names as their carrier; the section 5.3 supersession paragraph about a Category D brief deleted; the section 5.3 cross-reference to ANALYZER_RULES corrected to section 2. **Section 8.1** `ci` and `build` join the vocabulary, the `!` breaking-change suffix and the GitHub merge subject are named; the "100% in-vocabulary" claim corrected. **Section 8.3** a PR lands as a two-parent merge commit made by the operator; squash-merge and rebase-merge forbidden; fast-forward-only not required; PRs #55/#56 recorded as exceptions. **Section 8.4** roles: the operator ratifies and merges, the architect authors and orchestrates, the integrator commits, pushes once and opens the PR. **Section 9** Linux native build and selftest paths, the .NET governance tool in place of the retired PowerShell suite, 18 rules (12 Error / 5 Warning / 1 Info), Gitleaks as the only CI (F-92). Also: the section 3.1 example rewritten to section 6.2; the section 4.2 shader-target host condition and SDK roll-forward stated truly; the section 6 example cites К-L8 (component lifetime), not К-L1. Correction to the 3.0.0 row: its METHODOLOGY propagation landed in section 2.3 (verification cycle), not section 4.4. **MAJOR** per section 10.1 rule 3 (inverting existing rules). |
| **3.0.0** | 2026-08-20 | **§8.4 INVERTED — "Executor never pushes" → "Executor opens the PR; the architect merges."** The executor now pushes its work branch and opens a PR against `main` at the closure boundary; pushing to `main` and merging its own PR stay forbidden, and ratification moves from the push to the merge. Operator direction 2026-08-20: the old rule could not hold in a remote/cloud session, where an unpushed branch is lost work — a law that fails in a case the project actually runs is noise that forces the agent into an argument rather than a safeguard. `main` stays untouched by an agent, human ratification is preserved, and the PR adds a per-commit review surface a purely local branch never had. §8.4 also records the consequence that §8.3's no-rewrite prohibitions now attach routinely, so commit atomicity must be settled BEFORE the push. Propagated to `DEVELOPMENT_HYGIENE.md` §5 (2.1.0) and `METHODOLOGY.md` §4.4 (1.14.3) in the same commit per §10.1 rule 5. **MAJOR** per §10.1 rule 3 (inverting an existing rule). |
| **2.1.4** | 2026-07-17 | STACK_UPDATE Phase H doc census (EVT-2026-07-17-STACK_UPDATE) — toolchain facts to the landed state: §1 scope bullet + §3 heading C++20 → C++23 (К-L1 amended, KERNEL_ARCHITECTURE v1.1.0); §4.2 props hand-mirror net8.0 → net10.0, LangVersion 12.0 → 14.0, plus the new repo-root `global.json` SDK pin stated (`10.0.204`, `rollForward: latestFeature`); §4.3 CPM hand-mirror rows Microsoft.NET.Test.Sdk 17.11.1 → 18.8.1, xunit 2.9.2 → 2.9.3, xunit.runner.visualstudio 2.8.2 → 3.1.5, FluentAssertions 6.12.1 → AwesomeAssertions 9.4.0 (Apache-2.0 community fork; the FluentAssertions id is never used again — its 8.x line is paid-commercial). Hand-mirror refresh only; no rule change. **PATCH.** |
| **2.1.3** | 2026-07-15 | CODEX_CLOSURE cascade census-pin refresh (§5.2): `not yet` 8/7 → 10/9 — the CX-06 + CX-21 fix-comment markers landed at Codex `61f08ef` without a same-commit census-delta; recorded here + as ROADMAP F-33 (operator-ruled FOLD of the Skarlet-gate H2b). CensusMetaTests green post-refresh. **PATCH.** |
| **2.1.2** | 2026-07-02 | A'.9.1 Phase δ rider — commit-vocabulary historical reconciliation: §8.1 historical-prefix note extended with the full pre-law observed scope-token census (16 tokens over 53 commits, all predating the 2026-06-11 codification) + the unprefixed-era note (81 subjects: 42 merges + 39 plain). Descriptive correction of the incomplete observation — no normative change; post-codification history is 100% in-vocabulary (no live violation). **PATCH.** |
| **2.1.1** | 2026-07-01 | A'.9.1 Phase γ propagation PATCH (METHODOLOGY §12.7 step 9 / SYNTH-2 + §10.1 rule 5 cross-doc propagation): §9 «Analyzer enforcement: none today» → live Release 1.0 enforcement (16 build-breaking + 1 IDE-only; `.editorconfig` primed; `Shipped.md` populated); §5.1 + §5.3 DFL025_A / DF999 «Planned»-stub claims → detecting-since-β, enforcing-since-γ; §5.3 Diagnostic-ID form → underscore per the Phase β Crystalka-ratified descriptor-ID adjudication (the dotted/hyphen forms are Roslyn-invalid — the paragraph as written would have produced no-op waivers); §5.3 rule 5 waiver census → current pin 2 (dated); §5.2 `stub`/`deferred` baselines 48/18 → 51/20, 79/48 → 82/51 (the F-25 owed fold; live values carried by the compiled meta-tests since Phase β); stale v2.0.0 end-marker synced. **PATCH.** |
| **2.1.0** | 2026-06-12 | §6.1 citation-form rule added (Architecture Truth Cascade, D11): internal citations use stable identifiers + section topics; version pins of living documents and URL-fragment anchors forbidden in living prose and code comments (pins live only inside dated records). Codifies the two breakage classes the 2026-06-11 ARCHITECTURE TRUTH RECON surfaced, in the same cascade that fixed the found instances (SYNTH-2). **MINOR.** |
| **2.0.0** | 2026-06-11 | Full rewrite to code-truth: marker-family registry (§5: `[ReservedStub]`, doc-tag families, the new `DFK-WAIVER` suppression law superseding `A_PRIME_9_1_…BRIEF` §7.3), C++ ABI-boundary discipline (§3), build-config quoted verbatim (§4), enforcement claims reduced to existing artifacts (§9 — analyzer stated as 17 non-detecting stubs; CI stated absent). **MAJOR.** Authored per `tools/briefs/STANDING_LAW_CASCADE_BRIEF.md`. |
| 1.0 | 2026-05-12 (era) | Initial codification — naming, file-scoped namespaces, nullability, one-class-per-file, member order, stack-frame retention discipline. Preserved as historical; superseded by 2.0.0. |

---

**End of CODING_STANDARDS.md**