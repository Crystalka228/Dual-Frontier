using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Right-edge per-pawn detail card. Sections: header, mood, needs (4 bars),
/// current job pill, skills (3 demo skills with pip cells), bottom ornament.
/// Renders the most recent state for the currently selected pawn.
/// </summary>
public partial class PawnDetail : Panel
{
    private VBoxContainer _root      = null!;
    private Label         _initials  = null!;
    private Label         _nameLabel = null!;
    private Label         _roleLabel = null!;
    private Label         _moodValue = null!;
    private Label         _moodMood  = null!;
    private ColorRect     _moodBar   = null!;
    private NeedRow       _hunger    = null!;
    private NeedRow       _thirst    = null!;
    private NeedRow       _rest      = null!;
    private NeedRow       _comfort   = null!;
    private ColorRect     _jobDot    = null!;
    private Label         _jobLabel  = null!;
    private VBoxContainer _skillsBox = null!;

    private readonly Dictionary<EntityId, PawnState> _states = new();
    private EntityId? _shown;

    public override void _Ready()
    {
        AnchorLeft   = 1; AnchorRight = 1;
        AnchorTop    = 0; AnchorBottom = 1;
        OffsetLeft   = -260; OffsetRight = 0;
        OffsetTop    = 0;    OffsetBottom = 0;

        AddThemeStyleboxOverride("panel", MakePanelStyle());

        _root = new VBoxContainer
        {
            AnchorLeft = 0, AnchorRight = 1, AnchorTop = 0, AnchorBottom = 1,
            OffsetLeft = 16, OffsetRight = -16, OffsetTop = 16, OffsetBottom = -16
        };
        _root.AddThemeConstantOverride("separation", 12);
        AddChild(_root);

        BuildHeader();
        BuildMood();
        BuildNeeds();
        BuildJob();
        BuildSkills();
        BuildOrnament();
    }

    public void UpdatePawn(
        EntityId id, string name, float hunger, float thirst, float rest,
        float comfort, float mood, string jobLabel, bool jobUrgent)
    {
        _states[id] = new PawnState(name, hunger, thirst, rest, comfort, mood, jobLabel, jobUrgent);
        if (_shown is null) _shown = id;
        if (_shown.HasValue && _shown.Value.Equals(id))
            Render(_states[id], id);
    }

    public void ShowPawn(EntityId id)
    {
        _shown = id;
        if (_states.TryGetValue(id, out var s))
            Render(s, id);
    }

    private void Render(PawnState s, EntityId id)
    {
        _initials.Text  = MakeInitials(s.Name);
        _nameLabel.Text = s.Name.ToUpperInvariant();
        _roleLabel.Text = MakeRole(id);

        int moodPct = (int)MathF.Round(s.Mood * 100f);
        _moodValue.Text = moodPct.ToString();
        _moodMood.Text  = MoodLabel(s.Mood);
        _moodBar.Color  = StatusColor(s.Mood);
        _moodBar.CustomMinimumSize = new Vector2(BarWidth(s.Mood), 4);

        _hunger.Set(s.Hunger);
        _thirst.Set(s.Thirst);
        _rest.Set(s.Rest);
        _comfort.Set(s.Comfort);

        _jobDot.Color  = JobDotColor(s.JobLabel, s.JobUrgent);
        _jobLabel.Text = s.JobLabel;

        RenderSkills(id);
    }

    private void BuildHeader()
    {
        var headerBox = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        headerBox.AddThemeConstantOverride("separation", 10);
        _root.AddChild(headerBox);

        var avatar = new Panel
        {
            CustomMinimumSize = new Vector2(48, 48)
        };
        avatar.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor          = Palette.CardBg,
            BorderWidthLeft   = 1, BorderWidthRight = 1,
            BorderWidthTop    = 1, BorderWidthBottom = 1,
            BorderColor      = Palette.Border
        });
        headerBox.AddChild(avatar);

        _initials = ColonyPanel.MakeLabel("??", 18, Palette.Text);
        _initials.AnchorLeft = 0; _initials.AnchorRight = 1;
        _initials.AnchorTop  = 0; _initials.AnchorBottom = 1;
        _initials.HorizontalAlignment = HorizontalAlignment.Center;
        _initials.VerticalAlignment   = VerticalAlignment.Center;
        avatar.AddChild(_initials);

        var titleBox = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        headerBox.AddChild(titleBox);

        _nameLabel = ColonyPanel.MakeLabel("—", 14, Palette.Text);
        titleBox.AddChild(_nameLabel);

        _roleLabel = ColonyPanel.MakeLabel("инквизитор", 11, Palette.Muted);
        titleBox.AddChild(_roleLabel);
    }

    private void BuildMood()
    {
        AddSectionTitle("НАСТРОЕНИЕ");

        var row = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        row.AddThemeConstantOverride("separation", 12);
        _root.AddChild(row);

        _moodValue = ColonyPanel.MakeLabel("0", 24, Palette.Text);
        row.AddChild(_moodValue);

        var col = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        row.AddChild(col);

        _moodMood = ColonyPanel.MakeLabel("—", 11, Palette.Muted);
        col.AddChild(_moodMood);

        _moodBar = new ColorRect
        {
            CustomMinimumSize = new Vector2(0, 4),
            Color             = Palette.Neutral,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        col.AddChild(_moodBar);
    }

    private void BuildNeeds()
    {
        AddSectionTitle("НУЖДЫ");
        var box = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        box.AddThemeConstantOverride("separation", 6);
        _root.AddChild(box);

        _hunger  = new NeedRow("Голод");    box.AddChild(_hunger);
        _thirst  = new NeedRow("Жажда");    box.AddChild(_thirst);
        _rest    = new NeedRow("Усталость"); box.AddChild(_rest);
        _comfort = new NeedRow("Комфорт");  box.AddChild(_comfort);
    }

    private void BuildJob()
    {
        AddSectionTitle("ЗАДАЧА");
        var pill = new HBoxContainer();
        pill.AddThemeConstantOverride("separation", 8);
        _root.AddChild(pill);

        _jobDot = new ColorRect
        {
            CustomMinimumSize = new Vector2(8, 8),
            Color             = Palette.Neutral
        };
        pill.AddChild(_jobDot);

        _jobLabel = ColonyPanel.MakeLabel("—", 12, Palette.Text);
        pill.AddChild(_jobLabel);
    }

    private void BuildSkills()
    {
        AddSectionTitle("НАВЫКИ");
        _skillsBox = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _skillsBox.AddThemeConstantOverride("separation", 4);
        _root.AddChild(_skillsBox);
    }

    private void BuildOrnament()
    {
        var orn = ColonyPanel.MakeLabel("✦  ✦  ✦", 12, Palette.Border);
        orn.HorizontalAlignment = HorizontalAlignment.Center;
        _root.AddChild(orn);
    }

    private void AddSectionTitle(string text)
    {
        var l = ColonyPanel.MakeLabel(text, 9, Palette.Muted);
        _root.AddChild(l);
    }

    private void RenderSkills(EntityId id)
    {
        foreach (var child in _skillsBox.GetChildren())
            child.QueueFree();

        var demo = DemoSkills(id);
        foreach (var (name, level) in demo)
            _skillsBox.AddChild(new SkillRow(name, level));
    }

    private static (string Name, int Level)[] DemoSkills(EntityId id)
    {
        int seed = Math.Abs(id.Index);
        return new (string, int)[]
        {
            ("Construction", 4 + (seed       % 8)),
            ("Combat",       6 + ((seed * 3) % 9)),
            ("Crafting",     2 + ((seed * 7) % 11))
        };
    }

    private static string MakeInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "??";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "??";
        if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
        return $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();
    }

    private static string MakeRole(EntityId id)
    {
        string[] roles = { "инквизитор", "сержант", "техно-жрец", "магус", "санитар" };
        return roles[Math.Abs(id.Index) % roles.Length];
    }

    private static string MoodLabel(float mood)
    {
        if (mood < 0.26f) return "На грани срыва";
        if (mood < 0.46f) return "Подавлен";
        if (mood < 0.66f) return "Сносно";
        if (mood < 0.81f) return "Доволен";
        return "Воодушевлён";
    }

    private static Color StatusColor(float v)
    {
        if (v < 0.25f) return Palette.Critical;
        if (v < 0.45f) return Palette.Bad;
        if (v < 0.65f) return Palette.Neutral;
        return Palette.Good;
    }

    private static Color JobDotColor(string jobLabel, bool urgent)
    {
        if (urgent) return Palette.Critical;
        if (string.Equals(jobLabel, "Бездействие", StringComparison.Ordinal))
            return Palette.Neutral;
        return Palette.Good;
    }

    private static float BarWidth(float v) => Math.Clamp(v, 0f, 1f);

    private static StyleBoxFlat MakePanelStyle() => new()
    {
        BgColor          = Palette.PanelBg,
        BorderWidthLeft  = 1,
        BorderColor      = Palette.Border
    };

    private readonly record struct PawnState(
        string Name, float Hunger, float Thirst, float Rest, float Comfort,
        float Mood, string JobLabel, bool JobUrgent);
}

internal partial class NeedRow : HBoxContainer
{
    private const int LabelWidth = 64;
    private readonly string _name;

    private Label     _label = null!;
    private ColorRect _track = null!;
    private ColorRect _fill  = null!;
    private Label     _pct   = null!;

    public NeedRow(string name)
    {
        _name = name;
    }

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 6);

        _label = ColonyPanel.MakeLabel(_name, 11, Palette.Muted);
        _label.CustomMinimumSize = new Vector2(LabelWidth, 0);
        AddChild(_label);

        var bar = new Control
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            CustomMinimumSize   = new Vector2(0, 8)
        };
        AddChild(bar);

        _track = new ColorRect
        {
            AnchorLeft = 0, AnchorRight = 1,
            AnchorTop  = 0, AnchorBottom = 1,
            Color      = Palette.CardBg
        };
        bar.AddChild(_track);

        _fill = new ColorRect
        {
            AnchorLeft = 0, AnchorRight = 0,
            AnchorTop  = 0, AnchorBottom = 1,
            OffsetRight = 0,
            Color       = Palette.Neutral
        };
        bar.AddChild(_fill);

        _pct = ColonyPanel.MakeLabel("0%", 10, Palette.Muted);
        _pct.CustomMinimumSize = new Vector2(32, 0);
        AddChild(_pct);
    }

    public void Set(float v)
    {
        v = Math.Clamp(v, 0f, 1f);
        _pct.Text = $"{(int)MathF.Round(v * 100f)}%";
        _fill.AnchorRight = v;
        _fill.Color       = StatusColor(v);
    }

    private static Color StatusColor(float v)
    {
        if (v < 0.25f) return Palette.Critical;
        if (v < 0.45f) return Palette.Bad;
        if (v < 0.65f) return Palette.Neutral;
        return Palette.Good;
    }
}

internal partial class SkillRow : HBoxContainer
{
    private const int LabelWidth = 72;
    private const int PipCount   = 15;
    private const int PipSize    = 8;

    private readonly string _name;
    private readonly int    _level;

    public SkillRow(string name, int level)
    {
        _name  = name;
        _level = level;
    }

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 4);

        var label = ColonyPanel.MakeLabel(_name, 11, Palette.Muted);
        label.CustomMinimumSize = new Vector2(LabelWidth, 0);
        AddChild(label);

        Color pipColor = PipColor(_level);
        for (int i = 0; i < PipCount; i++)
        {
            var pip = new ColorRect
            {
                CustomMinimumSize = new Vector2(PipSize, PipSize),
                Color             = i < _level ? pipColor : Palette.CardBg
            };
            AddChild(pip);
        }

        var num = ColonyPanel.MakeLabel(_level.ToString(), 11, Palette.Text);
        AddChild(num);
    }

    private static Color PipColor(int level)
    {
        if (level <= 5)  return Palette.Neutral;
        if (level <= 9)  return Palette.Good;
        return Palette.SkillElite;
    }
}
