# Godot Removal Cascade — Deliberation + Execution Brief

**Status**: AUTHORED 2026-05-21 (Claude Opus 4.7, A'.8 deliberation Session 1 close session)
**Brief type**: **Deliberation framework + Execution spec hybrid** (Phase 0 reads inform conditional execution sections)
**Cascade type**: Parallel operational cascade — independent от A'.8 K-closure cadence
**Branch strategy**: Separate branch `claude/godot-removal-deliberation-Vfg2R`; merge к main at Crystalka discretion post-deliberation + execution completion
**Forward sequencing impact**: NONE к Q3 LOCKED (A'.8) sequencing diagram; parallel track footnote applies к К-closure report Section 9

**Project context**:
- Dual Frontier research framework
- Repository: `Crystalka228/Dual-Frontier`
- Discipline: «без костылей», К-L14 default-inclusion bias, decade-horizon investment
- К-series final at А'.8 closure (20 К-L invariants + К-L20 reserved post-Mod API lock)
- V substrate: V0.A/V0.B/V0.C.1/V0.C.2 closed; V1 partial (PR #40 merged); V2 deferred к post-A'.8

**Cascade purpose** (verbatim from Crystalka direction):
> «Удаление всех файлов Godot из проекта так как мы уже используем Vulkan прямую компиляцию PNG»

**Architectural framing**:
- Godot files = pre-existing infrastructure (used during early development phase)
- V substrate (К10.x + V0.x cascades) established direct Vulkan compute substrate + PNG asset pipeline
- К-L9 «vanilla = mods» implies vanilla content authored against IModApi NOT against Godot Node hierarchy
- Removal = «без костылей» discipline application к accumulated infrastructure
- Lesson #14 (Provisional, second clean application precedent) — pre-existing drift cleanup as separate cascade

---

## §1 — Deliberation framework

### §1.1 — Brief authoring caveat

This brief authored **without Phase 0 reads completed** in current session — Crystalka chose к defer detailed analysis к next session начало. Brief structure provides:

1. **Phase 0 reads enumeration** (§2) — mandatory before any execution
2. **Q-N deliberation surface** (§3) — 10 architectural questions для next session ratification
3. **Conditional execution sections** (§4-§7) — execution specs depend на Q-N ratified decisions
4. **Halt triggers** (§8) — explicit halt conditions для execution session
5. **Documentation cascade** (§9) — REGISTER amendments + audit_trail events
6. **Closure protocol** (§10) — Phase N verification

**Brief usage pattern**:
1. Next Opus session opens this brief
2. Phase 0 reads execution (mandatory)
3. Q-N deliberation (ratifies decisions based on Phase 0 evidence)
4. Brief amendment OR sub-brief authoring per ratified decisions
5. Claude Code execution session (sub-brief OR amended brief)
6. PR review + merge к main

### §1.2 — Why deliberation framework hybrid

Per research framework discipline:

**Lesson #9 Survey phase before brief authoring**: Phase 0 reads identify actual Godot scope, code anchors, placeholder vs production status, infrastructure inheritance.

**Lesson #16 Brief length scales с deliberation complexity**: Godot removal scope **unknown** без Phase 0 — brief size cannot finalize ahead of evidence.

**Lesson #25 Implementation depth follows consumer materialization**: removal depth determined by V substrate sufficiency assessment (Q-6) — что заменяет Godot capabilities?

**Lesson #N2 Mid-session brief amendment via halt-before-damage Path 2**: this brief explicitly invites amendment post-Phase 0; не fixed specification.

**Lesson #26 Cross-substrate scope splitting**: if Godot scope substantial → split к sub-cascades at architectural boundaries.

### §1.3 — К-L14 default-inclusion bias application

К-L14 thesis (LOCKED A'.8) — «performance derives from clean complex architecture»:

**Godot removal architectural justification** (must be specific reason, не tactical heuristic):
- Godot lifecycle conflicts с К-L12 native kernel scheduling sovereignty (potential)
- Godot Node hierarchy conflicts с composition-based ECS architecture (memory #18: «OOP deep hierarchies incompatible с LLM execution, composition over inheritance»)
- Godot save/load conflicts с К-L8 native ownership (potential)
- Godot Mono runtime constraints versus pure .NET 8+ ergonomics (commercial story per Crystalka's vision)
- К-L9 «vanilla = mods» pattern conflicts с Godot-specific vanilla code paths

**Falsifiability commitment**: removal may **reveal** Godot was providing structural value не previously articulated. Phase 0 surfaces this if so. Halt-before-damage triggers (Lesson #N2) if discovered mid-execution.

---

## §2 — Phase 0 reads (MANDATORY before execution)

### §2.1 — Repository Godot inventory

Phase 0 reads enumerate actual Godot scope в repository. Required reads:

**Project structure**:
- `project.godot` — Godot project manifest (if exists)
- `.godot/` directory contents (engine cache, possibly excluded from git)
- Root-level `*.csproj` files — check `<Project Sdk="Godot.NET.Sdk/...">` references
- Solution file (`*.sln`) — identify Godot-related projects

**Godot scene + resource files**:
- `*.tscn` scene files — count, identify production vs prototype scenes
- `*.tres` resource files — material, theme, environment resources
- `*.gd` GDScript files (if any)
- `*.import` asset import metadata files

**Godot-derived C# code**:
- Grep для `: Node` (class inheritance pattern)
- Grep для `: Node2D`, `: Node3D`, `: Control`, `: CanvasLayer`, `: Sprite`, `: Sprite2D`, etc.
- Grep для `_Ready()`, `_Process(delta)`, `_PhysicsProcess`, `_ExitTree`, `_Input` — Godot lifecycle method usage
- Grep для `using Godot;` — namespace import inventory
- Grep для `GD.Print`, `GD.Load`, `GD.ResourceLoader` — Godot static API usage

**Application entry point**:
- `GameBootstrap.cs` — is this Godot scene script (Node-derived) OR pure .NET Main()?
- `Program.cs` — pure .NET entry point existence?
- Main scene reference в project.godot

**Asset pipeline**:
- `assets/` directory structure
- PNG asset import — Godot .import metadata files vs V0.C.1 pure pipeline
- Texture import settings — Godot Texture2D resources vs Vulkan-native loading

**Build pipeline**:
- `build.cmd` / build scripts — Godot export vs `dotnet build` vs native cmake
- CI pipeline (if exists) — Godot dependency
- Output binaries — Godot-mediated executable vs standalone .NET host

### §2.2 — К-L17 / К-L18 implementation reads

**К-L17 display composition multi-layer** (LOCKED A'.8):
- Source files implementing display composition (К10.3 v2 cascade output)
- Check if uses Godot CanvasLayer pattern OR pure Vulkan render passes
- Layer types: sim state + intent overlay + combat feedback + static
- Файл(ы): `src/DualFrontier.Application/Display/` OR similar

**К-L18 mod lifecycle quiescent state** (LOCKED A'.8):
- Source files implementing mod lifecycle quiescent state
- Check if uses Godot pause OR pure managed signal
- Quiescent state precondition: sim paused + pipeline quiescent

### §2.3 — V substrate scope coverage assessment

Critical Phase 0 read — does V substrate currently provide replacement для all Godot capabilities?

**Window management**:
- V substrate creates Vulkan-compatible window? OR Godot provides window?
- VkSurfaceKHR creation pattern — VK_KHR_win32_surface direct OR Godot-mediated?
- Window event polling — input events flow source

**Swapchain management**:
- VkSwapchainKHR creation + present pattern
- Swapchain image acquisition + present queue submission
- VSync / present mode configuration

**Input handling**:
- Keyboard input event source
- Mouse input event source
- Gamepad support (если applicable)
- Input mapping configuration

**Asset loading**:
- PNG loading pipeline (V0.C.1 closed — confirm full coverage)
- Asset manifest discovery
- Hot-reload support (если present)

**Audio** (если applicable):
- Audio output infrastructure
- Sound asset loading

**UI rendering**:
- К-L17 display composition multi-layer implementation
- Text rendering (font handling)
- UI primitive draw calls

### §2.4 — Application lifecycle reads

**Current entry point pattern**:
- If GameBootstrap.cs = Godot Node — needs migration к pure .NET Main()
- If GameBootstrap.cs = pure .NET — Godot lifecycle hooks (если any) need removal
- Initialization order: native kernel init → V substrate init → game systems → main loop

**Main loop pattern**:
- Godot `_Process(delta)` callback model — frame-driven main loop
- Pure standalone — explicit `while (running) { Tick(); Render(); Sleep(); }` pattern
- К10.x native scheduler ownership — sovereignty over tick loop

### §2.5 — Test infrastructure reads

**Test execution**:
- Tests run via `dotnet test` (typical .NET pattern) OR Godot test runner?
- 1022 tests baseline (per К10.3 v2 closure) — verify які use Godot dependencies
- Headless test execution (CI compatibility)

---

## §3 — Q-N deliberation surface (next Opus session ratifies)

These 10 Q-N surface для next session deliberation. Phase 0 evidence informs each answer. Q-N ratification triggers brief amendment OR sub-brief authoring per ratified decisions.

### Q-1: Window + swapchain management strategy

**Question**: After Godot removal, how does application acquire Vulkan-compatible window + manage swapchain?

**Options**:
- (a) Direct Win32 (HWND + VK_KHR_win32_surface) — minimal external dependencies, Windows-specific
- (b) GLFW library — cross-platform window + input, well-established Vulkan companion
- (c) SDL2 / SDL3 library — cross-platform, broader scope (audio, controllers)
- (d) Custom minimal — windowing primitives authored as V substrate extension

**My provisional leaning**: Option (a) Direct Win32 initially + Option (b) GLFW migration when cross-platform need materializes.

### Q-2: К-L17 display composition implementation refactor scope

**Question**: К-L17 LOCKED implementation uses Godot CanvasLayer OR pure Vulkan?

**Phase 0 reads** §2.2 answer this empirically.

### Q-3: Application entry point pattern

**Question**: After Godot removal, what is application entry point?

**My provisional leaning**: Option (a) Program.cs Main() — pure .NET standalone with `[DllImport]` для native kernel.

### Q-4: К10.3 v2 К-L17/L18 Godot dependency check

**Question**: K10.3 v2 cascade (closed 2026-05-20) К-L17/L18 implementation files — Godot dependencies?

### Q-5: Godot scope inventory disposition

**Question**: After Phase 0 inventory of Godot files (§2.1), what removal pattern applies?

**My provisional leaning**: Option (c) migration cascade — atomic compilable commits + cross-substrate scope splitting.

### Q-6: V substrate sufficiency assessment

**Question**: Does V substrate (V0.A through V1) currently provide all replacements for Godot capabilities?

### Q-7: Migration cascade vs direct removal pattern

**Question**: Single PR direct removal OR phased migration cascade?

### Q-8: К-L18 mod lifecycle quiescent implementation refactor

**Question**: К-L18 LOCKED implementation uses Godot pause mechanism?

### Q-9: Cascade placement in operational timeline

**Question**: When does Godot removal cascade execute relative к other parallel tracks?

**My provisional leaning**: Option (c) parallel к bus refactor.

### Q-10: Decade-horizon framing — «без костылей» justification specifics

**Question**: What specific architectural reasons demand Godot removal?

**Candidate reasons**:
1. Composition-based architecture conflict — Godot Node hierarchy = deep OOP inheritance
2. К-L12 native kernel sovereignty — Godot owns main loop traditionally
3. К-L9 «vanilla = mods» purity — vanilla mods authored against IModApi
4. Pure .NET runtime advantages — .NET 8/10 LTS + cross-platform runtime
5. LLM pipeline compatibility — Godot Node hierarchy generates OOP patterns LLM-incompatible
6. Tooling simplicity — Godot editor not used per «без костылей»
7. Asset pipeline duplication — Godot .import system + V0.C.1 pipeline = redundant

---

## §4 — Conditional execution sections (depend on Q-N ratification)

### §4.1 — Execution conditional on Q-N ratification

After next Opus session ratifies Q-1 through Q-10, execution specs finalize. Brief amendment OR sub-brief authoring follows.

### §4.2 — Migration cascade sub-cascade structure (provisional per Q-5/Q-7 Option c leaning)

If migration cascade pattern ratified:

**Sub-cascade 1 — Application entry point migration**
**Sub-cascade 2 — Window + swapchain management** (depends on Q-1)
**Sub-cascade 3 — К-L17 display composition refactor** (conditional on Q-2 Scenario B)
**Sub-cascade 4 — К-L18 mod lifecycle refactor** (conditional on Q-8 Scenario B)
**Sub-cascade 5 — Asset pipeline cleanup**
**Sub-cascade 6 — Test infrastructure migration** (if applicable)
**Sub-cascade 7 — Godot SDK references removal**
**Sub-cascade 8 — Godot file deletion**

### §4.3 — Alternative — direct removal pattern (if Q-5/Q-7 Option a ratified)

If single atomic commit ratified — high risk for К-L8 violation unless Godot project not in main solution.

---

## §5 — Documentation cascade

REGISTER.yaml amendments, К-closure report addendum, METHODOLOGY.md amendments, KERNEL_ARCHITECTURE.md amendments.

## §6 — Atomic commit structure (provisional)

Per Lesson #8 atomic compilable commits + Lesson #26 cross-substrate splitting. Estimated commit count: 10-15 commits across sub-cascades — final count depends on Phase 0 scope discovery.

## §7 — Test infrastructure considerations

Preserve 1022 baseline. Stress test suite preserved. Visual regression if К-L17 refactor.

## §8 — Halt triggers

**SC-A** — К-L17/L18 Godot dependency discovery
**SC-B** — К-L17/L18 implementation refactor exceeds parallel cascade scope
**SC-C** — V substrate gap discovered
**SC-D** — Test suite regression beyond tolerance
**SC-E** — Architectural justification insufficient mid-execution
**SC-F** — К-L14 thesis falsification candidate
**SC-G** — Atomic discipline violation candidate
**SC-H** — Cross-document amendment scope grows beyond sub-cascade boundary

## §9 — Closure protocol

Phase N closure verification, branch merge protocol, lessons evidence recording.

## §10 — Reference data

Inherits from A'.8 K-closure state. Parallel к bus refactor cascade. Communication norms preserved.

---

## §11 — Execution outcome (2026-05-22, Claude Opus 4.7)

**Branch**: `claude/godot-removal-deliberation-Vfg2R`
**Phase 0 reads complete**. **All halt triggers cleared.** Q-N ratification by Phase 0 evidence:

**Decisive finding**: `DualFrontier.Presentation` (Godot project, SDK=`Godot.NET.Sdk/4.6.1`) **is NOT in `DualFrontier.sln`** — already excluded from main solution build. Removing it has zero impact on the build graph. Q-5/Q-7 collapse to **Option (a) single atomic removal** safely (contrary to brief's initial Option (c) leaning).

**V substrate sufficiency (Q-6)**: confirmed sufficient. `DualFrontier.Runtime` carries the full Vulkan stack (V0.A: Window + Instance + Device; V0.B: Surface + Swapchain + RenderPass + CommandPool + MemoryAllocator + ComputePipelineRegistry; V0.C.1: AssetManager + SpriteRenderer; V0.C.2: Camera2D). `DualFrontier.Presentation.Native` carries the stub backend (Phase 5+ integration deferred — not blocking Godot removal).

**К-L17/L18 (Q-2/Q-4/Q-8)**: Scenario A confirmed. `SimStateLayer.cs` uses `Action<ILayerRenderContext>` delegate pattern — Godot-independent. Only doc comments mention Godot; no implementation refactor required. К-L17.1 / К-L18.1 sub-invariants **not** codified.

**Application entry point (Q-3)**: `Runtime.cs` is the V substrate host. GameBootstrap remains the simulation graph factory (pure .NET, not Godot Node). No migration needed.

**Architectural reasons load-bearing (Q-10)**: #1 (composition over deep OOP hierarchy), #4 (pure .NET runtime + commercial story), #6 (Godot editor not used).

**Removed artifacts**:
- `src/DualFrontier.Presentation/` directory in full (17 C# files, 1 `.tscn`, 1 inner `.sln`, 1 inner `project.godot`, `.csproj` + `.csproj.old`, `addons/df_devkit/`, `assets/{kenney,cinzel}/`, `Scenes/`)
- Root `project.godot`
- Root `icon.svg.import`

**Doc + csproj edits**:
- `src/DualFrontier.Application/DualFrontier.Application.csproj`: removed `<InternalsVisibleTo Include="DualFrontier.Presentation" />`
- `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs`: broken `<see cref="DualFrontier.Presentation.Nodes.GameRoot"/>` replaced with prose (CS1574-safe)
- `src/DualFrontier.Application/Display/SimStateLayer.cs`: removed Godot-canvas doc comment mention
- `src/DualFrontier.Application/Loop/GameContext.cs`: removed `DualFrontier.Presentation` doc mention
- `src/DualFrontier.Application/Attributes/DevKitOnlyAttribute.cs`: rephrased «Godot DevKit» reference
- `src/DualFrontier.Application/Scene/ISceneLoader.cs`: removed `GD.Load (res://)` reference from doc

**Build impact**: zero (Presentation project not in solution before this change). Test baseline preserved.

**К-L14 evidence**: zero-hard-halt streak preserved (+1 verification). Architectural cleanliness demonstrated — Godot Mono runtime no longer present, pure .NET 8+ stack remains.

---

**End of Godot removal brief.**
