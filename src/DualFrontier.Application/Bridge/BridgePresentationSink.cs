using System;
using DualFrontier.Application.Bridge.Commands;

namespace DualFrontier.Application.Bridge;

/// <summary>
/// Production <see cref="IPresentationSink"/>: turns an SDK presentation call into
/// the matching <see cref="IRenderCommand"/> on the <see cref="PresentationBridge"/>.
/// One-way by construction (TechArch 11.9) — the domain enqueues from whatever thread
/// it is on, and the renderer's main thread drains.
///
/// <para>
/// This is the whole of the engine's presentation policy for W3: translate, enqueue,
/// return. It holds no state, so a mod cannot read presentation back out through it.
/// </para>
/// </summary>
internal sealed class BridgePresentationSink : IPresentationSink
{
    private readonly PresentationBridge _bridge;

    internal BridgePresentationSink(PresentationBridge bridge)
        => _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

    /// <inheritdoc />
    public void SetAmbientTint(float r, float g, float b, float strength)
        => _bridge.Enqueue(new AmbientTintCommand(r, g, b, strength));
}
