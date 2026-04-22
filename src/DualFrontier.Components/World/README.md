# World

## Назначение
Компоненты для сущностей, представляющих мир: тайлы, эфирные узлы, биомы.
Тайл-сетка — основа карты; эфирные узлы — источники повышенной регенерации
маны (GDD 4).

## Зависимости
- `DualFrontier.Contracts` — `IComponent`.

## Что внутри
- `TileComponent.cs` — тип рельефа + проходимость.
- `EtherNodeComponent.cs` — уровень узла + радиус влияния.
- `BiomeComponent.cs` — тип биома (лес, пустыня, туманное плато и т. п.).

## Правила
- В одной точке сетки живёт только одна entity с `TileComponent`.
- `EtherNodeComponent.Radius` — в тайлах. Перекрывающиеся радиусы
  складываются EtherFieldSystem.
- Биомы влияют на мудовые модификаторы пешек и доступную флору/фауну.

## Примеры использования
```csharp
var node = world.CreateEntity();
world.AddComponent(node, new EtherNodeComponent { /* Tier = 2, Radius = 5 */ });
world.AddComponent(node, new PositionComponent { Position = new GridVector(42, 17) });
```

## TODO
- [ ] Определить `TerrainKind` enum (Grass, Rock, Sand, Water, Ice, Swamp, Arcane …).
- [ ] Определить `BiomeKind` enum (TemperateForest, Desert, Tundra, EtherWastes …).
- [ ] Продумать слои (напольный / стеновой / декоративный) — возможно,
      через отдельные компоненты, а не через `TileComponent`.
