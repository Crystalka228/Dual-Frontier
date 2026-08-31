using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: entity <paramref name="Entity"/> has become visible at
/// (<paramref name="X"/>, <paramref name="Y"/>) in tile-grid units. The renderer creates a
/// sprite for it and places it on the scene.
///
/// <para>
/// W4 renamed this from PawnSpawnedCommand. The record's shape never mentioned a pawn — it is an
/// entity id and two coordinates — but its NAME did, and the SDK member that now feeds it must
/// not, because boundary law B-2 keeps gameplay nouns out of engine assemblies. Naming it for
/// what it carries is what let the mod-facing surface stay generic.
/// </para>
/// </summary>
/// <param name="Entity">The entity that appeared.</param>
/// <param name="X">X coordinate (tile-grid units).</param>
/// <param name="Y">Y coordinate (tile-grid units).</param>
public sealed record EntityAppearedCommand(EntityId Entity, float X, float Y) : IRenderCommand;
