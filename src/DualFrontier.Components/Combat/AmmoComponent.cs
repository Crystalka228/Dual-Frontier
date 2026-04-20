using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Combat;

/// <summary>
/// Запас патронов (или аналогичных расходников — болты, заряды маны).
/// Расходуется CombatSystem; пополнение — InventorySystem по двухшаговой
/// модели AmmoIntent → AmmoGranted/AmmoRefused (см. TechArch 11.5).
/// </summary>
public sealed class AmmoComponent : IComponent
{
    // TODO: создать DualFrontier.Components.Combat.AmmoType enum (Rifle, Pistol, Shotgun, Bolt, Mana …) — Фаза 6.
    // TODO: public AmmoType Type;
    // TODO: public int Count;
}
