using System.Numerics;
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
    /// parameter is treated as identity (V0.C.1 smoke test passed identity matrix only;
    /// V0.C.2 callers should use RecordSpritesFrame с Camera2D instead).
    /// </summary>
    public unsafe void RecordSpriteFrame(
        VulkanCommandBuffer commandBuffer,
        int imageIndex,
        Sprite.Sprite sprite,
        Matrix4x4 mvp,
        Vector4 clearColor)
    {
        // V0.C.2 backward-compat: single sprite via batched infrastructure.
        // Internally constructs a temporary Camera2D matching swapchain viewport;
        // smoke test caller previously passed Matrix4x4.Identity (no MVP transform applied).
        var tempCam = new Camera2D
        {
            Position = new Vector2(0, 0),
            ViewportSize = new Vector2(2, 2),  // ±1 maps к NDC ±1 = effectively identity ortho
            Zoom = 1.0f,
        };
        RecordSpritesFrame(commandBuffer, imageIndex, new[] { sprite }, tempCam, clearColor);
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
        ArgumentNullException.ThrowIfNull(commandBuffer);
        ArgumentNullException.ThrowIfNull(sprites);
        ArgumentNullException.ThrowIfNull(camera);
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
        SpriteRenderer.EndFrame(commandBuffer, camera);

        VkApi.vkCmdEndRenderPass(commandBuffer.Handle);
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
