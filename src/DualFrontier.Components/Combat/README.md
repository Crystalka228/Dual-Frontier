# Combat

## Назначение
Компоненты боевой подсистемы в духе Combat Extended: характеристики оружия,
брони, магических щитов и боеприпасов. См. GDD 6 «Боевая Система».

## Зависимости
- `DualFrontier.Contracts` — `IComponent`.

## Что внутри
- `WeaponComponent.cs` — урон, пробитие, тип урона, дистанция, точность.
- `ArmorComponent.cs` — сопротивления (острый / тупой / тепловой).
- `ShieldComponent.cs` — магический щит, пул HP + регенерация (GDD 6.4).
- `AmmoComponent.cs` — тип патрона и количество.

## Правила
- `WeaponComponent.Penetration` и `ArmorComponent.*Resist` — единая шкала
  (будет описана в `/docs/COMBAT.md`). Проверка происходит в DamageSystem.
- Магические щиты — отдельный слой защиты ПОВЕРХ брони (GDD 6.4).
  Сначала расходуется `HpPool` щита, затем броня, затем HP.
- `AmmoComponent` обычно живёт на сущности-оружии (магазин) или на пешке
  (запас патронов); конкретика — в InventorySystem.

## Примеры использования
```csharp
var rifle = world.CreateEntity();
world.AddComponent(rifle, new WeaponComponent { /* Damage = 18, Range = 25, ... */ });
world.AddComponent(rifle, new AmmoComponent { /* Type = AmmoType.Rifle, Count = 30 */ });
```

## TODO
- [ ] Определить `DamageType` enum (Sharp, Blunt, Heat, Frost, Arcane …) по GDD 6.1.
- [ ] Определить `ShieldKind` enum (Arcane, Kinetic, Void …) по GDD 6.4.
- [ ] Определить `AmmoType` enum (Rifle, Pistol, Shotgun, Mana, Bolt …).
- [ ] Решить, где хранить прочность оружия — в `WeaponComponent` или отдельно.
