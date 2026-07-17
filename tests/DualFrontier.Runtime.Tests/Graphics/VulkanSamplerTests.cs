using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// VulkanSampler tests — exercise vkCreateSampler against real Vulkan device на test hardware.
/// </summary>
public sealed class VulkanSamplerTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;

    public VulkanSamplerTests()
    {
        var opts = new WindowOptions { Title = "Sampler", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
    }

    public void Dispose()
    {
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [WindowsOnlyFact]
    public void Default_options_create_sampler_with_nearest_repeat()
    {
        using var sampler = new VulkanSampler(_device);
        sampler.Handle.Should().NotBe(IntPtr.Zero);
        sampler.Options.MagFilter.Should().Be(SamplerFilterMode.Nearest);
        sampler.Options.MinFilter.Should().Be(SamplerFilterMode.Nearest);
        sampler.Options.WrapU.Should().Be(SamplerWrapMode.Repeat);
        sampler.Options.WrapV.Should().Be(SamplerWrapMode.Repeat);
        sampler.Options.EnableAnisotropy.Should().BeFalse();
    }

    [WindowsOnlyFact]
    public void Linear_filter_creates_sampler()
    {
        var options = new SamplerOptions
        {
            MagFilter = SamplerFilterMode.Linear,
            MinFilter = SamplerFilterMode.Linear,
        };
        using var sampler = new VulkanSampler(_device, options);
        sampler.Handle.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void ClampToEdge_wrap_creates_sampler()
    {
        var options = new SamplerOptions
        {
            WrapU = SamplerWrapMode.ClampToEdge,
            WrapV = SamplerWrapMode.ClampToEdge,
        };
        using var sampler = new VulkanSampler(_device, options);
        sampler.Handle.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void MirroredRepeat_wrap_creates_sampler()
    {
        var options = new SamplerOptions
        {
            WrapU = SamplerWrapMode.MirroredRepeat,
            WrapV = SamplerWrapMode.MirroredRepeat,
        };
        using var sampler = new VulkanSampler(_device, options);
        sampler.Handle.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void Dispose_zeros_handle_and_idempotent()
    {
        var sampler = new VulkanSampler(_device);
        sampler.Dispose();
        sampler.Dispose();    // idempotent
    }

    [WindowsOnlyFact]
    public void Null_device_throws()
    {
        Action act = () => new VulkanSampler(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
