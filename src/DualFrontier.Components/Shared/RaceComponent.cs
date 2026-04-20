using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Раса entity. Влияет на уязвимости (напр. психо-атаки не работают на нежить
/// и големов — см. GDD 6.1), модификаторы нужд и социальные отношения.
/// </summary>
public sealed class RaceComponent : IComponent
{
    // TODO: создать DualFrontier.Components.Shared.RaceKind enum (Human, Undead, Synthetic, Golem …) — Фаза 2
    // TODO: public RaceKind Kind;
}
