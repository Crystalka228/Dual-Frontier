using System;
using System.Numerics;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Launcher;

/// <summary>
/// Dispatches drained <see cref="IRenderCommand"/> instances к scene state mutations
/// (consumed by <see cref="LauncherRenderer"/> per frame). К-extensions cascade #3
/// (2026-05-23): real implementations для pawn-3 arms (PawnSpawned/Moved/Died);
/// PawnState/ItemSpawned/TickAdvanced are silent stubs per S-LOCK-4 amendment
/// (Crystalka mid-cascade ratification 2026-05-23 — defensive throws would crash
/// Launcher in production composition flow; see brief §1 S-LOCK-4 amendment narrative).
///
/// Pawn sprite mapping (S-LOCK-2 procedural-only):
/// - Each pawn assigned deterministic tileIndex from PawnId hash → AtlasRegion
///   selected from <see cref="LauncherProceduralAtlas"/> (256 distinct tile types,
///   visually distinct per pawn, reproducible across runs).
/// - World position = tile coord × <see cref="WorldUnitsPerTile"/> (16 px/tile).
///
/// Composition (S-LOCK-10): instance constructed в Program.Main() and passed
/// <see cref="SceneState"/> via constructor injection. No singletons.
/// </summary>
internal sealed class RenderCommandDispatcher
{
    /// <summary>Pixels per tile in world space. Matches ProceduralAtlas tile dimensions.</summary>
    public const float WorldUnitsPerTile = 16f;

    private readonly SceneState _sceneState;

    public RenderCommandDispatcher(SceneState sceneState)
    {
        _sceneState = sceneState ?? throw new ArgumentNullException(nameof(sceneState));
    }

    public void Dispatch(IRenderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        switch (command)
        {
            case PawnSpawnedCommand cmd: HandlePawnSpawned(cmd); break;
            case PawnMovedCommand cmd: HandlePawnMoved(cmd); break;
            case PawnDiedCommand cmd: HandlePawnDied(cmd); break;
            case PawnStateCommand cmd: HandlePawnState(cmd); break;
            case ItemSpawnedCommand cmd: HandleItemSpawned(cmd); break;
            case TickAdvancedCommand cmd: HandleTickAdvanced(cmd); break;
            default:
                throw new NotSupportedException(
                    $"Unknown IRenderCommand type '{command.GetType().FullName}'. " +
                    "Add dispatch arm в RenderCommandDispatcher.Dispatch и accompanying handler.");
        }
    }

    private void HandlePawnSpawned(PawnSpawnedCommand cmd)
    {
        int tileIndex = Math.Abs(cmd.PawnId.GetHashCode()) % LauncherProceduralAtlas.TotalTiles;
        AtlasRegion region = LauncherProceduralAtlas.GetTileRegion(tileIndex);
        _sceneState.RegisterPawn(
            pawnId: cmd.PawnId,
            region: region,
            position: new Vector2(cmd.X, cmd.Y) * WorldUnitsPerTile,
            scale: new Vector2(WorldUnitsPerTile, WorldUnitsPerTile));
    }

    private void HandlePawnMoved(PawnMovedCommand cmd)
    {
        // Silent miss tolerated — domain may emit Moved before Spawned в edge races.
        _sceneState.MovePawn(cmd.PawnId, new Vector2(cmd.X, cmd.Y) * WorldUnitsPerTile);
    }

    private void HandlePawnDied(PawnDiedCommand cmd)
    {
        // Silent miss tolerated — same race tolerance as Moved.
        _sceneState.DespawnPawn(cmd.PawnId);
    }

    // ===========================================================================
    // Silent stubs per S-LOCK-4 amendment (Crystalka mid-cascade ratification 2026-05-23).
    // Defensive throws would crash Launcher в production composition flow (these
    // commands fire actively at startup / every tick / per pawn state change).
    // ===========================================================================

    private void HandlePawnState(PawnStateCommand cmd)
    {
        // CASCADE #3 STUB — pending post-Vanilla-mods cascade.
        // HUD pawn detail panel (name, needs, mood, job label, top skills) requires
        // Vanilla mods к define pawn structure first. Silent accept в production
        // composition (PawnStateReporterSystem emits these periodically; defensive
        // throw would crash Launcher on first tick). DO NOT TEST — stub has no
        // observable behavior; tests would lie by passing trivially (Q-H-6 discipline).
    }

    private void HandleItemSpawned(ItemSpawnedCommand cmd)
    {
        // CASCADE #3 STUB — pending post-Vanilla-mods cascade.
        // Item visuals require Vanilla mods к define item registry first. Silent
        // accept в production composition (GameBootstrap emits ~255 ItemSpawnedCommand
        // at startup для initial food/water/bed/decoration; defensive throw would
        // crash Launcher on first frame). DO NOT TEST.
    }

    private void HandleTickAdvanced(TickAdvancedCommand cmd)
    {
        // CASCADE #3 STUB — pending post-architecture cascade.
        // HUD tick label requires HUD primitives which не yet materialized. Silent
        // accept в production composition (GameLoop emits this every 33ms at 30 TPS;
        // defensive throw would crash Launcher within milliseconds). DO NOT TEST.
    }
}
