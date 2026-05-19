using DualFrontier.Runtime.Assets;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Runtime.SmokeTest;

/// <summary>
/// V0.C.2 procedural atlas generator для 10K sprite stress + 200×200 TileMap smoke tests.
/// Generates 256×256 RGBA8 atlas with 16×16 tile grid = 256 distinct tile types. Each tile
/// gets distinct color base + pattern modulation (solid / checker / gradient / ring).
/// </summary>
public static class ProceduralAtlas
{
    public const int AtlasWidth = 256;
    public const int AtlasHeight = 256;
    public const int TileWidth = 16;
    public const int TileHeight = 16;
    public const int TilesPerRow = AtlasWidth / TileWidth;        // 16
    public const int TilesPerColumn = AtlasHeight / TileHeight;   // 16
    public const int TotalTiles = TilesPerRow * TilesPerColumn;   // 256

    public static PngImage GenerateAtlas()
    {
        int pixelCount = AtlasWidth * AtlasHeight;
        byte[] pixels = new byte[pixelCount * 4];

        for (int tileIdx = 0; tileIdx < TotalTiles; tileIdx++)
        {
            int tileX = tileIdx % TilesPerRow;
            int tileY = tileIdx / TilesPerRow;
            FillTile(pixels, tileX, tileY, tileIdx);
        }

        return new PngImage(AtlasWidth, AtlasHeight, pixels);
    }

    private static void FillTile(byte[] pixels, int tileX, int tileY, int tileIdx)
    {
        // Each tile gets distinct color base + pattern modulation.
        byte baseR = (byte)((tileIdx * 37) % 256);
        byte baseG = (byte)((tileIdx * 71) % 256);
        byte baseB = (byte)((tileIdx * 113) % 256);

        int patternType = tileIdx % 4;  // 4 pattern types

        for (int py = 0; py < TileHeight; py++)
        {
            for (int px = 0; px < TileWidth; px++)
            {
                int pxAbs = tileX * TileWidth + px;
                int pyAbs = tileY * TileHeight + py;
                int idx = (pyAbs * AtlasWidth + pxAbs) * 4;

                byte r = baseR, g = baseG, b = baseB;

                switch (patternType)
                {
                    case 0: // Solid (base color)
                        break;
                    case 1: // Checker
                        if (((px / 4) + (py / 4)) % 2 == 1)
                        {
                            r ^= 0x55; g ^= 0x55; b ^= 0x55;
                        }
                        break;
                    case 2: // Horizontal gradient
                        r = (byte)((r + px * 8) & 0xFF);
                        break;
                    case 3: // Ring
                        int dx = px - 8, dy = py - 8;
                        int d2 = dx * dx + dy * dy;
                        if (d2 > 32 && d2 < 56)
                        {
                            r ^= 0xAA; g ^= 0xAA; b ^= 0xAA;
                        }
                        break;
                }

                pixels[idx + 0] = r;
                pixels[idx + 1] = g;
                pixels[idx + 2] = b;
                pixels[idx + 3] = 255;  // fully opaque
            }
        }
    }

    /// <summary>Returns AtlasRegion для specific tile index (0..TotalTiles-1).</summary>
    public static AtlasRegion GetTileRegion(int tileIndex)
    {
        if ((uint)tileIndex >= TotalTiles)
        {
            throw new ArgumentOutOfRangeException(nameof(tileIndex));
        }
        int tileX = tileIndex % TilesPerRow;
        int tileY = tileIndex / TilesPerRow;
        return AtlasRegion.FromPixels(
            x: tileX * TileWidth,
            y: tileY * TileHeight,
            width: TileWidth,
            height: TileHeight,
            textureWidth: AtlasWidth,
            textureHeight: AtlasHeight);
    }
}
