using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: entity <paramref name="Entity"/> is now at (<paramref name="X"/>,
/// <paramref name="Y"/>) in tile-grid units. Renamed from PawnMovedCommand at W4 — see
/// <see cref="EntityAppearedCommand"/> for why the engine's render vocabulary stopped naming
/// game concepts.
/// </summary>
/// <param name="Entity">The entity that moved.</param>
/// <param name="X">New X coordinate (tile-grid units).</param>
/// <param name="Y">New Y coordinate (tile-grid units).</param>
public sealed record EntityMovedCommand(EntityId Entity, float X, float Y) : IRenderCommand;
