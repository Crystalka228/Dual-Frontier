---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF
---
# К-extensions cascade #3 — Launcher Visual Implementation (Minimum Scope)

**Brief designation**: `K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF`  
**Cascade designation**: К-extensions cascade #3 (post-cascade-#2 closure)  
**Authoring date**: 2026-05-23  
**Authoring session**: Deliberation Session 2026-05-23 (Crystalka + Claude Opus 4.7)  
**Status**: AUTHORED — pending Crystalka RATIFICATION + Claude Code execution  
**Execution branch**: New feature branch off cascade-#2 closure commit (post-cascade-#2 push к origin/main)

**Brief shape**: Path B Hybrid (Phase 0 fully-locked + execution intent architecturally specified + mid-cascade checkpoint для Q-H detail when Phase 0 evidence requires)

---

## §0 — Timeline + provenance

### 0.1 Cascade #3 derivation от cascade #2 closure

К-extensions cascade #3 was **explicitly anticipated** during cascade #2 deliberation session 2026-05-23 per Q-G-6 LOCKED splitting:

> «(c) Maximal — Program + LauncherRenderer + RenderCommandDispatcher + scene state. Тут мы сделаем тогда отдельную сесcию после этой.»

Cascade #2 shipped infrastructure-only Launcher с Defensive Reserved Stub Pattern (Lesson #N12) — все 6 dispatch handler methods throw `NotImplementedException`. Cascade #3 scope = **replace defensive throws с real visual implementations** для pawn-3 subset (PawnSpawned/Moved/Died), preserving defensive throws для post-Vanilla-mods subset (PawnState/ItemSpawned/TickAdvanced) с updated messages.

### 0.2 Minimum-scope discipline ratification

Per Crystalka direction 2026-05-23 (post-cascade-#2 closure deliberation):

> «пока что минимальный так как нету ещё функционала, которые появятся после ванила модов, а это только после архитектуры»

This principled sequencing — Vanilla mod content materializes after architecture stabilizes — drove three key deliberation outcomes:
- Pawn-3 dispatch arms only (3 deferred к post-Vanilla-mods era)
- Scene state minimum (sprite registry only, no camera/HUD/layer machinery)
- Substrate extension deferred к когда consumer genuinely requires it

### 0.3 Empirical grounding via deliberation-agent Phase 0 reads

During cascade #3 deliberation, deliberation agent (Claude Opus 4.7) executed empirical reads via Filesystem MCP access к `D:\Colony_Simulator\Colony_Simulator\` — surfacing concrete substrate state, eliminating speculation tax.

**Critical empirical findings** (full audit trail в §2.0 Empirical Findings section below):
- PNG decoder supports **RGB8 + RGBA8 only** (color types 2 + 6); rejects palette (color type 3) per S-LOCK-2
- Kenney_micro-roguelike PNG assets all **4-bit/8-bit indexed palette** (color type 3) — substrate decoder rejects them
- `pawn.png` is **4-bit indexed palette** — also rejected
- `ProceduralAtlas` (`tests/DualFrontier.Runtime.SmokeTest/ProceduralAtlas.cs`) production-tested:
  - 256×256 RGBA8 atlas, 16×16 tile grid = 256 distinct tile types
  - 4 pattern types (solid/checker/gradient/ring)
  - 10K sprite stress test passes 60+ FPS
  - 200×200 TileMap (40K tiles) multi-cycle rendering passes 60+ FPS
- `SpriteRenderer` V0.C.2 batched API empirical signatures known: `BeginFrame(uint frameIndex)` / `Submit(Sprite)` / `EndFrame(VulkanCommandBuffer, Camera2D)`
- `Runtime.RecordSpriteFrame` / `RecordSpritesFrame` / `BeginRenderPassForSprites` / `EndSpriteRenderPass` methods exist on Runtime (per SmokeTest Program.cs usage)

### 0.4 Path γ-A ratification — Procedural-only cascade #3

Crystalka ratified **Path γ-A** (procedural-only cascade #3, substrate decoder palette extension deferred к future cascade) per minimum-scope discipline. Reasoning:
- Functional bar (b2) — pawns appear as sprites, move when commanded, despawn on death — satisfied by ProceduralAtlas tile-per-PawnId pattern
- Visual fidelity is **not** part of (b2) requirements
- К-L14 thesis preservation — substrate untouched через consumer materialization = first **clean additive evidence**
- Speculation discipline (Lesson #25 refined) — palette decoder waits для actual consumer (Vanilla mods era)
- Production-tested infrastructure (ProceduralAtlas) reused vs new substrate work

### 0.5 Cascade #2 verification gap inheritance — R-1 + R-2 as Phase 0 entry gates

Cascade #2 closure ratified Option 2 pragmatic CLEAN-WITH-ANNOTATION status, deferring R-1 (test variance falsifiability) + R-2 (Launcher composition smoke) к cascade #3 Phase 0. Per Crystalka direction:

> «я предлагаю в начало как раз и добавить (R-1 + R-2 verification gates)»

Brief 2 Phase 0 **mandatory entry gates** include R-1 + R-2 execution. Outcomes feed both cascade #2 retroactive ratification (CLEAN-WITH-ANNOTATION → CLEAN) AND cascade #3 baseline establishment.

### 0.6 Brief 2 shape ratification — Path B Hybrid

Per cascade #2 evidence (4 mid-cascade amendments arose despite fully-locked brief), cascade #3 enters **doubly new territory** (visual implementation + Vulkan recording integration + scene state primitive). Path B Hybrid shape ratified per honest match between brief shape и territory uncertainty:
- Phase 0 fully-locked (deterministic mandatory reads + verification gates)
- Execution intent **architecturally** specified (SpriteCatalog shape, scene state structure, dispatch arm patterns) — но **code-block detail held minimal** где Phase 0 evidence may inform
- Mid-cascade checkpoint allowed if Phase 0 surfaces signal-level surprises
- Atomic commit cascade structurally specified (phase + commit boundaries known); intra-commit code detail flexes by Phase 0 evidence

### 0.7 Deliberation session Q-H timeline

16 Q-H deliberated в session 2026-05-23 (К-extensions cascade #3 deliberation), 10 final-LOCKED после empirical correction:

| Q | Decision | Status |
|---|---|---|
| Q-H-0 | Path B Hybrid | LOCKED |
| Q-H-1 | Pawn-3 active, 3 deferred | LOCKED |
| Q-H-2 | Scene state = sprite registry only | LOCKED |
| Q-H-3 | Asset = Path γ-A (procedural-only) | LOCKED |
| Q-H-4 | Runtime SpriteRenderer integration via empirical signatures | LOCKED (signatures known) |
| Q-H-5 | R-1 fail-annotate + R-2 fail-halt | LOCKED |
| Q-H-6 | Test discipline = pawn-3 only | LOCKED |
| Q-H-7 | К-L14 #12 = clean additive evidence | LOCKED (restored framing) |
| Q-H-8 | К-L17.1 deferred | LOCKED |
| Q-H-9 | Filename = K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md | LOCKED |
| Q-H-10 | METHODOLOGY v1.11 → v1.12 + #N13/#N14 Provisional | LOCKED |
| Q-H-11 | DISSOLVED (PNG decoder discovery — empirical state known) | DISSOLVED |
| Q-H-12 | DISSOLVED (sprite asset format — Path γ-A handles) | DISSOLVED |
| Q-H-13 | DISSOLVED (substrate extension authorization — not needed) | DISSOLVED |
| Q-H-14 | DISSOLVED (palette decoder fallback — not in scope) | DISSOLVED |
| Q-H-15 | DISSOLVED (substrate extension scope bounding) | DISSOLVED |
| Q-H-16 | DISSOLVED (substrate test discipline) | DISSOLVED |

---

## §1 — S-LOCK reservation surface

S-LOCKs ratified post-Q-H deliberation. Brief execution **MUST NOT violate** any S-LOCK without halt + Crystalka re-ratification.

### S-LOCK-1 — Cascade #3 scope = pawn-3 dispatch arms only

**Statement**: Cascade #3 implements real visual dispatching для **exactly 3 dispatch arms**:
- `HandlePawnSpawned(PawnSpawnedCommand cmd)`
- `HandlePawnMoved(PawnMovedCommand cmd)`
- `HandlePawnDied(PawnDiedCommand cmd)`

The other 3 dispatch arms **stay defensive throws** с messages updated к reference post-Vanilla-mods era:
- `HandlePawnState(PawnStateCommand cmd)` — HUD pawn detail panel (needs Vanilla mods для pawn details)
- `HandleItemSpawned(ItemSpawnedCommand cmd)` — item visuals (needs Vanilla mods для item registry)
- `HandleTickAdvanced(TickAdvancedCommand cmd)` — HUD tick label (needs HUD primitives)

**Rationale**: Q-H-1 LOCKED. Minimum-scope discipline per Crystalka direction «пока что минимальный так как нету ещё функционала, которые появятся после ванила модов».

**Halt condition**: If execution agent implements any of 3 deferred arms, halt + Crystalka ratification required.

### S-LOCK-2 — Asset strategy = Path γ-A procedural-only, no substrate work

**Statement**: Cascade #3 uses **ProceduralAtlas** infrastructure exclusively для pawn visual representation. NO modifications к `DualFrontier.Runtime.Assets.PngDecoder` (palette support deferred). NO new asset files. NO kenney_micro-roguelike asset usage in cascade #3.

**Pawn sprite mapping pattern**:
- Each pawn assigned deterministic `tileIndex` based on `EntityId` (e.g. `tileIndex = pawnId.GetHashCode().Abs() % ProceduralAtlas.TotalTiles`)
- `AtlasRegion` extracted via `ProceduralAtlas.GetTileRegion(tileIndex)` 
- Visual result: each pawn = colored 16×16 tile from procedural atlas, visually distinct per pawn

**Rationale**: Q-H-3 + Q-H-13 (dissolved) LOCKED. Substrate untouched preserves К-L14 thesis cleanly. ProceduralAtlas production-tested (10K stress + 200×200 TileMap pass). Real kenney asset usage deferred к когда Vanilla mods materialize consumer need.

**Halt condition**: If execution agent attempts substrate PngDecoder modification OR adds new asset files, halt + Crystalka ratification required.

### S-LOCK-3 — Scene state = sprite registry only, minimum primitives

**Statement**: `SceneState` class в Launcher holds:
- `Dictionary<EntityId, PawnSpriteEntry>` — pawn sprite registry
- Methods: `RegisterPawn`, `MovePawn`, `DespawnPawn`, `EnumerateActiveSprites`

NO camera state (uses Runtime's default `Camera2D`). NO HUD state. NO layer ordering. NO sprite z-order. NO animation state. NO interpolation. NO selection state.

**`PawnSpriteEntry` structure**:
```csharp
public sealed record PawnSpriteEntry(
    EntityId PawnId,
    AtlasRegion Region,
    Vector2 Position,
    Vector2 Scale);
```

**Rationale**: Q-H-2 LOCKED. Minimum-scope discipline. Camera/HUD/layer machinery deferred к когда consumer requires.

**Halt condition**: If execution agent adds camera manipulation, HUD state, layer ordering, OR animation primitives, halt + Crystalka ratification required.

### S-LOCK-4 — Silent stubs для deferred arms (AMENDED 2026-05-23 mid-cascade)

**Status**: AMENDED 2026-05-23 mid-cascade per Crystalka ratification (К-extensions cascade #3 α0 — Phase 0 §2.5 + §2.8 empirical findings surfaced production-fires conflict). Original text preserved at end of section for audit trail.

**Amended statement** (post-Crystalka 2026-05-23 mid-cascade ratification): 3 deferred dispatch arms get **silent stub bodies** (empty method body — accept the command, do nothing visible) с honest documentation что (a) stub exists pending post-Vanilla-mods materialization, (b) production composition fires these commands so defensive throws would crash Launcher, (c) Q-H-6 test discipline preserves: DO NOT TEST stub paths (tests would lie by passing trivially — there is no observable behavior к assert).

```csharp
private void HandlePawnState(PawnStateCommand cmd)
{
    // CASCADE #3 STUB — pending post-Vanilla-mods cascade.
    // HUD pawn detail panel (name, needs, mood, job label, top skills) requires
    // Vanilla mods к define pawn structure first. Silent accept в production
    // composition (PawnStateReporterSystem emits these periodically; defensive
    // throw would crash Launcher on first tick). DO NOT TEST — stub has no
    // observable behavior; tests would lie by passing trivially (Q-H-6 discipline).
}

private void HandleItemSpawned(ItemSpawnedCommand cmd)
{
    // CASCADE #3 STUB — pending post-Vanilla-mods cascade.
    // Item visuals require Vanilla mods к define item registry first. Silent
    // accept в production composition (GameBootstrap emits ~255 ItemSpawnedCommand
    // at startup для initial food/water/bed/decoration; defensive throw would
    // crash Launcher on first frame). DO NOT TEST.
}

private void HandleTickAdvanced(TickAdvancedCommand cmd)
{
    // CASCADE #3 STUB — pending post-architecture cascade.
    // HUD tick label requires HUD primitives which не yet materialized. Silent
    // accept в production composition (GameLoop emits this every 33ms at 30 TPS;
    // defensive throw would crash Launcher within milliseconds). DO NOT TEST.
}
```

**Amended rationale**: Q-H-1 LOCKED (pawn-3 active, 3 deferred) preserved. Lesson #N12 application semantic refined через amendment: defensive throws appropriate ONLY when command type cannot fire в production composition flow; otherwise silent stub pattern с honest "DO NOT TEST" documentation. Cascade #2 defensive throw application was valid because cascade #2 не executed Launcher main loop (R-2 verification deferred к cascade #3); cascade #3 attempted same pattern but Phase 0 §2.5 + §2.8 empirical reads surfaced что GameBootstrap.PublishItemSpawnedEvents queues ~255 ItemSpawnedCommand at composition + GameLoop.RunLoop emits TickAdvancedCommand every 33ms + PawnStateReporterSystem emits PawnStateCommand periodically — all 3 deferred command types fire actively в production flow.

**Amended halt condition**: If execution agent uses defensive throws for any of the 3 deferred arms (the original S-LOCK-4 specification, now superseded), halt — defensive throws would crash Launcher на first frame. If execution agent adds real visual implementation для any of 3 deferred arms (real impl, не stub body), halt per S-LOCK-1. If execution agent writes any test exercising the 3 deferred arm paths, halt per Q-H-6 + Lesson #25 refined (lying-test surface — there is no observable behavior к verify in stubs).

**Lesson #N12 promotion criterion amended**: requires differentiating "production-fires" (silent stub pattern) vs "test-only-fires" (defensive throw pattern) sub-applications. Cascade #3 application = first observation of "production-fires" sub-pattern; substantially-different third application для promotion к FORMALIZE requires either (a) different sub-pattern (defensive throw in a new domain where test-only-fires holds), or (b) different domain entirely.

**R-2 verification gate resolution**: R-2 на cascade #2 state confirmed analytically to FAIL (defensive throws would crash Launcher on first frame). Per Crystalka mid-cascade ratification: skip empirical R-2 execution на cascade #2 state since outcome known + fix path ratified; R-2 equivalent verification deferred к Phase γ smoke на cascade #3 post-implementation state (silent stubs + pawn-3 real impl). Cascade #2 К-L14 verification #11 retroactive ratification deferred к Phase γ outcome: if Phase γ smoke PASSES, cascade #2 К-L14 #11 stays CLEAN (defensive throw design was non-falsifying since cascade #2 didn't expose it); if Phase γ smoke FAILS for other reasons, separate amendment.

---

**Original statement** (cascade #3 deliberation 2026-05-23, superseded mid-cascade — preserved для audit trail):

> **Statement**: 3 deferred dispatch arms get updated `NotImplementedException` messages reflecting post-Vanilla-mods deferral:
>
> ```csharp
> private void HandlePawnState(PawnStateCommand cmd) =>
>     throw new NotImplementedException(
>         "PawnState dispatch pending post-Vanilla-mods cascade. " +
>         "HUD pawn detail panel (name, needs, mood, job label, top skills) requires " +
>         "Vanilla mods к define pawn structure first. Lesson #N12 Defensive Reserved " +
>         "Stub Pattern second application.");
>
> private void HandleItemSpawned(ItemSpawnedCommand cmd) =>
>     throw new NotImplementedException(
>         "ItemSpawned dispatch pending post-Vanilla-mods cascade. " +
>         "Item visuals require Vanilla mods к define item registry first. " +
>         "Lesson #N12 second application.");
>
> private void HandleTickAdvanced(TickAdvancedCommand cmd) =>
>     throw new NotImplementedException(
>         "TickAdvanced dispatch pending post-architecture cascade. " +
>         "HUD tick label requires HUD primitives которые not yet materialized. " +
>         "Lesson #N12 second application.");
> ```
>
> **Rationale**: Q-H-1 LOCKED + Lesson #N12 second application. Defensive throws preserved + updated к reflect honest deferral reason (Vanilla mods architecture prerequisite, не arbitrary timing).
>
> **Halt condition**: If execution agent leaves cascade-#2-era throw messages unchanged OR removes defensive throws, halt + brief amendment required.

### S-LOCK-5 — R-1 + R-2 verification gates = Phase 0 entry conditions

**Statement**: Phase 0 includes mandatory verification execution before any commit cascade work:

**R-1 (test variance falsifiability)** — fail-annotate:
- Run `dotnet test` 3× на cascade-#2-closure baseline (current main pre-cascade-#3)
- Record per-run fail counts + fail test names
- Compare к cascade #2 closure annotation (5-9 fails variance, ~10 known flaky tests)
- If pattern matches: cascade #2 CLEAN-WITH-ANNOTATION upgraded к **CLEAN** retroactively
- If pattern differs: record finding, continue cascade #3 с annotated baseline; investigation deferred к future cascade
- **Does NOT halt** — investigation surface, не blocker

**R-2 (Launcher composition smoke)** — fail-halt:
- Run `dotnet run --project src/DualFrontier.Launcher/` against cascade-#2 closure state
- Verify: window opens, Vulkan initializes, GameLoop background thread starts, no early-frame exceptions before user-close
- Defensive throws may fire if domain produces commands — **expected behavior** validating Lesson #N12 design
- **PASS criterion**: Launcher constructs cleanly, window present, graceful close
- **FAIL = HALT**: cascade #2 не actually clean; cascade #3 cannot proceed; investigation + cascade #2 amendment required

**Rationale**: Q-H-5 LOCKED. Cascade #2 closure inherited verification gaps; cascade #3 absorbs them as productive Phase 0 work.

**Halt condition**: R-2 FAIL = cascade #3 halt + Crystalka ratification re: cascade #2 amendment.

### S-LOCK-6 — IRenderCommand marker preservation

**Statement**: `IRenderCommand` interface stays pure marker (Q-G-3 cascade #2 LOCKED). Cascade #3 does **NOT** restore Execute() methods on Commands. Bridge Commands stay pure data records.

**Rationale**: Cascade #2 architectural decision preserved. Dispatch happens centrally в Launcher's `RenderCommandDispatcher`. Adding Execute() back would re-introduce lying-test surface (Lesson #25 refined violation).

**Halt condition**: If execution agent adds Execute() method к any Bridge Command record, halt + brief amendment required.

### S-LOCK-7 — К-L14 verification #12 framing = clean additive evidence

**Statement**: К-L14 verification #12 framed as **first clean additive evidence**:
- Substrate (Runtime + PngDecoder + SpriteRenderer + ProceduralAtlas) **unchanged** through cascade #3
- New consumer functionality added в Launcher only
- Existing substrate tests preserved unchanged
- К-L14 thesis: «substrate primitives stable across consumer churn» — empirically verified by adding new dispatch arm implementations without substrate API touches

**Pass criteria** (7-point matrix):
1. Build integrity (every atomic commit)
2. Test count parity (R-1 baseline preserved или improved)
3. Pre-existing pollution NOT worse
4. Runtime substrate API surface unchanged (`dotnet build` verifies no break)
5. SmokeTest still passes (substrate isolation preserved)
6. Launcher composition smoke: pawns visible, move on dispatch, despawn on dispatch
7. METHODOLOGY §12.7 Modding-suite verification gate

**Rationale**: Q-H-7 LOCKED (restored framing post-empirical correction). Path γ-A ensures substrate untouched.

**Halt condition**: ANY 7-point criterion fail → halt + Crystalka ratification re: continuation OR substrate amendment.

### S-LOCK-8 — RenderCommandDispatcher signature stability

**Statement**: `RenderCommandDispatcher.Dispatch(IRenderCommand)` public method signature **stays unchanged** от cascade #2. Internal handler method signatures unchanged. ONLY method **bodies** of pawn-3 arms change от defensive throws к real implementations.

**Rationale**: Q-G-3 cascade #2 LOCKED RenderCommandDispatcher contract. Cascade #3 implements behavior, не contract. Preserves К-L14 substrate-adjacent consumer stability.

**Halt condition**: If execution agent modifies `Dispatch` signature OR handler method signatures (parameter additions, return type changes), halt + brief amendment required.

### S-LOCK-9 — LauncherRenderer.RenderFrame integration

**Statement**: `LauncherRenderer.RenderFrame(double deltaSeconds)` body extended к integrate Vulkan sprite recording per Runtime API empirical signatures (per §2.0 findings):

```csharp
public void RenderFrame(double deltaSeconds)
{
    ObjectDisposedException.ThrowIf(_disposed, this);

    // 1. Drain Bridge commands → dispatcher (updates SceneState)
    _bridge.DrainCommands(_dispatcher.Dispatch);

    // 2. Acquire swapchain image + record frame
    uint imageIndex = _runtime.Swapchain.AcquireNextImage(...);
    // ... per-frame setup
    
    // 3. Begin render pass + sprite batch
    _runtime.BeginRenderPassForSprites(commandBuffer, (int)imageIndex, clearColor: ...);
    _runtime.SpriteRenderer.BeginFrame(imageIndex);
    
    foreach (var entry in _sceneState.EnumerateActiveSprites())
    {
        var sprite = new Sprite(
            Texture: _atlasTexture,
            Region: entry.Region,
            Transform: new SpriteTransform(entry.Position, entry.Scale, 0f, WhiteTint));
        _runtime.SpriteRenderer.Submit(sprite);
    }
    
    _runtime.SpriteRenderer.EndFrame(commandBuffer, _runtime.Camera);
    _runtime.EndSpriteRenderPass(commandBuffer);
    
    // 4. Submit + present
    // ... swapchain present
}
```

Exact code structure subject к Phase 0 verification of Runtime API method signatures (`AcquireNextImage`, `BeginRenderPassForSprites`, `RecordSpritesFrame`, etc).

**Rationale**: Q-H-4 LOCKED. SmokeTest Program.cs provides reference implementation pattern; LauncherRenderer adopts same pattern для Bridge-driven sprite list.

**Halt condition**: If Runtime API signatures differ от brief expectations (per §2.0 empirical findings), mid-cascade checkpoint triggered per Path B Hybrid discipline.

### S-LOCK-10 — Scene state composition root

**Statement**: `SceneState` instance constructed в `Program.Main()` and passed к both `RenderCommandDispatcher` (writes via handlers) and `LauncherRenderer` (reads via `EnumerateActiveSprites`). NO singletons. NO static state. NO global accessors.

Composition order:
```csharp
var sceneState = new SceneState();
var dispatcher = new RenderCommandDispatcher(runtime, sceneState, atlasTexture);
using var renderer = new LauncherRenderer(runtime, bridge, dispatcher, sceneState, atlasTexture);
```

**Rationale**: Constructor injection per established codebase convention. Composition root pattern preserved. Testability + future modding scope flexibility.

**Halt condition**: If execution agent uses singleton/static pattern for SceneState, halt + refactor required.

---

## §2 — Phase 0 mandatory reads

Per Lesson #18 strengthened + Lesson #N14 candidate (Phase 0 reads must cover assumed-empty/assumed-state directories empirically): execution agent MUST `view` ALL files в this list before commit cascade begins. Skipping = brief violation.

### §2.0 — Empirical findings (deliberation-agent pre-authored — verify integrity)

**Note**: Following findings established by deliberation agent (Claude Opus 4.7) via Filesystem MCP reads 2026-05-23 during cascade #3 deliberation. Execution agent **MUST verify integrity** by re-reading источники + confirming findings unchanged at execution time. Если findings diverge от current state, mid-cascade checkpoint triggered.

**PNG decoder capability** (from `src/DualFrontier.Runtime/Assets/PngDecoder.cs` + `MODULE.md`):
- Supports: color type 2 (RGB8) + color type 6 (RGBA8); bit depth 8 only; non-interlaced; standard compression/filter
- Rejects: palette (color type 3), grayscale (0/4), 16-bit, interlaced — explicit `PngDecoderException` per Lesson #20
- RGB8 → RGBA8 conversion с alpha=255 per Q2 (a) ratification (V0.C.1)

**Kenney + pawn PNG asset formats** (verified via header parse 2026-05-23):
- `assets/sprites/pawn.png`: 16×16, 4-bit indexed palette → substrate REJECTS
- `assets/kenney_micro-roguelike/Tilemap/colored_tilemap.png`: 143×89, 8-bit indexed palette → REJECTS
- `assets/kenney_micro-roguelike/Tilemap/colored_tilemap_packed.png`: 128×80, 8-bit indexed palette → REJECTS
- (Other kenney_* directories not inspected exhaustively; assume palette pending Phase 0 expansion if needed)

**ProceduralAtlas capability** (from `tests/DualFrontier.Runtime.SmokeTest/ProceduralAtlas.cs`):
- 256×256 RGBA8, 16×16 tile grid = 256 distinct tiles, 4 pattern types (solid/checker/gradient/ring)
- Static API: `GenerateAtlas()` returns `PngImage`; `GetTileRegion(int tileIndex)` returns `AtlasRegion`
- Per-tile deterministic coloring: `baseR = (tileIdx * 37) % 256` etc.
- Production-tested: 10K sprite stress (60+ FPS) + 200×200 TileMap (40K tiles, 60+ FPS)

**SpriteRenderer V0.C.2 API** (from `src/DualFrontier.Runtime/Sprite/SpriteRenderer.cs`):
- `BeginFrame(uint frameIndex)` — frameIndex typically = swapchain image index
- `Submit(Sprite sprite)` — sprite grouped by `SpriteTexture` key
- `EndFrame(VulkanCommandBuffer commandBuffer, Camera2D camera)` — issues vkCmdDrawIndexed per atlas group; MVP = camera.ViewProjectionMatrix
- Throws `InvalidOperationException` if BeginFrame called twice без EndFrame
- Per-atlas descriptor set caching (64 capacity)
- Max sprites per frame: 10,000 (S-LOCK-3a uint16 hard cap)

**Runtime sprite recording helpers** (from SmokeTest Program.cs empirical usage):
- `runtime.BeginRenderPassForSprites(commandBuffer, imageIndex, clearColor)` — starts render pass
- `runtime.EndSpriteRenderPass(commandBuffer)` — ends render pass
- `runtime.RecordSpriteFrame(commandBuffer, imageIndex, sprite, mvp, clearColor)` — single-sprite convenience (V0.C.1)
- `runtime.RecordSpritesFrame(commandBuffer, imageIndex, sprites, camera, clearColor)` — sprite list convenience (V0.C.2)
- `runtime.SpriteRenderer` — direct access к batched renderer
- `runtime.Camera` — default `Camera2D` instance
- `runtime.MemoryAllocator` + `runtime.TextureUploader` — для VulkanImage creation
- `runtime.GraphicsCommandPool.AllocateBuffer()` — command buffer allocation
- `runtime.Swapchain.AcquireNextImage(imageAvailable.Handle, IntPtr.Zero, out bool outOfDate)` — frame acquire

**VulkanImage / SpriteTexture creation pattern** (from SmokeTest):
```csharp
PngImage atlasImage = ProceduralAtlas.GenerateAtlas();
VulkanImage atlasVkImage = VulkanImage.CreateFromPngImage(
    runtime.VulkanDevice, runtime.MemoryAllocator, runtime.TextureUploader, atlasImage);
var atlasSampler = new VulkanSampler(runtime.VulkanDevice);
using var atlasTexture = new SpriteTexture(atlasVkImage, atlasSampler);
```

### §2.1 — State verification reads

- `D:\Colony_Simulator\Colony_Simulator\.git\HEAD` — confirm starting branch
- `D:\Colony_Simulator\Colony_Simulator\.git\refs\heads\main` — confirm main hash = cascade-#2-closure commit
- `D:\Colony_Simulator\Colony_Simulator\.git\logs\HEAD` (tail 25) — confirm cascade #2 closure commits present in history

### §2.2 — R-1 verification gate execution (fail-annotate)

Execute pre-cascade-#3 test baseline 3 runs:
```powershell
cd D:\Colony_Simulator\Colony_Simulator
dotnet build  # ensure clean baseline build
for ($i=1; $i -le 3; $i++) {
    dotnet test --no-build --logger "console;verbosity=normal" 2>&1 | Tee-Object -FilePath "tests-run-$i.log"
}
```

Record per-run:
- Total test count
- Fail count + fail test names

Classify:
- **Pattern matches cascade #2 closure annotation** (5-9 fails, ~10 known flaky test names) → R-1 PASS; cascade #2 retroactive ratification CLEAN-WITH-ANNOTATION → CLEAN
- **Pattern diverges** (different test names или substantially different count) → R-1 ANNOTATE; record finding, continue cascade #3 baseline annotated; investigation deferred
- **R-1 does NOT halt cascade #3** — fail-annotate semantics

### §2.3 — R-2 verification gate execution (fail-halt)

Execute Launcher composition smoke:
```powershell
cd D:\Colony_Simulator\Colony_Simulator
dotnet run --project src/DualFrontier.Launcher/ 2>&1 | Tee-Object -FilePath "launcher-smoke.log"
# Manual interaction: confirm window opens, close window after 5-10 seconds к verify graceful shutdown
```

**PASS criteria**:
- ✓ Window opens
- ✓ No early-frame exception before user closes window (defensive throws may fire if Domain produces commands — expected per Lesson #N12 design)
- ✓ Graceful close (no crash on window close)
- ✓ Process exit code 0

**FAIL conditions** (any of):
- ✗ Window does NOT open
- ✗ Vulkan validation errors observed
- ✗ Composition exception before window present (Main() throws before Show())
- ✗ Crash on window close (resource leak/double-dispose pattern)
- ✗ Process exit code ≠ 0

**R-2 FAIL = HALT CASCADE #3**. Crystalka ratification required re: cascade #2 amendment OR investigation path.

### §2.4 — Production wiring reads (cascade #2 closure state)

- `src\DualFrontier.Launcher\Program.cs` (FULL READ) — cascade #2 composition pattern
- `src\DualFrontier.Launcher\LauncherRenderer.cs` (FULL READ) — cascade #2 renderer scaffold
- `src\DualFrontier.Launcher\RenderCommandDispatcher.cs` (FULL READ) — cascade #2 defensive stub dispatcher (6 throws)
- `src\DualFrontier.Launcher\DualFrontier.Launcher.csproj` — project config
- `src\DualFrontier.Launcher\README.md` — cascade #2 project documentation

### §2.5 — Bridge layer reads (cascade #2 closure state)

- `src\DualFrontier.Application\Bridge\PresentationBridge.cs` — Drain pattern
- `src\DualFrontier.Application\Bridge\IRenderCommand.cs` — marker interface (Q-G-3 cascade #2)
- `src\DualFrontier.Application\Bridge\Commands\PawnSpawnedCommand.cs` — pawn-3 active
- `src\DualFrontier.Application\Bridge\Commands\PawnMovedCommand.cs` — pawn-3 active
- `src\DualFrontier.Application\Bridge\Commands\PawnDiedCommand.cs` — pawn-3 active
- `src\DualFrontier.Application\Bridge\Commands\PawnStateCommand.cs` — deferred
- `src\DualFrontier.Application\Bridge\Commands\ItemSpawnedCommand.cs` — deferred
- `src\DualFrontier.Application\Bridge\Commands\TickAdvancedCommand.cs` — deferred

Verify all 6 Commands are pure records (no Execute() method per Q-G-3 cascade #2 LOCKED).

### §2.6 — Substrate API surface reads (verify §2.0 empirical findings unchanged)

- `src\DualFrontier.Runtime\Runtime.cs` (FULL READ) — `RecordSpriteFrame`, `RecordSpritesFrame`, `BeginRenderPassForSprites`, `EndSpriteRenderPass`, `SpriteRenderer`, `Camera`, `MemoryAllocator`, `TextureUploader`, `GraphicsCommandPool` properties confirmed accessible
- `src\DualFrontier.Runtime\Sprite\SpriteRenderer.cs` — confirm BeginFrame/Submit/EndFrame signatures match §2.0
- `src\DualFrontier.Runtime\Sprite\Sprite.cs` — confirm record structure (Texture/Region/Transform)
- `src\DualFrontier.Runtime\Sprite\SpriteTransform.cs` — confirm constructor signature (Position/Scale/Rotation/TintRgba)
- `src\DualFrontier.Runtime\Sprite\AtlasRegion.cs` — confirm `FromPixels` factory accessible
- `src\DualFrontier.Runtime\Sprite\SpriteTexture.cs` — confirm constructor (VulkanImage, VulkanSampler)
- `src\DualFrontier.Runtime\Sprite\Camera2D.cs` — confirm ViewportSize/Position/Zoom properties
- `src\DualFrontier.Runtime\Graphics\VulkanImage.cs` — confirm `CreateFromPngImage` static factory

### §2.7 — ProceduralAtlas reuse pattern reads

- `tests\DualFrontier.Runtime.SmokeTest\ProceduralAtlas.cs` (FULL READ) — pattern reference

**Reuse strategy**: ProceduralAtlas is currently в SmokeTest namespace. Three options для cascade #3:
- **Option A**: Reference SmokeTest project from Launcher (anti-pattern — production references test code)
- **Option B**: Move ProceduralAtlas к Runtime.Sprite namespace (substrate touch — violates S-LOCK-2)
- **Option C**: Copy ProceduralAtlas к Launcher (DualFrontier.Launcher.LauncherProceduralAtlas — production-side copy)

**Q-H-17 — ProceduralAtlas reuse strategy** — **CHECKPOINT-DEFERRED**: Phase 0 read confirms ProceduralAtlas surface; execution agent reports к Crystalka mid-cascade for ratification:
- **Recommendation**: Option C (copy) — preserves S-LOCK-2 substrate isolation; minor code duplication acceptable (ProceduralAtlas is ~80 lines deterministic)
- **Alternative ratification**: Option B if Crystalka decides ProceduralAtlas belongs в substrate (Runtime extension); would change S-LOCK-2 from «no substrate work» к «no decoder work» — palette deferral still preserved

### §2.8 — Composition wiring reads

- `src\DualFrontier.Application\Loop\GameBootstrap.cs` — CreateLoop signature (cascade #2 confirmed Start/Stop pattern, GameLoop self-ticks)
- `src\DualFrontier.Application\Loop\GameLoop.cs` — Tick semantics, threading model
- `src\DualFrontier.Application\Loop\GameContext.cs` — context structure
- `src\DualFrontier.Application\DualFrontier.Application.csproj` — InternalsVisibleTo("DualFrontier.Launcher") confirmed (cascade #2 α1 amendment)

### §2.9 — Governance reads (cascade #2 closure state)

- `docs\governance\REGISTER.yaml` — confirm register_version 2.4 (cascade #2 closure) + DOC-F-SRC-LAUNCHER enrolled
- `docs\architecture\KERNEL_ARCHITECTURE.md` — confirm v2.5.1 (cascade #2 closure)
- `docs\methodology\METHODOLOGY.md` — confirm v1.11 (cascade #2 closure)
- `docs\architecture\K_EXTENSIONS_LEDGER.md` — confirm §3.3 cascade #2 entry present
- `docs\architecture\PHASE_A_PRIME_SEQUENCING.md` — confirm cascade #2 chronological entry
- `docs\architecture\K_L14_EVIDENCE_DASHBOARD.md` — confirm verification #11 entry present (cascade #2 first removal-type evidence)

---

## §3 — Atomic commit cascade specification

Per Lesson #8 strengthened (atomic compilable commits) + Lesson #N13 candidate (commit integrity verification before commit) + Lesson #N14 candidate (Phase 0 reads must cover assumed-state empirically): every commit MUST `dotnet build` exit 0 + commit message claims verified against `git status` BEFORE commit.

Cascade structure: **4 phases (α-δ) + ε verification, 12-15 atomic commits**.

### Phase α — Verification + ProceduralAtlas integration (2-3 commits)

#### Commit α0 — Phase 0 verification execution + brief AUTHORED marker

**Scope**:
- Execute Phase 0 reads per §2.1–2.9
- Execute R-1 verification gate (3× test runs, record results)
- Execute R-2 verification gate (Launcher composition smoke)
- Document findings в execution log

**NO code changes в this commit** — verification + brief enrollment only. Brief copied к `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md`.

**Verification gate**:
- R-1 fail-annotate complete (any outcome acceptable)
- R-2 PASS (если FAIL, halt + cascade #2 amendment)
- All Phase 0 reads complete без halt-condition triggers

**Commit message template**:
```
chore(brief): K_EXT_3 α0 — Brief enrollment + Phase 0 verification

Per К-extensions cascade #3 Phase 0 mandatory reads + R-1/R-2 verification
gates execution. Brief authored 2026-05-23 (deliberation session); enrolled
к tools/briefs/ for execution.

R-1 outcome (test variance falsifiability):
- Run 1: [N fails — list]
- Run 2: [N fails — list]
- Run 3: [N fails — list]
- Pattern match cascade #2 closure annotation: [YES/NO]
- Status: [CLEAN retroactive ratification / ANNOTATE finding]

R-2 outcome (Launcher composition smoke):
- Window opened: [YES/NO]
- Vulkan validation: [clean/N errors]
- Graceful close: [YES/NO]
- Process exit code: [0/N]
- Status: [PASS / FAIL — if FAIL, halt before this commit]

Phase 0 §2.4–2.9 reads complete; substrate empirical findings confirmed:
- PngDecoder rejects palette (verified)
- ProceduralAtlas API confirmed (Option C reuse strategy recommended)
- SpriteRenderer V0.C.2 signatures match §2.0
- Runtime sprite recording helpers confirmed

Cascade #3 baseline established. No code changes этой commit.
```

#### Commit α1 — LauncherProceduralAtlas integration (Option C copy, if ratified)

**Scope**:
- Per Q-H-17 checkpoint: report ProceduralAtlas reuse decision к Crystalka
- If **Option C ratified** (recommended): copy `tests/DualFrontier.Runtime.SmokeTest/ProceduralAtlas.cs` к `src/DualFrontier.Launcher/LauncherProceduralAtlas.cs`:
  - Namespace: `DualFrontier.Launcher`
  - Internal access modifier (production-side, not public API)
  - Same algorithm (deterministic per-tile coloring + 4 pattern types + GenerateAtlas + GetTileRegion)
  - Comment header noting cascade #3 origin + future consolidation candidate
- If **Option B ratified** (substrate extension): different commit pattern (Move к Runtime.Sprite); brief amendment per Path B Hybrid checkpoint

**File template** (Option C):
```csharp
// src/DualFrontier.Launcher/LauncherProceduralAtlas.cs
using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Launcher-side procedural atlas generator для cascade #3 pawn visual representation.
/// Copies pattern from SmokeTest's ProceduralAtlas per Q-H-17 Option C ratification
/// (production-side copy preserves S-LOCK-2 substrate isolation; minor code duplication
/// acceptable until palette decoder extension justified by Vanilla mods consumer need).
///
/// Forward consolidation candidate: when substrate palette decoder extension materializes
/// (future cascade), this + SmokeTest ProceduralAtlas + future kenney loader can be
/// reconciled into unified atlas/asset registry.
/// </summary>
internal static class LauncherProceduralAtlas
{
    public const int AtlasWidth = 256;
    public const int AtlasHeight = 256;
    public const int TileWidth = 16;
    public const int TileHeight = 16;
    public const int TilesPerRow = AtlasWidth / TileWidth;
    public const int TilesPerColumn = AtlasHeight / TileHeight;
    public const int TotalTiles = TilesPerRow * TilesPerColumn;

    public static PngImage GenerateAtlas() { /* same algorithm as SmokeTest ProceduralAtlas */ }
    
    private static void FillTile(byte[] pixels, int tileX, int tileY, int tileIdx) { /* same */ }
    
    public static AtlasRegion GetTileRegion(int tileIndex) { /* same */ }
}
```

**Verification gate**:
- `dotnet build` exit 0
- LauncherProceduralAtlas.GenerateAtlas() produces valid PngImage
- LauncherProceduralAtlas.GetTileRegion(0) returns valid AtlasRegion

**Commit message template**:
```
feat(launcher): K_EXT_3 α1 — LauncherProceduralAtlas integration (Q-H-17 Option C)

Per К-extensions cascade #3 Q-H-17 Crystalka ratification (Option C copy):
add Launcher-side ProceduralAtlas copy preserving S-LOCK-2 substrate isolation.

Added:
- src/DualFrontier.Launcher/LauncherProceduralAtlas.cs — 256×256 RGBA8 atlas,
  16×16 tile grid (256 tiles), 4 pattern types (solid/checker/gradient/ring)

Algorithm copied verbatim от SmokeTest's ProceduralAtlas. Forward consolidation
candidate captured в file header comment.

S-LOCK-2 satisfied (no substrate touch).
Build verified: dotnet build exit 0.
```

#### Commit α2 — SceneState + PawnSpriteEntry types

**Scope**:
- Create `src/DualFrontier.Launcher/PawnSpriteEntry.cs`:

```csharp
using System.Numerics;
using DualFrontier.Contracts.Core;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Per-pawn scene entry held by <see cref="SceneState"/>. Pure data record
/// carrying pawn identity + UV region + position + scale. К-extensions
/// cascade #3 minimum scope per Q-H-2 LOCKED: no rotation, no tint, no
/// animation state — visual minimum.
/// </summary>
public sealed record PawnSpriteEntry(
    EntityId PawnId,
    AtlasRegion Region,
    Vector2 Position,
    Vector2 Scale);
```

- Create `src/DualFrontier.Launcher/SceneState.cs`:

```csharp
using System.Numerics;
using DualFrontier.Contracts.Core;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Scene state container holding pawn sprite registry для cascade #3 visual
/// implementation. Minimum scope per Q-H-2 LOCKED: sprite registry only,
/// no camera state, no HUD state, no layer ordering.
///
/// Read/write split:
/// - <see cref="RenderCommandDispatcher"/> writes via Register/Move/Despawn
/// - <see cref="LauncherRenderer"/> reads via EnumerateActiveSprites
///
/// Composition: constructed в Program.Main(), passed к both dispatcher
/// и renderer per S-LOCK-10 constructor injection pattern.
/// </summary>
internal sealed class SceneState
{
    private readonly Dictionary<EntityId, PawnSpriteEntry> _pawnSprites = new();

    public int ActivePawnCount => _pawnSprites.Count;

    public void RegisterPawn(EntityId pawnId, AtlasRegion region, Vector2 position, Vector2 scale)
    {
        _pawnSprites[pawnId] = new PawnSpriteEntry(pawnId, region, position, scale);
    }

    public bool MovePawn(EntityId pawnId, Vector2 newPosition)
    {
        if (!_pawnSprites.TryGetValue(pawnId, out PawnSpriteEntry? entry))
        {
            return false;  // Pawn not registered — silent miss; dispatcher may log
        }
        _pawnSprites[pawnId] = entry with { Position = newPosition };
        return true;
    }

    public bool DespawnPawn(EntityId pawnId)
    {
        return _pawnSprites.Remove(pawnId);
    }

    public IEnumerable<PawnSpriteEntry> EnumerateActiveSprites()
    {
        return _pawnSprites.Values;
    }
}
```

**Phase 0 dependency**: Verify `EntityId` type accessible from `DualFrontier.Contracts.Core` namespace (per cascade #2 PawnSpawnedCommand имеет EntityId parameter).

**Verification gate**:
- `dotnet build` exit 0
- SceneState constructable + Register/Move/Despawn methods callable

**Commit message template**:
```
feat(launcher): K_EXT_3 α2 — SceneState + PawnSpriteEntry types (Q-H-2 + S-LOCK-3)

Per К-extensions cascade #3 Q-H-2 LOCKED + S-LOCK-3: minimum scope scene state
container holding pawn sprite registry only. No camera/HUD/layer machinery.

Added:
- src/DualFrontier.Launcher/PawnSpriteEntry.cs — per-pawn scene entry record
- src/DualFrontier.Launcher/SceneState.cs — pawn sprite registry с Register/Move/Despawn

Composition pattern (S-LOCK-10): constructed в Program.Main(), passed к both
dispatcher (write) и renderer (read) via constructor injection.

S-LOCK-3 satisfied (sprite registry only).
Build verified: dotnet build exit 0.
```

### Phase β — Dispatcher implementation + Renderer integration (3 commits, atomic compilable unit)

#### Commit β1 — RenderCommandDispatcher pawn-3 implementations + S-LOCK-4 deferred message updates

**Scope** (single atomic commit per S-LOCK-8 signature stability + Lesson #8 compilable):
- Modify `src/DualFrontier.Launcher/RenderCommandDispatcher.cs`:
  - Constructor extended к accept `SceneState` + `SpriteTexture` атlas reference
  - 3 pawn arm bodies replaced с real implementations (HandlePawnSpawned/Moved/Died)
  - 3 deferred arm messages updated per S-LOCK-4

**Modified RenderCommandDispatcher.cs**:

```csharp
using System;
using System.Numerics;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Contracts.Core;
using DualFrontier.Runtime;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Dispatches drained <see cref="IRenderCommand"/> instances к scene state mutations
/// (consumed by <see cref="LauncherRenderer"/> per frame). К-extensions cascade #3
/// (2026-05-DD): real implementations для pawn-3 arms (PawnSpawned/Moved/Died);
/// PawnState/ItemSpawned/TickAdvanced stay defensive throws per Lesson #N12 second
/// application (deferred к post-Vanilla-mods cascades).
/// </summary>
internal sealed class RenderCommandDispatcher
{
    private const float PawnSpriteScale = 16f;  // Match ProceduralAtlas tile dimensions

    private readonly Runtime.Runtime _runtime;
    private readonly SceneState _sceneState;
    private readonly SpriteTexture _atlasTexture;
    private readonly int _atlasTotalTiles;

    public RenderCommandDispatcher(Runtime.Runtime runtime, SceneState sceneState, SpriteTexture atlasTexture)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        _sceneState = sceneState ?? throw new ArgumentNullException(nameof(sceneState));
        _atlasTexture = atlasTexture ?? throw new ArgumentNullException(nameof(atlasTexture));
        _atlasTotalTiles = LauncherProceduralAtlas.TotalTiles;
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
                    "Add dispatch arm в RenderCommandDispatcher.Dispatch и accompanying handler.");
        }
    }

    private void HandlePawnSpawned(PawnSpawnedCommand cmd)
    {
        // Deterministic tile assignment per PawnId — visually distinct per pawn,
        // reproducible across runs.
        int tileIndex = Math.Abs(cmd.PawnId.GetHashCode()) % _atlasTotalTiles;
        AtlasRegion region = LauncherProceduralAtlas.GetTileRegion(tileIndex);

        _sceneState.RegisterPawn(
            pawnId: cmd.PawnId,
            region: region,
            position: new Vector2(cmd.X, cmd.Y),
            scale: new Vector2(PawnSpriteScale, PawnSpriteScale));
    }

    private void HandlePawnMoved(PawnMovedCommand cmd)
    {
        bool moved = _sceneState.MovePawn(cmd.PawnId, new Vector2(cmd.X, cmd.Y));
        // Silent miss if pawn not registered — domain may emit Moved before Spawned
        // в edge race conditions; не fatal.
    }

    private void HandlePawnDied(PawnDiedCommand cmd)
    {
        bool despawned = _sceneState.DespawnPawn(cmd.PawnId);
        // Silent miss if pawn not registered — same race tolerance as Moved.
    }

    private void HandlePawnState(PawnStateCommand cmd) =>
        throw new NotImplementedException(
            "PawnState dispatch pending post-Vanilla-mods cascade. " +
            "HUD pawn detail panel (name, needs, mood, job label, top skills) requires " +
            "Vanilla mods к define pawn structure first. Lesson #N12 Defensive Reserved " +
            "Stub Pattern second application.");

    private void HandleItemSpawned(ItemSpawnedCommand cmd) =>
        throw new NotImplementedException(
            "ItemSpawned dispatch pending post-Vanilla-mods cascade. " +
            "Item visuals require Vanilla mods к define item registry first. " +
            "Lesson #N12 second application.");

    private void HandleTickAdvanced(TickAdvancedCommand cmd) =>
        throw new NotImplementedException(
            "TickAdvanced dispatch pending post-architecture cascade. " +
            "HUD tick label requires HUD primitives которые not yet materialized. " +
            "Lesson #N12 second application.");
}
```

**Phase 0 dependency check**:
- `PawnSpawnedCommand` имеет `EntityId PawnId, float X, float Y` parameters (cascade #2 verified)
- `PawnMovedCommand` имеет `EntityId PawnId, float X, float Y` parameters (verify)
- `PawnDiedCommand` имеет `EntityId PawnId` parameter (verify)

Если parameter names differ, update dispatcher accordingly.

**Verification gate**:
- `dotnet build` exit 0 (LauncherRenderer constructor needs update в β2 для compilable atomic unit)
- All 6 dispatch arms compile

**Atomicity note**: This commit produces compilable code IF LauncherRenderer constructor change happens в same commit OR LauncherRenderer pre-emptively accepts SceneState/SpriteTexture parameters in cascade #2 (which it doesn't — cascade #2 LauncherRenderer signature was `(Runtime, PresentationBridge, RenderCommandDispatcher)`). 

**Therefore**: β1 + β2 + β3 must be **single atomic commit** to satisfy Lesson #8.

#### Commit β1+β2+β3 (squashed) — Dispatcher pawn-3 + LauncherRenderer integration + Program.cs composition update

**Scope** — Single atomic commit с 4 file modifications:

##### β1+β2+β3.1 — `RenderCommandDispatcher.cs` per β1 above

##### β1+β2+β3.2 — `LauncherRenderer.cs` extended Vulkan recording

Replace cascade #2 LauncherRenderer.cs с integrated version:

```csharp
using System;
using System.Numerics;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Rendering;
using DualFrontier.Runtime;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Production <see cref="IRenderer"/> implementation. Wraps Vulkan substrate +
/// drains <see cref="PresentationBridge"/> per frame, dispatching commands к
/// <see cref="RenderCommandDispatcher"/> (updates SceneState), then records
/// Vulkan sprite frame from SceneState's active sprites.
///
/// К-extensions cascade #3 (2026-05-DD): real visual implementation для pawn-3
/// dispatch arms. SpriteRenderer V0.C.2 batched API used для per-frame recording.
/// Camera = Runtime's default Camera2D (no manipulation per Q-H-2 minimum scope).
/// </summary>
internal sealed class LauncherRenderer : IRenderer, IDisposable
{
    private readonly Runtime.Runtime _runtime;
    private readonly PresentationBridge _bridge;
    private readonly RenderCommandDispatcher _dispatcher;
    private readonly SceneState _sceneState;
    private readonly SpriteTexture _atlasTexture;
    
    // Per-frame sync primitives (created Initialize, disposed Shutdown)
    private VulkanCommandBuffer? _commandBuffer;
    private VulkanSemaphore? _imageAvailable;
    private VulkanSemaphore[]? _renderFinishedPerImage;
    private VulkanFence? _frameFence;
    
    private bool _initialized;
    private bool _disposed;

    public LauncherRenderer(
        Runtime.Runtime runtime,
        PresentationBridge bridge,
        RenderCommandDispatcher dispatcher,
        SceneState sceneState,
        SpriteTexture atlasTexture)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _sceneState = sceneState ?? throw new ArgumentNullException(nameof(sceneState));
        _atlasTexture = atlasTexture ?? throw new ArgumentNullException(nameof(atlasTexture));
    }

    /// <summary>
    /// Initialize per-frame sync primitives. Atlas texture composition happens в
    /// Program.Main() (passed via constructor) — caller owns atlasTexture lifetime.
    /// </summary>
    public void Initialize()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_initialized)
        {
            throw new InvalidOperationException("LauncherRenderer.Initialize called twice.");
        }

        // Configure Camera2D к default centered view matching swapchain.
        _runtime.Camera.Position = Vector2.Zero;
        _runtime.Camera.ViewportSize = new Vector2(_runtime.Swapchain.Width, _runtime.Swapchain.Height);
        _runtime.Camera.Zoom = 1.0f;

        // Allocate command buffer + sync primitives (per-image renderFinished semaphores
        // per Vulkan binary semaphore reuse constraint).
        _commandBuffer = _runtime.GraphicsCommandPool.AllocateBuffer();
        _imageAvailable = new VulkanSemaphore(_runtime.VulkanDevice);
        _renderFinishedPerImage = new VulkanSemaphore[_runtime.Swapchain.ImageCount];
        for (int i = 0; i < _renderFinishedPerImage.Length; i++)
        {
            _renderFinishedPerImage[i] = new VulkanSemaphore(_runtime.VulkanDevice);
        }
        _frameFence = new VulkanFence(_runtime.VulkanDevice);

        _initialized = true;
    }

    /// <summary>
    /// Per-frame rendering: drain Bridge commands к dispatcher (updates SceneState),
    /// then record + submit Vulkan sprite frame from SceneState active sprites.
    /// </summary>
    public void RenderFrame(double deltaSeconds)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_initialized)
        {
            throw new InvalidOperationException("LauncherRenderer.RenderFrame called before Initialize.");
        }

        // 1. Drain Bridge commands → dispatcher updates SceneState
        _bridge.DrainCommands(_dispatcher.Dispatch);

        // 2. Acquire next swapchain image
        uint imageIndex = _runtime.Swapchain.AcquireNextImage(
            _imageAvailable!.Handle, IntPtr.Zero, out bool outOfDate);
        if (outOfDate)
        {
            _runtime.VulkanDevice.WaitIdle();
            _runtime.Swapchain.Recreate((uint)_runtime.Window.Width, (uint)_runtime.Window.Height);
            _runtime.RecreateFramebuffersForSwapchain();
            // Update camera viewport on resize
            _runtime.Camera.ViewportSize = new Vector2(_runtime.Swapchain.Width, _runtime.Swapchain.Height);
            return;  // Skip this frame; next iteration will acquire fresh image
        }

        // 3. Record + submit sprite frame
        _commandBuffer!.Reset();
        _commandBuffer.Begin(VkCommandBufferUsageFlagsPublic.OneTimeSubmit);
        
        _runtime.BeginRenderPassForSprites(
            _commandBuffer, (int)imageIndex,
            clearColor: new Vector4(0.05f, 0.10f, 0.20f, 1.0f));
        
        _runtime.SpriteRenderer.BeginFrame(imageIndex);
        foreach (PawnSpriteEntry entry in _sceneState.EnumerateActiveSprites())
        {
            var sprite = new Sprite(
                Texture: _atlasTexture,
                Region: entry.Region,
                Transform: new SpriteTransform(
                    Position: entry.Position,
                    Scale: entry.Scale,
                    Rotation: 0f,
                    TintRgba: SpriteVertex.WhiteTint));
            _runtime.SpriteRenderer.Submit(sprite);
        }
        _runtime.SpriteRenderer.EndFrame(_commandBuffer, _runtime.Camera);
        
        _runtime.EndSpriteRenderPass(_commandBuffer);
        _commandBuffer.End();

        // 4. Submit + present
        IntPtr renderFinishedHandle = _renderFinishedPerImage![imageIndex].Handle;
        _commandBuffer.SubmitTo(
            _runtime.VulkanDevice.GraphicsQueue,
            waitSemaphore: _imageAvailable.Handle,
            waitStage: VkPipelineStageFlagsPublic.ColorAttachmentOutput,
            signalSemaphore: renderFinishedHandle,
            fence: _frameFence!);

        bool presentOutOfDate = _runtime.Swapchain.Present(
            _runtime.VulkanDevice.GraphicsQueue, renderFinishedHandle, imageIndex);
        if (presentOutOfDate)
        {
            _runtime.VulkanDevice.WaitIdle();
            _runtime.Swapchain.Recreate((uint)_runtime.Window.Width, (uint)_runtime.Window.Height);
            _runtime.Camera.ViewportSize = new Vector2(_runtime.Swapchain.Width, _runtime.Swapchain.Height);
        }

        // 5. Wait + reset fence для next frame (К-L7 atomic-from-observer)
        _frameFence!.Wait();
        _frameFence.Reset();
    }

    public void Shutdown()
    {
        if (_disposed) return;
        
        if (_initialized)
        {
            _runtime.VulkanDevice.WaitIdle();
            if (_renderFinishedPerImage is not null)
            {
                foreach (VulkanSemaphore s in _renderFinishedPerImage)
                {
                    s.Dispose();
                }
            }
            _frameFence?.Dispose();
            _imageAvailable?.Dispose();
            _commandBuffer?.Dispose();
        }
        _disposed = true;
    }

    public bool IsRunning => !_disposed && _runtime.Window.IsOpen;

    public void Dispose() => Shutdown();
}
```

**Phase 0 dependency checks** (mid-cascade checkpoint if any differ от brief):
- `VkCommandBufferUsageFlagsPublic.OneTimeSubmit` enum value exists
- `VkPipelineStageFlagsPublic.ColorAttachmentOutput` enum value exists
- `_commandBuffer.SubmitTo(queue, waitSemaphore, waitStage, signalSemaphore, fence)` signature matches
- `_runtime.Swapchain.Present(queue, semaphore, imageIndex)` returns bool outOfDate
- `_runtime.Swapchain.AcquireNextImage(semaphore, fence, out bool outOfDate)` signature matches
- `SpriteVertex.WhiteTint` constant exists

##### β1+β2+β3.3 — `Program.cs` composition update

Replace cascade #2 Program.cs с integrated version (composition root pattern S-LOCK-10):

```csharp
using System;
using System.Numerics;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using DualFrontier.Runtime;
using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Launcher;

/// <summary>
/// Production launcher entry point. К-extensions cascade #3 (2026-05-DD):
/// composition extended к include atlas texture upload (LauncherProceduralAtlas →
/// VulkanImage → SpriteTexture) + SceneState composition root + dispatcher/renderer
/// constructor injection per S-LOCK-10.
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
            EnableValidationLayer = true,
            AssetsDirectory = "assets",
        };

        using var runtime = Runtime.Runtime.Create(runtimeOptions);

        // Generate procedural atlas + upload к device-local memory
        PngImage atlasImage = LauncherProceduralAtlas.GenerateAtlas();
        VulkanImage atlasVkImage = VulkanImage.CreateFromPngImage(
            runtime.VulkanDevice, runtime.MemoryAllocator, runtime.TextureUploader, atlasImage);
        var atlasSampler = new VulkanSampler(runtime.VulkanDevice);
        using var atlasTexture = new SpriteTexture(atlasVkImage, atlasSampler);

        var bridge = new PresentationBridge();
        GameContext gameContext = GameBootstrap.CreateLoop(bridge);

        var sceneState = new SceneState();
        var dispatcher = new RenderCommandDispatcher(runtime, sceneState, atlasTexture);
        using var renderer = new LauncherRenderer(runtime, bridge, dispatcher, sceneState, atlasTexture);

        // === Lifecycle init ===
        renderer.Initialize();
        runtime.Window.Show();
        gameContext.GameLoop.Start();

        // === Main loop (Q-G-7 (d) hybrid orchestration preserved от cascade #2) ===
        var lastFrameTime = DateTime.UtcNow;
        while (runtime.Window.IsOpen)
        {
            var now = DateTime.UtcNow;
            var deltaSeconds = (now - lastFrameTime).TotalSeconds;
            lastFrameTime = now;

            // 1. Pump Windows messages
            runtime.Window.PumpMessages();

            // 2. Drain input events (cascade #4+ territory — InputBridge wiring TBD)
            while (runtime.InputQueue.TryDequeue(out IInputEvent? _))
            {
                // Cascade #4+ will forward к Application input bridge here.
            }

            // 3. GameLoop self-ticks на background thread (cascade #2 confirmed)
            //    No explicit Tick() call needed.

            // 4. Render frame (drain bridge + dispatch + Vulkan sprite recording)
            renderer.RenderFrame(deltaSeconds);
        }

        // === Shutdown ===
        gameContext.GameLoop.Stop();
        renderer.Shutdown();
        return 0;
    }
}
```

**Verification gate**:
- `dotnet build` exit 0 (full solution)
- All 4 modified files compile together
- No new warnings introduced

**Commit message template**:
```
feat(launcher): K_EXT_3 β — Dispatcher pawn-3 + Renderer + Program composition (S-LOCK-1/3/4/8/9/10)

Per К-extensions cascade #3 Phase β: replace cascade #2 defensive throws с
real visual implementations для pawn-3 arms (PawnSpawned/Moved/Died); preserve
defensive throws для 3 deferred arms (PawnState/ItemSpawned/TickAdvanced) с
updated messages per S-LOCK-4 (post-Vanilla-mods deferral reason).

Modified (atomic compilable unit per Lesson #8):
- src/DualFrontier.Launcher/RenderCommandDispatcher.cs:
  - Constructor accepts SceneState + SpriteTexture
  - HandlePawnSpawned: deterministic tile assignment по PawnId hash; register в SceneState
  - HandlePawnMoved: SceneState.MovePawn (silent miss if not registered)
  - HandlePawnDied: SceneState.DespawnPawn (silent miss if not registered)
  - Deferred 3 arms: updated NotImplementedException messages per S-LOCK-4

- src/DualFrontier.Launcher/LauncherRenderer.cs:
  - Constructor accepts SceneState + SpriteTexture
  - Initialize: configure Camera2D default; allocate per-frame sync primitives
  - RenderFrame: drain bridge → dispatch → acquire image → record sprite frame
    from SceneState active sprites → submit → present
  - Shutdown: clean up sync primitives

- src/DualFrontier.Launcher/Program.cs:
  - Generate LauncherProceduralAtlas → upload к VulkanImage → SpriteTexture
  - Composition root: construct SceneState + dispatcher + renderer с DI per S-LOCK-10
  - Main loop unchanged (Q-G-7 (d) hybrid orchestration preserved)

S-LOCK-1 satisfied: pawn-3 active, 3 deferred с updated messages
S-LOCK-3 satisfied: scene state = sprite registry only
S-LOCK-4 satisfied: defensive throws updated к post-Vanilla-mods framing
S-LOCK-8 satisfied: RenderCommandDispatcher.Dispatch signature unchanged
S-LOCK-9 satisfied: LauncherRenderer.RenderFrame Vulkan integration matches
                     Runtime API empirical signatures per §2.0
S-LOCK-10 satisfied: constructor injection composition root

Build verified: dotnet build exit 0.
К-L14 verification #12 contribution: substrate (Runtime + PngDecoder + SpriteRenderer)
unchanged through cascade #3 consumer addition.
```

### Phase γ — Smoke verification (1 commit OR no-commit log only)

#### Commit γ1 — Launcher smoke test post-implementation

**Scope**:
- Execute `dotnet run --project src/DualFrontier.Launcher/`
- Manual verification: window opens, pawn sprites visible (if domain emits Spawned commands), sprites move (if domain emits Moved), sprites despawn (if domain emits Died), graceful close
- Record observations in execution log

**Note**: This is **verification activity**, не code change. If verification passes → execution log entry, no commit. If verification surfaces issues → cascade #3 amendment + Crystalka ratification.

**Critical verification points**:
- ✓ Window opens с procedural atlas-colored background visible
- ✓ If GameBootstrap creates pawns: colored tiles appear at pawn positions
- ✓ If domain ticks move pawns: tiles move smoothly
- ✓ If domain kills pawns: tiles despawn
- ✓ No Vulkan validation errors observed
- ✓ No crash на graceful close (X button или Alt+F4)
- ✓ Process exit code 0

**Verification execution log entry** (no commit needed if PASS):
```
γ1 (no-commit if PASS):
Launcher smoke executed post-Phase β. Observations:
- Window opened: [YES/NO]
- Pawn sprites visible: [count observed]
- Pawn movement observed: [YES/NO]
- Pawn despawn observed: [YES/NO]
- Vulkan validation: [clean / N errors]
- Graceful close: [YES/NO]
- Process exit code: [N]

Status: [PASS — no commit needed / FAIL — halt + amendment]
```


### Phase δ — Governance + REGISTER cascade (6-7 commits)

#### Commit δ1 — KERNEL_ARCHITECTURE.md v2.5.1 → v2.5.2

**Scope**:
- Edit `docs/architecture/KERNEL_ARCHITECTURE.md`:
  - Version: `2.5.1` → `2.5.2` (patch — chronicle + cross-ref, не architectural innovation)
  - Status footnote: К-extensions cascade #3 chronicle entry
  - Cross-reference к K_EXTENSIONS_LEDGER.md §3.4
  - К-L14 evidence count update: 11 → 12 (verification #12 added — first clean additive evidence)

**Status footnote addition template**:
```markdown
### К-extensions cascade #3 chronicle (2026-05-DD)

К-extensions cascade #3 — Launcher Visual Implementation (Minimum Scope) completed
2026-05-DD. Cascade scope:
- Pawn-3 dispatch arms (PawnSpawned/Moved/Died) — real implementations replacing
  cascade #2 defensive throws per Lesson #N12 first application
- SceneState + PawnSpriteEntry types (sprite registry minimum per Q-H-2)
- LauncherProceduralAtlas (Option C copy от SmokeTest's ProceduralAtlas) — preserves
  S-LOCK-2 substrate isolation
- LauncherRenderer Vulkan recording integration с SpriteRenderer V0.C.2 batched API
- 3 deferred arms (PawnState/ItemSpawned/TickAdvanced) — defensive throws preserved
  с messages updated к post-Vanilla-mods framing (Lesson #N12 second application)

К-L impact: zero (cascade focused on consumer materialization; no substrate work).
К-L count unchanged: 21 final.

К-L14 verification #12 — first clean additive evidence (substrate untouched через
consumer addition). Passing per Q-H-7 LOCKED framing.

Lesson #N12 second application captured: defensive throws preserved для 3 deferred
arms; #N12 stays Provisional pending substantially different third application.

Cross-references:
- docs/architecture/K_EXTENSIONS_LEDGER.md §3.4
- tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md
- docs/methodology/METHODOLOGY.md v1.12
- docs/architecture/PHASE_A_PRIME_SEQUENCING.md cascade #3 entry
- К-L14 row: evidence count 11 → 12 (verification #12)

Cascade #2 retroactive ratification: R-1 + R-2 Phase 0 verification gates executed
в cascade #3 produced [CLEAN / ANNOTATE — outcome populated at execution]. Cascade
#2 К-L14 #11 status: [CLEAN-WITH-ANNOTATION upgraded к CLEAN / unchanged].
```

**Commit message template**:
```
docs(kernel): K_EXT_3 δ1 — KERNEL_ARCHITECTURE.md v2.5.1 → v2.5.2 (Q-H-10 patch)

Per К-extensions cascade #3: version 2.5.1 → 2.5.2 (patch — chronicle + cascade
#3 entry + К-L14 evidence count update 11 → 12).

К-L count unchanged: 21 final.
К-L14 evidence count: 11 → 12 (verification #12 added — first clean additive evidence).

Build verified: dotnet build exit 0 (documentation only).
sync_register.ps1 --validate gate: will pass post-δ6 REGISTER update.
```

#### Commit δ2 — METHODOLOGY.md v1.11 → v1.12

**Scope**:
- Edit `docs/methodology/METHODOLOGY.md`:
  - Version: `1.11` → `1.12` (minor — new Provisional candidates added + #N12 second-app evidence note)
  - Lesson #N12 entry updated с second-application evidence (this cascade)
  - **Lesson #N13 (Provisional, NEW)**: Commit integrity verification before commit
  - **Lesson #N14 (Provisional, NEW)**: Phase 0 reads must cover assumed-state empirically

**Lesson #N12 second-application note template**:
```markdown
### Lesson #N12 — Defensive Reserved Stub Pattern (Provisional, second application)

**Status**: Provisional (first application К-extensions cascade #2; second application
К-extensions cascade #3)

[... original statement unchanged ...]

**Applications**:
1. **К-extensions cascade #2 (2026-05-DD)**: Launcher's RenderCommandDispatcher all 6
   dispatch handler methods throw NotImplementedException pending cascade #3 
   visual implementation.
2. **К-extensions cascade #3 (2026-05-DD)**: 3 of 6 dispatch handlers (PawnState/
   ItemSpawned/TickAdvanced) preserved as defensive throws с updated messages
   reflecting post-Vanilla-mods deferral reason. Pattern proved reusable для
   incremental implementation cascades — pattern applies WHEN structural surface
   needed but consumer materialization staged.

**Promotion criterion (Provisional → FORMALIZE)**: Substantially different third
application (different domain — not just more dispatcher arms). Carry-forward.
```

**Lesson #N13 candidate template**:
```markdown
### Lesson #N13 candidate — Commit integrity verification before commit

**Status**: Provisional (first observation К-extensions cascade #2 α1)

**Statement**: Commit messages MUST не claim mutations что are не actually staged.
Before committing, execution agent verifies `git diff --cached` content matches
commit message claims. Mismatches trigger correction commits (per Lesson #8
discipline — separate commit, не amend), не silent commits с false claims.

**Rationale**: К-extensions cascade #2 α1 commit message claimed sln mutation
that was не actually staged. Discovery 4 commits later led к α1.5 correction
commit. Honest discovery + clean correction protocol preserved discipline, но
prevention reduces correction overhead.

**Pattern**:
```bash
# Before commit:
git status                    # what's staged
git diff --cached --stat       # what staged changes touch
# Match against commit message claims before `git commit`
```

**Anti-patterns**:
- Committing с commit message describing intent rather than actual diff
- Skipping `git status` verification step
- `git commit -am` без explicit add verification

**First observation**: К-extensions cascade #2 α1 (2026-05-23) — commit `a52996d`
claimed «sln Project entry removed + 12 ProjectConfigurationPlatforms + 1
NestedProjects» but actual diff omitted sln mutation entirely. Discovered β1
when build verification surfaced unstaged sln modifications. α1.5 correction
commit `8a8e507` resolved.

**Promotion criterion (Provisional → FORMALIZE)**: Second application с different
mutation type. Carry-forward.
```

**Lesson #N14 candidate template**:
```markdown
### Lesson #N14 candidate — Phase 0 reads empirical assumed-state coverage

**Status**: Provisional (first observation К-extensions cascade #2 α1 + cascade #3 §2.0)

**Statement**: Phase 0 brief reads MUST cover assumed-state directories + files
empirically, not by assumption. When brief assumes «directory X contains only Y»
or «no untracked files в Z», execute `git ls-files` (для tracked content) +
`Get-ChildItem` (для disk content) before commit cascade begins.

**Rationale**: 
- Cascade #2 brief assumed `Presentation/` was untracked-only directory; empirical
  state had ~45 tracked files. Required α1 scope expansion mid-cascade.
- Cascade #3 deliberation agent verified PNG asset formats empirically via Filesystem
  MCP, surfacing palette-indexed format что overrode initial RGB/RGBA assumption.
  Saved ~500 lines of speculative substrate-extension brief content.

**Pattern**:
```powershell
# Before relying on directory state assumption:
git ls-files <path>                # tracked content
Get-ChildItem <path> -Recurse -Force  # disk content (including untracked, gitignored)
# Verify both before proceeding
```

For asset/binary content verification:
```python
# Read binary header before relying on format assumption:
import struct
with open(path, 'rb') as f:
    sig = f.read(8)  # PNG signature
    # ... parse IHDR
```

**Anti-patterns**:
- Assuming directory state without empirical check
- Inferring file format от extension только (PNG can be RGB8/RGBA8/palette/grayscale/16-bit/interlaced)
- Trusting memory recall over filesystem reads when deliberation-agent has filesystem access

**First observation**: К-extensions cascade #2 α1 (2026-05-23) + cascade #3 §2.0
(2026-05-23) — both surfaced empirical state divergence от assumption.

**Promotion criterion (Provisional → FORMALIZE)**: Second application с different
state-class. Likely promotes quickly given how often assumption-vs-empirical gap
matters. Carry-forward.
```

**Lessons batch counts**:
- FORMALIZE: 12 → 12 (unchanged)
- DEFER (Provisional): 10 → 12 (#N13 + #N14 added)
- SUNSET: 1 → 1 (unchanged)

**Commit message template**:
```
docs(methodology): K_EXT_3 δ2 — METHODOLOGY v1.11 → v1.12 (#N12 + #N13 + #N14)

Per К-extensions cascade #3:
- Version 1.11 → 1.12 (minor — 2 new Provisional candidates + #N12 second-app note)
- Lesson #N12: second application captured (cascade #3 3 of 6 dispatch arms
  remained defensive throws с updated messages); promotion criterion preserved
  (substantially different third application needed)
- Lesson #N13 (Provisional, NEW): Commit integrity verification before commit
  (cascade #2 α1 first observation; carry-forward к second application)
- Lesson #N14 (Provisional, NEW): Phase 0 reads empirical assumed-state coverage
  (cascade #2 α1 + cascade #3 §2.0 first observations; promotes quickly likely)

Lessons batch counts:
- FORMALIZE: 12 → 12 (unchanged)
- DEFER (Provisional): 10 → 12 (#N13 + #N14 added)
- SUNSET: 1 → 1 (unchanged)

К-L14 thesis interaction: #N13 + #N14 both support К-L14 evidence quality —
honest commit/state discipline prevents false-positive evidence.

Build verified: dotnet build exit 0 (documentation only).
```

#### Commit δ3 — PHASE_A_PRIME_SEQUENCING.md cascade #3 entry

**Scope**:
- Append к `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`:

```markdown
## К-extensions cascade #3 — Launcher Visual Implementation (Minimum Scope)

**Designation**: К-extensions cascade #3 (post-cascade-#2 closure)  
**Dates**: 
- Deliberation session: 2026-05-23
- Brief authoring: 2026-05-23
- Execution: 2026-05-DD
- Closure: 2026-05-DD

**Scope** (per Path B Hybrid + Path γ-A asset strategy):
- Phase α: Verification + ProceduralAtlas integration (R-1/R-2 gates + LauncherProceduralAtlas + SceneState)
- Phase β: Dispatcher pawn-3 + Renderer Vulkan integration + Program composition
- Phase γ: Launcher smoke verification (pawns visible/move/despawn)
- Phase δ: Governance cascade (KERNEL v2.5.1→v2.5.2; METHODOLOGY v1.11→v1.12; LEDGER §3.4; REGISTER 3-commit)
- Phase ε: К-L14 verification #12 — first clean additive evidence

**К-L impact**: zero (К-L count unchanged: 21 final).

**Lessons surfaced**:
- Lesson #N12 — second application (provisional preserved)
- Lesson #N13 (Provisional, NEW) — commit integrity verification
- Lesson #N14 (Provisional, NEW) — Phase 0 empirical assumed-state coverage
- Lesson #N15 — carry-forward (substrate extension protocol; не applied этой cascade)
- Lesson #N16 — carry-forward (pre-authoring empirical grounding; meta-applied этой cascade)

**К-L14 verification**: #12 (first clean additive evidence) — passing per Q-H-7 LOCKED.

**Cascade #2 retroactive ratification**: R-1 + R-2 Phase 0 gates produced [outcome].
Cascade #2 К-L14 #11: [CLEAN-WITH-ANNOTATION upgraded к CLEAN / unchanged].

**Atomic commits**: [N commits] (within Q-H-10 budget 12-15).

**Cross-references**:
- tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md
- docs/architecture/K_EXTENSIONS_LEDGER.md §3.4
- docs/architecture/KERNEL_ARCHITECTURE.md v2.5.2
- docs/methodology/METHODOLOGY.md v1.12

**Forward sequencing**:
1. Pre-existing pollution cleanup cascade (`CreateLoop_RunningLoop_PawnStateCommandCarriesRealName`)
2. V2 amendment brief authoring (if needed)
3. A'.9 Roslyn analyzer milestone
4. Substrate palette decoder extension (когда Vanilla mods materialize consumer need; first
   K-L14 substrate-extension-evidence opportunity — Lesson #N15 first application)
5. Phase B M-cycle preparation (Vanilla mods)
```

**Commit message template**:
```
docs(sequencing): K_EXT_3 δ3 — PHASE_A_PRIME_SEQUENCING entry

Per К-extensions cascade #3: append chronological entry к Phase A' sequencing.

Build verified: dotnet build exit 0 (documentation only).
```

#### Commit δ4 — K_EXTENSIONS_LEDGER.md §3.4 entry

**Scope**:
- Edit `docs/architecture/K_EXTENSIONS_LEDGER.md`:
  - Add §3.4 cascade #3 entry
  - Update §4 forward roadmap (cascade #3 closed, next cascades anticipated)

**§3.4 entry template**:
```markdown
### §3.4 — К-extensions cascade #3 — Launcher Visual Implementation (Minimum Scope)

**Designation**: К-extensions cascade #3  
**Dates**: Authored 2026-05-23, Executed 2026-05-DD, Closed 2026-05-DD  
**Brief**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md`

**Scope summary**: Replace cascade #2 defensive throws с real visual implementations
для pawn-3 dispatch arms (PawnSpawned/Moved/Died). 3 deferred arms (PawnState/
ItemSpawned/TickAdvanced) preserved as defensive throws с updated messages reflecting
post-Vanilla-mods deferral. SceneState minimum sprite registry. LauncherProceduralAtlas
copy (Option C per Q-H-17 ratification) preserves S-LOCK-2 substrate isolation.

**К-L impact**: zero (consumer materialization only). К-L count unchanged: 21.

**Lessons surfaced**:
- Lesson #N12 — second application (Provisional preserved; promotion criterion
  requires substantially different third application)
- Lesson #N13 (Provisional, NEW) — commit integrity verification
- Lesson #N14 (Provisional, NEW) — Phase 0 empirical assumed-state coverage

**К-L14 verification**: #12 — first clean additive evidence. Pass per Q-H-7 LOCKED.

**Cascade #2 retroactive**: R-1 + R-2 Phase 0 gates produced [outcome]; cascade #2
К-L14 #11 [retroactively upgraded к CLEAN / unchanged].

**Atomic commits**: [N] (within Q-H-10 budget).

**Closure notes**:
- KERNEL v2.5.1 → v2.5.2 (patch — chronicle + К-L14 #12)
- METHODOLOGY v1.11 → v1.12 (minor — #N12 second-app + #N13 + #N14)
- register_version 2.4 → 2.5
- K_EXTENSIONS_LEDGER §3.4 added
- К-extensions cascade #4 (next) — TBD per emergent need
```

**§4 forward roadmap update**:
```markdown
## §4 — Forward roadmap

Anticipated К-extensions cascades:
- **Pre-existing pollution cleanup cascade** — flaky test stabilization
  (`CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` + 9 other flaky tests)
- **Substrate palette decoder extension cascade** — when Vanilla mods materialize
  consumer need для kenney/standard PNG asset loading; first К-L14
  substrate-extension-evidence opportunity (Lesson #N15 first application)
- **К-extensions cascade #4+** — emergent per V-series, FO-series, Phase B needs
```

**Commit message template**:
```
docs(architecture): K_EXT_3 δ4 — K_EXTENSIONS_LEDGER §3.4 + forward roadmap update

Per К-extensions cascade #3: add §3.4 entry к ledger + update §4 forward roadmap.

Forward cascades anticipated:
- Pre-existing pollution cleanup
- Substrate palette decoder extension (Lesson #N15 first application opportunity)
- Cascades emergent per V-series, FO-series, Phase B

Build verified: dotnet build exit 0 (documentation only).
```

#### Commit δ5 — REGISTER cascade Commit α (enrollments + modifications)

**Scope** — `docs/governance/REGISTER.yaml` mutations:

**Additions**:
- Add `DOC-F-SRC-LAUNCHER-PROCEDURAL-ATLAS`:
  ```yaml
  - id: DOC-F-SRC-LAUNCHER-PROCEDURAL-ATLAS
    path: src/DualFrontier.Launcher/LauncherProceduralAtlas.cs
    title: "Launcher Procedural Atlas — Cascade #3 Path γ-A Asset Source"
    category: F
    tier: 4
    lifecycle: Live
    owner: Crystalka
    version: "Live"
    last_modified: "2026-05-DD"
    last_modified_commit: "PENDING-COMMIT-K_EXT_3-CLOSURE"
    content_language: en
    review_cadence: phase-led
    last_review_date: "2026-05-DD"
    last_review_event: "К-extensions cascade #3 α1 — Option C copy от SmokeTest's ProceduralAtlas; preserves S-LOCK-2 substrate isolation; forward consolidation candidate when palette decoder extension materializes"
    next_review_due: "TBD — substrate palette decoder cascade"
    reviewer: Crystalka
  ```

- Add `DOC-F-SRC-LAUNCHER-SCENE-STATE`:
  ```yaml
  - id: DOC-F-SRC-LAUNCHER-SCENE-STATE
    path: src/DualFrontier.Launcher/SceneState.cs
    title: "Launcher SceneState — Cascade #3 Minimum Scope Sprite Registry"
    category: F
    tier: 4
    lifecycle: Live
    owner: Crystalka
    version: "Live"
    last_modified: "2026-05-DD"
    last_modified_commit: "PENDING-COMMIT-K_EXT_3-CLOSURE"
    content_language: en
    review_cadence: phase-led
    last_review_date: "2026-05-DD"
    last_review_event: "К-extensions cascade #3 α2 — minimum scope sprite registry per Q-H-2 LOCKED; no camera/HUD/layer machinery"
    next_review_due: "TBD — cascade #4 if scene state scope expands"
    reviewer: Crystalka
  ```

- Optionally add: `DOC-F-SRC-LAUNCHER-PAWN-SPRITE-ENTRY` (Tier 4 record type — может consolidate с SceneState entry; Crystalka discretion)

**Modifications**:
- `DOC-F-SRC-LAUNCHER` `last_modified` + `last_review_event` updates (Program + LauncherRenderer + RenderCommandDispatcher modifications)
- `DOC-A-KERNEL`: `version: "2.5.1"` → `version: "2.5.2"`
- `DOC-A-METHODOLOGY`: `version: "1.11"` → `version: "1.12"`

**Verification gate**:
- `sync_register.ps1 --validate` exit 0
- All new entries schema-valid

**Commit message template**:
```
governance(register): K_EXT_3 δ5 — REGISTER Commit α (enrollments + DOC modifications)

Per К-extensions cascade #3 governance cascade:

Added:
- DOC-F-SRC-LAUNCHER-PROCEDURAL-ATLAS (Tier 4 Live Category F)
- DOC-F-SRC-LAUNCHER-SCENE-STATE (Tier 4 Live Category F)

Modified:
- DOC-F-SRC-LAUNCHER: last_modified update (Program/Renderer/Dispatcher mods)
- DOC-A-KERNEL: version 2.5.1 → 2.5.2
- DOC-A-METHODOLOGY: version 1.11 → 1.12

sync_register.ps1 --validate gate: exit 0.
Build verified: dotnet build exit 0.

Forward Commits β + γ apply register_version bump + EVT + К-L14 #12 entry.
```

#### Commit δ6 — REGISTER cascade Commit β + γ (register_version + EVT + K_L14 #12)

**Scope** — REGISTER.yaml mutations:

**Commit β mutations** (К-L14 dashboard update):
- `DOC-A-K_L14_EVIDENCE_DASHBOARD` `last_modified` update (verification #12 entry appended)

**Commit γ mutations** (governance metadata):
- `register_version`: `"2.4"` → `"2.5"`
- `last_modified`: `"2026-05-DD"`
- Add new EVT к `audit_trail`:
  ```yaml
  - event_id: EVT-2026-05-DD-K_EXT_3-LAUNCHER_VISUAL_IMPLEMENTATION
    date: "2026-05-DD"
    type: cascade_closure
    cascade: К-extensions cascade #3
    title: "Launcher Visual Implementation (Minimum Scope)"
    summary: "Per К-extensions cascade #3 deliberation session 2026-05-23 + Crystalka ratification 2026-05-DD: replace cascade #2 defensive throws с real visual implementations для pawn-3 dispatch arms (PawnSpawned/Moved/Died); 3 deferred arms preserved as defensive throws с updated messages per Lesson #N12 second application. SceneState minimum sprite registry. LauncherProceduralAtlas Option C copy preserves S-LOCK-2 substrate isolation. Path γ-A asset strategy: substrate palette decoder extension deferred к когда Vanilla mods materialize consumer need. К-L impact: zero. К-L14 verification #12 — first clean additive evidence."
    affected_docs:
      - DOC-A-KERNEL (v2.5.1 → v2.5.2)
      - DOC-A-METHODOLOGY (v1.11 → v1.12)
      - DOC-F-SRC-LAUNCHER-PROCEDURAL-ATLAS (newly enrolled)
      - DOC-F-SRC-LAUNCHER-SCENE-STATE (newly enrolled)
      - DOC-F-SRC-LAUNCHER (modified)
      - DOC-A-K_EXTENSIONS_LEDGER §3.4 (updated)
      - DOC-A-K_L14_EVIDENCE_DASHBOARD (verification #12 added)
    decisions_ratified:
      - Q-H-0 Path B Hybrid
      - Q-H-1 Pawn-3 active, 3 deferred
      - Q-H-2 Scene state = sprite registry only
      - Q-H-3 Path γ-A (procedural-only, no substrate work)
      - Q-H-5 R-1 fail-annotate + R-2 fail-halt
      - Q-H-6 Test discipline = pawn-3 only
      - Q-H-7 К-L14 #12 = clean additive evidence
      - Q-H-8 К-L17.1 deferred
      - Q-H-10 METHODOLOGY v1.11 → v1.12
      - Q-H-17 Option C (mid-cascade checkpoint ratification)
    lessons_surfaced:
      - Lesson #N12 — second application (Provisional preserved)
      - Lesson #N13 (Provisional, NEW): commit integrity verification
      - Lesson #N14 (Provisional, NEW): Phase 0 empirical assumed-state coverage
    k_l_impact: "К-L count unchanged: 21 final. К-L14 evidence count 11 → 12 (verification #12 — first clean additive evidence)."
    verification:
      type: "Clean additive evidence (first of kind)"
      criterion: "7-point matrix per Q-H-7 LOCKED + Lesson #N12 second-application discipline"
      status: "[TBD — populate post-execution closure: CLEAN / SOFT-HALT / FAIL]"
    cascade_2_retroactive:
      r_1_outcome: "[CLEAN retroactive / ANNOTATE — populate at execution]"
      r_2_outcome: "[PASS / FAIL — populate at execution]"
      cascade_2_k_l14_11_status: "[CLEAN-WITH-ANNOTATION upgraded к CLEAN / unchanged]"
    brief: tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md
    cross_references:
      - docs/architecture/K_EXTENSIONS_LEDGER.md §3.4
      - docs/architecture/KERNEL_ARCHITECTURE.md v2.5.2
      - docs/methodology/METHODOLOGY.md v1.12
      - docs/architecture/PHASE_A_PRIME_SEQUENCING.md cascade #3 entry
  ```

- Add CAPA entries if Phase 0 or smoke verification surfaced findings requiring tracking

**Verification gate**:
- `sync_register.ps1 --validate` exit 0
- register_version field shows "2.5"
- audit_trail entry present + well-formed

**Commit message template**:
```
governance(register): K_EXT_3 δ6 — REGISTER Commits β + γ (DOC modifications + register_version + EVT)

Per К-extensions cascade #3 governance cascade Commits β + γ combined:

Commit β (DOC modifications):
- DOC-A-K_L14_EVIDENCE_DASHBOARD: last_modified update (verification #12 added)

Commit γ (governance metadata):
- register_version: 2.4 → 2.5
- Added к audit_trail: EVT-2026-05-DD-K_EXT_3-LAUNCHER_VISUAL_IMPLEMENTATION

sync_register.ps1 --validate gate: exit 0.
Build verified: dotnet build exit 0.

К-extensions cascade #3 governance cascade complete.
Brief AUTHORED → EXECUTED transition в δ7 final commit.
```

#### Commit δ7 — Brief AUTHORED → EXECUTED + closure section

**Scope**:
- Edit `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md`:
  - Status header: `AUTHORED — pending Crystalka RATIFICATION + Claude Code execution` →
    `EXECUTED — К-extensions cascade #3 closure 2026-05-DD`
  - Append `## §9 — Closure section`

**Closure section template**:

```markdown
## §9 — Closure section (К-extensions cascade #3 execution outcomes)

**Executed**: 2026-05-DD
**Closure timestamp**: [final commit hash]
**Pushed к origin/main**: 2026-05-DD

### §9.1 — Atomic commit summary

[Populate с actual commit hashes per execution]

Total commits: [N] (within Q-H-10 budget 12-15)

### §9.2 — К-L14 verification #12 outcome (first clean additive evidence)

Per Q-H-7 LOCKED 7-point matrix:

| Criterion | Pre-cascade baseline | Post-cascade state | Pass/Fail |
|---|---|---|---|
| 1. Build integrity | dotnet build exit 0 | [actual] | [✓/✗] |
| 2. Test count parity (R-1) | [from R-1 outcome] | [actual] | [✓/✗] |
| 3. Pre-existing pollution NOT worse | [from R-1 outcome] | [actual] | [✓/✗] |
| 4. Runtime API surface unchanged | dotnet build exit 0 | [actual] | [✓/✗] |
| 5. SmokeTest still passes | passes | [actual] | [✓/✗] |
| 6. Launcher smoke (pawns visible/move/despawn) | N/A pre-cascade | [actual γ1] | [✓/✗] |
| 7. METHODOLOGY §12.7 Modding gate | passes | [actual] | [✓/✗] |

К-L14 verification #12 final status: **[CLEAN / SOFT-HALT / FAIL]**

### §9.3 — Cascade #2 retroactive ratification

R-1 outcome: [pattern match / divergent]
- Run 1: [fails]
- Run 2: [fails]
- Run 3: [fails]
- Cascade #2 K-L14 #11 status: [CLEAN-WITH-ANNOTATION upgraded к CLEAN / unchanged]

R-2 outcome: [PASS / FAIL]
- Launcher composition smoke результат: [details]
- Cascade #2 verification gap closed: [YES / NO]

### §9.4 — Lessons applied + surfaced

Applied:
- Lesson #N12 — second application (3 of 6 dispatch arms preserved as defensive throws)
- Lesson #N14 — pre-authoring empirical grounding via deliberation-agent filesystem access (meta-application — saved ~500 lines speculative content)

Surfaced (added к METHODOLOGY v1.12):
- Lesson #N13 (Provisional, NEW): commit integrity verification
- Lesson #N14 (Provisional, NEW): Phase 0 empirical assumed-state coverage

Carry-forward (no application этой cascade):
- Lesson #N15 — К-L14 substrate extension protocol (waits для substrate gap consumer-driven)
- Lesson #N16 — pre-authoring empirical grounding (incorporated в #N14 effectively)

### §9.5 — Cascade closure ratification

Crystalka ratification: [confirm at execution]
Final commit pushed к origin/main: [commit hash + timestamp]
К-extensions cascade #3 formally CLOSED.

**Forward task**: 
- Pre-existing pollution cleanup cascade
- OR Phase B M-cycle preparation
- (substrate palette decoder extension reserved для когда Vanilla mods consumer need materializes)
```

**Commit message template**:
```
governance(brief): K_EXT_3 δ7 — Brief AUTHORED → EXECUTED + closure section

Per К-extensions cascade #3 closure protocol: transition brief status + append
closure section с execution outcomes.

К-extensions cascade #3 formally CLOSED.
Forward task: pre-existing pollution cleanup OR Phase B preparation.

Final build verified: dotnet build exit 0.
Final sync_register.ps1 --validate: exit 0.
```

### Phase ε — Verification (0 separate commits)

Verification protocol executed at cascade closure. Records appended к δ7 brief closure section.

**Verification execution checklist** (run before δ7):

1. **Build integrity check**: `dotnet build` exit 0 (full solution)
2. **Test suite execution**: `dotnet test` → record test count + per-suite results; compare к R-1 baseline
3. **Pre-existing pollution check**: confirm flaky test pattern matches R-1 baseline (NOT worse)
4. **Runtime SmokeTest execution**: `dotnet run --project tests/DualFrontier.Runtime.SmokeTest/` — verify passes
5. **Launcher visual smoke**: `dotnet run --project src/DualFrontier.Launcher/` — manual verification:
   - Window opens
   - Procedural-colored pawn sprites visible (when domain emits commands)
   - Sprites move на PawnMoved dispatch
   - Sprites despawn на PawnDied dispatch
   - No Vulkan validation errors
   - Graceful close
6. **METHODOLOGY §12.7 Modding-suite verification gate**: per protocol
7. **`sync_register.ps1 --validate`**: exit 0

**Falsifiability conditions** — if any of these trigger during verification:
- ✗ Test count decreases beyond R-1 baseline pollution adjustment
- ✗ Runtime API breaking change introduced
- ✗ SmokeTest regresses
- ✗ Vulkan validation errors appear
- ✗ Launcher visual smoke fails (no pawns visible OR sprite registry not updating)
- ✗ Pre-existing pollution worsens

→ **HALT execution**. Crystalka ratification re: continuation OR roll-back required.

---

## §4 — Halt conditions

Execution agent MUST halt + request Crystalka ratification on any of the following:

### 4.1 — Brief scope violations

- **Halt condition SC-1**: Attempt к implement any of 3 deferred dispatch arms (PawnState/ItemSpawned/TickAdvanced) — S-LOCK-1 violation
- **Halt condition SC-2**: Attempt к modify substrate PngDecoder (palette support OR other format extension) — S-LOCK-2 violation; Path γ-A explicit
- **Halt condition SC-3**: Attempt к add camera state, HUD state, layer ordering к SceneState — S-LOCK-3 violation
- **Halt condition SC-4**: Empty body, no-op stub, либо silent default-return в deferred dispatcher arms — S-LOCK-4 violation; Lesson #N12 requires loud throws
- **Halt condition SC-5**: IRenderCommand.Execute() restoration в any form — S-LOCK-6 violation
- **Halt condition SC-6**: RenderCommandDispatcher.Dispatch signature OR handler method signatures modified — S-LOCK-8 violation
- **Halt condition SC-7**: SceneState as singleton OR static — S-LOCK-10 violation; constructor injection required
- **Halt condition SC-8**: Real kenney asset file usage в cascade #3 — Path γ-A explicit

### 4.2 — R-2 verification gate failure (cascade #2 verification dependency)

- **Halt condition VG-1**: R-2 Launcher composition smoke FAIL на cascade #2 closure state — cascade #2 amendment required before cascade #3 proceeds

### 4.3 — К-L14 verification #12 falsifiability triggers

- **Halt condition VF-1**: Test count decreases beyond R-1 baseline pollution adjustment
- **Halt condition VF-2**: Runtime API breaking change introduced (substrate touched)
- **Halt condition VF-3**: Runtime SmokeTest regresses
- **Halt condition VF-4**: Vulkan validation errors appear during smoke
- **Halt condition VF-5**: Launcher visual smoke fails (pawns not visible OR not moving OR not despawning when expected)
- **Halt condition VF-6**: Pre-existing pollution worsens

### 4.4 — Phase 0 reads inconsistencies (mid-cascade checkpoint triggers)

- **Halt condition P0-1**: Runtime sprite recording method signatures differ от §2.0 empirical findings (`BeginRenderPassForSprites`, `EndSpriteRenderPass`, etc.)
- **Halt condition P0-2**: SpriteRenderer V0.C.2 API signatures differ (`BeginFrame`, `Submit`, `EndFrame`)
- **Halt condition P0-3**: Bridge Command record parameter names differ (PawnId/X/Y/etc.)
- **Halt condition P0-4**: VulkanImage.CreateFromPngImage factory method missing OR signature differs
- **Halt condition P0-5**: §2.0 empirical findings re: PNG format support diverge from current state (e.g. palette support added between deliberation и execution)
- **Halt condition P0-6**: ProceduralAtlas pattern not findable в SmokeTest directory (Q-H-17 Option C copy not feasible)

For halts P0-*: mid-cascade checkpoint per Path B Hybrid discipline — execution agent reports к Crystalka, Q-H detail re-ratified before continuation.

### 4.5 — sync_register.ps1 validation failure

- **Halt condition SR-1**: `sync_register.ps1 --validate` returns non-zero exit code at any REGISTER cascade commit

### 4.6 — Build break

- **Halt condition BB-1**: `dotnet build` returns non-zero exit code at any commit per Lesson #8 strengthened

### 4.7 — Commit integrity (Lesson #N13 first application)

- **Operational discipline** (не halt — preventive): Before each commit, execute `git status` + `git diff --cached --stat` к verify staged changes match commit message claims. Lesson #N13 first application.
- **Halt condition CI-1**: If post-commit verification surfaces commit message-vs-diff mismatch (cascade #2 α1 pattern), halt + correction commit (per Lesson #8 «new commits over amend») before continuing.

### 4.8 — Auto-mode classifier push block (operational reminder)

- **Operational note** (не halt — expected behavior): Claude Code auto-mode classifier blocks push-to-main even с explicit user instruction. Requires в-session re-confirmation after halt + resolution work. Push step requires Crystalka re-confirmation в-session.

---

## §5 — Closure protocol

### 5.1 — Pre-closure verification execution

Execute Phase ε verification protocol. Document outcomes в brief δ7 closure section.

### 5.2 — К-L14 verification #12 evidence capture

Populate `K_L14_EVIDENCE_DASHBOARD.md` с verification #12 entry:

```markdown
## Verification #12 — К-extensions cascade #3 (Launcher Visual Implementation Minimum Scope)

**Date**: 2026-05-DD
**Type**: Clean additive evidence (first of kind)
**Cascade**: К-extensions cascade #3

**Pre-cascade baseline** (R-1 outcome):
- Tests: [N] passing / [M] flaky
- Pre-existing pollution: [list]
- Runtime SmokeTest: passes
- Launcher smoke (R-2): [PASS]
- Solution: 30 projects

**Post-cascade state**:
- Tests: [N] passing / [M] flaky (variance pattern preserved)
- Pre-existing pollution: [unchanged / improved]
- Runtime SmokeTest: passes (substrate untouched verified)
- Launcher visual smoke: PASS (pawns visible, move, despawn)
- Solution: 30 projects (no project changes этой cascade)
- Vulkan validation: clean

**Status**: [CLEAN / SOFT-HALT / FAIL]

**Honest-framed evidence note**: First clean additive К-L14 verification. Substrate
(Runtime + PngDecoder + SpriteRenderer + ProceduralAtlas) primitives completely
untouched through consumer materialization (3 dispatch arm implementations + SceneState +
LauncherRenderer integration). К-L14 thesis preserved: substrate stability через
additive consumer churn empirically verified.

**Cascade #2 retroactive impact**: R-1 + R-2 Phase 0 gates closed cascade #2 verification
gaps; cascade #2 К-L14 #11 [CLEAN-WITH-ANNOTATION → CLEAN / unchanged based on R-1/R-2 outcomes].
```

### 5.3 — Crystalka ratification + push к origin/main

Same protocol as cascade #2:
1. Execution agent reports closure-ready state
2. Crystalka reviews execution log + final commit list
3. Verifies К-L14 #12 evidence + cascade #2 retroactive outcome
4. Approves OR amendment
5. Authorizes push к origin/main

### 5.4 — Forward sequencing

Per cascade #3 Q-H-10 deliberation + forward roadmap:
1. **Next**: Pre-existing pollution cleanup cascade
2. **OR**: Phase B M-cycle preparation (depending на Crystalka priority)
3. **Reserved**: Substrate palette decoder extension cascade (Lesson #N15 first application; triggers когда Vanilla mods materialize consumer need)
4. **Reserved**: A'.9 Roslyn analyzer milestone

---

## §6 — Forward consideration

### 6.1 — Substrate palette decoder extension — future cascade

К-extensions cascade #3 explicitly deferred substrate palette decoder extension per Path γ-A discipline. Future cascade triggers когда:
- Vanilla mods materialize consumer need для real asset loading
- Modder workflow requires kenney-format asset usage
- HUD primitives require font sprite atlas loading (typically palette PNG)

At that point:
- К-L14 #N (depending on cascade number) = **first К-L14 substrate-extension evidence**
- Lesson #N15 «К-L14 substrate extension protocol» first application
- Phase α₁ = decoder format extension; Phase α₂ = consumer materialization

### 6.2 — К-L17.1 sub-invariant candidacy (still deferred)

Cascade #3 minimum scope не materialized composition framework binding (Q-H-2 LOCKED scene state = sprite registry only, no camera composition). К-L17.1 candidate stays deferred к когда composition framework actually consumed by Launcher (later cascade — likely когда multi-layer rendering OR HUD overlay surfaces).

### 6.3 — Lesson #N12 third application opportunity

Cascade #3 second application preserved Lesson #N12 Provisional status. Promotion к FORMALIZE requires **substantially different third application** — different domain, не just more dispatcher arms. Likely candidates:
- HUD primitives reserved skeleton с defensive throws (когда HUD subsystem materializes)
- Audio dispatcher reserved skeleton (когда audio subsystem materializes)
- Network sync reserved skeleton (когда multiplayer prep occurs)

### 6.4 — Lesson #N13/#N14 second-application watch

Cascade #3 introduces #N13 + #N14 as Provisional. Forward cascades should watch для opportunities:
- #N13 (commit integrity): any cascade с >5 commits is high-probability second-application surface
- #N14 (Phase 0 empirical): any cascade с filesystem-accessible deliberation agent гets natural application

Promotion к FORMALIZE expected within next 2-3 cascades.

### 6.5 — A'.9 Roslyn analyzer preparation

Cascade #3 establishes additional analyzer rule candidates beyond cascade #2 contributions:
- Dispatch arm pattern enforcement («handler methods returning void must throw OR mutate scene state, не silent no-op»)
- Defensive throw message convention enforcement (regex match для «pending [cascade/era] cascade. ... Lesson #N12»)
- Constructor injection pattern enforcement (no singleton/static в Launcher)

A'.9 brief authoring incorporates these signals.

---

## §7 — Cross-references

### 7.1 — Predecessor briefs

- `tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md` — К-extensions cascade #2 (Launcher infrastructure)
- `tools/briefs/A_PRIME_7_5_BUS_SOURCE_SPLIT_BRIEF.md` — К-extensions cascade #1
- `tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md` — К-extensions cascade #0
- `tools/briefs/K_CLOSURE_AUTHORING_BRIEF.md` — А'.8 К-closure

### 7.2 — Authoritative artifacts (post-cascade-#3 closure)

- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.5.2 — К-L invariants canonical
- `docs/methodology/METHODOLOGY.md` v1.12 — Lessons + process invariants
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 LOCKED — Vulkan substrate spec (unchanged)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.11 LOCKED — Mod OS spec (unchanged)
- `docs/architecture/K_CLOSURE_REPORT.md` AUTHORED — К-series canonical closure
- `docs/architecture/K_EXTENSIONS_LEDGER.md` Live (§3.4 added) — cascade narratives
- `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` Live (#12 entry) — К-L14 metrics
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` Live (cascade #3 entry) — chronological timeline
- `docs/governance/REGISTER.yaml` register_version 2.5 — governance SoT

### 7.3 — Substrate empirical references (Phase 0 §2.0 sources)

- `src/DualFrontier.Runtime/Assets/PngDecoder.cs` — substrate decoder (V0.C.1)
- `src/DualFrontier.Runtime/Assets/PngImage.cs` — decoded image record
- `src/DualFrontier.Runtime/Assets/MODULE.md` — substrate Assets module overview
- `src/DualFrontier.Runtime/Sprite/SpriteRenderer.cs` — V0.C.2 batched renderer
- `src/DualFrontier.Runtime/Sprite/AtlasRegion.cs` — UV region с FromPixels factory
- `src/DualFrontier.Runtime/Sprite/SpriteTexture.cs` — VulkanImage+VulkanSampler wrapper
- `src/DualFrontier.Runtime/Sprite/MODULE.md` — substrate Sprite module overview
- `tests/DualFrontier.Runtime.SmokeTest/ProceduralAtlas.cs` — pattern source для LauncherProceduralAtlas
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` — Runtime API usage reference

### 7.4 — Asset references

- `assets/sprites/pawn.png` — 4-bit palette PNG (substrate REJECTS)
- `assets/kenney_micro-roguelike/Tilemap/colored_tilemap.png` — 8-bit palette PNG (REJECTS)
- `assets/kenney_micro-roguelike/Tilemap/colored_tilemap_packed.png` — 8-bit palette PNG (REJECTS)
- `assets/kenney_micro-roguelike/Tilesheet.txt` — atlas metadata (8×8 tiles, 16×10 grid)

---

## §8 — Appendix — Q-H lock summary

Per deliberation session 2026-05-23:

| Q | Decision | Status | Rationale |
|---|---|---|---|
| Q-H-0 | Path B Hybrid | LOCKED | Phase 0 + checkpoint discipline matches uncertainty |
| Q-H-1 | Pawn-3 active, 3 deferred | LOCKED | Minimum scope per Crystalka «нету ещё функционала» |
| Q-H-2 | Scene state = sprite registry only | LOCKED | Minimum scope; camera/HUD/layer deferred |
| Q-H-3 | Path γ-A (procedural-only) | LOCKED | Substrate untouched; К-L14 thesis preserved cleanly |
| Q-H-4 | Runtime API empirical signatures | LOCKED | Phase 0 §2.0 established empirical state |
| Q-H-5 | R-1 fail-annotate + R-2 fail-halt | LOCKED | Cascade #2 verification gap closure as Phase 0 entry |
| Q-H-6 | Test discipline = pawn-3 only | LOCKED | Match active scope; deferred arms covered by defensive throws |
| Q-H-7 | К-L14 #12 = clean additive evidence | LOCKED | Restored framing post-empirical correction |
| Q-H-8 | К-L17.1 deferred | LOCKED | Composition framework binding не materialized этой cascade |
| Q-H-9 | Filename K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md | LOCKED | Naming consistency с cascade #2 |
| Q-H-10 | METHODOLOGY v1.11 → v1.12 + #N13/#N14 | LOCKED | New Provisional candidates captured timely |
| Q-H-11 | DISSOLVED (PNG decoder discovery) | DISSOLVED | Empirical state known pre-authoring |
| Q-H-12 | DISSOLVED (sprite asset format fork) | DISSOLVED | Path γ-A subsumed |
| Q-H-13 | DISSOLVED (substrate extension authorization) | DISSOLVED | Not needed этой cascade |
| Q-H-14 | DISSOLVED (palette decoder fallback) | DISSOLVED | Not in scope |
| Q-H-15 | DISSOLVED (extension scope bounding) | DISSOLVED | Not in scope |
| Q-H-16 | DISSOLVED (substrate test discipline) | DISSOLVED | Not in scope |
| Q-H-17 | Mid-cascade checkpoint: ProceduralAtlas reuse strategy (Option C recommended) | CHECKPOINT | Path B Hybrid honest gap |

---

**End of brief**

**Authoring metadata**:
- Authored: 2026-05-23 by Claude Opus 4.7 (deliberation mode)
- Authored on behalf of: Crystalka (Volodymyr, solo dev)
- Authoring session: К-extensions cascade #3 deliberation session 2026-05-23
- Authoring filesystem: bash staging pattern (`/home/claude/staging/` → `/mnt/user-data/outputs/`)
- Project: Dual Frontier (Crystalka228/Dual-Frontier)
- Pre-authoring empirical grounding: Filesystem MCP reads of substrate sources + PNG header inspection (Lesson #N14/#N16 meta-application)

**Status at authoring**: AUTHORED — pending Crystalka RATIFICATION + Claude Code execution  
**Status transitions**: AUTHORED → RATIFIED (Crystalka) → EXECUTING → EXECUTED → CLOSED
