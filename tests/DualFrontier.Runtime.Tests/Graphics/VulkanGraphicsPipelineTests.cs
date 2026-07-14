using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// V0.B minimal graphics pipeline integration test (clearcolor fullscreen triangle).
/// Verifies vkCreatePipelineLayout + vkCreateGraphicsPipelines с full state structures
/// succeeds on real К-L19 hardware.
/// </summary>
public sealed class VulkanGraphicsPipelineTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly VulkanSurface _surface;
    private readonly VulkanSwapchain _swapchain;
    private readonly VulkanRenderPass _renderPass;

    public VulkanGraphicsPipelineTests()
    {
        var opts = new WindowOptions { Title = "GfxPipeline", Width = 640, Height = 480 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _surface = new VulkanSurface(_instance, _window);
        _swapchain = new VulkanSwapchain(_device, _surface, 640, 480);
        _renderPass = new VulkanRenderPass(_device, _swapchain.Format);
    }

    public void Dispose()
    {
        _renderPass.Dispose();
        _swapchain.Dispose();
        _surface.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    private static string FindShaderPath(string name)
    {
        string baseDir = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(baseDir);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
        {
            dir = dir.Parent;
        }
        if (dir == null)
        {
            throw new InvalidOperationException("Could not locate DualFrontier.sln from test base dir.");
        }
        return Path.Combine(dir.FullName, "assets", "shaders", name);
    }

    [WindowsOnlyFact]
    public void PipelineLayout_empty_constructs_non_zero()
    {
        using var layout = new VulkanPipelineLayout(_device);
        layout.Handle.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void GraphicsPipeline_clearcolor_constructs_successfully()
    {
        using var layout = new VulkanPipelineLayout(_device);
        using var vert = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("clearcolor.vert.spv"));
        using var frag = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("clearcolor.frag.spv"));
        using var pipeline = new VulkanGraphicsPipeline(_device, layout, _renderPass, vert, frag);
        pipeline.Handle.Should().NotBe(IntPtr.Zero);
    }
}
