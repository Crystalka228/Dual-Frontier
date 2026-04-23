using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Rendering;

namespace DualFrontier.Presentation.Native;

/// <summary>
/// Native <see cref="IRenderer"/> stub. Full implementation lands in Phase 5+
/// when Silk.NET window creation, SpriteBatch, and tilemap rendering arrive.
/// For now this class documents the expected surface area and compiles as
/// part of the solution so that the Application contracts can be referenced.
/// </summary>
public sealed class NativeRenderer : IRenderer
{
    private readonly PresentationBridge _bridge;

    public NativeRenderer(PresentationBridge bridge)
    {
        _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
    }

    /// <inheritdoc />
    public bool IsRunning => false; // flips true once Initialize is implemented

    /// <inheritdoc />
    public void Initialize()
        => throw new NotImplementedException("Phase 5+: create Silk.NET window and GL context.");

    /// <inheritdoc />
    public void RenderFrame(double deltaSeconds)
        => throw new NotImplementedException("Phase 5+: drain bridge, batch sprites, draw.");

    /// <inheritdoc />
    public void Shutdown()
        => throw new NotImplementedException("Phase 5+: release GL resources and close window.");
}
