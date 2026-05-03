using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Left-edge colony roster: title, list of clickable pawn rows, and a
/// bottom tick counter. Kenney textured panel + Cinzel display fonts for
/// title and pawn names; numerical content (TICK counter) keeps default
/// sans for legibility.
/// </summary>
public partial class ColonyPanel : Panel
{
    public event Action<EntityId>? PawnSelected;

    private VBoxContainer _list      = null!;
    private Label         _tickLabel = null!;

    private readonly Dictionary<EntityId, PawnRow> _rows = new();
    private EntityId? _selected;

    public override void _Ready()
    {
        AnchorTop    = 0;
        AnchorBottom = 1;
        OffsetLeft   = 0;
        OffsetRight  = 180;
        OffsetTop    = 0;
        OffsetBottom = 0;

        AddThemeStyleboxOverride("panel", MakePanelStyle());

        var root = new VBoxContainer
        {
            AnchorLeft = 0, AnchorRight = 1, AnchorTop = 0, AnchorBottom = 1,
            OffsetLeft = 12, OffsetRight = -12, OffsetTop = 12, OffsetBottom = -12
        };
        AddChild(root);

        var title = MakeLabel("COLONY", 14, Palette.Muted, font: Fonts.CinzelBold);
        root.AddChild(title);

        var ornament = MakeLabel("✦   ✦   ✦", 9, Palette.Border);
        ornament.HorizontalAlignment = HorizontalAlignment.Center;
        root.AddChild(ornament);

        _list = new VBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        _list.AddThemeConstantOverride("separation", 4);
        root.AddChild(_list);

        _tickLabel = MakeLabel("TICK: 0", 10, Palette.Muted);
        _tickLabel.HorizontalAlignment = HorizontalAlignment.Center;
        root.AddChild(_tickLabel);
    }

    public void UpdatePawn(
        EntityId id, string name, string jobLabel, float mood)
    {
        if (!_rows.TryGetValue(id, out var row))
        {
            row = new PawnRow(id);
            EntityId captured = id;
            row.Pressed += () =>
            {
                _selected = captured;
                PawnSelected?.Invoke(captured);
                RefreshSelection();
            };
            _list.AddChild(row);
            _rows[id] = row;

            if (_selected is null)
            {
                _selected = id;
                PawnSelected?.Invoke(id);
            }
        }

        row.SetData(name, jobLabel, mood);
        RefreshSelection();
    }

    public void SetTick(int tick) => _tickLabel.Text = $"TICK: {tick}";

    private void RefreshSelection()
    {
        foreach (var kv in _rows)
            kv.Value.SetSelected(_selected.HasValue && _selected.Value.Equals(kv.Key));
    }

    private static StyleBoxTexture MakePanelStyle() => UiTheme.MakeMainPanelBox();

    internal static Label MakeLabel(string text, int size, Color color, FontFile? font = null, bool bold = false)
    {
        var label = new Label { Text = text };
        label.AddThemeColorOverride("font_color", color);
        label.AddThemeFontSizeOverride("font_size", size);
        if (font is not null)
            label.AddThemeFontOverride("font", font);
        if (bold)
            label.AddThemeConstantOverride("outline_size", 0);
        return label;
    }
}

internal partial class PawnRow : Button
{
    private const int RowHeight = 44;

    public EntityId PawnId { get; }

    private Panel     _avatar  = null!;
    private Label     _initials = null!;
    private Label     _name    = null!;
    private Label     _job     = null!;
    private ColorRect _moodBar = null!;
    private bool      _selected;

    public PawnRow(EntityId id)
    {
        PawnId = id;
    }

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(0, RowHeight);
        Flat              = true;
        ToggleMode        = false;
        AddThemeStyleboxOverride("normal",   MakeRowStyle(false));
        AddThemeStyleboxOverride("hover",    MakeRowStyle(false, hover: true));
        AddThemeStyleboxOverride("pressed",  MakeRowStyle(true));
        AddThemeStyleboxOverride("focus",    new StyleBoxEmpty());

        _avatar = new Panel
        {
            OffsetLeft = 4, OffsetTop = 4,
            OffsetRight = 32, OffsetBottom = 32
        };
        _avatar.AddThemeStyleboxOverride("panel", MakeAvatarStyle());
        AddChild(_avatar);

        _initials = ColonyPanel.MakeLabel("", 11, Palette.Text);
        _initials.AnchorLeft = 0; _initials.AnchorRight = 1;
        _initials.AnchorTop = 0;  _initials.AnchorBottom = 1;
        _initials.HorizontalAlignment = HorizontalAlignment.Center;
        _initials.VerticalAlignment   = VerticalAlignment.Center;
        _avatar.AddChild(_initials);

        _name = ColonyPanel.MakeLabel("", 12, Palette.Text, font: Fonts.CinzelRegular);
        _name.OffsetLeft  = 38; _name.OffsetTop = 4;
        _name.OffsetRight = -4;
        AddChild(_name);

        _job = ColonyPanel.MakeLabel("", 9, Palette.Muted);
        _job.OffsetLeft  = 38; _job.OffsetTop = 20;
        _job.OffsetRight = -4;
        AddChild(_job);

        _moodBar = new ColorRect
        {
            OffsetLeft = 38, OffsetRight = 74,
            OffsetTop  = 36, OffsetBottom = 39,
            Color      = Palette.MoodOk
        };
        AddChild(_moodBar);
    }

    public void SetData(string name, string jobLabel, float mood)
    {
        _name.Text     = name;
        _job.Text      = jobLabel;
        _initials.Text = MakeInitials(name);
        _moodBar.Color = MoodColor(mood);
    }

    public void SetSelected(bool selected)
    {
        if (_selected == selected) return;
        _selected = selected;
        AddThemeStyleboxOverride("normal", MakeRowStyle(selected));
    }

    private static string MakeInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "??";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "??";
        if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
        return $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();
    }

    private static Color MoodColor(float mood01)
    {
        if (mood01 < 0.25f) return Palette.Critical;
        if (mood01 < 0.45f) return Palette.Bad;
        if (mood01 < 0.65f) return Palette.Neutral;
        return Palette.Good;
    }

    private static StyleBoxFlat MakeRowStyle(bool selected, bool hover = false)
    {
        Color bg = selected ? Palette.RowSelected
                  : hover    ? Palette.RowHover
                             : Palette.CardBg;
        return new StyleBoxFlat
        {
            BgColor          = bg,
            BorderWidthLeft  = selected ? 2 : 0,
            BorderColor      = Palette.Gold
        };
    }

    private static StyleBoxTexture MakeAvatarStyle() => UiTheme.MakeInsetBox();
}

internal static class Palette
{
    public static readonly Color PanelBg    = new(0.102f, 0.094f, 0.078f); // #1a1814
    public static readonly Color CardBg     = new(0.129f, 0.122f, 0.106f); // #211f1b
    public static readonly Color Border     = new(0.239f, 0.220f, 0.188f); // #3d3830
    public static readonly Color Text       = new(0.784f, 0.722f, 0.604f); // #c8b89a
    public static readonly Color Muted      = new(0.478f, 0.431f, 0.384f); // #7a6e62
    public static readonly Color Gold       = new(0.545f, 0.451f, 0.251f); // #8b7340
    public static readonly Color Critical   = new(0.545f, 0.125f, 0.125f); // #8b2020
    public static readonly Color Bad        = new(0.478f, 0.310f, 0.102f); // #7a4f1a
    public static readonly Color Neutral    = new(0.290f, 0.290f, 0.290f); // #4a4a4a
    public static readonly Color Good       = new(0.176f, 0.353f, 0.118f); // #2d5a1e
    public static readonly Color RowHover   = new(0.150f, 0.140f, 0.120f);
    public static readonly Color RowSelected= new(0.165f, 0.145f, 0.125f); // #2a2520
    public static readonly Color SkillElite = new(0.325f, 0.227f, 0.718f); // #533AB7
    public static readonly Color MoodOk     = new(0.290f, 0.290f, 0.290f);
}
