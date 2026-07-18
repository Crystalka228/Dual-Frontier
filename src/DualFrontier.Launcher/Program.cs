using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using DualFrontier.Runtime;
using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Launcher;

/// <summary>
/// Production launcher entry point for Dual Frontier. Composes Vulkan
/// substrate (<see cref="Runtime.Runtime"/>) + Domain layer
/// (<see cref="EngineSession"/> via <see cref="GameBootstrap"/>) +
/// <see cref="LauncherRenderer"/> bridge between them. Drives main loop
/// per Q-G-7 (d) hybrid orchestration (cascade #2 amendment Crystalka
/// Option A — GameLoop self-ticks on background thread).
///
/// К-extensions cascade #3 (2026-05-23): composition extended к include
/// atlas texture upload (LauncherProceduralAtlas → VulkanImage → SpriteTexture)
/// + SceneState composition root + dispatcher/renderer constructor injection
/// per S-LOCK-10.
/// </summary>
internal static class Program
{
    public static int Main(string[] args)
    {
        // === Composition ===
        var runtimeOptions = new RuntimeOptions
        {
            Window = new WindowOptions
            {
                Title = "Dual Frontier",
                Width = 1280,
                Height = 720,
            },
            AssetsDirectory = "assets",
            // EnableValidationLayer: omitted к use RuntimeOptions DEBUG/Release
            // conditional default (#if DEBUG = true, else = false).
        };
        using var runtime = Runtime.Runtime.Create(runtimeOptions);

        // Generate procedural atlas + upload к device-local memory.
        // S-LOCK-2 satisfied: no substrate touch; LauncherProceduralAtlas is
        // production-side copy (Q-H-17 Option C) preserving substrate isolation.
        PngImage atlasImage = LauncherProceduralAtlas.GenerateAtlas();
        VulkanImage atlasVkImage = VulkanImage.CreateFromPngImage(
            runtime.VulkanDevice, runtime.MemoryAllocator, runtime.TextureUploader, atlasImage);
        var atlasSampler = new VulkanSampler(runtime.VulkanDevice);
        using var atlasTexture = new SpriteTexture(atlasVkImage, atlasSampler);

        var bridge = new PresentationBridge();
        using EngineSession session = GameBootstrap.CreateSession(bridge);

        // S-LOCK-10 composition root: SceneState constructed here, passed к
        // both dispatcher (writes) и renderer (reads) via constructor injection.
        var sceneState = new SceneState();
        var dispatcher = new RenderCommandDispatcher(sceneState);
        using var renderer = new LauncherRenderer(runtime, bridge, dispatcher, sceneState, atlasTexture);

        // === Lifecycle init ===
        renderer.Initialize();
        runtime.Window.Show();
        session.Loop.Start();

        // === Main loop (Q-G-7 (d) hybrid orchestration, cascade #2 Crystalka Option A amendment) ===
        var lastFrameTime = DateTime.UtcNow;
        while (runtime.Window.IsOpen)
        {
            var now = DateTime.UtcNow;
            var deltaSeconds = (now - lastFrameTime).TotalSeconds;
            lastFrameTime = now;

            // 1. Pump Windows messages (surfaces input events к InputQueue).
            runtime.Window.PumpMessages();

            // 2. Drain InputQueue → forward к Application.
            //    Future cascade — InputBridge wiring TBD; events discarded for now.
            while (runtime.InputQueue.TryDequeue(out IInputEvent? _))
            {
                // Future cascade will forward к Application input bridge here.
            }

            // 3. (No simulation tick here — GameLoop self-ticks on background thread
            //    via Loop.Start() above. Cross-thread communication через
            //    PresentationBridge command queue.)

            // 4. Render frame (drain bridge + dispatch к SceneState + Vulkan record + present).
            renderer.RenderFrame(deltaSeconds);
        }

        // === Shutdown transaction (RESOURCE_OWNERSHIP_AND_LIFETIME 4.4 / CONCURRENCY 6.2) ===
        // Fence the sim + tear down engine state FIRST (the session's transaction:
        // bounded checked join + pipeline quiescence, then mods -> bus -> world),
        // THEN the GPU (renderer.Shutdown), THEN the device + window (the
        // using-unwind of renderer/atlasTexture/runtime). The self-contained fence
        // proves quiescence before world disposal, so this order closes the CMM
        // section 6.1 "WaitIdle while T2 still dispatching" race.
        session.Dispose();
        renderer.Shutdown();
        return 0;
    }
}
