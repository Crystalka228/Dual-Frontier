---
register_id: DOC-A-A_PRIME_9_RECONNAISSANCE_REPORT
project: Dual Frontier
category: A
tier: 2
lifecycle: EXECUTED
owner: Crystalka
version: Live
first_authored: 2026-05-24
last_modified: 2026-05-24
content_language: mixed
next_review_due: null
title: A'.9 Reconnaissance Report — Roslyn Analyzer Architecture Discovery
review_cadence: on-change+phase-led
last_review_date: 2026-05-24
last_review_event: "A'.9.0 Reconnaissance / К-extensions cascade #4 — initial AUTHORED entry. Standalone reconnaissance produced ~3340-line governance artifact covering 7 domains via multi-agent dispatch (7 sub-agents: 3 parallel batch A in α1 + 3 parallel batch B in α2 + 1 sequential C1 in α3 per S-LOCK-5). Domain 1 К-L analyzability 22-row matrix scored per S-LOCK-4 rubric (9 P0/8 P1/3 P2/3 P3). Domain 2 FORMALIZE Lessons analyzability 12-row matrix (11 T6 + 1 T2 — Lesson #8 auxiliary tooling). Domain 3 cascade #2/#3 surfaced rule candidates (10 candidates с cross-cascade observation Lesson #N12 underlies 4 candidates → [ReservedStub] attribute infrastructure recommended). Domain 4 Mod OS К-L20 prep surface (20 candidate DF020 sub-rules + 6 precursor relationships A'.9-era → К-L20 era). Domain 5 Roslyn ecosystem desk research (SDK Microsoft.CodeAnalysis.CSharp 5.3.0 2026-03-10 + xUnit testing framework variant 1.1.2 recommended). Domain 6 Build/CI integration surface (Option C hybrid tools/+tests/ + Directory.Build.props centralized + .editorconfig per-rule severity progressive). Domain 7 suppression governance precedent (near-zero baseline: 5 pragmas + 0 [SuppressMessage] + 0 GlobalSuppressions + 0 CAPA related; 5-tier classification + BAN GlobalSuppressions.cs + tiered CAPA tracking). §10 Brief A'.9.1 prerequisites enumerated (10 items с empirical anchors). §11 Q-K candidates aggregated (45 total: 42 sub-agent + 3 cross-cutting α4 synthesis). §12 cross-references complete. K-L impact: zero. К-L14 verification #13 first observational reconnaissance evidence."
reviewer: Crystalka
special_case_rationale: A'.9 Roslyn analyzer milestone reconnaissance artifact — comprehensive architecture discovery enabling A'.9.1 brief evidence-grounded authoring per Crystalka direction («Два брифа первый, он проведет полную разведку архитектуры»). Tier 2 Live Category A consistent with sister governance artifacts (K_EXTENSIONS_LEDGER + K_L14_EVIDENCE_DASHBOARD + PHASE_A_PRIME_SEQUENCING) — companion artifacts к K_CLOSURE_REPORT.md tracking post-А'.8 milestone evolution. Special review cadence (on-change+phase-led) supports phase milestone integration (A'.9.1 closure triggers next review).
---

# A'.9 Reconnaissance Report — Roslyn Analyzer Architecture Discovery

**Designation**: A'.9.0 Reconnaissance cascade output (К-extensions cascade #4 cross-reference)
**Milestone**: A'.9 Roslyn Architectural Analyzer (multi-cascade milestone)
**Authoring cascade**: A'.9.0 Reconnaissance — Brief `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md`
**Authoring date**: 2026-05-24
**Status**: Live (Phase α complete 2026-05-24; awaiting Phase β governance cascade + Phase γ closure)

---

## §1 — Executive summary

The A'.9.0 Reconnaissance cascade (К-extensions cascade #4 cross-reference) produced this report as its sole deliverable per S-LOCK-1 zero-production-code discipline. Reconnaissance covered all 7 brief-mandated domains via multi-agent dispatch (7 sub-agents: 3 parallel in α1 batch A + 3 parallel in α2 batch B + 1 sequential in α3) plus Phase 0 inventory.

### §1.1 — Key empirical findings

**Pre-existing analyzer artifacts surfaced during Phase 0** (deliberation agent's structural anchor missed these; documented в §2.1):
- `docs/architecture/ANALYZER_RULES.md` v0.1 AUTHORED-SKELETON exists с 18 active + 4 reserved DF### rules already enumerated (DF001–DF019 + DF003.1/DF007.1/DF015.1 sub-rules + DF006/DF008/DF014/DF020 reserved); per-rule §2 specification template defined. Recon scope shifted от «discover taxonomy» к «score analyzability + priority + rule shape refinement against existing taxonomy».
- `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` v0.1 AUTHORED-SKELETON exists since 2026-05-17 with A9.A–E sub-milestones sketched and anticipated `tools/analyzers/DualFrontier.Analyzers/` project location.

**К-L analyzability matrix (Domain 1, §3) — 22 К-L scored**: 2 T1 / 8 T2 / 5 T3 / 4 T4 / 1 T5 / 2 T6 ; 9 P0 critical / 8 P1 high / 3 P2 medium / 3 P3 low. Zero rule ID conflicts с existing ANALYZER_RULES.md DF### taxonomy. One severity refinement candidate (DF019 split — Q-K-§3-4). К-L21 row in skeleton was misnumbered; corrected (no К-L21 exists per K_CLOSURE §2.24).

**FORMALIZE Lessons analyzability matrix (Domain 2, §4) — 12 A'.8-batch Lessons scored**: 11 T6 documentation-only + 1 T2 auxiliary-tooling-only (Lesson #8 atomic-commit shape via git hook, NOT Roslyn). Provisional Lessons #N12 (Defensive Reserved Stub), #N13 (commit integrity verification), #N14 (Phase 0 empirical state coverage) flagged HIGH/MEDIUM-HIGH promotion proximity. Only #N12 has non-trivial Roslyn rule shape potential (T4 hybrid).

**Cascade #2 + #3 surfaced rule candidates (Domain 3, §5) — 10 candidates**: 5 from cascade #2 (IRenderCommand marker-only, Defensive Reserved Stub Pattern, banned-namespace using Godot/Silk.NET, defensive throw message convention, versioning convention) + 5 from cascade #3 (Lesson #N12 sub-pattern B silent stub, dispatch arm handler discipline, constructor injection в Launcher, Bridge Commands pure data, RenderCommandDispatcher signature stability). Net infrastructure recommendation: introduce `[ReservedStub]` + `[MarkerInterface]` attribute infrastructure + `ConventionMessageMatcher` bilingual helper at A'.9.1 Phase α before any DC### rule lands.

**Mod OS К-L20 prep surface (Domain 4, §6) — 20 candidate DF020 sub-rules** spanning 4 categories: 5 namespace/type restrictions (DF020.1–5), 4 API usage restrictions (DF020.6–9), 7 manifest field static cross-check rules (DF020.10–16), 4 forward-compatibility grace-period rules (DF020.17–20). 6 precursor relationships A'.9-era → К-L20 era identified. К-L20 LOCK timing: post-A'.9 milestone per K_CLOSURE §9.5 Q1–Q8 deliberation. M3.4 (manifest cross-check analyzer milestone) deferral disposition flagged as Q-K candidate.

**Roslyn ecosystem state (Domain 5, §7)** — concrete versions and recommendations:
- `Microsoft.CodeAnalysis.CSharp` 5.3.0 (released 2026-03-10) — current stable
- **CRITICAL**: analyzer csproj MUST target `netstandard2.0` (not net8.0); analyzer test csproj remains net8.0
- Test framework: `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` 1.1.2 (matches Dual Frontier xUnit baseline; soft-maintenance acceptable)
- Severity policy: confirm 15 Error + 4 Warning per ANALYZER_RULES.md §4; align with dotnet/roslyn-analyzers + dotnet/aspnetcore precedents (Warning default, Error reserved для unambiguous runtime/architectural violation)
- Code-fix providers: ship analyzer-only at A'.9.1 (defer code-fixes к A'.9.2 после cleanup phase)

**Build/CI integration surface (Domain 6, §8) — concrete recommendations**:
- sln placement: Option C hybrid — `tools/DualFrontier.Analyzers/` analyzer csproj + `tests/DualFrontier.Analyzers.Tests/` test csproj (matches ManifestRewriter precedent verbatim)
- Directory.Build.props: centralized `<ProjectReference OutputItemType="Analyzer">` with `MSBuildProjectName` conditional excluding analyzer-self + tests + Fixture.* projects
- .editorconfig: greenfield (current file 4 lines); recommend per-rule severity at `suggestion` для cleanup phase, promote к `error` post-cleanup (skip `warning` tier к avoid `TreatWarningsAsErrors=true` interaction)
- CI: analyzer runs automatically on `dotnet build`; staged A'.9.1 progression (ξ scaffolding → χ cleanup → ψ promotion)

**Suppression governance precedent (Domain 7, §9)** — existing pattern is near-zero baseline:
- 5 total suppressions (1 src CS0618 transitional + 4 tests CS0649 fixture); 0 `[SuppressMessage]` attributes; 0 `GlobalSuppressions.cs` files; 0 CAPA entries related to suppression
- Recommended governance: 5-tier classification (transitional / fixture / false-positive / carve-out / NOT-allowed); BAN `GlobalSuppressions.cs`; tiered CAPA tracking; per-closure suppression sweep + quarterly cumulative review cadence
- Lesson #25 refined extended: «structurally eliminate suppressed-warning surface» (proposed METHODOLOGY v1.13+ codification)

### §1.2 — Cumulative Q-K candidates surfaced

**42 Q-K candidates** aggregated across all 7 domains' §X.99 subsections for Brief A'.9.1 deliberation (see §11 enumeration):
- §3.99 К-L domain: 7 Q-Ks
- §4.99 Lessons domain: 5 Q-Ks
- §5.99 Cascade #2/#3 domain: 7 Q-Ks
- §6.99 Mod OS К-L20 domain: 6 Q-Ks
- §7.99 Roslyn ecosystem domain: 6 Q-Ks
- §8.99 Build/CI domain: 6 Q-Ks
- §9.99 Suppression governance domain: 5 Q-Ks

### §1.3 — Forward path

Brief A'.9.1 (Analyzer Infrastructure cascade) can be authored against this report's §10 prerequisites + §11 Q-K candidates without re-scanning the codebase. К-extensions cascade #5 designation cross-reference recommended (continues К-extensions sequence; A'.9.1 = first analyzer implementation cascade).

A'.9 milestone decomposition emerging from this report (refinement at Brief A'.9.1 deliberation):
- **A'.9.1**: Analyzer infrastructure + first DF### rules + `.editorconfig` baseline + cleanup phase (3-stage ξ/χ/ψ per §8.4 recommendation)
- **A'.9.2**: Severity promotion (cleanup → error) + optional code-fix providers for Trivial-feasibility rules
- **A'.9.N**: DC### cascade-derived rules + DL### Lesson-derived auxiliary tooling (per Domain 2/3 recommendations); M3.4 deferred analyzer milestones materialization (per Domain 4)
- **Post-A'.9**: К-L20 Mod API lock cascade → DF020 family activation

---

## §2 — Reconnaissance scope + methodology executed

### §2.0 — Execution narrative

A'.9.0 Reconnaissance cascade executed 2026-05-24 by Claude Code on `main` branch (cascade #3 closure baseline `8ea0d03` + 1 post-cascade commit `4981d78` Crystalka CI logs; branch strategy ratified pre-execution as «continue on main from `4981d78`» matching cascade #3 pattern, not feature branch per brief literal). Total cascade: **8 commits** across Phase α (5) + Phase β (TBD) + Phase γ (1), within Q-J-8 commit budget 4-8.

**Phase 0 anomaly halt + resolution** (Lesson #N13 commit integrity discipline applied):
- Initial `dotnet build` failed: 8 MSB3021/MSB3027 file-lock errors от orphan `testhost` PID 7380 (started 2026-05-23 23:48:23 — Crystalka's CI session per `4981d78` AccessViolation logs) holding DLLs в `tests/DualFrontier.Core.Tests/bin/`
- Resolution: Crystalka ratified minimal cleanup option; killed testhost 7380 only; 32 other dotnet processes left untouched
- Retry succeeded: 0 warnings, 0 errors, 7.15s elapsed
- S-LOCK-1 zero-production-code-change preserved (no S-LOCK violation; halt was environmental)

### §2.1 — Phase 0 anomalies surfaced

**Pre-existing analyzer artifacts** (deliberation agent's structural anchor missed these):

1. **`docs/architecture/ANALYZER_RULES.md`** v0.1 AUTHORED-SKELETON (created 2026-05-23 А'.8 К-closure cascade)
   - 18 active + 4 reserved rules already enumerated (DF001–DF019, DF003.1, DF007.1, DF015.1 + DF006/DF008/DF014/DF020 reserved)
   - Per-rule §2 specification template defined
   - Authority chain: K_CLOSURE_REPORT.md §7 canonical → ANALYZER_RULES.md encodes → analyzer implements
   - Forward к LOCKED at A'.9 milestone implementation
   - Implication for A'.9.0 recon: rule taxonomy NOT a discovery surface — already specified. Recon scores **analyzability + priority + rule shape refinement** против existing taxonomy.

2. **`tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md`** v0.1 AUTHORED-SKELETON (created 2026-05-17)
   - Predecessor analyzer brief skeleton — independent of A'.9.0 lineage
   - Anticipated project location: `tools/analyzers/DualFrontier.Analyzers/` (not `src/`)
   - Sub-milestones A9.A–E sketched (scaffolding → per-invariant rules → first-run cleanup → warning→error promotion → test infrastructure)
   - Implication for A'.9.0 recon: forms Brief A'.9.1 candidate skeleton; report §10 prerequisites should resolve conflict с this skeleton (supersede, merge, or revise).

3. **Suppression discipline already strong** (Phase 0 grep scan):
   - src/: 1 pragma (`NativeWorld.cs:526` CS0618 obsolete transition — legitimate)
   - tests/: 4 pragmas (CS0649 unused field в test fixtures — necessary for reflection-based fixtures)
   - SuppressMessage attribute: 0 occurrences across src/ tests/
   - GlobalSuppressions files: 0
   - Implication for Domain 7: governance recommendations build on a clean baseline, not remediation of existing debt.

4. **Build/CI surface bare**:
   - `.editorconfig` essentially empty (`root = true` + `charset = utf-8` only)
   - `Directory.Build.props`: `TreatWarningsAsErrors=true` already enforced, no analyzer references
   - No `.ruleset` files (no legacy code analysis config)
   - No `.github/workflows/*.yml` (no GitHub Actions CI — local-only build verification)
   - Implication for Domain 6: integration surface is "blank canvas" — A'.9.1 has full freedom для placement decisions.

### §2.2 — Multi-agent dispatch executed (S-LOCK-5 recommendation applied)

**Batch A (Phase α1)** — three sub-agents in parallel (~6 min wall-clock):
- **Agent A1 (Domain 1, К-L invariants analyzability)**: read KERNEL_ARCHITECTURE.md Part 0, K_CLOSURE_REPORT.md §2.1–§2.24 + §7, ANALYZER_RULES.md, VULKAN_SUBSTRATE.md crosscut; produced 22-row matrix + per-К-L detailed analysis + 7 Q-K candidates
- **Agent A2 (Domain 2, FORMALIZE Lessons analyzability)**: read METHODOLOGY.md FORMALIZE batch + Provisional batch; produced 12-row matrix + 12 Provisional bonus assessment + 5 Q-K candidates
- **Agent A3 (Domain 3, Cascade #2/#3 surfaced rule candidates)**: read K_EXT_2_GODOT_DEPRECATION_BRIEF + K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF §6 forward consideration sections; extracted 5+5 candidates + cross-cascade observations + 7 Q-K candidates

**Batch B (Phase α2)** — three sub-agents in parallel (~7 min wall-clock):
- **Agent B1 (Domain 5, Roslyn ecosystem desk research)**: 11 WebSearches + 5 WebFetches verifying current state May 2026; produced §7.1–§7.4 + 6 Q-K candidates
- **Agent B2 (Domain 6, Build/CI integration surface)**: deep reads of `DualFrontier.sln` (456 lines), `Directory.Build.props` (39 lines), `.editorconfig` (4 lines), `sync_register.ps1` (395 lines) + 3 representative csproj spot-reads; produced §8.1–§8.4 + 6 Q-K candidates
- **Agent B3 (Domain 7, Suppression governance precedent)**: deep reads of CS0618 site (NativeWorld.cs:500-531) + 4 CS0649 sites + REGISTER.yaml CAPA collection (lines 5776-6573) + Lesson #25 trace; produced §9.1–§9.2 + 5 Q-K candidates

**Sequential (Phase α3)** — one sub-agent depending on Domain 1 К-L20 handoff:
- **Agent C1 (Domain 4, Mod OS К-L20 prep surface)**: full read MOD_OS_ARCHITECTURE.md v1.11 LOCKED (1241 lines) + MODDING.md v1.1 LOCKED + K_CLOSURE §2.23/§9.5/§12 + ANALYZER_RULES.md DF020 + mods/DualFrontier.Mod.Example + ManifestRewriter precedent; produced §6.1–§6.3 enumerating 20 candidate DF020 sub-rules + 6 precursor relationships + 6 Q-K candidates

### §2.3 — S-LOCK compliance verification

| S-LOCK | Status | Evidence |
|---|---|---|
| S-LOCK-1 (zero production code) | ✓ | git diff for cascade commits shows only `docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` + `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md` modifications |
| S-LOCK-2 (comprehensive 7-domain scope) | ✓ | All 7 domains covered per §2.2 + report §3–§9 populated |
| S-LOCK-3 (report path + format) | ✓ | Report at `docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md`, markdown + YAML frontmatter, Tier 2 Live Category A |
| S-LOCK-4 (analyzability scoring rubric) | ✓ | Consistent T1-T6 tier + P0-P3 priority applied across §3 (21 К-L) + §4 (12 Lessons) + §5 (10 candidates) + §6 (20 DF020 sub-rules) |
| S-LOCK-5 (multi-agent dispatch encouraged) | ✓ | 7 sub-agents executed across batch A + batch B + sequential C1 per §2.2 |
| S-LOCK-6 (К-L14 #13 observational evidence framing) | ✓ | Verification #13 entry in K_L14_EVIDENCE_DASHBOARD.md (Phase β2) frames as 5th evidence type NEW category, degenerate pass criteria |
| S-LOCK-7 (full file reads, no truncation) | ✓ | Sub-agents performed full reads (Agent A1 read K_CLOSURE_REPORT.md 2302 lines + KERNEL Part 0; Agent C1 read MOD_OS 1241 lines + MODDING) |
| S-LOCK-8 (Brief A'.9.1 prerequisites §10 enumeration) | ✓ | §10 populated с 10 enumerated prerequisites + empirical anchors |
| S-LOCK-9 (Q-K candidates §11 enumeration) | ✓ | §11 aggregates 42 Q-K candidates from §3.99/§4.99/§5.99/§6.99/§7.99/§8.99/§9.99 + 3 cross-cutting candidates surfaced during synthesis |
| S-LOCK-10 (empirical reads cited per finding) | ✓ | Sub-agent outputs cite source files + sections/lines per claim; bare assertions excluded |

### §2.1 — Phase 0 anomalies surfaced

**Pre-existing analyzer artifacts** (deliberation agent's structural anchor missed these):

1. **`docs/architecture/ANALYZER_RULES.md`** v0.1 AUTHORED-SKELETON (created 2026-05-23 А'.8 К-closure cascade)
   - 18 active + 4 reserved rules already enumerated (DF001–DF019, DF003.1, DF007.1, DF015.1 + DF006/DF008/DF014/DF020 reserved)
   - Per-rule §2 specification template defined
   - Authority chain: K_CLOSURE_REPORT.md §7 canonical → ANALYZER_RULES.md encodes → analyzer implements
   - Forward к LOCKED at A'.9 milestone implementation
   - Implication for A'.9.0 recon: rule taxonomy NOT a discovery surface — already specified. Recon scores **analyzability + priority + rule shape refinement** против existing taxonomy.

2. **`tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md`** v0.1 AUTHORED-SKELETON (created 2026-05-17)
   - Predecessor analyzer brief skeleton — independent of A'.9.0 lineage
   - Anticipated project location: `tools/analyzers/DualFrontier.Analyzers/` (not `src/`)
   - Sub-milestones A9.A–E sketched (scaffolding → per-invariant rules → first-run cleanup → warning→error promotion → test infrastructure)
   - Implication for A'.9.0 recon: forms Brief A'.9.1 candidate skeleton; report §10 prerequisites should resolve conflict с this skeleton (supersede, merge, or revise).

3. **Suppression discipline already strong** (Phase 0 grep scan):
   - src/: 1 pragma (`NativeWorld.cs:526` CS0618 obsolete transition — legitimate)
   - tests/: 4 pragmas (CS0649 unused field в test fixtures — necessary for reflection-based fixtures)
   - SuppressMessage attribute: 0 occurrences across src/ tests/
   - GlobalSuppressions files: 0
   - Implication for Domain 7: governance recommendations build on a clean baseline, not remediation of existing debt.

4. **Build/CI surface bare**:
   - `.editorconfig` essentially empty (`root = true` + `charset = utf-8` only)
   - `Directory.Build.props`: `TreatWarningsAsErrors=true` already enforced, no analyzer references
   - No `.ruleset` files (no legacy code analysis config)
   - No `.github/workflows/*.yml` (no GitHub Actions CI — local-only build verification)
   - Implication for Domain 6: integration surface is "blank canvas" — A'.9.1 has full freedom для placement decisions.

---

## §3 — К-L invariants analyzability matrix

### §3.0 — Summary statistics

**Total К-L scored**: 22 rows (21 main-series invariants per K_CLOSURE_REPORT.md §2.24 + К-L20 reserved row, K-L20 deep analysis deferred to Domain 4 / Agent C1 per brief discipline). Note: the report skeleton's "К-L21" row is **misnumbered**; no К-L21 exists per KERNEL_ARCHITECTURE.md Part 0 final table (Q-K candidate surfaced — see §3.99).

**Counted main-series invariants** (K_CLOSURE_REPORT.md §2.24 verbatim count): К-L1, К-L2, К-L3, К-L3.1, К-L4, К-L5, К-L6 (SUPERSEDED), К-L7, К-L7.1, К-L8, К-L9, К-L10, К-L11, К-L12, К-L13, К-L14, К-L15, К-L15.1, К-L16, К-L17, К-L18, К-L19 = **22 rows** when К-L6 SUPERSEDED counted as row entry (К-L6 superseded by К-L12 — counts in row inventory but does not count toward "live invariant" total which is 21 per §2.24).

**Tier distribution**:
- T1 trivially analyzable: **2** (К-L1, К-L10)
- T2 statically analyzable (syntax tree): **8** (К-L2, К-L3, К-L3.1, К-L4, К-L5, К-L7, К-L11, К-L17)
- T3 advanced static (semantic): **5** (К-L8, К-L9, К-L12, К-L15, К-L18)
- T4 hybrid (static + runtime): **4** (К-L7.1, К-L13, К-L15.1, К-L16)
- T5 runtime-only: **1** (К-L19 — partly; hardware capability at runtime + API-shape sub-detector static)
- T6 documentation-only / meta / superseded: **2** (К-L6 SUPERSEDED, К-L14 meta-invariant)
- Deferred to Agent C1: **1** (К-L20 — Mod API forward-compatibility; reserved post-Mod API lock)

**Priority distribution**:
- P0 critical (must ship A'.9.1): **9** — К-L1, К-L2, К-L3, К-L3.1, К-L4, К-L5, К-L7, К-L11, К-L15
- P1 high (within A'.9 milestone): **8** — К-L7.1, К-L9, К-L10, К-L12, К-L15.1, К-L17, К-L18, К-L19
- P2 medium (post-A'.9 consumer materialization): **3** — К-L13, К-L16, К-L20
- P3 low / documentation-only: **3** — К-L6 SUPERSEDED, К-L8 process-invariant, К-L14 meta-invariant

**Alignment with existing ANALYZER_RULES.md taxonomy**: 18 active rules + 4 reserved (DF006/DF008/DF014/DF020) consistent with K_CLOSURE_REPORT.md §7.2 + §7.3. No rule ID conflicts proposed. One severity refinement candidate flagged for DF019 (§3.99 Q-K).

---

### §3.1 — Matrix

| К-L | Statement excerpt (≤80 chars) | Enforcement | Tier | Priority | Rule ID | Severity | Notes |
|---|---|---|---|---|---|---|---|
| К-L1 | Native language: C++20 (MSVC/GCC/Clang), no third-party C++ deps | compile-time (native build) | T1 | P0 | DF001 | Error | C# Roslyn analyzer scope is limited — true enforcement is in CMakeLists.txt `CMAKE_CXX_STANDARD 20`. Roslyn rule covers managed-side prohibition of non-P/Invoke binding patterns that imply native version drift. See §3.99 Q-K. |
| К-L2 | Bindings: pure P/Invoke к `DualFrontier.Core.Native.dll`; zero 3rd-party | compile-time | T2 | P0 | DF002 | Error | Static detection: DllImport target DLL whitelist; ban C++/CLI imports + 3rd-party native binding libraries. |
| К-L3 | Path α (`unmanaged struct`) default; Path β (`class` + `[ManagedStorage]`) opt-in | compile-time | T2 | P0 | DF003 | Error | Symbol shape check + attribute presence: struct+IComponent → no `[ManagedStorage]`; class+IComponent → requires `[ManagedStorage]`. |
| К-L3.1 | Path β first-class peer; runtime-only; ALC isolation; dual SystemBase API | compile-time + module-boundary | T3 | P0 | DF003.1 | Error | Semantic checks: cross-mod ManagedStore<T> access detection requires assembly identity analysis; persistence detection requires save system codepath analysis. |
| К-L4 | Component type IDs: explicit per-mod registration; FNV-1a prohibited | compile-time | T2 | P0 | DF004 | Error | Pattern detection: hash-based generators (FNV1a/xxHash/MurmurHash) used in component type ID context; reflection-based ID derivation via typeof(T).FullName.GetHashCode(). |
| К-L5 | Bootstrap orchestration: declarative graph, native, parallel | compile-time | T2 | P0 | DF005 | Error | Detect multiple Bootstrap entry points (fragmentation), reflection-based init discovery (implicit ordering), circular bootstrap dependencies. |
| К-L6 | Game tick scheduler: managed C# (SUPERSEDED by К-L12) | n/a | T6 | P3 | DF006 (reserved) | n/a | SUPERSEDED permanently per K_CLOSURE §7.3; rule reserved для historical traceability only; DF006 will never activate. К-L12 DF012 carries scheduling enforcement forward. |
| К-L7 | Span protocol: read-only spans + write command batching; atomic from observer | compile-time | T2 | P0 | DF007 | Error | Pattern detection: mutation through SpanLease<T> indexer; PointerForWrite bypass; mid-tick WriteBatch.Flush invocation. |
| К-L7.1 | GPU compute writes к RawTileField bound к pipeline slot; one-tick lag | hybrid (static + runtime fence) | T4 | P1 | DF007.1 | Error | Static: detect slot-mixing read patterns + sync-default forced migration. Runtime aspect: GPU fence completion not statically verifiable. Falsifiability includes runtime probe per K_CLOSURE §2.9. |
| К-L8 | Component lifetime: native owns storage; managed holds opaque `IntPtr` | process-invariant (git pre-commit) | T3 | P3 | DF008 (reserved) | n/a | Per K_CLOSURE §7.3: «process invariant — git pre-commit hook alternative more appropriate than Roslyn rule. К-L8 enforces at module boundary, not at code-pattern level». DF008 deliberately not implemented as analyzer; pre-commit hook recommended in А'.9 cascade. |
| К-L9 | «Vanilla = mods»: vanilla mods use same IModApi as third-party | compile-time + module-boundary | T3 | P1 | DF009 | Error | Semantic: detect vanilla namespace (`Vanilla.*`) accessing non-IModApi surface (direct kernel reference, separate bootstrap path, native scheduler direct access). Revisit post-Mod API lock per K_CLOSURE §7.2. |
| К-L10 | Decision rule: §8 metrics (GC pause / p99 / long-run drift) supersede §6 mean-speed | documentation + analyzer-assisted | T1 | P1 | DF010 | Error | Trivial: detect §6-style mean-speed benchmark APIs / attributes used in go/no-go decision context (e.g., decision attributes citing mean-speed metrics). High false-positive risk — narrow detector scope to decision-document or benchmark-result-attribute context. |
| К-L11 | Production storage backbone: NativeWorld single source; ManagedWorld test-only | compile-time | T2 | P0 | DF011 | Error | Pattern detection: `new ManagedWorld()` outside test assemblies (namespace whitelist `tests/`). Dual storage backbone reintroduction detection. |
| К-L12 | Native kernel scheduling sovereign; managed scope reduced к user-space | compile-time + module-boundary | T3 | P1 | DF012 | Error | Semantic: detect managed-side scheduling decisions (graph construction, runqueue mods, wake dispatch) outside kernel-space; detect non-batched cross-layer dispatch; detect facade bypass by mods. |
| К-L13 | On-demand activation: five wake types; only runnable systems enter dispatch | hybrid (efficiency, not correctness) | T4 | P2 | DF013 | Warning | Static aspect: detect full-inventory dispatch patterns (`foreach (var s in allSystems)` in tick loop). Runtime aspect: cache locality degradation not statically verifiable. Warning severity per K_CLOSURE §7.2 («efficiency, не correctness»). |
| К-L14 | Performance derives from clean complex architecture (meta-invariant) | documentation + metric dashboard | T6 | P3 | DF014 (reserved) | n/a | Per K_CLOSURE §7.3: «meta-invariant — K_L14_EVIDENCE_DASHBOARD.md alternative more appropriate than Roslyn rule. К-L14 thesis is project-wide architectural framing, not code-pattern level». 9-verification baseline at A'.8 closure; criterion 6 Provisional Q-N-8-7 pending 2nd soft-halt anchor. |
| К-L15 | Native bus authority + three-tier dispatch (fast ≤1ms / normal ≤1 tick / background ≤N ticks) | compile-time + runtime tier latency | T3 | P0 | DF015 | Error | Static: detect managed-side sovereign routing + capability declaration bypass + tier-FQN registration violations. Tier latency contracts (1ms/1 tick/N ticks) verified runtime; static rule cannot enforce timing. |
| К-L15.1 | Three-tier independence: state/runtime/compile-time layers; per-tier mutex + ID-bit isolation | hybrid (static compile-time + runtime deadlock probe) | T4 | P1 | DF015.1 | Error | Layer 3 (compile-time) detection: native-side concern, Roslyn analyzer scope limited; could enforce managed-side facade per-tier isolation. Layers 1+2 (state + runtime) are native-side enforcement (cppcheck/clang-tidy alternative may suit better). Cross-tier re-entrancy deadlock verified by runtime probe (S10 test). See §3.99 Q-K. |
| К-L16 | Sim tick pipeline depth D ≥ 1 (configurable 1-3, default 2) | compile-time + runtime | T4 | P2 | DF016 | Warning | Static: detect hardcoded D < 1 or D > 3; PipelineDepthSetting bypass. Runtime aspect: drain/refill ordering verified runtime. Configurable severity per K_CLOSURE §7.2. |
| К-L17 | Display composition multi-layer with independent latency contracts | compile-time | T2 | P1 | DF017 | Error | Pattern detection: composition order violation (overlays before SimState); `[Layer(LayerType)]` attribute presence + capability token registration; vanilla privilege at layer registration. |
| К-L18 | Mod load/unload requires sim-paused + pipeline-quiescent precondition | compile-time + module-boundary | T3 | P1 | DF018 | Error | Semantic: detect mod load/unload code paths bypassing `PauseAsync` → `WaitForQuiescenceAsync` → operation → `ResumeAsync` sequence; precondition helper bypass detection. |
| К-L19 | Vulkan 1.3 + async compute queue family mandate; fail-fast at startup | runtime-primary (hardware) + static API-shape | T5 | P1 | DF019 | Warning | Runtime-primary: hardware tier check happens at `Runtime.Create` (fail-fast). Static aspect: detect Vulkan 1.2-or-earlier API surface usage; async compute queue fallback bypass attempts. Configurable severity per K_CLOSURE §7.2 (V substrate contract). Severity refinement candidate — see §3.99 Q-K. |
| К-L20 | Mod API forward-compatibility (RESERVED — post-Mod API lock) | deferred | — | P2 | DF020 (reserved) | n/a | **DEEP ANALYSIS DEFERRED TO DOMAIN 4 (Agent C1)** per brief discipline. К-L20 text is pre-AUTHORED; canonical text TBD at Mod API lock deliberation. DF020 activates at Mod API lock milestone landing post-A'.9. Domain 4 (Mod OS К-L20 prep) will detail Mod API surface deviation detection, Bridge mechanism bypass, manifest grace period semantics. |

---

### §3.2 — Detailed per-К-L analysis

#### К-L1 — Native language C++20

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Native language: C++20, MSVC/GCC/Clang. Compiled native, modern features, no third-party deps

**Source citation**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L1 row (line 50); canonical text per `docs/architecture/K_CLOSURE_REPORT.md` §2.1 (lines 206-223).

**Enforcement domain**: compile-time (native build). C# Roslyn analyzer has limited scope here.

**Analyzability tier**: **T1** — Trivially analyzable on the native side (CMake `set(CMAKE_CXX_STANDARD 20)` + `set(CMAKE_CXX_STANDARD_REQUIRED ON)` already enforces dialect). Roslyn analyzer rule DF001 covers the managed-side surface only (patterns implying drift away from C++20 native target — e.g., introducing managed bindings к alternate native runtime).

**Priority**: **P0** — foundational; any drift here cascades through К-L2, К-L8, К-L11.

**Rule shape proposal**:
- Rule ID: **DF001** (existing in ANALYZER_RULES.md §1)
- Severity: **Error** (per K_CLOSURE §7.2)
- Detection pattern: managed-side — detect alternative-native-binding patterns (e.g., managed C# attempts к bypass single-DLL boundary). Native-side enforcement via CMake (not Roslyn scope).
- False-positive risk: **Low** — narrow managed surface
- Code-fix feasibility: **Not feasible** for native dialect (CMake config); **Moderate** for managed-side bypass detection (point at DllImport target list)

**Related К-L**: К-L2 (P/Invoke single-DLL), К-L15.1 (single DLL preserved through source split)

**Open questions / refinements**: Roslyn analyzer scope limitation — see §3.99 Q-K (analyzer infrastructure scope for native-side rules).

---

#### К-L2 — Pure P/Invoke bindings

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Bindings: Pure P/Invoke к `DualFrontier.Core.Native.dll`. Zero third-party C# в production binary (mirrors L2)

**Source citation**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L2 row (line 51); canonical text per `K_CLOSURE_REPORT.md` §2.2 (lines 225-243).

**Enforcement domain**: compile-time (managed Roslyn analyzer scope).

**Analyzability tier**: **T2** — Static analysis of `[DllImport]` declarations + reference assemblies. Detect: third-party C# binding library imports (System.Reflection.Emit native loader patterns, P/Invoke generator libraries, C++/CLI bridge constructs); dynamic native DLL loading via `LoadLibrary` outside `DualFrontier.Core.Native.dll`; production code reference к multiple production DLLs (single-DLL invariant — `bus_native.cpp → 4-file` source split preserved single binary per К-L15.1).

**Priority**: **P0** — foundational binding surface.

**Rule shape proposal**:
- Rule ID: **DF002** (existing)
- Severity: **Error**
- Detection pattern: `[DllImport]` target DLL whitelist (only `DualFrontier.Core.Native.dll`); reference-assembly scan for prohibited binding libraries; `LoadLibrary` / `NativeLibrary.Load` call sites outside infrastructure.
- False-positive risk: **Low** — clear whitelist
- Code-fix feasibility: **Trivial** for DllImport target mismatches (suggest correct DLL); **Not feasible** for binding-library introductions (architectural decision)

**Related К-L**: К-L1, К-L8, К-L11, К-L15 (single C ABI surface)

---

#### К-L3 — Component storage paths (Path α default, Path β opt-in)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Component storage paths Path α (`unmanaged struct`, NativeWorld) default; Path β (managed `class` via `[ManagedStorage]`, mod-side store) per opt-in

**Source citation**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L3 row (line 52); canonical text per `K_CLOSURE_REPORT.md` §2.3 (lines 245-267).

**Enforcement domain**: compile-time (symbol shape + attribute presence).

**Analyzability tier**: **T2** — Roslyn symbol analyzer on `IComponent` implementers: struct + `[ManagedStorage]` = violation; class without `[ManagedStorage]` = violation; struct without `[ManagedStorage]` = Path α (OK); class with `[ManagedStorage]` = Path β (OK).

**Priority**: **P0** — storage path choice gates everything downstream.

**Rule shape proposal**:
- Rule ID: **DF003** (existing)
- Severity: **Error**
- Detection pattern: symbol analyzer on `INamedTypeSymbol` implementing `IComponent`; check `TypeKind == Struct` vs `Class`; check `[ManagedStorage]` attribute presence; cross-tabulate.
- False-positive risk: **Low** — clear matrix
- Code-fix feasibility: **Moderate** — suggest adding/removing `[ManagedStorage]` based on type kind (but author intent may differ — architectural choice cannot be auto-corrected without human judgment)

**Related К-L**: К-L3.1 (Path β bridge sub-invariant), К-L4 (type ID registration interacts), К-L7 (span protocol applies к Path α only)

---

#### К-L3.1 — Path β bridge formalization (sub-invariant)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
Path β is first-class peer к Path α; Path β components runtime-only (not persisted); managed-storage per-mod (reclaimed deterministically on `AssemblyLoadContext.Unload`); within-mod cross-path access via dual SystemBase API; cross-mod managed-path direct access structurally impossible by ALC isolation. (Per `KERNEL_ARCHITECTURE.md` line 72-74 + `K_CLOSURE_REPORT.md` §2.4 lines 269-295.)

**Source citation**: `K_CLOSURE_REPORT.md` §2.4 (lines 269-295) canonical; parent К-L3 row in KERNEL Part 0.

**Enforcement domain**: compile-time + module-boundary (semantic analysis of assembly identity).

**Analyzability tier**: **T3** — semantic analysis required: cross-mod `ManagedStore<T>` access detection requires identifying which mod assembly the call site lives in and which mod owns the target ManagedStore; persistence detection requires identifying save-system codepaths and verifying Path β components excluded; within-mod cross-path access detection requires verifying `SystemBase.NativeWorld.AcquireSpan<T>()` для Path α and `SystemBase.ManagedStore<T>()` для Path β are used, not direct property access.

**Priority**: **P0** — К-L3 sub-invariant precedent pattern; foundational for mod architecture.

**Rule shape proposal**:
- Rule ID: **DF003.1** (existing — first sub-rule precedent)
- Severity: **Error**
- Detection pattern: semantic analyzer — assembly identity check at `ManagedStore<T>` call sites; save system code path identification (FQN-based whitelist for Path α serialization, Path β exclusion); dual SystemBase API usage enforcement.
- False-positive risk: **Medium** — cross-mod ALC boundary detection requires understanding of assembly load context which is runtime concept; static analysis may need conservative approximation.
- Code-fix feasibility: **Complex** — architectural violations rarely have mechanical fixes.

**Related К-L**: К-L3 parent, К-L9 (Mod parity), К-L11 (NativeWorld single source for Path α)

**Open questions / refinements**: ALC boundary detection robustness — see §3.99 Q-K.

---

#### К-L4 — Explicit type ID registry

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Type IDs: Explicit registry per-mod registration. FNV-1a hash collision-prone; explicit IDs deterministic

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L4 row (line 53); canonical text per `K_CLOSURE_REPORT.md` §2.5 (lines 297-313).

**Enforcement domain**: compile-time (pattern detection).

**Analyzability tier**: **T2** — pattern detection on type ID derivation call sites: hash-based generators (FNV1a, xxHash, MurmurHash3) used in component type ID context; `constexpr`-equivalent compile-time generation (likely C#-side `const`/`static readonly` hash); `typeof(T).FullName.GetHashCode()` reflection-based ID derivation.

**Priority**: **P0** — type ID determinism is foundational к cross-mod isolation.

**Rule shape proposal**:
- Rule ID: **DF004** (existing)
- Severity: **Error**
- Detection pattern: method invocation analyzer — detect known hash function calls (FNV1a, xxHash, MurmurHash) where return value flows into `RegisterComponentType<T>()` or type ID assignment context; flow analysis from `typeof(T)` к hash methods.
- False-positive risk: **Low** — narrow context (type ID assignment site)
- Code-fix feasibility: **Not feasible** — requires registering explicit ID values (human-assigned)

**Related К-L**: К-L3, К-L11, К-L20 (future Mod API surface)

---

#### К-L5 — Declarative bootstrap graph

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Bootstrap orchestration: Declarative graph, native, parallel where deps allow

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L5 row (line 54); canonical text per `K_CLOSURE_REPORT.md` §2.6 (lines 315-331).

**Enforcement domain**: compile-time (pattern detection).

**Analyzability tier**: **T2** — detect: multiple Bootstrap entry points (fragmentation — only `GameBootstrap.cs` should exist); reflection-based init discovery (implicit ordering); circular bootstrap dependencies (graph topological analysis); file-system-order-dependent initialization (relies on alphabetical or directory enumeration order).

**Priority**: **P0** — bootstrap determinism is foundational.

**Rule shape proposal**:
- Rule ID: **DF005** (existing)
- Severity: **Error**
- Detection pattern: assembly-level analyzer — count of `IBootstrap`-implementing types; check for reflection-based discovery patterns in bootstrap context (Assembly.GetTypes + filtering); check `[BootstrapOrder]` or equivalent for explicit declaration.
- False-positive risk: **Medium** — bootstrap pattern variations may be legitimate (e.g., test-fixture bootstrap); narrow scope to `src/DualFrontier.Application/Bootstrap/`.
- Code-fix feasibility: **Complex** — architectural pattern violations

**Related К-L**: К-L12 (native scheduler graph symmetric к bootstrap graph)

---

#### К-L6 — Game tick scheduler [SUPERSEDED by К-L12]

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> **SUPERSEDED by K-L12** (see KERNEL_FULL_NATIVE_SCHEDULER.md). Original rationale «Vanilla = mods» preserved as K-L9; execution layer concern factored out to K-L12 at K10.1 closure (2026-05-18)

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L6 row (line 55); supersession rationale per `K_CLOSURE_REPORT.md` §2.7 (lines 333-349).

**Enforcement domain**: n/a (superseded).

**Analyzability tier**: **T6** — Documentation-only / superseded; no production code path manifests К-L6 post-К10.1.

**Priority**: **P3** — DF006 reserved permanently per K_CLOSURE §7.3.

**Rule shape proposal**:
- Rule ID: **DF006** (reserved — will never activate; preserved для historical traceability per K_CLOSURE §7.3)
- Severity: n/a
- Detection pattern: n/a
- False-positive risk: n/a
- Code-fix feasibility: n/a

**Related К-L**: К-L12 (carries scheduling enforcement forward via DF012), К-L9 (preserved through facade pattern)

---

#### К-L7 — Span protocol (parent invariant)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1 opt-in coexistence) Read-only spans + write command batching (К-L7)

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L7 row (line 56); canonical text per `K_CLOSURE_REPORT.md` §2.8 (lines 351-373).

**Enforcement domain**: compile-time (pattern detection on SpanLease/WriteBatch APIs).

**Analyzability tier**: **T2** — detect: mutation through read span (`span[i] = value` on `SpanLease<T>` instance); write outside batch (`NativeWorld.AcquireSpan<T>().PointerForWrite`-style bypass); mid-tick `WriteBatch.Flush` invocation (flow analysis — should only happen at tick boundary).

**Priority**: **P0** — span protocol foundational к storage safety.

**Rule shape proposal**:
- Rule ID: **DF007** (existing)
- Severity: **Error**
- Detection pattern: type-based analyzer — `SpanLease<T>` indexer assignment detection (immutable span violated); `WriteBatch.Flush()` call-site analysis (must be at tick boundary — narrow к `PhaseCoordinator` / `SimulationLoop` scope).
- False-positive risk: **Low** — narrow API surface
- Code-fix feasibility: **Moderate** — suggest using `BeginBatch<T>` for writes; tick-boundary flush has structural fix candidates

**Related К-L**: К-L7.1 (GPU pipeline slot sub-invariant), К-L11 (NativeWorld storage), К-L16 (pipeline depth interacts)

---

#### К-L7.1 — GPU compute pipeline slot binding (sub-invariant)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> GPU compute writes к RawTileField storage bound к pipeline slot. Sim-thread reads of pipeline-managed fields see slot-tail state (sim tick T+D reads dispatched-at-(T+D-1) state). One-tick lag bounded и deterministic. К-L7 atomic-from-observer invariant preserved within pipeline slot boundary

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L7.1 row (line 56); canonical text per `K_CLOSURE_REPORT.md` §2.9 (lines 375-398).

**Enforcement domain**: hybrid (static slot-binding patterns + runtime GPU fence verification).

**Analyzability tier**: **T4** — Static analysis covers slot-binding API usage patterns (detect slot-mixing reads; sync-default forced migration where V1 consumers get coerced к pipeline path). Runtime aspect: GPU fence completion verification cannot be statically asserted — falsifiability criteria («fence-unsync reads») require runtime probe.

**Priority**: **P1** — sub-invariant of К-L7; ship within A'.9 cascade.

**Rule shape proposal**:
- Rule ID: **DF007.1** (existing — sub-rule precedent)
- Severity: **Error**
- Detection pattern: detect slot-mixing API patterns (sim tick T reading slot T-1 buffer without explicit slot accessor); V1 → pipeline migration call sites that force consumers (no opt-in checked).
- False-positive risk: **Medium** — pipeline slot semantics nuanced; flow analysis may miss legitimate cross-slot lookups (e.g., debug tooling).
- Code-fix feasibility: **Complex** — slot mixing fixes require architectural understanding.

**Related К-L**: К-L7 parent, К-L16 (pipeline depth D governs sub-invariant), К-L19 (async compute queue family)

**Open questions / refinements**: Runtime fence verification — see §3.99 Q-K for runtime-probe integration question.

---

#### К-L8 — Component lifetime (native owns storage)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Component lifetime: Native owns storage, managed holds opaque `IntPtr`. Single ownership boundary; managed holds handle only

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L8 row (line 57); canonical text per `K_CLOSURE_REPORT.md` §2.10 (lines 400-422).

**Enforcement domain**: process-invariant (per K_CLOSURE §7.3 — git pre-commit hook alternative).

**Analyzability tier**: **T3** — Theoretically static-analyzable (detect managed-side component pool reintroduction; detect dual ownership boundary patterns), BUT per K_CLOSURE §7.3 official rationale: «К-L8 enforces at module boundary, not at code-pattern level. Pre-commit hook can verify no managed-side component pool reintroduced.» Decision: reserve DF008.

**Priority**: **P3** — documentation-only Roslyn surface; process-invariant primary enforcement.

**Rule shape proposal**:
- Rule ID: **DF008** (reserved per K_CLOSURE §7.3 — process invariant, git pre-commit hook alternative)
- Severity: n/a (rule reserved; no Roslyn implementation planned)
- Detection pattern: n/a (deferred to pre-commit hook)
- False-positive risk: n/a
- Code-fix feasibility: n/a

**Related К-L**: К-L11 (NativeWorld single source of truth co-canonical), К-L3.1 (Path β managed-side storage is allowed exception specifically для Path β)

**Open questions / refinements**: Pre-commit hook design — see §3.99 Q-K (DF008 process-invariant tooling).

---

#### К-L9 — Mod parity («Vanilla = mods»)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Mod parity Vanilla mods register components и systems through same IModApi as third-party

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L9 row (line 58); canonical text per `K_CLOSURE_REPORT.md` §2.11 (lines 423-445).

**Enforcement domain**: compile-time + module-boundary (semantic).

**Analyzability tier**: **T3** — semantic analyzer required: detect vanilla namespace (`Vanilla.{Core,Combat,Magic,Inventory,Pawn,World}`) accessing non-IModApi surface (direct kernel reference, separate bootstrap path, native scheduler direct access); detect К-L9 facade pattern bypass (vanilla mod direct native scheduler access).

**Priority**: **P1** — К-L9 facade bypass is structural violation; ship within A'.9. Per K_CLOSURE §7.2: «revisit post-Mod API lock».

**Rule shape proposal**:
- Rule ID: **DF009** (existing)
- Severity: **Error**
- Detection pattern: namespace-scoped analyzer — assembly identity check at call sites; vanilla namespace identification (`Vanilla.*`); IModApi facade enforcement (only allowed surface from vanilla mods).
- False-positive risk: **Medium** — vanilla mods may legitimately reference shared infrastructure (е.g., test fixtures shared с third-party).
- Code-fix feasibility: **Complex** — vanilla privilege removal is architectural refactor.

**Related К-L**: К-L3.1 (mod ALC isolation), К-L12 (facade routes к native scheduler), К-L15 (facade routes к native bus), К-L20 (Mod API lock will refine К-L9 surface)

---

#### К-L10 — Decision rule (§8 metrics authority)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Decision rule §8 metrics (GC pause / p99 / long-run drift on weak hardware). §6 «20% mean speed» superseded

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L10 row (line 59); canonical text per `K_CLOSURE_REPORT.md` §2.12 (lines 446-465).

**Enforcement domain**: documentation + analyzer-assisted (narrow technical surface).

**Analyzability tier**: **T1** — Trivially analyzable in narrow context: detect §6-style mean-speed benchmark APIs or attributes used in go/no-go decision context (e.g., decision attributes citing mean-speed metrics in `[ArchitecturalDecision]`-style annotations или benchmark-result-feeds-into-decision flows).

**Priority**: **P1** — decision rule integrity matters; ship A'.9.

**Rule shape proposal**:
- Rule ID: **DF010** (existing)
- Severity: **Error**
- Detection pattern: attribute/annotation analyzer — narrow к decision-document or benchmark-result-attribute context; flag mean-speed metric usage in go/no-go decision contexts.
- False-positive risk: **High** — benchmarks for benchmark's sake (e.g., performance regression tracking) MAY use mean-speed legitimately; narrow к explicit decision-context attributes/methods needed.
- Code-fix feasibility: **Not feasible** — decision rule violations are documentation-level.

**Related К-L**: К-L14 (meta-invariant; default-inclusion bias), К-L19 (hardware tier dictates §8 metrics)

**Open questions / refinements**: Decision-context detection scope — see §3.99 Q-K.

---

#### К-L11 — Production storage backbone (NativeWorld single source of truth)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Production storage backbone NativeWorld single source of truth (Solution A); ManagedWorld retained as test fixture and research artifact only

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L11 row (line 60); canonical text per `K_CLOSURE_REPORT.md` §2.13 (lines 466-488).

**Enforcement domain**: compile-time (pattern detection).

**Analyzability tier**: **T2** — pattern detection: `new ManagedWorld()` outside test assemblies (namespace whitelist — `tests/*` allowed); dual storage backbone reintroduction (production code path using both `NativeWorld` and `ManagedWorld` simultaneously).

**Priority**: **P0** — production storage authority foundational.

**Rule shape proposal**:
- Rule ID: **DF011** (existing)
- Severity: **Error**
- Detection pattern: invocation analyzer — `ObjectCreationExpression` on `ManagedWorld`; assembly scope check (allow `tests/`, prohibit `src/`).
- False-positive risk: **Low** — clear assembly whitelist; `ManagedTestWorld` renaming convention reinforces split.
- Code-fix feasibility: **Trivial** within test assemblies (suggest `ManagedTestWorld`); **Complex** in production (architectural rewrite required).

**Related К-L**: К-L8 (component lifetime co-canonical), К-L3 (Path α uses NativeWorld), К-L7 (span protocol on NativeWorld)

---

#### К-L12 — Full native kernel scheduling

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Native kernel scheduling Sovereign per-tick scheduling for kernel-space systems (Core) native; managed scheduler scope reduced к user-space (mod) systems within mod ALCs. Kernel scheduling decisions are made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement. Cross-layer communication uses C ABI with batched callbacks

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L12 row (line 61); canonical text per `K_CLOSURE_REPORT.md` §2.14 (lines 489-515).

**Enforcement domain**: compile-time + module-boundary (semantic).

**Analyzability tier**: **T3** — semantic analyzer: detect managed-side scheduling decisions (graph construction, runqueue mods, wake dispatch) outside kernel-space; detect non-batched cross-layer dispatch (per-system synchronous callback from native к managed instead of batched callback ABI); К-L9 facade bypass detection (mods accessing native scheduler directly bypassing managed facade).

**Priority**: **P1** — К-L12 SUPERSEDES К-L6; facade bypass is structural violation. Ship A'.9.

**Rule shape proposal**:
- Rule ID: **DF012** (existing)
- Severity: **Error**
- Detection pattern: cross-assembly call analyzer — managed scheduler facade pattern enforcement (mods must route via `ManagedSchedulerFacade`); detect direct native scheduler P/Invoke from mod assemblies.
- False-positive risk: **Medium** — kernel-space (Core) systems legitimately call native scheduler directly; differentiate kernel-space vs user-space call sites.
- Code-fix feasibility: **Complex** — facade introduction is architectural refactor.

**Related К-L**: К-L6 (SUPERSEDED by К-L12), К-L9 (facade preserves «Vanilla = mods»), К-L13 (wake type dispatch), К-L14 (performance derives from clean scheduler)

---

#### К-L13 — On-demand system activation

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> On-demand system activation Five wake types (Timer / Event / StateChange / Init / Explicit); only runnable systems enter phase dispatch; per-tick dependency graph computed on runnable subset

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L13 row (line 62); canonical text per `K_CLOSURE_REPORT.md` §2.15 (lines 516-541).

**Enforcement domain**: hybrid (static patterns + runtime cache-locality observation).

**Analyzability tier**: **T4** — Static aspect: detect full-inventory dispatch patterns (`foreach (var s in allSystems)` in tick loop); wake source missing at registration (system relies on tick-based execution без registering wake source). Runtime aspect: cache locality degradation not statically verifiable — falsifiability criterion 3 («cache locality degrades systematically») requires runtime metrics.

**Priority**: **P2** — efficiency-not-correctness per K_CLOSURE §7.2; ship after critical rules.

**Rule shape proposal**:
- Rule ID: **DF013** (existing — Warning severity)
- Severity: **Warning** (per K_CLOSURE §7.2: «efficiency, не correctness»)
- Detection pattern: tick-loop pattern detector (full-inventory iteration in `SimulationLoop` or `PhaseCoordinator` context); `SystemBase` derived types — check wake source registration in constructor or initialization.
- False-positive risk: **Medium** — full-inventory iteration may be legitimate в debug/diagnostic contexts.
- Code-fix feasibility: **Moderate** — suggest wake source registration pattern.

**Related К-L**: К-L12 (scheduler dispatches runnable subset), К-L14 (cache locality contributes к К-L14 thesis), К-L15 (event tier interacts с wake type)

---

#### К-L14 — Performance derives from clean complex architecture (meta-invariant)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0 — Abbreviated form per Q-N-8-2):
> Abbreviated form (per Q-N-8-2 LOCKED hybrid Q2 (c)): Architectural completeness causes performance; tactical heuristics inapplicable в research framework; default-inclusion bias for architectural items. Canonical text (full thesis + Causality + Empirical + Falsifiability + Default-inclusion bias + Burden of proof sub-clauses): see K_CLOSURE_REPORT.md §1.2

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L14 row (line 63); canonical text per `K_CLOSURE_REPORT.md` §1.2 (lines 57-75) + falsifiability table per §2.16 (lines 542-568).

**Enforcement domain**: documentation + metric dashboard (K_L14_EVIDENCE_DASHBOARD.md per K_CLOSURE §7.3).

**Analyzability tier**: **T6** — Per brief instruction + K_CLOSURE §7.3 official rationale: «meta-invariant — К-L14 thesis is project-wide architectural framing, not code-pattern level. K_L14_EVIDENCE_DASHBOARD.md (DOC-A-K_L14_EVIDENCE_DASHBOARD) tracks К-L14 empirical evidence forward per Q9 LOCKED + Q-N-8-3 LOCKED.»

**Priority**: **P3** — documentation-only (per brief instruction and K_CLOSURE §7.3 reservation).

**Rule shape proposal**:
- Rule ID: **DF014** (reserved per K_CLOSURE §7.3 — meta-invariant, K_L14_EVIDENCE_DASHBOARD.md alternative)
- Severity: n/a
- Detection pattern: n/a (deferred к dashboard)
- False-positive risk: n/a
- Code-fix feasibility: n/a

**Related К-L**: All К-L (К-L14 thesis materializes through other invariants' clean architectural surface); К-L10 (decision rule aligns с К-L14 long-horizon framing); К-L20 (Mod API lock will test К-L14 в Phase B M-cycle migration)

**К-L14 evidence baseline at A'.8 closure**: 9 verifications per `K_CLOSURE_REPORT.md` §3.2; verification #12 (cascade #3) recorded в KERNEL chronicle line 17.

---

#### К-L15 — Native bus authority + three-tier event dispatch (parent invariant)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> К-L15: Native kernel owns sovereign event routing for kernel-space and cross-layer events. Bus implementation native: type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native authority. Bus supports three-tier dispatch (fast / normal / background) with tier declared per event type. Managed bus facade preserved as IModApi-exposed surface (K-L9 uniformity); implementation routes through C ABI bridge к native bus. Fast/background event publish/subscribe requires per-FQN per-tier capability declarations

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L15 row (line 64); canonical text per `K_CLOSURE_REPORT.md` §2.17 (lines 569-601).

**Enforcement domain**: compile-time (capability + facade) + runtime (tier latency).

**Analyzability tier**: **T3** — semantic analyzer: detect managed-side sovereign routing (event dispatch logic в managed bus implementation instead of facade); capability declaration bypass (mod publish/subscribe без per-FQN per-tier capability declaration). Tier latency contracts (fast ≤1ms / normal ≤1 tick / background ≤N ticks) verified runtime; static rule cannot enforce timing.

**Priority**: **P0** — bus authority foundational к cross-layer event safety.

**Rule shape proposal**:
- Rule ID: **DF015** (existing)
- Severity: **Error**
- Detection pattern: semantic analyzer — `BusFacade.Publish<T>` / `Subscribe<T>` call sites must have associated capability declarations (per-FQN per-tier); detect direct native bus P/Invoke from outside facade scope.
- False-positive risk: **Low** — narrow facade API surface
- Code-fix feasibility: **Moderate** — suggest capability declaration patterns

**Related К-L**: К-L15.1 (three-tier independence sub-invariant), К-L9 (facade pattern), К-L17 (CombatFeedback layer is К-L15 fast tier consumer)

---

#### К-L15.1 — Three-tier independence (sub-invariant)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> К-L15.1 (2-layer sub-invariant, А'.7.x К-extensions cascade #0, LOCKED 2026-05-21): каждый tier owns architectural isolation at two structural layers — (1) State layer: per-tier state struct (FastTierState/NormalTierState/BackgroundTierState) с separate std::mutex, separate next_seq counter, separate subscriber map, separate pending queue where applicable; no shared mutable state between tiers. (2) Runtime layer: subscription ID space uses high 8 bits = tier identifier + low 56 bits = per-tier sequential counter (cross-tier collisions structurally impossible); df_bus_unsubscribe dispatches via tier-bit; df_bus_clear acquires three tier mutexes in fixed fast→normal→background order для deadlock safety

Plus compile-time layer materialized at А'.7.5 (3rd structural layer per `K_CLOSURE_REPORT.md` §2.18).

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L15.1 row (line 64 inline); canonical text per `K_CLOSURE_REPORT.md` §2.18 (lines 602-642).

**Enforcement domain**: hybrid (static compile-time + runtime deadlock probe).

**Analyzability tier**: **T4** — Layer 3 (compile-time, А'.7.5 materialization): native-side translation unit separation enforced by C++ compiler — Roslyn analyzer scope limited (managed-side facade per-tier isolation only); Layers 1+2 (state + runtime): native-side enforcement (per-tier mutex + tier-bit ID disambiguation) — managed analyzer can verify managed-side bus facade respects tier boundaries но cannot enforce native-side invariants. Runtime aspect: S10 cross-tier re-entrancy probe в `SchedulerExtremeTests.cs:1007` verifies deadlock safety dynamically.

**Priority**: **P1** — sub-invariant precedent; ship within A'.9.

**Rule shape proposal**:
- Rule ID: **DF015.1** (existing — NEW at A'.8 closure, sub-rule precedent)
- Severity: **Error**
- Detection pattern: managed-side — detect `BusFacade` per-tier API usage patterns (no cross-tier state sharing in facade); native-side enforcement deferred к C++ tooling (cppcheck / clang-tidy may suit better). Diagnostic per K_CLOSURE §7.2: «К-L15.1 three-tier independence violation: per-tier isolation broken at state/runtime/compile-time layer».
- False-positive risk: **Medium** — facade-level enforcement only partially covers К-L15.1 scope.
- Code-fix feasibility: **Complex** — tier isolation violations are architectural.

**Related К-L**: К-L15 parent, К-L2 (single-DLL invariant preserved through source split), К-L1 (C++20 source-split discipline)

**Open questions / refinements**: Roslyn scope vs native-side enforcement boundary — see §3.99 Q-K (analyzer infrastructure layered enforcement question).

---

#### К-L16 — Simulation tick pipeline depth (D ≥ 1, configurable 1-3, default 2)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Simulation tick pipeline depth D ≥ 1 (configurable 1-3, default 2). Simulation thread runs D ticks ahead of display thread for pipeline-managed dispatches. Cross-layer async operations (GPU compute pipeline-managed, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread. Display thread reads results from simulation tick (CurrentSimTick - D). Pipeline drain orderly at save/pause; pipeline refill orderly at load/resume. К-L16 establishes display latency invariant (D × tick_period)

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L16 row (line 65); canonical text per `K_CLOSURE_REPORT.md` §2.19 (lines 643-671).

**Enforcement domain**: hybrid (compile-time bounds + runtime drain/refill verification).

**Analyzability tier**: **T4** — Static aspect: detect hardcoded D < 1 or D > 3; `PipelineDepthSetting` bypass (direct D assignment outside settings). Runtime aspect: drain/refill ordering verified runtime (save/pause sees mid-pipeline state); display reads ahead of sim verified runtime.

**Priority**: **P2** — configurable severity per K_CLOSURE §7.2; ship after critical rules.

**Rule shape proposal**:
- Rule ID: **DF016** (existing — Warning severity, configurable per K_CLOSURE §7.2)
- Severity: **Warning** (configurable)
- Detection pattern: constant value analyzer — `D < 1` or `D > 3` literal values in pipeline-depth context; `PipelineDepthSetting` bypass detection (D variable assigned outside settings system).
- False-positive risk: **Low** — narrow pipeline-depth context
- Code-fix feasibility: **Trivial** — clamp value к [1, 3]; redirect к settings

**Related К-L**: К-L7.1 (GPU pipeline slot binding governs pipeline-managed dispatches), К-L17 (display latency invariant interacts), К-L19 (async compute queue family hardware requirement)

---

#### К-L17 — Display composition multi-layer

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Display composition multi-layer Display tick T composites multi-layer state where layers carry independent latency contracts: SimState layer (К-L16 pipeline tail for pipeline-managed, либо current sim state for К-L7 sync) → Intent overlay (current input state, ≤16ms latency) → CombatFeedback layer (К-L15 Fast tier consumers, ≤17ms event-к-visible) → Static layer (loaded assets). Composition order: SimState rendered first, intent + combat overlays composited on top, static last. Framework lives в `src/DualFrontier.Application/Display/`. Mod-registered layers declare via `[Layer(LayerType.Intent | CombatFeedback)]` attribute + capability tokens `kernel.layer.intent:{FQN}` / `kernel.layer.combat_feedback:{FQN}`

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L17 row (line 66); canonical text per `K_CLOSURE_REPORT.md` §2.20 (lines 672-705).

**Enforcement domain**: compile-time (attribute + composition order pattern).

**Analyzability tier**: **T2** — pattern detection: composition order violation (overlays before SimState in `CompositionFramework`; static layer mid-composite); `[Layer(LayerType)]` attribute presence + capability token registration for mod-registered layers; vanilla privilege detection at layer registration (vanilla layers must use same attribute + capability pattern per К-L9).

**Priority**: **P1** — display composition order is structural; ship A'.9.

**Rule shape proposal**:
- Rule ID: **DF017** (existing)
- Severity: **Error**
- Detection pattern: `CompositionFramework` call-order analyzer (SimState → Intent → CombatFeedback → Static); `[Layer(LayerType)]` attribute presence on layer classes; capability token registration call detection.
- False-positive risk: **Medium** — composition order may vary в legitimate debug scenarios; narrow к production composition flow.
- Code-fix feasibility: **Moderate** — suggest correct composition order; suggest attribute addition.

**Related К-L**: К-L15 (CombatFeedback is fast tier consumer), К-L16 (SimState pipeline depth), К-L9 («Vanilla = mods» at layer registration)

---

#### К-L18 — Mod lifecycle quiescent state precondition

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Mod lifecycle quiescent state Mod load/unload operations require simulation paused state + pipeline slots quiescent (all fences completed; no concurrent compute dispatches in-flight). Precondition enforced at native unload primitive. UI helper layer provides programmatic API: `PauseAsync` → `WaitForQuiescenceAsync(timeout)` → mod operation → `ResumeAsync`. К10.3 v2 §9.5 unload chain extended 8-step → 9-step с Step 3.6 V (Vulkan) resource cleanup placeholder

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L18 row (line 67); canonical text per `K_CLOSURE_REPORT.md` §2.21 (lines 706-736); MOD_OS_ARCHITECTURE.md §9.5 unload chain (referenced).

**Enforcement domain**: compile-time + module-boundary (semantic).

**Analyzability tier**: **T3** — semantic analyzer: detect mod load/unload code paths bypassing `PauseAsync` → `WaitForQuiescenceAsync(timeout)` → operation → `ResumeAsync` sequence (call-graph analysis); precondition helper bypass detection (`SimulationStateController` skipped); К10.3 v2 Item 42 V resource cleanup regression (vacuous success replaced с non-vacuous failure detection).

**Priority**: **P1** — mod lifecycle structural integrity; ship A'.9.

**Rule shape proposal**:
- Rule ID: **DF018** (existing)
- Severity: **Error**
- Detection pattern: call-sequence analyzer — `ModLoader.Load` / `ModLoader.Unload` invocations must be preceded by `PauseAsync` + `WaitForQuiescenceAsync` and followed by `ResumeAsync` (within same control flow); flow analysis to verify sequence.
- False-positive risk: **Medium** — async sequencing detection nuanced; may need configuration helpers.
- Code-fix feasibility: **Complex** — sequence injection is non-trivial.

**Related К-L**: К-L7.1 (pipeline quiescence query `df_pipeline_is_quiescent`), К-L16 (pipeline drain at pause), К-L9 (vanilla layer mod lifecycle uniformity), К-L19 (V resource cleanup placeholder)

---

#### К-L19 — Hardware tier commitment (Vulkan 1.3 + async compute queue)

**Statement** (verbatim from KERNEL_ARCHITECTURE.md Part 0):
> Hardware tier commitment Vulkan 1.3 + async compute queue family mandate. Target hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20-series и newer), AMD RDNA 1+ (Radeon RX 5500 и newer), Intel Arc Alchemist+ (Arc A380 и newer). Async compute queue family used для compute-side pipeline depth dispatches (К-L16 К10.3 v2); graphics queue for display rendering; copy/transfer queue for asset transfers. Кernel mandates queue family availability at startup; failure to detect async compute queue is fail-fast condition с user-facing diagnostic message pointing к README hardware requirements section

**Source citation**: `KERNEL_ARCHITECTURE.md` Part 0 К-L19 row (line 68); canonical text per `K_CLOSURE_REPORT.md` §2.22 (lines 737-773); `VULKAN_SUBSTRATE.md` v1.1 LOCKED (substrate crosscut).

**Enforcement domain**: runtime-primary (hardware capability) + compile-time API-shape detection.

**Analyzability tier**: **T5** — Runtime-primary: hardware tier check happens at `Runtime.Create` (fail-fast condition); cannot statically verify GPU vendor/generation. Static aspect (sub-detector): detect Vulkan 1.2-or-earlier API surface usage (calling extensions deprecated в 1.3); async compute queue family fallback bypass attempts (managed code path forcing fallback к graphics queue).

**Priority**: **P1** — V substrate contract; ship A'.9.

**Rule shape proposal**:
- Rule ID: **DF019** (existing — Warning severity per K_CLOSURE §7.2, V substrate contract, configurable)
- Severity: **Warning** (configurable per K_CLOSURE §7.2). **Severity refinement candidate** — see §3.99 Q-K (Error vs Warning for static API-shape sub-detector).
- Detection pattern: Vulkan API call analyzer (1.2-or-earlier extensions used in production code); queue family selection bypass detector (graphics queue used for compute dispatch).
- False-positive risk: **Medium** — Vulkan version detection nuanced (some 1.2 features promoted к 1.3 core).
- Code-fix feasibility: **Moderate** — suggest 1.3-promoted-core API alternatives.

**Related К-L**: К-L7.1 (async compute queue family для pipeline dispatches), К-L16 (pipeline depth governs async compute usage), К-L17 (display rendering uses graphics queue), К-L18 (V resource cleanup at mod lifecycle)

---

#### К-L20 — Mod API forward-compatibility [RESERVED — deferred к Agent C1]

**Statement** (per `KERNEL_ARCHITECTURE.md` Part 0 — К-L20 reserved post-Mod API lock; canonical text TBD at Mod API lock deliberation):
> [TBD at Mod API lock milestone — К-L20 represents architectural commitment future-cascade for mod ecosystem stability через grace period mechanism]

**Source citation**: `K_CLOSURE_REPORT.md` §2.23 (lines 774-796); `MOD_OS_ARCHITECTURE.md` v1.11 LOCKED (referenced for Mod API surface details).

**Enforcement domain**: deferred.

**Analyzability tier**: deferred к Domain 4 / Agent C1 per brief discipline.

**Priority**: **P2** (post-A'.9 consumer materialization — activates at Mod API lock milestone).

**Rule shape proposal**:
- Rule ID: **DF020** (reserved per K_CLOSURE §7.3 — activates at Mod API lock milestone landing)
- Severity: TBD at Mod API lock deliberation
- Detection pattern: TBD (per K_CLOSURE §7.3 expected scope: Mod API surface deviation from locked v3 manifest, Bridge mechanism bypass attempt, manifest grace period semantics violation)
- False-positive risk: TBD
- Code-fix feasibility: TBD

**Related К-L**: К-L9 (Mod parity «Vanilla = mods»; combined establish full Mod API stability commitment per K_CLOSURE §2.23)

**Deep analysis deferral**: per brief discipline, К-L20 deep analysis (Mod API restrictions analyzer-enforceable; A'.9-era preparatory rules; Mod API surface freeze; Bridge mechanism semantics; manifest grace period semantics; deprecation cadence) deferred к Domain 4 (Agent C1 / Mod OS К-L20 prep). Domain 1 (this report) flags К-L20 row для completeness but does NOT duplicate Agent C1 work.

---

### §3.99 — Open questions surfaced (for §11 Q-K candidates)

- **Q-K candidate**: Report skeleton's «K-L21» row is misnumbered — does no К-L21 exist?
  - Context: `K_CLOSURE_REPORT.md` §2.24 summary table lists 21 invariants (К-L1..L19 main + 3 subs — К-L3.1, К-L7.1, К-L15.1 — with К-L6 SUPERSEDED counted; К-L20 reserved separately). KERNEL_ARCHITECTURE.md Part 0 ends at К-L19 row + К-L20 reserved. The report skeleton row labeled "K-L21" appears to be a counting artifact (treating «21 invariants» as «К-L1 through К-L21»). Verified: NO К-L21 exists.
  - Options: (a) remove К-L21 row from skeleton matrix; (b) renumber to reflect actual 21-invariant count (К-L1..L19 + 3 subs); (c) document the К-L21-doesn't-exist explicitly в skeleton с placeholder.
  - Recommendation: **Option (a)** — remove К-L21 row from skeleton; clarify «total К-L = 21 cumulative invariants (К-L1..L19 main series with К-L6 SUPERSEDED + 3 sub-invariants К-L3.1/L7.1/L15.1)» in §3.0 summary statistics.

- **Q-K candidate**: Roslyn analyzer scope limitation для native-side К-L invariants (К-L1, К-L8, К-L15.1 Layer 3, К-L19 hardware tier).
  - Context: К-L1 (C++20 dialect), К-L8 (native owns storage), К-L15.1 Layer 3 (compile-time translation unit isolation), К-L19 (hardware GPU vendor/generation) are not enforceable от managed-side Roslyn analyzer. К-L8 has explicit «pre-commit hook alternative» per K_CLOSURE §7.3.
  - Options: (a) limit А'.9 cascade scope к managed-side rules only; (b) extend А'.9 scope к include native-side tooling (cppcheck, clang-tidy) for К-L15.1 Layer 3 + К-L1 dialect; (c) defer native-side enforcement к separate cascade (post-А'.9 «native analyzer milestone»); (d) document explicitly per-rule which enforcement tier carries the burden (Roslyn / pre-commit / runtime probe / CI native build / hardware-capability-check).
  - Recommendation: **Option (d)** — document explicitly per-rule which enforcement tier carries the burden; defer native-side tooling к post-А'.9 cascade per Option (c) but inform brief A'.9.1 deliberation так that scope expectations are clear.

- **Q-K candidate**: К-L15.1 Roslyn analyzer scope — Layer 1 (state) + Layer 2 (runtime) + Layer 3 (compile-time) — what facade enforcement makes sense from managed side?
  - Context: К-L15.1 is sub-invariant of К-L15. All three layers (state, runtime, compile-time) are native-side enforcement primarily. Managed-side facade (`BusFacade`) may enforce per-tier API usage patterns (no cross-tier state sharing in facade methods), but state/runtime/compile-time layers are native concerns. DF015.1 detection narrative в K_CLOSURE §7.2 lists all three layers as detection scope — analyzer cannot meaningfully detect Layer 3 violations.
  - Options: (a) restrict DF015.1 detection scope к managed-side facade per-tier API usage; (b) extend DF015.1 к native-side tooling (clang-tidy custom check for translation unit boundary); (c) split DF015.1 into managed-side (DF015.1a) + native-side (DF015.1b) sub-rules.
  - Recommendation: **Option (a)** for А'.9 cascade scope (managed-side facade enforcement); native-side tooling deferred к post-А'.9 cascade per Q-K above.

- **Q-K candidate**: DF019 severity refinement — Error vs Warning for static API-shape sub-detector portion.
  - Context: К-L19 has two enforcement aspects — runtime hardware capability check (fail-fast at `Runtime.Create`) and static API-shape detection (Vulkan 1.2-or-earlier API surface usage; async compute queue family fallback bypass). K_CLOSURE §7.2 assigns Warning severity к full DF019 («V substrate contract, configurable»). The static API-shape sub-detector portion (Vulkan version downgrade attempt) is arguably Error-severity (architectural commitment violation), while the configurable hardware tier scope is Warning-severity.
  - Options: (a) keep unified DF019 Warning severity (current K_CLOSURE §7.2 assignment); (b) split DF019 into DF019.1 (static API-shape, Error severity) + DF019 (hardware tier, Warning severity); (c) make DF019 severity-configurable per detection sub-pattern.
  - Recommendation: **Option (c)** for А'.9 cascade — make DF019 severity configurable per detection sub-pattern (defer split к later cascade if clarity demands); align с K_CLOSURE §7.2 «configurable severity» commitment.

- **Q-K candidate**: DF010 (К-L10 decision rule) detection scope — narrow to decision-context attributes only?
  - Context: К-L10 decision rule («§8 metrics supersede §6 mean-speed») is conceptually static-analyzable but high false-positive risk — benchmark APIs may use mean-speed legitimately для performance regression tracking. Detection narrative в K_CLOSURE §7.2 specifies «§6 «20% mean speed» reintroduced as decision criterion (mean-speed benchmark used for go/no-go architecture decision)» — narrow к decision-context.
  - Options: (a) narrow DF010 к `[ArchitecturalDecision]`-attribute-annotated methods/properties; (b) narrow DF010 к `docs/methodology/` markdown content (would require non-Roslyn tooling); (c) defer DF010 implementation pending decision-context attribute introduction.
  - Recommendation: **Option (c)** — defer DF010 implementation pending decision-context attribute introduction; document expectation в brief A'.9.1 that DF010 may not ship в first А'.9 release.

- **Q-K candidate**: DF008 (К-L8) process-invariant tooling — git pre-commit hook design scope?
  - Context: K_CLOSURE §7.3 reserves DF008 as «process invariant — git pre-commit hook alternative more appropriate than Roslyn rule». Pre-commit hook design is not Roslyn analyzer scope — needs separate tooling cascade.
  - Options: (a) defer pre-commit hook design к separate cascade (post-А'.9); (b) include pre-commit hook design in А'.9 scope; (c) document DF008 as «implementation deferred indefinitely» (К-L8 enforcement через other rules — DF011 NativeWorld single source covers ownership boundary indirectly).
  - Recommendation: **Option (a)** — defer pre-commit hook design к separate cascade; document expectation в brief A'.9.1.

- **Q-K candidate**: Code-fix provider feasibility scope для А'.9 cascade.
  - Context: Per-rule code-fix feasibility analysis in §3.2 shows mixed (Trivial / Moderate / Complex / Not feasible). А'.9 cascade scope must decide whether code-fix providers are in-scope (NuGet package size + test surface implications) или deferred к post-А'.9.
  - Options: (a) include code-fix providers in А'.9 для Trivial-feasibility rules only (DF002, DF011); (b) include code-fix providers for all rules где feasible; (c) defer code-fix providers entirely к post-А'.9 cascade.
  - Recommendation: **Option (a)** — include code-fix providers in А'.9 for Trivial-feasibility rules only (DF002 DllImport target, DF011 ManagedWorld → ManagedTestWorld в test scope); defer Moderate/Complex к post-А'.9.

---

## §4 — FORMALIZE Lessons analyzability matrix

### §4.0 — Summary statistics

**Lesson corpus state at A'.8 К-series formal closure + cascade #2 + cascade #3** (per [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Phase A' lessons» + §«Provisional Lessons», v1.12 dated 2026-05-23):

- **FORMALIZE batch (per Q-N-8-5 LOCKED Session 2 Day 2)**: **12 Lessons** enumerated per the A'.8 promotion outcomes block (METHODOLOGY.md lines 964–976). Discrepancy note: the corpus *also* contains pre-A'.8 formalized Lessons **#11**, **#20**, **#22** (К10 deliberation S6 lock, METHODOLOGY v1.8 2026-05-17, lines 865–956) under the «Phase A' lessons» heading; these are NOT in the A'.8 batch (they were already formalized at v1.8). This matrix scopes to the **12 A'.8 FORMALIZE batch** per brief expectation. The full formalized corpus is ~16 distinct Lessons (12 A'.8 + 3 v1.8 + #7, #8 originally formalized at A'.5 then strengthened at A'.8).
- **Provisional batch (post-A'.8 + post-cascade-#2 + post-cascade-#3 DEFER pool)**: **12 Provisional Lessons** (METHODOLOGY.md lines 981–993): #18, #19, #N3, #N5, #N6, #N7, #N8, #N9, #N10, #N12, #N13, #N14.
- **SUNSET**: 1 (Lesson #15, subsumed by #20) — explicitly out of scope for this matrix.

**Tier distribution (12 FORMALIZE A'.8 batch)**:
- T1: 0 | T2: 1 | T3: 0 | T4: 0 | T5: 0 | **T6: 11**

**Priority distribution (12 FORMALIZE A'.8 batch)**:
- P0: 0 | P1: 1 | P2: 0 | **P3: 11**

**Of T1–T4 analyzable Lessons in FORMALIZE batch**: **1 candidate rule** proposed (Lesson #8 — atomic compilable commit shape, indirectly enforceable via auxiliary brief-template lint / pre-commit hook tooling, **not Roslyn semantic analyzer**).

**Key finding**: 11 of 12 FORMALIZE Lessons are governance / process / methodology Lessons (T6 documentation-only). This is **correct and expected** — they govern brief authoring, deliberation discipline, and architectural reasoning at the methodology layer (METHODOLOGY.md authority). Analyzer scope per К-L invariants (DF### family per [ANALYZER_RULES.md §1](./ANALYZER_RULES.md#1--rule-taxonomy-at-a8-closure)) is structurally separate from Lessons scope. Lessons enforce *how briefs are authored*; DF### rules enforce *what code shapes K-Lxx invariants permit*. The two layers do not overlap.

**Provisional batch finding**: Of 12 Provisional Lessons, **2** are HIGH promotion proximity per cascade chronicle (**#N12** Defensive Reserved Stub Pattern — already at 2 applications with refined semantic; **#N13** commit integrity verification — explicitly tagged «promotes quickly» in METHODOLOGY.md line 992), and 1 is MEDIUM-HIGH (**#N14** Phase 0 empirical state coverage — likewise tagged «likely promotes quickly» line 993). None are pure code-pattern Lessons amenable to DF###-style Roslyn enforcement; **#N12** has the weakest T4 hybrid analyzability if promoted (the «stub fires only in tests» vs «stub fires in production composition» split can be partially semantically inferred, but call-graph-from-composition-root analysis is beyond Roslyn syntactic scope).

---

### §4.1 — FORMALIZE Lessons matrix (12 A'.8 batch)

| Lesson | Statement excerpt (≤80 chars) | Tier | Priority | Rule shape | Notes |
|---|---|---|---|---|---|
| #7 | A brief that prescribes an API must transcribe the API, not paraphrase | T6 | P3 | — | Brief-authoring discipline; about brief content, not code |
| #8 | A brief that splits a change into N steps must prove N-1 valid | T2 | P1 | DL008 (auxiliary tooling) | Indirect commit-shape enforcement via pre-commit hook; not Roslyn |
| #9 + #9.1 | Survey phase before brief authoring when scope undefined | T6 | P3 | — | Process discipline; survey vs brief mode switch |
| #10 | Architecture audit + technical debt inventory in one pass | T6 | P3 | — | Pass methodology; documentation-only |
| #14 | Pre-existing drift cleanup as separate cascade | T6 | P3 | — | Cascade scoping discipline; governance |
| #16 | Brief length scales with deliberation complexity, not execution | T6 | P3 | — | Brief sizing heuristic |
| #17 | Performance reasoning tactical; architectural integrity strategic | T6 | P3 | — | Deliberation framing discipline (merge candidate w/ #20) |
| #21 | Redundancy check before default-inclusion (К-L14 complement) | T6 | P3 | — | Deliberation mental check |
| #25 | Implementation depth follows consumer materialization | T6 | P3 | — | Anti-speculation discipline; partially expressible as «no empty interfaces» lint but governance-shaped |
| #26 | Cross-substrate scope splitting | T6 | P3 | — | Milestone scoping; governance |
| #27 | Render/compute workload exercises prior substrate primitives | T6 | P3 | — | Milestone authoring discipline (V substrate sequencing) |
| #N2 | Mid-session brief amendment via halt-before-damage Path 2 | T6 | P3 | — | Halt protocol; methodology process |

---

### §4.2 — Per-Lesson detailed analysis

#### Lesson #7 — A brief that prescribes an API must transcribe the API, not paraphrase

**Statement** (verbatim from METHODOLOGY.md lines 847–853):
> «When a brief tells the executor to call a constructor, a helper, or a file path, the brief author must open the actual source and copy the real signature into the brief at authoring time. «K2-era registry ready» is a note, not a signature. CAPA-2026-05-13's lesson («read entry-point files in full») addressed transitional-state comments; this lesson addresses *API surface*. A brief is a contract for mechanical execution; a contract cannot reference an interface it has not read.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Phase A' lessons» → «Lesson #7 — A brief that prescribes an API must transcribe the API, not paraphrase it» (lines 847–853). Origin A'.5 K8.3+K8.4 closure 2026-05-14; strengthened at A'.8.

**Lesson kind**: process (brief authoring discipline).

**Analyzability tier**: **T6** — Lesson governs *brief content* (the .md document the architect authors), not code shapes the executor produces. A Roslyn analyzer operates on C# source trees; briefs are markdown. The only auxiliary-tooling expression would be «brief-template lint: any `new XYZ(…)` or `XYZ.Method(…)` reference in brief markdown must match a signature actually present in the cited source files» — but that requires the brief to *declare* its API citations machine-readably, which is itself a brief-authoring convention change. Outside Roslyn analyzer scope.

**Priority**: **P3** — Documentation-only acknowledgment. Already enforced by halt-before-damage discipline (executor halts on API mismatch at Phase 0 / build gate).

**Related К-L**: none (К-Lxx invariants govern code shapes; this Lesson governs brief authoring).

**Open questions / refinements**: brief-template machine-readable API-citation block could enable lint-style enforcement at A'.9.X+ phase; Q-K candidate (see §4.99).

---

#### Lesson #8 — A brief that splits a change into N steps must prove each of the N−1 intermediate states is valid

**Statement** (verbatim from METHODOLOGY.md lines 855–863):
> «Before a brief prescribes an incremental sequence, the author must walk each intermediate state and confirm it compiles, passes tests, and is architecturally coherent. If an intermediate state cannot be made valid, the change is not incrementally divisible — the atomic unit is larger than one step, and the brief must say so.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Phase A' lessons» → «Lesson #8 — A brief that splits a change into N steps must prove each of the N−1 intermediate states is valid» (lines 855–863). Origin A'.5 K8.3+K8.4 closure 2026-05-14 mid-transition-drift halt; strengthened at A'.8 with К10.3 v2 multi-document evidence + А'.7.x 13-atomic precedent.

**Lesson kind**: hybrid (process + commit-shape; the commit shape is the executable artifact).

**Analyzability tier**: **T2** — *with auxiliary tooling, not Roslyn*. Each commit's «compiles + tests pass» property is mechanically checkable: a pre-commit hook (or CI per-commit check) can run `dotnet build` and `dotnet test` against the staged state. This is **not** a Roslyn rule (Roslyn analyzers see source trees, not git history). It is an **auxiliary commit-validation tool** (git hook / CI matrix per commit / `git rebase --exec`). The Lesson maps cleanly to such a tool, hence T2 syntax-tree-equivalent analyzability via commit-level static check, but the *enforcement substrate* is git tooling, not the Roslyn analyzer package planned at A'.9.

**Priority**: **P1** — High value (atomic-commit discipline is load-bearing per Lesson #8 + Lesson #14 + multiple cascade closure protocols), but **ship target is A'.9.X auxiliary cascade**, not the core Roslyn A'.9.1 analyzer release. The atomic-commit invariant is currently enforced by closure-protocol per-test-suite run (§12.7) on the *closure commit*, not per-intermediate-commit; pre-commit hook would close that gap.

**Rule shape** (auxiliary tooling — NOT Roslyn analyzer):
- Rule ID: **DL008** (proposed; *DualFrontier Lesson #8*; namespace TBD per §4.99 Q-K candidate)
- Severity: **Error** (commit blocked if intermediate state fails)
- Detection pattern: per-commit `dotnet build -c Release` exit code + `dotnet test` exit code on the post-commit working tree state. Implemented as `tools/governance/check_atomic_commits.ps1` invoked from `git rebase --exec` or CI matrix.
- False-positive risk: **Low** — build success is mechanical; rare cases of «commit relies on uncommitted environment artifact» surface as the failure mode this Lesson is meant to catch.
- Code-fix feasibility: **Not feasible** (the fix is rewriting commit history — squash, split, reorder — which is destructive and out of analyzer scope; the tool reports the bad commit, human resolves via `git rebase -i`).

**Related К-L**: none directly; tangentially complements К-L14 «atomic commit + closure verification» falsifiability commitments (per [K_CLOSURE_REPORT.md §2](./K_CLOSURE_REPORT.md)).

**Open questions / refinements**: scoping decision — should DL008 enforcement be branch-local (claude/... feature branches) or main-only (only on merge)? Per-commit CI cost on long cascades (К-extensions cascade #3 ran 14 commits; per-commit `dotnet build` + `dotnet test` adds ~14× per-cascade CI time). Q-K candidate: «DL008 enforcement scope and CI cost trade-off» — see §4.99.

---

#### Lesson #9 + #9.1 sub — Survey phase before brief authoring + placeholder vs production status

**Statement** (verbatim from METHODOLOGY.md lines 997–1002):
> «Architecture recon surveys precede brief authoring когда «fully update LOCKED» arises и scope is undefined. Briefs are for known scope; surveys define scope. Switch authoring → survey mode.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons» → «Lesson #9 — Survey phase before brief authoring (provisional)» (lines 997–1002). PROMOTED from Provisional at A'.8 (line 967). Sub-lesson #9.1 «placeholder vs production status» added at promotion (per line 967 «#9 + #9.1 sub»); full sub-Lesson text лежит в [K_CLOSURE_REPORT.md §6.2](./K_CLOSURE_REPORT.md#62--per-lesson-formalize-rationale-12-lessons) per cross-reference at METHODOLOGY.md line 995.

**Lesson kind**: process (brief authoring vs survey authoring mode selection).

**Analyzability tier**: **T6** — Lesson governs the *meta-decision* of «which document type to author for this scope» before any code is written. There is no code surface to analyze. The Lesson's enforcement substrate is the architect's deliberation discipline (Q-K-style lock «is scope known yes/no? if no → survey first»).

**Priority**: **P3** — Documentation-only. Enforced by halt protocol (architect halts brief authoring if scope undefined; switches to survey mode).

**Related К-L**: none.

**Open questions / refinements**: A'.9.0 reconnaissance brief IS itself an application of this Lesson (this very cascade is the survey before A'.9.1 analyzer brief authoring). Self-referential evidence captures application #2; promotion track on solid ground.

---

#### Lesson #10 — Architecture audit + technical debt inventory in one pass

**Statement** (verbatim from METHODOLOGY.md lines 1004–1009):
> «Debt signals where compromise hid architectural truth. Combining audit + debt inventory surfaces both в same documentation pass.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons» → «Lesson #10 — Architecture audit + technical debt inventory in one pass (provisional)» (lines 1004–1009). PROMOTED from Provisional at A'.8 (line 968).

**Lesson kind**: process (cascade authoring discipline — bundle audit + debt inventory).

**Analyzability tier**: **T6** — Governs cascade-document authoring shape (single pass vs split passes). No code surface.

**Priority**: **P3** — Documentation-only.

**Related К-L**: none.

**Open questions / refinements**: none.

---

#### Lesson #14 — Pre-existing drift cleanup as separate cascade

**Statement** (verbatim from METHODOLOGY.md lines 1011–1017):
> «Pre-existing drift surfaced during prior cleanup gets deferral notes. Subsequent cleanup must respect deferrals, не override them without re-deliberation. Self-consistent governance behavior across cleanup cycles.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons» → «Lesson #14 — Pre-existing drift cleanup as separate cascade (refined: deferrals must be respected) (provisional)» (lines 1011–1017). PROMOTED from Provisional at A'.8 (line 969); second clean application surfaced — A'.7.x bus refactor on `claude/scheduler-stress-test-KmVM3` + Godot removal cascade on `claude/godot-removal-deliberation-Vfg2R` both separate-branch atomic.

**Lesson kind**: process (cascade scoping discipline).

**Analyzability tier**: **T6** — Governs cascade boundary decisions. The «drift cleanup belongs on separate branch» discipline operates at the git-branch / brief-authoring layer, not code shape.

**Priority**: **P3** — Documentation-only.

**Related К-L**: none directly; complements К-L14 evidence accumulation discipline (separate-branch cleanup cascades are clean К-L14 verifications per [K_L14_EVIDENCE_DASHBOARD.md](./K_L14_EVIDENCE_DASHBOARD.md)).

**Open questions / refinements**: none.

---

#### Lesson #16 — Brief length scales with deliberation complexity, not execution scope

**Statement** (verbatim from METHODOLOGY.md lines 1022–1026):
> «Brief length scales с amount of decision-recording needed, не amount of mechanical work. Comprehensive defensive briefs preferred for context safety across session gaps.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons» → «Lesson #16 — Brief length correlates с deliberation complexity, не execution scope (provisional)» (lines 1022–1026). PROMOTED from Provisional at A'.8 (line 970).

**Lesson kind**: process (brief sizing heuristic).

**Analyzability tier**: **T6** — Brief length is a continuous quality dimension; no falsifiable threshold suitable for mechanical check. Could express as `tools/governance/check_brief_length.ps1` flagging «brief < N lines on cascade > M commits» — but the relationship is heuristic, not hard rule, and would generate noise.

**Priority**: **P3** — Documentation-only.

**Related К-L**: none.

**Open questions / refinements**: none.

---

#### Lesson #17 — Performance reasoning tactical vs strategic

**Statement** (verbatim from METHODOLOGY.md lines 1028–1032):
> «Performance optimization is tactical; architectural integrity is strategic. Migration motivation must engage с architectural integrity, не just performance.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons» → «Lesson #17 — Performance reasoning tactical vs strategic (provisional, may merge с #20)» (lines 1028–1032). PROMOTED from Provisional at A'.8 (line 971). Per the «Status» line: «refined elements folded into Lesson #20 «Refined element 4». May merge entirely с #20 at next deliberation.»

**Lesson kind**: process (deliberation discipline — reasoning mode selection).

**Analyzability tier**: **T6** — Governs deliberation framing. No code surface.

**Priority**: **P3** — Documentation-only.

**Related К-L**: none directly; tangentially related to К-L14 default-inclusion bias (per Lesson #20 sub-pattern).

**Open questions / refinements**: per METHODOLOGY.md line 1032, #17 «may merge entirely с #20 at next deliberation». Q-K candidate: «Lesson #17 vs #20 merge decision» — see §4.99.

---

#### Lesson #21 — Redundancy check before default-inclusion (К-L14 complement)

**Statement** (verbatim from METHODOLOGY.md lines 1048–1053):
> «Default-inclusion bias под К-L14 не применяется когда proposed item дублирует функцию existing item. Mental check: «что **уникально** добавляет item к architectural surface?» Если ответ «ничего» — exclude. Complement к Lesson #20 (don't reject on cost grounds), не contradiction.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons» → «Lesson #21 — Redundancy check before default-inclusion (К-L14 complement) (provisional)» (lines 1048–1053). PROMOTED from Provisional at A'.8 (line 972).

**Lesson kind**: process (deliberation mental check).

**Analyzability tier**: **T6** — Mental discipline during architectural deliberation. No code surface.

**Priority**: **P3** — Documentation-only.

**Related К-L**: К-L14 (complement — K-L14 is default-inclusion, #21 is redundancy filter that gates default-inclusion).

**Open questions / refinements**: none.

---

#### Lesson #25 — Implementation depth follows consumer materialization

**Statement** (verbatim from METHODOLOGY.md frontmatter version 1.11 line 19, refined):
> «Design abstractions when consumer materializes AND structurally eliminate test-lying surface — empty stub implementations что pass tests by doing nothing constitute architectural debt independent of speculation discipline.»

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) v1.11 changelog block (line 19) — refined statement per К-extensions cascade #2 closure 2026-05-23. FORMALIZED at A'.8 with A'.7.5 compile-time layer materialization as third application (line 973). Full Lesson narrative anchored к [K_CLOSURE_REPORT.md §6.2](./K_CLOSURE_REPORT.md#62--per-lesson-formalize-rationale-12-lessons).

**Lesson kind**: hybrid (process + code-pattern; the «empty stub that passes tests» is a code shape).

**Analyzability tier**: **T6** primarily, with weak T3 sub-component. The «consumer materializes» judgment is architectural deliberation (T6). The «empty stub implementation that passes tests by doing nothing» sub-clause has a partial code surface: a Roslyn rule could flag method bodies that are *empty* (no statements, no `throw`) on types implementing project-defined interfaces — but distinguishing «legitimate no-op stub» from «forward-options stub» from «defensive reserved stub per #N12 sub-pattern A» requires semantic context the analyzer doesn't have. Lesson #N12 (Provisional, near-promotion) explicitly carves out the «paired discipline с Lesson #25 refined» split — see §4.3 #N12 expansion.

**Priority**: **P3** — Documentation-only at A'.9.1 release. If Lesson #N12 promotes and combined Lesson #25 + #N12 rule shape stabilizes (sub-pattern A vs B detection), revisit for A'.9.X.

**Related К-L**: none.

**Open questions / refinements**: combined #25 + #N12 rule shape Q-K candidate — see §4.99 and §4.3 #N12.

---

#### Lesson #26 — Cross-substrate scope splitting

**Statement** (verbatim summary from METHODOLOGY.md line 974, anchored к [K_CLOSURE_REPORT.md §6.2](./K_CLOSURE_REPORT.md#62--per-lesson-formalize-rationale-12-lessons)):
> «Lesson #26 — Cross-substrate scope splitting [FORMALIZED at A'.8]»

(Full Lesson text resides in K_CLOSURE_REPORT.md §6.2 per the cross-reference at METHODOLOGY.md line 995; this matrix uses the line-974 summary as the canonical excerpt.)

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons → A'.8 promotion outcomes» (line 974); detailed narrative in [K_CLOSURE_REPORT.md §6.2](./K_CLOSURE_REPORT.md#62--per-lesson-formalize-rationale-12-lessons).

**Lesson kind**: process (milestone scoping — split cascades across substrate boundaries G/V/etc.).

**Analyzability tier**: **T6** — Milestone authoring discipline. No code surface.

**Priority**: **P3** — Documentation-only.

**Related К-L**: tangentially related to К-L7.1 (GPU pipeline slot binding) and the V substrate primitive split (V0/V1/V2 per [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md)).

**Open questions / refinements**: none.

---

#### Lesson #27 — Render/compute workload exercises prior substrate primitives

**Statement** (verbatim summary from METHODOLOGY.md line 975):
> «Lesson #27 — Render/compute workload exercises prior substrate primitives [PROMOTED from Provisional at A'.8; third application surfaced — А'.7.x bus stress workload surfaced 5 Bugs in К10.2 primitives]»

(Full Lesson text in K_CLOSURE_REPORT.md §6.2 per cross-reference.)

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons → A'.8 promotion outcomes» (line 975). PROMOTED from Provisional at A'.8; third application surfaced from А'.7.x bus stress workload.

**Lesson kind**: process (milestone authoring — workload sequencing across substrate primitives).

**Analyzability tier**: **T6** — Cascade sequencing discipline. No code surface.

**Priority**: **P3** — Documentation-only.

**Related К-L**: К-L13 (wake-source diversity — bus stress workload as К-L13 evidence). А'.7.x cascade evidence anchors the third application.

**Open questions / refinements**: none.

---

#### Lesson #N2 — Mid-session brief amendment via halt-before-damage Path 2

**Statement** (verbatim summary from METHODOLOGY.md line 976):
> «Lesson #N2 — Mid-session brief amendment via halt-before-damage Path 2 [FORMALIZED at A'.8; A'.7.x δ1-δ3 application добавлено]»

(Full Lesson text in K_CLOSURE_REPORT.md §6.2 per cross-reference.)

**Source citation**: [METHODOLOGY.md](../methodology/METHODOLOGY.md) §«Provisional Lessons → A'.8 promotion outcomes» (line 976). FORMALIZED at A'.8; A'.7.x δ1-δ3 application added.

**Lesson kind**: process (halt protocol — mid-session brief amendment when Phase 0 evidence contradicts brief assumption).

**Analyzability tier**: **T6** — Halt protocol governs how the executor escalates and how the architect amends mid-session. No code surface.

**Priority**: **P3** — Documentation-only. Enforced by halt-before-damage Path 2 procedure documented in brief templates.

**Related К-L**: К-L14 (soft-halt rate falsifiability criterion 6 per Q-N-8-7 LOCKED — halt-before-damage Path 2 surfaces in К-L14 evidence stream).

**Open questions / refinements**: none.

---

### §4.3 — Provisional Lessons analyzability assessment

Brief scoring of 12 Provisional Lessons in the DEFER pool per [METHODOLOGY.md lines 981–993](../methodology/METHODOLOGY.md). Focus per brief: which Provisional Lessons are NEAR PROMOTION + analyzer-relevant if promoted.

| Provisional Lesson | Statement excerpt (≤80 chars) | Promotion proximity | Analyzability if promoted |
|---|---|---|---|
| #18 | Boundary crossing batching pattern (symmetric fwd/reverse) | low | T6 |
| #19 | On-demand activation multi-wake-source (TickScheduler one wake) | low (sunset candidate) | T6 |
| #N3 | К-L9 mod-facing boundary Contracts/Application | low | T6 |
| #N5 | Independent investigation branch as К-L14 evidence | low | T6 |
| #N6 | Test fixture cleanup discipline as invariant | medium | T2 (auxiliary: pre-commit hook scrubs `bin/Fixtures/` before fixture suite run) |
| #N7 | Gap audit between AUTHORED and Q-N ratification | low | T6 |
| #N8 | Pre-flight reproduction disproves diagnostic hypothesis | low | T6 |
| #N9 | Closure-protocol gap as soft-halt class (К-L14 crit 6) | low | T6 |
| #N10 | Brief leaning vs Phase 0 evidence reduction discipline | low | T6 |
| **#N12** | **Defensive Reserved Stub Pattern (Sub-A throws / Sub-B silent)** | **HIGH (2 applications + sub-pattern split refined)** | **T4 hybrid (partial Roslyn semantic + composition-root reflection)** |
| **#N13** | **Commit integrity verification before commit** | **HIGH («promotes quickly likely» per METHODOLOGY.md L992)** | **T2 (auxiliary: pre-commit hook diffs staged vs message claims)** |
| **#N14** | **Phase 0 reads empirical assumed-state coverage** | **MEDIUM-HIGH («likely promotes quickly» L993)** | **T6 (brief-authoring discipline; not Roslyn)** |

#### Expanded analysis for HIGH-promotion + T1–T4 analyzability candidates

##### Lesson #N12 — Defensive Reserved Stub Pattern (HIGH promotion; T4 hybrid)

**Statement** (verbatim from METHODOLOGY.md line 991):
> «When interface либо implementation should exist structurally without real implementation (dormant abstractions, reserved tiers, forward-options), implementations MUST handle invocation honestly per production composition context. **Sub-pattern A (test-only-fires)**: when command type CANNOT fire в production composition (test-only invocation pathway), use defensive throw (NotImplementedException) so tests exercising stub fail loudly — prevents lying-test surface. **Sub-pattern B (production-fires, REFINED К-extensions cascade #3 2026-05-23)**: when command type DOES fire в production composition (silent-throw would crash production), use silent stub body (empty method) с honest «DO NOT TEST — stub has no observable behavior; tests would lie by passing trivially» documentation; test discipline (Q-H-6 pattern) prevents test coverage.»

**Promotion proximity**: HIGH. Per METHODOLOGY.md line 991, Lesson #N12 has 2 cascade applications (cascade #2 sub-pattern A first application; cascade #3 sub-pattern B first application + sub-pattern A retention), with «promotion gate (REFINED)» requiring «third application с substantially-different sub-pattern usage OR sub-pattern usage в different domain». Memory notes the same («promotion criterion amended к require differentiating sub-pattern»). Concrete promotion candidate domains: HUD primitives reserved skeleton, audio dispatcher reserved skeleton, network sync skeleton (per METHODOLOGY.md line 991 enumeration).

**Tier if promoted**: **T4 hybrid**. Two halves:
- Sub-pattern A (defensive throw on test-only stub) — **T3 partial**: Roslyn rule can flag «method body is `throw new NotImplementedException(...)` only» on types implementing project interface; cannot semantically distinguish «legitimate defensive stub» from «forgot to implement». Lesson would require pairing the throw with a structured XML doc tag (e.g. `<remarks>defensive-reserved-stub: sub-pattern-A: production-cannot-fire</remarks>`) so the rule could allow-list intentional uses.
- Sub-pattern B (silent stub with «DO NOT TEST» doc) — **T4 hybrid**: empty method body detection is syntactically trivial; the «DO NOT TEST» discipline requires a *test-discoverer* component (separate from compile-time analyzer) that refuses to load tests that exercise such methods. Composition-root semantic analysis («does X fire in production?») is beyond Roslyn syntactic scope — relies on hand-authored XML doc declaration.

**Rule shape** (if promoted):
- Rule ID: **DL012-A** (sub-pattern A: defensive throw) + **DL012-B** (sub-pattern B: silent stub) — proposed; namespace per §4.99 Q-K
- Severity: Warning (advisory; allow-list via XML doc tag converts to clean)
- Detection pattern: syntactic for body shape; XML-doc-tag-based allow-listing; companion test-discoverer for sub-pattern B
- False-positive risk: **Medium** without XML-doc allow-list; **Low** with allow-list enforced
- Code-fix feasibility: **Trivial** for the doc-tag addition; **Not feasible** for the architectural decision «should this be sub-pattern A or B» (that requires composition-root reads per Lesson #N14)

**Notes**: This is the *only* Provisional Lesson with non-trivial analyzer-rule-shape potential. The «paired discipline с Lesson #25 refined» note in METHODOLOGY.md line 991 suggests combined Lesson #25 + #N12 rule design at A'.9.X+. Q-K candidate — see §4.99.

---

##### Lesson #N13 — Commit integrity verification before commit (HIGH promotion; T2 auxiliary)

**Statement** (verbatim from METHODOLOGY.md line 992):
> «Commit messages MUST не claim mutations что are не actually staged. Before committing, execution agent verifies `git diff --cached` content matches commit message claims. Mismatches trigger correction commits (per Lesson #8 discipline — separate commit, не amend), не silent commits с false claims.»

**Promotion proximity**: HIGH. METHODOLOGY.md line 992 explicitly states «promotes quickly likely given how often message/diff mismatch potential exists». First observation cascade #2 α1 (a52996d / 8a8e507 correction); cascade #3 α0 explicit pre-commit verification. Awaiting one more application с different mutation type (binary / generated file / test fixture omission).

**Tier if promoted**: **T2** — auxiliary tooling, NOT Roslyn. The mismatch detection is mechanical on the git-staging layer: a pre-commit hook can extract claim-tokens from commit message (e.g., «sln mutation», «add file X», «modify Y») and cross-reference against `git diff --cached --name-only` / `git diff --cached --stat`. Natural-language extraction is the hard part; structured commit-message conventions (per [docs/methodology/CODING_STANDARDS.md](../methodology/CODING_STANDARDS.md) scope-prefix discipline `feat(scope): ...`) provide partial structure but не full machine-readable claim list.

**Rule shape** (if promoted):
- Rule ID: **DL013** (proposed; namespace per §4.99 Q-K)
- Severity: Warning (advisory; pre-commit hook flags potential mismatch, agent decides)
- Detection pattern: regex extraction of file-path / scope-prefix claims from commit message body; cross-reference against staged diff stat
- False-positive risk: **Medium-High** for natural-language commit message bodies; **Low** if commit-message-format convention extended с machine-readable «mutations:» block
- Code-fix feasibility: **Not feasible** (agent rewrites commit message manually — the tool flags, doesn't fix)

**Notes**: Could be combined with DL008 (Lesson #8 atomic-commit shape) into a single `tools/governance/check_commit.ps1` pre-commit hook covering both Lessons. Q-K candidate combined Lessons #8 + #N13 tool shape.

---

##### Lesson #N14 — Phase 0 reads empirical assumed-state coverage (MEDIUM-HIGH promotion; T6)

**Statement** (verbatim from METHODOLOGY.md line 993):
> «Phase 0 brief reads MUST cover assumed-state directories + files empirically, не by assumption. When brief assumes «directory X contains only Y» либо «no untracked files в Z» либо «command type Q fires only от tests», execute `git ls-files` + `Get-ChildItem` (для directory state) OR read source code paths (для production composition state) before commit cascade begins.»

**Promotion proximity**: MEDIUM-HIGH. Per METHODOLOGY.md line 993 «likely promotes quickly given how often assumption-vs-empirical gap matters». 2 first observations: cascade #2 α1 (directory state divergence) + cascade #3 α0 (production composition divergence). Awaiting one more application с different state-class (asset format, binary header, config file content).

**Tier if promoted**: **T6** — Lesson governs *brief authoring* (Phase 0 reads must cover empirically). The artifact is the brief's Phase 0 section. The «empirical reads» themselves are bash / PowerShell commands. No code surface for Roslyn enforcement; no auxiliary tooling target either (the Lesson is a brief-template discipline, not a runtime/build invariant).

**Priority if promoted**: **P3** — Documentation-only.

---

### §4.99 — Open questions surfaced (for §11 Q-K candidates)

- **Q-K-§4-1: DL### namespace decision for Lesson-derived auxiliary rules**
  - Context: This matrix proposes `DL###` (DualFrontier Lesson) for Lesson-derived rules to avoid collision with `DF###` (К-L invariant rules per ANALYZER_RULES.md §1). Of 12 FORMALIZE A'.8 Lessons, 1 (Lesson #8) maps к auxiliary tooling; of 12 Provisional Lessons, 3 (#N12, #N13, plus weak candidate from #N6) map к potential rule shape. Total: ≤4 candidate rules at A'.9.1, modest growth potential.
  - Options:
    (a) `DL###` namespace as proposed (clear separation from `DF###`)
    (b) Extend `DF###` namespace (DF021+) keeping single analyzer namespace
    (c) Defer namespace decision; tool prefix `tools/governance/check_*.ps1` for auxiliary tooling — no Roslyn rule ID needed (Lesson rules are explicitly NOT Roslyn rules per matrix tier analysis)
  - Recommendation: **(c)** with optional reservation of `DL###` namespace for future Roslyn-enforceable Lesson rules (currently none, but Lesson #N12 sub-pattern A could materialize as DL012-A if combined with Lesson #25 refined). DL### reservation as forward-option, не immediate use.

- **Q-K-§4-2: Lesson #8 DL008 enforcement scope and CI cost trade-off**
  - Context: DL008 (atomic compilable commit) requires per-commit `dotnet build` + `dotnet test`. К-extensions cascade #3 ran 14 commits; per-commit CI cost = ~14× per-cascade. К-extensions cascade #0 (А'.7.x) ran 13 atomic commits — same multiplier.
  - Options:
    (a) Branch-local enforcement: `git rebase --exec` opt-in by executor at cascade start; main-only CI runs closure verification only (current state)
    (b) CI matrix per-commit on PR submission (cost ~14× per cascade × CI cost; latency impact on PR ratification cadence)
    (c) Closure-protocol expansion: §12.7 closure verification iterates over all commits in cascade (PowerShell loop), runs per-commit checkout + build + test (cost amortized to closure event, не per-commit)
  - Recommendation: **(c)** — enforce at closure protocol (matches existing §12.7 single-event cost model); preserve (a) as opt-in mid-cascade tool for executor self-verification.

- **Q-K-§4-3: Lesson #25 + Lesson #N12 combined rule shape (if #N12 promotes)**
  - Context: METHODOLOGY.md line 991 explicitly notes «paired discipline с Lesson #25 refined». If #N12 promotes at А'.9.X cascade post-A'.9.1, combined rule shape covering #25 («design abstractions when consumer materializes; structurally eliminate test-lying surface») + #N12 sub-A («defensive throw on test-only stub») + #N12 sub-B («silent stub + DO NOT TEST на production-fires stub») could be coherent single analyzer rule.
  - Options:
    (a) Three separate rules DL025, DL012-A, DL012-B
    (b) Combined rule DL025+12 (single analyzer class, three diagnostic IDs internal)
    (c) Defer combined rule design until #N12 reaches promotion (3rd application surfaced)
  - Recommendation: **(c)** — premature combined-rule design before #N12 promotion is over-engineering. Revisit at А'.9.X cascade when #N12 promotes.

- **Q-K-§4-4: Lesson #17 vs Lesson #20 merge decision**
  - Context: METHODOLOGY.md line 1032 explicitly notes Lesson #17 «may merge entirely с Lesson #20 at next deliberation». Both formalized; #17 promoted at A'.8 with «status: refined elements folded into Lesson #20 'Refined element 4'» (line 1032).
  - Options:
    (a) Formal merge: SUNSET #17 → subsumed by #20 (parallel to Lesson #15 SUNSET → subsumed by #20 at A'.8 per METHODOLOGY.md line 979)
    (b) Keep separate (current state) с cross-reference
    (c) Defer to A'.9.0 cascade closure deliberation
  - Recommendation: **(c)** — surface at A'.9.0 closure for Crystalka ratification; #17 vs #20 merge is governance bookkeeping, не A'.9 analyzer scope.

- **Q-K-§4-5: Lesson #N6 (test fixture cleanup) auxiliary tooling shape if promoted**
  - Context: Provisional Lesson #N6 (test fixture cleanup discipline as invariant) was surfaced by A'.7.x К10.3 v2 soft-halt root cause (per METHODOLOGY.md line 986 + frontmatter v1.9 line 23). К-L14 falsifiability criterion 6 («soft-halt rate exceeds X% across N consecutive cascades») introduced as Provisional per Q-N-8-7 LOCKED references this class.
  - Options:
    (a) Pre-test hook scrubs `tests/*/bin/*/Fixtures/` before fixture-consuming suite runs (cost: ~5s per suite invocation)
    (b) Closure protocol §12.7 step 1 already mandates Modding suite explicit run; assumes clean Fixtures/ implicitly — extend with explicit «rm -rf bin/Fixtures && rebuild» pre-step
    (c) Defer until #N6 promotes (currently «medium» promotion proximity)
  - Recommendation: **(b)** — extend §12.7 step 1 wording at next METHODOLOGY closure to mandate Fixtures/ rebuild before mandatory Modding suite run. Does not require #N6 promotion (the discipline is already implicit in CAPA-2026-05-21-A_PRIME_7_X-K10_3-V2-SOFT-HALT).

---

## §5 — Cascade #2 + #3 surfaced rule candidates

### §5.0 — Summary statistics
- Cascade #2 candidates extracted: 5
- Cascade #3 candidates extracted: 5
- T1-T4 (analyzable): 9
- T5-T6: 1
- P0 candidates: 2 | P1: 4 | P2: 3 | P3: 1

### §5.1 — Cascade #2 candidates (Godot deprecation)

#### C2-Rule-1: IRenderCommand marker-only — commands MUST NOT have Execute() method

**Source**: `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md` §6.3 (Forward consideration: A'.9 Roslyn analyzer preparation) + §1 S-LOCK-5 (IRenderCommand as pure marker interface)

**Statement** (verbatim from brief):
> «Future rules могут leverage cascade #2 architectural decisions (е.g. IRenderCommand marker-only pattern can be enforced via analyzer rule «commands MUST не have Execute() method»)»

Plus S-LOCK-5 normative anchor:
> «`IRenderCommand` interface stripped к pure marker — NO Execute() method, NO body methods of any kind. All Bridge.Commands records lose Execute() implementations.»
>
> «**Halt condition**: If execution agent retains Execute() in any form (even с throw), halt + verification — pattern requires marker-only.»

**Linkage**: Q-G-3 LOCKED (cascade #2 deliberation); Lesson #25 refined (lying-test prevention); cascade #2 β1.6 + β1.7 enforcement (commits 339ed3a).

**Analyzability tier**: **T2** — Static syntax-tree analysis. Detect any type implementing `IRenderCommand` (semantic check on base list) that declares a member named `Execute` of any signature. Marker interface check (no members declared on `IRenderCommand`) is a separate T1 sub-check.

**Priority**: **P1** — High. Architectural invariant codified by cascade #2 LOCKED Q-G-3 decision; regression here re-introduces lying-test surface (Lesson #25 refined violation). Pattern stability matters for future Command additions (cascade #4+). Not P0 because catch-on-commit via CI suffices — no existing violation to prevent at A'.9.1 ship time (cascade #2 already enforced cleanup).

**Rule shape**:
- Rule ID: **DC001** (cascade-derived namespace proposal — DC### chosen to disambiguate from DF### К-L rules and DL### Lesson rules; collision flagged as Q-K candidate below)
- Severity: **Error** — structural contract violation; consistent with S-LOCK-5 halt condition severity
- Detection pattern: **Semantic** — `ITypeSymbol.AllInterfaces` includes `IRenderCommand` → check `GetMembers("Execute")` is empty on the declaring type. Sub-rule: interface declaration `IRenderCommand` itself must have zero members (verifies marker-only at source).
- False-positive risk: **Low** — `IRenderCommand` is a known fully-qualified type (`DualFrontier.Application.Bridge.IRenderCommand`); only legitimate consumers are 6 Bridge.Commands records; `Execute` is a domain-specific method name in this context.
- Code-fix feasibility: **Moderate** — code-fix can delete the `Execute` member declaration but cannot recover the dispatch logic (which belongs in `RenderCommandDispatcher`). Offer code-fix that removes the body and emits a TODO comment pointing at `RenderCommandDispatcher.Dispatch`.

**Open questions**: Should the rule scope generalize beyond `IRenderCommand` to any `[MarkerInterface]`-attributed interface? (Cascade #2 codified the principle but only applied it to one interface — generalization is a Q-K candidate.)

#### C2-Rule-2: Defensive Reserved Stub Pattern — non-empty body required for reserved stubs (Lesson #N12 sub-pattern A)

**Source**: `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md` §6.3 (Forward consideration) + §1 S-LOCK-4 (Defensive Reserved Stub Pattern first application) + §3 ε2 (Lesson #N12 candidate template)

**Statement** (verbatim from brief, §6.3):
> «Defensive Reserved Stub Pattern (Lesson #N12) потенциально codifiable as analyzer rule «interface implementations MUST не have empty/no-op bodies»»

S-LOCK-4 normative anchor:
> «ALL RenderCommandDispatcher dispatch arm methods MUST throw `NotImplementedException` с **descriptive message** linking к cascade #2 + Lesson #N12 + forward cascade #3 reference. NO empty bodies, NO no-op stubs, NO default-value returns.»

Lesson #N12 anti-patterns enumerated:
> «**Anti-patterns**:
> - Empty bodies `{ }`
> - No-op stubs `return;`
> - Default return values `return default;`
> - Silent success patterns `return Result.Success;`»

**Linkage**: Lesson #N12 (Provisional, cascade #2 first application; SEMANTIC REFINED at cascade #3 — see C3-Rule-1 for sub-pattern B); Lesson #25 refined (lying-test elimination); paired discipline.

**Analyzability tier**: **T3** — Semantic + attribute-based analysis. Requires (a) marker mechanism to identify a method as a "reserved stub" (likely via `[ReservedStub]` attribute or convention like `// CASCADE #N STUB` comment marker — convention detection is regex-bound and brittle; attribute is cleaner), (b) body-content inspection (empty block, single `return`, single default-return, `return Result.Success;`), (c) tolerance for the canonical defensive throw pattern. Conventional opt-in (attribute) brings this to T3; pure semantic "every interface implementation must throw" would be wrong (overbroad).

**Priority**: **P1** — High. Lesson #N12 is Provisional with two applications (cascade #2 + cascade #3) — promotion to FORMALIZE expected in next 1-2 cascades per K_EXT_3 §6.4. Cascade #3 SEMANTIC REFINEMENT split the rule into sub-patterns (see C3-Rule-1), which lifts complexity but raises the value of static enforcement. Not P0 because the convention is still hardening.

**Rule shape**:
- Rule ID: **DC002** (proposed; sibling to DC001; collision with DL### Lesson namespace flagged as Q-K candidate)
- Severity: **Warning** — convention enforcement at this maturity level; promote to Error when Lesson #N12 reaches FORMALIZE batch
- Detection pattern: **Semantic + attribute** — `[ReservedStub("reason")]` attribute on method → body must contain exactly one `throw new NotImplementedException(...)` statement (sub-pattern A) OR exactly one `// CASCADE #N STUB` comment + empty body marked silent (sub-pattern B; see C3-Rule-1).
- False-positive risk: **Medium** — without an attribute marker, distinguishing "intentional empty body" from "stub" requires comment-regex matching that's brittle; with an attribute the FP risk drops to Low. Recommendation: introduce `[ReservedStub]` attribute as part of A'.9 to disambiguate.
- Code-fix feasibility: **Moderate** — code-fix can transform empty body into canonical defensive throw with placeholder message; author still needs to author the descriptive message (cannot be machine-generated meaningfully).

**Open questions**: Should `[ReservedStub]` attribute be introduced in A'.9.1 cascade as prerequisite infrastructure? (Q-K candidate.) Should the rule apply only to methods on types implementing certain marker interfaces (e.g. `IDispatcher`), or to any method bearing the attribute? (Q-K candidate.)

#### C2-Rule-3: No `using Godot;` directive (post-removal regression prevention)

**Source**: `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md` §3 (Phase α §α3 verification grep) + `src/DualFrontier.Launcher/README.md` ## Rules section (per brief §3 δ3.1):

**Statement** (verbatim from brief, §α3 verification specification):
> ```powershell
> grep -rn "using Godot" src/ --include="*.cs" --exclude-dir={bin,obj,historical}
> ```

And from brief §3 δ3.1 (Launcher README template, ## Rules section):
> «- No `using Godot;` (Godot path retired per К-extensions cascade #2)
> - No `using Silk.NET;` (Silk.NET path retired — superseded by Vulkan substrate)»

**Linkage**: Q-G-2 LOCKED (Presentation.Native removal); S-LOCK-7 (Presentation.Native full removal); historical archive cross-references (β5).

**Analyzability tier**: **T1** — Trivially analyzable. `UsingDirectiveSyntax` traversal; match qualified name `Godot` (and sub-namespaces). Sibling rule for `Silk.NET`.

**Priority**: **P2** — Medium. Defensive against regression but no active threat surface (Godot+Silk.NET projects already deleted; build will fail anyway if `using Godot;` appears since assembly missing). Value is signal-clarity (clearer diagnostic than CS0246) + documentation-by-rule. Defer to post-A'.9.1 unless cheap to ship alongside DC001.

**Rule shape**:
- Rule ID: **DC003** (proposed) — could pair with DC003-Silk for Silk.NET sibling
- Severity: **Error** — banned API per cascade #2 deprecation arc; consistent with cascade #2 S-LOCK-7 absolute removal
- Detection pattern: **Syntactic** — `UsingDirectiveSyntax` with name starting `Godot` or `Silk.NET`. Filter by `historical/` path exclusion (consistent with brief §α3 grep).
- False-positive risk: **Low** — namespace prefixes are unique to the deprecated frameworks; `historical/` archive paths excluded by path filter.
- Code-fix feasibility: **Trivial** — remove the using directive. (Cannot fix follow-on compilation errors if the type was actually used, but those will surface as CS0246 with location pointing at the call site.)

**Open questions**: Is there a generic "banned namespace" rule infrastructure planned in A'.9 (would allow consolidating DC003-Godot + DC003-Silk + future bans into one rule with a config table)? (Q-K candidate.)

#### C2-Rule-4: Defensive throw message convention (Lesson #N12 first-application form)

**Source**: `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md` §1 S-LOCK-4 (canonical pattern) + §3 ε2 (Lesson #N12 candidate pattern template)

**Statement** (verbatim from brief, S-LOCK-4 canonical pattern):
> ```csharp
> private void HandlePawnSpawned(PawnSpawnedCommand cmd) =>
>     throw new NotImplementedException(
>         "PawnSpawned dispatch pending К-extensions cascade #3. " +
>         "If this throws в test, the test is exercising visual rendering path " +
>         "что cascade #2 explicitly scoped out (Lesson #N12 Defensive Reserved Stub Pattern).");
> ```

And from Lesson #N12 candidate template (ε2):
> ```csharp
> throw new NotImplementedException(
>     "Method pending [governing cascade/milestone]. " +
>     "If this throws в test, the test is exercising [scope] " +
>     "that [governing decision] explicitly scoped out " +
>     "(Lesson #N12 Defensive Reserved Stub Pattern).");
> ```

**Linkage**: Lesson #N12 (Provisional). Companion to C2-Rule-2 (body-shape rule) — this rule constrains the *message content* of the throw.

**Analyzability tier**: **T2** — Static syntax + regex on string-literal argument. Detect `throw new NotImplementedException("...")` inside a method tagged `[ReservedStub]` (or matching the convention from C2-Rule-2) → verify message contains markers: «pending», cascade designator («К-extensions cascade #N» or «post-Vanilla-mods cascade»), and «Lesson #N12».

**Priority**: **P2** — Medium. Message conventions are documentation-by-test discipline; helpful for future grep/audit work but no immediate regression risk. Cascade #3 K_EXT_3 §6.5 explicitly anticipates this rule («Defensive throw message convention enforcement (regex match для «pending [cascade/era] cascade. ... Lesson #N12»)»). Defer to A'.9.2+ unless cheap to ship.

**Rule shape**:
- Rule ID: **DC004** (proposed; depends on DC002 for marker mechanism)
- Severity: **Info** — convention enforcement; not architectural invariant
- Detection pattern: **Syntactic + regex** — `ThrowStatementSyntax` → `ObjectCreationExpressionSyntax` with `NotImplementedException` type → string literal argument → regex match `/pending .* cascade.*Lesson #N12/i` (or equivalent capture of cascade designator + lesson anchor).
- False-positive risk: **Medium** — message wording flexibility (Russian/English mix, evolving cascade designators) makes regex brittle; could be relaxed to require any one of {«pending», «deferred», «not yet»} + «Lesson #N12» (Russian/English) + cascade-or-era reference.
- Code-fix feasibility: **Complex** — message content cannot be reasonably generated; offer fix-it that inserts a template with placeholders the author must fill.

**Open questions**: Should the message convention support Russian-language constructs (cascade #2 messages mix English keywords with Russian prose)? (Q-K candidate — likely yes, since governance/briefs are bilingual.)

#### C2-Rule-5: Per-tier doc versioning convention enforcement (minor / patch / major semantics)

**Source**: `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md` §3 ε1 (Versioning convention codification addition):

**Statement** (verbatim from brief, ε1):
> «### Tier 1 versioning convention (codified К-extensions cascade #2)
>
> Для Tier 1 LOCKED documents (KERNEL_ARCHITECTURE, METHODOLOGY, VULKAN_SUBSTRATE, MOD_OS_ARCHITECTURE):
>
> | Change type | Version bump | Example |
> |---|---|---|
> | New К-L invariant / SUPERSEDE / behavioral contract change | **Minor**: 2.5 → 2.6 | А'.8 (8 К-L LOCK batch), А'.7.x (К-L15.1) |
> | Cleanup / chronicle / cross-reference / status footnote | **Patch**: 2.5 → 2.5.1 | К-extensions cascade #2 (this cascade) |
> | Structural reorganization / major architectural pivot | **Major**: 2.x → 3.0 | Hypothetical future event |»

**Linkage**: Q-G-12 LOCKED. Governance-tier rule, not source-code rule.

**Analyzability tier**: **T6** — Documentation-only. Versioning of Markdown documents is governed by REGISTER.yaml and sync_register.ps1 validation, not by a Roslyn analyzer (Roslyn scopes C# source). Possible MSBuild target enforcement is out-of-scope for A'.9 (Roslyn analyzer milestone).

**Priority**: **P3** — Low. Already enforced by `sync_register.ps1 --validate` gate in the REGISTER cascade — this candidate is a documentation acknowledgment rather than a Roslyn rule. Suggest moving to "non-analyzer governance enforcement" workstream.

**Rule shape**:
- Rule ID: **DC005** — explicit non-Roslyn marker; could become a PowerShell script rule in `tools/governance/`
- Severity: N/A (out of Roslyn scope)
- Detection pattern: N/A (out of Roslyn scope)
- False-positive risk: N/A
- Code-fix feasibility: N/A

**Open questions**: Should A'.9 acknowledge governance-tier rules that fall outside Roslyn scope but warrant separate enforcement infrastructure? (Q-K candidate.)

### §5.2 — Cascade #3 candidates (Launcher visual implementation)

#### C3-Rule-1: Lesson #N12 sub-pattern B — silent stub for production-fires path (with DO NOT TEST documentation)

**Source**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` §1 S-LOCK-4 (AMENDED 2026-05-23 mid-cascade) + §6.5 (Forward consideration: A'.9 Roslyn analyzer preparation)

**Statement** (verbatim from brief, S-LOCK-4 amended):
> «3 deferred dispatch arms get **silent stub bodies** (empty method body — accept the command, do nothing visible) с honest documentation что (a) stub exists pending post-Vanilla-mods materialization, (b) production composition fires these commands so defensive throws would crash Launcher, (c) Q-H-6 test discipline preserves: DO NOT TEST stub paths (tests would lie by passing trivially — there is no observable behavior к assert).»

And from §6.5 (A'.9 preparation):
> «Dispatch arm pattern enforcement («handler methods returning void must throw OR mutate scene state, не silent no-op»)»

Note: the §6.5 wording predates the S-LOCK-4 amendment; the amended rule is *strictly weaker* — silent no-op IS permitted when documented as a production-fires stub. Rule must encode the amended discipline (silent stub OK iff documented appropriately).

**Linkage**: Lesson #N12 SEMANTIC REFINED (cascade #3 second application + sub-pattern split per K_EXT_3 §9.4). Companion to C2-Rule-2 (sub-pattern A — test-only-fires defensive throw) and C2-Rule-4 (defensive throw message convention).

**Analyzability tier**: **T3** — Semantic + attribute + body inspection. Requires `[ReservedStub(SubPattern.ProductionFiresSilent)]` or equivalent attribute marker → method body must be empty OR contain only a `// CASCADE #N STUB` line-comment block matching the canonical pattern. Without attribute support, the rule degenerates to comment-regex matching (brittle).

**Priority**: **P1** — High. Direct consequence of cascade #3 SEMANTIC REFINEMENT; Lesson #N12 promotion to FORMALIZE expected after one more substantially-different application (per K_EXT_3 §6.3). Coupled with C2-Rule-2 — they form a paired rule set, recommended to ship together in A'.9.2.

**Rule shape**:
- Rule ID: **DC006** (paired with DC002)
- Severity: **Warning** at A'.9.1 introduction; **Error** after Lesson #N12 → FORMALIZE
- Detection pattern: **Semantic + attribute + body** — `[ReservedStub(SubPattern.ProductionFiresSilent)]` → body must be empty or contain only documented stub commentary; conversely, an *unmarked* empty body on a method overriding/implementing an interface is a separate violation (general lying-test surface rule, sibling to DC002).
- False-positive risk: **Medium** — `EmptyStatement` is legitimate in some contexts (e.g. `partial void` patterns, ABI placeholder); attribute opt-in mitigates.
- Code-fix feasibility: **Moderate** — code-fix can add the canonical `// CASCADE #N STUB ...` comment block to an empty body marked with the attribute.

**Open questions**: Should `ReservedStub` attribute carry the cascade designator + lesson reference as constructor arguments (machine-parseable) instead of relying on comment matching? (Q-K candidate — strong yes recommendation.) Should the enum `SubPattern` be defined now (TestOnlyFiresThrowing, ProductionFiresSilent) anticipating future sub-patterns? (Q-K candidate.)

#### C3-Rule-2: Dispatch arm handler discipline — void-returning handlers must throw, mutate scene state, OR be a marked silent stub

**Source**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` §6.5 (Forward consideration: A'.9 Roslyn analyzer preparation)

**Statement** (verbatim from brief):
> «Dispatch arm pattern enforcement («handler methods returning void must throw OR mutate scene state, не silent no-op»)»

Note: per the S-LOCK-4 amendment, "no silent no-op" must be relaxed to "no UNMARKED silent no-op" — see C3-Rule-1 interaction.

**Linkage**: Q-H-1 LOCKED (pawn-3 active + 3 deferred); cascade #3 RenderCommandDispatcher dispatch arm pattern (3 real impl + 3 silent stubs).

**Analyzability tier**: **T3** — Semantic + control-flow analysis. For a void-returning method bound to a dispatch arm role (likely identified by `[DispatchHandler]` attribute or by being a private method invoked from a `switch (command)` block in a class implementing `IDispatcher` or marked `[DispatcherClass]`), verify method body contains *at least one* of: (a) `throw` statement, (b) mutation of an instance field of type `SceneState` (or marked `[SceneStateContainer]`), (c) the marker attribute for silent-stub allowance per C3-Rule-1.

**Priority**: **P1** — High. Direct codification of cascade #3 dispatch pattern. Concrete and well-bounded; cascade #4+ HUD/audio dispatchers will inherit the pattern naturally.

**Rule shape**:
- Rule ID: **DC007**
- Severity: **Warning** at A'.9.1; **Error** as pattern proves out across cascade #4 application
- Detection pattern: **Semantic** — `MethodDeclarationSyntax` with void return type + enclosing type satisfying dispatcher convention → body control-flow must include throw, scene-state mutation, or silent-stub attribute.
- False-positive risk: **Medium** — depends on robust dispatcher-class identification. Without `[DispatcherClass]` attribute infrastructure, the rule must rely on naming heuristics (`*Dispatcher` class with `Dispatch(...)` method) — brittle.
- Code-fix feasibility: **Not feasible** — cannot infer author intent (real impl vs silent stub vs forgotten); offer diagnostic only.

**Open questions**: Same as C3-Rule-1 — should `[DispatcherClass]` and `[DispatchHandler]` attributes be introduced as A'.9.1 prerequisite infrastructure? (Q-K candidate.)

#### C3-Rule-3: Constructor injection — no singleton/static state in Launcher classes

**Source**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` §1 S-LOCK-10 (Scene state composition root) + §6.5 (A'.9 preparation)

**Statement** (verbatim from brief, S-LOCK-10):
> «`SceneState` instance constructed в `Program.Main()` and passed к both `RenderCommandDispatcher` (writes via handlers) and `LauncherRenderer` (reads via `EnumerateActiveSprites`). NO singletons. NO static state. NO global accessors.»
>
> «**Halt condition**: If execution agent uses singleton/static pattern for SceneState, halt + refactor required.»

And from §6.5 (A'.9 preparation):
> «Constructor injection pattern enforcement (no singleton/static в Launcher)»

**Linkage**: S-LOCK-10 LOCKED; established codebase convention (per K_EXT_3 §1 «Constructor injection per established codebase convention. Composition root pattern preserved.»). Also coheres with existing memory feedback `feedback_bus_access.md` («Systems must reach IGameServices via SystemBase.Services, never ctor injection» — note: that feedback is about *bus access* specifically, not a generalized DI rule; the cascade #3 rule is about *Launcher* class composition and is consistent rather than conflicting).

**Analyzability tier**: **T3** — Semantic analysis on assembly/namespace scope. For types in `DualFrontier.Launcher` namespace (or matching `[LauncherComponent]` if attribute introduced): forbid (a) `static` field/property declarations holding mutable state, (b) singleton pattern (private static instance field + public static accessor), (c) `Service Locator` style access (e.g. `XXX.Instance` static accessor invocation from constructor or method body).

**Priority**: **P2** — Medium. Cascade #3 enforces this discipline manually; cascade #4+ Launcher additions will need guard rails. Not P0/P1 because the current Launcher codebase is small (~5 files) and Crystalka reviews each cascade — analyzer enforcement is preventive against future expansion.

**Rule shape**:
- Rule ID: **DC008**
- Severity: **Warning** — convention enforcement; allow opt-out via `[AllowStaticState]` attribute for principled exceptions
- Detection pattern: **Semantic** — namespace filter `DualFrontier.Launcher.*` → forbid mutable static fields/properties; forbid `IXxx.Instance` static accessor patterns; forbid `static` constructors mutating shared state.
- False-positive risk: **Low** — constants (`const`) and immutable readonly-static (e.g. `static readonly Vector2 DefaultPosition`) excluded; the rule targets *mutable* shared state specifically.
- Code-fix feasibility: **Complex** — refactoring a singleton to constructor injection requires call-site rewrites across the composition root; not amenable to automated code-fix.

**Open questions**: Should the rule scope generalize to all production assemblies (`DualFrontier.*` excluding `tests/`) or stay Launcher-scoped per the brief? (Q-K candidate — generalization aligns with the existing codebase convention but exceeds cascade #3's literal scope; recommend Launcher-scoped at A'.9.1 with generalization as future expansion.) Conflict resolution with `feedback_bus_access.md` SystemBase.Services pattern needs explicit carve-out for the bus-access exception. (Q-K candidate.)

#### C3-Rule-4: Bridge Command records must be pure data (no Execute, no methods)

**Source**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` §1 S-LOCK-6 (IRenderCommand marker preservation)

**Statement** (verbatim from brief, S-LOCK-6):
> «`IRenderCommand` interface stays pure marker (Q-G-3 cascade #2 LOCKED). Cascade #3 does **NOT** restore Execute() methods on Commands. Bridge Commands stay pure data records.»
>
> «**Rationale**: Cascade #2 architectural decision preserved. Dispatch happens centrally в Launcher's `RenderCommandDispatcher`. Adding Execute() back would re-introduce lying-test surface (Lesson #25 refined violation).»

**Linkage**: Companion to C2-Rule-1 (IRenderCommand marker-only enforcement). C2-Rule-1 enforces "no Execute method"; this rule strengthens to "no methods of any kind" (records remain pure data carriers).

**Analyzability tier**: **T2** — Static syntax-tree analysis. For any record type implementing `IRenderCommand`: forbid declared methods (other than compiler-generated record members `Equals`, `GetHashCode`, `ToString`, `Deconstruct`, copy ctor).

**Priority**: **P1** — High. Promotes C2-Rule-1 from "no Execute" to the stronger "no methods at all" — eliminates the entire lying-test class, not just the one method shape. Should ship alongside C2-Rule-1 in A'.9.1 since they're a coordinated pair.

**Rule shape**:
- Rule ID: **DC009** (consolidate with DC001 into single rule with two sub-checks, or keep separate as graduated severity)
- Severity: **Error** — same as DC001
- Detection pattern: **Semantic** — `RecordDeclarationSyntax` (or `ClassDeclarationSyntax`) implementing `IRenderCommand` → `GetMembers()` filtered to exclude compiler-generated record members → must be empty.
- False-positive risk: **Low** — records with extra members are rare in this codebase; whitelist compiler-generated members.
- Code-fix feasibility: **Moderate** — remove non-data members; cannot recover dispatch logic.

**Open questions**: Should DC001 and DC009 be merged into a single rule "IRenderCommand implementations are pure data records"? (Q-K candidate — recommend yes for analyzer ergonomics; one rule ID is easier to reason about than two paired rules.)

#### C3-Rule-5: RenderCommandDispatcher.Dispatch signature stability (handler signature contract)

**Source**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` §1 S-LOCK-8 (RenderCommandDispatcher signature stability)

**Statement** (verbatim from brief, S-LOCK-8):
> «`RenderCommandDispatcher.Dispatch(IRenderCommand)` public method signature **stays unchanged** от cascade #2. Internal handler method signatures unchanged. ONLY method **bodies** of pawn-3 arms change от defensive throws к real implementations.»
>
> «**Halt condition**: If execution agent modifies `Dispatch` signature OR handler method signatures (parameter additions, return type changes), halt + brief amendment required.»

**Linkage**: Q-G-3 cascade #2 + S-LOCK-8 cascade #3. К-L14 substrate-adjacent consumer stability evidence.

**Analyzability tier**: **T5** — Runtime-only / cross-version. Signature stability is a *between-versions* property; a single point-in-time analyzer cannot detect "this signature changed compared to last cascade's signature". This requires a baseline (e.g. PublicApiAnalyzers-style `*.Shipped.txt` + `*.Unshipped.txt`) or a custom API-surface diffing tool. Not feasible as a stateless Roslyn rule.

**Priority**: **P2** — Medium. Cascade-to-cascade signature stability is governance discipline (brief halt condition + Crystalka review). Could be promoted to API-surface tooling (e.g. `Microsoft.CodeAnalysis.PublicApiAnalyzers`) which is adjacent to but distinct from custom A'.9 rules.

**Rule shape**:
- Rule ID: **DC010** (proposed — but better implemented via `PublicApiAnalyzers` baseline file convention)
- Severity: **Error** when API baseline breaks
- Detection pattern: **API-baseline diff** — out of scope for plain Roslyn rules; recommend adopting `Microsoft.CodeAnalysis.PublicApiAnalyzers` for the `DualFrontier.Launcher` and `DualFrontier.Application.Bridge` assemblies and including a baseline file in REGISTER governance.
- False-positive risk: N/A (baseline tooling handles this)
- Code-fix feasibility: N/A (would need to roll back the signature change manually)

**Open questions**: Should A'.9.1 introduce `PublicApiAnalyzers` package as part of the analyzer milestone infrastructure? (Q-K candidate — strong recommendation, since cascade signature-stability is a recurring K_EXT_N concern.)

### §5.3 — Cross-cascade observation

Three meta-patterns emerge across cascade #2 and cascade #3 candidates that warrant analyzer infrastructure design consideration for A'.9.1:

**1. Lesson #N12 underlies 4 candidate rules (C2-Rule-2, C2-Rule-4, C3-Rule-1, C3-Rule-2).** All four require some form of marker (attribute or comment convention) to distinguish "this method is a reserved stub" from "this method is a normal implementation". The brittleness of comment-regex matching argues strongly for introducing a `[ReservedStub(SubPattern, Reason)]` attribute in `DualFrontier.Application.Attributes` (or a new `DualFrontier.Conventions` assembly) **as analyzer infrastructure prerequisite for A'.9.1**, before any of these four rules ships. Without the attribute, all four rules drop from T2/T3 to T5 (regex-bound) with Medium-to-High false-positive rates. Per K_EXT_3 §6.4 «Promotion к FORMALIZE expected within next 2-3 cascades» — analyzer ergonomics warrant getting this right at A'.9.1 introduction.

**2. Marker-interface pattern recurrence.** C2-Rule-1 (IRenderCommand marker-only) and C3-Rule-4 (Bridge Commands pure data records) both enforce the same architectural principle: «marker interfaces have zero members AND their implementations are pure data carriers, with behavior centralized elsewhere». Cascade #4+ may surface additional marker interfaces (HUD primitives reserved skeleton per K_EXT_3 §6.3 — likely `IHudCommand`, `IAudioCommand`, etc.). Recommend designing the analyzer as a **`[MarkerInterface]`-attributed pattern** rather than baking `IRenderCommand` into rule logic — generalization cost is low (~1 line of attribute check) and pays off the first time a second marker interface lands.

**3. Bilingual message convention.** Cascade #2 and cascade #3 briefs and defensive-throw messages mix Russian and English freely (e.g. «PawnSpawned dispatch pending К-extensions cascade #3» — English noun + Russian preposition + mixed-case cascade designator). Any rule with text-content enforcement (C2-Rule-4 defensive throw message convention, C3-Rule-1 silent stub commentary) must accept bilingual patterns from the outset. Single-language regex patterns will produce false-positives on otherwise-correct code. Recommend baking bilingual tolerance into a shared helper (`ConventionMessageMatcher` or similar) at A'.9.1 infrastructure layer.

**Net infrastructure recommendation for A'.9.1**: introduce three pieces of analyzer-prerequisite infrastructure before any DC### rule ships:
- `[ReservedStub(SubPattern, Reason)]` attribute (used by DC002, DC004, DC006, DC007)
- `[MarkerInterface]` attribute (used by DC001, DC009)
- `ConventionMessageMatcher` shared helper for bilingual text matching (used by DC004, DC006)

This batched infrastructure work fits naturally as A'.9.1 Phase α (analyzer foundation) before any DC### rule lands in Phase β.

### §5.99 — Open questions surfaced (for §11 Q-K candidates)

- **Q-K-§5-1 (rule namespace allocation)**: Should cascade-derived rules use a fresh **DC###** prefix (DualFrontier Cascade), share the **DF###** prefix with К-L rules, or use **DL###** (Lesson-derived)?
  - Context: §5 candidates derive from architectural decisions ratified in cascade Q-N/Q-G/Q-H, not from К-L invariants or Lessons directly — yet they touch both substrates (Lesson #N12 underlies four candidates). Namespace collision risk if DF### is reused (cascade #2 K_EXT_2 §6.3 mentions DF015.1 already LOCKED for К-L15.1).
  - Options:
    - (a) DC### fresh namespace (proposed in §5 candidates)
    - (b) DF### shared (cascade-derived rules treated as К-L-adjacent)
    - (c) DL### shared (cascade-derived rules treated as Lesson-derived, given Lesson #N12 underlies most)
    - (d) Tiered namespace (DFK### for К-L, DFL### for Lessons, DFC### for cascade — explicit prefix discipline)
  - Recommendation: **(d) Tiered namespace** with DFC### for cascade-derived rules; explicit prefix discipline matches the existing governance taxonomy (К-L vs Lessons vs cascades are separate concepts in METHODOLOGY).

- **Q-K-§5-2 (analyzer infrastructure attributes)**: Should A'.9.1 introduce `[ReservedStub(SubPattern, Reason)]` + `[MarkerInterface]` + (optionally) `[DispatcherClass]` / `[DispatchHandler]` / `[LauncherComponent]` attributes as Phase α infrastructure before any DC### rule lands?
  - Context: 4 of 10 §5 candidates degrade from T2/T3 to T5 (regex-bound, brittle) without attribute markers. Per K_EXT_3 §6.4, Lesson #N12 promotion to FORMALIZE expected within 2-3 cascades — ergonomics matter.
  - Options:
    - (a) Introduce all 4-5 attributes in A'.9.1 Phase α (~50-100 LOC, single commit)
    - (b) Introduce minimal `[ReservedStub]` only, defer others
    - (c) Defer all attribute infrastructure to A'.9.2 — rules ship as Info-severity regex-bound until then
  - Recommendation: **(a)** — batched infrastructure work is cheap and unlocks higher-priority rules cleanly.

- **Q-K-§5-3 (rule pairing / consolidation)**: Should DC001 (IRenderCommand no Execute) and DC009 (Bridge Commands pure data records) merge into a single rule with two sub-checks?
  - Context: DC009 is a strict superset of DC001 (no methods at all ⇒ no Execute method); shipping as one rule simplifies analyzer mental model.
  - Options:
    - (a) Merge into single DFC001 with two sub-checks
    - (b) Keep separate (allows graduated severity — Error for Execute method, Warning for other methods)
    - (c) Drop DC001 entirely, ship only DC009
  - Recommendation: **(a)** merge; simplicity wins, and the cascade #2 + #3 rationale for both rules is identical (lying-test surface elimination).

- **Q-K-§5-4 (banned-namespace consolidation)**: Should C2-Rule-3 (no `using Godot;`/`using Silk.NET;`) be implemented as a generic "banned namespaces" rule with a config table, or as two separate hard-coded rules?
  - Context: Cascade-driven deprecation (Godot, Silk.NET in cascade #2; potentially more in future cascades) suggests this is a recurring pattern.
  - Options:
    - (a) Single configurable rule `DFC003 BannedNamespace` reading from `.editorconfig` or analyzer config
    - (b) Hard-coded sibling rules `DFC003-Godot`, `DFC003-Silk`
    - (c) Use existing `BannedApiAnalyzer` package
  - Recommendation: **(c)** — `BannedApiAnalyzer` exists, is well-tested, and integrates naturally with `.editorconfig`-driven configuration; reinventing this is unnecessary substrate work.

- **Q-K-§5-5 (DC008 scope vs `feedback_bus_access.md`)**: Should DC008 (no singleton/static in Launcher) carve out an exception for `SystemBase.Services` bus access (per memory `feedback_bus_access.md`)?
  - Context: Memory note explicitly states «Systems must reach IGameServices via SystemBase.Services, never ctor injection» — that's a *bus access* exception that lives in a different assembly (`DualFrontier.Core` systems), not Launcher. So no direct collision, but if DC008 generalizes beyond Launcher (Q-K candidate above), the conflict surfaces.
  - Options:
    - (a) DC008 stays Launcher-scoped (no conflict with bus access pattern)
    - (b) DC008 generalizes to all `DualFrontier.*` assemblies with explicit `SystemBase.Services` carve-out
    - (c) Defer generalization to A'.9.2+ and only ship Launcher-scoped version at A'.9.1
  - Recommendation: **(a) or (c)** — Launcher-scope only at A'.9.1; revisit generalization after observing analyzer ergonomics.

- **Q-K-§5-6 (signature-stability tooling)**: Should A'.9.1 adopt `Microsoft.CodeAnalysis.PublicApiAnalyzers` to address C3-Rule-5 (RenderCommandDispatcher signature stability) and similar cross-cascade signature-stability concerns?
  - Context: Cascade halt conditions on signature changes (S-LOCK-8 + future S-LOCKs) are governance discipline; tooling can make these enforcement-by-build rather than enforcement-by-review.
  - Options:
    - (a) Adopt PublicApiAnalyzers for `DualFrontier.Launcher` + `DualFrontier.Application.Bridge` + (eventually) `DualFrontier.Application.Rendering` assemblies at A'.9.1
    - (b) Defer to a separate post-A'.9 tooling cascade
    - (c) Don't adopt — rely on cascade brief Halt conditions + Crystalka review
  - Recommendation: **(a)** — strong recommendation; PublicApiAnalyzers is a well-known pattern, integrates with `.editorconfig`, and directly addresses a recurring K_EXT_N concern.

- **Q-K-§5-7 (governance-tier rule scope)**: Should A'.9 acknowledge that governance-tier rules (like C2-Rule-5 versioning convention) fall outside Roslyn scope and warrant a separate enforcement infrastructure workstream?
  - Context: C2-Rule-5 is T6 (documentation-only as a Roslyn rule) but enforced today by `sync_register.ps1 --validate` gate. Future analogous rules (e.g. brief naming convention, deliberation Q-N numbering convention) will accumulate.
  - Options:
    - (a) Out of scope for A'.9 — document acknowledgment in A'.9 brief §11 and defer to post-A'.9 governance-tooling cascade
    - (b) Expand A'.9 to include PowerShell governance script rules
    - (c) Treat as out-of-scope without acknowledgment
  - Recommendation: **(a)** — explicit acknowledgment + deferral preserves A'.9's focus on Roslyn analyzer ergonomics while flagging the parallel workstream for future cascade authoring.

---

## §6 — Mod OS К-L20 prep surface

### §6.0 — Reconnaissance methodology

**Source documents read (Domain 4, Sub-Agent C1, 2026-05-24)**:

- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.11 LOCKED — full read (1241 lines): manifest v2 schema (§2), capability model (§3 incl. К10.2 tier extensions + К10.3 v2 К-L17 layer extensions), IModApi v3 strict surface (§4 incl. §4.6 / §4.6.1 / §4.6.3 v3-strict backward-incompat statement), type-sharing protocol (§5), three-level contracts (§6), bridge replacement (§7), three-axis versioning (§8 incl. caret-subset §8.4 LOCKED), lifecycle (§9 incl. К10.2/К10.3 v2 amendments), threat model (§10 caught/uncaught threats), migration plan (§11 incl. M3.4 + M3.5 deferred analyzer milestones), seven locked decisions (§12 D-1..D-7), and «Modding с native ECS kernel» trailer.
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.5.2 LOCKED Part 0 — К-L9 (line 58) + К-L20 row absence in Part 0 table (table ends at К-L19, line 68; К-L20 lives in K_CLOSURE §2.23 per Q-N-8-1 reservation discipline) confirmed. Implication of K-L9 (line 84) read («No vanilla privilege»).
- `docs/architecture/K_CLOSURE_REPORT.md` AUTHORED Tier 1 — §2.11 К-L9 entry (lines 423-445), §2.23 К-L20 entry (lines 774-795), §7.3 reserved rules table DF020 row (line 1689), §9.5 Mod API lock milestone Q1-Q8 surface (lines 1908-1929), §12 KERNEL Part 0 К-L20 placeholder note (line 2097).
- `docs/architecture/ANALYZER_RULES.md` v0.1 AUTHORED-SKELETON — §1 18-active + 4-reserved enumeration (lines 26-53), §3 forward implementation plan including «DF020 post-Mod API lock» bullet block (lines 111-114), §4 reserved-rules table DF020 row (line 155).
- `docs/architecture/MODDING.md` v1.1 LOCKED — IModApi v3 strict surface verbatim (lines 47-86), Allowed/Not-Allowed table (lines 91-104), AssemblyLoadContext block list (lines 106-114), manifest schema example (lines 156-188).
- `docs/architecture/MOD_PIPELINE.md` (spot reads) — RestrictedModApi v3 strict declaration (line 122-123), Phase A ContractsVersion gate (line 61), validation error enum entries (line 82).
- `docs/methodology/METHODOLOGY.md` — Provisional pool §16.X bullet block (line 984: «Lesson #N3 — К-L9 mod-facing boundary Contracts/Application (carried)»); full Lesson #N3 detailed narrative resides in v1.7+ post-А'.0 Methodology body (not extracted in this reconnaissance because Provisional bullet text is the source-of-truth handle for tracking, full narrative lives in METHODOLOGY body and is bookmarked as «carried Provisional» per Q-N-8-6 forward sequencing).
- `tools/briefs/MOD_OS_V16_AMENDMENT_BRIEF.md` + `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md` — historical context for v1.6 GPU compute integration amendment (Phase 1-7 execution; 11-commit cascade `260103b..d5dcdde`); informs §6.2.4 grace-period evolution baseline.
- `tools/DualFrontier.Mod.ManifestRewriter/ManifestRewriter.cs` (127 lines) — `hotReload: true → false` build-time rewriter per D-7 LOCKED. Source preservation contract + idempotency contract documented in XML doc comments. Establishes precedent for a future `DualFrontier.Mod.ManifestAnalyzer` (separate Roslyn analyzer project) following the same project-structure pattern (one-purpose tool, deterministic, library + thin CLI wrapper).
- `mods/DualFrontier.Mod.Example/ExampleMod.cs` + `mod.manifest.json` — current Mod API surface usage pattern: minimal IMod implementation (Initialize-stub + Unload-stub); manifest `manifestVersion: "3"` (strict per K8.3+K8.4 cutover), no `capabilities`/`kind`/`apiVersion`/`replaces` fields (uses §2.2 defaults).
- `mods/DualFrontier.Mod.Vanilla.{Pawn,Combat,Magic,Inventory,Core,World}/*.cs` + `mod.manifest.json` — vanilla mod skeleton state; all five vanilla regular mods + Vanilla.Core shared mod register-empty per M8 skeleton; manifests carry full v3 surface including `capabilities: { required: [], provided: [] }` (registration content lands М9/M10 per §11.1 migration plan).

**Cross-references к other Domain reports**:

- Domain 1 (Agent A1) К-L20 row in §3.1 matrix (line 121): «**DEEP ANALYSIS DEFERRED TO DOMAIN 4 (Agent C1)** per brief discipline». This §6 is the deferred analysis target.
- Domain 1 К-L9 deep analysis in §3.2 (lines 375-396): DF009 «revisit post-Mod API lock» — DF009/DF020 forward-design relationship motivates §6.3.1 precursor table below.
- Domain 1 К-L3.1 deep analysis (lines 201-223): ALC isolation enforcement model informs §6.2.1 namespace/type restriction surface (cross-mod managed-path access is structurally impossible per К-L3.1; this is the load-bearing primitive on which §6.2.1 restrictions rest).

**Honest deferral statement**: К-L20 is RESERVED post-Mod API lock per K_CLOSURE §7.3 + §9.5. Canonical text is TBD at Mod API lock deliberation (Q1-Q8 surface enumerated K_CLOSURE §9.5, lines 1910-1918). This §6 enumerates analyzer-enforceable restrictions **derivable from current MOD_OS v1.11 LOCKED surface + KERNEL Part 0 implications**; restrictions whose canonical formulation depends on Mod API lock deliberation outcome (Bridge mechanism design, grace period semantics evolution post-v1/v2-grace-sunset, manifest schema v3 freeze policy specifics) are flagged «pending Mod API lock canonical text».

---

### §6.1 — К-L20 statement (from KERNEL Part 0 + K_CLOSURE §2.23)

**KERNEL Part 0 К-L20 row** (verbatim from `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 table, lines 48-69):

> [К-L20 row is **NOT present** in the Part 0 table. Part 0 table runs К-L1..К-L19 (terminating at line 68). The К-L20 row in KERNEL Part 0 is the «RESERVED placeholder» referenced by `K_CLOSURE_REPORT.md` §12 line 2097: «К-L20: KERNEL_ARCHITECTURE.md Part 0 К-L20 row (RESERVED placeholder; populated at Mod API lock)». KERNEL v2.5.2 prose header line 17 confirms «К-L20 reserved post-Mod API lock excluded from main count» and «cumulative К-Lxx series total post-А'.7.x: 21 invariants» — К-L20 is the 22nd invariant reserved as such.]

**K_CLOSURE §2.23 entry** (verbatim from `docs/architecture/K_CLOSURE_REPORT.md` §2.23, lines 774-795):

> **§2.23 — К-L20: Mod API forward-compatibility [RESERVED]**
>
> **Status**: RESERVED (target: Mod API lock milestone post-А'.9)
> **LOCK history**: Pre-AUTHORED. Text TBD at Mod API lock deliberation.
> **Canonical text**: [TBD at Mod API lock milestone — К-L20 represents architectural commitment future-cascade for mod ecosystem stability через grace period mechanism]
>
> **Reservation rationale**:
> - Mod API lock is downstream milestone post-A'.9 (per Q-N-8-6 LOCKED forward sequencing)
> - К-L20 text requires Q-N deliberation at Mod API lock milestone (Q1-Q8 surfaces: Bridge mechanism, manifest freeze policy, grace period semantics, deprecation cadence, etc.)
> - Pre-AUTHORED state preserved here as architectural placeholder + commitment
>
> **Falsifiability commitment**: TBD at Mod API lock deliberation. Expected criteria: Mod API v3 + manifest grace period semantics; mod authoring against locked API stays compatible; Bridge mechanism for forward-compatible amendments.
>
> **Production manifestation post-LOCK** (planned):
> - `src/DualFrontier.Modding/IModApi.cs` — version-frozen surface
> - `src/DualFrontier.Modding/ModManifest.cs` — manifest schema v3 frozen
> - `src/DualFrontier.Modding/ModApiBridge.cs` — Bridge mechanism implementation
> - `docs/architecture/MOD_API_CONTRACT.md` — Tier 1 LOCKED authored at Mod API lock milestone
> - `docs/architecture/MOD_AUTHORING_GUIDE.md` — Tier 2 authored at Mod API lock milestone
> - DF### rule: **DF020** RESERVED (reserved post-Mod API lock; will activate at Mod API lock milestone landing)
>
> **К-extensions tracking**: К-L20 forward к Mod API lock milestone. Phase B M-cycle migration verifies К-L20 + К-L9 «Vanilla = mods» purity combined. Mod API lock cascade is hard gate before Phase B (per Q-N-8-6 LOCKED forward sequencing).

**Current status** (2026-05-24, A'.9.0 reconnaissance): **RESERVED post-Mod API lock**. Canonical К-L20 text TBD at Mod API lock deliberation. DF020 analyzer rule reserved per ANALYZER_RULES.md §1.1 (line 53) and K_CLOSURE §7.3 (line 1689). К-L20 LOCK + DF020 activation will land in Mod API lock cascade post-A'.9 per Q-N-8-6 LOCKED forward sequencing.

**Source citations consolidated**:

- KERNEL_ARCHITECTURE.md Part 0 К-L20 row: absent from main table (lines 48-69 cover К-L1..К-L19); placeholder commitment in header prose (line 17) + summary row in K_CLOSURE_REPORT §2.24 table (line 825).
- K_CLOSURE_REPORT.md §2.23 (lines 774-795): canonical narrative + reservation rationale + falsifiability commitment + production manifestation plan.
- K_CLOSURE_REPORT.md §7.3 (line 1689): DF020 reserved-rules table row — «Expected detection scope: Mod API surface deviation от locked v3 manifest, Bridge mechanism bypass attempt, manifest grace period semantics violation».
- K_CLOSURE_REPORT.md §9.5 (lines 1908-1929): Mod API lock milestone Q1-Q8 deliberation surface — Q1 Bridge mechanism design, Q2 manifest freeze policy, Q3 grace period semantics, Q4 deprecation cadence, Q5 К-L9 «Vanilla = mods» purity verification, Q6 К-L20 codification, Q7 DF020 scope, Q8 MOD_API_CONTRACT.md authoring scope.
- ANALYZER_RULES.md §1.1 + §3 + §4 reserved rules (lines 53, 111-114, 155): DF020 reserved entry consistent across analyzer-rules document.
- MOD_OS_ARCHITECTURE.md §1 LOCK (line 60) confirms IModApi v3 strict cutover: «v2 IModApi deleted entirely — no backward compatibility».

---

### §6.2 — Mod API restrictions analyzer-enforceable

This sub-section enumerates analyzer-enforceable Mod API surface restrictions derivable from the current MOD_OS v1.11 LOCKED surface. Restrictions are organized into four sub-sub-sections per the brief scope. Each restriction carries: source citation (MOD_OS section), analyzability tier (T1-T6 per S-LOCK-4 rubric inherited from Domain 1), priority (P0-P3), rule shape proposal (existing rule extension / new sub-rule / DF020.X), and notes.

**Tier rubric reminder** (per S-LOCK-4 / Domain 1 §3.0):
- T1: Trivially analyzable via simple pattern match
- T2: Static analysis (symbol shape + attribute presence + reference assemblies)
- T3: Semantic analysis (assembly identity, namespace scoping, module boundary)
- T4: Hybrid (static + runtime fence / probe)
- T5: Runtime-primary (hardware / timing / nondeterminism)
- T6: Not analyzer-scoped (process invariant / meta-invariant)

**Priority rubric reminder** (per S-LOCK-4):
- P0: Foundational; ship within A'.9 analyzer infrastructure
- P1: Important; ship within A'.9 or Mod API lock cascade
- P2: Useful; ship at Mod API lock cascade or post-lock
- P3: Deferred / reserved indefinitely

#### §6.2.1 — Namespace / type restrictions (what Mod assemblies may NOT reference)

| Restriction | Source | Tier | Priority | Rule shape | Notes |
|---|---|---|---|---|---|
| Mod assemblies must NOT P/Invoke directly to native — must use IModApi facade (К-L9 + К-L12 + К-L15 facade mandate; К-L2 single-DLL preserved) | MOD_OS_ARCHITECTURE.md §4.4 (RestrictedModApi cast prevention D-3 LOCKED) + §10.1 (caught threats table); KERNEL Part 0 К-L9 line 58 + К-L12 line 61 + К-L15 line 64 | T3 | P0 | **DF020.1 / extends DF009 + DF012 + DF015** | Static: detect `[DllImport]` declarations in assemblies whose root namespace matches mod-namespace pattern (`Vanilla.*` or third-party `mod.*` per shared-mod naming §5.5). DF009 currently scopes vanilla-namespace direct-kernel-reference; DF020.1 generalizes к ALL mod assemblies (including non-vanilla regular and shared mods). |
| Mod regular-ALC assemblies must NOT reference `DualFrontier.Core.*` / `DualFrontier.Application.*` / `DualFrontier.Systems.*` / `DualFrontier.Components.*` / `DualFrontier.Events.*` — only `DualFrontier.Contracts.*` permitted | MOD_OS_ARCHITECTURE.md §5.4 «Restrictions on shared mods» (line 666-669) extended к regular mods via §5.3 loader rules; MODDING.md AssemblyLoadContext block list (lines 106-114) | T2 | P0 | **DF020.2 / new** | Static: reference-assembly scan for forbidden production assembly references in mod project (.csproj `<ProjectReference>` / `<Reference>` whitelist). Currently enforced at runtime by `ModLoadContext.Load` returning null for non-Contracts assemblies (MODDING.md line 111-112: «refused»); analyzer adds compile-time signal. Code-fix feasibility: **High** — suggest remove reference + replace with IModApi-mediated equivalent. |
| Mod regular-ALC assemblies must NOT reach into another mod's regular ALC via reflection (`Type.GetType("Vanilla.Pawn.JobQueueComponent", "Vanilla.Pawn.dll")` cross-mod load) | MOD_OS_ARCHITECTURE.md §1.4 «invariants» (lines 185-190): «A regular mod's ALC may resolve types from the shared ALC, but never from another regular ALC»; §10.2 «not caught» line 1034 acknowledges reflection-based bypass as out-of-scope for runtime sandbox, but static analyzer CAN catch the documented anti-pattern at compile time | T3 | P1 | **DF020.3 / new** | Semantic: detect `Type.GetType(string)` / `Assembly.Load(string)` call sites within mod assembly where string literal argument matches another mod's identity (FQN starts with `Vanilla.` other than self-mod, or `DualFrontier.Mod.*` cross-mod pattern). Conservative: emit Warning (not Error) because reflection patterns admit false positives (dynamic plugin discovery via own-assembly types is legitimate). |
| Mod assemblies must NOT subclass kernel-sealed types (`RestrictedModApi`, `ModLoadContext`, `SystemExecutionContext`, etc.); cast `IModApi → RestrictedModApi` forbidden per D-3 LOCKED | MOD_OS_ARCHITECTURE.md §4.4 (lines 459-463); MODDING.md «not allowed» table (lines 101-104) | T2 | P0 | **DF020.4 / new** | Static: pattern detect `(RestrictedModApi)api` / `api is RestrictedModApi` / inheritance `: RestrictedModApi`. Per D-3 LOCKED, structural barrier is the primary defense (`internal sealed` + ALC isolation); analyzer is defensive secondary per «(a) is held in reserve for v1.x if a real bypass attempt is observed». Pending Mod API lock canonical text: whether D-3 LOCKED defers analyzer rule indefinitely or activates as DF020.4 at Mod API lock cascade. |
| Shared mod assemblies must NOT export types implementing `IModContract` or `IEvent` from a non-shared assembly — those types must live in the shared ALC (D-4 LOCKED active scan) | MOD_OS_ARCHITECTURE.md §6.5 «Anti-pattern: type in regular mod» + D-4 LOCKED (§12, lines 1167-1179): «The loader actively scans every regular-mod assembly via reflection for types implementing `IModContract` or `IEvent`. Detection rejects the mod with `ValidationErrorKind.ContractTypeInRegularMod`» | T2 | P0 | **DF020.5 / extends D-4 LOCKED loader scan to compile-time** | Static: scan mod assembly for types implementing `IModContract` / `IEvent`; if assembly manifest declares `kind: "regular"`, emit Error. Currently load-time enforcement (D-4 LOCKED, §6.5); analyzer adds compile-time signal so author catches mistake before publication. Pre-existing precedent: M4 added `ContractTypeInRegularMod` validation error (§11.2). |

**Notes on §6.2.1 surface**:
- К-L20 + К-L9 are intertwined: К-L9 mandates «Vanilla = mods» (one IModApi surface for vanilla and third-party); К-L20 will lock that surface (no covert extensions for vanilla privileged access). DF020.1 explicitly extends DF009's vanilla-only scope to ALL mods.
- Restrictions DF020.2 and DF020.5 mirror existing load-time enforcement (`ModLoadContext.Load` refusal + D-4 active scan); analyzer adds compile-time signal per author-experience improvement (consistent with M3.4 «CI Roslyn analyzer for `[ModCapabilities]` honesty» pattern from MOD_OS §11.1).

#### §6.2.2 — API usage restrictions (what Mod assemblies may NOT call)

| Restriction | Source | Tier | Priority | Rule shape | Notes |
|---|---|---|---|---|---|
| Reflection-based bypass of IModApi facade (e.g., direct `Services.X.Publish<T>` invocation outside `IModApi.Publish<T>` proxy) | MOD_OS_ARCHITECTURE.md §3.6 hybrid enforcement runtime layer (lines 372-376): «Reflection-based bypass of `[ModCapabilities]` declarations (deliberate violation rather than accident)»; §10.2 «not caught» (line 1034) acknowledges determined reflection bypass as out-of-runtime-scope | T4 | P1 | **DF020.6 / new (hybrid)** | Static: detect reflection patterns (`MethodInfo.Invoke` / `Activator.CreateInstance` / `Expression.Call`) targeting `DualFrontier.Application.*` member names from within mod assembly. Runtime aspect: `RestrictedModApi.EnforceCapability` already runtime-checks (per §3.6 hot path lookup); analyzer extends compile-time visibility. Conservative severity Warning per false-positive risk (legitimate reflection on mod-internal types). |
| Bridge mechanism bypass attempt — mod calls into Bridge surface other than through IModApi-exposed `replaces` declaration | K_CLOSURE §7.3 DF020 expected scope (line 1689): «Bridge mechanism bypass attempt» | T3 | P1 | **DF020.7 / new (pending Mod API lock canonical text)** | **Pending Mod API lock canonical text** — Bridge mechanism design itself is Q1 of Mod API lock deliberation (K_CLOSURE §9.5 line 1911). Current MOD_OS §7 documents `[BridgeImplementation(Replaceable = true)]` + manifest `replaces` field — analyzer scope will likely include detecting code paths that invoke `[BridgeImplementation]` systems directly (bypassing scheduler dispatch) or that bypass the `replaces` declaration to silently shadow a kernel system. |
| Native scheduler direct access by mods (К-L12 prohibition specifically applied к mods) | KERNEL Part 0 К-L12 line 61; K_CLOSURE §7.2 DF012 entry (line 1673): «К-L9 facade bypass (mods accessing native scheduler directly bypassing managed facade)» | T3 | P0 | **DF020.8 / extends DF012** | Static: namespace-scoped analyzer — assembly identity check at `df_scheduler_*` P/Invoke call sites; mods (any assembly outside `DualFrontier.Application.*` + `DualFrontier.Core.*`) must NOT carry these P/Invoke declarations. Currently DF012 (K_CLOSURE §7.2) detects «К-L9 facade bypass»; DF020.8 sharpens к Mod-API-lock semantic (vanilla mod has identical restriction). |
| Native bus direct access by mods (К-L15 facade bypass specifically applied к mods) | KERNEL Part 0 К-L15 line 64; K_CLOSURE §7.2 DF015 entry (line 1675): «capability declaration bypass (mod publish/subscribe without per-FQN per-tier capability declaration)» | T3 | P0 | **DF020.9 / extends DF015** | Static: assembly identity check at `df_bus_*` P/Invoke call sites; mods must NOT carry these declarations. Currently DF015 detects managed-side sovereign routing; DF020.9 sharpens к Mod-API-lock semantic. К-L15.1 three-tier independence preserved (DF015.1 separate concern). |
| Mod uses `IModApi.Publish` / `Subscribe` for an event type whose `[EventTier]` attribute names a tier the mod's manifest doesn't declare capability for (К-L15 + К-L17 per-FQN per-tier capability mandate) | MOD_OS_ARCHITECTURE.md §3.2 tier verb semantics (lines 311-318) + §3.8 D-2 LOCKED hybrid enforcement (lines 396-399); К10.2 `BusTierMismatch` validation error per §11.2 (line 1102) | T2 | P0 | **DF020.10 / extends D-2 LOCKED CI static analysis** | Static: per-system `[ModCapabilities]` attribute cross-referenced with `Services.X.Publish<T>` / `Subscribe<T>` call site event type's `[EventTier]` attribute. D-2 LOCKED Hybrid already promises CI static analysis pass; DF020.10 lands that pass at Mod API lock cascade (after A'.9 analyzer infrastructure is in place). |

**Notes on §6.2.2 surface**:
- DF020.6 (reflection bypass) is a fundamental analyzer scope limitation: a determined mod can always defeat static analysis via dynamic code generation. К-L20 canonical text should acknowledge this limit explicitly — the analyzer guarantees catching ACCIDENTAL violations, not DELIBERATE ones (consistent with §10.3 «best-effort structural isolation»).
- DF020.7 (Bridge mechanism) is the largest pending-Mod-API-lock-canonical-text item — Bridge mechanism Q1 deliberation outcome will determine whether DF020.7 is single rule or sub-family DF020.7.{a,b,c}.

#### §6.2.3 — Manifest field enforcement (static cross-check between code and mod.json)

| Restriction | Source | Tier | Priority | Rule shape | Notes |
|---|---|---|---|---|---|
| Mod uses a Bus tier without declaring capability in manifest (e.g., code calls `api.Publish<FastTierEvent>` but manifest lacks `kernel.fast.publish:FastTierEvent`) | MOD_OS_ARCHITECTURE.md §3.2 К10.2 tier verbs (lines 311-318); §11.2 `BusTierMismatch` (line 1102) | T2 | P0 | **DF020.11 / extends D-2 LOCKED + extends DF015** | Static: cross-check manifest `capabilities.required` against `[EventTier]` attributes of event types in `Services.X.Publish<T>` / `Subscribe<T>` call sites within mod assembly. Read manifest JSON as analyzer additional file. **Manifest analyzer infrastructure precedent**: `tools/DualFrontier.Mod.ManifestRewriter/` (build-time JSON tool) demonstrates idempotent manifest manipulation pattern — a `DualFrontier.Mod.ManifestAnalyzer` Roslyn project would follow same one-purpose-tool structure. |
| Mod assembly targets Mod API version other than `"3"` (manifest `manifestVersion` field) | MOD_OS_ARCHITECTURE.md §4.6.3 strict v3 LOCK (lines 586-590); MODDING.md mandatory `"manifestVersion": "3"` (lines 158, 181) | T1 | P0 | **DF020.12 / new** | Trivial: read manifest as analyzer additional file; check `manifestVersion == "3"` literal. Per K8.3+K8.4 cutover (2026-05-14): «the manifest parser rejects any `manifestVersion` other than `"3"`». Analyzer adds compile-time signal. Code-fix feasibility: **Trivial** — suggest `"manifestVersion": "3"`. |
| Mod replaces vanilla mod without manifest `replaces` declaration (mod registers a system whose type matches an existing `[BridgeImplementation(Replaceable = true)]` system without declaring the FQN in manifest `replaces` field) | MOD_OS_ARCHITECTURE.md §7.1 explicit `replaces` LOCKED (lines 766-783); §7.4 `Replaceable` flag (lines 794-803) | T3 | P1 | **DF020.13 / new** | Semantic: scan mod assembly for `RegisterSystem<T>` calls where T matches existing `[BridgeImplementation(Replaceable = true)]` system in kernel reference assemblies; cross-check manifest `replaces` field contains T's FQN. Currently load-time enforcement via `ContractValidator`; analyzer adds compile-time signal. |
| Mod implements `IHotReloadOverride` (hypothetical future interface) without manifest `hotReload: true` declaration | MOD_OS_ARCHITECTURE.md §9.2 hot reload LOCKED (lines 940-950) + §9.6 `hotReload: false` semantics (lines 994-995) + D-7 LOCKED build-time rewriter (lines 1202-1212) | T2 | P2 | **DF020.14 / new (pending Mod API lock canonical text — interface itself TBD)** | **Pending Mod API lock canonical text** — `IHotReloadOverride` is hypothetical placeholder; Mod API lock deliberation will determine whether hot-reload override needs interface or remains pure manifest flag. Current D-7 LOCKED contract is the precedent (vanilla mods declare `hotReload: true` in source; build-time rewriter flips к `false` in release). DF020.14 sits in the «manifest schema vs. code» cross-check family — analyzer infrastructure should land at Mod API lock cascade. |
| Mod declares capability under another mod's namespace (`mod.<other-id>.publish:...`) | MOD_OS_ARCHITECTURE.md §3.3 reserved namespaces (lines 333-335): «Mods cannot claim to provide capabilities under another mod's namespace; the loader rejects such manifests» | T2 | P0 | **DF020.15 / new** | Static: read manifest as analyzer additional file; check each capability string in `capabilities.provided`; assert prefix `mod.<self-id>.*` matches manifest's own `id` field. Currently load-time enforcement; analyzer adds compile-time signal. |
| Mod manifest `capabilities.required` capability cannot be satisfied by kernel-provided set OR a dependency's `capabilities.provided` | MOD_OS_ARCHITECTURE.md §3.4 static check at load time (lines 337-345): «A required capability cannot be satisfied by a mod *not* listed in `dependencies`. This is a hard rule» | T2 | P0 | **DF020.16 / new** | Static: cross-check manifest `capabilities.required` against kernel-provided set + dependency manifests' `capabilities.provided`; emit `MissingCapability` analog at compile time. Currently load-time enforcement; analyzer adds compile-time signal per M3.4 «CI Roslyn analyzer for `[ModCapabilities]` honesty» pattern (deferred milestone, MOD_OS §11.1 line 1073). |

**Notes on §6.2.3 surface**:
- Manifest-vs-code cross-checks all require analyzer access to `mod.manifest.json` as an additional file (Roslyn `AdditionalFiles` MSBuild item). Domain 5 (Roslyn ecosystem state) recommendations for additional-files reading should apply directly.
- All §6.2.3 restrictions mirror existing load-time enforcement (per MOD_OS §3.4 + §3.3 + §7.1 + §11.2 baseline). DF020 family adds compile-time signal — same architectural philosophy as M3.4 (D-2 LOCKED hybrid completion).
- The `DualFrontier.Mod.ManifestAnalyzer` project skeleton precedent (per `DualFrontier.Mod.ManifestRewriter` pattern) is the recommended infrastructure landing target. ManifestRewriter has 127 LOC + Library entry point shape + Idempotency contract + Source preservation contract — same discipline applies к ManifestAnalyzer.

#### §6.2.4 — Forward-compatibility («grace period») semantics — analyzer-enforceable

| Restriction | Source | Tier | Priority | Rule shape | Notes |
|---|---|---|---|---|---|
| Deprecated IModApi types/methods still legal during grace period but flagged Warning | MOD_OS_ARCHITECTURE.md §4.5 v1 grace period (line 467): «log a v1-API warning. The mod author updates capability declarations in the manifest to migrate to functional v2 semantics. This grace period closes at kernel API version `2.0.0`»; K_CLOSURE §9.5 Q3 (line 1913): «Grace period semantics» | T2 | P2 | **DF020.17 / new (pending Mod API lock canonical text)** | **Pending Mod API lock canonical text** — current v1 grace-period semantic is documented (§4.5) but К-L20 will codify the FORWARD policy (deprecation cadence Q4, sunset criteria Q3-Q4). Static: detect calls к types/methods annotated `[Obsolete("...", error: false)]` on IModApi surface, emit DF020.17 Warning. Standard Roslyn `CS0618` pattern (`Obsolete` attribute) is the analyzer foundation; DF020.17 adds project-specific severity/messaging discipline. |
| Removed IModApi types/methods (post-grace) — illegal | MOD_OS_ARCHITECTURE.md §4.6.3 v3 strict cutover (line 588): «v2 IModApi was deleted entirely» — pattern; K_CLOSURE §9.5 Q2 «Manifest freeze policy» + Q4 «Deprecation cadence» (lines 1912-1914) | T2 | P1 | **DF020.18 / new (pending Mod API lock canonical text)** | **Pending Mod API lock canonical text** — current strict v3 cutover (no backward compat) is the precedent; К-L20 codification will determine whether future API evolution permits Bridge-mediated deprecation OR strict cutover. Static: detect calls к types/methods removed in current Mod API version surface; standard «compilation error» — DF020.18 catches the same error with project-specific diagnostic. |
| Manifest schema version compatibility (mod manifest schema version vs. kernel supported version) | MOD_OS_ARCHITECTURE.md §4.6.3 strict v3 LOCK (line 588): «the manifest parser rejects any `manifestVersion` other than `"3"`»; pending Mod API lock Q2 «Manifest freeze policy» (K_CLOSURE §9.5 line 1912) | T1 | P0 | **DF020.19 / extends DF020.12** | Trivial extension of DF020.12 (manifestVersion literal check) к forward versioning. Pending Mod API lock canonical text: whether v3 is permanent freeze («Manifest v3 frozen forever» per K_CLOSURE §9.5 line 1926) OR whether v4+ becomes possible via Bridge mechanism (Q1 deliberation outcome). |
| Mod uses Mod API surface element NOT present in baseline (`PublicApiAnalyzers`-style baseline file enforcement per Cascade #3 Q-K-§5-6 recommendation surfaced в Domain 6) | K_CLOSURE §9.5 Q1 Bridge mechanism design (line 1911); Cascade #3 Q-K candidate from §5.99 (forward design intent) | T2 | P1 | **DF020.20 / new (pending Mod API lock canonical text + leverages PublicApiAnalyzers)** | Static: maintain `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` baseline files (standard Roslyn `Microsoft.CodeAnalysis.PublicApiAnalyzers` pattern); DF020.20 leverages PublicApiAnalyzers infrastructure with project-specific severity к catch additions/removals from Mod API surface. **Cross-references Cascade #3 Q-K candidate** — Cascade #3 Q-K-§5-6 (per Domain 6 Reconnaissance Report §5.99) recommends expose IModApi surface as machine-readable baseline. |

**Notes on §6.2.4 surface**:
- Grace-period semantics are the LEAST analyzer-determinable surface in §6.2 because the semantic itself (how long is grace, what triggers sunset, whether Bridge mechanism mediates deprecation) is the Mod API lock deliberation Q1-Q4 outcome.
- DF020.17 + DF020.18 leverage standard Roslyn `[Obsolete]` attribute infrastructure; project-specific layer adds diagnostic messaging tying к К-L20 canonical text.
- DF020.20 (PublicApiAnalyzers integration) is the «highest-leverage» rule because it provides automated regression detection on Mod API surface changes — a single file diff in `PublicAPI.Shipped.txt` is a 1-line review for «did this PR add/remove anything on the Mod API surface?» discipline.

---

### §6.3 — A'.9-era preparatory rules (helping К-L20 era)

This sub-section identifies A'.9-era rules (not К-L20-specific) that LATER become foundations for К-L20 enforcement. The argument is: А'.9 analyzer infrastructure ships 18 active rules including DF003.1 / DF009 / DF012 / DF015 / DF015.1 — each carries domain knowledge (assembly identity, namespace scoping, P/Invoke call site detection, manifest cross-reference) that DF020 family inherits.

#### §6.3.1 — Precursor A'.9 rules → К-L20 forward-compat foundations

| A'.9.1-era rule | Becomes foundation for | Forward design notes |
|---|---|---|
| **DF003.1** (К-L3.1 Path β bridge — semantic analyzer covering ALC isolation, cross-mod ManagedStore access detection, dual SystemBase API enforcement) | **DF020.3** (cross-mod regular-ALC reflection access detection) | DF003.1 already implements assembly-identity analysis at `ManagedStore<T>` call sites; DF020.3 inherits the same assembly-identity primitive and applies к reflection-based cross-mod load attempts. Per Domain 1 §3.2 К-L3.1 entry (line 218): «cross-mod ALC boundary detection requires understanding of assembly load context which is runtime concept; static analysis may need conservative approximation» — DF020.3 inherits the same approximation discipline. |
| **DF009** (К-L9 mod parity — vanilla namespace direct-kernel-reference detection) | **DF020.1 / DF020.2 / DF020.8 / DF020.9** (Mod API surface deviation, kernel/native scheduler/native bus direct access by ANY mod assembly) | DF009 scope is «vanilla mods accessing non-IModApi surface»; DF020 family generalizes к ALL mod assemblies (vanilla and third-party). Per K_CLOSURE §7.2 DF009 row: «Active (revisit post-Mod API lock)» — the revisit means: at Mod API lock cascade, DF009's narrow vanilla-scope assembly-identity check expands к full mod-namespace assembly-identity check, becoming DF020.1's foundation. **Cross-references Domain 1 §3.2 К-L9 analysis line 395**: «К-L20 (Mod API lock will refine К-L9 surface)». |
| **DF012** (К-L12 native kernel scheduling — managed-side sovereign-authority detection + facade bypass detection) | **DF020.8** (native scheduler direct access by mods specifically) | DF012 currently detects «mods accessing native scheduler directly bypassing managed facade» (K_CLOSURE §7.2 line 1673). DF020.8 sharpens that detection с Mod-API-lock canonical semantic (vanilla mod gets identical restriction per К-L9 + К-L20 combined). DF012's `df_scheduler_*` P/Invoke call-site detection is the load-bearing primitive. |
| **DF015** (К-L15 native bus authority — managed-side sovereign routing + capability declaration bypass + tier-FQN registration violations detection) | **DF020.9** (native bus direct access by mods) + **DF020.10** + **DF020.11** (tier capability cross-check) | DF015 currently detects per-FQN per-tier capability declaration bypass. DF020.9 sharpens к mod-specific scope. DF020.10/DF020.11 inherit DF015's `[EventTier]` attribute cross-reference infrastructure for manifest-vs-code tier consistency check. |
| **DF015.1** (К-L15.1 three-tier independence — per-tier mutex isolation, tier-bit subscription ID isolation, fixed-order acquire) | **(weakly related, K-L20-orthogonal)** | DF015.1 is native-side concern primarily; minimal К-L20 relationship. Listed here for completeness — DF015.1 inherits multi-layer sub-rule precedent (Layer 1 state, Layer 2 runtime, Layer 3 compile-time) and DF020 family may adopt similar multi-layer structure (DF020.X.{namespace, manifest, runtime} sub-rules if К-L20 deliberation surfaces multi-aspect enforcement need). |
| **DF018** (К-L18 mod lifecycle quiescent state — sim-paused + pipeline-quiescent precondition detection) | **(К-L18 + К-L20 combined at Mod API lock cascade)** | DF018 detects mod load/unload bypassing the K-L18 quiescent state precondition. К-L20 lock adds: Mod API surface evolution requires K-L18 quiescent state for hot reload (Bridge mechanism's grace-period semantics interact with hot-reload). DF018's `PauseAsync → WaitForQuiescenceAsync → operation → ResumeAsync` sequence detection becomes the foundation for К-L20's «mod-API-version-upgrade-via-hot-reload» enforcement (if Mod API lock deliberation surfaces such mechanism). |
| **«CI Roslyn analyzer for `[ModCapabilities]` honesty»** (D-2 LOCKED hybrid completion, MOD_OS §11.1 M3.4 deferred milestone, NOT in 18-active list) | **DF020.10 / DF020.11 / DF020.16** (manifest-vs-code cross-check family) | M3.4 is currently deferred per MOD_OS §11.1: «Standalone analyzer package; runs in mod-publication CI, not at game load» + «unblocked when the first external mod author appears». Per A'.9 brief discipline: M3.4 may land at A'.9 (covering D-2 LOCKED hybrid completion) OR at Mod API lock cascade (combined с DF020.10/.11/.16). Decision deferred к A'.9.1 brief authoring. |

#### §6.3.2 — К-L20 LOCK timing estimate

Per K_CLOSURE §9 forward sequencing (lines 1822-1856) + memory entry [`project_a_prime_8_k_closure.md`] + ANALYZER_RULES.md §3:

```
[Current state 2026-05-24 — A'.9.0 Reconnaissance]
         ↓
[A'.9.1 brief authored — analyzer infrastructure + 18 active rules scope]
         ↓
[A'.9.X cascades — DF### rules implemented + first-run cleanup phase]
         ↓ — К-L14 verification #12 candidate (per K_CLOSURE §3.2)
         ↓ — 18 active rules + 4 reserved per S-LOCK-10
[A'.9 milestone CLOSED — analyzer infrastructure operational]
         ↓
[Mod API lock cascade — Q1-Q8 deliberation surface]
         ↓ — Q1 Bridge mechanism design (forward-compatible API evolution mechanism)
         ↓ — Q2 Manifest freeze policy (manifest schema v3 frozen; v1/v2 grace period sunset)
         ↓ — Q3 Grace period semantics (deprecation timeline; backwards-compatible operation duration)
         ↓ — Q4 Deprecation cadence (when к sunset deprecated API; semver implications)
         ↓ — Q5 К-L9 «Vanilla = mods» purity verification (vanilla mods using locked API for sustained period)
         ↓ — Q6 К-L20 invariant codification (Mod API forward-compatibility guarantee text)
         ↓ — Q7 DF020 analyzer rule scope (detection patterns for К-L20 violations)
         ↓ — Q8 MOD_API_CONTRACT.md authoring scope (Tier 1 LOCKED document)
[Mod API lock cascade CLOSED]
         ↓ — К-L20 LOCKED
         ↓ — DF020 family activated (DF020.1..DF020.20 per §6.2 enumeration; subset to be ratified by Q7 deliberation)
         ↓ — MOD_API_CONTRACT.md authored Tier 1 LOCKED
         ↓ — MOD_AUTHORING_GUIDE.md authored Tier 2
         ↓ — Manifest v1 grace period sunset
         ↓ — Manifest v3 frozen forever (or Bridge-mechanism-mediated v4+ — Q1 outcome)
         ↓ — К-L14 verification #N candidate (per K_CLOSURE §3.2 line 171)
[Phase B M-cycle vanilla content migration]
         ↓ — К-L20 + К-L9 «Vanilla = mods» combined verification via empirical mod authoring
```

**Estimated К-L20 LOCK timing** (per K_CLOSURE §9.4 estimated А'.9 duration ~10-15 hours auto-mode + Mod API lock cascade single cascade per §10.2 table line 2006):

- A'.9 milestone duration: ~10-15 hours auto-mode (per K_CLOSURE §9.4 line 1906)
- Mod API lock cascade duration: estimated comparable scope (Q1-Q8 deliberation + DF020 implementation + MOD_API_CONTRACT.md + MOD_AUTHORING_GUIDE.md) — likely 15-25 hours auto-mode given 8-Q surface
- **К-L20 LOCK timing**: post-A'.9 (hard gate per K_CLOSURE §9.5 line 1929: «Mod API lock complete before Phase B M-cycle begins»). Calendar-wise: weeks after A'.9.1 brief authoring at current hobby pace; no specific date commitment per «no commit-to-date discipline» (per K_CLOSURE §3.3).

**Implementation surface materialization sequence**:
- A'.9 ships analyzer infrastructure (Roslyn project, test harness, CI integration, 18 active rules, .editorconfig severity overrides per Domain 6 Reconnaissance Report)
- A'.9-era DF003.1 / DF009 / DF012 / DF015 / DF018 implementations encode reusable primitives (assembly-identity analysis, namespace scoping, P/Invoke call-site detection, manifest cross-reference)
- Mod API lock cascade activates DF020 family — implementation is incremental on top of A'.9 primitives
- Per К-L14 default-inclusion bias: DF020 family fully ratified at Mod API lock cascade may be a SUPERSET of §6.2 enumeration (DF020.1..DF020.20) depending on Q7 deliberation outcome

#### §6.3.3 — Mod API surface freeze prep

This sub-section addresses what А'.9.1-era can do к make К-L20 LOCK cascade easier. Surface enumerated per §6.3.1 precursor relationships + Domain 6 Cascade #3 Q-K-§5-6 baseline-file recommendation.

**А'.9.1-era preparation actions** (analyzer-side discipline that smooths Mod API lock cascade):

1. **Expose IModApi surface as machine-readable baseline via `PublicApiAnalyzers`** (per Cascade #3 Q-K-§5-6 surfaced в Domain 6):
    - Adopt `Microsoft.CodeAnalysis.PublicApiAnalyzers` NuGet package в `src/DualFrontier.Contracts/Modding/` project
    - Maintain `PublicAPI.Shipped.txt` (current public API surface) + `PublicAPI.Unshipped.txt` (additions pending shipping)
    - At Mod API lock cascade, `PublicAPI.Shipped.txt` becomes the К-L20 canonical surface manifest — automated regression detection on any addition/removal
    - **Rule infrastructure**: PublicApiAnalyzers provides RS0016 (missing PublicAPI entry) + RS0017 (removed PublicAPI entry); DF020.20 adds project-specific severity/messaging layer

2. **Implement DF003.1 / DF009 / DF012 / DF015 / DF018 with reusable primitives**:
    - Assembly-identity analysis (assembly attribute scan, namespace-prefix matching) — extract к shared analyzer infrastructure
    - Namespace scoping (`Vanilla.*` / `DualFrontier.Mod.*` / `DualFrontier.Core.*` / `DualFrontier.Application.*` whitelists) — externalize к `.editorconfig`-driven configuration per Domain 6 §8.3 severity-override surface
    - P/Invoke call-site detection (`[DllImport]` declarations targeting `DualFrontier.Core.Native.dll`) — reusable across DF002 / DF012 / DF015 / DF020.8 / DF020.9

3. **Adopt manifest-additional-file analyzer pattern**:
    - Configure Roslyn `AdditionalFiles` MSBuild item to expose `mod.manifest.json` к analyzer
    - Implement manifest-parsing utility (small helper, shared across DF020.10..DF020.16 + DF020.19)
    - Precedent: `tools/DualFrontier.Mod.ManifestRewriter/ManifestRewriter.cs` demonstrates idempotent JSON manipulation pattern — `DualFrontier.Mod.ManifestAnalyzer` follows same one-purpose-tool discipline

4. **Document IModApi surface explicitly в `MOD_API_CONTRACT.md` skeleton at A'.9 cascade**:
    - Pre-author skeleton (Tier 2 AUTHORED-SKELETON status) at A'.9 milestone closure
    - Mod API lock cascade promotes skeleton к Tier 1 LOCKED (per K_CLOSURE §2.23 line 791: «`docs/architecture/MOD_API_CONTRACT.md` — Tier 1 LOCKED authored at Mod API lock milestone»)
    - Skeleton at A'.9 provides structural anchor: IModApi v3 surface verbatim (already present в MODDING.md lines 47-86), manifest schema v3 verbatim (already present в MOD_OS §2.1), capability syntax verbatim (already present в MOD_OS §3.2)

5. **Capture Mod API surface state at A'.9 closure as baseline snapshot**:
    - At A'.9 milestone closure, commit current `IModApi.cs` + `ModManifest.cs` + capability-strings list as «A'.9 Mod API surface baseline» artifact
    - Mod API lock cascade Q2 (manifest freeze policy) + Q3 (grace period semantics) deliberation uses this snapshot as the reference state
    - Falsifiability anchor: any К-L20 LOCK-cascade-era change к the snapshot is recorded as Mod API evolution event (Bridge-mediated change OR grace-period sunset OR strict cutover)

**Forward-design implication for §6.2 enumeration**:

The §6.2 DF020.1..DF020.20 enumeration is reconnaissance-baseline scope; Q7 (K_CLOSURE §9.5 line 1917 «DF020 analyzer rule scope») deliberation at Mod API lock cascade will ratify the final DF020 family. Probable outcomes:

- **Subset ratification**: Q7 narrows §6.2 enumeration к 8-12 most-load-bearing rules (DF020.1..DF020.5 namespace/type + DF020.10..DF020.16 manifest cross-check + DF020.20 PublicApiAnalyzers integration)
- **Superset ratification**: Q7 expands §6.2 enumeration с rules specific to Bridge mechanism design (Q1) outcome — e.g., DF020.21 «Bridge invocation outside `replaces`-declared system» (if Bridge mechanism is invocation-based per Q1 deliberation)
- **Sub-rule structure**: Q7 adopts DF015.1-style multi-layer sub-rule pattern — DF020.X.{namespace, manifest, runtime} sub-rules

---

### §6.99 — Open questions surfaced (for §11 Q-K candidates)

- **Q-K-§6-1**: **Scope of DF020 family at Mod API lock cascade — subset or superset of §6.2 enumeration?**
  - Context: §6.2 enumerates 20 candidate restrictions (DF020.1..DF020.20). K_CLOSURE §9.5 Q7 «DF020 analyzer rule scope» is the deliberation surface that ratifies the final DF020 family.
  - Options:
    - (a) Subset — narrow к 8-12 most-load-bearing rules (foundational namespace/type + manifest cross-check + PublicApiAnalyzers)
    - (b) Full — ratify all 20 §6.2 candidates as DF020 family
    - (c) Superset — ratify §6.2 plus Bridge mechanism-specific rules (Q1 outcome dependent)
    - (d) Multi-layer sub-rules — DF020.X.{namespace, manifest, runtime} per DF015.1 pattern
  - Recommendation: **Defer к Mod API lock cascade Q7 deliberation**. А'.9.1-era brief should NOT pre-commit DF020 family scope; A'.9 ships analyzer infrastructure that DF020 family inherits, but DF020-specific implementation lands at Mod API lock cascade.

- **Q-K-§6-2**: **Should `PublicApiAnalyzers` be adopted at A'.9 cascade OR at Mod API lock cascade?**
  - Context: Cascade #3 Q-K-§5-6 recommendation surfaces baseline-file approach (per Domain 6 Reconnaissance Report). PublicApiAnalyzers provides RS0016/RS0017 infrastructure that DF020.20 leverages.
  - Options:
    - (a) Adopt at A'.9 cascade — sets up baseline tracking earlier; DF020.20 becomes incremental at Mod API lock
    - (b) Defer к Mod API lock cascade — combined adoption with К-L20 codification; one cascade contains all Mod API surface freeze infrastructure
  - Recommendation: **(a) Adopt at A'.9 cascade**. Early adoption provides empirical baseline data (which IModApi surface elements churn between A'.9 closure and Mod API lock cascade) — informs Q2 manifest freeze policy + Q3 grace period semantics deliberation with real data, not hypothesis.

- **Q-K-§6-3**: **Should `M3.4 — CI Roslyn analyzer for `[ModCapabilities]` honesty` (D-2 LOCKED hybrid completion per MOD_OS §11.1) land at A'.9 cascade OR at Mod API lock cascade?**
  - Context: M3.4 is MOD_OS §11.1 deferred milestone. It implements D-2 LOCKED hybrid completion (per-system `[ModCapabilities]` attribute manifest cross-check). It's adjacent к DF020.10/.11/.16 manifest-vs-code cross-check family.
  - Options:
    - (a) Land at A'.9 cascade — manifest-vs-code cross-check primitive available для DF020 family к inherit
    - (b) Land at Mod API lock cascade — combined с DF020.10/.11/.16
  - Recommendation: **(a) Land at A'.9 cascade**. M3.4 is а straightforward standalone analyzer; landing it early provides D-2 LOCKED hybrid completion + reusable manifest-cross-check infrastructure для DF020 family. Per MOD_OS §11.1 «unblocked when the first external mod author appears» — A'.9 timing is forward-compatible с that trigger.

- **Q-K-§6-4**: **Should `DualFrontier.Mod.ManifestAnalyzer` be a separate Roslyn analyzer project (per `DualFrontier.Mod.ManifestRewriter` precedent) OR integrated into the main `DualFrontier.Analyzer` package?**
  - Context: ManifestRewriter is а standalone build-time tool (127 LOC, library + thin CLI). ManifestAnalyzer is а Roslyn analyzer (compile-time). One-purpose-tool discipline (per ManifestRewriter precedent) suggests separation; package count minimization suggests integration.
  - Options:
    - (a) Separate project `tools/DualFrontier.Mod.ManifestAnalyzer/` mirroring ManifestRewriter structure
    - (b) Integrate into `DualFrontier.Analyzer` Roslyn package (single NuGet package, multiple analyzers внутри)
    - (c) Hybrid — separate test/development project, but ship combined в single NuGet package for distribution
  - Recommendation: **(b) Integrate into `DualFrontier.Analyzer` Roslyn package**. Manifest cross-check rules (DF020.10..DF020.16) need same `AdditionalFiles` infrastructure + same shared primitives (assembly identity, namespace scoping) as code-pattern rules. Per Domain 5 recommendation (Roslyn analyzer NuGet package conventions): one analyzer package per logical analyzer set. ManifestRewriter is build-time tool (different lifecycle); ManifestAnalyzer is compile-time tool (same lifecycle as DF### rules).

- **Q-K-§6-5**: **К-L20 canonical text scope — narrow (Mod API surface deviation detection only) OR wide (Mod API surface deviation + Bridge mechanism semantics + grace period semantics + manifest freeze policy combined)?**
  - Context: K_CLOSURE §2.23 falsifiability commitment text reads «Mod API v3 + manifest grace period semantics; mod authoring against locked API stays compatible; Bridge mechanism for forward-compatible amendments» — suggests wide scope. К-L9 «vanilla = mods» is the precedent narrow-scope (single architectural claim).
  - Options:
    - (a) Narrow К-L20: «Mod API surface element removal requires Bridge mechanism mediation OR grace period sunset; mods authored against API v3 stay compatible» — single claim, parallel к К-L9
    - (b) Wide К-L20: full forward-compatibility policy (Bridge mechanism design + grace period semantics + manifest freeze policy + deprecation cadence) — combined claim
    - (c) К-L20 + sub-invariants — К-L20.1 Bridge mechanism, К-L20.2 grace period semantics, К-L20.3 manifest freeze policy (per К-L3.1 / К-L7.1 / К-L15.1 sub-invariant precedent pattern)
  - Recommendation: **(c) К-L20 + sub-invariants**. Per К-L3.1 / К-L7.1 / К-L15.1 precedent (sub-invariant pattern adopted А'.0 + А'.7.x + К10.3 v2), wide-scope architectural concerns naturally decompose into parent invariant + 2-3 sub-invariants. Allows incremental LOCK transition (К-L20.1 Bridge mechanism could LOCK at Mod API lock cascade while К-L20.3 manifest freeze policy stays AUTHORED pending empirical verification).

- **Q-K-§6-6**: **Should А'.9.1 brief pre-author `MOD_API_CONTRACT.md` Tier 2 AUTHORED-SKELETON at A'.9 closure?**
  - Context: K_CLOSURE §2.23 plans MOD_API_CONTRACT.md as Tier 1 LOCKED at Mod API lock cascade. Per ANALYZER_RULES.md precedent (Tier 1 AUTHORED-SKELETON at А'.8 closure, populated к LOCKED at A'.9 cascade), early skeleton anchors the canonical artifact.
  - Options:
    - (a) Pre-author Tier 2 AUTHORED-SKELETON at A'.9 closure — anchors canonical artifact early; Mod API lock cascade promotes к Tier 1 LOCKED
    - (b) Defer all authoring к Mod API lock cascade — preserves Mod API lock cascade scope (Q1-Q8 deliberation determines canonical structure)
    - (c) Pre-author Tier 3 informal sketch at A'.9 closure — even lighter precommitment than (a)
  - Recommendation: **(a) Pre-author Tier 2 AUTHORED-SKELETON at A'.9 closure**. Per ANALYZER_RULES.md / K_L14_EVIDENCE_DASHBOARD.md skeleton precedent (А'.8 closure new DOC enrolled pattern), Tier 2 AUTHORED-SKELETON provides structural anchor с minimal scope commitment. Mod API lock cascade Q-N deliberation enriches к Tier 1 LOCKED without architectural surprise.

---

## §7 — Roslyn ecosystem state (desk research 2026-05-24)

### §7.0 — Research methodology

**Web searches performed** (2026-05-24, Sub-Agent B1, Domain 5):

1. `Microsoft.CodeAnalysis.CSharp NuGet latest stable version 2026` — currency: results dated through 1/13/2026 + 3/10/2026; current SDK confirmed
2. `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit latest version NuGet` — currency: results dated through 5/22/2026
3. `Roslyn analyzer development net8.0 best practices 2026` — currency: mix of 2024-2026 docs
4. `Roslyn analyzer severity DiagnosticSeverity Error Warning Info conventions dotnet runtime` — currency: MS Learn docs current
5. `Microsoft.CodeAnalysis.CSharp 5.3.0 release notes breaking changes netstandard2.0` — currency: 2026
6. `CodeFixProvider Roslyn analyzer best practices test pattern CSharpCodeFixVerifier` — currency: mix of 2024-2026
7. `dotnet/roslyn-analyzers severity assignment Error Warning Info rule defaults` — currency: MS Learn current
8. `Roslyn analyzer NuGet packaging analyzer codefix separate package` — currency: mix of 2018-2024 (packaging conventions stable across versions)
9. `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing NuGet generic framework-agnostic 2026` — currency: 2026
10. `Roslyn analyzer "DiagnosticSeverity.Hidden" vs Info vs Warning IDE rules architecture invariants` — currency: MS Learn current
11. `aspnet/AspNetCore analyzer diagnostic ID severity convention error warning` — currency: dotnet/aspnetcore main branch (current)

**Web fetches performed** (2026-05-24):

- `nuget.org/packages/Microsoft.CodeAnalysis.CSharp/` — confirmed 5.3.0 released 2026-03-10
- `nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` — flagged 1.1.2 as legacy/deprecated path
- `nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Analyzer.Testing` — confirmed 1.1.4 released 2026-05-22 (base package still required by framework-specific variants)
- `github.com/dotnet/roslyn-sdk/blob/main/src/Microsoft.CodeAnalysis.Testing/README.md` — testing framework guidance
- `github.com/dotnet/aspnetcore/blob/main/src/Framework/AspNetCoreAnalyzers/src/Analyzers/DiagnosticDescriptors.cs` — severity distribution empirical sample

**Citation discipline (S-LOCK-10)**: every numeric version + release-date + severity-distribution claim below cites a URL listed in §12.1 (consolidated cross-references). No claim relies on prior knowledge alone — all empirical anchors are dated 2024 or later, with primary NuGet/GitHub sources dated 2026.

---

### §7.1 — Current Roslyn SDK version (May 2026)

#### §7.1.1 — Core packages (latest stable, verified 2026-05-24)

| NuGet package | Latest stable | Released | Source |
|---|---|---|---|
| `Microsoft.CodeAnalysis.CSharp` | **5.3.0** | 2026-03-10 | [NuGet](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp/) |
| `Microsoft.CodeAnalysis.CSharp.Workspaces` | **5.3.0** | 2026-03-10 | [NuGet](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Workspaces/) |
| `Microsoft.CodeAnalysis` (meta-package) | **5.3.0** | 2026-03-10 | [NuGet](https://www.nuget.org/packages/Microsoft.CodeAnalysis/) |
| `Microsoft.CodeAnalysis.Common` | **5.3.0** | 2026-03-10 | [NuGet](https://www.nuget.org/packages/microsoft.codeanalysis.common/) |
| `Microsoft.CodeAnalysis.Analyzers` (analyzer-of-analyzers) | **5.3.0** | 2026-03-10 | [NuGet](https://www.nuget.org/packages/microsoft.codeanalysis.analyzers/) |
| `Microsoft.CodeAnalysis.NetAnalyzers` (FxCop replacement) | **10.0.203** | 2026 | [NuGet](https://www.nuget.org/packages/Microsoft.CodeAnalysis.NetAnalyzers) |

**Version line transition**: Roslyn jumped from the 4.x line (4.11 / 4.12 / 4.13) into the 5.x line in late 2025 / early 2026, aligning with the C# 14 / .NET 10 wave. Version 5.3.0 (March 2026) is the current stable; per [dotnet/roslyn issue #82780](https://github.com/dotnet/roslyn/issues/82780), 5.3.0 has at least one known regression in interceptor file handling (not relevant к DualFrontier analyzer scope — interceptors не used here).

#### §7.1.2 — Critical compatibility constraint: analyzer projects MUST target `netstandard2.0`

Per [dotnet/roslyn-analyzers README](https://github.com/dotnet/roslyn-analyzers/blob/main/README.md) + best-practices research synthesis:

> «Analyzer projects must target netstandard2.0, as the compiler loads analyzers into various host processes (Visual Studio on .NET Framework/Mono, MSBuild on .NET Core, dotnet build CLI) — targeting net8.0+ breaks compatibility with hosts that do not run on that runtime.»

This is **non-negotiable** despite Dual Frontier global `TargetFramework=net8.0` — the analyzer csproj **must override** к `netstandard2.0`. Consumer code (the rest of `src/`) remains net8.0; only the analyzer source assembly needs netstandard2.0.

Pattern:

```xml
<!-- src/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj -->
<PropertyGroup>
  <!-- Override Directory.Build.props net8.0 default -->
  <TargetFramework>netstandard2.0</TargetFramework>
  <LangVersion>latest</LangVersion>  <!-- analyzer can use newer C# than netstandard2.0 BCL implies -->
  <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
  <IsRoslynComponent>true</IsRoslynComponent>
</PropertyGroup>
```

**`EnforceExtendedAnalyzerRules`** (per [Microsoft.CodeAnalysis.Analyzers.md](https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/Microsoft.CodeAnalysis.Analyzers.md)) opts in к stricter RS#### rules — recommended for new analyzer projects к catch issues like missing `helpLinkUri` (RS1015), use of disallowed APIs in analyzers (RS1035), etc.

#### §7.1.3 — Analyzer-vs-code-fix separation (architectural constraint)

Per [Meziantou packaging guidance](https://www.meziantou.net/packaging-a-roslyn-analyzer-with-nuget-dependencies.htm) + [dotnet/roslyn-sdk #105](https://github.com/dotnet/roslyn-sdk/issues/105):

> «Analyzers must not reference the Roslyn Workspaces package although their code fixes are required to. To do this reliably, one must split these into two assemblies, yet package them up together.»

**Implication for A'.9.1 project layout**:
- `src/DualFrontier.Analyzers/` (OR `tools/DualFrontier.Analyzers/` per Domain 6 Option C) — references only `Microsoft.CodeAnalysis.CSharp` (no Workspaces); contains `DiagnosticAnalyzer` subclasses
- `src/DualFrontier.Analyzers.CodeFixes/` (only if §7.4 adoption recommendation accepted) — references `Microsoft.CodeAnalysis.CSharp.Workspaces`; contains `CodeFixProvider` subclasses
- Both packed together в single NuGet (`DualFrontier.Analyzers.nupkg`) for consumer simplicity

If A'.9.1 ships analyzer-only (no code-fixes), the second assembly is deferred to a later A'.9.x cascade.

#### §7.1.4 — VS / VSCode tooling compatibility

- **Visual Studio 2022 17.13+**: Roslyn 4.13 → built-in analyzer host updated to handle 5.x-loaded analyzers (per [MS Learn analyzer overview](https://learn.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview?view=visualstudio))
- **VS 2026 (preview)**: ships 5.x compiler natively
- **VSCode + C# Dev Kit**: tracks OmniSharp/LSP that bundles current Roslyn — analyzer NuGets referencing 5.3.0 surface diagnostics in both VS + VSCode без extra configuration
- **dotnet CLI**: SDK 8.0.300+ ships compatible analyzer host; SDK 10.0+ ships native 5.x

Crystalka's VS + VSCode workflow (per project context) requires no special handling — analyzer surfacing works из коробки.

#### §7.1.5 — Recommendation для A'.9.1

**Lock these versions в Directory.Build.props / Directory.Packages.props or per-csproj** (use Central Package Management `Directory.Packages.props` if not already adopted — gives one-place upgrade leverage):

```xml
<!-- Recommended pin for A'.9.1 analyzer infrastructure cascade -->
<PackageVersion Include="Microsoft.CodeAnalysis.CSharp" Version="5.3.0" />
<PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="5.3.0" />
<PackageVersion Include="Microsoft.CodeAnalysis.Analyzers" Version="5.3.0" PrivateAssets="all" />
```

Rationale:
- 5.3.0 = current stable, broad host compatibility
- All four core packages must be version-aligned (per general SDK guidance — mismatched 5.3.0 + 5.2.0 surfaces subtle MEF composition failures)
- `Microsoft.CodeAnalysis.Analyzers` (analyzer-of-analyzers) `PrivateAssets="all"` — analysis rules apply during build of the analyzer project, не leaked к consumers

If 5.3.x patch ships before A'.9.1 execution, prefer latest 5.3.x patch (zero break risk within minor).

---

### §7.2 — Test framework recommendations

#### §7.2.1 — Available variants (verified 2026-05-24)

| Package | Latest | Released | Maintenance status |
|---|---|---|---|
| `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing` (base) | **1.1.4** | 2026-05-22 | Active — base types, required by all framework variants |
| `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` | 1.1.2 | 2024-06-19 | **Last release predates 1.1.3/1.1.4 by ~2 years** |
| `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.MSTest` | 1.1.2 | 2024-06-19 | Same — lags base |
| `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.NUnit` | 1.1.2 | 2024-06-19 | Same — lags base |
| `Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.XUnit` | 1.1.2 | 2024-06-19 | Same |
| `Microsoft.CodeAnalysis.Testing.Verifiers.XUnit` | 1.1.2 | 2024-06-19 | Legacy verifier path |

Sources: [NuGet base 1.1.4](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Analyzer.Testing), [NuGet xUnit variant 1.1.2](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit).

#### §7.2.2 — Framework-specific variants: deprecation signal nuance

The [roslyn-sdk testing README](https://github.com/dotnet/roslyn-sdk/blob/main/src/Microsoft.CodeAnalysis.Testing/README.md) referenced [dotnet/roslyn-sdk issue #1175](https://github.com/dotnet/roslyn-sdk/issues/1175) as marking the framework-specific helper packages obsolete in favor of «generic test packages». However, empirical NuGet check (2026-05-24) shows:

- The **base package** (`Microsoft.CodeAnalysis.CSharp.Analyzer.Testing` 1.1.4) is the actively maintained generic — released **2026-05-22, two days ago**
- The **framework-specific variants** (XUnit/MSTest/NUnit) at 1.1.2 are at a release floor — they have not received version bumps to match 1.1.3/1.1.4 of the base
- Adoption metrics (per [NuGet](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Analyzer.Testing)): xUnit variant 2.9M downloads, NUnit 1.8M, MSTest 1.4M — community still using framework variants in 2026

**Net read**: framework variants are in *soft maintenance* (no new features, but still functional and version-compatible with base 1.1.4 transitively). The base package is where active development happens. New analyzer projects can either:
- **Option A**: Use the **base** package + write thin glue к xUnit (more code, but future-proof against framework-variant sunset)
- **Option B**: Use the **xUnit framework variant** + accept that it's frozen at 1.1.2 (less code, broader community precedent — still safe)

#### §7.2.3 — Version coupling к SDK

Per [roslyn-sdk testing README](https://github.com/dotnet/roslyn-sdk/blob/main/src/Microsoft.CodeAnalysis.Testing/README.md):

> «In case of version conflicts, override the version of Microsoft.CodeAnalysis.CSharp.Workspaces or Microsoft.CodeAnalysis.VisualBasic.Workspaces in your testing project to match the version»

Testing package 1.1.2 transitively brings `Microsoft.CodeAnalysis.CSharp.Workspaces >= 1.0.1`. Since our analyzer references 5.3.0, the test project csproj MUST explicitly pin the 5.3.0 version к override the transitive — otherwise NuGet picks lowest-acceptable (1.0.1) and the test host loads the wrong workspace assembly. Symptom: cryptic MEF composition exceptions at first test run.

```xml
<!-- tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj -->
<ItemGroup>
  <!-- Pin testing framework -->
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit" Version="1.1.2" />
  
  <!-- CRITICAL: override transitive Workspaces version to match analyzer's CodeAnalysis -->
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="5.3.0" />
  
  <!-- xUnit (test project keeps Dual Frontier net8.0 baseline) -->
  <PackageReference Include="xunit" Version="2.x" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.x" />
</ItemGroup>
```

#### §7.2.4 — Recommendation для A'.9.1

**Pick `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` 1.1.2** — explicit framework variant, not the base.

Rationale:
1. **xUnit baseline match**: Dual Frontier already uses xUnit (`DualFrontier.Core.Tests`, `DualFrontier.Systems.Tests`, etc. per project context) — zero new test framework к learn
2. **Highest community precedent**: 2.9M downloads = largest body of public analyzer test code за reference; debugging via Stack Overflow easier
3. **«Pragmatic move» principle** (Crystalka stated preference): xUnit variant ships `CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.VerifyAnalyzerAsync` working out-of-box; the base-package-only path requires writing 50-100 lines of xUnit-к-verifier glue per project — fragile churn без upside для A'.9.1 scope
4. **Soft-maintenance acceptable**: 1.1.2 still functional; if framework variants ever fully sunset, migration к base+glue is mechanical (no logic loss) and can happen at a future A'.9.x cleanup cascade

**Version-coupling discipline** (S-LOCK для A'.9.1 csproj): the test project MUST explicitly pin `Microsoft.CodeAnalysis.CSharp.Workspaces 5.3.0` к override the testing-package transitive — without this, MEF composition fails at first test run.

**Single test pattern** (per [roslyn-sdk testing README](https://github.com/dotnet/roslyn-sdk/blob/main/src/Microsoft.CodeAnalysis.Testing/README.md) recommendation):

> «VerifyCodeFixAsync automatically performs several tests related to code fix scenarios: diagnostic detection, fix quality, single-fix application, and bulk fix operations.»

For analyzer-with-codefix rules — prefer `VerifyCodeFixAsync` over separate `VerifyAnalyzerAsync` calls (one test exercises four scenarios). For analyzer-only rules — use `VerifyAnalyzerAsync`.

---

### §7.3 — Severity policy precedents

#### §7.3.1 — Available severity values

Per [MS Learn — Customize Roslyn analyzer rules](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022) + [Diagnostic.Severity API](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic.severity):

| Severity | Compile behavior | IDE behavior | `.editorconfig` token | Use case |
|---|---|---|---|---|
| `Error` | Build fails (CI red) | Red squiggle | `error` | Hard architectural violation; зatop production |
| `Warning` | Build warning; **fails** under `TreatWarningsAsErrors` | Yellow squiggle | `warning` / `warn` | Strong recommendation, configurable per consumer |
| `Info` / `Suggestion` | No build impact | Light bulb / gray dots | `suggestion` / `info` | Best-practice nudge; not blocking |
| `Hidden` | No build impact, не shown to user | Light bulb hint only | `silent` / `refactoring` / `hidden` | Triggers code-fix без noise |
| `None` | Rule disabled | Rule disabled | `none` | Explicit opt-out |

**Critical Dual Frontier interaction**: `Directory.Build.props` already sets `TreatWarningsAsErrors=true`. This means **Warning severity rules behave like Error rules at build time** for Crystalka's workflow. Implication: the «Error vs Warning» distinction is functionally **Error vs Error-but-suppressible-by-editorconfig** в this project. Real severity ladder is:

- `Error` → cannot be suppressed via `.editorconfig` без disabling entirely
- `Warning` → can be downgraded per-folder/per-file via `.editorconfig` overrides (still fails CI без override)
- `Info` → discoverable, never fails CI

This makes the distinction **important** даже несмотря на TWAE — `Warning` is the right choice for «correct в general but К-L claim may carve out exceptions».

#### §7.3.2 — Precedent #1: dotnet/roslyn-analyzers itself

Per [dotnet/roslyn-analyzers wiki](https://github.com/dotnet/roslyn-analyzers/wiki) + per general analyzer-development guidance returned by search:

> «Default to Warning for most rules. Use Error sparingly — users cannot suppress errors via EditorConfig without disabling the rule entirely.»

- **CA rules (FxCop)**: ~90% Warning, ~5% Info, ~5% Error
- **Error reserved for**: rules whose violation **certainly** indicates incorrect code that breaks behavior at runtime (e.g., CA2014 «Do not use stackalloc in loops»)
- **Warning default**: e.g., CA1801 «Review unused parameters» — discouraged but legal in some contexts
- **Info default**: usage suggestions, style nudges

#### §7.3.3 — Precedent #2: dotnet/aspnetcore (28 active descriptors empirically counted)

Per [DiagnosticDescriptors.cs main branch](https://github.com/dotnet/aspnetcore/blob/main/src/Framework/AspNetCoreAnalyzers/src/Analyzers/DiagnosticDescriptors.cs) — distribution:

- **Error: 6 descriptors** (~21%) — e.g., ASP0020 `RouteParameterComplexTypeIsNotParsable`, ASP0024 `AtMostOneFromBodyAttribute`
  - Pattern: violation causes **runtime exception** or **invalid binding** — cannot be «working code»
- **Warning: 16 descriptors** (~57%) — e.g., ASP0003 `DoNotUseModelBindingAttributesOnRouteHandlerParameters`, ASP0019 `DoNotUseIHeaderDictionaryAdd`
  - Pattern: anti-pattern that **happens to work** in narrow cases но discouraged
- **Info: 4 descriptors** (~14%) — e.g., ASP0015 `UseHeaderDictionaryPropertiesInsteadOfIndexer`, ASP0027 (marked «Unnecessary»)
  - Pattern: stylistic / convenience preferences

#### §7.3.4 — Precedent #3: IDE code style rules (IDExxxx)

Per [MS Learn analyzer rules](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022) + search result on IDE rules architecture:

> «All IDE code style rules are hidden or suggestions by default. This design is intentional to balance discoverability with build impact.»

Lesson for DualFrontier: «style» rules belong at Info or Hidden. К-L invariant rules (architectural correctness) belong at Warning minimum, Error when violation cannot be «working code».

#### §7.3.5 — `.editorconfig` override pattern

Per [MS Learn — Configure code analysis rules](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-options):

```ini
# Per-rule override (highest precedence)
dotnet_diagnostic.DF015.severity = error

# Per-category override (only affects enabled-by-default rules in category)
dotnet_analyzer_diagnostic.category-DualFrontierArchitecture.severity = warning

# All-rules override (only affects enabled-by-default)
dotnet_analyzer_diagnostic.severity = warning
```

**Caveat** (per same source + [dotnet/roslyn #61777](https://github.com/dotnet/roslyn/issues/61777)): when `AnalysisMode=All` / `AnalysisLevel` set globally, bulk `dotnet_analyzer_diagnostic.*` directives are ignored. For Dual Frontier — prefer per-rule overrides only; do not set bulk directives.

#### §7.3.6 — Recommendation for DF### severity assignments

Cross-referencing the existing `ANALYZER_RULES.md §4` table (15 Error + 4 Warning = 19 active; DF020 reserved):

**Confirm existing Error assignments (15 rules)** — these match the «runtime violation OR architectural correctness» criterion:
- **DF001, DF002, DF003, DF003.1, DF004, DF005, DF007, DF007.1, DF010, DF011, DF012, DF015, DF015.1, DF017, DF018**
- Rationale: violation of К-L1 (Godot reference), К-L2 (P/Invoke purity), К-L15.1 (mutex tier crossing) — these **cannot** be «working code with subtle issue»; they are flat architectural invariant breaches. Match ASP0020/ASP0024 pattern.

**Confirm existing Warning assignments (3 rules + 1 boundary case)**:
- **DF009 (К-L9 mod parity)** — currently Error per ANALYZER_RULES.md table; **recommend re-examining at A'.9.1 deliberation** — mod parity has «pending Mod API lock» status (note in §4: «revisit post-Mod API lock»). May benefit from Warning until К-L20 LOCK lands и Mod API surface frozen. Surfaced как **Q-K-§7-1** below.
- **DF013 (К-L13 on-demand activation, Warning)** — efficiency-not-correctness — confirmed Warning. Match CA1801 pattern.
- **DF016 (К-L16 sim tick pipeline depth, Warning, configurable)** — configurable threshold = correctness depends on consumer setting; Warning correct.
- **DF019 (К-L19 hardware tier, Warning, V substrate contract)** — contract surface depends on substrate tier consumer chose; Warning correct.

**No rules currently Info** — confirmed appropriate. К-L invariants are architectural promises, not stylistic preferences; Info-level К-L rule would be a category error.

**TWAE interaction reminder**: Since `TreatWarningsAsErrors=true` is enforced globally, Warning-tier DF013/DF016/DF019 fail CI by default. Consumers wanting к relax these per-folder use `.editorconfig` overrides per §7.3.5. This is **already the right design** — escape valve exists for the genuinely-configurable rules без weakening default discipline.

**Helper link URI policy** (per analyzer best-practices research, RS1015 rule):
- Every `DiagnosticDescriptor` MUST set non-null `helpLinkUri`
- Pattern: `https://github.com/Crystalka/Colony_Simulator/blob/main/docs/architecture/ANALYZER_RULES.md#df015` (anchor per rule)
- Enforced by `Microsoft.CodeAnalysis.Analyzers` RS1015 — fails build of analyzer if missing

---

### §7.4 — Code-fix provider patterns + adoption recommendation

#### §7.4.1 — `CodeFixProvider` API surface

Per [Meziantou — testing analyzers](https://www.meziantou.net/how-to-test-a-roslyn-analyzer.htm) + [roslyn-sdk testing README](https://github.com/dotnet/roslyn-sdk/blob/main/src/Microsoft.CodeAnalysis.Testing/README.md) + general best-practices guidance:

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DF002CodeFixProvider))]
[Shared]
public sealed class DF002CodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DF002Analyzer.DiagnosticId);

    public override FixAllProvider? GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;  // Most rules

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // 1. Get root, locate diagnostic span
        // 2. Compute fix (typically via SyntaxFactory transformations OR semantic-model rewrites)
        // 3. context.RegisterCodeFix(CodeAction.Create(title, ct => ApplyFix(context, ct), equivalenceKey: nameof(DF002CodeFixProvider)))
    }
}
```

**Critical requirements**:
- **`Shared`** attribute (MEF) — Roslyn host caches CodeFixProvider instances
- **`equivalenceKey`** on `CodeAction` — required для Fix-All across multiple violations; missing equivalenceKey → Fix-All silently no-op
- **`GetFixAllProvider`** override — return `WellKnownFixAllProviders.BatchFixer` for stateless mechanical fixes; return `null` for fixes that must be applied one-at-a-time (e.g., fixes that depend on user choice)
- **Cancellation token discipline** — pass `ct` through к all async ops; analyzer host cancels on IDE keystroke

#### §7.4.2 — When code-fixes are valuable vs noise

Per Aaronontheweb packaging guidance + general analyzer literature:

| Fix shape | Adoption guidance | Examples |
|---|---|---|
| **Mechanical 1-line replacement** | High value — adopt | `String.Format(...)` → interpolated string |
| **Add missing attribute/marker** | High value — adopt | Add `[Shared]` к MEF export |
| **Remove forbidden import/usage** | Medium value — adopt **if** unambiguous replacement | Remove `using Godot;` (no replacement — manual; DF001 = no codefix) |
| **Restructure architectural decision** | **Noise — do NOT adopt** | Move type between layers (DF003 component storage) |
| **Insert missing template** | Medium value — adopt **if** template fits 80%+ of cases | Generate boilerplate handler stub |
| **Modify pattern matched across multiple files** | Low value — defer | Refactoring spanning solution |

**General rule**: code-fix is appropriate when **violation has a single, correct, mechanical remedy that the user would type same way themselves 95% of the time**. Otherwise, the «fix» becomes architectural advice imposed by tool — wrong epistemic relationship.

#### §7.4.3 — Test pattern for code-fix providers

Per [roslyn-sdk testing README](https://github.com/dotnet/roslyn-sdk/blob/main/src/Microsoft.CodeAnalysis.Testing/README.md):

```csharp
[Fact]
public async Task DF002_CodeFix_ReplacesUnsafeMarshalWithSpan()
{
    var input = @"using System.Runtime.InteropServices;
public static class Native {
    public static unsafe void* AllocUnsafe(int size) => Marshal.AllocHGlobal(size).ToPointer();
}";
    var expected = @"using System.Runtime.InteropServices;
public static class Native {
    public static Span<byte> AllocSpan(int size) => new Span<byte>(...);  // expected fix output
}";

    await CSharpCodeFixVerifier<DF002Analyzer, DF002CodeFixProvider, DefaultVerifier>
        .VerifyCodeFixAsync(input, 
            new[] { new DiagnosticResult(DF002Analyzer.Descriptor).WithSpan(3, 25, 3, 65) },
            expected);
}
```

**Single test exercises four scenarios** (per same README §7.2.4 quote):
1. Analyzer correctly reports diagnostic at expected location в input
2. After applying code-fix, output matches expected (no fixable diagnostics remain)
3. Single-fix application produces expected output
4. Fix-All operation (when applicable) produces expected output across multiple sites

**Test naming convention** (Dual Frontier inheritance from xUnit baseline): `{RuleId}_CodeFix_{Scenario}` — pattern matches existing test naming in `DualFrontier.Core.Tests`.

#### §7.4.4 — NuGet packaging implications

Per [Meziantou packaging](https://www.meziantou.net/packaging-a-roslyn-analyzer-with-nuget-dependencies.htm) + [Aaronontheweb distribution guide](https://aaronstannard.com/roslyn-nuget/) + [roslyn-analyzers-docs ReadTheDocs](https://roslyn-analyzers.readthedocs.io/en/latest/create-nuget-package.html):

**Three packaging strategies**:

| Strategy | Pro | Con | Recommend для A'.9.x |
|---|---|---|---|
| **Single NuGet, analyzer-only assembly** | Simplest | No code-fixes possible | YES if A'.9.1 ships analyzer-only |
| **Single NuGet, analyzer + codefix assemblies (packed together)** | Best UX for consumers | csproj plumbing complex (`IsPackable=false` on analyzer csproj, packing project takes name) | YES if A'.9.1 ships any code-fixes |
| **Separate NuGets для analyzer vs codefix** | Cleanest separation | Two version-coupled packages к maintain | NO — overhead не justified |

**File layout in NuGet** (per ReadTheDocs guide):

```
DualFrontier.Analyzers.nupkg
└── analyzers/
    └── dotnet/
        └── cs/
            ├── DualFrontier.Analyzers.dll      ← references CodeAnalysis only
            └── DualFrontier.Analyzers.CodeFixes.dll  ← references Workspaces
```

The `analyzers/dotnet/cs/` path is **convention-enforced** — compiler MEF-discovers analyzers + codefix providers from this exact path. Wrong path = analyzer silently не loaded.

**`<DevelopmentDependency>true</DevelopmentDependency>`** on the analyzer csproj — recommended per Meziantou; prevents transitive flow (downstream consumers of a project that references the analyzer NuGet don't also pull the analyzer).

**Important для Dual Frontier scope**: since the analyzer is **internal-only** (no public NuGet publication planned), packaging discipline can be relaxed. Pattern:

```xml
<!-- src/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj -->
<PropertyGroup>
  <IsPackable>false</IsPackable>  <!-- internal use only -->
</PropertyGroup>

<!-- Consumer projects (Directory.Build.props or per-csproj) -->
<ItemGroup>
  <ProjectReference Include="..\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj"
                    OutputItemType="Analyzer" 
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

Direct `<ProjectReference OutputItemType="Analyzer">` skips NuGet entirely — analyzer DLL fed к compiler from build output. Simpler than packaging для internal use.

#### §7.4.5 — Per-rule code-fix feasibility cross-reference

Cross-referencing ANALYZER_RULES.md §4 table + general analyzer-development judgment:

| Rule | К-L | Code-fix feasibility | A'.9.1 adoption? |
|---|---|---|---|
| DF001 | К-L1 (no Godot) | Not feasible — no mechanical replacement for Godot usage; manual rewrite required | No |
| DF002 | К-L2 (pure P/Invoke) | **Trivial in tests** — mechanical unsafe→Span replacement | Maybe (if §3 analyzability matrix scores Trivial) |
| DF003 | К-L3 (component storage paths) | Not feasible — architectural decision per-component | No |
| DF003.1 | К-L3.1 (Path β bridge) | Not feasible — bridge identity decision | No |
| DF004 | К-L4 (type ID registry) | **Trivial** — auto-generate registration call | Yes (high-value) |
| DF005 | К-L5 (declarative bootstrap) | Not feasible — graph topology decision | No |
| DF007 | К-L7 (span protocol) | Moderate — mechanical signature rewrite (T→Span<T>) | Defer к A'.9.x (post-baseline) |
| DF007.1 | К-L7.1 (GPU pipeline slot) | Not feasible | No |
| DF009 | К-L9 (mod parity) | Not feasible — Mod API surface decision | No (revisit at К-L20 LOCK) |
| DF010 | К-L10 (decision rule) | Not feasible | No |
| DF011 | К-L11 (production storage backbone) | **Trivial in tests** (per Domain 3 cascade #2/#3 candidates) — mechanical AoS→SoA  | Maybe |
| DF012 | К-L12 (native kernel scheduling) | Not feasible | No |
| DF013 | К-L13 (on-demand activation) | Moderate — pattern-match rewrite | Defer |
| DF015 | К-L15 (native bus authority) | Not feasible | No |
| DF015.1 | К-L15.1 (3-tier mutex) | Not feasible — tier assignment decision | No |
| DF016 | К-L16 (sim tick pipeline depth) | Not feasible — depth target decision | No |
| DF017 | К-L17 (display composition) | Not feasible | No |
| DF018 | К-L18 (mod lifecycle quiescent) | Moderate | Defer |
| DF019 | К-L19 (hardware tier commitment) | Not feasible | No |

**Code-fix adoption summary**:
- **Adopt at A'.9.1** (Trivial + High-value): DF004 (auto-generate type registration)
- **Maybe adopt at A'.9.1** (depending on §3 analyzability scoring): DF002, DF011
- **Defer к A'.9.x cleanup cascades**: DF007, DF013, DF018 (Moderate complexity — value не justified в first analyzer release)
- **Never code-fix**: 14 rules architectural (DF001, DF003, DF003.1, DF005, DF007.1, DF009, DF010, DF012, DF015, DF015.1, DF016, DF017, DF019, DF020-pending)

**Recommendation для A'.9.1**:

Ship **analyzer-only** (no code-fixes) at A'.9.1.

Rationale:
1. **Scope discipline**: A'.9.1 = analyzer infrastructure cascade. Adding code-fix machinery (separate Workspaces-referencing assembly, FixAllProvider plumbing, fix-snapshot tests) doubles surface area
2. **Trivial-code-fix value ROI**: only 1-3 rules have Trivial code-fixes; can be added at A'.9.2 «code-fix providers cascade» without blocking first analyzer ship
3. **Cleanup phase consideration**: ANALYZER_RULES.md §3 specifies «first-run cleanup phase: warning→error promotion as architectural debt resolved». During this phase, code-fixes would auto-resolve violations — which **defeats the discipline** of human-reviewing each violation. Defer code-fixes until cleanup phase completes
4. **Test framework parity**: code-fix tests use `VerifyCodeFixAsync` (different verifier shape than analyzer-only `VerifyAnalyzerAsync`) — introducing both patterns simultaneously increases A'.9.1 cognitive load

If during A'.9.1 deliberation a clear Trivial-fix winner emerges (e.g., DF004 type registration auto-gen surfaces as cleanup multiplier), pull-forward к A'.9.1 — but default plan is analyzer-only ship.

**A'.9.2 (or similar) — «Code-Fix Providers» cascade scope sketch**:
- Add `src/DualFrontier.Analyzers.CodeFixes/` csproj (Workspaces-referencing)
- Implement DF004 code-fix (auto-generate type registration on missing-registration diagnostic)
- Implement DF002, DF011 code-fixes (if §3 analyzability matrix confirms Trivial scoring)
- Test infrastructure: switch from `VerifyAnalyzerAsync` к `VerifyCodeFixAsync` for affected rules
- NuGet packaging update (if A'.9.1 chose NuGet vs ProjectReference path)

---

### §7.99 — Open questions surfaced (for §11 Q-K candidates)

- **Q-K-§7-1**: Should DF009 (К-L9 mod parity) severity be downgraded Error → Warning until К-L20 Mod API lock lands?
  - Context: ANALYZER_RULES.md §4 marks DF009 «Active (pending A'.9 impl; revisit post-Mod API lock)». Currently Error. But mod parity rule fires against in-flux mod surface — until К-L20 LOCK, false-positive risk is high (rule «correct» relative to current mod surface but mod surface itself volatile). Cross-precedent (ASP0019 «DoNotUseIHeaderDictionaryAdd») uses Warning when the rule represents «anti-pattern that may evolve».
  - Options:
    - **A**: Keep Error — discipline-first, accept high false-positive churn
    - **B**: Downgrade к Warning until К-L20 LOCK — escape valve via `.editorconfig`, planned promotion post-LOCK
    - **C**: Defer DF009 implementation entirely к К-L20 LOCK cascade — analyzer infrastructure exists без the rule
  - Recommendation: **B** (Warning until К-L20 LOCK). Aligns with ASP0019 precedent + ANALYZER_RULES.md explicit «revisit post-Mod API lock» note. Promotion к Error becomes part of К-L20 LOCK cascade verification.

- **Q-K-§7-2**: Should DualFrontier adopt Central Package Management (`Directory.Packages.props`) at A'.9.1 to centralize Roslyn SDK version pin?
  - Context: §7.1.5 recommends pinning 5.3.0 across multiple csprojs (analyzer + tests + potentially codefix assembly). Without CPM, version drift between csprojs is silent failure surface. Dual Frontier may or may not already use CPM — Domain 6 sub-agent verification needed (Domain 6 confirms CPM not currently adopted).
  - Options:
    - **A**: Adopt CPM at A'.9.1 — comprehensive version discipline, fits «no costyl'» principle
    - **B**: Per-csproj pinning at A'.9.1 — minimal infrastructure change, accept manual discipline
    - **C**: Defer CPM adoption к separate methodology cascade — A'.9.1 stays narrowly-scoped
  - Recommendation: **A** (since Domain 6 confirms no CPM today) — version drift risk is real (analyzer/test/codefix triad must align) и CPM is one-file change.

- **Q-K-§7-3**: Should A'.9.1 ship via `ProjectReference OutputItemType="Analyzer"` or via NuGet package (.nupkg consumed from local feed)?
  - Context: §7.4.4 noted internal-only analyzer can skip NuGet machinery entirely with `ProjectReference OutputItemType="Analyzer"`. Simpler. But NuGet path gives versioning + freeze semantics that may matter for governance.
  - Options:
    - **A**: ProjectReference path — simplest, no NuGet plumbing, analyzer rebuilds on every solution build
    - **B**: Local NuGet feed path — versioned analyzer artifacts, can pin consumers к specific analyzer version (useful if cleanup phase wants frozen-rules contract)
    - **C**: Hybrid — ProjectReference в dev, NuGet for CI/release
  - Recommendation: **A** (ProjectReference). Single-developer internal-only context makes NuGet overhead unjustified. Re-evaluate at A'.9.x когда analyzer surface matures + multi-consumer scenarios materialize (e.g., separate Launcher analyzer subset).

- **Q-K-§7-4**: Should A'.9.1 enforce `Microsoft.CodeAnalysis.Analyzers` (analyzer-of-analyzers) at all DF### analyzer projects from day one?
  - Context: §7.1.5 recommends `EnforceExtendedAnalyzerRules=true` + `Microsoft.CodeAnalysis.Analyzers` PrivateAssets=all. This catches issues like missing helpLinkUri (RS1015), disallowed APIs в analyzers (RS1035), etc. But adds development friction — every new descriptor must satisfy stricter rules.
  - Options:
    - **A**: Enable from day one — catch issues early, ANALYZER_RULES.md helpLinkUri convention enforced automatically
    - **B**: Defer until A'.9.x cleanup cascade — minimize initial friction
    - **C**: Enable but downgrade RS1015 (helpLinkUri) к Warning for first cleanup phase only
  - Recommendation: **A** (enable from day one). Friction is minor; RS1015 in particular ensures every DF### has documentation anchor — aligns с governance discipline of cross-referenced rules.

- **Q-K-§7-5**: Should A'.9.1 introduce a dedicated `DualFrontierArchitecture` category for all DF### rules, enabling category-level `.editorconfig` overrides?
  - Context: §7.3.5 noted `dotnet_analyzer_diagnostic.category-<category>.severity` works for enabled-by-default rules. Category name appears в the `DiagnosticDescriptor` constructor.
  - Options:
    - **A**: Single `DualFrontierArchitecture` category — simplest, supports bulk overrides
    - **B**: Per-К-L-tier categories (`DualFrontierKernel`, `DualFrontierMod`, etc.) — finer-grained override surface
    - **C**: Use Microsoft-standard categories («Design», «Reliability») — matches NetAnalyzers convention but loses DF identity
  - Recommendation: **B** (per-К-L-tier categories: `DualFrontier.Architecture`, `DualFrontier.NativeBoundary`, `DualFrontier.ModSurface`). Supports targeted `.editorconfig` overrides (e.g., test-only files can downgrade `DualFrontier.ModSurface.*` без touching kernel rules). Matches ANALYZER_RULES.md taxonomic structure already implicit в К-L grouping.

- **Q-K-§7-6**: Should A'.9.1 baseline include `Microsoft.CodeAnalysis.NetAnalyzers` (10.0.203) alongside DF### rules?
  - Context: NetAnalyzers ships ~200 CA-prefix rules covering general .NET best practices (perf, reliability, security). Dual Frontier currently does not reference NetAnalyzers (greenfield per project context). Adding it surfaces a wave of CA violations alongside DF### violations during cleanup phase.
  - Options:
    - **A**: Adopt NetAnalyzers at A'.9.1 — broad coverage, established precedent
    - **B**: Defer NetAnalyzers — DF### cleanup phase already substantial; CA cleanup separate concern
    - **C**: Adopt with `AnalysisMode=Minimum` — only Error-default CA rules, defer Warning-default
  - Recommendation: **B** (defer). NetAnalyzers adoption is its own decision с its own cleanup cost. Keeping A'.9.1 focused on DF### rules respects discipline. Open separate brief A'.X for NetAnalyzers adoption когда DF### baseline stable.

---

## §8 — Build/CI integration surface

### §8.0 — Reconnaissance methodology

**Deep reads** (full file, S-LOCK-7 honest depth):
- `DualFrontier.sln` — 456 lines; full project enumeration + Solution Folders + NestedProjects mapping
- `Directory.Build.props` — 39 lines; PropertyGroup + SPIR-V CompileShaders MSBuild target
- `.editorconfig` — 4 lines; minimal stub (`root = true` + `[*] charset = utf-8`)
- `tools/governance/sync_register.ps1` — 395 lines; existing governance tooling pattern reference (PowerShell idioms, exit code convention, validation/sync dual-mode)
- `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` — 151 lines; AUTHORED-SKELETON anticipated structure

**Spot-reads** (representative csproj sample):
- `src/DualFrontier.Core/DualFrontier.Core.csproj` — minimal SDK-style, only ProjectReference + InternalsVisibleTo
- `src/DualFrontier.Launcher/DualFrontier.Launcher.csproj` — Exe OutputType + Native.dll copy pattern
- `tests/DualFrontier.Core.Tests/DualFrontier.Core.Tests.csproj` — test SDK + xunit + FluentAssertions + Native.dll copy + `<IsPackable>false</IsPackable>`+`<GenerateDocumentationFile>false</GenerateDocumentationFile>`

### §8.1 — sln structural integration points

**Solution structure** (per `DualFrontier.sln` deep read, lines 6–73 + 424–455 NestedProjects):

Four Solution Folders exist (`Project("{2150E333-…}")` GUIDs are Solution Folder type):
- `{11111111-…}` — **src** (lines 6–7): hosts all 12 src projects (Contracts, Core, Components, Events, Systems, AI, Application, Persistence, Core.Interop, Crypto.Future, Runtime, Launcher)
- `{22222222-…}` — **tests** (lines 8–9): hosts test projects (Core.Tests, Systems.Tests, Modding.Tests, Persistence.Tests, Core.Interop.Tests, Runtime.Tests, Runtime.SmokeTest, Application.Tests, ManifestRewriter.Tests, Core.Benchmarks) **+** Fixture.* projects **+** anomalously contains `VanillaMod_HotReloadOverride` fixture
- `{33333333-…}` — **mods** (lines 10–11): hosts `DualFrontier.Mod.Example` only (line 439)
- `{07C2787E-EAC7-C090-1BA3-A61EC2A24D84}` — **tools** (line 52): currently hosts **only** `DualFrontier.Mod.ManifestRewriter` (NestedProjects line 445)

**Notable conventions observed**:
- All src projects use modern SDK-style csproj GUID `{9A19103F-16F7-4668-BE54-9A1E7A4F7556}` for C# SDK projects (lines 12–28, 72)
- Some newer src projects (Crypto.Future line 60, Runtime line 64) use the legacy C# project GUID `{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}` — minor inconsistency (both work; legacy GUID is what VS auto-emits for some templates)
- tools folder is light — single project today (ManifestRewriter)
- `DualFrontier.Launcher` (line 72) has `Debug|Any CPU` + `Release|Any CPU` only (no x64/x86 variants per lines 416–419) — diverges from full 6-config matrix used by most projects

**Options analysis**:

**Option A** — `src/DualFrontier.Analyzers/` + `tests/DualFrontier.Analyzers.Tests/`:
- **Pros**: matches existing src/tests convention; test project lands in established `{22222222-…}` tests Solution Folder alongside Core.Tests etc.; closure-protocol `dotnet test` selectors (`tests/DualFrontier.*.Tests`) naturally pick it up
- **Cons**: analyzer is meta-infrastructure (acts on src, not part of runtime); placing under `src/` blurs "shipped code" vs "build tooling" boundary; analyzer csproj is structurally different (`<IsRoslynComponent>true</IsRoslynComponent>`, targets netstandard2.0, no implicit usings — see §8.2 below); will look out-of-place next to net8.0 game code

**Option B** — `tools/analyzers/DualFrontier.Analyzers/` + `tools/analyzers/DualFrontier.Analyzers.Tests/` (per A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md §2 A9.A line 60–63):
- **Pros**: matches brief's pre-AUTHORED skeleton intent; semantically correct — analyzer IS a build-time tool, not runtime code; sits next to existing `tools/DualFrontier.Mod.ManifestRewriter/` (line 54) which is also a build-side utility; tools Solution Folder already exists (line 52) but is underpopulated, this fills it; netstandard2.0 + Roslyn-specific csproj structure is visually segregated from net8.0 game projects
- **Cons**: introduces nested folder convention (`tools/analyzers/…/` vs current flat `tools/<Project>/`); the tests project for ManifestRewriter sits in `tests/DualFrontier.Mod.ManifestRewriter.Tests/` (line 56) — splitting analyzer tests into `tools/analyzers/` breaks that precedent; closure-protocol `dotnet test tests/…` glob may need extension to also pick up `tools/analyzers/*.Tests/`

**Option C** — Hybrid: `tools/analyzers/DualFrontier.Analyzers/` (analyzer csproj) + `tests/DualFrontier.Analyzers.Tests/` (test csproj following ManifestRewriter precedent):
- **Pros**: analyzer where it semantically belongs (build tooling next to ManifestRewriter); tests where all test projects live (uniform `tests/` discovery); no `dotnet test` glob change needed; matches ManifestRewriter precedent exactly (tool csproj in `tools/<Project>/`, test csproj in `tests/<Project>.Tests/`)
- **Cons**: minor — tests project will need a longer relative ProjectReference path (`..\..\tools\analyzers\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj`); slightly less local than co-located tests

**Recommendation**: **Option C** (hybrid). It matches the **existing ManifestRewriter precedent verbatim** (a Roslyn-equivalent build-tooling project where tool lives in `tools/` and tests live in `tests/` — sln lines 54+56), respects the semantic distinction (analyzer is not runtime code), and keeps `tests/` as the single source of truth for test discovery. NestedProjects entries should map analyzer → `{07C2787E-…}` tools folder and tests → `{22222222-…}` tests folder, matching ManifestRewriter (sln lines 445–446).

**Sub-recommendation on tools folder layout**: do NOT introduce a `tools/analyzers/` subfolder if there is only one analyzer project. Flat `tools/DualFrontier.Analyzers/` matches ManifestRewriter pattern (`tools/DualFrontier.Mod.ManifestRewriter/`). The brief's "tools/analyzers/" path is skeleton-level guesswork; recommend overriding with the established flat convention for consistency. If multiple analyzer projects appear later, refactor at that point.

### §8.2 — Directory.Build.props integration recommendation

**Current state** (per `Directory.Build.props` deep read, lines 1–39):

The file contains:
1. **Single PropertyGroup** (lines 2–14) applying to every project in the solution: `TargetFramework=net8.0`, `LangVersion=12.0`, `Nullable=enable`, `ImplicitUsings=enable`, `TreatWarningsAsErrors=true`, `GenerateDocumentationFile=true`, `NoWarn=$(NoWarn);CS1591`, `Company=DualFrontier`, `Product=Dual Frontier`
2. **One conditional MSBuild Target** `CompileShaders` (lines 22–38) scoped via `Condition="'$(MSBuildProjectName)' == 'DualFrontier.Runtime'"` — invokes `tools/glslangValidator.exe` to compile GLSL → SPIR-V before Build

Notable: no ItemGroup, no PackageReference, no centralized ProjectReference at the Directory.Build.props level today. Each csproj independently declares its references (spot-read sample confirms this — Core.csproj lines 6–14, Launcher.csproj lines 7–10, Core.Tests.csproj lines 8–18 all use local `<ItemGroup>` blocks).

Three test projects must override the inherited `GenerateDocumentationFile=true` (Core.Tests.csproj line 6: `<GenerateDocumentationFile>false</GenerateDocumentationFile>`) — established pattern for "test projects opt out of doc generation". Same projects also override `<IsPackable>false</IsPackable>` (line 5).

**Options analysis**:

**Option A** — **Per-project `<ProjectReference>` with analyzer attributes** (each csproj individually adds analyzer reference):
```xml
<ProjectReference Include="..\..\tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false" />
```
- **Pros**: matches existing pattern of local ItemGroups (no Directory.Build.props pollution); each project explicitly opts in; trivially excludes projects that shouldn't be analyzed (e.g., the analyzer itself, fixtures)
- **Cons**: 12 src projects × identical 4-line ItemGroup = boilerplate duplication; future src additions risk forgetting to add the reference (silent analysis miss); ~30 test/fixture projects need decision — if every test/fixture also gets the reference, that's ~42 csproj edits; if opt-in, possible coverage gaps

**Option B** — **Centralized `<ProjectReference>` in `Directory.Build.props`** with conditional scoping:
```xml
<ItemGroup Condition="'$(MSBuildProjectName)' != 'DualFrontier.Analyzers'
                  And '$(MSBuildProjectName)' != 'DualFrontier.Analyzers.Tests'
                  And !$(MSBuildProjectName.StartsWith('Fixture.'))">
  <ProjectReference Include="$(MSBuildThisFileDirectory)tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```
- **Pros**: single declaration covers all 40+ projects automatically; future src additions inherit analyzer enforcement with zero csproj edits; matches the existing centralized pattern of `Directory.Build.props` (which already centralizes TargetFramework, TreatWarningsAsErrors, etc.); the `CompileShaders` Target (lines 22–38) already demonstrates project-name-conditional MSBuild logic exists in this file (precedent for `Condition="'$(MSBuildProjectName)' == '…'"`)
- **Cons**: requires careful exclusion list to prevent the analyzer from analyzing itself (cycle) and Fixture.* projects that intentionally violate rules; centralized exclusion list is single-edit-point but easier to miss; relative path from Directory.Build.props (root) is uniform but Fixture/mods projects nest at different depths — `$(MSBuildThisFileDirectory)tools/…` reads natively because Directory.Build.props lives at root

**Option C** — **NuGet PackageReference distribution**:
- Pack `DualFrontier.Analyzers.csproj` as `.nupkg`, publish to local feed or NuGet.org, reference via `<PackageReference Include="DualFrontier.Analyzers" Version="…">`
- **Pros**: industry-standard pattern; clean dependency boundary; supports per-project version pinning
- **Cons**: heavy for solo-dev solution (no NuGet feed infrastructure exists today, no `.nupkg` publishing today, no Versioning workflow for analyzer); introduces release cadence concern (analyzer rule change → repack → republish → consume); fundamentally over-engineered for a monorepo that builds entirely in one `dotnet build` invocation; loses immediate feedback loop (edit analyzer → next build sees new rules)

**Recommendation**: **Option B** (centralized in Directory.Build.props) with explicit exclusion conditional. Rationale:

1. **Consistency with existing centralization pattern**: every cross-cutting concern in this solution is already centralized in Directory.Build.props (TargetFramework, language version, warnings-as-errors, shader compilation target). Analyzer enforcement is exactly the same kind of cross-cutting concern.
2. **Future-proof**: 40+ projects today, more expected — opt-in-per-csproj scales poorly and risks silent coverage gaps when projects are added.
3. **The MSBuildProjectName conditional precedent is already established** at line 23 (`'$(MSBuildProjectName)' == 'DualFrontier.Runtime'`) — extending this pattern for exclusions is idiomatic.

**Insertion-point sketch** for `Directory.Build.props` (insert after the existing PropertyGroup at line 14, before the `CompileShaders` Target):

```xml
  <!--
    A'.9.1 Roslyn analyzer integration. Applies to every project except the
    analyzer itself, its test project, and Fixture.* projects (which intentionally
    encode violation scenarios). Exclusion list mirrors the analyzer's own
    AnalyzerConfigOptions opt-out wiring.
  -->
  <ItemGroup Condition="'$(MSBuildProjectName)' != 'DualFrontier.Analyzers'
                    And '$(MSBuildProjectName)' != 'DualFrontier.Analyzers.Tests'
                    And !$(MSBuildProjectName.StartsWith('Fixture.'))">
    <ProjectReference Include="$(MSBuildThisFileDirectory)tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false"
                      PrivateAssets="all" />
  </ItemGroup>
```

`PrivateAssets="all"` prevents the analyzer reference from leaking through transitive consumers (the example mod, packed contracts).

**Caveat (becomes Q-K candidate)**: `Directory.Build.props` is imported by every csproj including the analyzer itself. The conditional must exclude the analyzer csproj BEFORE the analyzer csproj has been built — MSBuild evaluates the conditional at evaluation time using only `$(MSBuildProjectName)`, which is available immediately, so the cycle-break works. But if the analyzer csproj inherits the centralized `<TargetFramework>net8.0</TargetFramework>` (line 3), it cannot — Roslyn analyzers MUST target `netstandard2.0`. See §8.99 Q-K-§8-1 below.

### §8.3 — .editorconfig severity override surface

**Current state** (per `.editorconfig` deep read, 4 lines):

```ini
root = true

[*]
charset = utf-8
```

That is the entire file. No analyzer severity directives, no formatting rules, no naming conventions, no IDE preferences. A'.9.1 has greenfield authority on the analyzer severity convention — there are no pre-existing rules to negotiate with.

**Tension to consider** (per `Directory.Build.props` line 7): `TreatWarningsAsErrors=true` is set globally. This means **any analyzer rule emitted at `warning` severity will be promoted to error at build time**, identical to compiler warnings. This has direct consequences for the brief's A9.C sub-milestone strategy (first-run cleanup with rules initially at Warning):

- **First run at default Warning severity → build fails immediately** because of `TreatWarningsAsErrors=true`
- **First run requires explicit override** — either `<TreatWarningsAsErrors>false</TreatWarningsAsErrors>` temporarily (unsafe, hides genuine compiler warnings), or per-rule `severity = suggestion` (visible in IDE, not promoted to error), or `<WarningsNotAsErrors>DF001;DF002;…</WarningsNotAsErrors>` (surgical)
- This is **the central A'.9.1 tension** the brief A9.C/A9.D progression must address

**Recommended baseline `.editorconfig` structure for A'.9.1**:

```ini
root = true

[*]
charset = utf-8

# ============================================================================
# A'.9.1 Roslyn architectural analyzer — К-Lxx invariant enforcement
# Rule catalog: tools/DualFrontier.Analyzers/ANALYZER_RULES.md
# ============================================================================

[*.cs]

# --- DF### : К-Lxx invariant rules (kernel architectural invariants) ---
# Per К-closure report (A'.8) Part 6. К-L6 SUPERSEDED + К-L14 meta-invariant
# (not encodable) — see ANALYZER_RULES.md §1.1 for exclusion rationale.
# Initial cascade A'.9.1 severity = suggestion (visible, non-fatal) for
# cleanup phase A9.C; promoted to error in A'.9.2 sub-cascade per A9.D.
dotnet_diagnostic.DF001.severity = suggestion   # К-L1 native language
dotnet_diagnostic.DF002.severity = suggestion   # К-L2 pure P/Invoke
dotnet_diagnostic.DF003.severity = suggestion   # К-L3 component storage paths
dotnet_diagnostic.DF004.severity = suggestion   # К-L4 explicit registry
dotnet_diagnostic.DF005.severity = suggestion   # К-L5 declarative bootstrap
dotnet_diagnostic.DF007.severity = suggestion   # К-L7 span protocol
dotnet_diagnostic.DF007_1.severity = suggestion # К-L7.1 GPU slot binding
dotnet_diagnostic.DF008.severity = suggestion   # К-L8 component lifetime
dotnet_diagnostic.DF009.severity = suggestion   # К-L9 mod parity
dotnet_diagnostic.DF010.severity = suggestion   # К-L10 decision rule metrics
dotnet_diagnostic.DF011.severity = suggestion   # К-L11 NativeWorld SSoT
dotnet_diagnostic.DF012.severity = suggestion   # К-L12 native scheduling
dotnet_diagnostic.DF013.severity = suggestion   # К-L13 on-demand activation
dotnet_diagnostic.DF015.severity = suggestion   # К-L15 native bus three-tier
dotnet_diagnostic.DF016.severity = suggestion   # К-L16 pipeline depth
dotnet_diagnostic.DF017.severity = suggestion   # К-L17 display composition
dotnet_diagnostic.DF018.severity = suggestion   # К-L18 mod lifecycle quiescent
dotnet_diagnostic.DF019.severity = suggestion   # К-L19 hardware tier

# --- DC### : Cascade-specific rules (drift detection between cascades) ---
# Per Domain 3 cascade #2/#3 candidates (~10 rules anticipated). Default
# severity warning (promoted to error via TreatWarningsAsErrors after
# A'.9.2 ramp).
# (specific rules added as authored)

# --- DL### : Lesson-derived rules (Lesson #N family automated catch) ---
# Per Domain 2 FORMALIZE Lessons analyzability matrix (~5 rules anticipated).
# (specific rules added as authored)

# ----------------------------------------------------------------------------
# Test + fixture project per-folder opt-outs
# ----------------------------------------------------------------------------

# Fixture projects intentionally encode violation scenarios; rules muted here
# so the fixtures themselves compile clean while exercising analyzer logic.
[tests/Fixture.*/**/*.cs]
dotnet_diagnostic.DF001.severity = none
dotnet_diagnostic.DF002.severity = none
dotnet_diagnostic.DF003.severity = none
# (extend per-rule as fixture coverage grows)

# Test projects: К-L invariants apply to production code only. Tests
# legitimately construct synthetic scenarios that violate (e.g., creating
# ad-hoc ManagedTestWorld instances для К-L11 stress fixtures).
[tests/**/*.cs]
dotnet_diagnostic.DF011.severity = suggestion   # tests may legitimately use ManagedTestWorld
dotnet_diagnostic.DF012.severity = suggestion   # tests may legitimately use managed scheduler

# Benchmark project: К-L8/К-L11 relaxed (cross-implementation comparison probe)
[tests/DualFrontier.Core.Benchmarks/**/*.cs]
dotnet_diagnostic.DF008.severity = none
dotnet_diagnostic.DF011.severity = none

# Mod example: К-L9 enforcement applies (vanilla = mods principle), but
# certain demo-only patterns may need suggestion-level relaxation
[mods/**/*.cs]
# (no specific overrides initially — mod example should be exemplary)
```

**Discussion of convention**:

1. **Rule ID grouping** (DF### / DC### / DL###):
   - `DF` (Dual Frontier) → К-Lxx invariant rules, 18 rules covering all encodable К-L invariants
   - `DC` (Dual frontier Cascade) → cascade-specific drift detection, ~10 rules anticipated for catching M-series migration drift
   - `DL` (Dual frontier Lesson) → Lesson-derived rules, ~5 rules anticipated, automating catches from N1-N14 lessons (per current state Lessons #N12-#N14 from К-extensions cascade #3)

2. **Opt-in/opt-out per folder** uses `.editorconfig` glob syntax `[tests/Fixture.*/**/*.cs]` which respects directory hierarchy. This is the standard pattern and works without nested `.editorconfig` files. Per-folder `.editorconfig` files can be added later if rule overrides become voluminous, but a flat root-level config is recommended at A'.9.1 inception для readability.

3. **Severity grouping convention**:
   - `error` → invariant violation, build must fail (post-A'.9.2 ramp)
   - `warning` → in conjunction with `TreatWarningsAsErrors=true` ALSO promotes to error; use only for rules that should be promotable but not yet
   - `suggestion` → IDE-visible, non-fatal, doesn't trigger `TreatWarningsAsErrors`; **the safe default for first-run cleanup phase A9.C**
   - `silent` → analyzer runs but no diagnostic surfaces (useful for telemetry-only rules during authoring)
   - `none` → analyzer doesn't run for matching files (use for Fixture.* exclusions)

4. **Tension with `TreatWarningsAsErrors=true`** is resolved by **never using `warning` severity during the cleanup phase**. The cleanup phase uses `suggestion` (visible but non-fatal); the ramp to error (A9.D) jumps directly from `suggestion` → `error`, skipping `warning` entirely. Alternative — keep `warning` and add `<WarningsNotAsErrors>DF001;…;DF019</WarningsNotAsErrors>` to Directory.Build.props during cleanup — is more surgical but adds inconsistency between Directory.Build.props and .editorconfig (two places to update at promotion time). The "skip warning, jump suggestion → error" path is cleaner.

5. **Test project relaxation precedent**: test csproj files already override inherited `<GenerateDocumentationFile>` (Core.Tests.csproj line 6). The .editorconfig per-folder relaxation for `[tests/**/*.cs]` mirrors this principle (test code lives by relaxed rules vs production).

### §8.4 — CI integration trigger recommendation

**Current state** (per inventory):
- No `.github/workflows/*.yml` exists — no GitHub Actions CI today
- `dotnet build` is the local-only verification gate
- Closure-protocol §12.7 mandates `dotnet build` + targeted `dotnet test` per cascade closure (Modding suite + relevant Domain)
- `tools/governance/sync_register.ps1` is the existing closure-time governance gate (exit 0 required per K10.3 protocol, lines 388–394)
- Roslyn analyzers are **invoked automatically by the .NET SDK during every `dotnet build`** — no separate CI trigger needed; analyzer activation is purely a function of (a) the analyzer being referenced via `<ProjectReference OutputItemType="Analyzer">` per §8.2, and (b) per-rule severity per §8.3

**Discussion**:

1. **Automatic on every `dotnet build`** — yes, by default. Once the analyzer is referenced per §8.2 Option B, every `dotnet build` invocation (local closure verification, future CI) runs the analyzer in-process during compilation. No explicit step needed. This is desirable: the same analyzer that surfaces in the IDE (Visual Studio/Rider) also runs at build time, single source of truth.

2. **`TreatWarningsAsErrors=true` × analyzer warning interaction** (per `Directory.Build.props` line 7): as documented in §8.3, this is the central tension. **Intentional configuration recommendation**: keep `TreatWarningsAsErrors=true` global; do NOT add `<TreatWarningsAsErrors>false</TreatWarningsAsErrors>` per-project; instead control analyzer severity exclusively via .editorconfig. This preserves the existing strictness on compiler warnings (CS####) while giving fine-grained per-rule control on analyzer diagnostics (DF/DC/DL ###). The trade-off: rule promotion is .editorconfig-edit-only (no Directory.Build.props change), making the A9.D promotion a single file diff.

3. **First-run cleanup phase implications** (per brief A9.C lines 86–90):
   - Initial rule deployment: all 18 DF### at `severity = suggestion` per §8.3 (non-fatal, build still passes)
   - First `dotnet build` after analyzer reference lands: build succeeds; rule diagnostics surface in IDE Problems panel and `dotnet build` text output but don't fail the build
   - Cleanup phase scope: each surfaced diagnostic triaged into (a) fix in code, (b) suppress with rationale (`#pragma warning disable DF### // <citation>`), (c) rule refinement if false-positive
   - Brief A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md §0 line 33: "First run expected к surface pre-existing debt — fix budget внутри А'.9 scope. Linux `-Werror` adoption precedent." Strongly suggests the cleanup phase must be **bounded** — a defined target (e.g., "all DF001-DF019 emit zero diagnostics on src/ paths") before promotion
   - **Q-K candidate (anticipated в Brief A'.9.1)**: Fix budget — hard cap (e.g., 100 violations) or open-ended within А'.9 scope? Recommendation: open-ended within А'.9.1, but if budget exceeds X violations the cascade splits (A'.9.1 = scaffolding + cleanup; A'.9.2 = rule promotion)

4. **`dotnet test` selector implications**: closure protocol selectors must additionally pick up the analyzer test project. Per §8.1 Option C recommendation (tests in `tests/DualFrontier.Analyzers.Tests/`), the existing `tests/DualFrontier.*.Tests` glob continues to work without modification.

5. **Future GitHub Actions consideration** (flag for §11, not required at A'.9.1):
   - No CI infrastructure exists today; manual local-only verification is the convention
   - Roslyn analyzers DO run identically in any environment that invokes `dotnet build` — so when GitHub Actions is eventually adopted, no analyzer-specific CI changes are needed; the same `dotnet build` step surfaces the same diagnostics
   - The analyzer ITSELF having tests creates an interesting chicken-and-egg: the analyzer's own csproj should NOT reference itself as an analyzer (per §8.2 exclusion conditional), and the analyzer test project should reference Microsoft.CodeAnalysis.Testing — not the analyzer-as-analyzer
   - **Flag for §11**: should A'.9 include a minimal `.github/workflows/build.yml` running `dotnet build` + `dotnet test` + `tools/governance/sync_register.ps1 -Validate` to make analyzer enforcement enforceable on PRs (when the project transitions out of solo-dev)? Recommendation: NO at A'.9.1 (out of scope, separate concern, no PR workflow yet), but candidate for A'.10 or sibling milestone if collaboration model changes.

**Recommended A'.9.1 progression** (Warning → cleanup → Error staged plan):

**Stage 1 — A'.9.1 ξ-cascade (analyzer scaffolding + initial rule authoring)**:
- Add `tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj` + `tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj` per §8.1 Option C
- Add centralized analyzer reference to `Directory.Build.props` per §8.2 Option B
- Add baseline `.editorconfig` rules at `severity = suggestion` per §8.3
- Author DF001-DF019 (skipping DF006 К-L6 SUPERSEDED + DF014 К-L14 meta) — 18 rules
- Each rule has positive + negative test per A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md A9.E
- **Closure gate**: `dotnet build` exit 0 (build passes even с suggestions surfacing); `dotnet test tests/DualFrontier.Analyzers.Tests` exit 0; existing test suite exit 0 (rules at suggestion don't break existing builds); `sync_register.ps1 -Validate` exit 0

**Stage 2 — A'.9.1 χ-cascade (cleanup phase)**:
- Run analyzer against full codebase via `dotnet build` — collect all DF### diagnostics
- Per-diagnostic triage per brief A9.C (fix / suppress with rationale / refine rule)
- Fix budget tracked в cleanup ledger (recommend open-ended within A'.9.1 with soft-halt to Crystalka if exceeds e.g. 150 violations)
- **Closure gate**: zero DF### diagnostics on src/ paths at suggestion severity (i.e., the analyzer would emit zero warnings if promoted to warning); suppression-with-rationale count documented in cleanup report

**Stage 3 — A'.9.1 ψ-cascade (severity promotion)**:
- `.editorconfig` edit: each `dotnet_diagnostic.DF###.severity = suggestion` → `= error`
- Single-file diff per §8.4 recommendation #2
- `dotnet build` exit 0 after promotion (cleanup is complete; promotion is the test)
- **Closure gate**: `dotnet build` exit 0 with all DF### at `severity = error`; this proves К-Lxx compile-time enforcement is live
- Per brief A9.D: CodeFixProviders authored where mechanical fix possible — likely sub-cascade or follow-up milestone

**Stage 4 — sync_register.ps1 integration (optional, low-priority)**:
- The existing `tools/governance/sync_register.ps1` is a DOCUMENT control gate (REGISTER.yaml validation, frontmatter sync), NOT a build gate
- It does NOT need to know about the analyzer; the analyzer runs independently via `dotnet build`
- However, the cumulative closure protocol per K10.3 cascade pattern likely chains them: `dotnet build && dotnet test … && pwsh tools/governance/sync_register.ps1 -Validate` — that chain continues to work with analyzer enforcement in place because analyzer rule violations cause `dotnet build` to fail (after Stage 3), failing the chain at the right step

### §8.99 — Open questions surfaced (for §11 Q-K candidates)

- **Q-K-§8-1**: Analyzer csproj TargetFramework override mechanism
  - Context: `Directory.Build.props` line 3 sets global `<TargetFramework>net8.0</TargetFramework>`. Roslyn analyzer projects MUST target `netstandard2.0` (the version Roslyn's compiler-host can load). The analyzer csproj must override the inherited TFM. How is this overridden cleanly without breaking the centralized convention?
  - Options:
    - (a) Analyzer csproj explicitly sets `<TargetFramework>netstandard2.0</TargetFramework>` (overrides inherited)
    - (b) Directory.Build.props adds `Condition="'$(MSBuildProjectName)' != 'DualFrontier.Analyzers'"` to the TFM line (parallel exclusion pattern, makes Directory.Build.props messier)
    - (c) Analyzer csproj sits OUTSIDE the Directory.Build.props reach — e.g., its own `tools/DualFrontier.Analyzers/Directory.Build.props` overriding root (most idiomatic per Microsoft docs)
  - Recommendation: **Option (a)** — analyzer csproj sets `<TargetFramework>netstandard2.0</TargetFramework>` explicitly; MSBuild's inheritance semantics allow child overrides cleanly. Simplest, no Directory.Build.props churn. The analyzer test csproj remains net8.0 (uses Microsoft.CodeAnalysis.Testing which is net8.0-friendly).

- **Q-K-§8-2**: Whether to suppress CS1591 inheritance for analyzer csproj
  - Context: `Directory.Build.props` line 11 globally suppresses CS1591 (missing XML doc). Roslyn analyzer rule classes conventionally have rich XML docs for tooling extraction (severity, description, help link). Should the analyzer csproj un-suppress CS1591 to enforce its own documentation standards?
  - Options:
    - (a) Inherit suppression (CS1591 stays suppressed in analyzer csproj — analyzer rule classes can have or omit XML docs at author discretion)
    - (b) Analyzer csproj sets `<NoWarn></NoWarn>` (re-enables CS1591) or `<NoWarn>$(NoWarn.Replace('CS1591', ''))</NoWarn>` to enforce XML docs on all analyzer types
  - Recommendation: **Option (a)** for A'.9.1 (don't add scope); ANALYZER_RULES.md (per §8.3 reference) becomes the canonical rule documentation, not C# XML comments. Revisit at A'.10 if scaling analyzer rule count motivates per-rule XML-doc-driven help URLs.

- **Q-K-§8-3**: Should mods/DualFrontier.Mod.Example carry analyzer enforcement?
  - Context: `sln` line 40 + NestedProjects line 439. The example mod is shipping demonstration code; К-Lxx invariants (especially К-L9 vanilla=mods parity per DF009) apply, but mod authors generally won't have the analyzer wired into their own mod projects. Should the example demonstrate the analyzer integration pattern, or stay analyzer-free?
  - Options:
    - (a) Include mod in centralized analyzer reference (recommended in §8.2 — `mods/` is NOT excluded). Forces exemplary compliance, demonstrates pattern.
    - (b) Exclude `mods/**` from analyzer reference (treat mods as third-party). Loses К-L9 enforcement on the example.
  - Recommendation: **Option (a)** — the example mod should be exemplary; К-L9 explicitly says vanilla = mods, so the analyzer should treat mod code identically. This forces the example to demonstrate compliance, which is exactly the right signal for downstream mod authors.

- **Q-K-§8-4**: Cleanup-phase suppression rationale grammar/convention
  - Context: brief A9.C line 90 says "(b) suppressed with rationale [#pragma warning disable + comment]". What is the rationale format convention? Free-text comment, structured tag (e.g., `// DF011-SUPPRESS: legacy K8.2 staging — see migration ticket M-12`), or YAML frontmatter in a sidecar suppressions file?
  - Options: free-text vs structured tag vs sidecar GlobalSuppressions.cs vs sidecar .editorconfig suppression file
  - Recommendation: structured single-line tag `// DFNNN-SUPPRESS: <citation-or-rationale>` — greppable, links to the rule, encourages explicit rationale; combined with `#pragma warning disable DFNNN` line above + `#pragma warning restore DFNNN` line after for scoped suppression. Avoid sidecar GlobalSuppressions.cs (hides locality).

- **Q-K-§8-5**: Closure protocol §12.7 inclusion of analyzer test project
  - Context: closure protocol mandates `dotnet build` + targeted `dotnet test` per cascade. Should the analyzer test project (`tests/DualFrontier.Analyzers.Tests`) be added to the canonical "always-run" closure test set, or only when the analyzer or К-Lxx invariants are touched?
  - Recommendation: **always-run** post-A'.9.1 (the analyzer test suite is fast — Microsoft.CodeAnalysis.Testing is in-memory compilation — and surfaces regressions on EVERY К-Lxx-adjacent change, which is most architectural change). Per current `tests/DualFrontier.*.Tests` glob convention, no protocol change needed if Option C placement is adopted.

- **Q-K-§8-6**: GitHub Actions adoption decision deferral target
  - Context: §8.4 discussion #5 — no CI today; analyzer works identically in local + CI; question of when to introduce `.github/workflows/build.yml` is project-governance, not analyzer-scoped
  - Recommendation: defer to §11 Q-K consideration outside A'.9 scope; flag as "becomes relevant when collaboration model changes from solo-dev to multi-contributor". A'.9.1 should NOT introduce CI infrastructure as side-effect.

---

## §9 — Suppression governance precedent + recommendations

### §9.0 — Reconnaissance methodology

**Source files read** (full or targeted-range, per S-LOCK-7 honest depth):
- `src/DualFrontier.Core.Interop/NativeWorld.cs` lines 500–531 — context surrounding the CS0618 pragma at line 526 (`ResolveTypeId<T>` legacy fallback branch).
- `src/DualFrontier.Core.Interop/Marshalling/NativeComponentType.cs` lines 21–58 — both `[Obsolete]` types being suppressed (`NativeComponentType<T>` line 23; `NativeComponentTypeRegistry` line 53), both intentionally `error: false`.
- `tests/DualFrontier.Core.Interop.Tests/BootstrapTests.cs` (full, 98 lines) — CS0649 fixture pattern reference.
- `tests/DualFrontier.Core.Interop.Tests/ComponentTypeRegistryTests.cs` (full, 133 lines) — placeholder-types fixture pattern with explicit justification comment.
- `tests/DualFrontier.Core.Tests/Bus/ManagedBusBridgeTests.cs` (full, 373 lines) — narrowest-scope (single-field) CS0649 pragma pattern.
- `docs/governance/REGISTER.yaml` — `capa_entries:` collection (lines 5776–6573) inventoried in full; CAPA cross-reference patterns audited.
- `docs/methodology/METHODOLOGY.md` — Lesson #25 (line 973: PROMOTED → FORMALIZED at A'.8; A'.7.5 third application); refined statement traced to METHODOLOGY v1.11 changelog and К_EXT_2 brief lines 1257–1259.

**What was inventoried**: 5 pragma occurrences (1 src + 4 tests) confirmed against Phase 0; CAPA collection categorized for any «we suppressed a warning» precedent; «test-lying surface» principle traced to its current methodology and code-comment footprints.

---

### §9.1 — Existing suppression patterns inventory

**Summary table** (Phase 0 facts confirmed + classified):

| Location | Code | Pragma vs Attribute | Justification comment (verbatim) | Classification |
|---|---|---|---|---|
| `src/DualFrontier.Core.Interop/NativeWorld.cs:526` | CS0618 | pragma (3-line scope: 526–530) | «NativeComponentType<T> is obsolete (legacy fallback path).» | **Legitimate transitional** |
| `tests/DualFrontier.Core.Interop.Tests/BootstrapTests.cs:12` | CS0649 | pragma (6-line scope: 12–17, around `TestComponent`) | none inline | **Fixture pattern** |
| `tests/DualFrontier.Core.Interop.Tests/ComponentTypeRegistryTests.cs:13` | CS0649 | pragma (5-line scope: 13–17, around `TypeA/B/C`) | «Placeholder types exercise registration semantics by identity only — the fields are not read in any test.» | **Fixture pattern (best-in-class — explicit justification)** |
| `tests/DualFrontier.Core.Tests/Bus/ManagedBusBridgeTests.cs:32` | CS0649 | pragma (4-line scope: 32–34, around `NormalTestEvent.Value`) | none inline | **Fixture pattern (narrowest scope — single field)** |
| `tests/DualFrontier.Core.Tests/Bus/ManagedBusBridgeTests.cs:45` | CS0649 | pragma (4-line scope: 45–47, around `DefaultTierTestEvent.Value`) | none inline | **Fixture pattern (narrowest scope — single field)** |

**Aggregate statistics**:
- **5 total suppressions** across the entire solution (1 src + 4 tests).
- **0 `[SuppressMessage]` attributes** anywhere.
- **0 `GlobalSuppressions.cs` files** anywhere.
- **2 warning codes** in scope: CS0618 (1× — obsolete usage) and CS0649 (4× — unread fields).
- **Classification breakdown**: 1 legitimate transitional + 4 fixture-pattern + 0 questionable.
- **Justification-comment coverage**: 2 of 5 (40%) have inline justifications; the src-side CS0618 and the most architecturally-self-explanatory test (`ComponentTypeRegistryTests`) carry comments. The three undocumented test pragmas are around obviously-blittable test event/component structs where the «reflection-based access» nature is contextually obvious to a reader.

**Pragma scope discipline**: All 5 suppressions use **disable/restore pair brackets** wrapping the smallest possible declaration unit (a method body, a struct, or even a single field as in `ManagedBusBridgeTests:32` and `:45`). No file-wide `#pragma warning disable` without `restore`. No suppressions span > 6 source lines. This is **disciplined narrow-scope** usage in 5/5 cases.

**Deep dive: the src-side CS0618 (`NativeWorld.cs:526`)**:
- Wraps the `ResolveTypeId<T>` legacy-fallback branch executed when `_registry == null` (i.e., post-K8 cutover the `_registry`-bound path on line 524 will become unconditional and the entire suppressed block disappears).
- The two types being suppressed (`NativeComponentType<T>` at `Marshalling/NativeComponentType.cs:23` and `NativeComponentTypeRegistry` at line 53) are tagged `[Obsolete(..., error: false)]` **by design** — the deprecation is warning-class precisely so the well-contained legacy fallback can compile cleanly with local pragma suppression. The pragma is not hiding a bug; it is the **legitimate sanctioned form** of «I am the one caller that knows it is OK to still use this».
- **Removal trigger**: the second `[Obsolete]` attribute (line 50) explicitly states «NativeComponentTypeRegistry will be removed when NativeComponentType<T> is removed (K8 cutover)». The CS0618 suppression is **causally linked to a removable transitional state** — exactly the «transitional» classification's defining condition.

**Deep dive: the 4 CS0649 fixtures**:
- All four wrap private struct field declarations in test classes. CS0649 fires because xUnit + the `df_native` interop layer access these fields **by-value through marshalling or by-identity** — not via property/method reads the compiler can see. The struct value gets boxed through `AddComponent<T>(...)` / `Publish<T>(...)`, copied across the managed/native boundary, and round-tripped back; the field is **read via Unsafe.SizeOf<T>() and pointer arithmetic in native code**, which CS0649's reachability analysis cannot follow.
- The `ComponentTypeRegistryTests` instance is exemplary — its comment «exercises registration semantics by identity only — the fields are not read in any test» tells the reader: *the field exists to give the struct a non-zero size for component-id assignment; it is structurally required even though no test reads it.*
- The two `ManagedBusBridgeTests` instances are the narrowest possible scope: a 3-line bracket around a single field. This is **the gold-standard pattern** for CS0649 in event-payload test fixtures.

**Why `[SuppressMessage]` and `GlobalSuppressions.cs` are absent**: there is currently **no codebase-internal analyzer** producing diagnostics that would warrant the attribute pattern; all suppressions target **compiler-built-in** warnings (CS-prefix), and the `#pragma` form is the canonical way to suppress those. The attribute form becomes the natural carrier once DF### analyzer rules ship (A'.9.1+).

**CAPA entries related to suppression (from REGISTER.yaml `capa_entries:` collection, lines 5776–6573)**:

Full inventory of the 14 CAPA entries currently in the register:
1. `CAPA-2026-05-09-K8.2-V2-REFRAMING` — K-L3 framing reformulation
2. `CAPA-2026-05-10-A_PRIME_0_7-AUDIENCE-INVERSION` — methodology audience contract
3. `CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT` — inventory point-in-time drift
4. `CAPA-2026-05-13-K8.3-PREMISE-MISS` — brief premise miss
5. `CAPA-2026-05-14-K8.34-API-SURFACE-MISS` — API transcription (Lesson #7 origin)
6. `CAPA-2026-05-14-K8.34-MID-TRANSITION-DRIFT` — mid-transition framing
7. `CAPA-2026-05-16-ISOLATION-AUTHORITY-RESTORATION` — isolation rule restoration
8. `CAPA-2026-05-16-LIVE-STATE-CLOSURE-PROTOCOL-GAP` — closure protocol
9. `CAPA-2026-05-16-POWER-DELETION-PROPAGATION` — power-subsystem deletion follow-through
10. `CAPA-2026-05-16-MOD-API-V3-AUTHORITY` — mod API authority restoration
11. `CAPA-2026-05-16-V-SUBSTRATE-SUPERSESSION` — V substrate supersession sweep
12. `CAPA-2026-05-18-K8_5-DRIFT` — K8.5 classification drift
13. `CAPA-2026-05-21-A_PRIME_7_X-BUS-COALESCE-KEY-LOST` / `-DISPATCH-ORPHAN` / `-COALESCE-ONSQUARED` / `-K10_3-V2-SOFT-HALT` / `-STRESS-CROSS-TEST-POLLUTION` — A'.7.x bus bugs cluster (5 entries)

**Result: 0 CAPA entries currently relate to suppression governance.** All 14 existing CAPAs track *brief premise / methodology framing / cross-document drift / cross-test pollution / bug regressions*, not «we suppressed a warning rather than fixing the root cause». This is consistent with the **near-zero suppression baseline** (5 occurrences, all classified legitimate) — there has been nothing to track.

**Relationship to Lesson #25 refined** (cascade #2 closure 2026-05-23, METHODOLOGY v1.11):

Verbatim refined statement (per К_EXT_2 brief lines 1257–1259 + `IRenderCommand.cs` doc-comment lines 9–10):

> «Design abstractions when consumer materializes AND structurally eliminate test-lying surface — empty stub implementations что pass tests by doing nothing constitute architectural debt independent of speculation discipline.»

**Pragma suppressions are an analogous «hide-rather-than-fix» surface, but a categorically different sub-type**:

| Dimension | Lesson #25 «test-lying surface» | Pragma suppression |
|---|---|---|
| Hides what | Behavioral defect (empty `Execute()` body passes tests by doing nothing) | Diagnostic signal (compiler / analyzer warning) |
| Detection | Tests pass, defect ships silently | Build produces no warning, defect/anti-pattern accumulates silently |
| Cure (cascade #2 pattern) | **Structural elimination** — `IRenderCommand` reduced to marker, no `Execute()` to be empty | **Structural elimination** — fix the root cause OR remove the rule OR add `[AllowXxx]` carve-out attribute |
| Cure (escape-valve) | none — empty bodies are always wrong | suppression *is* the escape valve, but must be CAPA-tracked |
| Current codebase footprint | 0 (eliminated cascade #2) | 5 (all legitimate) |

**The parallel is exact and worth codifying**: just as cascade #2 eliminated `IRenderCommand.Execute()` empty bodies by reshaping the abstraction (marker-only interface), the A'.9 analyzer milestone should treat «I'm suppressing this rule everywhere because it doesn't match my code shape» as a signal to **reshape the rule** (refine the analyzer, or add an architectural carve-out attribute), not as a signal to accept ambient suppression. The «structurally eliminate suppressed-warning surface» rephrasing of Lesson #25 is the proposed §9.2.5 codification.

---

### §9.2 — Suppression governance recommendations

#### §9.2.1 — When suppression is allowed

Five sanctioned categories, in descending order of legitimacy:

**(a) Transitional suppressions — LEGITIMATE.** A symbol is `[Obsolete(..., error: false)]` because exactly one well-bounded legacy caller still needs it, and the removal trigger is identified. Reference exemplar: `NativeWorld.cs:526` (removable at K8 cutover when `_registry == null` branch disappears). Requirements:
- The obsolete symbol carries a non-trivial `[Obsolete]` message with a documented replacement.
- The pragma scope is the minimal callsite (≤ 1 method body).
- A removal trigger is described in code comment OR in a CAPA entry.

**(b) Fixture suppressions — LEGITIMATE.** Test-side suppressions for fields that are structurally required but compiler-invisibly-read (reflection, P/Invoke marshalling, `Unsafe.SizeOf<T>()` consumers, blittable struct round-trips). Reference exemplars: all 4 existing CS0649 occurrences. Requirements:
- Pragma scope is the minimum struct or field declaration.
- One-line justification comment if the «invisibly-read» mechanism is not obvious from surrounding test code (the `ComponentTypeRegistryTests` comment is exemplary; bare struct-field pragmas in obvious payload contexts may omit it).
- **Restricted to `tests/` tree** — fixture suppression is not a valid pattern in `src/`.

**(c) Diagnostic false-positives — LEGITIMATE BUT FLAG FOR RULE REFINEMENT.** A DF### analyzer rule fires on a callsite that semantically does not violate the rule. The pragma is an escape valve, but **the preferred fix is rule refinement** — the suppression should open a CAPA whose corrective action is «refine analyzer rule to recognize this shape». If 2+ false-positives accumulate against the same rule, the analyzer rule itself is in CAPA.

**(d) Documented architectural exceptions — LEGITIMATE.** Rule-specific opt-out attributes (e.g., `[AllowStaticState]` for DC008 per cascade #3 Domain 3 recommendation). The attribute form is **structurally preferred over `#pragma`** for this case because it travels with the type/member and survives refactoring. Requirements:
- The opt-out attribute must be defined as part of the analyzer ruleset's contract documents.
- The carve-out usage must carry a `Justification = "..."` string (matching `[SuppressMessage]` field convention).
- Carve-out attribute applications are inventoried per release cascade closure (see §9.2.4).

**(e) NOT ALLOWED — recognize and reject the following patterns**:
- **«I don't want to fix this rn» suppressions** — no removal trigger, no CAPA, no architectural carve-out.
- **Cosmetic suppressions** — suppressing a style/format warning that the analyzer is correctly reporting.
- **Justification-free suppressions in `src/`** — `src/` pragmas without an inline comment OR a corresponding CAPA fail review. (Carve-out for the existing CS0618: the comment is present and traceable to the K8 cutover.)
- **File-wide / project-wide blanket suppressions** — no `<NoWarn>` MSBuild property additions, no top-of-file `#pragma warning disable` without matching `restore`.
- **Cross-cutting `GlobalSuppressions.cs` files** — discussed in §9.2.2, recommended ban.

#### §9.2.2 — Suppression syntax conventions

**Recommendation: dual-pattern with category-driven choice.**

| Pattern | Use for | Status |
|---|---|---|
| `#pragma warning disable XX### // <justification>` ... `#pragma warning restore XX###` | (a) Transitional, (b) Fixture, (c) Diagnostic false-positive | **Adopted** — matches existing 5-occurrence codebase pattern. Maintain minimum-scope bracket discipline (≤ 6 lines, single declaration unit). |
| `[SuppressMessage("DualFrontier.Analyzer", "DF###", Justification = "...")]` | (c) Diagnostic false-positive when scope is a whole symbol (type, method, property) | **Introduced at A'.9.1** — first use of attribute form. Mandatory `Justification = "..."` parameter; no `MessageId`/`Scope`/`Target` parameter sprawl. |
| `[AllowXxx]` carve-out attributes (e.g., `[AllowStaticState]`) | (d) Documented architectural exception | **Introduced at A'.9.1** alongside the analyzer rules that respect them. Carve-out attributes are **part of the rule contract**, not generic suppressions. |

**Stance on `GlobalSuppressions.cs`: BAN.**

Rationale:
- Currently 0 such files exist — establishing the ban is a near-zero-cost greenfield decision.
- `GlobalSuppressions.cs` decouples the suppression from its site, defeating the «pragma stays adjacent to the suppressed expression» discipline that makes the existing 5 pragmas auditable.
- It enables the «I don't want to fix this rn» anti-pattern at scale.
- The legitimate use case (suppressing one rule across an entire assembly because it is conceptually inapplicable) is better served by **rule scope configuration in the `.editorconfig` / analyzer ruleset file**, where it is governance-visible.

**Concrete enforcement mechanism (proposed for A'.9.X)**: an internal analyzer rule `DF999` (placeholder) that flags any file matching `GlobalSuppressions.cs` or any `[assembly: SuppressMessage(...)]` attribute. This self-policing analyzer makes the ban falsifiable per K-L14 criterion 1.

#### §9.2.3 — CAPA tracking requirement

**Recommendation: tiered tracking based on suppression category.**

| Category | CAPA mandatory? | Inline justification mandatory? | Notes |
|---|---|---|---|
| (a) Transitional | Optional if removal trigger is in code comment; **mandatory** if removal trigger is cross-cascade (e.g., «K8 cutover») | **Mandatory** | Existing CS0618 at `NativeWorld.cs:526` satisfies «removal trigger in code comment» — no CAPA required at this scale; if a 2nd transitional appears with the same trigger, open a CAPA at that point to track convergence. |
| (b) Fixture | **Not required** for any single instance | Optional (recommended for non-obvious mechanisms) | Test-side fixture pragmas are a known idiom; CAPA-tracking each one would be ceremony without signal. |
| (c) Diagnostic false-positive | **Mandatory** | **Mandatory** | The CAPA's corrective action is rule refinement; tracking the false-positive census per rule is the falsifiability mechanism for «is this analyzer rule well-shaped?». |
| (d) Architectural exception (carve-out attribute) | **Mandatory at first use of each carve-out site**, then cumulative inventory per cascade closure | **Mandatory** (the `Justification = "..."` field) | First-use CAPA documents the carve-out's legitimacy; subsequent uses are inventoried per §9.2.4 review, not CAPA-by-CAPA. |
| (e) NOT ALLOWED | N/A — rejected at review | N/A | Build fails OR PR rejected. |

**Recommended CAPA field schema for suppression CAPAs** (modeling existing convention from REGISTER.yaml lines 5776–6573):

```yaml
- id: CAPA-YYYY-MM-DD-SUPPRESS-<RULE_ID>-<SCOPE_TAG>
  opened_date: "YYYY-MM-DD"
  closure_status: OPEN | CLOSED
  closed_date: "YYYY-MM-DD"   # when closed
  closed_commit: "<sha>"
  closure_outcome: |
    <description of how the suppression was resolved — fix landed,
    rule refined, removal trigger fired, or carve-out attribute defined>
  trigger: |
    <file:line + rule_id that necessitated suppression + why the suppression
    is preferred over the immediate fix at this cascade>
  affected_documents:
    - <governance / brief / methodology references>
  root_cause: |
    <why the warning fires; what shape of code triggered it>
  immediate_action: |
    <which pragma / attribute was placed and where>
  corrective_action: |
    <removal plan — rule refinement, carve-out attribute introduction,
    legacy code removal, etc.>
  preventive_action: |
    <optional — Lesson candidate if pattern generalizes>
  effectiveness_verification:
    method: "<how to verify the suppression no longer exists OR is no longer needed>"
    date_verified: null
    verification_commit: null
    verification_outcome: null
    verification_pending: "<which cascade closure verifies>"
  lessons_learned_reference: <DOC-ID>
```

**Connection to existing convention**: this is structurally identical to the 14 existing CAPA entries — same field set, same `trigger / root_cause / immediate_action / corrective_action / effectiveness_verification` skeleton. The only suppression-specific field is the `id` prefix `CAPA-YYYY-MM-DD-SUPPRESS-<RULE_ID>-<SCOPE_TAG>` which makes suppression CAPAs grep-discoverable.

#### §9.2.4 — Review cadence

**Recommendation: dual-cadence (per-closure + quarterly).**

**Per-closure suppression sweep** (every cascade γ closure, integrated as a γ verification step):
- A `grep -rE '#pragma warning disable|\[SuppressMessage|\[Allow' src/ tests/` enumeration.
- Cross-reference against open suppression CAPAs.
- Verify: each `src/` suppression has either (i) an inline justification comment OR (ii) an open CAPA. Test-side fixture suppressions exempt from CAPA requirement per §9.2.3.
- New suppressions added in the cascade must each have CAPA-tracking decision made (open CAPA OR document why CAPA is not required per §9.2.3 tier).
- **Output**: one line in the cascade closure section: «Suppression sweep: N total (M src, K tests), 0 unjustified, J open CAPAs» — analogous to existing closure metrics like «Modding suite (filter Category!=Stress): 395/395 PASS».

**Quarterly cumulative review** (cascade-aligned, ~ every 3 K-extensions cascades or ~ every major Phase milestone):
- Full audit of suppression census evolution: did totals grow? did any rule accumulate ≥2 false-positives (rule refinement trigger)?
- Verify each open suppression CAPA's `corrective_action` remains realistic and time-bounded.
- Verify removal triggers for transitional suppressions remain accurate (e.g., the K8 cutover still planned for the same milestone).
- **Output**: an entry in the relevant Phase milestone closure or K-closure report, structurally analogous to К-L14 Evidence Dashboard verifications.

**Why both cadences**: per-closure catches new ambient debt before it crystallizes; quarterly catches accumulation patterns the per-closure view cannot see. The dual cadence parallels the existing `effectiveness_verification` discipline in REGISTER CAPAs («CAPA opened+closed within same governance commit per A'.4.5 precedent» for tight loops; multi-milestone tracking for cross-cascade convergence).

#### §9.2.5 — Connection к Lesson #25 refined

**Proposed codification (candidate refinement for METHODOLOGY v1.13+)**:

> «Design abstractions when consumer materializes AND structurally eliminate test-lying surface AND structurally eliminate suppressed-warning surface — empty stub implementations что pass tests by doing nothing, AND ambient pragma suppressions что hide diagnostic signals without removal trigger or carve-out justification, both constitute architectural debt independent of speculation discipline.»

**Mechanistic parallel** (the proposed §9.2.5 contribution):

| Lesson #25 cascade #2 application | §9 cascade A'.9.X application |
|---|---|
| `IRenderCommand.Execute()` empty bodies (`PresentationBridge` legacy pattern) | Future ambient `#pragma warning disable DF###` lacking justification / CAPA |
| **Structural fix**: reduce `IRenderCommand` to marker-only interface; no `Execute()` to be empty | **Structural fix**: refine the DF### rule OR introduce a carve-out attribute (e.g., `[AllowStaticState]`) OR fix the root cause |
| **Falsifiability**: grep for `class.*: IRenderCommand` + check for `Execute()` member presence | **Falsifiability**: per-closure suppression sweep (§9.2.4) flags pragma without inline justification or open CAPA |
| **Outcome**: 0 lying-test surfaces post-cascade #2 | **Outcome**: every suppression at end of cascade is either (i) transitional with removal trigger, (ii) fixture (test-side only), (iii) CAPA-tracked false-positive, or (iv) sanctioned carve-out attribute |

**Why this parallel is worth formalizing** (not merely analogizing):

Both surfaces share a defining structural property — **the absence of a signal where a signal should exist**. A test that passes because the implementation does nothing is information-isomorphic to a build that succeeds because a diagnostic was suppressed: both produce a green light that does not correspond to verified correctness. Cascade #2 established the methodology pattern «when the green light is unearned, fix the structure that makes the light unearned» (the marker-only interface). §9 generalizes the pattern to the analyzer ecosystem: when a pragma earns the green light unjustifiedly, the structural fix is rule refinement, carve-out, or removal — not perpetual suppression.

**Lesson #25 carries the K-L14 falsifiability discipline (Criterion 6 — structurally non-falsifiable green is the worst architectural debt). §9 inherits that discipline applied to the analyzer surface.**

---

### §9.99 — Open questions surfaced (for §11 Q-K candidates)

- **Q-K-§9-1**: Should the proposed `DF999` self-policing analyzer rule (ban `GlobalSuppressions.cs` and `[assembly: SuppressMessage(...)]`) ship in A'.9.1 alongside the first DF### rules, or be deferred until at least one DF### rule has a non-trivial false-positive census (i.e., until the ban has empirically been needed)?
  - Context: shipping the ban with the first analyzer release is greenfield-clean (0 existing assembly-level suppressions to grandfather); shipping later risks an ambient `GlobalSuppressions.cs` appearing during A'.9.X iteration and needing retroactive cleanup.
  - Options:
    (a) Ship `DF999` in A'.9.1 with the first DF### rules
    (b) Defer to A'.9.2 once empirical evidence justifies it
    (c) Ship as a `.editorconfig`-only policy (not a coded analyzer) in A'.9.1, promote to coded `DF999` if violated
  - Recommendation: **(a)** — the cost of shipping a near-trivial banning rule alongside the first ruleset is low; the cost of retroactively cleaning ambient `GlobalSuppressions.cs` is high. Establish the discipline at greenfield.

- **Q-K-§9-2**: Should the `[SuppressMessage]` attribute form be allowed at all in A'.9.1, or restricted to A'.9.X cascades only after pragma-form usage has settled?
  - Context: codebase currently has **zero** `[SuppressMessage]` attributes; introducing it as a sanctioned form simultaneously with the first DF### rules creates two parallel suppression syntaxes from day 1.
  - Options:
    (a) Sanction both `#pragma` (line-scope) and `[SuppressMessage]` (symbol-scope) at A'.9.1
    (b) A'.9.1 ships pragma-only; `[SuppressMessage]` introduced at first false-positive that genuinely needs symbol-scope
    (c) A'.9.1 ships `[SuppressMessage]`-only for DF### rules (reserve `#pragma` for CS-prefix compiler warnings only)
  - Recommendation: **(b)** — match the codebase's empirical pattern (5/5 existing suppressions are pragma form) and introduce attribute form only when a real false-positive case demands symbol-scope. Avoids syntax sprawl until justified.

- **Q-K-§9-3**: Should the proposed CAPA-tracking tier for «Diagnostic false-positives — mandatory CAPA» be cascade-scoped (one CAPA per suppression site) or rule-scoped (one CAPA per rule accumulating false-positives, with site list)?
  - Context: site-scoped CAPAs match existing REGISTER convention but could generate CAPA proliferation; rule-scoped CAPAs match the «fix the rule, not the suppression» preferred resolution path but break the «one CAPA = one issue» pattern.
  - Options:
    (a) Site-scoped (one CAPA per suppression site)
    (b) Rule-scoped (one CAPA per rule, with embedded site list updated as new sites accumulate)
    (c) Hybrid: site-scoped for first occurrence of a rule, rule-scoped thereafter (first CAPA establishes the rule census, subsequent sites append to its `affected_documents` list)
  - Recommendation: **(c)** — preserves the «one CAPA = one issue» pattern at first occurrence (defensible governance hygiene), then naturally transitions to rule-level tracking once a refinement pattern emerges. Matches the existing CAPA-2026-05-14-K8.34-API-SURFACE-MISS pattern that generalizes from a single observation to a class.

- **Q-K-§9-4**: Should the «test-side fixture suppression CAPA-exempt» policy (§9.2.3 tier (b)) extend to **every** test-only file, or be restricted to a defined set of test layers (e.g., the `tests/DualFrontier.Core.Tests/Fixtures/` tree or `*Tests.cs` files specifically)?
  - Context: cascade #2 + cascade #3 demonstrated that test-side discipline can drift (the «test-lying surface» Lesson #25 origin was test-side); blanket exemption for «anything in `tests/`» risks reproducing that drift in suppression form.
  - Options:
    (a) Blanket exempt all `tests/**`
    (b) Exempt only `*Tests.cs` (the xUnit-recognized test class files), require CAPA for fixtures-utility files
    (c) Exempt all `tests/**` but require quarterly review (§9.2.4) to spot pattern drift
  - Recommendation: **(c)** — blanket exemption matches the current 4/4 test pragma pattern (all in `*Tests.cs` directly), but quarterly review provides a falsifiability mechanism if test-side pragma usage grows in fixtures-utility or shared-helper layers where the «obvious idiom» justification weakens.

- **Q-K-§9-5**: For carve-out attributes like `[AllowStaticState]` (DC008 escape valve per cascade #3 Domain 3), should the carve-out attribute carry a **mandatory `Justification` string parameter** (matching the `[SuppressMessage("...", Justification = "...")]` convention), or is the attribute's presence alone sufficient signal?
  - Context: `[AllowStaticState]` without justification reduces to «I assert this is OK» without traceable rationale; with justification it carries forward the «every suppression has a defensible reason» discipline of pragma comments.
  - Options:
    (a) Justification mandatory (analyzer rule fails if `[AllowStaticState]` is applied without `Justification = "..."`)
    (b) Justification optional (attribute presence is the signal; rationale lives in adjacent code comments or commit messages)
    (c) Justification optional for cascade #3 Phase A'.9.1, mandatory at A'.9.2+ once the carve-out idiom is established
  - Recommendation: **(a)** — preserves the discipline established by the existing CS0618 inline comment («NativeComponentType<T> is obsolete (legacy fallback path).») in attribute form. The marginal authoring cost is one string parameter; the marginal review value is full traceability of every carve-out usage to its architectural justification.

---

## §10 — Brief A'.9.1 prerequisites

Per S-LOCK-8: explicit enumeration of decisions Brief A'.9.1 deliberation must ratify based on this report. Each prerequisite carries empirical anchor + recommendation (from sub-agent recon) + decision pointer.

### Prerequisite 1 — Rule prioritization batch для A'.9.1

**Empirical anchor** (§3.0 + §3.1):
- 9 К-L scored P0 critical: DF001, DF002, DF003, DF003.1, DF004, DF005, DF007, DF011, DF015
- 8 К-L scored P1 high: DF007.1, DF009, DF010, DF012, DF015.1, DF017, DF018, DF019
- 3 К-L scored P2 medium: DF013, DF016, DF020 (К-L20 deferred к Mod API lock cascade)
- 3 К-L scored P3 low / docs-only: DF006 (SUPERSEDED), DF008 (process-invariant — pre-commit hook alternative), DF014 (meta-invariant — K_L14_EVIDENCE_DASHBOARD alternative)

**Recommendation**: Ship all 9 P0 + 8 P1 = **17 rules at A'.9.1** (skipping DF006/DF008/DF014 reserved; deferring DF013/DF016/DF019 P2 к A'.9.2 OR shipping at suggestion-severity если cheap). DF020 family deferred к Mod API lock cascade per §6.3 timing.

**Decision**: Brief A'.9.1 deliberation Q-N ratifies (a) initial 17-rule batch (b) sequencing within batch (which rule first к minimize cascade complexity).

### Prerequisite 2 — Analyzer project structure

**Empirical anchor** (§8.1):
- A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md skeleton expected `tools/analyzers/DualFrontier.Analyzers/`
- ManifestRewriter precedent: flat `tools/DualFrontier.Mod.ManifestRewriter/` (tool csproj) + `tests/DualFrontier.Mod.ManifestRewriter.Tests/` (test csproj)
- Three options analyzed (src/, tools/analyzers/, hybrid)

**Recommendation**: **Option C hybrid** — `tools/DualFrontier.Analyzers/` (flat — match ManifestRewriter precedent verbatim; do NOT introduce `tools/analyzers/` subfolder для single analyzer) + `tests/DualFrontier.Analyzers.Tests/` (match `tests/DualFrontier.*.Tests/` glob without protocol change). NestedProjects entries: analyzer → tools Solution Folder; tests → tests Solution Folder.

**csproj config** (per §7.1.2 + §7.1.5):
- Analyzer csproj: `<TargetFramework>netstandard2.0</TargetFramework>` (override Directory.Build.props net8.0), `<IsRoslynComponent>true</IsRoslynComponent>`, `<EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>`, `<IsPackable>false</IsPackable>`
- Test csproj: stays net8.0 (Microsoft.CodeAnalysis.Testing supports net8.0)

**Decision**: Brief A'.9.1 deliberation Q-N ratifies location + csproj baseline. Q-K-§8-1 (TFM override mechanism) + Q-K-§8-2 (CS1591 inheritance) addressed here.

### Prerequisite 3 — Test framework choice

**Empirical anchor** (§7.2):
- `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` 1.1.2 (2024-06-19) — soft-maintenance но functional + 2.9M downloads (largest community precedent)
- Base package `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing` 1.1.4 (2026-05-22) — actively maintained generic; framework-specific variants frozen at 1.1.2
- Critical version coupling: testing project MUST explicitly pin `Microsoft.CodeAnalysis.CSharp.Workspaces 5.3.0` к override 1.0.1 transitive (else MEF composition fails)

**Recommendation**: **xUnit framework variant** (matches Dual Frontier xUnit baseline + soft-maintenance acceptable + smallest-LOC option). Pin `Microsoft.CodeAnalysis.CSharp.Workspaces 5.3.0` explicitly в test csproj.

**Decision**: Brief A'.9.1 deliberation Q-N ratifies test framework + version-pinning discipline.

### Prerequisite 4 — Severity policy

**Empirical anchor** (§7.3 + §8.3):
- ANALYZER_RULES.md §4 assigns 15 Error + 4 Warning across 19 active rules; consistent с dotnet/roslyn-analyzers + dotnet/aspnetcore precedents
- `TreatWarningsAsErrors=true` already in Directory.Build.props line 7 — Warning-severity rules effectively become build-error rules

**Recommendation**: **per-rule severity exclusively via .editorconfig** (no Directory.Build.props additions; preserves single-file diff for severity changes). Staged progression: cleanup phase = `suggestion` (visible, non-fatal); promotion = `error` (skip `warning` tier to avoid TWAE confusion). DF### category convention: per-К-L-tier categories (`DualFrontier.Architecture`, `DualFrontier.NativeBoundary`, `DualFrontier.ModSurface`) supporting targeted overrides per Q-K-§7-5.

**Open severity refinements** (Q-K candidates):
- Q-K-§7-1: DF009 Error → Warning until К-L20 LOCK (mod parity volatile pre-LOCK)
- Q-K-§3-4: DF019 split — Error для API-shape sub-detector / Warning for hardware-tier
- Q-K-§3-3: К-L15.1 Roslyn scope — managed-side facade only (Layer 3 native-side deferred)

**Decision**: Brief A'.9.1 deliberation Q-N ratifies (a) per-rule severity assignments (b) category convention (single vs tiered) (c) cleanup-phase severity baseline.

### Prerequisite 5 — Suppression policy

**Empirical anchor** (§9.1 + §9.2):
- Current baseline: 5 pragmas (1 src CS0618 transitional + 4 tests CS0649 fixture); 0 [SuppressMessage]; 0 GlobalSuppressions.cs; 0 CAPA related
- Proposed: 5-tier classification (transitional / fixture / false-positive / carve-out / NOT-allowed); BAN GlobalSuppressions.cs; tiered CAPA tracking

**Recommendation**: Adopt §9.2 governance protocol verbatim. Ship `DF999` self-policing rule (ban GlobalSuppressions.cs + [assembly: SuppressMessage]) at A'.9.1 alongside first DF### rules per Q-K-§9-1 recommendation (a). [SuppressMessage] attribute form deferred к first false-positive demanding symbol-scope per Q-K-§9-2 recommendation (b). Per-closure suppression sweep + quarterly review cadence integrated to closure protocol §12.7.

**Lesson #25 refined extension**: «structurally eliminate suppressed-warning surface» codification as METHODOLOGY v1.13+ candidate (per §9.2.5).

**Decision**: Brief A'.9.1 deliberation Q-N ratifies (a) tiered classification + CAPA matrix (b) DF999 self-policing rule scope (c) METHODOLOGY v1.13+ Lesson #25 extension proposal.

### Prerequisite 6 — Build/CI integration trigger

**Empirical anchor** (§8.4):
- No `.github/workflows/*.yml` — no GitHub Actions CI today; local-only `dotnet build` verification
- Roslyn analyzers invoked automatically by `dotnet build` once `<ProjectReference OutputItemType="Analyzer">` is added — no separate CI trigger

**Recommendation**: **Centralized analyzer reference in Directory.Build.props** with `MSBuildProjectName` conditional excluding analyzer-self + tests + Fixture.* projects. `PrivateAssets="all"` prevents transitive leak. Analyzer runs automatically on every `dotnet build`. Staged A'.9.1 progression: ξ scaffolding → χ cleanup phase → ψ severity promotion (per §8.4 Stages 1-3).

**Mod inclusion**: `mods/DualFrontier.Mod.Example` INCLUDED в analyzer enforcement per Q-K-§8-3 recommendation (a) — exemplary compliance demonstrates pattern.

**Decision**: Brief A'.9.1 deliberation Q-N ratifies (a) centralized vs per-project reference choice (b) exclusion list scope (c) stage progression timing (single A'.9.1 cascade vs sub-cascades ξ/χ/ψ).

### Prerequisite 7 — A'.9 cascade decomposition refinement

**Empirical anchor** (§1.3 + §8.4):
- Brief A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md skeleton had A9.A/B/C/D/E sub-milestones
- §8.4 recommends 3-stage ξ/χ/ψ within A'.9.1
- Domain 4 §6.3 identifies DF020 family as post-A'.9 К-L20 LOCK cascade

**Recommendation**:
- **A'.9.1**: analyzer infrastructure (Stage ξ) + first 17 DF### rules + cleanup phase (Stage χ) + `.editorconfig` baseline + `DF999` self-policing
- **A'.9.2**: severity promotion (Stage ψ — `.editorconfig` edit) + optional code-fix providers for Trivial-feasibility rules (DF004 type registration auto-gen; possibly DF002, DF011 within tests)
- **A'.9.3+**: DC### cascade-derived rules (per §5 — 10 candidates: IRenderCommand marker, defensive stub pattern с `[ReservedStub]` attribute infrastructure, banned-namespace via BannedApiAnalyzer, etc.); DL### Lesson-derived auxiliary tooling (DL008 atomic-commit hook); M3.4 manifest cross-check analyzer (per Domain 4)
- **Post-A'.9 / pre-К-L20 LOCK**: V-extension (per Crystalka «расширять V»)
- **К-L20 LOCK cascade**: DF020 family (20 sub-rules per §6.2 — namespace/type + API usage + manifest cross-check + grace period); analyzer enables Mod API enforcement automation

**Decision**: Brief A'.9.1 deliberation Q-N ratifies decomposition. Q-K-§6-3 (M3.4 placement decision: A'.9.X vs К-L20 LOCK) addressed here.

### Prerequisite 8 — К-L20 Mod API lock timing forward path

**Empirical anchor** (§6.1 + §6.3.2):
- К-L20 canonical text resides в K_CLOSURE_REPORT.md §2.23 (lines 774-795); KERNEL Part 0 table ends at К-L19 per Q-N-8-1 reservation discipline
- DF020 reserved post-Mod API lock per K_CLOSURE §7.3 line 1689 + ANALYZER_RULES.md §1
- K_CLOSURE §9.5 enumerates Q1-Q8 Mod API lock milestone surface (lines 1908-1929)

**Recommendation**:
- Forward sequence: A'.9 milestone (analyzer infrastructure + DF001-DF019) → V-extension → **К-L20 LOCK cascade** (DF020 activation as verification step)
- A'.9-era preparatory work (per §6.3.1): 6 precursor relationships A'.9-era → DF020 (DF003.1→DF020.3, DF009→DF020.{1,2,8,9}, DF012→DF020.8, DF015→DF020.{9,10,11}, DF018→К-L18+К-L20 interaction, M3.4→DF020.{10,11,16})
- A'.9 closure outputs Mod API surface baseline snapshot (per §6.3.3) for K-L20 LOCK cascade reference state

**Decision**: Brief A'.9.1 deliberation Q-N ratifies (a) acknowledgment that К-L20 LOCK comes after A'.9 milestone (b) baseline-snapshot capture at A'.9 closure (c) MOD_API_CONTRACT.md skeleton pre-authoring per Q-K-§6-6 recommendation.

### Prerequisite 9 — Disposition of A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md skeleton

**Empirical anchor** (§2.1):
- A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md exists at AUTHORED-SKELETON v0.1 (created 2026-05-17)
- Content: A9.A/B/C/D/E sub-milestones sketched + 18 rules listed + project location `tools/analyzers/DualFrontier.Analyzers/`
- A'.9.0 reconnaissance findings supersede some of skeleton's specifications (e.g., flat `tools/DualFrontier.Analyzers/` recommended over `tools/analyzers/`)

**Recommendation**: **REVISE** at Brief A'.9.1 deliberation — skeleton provides structural anchor + scope expectations; A'.9.1 brief authored against THIS reconnaissance report supersedes specific path/scope choices in skeleton. Skeleton remains as historical reference (Tier 3 AUTHORED-SKELETON status preserved); Brief A'.9.1 becomes new Tier 3 AUTHORED then EXECUTED через A'.9.1 cascade.

**Decision**: Brief A'.9.1 deliberation explicitly addresses disposition (supersede / merge / revise) и records в Q-N lock.

### Prerequisite 10 — Disposition of ANALYZER_RULES.md AUTHORED-SKELETON

**Empirical anchor** (§2.1):
- ANALYZER_RULES.md v0.1 AUTHORED-SKELETON (created А'.8 К-closure 2026-05-23) с 18 active + 4 reserved rules enumerated
- Per-rule §2 specification template ready для population
- Forward к Tier 1 LOCKED at A'.9 milestone implementation per current §5 lifecycle plan

**Recommendation**: **Continue к LOCKED per existing forward plan** — A'.9.1 cascade implementation actualizes the §2 template populations per-rule. Each DF### rule's analyzer class lands code + ANALYZER_RULES.md §4 detail section populated. Promotion к Tier 1 LOCKED at A'.9 milestone completion (per current §5 specification).

**Decision**: Brief A'.9.1 deliberation acknowledges existing plan + commits к §2 template population per-rule as cascade closure step.

---

## §11 — Open questions for Brief A'.9.1 deliberation (Q-K candidates)

Per S-LOCK-9: aggregated index of all Q-K candidates surfaced through reconnaissance, consolidated into single linear sequence for Brief A'.9.1 deliberation Q-N lock surface. Each entry cross-references the originating §X.99 subsection where full context + options + recommendation are detailed. **45 total Q-K candidates** (42 from sub-agents + 3 cross-cutting from α4 synthesis).

### §11.1 — К-L domain (Domain 1, originated in §3.99)

- **Q-K-1**: К-L21 row in skeleton misnumbered — no К-L21 exists per K_CLOSURE §2.24. (See §3.99 first bullet.) Recommendation: remove К-L21 row from skeleton matrix; clarify «total К-L = 21 cumulative invariants (К-L1..L19 main с К-L6 SUPERSEDED + 3 sub-invariants К-L3.1/L7.1/L15.1)» в §3.0 summary statistics.
- **Q-K-2**: Roslyn analyzer scope limitation для native-side К-L invariants (К-L1 C++20 dialect, К-L8 native owns storage, К-L15.1 Layer 3 compile-time isolation, К-L19 hardware GPU). (See §3.99.) Recommendation: document explicitly per-rule which enforcement tier carries the burden (Roslyn / pre-commit / runtime probe / CI native build / hardware capability check); defer native-side tooling к post-A'.9 cascade.
- **Q-K-3**: К-L15.1 Roslyn analyzer scope — managed-side facade only (Layer 3 native-side enforcement deferred к clang-tidy custom check). (See §3.99.) Recommendation: Option (a) restrict DF015.1 detection scope к managed-side facade per-tier API usage.
- **Q-K-4**: DF019 severity refinement — Error для static API-shape sub-detector / Warning для hardware-tier configurable. (See §3.99.) Recommendation: Option (c) make DF019 severity-configurable per detection sub-pattern.
- **Q-K-5**: DF010 (К-L10 decision rule) detection scope — narrow к `[ArchitecturalDecision]`-attribute-annotated context. (See §3.99.) Recommendation: Option (c) defer DF010 implementation pending decision-context attribute introduction.
- **Q-K-6**: DF008 (К-L8) process-invariant tooling — git pre-commit hook design scope. (See §3.99.) Recommendation: Option (a) defer pre-commit hook design к separate cascade (post-А'.9); document expectation в Brief A'.9.1.
- **Q-K-7**: Code-fix provider feasibility scope для А'.9 cascade. (See §3.99.) Recommendation: Option (a) include code-fix providers for Trivial-feasibility rules only (DF002 DllImport target, DF011 ManagedWorld → ManagedTestWorld в test scope); defer Moderate/Complex.

### §11.2 — FORMALIZE Lessons domain (Domain 2, originated in §4.99)

- **Q-K-8**: DL### namespace decision для Lesson-derived auxiliary rules. (See §4.99 Q-K-§4-1.) Recommendation: Option (c) defer namespace decision; tool prefix `tools/governance/check_*.ps1` for auxiliary tooling — Lesson rules are explicitly NOT Roslyn rules per matrix tier analysis. DL### reservation as forward-option.
- **Q-K-9**: Lesson #8 DL008 enforcement scope and CI cost trade-off (per-commit `dotnet build` + `dotnet test`). (See §4.99 Q-K-§4-2.) Recommendation: Option (c) enforce at closure protocol (matches existing §12.7 single-event cost model); preserve branch-local opt-in.
- **Q-K-10**: Lesson #25 + Lesson #N12 combined rule shape (if #N12 promotes к FORMALIZE). (See §4.99 Q-K-§4-3.) Recommendation: Option (c) defer combined rule design until #N12 promotion (3rd application surfaced).
- **Q-K-11**: Lesson #17 vs Lesson #20 merge decision. (See §4.99 Q-K-§4-4.) Recommendation: Option (c) defer к A'.9.0 closure deliberation; #17 vs #20 merge is governance bookkeeping, не A'.9 analyzer scope.
- **Q-K-12**: Lesson #N6 (test fixture cleanup discipline) auxiliary tooling shape if promoted. (See §4.99 Q-K-§4-5.) Recommendation: Option (b) extend closure protocol §12.7 step 1 wording at next METHODOLOGY closure к mandate Fixtures/ rebuild before Modding suite run.

### §11.3 — Cascade #2 + #3 surfaced candidates domain (Domain 3, originated in §5.99)

- **Q-K-13**: Rule namespace allocation для cascade-derived rules (DC### vs DF### vs DL### vs tiered DFK/DFL/DFC). (See §5.99 Q-K-§5-1.) Recommendation: Option (d) tiered namespace (DFK### / DFL### / DFC### explicit prefix discipline) matches existing governance taxonomy.
- **Q-K-14**: Analyzer infrastructure attributes (`[ReservedStub(SubPattern, Reason)]` + `[MarkerInterface]` + optionally `[DispatcherClass]` / `[DispatchHandler]` / `[LauncherComponent]`). (See §5.99 Q-K-§5-2.) Recommendation: Option (a) introduce all attributes в A'.9.1 Phase α infrastructure (~50-100 LOC single commit) — batched infrastructure unlocks higher-priority rules cleanly.
- **Q-K-15**: Rule pairing/consolidation — DC001 (IRenderCommand no Execute) + DC009 (Bridge Commands pure data) merge into single rule. (See §5.99 Q-K-§5-3.) Recommendation: Option (a) merge into single DFC001 с two sub-checks — simplicity wins.
- **Q-K-16**: Banned-namespace consolidation — C2-Rule-3 (no using Godot;/Silk.NET;) implementation pattern. (See §5.99 Q-K-§5-4.) Recommendation: Option (c) use existing `BannedApiAnalyzer` package; well-tested, integrates с .editorconfig configuration.
- **Q-K-17**: DC008 (no singleton/static в Launcher) scope vs `feedback_bus_access.md` SystemBase.Services carve-out. (See §5.99 Q-K-§5-5.) Recommendation: Option (a) DC008 stays Launcher-scoped (no conflict); revisit generalization after observing analyzer ergonomics.
- **Q-K-18**: Signature-stability tooling — adopt `Microsoft.CodeAnalysis.PublicApiAnalyzers` для C3-Rule-5 (RenderCommandDispatcher signature stability). (See §5.99 Q-K-§5-6.) Recommendation: Option (a) adopt PublicApiAnalyzers для `DualFrontier.Launcher` + `DualFrontier.Application.Bridge` assemblies at A'.9.1; directly addresses recurring K_EXT_N concern.
- **Q-K-19**: Governance-tier rule scope (C2-Rule-5 versioning convention enforcement outside Roslyn scope). (See §5.99 Q-K-§5-7.) Recommendation: Option (a) explicit acknowledgment + deferral preserves A'.9's focus on Roslyn analyzer ergonomics; flag parallel governance-tooling workstream.

### §11.4 — Mod OS К-L20 domain (Domain 4, originated in §6.99)

- **Q-K-20**: DF020 family scope ratification at К-L20 LOCK cascade Q7 deliberation. (See §6.99.) Recommendation: §6.2 enumeration (20 sub-rules) is reconnaissance-baseline; К-L20 LOCK cascade Q7 narrows or expands at deliberation time.
- **Q-K-21**: PublicApiAnalyzers timing — adopt at A'.9.1 (per Q-K-18) vs defer к К-L20 LOCK cascade. (See §6.99.) Recommendation: adopt at A'.9.1 (covers cascade #3 S-LOCK-8 + future K_EXT_N + provides K-L20 LOCK cascade с Mod API surface baseline tooling).
- **Q-K-22**: M3.4 (manifest cross-check analyzer milestone) placement — A'.9.X cascade vs К-L20 LOCK cascade. (See §6.99.) Recommendation: defer к A'.9.3+ standalone «Manifest Analyzer» sub-cascade — M3.4 is structurally independent of DF### К-L rules but conceptually tied к К-L20.
- **Q-K-23**: ManifestAnalyzer project structure — single project с DF### + DF020 namespaces, OR separate DualFrontier.Analyzers + DualFrontier.ManifestAnalyzer projects. (See §6.99.) Recommendation: separate `tools/DualFrontier.ManifestAnalyzer/` project (different concern: manifest-cross-check requires JSON parser + cross-file analysis vs C# syntax tree analysis).
- **Q-K-24**: К-L20 sub-invariant decomposition convention (DF020.1 through DF020.20 namespace) vs sub-К-L (К-L20.1, К-L20.2, ...) anchoring. (See §6.99.) Recommendation: keep DF### rule-ID sub-numbering (DF020.1..DF020.20) — К-L20 stays atomic invariant; rule decomposition exists at analyzer enforcement layer.
- **Q-K-25**: MOD_API_CONTRACT.md skeleton pre-authoring at A'.9 milestone closure (per K_CLOSURE §2.23 line 791). (See §6.99.) Recommendation: pre-author Tier 2 AUTHORED-SKELETON at A'.9 closure containing (a) IModApi v3 surface verbatim (b) manifest schema v3 verbatim (c) capability syntax verbatim — К-L20 LOCK cascade promotes к Tier 1 LOCKED без architectural surprise.

### §11.5 — Roslyn ecosystem domain (Domain 5, originated in §7.99)

- **Q-K-26**: DF009 severity downgrade Error → Warning until К-L20 Mod API lock lands. (See §7.99 Q-K-§7-1.) Recommendation: Option (b) Warning until К-L20 LOCK; aligns с ASP0019 precedent + ANALYZER_RULES.md «revisit post-Mod API lock» note.
- **Q-K-27**: Central Package Management (`Directory.Packages.props`) adoption at A'.9.1 для Roslyn SDK version centralization. (See §7.99 Q-K-§7-2 + Domain 6 confirms no CPM today.) Recommendation: Option (a) adopt CPM at A'.9.1; version drift risk (analyzer/test/codefix triad) is real + CPM is one-file change.
- **Q-K-28**: Analyzer distribution path — ProjectReference vs local NuGet vs hybrid. (See §7.99 Q-K-§7-3.) Recommendation: Option (a) ProjectReference path; single-developer internal-only context makes NuGet overhead unjustified.
- **Q-K-29**: `Microsoft.CodeAnalysis.Analyzers` (analyzer-of-analyzers) `EnforceExtendedAnalyzerRules=true` from day one. (See §7.99 Q-K-§7-4.) Recommendation: Option (a) enable from day one; friction is minor + RS1015 (helpLinkUri) ensures every DF### has documentation anchor.
- **Q-K-30**: Per-К-L-tier rule categories (`DualFrontier.Architecture` / `.NativeBoundary` / `.ModSurface`) для `.editorconfig` targeted overrides. (See §7.99 Q-K-§7-5.) Recommendation: Option (b) per-К-L-tier categories — supports targeted overrides + matches ANALYZER_RULES.md taxonomic structure.
- **Q-K-31**: `Microsoft.CodeAnalysis.NetAnalyzers` adoption alongside DF### rules. (See §7.99 Q-K-§7-6.) Recommendation: Option (b) defer NetAnalyzers; A'.9.1 focused on DF### rules; open separate brief A'.X for NetAnalyzers adoption когда DF### baseline stable.

### §11.6 — Build/CI domain (Domain 6, originated in §8.99)

- **Q-K-32**: Analyzer csproj TargetFramework override mechanism (netstandard2.0 vs Directory.Build.props conditional vs sibling Directory.Build.props). (See §8.99 Q-K-§8-1.) Recommendation: Option (a) analyzer csproj explicitly sets `<TargetFramework>netstandard2.0</TargetFramework>` (simplest, no Directory.Build.props churn).
- **Q-K-33**: CS1591 inheritance для analyzer csproj (suppress vs require XML docs). (See §8.99 Q-K-§8-2.) Recommendation: Option (a) inherit suppression for A'.9.1; ANALYZER_RULES.md is canonical rule documentation, не C# XML comments.
- **Q-K-34**: mods/DualFrontier.Mod.Example analyzer enforcement inclusion. (See §8.99 Q-K-§8-3.) Recommendation: Option (a) include mod in centralized analyzer reference — example mod should be exemplary; К-L9 vanilla=mods means analyzer treats mod code identically.
- **Q-K-35**: Cleanup-phase suppression rationale grammar/convention (free-text vs structured tag vs sidecar). (See §8.99 Q-K-§8-4.) Recommendation: structured single-line tag `// DFNNN-SUPPRESS: <citation>` + `#pragma warning disable/restore` brackets; avoid sidecar GlobalSuppressions.cs.
- **Q-K-36**: Closure protocol §12.7 inclusion of analyzer test project (always-run vs touch-triggered). (See §8.99 Q-K-§8-5.) Recommendation: always-run post-A'.9.1; analyzer test suite is fast (in-memory compilation) + surfaces regressions on every К-Lxx-adjacent change.
- **Q-K-37**: GitHub Actions adoption decision deferral target. (See §8.99 Q-K-§8-6.) Recommendation: defer outside A'.9 scope; relevant when collaboration model changes от solo-dev к multi-contributor.

### §11.7 — Suppression governance domain (Domain 7, originated in §9.99)

- **Q-K-38**: DF999 self-policing analyzer rule (ban GlobalSuppressions.cs + [assembly: SuppressMessage]) shipping timing. (See §9.99 Q-K-§9-1.) Recommendation: Option (a) ship in A'.9.1 alongside first DF### rules; greenfield-clean (0 existing assembly-level suppressions); retroactive cleanup cost high.
- **Q-K-39**: `[SuppressMessage]` attribute form sanction at A'.9.1 vs deferral. (See §9.99 Q-K-§9-2.) Recommendation: Option (b) A'.9.1 ships pragma-only; introduce attribute form at first false-positive demanding symbol-scope.
- **Q-K-40**: CAPA-tracking tier for diagnostic false-positives — site-scoped vs rule-scoped vs hybrid. (See §9.99 Q-K-§9-3.) Recommendation: Option (c) hybrid — site-scoped for first occurrence of rule, rule-scoped thereafter.
- **Q-K-41**: Test-side fixture suppression CAPA-exempt policy scope (blanket vs restricted vs blanket + quarterly review). (See §9.99 Q-K-§9-4.) Recommendation: Option (c) blanket exempt + quarterly review provides falsifiability mechanism.
- **Q-K-42**: Carve-out attribute mandatory Justification parameter. (See §9.99 Q-K-§9-5.) Recommendation: Option (a) Justification mandatory; preserves discipline established by existing CS0618 inline comment; marginal authoring cost offset by full traceability.

### §11.8 — Cross-cutting Q-Ks surfaced during α4 synthesis

- **Q-K-43**: A'.9.1 cascade shape — single multi-stage cascade (ξ/χ/ψ within one cascade-brief execution) vs three independent cascades (A'.9.1a scaffolding, A'.9.1b cleanup, A'.9.1c promotion).
  - Context: §8.4 recommends 3-stage progression ξ scaffolding → χ cleanup → ψ promotion. Cascade #3 ran 14 commits successfully — single-cascade ξ/χ/ψ feasible. But χ cleanup phase may surface high violation count (estimated 50-200 violations across 17 rules × 12 src projects); если exceeds budget, χ → standalone cascade A'.9.1b.
  - Options:
    - (a) Single A'.9.1 cascade с ξ/χ/ψ Phase α-equivalent sub-phases (matches existing cascade structure; commit count likely 15-25)
    - (b) Three sub-cascades A'.9.1a/b/c (each ~5-10 commits; clear χ cleanup-budget gate; lower per-cascade complexity)
    - (c) Hybrid — A'.9.1 ships ξ + small χ subset; standalone A'.9.1b finishes χ + ψ
  - Recommendation: Brief A'.9.1 deliberation chooses based on χ violation budget estimate. Default к (a) single cascade if estimate ≤80 violations; (b) three sub-cascades if estimate >150 violations; (c) hybrid otherwise.

- **Q-K-44**: A'.9 milestone cascade naming convention — К-extensions cascade #5+ (continues К-ext sequence) vs A'.9.1/A'.9.2/... (milestone-internal numbering only).
  - Context: A'.9.0 = К-extensions cascade #4 (dual designation per brief §0.5). A'.9.1 et seq follow same dual designation OR К-ext convention sunsets at A'.9 boundary?
  - Options:
    - (a) Continue dual designation: A'.9.1 = К-extensions cascade #5; A'.9.2 = К-extensions cascade #6; etc.
    - (b) A'.9 milestone-internal naming only: A'.9.1 = «A'.9.1» (К-ext numbering paused during A'.9 milestone)
    - (c) Hybrid — A'.9.1 dual designation (К-ext #5) because it's first analyzer impl, then A'.9.2+ milestone-internal only
  - Recommendation: Option (a) — continue dual designation для KERNEL chronicle + LEDGER §3.6+ entries continuity. К-extensions sequence captures «cumulative milestone-touching cascades», analyzer cascades qualify.

- **Q-K-45**: Documentation forward propagation plan для A'.9.1+ cascade outputs.
  - Context: A'.9.1 cascade produces (a) actualized ANALYZER_RULES.md §4 detail sections per-rule (b) MOD_API_CONTRACT.md skeleton per Q-K-25 (c) cleanup-phase ledger documenting violations resolved/suppressed/refined. Governance integration plan needed.
  - Options:
    - (a) Each artifact gets individual REGISTER enrollment + frontmatter (ANALYZER_RULES.md already enrolled at v0.1; MOD_API_CONTRACT.md becomes new Tier 2 AUTHORED-SKELETON; cleanup-ledger as Tier 4 closure-attached governance artifact)
    - (b) Bundle all into ANALYZER_RULES.md sub-sections (single governance artifact; lower REGISTER churn but lower granularity)
    - (c) Defer governance integration к A'.9.1 closure deliberation
  - Recommendation: Option (a) — explicit per-artifact REGISTER discipline matches existing pattern (cascade #2/#3 produced multiple governance artifacts; A'.9.1 likely produces 3-5 artifacts). Cleanup-ledger as cascade-closure attachment matches existing K_EXT_N cleanup-trace pattern.

---

## §12 — Cross-references

### §12.1 — Source documents read

**Phase 0 reconnaissance reads** (α0):
- `.git/HEAD` + `.git/refs/heads/main` + `.git/logs/HEAD` — state verification
- `docs/architecture/*.md` — 31 files (existence inventory via Glob)
- `docs/methodology/*.md` — 6 files (existence inventory via Glob)
- `docs/governance/*.{yaml,md}` — 6 files (existence inventory via Glob)
- `tools/briefs/*.md` — 63 files (existence inventory via Glob)
- `src/*` — 12 project directories (structural inventory)
- `tests/*` — 30 directories (10 test projects + 20 Fixture.* projects)
- `native/**` — DualFrontier.Core.Native C++ kernel extensive structure
- `DualFrontier.sln`, `Directory.Build.props`, `.editorconfig`, `.gitignore` (full reads + spot-read)
- Suppression scans: `#pragma warning disable` (5 hits) + `SuppressMessage` (0 hits) + `GlobalSuppressions` (0 hits)
- `tools/governance/*` — 5 files (existence inventory)
- `docs/architecture/ANALYZER_RULES.md` — full read (175 lines; surfaced as Phase 0 anomaly)
- `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` — partial read (100 lines; surfaced as Phase 0 anomaly)

**Reconnaissance batch A reads** (α1):
- *Agent A1*: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L table + chronicle; `docs/architecture/K_CLOSURE_REPORT.md` §2.1–§2.24 + §7; `docs/architecture/ANALYZER_RULES.md`; `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 LOCKED crosscut
- *Agent A2*: `docs/methodology/METHODOLOGY.md` v1.12 full read (1078 lines); §«Phase A' lessons» (lines 817–956) + §«Provisional Lessons» (lines 958–1073)
- *Agent A3*: `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md` full read; `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` full read; `docs/architecture/K_EXTENSIONS_LEDGER.md` §3.3 + §3.4 cascade narratives

**Reconnaissance batch B reads** (α2):
- *Agent B1*: 11 WebSearches + 5 WebFetches against NuGet.org, dotnet/roslyn-sdk, dotnet/aspnetcore, MS Learn, Meziantou, Aaronontheweb (full URLs cited in §7.0)
- *Agent B2*: `DualFrontier.sln` full read (456 lines); `Directory.Build.props` full read (39 lines); `.editorconfig` full read (4 lines); `tools/governance/sync_register.ps1` full read (395 lines); 3 csproj spot-reads (Core/Launcher/Core.Tests); `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` full read (151 lines)
- *Agent B3*: `src/DualFrontier.Core.Interop/NativeWorld.cs` lines 500–531; `src/DualFrontier.Core.Interop/Marshalling/NativeComponentType.cs` lines 21–58; `tests/DualFrontier.Core.Interop.Tests/BootstrapTests.cs` full (98 lines); `tests/DualFrontier.Core.Interop.Tests/ComponentTypeRegistryTests.cs` full (133 lines); `tests/DualFrontier.Core.Tests/Bus/ManagedBusBridgeTests.cs` full (373 lines); `docs/governance/REGISTER.yaml` capa_entries (lines 5776-6573); `docs/methodology/METHODOLOGY.md` Lesson #25 (line 973) + v1.11 changelog

**Reconnaissance Domain 4 reads** (α3):
- *Agent C1*: `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.11 LOCKED full read (1241 lines); `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L9 row (line 58); `docs/architecture/K_CLOSURE_REPORT.md` §2.11 К-L9 (lines 423-445), §2.23 К-L20 (lines 774-795), §7.3 DF020 (line 1689), §9.5 Mod API lock milestone Q1-Q8 (lines 1908-1929), §12 KERNEL Part 0 К-L20 placeholder note (line 2097); `docs/architecture/ANALYZER_RULES.md` §1 + §3 + §4 DF020 row; `docs/architecture/MODDING.md` v1.1 LOCKED IModApi v3 surface (lines 47-86); `mods/DualFrontier.Mod.Example` structure; `tools/DualFrontier.Mod.ManifestRewriter` project precedent

**Total empirical reads**: ~25 unique source files + ~10 cascade brief reads + ~11 web searches + ~5 web fetches

### §12.2 — Briefs

- **Predecessor cascade**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` (cascade #3 EXECUTED + PUSHED 2026-05-24)
- **This cascade**: `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md`
- **Predecessor analyzer skeleton**: `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` (AUTHORED-SKELETON 2026-05-17 — disposition pending Brief A'.9.1 deliberation)
- **Successor**: `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` (к be authored post-A'.9.0 closure against this report)

### §12.3 — Authoritative artifacts

- `docs/architecture/KERNEL_ARCHITECTURE.md` — К-L invariants canonical (Part 0)
- `docs/methodology/METHODOLOGY.md` — Lessons (FORMALIZE + Provisional)
- `docs/architecture/K_CLOSURE_REPORT.md` §7 — analyzer rule specifications canonical
- `docs/architecture/ANALYZER_RULES.md` AUTHORED-SKELETON v0.1 — rule taxonomy already enumerated
- `docs/architecture/K_EXTENSIONS_LEDGER.md` — cascade narratives (§3.5 entry pending β1)
- `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` — К-L14 evidence ledger (#13 entry pending β2)
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` — Phase A' chronological timeline (A'.9.0 entry pending β1)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.11 LOCKED — К-L20 prep surface source
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 LOCKED — substrate К-L crosscut
- `docs/governance/REGISTER.yaml` — governance SoT (register_version bump 2.5 → 2.6 pending β3)

---

*End of A_PRIME_9_RECONNAISSANCE_REPORT.md — Phase α complete*
*Authored 2026-05-24 via multi-agent reconnaissance (7 sub-agents across Domains 1–7 per brief §2 specification)*
*Brief A'.9.1 (Analyzer Infrastructure cascade) authoring per §10 prerequisites + §11 Q-K candidates*