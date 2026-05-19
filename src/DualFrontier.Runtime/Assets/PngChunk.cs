namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Internal PNG chunk record per RFC 2083 §3.2: 4-byte ASCII type code + data bytes
/// + stored CRC32 (used for verification against computed CRC32 over type + data).
/// </summary>
internal readonly record struct PngChunk(string Type, byte[] Data, uint Crc32Stored);
