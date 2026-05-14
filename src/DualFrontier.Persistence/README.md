# DualFrontier.Persistence

## Purpose

Compression algorithms for save files. An engine-layer assembly — it does
**not** depend on `Core`, `Systems`, or `Application`; it knows only
`Contracts` (for `EntityId`) and `Components` (for `TerrainKind`,
`TileComponent`, etc.).

No I/O. All work is `byte[] → byte[]` in memory. File writes and integration
with `GameLoop` are wired by `SaveSystem` in `Application` — this package
gives it the underlying codecs.

## Internal layers

```
Snapshots/      ← read-only world snapshots (data only, no logic)
  ├── SaveMeta            — header: version, tick, map size
  ├── TileMapSnapshot     — flat row-major TerrainKind[]
  ├── PawnSnapshot        — position + 5 needs + job
  ├── StorageSnapshot     — capacity + dictionary of stacks
  └── WorldSnapshot       — composition of everything above

Compression/    ← codecs, one file per algorithm
  ├── TileEncoder         — RLE over the tile map
  ├── ComponentEncoder    — quantize float [0..1] to byte
  ├── EntityEncoder       — range encoding over EntityId.Index
  ├── StringPool          — string-tag deduplication
  └── DfCompressor        — facade / entry point
```

## Algorithms

### TileEncoder — RLE

A 100×100 map = 10,000 tiles. Naive byte encoding = 10,000 bytes.

Format: `(count: ushort)(kind: byte)` pairs. A run longer than `ushort.MaxValue`
(65,535) splits into chunks. If 95% of tiles are `Grass`, the size drops to a
few dozen bytes.

```csharp
var bytes = TileEncoder.Encode(snapshot);    // ~30 bytes for a homogeneous biome
var back  = TileEncoder.Decode(bytes, w, h); // round trip restores the original
```

### ComponentEncoder — quantization

`NeedsComponent.Hunger` and similar float fields live in `[0, 1]`.
Quantization into a `byte`:

```csharp
byte q = ComponentEncoder.QuantizeFloat(0.75f);     // 191
float v = ComponentEncoder.DequantizeFloat(q);      // 0.7490..
```

The worst round-trip error is `1/255 ≈ 0.004`, below the resolution any
needs/mood system actually inspects.

Delta encoding between saves (writing only the changed fields with a
`fieldMask: byte` and a value list) is planned as part of the full
`DfCompressor.Compress(WorldSnapshot)` integration in a later phase.

### EntityEncoder — range encoding

`EntityId.Index` grows monotonically; live entities form sparse dense
ranges. Instead of "one int per entity", pairs `(start: int)(count: ushort)`:

```
[1, 2, 3, 4, 5, 10, 11, 12]
  → (start=1, count=5), (start=10, count=3)
  → 12 bytes instead of 32
```

`Version` is lost — saves freeze a moment, and on load every restored entity
gets a fresh version. Internal references inside the save work by `Index`,
not by full `EntityId`.

### StringPool — deduplication

`StorageComponent.Items` uses strings like `iron_ore`, `wood`, `cloth` as
keys; in a large colony the same strings repeat tens of thousands of times.
The pool interns a string on first access and returns a 16-bit `ushort`
handle.

Serialization format: `[count: ushort]` then per string
`[len: ushort][bytes: utf8]`. The pool's capacity is bounded by
`ushort.MaxValue` (65,535) unique strings — on overflow the codec screams
rather than silently losing data.

## Boundary

```
DualFrontier.Persistence
  ├── depends on: Contracts, Components
  └── does NOT depend on: Core, Systems, AI, Application, Presentation
```

This gives three guarantees:
1. Algorithms are tested without the ECS infrastructure.
2. Persistence can be loaded as part of the mod API without dragging in the
   whole engine.
3. The package is reusable in tools outside the main assembly (mod editor,
   save inspector).

## Tests

`tests/DualFrontier.Persistence.Tests/` — four round-trip tests:

- `TileEncoder_RLE_roundtrip` — 100×100 95% Grass + 5% Rock; checks identity
  and `encoded.Length < 100`.
- `ComponentEncoder_quantize_roundtrip` — `0.75f → byte → float`, error
  `< 0.005f`.
- `EntityEncoder_range_roundtrip` — indices `{1..5, 10..12}`; checks identity
  and `encoded.Length < 32`.
- `StringPool_intern_dedup` — a repeated `Intern` returns the same handle
  and does not grow the pool.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-PERSISTENCE
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-PERSISTENCE
---
