using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Building;

/// <summary>
/// Верстак для крафта. <c>Kind</c> определяет доступные рецепты,
/// <c>WorkSpeed</c> — множитель скорости относительно базовой
/// (1.0 — стандарт, 2.0 — улучшенный, и т. д.).
/// </summary>
public sealed class WorkbenchComponent : IComponent
{
    // TODO: создать DualFrontier.Components.Building.WorkbenchKind enum (Cooking, Smithing, Research, GolemForge …) — Фаза 4.
    // TODO: public WorkbenchKind Kind;
    // TODO: public float WorkSpeed;
}
