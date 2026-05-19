namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Decoded PNG image — width × height pixels in RGBA8 byte order (tightly packed,
/// no row padding). RGB8 source PNGs are converted to RGBA8 at decode time with
/// alpha = 255 (fully opaque) per Q2 (a) ratification.
/// </summary>
/// <param name="Width">Image width in pixels.</param>
/// <param name="Height">Image height in pixels.</param>
/// <param name="PixelsRgba8">Tightly packed RGBA8 pixel data; length = Width × Height × 4.</param>
public sealed record PngImage(int Width, int Height, byte[] PixelsRgba8);
