using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: entity <paramref name="Entity"/> is no longer visible and its sprite is released.
/// Renamed from PawnDiedCommand at W4 — see <see cref="EntityAppearedCommand"/> for why the
/// engine's render vocabulary stopped naming game concepts. The rename also widens what it can
/// honestly describe: an entity leaving the view is not necessarily an entity dying.
/// </summary>
/// <param name="Entity">The entity that vanished.</param>
public sealed record EntityVanishedCommand(EntityId Entity) : IRenderCommand;
