using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Combat;

/// <summary>
/// Ammunition stockpile (or analogous consumables — bolts, mana charges).
/// Consumed by CombatSystem; replenished by InventorySystem via the
/// two-step AmmoIntent → AmmoGranted/AmmoRefused model (see TechArch 11.5).
/// </summary>
[ModAccessible(Read = true)]
public sealed class AmmoComponent : IComponent
{
    // TODO: introduce DualFrontier.Components.Combat.AmmoType enum (Rifle, Pistol, Shotgun, Bolt, Mana …) — Phase 6.
    // TODO: public AmmoType Type;
    // TODO: public int Count;
}
