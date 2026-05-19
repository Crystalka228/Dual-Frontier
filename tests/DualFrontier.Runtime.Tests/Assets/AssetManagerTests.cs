using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using DualFrontier.Runtime.Assets;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Assets;

/// <summary>
/// AssetManager tests. Verifies path resolution + caching + path traversal protection.
/// PNG decoding tested separately via <see cref="PngDecoderTests"/>; AssetManager tests
/// use a tiny synthetic PNG to exercise the integration без depending on external assets.
/// </summary>
public sealed class AssetManagerTests : IDisposable
{
    private readonly string _tempRoot;

    public AssetManagerTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "df_asset_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempRoot);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    [Fact]
    public void Constructor_NonexistentDirectory_Throws()
    {
        string fake = Path.Combine(_tempRoot, "nonexistent");
        Action act = () => new AssetManager(fake);
        act.Should().Throw<DirectoryNotFoundException>();
    }

    [Fact]
    public void Constructor_NullPath_Throws()
    {
        Action act = () => new AssetManager(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LoadPng_ValidPath_ReturnsImage()
    {
        WriteSyntheticPng(Path.Combine(_tempRoot, "test.png"), 2, 2);
        using var assets = new AssetManager(_tempRoot);
        PngImage image = assets.LoadPng(new AssetPath("test.png"));
        image.Width.Should().Be(2);
        image.Height.Should().Be(2);
        image.PixelsRgba8.Should().HaveCount(2 * 2 * 4);
    }

    [Fact]
    public void LoadPng_SamePathTwice_ReturnsCachedInstance()
    {
        WriteSyntheticPng(Path.Combine(_tempRoot, "test.png"), 2, 2);
        using var assets = new AssetManager(_tempRoot);
        PngImage first = assets.LoadPng(new AssetPath("test.png"));
        PngImage second = assets.LoadPng(new AssetPath("test.png"));
        ReferenceEquals(first, second).Should().BeTrue();
    }

    [Fact]
    public void LoadPng_MissingFile_ThrowsFileNotFound()
    {
        using var assets = new AssetManager(_tempRoot);
        Action act = () => assets.LoadPng(new AssetPath("missing.png"));
        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void LoadPng_PathTraversal_Throws()
    {
        using var assets = new AssetManager(_tempRoot);
        Action act = () => assets.LoadPng(new AssetPath("../../etc/passwd"));
        act.Should().Throw<InvalidOperationException>().WithMessage("*escapes root*");
    }

    [Fact]
    public void ClearCache_NextLoadReDecodes()
    {
        WriteSyntheticPng(Path.Combine(_tempRoot, "test.png"), 2, 2);
        using var assets = new AssetManager(_tempRoot);
        PngImage first = assets.LoadPng(new AssetPath("test.png"));
        assets.ClearCache();
        PngImage second = assets.LoadPng(new AssetPath("test.png"));
        ReferenceEquals(first, second).Should().BeFalse();
        second.Width.Should().Be(2);
    }

    [Fact]
    public void LoadPng_NestedDirectory_Resolves()
    {
        string subdir = Path.Combine(_tempRoot, "sprites");
        Directory.CreateDirectory(subdir);
        WriteSyntheticPng(Path.Combine(subdir, "pawn.png"), 4, 4);

        using var assets = new AssetManager(_tempRoot);
        PngImage image = assets.LoadPng(new AssetPath("sprites/pawn.png"));
        image.Width.Should().Be(4);
        image.Height.Should().Be(4);
    }

    [Fact]
    public void Dispose_ClearsCache_SubsequentLoadThrows()
    {
        WriteSyntheticPng(Path.Combine(_tempRoot, "test.png"), 2, 2);
        var assets = new AssetManager(_tempRoot);
        assets.LoadPng(new AssetPath("test.png"));
        assets.Dispose();

        Action act = () => assets.LoadPng(new AssetPath("test.png"));
        act.Should().Throw<ObjectDisposedException>();
    }

    // ============================================================
    // Synthetic PNG file writer
    // ============================================================

    private static void WriteSyntheticPng(string path, int width, int height)
    {
        byte[] pixels = new byte[width * height * 4];
        for (int i = 0; i < pixels.Length; i += 4)
        {
            pixels[i + 0] = 100;
            pixels[i + 1] = 150;
            pixels[i + 2] = 200;
            pixels[i + 3] = 255;
        }
        byte[] png = BuildPng(width, height, pixels);
        File.WriteAllBytes(path, png);
    }

    private static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

    private static byte[] BuildPng(int width, int height, byte[] rgbaPixels)
    {
        int scanlineLength = width * 4;
        byte[] filtered = new byte[(scanlineLength + 1) * height];
        for (int y = 0; y < height; y++)
        {
            int dstOffset = y * (scanlineLength + 1);
            filtered[dstOffset] = 0;  // filter type None
            Buffer.BlockCopy(rgbaPixels, y * scanlineLength, filtered, dstOffset + 1, scanlineLength);
        }

        byte[] compressed = DeflateCompress(filtered);
        var idat = new byte[compressed.Length + 2];
        idat[0] = 0x78;
        idat[1] = 0x9C;
        Buffer.BlockCopy(compressed, 0, idat, 2, compressed.Length);

        var ihdr = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(ihdr.AsSpan(0, 4), width);
        BinaryPrimitives.WriteInt32BigEndian(ihdr.AsSpan(4, 4), height);
        ihdr[8] = 8;     // bit depth
        ihdr[9] = 6;     // color type RGBA
        ihdr[10] = 0;    // compression method
        ihdr[11] = 0;    // filter method
        ihdr[12] = 0;    // interlace method

        var bytes = new List<byte>(PngSignature);
        AppendChunk(bytes, "IHDR", ihdr);
        AppendChunk(bytes, "IDAT", idat);
        AppendChunk(bytes, "IEND", Array.Empty<byte>());
        return bytes.ToArray();
    }

    private static byte[] DeflateCompress(byte[] raw)
    {
        using var output = new MemoryStream();
        using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            deflate.Write(raw, 0, raw.Length);
        }
        return output.ToArray();
    }

    private static void AppendChunk(List<byte> bytes, string type, byte[] data)
    {
        bytes.Add((byte)(data.Length >> 24));
        bytes.Add((byte)(data.Length >> 16));
        bytes.Add((byte)(data.Length >> 8));
        bytes.Add((byte)data.Length);
        byte[] typeBytes = Encoding.ASCII.GetBytes(type);
        bytes.AddRange(typeBytes);
        bytes.AddRange(data);
        uint crc = ComputeCrc32(typeBytes, data);
        bytes.Add((byte)(crc >> 24));
        bytes.Add((byte)(crc >> 16));
        bytes.Add((byte)(crc >> 8));
        bytes.Add((byte)crc);
    }

    private static readonly uint[] Crc32Table = BuildCrc32Table();

    private static uint[] BuildCrc32Table()
    {
        var table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            uint c = n;
            for (int k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            }
            table[n] = c;
        }
        return table;
    }

    private static uint ComputeCrc32(byte[] typeBytes, byte[] data)
    {
        uint c = 0xFFFFFFFFu;
        foreach (byte b in typeBytes) c = Crc32Table[(c ^ b) & 0xFF] ^ (c >> 8);
        foreach (byte b in data) c = Crc32Table[(c ^ b) & 0xFF] ^ (c >> 8);
        return c ^ 0xFFFFFFFFu;
    }
}
