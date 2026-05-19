using System.Numerics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 orthographic 2D camera per S-LOCK-4 (Q3b standard scope ratification).
/// Position + zoom + rotation + viewport + matrices + transforms. ~150 LOC.
///
/// View matrix: inverse of camera world transform — applied scale × rotate × translate
/// в matrix multiplication right-to-left order (System.Numerics row-major convention).
/// Projection matrix: orthographic from viewport dimensions; Vulkan NDC +Y down.
/// Composed ViewProjectionMatrix consumed via push constant by sprite vertex shader.
///
/// Out of scope per S-LOCK-4 «features only on demand»:
///   - Culling helpers (IsVisible(AABB))
///   - Interpolation for smooth transitions
///   - Bounded mode (clamp к world bounds)
/// Extends Camera2D when measurement justifies (Lesson #25).
/// </summary>
public sealed class Camera2D
{
    /// <summary>World-space position of camera center.</summary>
    public Vector2 Position { get; set; }

    /// <summary>Zoom factor. 1.0 = no zoom; 2.0 = 2× zoom (objects appear larger).</summary>
    public float Zoom { get; set; } = 1.0f;

    /// <summary>Rotation angle в radians around camera position.</summary>
    public float Rotation { get; set; }

    /// <summary>Viewport size в pixels (typically window framebuffer dimensions).</summary>
    public Vector2 ViewportSize { get; set; } = new(1280, 720);

    /// <summary>
    /// View matrix: inverse of camera world transform.
    /// translate(-position) × rotate(-rotation) × scale(zoom)
    /// </summary>
    public Matrix4x4 ViewMatrix
    {
        get
        {
            Matrix4x4 translate = Matrix4x4.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f));
            Matrix4x4 rotate = Matrix4x4.CreateRotationZ(-Rotation);
            Matrix4x4 scale = Matrix4x4.CreateScale(Zoom, Zoom, 1.0f);
            // System.Numerics row-major: M = A * B applies A first then B.
            // We want: world → translated → rotated → scaled → view
            return translate * rotate * scale;
        }
    }

    /// <summary>
    /// Orthographic projection matrix. Vulkan NDC: +X right, +Y down.
    /// CreateOrthographicOffCenter(left, right, bottom, top, near, far) maps:
    ///   bottom argument → NDC Y = -1
    ///   top argument → NDC Y = +1
    /// For Vulkan +Y down NDC, world +Y (visually bottom of screen) maps к NDC +1.
    /// So bottom = -halfHeight, top = +halfHeight (standard numeric ordering bottom &lt; top).
    /// </summary>
    public Matrix4x4 ProjectionMatrix
    {
        get
        {
            float halfWidth = ViewportSize.X * 0.5f;
            float halfHeight = ViewportSize.Y * 0.5f;
            return Matrix4x4.CreateOrthographicOffCenter(
                left: -halfWidth, right: halfWidth,
                bottom: -halfHeight, top: halfHeight,
                zNearPlane: -1.0f, zFarPlane: 1.0f);
        }
    }

    /// <summary>Composed view × projection matrix. Consumed as push constant.</summary>
    public Matrix4x4 ViewProjectionMatrix => ViewMatrix * ProjectionMatrix;

    /// <summary>Transform world position к screen pixel coordinates.</summary>
    public Vector2 WorldToScreen(Vector2 worldPos)
    {
        Vector4 worldVec = new(worldPos.X, worldPos.Y, 0f, 1f);
        Vector4 clipVec = Vector4.Transform(worldVec, ViewProjectionMatrix);
        // Clip → NDC (ortho ensures w = 1, but general form preserved).
        float ndcX = clipVec.X / clipVec.W;
        float ndcY = clipVec.Y / clipVec.W;
        // NDC → screen pixel (Vulkan NDC range [-1, +1]).
        return new Vector2(
            (ndcX + 1f) * 0.5f * ViewportSize.X,
            (ndcY + 1f) * 0.5f * ViewportSize.Y);
    }

    /// <summary>Transform screen pixel coordinates к world position.</summary>
    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        // Screen → NDC.
        float ndcX = screenPos.X / ViewportSize.X * 2f - 1f;
        float ndcY = screenPos.Y / ViewportSize.Y * 2f - 1f;
        // NDC → world via inverse(ViewProjection).
        if (!Matrix4x4.Invert(ViewProjectionMatrix, out Matrix4x4 invVP))
        {
            throw new InvalidOperationException("ViewProjectionMatrix is not invertible.");
        }
        Vector4 ndcVec = new(ndcX, ndcY, 0f, 1f);
        Vector4 worldVec = Vector4.Transform(ndcVec, invVP);
        return new Vector2(worldVec.X / worldVec.W, worldVec.Y / worldVec.W);
    }
}
