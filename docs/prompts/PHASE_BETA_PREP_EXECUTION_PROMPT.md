# Phase β-prep Execution Prompt — A'.9.1 Analyzer Infrastructure Cascade

**Target session**: Fresh-context Claude Code (Opus 4.7, 1M context window)
**Working directory**: `D:\Colony_Simulator\Colony_Simulator`
**Branch baseline**: `main` at `a23556f1fae33a819ff4fc40359a2f46c4015c1b` (post Phase α Commit 9 — 9 commits ahead of `origin/main`, push deferred к Phase δ closure per Crystalka decision)
**Date authored**: 2026-05-25
**Authored by**: Claude Opus 4.7 (deliberation session post Phase α execution completion)

---

## §0 — Role + mission

You are **Claude Code (Opus 4.7)** executing **Phase β-prep** of cascade **A'.9.1 / К-extensions cascade #5 — Analyzer Infrastructure**.

**Phase β-prep scope** (this session): Implement 17 stub analyzers + wire `ProjectReference` into 5 src/ analyzer-scoped projects + run `dotnet build` full solution + enumerate violation count per rule. **STOP after violation count enumerated and reported к Crystalka.**

**Phase β-prep is NOT Phase β cleanup-phase**. Cleanup-phase methodology (per-rule violation triage + suppression CAPA cascade + severity promotion) cannot start until Q-L-1 adaptive gate decision made:
- ≤80 violations → continue single A'.9.1 cascade с Phase β cleanup + Phase γ promotion + Phase δ closure
- 80-150 → Crystalka decision (hybrid split point)
- >150 → split: A'.9.1 closes Phase β-prep here; A'.9.1b standalone cascade for full Phase β + γ + δ

**Out of scope for THIS session**:
- Cleanup phase (per-rule violation triage) — happens after Q-L-1 gate decision
- Severity promotion (.editorconfig edits) — Phase γ
- Closure cascade governance (KERNEL chronicle v2.5.4 + LEDGER §3.6 + EVIDENCE #14) — Phase δ
- Push к origin/main — deferred per Crystalka decision к Phase δ closure
- Q-L deliberation re-opening — all forward-locked decisions ratified

---

## §1 — Reading order (mandatory before any code change)

### 1.1 — Primary authoritative inputs (full read)

1. **`docs/architecture/ANALYZER_RULES.md` v0.1 AUTHORED-SKELETON** (~24 KB, ~330 lines post-Commit 5 structural reorganization)
   - **AUTHORITATIVE rule enumeration source** — §4 first-batch active rules (17 rules total)
   - §4.6 enforcement surface total с arithmetic rationale (resolves Brief §1.3 «15-16» discrepancy)
   - §10.5 forward implementation plan
   - Full read

2. **`tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` v1.0 AUTHORED** — §7 (Phase β scope) + §10.3 (Phase β rule implementation order)
   - Use `view_range` targeted reads только для §7 (~120 lines) + §10.3 (~60 lines)
   - DO NOT re-read full brief — Phase α executed it already

3. **`tools/briefs/A_PRIME_9_1_PHASE_0_CLOSURE_REPORT.md` v1.0 AUTHORED** — §2.7 (Task 7 deferred — circular dependency rationale)
   - Use `view_range` targeted read только §2.7 + §7 (Q-L-1 adaptive gate disposition)

### 1.2 — Empirical state verification (mandatory before Commit 1)

4. **`tools/DualFrontier.Analyzers/AnalyzerEntryPoint.cs`** — full read
   - Placeholder marker type — confirms namespace `DualFrontier.Analyzers` + base structure
   - Stub analyzers go into same project alongside this file

5. **`tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj`** — full read
   - Verify netstandard2.0 + Microsoft.CodeAnalysis.CSharp + Microsoft.CodeAnalysis.Analyzers references present
   - Note inline rationale comment about dropped ProjectReference к Contracts (analyzers use FQN string matching per canonical Roslyn pattern)

6. **`tests/DualFrontier.Analyzers.Tests/`** directory listing + `DualFrontier.Analyzers.Tests.csproj` + any existing test files
   - Verify Workspaces 5.3.0 override present
   - Locate PlaceholderTests.cs OR equivalent для test pattern reference

7. **`Directory.Packages.props`** — full read
   - Verify all 4 Roslyn package versions central-pinned (CSharp 5.3.0 + Analyzers 5.3.0 + Workspaces 5.3.0 + Analyzer.Testing.XUnit 1.1.2)

8. **`src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs` + `ReservedStubPurpose.cs`** — full reads
   - Confirm attribute class FQN = `DualFrontier.Contracts.Analyzer.ReservedStubAttribute`
   - This FQN string used for DFL025-A detection per Phase β rule implementation

### 1.3 — К-L canonical reference (read only when implementing specific rule)

When implementing a specific DFK### rule, read its К-L source row at:
- **`docs/architecture/KERNEL_ARCHITECTURE.md` Part 0** — К-L# row verbatim (Tier 1 LOCKED — DO NOT MODIFY per S-LOCK-1)
- **`docs/architecture/K_CLOSURE_REPORT.md` §7** — canonical detection narrative per rule (Tier 1 LOCKED)

Use `view_range` targeted reads only. Both are large files; do not read full.

### 1.4 — Production code surface verification (per-rule basis)

When implementing DFK### rule, also read **the src/ file containing the К-L target pattern** to confirm anchor exists empirically. Per Phase 0 closure report §3 grounded anchors:

| Rule | Empirical anchor file |
|---|---|
| DFK002 (К-L2 P/Invoke) | `src/DualFrontier.Core.Interop/*.cs` (P/Invoke surface) |
| DFK005 (К-L5 declarative bootstrap) | `src/DualFrontier.Core.Interop/Bootstrap.cs` |
| DFK007.1 (К-L7.1 GPU pipeline slot) | `src/DualFrontier.Core.Interop/PipelineSlotInterop.cs` |
| DFK011 (К-L11 NativeWorld SSoT) | `src/DualFrontier.Application/Loop/GameBootstrap.cs` (L76 NativeWorld instantiation) |
| DFK013 (К-L13 wake_type discipline) | `src/DualFrontier.Contracts/Scheduling/WakeAttributes.cs` |
| DFK016 (К-L16 pipeline depth) | `src/DualFrontier.Core.Interop/PipelineSlotInterop.cs` (DefaultDepth/MaxDepth) |
| DFL025-A (ReservedStub behavior invocation) | `src/DualFrontier.Launcher/RenderCommandDispatcher.cs` (3 cascade #3 sites) + 31 Pattern A sites from Commit 7 |

NOTE: Phase β-prep ships **stub analyzers only** (return zero diagnostics). Empirical anchor verification matters for Phase β cleanup-phase, не для Phase β-prep stubs. Listed here as forward reference.

---

## §2 — Rule enumeration (17 stubs per ANALYZER_RULES §4 + §10)

Per `docs/architecture/ANALYZER_RULES.md` §4.6 enforcement surface total: **17 own rules**.

### 2.1 — P0 Critical (8 rules — `DualFrontier.Architecture` / `NativeBoundary`)

| Rule ID | К-L | Category | Severity (post-promotion) | Suggested class name |
|---|---|---|---|---|
| DFK001 | К-L1 | NativeBoundary | Error | `DFK001NativeLanguageAnalyzer` |
| DFK002 | К-L2 | NativeBoundary | Error | `DFK002PInvokeBindingsAnalyzer` |
| DFK003 | К-L3 | Architecture | Error | `DFK003StorageOwnershipAnalyzer` |
| DFK003.1 | К-L3.1 | Architecture | Error | `DFK003_1StorageBridgeAnalyzer` |
| DFK004 | К-L4 | Architecture | Error | `DFK004TypeIdRegistryAnalyzer` |
| DFK005 | К-L5 | Architecture | Error | `DFK005DeclarativeBootstrapAnalyzer` |
| DFK007 | К-L7 | Architecture | Error | `DFK007SpanProtocolAnalyzer` |
| DFK011 | К-L11 | Architecture | Error | `DFK011NativeWorldSsotAnalyzer` |

### 2.2 — P1 High (4 rules — post-Q-L-9 supersession)

| Rule ID | К-L | Category | Severity | Suggested class name |
|---|---|---|---|---|
| DFK007.1 | К-L7.1 GPU pipeline slot | NativeBoundary | Error | `DFK007_1GpuPipelineSlotAnalyzer` |
| DFK015.1 | К-L15.1 three-tier mutex managed facade | NativeBoundary | Error | `DFK015_1ThreeTierMutexFacadeAnalyzer` |
| DFK017 | К-L17 display composition multi-layer | Architecture | Error | `DFK017DisplayCompositionAnalyzer` |
| DFK019.A | К-L19 static API surface | NativeBoundary | Error | `DFK019_AStaticVulkanApiAnalyzer` |

### 2.3 — Phase β secondary (2 rules — Warning severity)

| Rule ID | К-L | Category | Severity | Suggested class name |
|---|---|---|---|---|
| DFK013 | К-L13 on-demand activation | Architecture | Warning | `DFK013WakeTypeDisciplineAnalyzer` |
| DFK016 | К-L16 pipeline depth | Architecture | Warning | `DFK016PipelineDepthAnalyzer` |

### 2.4 — Lesson-derived Discipline (2 rules — DFL025 family)

| Rule ID | Lesson | Category | Severity | Suggested class name |
|---|---|---|---|---|
| DFL025-A | #25 refined 3rd extension | Discipline | Warning | `DFL025_AReservedStubInvocationAnalyzer` |
| DFL025-B | #25 refined 3rd extension | Discipline | Suggestion | `DFL025_BStandaloneSkipAnalyzer` |

### 2.5 — Self-policing (1 rule — Q-L-18 default ship at A'.9.1)

| Rule ID | Lesson | Category | Severity | Suggested class name |
|---|---|---|---|---|
| DF999 | (analyzer self-discipline) | Discipline | Warning | `DF999GlobalSuppressionBanAnalyzer` |

**Total: 17 stub analyzers.**

NOTE on class naming convention: `_1` substitution для dotted IDs (DFK007.1 → `DFK007_1...`). Dot is invalid identifier character в C#. Alternative: `DFK007Point1...` if preferred. Pick one consistent convention для all 4 dotted IDs (DFK003.1, DFK007.1, DFK015.1, DFK019.A) and document choice в Commit 1 rationale.

---

## §3 — Stub analyzer template (each of 17 rules — identical pattern)

Each stub analyzer is a minimal `DiagnosticAnalyzer` subclass that:
1. Declares a single `DiagnosticDescriptor` с rule ID + title placeholder + message format placeholder + category + severity per §2 tables
2. Reports no diagnostics (empty `Initialize` or `Initialize` that registers no actions)
3. Includes XML documentation referencing К-L# OR Lesson# canonical source
4. Includes inline `// Phase β cleanup-phase will populate detection logic` comment

### 3.1 — Stub template (verbatim — substitute per-rule placeholders)

```csharp
// tools/DualFrontier.Analyzers/Rules/<Category>/<ClassName>.cs
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DualFrontier.Analyzers.Rules.<Category>;

/// <summary>
/// <Rule short description — К-L# OR Lesson# canonical reference>
/// </summary>
/// <remarks>
/// <para>
/// Phase β-prep stub — declares <c>DiagnosticDescriptor</c> for build-time wiring
/// + violation count enumeration (Q-L-1 adaptive gate). Detection logic populated
/// at Phase β cleanup-phase per Brief A'.9.1 §10.5 + ANALYZER_RULES.md §<2.x>.
/// </para>
/// <para>
/// Canonical К-L invariant text: <see href="...KERNEL_ARCHITECTURE.md#part-0">КERNEL_ARCHITECTURE.md Part 0 К-L#</see>.
/// Canonical detection narrative: <see href="...K_CLOSURE_REPORT.md#7">K_CLOSURE_REPORT.md §7.x</see>.
/// </para>
/// <para>
/// К-L14 thesis: tooling addition; zero substrate touch.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class <ClassName> : DiagnosticAnalyzer
{
    public const string DiagnosticId = "<RuleId>";  // e.g., "DFK001"

    private static readonly LocalizableString Title =
        "<Rule short title>";  // e.g., "К-L1 native language discipline"

    private static readonly LocalizableString MessageFormat =
        "<Diagnostic message format string with {0} placeholders>";

    private static readonly LocalizableString Description =
        "<3-5 sentence detection narrative per ANALYZER_RULES.md §2 template>";

    private const string Category = "<DualFrontier.Architecture | DualFrontier.NativeBoundary | DualFrontier.Discipline>";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: Title,
        messageFormat: MessageFormat,
        category: Category,
        defaultSeverity: DiagnosticSeverity.<Error | Warning | Info>,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: "https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#<rule-id-lowercase>");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Phase β cleanup-phase populates detection logic here.
        // Stub returns zero diagnostics при build time.
    }
}
```

### 3.2 — Severity mapping (Microsoft.CodeAnalysis enum)

| Brief severity | DiagnosticSeverity enum |
|---|---|
| Error (post-promotion) | `DiagnosticSeverity.Error` (Phase β-prep stubs may use `.Info` if `.Error` triggers RS1015 helpLinkUri checks при build) |
| Warning | `DiagnosticSeverity.Warning` |
| Suggestion | `DiagnosticSeverity.Info` |

**Recommendation для Phase β-prep**: all 17 stubs use `DiagnosticSeverity.Info` (= IDE «Suggestion») by default. Phase γ severity promotion shifts к brief-specified severity per ANALYZER_RULES §4. This protects Phase β-prep from accidental build breakage due к analyzer-side warnings being elevated к errors by `TreatWarningsAsErrors=true` inheritance.

**Alternative**: use `defaultSeverity` per ANALYZER_RULES §4 + override via `.editorconfig` к `suggestion` для cleanup-phase mode. Both approaches valid; choose one consistent.

### 3.3 — Category strings (verbatim — Q-L-7 ratification)

Use exactly these category strings (case-sensitive, dot-namespaced):

- `"DualFrontier.Architecture"` (10 rules: DFK003, DFK003.1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017)
  - Wait — that's 9. Verify: DFK003, DFK003.1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017 = 9 rules. Correct.
- `"DualFrontier.NativeBoundary"` (6 rules: DFK001, DFK002, DFK007.1, DFK015.1, DFK019.A)
  - Wait — that's 5. Verify against §2.1 + §2.2: DFK001, DFK002 (P0) + DFK007.1, DFK015.1, DFK019.A (P1) = 5 rules. Correct.
- `"DualFrontier.Discipline"` (3 rules: DFL025-A, DFL025-B, DF999)

Total: 9 + 5 + 3 = **17 rules.** ✓ matches §2.5 total.

---

## §4 — Atomic commit plan для Phase β-prep

Per brief §2.4 «pre-flight grep AD» discipline + Lesson #N13 commit integrity verification.

### 4.1 — Suggested commit decomposition (3-5 atomic commits)

**Commit β1 — `analyzer(stubs-architecture)`**: 9 stubs in `DualFrontier.Architecture` category
- Files: 9 new files в `tools/DualFrontier.Analyzers/Rules/Architecture/*.cs`
- Verification: `dotnet build` exit 0; no new diagnostics emitted (stubs return zero)

**Commit β2 — `analyzer(stubs-nativeboundary)`**: 5 stubs in `DualFrontier.NativeBoundary` category
- Files: 5 new files в `tools/DualFrontier.Analyzers/Rules/NativeBoundary/*.cs`
- Verification: `dotnet build` exit 0

**Commit β3 — `analyzer(stubs-discipline)`**: 3 stubs in `DualFrontier.Discipline` category
- Files: 3 new files в `tools/DualFrontier.Analyzers/Rules/Discipline/*.cs`
- Verification: `dotnet build` exit 0

**Commit β4 — `analyzer(wiring)`**: ProjectReference wiring в 5 src/ analyzer-scoped projects
- Files: 5 csproj modifications (existing src/ projects)
- Per brief §6.1 ProjectReference pattern (analyzer-only reference, no assembly output):
  ```xml
  <ItemGroup>
    <ProjectReference Include="..\..\tools\DualFrontier.Analyzers\DualFrontier.Analyzers.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false" />
  </ItemGroup>
  ```
- Target projects per Q-L-5 (managed-side scope):
  - `src/DualFrontier.Core/DualFrontier.Core.csproj`
  - `src/DualFrontier.Application/DualFrontier.Application.csproj`
  - `src/DualFrontier.Core.Interop/DualFrontier.Core.Interop.csproj`
  - `src/DualFrontier.Launcher/DualFrontier.Launcher.csproj`
  - `src/DualFrontier.Modding/DualFrontier.Modding.csproj` (if exists) OR equivalent application-scheduler/modding project per Phase α empirical state
- Verification: `dotnet build` full solution exit 0
- **CRITICAL**: with 17 analyzers wired but returning zero diagnostics, full solution build must produce zero new warnings/errors. If build fails, **HALT** — analyzer infrastructure issue surfaces.

**Commit β5 — OPTIONAL — `analyzer(violation-count-baseline)`**: Generate violation count baseline report
- File: `tools/briefs/A_PRIME_9_1_PHASE_B_PREP_VIOLATION_COUNT_REPORT.md` (NEW)
- Content: `dotnet build` output captured + per-rule violation histogram (zero для stubs) + Q-L-1 gate disposition surfaced
- THIS commit OPTIONAL — primary purpose is generating data для Crystalka decision; can be captured в session report instead of repo artifact

### 4.2 — Empirical 5-project discovery (Phase β-prep mandatory step)

Phase β-prep agent MUST verify the 5 src/ analyzer-scoped projects empirically. Use:

```bash
ls src/
find src/ -name '*.csproj' -not -path '*/bin/*' -not -path '*/obj/*'
```

Compare against brief §6.1 mention of «5 src projects of 12 — Core + Application + Application.Scheduler + Application.Modding + Core.Scheduling». If empirical state differs (e.g., `DualFrontier.Modding` instead of `DualFrontier.Application.Modding`), surface к Crystalka before Commit β4. Brief estimate may be empirically off (precedent: Phase α Commit 3 CPM migration count 11 vs ~30 estimate).

---

## §5 — Forward-locked constraints (S-LOCK reminders)

Phase β-prep must preserve all 14 S-LOCKs from brief §5. Critical ones для this session:

- **S-LOCK-1**: Zero K-Lxx invariant text change в KERNEL_ARCHITECTURE.md Part 0
- **S-LOCK-2**: Scope ≤ managed-side enforcement (no native-side rules)
- **S-LOCK-4**: First-batch rule count = 17 (per ANALYZER_RULES §4.6 — supersedes brief «15-16» imprecision)
- **S-LOCK-5**: tests/DualFrontier.Analyzers.Tests/ plural naming preserved (do not rename)
- **S-LOCK-6**: NO CodeFixProvider classes (PA-001 axiom permanent)
- **S-LOCK-13**: ANALYZER_RULES.md DF→DFK rename preserved (no historical reversion)
- **S-LOCK-14**: PROJECT_AXIOMS.md v1.0 Tier 1 LOCKED untouched

**Phase β-prep specific verification**: each of 17 stub analyzers MUST have:
- Unique `DiagnosticId` (no duplicates)
- Valid `helpLinkUri` к `https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#<rule-id-lowercase>` (per Commit 6 GitHub URL correction)
- Correct category string (verbatim per Q-L-7 — `"DualFrontier.Architecture"` / `"DualFrontier.NativeBoundary"` / `"DualFrontier.Discipline"`)
- Empty `Initialize` body (или explicitly «registers nothing» per stub semantic)

---

## §6 — Violation count enumeration protocol (Q-L-1 adaptive gate)

After Commits β1-β4 land, execute violation count enumeration:

### 6.1 — Build + diagnostic capture

```powershell
# From repo root
dotnet build DualFrontier.sln 2>&1 | Tee-Object -FilePath build_phase_beta_prep.log

# Filter for DFK### / DFL### / DF999 diagnostics
Select-String -Path build_phase_beta_prep.log -Pattern "DF[KL]?\d{3}" | 
  ForEach-Object { $_.Line } | 
  Sort-Object | 
  Group-Object | 
  Select-Object Name, Count |
  Sort-Object Count -Descending
```

Expected result (stubs only): **zero diagnostics emitted** (since `Initialize` registers nothing). If non-zero, stub implementation has bug — fix before proceeding.

### 6.2 — Violation count gate (Q-L-1 forecast — stubs only baseline)

For Phase β-prep с STUB analyzers, expected violation count = **0** (stubs return zero diagnostics).

This is the **baseline state** — Phase β-prep proves analyzer infrastructure wires correctly into 5 src/ projects WITHOUT breaking build, providing empirical foundation для Phase β cleanup-phase detection logic implementation.

### 6.3 — Q-L-1 gate decision surface

When Phase β cleanup-phase agent implements actual detection logic (per-rule), the violation count then becomes meaningful. Phase β-prep's role is providing the **wired-analyzer-infrastructure baseline** so Phase β cleanup-phase can:
1. Implement DFK001 detection logic → `dotnet build` shows N1 DFK001 violations
2. Implement DFK002 detection logic → `dotnet build` shows N2 DFK002 violations (cumulative N1+N2)
3. ... continue per §10.3 implementation order
4. Final violation count = sum of all 17 rules' diagnostic emissions
5. **Apply Q-L-1 gate at final cumulative count**:
   - ≤80 → continue single cascade с Phase γ severity promotion
   - 80-150 → Crystalka decision (hybrid split)
   - >150 → split к A'.9.1b

**Architectural insight**: Phase β-prep is **decoupled** from Q-L-1 gate. The gate triggers at cumulative cleanup-phase boundary, not at Phase β-prep stub-only baseline.

### 6.4 — Phase β-prep exit gate

Phase β-prep STOPS when:
- All 17 stubs compile clean
- ProjectReference wiring complete (5 src/ projects)
- `dotnet build DualFrontier.sln` exits 0 с zero new diagnostics
- All commits clean + working tree clean
- Violation count report generated (zero baseline confirmed)

**Surface к Crystalka**:
- Phase β-prep complete; analyzer infrastructure wired; zero baseline confirmed
- Phase β cleanup-phase next: implement detection logic per rule per §10.3 implementation order
- Per-rule implementation can be single session (если scope small) OR multiple sessions (если scope large per rule's К-L surface)
- Crystalka direction для cleanup-phase session shape

---

## §7 — Per-commit verification report template

After EACH commit, output a brief status report:

```
COMMIT β{N}/{4-5} — {scope}: {one-line description}
SHA: {commit SHA from git log -1}
Files touched: {N files}
LOC delta: +{X} / -{Y}
Stubs introduced: {N stubs} ({rule IDs})
Pre-flight grep AD: {pass/fail с findings}
Verification gates:
  - dotnet build DualFrontier.sln: exit 0
  - DFK###/DFL###/DF999 diagnostics emitted: 0 (stubs baseline)
К-L14 checklist: ☑ zero substrate / ☑ zero production semantic / ☑ zero test semantic / ☑ tooling-additive only
Next: Commit β{N+1} OR Phase β-prep exit gate verification
```

After final commit (β4 OR β5):

```
PHASE β-PREP EXECUTION COMPLETE
Total commits: 4-5
Commits ahead of origin/main: 13-14 (Phase 0 cascade 2 + Phase α 9 + Phase β-prep 4-5)
Working tree: clean
Solution build: dotnet build DualFrontier.sln exit 0 (zero new diagnostics)
Stubs operational: 17 analyzers wired, zero baseline
Phase β cleanup-phase ready: per-rule detection logic implementation per §10.3 order
Surface к Crystalka: cleanup-phase session shape decision
К-L14 Evidence #14 candidate: tooling infrastructure complete + wired; cleanup-phase will produce full Evidence at Phase δ closure
```

---

## §8 — Halt conditions

Stop and surface к Crystalka if ANY of the following:

1. **5-project discovery mismatch** — empirical src/ analyzer-scoped projects differ from brief §6.1 enumeration. Surface inventory before Commit β4 wiring.

2. **Stub build failure** — `dotnet build tools/DualFrontier.Analyzers/DualFrontier.Analyzers.csproj` exits non-zero after stub implementation. Likely RS1015 helpLinkUri OR analyzer authoring rule violation. Read error message; if not immediately fixable, halt.

3. **Wired build failure** — `dotnet build DualFrontier.sln` exits non-zero after Commit β4 ProjectReference wiring. Most likely: analyzer assembly load failure (netstandard2.0 vs net8.0 compatibility issue surfaces here, not at csproj build time). Investigate per Phase α Commit 1 rationale comment.

4. **Non-zero diagnostic emission from stubs** — stubs are supposed к emit zero diagnostics. If `dotnet build` log shows any DFK###/DFL###/DF999 emissions при Phase β-prep stage, stub implementation has bug. Fix before proceeding.

5. **Category string mismatch** — if any stub uses category string differing from `"DualFrontier.Architecture"` / `"DualFrontier.NativeBoundary"` / `"DualFrontier.Discipline"` (verbatim per Q-L-7), surface — this would silently broken cleanup-phase .editorconfig per-category configuration.

6. **Class naming convention inconsistency** — pick `_1` OR `Point1` for dotted IDs (DFK003.1 → `DFK003_1...` OR `DFK003Point1...`). Apply consistently across all 4 dotted IDs. If inconsistency surfaces, halt + ratify pattern с Crystalka before continuing.

7. **DFL025-A FQN string mismatch** — DFL025-A stub references `"DualFrontier.Contracts.Analyzer.ReservedStubAttribute"` FQN for forward cleanup-phase detection. If FQN differs from actual attribute namespace (per Commit 6 empirical state), surface inconsistency before proceeding.

---

## §9 — Architectural framing reminders

**К-L14 thesis preservation**: Phase β-prep is **infrastructure wiring** — pure tooling addition. No К-L# canonical text change. No production code semantic change (only ProjectReference additions в csproj files). К-L14 thesis preserved trivially.

**Lesson #N13 commit integrity**: each commit's `git diff --staged` MUST match commit message claims. If commit message says «3 stubs Discipline category», staged content shows exactly 3 new files в `Rules/Discipline/`, not more not less.

**Lesson #N14 empirical state coverage** (5th application now — Phase β-prep continuation of N14 chain through cascade):
- Phase 0 = 4th application (path corrections + 31-site grep gap surfacing)
- Phase β-prep = 5th application (5-project discovery + violation count baseline)
- Each application strengthens FORMALIZE candidacy

**PA-002 без костылей**: stub analyzers must be **honest stubs** — not fake-detection logic that emits arbitrary diagnostics. Zero-emission baseline is the architecturally clean foundation.

**PA-003 архитектурная сложность всегда оправдана**: 17 stub classes is non-trivial scope, но architectural justification = Phase β cleanup-phase implementation surface predetermined. Cleaner to ship all 17 stubs at infrastructure baseline than incrementally add per cleanup-phase session.

---

## §10 — Quick reference

**Working dir**: `D:\Colony_Simulator\Colony_Simulator`
**Branch baseline SHA**: `a23556f1fae33a819ff4fc40359a2f46c4015c1b`
**Authoritative rule enumeration**: `docs/architecture/ANALYZER_RULES.md` §4.6 = **17 rules**
**Target stub directory**: `tools/DualFrontier.Analyzers/Rules/{Architecture|NativeBoundary|Discipline}/`
**Class naming convention**: pick `_1` OR `Point1` для dotted IDs; apply consistently
**Default stub severity**: `DiagnosticSeverity.Info` (safe baseline)
**Repo URL для helpLinkUri**: `https://github.com/Crystalka228/Dual-Frontier`
**ReservedStub FQN для DFL025-A**: `DualFrontier.Contracts.Analyzer.ReservedStubAttribute`
**Push к origin/main**: NOT in this session scope; deferred к Phase δ closure
**Q-L-1 adaptive gate**: triggers at Phase β cleanup-phase final cumulative violation count, NOT at Phase β-prep stub baseline

---

**Begin Phase β-prep execution. Read inputs §1.1-1.2 first. Then proceed к Commits β1-β4 per §4.1 decomposition.**

**К-L14 thesis preserved. PA-001 + PA-002 + PA-003 + PA-004 axioms applied. Без костылей.**
