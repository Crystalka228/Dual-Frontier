---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ANALYZER_RULES
category: A
tier: 1
lifecycle: AUTHORED-SKELETON
owner: Crystalka
version: "0.4.1"
next_review_due: 2027-05-23
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ANALYZER_RULES
---
# DualFrontier Roslyn Analyzer Rule Specifications

**Lifecycle**: AUTHORED-SKELETON (initial entry at A'.8 closure 2026-05-23; structural reorganization at A'.9.1 Phase α Commit 5 2026-05-25; Standing-Law restructure 2026-06-11; Phase β detection + Phase γ severity promotion 2026-07-01 — lifecycle unchanged: LOCK criteria per ROADMAP «Analyzer track» Phase δ)
**Version**: 0.4.0
**Date created**: 2026-05-23 (А'.8 К-series formal closure cascade Commit 5 REGISTER enrollment)
**Date restructured**: 2026-05-25 (A'.9.1 / К-extensions cascade #5 Phase α Commit 5); 2026-06-11 (Standing-Law cascade — roadmap-load extraction + stub-truth correction per [DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md §6.3.2](../reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md))
**Purpose**: Roslyn analyzer rule specifications encoding К-Lxx invariants. This document states what exists; scheduled futures live in [ROADMAP.md «Analyzer track»](../ROADMAP.md). Populated к LOCKED at A'.9.1+ Roslyn analyzer milestone implementation cascade.

**Authority**: This document specifies analyzer rules. Canonical rule narratives + detection patterns + diagnostic messages reside в [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications). К-Lxx invariant authority resides в [KERNEL_ARCHITECTURE.md Part 0](KERNEL_ARCHITECTURE.md#part-0-locked-foundational-decisions). PA-001..PA-004 axiom anchors reside в [PROJECT_AXIOMS.md](../governance/PROJECT_AXIOMS.md) (introduced A'.9.1 Phase α Commit 8).

**Forward sequencing**: relocated — see [ROADMAP.md «Analyzer track»](../ROADMAP.md) for all analyzer sequencing (A'.9.1 Phase β/γ/δ) and the deferred rule families (К-L20 LOCK cascade, hardware tier expansion).

---

## §1 — Document framing + authority chain

This document is the authoritative specification for the DualFrontier Roslyn analyzer rule surface: rule IDs, namespaces, categories, the per-rule template, and the realized scope decisions. What the rules **do today** is stated in §4 (ground truth: 17 detecting rules enforced at shipped severities — 16 build-breaking under `TreatWarningsAsErrors`, 1 IDE-only). What the rules **are scheduled to become** is owned by [ROADMAP.md «Analyzer track»](../ROADMAP.md) — spec/roadmap separation per Standing-Law cascade 2026-06-11. §7 and §8 are **realized decision records** (closed Q-L outcomes), not forward plans.

**Authority chain**:
- [KERNEL_ARCHITECTURE.md Part 0](KERNEL_ARCHITECTURE.md#part-0-locked-foundational-decisions) — К-Lxx invariant canonical text (Tier 1 LOCKED)
- [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications) — canonical detection narratives for each rule (Tier 1 LOCKED)
- [PROJECT_AXIOMS.md](../governance/PROJECT_AXIOMS.md) — PA-001..PA-004 axiom anchors driving rule scope decisions (Tier 1 LOCKED, NEW A'.9.1)
- **This document** — analyzer rule specifications: IDs, namespaces, categories, templates, realized decision records (Tier 1, lifecycle AUTHORED-SKELETON; forward lifecycle sequencing in [ROADMAP.md «Analyzer track»](../ROADMAP.md))

**Q-L ratifications history** (forward-locked at Brief A'.9.1 batch 1 + 2 deliberation 2026-05-24):
- Q-L-3 — Tiered namespaces (DFK###/DFL###/DFC###) per §3
- Q-L-7 — 3 categories (Architecture/NativeBoundary/Discipline) + ModSurface reserved per §4
- Q-L-8 — DFK019 split (DFK019.A ships A'.9.1; DFK019.B deferred к hardware tier expansion cascade) per §6
- Q-L-9 — DFK010 PERMANENTLY DROPPED (methodology-layer; PA-002 anchor) per §7
- Q-L-11 — DFC001 deferred к К-L20 LOCK cascade (Bridge surface Mod-API-coupled) per §5
- Q-L-12 — BannedApiAnalyzer DROPPED entirely (Godot closed concern) per §8
- Q-L-13 — PublicApiAnalyzers DEFERRED audience-driven (PA-001 anchor) per §8
- Q-L-14 — DF→DFK rename two-commit (mechanical Commit 4 + structural Commit 5)
- Q-L-15 — Code-fix providers PERMANENTLY DROPPED (PA-001 anchor) per §8
- Q-L-16 — DFK016 Phase 0 → retain α (managed surface stable per A'.9.1 Phase 0 closure §3.3) per §4

---

## §2 — Per-rule specification template

**Per-rule specification format** (entries populated into §10 as each rule's Phase β implementation lands):

```markdown
### DFK### — <К-L invariant title>

**К-L invariant**: <К-L# reference + canonical text link к KERNEL_ARCHITECTURE.md Part 0>
**Category**: DualFrontier.Architecture / DualFrontier.NativeBoundary / DualFrontier.Discipline
**Severity (shipped, Release 1.0)**: Error / Warning / Suggestion (= descriptor Info, IDE-only) — per the §4.1 registry
**Status**: Stub / Detecting / Reserved / Outside-Roslyn-scope
**Implementation status**: Pending A'.9.1 Phase β / Implemented at A'.9.1 / Active post-A'.9.1

**Detection narrative** (3-5 sentences per Q5.3 LOCKED Session 1):
<What patterns the rule detects. Reference K_CLOSURE_REPORT.md §7.2 для canonical detection narrative.>

**Diagnostic message text**:
> «<diagnostic message string>»

**Example violation patterns**:
- <code pattern 1 that triggers diagnostic>
- <code pattern 2 that triggers diagnostic>

**Suppression policy**:
- Governed by CODING_STANDARDS.md §5.3 «DFK-WAIVER — the suppression law» (see §12) — cite the law, do not restate syntax
- When suppression appropriate: <rare cases с justification per the DFK-WAIVER form>

**Test cases** (positive + negative examples):
- Positive: <code pattern that должно triggered diagnostic>
- Negative: <code pattern that должно NOT trigger diagnostic>

**Cross-references**:
- К-L canonical text: KERNEL_ARCHITECTURE.md Part 0 К-L# row
- К-L falsifiability commitment: K_CLOSURE_REPORT.md §2.# entry
- Related Lessons: <list>
- Related PA axioms: <list>
```

---

## §3 — Tiered namespaces convention per Q-L-3

Three-tier rule ID namespace convention forward-locked at Brief A'.9.1 batch 1 deliberation 2026-05-24:

- **DFK###** — К-Lxx invariant rules (architectural). Each rule encodes a specific К-Lxx kernel invariant from [KERNEL_ARCHITECTURE.md Part 0](KERNEL_ARCHITECTURE.md#part-0-locked-foundational-decisions). Numbering aligned 1:1 with К-L# (e.g., DFK001 encodes К-L1).
- **DFL###** — Lesson-derived rules (discipline). Each rule encodes a specific [METHODOLOGY.md](../methodology/METHODOLOGY.md) Lesson. Numbering follows Lesson number (e.g., DFL025 family encodes Lesson #25 refined extension).
- **DFC###** — Cascade-specific rules (drift detection). Reserved для rules surfaced by individual cascade execution to detect specific drift patterns. No DFC### stub exists at A'.9.1 per Q-L-11 (DFC001 deferred к К-L20 LOCK cascade — see §5 pointer).

**Self-policing**: DF999 (analyzer rule that bans solution-wide GlobalSuppressions / `[assembly: SuppressMessage]` per Q-L-18 default — pending batch 3 governance ratification). Shipped as a non-detecting stub at A'.9.1 Phase β-prep per the default; detecting since Phase β, enforcing at Warning since Phase γ (Release 1.0).

---

## §4 — A'.9.1 first-batch rule registry (17 rules — enforced at shipped severities)

**Ground truth (v0.4.0, verified on disk 2026-07-01):** all 17 first-batch rules carry real detection logic (landed at A'.9.1 Phase β) and are **enforced at their shipped severities** (promoted at A'.9.1 Phase γ; F-12 ratified 2026-07-01). The three severity surfaces agree and are structurally locked: the descriptors declare the shipped `defaultSeverity` (11 Error + 5 Warning + DFL025_B Info); the root `.editorconfig` restates the same values as `dotnet_diagnostic.<ID>.severity` keys (no override semantics — a key differing from its descriptor is a defect); `AnalyzerReleases.Shipped.md` records them under Release 1.0, cross-checked against the descriptors by RS release tracking. The enforcer: the analyzer loads at every build of the 12 wired src projects (§4.3) and `TreatWarningsAsErrors=true` (root `Directory.Build.props`) makes Error AND Warning diagnostics fail the build — **16 rules are build-breaking; DFL025_B (descriptor Info ≡ editorconfig `suggestion`) is IDE-only**. Suppression only per the DFK-WAIVER law (§12); current census: 2 waivers (both DFK001, К-L19-sanctioned Vulkan debug-messenger interop).

On-disk layout (9 Architecture + 3 Discipline + 5 NativeBoundary):

```
tools/DualFrontier.Analyzers/Rules/
├── Architecture/    DFK003StorageOwnershipAnalyzer · DFK003_1StorageBridgeAnalyzer ·
│                    DFK004TypeIdRegistryAnalyzer · DFK005DeclarativeBootstrapAnalyzer ·
│                    DFK007SpanProtocolAnalyzer · DFK011NativeWorldSsotAnalyzer ·
│                    DFK013WakeTypeDisciplineAnalyzer · DFK016PipelineDepthAnalyzer ·
│                    DFK017DisplayCompositionAnalyzer
├── Discipline/      DF999GlobalSuppressionBanAnalyzer · DFL025_AReservedStubInvocationAnalyzer ·
│                    DFL025_BStandaloneSkipAnalyzer
└── NativeBoundary/  DFK001NativeLanguageAnalyzer · DFK002PInvokeBindingsAnalyzer ·
                     DFK007_1GpuPipelineSlotAnalyzer · DFK015_1ThreeTierMutexFacadeAnalyzer ·
                     DFK019_AStaticVulkanApiAnalyzer
```

**Naming convention** (stated once; amended 2026-07-01 — descriptor-ID adjudication): descriptor ID strings, file names, and class names **all use underscores** for sub-rules and variants (`DFK003_1`, `DFK007_1`, `DFK015_1`, `DFK019_A`, `DFL025_A`, `DFL025_B`; class `DFK003_1StorageBridgeAnalyzer`). A **dotted or hyphenated diagnostic ID is rejected by Roslyn `ReportDiagnostic` as an invalid identifier** (Phase β empirical finding) and would also break the `.editorconfig` `dotnet_diagnostic.<id>.severity` key form; the pre-2026-07-01 dotted/hyphenated forms (still visible in the recon and the Phase β brief) are the **superseded** convention. `DFL025-C` is a shell-level filter category, not a Roslyn descriptor id, and is unaffected.

**Categories per Q-L-7** (3 in use by shipped stub descriptors + 1 reserved):
- `DualFrontier.Architecture` — DFK### kernel architectural invariants
- `DualFrontier.NativeBoundary` — DFK### native interop discipline (P/Invoke surface, GPU pipeline slot)
- `DualFrontier.Discipline` — DFL### Lesson-derived discipline (NEW category at A'.9.1)
- `DualFrontier.ModSurface` — RESERVED (no stub carries it; scheduled for the К-L20 LOCK cascade — see §5 pointer)

### §4.1 — Registry (single source — 17 rules)

| Rule ID | К-L / Lesson anchor | Tier | Category | Shipped severity (Release 1.0)¹ | Status |
|---|---|---|---|---|---|
| DFK001 | К-L1 | P0 | NativeBoundary | Error | enforcing |
| DFK002 | К-L2 | P0 | NativeBoundary | Error | enforcing |
| DFK003 | К-L3 | P0 | Architecture | Error | enforcing |
| DFK003_1 | К-L3.1 | P0 | Architecture | Error | enforcing |
| DFK004 | К-L4 | P0 | Architecture | Error | enforcing |
| DFK005 | К-L5 | P0 | Architecture | Error | enforcing |
| DFK007 | К-L7 | P0 | Architecture | Error | enforcing |
| DFK011 | К-L11 | P0 | Architecture | Error | enforcing |
| DFK007_1 | К-L7.1 | P1 | NativeBoundary (GPU pipeline slot) | Error | enforcing |
| DFK015_1 | К-L15.1 | P1 | NativeBoundary (three-tier mutex managed facade) | Error | enforcing |
| DFK017 | К-L17 | P1 | Architecture (display composition multi-layer) | Error | enforcing |
| DFK019_A | К-L19 static API surface | P1 | NativeBoundary | Warning² | enforcing |
| DFK013 | К-L13 | β-secondary | Architecture | Warning | enforcing |
| DFK016 | К-L16 (retain α — Phase 0 ratified per Q-L-16) | β-secondary | Architecture | Warning | enforcing |
| DFL025_A | Lesson #25 refined 3rd extension | Discipline | Discipline | Warning | enforcing |
| DFL025_B | Lesson #25 refined 3rd extension | Discipline | Discipline | Suggestion (descriptor Info) | detecting (IDE-only) |
| DF999 | analyzer self-discipline (Q-L-18 default) | Self-policing | Discipline | Warning | enforcing |

¹ Shipped at A'.9.1 Phase γ (2026-07-01): the pre-promotion registry carried a «Phase-γ target» column beside a uniform Info shipped column; Phase γ executed the targets and collapsed the pair into this single live column. «Enforcing» = build-breaking under `TreatWarningsAsErrors` (Error and Warning both fail the build). DFL025_B ships descriptor `Info` (`DiagnosticSeverity` has no Suggestion member), restated as `.editorconfig` `suggestion` — IDE-only. Promotion history: [ROADMAP.md «Analyzer track»](../ROADMAP.md).

² Registry-drift resolution (v0.2.0): v0.1 carried two parallel registries (§4.1–4.5 and §10.1–10.3) whose columns drifted — §4.2 grouped DFK019_A under an «Error severity post-promotion» header while §10.1 listed Warning. Resolved to **Warning** per canonical [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications) DFK019 row (Warning, V substrate contract — configurable). v0.2.0 keeps exactly one registry — this table.

**Detection notes** (implemented at Phase β; canonical narratives: K_CLOSURE_REPORT.md §7.2):

- **DFK013** — on-demand activation discipline (efficiency-not-correctness): `SystemBase` subclass без `[WakeOn*]` AND без `[TickRate]`, OR eager-init в `Initialize()`.
- **DFK016** — pipeline depth D ∈ {1, 2, 3} hardcoded vs `PipelineSlotInterop.DefaultDepth` / `.MaxDepth` constants. Phase 0 ratified retain α (managed surface stable per closure report §3.3).
- **DFL025_A** — test class invoking `[ReservedStub]`-tagged type MUST carry `[Trait("Category", "ReservedStub")]`; T2 syntax tree + SemanticModel attribute check via fully-qualified name match.
- **DFL025_B** — standalone test methods против reserved-stub modules SHOULD use `[Fact(Skip="...")]`; edge-case discipline.
- **DF999** — bans solution-wide `GlobalSuppressions.cs` / `[assembly: SuppressMessage]` (Q-L-18 default, pending batch 3 governance ratification); forces per-site suppression discipline (Q-L-20).
- **DFL025-C** — closure-protocol shell-level rule (`--filter "Category!=ReservedStub"` в `dotnet test` invocation) — **NOT a Roslyn analyzer**; deferred к governance tooling cascade per amendments §4.5. Listed here as a scope exclusion only.

### §4.2 — Composition + S-LOCK-4 count audit (trimmed)

Composition reconciled to on-disk truth: **8 P0 + 4 P1 + DFK013 + DFK016 (retain α — Phase 0 ratified) + DFL025_A/B + DF999 = 17 rules.**

**S-LOCK-4 count audit (essentials)**: Brief §6.5 narrative stated «Total A'.9.1 enforcement surface: 15-16 own rules» — arithmetic carryover from pre-Q-L-9 / pre-Phase-0 drafting (DFK010 still counted as P1; DFK016 still a Phase-0 conditional). Post-supersession arithmetic = 17, matching the 17 files on disk. Surfaced for forward-cascade audit trail. The v0.1 phrase «enforcement surface» was an overclaim at v0.1–v0.2.x (stub surface, detection pending); since Phase β detection and the Phase γ promotion (v0.4.0), **enforcement surface** is the accurate, empirical term.

### §4.3 — Infrastructure ground truth

- **Wiring**: `src/Directory.Build.props` adds `tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj` as a `ProjectReference` with `OutputItemType="Analyzer"` to all 12 `src/` projects — the analyzer assembly loads at every build; since Phase β it detects, since Phase γ it enforces (16 build-breaking rules under `TreatWarningsAsErrors`).
- **Release tracking**: `AnalyzerReleases.Shipped.md` carries Release 1.0 — all 17 rules at their shipped severities (transitioned at Phase γ); `AnalyzerReleases.Unshipped.md` is empty-with-header. RS release tracking cross-checks descriptor ↔ Shipped-table severity mechanically; any future severity change requires the «Changed Rules» ceremony in `Shipped.md`, making severity drift structurally visible.
- **Tests**: `tests/DualFrontier.Analyzers.Tests` carries the Phase β suite — 54 tests (≥1 positive + ≥1 negative per detecting rule + harness + census meta-tests), severity-transparent (expected severities derive from the descriptors).

---

## §5 — К-L20 LOCK cascade deferred family — relocated

Planned — see [ROADMAP.md «Analyzer track»](../ROADMAP.md), subsection «К-L20 LOCK cascade rule family».

> **Relocated 2026-06-11 (Standing-Law cascade spec/roadmap separation).** The deferral
> table — DFK009 / DFK012 / DFK015 / DFK018 + DFK020 family (~20 sub-rules) + DFC001.A /
> DFC001.B, with per-rule deferral rationales (PA-002 «без костылей» anchor, Q-L-11) and
> the activation note — moved to ROADMAP.md. The §1 Q-L references («per §5») resolve
> through this pointer.

**`MOD_API_CONTRACT.md` — not yet authored; lands with the К-L20 LOCK cascade — see [ROADMAP.md «Analyzer track»](../ROADMAP.md).** Pre-v0.2.0 text in this family's orbit (K_CLOSURE-derived «К-L20 Mod API contract» wording) read as if a contract document existed; no such document exists in the repository. Every reference to it is a forward reference until that cascade authors it (per K_CLOSURE §2.23 + §9.5 Q8).

---

## §6 — Hardware tier expansion cascade deferred — relocated

Planned — see [ROADMAP.md «Analyzer track»](../ROADMAP.md), subsection «Hardware tier expansion cascade». The deferral table — DFK019.B (hardware tier capability runtime check) + DFK016 threshold customization API, with rationales and the audience-driven activation trigger (Q-L-8 split, Lesson #N17 Provisional) — moved there 2026-06-11.

---

## §7 — Outside Roslyn scope — decision record (realized)

**Framing**: this section is a **closed decision record**, not a forward plan. Per K_CLOSURE §7.3 + Brief A'.9.1 Q-L-9 ratification, the rules below are settled as requiring alternative (non-Roslyn) enforcement mechanisms (PA-002 anchor «без костылей» — Roslyn pretending к enforce methodology-layer = kostyl). No Roslyn implementation is scheduled for any of them at any phase.

| Rule | К-L | Reason outside Roslyn scope | Designated alternative mechanism |
|---|---|---|---|
| DFK006 | К-L6 SUPERSEDED | К-L6 displaced by К-L12 native scheduler sovereignty (К10 cascade) — historical traceability only; never activates | Reserved historical entry; no enforcement |
| DFK008 | К-L8 component lifetime | Process-invariant (storage path discipline); pre-commit hook OR runtime probe more appropriate | Git pre-commit hook (designated per K_CLOSURE §7.3) |
| DFK010 | К-L10 decision rule | **PERMANENTLY DROPPED per Q-L-9 + PA-002 axiom**. К-L10 governs decision attribution at document/methodology layer, NOT code layer. Code-layer marker masquerading doc-layer reasoning = kostyl pattern. | FRAMEWORK + METHODOLOGY documentation discipline only |
| DFK014 | К-L14 meta-invariant | Meta-invariant tracking «defect rate / architectural integrity / pipeline economics» — not pattern detectable in syntax tree | [K_L14_EVIDENCE_DASHBOARD.md](K_L14_EVIDENCE_DASHBOARD.md) cumulative evidence tracking |

**S-LOCK-10 invariant**: DFK010 has NO implementation file. Verify ANALYZER_RULES.md §4 P1 = 4 rules (NOT 5 per amendments §3.3 pre-supersession list).

---

## §8 — Dropped / deferred tooling — decision record (realized)

**Framing**: closed Q-L outcomes (Q-L-12 / Q-L-13 / Q-L-15 — Brief A'.9.1 batch 2 deliberation 2026-05-24) — decision records, not forward work. PA-001 axiom anchor: current audience profile = AI agents permanently; community ecosystem absent.

### §8.1 — PublicApiAnalyzers (RS0016/RS0017/RS0024) — Q-L-13 + PA-001 — DEFERRED

**Status**: DEFERRED entirely. Not in `Directory.Packages.props`.

**Rationale**: Community ecosystem absent (PA-001 anchor). All candidate assemblies (Contracts/Application/Bridge/Launcher) = Mod API surface volatile pre-К-L20 LOCK OR not mod-facing.

**Activation conditions**: relocated to [ROADMAP.md «Analyzer track»](../ROADMAP.md), subsection «PublicApiAnalyzers deferral — activation conditions». The deferral decision itself is closed here; only the re-activation trigger list lives on the roadmap.

### §8.2 — Code-fix providers — Q-L-15 + PA-001 PERMANENT

**Status**: DROPPED PERMANENTLY. Not in `Directory.Packages.props`. No `CodeFixProvider` subclasses.

**Rationale**: PA-001 axiom — AI-agent-first consumer profile permanent. Code-fix providers serve human IDE workflow; AI agent reads diagnostic text directly and applies edits via Edit tool. Diagnostic message quality elevated к compensate (rich text guiding AI agent к edit per §2 template «Detection narrative»).

**Activation conditions**: NONE (PERMANENT). Audience materialization (community emergence) would re-trigger Q-L deliberation — at that point, PROJECT_AXIOMS.md PA-001 amendment via FRAMEWORK §7.2 Tier 1 LOCKED amendment protocol required FIRST.

### §8.3 — BannedApiAnalyzer for Godot/Silk.NET — Q-L-12

**Status**: DROPPED entirely. Not in `Directory.Packages.props`. No `BannedSymbols.txt`.

**Rationale**: Closed historical concern. Godot permanently removed К-extensions cascade #2. No external re-introduction risk audience exists. Documentation discipline (DualFrontier.Contracts README) sufficient.

**Activation conditions**: NONE. Re-introduction risk would require new audience surface — Crystalka direction surface.

---

## §9 — Reserved namespaces

- **DFC###** namespace — RESERVED at A'.9.1 (no DFC### stub exists; scheduled for the К-L20 LOCK cascade per Q-L-11 DFC001 deferral — see §5 pointer). Forward designation для cascade-specific drift detection rules.
- **DFL###** namespace — in use at A'.9.1 (DFL025_A + DFL025_B shipped as non-detecting stubs at Phase α/β-prep; detection landed at Phase β; enforced since Phase γ Release 1.0 — DFL025_A at Warning, DFL025_B descriptor Info ≡ `.editorconfig` suggestion, IDE-only; future DFL### rules added как additional Lessons materialize forward).
- **DualFrontier.ModSurface** category — RESERVED at A'.9.1 (no stub carries it; scheduled for the К-L20 LOCK cascade per Q-L-11 + §5 pointer).

---

## §10 — Per-rule detail specifications (populated as Phase β lands)

Landing zone for per-rule §2-template entries. **Empty at v0.4.0** — the §4.1 registry is the single live surface; per-rule §2-template population was deferred through Phase β/γ and is a Phase δ+ item (see [ROADMAP.md «Analyzer track»](../ROADMAP.md)).

**Registry consolidation (v0.2.0)**: the v0.1 §10.1–§10.3 tables restated the §4 registry a second time with an independent Status column («Active (pending A'.9.1 Phase β impl)») that overstated ground truth and drifted from §4 (see §4.1 footnote ²). One registry now exists — §4.1.

### §10.4 — Deferred rules summary (cross-reference)

Compact cross-reference only — rationales live at the pointed-to locations.

| Rule | К-L / scope | Disposition | Where |
|---|---|---|---|
| DFK006 | К-L6 SUPERSEDED | Never activates (historical traceability) | §7 |
| DFK008 | К-L8 process invariant | Outside Roslyn scope (pre-commit hook designated) | §7 |
| DFK009 | К-L9 Vanilla=mods | Deferred к К-L20 LOCK cascade | [ROADMAP.md «Analyzer track»](../ROADMAP.md) |
| DFK010 | К-L10 decision rule | PERMANENTLY DROPPED (Q-L-9 + PA-002) | §7 |
| DFK012 | К-L12 native scheduler sovereignty | Deferred к К-L20 LOCK cascade | [ROADMAP.md «Analyzer track»](../ROADMAP.md) |
| DFK014 | К-L14 meta-invariant | Outside Roslyn scope (evidence dashboard) | §7 |
| DFK015 | К-L15 bus capability declaration | Deferred к К-L20 LOCK cascade | [ROADMAP.md «Analyzer track»](../ROADMAP.md) |
| DFK018 | К-L18 mod unload quiescence | Deferred к К-L20 LOCK cascade | [ROADMAP.md «Analyzer track»](../ROADMAP.md) |
| DFK019.B | К-L19 hardware tier runtime check | Deferred к hardware tier expansion cascade | [ROADMAP.md «Analyzer track»](../ROADMAP.md) |
| DFK020 family | К-L20 (~20 sub-rules) | Deferred к К-L20 LOCK cascade | [ROADMAP.md «Analyzer track»](../ROADMAP.md) |
| DFC001.A / DFC001.B | Bridge marker / record purity | Deferred к К-L20 LOCK cascade (Q-L-11) | [ROADMAP.md «Analyzer track»](../ROADMAP.md) |
| DFL025-C | Lesson #25 shell-level rule | Not a Roslyn analyzer (scope exclusion) | §4.1 notes |

### §10.5 — Forward implementation plan — relocated

Planned — see [ROADMAP.md «Analyzer track»](../ROADMAP.md) (Phase β detection implementation + tests + violation triage + adaptive gate; Phase γ severity promotion; cleanup-phase discipline). Relocated 2026-06-11. The v0.1 `DFK###-SUPPRESS` comment-marker sketch that lived here is **superseded** by the DFK-WAIVER form — see §12.

---

## §11 — Lifecycle

**Current**: AUTHORED-SKELETON v0.4.0 (А'.8 closure 2026-05-23; A'.9.1 Phase α structural reorganization 2026-05-25; Standing-Law restructure 2026-06-11; Phase β detection + Phase γ severity promotion 2026-07-01). 17 detecting rules shipped at Release 1.0 severities — 16 build-breaking under `TreatWarningsAsErrors`, DFL025_B IDE-only; 2 DFK-WAIVER suppressions (census-pinned).

**Forward**: lifecycle sequencing (Phase β population → Phase γ promotion → Phase δ closure/governance → promotion к Tier 1 LOCKED) relocated — see [ROADMAP.md «Analyzer track»](../ROADMAP.md).

---

## §12 — Suppression (deferred by citation)

Suppression and waiver law for all DFK### / DFL### / DF999 diagnostics is owned by [CODING_STANDARDS.md §5.3 «DFK-WAIVER — the suppression law»](../methodology/CODING_STANDARDS.md) (authored in the same Standing-Law cascade). ANALYZER_RULES.md does not own marker or suppression law:

- The §2 template's per-rule «Suppression policy» rows **cite** the DFK-WAIVER form; they do not restate syntax.
- The v0.1 §2 syntax sketch (`#pragma warning disable DFK###` / `[SuppressMessage("DualFrontier.<Category>", "DFK###")]`) and the v0.1 §10.5 `DFK###-SUPPRESS` comment-marker sketch are **superseded** by the DFK-WAIVER form.
- Baseline at v0.2.0: **zero waivers** in the solution (no DFK/DFL/DF9 suppression markers in any `.cs`). Current since the Phase β triage: **2 waivers** — both DFK001, `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs` (К-L19-sanctioned VK_EXT_debug_utils debug-messenger interop), census-pinned by the compiled meta-test (TESTING_STRATEGY §4.3 — DFK-WAIVER census).

---

## Change history

| Version | Date | Change |
|---|---|---|
| 0.4.1 | 2026-07-02 | A'.9.1 Phase δ rider — F-27(c) PATCH: §9 DFL### note gains the realized tail (past-tense stub provenance kept; detection landed Phase β; enforcement since Phase γ Release 1.0) + underscore rule-ID forms on the touched line. No spec change. |
| 0.4.0 | 2026-07-01 | A'.9.1 Phase γ severity promotion (F-12 ratified 2026-07-01 — DFK019_A = Warning): 16 descriptors flipped Info → shipped severities (11 Error + 5 Warning; DFL025_B stays descriptor Info ≡ editorconfig «suggestion»); AnalyzerReleases Release 1.0 (Unshipped → Shipped, RS-tracked); root .editorconfig primed with the 17 identical keys. Document truthed-up from stub/detection-pending wording to the enforcement state: §1/§4/§4.1 (Status → enforcing; target column collapsed into «Shipped severity (Release 1.0)»), §2 template, §3, §4.2, §4.3 (Release 1.0 tables, 54-test suite), §10, §11, §12 (waiver census 0 → 2 current). In-body version markers (the stale 0.2.1 Version line + v0.2.0 end-marker, recon Anomaly 4) synced. Register-mirror version field sync folded at the Phase γ REGISTER cascade. |
| 0.2.2 | 2026-07-01 | Descriptor-ID adjudication (Phase β empirical finding, ratified by Crystalka): the dotted DFK sub-rule IDs (DFK003.1/DFK007.1/DFK015.1/DFK019.A) and hyphenated DFL variant IDs (DFL025-A/DFL025-B) are rejected by Roslyn `ReportDiagnostic` as invalid identifiers, so all descriptor IDs are normalized to the underscore form already used by the file/class names (DFK003_1 … DFL025_B). §4.1 naming convention amended; registry table + `AnalyzerReleases.Unshipped.md` rule IDs + the six stub descriptor consts/help-anchors updated in the same change. DFL025-C (shell-level filter, not a Roslyn rule) unaffected. Register version bump folded at Phase β REGISTER cascade. |
| 0.1 | 2026-05-23 / 2026-05-25 | Initial AUTHORED-SKELETON at А'.8 closure (Commit 5 REGISTER enrollment); structural reorganization at A'.9.1 Phase α Commit 5. |
| 0.2.1 | 2026-06-12 | Architecture Truth Cascade PATCH: §4.1 footnote ² cross-reference old-form `DF019` → `DFK019` (current namespace; aligns with the DFK019.A registry row). No spec change. |
| 0.2.0 | 2026-06-11 | Structural separation per [DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md §6.3.2](../reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md) + Standing-Law cascade: roadmap load → [ROADMAP.md «Analyzer track»](../ROADMAP.md) (§4.1/§4.2 promotion columns + implementation order, §5 К-L20 family, §6 hardware tier, §10.5 forward plan, §11 forward, frontmatter «Forward sequencing»); §4 stub-truth correction (enforcement-surface overclaim cured — 17 non-detecting Info stubs, zero diagnostics, detection PENDING Phase β); single registry (v0.1 §10.1–10.3 duplicates collapsed; DFK019.A target resolved к Warning per K_CLOSURE §7.2); §7/§8 reframed as realized decision records; `MOD_API_CONTRACT.md` phantom reference resolved to explicit forward-reference; suppression law deferred к CODING_STANDARDS.md §5.3 «DFK-WAIVER» (§12 NEW). |

---

**End of ANALYZER_RULES.md v0.4.1 AUTHORED-SKELETON (A'.9.1 Phase δ F-27 rider 2026-07-02)**

**Maintenance**: §4.1 is the single rule registry; §10 receives per-rule detail as Phase β lands; all sequencing and futures live in [ROADMAP.md «Analyzer track»](../ROADMAP.md). К-Lxx invariant authority resides в KERNEL_ARCHITECTURE.md Part 0; canonical detection narratives в K_CLOSURE_REPORT.md §7; PA-001..PA-004 anchors в PROJECT_AXIOMS.md.
