using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests;

public sealed class RuntimeCompositionTests
{
    [WindowsOnlyFact]
    public void Window_plus_VulkanInstance_compose_without_crash()
    {
        var winOpts = new WindowOptions { Title = "Compose A", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using var window = new global::DualFrontier.Runtime.Window.Window(winOpts, queue);
        using var instance = new VulkanInstance(enableValidation: false);
        window.Handle.Should().NotBe(IntPtr.Zero);
        instance.Handle.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void Window_plus_VulkanInstance_plus_VulkanDevice_compose_without_crash()
    {
        var winOpts = new WindowOptions { Title = "Compose B", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using var window = new global::DualFrontier.Runtime.Window.Window(winOpts, queue);
        using var instance = new VulkanInstance(enableValidation: false);
        using var device = new VulkanDevice(instance);
        device.Handle.Should().NotBe(IntPtr.Zero);
        device.GraphicsQueue.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void Create_with_validation_disabled_composes_without_validation_layer()
    {
        var options = new RuntimeOptions
        {
            Window = new WindowOptions
            {
                Title = "Composition Test",
                Width = 800,
                Height = 600,
            },
            EnableValidationLayer = false,
        };

        using var runtime = Runtime.Create(options);

        runtime.Window.Should().NotBeNull();
        runtime.Window.IsOpen.Should().BeTrue();
        runtime.VulkanInstance.Should().NotBeNull();
        runtime.VulkanInstance.Handle.Should().NotBe(IntPtr.Zero);
        runtime.VulkanInstance.ValidationLayerEnabled.Should().BeFalse();
        runtime.ValidationLayer.Should().BeNull();
        runtime.VulkanDevice.Should().NotBeNull();
        runtime.VulkanDevice.Handle.Should().NotBe(IntPtr.Zero);
        runtime.VulkanDevice.GraphicsQueue.Should().NotBe(IntPtr.Zero);
        runtime.InputQueue.Should().NotBeNull();
    }

    [WindowsOnlyFact]
    public void Create_with_validation_enabled_composes_validation_layer()
    {
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "Validation Test", Width = 400, Height = 300 },
            EnableValidationLayer = true,
        };

        using var runtime = Runtime.Create(options);

        runtime.VulkanInstance.ValidationLayerEnabled.Should().BeTrue();
        runtime.ValidationLayer.Should().NotBeNull();
        // Validation layer present + clean instance creation expected at composition time.
        runtime.ValidationLayer!.Log.ErrorCount.Should().Be(0);
    }

    [WindowsOnlyFact]
    public void Dispose_idempotent_safe_to_call_twice()
    {
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "Dispose Test", Width = 400, Height = 300 },
            EnableValidationLayer = false,
        };

        var runtime = Runtime.Create(options);
        runtime.Dispose();
        var act = () => runtime.Dispose();
        act.Should().NotThrow();
    }
}
