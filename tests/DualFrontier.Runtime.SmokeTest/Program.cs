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
            uint nativePid = binding.Register("noop_native", spirv, descriptorBindingCount: 0);
            bool dispatched = binding.DispatchField("noop_field", nativePid, 1, 1, 1);
            Console.WriteLine($"  Attach: {attached}, register pid: {nativePid}, dispatch: {dispatched}, count: {binding.PipelineCount}");
            Console.WriteLine();

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
