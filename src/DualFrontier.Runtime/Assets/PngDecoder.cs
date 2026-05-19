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
    // DEFLATE decompression + filter unfiltering (Part 2 — V0.C.1 Commit 4)
    // =====================================================================

    private static byte[] DecompressAndUnfilter(List<byte[]> idatChunks, int width, int height, int colorType)
    {
        int bytesPerPixel = colorType == 6 ? 4 : 3;  // RGBA или RGB

        // 1. Concatenate IDAT bytes.
        int totalLength = 0;
        foreach (byte[] c in idatChunks)
        {
            totalLength += c.Length;
        }
        byte[] compressed = new byte[totalLength];
        int pos = 0;
        foreach (byte[] c in idatChunks)
        {
            Buffer.BlockCopy(c, 0, compressed, pos, c.Length);
            pos += c.Length;
        }

        if (compressed.Length < 2)
        {
            throw new PngDecoderException("IDAT data too short to contain zlib header.");
        }

        // 2. Skip 2-byte zlib header (CMF + FLG per RFC 1950 §2.2). Verify CMF compression
        // method = 8 (DEFLATE) per RFC 1950 §2.2 + §2.4. FLG check (FCHECK + FDICT + FLEVEL)
        // not strictly needed for decode; rely on DeflateStream for DEFLATE-level validation.
        byte cmf = compressed[0];
        if ((cmf & 0x0F) != 8)
        {
            throw new PngDecoderException($"Unsupported zlib compression method {cmf & 0x0F}; expected 8 (DEFLATE).");
        }

        // 3. DEFLATE decompression via System.IO.Compression.DeflateStream (BCL per §0 L5).
        int scanlineLength = width * bytesPerPixel + 1;  // +1 для filter type byte per RFC 2083 §6
        int rawTotal = scanlineLength * height;
        byte[] raw;
        try
        {
            using var ms = new MemoryStream(compressed, index: 2, count: compressed.Length - 2, writable: false);
            using var deflate = new DeflateStream(ms, CompressionMode.Decompress);
            using var rawMs = new MemoryStream(rawTotal);
            deflate.CopyTo(rawMs);
            raw = rawMs.ToArray();
        }
        catch (InvalidDataException ex)
        {
            throw new PngDecoderException("Corrupt DEFLATE stream in IDAT chunks.", ex);
        }

        if (raw.Length != rawTotal)
        {
            throw new PngDecoderException(
                $"DEFLATE output size {raw.Length} bytes does not match expected {rawTotal} bytes ({width}×{height}, {bytesPerPixel} bpp).");
        }

        // 4. Unfilter scanlines (per RFC 2083 §6).
        byte[] unfiltered = new byte[width * bytesPerPixel * height];
        UnfilterScanlines(raw, unfiltered, width, height, bytesPerPixel);
        return unfiltered;
    }

    private static void UnfilterScanlines(byte[] raw, byte[] unfiltered, int width, int height, int bytesPerPixel)
    {
        int scanlineLength = width * bytesPerPixel;
        int rawScanlineLength = scanlineLength + 1;  // +1 filter byte prefix per RFC 2083 §6

        // Previous scanline initialized to zeros per RFC 2083 §6.3 (Filter Algorithm: prior row
        // of scanline 0 is virtual all-zero scanline).
        byte[] previousScanline = new byte[scanlineLength];

        for (int y = 0; y < height; y++)
        {
            int rawOffset = y * rawScanlineLength;
            byte filterType = raw[rawOffset];
            ReadOnlySpan<byte> currentRaw = raw.AsSpan(rawOffset + 1, scanlineLength);
            Span<byte> currentOut = unfiltered.AsSpan(y * scanlineLength, scanlineLength);

            switch (filterType)
            {
                case 0: UnfilterNone(currentRaw, currentOut); break;
                case 1: UnfilterSub(currentRaw, currentOut, bytesPerPixel); break;
                case 2: UnfilterUp(currentRaw, currentOut, previousScanline); break;
                case 3: UnfilterAverage(currentRaw, currentOut, previousScanline, bytesPerPixel); break;
                case 4: UnfilterPaeth(currentRaw, currentOut, previousScanline, bytesPerPixel); break;
                default:
                    throw new PngDecoderException($"Unknown filter type {filterType} on scanline {y}.");
            }

            currentOut.CopyTo(previousScanline);
        }
    }

    private static void UnfilterNone(ReadOnlySpan<byte> raw, Span<byte> output)
    {
        raw.CopyTo(output);
    }

    private static void UnfilterSub(ReadOnlySpan<byte> raw, Span<byte> output, int bytesPerPixel)
    {
        // RFC 2083 §6.3: Sub(x) = Raw(x) - Raw(x - bpp); reconstructed: Raw(x) = Sub(x) + Recon(x - bpp).
        for (int i = 0; i < raw.Length; i++)
        {
            byte left = i >= bytesPerPixel ? output[i - bytesPerPixel] : (byte)0;
            output[i] = (byte)(raw[i] + left);
        }
    }

    private static void UnfilterUp(ReadOnlySpan<byte> raw, Span<byte> output, byte[] previousScanline)
    {
        // RFC 2083 §6.4: Up(x) = Raw(x) - Prior(x); reconstructed: Raw(x) = Up(x) + Prior(x).
        for (int i = 0; i < raw.Length; i++)
        {
            output[i] = (byte)(raw[i] + previousScanline[i]);
        }
    }

    private static void UnfilterAverage(ReadOnlySpan<byte> raw, Span<byte> output, byte[] previousScanline, int bytesPerPixel)
    {
        // RFC 2083 §6.5: Average(x) = Raw(x) - floor((Raw(x - bpp) + Prior(x)) / 2).
        // Reconstruction uses integer division (floor) per RFC.
        for (int i = 0; i < raw.Length; i++)
        {
            int left = i >= bytesPerPixel ? output[i - bytesPerPixel] : 0;
            int up = previousScanline[i];
            output[i] = (byte)(raw[i] + ((left + up) >> 1));
        }
    }

    private static void UnfilterPaeth(ReadOnlySpan<byte> raw, Span<byte> output, byte[] previousScanline, int bytesPerPixel)
    {
        // RFC 2083 §6.6: Paeth(x) = Raw(x) - PaethPredictor(Recon(a), Recon(b), Recon(c)).
        for (int i = 0; i < raw.Length; i++)
        {
            byte a = i >= bytesPerPixel ? output[i - bytesPerPixel] : (byte)0;             // left
            byte b = previousScanline[i];                                                    // above
            byte c = i >= bytesPerPixel ? previousScanline[i - bytesPerPixel] : (byte)0;    // upper-left
            output[i] = (byte)(raw[i] + PaethPredictor(a, b, c));
        }
    }

    private static byte PaethPredictor(byte a, byte b, byte c)
    {
        // RFC 2083 §6.6 PaethPredictor algorithm.
        int p = a + b - c;
        int pa = Math.Abs(p - a);
        int pb = Math.Abs(p - b);
        int pc = Math.Abs(p - c);
        if (pa <= pb && pa <= pc)
        {
            return a;
        }
        if (pb <= pc)
        {
            return b;
        }
        return c;
    }

    private static byte[] ConvertRgb8ToRgba8(byte[] rgb, int width, int height)
    {
        byte[] rgba = new byte[width * height * 4];
        int srcIdx = 0;
        int dstIdx = 0;
        for (int i = 0; i < width * height; i++)
        {
            rgba[dstIdx + 0] = rgb[srcIdx + 0];
            rgba[dstIdx + 1] = rgb[srcIdx + 1];
            rgba[dstIdx + 2] = rgb[srcIdx + 2];
            rgba[dstIdx + 3] = 255;  // fully opaque per Q2 (a) ratification
            srcIdx += 3;
            dstIdx += 4;
        }
        return rgba;
    }
}
