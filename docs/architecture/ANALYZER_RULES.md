---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ANALYZER_RULES
category: A
tier: 1
lifecycle: AUTHORED-SKELETON
owner: Crystalka
version: "0.1"
next_review_due: 2027-05-23
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ANALYZER_RULES
---
# DualFrontier Roslyn Analyzer Rule Specifications

**Lifecycle**: AUTHORED-SKELETON (initial entry at A'.8 closure 2026-05-23; structural reorganization at A'.9.1 Phase α Commit 5 2026-05-25)
**Version**: 0.1
**Date created**: 2026-05-23 (А'.8 К-series formal closure cascade Commit 5 REGISTER enrollment)
**Date restructured**: 2026-05-25 (A'.9.1 / К-extensions cascade #5 Phase α Commit 5)
**Purpose**: Roslyn analyzer rule specifications encoding К-Lxx invariants. Populated к LOCKED at A'.9.1+ Roslyn analyzer milestone implementation cascade.

**Authority**: This document specifies analyzer rules. Canonical rule narratives + detection patterns + diagnostic messages reside в [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications). К-Lxx invariant authority resides в [KERNEL_ARCHITECTURE.md Part 0](KERNEL_ARCHITECTURE.md#part-0-locked-foundational-decisions). PA-001..PA-004 axiom anchors reside в [PROJECT_AXIOMS.md](../governance/PROJECT_AXIOMS.md) (introduced A'.9.1 Phase α Commit 8).

**Forward sequencing**: A'.9.1 Analyzer Infrastructure cascade implements first-batch active rules per §4. К-L20 LOCK cascade activates DFK009/DFK012/DFK015/DFK018/DFK020 family + DFC001 (per §5). Hardware tier expansion cascade activates DFK019.B (per §6).

---

## §1 — Document framing + authority chain

This document is the authoritative specification for the DualFrontier Roslyn analyzer rule surface.

**Authority chain**:
- [KERNEL_ARCHITECTURE.md Part 0](KERNEL_ARCHITECTURE.md#part-0-locked-foundational-decisions) — К-Lxx invariant canonical text (Tier 1 LOCKED)
- [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications) — canonical detection narratives for each rule (Tier 1 LOCKED)
- [PROJECT_AXIOMS.md](../governance/PROJECT_AXIOMS.md) — PA-001..PA-004 axiom anchors driving rule scope decisions (Tier 1 LOCKED, NEW A'.9.1)
- **This document** — analyzer rule specifications, severity, category, lifecycle (Tier 1, lifecycle AUTHORED-SKELETON → LOCKED at A'.9.1+ closure)

**Q-L ratifications history** (forward-locked at Brief A'.9.1 batch 1 + 2 deliberation 2026-05-24):
- Q-L-3 — Tiered namespaces (DFK###/DFL###/DFC###) per §3
- Q-L-7 — 3 active categories (Architecture/NativeBoundary/Discipline) + ModSurface reserved per §4
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

**Per-rule specification format**:

```markdown
### DFK### — <К-L invariant title>

**К-L invariant**: <К-L# reference + canonical text link к KERNEL_ARCHITECTURE.md Part 0>
**Category**: DualFrontier.Architecture / DualFrontier.NativeBoundary / DualFrontier.Discipline
**Severity**: Error / Warning / Suggestion
**Status**: Active / Reserved / Outside-Roslyn-scope
**Implementation status**: Pending A'.9.1 Phase β / Implemented at A'.9.1 / Active post-A'.9.1

**Detection narrative** (3-5 sentences per Q5.3 LOCKED Session 1):
<What patterns the rule detects. Reference K_CLOSURE_REPORT.md §7.2 для canonical detection narrative.>

**Diagnostic message text**:
> «<diagnostic message string>»

**Example violation patterns**:
- <code pattern 1 that triggers diagnostic>
- <code pattern 2 that triggers diagnostic>

**Suppression policy**:
- When suppression appropriate: <rare cases с justification>
- Suppression syntax: `#pragma warning disable DFK###` OR `[SuppressMessage("DualFrontier.<Category>", "DFK###")]`
- Suppression requires architectural justification comment

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
- **DFC###** — Cascade-specific rules (drift detection). Reserved для rules surfaced by individual cascade execution to detect specific drift patterns. Inactive at A'.9.1 per Q-L-11 (DFC001 deferred к К-L20 LOCK cascade).

**Self-policing**: DF999 (analyzer rule that bans solution-wide GlobalSuppressions / `[assembly: SuppressMessage]` per Q-L-18 default — pending batch 3 governance). Ships at A'.9.1 per default.

---

## §4 — A'.9.1 active rules — first-batch enforcement surface

**Categories per Q-L-7** (3 active + 1 reserved):
- `DualFrontier.Architecture` — DFK### kernel architectural invariants
- `DualFrontier.NativeBoundary` — DFK### native interop discipline (P/Invoke surface, GPU pipeline slot)
- `DualFrontier.Discipline` — DFL### Lesson-derived discipline (NEW category at A'.9.1)
- `DualFrontier.ModSurface` — RESERVED (no active rules at A'.9.1; activates at К-L20 LOCK cascade per §5)

### §4.1 — P0 Critical (8 rules — Error severity post-promotion)

| Rule | К-L | Category | Phase β implementation order |
|---|---|---|---|
| DFK001 | К-L1 | NativeBoundary | 1 |
| DFK002 | К-L2 | NativeBoundary | 2 |
| DFK003 | К-L3 | Architecture | 3 |
| DFK003.1 | К-L3.1 | Architecture | 4 |
| DFK004 | К-L4 | Architecture | 5 |
| DFK005 | К-L5 | Architecture | 6 |
| DFK007 | К-L7 | Architecture | 7 |
| DFK011 | К-L11 | Architecture | 8 |

### §4.2 — P1 High (4 rules — Error severity post-promotion)

| Rule | К-L | Category | Phase β implementation order |
|---|---|---|---|
| DFK007.1 | К-L7.1 | NativeBoundary (GPU pipeline slot) | 9 |
| DFK015.1 | К-L15.1 | NativeBoundary (three-tier mutex managed facade) | 10 |
| DFK017 | К-L17 | Architecture (display composition multi-layer) | 12 |
| DFK019.A | К-L19 static API surface | NativeBoundary | 13 |

**Audit note on P1 count**: Brief §1.2 + amendments log §3.3 specify «P1 (5 rules) per amendments log §3.3» enumerating `[DF007.1, DF010, DF015.1, DF017, DF019]`. Per **Q-L-9 supersession** (Brief A'.9.1 batch 2 deliberation): DFK010 PERMANENTLY DROPPED (methodology-layer outside Roslyn scope; PA-002 axiom anchor) — moved к §7. Effective post-Q-L-9 P1 = **4 rules** (Q-L-9 supersedes the amendments log enumeration). DFK019 split per Q-L-8 — only DFK019.A here; DFK019.B at §6.

### §4.3 — Phase β secondary (2 rules — Warning severity, retain α per Phase 0)

| Rule | К-L | Category | Severity | Notes |
|---|---|---|---|---|
| DFK013 | К-L13 | Architecture | Warning | On-demand activation discipline (efficiency-not-correctness). Detection: SystemBase subclass без [WakeOn*] AND без [TickRate] OR eager-init in Initialize(). |
| DFK016 | К-L16 | Architecture | Warning | Pipeline depth D ∈ {1, 2, 3} hardcoded vs PipelineSlotInterop.DefaultDepth / .MaxDepth constants. Phase 0 ratified retain α (managed surface stable per closure report §3.3). |

### §4.4 — Lesson-derived Discipline (2 rules — DFL025 family per amendments §4)

| Rule | Lesson | Category | Severity | Notes |
|---|---|---|---|---|
| DFL025-A | #25 refined 3rd extension | Discipline | Warning | Test class invoking [ReservedStub]-tagged type MUST carry `[Trait("Category", "ReservedStub")]`. Detection: T2 syntax tree + SemanticModel attribute check via fully-qualified name match. |
| DFL025-B | #25 refined 3rd extension | Discipline | Suggestion | Standalone test methods против reserved-stub modules SHOULD use `[Fact(Skip="...")]`. Edge case discipline. |

**DFL025-C** — closure-protocol shell-level rule (`--filter "Category!=ReservedStub"` in `dotnet test` invocation) — **NOT a Roslyn analyzer**, defer к governance tooling cascade per amendments §4.5.

### §4.5 — Self-policing (1 rule per Q-L-18 default)

| Rule | Lesson | Category | Severity | Notes |
|---|---|---|---|---|
| DF999 | (analyzer self-discipline) | Discipline | Warning | Bans solution-wide `GlobalSuppressions.cs` / `[assembly: SuppressMessage]` per Q-L-18 default (pending batch 3 governance ratification; ships at A'.9.1 per default). Forces per-site suppression discipline (Q-L-20). |

### §4.6 — Enforcement surface total

**A'.9.1 first-batch active enforcement surface: 17 own rules.**

Composition: 8 P0 + 4 P1 + 2 Phase β secondary (DFK013 + DFK016 retain α) + 2 DFL### Discipline + 1 DF999 self-policing = **17 rules**.

**S-LOCK-4 count audit**: Brief §6.5 §4 narrative text states «Total A'.9.1 enforcement surface: 15-16 own rules» — this appears to be arithmetic carryover from pre-Q-L-9 / pre-Phase-0 drafting (when DFK010 was P1 and DFK016 was «pending Phase 0 empirical decision»). Post-supersession arithmetic:
- 8 P0 + 5 P1 (pre-Q-L-9) + DFK013 secondary + 2 DFL + 1 DF999 + Phase 0 DFK016 conditional → 16 own rules pre-Phase-0 + 0..1 conditional = «15-16» surface estimate.
- 8 P0 + 4 P1 (post-Q-L-9) + DFK013 + DFK016 retain α (Phase 0 ratified) + 2 DFL + 1 DF999 = **17 rules**.

Surfaced for forward-cascade audit trail. Phase β stub implementation (Phase α exit gate) will materialize all 17 rules per §10.3 implementation order.

---

## §5 — К-L20 LOCK cascade deferred (Mod-API-coupled)

Per amendments log §3 (5-rule deferral) + Crystalka direction §1.1 batch 2 (Brief A'.9.1 deliberation 2026-05-24), the following are deferred к К-L20 LOCK cascade — pre-emptive enforcement against moving Mod API target violates PA-002 axiom («без костылей»):

| Rule | К-L | Deferral rationale |
|---|---|---|
| DFK009 | К-L9 Vanilla=mods | IModApi surface volatile pre-К-L20 LOCK; mod parity surface defines what «IModApi» means — undefined pre-LOCK. |
| DFK012 | К-L12 native scheduler sovereignty | Managed scheduler facade boundary not finalized pre-К-L20 LOCK; facade contract (К-L9 «facade preserves Vanilla = mods») depends on К-L20 LOCK for definition. |
| DFK015 | К-L15 bus capability declaration | Capability vocabulary (К-L15 tier registration, capability tokens, FQN scoping) ties tightly к К-L20 mod API surface; pre-LOCK не finalized. |
| DFK018 | К-L18 mod unload quiescence | Lifecycle sequence (PauseAsync → WaitForQuiescenceAsync → operation → ResumeAsync) part of К-L20 Mod API contract; refinement at К-L20 LOCK pending (К10.3 v2 §9.5 8-step → 9-step с V resource cleanup placeholder per K_CLOSURE §2.21). |
| DFK020 family | К-L20 | 20 candidate sub-rules per recon §6.2: namespace/type restrictions, API usage restrictions, manifest field static cross-check, forward-compatibility grace period semantics. К-L20 canonical text post-LOCK. |
| DFC001.A | Bridge IRenderCommand marker purity | Bridge surface = Mod API-coupled per Q-L-11. К-L20 LOCK clarifies Bridge contract; pre-LOCK marker enforcement = pre-emptive kostyl. |
| DFC001.B | Bridge Command record purity | Same rationale as DFC001.A. Record purity (no mutable members) part of К-L20 Mod API immutability contract. |

**Activation**: К-L20 LOCK cascade post-A'.9 milestone per K_CLOSURE §9.5 Q1-Q8 deliberation timing. Cascade likely multi-stage decomposition (substantial 5 K-L-specific + 20 DFK020 sub-rules surface per amendments log §3.4).

---

## §6 — Hardware tier expansion cascade deferred

Per Q-L-8 split + Crystalka direction §1.6 batch 2 + recon Q-K-4 — multi-hardware-tier audience absent at current cascade. Activation when audience materializes:

| Rule | К-L | Deferral rationale |
|---|---|---|
| DFK019.B | К-L19 hardware tier capability runtime check | Requires runtime hardware capability probe (Vulkan extension query, GPU memory tier detection). Multi-tier hardware audience absent — single tier (T1 high-end Vulkan 1.3) is current substrate baseline. Audience-driven deferral per Lesson #N17 Provisional. |
| DFK016 threshold customization API | К-L16 configurable depth | Optional follow-on if DFK016 retain α surface needs runtime customization API beyond compile-time PipelineSlotInterop constants. Currently NOT activated — Phase 0 retain α is sufficient (compile-time constants stable). |

**Activation**: hardware tier expansion cascade (timing TBD when multi-tier hardware audience materializes — Crystalka direction §1.6 batch 2 «расширять V» post-A'.9 V-extension cascade may surface this).

---

## §7 — Outside Roslyn scope (alternative enforcement)

Per K_CLOSURE §7.3 + Brief A'.9.1 Q-L-9 ratification — rules requiring alternative enforcement mechanisms (PA-002 anchor «без костылей» — Roslyn pretending к enforce methodology-layer = kostyl):

| Rule | К-L | Reason outside Roslyn scope | Alternative mechanism |
|---|---|---|---|
| DFK006 | К-L6 SUPERSEDED | К-L6 displaced by К-L12 native scheduler sovereignty (К10 cascade) — historical traceability only; never activates | Reserved historical entry; no enforcement |
| DFK008 | К-L8 component lifetime | Process-invariant (storage path discipline); pre-commit hook OR runtime probe more appropriate | Git pre-commit hook (per K_CLOSURE §7.3) |
| DFK010 | К-L10 decision rule | **PERMANENTLY DROPPED per Q-L-9 + PA-002 axiom**. К-L10 governs decision attribution at document/methodology layer, NOT code layer. Code-layer marker masquerading doc-layer reasoning = kostyl pattern. | FRAMEWORK + METHODOLOGY documentation discipline only |
| DFK014 | К-L14 meta-invariant | Meta-invariant tracking «defect rate / architectural integrity / pipeline economics» — not pattern detectable in syntax tree | [K_L14_EVIDENCE_DASHBOARD.md](K_L14_EVIDENCE_DASHBOARD.md) cumulative evidence tracking |

**S-LOCK-10 invariant**: DFK010 has NO implementation file. Verify ANALYZER_RULES.md §4 P1 = 4 rules (NOT 5 per amendments §3.3 pre-supersession list).

---

## §8 — Audience-driven deferred (community emergence)

Per Q-L-13 + Q-L-15 + Q-L-12 — Brief A'.9.1 batch 2 deliberation 2026-05-24. PA-001 axiom anchor: current audience profile = AI agents permanently; community ecosystem absent.

### §8.1 — PublicApiAnalyzers (RS0016/RS0017/RS0024) — Q-L-13 + PA-001

**Status**: DEFERRED entirely. Not in `Directory.Packages.props`.

**Rationale**: Community ecosystem absent (PA-001 anchor). All candidate assemblies (Contracts/Application/Bridge/Launcher) = Mod API surface volatile pre-К-L20 LOCK OR not mod-facing.

**Activation conditions** (any of):
- Community emergence (third-party developers consuming public API surface)
- Public API stability commitment к external consumers
- Specific cascade brief explicitly requesting PublicApiAnalyzers adoption (would re-trigger Q-L deliberation)

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

- **DFC###** namespace — RESERVED at A'.9.1 (no active rules; activates at К-L20 LOCK cascade per Q-L-11 DFC001 deferral). Forward designation для cascade-specific drift detection rules.
- **DFL###** namespace — ACTIVE at A'.9.1 (DFL025-A + DFL025-B ship; future DFL### rules added как additional Lessons materialize forward).
- **DualFrontier.ModSurface** category — RESERVED at A'.9.1 (no active rules; activates at К-L20 LOCK cascade per Q-L-11 + §5 deferral).

---

## §10 — Per-rule detail specifications (populated post-Phase-β implementation)

This section absorbs the prior §3 (Forward implementation plan) + §4 (Initial rule list table) from the AUTHORED-SKELETON v0.1 baseline. Per-rule detailed specifications per §2 template are populated as each rule's Phase β implementation lands.

### §10.1 — Initial rule list (per K_CLOSURE_REPORT.md §7.2)

| Rule | К-L | Severity | Category | Status |
|---|---|---|---|---|
| DFK001 | К-L1 | Error | NativeBoundary | Active (pending A'.9.1 Phase β impl) |
| DFK002 | К-L2 | Error | NativeBoundary | Active (pending A'.9.1 Phase β impl) |
| DFK003 | К-L3 | Error | Architecture | Active (pending A'.9.1 Phase β impl) |
| DFK003.1 | К-L3.1 | Error | Architecture | Active (pending A'.9.1 Phase β impl) |
| DFK004 | К-L4 | Error | Architecture | Active (pending A'.9.1 Phase β impl) |
| DFK005 | К-L5 | Error | Architecture | Active (pending A'.9.1 Phase β impl) |
| DFK007 | К-L7 | Error | Architecture | Active (pending A'.9.1 Phase β impl) |
| DFK007.1 | К-L7.1 | Error | NativeBoundary | Active (pending A'.9.1 Phase β impl) |
| DFK011 | К-L11 | Error | Architecture | Active (pending A'.9.1 Phase β impl) |
| DFK013 | К-L13 | Warning | Architecture | Active (Phase β secondary; efficiency-not-correctness) |
| **DFK015.1** | **К-L15.1** | **Error** | **NativeBoundary** | **Active (NEW А'.7.x; pending A'.9.1 Phase β impl)** |
| DFK016 | К-L16 | Warning | Architecture | Active (Phase β secondary; Phase 0 retain α — managed surface stable) |
| DFK017 | К-L17 | Error | Architecture | Active (pending A'.9.1 Phase β impl) |
| DFK019.A | К-L19 static API surface | Warning | NativeBoundary | Active (Q-L-8 split — pending A'.9.1 Phase β impl) |

### §10.2 — Lesson-derived (DFL### family)

| Rule | Lesson | Severity | Category | Status |
|---|---|---|---|---|
| DFL025-A | #25 refined 3rd extension | Warning | Discipline | Active (pending A'.9.1 Phase β impl; requires [ReservedStub] attribute from Commit 6) |
| DFL025-B | #25 refined 3rd extension | Suggestion | Discipline | Active (pending A'.9.1 Phase β impl) |

### §10.3 — Self-policing

| Rule | Severity | Category | Status |
|---|---|---|---|
| DF999 | Warning | Discipline | Active (Q-L-18 default; pending A'.9.1 Phase β impl) — bans GlobalSuppressions / `[assembly: SuppressMessage]` |

### §10.4 — Deferred rules (К-L20 LOCK cascade or hardware tier expansion or outside Roslyn scope)

| Rule | К-L | Deferral target | Reference |
|---|---|---|---|
| DFK006 | К-L6 SUPERSEDED | NEVER activates (historical) | §7 |
| DFK008 | К-L8 process invariant | Outside Roslyn scope (git pre-commit hook) | §7 |
| DFK009 | К-L9 Vanilla=mods | К-L20 LOCK cascade | §5 |
| DFK010 | К-L10 decision rule | PERMANENTLY DROPPED (PA-002) — outside Roslyn scope | §7 |
| DFK012 | К-L12 native scheduler sovereignty | К-L20 LOCK cascade | §5 |
| DFK014 | К-L14 meta-invariant | Outside Roslyn scope (K_L14_EVIDENCE_DASHBOARD) | §7 |
| DFK015 | К-L15 bus capability declaration | К-L20 LOCK cascade | §5 |
| DFK018 | К-L18 mod unload quiescence | К-L20 LOCK cascade | §5 |
| DFK019.B | К-L19 hardware tier capability runtime check | Hardware tier expansion cascade | §6 |
| DFK020 family | К-L20 family | К-L20 LOCK cascade (20 sub-rules per recon §6.2) | §5 |
| DFC001.A | Bridge IRenderCommand marker purity | К-L20 LOCK cascade (Mod-API-coupled) | §5 |
| DFC001.B | Bridge Command record purity | К-L20 LOCK cascade (Mod-API-coupled) | §5 |

### §10.5 — Forward implementation plan (A'.9.1 Phase β + γ)

**A'.9.1 Phase β scope** (post-Phase-α stub-analyzer violation count gate per Q-L-1 adaptive):
- Per-rule analyzer class (one per active DFK### / DFL### / DF999 rule)
- Test project с positive + negative cases per rule (via `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit`)
- Suppression governance: per-rule severity baseline at `suggestion` (cleanup phase mode)
- Cleanup-phase χ violation triage: (a) fix in code, (b) suppress with `// DFK###-SUPPRESS: <citation>`, (c) rule refinement if false-positive

**A'.9.1 Phase γ scope** (severity promotion):
- Per-rule `.editorconfig` baseline promotion `suggestion` → `error`
- `dotnet build` exit 0 with all DFK### at `severity = error` — proves К-Lxx compile-time enforcement live

**Cleanup phase discipline**:
- Per-rule cleanup discipline (one rule's violations resolved at a time per Lesson #8 atomic discipline)
- Cumulative debt resolution across 17 active rules
- Codebase audit may surface architectural debt unrelated к rule (handled per Lesson #14 separate cleanup cascade)

---

## §11 — Lifecycle forward

**Current**: AUTHORED-SKELETON v0.1 (А'.8 closure 2026-05-23; structural reorganization at A'.9.1 Phase α Commit 5 2026-05-25).

**Forward к LOCKED at A'.9.1 closure**:
- Per-rule §2 template populated через A'.9.1 Phase β cascade
- Roslyn analyzer infrastructure shipped (Phase α Commits 1-3 — csproj + tests + CPM)
- Test coverage per rule (positive + negative cases) at Phase β
- Phase γ severity promotion executed
- Cleanup-phase outcomes recorded per CAPA cascade
- Promotion к Tier 1 LOCKED при completed implementation + first-run cleanup phase

---

**End of ANALYZER_RULES.md v0.1 AUTHORED-SKELETON (post-A'.9.1 Phase α Commit 5 structural reorganization)**

**Forward maintenance**: A'.9.1 Phase β cascade implements 17 active rules per §2 template + §10.5 implementation plan. DFK020 family + DFK009/DFK012/DFK015/DFK018 added at К-L20 LOCK cascade. DFK019.B added at hardware tier expansion cascade. PROJECT_AXIOMS.md (NEW A'.9.1) provides PA-001..PA-004 anchor authority. К-Lxx invariant authority resides в KERNEL_ARCHITECTURE.md; this document encodes К-Lxx invariants as analyzer rule specifications.
