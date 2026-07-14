using System.Numerics;
using DualFrontier.Core.Interop;
using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime;

/// <summary>
/// Top-level facade for the Vulkan substrate. V0.A composition: Window + VulkanInstance +
/// ValidationLayer (DEBUG) + VulkanDevice. V0.B composition adds Surface + Swapchain +
/// RenderPass + Framebuffers + CommandPool + MemoryAllocator + ComputePipelineRegistry.
/// V0.C.1 composition adds AssetManager + DefaultSampler + TextureUploader + sprite shaders
/// + sprite descriptor set layout + sprite pipeline layout + sprite pipeline + SpriteRenderer.
/// </summary>
public sealed class Runtime : IDisposable
{
    public IWindow Window { get; private set; } = null!;
    public VulkanInstance VulkanInstance { get; private set; } = null!;
    public VulkanDevice VulkanDevice { get; private set; } = null!;
    public ValidationLayer? ValidationLayer { get; private set; }
    public InputEventQueue InputQueue { get; private set; } = null!;

    // V0.B additions
    public VulkanSurface Surface { get; private set; } = null!;
    public VulkanSwapchain Swapchain { get; private set; } = null!;
    public VulkanRenderPass RenderPass { get; private set; } = null!;
    public IReadOnlyList<VulkanFramebuffer> Framebuffers => _framebuffers;
    public VulkanCommandPool GraphicsCommandPool { get; private set; } = null!;
    public VulkanCommandPool ComputeCommandPool { get; private set; } = null!;
    public MemoryAllocator MemoryAllocator { get; private set; } = null!;
    public ComputePipelineRegistry ComputePipelines { get; private set; } = null!;

    // V0.C.1 additions
    public AssetManager AssetManager { get; private set; } = null!;
    public VulkanSampler DefaultSampler { get; private set; } = null!;
    public TextureUploader TextureUploader { get; private set; } = null!;
    public VulkanShaderModule SpriteVertexShader { get; private set; } = null!;
    public VulkanShaderModule SpriteFragmentShader { get; private set; } = null!;
    public SpriteDescriptorSetLayout SpriteDescriptorSetLayout { get; private set; } = null!;
    public VulkanPipelineLayout SpritePipelineLayout { get; private set; } = null!;
    public VulkanSpritePipeline SpritePipeline { get; private set; } = null!;
    public SpriteRenderer SpriteRenderer { get; private set; } = null!;

    // V0.C.2 additions
    public Camera2D Camera { get; private set; } = null!;

    private readonly List<VulkanFramebuffer> _framebuffers = new();
    private byte[]? _v1DiffusionSpirvCache;
    private bool _disposed;

    private Runtime()
    {
    }

    /// <summary>
    /// Constructs Runtime composing все V0.A + V0.B + V0.C.1 primitives. Disposes any partially-
    /// constructed component on failure (preserves «no leaked Vulkan handles» exit criterion).
    /// </summary>
    public static Runtime Create(RuntimeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var runtime = new Runtime();
        try
        {
            // V0.A primitives.
            runtime.InputQueue = new InputEventQueue();
            runtime.Window = new Window.Window(options.Window, runtime.InputQueue);
            runtime.VulkanInstance = new VulkanInstance(options.EnableValidationLayer);

            if (options.EnableValidationLayer)
            {
                runtime.ValidationLayer = new ValidationLayer(runtime.VulkanInstance);
            }

            runtime.VulkanDevice = new VulkanDevice(runtime.VulkanInstance);

            // K-L19 fail-fast.
            HardwareCapabilityCheck.Verify(runtime.VulkanInstance, runtime.VulkanDevice);

            // V0.B primitives.
            runtime.Surface = new VulkanSurface(runtime.VulkanInstance, runtime.Window);

            // F06: VulkanDevice selects the physical device + graphics queue family before the
            // surface exists, on graphics capability alone. Vulkan does NOT guarantee the graphics
            // family can also present to this surface — on split graphics/present topologies it may
            // not, and vkQueuePresentKHR on a non-present-capable family is a spec violation. Now
            // that the surface exists, validate present support explicitly (fail fast) before the
            // swapchain is built on the assumption. Full present-family *selection* (which requires
            // creating the surface before final device selection) is a follow-up.
            VkResult presentSupportResult = VkApi.vkGetPhysicalDeviceSurfaceSupportKHR(
                runtime.VulkanDevice.PhysicalDevice,
                runtime.VulkanDevice.GraphicsQueueFamilyIndex,
                runtime.Surface.Handle,
                out uint graphicsFamilyPresentSupport);
            if (presentSupportResult != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException(
                    $"vkGetPhysicalDeviceSurfaceSupportKHR failed: {presentSupportResult}");
            }
            if (graphicsFamilyPresentSupport == 0)
            {
                throw new InvalidOperationException(
                    $"Selected graphics queue family {runtime.VulkanDevice.GraphicsQueueFamilyIndex} cannot " +
                    "present to the window surface. Dual Frontier currently requires a graphics queue that " +
                    "also supports present; a separate present-queue family is not yet selected (F06).");
            }

            runtime.Swapchain = new VulkanSwapchain(
                runtime.VulkanDevice, runtime.Surface,
                (uint)options.Window.Width, (uint)options.Window.Height);
            runtime.RenderPass = new VulkanRenderPass(runtime.VulkanDevice, runtime.Swapchain.Format);
            foreach (SwapchainImage img in runtime.Swapchain.Images)
            {
                runtime._framebuffers.Add(new VulkanFramebuffer(
                    runtime.VulkanDevice, runtime.RenderPass, img.ImageViewHandle,
                    runtime.Swapchain.Width, runtime.Swapchain.Height));
            }
            runtime.GraphicsCommandPool = new VulkanCommandPool(
                runtime.VulkanDevice, runtime.VulkanDevice.GraphicsQueueFamilyIndex);
            runtime.ComputeCommandPool = new VulkanCommandPool(
                runtime.VulkanDevice, runtime.VulkanDevice.AsyncComputeQueueFamilyIndex!.Value);
            runtime.MemoryAllocator = new MemoryAllocator(runtime.VulkanDevice);
            runtime.ComputePipelines = new ComputePipelineRegistry(runtime.VulkanDevice);

            // V0.C.1 primitives.
            runtime.AssetManager = new AssetManager(options.AssetsDirectory);
            runtime.DefaultSampler = new VulkanSampler(runtime.VulkanDevice);
            runtime.TextureUploader = new TextureUploader(
                runtime.VulkanDevice, runtime.MemoryAllocator, runtime.GraphicsCommandPool);

            // Load sprite shaders (assets/shaders/sprite.vert.spv + sprite.frag.spv).
            // Use resolved RootDirectory (smart-resolved by AssetManager) for path стабильность.
            string spriteVertPath = Path.Combine(runtime.AssetManager.RootDirectory, "shaders", "sprite.vert.spv");
            string spriteFragPath = Path.Combine(runtime.AssetManager.RootDirectory, "shaders", "sprite.frag.spv");
            runtime.SpriteVertexShader = VulkanShaderModule.LoadFromFile(runtime.VulkanDevice, spriteVertPath);
            runtime.SpriteFragmentShader = VulkanShaderModule.LoadFromFile(runtime.VulkanDevice, spriteFragPath);

            // Sprite descriptor + pipeline layout + pipeline.
            runtime.SpriteDescriptorSetLayout = new SpriteDescriptorSetLayout(runtime.VulkanDevice);
            var pushConstantRange = new VkPushConstantRangePublic(
                StageFlags: VkShaderStageFlagsPublic.Vertex,
                Offset: 0,
                Size: 64);
            runtime.SpritePipelineLayout = new VulkanPipelineLayout(
                runtime.VulkanDevice,
                descriptorSetLayouts: new[] { runtime.SpriteDescriptorSetLayout.Handle },
                pushConstantRanges: new[] { pushConstantRange });
            runtime.SpritePipeline = new VulkanSpritePipeline(
                runtime.VulkanDevice,
                runtime.RenderPass,
                runtime.SpriteVertexShader,
                runtime.SpriteFragmentShader,
                runtime.SpriteDescriptorSetLayout,
                runtime.SpritePipelineLayout);

            // V0.C.2: SpriteRenderer now batched per S-LOCK-7; constructor accepts
            // swapchainImageCount + maxSpritesPerFrame.
            runtime.SpriteRenderer = new SpriteRenderer(
                runtime.VulkanDevice,
                runtime.MemoryAllocator,
                runtime.SpritePipeline,
                runtime.Swapchain.Images.Count,
                maxSpritesPerFrame: 10_000);

            // V0.C.2: Camera2D initialized с window viewport size, identity position/zoom.
            runtime.Camera = new Camera2D
            {
                Position = Vector2.Zero,
                Zoom = 1.0f,
                Rotation = 0f,
                ViewportSize = new Vector2(options.Window.Width, options.Window.Height),
            };

            return runtime;
        }
        catch
        {
            runtime.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Recreate framebuffers к match current swapchain image views. Caller must invoke after
    /// <c>Swapchain.Recreate</c> + <c>VulkanDevice.WaitIdle</c> к prevent rendering into stale
    /// framebuffer references (old image views are destroyed by Swapchain.Recreate).
    /// </summary>
    public void RecreateFramebuffersForSwapchain()
    {
        foreach (VulkanFramebuffer fb in _framebuffers)
        {
            fb.Dispose();
        }
        _framebuffers.Clear();
        foreach (SwapchainImage img in Swapchain.Images)
        {
            _framebuffers.Add(new VulkanFramebuffer(
                VulkanDevice, RenderPass, img.ImageViewHandle,
                Swapchain.Width, Swapchain.Height));
        }
    }

    /// <summary>
    /// V0.C.1 convenience preserved as V0.C.2 backward-compat shim: routes through the new
    /// batched SpriteRenderer API (BeginFrame/Submit/EndFrame). The <paramref name="mvp"/>
    /// parameter is applied as the vertex MVP push constant (pass Matrix4x4.Identity for the
    /// original V0.C.1 smoke-test behaviour; V0.C.2 callers should prefer RecordSpritesFrame
    /// с Camera2D).
    /// </summary>
    public unsafe void RecordSpriteFrame(
        VulkanCommandBuffer commandBuffer,
        int imageIndex,
        Sprite.Sprite sprite,
        Matrix4x4 mvp,
        Vector4 clearColor)
    {
        // V0.C.2 backward-compat: single sprite via batched infrastructure. The caller-supplied
        // MVP is now honoured (previously this shim discarded `mvp` and rendered through a fixed
        // identity-ortho Camera2D — F01). Passing Matrix4x4.Identity reproduces the original
        // V0.C.1 smoke-test behaviour exactly.
        RecordSpritesFrame(commandBuffer, imageIndex, new[] { sprite }, mvp, clearColor);
    }

    /// <summary>
    /// V0.C.2: begin a render pass + set viewport/scissor + clear к specified color.
    /// Public helper для multi-cycle SpriteRenderer rendering (per S-LOCK-5a TileMap case).
    /// Caller invokes BeginRenderPassForSprites → SpriteRenderer.BeginFrame/Submit/EndFrame
    /// (one or more cycles) → EndSpriteRenderPass.
    /// </summary>
    public unsafe void BeginRenderPassForSprites(
        VulkanCommandBuffer commandBuffer,
        int imageIndex,
        Vector4 clearColor)
    {
        ArgumentNullException.ThrowIfNull(commandBuffer);
        if ((uint)imageIndex >= _framebuffers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(imageIndex));
        }

        VulkanFramebuffer framebuffer = _framebuffers[imageIndex];

        VkClearValue clearValue = default;
        clearValue.color.float32[0] = clearColor.X;
        clearValue.color.float32[1] = clearColor.Y;
        clearValue.color.float32[2] = clearColor.Z;
        clearValue.color.float32[3] = clearColor.W;

        var renderPassBegin = new VkRenderPassBeginInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO,
            pNext = IntPtr.Zero,
            renderPass = RenderPass.Handle,
            framebuffer = framebuffer.Handle,
            renderArea = new VkRect2D
            {
                offsetX = 0, offsetY = 0,
                width = framebuffer.Width, height = framebuffer.Height,
            },
            clearValueCount = 1,
            _padBeforePtr = 0,
            pClearValues = &clearValue,
        };
        VkApi.vkCmdBeginRenderPass(commandBuffer.Handle, in renderPassBegin, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);

        VkViewport viewport = new()
        {
            x = 0, y = 0,
            width = framebuffer.Width, height = framebuffer.Height,
            minDepth = 0.0f, maxDepth = 1.0f,
        };
        VkApi.vkCmdSetViewport(commandBuffer.Handle, 0, 1, &viewport);

        VkRect2D scissor = new()
        {
            offsetX = 0, offsetY = 0,
            width = framebuffer.Width, height = framebuffer.Height,
        };
        VkApi.vkCmdSetScissor(commandBuffer.Handle, 0, 1, &scissor);
    }

    /// <summary>
    /// V0.C.2: end the render pass started by <see cref="BeginRenderPassForSprites"/>.
    /// </summary>
    public void EndSpriteRenderPass(VulkanCommandBuffer commandBuffer)
    {
        ArgumentNullException.ThrowIfNull(commandBuffer);
        VkApi.vkCmdEndRenderPass(commandBuffer.Handle);
    }

    /// <summary>
    /// V0.C.2 batched convenience: record many sprites per frame с Camera2D MVP.
    /// Begins render pass с clear color, sets viewport/scissor, calls SpriteRenderer
    /// BeginFrame/Submit/EndFrame, ends render pass.
    /// </summary>
    public unsafe void RecordSpritesFrame(
        VulkanCommandBuffer commandBuffer,
        int imageIndex,
        IEnumerable<Sprite.Sprite> sprites,
        Camera2D camera,
        Vector4 clearColor)
    {
        ArgumentNullException.ThrowIfNull(camera);
        RecordSpritesFrame(commandBuffer, imageIndex, sprites, camera.ViewProjectionMatrix, clearColor);
    }

    /// <summary>
    /// V0.C.2 batched rendering with an explicit MVP matrix rather than a <see cref="Camera2D"/>.
    /// Shared core for the camera overload above and for the V0.C.1 backward-compat
    /// <see cref="RecordSpriteFrame"/> shim, which supplies its own matrix.
    /// </summary>
    public unsafe void RecordSpritesFrame(
        VulkanCommandBuffer commandBuffer,
        int imageIndex,
        IEnumerable<Sprite.Sprite> sprites,
        Matrix4x4 mvp,
        Vector4 clearColor)
    {
        ArgumentNullException.ThrowIfNull(commandBuffer);
        ArgumentNullException.ThrowIfNull(sprites);
        if ((uint)imageIndex >= _framebuffers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(imageIndex));
        }

        VulkanFramebuffer framebuffer = _framebuffers[imageIndex];

        VkClearValue clearValue = default;
        clearValue.color.float32[0] = clearColor.X;
        clearValue.color.float32[1] = clearColor.Y;
        clearValue.color.float32[2] = clearColor.Z;
        clearValue.color.float32[3] = clearColor.W;

        var renderPassBegin = new VkRenderPassBeginInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO,
            pNext = IntPtr.Zero,
            renderPass = RenderPass.Handle,
            framebuffer = framebuffer.Handle,
            renderArea = new VkRect2D
            {
                offsetX = 0, offsetY = 0,
                width = framebuffer.Width, height = framebuffer.Height,
            },
            clearValueCount = 1,
            _padBeforePtr = 0,
            pClearValues = &clearValue,
        };
        VkApi.vkCmdBeginRenderPass(commandBuffer.Handle, in renderPassBegin, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);

        VkViewport viewport = new()
        {
            x = 0, y = 0,
            width = framebuffer.Width, height = framebuffer.Height,
            minDepth = 0.0f, maxDepth = 1.0f,
        };
        VkApi.vkCmdSetViewport(commandBuffer.Handle, 0, 1, &viewport);

        VkRect2D scissor = new()
        {
            offsetX = 0, offsetY = 0,
            width = framebuffer.Width, height = framebuffer.Height,
        };
        VkApi.vkCmdSetScissor(commandBuffer.Handle, 0, 1, &scissor);

        SpriteRenderer.BeginFrame((uint)imageIndex);
        foreach (var sprite in sprites)
        {
            SpriteRenderer.Submit(sprite);
        }
        SpriteRenderer.EndFrame(commandBuffer, mvp);

        VkApi.vkCmdEndRenderPass(commandBuffer.Handle);
    }

    /// <summary>
    /// V1-14: attach а caller-owned <see cref="NativeWorld"/> к the Vulkan substrate so it
    /// can register + dispatch compute pipelines (V1 diffusion, V2 wave, …). Wraps the world
    /// в а <see cref="FieldStorageBinding"/> and validates async-compute queue availability
    /// per К-L19.
    /// </summary>
    /// <remarks>
    /// World ownership stays с the caller; Runtime supplies only the Vulkan handles + pipeline
    /// + descriptor layout. The returned binding can be passed к factory methods like
    /// <see cref="CreateV1DiffusionPipeline"/> к compose compute pipelines.
    /// </remarks>
    public FieldStorageBinding CreateFieldStorageBinding(NativeWorld world)
    {
        ArgumentNullException.ThrowIfNull(world);
        ObjectDisposedException.ThrowIf(_disposed, this);
        var binding = new FieldStorageBinding(world);
        if (!binding.Attach(VulkanInstance, VulkanDevice))
        {
            throw new InvalidOperationException(
                "FieldStorageBinding.Attach failed — caller's NativeWorld could не bind к the " +
                "Vulkan instance/device. Check async-compute queue family availability and that " +
                "the world has не already been attached к a different Vulkan device.");
        }
        return binding;
    }

    /// <summary>
    /// V1-14 factory: constructs а V1 diffusion compute pipeline registered against
    /// <paramref name="binding"/>'s world. Shader SPIR-V (diffusion.comp.spv) is loaded from
    /// the AssetManager-resolved <c>shaders/</c> directory on first call and cached for
    /// subsequent invocations (multiple consumers can register distinct pipelines reusing
    /// the same bytecode без re-reading the file).
    /// </summary>
    /// <param name="binding">Field storage binding obtained via <see cref="CreateFieldStorageBinding"/>.</param>
    /// <param name="pipelineName">Unique name на the native side; consumer-chosen.</param>
    public V1DiffusionPipeline CreateV1DiffusionPipeline(FieldStorageBinding binding, string pipelineName)
    {
        ArgumentNullException.ThrowIfNull(binding);
        ArgumentException.ThrowIfNullOrEmpty(pipelineName);
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_v1DiffusionSpirvCache is null)
        {
            string spirvPath = Path.Combine(AssetManager.RootDirectory, "shaders", "diffusion.comp.spv");
            if (!File.Exists(spirvPath))
            {
                throw new FileNotFoundException(
                    $"V1 diffusion shader не found at '{spirvPath}'. " +
                    "Verify Directory.Build.props CompileShaders target produced diffusion.comp.spv " +
                    "during build.", spirvPath);
            }
            _v1DiffusionSpirvCache = File.ReadAllBytes(spirvPath);
        }

        return new V1DiffusionPipeline(binding, pipelineName, _v1DiffusionSpirvCache);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        // Wait for any pending GPU work before destroying resources (validation-safety).
        VulkanDevice?.WaitIdle();

        // V0.C.1 teardown (reverse construction order).
        SpriteRenderer?.Dispose();
        SpritePipeline?.Dispose();
        SpritePipelineLayout?.Dispose();
        SpriteDescriptorSetLayout?.Dispose();
        SpriteFragmentShader?.Dispose();
        SpriteVertexShader?.Dispose();
        TextureUploader?.Dispose();
        DefaultSampler?.Dispose();
        AssetManager?.Dispose();

        // V0.B teardown.
        ComputePipelines?.Dispose();
        MemoryAllocator?.Dispose();
        ComputeCommandPool?.Dispose();
        GraphicsCommandPool?.Dispose();
        foreach (VulkanFramebuffer fb in _framebuffers)
        {
            fb.Dispose();
        }
        _framebuffers.Clear();
        RenderPass?.Dispose();
        Swapchain?.Dispose();
        Surface?.Dispose();

        // V0.A teardown.
        VulkanDevice?.Dispose();
        ValidationLayer?.Dispose();
        VulkanInstance?.Dispose();
        Window?.Dispose();
        _disposed = true;
    }
}
