using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: a projectile <paramref name="ProjectileId"/> has spawned, flying
/// from (<paramref name="FromX"/>,<paramref name="FromY"/>) to
/// (<paramref name="ToX"/>,<paramref name="ToY"/>). Presentation creates the
/// visual and starts the flight animation.
/// </summary>
/// <param name="ProjectileId">Identifier of the projectile entity in the domain.</param>
/// <param name="FromX">Start X coordinate.</param>
/// <param name="FromY">Start Y coordinate.</param>
/// <param name="ToX">Target X coordinate.</param>
/// <param name="ToY">Target Y coordinate.</param>
public sealed record ProjectileSpawnedCommand(
    EntityId ProjectileId,
    int FromX,
    int FromY,
    int ToX,
    int ToY) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Phase 5 — apply via active IRenderer backend. */
    }
}
