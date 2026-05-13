---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-UI_REVIEW_PRE_M75B2
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-UI_REVIEW_PRE_M75B2
---
# UI architecture review — pre-M7.5.B.2 audit

**Status:** Findings document (NOT a brief — no commits to execute).
**Purpose:** Crystalka asked for full review of UI layer и how it connects to simulation, before deciding на M7.5.B.2 path.
**Baseline:** `700cbc0` (TickScheduler race fix). 417/417 tests passing. F5 production-verified.

---

## TL;DR

UI layer **architecturally clean**, layer separation correct. **Three real issues** present:

1. ✅ **TICK display** — was orphaned, fixed in housekeeping commits `1b16e9e`/`21921887`/`3d800d2`. Working at F5.
2. ⏭ **Needs decay direction wrong** — internal field decays toward 0 instead of growing toward 1. Self-consistent inside simulation but no recovery loop, so colony reaches false "perfect satisfaction" steady state. **Phase 5 territory** (already in Backlog).
3. ⏭ **3 stub UI files** (`AlertPanel.cs`, `ManaBar.cs`, `PawnInspector.cs`, plus `BuildMenu.cs` previously identified) — Phase 3 placeholder classes, never derived from `Godot.Control`. **Phase 5 territory.**

**For M7.5.B.2:** UI architecture supports either code-only `ModMenuPanel` (parallel to ColonyPanel) OR scene-based authoring (your insight applied). Both viable; recommendation below.

---

## 1. Architecture — data flow trace

### 1.1 Bridge layer (one-way, thread-safe)

```
[ECS Systems run on simulation thread]
    │
    │  Services.Pawns.Publish(PawnStateChangedEvent { ... })
    ▼
[GameServices event bus — in-memory pub-sub]
    │
    │  Subscribed in GameBootstrap.CreateLoop:
    │    services.Pawns.Subscribe<PawnStateChangedEvent>(e =>
    │        bridge.Enqueue(new PawnStateCommand(...)));
    ▼
[PresentationBridge (ConcurrentQueue<IRenderCommand>)]
    │
    │  GameRoot._Process (Godot main thread, every frame ~60 Hz):
    │    _bridge.DrainCommands(_dispatcher.Dispatch);
    ▼
[RenderCommandDispatcher.Dispatch (switch on command type)]
    │
    ├─ PawnSpawnedCommand → PawnLayer.SpawnPawn
    ├─ PawnMovedCommand   → PawnLayer.MovePawn
    ├─ PawnDiedCommand    → PawnLayer.RemovePawn
    ├─ PawnStateCommand   → GameHUD.UpdatePawn → ColonyPanel + PawnDetail
    └─ TickAdvancedCommand → GameHUD.SetTick → ColonyPanel
```

### 1.2 Where bridge subscriptions live

**`src/DualFrontier.Application/Loop/GameBootstrap.CreateLoop`** — lines ~80-95. Direct subscriptions, no separate `PresentationAdapter` class. This is intentional minimalism — only 4 events need translation and they're all simple maps.

```csharp
services.Pawns.Subscribe<PawnSpawnedEvent>(e =>
    bridge.Enqueue(new PawnSpawnedCommand(e.PawnId, e.X, e.Y)));
services.Pawns.Subscribe<PawnMovedEvent>(e =>
    bridge.Enqueue(new PawnMovedCommand(e.PawnId, e.X, e.Y)));
services.Combat.Subscribe<DeathEvent>(e =>
    bridge.Enqueue(new PawnDiedCommand(e.Who)));
services.Pawns.Subscribe<PawnStateChangedEvent>(e =>
    bridge.Enqueue(new PawnStateCommand(...)));
```

**Twoя observation about UI being "decoupled" подтверждена architecturally:**
- UI widgets receive method calls (`UpdatePawn`, `SetTick`) — they never query simulation state
- Bridge is one-way (TechArch §11.9 enforced)
- ECS systems publish to abstract event bus, not к UI directly
- Adding new commands requires только: command type + bootstrap subscription + dispatcher switch arm + UI method call

Это означает **М7.5.B.2 mod menu может быть authored полностью в Godot scene editor**, с C# script делающий только controller binding. UI architecture не диктует code-only authoring.

### 1.3 Strict layer ownership

| Layer | Knows about | Doesn't know about |
|---|---|---|
| Systems (ECS) | World, Components, Events bus | Bridge, Godot, UI widgets |
| Application/Bridge | Events bus, Bridge queue, Render commands | Godot nodes, UI rendering |
| Application/Loop (Bootstrap) | Both above + TickScheduler + ModMenuController | Godot nodes |
| Presentation/Rendering (Dispatcher) | Render commands, Godot nodes (PawnLayer, HUD) | ECS, simulation thread |
| Presentation/UI (widgets) | Godot Control APIs, Render commands as input | ECS, simulation thread, bridge |
| Presentation/Nodes (GameRoot) | All of above | — |

Boundary clean. Tests (M7.5.B.1) verify production wiring without violating any layer.

---

## 2. UI components inventory

### 2.1 Working components (5 files)

| File | Type | Status |
|---|---|---|
| `Nodes/PawnLayer.cs` | `Node2D` | ✅ Working — pawn visuals dictionary |
| `Nodes/PawnVisual.cs` | `Node2D` | ✅ Working — single pawn sprite |
| `Nodes/TileMapRenderer.cs` | `Node2D` | ✅ Working — 50×50 grass/rock tiles |
| `Nodes/ProjectileVisual.cs` | (not read; presumably similar shape) | Phase 5 placeholder |
| `Nodes/GameRoot.cs` | `Node2D` | ✅ Working — production scene root |

### 2.2 Working UI widgets (2 files)

| File | Type | Status |
|---|---|---|
| `UI/GameHUD.cs` | `CanvasLayer` (Layer=10) | ✅ Working — fans `PawnStateCommand` to ColonyPanel + PawnDetail; TICK to ColonyPanel |
| `UI/ColonyPanel.cs` | `Panel` | ✅ Working — title, ornament, pawn rows, tick label |
| `UI/PawnDetail.cs` | `Panel` | ✅ Working — header, mood, 4 needs bars, job pill, skills, ornament |

### 2.3 Stub files (4 stubs — all Phase 3 leftover)

| File | Status | Body |
|---|---|---|
| `UI/BuildMenu.cs` | `// TODO Phase 3 — derive from Godot.Control` | empty class |
| `UI/AlertPanel.cs` | `// TODO Phase 3 — derive from Godot.Control` | empty class |
| `UI/ManaBar.cs` | `// TODO Phase 3 — derive from Godot.Control` | empty class |
| `UI/PawnInspector.cs` | `// TODO Phase 3 — derive from Godot.Control` | empty class |

These are **placeholder classes left from Phase 3 planning**. They don't render anything, don't get instantiated, don't do harm. Should be either:
- Implemented в Phase 5 (proper UI work)
- Deleted as scaffolding artifacts (cleanup commit)

**They are not blocking M7.5.B.2.** ModMenuPanel is a separate widget.

### 2.4 Three render commands defined but never dispatched

In `Application/Bridge/Commands/`:
- `ProjectileSpawnedCommand.cs` — exists, no `case` in dispatcher
- `SpellCastCommand.cs` — exists, no `case` in dispatcher
- `UIUpdateCommand.cs` — exists, no `case` in dispatcher

Same Phase 5 territory as стubs. Scaffolding for combat / magic / generic UI updates.

---

## 3. Why needs grow display values (your observation)

Полное объяснение, поскольку это критическая mental model для Phase 5.

### 3.1 The actual data flow

**Step 1 — NeedsSystem** (`src/DualFrontier.Systems/Pawn/NeedsSystem.cs`):

```csharp
[TickRate(TickRates.SLOW)]
public sealed class NeedsSystem : SystemBase {
    private const float HungerDecayPerTick  = 0.002f;  // ←
    
    public override void Update(float delta) {
        foreach (var entity in Query<NeedsComponent>()) {
            var needs = GetComponent<NeedsComponent>(entity);
            needs.Hunger  = Math.Clamp(needs.Hunger  - HungerDecayPerTick,  0f, 1f);
            //                                       ^ DECREASES every tick
```

Initial spawn: `Hunger = 0.1`. Per SLOW tick: subtract 0.002. Floor at 0. Reaches 0 in ~50 SLOW ticks.

**Step 2 — MoodSystem** computes mood from raw `needs`:

```csharp
// src/DualFrontier.Systems/Pawn/MoodSystem.cs line ~24
float mood = 1f
    - (needs.Hunger + needs.Thirst + needs.Rest + needs.Comfort)
    / 4f;
```

Comment: `// Mood formula: average of inverted needs (0 = bad, 1 = good)`.
Internal semantics: **high `needs.X` = high deficit = bad mood**.
With decay-toward-0: average shrinks → mood rises toward 1.0.

**Step 3 — PawnStateReporterSystem** publishes display values:

```csharp
// src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs
Hunger  = 1f - needs.Hunger,   // ← inverts again
Thirst  = 1f - needs.Thirst,
Rest    = 1f - needs.Rest,
Comfort = 1f - needs.Comfort,
```

Internal `0.1 hunger` → published `0.9` → display `90%` (green bar 90% full).
Internal `0.0 hunger` (decayed floor) → published `1.0` → display `100%` (full green).

**Step 4 — UI bar** (`PawnDetail.NeedRow.Set`):

```csharp
public void Set(float v) {
    v = Math.Clamp(v, 0f, 1f);
    _pct.Text = $"{(int)MathF.Round(v * 100f)}%";
    _fill.AnchorRight = v;          // bar fill width = v
    _fill.Color = StatusColor(v);   // green if v > 0.65, neutral 0.45-0.65, ...
}
```

Higher value → wider bar + green color → user perceives "bar full = good".

### 3.2 Why this is internally consistent но gameplay-wrong

**Internal semantics (NeedsSystem, MoodSystem)**: ✅ correct
- High `needs.X` = much hunger / thirst = bad
- Low `needs.X` = satisfied = good
- Mood formula `1 - avg(needs)` correctly maps deficit → low mood
- `NeedsCriticalEvent` fires when `value >= CriticalThreshold` (high deficit)
- `JobSystem.OnNeedsCritical` subscribes to add pawn к `_urgentPawns` for priority job assignment

**Decay direction**: ❌ wrong
- Code: `needs.Hunger - HungerDecayPerTick` (subtract)
- Should be: `needs.Hunger + HungerDecayPerTick` (add)
- Real semantics: hunger should grow over time; eating decreases it
- Current behavior: pawns get LESS hungry без food, never reach CriticalThreshold, JobSystem never receives NeedsCriticalEvent, jobs stay Idle

**Display layer (Reporter inversion + bar rendering)**: ✅ correct *for inverted-direction bug*
- Reporter inverts because internal "high = bad"
- Display "Hunger 100% green" = "100% satiated, no hunger deficit"
- Color thresholds (`v < 0.25 = critical`) align с display semantic

**The naming creates user confusion**: ⚠ cosmetic
- Bar label: "Hunger 100%"
- User reading: "100% hungry?" → no, "100% satisfied"
- Either rename internal field `Hunger → Satiety` (and decay direction stays as-is)
- Or rename display label `Hunger → Satiation`
- Or fix decay direction (then internal "high = much hunger" becomes гameплay-correct, display semantic flips appropriately)

### 3.3 Why fixing this is Phase 5, not now

If you flip decay sign tomorrow:
1. `needs.Hunger` grows from 0.1 → 0.7 → 1.0 over time
2. At 0.7 (CriticalThreshold), `NeedsCriticalEvent` fires
3. `JobSystem._urgentPawns.Add(pawnId)` 
4. JobSystem looks for an Eat job to assign...
5. **No food entities exist в world**
6. No Eat/Drink/Sleep job types implemented в JobSystem (it just has `JobKind.Idle`)
7. Pawn stays urgent indefinitely с rising hunger
8. MoodSystem: `1 - avg(needs)` → mood drops to 0
9. `MoodBreakEvent` fires (MoodSystem)
10. No subscriber handles MoodBreak → pawn just shows "On the brink"

So decay-direction fix alone produces **broken-pawn steady state** instead of **fake-satisfied steady state**. Both wrong, both Phase 5 territory.

**Phase 5 work needed (5 layers):**
1. Flip decay sign в NeedsSystem
2. Implement food/water/bed entities + spawning logic
3. Extend JobKind enum + JobSystem to assign Eat/Drink/Sleep jobs to urgent pawns
4. Job execution flow: pawn paths to food → eats → `needs.Hunger -= eatAmount`
5. MoodBreak event handlers (run away, fight others, breakdown anim, etc.)

Backlog already has entry "NeedsSystem decay direction — Phase 5". Confirmed correct categorization.

---

## 4. M7.5.B.2 readiness assessment

### 4.1 What's готово (no blockers)

✅ `ModMenuController` живёт в `DualFrontier.Application/Modding/`, full unit-tested (M7.5.A: 30 tests).
✅ Production wiring: `GameRoot._modMenuController` field populated в `_Ready` from `GameContext.Controller` (M7.5.B.1: 7 smoke tests).
✅ Pipeline starts paused (M7.1 default), controller available для BeginEditing immediately.
✅ `InternalsVisibleTo` chain wires `DualFrontier.Presentation` → `DualFrontier.Application` (existing). No accessibility issues.
✅ UI patterns established: `Palette` static class с grimdark colors, `PawnRow` button pattern, `NeedRow` bar pattern, `MakeLabel` factory, `MakePanelStyle` factory.
✅ Render dispatcher pattern для adding new commands (если надо для menu — likely не нужно since menu пишет к controller, не reads из simulation).

### 4.2 What needs decisions

**Decision A — Authoring approach.**

Option (1) **Code-only `ModMenuPanel : Control`** — parallel to ColonyPanel/PawnDetail. All layout via code в `_Ready()`.

Option (2) **Scene-based** (your insight) — `ModMenuPanel.tscn` authored in Godot editor. Script attached to scene root does only state-binding + signal-wiring.

Option (3) **Hybrid** — minimal scaffold scene + iterate.

Existing UI architecture **fully supports** all three. Choice is taste/workflow, не technical.

**Decision B — Trigger mechanism.**

Where does user open the menu? Three options:
- **(α) Hotkey** (e.g. F10) — `InputRouter` already exists, can subscribe key
- **(β) Button в HUD** — small "MODS" button в ColonyPanel или GameHUD
- **(γ) Pause menu integration** — Godot scene hierarchy, future game

Recommend **(α) hotkey F10** для now — minimal UI footprint, не trogal existing layout, easy to test. Future move to (γ) после Phase 5 main menu.

**Decision C — Visual scope of menu.**

Minimum viable:
- List of mods (active + discovered)
- Toggle button per row
- Apply / Cancel buttons at bottom
- Hot-reload disabled tooltip per §9.6

Nice-to-have (defer):
- Mod metadata expanded view (description, version, author, dependencies)
- Search / filter
- Drag-reorder для load order
- Validation error display

### 4.3 What's NOT a blocker

❌ Stub UI files (`AlertPanel`, `ManaBar`, `PawnInspector`, `BuildMenu`) — orthogonal к menu work
❌ NeedsSystem decay direction — orthogonal (Phase 5)
❌ Display semantic naming — orthogonal (Phase 5)
❌ Three undispatched render commands — orthogonal (Phase 5)
❌ Job system stuck at Idle — orthogonal (Phase 5)
❌ TickScheduler race — already fixed

---

## 5. Recommendations

### 5.1 For M7.5.B.2 — proceed without further housekeeping

UI architecture не имеет blockers. Phase 5 backlog items don't affect mod menu functionality. M7.5.B.2 пишется next.

**Authoring path recommendation: Option (2) scene-based.**

Reasoning:
- Twoё insight valid: UI is decoupled, scene authoring leverages Godot strengths
- Menu inherently two-way (queries + actions) → natural fit для signal-based scene
- Taste decisions stay в твоих руках (Warhammer aesthetic, layout, copy)
- AI scope reduces to falsifiable controller binding code — clean
- Existing code-only widgets (ColonyPanel/PawnDetail) stay as Phase 4 reference; не required к match

**Trigger recommendation: hotkey F10.**

Adds 1 line to `InputRouter` plus 1 method on GameRoot. Минимальный UI footprint.

### 5.2 For Phase 5 (whenever you get there)

Ordered by dependencies:

1. **Job loop foundations** (multiple weeks).
   - Flip NeedsSystem decay sign
   - Spawn food/water/bed entities в world
   - Extend JobKind enum: Eat, Drink, Sleep, Wander
   - Wire JobSystem to assign jobs to urgent pawns
   - Pathing + interaction handlers

2. **Display semantic cleanup** (1 commit).
   - Rename `NeedsComponent.Hunger → Satiety`, `Thirst → Hydration`, `Rest → Energy`
   - Remove inversion в PawnStateReporterSystem (no longer needed; internal name matches display)
   - Update MoodSystem formula: `mood = avg(needs)` (no inversion)
   - Saves ripple — handle с migration

3. **Stub file cleanup** (1 commit).
   - Implement или delete `BuildMenu`, `AlertPanel`, `ManaBar`, `PawnInspector`
   - Same for `ProjectileSpawnedCommand`, `SpellCastCommand`, `UIUpdateCommand`

4. **Combat / magic systems** (later).
   - ProjectileSystem, SpellSystem, etc. — actually use the orphaned commands.

### 5.3 Backlog entry update

ROADMAP "Backlog" section currently has 3 entries (NeedsSystem decay, NeedsComponent rename, BuildMenu stub). Recommend extending to 6:

```
**Phase 5 backlog (UI layer):**
- NeedsSystem decay direction (existing entry)
- NeedsComponent field rename (existing entry)  
- BuildMenu.cs stub (existing entry)
- AlertPanel.cs stub — same shape as BuildMenu
- ManaBar.cs stub — same shape; Phase 5 magic system
- PawnInspector.cs stub — same shape; Phase 5 expanded pawn view

**Phase 5 backlog (bridge commands undispatched):**
- ProjectileSpawnedCommand — Phase 5 combat
- SpellCastCommand — Phase 5 magic
- UIUpdateCommand — generic UI updates, Phase 5 alert/notification system
```

This makes future Phase 5 work scope-visible. Не requires commit now — can be merged into M7-closure ROADMAP update or Phase 5 kickoff commit.

---

## 6. Verdict

UI **architecture is sound**. No structural blockers for M7.5.B.2. Phase 5 work needed for gameplay completion is correctly partitioned and Backlog'd.

Twoё intuition **архитектурно правильное**: UI layer и simulation layer are intentionally decoupled. M7.5.B.2 ModMenuPanel может быть authored как Godot scene с thin C# script, не required к follow code-only ColonyPanel pattern.

**Recommended next step:** написать M7.5.B.2 brief assuming scene-based authoring + F10 hotkey trigger. Delegated work split:
- You: author `ModMenuPanel.tscn` in Godot editor (Warhammer styling, Apply/Cancel buttons, mod list container, named child nodes)
- AI brief: write `ModMenuPanel.cs` script that pulls state from `_modMenuController.GetEditableState()`, renders rows, wires button signals to `Toggle/Commit/Cancel`, plus `InputRouter` F10 wiring

Brief will define the scene contract (required child node names + types) up front so you can author scene independently of script work.
