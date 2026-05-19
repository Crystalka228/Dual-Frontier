using System.Buffers.Binary;
using System.IO.Compression;

namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Manual PNG decoder. V0.C.1 scope per S-LOCK-2: 8-bit RGB / RGBA, non-interlaced,
/// no palette, no color management. Uses <see cref="DeflateStream"/> (BCL) for DEFLATE
/// decompression per §0 L5 + RFC 1950/1951 zlib + DEFLATE.
///
/// Implementation split across V0.C.1 Commit 3 + Commit 4 per brief atomic cascade:
///   Commit 3 (Part 1): PNG signature + chunk parsing + CRC32 verification + IHDR validation
///                       + IDAT collection + IEND detection.
///   Commit 4 (Part 2): DEFLATE decompression + filter unfiltering (None/Sub/Up/Average/Paeth)
///                       + RGB8 → RGBA8 conversion.
/// </summary>
public static class PngDecoder
{
    // RFC 2083 §3.1: PNG signature.
    private static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

    /// <summary>
    /// Decode PNG bytes to RGBA8 pixel data. RGB8 source converted to RGBA8 with alpha=255.
    /// </summary>
    /// <exception cref="PngDecoderException">Thrown for unsupported PNG variants or malformed data.</exception>
    public static PngImage Decode(ReadOnlySpan<byte> pngBytes)
    {
        // 1. Verify PNG signature (first 8 bytes per RFC 2083 §3.1).
        if (pngBytes.Length < 8 || !pngBytes[..8].SequenceEqual(PngSignature))
        {
            throw new PngDecoderException("Invalid PNG signature.");
        }

        // 2. Parse chunks sequentially.
        var idatChunks = new List<byte[]>();
        int width = 0, height = 0;
        int bitDepth = 0, colorType = 0;
        bool seenIhdr = false, seenIend = false;

        int offset = 8;
        while (offset < pngBytes.Length)
        {
            PngChunk chunk = ReadChunk(pngBytes, ref offset);

            // CRC32 verification mandatory per RFC 2083 §5.3.
            if (!VerifyChunkCrc32(chunk))
            {
                throw new PngDecoderException($"Chunk '{chunk.Type}' CRC32 mismatch.");
            }

            switch (chunk.Type)
            {
                case "IHDR":
                    if (seenIhdr)
                    {
                        throw new PngDecoderException("Duplicate IHDR chunk.");
                    }
                    seenIhdr = true;
                    if (chunk.Data.Length != 13)
                    {
                        throw new PngDecoderException($"IHDR chunk wrong size: expected 13, got {chunk.Data.Length}.");
                    }
                    width = ReadBigEndianInt32(chunk.Data, 0);
                    height = ReadBigEndianInt32(chunk.Data, 4);
                    bitDepth = chunk.Data[8];
                    colorType = chunk.Data[9];
                    int compressionMethod = chunk.Data[10];
                    int filterMethod = chunk.Data[11];
                    int interlaceMethod = chunk.Data[12];

                    // S-LOCK-2 scope checks. Explicit rejection per Lesson #20 «no improvisation»:
                    // produce wrong pixels never; reject unsupported variants honestly.
                    if (width <= 0 || height <= 0)
                    {
                        throw new PngDecoderException($"Invalid image dimensions: {width}×{height}.");
                    }
                    if (bitDepth != 8)
                    {
                        throw new PngDecoderException($"Unsupported bit depth {bitDepth}; only 8 supported.");
                    }
                    if (colorType != 2 && colorType != 6)
                    {
                        throw new PngDecoderException(
                            $"Unsupported color type {colorType}; only 2 (RGB) and 6 (RGBA) supported.");
                    }
                    if (compressionMethod != 0)
                    {
                        throw new PngDecoderException($"Unsupported compression method {compressionMethod}.");
                    }
                    if (filterMethod != 0)
                    {
                        throw new PngDecoderException($"Unsupported filter method {filterMethod}.");
                    }
                    if (interlaceMethod != 0)
                    {
                        throw new PngDecoderException("Interlaced PNG not supported.");
                    }
                    break;

                case "IDAT":
                    if (!seenIhdr)
                    {
                        throw new PngDecoderException("IDAT chunk before IHDR.");
                    }
                    idatChunks.Add(chunk.Data);
                    break;

                case "IEND":
                    if (!seenIhdr)
                    {
                        throw new PngDecoderException("IEND chunk before IHDR.");
                    }
                    seenIend = true;
                    break;

                default:
                    // Ancillary chunk — silently ignored per S-LOCK-2 (gAMA/sRGB/tEXt/tIME/etc).
                    break;
            }

            if (seenIend)
            {
                break;
            }
        }

        if (!seenIhdr)
        {
            throw new PngDecoderException("Missing IHDR chunk.");
        }
        if (!seenIend)
        {
            throw new PngDecoderException("Missing IEND chunk.");
        }
        if (idatChunks.Count == 0)
        {
            throw new PngDecoderException("No IDAT chunks.");
        }

        // 3. Decompress + unfilter (Part 2 — Commit 4).
        byte[] pixelBytes = DecompressAndUnfilter(idatChunks, width, height, colorType);

        // 4. Convert to RGBA8 (if RGB8 source).
        byte[] rgba8 = colorType == 6 ? pixelBytes : ConvertRgb8ToRgba8(pixelBytes, width, height);

        return new PngImage(width, height, rgba8);
    }

    /// <summary>Convenience load from file path.</summary>
    public static PngImage DecodeFile(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        byte[] bytes = File.ReadAllBytes(path);
        return Decode(bytes);
    }

    // =====================================================================
    // Chunk parsing (Part 1)
    // =====================================================================

    private static PngChunk ReadChunk(ReadOnlySpan<byte> data, ref int offset)
    {
        // Chunk layout per RFC 2083 §3.2: length (4) + type (4) + data (length) + CRC32 (4).
        if (offset + 8 > data.Length)
        {
            throw new PngDecoderException("Truncated PNG (chunk header).");
        }

        int length = ReadBigEndianInt32(data, offset);
        if (length < 0)
        {
            throw new PngDecoderException($"Invalid chunk length {length}.");
        }
        offset += 4;

        if (offset + 4 + length + 4 > data.Length)
        {
            throw new PngDecoderException($"Truncated PNG (chunk body, type={offset}).");
        }

        // Chunk type — 4 ASCII bytes per RFC 2083 §3.2.
        string type = System.Text.Encoding.ASCII.GetString(data.Slice(offset, 4));
        offset += 4;

        byte[] chunkData = data.Slice(offset, length).ToArray();
        offset += length;

        uint crc32Stored = (uint)ReadBigEndianInt32(data, offset);
        offset += 4;

        return new PngChunk(type, chunkData, crc32Stored);
    }

    private static bool VerifyChunkCrc32(PngChunk chunk)
    {
        // CRC32 covers chunk type + chunk data per RFC 2083 §5.3.
        byte[] typeBytes = System.Text.Encoding.ASCII.GetBytes(chunk.Type);
        uint crc = Crc32Update(0xFFFFFFFFu, typeBytes);
        crc = Crc32Update(crc, chunk.Data);
        crc ^= 0xFFFFFFFFu;
        return crc == chunk.Crc32Stored;
    }

    private static readonly uint[] Crc32Table = BuildCrc32Table();

    private static uint[] BuildCrc32Table()
    {
        // PNG uses standard CRC-32-IEEE polynomial 0xEDB88320 per RFC 2083 §5.3 (Crc Table).
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

    private static uint Crc32Update(uint crc, ReadOnlySpan<byte> data)
    {
        uint c = crc;
        for (int i = 0; i < data.Length; i++)
        {
            c = Crc32Table[(c ^ data[i]) & 0xFF] ^ (c >> 8);
        }
        return c;
    }

    private static int ReadBigEndianInt32(ReadOnlySpan<byte> data, int offset)
    {
        return BinaryPrimitives.ReadInt32BigEndian(data.Slice(offset, 4));
    }

    // =====================================================================
    // DEFLATE decompression + filter unfiltering (Part 2 — Commit 4 implements)
    // =====================================================================

    private static byte[] DecompressAndUnfilter(List<byte[]> idatChunks, int width, int height, int colorType)
    {
        throw new PngDecoderException(
            "DecompressAndUnfilter not yet implemented; lands in V0.C.1 Commit 4 (Part 2).");
    }

    private static byte[] ConvertRgb8ToRgba8(byte[] rgb, int width, int height)
    {
        throw new PngDecoderException(
            "ConvertRgb8ToRgba8 not yet implemented; lands in V0.C.1 Commit 4 (Part 2).");
    }
}
