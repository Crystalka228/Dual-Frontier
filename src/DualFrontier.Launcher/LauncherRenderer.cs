using System;
using System.Collections.Generic;
using System.Numerics;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Rendering;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Production <see cref="IRenderer"/> implementation. Wraps Vulkan substrate
/// (<see cref="Runtime.Runtime"/>) + drains <see cref="PresentationBridge"/>
/// per frame, dispatching commands к <see cref="RenderCommandDispatcher"/>
/// (updates <see cref="SceneState"/>), then records + submits Vulkan sprite
/// frame from SceneState's active sprites.
///
/// К-extensions cascade #3 (2026-05-23): real visual implementation для pawn-3
/// dispatch arms via <see cref="Runtime.Runtime.RecordSpritesFrame"/> batched
/// API (V0.C.2 per S-LOCK-9). Camera2D default-configured к view a 200×200 tile
/// map (assumption documented in <see cref="Initialize"/>; cascade-#3-minimum-
/// scope choice; future cascade ratifies camera control mechanism).
///
/// Per-frame sync pattern mirrors SmokeTest reference (RunStressTest10K в
/// tests/DualFrontier.Runtime.SmokeTest/Program.cs): per-image renderFinished
/// semaphore (Vulkan binary semaphore reuse constraint) + shared imageAvailable
/// semaphore + frame fence (К-L7 atomic-from-observer simple form).
/// </summary>
internal sealed class LauncherRenderer : IRenderer, IDisposable
{
    /// <summary>
    /// Camera setup assumption: 200×200 tile map at 16 px/tile = 3200×3200 world
    /// units. Camera zoom auto-fits this к swapchain viewport. Future cascade
    /// replaces с domain-driven camera control.
    /// </summary>
    private const float AssumedMapWidthTiles = 200f;
    private const float AssumedMapHeightTiles = 200f;

    private readonly Runtime.Runtime _runtime;
    private readonly PresentationBridge _bridge;
    private readonly RenderCommandDispatcher _dispatcher;
    private readonly SceneState _sceneState;
    private readonly SpriteTexture _atlasTexture;

    private VulkanCommandBuffer? _commandBuffer;
    private VulkanSemaphore? _imageAvailable;
    private VulkanSemaphore[]? _renderFinishedPerImage;
    private VulkanFence? _frameFence;

    private bool _initialized;
    private bool _disposed;

    public LauncherRenderer(
        Runtime.Runtime runtime,
        PresentationBridge bridge,
        RenderCommandDispatcher dispatcher,
        SceneState sceneState,
        SpriteTexture atlasTexture)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _sceneState = sceneState ?? throw new ArgumentNullException(nameof(sceneState));
        _atlasTexture = atlasTexture ?? throw new ArgumentNullException(nameof(atlasTexture));
    }

    /// <summary>
    /// Initialize per-frame sync primitives + configure Camera2D defaults.
    /// Atlas texture composition happens в Program.Main() (passed via constructor)
    /// — caller owns atlasTexture lifetime.
    /// </summary>
    public void Initialize()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_initialized)
        {
            throw new InvalidOperationException("LauncherRenderer.Initialize called twice.");
        }

        // Camera2D: center on assumed-map-center, auto-zoom к fit viewport.
        // Cascade-#3-minimum-scope defaults; future cascade introduces camera control.
        float mapWorldWidth = AssumedMapWidthTiles * RenderCommandDispatcher.WorldUnitsPerTile;
        float mapWorldHeight = AssumedMapHeightTiles * RenderCommandDispatcher.WorldUnitsPerTile;
        float viewportWidth = _runtime.Swapchain.Width;
        float viewportHeight = _runtime.Swapchain.Height;

        _runtime.Camera.Position = new Vector2(mapWorldWidth * 0.5f, mapWorldHeight * 0.5f);
        _runtime.Camera.ViewportSize = new Vector2(viewportWidth, viewportHeight);
        _runtime.Camera.Zoom = MathF.Min(viewportWidth / mapWorldWidth, viewportHeight / mapWorldHeight);
        _runtime.Camera.Rotation = 0f;

        // Per-frame sync primitives. Per-image renderFinished semaphore (binary
        // semaphores cannot be reused while still pending в present).
        _commandBuffer = _runtime.GraphicsCommandPool.AllocateBuffer();
        _imageAvailable = new VulkanSemaphore(_runtime.VulkanDevice);
        _renderFinishedPerImage = new VulkanSemaphore[_runtime.Swapchain.ImageCount];
        for (int i = 0; i < _renderFinishedPerImage.Length; i++)
        {
            _renderFinishedPerImage[i] = new VulkanSemaphore(_runtime.VulkanDevice);
        }
        _frameFence = new VulkanFence(_runtime.VulkanDevice);

        _initialized = true;
    }

    /// <summary>
    /// Per-frame rendering: drain Bridge commands → dispatcher updates SceneState,
    /// then record + submit Vulkan sprite frame от SceneState active sprites.
    /// </summary>
    public void RenderFrame(double deltaSeconds)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_initialized)
        {
            throw new InvalidOperationException("LauncherRenderer.RenderFrame called before Initialize.");
        }

        // 1. Drain Bridge commands → dispatcher updates SceneState (pawn-3 active;
        //    PawnState/ItemSpawned/TickAdvanced silent stubs per S-LOCK-4 amended).
        _bridge.DrainCommands(_dispatcher.Dispatch);

        // 2. Acquire next swapchain image.
        uint imageIndex = _runtime.Swapchain.AcquireNextImage(
            _imageAvailable!.Handle, IntPtr.Zero, out bool outOfDate);
        if (outOfDate)
        {
            _runtime.VulkanDevice.WaitIdle();
            _runtime.Swapchain.Recreate((uint)_runtime.Window.Width, (uint)_runtime.Window.Height);
            _runtime.RecreateFramebuffersForSwapchain();
            // Recompute zoom + viewport on resize.
            float mapWorldWidth = AssumedMapWidthTiles * RenderCommandDispatcher.WorldUnitsPerTile;
            float mapWorldHeight = AssumedMapHeightTiles * RenderCommandDispatcher.WorldUnitsPerTile;
            float vw = _runtime.Swapchain.Width;
            float vh = _runtime.Swapchain.Height;
            _runtime.Camera.ViewportSize = new Vector2(vw, vh);
            _runtime.Camera.Zoom = MathF.Min(vw / mapWorldWidth, vh / mapWorldHeight);
            return;
        }

        // 3. Build sprite list from SceneState и record frame.
        var sprites = new List<Sprite>(_sceneState.ActivePawnCount);
        foreach (PawnSpriteEntry entry in _sceneState.EnumerateActiveSprites())
        {
            sprites.Add(new Sprite(
                Texture: _atlasTexture,
                Region: entry.Region,
                Transform: new SpriteTransform(
                    Position: entry.Position,
                    Scale: entry.Scale,
                    Rotation: 0f,
                    TintRgba: SpriteVertex.WhiteTint)));
        }

        _commandBuffer!.Reset();
        _commandBuffer.Begin(VkCommandBufferUsageFlagsPublic.OneTimeSubmit);
        _runtime.RecordSpritesFrame(
            _commandBuffer, (int)imageIndex, sprites, _runtime.Camera,
            clearColor: new Vector4(0.05f, 0.10f, 0.20f, 1.0f));
        _commandBuffer.End();

        // 4. Submit + present.
        IntPtr renderFinishedHandle = _renderFinishedPerImage![imageIndex].Handle;
        _commandBuffer.SubmitTo(
            _runtime.VulkanDevice.GraphicsQueue,
            waitSemaphore: _imageAvailable.Handle,
            waitStage: VkPipelineStageFlagsPublic.ColorAttachmentOutput,
            signalSemaphore: renderFinishedHandle,
            fence: _frameFence!);

        bool presentOutOfDate = _runtime.Swapchain.Present(
            _runtime.VulkanDevice.GraphicsQueue, renderFinishedHandle, imageIndex);
        if (presentOutOfDate)
        {
            _runtime.VulkanDevice.WaitIdle();
            _runtime.Swapchain.Recreate((uint)_runtime.Window.Width, (uint)_runtime.Window.Height);
            _runtime.RecreateFramebuffersForSwapchain();
            float mapWorldWidth = AssumedMapWidthTiles * RenderCommandDispatcher.WorldUnitsPerTile;
            float mapWorldHeight = AssumedMapHeightTiles * RenderCommandDispatcher.WorldUnitsPerTile;
            float vw = _runtime.Swapchain.Width;
            float vh = _runtime.Swapchain.Height;
            _runtime.Camera.ViewportSize = new Vector2(vw, vh);
            _runtime.Camera.Zoom = MathF.Min(vw / mapWorldWidth, vh / mapWorldHeight);
        }

        // 5. Wait + reset fence для next frame (К-L7 atomic-from-observer simple form).
        _frameFence!.Wait();
        _frameFence.Reset();
    }

    public void Shutdown()
    {
        if (_disposed) return;

        if (_initialized)
        {
            _runtime.VulkanDevice.WaitIdle();
            if (_renderFinishedPerImage is not null)
            {
                foreach (VulkanSemaphore s in _renderFinishedPerImage)
                {
                    s.Dispose();
                }
            }
            _frameFence?.Dispose();
            _imageAvailable?.Dispose();
            _commandBuffer?.Dispose();
        }
        _disposed = true;
    }

    public bool IsRunning => !_disposed && _runtime.Window.IsOpen;

    public void Dispose() => Shutdown();
}
