using System.Numerics;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests;

public sealed class RuntimeCompositionTests
{
    [RequiresDisplayFact]
    public void Window_plus_VulkanInstance_compose_without_crash()
    {
        var winOpts = new WindowOptions { Title = "Compose A", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using IWindow window = PlatformWindow.Create(winOpts, queue);
        using var instance = new VulkanInstance(enableValidation: false);
        instance.Handle.Should().NotBe(IntPtr.Zero);
        window.IsOpen.Should().BeTrue();

        // IWindow.Handle retired: the window's platform binding is now proven by the thing that
        // handle existed for — it can produce a real VkSurfaceKHR against this instance.
        using var surface = new VulkanSurface(instance, window);
        surface.Handle.Should().NotBe(IntPtr.Zero);
    }

    [RequiresDisplayFact]
    public void Window_plus_VulkanInstance_plus_VulkanDevice_compose_without_crash()
    {
        var winOpts = new WindowOptions { Title = "Compose B", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        using IWindow window = PlatformWindow.Create(winOpts, queue);
        using var instance = new VulkanInstance(enableValidation: false);
        using var device = new VulkanDevice(instance);
        device.Handle.Should().NotBe(IntPtr.Zero);
        device.GraphicsQueue.Should().NotBe(IntPtr.Zero);
    }

    [RequiresDisplayFact]
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

    [RequiresVulkanValidationFact]
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

    [RequiresDisplayFact]
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

    [RequiresDisplayFact]
    public void Consecutive_frames_may_record_the_same_swapchain_image_index()
    {
        // F-51 wiring pin, in the production shape. Acquire indices repeat legitimately -- a
        // swapchain recreate restarts the sequence, and MAILBOX present can release an image
        // immediately so the next acquire returns the index just used. Both are safe (a submit
        // and a fence wait separate the batches) and both used to kill the Launcher on the
        // VertexBufferRing reuse guard. Recording twice on one index is exactly that case.
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "Ring reuse", Width = 400, Height = 300 },
            EnableValidationLayer = false,
        };
        using var runtime = Runtime.Create(options);

        VulkanCommandBuffer commandBuffer = runtime.GraphicsCommandPool.AllocateBuffer();
        var noSprites = new List<global::DualFrontier.Runtime.Sprite.Sprite>();
        var clearColor = new Vector4(0f, 0f, 0f, 1f);

        RecordOneFrame(runtime, commandBuffer, noSprites, clearColor);

        Action act = () => RecordOneFrame(runtime, commandBuffer, noSprites, clearColor);
        act.Should().NotThrow(
            "the same swapchain image index may be acquired on consecutive frames, and each " +
            "recording is a complete batch the caller submits");
    }

    [RequiresDisplayFact]
    public void Recreating_the_swapchain_does_not_break_sprite_recording()
    {
        // The other index-repeat route: a recreate restarts the acquire sequence at 0.
        var options = new RuntimeOptions
        {
            Window = new WindowOptions { Title = "Ring recreate", Width = 400, Height = 300 },
            EnableValidationLayer = false,
        };
        using var runtime = Runtime.Create(options);

        VulkanCommandBuffer commandBuffer = runtime.GraphicsCommandPool.AllocateBuffer();
        var noSprites = new List<global::DualFrontier.Runtime.Sprite.Sprite>();
        var clearColor = new Vector4(0f, 0f, 0f, 1f);

        RecordOneFrame(runtime, commandBuffer, noSprites, clearColor);

        runtime.VulkanDevice.WaitIdle();
        runtime.Swapchain.Recreate(320, 240);
        runtime.RecreateFramebuffersForSwapchain();

        Action act = () => RecordOneFrame(runtime, commandBuffer, noSprites, clearColor);
        act.Should().NotThrow("a swapchain recreate restarts acquire at image 0");
    }

    private static void RecordOneFrame(
        Runtime runtime,
        VulkanCommandBuffer commandBuffer,
        List<global::DualFrontier.Runtime.Sprite.Sprite> sprites,
        Vector4 clearColor)
    {
        commandBuffer.Reset();
        commandBuffer.Begin();
        runtime.RecordSpritesFrame(commandBuffer, imageIndex: 0, sprites, Matrix4x4.Identity, clearColor);
        commandBuffer.End();
    }
}
