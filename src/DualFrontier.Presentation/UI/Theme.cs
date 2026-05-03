using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Centralized texture and font loaders for the Kenney + Cinzel themed UI.
/// Parallels <see cref="Palette"/>'s static-class pattern — exposes loaded
/// resources as static readonly fields, plus factory methods for
/// StyleBoxTexture configuration that panels/buttons consume via
/// AddThemeStyleboxOverride.
///
/// Operating principle: each loader fails fast if the underlying asset is
/// missing — no fallback to placeholder, no silent default. If a panel
/// can't load its texture, it shouldn't render with a "looks roughly
/// right" stand-in; it should fail loudly so the missing-asset condition
/// is visible.
/// </summary>
public static class KenneyTextures
{
    private const string PackPath = "res://assets/kenney/";

    public static readonly Texture2D PanelBrown        = LoadTexture("panel_brown.png");
    public static readonly Texture2D PanelInsetBrown   = LoadTexture("panelInset_brown.png");
    public static readonly Texture2D PanelBeige        = LoadTexture("panel_beige.png");
    public static readonly Texture2D ButtonLong        = LoadTexture("buttonLong_brown.png");
    public static readonly Texture2D ButtonLongPressed = LoadTexture("buttonLong_brown_pressed.png");
    public static readonly Texture2D BarBackLeft       = LoadTexture("barBack_horizontalLeft.png");
    public static readonly Texture2D BarBackMid        = LoadTexture("barBack_horizontalMid.png");
    public static readonly Texture2D BarBackRight      = LoadTexture("barBack_horizontalRight.png");
    public static readonly Texture2D IconCheckBeige    = LoadTexture("iconCheck_beige.png");
    public static readonly Texture2D IconCheckGrey     = LoadTexture("iconCheck_grey.png");

    private static Texture2D LoadTexture(string filename)
    {
        var tex = ResourceLoader.Load<Texture2D>(PackPath + filename);
        if (tex is null)
            throw new System.IO.FileNotFoundException(
                $"Required Kenney UI texture missing: {PackPath}{filename}. " +
                $"Verify file is present in src/DualFrontier.Presentation/assets/kenney/ and " +
                $"that Godot has imported it (open the editor once to trigger import).");
        return tex;
    }
}

public static class Fonts
{
    private const string FontPath = "res://assets/cinzel/";

    public static readonly FontFile CinzelBold    = LoadFont("Cinzel-Bold.otf");
    public static readonly FontFile CinzelRegular = LoadFont("Cinzel-Regular.otf");

    private static FontFile LoadFont(string filename)
    {
        var font = ResourceLoader.Load<FontFile>(FontPath + filename);
        if (font is null)
            throw new System.IO.FileNotFoundException(
                $"Required Cinzel font missing: {FontPath}{filename}. " +
                $"Verify file is present in src/DualFrontier.Presentation/assets/cinzel/ and " +
                $"that Godot has imported it (open the editor once to trigger import).");
        return font;
    }
}

public static class UiTheme
{
    /// <summary>Textured panel background for ColonyPanel, PawnDetail (warm brown).</summary>
    public static StyleBoxTexture MakeMainPanelBox() => MakePanelBox(KenneyTextures.PanelBrown);

    /// <summary>Textured panel background for ModMenuPanel modal (beige, visual differentiation from game HUD).</summary>
    public static StyleBoxTexture MakeModalPanelBox() => MakePanelBox(KenneyTextures.PanelBeige);

    /// <summary>Textured panel background for inset panels (avatars, cards).</summary>
    public static StyleBoxTexture MakeInsetBox() => MakePanelBox(KenneyTextures.PanelInsetBrown);

    /// <summary>Textured button — normal state.</summary>
    public static StyleBoxTexture MakeButtonNormalBox() => MakeButtonBox(KenneyTextures.ButtonLong);

    /// <summary>Textured button — pressed state.</summary>
    public static StyleBoxTexture MakeButtonPressedBox() => MakeButtonBox(KenneyTextures.ButtonLongPressed);

    /// <summary>Track backdrop for need bars (9-slice horizontal).</summary>
    public static StyleBoxTexture MakeBarBackBox()
    {
        var box = new StyleBoxTexture { Texture = KenneyTextures.BarBackMid };
        SetSliceMargins(box, left: 6, right: 6, top: 0, bottom: 0);
        SetContentMargins(box, all: 0);
        return box;
    }

    private static StyleBoxTexture MakePanelBox(Texture2D texture)
    {
        var box = new StyleBoxTexture { Texture = texture };
        SetSliceMargins(box, left: 12, right: 12, top: 12, bottom: 12);
        SetContentMargins(box, all: 0);
        return box;
    }

    private static StyleBoxTexture MakeButtonBox(Texture2D texture)
    {
        var box = new StyleBoxTexture { Texture = texture };
        SetSliceMargins(box, left: 6, right: 6, top: 6, bottom: 6);
        SetContentMargins(box, all: 4);
        return box;
    }

    private static void SetSliceMargins(StyleBoxTexture box, int left, int right, int top, int bottom)
    {
        box.TextureMarginLeft   = left;
        box.TextureMarginRight  = right;
        box.TextureMarginTop    = top;
        box.TextureMarginBottom = bottom;
    }

    private static void SetContentMargins(StyleBoxTexture box, int all)
    {
        box.ContentMarginLeft   = all;
        box.ContentMarginRight  = all;
        box.ContentMarginTop    = all;
        box.ContentMarginBottom = all;
    }
}
