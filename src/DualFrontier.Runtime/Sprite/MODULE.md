# DualFrontier.Runtime.Sprite

Sprite rendering module. V0.C.1 scope: vertex format + sprite primitive types (texture handle +
atlas region + transform) + single-sprite-per-draw renderer. V0.C.2 will refactor SpriteRenderer
к batched (10,000 sprites at 60+ FPS per VULKAN_SUBSTRATE §4.2 R.2) + add Camera2D class +
TileMap rendering (R.3).

## Module purpose (per VULKAN_SUBSTRATE.md §1.1 + §2.2)

- **Sprite vertex format** (`SpriteVertex` — 20 bytes per S-LOCK-3): pos+uv+tint.
- **Atlas region descriptor** (`AtlasRegion`) — UV rect within texture atlas.
- **Texture handle** (`SpriteTexture`) — VulkanImage + VulkanSampler ownership wrapper.
- **Transform** (`SpriteTransform`) — position + scale + rotation + tint color.
- **Sprite handle** (`Sprite`) — composite record: texture ref + UV region + transform.
- **Pipeline** (`VulkanSpritePipeline`) — extends graphics pipeline pattern с vertex input +
  alpha blending + descriptor sets + push constants.
- **Renderer** (`SpriteRenderer`) — single-sprite-per-draw API (V0.C.1); batched extension
  V0.C.2.

## Dependency rules

Per VULKAN_SUBSTRATE.md §2.4 Rule 1-5: Sprite depends on Graphics (VulkanImage, VulkanSampler,
VulkanGraphicsPipeline pattern) + Assets (loads PngImage за sprites). Sprite does not depend
on Compute или Domain layers.

## Out-of-scope (V0.C.1, deferred к V0.C.2)

- Batched sprite renderer (10,000 sprites at 60+ FPS, single draw call per atlas)
- Camera2D orthographic class с ortho projection × view matrix
- TileMap renderer (200×200 grid)
- Sprite sorting by atlas/material для batch grouping
- Atlas region metadata loading (JSON или code-defined)
