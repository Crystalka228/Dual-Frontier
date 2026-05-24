using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Rendering;

namespace DualFrontier.Launcher;

/// <summary>
/// Production <see cref="IRenderer"/> implementation. Wraps Vulkan substrate
/// (<see cref="Runtime.Runtime"/>) + drains <see cref="PresentationBridge"/>
/// per frame, dispatching commands к <see cref="RenderCommandDispatcher"/>.
///
/// К-extensions cascade #2 (2026-05-23): Infrastructure scaffold per Q-G-6 (b1).
/// Real visual command dispatching deferred к cascade #3 — current dispatcher
/// arms throw <see cref="NotImplementedException"/> per Lesson #N12 «Defensive
/// Reserved Stub Pattern» first application.
/// </summary>
internal sealed class LauncherRenderer : IRenderer, IDisposable
{
    private readonly Runtime.Runtime _runtime;
    private readonly PresentationBridge _bridge;
    private readonly RenderCommandDispatcher _dispatcher;
    private bool _disposed;

    public LauncherRenderer(Runtime.Runtime runtime, PresentationBridge bridge, RenderCommandDispatcher dispatcher)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    /// <summary>
    /// Initialization hook called once before main loop iteration begins.
    /// Cascade #2: <see cref="Runtime.Runtime.Create"/> constructs Vulkan stack
    /// при composition в Program.cs; no extra init needed at this layer.
    /// Cascade #3 may add sprite catalog initialization here.
    /// </summary>
    public void Initialize()
    {
        // No-op в cascade #2 — Runtime composition handles Vulkan init.
    }

    /// <summary>
    /// Per-frame rendering: drain <see cref="PresentationBridge"/> commands к
    /// <see cref="RenderCommandDispatcher"/>; cascade #3 will add actual Vulkan
    /// sprite recording here.
    /// </summary>
    public void RenderFrame(double deltaSeconds)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Drain Bridge commands → dispatch к Runtime primitives via dispatcher.
        // Cascade #2: dispatcher arms throw NotImplementedException для visual
        // dispatch paths (Lesson #N12). Test enqueuing should verify dispatcher
        // receives command type без invoking the throw path.
        _bridge.DrainCommands(_dispatcher.Dispatch);

        // Frame Vulkan recording — cascade #3 territory.
        // К-extensions cascade #3 will add:
        //   - BeginRenderPassForSprites
        //   - SpriteRenderer.BeginFrame/Submit/EndFrame
        //   - EndSpriteRenderPass
        //   driven by accumulated dispatcher state (sprite catalog, scene state)
    }

    public void Shutdown()
    {
        _disposed = true;
    }

    public bool IsRunning => !_disposed && _runtime.Window.IsOpen;

    public void Dispose() => Shutdown();
}
