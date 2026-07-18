using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using DualFrontier.Runtime.Assets;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Assets;

/// <summary>
/// PNG decoder tests. V0.C.1 Commit 3 (Part 1) covers chunk parsing + CRC32 + IHDR validation
/// + IDAT collection + IEND detection. Positive decode tests (with DEFLATE + filter unfiltering)
/// land V0.C.1 Commit 4 (Part 2).
/// </summary>
public sealed class PngDecoderTests
{
    private static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

    // ============================================================
    // Signature validation
    // ============================================================

    [Fact]
    public void Decode_EmptyInput_Throws()
    {
        Action act = () => PngDecoder.Decode(ReadOnlySpan<byte>.Empty);
        act.Should().Throw<PngDecoderException>().WithMessage("*signature*");
    }

    [Fact]
    public void Decode_BadSignature_Throws()
    {
        byte[] bad = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Action act = () => PngDecoder.Decode(bad);
        act.Should().Throw<PngDecoderException>().WithMessage("*signature*");
    }

    [Fact]
    public void Decode_TruncatedAfterSignature_Throws()
    {
        Action act = () => PngDecoder.Decode(PngSignature);
        act.Should().Throw<PngDecoderException>().WithMessage("*IHDR*");
    }

    // ============================================================
    // IHDR validation (S-LOCK-2 scope)
    // ============================================================

    [Fact]
    public void Decode_MissingIhdr_Throws()
    {
        var bytes = new List<byte>(PngSignature);
        AppendChunk(bytes, "IEND", Array.Empty<byte>());
        Action act = () => PngDecoder.Decode(bytes.ToArray());
        act.Should().Throw<PngDecoderException>().WithMessage("*IHDR*");
    }

    [Fact]
    public void Decode_DuplicateIhdr_Throws()
    {
        var bytes = new List<byte>(PngSignature);
        AppendChunk(bytes, "IHDR", BuildIhdr(8, 8, bitDepth: 8, colorType: 6));
        AppendChunk(bytes, "IHDR", BuildIhdr(8, 8, bitDepth: 8, colorType: 6));
        AppendChunk(bytes, "IEND", Array.Empty<byte>());
        Action act = () => PngDecoder.Decode(bytes.ToArray());
        act.Should().Throw<PngDecoderException>().WithMessage("*Duplicate IHDR*");
    }

    [Fact]
    public void Decode_BitDepth16_Throws()
    {
        var bytes = BuildMinimalPng(8, 8, bitDepth: 16, colorType: 6);
        Action act = () => PngDecoder.Decode(bytes);
        act.Should().Throw<PngDecoderException>().WithMessage("*bit depth*");
    }

    [Fact]
    public void Decode_ColorType3Palette_Throws()
    {
        var bytes = BuildMinimalPng(8, 8, bitDepth: 8, colorType: 3);
        Action act = () => PngDecoder.Decode(bytes);
        act.Should().Throw<PngDecoderException>().WithMessage("*color type*");
    }

    [Fact]
    public void Decode_ColorType0Grayscale_Throws()
    {
        var bytes = BuildMinimalPng(8, 8, bitDepth: 8, colorType: 0);
        Action act = () => PngDecoder.Decode(bytes);
        act.Should().Throw<PngDecoderException>().WithMessage("*color type*");
    }

    [Fact]
    public void Decode_InterlacedPng_Throws()
    {
        var bytes = BuildMinimalPng(8, 8, bitDepth: 8, colorType: 6, interlaceMethod: 1);
        Action act = () => PngDecoder.Decode(bytes);
        act.Should().Throw<PngDecoderException>().WithMessage("*Interlaced*");
    }

    [Fact]
    public void Decode_ZeroDimensions_Throws()
    {
        var bytes = BuildMinimalPng(0, 0, bitDepth: 8, colorType: 6);
        Action act = () => PngDecoder.Decode(bytes);
        act.Should().Throw<PngDecoderException>().WithMessage("*dimensions*");
    }

    // ============================================================
    // CRC32 verification
    // ============================================================

    [Fact]
    public void Decode_CorruptIhdrCrc_Throws()
    {
        var bytes = new List<byte>(PngSignature);
        // Manually build an IHDR chunk with wrong CRC32.
        byte[] ihdrData = BuildIhdr(8, 8, bitDepth: 8, colorType: 6);
        AppendInt32BigEndian(bytes, ihdrData.Length);
        bytes.AddRange(Encoding.ASCII.GetBytes("IHDR"));
        bytes.AddRange(ihdrData);
        AppendInt32BigEndian(bytes, unchecked((int)0xDEADBEEF));   // bogus CRC32
        AppendChunk(bytes, "IEND", Array.Empty<byte>());

        Action act = () => PngDecoder.Decode(bytes.ToArray());
        act.Should().Throw<PngDecoderException>().WithMessage("*CRC32 mismatch*");
    }

    // ============================================================
    // V0.C.1 Commit 4 (Part 2) — DEFLATE + filter unfiltering positive tests
    // ============================================================

    [Fact]
    public void Decode_1x1Rgba_Filter0_Roundtrips()
    {
        // 1×1 RGBA red opaque (filter type 0 = None).
        byte[] pixels = { 200, 50, 75, 255 };
        byte[] png = BuildValidPng(1, 1, colorType: 6, pixels: pixels, filterType: 0);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.Width.Should().Be(1);
        decoded.Height.Should().Be(1);
        decoded.PixelsRgba8.Should().BeEquivalentTo(pixels);
    }

    [Fact]
    public void Decode_2x2Rgba_Filter0_Roundtrips()
    {
        // 2×2 RGBA: 4 distinct pixels.
        byte[] pixels =
        {
             10,  20,  30, 255,    40,  50,  60, 255,
             70,  80,  90, 200,   100, 110, 120, 150,
        };
        byte[] png = BuildValidPng(2, 2, colorType: 6, pixels: pixels, filterType: 0);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.Width.Should().Be(2);
        decoded.Height.Should().Be(2);
        decoded.PixelsRgba8.Should().BeEquivalentTo(pixels);
    }

    [Fact]
    public void Decode_2x2Rgb8_Filter0_ExpandsToRgba8()
    {
        // 2×2 RGB (no alpha channel) — decoder converts к RGBA8 с alpha=255.
        byte[] pixels =
        {
             10,  20,  30,    40,  50,  60,
             70,  80,  90,   100, 110, 120,
        };
        byte[] png = BuildValidPng(2, 2, colorType: 2, pixels: pixels, filterType: 0);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.PixelsRgba8.Should().HaveCount(2 * 2 * 4);
        // Verify alpha = 255 для every pixel + RGB preserved.
        for (int i = 0; i < 4; i++)
        {
            decoded.PixelsRgba8[i * 4 + 0].Should().Be(pixels[i * 3 + 0]);
            decoded.PixelsRgba8[i * 4 + 1].Should().Be(pixels[i * 3 + 1]);
            decoded.PixelsRgba8[i * 4 + 2].Should().Be(pixels[i * 3 + 2]);
            decoded.PixelsRgba8[i * 4 + 3].Should().Be(255);
        }
    }

    [Fact]
    public void Decode_2x2Rgba_Filter1_Sub_Roundtrips()
    {
        byte[] pixels =
        {
             10,  20,  30, 255,    40,  50,  60, 255,
             70,  80,  90, 200,   100, 110, 120, 150,
        };
        byte[] png = BuildValidPng(2, 2, colorType: 6, pixels: pixels, filterType: 1);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.PixelsRgba8.Should().BeEquivalentTo(pixels);
    }

    [Fact]
    public void Decode_2x2Rgba_Filter2_Up_Roundtrips()
    {
        byte[] pixels =
        {
             10,  20,  30, 255,    40,  50,  60, 255,
             70,  80,  90, 200,   100, 110, 120, 150,
        };
        byte[] png = BuildValidPng(2, 2, colorType: 6, pixels: pixels, filterType: 2);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.PixelsRgba8.Should().BeEquivalentTo(pixels);
    }

    [Fact]
    public void Decode_2x2Rgba_Filter3_Average_Roundtrips()
    {
        byte[] pixels =
        {
             10,  20,  30, 255,    40,  50,  60, 255,
             70,  80,  90, 200,   100, 110, 120, 150,
        };
        byte[] png = BuildValidPng(2, 2, colorType: 6, pixels: pixels, filterType: 3);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.PixelsRgba8.Should().BeEquivalentTo(pixels);
    }

    [Fact]
    public void Decode_2x2Rgba_Filter4_Paeth_Roundtrips()
    {
        byte[] pixels =
        {
             10,  20,  30, 255,    40,  50,  60, 255,
             70,  80,  90, 200,   100, 110, 120, 150,
        };
        byte[] png = BuildValidPng(2, 2, colorType: 6, pixels: pixels, filterType: 4);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.PixelsRgba8.Should().BeEquivalentTo(pixels);
    }

    [Fact]
    public void Decode_8x8Rgba_GradientPattern_Roundtrips()
    {
        // Larger image to verify scanline indexing + multi-IDAT path (single IDAT in our test).
        byte[] pixels = new byte[8 * 8 * 4];
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int idx = (y * 8 + x) * 4;
                pixels[idx + 0] = (byte)(x * 32);
                pixels[idx + 1] = (byte)(y * 32);
                pixels[idx + 2] = (byte)((x + y) * 16);
                pixels[idx + 3] = 255;
            }
        }
        byte[] png = BuildValidPng(8, 8, colorType: 6, pixels: pixels, filterType: 0);
        PngImage decoded = PngDecoder.Decode(png);
        decoded.Width.Should().Be(8);
        decoded.Height.Should().Be(8);
        decoded.PixelsRgba8.Should().BeEquivalentTo(pixels);
    }

    // ============================================================
    // Synthetic PNG construction with DEFLATE + filter
    // ============================================================

    /// <summary>
    /// Build a fully valid PNG byte sequence from raw RGBA8 or RGB8 pixel data + applied filter.
    /// Used by Part 2 positive tests to exercise full Decode path including DEFLATE + filter
    /// unfiltering. Filter algorithms applied INVERSE to PngDecoder's reconstruction.
    /// </summary>
    private static byte[] BuildValidPng(int width, int height, byte colorType, byte[] pixels, byte filterType)
    {
        int bytesPerPixel = colorType == 6 ? 4 : 3;
        int scanlineLength = width * bytesPerPixel;
        if (pixels.Length != scanlineLength * height)
        {
            throw new ArgumentException("Pixel buffer size mismatch.");
        }

        // 1. Apply filter to each scanline + prefix filter type byte.
        byte[] filtered = new byte[(scanlineLength + 1) * height];
        byte[] previousScanline = new byte[scanlineLength];
        for (int y = 0; y < height; y++)
        {
            int srcOffset = y * scanlineLength;
            int dstOffset = y * (scanlineLength + 1);
            filtered[dstOffset] = filterType;
            ReadOnlySpan<byte> current = pixels.AsSpan(srcOffset, scanlineLength);

            switch (filterType)
            {
                case 0:
                    current.CopyTo(filtered.AsSpan(dstOffset + 1, scanlineLength));
                    break;
                case 1:
                    // Sub(x) = Raw(x) - Raw(x - bpp).
                    for (int i = 0; i < scanlineLength; i++)
                    {
                        byte left = i >= bytesPerPixel ? current[i - bytesPerPixel] : (byte)0;
                        filtered[dstOffset + 1 + i] = (byte)(current[i] - left);
                    }
                    break;
                case 2:
                    // Up(x) = Raw(x) - Prior(x).
                    for (int i = 0; i < scanlineLength; i++)
                    {
                        filtered[dstOffset + 1 + i] = (byte)(current[i] - previousScanline[i]);
                    }
                    break;
                case 3:
                    // Average(x) = Raw(x) - floor((Raw(x - bpp) + Prior(x)) / 2).
                    for (int i = 0; i < scanlineLength; i++)
                    {
                        int left = i >= bytesPerPixel ? current[i - bytesPerPixel] : 0;
                        int up = previousScanline[i];
                        filtered[dstOffset + 1 + i] = (byte)(current[i] - ((left + up) >> 1));
                    }
                    break;
                case 4:
                    // Paeth(x) = Raw(x) - PaethPredictor(Raw(x - bpp), Prior(x), Prior(x - bpp)).
                    for (int i = 0; i < scanlineLength; i++)
                    {
                        byte a = i >= bytesPerPixel ? current[i - bytesPerPixel] : (byte)0;
                        byte b = previousScanline[i];
                        byte c = i >= bytesPerPixel ? previousScanline[i - bytesPerPixel] : (byte)0;
                        filtered[dstOffset + 1 + i] = (byte)(current[i] - PaethPredictor(a, b, c));
                    }
                    break;
                default:
                    throw new ArgumentException($"Unsupported filter type {filterType}.");
            }

            current.CopyTo(previousScanline);
        }

        // 2. DEFLATE compress, prepend zlib header.
        byte[] compressed = DeflateCompress(filtered);
        byte[] zlibWrapped = WrapZlib(compressed);

        // 3. Construct PNG byte sequence.
        var bytes = new List<byte>(PngSignature);
        AppendChunk(bytes, "IHDR", BuildIhdr(width, height, bitDepth: 8, colorType: colorType));
        AppendChunk(bytes, "IDAT", zlibWrapped);
        AppendChunk(bytes, "IEND", Array.Empty<byte>());
        return bytes.ToArray();
    }

    private static byte PaethPredictor(byte a, byte b, byte c)
    {
        int p = a + b - c;
        int pa = Math.Abs(p - a);
        int pb = Math.Abs(p - b);
        int pc = Math.Abs(p - c);
        if (pa <= pb && pa <= pc) return a;
        if (pb <= pc) return b;
        return c;
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

    private static byte[] WrapZlib(byte[] deflateData)
    {
        // Minimal zlib wrapper: CMF = 0x78 (32K window, DEFLATE method 8) + FLG (FCHECK matching CMF).
        // RFC 1950 §2.2 — (CMF*256 + FLG) must be divisible by 31. CMF=0x78 → FLG=0x9C works.
        // Adler-32 trailer omitted: PngDecoder skips 2-byte zlib header + DeflateStream
        // decodes raw DEFLATE stream end-marker; trailing Adler-32 not consumed by our decoder.
        var output = new byte[deflateData.Length + 2];
        output[0] = 0x78;
        output[1] = 0x9C;
        Buffer.BlockCopy(deflateData, 0, output, 2, deflateData.Length);
        return output;
    }

    // ============================================================
    // Test helpers (construct synthetic PNG byte sequences)
    // ============================================================

    /// <summary>
    /// Build minimal-but-valid PNG byte sequence with given IHDR parameters and one empty
    /// IDAT chunk + IEND. Used in Part 1 tests to exercise IHDR validation paths without
    /// actually requiring DEFLATE/filter logic (Part 2 will exercise IDAT contents).
    /// </summary>
    private static byte[] BuildMinimalPng(int width, int height, byte bitDepth, byte colorType,
        byte compressionMethod = 0, byte filterMethod = 0, byte interlaceMethod = 0)
    {
        var bytes = new List<byte>(PngSignature);
        AppendChunk(bytes, "IHDR",
            BuildIhdr(width, height, bitDepth, colorType, compressionMethod, filterMethod, interlaceMethod));
        AppendChunk(bytes, "IDAT", new byte[] { 0x78, 0x01 });  // minimal zlib header placeholder
        AppendChunk(bytes, "IEND", Array.Empty<byte>());
        return bytes.ToArray();
    }

    private static byte[] BuildIhdr(int width, int height, byte bitDepth, byte colorType,
        byte compressionMethod = 0, byte filterMethod = 0, byte interlaceMethod = 0)
    {
        var ihdr = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(ihdr.AsSpan(0, 4), width);
        BinaryPrimitives.WriteInt32BigEndian(ihdr.AsSpan(4, 4), height);
        ihdr[8] = bitDepth;
        ihdr[9] = colorType;
        ihdr[10] = compressionMethod;
        ihdr[11] = filterMethod;
        ihdr[12] = interlaceMethod;
        return ihdr;
    }

    private static void AppendChunk(List<byte> bytes, string type, byte[] data)
    {
        AppendInt32BigEndian(bytes, data.Length);
        byte[] typeBytes = Encoding.ASCII.GetBytes(type);
        bytes.AddRange(typeBytes);
        bytes.AddRange(data);
        uint crc = ComputeCrc32(typeBytes, data);
        AppendInt32BigEndian(bytes, unchecked((int)crc));
    }

    private static void AppendInt32BigEndian(List<byte> bytes, int value)
    {
        bytes.Add((byte)(value >> 24));
        bytes.Add((byte)(value >> 16));
        bytes.Add((byte)(value >> 8));
        bytes.Add((byte)value);
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
