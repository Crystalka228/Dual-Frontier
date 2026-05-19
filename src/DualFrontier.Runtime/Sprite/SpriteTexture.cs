using DualFrontier.Runtime.Graphics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Texture handle: <see cref="VulkanImage"/> + <see cref="VulkanSampler"/> ownership wrapper.
/// Disposing the SpriteTexture disposes both contained handles. Caller transfers ownership
/// of image + sampler к the SpriteTexture; do not dispose them separately.
///
/// Future V0.C.2/V1: separate handle с texture caching + reference counting will replace this.
/// V0.C.1 single-sprite test creates one SpriteTexture с the Kenney pawn.
/// </summary>
public sealed class SpriteTexture : IDisposable
{
    public VulkanImage Image { get; }
    public VulkanSampler Sampler { get; }
    public int Width => (int)Image.Width;
    public int Height => (int)Image.Height;

    private bool _disposed;

    public SpriteTexture(VulkanImage image, VulkanSampler sampler)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(sampler);
        Image = image;
        Sampler = sampler;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        Image.Dispose();
        Sampler.Dispose();
        _disposed = true;
    }
}
