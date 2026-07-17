---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF
category: D
tier: 4
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF
---
# Brief A'.9.1 — Analyzer Infrastructure Cascade (К-extensions cascade #5)

**Designation**: A'.9.1 Analyzer Infrastructure cascade — first analyzer implementation cascade
**Dual lineage**: A'.9.1 milestone-internal + К-extensions cascade #5 (per Q-L-2 ratification + Q-K-44 recon recommendation)
**Predecessor cascade**: A'.9.0 Reconnaissance (К-ext #4) — `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md` v1.0 EXECUTED
**Authoring deliberation**: Two-session pre-authoring deliberation 2026-05-24
  - Batch 1: `SESSION_LOG_2026_05_24_A_PRIME_9_1_DELIBERATION.md` (Q-L-1..Q-L-7 ratifications)
  - Batch 2: `SESSION_LOG_2026_05_24_A_PRIME_9_1_BATCH_2_COMPLETE.md` (Q-L-8..Q-L-17 + Axiom Option (VII) ratifications)
**Authoring date**: 2026-05-24
**Estimated artifact size**: ~3380 LOC (Path B Hybrid per Q-L-1 default if χ violation count ≤80)
**Estimated wall-clock**: Phase α 60-90 min + Phase β TBD (post-Phase-0 violation count) + Phase γ 30 min
**Status**: AUTHORED — pending Crystalka ratification + execution session handoff

---

## §0 — Framing

### 0.1 — Cascade designation

A'.9.1 = **К-extensions cascade #5** + **A'.9.1 milestone-internal** dual designation per Q-L-2 ratification (recon §11.8 Q-K-44 Option (a) recommendation). Continues К-extensions sequence for KERNEL chronicle + LEDGER §3.6+ entries continuity. К-extensions sequence captures «cumulative milestone-touching cascades»; analyzer cascades qualify because they touch K-Lxx invariant enforcement surface.

**Cascade sequence post-A'.8 closure**:
- К-ext #1 — K10 Native Scheduler Sovereignty (К-L12 establishment)
- К-ext #2 — Godot Full Deprecation (К-L14 #11 first removal-type evidence)
- К-ext #3 — Launcher Visual Implementation (К-L14 #12 first clean additive evidence)
- К-ext #4 — A'.9.0 Reconnaissance (К-L14 #13 first observational baseline evidence)
- **К-ext #5 — A'.9.1 Analyzer Infrastructure (К-L14 #14 expected — first analyzer implementation evidence)** ← THIS CASCADE

### 0.2 — Project axioms applied (this cascade)

Per Axiom Option (VII) ratification (batch 2 session log §1.4) + PROJECT_AXIOMS.md draft (batch 2 session log §6.1), this cascade ships **PA-001..PA-004 codification** as a foundational governance artifact. The four axioms inform every Q-L ratification in this cascade:

- **PA-001** (AI-agent-first consumer profile permanent) → anchors Q-L-15 (code-fix providers PERMANENTLY dropped); diagnostic message quality elevated to compensate
- **PA-002** («без костылей» / no shortcuts) → anchors Q-L-9 (DFK010 dropped — methodology-layer not Roslyn scope); Q-L-11 (DFC001 К-L20 LOCK deferred — Bridge surface Mod-API-coupled); Q-L-13 (PublicApiAnalyzers audience-driven deferred)
- **PA-003** (architectural complexity always justified for clean execution) → anchors three-graph + two-scheduler analyzer rule shape; tiered DFK/DFL/DFC namespace per Q-L-3
- **PA-004** (К-L14 thesis preservation) → anchors substrate minimality discipline; this cascade expected К-L14 Evidence #14 (first analyzer implementation evidence)

User framing 2026-05-24 (verbatim): «Ни каких костылей, сложность архитектуры всегда оправдана, так как проект на долгие горизонты». PA-002 + PA-003 manifest in this cascade.

### 0.3 — К-L14 thesis preservation framing

This cascade is **К-L14 Evidence #14 candidate** — first analyzer implementation evidence. Evidence type categorization per K_L14_EVIDENCE_DASHBOARD framework:

| Evidence type | Cascade origin | Status |
|---|---|---|
| Type 1 — Clean additive (zero removal) | К-ext #3 (cascade #12) | EXISTS |
| Type 2 — Clean removal (zero substrate touch) | К-ext #2 (cascade #11) | EXISTS |
| Type 3 — Substrate refinement (deliberate К-L change) | К10 (cascade #10) | EXISTS |
| Type 4 — Bug-driven correction | A'.7.x bus cluster | EXISTS |
| Type 5 — Observational baseline (no production code change) | A'.9.0 (cascade #13) | EXISTS |
| Type 6 — **Tooling addition (analyzer/discipline infrastructure)** | **A'.9.1 (cascade #14)** | **NEW CATEGORY EXPECTED** |

К-L14 thesis: «substrate minimal; falsifiability tracked through defect rate, architectural integrity, pipeline economics». Tooling addition that enforces existing substrate invariants reduces drift surface without expanding substrate — К-L14 evidence-positive.

### 0.4 — Dependency on A'.9.0 reconnaissance + amendments + axiom codification

Brief A'.9.1 authoring strictly inherits:

1. **A'.9.0 Reconnaissance Report** (`docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` Tier 2 Live Category A — 335 KB / ~3300 lines)
   - 7-domain reconnaissance output authoritative
   - 22 К-L analyzability matrix (§3.1)
   - 12 FORMALIZE Lessons matrix (§4)
   - 10 cascade #2 + #3 surfaced rule candidates (§5)
   - 20 candidate DF020 sub-rules (§6.2)
   - Roslyn ecosystem state SDK 5.3.0 (§7)
   - Build/CI Option C hybrid placement (§8)
   - Suppression governance near-zero baseline (§9)
   - 10 Brief A'.9.1 prerequisites (§10)
   - 45 Q-K candidates (§11) + Q-K-46 (amendments log §4 added)

2. **A'.9.0 Amendments Log** (`docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md` Tier 4 AUTHORED Category D — 649 lines)
   - 4 amendments captured post-closure
   - §3.3 verbatim 13-rule first-batch (8 P0 + 5 P1 post-5-rule-deferral к К-L20 LOCK cascade)
   - §4 test exclusion principle (DFL025 family)

3. **Two-session deliberation batch 1+2** (this brief's deliberation surface)
   - Batch 1: Q-L-1..Q-L-7 ratifications (cascade designation + adaptive gate + namespaces + csproj + sln + CPM + categories)
   - Batch 2: Q-L-8..Q-L-17 + Axiom Option (VII) ratifications (DFK019 split + DFK010 drop + ReservedStub + DFC001 deferral + BannedApiAnalyzer drop + PublicApiAnalyzers deferral + DF→DFK rename + code-fix permanent drop + DFK016 Phase 0 + test infra naming + PROJECT_AXIOMS codification)

4. **PROJECT_AXIOMS.md v1.0 draft** (batch 2 session log §6.1 verbatim)
   - PA-001..PA-004 codification authoritative
   - Authored at Phase α Commit 8 of this cascade

### 0.5 — К-extensions cascade #5 vs A'.9.1 sub-cascade distinction

Per Q-L-1 ratification (batch 1) + Q-K-43 recon recommendation:
- **Single A'.9.1 cascade with ξ/χ/ψ phases** = default if χ violation count ≤80
- **Three sub-cascades A'.9.1a/b/c** = if χ violation count >150
- **Hybrid** = if 80-150 violations

Phase 0 empirical scan determines violation count → Phase α adaptive gate selects shape. Default forecast: single cascade (recon §1.1 «5 total suppressions» low-baseline + 13 rules first-batch focused scope).

---

## §1 — Milestone surface + Q-N lock surface

### 1.1 — Milestone scope

**A'.9 milestone scope** (multi-cascade):
- A'.9.0 — Reconnaissance (EXECUTED — К-ext #4)
- **A'.9.1 — Analyzer Infrastructure (THIS BRIEF — К-ext #5)**
- A'.9.2 — Severity promotion (cleanup → error) + optional code-fix providers (forward — K-ext #6) — **NOTE per Q-L-15: code-fix providers PERMANENTLY DROPPED; A'.9.2 scope becomes severity promotion only**
- A'.9.3+ — DC### cascade-derived rules + DL### Lesson-derived rules (forward — К-ext #7+)
- A'.9.N — M3.4 deferred analyzer milestones (per recon Domain 4)
- Post-A'.9 → V-extension cascade (per Crystalka «расширять V»)
- К-L20 LOCK cascade — DF020 family + 5 deferred Mod-API-coupled rules (DFK009/DFK012/DFK015/DFK018 + DFK020 family) + DFC001 Bridge surface activation

### 1.2 — A'.9.1 cascade scope

**A'.9.1 ships**:
- Analyzer infrastructure (csproj scaffolding netstandard2.0 + Roslyn SDK 5.3.0)
- Test infrastructure (`tests/DualFrontier.Analyzers.Tests/` per Q-L-17)
- Central Package Management (`Directory.Packages.props` per Q-L-6)
- 13 first-batch DFK### rules (8 P0 + 5 P1 per amendments log §3.3) + Phase 0 DFK016 decision
- 2 DFL### Lesson-derived rules (DFL025-A + DFL025-B per amendments log §4)
- [ReservedStub] attribute infrastructure per Q-L-10
- Mandatory annotation pass for cascade #3 deferred dispatch arms (3 sites)
- ANALYZER_RULES.md DF→DFK rename + structural reorganization (Q-L-14 two-commit)
- PROJECT_AXIOMS.md Tier 1 LOCKED v1.0 (Axiom Option (VII))
- DF999 self-policing rule per Q-L-18 (pending batch 3 governance)
- .editorconfig per-rule severity baseline at `suggestion` (cleanup phase mode)
- Phase χ cleanup-phase suppression sweep + CAPA cascade
- Phase ψ severity promotion `suggestion` → `error` (if violation count ≤80 per Q-L-1)

**A'.9.1 explicitly does NOT ship** (per Q-L ratifications batch 2):
- Code-fix providers (Q-L-15 PERMANENTLY DROPPED — PA-001 axiom anchor)
- PublicApiAnalyzers RS0016/RS0017/RS0024 (Q-L-13 audience-driven deferred — community ecosystem absent)
- BannedApiAnalyzer for Godot/Silk.NET (Q-L-12 dropped — closed historical concern)
- DFK009 К-L9 Vanilla=mods (deferred к К-L20 LOCK — amendments log §3)
- DFK010 К-L10 decision rule (Q-L-9 dropped — methodology-layer not Roslyn scope)
- DFK012 К-L12 native scheduler sovereignty (deferred к К-L20 LOCK — amendments log §3)
- DFK015 К-L15 bus capability declaration (deferred к К-L20 LOCK — amendments log §3)
- DFK018 К-L18 mod unload quiescence (deferred к К-L20 LOCK — amendments log §3)
- DFK019.B hardware tier capability check (Q-L-8 deferred к hardware tier expansion cascade)
- DFK020 family (20 sub-rules per recon §6.2 — К-L20 LOCK cascade)
- DFC001 Bridge surface (Q-L-11 deferred к К-L20 LOCK — Mod-API-coupled)
- DFC### other cascade-derived rules (A'.9.3+ cascade)
- DL### Lesson-derived rules other than DFL025-A/B (A'.9.3+ cascade)
- NetAnalyzers CA-prefix rules (recon Q-K-31 deferred separate brief)
- GitHub Actions CI infrastructure (recon Q-K-37 deferred outside A'.9 scope)

### 1.3 — Q-N lock surface (17 forward-locked Q-L decisions)

Per Q-L ratifications batch 1 + batch 2 deliberation sessions:

| Q-L | Decision | Forward-locked (cannot deviate in execution) |
|---|---|---|
| Q-L-1 | Single A'.9.1 cascade with ξ/χ/ψ phases if χ violations ≤80; three sub-cascades if >150; hybrid otherwise | YES |
| Q-L-2 | К-extensions cascade #5 + A'.9.1 milestone-internal dual designation | YES |
| Q-L-3 | Tiered namespaces DFK### / DFL### / DFC### | YES |
| Q-L-4 | Analyzer csproj `<TargetFramework>netstandard2.0</TargetFramework>` explicit override | YES |
| Q-L-5 | Scope limitation hybrid table per Q-K-§3-2 (Roslyn scope only for managed-side enforcement) | YES |
| Q-L-6 | Central Package Management adoption — `Directory.Packages.props` | YES |
| Q-L-7 | 3 active categories at A'.9.1: `DualFrontier.Architecture` + `DualFrontier.NativeBoundary` + `DualFrontier.Discipline` (NEW); `DualFrontier.ModSurface` reserved (no active rules) | YES |
| Q-L-8 | DFK019 split — DFK019.A static Vulkan API surface (A'.9.1) + DFK019.B hardware tier (deferred к hardware tier expansion cascade) | YES |
| Q-L-9 | DFK010 DROPPED — methodology-layer not Roslyn scope (PA-002 axiom anchor) | YES |
| Q-L-10 | [ReservedStub] attribute Phase α only; Purpose enum (BuildComposition, ArchitecturalSketch); mandatory Reason field; namespace `DualFrontier.Contracts.Analyzer` | YES |
| Q-L-11 | DFC001 merged with DFC001.A marker purity + DFC001.B record purity; DEFERRED к К-L20 LOCK cascade (Bridge surface = Mod API coupled) | YES |
| Q-L-12 | BannedApiAnalyzer DROPPED entirely — closed historical concern (Godot permanently removed cascade #2) | YES |
| Q-L-13 | PublicApiAnalyzers entirely deferred audience-driven — community ecosystem absent (PA-001 axiom anchor); activates when community emerges OR public API stability commitment OR specific cascade brief | YES |
| Q-L-14 | DF→DFK rename two-commit sequence (mechanical rename + structural reorganization); historical documents preserved per Option γ Hybrid | YES |
| Q-L-15 | Code-fix providers DROPPED PERMANENTLY — PA-001 axiom AI-agent-first consumer; diagnostic message quality elevated to compensate | YES |
| Q-L-16 | DFK016 Phase 0 empirical assessment per Option γ — decision criteria documented (α retain / β drop / Mod-API reclassification к К-L20 LOCK) | YES |
| Q-L-17 | Test infrastructure naming — `tests/DualFrontier.Analyzers.Tests/` plural matches convention + namespace tier + recon Option C hybrid | YES |
| Axiom Option (VII) | NEW `docs/governance/PROJECT_AXIOMS.md` Tier 1 LOCKED v1.0; PA-001..PA-004 codification; FRAMEWORK v1.1→v1.1.1 + SYNTHESIS_RATIONALE v1.0→v1.0.1 cross-reference patches | YES |

### 1.4 — Pending Q-N batch 3 (governance — deferred to next focused session)

Per Option D ratification batch 2 (§8 of session log), 10 Q-L candidates deferred к next focused session. Brief A'.9.1 execution proceeds WITHOUT batch 3 ratifications:

| Q-L | Decision pending | Default at execution |
|---|---|---|
| Q-L-18 | DF999 self-policing rule shipping timing (A'.9.1 vs A'.9.2) | Default (a) ship at A'.9.1 alongside first DFK### rules per recon Q-K-38 |
| Q-L-19 | [SuppressMessage] attribute form sanction timing | Default (b) A'.9.1 ships pragma-only; defer attribute form to first false-positive per recon Q-K-39 |
| Q-L-20 | Suppression CAPA-tracking tier | Default (c) hybrid site-scoped first, rule-scoped thereafter per recon Q-K-40 |
| Q-L-21 | Test-side fixture suppression CAPA-exempt policy | Default (c) blanket exempt + quarterly review per recon Q-K-41 |
| Q-L-22 | Carve-out attribute mandatory Justification | Default (a) mandatory per recon Q-K-42 |
| Q-L-23 | Documentation forward propagation plan | Default (a) per-artifact REGISTER enrollment per recon Q-K-45 |
| Q-L-24 | MOD_API_CONTRACT.md AUTHORED-SKELETON pre-authoring | DEFERRED entirely к К-L20 LOCK cascade per Crystalka direction §1.1 batch 2 |
| Q-L-25 | Analyzer csproj REGISTER enrollment Cat×Tier | Default Cat D × Tier 4 AUTHORED |
| Q-L-26 | Test exclusion principle formalization target | Default (c) defer formal codification к Brief A'.9.1 closure per recon Q-K-46 |
| Q-L-27 | KERNEL chronicle v2.5.4 entry timing | Default Phase δ governance cascade at A'.9.1 closure |

Batch 3 may be ratified mid-cascade if Crystalka direction surfaces; otherwise defaults apply per above. None of batch 3 blocks Phase α execution.

---

## §2 — Cascade architectural scope (ξ scaffolding + χ cleanup + ψ promotion phases)

### 2.1 — Three-phase progression overview

Per Q-L-1 ratification + recon §8.4 Stages 1-3:

**Phase α (ξ scaffolding)** — analyzer infrastructure + first-batch rule authoring:
- Wall-clock estimate: 60-90 min execution session
- Atomic commit count: 9 commits (per session log batch 2 §4.1)
- К-L14 evidence: tooling addition (Evidence #14 first analyzer implementation)
- Closure gate: `dotnet build` exit 0 (build passes even with suggestions surfacing); `dotnet test tests/DualFrontier.Analyzers.Tests` exit 0; existing test suite exit 0; `sync_register.ps1 -Validate` exit 0

**Phase β (χ cleanup)** — cleanup phase + violation triage:
- Wall-clock estimate: TBD post-Phase-0 violation count
- Atomic commit count: TBD (1 commit per ≥10 violations resolved or per-rule sub-batch)
- Per-diagnostic triage: (a) fix in code, (b) suppress with rationale `// DFK###-SUPPRESS: <citation>`, (c) rule refinement if false-positive
- Closure gate: zero DFK### diagnostics on src/ paths at suggestion severity (analyzer would emit zero warnings if promoted to error)

**Phase γ (ψ promotion)** — severity promotion `suggestion` → `error`:
- Wall-clock estimate: 15-30 min
- Atomic commit count: 1 commit (single .editorconfig edit)
- Per Q-L-1: only executes if χ violation count ≤80 (single A'.9.1 cascade); >150 splits to A'.9.1c; 80-150 hybrid per Crystalka decision
- Closure gate: `dotnet build` exit 0 with all DFK### at `severity = error`; this proves К-Lxx compile-time enforcement is live

**Phase δ (governance cascade)** — closure protocol + REGISTER cascade:
- Wall-clock estimate: 30 min
- Atomic commit count: 1-2 commits (closure report + REGISTER + audit_trail event)
- Closure gate: `sync_register.ps1 -Validate` exit 0; REGISTER cascade complete; K_EXTENSIONS_LEDGER §3.6 + K_L14_EVIDENCE_DASHBOARD #14 entries appended

### 2.2 — Adaptive gate decision tree (Phase 0 violation count)

Phase 0 produces violation count estimate via stub-implementation build:

```
violation_count = empirical Phase 0 dry-run

if violation_count ≤ 80:
    → Single A'.9.1 cascade with ξ/χ/ψ/δ phases
    → Total commits: 9 (Phase α) + N (Phase β per cleanup) + 1 (Phase γ) + 1-2 (Phase δ) = 11-15+ commits
    → Default expected path

elif violation_count ≤ 150:
    → Hybrid — A'.9.1 ships Phase α + Phase β subset; standalone A'.9.1b finishes χ + ψ
    → Crystalka decision at Phase 0 closure determines split point
    → Wall-clock: 90-120 min per sub-cascade

else (violation_count > 150):
    → Three sub-cascades A'.9.1a (scaffolding) / A'.9.1b (cleanup) / A'.9.1c (promotion)
    → Per-cascade commit count: ~5-10 each
    → Wall-clock: 60-90 min each
```

### 2.3 — Architectural reality recap (three-graph + two-scheduler model)

Per session log batch 2 §0.3 empirical reads (sixth application Lesson #N14):

| # | Graph | Owner | Purpose | Rebuild trigger | Analyzer touch? |
|---|---|---|---|---|---|
| 1 | Native SystemGraph | `system_graph.h/cpp` | Authoritative post-К10.1; static + per-tick modes | System add/remove via mod load/unload | DFK013 (К-L13 wake_type discipline) — managed-side detection only |
| 2 | Managed DependencyGraph | `DependencyGraph.cs` | Adapter-facade role post-К10.1 Commit 14 | `ParallelSystemScheduler.Rebuild()` only from menu | DFK005 (К-L5 declarative bootstrap) — multi-bootstrap detection |
| 3 | Mod manifest dependency graph | `ModIntegrationPipeline.TopoSortByPredicate` | Inter-mod load ordering (shared + regular cycle detection) | Each `Apply()` invocation | Outside Roslyn scope (manifest layer) — per Q-L-11 К-L20 LOCK deferred |

**Per-tick flow (post-К10.1)**:
1. Native scheduler tick boundary
2. Native wake_registry fires runqueue (5 wake types — К-L13 on-demand activation)
3. Native SystemGraph.compute_per_tick_graph(runnable_ids) → Kahn on runnable subset
4. Native scheduler dispatches batches per phase via batched reverse-P/Invoke
5. Managed ManagedSystemDispatcher.OnBatch invokes BatchExecutor (SystemBase[] indexed table)
6. Native scheduler advances tick

**Analyzer reachable**: managed-side artifacts (5 src projects of 12 — Core + Application + Application.Scheduler + Application.Modding + Core.Scheduling). Native + manifest layers OUTSIDE Roslyn scope per Q-L-5.

### 2.4 — Atomic commit discipline (Phase α 9 commits — per session log batch 2 §4.1)

Each Phase α commit follows scope-prefix format `analyzer(<scope>): <description>` per cascade #2 + #3 atomic commit pattern (Lesson #N13 commit integrity verification):

| # | Scope | Description | Files touched | Lines (estimate) |
|---|---|---|---|---|
| 1 | `analyzer(csproj)` | Analyzer csproj scaffolding per Q-L-4 | `tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj` (NEW) | ~40 |
| 2 | `analyzer(tests)` | Analyzer tests csproj scaffolding per Q-L-17 | `tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj` (NEW) | ~50 |
| 3 | `analyzer(cpm)` | Central Package Management adoption per Q-L-6 | `Directory.Packages.props` (NEW) + ~30 csproj migrations | ~50 + ~30 modifications |
| 4 | `docs(rename)` | ANALYZER_RULES.md mechanical DF→DFK rename per Q-L-14 Commit 1 | `docs/architecture/ANALYZER_RULES.md` + scan output | ~200 mechanical |
| 5 | `docs(restructure)` | ANALYZER_RULES.md structural reorganization per Q-L-14 Commit 2 | `docs/architecture/ANALYZER_RULES.md` | ~400-600 (scope split §4-§9) |
| 6 | `analyzer(reservedstub)` | [ReservedStub] attribute infrastructure per Q-L-10 | `src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs` (NEW) + `ReservedStubPurpose.cs` (NEW) | ~80 |
| 7 | `analyzer(annotations)` | Mandatory annotation pass for cascade #3 deferred dispatch arms | 3 sites (HandlePawnState/HandleItemSpawned/HandleTickAdvanced) + ~5 other reserved-stubs surfaced Phase 0 | ~15-25 |
| 8 | `governance(axioms)` | PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED per Axiom Option (VII) | `docs/governance/PROJECT_AXIOMS.md` (NEW) + REGISTER cascade | ~250 doc + ~50 REGISTER |
| 9 | `governance(crossrefs)` | FRAMEWORK + SYNTHESIS_RATIONALE PATCH-level cross-reference additions | `docs/governance/FRAMEWORK.md` v1.1→v1.1.1 + `docs/governance/SYNTHESIS_RATIONALE.md` v1.0→v1.0.1 + REGISTER | ~10 + ~10 + ~30 REGISTER |

**Total Phase α**: ~1300 LOC across 9 atomic commits. Commit ordering: 1→2→3 (build infrastructure) → 4→5 (docs rename two-step per Q-L-14) → 6→7 (attribute + annotation) → 8→9 (governance axiom + crossrefs).

**Pre-flight discipline per commit**: grep AD (Architecture Discipline — file rename + structural changes) per session log batch 2 §3.5 «pre-flight grep AD» protocol; `sync_register.ps1 -Validate` exit code 0 before REGISTER cascade commits.

---

## §3 — К-L14 framework + evidence target

### 3.1 — К-L14 thesis preservation discipline

Per PA-004 axiom + K_CLOSURE §2.20 К-L14 canonical text:

> «Performance derives from clean complex architecture (meta-invariant): substrate minimal; falsifiability tracked through defect rate, architectural integrity, pipeline economics.»

**A'.9.1 cascade К-L14 evidence framing**:
- **Type 6 NEW category** — tooling addition that enforces existing substrate invariants
- Substrate touch: **zero K-Lxx invariant text change**; no К-L additions/removals/refinements
- Falsifiability mechanism: **automated invariant enforcement** at build time (analyzer rule failures replace manual cross-document audit)
- Defect rate: expected zero post-cleanup (Phase β closure gate = zero src/ violations)
- Architectural integrity: enforced at compile time vs prior manual document attestation
- Pipeline economics: per-commit analyzer pass adds ~1-3s to `dotnet build` (acceptable per CI gate cadence)

### 3.2 — Evidence #14 — first analyzer implementation

К-L14 Evidence #14 target — first analyzer implementation evidence. Recording template per K_L14_EVIDENCE_DASHBOARD.md convention:

```yaml
verification_number: 14
date: "2026-MM-DD"
cascade: "A'.9.1 / К-extensions cascade #5"
evidence_type: "Type 6 — Tooling addition (NEW category)"
substrate_touch: "Zero — no K-Lxx invariant text change"
clean_status: "CLEAN"

evidence_summary: |
  First analyzer implementation cascade. Ships ~13-15 DFK### rules + 2 DFL### 
  rules + [ReservedStub] attribute infrastructure + PROJECT_AXIOMS Tier 1 LOCKED.
  Substrate completely untouched (S-LOCK-1 zero K-Lxx text change).
  Falsifiability mechanism shifts from manual cross-document audit to automated 
  build-time invariant enforcement.

falsifiability_metric_shift:
  before: "Manual cross-document audit per cascade closure (Lesson #N14 empirical state coverage)"
  after:  "Automated compile-time analyzer pass (DFK### error severity post-Phase γ)"

defect_rate_baseline:
  pre_cascade: "<measured baseline at A'.9.0 closure>"
  post_cascade: "<measured post-Phase γ baseline>"
  expected_change: "Zero new defects introduced; existing defects surfaced by cleanup phase"

architectural_integrity_metric:
  before: "Trust-by-discipline (briefs, deliberation sessions, code reviews)"
  after:  "Trust-by-enforcement (analyzer rule errors block builds)"

pipeline_economics:
  build_time_impact: "+1-3s per `dotnet build` invocation (analyzer pass)"
  test_time_impact:  "+5-10s per `dotnet test` invocation (DualFrontier.Analyzers.Tests suite)"
  pipeline_acceptable: true
  rationale: "Per-commit analyzer pass amortizes manual audit cost; immediate feedback loop superior to post-cascade audit"
```

### 3.3 — Forward К-L14 evidence considerations

**Evidence #15+ candidates surfaced by A'.9.1 cascade**:

- **К-L14 Evidence #15 candidate** — DFK016 Phase 0 empirical decision (retain α / drop β / Mod-API reclassification к К-L20 LOCK). If retained α, Phase β contribution; if dropped β, evidence of К-L14 substrate-minimization principle (rule removed via empirical assessment); if Mod-API reclassification, evidence aligned with К-L20 LOCK cascade scope.

- **К-L14 Evidence #16 candidate** — Phase γ severity promotion if executed (`suggestion` → `error` single-file diff proves cleanup completeness).

- **К-L14 Evidence #17 candidate** — METHODOLOGY v1.13+ Lesson #25 refined extension OR Lesson #N17 codification at A'.9.1 closure (per Q-L-26 batch 3 default).

---

## §4 — Prerequisites (read deps + verification reqs)

### 4.1 — Phase 0 mandatory reads (14 sources)

Per Lesson #N14 «empirical state coverage» discipline (seventh+ application). Read access pattern: direct `Filesystem:read_text_file` from `D:\Colony_Simulator\Colony_Simulator\` repo path — both Tier 1 LOCKED governance + Tier 2 Live authoritative surfaces available without chat-transfer staging.

**Critical authoritative sources (read first):**

| # | Path | Tier | Size | Read scope |
|---|---|---|---|---|
| 1 | `docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` | T2 Live A | 335 KB | head 200 + targeted view_range (§3 К-L matrix, §5 rule candidates, §6 К-L20 prep, §7 Roslyn ecosystem, §8 Build/CI, §9 suppression, §10 prerequisites, §11 Q-K candidates) |
| 2 | `docs/governance/REGISTER.yaml` | T1 LOCKED A | 524 KB | head 200 + targeted view_range (register_version state, audit_trail tail, capa_entries inventory, DOC enrollment patterns) |
| 3 | `docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md` | T4 AUTHORED D | ~30 KB | Full read (4 amendments, §3.3 13-rule first-batch verbatim, §4 test exclusion principle) |
| 4 | `docs/architecture/ANALYZER_RULES.md` v0.1 AUTHORED-SKELETON | T4 AUTHORED D | ~10 KB | Full read baseline pre-Q-L-14 rename + Q-L-7 category restructure |
| 5 | `docs/architecture/K_CLOSURE_REPORT.md` §7 | T1 LOCKED A | ~80 KB | Section read targeted via view_range (§7 analyzer rule specifications canonical) |
| 6 | `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 | T1 LOCKED A | ~100 KB | Section read targeted via view_range (Part 0 К-L invariants 1-21 + v2.5.3 chronicle) |
| 7 | `docs/methodology/METHODOLOGY.md` v1.12 | T1 LOCKED A | ~50 KB | Lesson #N12 + Lesson #N14 + Lesson #25 refined sections |
| 8 | `src/DualFrontier.Application/Loop/GameBootstrap.cs` | Production | ~varies | Full read (transitional-state comment surface; coreSystems enumeration). Path corrected per Phase 0 closure §3.1 F1. |
| 9 | `src/DualFrontier.Core.Interop/Bootstrap.cs` | Production | ~varies | Full read (transitional-state comment surface; bootstrap_graph.h consumer). Path corrected per Phase 0 closure §3.1 F1. |
| 10 | Native scheduler headers (8 files) | Native (cross-ref only) | ~varies | Reference-only — already empirically read batch 2 §0.2 inventory |
| 11 | Managed scheduler files (7 files) | Production | ~varies | Reference-only — already empirically read batch 2 §0.2 inventory |
| 12 | `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` v0.1 AUTHORED-SKELETON | T4 AUTHORED D | ~10 KB | Full read + disposition decision (supersede/merge/revise per recon §10 Prerequisite 9) |
| 13 | `docs/governance/FRAMEWORK.md` v1.1 | T1 LOCKED A | ~varies | Section read §7.2 Tier 1 LOCKED amendment protocol + §13 cross-references target |
| 14 | `docs/governance/SYNTHESIS_RATIONALE.md` v1.0 | T1 LOCKED A | ~varies | Section read §7 cross-references addition target |

**Phase 0 closure protocol**: Each read must produce explicit cite in Brief §1 (rule enumeration) OR §4 (prerequisites) OR §9 (closure protocol REGISTER cascade). No read «idle consultation» — every read justifies appearance via Brief artifact line attribution per METHODOLOGY discipline.

### 4.2 — Phase 0 mandatory empirical scans (7 tasks)

Per session log batch 2 §4.2:

**Task 1 — DFK016 К-L16 pipeline depth detection feasibility** (Q-L-16 Option γ):
- Grep codebase для `PipelineDepthSetting` / `pipeline_depth` / `D parameter`:
  - `grep -rni "pipeline_depth\|PipelineDepth\|pipeline depth" src/ native/`
  - `grep -rni "Phase.Compute\|Phase_Compute" src/ native/`
- Identify hardcoded D values vs canonical config paths
- Identify mod-side D access (would trigger Mod-API-coupling reclassification)
- Output: Q-L-16 final α/β/Mod-coupled decision documented in Phase 0 closure
- ANALYZER_RULES.md positioning updated per decision:
  - If α: §4 «A'.9.1 active rules — Phase β secondary»
  - If β: §7 «Outside Roslyn scope»
  - If Mod-coupled: §5 «К-L20 LOCK cascade deferred»

**Task 2 — DFK013 wake_type declaration discipline scope determination**:
- Re-read `wake_registry.h` + managed adapters
- Identify detection pattern для Initialize() eager-allocation anti-patterns
- Determine wake_type declaration mechanism (attribute vs registration call)
- Empirical: scan existing systems для wake_type setting patterns
- Output: DFK013 detection rule shape final scope

**Task 3 — PROJECT_AXIOMS.md draft refinement against codebase reality**:
- Verify PA-001 anchor references against FRAMEWORK.md §0 verbatim
- Verify PA-002 anchor references against Crystalka direction history (briefs, sessions)
- Verify PA-003 anchor references against cascade #1+ architectural decisions
- Verify PA-004 anchor references against KERNEL Part 0 + K_L14_EVIDENCE_DASHBOARD.md
- Output: PROJECT_AXIOMS.md final draft ready for Phase α Commit 8

**Task 4 — Lesson #N17 candidate documentation**:
- Document candidate в `A_PRIME_7_X_LESSON_CANDIDATES.md` (existing project file)
- 5 empirical applications enumerated (per session log batch 2 §5.1):
  1. Code-fix providers — Q-L-15 (PA-001 axiom permanent)
  2. PublicApiAnalyzers — Q-L-13 (community ecosystem absent)
  3. BannedApiAnalyzer — Q-L-12 (closed concern)
  4. DFK019.B hardware tier — Q-L-8 split (multi-hardware-tier audience absent)
  5. DFK016 threshold customization API — Q-L-16 deliberation reasoning (multi-hardware-tier audience)
- Promotion criterion: second formal application in subsequent cascade
- Provisional status, awaiting Brief A'.9.1 closure formal codification per Q-L-26 default

**Task 5 — Standard Phase 0 mandatory reads per Lesson #N14**:
- GameBootstrap.cs + Bootstrap.cs full reads (transitional-state comments surfacing)
- K_CLOSURE_REPORT.md §7 (analyzer rule specifications canonical)
- ANALYZER_RULES.md v0.1 baseline (pre-rename state)
- All ratified Q-L decisions cross-referenced against actual code state

**Task 6 — DF→DFK rename empirical scope determination**:
- `grep -r "DF[0-9]" docs/ --include="*.md"` — count occurrences
- `grep -r "DF[0-9]" tools/briefs/ --include="*.md"` — count occurrences
- Determine total rename scope estimate (lines, files, references)
- Compare against current state — last empirical estimate ~195 references; verify actual
- Output: Phase α Commit 4 mechanical rename scope

**Task 7 — Phase β cleanup violation count estimate** (per Q-L-1 adaptive gate):
- Build analyzer с 13-15 active rules implemented as stubs (return empty diagnostic each)
- Wire к main solution per Q-L-6 CPM + Q-L-2 §2.2 of recon (Directory.Build.props centralized reference)
- Dry-run violation enumeration across 12 src projects + tests/* (excluding Fixture.*)
- Counts:
  - If ≤80 violations: Q-L-1 (a) single cascade с ξ/χ/ψ phases
  - If >150 violations: Q-L-1 (b) three sub-cascades A'.9.1a/b/c
  - If 80-150 violations: Q-L-1 (c) hybrid (Crystalka decision)
- Output: Phase α exit determines Phase β shape

### 4.3 — Verification requirements

**Pre-Phase-α verification**:
1. Filesystem MCP access pattern verified — `Filesystem:list_allowed_directories` returns `D:\Colony_Simulator\Colony_Simulator`
2. Repo state verified — `.git/HEAD` + `.git/refs/heads/main` + `.git/logs/HEAD` reads via Filesystem MCP
3. Branch verification: post-amendments-log commit baseline (check audit_trail.last_commit for current SHA reference)
4. Pre-existing analyzer artifacts confirmed — `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` v0.1 + `docs/architecture/ANALYZER_RULES.md` v0.1
5. Pre-existing CPM state — `Directory.Packages.props` absent (CPM not yet adopted; Q-L-6 introduces)
6. Pre-existing analyzer csproj state — `tools/DualFrontier.Analyzers/` absent

**Per-commit verification** (Phase α):
- `dotnet build` exit 0 (incremental — analyzer adds но does не block existing builds at suggestion severity)
- For commits touching csproj: `dotnet restore` clean
- For commits touching REGISTER: `pwsh tools/governance/sync_register.ps1 -Validate` exit 0
- For commits touching analyzer rule code: `dotnet test tests/DualFrontier.Analyzers.Tests --filter "Category!=ReservedStub"` exit 0

**Phase α exit verification**:
- All 9 commits clean
- `dotnet build` full solution exit 0
- `dotnet test` Modding suite + DualFrontier.Analyzers.Tests exit 0
- `sync_register.ps1 -Validate` exit 0
- Phase β violation count empirically determined (Task 7)
- PROJECT_AXIOMS.md authored Tier 1 LOCKED

**Phase β exit verification**:
- Zero DFK### / DFL### diagnostics at `suggestion` severity on src/ paths
- Suppression CAPA cascade complete (per Q-L-20 default (c) hybrid site-scoped first + rule-scoped thereafter)
- Suppression sweep log entry: «Suppression sweep: N total (M src, K tests), 0 unjustified, J open CAPAs»

**Phase γ exit verification**:
- `.editorconfig` per-rule severity = error for all 15 (13 first-batch + 2 DFL) DFK###/DFL### rules
- `dotnet build` exit 0 (cleanup proven complete)
- К-L14 Evidence #14 entry в K_L14_EVIDENCE_DASHBOARD.md

**Phase δ exit verification**:
- KERNEL_ARCHITECTURE.md v2.5.3 → v2.5.4 chronicle entry per Q-L-27 default
- K_EXTENSIONS_LEDGER §3.6 entry (К-ext cascade #5)
- K_L14_EVIDENCE_DASHBOARD.md #14 entry
- METHODOLOGY v1.12 → v1.13 (if Lesson #25 refined extension OR #N17 codified per Q-L-26 default (c))
- REGISTER.yaml register_version 2.7 → 2.8 (axiom enrollment) → 2.9 (analyzer csproj enrollments) → 2.10 (closure cascade event)
- `sync_register.ps1 -Validate` exit 0

---

## §5 — S-LOCK enumeration

Per cascade #2 + #3 S-LOCK precedent — explicit immutable architectural constraints that bound cascade execution.

### S-LOCK-1 — Zero substrate touch (К-L14 thesis preservation)

**Constraint**: No K-Lxx invariant text change in `KERNEL_ARCHITECTURE.md` Part 0. К-L1..К-L19 + К-L3.1 + К-L7.1 + К-L15.1 verbatim text preserved.

**Verification**: `git diff main -- docs/architecture/KERNEL_ARCHITECTURE.md` shows ONLY chronicle/changelog entries (Part 0 invariants table untouched).

**Rationale**: A'.9.1 cascade enforces existing substrate; cascade scope is tooling addition not substrate refinement.

**Exception**: NONE. Substrate change requires explicit deliberation cascade (e.g., К10 substrate refinement cascade) — A'.9.1 has no mandate for substrate change.

### S-LOCK-2 — Analyzer scope ≤ managed-side enforcement (Q-L-5)

**Constraint**: Roslyn analyzer rules detect violations in managed C# code only. Native-side К-L invariants (К-L1 C++20 dialect, К-L8 native owns storage, К-L15.1 Layer 3 compile-time isolation native facade, К-L19 hardware GPU) enforced by alternative mechanisms (CMake, pre-commit hook, runtime probe, hardware capability check).

**Verification**: Each DFK### rule's detection logic operates on `SyntaxTree` / `SemanticModel` from `Microsoft.CodeAnalysis.CSharp`. No rule attempts native-side enforcement.

**Rationale**: Per recon Q-K-2 — Roslyn analyzer scope is bounded. Native-side tooling deferred to post-A'.9 cascade.

**Exception**: NONE. Native-side rule attempts at A'.9.1 require explicit deliberation amendment.

### S-LOCK-3 — Path α discipline (deliberation → authoring → execution)

**Constraint**: Brief A'.9.1 authoring follows Path α discipline per Q-J-8 budget — deliberation session(s) → authoring session → execution session(s) as separate context windows.

**Verification**: Brief A'.9.1 authored from session log batch 1 + 2 + recon report + REGISTER state (this artifact authoring); execution session reads brief + Phase 0 + executes Phase α.

**Rationale**: Q-J-8 budget per Lesson #N12 + cascade #3 precedent. Single 1M context window per cascade preferred (per session log memory).

**Exception**: NONE in this brief authoring. Mid-cascade Crystalka direction may amend (precedent: Crystalka direction §1.1 batch 2 deferred Q-L-11 + Q-L-13).

### S-LOCK-4 — First-batch rule count 13 + 2 = 15 (Amendment #3 + Amendment #4)

**Constraint**: A'.9.1 first-batch active rules = 13 DFK### (8 P0 + 5 P1) + 2 DFL### (DFL025-A + DFL025-B) + 1 self-policing (DF999 per Q-L-18 default) + Phase 0 conditional (DFK016 retain α / drop β / Mod-API reclassify к К-L20 LOCK).

**Verification per Phase α completion**:
- `dotnet build` exit 0 with rule count 15-16 active
- `dotnet test tests/DualFrontier.Analyzers.Tests` exit 0
- ANALYZER_RULES.md §4 «A'.9.1 active rules» section enumerates exactly 15 + Phase 0 conditional

**Rationale**: Per amendments log §3.3 + §4 + Q-L-1 adaptive gate. 5 rules deferred to К-L20 LOCK cascade (Mod-API-coupled per Crystalka direction §1.1 batch 2).

**Exception**: NONE for first-batch count. Phase 0 DFK016 decision modifies +0 or +1 only.

### S-LOCK-5 — Test infrastructure naming plural (Q-L-17)

**Constraint**: Test project named `tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj` (plural matches `tests/*.Tests/` convention + namespace tier Q-L-3 + recon §8.1 Option C hybrid).

**Verification**: Project path exists at `tests/DualFrontier.Analyzers.Tests/`; sln file NestedProjects entry maps к `{22222222-…}` tests Solution Folder; closure-protocol `tests/DualFrontier.*.Tests` glob auto-picks-up.

**Rationale**: Convention alignment + no protocol change required.

**Exception**: NONE.

### S-LOCK-6 — Code-fix providers PERMANENTLY ABSENT (Q-L-15 + PA-001)

**Constraint**: No code-fix provider infrastructure ships in this cascade OR ever in future cascades. `Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.XUnit` NOT added к CPM. No `CodeFixProvider` subclasses authored.

**Verification**: `grep -r "CodeFixProvider\|CodeFix" tools/DualFrontier.Analyzers/ tests/DualFrontier.Analyzers.Tests/` returns 0 matches.

**Rationale**: PA-001 axiom — AI-agent-first consumer profile permanent. Code-fix providers serve human IDE workflow; AI agent reads diagnostic text directly. Diagnostic message quality elevated к compensate (rich text guiding AI agent к edit).

**Exception**: NONE. Audience materialization (community emergence) would re-trigger Q-L deliberation — at that point, PROJECT_AXIOMS.md PA-001 amendment per FRAMEWORK §7.2 Tier 1 LOCKED amendment protocol required FIRST.

### S-LOCK-7 — PublicApiAnalyzers absent (Q-L-13 + PA-001 + PA-002)

**Constraint**: `Microsoft.CodeAnalysis.PublicApiAnalyzers` package NOT added к CPM. No RS0016/RS0017/RS0024 enforcement infrastructure ships.

**Verification**: `Directory.Packages.props` does NOT contain `<PackageVersion Include="Microsoft.CodeAnalysis.PublicApiAnalyzers" ... />`.

**Rationale**: Audience-driven deferral. Community ecosystem absent (PA-001 anchor). All candidate assemblies (Contracts/Application/Bridge/Launcher) = Mod API surface volatile pre-К-L20 LOCK OR not mod-facing.

**Exception**: NONE. Activation conditions documented в ANALYZER_RULES.md §8 «Audience-driven deferred»: community emergence OR public API stability commitment OR specific cascade brief.

### S-LOCK-8 — BannedApiAnalyzer absent (Q-L-12 + PA-002)

**Constraint**: No BannedApiAnalyzer package adopted. No Godot/Silk.NET namespace ban rule ships.

**Verification**: `Directory.Packages.props` does NOT contain BannedApiAnalyzer reference; `BannedSymbols.txt` does NOT exist в repo.

**Rationale**: Closed historical concern. Godot permanently removed cascade #2. Documentation discipline (DualFrontier.Contracts README) sufficient. No external re-introduction risk audience exists.

**Exception**: NONE.

### S-LOCK-9 — Mod-API-coupled rules deferred к К-L20 LOCK (amendments log §3 + Crystalka direction §1.1)

**Constraint**: Following rules NOT shipped in A'.9.1:
- DFK009 (К-L9 Vanilla=mods)
- DFK012 (К-L12 native scheduler sovereignty)
- DFK015 (К-L15 bus capability declaration)
- DFK018 (К-L18 mod unload quiescence)
- DFK020 family (20 sub-rules per recon §6.2)
- DFC001.A + DFC001.B (Bridge surface — IRenderCommand marker + Bridge Command record purity)

**Verification**: ANALYZER_RULES.md §5 «К-L20 LOCK cascade deferred» enumerates all 6 rule groups. No analyzer source file implements detection logic for these IDs.

**Rationale**: Mod API surface volatile pre-К-L20 LOCK. Pre-emptive enforcement against moving target = kostyl pattern (PA-002 violation). К-L20 LOCK cascade activates entire deferred set.

**Exception**: NONE. К-L20 LOCK cascade timing per K_CLOSURE §9.5 Q1-Q8 deliberation (post-A'.9 milestone).

### S-LOCK-10 — DFK010 PERMANENTLY DROPPED (Q-L-9 + PA-002)

**Constraint**: No DFK010 К-L10 decision rule ships in A'.9.1 OR future cascades. К-L10 methodology-layer enforcement NOT Roslyn scope.

**Verification**: ANALYZER_RULES.md §7 «Outside Roslyn scope» enumerates DFK010 + DFK014 (К-L14 meta-invariant).

**Rationale**: К-L10 governs decision attribution at document/methodology layer, NOT code layer. Code-layer marker masquerades doc-layer reasoning = kostyl pattern (PA-002 violation). Overrides recon Option (c) «defer pending decision-context attribute».

**Exception**: NONE. К-L10 enforcement via FRAMEWORK + METHODOLOGY documentation discipline only.

### S-LOCK-11 — DFK019 split discipline (Q-L-8)

**Constraint**: DFK019 splits into DFK019.A (static Vulkan API surface, ships A'.9.1) + DFK019.B (hardware tier capability runtime check, deferred к hardware tier expansion cascade).

**Verification**: ANALYZER_RULES.md §4 «A'.9.1 active rules» lists DFK019.A; §6 «Hardware tier expansion cascade deferred» lists DFK019.B.

**Rationale**: Recon §3.1 К-L19 row split recognition. Static API-shape detection at managed-side (Vulkan 1.2-or-earlier API surface usage + async compute queue fallback bypass). Hardware tier capability requires runtime probe — deferred per Crystalka direction §1.6.

**Exception**: NONE.

### S-LOCK-12 — [ReservedStub] attribute Phase α infrastructure (Q-L-10)

**Constraint**: `[ReservedStub]` attribute infrastructure ships at Phase α Commit 6. Purpose enum (BuildComposition, ArchitecturalSketch) + mandatory Reason field. Namespace `DualFrontier.Contracts.Analyzer` (mod-accessible per К-L9). Mandatory annotation pass for cascade #3 deferred dispatch arms (3 sites: HandlePawnState/HandleItemSpawned/HandleTickAdvanced).

**Verification**: 
- `src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs` exists
- `src/DualFrontier.Contracts/Analyzer/ReservedStubPurpose.cs` exists
- 3+ sites annotated with `[ReservedStub(BuildComposition, "...")]`
- DFL025-A detection logic recognizes `[ReservedStub]`-tagged types

**Rationale**: DFL025 family detection requires [ReservedStub] anchor (per amendments log §4 mechanism Q4 (b)). Phase α infrastructure unlocks DFL025 enforcement.

**Exception**: NONE.

### S-LOCK-13 — DF→DFK rename two-commit (Q-L-14)

**Constraint**: DF→DFK rename executes as 2 atomic commits at Phase α — Commit 4 mechanical (identifier-only rename) + Commit 5 structural (scope split per Q-K-47 (a) + Q-L-7 categories). Historical documents preserved (recon report + amendments log + K_CLOSURE_REPORT + KERNEL chronicle) — not edited retroactively per Option γ Hybrid.

**Verification**:
- Commit 4 git diff shows only identifier rename (no structural changes)
- Commit 5 git diff shows §4-§9 restructure
- Forward governance docs (KERNEL chronicle, LEDGER §3.6, METHODOLOGY v1.13+, Brief A'.9.1) use DFK### from authoring time
- `grep -r "\bDF0[0-9][0-9]\b" docs/` returns 0 outside historical artifacts (recon report, amendments log, K_CLOSURE_REPORT)

**Rationale**: Mechanical separation simplifies review + supports rollback. Structural changes batched. Historical preservation per Option γ Hybrid.

**Exception**: NONE.

### S-LOCK-14 — PROJECT_AXIOMS.md Tier 1 LOCKED v1.0 ships at Phase α (Axiom Option (VII))

**Constraint**: `docs/governance/PROJECT_AXIOMS.md` v1.0 Tier 1 LOCKED ships at Phase α Commit 8. PA-001..PA-004 codification per session log batch 2 §6.1 verbatim draft.

**Verification**:
- `docs/governance/PROJECT_AXIOMS.md` exists with v1.0 frontmatter
- PA-001..PA-004 sections present
- REGISTER.yaml DOC-A-PROJECT_AXIOMS enrolled
- FRAMEWORK + SYNTHESIS_RATIONALE cross-references added (Phase α Commit 9)

**Rationale**: Single responsibility per governance document (Crystalka direction §1.4 batch 2). Predictable AI agent discovery location. Three-document governance trio at `docs/governance/` complete (FRAMEWORK + SYNTHESIS_RATIONALE + PROJECT_AXIOMS).

**Exception**: NONE. Axiom additions require FRAMEWORK §7.2 amendment protocol post-LOCK.

---

## §6 — Phase α atomic commit specifications

### 6.1 — Commit 1 — `analyzer(csproj): scaffolding per Q-L-4`

**Scope**: Analyzer csproj scaffolding (netstandard2.0 mandatory override per recon §7.1.2)

**Files touched**:
- `tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj` (NEW, ~40 LOC)
- `DualFrontier.sln` (MODIFIED — Project + NestedProjects entries)

**Content specification**:

```xml
<!-- tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <!-- CRITICAL: override Directory.Build.props net8.0 default per Q-L-4 + S-LOCK-2 + recon §7.1.2 -->
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    
    <!-- Roslyn analyzer machinery -->
    <IsRoslynComponent>true</IsRoslynComponent>
    <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
    
    <!-- Internal-only — no NuGet publication per recon §7.4.4 -->
    <IsPackable>false</IsPackable>
    
    <!-- Inherit Nullable=enable, ImplicitUsings=enable from Directory.Build.props -->
    <!-- TreatWarningsAsErrors inherits (true) -->
    
    <!-- Documentation file inherits (true) — RS1015 helpLinkUri enforced per recon §7.1.5 -->
  </PropertyGroup>
  
  <!-- Per Q-L-6 CPM, package versions resolved via Directory.Packages.props -->
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" PrivateAssets="all" />
    <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" PrivateAssets="all" />
  </ItemGroup>
  
  <!-- DualFrontier.Contracts reference для [ReservedStub] attribute access — per Q-L-10 + S-LOCK-12 -->
  <ItemGroup>
    <ProjectReference Include="..\..\src\DualFrontier.Contracts\DualFrontier.Contracts.csproj" 
                      OutputItemType="" 
                      ReferenceOutputAssembly="false" />
  </ItemGroup>
  
</Project>
```

**sln modifications**:
1. Add Project entry под tools Solution Folder `{07C2787E-…}`:
   ```
   Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "DualFrontier.Analyzers", "tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj", "{<NEW-GUID-1>}"
   EndProject
   ```
2. Add 6-config matrix in GlobalSection(ProjectConfigurationPlatforms):
   ```
   {<NEW-GUID-1>}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
   {<NEW-GUID-1>}.Debug|Any CPU.Build.0 = Debug|Any CPU
   ... (Release|Any CPU, Debug|x64, Release|x64, Debug|x86, Release|x86 пары)
   ```
3. Add NestedProjects entry mapping new project к tools folder:
   ```
   {<NEW-GUID-1>} = {07C2787E-EAC7-C090-1BA3-A61EC2A24D84}
   ```

**Verification**:
- `dotnet restore` clean exit 0
- `dotnet build tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj` exit 0 (empty analyzer — no DFK### implementations yet, just project scaffolding)
- `git diff DualFrontier.sln` shows 1 Project entry + 6 config entries + 1 NestedProjects entry

**Pre-flight grep AD**: 
- `grep -r "DualFrontier.Analyzers" tools/` returns expected occurrences only (this csproj)
- No accidental editing of other `tools/` projects (ManifestRewriter sanity check)

**Commit message**:
```
analyzer(csproj): scaffolding netstandard2.0 per Q-L-4 + recon §7.1.2

Adds tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj —
the Roslyn analyzer project hosting DFK###/DFL### rule implementations.

Per Q-L-4 ratification (batch 1 deliberation 2026-05-24): TargetFramework
explicit override to netstandard2.0 (NOT inherited net8.0 — Roslyn analyzer
host loads compatibility requires netstandard2.0 per recon §7.1.2).

Per Q-L-6 CPM: PackageReference uses Directory.Packages.props version
resolution (introduced Commit 3 of this cascade).

Per Q-L-10 + S-LOCK-12: ProjectReference к DualFrontier.Contracts для
[ReservedStub] attribute access (attribute introduced Commit 6).

References:
- recon §7.1.2 netstandard2.0 mandatory
- recon §8.1 Option C hybrid placement (tools/ for analyzer, tests/ for tests)
- session log batch 2 §4.1 Commit 1
- К-L14 thesis: tooling addition, zero substrate touch
```

### 6.2 — Commit 2 — `analyzer(tests): csproj scaffolding per Q-L-17`

**Scope**: Analyzer tests csproj scaffolding (net8.0 + Microsoft.CodeAnalysis.Testing.XUnit per recon §7.2.4)

**Files touched**:
- `tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj` (NEW, ~50 LOC)
- `DualFrontier.sln` (MODIFIED — Project + NestedProjects entries)

**Content specification**:

```xml
<!-- tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <!-- Inherit Directory.Build.props net8.0 baseline -->
    <!-- Test project per established convention -->
    <IsPackable>false</IsPackable>
    <GenerateDocumentationFile>false</GenerateDocumentationFile>
  </PropertyGroup>
  
  <!-- Per Q-L-17 + recon §7.2.4 (xUnit variant 1.1.2 + Workspaces 5.3.0 pinned) -->
  <!-- Per Q-L-6 CPM, package versions via Directory.Packages.props -->
  <ItemGroup>
    <!-- Test framework -->
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit" />
    
    <!-- CRITICAL per recon §7.2.3: explicit Workspaces 5.3.0 к override transitive 1.0.1 -->
    <!-- Without this, MEF composition fails at first test run -->
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" />
    
    <!-- xUnit baseline (matches existing DualFrontier.* test projects) -->
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    
    <!-- Microsoft.NET.Test.Sdk for dotnet test integration -->
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    
    <!-- FluentAssertions for assertion ergonomics (matches existing test projects) -->
    <PackageReference Include="FluentAssertions" />
  </ItemGroup>
  
  <ItemGroup>
    <!-- ProjectReference к analyzer (compile-time only; analyzer types accessed via reflection в test verifier) -->
    <ProjectReference Include="..\..\tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj" />
  </ItemGroup>
  
</Project>
```

**sln modifications**:
1. Add Project entry under tests Solution Folder `{22222222-…}`:
   ```
   Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "DualFrontier.Analyzers.Tests", "tests\DualFrontier.Analyzers.Tests\DualFrontier.Analyzers.Tests.csproj", "{<NEW-GUID-2>}"
   EndProject
   ```
2. Add 6-config matrix in GlobalSection(ProjectConfigurationPlatforms)
3. Add NestedProjects entry mapping new project к tests folder:
   ```
   {<NEW-GUID-2>} = {22222222-22222222-22222222-22222222}
   ```

**Test file placeholder** (empty test stub for first build):
```csharp
// tests/DualFrontier.Analyzers.Tests/PlaceholderTests.cs
namespace DualFrontier.Analyzers.Tests;

public sealed class PlaceholderTests
{
    [Fact]
    public void Placeholder_AssemblyLoaded_ReturnsTrue()
    {
        // Phase 0 verification — analyzer assembly loads in test verifier host
        // Will be replaced by per-rule verifier tests at Phase β
        typeof(DualFrontier.Analyzers.AnalyzerEntryPoint).Assembly.Should().NotBeNull();
    }
}
```

NOTE: `AnalyzerEntryPoint` is a placeholder marker type (empty class) — added к analyzer csproj в Commit 1 for assembly-load verification. Real DFK### / DFL### analyzer types added during Phase β per-rule implementation cascade.

**Verification**:
- `dotnet restore` clean
- `dotnet build tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj` exit 0
- `dotnet test tests/DualFrontier.Analyzers.Tests` exit 0 (1 placeholder test passes)
- `git diff DualFrontier.sln` shows expected entries

**Commit message**:
```
analyzer(tests): csproj scaffolding per Q-L-17 + recon §7.2.4

Adds tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj —
the xUnit-based test harness для DFK###/DFL### rule verification.

Per Q-L-17 ratification (batch 2 deliberation 2026-05-24): plural naming
matches tests/*.Tests/ convention + Q-L-3 namespace tier + recon §8.1
Option C hybrid placement.

Per recon §7.2.4: Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit
1.1.2 (soft-maintenance acceptable per 2.9M downloads + matches existing
xUnit baseline).

Per recon §7.2.3 CRITICAL: explicit Microsoft.CodeAnalysis.CSharp.Workspaces
5.3.0 PackageReference к override testing-package transitive 1.0.1 — без
this MEF composition fails at first test run.

References:
- recon §7.2 test framework recommendations
- session log batch 2 §4.1 Commit 2
- К-L14 thesis: tooling addition, zero substrate touch
```

### 6.3 — Commit 3 — `analyzer(cpm): Central Package Management adoption per Q-L-6`

**Scope**: Central Package Management adoption — single source of truth для package versions

**Files touched**:
- `Directory.Packages.props` (NEW, ~50 LOC)
- ~30 csproj files (MIGRATED — remove explicit Version attributes from PackageReference entries)

**Content specification**:

```xml
<!-- Directory.Packages.props (REPO ROOT) -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    
    <!-- Per recon §7.1.5 + Q-L-6 ratification -->
    <!-- Central pin для Roslyn analyzer + test framework + existing project deps -->
  </PropertyGroup>
  
  <ItemGroup Label="Roslyn Analyzer SDK (A'.9.1 cascade #5)">
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp" Version="5.3.0" />
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="5.3.0" />
    <PackageVersion Include="Microsoft.CodeAnalysis.Analyzers" Version="5.3.0" />
    
    <!-- Analyzer Testing framework per Q-L-17 + recon §7.2.4 -->
    <PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit" Version="1.1.2" />
    
    <!-- NOT INCLUDED per Q-L-13 (PublicApiAnalyzers entirely deferred): -->
    <!-- Microsoft.CodeAnalysis.PublicApiAnalyzers (audience-driven deferred к community emergence) -->
    
    <!-- NOT INCLUDED per Q-L-15 (code-fix providers PERMANENTLY dropped per PA-001): -->
    <!-- Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.XUnit (AI-agent-first axiom anchor) -->
    
    <!-- NOT INCLUDED per Q-L-12 (BannedApiAnalyzer dropped — closed historical concern Godot): -->
    <!-- Microsoft.CodeAnalysis.BannedApiAnalyzers -->
  </ItemGroup>
  
  <ItemGroup Label="Existing project deps (migrated from per-csproj pinning к centralized)">
    <!-- xUnit -->
    <PackageVersion Include="xunit" Version="2.x.x" /> <!-- per current spot-read -->
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.x.x" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.x.x" />
    
    <!-- FluentAssertions (test ergonomics) -->
    <PackageVersion Include="FluentAssertions" Version="6.x.x" />
    
    <!-- Other existing dependencies migrated per Phase 0 grep audit -->
    <!-- (xunit, FluentAssertions, etc. — exact versions empirically determined Phase 0) -->
  </ItemGroup>
  
</Project>
```

**csproj migrations** (~30 projects):

For each existing csproj с `<PackageReference Include="X" Version="Y" />`:
- Remove `Version="Y"` attribute
- Leave `<PackageReference Include="X" />` — version resolved via CPM

**Verification**:
- `dotnet restore` clean (CPM resolution succeeds across all 40+ projects)
- `dotnet build` full solution exit 0
- `git diff src/ tests/ tools/ mods/ --stat | grep .csproj` shows ~30 modifications (Version attribute removal only)
- `git diff Directory.Packages.props` shows new file ~50 LOC

**Phase 0 sub-task** (auditing existing versions to populate Directory.Packages.props):
```bash
# Empirical version inventory (Phase 0 of this commit)
grep -rh 'PackageReference Include' src/ tests/ tools/ mods/ \
  --include='*.csproj' | \
  sort -u
```

Output → exact Version pins go into Directory.Packages.props.

**Commit message**:
```
analyzer(cpm): Central Package Management adoption per Q-L-6 + recon Q-K-27

Adds Directory.Packages.props (repo root) — single source of truth для
all package versions across 40+ csproj projects.

Per Q-L-6 ratification (batch 1 deliberation 2026-05-24): CPM adoption
к prevent silent version drift across analyzer + tests + production
projects. Per recon Q-K-27 Option (a): version drift risk is real
(analyzer/test/codefix triad must align) + CPM is one-file change.

Migration scope: removed Version="..." attributes from ~30 csproj files'
PackageReference entries. Empirical version inventory via Phase 0 grep
audit (sorted -u uniq pinning).

NOT INCLUDED per ratifications:
- Microsoft.CodeAnalysis.PublicApiAnalyzers (Q-L-13 audience-driven deferred)
- Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.XUnit (Q-L-15 PA-001 axiom)
- Microsoft.CodeAnalysis.BannedApiAnalyzers (Q-L-12 closed concern)

References:
- recon §7.1.5 SDK 5.3.0 pinning
- recon Q-K-27 CPM adoption recommendation
- session log batch 2 §4.1 Commit 3
- К-L14 thesis: governance refinement, zero substrate touch
```

### 6.4 — Commit 4 — `docs(rename): ANALYZER_RULES.md mechanical DF→DFK rename per Q-L-14 (Commit 1)`

**Scope**: Mechanical identifier-only rename DF### → DFK### in ANALYZER_RULES.md (no structural changes)

**Files touched**:
- `docs/architecture/ANALYZER_RULES.md` (MODIFIED — mechanical rename only)

**Phase 0 empirical scope determination**:
```bash
# Per session log batch 2 §4.2 Task 6
grep -c "DF[0-9]" docs/architecture/ANALYZER_RULES.md
# Expected ~50-100 occurrences (per recon §3.1 18 active + 4 reserved rules × ~2-3 mentions each)

# Verify no DF### outside ANALYZER_RULES.md in forward governance:
grep -rl "DF[0-9]" docs/architecture/ --include="*.md"
# Should return ONLY: ANALYZER_RULES.md, A_PRIME_9_RECONNAISSANCE_REPORT.md (historical), 
#                    A_PRIME_9_0_AMENDMENTS_LOG.md (historical), K_CLOSURE_REPORT.md (historical reference)
```

**Sed-style mechanical rename pattern** (review per-occurrence, не automated):
- `DF001` → `DFK001`
- `DF002` → `DFK002`
- ...
- `DF019` → `DFK019` (note: DFK019 splits at Commit 5, this commit just renames)
- `DF020` → `DFK020`
- Sub-rules: `DF003.1` → `DFK003.1`, `DF007.1` → `DFK007.1`, `DF015.1` → `DFK015.1`

**Per Option γ Hybrid historical preservation**:
- DO NOT modify: `A_PRIME_9_RECONNAISSANCE_REPORT.md` (historical record at A'.9.0 closure)
- DO NOT modify: `A_PRIME_9_0_AMENDMENTS_LOG.md` (historical record post-A'.9.0)
- DO NOT modify: `K_CLOSURE_REPORT.md` (Tier 1 LOCKED — К-L canonical text untouched)
- DO modify: `ANALYZER_RULES.md` (current AUTHORED-SKELETON, this commit's target)

**Verification**:
- `grep -c "DF[0-9]" docs/architecture/ANALYZER_RULES.md` returns 0 (all renamed)
- `grep -c "DFK[0-9]" docs/architecture/ANALYZER_RULES.md` returns same count as pre-rename DF[0-9] count
- `git diff docs/architecture/ANALYZER_RULES.md` shows ONLY identifier renames (no structural changes — no header reorganization, no section moves, no new sections)
- Historical artifacts unchanged: `git diff docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` empty, `git diff docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md` empty

**Commit message**:
```
docs(rename): ANALYZER_RULES.md mechanical DF→DFK rename per Q-L-14 (Commit 1/2)

Mechanical identifier-only rename — first of two-commit sequence per Q-L-14
ratification (batch 2 deliberation 2026-05-24).

Scope: docs/architecture/ANALYZER_RULES.md ONLY. ~N occurrences renamed
(empirical Phase 0 count via grep -c).

Historical artifacts preserved per Option γ Hybrid:
- docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md UNCHANGED
- docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md UNCHANGED
- docs/architecture/K_CLOSURE_REPORT.md UNCHANGED (Tier 1 LOCKED)
- KERNEL chronicle DFK### entries use new identifier from this commit forward

Per Q-L-3 tiered namespace ratification: DFK### = К-Lxx invariant rules;
DFL### = Lesson-derived rules; DFC### = cascade-specific rules. Tiered
namespaces appear at Commit 5 structural reorganization.

References:
- Q-L-14 batch 2 deliberation 2026-05-24
- session log batch 2 §4.1 Commit 4
- К-L14 thesis: governance refinement, zero substrate touch
```

### 6.5 — Commit 5 — `docs(restructure): ANALYZER_RULES.md structural reorganization per Q-L-14 (Commit 2)`

**Scope**: Structural reorganization per Q-K-47 (a) + Q-L-7 categories. Scope split §4-§9.

**Files touched**:
- `docs/architecture/ANALYZER_RULES.md` (RESTRUCTURED — section reorganization)

**Target structure** (post-restructure):

```markdown
# DualFrontier Roslyn Architectural Analyzer Rules

## §1 — Document framing + authority chain
(unchanged from skeleton + add Q-L ratifications history reference)

## §2 — Per-rule specification template
(unchanged from skeleton)

## §3 — Tiered namespaces convention per Q-L-3 (NEW SECTION)

DFK### — К-Lxx invariant rules (architectural)
DFL### — Lesson-derived rules (discipline)
DFC### — Cascade-specific rules (drift detection)

## §4 — A'.9.1 active rules (NEW SECTION — first-batch enumeration)

Categories per Q-L-7:
- DualFrontier.Architecture (DFK### kernel architectural)
- DualFrontier.NativeBoundary (DFK### native interop)
- DualFrontier.Discipline (DFL### NEW category Lesson-derived)

P0 Critical (8 rules):
- DFK001 (К-L1) — Architecture category
- DFK002 (К-L2) — NativeBoundary category
- DFK003 (К-L3) — Architecture category
- DFK003.1 (К-L3.1) — Architecture category
- DFK004 (К-L4) — Architecture category
- DFK005 (К-L5) — Architecture category
- DFK007 (К-L7) — Architecture category
- DFK011 (К-L11) — Architecture category

P1 High (5 rules):
- DFK007.1 (К-L7.1) — NativeBoundary category (GPU pipeline slot)
- DFK015.1 (К-L15.1) — NativeBoundary category (per-tier mutex managed facade)
- DFK017 (К-L17) — Architecture category (display composition)
- DFK019.A (К-L19 static API surface) — NativeBoundary category (per Q-L-8 split)

Phase β secondary:
- DFK013 (К-L13 wake_registry discipline) — Architecture category (Warning severity)
- [DFK016 — pending Phase 0 empirical decision per Q-L-16 Option γ]

Lesson-derived (Discipline category):
- DFL025-A — Test class trait enforcement (DFL025 family per amendments §4)
- DFL025-B — Standalone Skip method enforcement

Self-policing:
- DF999 (placeholder rule ID — pending Q-L-18 batch 3 default) — ban GlobalSuppressions + [assembly: SuppressMessage]

**Total A'.9.1 enforcement surface: 15-16 own rules**

## §5 — К-L20 LOCK cascade deferred (NEW SECTION — Mod-API-coupled)

Per amendments log §3 + Crystalka direction §1.1 batch 2:

- DFK009 (К-L9 Vanilla=mods) — IModApi surface volatile pre-К-L20 LOCK
- DFK012 (К-L12 native scheduler sovereignty) — facade boundary not finalized pre-LOCK
- DFK015 (К-L15 bus capability declaration) — capability vocabulary not finalized pre-LOCK
- DFK018 (К-L18 mod unload quiescence) — lifecycle sequence may refine при К-L20
- DFK020 family (20 sub-rules per recon §6.2) — К-L20 canonical text post-LOCK
- DFC001.A (Bridge IRenderCommand marker purity)
- DFC001.B (Bridge Command record purity)

Activation: К-L20 LOCK cascade post-A'.9 milestone per K_CLOSURE §9.5 Q1-Q8 deliberation.

## §6 — Hardware tier expansion cascade deferred (NEW SECTION)

- DFK019.B (К-L19 hardware tier capability runtime check) — multi-hardware-tier audience absent (Q-L-8 + recon Q-K-4 + Crystalka direction §1.6 batch 2)
- DFK016 threshold customization API (only if DFK016 retained α at Phase 0 ratification)

Activation: hardware tier expansion cascade (timing TBD when multi-tier hardware audience materializes).

## §7 — Outside Roslyn scope (NEW SECTION — alternative enforcement)

- DFK010 (К-L10 decision rule) — PERMANENTLY DROPPED per Q-L-9; methodology-layer enforcement via FRAMEWORK + METHODOLOGY documentation discipline
- DFK014 (К-L14 meta-invariant) — reserved per K_CLOSURE §7.3; enforcement via K_L14_EVIDENCE_DASHBOARD.md tracking
- DFK006 (К-L6 SUPERSEDED) — reserved historical traceability; never activates
- DFK008 (К-L8 component lifetime) — process-invariant; pre-commit hook alternative more appropriate per K_CLOSURE §7.3

## §8 — Audience-driven deferred (NEW SECTION — community emergence)

Per Q-L-13 + PA-001 axiom anchor:

- PublicApiAnalyzers (RS0016/RS0017/RS0024) — community ecosystem absent
- Activation conditions: community emergence OR public API stability commitment OR specific cascade brief
- All candidate assemblies (Contracts/Application/Bridge/Launcher) = Mod API surface volatile pre-К-L20 LOCK OR not mod-facing

Per Q-L-15 + PA-001 axiom anchor:

- Code-fix providers (any rule) — AI-agent-first consumer profile permanent
- Activation: NONE (PERMANENT). Requires PA-001 axiom amendment via FRAMEWORK §7.2 protocol FIRST.

Per Q-L-12:

- BannedApiAnalyzer for Godot/Silk.NET — CLOSED HISTORICAL CONCERN
- Activation: NONE. Documentation discipline (DualFrontier.Contracts README) sufficient.

## §9 — Reserved namespaces (NEW SECTION)

- DFC### namespace reserved (no active rules at A'.9.1; activates at К-L20 LOCK cascade per Q-L-11 DFC001 deferral)
- DFL### namespace ACTIVE (DFL025-A + DFL025-B ship at A'.9.1)

## §10 — Per-rule detail specifications (was skeleton §X)

(per-rule §2 template populations as authored Phase β implementation per-rule)
```

**Verification**:
- ANALYZER_RULES.md structure matches above target (8 new sections §3-§9)
- Per Q-L-14 Commit 5 scope: structural changes only — DFK### identifiers ALREADY renamed in Commit 4
- `git diff docs/architecture/ANALYZER_RULES.md` shows section reorganization (NO identifier changes — those happened in Commit 4)
- All Q-L-7 categories enumerated explicitly with rule mappings
- All Q-L-9, Q-L-11, Q-L-12, Q-L-13, Q-L-15 deferral/drop rationale documented с PA-001/PA-002 anchors

**Commit message**:
```
docs(restructure): ANALYZER_RULES.md structural reorganization per Q-L-14 (Commit 2/2)

Structural reorganization per Q-K-47 (a) + Q-L-7 categories ratifications.

Sections added:
- §3 Tiered namespaces convention (DFK###/DFL###/DFC### per Q-L-3)
- §4 A'.9.1 active rules (15-16 enforcement surface)
- §5 К-L20 LOCK cascade deferred (Mod-API-coupled rules per amendments §3)
- §6 Hardware tier expansion cascade deferred (DFK019.B per Q-L-8 split)
- §7 Outside Roslyn scope (DFK010 PERMANENTLY DROPPED Q-L-9; DFK014 meta; etc.)
- §8 Audience-driven deferred (PublicApiAnalyzers Q-L-13; code-fix Q-L-15; BannedApi Q-L-12)
- §9 Reserved namespaces (DFC### inactive until К-L20 LOCK)

Categories per Q-L-7:
- DualFrontier.Architecture (DFK### kernel architectural)
- DualFrontier.NativeBoundary (DFK### native interop)
- DualFrontier.Discipline (NEW — DFL### Lesson-derived)
- DualFrontier.ModSurface (RESERVED — no active rules at A'.9.1)

PA-001 anchors documented at §8 (audience-driven).
PA-002 anchors documented at §7 (methodology-layer outside Roslyn scope).

References:
- Q-L-7 + Q-L-9 + Q-L-11 + Q-L-12 + Q-L-13 + Q-L-14 + Q-L-15 batch 2 deliberation
- session log batch 2 §4.1 Commit 5
- К-L14 thesis: governance refinement, zero substrate touch
```

### 6.6 — Commit 6 — `analyzer(reservedstub): [ReservedStub] attribute infrastructure per Q-L-10`

**Scope**: [ReservedStub] attribute introduction in DualFrontier.Contracts.Analyzer namespace

**Files touched**:
- `src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs` (NEW, ~50 LOC)
- `src/DualFrontier.Contracts/Analyzer/ReservedStubPurpose.cs` (NEW, ~30 LOC)

**Content specification — ReservedStubAttribute.cs**:

```csharp
// src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs
using System;

namespace DualFrontier.Contracts.Analyzer;

/// <summary>
/// Marks a type, method, or property as an intentional reserved stub —
/// a placeholder structurally required для build composition or 
/// architectural sketching, не a runtime-functional implementation.
/// </summary>
/// <remarks>
/// <para>
/// Per К-extensions cascade #5 (A'.9.1) deliberation Q-L-10:
/// reserved-stub patterns are acceptable for two purposes only:
/// </para>
/// <list type="bullet">
///   <item><see cref="ReservedStubPurpose.BuildComposition"/> —
///   placeholder needed для assembly composition (e.g., dispatch arm
///   reserved для future cascade activation)</item>
///   <item><see cref="ReservedStubPurpose.ArchitecturalSketch"/> —
///   structural anchor для forward-design (e.g., interface shape
///   committed before consumer materialization)</item>
/// </list>
/// 
/// <para>
/// Reason field MANDATORY per Q-L-10 + PA-002 axiom (без костылей —
/// no marker without justification). DFL025-A analyzer rule enforces
/// presence at compile time + restricts behavior invocation (not
/// reflection-only access).
/// </para>
/// 
/// <para>
/// Cascade #3 precedent: deferred dispatch arms (HandlePawnState,
/// HandleItemSpawned, HandleTickAdvanced) marked with this attribute
/// at A'.9.1 Phase α Commit 7 (Lesson #N12 sub-pattern B silent stub).
/// </para>
/// </remarks>
/// <seealso cref="ReservedStubPurpose"/>
/// <seealso href="https://github.com/Crystalka228/Colony_Simulator/blob/main/docs/architecture/ANALYZER_RULES.md#dfl025">DFL025 family detection</seealso>
[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]
public sealed class ReservedStubAttribute : Attribute
{
    /// <summary>
    /// The architectural purpose justifying the reserved-stub state.
    /// </summary>
    public ReservedStubPurpose Purpose { get; }
    
    /// <summary>
    /// MANDATORY per Q-L-10 + PA-002 axiom — specific rationale + 
    /// activation trigger reference (e.g., К-L20 LOCK cascade, M3.4 milestone).
    /// </summary>
    public string Reason { get; }
    
    public ReservedStubAttribute(ReservedStubPurpose purpose, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "ReservedStub Reason MANDATORY per Q-L-10 + PA-002 axiom — " +
                "specific rationale + activation trigger reference required.",
                nameof(reason));
        }
        
        Purpose = purpose;
        Reason = reason;
    }
}
```

**Content specification — ReservedStubPurpose.cs**:

```csharp
// src/DualFrontier.Contracts/Analyzer/ReservedStubPurpose.cs
namespace DualFrontier.Contracts.Analyzer;

/// <summary>
/// Architectural purpose justifying a <see cref="ReservedStubAttribute"/> 
/// marker. Two purposes only per Q-L-10 ratification.
/// </summary>
public enum ReservedStubPurpose
{
    /// <summary>
    /// Placeholder structurally required для build composition.
    /// 
    /// Example use case: dispatch arm reserved для future cascade activation
    /// (cascade #3 deferred dispatch arms HandlePawnState/HandleItemSpawned/
    /// HandleTickAdvanced). The arm exists structurally to satisfy interface
    /// contract OR enable compilation, не to implement runtime behavior at 
    /// current cascade.
    /// </summary>
    BuildComposition,
    
    /// <summary>
    /// Structural anchor для forward-design (forward-compatibility sketch).
    /// 
    /// Example use case: interface shape committed before consumer materialization
    /// (e.g., К-L20 Mod API surface skeleton at A'.9 closure pre-LOCK cascade).
    /// The structure exists to anchor canonical artifact + freeze architectural
    /// decisions, не to provide functional implementation.
    /// </summary>
    ArchitecturalSketch
}
```

**Verification**:
- Files exist at expected paths
- `dotnet build src/DualFrontier.Contracts/DualFrontier.Contracts.csproj` exit 0
- Attribute usage constraint (AttributeUsage attribute) applied correctly
- Mandatory Reason field constructor throws ArgumentException on null/whitespace
- XML documentation present for RS1015 helpLinkUri compliance

**Commit message**:
```
analyzer(reservedstub): [ReservedStub] attribute infrastructure per Q-L-10

Adds [ReservedStub] attribute infrastructure to DualFrontier.Contracts.Analyzer 
namespace:

- src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs (NEW, ~50 LOC)
- src/DualFrontier.Contracts/Analyzer/ReservedStubPurpose.cs (NEW, ~30 LOC)

Per Q-L-10 ratification (batch 2 deliberation 2026-05-24):
- Purpose enum: BuildComposition, ArchitecturalSketch (dual purpose recognized)
- Mandatory Reason field per PA-002 axiom (без костылей — no marker без justification)
- Namespace DualFrontier.Contracts.Analyzer (mod-accessible per К-L9)
- AttributeUsage: Class | Struct | Interface | Method | Property
- AllowMultiple: false, Inherited: false

Constructor validation: ArgumentException thrown on null/whitespace Reason —
enforces PA-002 axiom anchor at runtime + compile-time via DFL025-A rule.

Phase α Commit 7 will apply this attribute к cascade #3 deferred dispatch
arms (HandlePawnState, HandleItemSpawned, HandleTickAdvanced) + other 
reserved-stub sites surfaced via Phase 0 empirical scan.

References:
- Q-L-10 + PA-002 axiom batch 2 deliberation 2026-05-24
- amendments log §4 DFL025 family detection mechanism
- Lesson #N12 sub-pattern B silent stub (cascade #3 precedent)
- session log batch 2 §4.1 Commit 6
- К-L14 thesis: tooling addition, zero substrate touch
```

### 6.7 — Commit 7 — `analyzer(annotations): mandatory annotation pass for cascade #3 deferred dispatch arms`

**Scope**: Apply [ReservedStub] attribute к 3 cascade #3 deferred dispatch arms + Phase 0 surfaced other reserved-stub sites

**Files touched**:
- `src/DualFrontier.Launcher/<RenderCommandDispatcher path>/RenderCommandDispatcher.cs` OR equivalent (3 dispatch arm methods)
- 0-5 other reserved-stub sites surfaced via Phase 0 empirical scan

**Phase 0 sub-task** (cascade #3 deferred dispatch arm location):
```bash
# Locate cascade #3 deferred dispatch arms
grep -rn "HandlePawnState\|HandleItemSpawned\|HandleTickAdvanced" src/
# Expected to surface 3 method implementations in RenderCommandDispatcher OR similar

# Locate other reserved-stub candidates
grep -rn "// TODO\|// FIXME\|throw new NotImplementedException" src/ \
  --include='*.cs' \
  | grep -v 'tests/'
# Phase 0 review surfaces additional candidates
```

**Annotation pattern** (verbatim per dispatch arm):

```csharp
// Cascade #3 deferred dispatch arm — pre-annotation:
private void HandlePawnState(PawnStateCommand command)
{
    // Silent stub — deferred к future cascade per Lesson #N12 sub-pattern B
}

// Post-annotation (this commit):
[ReservedStub(
    ReservedStubPurpose.BuildComposition,
    "Cascade #3 deferred dispatch arm — К-extensions cascade #5+ activation. " +
    "RenderCommandDispatcher signature stability per cascade #3 ratification (Lesson #N12 sub-pattern B silent stub). " +
    "Activation trigger: PawnStateCommand consumer materialization (M-series migration).")]
private void HandlePawnState(PawnStateCommand command)
{
    // Silent stub — deferred к future cascade per Lesson #N12 sub-pattern B
}
```

**Detection by DFL025-A** (rule shape):

DFL025-A enforces:
1. Methods/types tagged `[ReservedStub]` MUST NOT be invoked (behavior invocation forbidden)
2. Reflection-only access (e.g., `typeof(ReservedStubType)` inspection) IS permitted
3. Test classes (xUnit `[Trait("Category", "ReservedStub")]`) tagged DFL025-A get exemption — they test reflection behavior not runtime invocation

**Verification**:
- All 3 cascade #3 dispatch arms have [ReservedStub] attribute applied
- Each annotation has specific Reason field referencing cascade #3 + Lesson #N12 sub-pattern B
- Build clean: `dotnet build` exit 0
- DFL025-A rule (not yet implemented detection logic at this commit; stub) loads successfully

**Phase 0 surfaced additional candidates** (if any):
- Each candidate evaluated per Q-L-10 criteria (BuildComposition OR ArchitecturalSketch purpose; Reason mandatory)
- Phase 0 review log appended to ANALYZER_RULES.md §4 audit trail

**Commit message**:
```
analyzer(annotations): [ReservedStub] mandatory annotation pass per Q-L-10

Applies [ReservedStub] attribute к cascade #3 deferred dispatch arms 
(per Lesson #N12 sub-pattern B silent stub) + Phase 0 surfaced reserved-stub sites.

Cascade #3 deferred dispatch arms (3 sites — verbatim per К-L14 #12 precedent):
- HandlePawnState — Purpose=BuildComposition, Reason="Cascade #3 deferred — PawnStateCommand consumer materialization (M-series)"
- HandleItemSpawned — Purpose=BuildComposition, Reason="Cascade #3 deferred — ItemSpawnedCommand consumer materialization"
- HandleTickAdvanced — Purpose=BuildComposition, Reason="Cascade #3 deferred — TickAdvancedCommand consumer materialization"

Phase 0 surfaced additional sites: <N candidates per Phase 0 review log>

Per Q-L-10 + PA-002 axiom: every reserved-stub site carries explicit 
architectural justification + activation trigger reference. DFL025-A rule
(Phase β implementation) enforces behavior-invocation restriction.

References:
- Q-L-10 batch 2 deliberation 2026-05-24
- amendments log §4 DFL025 family
- Lesson #N12 sub-pattern B silent stub (cascade #3 K_EXT_3 brief)
- session log batch 2 §4.1 Commit 7
- К-L14 thesis: tooling addition; zero substrate touch
```

### 6.8 — Commit 8 — `governance(axioms): PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED per Axiom Option (VII)`

**Scope**: PROJECT_AXIOMS.md authoring per session log batch 2 §6.1 verbatim draft

**Files touched**:
- `docs/governance/PROJECT_AXIOMS.md` (NEW, ~250 LOC)
- `docs/governance/REGISTER.yaml` (MODIFIED — DOC-A-PROJECT_AXIOMS enrollment + register_version bump 2.7 → 2.8 + audit_trail event)

**Content**: Use session log batch 2 §6.1 verbatim draft (PA-001..PA-004 codification). Apply Phase 0 Task 3 refinements (anchor references verified against codebase reality).

**REGISTER cascade additions**:

```yaml
# New DOC enrollment
- id: DOC-A-PROJECT_AXIOMS
  path: docs/governance/PROJECT_AXIOMS.md
  title: "DualFrontier Project Axioms"
  category: A
  tier: 1
  lifecycle: LOCKED
  owner: Crystalka
  version: "1.0"
  last_modified: "2026-MM-DD"
  last_modified_commit: "PENDING-COMMIT-A_PRIME_9_1-AXIOM-CODIFICATION"
  content_language: en
  review_cadence: on-change+annual
  last_review_date: "2026-MM-DD"
  last_review_event: "A'.9.1 / К-extensions cascade #5 Phase α Commit 8 — PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED initial codification per Axiom Option (VII) ratification (batch 2 deliberation 2026-05-24). PA-001..PA-004 codified. AI-agent-first consumer profile permanent (PA-001). Без костылей (PA-002). Сложность архитектуры всегда оправдана (PA-003). К-L14 thesis preservation (PA-004). Anchor references verified against codebase reality per Phase 0 Task 3."
  next_review_due: "2027-05-24"
  reviewer: Crystalka

# register_version bump
register_version: "2.8"  # was 2.7
last_modified: "2026-MM-DD"
last_modified_commit: "PENDING-COMMIT-A_PRIME_9_1-AXIOM-CODIFICATION"
last_modified_by: "Claude Code"

# audit_trail event append
- id: EVT-2026-MM-DD-PROJECT_AXIOMS-V10-LOCK
  timestamp: "2026-MM-DDTHH:MM:SSZ"
  cascade: "A'.9.1 / К-extensions cascade #5"
  phase: "Phase α Commit 8"
  type: "DOCUMENT_ENROLLMENT_LOCKED"
  documents_affected:
    - DOC-A-PROJECT_AXIOMS  # NEW Tier 1 LOCKED Category A
    - DOC-G-REGISTER        # register_version 2.7 → 2.8
  commits:
    range: "PENDING-COMMIT-A_PRIME_9_1-AXIOM-CODIFICATION"
    key_commits:
      - hash: "PENDING-COMMIT-A_PRIME_9_1-AXIOM-CODIFICATION"
        summary: "governance(axioms): PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED per Axiom Option (VII)"
  governance_impact: |
    A'.9.1 PHASE α COMMIT 8 — PROJECT_AXIOMS.md V1.0 TIER 1 LOCKED.
    
    Foundational governance artifact codifying PA-001..PA-004 axioms.
    Per Axiom Option (VII) ratification (batch 2 deliberation 2026-05-24):
    single responsibility per governance document; predictable AI agent
    discovery location at docs/governance/PROJECT_AXIOMS.md.
    
    Three-document governance trio at docs/governance/ now complete:
    - FRAMEWORK.md (schema/protocol)
    - SYNTHESIS_RATIONALE.md (provenance)
    - PROJECT_AXIOMS.md (foundational framing)
    
    Axiom inventory:
    - PA-001 — AI-agent-first consumer profile (PERMANENT)
    - PA-002 — Без костылей (no shortcuts)
    - PA-003 — Сложность архитектуры всегда оправдана
    - PA-004 — К-L14 thesis preservation
    
    Brief A'.9.1 first explicit axiom application (§0.2 framing references
    PA-001..PA-004 anchors directly).
    
    K-L14 thesis preservation: zero production code touched (governance
    artifact only). Zero substrate API surface modified. К-L14 evidence
    count unchanged at this commit; cascade closure increments к #14.
  cross_references:
    capa_entries: []
    risks: []
    lifecycle_transitions:
      - document: DOC-A-PROJECT_AXIOMS
        from: NOT_ENROLLED
        to: LOCKED
```

**Verification**:
- `docs/governance/PROJECT_AXIOMS.md` exists with v1.0 frontmatter
- PA-001..PA-004 sections present с anchor references + applied-in fields
- REGISTER.yaml DOC-A-PROJECT_AXIOMS entry present
- register_version 2.8
- audit_trail tail shows EVT-2026-MM-DD-PROJECT_AXIOMS-V10-LOCK
- `pwsh tools/governance/sync_register.ps1 -Validate` exit 0
- `pwsh tools/governance/sync_register.ps1 -Sync` produces PROJECT_AXIOMS.md frontmatter mirror

**Commit message**:
```
governance(axioms): PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED per Axiom Option (VII)

Foundational governance artifact codifying PA-001..PA-004 axioms.

Per Axiom Option (VII) ratification (batch 2 deliberation 2026-05-24):
NEW separate docs/governance/PROJECT_AXIOMS.md (single responsibility per
governance document; predictable AI agent discovery location per Crystalka
direction §1.4 batch 2).

Three-document governance trio at docs/governance/ now complete:
- FRAMEWORK.md (schema/protocol)
- SYNTHESIS_RATIONALE.md (provenance)
- PROJECT_AXIOMS.md (foundational framing — NEW)

Axiom inventory:
- PA-001 — AI-agent-first consumer profile (PERMANENT)
  Anchors: Q-L-15 code-fix providers permanently dropped
- PA-002 — Без костылей (no shortcuts)
  Anchors: Q-L-9 DFK010 dropped, Q-L-11 DFC001 deferred, Q-L-13 PublicApiAnalyzers deferred
- PA-003 — Сложность архитектуры всегда оправдана
  Anchors: tiered DFK/DFL/DFC namespace, three-graph + two-scheduler architecture
- PA-004 — К-L14 thesis preservation
  Anchors: substrate minimality discipline

Phase 0 Task 3 refinements applied: anchor references verified against
FRAMEWORK.md §0, Crystalka direction history (briefs/sessions), cascade #1+
architectural decisions, KERNEL Part 0 + K_L14_EVIDENCE_DASHBOARD.md.

REGISTER cascade:
- DOC-A-PROJECT_AXIOMS enrollment (Tier 1 LOCKED Category A)
- register_version 2.7 → 2.8
- audit_trail event EVT-2026-MM-DD-PROJECT_AXIOMS-V10-LOCK

References:
- Axiom Option (VII) batch 2 deliberation 2026-05-24
- session log batch 2 §6.1 verbatim draft
- session log batch 2 §4.1 Commit 8
- Crystalka direction §1.4 batch 2 (single responsibility per document)
- К-L14 thesis: governance refinement, zero substrate touch
```

### 6.9 — Commit 9 — `governance(crossrefs): FRAMEWORK + SYNTHESIS_RATIONALE cross-reference PATCH bumps`

**Scope**: Add cross-references к PROJECT_AXIOMS.md в FRAMEWORK.md + SYNTHESIS_RATIONALE.md (PATCH-level version bumps)

**Files touched**:
- `docs/governance/FRAMEWORK.md` (MODIFIED — §13 «See also» entry + v1.1 → v1.1.1)
- `docs/governance/SYNTHESIS_RATIONALE.md` (MODIFIED — §7 «Cross-references» entry + v1.0 → v1.0.1)
- `docs/governance/REGISTER.yaml` (MODIFIED — version bumps + audit_trail event)

**FRAMEWORK.md §13 «See also» addition**:

```markdown
## §13 — See also

[existing entries...]

- [PROJECT_AXIOMS.md](./PROJECT_AXIOMS.md) — DualFrontier project axioms (PA-001..PA-004) codifying foundational framing distinct from K-L architectural invariants. Authored at A'.9.1 closure per Axiom Option (VII) ratification (batch 2 deliberation 2026-05-24). FRAMEWORK provides schema; PROJECT_AXIOMS provides framing.
```

**Frontmatter update**:
```yaml
version: "1.1.1"  # was 1.1
last_modified: "2026-MM-DD"
last_modified_commit: "PENDING-COMMIT-A_PRIME_9_1-CROSSREFS"
```

**SYNTHESIS_RATIONALE.md §7 «Cross-references» addition**:

```markdown
## §7 — Cross-references

[existing entries...]

- [PROJECT_AXIOMS.md](./PROJECT_AXIOMS.md) — Foundational project axioms (PA-001..PA-004). SYNTHESIS_RATIONALE traces framework derivation from 5 industry standards; PROJECT_AXIOMS traces framework MOTIVATION (single-developer + AI pipeline audience profile justifies governance complexity per PA-003).
```

**Frontmatter update**:
```yaml
version: "1.0.1"  # was 1.0
last_modified: "2026-MM-DD"
last_modified_commit: "PENDING-COMMIT-A_PRIME_9_1-CROSSREFS"
```

**REGISTER cascade additions**:

```yaml
# DOC-A-FRAMEWORK version bump
- id: DOC-A-FRAMEWORK
  version: "1.1.1"  # was 1.1 — PATCH per §7.1 ruleset (cross-reference addition)
  last_modified: "2026-MM-DD"
  last_modified_commit: "PENDING-COMMIT-A_PRIME_9_1-CROSSREFS"
  last_review_event: "A'.9.1 / К-extensions cascade #5 Phase α Commit 9 — FRAMEWORK v1.1 → v1.1.1 PATCH bump для §13 «See also» PROJECT_AXIOMS.md cross-reference addition per Axiom Option (VII) ratification (batch 2 deliberation 2026-05-24)."

# DOC-A-SYNTHESIS_RATIONALE version bump
- id: DOC-A-SYNTHESIS_RATIONALE
  version: "1.0.1"  # was 1.0
  last_modified: "2026-MM-DD"
  last_modified_commit: "PENDING-COMMIT-A_PRIME_9_1-CROSSREFS"
  last_review_event: "A'.9.1 / К-extensions cascade #5 Phase α Commit 9 — SYNTHESIS_RATIONALE v1.0 → v1.0.1 PATCH bump для §7 «Cross-references» PROJECT_AXIOMS.md cross-reference addition per Axiom Option (VII) ratification."

# register_version bump
register_version: "2.9"  # was 2.8 (post-Commit 8)
last_modified: "2026-MM-DD"
last_modified_commit: "PENDING-COMMIT-A_PRIME_9_1-CROSSREFS"

# audit_trail event append
- id: EVT-2026-MM-DD-AXIOM-CROSSREFS-PATCH
  timestamp: "2026-MM-DDTHH:MM:SSZ"
  cascade: "A'.9.1 / К-extensions cascade #5"
  phase: "Phase α Commit 9"
  type: "DOCUMENT_VERSION_PATCH_BUMP"
  documents_affected:
    - DOC-A-FRAMEWORK              # v1.1 → v1.1.1 PATCH
    - DOC-A-SYNTHESIS_RATIONALE    # v1.0 → v1.0.1 PATCH
    - DOC-G-REGISTER               # register_version 2.8 → 2.9
  governance_impact: |
    Cross-reference patches for PROJECT_AXIOMS.md introduction.
    
    FRAMEWORK §13 + SYNTHESIS_RATIONALE §7 cross-references added per
    Axiom Option (VII) ratification — complete three-document governance
    trio at docs/governance/ visible from every document.
    
    PATCH-level bumps per FRAMEWORK §7.1 versioning ruleset (cross-reference
    addition, no schema change).
    
    К-L14 thesis preservation: zero production code, zero substrate touch.
```

**Verification**:
- FRAMEWORK.md §13 contains PROJECT_AXIOMS.md cross-reference
- SYNTHESIS_RATIONALE.md §7 contains PROJECT_AXIOMS.md cross-reference
- FRAMEWORK.md frontmatter version 1.1.1
- SYNTHESIS_RATIONALE.md frontmatter version 1.0.1
- REGISTER.yaml entries updated
- register_version 2.9
- `pwsh tools/governance/sync_register.ps1 -Validate` exit 0

**Commit message**:
```
governance(crossrefs): FRAMEWORK + SYNTHESIS_RATIONALE PATCH bumps для PROJECT_AXIOMS cross-references

Adds cross-references к docs/governance/PROJECT_AXIOMS.md в both authoritative 
governance documents per Axiom Option (VII) completion (batch 2 deliberation 
2026-05-24).

FRAMEWORK.md v1.1 → v1.1.1 (PATCH):
- §13 «See also» entry для PROJECT_AXIOMS.md
- Rationale: framework provides schema; axioms provide framing

SYNTHESIS_RATIONALE.md v1.0 → v1.0.1 (PATCH):
- §7 «Cross-references» entry для PROJECT_AXIOMS.md
- Rationale: synthesis traces framework derivation; axioms trace framework motivation

PATCH-level bumps per FRAMEWORK §7.1 versioning ruleset (cross-reference 
addition, no schema change).

Three-document governance trio at docs/governance/ now mutually discoverable
from any entry point:
- FRAMEWORK.md (schema/protocol) ↔ PROJECT_AXIOMS.md ↔ SYNTHESIS_RATIONALE.md

REGISTER cascade:
- DOC-A-FRAMEWORK version 1.1 → 1.1.1
- DOC-A-SYNTHESIS_RATIONALE version 1.0 → 1.0.1
- register_version 2.8 → 2.9
- audit_trail event EVT-2026-MM-DD-AXIOM-CROSSREFS-PATCH

References:
- Axiom Option (VII) batch 2 deliberation 2026-05-24
- session log batch 2 §4.1 Commit 9
- К-L14 thesis: governance refinement, zero substrate touch
```

---

## §7 — Phase β cleanup-phase specifications

### 7.1 — Phase β overview

Per Q-L-1 ratification + recon §8.4 Stage 2 + Q-L-15 + Q-L-19 + Q-L-20 + Q-L-21:

**Phase β scope**:
- Implement detection logic for all 15-16 active rules (DFK### + DFL### + DF999 self-policing)
- Run analyzer against full codebase via `dotnet build` — collect all diagnostics
- Per-diagnostic triage: (a) fix in code, (b) suppress with rationale, (c) rule refinement if false-positive
- Suppression CAPA cascade per Q-L-20 default (c) hybrid site-scoped first + rule-scoped thereafter
- Adaptive split per violation count threshold (Q-L-1)

### 7.2 — Per-rule violation count tracking

For each active rule, Phase β tracks:

```yaml
rule_id: DFK001
violation_inventory:
  src_violations: <count>
  test_violations: <count>
  fixture_violations: <count>  # tests/Fixture.* — exempt from CAPA per Q-L-21
  mod_violations: <count>      # mods/* per Q-K-§8-3 included
  total: <sum>
disposition:
  fix_in_code: <count>
  suppress_pragma: <count>     # pragma-only at A'.9.1 per Q-L-19 default (b)
  suppress_attribute: <count>  # 0 expected per Q-L-19 default
  rule_refinement: <count>     # false-positive count
capa_tracking:
  site_scoped_capas: <count>   # first occurrence per rule per Q-L-20 default (c)
  rule_scoped_capa_active: <true/false>  # thereafter consolidated
```

### 7.3 — Suppression discipline (Q-L-19 default + Q-L-20 default + Q-L-22 default)

**Pragma suppression syntax** (per Q-L-19 default (b) pragma-only at A'.9.1):

```csharp
#pragma warning disable DFK###  // DFK###-SUPPRESS: <citation/rationale>
<suppressed code>
#pragma warning restore DFK###
```

**Scope discipline**:
- Minimum scope brackets (≤6 lines per existing convention per recon §9.1)
- Per Q-L-22 default (a): carve-out attributes require mandatory Justification parameter
- Per S-LOCK-13 implicit: no `GlobalSuppressions.cs` file creation (DF999 self-policing rule enforces ban per Q-L-18 default (a))

**CAPA cascade** (per Q-L-20 default (c) hybrid):

First-occurrence per rule → site-scoped CAPA:
```yaml
id: CAPA-YYYY-MM-DD-SUPPRESS-DFK001-NATIVE_WORLD-526
opened_date: "2026-MM-DD"
closure_status: OPEN
trigger: "src/DualFrontier.Core.Interop/NativeWorld.cs:526 + DFK001 (К-L1 native language)"
root_cause: "Legacy fallback branch CS0618 obsolete usage transitional"
immediate_action: "#pragma warning disable DFK001 — pragma in-source suppression"
corrective_action: "Removal at K8 cutover trigger (когда _registry == null branch disappears)"
preventive_action: null
effectiveness_verification:
  method: "grep -E '#pragma warning disable DFK001' src/ returns 0 occurrences post-K8"
  date_verified: null
  verification_commit: null
  verification_pending: "K8 cutover cascade"
lessons_learned_reference: null
```

Subsequent occurrences of same rule → append к rule-scoped CAPA `affected_documents` (preserves «one CAPA = one issue» pattern at first occurrence + natural transition к rule-level tracking).

### 7.4 — Phase β closure gate

**Required for Phase γ transition**:
1. Zero DFK###/DFL### diagnostics on src/ paths at `suggestion` severity (analyzer would emit zero warnings if promoted к error)
2. Suppression sweep log entry appended: «Suppression sweep: N total (M src, K tests, P fixtures, Q mods), 0 unjustified, J open CAPAs»
3. All P1+ suppressions have either inline justification comment OR open CAPA per Q-L-21 default (c) + Q-L-22 default (a)
4. False-positive count tracking — if any rule accumulates ≥2 false-positives, rule refinement CAPA OPEN before Phase γ

**Adaptive gate per Q-L-1**:
- `violation_count ≤ 80` → continue single A'.9.1 cascade к Phase γ
- `violation_count > 150` → split: this cascade closes with Phase β subset; A'.9.1b standalone cascade brief authored для cleanup completion + Phase γ
- `80 < violation_count ≤ 150` → Crystalka decision (hybrid split point determined per direction)

---

## §8 — Phase γ closure protocol

### 8.1 — Severity promotion `suggestion` → `error` (single .editorconfig edit)

Per recon §8.4 Stage 3:

**Single-file diff**:
```ini
# .editorconfig (before Phase γ)
dotnet_diagnostic.DFK001.severity = suggestion
dotnet_diagnostic.DFK002.severity = suggestion
... (15 rules at suggestion)

# .editorconfig (after Phase γ — single commit)
dotnet_diagnostic.DFK001.severity = error
dotnet_diagnostic.DFK002.severity = error
... (15 rules at error)
```

**Exception** (per recon §3.1 К-L13 efficiency-not-correctness):
- DFK013 (К-L13 wake_registry discipline) — promote к `warning` (NOT `error`) per recon §3.1 Warning severity per K_CLOSURE §7.2
- DFK016 (if retained α post-Phase 0) — promote к `warning` per recon §3.1

**Verification**:
- `dotnet build` exit 0 after promotion
- `dotnet test` Modding suite + DualFrontier.Analyzers.Tests exit 0
- No `#pragma warning disable` без accompanying justification OR open CAPA

### 8.2 — Cascade closure deliverables (Phase δ)

**Required closure artifacts**:

1. **K_EXTENSIONS_LEDGER.md §3.6 entry** (К-ext cascade #5):
```markdown
### §3.6 — К-extensions cascade #5 (A'.9.1) — Analyzer Infrastructure (2026-MM-DD)

Brief: `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` v1.0 (EXECUTED)

Cascade outcome: <summary — Phase α 9 commits + Phase β N commits + Phase γ 1 commit + Phase δ 1-2 commits>

К-L14 evidence: #14 first analyzer implementation evidence (Type 6 NEW category — tooling addition)

Rules shipped: <enumeration — 15-16 active rules across DFK### + DFL### + DF999>

Substrate touch: zero (S-LOCK-1 preserved)

Forward implications:
- A'.9.2 — severity promotion completion (если split) OR К-L20 LOCK cascade authoring
- К-L20 LOCK cascade — 6 rule groups deferred activate (DFK009/012/015/018 + DFK020 family + DFC001)
- Hardware tier expansion cascade — DFK019.B activate when multi-tier audience materializes
```

2. **K_L14_EVIDENCE_DASHBOARD.md #14 entry** (per §3.2 verbatim template).

3. **KERNEL_ARCHITECTURE.md v2.5.3 → v2.5.4 chronicle entry** (per Q-L-27 batch 3 default):
```markdown
[chronicle footnote append]:
A'.9.1 / К-extensions cascade #5 — Analyzer Infrastructure cascade (2026-MM-DD).
Shipped Roslyn analyzer infrastructure (15-16 active rules across DFK###/DFL###/DF999).
К-L14 thesis preserved (zero substrate touch). К-L14 Evidence #14 first analyzer 
implementation evidence (Type 6 NEW category tooling addition). К-L count unchanged: 21 final.
```

4. **METHODOLOGY.md v1.12 → v1.13 entry** (per Q-L-26 batch 3 default (c)):
- Lesson #25 refined 3rd extension OR Lesson #N17 codification — formal codification at Brief A'.9.1 closure deliberation
- Lesson #N17 audience-driven tooling deferral provisional status removed (per session log batch 2 §5 + 2nd formal application = this cascade)

5. **REGISTER.yaml cascade** (final increments):
```yaml
register_version: "2.10"  # was 2.9 (post-Commit 9 Phase α)
audit_trail event:
  - EVT-2026-MM-DD-A_PRIME_9_1-CASCADE-CLOSURE
    cascade: "A'.9.1 / К-extensions cascade #5"
    type: "CASCADE_CLOSURE"
    governance_impact: <comprehensive cascade summary>
```

### 8.3 — Push к origin/main protocol

Per session log memory + Claude Code behavior note:
- Auto-mode classifier blocks push-to-main even с explicit user instruction в initial prompt
- Requires in-session re-confirmation after halt + resolution work
- Expected behavior, не bug
- Crystalka ratification required before push

**Push sequence**:
1. Verify all Phase α + β + γ + δ commits clean
2. Crystalka ratification via chat session
3. `git push origin main` — auto-mode classifier halt expected
4. Crystalka re-confirmation in-session post-halt
5. Push completion verification via Filesystem MCP (`.git/refs/heads/main` SHA matches latest commit)

---

## §9 — K-extensions ledger + KERNEL chronicle + LEDGER §3.6 entry templates

### 9.1 — K_EXTENSIONS_LEDGER.md §3.6 entry (verbatim template for Phase δ commit)

```markdown
### §3.6 — К-extensions cascade #5 (A'.9.1) — Analyzer Infrastructure (2026-MM-DD)

**Cascade designation**: К-extensions cascade #5 + A'.9.1 milestone-internal dual designation per Q-L-2

**Brief**: `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` v1.0 (EXECUTED)

**Predecessor**: К-ext #4 (A'.9.0 Reconnaissance) — `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md` v1.0 EXECUTED

**Authoring deliberation**: Two-session pre-authoring 2026-05-24
- Batch 1 — Q-L-1..Q-L-7 ratifications (`SESSION_LOG_2026_05_24_A_PRIME_9_1_DELIBERATION.md`)
- Batch 2 — Q-L-8..Q-L-17 + Axiom Option (VII) (`SESSION_LOG_2026_05_24_A_PRIME_9_1_BATCH_2_COMPLETE.md`)

#### §3.6.1 — Cascade outcome summary

A'.9.1 cascade shipped Roslyn analyzer infrastructure (K-extensions cascade #5).

**Phase α (ξ scaffolding)** — 9 atomic commits:
1. `analyzer(csproj)` — tools/DualFrontier.Analyzers/ csproj scaffolding netstandard2.0
2. `analyzer(tests)` — tests/DualFrontier.Analyzers.Tests/ csproj scaffolding
3. `analyzer(cpm)` — Directory.Packages.props CPM adoption
4. `docs(rename)` — ANALYZER_RULES.md mechanical DF→DFK rename
5. `docs(restructure)` — ANALYZER_RULES.md structural reorganization (§4-§9 scope split)
6. `analyzer(reservedstub)` — [ReservedStub] attribute infrastructure
7. `analyzer(annotations)` — mandatory annotation pass cascade #3 deferred dispatch arms
8. `governance(axioms)` — PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED
9. `governance(crossrefs)` — FRAMEWORK + SYNTHESIS_RATIONALE PATCH-level cross-references

**Phase β (χ cleanup)** — <N atomic commits> per violation triage per Q-L-1 adaptive gate
**Phase γ (ψ promotion)** — 1 atomic commit (.editorconfig severity promotion suggestion → error)
**Phase δ (governance cascade)** — 1-2 atomic commits (closure report + REGISTER cascade)

**Total cascade commits**: <11-15+ commits>

#### §3.6.2 — Rules shipped

**A'.9.1 active enforcement surface** (15-16 own rules):

| Tier | Rules | Count | Category |
|---|---|---|---|
| P0 Critical К-L base substrate | DFK001, DFK002, DFK003, DFK003.1, DFK004, DFK005, DFK007, DFK011 | 8 | Architecture + NativeBoundary |
| P1 High К-L extensions | DFK007.1, DFK015.1, DFK017, DFK019.A | 4 | Architecture + NativeBoundary |
| Phase β secondary | DFK013 + [DFK016 conditional per Phase 0] | 1-2 | Architecture |
| Lesson-derived | DFL025-A, DFL025-B | 2 | Discipline (NEW category) |
| Self-policing | DF999 (per Q-L-18 default ship at A'.9.1) | 1 | Discipline |
| **TOTAL** | | **15-16** | 3 active categories |

#### §3.6.3 — Forward implications

**Deferred к К-L20 LOCK cascade** (Mod-API-coupled per amendments log §3 + Q-L-11):
- DFK009 (К-L9 Vanilla=mods)
- DFK012 (К-L12 native scheduler sovereignty)
- DFK015 (К-L15 bus capability declaration)
- DFK018 (К-L18 mod unload quiescence)
- DFK020 family (20 sub-rules per recon §6.2)
- DFC001.A + DFC001.B (Bridge surface)

**Deferred к hardware tier expansion cascade**:
- DFK019.B (hardware tier capability runtime check per Q-L-8 split)

**PERMANENTLY ABSENT** (PA-001/PA-002 axiom anchors):
- Code-fix providers (PA-001 — Q-L-15)
- PublicApiAnalyzers (PA-001 audience-driven — Q-L-13)
- BannedApiAnalyzer (closed historical concern — Q-L-12)
- DFK010 (PA-002 methodology-layer — Q-L-9)

#### §3.6.4 — К-L14 thesis preservation

- **Substrate touch**: zero (S-LOCK-1 preserved)
- **К-L14 evidence increment**: #14 first analyzer implementation evidence (Type 6 NEW category — tooling addition)
- **К-L count unchanged**: 21 final
- **Falsifiability mechanism shift**: manual cross-document audit → automated compile-time analyzer pass

#### §3.6.5 — Lessons surfaced/applied

- **Lesson #N14 (Phase 0 empirical state coverage)**: seventh+ application — codified discipline applied к Brief A'.9.1 authoring (mandatory reads list expanded к 14 sources including REGISTER + recon report direct access)
- **Lesson #25 refined extension OR Lesson #N17 candidate**: METHODOLOGY v1.13+ codification at this cascade closure per Q-L-26 default (c)
- **Lesson #N17 (audience-driven tooling deferral)**: 2nd formal application (cascade #5 = formal mechanism application; cascade #4 was by-absence) — promotion criterion met
```

### 9.2 — K_L14_EVIDENCE_DASHBOARD.md #14 entry template (per §3.2 verbatim)

(See §3.2 verbatim entry template.)

### 9.3 — KERNEL_ARCHITECTURE.md v2.5.3 → v2.5.4 chronicle entry template

```markdown
[Appended к existing chronicle status footnote]:

— A'.9.1 / К-extensions cascade #5 — Analyzer Infrastructure cascade (2026-MM-DD). Brief 
`tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` v1.0 EXECUTED. Shipped Roslyn 
analyzer infrastructure: 15-16 own rules (8 P0 + 4 P1 К-L invariant DFK### + 1-2 Phase β 
secondary + 2 DFL### Lesson-derived + 1 DF999 self-policing). [ReservedStub] attribute 
infrastructure introduced (DualFrontier.Contracts.Analyzer namespace). Cascade #3 deferred 
dispatch arms (HandlePawnState/HandleItemSpawned/HandleTickAdvanced) annotated с
[ReservedStub(BuildComposition, "...")] per Q-L-10. ANALYZER_RULES.md DF→DFK mechanical 
rename + structural reorganization per Q-L-14 two-commit sequence. Central Package Management 
adopted via Directory.Packages.props per Q-L-6. Tests project tests/DualFrontier.Analyzers.Tests/ 
authored per Q-L-17. PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED authored per Axiom Option (VII) 
ratification (batch 2 deliberation 2026-05-24) — three-document governance trio at 
docs/governance/ complete (FRAMEWORK + SYNTHESIS_RATIONALE + PROJECT_AXIOMS). PA-001..PA-004 
codified (AI-agent-first PERMANENT + без костылей + сложность архитектуры всегда оправдана + 
К-L14 thesis preservation). 6 rule groups deferred к К-L20 LOCK cascade (DFK009/012/015/018 + 
DFK020 family + DFC001 — Mod-API-coupled per amendments log §3 + Crystalka direction 
§1.1 batch 2). 4 surfaces PERMANENTLY ABSENT (code-fix providers PA-001 / PublicApiAnalyzers 
PA-001 audience-driven / BannedApiAnalyzer closed concern / DFK010 PA-002 methodology-layer). 
DFK019 split: DFK019.A (static Vulkan API surface, ships A'.9.1) + DFK019.B (hardware tier 
deferred к hardware tier expansion cascade) per Q-L-8. К-L impact: zero substrate change 
(S-LOCK-1 preserved). К-L14 verification #14 first analyzer implementation evidence — Type 6 
NEW category tooling addition (substrate completely untouched; falsifiability mechanism shifts 
от manual audit к automated compile-time enforcement). К-L count unchanged: 21 final. 
Prior context: A'.9.0 Reconnaissance / К-extensions cascade #4 = v2.5.2 → v2.5.3 patch bump 
(observational baseline evidence #13).

Status: v2.5.4 — A'.9.1 chronicle entry appended.
```

### 9.4 — METHODOLOGY.md v1.12 → v1.13 candidate entry template

(Per Q-L-26 default (c) — formal codification at this cascade closure deliberation)

```markdown
## Lesson #N17 — Audience-driven tooling deferral (FORMALIZED — was Provisional candidate)

### Statement

Tooling infrastructure that serves specific consumer audience (human IDE workflow / 
external community / multi-environment deployment / multi-tier hardware) ships only 
when that audience materializes. Pre-emptive shipping = kostyl pattern violating 
PA-002 (без костылей) + К-L14 substrate minimality (PA-004). Activation triggers 
documented per-deferral. Anchored в PA-001 (current audience profile = AI agents 
permanently).

### Falsifiability mechanism

Per-cascade scan для shipped enforcement infrastructure без active consumer audience documented:
- Does target audience materialize at current phase?
- If no, defer activation к audience emergence trigger
- If yes, ship с appropriate scope
- Audit cadence: per-closure suppression sweep parallels (per recon §9.2.4)
- Surface: every cascade brief identifies tooling additions + audience justification

### Empirical applications

**Cascade #4 (A'.9.0) — by absence** (provisional first application):
- Reconnaissance report explicitly notes audience absence для PublicApiAnalyzers + BannedApiAnalyzer
- Q-K candidates surfaced but no decision yet

**Cascade #5 (A'.9.1) — by formal mechanism** (second formal application — promotion trigger):
1. Code-fix providers PERMANENTLY DROPPED — Q-L-15 ratification (PA-001 axiom anchor)
2. PublicApiAnalyzers entirely deferred — Q-L-13 ratification (community ecosystem absent, PA-001 anchor)
3. BannedApiAnalyzer dropped — Q-L-12 ratification (closed historical concern Godot — PA-002 anchor)
4. DFK019.B hardware tier deferred — Q-L-8 split (multi-hardware-tier audience absent — PA-002 anchor)
5. DFK016 threshold customization API deferred — Q-L-16 deliberation reasoning (multi-hardware-tier audience)

### Cross-references

- PA-001 axiom (AI-agent-first consumer profile)
- PA-002 axiom (без костылей)
- PA-004 axiom (К-L14 thesis preservation)
- Q-L-12, Q-L-13, Q-L-15 ratifications (audience-driven precedent at A'.9.1)
- Lesson #25 refined «structurally eliminate test-lying surface» (analogous pattern)

### Status

- **Promotion criterion**: 2nd formal application (this cascade — A'.9.1)
- **Promoted к FORMALIZED**: Brief A'.9.1 closure deliberation (this entry)
- **METHODOLOGY version**: v1.12 → v1.13

### Analogous pattern с Lesson #25 refined

| Lesson #25 refined cascade #2 | Lesson #N17 cascade #5 |
|---|---|
| Empty stub implementations passing tests | Enforcement infrastructure без active consumer audience |
| «Information-isomorphic к bug» — unearned green light | «Information-isomorphic к kostyl» — unused infrastructure |
| Structural fix: marker-only interface (IRenderCommand) | Structural fix: defer activation к audience emergence |
| Falsifiability: grep для empty Execute() bodies | Falsifiability: per-cascade audit для shipped surface without audience |
| Cascade #2 cure: reshape abstraction | Q-L cure: defer instead of speculate |
```

---

## §10 — ANALYZER_RULES.md scope-split detail (§4-§9 structure)

(Per Q-L-14 Commit 5 structural reorganization specification — verbatim section structure expanded from §6.5)

### 10.1 — ANALYZER_RULES.md post-restructure section inventory

| § | Section | Content scope | Q-L anchors |
|---|---|---|---|
| §1 | Document framing + authority chain | Authority chain (K_CLOSURE §7 canonical → this doc encodes → analyzer implements); Q-L ratifications history reference | All Q-L batch 1 + 2 |
| §2 | Per-rule specification template | Skeleton template для §10 per-rule populations | unchanged |
| §3 | Tiered namespaces convention | DFK### / DFL### / DFC### namespace rationale | Q-L-3 |
| §4 | A'.9.1 active rules | 15-16 enforcement surface per cascade #5 | Q-L-2, Q-L-7, Q-L-8, Q-L-10, Q-L-16, Q-L-18 |
| §5 | К-L20 LOCK cascade deferred | 6 rule groups deferred (Mod-API-coupled) | Q-L-11, amendments §3 |
| §6 | Hardware tier expansion cascade deferred | DFK019.B + DFK016 threshold customization conditional | Q-L-8, Q-L-16 |
| §7 | Outside Roslyn scope | DFK010 + DFK014 + DFK006 + DFK008 (alternative enforcement) | Q-L-9, PA-002 |
| §8 | Audience-driven deferred | PublicApiAnalyzers + code-fix providers + BannedApiAnalyzer | Q-L-12, Q-L-13, Q-L-15, PA-001, PA-002 |
| §9 | Reserved namespaces | DFC### / DFL### namespace activation timing | Q-L-7, Q-L-11 |
| §10 | Per-rule detail specifications | §2 template populations as authored per-rule (Phase β implementation outputs append here) | Phase β iterative |

### 10.2 — Per-rule detail template (Phase β authoring spec)

Each rule's §10 detail section follows §2 template:

```markdown
### DFK### — К-Lxx <invariant short name>

**Canonical К-Lxx statement**: <verbatim K_CLOSURE_REPORT.md §2.X excerpt>

**Authority anchor**: K_CLOSURE_REPORT.md §2.X (lines X-Y); KERNEL_ARCHITECTURE.md Part 0 К-Lxx row

**Enforcement tier**: <T1 trivial / T2 syntax tree / T3 semantic / T4 hybrid / T5 runtime / T6 doc>

**Priority**: <P0 critical / P1 high / P2 medium / P3 low>

**Category**: <DualFrontier.Architecture / DualFrontier.NativeBoundary / DualFrontier.Discipline>

**Severity**: <Error / Warning>

**Detection pattern**:
- Syntactic anchor: <specific symbol/attribute/usage pattern>
- Semantic check: <semantic model invariant>
- Exclusion scope: <tests/ + tests/Fixture.* per Q-L-21 + others>

**Diagnostic message** (per S-LOCK-6 elevated quality):
- Format: <DFK### {context}: {violation} — К-Lxx requires {expected}. {action_guidance}>
- Example: "DFK002 [DllImport]: target DLL 'thirdparty.dll' — К-L2 requires bindings к DualFrontier.Core.Native.dll only. Replace [DllImport] target."

**Help URL**: https://github.com/Crystalka228/Colony_Simulator/blob/main/docs/architecture/ANALYZER_RULES.md#dfk###

**Code-fix feasibility**: <Not feasible (PA-001 anchor — see §8 absence)>

**False-positive carve-outs**: <enumeration if any>

**Test coverage** (Phase α scaffolding test verification):
- Positive case: <example violating code → expected diagnostic>
- Negative case: <example compliant code → no diagnostic>

**Related К-L**: <cross-references>

**Cascade lineage**:
- Authored cascade: A'.9.1 (К-ext #5)
- Last refinement cascade: <if applicable>
```

### 10.3 — Phase β rule implementation order

Suggested implementation order (per dependency + priority):

1. **DFK001 + DFK002** (P0 NativeBoundary foundational) — establishes managed-side К-L1+К-L2 detection
2. **DFK003 + DFK003.1** (P0 storage paths) — establishes attribute presence + symbol shape semantic checks
3. **DFK004** (P0 type ID registry) — establishes registration call detection
4. **DFK005** (P0 declarative bootstrap) — establishes multi-bootstrap fragmentation detection
5. **DFK007 + DFK007.1** (P0+P1 Span protocol) — establishes mutation detection через SpanLease
6. **DFK011** (P0 NativeWorld SSoT) — establishes ManagedWorld whitelist
7. **DFK015.1** (P1 per-tier mutex managed facade) — semantic per-tier API usage detection
8. **DFK017** (P1 display composition) — composition order + layer attribute detection
9. **DFK019.A** (P1 static Vulkan API surface) — Vulkan 1.2-or-earlier surface usage
10. **DFK013** (Phase β secondary efficiency Warning) — full-inventory dispatch detection
11. **DFK016** (Phase β conditional если retained α post-Phase 0)
12. **DFL025-A** (Discipline behavior invocation enforcement) — uses [ReservedStub] attribute Commit 6
13. **DFL025-B** (Discipline standalone Skip method enforcement)
14. **DF999** (self-policing) — ban GlobalSuppressions.cs + [assembly: SuppressMessage]

Each rule implementation = 1 atomic commit minimum (analyzer class + 2-4 verifier tests positive + negative).

---

## §11 — Forward references

### 11.1 — К-L20 LOCK cascade — 6 rule groups + DFC### activation

**Cascade designation**: К-L20 LOCK cascade — Mod API lock milestone activation

**Timing**: Post-A'.9 milestone per K_CLOSURE §9.5 Q1-Q8 deliberation

**Scope additions** (forward — этот brief preserves slot):
- DFK009 (К-L9 Vanilla=mods) — IModApi surface enforcement
- DFK012 (К-L12 native scheduler sovereignty) — facade boundary enforcement
- DFK015 (К-L15 bus capability declaration) — capability vocabulary enforcement
- DFK018 (К-L18 mod unload quiescence) — lifecycle sequence enforcement
- DFK020 family (20 sub-rules per recon §6.2):
  - DFK020.1-5 namespace/type restrictions
  - DFK020.6-9 API usage restrictions
  - DFK020.10-16 manifest field static cross-check rules
  - DFK020.17-20 forward-compatibility grace-period rules
- DFC001.A (Bridge IRenderCommand marker purity)
- DFC001.B (Bridge Command record purity)

**К-L20 LOCK cascade likely shape**:
- Multi-phase analogous к A'.9.1 ξ/χ/ψ progression
- К-L20 canonical text authoring → DFK020 family implementation → Mod-API-coupled DFK### activation → DFC### Bridge surface enforcement
- Estimated scope: substantially larger чем A'.9.1 (4+ deferred K-L rules + 20 DFK020 sub-rules + 2 DFC### rules = 25+ analyzer rules total)
- К-L14 Evidence type: combination Type 3 (substrate refinement — К-L20 canonical text addition) + Type 6 (tooling addition)

### 11.2 — Hardware tier expansion cascade — DFK019.B activation

**Cascade designation**: Hardware tier expansion cascade — multi-hardware-tier audience materialization

**Timing**: When multi-tier hardware audience materializes (timing TBD)

**Scope addition** (forward):
- DFK019.B (К-L19 hardware tier capability runtime check)
- DFK016 threshold customization API (только если DFK016 retained α post-A'.9.1 Phase 0)

**Activation trigger**: Multi-hardware-tier audience emergence (e.g., expansion beyond Crystalka's ASUS TUF Gaming A16 «Skarlet» RX 7600S baseline — multiple hardware tiers in production).

### 11.3 — Phase A'.9.2 — severity promotion completion (если split per Q-L-1)

**Cascade designation**: A'.9.2 / К-extensions cascade #6 (если A'.9.1 split per Q-L-1 violation count >150)

**Conditional scope**:
- If `χ violation_count ≤ 80` at A'.9.1 Phase 0: A'.9.2 NOT required (single A'.9.1 cascade includes Phase γ)
- If `χ violation_count > 150`: A'.9.2 = A'.9.1c standalone cascade — finishes cleanup phase + Phase γ severity promotion
- If `80 < violation_count ≤ 150`: Crystalka hybrid decision determines A'.9.2 scope

**A'.9.2 explicit scope reduction per Q-L-15**: NO code-fix provider work (PA-001 axiom permanent absence). A'.9.2 = pure cleanup + promotion if needed.

### 11.4 — Phase A'.9.3+ — DC### + DL### + M3.4 cascade

**Cascade designation**: A'.9.3+ / К-extensions cascade #7+ — cascade-derived + Lesson-derived auxiliary rules

**Forward scope** (per recon §1.3 + §5):
- DFC### cascade-derived rules (5 candidates from cascade #2 + 5 from cascade #3 per recon §5)
- DFL### Lesson-derived rules (DFL008 atomic-commit hook per Lesson #8; others per matrix)
- M3.4 manifest cross-check analyzer milestone (per recon Domain 4)

**A'.9.3+ explicit non-scope per Q-L-12 + Q-L-13 + Q-L-15**: NO BannedApiAnalyzer, NO PublicApiAnalyzers, NO code-fix providers (PA-001/PA-002 axiom anchors permanent).

### 11.5 — Future option exploration (FO-1..FO-4)

Per user memory + session context:

- **FO-1**: Lean 4 verified mod authoring fork (2-3 year horizon)
- **FO-2**: Theorem-proved kernel с cheating-as-modding pathway
- **FO-3**: Environmental mana magic system (first production K9 RawTileField consumer)
- **FO-4**: Decentralized verification engine infrastructure

These are tracked but not active в A'.9.1 scope. Analyzer infrastructure at A'.9.1 is forward-compatible with FO-1 Lean 4 fork (analyzer rules could output Lean 4 proofs as alternative diagnostic mechanism post-FO-1) and FO-2 theorem-proved kernel (DFK### rules could feed proof obligation generation post-FO-2).

---

## §12 — Cross-references

### 12.1 — Deliberation surface (Tier 1 input)

- `SESSION_LOG_2026_05_24_A_PRIME_9_1_DELIBERATION.md` — batch 1 (Q-L-1..Q-L-7 ratifications)
- `SESSION_LOG_2026_05_24_A_PRIME_9_1_BATCH_2_COMPLETE.md` — batch 2 (Q-L-8..Q-L-17 + Axiom Option (VII))
- `SESSION_LOG_PATCH_2026_05_24_PHASE_0_ACCESS_PATTERN.md` — Phase 0 access pattern patch (REGISTER + recon direct access)

### 12.2 — Authoritative reconnaissance surface (Tier 2 input)

- `docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` v Live — 7-domain reconnaissance output
- `docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md` v1.0 — 4 amendments captured post-A'.9.0 closure

### 12.3 — Authoritative governance trio (Tier 1 LOCKED)

- `docs/governance/FRAMEWORK.md` v1.1 (this cascade v1.1.1 PATCH at Phase α Commit 9) — schema/protocol
- `docs/governance/SYNTHESIS_RATIONALE.md` v1.0 (this cascade v1.0.1 PATCH at Phase α Commit 9) — provenance
- `docs/governance/PROJECT_AXIOMS.md` (NEW at Phase α Commit 8 — Tier 1 LOCKED v1.0) — foundational framing
- `docs/governance/REGISTER.yaml` (operational SoT — register_version 2.7 → 2.10 across this cascade)

### 12.4 — Authoritative architecture surface (Tier 1 LOCKED)

- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.5.3 (v2.5.4 chronicle entry at Phase δ per Q-L-27 default)
- `docs/architecture/K_CLOSURE_REPORT.md` v1.0 — К-L canonical specs (§7 analyzer rule specifications canonical)

### 12.5 — Authoritative methodology + ledger surface (Tier 1 LOCKED)

- `docs/methodology/METHODOLOGY.md` v1.12 (v1.13 candidate at Phase δ per Q-L-26 default (c))
- `docs/architecture/K_EXTENSIONS_LEDGER.md` (§3.6 entry at Phase δ per §9.1)
- `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` (#14 entry at Phase δ per §3.2)
- `docs/architecture/A_PRIME_7_X_LESSON_CANDIDATES.md` (Lesson #N17 candidate documentation at Phase 0 per §4.2 Task 4)

### 12.6 — Production code references

- `src/DualFrontier.Application/Loop/GameBootstrap.cs` — coreSystems enumeration + transitional-state comments (path corrected per Phase 0 §3.1 F1)
- `src/DualFrontier.Core.Interop/Bootstrap.cs` — bootstrap_graph.h consumer (path corrected per Phase 0 §3.1 F1)
- `src/DualFrontier.Application/Scheduler/SchedulerAdapter.cs` + `ManagedSystemDispatcher.cs` — К10.1 interop layer
- `src/DualFrontier.Core/Scheduling/TickScheduler.cs` + `ParallelSystemScheduler.cs` + `DependencyGraph.cs` — managed scheduler facade
- `src/DualFrontier.Application/Modding/ModSubScheduler.cs` + `ModIntegrationPipeline.cs` — mod integration + manifest topo sort
- `src/DualFrontier.Core.Interop/NativeWorld.cs` — К-L11 production storage backbone reference

### 12.7 — Native code references (read-only от Roslyn analyzer scope per S-LOCK-2)

- `native/DualFrontier.Core.Native/include/system_graph.h` — К10.1 authoritative dependency graph
- `native/DualFrontier.Core.Native/include/scheduling_policies.h` — SchedulingClass enum + CPU quotas
- `native/DualFrontier.Core.Native/include/wake_registry.h` — К-L13 5 wake types
- `native/DualFrontier.Core.Native/include/managed_callback.h` — К-L12 batched reverse-P/Invoke ABI
- `native/DualFrontier.Core.Native/include/scheduler_intrinsics.h` — cli/sti stop_machine semantics
- `native/DualFrontier.Core.Native/include/phase_compute.h` — К10.3 v2 Phase.Compute integration (relevant к DFK016 Phase 0 Task 1)
- `native/DualFrontier.Core.Native/include/phase_barrier.h` — К10.1 BarrierType
- `native/DualFrontier.Core.Native/include/bootstrap_graph.h` — declarative bootstrap dependency graph

### 12.8 — Brief artifacts (cascade lineage)

- `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md` v1.0 EXECUTED — predecessor cascade #4
- `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` v0.1 AUTHORED-SKELETON — predecessor analyzer brief skeleton (disposition: REVISE per recon §10 Prerequisite 9 — supersedes specific path/scope choices)
- `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` v1.0 (this artifact)

---

## §13 — Authoring metadata

**Brief authored**: 2026-05-24 by Claude Opus 4.7 (deliberation mode authoring session)
**Brief authored on behalf of**: Crystalka (Volodymyr, solo dev)
**Authoring session**: A'.9.1 Brief authoring (continuous chat session post-batch-2 deliberation completion)
**Authoring input surfaces**:
- Two session logs (batch 1 + batch 2) — chat-transfer artifacts
- Recon report (A_PRIME_9_RECONNAISSANCE_REPORT.md) — Filesystem MCP direct read (335 KB)
- REGISTER.yaml — Filesystem MCP direct read (524 KB)
- Amendments log (A_PRIME_9_0_AMENDMENTS_LOG.md) — Filesystem MCP direct read
- Native scheduler headers + managed scheduler files — empirical state coverage per Lesson #N14 batch 2 §0.2
- Phase 0 access pattern patch (SESSION_LOG_PATCH_2026_05_24_PHASE_0_ACCESS_PATTERN.md)

**Authoring filesystem**: bash staging pattern (`/home/claude/staging/brief/PART_N.md`) → concatenation → `/mnt/user-data/outputs/`
**Project**: Dual Frontier (Crystalka228/Dual-Frontier)
**К-L14 thesis preservation**: zero substrate touch; brief artifact only

### 13.1 — Status at authoring

- **Status**: AUTHORED — pending Crystalka RATIFICATION + execution session handoff
- **Status transitions**: AUTHORED → RATIFIED (Crystalka) → HANDOFF к execution session → EXECUTED (post-cascade closure)

### 13.2 — Quality discipline applied

Per session log batch 2 §3.5 «atomic-commit pattern; scope-prefix format; pre-flight grep AD» + Lesson #N13 commit integrity verification:

- **Verbatim faithfulness**: All Q-L ratifications quoted verbatim from session logs (no semantic drift between deliberation surface and brief authoring)
- **File:line attribution**: Every architectural claim references specific file + line OR specific Q-L decision OR recon section
- **Forward-locked compliance**: Brief §7.2 enumerates 17 forward-locked Q-L decisions that execution agent MAY NOT deviate from
- **Adaptive gate documentation**: Q-L-1 violation count threshold thresholds explicit (≤80 / 80-150 / >150)
- **Defensive completeness**: Comprehensive coverage of all 17 Q-L ratifications + Axiom Option (VII) + all 9 Phase α commits + Phase β/γ/δ closure protocol

### 13.3 — Pending Crystalka ratification surface

Crystalka ratification should verify:

1. **17 forward-locked Q-L decisions** (§1.3) verbatim match deliberation session log ratifications
2. **9 Phase α atomic commits** (§6.1-§6.9) cover all infrastructure + governance scope from Q-L decisions
3. **PROJECT_AXIOMS.md v1.0 draft** (Phase α Commit 8) verbatim matches session log batch 2 §6.1 draft
4. **Phase 0 mandatory reads list** (§4.1) covers all 14 sources including REGISTER + recon report direct access
5. **Phase 0 mandatory empirical scans** (§4.2) cover all 7 tasks (DFK016 feasibility + DFK013 wake_type + axioms refinement + Lesson #N17 documentation + standard reads + DF→DFK scope + violation count estimate)
6. **К-L14 thesis preservation** documented at every commit + Phase + S-LOCK
7. **Adaptive gate** Q-L-1 violation count thresholds correctly documented
8. **К-L20 LOCK cascade forward references** comprehensive (6 rule groups + DFC### activation)
9. **PERMANENTLY ABSENT surfaces** correctly anchored к PA-001/PA-002 axioms (code-fix providers + PublicApiAnalyzers + BannedApiAnalyzer + DFK010)

---

**End of Brief A'.9.1 — Analyzer Infrastructure Cascade (К-extensions cascade #5)**

**Forward expected status**: AUTHORED → Crystalka ratification → execution session handoff (fresh context) → Phase 0 mandatory reads + empirical scans → Phase α 9 atomic commits → Phase β violation triage → Phase γ severity promotion (если ≤80 violations) → Phase δ closure cascade → A'.9.1 closed

**Cascade #5 К-L14 Evidence #14 target**: first analyzer implementation evidence (Type 6 NEW category — tooling addition; zero substrate touch; falsifiability mechanism shifts от manual cross-document audit к automated compile-time invariant enforcement)
