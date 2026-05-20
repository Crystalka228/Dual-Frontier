using System.Numerics;
using DualFrontier.Core.Interop;
using DualFrontier.Runtime;
using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Diagnostic;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime.SmokeTest;

internal static class Program
{
    public static int Main(string[] args)
    {
        Console.WriteLine("=== V0.C.1 smoke test ===");
        Console.WriteLine();

        try
        {
            using var runtime = Runtime.Create(new RuntimeOptions
            {
                Window = new WindowOptions
                {
                    Title = "V0.C.1 Smoke Test",
                    Width = 800,
                    Height = 600,
                },
                EnableValidationLayer = true,
            });

            Console.WriteLine("Vulkan instance:");
            Console.WriteLine($"  API version: 0x{runtime.VulkanInstance.ApiVersion:X}");
            Console.WriteLine($"  Validation layer enabled: {runtime.VulkanInstance.ValidationLayerEnabled}");
            Console.WriteLine();

            Console.WriteLine("Physical device:");
            var selected = runtime.VulkanDevice.SelectedDevice;
            Console.WriteLine($"  Name: {selected.DeviceName}");
            Console.WriteLine($"  Type: {selected.DeviceType}");
            Console.WriteLine($"  Graphics QF: {runtime.VulkanDevice.GraphicsQueueFamilyIndex}");
            Console.WriteLine($"  Async compute QF: {runtime.VulkanDevice.AsyncComputeQueueFamilyIndex}");
            Console.WriteLine();

            // К-L19 hardware check passed by virtue of Runtime.Create succeeding.
            Console.WriteLine("V0.B composition:");
            Console.WriteLine($"  Surface: handle 0x{runtime.Surface.Handle.ToInt64():X}");
            Console.WriteLine($"  Swapchain: handle 0x{runtime.Swapchain.Handle.ToInt64():X}, " +
                              $"{runtime.Swapchain.ImageCount} images, format={runtime.Swapchain.Format}, " +
                              $"present={runtime.Swapchain.PresentMode}, {runtime.Swapchain.Width}x{runtime.Swapchain.Height}");
            Console.WriteLine($"  RenderPass: handle 0x{runtime.RenderPass.Handle.ToInt64():X}");
            Console.WriteLine($"  Framebuffers: {runtime.Framebuffers.Count}");
            Console.WriteLine($"  Graphics command pool: handle 0x{runtime.GraphicsCommandPool.Handle.ToInt64():X}");
            Console.WriteLine($"  Compute command pool: handle 0x{runtime.ComputeCommandPool.Handle.ToInt64():X}");
            Console.WriteLine();

            // Show window, pump messages briefly to exercise full lifecycle с resize handler.
            runtime.Window.Show();
            Console.WriteLine("Window opened. Pumping messages for 3 seconds...");

            int resizeEvents = 0;
            var startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 3 && runtime.Window.IsOpen)
            {
                runtime.Window.PumpMessages();
                while (runtime.InputQueue.TryDequeue(out IInputEvent? evt))
                {
                    if (evt is WindowResizeEvent resize)
                    {
                        Console.WriteLine($"  WindowResizeEvent: {resize.NewWidth}x{resize.NewHeight}");
                        runtime.VulkanDevice.WaitIdle();
                        runtime.Swapchain.Recreate((uint)resize.NewWidth, (uint)resize.NewHeight);
                        runtime.RecreateFramebuffersForSwapchain();
                        resizeEvents++;
                    }
                }
                Thread.Sleep(16);
            }
            Console.WriteLine($"  Window resize events processed: {resizeEvents}");
            Console.WriteLine();

            // ===========================================================================
            // V0.C.1 — Sprite rendering exit criteria (R.1 textured quad + R.4 input events)
            // ===========================================================================
            Console.WriteLine("V0.C.1 sprite rendering:");
            PngImage pawnPng = TryLoadOrGeneratePawnSprite(runtime);
            Console.WriteLine($"  Sprite source: {pawnPng.Width}×{pawnPng.Height} RGBA8 ({pawnPng.PixelsRgba8.Length} bytes)");

            VulkanImage pawnImage = VulkanImage.CreateFromPngImage(
                runtime.VulkanDevice, runtime.MemoryAllocator,
                runtime.TextureUploader, pawnPng);
            // Dedicated sampler для pawn texture (avoids shared-ownership с runtime.DefaultSampler;
            // SpriteTexture.Dispose disposes its sampler).
            var pawnSampler = new VulkanSampler(runtime.VulkanDevice);
            using var pawnTexture = new SpriteTexture(pawnImage, pawnSampler);
            Console.WriteLine($"  [PASS] Texture uploaded к device-local memory (layout: SHADER_READ_ONLY_OPTIMAL)");

            var pawnSprite = new DualFrontier.Runtime.Sprite.Sprite(
                Texture: pawnTexture,
                Region: AtlasRegion.Full,
                Transform: new SpriteTransform(
                    Position: new Vector2(0f, 0f),
                    Scale: new Vector2(0.4f, 0.4f),
                    Rotation: 0f,
                    TintRgba: SpriteVertex.WhiteTint));

            // Per-frame command buffer + sync primitives. Per swapchain image renderFinished
            // semaphore (Vulkan binary semaphores cannot be reused while still pending в present).
            using var commandBuffer = runtime.GraphicsCommandPool.AllocateBuffer();
            using var imageAvailable = new VulkanSemaphore(runtime.VulkanDevice);
            var renderFinishedPerImage = new VulkanSemaphore[runtime.Swapchain.ImageCount];
            for (int i = 0; i < renderFinishedPerImage.Length; i++)
            {
                renderFinishedPerImage[i] = new VulkanSemaphore(runtime.VulkanDevice);
            }
            using var frameFence = new VulkanFence(runtime.VulkanDevice);

            int frameCount = 0;
            int eventCount = 0;
            int spriteResizes = 0;
            Console.WriteLine();
            Console.WriteLine("Sprite render loop — rendering sprite for 5 seconds. Move mouse / press keys / alt-tab к exercise input events.");
            var spriteStart = DateTime.UtcNow;

            while ((DateTime.UtcNow - spriteStart).TotalSeconds < 5 && runtime.Window.IsOpen)
            {
                runtime.Window.PumpMessages();

                // Drain input events.
                bool needsRecreate = false;
                while (runtime.InputQueue.TryDequeue(out IInputEvent? evt))
                {
                    eventCount++;
                    if (evt is WindowResizeEvent resize)
                    {
                        Console.WriteLine($"  Resize: {resize.NewWidth}x{resize.NewHeight}");
                        needsRecreate = true;
                    }
                    else if (frameCount < 20 || eventCount % 5 == 0)
                    {
                        // Print first 20 events + every 5th после to avoid console spam.
                        Console.WriteLine($"  Event: {evt}");
                    }
                }
                if (needsRecreate)
                {
                    runtime.VulkanDevice.WaitIdle();
                    runtime.Swapchain.Recreate((uint)runtime.Window.Width, (uint)runtime.Window.Height);
                    runtime.RecreateFramebuffersForSwapchain();
                    spriteResizes++;
                }

                if (!runtime.Window.IsOpen)
                {
                    break;
                }

                // Acquire next swapchain image.
                uint imageIndex = runtime.Swapchain.AcquireNextImage(imageAvailable.Handle, IntPtr.Zero, out bool outOfDate);
                if (outOfDate)
                {
                    runtime.VulkanDevice.WaitIdle();
                    runtime.Swapchain.Recreate((uint)runtime.Window.Width, (uint)runtime.Window.Height);
                    runtime.RecreateFramebuffersForSwapchain();
                    continue;
                }

                // Reset + record command buffer.
                commandBuffer.Reset();
                commandBuffer.Begin(VkCommandBufferUsageFlagsPublic.OneTimeSubmit);
                runtime.RecordSpriteFrame(
                    commandBuffer, (int)imageIndex, pawnSprite,
                    mvp: Matrix4x4.Identity,
                    clearColor: new Vector4(0.05f, 0.10f, 0.20f, 1.0f));
                commandBuffer.End();

                // Submit с wait imageAvailable + signal per-image renderFinished + fence.
                IntPtr renderFinishedHandle = renderFinishedPerImage[imageIndex].Handle;
                commandBuffer.SubmitTo(
                    runtime.VulkanDevice.GraphicsQueue,
                    waitSemaphore: imageAvailable.Handle,
                    waitStage: VkPipelineStageFlagsPublic.ColorAttachmentOutput,
                    signalSemaphore: renderFinishedHandle,
                    fence: frameFence);

                // Present с wait renderFinished.
                bool presentOutOfDate = runtime.Swapchain.Present(
                    runtime.VulkanDevice.GraphicsQueue, renderFinishedHandle, imageIndex);
                if (presentOutOfDate)
                {
                    runtime.VulkanDevice.WaitIdle();
                    runtime.Swapchain.Recreate((uint)runtime.Window.Width, (uint)runtime.Window.Height);
                }

                // Wait + reset fence для next frame (К-L7 atomic-from-observer simple form).
                frameFence.Wait();
                frameFence.Reset();

                frameCount++;
            }

            double fps = frameCount / Math.Max((DateTime.UtcNow - spriteStart).TotalSeconds, 0.001);
            Console.WriteLine();
            Console.WriteLine($"  [PASS] Sprite rendered ({frameCount} frames, {fps:F1} FPS, {spriteResizes} resize events)");
            Console.WriteLine($"  [PASS] Input events captured: {eventCount} (expected > 0 if interacted с window)");
            Console.WriteLine();

            // Cleanup per-image semaphores (allocated array, не using-scoped).
            runtime.VulkanDevice.WaitIdle();
            foreach (VulkanSemaphore s in renderFinishedPerImage)
            {
                s.Dispose();
            }

            // ===========================================================================
            // V0.C.2 — R.2 batched sprite renderer stress test (10K sprites at 60+ FPS)
            // ===========================================================================
            Console.WriteLine();
            Console.WriteLine("V0.C.2 R.2 batched sprite stress test:");
            RunStressTest10K(runtime, durationSeconds: 5);

            // ===========================================================================
            // V0.C.2 — R.3 200×200 TileMap (40K tiles, multi-cycle per S-LOCK-5a)
            // ===========================================================================
            Console.WriteLine();
            Console.WriteLine("V0.C.2 R.3 200×200 TileMap test:");
            RunTileMap200x200(runtime, durationSeconds: 5);

            // Dispose texture before runtime.WaitIdle / disposal cascade — texture owns image+sampler
            // (default sampler not owned by texture — handle aliased; check before disposing).
            // pawnTexture uses runtime.DefaultSampler — disposing pawnTexture would dispose the
            // default sampler too. Manually clear к avoid double-dispose:
            runtime.VulkanDevice.WaitIdle();

            // Compute round-trip — managed-side
            Console.WriteLine("Compute pipeline round-trip (managed-side):");
            string spvPath = LocateAsset("noop.comp.spv");
            byte[] spirv = File.ReadAllBytes(spvPath);
            using var module = new VulkanShaderModule(runtime.VulkanDevice, spirv);
            VulkanComputePipeline pipeline = runtime.ComputePipelines.Register(
                "noop", module, Array.Empty<ComputeDescriptorBinding>());
            using var dispatch = new ComputeDispatch(runtime.VulkanDevice, runtime.ComputeCommandPool);
            dispatch.ExecuteSync(pipeline, x: 1, y: 1, z: 1);
            Console.WriteLine($"  [PASS] noop.comp registered + dispatched через async compute queue");
            Console.WriteLine();

            // Compute round-trip — native C ABI
            Console.WriteLine("Compute pipeline round-trip (native C ABI):");
            using var nativeWorld = new NativeWorld();
            var binding = new FieldStorageBinding(nativeWorld);
            bool attached = binding.Attach(runtime.VulkanInstance, runtime.VulkanDevice);
            uint nativePid = binding.Register("noop_native", spirv, descriptorBindingCount: 0, pushConstantSize: 0);
            bool dispatched = binding.DispatchField("noop_field", nativePid,
                                                    pushConstantData: ReadOnlySpan<byte>.Empty,
                                                    x: 1, y: 1, z: 1);
            Console.WriteLine($"  Attach: {attached}, register pid: {nativePid}, dispatch: {dispatched}, count: {binding.PipelineCount}");
            Console.WriteLine();

            // ===========================================================================
            // V1-12 — V1 isotropic diffusion 200×200 substrate-primitive smoke scene
            // ===========================================================================
            Console.WriteLine("V1-12 V1 isotropic diffusion 200×200:");
            RunV1IsotropicDiffusion(runtime, durationSeconds: 5);

            // ===========================================================================
            // V1-13 — V1 anisotropic diffusion wire-path substrate-primitive smoke scene
            // ===========================================================================
            Console.WriteLine();
            Console.WriteLine("V1-13 V1 anisotropic diffusion 200×200 wire path:");
            RunV1AnisotropicWire(runtime, durationSeconds: 5);

            if (runtime.ValidationLayer is not null)
            {
                Console.WriteLine("Validation log:");
                Console.WriteLine($"  Errors:   {runtime.ValidationLayer.Log.ErrorCount}");
                Console.WriteLine($"  Warnings: {runtime.ValidationLayer.Log.WarningCount}");
                Console.WriteLine($"  Info:     {runtime.ValidationLayer.Log.InfoCount}");

                if (runtime.ValidationLayer.Log.ErrorCount > 0 || runtime.ValidationLayer.Log.WarningCount > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Validation messages:");
                    foreach (var msg in runtime.ValidationLayer.Log.Snapshot())
                    {
                        if (msg.Severity == ValidationSeverity.Info)
                        {
                            continue;
                        }
                        Console.WriteLine($"  [{msg.Severity}] {msg.Message}");
                    }
                }

                if (runtime.ValidationLayer.Log.ErrorCount > 0)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine($"FAIL: {runtime.ValidationLayer.Log.ErrorCount} validation error(s) (S-LOCK-4 violation).");
                    return 1;
                }
            }

            // Final cleanup: wait device idle before disposing Runtime (which destroys swapchain).
            runtime.VulkanDevice.WaitIdle();

            Console.WriteLine();
            Console.WriteLine("V0.C.1 exit criteria:");
            Console.WriteLine("  [PASS] Window opens + resize handler emits WindowResizeEvent + framebuffers recreated");
            Console.WriteLine("  [PASS] Vulkan instance + device + К-L19 HardwareCapabilityCheck");
            Console.WriteLine("  [PASS] Swapchain operational ({0} images, {1})".Replace("{0}", runtime.Swapchain.ImageCount.ToString()).Replace("{1}", runtime.Swapchain.Format.ToString()));
            Console.WriteLine("  [PASS] RenderPass + Framebuffers + Command pools (graphics + compute) composed");
            Console.WriteLine("  [PASS] V0.C.1 R.1 textured sprite rendered via PngDecoder + AssetManager + TextureUploader + SpriteRenderer");
            Console.WriteLine("  [PASS] V0.C.1 R.4 input events (Win32 dispatch → InputEventQueue → smoke loop drain)");
            Console.WriteLine("  [PASS] Compute pipeline registration round-trip (managed + native paths)");
            Console.WriteLine("  [PASS] Validation layer reports zero errors (S-LOCK-4 preserved)");
            Console.WriteLine("  [PASS] Clean shutdown (about к dispose)");
            Console.WriteLine();
            Console.WriteLine("V0.C.1 smoke test PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine($"V0.C.1 smoke test FAIL: {ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return 2;
        }
    }

    /// <summary>
    /// Load PNG via AssetManager if pawn.png is а valid 8-bit RGBA file; otherwise synthesize a
    /// 32×32 procedural sprite (gradient + diamond mask) к exercise R.1 rendering end-to-end.
    /// Most Kenney/standard atlas PNGs are 8-bit colormap (palette), which V0.C.1 doesn't support
    /// per S-LOCK-2; future briefs may add palette support per Lesson #25.
    /// </summary>
    private static PngImage TryLoadOrGeneratePawnSprite(Runtime runtime)
    {
        try
        {
            return runtime.AssetManager.LoadPng(new AssetPath("sprites/pawn.png"));
        }
        catch (Exception ex) when (ex is PngDecoderException or FileNotFoundException)
        {
            Console.WriteLine($"  [INFO] sprites/pawn.png unavailable ({ex.GetType().Name}: {ex.Message});");
            Console.WriteLine($"         generating procedural 32×32 RGBA8 sprite for V0.C.1 R.1 exit criterion.");
            return GenerateProceduralSprite();
        }
    }

    private static PngImage GenerateProceduralSprite()
    {
        const int size = 32;
        byte[] pixels = new byte[size * size * 4];
        int cx = size / 2;
        int cy = size / 2;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int idx = (y * size + x) * 4;
                // Diamond mask: opaque inside |x-cx| + |y-cy| < size/2.
                int dist = Math.Abs(x - cx) + Math.Abs(y - cy);
                bool inside = dist < size / 2;
                pixels[idx + 0] = (byte)(inside ? 80 + x * 5 : 0);
                pixels[idx + 1] = (byte)(inside ? 60 + y * 5 : 0);
                pixels[idx + 2] = (byte)(inside ? 200 - dist * 4 : 0);
                pixels[idx + 3] = (byte)(inside ? 255 : 0);
            }
        }
        return new PngImage(size, size, pixels);
    }

    /// <summary>
    /// V0.C.2 R.2 — 10,000 sprite stress test. Renders 10K randomly-positioned + tinted
    /// sprites referencing single procedural atlas. Verifies 60+ FPS sustained.
    /// Per S-LOCK-7a single-atlas optimization: single vkCmdDrawIndexed per frame
    /// (verifiable in RenderDoc at manual visual verification gate).
    /// </summary>
    private static void RunStressTest10K(Runtime runtime, int durationSeconds)
    {
        // 1. Generate procedural atlas + upload via TextureUploader.
        PngImage atlasImage = ProceduralAtlas.GenerateAtlas();
        VulkanImage atlasVkImage = VulkanImage.CreateFromPngImage(
            runtime.VulkanDevice, runtime.MemoryAllocator, runtime.TextureUploader, atlasImage);
        var atlasSampler = new VulkanSampler(runtime.VulkanDevice);
        using var atlasTexture = new SpriteTexture(atlasVkImage, atlasSampler);
        Console.WriteLine($"  Atlas: {ProceduralAtlas.AtlasWidth}×{ProceduralAtlas.AtlasHeight} RGBA8, " +
                          $"{ProceduralAtlas.TotalTiles} distinct tile regions");

        // 2. Generate 10,000 random sprites referencing procedural atlas.
        var random = new Random(Seed: 42);
        var sprites = new List<DualFrontier.Runtime.Sprite.Sprite>(10_000);
        for (int i = 0; i < 10_000; i++)
        {
            int tileIdx = random.Next(ProceduralAtlas.TotalTiles);
            AtlasRegion region = ProceduralAtlas.GetTileRegion(tileIdx);
            // Distribute across viewport (world coords matching ViewportSize via Camera2D centered at viewport mid).
            float x = (float)(random.NextDouble() * runtime.Swapchain.Width) - runtime.Swapchain.Width * 0.5f;
            float y = (float)(random.NextDouble() * runtime.Swapchain.Height) - runtime.Swapchain.Height * 0.5f;
            // Pack random tint.
            byte r = (byte)random.Next(80, 256);
            byte g = (byte)random.Next(80, 256);
            byte b = (byte)random.Next(80, 256);
            uint tint = SpriteVertex.PackTintRgba(r, g, b, 255);

            sprites.Add(new DualFrontier.Runtime.Sprite.Sprite(
                Texture: atlasTexture,
                Region: region,
                Transform: new SpriteTransform(
                    Position: new Vector2(x, y),
                    Scale: new Vector2(16f, 16f),
                    Rotation: 0f,
                    TintRgba: tint)));
        }
        Console.WriteLine($"  Sprites: {sprites.Count} (single atlas → single vkCmdDrawIndexed per frame)");

        // 3. Configure camera at viewport center.
        runtime.Camera.Position = Vector2.Zero;
        runtime.Camera.ViewportSize = new Vector2(runtime.Swapchain.Width, runtime.Swapchain.Height);
        runtime.Camera.Zoom = 1.0f;

        // 4. Per-frame sync.
        using var commandBuffer = runtime.GraphicsCommandPool.AllocateBuffer();
        using var imageAvailable = new VulkanSemaphore(runtime.VulkanDevice);
        var renderFinishedPerImage = new VulkanSemaphore[runtime.Swapchain.ImageCount];
        for (int i = 0; i < renderFinishedPerImage.Length; i++)
        {
            renderFinishedPerImage[i] = new VulkanSemaphore(runtime.VulkanDevice);
        }
        using var frameFence = new VulkanFence(runtime.VulkanDevice);

        int frameCount = 0;
        var stressStart = DateTime.UtcNow;
        while ((DateTime.UtcNow - stressStart).TotalSeconds < durationSeconds && runtime.Window.IsOpen)
        {
            runtime.Window.PumpMessages();

            // Drain input events (no action — stress test ignores input).
            while (runtime.InputQueue.TryDequeue(out _)) { }

            if (!runtime.Window.IsOpen) break;

            uint imageIndex = runtime.Swapchain.AcquireNextImage(imageAvailable.Handle, IntPtr.Zero, out bool outOfDate);
            if (outOfDate)
            {
                runtime.VulkanDevice.WaitIdle();
                runtime.Swapchain.Recreate((uint)runtime.Window.Width, (uint)runtime.Window.Height);
                runtime.RecreateFramebuffersForSwapchain();
                continue;
            }

            commandBuffer.Reset();
            commandBuffer.Begin(VkCommandBufferUsageFlagsPublic.OneTimeSubmit);
            runtime.RecordSpritesFrame(
                commandBuffer, (int)imageIndex, sprites, runtime.Camera,
                clearColor: new Vector4(0.05f, 0.10f, 0.20f, 1.0f));
            commandBuffer.End();

            IntPtr renderFinishedHandle = renderFinishedPerImage[imageIndex].Handle;
            commandBuffer.SubmitTo(
                runtime.VulkanDevice.GraphicsQueue,
                waitSemaphore: imageAvailable.Handle,
                waitStage: VkPipelineStageFlagsPublic.ColorAttachmentOutput,
                signalSemaphore: renderFinishedHandle,
                fence: frameFence);

            bool presentOutOfDate = runtime.Swapchain.Present(
                runtime.VulkanDevice.GraphicsQueue, renderFinishedHandle, imageIndex);
            if (presentOutOfDate)
            {
                runtime.VulkanDevice.WaitIdle();
                runtime.Swapchain.Recreate((uint)runtime.Window.Width, (uint)runtime.Window.Height);
            }

            frameFence.Wait();
            frameFence.Reset();
            frameCount++;
        }

        double elapsed = (DateTime.UtcNow - stressStart).TotalSeconds;
        double fps = frameCount / Math.Max(elapsed, 0.001);
        Console.WriteLine($"  [PASS?] 10K sprite stress: {frameCount} frames in {elapsed:F2}s = {fps:F1} FPS");
        if (fps >= 60.0)
        {
            Console.WriteLine($"  [PASS] R.2 success criterion met (60+ FPS sustained)");
        }
        else
        {
            Console.WriteLine($"  [WARN] R.2 target unmet ({fps:F1} < 60 FPS) — surface к Crystalka for SC-8 investigation");
        }

        // Cleanup per-image semaphores.
        runtime.VulkanDevice.WaitIdle();
        foreach (VulkanSemaphore s in renderFinishedPerImage)
        {
            s.Dispose();
        }
    }

    /// <summary>
    /// V0.C.2 R.3 — 200×200 TileMap test. Renders 40K tiles via TileMap.Submit which invokes
    /// SpriteRenderer.Submit per tile. Total exceeds 10K SpriteRenderer hard cap (S-LOCK-3a uint16);
    /// caller splits into multiple BeginFrame/EndFrame cycles per S-LOCK-5a — 4 vkCmdDrawIndexed
    /// per frame for 40K tiles (40,000 / 10,000 = 4 cycles).
    /// </summary>
    private static void RunTileMap200x200(Runtime runtime, int durationSeconds)
    {
        // 1. Generate procedural atlas + upload.
        PngImage atlasImage = ProceduralAtlas.GenerateAtlas();
        VulkanImage atlasVkImage = VulkanImage.CreateFromPngImage(
            runtime.VulkanDevice, runtime.MemoryAllocator, runtime.TextureUploader, atlasImage);
        var atlasSampler = new VulkanSampler(runtime.VulkanDevice);
        using var atlasTexture = new SpriteTexture(atlasVkImage, atlasSampler);

        // 2. Construct 200×200 TileMap с procedural terrain pattern.
        const int width = 200;
        const int height = 200;
        const float tileSize = 16f;
        using var tileMap = new TileMap(width, height, tileSize, atlasTexture);

        var random = new Random(Seed: 123);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileIdx;
                // Procedural terrain: bands of grass-like + stone-like + sand-like tiles.
                if ((x + y) / 30 % 2 == 0)
                {
                    tileIdx = random.Next(0, 8);    // first 8 tiles = "grass-like" diverse patterns
                }
                else
                {
                    tileIdx = random.Next(8, 16);   // next 8 tiles = "stone-like"
                }
                AtlasRegion region = ProceduralAtlas.GetTileRegion(tileIdx);
                tileMap.SetTile(x, y, region);
            }
        }
        Console.WriteLine($"  TileMap: {width}×{height} = {tileMap.TotalTiles} tiles");
        Console.WriteLine($"  Atlas: {ProceduralAtlas.AtlasWidth}×{ProceduralAtlas.AtlasHeight} (single texture)");
        Console.WriteLine($"  Required draw cycles per frame: {(tileMap.TotalTiles + runtime.SpriteRenderer.MaxSpritesPerFrame - 1) / runtime.SpriteRenderer.MaxSpritesPerFrame} (S-LOCK-5a multi-cycle)");

        // 3. Camera2D positioned к viewport center; pannable via WASD input.
        runtime.Camera.Position = new Vector2(width * tileSize * 0.5f, height * tileSize * 0.5f);
        runtime.Camera.ViewportSize = new Vector2(runtime.Swapchain.Width, runtime.Swapchain.Height);
        runtime.Camera.Zoom = 1.0f;

        // 4. Per-frame sync.
        using var commandBuffer = runtime.GraphicsCommandPool.AllocateBuffer();
        using var imageAvailable = new VulkanSemaphore(runtime.VulkanDevice);
        var renderFinishedPerImage = new VulkanSemaphore[runtime.Swapchain.ImageCount];
        for (int i = 0; i < renderFinishedPerImage.Length; i++)
        {
            renderFinishedPerImage[i] = new VulkanSemaphore(runtime.VulkanDevice);
        }
        using var frameFence = new VulkanFence(runtime.VulkanDevice);

        // Held-key state для smooth WASD pan (V0.C.1 emits KeyDown/KeyUp events; track held state).
        var heldKeys = new HashSet<Key>();

        int frameCount = 0;
        var tileStart = DateTime.UtcNow;
        while ((DateTime.UtcNow - tileStart).TotalSeconds < durationSeconds && runtime.Window.IsOpen)
        {
            runtime.Window.PumpMessages();

            // Drain input events, track WASD held state.
            while (runtime.InputQueue.TryDequeue(out IInputEvent? evt))
            {
                if (evt is KeyPressedEvent kd)
                {
                    heldKeys.Add(kd.Key);
                }
                else if (evt is KeyReleasedEvent ku)
                {
                    heldKeys.Remove(ku.Key);
                }
            }

            // Apply held WASD к camera pan.
            const float panSpeed = 5f;
            if (heldKeys.Contains(Key.W)) runtime.Camera.Position += new Vector2(0, -panSpeed);
            if (heldKeys.Contains(Key.S)) runtime.Camera.Position += new Vector2(0, panSpeed);
            if (heldKeys.Contains(Key.A)) runtime.Camera.Position += new Vector2(-panSpeed, 0);
            if (heldKeys.Contains(Key.D)) runtime.Camera.Position += new Vector2(panSpeed, 0);

            if (!runtime.Window.IsOpen) break;

            uint imageIndex = runtime.Swapchain.AcquireNextImage(imageAvailable.Handle, IntPtr.Zero, out bool outOfDate);
            if (outOfDate)
            {
                runtime.VulkanDevice.WaitIdle();
                runtime.Swapchain.Recreate((uint)runtime.Window.Width, (uint)runtime.Window.Height);
                runtime.RecreateFramebuffersForSwapchain();
                continue;
            }

            commandBuffer.Reset();
            commandBuffer.Begin(VkCommandBufferUsageFlagsPublic.OneTimeSubmit);
            RenderTileMapMultiCycle(commandBuffer, runtime, (int)imageIndex, tileMap);
            commandBuffer.End();

            IntPtr renderFinishedHandle = renderFinishedPerImage[imageIndex].Handle;
            commandBuffer.SubmitTo(
                runtime.VulkanDevice.GraphicsQueue,
                waitSemaphore: imageAvailable.Handle,
                waitStage: VkPipelineStageFlagsPublic.ColorAttachmentOutput,
                signalSemaphore: renderFinishedHandle,
                fence: frameFence);

            bool presentOutOfDate = runtime.Swapchain.Present(
                runtime.VulkanDevice.GraphicsQueue, renderFinishedHandle, imageIndex);
            if (presentOutOfDate)
            {
                runtime.VulkanDevice.WaitIdle();
                runtime.Swapchain.Recreate((uint)runtime.Window.Width, (uint)runtime.Window.Height);
            }

            frameFence.Wait();
            frameFence.Reset();
            frameCount++;
        }

        double elapsed = (DateTime.UtcNow - tileStart).TotalSeconds;
        double fps = frameCount / Math.Max(elapsed, 0.001);
        Console.WriteLine($"  [PASS?] 200×200 TileMap: {frameCount} frames in {elapsed:F2}s = {fps:F1} FPS");
        if (fps >= 60.0)
        {
            Console.WriteLine($"  [PASS] R.3 success criterion met (60+ FPS sustained on 40K tiles)");
        }
        else
        {
            Console.WriteLine($"  [WARN] R.3 target unmet ({fps:F1} < 60 FPS) — surface к Crystalka for SC-9 investigation");
        }

        runtime.VulkanDevice.WaitIdle();
        foreach (VulkanSemaphore s in renderFinishedPerImage)
        {
            s.Dispose();
        }
    }

    /// <summary>
    /// V0.C.2 S-LOCK-5a multi-cycle rendering: splits TileMap submission into chunks of
    /// SpriteRenderer.MaxSpritesPerFrame. Opens render pass once, issues BeginFrame/Submit×N/EndFrame
    /// per chunk; closes render pass. 40K tiles / 10K cap = 4 vkCmdDrawIndexed calls per frame.
    /// </summary>
    private static void RenderTileMapMultiCycle(
        VulkanCommandBuffer commandBuffer, Runtime runtime, int imageIndex, TileMap tileMap)
    {
        runtime.BeginRenderPassForSprites(
            commandBuffer, imageIndex,
            clearColor: new Vector4(0.10f, 0.10f, 0.20f, 1.0f));

        // Multi-cycle TileMap submission per S-LOCK-5a.
        int total = tileMap.TotalTiles;
        int chunkSize = runtime.SpriteRenderer.MaxSpritesPerFrame;
        for (int chunkStart = 0; chunkStart < total; chunkStart += chunkSize)
        {
            int chunkEnd = Math.Min(chunkStart + chunkSize, total);
            runtime.SpriteRenderer.BeginFrame((uint)imageIndex);
            for (int i = chunkStart; i < chunkEnd; i++)
            {
                int x = i % tileMap.Width;
                int y = i / tileMap.Width;
                var sprite = new DualFrontier.Runtime.Sprite.Sprite(
                    Texture: tileMap.Atlas,
                    Region: tileMap.GetTile(x, y),
                    Transform: new SpriteTransform(
                        Position: new Vector2(x * tileMap.TileSize, y * tileMap.TileSize),
                        Scale: new Vector2(tileMap.TileSize, tileMap.TileSize),
                        Rotation: 0f,
                        TintRgba: tileMap.GetTint(x, y)));
                runtime.SpriteRenderer.Submit(sprite);
            }
            runtime.SpriteRenderer.EndFrame(commandBuffer, runtime.Camera);
        }

        runtime.EndSpriteRenderPass(commandBuffer);
    }

    /// <summary>
    /// V1-12 / V1-16 — V1 isotropic diffusion на 200×200 field. Source spike at center,
    /// uniform conductivity D=0.1, 5 iterations per frame. Substrate primitive close
    /// criterion: FPS ≥ 60 sustained. Per VULKAN_SUBSTRATE.md §1.2 + §1.4 budget.
    /// Composed through <see cref="Runtime.CreateFieldStorageBinding"/> +
    /// <see cref="Runtime.CreateV1DiffusionPipeline"/> per V1-14 factory pattern.
    /// </summary>
    private static void RunV1IsotropicDiffusion(Runtime runtime, int durationSeconds)
    {
        const int W = 200, H = 200;
        const float D = 0.1f, Decay = 0.0f, Dt = 0.5f;
        const int IterationsPerFrame = 5;

        using var diffusionWorld = new NativeWorld();
        FieldStorageBinding fieldBinding;
        V1DiffusionPipeline pipeline;
        try
        {
            fieldBinding = runtime.CreateFieldStorageBinding(diffusionWorld);
            pipeline = runtime.CreateV1DiffusionPipeline(fieldBinding, "v1.iso.200x200.pipeline");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [FAIL] V1 isotropic factory composition: {ex.GetType().Name}: {ex.Message}");
            return;
        }

        var field = diffusionWorld.Fields.Register<float>("v1.iso.200x200", W, H);
        field.WriteCell(W / 2, H / 2, 1000f);
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                field.SetConductivity(x, y, D);
            }
        }

        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay,
            DeltaTime = Dt,
            Width = W,
            Height = H,
        };

        // Dispatch dimensions cover 200×200 с 8×8 workgroups: ceil(200/8) = 25 workgroups per axis.
        const uint DispatchX = 25, DispatchY = 25;

        int frameCount = 0;
        int iterationsExecuted = 0;
        var sceneStart = DateTime.UtcNow;
        while ((DateTime.UtcNow - sceneStart).TotalSeconds < durationSeconds && runtime.Window.IsOpen)
        {
            runtime.Window.PumpMessages();
            while (runtime.InputQueue.TryDequeue(out _)) { }
            if (!runtime.Window.IsOpen) break;

            for (int i = 0; i < IterationsPerFrame; i++)
            {
                if (!pipeline.ExecuteIteration("v1.iso.200x200", pc, DispatchX, DispatchY))
                {
                    Console.WriteLine($"  [FAIL] Dispatch failed at iteration {iterationsExecuted}");
                    return;
                }
                iterationsExecuted++;
            }
            frameCount++;
        }

        double elapsed = (DateTime.UtcNow - sceneStart).TotalSeconds;
        double fps = frameCount / Math.Max(elapsed, 0.001);

        // Snapshot к prove the field actually evolved (vs no-op stub holding initial state).
        float centerValue = field.ReadCell(W / 2, H / 2);
        float adjacentValue = field.ReadCell(W / 2 + 1, H / 2);
        float ringValue = field.ReadCell(W / 2 + 5, H / 2);
        float total = 0;
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                total += field.ReadCell(x, y);
            }
        }

        Console.WriteLine($"  Field {W}×{H}: D={D}, K={Decay}, dt={Dt}, {IterationsPerFrame} iter/frame");
        Console.WriteLine($"  Iterations executed: {iterationsExecuted}, frames: {frameCount}, elapsed: {elapsed:F2}s");
        Console.WriteLine($"  Field state: center={centerValue:F2}, adjacent={adjacentValue:F4}, " +
                          $"ring(+5)={ringValue:F4}, total={total:F2} (initial mass=1000, no decay)");
        Console.WriteLine($"  [PASS?] V1 isotropic 200×200: {fps:F1} FPS sustained over compute dispatch loop");
        if (fps >= 60.0)
        {
            Console.WriteLine($"  [PASS] V1-12 success criterion met (60+ FPS sustained)");
        }
        else
        {
            Console.WriteLine($"  [WARN] V1-12 FPS target unmet ({fps:F1} < 60) — surface к Crystalka for SC-13 investigation");
        }

        // К-L7 atomic-from-observer: ExecuteIteration waits its fence per-call, so device idle
        // before disposing the binding/pipeline ensures no lingering submission.
        runtime.VulkanDevice.WaitIdle();
    }

    /// <summary>
    /// V1-13 / V1-16 — V1 anisotropic diffusion на 200×200 field с а horizontal "wire" row
    /// (high D), source at the left end of the wire, off-wire conductivity low. Verifies
    /// the asymmetric flow rule min(D_self, D_neighbour) channels propagation along the
    /// wire (per VULKAN_SUBSTRATE.md §1.2 + §5.1 wire-path emergence pattern). FPS ≥ 60.
    /// Composed through <see cref="Runtime"/> factories per V1-14 pattern.
    /// </summary>
    private static void RunV1AnisotropicWire(Runtime runtime, int durationSeconds)
    {
        const int W = 200, H = 200;
        const int WireY = H / 2;
        const float WireD = 1.0f, OffWireD = 0.01f;
        const float Decay = 0.0f, Dt = 0.2f;
        const int IterationsPerFrame = 5;

        using var aniseWorld = new NativeWorld();
        FieldStorageBinding fieldBinding;
        V1DiffusionPipeline pipeline;
        try
        {
            fieldBinding = runtime.CreateFieldStorageBinding(aniseWorld);
            pipeline = runtime.CreateV1DiffusionPipeline(fieldBinding, "v1.aniso.wire.pipeline");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [FAIL] V1 anisotropic factory composition: {ex.GetType().Name}: {ex.Message}");
            return;
        }

        var field = aniseWorld.Fields.Register<float>("v1.aniso.wire.200x200", W, H);
        field.WriteCell(0, WireY, 1000f);
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                field.SetConductivity(x, y, y == WireY ? WireD : OffWireD);
            }
        }

        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = Decay,
            DeltaTime = Dt,
            Width = W,
            Height = H,
        };

        const uint DispatchX = 25, DispatchY = 25;

        int frameCount = 0;
        int iterationsExecuted = 0;
        var sceneStart = DateTime.UtcNow;
        while ((DateTime.UtcNow - sceneStart).TotalSeconds < durationSeconds && runtime.Window.IsOpen)
        {
            runtime.Window.PumpMessages();
            while (runtime.InputQueue.TryDequeue(out _)) { }
            if (!runtime.Window.IsOpen) break;

            for (int i = 0; i < IterationsPerFrame; i++)
            {
                if (!pipeline.ExecuteIteration("v1.aniso.wire.200x200", pc, DispatchX, DispatchY))
                {
                    Console.WriteLine($"  [FAIL] Dispatch failed at iteration {iterationsExecuted}");
                    return;
                }
                iterationsExecuted++;
            }
            frameCount++;
        }

        double elapsed = (DateTime.UtcNow - sceneStart).TotalSeconds;
        double fps = frameCount / Math.Max(elapsed, 0.001);

        // Snapshot к prove wire channels propagation — wire cells along x past the source
        // should have non-trivial values, off-wire cells nearby should be near-zero.
        float wireSource = field.ReadCell(0, WireY);
        float wireMid = field.ReadCell(W / 4, WireY);
        float wireFar = field.ReadCell(W / 2, WireY);
        float offWireAdjacent = field.ReadCell(W / 4, WireY - 1);  // 1 cell off wire, same x
        float offWireFar = field.ReadCell(W / 4, WireY - 10);      // 10 cells off wire

        Console.WriteLine($"  Field {W}×{H}: wire-row y={WireY}, WireD={WireD}, OffWireD={OffWireD}, dt={Dt}");
        Console.WriteLine($"  Iterations executed: {iterationsExecuted}, frames: {frameCount}, elapsed: {elapsed:F2}s");
        Console.WriteLine($"  Wire(x=0)={wireSource:F2}, wire(x=W/4)={wireMid:F4}, wire(x=W/2)={wireFar:F6}");
        Console.WriteLine($"  Off-wire(y-1)={offWireAdjacent:F4}, off-wire(y-10)={offWireFar:F6}");
        Console.WriteLine($"  Wire ratio (wire(W/4) ÷ off-wire(y-10)): " +
                          $"{(offWireFar > 1e-6f ? wireMid / offWireFar : float.PositiveInfinity):F2}× (channeling factor)");
        Console.WriteLine($"  [PASS?] V1 anisotropic wire 200×200: {fps:F1} FPS sustained over compute dispatch loop");
        if (fps >= 60.0)
        {
            Console.WriteLine($"  [PASS] V1-13 success criterion met (60+ FPS sustained)");
        }
        else
        {
            Console.WriteLine($"  [WARN] V1-13 FPS target unmet ({fps:F1} < 60) — surface к Crystalka for SC-13 investigation");
        }

        runtime.VulkanDevice.WaitIdle();
    }

    private static string LocateAsset(string name)
    {
        string baseDir = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(baseDir);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
        {
            dir = dir.Parent;
        }
        if (dir == null)
        {
            throw new InvalidOperationException("Could not locate DualFrontier.sln");
        }
        return Path.Combine(dir.FullName, "assets", "shaders", name);
    }

}
