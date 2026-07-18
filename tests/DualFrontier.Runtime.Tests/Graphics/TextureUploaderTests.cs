using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Window;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Graphics;

/// <summary>
/// TextureUploader integration tests. Exercises full staging-buffer → device-local image
/// transfer path with image layout transitions UNDEFINED → TRANSFER_DST → SHADER_READ_ONLY.
/// </summary>
public sealed class TextureUploaderTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly MemoryAllocator _allocator;
    private readonly VulkanCommandPool _commandPool;

    public TextureUploaderTests()
    {
        var opts = new WindowOptions { Title = "TexUpload", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _allocator = new MemoryAllocator(_device);
        _commandPool = new VulkanCommandPool(_device, _device.GraphicsQueueFamilyIndex);
    }

    public void Dispose()
    {
        _device.WaitIdle();
        _commandPool.Dispose();
        _allocator.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [WindowsOnlyFact]
    public void Upload_2x2Rgba_completes_without_error()
    {
        using var image = new VulkanImage(
            _device, _allocator,
            width: 2, height: 2,
            VkFormatPublic.R8G8B8A8_UNorm,
            VkImageUsageFlagsPublic.Sampled | VkImageUsageFlagsPublic.TransferDst,
            VkMemoryPropertyFlagsPublic.DeviceLocal);

        var uploader = new TextureUploader(_device, _allocator, _commandPool);
        byte[] pixels = new byte[2 * 2 * 4];
        for (int i = 0; i < pixels.Length; i += 4)
        {
            pixels[i + 0] = 100;
            pixels[i + 1] = 150;
            pixels[i + 2] = 200;
            pixels[i + 3] = 255;
        }
        Action act = () => uploader.Upload(image, pixels);
        act.Should().NotThrow();
        image.Handle.Should().NotBe(IntPtr.Zero);
        image.ViewHandle.Should().NotBe(IntPtr.Zero);
    }

    [WindowsOnlyFact]
    public void Upload_NullImage_Throws()
    {
        var uploader = new TextureUploader(_device, _allocator, _commandPool);
        Action act = () => uploader.Upload(null!, new byte[16]);
        act.Should().Throw<ArgumentNullException>();
    }

    [WindowsOnlyFact]
    public void Upload_EmptyPixels_Throws()
    {
        using var image = new VulkanImage(
            _device, _allocator, width: 1, height: 1,
            VkFormatPublic.R8G8B8A8_UNorm,
            VkImageUsageFlagsPublic.Sampled | VkImageUsageFlagsPublic.TransferDst,
            VkMemoryPropertyFlagsPublic.DeviceLocal);

        var uploader = new TextureUploader(_device, _allocator, _commandPool);
        Action act = () => uploader.Upload(image, Array.Empty<byte>());
        act.Should().Throw<ArgumentException>();
    }

    [WindowsOnlyFact]
    public void CreateFromPngImage_4x4_works()
    {
        // Synthetic decoded PNG with known RGBA pattern.
        byte[] pixels = new byte[4 * 4 * 4];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = (byte)(i & 0xFF);
        }
        var pngImage = new PngImage(4, 4, pixels);

        var uploader = new TextureUploader(_device, _allocator, _commandPool);
        using var image = VulkanImage.CreateFromPngImage(_device, _allocator, uploader, pngImage);

        image.Width.Should().Be(4u);
        image.Height.Should().Be(4u);
        image.Handle.Should().NotBe(IntPtr.Zero);
        image.ViewHandle.Should().NotBe(IntPtr.Zero);
    }
}
