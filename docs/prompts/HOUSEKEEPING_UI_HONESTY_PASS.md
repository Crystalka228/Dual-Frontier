---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-HOUSEKEEPING_UI_HONESTY_PASS
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-HOUSEKEEPING_UI_HONESTY_PASS
---
# UI ↔ Code wiring audit — comprehensive findings + fix scope

## Audit findings

Rigorous re-read of every UI file, every bridge subscription, every component referenced by display, plus the source of every label/value shown in the HUD. Not just "does it compile and render" but "does the displayed value match a real simulation source".

### Architecture — VERIFIED CORRECT

| Component | Status |
|---|---|
| `PresentationBridge` ConcurrentQueue + drain pattern | ✅ Sound |
| Layer separation (UI never queries simulation state) | ✅ Enforced |
| `GameBootstrap` event subscriptions | ✅ All fire correctly |
| `RenderCommandDispatcher` switch routing | ✅ All 5 commands route to widgets |
| `GameRoot._Process` drain pattern | ✅ Per-frame |
| `TickAdvancedCommand` wiring (housekeeping) | ✅ Production-verified |
| Scene tree (`main.tscn`) hierarchy | ✅ 7 scripts wired correctly |

### Display data sources — VERIFIED REAL

These display values trace to actual simulation state:

| Display | Source | Verdict |
|---|---|---|
| Pawn position (sprite location) | `PawnMovedEvent` from `MovementSystem` (writes `PositionComponent`) | ✅ Real |
| Pawn count (3 sprites) | Initial spawn в `GameBootstrap.SpawnInitialPawns` | ✅ Real |
| Mood value (94, 100, etc.) | `MindComponent.Mood` updated by `MoodSystem` formula | ✅ Real |
| Mood label ("Inspired", etc.) | Threshold map in `PawnDetail.MoodLabel` from real Mood value | ✅ Real |
| Need bar values (Hunger 90%, etc.) | `1f - needs.X` from `NeedsComponent` | ✅ Real (decay direction wrong, but value flows correctly) |
| Job label ("Idle") | `TranslateJob(JobComponent.Current)` | ✅ Real |
| Job urgent flag | `Current == JobKind.Eat \|\| Current == JobKind.Sleep` | ✅ Real |
| TICK counter | `TickScheduler.CurrentTick` | ✅ Real |

### Display data sources — FAKE / PLACEHOLDER (UI lies)

These display values do NOT trace to any real component data:

| Display | What user sees | What code does | Severity |
|---|---|---|---|
| Pawn name ("Sister Maria") | Real-looking character name | Hardcoded array indexed by `pawn.Index % 6` in `PawnStateReporterSystem.Names` | High — Warhammer flavor + no `IdentityComponent` exists |
| Pawn role ("sergeant", "magus") | Looks like character class data | `PawnDetail.MakeRole` hashes `EntityId.Index` into 5-element array `{ "inquisitor", "sergeant", "tech-priest", "magus", "medic" }` | **Critical — no role concept exists anywhere; pure UI fabrication** |
| Skill bars (Construction 6, Combat 8, Crafting 4) | Looks like real character skills with progression pips | `PawnDetail.DemoSkills` hashes `EntityId.Index` through formulas. **`SkillsComponent` exists but is COMPLETELY IGNORED** by display | **Critical — UI shows fake skill data while real component data exists** |

### Movement vs job inconsistency — DESIGN-LEVEL ISSUE

`MovementSystem` picks **random wander targets** when `MovementComponent.Path` is empty, regardless of `JobComponent.Current`:

```csharp
// MovementSystem.cs line ~62
if (move.Path.Count == 0) {
    var target = new GridVector(_rng.Next(0, MapWidth), _rng.Next(0, MapHeight));
    if (_pathfinding.TryFindPath(pos.Position, target, out var path) ...) {
        move.Path = new List<GridVector>(path);
    }
}
```

So pawns visibly walk around the map (as in screenshots) while UI displays "Idle". Display is technically truthful (`JobKind.Idle`) but conceptually wrong — pawns are wandering, not idle. Phase 5 territory but worth flagging.

### Stub UI files — DEAD CODE (5 files)

| File | Body | Comment |
|---|---|---|
| `UI/BuildMenu.cs` | Empty class | `// TODO: Phase 3 — derive from Godot.Control once GodotSharp is wired in.` |
| `UI/AlertPanel.cs` | Empty class | Same comment |
| `UI/ManaBar.cs` | Empty class | Same comment |
| `UI/PawnInspector.cs` | Empty class | Same comment |
| `Nodes/ProjectileVisual.cs` | Empty class | Same comment |

These are Phase 3 planning placeholders. None instantiated anywhere. None referenced in `main.tscn`. None used in code. They compile but render nothing. Phase 5 will re-create them as proper `Godot.Control` / `Godot.Node2D` subclasses; current shells contribute nothing.

### Undispatched render commands — DEAD CODE (3 files)

| File | Status |
|---|---|
| `Bridge/Commands/ProjectileSpawnedCommand.cs` | Defined, no `case` in dispatcher, no event subscriber publishes it |
| `Bridge/Commands/SpellCastCommand.cs` | Same — defined, never published, never dispatched |
| `Bridge/Commands/UIUpdateCommand.cs` | Same — defined, never published, never dispatched |

Phase 5 combat/magic scaffolding. Will be redesigned when real systems land. Current shells are noise.

### Warhammer-flavored placeholder strings — non-required theming

User explicitly clarified: "Стиль тут не обязателен — Warhammer styling он как заглушка был изначально". So all Warhammer-flavored strings are placeholder, not design decisions:

| Source | Content |
|---|---|
| `PawnStateReporterSystem.Names` array | "Brother Cassian", "Sister Maria", "Magus Ferro", "Vexillus Korvin", "Inquisitor Vex", "Acolyte Veneris" |
| `PawnDetail.MakeRole` array | "inquisitor", "sergeant", "tech-priest", "magus", "medic" |

Both should go — names need to be honest placeholders, roles need to be removed entirely (no concept exists yet).

### Phase 5 territory (do NOT fix in this pass)

- NeedsSystem decay direction (in Backlog)
- IdentityComponent + name from real data (Phase 5)
- SkillsComponent display wiring (Phase 5)
- Job loop (food/water/bed entities, Eat/Drink/Sleep job assignment, Phase 5)
- MoodBreakEvent handlers (Phase 5)
- HealthComponent / FactionComponent / RaceComponent display (Phase 5)
- ManaComponent / EtherComponent display (Phase 5 magic)
- Combat command dispatch (Phase 5)
- MovementSystem wander vs job-following inconsistency (Phase 5)

---

## Proposed fix scope

Three-commit "UI honesty pass". Removes UI-side lies; deletes confirmed dead code; documents remaining Phase 5 work. Does NOT touch simulation systems.

### Commit 1 — UI honesty in `PawnDetail`

**Goal:** stop showing fake data. UI displays only what real components carry.

Changes to `src/DualFrontier.Presentation/UI/PawnDetail.cs`:

- **Remove role label entirely.** No role concept exists in any component. The `_roleLabel` field, `MakeRole(EntityId)` helper, and the `_roleLabel` line in `BuildHeader` go away. Header becomes: avatar + name only.
- **Remove SKILLS section entirely.** `BuildSkills`, `RenderSkills`, `DemoSkills`, `_skillsBox` field, the `BuildSkills()` call в `_Ready`, and the `RenderSkills(id)` call в `Render` all go. `SkillRow` partial class also goes (used only here).
- Keep ornament line at bottom (visual separator, no data).

Final PawnDetail sections after removal: header (avatar + name), MOOD (real), NEEDS (real, 4 bars), JOB (real), ornament. No fake data.

Changes to `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs`:

- **Replace Warhammer-flavored Names array with neutral placeholder.** Replace `Names` array + `ResolveName` helper with a generic format: `Name = $"Pawn {pawn.Index}"`. Truthful — pawns don't have names yet, this is clearly a placeholder format.
- Add XML doc comment on the line: `// Phase 5: replace with IdentityComponent.Name lookup once that component exists.`
- Update PawnStateReporterSystem class XML doc to note name is currently a placeholder format.

### Commit 2 — Delete dead code

**Goal:** delete files that exist but do nothing. Phase 5 will re-create with real implementations.

Delete:
- `src/DualFrontier.Presentation/UI/BuildMenu.cs`
- `src/DualFrontier.Presentation/UI/AlertPanel.cs`
- `src/DualFrontier.Presentation/UI/ManaBar.cs`
- `src/DualFrontier.Presentation/UI/PawnInspector.cs`
- `src/DualFrontier.Presentation/Nodes/ProjectileVisual.cs`
- `src/DualFrontier.Application/Bridge/Commands/ProjectileSpawnedCommand.cs`
- `src/DualFrontier.Application/Bridge/Commands/SpellCastCommand.cs`
- `src/DualFrontier.Application/Bridge/Commands/UIUpdateCommand.cs`

Build will succeed (none of these are referenced anywhere). 8 files removed, ~50 lines of dead code gone. Phase 5 кickoff will re-create with real bodies.

### Commit 3 — ROADMAP update

**Goal:** document the cleanup + extend Backlog with comprehensive Phase 5 UI work list.

Edits to `docs/ROADMAP.md`:

- Header status line update with date and "UI honesty pass" mention.
- Engine snapshot tests count unchanged (no test changes).
- Backlog section restructured into subsections:

```
## Backlog

### Phase 5 — gameplay completeness
- **NeedsSystem decay direction** — current code decays toward 0 ("becoming satiated"); should grow toward 1 ("becoming hungry"). Requires Phase 5 eat/drink/sleep job loop to land first so pawns have recovery mechanism.
- **Job loop** — JobSystem currently never assigns Eat/Drink/Sleep; food/water/bed entities don't exist; pawns wander randomly via MovementSystem regardless of JobComponent. Needs full Phase 5 implementation.
- **MoodBreakEvent handlers** — published by MoodSystem, no subscribers; pawns reach low mood with no consequence.

### Phase 5 — UI ↔ data wiring
- **IdentityComponent** — give pawns real names, replace `Pawn N` placeholder format in PawnStateReporterSystem with real component lookup.
- **Role / class concept** — if introduced (sergeant, magus, etc.), publish via PawnStateChangedEvent and re-add role label to PawnDetail header.
- **SkillsComponent display wiring** — extend PawnStateChangedEvent to carry skill levels by SkillKind, re-add SKILLS section to PawnDetail with real data. Existing SkillsComponent already has Levels and Experience dictionaries.
- **HealthComponent display** — health bar in PawnDetail or HUD.
- **FactionComponent display** — faction tag/color in roster.
- **MovementSystem job-aware wandering** — currently random regardless of JobComponent; should respect JobKind.Idle (hold position) vs follow job target.
- **MovementSystem JobLabel honesty** — if pawn is wandering randomly, JobLabel should say "Wandering", not "Idle".

### Phase 5 — combat / magic scaffolding
- **ProjectileSpawnedCommand** — when projectile system lands, re-create with real fire-and-forget rendering.
- **SpellCastCommand** — when spell system lands, re-create with VFX trigger.
- **UIUpdateCommand** — generic notification surface; reintroduce when alert/notification system lands.
- **Stub UI re-creation** — BuildMenu, AlertPanel, ManaBar, PawnInspector deleted in housekeeping; Phase 5 introduces them as real Godot.Control subclasses with real data sources.

### Resolved
- TickScheduler.ShouldRun race (housekeeping commit `e0b0ecf` etc.) — see existing entry.
```

---

## Out of scope (housekeeping discipline)

- ANY change to `DualFrontier.Core` or `DualFrontier.Contracts`. Verified by `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returning empty.
- Simulation systems (NeedsSystem, MoodSystem, MovementSystem, JobSystem) — only `PawnStateReporterSystem` modified, and only the placeholder name source.
- New components — IdentityComponent etc. are Phase 5 work.
- New tests — this is pure deletion + UI cleanup; existing test suite must continue to pass at 417/417.
- Palette colors / dark theme styling — kept as-is. The color values are functional (semantic: critical=red, good=green, neutral=grey) regardless of theme. User clarification was about Warhammer-flavored strings, not palette per se.

## Approved decisions

1. **Pawn name format = `$"Pawn {pawn.Index}"`.** Neutral placeholder. Phase 5 replaces with IdentityComponent lookup.
2. **PawnDetail role label = REMOVED.** No role concept exists anywhere. Re-added in Phase 5 if/when roles introduced.
3. **PawnDetail SKILLS section = REMOVED.** Re-added in Phase 5 with real SkillsComponent data wiring through PawnStateChangedEvent.
4. **5 UI stub files + 3 unused render commands = DELETED.** Phase 5 re-creates with real bodies.
5. **Palette / dark theme = KEPT.** Functional color choices work for any setting.
6. **No test changes.** 417/417 must hold (existing tests don't reference any deleted file).
7. **Single housekeeping pass.** Three commits per METHODOLOGY §7.3 invariant.

## Required reading

1. Audit findings above (this document's "Audit findings" section IS the brief context — agent does not need to re-audit).
2. `src/DualFrontier.Presentation/UI/PawnDetail.cs` — full file (the modify target).
3. `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs` — full file (the modify target).
4. `docs/ROADMAP.md` — locate Backlog section established by housekeeping commit `3d800d2`. New subsections will replace the flat list with categorized Phase 5 entries.
5. `docs/methodology/METHODOLOGY.md` — §7.3 three-commit invariant (deletion commits count as their own atomic step).
6. The 8 files slated for deletion (verify each is truly unused before deletion, especially via `grep -r` for any stray reference):
   - `src/DualFrontier.Presentation/UI/BuildMenu.cs`
   - `src/DualFrontier.Presentation/UI/AlertPanel.cs`
   - `src/DualFrontier.Presentation/UI/ManaBar.cs`
   - `src/DualFrontier.Presentation/UI/PawnInspector.cs`
   - `src/DualFrontier.Presentation/Nodes/ProjectileVisual.cs`
   - `src/DualFrontier.Application/Bridge/Commands/ProjectileSpawnedCommand.cs`
   - `src/DualFrontier.Application/Bridge/Commands/SpellCastCommand.cs`
   - `src/DualFrontier.Application/Bridge/Commands/UIUpdateCommand.cs`

## Implementation

### Commit 1: PawnDetail honesty + name format

`src/DualFrontier.Presentation/UI/PawnDetail.cs`:

1. Remove field declarations:
   ```csharp
   private Label _roleLabel = null!;        // remove
   private VBoxContainer _skillsBox = null!; // remove
   ```

2. In `BuildHeader`, remove the `_roleLabel` setup block:
   ```csharp
   // remove these lines
   _roleLabel = ColonyPanel.MakeLabel("inquisitor", 11, Palette.Muted);
   titleBox.AddChild(_roleLabel);
   ```

3. In `_Ready`, remove the `BuildSkills();` call.

4. Delete the entire `BuildSkills` method.

5. In `Render`, remove the `RenderSkills(id);` call.

6. Delete the entire `RenderSkills` method.

7. Delete the entire `MakeRole` method.

8. Delete the entire `DemoSkills` method.

9. In `Render`, remove the `_roleLabel.Text = MakeRole(id);` line.

10. Delete the entire `SkillRow` partial class at the bottom of the file.

`src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs`:

1. Delete the `Names` static array.

2. Delete the `ResolveName` static method.

3. In the `foreach` loop, replace `Name = ResolveName(pawn.Index)` with:
   ```csharp
   // Phase 5: replace with IdentityComponent.Name lookup once that component exists.
   Name = $"Pawn {pawn.Index}",
   ```

4. Update the class XML doc to note the placeholder name format.

### Commit 2: Delete dead code (8 files)

Use `git rm` for each:
```
git rm src/DualFrontier.Presentation/UI/BuildMenu.cs
git rm src/DualFrontier.Presentation/UI/AlertPanel.cs
git rm src/DualFrontier.Presentation/UI/ManaBar.cs
git rm src/DualFrontier.Presentation/UI/PawnInspector.cs
git rm src/DualFrontier.Presentation/Nodes/ProjectileVisual.cs
git rm src/DualFrontier.Application/Bridge/Commands/ProjectileSpawnedCommand.cs
git rm src/DualFrontier.Application/Bridge/Commands/SpellCastCommand.cs
git rm src/DualFrontier.Application/Bridge/Commands/UIUpdateCommand.cs
```

Before deletion, run `grep -r` for each filename (without extension) across `src/`, `tests/`, and `*.tscn` files to confirm zero references. Empty grep result is acceptance criterion.

### Commit 3: ROADMAP backlog restructure

Replace the flat backlog section в `docs/ROADMAP.md` with the categorized subsections shown above. Update header status line. Engine snapshot count unchanged.

---

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors after each commit.
2. `dotnet test` — **417/417 passing** (no test changes; existing tests don't reference deleted files; verify post-each-commit).
3. After commit 1: PawnDetail header shows only avatar + name (no role label). PawnDetail body has no SKILLS section. PawnStateChangedEvent carries `Name = $"Pawn {Index}"` format.
4. After commit 2: 8 files removed; `grep -r` for each deleted filename returns zero matches across `src/`, `tests/`, and `*.tscn`.
5. After commit 3: ROADMAP Backlog section has three categorized subsections (Phase 5 gameplay, Phase 5 UI ↔ data, Phase 5 combat/magic) plus "Resolved" subsection. Each entry concrete and actionable.
6. M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty.
7. `dotnet sln list` count unchanged (no project changes).
8. **Manual F5 verification documented in commit 1 message** — agent cannot launch Godot from terminal; user verifies post-commit-1 that PawnDetail shows avatar + name only (no role, no skills section), names display as "Pawn 1" through "Pawn 6" instead of Warhammer flavor.

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `refactor(presentation): UI honesty — remove fake role and skill display, neutral pawn names`

- Modify `src/DualFrontier.Presentation/UI/PawnDetail.cs` per §Commit 1 changes 1-10 above.
- Modify `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs` per §Commit 1 changes 1-4 above.
- Verify build clean + 417/417 tests still pass.
- Commit message body notes: "PawnDetail no longer fabricates role/skill data; what's shown matches what's in real components. Name format moved to neutral placeholder until IdentityComponent lands in Phase 5. F5 verification deferred to user — agent cannot launch Godot."

**2.** `chore(cleanup): delete unused Phase 3 stubs and undispatched render commands`

- `git rm` each of the 8 files per §Commit 2.
- Verify zero references remain via `grep -r` for each filename across `src/`, `tests/`, `*.tscn`.
- Verify build clean + 417/417 tests still pass.
- Commit message lists all 8 deleted files with one-line justification each.

**3.** `docs(roadmap): UI honesty pass closure + categorized Phase 5 backlog`

- Replace Backlog section per §Commit 3 above.
- Header status line: `*Updated: YYYY-MM-DD (housekeeping — UI honesty pass + dead code cleanup; M7.5.B.2 + M7-closure pending; Phase 5 backlog categorized).*`
- Verify ROADMAP renders cleanly.

**Special verification preamble:**

After commit 1: `dotnet build && dotnet test` — must pass. No `[Fact]` should fail. Manual F5 sign-off deferred to user.

After commit 2: For each of the 8 deleted files, run `grep -rn "<filename-without-extension>" src/ tests/ src/DualFrontier.Presentation/Scenes/`. Expected: zero matches per file (only the deletion in git diff itself). If any reference found, STOP — file is not actually dead, do not delete.

After commit 3: ROADMAP renders; categorized subsections present; "Resolved" subsection retained from prior housekeeping.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice.

**Hypothesis-falsification clause:**

Housekeeping commit, не M-cycle phase. M-cycle datapoint sequence (10 consecutive zeros post-M4) unaffected. No spec contradictions plausible (this is UI cleanup; MOD_OS_ARCHITECTURE doesn't speak to colonist roster display).

Plausible non-spec frictions:
- (a) `_roleLabel` or `_skillsBox` field referenced from a Godot node `.tscn` file via `unique_name_in_owner` or scripted child path — should not be the case (current `main.tscn` doesn't reference them) but verify.
- (b) `SkillRow` partial class referenced from outside `PawnDetail.cs` — grep verified before deletion.
- (c) Some test references `Brother Cassian` or other Names array entries — grep before commit 1 to verify.
- (d) Some test references `MakeRole` or `DemoSkills` — should not, but grep verifies.
- (e) Some test or build script references one of the 8 deleted files — grep before commit 2.

## Report-back format

- 3 commit SHAs (full hex).
- `dotnet test` count: 417/417 expected (unchanged).
- After commit 2: list of 8 deleted files plus zero-grep-reference confirmation per file.
- M-phase boundary: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.
- Solution file `dotnet sln list` count unchanged.
- ROADMAP rendering: confirm categorized backlog subsections present.
- Manual F5 verification: deferred to user.
- Any unexpected references found during grep-before-deletion verification (would block commit 2).
