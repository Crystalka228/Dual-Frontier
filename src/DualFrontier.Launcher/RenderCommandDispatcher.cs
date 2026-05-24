using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;

namespace DualFrontier.Launcher;

/// <summary>
/// Dispatches drained <see cref="IRenderCommand"/> instances к Vulkan primitives
/// (via <see cref="Runtime.Runtime"/>) using pattern matching по concrete
/// command type.
///
/// К-extensions cascade #2 (2026-05-23): Infrastructure scaffold per Q-G-6 (b1).
/// All dispatch arms currently throw <see cref="NotImplementedException"/> с
/// descriptive message linking к Lesson #N12 «Defensive Reserved Stub Pattern»
/// first application. Real visual dispatching lands в К-extensions cascade #3
/// (next session).
///
/// Defensive throws prevent lying tests — test что exercises any visual
/// dispatch path will fail loudly until cascade #3 supplies real implementation.
/// Empty bodies would have passed tests by doing nothing → architectural debt
/// per Lesson #25 refined.
/// </summary>
internal sealed class RenderCommandDispatcher
{
    private readonly Runtime.Runtime _runtime;

    public RenderCommandDispatcher(Runtime.Runtime runtime)
    {
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
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
                    "Add dispatch arm в RenderCommandDispatcher.Dispatch (К-extensions " +
                    "cascade #3 territory) и accompanying handler method below.");
        }
    }

    private void HandlePawnSpawned(PawnSpawnedCommand cmd) =>
        throw new NotImplementedException(
            "PawnSpawned dispatch pending К-extensions cascade #3. " +
            "If this throws в test, the test is exercising visual rendering path " +
            "что cascade #2 explicitly scoped out (Lesson #N12 Defensive Reserved " +
            "Stub Pattern first application). Cascade #3 will implement: create " +
            "sprite at (cmd.X, cmd.Y) using pawn atlas, register в SpriteCatalog " +
            "keyed by cmd.PawnId.");

    private void HandlePawnMoved(PawnMovedCommand cmd) =>
        throw new NotImplementedException(
            "PawnMoved dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: lookup sprite by cmd.PawnId, update position " +
            "к (cmd.X, cmd.Y).");

    private void HandlePawnDied(PawnDiedCommand cmd) =>
        throw new NotImplementedException(
            "PawnDied dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: lookup sprite by cmd.PawnId, play death " +
            "animation, despawn sprite from SpriteCatalog.");

    private void HandlePawnState(PawnStateCommand cmd) =>
        throw new NotImplementedException(
            "PawnState dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: update HUD pawn detail panel (name, needs, " +
            "mood, job label, top skills).");

    private void HandleItemSpawned(ItemSpawnedCommand cmd) =>
        throw new NotImplementedException(
            "ItemSpawned dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: create item visual at (cmd.X, cmd.Y) " +
            "selecting atlas region based on cmd.Kind.");

    private void HandleTickAdvanced(TickAdvancedCommand cmd) =>
        throw new NotImplementedException(
            "TickAdvanced dispatch pending К-extensions cascade #3 (Lesson #N12). " +
            "Cascade #3 will implement: update HUD tick label к cmd.Tick.");
}
