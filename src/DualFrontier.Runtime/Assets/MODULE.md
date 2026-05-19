# DualFrontier.Runtime.Assets

Asset loading + decoding module. V0.C.1 scope: PNG decoder + AssetManager (path resolution + caching).
Future expansion: bitmap fonts, audio, JSON manifest loading, hot reload — extended via similar
loader patterns (PngDecoder + AssetManager) when consumer materializes per Lesson #25.

## Module purpose (per VULKAN_SUBSTRATE.md §2.2 + §2.6)

- **Centralized asset path resolution** within configurable root directory (default `assets/`).
- **Decoded asset caching** to avoid redundant decode work across rendering frames.
- **Path traversal protection** (asset paths cannot escape the root via `..` segments).
- **Format scope discipline** (V0.C.1 supports PNG RGBA8/RGB8 8-bit non-interlaced only; other
  variants throw explicit `PngDecoderException` rather than silently producing wrong pixels —
  per S-LOCK-2 + Lesson #20 «no improvisation»).

## V0.C.1 surface

- `AssetPath` — typed wrapper struct (relative path within asset root).
- `AssetManager` — `IDisposable`; LoadPng (cached); path resolution.
- `PngImage` — decoded image record (width × height × RGBA8 byte[]).
- `PngDecoder` — static decoder; `Decode(ReadOnlySpan<byte>)` + `DecodeFile(string)`.
- `PngDecoderException` — thrown for unsupported variants or malformed data.

## Dependency rules

Per VULKAN_SUBSTRATE.md §2.4 Rule 1-5: Assets module depends only on BCL (no Graphics, no Vulkan,
no Sprite). PngDecoder uses `System.IO.Compression.DeflateStream` per §0 L5 (BCL DEFLATE
implementation, no third-party PNG library).

## Out-of-scope (V0.C.1)

- Interlaced PNG (Adam7) — throws exception
- Palette indexed color (color type 3) — throws
- Grayscale (color type 0) / Grayscale+Alpha (color type 4) — throws (Kenney atlases are RGB/RGBA)
- 16-bit channels (bit depth 16) — throws
- Color management chunks (gAMA, sRGB, cHRM, iCCP) — silently ignored
- Ancillary chunks (tEXt, tIME, etc.) — silently ignored
