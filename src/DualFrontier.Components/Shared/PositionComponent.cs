using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Позиция entity в тайловой сетке мира. Pure data.
/// Модификация — только через MovementSystem / TeleportSystem.
/// </summary>
public sealed class PositionComponent : IComponent
{
    // TODO: определить struct GridVector(int X, int Y) в DualFrontier.Components.Shared — Фаза 1
    // TODO: public GridVector Position;
}
