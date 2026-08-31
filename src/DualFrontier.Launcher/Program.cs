using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Distribution;
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
/// substrate (<see cref="Runtime.Runtime"/>) + the engine session
/// (<see cref="EngineSession"/> via <see cref="EngineComposer"/>, whose content
/// arrives from the distribution manifest's root mods) +
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
        // === Distribution ===
        // The manifest is read BEFORE the runtime is composed, so a malformed one fails before
        // a Vulkan device exists rather than after. It is located by walking upward from the
        // directory holding this binary, never from the working directory: in a published
        // layout the walk stops immediately because the manifest sits beside the executable,
        // and in the repository it climbs to the root. The directory it was FOUND in is the
        // distribution root, and the composer derives the mods root from it.
        string manifestPath = DistributionManifestLoader.Locate();
        DistributionManifest manifest = DistributionManifestLoader.Load(manifestPath);
        string distributionRoot = DistributionManifestLoader.RootFor(manifestPath);

        // The distribution's content assemblies ship beside this executable but are absent from
        // its dependency file, because the engine no longer references them -- which is the whole
        // point of the boundary cut. Teach the default context to find them before anything tries
        // to load a mod.
        DistributionAssemblyProbe.Install(distributionRoot);

        // === Composition ===
        // The manifest is the product's definition, so the product's NAME and its asset root come
        // from it rather than from literals here. Both were parsed and then ignored, which made
        // the loader's strictness theatre: it refused an unknown key and an absent field while the
        // values it accepted changed nothing.
        //
        // The asset root is handed to the runtime UNRESOLVED, not combined with the distribution
        // root. That is deliberate and it is the difference between a fix and an outage: no build
        // step places an assets tree beside the executable, so an anchored path would name a
        // directory that does not exist, and the asset manager's rooted branch is an existence
        // check that throws before the window is ever shown. Passing the string through preserves
        // the manager's own contract -- absolute used as given, relative looked for beside the
        // working directory and then up the ancestors of the binary -- so the shipped manifest
        // resolves to exactly the directory it resolved to before, while editing the manifest now
        // genuinely changes which directory is loaded.
        //
        // Anchoring assets to the distribution root, as mods already are, is defensible and is a
        // PACKAGING change: it needs a step that copies the asset tree into the output first, and
        // must not land before that step exists.
        var runtimeOptions = new RuntimeOptions
        {
            Window = new WindowOptions
            {
                Title = manifest.Product.Name,
                Width = 1280,
                Height = 720,
            },
            // Exactly one root: the loader refuses any other count, so this cannot be empty.
            AssetsDirectory = manifest.AssetRoots[0],
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
        // The composer builds ENGINE parts only and then loads the manifest's root mod set. It
        // throws if any root mod is missing or refuses: a distribution without its root set is
        // not a degraded product, it is a broken one.
        using EngineSession session =
            EngineComposer.CreateSession(bridge, manifest, distributionRoot);

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
        // Device-loss boundary (M9 / D1): a VK_ERROR_DEVICE_LOST surfaced from inside RenderFrame is
        // caught here and routed to a structured fail-fast (ELT §4 device-lost class). Null hook =
        // production fail-fast; no recovery in v1 (device re-creation is future work).
        var deviceLoss = new DeviceLossBoundary();
        long frameIndex = 0;
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
            deviceLoss.RunGuarded(frameIndex++, () => renderer.RenderFrame(deltaSeconds));
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
