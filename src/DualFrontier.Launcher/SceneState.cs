using System.Collections.Generic;
using System.Numerics;
using DualFrontier.Contracts.Core;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Scene state container holding pawn sprite registry для К-extensions cascade #3
/// visual implementation. Minimum scope per Q-H-2 LOCKED + S-LOCK-3: sprite registry
/// only, no camera state, no HUD state, no layer ordering, no z-order, no animation,
/// no interpolation, no selection.
///
/// Read/write split:
/// - <see cref="RenderCommandDispatcher"/> writes via <see cref="RegisterPawn"/> /
///   <see cref="MovePawn"/> / <see cref="DespawnPawn"/>.
/// - <see cref="LauncherRenderer"/> reads via <see cref="EnumerateActiveSprites"/>.
///
/// Composition (S-LOCK-10): instance constructed в Program.Main() and passed к both
/// dispatcher (writes via handlers) и renderer (reads per frame) via constructor
/// injection. No singletons, no static state, no global accessors.
///
/// Thread model: drained on the render thread only (PresentationBridge.DrainCommands
/// per-frame на main thread per К-extensions cascade #2 architecture). Single-threaded
/// access discipline — no internal locking needed.
/// </summary>
internal sealed class SceneState
{
    private readonly Dictionary<EntityId, PawnSpriteEntry> _pawnSprites = new();

    public int ActivePawnCount => _pawnSprites.Count;

    public void RegisterPawn(EntityId pawnId, AtlasRegion region, Vector2 position, Vector2 scale)
    {
        _pawnSprites[pawnId] = new PawnSpriteEntry(pawnId, region, position, scale);
    }

    /// <summary>
    /// Update existing pawn's position. Returns true if pawn was registered и updated;
    /// false if pawn not registered (silent miss tolerated — domain may emit Moved before
    /// Spawned в edge race conditions; не fatal).
    /// </summary>
    public bool MovePawn(EntityId pawnId, Vector2 newPosition)
    {
        if (!_pawnSprites.TryGetValue(pawnId, out PawnSpriteEntry? entry))
        {
            return false;
        }
        _pawnSprites[pawnId] = entry with { Position = newPosition };
        return true;
    }

    /// <summary>
    /// Remove pawn from registry. Returns true if pawn was registered и removed;
    /// false if not registered (silent miss tolerated per MovePawn rationale).
    /// </summary>
    public bool DespawnPawn(EntityId pawnId)
    {
        return _pawnSprites.Remove(pawnId);
    }

    public IEnumerable<PawnSpriteEntry> EnumerateActiveSprites()
    {
        return _pawnSprites.Values;
    }
}
