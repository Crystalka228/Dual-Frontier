using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// SpriteRenderer integration tests. Verifies descriptor pool + vertex buffer + descriptor set
/// caching. Actual GPU rendering verified through smoke test (Commit 16).
/// </summary>
public sealed class SpriteRendererTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly VulkanSurface _surface;
    private readonly VulkanSwapchain _swapchain;
    private readonly VulkanRenderPass _renderPass;
    private readonly MemoryAllocator _allocator;
    private readonly VulkanCommandPool _commandPool;
    private readonly VulkanShaderModule _vertexShader;
    private readonly VulkanShaderModule _fragmentShader;
    private readonly SpriteDescriptorSetLayout _descriptorSetLayout;
    private readonly VulkanPipelineLayout _pipelineLayout;
    private readonly VulkanSpritePipeline _pipeline;
    private readonly TextureUploader _textureUploader;
    private readonly VulkanSampler _sampler;

    public SpriteRendererTests()
    {
        var opts = new WindowOptions { Title = "SpriteRenderer", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _surface = new VulkanSurface(_instance, _window);
        _swapchain = new VulkanSwapchain(_device, _surface, 400, 300);
        _renderPass = new VulkanRenderPass(_device, _swapchain.Format);
        _allocator = new MemoryAllocator(_device);
        _commandPool = new VulkanCommandPool(_device, _device.GraphicsQueueFamilyIndex);
        _vertexShader = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("sprite.vert.spv"));
        _fragmentShader = VulkanShaderModule.LoadFromFile(_device, FindShaderPath("sprite.frag.spv"));
        _descriptorSetLayout = new SpriteDescriptorSetLayout(_device);
        var pushRange = new VkPushConstantRangePublic(VkShaderStageFlagsPublic.Vertex, 0, 64);
        _pipelineLayout = new VulkanPipelineLayout(_device,
            descriptorSetLayouts: new[] { _descriptorSetLayout.Handle },
            pushConstantRanges: new[] { pushRange });
        _pipeline = new VulkanSpritePipeline(_device, _renderPass, _vertexShader, _fragmentShader, _descriptorSetLayout, _pipelineLayout);
        _textureUploader = new TextureUploader(_device, _allocator, _commandPool);
        _sampler = new VulkanSampler(_device);
    }

    public void Dispose()
    {
        _device.WaitIdle();
        _sampler.Dispose();
        _textureUploader.Dispose();
        _pipeline.Dispose();
        _pipelineLayout.Dispose();
        _descriptorSetLayout.Dispose();
        _fragmentShader.Dispose();
        _vertexShader.Dispose();
        _commandPool.Dispose();
        _allocator.Dispose();
        _renderPass.Dispose();
        _swapchain.Dispose();
        _surface.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void Renderer_constructs_with_descriptor_pool_and_vertex_buffer()
    {
        using var renderer = new SpriteRenderer(_device, _allocator, _pipeline);
        renderer.CachedDescriptorSetCount.Should().Be(0);
    }

    [Fact]
    public void Disposed_renderer_throws_on_use()
    {
        var renderer = new SpriteRenderer(_device, _allocator, _pipeline);
        renderer.Dispose();
        Action act = () => renderer.DrawSprite(default!, null!, System.Numerics.Matrix4x4.Identity);
        act.Should().Throw<ObjectDisposedException>();
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
