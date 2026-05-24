---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-A_PRIME_9_RECONNAISSANCE_REPORT
category: A
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "post-A'.9.1 closure"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-A_PRIME_9_RECONNAISSANCE_REPORT
---

# A'.9 Reconnaissance Report — Roslyn Analyzer Architecture Discovery

**Designation**: A'.9.0 Reconnaissance cascade output (К-extensions cascade #4 cross-reference)
**Milestone**: A'.9 Roslyn Architectural Analyzer (multi-cascade milestone)
**Authoring cascade**: A'.9.0 Reconnaissance — Brief `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md`
**Authoring date**: 2026-05-24
**Status at α0**: SKELETON — sections populated through cascade Phase α (α1–α4)
**Status at closure**: Live (post-A'.9.0 closure)

---

## §1 — Executive summary

*[To be populated в Phase α4 — synthesizes §3–§9 findings into 1–2 page high-level summary]*

---

## §2 — Reconnaissance scope + methodology executed

*[To be populated through cascade — documents actual reads performed, multi-agent dispatch usage if any, any deviations от brief specification, halt-condition triggers + their resolution]*

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

*[To be populated в Phase α3 (Domain 4) — depends on Domain 1 results]*

### §6.1 — К-L20 statement (from KERNEL Part 0)

*[Verbatim quote]*

### §6.2 — Mod API restrictions analyzer-enforceable

*[Per Domain 4 methodology output]*

### §6.3 — A'.9-era preparatory rules (helping К-L20 era)

*[Forward-planning insights]*

---

## §7 — Roslyn ecosystem state

*[To be populated в Phase α2 (Domain 5) — desk research]*

### §7.1 — Current Roslyn SDK version

*[NuGet package + version + release date]*

### §7.2 — Test framework recommendations

*[Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit vs alternatives]*

### §7.3 — Severity policy precedents

*[Examples from Roslyn analyzers / ASP.NET Core / EF Core / etc.]*

### §7.4 — Code-fix provider patterns + adoption recommendation

*[Patterns + recommended scope для A'.9.1]*

---

## §8 — Build/CI integration surface

*[To be populated в Phase α2 (Domain 6)]*

### §8.1 — sln structural integration points

*[Findings — DualFrontier.sln has 12 src/ projects + 30+ test/fixture projects per Phase 0 inventory]*

### §8.2 — Directory.Build.props integration recommendation

*[Per Phase 0: TreatWarningsAsErrors=true already; no analyzer references; recommended insertion point]*

### §8.3 — .editorconfig severity override surface

*[Per Phase 0: file essentially empty — full freedom для rule severity convention]*

### §8.4 — CI integration trigger recommendation

*[Per Phase 0: no .github/workflows — local-only build verification; analyzer runs on `dotnet build` automatically OR opt-in?]*

---

## §9 — Suppression governance precedent + recommendations

*[To be populated в Phase α2 (Domain 7)]*

### §9.1 — Existing suppression patterns inventory

*[Per Phase 0 grep: 1 src pragma + 4 test pragmas + 0 SuppressMessage + 0 GlobalSuppressions]*

### §9.2 — Suppression governance recommendations

*[When allowed / CAPA tracking / review cadence — building on clean baseline]*

---

## §10 — Brief A'.9.1 prerequisites

*[To be populated в Phase α4 — per S-LOCK-8 mandatory enumeration]*

Decisions Brief A'.9.1 deliberation must ratify (based on this report):

1. **Rule prioritization batch для A'.9.1**: *[P0 candidate set]*
2. **Analyzer project structure**: *[location confirmation — `src/DualFrontier.Analyzers/` vs `tools/analyzers/DualFrontier.Analyzers/` per A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md skeleton; csproj config; dependencies]*
3. **Test framework choice**: *[recommendation]*
4. **Severity policy**: *[per-rule severity assignment rules]*
5. **Suppression policy**: *[when allowed, CAPA tracking, governance protocol]*
6. **Build/CI integration trigger**: *[analyzer runs on `dotnet build` automatically OR opt-in?]*
7. **A'.9 cascade decomposition refinement**: *[A'.9.2/A'.9.3/... initial scope based on rule sequencing]*
8. **К-L20 Mod API lock timing**: *[forward path]*
9. **Disposition of A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md skeleton**: *[supersede / merge / revise — Phase 0 surfaced this predecessor; needs explicit decision]*
10. **Disposition of ANALYZER_RULES.md AUTHORED-SKELETON**: *[continue к LOCKED per existing forward plan; ensure A'.9.1 implementation actualizes the §2 template populations]*

---

## §11 — Open questions for Brief A'.9.1 deliberation (Q-K candidates)

*[To be populated в Phase α4 — per S-LOCK-9 mandatory]*

---

## §12 — Cross-references

### §12.1 — Source documents read

*[Populated через cascade execution log]*

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

*End of A_PRIME_9_RECONNAISSANCE_REPORT.md skeleton — Phase α0*
*Sections §1–§11 populated through cascade Phase α1–α4 per brief §4 specification*
