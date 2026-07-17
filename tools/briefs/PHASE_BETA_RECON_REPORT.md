---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-PHASE_BETA_RECON_REPORT
category: D
tier: 4
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-PHASE_BETA_RECON_REPORT
---
# PHASE BETA RECON REPORT — 2026-07-01

Repository: `D:\Colony_Simulator\Colony_Simulator` @ `02be616` · read-only session · census discipline per `TESTING_STRATEGY` §4 (every count carries its verbatim expression; UPPER-BOUND proxies in R3 are explicitly labelled, never presented as actual violation counts).

## R1 — Base state

| Fact | Value | Source expression |
|---|---|---|
| `.git/HEAD` | `ref: refs/heads/main` | `cat .git/HEAD` |
| Current branch | `main` | `git branch --show-current` |
| HEAD | `02be616` — *governance(register): render regeneration + header backfill* (2026-06-30 01:55 -0400) | `git rev-parse HEAD` / `git log -1` |
| Working tree | **CLEAN** (0 tracked changes — the two builds touched only gitignored `bin`/`obj`) | `git status --porcelain` → empty (verified post-build) |
| `main` HEAD | `02be616` | `.git/refs/heads/main` |
| `origin/main` (local ref) | `02be616` | `git rev-parse origin/main` |
| Divergence | `0  0` (main ↔ origin/main, **local refs only — NO fetch this session**; `.git/FETCH_HEAD` mtime Jul 1 02:22, pre-session) | `git rev-list --left-right --count main...origin/main` |
| `register_version` | **`2.18`** (expected 2.18 ✓); `schema_version 1.0`; `last_modified 2026-06-29 / d2d0901` | `grep register_version docs/governance/REGISTER.yaml` |
| REGISTER counts | **275 DOC, 41 EVT, 41 REQ, 17 CAPA, 14 RISK** | `grep -cE '^  - id: DOC-'` (275); `grep -oE '^  - id: [A-Z]+-' \| sort \| uniq -c` |

HEAD is exactly the expected post-Godot-eradication baseline `02be616`; operator landed nothing further. Godot eradication is real on disk (top-level has `native/`, Vulkan shaders in `tools/shaders`, `glslangValidator.exe`; no Godot project).

## R2 — 17-stub inventory

Expected set **verified exactly**: Architecture 9 (DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017); NativeBoundary 5 (DFK001, DFK002, DFK007_1, DFK015_1, DFK019_A); Discipline 3 (DF999, DFL025_A, DFL025_B). All under `tools/DualFrontier.Analyzers/Rules/{Architecture,NativeBoundary,Discipline}/`.

**Shared structure (identical across all 17 — stated once, per `RESERVED_SURFACE_MUTABILITY` §3 "descriptor ID is the contract; class name is surface"):**
- `DiagnosticDescriptor` fields: `id` = the dotted/hyphen descriptor string; `title`/`messageFormat`/`description` = per-rule; `category` = `DualFrontier.{Architecture|NativeBoundary|Discipline}`; **`defaultSeverity: DiagnosticSeverity.Info`** (all 17); **`isEnabledByDefault: true`** (all 17); `helpLinkUri` → GitHub `Crystalka228/Dual-Frontier/.../ANALYZER_RULES.md#<slug>`.
- **Stub body (operative lines, byte-identical intent across all 17):**
  ```csharp
  public override void Initialize(AnalysisContext context)
  {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      // Phase β cleanup-phase populates detection logic here.
      // Stub returns zero diagnostics при build time.
  }
  ```
  **Zero-diagnostic proof (machine-verified):** `rg -t cs -o 'context\.Register\w+' tools/DualFrontier.Analyzers/Rules` → **0** matches. A Roslyn analyzer that registers no actions is structurally incapable of emitting a diagnostic. This is the strongest guarantee; the clean build (R4) corroborates it.
- **AnalyzerReleases registration:** all 17 present in `AnalyzerReleases.Unshipped.md` "New Rules" table (yes ×17); `Shipped.md` empty (header only).

| Rule | Cat | Stub sev → Phase-γ target | Registered (Unshipped) | Intended semantics — the non-compliant pattern it must flag | Detection kind |
|---|---|---|---|---|---|
| DFK001 | NB | Info → **Error** | yes | Managed bridge bypassing canonical P/Invoke surface / unsanctioned interop (native-side C++20 is outside Roslyn per S-LOCK-2) | SEMANTIC (+FQN) |
| DFK002 | NB | Info → **Error** | yes | Ad-hoc `[DllImport]`/`[LibraryImport]` **outside `DualFrontier.Core.Interop`** | **FQN-STRING** / SYNTAX |
| DFK003 | Arch | Info → **Error** | yes | Managed allocation of ECS-shaped storage / ownership transfer; struct+`[ManagedStorage]` or class-`IComponent`-without-`[ManagedStorage]`; cross-path bypass | SEMANTIC |
| DFK003.1 | Arch | Info → **Error** | yes | Storage-bridge facade that allocates/copies/mutates native storage; cross-mod `ManagedStore<T>`; Path-β persistence; API bypass | SEMANTIC (+FQN) |
| DFK004 | Arch | Info → **Error** | yes | Implicit type-ID derivation (hash / `typeof().FullName.GetHashCode()`) bypassing `ComponentTypeRegistry` | SEMANTIC |
| DFK005 | Arch | Info → **Error** | yes | Multiple/managed-only bootstrap entry points diverging from declarative `bootstrap_graph` | SYNTAX/SEMANTIC |
| DFK007 | Arch | Info → **Error** | yes | Span-protocol violation: mutation through read span, write outside batch, storage retained/copied out | SEMANTIC (hardest) |
| DFK011 | Arch | Info → **Error** | yes | `new ManagedWorld()` in production / repeated NativeWorld instantiation / shadow world | SEMANTIC (+FQN) |
| DFK007.1 | NB | Info → **Error** | yes | GPU pipeline-slot access bypassing `PipelineSlotInterop.ReadSlotTail` (direct slot indexing) | SEMANTIC |
| DFK015.1 | NB | Info → **Error** | yes | Managed concurrency bypassing the 3-tier mutex facade (raw `lock`/`Monitor` vs native-owned) | SEMANTIC |
| DFK017 | Arch | Info → **Error** | yes | Display multi-layer composition: cross-layer / out-of-order draws / alternate surface | SEMANTIC |
| DFK019.A | NB | Info → **Warning** ² | yes | Vulkan <1.3 API / vendor extensions / alternate graphics API (static surface only; runtime tier = DFK019.B, deferred) | SEMANTIC (+FQN) |
| DFK013 | Arch | Info → **Warning** | yes | `SystemBase` subclass without `[WakeOn*]`/`[TickRate]` (efficiency, not correctness) | SEMANTIC |
| DFK016 | Arch | Info → **Warning** (retain-α, Q-L-16) | yes | Hardcoded pipeline-depth literal 1/2/3 vs `PipelineSlotInterop.DefaultDepth`/`.MaxDepth` | SYNTAX/SEMANTIC |
| DFL025-A | Disc | Info → **Warning** | yes | Test class invoking a `[ReservedStub]`-tagged type without `[Trait("Category","ReservedStub")]` | **FQN-STRING** (explicit) |
| DFL025-B | Disc | Info → **Suggestion** | yes | Standalone test vs reserved-stub module not using `[Fact(Skip="…")]` | SEMANTIC |
| DF999 | Disc | Info → **Warning** | yes | Solution-wide `GlobalSuppressions.cs` / `[assembly: SuppressMessage]` | SYNTAX |

**Detection-kind classification vs Lesson #N19** (detection via canonical FQN strings, not CLR type references — the analyzer csproj deliberately has **no** `ProjectReference` to Contracts, confirmed in its rationale comment): the rules that *naturally want FQN-string detection* are **DFL025-A** (canonical: `attr.AttributeClass?.ToDisplayString() == "DualFrontier.Contracts.Analyzer.ReservedStubAttribute"` — documented verbatim in the stub + csproj), **DFK002** (`DllImportAttribute`/`LibraryImportAttribute` FQN + containing-assembly check), **DFK011** (`ManagedWorld` FQN outside test namespace), **DFK003.1** (`ManagedStore<T>` FQN), **DFK019.A** (Vulkan symbol FQNs). Pure **SYNTAX** (cheap node walk): DF999, DFK005, DFK016, DFL025-B. Everything else needs the **SEMANTIC** model (symbol/type/dataflow); DFK007 (span lifetime/dataflow) is the costliest.

**Severity cross-check.** No mismatch in any stub: the ratified plan is *all 17 ship `Info` through Phase β-prep, promoted at Phase γ* (`ANALYZER_RULES` §4.1 footnote¹). **DFK019.A** confirmed: stub = `Info`; **Phase-γ target = Warning** per `K_CLOSURE_REPORT` §7.2 (DF019 row: "Warning … V substrate contract, configurable"), footnoted in `ANALYZER_RULES` §4.1 (²) and the ROADMAP promotion map. This is tracked as **F-12 (S2, OPEN)** in `docs/ROADMAP.md` Findings ledger: *"K_CLOSURE §7.2 + ANALYZER_RULES say Warning; A'.9.1 brief §8.1 blanket promotion reads Error … Crystalka ratifies before Phase γ promotion executes."* → gates **Phase γ**, not Phase β. **DFK016** conditional status confirmed: **retained at α** (Q-L-16, managed surface stable per Phase 0 closure §3.3), Phase-γ target Warning (efficiency-class, with DFK013).

## R3 — Violation-surface proxy (front B — the calibration)

**All counts below are UPPER-BOUND candidate-site proxies** (pattern populations), not violations — detection logic does not yet exist to run. Scope = `src/` (the gate's own scope: brief §7.4 counts "diagnostics on **src/** paths"). Unit: `occ` = occurrences (`rg -t cs -o <pat> src | wc -l`); `files` = `rg -t cs -l`.

| Rule | Verbatim rg expression (over `src`) | Candidate | Refined likely-violation | Note (over-approximation) |
|---|---|---|---|---|
| DFK001 | shares P/Invoke surface ↓ | 116 / 3f | ~0–3 | K-L1 largely native-side (out of scope); managed = bridge-bypass subset of DFK002 surface |
| **DFK002** | `rg -t cs -o 'DllImport\|LibraryImport' src -g '!**/DualFrontier.Core.Interop/**'` | **116 / 3f** | **116 → ~13** | **DOMINANT.** VkApi.cs 86 + Win32Api.cs ~16 + ManagedBusBridge.cs 13. "Violation?" hinges on one Phase-β decision (below) |
| DFK003 | `: IComponent` (+ `\[ManagedStorage\]`) | 26/25 (+8/5) | ~0–8 | needs SemanticModel; compile-time isolation already enforced (A'.6) |
| DFK003.1 | `ManagedStore` | 53/10 | ~0–3 | cross-mod/persistence/API-bypass — semantic |
| DFK004 | `typeof\(` (+ `GetHashCode`) | 137/48 (+8/5) | ~0–3 | typeof/GHC overwhelmingly legit; `ComponentTypeRegistry` (26/6) is the sanctioned surface, well-used |
| DFK005 | `class \w*Bootstrap` | 3/2 | ~0–2 | canonical GameBootstrap present; violation = *extra* entry points |
| DFK007 | `Span<` (+ `ReadOnlySpan<`) | 132/31 (+75/24) | ~0–5 | span misuse needs dataflow; vast majority correct usage |
| DFK007.1 | `PipelineSlot` (`ReadSlotTail` = sanctioned) | 30/4 (RST 2/1) | ~0–2 | direct slot indexing bypass only |
| DFK011 | `new ManagedWorld\(` / `\bManagedWorld\b` | **0 / 0** | **0** | CLEAN — ManagedWorld fully retired (A'.7) |
| DFK013 | `:\s*SystemBase\b` minus `\[WakeOn\|\[TickRate` | 31 subclass files; **1 lacks** | **0** | the 1 (`ModCapabilitiesAttribute.cs`) is a `///` doc-comment example `MyCombatSystem : SystemBase` → **confirmed false positive** |
| DFK016 | `PipelineSlotInterop` (+ `DefaultDepth\|MaxDepth`) | 13/4 (+4/1) | ~0–2 | retain-α; managed surface stable; violation = hardcoded literal |
| DFK017 | `LayerType` (+ `\[Layer\(`) | 27/8 (+2/2) | ~0–2 | cross-layer/order — semantic |
| DFK015.1 | `lock \(` + `Mutex\|Semaphore` (`Monitor.` = 0) | 26/6 + 45/6 | ~0–5 | many managed-only locks are legit; violation = vs native-owned |
| DFK019.A | `OpenGL\|DirectX\|Direct3D\|\bD3D[0-9]\|\bMetal\b\|VK_API_VERSION_1_[012]\|Vulkan1[012]` | **1** | **~0** | the 871-occ `[Vv]ulkan\|Silk\.NET\|\bvk[A-Z]` raw is the **sanctioned Vulkan 1.3 interop surface**, not violations |
| DF999 | `GlobalSuppressions.cs` files + `rg 'assembly:\s*SuppressMessage' src tests mods` | **0 + 0** | **0** | CLEAN |
| DFL025-A | `\[Trait\("Category",\s*"ReservedStub"` (tests) + `rg -l 'ReservedStub' tests` | **0 / 0** | **0** | no stub-touching tests exist (matches `TESTING_STRATEGY` §5.3) |
| DFL025-B | `\[Fact\(Skip` (tests) | **0** | **0** | none |

**Baseline pins verified (HARD-pin discipline):** `[ReservedStub` census `rg -t cs -o '\[ReservedStub' src -g '!**/ReservedStubAttribute.cs'` = **34 occ / 13 files — exactly matches the `TESTING_STRATEGY` §4.1 / `CODING_STANDARDS` §5.1 pin (34/13)**. DFK-WAIVER census `rg -t cs '#pragma warning disable (DFK|DFL|DF9)' src tests mods` = **0**; `[SuppressMessage]` over src+tests = **0** — matches §4.3 baseline 0.

**Aggregate & Q-L-1 gate prediction.**

- **RAW upper-bound** = Σ broadest candidate populations ≈ **~1,771** (`277+34+53+145+9+207+30+1+31+13+29+71+871`). → mechanically **">150 → split"**, but **non-actionable**: ~49% is DFK019.A's sanctioned Vulkan surface (871), ~16% is canonical `Core.Interop` P/Invoke (161), the remainder is legitimate `typeof`/`Span`/`lock` language use. No correctly-implemented rule flags these.

- **REFINED estimate** — dominated by one rule (DFK002), two scenarios:
  - **Scenario A — DFK002 applied literally** (Runtime.Native P/Invoke = violations): `116 + Σ(residuals ~10–32)` ≈ **~126–148** → **80–150 band → "Crystalka discretion / hybrid"** (near the top edge).
  - **Scenario B — DFK002 refined to sanction `DualFrontier.Runtime.Native.*`** (the realistic Phase-β outcome — VkApi/Win32Api are the legitimate post-Godot native runtime, architecturally analogous to `Core.Interop`): `~13 (ManagedBusBridge) + Σ(residuals ~10–32)` ≈ **~23–45** → **≤80 → "continue single cascade."**

- **Predicted branch: ≤80 "continue single A'.9.1 cascade" is most probable** (Scenario B), contingent on the DFK002 Runtime.Native sanction decision. **80–150 "Crystalka discretion" is the worst realistic case** (Scenario A). **">150 split" is NOT predicted under any realistic reading.** The brief must (a) plan the ≤80 single-cascade path and (b) pre-stage the DFK002 architectural decision as the gate's swing variable.

## R4 — Test harness + build baseline (front C)

- **Test project state:** `tests/DualFrontier.Analyzers.Tests/` = csproj + **single `PlaceholderTests.cs`** (one `[Fact] Placeholder_AnalyzerAssembly_LoadsCleanly` asserting `typeof(AnalyzerEntryPoint).Assembly.Should().NotBeNull()`). Placeholder-level exactly as prior recon expected; per-rule suites are Phase β scope (`TESTING_STRATEGY` §3.5, §4.1).
- **Roslyn analyzer-test harness present ✓:** csproj references `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` **+** an explicit `Microsoft.CodeAnalysis.CSharp.Workspaces` PackageReference (documented reason: forces CPM-pinned **5.3.0** to win MEF composition over the testing package's transitive Workspaces 1.0.1). xUnit 2.9.2 + FluentAssertions 6.12.1 + `Microsoft.NET.Test.Sdk` 17.11.1. The Phase-β pattern to extend = `CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>`.
- **Run command:** `dotnet test tests/DualFrontier.Analyzers.Tests` (per-commit gate in brief §4.3 uses `--filter "Category!=ReservedStub"`).
- **Build baseline (Release):** analyzer project → **Build succeeded, 0 Warning(s), 0 Error(s)**; analyzer tests → **Passed! 1/1 (0 failed)**; **full solution `dotnet build DualFrontier.sln -c Release`** → **Build succeeded, 0 Warning(s), 0 Error(s)** (23s). With `TreatWarningsAsErrors=true` (root `Directory.Build.props`) and the analyzer wired to **all 12 src projects** (`src/Directory.Build.props`, `OutputItemType="Analyzer"`), the clean build confirms the **17 stubs emit ZERO diagnostics against `src/` today** — the regression anchor Phase β must not disturb except via deliberate new detection.
- **Package versions (`Directory.Packages.props`, CPM):** `Microsoft.CodeAnalysis.CSharp` **5.3.0**, `.CSharp.Workspaces` 5.3.0, `.Analyzers` 5.3.0, `.CSharp.Analyzer.Testing.XUnit` **1.1.2**. dotnet SDK **10.0.204**. Analyzer csproj: `TargetFramework=netstandard2.0` (Q-L-4 host-load compat), `IsRoslynComponent=true`, `EnforceExtendedAnalyzerRules=true`.

## R5 — AnalyzerReleases + RS2008

- **`AnalyzerReleases.Shipped.md`:** empty (comment header only) — "First release entry added at A'.9.1 closure (Phase γ severity promotion …)."
- **`AnalyzerReleases.Unshipped.md`:** "### New Rules" table lists **all 17** (Rule ID | Category | Severity=**Info** | Notes), grouped 9 Architecture / 5 NativeBoundary / 3 Discipline. Notes already carry the promotion intent ("Warning post-promotion" for DFL025-A/DF999, "Suggestion post-promotion" for DFL025-B).
- **RS2008 active ✓:** analyzer csproj declares both `<AdditionalFiles Include="AnalyzerReleases.Shipped.md"/>` + `Unshipped.md`, with `Microsoft.CodeAnalysis.Analyzers` referenced (release-tracking analyzer). This is the canonical pattern chosen over `<NoWarn>` per PA-002 (csproj comment §36-43).
- **Activation transitions the brief must specify:** today every rule is a stub in Unshipped at `Info`. When a rule goes stub→detecting, RS2008 governs its placement. Because the ruleset only "ships" at **Phase γ** (Shipped.md populated then per brief §2.1), the correct sequence is: Phase β leaves rules in **Unshipped** (detection lands, severity still Info) → Phase γ **moves the promoted rules Unshipped→Shipped** with their new severities (Error/Warning/Suggestion) in one release block. No rule currently sits in a state RS2008 would flag (all consistently Unshipped/Info).

## R6 — Standing-law citation anchors (front D)

| Anchor | Exists | Citation form | What it says (one line) |
|---|---|---|---|
| `ANALYZER_RULES.md` §4 (v0.2.1) | ✓ | `ANALYZER_RULES §4 (rule registry)` | The 17-stub registry (§4.1 single table + Phase-γ targets); ground-truth "17 non-detecting Info stubs, zero diagnostics." |
| — its DFK-WAIVER deferral pointer | ✓ | `ANALYZER_RULES §12 (suppression, deferred by citation)` | §12 owns no suppression law; **cites** `CODING_STANDARDS §5.3 "DFK-WAIVER"`; v0.1 `#pragma`/`DFK###-SUPPRESS` sketches superseded. |
| `CODING_STANDARDS.md` §5.3 "DFK-WAIVER — the suppression law" | ✓ | `CODING_STANDARDS §5.3 (DFK-WAIVER suppression law)` | Form (`// DFK-WAIVER(<id>)` immediately preceding a minimal `#pragma disable/restore`); 3 allowed reason classes (false-positive-pending-refinement / sanctioned-exception citing Q-L#/К-L# / generated-or-interop); **baseline 0**; file/project `<NoWarn>` for DFK/DFL **forbidden**; descriptor-ID form (dots/hyphens). |
| `TESTING_STRATEGY.md` §4 (v2.0.0) census pins | ✓ | `TESTING_STRATEGY §4.1 (reserved-surface census pin)` | §4.1 `[ReservedStub` **EXACT pin 34/13**, monotonicity NOT asserted (HARD pin); §4.2 five marker families; §4.3 DFK-WAIVER baseline 0; §4.4 S-LOCK→verifying-artifact. |
| — §6 brief-integration | ✓ | `TESTING_STRATEGY §6 (brief integration)` | What a brief CARRIES (test files, named test list, layer, coverage anchors, 1–2 bodies, count delta) vs CITES; §3.5 analyzer-test convention; §5.3 lying-stub law. |
| `RESERVED_SURFACE_MUTABILITY.md` | ✓ (v1.0 LOCKED) | `RESERVED_SURFACE_MUTABILITY §3/§5` | Mutable vs immutable catalogs; **"Skeleton revisions:"** commit-body form for surface/census-delta records; §4 pins the 21 К-L composition + F-4 bi-script grep warning. |
| Brief §7 (Phase β cleanup spec) | ✓ | `A'.9.1 brief §7.1–§7.4` (lines 1716–1799) | Phase β: implement detection for all 15–16 active rules, run over codebase, per-diagnostic triage (fix/suppress/refine), per-rule violation-inventory YAML, §7.4 closure gate + adaptive gate. |
| Brief §10.3 (referenced closure inputs) | ⚠ **soft drift** | see note | Brief **has** a §10 ("ANALYZER_RULES.md scope-split detail", line 2081) but **no §10.3/§10.5**; the "§10.3 per-rule order" (cited by `src/Directory.Build.props`) and "§10.5" (cited by all 17 stubs as "Brief A'.9.1 §10.5") now live in **`ANALYZER_RULES` §10.5** + **`ROADMAP` «Analyzer track»** (relocated at the 2026-06-11 Standing-Law cascade). Content exists; the *attribution* is stale — a §3(3) annotation-surface fix, not a hard dangling citation. |
| `K_CLOSURE_REPORT.md` §7.2 (DFK019.A=Warning) | ✓ | `K_CLOSURE §7.2 (active rules table)` | DF019 row = **Warning** (V substrate contract, configurable) + full DF001–DF019 detection narratives (the canonical source for Phase-β detection logic). |
| K-L19 canonical text | ✓ | `K_CLOSURE §2.22` | Vulkan 1.3 + async-compute-queue mandate; fail-fast on missing async compute. |

**Q-L-1 gate — exact text + evaluation point.** Authored in the **brief** (§1.3 Q-L-1 row; §2.2 decision tree; §4.2 Task 7; **§7.4 canonical**) and restated in **`ROADMAP` «Analyzer track»** (line 870). Verbatim (brief §7.4 "Adaptive gate per Q-L-1"):
> `violation_count ≤ 80` → continue single A'.9.1 cascade to Phase γ
> `violation_count > 150` → split: this cascade closes with Phase β subset; A'.9.1b standalone cascade authored for cleanup + Phase γ
> `80 < violation_count ≤ 150` → Crystalka decision (hybrid split point)

(ROADMAP notation: "≤ 80 … 81–150 Crystalka decision … > 150 split" — the `80` boundary is `≤80=continue` in both; `80–150`/`81–150` is cosmetic.)

**Evaluation point — dual-framed; confirm the load-bearing one is Phase β closure.** The brief frames it twice: (i) as a **Phase 0 / Phase-α-exit *estimate*** (§2.2 "Phase 0 violation count"; §4.2 Task 7 "Phase α exit determines Phase β shape"), and (ii) as the **Phase β closure *gate*** (§7.4, under "Phase β closure gate → Adaptive gate per Q-L-1") that decides continue-to-γ vs split. **The load-bearing evaluation is at Phase β closure** — the *actual* diagnostic count only exists once Phase β populates detection and runs it. **Gap surfaced:** Task 7 as literally written ("build analyzer with rules implemented as stubs (return empty diagnostic each) → dry-run violation enumeration") is self-contradictory — stubs emit zero, so the dry-run yields 0. The real calibration must come from populated detection (Phase β) or a pre-implementation proxy — **which is precisely what this recon's R3 supplies**, so the brief can pre-commit the gate branch instead of discovering it at Phase β closure.

## R7 — Anomalies + scale/calibration estimate

**Predicted branch + dominant contributors (ranked by candidate count):** DFK019.A (871 raw, **~0 real**) ≫ DFK002/DFK001 (277 raw → **116 real-candidate, the sole material lever**) > DFK007 (207, ~0 real) > DFK004 (145, ~0 real) > DFK015.1 (71, ~0–5) > DFK003.1 (53). **By refined violation, one rule dominates: DFK002.** Branch: **≤80 (continue) most likely; 80–150 (discretion) worst realistic; >150 not realistic.**

**Anomalies / WIP:**
1. **DFK002 Runtime.Native swing variable (highest leverage).** New post-Godot native surface `src/DualFrontier.Runtime/Native/{Vulkan/VkApi.cs (86 LibraryImport), Win32/Win32Api.cs (~16)}` + `Application/Bus/ManagedBusBridge.cs (13 DllImport)` = 116 P/Invoke outside `Core.Interop`. DFK002 as specified ("`Core.Interop` only") flags all 116. **Phase β must decide: sanction `DualFrontier.Runtime.Native.*` as an additional canonical interop surface (rule refinement) — collapsing 116→~13 (≤80 branch) — or waiver-with-citation.** ManagedBusBridge's 13 DllImport in the Application layer is the one genuinely-odd cluster worth real triage.
2. **Brief §2.3 project enumeration is STALE.** It names `Application.Scheduler / Application.Modding / Core.Scheduling`; on-disk (truth per `RESERVED_SURFACE_MUTABILITY` §3(4)) is 12 projects: AI, Application, Components, Contracts, Core, Core.Interop, Crypto.Future, Events, Launcher, Persistence, Runtime, Systems. Analyzer wiring (`src/Directory.Build.props`) is current (all 12). No action beyond noting the recap is intent, not truth.
3. **F-12 (S2, OPEN)** — DFK019.A Phase-γ Warning vs brief §8.1 blanket-Error; gates **Phase γ** (Crystalka ratifies), **not Phase β**.
4. **Citation drift** — stubs cite "Brief A'.9.1 §10.5", `src/Directory.Build.props` cites "brief §10.3"; content relocated to `ANALYZER_RULES` §10.5 + `ROADMAP` «Analyzer track» (soft §3(3) fix).
5. **`.editorconfig` present but unprimed** — no `dotnet_diagnostic.DFK*.severity` lines yet (Phase γ mechanism exists, populated at Phase γ). Expected.
6. **DFK013 proxy false positive** (doc-comment example) — refined DFK013 = 0; codebase is thoroughly wake-annotated (39 `[TickRate]` + 2 `[WakeOn]` cover 31 SystemBase subclasses).

**Complexity tiers (implementation cost):**
- **Tier 1 — SYNTAX, cheap:** DF999, DFK002, DFK005, DFK016, DFL025-B (+ DFK002 wants a simple containing-assembly check).
- **Tier 2 — FQN-STRING (Lesson #N19 canonical):** DFL025-A (documented), DFK011, DFK003.1, DFK019.A (+ DFK002 by FQN).
- **Tier 3 — SEMANTIC:** DFK001, DFK003, DFK004, DFK007 (costliest — span dataflow/lifetime), DFK007.1, DFK013, DFK015.1, DFK017.

**Test-authoring estimate:** 17 analyzer-test classes (1/rule), each ≥1 positive + ≥1 negative → **~34 floor; ~50–80 realistic** (multiple violation patterns per K_CLOSURE §7.2 narrative) via `CSharpAnalyzerVerifier<T>` (harness present). Plus **~7 §4 census meta-tests** (reserved-surface 34/13 exact-pin, 5 marker families, DFK-WAIVER=0) that `TESTING_STRATEGY` §4.1–4.3 says materialize with the Phase-β buildout. Per `TESTING_STRATEGY` §6.1, the brief must carry each as a named-test list with layer (§3.5) + coverage anchor + count delta.

**Proposed commit-class split + topology:**
1. **Harness upgrade** — replace `PlaceholderTests` with `CSharpAnalyzerVerifier` scaffolding (1 commit).
2. **Rule batches (disjoint files → parallel wave):** β1 SYNTAX (DF999, DFK002, DFK005, DFK016, DFL025-B); β2 FQN-string (DFL025-A, DFK011, DFK003.1, DFK019.A); β3 SEMANTIC-Architecture (DFK003, DFK004, DFK007, DFK013, DFK017); β4 SEMANTIC-NativeBoundary (DFK001, DFK007.1, DFK015.1). Each batch = disjoint `Rules/<Cat>/*.cs` + disjoint test classes + Unshipped rows.
3. **Gate evaluation + triage** — run analyzer over `src/`, enumerate real diagnostics, DFK-WAIVER triage, evaluate Q-L-1 branch (needs ALL detection landed → **barrier** after batches).
4. **AnalyzerReleases + Phase γ** — Unshipped→Shipped transitions + `.editorconfig` severities (resolve F-12 first).
5. **Phase δ closure** — §7 census table, S-LOCK coverage, EVT + `register_version` bump.

**Multi-agent wave topology FITS:** rule `.cs` files and test classes are disjoint (one per rule) → clean fan-out of writers, each owning a rule+test pair (worktree-isolated). **Barrier before the gate-evaluation commit** (step 3 needs the full detection set to count). **Serialize the two shared-mutation files** — `AnalyzerReleases.Unshipped.md` and `.editorconfig` — into a post-wave consolidation commit to avoid write conflicts.

## R8 — Self-attestation

- **Zero writes** (validate NOT run — `sync_register.ps1 -Validate` avoided per its `VALIDATION_REPORT.md` write-trap; REGISTER.yaml read directly; the two `dotnet build`/`test` runs touched only gitignored `bin`/`obj`): **CONFIRMED** — `git status --porcelain` empty post-build, HEAD unchanged at `02be616`.
- **Zero git mutations** (no commit/checkout/switch/merge/fetch/stash; only read-only `status`/`log`/`rev-parse`/`rev-list`/`branch`/`for-each-ref`; `.git/FETCH_HEAD` mtime Jul 1 02:22 predates session): **CONFIRMED**.
- **Every census expression recorded verbatim** beside its count (R1, R3, baseline pins): **CONFIRMED**.
- **Violation counts labelled as upper-bound proxies** (R3 header + per-row refined columns + explicit raw-vs-refined aggregate framing): **CONFIRMED**.

Bez kostylei — the T3 proxy resolves the gate to a single lever (DFK002 / Runtime.Native), so the Phase β brief can pre-commit the ≤80 single-cascade path and stage that one architectural decision, rather than discovering the branch at Phase β closure.
