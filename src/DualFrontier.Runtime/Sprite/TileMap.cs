using System.Numerics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 TileMap class per S-LOCK-5 (Q4a one-sprite-per-tile ratification).
/// 2D grid of tiles, each tile = AtlasRegion + tint over shared SpriteTexture atlas.
/// Submit() enumerates all tiles + invokes SpriteRenderer.Submit per tile.
///
/// 200×200 stress = 40,000 tiles = 4× R.2 sprite stress target. If total tile count
/// exceeds SpriteRenderer.MaxSpritesPerFrame (10K hard cap per S-LOCK-3a), caller is
/// responsible for invoking multiple BeginFrame/EndFrame cycles per S-LOCK-5a.
///
/// Out of scope per S-LOCK-5 «features only on demand»:
///   - Visible-tile culling (Camera2D AABB intersection) — deferred к Lesson #25
///   - Chunked rendering (32×32 tile chunks с per-chunk vertex buffer) — deferred
///   - GPU-instanced TileMap rendering — deferred
/// </summary>
public sealed class TileMap : IDisposable
{
    private readonly AtlasRegion[] _regions;
    private readonly uint[] _tints;
    private bool _disposed;

    public int Width { get; }
    public int Height { get; }
    public float TileSize { get; }
    public SpriteTexture Atlas { get; }
    public int TotalTiles => Width * Height;

    public TileMap(int width, int height, float tileSize, SpriteTexture atlas)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
        }
        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
        }
        if (tileSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tileSize), "Tile size must be positive.");
        }
        ArgumentNullException.ThrowIfNull(atlas);

        Width = width;
        Height = height;
        TileSize = tileSize;
        Atlas = atlas;

        int count = width * height;
        _regions = new AtlasRegion[count];
        _tints = new uint[count];
        for (int i = 0; i < count; i++)
        {
            _tints[i] = SpriteVertex.WhiteTint;
        }
    }

    /// <summary>Set tile at (x,y) к specified atlas region. Tint stays unchanged (default white).</summary>
    public void SetTile(int x, int y, AtlasRegion region)
    {
        ValidateCoords(x, y);
        _regions[y * Width + x] = region;
    }

    /// <summary>Set tile at (x,y) с specified atlas region + tint.</summary>
    public void SetTile(int x, int y, AtlasRegion region, uint tintRgba)
    {
        ValidateCoords(x, y);
        int idx = y * Width + x;
        _regions[idx] = region;
        _tints[idx] = tintRgba;
    }

    public AtlasRegion GetTile(int x, int y)
    {
        ValidateCoords(x, y);
        return _regions[y * Width + x];
    }

    public uint GetTint(int x, int y)
    {
        ValidateCoords(x, y);
        return _tints[y * Width + x];
    }

    /// <summary>
    /// Submit all tiles к SpriteRenderer. Caller must have called BeginFrame on the renderer.
    /// V0.C.2 V0: enumerates всё (no culling). Future culling extension: filter via Camera2D AABB.
    /// If total tile count exceeds renderer.MaxSpritesPerFrame, multiple cycles required (S-LOCK-5a).
    /// </summary>
    public void Submit(SpriteRenderer renderer, Camera2D camera)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(renderer);
        ArgumentNullException.ThrowIfNull(camera);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int idx = y * Width + x;
                var region = _regions[idx];
                var tint = _tints[idx];

                var sprite = new Sprite(
                    Texture: Atlas,
                    Region: region,
                    Transform: new SpriteTransform(
                        Position: new Vector2(x * TileSize, y * TileSize),
                        Scale: new Vector2(TileSize, TileSize),
                        Rotation: 0f,
                        TintRgba: tint));
                renderer.Submit(sprite);
            }
        }
    }

    private void ValidateCoords(int x, int y)
    {
        if ((uint)x >= Width || (uint)y >= Height)
        {
            throw new ArgumentOutOfRangeException(
                $"Tile coordinates ({x},{y}) out of TileMap bounds {Width}x{Height}.");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        // Atlas is caller-owned — not disposed by TileMap.
        _disposed = true;
    }
}
