using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// VulkanSpritePipeline integration tests. Verifies pipeline creation on real Vulkan hardware
/// с vertex input + alpha blending + descriptor set layout + push constants.
/// </summary>
public sealed class VulkanSpritePipelineTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly VulkanSurface _surface;
    private readonly VulkanSwapchain _swapchain;
    private readonly VulkanRenderPass _renderPass;
    private readonly VulkanShaderModule _vertexShader;
    private readonly VulkanShaderModule _fragmentShader;

    public VulkanSpritePipelineTests()
    {
        var opts = new WindowOptions { Title = "SpritePipeline", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _surface = new VulkanSurface(_instance, _window);
        _swapchain = new VulkanSwapchain(_device, _surface, 400, 300);
        _renderPass = new VulkanRenderPass(_device, _swapchain.Format);
        _vertexShader = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("sprite.vert.spv"));
        _fragmentShader = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("sprite.frag.spv"));
    }

    public void Dispose()
    {
        _device.WaitIdle();
        _fragmentShader.Dispose();
        _vertexShader.Dispose();
        _renderPass.Dispose();
        _swapchain.Dispose();
        _surface.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [WindowsOnlyFact]
    public void Pipeline_creates_successfully_with_vertex_input_and_blending()
    {
        using var descLayout = new SpriteDescriptorSetLayout(_device);
        var pushRange = new VkPushConstantRangePublic(VkShaderStageFlagsPublic.Vertex, 0, 64);
        using var pipelineLayout = new VulkanPipelineLayout(
            _device,
            descriptorSetLayouts: new[] { descLayout.Handle },
            pushConstantRanges: new[] { pushRange });
        using var pipeline = new VulkanSpritePipeline(
            _device, _renderPass, _vertexShader, _fragmentShader, descLayout, pipelineLayout);

        pipeline.Handle.Should().NotBe(IntPtr.Zero);
        pipeline.Layout.Should().BeSameAs(pipelineLayout);
        pipeline.DescriptorSetLayout.Should().BeSameAs(descLayout);
    }

    [WindowsOnlyFact]
    public void DescriptorSetLayout_creates_successfully()
    {
        using var descLayout = new SpriteDescriptorSetLayout(_device);
        descLayout.Handle.Should().NotBe(IntPtr.Zero);
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
}
