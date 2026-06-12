---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ANALYZER_RULES
category: A
tier: 1
lifecycle: AUTHORED-SKELETON
owner: Crystalka
version: "0.2.0"
next_review_due: 2027-05-23
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ANALYZER_RULES
---
# DualFrontier Roslyn Analyzer Rule Specifications

**Lifecycle**: AUTHORED-SKELETON (initial entry at A'.8 closure 2026-05-23; structural reorganization at A'.9.1 Phase α Commit 5 2026-05-25; Standing-Law restructure 2026-06-11 — lifecycle unchanged: Phase β mutates §4 again)
**Version**: 0.2.0
**Date created**: 2026-05-23 (А'.8 К-series formal closure cascade Commit 5 REGISTER enrollment)
**Date restructured**: 2026-05-25 (A'.9.1 / К-extensions cascade #5 Phase α Commit 5); 2026-06-11 (Standing-Law cascade — roadmap-load extraction + stub-truth correction per [DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md §6.3.2](../reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md))
**Purpose**: Roslyn analyzer rule specifications encoding К-Lxx invariants. This document states what exists; scheduled futures live in [ROADMAP.md «Analyzer track»](../ROADMAP.md). Populated к LOCKED at A'.9.1+ Roslyn analyzer milestone implementation cascade.

**Authority**: This document specifies analyzer rules. Canonical rule narratives + detection patterns + diagnostic messages reside в [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications). К-Lxx invariant authority resides в [KERNEL_ARCHITECTURE.md Part 0](KERNEL_ARCHITECTURE.md#part-0-locked-foundational-decisions). PA-001..PA-004 axiom anchors reside в [PROJECT_AXIOMS.md](../governance/PROJECT_AXIOMS.md) (introduced A'.9.1 Phase α Commit 8).

**Forward sequencing**: relocated — see [ROADMAP.md «Analyzer track»](../ROADMAP.md) for all analyzer sequencing (A'.9.1 Phase β/γ/δ) and the deferred rule families (К-L20 LOCK cascade, hardware tier expansion).

---

## §1 — Document framing + authority chain

This document is the authoritative specification for the DualFrontier Roslyn analyzer rule surface: rule IDs, namespaces, categories, the per-rule template, and the realized scope decisions. What the rules **do today** is stated in §4 (ground truth: 17 non-detecting stubs, zero diagnostics). What the rules **are scheduled to become** is owned by [ROADMAP.md «Analyzer track»](../ROADMAP.md) — spec/roadmap separation per Standing-Law cascade 2026-06-11. §7 and §8 are **realized decision records** (closed Q-L outcomes), not forward plans.

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
**Severity (Phase-γ promotion target)**: Error / Warning / Suggestion — shipped stubs carry `Info` until Phase γ (see ROADMAP.md «Analyzer track»)
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

**Self-policing**: DF999 (analyzer rule that bans solution-wide GlobalSuppressions / `[assembly: SuppressMessage]` per Q-L-18 default — pending batch 3 governance ratification). Shipped as a non-detecting stub at A'.9.1 Phase β-prep per the default.

---

## §4 — A'.9.1 first-batch rule registry (17 stubs — detection PENDING Phase β)

**Ground truth (v0.2.0, verified on disk 2026-06-11):** all 17 first-batch rules exist as **non-detecting stubs**. Every stub declares its `DiagnosticDescriptor` with `DiagnosticSeverity.Info`; every `Initialize()` calls `ConfigureGeneratedCodeAnalysis` + `EnableConcurrentExecution` and registers **no** analysis actions; **zero diagnostics are emitted by design** (Phase α / Phase β-prep scaffolding). Detection logic lands at A'.9.1 Phase β — see [ROADMAP.md «Analyzer track»](../ROADMAP.md). No rule detects or blocks anything today; nothing in this section is an enforcement claim.

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

**Naming convention** (stated once): descriptor ID strings use **dots** for DFK sub-rules (`DFK003.1`, `DFK007.1`, `DFK015.1`, `DFK019.A`) and **hyphens** for DFL variants (`DFL025-A`, `DFL025-B`); file and class names substitute **underscores** (`DFK003_1StorageBridgeAnalyzer`, `DFL025_AReservedStubInvocationAnalyzer`) — C# identifiers admit neither dots nor hyphens.

**Categories per Q-L-7** (3 in use by shipped stub descriptors + 1 reserved):
- `DualFrontier.Architecture` — DFK### kernel architectural invariants
- `DualFrontier.NativeBoundary` — DFK### native interop discipline (P/Invoke surface, GPU pipeline slot)
- `DualFrontier.Discipline` — DFL### Lesson-derived discipline (NEW category at A'.9.1)
- `DualFrontier.ModSurface` — RESERVED (no stub carries it; scheduled for the К-L20 LOCK cascade — see §5 pointer)

### §4.1 — Registry (single source — 17 rules)

| Rule ID | К-L / Lesson anchor | Tier | Category | Shipped severity | Phase-γ target¹ | Status |
|---|---|---|---|---|---|---|
| DFK001 | К-L1 | P0 | NativeBoundary | Info | Error | stub |
| DFK002 | К-L2 | P0 | NativeBoundary | Info | Error | stub |
| DFK003 | К-L3 | P0 | Architecture | Info | Error | stub |
| DFK003.1 | К-L3.1 | P0 | Architecture | Info | Error | stub |
| DFK004 | К-L4 | P0 | Architecture | Info | Error | stub |
| DFK005 | К-L5 | P0 | Architecture | Info | Error | stub |
| DFK007 | К-L7 | P0 | Architecture | Info | Error | stub |
| DFK011 | К-L11 | P0 | Architecture | Info | Error | stub |
| DFK007.1 | К-L7.1 | P1 | NativeBoundary (GPU pipeline slot) | Info | Error | stub |
| DFK015.1 | К-L15.1 | P1 | NativeBoundary (three-tier mutex managed facade) | Info | Error | stub |
| DFK017 | К-L17 | P1 | Architecture (display composition multi-layer) | Info | Error | stub |
| DFK019.A | К-L19 static API surface | P1 | NativeBoundary | Info | Warning² | stub |
| DFK013 | К-L13 | β-secondary | Architecture | Info | Warning | stub |
| DFK016 | К-L16 (retain α — Phase 0 ratified per Q-L-16) | β-secondary | Architecture | Info | Warning | stub |
| DFL025-A | Lesson #25 refined 3rd extension | Discipline | Discipline | Info | Warning | stub |
| DFL025-B | Lesson #25 refined 3rd extension | Discipline | Discipline | Info | Suggestion | stub |
| DF999 | analyzer self-discipline (Q-L-18 default) | Self-policing | Discipline | Info | Warning | stub |

¹ «Phase-γ target» is the severity the rule is **specified to receive at A'.9.1 Phase γ promotion** — it is not the current behavior. Promotion sequencing and the `.editorconfig` mechanics are owned by [ROADMAP.md «Analyzer track»](../ROADMAP.md). Until Phase γ executes, every rule ships `Info` and emits nothing.

² Registry-drift resolution (v0.2.0): v0.1 carried two parallel registries (§4.1–4.5 and §10.1–10.3) whose columns drifted — §4.2 grouped DFK019.A under an «Error severity post-promotion» header while §10.1 listed Warning. Resolved to **Warning** per canonical [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications) DF019 row (Warning, V substrate contract — configurable). v0.2.0 keeps exactly one registry — this table.

**Intended detection notes** (Phase β targets — none implemented at v0.2.0; canonical narratives: K_CLOSURE_REPORT.md §7.2):

- **DFK013** — on-demand activation discipline (efficiency-not-correctness): `SystemBase` subclass без `[WakeOn*]` AND без `[TickRate]`, OR eager-init в `Initialize()`.
- **DFK016** — pipeline depth D ∈ {1, 2, 3} hardcoded vs `PipelineSlotInterop.DefaultDepth` / `.MaxDepth` constants. Phase 0 ratified retain α (managed surface stable per closure report §3.3).
- **DFL025-A** — test class invoking `[ReservedStub]`-tagged type MUST carry `[Trait("Category", "ReservedStub")]`; T2 syntax tree + SemanticModel attribute check via fully-qualified name match.
- **DFL025-B** — standalone test methods против reserved-stub modules SHOULD use `[Fact(Skip="...")]`; edge-case discipline.
- **DF999** — bans solution-wide `GlobalSuppressions.cs` / `[assembly: SuppressMessage]` (Q-L-18 default, pending batch 3 governance ratification); forces per-site suppression discipline (Q-L-20).
- **DFL025-C** — closure-protocol shell-level rule (`--filter "Category!=ReservedStub"` в `dotnet test` invocation) — **NOT a Roslyn analyzer**; deferred к governance tooling cascade per amendments §4.5. Listed here as a scope exclusion only.

### §4.2 — Composition + S-LOCK-4 count audit (trimmed)

Composition reconciled to on-disk truth: **8 P0 + 4 P1 + DFK013 + DFK016 (retain α — Phase 0 ratified) + DFL025-A/B + DF999 = 17 stubs.**

**S-LOCK-4 count audit (essentials)**: Brief §6.5 narrative stated «Total A'.9.1 enforcement surface: 15-16 own rules» — arithmetic carryover from pre-Q-L-9 / pre-Phase-0 drafting (DFK010 still counted as P1; DFK016 still a Phase-0 conditional). Post-supersession arithmetic = 17, matching the 17 files on disk. Surfaced for forward-cascade audit trail. The v0.1 phrase «enforcement surface» itself was an overclaim — at v0.2.0 the accurate term is **stub surface** (detection PENDING Phase β).

### §4.3 — Infrastructure ground truth

- **Wiring**: `src/Directory.Build.props` adds `tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj` as a `ProjectReference` with `OutputItemType="Analyzer"` to all 12 `src/` projects — the stub assembly loads at every build; loading is not detecting.
- **Release tracking**: `AnalyzerReleases.Shipped.md` is empty (header only — first release entry lands at A'.9.1 closure); `AnalyzerReleases.Unshipped.md` lists all 17 rules in its New Rules table.
- **Tests**: `tests/DualFrontier.Analyzers.Tests` contains a single placeholder Fact; per-rule positive/negative suites land at Phase β (see [ROADMAP.md «Analyzer track»](../ROADMAP.md)).

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
- **DFL###** namespace — in use at A'.9.1 (DFL025-A + DFL025-B shipped as non-detecting stubs; future DFL### rules added как additional Lessons materialize forward).
- **DualFrontier.ModSurface** category — RESERVED at A'.9.1 (no stub carries it; scheduled for the К-L20 LOCK cascade per Q-L-11 + §5 pointer).

---

## §10 — Per-rule detail specifications (populated as Phase β lands)

Landing zone for per-rule §2-template entries. **Empty at v0.2.0** — all 17 rules are non-detecting stubs, so no per-rule detail exists beyond the §4.1 registry. Entries are added here as each rule's Phase β detection implementation lands.

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

**Current**: AUTHORED-SKELETON v0.2.0 (А'.8 closure 2026-05-23; A'.9.1 Phase α structural reorganization 2026-05-25; Standing-Law restructure 2026-06-11). 17 rule stubs shipped; zero detection logic; zero diagnostics.

**Forward**: lifecycle sequencing (Phase β population → Phase γ promotion → Phase δ closure/governance → promotion к Tier 1 LOCKED) relocated — see [ROADMAP.md «Analyzer track»](../ROADMAP.md).

---

## §12 — Suppression (deferred by citation)

Suppression and waiver law for all DFK### / DFL### / DF999 diagnostics is owned by [CODING_STANDARDS.md §5.3 «DFK-WAIVER — the suppression law»](../methodology/CODING_STANDARDS.md) (authored in the same Standing-Law cascade). ANALYZER_RULES.md does not own marker or suppression law:

- The §2 template's per-rule «Suppression policy» rows **cite** the DFK-WAIVER form; they do not restate syntax.
- The v0.1 §2 syntax sketch (`#pragma warning disable DFK###` / `[SuppressMessage("DualFrontier.<Category>", "DFK###")]`) and the v0.1 §10.5 `DFK###-SUPPRESS` comment-marker sketch are **superseded** by the DFK-WAIVER form.
- Baseline at v0.2.0: **zero waivers** in the solution (no DFK/DFL/DF9 suppression markers in any `.cs`).

---

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1 | 2026-05-23 / 2026-05-25 | Initial AUTHORED-SKELETON at А'.8 closure (Commit 5 REGISTER enrollment); structural reorganization at A'.9.1 Phase α Commit 5. |
| 0.2.0 | 2026-06-11 | Structural separation per [DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md §6.3.2](../reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md) + Standing-Law cascade: roadmap load → [ROADMAP.md «Analyzer track»](../ROADMAP.md) (§4.1/§4.2 promotion columns + implementation order, §5 К-L20 family, §6 hardware tier, §10.5 forward plan, §11 forward, frontmatter «Forward sequencing»); §4 stub-truth correction (enforcement-surface overclaim cured — 17 non-detecting Info stubs, zero diagnostics, detection PENDING Phase β); single registry (v0.1 §10.1–10.3 duplicates collapsed; DFK019.A target resolved к Warning per K_CLOSURE §7.2); §7/§8 reframed as realized decision records; `MOD_API_CONTRACT.md` phantom reference resolved to explicit forward-reference; suppression law deferred к CODING_STANDARDS.md §5.3 «DFK-WAIVER» (§12 NEW). |

---

**End of ANALYZER_RULES.md v0.2.0 AUTHORED-SKELETON (Standing-Law cascade restructure 2026-06-11)**

**Maintenance**: §4.1 is the single rule registry; §10 receives per-rule detail as Phase β lands; all sequencing and futures live in [ROADMAP.md «Analyzer track»](../ROADMAP.md). К-Lxx invariant authority resides в KERNEL_ARCHITECTURE.md Part 0; canonical detection narratives в K_CLOSURE_REPORT.md §7; PA-001..PA-004 anchors в PROJECT_AXIOMS.md.
