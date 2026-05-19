using System.Buffers.Binary;
using System.Text;
using DualFrontier.Runtime.Assets;
using FluentAssertions;
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
