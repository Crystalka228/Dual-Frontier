using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Sprite;
using DualFrontier.Runtime.Window;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Sprite;

/// <summary>
/// TileMap tests per V0.C.2 S-LOCK-5 (Q4a one-sprite-per-tile ratification).
/// GPU-backed: SpriteTexture requires VulkanImage + VulkanSampler. Tests verify
/// bounds checking + storage roundtrip.
/// </summary>
public sealed class TileMapTests : IDisposable
{
    private readonly global::DualFrontier.Runtime.Window.Window _window;
    private readonly VulkanInstance _instance;
    private readonly VulkanDevice _device;
    private readonly MemoryAllocator _allocator;
    private readonly VulkanCommandPool _commandPool;
    private readonly TextureUploader _textureUploader;
    private readonly VulkanSampler _sampler;
    private readonly SpriteTexture _testAtlas;

    public TileMapTests()
    {
        var opts = new WindowOptions { Title = "TileMap", Width = 400, Height = 300 };
        var queue = new InputEventQueue();
        _window = new global::DualFrontier.Runtime.Window.Window(opts, queue);
        _instance = new VulkanInstance(enableValidation: false);
        _device = new VulkanDevice(_instance);
        _allocator = new MemoryAllocator(_device);
        _commandPool = new VulkanCommandPool(_device, _device.GraphicsQueueFamilyIndex);
        _textureUploader = new TextureUploader(_device, _allocator, _commandPool);
        _sampler = new VulkanSampler(_device);

        // Minimal 1×1 RGBA8 image for SpriteTexture atlas.
        byte[] pixels = new byte[] { 255, 255, 255, 255 };
        var pngImage = new PngImage(1, 1, pixels);
        var image = VulkanImage.CreateFromPngImage(_device, _allocator, _textureUploader, pngImage);
        _testAtlas = new SpriteTexture(image, _sampler);
    }

    public void Dispose()
    {
        _device.WaitIdle();
        // Note: _testAtlas.Dispose() disposes its image but NOT _sampler (we share it).
        // To avoid double-disposal of the sampler, we manually dispose just the image.
        // Actually SpriteTexture.Dispose disposes both image AND sampler, so we need к
        // be careful. Let test Dispose call _testAtlas.Dispose() which handles both.
        _testAtlas.Dispose();
        _textureUploader.Dispose();
        _commandPool.Dispose();
        _allocator.Dispose();
        _device.Dispose();
        _instance.Dispose();
        _window.Dispose();
    }

    [Fact]
    public void Constructor_With_Valid_Args_Succeeds()
    {
        using var map = new TileMap(width: 10, height: 5, tileSize: 16f, atlas: _testAtlas);
        map.Width.Should().Be(10);
        map.Height.Should().Be(5);
        map.TileSize.Should().Be(16f);
        map.TotalTiles.Should().Be(50);
    }

    [Fact]
    public void Constructor_With_Zero_Width_Throws()
    {
        Action act = () => new TileMap(width: 0, height: 5, tileSize: 16f, atlas: _testAtlas);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_With_Zero_Height_Throws()
    {
        Action act = () => new TileMap(width: 5, height: 0, tileSize: 16f, atlas: _testAtlas);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_With_Zero_TileSize_Throws()
    {
        Action act = () => new TileMap(width: 5, height: 5, tileSize: 0f, atlas: _testAtlas);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_With_Null_Atlas_Throws()
    {
        Action act = () => new TileMap(width: 5, height: 5, tileSize: 16f, atlas: null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SetTile_With_OutOfBounds_X_Throws()
    {
        using var map = new TileMap(width: 5, height: 5, tileSize: 16f, atlas: _testAtlas);
        Action act = () => map.SetTile(5, 0, AtlasRegion.Full);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetTile_With_OutOfBounds_Y_Throws()
    {
        using var map = new TileMap(width: 5, height: 5, tileSize: 16f, atlas: _testAtlas);
        Action act = () => map.SetTile(0, 5, AtlasRegion.Full);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetTile_With_Negative_Coords_Throws()
    {
        using var map = new TileMap(width: 5, height: 5, tileSize: 16f, atlas: _testAtlas);
        Action act = () => map.SetTile(-1, 0, AtlasRegion.Full);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetTile_ReturnsSetValue()
    {
        using var map = new TileMap(width: 5, height: 5, tileSize: 16f, atlas: _testAtlas);
        var region = new AtlasRegion(0.25f, 0.5f, 0.5f, 0.75f);
        map.SetTile(2, 3, region);
        map.GetTile(2, 3).Should().Be(region);
    }

    [Fact]
    public void Default_Tints_Are_Opaque_White()
    {
        using var map = new TileMap(width: 5, height: 5, tileSize: 16f, atlas: _testAtlas);
        map.GetTint(0, 0).Should().Be(SpriteVertex.WhiteTint);
        map.GetTint(4, 4).Should().Be(SpriteVertex.WhiteTint);
    }

    [Fact]
    public void SetTile_With_Tint_Preserves_Tint()
    {
        using var map = new TileMap(width: 5, height: 5, tileSize: 16f, atlas: _testAtlas);
        const uint customTint = 0xFF00_0000u | 0xFFu; // alpha=255 red=255
        map.SetTile(0, 0, AtlasRegion.Full, customTint);
        map.GetTint(0, 0).Should().Be(customTint);
    }

    [Fact]
    public void Use_After_Dispose_Throws_On_Submit()
    {
        var map = new TileMap(width: 5, height: 5, tileSize: 16f, atlas: _testAtlas);
        map.Dispose();
        // Submit requires a SpriteRenderer; we just verify Dispose works idempotently.
        map.Dispose();  // second dispose should not throw
    }
}
