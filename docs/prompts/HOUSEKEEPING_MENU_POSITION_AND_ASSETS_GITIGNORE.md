---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE
---
# Housekeeping — ModMenuPanel position fix + extracted assets gitignore

## Operating principle (load-bearing)

> «Будем работать без заглушек которые обманывают состояние, оно либо есть, либо его нет вообще.»

Apply here: only the source-of-truth zip files belong in git. Extracted Kenney/Cinzel asset folders are derived state and must not bloat the repository.

## Context

M7.5.B.2 closed at commit `b519804` (3 commits: `9a75b75`, `6ecee53`, `b519804`). 434/434 tests passing. F5 manual verification surfaced two issues:

1. **`ModMenuPanel` modal misposition** — the menu opens in the wrong place because `ModMenuPanel` extends `Godot.Control` and was added as a direct child of `GameRoot` (a `Node2D`). Godot's anchor system requires a Viewport-relative parent (`CanvasLayer` or another Control); `Node2D` doesn't provide one. This is plausible friction (e) flagged in the original M7.5.B.2 brief — the F5 verification check did its job.

2. **Extracted asset folders in working tree** — user has run extraction on `assets/*.zip` (Kenney UI packs, Cinzel font, sprite packs). The source `.zip` files are already in `.gitignore`. The extracted folders are now in the working tree and must also be ignored: source zips remain SoT in git, extractions are local-only derived state.

Phase 4 UI pattern reference: `GameHUD` extends `CanvasLayer` (Layer 10), with `ColonyPanel` and `PawnDetail` as Control children of GameHUD. They render correctly because CanvasLayer provides Viewport-relative anchor space. `ModMenuPanel` must follow the same pattern but on a higher layer (Layer 20) so the modal sits on top of the regular HUD.

## Out of scope

- **UI redesign with Kenney + Cinzel** — separate larger brief. This pass only fixes positioning and gitignore hygiene; visual styling stays unchanged using the existing `Palette` static class.
- **Pawn sprite replacement** — Phase 5 / M8 territory.
- **TileMapRenderer asset replacement** — Phase 5 / M8 territory.
- **Theme.tres resource creation** — Phase 5 / M8 territory.
- **`.sln` build verification gap** — separate small housekeeping brief; deferred until after this fix.
- **M7-closure session** — deferred.
- ANY change to `src/DualFrontier.Core` or `src/DualFrontier.Contracts`. M-phase boundary preserved through M7.5.B.2 + 4 prior housekeeping commits.
- ANY change to `ModMenuController`, `ModIntegrationPipeline`, or M7.5.A surface. The controller's contract is final.
- ANY change to existing UI widgets (`ColonyPanel`, `PawnDetail`, `GameHUD`). Only `ModMenuPanel` is touched.
- ANY change to `GameBootstrap`, `GameLoop`, `GameContext`, or other Application-layer types.
- ANY existing test fixture modification. Existing 3 MenuFlow tests cover controller-level contract; positioning is visual-only and untestable in this codebase per Phase 4 UI precedent.

## Approved architectural decisions

1. **`ModMenuPanel` converts from `Control` to `CanvasLayer`.** This matches the `GameHUD` pattern (CanvasLayer Layer 10 with Control children). The class signature changes from `public partial class ModMenuPanel : Control` to `public partial class ModMenuPanel : CanvasLayer`.

2. **`Layer = 20`.** Higher than `GameHUD.Layer = 10` so the modal renders above the regular HUD (ColonyPanel, PawnDetail) without z-index gymnastics.

3. **Internal Control wrapper named `_root`.** All existing UI structure (dim overlay, centered panel, title, list, status, buttons) becomes children of `_root` instead of `this`. `_root` has full-screen anchors and provides the Control layout space the modal needs. CanvasLayer itself doesn't have anchors — it's a coordinate space, not a Rect.

4. **`Visible` toggle stays on the CanvasLayer itself.** Godot 4: `CanvasLayer.Visible` is inherited from `Node` (specifically, `CanvasLayer` exposes a `Visible` property that hides the entire layer). `GameRoot._UnhandledInput` continues to read/write `_modMenuPanel.Visible` unchanged. Verify at build time — if `CanvasLayer.Visible` is missing in the project's Godot version, fall back to toggling `_root.Visible`.

5. **`Setup`, `OpenAndBegin`, `CloseAndCancel`, `OnRowToggled`, `OnApplyPressed`, `OnCancelPressed`, `RebuildList`, `BuildRow`, `MakePanelStyle` method signatures unchanged.** Only the `_Ready` body is restructured.

6. **`MouseFilter = MouseFilterEnum.Stop` moves from the (deleted) `this`-Control onto the `_root` Control.** Same effect — `_root` catches all mouse events when visible, blocking click-through to simulation underneath.

7. **`ZIndex = 100` removed.** Was set on `this` (the old Control). Replaced by CanvasLayer's `Layer = 20` which is the correct mechanism for layer ordering. ZIndex on the inner `_root` not needed because Layer already ordering everything.

8. **`GameRoot.cs` is NOT modified.** Field type stays `private ModMenuPanel _modMenuPanel = null!;`. `_Ready` still does `_modMenuPanel = new ModMenuPanel(); AddChild(_modMenuPanel); _modMenuPanel.Setup(_modMenuController);`. `_UnhandledInput` still toggles `_modMenuPanel.Visible`. The CanvasLayer-vs-Control parent type difference is invisible from GameRoot's perspective because both are `Node` subtypes that accept `AddChild`.

9. **No changes to existing 3 MenuFlow integration tests.** They exercise controller logic, not Godot widget visibility.

10. **`.gitignore` adds explicit patterns for extracted Kenney/Cinzel folders.** Patterns must:
    - Cover all `kenney_*` extracted folders (case-insensitive consideration: extracted folder names match zip names typically — `kenney_ui-pack.zip` → `kenney_ui-pack/`)
    - Cover the Cinzel font folder (likely `cinzel/` or `Cinzel/` depending on extraction)
    - NOT match `assets/scenes/` (which has tracked `README.md` and `sample.dfscene`)
    - NOT match the `.zip` files themselves (already in gitignore per user's prior commit; verify)

11. **Pre-flight `git status` check.** If extraction happened before gitignore patterns are in place and the agent finds extracted folders in `git status` output (untracked but visible), the gitignore commit alone is sufficient — they were never tracked. If somehow `git ls-files assets/<folder>/` returns paths (meaning they got tracked), prepend `git rm --cached -r <folder>` for each before adding gitignore patterns. Defensive handling for either scenario.

12. **METHODOLOGY §2.4 atomic phase review** — implementation, gitignore, ROADMAP closure all in one session. Three commits per §7.3 (`fix → chore → docs`).

## Required reading

1. `src/DualFrontier.Presentation/UI/ModMenuPanel.cs` — full file. The fix target.
2. `src/DualFrontier.Presentation/UI/GameHUD.cs` — full file. CanvasLayer + Control children pattern reference.
3. `src/DualFrontier.Presentation/Nodes/GameRoot.cs` — full file. Confirms `_modMenuPanel.Visible` access pattern stays valid; confirms no changes needed.
4. `.gitignore` (project root) — current content. Confirms `*.zip` or `assets/*.zip` already ignored; identify the section to extend with extracted folder patterns.
5. `assets/` directory — `ls` to inventory current content (zips + extracted folders). Use `dir` (Windows) or `ls -la` (bash). Record exact folder names for gitignore patterns.
6. `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — confirm 3 MenuFlow tests still pass without modification.
7. `docs/ROADMAP.md` — locate the M7.5.B.2 closure entry; this housekeeping appends a follow-up note, does not move M7.5.B.2 status.
8. `docs/methodology/METHODOLOGY.md` §2.4, §7.3.
9. `docs/methodology/CODING_STANDARDS.md` — English-only comments, member order.

Pre-flight verification commands (run before any edits):

```
# Confirm baseline state.
git log --oneline -1                                # expect b519804
dotnet test                                         # expect 434/434 passing

# Inventory assets folder.
ls assets/                                          # list zips + extracted folders
git ls-files assets/                                # what git tracks under assets/
git status assets/                                  # what's untracked or modified

# Verify .gitignore current state.
cat .gitignore | grep -i -E '\.zip|assets'          # current asset-related rules
```

Record findings in TodoWrite or working memory before making changes.

## Implementation

### Commit 1 — `fix(presentation): ModMenuPanel position via CanvasLayer conversion`

**File: `src/DualFrontier.Presentation/UI/ModMenuPanel.cs`**

Replace the entire file contents with:

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
/// Rendered as a <see cref="CanvasLayer"/> at Layer 20 (above <c>GameHUD</c>'s
/// Layer 10) so the modal sits on top of the regular HUD. Toggled via F10
/// (handled by <c>GameRoot._UnhandledInput</c>).
///
/// CanvasLayer parentage chosen over plain Control because Control's anchor
/// system requires a Viewport-relative parent; <c>GameRoot</c> is a
/// <see cref="Node2D"/> and does not provide that. Mirrors the
/// <c>GameHUD</c> pattern: CanvasLayer wrapper + Control children with
/// full-screen anchors. The internal <c>_root</c> Control hosts the dim
/// overlay, centered panel, and all interactive widgets.
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
public partial class ModMenuPanel : CanvasLayer
{
    private const int PanelWidth  = 500;
    private const int PanelHeight = 420;

    private ModMenuController? _controller;
    private Control _root = null!;
    private VBoxContainer _list = null!;
    private Label _statusLabel = null!;
    private readonly Dictionary<string, CheckBox> _rowCheckBoxes = new();

    public override void _Ready()
    {
        Layer = 20;
        Visible = false;

        // Full-viewport Control wrapper. Anchors map relative to Viewport
        // when the parent is a CanvasLayer.
        _root = new Control
        {
            AnchorLeft = 0, AnchorRight = 1, AnchorTop = 0, AnchorBottom = 1,
            OffsetLeft = 0, OffsetRight = 0, OffsetTop = 0, OffsetBottom = 0,
            MouseFilter = Control.MouseFilterEnum.Stop,
        };
        AddChild(_root);

        // Dim background covers the entire viewport.
        var dim = new ColorRect
        {
            AnchorLeft = 0, AnchorRight = 1, AnchorTop = 0, AnchorBottom = 1,
            Color = new Color(0f, 0f, 0f, 0.6f),
            MouseFilter = Control.MouseFilterEnum.Stop,
        };
        _root.AddChild(dim);

        // Centered fixed-size panel.
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
        _root.AddChild(panel);

        var content = new VBoxContainer
        {
            AnchorLeft = 0, AnchorRight = 1, AnchorTop = 0, AnchorBottom = 1,
            OffsetLeft = 16, OffsetRight = -16, OffsetTop = 16, OffsetBottom = -16,
        };
        content.AddThemeConstantOverride("separation", 12);
        panel.AddChild(content);

        // Title.
        var title = ColonyPanel.MakeLabel("MOD MENU", 13, Palette.Muted, bold: true);
        title.HorizontalAlignment = HorizontalAlignment.Center;
        content.AddChild(title);

        var ornament = ColonyPanel.MakeLabel("✦   ✦   ✦", 9, Palette.Border);
        ornament.HorizontalAlignment = HorizontalAlignment.Center;
        content.AddChild(ornament);

        // Mod list (scroll).
        var scroll = new ScrollContainer
        {
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
        };
        content.AddChild(scroll);

        _list = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };
        _list.AddThemeConstantOverride("separation", 4);
        scroll.AddChild(_list);

        // Status label.
        _statusLabel = ColonyPanel.MakeLabel("", 10, Palette.Critical);
        _statusLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _statusLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        content.AddChild(_statusLabel);

        // Buttons row.
        var buttonRow = new HBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        buttonRow.AddThemeConstantOverride("separation", 8);
        content.AddChild(buttonRow);

        var spacerL = new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
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
    internal void Setup(ModMenuController controller)
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
        var row = new HBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        row.AddThemeConstantOverride("separation", 8);

        ModManifest manifest = info.Manifest;
        var label = ColonyPanel.MakeLabel(
            $"{manifest.Name} v{manifest.Version}", 11, Palette.Text);
        label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(label);

        var check = new CheckBox { ButtonPressed = info.IsPendingActive, ToggleMode = true };
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

**Diff summary:** class extends `CanvasLayer` not `Control`; new `_root` Control field hosts all UI; `_Ready` body restructured to add `_root` as first child of CanvasLayer with all subsequent UI nodes under `_root`; `Layer = 20` replaces `ZIndex = 100`; full-screen anchors and `MouseFilter` move from `this` onto `_root`. All public method signatures unchanged.

**Build verification:** must use the explicit Presentation project build (top-level `dotnet build` does not cover Godot project per M7.5.B.2 finding):

```
dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj
```

If `CanvasLayer.Visible` does not exist in this Godot version (4.6.1 should support it), fall back: keep the property toggling on `_root` instead. Document deviation in commit body if encountered.

**Test verification:**

```
dotnet test
```

Expected: 434/434 pass unchanged. The 3 MenuFlow tests exercise controller logic and do not touch Godot widgets.

**Commit message body:**

> M7.5.B.2's modal opened in the wrong place because the panel extended Godot.Control and was added as direct child of GameRoot (Node2D). Control anchors require a Viewport-relative parent; Node2D doesn't provide one. Converting to CanvasLayer (Layer 20) matches the GameHUD pattern (Layer 10) and provides the proper coordinate space. Internal _root Control wraps all UI and supplies anchor-able full-screen layout. Public method signatures unchanged; GameRoot.cs not modified. Manual F5 verification by user: F10 should now open a centered modal.

### Commit 2 — `chore(repo): ignore extracted asset folders, keep source zips tracked`

**Pre-flight inventory (run first):**

```
ls assets/
git status assets/
git ls-files assets/
```

Record the exact extracted folder names. Common Kenney pattern: `kenney_ui-pack/`, `kenney_ui-pack-rpg-expansion/`, `kenney_game-icons/`, `kenney_micro-roguelike/`, `kenney_roguelike-rpg-pack/`, `kenney_rpg-urban-pack/`, `kenney_simple-space/`, `kenney_tiny-dungeon/`, `kenney_particle-pack/`. Cinzel typically: `cinzel/` or `Cinzel/`.

**If any extracted folder is already tracked** (appears in `git ls-files assets/<folder>/`):

```
git rm --cached -r assets/<folder>/
```

For each tracked extracted folder. Do NOT delete from disk — only from git index.

**File: `.gitignore`**

Locate the existing assets-related section (where `*.zip` or `assets/*.zip` is listed). Append a new block:

```gitignore
# Extracted Kenney asset packs and Cinzel font — local-only derived state.
# The source .zip files committed at repo root are the SoT; extractions are
# performed by the contributor locally and must not pollute git history.
/assets/cinzel/
/assets/Cinzel/
/assets/kenney_*/
/assets/Kenney_*/
```

If the extraction produced folders with different naming (verify via the pre-flight `ls`), add explicit patterns matching the actual folder names. Use a wildcard pattern only when the family of names is known (e.g. all `kenney_*` zips do produce `kenney_*` folders).

**Verification:**

```
git status                                         # extracted folders should NOT appear
git check-ignore -v assets/kenney_ui-pack/         # should report which rule matches
git check-ignore -v assets/cinzel/                 # same
ls assets/scenes/                                  # confirm scenes folder still tracked
git ls-files assets/scenes/                        # README.md and sample.dfscene present
```

**Commit message body:**

> Extracted Kenney UI/sprite packs and Cinzel font live in assets/ as local derived state. Source .zip files at assets/<name>.zip are already gitignored (committed by user previously) and remain the source of truth. Extractions add hundreds of PNG/TTF/etc. files that would bloat git history. The `assets/scenes/` directory stays tracked (README.md + sample.dfscene fixtures used by tests).

### Commit 3 — `docs(roadmap): note ModMenuPanel position fix and asset extraction prep`

**File: `docs/ROADMAP.md`**

1. Update header status line:

   ```
   *Updated: 2026-05-02 (housekeeping — ModMenuPanel position fixed via CanvasLayer conversion; extracted asset folders gitignored; UI redesign with Kenney+Cinzel pending; M7-closure pending; .sln gap fix pending; Phase 5 backlog tracked).*
   ```

2. Engine snapshot tests count: unchanged at 434.

3. Append to the M7.5.B.2 closure entry a follow-up note:

   ```
   - **Follow-up housekeeping** (commits `<sha-1>`, `<sha-2>`, `<sha-3>`):
     - ModMenuPanel converted from Control to CanvasLayer (Layer 20) to fix
       modal misposition surfaced during F5 verification — Phase 4 UI
       pattern conformance.
     - Extracted Kenney UI packs and Cinzel font added to `.gitignore`;
       source `.zip` files remain the in-git SoT. Foundation prep for
       upcoming UI redesign brief.
   ```

4. In the Backlog section under "Phase 5 — UI ↔ data wiring" subsection, add:

   ```
   - **UI redesign with Kenney UI pack + Cinzel font** — replace the
     placeholder Palette-driven minimal styling with proper themed UI using
     extracted assets at `assets/kenney_ui-pack/`, `assets/kenney_ui-pack-rpg-expansion/`,
     `assets/cinzel/`. Scope decisions pending (which Kenney pack as base,
     theme system breadth, font application rules). Separate brief required.
   ```

5. Add a new entry under "Phase 5 — combat / magic / scaffolding" subsection:

   ```
   - **Pawn sprite replacement** — current PawnVisual uses solid blue squares;
     Kenney roguelike packs (assets/kenney_roguelike-rpg-pack/, kenney_tiny-dungeon/)
     have pixel-art sprites. Tied to PawnVisual + render command extensions;
     belongs with combat/magic system landing.
   ```

6. Ensure `docs/prompts/HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE.md` (this brief) is referenced in the housekeeping artifacts list if such a list exists in ROADMAP.

**Verification:**

```
git diff docs/ROADMAP.md     # confirm only the noted edits
```

## Acceptance criteria

1. `dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj` clean — 0 warnings, 0 errors.
2. `dotnet test` count: **434/434 unchanged**. No new tests, no fixture changes.
3. `ModMenuPanel` extends `CanvasLayer`, has `Layer = 20`, internal `_root` Control wraps all UI.
4. `GameRoot.cs` unchanged from baseline `b519804`.
5. `.gitignore` contains explicit patterns matching all extracted asset folders. `git status` shows no extracted folder paths under `assets/`.
6. `assets/scenes/` directory and its tracked files (README.md, sample.dfscene) unchanged.
7. M-phase boundary preserved: `git diff b519804..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty.
8. M7.x + previous housekeeping suites all green.
9. `dotnet sln list` count unchanged.
10. **Manual F5 verification deferred to user.** Predicted observations after this commit:
    - F5 launches as before (10 pawns, TICK counter, mood/needs UI all rendering correctly).
    - Pressing **F10** opens a centered modal panel — title "MOD MENU", "No mods found in mods/" empty state, Cancel/Apply buttons, all positioned correctly in screen center (not off to one side).
    - Modal renders ON TOP of the regular HUD (ColonyPanel and PawnDetail visible behind dim overlay but underneath modal).
    - Pressing **F10 again** closes the panel cleanly.
    - **Apply** on empty/unchanged session closes without errors.
    - **Cancel** closes the panel.
    - Simulation continues running underneath modal (pawns visible behind dim overlay, TICK still advances).
    - **ESC** continues to quit the game (`InputRouter` unchanged).

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj && dotnet test`:

**1.** `fix(presentation): ModMenuPanel position via CanvasLayer conversion`

- Replace `src/DualFrontier.Presentation/UI/ModMenuPanel.cs` per §Commit 1.
- Build + test verification.
- Commit message body per §Commit 1.

**2.** `chore(repo): ignore extracted asset folders, keep source zips tracked`

- Pre-flight inventory of `assets/` and git tracking state.
- If any extracted folder is tracked: `git rm --cached -r` it.
- Append asset-folder patterns to `.gitignore` per §Commit 2.
- Verification commands per §Commit 2.
- Commit message body per §Commit 2.

**3.** `docs(roadmap): note ModMenuPanel position fix and asset extraction prep`

- Edit `docs/ROADMAP.md` per §Commit 3.
- Build + test verification (no code change but sanity check).

**Special verification preamble:**

After commit 1: `dotnet test` — must pass at 434/434 (no new tests, no fixture changes). Manual F5 deferred to user with the observation checklist.

After commit 2: `git status` shows clean; `git check-ignore -v assets/<extracted>/` confirms each pattern matches. `assets/scenes/` tracked files visible via `git ls-files assets/scenes/`.

After commit 3: ROADMAP renders cleanly; M7.5.B.2 follow-up note appended; new UI-redesign and pawn-sprite Phase 5 backlog entries present.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice.

**Hypothesis-falsification clause:**

This is housekeeping post-M7.5.B.2 closure. M-cycle datapoint sequence (now 11 consecutive zeros post-M4 after M7.5.B.2 closure) unaffected. No `MOD_OS_ARCHITECTURE` ratification candidate is plausible — this is presentation-layer + repository-hygiene work, below the spec layer.

Plausible non-spec frictions worth flagging if encountered:

(a) **`CanvasLayer.Visible` API in Godot 4.6.1**: should exist (`CanvasLayer` exposes `Visible` boolean property since Godot 4.0). If missing, fall back to toggling `_root.Visible` and document in commit 1 body.

(b) **Extracted folder names with unusual casing**: pre-flight `ls assets/` reveals exact names. If patterns in `.gitignore` don't match (e.g. zip extracts to `Kenney UI Pack/` with spaces, not `kenney_ui-pack/`), expand the gitignore patterns to cover actual names. Don't assume; verify.

(c) **Already-tracked extracted folders**: if `git ls-files assets/<folder>/` returns paths, prepend `git rm --cached -r` for each before committing the gitignore. This unstages without deleting on disk.

(d) **`MouseFilterEnum` / `SizeFlags` / `AutowrapMode` enum paths**: if Godot 4.6.1 nests these differently, adjust. Should be `Control.MouseFilterEnum.Stop`, `Control.SizeFlags.ExpandFill`, `TextServer.AutowrapMode.WordSmart` per the 4.6.1 docs, but verify on the actual project.

(e) **`Setup` method accessibility**: was lifted to `internal` during M7.5.B.2 commit 1 because `ModMenuController` is `internal sealed`. Stays `internal`.

(f) **`assets/.gitkeep`**: not present and not needed. The `assets/scenes/` subdirectory keeps the assets/ folder alive in git via tracked `README.md` and `sample.dfscene`.

(g) **CRLF line endings on Windows**: if `.gitignore` editing produces CRLF/LF mixing warnings, ensure platform-appropriate endings; do not introduce mixed style.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (434/434 expected).
- Build verification command used (`dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj`) — confirmed passing.
- `ModMenuPanel.cs` change summary: now extends `CanvasLayer`, Layer 20, internal `_root` Control wrapper.
- `GameRoot.cs` unchanged confirmation: `git diff b519804..HEAD -- src/DualFrontier.Presentation/Nodes/GameRoot.cs` returns empty.
- Pre-flight asset inventory: list of folders found in `assets/`, list of folders that were in git index (if any) before `git rm --cached`.
- `.gitignore` patterns added: exact text of new lines.
- `git check-ignore -v assets/<folder>/` results for each extracted folder — confirms each matches a rule.
- `assets/scenes/` still tracked: `git ls-files assets/scenes/` output.
- ROADMAP backlog: confirm follow-up note appended to M7.5.B.2 entry, two new Phase 5 entries (UI redesign + pawn sprite replacement).
- M-phase boundary confirmation: `git diff b519804..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.
- Manual F5 verification: deferred to user with observation checklist (centered modal, F10 toggles, Apply/Cancel work, simulation continues underneath).
- Any API adaptations from brief: flag if `CanvasLayer.Visible` was missing, if folder names differed from assumptions, if Godot enum paths needed adjustment.
- Any unexpected findings.
