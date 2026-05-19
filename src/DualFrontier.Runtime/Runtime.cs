using DualFrontier.Runtime.Compute;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime;

/// <summary>
/// Top-level facade for the Vulkan substrate. V0.A composition: Window + VulkanInstance +
/// ValidationLayer (DEBUG) + VulkanDevice. V0.B composition adds Surface + Swapchain +
/// RenderPass + Framebuffers + CommandPool + MemoryAllocator + ComputePipelineRegistry.
/// V0.C extends с sprite/text/atlas + PNG decoder + threading.
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

    private readonly List<VulkanFramebuffer> _framebuffers = new();
    private bool _disposed;

    private Runtime()
    {
    }

    /// <summary>
    /// Constructs Runtime composing все V0.A + V0.B primitives. Disposes any partially-
    /// constructed component on failure (preserves «no leaked Vulkan handles» exit
    /// criterion per S-LOCK-1).
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

            return runtime;
        }
        catch
        {
            runtime.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        // Reverse construction order. V0.B teardown first, then V0.A.
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
        VulkanDevice?.Dispose();
        ValidationLayer?.Dispose();
        VulkanInstance?.Dispose();
        Window?.Dispose();
        _disposed = true;
    }
}
