# M7.5.B.2 — Godot UI scene + ModMenuController binding

## Operating principle (load-bearing)

> «Будем работать без заглушек которые обманывают состояние, оно либо есть, либо его нет вообще.»

Mod menu UI shows only what `ModMenuController.GetEditableState()` actually returns. Empty mods/ directory → menu shows "No mods found". Hot-reload disabled per §9.6 → toggle disabled with explicit tooltip. Failed Commit → errors displayed verbatim. No fabricated data, no silent fallbacks.

## Context

M7.5.A closed (`9c895fe`, `4c648c6`, `198d948`): `ModMenuController` + `IModDiscoverer` + `Pipeline.GetActiveMods` shipped as `internal sealed` types in `DualFrontier.Application/Modding/`, fully unit-tested (30 tests).

M7.5.B.1 closed (`4956a13`, `94128be`, `b6b6d7e`): production bootstrap wires `ModIntegrationPipeline` + `DefaultModDiscoverer` + `ModMenuController`. `GameRoot._modMenuController` field populated from `GameContext.Controller`. F5 production startup verified clean.

Subsequent housekeeping commits cleaned up tangential concerns (TICK display, TickScheduler race, real pawn data, needs decay direction). 431/431 tests at baseline `9d7b7f6`. UI architecture audit confirmed: layer separation correct, UI receives method calls only, never queries simulation state.

M7.5.B.2 closes the M7 cycle's core implementation by binding the `ModMenuController` surface to a Godot Control widget that user can open via F10 hotkey. After this lands, only the M7-closure session (parallel to M5/M6 closure pattern) remains before M8.

## Out of scope

- M7-closure session — separate post-M7.5.B.2 session. Produces `docs/M7_CLOSURE_REVIEW.md` parallel to existing M3–M6 closure reviews; marks M7 row → ✅ Closed in ROADMAP; updates contradiction-discovery datapoint sequence with final tally.
- M8 vanilla mod skeletons — needed for the menu to actually have something to toggle; until then the menu shows "No mods found" empty state, which is the honest state.
- Any change to `src/DualFrontier.Core` or `src/DualFrontier.Contracts`. M-phase boundary preserved through M3–M7.5.B.1 + 4 housekeeping commits. Verified by `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returning empty.
- Any change to `ModMenuController`, `IModDiscoverer`, `DefaultModDiscoverer`, `ModIntegrationPipeline`, or related M7.5.A surface. The controller's contract is final; M7.5.B.2 only consumes it.
- Any change to existing UI widgets (`ColonyPanel`, `PawnDetail`, `GameHUD`). `ModMenuPanel` is a new sibling under `GameRoot`, not a child of `GameHUD`.
- Phase 5 backlog items (NeedsSystem decay recovery, food/water/bed entities, MoodBreakEvent handlers, etc.).
- Mod-menu UX polish: search/filter, mod metadata expanded view, drag-reorder for load order, validation error formatting beyond plain text. M8+ may add these; M7.5.B.2 ships minimum viable.

## Approved architectural decisions

1. **Authoring approach: code-only `Godot.Control` subclass.** Mirrors the existing `ColonyPanel` / `PawnDetail` / `GameHUD` pattern in this project. All layout constructed in `_Ready()` — no `.tscn` scene file. Reasons: (a) consistency with the established pattern, (b) self-contained and AI-executable without manual scene authoring between commits, (c) Phase 4 UI precedent (no Godot Control widgets in this project use editor-authored scenes — `main.tscn` only attaches root scripts).

2. **Trigger: F10 hotkey handled by `GameRoot._UnhandledInput`.** F10 toggles `ModMenuPanel.Visible`. ESC quitting via `InputRouter` stays unchanged for first iteration; ESC inside the menu does NOT cancel the editing session (user clicks Cancel button explicitly). Menu close is via F10 (treated as Cancel) or via Apply / Cancel buttons.

3. **Menu close-without-explicit-action = Cancel.** F10-while-open and (future) ESC-while-open both call `controller.Cancel()` via `ModMenuPanel.CloseAndCancel()`. Apply button calls `controller.Commit()`; on success closes, on failure stays open with errors displayed.

4. **Modal full-screen overlay.** `ModMenuPanel` covers the full viewport with semi-transparent dim background plus a centered fixed-size panel containing the actual UI. `MouseFilter = Stop` on the dim overlay prevents click-through to the simulation. Initial `Visible = false`.

5. **Empty list state: explicit "No mods found" label.** When `controller.GetEditableState()` returns 0 entries (production case until M8 lands), the list area shows a single muted Label "No mods found in mods/" with no rows. Honest signal: discoverer ran, found nothing.

6. **Per-mod row layout.** HBoxContainer with: `Name v{Version}` Label (left, expand), CheckBox toggle (right). When `info.CanToggle` is false (per §9.6 hot-reload disabled): CheckBox.Disabled = true + TooltipText = "Hot-reload disabled — restart required to load/unload this mod". CheckBox.ButtonPressed reflects `info.IsPendingActive` (mutates as user toggles).

7. **CheckBox.Toggled handler calls `controller.Toggle(modId)`.** Result handling:
    - `ToggleResult.Toggled` → no UI feedback (CheckBox already shows new state).
    - `ToggleResult.RejectedHotReloadDisabled` → status label shows "Cannot toggle hot-reload-disabled mod mid-session" + revert CheckBox to previous state.
    - `ToggleResult.UnknownMod` → status label shows "Unknown mod" (defensive — should not happen in practice).
    - `ToggleResult.NoSession` → status label shows "Menu not in editing session" (defensive — should not happen).

8. **Apply button calls `controller.Commit()`.** Result handling:
    - `result.Success == true` → set `Visible = false`, status label cleared. Editing session closed by Commit.
    - `result.Success == false` → status label shows joined `result.Errors[*].Message` lines, panel stays open per AD #4 of M7.5.A (failed commit leaves session open for retry).

9. **Cancel button calls `controller.Cancel()`.** Set `Visible = false`. Session closed.

10. **Setup pattern: `ModMenuPanel.Setup(ModMenuController controller)` called by `GameRoot._Ready` after construction.** Constructor must be parameterless (Godot rule). Controller stored as field; null-check in `OpenAndBegin` / `CloseAndCancel` / button handlers makes the panel safe before Setup.

11. **`GameRoot` adds `ModMenuPanel` to scene tree and wires F10.** New field `_modMenuPanel`. Construction in `_Ready` after `_modMenuController` is obtained from GameContext. F10 handling in `GameRoot._UnhandledInput`.

12. **Visual styling: minimal functional.** Uses existing `Palette` static class colors for consistency (PanelBg, CardBg, Border, Text, Muted, Critical for errors, Good for success states). No new theming work. The user has explicitly stated Warhammer styling is not required; existing palette is functional and stays as-is.

13. **No automated UI test surface.** Phase 4 UI widgets (`ColonyPanel`, `PawnDetail`, `GameHUD`) have no automated tests in this project — manual F5 verification is the convention. M7.5.B.2 follows precedent. Test commit (per §7.3) adds **integration tests at controller level** verifying menu-flow sequences through `GameBootstrap.CreateLoop` — these lock the contract M7.5.B.2's UI must respect, even though they don't exercise the Godot widget itself.

14. **No `Apply` button enabled-state logic.** Apply is always clickable; calling `Commit` with no changes is a valid no-op success per M7.5.A test 16 (`Commit_NoChanges_NoOpSuccess_Resumes`). User pressing Apply on an unchanged session simply closes the menu and resumes.

15. **METHODOLOGY §2.4 atomic phase review** — implementation, tests, ROADMAP closure all in one session. Three commits per §7.3.

## Required reading

1. **MOD_OS_ARCHITECTURE.md LOCKED v1.5** — §9.2 (menu flow Pause-Toggle-Apply-Resume), §9.6 (hot-reload disabled semantics), §2.2 (`hotReload` field default).
2. **M7.5.A surface** (full files):
    - `src/DualFrontier.Application/Modding/ModMenuController.cs` — public methods: `BeginEditing`, `Cancel`, `Toggle`, `CanToggle`, `GetEditableState`, `Commit`, `IsEditing`. Read carefully — M7.5.B.2 calls these from UI.
    - `src/DualFrontier.Application/Modding/EditableModInfo.cs` — record carrying `ModId`, `Manifest`, `IsCurrentlyActive`, `IsPendingActive`, `CanToggle`.
    - `src/DualFrontier.Application/Modding/ToggleResult.cs` — enum: `Toggled`, `RejectedHotReloadDisabled`, `NoSession`, `UnknownMod`.
    - `src/DualFrontier.Application/Modding/CommitResult.cs` — record carrying `Success`, `Errors`, `Warnings`, `NewlyActiveModIds`, `NewlyInactiveModIds`.
3. **M7.5.B.1 wiring**:
    - `src/DualFrontier.Application/Loop/GameContext.cs` — internal record `(GameLoop Loop, ModMenuController Controller)`.
    - `src/DualFrontier.Application/Loop/GameBootstrap.cs` — confirms controller construction.
    - `src/DualFrontier.Presentation/Nodes/GameRoot.cs` — confirms `_modMenuController` field already populated in `_Ready`. M7.5.B.2 extends this `_Ready` body.
4. **Existing UI patterns** (full files for layout/style reference):
    - `src/DualFrontier.Presentation/UI/ColonyPanel.cs` — code-only Godot Control construction pattern, `Palette` static class definition, `MakePanelStyle` factory, `MakeLabel` factory, theme overrides.
    - `src/DualFrontier.Presentation/UI/PawnDetail.cs` — additional pattern reference (HBox/VBox layout, button click handlers, Render method pattern).
    - `src/DualFrontier.Presentation/UI/GameHUD.cs` — CanvasLayer pattern (NOT used here — ModMenuPanel is a Control under GameRoot, not under GameHUD).
5. **Existing input handling**:
    - `src/DualFrontier.Presentation/Input/InputRouter.cs` — current ESC handler. Stays unchanged.
6. **Spec conformance reference**:
    - `docs/MOD_OS_ARCHITECTURE.md` §9.6 wording for the hot-reload disabled tooltip text. Match the spec phrasing closely.
7. **Existing M7.5.B.1 tests** (head only):
    - `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — pattern for the new integration tests in commit 2.
8. **METHODOLOGY** §2.4, §7.3.
9. **CODING_STANDARDS** — one class per file, English-only comments, member order, `_camelCase` private fields, no LINQ in `DualFrontier.Application/Modding/` (verified empirically; not directly relevant to M7.5.B.2 since `ModMenuPanel` lives in Presentation, but consistent style preferred).
10. **Pre-flight grep** — `grep -rn "ModMenuPanel" src/ tests/` should return zero results before commit 1 (file doesn't exist yet). Any pre-existing reference would suggest a leftover stub.

## Implementation

### Layer 1 — `ModMenuPanel` widget (1 new file)

`src/DualFrontier.Presentation/UI/ModMenuPanel.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Modal mod-menu overlay bound to <see cref="ModMenuController"/>.
/// Rendered as a full-screen <see cref="Control"/> sibling under
/// <c>GameRoot</c>; toggled via F10 (handled by <c>GameRoot._UnhandledInput</c>).
///
/// Lifecycle (per MOD_OS_ARCHITECTURE §9.2):
/// 1. F10 → <see cref="OpenAndBegin"/> calls <c>controller.BeginEditing</c>
///    (which pauses the pipeline) and renders the current editable state.
/// 2. User toggles checkboxes — each emits <c>controller.Toggle(modId)</c>.
/// 3. Apply → <c>controller.Commit</c>. On success the panel closes; on
///    failure (validation errors) the panel stays open with errors shown
///    so the user can fix and retry.
/// 4. Cancel button or F10-while-open → <c>controller.Cancel</c> (resumes
///    the pipeline) and closes the panel.
///
/// Operating principle: the panel displays only what
/// <see cref="ModMenuController.GetEditableState"/> actually returns.
/// Empty mods/ directory → "No mods found" label. Hot-reload disabled
/// (§9.6) → checkbox disabled with explicit tooltip. No fabricated data.
/// </summary>
public partial class ModMenuPanel : Control
{
    private const int PanelWidth  = 500;
    private const int PanelHeight = 420;

    private ModMenuController? _controller;
    private VBoxContainer _list = null!;
    private Label _statusLabel = null!;
    private readonly Dictionary<string, CheckBox> _rowCheckBoxes = new();

    public override void _Ready()
    {
        // Full-screen overlay anchors.
        AnchorLeft = 0; AnchorRight = 1; AnchorTop = 0; AnchorBottom = 1;
        OffsetLeft = 0; OffsetRight = 0; OffsetTop = 0; OffsetBottom = 0;
        MouseFilter = MouseFilterEnum.Stop;
        Visible = false;
        ZIndex = 100;

        // Dim background.
        var dim = new ColorRect
        {
            AnchorLeft = 0, AnchorRight = 1, AnchorTop = 0, AnchorBottom = 1,
            Color = new Color(0f, 0f, 0f, 0.6f),
            MouseFilter = MouseFilterEnum.Stop,
        };
        AddChild(dim);

        // Centered panel.
        var panel = new Panel
        {
            AnchorLeft = 0.5f, AnchorRight = 0.5f,
            AnchorTop  = 0.5f, AnchorBottom = 0.5f,
            OffsetLeft   = -PanelWidth / 2f,
            OffsetRight  =  PanelWidth / 2f,
            OffsetTop    = -PanelHeight / 2f,
            OffsetBottom =  PanelHeight / 2f,
        };
        panel.AddThemeStyleboxOverride("panel", MakePanelStyle());
        AddChild(panel);

        var root = new VBoxContainer
        {
            AnchorLeft = 0, AnchorRight = 1, AnchorTop = 0, AnchorBottom = 1,
            OffsetLeft = 16, OffsetRight = -16, OffsetTop = 16, OffsetBottom = -16,
        };
        root.AddThemeConstantOverride("separation", 12);
        panel.AddChild(root);

        // Title.
        var title = ColonyPanel.MakeLabel("MOD MENU", 13, Palette.Muted, bold: true);
        title.HorizontalAlignment = HorizontalAlignment.Center;
        root.AddChild(title);

        var ornament = ColonyPanel.MakeLabel("✦   ✦   ✦", 9, Palette.Border);
        ornament.HorizontalAlignment = HorizontalAlignment.Center;
        root.AddChild(ornament);

        // Mod list (scroll).
        var scroll = new ScrollContainer
        {
            SizeFlagsVertical = SizeFlags.ExpandFill,
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
        };
        root.AddChild(scroll);

        _list = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        _list.AddThemeConstantOverride("separation", 4);
        scroll.AddChild(_list);

        // Status label.
        _statusLabel = ColonyPanel.MakeLabel("", 10, Palette.Critical);
        _statusLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _statusLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        root.AddChild(_statusLabel);

        // Buttons row.
        var buttonRow = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        buttonRow.AddThemeConstantOverride("separation", 8);
        root.AddChild(buttonRow);

        var spacerL = new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        buttonRow.AddChild(spacerL);

        var cancelButton = new Button { Text = "Cancel", CustomMinimumSize = new Vector2(96, 28) };
        cancelButton.Pressed += OnCancelPressed;
        buttonRow.AddChild(cancelButton);

        var applyButton = new Button { Text = "Apply", CustomMinimumSize = new Vector2(96, 28) };
        applyButton.Pressed += OnApplyPressed;
        buttonRow.AddChild(applyButton);
    }

    /// <summary>
    /// Wires the panel to its controller. Called by <c>GameRoot._Ready</c>
    /// after both the controller and panel exist. Idempotent — calling
    /// twice with the same controller is a no-op; calling with a different
    /// controller silently replaces the binding (no production caller
    /// does this).
    /// </summary>
    public void Setup(ModMenuController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    /// <summary>
    /// F10 → open path. Begins editing on the controller, renders the
    /// current state, makes the panel visible. Safe to call when
    /// already visible — no-op.
    /// </summary>
    public void OpenAndBegin()
    {
        if (_controller is null) return;
        if (Visible) return;

        _controller.BeginEditing();
        _statusLabel.Text = string.Empty;
        RebuildList();
        Visible = true;
    }

    /// <summary>
    /// F10-while-open and Cancel-button path. Cancels the editing
    /// session (which resumes the pipeline) and hides the panel.
    /// Safe to call when not editing — no-op.
    /// </summary>
    public void CloseAndCancel()
    {
        if (_controller is null) return;
        if (!Visible) return;

        if (_controller.IsEditing)
            _controller.Cancel();
        Visible = false;
    }

    private void RebuildList()
    {
        if (_controller is null) return;

        foreach (var child in _list.GetChildren())
            child.QueueFree();
        _rowCheckBoxes.Clear();

        IReadOnlyList<EditableModInfo> rows = _controller.GetEditableState();
        if (rows.Count == 0)
        {
            var empty = ColonyPanel.MakeLabel("No mods found in mods/", 11, Palette.Muted);
            empty.HorizontalAlignment = HorizontalAlignment.Center;
            _list.AddChild(empty);
            return;
        }

        foreach (EditableModInfo info in rows)
            _list.AddChild(BuildRow(info));
    }

    private Control BuildRow(EditableModInfo info)
    {
        var row = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        row.AddThemeConstantOverride("separation", 8);

        ModManifest manifest = info.Manifest;
        var label = ColonyPanel.MakeLabel(
            $"{manifest.Name} v{manifest.Version}", 11, Palette.Text);
        label.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        row.AddChild(label);

        var check = new CheckBox { ButtonPressed = info.IsPendingActive };
        if (!info.CanToggle)
        {
            check.Disabled = true;
            check.TooltipText = "Hot-reload disabled — restart required to load/unload this mod";
        }
        string capturedId = info.ModId;
        bool capturedPriorState = info.IsPendingActive;
        check.Toggled += (bool pressed) => OnRowToggled(capturedId, pressed, capturedPriorState, check);
        _rowCheckBoxes[info.ModId] = check;
        row.AddChild(check);

        return row;
    }

    private void OnRowToggled(string modId, bool pressed, bool priorState, CheckBox check)
    {
        if (_controller is null) return;

        ToggleResult result = _controller.Toggle(modId);
        switch (result)
        {
            case ToggleResult.Toggled:
                _statusLabel.Text = string.Empty;
                break;
            case ToggleResult.RejectedHotReloadDisabled:
                check.SetPressedNoSignal(priorState);
                _statusLabel.Text = "Cannot toggle hot-reload-disabled mod mid-session";
                break;
            case ToggleResult.UnknownMod:
                check.SetPressedNoSignal(priorState);
                _statusLabel.Text = "Unknown mod";
                break;
            case ToggleResult.NoSession:
                check.SetPressedNoSignal(priorState);
                _statusLabel.Text = "Menu not in editing session";
                break;
        }
    }

    private void OnApplyPressed()
    {
        if (_controller is null) return;
        if (!_controller.IsEditing) return;

        CommitResult result = _controller.Commit();
        if (result.Success)
        {
            _statusLabel.Text = string.Empty;
            Visible = false;
        }
        else
        {
            var sb = new StringBuilder();
            for (int i = 0; i < result.Errors.Count; i++)
            {
                if (i > 0) sb.Append('\n');
                sb.Append(result.Errors[i].Message);
            }
            _statusLabel.Text = sb.ToString();
        }
    }

    private void OnCancelPressed()
    {
        CloseAndCancel();
    }

    private static StyleBoxFlat MakePanelStyle() => new()
    {
        BgColor          = Palette.PanelBg,
        BorderWidthLeft   = 1, BorderWidthRight = 1,
        BorderWidthTop    = 1, BorderWidthBottom = 1,
        BorderColor      = Palette.Border,
    };
}
```

**Implementation notes for the agent:**

- `CheckBox.SetPressedNoSignal(bool)` — Godot 4 API for setting checked state without re-firing the `Toggled` signal. If the API name differs in the project's Godot version, use the equivalent (e.g. assign `ButtonPressed` after temporarily disconnecting the handler). Verify before final commit.
- `TextServer.AutowrapMode.WordSmart` — Godot 4 enum. Confirm exact path; if differs, use `Label.AutowrapMode.Word` or similar.
- `ColonyPanel.MakeLabel` is `internal static` per existing source — accessible from `ModMenuPanel.cs` since both are in same assembly. Verify `MakeLabel` is internal; if it's private, lift it to `internal static` in `ColonyPanel.cs` as a tiny enabling edit (called out in commit 1 message). Only required if currently private.
- `Palette` is `internal static class` per audit — accessible.

### Layer 2 — `GameRoot` integration

`src/DualFrontier.Presentation/Nodes/GameRoot.cs` — modify in two places:

**Add field:**
```csharp
private ModMenuPanel _modMenuPanel = null!;
```

**Extend `_Ready` (after existing setup, before `_loop.Start()`):**
```csharp
_modMenuPanel = new ModMenuPanel();
AddChild(_modMenuPanel);
_modMenuPanel.Setup(_modMenuController);
```

**Add `_UnhandledInput` override:**
```csharp
public override void _UnhandledInput(InputEvent @event)
{
    if (@event is InputEventKey { Pressed: true, Keycode: Key.F10 })
    {
        if (_modMenuPanel.Visible)
            _modMenuPanel.CloseAndCancel();
        else
            _modMenuPanel.OpenAndBegin();
        GetViewport().SetInputAsHandled();
    }
}
```

XML doc on `_modMenuPanel` field: `// M7.5.B.2 — modal mod menu overlay; constructed and wired here in _Ready, toggled by F10 via _UnhandledInput.`

### No other files modified

No changes to `InputRouter.cs` (ESC handling stays). No changes to `main.tscn` (ModMenuPanel added to scene tree at runtime via `AddChild`, not declared in scene file). No changes to `GameHUD.cs`, `ColonyPanel.cs`, `PawnDetail.cs` (other than the conditional `MakeLabel` accessibility lift if needed).

## Tests

### `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — 3 new facts

These tests verify menu-flow sequences at the controller level using the production `GameBootstrap.CreateLoop` wiring. They lock the contract that M7.5.B.2's UI must respect even though the Godot widget itself is not exercised.

```csharp
[Fact]
public void MenuFlow_OpenCommitClose_LeavesEditingFalse()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);
    
    context.Controller.BeginEditing();
    Assert.True(context.Controller.IsEditing);
    
    var commit = context.Controller.Commit();
    Assert.True(commit.Success);
    Assert.False(context.Controller.IsEditing);
}

[Fact]
public void MenuFlow_OpenCancelClose_LeavesEditingFalse()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);
    
    context.Controller.BeginEditing();
    Assert.True(context.Controller.IsEditing);
    
    context.Controller.Cancel();
    Assert.False(context.Controller.IsEditing);
}

[Fact]
public void MenuFlow_OpenWithoutCommitOrCancel_StaysEditing()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);
    
    context.Controller.BeginEditing();
    Assert.True(context.Controller.IsEditing);
    
    // No Commit, no Cancel — session must persist for the user to
    // continue editing across multiple Toggle calls.
    var rows = context.Controller.GetEditableState();
    Assert.True(context.Controller.IsEditing);  // unchanged
}
```

These three tests exercise the sequence patterns the UI uses:
- **Test 1**: Apply button success path — Commit closes the session. UI then sets `Visible = false`.
- **Test 2**: Cancel button path — Cancel closes the session. UI then sets `Visible = false`.
- **Test 3**: User keeps menu open without applying — session persists. UI continues to allow toggles.

Tests use `Xunit.Assert` to match the surrounding test class's existing style (no FluentAssertions).

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors after each commit.
2. `dotnet test` count: **431 + 3 new = 434/434** passing.
3. `src/DualFrontier.Presentation/UI/ModMenuPanel.cs` exists, derived from `Godot.Control`, with `_Ready`, `Setup`, `OpenAndBegin`, `CloseAndCancel` public methods plus internal handlers.
4. `GameRoot._Ready` constructs `_modMenuPanel`, calls `AddChild`, calls `Setup(_modMenuController)`.
5. `GameRoot._UnhandledInput` handles F10 → toggle visibility.
6. `ColonyPanel.MakeLabel` accessibility unchanged unless needed (already `internal static`); if a lift was required, it's documented in commit 1 message.
7. M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty.
8. M7.x + housekeeping regression suites all green: M7.1 (11), M7.2 (13), M7.3 (5+2), M7.4 (9), M7.5.A (30), M7.5.B.1 (7), housekeeping commits (TickScheduler, RandomPawnFactory, NeedsAccumulation), all M0–M6 + Persistence + Systems + Core suites.
9. `dotnet sln list` count unchanged from prior baseline.
10. **Manual F5 verification deferred to user.** Predicted observation:
    - F5 launches as before — 10 pawns, TICK counter, mood/needs UI all rendering correctly.
    - Pressing **F10** opens a centered modal panel with title "MOD MENU", a list area showing "No mods found in mods/" (empty state, since production `mods/` likely contains only `DualFrontier.Mod.Example` which may or may not be discovered depending on its manifest validity), and Cancel / Apply buttons at the bottom.
    - Pressing **F10 again** closes the panel.
    - Pressing **Apply** on an empty/unchanged session closes the panel without errors.
    - Pressing **Cancel** closes the panel.
    - Simulation continues running underneath the modal (pawns visible behind dim overlay, TICK still advances).
    - ESC continues to quit the game (InputRouter unchanged).

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `feat(presentation): ModMenuPanel + F10 hotkey wiring + GameRoot integration`

- New file `src/DualFrontier.Presentation/UI/ModMenuPanel.cs` per §Layer 1.
- Modify `src/DualFrontier.Presentation/Nodes/GameRoot.cs` per §Layer 2: new field, _Ready extension, _UnhandledInput override.
- If `ColonyPanel.MakeLabel` is not internal-or-public-accessible from ModMenuPanel.cs, lift its accessibility within the same commit and note in commit body.
- Verify existing M7.x + housekeeping suites still pass via `dotnet test --filter "FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ModMenuController|FullyQualifiedName~DefaultModDiscoverer|FullyQualifiedName~PipelineGetActiveMods|FullyQualifiedName~ManifestRewriter|FullyQualifiedName~GameBootstrapIntegration|FullyQualifiedName~TickScheduler|FullyQualifiedName~RandomPawnFactory|FullyQualifiedName~NeedsAccumulation|FullyQualifiedName~NeedsJobIntegration"` — all green.
- Commit message body documents: «M7.5.B.2 — Godot UI scene + ModMenuController binding. Code-only Control subclass mirroring the ColonyPanel/PawnDetail/GameHUD pattern (Phase 4 UI precedent: no .tscn editor authoring, all layout in C#). F10 toggles modal visibility via GameRoot._UnhandledInput. Modal overlay covers viewport with dim background + centered fixed-size panel. Empty mods/ → "No mods found" label (honest state per operating principle). §9.6 hot-reload disabled mods → CheckBox.Disabled with explicit tooltip. Failed Commit leaves session open with errors displayed. Manual F5 verification deferred to user — UI widgets in this project are untested by convention.»

**2.** `test(bootstrap): mod-menu interaction sequence integration tests`

- Add 3 new `[Fact]` to existing `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` per §Tests above.
- Run full suite. Confirm 434/434.
- Commit message notes: «Three integration tests at controller level verifying the menu-flow sequences M7.5.B.2's UI invokes: Commit closes session, Cancel closes session, opening without explicit close keeps session live. Lock the contract M7.5.B.2 must respect even though the Godot widget itself is not exercised. Phase 4 UI test precedent: widgets like ColonyPanel and PawnDetail have no automated tests; manual F5 verification dominant.»

**3.** `docs(roadmap): close M7.5.B.2 — Godot mod-menu UI scene`

- `ROADMAP.md`:
  - Header status line: `*Updated: YYYY-MM-DD (M7.5.B.2 closed — Godot mod-menu UI scene + ModMenuController binding via F10 hotkey; M7-closure pending; Phase 5 backlog tracked).*`
  - Engine snapshot: 431 → 434 tests.
  - M7 sub-phase status block: M7.5.B.2 ⏭ Pending → ✅ Closed with commits 1+2 SHA + acceptance summary in M7.1/M7.2/.../M7.5.B.1 entry pattern. Mention: code-only Control subclass per Phase 4 UI precedent, F10 toggle, modal overlay, empty-state honesty, §9.6 hot-reload disabled handling, failed-commit-stays-open per AD #4 of M7.5.A.
  - Status overview table M7 row tests column extended: "M7.5.B.2 added (`MenuFlow_OpenCommitClose_LeavesEditingFalse`, `MenuFlow_OpenCancelClose_LeavesEditingFalse`, `MenuFlow_OpenWithoutCommitOrCancel_StaysEditing`)".
  - M7-closure entry stays ⏭ Pending.

**Special verification preamble:**

After commit 1: full `dotnet test` — must pass at 431/431 (no new tests yet). If a Godot-related compile error surfaces (CheckBox.SetPressedNoSignal API mismatch, Color constructor signature, AutowrapMode enum path), STOP and adapt to the actual API, document deviation in commit body.

After commit 2: `dotnet test --filter "FullyQualifiedName~MenuFlow"` — 3 new tests green. Full suite at 434/434.

After commit 3: ROADMAP renders cleanly; M7.5.B.2 marked closed; M7-closure entry remains pending.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice.

**Hypothesis-falsification clause:**

Datapoints (per [M6 closure review §10](./M6_CLOSURE_REVIEW.md)): M3=1, M4=1, M5=0, M6=0, M7.1=0, M7.2=0, M7.3=0, M7.4=0, M7.5.A=0, M7.5.B.1=0. **M7.5.B.2 closure pending = potentially eleventh consecutive zero post-M4.**

M7.5.B.2 exercises §9.2 (menu flow surface) + §9.6 (hot-reload disabled UI semantics). Implementation surface is Godot UI presentation layer — entirely below the spec layer in technical contract sense. **If implementation surfaces a §9 contradiction requiring v1.6 ratification → hypothesis falsified. Report immediately.**

Plausible v1.6 candidates worth flagging if encountered:

(a) **§9.2 menu open semantic underspec.** AD #2 (F10 toggle = open/close, close-without-action treated as Cancel) is interpreted from the canonical sequence. If §9.2 wording demands different close-without-Apply semantics, ratification candidate.

(b) **§9.6 tooltip text wording.** AD #6 chose "Hot-reload disabled — restart required to load/unload this mod". If §9.6 mandates exact wording or a different message, ratification.

(c) **Failed-Commit close-or-stay behaviour.** AD #8 follows M7.5.A AD #4 (failed Commit leaves session open + simulation paused). If §9 wording changed implications since v1.5, ratification.

Plausible non-spec frictions:

(d) **Godot CheckBox.SetPressedNoSignal API**: if the method name differs (e.g. `SetPressedNoEmit` or older Godot signature `SetPressed(bool, bool)`), adapt and note.

(e) **Z-index / scene tree input ordering**: F10 in `GameRoot._UnhandledInput` may not fire if a child node consumes the event first. Verify by manual F5; if F10 doesn't open menu, adjust input handling (consider `_Input` instead, or move handling to ModMenuPanel).

(f) **TextServer.AutowrapMode enum path**: Godot 4.0/4.1/4.2 may have moved the enum. If compile fails, locate correct path.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (431 + 3 = 434 expected, or actual).
- 3 new tests by name (`MenuFlow_OpenCommitClose_LeavesEditingFalse`, `MenuFlow_OpenCancelClose_LeavesEditingFalse`, `MenuFlow_OpenWithoutCommitOrCancel_StaysEditing`) — all green.
- Regression confirmation: full M7.x + housekeeping + M0–M6 + Persistence + Systems + Core suites green.
- Working tree state: clean.
- M-phase boundary: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.
- Solution file: `dotnet sln list` count unchanged.
- ROADMAP: confirm M7.5.B.2 entry marked ✅ Closed; M7-closure remains ⏭ Pending; status line updated; engine snapshot bumped to 434.
- Manual F5 verification: deferred to user with predicted-observation checklist (F10 opens, F10 closes, Apply on empty closes, Cancel closes, ESC quits, simulation continues underneath).
- **§9 contradiction status**: zero (or REPORT IMMEDIATELY with category).
- **API adaptations**: any deviation from the brief's Godot API usage (CheckBox.SetPressedNoSignal, AutowrapMode, etc.).
- **Z-index / input handling adjustments**: if F10 routing required modification.
- **`MakeLabel` accessibility**: confirm whether the accessibility lift was needed.
- Any unexpected findings.
