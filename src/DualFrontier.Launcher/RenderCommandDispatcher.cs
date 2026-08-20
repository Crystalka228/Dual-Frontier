using System;
using System.Numerics;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Contracts.Analyzer;
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

    /// <summary>
    /// Current whole-scene ambient tint: colour in XYZ, strength in W. Written here by
    /// <see cref="AmbientTintCommand"/> dispatch, read by <see cref="LauncherRenderer"/>
    /// when it records the frame. Same single-thread discipline as <see cref="SceneState"/>
    /// (commands drain on the render thread only), so no locking. Default (1,1,1,0) is
    /// strength 0 -- untinted.
    /// </summary>
    public Vector4 AmbientTint { get; private set; } = new Vector4(1f, 1f, 1f, 0f);

    /// <summary>
    /// The per-channel multiplier the ambient tint applies to the whole scene:
    /// lerp(white, tint colour, strength). At strength 0 this is exactly (1,1,1), so every
    /// sprite keeps its white tint and the clear colour is untouched -- that identity IS the
    /// "strength 0 restores the untinted scene" contract, not an approximation of it.
    /// </summary>
    public Vector3 AmbientModulation
    {
        get
        {
            float s = Math.Clamp(AmbientTint.W, 0f, 1f);
            return new Vector3(
                1f + ((AmbientTint.X - 1f) * s),
                1f + ((AmbientTint.Y - 1f) * s),
                1f + ((AmbientTint.Z - 1f) * s));
        }
    }

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
            case AmbientTintCommand cmd: HandleAmbientTint(cmd); break;
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

    /// <summary>
    /// W3/G2 — store the whole-scene tint the renderer applies next frame. Unlike the three
    /// silent-accept handlers below, this arm has real observable behavior: the command is
    /// engine-generic (a colour, no game meaning) and the renderer consumes
    /// <see cref="AmbientModulation"/> every frame. Channels and strength are clamped here so
    /// a mod cannot drive the renderer out of range.
    /// </summary>
    private void HandleAmbientTint(AmbientTintCommand cmd)
    {
        AmbientTint = new Vector4(
            Math.Clamp(cmd.R, 0f, 1f),
            Math.Clamp(cmd.G, 0f, 1f),
            Math.Clamp(cmd.B, 0f, 1f),
            Math.Clamp(cmd.Strength, 0f, 1f));
    }

    // ===========================================================================
    // Silent stubs per S-LOCK-4 amendment (Crystalka mid-cascade ratification 2026-05-23).
    // Defensive throws would crash Launcher в production composition flow (these
    // commands fire actively at startup / every tick / per pawn state change).
    // ===========================================================================

    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Cascade #3 silent stub (Lesson #N12 sub-pattern B) — pending post-Vanilla-mods cascade. " +
        "HUD pawn detail panel (name, needs, mood, job label, top skills) requires Vanilla mods к " +
        "define pawn structure first. Silent accept в production composition per S-LOCK-4 amendment " +
        "(Crystalka mid-cascade ratification 2026-05-23); defensive throw would crash Launcher on " +
        "first tick from PawnStateReporterSystem. " +
        "Activation: HUD pawn detail consumer materialization (M-series migration).")]
    private void HandlePawnState(PawnStateCommand cmd)
    {
        // CASCADE #3 STUB — pending post-Vanilla-mods cascade.
        // HUD pawn detail panel (name, needs, mood, job label, top skills) requires
        // Vanilla mods к define pawn structure first. Silent accept в production
        // composition (PawnStateReporterSystem emits these periodically; defensive
        // throw would crash Launcher on first tick). DO NOT TEST — stub has no
        // observable behavior; tests would lie by passing trivially (Q-H-6 discipline).
    }

    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Cascade #3 silent stub (Lesson #N12 sub-pattern B) — pending post-Vanilla-mods cascade. " +
        "Item visuals require Vanilla mods к define item registry first. Silent accept в production " +
        "composition per S-LOCK-4 amendment (Crystalka mid-cascade ratification 2026-05-23); " +
        "defensive throw would crash Launcher at startup from ~255 GameBootstrap-emitted commands. " +
        "Activation: Item visual consumer materialization (Vanilla-mods cascade).")]
    private void HandleItemSpawned(ItemSpawnedCommand cmd)
    {
        // CASCADE #3 STUB — pending post-Vanilla-mods cascade.
        // Item visuals require Vanilla mods к define item registry first. Silent
        // accept в production composition (GameBootstrap emits ~255 ItemSpawnedCommand
        // at startup для initial food/water/bed/decoration; defensive throw would
        // crash Launcher on first frame). DO NOT TEST.
    }

    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Cascade #3 silent stub (Lesson #N12 sub-pattern B) — pending post-architecture cascade. " +
        "HUD tick label requires HUD primitives which не yet materialized. Silent accept в " +
        "production composition per S-LOCK-4 amendment (Crystalka mid-cascade ratification " +
        "2026-05-23); defensive throw would crash Launcher within milliseconds от GameLoop's " +
        "33ms tick cadence (30 TPS). " +
        "Activation: HUD primitives cascade materialization.")]
    private void HandleTickAdvanced(TickAdvancedCommand cmd)
    {
        // CASCADE #3 STUB — pending post-architecture cascade.
        // HUD tick label requires HUD primitives which не yet materialized. Silent
        // accept в production composition (GameLoop emits this every 33ms at 30 TPS;
        // defensive throw would crash Launcher within milliseconds). DO NOT TEST.
    }
}
