using System.Numerics;
using DualFrontier.Contracts.Core;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Per-pawn scene entry held by <see cref="SceneState"/>. Pure data record
/// carrying pawn identity + UV region + position + scale. К-extensions
/// cascade #3 minimum scope per Q-H-2 LOCKED: no rotation, no tint, no
/// animation state — visual minimum.
/// </summary>
/// <param name="PawnId">Pawn entity identifier from domain.</param>
/// <param name="Region">UV region within atlas texture (deterministic per <paramref name="PawnId"/>).</param>
/// <param name="Position">World-space position в tile-grid units (from PawnSpawnedCommand/PawnMovedCommand).</param>
/// <param name="Scale">Sprite size в pixels (16×16 default per ProceduralAtlas tile dimensions).</param>
public sealed record PawnSpriteEntry(
    EntityId PawnId,
    AtlasRegion Region,
    Vector2 Position,
    Vector2 Scale);
