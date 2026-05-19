using System.Numerics;
using DualFrontier.Runtime.Sprite;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// Camera2D tests per V0.C.2 S-LOCK-4 (Q3b standard scope ratification).
/// Verifies matrix math, world-screen round trips, zoom + position effects.
/// </summary>
public sealed class Camera2DTests
{
    private const float Tolerance = 0.001f;

    [Fact]
    public void Default_Camera_Has_Sensible_Properties()
    {
        var cam = new Camera2D();
        cam.Position.Should().Be(Vector2.Zero);
        cam.Zoom.Should().Be(1.0f);
        cam.Rotation.Should().Be(0f);
        cam.ViewportSize.Should().Be(new Vector2(1280, 720));
    }

    [Fact]
    public void WorldToScreen_At_Origin_Returns_Viewport_Center()
    {
        var cam = new Camera2D { ViewportSize = new Vector2(1280, 720) };
        Vector2 screen = cam.WorldToScreen(Vector2.Zero);
        screen.X.Should().BeApproximately(640f, Tolerance);
        screen.Y.Should().BeApproximately(360f, Tolerance);
    }

    [Fact]
    public void WorldToScreen_ScreenToWorld_RoundTrip()
    {
        var cam = new Camera2D
        {
            Position = new Vector2(100, 50),
            Zoom = 1.5f,
            ViewportSize = new Vector2(800, 600),
        };
        Vector2 originalWorld = new(123, 456);
        Vector2 screen = cam.WorldToScreen(originalWorld);
        Vector2 backToWorld = cam.ScreenToWorld(screen);
        backToWorld.X.Should().BeApproximately(originalWorld.X, 0.01f);
        backToWorld.Y.Should().BeApproximately(originalWorld.Y, 0.01f);
    }

    [Fact]
    public void Position_Translates_View()
    {
        // World position equal к camera position appears at viewport center.
        var cam = new Camera2D
        {
            Position = new Vector2(100, 50),
            ViewportSize = new Vector2(800, 600),
        };
        Vector2 screen = cam.WorldToScreen(new Vector2(100, 50));
        screen.X.Should().BeApproximately(400f, Tolerance);
        screen.Y.Should().BeApproximately(300f, Tolerance);
    }

    [Fact]
    public void Zoom_Affects_WorldToScreen_Mapping()
    {
        var cam1 = new Camera2D { ViewportSize = new Vector2(800, 600), Zoom = 1.0f };
        var cam2 = new Camera2D { ViewportSize = new Vector2(800, 600), Zoom = 2.0f };
        Vector2 worldPos = new(100, 0);
        Vector2 screen1 = cam1.WorldToScreen(worldPos);
        Vector2 screen2 = cam2.WorldToScreen(worldPos);
        // Zoom 2× → world position appears twice as far from center
        float offset1 = screen1.X - 400f;
        float offset2 = screen2.X - 400f;
        offset2.Should().BeApproximately(offset1 * 2f, 0.01f);
    }

    [Fact]
    public void ViewMatrix_Identity_With_Default_Camera()
    {
        var cam = new Camera2D { Position = Vector2.Zero, Zoom = 1.0f, Rotation = 0f };
        Matrix4x4 view = cam.ViewMatrix;
        view.Should().Be(Matrix4x4.Identity);
    }

    [Fact]
    public void ProjectionMatrix_Maps_HalfWidth_To_NDC_One()
    {
        var cam = new Camera2D { ViewportSize = new Vector2(800, 600) };
        Matrix4x4 proj = cam.ProjectionMatrix;
        // Point at (halfWidth, 0) should map к NDC (+1, 0)
        Vector4 v = new(400f, 0f, 0f, 1f);
        Vector4 clip = Vector4.Transform(v, proj);
        clip.X.Should().BeApproximately(1f, Tolerance);
        clip.Y.Should().BeApproximately(0f, Tolerance);
    }

    [Fact]
    public void ProjectionMatrix_Positive_Y_World_Maps_To_Positive_Y_NDC_For_Vulkan_Y_Down()
    {
        var cam = new Camera2D { ViewportSize = new Vector2(800, 600) };
        Matrix4x4 proj = cam.ProjectionMatrix;
        Vector4 v = new(0f, 300f, 0f, 1f);   // bottom of viewport in world space
        Vector4 clip = Vector4.Transform(v, proj);
        // For Vulkan +Y down: world +Y (down) → NDC +Y (down).
        clip.Y.Should().BeApproximately(1f, Tolerance);
    }

    [Fact]
    public void Setting_ViewportSize_Affects_Subsequent_Matrices()
    {
        var cam = new Camera2D { ViewportSize = new Vector2(800, 600) };
        Matrix4x4 proj1 = cam.ProjectionMatrix;
        cam.ViewportSize = new Vector2(1600, 1200);
        Matrix4x4 proj2 = cam.ProjectionMatrix;
        proj1.Should().NotBe(proj2);
    }

    [Fact]
    public void Rotation_Affects_World_To_Screen()
    {
        var cam = new Camera2D { ViewportSize = new Vector2(800, 600), Rotation = MathF.PI / 2f };
        // World point at (+X, 0) rotated 90° CCW should appear at (0, -Y) screen offset
        // Actual direction depends on rotation sign convention; just verify rotation actually changes mapping.
        Vector2 screenNoRot = new Camera2D { ViewportSize = new Vector2(800, 600), Rotation = 0f }.WorldToScreen(new Vector2(100, 0));
        Vector2 screenRot = cam.WorldToScreen(new Vector2(100, 0));
        screenNoRot.Should().NotBe(screenRot);
    }
}
