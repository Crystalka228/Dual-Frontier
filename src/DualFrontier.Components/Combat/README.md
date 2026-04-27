# Combat

## Purpose
Components of the combat subsystem in the spirit of Combat Extended: weapon
characteristics, armor, magic shields, and ammunition. See GDD 6 "Combat System".

## Dependencies
- `DualFrontier.Contracts` — `IComponent`.

## Contents
- `WeaponComponent.cs` — damage, penetration, damage type, range, accuracy.
- `ArmorComponent.cs` — resistances (sharp / blunt / heat).
- `ShieldComponent.cs` — magic shield, HP pool + regeneration (GDD 6.4).
- `AmmoComponent.cs` — ammo type and count.

## Rules
- `WeaponComponent.Penetration` and `ArmorComponent.*Resist` use a single scale
  (to be documented in `/docs/COMBAT.md`). The check happens in DamageSystem.
- Magic shields are a separate defense layer ON TOP of armor (GDD 6.4).
  The shield's `HpPool` is consumed first, then armor, then HP.
- `AmmoComponent` typically lives on a weapon entity (the magazine) or on the
  pawn (ammo reserve); specifics are settled in InventorySystem.

## Usage examples
```csharp
var rifle = world.CreateEntity();
world.AddComponent(rifle, new WeaponComponent { /* Damage = 18, Range = 25, ... */ });
world.AddComponent(rifle, new AmmoComponent { /* Type = AmmoType.Rifle, Count = 30 */ });
```

## TODO
- [ ] Define the `DamageType` enum (Sharp, Blunt, Heat, Frost, Arcane …) per GDD 6.1.
- [ ] Define the `ShieldKind` enum (Arcane, Kinetic, Void …) per GDD 6.4.
- [ ] Define the `AmmoType` enum (Rifle, Pistol, Shotgun, Mana, Bolt …).
- [ ] Decide where to store weapon durability — in `WeaponComponent` or separately.
