using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Физиологические нужды пешки. Значения 0..1 (или 0..100 — решается в Фазе 2).
/// Деградация — NeedsSystem (SLOW tick). Падение до 0 → мудовый штраф.
/// </summary>
public sealed class NeedsComponent : IComponent
{
    // TODO: public float Hunger;
    // TODO: public float Sleep;
    // TODO: public float Comfort;
}
