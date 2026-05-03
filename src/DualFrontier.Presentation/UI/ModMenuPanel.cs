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
		var title = ColonyPanel.MakeLabel("MOD MENU", 16, Palette.Muted, font: Fonts.CinzelBold);
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

		var cancelButton = new Button { Text = "Cancel", CustomMinimumSize = new Vector2(96, 32) };
		cancelButton.Pressed += OnCancelPressed;
		StyleButton(cancelButton);
		buttonRow.AddChild(cancelButton);

		var applyButton = new Button { Text = "Apply", CustomMinimumSize = new Vector2(96, 32) };
		applyButton.Pressed += OnApplyPressed;
		StyleButton(applyButton);
		buttonRow.AddChild(applyButton);
	}

	private static void StyleButton(Button btn)
	{
		btn.AddThemeStyleboxOverride("normal",  UiTheme.MakeButtonNormalBox());
		btn.AddThemeStyleboxOverride("hover",   UiTheme.MakeButtonNormalBox());
		btn.AddThemeStyleboxOverride("pressed", UiTheme.MakeButtonPressedBox());
		btn.AddThemeStyleboxOverride("focus",   new StyleBoxEmpty());
		btn.AddThemeColorOverride("font_color", Palette.Text);
	}

	/// <summary>
	/// Wires the panel to its controller. Called by <c>GameRoot._Ready</c>
	/// after both the controller and panel exist. <c>internal</c> because
	/// <see cref="ModMenuController"/> is <c>internal</c> in
	/// DualFrontier.Application; both projects expose internals to
	/// DualFrontier.Presentation via <c>InternalsVisibleTo</c>.
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

		var check = new CheckBox { ToggleMode = true, ButtonPressed = info.IsPendingActive };
		check.AddThemeIconOverride("checked",   KenneyTextures.IconCheckBeige);
		check.AddThemeIconOverride("unchecked", KenneyTextures.IconCheckGrey);
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

	private static StyleBoxTexture MakePanelStyle() => UiTheme.MakeModalPanelBox();
}
