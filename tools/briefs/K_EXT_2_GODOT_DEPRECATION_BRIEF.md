# К-extensions cascade #2 — Godot Full Deprecation + Launcher Formalization

**Brief designation**: `K_EXT_2_GODOT_DEPRECATION_BRIEF`  
**Cascade designation**: К-extensions cascade #2 (post-А'.8 К-closure)  
**Authoring date**: 2026-05-23  
**Authoring session**: Deliberation Session 2026-05-23 (Crystalka + Claude Opus 4.7)  
**Status**: AUTHORED — pending Crystalka RATIFICATION + Claude Code execution  
**Execution branch**: New feature branch off `9ea5dbe` (current main = origin/main post-А'.8 closure)

---

## §0 — Timeline + provenance

### 0.1 К-extensions cascade #2 surfacing

К-extensions cascade #2 was originally **anticipated** in А'.8 K-closure deliberation Session 1 (2026-05-20/21) per Q-N-8-11 forward sequencing. At that time scope was framed as «Godot removal merge» — completion of work started on branch `claude/godot-removal-deliberation-Vfg2R` (`2ba8130`, single commit, 2026-05-21).

### 0.2 Original Godot branch + А'.7.5 deferral

Branch `2ba8130` was created 2026-05-21 evening as parallel cascade alongside А'.7.x BUS_ARCHITECTURE_AMENDMENT. Per Phase 0 reads on that branch, original scope was **incomplete** — tracked file removal only (67 files, +394/-2349), без disk-level untracked residue cleanup, документация Godot references, или launch path migration. 

Per Crystalka direction 2026-05-22 (post-А'.7.5 execution), Godot branch was **deferred к post-К-closure merge** — became designation К-extensions cascade #2 awaiting Crystalka discretion timing.

А'.7.5 merged к origin/main alone (`fe5a871`). Godot branch CLOSED but NOT merged.

### 0.3 А'.8 K-closure completion

А'.8 К-series formal closure executed 2026-05-23, 6 atomic commits `044855c..9ea5dbe`, pushed к origin/main. К-closure event boundary established. Phase A' formally complete.

Forward sequencing post-closure prioritized К-extensions cascade #2 (Godot merge) as first track per Q-N-8-11.

### 0.4 Cascade #2 scope expansion 2026-05-23 deliberation

In К-extensions cascade #2 deliberation session 2026-05-23, Crystalka **expanded scope** beyond original «merge Godot branch» framing:

> «Физически удалить всё связанное с Godot в проекте убрав все артефакты, формально записать в документы проекта что годот не используется и поставить пометку историческое(архив), потом в той же сессии перевести после чистки запуск проекта в win32 или Vulcan раз Godot больше не будет»

This expanded scope (физический purge + документация markup + launch path migration) made original Godot branch **insufficient precursor**. Per Crystalka direction:

> «Тогда сделам лучше, так как в main ветке прошли большие изменения, то я верну сейчас main и уже в нём мы сделаем чистую новую чистку, и перепривяжем запуск»

**Decision**: Discard original Godot branch (`2ba8130`) as obsolete precursor. Execute clean cascade off current main (`9ea5dbe`).

### 0.5 Scope ratification — (c) Maximal

Crystalka ratified **(c) Maximal — Full Application/Bridge refactor** scope:

> «(c) Maximal — Full Application/Bridge refactor, так как будут внедряться много новых технологий и лучше заранее всё очистить, это даст максимальное понимание в случае ошибок в будущем»

Forward V-series + FO-series + Phase B M-cycle work justified maximal cleanup now to avoid carrying technical debt forward.

### 0.6 Single cascade structure ratification — Option X

Crystalka ratified **Option X — Single large cascade в одной execution session**:

> «Окно контекста в сесии Claude Code 1 миллион токенов можем сделать всё в одной сессии, это как раз даст чистоту к последующему внедрению анализатора»

Lesson #2 K-Lessons (milestone consolidation under session-mode pipeline) applies — 1M context window resolves split necessity. Plus cleanup→analyzer adjacency provides clean baseline для A'.9 Roslyn analyzer rule definition.

### 0.7 Brief 1 + Brief 2 split

During Q-G-6 (Launcher project structure) deliberation, Crystalka determined visual implementation scope warrants **separate cascade #3**:

> «(c) Maximal — Program + LauncherRenderer + RenderCommandDispatcher + scene state. Тут мы сделаем тогда отдельную сесcию после этой.»

**Decision**: This brief (К-extensions cascade #2) ships **infrastructure-only** Launcher per (b1) functional bar — defensive throw stubs for visual dispatch per Lesson #N12 first application. Cascade #3 будет **separate brief** (`K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md`) authored deliberation framework form, executed post-cascade-#2 closure.

### 0.8 Deliberation session Q-N timeline

15 Q-N ratified в deliberation session 2026-05-23:

| Q | Decision | Section |
|---|---|---|
| Q-G-0 | Option X (single large cascade) | Cascade shape |
| Q-G-1 | (c) Keep IDevKitRenderer dormant, decoupled from Godot | Architectural |
| Q-G-2 | (a) Remove Presentation.Native outright | Architectural |
| Q-G-3 | (a) IRenderCommand as pure marker, drop Execute() | Architectural |
| Q-G-4 | SUBSUMED by Q-G-3 | Architectural |
| Q-G-5 | (a) No new К-L + Lesson #N12 candidate captured | Architectural |
| Q-G-6 | (b)+(i)+(b1) Standard launcher с infrastructure-only с Lesson #N12 first application | Launcher design |
| Q-G-7 | (d) Hybrid orchestration в Program.cs | Launcher design |
| Q-G-8 | (b) Executable-focused config | Launcher design |
| Q-G-9 | (a) Keep SmokeTest as is | Launcher design |
| Q-G-10 | (d) Tiered scan с conditional expansion | Documentation |
| Q-G-11 | (d) Separate K_EXTENSIONS_LEDGER.md companion | Documentation |
| Q-G-12 | (b) Patch bump v2.5 → v2.5.1 + versioning convention | Governance |
| Q-G-13 | (d) Hybrid 3-commit REGISTER cascade | Governance |
| Q-G-14 | (d) Honest-framed removal-type К-L14 verification #11 | Verification |

---

## §1 — S-LOCK reservation surface

S-LOCKs ratified post-Q-N deliberation. Brief execution **MUST NOT violate** any S-LOCK without halt + Crystalka re-ratification.

### S-LOCK-1 — Discard Godot branch (`2ba8130`) as obsolete precursor

**Statement**: Original Godot branch `claude/godot-removal-deliberation-Vfg2R` (`2ba8130`, 1 commit, 67 files +394/-2349) is treated as **obsolete precursor**. NO rebase, NO cherry-pick, NO merge of original branch к main.

**Rationale**: Original scope incomplete (tracked file removal only); 18 commits divergence на main since branch base (А'.7.x + А'.7.5 + А'.8 closure); clean redo на current main avoids mechanical resolution complexity + preserves cascade coherence per Q-G-0 Option X ratification.

**Forward action**: Original branch может be deleted post-cascade-closure (Crystalka discretion) или kept as historical artifact. Either acceptable.

### S-LOCK-2 — К-extensions cascade #2 designation preserved

**Statement**: This cascade IS К-extensions cascade #2 per А'.8 closure Q-N-8-11 designation. NO renumbering, NO relabeling, NO promotion к milestone designation despite expanded scope.

**Rationale**: Cascade #2 designation established at А'.8 closure. Scope expansion от original «Godot merge» к «Godot full deprecation + Launcher» does NOT change cascade identity. Designation continuity preserves К-extensions ledger numbering.

### S-LOCK-3 — Infrastructure-only Launcher per Q-G-6 (b1)

**Statement**: Launcher project ships **infrastructure-only** в cascade #2:
- Project scaffold (.csproj + sln integration)
- Program.cs composition root с (d) hybrid orchestration loop
- LauncherRenderer implementing IRenderer (substrate wrapper)
- RenderCommandDispatcher с **defensive throws** per Lesson #N12 first application

NO visual implementation. NO SpriteCatalog. NO scene state. NO real dispatch bodies. Visual implementation explicitly deferred к К-extensions cascade #3 (separate brief).

**Rationale**: Per Q-G-6 (b1) functional bar + Crystalka direction 2026-05-23 «тут мы сделем тогда отдельную сесиию после этой».

**Halt condition**: If execution agent attempts к add visual dispatch implementation, halt + Crystalka ratification required.

### S-LOCK-4 — Defensive Reserved Stub Pattern (Lesson #N12) first application

**Statement**: ALL RenderCommandDispatcher dispatch arm methods MUST throw `NotImplementedException` с **descriptive message** linking к cascade #2 + Lesson #N12 + forward cascade #3 reference. NO empty bodies, NO no-op stubs, NO default-value returns.

**Canonical pattern**:
```csharp
private void HandlePawnSpawned(PawnSpawnedCommand cmd) =>
    throw new NotImplementedException(
        "PawnSpawned dispatch pending К-extensions cascade #3. " +
        "If this throws в test, the test is exercising visual rendering path " +
        "что cascade #2 explicitly scoped out (Lesson #N12 Defensive Reserved Stub Pattern).");
```

**Rationale**: Lesson #25 refined (lying-test prevention) + Lesson #N12 first application (defensive throw documents not-yet-implemented status loudly). Empty bodies would pass tests that exercise dispatch paths → lying-test surface → architectural debt.

**Halt condition**: If execution agent generates empty Execute()/dispatch bodies, halt + brief amendment required.

### S-LOCK-5 — IRenderCommand as pure marker interface

**Statement**: `IRenderCommand` interface stripped к pure marker — NO Execute() method, NO body methods of any kind. All Bridge.Commands records lose Execute() implementations.

**Canonical pattern after change**:
```csharp
public interface IRenderCommand
{
}

public sealed record PawnSpawnedCommand(EntityId PawnId, float X, float Y) : IRenderCommand;
```

**Rationale**: Q-G-3 LOCKED. Empty Execute() bodies were lying-test surface (Lesson #25 refined). Structural elimination через marker-only interface. Dispatcher pattern (Launcher's RenderCommandDispatcher) is real dispatch mechanism.

**Halt condition**: If execution agent retains Execute() в any form (even с throw), halt + verification — pattern requires marker-only.

### S-LOCK-6 — IDevKitRenderer dormant + Godot-decoupled

**Statement**: `IDevKitRenderer` interface preserved structurally (NOT deleted) but **fully decoupled** from Godot framing:
- XML docstring rewritten per Q-G-1 spec
- No implementations provided in cascade #2 (interface-only)
- `[DevKitOnly]` attribute message updated к "Reserved DevKit surface — not shipped to players. Currently dormant."

**Rationale**: Q-G-1 LOCKED. Crystalka plans first-party DevKit later — preserves architectural intent (DevKit/Production split). Interface-only structurally cannot generate lying tests (Lesson #25 refined). Future DevKit будет implement над Vulkan substrate (Runtime), не extend Runtime.

**Halt condition**: If execution agent deletes IDevKitRenderer or adds stub implementation, halt + brief amendment required.

### S-LOCK-7 — Presentation.Native full removal

**Statement**: `src/DualFrontier.Presentation.Native/` directory + all contents (NativeRenderer.cs, NativeSceneLoader.cs, NativeInputHandler.cs, .csproj, README.md, untracked .cs.uid orphans) deleted. Project removed from `DualFrontier.sln`. REGISTER `DOC-F-SRC-PRESENTATION-NATIVE` retired.

**Rationale**: Q-G-2 LOCKED. Silk.NET+OpenGL path superseded by Vulkan substrate (V0.A decision). Existing stub implementations were lying-test surface. Production renderer materializes в DualFrontier.Launcher, не Presentation.Native.

**Halt condition**: If execution agent partial-removes (e.g. csproj kept, code deleted), halt + complete removal required.

### S-LOCK-8 — Tiered documentation scan (Q-G-10)

**Statement**: Documentation cleanup follows tiered scan per Q-G-10:
- **Tier 1 mandatory**: 8 Application/* files (Application README, Bridge README + 6 Commands XML doc, Rendering README + IRenderer + IDevKitRenderer + IRenderCommand)
- **Tier 2 mandatory**: `grep -r "Godot" docs/architecture/ --include="*.md"` excluding `historical/`; per-hit classify (historical preserve / stale cleanup / intentional contrast)
- **Tier 3 conditional**: src/ grep, trigger = Tier 2 surfaces non-Application drift
- **Tier 4 conditional**: docs/governance, docs/methodology, docs/processes grep; trigger = Tier 2/3 surfaces process drift

**Out-of-scope drift findings protocol**: documented но deferred к separate cleanup cascade. Cascade #2 discipline preserved.

**Default-include bias** (Lesson #14 PROMOTED): ambiguous classification → cleanup. Reversible через git history.

### S-LOCK-9 — К-L14 verification #11 honest-framed removal-type evidence

**Statement**: К-L14 verification #11 framed as **first removal-type evidence**. Per-criterion verification protocol per Q-G-14:
1. Build integrity (every atomic commit)
2. Test count parity (relative to pre-cascade baseline)
3. Pre-existing pollution status (NOT worse)
4. Runtime substrate intact (SmokeTest still passes, API surface unchanged, Vulkan validation clean)
5. Launcher composition smoke (constructs + Main() runs + GameLoop.Tick + bridge connects)
6. Solution integrity (project count parity 32 → 32)
7. METHODOLOGY v1.10 §12.7 Modding-suite verification gate

**Falsifiability conditions**: test regression beyond pollution, Runtime API break, SmokeTest regression, Vulkan validation errors, Launcher construction failure, pre-existing pollution worsening.

**Halt condition**: ANY falsifiability condition triggered → halt + ratification re: continuation.

### S-LOCK-10 — Lesson #N12 candidate captured + Lesson #25 refined

**Statement**: METHODOLOGY.md amended к include:
- **Lesson #N12 (new candidate)**: "Defensive Reserved Stub Pattern" — paired discipline с Lesson #25 refined. Provisional pool entry с first-application evidence (this cascade's RenderCommandDispatcher).
- **Lesson #25 refined**: Original Lesson #25 ("Design abstractions when consumer materializes") strengthened per Crystalka 2026-05-23 deliberation:
  > "Главное правило — не тестировать пустые интерфейсы и пустую реализацию которые врут в тестах, показывая что всё OK"
  
  Refined statement: "Design abstractions when consumer materializes AND structurally eliminate test-lying surface — empty stub implementations что pass tests by doing nothing constitute architectural debt independent of speculation discipline."

**Rationale**: Q-G-5 + Q-G-3 + Q-G-6 deliberation surfaced principles structurally. Methodology codification required для forward application.

---

## §2 — Phase 0 mandatory reads

Per Lesson #18 strengthened (production wiring file reads mandatory before milestone briefs), execution agent MUST `view` ALL files в this list before commit cascade begins. Skipping = brief violation.

### 2.1 State verification reads

- `D:\Colony_Simulator\Colony_Simulator\.git\HEAD` — confirm starting branch
- `D:\Colony_Simulator\Colony_Simulator\.git\refs\heads\main` — confirm main hash = `9ea5dbe`
- `D:\Colony_Simulator\Colony_Simulator\.git\logs\HEAD` (tail 10) — confirm session continuity

### 2.2 Production wiring reads (Lesson #18)

- `src\DualFrontier.Application\Loop\GameBootstrap.cs` (FULL READ) — CreateLoop signature + bridge consumption
- `src\DualFrontier.Application\Loop\GameLoop.cs` (FULL READ) — Tick semantics, Start/Stop lifecycle, threading model
- `src\DualFrontier.Application\Loop\GameContext.cs` — context structure
- `src\DualFrontier.Application\Loop\SimulationStateController.cs` — pause/resume semantics
- `src\DualFrontier.Application\Modding\ModMenuController.cs` — controller surface
- `src\DualFrontier.Runtime\Runtime.cs` — confirm V0.A+V0.B+V0.C.1+V0.C.2+V1 composition
- `src\DualFrontier.Runtime\RuntimeOptions.cs` — options surface
- `src\DualFrontier.Runtime\Window\Window.cs` — Win32 surface
- `src\DualFrontier.Runtime\Window\IWindow.cs` — interface
- `src\DualFrontier.Runtime\Window\InputEventQueue.cs` — input event flow
- `tests\DualFrontier.Runtime.SmokeTest\Program.cs` (FULL READ) — production-grade launcher reference

### 2.3 Bridge layer reads

- `src\DualFrontier.Application\Bridge\PresentationBridge.cs`
- `src\DualFrontier.Application\Bridge\IRenderCommand.cs`
- `src\DualFrontier.Application\Bridge\README.md`
- `src\DualFrontier.Application\Bridge\Commands\*.cs` (all 6 commands)
- `src\DualFrontier.Application\Bridge\VResourceCleanup.cs`

### 2.4 Rendering contracts reads

- `src\DualFrontier.Application\Rendering\IRenderer.cs`
- `src\DualFrontier.Application\Rendering\IDevKitRenderer.cs`
- `src\DualFrontier.Application\Rendering\README.md`
- `src\DualFrontier.Application\Attributes\*.cs` — `[DevKitOnly]` attribute definition

### 2.5 Application top-level reads

- `src\DualFrontier.Application\README.md`
- `src\DualFrontier.Application\DualFrontier.Application.csproj`
- `src\DualFrontier.Application\Display\CompositionFramework.cs` — К-L17 integration point
- `src\DualFrontier.Application\Display\Layer.cs` — layer abstraction

### 2.6 Solution + project reference reads

- `DualFrontier.sln` (full content)
- `.gitignore`
- `Directory.Build.props` (if exists) — global MSBuild props
- `global.json` (if exists) — .NET SDK version

### 2.7 Governance reads

- `docs\governance\REGISTER.yaml` (confirm register_version 2.3 + structure)
- `docs\architecture\KERNEL_ARCHITECTURE.md` (confirm v2.5 LOCKED)
- `docs\methodology\METHODOLOGY.md` (confirm v1.10 LOCKED)
- `docs\architecture\VULKAN_SUBSTRATE.md` (confirm v1.1 LOCKED)
- `docs\architecture\MOD_OS_ARCHITECTURE.md` (confirm v1.11 LOCKED)
- `docs\architecture\PHASE_A_PRIME_SEQUENCING.md` (current entry list)
- `docs\architecture\K_CLOSURE_REPORT.md` (head — confirm 2296 lines AUTHORED)
- `docs\architecture\historical\GODOT_INTEGRATION.md` (head — historical archive baseline)
- `docs\architecture\historical\VISUAL_ENGINE.md` (head — historical archive baseline)

### 2.8 Tier 2 grep preparation

Execute pre-Phase β:
```powershell
grep -rn "Godot" docs/architecture/ --include="*.md" `
  | Where-Object { $_ -notmatch "historical/" }
```

Classify each hit per criteria (§ 3 Phase β β2). Bring classification к Crystalka if anomalies surface.

### 2.9 Untracked Godot residue inventory

Execute pre-Phase α:
```powershell
Get-ChildItem -Path src\DualFrontier.Presentation -Recurse -Force | 
  Select-Object FullName, Length
```

Record inventory к brief execution log section.

---

## §3 — Atomic commit cascade specification

Per Lesson #8 strengthened (atomic compilable commits + multi-document evidence): every commit MUST `dotnet build` exit 0 + `sync_register.ps1 --validate` exit 0 (when REGISTER touched).

Cascade structure: **5 phases (α-ε) + ζ verification, 14-20 atomic commits**.

### Phase α — Physical purge (1-3 commits)

#### Commit α1 — Remove DualFrontier.Presentation.Native + sln mutation

**Scope**:
- Delete `src/DualFrontier.Presentation.Native/` directory entirely (all tracked files)
- Mutate `DualFrontier.sln`:
  - Remove `Project("{9A19103F-...}") = "DualFrontier.Presentation.Native", ...` block
  - Remove 12 ProjectConfigurationPlatforms entries for `{D76E0F3E-2B93-4096-8E33-84A4F3D70C77}`
  - Remove NestedProjects entry `{D76E0F3E-...} = {11111111-...}`
- Untracked Godot residue в `src/DualFrontier.Presentation.Native/` directory (`.cs.uid`, `bin/`, `obj/`, `.csproj.lscache`) cleaned alongside (manual rm-rf после tracked delete, since .gitignored)

**Verification**:
- `dotnet build` exit 0 (no other project references Presentation.Native — verified by Phase 0 reads)
- `dotnet test --no-build` exit 0 (test count parity preserved)

**Commit message template**:
```
chore(presentation): K_EXT_2 α1 — remove DualFrontier.Presentation.Native (Q-G-2)

Per К-extensions cascade #2 Q-G-2 LOCKED: Silk.NET+OpenGL stub project
superseded by Vulkan substrate (DualFrontier.Runtime, V0.A decision).
Existing stub implementations constituted lying-test surface (Lesson #25
refined). Production renderer materializes в DualFrontier.Launcher
(δ phase below), не Presentation.Native.

Removed:
- src/DualFrontier.Presentation.Native/ directory + all contents
- DualFrontier.sln Project entry {D76E0F3E-2B93-4096-8E33-84A4F3D70C77}
- 12 ProjectConfigurationPlatforms entries
- 1 NestedProjects entry

Untracked .cs.uid + bin/ + obj/ + .csproj.lscache residue cleaned alongside.

Build verified: dotnet build exit 0 (no remaining references к
DualFrontier.Presentation.Native confirmed via Phase 0 reads + grep).

S-LOCK-7 satisfied (full removal, not partial).
К-L14 verification #11 contribution: substrate (Runtime) unchanged through
removal of dead consumer scaffold.
```

#### Commit α2 — Clean untracked Godot residue в DualFrontier.Presentation/ (no-commit, disk cleanup)

**Scope**:
- Physical filesystem delete (NOT git operation since untracked) of `src/DualFrontier.Presentation/` residue:
  - `.cs.uid` files (12+)
  - `addons/` directory (Godot devkit metadata)
  - `.godot/` directory (Godot project metadata)
  - `.vs/` directory (Visual Studio cache)
  - `bin/`, `obj/` directories (.NET build artifacts)
  - `assets/` directory (Godot asset symlinks/files)
  - `*.lscache` files
  - `DualFrontier.Presentation.csproj.lscache`
- After cleanup: confirm `src/DualFrontier.Presentation/` directory empty OR removed entirely
- If directory empty, **remove directory** (physical rmdir)

**Note**: This is **filesystem operation**, not git operation. Files are all .gitignored (per Phase 0 read of .gitignore). Git tree unchanged. Recorded в execution log, no commit.

**Execution log entry**:
```
α2 (no-commit, disk cleanup):
Cleaned untracked Godot residue в src/DualFrontier.Presentation/:
- 12 .cs.uid files
- addons/ directory (Godot devkit metadata)
- .godot/ directory (Godot project metadata)  
- .vs/, bin/, obj/, assets/ directories
- *.lscache files
- DualFrontier.Presentation.csproj.lscache
Directory removed entirely after cleanup.
git status: no changes (all .gitignored).
```

#### Commit α3 — Confirm no remaining Godot tracked residue (verification only)

**Scope**:
- Execute verification greps:
  ```powershell
  grep -rn "using Godot" src/ --include="*.cs" --exclude-dir={bin,obj,historical}
  grep -rn "project.godot" --include="*.cs" --include="*.csproj" --include="*.md"
  ```
- If hits surface: address them в this commit (or separate β commits если documentation)
- If clean: **no commit**, execution log note suffices

**Execution log entry** (if clean):
```
α3 (no-commit if clean):
Verified no remaining Godot tracked residue в src/ за пределами Application/* 
documentation (which is β phase scope).
```

### Phase β — Documentation cleanup, tiered (3-5 commits)

#### Commit β1 — Tier 1 Application/* documentation cleanup

**Scope** — 8 file modifications:

##### β1.1 — `src/DualFrontier.Application/README.md`
- Replace "the bridge into the Godot layer" → "the bridge into the rendering layer"
- Replace "Application **must not** know about Godot or call `Presentation` directly" → reframed для Launcher pattern
- Replace "Phase 3 — wire `PresentationBridge.DrainCommands` into Godot `_Process`" → "wire into Launcher main loop iteration (К-extensions cascade #2 closure 2026-05-DD)"

##### β1.2 — `src/DualFrontier.Application/Bridge/README.md`
- "between the domain (multithreaded) and the Godot layer (main thread only)" → "between the domain (multithreaded) and the rendering layer (main thread only)"
- "Godot в его `_Process` reads the queue" → "Launcher's main loop iteration reads the queue (per Q-G-7 hybrid orchestration)"
- "In the Godot assembly this is `GameRoot`; in the Native assembly it is `NativeRenderer`" → "Production renderer (Launcher's `LauncherRenderer`) receives commands via `RenderCommandDispatcher`. Future DevKit renderer (dormant `IDevKitRenderer` consumer) будет implement similar pattern над Vulkan substrate"

##### β1.3 — `src/DualFrontier.Application/Rendering/README.md`
- "concrete implementations live в Presentation assemblies: Godot DevKit and Native (Silk.NET)" → "concrete implementation lives в `DualFrontier.Launcher` (Vulkan-native via DualFrontier.Runtime substrate). Reserved DevKit-tier extension (`IDevKitRenderer`, dormant per К-extensions cascade #2 2026-05-DD) для future first-party DevKit work."
- DELETE "Phase 3.5 — `GodotRenderer` in `DualFrontier.Presentation`"
- DELETE "Phase 5+ — `NativeRenderer` in `DualFrontier.Presentation.Native` (Silk.NET + OpenGL...)"
- Update VISUAL_ENGINE.md link → historical path
- Add TODO: "К-extensions cascade #3 — `LauncherRenderer` real visual implementation"

##### β1.4 — `src/DualFrontier.Application/Rendering/IRenderer.cs` XML doc rewrite
- "Debug visualisations... live в the devkit-tier extension `IDevKitRenderer`, which only Godot DevKit implements" → "Debug visualisations... live в the devkit-tier extension `IDevKitRenderer` (dormant per К-extensions cascade #2 — reserved для future first-party DevKit над Vulkan substrate)"

##### β1.5 — `src/DualFrontier.Application/Rendering/IDevKitRenderer.cs` XML doc + attribute message
Full XML doc replacement per Q-G-1 LOCKED:
```csharp
/// <summary>
/// DevKit-tier extension of <see cref="IRenderer"/> — reserved abstraction
/// surface для future first-party DevKit backend. Currently dormant: no
/// active consumer post-Godot deprecation (К-extensions cascade #2,
/// 2026-05-DD). The native production runtime never references this
/// interface — debug surface deliberately empty к keep shipped runtime
/// lean.
/// </summary>
/// <remarks>
/// Reserved для forward DevKit work: when first-party developer tooling
/// materializes, it MAY implement this contract atop the Vulkan substrate
/// (DualFrontier.Runtime primitives) либо supersede it с a Vulkan-native
/// equivalent. Until then the abstraction stays в place as architectural
/// intent — DevKit/Production renderer split survives the Godot deprecation.
/// </remarks>
[DevKitOnly("Reserved DevKit surface — not shipped to players. Currently dormant.")]
public interface IDevKitRenderer : IRenderer { /* signatures unchanged */ }
```

##### β1.6 — `src/DualFrontier.Application/Bridge/IRenderCommand.cs` STRIP к marker (Q-G-3)
Full file replacement:
```csharp
namespace DualFrontier.Application.Bridge;

/// <summary>
/// Marker interface для render commands enqueued onto the
/// <see cref="PresentationBridge"/>. Commands are pure immutable data records
/// carrying the information needed для visual effects. Dispatch is handled
/// by the active renderer (e.g. Launcher's <c>RenderCommandDispatcher</c>),
/// не by the command itself — per Lesson #25 refined: empty Execute() bodies
/// were lying-test surface, structurally eliminated в К-extensions cascade #2
/// (2026-05-DD).
/// </summary>
public interface IRenderCommand
{
}
```

##### β1.7 — `src/DualFrontier.Application/Bridge/Commands/*.cs` — strip Execute() from all 6
Files: PawnSpawnedCommand, PawnMovedCommand, PawnDiedCommand, PawnStateCommand, ItemSpawnedCommand, TickAdvancedCommand.

For each: remove `public void Execute(object renderContext) { /* TODO Phase 5 ... */ }` block. Update XML doc к drop Godot framing.

**Canonical pattern**:
```csharp
/// <summary>
/// Command: pawn <paramref name="PawnId"/> has appeared в the world at
/// position (<paramref name="X"/>, <paramref name="Y"/>). The presentation
/// layer (Launcher's <c>RenderCommandDispatcher</c>) creates the visual node
/// and places it on the scene. Per К-extensions cascade #2 (2026-05-DD):
/// Commands are pure data records — dispatch handled centrally by Launcher,
/// не via per-command Execute() method (Lesson #25 refined applied).
/// </summary>
public sealed record PawnSpawnedCommand(EntityId PawnId, float X, float Y) : IRenderCommand;
```

##### β1.8 — `src/DualFrontier.Application/Bridge/PresentationBridge.cs` XML doc update
- "the main thread of the active IRenderer (Godot or Native) drains them" → "the main thread of the active IRenderer drains them. К-extensions cascade #2 (2026-05-DD) deprecated Godot+Silk.NET paths; current single backend = Launcher's `LauncherRenderer`"
- Remove "TODO: Phase 3" prefixes (cascade #2 completes Phase 3 wiring)

**Commit message template**:
```
docs(application): K_EXT_2 β1 — Tier 1 documentation cleanup (Q-G-10 Tier 1)

Per К-extensions cascade #2 Q-G-10 LOCKED Tier 1 (mandatory): 8 Application/*
file documentation cleanup removing Godot/Silk.NET stale references,
applying Q-G-1 (IDevKitRenderer dormant) + Q-G-3 (IRenderCommand pure marker)
rewrites.

Files modified:
- src/DualFrontier.Application/README.md — Godot → Launcher framing
- src/DualFrontier.Application/Bridge/README.md — Godot _Process → Launcher loop
- src/DualFrontier.Application/Rendering/README.md — Silk.NET + Godot retired
- src/DualFrontier.Application/Rendering/IRenderer.cs — XML doc decoupled
- src/DualFrontier.Application/Rendering/IDevKitRenderer.cs — dormant rewrite (Q-G-1)
- src/DualFrontier.Application/Bridge/IRenderCommand.cs — strip к marker (Q-G-3)
- src/DualFrontier.Application/Bridge/Commands/*.cs — drop Execute() (6 files)
- src/DualFrontier.Application/Bridge/PresentationBridge.cs — XML doc update

Build verified: dotnet build exit 0 (interface contract simplification
backward-compat at consumer sites — no Execute() callers in current code
per Phase 0 reads + grep verification).

S-LOCK-5 (IRenderCommand marker) + S-LOCK-6 (IDevKitRenderer dormant) +
S-LOCK-8 (Tier 1 documentation cleanup) satisfied.
```

#### Commit β2 — Tier 2 docs/architecture grep findings cleanup (mandatory)

**Scope**:
- Execute Tier 2 grep: `grep -rn "Godot" docs/architecture/ --include="*.md"` excluding `historical/`
- Per-hit classify (historical preserve / stale cleanup / intentional contrast)
- Apply cleanup к stale references; preserve historical/intentional refs unchanged
- Files likely affected (Phase 0 read confirmation needed): ARCHITECTURE.md, THREADING.md, MODDING.md, possibly others

**Halt condition**: If Tier 2 grep returns > 20 hits across > 5 files → halt + Crystalka ratification.

**Commit message template**:
```
docs(architecture): K_EXT_2 β2 — Tier 2 Godot reference cleanup (Q-G-10 Tier 2)

Per К-extensions cascade #2 Q-G-10 LOCKED Tier 2 (mandatory): grep -rn "Godot"
docs/architecture/ findings classified + stale references cleaned.

Findings inventory (from pre-Phase β grep):
[N hits across M files — populate at execution time]

Classification applied:
- Historical context preserve: [list]
- Stale production reference → cleanup: [list of edits]
- Intentional contrast preserve: [list]

Default-include bias (Lesson #14 PROMOTED) applied to ambiguous cases.

Build verified: dotnet build exit 0 (documentation changes only).
```

#### Commit β3 — Tier 3 src/* grep findings cleanup (conditional)

**Trigger condition**: Tier 2 surfaces non-Application drift patterns.

**Scope** (if triggered):
- Execute `grep -rn "Godot" src/ --include="*.cs" --include="*.md" --exclude-dir={bin,obj,DualFrontier.Presentation,historical}`
- Per-hit cleanup similar к β2

**If not triggered**: Skip commit + execution log note.

#### Commit β4 — Tier 4 governance/methodology/processes cleanup (conditional)

**Trigger condition**: Tier 2 OR Tier 3 surfaces process-level drift.

**Scope** (if triggered):
- Execute `grep -rn "Godot" docs/governance/ docs/methodology/ docs/processes/`
- Cleanup applies

**If not triggered**: Skip + log note.

#### Commit β5 — historical/* cross-reference normalization (mandatory)

**Scope**:
- Add "К-extensions cascade #2 closure" addendum к `docs/architecture/historical/GODOT_INTEGRATION.md`:
  ```markdown
  ## К-extensions cascade #2 closure (2026-05-DD)
  
  К-extensions cascade #2 — Godot Full Deprecation + Launcher Formalization —
  completes the deprecation arc. Architectural intent preservation:
  - DevKit/Production renderer split survives через `IDevKitRenderer` interface
  - Bridge command queue pattern survives — dispatch now centralized в Launcher's
    `RenderCommandDispatcher` (vs original per-command Execute() pattern, which
    constituted lying-test surface per Lesson #25 refined).
  
  Cross-references:
  - tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md
  - docs/architecture/K_EXTENSIONS_LEDGER.md §3.3
  ```
- Possibly add similar addendum к `docs/architecture/historical/VISUAL_ENGINE.md`

**Commit message template**:
```
docs(historical): K_EXT_2 β5 — Godot/Visual Engine historical archives closure note

Per К-extensions cascade #2 completion: add closure addendum к historical
archives. Architectural intent preservation documented + cross-references
к active artifacts.

Files modified:
- docs/architecture/historical/GODOT_INTEGRATION.md — closure addendum
- docs/architecture/historical/VISUAL_ENGINE.md — cross-reference paragraph

Build verified: dotnet build exit 0 (documentation only).
```

### Phase γ — Architectural decisions (0 separate commits, applied в α/β/δ)

γ phase decisions ratified в deliberation; code changes happen в α/β/δ:
- **γ1 IDevKitRenderer dormant + Godot-decoupled**: applied в β1.5
- **γ2 Presentation.Native removed**: applied в α1
- **γ3 IRenderCommand pure marker**: applied в β1.6 + β1.7
- **γ4 SUBSUMED by Q-G-3**: no separate action
- **γ5 No new К-L + Lesson #N12 captured**: К-L unchanged (21 final); Lesson #N12 codified в ε2


### Phase δ — Launcher project scaffold (3 commits)

#### Commit δ1 — Create DualFrontier.Launcher project + csproj

**Scope**:
- Create directory: `src/DualFrontier.Launcher/`
- Create file: `src/DualFrontier.Launcher/DualFrontier.Launcher.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
    <RootNamespace>DualFrontier.Launcher</RootNamespace>
    <AssemblyName>DualFrontier.Launcher</AssemblyName>
    <ApplicationIcon></ApplicationIcon>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\DualFrontier.Application\DualFrontier.Application.csproj" />
    <ProjectReference Include="..\DualFrontier.Runtime\DualFrontier.Runtime.csproj" />
  </ItemGroup>
</Project>
```

**Phase 0 dependency**: TargetFramework value confirmed via reading existing `src/*/.*csproj` before this commit. If existing TargetFramework differs от `net8.0`, update accordingly (consistency с codebase).

**Verification gate**: `dotnet build src/DualFrontier.Launcher/DualFrontier.Launcher.csproj` exit 0 (compiles empty project successfully).

**Commit message template**:
```
feat(launcher): K_EXT_2 δ1 — create DualFrontier.Launcher project scaffold (Q-G-6 δ1)

Per К-extensions cascade #2 Q-G-6 LOCKED (b)+(i)+(b1): create new production
launcher project. Cascade #2 ships infrastructure-only per (b1) functional bar;
visual implementation deferred к К-extensions cascade #3.

Project configuration (Q-G-8 (b) executable-focused):
- OutputType: Exe
- TargetFramework: net8.0
- 2 ProjectReferences: DualFrontier.Application + DualFrontier.Runtime
- 2 sln configurations (Debug|Any CPU + Release|Any CPU) — added в δ3

Build verified: dotnet build src/DualFrontier.Launcher/ exit 0.
S-LOCK-3 (infrastructure-only) preserved — only scaffold committed.
```

#### Commit δ2 — Program.cs + LauncherRenderer.cs + RenderCommandDispatcher.cs (atomic compilable unit)

**Note on atomicity**: Per Lesson #8 strengthened, this commit ships all 3 files together as one compilable unit. Splitting would create non-compilable intermediate commits violating Lesson #8.

**Scope** — 3 files:

##### δ2.1 — `src/DualFrontier.Launcher/Program.cs`

```csharp
using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using DualFrontier.Runtime;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Launcher;

/// <summary>
/// Production launcher entry point для Dual Frontier. Composes Vulkan
/// substrate (<see cref="Runtime.Runtime"/>) + Domain layer
/// (<see cref="GameContext"/> via <see cref="GameBootstrap"/>) +
/// <see cref="LauncherRenderer"/> bridge between them. Drives main loop
/// per Q-G-7 (d) hybrid orchestration.
///
/// К-extensions cascade #2 (2026-05-DD) ships infrastructure-only:
/// — Window opens, Vulkan initializes, GameLoop ticks, bridge connects,
///   dispatcher receives commands (defensive throws fire if visual paths
///   invoked) per Lesson #N12 first application.
/// Visual implementation lands в К-extensions cascade #3 (next session).
/// </summary>
internal static class Program
{
    public static int Main(string[] args)
    {
        // === Composition ===
        var runtimeOptions = new RuntimeOptions
        {
            Window = new WindowOptions
            {
                Title = "Dual Frontier",
                Width = 1280,
                Height = 720,
            },
            EnableValidationLayer = true,  // DEBUG; Release config should disable
            AssetsDirectory = "assets",
        };

        using var runtime = Runtime.Runtime.Create(runtimeOptions);

        var bridge = new PresentationBridge();
        GameContext gameContext = GameBootstrap.CreateLoop(bridge);

        var dispatcher = new RenderCommandDispatcher(runtime);
        using var renderer = new LauncherRenderer(runtime, bridge, dispatcher);

        // === Lifecycle init ===
        renderer.Initialize();
        runtime.Window.Show();
        gameContext.GameLoop.Start();

        // === Main loop (Q-G-7 (d) hybrid orchestration) ===
        var lastFrameTime = DateTime.UtcNow;
        while (runtime.Window.IsOpen)
        {
            var now = DateTime.UtcNow;
            var deltaSeconds = (now - lastFrameTime).TotalSeconds;
            lastFrameTime = now;

            // 1. Pump Windows messages (surfaces input events к InputQueue)
            runtime.Window.PumpMessages();

            // 2. Drain InputQueue → forward к Application
            //    К-extensions cascade #3 territory — InputBridge wiring TBD.
            //    Cascade #2: input events pumped but discarded (no consumer yet).
            while (runtime.InputQueue.TryDequeue(out IInputEvent? _))
            {
                // Cascade #3 will forward к Application input bridge here.
            }

            // 3. Tick simulation (GameLoop internal accumulator handles fixed-step)
            gameContext.GameLoop.Tick();

            // 4. Render frame (drain bridge + dispatch + future Runtime record)
            renderer.RenderFrame(deltaSeconds);
        }

        // === Shutdown ===
        gameContext.GameLoop.Stop();
        renderer.Shutdown();
        return 0;
    }
}
```

**Phase 0 signature dependencies** (confirm before commit):
- `GameLoop.Start()` + `GameLoop.Stop()` + `GameLoop.Tick()` signatures exist
- `InputQueue.TryDequeue(out IInputEvent? evt)` signature
- `Runtime.Window.IsOpen` property exists
- `Runtime.Window.PumpMessages()` method exists
- `AssetsDirectory` property on `RuntimeOptions` exists
- `Runtime.Runtime.Create(RuntimeOptions)` factory exists

If any signature differs, adjust Program.cs accordingly (preserving (d) hybrid orchestration intent). Halt condition P0-1..P0-5 applies if substantial deviation.

##### δ2.2 — `src/DualFrontier.Launcher/LauncherRenderer.cs`

```csharp
using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Rendering;
using DualFrontier.Runtime;

namespace DualFrontier.Launcher;

/// <summary>
/// Production <see cref="IRenderer"/> implementation. Wraps Vulkan substrate
/// (<see cref="Runtime.Runtime"/>) + drains <see cref="PresentationBridge"/>
/// per frame, dispatching commands к <see cref="RenderCommandDispatcher"/>.
///
/// К-extensions cascade #2 (2026-05-DD): Infrastructure scaffold per Q-G-6 (b1).
/// Real visual command dispatching deferred к cascade #3 — current dispatcher
/// arms throw <see cref="NotImplementedException"/> per Lesson #N12 «Defensive
/// Reserved Stub Pattern» first application.
/// </summary>
internal sealed class LauncherRenderer : IRenderer, IDisposable
{
    private readonly Runtime.Runtime _runtime;
    private readonly PresentationBridge _bridge;
    private readonly RenderCommandDispatcher _dispatcher;
    private bool _disposed;

    public LauncherRenderer(Runtime.Runtime runtime, PresentationBridge bridge, RenderCommandDispatcher dispatcher)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    /// <summary>
    /// Initialization hook called once before main loop iteration begins.
    /// Cascade #2: Runtime constructs Vulkan stack в <see cref="Runtime.Runtime.Create"/>;
    /// no extra init needed at this layer. Cascade #3 may add sprite catalog
    /// initialization here.
    /// </summary>
    public void Initialize()
    {
        // No-op в cascade #2 — Runtime composition handles Vulkan init.
    }

    /// <summary>
    /// Per-frame rendering: drain <see cref="PresentationBridge"/> commands к
    /// <see cref="RenderCommandDispatcher"/>; cascade #3 will add actual Vulkan
    /// sprite recording here.
    /// </summary>
    public void RenderFrame(double deltaSeconds)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Drain Bridge commands → dispatch к Runtime primitives via dispatcher.
        // Cascade #2: dispatcher arms throw NotImplementedException for visual
        // dispatch paths (Lesson #N12). Test enqueuing should verify dispatcher
        // receives command type без invoking the throw path.
        _bridge.DrainCommands(_dispatcher.Dispatch);

        // Frame Vulkan recording — cascade #3 territory.
        // К-extensions cascade #3 will add:
        //   - BeginRenderPassForSprites
        //   - SpriteRenderer.BeginFrame/Submit/EndFrame
        //   - EndSpriteRenderPass
        //   driven by accumulated dispatcher state (sprite catalog, scene state)
    }

    public void Shutdown()
    {
        _disposed = true;
    }

    public bool IsRunning => !_disposed && _runtime.Window.IsOpen;

    public void Dispose() => Shutdown();
}
```

**Phase 0 signature dependencies**:
- `IRenderer` interface — confirm method set (Initialize, RenderFrame, Shutdown, IsRunning)
- `PresentationBridge.DrainCommands(Action<IRenderCommand>)` — confirm signature

If `IRenderer` has methods не yet covered by LauncherRenderer (e.g. additional lifecycle hooks), add them with defensive throws OR no-op per Lesson #N12 discipline.

##### δ2.3 — `src/DualFrontier.Launcher/RenderCommandDispatcher.cs`

```csharp
using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Runtime;

namespace DualFrontier.Launcher;

/// <summary>
/// Dispatches drained <see cref="IRenderCommand"/> instances к Vulkan primitives
/// (via <see cref="Runtime.Runtime"/>) using pattern matching по concrete
/// command type.
///
/// К-extensions cascade #2 (2026-05-DD): Infrastructure scaffold per Q-G-6 (b1).
/// All dispatch arms currently throw <see cref="NotImplementedException"/> с
/// descriptive message linking к Lesson #N12 «Defensive Reserved Stub Pattern»
/// first application. Real visual dispatching lands в К-extensions cascade #3
/// (next session).
///
/// Defensive throws prevent lying tests — test что exercises any visual
/// dispatch path will fail loudly until cascade #3 supplies real implementation.
/// Empty bodies would have passed tests by doing nothing → architectural debt
/// per Lesson #25 refined.
/// </summary>
internal sealed class RenderCommandDispatcher
{
    private readonly Runtime.Runtime _runtime;

    public RenderCommandDispatcher(Runtime.Runtime runtime)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
    }

    public void Dispatch(IRenderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        switch (command)
        {
            case PawnSpawnedCommand cmd: HandlePawnSpawned(cmd); break;
            case PawnMovedCommand cmd: HandlePawnMoved(cmd); break;
            case PawnDiedCommand cmd: HandlePawnDied(cmd); break;
            case PawnStateCommand cmd: HandlePawnState(cmd); break;
            case ItemSpawnedCommand cmd: HandleItemSpawned(cmd); break;
            case TickAdvancedCommand cmd: HandleTickAdvanced(cmd); break;
            default:
                throw new NotSupportedException(
                    $"Unknown IRenderCommand type '{command.GetType().FullName}'. " +
                    "Add dispatch arm в RenderCommandDispatcher.Dispatch (К-extensions " +
                    "cascade #3 territory) и accompanying handler method below.");
        }
    }

    private void HandlePawnSpawned(PawnSpawnedCommand cmd) =>
        throw new NotImplementedException(
            "PawnSpawned dispatch pending К-extensions cascade #3. " +
            "If this throws в test, the test is exercising visual rendering path " +
            "что cascade #2 explicitly scoped out (Lesson #N12 Defensive Reserved " +
            "Stub Pattern first application). Cascade #3 will implement: create " +
            "sprite at (cmd.X, cmd.Y) using pawn atlas, register в SpriteCatalog " +
            "keyed by cmd.PawnId.");

    private void HandlePawnMoved(PawnMovedCommand cmd) =>
        throw new NotImplementedException(
            "PawnMoved dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: lookup sprite by cmd.PawnId, update position " +
            "к (cmd.X, cmd.Y).");

    private void HandlePawnDied(PawnDiedCommand cmd) =>
        throw new NotImplementedException(
            "PawnDied dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: lookup sprite by cmd.PawnId, play death " +
            "animation, despawn sprite from SpriteCatalog.");

    private void HandlePawnState(PawnStateCommand cmd) =>
        throw new NotImplementedException(
            "PawnState dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: update HUD pawn detail panel (name, needs, " +
            "mood, job label, top skills).");

    private void HandleItemSpawned(ItemSpawnedCommand cmd) =>
        throw new NotImplementedException(
            "ItemSpawned dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: create item visual at (cmd.X, cmd.Y) " +
            "selecting atlas region based on cmd.Kind.");

    private void HandleTickAdvanced(TickAdvancedCommand cmd) =>
        throw new NotImplementedException(
            "TickAdvanced dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: update HUD tick label к cmd.Tick.");
}
```

**Phase 0 signature dependencies**:
- 6 Bridge.Commands record types confirmed accessible (`PawnSpawnedCommand` etc.)
- Each Command record's property names confirmed (PawnId, X, Y, Kind, Tick) — match defensive throw messages

If property names differ, update defensive throw messages accordingly.

**Verification gate**:
- `dotnet build` exit 0 (all 3 files compile together с Application + Runtime references)
- Solution still loads (sln modifications come в δ3)

**Commit message template**:
```
feat(launcher): K_EXT_2 δ2 — Program + LauncherRenderer + RenderCommandDispatcher (Q-G-6 + Q-G-7)

Per К-extensions cascade #2 Q-G-6 LOCKED (b)+(i)+(b1) + Q-G-7 LOCKED (d):
add three core Launcher files implementing infrastructure-only renderer +
dispatcher per Lesson #N12 first application.

Added (atomic compilable unit):
- src/DualFrontier.Launcher/Program.cs — composition root + (d) hybrid main loop
- src/DualFrontier.Launcher/LauncherRenderer.cs — IRenderer impl + bridge drain
- src/DualFrontier.Launcher/RenderCommandDispatcher.cs — defensive stub dispatcher

Defensive Reserved Stub Pattern (Lesson #N12) first application: all 6
dispatch handler methods throw NotImplementedException с descriptive message
linking к cascade #3 future implementation + Lesson #N12 explanation.

S-LOCK-3 (infrastructure-only) + S-LOCK-4 (Defensive Reserved Stub) satisfied.

Forward cascade #3 scope: replace defensive throws с real visual implementations
(SpriteCatalog + scene state + Vulkan sprite recording).

Build verified: dotnet build exit 0 (all 3 files compile together).
К-L14 verification #11 contribution: substrate (Runtime) unchanged; new
consumer (Launcher) added без modifying any К-L primitive.
```

#### Commit δ3 — Launcher README + sln integration (atomic)

**Scope** — 2 changes в one commit:

##### δ3.1 — `src/DualFrontier.Launcher/README.md`

```markdown
# DualFrontier.Launcher — Production Launch Entry Point

## Purpose
Production launcher для Dual Frontier. Composes:
- **Vulkan substrate** (`DualFrontier.Runtime`) — window, Vulkan device, sprite
  pipeline, compute primitives
- **Domain layer** (`DualFrontier.Application`) — GameLoop, ModMenuController,
  PresentationBridge, GameBootstrap
- **Bridge** (`LauncherRenderer` + `RenderCommandDispatcher`) — drains
  PresentationBridge commands per frame, dispatches к Vulkan primitives

Implements `IRenderer` contract from `DualFrontier.Application.Rendering` via
`LauncherRenderer` class.

## Dependencies
- `DualFrontier.Application` — contracts + GameBootstrap + bridge + commands
- `DualFrontier.Runtime` — Vulkan substrate primitives

## Contents
- `Program.cs` — `Main()` entry point, composition root, (d) hybrid main loop
  per Q-G-7 LOCKED orchestration
- `LauncherRenderer.cs` — `IRenderer` implementation wrapping Runtime + bridge
- `RenderCommandDispatcher.cs` — pattern-matching dispatcher для IRenderCommand
  instances

## Cascade scope (К-extensions cascade #2)

К-extensions cascade #2 (2026-05-DD) ships **infrastructure-only** per Q-G-6
(b1) functional bar:
- Window opens, Vulkan initializes, GameLoop ticks
- PresentationBridge connects к dispatcher
- Dispatcher receives commands (defensive throws fire if visual paths invoked)

**Defensive Reserved Stub Pattern** (Lesson #N12 first application): all
dispatch handler methods throw `NotImplementedException` с descriptive message.
This is **intentional** — prevents lying tests per Lesson #25 refined.

## Forward roadmap

**К-extensions cascade #3** (next session, separate brief):
- Replace defensive throws с real visual implementations
- Add SpriteCatalog (`PawnId → Sprite` mapping)
- Add scene state management
- (b2) functional bar — pawns appear as sprites, move, despawn on death

## Rules

- No `using Godot;` (Godot path retired per К-extensions cascade #2)
- No `using Silk.NET;` (Silk.NET path retired — superseded by Vulkan substrate)
- Implements `DualFrontier.Application.Rendering.IRenderer` contract
- Domain knowledge limited к `PresentationBridge` + `IRenderCommand` types

---
<!-- Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY -->
<!-- Manual edits overwritten by sync_register.ps1 on next sync. -->
<!-- register_id: DOC-F-SRC-LAUNCHER -->
<!-- category: F | tier: 4 | lifecycle: Live | owner: Crystalka -->
```

##### δ3.2 — `DualFrontier.sln` Launcher project entry

Generate new GUID для Launcher project (e.g. via PowerShell `[guid]::NewGuid()` — random v4 UUID).

Mutate `DualFrontier.sln`:
- Add Project block:
  ```
  Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "DualFrontier.Launcher", "src\DualFrontier.Launcher\DualFrontier.Launcher.csproj", "{NEW-GUID}"
  EndProject
  ```
- Add 4 ProjectConfigurationPlatforms entries (Q-G-8 (b) executable-focused = 2 configs):
  ```
  {NEW-GUID}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
  {NEW-GUID}.Debug|Any CPU.Build.0 = Debug|Any CPU
  {NEW-GUID}.Release|Any CPU.ActiveCfg = Release|Any CPU
  {NEW-GUID}.Release|Any CPU.Build.0 = Release|Any CPU
  ```
- Add NestedProjects entry under `src/` folder:
  ```
  {NEW-GUID} = {11111111-1111-1111-1111-111111111111}
  ```

**Verification gate**:
- `dotnet sln list` lists DualFrontier.Launcher
- `dotnet build` exit 0 (full solution)
- `dotnet test --no-build` exit 0 (test count parity preserved)
- Solution loads в Visual Studio без errors

**Commit message template**:
```
feat(launcher): K_EXT_2 δ3 — Launcher README + sln integration (Q-G-6 + Q-G-8)

Per К-extensions cascade #2 Q-G-6 LOCKED + Q-G-8 LOCKED (b) executable-focused:
add Launcher README + integrate project к solution.

Added:
- src/DualFrontier.Launcher/README.md — project documentation
- DualFrontier.sln Project entry: DualFrontier.Launcher (GUID: {NEW-GUID})
- 4 ProjectConfigurationPlatforms entries (2 configs × ActiveCfg/Build.0)
- 1 NestedProjects entry under src/ folder

Build verified: dotnet build exit 0 (full solution).
Tests verified: dotnet test --no-build exit 0 (test count parity).

Net project count: 32 → 32 (Presentation.Native -1 в α1, Launcher +1 here).
К-L14 verification #11 contribution: solution structural integrity preserved.

REGISTER enrollment для DOC-F-SRC-LAUNCHER comes в ε6.
```


### Phase ε — Governance + REGISTER cascade (6 commits)

#### Commit ε1 — KERNEL_ARCHITECTURE.md v2.5 → v2.5.1

**Scope**:
- Edit `docs/architecture/KERNEL_ARCHITECTURE.md`:
  - Version field: `2.5` → `2.5.1`
  - Status footnote section: add cascade #2 chronicle entry (template below)
  - Cross-reference к `K_EXTENSIONS_LEDGER.md` (added в ε4)
  - Dormant abstractions note (IDevKitRenderer status per Q-G-1)
  - **NEW**: Versioning convention codified в Part 0

**Status footnote addition template**:
```markdown
### К-extensions cascade #2 chronicle (2026-05-DD)

К-extensions cascade #2 — Godot Full Deprecation + Launcher Formalization
completed 2026-05-DD. Cascade scope:
- Physical removal of Godot/Silk.NET legacy paths (DualFrontier.Presentation.Native
  retired; src/DualFrontier.Presentation/ untracked residue cleaned)
- Documentation cleanup (Tier 1 mandatory: 8 Application/* files; Tier 2 grep
  findings classified + cleaned per Lesson #14 default-include bias)
- Launcher project scaffold (DualFrontier.Launcher, infrastructure-only per
  Q-G-6 (b1) с Defensive Reserved Stub dispatcher — Lesson #N12 first
  application)
- Architectural decisions: IDevKitRenderer dormant + decoupled от Godot (Q-G-1);
  IRenderCommand pure marker (Q-G-3)
- К-L impact: zero (cascade focused on cleanup + launcher; no new К-L invariants)
- К-L14 verification #11 — first removal-type evidence (passing per honest-framed
  protocol Q-G-14)

К-L count unchanged: 21 final (20 LOCKED + 1 RESERVED).
Lesson #N12 «Defensive Reserved Stub Pattern» captured as Provisional Lesson
candidate с first-application evidence.

Cross-references:
- docs/architecture/K_EXTENSIONS_LEDGER.md §3.3 (cascade ledger entry)
- tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md (this cascade's brief)
- docs/methodology/METHODOLOGY.md v1.11 (Lesson #N12 + Lesson #25 refined)
- docs/architecture/PHASE_A_PRIME_SEQUENCING.md (cascade chronological entry)
- src/DualFrontier.Launcher/README.md (new launcher project)
- К-L14 row: evidence count 10 → 11 (verification #11 added)
```

**Versioning convention codification** addition к KERNEL Part 0 либо similar canonical location:
```markdown
### Tier 1 versioning convention (codified К-extensions cascade #2)

Для Tier 1 LOCKED documents (KERNEL_ARCHITECTURE, METHODOLOGY, VULKAN_SUBSTRATE,
MOD_OS_ARCHITECTURE):

| Change type | Version bump | Example |
|---|---|---|
| New К-L invariant / SUPERSEDE / behavioral contract change | **Minor**: 2.5 → 2.6 | А'.8 (8 К-L LOCK batch), А'.7.x (К-L15.1) |
| Cleanup / chronicle / cross-reference / status footnote | **Patch**: 2.5 → 2.5.1 | К-extensions cascade #2 (this cascade) |
| Structural reorganization / major architectural pivot | **Major**: 2.x → 3.0 | Hypothetical future event |
```

**Commit message template**:
```
docs(kernel): K_EXT_2 ε1 — KERNEL_ARCHITECTURE.md v2.5 → v2.5.1 (Q-G-12)

Per К-extensions cascade #2 Q-G-12 LOCKED (b) patch bump:
- Version: 2.5 → 2.5.1 (cleanup/chronicle change, не architectural innovation)
- Status footnote: К-extensions cascade #2 chronicle entry
- Cross-references к K_EXTENSIONS_LEDGER.md, METHODOLOGY v1.11, SEQUENCING entry
- Dormant abstractions note: IDevKitRenderer status per Q-G-1
- Versioning convention codified: minor / patch / major semantics

К-L count unchanged: 21 final.
К-L14 evidence count: 10 → 11 (verification #11 added — first removal-type).

Q-G-12 versioning convention precedent established для forward Tier 1 docs.

Build verified: dotnet build exit 0 (documentation only).
sync_register.ps1 --validate gate: will pass post-ε6 REGISTER update.
```

#### Commit ε2 — METHODOLOGY.md v1.10 → v1.11 (Lesson #N12 + #25 refined)

**Scope**:
- Edit `docs/methodology/METHODOLOGY.md`:
  - Version: `1.10` → `1.11`
  - Add **Lesson #N12 candidate** к Provisional pool
  - Refine **Lesson #25** entry per Crystalka 2026-05-23 deliberation
  - Update Lessons batch counts

**Lesson #N12 candidate template**:
```markdown
### Lesson #N12 candidate — Defensive Reserved Stub Pattern

**Status**: Provisional (first-application captured К-extensions cascade #2)

**Statement**: Когда interface либо implementation should exist structurally
без real implementation (dormant abstractions, reserved tiers, forward-options),
implementations MUST throw descriptive exceptions (`NotImplementedException`
or similar) on invocation, не silently return defaults либо empty values.

**Rationale**: Empty stub implementations compile, pass type-checker, and
pass tests by doing nothing — constituting lying-test surface. Tests
reporting green when system functionally does nothing = test integrity
violation. Defensive throws prevent this structurally:
- Test exercises stub method → throw fires → test FAILS loudly
- CI integrates stub silently → throw triggers в first invocation
- Future-author forgets implementation needed → exception message reminds
- Stub mistakenly used as fallback в production → crashes loudly, не silently

**Paired discipline с Lesson #25 refined**: Lesson #25 says «design abstractions
when consumer materializes». Lesson #N12 says «if you must ship structural
surface ahead of implementation, make it loudly defensive».

**Pattern**:
```csharp
public void Method()
{
    throw new NotImplementedException(
        "Method pending [governing cascade/milestone]. " +
        "If this throws в test, the test is exercising [scope] " +
        "that [governing decision] explicitly scoped out " +
        "(Lesson #N12 Defensive Reserved Stub Pattern).");
}
```

**Anti-patterns**:
- Empty bodies `{ }`
- No-op stubs `return;`
- Default return values `return default;`
- Silent success patterns `return Result.Success;`

**First application**: К-extensions cascade #2 (2026-05-DD) —
DualFrontier.Launcher's RenderCommandDispatcher все 6 dispatch handler
methods throw NotImplementedException pending cascade #3 visual implementation.

**Promotion criterion (Provisional → FORMALIZE)**: Second application с
real evidence pattern reusable. Carry-forward к future К-extensions cascades
или V-series implementation work.
```

**Lesson #25 refined template**:
```markdown
### Lesson #25 (refined К-extensions cascade #2)

**Original statement** (А'.7.5 consumer materialization pattern):
"Design abstractions when consumer materializes."

**Refined statement** (К-extensions cascade #2 Q-G-3 deliberation 2026-05-23):
"Design abstractions when consumer materializes AND structurally eliminate
test-lying surface — empty stub implementations что pass tests by doing
nothing constitute architectural debt independent of speculation discipline."

**Crystalka's deliberation framing** (verbatim, deliberation session
2026-05-23):
> «Главное правило — не тестировать пустые интерфейсы и пустую реализацию
> которые врут в тестах, показывая что всё OK»

**Paired complement**: Lesson #N12 «Defensive Reserved Stub Pattern» —
covers cases где structural surface must ship ahead of consumer materialization.

**Combined discipline**:
- Don't design abstractions speculatively (Lesson #25 original)
- If you must, structurally prevent lying tests (Lesson #25 refined)
- If you must ship implementation skeleton, make it loudly defensive (Lesson #N12)
```

**Lessons batch counts update**:
- FORMALIZE: 12 → 12 (#25 refinement не FORMALIZE bump)
- DEFER (Provisional): 9 → 10 (#N12 added)
- SUNSET: 1 → 1 (unchanged)

**Commit message template**:
```
docs(methodology): K_EXT_2 ε2 — METHODOLOGY v1.10 → v1.11 (Lesson #N12 + #25 refined)

Per К-extensions cascade #2 Q-G-5 + Q-G-3 deliberation 2026-05-23:
- Version: 1.10 → 1.11 (new Lesson candidate + Lesson refinement)
- Lesson #N12 (Provisional, NEW): "Defensive Reserved Stub Pattern" —
  paired discipline с Lesson #25 refined; first application captured
  К-extensions cascade #2 (Launcher's RenderCommandDispatcher defensive throws)
- Lesson #25 refined: strengthened с lying-test prevention principle per
  Crystalka 2026-05-23 deliberation framing

Lessons batch counts:
- FORMALIZE: 12 → 12 (unchanged — #25 refinement не FORMALIZE bump)
- DEFER (Provisional): 9 → 10 (#N12 added)
- SUNSET: 1 → 1 (unchanged)

Promotion criterion для #N12: second application с reusable pattern.

К-L14 thesis interaction: Lesson #N12 supports К-L14 thesis (substrate
primitives unchanged через consumer transitions) by preventing lying-test
false positives masking real consumer regression.

Build verified: dotnet build exit 0 (documentation only).
```

#### Commit ε3 — PHASE_A_PRIME_SEQUENCING.md entry

**Scope**:
- Append к `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` cascade #2 chronological entry

**Entry template**:
```markdown
## К-extensions cascade #2 — Godot Full Deprecation + Launcher Formalization

**Designation**: К-extensions cascade #2 (post-А'.8 К-closure)  
**Dates**: 
- Deliberation session: 2026-05-23
- Brief authoring: 2026-05-23
- Execution: 2026-05-DD (Crystalka via Claude Code agent)
- Closure: 2026-05-DD

**Scope** (per Q-G-0 LOCKED Option X single large cascade):
- Phase α: Physical purge — Presentation.Native removed; src/DualFrontier.Presentation/
  untracked residue cleaned
- Phase β: Documentation cleanup tiered (Q-G-10) — Tier 1 mandatory 8 Application/* +
  Tier 2 mandatory docs/architecture grep + Tier 3/4 conditional
- Phase γ: Architectural decisions ratified (Q-G-1 IDevKitRenderer dormant;
  Q-G-2 Presentation.Native removed; Q-G-3 IRenderCommand pure marker;
  Q-G-5 no new К-L + Lesson #N12 candidate)
- Phase δ: Launcher project scaffold (Q-G-6 (b1) infrastructure-only с
  Defensive Reserved Stub dispatcher)
- Phase ε: Governance cascade (KERNEL v2.5→v2.5.1; METHODOLOGY v1.10→v1.11;
  new K_EXTENSIONS_LEDGER.md; REGISTER 3-commit cascade 2.3→2.4)
- Phase ζ: К-L14 verification #11 honest-framed removal-type evidence (Q-G-14)

**К-L impact**: zero (К-L count unchanged: 21 final).

**Lessons surfaced**:
- Lesson #N12 (Provisional, NEW): «Defensive Reserved Stub Pattern»
- Lesson #25 refined: lying-test prevention principle added

**К-L14 verification**: #11 (first removal-type evidence) — passing per
Q-G-14 honest-framed protocol.

**Atomic commits**: [N commits] (track via execution log).

**Cross-references**:
- tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md
- docs/architecture/K_EXTENSIONS_LEDGER.md §3.3
- docs/architecture/KERNEL_ARCHITECTURE.md v2.5.1
- docs/methodology/METHODOLOGY.md v1.11

**Forward sequencing**:
1. К-extensions cascade #3 — Launcher Visual Implementation (Basic Pawns + Items)
   — separate brief, deliberation framework, executed post-cascade-#2 closure
2. Pre-existing pollution cleanup cascade (`CreateLoop_RunningLoop_PawnStateCommandCarriesRealName`)
3. A'.9 Roslyn analyzer milestone
4. Phase B M-cycle preparation
```

**Commit message template**:
```
docs(sequencing): K_EXT_2 ε3 — PHASE_A_PRIME_SEQUENCING entry (К-extensions cascade #2)

Per К-extensions cascade #2: append chronological entry к Phase A' sequencing
document. Cascade scope, dates, К-L impact, lessons surfaced, atomic commit
count, cross-references documented.

Forward sequencing updated с cascade #3 + pollution cleanup + A'.9 + Phase B
preparation noted.

Build verified: dotnet build exit 0 (documentation only).
```

#### Commit ε4 — K_EXTENSIONS_LEDGER.md authored + K_CLOSURE_REPORT.md cross-ref

**Scope** — 2 changes в one commit (companion artifacts created together):

##### ε4.1 — Create `docs/architecture/K_EXTENSIONS_LEDGER.md`

Full content (see §3 Phase ε4 specification — abbreviated here):

```markdown
# К-extensions Cascade Ledger — Dual Frontier

**Document role**: Thematic narrative tracking of К-extensions cascades executed
post-А'.8 К-closure event boundary (2026-05-23). Sister artifact к:
- `K_CLOSURE_REPORT.md` (canonical К-series closure artifact, AUTHORED 2026-05-23)
- `K_L14_EVIDENCE_DASHBOARD.md` (К-L14 verification metrics + pass/fail evidence)
- `PHASE_A_PRIME_SEQUENCING.md` (chronological master timeline)

This ledger captures cascade-level decisions, scope, К-L impact, lessons surfaced —
narrative complement к metrics dashboard + chronological timeline.

---

## §1 — Purpose

К-extensions cascades execute architectural work что extends К-series invariants
beyond the formal closure event boundary. Each cascade:
- Verifies К-L14 thesis (substrate primitives unchanged через consumer exercise)
- May introduce new К-L sub-invariants (rare; cascade work usually preserves К-L count)
- Surfaces lessons added к METHODOLOGY Provisional pool либо FORMALIZE batch
- Documents architectural decisions ratified в deliberation Q-N

This ledger captures cascade narratives с designation, scope summary, К-L impact,
lessons, К-L14 verification number + status, and brief cross-reference.

---

## §2 — Cross-references

- **K_CLOSURE_REPORT.md** §1-12 — К-series canonical closure narrative
- **K_L14_EVIDENCE_DASHBOARD.md** — К-L14 verification metrics
- **PHASE_A_PRIME_SEQUENCING.md** — chronological master timeline
- **METHODOLOGY.md** — Lessons FORMALIZE/DEFER/SUNSET batches с cascade attribution
- **KERNEL_ARCHITECTURE.md** Part 0 К-L table — К-L count + status

---

## §3 — Cascade entries (chronological)

### §3.1 — К-extensions cascade #0 — А'.7.x BUS_ARCHITECTURE_AMENDMENT

**Designation**: К-extensions cascade #0  
**Dates**: Authored 2026-05-21, Executed 2026-05-21, Closed 2026-05-21  
**Brief**: `tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md`

**Scope summary**: Bus refactor (per-tier mutex split + O(N) coalesce + S10 cross-tier
re-entrancy probe) + 5 bug fixes from independent stress test investigation +
К-L15.1 sub-invariant LOAD-BEARING (2-layer state + runtime isolation).

**К-L impact**: К-L15.1 LOCKED (2-layer); 3rd layer deferred к А'.7.5. К-L count: 20 → 21.

**Lessons surfaced**: Lesson #N2 (mid-session brief amendment), #N5 (independent investigation),
#N6 (test fixture cleanup), #N7 (gap audit), #N8 (pre-flight reproduction), #N9 (closure-protocol gap),
#27 strengthened (third application).

**К-L14 verification**: #8 — Clean (+45% bus throughput, S10 ≤100ms).

**Atomic commits**: 13.

### §3.2 — К-extensions cascade #1 — А'.7.5 BUS_SOURCE_SPLIT

**Designation**: К-extensions cascade #1  
**Dates**: Authored 2026-05-22, Executed 2026-05-22, Closed 2026-05-22  
**Brief**: `tools/briefs/A_PRIME_7_5_BUS_SOURCE_SPLIT_BRIEF.md`

**Scope summary**: Pure code reorganization — К-L15.1 compile-time layer materialization
(3rd layer of 3-layer К-L15.1 sub-invariant). Helper primitives extracted; bus_native.cpp
source split к 4-file (К-L15.1 compile-time layer); stale O(N²) comment cleanup.

**К-L impact**: К-L15.1 3-layer manifestation complete. К-L count unchanged: 21.

**Lessons surfaced**: Lesson #25 application; #N6 second observation.

**К-L14 verification**: #9 — Clean (731 tests preserved).

**Atomic commits**: 5.

### §3.3 — К-extensions cascade #2 — Godot Full Deprecation + Launcher Formalization

**Designation**: К-extensions cascade #2  
**Dates**: Authored 2026-05-23, Executed 2026-05-DD, Closed 2026-05-DD  
**Brief**: `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md`

**Scope summary**: Godot full deprecation (physical purge + documentation cleanup
tiered + IDevKitRenderer dormant decoupling + IRenderCommand pure marker) +
Launcher project formalization (DualFrontier.Launcher infrastructure-only с
Defensive Reserved Stub dispatcher per Lesson #N12 first application).

Original Godot branch `2ba8130` discarded as obsolete precursor. Clean redo
на current main (`9ea5dbe`).

**К-L impact**: zero. К-L count unchanged: 21.

**Lessons surfaced**:
- Lesson #N12 (Provisional, NEW): «Defensive Reserved Stub Pattern» — first application
- Lesson #25 refined: lying-test prevention principle added
- Lesson #14 PROMOTED third application

**К-L14 verification**: #11 — First removal-type evidence. Pass per Q-G-14 honest-framed protocol.

**Atomic commits**: [N] (within 14-20 budget per Q-G-13).

**Closure notes**:
- KERNEL v2.5 → v2.5.1 (patch bump per Q-G-12 + versioning convention codified)
- METHODOLOGY v1.10 → v1.11 (Lesson #N12 added + Lesson #25 refined)
- register_version 2.3 → 2.4
- K_EXTENSIONS_LEDGER.md authored (this document)
- К-extensions cascade #3 scope split к separate brief

---

## §4 — Forward roadmap

Anticipated К-extensions cascades:
- **К-extensions cascade #3** — Launcher Visual Implementation (separate brief
  authored 2026-05-DD; executes post-cascade-#2 closure)
- **Future К-extensions cascades** — emergent per V-series, FO-series, Phase B
  needs (no predetermined timeline)

---
<!-- Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY -->
<!-- register_id: DOC-A-K_EXTENSIONS_LEDGER -->
<!-- category: A | tier: 2 | lifecycle: Live | owner: Crystalka -->
```

##### ε4.2 — Edit `docs/architecture/K_CLOSURE_REPORT.md` add cross-reference

Add cross-reference paragraph (in introduction OR closing section):

```markdown
## Post-closure note: К-extensions cascade tracking

К-extensions cascades что execute post-К-closure event boundary are tracked в
separate artifact: `K_EXTENSIONS_LEDGER.md` (Tier 2 Live Category A, authored
К-extensions cascade #2 2026-05-DD). This preserves K_CLOSURE_REPORT.md as
strict closure artifact (event boundary semantic intact) while providing
natural home для cascade narrative growth.

See `docs/architecture/K_EXTENSIONS_LEDGER.md` для cascade #0 (А'.7.x), cascade
#1 (А'.7.5), cascade #2 (Godot deprecation + Launcher), and future cascades.
```

**Patch-level edit only** — closure event boundary semantic strictly preserved. No version field change (K_CLOSURE_REPORT.md lifecycle is AUTHORED pending downstream review per Q-N-8-4 amendment).

**Commit message template**:
```
docs(architecture): K_EXT_2 ε4 — K_EXTENSIONS_LEDGER.md authored + K_CLOSURE cross-ref (Q-G-11)

Per К-extensions cascade #2 Q-G-11 LOCKED (d): create separate companion
document для К-extensions cascade narratives. Sister к K_CLOSURE_REPORT.md
(closure event boundary preserved) + K_L14_EVIDENCE_DASHBOARD.md (metrics) +
PHASE_A_PRIME_SEQUENCING.md (chronology).

K_EXTENSIONS_LEDGER.md initial content:
- §1 Purpose + scope
- §2 Cross-references к sister artifacts
- §3 Cascade entries (chronological):
  - §3.1 К-extensions cascade #0 (А'.7.x) — retroactive
  - §3.2 К-extensions cascade #1 (А'.7.5) — retroactive
  - §3.3 К-extensions cascade #2 (this cascade) — first authored entry
- §4 Forward roadmap (cascade #3 noted)

K_CLOSURE_REPORT.md cross-reference paragraph added — closure event boundary
semantic strictly preserved (no version bump, lifecycle unchanged).

REGISTER enrollment: DOC-A-K_EXTENSIONS_LEDGER Tier 2 Live Category A
(added в REGISTER cascade ε5).

Build verified: dotnet build exit 0 (documentation only).
```

#### Commit ε5 — REGISTER cascade Commit α (enrollments + retirements)

**Scope** — `docs/governance/REGISTER.yaml` mutations:

**Removals**:
- Delete entry `DOC-F-SRC-PRESENTATION-NATIVE` (full block)

**Additions**:
- Add `DOC-A-K_EXTENSIONS_LEDGER`:
  ```yaml
  - id: DOC-A-K_EXTENSIONS_LEDGER
    path: docs/architecture/K_EXTENSIONS_LEDGER.md
    title: "К-extensions Cascade Ledger — Dual Frontier"
    category: A
    tier: 2
    lifecycle: Live
    owner: Crystalka
    version: "Live"
    last_modified: "2026-05-DD"
    last_modified_commit: "PENDING-COMMIT-K_EXT_2-CLOSURE"
    content_language: mixed
    review_cadence: on-change+quarterly
    last_review_date: "2026-05-DD"
    last_review_event: "К-extensions cascade #2 authored ε4 — initial AUTHORED entry с retroactive cascades #0 + #1 + #2 (this cascade)"
    next_review_due: "2026-Q3"
    reviewer: Crystalka
    special_case_rationale: "Companion artifact к K_CLOSURE_REPORT.md tracking К-extensions cascade narratives post-А'.8 closure event boundary. Sister к K_L14_EVIDENCE_DASHBOARD.md (metrics) + PHASE_A_PRIME_SEQUENCING.md (chronology)."
  ```

- Add `DOC-F-SRC-LAUNCHER`:
  ```yaml
  - id: DOC-F-SRC-LAUNCHER
    path: src/DualFrontier.Launcher/README.md
    title: "DualFrontier.Launcher — Production Launch Entry Point"
    category: F
    tier: 4
    lifecycle: Live
    owner: Crystalka
    version: "Live"
    last_modified: "2026-05-DD"
    last_modified_commit: "PENDING-COMMIT-K_EXT_2-CLOSURE"
    content_language: en
    review_cadence: phase-led
    last_review_date: "2026-05-DD"
    last_review_event: "К-extensions cascade #2 δ3 — Launcher project README authored (infrastructure-only per Q-G-6 (b1); Defensive Reserved Stub dispatcher per Lesson #N12 first application)"
    next_review_due: "TBD — after cascade #3 closure"
    reviewer: Crystalka
  ```

**Verification gate**:
- `sync_register.ps1 --validate` exit 0
- REGISTER.yaml schema integrity preserved
- Auto-mirrored frontmatter в new doc files updates

**Commit message template**:
```
governance(register): K_EXT_2 ε5 — REGISTER Commit α (enrollments + retirements)

Per К-extensions cascade #2 Q-G-13 LOCKED (d) hybrid 3-commit REGISTER cascade:
Commit α (this commit) — DOC enrollments + retirements.

Added:
- DOC-A-K_EXTENSIONS_LEDGER (Tier 2 Live Category A) — companion artifact per Q-G-11
- DOC-F-SRC-LAUNCHER (Tier 4 Live Category F) — new Launcher project README per Q-G-6

Retired:
- DOC-F-SRC-PRESENTATION-NATIVE (Silk.NET+OpenGL stub project removed per Q-G-2)

sync_register.ps1 --validate gate: exit 0.
Build verified: dotnet build exit 0 (REGISTER.yaml + auto-mirror updates).

Forward Commits β + γ apply DOC modifications + register_version bump.
```

#### Commit ε6 — REGISTER cascade Commit β + γ (DOC modifications + register_version + EVT)

**Scope** — `docs/governance/REGISTER.yaml` mutations (combined β + γ per execution efficiency; if too large, split into ε6a + ε6b):

##### Commit β mutations (DOC modifications):

- `DOC-A-KERNEL`:
  - `version: "2.5"` → `version: "2.5.1"`
  - `last_modified`, `last_modified_commit`, `last_review_event` updates

- `DOC-A-METHODOLOGY`:
  - `version: "1.10"` → `version: "1.11"`
  - `last_modified`, `last_modified_commit`, `last_review_event` updates

- 8 Application/* DOC entries:
  - `DOC-F-SRC-APPLICATION`, `DOC-F-SRC-APPLICATION-BRIDGE`, `DOC-F-SRC-APPLICATION-RENDERING`,
    plus any other Application/* entries — `last_modified` + `last_review_event` updates

- `DOC-A-K_CLOSURE_REPORT`: `last_modified` update (cross-ref added в ε4)

- `DOC-A-K_L14_EVIDENCE_DASHBOARD`: `lifecycle` transition `AUTHORED-SKELETON` → `Live` (first populated entry — verification #11)

- Conditional: `DOC-A-VULKAN_SUBSTRATE` patch bump if Tier 2 grep findings warrant

##### Commit γ mutations (governance metadata):

- `register_version`: `"2.3"` → `"2.4"`
- `last_modified`: `"2026-05-DD"`
- `last_modified_commit`: `"PENDING-COMMIT-K_EXT_2-CLOSURE-RATIFICATION"`
- Add new EVT к `audit_trail`:
  ```yaml
  - event_id: EVT-2026-05-DD-K_EXT_2-GODOT_FULL_DEPRECATION
    date: "2026-05-DD"
    type: cascade_closure
    cascade: К-extensions cascade #2
    title: "Godot Full Deprecation + Launcher Formalization"
    summary: "Per К-extensions cascade #2 deliberation session 2026-05-23 + Crystalka ratification 2026-05-DD: physical purge of Godot/Silk.NET legacy paths + documentation cleanup tiered + Launcher project scaffold (infrastructure-only с Defensive Reserved Stub dispatcher per Lesson #N12 first application). К-L impact: zero. К-L14 verification #11 — first removal-type evidence (passing per Q-G-14 honest-framed protocol)."
    affected_docs:
      - DOC-A-KERNEL (v2.5 → v2.5.1)
      - DOC-A-METHODOLOGY (v1.10 → v1.11)
      - DOC-A-K_EXTENSIONS_LEDGER (newly enrolled)
      - DOC-F-SRC-LAUNCHER (newly enrolled)
      - DOC-F-SRC-PRESENTATION-NATIVE (retired)
      - DOC-A-K_CLOSURE_REPORT (cross-ref note added)
      - 8 DOC-F-SRC-APPLICATION/* entries (documentation cleanup)
      - DOC-A-K_L14_EVIDENCE_DASHBOARD (AUTHORED-SKELETON → Live)
    decisions_ratified:
      - Q-G-0 Option X (single large cascade)
      - Q-G-1 (c) IDevKitRenderer dormant
      - Q-G-2 (a) Remove Presentation.Native
      - Q-G-3 (a) IRenderCommand pure marker
      - Q-G-5 (a) No new К-L + Lesson #N12 candidate
      - Q-G-6 (b)+(i)+(b1) Standard launcher infrastructure-only
      - Q-G-7 (d) Hybrid orchestration
      - Q-G-8 (b) Executable-focused config
      - Q-G-9 (a) Keep SmokeTest
      - Q-G-10 (d) Tiered scan
      - Q-G-11 (d) K_EXTENSIONS_LEDGER companion
      - Q-G-12 (b) Patch bump
      - Q-G-13 (d) 3-commit REGISTER cascade
      - Q-G-14 (d) Honest-framed К-L14 verification #11
    lessons_surfaced:
      - Lesson #N12 (Provisional, NEW): "Defensive Reserved Stub Pattern"
      - Lesson #25 refined: lying-test prevention principle
      - Lesson #14 PROMOTED third application
    k_l_impact: "К-L count unchanged: 21 final. К-L14 evidence count 10 → 11 (verification #11 added — first removal-type)."
    verification:
      type: "Removal-type evidence (first of kind)"
      criterion: "Honest-framed per Q-G-14 LOCKED — 7-point post-cascade verification matrix + 6-condition falsifiability checklist"
      status: "[TBD — populate post-execution closure: CLEAN / SOFT-HALT / FAIL]"
    brief: tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md
    cross_references:
      - docs/architecture/K_EXTENSIONS_LEDGER.md §3.3
      - docs/architecture/KERNEL_ARCHITECTURE.md v2.5.1
      - docs/methodology/METHODOLOGY.md v1.11
      - docs/architecture/PHASE_A_PRIME_SEQUENCING.md cascade #2 entry
  ```

- Add CAPA entries if Tier 2 grep findings surfaced drift requiring remediation tracking

**Verification gate**:
- `sync_register.ps1 --validate` exit 0
- register_version field shows "2.4"
- audit_trail entry present + well-formed
- Auto-mirrored frontmatter в touched doc files updates

**Commit message template**:
```
governance(register): K_EXT_2 ε6 — REGISTER Commits β + γ (DOC mods + register_version + EVT)

Per К-extensions cascade #2 Q-G-13 LOCKED (d) hybrid 3-commit REGISTER cascade:
Commits β + γ combined (DOC modifications + register_version bump + audit_trail).

Modified (Commit β):
- DOC-A-KERNEL: version 2.5 → 2.5.1 (Q-G-12 patch bump)
- DOC-A-METHODOLOGY: version 1.10 → 1.11 (Lesson #N12 + #25 refined)
- 8 Application/* DOCs: last_modified + last_review_event updates
- DOC-A-K_CLOSURE_REPORT: last_modified update (cross-ref added в ε4)
- DOC-A-K_L14_EVIDENCE_DASHBOARD: lifecycle AUTHORED-SKELETON → Live (verification #11)
- [conditional]: DOC-A-VULKAN_SUBSTRATE patch bump if Tier 2 findings warrant

Modified (Commit γ):
- register_version: 2.3 → 2.4
- Added к audit_trail: EVT-2026-05-DD-K_EXT_2-GODOT_FULL_DEPRECATION
- Added к CAPA entries (if Tier 2 findings warrant): [list]

sync_register.ps1 --validate gate: exit 0.
Build verified: dotnet build exit 0.

К-extensions cascade #2 governance cascade complete.
Brief AUTHORED → EXECUTED transition в ε7 final commit.
```


#### Commit ε7 — Brief AUTHORED → EXECUTED + closure section

**Scope**:
- Edit `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md`:
  - Status header: `AUTHORED — pending Crystalka RATIFICATION + Claude Code execution` →
    `EXECUTED — К-extensions cascade #2 closure 2026-05-DD`
  - Append `## §9 — Closure section` documenting actual execution outcomes

**Closure section template**:

```markdown
## §9 — Closure section (К-extensions cascade #2 execution outcomes)

**Executed**: 2026-05-DD (Crystalka via Claude Code execution agent)
**Closure timestamp**: [commit hash of final commit]
**Pushed к origin/main**: 2026-05-DD

### §9.1 — Atomic commit summary

[Populate с actual commit hashes + descriptions per execution]

Total commits: [N] (within Q-G-13 budget 14-20)

### §9.2 — К-L14 verification #11 outcome

Per Q-G-14 LOCKED honest-framed protocol:

| Criterion | Pre-cascade baseline | Post-cascade state | Pass/Fail |
|---|---|---|---|
| 1. Build integrity | dotnet build exit 0 | [actual] | [✓/✗] |
| 2. Test count parity | [N tests] | [actual] | [✓/✗] |
| 3. Pre-existing pollution status | 1 known (CreateLoop_RunningLoop_PawnStateCommandCarriesRealName) | [actual] | [✓/✗] |
| 4a. Runtime SmokeTest | passes | [actual] | [✓/✗] |
| 4b. Runtime API surface | unchanged | [actual] | [✓/✗] |
| 4c. Vulkan validation clean | no errors | [actual] | [✓/✗] |
| 5. Launcher composition smoke | N/A (new) | [actual] | [✓/✗] |
| 6. Solution integrity | 32 projects | 32 projects | [✓/✗] |
| 7. METHODOLOGY §12.7 gate | passes | [actual] | [✓/✗] |

К-L14 verification #11 final status: **[CLEAN / SOFT-HALT / FAIL]**

### §9.3 — Lessons reaffirmed/refined

- Lesson #N12 «Defensive Reserved Stub Pattern» — first application complete
  (Launcher's RenderCommandDispatcher 6 defensive throws). Promotion criterion
  (Provisional → FORMALIZE) requires second application; carry-forward к future
  cascades.
- Lesson #25 refined — operationalized в Q-G-3 + Q-G-6 decisions.
- Lesson #14 PROMOTED — third application (Godot deprecation arc completion
  as pre-existing drift cleanup).

### §9.4 — Forward cascade #3 prerequisites confirmed

Cascade #3 (Launcher Visual Implementation) prerequisites established by cascade #2:
- ✓ Launcher project exists с infrastructure-only scaffold
- ✓ RenderCommandDispatcher с 6 defensive throws (ready для replacement)
- ✓ IRenderCommand pure marker (data records ready для dispatch)
- ✓ PresentationBridge ↔ Launcher wiring connected
- ✓ Runtime substrate primitives accessible (Window + Vulkan + Camera + SpriteRenderer)
- ✓ К-L14 baseline established (verification #11 evidence)

К-extensions cascade #3 brief authoring deferred к post-cascade-#2 closure session
(Crystalka direction: «после исполнения в сесии claude code я приложу отчёт и мы 
продолжим уже делать второй»).

### §9.5 — Cascade closure ratification

Crystalka ratification: [confirm at execution time]
Final commit pushed к origin/main: [commit hash + timestamp]
К-extensions cascade #2 formally CLOSED.

**Forward task**: К-extensions cascade #3 — Launcher Visual Implementation
(separate session, separate brief, post-cascade-#2-closure baseline).
```

**Commit message template**:
```
governance(brief): K_EXT_2 ε7 — Brief AUTHORED → EXECUTED + closure section

Per К-extensions cascade #2 closure protocol: transition brief status к
EXECUTED + append closure section documenting actual execution outcomes.

Closure section contents:
- §9.1 Atomic commit summary с hashes
- §9.2 К-L14 verification #11 outcome table (7 criteria + 6 falsifiability conditions)
- §9.3 Lessons reaffirmed/refined narrative
- §9.4 Cascade #3 prerequisites confirmation
- §9.5 Cascade closure ratification

К-extensions cascade #2 formally CLOSED.
Forward: К-extensions cascade #3 (Launcher Visual Implementation) via separate
session + brief.

Final build verified: dotnet build exit 0.
Final sync_register.ps1 --validate: exit 0.
```

### Phase ζ — Verification (0 separate commits)

Verification protocol executed at cascade closure, не as separate commits. Records appended к ε7 brief closure section.

**Verification execution checklist** (run before ε7):

1. **Build integrity check**: `dotnet build` exit 0 (full solution)
2. **Test suite execution**: `dotnet test` → record test count + per-suite results
3. **Pre-existing pollution check**: confirm `CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` status (passes в isolation, fails в full Modding suite) — same as pre-cascade baseline (NOT worse)
4. **Runtime SmokeTest execution**: `dotnet run --project tests/DualFrontier.Runtime.SmokeTest/` — verify passes (window opens, sprite renders, V1 diffusion registers)
5. **Runtime API surface check**: `dotnet build` exit 0 verifies no breaking API changes
6. **Vulkan validation check**: Runtime SmokeTest run shows no Vulkan validation layer errors
7. **Launcher composition smoke**: `dotnet run --project src/DualFrontier.Launcher/` — verify window opens, then close gracefully (defensive throws will fire if any visual path invoked, expected behavior)
8. **Solution project count**: `dotnet sln list | wc -l` → confirm 32 projects (Presentation.Native -1, Launcher +1)
9. **METHODOLOGY §12.7 Modding-suite verification gate**: per protocol
10. **`sync_register.ps1 --validate`**: exit 0 after all REGISTER mutations

**Falsifiability conditions** — if any of these trigger during verification:
- ✗ Test count decreases beyond pre-existing pollution adjustment
- ✗ Runtime API breaking change introduced
- ✗ SmokeTest regresses (anything that passed pre-cascade now fails)
- ✗ Vulkan validation errors appear
- ✗ Launcher fails к construct (composition exception)
- ✗ Pre-existing pollution worsens (more tests fail than baseline)

→ **HALT execution**. Crystalka ratification re: continuation OR roll-back required.

---

## §4 — Halt conditions

Execution agent MUST halt + request Crystalka ratification on any of the following:

### 4.1 — Brief scope violations

- **Halt condition SC-1**: Attempt к add visual implementation (SpriteCatalog, scene state,
  real dispatcher body) — S-LOCK-3 violation. Cascade #3 territory.
- **Halt condition SC-2**: Attempt к delete IDevKitRenderer interface OR add implementation —
  S-LOCK-6 violation. Q-G-1 LOCKED dormant status.
- **Halt condition SC-3**: Attempt к retain IRenderCommand.Execute() in any form —
  S-LOCK-5 violation. Q-G-3 LOCKED marker-only.
- **Halt condition SC-4**: Empty body, no-op stub, либо silent default-return в any
  defensive dispatcher arm — S-LOCK-4 violation. Lesson #N12 first application
  requires loud throws с descriptive messages.
- **Halt condition SC-5**: Partial removal of DualFrontier.Presentation.Native (e.g. csproj
  retained, code deleted, либо vice versa) — S-LOCK-7 violation.

### 4.2 — К-L14 verification falsifiability triggers

- **Halt condition VF-1**: Test count decreases beyond pre-existing pollution adjustment
- **Halt condition VF-2**: Runtime API breaking change introduced
- **Halt condition VF-3**: Runtime SmokeTest regresses
- **Halt condition VF-4**: Vulkan validation errors appear during SmokeTest
- **Halt condition VF-5**: Launcher fails к construct (Main() throws before window present)
- **Halt condition VF-6**: Pre-existing pollution worsens (more tests fail than baseline)

### 4.3 — Tier 2 grep escalation

- **Halt condition TR-1**: Tier 2 grep returns > 20 hits across > 5 files. May indicate
  Tier 3/4 escalation needed within cascade #2 OR deferral к separate cleanup cascade
  per S-LOCK-8 out-of-scope protocol.

### 4.4 — Pre-execution Phase 0 reads inconsistencies

- **Halt condition P0-1**: GameBootstrap.CreateLoop signature differs from brief assumption
  (e.g. additional parameters, different return type)
- **Halt condition P0-2**: GameLoop public API differs from brief assumption (e.g. no Tick()
  method, или different signature)
- **Halt condition P0-3**: RuntimeOptions surface differs from brief assumption (e.g. no
  AssetsDirectory property)
- **Halt condition P0-4**: InputEventQueue.TryDequeue signature differs
- **Halt condition P0-5**: Any other production wiring assumption violated

For halts P0-*: brief amendment may be required (signature adjustments) OR Crystalka
ratification re: alternative wiring pattern.

### 4.5 — sync_register.ps1 validation failure

- **Halt condition SR-1**: `sync_register.ps1 --validate` returns non-zero exit code
  at any REGISTER cascade commit. Block subsequent commits until validation passes.

### 4.6 — Build break

- **Halt condition BB-1**: `dotnet build` returns non-zero exit code at any commit per
  Lesson #8 strengthened atomic compilable commits. Block subsequent commits.

### 4.7 — Auto-mode classifier push block (operational reminder)

- **Operational note** (не halt condition, expected behavior): Claude Code auto-mode
  classifier blocks push-to-main even с explicit user instruction в initial prompt.
  Requires в-session re-confirmation after halt + resolution work. Expected behavior,
  не bug. Push step explicitly requires Crystalka re-confirmation в-session.

---

## §5 — Closure protocol

Per METHODOLOGY v1.10 §12.7 closure protocol + cascade-specific extensions:

### 5.1 — Pre-closure verification execution

Execute Phase ζ verification protocol (§3 Phase ζ checklist). Document outcomes
в brief ε7 closure section.

### 5.2 — К-L14 verification #11 evidence capture

Populate `K_L14_EVIDENCE_DASHBOARD.md` с verification #11 entry. Transition document
lifecycle `AUTHORED-SKELETON` → `Live`.

Evidence entry template:
```markdown
## Verification #11 — К-extensions cascade #2 (Godot Full Deprecation + Launcher Formalization)

**Date**: 2026-05-DD
**Type**: Removal-type (first of kind)
**Cascade**: К-extensions cascade #2

**Pre-cascade baseline**:
- Tests: [N] passing
- Pre-existing pollution: 1 known (CreateLoop_RunningLoop_PawnStateCommandCarriesRealName)
- Runtime SmokeTest: passes
- Solution: 32 projects

**Post-cascade state**:
- Tests: [N] passing
- Pre-existing pollution: 1 known (unchanged)
- Runtime SmokeTest: passes
- Solution: 32 projects (Presentation.Native -1, Launcher +1)
- Vulkan validation: clean
- Launcher composition smoke: passes

**Status**: [CLEAN / SOFT-HALT / FAIL]

**Honest-framed evidence note**: First removal-type К-L14 verification. Substrate
(Runtime) primitives unchanged through removal of dead consumer scaffold
(Presentation.Native) + addition of new consumer (Launcher). К-L14 thesis preserved:
substrate exhibits stability across consumer churn.
```

### 5.3 — Crystalka ratification

Brief execution agent reports closure-ready state к Crystalka. Crystalka:
1. Reviews execution log + final commit list
2. Verifies К-L14 verification #11 evidence
3. Approves OR requests amendment (mid-cascade amendment per Lesson #N2 pattern)
4. Authorizes push к origin/main

### 5.4 — Push к origin/main

- Push feature branch к origin
- Merge к main (fast-forward или merge commit per Crystalka discretion)
- Push main к origin/main

К-extensions cascade #2 event boundary timestamp = push к origin/main.

### 5.5 — Post-closure governance

- Brief AUTHORED → EXECUTED (already в ε7)
- REGISTER `last_modified_commit` field updates resolve `PENDING-COMMIT-K_EXT_2-CLOSURE`
  к actual closure commit hash
- K_EXTENSIONS_LEDGER.md §3.3 cascade entry finalized с actual dates + commit count
- PHASE_A_PRIME_SEQUENCING.md cascade #2 entry finalized

### 5.6 — Forward sequencing

Per A'.8 Session 1 LOCKED forward sequencing (cascade #2 LOCKED here):
1. **Next**: К-extensions cascade #3 — Launcher Visual Implementation
   (separate brief, authored в next deliberation session post-cascade-#2 closure
   per Crystalka direction)
2. **After cascade #3**: Pre-existing pollution cleanup cascade
3. **After pollution**: V2 amendment brief authoring
4. **After V2**: A'.9 Roslyn analyzer milestone
5. **After analyzer**: Mod API lock milestone
6. **Phase B M-cycle preparation**

---

## §6 — Forward consideration: post-cascade analysis

К-extensions cascade #2 closure outcomes may surface forward considerations:

### 6.1 — К-L17.1 sub-invariant candidacy (deferred)

Per Q-G-5 deliberation: К-L17.1 «Renderer-to-CompositionFramework binding»
sub-invariant candidate noted, deferred к когда Launcher's IRenderer implementation
stable + composition framework binding tested under real load. Cascade #3 (real
visual implementation) is natural surface для potentially LOCKING К-L17.1.

### 6.2 — Lesson #N12 second application opportunity

Cascade #3 (visual implementation) will **replace** Defensive Reserved Stub
throws с real implementations. This is **not** a second application of Lesson #N12
(it's the resolution of первой application). Second application requires
**different** reserved stub surface — потенциально IDevKitRenderer materialization
либо new architectural skeleton.

### 6.3 — A'.9 Roslyn analyzer preparation

Cascade #2 establishes clean baseline для A'.9 analyzer rule definition:
- DF015.1 (К-L15.1) Error — already LOCKED А'.8
- Future rules могут leverage cascade #2 architectural decisions (е.g. IRenderCommand
  marker-only pattern can be enforced via analyzer rule «commands MUST не have
  Execute() method»)
- Defensive Reserved Stub Pattern (Lesson #N12) потенциально codifiable as analyzer
  rule «interface implementations MUST не have empty/no-op bodies»

A'.9 brief authoring (post-Phase B preparation) will incorporate these signals.

---

## §7 — Cross-references

### 7.1 — Predecessor briefs

- `tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md` — К-extensions cascade #0
- `tools/briefs/A_PRIME_7_5_BUS_SOURCE_SPLIT_BRIEF.md` — К-extensions cascade #1
- `tools/briefs/K_CLOSURE_AUTHORING_BRIEF.md` — А'.8 К-closure brief
- `tools/briefs/GODOT_REMOVAL_BRIEF.md` (archived/historical) — original Godot removal
  brief that authored branch `2ba8130` (discarded as obsolete precursor per S-LOCK-1)

### 7.2 — Authoritative artifacts

- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.5.1 — К-L invariants canonical
- `docs/methodology/METHODOLOGY.md` v1.11 — Lessons + process invariants
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 LOCKED — Vulkan substrate spec
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.11 LOCKED — Mod OS spec
- `docs/architecture/K_CLOSURE_REPORT.md` AUTHORED — К-series canonical closure
- `docs/architecture/K_EXTENSIONS_LEDGER.md` Live — cascade narratives
- `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` Live — К-L14 verification metrics
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` Live — chronological timeline
- `docs/governance/REGISTER.yaml` register_version 2.4 — governance SoT

### 7.3 — Successor brief (forward reference)

- `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` — К-extensions
  cascade #3 (Launcher Visual Implementation) — authored в next deliberation
  session post-cascade-#2 closure per Crystalka direction

---

## §8 — Appendix — Q-N lock summary

Per deliberation session 2026-05-23 — 15 Q-N ratified в advance of brief authoring:

| Q | Decision | Rationale |
|---|---|---|
| Q-G-0 | Option X (single large cascade) | 1M context window resolves split necessity; cleanup→analyzer adjacency |
| Q-G-1 | (c) Keep IDevKitRenderer dormant, decoupled from Godot | Forward DevKit work planned by Crystalka; preserves architectural intent |
| Q-G-2 | (a) Remove Presentation.Native outright | Silk.NET path abandoned; existing stubs were lying-test surface |
| Q-G-3 | (a) IRenderCommand as pure marker, drop Execute() | Lesson #25 refined — empty Execute() bodies = lying tests |
| Q-G-4 | SUBSUMED by Q-G-3 | Execute() bodies removed via Q-G-3 mechanism |
| Q-G-5 | (a) No new К-L + Lesson #N12 candidate captured | Cleanup cascade; new pattern «Defensive Reserved Stub» surfaced |
| Q-G-6 | (b)+(i)+(b1) Standard launcher infrastructure | Cascade #2 ships infrastructure; cascade #3 ships visuals |
| Q-G-7 | (d) Hybrid orchestration в Program.cs | Explicit > implicit для forward debugability |
| Q-G-8 | (b) Executable-focused config | Launcher is executable, не library |
| Q-G-9 | (a) Keep SmokeTest as is | Runtime substrate isolation preserved |
| Q-G-10 | (d) Tiered scan с conditional expansion | Lesson #14 default-include + evidence-driven scope |
| Q-G-11 | (d) Separate K_EXTENSIONS_LEDGER.md companion | Closure event boundary preserved; thematic vs chronological |
| Q-G-12 | (b) Patch bump v2.5 → v2.5.1 + versioning convention | Cleanup = patch (not minor); convention codified |
| Q-G-13 | (d) Hybrid 3-commit REGISTER cascade | Granular enough + atomic enough |
| Q-G-14 | (d) Honest-framed removal-type evidence | К-L14 thesis empirical credibility preserved |

---

**End of brief**

**Authoring metadata**:
- Authored: 2026-05-23 by Claude Opus 4.7 (deliberation mode)
- Authored on behalf of: Crystalka (Volodymyr, solo dev)
- Authoring session: К-extensions cascade #2 deliberation session 2026-05-23
- Authoring filesystem: bash staging pattern (`/home/claude/staging/` →
  `/mnt/user-data/outputs/`)
- Project: Dual Frontier (Crystalka228/Dual-Frontier)

**Status at authoring**: AUTHORED — pending Crystalka RATIFICATION + Claude Code execution
**Status transitions**: AUTHORED → RATIFIED (Crystalka) → EXECUTING → EXECUTED → CLOSED
