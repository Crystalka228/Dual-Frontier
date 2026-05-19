namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Thrown by <see cref="PngDecoder"/> when an unsupported PNG variant is encountered
/// (interlaced, palette, grayscale, 16-bit channels) or when PNG data is malformed
/// (bad signature, missing chunks, CRC32 mismatch, corrupt DEFLATE stream).
/// </summary>
public sealed class PngDecoderException : Exception
{
    public PngDecoderException(string message) : base(message) { }
    public PngDecoderException(string message, Exception innerException) : base(message, innerException) { }
}
