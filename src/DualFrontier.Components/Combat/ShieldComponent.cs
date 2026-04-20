using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Combat;

/// <summary>
/// Магический щит. Слой 2 защиты (GDD 6.4 «Магические Щиты»):
/// сначала поглощает урон пул <c>HpPool</c>, восстанавливается со скоростью
/// <c>Regen</c> (ед./тик). Тип щита (<c>Kind</c>) определяет модификаторы
/// поглощения для разных <c>DamageType</c>.
/// </summary>
public sealed class ShieldComponent : IComponent
{
    // TODO: public float HpPool;
    // TODO: public float Regen;
    // TODO: создать DualFrontier.Components.Combat.ShieldKind enum (Arcane, Kinetic, Void …) — GDD 6.4, Фаза 6.
    // TODO: public ShieldKind Kind;
}
